# Klyroo.Stack API reference

> Generated from the compiled assemblies and their XML documentation. Do not edit by hand.

## Klyroo.Stack.Core

Static classes and extensions (1) · Records (1)

### Static classes and extensions

#### `KlyrooStackCatalog`

`Klyroo.Stack` · service

The registry of the eight libraries of the Klyroo ecosystem. It maps a component or controller name to the library, packages, `dotnet new` short name and agent skill that own it.

| Property | Type | Description |
| :--- | :--- | :--- |
| `static Modules` | `IReadOnlyList<KlyrooStackModule>` | One entry for each of the eight libraries of the Klyroo ecosystem. |

| Method | Returns | Description |
| :--- | :--- | :--- |
| `static GetModule(string idOrName)` | `KlyrooStackModule?` | Finds a module by its repository ID (e.g. `"klyroo-table"`) or package name (e.g. `"Klyroo.Table"`). |
| `static ResolveComponentModule(string componentOrControllerName)` | `KlyrooStackModule?` | Finds the owning `KlyrooStackModule` for a given component or controller name (case-insensitive). |

### Records

#### `KlyrooStackModule`

`Klyroo.Stack` · record

Describes one repository/module in the Klyroo ecosystem. `KlyrooStackModule.RepositoryPath`, `KlyrooStackModule.SolutionFile` and `KlyrooStackModule.SkillPath` are relative to the folder that holds the sibling Klyroo clones (for example `klyroo-ui/Klyroo.UI.slnx`).

| Constructor | Description |
| :--- | :--- |
| `KlyrooStackModule(string Id, string Name, string RepositoryPath, string SolutionFile, string? CorePackage, string UiPackage, string TemplateShortName, string SkillPath, string Summary, IReadOnlyList<string> KeyComponents, string AccentHex = "#ffd23f", string LandingPagePath = "")` | Describes one repository/module in the Klyroo ecosystem. `KlyrooStackModule.RepositoryPath`, `KlyrooStackModule.SolutionFile` and `KlyrooStackModule.SkillPath` are relative to the folder that holds the sibling Klyroo clones (for example `klyroo-ui/Klyroo.UI.slnx`). |

| Property | Type | Description |
| :--- | :--- | :--- |
| `AccentHex` (init) | `string` | The accent color of the library, as a hex string. |
| `CorePackage` (init) | `string?` | The headless package of the library, or null when it has none. |
| `Id` (init) | `string` | The repository name, such as `klyroo-table`. |
| `KeyComponents` (init) | `IReadOnlyList<string>` | The main public types and components of the library, used by `KlyrooStackCatalog.ResolveComponentModule`. |
| `LandingPagePath` (init) | `string` | The path of the library page on the documentation site, such as `/table`. |
| `Name` (init) | `string` | The library name, such as `Klyroo.Table`. |
| `RepositoryPath` (init) | `string` | The repository folder, relative to the folder that holds the sibling clones. |
| `SkillPath` (init) | `string` | The agent skill file of the repository, relative to the folder that holds the sibling clones. |
| `SolutionFile` (init) | `string` | The solution file of the repository, relative to the same folder. |
| `Summary` (init) | `string` | A one-sentence description of what the library offers. |
| `TemplateShortName` (init) | `string` | The short name the catalog records for the `dotnet new` template of the library. |
| `UiPackage` (init) | `string` | The Blazor package of the library. |

## Klyroo.Stack

Static classes and extensions (1)

### Static classes and extensions

#### `KlyrooStackServiceCollectionExtensions`

`Klyroo.Stack` · service

Dependency injection extensions of the `Klyroo.Stack` meta-package.

| Method | Returns | Description |
| :--- | :--- | :--- |
| `static AddKlyrooStack(IServiceCollection services, Action<KlyrooOptions>? configureUi = null, Action<KlyrooTableOptions>? configureTable = null)` | `IServiceCollection` | Registers the entire Klyroo ecosystem in one call: `Klyroo.UI`, `Klyroo.Query`, `Klyroo.Table`, `Klyroo.Form`, `Klyroo.Dashboard`, `Klyroo.Display`, `Klyroo.Spreadsheet`, and `Klyroo.Agent`. Works in Blazor Web App (SSR / Interactive Server / WebAssembly) and .NET MAUI Blazor Hybrid. In an app with a WebAssembly client, call it in the server and in the client `Program.cs` with the same configuration. |
