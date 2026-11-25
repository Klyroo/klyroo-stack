# API Reference — Klyroo.Stack

Complete public API for `Klyroo.Stack.Core` and `Klyroo.Stack`.

## `record KlyrooStackModule`
*Package*: `Klyroo.Stack.Core` · *Source*: `src/Klyroo.Stack.Core/KlyrooStackCatalog.cs`

Describes one repository/module in the Klyroo ecosystem.

## `class KlyrooStackCatalog`
*Package*: `Klyroo.Stack.Core` · *Source*: `src/Klyroo.Stack.Core/KlyrooStackCatalog.cs`

Central registry and router for the 8 repositories of the Klyroo ecosystem. Resolves any component, headless controller, landing page, template, or skill to its owning repository and package.

| Member | Type | Description |
| :--- | :--- | :--- |
| `Modules` | `IReadOnlyList<KlyrooStackModule>` | — |
| `ResolveComponentModule(string componentOrControllerName)` | `KlyrooStackModule?` | Finds the owning `KlyrooStackModule` for a given component or controller name (case-insensitive). |
| `GetModule(string idOrName)` | `KlyrooStackModule?` | Finds a module by its repository ID (e.g. `"klyroo-table"`) or package name (e.g. `"Klyroo.Table"`). |

## `class KlyrooStackServiceCollectionExtensions`
*Package*: `Klyroo.Stack` · *Source*: `src/Klyroo.Stack/KlyrooStackServiceCollectionExtensions.cs`

| Member | Type | Description |
| :--- | :--- | :--- |
| `AddKlyrooStack(
        this IServiceCollection services,
        Action<KlyrooOptions>? configureUi = null,
        Action<KlyrooTableOptions>? configureTable = null)` | `IServiceCollection` | Registers the entire Klyroo ecosystem in one call: `Klyroo.UI`, `Klyroo.Query`, `Klyroo.Table`, `Klyroo.Form`, `Klyroo.Dashboard`, `Klyroo.Display`, `Klyroo.Spreadsheet`, and `Klyroo.Agent`. Works in Blazor Web App (SSR / Interactive Server / WebAssembly) and .NET MAUI Blazor Hybrid. |
