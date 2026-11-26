# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `KlyrooStackModule.AddOnPackages` and `KlyrooStackCatalog` entries for the packages `Klyroo.Stack` does not include: `Klyroo.UI.Server`, `Klyroo.UI.Images`, `Klyroo.UI.Images.Local`, `Klyroo.Query.EntityFrameworkCore`, the new `Klyroo.Form.DataAnnotations` and `Klyroo.Agent.AI` (the agent bridge as `Microsoft.Extensions.AI` functions). The README table "You need / Install", the concepts, platforms and FAQ pages and the installation page of the site list `Klyroo.Form.DataAnnotations` next to `Klyroo.UI.Server`, and a test checks that the hub and the Core meta-package depend on none of them.
- Documentation examples that bind Klyroo.UI inputs to the types of a model: `KlySelect` to an `int` foreign key (and an `int?`), an enum, a `Guid` and a `DateTime`, `KlySelectMenu` to a list of ints, `KlyInputMenu` to an `int?`, `KlyInputNumber` to `int`, `double` and `int?`, `KlyInputDate` to `DateTime` and `DateTimeOffset?`, and `KlySlider` to `int` and `float`; and an example of `KlyChart.OnSelect`.
- The MCP server also carries the hub documentation (`/stack/docs/<page>`: getting started, the templates, the options of every library), and a tenth tool, `resolve-module`, says which library, packages, template and skill own a component or a controller, from `KlyrooStackCatalog`.
- A CI job builds what a new user gets: it packs the nine repositories into a local feed, installs `Klyroo.Templates` from it, generates the saas, admin and headless templates in every interactivity mode, builds them with warnings as errors against that feed alone, requests their pages, and publishes and runs the headless app with NativeAOT (`build/pack-feed.sh`, `build/verify-templates.sh`).
- `build/build-stack-css.sh` rebuilds `klyroo-stack.css` from the sources of the sister libraries with the Tailwind binary of Klyroo.UI, then removes the rules `klyroo.css` already holds.
- The `klyroo-saas` and `klyroo-admin` templates are starters now, not one page: a Klyroo.UI shell (a header with navigation, or a resizable sidebar) with `KlyApp`, `KlyFormProvider`, `KlyFilePreviewProvider` and a color mode button, the stock .NET 10 Blazor Web App plumbing (`MapStaticAssets`, exception handler, HSTS, HTTPS redirection, `not-found` and `error` pages, `launchSettings.json`, `appsettings.json`, `.gitignore`), and pages that use each library the documented way: a dashboard with `DashboardController.LoadAsync`, `DashboardPersistence.Attach` and JSON type infos, a `KlyDataList` with a `Provider` (saas) or in-memory rows with selection and a sum row (admin), a `FormService` drawer to create and edit, `KlyDisplay` blocks, and a `KlySpreadsheetImport` (admin). The sample data is in memory.
- `--interactivity Server|WebAssembly|Auto` on both web templates. `WebAssembly` and `Auto` generate a server project, a `.Client` project that holds the pages and the sample data, and a solution file.
- `--product-name` on both web templates, and a product name derived from the project name (`MyShop` becomes "My Shop") instead of the raw namespace.
- The templates reference the Klyroo packages at the version of the `Klyroo.Templates` package: each `template.json` has a constant `KlyrooVersion` symbol, and a pre-pack target writes it from `$(Version)`. Nothing in a template carries a version of its own, and a test checks it.
- Tests of the template sources: each `template.json` names itself and takes its version from the placeholder, no template file has a hard-coded version, and the markup of the saas, admin and MAUI templates only uses classes that a stylesheet it loads defines.
- The MCP server carries the documentation pages of Klyroo.Query, Table, Form, Dashboard, Display, Spreadsheet and Agent (`/<library>/docs/<page>`), searchable with `search-documentation` and readable with `get-documentation-page`.

### Changed

