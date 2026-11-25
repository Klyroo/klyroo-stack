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

public static class KlyrooStackServiceCollectionExtensions
{
    /// <summary>
    /// Registers the entire Klyroo ecosystem in one call:
    /// <c>Klyroo.UI</c>, <c>Klyroo.Query</c>, <c>Klyroo.Table</c>, <c>Klyroo.Form</c>,
    /// <c>Klyroo.Dashboard</c>, <c>Klyroo.Display</c>, <c>Klyroo.Spreadsheet</c>, and <c>Klyroo.Agent</c>.
    /// Works in Blazor Web App (SSR / Interactive Server / WebAssembly) and .NET MAUI Blazor Hybrid.
    /// </summary>
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
