namespace Klyroo.Playground.Data;

public sealed record SalesSummary(double Revenue, double PreviousRevenue, int Orders, double AverageOrder, double Goal);
public sealed record RegionSales(string Region, double Revenue);
public sealed record CustomerRow(string Customer, string Region, string Channel, double Revenue, int Orders, double Growth);

/// <summary>Sample sales numbers for 2026 and 2025 by month, region and channel, computed from fixed curves: the same on every load.</summary>
public static class SalesData
{
    public static readonly string[] Regions = ["EMEA", "Americas", "APAC"];
    public static readonly string[] Channels = ["Web", "Retail", "Partner"];
    public static readonly string[] Months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    private static readonly double[] RegionBase = [46_000, 38_000, 24_000];
    private static readonly double[] ChannelShare = [0.5, 0.3, 0.2];

    private static double Revenue(int month, int region, int channel, int year) =>
        RegionBase[region] * ChannelShare[channel] * (1 + 0.035 * month + 0.12 * Math.Sin((month + region * 2 + channel) / 1.9)) * (year == 2026 ? 1.0 : 0.86 + 0.02 * Math.Cos(month + channel));

    private static IEnumerable<int> RegionIndexes(string? region) =>
        region is { Length: > 0 } && Array.IndexOf(Regions, region) is var i and >= 0 ? [i] : Enumerable.Range(0, Regions.Length);

    private static IEnumerable<int> ChannelIndexes(IReadOnlyCollection<string> channels) =>
        channels.Count == 0 ? Enumerable.Range(0, Channels.Length) : channels.Select(c => Array.IndexOf(Channels, c)).Where(i => i >= 0);

    public static double[] Monthly(string? region, IReadOnlyCollection<string> channels, int year, int months)
    {
        var channelIndexes = ChannelIndexes(channels).ToArray();
        return [.. Enumerable.Range(12 - months, months).Select(m => Math.Round(RegionIndexes(region).Sum(r => channelIndexes.Sum(c => Revenue(m, r, c, year)))))];
    }

    public static string[] MonthLabels(int months) => Months[(12 - months)..];

    public static SalesSummary Summary(string? region, IReadOnlyCollection<string> channels, int months)
    {
        var now = Monthly(region, channels, 2026, months).Sum();
        var before = Monthly(region, channels, 2025, months).Sum();
        var orders = (int)Math.Round(now / 87.0);
        return new SalesSummary(now, before, orders, orders == 0 ? 0 : now / orders, now * 1.12);
    }

    public static IReadOnlyList<RegionSales> ByRegion(IReadOnlyCollection<string> channels, int months)
    {
        var channelIndexes = ChannelIndexes(channels).ToArray();
        return [.. Regions.Select((name, r) => new RegionSales(name, Math.Round(Enumerable.Range(12 - months, months).Sum(m => channelIndexes.Sum(c => Revenue(m, r, c, 2026))))))];
    }

    private static readonly (string Name, int Region, int Channel, double Weight)[] CustomerSeeds =
    [
        ("Northwind", 0, 0, 1.0), ("Lumeco", 0, 1, 0.8), ("Fabrikam", 0, 2, 0.6), ("Contoso", 1, 0, 1.1), ("Globex", 1, 1, 0.9), ("Initech", 1, 2, 0.5),
        ("Umbrella", 2, 0, 0.7), ("Hooli", 2, 1, 0.55), ("Stark Retail", 2, 2, 0.4), ("Wayne Partners", 0, 2, 0.45), ("Acme", 1, 0, 0.65), ("Soylent", 0, 1, 0.35),
    ];

    public static IReadOnlyList<CustomerRow> Customers(string? region, IReadOnlyCollection<string> channels, int months)
    {
        var regions = RegionIndexes(region).ToHashSet();
        var channelIndexes = ChannelIndexes(channels).ToHashSet();
        return [.. CustomerSeeds
            .Where(c => regions.Contains(c.Region) && channelIndexes.Contains(c.Channel))
            .Select(c =>
            {
                var revenue = Math.Round(Enumerable.Range(12 - months, months).Sum(m => Revenue(m, c.Region, c.Channel, 2026)) * c.Weight * 0.35);
                var previous = Enumerable.Range(12 - months, months).Sum(m => Revenue(m, c.Region, c.Channel, 2025)) * c.Weight * 0.35;
                return new CustomerRow(c.Name, Regions[c.Region], Channels[c.Channel], revenue, (int)Math.Round(revenue / 87.0), previous == 0 ? 0 : Math.Round((revenue / previous - 1) * 100, 1));
            })];
    }
}
