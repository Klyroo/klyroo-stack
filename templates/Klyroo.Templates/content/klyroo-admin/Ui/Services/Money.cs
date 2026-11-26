using Klyroo.Dashboard;

namespace KlyrooAdminApp.Services;

/// <summary>One currency format for the whole app, so that a price reads the same in the list, the dashboard and the form.</summary>
public static class Money
{
    /// <summary>A price with the symbol in front and no decimals: <c>€1,240</c>.</summary>
    public static string Format(decimal amount) => DashboardFormat.FormatCurrency((double)Math.Round(amount), "€");

    /// <summary>A large amount in the compact form: <c>€148.9K</c>.</summary>
    public static string Compact(double amount) => "€" + DashboardFormat.FormatCompact(amount);
}
