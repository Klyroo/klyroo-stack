# Contributing to klyroo-stack

Thanks for helping build Klyroo. This guide explains how to propose a change to **Klyroo.Stack** and what a pull
request needs before it is merged. Klyroo is a family of independent repositories: each one has its own issues,
tests, versions and releases, so a change usually belongs to exactly one of them.

## Prerequisites

- **.NET SDK 10** (`global.json` pins the feature band and rolls forward). Install from [dot.net](https://dot.net).
- **Node.js 22**, only to regenerate the stylesheet of the documentation site with `npm run site:css`, which uses the Tailwind CLI installed in `../klyroo-ui`.
- Some projects reference sibling Klyroo repositories by relative path (`../klyroo-ui`, ...). Clone the
  repositories you need next to each other:

  ```bash
  mkdir klyroo && cd klyroo
  git clone https://github.com/klyroo/klyroo-stack.git
  # plus every library, which the meta-package, the site and the tests reference:
  for r in ui query table form dashboard display spreadsheet agent; do
    git clone https://github.com/klyroo/klyroo-$r.git
  done
  ```

## Build, test, format

```bash
dotnet build Klyroo.Stack.slnx -c Release
dotnet test Klyroo.Stack.slnx -c Release --no-build
dotnet format Klyroo.Stack.slnx --include src/ tests/   # fix formatting of the files you touched
```

### Browser tests

`tests/Klyroo.Stack.E2E` drives the documentation site (`site/Klyroo.Stack.Site`) and what it serves in a real browser with Playwright for .NET: the component pages of
Klyroo.UI and of the sister libraries in the four render modes (an axe theory per family plus behavior tests), the guides, the `/templates` gallery, and the live
demos of the templates behind the reverse proxy of the docs host. The templates themselves live in `klyroo-ui`, whose `tests/Klyroo.UI.E2E` runs each one on its own;
the checks of a template are compiled from there (`klyroo-ui/tests/Klyroo.UI.E2E/Checks`), so one scenario runs against the template alone and against its demo.
Two classes cover the project templates of this repository: `SaasAppTests` and `AdminAppTests` generate `klyroo-saas` and `klyroo-admin` with `dotnet new` and fill,
validate, submit and cancel their typed account and customer forms (error summary, focus, ranges, radio group, drawer).

Playwright needs a browser, and the generated apps need a feed with the nine repositories packed:

```bash
build/pack-feed.sh /tmp/klyroo-feed                           # once, and again after a change in a library
dotnet build tests/Klyroo.Stack.E2E -c Release                # the site and the nine templates it runs as demos are built first
pwsh tests/Klyroo.Stack.E2E/bin/Release/net10.0/playwright.ps1 install --with-deps chromium
KLYROO_E2E_FEED=/tmp/klyroo-feed dotnet test --project tests/Klyroo.Stack.E2E -c Release --no-build
```

Without PowerShell, use a browser that is already installed: `KLYROO_BROWSER_EXECUTABLE=/usr/bin/google-chrome` (any Chromium-based browser) or `KLYROO_BROWSER_CHANNEL=chrome`;
`KLYROO_BROWSER` picks `chromium` (default), `firefox` or `webkit`. Without `KLYROO_E2E_FEED` the two generated-app classes are skipped on a machine and fail in CI. A class runs
on its own with `-- --filter-class Klyroo.Stack.E2E.SaasAppTests`. The generated apps are built in a temporary folder with a template hive and a package folder of their own,
so nothing of your `dotnet new` templates or global NuGet cache is touched or used. The whole project takes a long time (the component pages alone run in four modes): run
the classes of the family you changed.

The project is in `Klyroo.Stack.slnx`, but a plain `dotnet test Klyroo.Stack.slnx` would run it too, so the CI workflow leaves it out of the quick run
(`--filter-not-namespace Klyroo.Stack.E2E`) and runs it in the `browser-tests` job, on every push to `main` and by hand (Actions, Run workflow); a pull request does not wait for it.

The build treats warnings as errors and nullable reference types are enabled everywhere: fix a warning instead
of suppressing it, and justify any `#pragma` or `[SuppressMessage]` in the code.

## Documentation sites

The README and `docs/` of every library are published as a site at `https://klyroo.github.io/<repo>/`. The site is built here, in `docs-site/Klyroo.Docs.Site`, from the files of the library as they are, and each library publishes it with its own `.github/workflows/docs.yml`. See [`docs-site/README.md`](docs-site/README.md) for the settings (`docs/site.yml`), the links and the export.

Preview a library from a folder that holds this repository, `klyroo-ui` and the library:

```bash
dotnet run -c Release --project klyroo-stack/docs-site/Klyroo.Docs.Site -- --repo klyroo-table   # http://localhost:5000/
```

Write the static site the way GitHub Pages will serve it (the export fails on a broken link, anchor or asset):

```bash
dotnet run -c Release --project klyroo-stack/docs-site/Klyroo.Docs.Site -- export --repo klyroo-table --out preview/klyroo-table
python3 -m http.server 8000 --directory preview   # http://localhost:8000/klyroo-table/
```

Change the look or the behavior of every site in `docs-site/`, and the content, the order of the pages and the links of one site in the repository of that library.

## Workflow

1. **Discuss first** for anything larger than a bug fix: open an issue (or comment on an existing one) so the
   design is agreed before you write code.
2. **Fork and branch** from `main`. Name the branch after the change: `feat/<short-topic>`, `fix/<short-topic>`,
   `docs/<short-topic>`, `refactor/...`, `test/...`, `ci/...` or `chore/...`.
3. **Commit** with [Conventional Commits](https://www.conventionalcommits.org/):

   ```text
   <type>(<optional scope>): <summary in the imperative, lower case, no period>

   <body: what and why, wrapped at 100 columns>

   BREAKING CHANGE: <what breaks and how to migrate>   # only when it applies
   ```

   Types: `feat`, `fix`, `docs`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`. A `!` after the type
   (`feat!:`) or a `BREAKING CHANGE:` footer marks a breaking change.
4. **Open a pull request** against `main` and fill in the template.

## What a pull request needs

- **One concern per pull request.** Unrelated refactors or reformatting go in their own pull request.
- **Tests.** A bug fix comes with a test that fails without it; a feature comes with tests of its public
  behavior. Never delete or disable a test to make a build pass: fix the cause, or explain in the pull request
  why the test was wrong.
- **Public API.** Every public type and member has XML documentation. A change to a public API is called out in
  the pull request; a breaking one needs a `BREAKING CHANGE:` footer and an entry in `CHANGELOG.md`.
- **Documentation.** Update `README.md` and `docs/` when behavior or usage changes. Examples must compile against
  the real API: never document an API that does not exist.
- **Changelog.** Add a line under `## [Unreleased]` in `CHANGELOG.md` for any user-visible change.
- **CI is green**: restore, build, tests, NativeAOT/trimming analyzers where they apply, and packaging.

## Review

A maintainer reviews every pull request. Expect questions about the public API, render modes (Static SSR,
Interactive Server, WebAssembly, MAUI Blazor Hybrid), accessibility and NativeAOT compatibility. At least one
maintainer approval and a green CI are required; the pull request is squash-merged, so its title must be a valid
Conventional Commit.

## Versioning and releases

Klyroo.Stack follows [Semantic Versioning](https://semver.org/). While the version is `0.x`, minor versions may contain
breaking changes, always listed in the changelog. The version lives in `build/Version.props`
(`VersionPrefix` / `VersionSuffix`) and is the same in the nine Klyroo repositories, which release together.
Maintainers release by bumping it in each of them, moving the `Unreleased` changelog entries under the new version,
and running `build/tag-release.sh` from klyroo-stack: it checks that every repository is clean, on an up-to-date
`main`, at that version and with a changelog section for it, then `--tag` creates the `v<version>` tags without
pushing them. Pushing the tags starts the release workflows. Each one waits for the same tag in the sibling
repositories and builds them at that tag, checks that the tag matches the version, builds, tests, packs, and waits
for approval before publishing to NuGet and creating the GitHub Release.

## Issues

- **Bug**: use the bug template, with the package version, the render mode or platform, and a minimal reproduction.
- **Feature**: use the feature template and describe the problem before the solution.
- **Security**: never in a public issue; follow [SECURITY.md](./SECURITY.md).
- **Question**: use GitHub Discussions.

By contributing you agree that your contribution is licensed under the [MIT License](./LICENSE) and that you
follow the [Code of Conduct](./CODE_OF_CONDUCT.md).

## Rebuild klyroo-stack.css

`src/Klyroo.Stack/wwwroot/klyroo-stack.css` holds the Tailwind utilities that the components of the sister libraries use, minus the rules `klyroo.css` already holds, so an app that references Klyroo.Stack needs no Tailwind build. Rebuild it after a component of Table, Form, Dashboard, Display, Spreadsheet or Agent changes its classes, with the sister repositories cloned next to this one:

```bash
build/build-stack-css.sh
```

It runs the Tailwind 4.3.5 binary that Klyroo.UI's `KlyrooTailwind` build downloads (pass another path as the first argument), then `build/dedupe-stack-css.mjs`.
