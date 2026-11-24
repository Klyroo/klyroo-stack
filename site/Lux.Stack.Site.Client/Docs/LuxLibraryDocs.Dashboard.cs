using Lux.Dashboard;

namespace Lux.Stack.Site.Client.Docs;

public static partial class LuxLibraryDocs
{
    private static LibraryDocTree BuildDashboard() => new(
        "dashboard", "Lux.Dashboard", "#8b5cf6",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "Analytical dashboard orchestrator with 3-wave staged queries (Essential -> Background -> Deferred), global filters & presets, leap-year comparison ranges, localized UTF-8 BOM CSV export, and 13 visualization blocks.",
                [
                    new("overview", "What is Lux.Dashboard?",
                        Paragraphs:
                        [
                            "Analytical dashboards often suffer from slow waterfalls or blank full-page spinners. Lux.Dashboard puts a 100% NativeAOT orchestrator (DashboardController) in Lux.Dashboard.Core that executes data resources in 3 priority waves, coordinates global filters and badged views, formats numbers/deltas consistently, and feeds 13 ready-to-use Razor visualization blocks."
                        ],
                        LiveDemoKey: "dashboard-kpi",
                        LiveDemoTitle: "Lux.Dashboard · Live Executive Stats, SVG Gauge & Bars"),
                    new("pipeline", "How Lux.Dashboard Works End-to-End (The 3-Wave Orchestration Pipeline)",
                        TableHeaders: ["Wave / Stage", "API / Type", "What Happens"],
                        TableRows:
                        [
                            new("1. Register Resources", "DashboardController.RegisterResource(new DashboardResource<T>(...))", "Assigns every widget loader to Essential, Background, or Deferred stage with its own Data, Error, State, and UpdatedAt."),
                            new("2. Wave 1 (Essential)", "await dashboard.RefreshAsync()", "Loads critical KPIs first while cards show matched DashboardSkeletonKind placeholders; keeps existing data visible during refreshes."),
                            new("3. Wave 2 (Background)", "Automatic after Wave 1", "Streams secondary charts, regional bars, and tables without delaying hero KPIs."),
                            new("4. Wave 3 (Deferred)", "await resource.ActivateAsync()", "Heavy audit tables or secondary tabs remain Idle with zero bandwidth cost until explicitly opened."),
                            new("5. Filter, Format & Export", "DashboardFilterControl · DashboardFormat.ExportCsv", "Global filter changes flip controller.Filtered, resolve leap-year comparison ranges, and export culture-aware UTF-8 BOM CSV files.")
                        ]),
                    new("layers", "Two-Layer Package Split",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Lux.Dashboard.Core", "DashboardController, DashboardResource<TData>, DashboardFilterControl, DashboardViewMeta, DashboardFormat, all analytical record models", "Headless C# · IsAotCompatible=true"),
                            new("Lux.Dashboard", "13 analytical blocks: Page, Grid, Card, Stat, Stats, Gauge, Details, Bars, List, Table, Alerts, Feed, Chart", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Lux.Dashboard and register services.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Lux.Dashboard.Core   # Headless NativeAOT orchestrator & formatters
                                dotnet add package Lux.Dashboard        # 13 Blazor Web & .NET MAUI visualization blocks
                                """)
                        ]),
                    new("di", "2. Register Services & Imports",
                        CodeSamples:
                        [
                            new("Program.cs / MauiProgram.cs", "builder.Services.AddLuxDashboard();"),
                            new("_Imports.razor", "@using Lux.Dashboard")
                        ])
                ]),

            // =========================================================================
            // GROUP 2: ORCHESTRATION & FORMATTING
            // =========================================================================
            new("staged-queries", "Staged 3-Wave Queries", "Orchestration",
                "Prioritize critical KPIs, stream secondary charts second, and keep heavy resources deferred until explicitly activated.",
                [
                    new("stages", "Stages, Resource Lifecycle & Block Phases",
                        TableHeaders: ["Enum", "Exact Values", "Behavior"],
                        TableRows:
                        [
                            new("DashboardQueryStage", "Essential, Background, Deferred", "RefreshAsync() awaits Essential first, then Background; Deferred stays Idle until ActivateAsync()"),
                            new("DashboardResourceState", "Idle, Loading, Ready, Error, Disabled", "Tracked per DashboardResource<TData> and aggregated via DashboardController.CombineStates()"),
                            new("DashboardBlockPhase", "Idle, Loading, Error, Empty, Content", "Determines whether each card renders its skeleton, error state, empty state, or data")
                        ]),
                    new("example", "Interactive 3-Wave Staged Orchestration",
                        Paragraphs: ["DashboardResource<TData> tracks Data, Error, Refreshing, Fetching, UpdatedAt, and Active. Set controller.AutoRefresh (seconds) to enable periodic background refreshes while preserving displayed data during reload."],
                        LiveDemoKey: "dashboard-waves",
                        LiveDemoTitle: "Interactive 3-Wave Staged Loading (Essential · Background · Deferred)",
                        CodeSamples:
                        [
                            new("StagedOrchestration.cs", """
                                var dashboard = new DashboardController("revenue") { AutoRefresh = 60 };

                                var kpis = dashboard.RegisterResource(new DashboardResource<KpiSummary>(
                                    "kpis", LoadKpisAsync, DashboardQueryStage.Essential));
                                var channels = dashboard.RegisterResource(new DashboardResource<IReadOnlyList<DashboardBarItem>>(
                                    "channels", LoadChannelsAsync, DashboardQueryStage.Background));
                                var audit = dashboard.RegisterResource(new DashboardResource<DashboardDataTable>(
                                    "audit", LoadHeavyAuditAsync, DashboardQueryStage.Deferred));

                                // Wave 1: kpis -> Wave 2: channels; audit remains Idle
                                await dashboard.RefreshAsync();

                                // Wave 3 runs on demand when user opens the Audit tab:
                                await audit.ActivateAsync();
                                """)
                        ])
                ]),
            new("filters-and-views", "Global Filters, Presets & Views", "Orchestration",
                "Coordinate 10 filter kinds, quick presets, automatic Reset state detection, and badged dashboard views.",
                [
                    new("kinds", "All 10 DashboardFilterKind Values",
                        TableHeaders: ["Category", "Exact DashboardFilterKind Values", "Usage"],
                        TableRows:
                        [
                            new("Scalar & Toggle", "String, Number, Boolean", "Free-text search, numeric thresholds, include/exclude flags"),
                            new("Temporal", "Date, DateRange, Comparison", "Period selectors paired with Previous / Year comparison mode"),
                            new("Dimensions", "Enum, Options, Remote, Custom", "Single/multi-select dimensions, searchable remote lists & custom controls")
                        ]),
                    new("controls", "Interactive DashboardFilterControl & Views",
                        LiveDemoKey: "dashboard-filters-views",
                        LiveDemoTitle: "Interactive Badged Views, Multi-Select Filters & ResetFilters()",
                        CodeSamples:
                        [
                            new("FiltersAndViews.cs", """
                                dashboard.AddView("overview", "Overview", badge: "3");
                                dashboard.AddView("cohorts", "Cohorts");

                                var region = dashboard.AddFilter(new DashboardFilterControl(
                                    "region", "Region",
                                    items: [new("eu", "Europe"), new("us", "North America"), new("ap", "Asia-Pacific")],
                                    defaultValues: ["eu"],
                                    multiple: true,
                                    searchable: true));

                                region.Toggle("us");
                                Assert.True(dashboard.Filtered);
                                dashboard.ResetFilters();
                                Assert.False(dashboard.Filtered);
                                """)
                        ])
                ]),
            new("formatting-and-csv", "Formatting, Comparison Ranges & CSV", "Orchestration",
                "Consistent null/NaN-safe formatting, currency & relative time helpers, leap-year comparison ranges, and UTF-8 BOM CSV exports.",
                [
                    new("presets", "All 9 DashboardFormatPreset Values & Leap-Year Comparison",
                        TableHeaders: ["Preset / Method", "Sample Input", "Output (InvariantCulture)"],
                        TableRows:
                        [
                            new("Number / Integer / Decimal", "12600 / 12.345", "12,600 / 12 / 12.35"),
                            new("Compact", "12600 / 2400000", "12.6K / 2.4M"),
                            new("Percent / Ratio", "86 / 0.57", "86% / 57%"),
                            new("Delta / Points / Signed", "14.2 / 2.1 / 1.0", "+14.2% / +2.1 pts / +1 pt"),
                            new("FormatCurrency / FormatRelativeTime", "148900, \"€\" / 5m ago", "€148,900 / 5m ago"),
                            new("Null / NaN Guard", "null or double.NaN", "\"—\" (em-dash placeholder)")
                        ],
                        LiveDemoKey: "dashboard-format-compare",
                        LiveDemoTitle: "Interactive DashboardFormat & Leap-Year Comparison Calculator",
                        CodeSamples:
                        [
                            new("FormatAndCsv.cs", """
                                var leapStart = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero);
                                var leapEnd   = new DateTimeOffset(2024, 2, 29, 0, 0, 0, TimeSpan.Zero);
                                var yearAgo   = DashboardFormat.ResolveComparisonRange((leapStart, leapEnd), DashboardComparisonMode.Year);
                                // yearAgo.Value.End == 2023-02-28 (clamped safely!)

                                var fileName = DashboardFormat.ResolveFileName("Consommation mensuelle"); // "consommation-mensuelle.csv"
                                var csv = DashboardFormat.ExportCsv(table, CultureInfo.GetCultureInfo("fr-FR"));
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 3: VISUALIZATION BLOCKS
            // =========================================================================
            new("blocks", "All 13 Visualization Blocks", "Visualization Blocks",
                "Build responsive 12-column analytical shells with KPIs, gauges, category bars, lists, details, sortable tables, charts, alerts, and feeds.",
                [
                    new("layout-kpi", "Page, Grid, Card, Stat, Stats & SVG Gauge",
                        Paragraphs: ["Every block automatically renders its matched DashboardSkeletonKind (13 shapes: Rows, Stat, Stats, Gauge, Details, Bars, Paired, Funnel, Stack, Table, Feed, Lines, Donut) while its linked IDashboardSource is loading."],
                        LiveDemoKey: "dashboard-kpi",
                        LiveDemoTitle: "LxDashboardStats, LxDashboardGauge & LxDashboardBars",
                        CodeSamples:
                        [
                            new("KpiBlocks.razor", """
                                <LxDashboardPage Dashboard="@_dashboard" Title="Revenue Analytics">
                                    <LxDashboardGrid Menu="true" Freshness="true">
                                        <LxDashboardStats Title="Executive KPIs" Items="@([
                                            new DashboardStatsItem("mrr", "MRR", 148900, DashboardFormatPreset.Compact, Delta: 14.2, Progress: 0.86),
                                            new DashboardStatsItem("churn", "Churn", 1.8, DashboardFormatPreset.Percent, Delta: -0.4, InvertDelta: true)
                                        ])" />
                                        <LxDashboardGauge Title="Q4 ARR Target" Value="86" Min="0" Max="100" Target="90"
                                                          Format="DashboardFormatPreset.Percent" Caption="86% reached" />
                                        <LxDashboardBars Title="Traffic by Platform" Items="@([
                                            new DashboardBarItem("Blazor Web", 14200, Tag: "Web", Meta: "57%"),
                                            new DashboardBarItem(".NET MAUI Mobile", 7850, Tag: "iOS/Android", Meta: "32%")
                                        ])" Format="DashboardFormatPreset.Compact" />
                                    </LxDashboardGrid>
                                </LxDashboardPage>
                                """)
                        ]),
                    new("table-alerts", "Table, Alerts, Feed, Details, List & Chart",
                        Paragraphs: ["LxDashboardTable supports sortable columns (Type: \"text\" | \"number\" | \"bar\" | \"delta\") and built-in CSV export; LxDashboardAlerts displays Error, Warning, Info, and Success severity items."],
                        LiveDemoKey: "dashboard-table-alerts",
                        LiveDemoTitle: "LxDashboardTable, LxDashboardAlerts & CSV Export",
                        CodeSamples:
                        [
                            new("TablesAndAlerts.razor", """
                                <LxDashboardTable Title="Top Regions" Menu="true"
                                                  Columns="@([
                                                      new DashboardTableColumn("region", "Region", Type: "text"),
                                                      new DashboardTableColumn("revenue", "Revenue", Type: "bar"),
                                                      new DashboardTableColumn("delta", "Growth", Type: "delta")
                                                  ])"
                                                  Rows="@_regionRows" />

                                <LxDashboardAlerts Title="Operational Alerts" Items="@([
                                    new DashboardAlertItem(DashboardAlertSeverity.Success, "EU latency within SLA", "P95 at 24ms"),
                                    new DashboardAlertItem(DashboardAlertSeverity.Warning, "Cache eviction spike in us-east", "Raise memory tier")
                                ])" />
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: INTEGRATIONS WITH OTHER LUXSTACK LIBRARIES
            // =========================================================================
            new("integrations-query-table", "Integration: Lux.Query & Lux.Table", "Integrations",
                "Persist dashboard filters & views in the URL with Lux.Query (QueryGroup<T>), compute KPIs from shared DataListSchema<T> filters, and drill down into Lux.Table.",
                [
                    new("url-sync", "1. Lux.Dashboard + Lux.Query (Shareable URL-Synced Dashboards)",
                        Paragraphs: ["Bind your dashboard's active view and region/period filters to a QueryGroup<DashState> from Lux.Query so refreshing the page or hitting Browser Back/Forward restores the exact view and triggers dashboard.RefreshAsync() automatically."],
                        CodeSamples:
                        [
                            new("UrlSyncedDashboard.cs", """
                                using Lux.Dashboard;
                                using Lux.Query;

                                var url = queryState.Group(new DashUrl("overview", "eu"), "RevDash", RendererInfo, prefix: "d")
                                    .Add("view", s => s.View, (s, v) => s with { View = v }, QueryCodecs.String)
                                    .Add("region", s => s.Region, (s, v) => s with { Region = v }, QueryCodecs.String);

                                // Hydrate DashboardController from URL & refresh whenever URL changes
                                dashboard.CurrentView = url.Value.View;
                                regionFilter.SetValues([url.Value.Region]);
                                url.Changed += () => _ = InvokeAsync(async () =>
                                {
                                    dashboard.CurrentView = url.Value.View;
                                    regionFilter.SetValues([url.Value.Region]);
                                    await dashboard.RefreshAsync();
                                });
                                """)
                        ]),
                    new("table-drilldown", "2. Lux.Dashboard + Lux.Table (Deferred Drill-Down Tab)",
                        Paragraphs: ["Mount a full <LxDataList> inside a secondary dashboard view tab and trigger its data load only when ActiveView == \"details\", sharing the exact same region/period filter predicates."])
                ]),
            new("integrations-agent-form", "Integration: Lux.Agent & Lux.Form", "Integrations",
                "Control dashboards conversationally with Lux.Agent (RegisterDashboard) and let users create custom alert rules or saved views using Lux.Form overlays.",
                [
                    new("agent-dashboard", "1. Lux.Dashboard + Lux.Agent (AI Copilot Analytics Control)",
                        Paragraphs: ["Call agent.RegisterDashboard(dashboard) to expose every non-headless DashboardFilterControl and the dashboard root (setView, resetFilters, refresh) to AI copilots via Snapshot/Query/Act."],
                        CodeSamples:
                        [
                            new("AgentDashboard.cs", """
                                using Lux.Agent;
                                using Lux.Dashboard;

                                agent.RegisterDashboard(revenueDashboard);

                                // AI assistant switches to Cohorts view, filters to Europe, and refreshes all waves
                                await agent.ActAsync([
                                    new AgentActStep("Region", "setValue", ["eu"]),
                                    new AgentActStep("revenue", "setView", ["cohorts"]),
                                    new AgentActStep("revenue", "refresh")
                                ]);
                                """)
                        ]),
                    new("form-alerts", "2. Lux.Dashboard + Lux.Form (Configure Alert Thresholds in a Drawer)",
                        CodeSamples:
                        [
                            new("AlertRuleModal.cs", """
                                var outcome = await formService.CreateFormAsync(AlertRuleFormSchema, mode: FormShellMode.Drawer);
                                if (outcome.Submitted)
                                {
                                    await alertsApi.SaveRuleAsync(outcome.Data);
                                    await dashboard.RefreshAsync();
                                }
                                """)
                        ])
                ]),
            new("integrations-display-fullstack", "Integration: Lux.Display, EF Core & MAUI", "Integrations",
                "Combine Lux.Display FilePreviewService for downloadable reports, stream parallel EF Core queries cleanly across waves, and share DashboardController in native .NET MAUI.",
                [
                    new("ef-waves", "1. Parallel EF Core Queries Across Dashboard Waves",
                        Paragraphs: ["Because EF Core DbContext is not thread-safe, inject IDbContextFactory<AppDbContext> into your dashboard loaders so Wave 1 (Essential) and Wave 2 (Background) execute concurrently on separate DbContext instances."],
                        CodeSamples:
                        [
                            new("DbFactoryLoader.cs", """
                                var kpis = dashboard.RegisterResource(new DashboardResource<KpiSummary>(
                                    "kpis",
                                    async ct =>
                                    {
                                        await using var db = await dbFactory.CreateDbContextAsync(ct);
                                        return await db.Orders.AsNoTracking().ComputeKpisAsync(regionFilter.Values, ct);
                                    },
                                    DashboardQueryStage.Essential));
                                """)
                        ]),
                    new("display-csv-preview", "2. Lux.Dashboard + Lux.Display (Preview Exported CSV in Lightbox)",
                        CodeSamples:
                        [
                            new("PreviewCsvExport.cs", """
                                using Lux.Dashboard;
                                using Lux.Display;

                                var csv = DashboardFormat.ExportCsv(table, CultureInfo.CurrentCulture);
                                filePreview.Open(new FilePreviewFile
                                {
                                    Name = DashboardFormat.ResolveFileName("Regional Revenue"),
                                    Mime = "text/csv",
                                    Content = csv
                                });
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 5: PLATFORMS
            // =========================================================================
            MauiAndAotPage("Lux.Dashboard", "Lux.Dashboard.Core", "Lux.Dashboard", "builder.Services.AddLuxDashboard();",
                "await dashboardController.RefreshAsync(); var csv = DashboardFormat.ExportCsv(table);"),

            // =========================================================================
            // GROUP 6: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive analytics playgrounds covering KPI strips & gauges, 3-wave staged loading, sortable tables & CSV export, badged views & multi-select filters, and leap-year comparison formatting.",
                [
                    new("kpi-strip", "1. Executive KPI Strip, Circular SVG Gauge & Category Bars",
                        Paragraphs: ["Combine LxDashboardStats (with InvertDelta on Churn), LxDashboardGauge with target marker, and LxDashboardBars with compact formatting."],
                        LiveDemoKey: "dashboard-kpi",
                        LiveDemoTitle: "LxDashboardStats, LxDashboardGauge & LxDashboardBars",
                        CodeSamples:
                        [
                            new("KpiStrip.razor", """
                                <LxDashboardStats Title="Executive KPIs" Items="@([
                                    new DashboardStatsItem("mrr", "MRR", 148900, DashboardFormatPreset.Compact, Delta: 14.2, Progress: 0.86),
                                    new DashboardStatsItem("churn", "Churn", 1.8, DashboardFormatPreset.Percent, Delta: -0.4, InvertDelta: true)
                                ])" />
                                <LxDashboardGauge Title="Q4 ARR Target" Value="86" Max="100" Target="90" Format="DashboardFormatPreset.Percent" Caption="86% reached" />
                                """)
                        ]),
                    new("staged-waves", "2. 3-Wave Staged Loading (Essential · Background · Deferred)",
                        Paragraphs: ["Trigger RefreshAsync() to observe Wave 1 (Essential) and Wave 2 (Background) complete while Wave 3 (Deferred) stays Idle until ActivateAsync() is clicked."],
                        LiveDemoKey: "dashboard-waves",
                        LiveDemoTitle: "DashboardController · 3-Wave Staged Loading",
                        CodeSamples:
                        [
                            new("StagedWaves.cs", """
                                var controller = new DashboardController("sales");
                                var essential  = controller.RegisterResource(new DashboardResource<int?>("kpis", LoadKpisAsync, DashboardQueryStage.Essential));
                                var background = controller.RegisterResource(new DashboardResource<string>("chart", LoadChartAsync, DashboardQueryStage.Background));
                                var deferred   = controller.RegisterResource(new DashboardResource<string>("audit", LoadAuditAsync, DashboardQueryStage.Deferred));

                                await controller.RefreshAsync(); // Runs Wave 1, then Wave 2
                                await deferred.ActivateAsync();  // Wave 3 runs on demand
                                """)
                        ]),
                    new("tables-alerts", "3. Analytical Table, Severity Alerts & fr-FR BOM CSV Export",
                        Paragraphs: ["Sort LxDashboardTable columns with inline bars and deltas, view severity-coded LxDashboardAlerts, and generate an Excel-safe fr-FR CSV with UTF-8 BOM."],
                        LiveDemoKey: "dashboard-table-alerts",
                        LiveDemoTitle: "LxDashboardTable, LxDashboardAlerts & CSV Export",
                        CodeSamples:
                        [
                            new("OpsOverview.razor", """
                                <LxDashboardTable Title="Top Regions" Columns="@_columns" Rows="@_rows" Menu="true" />
                                <LxDashboardAlerts Title="Operational Alerts" Items="@([
                                    new DashboardAlertItem(DashboardAlertSeverity.Success, "EU cluster latency within SLA", "P95 at 24ms"),
                                    new DashboardAlertItem(DashboardAlertSeverity.Warning, "Cache eviction spike in us-east", "Raise memory tier")
                                ])" />
                                """)
                        ]),
                    new("filters-views", "4. Badged Views, Multi-Select Filters & Reset Detection",
                        Paragraphs: ["Toggle regions and switch badged views on DashboardController — notice how controller.Filtered turns true automatically whenever selection deviates from defaultValues."],
                        LiveDemoKey: "dashboard-filters-views",
                        LiveDemoTitle: "DashboardController · Views, Filters & ResetFilters()",
                        CodeSamples:
                        [
                            new("ViewsAndFilters.cs", """
                                dashboard.AddView("overview", "Overview", badge: "3");
                                var region = dashboard.AddFilter(new DashboardFilterControl("region", "Region", items, defaultValues: ["eu"]) { Multiple = true });
                                region.Toggle("us");      // dashboard.Filtered == true
                                dashboard.ResetFilters(); // dashboard.Filtered == false
                                """)
                        ]),
                    new("format-compare", "5. DashboardFormat Presets & Leap-Year Comparison Ranges",
                        Paragraphs: ["Test Compact, Delta, Ratio, and NaN/null safety alongside DashboardFormat.ResolveComparisonRange on a leap-year February (2024-02-01..2024-02-29)."],
                        LiveDemoKey: "dashboard-format-compare",
                        LiveDemoTitle: "DashboardFormat · Presets & Leap-Year Comparison",
                        CodeSamples:
                        [
                            new("FormatCompare.cs", """
                                var compact = DashboardFormat.Format(2_400_000, DashboardFormatPreset.Compact, CultureInfo.InvariantCulture); // "2.4M"
                                var yearAgo = DashboardFormat.ResolveComparisonRange((leapStart, leapEnd), DashboardComparisonMode.Year);     // 2023-02-01..2023-02-28
                                """)
                        ])
                ]),
            new("api", "API Reference", "Reference",
                "Complete public API reference for Lux.Dashboard.Core and Lux.Dashboard.",
                [],
                ApiTypes: [typeof(LxDashboardPage), typeof(LxDashboardStats), typeof(LxDashboardStat), typeof(LxDashboardGauge), typeof(LxDashboardBars), typeof(LxDashboardTable), typeof(LxDashboardAlerts), typeof(DashboardController), typeof(DashboardFilterControl)])
        ]);
}