- **Breaking (documentation, samples, templates):** the templates, samples, site and documentation of this repository follow the strongly typed libraries (no `object` left in their public API except the boundaries listed in each repository), so what users copy is typed. The `klyroo-maui` page opens a `FormSchema<NoteForm>` through `FormService` and reads `outcome.Model`. The `klyroo-saas` and `klyroo-admin` forms are `FormSchema<AccountForm>` and `FormSchema<CustomerForm>` models that `FormService.CreateFormAsync` fills and returns as `outcome.Model`, the admin import is a `SpreadsheetImporter<CustomerRow>` whose `OnSubmit` receives `SpreadsheetRow<CustomerRow>`, the admin dashboard table is a `KlyDashboardTable<TopCustomer>` with `DashboardTableColumn.For` columns, the dashboard and chart enums replace their texts, and the headless template builds a typed `FormSchema`, `SpreadsheetSchema` and `FormController<Config>` under `PublishAot`. `build/verify-templates.sh` packs the nine repositories into a local feed, builds the six generated apps with warnings as errors, runs the five web apps and publishes the headless app with NativeAOT. It also generates `klyroo-maui` and compiles its Razor pages and layout against the feed in a plain Razor project (a stub stands in for `MauiProgram`), which needs no workload; when the `maui-android` workload is installed it builds the whole app for `net10.0-android` too (`SKIP_MAUI_BUILD=1` turns that off). The CI runner has no MAUI workload, so there the platform code, the XAML and the resources of the MAUI template are not built.
- The documentation site follows the typed APIs: the live demos, hero stages, showcase and docs pages build typed forms (`FormSchema.For<T>`, `FormController<T>`, `s.Step`, `s.Section`, `s.List`), typed importers (`SpreadsheetSchema.For<TRow>`), `KlyDataListFieldColumn` and `KlyDataListTemplateColumn`, `DataListGroupBy<T>.By(...)`, `KlyDashboardTable<TRow>`, `KlyChart<TItem, TIndex>` and `KlyColumn<TItem, TValue>`, and the enums that replaced closed sets of text (`ButtonType.Submit`, `InputType.Search`, `KlyElement.Div`, `KlyLinkTarget.Blank`, `KlyPlacement`, `KlyImageFormat.WebP`, `DisplayVariant.Secret`, `HeadingLevel`). The agent pages say that the model cannot write a sensitive field (unless it is named in `allowSensitiveWrites`) or a field of another step. The examples and the docs that were not compiled (snippets in the guides and in the library pages) are rewritten with the same APIs.
- The `KlyrooStackCatalog` descriptions and key components name the typed and Dynamic form types, the three data list column components and the typed Agent bridge types, and the benchmark and the integration tests use the typed schemas. The skill reference of Klyroo.UI (`klyroo-ui/skills/klyroo-ui/references/components.md`) is regenerated.
- The documentation site follows Klyroo.UI, whose select, select menu, input menu, listbox, number, date and slider components are generic: the examples whose `ValueChanged` is a method group or an untyped lambda name the type (`TValue="string"`), and the catalog documents `KlySelect<string>`, `KlyInputNumber<decimal?>` and so on. The component reference of the agent skill is regenerated.
- The XML documentation of `AddKlyrooStack` and the guides say how to configure the options of every library: a later `AddKlyrooDashboard(o => ...)`, `AddKlyrooDisplay(o => ...)` or `AddKlyrooForm(o => ...)` call configures the instances `AddKlyrooStack` registered.
- The `klyroo-maui` layout wraps the body in `KlyApp` with `KlyFormProvider` and `KlyFilePreviewProvider`, its `_Imports.razor` lists the namespaces of the other templates, and the page has a button that opens a form drawer. Its layout classes moved to `wwwroot/app.css`, because `min-h-dvh` and `max-w-3xl` are in none of the precompiled sheets.

### Fixed

- The `klyroo-maui` template still used the removed dictionary `FormSchema`, `FormFieldSchema` and `outcome.FormData["text"]`, so a generated app did not compile. It declares a `NoteForm` and a `FormSchema.For<NoteForm>` and reads `outcome.Model`.
- `resolve-module` lists the add-on packages of a library in its Packages section, and `KlyrooStackCatalog.GetModule` resolves an add-on package name (`Klyroo.Form.DataAnnotations`, `Klyroo.UI.Server`, `Klyroo.Agent.AI`) to its library, as the tool description says.
- `klyroo-stack.css` lacked about fifty classes that the Dashboard, Display and Form components use (`max-h-80`, `bg-current`, `col-span-12`...), so some blocks rendered partly unstyled; it is now rebuilt from their sources.
- This changelog was emptied by a commit made while the disk of the build machine was full; it is restored with the entries of the changes made since.
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
