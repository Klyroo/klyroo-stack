namespace KlyrooSaaSApp.Services;

/// <summary>A customer of the product.</summary>
public sealed record Account(int Id, string Name, string Owner, string Plan, string Status, decimal Mrr, DateOnly Since);

/// <summary>The numbers at the top of the overview, with the value of the month before for the comparison.</summary>
public sealed record Summary(double Mrr, double PreviousMrr, int ActiveAccounts, int PreviousActiveAccounts, double ChurnRate, double PreviousChurnRate);

/// <summary>The monthly recurring revenue at the end of one month.</summary>
public sealed record RevenuePoint(string Month, double Mrr);

/// <summary>The part of the revenue that one plan brings.</summary>
public sealed record PlanShare(string Plan, double Mrr, int Accounts);

/// <summary>One line of the activity feed.</summary>
public sealed record ActivityEntry(string Title, string Detail, DateTimeOffset Time, string Initials);
