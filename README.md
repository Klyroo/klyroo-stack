# Klyroo.Stack

The hub of the Klyroo ecosystem: one meta-package that registers every Klyroo library, an MCP server that gives coding agents the Klyroo.UI documentation, and `dotnet new` templates, for .NET developers who build Blazor and .NET MAUI Blazor Hybrid apps.

[![CI](https://github.com/klyroo/klyroo-stack/actions/workflows/ci.yml/badge.svg)](https://github.com/klyroo/klyroo-stack/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/vpre/Klyroo.Stack.svg)](https://www.nuget.org/packages/Klyroo.Stack)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Klyroo is a family of independent .NET libraries, each in its own repository with its own versions. This repository ties them together: `Klyroo.Stack` references the Blazor package of all eight libraries and registers them with one call, `Klyroo.Stack.Core` does the same for their headless engines, and the templates and the MCP server start a project or an AI session with the right packages and documentation. You can use the hub, or ignore it and install one library on its own.

## Features

- **One registration call**: `services.AddKlyrooStack()` registers Klyroo.UI, Query, Table, Form, Dashboard, Display, Spreadsheet and Agent, and accepts the option callbacks of Klyroo.UI and Klyroo.Table.
- **Headless meta-package**: `Klyroo.Stack.Core` references the seven `.Core` engines, none of which depends on Razor or ASP.NET Core, and is marked `IsAotCompatible` and `IsTrimmable`.
- **Component catalog**: `KlyrooStackCatalog` maps a component or controller name such as `KlyDataList<Order>` to the library, the packages, the template short name and the agent skill that own it.
- **MCP documentation server**: the `klyroo-stack-mcp` tool answers over stdio, offline, with the components, parameters, events, slots, theme slots, example sources and guides of the Klyroo.UI version it was built with.
- **Project templates**: `Klyroo.Templates` adds four `dotnet new` templates: `klyroo-saas`, `klyroo-admin`, `klyroo-headless` and `klyroo-maui`.
- **Precompiled stylesheet**: `Klyroo.Stack` ships `klyroo-stack.css`, so an app needs neither Node nor Tailwind to style the components.
- **Agent skill**: `skills/klyroo-stack/SKILL.md` routes a coding agent to the skill of each library.
- **Documentation site source**: `site/` holds the Blazor app that documents the ecosystem. It is not deployed yet and runs locally.

## Packages

| Package | Description |
| :--- | :--- |
| [`Klyroo.Stack`](https://www.nuget.org/packages/Klyroo.Stack) | Meta-package for Blazor Web and .NET MAUI Blazor Hybrid: the Blazor package of all eight libraries, `AddKlyrooStack()` and `klyroo-stack.css` |
| [`Klyroo.Stack.Core`](https://www.nuget.org/packages/Klyroo.Stack.Core) | Headless meta-package: the seven `.Core` engines and `KlyrooStackCatalog`, safe for NativeAOT and trimming |
| [`Klyroo.Stack.Mcp`](https://www.nuget.org/packages/Klyroo.Stack.Mcp) | `dotnet` tool that runs the MCP documentation server (`klyroo-stack-mcp`) |
| [`Klyroo.Templates`](https://www.nuget.org/packages/Klyroo.Templates) | `dotnet new` templates that start a project with the meta-package |

### Choosing a package

Install the hub when you want every library with one registration call, or when you start a new app. Install a single library when you only need one concern: `Klyroo.Query` has no UI dependency, and `Klyroo.Agent` depends on the `.Core` packages of Form, Dashboard and Spreadsheet but not on Klyroo.UI.

| You need | Install |
| :--- | :--- |
| Every library in a Blazor or MAUI Blazor Hybrid app | `Klyroo.Stack` |
| Query, form, dashboard, import or display logic in a console app, a worker or a NativeAOT binary | `Klyroo.Stack.Core`, or one `Klyroo.<Library>.Core` package |
| One library with its components | The `Klyroo.<Library>` package, for example `Klyroo.Table` (it brings Klyroo.UI and Klyroo.Query) |
| EF Core execution of Klyroo.Query | `Klyroo.Query.EntityFrameworkCore`, which `Klyroo.Stack` does not include |
| Server-only helpers or the image pipeline of Klyroo.UI | `Klyroo.UI.Server` and `Klyroo.UI.Images`, which `Klyroo.Stack` does not include |
| Your coding agent to know the Klyroo.UI API | `Klyroo.Stack.Mcp` |

## Installation

### The meta-package

```bash
dotnet add package Klyroo.Stack --prerelease
```

Register the libraries in `Program.cs` (or `MauiProgram.cs`), load the stylesheets, and import the namespaces you use:

```csharp
using Klyroo.Stack;

builder.Services.AddKlyrooStack();
```

```razor
<link rel="stylesheet" href="@Assets["_content/Klyroo.UI/klyroo.preflight.css"]" />
<link rel="stylesheet" href="@Assets["_content/Klyroo.UI/klyroo.css"]" />
<link rel="stylesheet" href="@Assets["_content/Klyroo.Stack/klyroo-stack.css"]" />
<KlyHead />
```

In an app with a WebAssembly client, call `AddKlyrooStack()` in the server and in the client `Program.cs` with the same options. [Getting started](docs/getting-started.md) walks through a complete project.

### Individual libraries

Each library has its own registration method and its own repository. `AddKlyrooTable()` also registers Klyroo.UI and Klyroo.Query, and `AddKlyrooForm()`, `AddKlyrooDashboard()`, `AddKlyrooDisplay()` and `AddKlyrooSpreadsheet()` also register Klyroo.UI, so you can call only the ones you use:

```bash
dotnet add package Klyroo.Table --prerelease
dotnet add package Klyroo.Form --prerelease
```

```csharp
using Klyroo.Form;
using Klyroo.Table;

builder.Services.AddKlyrooTable();
builder.Services.AddKlyrooForm();
```

Each Blazor package ships its own stylesheet next to the Klyroo.UI one when it has components (for example `_content/Klyroo.Table/klyroo-table.css`). The headless engines install the same way: `dotnet add package Klyroo.Stack.Core --prerelease`.

### The MCP server

```bash
dotnet tool install --global Klyroo.Stack.Mcp --prerelease
claude mcp add --transport stdio klyroo -- klyroo-stack-mcp
```

The server covers the components of Klyroo.UI and the Klyroo.Table components that the documentation site describes. It does not document Klyroo.Form, Dashboard, Display, Spreadsheet or Agent: use the skill of each repository for those. See the [package page](src/Klyroo.Stack.Mcp/README.md) for the tools, prompts, resources, options and client configuration.

### Project templates

```bash
dotnet new install Klyroo.Templates::0.1.0-preview.1
```

| Short name | Creates |
| :--- | :--- |
| `klyroo-saas` | Blazor Web App (Interactive Server) with `AddKlyrooStack()`, the stylesheets, a stat card and a form drawer |
| `klyroo-admin` | Same host with a dashboard page, a data list with a sum row, a form drawer and a CSV preview |
| `klyroo-headless` | Console app with `PublishAot` that uses `Klyroo.Stack.Core` only |
| `klyroo-maui` | .NET MAUI Blazor Hybrid starting point. It is incomplete in this preview, see [Platforms](docs/platforms.md#net-maui-blazor-hybrid) |

```bash
dotnet new klyroo-saas -n MyShop
```

The three web and console templates were generated, built and run (the headless one also published with NativeAOT) when this documentation was written.

### Run the documentation site locally

The site in `site/` is a Blazor Web App (Interactive Server and WebAssembly). It references the sibling repositories by relative path, so clone `klyroo-ui`, `klyroo-query`, `klyroo-table`, `klyroo-form`, `klyroo-dashboard`, `klyroo-display`, `klyroo-spreadsheet` and `klyroo-agent` next to this repository, then:

```bash
dotnet run --project site/Klyroo.Stack.Site
```

The site also hosts live demos of templates as child processes. Set `Klyroo__Templates__Enabled=false` to start it without them.

## Quick start

Headless first. `Klyroo.Stack.Core` brings the query engine of Klyroo.Query, so you can filter, sort and page data without any UI:

```csharp
using Klyroo.Query;
using Klyroo.Stack;

var module = KlyrooStackCatalog.ResolveComponentModule("KlyDataList<Product>")!;
Console.WriteLine($"{module.Name}: {module.CorePackage} + {module.UiPackage}");

var schema = DataListSchema.For<Product>(s => s
    .RowKey(p => p.Id)
    .Search(p => p.Name)
    .Field(p => p.Name, f => f.Key("name").Sortable())
    .Field(p => p.Price, f => f.Key("price").Sortable()));

var request = new DataListRequest(
    DataListPaging.Offset.ForPage(1, 10),
    [new DataListSort("price", Descending: true)],
    DataListFilterGroup.Empty);

var result = schema.ExecuteInMemory(
    [new Product(1, "Keyboard", 120m), new Product(2, "Monitor", 350m), new Product(3, "Mouse", 40m)],
    request);

Console.WriteLine($"{result.TotalCount} products, most expensive first: {result.Items[0].Name}");

sealed record Product(int Id, string Name, decimal Price);
```

This prints `Klyroo.Table: Klyroo.Table.Core + Klyroo.Table` and `3 products, most expensive first: Monitor`.

Then Blazor. With `AddKlyrooStack()` registered and the stylesheets loaded, wrap your router in `<KlyApp>` and use the components:

```razor
@page "/"
@inject FormService Forms

<KlyDashboardStat Label="Active accounts" Value="128" Compare="112" />
<KlyButton Label="New account" Color="KlyColor.Primary" OnClick="OpenFormAsync" />

@code {
    private Task OpenFormAsync() => Forms.CreateFormAsync(new FormSchema
    {
        FormKey = "new-account",
        Title = "New account",
        Fields = [new FormFieldSchema { Key = "name", Label = "Company name", Required = true }],
    }, mode: FormShellMode.Drawer);
}
```

The page needs `<KlyFormProvider />` inside `<KlyApp>` to show the drawer, as in the templates.

## Documentation

| | |
| :--- | :--- |
| [Getting started](docs/getting-started.md) | Step-by-step first integration |
| [Guides](docs/guides.md) | Task-oriented recipes |
| [Concepts](docs/concepts.md) | How the library works |
| [Platforms](docs/platforms.md) | Render modes, .NET MAUI, NativeAOT |
| [API reference](docs/api-reference.md) | Every public type and member |
| [FAQ](docs/faq.md) | Troubleshooting and limitations |

## Requirements

- .NET 10 SDK. `global.json` pins SDK 10.0.100 and rolls forward to a newer major.
- Blazor render modes: Klyroo.UI documents Static SSR, Interactive Server, WebAssembly and Auto. The hub site runs Interactive Server and WebAssembly.
- .NET MAUI Blazor Hybrid: `Klyroo.Stack` declares `browser`, `ios`, `android`, `maccatalyst` and `windows` as supported platforms. According to the Klyroo.UI documentation only Android has been run on an emulator so far.
- NativeAOT and trimming: `Klyroo.Stack.Core` is `IsAotCompatible` and `IsTrimmable`. `Klyroo.Stack` is `IsTrimmable` but not declared AOT compatible.
- The tools and templates need the .NET 10 runtime or newer.

## Klyroo ecosystem

Klyroo is a family of independent .NET libraries that compose. Each one has its own repository, versions and releases.

| Library | Description |
| :--- | :--- |
| [Klyroo.UI](https://github.com/klyroo/klyroo-ui) | Accessible Blazor and .NET MAUI Hybrid components styled with Tailwind CSS v4 |
| [Klyroo.Query](https://github.com/klyroo/klyroo-query) | Typed query model, URL codecs and IQueryable / EF Core execution |
| [Klyroo.Table](https://github.com/klyroo/klyroo-table) | Data lists and trees built on Klyroo.Query |
| [Klyroo.Form](https://github.com/klyroo/klyroo-form) | Schema-driven forms, validation and wizards |
| [Klyroo.Dashboard](https://github.com/klyroo/klyroo-dashboard) | Analytical dashboards with filters, comparisons and KPI blocks |
| [Klyroo.Display](https://github.com/klyroo/klyroo-display) | Read-only display blocks, media players and file preview |
| [Klyroo.Spreadsheet](https://github.com/klyroo/klyroo-spreadsheet) | CSV and TSV import with column mapping and validation |
| [Klyroo.Agent](https://github.com/klyroo/klyroo-agent) | Bridge that lets AI agents read and act on a Blazor UI |
| [**Klyroo.Stack**](https://github.com/klyroo/klyroo-stack) | Meta-package, MCP server and `dotnet new` templates |

## Contributing

Contributions are welcome. Read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request, and follow the
[code of conduct](CODE_OF_CONDUCT.md). Report security issues privately as described in [SECURITY.md](SECURITY.md).

## License

[MIT](LICENSE)
