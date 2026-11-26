using Klyroo.Agent;
using Klyroo.Dashboard;
using Klyroo.Display;
using Klyroo.Form;
using Klyroo.Query;
using Klyroo.Spreadsheet;
using Klyroo.Stack;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Klyroo.Stack.Tests;

public sealed class EcosystemIntegrationTests
{
    private sealed record Deal(int Id, string Name, string Region, decimal Amount);

    [Fact]
    public async Task EndToEnd_Spreadsheet_to_Query_to_Dashboard_to_Display_and_Agent_works_together()
    {
        // 1. Import deals via Klyroo.Spreadsheet
        var deals = new List<Deal>();
        var importer = new SpreadsheetImporter(new SpreadsheetSchema
        {
            Key = "deals",
            Columns =
            [
                new SpreadsheetColumnSchema { Key = "name", Label = "Name", Required = true },
                new SpreadsheetColumnSchema { Key = "region", Label = "Region", Required = true },
                new SpreadsheetColumnSchema { Key = "amount", Label = "Amount", Kind = SpreadsheetColumnKind.Number, Required = true }
            ],
            OnSubmit = batch =>
            {
                var id = 1;
                foreach (var r in batch)
                    deals.Add(new Deal(id++, r.Data["name"]!.ToString()!, r.Data["region"]!.ToString()!, Convert.ToDecimal(r.Data["amount"])));
                return Task.FromResult<IReadOnlyList<SpreadsheetRejectedRow>?>(null);
            }
        });
        importer.LoadText("Name,Region,Amount\nAcme,EU,100000\nBeta,US,50000\nGamma,EU,75000", "deals.csv");
        await importer.SubmitAsync(TestContext.Current.CancellationToken);
        Assert.Equal(3, deals.Count);

        // 2. Query EU deals via Klyroo.Query
        var schema = DataListSchema.For<Deal>(s => s
            .RowKey(d => d.Id)
            .Field(d => d.Region, f => f.Key("region").Filter(DataListFilter.Option().WithFacets()))
            .Field(d => d.Amount, f => f.Key("amount").Sortable()));

        var eu = schema.ExecuteInMemory(deals, new DataListRequest(
            DataListPaging.None.Instance,
            [new DataListSort("amount", Descending: true)],
            DataListFilterGroup.And(new FilterRule("region", FilterOperator.Is, new FilterValue.Text("EU"))),
            facets: [new DataListFacetRequest("region", Mode: FacetMode.ExcludeSelf)]));

        Assert.Equal(2, eu.TotalCount);
        Assert.Equal("Acme", eu.Items[0].Name);

        // 3. Orchestrate KPI wave in Klyroo.Dashboard + export CSV & parse in Klyroo.Display
        var dash = new DashboardController("deals-dash");
        var totalWave = dash.RegisterResource(new DashboardResource<decimal>(
            "eu-total", _ => Task.FromResult(eu.Items.Sum(d => d.Amount)), DashboardQueryStage.Essential));
        await dash.RefreshAsync();
        Assert.Equal(175_000m, totalWave.Data);

        var csv = DashboardFormat.ExportCsv(new DashboardDataTable(
            [new("name", "Name"), new("amount", "Amount", Numeric: true)],
            [.. eu.Items.Select(d => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?> { ["name"] = d.Name, ["amount"] = d.Amount.ToString() })]));
        var parsed = FilePreviewService.ParseCsv(csv.TrimStart('\uFEFF'));
        Assert.Equal(2, parsed.Rows.Count);

        // 4. Register Form & Dashboard on Klyroo.Agent and execute cross-controller ActAsync
        var agent = new AgentBridgeService();
        dash.AddView("overview", "Overview");
        dash.AddView("eu", "EU Focus");
        agent.RegisterDashboard(dash);
        var act = await agent.ActAsync([new AgentActStep("deals-dash", "setView", ["eu"])]);
        Assert.True(act.Ok);
        Assert.Equal("eu", dash.CurrentView);
    }
}
