# Guides

Task-oriented recipes for the hub. Each one states its goal, shows complete code and explains the part that is not obvious. For the first project, start with [Getting started](getting-started.md).

- [Register only the libraries you use](#register-only-the-libraries-you-use)
- [Share options between the server and the WebAssembly client](#share-options-between-the-server-and-the-webassembly-client)
- [Find which library owns a component](#find-which-library-owns-a-component)
- [Test an app that uses the stack](#test-an-app-that-uses-the-stack)
- [Let an agent fill a form](#let-an-agent-fill-a-form)
- [Connect a coding agent to the Klyroo.UI documentation](#connect-a-coding-agent-to-the-klyrooui-documentation)
- [Start a project from a template](#start-a-project-from-a-template)
- [Style a generated app](#style-a-generated-app)
- [Publish the headless template with NativeAOT](#publish-the-headless-template-with-nativeaot)
- [Run the MCP server and the documentation site from source](#run-the-mcp-server-and-the-documentation-site-from-source)

## Register only the libraries you use

Goal: keep the dependency list and the registrations to what the app needs.

Install the packages of the libraries you use instead of `Klyroo.Stack`, then call their registration methods:

```bash
dotnet add package Klyroo.Table --prerelease
dotnet add package Klyroo.Form --prerelease
```

```csharp
using Klyroo.Form;
using Klyroo.Table;

builder.Services.AddKlyrooTable();   // also registers Klyroo.UI and Klyroo.Query
builder.Services.AddKlyrooForm();    // also registers Klyroo.UI
```

`AddKlyrooTable()` calls `AddKlyrooUi()` and `AddKlyrooQuery()` itself, and `AddKlyrooForm()`, `AddKlyrooDashboard()`, `AddKlyrooDisplay()` and `AddKlyrooSpreadsheet()` call `AddKlyrooUi()`. Calling `AddKlyrooUi` more than once is safe: a later call configures the same options object. `AddKlyrooQuery()` and `AddKlyrooAgent()` do not register Klyroo.UI, because those libraries do not depend on it.

`AddKlyrooStack()` is exactly this list for all eight libraries. The only reason to prefer it is that you do not have to keep the list in step with the packages you reference.

## Share options between the server and the WebAssembly client

Goal: give the server and the WebAssembly client the same Klyroo options, so components look the same before and after the client takes over.

Put the registration in one method of a project that both reference, and call it from both `Program.cs` files:

```csharp
using Klyroo.Stack;
using Microsoft.Extensions.DependencyInjection;

public static class KlyrooSetup
{
    // Shared by the server and the client Program.cs, so both register the same options.
    public static IServiceCollection AddAppKlyroo(this IServiceCollection services) =>
        services.AddKlyrooStack(configureUi: ui => ui.Button.Slot("base", "font-semibold"));
}
```

```csharp
// Program.cs of the server project and of the .Client project
builder.Services.AddAppKlyroo();
```

`AddKlyrooStack` takes two callbacks: `configureUi` receives `KlyrooOptions` (the themes and settings of Klyroo.UI) and `configureTable` receives `KlyrooTableOptions` (the themes and messages of Klyroo.Table). Both objects freeze once components start resolving themes and texts (Klyroo.Table messages throw `InvalidOperationException` when changed after that), so configure them in the registration call, never later.

## Find which library owns a component

Goal: route a component or controller name to its library, packages and template, for a tool, a generator or a help command.

`KlyrooStackCatalog` lives in `Klyroo.Stack.Core`, so it works in a console app:

```csharp
using Klyroo.Stack;

foreach (var module in KlyrooStackCatalog.Modules)
{
    Console.WriteLine($"{module.Id,-20} {module.UiPackage,-22} {module.CorePackage ?? "-"}");
}

var owner = KlyrooStackCatalog.ResolveComponentModule("KlyDataListTable<Order>");
Console.WriteLine(owner?.Id);                       // klyroo-table
Console.WriteLine(KlyrooStackCatalog.GetModule("Klyroo.Form.Core")?.Name);   // Klyroo.Form
```

`ResolveComponentModule` ignores generic arguments and case, and returns `null` for a name that no module lists. It only knows the names in each module's `KeyComponents`, which is a short list of the main types, not every component. `GetModule` accepts a repository id (`klyroo-table`), a library name (`Klyroo.Table`) or either package name.

## Test an app that uses the stack

Goal: verify a registration, a component and a catalog lookup with xUnit and bUnit.

Create a test project with the Razor SDK so bUnit can render components. These are the package versions this repository's own tests use:

```xml
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Exe</OutputType>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="xunit.v3" Version="4.0.3" />
    <PackageReference Include="bunit" Version="2.15.2" />
    <PackageReference Include="Klyroo.Stack" Version="0.1.0-preview.1" />
  </ItemGroup>
</Project>
```

```csharp
using Bunit;
using Klyroo.Agent;
using Klyroo.Dashboard;
using Klyroo.Form;
using Klyroo.Query;
using Klyroo.Stack;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Shop.Tests;

public sealed class StackTests : BunitContext
{
    [Fact]
    public void Catalog_resolves_a_component_to_the_library_that_owns_it()
    {
        var module = KlyrooStackCatalog.ResolveComponentModule("KlyDataList<Product>");

        Assert.Equal("klyroo-table", module?.Id);
        Assert.Equal("Klyroo.Table.Core", module?.CorePackage);
        Assert.Null(KlyrooStackCatalog.ResolveComponentModule("NotAComponent"));
    }

    [Fact]
    public void AddKlyrooStack_registers_the_scoped_services_of_every_library()
    {
        Services.AddKlyrooStack();

        Assert.NotNull(Services.GetRequiredService<QueryState>());
        Assert.NotNull(Services.GetRequiredService<FormService>());
        Assert.NotNull(Services.GetRequiredService<AgentBridgeService>());
    }

    [Fact]
    public void Stat_shows_the_change_against_its_comparison_value()
    {
        Services.AddKlyrooStack();
        JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = Render<KlyDashboardStat>(p => p
            .Add(x => x.Label, "Total MRR")
            .Add(x => x.Value, 240_000)
            .Add(x => x.Compare, 200_000));

        Assert.Contains("+20%", cut.Markup);
    }
}
```

Run it with `dotnet run` (xUnit v3 test projects are executables) or `dotnet test`. `JSInterop.Mode = JSRuntimeMode.Loose` lets components call into JavaScript during a test without you stubbing each call: Klyroo components hand focus, overlays and positioning to the browser, and bUnit has no browser. A test that asserts on that browser behavior needs an end-to-end test, not bUnit.

## Let an agent fill a form

Goal: expose a Klyroo.Form controller to an AI agent and let it set values by field label.

`AddKlyrooAgent()` (part of `AddKlyrooStack()`) registers `AgentBridgeService` as a scoped service. Register a controller on it, then act on the form by label:

```csharp
using Klyroo.Agent;
using Klyroo.Form;
using Klyroo.Stack;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection().AddKlyrooStack().BuildServiceProvider();
var agent = services.GetRequiredService<AgentBridgeService>();

var form = new FormController(new FormSchema
{
    Title = "Quick setup",
    Fields = [new FormFieldSchema { Key = "org", Label = "Organization" }],
});
agent.RegisterForm(form);

var result = await agent.ActAsync([new AgentActStep("Organization", "setValue", ["Acme"])]);
Console.WriteLine(result.Ok);                       // True
Console.WriteLine(form.GetValue<string>("org"));    // Acme
```

The step targets the field by its label (`Organization`), not by its key (`org`). The same pattern works for dashboards (`RegisterDashboard`) and spreadsheet importers (`RegisterSpreadsheet`). The agent library has its own repository and guides: see [klyroo-agent](https://github.com/klyroo/klyroo-agent).

## Connect a coding agent to the Klyroo.UI documentation

Goal: let Claude Code, Cursor or another MCP client look up components and parameters instead of inventing them.

Install the tool and register it with your client:

```bash
dotnet tool install --global Klyroo.Stack.Mcp --prerelease
claude mcp add --transport stdio klyroo -- klyroo-stack-mcp
```

For a client that reads an `mcpServers` JSON file, use `{ "mcpServers": { "klyroo": { "command": "klyroo-stack-mcp" } } }`. Once connected, the agent finds components with `search-components`, reads their parameters with `get-component-metadata` and copies working markup with `get-example`. The server instructions tell it to use the metadata before writing markup.

Two options are worth knowing. `--tools search-components,get-component` limits the tools a client sees, which keeps the tool list short for clients that load every description. `--site-url https://docs.example.com` prefixes page paths in the answers with your own address once the documentation site is deployed. The server answers from the documentation bundled when the tool was built, so update the tool to get newer documentation.

The server has component pages for Klyroo.UI and Klyroo.Table, and the documentation pages of every other library at `/<library>/docs/<page>`, for example `/spreadsheet/docs/guides`. `search-documentation` searches all of them.

## Start a project from a template

Goal: get a runnable app with the meta-package, the stylesheets, a shell and a few pages that use the libraries the way they are meant to be used.

```bash
dotnet new install Klyroo.Templates::0.1.0-preview.1
dotnet new klyroo-saas -n MyShop
cd MyShop
dotnet run
```

`dotnet new list klyroo` shows the four templates. The two web templates are complete starters with sample data in memory, so they run with no database, and both share the same plumbing: the stock .NET 10 Blazor Web App parts (`MapStaticAssets`, the exception handler, HSTS, HTTPS redirection, a `not-found` page, `launchSettings.json`, `appsettings.json` and a `.gitignore`), `KlyApp` with `KlyFormProvider` and `KlyFilePreviewProvider` in the layout, a color mode button, and `Klyroo.UI.Server` for the cookies of the request.

| Template | Shell | Pages |
| :--- | :--- | :--- |
| `klyroo-saas` | `KlyHeader` with the navigation, the product name and a color mode button | **Overview**: a `DashboardController` with a plan filter and four resources, `DashboardPersistence` and JSON type infos. **Accounts**: a `KlyDataList` with a `Provider`, filters with counts, search, paging, euro prices, a bulk delete and an edit row action, and a "New account" button that opens a `FormService` drawer. **Usage**: `KlyDisplay` blocks (stat row, meters, facts, breakdown, timeline, secret code) and a file preview |
| `klyroo-admin` | `KlyDashboardGroup` with a resizable sidebar and a navbar on every page | **Dashboard**: stats, a gauge, a line chart, bars, a table and a feed. **Customers**: a `KlyDataList` over in-memory rows with selection, a sum row, bulk export to the file preview, an edit drawer and a link to each customer. **Customer**: `KlyDisplay` person, facts, meter, timeline and alert blocks. **Import**: `KlySpreadsheetImport` with a schema (a required field, a select with aliases, a number with a rule), a sample file, and the rows added to the list |

`klyroo-headless` creates a console app that references only `Klyroo.Stack.Core`, and `klyroo-maui` a .NET MAUI Blazor Hybrid app whose layout wraps the body in `KlyApp` with the two providers.

**Options.** `--interactivity Server|WebAssembly|Auto` (default `Server`) picks how the pages run. With `Server` there is one project. With `WebAssembly` or `Auto` the template creates `MyShop` (the server: `Program.cs`, `App.razor`, `wwwroot`) and `MyShop.Client` (the layout, the pages, the sample data and `Routes.razor`, because a page that runs in the browser has to live in the client assembly), plus `MyShop.slnx`; the server project references the client project and both call `AddKlyrooStack()` and `AddAppServices()`. `--product-name "Acme Cloud"` sets the name the pages show; without it the project name is written as words (`MyShop` becomes "My Shop"). The render mode of the app is one line, `AppRender.Mode`, and it is the value that `DashboardPersistence.Attach` receives, which is what hands the data of the prerender to the interactive render in every mode.

**The version.** The generated `.csproj` references `Klyroo.Stack` at the version of the `Klyroo.Templates` package you installed. Each `template.json` holds a constant symbol, `KlyrooVersion`, with a placeholder value; a pre-pack target in `Klyroo.Templates.csproj` writes a copy of every `template.json` with `$(Version)` and packs the copy. A release changes the version in `build/Version.props` and nowhere else, and a test fails when a template file carries a version of its own.

**Where the sample data lives.** The store of each app is a scoped service, so with Interactive Server the data starts again with every circuit (a full page reload) and with WebAssembly with every browser tab. Navigating with the links of the app keeps it. Replace the bodies of the store with your data access: the pages only call its methods, and each read takes the cancellation token that Klyroo cancels when a newer request replaces it.

## Style a generated app

Goal: know which classes work in the markup of a generated app, and how to switch to Tailwind.

The web templates use the zero-setup path that [Getting started](getting-started.md#load-the-stylesheets) describes: `klyroo.preflight.css` and `klyroo.css` from Klyroo.UI, then `klyroo-stack.css` from `Klyroo.Stack`, then the `app.css` of the app. The first three are precompiled, so the app needs neither Node nor a Tailwind build, and the first build needs no network. The consequence is that the sheets only hold the classes the Klyroo components use. A utility such as `md:grid-cols-2` or `space-y-6` that you write in your own markup is not in them and does nothing, because no Tailwind run reads your pages. The templates therefore keep their layout in `wwwroot/app.css`, in plain CSS on the Klyroo tokens (`var(--klyroo-text-muted)` and the others), which follows the theme and the color mode.

A test of this repository checks the markup of the `klyroo-saas`, `klyroo-admin` and `klyroo-maui` templates: every class in a `class` or `Class` attribute has to be defined by one of the sheets or by the `app.css` of the template.

To write Tailwind utilities in your markup instead, compile Tailwind in the app with `KlyrooTailwind`, as the Klyroo.UI guide [Compile Tailwind in your app](https://github.com/klyroo/klyroo-ui/blob/main/docs/guides.md#compile-tailwind-in-your-app) explains, and load its output in place of the three Klyroo sheets. `Klyroo.Table` then needs the extra import that the [Klyroo.Table guide](https://github.com/klyroo/klyroo-table/blob/main/docs/getting-started.md#4-style-the-list) describes, and so do the other libraries whose classes the Klyroo.UI theme does not list. Do not load `klyroo.css` or `klyroo-stack.css` next to a Tailwind build: two sheets that declare the same utilities in one cascade layer override each other.

## Publish the headless template with NativeAOT

Goal: produce a native binary that uses the headless engines.

```bash
dotnet new klyroo-headless -n Worker
cd Worker
dotnet publish -c Release -r linux-x64
./bin/Release/net10.0/linux-x64/publish/Worker
```

The template sets `PublishAot` and references `Klyroo.Stack.Core`, whose engines are marked `IsAotCompatible`. The program prints a line that shows each engine working: a query, a spreadsheet import, a CSV preview, a form controller, a KPI format and an undo stack. Use the runtime identifier of your machine instead of `linux-x64`. NativeAOT needs the platform's native toolchain (a C compiler and linker on Linux); that requirement comes from the .NET SDK, not from Klyroo.

## Run the MCP server and the documentation site from source

Goal: work on the hub itself.

The projects reference the sibling repositories by relative path, so clone `klyroo-ui`, `klyroo-query`, `klyroo-table`, `klyroo-form`, `klyroo-dashboard`, `klyroo-display`, `klyroo-spreadsheet` and `klyroo-agent` next to this repository first. Then:

```bash
dotnet build Klyroo.Stack.slnx -c Release
dotnet test Klyroo.Stack.slnx -c Release --no-build
dotnet run --project src/Klyroo.Stack.Mcp -- --list-tools
dotnet run --project site/Klyroo.Stack.Site
```

Building `src/Klyroo.Stack.Mcp.Core` runs `build/Klyroo.Stack.DocsBundle`, which reads the sources of the site and of Klyroo.UI and embeds the result as `docs-bundle.json`. The test projects in the solution cover the MCP server, the generated `llms.txt` and the demo supervisor of the site. The site starts the live template demos as child processes; set `Klyroo__Templates__Enabled=false` to run it without them.
