using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Klyroo.Dashboard;
using Klyroo.Display;
using Klyroo.Query;
using Klyroo.Query.EntityFrameworkCore;
using Klyroo.Spreadsheet;
using Klyroo.Table;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("# Klyroo Performance & Footprint Benchmarks");
Console.WriteLine($"- Runtime: `{RuntimeInformation.FrameworkDescription}` ({RuntimeInformation.OSArchitecture})");
Console.WriteLine($"- Build: `Release` | Date: `{DateTime.UtcNow:yyyy-MM-dd}`");
Console.WriteLine();

// 1. Klyroo.Query — 100,000 rows InMemory (Filter + Sort + Facets + Deep Offset vs Keyset Cursor)
const int rowCount = 100_000;
string[] regions = ["EU", "US", "APAC", "LATAM", "MEA"];
var deals = Enumerable.Range(1, rowCount)
    .Select(i => new Deal(i, $"Account-{i:D6}", regions[i % regions.Length], (i * 37 % 250_000) + 1_000m, i % 5 != 0))
    .ToArray();

var schema = DataListSchema.For<Deal>(s => s
    .RowKey(d => d.Id)
    .RowLabel(d => d.Name)
    .Search(d => d.Name, d => d.Region)
    .Field(d => d.Id, f => f.Key("id").Sortable())
    .Field(d => d.Region, f => f.Key("region").Sortable().Filter(DataListFilter.Option().WithFacets()))
    .Field(d => d.Amount, f => f.Key("amount").Sortable().Filter(DataListFilter.Number(min: 0)))
    .Field(d => d.Active, f => f.Key("active").Filter(DataListFilter.Boolean()))
    .DefaultSort(d => d.Id)
    .MaxPageSize(100));

var filterReq = new DataListRequest(
    DataListPaging.Offset.ForPage(1, 50),
    [new DataListSort("amount", Descending: true)],
    DataListFilterGroup.And(
        new FilterRule("active", FilterOperator.Is, new FilterValue.Boolean(true)),
        new FilterRule("amount", FilterOperator.Between, new FilterValue.Range(new FilterValue.Number(50_000m), new FilterValue.Number(200_000m)))),
    facets: [new DataListFacetRequest("region", Mode: FacetMode.ExcludeSelf)]);

var (inMemMs, inMemKb) = Measure("Klyroo.Query InMemory 100k rows (Filter + Sort + 5 Facets)", 5, () =>
{
    var r = schema.ExecuteInMemory(deals, filterReq);
    if (r.Items.Count != 50) throw new InvalidOperationException();
});

// 2. EF Core SQLite — 50,000 rows indexed on Id: Deep OFFSET (page 999) vs Keyset Cursor
await using var conn = new SqliteConnection("DataSource=:memory:");
await conn.OpenAsync();
var dbOpts = new DbContextOptionsBuilder<BenchDbContext>().UseSqlite(conn).Options;
await using var db = new BenchDbContext(dbOpts);
await db.Database.EnsureCreatedAsync();
db.Deals.AddRange(deals.Take(50_000));
await db.SaveChangesAsync();

var deepOffsetReq = new DataListRequest(
    DataListPaging.Offset.ForPage(999, 50),
    [new DataListSort("id", Descending: false)],
    DataListFilterGroup.Empty);

// Seed a cursor at row 49,900
var seedReq = new DataListRequest(
    new DataListPaging.Cursor(null, Take: 50, Count: false),
    [new DataListSort("id", Descending: false)],
    DataListFilterGroup.And(new FilterRule("id", FilterOperator.Gt, new FilterValue.Number(49_850m))));
var seedQ = schema.Apply(db.Deals.AsNoTracking(), seedReq);
var seedRows = await EfAsyncQueryExecutor.ToListAsync(seedQ.Rows);
var deepCursorToken = schema.ToResult(seedReq, seedRows).NextCursor!;

var deepCursorReq = new DataListRequest(
    new DataListPaging.Cursor(deepCursorToken, Take: 50, Count: false),
    [new DataListSort("id", Descending: false)],
    DataListFilterGroup.Empty);

