#!/usr/bin/env bash
# Rebuilds src/Klyroo.Stack/wwwroot/klyroo-stack.css: the Tailwind utilities that the components of the sister libraries use
# (Table, Form, Dashboard, Display, Spreadsheet, Agent, and Klyroo.Stack itself), minus the rules klyroo.css already holds.
# An app that references Klyroo.Stack loads klyroo.css then klyroo-stack.css and needs no Tailwind build of its own.
#
#   build/build-stack-css.sh [path-to-tailwindcss-binary]
#
# The sister repositories are siblings of klyroo-stack. The binary defaults to the one Klyroo.UI's KlyrooTailwind build downloads
# (~/.klyroo/tailwind/4.3.5/), the version the other Klyroo stylesheets are compiled with.
set -euo pipefail
here="$(cd "$(dirname "$0")/.." && pwd)"
parent="$(cd "$here/.." && pwd)"
tw="${1:-$HOME/.klyroo/tailwind/4.3.5/tailwindcss-linux-x64}"
[[ -x "$tw" ]] || { echo "Tailwind binary not found: $tw" >&2; exit 1; }
input="$(mktemp --suffix=.css)"
trap 'rm -f "$input"' EXIT
{
  echo "@layer theme, base, components, utilities;"
  echo "@import 'tailwindcss/theme.css' layer(theme);"
  echo "@import 'tailwindcss/utilities.css' layer(utilities) source(none);"
  for lib in table/src/Klyroo.Table form/src/Klyroo.Form dashboard/src/Klyroo.Dashboard display/src/Klyroo.Display spreadsheet/src/Klyroo.Spreadsheet agent/src/Klyroo.Agent; do
    [[ -d "$parent/klyroo-$lib" ]] || { echo "Missing sibling: $parent/klyroo-$lib" >&2; exit 1; }
    echo "@source '$parent/klyroo-$lib';"
  done
  echo "@source '$here/src/Klyroo.Stack';"
} > "$input"
(cd "$here" && "$tw" -i "$input" -o src/Klyroo.Stack/wwwroot/klyroo-stack.css --minify)
node "$here/build/dedupe-stack-css.mjs"
