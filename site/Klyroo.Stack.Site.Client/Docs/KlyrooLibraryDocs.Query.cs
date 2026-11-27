using Klyroo.Table;
using Klyroo.Query;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class KlyrooLibraryDocs
{
    private static LibraryDocTree BuildQuery() => new(
        "query", "Klyroo.Query", "#ff4154",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "Understand the complete Klyroo.Query architecture: one strongly typed schema, one immutable AST, reversible URL codecs, InMemory/EF Core executors, reactive QueryState, and seamless integration across every Klyroo library.",
                [
                    new("overview", "What is Klyroo.Query?",
                        Paragraphs:
                        [
                            "Klyroo.Query is the headless data-querying and URL-state backbone of Klyroo. Instead of scattering ad-hoc search strings, magic column names, LINQ switches, and NavigationManager hacks across pages and controllers, you declare the queryable contract of a model once using DataListSchema.For<T>().",
                            "From that single immutable schema, Klyroo.Query powers the entire lifecycle: parsing untrusted URL search parameters into a DataListState, sanitizing limits and unknown keys, serializing DataListRequest payloads over HTTP with zero-reflection NativeAOT JSON, translating filters/sorts/facets/cursors to SQL via IQueryable/EF Core or running identically in memory, and keeping browser URLs and history synchronized via QueryState."
                        ],
                        Bullets:
                        [
                            "Klyroo.Query.Core — 100% headless C#, zero UI dependencies, NativeAOT & trimming safe (<IsAotCompatible>true</IsAotCompatible>). Ships DataListSchema<T>, DataListRequest, DataListResult<T>, FilterRule, FilterValue, QueryCodecs, DataListStateQuery, InMemoryDataList, QueryableDataList, and DataListJsonContext.",
                            "Klyroo.Query — Multiplatform Razor Class Library (browser, ios, android, maccatalyst, windows). Ships reactive QueryState, typed QueryValue<T>, record-backed QueryGroup<TRecord>, collision-detecting QueryClaim, and AddKlyrooQuery().",
                            "Klyroo.Query.EntityFrameworkCore — Server-only adapter exposing EfAsyncQueryExecutor.CountAsync and ToListAsync so client WASM/MAUI bundles never pull in EF Core."
                        ],
                        LiveDemoKey: "query-inmemory",
                        LiveDemoTitle: "Klyroo.Query · Live DataListSchema.ExecuteInMemory"),
                    new("pipeline", "How Klyroo.Query Works End-to-End (The 5-Stage Pipeline)",
                        Paragraphs:
                        [
                            "Every query in Klyroo.Query flows through five deterministic stages. You can enter or exit the pipeline at any stage depending on whether you are building a local WASM list, a Static SSR Blazor page, an EF Core REST API, or a .NET MAUI app."
                        ],
                        TableHeaders: ["Stage", "Types Involved", "What Happens"],
                        TableRows:
                        [
                            new("1. Declare Contract", "DataListSchema.For<T>(s => ...)", "Compiles strongly typed expression accessors once at startup: RowKey, Search fields, sortable/filterable fields, collection predicates, defaults, and safety caps."),
                            new("2. URL ↔ State", "QueryState · QueryGroup<T> · DataListStateQuery", "Reads the browser URL (?q=alice&p=2&s=-salary&f.dept=Eng) into an immutable DataListState (or custom record), or writes only non-default keys back in a single batched navigation."),
                            new("3. Request & Sanitize", "state.ToRequest(...) · schema.Sanitize(req)", "Converts state into an immutable DataListRequest (Offset or Keyset Cursor, Sort, Filter, Search, Facets, TimeZone) and strips unknown keys, invalid operators, and oversized Take counts."),
                            new("4. Execute", "schema.ExecuteInMemory(...) OR schema.Apply(...) + EfAsyncQueryExecutor", "Evaluates predicates, facets (ExcludeSelf/IncludeSelf), nulls-last multi-column sorts, RowKey tie-breakers, and paging — identically in memory or translated to parameterized SQL."),
                            new("5. Deliver Result", "DataListResult<T> · DataListJsonContext", "Returns Items, TotalCount, NextCursor, and Facets to Klyroo.Table, Klyroo.Dashboard, Klyroo.Display, or any custom Razor/MAUI component — with NativeAOT JSON serialization over HTTP.")
                        ],
                        Callout: "Because every stage relies on the exact same DataListSchema<T> placed in a shared C# class library, your client URL parser, your HTTP serializer, and your server SQL translator can never drift out of sync."),
                    new("packages", "Package & Target Matrix",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Klyroo.Query.Core", "DataListSchema, DataListRequest, DataListResult<T>, FilterRule, FilterValue, QueryCodecs, DataListStateQuery, InMemoryDataList, QueryableDataList, DataListJsonContext", "NativeAOT · Any .NET 10 host (API, Worker, CLI, WASM, MAUI)"),
                            new("Klyroo.Query", "QueryState, QueryValue<T>, QueryGroup<TRecord>, QueryClaim, QueryHistory, AddKlyrooQuery()", "browser, ios, android, maccatalyst, windows"),
                            new("Klyroo.Query.EntityFrameworkCore", "EfAsyncQueryExecutor.CountAsync, EfAsyncQueryExecutor.ToListAsync", "EF Core 10 (SQLite, PostgreSQL, SQL Server)")
                        ]),
                    new("guards", "Built-In Safety Guardrails (schema.Sanitize)",
                        Paragraphs: ["Every executor automatically runs schema.Sanitize(request) before evaluating a single row. Even if a malicious client crafts a hostile URL or raw JSON body, your server stays protected without writing manual validation boilerplate."],
                        TableHeaders: ["Schema Limit", "Default", "Protects Against"],
                        TableRows:
                        [
                            new("MaxPageSize", "500", "Oversized Take requests (silently clamped to 1..MaxPageSize)"),
                            new("MaxFilterRules", "100", "Deeply nested or abusive filter ASTs"),
                            new("MaxListValues", "500", "Unbounded FilterOperator.IsAnyOf lists producing giant SQL IN (...) clauses"),
                            new("MaxFacetBuckets", "1000", "High-cardinality facet GROUP BY explosions (sets DataListFacetResult.Truncated = true)"),
                            new("DataListState.MaxPage", "1,000,000", "Integer overflow when computing Skip = (Page - 1) * PageSize")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install only the packages each project needs so WebAssembly and .NET MAUI binaries stay trimmed while server projects get full EF Core SQL translation.",
                [
                    new("nuget", "1. Install NuGet Packages",
                        Paragraphs:
                        [
                            "In a multi-project solution (Shared + Server + Client/MAUI), reference Klyroo.Query.Core in your Shared models library, Klyroo.Query in your Blazor/MAUI UI project, and Klyroo.Query.EntityFrameworkCore strictly in your Server API project."
                        ],
                        CodeSamples:
                        [
                            new("Terminal", """
                                # Shared project (models + DataListSchema<T> definitions)
                                dotnet add package Klyroo.Query.Core

                                # Blazor Web / WebAssembly / .NET MAUI Blazor Hybrid project
                                dotnet add package Klyroo.Query

                                # Server / Minimal API project (only if using EF Core)
                                dotnet add package Klyroo.Query.EntityFrameworkCore
                                """)
                        ]),
                    new("di", "2. Register Services",
                        Paragraphs:
                        [
                            "Call AddKlyrooQuery() in Program.cs to register the scoped QueryState service. In Blazor Interactive Auto apps, register AddKlyrooQuery() in BOTH the Server Program.cs and the WebAssembly Client Program.cs so prerendering and client interactivity share identical URL state behavior.",
                            "When using the full Klyroo meta-package, AddKlyrooStack() already calls AddKlyrooQuery() automatically."
                        ],
                        CodeSamples:
                        [
                            new("Program.cs (Blazor Server / Client / MAUI)", """
                                using Klyroo.Query;

                                builder.Services.AddKlyrooQuery();
                                """),
                            new("Program.cs (Or via full Klyroo)", """
                                using Klyroo.Stack;

                                builder.Services.AddKlyrooStack(); // Includes Klyroo.Query, Klyroo.Table, Klyroo.Form, Klyroo.Dashboard, Klyroo.Display, Klyroo.Spreadsheet, Klyroo.Agent & Klyroo.UI
                                """)
                        ]),
                    new("usings", "3. Import Namespaces",
                        CodeSamples:
                        [
                            new("_Imports.razor", """
                                @using Klyroo.Query
                                """),
                            new("GlobalUsings.cs (Server / Shared)", """
                                global using Klyroo.Query;
                                global using Klyroo.Query.EntityFrameworkCore; // Server project only
                                """)
                        ]),
                    new("json", "4. NativeAOT JSON Serialization (DataListJsonContext)",
                        Paragraphs: ["Klyroo.Query.Core ships a source-generated System.Text.Json JsonSerializerContext (DataListJsonContext) covering DataListRequest, DataListPaging subtypes, DataListFilterNode, FilterRule, DataListFilterGroup, and all 7 FilterValue subtypes. Use it directly or chain it into your Minimal API JsonSerializerOptions."],
                        CodeSamples:
                        [
                            new("JsonAot.cs", """
                                using System.Text.Json;
                                using Klyroo.Query;

                                // Serialize and deserialize polymorphic requests with zero reflection
                                string json = JsonSerializer.Serialize(request, DataListJsonContext.Default.DataListRequest);
                                DataListRequest? parsed = JsonSerializer.Deserialize(json, DataListJsonContext.Default.DataListRequest);

                                // Or register on ASP.NET Core Minimal APIs:
                                builder.Services.ConfigureHttpJsonOptions(o =>
                                    o.SerializerOptions.TypeInfoResolverChain.Insert(0, DataListJsonContext.Default));
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 2: CORE ENGINE
            // =========================================================================
            new("schema", "Typed Schema (DataListSchema)", "Core Engine",
                "Declare row identity, full-text search fields, scalar & nested fields, one-to-many collection predicates, remote option providers, query-only filters, and defaults in one immutable schema.",
                [
                    new("builder", "Fluent DataListSchema.For<T>",
                        Paragraphs:
                        [
                            "DataListSchema.For<T>(s => ...) compiles strongly typed property accessors and expression trees once at startup. Every field key is automatically derived in camelCase from the member path (e.g., e => e.Department.Name becomes \"department.name\") unless you override it with .Key(\"dept\").",
                            "Always store the returned DataListSchema<T> in a static readonly field. It is completely immutable and thread-safe across concurrent HTTP requests."
                        ],
                        CodeSamples:
                        [
                            new("EmployeeSchemas.cs", """
                                using Klyroo.Query;

                                public sealed record Tag(string Name);
                                public sealed record Department(string Name, string Code);
                                public sealed record Employee(
                                    int Id,
                                    string Name,
                                    string Email,
                                    int Age,
                                    decimal Salary,
                                    DateTimeOffset HiredAt,
                                    bool Active,
                                    Department? Department,
                                    IReadOnlyList<Tag> Tags);

                                public static class EmployeeSchemas
                                {
                                    public static readonly DataListSchema<Employee> Directory = DataListSchema.For<Employee>(s => s
                                        .RowKey(e => e.Id)
                                        .RowLabel(e => e.Name)
                                        .Search(e => e.Name, e => e.Email)
                                        .Field(e => e.Name, f => f.Key("name").Sortable().Filter(DataListFilter.Text()))
                                        .Field(e => e.Age, f => f.Sortable().Filter(DataListFilter.Number(min: 18, max: 120, step: 1)))
                                        .Field(e => e.Salary, f => f.Key("salary").Sortable().Filter(
                                            DataListFilter.Number(min: 0, max: 500_000, step: 1000)
                                                .WithDisplay(DataListNumberDisplay.InputAndSlider)))
                                        .Field(e => e.HiredAt, f => f.Key("hired").Sortable().Filter(DataListFilter.Date()))
                                        .Field(e => e.Active, f => f.Key("active").Filter(DataListFilter.Boolean().WithDefault(true)))
                                        .Field(e => e.Department!.Name, f => f.Key("dept").Sortable().Filter(
                                            DataListFilter.Option().WithFacets(FacetMode.ExcludeSelf)))
                                        .Collection(e => e.Tags, t => t.Name, f => f.Key("tag").Filter(
                                            DataListFilter.Option().WithFacets()))
                                        .QueryFilter("archived", DataListFilter.Boolean())
                                        .DefaultSort(e => e.Name)
                                        .DefaultPageSize(25)
                                        .MaxPageSize(200));
                                }
                                """)
                        ]),
                    new("builder-methods", "Complete DataListSchemaBuilder<T> API",
                        TableHeaders: ["Method", "Purpose", "Behavior & Notes"],
                        TableRows:
                        [
                            new("RowKey(e => e.Id)", "Unique row identity", "Required for keyset cursor paging, collection facets, and deterministic tie-breaking on equal sort values."),
                            new("RowLabel(e => e.Name)", "Human-readable row label", "Used by accessible UI consumers (e.g. checkbox aria-label 'Select Alice')."),
                            new("Search(e => e.A, e => e.B)", "Full-text search fields", "Matches when ANY listed string field contains the search term (OR semantics)."),
                            new("Field(e => e.Prop, cfg)", "Scalar or nested path", "Null-safe in memory (e.Department?.Name never throws) and native SQL navigation in EF Core."),
                            new("Collection(e => e.Tags, t => t.Name, cfg)", "One-to-many collection predicate", "Translates to e.Tags.Any(t => ...) in LINQ/SQL; IsNot translates to !e.Tags.Any(...). Collection fields cannot be sorted."),
                            new("QueryFilter(key, filter)", "Virtual query-only filter", "Validated and URL-encoded by Klyroo.Query without mapping to a single entity column; read with schema.QueryFilterRules(request) before calling schema.Apply."),
                            new("DefaultSort / DefaultPageSize / DefaultLayout", "Initial DataListState defaults", "Values equal to schema.DefaultState are omitted from the URL to keep URLs short and clean."),
                            new("MaxPageSize / MaxFilterRules / MaxListValues / MaxFacetBuckets", "Defensive guardrails", "Enforced automatically by schema.Sanitize(request) on every execution.")
                        ]),
                    new("query-only", "Using Virtual QueryFilters (Custom Server Logic)",
                        Paragraphs: ["Use QueryFilter when a filter controls custom SQL or domain logic (such as soft-deleted records, tenant scope, or full-text PostgreSQL tsvector search) that doesn't map 1:1 to an entity property."],
                        CodeSamples:
                        [
                            new("QueryFilterUsage.cs", """
                                // 1. Declare in schema: .QueryFilter("archived", DataListFilter.Boolean())
                                // 2. Extract sanitized rule on the server before calling schema.Apply():
                                IQueryable<Employee> source = db.Employees.AsNoTracking();

                                var includeArchived = EmployeeSchemas.Directory
                                    .QueryFilterRules(request)
                                    .Any(r => r.Key == "archived" && r.Value is FilterValue.Boolean { Value: true });

                                if (!includeArchived)
                                    source = source.Where(e => !e.IsArchived);

                                var query = EmployeeSchemas.Directory.Apply(source, request);
                                """)
                        ]),
                    new("remote-options", "Remote Searchable Option Filters (DataListRemoteOptions)",
                        Paragraphs: ["When an option filter has thousands of possible values (customers, SKUs, accounts), attach DataListFilter.WithRemoteOptions(...) with an async DataListOptionsProvider and an optional DataListOptionsResolver to label IDs restored from a bookmarked URL."],
                        CodeSamples:
                        [
                            new("RemoteOptions.cs", """
                                DataListFilter.Option().WithRemoteOptions(
                                    provider: async (req, ct) =>
                                    {
                                        // req.Search, req.Skip, req.Take, req.Cursor
                                        var page = await api.SearchCustomersAsync(req.Search, req.Cursor, req.Take, ct);
                                        return new DataListOptionsPage(
                                            Items: [.. page.Items.Select(c => new DataListFilterOption(c.Id.ToString(), c.Name))],
                                            HasMore: page.NextCursor is not null,
                                            NextCursor: page.NextCursor);
                                    },
                                    resolver: async (ids, ct) =>
                                    {
                                        // Called when ?f.customer=42,99 is restored from URL before the dropdown is opened
                                        var found = await api.GetCustomerLabelsAsync(ids, ct);
                                        return [.. found.Select(c => new DataListFilterOption(c.Id.ToString(), c.Name))];
                                    },
                                    pageSize: 25,
                                    minLength: 0)
                                """)
                        ])
                ]),
            new("filters", "Filter AST, Operators & Facets", "Core Engine",
                "Compose arbitrarily nested AND/OR filter trees across 13 operators and 7 strongly typed FilterValue records, plus faceted counts with ExcludeSelf/IncludeSelf modes.",
                [
                    new("operators", "All 13 FilterOperator Values",
                        TableHeaders: ["Operator", "Supported FilterKinds", "Semantics"],
                        TableRows:
                        [
                            new("Contains", "Text", "Substring match (case-insensitive in memory; database collation in SQL)"),
                            new("Is", "Text, Option, Boolean, Number, Date", "Exact equality (or full calendar day/hour/minute/second half-open window for dates)"),
                            new("IsAnyOf", "Option, Text, Number", "Matches when the field equals any item in FilterValue.List (SQL IN (...))"),
                            new("IsNot", "Text, Option, Boolean, Number, Date", "Negated equality (for collections: !collection.Any(...))"),
                            new("Gt / Gte", "Number, Date", "Strictly greater than / greater than or equal"),
                            new("Lt / Lte", "Number, Date", "Strictly less than / less than or equal"),
                            new("Between", "Number, Date", "Inclusive range via FilterValue.Range(From, To); either bound may be null for open-ended ranges"),
                            new("Before / After", "Date", "Temporal comparison aligned to DateGranularity (Day, Hour, Minute, Second)"),
                            new("IsEmpty / IsNotEmpty", "All nullable / text / collection", "Matches null, empty string, or empty collection (Value is null)")
                        ]),
                    new("filter-values", "All 7 FilterValue Subtypes",
                        TableHeaders: ["Subtype", "Constructor", "Notes"],
                        TableRows:
                        [
                            new("FilterValue.Text", "new FilterValue.Text(string)", "Text operand for Contains / Is / IsNot"),
                            new("FilterValue.Number", "new FilterValue.Number(decimal)", "Exact decimal so int, long, double, and decimal compare without float drift"),
                            new("FilterValue.Boolean", "new FilterValue.Boolean(bool)", "True / False operand"),
                            new("FilterValue.Date", "new FilterValue.Date(DateOnly)", "Calendar day cut in Request.TimeZone: compiles to half-open [utcStart, utcNextDayStart)"),
                            new("FilterValue.Moment", "new FilterValue.Moment(DateTimeOffset)", "Exact instant matched at DateGranularity"),
                            new("FilterValue.List", "new FilterValue.List(IReadOnlyList<string>)", "Option values coerced cleanly to the target property type (enum, int, Guid, string, DateOnly)"),
                            new("FilterValue.Range", "new FilterValue.Range(FilterValue? From, FilterValue? To)", "Inclusive range whose bounds are Number, Date, or Moment")
                        ]),
                    new("composition", "Interactive Compound AST + NativeAOT JSON",
                        Paragraphs:
                        [
                            "Combine FilterRule and nested DataListFilterGroup nodes using DataListFilterGroup.And(...) and DataListFilterGroup.Or(...). Set IncludeEmpty: true on any rule to match rows satisfying the rule OR rows where the field is null/empty."
                        ],
                        LiveDemoKey: "query-filters",
                        LiveDemoTitle: "Compound FilterGroup AST + DataListJsonContext JSON",
                        CodeSamples:
                        [
                            new("NestedFilterTree.cs", """
                                var filter = DataListFilterGroup.And(
                                    new FilterRule("active", FilterOperator.Is, new FilterValue.Boolean(true)),
                                    new FilterRule("salary", FilterOperator.Between,
                                        new FilterValue.Range(new FilterValue.Number(80_000m), new FilterValue.Number(150_000m))),
                                    DataListFilterGroup.Or(
                                        new FilterRule("dept", FilterOperator.IsAnyOf, new FilterValue.List(["Engineering", "Product"]), IncludeEmpty: true),
                                        new FilterRule("hired", FilterOperator.After, new FilterValue.Date(new DateOnly(2025, 1, 1)))));

                                var request = new DataListRequest(
                                    paging: DataListPaging.Offset.ForPage(1, 25),
                                    sort: [new DataListSort("salary", Descending: true)],
                                    filter: filter,
                                    search: new DataListSearch("alice"),
                                    timeZone: "Europe/Paris");
                                """)
                        ]),
                    new("facets-deep", "Faceted Counts (ExcludeSelf vs IncludeSelf)",
                        Paragraphs:
                        [
                            "Request facet counts on any Option or Boolean field by passing DataListFacetRequest entries in DataListRequest.Facets. In the default FacetMode.ExcludeSelf mode, each facet computes counts with every OTHER active filter applied while ignoring its own filter — letting users see how many matches each sibling option would add in a multi-select sidebar.",
                            "Switch to FacetMode.IncludeSelf when you want counts strictly intersected with the field's own active selection. Pass IncludeMissing: true to also count rows where the field is null or empty."
                        ],
                        LiveDemoKey: "query-facets-cursor",
                        LiveDemoTitle: "Interactive FacetMode (ExcludeSelf vs IncludeSelf) & Keyset Cursor Paging",
                        CodeSamples:
                        [
                            new("FacetsRequest.cs", """
                                var request = new DataListRequest(
                                    paging: new DataListPaging.Cursor(After: cursor, Take: 20, Count: true),
                                    sort: [new DataListSort("price", Descending: true)],
                                    filter: DataListFilterGroup.And(
                                        new FilterRule("category", FilterOperator.Is, new FilterValue.Text("Audio"))),
                                    search: null,
                                    facets:
                                    [
                                        new DataListFacetRequest("category", Mode: FacetMode.ExcludeSelf, Limit: 20, IncludeMissing: true)
                                    ]);

                                var result = schema.ExecuteInMemory(items, request);
                                // result.Facets[0].Options -> [("Displays", 2), ("Input", 2), ("Audio", 2)]
                                // result.NextCursor        -> opaque keyset token for the next page
                                """)
                        ])
                ]),
            new("paging-and-sorting", "Sorting, Paging, Cursors & TimeZones", "Core Engine",
                "Choose between numbered Offset paging, keyset Cursor paging, or unpaged queries — with nulls-last sorting, RowKey tie-breakers, and IANA TimeZone day boundaries.",
                [
                    new("paging", "All 3 DataListPaging Strategies",
                        TableHeaders: ["Strategy", "Constructor / Factory", "Best For"],
                        TableRows:
                        [
                            new("DataListPaging.Offset", "DataListPaging.Offset.ForPage(page, pageSize) or new(Skip, Take)", "Admin grids with numbered page bars, known TotalCount, and random page jumps."),
                            new("DataListPaging.Cursor", "new DataListPaging.Cursor(string? After, int Take, bool Count = false)", "Infinite scroll, feeds, and high-volume SQL tables where OFFSET scans degrade linearly."),
                            new("DataListPaging.None", "DataListPaging.None.Instance", "Small datasets, CSV exports, or client-side tree/group views needing all matching rows.")
                        ],
                        LiveDemoKey: "query-paging",
                        LiveDemoTitle: "Interactive Offset Pagination & PageSize Switching"),
                    new("cursors-how", "How Keyset Cursors Stay Deterministic Under Concurrent Writes",
                        Paragraphs:
                        [
                            "Offset pagination shifts rows whenever someone inserts or deletes a record above the current page. DataListPaging.Cursor encodes the last row's active sort field values AND its RowKey tie-breaker into a compact URL-safe token.",
                            "When executing the next page, Klyroo.Query compiles a keyset WHERE predicate over the sort columns and RowKey (fetching Take + 1 rows to know whether a next page exists), guaranteeing O(log N) index seeks with zero skipped or duplicated rows."
                        ],
                        CodeSamples:
                        [
                            new("CursorLoop.cs", """
                                string? cursor = null;
                                do
                                {
                                    var req = new DataListRequest(
                                        paging: new DataListPaging.Cursor(After: cursor, Take: 50, Count: false),
                                        sort: [new DataListSort("hired", Descending: true)],
                                        filter: DataListFilterGroup.Empty);

                                    var page = schema.ExecuteInMemory(employees, req);
                                    ProcessBatch(page.Items);
                                    cursor = page.NextCursor;
                                } while (cursor is not null);
                                """)
                        ],
                        Callout: "Always declare .RowKey(e => e.Id) on your DataListSchema<T> when using cursor paging, and prefer numeric/string/date keys over Guid for SQL keyset ordering because databases order GUIDs differently."),
                    new("sorting-tz", "Nulls-Last Multi-Column Sort & IANA TimeZone Cuts",
                        Paragraphs:
                        [
                            "Both InMemoryDataList and QueryableDataList order null values LAST in both ascending and descending directions, then append RowKey as a final tie-breaker. In memory, strings use natural numeric comparison ('item2' before 'item10').",
                            "Pass an IANA TimeZone identifier (e.g. \"Europe/Paris\" or \"America/New_York\") on DataListRequest so FilterValue.Date(2026-05-10) filters DateTimeOffset columns by the user's exact local midnight [2026-05-09T22:00Z, 2026-05-10T22:00Z) rather than UTC midnight."
                        ])
                ]),
            new("codecs", "URL Codecs & DataListStateQuery", "Core Engine",
                "Serialize both individual typed values (QueryCodecs) and complete DataListState snapshots (DataListStateQuery) into clean, readable, tamper-proof URL query strings.",
                [
                    new("state-query-syntax", "Exact URL Grammar of DataListStateQuery",
                        Paragraphs:
                        [
                            "DataListStateQuery converts a DataListState to and from raw query key/value pairs with zero Blazor or NavigationManager dependency — so both clients and ASP.NET Core servers can parse the exact same URLs.",
                            "Any value equal to schema.DefaultState is omitted automatically so untouched pages have clean URLs. Bad or tampered URL inputs never throw: invalid keys/operators are dropped and out-of-range page/pageSize values are clamped safely."
                        ],
                        TableHeaders: ["Query Key", "Example", "Meaning"],
                        TableRows:
                        [
                            new("q", "?q=alice", "Full-text search term"),
                            new("p / ps", "?p=2&ps=25", "1-based page number (omitted when cursor is active) and page size"),
                            new("s", "?s=dept|-salary", "Multi-column sort separated by '|'; leading '-' means descending; empty ?s= means unsorted"),
                            new("f.<key>", "?f.dept=Eng,Product", "Filter rule using the field's DefaultOperator; '~' token sets IncludeEmpty = true"),
                            new("f.<key>.<op>", "?f.salary.between=80000..150000", "Explicit operator (contains, is, isAnyOf, isNot, gt, gte, lt, lte, between, before, after, isEmpty, isNotEmpty)"),
                            new("c", "?c=eyJ...", "Opaque keyset cursor token when cursor paging is enabled"),
                            new("l", "?l=grid", "Active layout ('table' or 'grid')"),
                            new("<prefix>.<key>", "?orders.p=3&orders.s=-total", "Optional dot-separated prefix to host multiple independent lists on one URL")
                        ],
                        LiveDemoKey: "query-url-codecs",
                        LiveDemoTitle: "Interactive DataListStateQuery.ToQuery / FromQuery Round-Trip",
                        CodeSamples:
                        [
                            new("StateQueryRoundTrip.cs", """
                                // 1. Serialize DataListState -> URL pairs (omitting schema defaults)
                                IReadOnlyDictionary<string, string> pairs =
                                    DataListStateQuery.ToQuery(state, EmployeeSchemas.Directory, prefix: "emp");

                                // 2. Reconstruct DataListState from incoming query pairs (never throws on bad input)
                                DataListState restored = DataListStateQuery.FromQuery(
                                    pairs.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value)),
                                    EmployeeSchemas.Directory,
                                    prefix: "emp",
                                    timeZone: "Europe/Paris");

                                // 3. Convert directly into an executable DataListRequest
                                DataListRequest request = restored.ToRequest(cursorPaging: false, timeZone: "Europe/Paris");
                                """)
                        ]),
                    new("builtin-codecs", "All Built-In QueryCodecs",
                        TableHeaders: ["Codec", "C# Type", "Wire Format"],
                        TableRows:
                        [
                            new("QueryCodecs.String", "string", "Raw UTF-8 string"),
                            new("QueryCodecs.Int32 / Int64", "int / long", "Invariant culture integer"),
                            new("QueryCodecs.Decimal / Double", "decimal / double", "Invariant culture number (rejects NaN/Infinity)"),
                            new("QueryCodecs.Boolean", "bool", "\"true\" / \"false\" (also parses \"1\" / \"0\")"),
                            new("QueryCodecs.DateOnly", "DateOnly", "ISO-8601 \"yyyy-MM-dd\""),
                            new("QueryCodecs.DateTimeOffset", "DateTimeOffset", "ISO-8601 round-trip \"O\""),
                            new("QueryCodecs.Enum<TEnum>(lowercase)", "TEnum", "Case-insensitive enum name (optionally lowercase)"),
                            new("QueryCodecs.Array(itemCodec, ',')", "T[]", "Escaped delimiter-separated list"),
                            new("QueryCodecs.Range(itemCodec)", "QueryRange<T>", "\"from..to\" with nullable bounds"),
                            new("QueryCodecs.Optional(itemCodec)", "T?", "Wraps any value-type codec as nullable")
                        ]),
                    new("custom-codec", "Writing a Custom IQueryCodec<T>",
                        CodeSamples:
                        [
                            new("SlugCodec.cs", """
                                using System.Diagnostics.CodeAnalysis;
                                using Klyroo.Query;

                                public sealed class LowercaseSlugCodec : IQueryCodec<string>
                                {
                                    public bool TryParse(string raw, [MaybeNullWhen(false)] out string value)
                                    {
                                        var trimmed = raw.Trim().ToLowerInvariant();
                                        if (trimmed.Length == 0) { value = null; return false; }
                                        value = trimmed;
                                        return true;
                                    }

                                    public string? Format(string value) =>
                                        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
                                }
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 3: EXECUTORS & STATE
            // =========================================================================
            new("executors", "InMemory & IQueryable", "Executors & State",
                "Run the exact same DataListRequest against local collections (InMemoryDataList) or database IQueryables (QueryableDataList).",
                [
                    new("inmemory", "InMemoryDataList.ExecuteInMemory & InMemoryProvider",
                        Paragraphs:
                        [
                            "schema.ExecuteInMemory(items, request) evaluates search, nested filter trees, natural text sorting, nulls-last ordering, facets, and offset/cursor paging in one deterministic pass.",
                            "For components expecting an async DataListProvider<T> delegate (like Klyroo.Table's KlyDataList), call schema.InMemoryProvider(items) to get a ready-to-use provider."
                        ],
                        CodeSamples:
                        [
                            new("InMemoryExecution.cs", """
                                // Direct synchronous execution (unit tests, WASM caches, MAUI ViewModels)
                                DataListResult<Employee> result = EmployeeSchemas.Directory.ExecuteInMemory(employees, request);

                                // Or expose as an async DataListProvider<Employee> delegate
                                DataListProvider<Employee> provider = EmployeeSchemas.Directory.InMemoryProvider(employees);
                                DataListResult<Employee> asyncResult = await provider(request, cancellationToken);
                                """)
                        ]),
                    new("iqueryable", "QueryableDataList.Apply, FacetQueries & ToResult",
                        Paragraphs:
                        [
                            "schema.Apply(IQueryable<T> source, DataListRequest request) builds pure System.Linq.Expressions trees with parameterized values (never string concatenation, never client evaluation) and returns a DataListQuery<T> containing three composable IQueryables: Matching (unpaged count), Page (offset page), and Rows (cursor page with sort values & key projected alongside the entity).",
                            "QueryableDataList has zero dependency on EF Core — it works with EF Core, Marten, RavenDB, or any LINQ provider."
                        ],
                        TableHeaders: ["DataListQuery<T> Member", "Type", "When to Execute"],
                        TableRows:
                        [
                            new("query.Matching", "IQueryable<T>", "Run CountAsync() when query.WantsTotal is true."),
                            new("query.Page", "IQueryable<T>", "Run ToListAsync() for Offset or None paging, then call schema.ToResult(request, items, total, facets)."),
                            new("query.Rows", "IQueryable<DataListRow<T>>", "Run ToListAsync() for Cursor paging (projects sort values + key without needing Include), then call schema.ToResult(request, rows, total, facets)."),
                            new("schema.FacetQueries(source, req)", "IReadOnlyList<DataListFacetQuery<T>>", "For each facet, run ToListAsync() on facet.Buckets and optional CountAsync() on facet.Missing, then call facet.ToResult(buckets, missing).")
                        ],
                        Callout: "QueryableDataList.Apply intentionally throws InvalidOperationException if you pass an in-memory .AsQueryable() sequence. Always use schema.ExecuteInMemory(...) for in-memory data so case-insensitive search and null-safe navigation guards run properly.")
                ]),
            new("ef-core", "Entity Framework Core", "Executors & State",
                "Translate DataListRequest into parameterized, index-friendly async SQL queries on PostgreSQL, SQL Server, and SQLite.",
                [
                    new("ef-offset", "Offset Paging + Facets with EfAsyncQueryExecutor",
                        Paragraphs:
                        [
                            "Klyroo.Query.EntityFrameworkCore provides EfAsyncQueryExecutor.CountAsync and EfAsyncQueryExecutor.ToListAsync. Calendar date filters compile to half-open range predicates (e.HiredAt >= @start && e.HiredAt < @end) rather than SQL DATEPART functions so database B-Tree indexes remain fully usable."
                        ],
                        CodeSamples:
                        [
                            new("EmployeeEndpoints.cs", """
                                using Klyroo.Query;
                                using Klyroo.Query.EntityFrameworkCore;
                                using Microsoft.EntityFrameworkCore;

                                app.MapPost("/api/employees/query", async (
                                    DataListRequest request,
                                    AppDbContext db,
                                    CancellationToken ct) =>
                                {
                                    IQueryable<Employee> source = db.Employees
                                        .AsNoTracking()
                                        .Include(e => e.Department);

                                    // 1. Build composable SQL queries (zero execution yet)
                                    DataListQuery<Employee> query = EmployeeSchemas.Directory.Apply(source, request);

                                    // 2. Run total count only if needed
                                    int? total = query.WantsTotal
                                        ? await EfAsyncQueryExecutor.CountAsync(query.Matching, ct)
                                        : null;

                                    // 3. Run facet GROUP BY queries if requested
                                    List<DataListFacetResult>? facets = null;
                                    var facetQueries = EmployeeSchemas.Directory.FacetQueries(source, request);
                                    if (facetQueries.Count > 0)
                                    {
                                        facets = [];
                                        foreach (var fq in facetQueries)
                                        {
                                            var buckets = await EfAsyncQueryExecutor.ToListAsync(fq.Buckets, ct);
                                            int? missing = fq.Missing is not null
                                                ? await EfAsyncQueryExecutor.CountAsync(fq.Missing, ct)
                                                : null;
                                            facets.Add(fq.ToResult(buckets, missing));
                                        }
                                    }

                                    // 4. Execute either Cursor paging (query.Rows) or Offset paging (query.Page)
                                    DataListResult<Employee> result = request.Paging is DataListPaging.Cursor
                                        ? EmployeeSchemas.Directory.ToResult(
                                            request,
                                            await EfAsyncQueryExecutor.ToListAsync(query.Rows, ct),
                                            total,
                                            facets)
                                        : EmployeeSchemas.Directory.ToResult(
                                            request,
                                            await EfAsyncQueryExecutor.ToListAsync(query.Page, ct),
                                            total,
                                            facets);

                                    return Results.Ok(result);
                                });
                                """)
                        ]),
                    new("ef-providers", "Database Provider Notes (PostgreSQL, SQL Server, SQLite)",
                        TableHeaders: ["Database", "Collation & Contains", "Configuration Tip"],
                        TableRows:
                        [
                            new("PostgreSQL (Npgsql)", "Case-sensitive by default (LIKE); use citext or case-insensitive collation if needed", "Native DateTimeOffset (timestamptz) and decimal support; B-Tree indexes work directly with keyset cursors."),
                            new("SQL Server", "Case-insensitive by default (CI collation)", "Full native support for decimal, DateTimeOffset, and keyset WHERE comparisons."),
                            new("SQLite", "LIKE / Contains is ASCII case-sensitive", "Register ValueConverters for DateTimeOffset (UTC ticks) and decimal (double) in OnModelCreating so ORDER BY and range comparisons translate to SQL.")
                        ],
                        Callout: "Always call .AsNoTracking() on read-only query endpoints to eliminate EF Core change-tracker allocations.")
                ]),
            new("query-state", "Reactive QueryState, QueryValue & QueryGroup", "Executors & State",
                "Make the browser URL your single source of truth across Static SSR, Interactive Server, WebAssembly, and .NET MAUI Blazor Hybrid.",
                [
                    new("how-qs-works", "How QueryState Works Under the Hood",
                        Paragraphs:
                        [
                            "QueryState is a scoped service backed by NavigationManager. Synchronous writes within a single UI event handler are automatically coalesced into one single navigation (if any write in the batch requests QueryHistory.Push, the entire batch pushes a history entry; otherwise it replaces in place).",
                            "During Static SSR and prerendering, writes safely return false instead of throwing NavigationException — and HrefWith(...) generates plain <a href=\"...\"> links that work even with JavaScript disabled. If a NavigationLock cancels a navigation, QueryState automatically rolls back to the real URL."
                        ]),
                    new("qs-api", "Complete QueryState, QueryValue<T> & QueryGroup<T> API",
                        TableHeaders: ["API", "Signature", "Purpose"],
                        TableRows:
                        [
                            new("Query.Bind<T>", "Bind<T>(key, codec, defaultValue, RendererInfo, history)", "Creates a disposable QueryValue<T> with typed .Value, .Set(), .HrefWith(), and .Changed event. Default values are omitted from the URL."),
                            new("Query.Group<TRecord>", "Group<TRecord>(defaultRecord, owner, RendererInfo, prefix?, history)", "Binds an immutable C# record to multiple query keys as one atomic unit via .Add(...), .Update(r => r with { ... }), and .HrefWith(record)."),
                            new("Query.Claim", "Claim(prefix, owner)", "Reserves a prefix so two components on the same page detect accidental URL key collisions via .ThrowIfConflicted()."),
                            new("Query.Subscribe", "Subscribe(prefix, Action<IReadOnlyList<string>>)", "Fires only when keys matching prefix change (from code OR browser Back/Forward)."),
                            new("Query.Set / SetMany", "Set(key, value?, history) / SetMany(pairs, history)", "Low-level batched URL writes (call Query.UseRenderer(RendererInfo) first). Passing null removes the key."),
                            new("Query.HrefWith", "HrefWith(params (string Key, string? Value)[])", "Computes the next URL without navigating — ideal for SSR pagination and filter links.")
                        ]),
                    new("query-group-example", "Recommended Pattern: QueryGroup<TRecord> + QueryValue<T>",
                        CodeSamples:
                        [
                            new("CatalogFilters.razor", """
                                @inject QueryState Query
                                @implements IDisposable

                                <div class="flex flex-wrap items-center gap-3">
                                    <KlyInput Value="@_filters.Value.Search"
                                             ValueChanged="@(v => _filters.Update(f => f with { Search = v, Page = 1 }, QueryHistory.Replace))"
                                             Placeholder="Search..." />

                                    <a href="@_filters.HrefWith(_filters.Value with { Page = _filters.Value.Page + 1 })">
                                        Next Page (works in Static SSR & Interactive!)
                                    </a>
                                </div>

                                @code {
                                    private sealed record CatalogFilter(string Search, string Category, int Page, bool InStock);
                                    private QueryGroup<CatalogFilter> _filters = default!;

                                    protected override void OnInitialized()
                                    {
                                        _filters = Query.Group(
                                                new CatalogFilter("", "all", 1, false),
                                                owner: "CatalogPage",
                                                RendererInfo,
                                                prefix: "cat",
                                                history: QueryHistory.Push)
                                            .Add("q", f => f.Search, (f, v) => f with { Search = v }, QueryCodecs.String)
                                            .Add("category", f => f.Category, (f, v) => f with { Category = v }, QueryCodecs.String)
                                            .Add("page", f => f.Page, (f, v) => f with { Page = v }, QueryCodecs.Int32)
                                            .Add("stock", f => f.InStock, (f, v) => f with { InStock = v }, QueryCodecs.Boolean);

                                        _filters.Changed += OnUrlChanged;
                                    }

                                    private void OnUrlChanged() => _ = InvokeAsync(StateHasChanged);

                                    public void Dispose()
                                    {
                                        _filters.Changed -= OnUrlChanged;
                                        _filters.Dispose();
                                    }
                                }
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: INTEGRATIONS WITH OTHER KLYROO LIBRARIES
            // =========================================================================
            new("integrations-table", "Integration: Klyroo.Table", "Integrations",
                "Klyroo.Table (KlyDataList<TItem>) is built directly on top of Klyroo.Query — sharing DataListSchema<T>, DataListProvider<T>, QueryState URL synchronization, facets, and cursor infinite scroll.",
                [
                    new("table-shared-schema", "1. How KlyDataList<T> Uses DataListSchema & DataListProvider",
                        Paragraphs:
                        [
                            "Pass any DataListSchema<T> from Klyroo.Query.Core directly to <KlyDataList>. You can feed it either an in-memory collection via Items=\"@_rows\" (internally backed by schema.InMemoryProvider) OR an async server/HTTP DataListProvider<T> delegate via Provider=\"@LoadPageAsync\".",
                            "When UrlSync=\"true\" (the default), KlyDataList automatically claims a URL prefix in QueryState, reads/writes DataListStateQuery parameters (?q=&p=&s=&f.*=), handles Browser Back/Forward, and renders SSR-safe links."
                        ],
                        CodeSamples:
                        [
                            new("EmployeesPage.razor", """
                                @using Klyroo.Query
                                @using Klyroo.Table
                                @inject HttpClient Http

                                <KlyDataList TItem="Employee"
                                            Schema="@EmployeeSchemas.Directory"
                                            Provider="@LoadEmployeesAsync"
                                            UrlSync="true"
                                            UrlPrefix="emp">
                                    <KlyDataListToolbar TItem="Employee" />
                                    <KlyDataListFilterPanel TItem="Employee" />

                                    <KlyDataListTable TItem="Employee" HeaderMenu="true" ShowProgressLine="true">
                                        <KlyDataListFieldColumn TItem="Employee" Field="name" Title="Name" />
                                        <KlyDataListFieldColumn TItem="Employee" Field="dept" Title="Department" />
                                        <KlyDataListFieldColumn TItem="Employee" Field="salary" Title="Salary" Summary="DataListSummaryKind.Sum" />
                                    </KlyDataListTable>

                                    <KlyDataListPagination TItem="Employee" />
                                </KlyDataList>

                                @code {
                                    private async ValueTask<DataListResult<Employee>> LoadEmployeesAsync(
                                        DataListRequest request, CancellationToken ct)
                                    {
                                        using var resp = await Http.PostAsJsonAsync(
                                            "/api/employees/query", request, DataListJsonContext.Default.DataListRequest, ct);
                                        resp.EnsureSuccessStatusCode();
                                        return (await resp.Content.ReadFromJsonAsync<DataListResult<Employee>>(ct))!;
                                    }
                                }
                                """)
                        ]),
                    new("table-multi-prefix", "2. Hosting Two Independent Tables on One Page (UrlPrefix)",
                        Paragraphs: ["Because KlyDataList registers a QueryClaim via QueryState, placing two lists on the same page without distinct UrlPrefix values throws an immediate conflict diagnostic instead of silently corrupting each other's URL keys."],
                        CodeSamples:
                        [
                            new("DualTables.razor", """
                                @* URL stays cleanly isolated: ?active.p=2&active.s=-salary&pending.q=alice *@
                                <KlyDataList TItem="Employee" Schema="@EmployeeSchemas.Directory" Items="@_active" UrlPrefix="active">
                                    <KlyDataListTable TItem="Employee" />
                                </KlyDataList>

                                <KlyDataList TItem="Employee" Schema="@EmployeeSchemas.Directory" Items="@_pending" UrlPrefix="pending">
                                    <KlyDataListTable TItem="Employee" />
                                </KlyDataList>
                                """)
                        ]),
                    new("table-cursor-facets", "3. Keyset Cursor Infinite Scroll & Facet Sidebars",
                        Paragraphs: ["Set CursorPaging=\"true\" on KlyDataList and swap <KlyDataListPagination> for <KlyDataListLoadMore> — Klyroo.Query automatically passes DataListPaging.Cursor(After, Take) and populates NextCursor. Any field configured with .WithFacets() in DataListSchema automatically renders live bucket counts inside <KlyDataListFilterPanel>."],
                        CodeSamples:
                        [
                            new("InfiniteFeed.razor", """
                                <KlyDataList TItem="Employee"
                                            Schema="@EmployeeSchemas.Directory"
                                            Provider="@LoadEmployeesAsync"
                                            CursorPaging="true">
                                    <KlyDataListFilterPanel TItem="Employee" />
                                    <KlyDataListGrid TItem="Employee" />
                                    <KlyDataListLoadMore TItem="Employee" />
                                </KlyDataList>
                                """)
                        ])
                ]),
            new("integrations-dashboard-form", "Integration: Klyroo.Dashboard & Klyroo.Form", "Integrations",
                "Synchronize Klyroo.Dashboard global filters with the URL via QueryGroup<T>, and drive Klyroo.Query search requests directly from validated Klyroo.Form schemas.",
                [
                    new("dashboard-integration", "1. Klyroo.Query + Klyroo.Dashboard (URL-Synced Analytics & 3-Wave Refresh)",
                        Paragraphs:
                        [
                            "Combine QueryGroup<DashboardFilters> from Klyroo.Query with DashboardController from Klyroo.Dashboard so timeframe, region, and currency selectors live in the URL. When a user bookmarks an executive dashboard, shares a Slack link, or presses Browser Back, QueryGroup.Changed fires and refreshes all Essential/Background waves automatically.",
                            "Inside each DashboardResource<T> loader, reuse DataListSchema<T>.ExecuteInMemory or QueryableDataList.Apply so KPI cards, charts, and drill-down tables always compute against the exact same filter AST."
                        ],
                        CodeSamples:
                        [
                            new("AnalyticsPage.razor", """
                                @using Klyroo.Query
                                @using Klyroo.Dashboard
                                @inject QueryState Query
                                @implements IDisposable

                                <KlyDashboardPage Controller="@_dashboard" Title="Revenue Analytics">
                                    <KlyDashboardStats Columns="3" Items="@_kpis.Data" />
                                    <KlyDashboardTable Title="Top Accounts" Columns="@_cols" Rows="@_topRows.Data" />
                                </KlyDashboardPage>

                                @code {
                                    private sealed record DashFilter(string Period, string Region, bool ActiveOnly);
                                    private sealed record TopAccount(string Name, decimal Revenue);

                                    private QueryGroup<DashFilter> _url = default!;
                                    private readonly DashboardController _dashboard = new("revenue");
                                    private static readonly IReadOnlyList<DashboardTableColumn<TopAccount>> _cols =
                                    [
                                        DashboardTableColumn.For<TopAccount, string>("name", "Account", a => a.Name),
                                        DashboardTableColumn.For<TopAccount, decimal>("revenue", "Revenue", a => a.Revenue, DashboardFormatPreset.Compact),
                                    ];
                                    private DashboardResource<IReadOnlyList<DashboardStatItem>> _kpis = default!;
                                    private DashboardResource<IReadOnlyList<TopAccount>> _topRows = default!;

                                    protected override void OnInitialized()
                                    {
                                        _url = Query.Group(new DashFilter("30d", "all", true), "RevenueDash", RendererInfo, prefix: "dash")
                                            .Add("period", f => f.Period, (f, v) => f with { Period = v }, QueryCodecs.String)
                                            .Add("region", f => f.Region, (f, v) => f with { Region = v }, QueryCodecs.String)
                                            .Add("active", f => f.ActiveOnly, (f, v) => f with { ActiveOnly = v }, QueryCodecs.Boolean);

                                        _kpis = _dashboard.RegisterResource(new DashboardResource<IReadOnlyList<DashboardStatItem>>(
                                            "kpis", ct => LoadKpisAsync(_url.Value, ct), DashboardQueryStage.Essential));

                                        _topRows = _dashboard.RegisterResource(new DashboardResource<IReadOnlyList<TopAccount>>(
                                            "top", ct => LoadTopAccountsAsync(_url.Value, ct), DashboardQueryStage.Background));

                                        _url.Changed += OnUrlFilterChanged;
                                    }

                                    private void OnUrlFilterChanged() => _ = InvokeAsync(async () =>
                                    {
                                        await _dashboard.RefreshAsync();
                                        StateHasChanged();
                                    });

                                    public void Dispose()
                                    {
                                        _url.Changed -= OnUrlFilterChanged;
                                        _url.Dispose();
                                    }
                                }
                                """)
                        ]),
                    new("form-integration", "2. Klyroo.Query + Klyroo.Form (Advanced Search Drawer → Filter AST)",
                        Paragraphs:
                        [
                            "For complex multi-criteria search dialogs (min/max budgets, date ranges, multi-select tags, conditional fields), render a Klyroo.Form schema inside a Drawer via FormService.CreateFormAsync or <KlyFormEngine>, then translate the validated dictionary into FilterRule[] and write it cleanly to QueryState via DataListStateQuery.ToQuery."
                        ],
                        CodeSamples:
                        [
                            new("AdvancedSearchHelper.cs", """
                                using Klyroo.Form;
                                using Klyroo.Query;

                                // Open a typed Klyroo.Form drawer and turn the submitted model into a Klyroo.Query DataListState
                                public sealed class EmployeeSearch
                                {
                                    public string? Q { get; set; }
                                    public decimal? MinSalary { get; set; }
                                    public List<string> Departments { get; set; } = [];
                                }

                                public async Task OpenAdvancedSearchAsync(
                                    FormService formService,
                                    QueryState queryState,
                                    FormSchema<EmployeeSearch> searchFormSchema)
                                {
                                    var outcome = await formService.CreateFormAsync(
                                        searchFormSchema, mode: FormShellMode.Drawer);

                                    if (outcome is not { IsCompleted: true, Model: { } search }) return;

                                    var rules = new List<FilterRule>();
                                    if (search.MinSalary is { } minSalary)
                                        rules.Add(new FilterRule("salary", FilterOperator.Gte, new FilterValue.Number(minSalary)));

                                    if (search.Departments.Count > 0)
                                        rules.Add(new FilterRule("dept", FilterOperator.IsAnyOf, new FilterValue.List(search.Departments)));

                                    var state = new DataListState(
                                        search: search.Q ?? "",
                                        filters: rules,
                                        sort: [new DataListSort("salary", Descending: true)],
                                        page: 1,
                                        pageSize: 25,
                                        layout: DataListLayout.Table);

                                    var pairs = DataListStateQuery.ToQuery(state, EmployeeSchemas.Directory, prefix: "emp")
                                        .Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value));

                                    queryState.SetMany(pairs, QueryHistory.Push);
                                }
                                """)
                        ])
                ]),
            new("integrations-ui-ecosystem", "Integration: Klyroo.UI, Display, Spreadsheet & Agent", "Integrations",
                "Wire Klyroo.Query into Klyroo.UI primitives (Tabs, Pagination, Inputs, Static SSR links), Klyroo.Display KPI summaries, Klyroo.Spreadsheet pre-commit filtering, and Klyroo.Agent AI workflows.",
                [
                    new("ui-integration", "1. Klyroo.Query + Klyroo.UI (Tabs, Inputs, Pagination & Zero-JS SSR Links)",
                        Paragraphs:
                        [
                            "Any Klyroo.UI component — KlyTabs, KlyInput, KlySelectMenu, KlySwitch, KlyPagination — binds cleanly to QueryValue<T> or QueryGroup<TRecord>. Because QueryValue<T>.HrefWith(...) builds valid query URLs without JS, your navigation works identically in Static SSR, Server, WASM, and MAUI."
                        ],
                        CodeSamples:
                        [
                            new("UiUrlBinding.razor", """
                                @using Klyroo.UI
                                @using Klyroo.Query
                                @inject QueryState Query
                                @implements IDisposable

                                <KlyTabs Value="@_tab.Value"
                                        ValueChanged="@(v => _tab.Set(v, QueryHistory.Push))"
                                        Items="@([
                                            new TabItem("overview", "Overview"),
                                            new TabItem("members", "Members"),
                                            new TabItem("billing", "Billing")
                                        ])" />

                                <div class="mt-4 flex gap-2">
                                    @* Works with zero JavaScript in Static SSR too *@
                                    <KlyButton Label="Go to Billing" Href="@_tab.HrefWith("billing")" Variant="ButtonVariant.Outline" />
                                </div>

                                @code {
                                    private QueryValue<string> _tab = default!;

                                    protected override void OnInitialized()
                                    {
                                        _tab = Query.Bind("tab", QueryCodecs.String, "overview", RendererInfo, QueryHistory.Push);
                                        _tab.Changed += HandleChanged;
                                    }

                                    private void HandleChanged() => _ = InvokeAsync(StateHasChanged);
                                    public void Dispose() { _tab.Changed -= HandleChanged; _tab.Dispose(); }
                                }
                                """)
                        ]),
                    new("display-integration", "2. Klyroo.Query + Klyroo.UI blocks (Live Result & Facet Summary Cards)",
                        Paragraphs: ["Feed DataListResult<T>.TotalCount and DataListResult<T>.Facets directly into the KlyStat and KlyFacts blocks of Klyroo.UI so summary ribbons stay synchronized with the active query."],
                        CodeSamples:
                        [
                            new("DisplayFromQuery.razor", """
                                @using Klyroo.Query
                                @using Klyroo.UI

                                <div class="grid gap-4 sm:grid-cols-2">
                                    <KlyStat Title="Matching Employees" Value="@_result.TotalCount" Format="N0" />

                                    <KlyFacts Groups="@([new KlyFactGroup("Breakdown by Department",
                                        [.. (_result.Facets?.FirstOrDefault()?.Options ?? [])
                                            .Select(o => new KlyFact(o.Value) { Value = $"{o.Count} people" })])])" />
                                </div>
                                """)
                        ]),
                    new("spreadsheet-integration", "3. Klyroo.Query + Klyroo.Spreadsheet (Filter & Search Imported Rows)",
                        Paragraphs: ["After SpreadsheetImporter parses and validates thousands of CSV/XLSX rows, wrap its parsed row collection with a DataListSchema<SpreadsheetRow> to let users search, sort, and isolate only rows with validation errors or specific categories before committing."],
                        CodeSamples:
                        [
                            new("SpreadsheetQuery.cs", """
                                using Klyroo.Query;
                                using Klyroo.Spreadsheet;

                                var rowSchema = DataListSchema.For<SpreadsheetRow>(s => s
                                    .RowKey(r => r.RowIndex)
                                    .Field(r => r.HasErrors, f => f.Key("errors").Filter(DataListFilter.Boolean()))
                                    .Field(r => r.Status, f => f.Key("status").Sortable().Filter(DataListFilter.Option())));

                                // Filter only invalid rows from a 10,000-row CSV import in microseconds
                                var invalidPage = rowSchema.ExecuteInMemory(
                                    importer.Rows,
                                    new DataListRequest(
                                        DataListPaging.Offset.ForPage(1, 50),
                                        [],
                                        DataListFilterGroup.And(new FilterRule("errors", FilterOperator.Is, new FilterValue.Boolean(true)))));
                                """)
                        ]),
                    new("agent-integration", "4. Klyroo.Query + Klyroo.Agent (Safe AI-Driven Filtering & Sorting)",
                        Paragraphs: ["Because DataListSchema.Sanitize(request) rejects unknown keys, invalid operators, and unbounded page sizes, you can safely let a Klyroo.Agent tool call construct a DataListState or write keys into QueryState — the user immediately sees the URL update and the filtered results appear."],
                        CodeSamples:
                        [
                            new("AgentQueryTool.cs", """
                                using Klyroo.Agent;
                                using Klyroo.Query;

                                // Expose a safe semantic action that updates the user's live URL-backed query
                                void ApplyAgentFilter(QueryState queryState, string? search, string? department, bool descSalary)
                                {
                                    IReadOnlyList<FilterRule> rules = string.IsNullOrWhiteSpace(department)
                                        ? []
                                        : [new FilterRule("dept", FilterOperator.Is, new FilterValue.Text(department))];

                                    var state = new DataListState(
                                        search ?? "", rules, [new DataListSort("salary", descSalary)], 1, 25, DataListLayout.Table);

                                    var pairs = DataListStateQuery.ToQuery(state, EmployeeSchemas.Directory)
                                        .Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value));

                                    queryState.SetMany(pairs, QueryHistory.Push);
                                }
                                """)
                        ])
                ]),
            new("integrations-fullstack", "Full-Stack Blueprint (Shared + API + WASM/MAUI)", "Integrations",
                "Complete 3-project production architecture sharing one DataListSchema<T> across an EF Core Minimal API server and a Blazor WebAssembly or .NET MAUI client.",
                [
                    new("architecture", "Project Structure",
                        TableHeaders: ["Project", "References", "Contains"],
                        TableRows:
                        [
                            new("MyApp.Shared", "Klyroo.Query.Core", "DTO records (EmployeeDto) and static DataListSchema<EmployeeDto> definitions."),
                            new("MyApp.Server", "MyApp.Shared, Klyroo.Query.EntityFrameworkCore", "EF Core DbContext + Minimal API POST /api/employees/query using schema.Apply + EfAsyncQueryExecutor + DataListJsonContext."),
                            new("MyApp.Client / MyApp.Maui", "MyApp.Shared, Klyroo.Query, Klyroo.Table", "Typed HttpClient DataListProvider<EmployeeDto> wired into <KlyDataList> or custom UI with QueryState URL sync.")
                        ]),
                    new("fullstack-code", "End-to-End Shared Schema, Server Endpoint & Client Provider",
                        CodeSamples:
                        [
                            new("1. MyApp.Shared/EmployeeSchemas.cs", """
                                using Klyroo.Query;

                                public sealed record EmployeeDto(int Id, string Name, string Department, decimal Salary, bool Active);

                                public static class EmployeeSchemas
                                {
                                    public static readonly DataListSchema<EmployeeDto> Directory = DataListSchema.For<EmployeeDto>(s => s
                                        .RowKey(e => e.Id)
                                        .Search(e => e.Name, e => e.Department)
                                        .Field(e => e.Name, f => f.Key("name").Sortable().Filter(DataListFilter.Text()))
                                        .Field(e => e.Department, f => f.Key("dept").Sortable().Filter(DataListFilter.Option().WithFacets()))
                                        .Field(e => e.Salary, f => f.Key("salary").Sortable().Filter(DataListFilter.Number(min: 0)))
                                        .Field(e => e.Active, f => f.Key("active").Filter(DataListFilter.Boolean()))
                                        .DefaultSort(e => e.Name)
                                        .DefaultPageSize(25));
                                }
                                """),
                            new("2. MyApp.Server/Program.cs", """
                                using Klyroo.Query;
                                using Klyroo.Query.EntityFrameworkCore;
                                using Microsoft.EntityFrameworkCore;

                                builder.Services.ConfigureHttpJsonOptions(o =>
                                    o.SerializerOptions.TypeInfoResolverChain.Insert(0, DataListJsonContext.Default));

                                app.MapPost("/api/employees/query", async (DataListRequest req, AppDbContext db, CancellationToken ct) =>
                                {
                                    // Project entity -> DTO before Apply so SQL selects only DTO columns
                                    IQueryable<EmployeeDto> projected = db.Employees
                                        .AsNoTracking()
                                        .Select(e => new EmployeeDto(e.Id, e.Name, e.Department.Name, e.Salary, e.Active));

                                    var query = EmployeeSchemas.Directory.Apply(projected, req);
                                    int? total = query.WantsTotal ? await EfAsyncQueryExecutor.CountAsync(query.Matching, ct) : null;

                                    var facets = new List<DataListFacetResult>();
                                    foreach (var fq in EmployeeSchemas.Directory.FacetQueries(projected, req))
                                        facets.Add(fq.ToResult(await EfAsyncQueryExecutor.ToListAsync(fq.Buckets, ct)));

                                    var items = await EfAsyncQueryExecutor.ToListAsync(query.Page, ct);
                                    return Results.Ok(EmployeeSchemas.Directory.ToResult(req, items, total, facets));
                                });
                                """),
                            new("3. MyApp.Client/EmployeeClient.cs", """
                                using System.Net.Http.Json;
                                using Klyroo.Query;

                                public sealed class EmployeeClient(HttpClient http)
                                {
                                    public async ValueTask<DataListResult<EmployeeDto>> QueryAsync(DataListRequest req, CancellationToken ct)
                                    {
                                        using var resp = await http.PostAsJsonAsync(
                                            "/api/employees/query", req, DataListJsonContext.Default.DataListRequest, ct);
                                        resp.EnsureSuccessStatusCode();
                                        return (await resp.Content.ReadFromJsonAsync<DataListResult<EmployeeDto>>(ct))!;
                                    }
                                }
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 5: PLATFORMS
            // =========================================================================
            MauiAndAotPage("Klyroo.Query", "Klyroo.Query.Core", "Klyroo.Query", "builder.Services.AddKlyrooQuery();",
                "var page = EmployeeSchemas.Directory.ExecuteInMemory(localEmployees, request);"),

            // =========================================================================
            // GROUP 6: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive, runnable playgrounds exercising Klyroo.Query.Core schemas, compound FilterGroup ASTs, NativeAOT JSON serialization, offset & keyset cursor paging, faceted search, and URL codec round-tripping.",
                [
                    new("inmemory-search", "1. In-Memory Search, Sort & Boolean Filter",
                        Paragraphs: ["Execute a strongly typed DataListSchema<T> directly over an in-memory sequence with instant search, sort toggling, and boolean predicate filtering — zero reflection and NativeAOT-ready."],
                        LiveDemoKey: "query-inmemory",
                        LiveDemoTitle: "ExecuteInMemory · Search + Sort + Filter",
                        CodeSamples:
                        [
                            new("ProductQuery.cs", """
                                var schema = DataListSchema.For<Product>(s => s
                                    .RowKey(p => p.Id)
                                    .Search(p => p.Name, p => p.Category)
                                    .Field(p => p.Price, f => f.Key("price").Sortable().Filter(DataListFilter.Number()))
                                    .Field(p => p.Category, f => f.Key("category").Filter(DataListFilter.Option()))
                                    .Field(p => p.Active, f => f.Key("active").Filter(DataListFilter.Boolean())));

                                var request = new DataListRequest(
                                    paging: new DataListPaging.Offset(Skip: 0, Take: 10),
                                    sort: [new DataListSort("price", Descending: true)],
                                    filter: DataListFilterGroup.And(
                                        new FilterRule("active", FilterOperator.Is, new FilterValue.Boolean(true))),
                                    search: new DataListSearch("studio"));

                                DataListResult<Product> result = schema.ExecuteInMemory(catalog, request);
                                """)
                        ]),
                    new("compound-ast", "2. Compound Range & List Filter AST + NativeAOT JSON",
                        Paragraphs: ["Combine FilterOperator.Between (FilterValue.Range) and FilterOperator.IsAnyOf (FilterValue.List) inside a single DataListFilterGroup and inspect the exact wire payload produced by DataListJsonContext."],
                        LiveDemoKey: "query-filters",
                        LiveDemoTitle: "Compound FilterGroup AST + DataListJsonContext",
                        CodeSamples:
                        [
                            new("CompoundFilterAst.cs", """
                                var rules = new List<DataListFilterNode>
                                {
                                    new FilterRule("price", FilterOperator.Between,
                                        new FilterValue.Range(new FilterValue.Number(min), new FilterValue.Number(max))),
                                    new FilterRule("category", FilterOperator.IsAnyOf,
                                        new FilterValue.List(["Displays", "Audio"]))
                                };

                                var request = new DataListRequest(
                                    paging: new DataListPaging.Offset(0, 10),
                                    sort: [new DataListSort("price", Descending: true)],
                                    filter: new DataListFilterGroup(FilterCombinator.And, rules),
                                    search: null);

                                var result  = schema.ExecuteInMemory(catalog, request);
                                var payload = JsonSerializer.Serialize(request, DataListJsonContext.Default.DataListRequest);
                                """)
                        ]),
                    new("deterministic-paging", "3. Deterministic Offset Pagination & PageSize Switching",
                        Paragraphs: ["Switch Page and PageSize dynamically with DataListPaging.Offset.ForPage while keeping TotalCount and slice boundaries strictly deterministic."],
                        LiveDemoKey: "query-paging",
                        LiveDemoTitle: "Deterministic Offset Paging & TotalCount",
                        CodeSamples:
                        [
                            new("PagedQuery.cs", """
                                var request = new DataListRequest(
                                    paging: DataListPaging.Offset.ForPage(page: currentPage, pageSize: pageSize),
                                    sort: [new DataListSort("price", Descending: false)],
                                    filter: DataListFilterGroup.Empty);

                                var page = schema.ExecuteInMemory(catalog, request);
                                var totalPages = Math.Max(1, (int)Math.Ceiling((page.TotalCount ?? 0) / (double)pageSize));
                                """)
                        ]),
                    new("facets-and-cursor", "4. Faceted Counts (ExcludeSelf / IncludeSelf) & Keyset Cursor Paging",
                        Paragraphs: ["Compare FacetMode.ExcludeSelf vs IncludeSelf live and step forward through opaque keyset cursor tokens (DataListPaging.Cursor) with Take = 2."],
                        LiveDemoKey: "query-facets-cursor",
                        LiveDemoTitle: "Facets + Keyset Cursor Pagination",
                        CodeSamples:
                        [
                            new("FacetsAndCursor.cs", """
                                var request = new DataListRequest(
                                    paging: new DataListPaging.Cursor(After: cursor, Take: 2, Count: true),
                                    sort: [new DataListSort("price", Descending: true)],
                                    filter: DataListFilterGroup.Empty,
                                    search: null,
                                    facets: [new DataListFacetRequest("category", Mode: FacetMode.ExcludeSelf)]);

                                var result = schema.ExecuteInMemory(catalog, request);
                                // Next page token: result.NextCursor
                                // Facet counts:    result.Facets![0].Options
                                """)
                        ]),
                    new("url-codecs-live", "5. Live URL Query Round-Trip (DataListStateQuery)",
                        Paragraphs: ["Modify search, prefix isolation, layout, page, and filters to inspect the exact URL query string emitted by DataListStateQuery.ToQuery and verified via DataListStateQuery.FromQuery."],
                        LiveDemoKey: "query-url-codecs",
                        LiveDemoTitle: "DataListStateQuery · ToQuery / FromQuery Live",
                        CodeSamples:
                        [
                            new("UrlRoundTrip.cs", """
                                var state = new DataListState(
                                    search: "studio",
                                    filters: [new FilterRule("category", FilterOperator.IsAnyOf, new FilterValue.List(["Audio"]))],
                                    sort: [new DataListSort("price", Descending: true)],
                                    page: 2,
                                    pageSize: 25,
                                    layout: DataListLayout.Grid);

                                var pairs    = DataListStateQuery.ToQuery(state, schema, prefix: "cat");
                                var restored = DataListStateQuery.FromQuery(
                                    pairs.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value)), schema, prefix: "cat");
                                // restored.Equals(state) == true
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 7: REFERENCE
            // =========================================================================
            new("api", "API Reference", "Reference",
                "Complete public API reference for Klyroo.Query.Core and Klyroo.Query.",
                [],
                ApiTypes:
                [
                    typeof(DataListRequest),
                    typeof(DataListResult<object>),
                    typeof(DataListSort),
                    typeof(DataListSearch),
                    typeof(DataListFacetRequest),
                    typeof(DataListFacetResult),
                    typeof(DataListState),
                    typeof(FilterRule),
                    typeof(DataListFilterGroup),
                    typeof(DataListFilter),
                    typeof(DataListRemoteOptions),
                    typeof(QueryState),
                    typeof(QueryClaim)
                ])
        ]);
}
