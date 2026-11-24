# API Reference — Lux.Stack

Complete public API for `Lux.Stack.Core` and `Lux.Stack`.

## `record LuxStackModule`
*Package*: `Lux.Stack.Core` · *Source*: `src/Lux.Stack.Core/LuxStackCatalog.cs`

Describes one repository/module in the LuxStack ecosystem.

## `class LuxStackCatalog`
*Package*: `Lux.Stack.Core` · *Source*: `src/Lux.Stack.Core/LuxStackCatalog.cs`

Central registry and router for the 8 repositories of the LuxStack ecosystem. Resolves any component, headless controller, landing page, template, or skill to its owning repository and package.

| Member | Type | Description |
| :--- | :--- | :--- |
| `Modules` | `IReadOnlyList<LuxStackModule>` | — |
| `ResolveComponentModule(string componentOrControllerName)` | `LuxStackModule?` | Finds the owning `LuxStackModule` for a given component or controller name (case-insensitive). |
| `GetModule(string idOrName)` | `LuxStackModule?` | Finds a module by its repository ID (e.g. `"lux-table"`) or package name (e.g. `"Lux.Table"`). |

## `class LuxStackServiceCollectionExtensions`
*Package*: `Lux.Stack` · *Source*: `src/Lux.Stack/LuxStackServiceCollectionExtensions.cs`

| Member | Type | Description |
| :--- | :--- | :--- |
| `AddLuxStack(
        this IServiceCollection services,
        Action<BluxOptions>? configureUi = null,
        Action<BluxToolsOptions>? configureTable = null)` | `IServiceCollection` | Registers the entire LuxStack ecosystem in one call: `Lux.UI`, `Lux.Query`, `Lux.Table`, `Lux.Form`, `Lux.Dashboard`, `Lux.Display`, `Lux.Spreadsheet`, and `Lux.Agent`. Works in Blazor Web App (SSR / Interactive Server / WebAssembly) and .NET MAUI Blazor Hybrid. |
