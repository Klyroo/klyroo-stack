using Lux.UI;
using Lux.Query;
using Lux.Table;
using Lux.Form;
using Lux.Dashboard;
using Lux.Display;
using Lux.Spreadsheet;
using Lux.Agent;
using Microsoft.Extensions.DependencyInjection;

namespace Lux.Stack;

public static class LuxStackServiceCollectionExtensions
{
    /// <summary>
    /// Registers the entire LuxStack ecosystem in one call:
    /// <c>Lux.UI</c>, <c>Lux.Query</c>, <c>Lux.Table</c>, <c>Lux.Form</c>,
    /// <c>Lux.Dashboard</c>, <c>Lux.Display</c>, <c>Lux.Spreadsheet</c>, and <c>Lux.Agent</c>.
    /// Works in Blazor Web App (SSR / Interactive Server / WebAssembly) and .NET MAUI Blazor Hybrid.
    /// </summary>
    public static IServiceCollection AddLuxStack(
        this IServiceCollection services,
        Action<LuxOptions>? configureUi = null,
        Action<LuxTableOptions>? configureTable = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddLuxUi(configureUi);
        services.AddLuxQuery();
        services.AddLuxTable(configureTable);
        services.AddLuxForm();
        services.AddLuxDashboard();
        services.AddLuxDisplay();
        services.AddLuxSpreadsheet();
        services.AddLuxAgent();
        return services;
    }
}
