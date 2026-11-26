using Klyroo.Dashboard;
using Klyroo.Display;
using Klyroo.Table;
using Klyroo.UI;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Klyroo.Stack.Tests;

/// <summary>
/// <c>AddKlyrooStack</c> only takes the callbacks of Klyroo.UI and Klyroo.Table. The options of the other libraries are configured by calling their own
/// registration method again: every one of them changes the instance that the first call created, so the order of the calls does not matter.
/// </summary>
public sealed class StackOptionsTests
{
    [Fact]
    public void A_later_AddKlyrooDashboard_configures_the_instance_that_AddKlyrooStack_registered()
    {
        var services = new ServiceCollection();
        services.AddKlyrooStack();
        services.AddKlyrooDashboard(o => o.PersistMaxBytes = 4096);

        using var provider = services.BuildServiceProvider();

        Assert.Equal(4096, provider.GetRequiredService<KlyrooDashboardOptions>().PersistMaxBytes);
        Assert.Single(services, d => d.ServiceType == typeof(KlyrooDashboardOptions));
    }

    [Fact]
    public void AddKlyrooDashboard_before_AddKlyrooStack_configures_the_same_instance()
    {
        var services = new ServiceCollection();
        services.AddKlyrooDashboard(o => o.PersistMaxBytes = 2048);
        services.AddKlyrooStack();

        using var provider = services.BuildServiceProvider();

        Assert.Equal(2048, provider.GetRequiredService<KlyrooDashboardOptions>().PersistMaxBytes);
        Assert.Single(services, d => d.ServiceType == typeof(KlyrooDashboardOptions));
    }

    [Fact]
    public void A_later_AddKlyrooDisplay_configures_the_Display_options_that_live_in_the_Klyroo_UI_options()
    {
        var services = new ServiceCollection();
        services.AddKlyrooStack();
        KlyrooDisplayOptions? seen = null;
        services.AddKlyrooDisplay(o => seen = o);

        using var provider = services.BuildServiceProvider();

        // The Display options are an extension of the Klyroo.UI options, so there is one instance whatever the number of calls.
        Assert.NotNull(seen);
        Assert.Same(seen, provider.GetRequiredService<KlyrooOptions>().Extension<KlyrooDisplayOptions>());
    }

    [Fact]
    public void A_later_AddKlyrooTable_and_AddKlyrooUi_add_to_the_callbacks_given_to_AddKlyrooStack()
    {
        var services = new ServiceCollection();
        services.AddKlyrooStack(
            configureUi: ui => ui.Defaults.Size = KlySize.Sm,
            configureTable: table => table.SearchDebounce = 450);
        services.AddKlyrooTable(table => table.PersistMaxBytes = 12_000);
        services.AddKlyrooUi(ui => ui.Defaults.Color = KlyColor.Secondary);

        using var provider = services.BuildServiceProvider();
        var ui = provider.GetRequiredService<KlyrooOptions>();
        var table = ui.Extension<KlyrooTableOptions>();

        Assert.Equal(KlySize.Sm, ui.Defaults.Size);
        Assert.Equal(KlyColor.Secondary, ui.Defaults.Color);
        Assert.Equal(450, table.SearchDebounce);
        Assert.Equal(12_000, table.PersistMaxBytes);
    }
}