var (sqlOffsetMs, sqlOffsetKb) = await MeasureAsync("EF Core SQLite 50k rows — Deep OFFSET (page 999)", 10, async () =>
{
    var q = schema.Apply(db.Deals.AsNoTracking(), deepOffsetReq);
    var rows = await EfAsyncQueryExecutor.ToListAsync(q.Page);
    if (rows.Count != 50) throw new InvalidOperationException();
});

var (sqlCursorMs, sqlCursorKb) = await MeasureAsync("EF Core SQLite 50k rows — Keyset Cursor (row 49,900+)", 10, async () =>
{
    var q = schema.Apply(db.Deals.AsNoTracking(), deepCursorReq);
    var rows = await EfAsyncQueryExecutor.ToListAsync(q.Rows);
    var res = schema.ToResult(deepCursorReq, rows);
    if (res.Items.Count != 50) throw new InvalidOperationException();
});

// 3. Klyroo.Spreadsheet — 10,000 CSV rows (NFD header normalization + email + decimal + duplicate check)
var csvSb = new StringBuilder(600_000);
csvSb.AppendLine("Code;Émail;Amount");
for (var i = 1; i <= 10_000; i++)
{
    var amt = (i * 1.25).ToString("F2", CultureInfo.InvariantCulture).Replace('.', ',');
    csvSb.Append($"SKU-{i:D6};user{i}@klyroo.dev;{amt}\n");
}
var csvText = csvSb.ToString();
var sheetSchema = new SpreadsheetSchema
{
    Key = "bench-csv",
    RowKeyField = "code",
    Duplicates = SpreadsheetDuplicatesPolicy.Error,
    Columns =
    [
        new SpreadsheetColumnSchema { Key = "code", Label = "Code", Required = true },
        new SpreadsheetColumnSchema { Key = "email", Label = "Email", Required = true, Rules = [SpreadsheetRule.Email()] },
        new SpreadsheetColumnSchema { Key = "amount", Label = "Amount", Kind = SpreadsheetColumnKind.Number, DecimalSeparator = ',', Required = true, Rules = [SpreadsheetRule.Min(0)] }
    ]
};

var (sheetMs, sheetKb) = Measure("Klyroo.Spreadsheet 10k CSV rows (Parse + NFD + Validate + Dedup)", 5, () =>
{
    var imp = new SpreadsheetImporter(sheetSchema);
    imp.LoadText(csvText, "bench.csv");
    if (imp.ImportableRows.Count != 10_000) throw new InvalidOperationException();
});

// 4. Klyroo.Table — 2,000-node 3-level hierarchy flattening
var treeId = 1;
var treeRoots = new List<TreeItem>();
for (var r = 0; r < 20; r++)
{
    var branches = new List<TreeItem>();
    for (var b = 0; b < 10; b++)
    {
        var leaves = new List<TreeItem>();
        for (var l = 0; l < 9; l++) leaves.Add(new TreeItem(treeId++, $"Leaf-{treeId}"));
        branches.Add(new TreeItem(treeId++, $"Branch-{treeId}", leaves));
    }
    treeRoots.Add(new TreeItem(treeId++, $"Root-{treeId}", branches));
}
var treeSchema = DataListSchema.For<TreeItem>(s => s.RowKey(x => x.Id).RowLabel(x => x.Name));
var tree = new DataListTree<TreeItem> { Children = x => x.Children, DefaultExpandedDepth = 3 };

var (treeMs, treeKb) = Measure("Klyroo.Table BuildTree (2,020 nodes, 3 levels)", 25, () =>
{
    var (_, flat) = tree.BuildTree(treeRoots, treeSchema);
    if (flat.Count != 2020) throw new InvalidOperationException();
});

// 5. Klyroo.Display — RFC 4180 CSV preview parser (1,000-row cap on 10k CSV)
var (dispMs, dispKb) = Measure("Klyroo.Display FilePreviewService.ParseCsv (10k input, 1k preview cap)", 15, () =>
{
    var parsed = FilePreviewService.ParseCsv(csvText, "bench.csv", maxRows: 1000);
    if (parsed.Rows.Count != 1000 || !parsed.Truncated) throw new InvalidOperationException();
});

// 6. Measure compiled Core DLL sizes (NativeAOT / WASM footprint indicator)
var binDir = AppContext.BaseDirectory;
var coreDlls = Directory.EnumerateFiles(binDir, "Klyroo.*.Core.dll").OrderBy(f => f).ToList();
var totalCoreKb = coreDlls.Sum(f => new FileInfo(f).Length) / 1024.0;

