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
    public sealed class QuickSetup
    {
        public string Organization { get; set; } = "";
    }

    public sealed class ContactRow
    {
        public string? Email { get; set; }
    }

    private static readonly FormSchema<QuickSetup> QuickSetupSchema = FormSchema.For<QuickSetup>(s =>
    {
        s.Title = "Quick Setup";
        s.Field(m => m.Organization, f => f.Default("Klyroo"));
    });

    [Fact]
    public void KlyrooStackCatalog_indexes_all_8_repositories_and_resolves_components_to_their_owners()
    {
        Assert.Equal(8, KlyrooStackCatalog.Modules.Count);

        Assert.Equal("klyroo-ui", KlyrooStackCatalog.ResolveComponentModule("KlyButton")?.Id);
        Assert.Equal("klyroo-query", KlyrooStackCatalog.ResolveComponentModule("QueryState")?.Id);
        Assert.Equal("klyroo-table", KlyrooStackCatalog.ResolveComponentModule("KlyDataList<Person>")?.Id);
        Assert.Equal("klyroo-form", KlyrooStackCatalog.ResolveComponentModule("KlyFormEngine")?.Id);
        Assert.Equal("klyroo-dashboard", KlyrooStackCatalog.ResolveComponentModule("KlyDashboardPage")?.Id);
        Assert.Equal("klyroo-display", KlyrooStackCatalog.ResolveComponentModule("KlyCodeBlock")?.Id);
        Assert.Equal("klyroo-ui", KlyrooStackCatalog.ResolveComponentModule("KlyStat<decimal>")?.Id);
        Assert.Null(KlyrooStackCatalog.ResolveComponentModule("KlyDisplay"));
        Assert.Equal("klyroo-spreadsheet", KlyrooStackCatalog.ResolveComponentModule("KlySpreadsheetImport")?.Id);
        Assert.Equal("klyroo-agent", KlyrooStackCatalog.ResolveComponentModule("AgentBridgeService")?.Id);
    }

    [Fact]
    public void KlyrooStackCatalog_lists_the_add_on_packages_that_Klyroo_Stack_does_not_include()
    {
        var form = KlyrooStackCatalog.Modules.Single(m => m.Id == "klyroo-form");
        Assert.Equal(["Klyroo.Form.DataAnnotations"], form.AddOnPackages);
        Assert.Contains("Klyroo.UI.Server", KlyrooStackCatalog.Modules.Single(m => m.Id == "klyroo-ui").AddOnPackages!);
        Assert.Contains("Klyroo.Query.EntityFrameworkCore", KlyrooStackCatalog.Modules.Single(m => m.Id == "klyroo-query").AddOnPackages!);
        Assert.Equal(["Klyroo.Agent.AI"], KlyrooStackCatalog.Modules.Single(m => m.Id == "klyroo-agent").AddOnPackages);
    }

    [Fact]
    public void KlyrooStackCatalog_GetModule_resolves_an_add_on_package_to_its_library()
    {
        Assert.Equal("klyroo-form", KlyrooStackCatalog.GetModule("Klyroo.Form.DataAnnotations")?.Id);
        Assert.Equal("klyroo-ui", KlyrooStackCatalog.GetModule("klyroo.ui.server")?.Id);
        Assert.Equal("klyroo-ui", KlyrooStackCatalog.GetModule("Klyroo.UI.Images.Local")?.Id);
        Assert.Equal("klyroo-agent", KlyrooStackCatalog.GetModule("Klyroo.Agent.AI")?.Id);
        foreach (var module in KlyrooStackCatalog.Modules)
        {
            foreach (var package in module.AddOnPackages ?? []) Assert.Equal(module.Id, KlyrooStackCatalog.GetModule(package)?.Id);
        }
        Assert.Null(KlyrooStackCatalog.GetModule("Klyroo.Nothing"));
    }

    [Fact]
    public void No_add_on_package_is_a_dependency_of_the_hub_or_the_Core_meta_package()
    {
        // None of them is a package the hub or the Core meta-package depends on.
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "Klyroo.Stack.slnx"))) root = root.Parent;
        Assert.NotNull(root);
        var projects = string.Concat(File.ReadAllText(Path.Combine(root.FullName, "src", "Klyroo.Stack", "Klyroo.Stack.csproj")), File.ReadAllText(Path.Combine(root.FullName, "src", "Klyroo.Stack.Core", "Klyroo.Stack.Core.csproj")));
        Assert.All(KlyrooStackCatalog.Modules.SelectMany(m => m.AddOnPackages ?? []), package => Assert.DoesNotContain(package, projects, StringComparison.Ordinal));
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

        var organization = QuickSetupSchema.Field(m => m.Organization);
        var form = new FormController<QuickSetup>(QuickSetupSchema);
        var dashboard = new DashboardController("metrics");
        var importer = new SpreadsheetImporter<ContactRow>(SpreadsheetSchema.For<ContactRow>(s =>
        {
            s.Key = "contacts";
            s.Column(r => r.Email, c => c.Label("Email"));
        }));

        agent.RegisterForm(form);
        agent.RegisterDashboard(dashboard);
        agent.RegisterSpreadsheet(importer);

        var snap = agent.Snapshot();
        Assert.Equal(4, snap.Count);

        var act = await agent.ActAsync([new AgentActStep("Organization", "setValue", ["Acme"])]);
        Assert.True(act.Ok);
        Assert.Equal("Acme", form.Get(organization));
    }
}
