using System.Globalization;

namespace KlyrooAdminApp.Services;

/// <summary>
/// The data of the app, kept in memory so that the template runs with no database. Replace the bodies with calls to your own data access: the pages only
/// use these methods, and every read takes a cancellation token, which Klyroo cancels when a newer request replaces the one in flight.
/// </summary>
public sealed class CustomerStore
{
    public static readonly string[] Regions = ["EMEA", "Americas", "APAC"];
    public static readonly string[] Tiers = ["Standard", "Plus", "Enterprise"];

    private readonly List<Customer> _customers = Seed();
    private int _nextId = 100;

    /// <summary>Every customer, in the order they were added, newest first.</summary>
    public IReadOnlyList<Customer> All => _customers;

    public Customer? Find(int id) => _customers.Find(c => c.Id == id);

    public Customer Add(string name, string contact, string region, string tier, decimal arr, int health = 70)
    {
        var customer = new Customer(_nextId++, name, region, tier, arr, health, DateOnly.FromDateTime(DateTime.UtcNow), contact);
        _customers.Insert(0, customer);
        return customer;
    }

    public void Update(Customer customer)
    {
        var index = _customers.FindIndex(c => c.Id == customer.Id);
        if (index >= 0) _customers[index] = customer;
    }

    public void Remove(IEnumerable<int> ids)
    {
        var set = ids.ToHashSet();
        _customers.RemoveAll(c => set.Contains(c.Id));
    }

    public async Task<Summary> GetSummaryAsync(string? region, CancellationToken cancellationToken)
    {
        await Task.Delay(80, cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var scope = InRegion(region).ToList();
        var before = scope.Where(c => c.Since <= today.AddDays(-30)).ToList();
        return new Summary(
            Customers: scope.Count,
            PreviousCustomers: before.Count,
            Arr: (double)scope.Sum(c => c.Arr),
            PreviousArr: (double)before.Sum(c => c.Arr),
            AtRisk: scope.Count(c => c.Health < 50),
            PreviousAtRisk: before.Count(c => c.Health < 50),
            AverageHealth: scope.Count == 0 ? 0 : scope.Average(c => c.Health));
    }

    /// <summary>The recurring revenue at the end of each of the last twelve months, from the customers that had signed by then.</summary>
    public async Task<ArrPoint[]> GetArrTrendAsync(string? region, CancellationToken cancellationToken)
    {
        await Task.Delay(120, cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var scope = InRegion(region).ToList();
        return
        [
            .. Enumerable.Range(0, 12).Select(i =>
            {
                var end = today.AddMonths(i - 11);
                return new ArrPoint(end.ToString("MMM", CultureInfo.InvariantCulture), (double)scope.Where(c => c.Since <= end).Sum(c => c.Arr));
            })
        ];
    }

    public async Task<RegionShare[]> GetRegionSharesAsync(string? region, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        var scope = InRegion(region).ToList();
        return
        [
            .. Regions
                .Select(r => new RegionShare(r, (double)scope.Where(c => c.Region == r).Sum(c => c.Arr), scope.Count(c => c.Region == r)))
                .Where(s => s.Customers > 0)
                .OrderByDescending(s => s.Arr)
        ];
    }

    public async Task<TopCustomer[]> GetTopCustomersAsync(string? region, CancellationToken cancellationToken)
    {
        await Task.Delay(140, cancellationToken);
        return [.. InRegion(region).OrderByDescending(c => c.Arr).Take(5).Select(c => new TopCustomer(c.Name, c.Region, (double)c.Arr, c.Health))];
    }

    public async Task<ActivityEntry[]> GetActivityAsync(string? region, CancellationToken cancellationToken)
    {
        await Task.Delay(160, cancellationToken);
        return
        [
            .. InRegion(region).Take(6).Select(c => new ActivityEntry(
                $"{c.Name} signed a {c.Tier} contract",
                $"{Money.Format(c.Arr)} a year, contact {c.Contact}",
                new DateTimeOffset(c.Since.ToDateTime(new TimeOnly(9, 0)), TimeSpan.Zero),
                Initials(c.Name)))
        ];
    }

    private IEnumerable<Customer> InRegion(string? region) =>
        string.IsNullOrEmpty(region) ? _customers : _customers.Where(c => string.Equals(c.Region, region, StringComparison.OrdinalIgnoreCase));

    private static string Initials(string name) =>
        string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => char.ToUpperInvariant(w[0])));

    private static List<Customer> Seed()
    {
        string[] names =
        [
            "Acme Corp", "Nordic Retail", "Pacific Labs", "Blue Harbor", "Orbit Analytics", "Fernwood Studio", "Kite Logistics", "Granite Bank",
            "Lumen Health", "Maple Foods", "Quanta Cloud", "Redwood Energy", "Summit Travel", "Tidal Media", "Umbra Security", "Vertex Legal",
            "Willow Schools", "Zenith Motors", "Atlas Freight", "Beacon Labs", "Cobalt Retail", "Delta Printing", "Ember Coffee", "Flint Tools",
        ];
        string[] contacts = ["ada", "grace", "linus", "margaret", "alan", "edsger"];
        decimal[] tierArr = [12_000m, 48_000m, 140_000m];
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var customers = new List<Customer>();
        for (var i = 0; i < names.Length; i++)
        {
            var tier = i % 3;
            var domain = names[i].ToLowerInvariant().Replace(" ", "");
            customers.Add(new Customer(i + 1, names[i], Regions[(i + i / 3) % 3], Tiers[tier], tierArr[tier] + (i * 1_370 % 9_000),
                30 + (i * 37) % 70, today.AddDays(-(i * 15 + 4)), $"{contacts[i % contacts.Length]}@{domain}.example"));
        }
        return customers;
    }
}
