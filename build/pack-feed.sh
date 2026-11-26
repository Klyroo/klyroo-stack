#!/usr/bin/env bash
# Packs the nine Klyroo repositories into one local NuGet feed, in dependency order, so that a generated app can restore
# every Klyroo.* package from it. The repositories are siblings of klyroo-stack (the layout the projects already need).
#
#   build/pack-feed.sh <feed-folder> [Release|Debug]
set -euo pipefail

feed="${1:?usage: pack-feed.sh <feed-folder> [configuration]}"
configuration="${2:-Release}"
root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
mkdir -p "$feed"
feed="$(cd "$feed" && pwd)"

# ui first: every other library references it. stack last: it references all of them.
for repo in ui query table form dashboard display spreadsheet agent stack; do
  dir="$root/klyroo-$repo"
  slnx="$(ls "$dir"/*.slnx | head -n 1)"
  echo "::group::pack klyroo-$repo"
  dotnet pack "$slnx" -c "$configuration" -o "$feed" --nologo
  echo "::endgroup::"
done

# The templates are not in the solution of the hub: pack them too, so that "dotnet new install Klyroo.Templates" works from the feed.
echo "::group::pack Klyroo.Templates"
dotnet pack "$root/klyroo-stack/templates/Klyroo.Templates/Klyroo.Templates.csproj" -c "$configuration" -o "$feed" --nologo
echo "::endgroup::"

ls "$feed"
