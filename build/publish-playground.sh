#!/usr/bin/env bash
# Publishes the Klyroo playground (playground/Klyroo.Playground, a standalone Blazor WebAssembly app) as static files ready for GitHub Pages.
#
#   build/publish-playground.sh <out-dir> [base-path]
#
# base-path is where the files will be served (default /klyroo-stack/playground/, which is https://klyroo.github.io/klyroo-stack/playground/).
# Use / for a server that serves the playground at its root. The script writes:
#   - the app (index.html, _framework/, app.css, the self-hosted fonts, the sample files) with <base href> set to base-path;
#   - 404.html, the GitHub Pages fallback: Pages has no server routes, so an address such as <base-path>table/ lands on 404.html, which sends the
#     route to index.html as ?/table/ and index.html puts the address back before Blazor starts (a reload or a deep link then works);
#   - .nojekyll, so Pages serves the _framework and _klyroo folders as they are.
# The stack Pages workflow can copy <out-dir> to /playground/ next to the showcase it publishes at the root of klyroo-stack.
#
# The sibling repositories (klyroo-ui, klyroo-query, ...) are expected next to klyroo-stack, like for the other projects of this repository.
# The first build downloads the Tailwind CLI, the fonts and the icons and keeps them in ~/.klyroo (set KlyrooOffline=true to forbid the network).
set -euo pipefail

if [[ $# -lt 1 || $# -gt 2 ]]; then
  echo "usage: $0 <out-dir> [base-path]" >&2
  exit 2
fi

here="$(cd "$(dirname "$0")/.." && pwd)"
out="$1"
base="${2:-/klyroo-stack/playground/}"
[[ "$base" == /* ]] || base="/$base"
[[ "$base" == */ ]] || base="$base/"
default_base="/klyroo-stack/playground/"

# Never empty a folder that is not the output of an earlier run.
if [[ -e "$out" ]]; then
  if [[ -n "$(ls -A "$out")" ]]; then
    [[ -f "$out/index.html" && -f "$out/404.html" && -d "$out/_framework" ]] || { echo "$out exists and is not the output of this script: give an empty or a new folder." >&2; exit 1; }
    rm -rf "${out:?}"/* "$out"/.nojekyll
  fi
else
  mkdir -p "$out"
fi
out="$(cd "$out" && pwd)"

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

dotnet publish "$here/playground/Klyroo.Playground/Klyroo.Playground.csproj" -c Release -o "$work" --nologo -v:minimal

[[ -d "$work/wwwroot" ]] || { echo "the publish folder has no wwwroot: the project did not publish as a standalone Blazor WebAssembly app." >&2; exit 1; }
cp -a "$work/wwwroot/." "$out/"
touch "$out/.nojekyll"

# The base path is written in two places: <base href> of index.html and the route restore of 404.html.
for file in index.html 404.html; do
  sed -i "s#${default_base}#${base}#g" "$out/$file"
done

echo "Published the playground to $out for the base path $base"
du -sh "$out" | cut -f1 | sed 's/^/Size on disk: /'
