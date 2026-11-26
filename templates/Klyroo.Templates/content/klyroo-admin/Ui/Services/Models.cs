namespace KlyrooAdminApp.Services;

/// <summary>A customer of the company, as the back office sees it.</summary>
public sealed record Customer(int Id, string Name, string Region, string Tier, decimal Arr, int Health, DateOnly Since, string Contact);

/// <summary>The numbers at the top of the dashboard, with the value of the month before for the comparison.</summary>
public sealed record Summary(int Customers, int PreviousCustomers, double Arr, double PreviousArr, int AtRisk, int PreviousAtRisk, double AverageHealth);

/// <summary>The annual recurring revenue at the end of one month.</summary>
public sealed record ArrPoint(string Month, double Arr);

/// <summary>The part of the revenue that one region brings.</summary>
public sealed record RegionShare(string Region, double Arr, int Customers);

/// <summary>One of the biggest customers, for the table of the dashboard.</summary>
public sealed record TopCustomer(string Name, string Region, double Arr, double Health);

/// <summary>One line of the activity feed.</summary>
public sealed record ActivityEntry(string Title, string Detail, DateTimeOffset Time, string Initials);
