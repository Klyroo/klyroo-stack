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
