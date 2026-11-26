# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- The `klyroo-saas` and `klyroo-admin` templates are starters now, not one page: a Klyroo.UI shell (a header with navigation, or a resizable sidebar) with `KlyApp`, `KlyFormProvider`, `KlyFilePreviewProvider` and a color mode button, the stock .NET 10 Blazor Web App plumbing (`MapStaticAssets`, exception handler, HSTS, HTTPS redirection, `not-found` and `error` pages, `launchSettings.json`, `appsettings.json`, `.gitignore`), and pages that use each library the documented way: a dashboard with `DashboardController.LoadAsync`, `DashboardPersistence.Attach` and JSON type infos, a `KlyDataList` with a `Provider` (saas) or in-memory rows with selection and a sum row (admin), a `FormService` drawer to create and edit, `KlyDisplay` blocks, and a `KlySpreadsheetImport` (admin). The sample data is in memory.
- `--interactivity Server|WebAssembly|Auto` on both web templates. `WebAssembly` and `Auto` generate a server project, a `.Client` project that holds the pages and the sample data, and a solution file.
- `--product-name` on both web templates, and a product name derived from the project name (`MyShop` becomes "My Shop") instead of the raw namespace.
- The templates reference the Klyroo packages at the version of the `Klyroo.Templates` package: each `template.json` has a constant `KlyrooVersion` symbol, and a pre-pack target writes it from `$(Version)`. Nothing in a template carries a version of its own, and a test checks it.
- Tests of the template sources: each `template.json` names itself and takes its version from the placeholder, no template file has a hard-coded version, and the markup of the saas, admin and MAUI templates only uses classes that a stylesheet it loads defines.
- The MCP server carries the documentation pages of Klyroo.Query, Table, Form, Dashboard, Display, Spreadsheet and Agent (`/<library>/docs/<page>`), searchable with `search-documentation` and readable with `get-documentation-page`.

### Changed

- The `klyroo-maui` layout wraps the body in `KlyApp` with `KlyFormProvider` and `KlyFilePreviewProvider`, its `_Imports.razor` lists the namespaces of the other templates, and the page has a button that opens a form drawer. Its layout classes moved to `wwwroot/app.css`, because `min-h-dvh` and `max-w-3xl` are in none of the precompiled sheets.

### Fixed

- The templates linked `<KlyHead />` in `App.razor`, which renders nothing in a browser (it is the head content of a MAUI web view), so the pages had no theme variables and no color mode script. They use `KlyColorModeScript`, `KlyCultureMeta` and `KlyThemeStyle`, and the documentation says the same.
- `klyroo-stack.css` repeated 222 utility rules that `klyroo.css` already holds. Both sheets put them in one cascade layer, so the later copy won over the rules between them: the items of the header navigation and the search field of a data list were laid out wrongly. The sheet now only holds what `klyroo.css` lacks (41.9 KB to 27.8 KB), `build/dedupe-stack-css.mjs` rewrites it, and a test fails when a shared rule comes back.
- `KlyrooStackCatalog.TemplateShortName` named templates that do not exist; each library now points at the `Klyroo.Templates` template that shows it, and a test keeps it that way.
- `search-documentation` split camelCase words in the pages but not in the query, so a query such as `RegisterForm` found nothing.

- The `klyroo-maui` template generated a project that did not build: it had no `App` class, `MainPage`, `index.html` or platform folders. It is now a complete MAUI Blazor Hybrid app.
- The documentation site no longer lists `klyroo-behaviors.js` in Klyroo.Agent, which removed the module.

## [0.1.0-preview.1] - 2026-11-24

### Added

- `Klyroo.Stack` meta-package for Blazor Web and .NET MAUI Blazor Hybrid: `AddKlyrooStack()` registers Klyroo.UI, Query, Table, Form, Dashboard, Display, Spreadsheet and Agent in one call and accepts the option callbacks of Klyroo.UI and Klyroo.Table.
- `Klyroo.Stack.Core` headless meta-package that references the seven `.Core` engines, marked `IsAotCompatible` and `IsTrimmable`.
- `KlyrooStackCatalog`, which maps a component or controller name to the library, packages, template short name and agent skill that own it.
- `Klyroo.Stack.Mcp`, a `dotnet` tool (`klyroo-stack-mcp`) that serves the Klyroo.UI documentation to MCP clients over stdio: nine read-only tools, two prompts and four resources, with `--tools` and `--site-url` options.
- `Klyroo.Templates` with the `klyroo-saas`, `klyroo-admin`, `klyroo-headless` and `klyroo-maui` project templates.
- `klyroo-stack.css`, a precompiled stylesheet shipped with `Klyroo.Stack`, so apps need neither Node nor Tailwind.
- The `klyroo-stack` agent skill, which routes a coding agent to the skill of each library.

[Unreleased]: https://github.com/klyroo/klyroo-stack/compare/v0.1.0-preview.1...HEAD
[0.1.0-preview.1]: https://github.com/klyroo/klyroo-stack/releases/tag/v0.1.0-preview.1
