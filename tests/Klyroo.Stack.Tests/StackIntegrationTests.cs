using Klyroo.Query;
using Klyroo.Table;
using Klyroo.Form;
using Klyroo.Dashboard;
using Klyroo.Display;
using Klyroo.Spreadsheet;
using Klyroo.Agent;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Klyroo.Stack.Tests;

public sealed class StackIntegrationTests : BunitContext
{
    [Fact]
    public void KlyrooStackCatalog_indexes_all_8_repositories_and_resolves_components_to_their_owners()
    {
        Assert.Equal(8, KlyrooStackCatalog.Modules.Count);

        Assert.Equal("klyroo-ui", KlyrooStackCatalog.ResolveComponentModule("KlyButton")?.Id);
        Assert.Equal("klyroo-query", KlyrooStackCatalog.ResolveComponentModule("QueryState")?.Id);
        Assert.Equal("klyroo-table", KlyrooStackCatalog.ResolveComponentModule("KlyDataList<Person>")?.Id);
        Assert.Equal("klyroo-form", KlyrooStackCatalog.ResolveComponentModule("KlyFormEngine")?.Id);
        Assert.Equal("klyroo-dashboard", KlyrooStackCatalog.ResolveComponentModule("KlyDashboardPage")?.Id);
        Assert.Equal("klyroo-display", KlyrooStackCatalog.ResolveComponentModule("KlyDisplay")?.Id);
        Assert.Equal("klyroo-spreadsheet", KlyrooStackCatalog.ResolveComponentModule("KlySpreadsheetImport")?.Id);
        Assert.Equal("klyroo-agent", KlyrooStackCatalog.ResolveComponentModule("AgentBridgeService")?.Id);
    }

    [Fact]
    public async Task AddKlyrooStack_registers_all_services_and_bridges_Form_Dashboard_and_Spreadsheet_through_Agent()
    {
        Services.AddKlyrooStack();

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
            Fields = [new FormFieldSchema { Key = "org", Label = "Organization", DefaultValue = "Klyroo" }]
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

        var act = await agent.ActAsync([new AgentActStep("Organization", "setValue", ["Klyroo"])]);
        Assert.True(act.Ok);
        Assert.Equal("Klyroo", form.GetValue<string>("org"));
    }
}
