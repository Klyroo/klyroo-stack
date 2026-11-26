#!/usr/bin/env bash
# Checks that the nine Klyroo repositories are ready to release the same version, then (with --tag) creates the
# annotated v<version> tag in each of them. It never pushes: push the tags yourself with the command it prints.
# The release workflow of each repository builds its siblings at that same tag, so every repository needs it.
#
#   build/tag-release.sh          check only
#   build/tag-release.sh --tag    check, then tag
set -euo pipefail

root="$(cd "$(dirname "$0")/../.." && pwd)"
repos=(klyroo-agent klyroo-dashboard klyroo-display klyroo-form klyroo-query klyroo-spreadsheet klyroo-stack klyroo-table klyroo-ui)

read_version() {
  local props="$1/build/Version.props" prefix suffix
  prefix=$(sed -n 's:.*<VersionPrefix>\(.*\)</VersionPrefix>.*:\1:p' "$props")
  suffix=$(sed -n 's:.*<VersionSuffix>\(.*\)</VersionSuffix>.*:\1:p' "$props")
  echo "${prefix}${suffix:+-$suffix}"
}

version=$(read_version "$root/klyroo-stack")
if [[ ! "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+ ]]; then echo "error: cannot read the version of klyroo-stack/build/Version.props" >&2; exit 1; fi
tag="v$version"
fail=0
err() { echo "error: $1: $2" >&2; fail=1; }

for r in "${repos[@]}"; do
  dir="$root/$r"
  if ! git -C "$dir" rev-parse --git-dir >/dev/null 2>&1; then err "$r" "not found next to klyroo-stack"; continue; fi
  [ "$(read_version "$dir")" = "$version" ] || err "$r" "version $(read_version "$dir") is not $version"
  grep -qF "## [$version]" "$dir/CHANGELOG.md" || err "$r" "CHANGELOG.md has no '## [$version]' section"
  [ -z "$(git -C "$dir" status --porcelain)" ] || err "$r" "uncommitted changes"
  [ "$(git -C "$dir" rev-parse --abbrev-ref HEAD)" = main ] || err "$r" "not on main"
  if ! git -C "$dir" fetch --quiet origin main --tags; then err "$r" "cannot fetch origin (or a local tag differs from the remote one)"; continue; fi
  [ "$(git -C "$dir" rev-parse HEAD)" = "$(git -C "$dir" rev-parse origin/main)" ] || err "$r" "main differs from origin/main"
  if git -C "$dir" rev-parse -q --verify "refs/tags/$tag" >/dev/null; then err "$r" "tag $tag already exists"; fi
done

if [ "$fail" != 0 ]; then echo "Not ready to tag $tag." >&2; exit 1; fi
echo "All nine repositories are ready for $tag."
[ "${1:-}" = "--tag" ] || { echo "Run again with --tag to create the tags."; exit 0; }

tagged=()
for r in "${repos[@]}"; do
  if ! git -C "$root/$r" tag -a "$tag" -m "$tag"; then
    echo "error: $r: git tag failed; removing the tags already created so a rerun starts clean" >&2
    for t in "${tagged[@]}"; do git -C "$root/$t" tag -d "$tag" >/dev/null; done
    exit 1
  fi
  tagged+=("$r")
done
echo "Tagged $tag in the nine repositories. Nothing was pushed. To start the releases:"
echo "  for r in ${repos[*]}; do git -C \"$root/\$r\" push origin $tag; done"
