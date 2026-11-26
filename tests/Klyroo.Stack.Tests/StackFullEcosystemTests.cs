using System.Globalization;
using Bunit;
using Klyroo.Agent;
using Klyroo.Dashboard;
using Klyroo.Display;
using Klyroo.Form;
using Klyroo.Query;
using Klyroo.Spreadsheet;
using Klyroo.Stack;
using Klyroo.Table;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Klyroo.Stack.Tests;

public sealed class StackFullEcosystemTests : BunitContext
{
    private sealed record Customer(int Id, string Name, string Region, decimal Mrr);

    private static readonly DataListSchema<Customer> CustomerSchema = DataListSchema.For<Customer>(s => s
        .RowKey(c => c.Id)
        .RowLabel(c => c.Name)
        .Search(c => c.Name, c => c.Region)
        .Field(c => c.Name, f => f.Key("name").Sortable())
        .Field(c => c.Region, f => f.Key("region").Sortable().Filter(DataListFilter.Option().WithFacets()))
        .Field(c => c.Mrr, f => f.Key("mrr").Sortable())
        .DefaultSort(c => c.Name));

    [Fact]
    public void KlyrooStackCatalog_template_names_are_templates_that_Klyroo_Templates_ships()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "Klyroo.Stack.slnx"))) root = root.Parent;
        Assert.NotNull(root);
        var shortNames = Directory.GetFiles(Path.Combine(root.FullName, "templates", "Klyroo.Templates", "content"), "template.json", SearchOption.AllDirectories)
            .Select(path => System.Text.Json.JsonDocument.Parse(File.ReadAllText(path)).RootElement.GetProperty("shortName").GetString())
            .ToHashSet(StringComparer.Ordinal);

        Assert.All(KlyrooStackCatalog.Modules, m => Assert.Contains(m.TemplateShortName, shortNames));
    }

    [Fact]
    public void KlyrooStackCatalog_metadata_is_complete_unique_and_returns_null_for_unknown_components()
    {
        var ids = KlyrooStackCatalog.Modules.Select(m => m.Id).ToList();
        Assert.Equal(ids.Count, ids.Distinct(StringComparer.OrdinalIgnoreCase).Count());

        foreach (var module in KlyrooStackCatalog.Modules)
        {
            Assert.False(string.IsNullOrWhiteSpace(module.Id));
            Assert.False(string.IsNullOrWhiteSpace(module.Name));
            Assert.False(string.IsNullOrWhiteSpace(module.UiPackage));
            Assert.NotEmpty(module.KeyComponents);
        }

        Assert.Null(KlyrooStackCatalog.ResolveComponentModule("NonExistentWidget"));
        Assert.Equal("klyroo-table", KlyrooStackCatalog.ResolveComponentModule("KlyDataListTable<Customer>")?.Id);
    }

    [Fact]
    public void AddKlyrooStack_mounts_providers_Dashboard_Table_and_Display_together_in_bUnit()
    {
        Services.AddKlyrooStack();
        JSInterop.Mode = JSRuntimeMode.Loose;
        SetRendererInfo(new RendererInfo("Server", true));

        var formProvider = Render<KlyFormProvider>();
        var fileProvider = Render<KlyFilePreviewProvider>();
        Assert.True(Services.GetRequiredService<FilePreviewService>().IsMounted);

        var dash = new DashboardController("eco");
        dash.AddView("summary", "Summary");
        var dashCut = Render<KlyDashboardPage>(p => p
            .Add(x => x.Dashboard, dash)
            .Add(x => x.Title, "Klyroo Control Center")
            .AddChildContent<KlyDashboardStat>(s => s
                .Add(x => x.Label, "Total MRR")
                .Add(x => x.Value, 240_000)
                .Add(x => x.Compare, 200_000)));
        Assert.Contains("Klyroo Control Center", dashCut.Markup);
        Assert.Contains("+20%", dashCut.Markup);

        Customer[] rows = [new(1, "Acme Corp", "EU", 120_000m), new(2, "Beta Labs", "US", 80_000m)];
        var tableCut = Render(builder =>
        {
            builder.OpenComponent<KlyDataList<Customer>>(0);
            builder.AddAttribute(1, nameof(KlyDataList<Customer>.Schema), CustomerSchema);
            builder.AddAttribute(2, nameof(KlyDataList<Customer>.Items), rows);
            builder.AddAttribute(3, nameof(KlyDataList<Customer>.UrlSync), false);
            builder.AddAttribute(4, nameof(KlyDataList<Customer>.ChildContent), (RenderFragment)(b =>
            {
                b.OpenComponent<KlyDataListTable<Customer>>(0);
                b.AddAttribute(1, nameof(KlyDataListTable<Customer>.ChildContent), (RenderFragment)(cb =>
                {
                    cb.OpenComponent<KlyDataListColumn<Customer>>(0);
                    cb.AddAttribute(1, nameof(KlyDataListColumn<Customer>.Field), "name");
                    cb.AddAttribute(2, nameof(KlyDataListColumn<Customer>.Title), "Customer");
                    cb.CloseComponent();
                }));
                b.CloseComponent();
            }));
            builder.CloseComponent();
        });
        Assert.Contains("Acme Corp", tableCut.Markup);
        Assert.Contains("Beta Labs", tableCut.Markup);

        // Open CSV preview via DI FilePreviewService and verify KlyFilePreviewProvider renders CSV rows
        var previewSvc = Services.GetRequiredService<FilePreviewService>();
        previewSvc.Open(new FilePreviewFile
        {
            Src = "customers.csv",
            Name = "customers.csv",
            Mime = "text/csv",
            Content = "Name,Region,MRR\nAcme Corp,EU,120000\nBeta Labs,US,80000"
        });
        fileProvider.Render();
        Assert.NotNull(fileProvider.Find("[data-slot=renderer-csv]"));
        Assert.Equal(2, fileProvider.FindAll("[data-slot=csv-row]").Count);
        Assert.NotNull(formProvider);
    }

    [Fact]
    public async Task Full_workflow_with_Agent_UndoStack_Query_URL_roundtrip_and_FormService_Drawer()
    {
        Services.AddKlyrooStack();
        JSInterop.Mode = JSRuntimeMode.Loose;

        var accounts = new List<Customer> { new(1, "Acme", "EU", 100_000m) };
        var history = new UndoStack<IReadOnlyList<Customer>>([.. accounts], limit: 10);

        // 1. Add account via FormService Drawer
        var formProvider = Render<KlyFormProvider>();
        var formSvc = Services.GetRequiredService<FormService>();
        var formTask = formSvc.CreateFormAsync(new FormSchema
        {
            FormKey = "new-customer",
            Title = "New Customer",
            Fields =
            [
                new FormFieldSchema { Key = "name", Label = "Name", Required = true, DefaultValue = "Zephyr" },
                new FormFieldSchema { Key = "region", Label = "Region", DefaultValue = "APAC" },
                new FormFieldSchema { Key = "mrr", Label = "MRR", Kind = FormFieldKind.Number, DefaultValue = 65_000.0 }
            ]
        }, mode: FormShellMode.Drawer);
        formProvider.Render();
        await formProvider.InvokeAsync(() => formSvc.SubmitActiveFormAsync());
        var outcome = await formTask;
        Assert.True(outcome.IsCompleted);

        accounts.Add(new Customer(2, outcome.FormData["name"]!.ToString()!, outcome.FormData["region"]!.ToString()!, Convert.ToDecimal(outcome.FormData["mrr"], CultureInfo.InvariantCulture)));
        history.Record([.. accounts]);
        Assert.Equal(2, accounts.Count);

        // 2. Roundtrip Query URL state & execute
        var state = new DataListState("", [new FilterRule("region", FilterOperator.IsAnyOf, new FilterValue.List(["APAC"]))], [new DataListSort("mrr", Descending: true)], 1, 10, DataListLayout.Table);
        var pairs = DataListStateQuery.ToQuery(state, CustomerSchema, prefix: "c");
        var restored = DataListStateQuery.FromQuery(pairs.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value)), CustomerSchema, prefix: "c");
        Assert.Equal(state, restored);

        var filtered = CustomerSchema.ExecuteInMemory(accounts, new DataListRequest(DataListPaging.None.Instance, restored.Sort, DataListFilterGroup.And([.. restored.Filters])));
        Assert.Single(filtered.Items);
        Assert.Equal("Zephyr", filtered.Items[0].Name);

        // 3. Undo rollback restores original 1-account snapshot
        var rolledBack = history.Undo();
        Assert.Single(rolledBack);
        Assert.Equal("Acme", rolledBack[0].Name);
    }
}
