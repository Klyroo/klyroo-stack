namespace Klyroo.Playground.Data;

public enum EmployeeStatus { Active, Remote, OnLeave }

/// <summary>A row of the sample directory. <see cref="Reports"/> is filled for the tree view only.</summary>
public sealed class Employee
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Department { get; init; }
    public required string Title { get; init; }
    public required string Country { get; init; }
    public EmployeeStatus Status { get; set; }
    public decimal Salary { get; set; }
    public required DateOnly Hired { get; init; }
    public int? ManagerId { get; init; }
    public List<Employee>? Reports { get; set; }
}

/// <summary>Two hundred sample employees, generated the same way on every load (a fixed seed), with a reporting line for the tree view.</summary>
public static class Employees
{
    public static readonly string[] Departments = ["Engineering", "Sales", "Marketing", "Support", "Finance", "Design", "Operations"];
    public static readonly string[] Countries = ["France", "Germany", "Spain", "Italy", "Canada", "United States", "Japan", "Brazil"];

    private static readonly string[] First = ["Ada", "Alan", "Grace", "Linus", "Margaret", "Dennis", "Barbara", "Ken", "Radia", "Tim", "Hedy", "Donald", "Frances", "Edsger", "Katherine", "Guido", "Anita", "Bjarne", "Sophie", "Yukihiro", "Lena", "Mateo", "Noor", "Ines", "Tomas", "Aiko", "Pablo", "Clara", "Omar", "Elsa"];
    private static readonly string[] Last = ["Lovelace", "Turing", "Hopper", "Torvalds", "Hamilton", "Ritchie", "Liskov", "Thompson", "Perlman", "Berners", "Lamarr", "Knuth", "Allen", "Dijkstra", "Johnson", "Rossum", "Borg", "Stroustrup", "Wilson", "Matsumoto", "Weber", "Garcia", "Haddad", "Moreau", "Novak", "Tanaka", "Silva", "Rossi", "Khan", "Lindqvist"];
    private static readonly string[] Titles = ["Engineer", "Senior engineer", "Staff engineer", "Account executive", "Account manager", "Marketing manager", "Content lead", "Support specialist", "Support lead", "Financial analyst", "Controller", "Product designer", "Design lead", "Operations analyst", "Operations manager"];

    private static readonly Lazy<IReadOnlyList<Employee>> Flat = new(Build);
    private static readonly Lazy<IReadOnlyList<Employee>> Roots = new(BuildTree);

    /// <summary>The flat list of 200 rows.</summary>
    public static IReadOnlyList<Employee> All => Flat.Value;

    /// <summary>The same people as a forest: the rows without a manager, each carrying its reports. The rows are copies, so the flat list stays flat.</summary>
    public static IReadOnlyList<Employee> Tree => Roots.Value;

    private static IReadOnlyList<Employee> Build()
    {
        var rng = new Random(20260101);
        var list = new List<Employee>(200);
        for (var i = 1; i <= 200; i++)
        {
            var first = First[rng.Next(First.Length)];
            var last = Last[rng.Next(Last.Length)];
            var dept = Departments[rng.Next(Departments.Length)];
            var seniority = rng.Next(0, 4);
            var salary = Math.Round((38_000 + seniority * 22_000 + rng.Next(0, 22_000)) / 500m) * 500m;
            list.Add(new Employee
            {
                Id = i,
                Name = $"{first} {last}",
                Email = $"{first}.{last}{i}@example.test".ToLowerInvariant(),
                Department = dept,
                Title = Titles[Math.Min(Titles.Length - 1, (Array.IndexOf(Departments, dept) * 2 + (seniority > 1 ? 1 : 0)) % Titles.Length)],
                Country = Countries[rng.Next(Countries.Length)],
                Status = rng.Next(10) switch { 0 => EmployeeStatus.OnLeave, < 4 => EmployeeStatus.Remote, _ => EmployeeStatus.Active },
                Salary = salary,
                Hired = new DateOnly(2016, 1, 1).AddDays(rng.Next(0, 365 * 10 + 120)),
                ManagerId = i <= 8 ? null : rng.Next(1, Math.Min(i, 60)),
            });
        }
        return list;
    }

    private static IReadOnlyList<Employee> BuildTree()
    {
        var copies = All.Select(e => new Employee
        {
            Id = e.Id, Name = e.Name, Email = e.Email, Department = e.Department, Title = e.Title, Country = e.Country,
            Status = e.Status, Salary = e.Salary, Hired = e.Hired, ManagerId = e.ManagerId,
        }).ToDictionary(e => e.Id);
        var roots = new List<Employee>();
        foreach (var e in copies.Values.OrderBy(e => e.Id))
        {
            if (e.ManagerId is { } m && copies.TryGetValue(m, out var manager)) (manager.Reports ??= []).Add(e);
            else roots.Add(e);
        }
        return roots;
    }
}
