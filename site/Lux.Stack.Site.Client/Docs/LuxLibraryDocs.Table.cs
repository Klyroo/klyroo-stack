using Lux.Table;
using Lux.Query;

namespace Lux.Stack.Site.Client.Docs;

public static partial class LuxLibraryDocs
{
    private static LibraryDocTree BuildTable() => new(
        "table", "Lux.Table", "#00d8ff",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "Enterprise DataList, card grids, visual grouping, footer aggregations, and NativeAOT headless tree hierarchy engine for Blazor Web and .NET MAUI Blazor Hybrid.",
                [
                    new("overview", "What is Lux.Table?",
                        Paragraphs:
                        [
                            "Lux.Table separates headless tree/hierarchy modeling (Lux.Table.Core, 100% NativeAOT C#) from high-density Blazor Web & .NET MAUI Blazor Hybrid table and card-grid components (Lux.Table).",
                            "Every table is powered by a DataListSchema<T> from Lux.Query.Core: columns, sorting, filtering, facets, URL synchronization, sticky column pinning, keyboard/pointer column resizing with cookie/localStorage persistence, hierarchical tree expansion with continuation rails, visual row grouping, footer summaries, bulk selection bars, 2px loading progress lines, 9 cell skeleton shapes, and automatic phone summary rows work seamlessly together."
                        ],
                        LiveDemoKey: "table-basic",
                        LiveDemoTitle: "Lux.Table · Live LxDataList with HeaderMenu & Footer Sum"),
                    new("pipeline", "How Lux.Table Works End-to-End (The 5-Stage Lifecycle)",
                        Paragraphs: ["LxDataList<TItem> acts as a headless coordinator that cascades a DataListContext<TItem> to every child slot (Toolbar, FilterBar, FilterPanel, SelectionBar, Table, Grid, Pagination, LoadMore)."],
                        TableHeaders: ["Stage", "Component / Type", "What Happens"],
                        TableRows:
                        [
                            new("1. Schema & Source", "DataListSchema<T> + Items or Provider", "Declares sortable/filterable columns, RowKey, and either local Items (InMemoryProvider) or an async server DataListProvider<T>."),
                            new("2. URL & Preferences", "QueryState (UrlSync) + Lux.Table.lib.module.js", "Reads query state (?q=&p=&s=&f.*) via DataListStateQuery and restores pinned/hidden/resized column widths from cookie (Web) or localStorage (MAUI)."),
                            new("3. Execute Query", "DataListController<TItem>", "Coalesces filter/sort/page edits, shows a 2px progress line during background reloads, and cancels stale in-flight requests via CancellationToken."),
                            new("4. Shape Rows", "DataListTree<T> · DataListGroupBy<T> · DataListSummaries<T>", "Builds cycle-guarded tree nodes, collapsible group headers, or footer Sum/Average/Count/Min/Max aggregations."),
                            new("5. Responsive Render", "LxDataListTable · LxDataListGrid · DataListRowSummary", "Renders full desktop columns, switches to Card Grid via layout toggle, or auto-switches to 2-line mobile summary rows under 768px.")
                        ]),
                    new("layers", "Two-Layer Architecture",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Lux.Table.Core", "DataListTree<TItem>, DataListTreeNode<TItem>, DataListTreeSelectionMode", "Headless C# · IsAotCompatible=true"),
                            new("Lux.Table", "LxDataList, LxDataListTable, LxDataListGrid, LxDataListColumn, Toolbar, FilterBar, FilterPanel, SelectionBar, Pagination, LoadMore, GroupBy, Summaries, RowSummary, Lux.Table.lib.module.js", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Lux.Table, register services, and understand the automatic Blazor JS initializer.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Lux.Table.Core   # Headless NativeAOT tree hierarchy engine
                                dotnet add package Lux.Table        # Full Blazor Web & .NET MAUI DataList
                                """)
                        ]),
                    new("di", "2. Register Services",
                        Paragraphs: ["AddLuxTable() registers Lux.UI, Lux.Query, and optional LuxTableOptions theme/default overrides. When using Lux.Stack, AddLuxStack() already registers Lux.Table."],
                        CodeSamples:
                        [
                            new("Program.cs / MauiProgram.cs", """
                                using Lux.Table;

                                builder.Services.AddLuxTable();
                                """),
                            new("_Imports.razor", """
                                @using Lux.UI
                                @using Lux.Query
                                @using Lux.Table
                                """)
                        ]),
                    new("js", "3. Automatic JS Initializer (Lux.Table.lib.module.js)",
                        Paragraphs: ["Lux.Table automatically ships Lux.Table.lib.module.js as a Blazor JS module initializer — no manual <script> tag required. It handles pointer/keyboard column resize handles, persisted column width/visibility cookies (or localStorage inside MAUI BlazorWebView), sticky actions-column width measurement, focus preservation across virtualized renders, and 768px mobile viewport detection."])
                ]),

            // =========================================================================
            // GROUP 2: CORE COMPONENTS & FEATURES
            // =========================================================================
            new("data-list", "LxDataList, Toolbar & Layouts", "Components",
                "Coordinate Table and Card Grid views, Toolbars, FilterBars, FilterPanels, Bulk Selection Bars, and Pagination inside one LxDataList<TItem> root.",
                [
                    new("composition", "Full Enterprise Composition",
                        Paragraphs: ["LxDataList<TItem> cascades DataListContext<TItem> to every descendant so you can compose Toolbars, FilterBars, FilterPanels, SelectionBars, Tables, Grids, LayoutSwitches, ViewToggles, and Paginators in any layout order."],
                        LiveDemoKey: "table-basic",
                        LiveDemoTitle: "LxDataList with HeaderMenu & Footer Sum",
                        CodeSamples:
                        [
                            new("DirectoryPage.razor", """
                                <LxDataList TItem="Employee"
                                            Schema="@EmployeeSchemas.Directory"
                                            Items="@_employees"
                                            UrlSync="true"
                                            ApplyMode="DataListApplyMode.Live">
                                    <LxDataListToolbar TItem="Employee" />
                                    <LxDataListFilterBar TItem="Employee" />
                                    <LxDataListTable TItem="Employee"
                                                     HeaderMenu="true"
                                                     ShowProgressLine="true"
                                                     Density="DataListDensity.Default"
                                                     Summaries="@(new DataListSummaries<Employee>())">
                                        <LxDataListColumn TItem="Employee" Field="name" Title="Employee" Pin="DataListPin.Start" />
                                        <LxDataListColumn TItem="Employee" Field="dept" Title="Department" />
                                        <LxDataListColumn TItem="Employee" Field="salary" Title="Salary"
                                                          Align="DataListAlign.End"
                                                          Summary="DataListSummaryKind.Sum" />
                                    </LxDataListTable>
                                    <LxDataListPagination TItem="Employee" />
                                </LxDataList>
                                """)
                        ]),
                    new("enums", "Layout, Density & Selection Enums",
                        TableHeaders: ["Enum", "Exact Values", "Behavior"],
                        TableRows:
                        [
                            new("DataListApplyMode", "Live, Submit", "Live re-queries on every filter change; Submit waits for an explicit Apply button"),
                            new("DataListDensity", "Default, Compact, Comfortable", "Controls vertical cell padding and information density"),
                            new("DataListAlign", "Start, Center, End", "Horizontal alignment for header, body, and summary cells"),
                            new("DataListSelectionScope", "Auto, Page, All", "Defines whether Select-All selects visible page rows or the entire filtered result set"),
                            new("DataListSelectionClear", "Never, Trigger, Success", "Controls when selected keys clear after invoking a bulk action"),
                            new("DataListSelectionBarPosition", "Sticky, Fixed", "Sticky bar above table vs floating fixed bar at viewport bottom")
                        ])
                ]),
            new("columns", "Columns, Pinning & HeaderMenu", "Components",
                "Configure sortable, hideable, resizable, and sticky-pinned columns with automatic HeaderMenu actions and keyboard resizing.",
                [
                    new("header-menu", "HeaderMenu, Pinning & Keyboard Resizing",
                        Paragraphs:
                        [
                            "Set HeaderMenu=\"true\" on LxDataListTable so clicking any sortable or hideable column header opens an accessible popover menu offering Sort Ascending, Sort Descending, Clear Sort, Pin Start/End, and Hide Column.",
                            "When column resizing is enabled, resize handles support both pointer drag and keyboard navigation (ArrowRight/ArrowLeft in 16px steps, Shift+Arrow in 64px steps, Home/End for min/max width, Delete/Backspace to reset). Set PreferencesKey on LxDataList to persist column order, visibility, pins, and widths automatically."
                        ],
                        TableHeaders: ["DataListPin", "Behavior", "Typical Column"],
                        TableRows:
                        [
                            new("DataListPin.None", "Scrolls horizontally with table body (default)", "Standard data columns"),
                            new("DataListPin.Start", "Sticky to the leading edge (RTL-aware)", "Selection checkbox, Name/ID column"),
                            new("DataListPin.End", "Sticky to the trailing edge (RTL-aware)", "Row actions menu, Total column")
                        ],
                        CodeSamples:
                        [
                            new("PinnedColumns.razor", """
                                <LxDataListTable TItem="Employee" HeaderMenu="true">
                                    <LxDataListColumn TItem="Employee" Field="name" Title="Employee" Pin="DataListPin.Start" SkeletonKind="DataListSkeletonKind.Avatar" />
                                    <LxDataListColumn TItem="Employee" Field="dept" Title="Department" SkeletonKind="DataListSkeletonKind.Badge" />
                                    <LxDataListColumn TItem="Employee" Field="salary" Title="Salary" Align="DataListAlign.End" SkeletonKind="DataListSkeletonKind.Number" Summary="DataListSummaryKind.Sum" />
                                </LxDataListTable>
                                """)
                        ])
                ]),
            new("tree", "Tree Hierarchy (DataListTree)", "Components",
                "Render arbitrarily nested parent-child trees with indentation rails, keyboard expand/collapse, and cycle/duplicate key protection.",
                [
                    new("headless-tree", "DataListTree<TItem> in Lux.Table.Core (NativeAOT)",
                        Paragraphs:
                        [
                            "DataListTree<TItem> is 100% headless in Lux.Table.Core. BuildTree(rootItems, schema) skips null entries and duplicate keys (preventing infinite cycles) and builds flat DataListTreeNode<TItem> descriptors carrying Key, Item, Depth, Parent, RootIndex, Children, HasChildren, IsLast, and AncestorContinuations.",
                            "DefaultExpandedDepth (e.g., 1 expands roots only, 2 expands roots and children) overrides DefaultExpanded when specified."
                        ],
                        TableHeaders: ["Selection Mode", "Behavior", "Best For"],
                        TableRows:
                        [
                            new("DataListTreeSelectionMode.All", "Every root and descendant row renders its own checkbox", "Folder trees, org charts, permissions"),
                            new("DataListTreeSelectionMode.Roots", "Only depth-0 root rows show checkboxes; descendants reserve alignment width", "Invoices with nested line items")
                        ],
                        LiveDemoKey: "table-tree",
                        LiveDemoTitle: "Interactive DataListTree<TItem> Org Hierarchy",
                        CodeSamples:
                        [
                            new("OrgTree.cs", """
                                var tree = new DataListTree<OrgRow>
                                {
                                    Children = e => e.Reports,
                                    DefaultExpandedDepth = 2,
                                    Selectable = DataListTreeSelectionMode.All
                                };

                                var (roots, allNodes) = tree.BuildTree(orgRoots, orgSchema);
                                """)
                        ],
                        Callout: "Keyboard support: press Enter/Space or ArrowRight to expand a focused branch node, and ArrowLeft to collapse it.")
                ]),
            new("grouping-and-summaries", "Grouping, Summaries & Row Actions", "Components",
                "Group rows under collapsible headers, calculate footer totals across 6 aggregation modes, and attach row/bulk actions with confirmation dialogs.",
                [
                    new("grouping", "Visual Row Grouping (DataListGroupBy<TItem>)",
                        Paragraphs: ["DataListGroupBy<TItem> groups loaded rows by any key selector (Key = e => e.Dept), renders collapsible group headers with item counts and tri-state group checkboxes, and persists fold state when PreferencesKey is set."],
                        LiveDemoKey: "table-grouped",
                        LiveDemoTitle: "Interactive GroupBy & Density Switcher",
                        CodeSamples:
                        [
                            new("Grouping.cs", """
                                var groupBy = new DataListGroupBy<Employee>
                                {
                                    Key = e => e.Dept
                                };
                                """)
                        ]),
                    new("summaries", "All 6 DataListSummaryKind Aggregations & Scopes",
                        TableHeaders: ["Kind", "Computation", "Example Use"],
                        TableRows:
                        [
                            new("DataListSummaryKind.Sum", "Arithmetic sum of numeric values", "Total Payroll, Revenue, Quantity"),
                            new("DataListSummaryKind.Average", "Mean across non-null numeric rows", "Average Rating, Margin %, Latency"),
                            new("DataListSummaryKind.Count", "Count of non-empty values", "Active Records, Assigned Tasks"),
                            new("DataListSummaryKind.Min / Max", "Minimum or maximum value in column", "Lowest Cost, Peak Throughput"),
                            new("DataListSummaryKind.None", "No summary calculation (default)", "Free-text or action columns")
                        ]),
                    new("actions", "Row Actions & Bulk Selection Actions",
                        Paragraphs: ["Attach DataListRowAction<TItem> to individual rows (moving focus safely to the next row when an action deletes a record) and DataListAction<TItem> to the bulk selection bar with built-in confirmation dialogs (DataListActionConfirm<TItem>)."])
                ]),
            new("mobile-and-polish", "Mobile Summary Rows, Skeletons & Progress", "Components",
                "Switch wide tables automatically into scannable two-line mobile rows below 768px and keep reloads silky smooth with 9 cell skeleton shapes and a 2px progress line.",
                [
                    new("row-summary", "DataListRowSummary<TItem> & Responsive View Toggle",
                        Paragraphs: ["DataListRowSummary<TItem> eliminates awkward horizontal scrolling on phones. Below 768px, AutoPhone = true automatically displays a Leading 32px visual, primary Title, Status pill, Details joined with ' · ', selection checkbox, and row actions."],
                        LiveDemoKey: "table-density-summary",
                        LiveDemoTitle: "Interactive Summary Aggregation & Mobile RowSummary Toggle",
                        CodeSamples:
                        [
                            new("PhoneSummary.cs", """
                                private static readonly DataListRowSummary<Employee> PhoneSummary = new()
                                {
                                    Title = e => e.Name,
                                    Details = e => [e.Dept, $"€{e.Salary:N0}"],
                                    Href = e => $"/employees/{e.Id}",
                                    AutoPhone = true
                                };
                                """)
                        ]),
                    new("skeletons", "All 9 DataListSkeletonKind Cell Shapes",
                        TableHeaders: ["SkeletonKind", "Rendered Placeholder", "Recommended Column"],
                        TableRows:
                        [
                            new("Text / Number", "Horizontal pill (start-aligned for Text, end-aligned for Number)", "Titles, names, prices, quantities"),
                            new("Avatar / Icon / Dot", "32px circle, 20px icon square, or 10px status dot", "User profiles, file type icons, status dots"),
                            new("Badge / Check / Progress", "Status pill, checkbox square, or thin progress bar", "Statuses, booleans, goal completion %"),
                            new("None", "Leaves cell empty while loading", "Action columns or spacers")
                        ])
                ]),

            // =========================================================================
            // GROUP 3: INTEGRATIONS WITH OTHER LUXSTACK LIBRARIES
            // =========================================================================
            new("integrations-query", "Integration: Lux.Query & EF Core", "Integrations",
                "Wire LxDataList directly to Lux.Query schemas, remote EF Core Minimal APIs, facets, URL synchronization, and keyset cursor infinite scroll.",
                [
                    new("query-provider", "1. Connecting LxDataList to an EF Core Minimal API via DataListProvider<T>",
                        Paragraphs: ["Share one DataListSchema<T> from Lux.Query.Core between your server API and your Blazor/MAUI client. LxDataList invokes your DataListProvider<T> whenever search, filters, sorts, or paging change, cancelling stale requests automatically."],
                        CodeSamples:
                        [
                            new("ServerBackedTable.razor", """
                                @using Lux.Query
                                @using Lux.Table
                                @inject HttpClient Http

                                <LxDataList TItem="EmployeeDto"
                                            Schema="@EmployeeSchemas.Directory"
                                            Provider="@LoadAsync"
                                            UrlSync="true"
                                            UrlPrefix="emp"
                                            PreferencesKey="emp-directory">
                                    <LxDataListToolbar TItem="EmployeeDto" />
                                    <LxDataListFilterPanel TItem="EmployeeDto" />
                                    <LxDataListTable TItem="EmployeeDto" HeaderMenu="true" ShowProgressLine="true">
                                        <LxDataListColumn TItem="EmployeeDto" Field="name" Title="Employee" Pin="DataListPin.Start" />
                                        <LxDataListColumn TItem="EmployeeDto" Field="dept" Title="Department" />
                                        <LxDataListColumn TItem="EmployeeDto" Field="salary" Title="Salary" Summary="DataListSummaryKind.Sum" />
                                    </LxDataListTable>
                                    <LxDataListPagination TItem="EmployeeDto" />
                                </LxDataList>

                                @code {
                                    private async ValueTask<DataListResult<EmployeeDto>> LoadAsync(DataListRequest req, CancellationToken ct)
                                    {
                                        using var resp = await Http.PostAsJsonAsync(
                                            "/api/employees/query", req, DataListJsonContext.Default.DataListRequest, ct);
                                        resp.EnsureSuccessStatusCode();
                                        return (await resp.Content.ReadFromJsonAsync<DataListResult<EmployeeDto>>(ct))!;
                                    }
                                }
                                """)
                        ]),
                    new("ssr-cookies", "2. Zero-Flicker Column Persistence in Static SSR (Lux.UI.Server)",
                        Paragraphs: ["Register builder.Services.AddLuxServer() on the ASP.NET Core server so hidden columns, column order, pins, and widths saved in the PreferencesKey cookie are applied on the very first SSR HTML frame with zero layout shift."])
                ]),
            new("integrations-form-display", "Integration: Lux.Form & Lux.Display", "Integrations",
                "Launch row edit drawers with Lux.Form (FormService.CreateFormAsync) and preview row attachments or detail modals with Lux.Display (FilePreviewService & LxDisplay).",
                [
                    new("row-edit-form", "1. Row Action -> Lux.Form Drawer (Edit & Reload Table)",
                        Paragraphs: ["Wire a DataListRowAction<TItem> to FormService.CreateFormAsync(editSchema, initialData, FormShellMode.Drawer). When outcome.Submitted is true, persist the record and reload the table cleanly."],
                        CodeSamples:
                        [
                            new("EditRowDrawer.cs", """
                                using Lux.Form;
                                using Lux.Table;

                                var editAction = new DataListRowAction<Employee>("edit", "Edit Employee", async ctx =>
                                {
                                    var outcome = await formService.CreateFormAsync(
                                        EmployeeFormSchemas.Edit,
                                        input: new Dictionary<string, object?>
                                        {
                                            ["name"] = ctx.Item.Name,
                                            ["dept"] = ctx.Item.Dept,
                                            ["salary"] = (double)ctx.Item.Salary
                                        },
                                        mode: FormShellMode.Drawer);

                                    if (outcome.Submitted)
                                    {
                                        await api.UpdateEmployeeAsync(ctx.Item.Id, outcome.Data);
                                        await _dataListRef.ReloadAsync();
                                    }
                                });
                                """)
                        ]),
                    new("row-preview-display", "2. Row Action -> Lux.Display FilePreviewService Lightbox",
                        Paragraphs: ["Open PDF invoices, contracts, or photos directly from any table row using FilePreviewService.Open(...) without leaving the table's scroll or filter state."],
                        CodeSamples:
                        [
                            new("PreviewRowFile.cs", """
                                using Lux.Display;
                                using Lux.Table;

                                var previewAction = new DataListRowAction<DocumentRow>("preview", "Preview File", ctx =>
                                {
                                    filePreview.Open(new FilePreviewFile
                                    {
                                        Src = ctx.Item.Url,
                                        Name = ctx.Item.FileName,
                                        Size = ctx.Item.SizeBytes
                                    });
                                    return Task.CompletedTask;
                                });
                                """)
                        ])
                ]),
            new("integrations-spreadsheet-dashboard", "Integration: Lux.Spreadsheet, Lux.Dashboard & Lux.Agent", "Integrations",
                "Pair bulk CSV imports (Lux.Spreadsheet) with live data tables, drill down from Lux.Dashboard cards into filtered LxDataList views, and test everything in xUnit/bUnit.",
                [
                    new("import-to-table", "1. Lux.Spreadsheet Bulk Import -> Refresh Lux.Table",
                        CodeSamples:
                        [
                            new("ImportAndRefresh.razor", """
                                @* After LxSpreadsheetImport finishes SubmitAsync(), refresh your LxDataList immediately *@
                                <LxSpreadsheetImport Importer="@_importer" OnCompleted="@(() => _listRef.ReloadAsync())" />
                                <LxDataList @ref="_listRef" TItem="ProductRate" Schema="@RateSchema" Provider="@LoadRatesAsync">
                                    <LxDataListTable TItem="ProductRate" ShowProgressLine="true" />
                                </LxDataList>
                                """)
                        ]),
                    new("dashboard-drilldown", "2. Lux.Dashboard KPI Click -> Pre-Filtered Lux.Table URL",
                        Paragraphs: ["Generate deep-links from any Lux.Dashboard card or alert using DataListStateQuery.ToQuery(...) so clicking 'Pending Invoices (12)' lands directly on your Lux.Table page with ?f.status=Pending already active."],
                        CodeSamples:
                        [
                            new("DrilldownLink.cs", """
                                var state = DataListState.Initial with
                                {
                                    Filters = [new FilterRule("status", FilterOperator.Is, new FilterValue.Text("Pending"))]
                                };
                                var qs = string.Join("&", DataListStateQuery.ToQuery(state, InvoiceSchemas.All).Select(kv => $"{kv.Key}={kv.Value}"));
                                string href = $"/invoices?{qs}"; // Pass to LxDashboardStat / DashboardAlertItem.ActionHref
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: PLATFORMS & TESTING
            // =========================================================================
            MauiAndAotPage("Lux.Table", "Lux.Table.Core", "Lux.Table", "builder.Services.AddLuxTable();",
                "var (roots, allNodes) = new DataListTree<Employee> { Children = e => e.Reports }.BuildTree(employees);"),
            new("recipes", "Testing & Recipes", "Platforms",
                "Unit testing DataListTree in xUnit and rendering LxDataList in bUnit.",
                [
                    new("xunit", "Headless Tree Tests",
                        CodeSamples:
                        [
                            new("TableCoreTests.cs", """
                                var tree = new DataListTree<Employee> { Children = e => e.Reports, DefaultExpanded = true };
                                var (_, allNodes) = tree.BuildTree([new Employee(1, "Alice", "Eng", 120000m, [new(2, "Bob", "Eng", 95000m)])]);

                                Assert.Equal(2, allNodes.Count);
                                Assert.Equal(0, allNodes[0].Depth);
                                Assert.Equal(1, allNodes[1].Depth);
                                """)
                        ]),
                    new("bunit", "bUnit Service Registration Order",
                        CodeSamples:
                        [
                            new("TableBunitTests.cs", """
                                Services.AddLuxTable();
                                JSInterop.Mode = JSRuntimeMode.Loose;
                                SetRendererInfo(new RendererInfo("Server", true));

                                var cut = Render<LxDataList<Employee>>(...);
                                Assert.Equal(2, cut.FindAll("tbody tr").Count);
                                Assert.NotNull(cut.Find("tfoot"));
                                """)
                        ],
                        Callout: "Always invoke Services.AddLuxTable() BEFORE SetRendererInfo(...) in bUnit tests; SetRendererInfo resolves the renderer and locks DI.")
                ]),

            // =========================================================================
            // GROUP 5: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive LxDataList playgrounds demonstrating HeaderMenu, Sum/Average/Min/Max footers, hierarchical trees, GroupBy density switching, mobile RowSummary, and headless BuildTree.",
                [
                    new("basic-summary", "1. Interactive Grid with HeaderMenu & Sum Footer",
                        Paragraphs: ["Click column headers to sort or open the HeaderMenu, and inspect the live Sum aggregation in <tfoot>."],
                        LiveDemoKey: "table-basic",
                        LiveDemoTitle: "LxDataList · Sorting, HeaderMenu & Sum Footer",
                        CodeSamples:
                        [
                            new("EmployeeGrid.razor", """
                                <LxDataList TItem="Employee" Schema="@EmployeeSchema" Items="@_employees" UrlSync="false">
                                    <LxDataListTable TItem="Employee" HeaderMenu="true" ShowProgressLine="true"
                                                     Summaries="@(new DataListSummaries<Employee>())">
                                        <LxDataListColumn TItem="Employee" Field="name" Title="Employee" />
                                        <LxDataListColumn TItem="Employee" Field="dept" Title="Department" />
                                        <LxDataListColumn TItem="Employee" Field="salary" Title="Salary (€)"
                                                          Summary="DataListSummaryKind.Sum" />
                                    </LxDataListTable>
                                </LxDataList>
                                """)
                        ]),
                    new("tree-hierarchy", "2. Hierarchical Tree Grid with Expand/Collapse Rails",
                        Paragraphs: ["Render parent-child hierarchies using DataListTree<TItem> with DefaultExpandedDepth and keyboard navigation."],
                        LiveDemoKey: "table-tree",
                        LiveDemoTitle: "DataListTree<TItem> · Expandable Org Chart",
                        CodeSamples:
                        [
                            new("OrgTree.razor", """
                                <LxDataList TItem="OrgRow" Schema="@OrgSchema" Items="@_roots" UrlSync="false">
                                    <LxDataListTable TItem="OrgRow"
                                                     Tree="@(new DataListTree<OrgRow> { Children = m => m.Reports, DefaultExpandedDepth = 2 })">
                                        <LxDataListColumn TItem="OrgRow" Field="name" Title="Organization Hierarchy" />
                                        <LxDataListColumn TItem="OrgRow" Field="role" Title="Role" />
                                    </LxDataListTable>
                                </LxDataList>
                                """)
                        ]),
                    new("grouped-rows", "3. Visual Grouping & Live Density Switcher",
                        Paragraphs: ["Switch between Default, Compact, and Comfortable density while grouping rows by department with Average salary summaries."],
                        LiveDemoKey: "table-grouped",
                        LiveDemoTitle: "DataListGroupBy<TItem> · Department Grouping & Density",
                        CodeSamples:
                        [
                            new("DepartmentGrid.razor", """
                                <LxDataList TItem="Employee" Schema="@EmployeeSchema" Items="@_employees" UrlSync="false">
                                    <LxDataListTable TItem="Employee"
                                                     Density="@_density"
                                                     GroupBy="@(new DataListGroupBy<Employee> { Key = e => e.Dept })"
                                                     Summaries="@(new DataListSummaries<Employee>())">
                                        <LxDataListColumn TItem="Employee" Field="name" Title="Employee" />
                                        <LxDataListColumn TItem="Employee" Field="dept" Title="Department" />
                                        <LxDataListColumn TItem="Employee" Field="salary" Title="Salary (€)"
                                                          Summary="DataListSummaryKind.Average" />
                                    </LxDataListTable>
                                </LxDataList>
                                """)
                        ]),
                    new("summary-and-mobile", "4. Dynamic Summary Aggregations & Mobile RowSummary Mode",
                        Paragraphs: ["Switch the footer summary function live across Sum, Average, Min, Max, and Count, and toggle the compact mobile DataListRowSummary presentation."],
                        LiveDemoKey: "table-density-summary",
                        LiveDemoTitle: "DataListSummaryKind & DataListRowSummary Live",
                        CodeSamples:
                        [
                            new("SummaryAndMobile.razor", """
                                <LxDataListTable TItem="Employee"
                                                 RowSummary="@_phoneSummary"
                                                 Summaries="@(new DataListSummaries<Employee>())">
                                    <LxDataListColumn TItem="Employee" Field="salary" Title="Salary (€)" Summary="@_summaryKind" />
                                </LxDataListTable>
                                """)
                        ]),
                    new("headless-tree-core", "5. Headless Lux.Table.Core BuildTree() Inspector (NativeAOT)",
                        Paragraphs: ["Inspect the flat DataListTreeNode<TItem> output produced by DataListTree<TItem>.BuildTree() with zero Blazor dependency — usable in CLI tools, APIs, and MAUI ViewModels."],
                        LiveDemoKey: "table-selection-actions",
                        LiveDemoTitle: "Lux.Table.Core · Headless BuildTree() Output",
                        CodeSamples:
                        [
                            new("HeadlessTree.cs", """
                                var tree = new DataListTree<OrgRow> { Children = r => r.Reports, DefaultExpandedDepth = 2 };
                                var (roots, allNodes) = tree.BuildTree(orgRoots, orgSchema);
                                // Each node exposes: Key, Item, Depth, Parent, HasChildren, IsLast, AncestorContinuations
                                """)
                        ])
                ]),
            new("api", "API Reference", "Reference",
                "Complete public API reference for Lux.Table.Core and Lux.Table.",
                [],
                ApiTypes: [typeof(LxDataList<object>), typeof(LxDataListTable<object>), typeof(LxDataListColumn<object>), typeof(DataListTree<object>), typeof(DataListGroupBy<object>), typeof(DataListSummaries<object>), typeof(DataListRowSummary<object>)])
        ]);
}
