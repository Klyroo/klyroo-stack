# Getting started

This tutorial builds a small product list in two steps: first the data logic with no UI at all, using the headless packages, then a Blazor Web App that shows the same kind of list with a stat card and a form drawer. It takes about fifteen minutes and every file is shown in full.

You need the .NET 10 SDK. The packages are preview releases, so every `dotnet add package` command carries `--prerelease`.

## 1. Query data without a UI

`Klyroo.Stack.Core` references the headless engine of each library. Create a console app and add it:

```bash
dotnet new console -n ShopLogic
cd ShopLogic
dotnet add package Klyroo.Stack.Core --prerelease
```

Replace `Program.cs`:

```csharp
using Klyroo.Query;
using Klyroo.Stack;

// Which library owns a component, and which packages does it ship in?
var module = KlyrooStackCatalog.ResolveComponentModule("KlyDataList<Product>")!;
Console.WriteLine($"{module.Name}: {module.CorePackage} + {module.UiPackage}");

// Describe the data once, then filter, sort and page it with no UI involved.
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

Run it with `dotnet run`:

```text
Klyroo.Table: Klyroo.Table.Core + Klyroo.Table
3 products, most expensive first: Monitor
```

`DataListSchema` is a Klyroo.Query type: it declares which fields exist, which are sortable and how rows are keyed. The same schema later drives the table in Blazor, so the rules you test here are the rules the UI applies. `KlyrooStackCatalog` is the one type that `Klyroo.Stack.Core` adds itself.

## 2. Create the Blazor app

Create an interactive Blazor Web App and add the meta-package:

```bash
dotnet new blazor -n Shop --interactivity Server --all-interactive --empty
cd Shop
dotnet add package Klyroo.Stack --prerelease
```

The steps below describe the files that the template generates; your copy may differ in details such as the layout. Keep what the template gives you and make the changes shown.

### Register the libraries

`AddKlyrooStack()` registers all eight libraries. Replace `Program.cs`:

```csharp
using Klyroo.Stack;
using Shop.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKlyrooStack();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### Load the stylesheets

In `Components/App.razor`, add the three stylesheets and `<KlyHead />` inside `<head>`. The first two come from Klyroo.UI, the third from the meta-package; all of them are precompiled, so the app does not need Node or Tailwind:

```razor
<link rel="stylesheet" href="@Assets["_content/Klyroo.UI/klyroo.preflight.css"]" />
<link rel="stylesheet" href="@Assets["_content/Klyroo.UI/klyroo.css"]" />
<link rel="stylesheet" href="@Assets["_content/Klyroo.Stack/klyroo-stack.css"]" />
<KlyHead />
```

If the project still loads Bootstrap or another stylesheet that sits outside CSS layers, remove it: unlayered CSS overrides the layered CSS of Tailwind v4 and changes how the components look.

### Add the root component

Replace `Components/Routes.razor`. `KlyApp` is the root of the Klyroo components, `KlyFormProvider` hosts the form overlays and `KlyFilePreviewProvider` hosts the file preview:

```razor
<KlyApp>
    <KlyFormProvider />
    <KlyFilePreviewProvider />
    <Router AppAssembly="typeof(Program).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="routeData" />
        </Found>
    </Router>
</KlyApp>
```

### Import the namespaces

Add these lines to `Components/_Imports.razor`:

```razor
@using Klyroo.UI
@using Klyroo.Query
@using Klyroo.Table
@using Klyroo.Form
@using Klyroo.Dashboard
@using Klyroo.Display
@using Klyroo.Spreadsheet
@using Klyroo.Agent
@using Klyroo.Stack
```

## 3. Build the page

Delete `Components/Pages/Home.razor` if it exists and create `Components/Pages/Products.razor`. The page combines three libraries: Klyroo.Dashboard for the stat cards, Klyroo.Table for the list and Klyroo.Form for the "new product" drawer.

```razor
@page "/"
@inject FormService Forms

<KlyDashboardPage Dashboard="_dashboard" Title="Shop">
    <KlyDashboardGrid>
        <KlyDashboardStat Label="Products" Value="_products.Count" />
        <KlyDashboardStat Label="Catalog value" Value="(double)_products.Sum(p => p.Price)" Format="DashboardFormatPreset.Compact" />
    </KlyDashboardGrid>

    <div class="mt-6">
        <KlyButton Label="New product" Color="KlyColor.Primary" OnClick="AddProductAsync" />
    </div>

    <div class="mt-4">
        <KlyDataList Schema="Schema" Items="_products" UrlSync="false">
            <KlyDataListTable TItem="Product">
                <KlyDataListColumn TItem="Product" Field="name" Title="Product" />
                <KlyDataListColumn TItem="Product" Field="price" Title="Price" Summary="DataListSummaryKind.Sum" />
            </KlyDataListTable>
        </KlyDataList>
    </div>
</KlyDashboardPage>

@code {
    private sealed record Product(int Id, string Name, decimal Price);

    private static readonly DataListSchema<Product> Schema = DataListSchema.For<Product>(s => s
        .RowKey(p => p.Id)
        .RowLabel(p => p.Name)
        .Field(p => p.Name, f => f.Key("name").Sortable())
        .Field(p => p.Price, f => f.Key("price").Sortable())
        .DefaultSort(p => p.Name));

    private readonly DashboardController _dashboard = new("shop");

    private readonly List<Product> _products =
    [
        new(1, "Keyboard", 120m),
        new(2, "Monitor", 350m),
        new(3, "Mouse", 40m),
    ];

    private async Task AddProductAsync()
    {
        var outcome = await Forms.CreateFormAsync(new FormSchema
        {
            FormKey = "new-product",
            Title = "New product",
            Fields =
            [
                new FormFieldSchema { Key = "name", Label = "Name", Required = true },
                new FormFieldSchema { Key = "price", Label = "Price", Kind = FormFieldKind.Number, DefaultValue = 50.0 },
            ],
        }, mode: FormShellMode.Drawer);

        if (outcome.IsCompleted)
        {
            _products.Add(new Product(
                _products.Count + 1,
                outcome.FormData["name"]?.ToString() ?? "Product",
                Convert.ToDecimal(outcome.FormData["price"] ?? 50m)));
        }
    }
}
```

Run `dotnet run`, open the address it prints, and you see the two stat cards, the list sorted by name with a sum row under the price column, and a button that opens the drawer. `UrlSync="false"` keeps the list state out of the query string, which is what you want for an in-memory list on a single page. `FormService.CreateFormAsync` returns when the drawer closes, and `outcome.IsCompleted` is true only when the user submitted the form.

The form values come back as `outcome.FormData`, a dictionary keyed by field key, so the number field is read with `Convert.ToDecimal`.

## Next steps

- [Guides](guides.md): register a single library, share options between the server and the client, test a page with bUnit, let an agent drive a form, and connect the MCP server.
- [Concepts](concepts.md): how the libraries, the `.Core` packages and the registration call fit together.
- [Platforms](platforms.md): render modes, .NET MAUI Blazor Hybrid and NativeAOT.
- [Templates](../README.md#project-templates): start the next project from `dotnet new klyroo-saas` instead of editing files by hand.
