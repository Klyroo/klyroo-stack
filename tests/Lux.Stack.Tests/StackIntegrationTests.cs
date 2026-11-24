using Blux.UI.Tools;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lux.Stack.Tests;

public sealed class StackIntegrationTests : BunitContext
{
    [Fact]
    public void LuxStackCatalog_indexes_all_8_repositories_and_resolves_components_to_their_owners()
    {
        Assert.Equal(8, LuxStackCatalog.Modules.Count);

        Assert.Equal("lux-ui", LuxStackCatalog.ResolveComponentModule("BxButton")?.Id);
        Assert.Equal("lux-query", LuxStackCatalog.ResolveComponentModule("QueryState")?.Id);
        Assert.Equal("lux-table", LuxStackCatalog.ResolveComponentModule("BxDataList<Person>")?.Id);
        Assert.Equal("lux-form", LuxStackCatalog.ResolveComponentModule("BxFormEngine")?.Id);
        Assert.Equal("lux-dashboard", LuxStackCatalog.ResolveComponentModule("BxDashboardPage")?.Id);
        Assert.Equal("lux-display", LuxStackCatalog.ResolveComponentModule("BxDisplay")?.Id);
        Assert.Equal("lux-spreadsheet", LuxStackCatalog.ResolveComponentModule("BxSpreadsheetImport")?.Id);
        Assert.Equal("lux-agent", LuxStackCatalog.ResolveComponentModule("AgentBridgeService")?.Id);
    }

    [Fact]
    public async Task AddLuxStack_registers_all_services_and_bridges_Form_Dashboard_and_Spreadsheet_through_Agent()
    {
        Services.AddLuxStack();

        var queryState = Services.GetRequiredService<QueryState>();
        var formService = Services.GetRequiredService<FormService>();
        var filePreview = Services.GetRequiredService<FilePreviewService>();
        var agent = Services.GetRequiredService<AgentBridgeService>();

        Assert.NotNull(queryState);
        Assert.NotNull(formService);
        Assert.NotNull(filePreview);

        var form = new FormController(new FormSchema
        {
            Title = "Quick Setup",
            Fields = [new FormFieldSchema { Key = "org", Label = "Organization", DefaultValue = "Blux" }]
        });
        var dashboard = new DashboardController("metrics");
        var importer = new SpreadsheetImporter(new SpreadsheetSchema
        {
            Key = "contacts",
            Columns = [new SpreadsheetColumnSchema { Key = "email", Label = "Email" }]
        });

        agent.RegisterForm(form);
        agent.RegisterDashboard(dashboard);
        agent.RegisterSpreadsheet(importer);

        var snap = agent.Snapshot();
        Assert.Equal(4, snap.Count);

        var act = await agent.ActAsync([new AgentActStep("Organization", "setValue", ["LuxStack"])]);
        Assert.True(act.Ok);
        Assert.Equal("LuxStack", form.GetValue<string>("org"));
    }
}
