using System.Globalization;
using Klyroo.Query;

namespace KlyrooSaaSApp.Services;

/// <summary>
/// The data of the app, kept in memory so that the template runs with no database. Replace the bodies with calls to your own data access: the pages only
/// use these methods, and every read takes a cancellation token, which Klyroo cancels when a newer request replaces the one in flight.
/// </summary>
public sealed class AccountStore
{
    public static readonly string[] Plans = ["Starter", "Team", "Business"];
    public static readonly string[] Statuses = ["Active", "Trial", "Past due", "Churned"];

    private static readonly decimal[] PlanPrices = [49m, 199m, 899m];

    private readonly List<Account> _accounts = Seed();
    private int _nextId = 100;

    /// <summary>Every account, newest first.</summary>
    public IReadOnlyList<Account> All => _accounts;

    public Account? Find(int id) => _accounts.Find(a => a.Id == id);

    public Account Add(string name, string owner, string plan, decimal mrr)
    {
        var account = new Account(_nextId++, name, owner, plan, "Trial", mrr, DateOnly.FromDateTime(DateTime.UtcNow));
        _accounts.Insert(0, account);
        return account;
    }

    public void Update(Account account)
    {
        var index = _accounts.FindIndex(a => a.Id == account.Id);
        if (index >= 0) _accounts[index] = account;
    }

    public void Remove(IEnumerable<int> ids)
    {
        var set = ids.ToHashSet();
        _accounts.RemoveAll(a => set.Contains(a.Id));
    }

    /// <summary>One page of accounts for a request of the list: the schema applies the search, the filters, the sort and the paging.</summary>
    public async ValueTask<DataListResult<Account>> QueryAsync(DataListRequest request, CancellationToken cancellationToken)
    {
        await Task.Delay(60, cancellationToken);
        return AccountSchema.Schema.ExecuteInMemory(_accounts, request);
    }

    public async Task<Summary> GetSummaryAsync(string? plan, CancellationToken cancellationToken)
    {
        await Task.Delay(80, cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var scope = InPlan(plan).ToList();
        var paying = scope.Where(a => a.Status is "Active" or "Past due").ToList();
        var before = paying.Where(a => a.Since <= today.AddDays(-30)).ToList();
        var churnBase = Math.Max(1, scope.Count(a => a.Since <= today.AddDays(-30)));
        var churned = scope.Count(a => a.Status == "Churned");
        return new Summary(
            Mrr: (double)paying.Sum(a => a.Mrr),
            PreviousMrr: (double)before.Sum(a => a.Mrr),
            ActiveAccounts: scope.Count(a => a.Status == "Active"),
            PreviousActiveAccounts: scope.Count(a => a.Status == "Active" && a.Since <= today.AddDays(-30)),
            ChurnRate: 100d * churned / Math.Max(1, scope.Count),
            PreviousChurnRate: 100d * Math.Max(0, churned - 1) / churnBase);
    }

    /// <summary>The recurring revenue at the end of each of the last twelve months, from the accounts that were paying then.</summary>
    public async Task<RevenuePoint[]> GetRevenueAsync(string? plan, CancellationToken cancellationToken)
    {
        await Task.Delay(120, cancellationToken);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var paying = InPlan(plan).Where(a => a.Status is "Active" or "Past due").ToList();
        return
        [
            .. Enumerable.Range(0, 12).Select(i =>
            {
                var end = today.AddMonths(i - 11);
                return new RevenuePoint(end.ToString("MMM", CultureInfo.InvariantCulture), (double)paying.Where(a => a.Since <= end).Sum(a => a.Mrr));
            })
        ];
    }

    public async Task<PlanShare[]> GetPlanSharesAsync(string? plan, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        var paying = InPlan(plan).Where(a => a.Status is "Active" or "Past due").ToList();
        return
        [
            .. Plans
                .Select(p => new PlanShare(p, (double)paying.Where(a => a.Plan == p).Sum(a => a.Mrr), paying.Count(a => a.Plan == p)))
                .Where(s => s.Accounts > 0)
                .OrderByDescending(s => s.Mrr)
        ];
    }

    public async Task<ActivityEntry[]> GetActivityAsync(string? plan, CancellationToken cancellationToken)
    {
        await Task.Delay(140, cancellationToken);
        return
        [
            .. InPlan(plan).Take(6).Select(a => new ActivityEntry(
                a.Status == "Trial" ? $"{a.Name} started a trial" : $"{a.Name} is on the {a.Plan} plan",
                $"{Money.Format(a.Mrr)} a month, owned by {a.Owner}",
                new DateTimeOffset(a.Since.ToDateTime(new TimeOnly(9, 0)), TimeSpan.Zero),
                Initials(a.Name)))
        ];
    }

    private IEnumerable<Account> InPlan(string? plan) =>
        string.IsNullOrEmpty(plan) ? _accounts : _accounts.Where(a => string.Equals(a.Plan, plan, StringComparison.OrdinalIgnoreCase));

    private static string Initials(string name) =>
        string.Concat(name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(w => char.ToUpperInvariant(w[0])));

    private static List<Account> Seed()
    {
        string[] names =
        [
            "Acme Corp", "Nordic Retail", "Pacific Labs", "Blue Harbor", "Orbit Analytics", "Fernwood Studio", "Kite Logistics", "Granite Bank",
            "Lumen Health", "Maple Foods", "Quanta Cloud", "Redwood Energy", "Summit Travel", "Tidal Media", "Umbra Security", "Vertex Legal",
            "Willow Schools", "Zenith Motors", "Atlas Freight", "Beacon Labs", "Cobalt Retail", "Delta Printing", "Ember Coffee", "Flint Tools",
        ];
        string[] owners = ["ada", "grace", "linus", "margaret", "alan", "edsger"];
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var accounts = new List<Account>();
        for (var i = 0; i < names.Length; i++)
        {
            var planIndex = i % 3;
            var seats = 1 + i % 4;
            var status = i == 2 ? "Trial" : i % 11 == 7 ? "Churned" : i % 9 == 4 ? "Past due" : "Active";
            var domain = names[i].ToLowerInvariant().Replace(" ", "");
            accounts.Add(new Account(i + 1, names[i], $"{owners[i % owners.Length]}@{domain}.example", Plans[planIndex], status,
                PlanPrices[planIndex] * seats, today.AddDays(-(i * 15 + 4))));
        }
        return accounts;
    }
}
