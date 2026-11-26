#!/usr/bin/env bash
# Proves that the templates work for someone who has only the packages: installs Klyroo.Templates from a local feed, generates the templates, builds them
# against that feed alone (a nuget.config maps Klyroo.* to it), starts the web apps and requests their pages, and publishes the headless app with NativeAOT.
#
#   build/pack-feed.sh <feed>               # once
#   build/verify-templates.sh <feed> <work-folder>
#
# The klyroo-maui template needs the .NET MAUI workload to build as a whole. Its Razor pages and layout do not, so they are always compiled in a plain
# Razor project against the feed (the platform code, XAML and resources are not part of that check). When the maui-android workload is installed the
# whole app is also built for net10.0-android; set SKIP_MAUI_BUILD=1 to skip that build.
#
# Set DOTNET_CLI_HOME and NUGET_PACKAGES to folders of your own to leave the caches of your user alone (the CI job does). AOT_ARGS adds arguments to
# the NativeAOT publish, for example -p:CppCompilerAndLinker=gcc on a machine without clang.
set -euo pipefail

feed="$(cd "${1:?usage: verify-templates.sh <feed> <work-folder>}" && pwd)"
mkdir -p "${2:?usage: verify-templates.sh <feed> <work-folder>}"
work="$(cd "$2" && pwd)"
root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

prefix="$(grep -oP '(?<=<VersionPrefix>)[^<]+' "$root/build/Version.props")"
suffix="$(grep -oP '(?<=<VersionSuffix>)[^<]+' "$root/build/Version.props" || true)"
version="$prefix${suffix:+-$suffix}"

# Every Klyroo.* package comes from the feed and nothing else can supply one; the rest (Microsoft packages) comes from nuget.org.
cat > "$work/nuget.config" <<XML
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="klyroo-local" value="$feed" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="klyroo-local"><package pattern="Klyroo.*" /></packageSource>
    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
  </packageSourceMapping>
</configuration>
XML

cd "$work"
echo "::group::install Klyroo.Templates $version from the feed"
dotnet new uninstall Klyroo.Templates >/dev/null 2>&1 || true
dotnet new install "Klyroo.Templates::$version" --add-source "$feed"
echo "::endgroup::"

generate() { # name, template, extra options
  local name="$1" template="$2"; shift 2
  rm -rf "${work:?}/$name"
  dotnet new "$template" -n "$name" "$@" >/dev/null
  echo "generated $name ($template $*)"
}

build() { # name, project or solution to build, relative to the app folder, extra dotnet build arguments
  echo "::group::build $1"
  dotnet build "$work/$1/$2" -c Release --nologo -warnaserror "${@:3}"
  echo "::endgroup::"
}

port=5400
smoke() { # name, folder of the server project, pages...
  local name="$1" dir="$2"; shift 2
  port=$((port + 1))
  echo "::group::run $name on port $port"
  ( cd "$work/$name/$dir" && ASPNETCORE_ENVIRONMENT=Development exec dotnet run --no-build -c Release --no-launch-profile --urls "http://localhost:$port" ) > "$work/$name.log" 2>&1 &
  local pid=$!
  local up=0
  for _ in $(seq 1 60); do
    if curl -sf -o /dev/null "http://localhost:$port/"; then up=1; break; fi
    sleep 1
  done
  if [[ $up == 0 ]]; then cat "$work/$name.log"; kill $pid 2>/dev/null || true; echo "::error::$name did not start"; return 1; fi
  for page in "$@"; do
    html="$(curl -sf "http://localhost:$port/$page")" || { kill $pid 2>/dev/null || true; echo "::error::$name: GET /$page failed"; return 1; }
    # The page is complete HTML with the stylesheets and an h1, not the error page.
    grep -q '<h1' <<<"$html" && grep -Eq 'klyroo(\.[a-z0-9]+)?\.css' <<<"$html" || { kill $pid 2>/dev/null || true; echo "::error::$name: /$page has no h1 or no Klyroo stylesheet"; return 1; }
    echo "GET /$page ok"
  done
  if grep -qE "^(fail|crit):" "$work/$name.log"; then cat "$work/$name.log"; kill $pid 2>/dev/null || true; echo "::error::$name logged an error"; return 1; fi
  kill $pid 2>/dev/null || true
  wait $pid 2>/dev/null || true
  echo "::endgroup::"
}

generate SaasServer klyroo-saas
generate SaasWasm klyroo-saas --interactivity WebAssembly
generate SaasAuto klyroo-saas --interactivity Auto
generate AdminServer klyroo-admin
generate AdminAuto klyroo-admin --interactivity Auto --product-name "Back Office"
generate Worker klyroo-headless
# Not named MauiApp: the namespace would hide the MauiApp type of MAUI.
generate KlyrooMobile klyroo-maui

build SaasServer SaasServer.csproj
build SaasWasm SaasWasm.slnx
build SaasAuto SaasAuto.slnx
build AdminServer AdminServer.csproj
build AdminAuto AdminAuto.slnx
build Worker Worker.csproj

# The Components folder of the klyroo-maui app (pages, layout, imports) in a Razor project that needs no workload. MauiProgram is the one MAUI type the
# Router refers to, so a stub stands in for it.
echo "::group::compile the Razor pages of KlyrooMobile without the MAUI workload"
rm -rf "${work:?}/MauiPages"
mkdir -p "$work/MauiPages"
cp -r "$work/KlyrooMobile/Components" "$work/MauiPages/Components"
cat > "$work/MauiPages/MauiPages.csproj" <<XML
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>KlyrooMobile</RootNamespace>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Klyroo.Stack" Version="$version" />
  </ItemGroup>
</Project>
XML
printf 'namespace KlyrooMobile;\n\n/// <summary>Stands in for the MAUI entry point that Routes.razor reads the assembly of.</summary>\npublic static class MauiProgram;\n' > "$work/MauiPages/MauiProgram.cs"
dotnet build "$work/MauiPages/MauiPages.csproj" -c Release --nologo -warnaserror
echo "::endgroup::"

workloads="$(dotnet workload list 2>/dev/null || true)"
if [[ "${SKIP_MAUI_BUILD:-}" != 1 ]] && grep -Eq '^maui(-android)? ' <<<"$workloads"; then
  build KlyrooMobile KlyrooMobile.csproj -f net10.0-android
else
  echo "The MAUI workload is not installed (or SKIP_MAUI_BUILD=1): KlyrooMobile is checked through its Razor pages only, not built as a MAUI app."
fi

smoke SaasServer . "" accounts usage
smoke SaasWasm SaasWasm "" accounts usage
smoke SaasAuto SaasAuto "" accounts usage
smoke AdminServer . "" customers import
smoke AdminAuto AdminAuto "" customers import

echo "::group::publish Worker with NativeAOT"
rid="$(dotnet --info | sed -n 's/^ *RID: *//p' | head -n 1)"
# shellcheck disable=SC2086
dotnet publish "$work/Worker/Worker.csproj" -c Release -r "$rid" -o "$work/Worker/publish" --nologo ${AOT_ARGS:-}
output="$("$work/Worker/publish/Worker")"
echo "$output"
grep -q "Klyroo NativeAOT OK" <<<"$output" || { echo "::error::the native binary did not print the expected line"; exit 1; }
echo "::endgroup::"

echo "Templates verified: the saas, admin and headless apps (six projects) generated, built against $feed, the five web apps run and the headless one published with NativeAOT; the klyroo-maui template generated and its Razor pages compiled against the feed (the whole app built for Android only when the MAUI workload is installed)."
