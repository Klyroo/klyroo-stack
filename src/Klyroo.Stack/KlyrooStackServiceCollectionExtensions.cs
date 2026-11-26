using Klyroo.UI;
using Klyroo.Query;
using Klyroo.Table;
using Klyroo.Form;
using Klyroo.Dashboard;
using Klyroo.Display;
using Klyroo.Spreadsheet;
using Klyroo.Agent;
using Microsoft.Extensions.DependencyInjection;

namespace Klyroo.Stack;

/// <summary>Dependency injection extensions of the <c>Klyroo.Stack</c> meta-package.</summary>
public static class KlyrooStackServiceCollectionExtensions
{
    /// <summary>
    /// Registers the entire Klyroo ecosystem in one call:
    /// <c>Klyroo.UI</c>, <c>Klyroo.Query</c>, <c>Klyroo.Table</c>, <c>Klyroo.Form</c>,
    /// <c>Klyroo.Dashboard</c>, <c>Klyroo.Display</c>, <c>Klyroo.Spreadsheet</c>, and <c>Klyroo.Agent</c>.
    /// Works in Blazor Web App (SSR / Interactive Server / WebAssembly) and .NET MAUI Blazor Hybrid. In an app with a WebAssembly client,
    /// call it in the server and in the client <c>Program.cs</c> with the same configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only the options of Klyroo.UI and Klyroo.Table have a callback here. To configure another library, call its own registration method after this
    /// one, with the callback: the libraries keep one options object, created by the first call, and every later call changes that same instance, so the
    /// order of the calls does not matter.
    /// </para>
    /// <code>
    /// builder.Services.AddKlyrooStack(configureUi: ui =&gt; ui.Defaults.Size = KlySize.Sm);
    /// builder.Services.AddKlyrooDashboard(o =&gt; o.PersistMaxBytes = 4096);        // Klyroo.Dashboard.KlyrooDashboardOptions
    /// builder.Services.AddKlyrooDisplay(o =&gt; o.Texts("de", t =&gt; t with { ClosePreview = "Schließen" })); // Klyroo.Display.KlyrooDisplayOptions
    /// </code>
    /// <para>
    /// <c>AddKlyrooForm</c>, <c>AddKlyrooSpreadsheet</c>, <c>AddKlyrooQuery</c> and <c>AddKlyrooAgent</c> take no callback. Options are read when the first
    /// component renders and some freeze at that point (a theme or a message table throws <see cref="InvalidOperationException"/> when changed
    /// afterwards), so configure them during registration, in the server and in the client project alike.
    /// </para>
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <param name="configureUi">Configures <see cref="KlyrooOptions"/>, the themes and settings of Klyroo.UI.</param>
    /// <param name="configureTable">Configures <see cref="KlyrooTableOptions"/>, the themes and messages of Klyroo.Table.</param>
    /// <returns>The same collection, for chaining.</returns>
    public static IServiceCollection AddKlyrooStack(
        this IServiceCollection services,
        Action<KlyrooOptions>? configureUi = null,
        Action<KlyrooTableOptions>? configureTable = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddKlyrooUi(configureUi);
        services.AddKlyrooQuery();
        services.AddKlyrooTable(configureTable);
        services.AddKlyrooForm();
        services.AddKlyrooDashboard();
        services.AddKlyrooDisplay();
        services.AddKlyrooSpreadsheet();
        services.AddKlyrooAgent();
        return services;
    }
}