Console.WriteLine("## Runtime Benchmarks (Median)");
Console.WriteLine("| Scenario | Median Time | Allocated |");
Console.WriteLine("| :--- | ---: | ---: |");
Console.WriteLine($"| Klyroo.Query InMemory — **100,000 rows** (Filter + Sort + 5 Facets) | **{inMemMs:F2} ms** | {inMemKb:N0} KB |");
Console.WriteLine($"| Klyroo.Query EF Core SQLite — **50,000 rows Deep OFFSET** (page 999) | **{sqlOffsetMs:F2} ms** | {sqlOffsetKb:N0} KB |");
Console.WriteLine($"| Klyroo.Query EF Core SQLite — **50,000 rows Keyset Cursor** (row 49,900+) | **{sqlCursorMs:F2} ms** | {sqlCursorKb:N0} KB |");
Console.WriteLine($"| Klyroo.Spreadsheet — **10,000 CSV rows** (Parse + NFD + Rules + Dedup) | **{sheetMs:F2} ms** | {sheetKb:N0} KB |");
Console.WriteLine($"| Klyroo.Table — **2,020-node Tree** (`BuildTree`, 3 levels) | **{treeMs:F2} ms** | {treeKb:N0} KB |");
Console.WriteLine($"| Klyroo.Display — **RFC 4180 CSV Preview** (10k input, 1k cap) | **{dispMs:F2} ms** | {dispKb:N0} KB |");
Console.WriteLine();
Console.WriteLine($"**Keyset Cursor Speedup at row 49,900 (SQLite)**: **{sqlOffsetMs / Math.Max(0.01, sqlCursorMs):F1}x faster** than `OFFSET`!");
Console.WriteLine();
Console.WriteLine("## Headless Core Assembly Footprint (Release IL, pre-trim / pre-brotli)");
Console.WriteLine("| Assembly | Size (KB) |");
Console.WriteLine("| :--- | ---: |");
foreach (var dll in coreDlls)
    Console.WriteLine($"| `{Path.GetFileName(dll)}` | {new FileInfo(dll).Length / 1024.0:F1} KB |");
Console.WriteLine($"| **Total (all {coreDlls.Count} Core engines)** | **{totalCoreKb:F1} KB** |");

static (double MedianMs, double AllocKb) Measure(string _, int iterations, Action body)
{
    body(); // warmup
    var times = new List<double>(iterations);
    long alloc = 0;
    for (var i = 0; i < iterations; i++)
    {
        GC.Collect();
        var before = GC.GetAllocatedBytesForCurrentThread();
        var sw = Stopwatch.StartNew();
        body();
        sw.Stop();
        alloc = GC.GetAllocatedBytesForCurrentThread() - before;
        times.Add(sw.Elapsed.TotalMilliseconds);
    }
    times.Sort();
    return (times[times.Count / 2], alloc / 1024.0);
}

static async Task<(double MedianMs, double AllocKb)> MeasureAsync(string _, int iterations, Func<Task> body)
{
    await body(); // warmup
    var times = new List<double>(iterations);
    long alloc = 0;
    for (var i = 0; i < iterations; i++)
    {
        var before = GC.GetTotalAllocatedBytes(precise: false);
        var sw = Stopwatch.StartNew();
        await body();
        sw.Stop();
        alloc = GC.GetTotalAllocatedBytes(precise: false) - before;
        times.Add(sw.Elapsed.TotalMilliseconds);
    }
    times.Sort();
    return (times[times.Count / 2], alloc / 1024.0);
}

public sealed class Deal
{
    public Deal() { }
    public Deal(int id, string name, string region, decimal amount, bool active)
    { Id = id; Name = name; Region = region; Amount = amount; Active = active; }
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Region { get; set; } = "";
    public decimal Amount { get; set; }
    public bool Active { get; set; }
}

public sealed class BenchDbContext(DbContextOptions<BenchDbContext> options) : DbContext(options)
{
    public DbSet<Deal> Deals => Set<Deal>();
}

public sealed record TreeItem(int Id, string Name, IReadOnlyList<TreeItem>? Children = null);
