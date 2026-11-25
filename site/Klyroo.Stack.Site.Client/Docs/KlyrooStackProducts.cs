using Klyroo.UI;

namespace Klyroo.Stack.Site.Client.Docs;

public sealed record KlyrooProductComponentLink(
    string Name,
    string Description,
    string Href,
    string Layer);

public sealed record KlyrooProductInfo(
    string Slug,
    string Name,
    string ShortName,
    string Tagline,
    string HeroHeadline,
    string HeroAccent,
    string Description,
    string AccentHex,
    KlyColor BadgeColor,
    string RepoName,
    string RepoPath,
    string CorePackage,
    string UiPackage,
    string DocsHref,
    IReadOnlyList<string> Platforms,
    IReadOnlyList<(string Title, string Description, string Tag)> Features,
    string CoreCodeTitle,
    string CoreCode,
    string UiCodeTitle,
    string UiCode,
    IReadOnlyList<KlyrooProductComponentLink> Components);

public static class KlyrooStackProducts
{
    public static IReadOnlyList<KlyrooProductInfo> All { get; } =
    [
        new(
            Slug: "ui",
            Name: "Klyroo.UI",
            ShortName: "UI",
            Tagline: "125+ zero-roundtrip primitives & composable DOM behaviors",
            HeroHeadline: "Accessible primitives",
            HeroAccent: "zero SignalR lag.",
            Description: "Complete design system for Blazor (Server, WebAssembly, Auto, Static SSR) and .NET MAUI Blazor Hybrid. Menus, dialogs, popovers, and tabs answer instantaneously at 60fps in the browser via native Web Components and composable behaviors (dialog, popup, rovingTabindex, formAssociated).",
            AccentHex: "#10b981",
            BadgeColor: KlyColor.Success,
            RepoName: "klyroo-ui",
            RepoPath: "klyroo-ui",
            CorePackage: "Klyroo.UI.Build.Tasks",
            UiPackage: "Klyroo.UI",
            DocsHref: "/ui/docs",
            Platforms: ["Blazor Server", "Blazor WASM", "Blazor Auto / SSR", ".NET MAUI Hybrid (iOS, Android, Mac, Win)", "Android TV (D-Pad)"],
            Features:
            [
                ("Zero-Roundtrip Popovers & Dialogs", "Built on native HTML Popover and <dialog> APIs with light-dismiss, Escape handling, and top-layer stacking managed directly by the browser.", "DOM Native"),
                ("Composable Behaviors (ui-sdk.dev)", "Standalone JS primitives: dialog(), popup(), rovingTabindex(), formAssociated(), and dismissable() with zero external dependencies.", "Behaviors"),
                ("Tailwind v4 & Design Tokens", "A single stylesheet driven by semantic CSS variables (--klyroo-primary, --klyroo-bg, --klyroo-border), 12 built-in locales, and instant dark mode.", "Tailwind v4"),
                ("Mobile 44px Touch & TV D-Pad", "44px touch hit targets calibrated for iOS/Android, safe area insets, and directional D-pad focus rings for Android TV.", "Multiplatform")
            ],
            CoreCodeTitle: "js/behaviors.js · Composable DOM Behaviors",
            CoreCode: """
                import { dialog, rovingTabindex } from './behaviors.js';

                // Focus trap, Escape dismiss, aria-modal & return focus
                const modalCtrl = dialog(dialogElement, {
                  closeOnBackdrop: true,
                  onClose: () => dialogElement.dispatchEvent(new CustomEvent('kly-close'))
                });

                // Roving tabindex across toolbar or tabs (Arrow keys, Home/End)
                const tabsCtrl = rovingTabindex(tabListElement, {
                  selector: '[role="tab"]',
                  orientation: 'horizontal'
                });
                """,
            UiCodeTitle: "App.razor · Blazor Web & MAUI Hybrid",
            UiCode: """
                <KlyModal Title="Deploy to production" Description="Zero round-trip open & close.">
                    <ChildContent>
                        <KlyButton Label="Deploy now" Color="KlyColor.Primary" />
                    </ChildContent>
                    <Body>
                        <KlyFormField Label="Environment">
                            <KlySelect Items="@Environments" @bind-Value="_env" />
                        </KlyFormField>
                    </Body>
                    <Footer>
                        <KlyButton Label="Confirm" data-kly-close />
                    </Footer>
                </KlyModal>
                """,
            Components:
            [
                new("KlyButton", "Seven colors, six variants, loading and link states.", "/components/button", "UI"),
                new("KlyModal", "Native <dialog> with focus trap, inert background and Escape.", "/components/modal", "UI"),
                new("KlySelect", "Keyboard typeahead and hidden input for native forms.", "/components/select", "UI"),
                new("KlyTabs", "Arrow keys and automatic activation without SignalR round-trip.", "/components/tabs", "UI"),
                new("KlyCommandPalette", "Full keyboard command launcher with groups and shortcuts.", "/components/command-palette", "UI"),
                new("KlyChatMessages", "Streaming AI chat bubbles, reasoning and tool calls.", "/components/chat-messages", "UI")
            ]),

        new(
            Slug: "query",
            Name: "Klyroo.Query",
            ShortName: "Query",
            Tagline: "Headless NativeAOT Query AST, URL codecs, IQueryable applier, EF Core & QueryState",
            HeroHeadline: "Headless query engine",
            HeroAccent: "& reactive URL state.",
            Description: "Strongly typed, 100% NativeAOT query engine: composable filter AST (DataListSchema, DataListRequest, DataListFilterGroup), reversible URL query codecs, InMemory & IQueryable evaluators, Entity Framework Core async SQL executor (EfAsyncQueryExecutor), and reactive QueryState for Blazor and MAUI Hybrid.",
            AccentHex: "#ff4154",
            BadgeColor: KlyColor.Error,
            RepoName: "klyroo-query",
            RepoPath: "klyroo-query",
            CorePackage: "Klyroo.Query.Core",
            UiPackage: "Klyroo.Query",
            DocsHref: "/query/docs",
            Platforms: ["Headless C# / NativeAOT", ".NET MAUI Native C#", "Blazor Server / WASM / Auto", ".NET MAUI Blazor Hybrid", "Entity Framework Core"],
            Features:
            [
                ("Typed Schema & Filter AST", "Declare searchable, sortable, and filterable fields with DataListSchema.For<T>() with zero magic strings and full compile-time safety.", "Typed AST"),
                ("InMemory, IQueryable & EF Core", "Execute the exact same DataListRequest in memory (ExecuteInMemory), over LINQ IQueryable, or translated to async SQL via EfAsyncQueryExecutor.", "Any Backend"),
                ("URL Query Codecs & QueryState", "Reversible serialization of filters, sorts, and paging into the query string with browser history control and mobile WebView support.", "URL Sync"),
                ("100% Headless & NativeAOT", "Klyroo.Query.Core has zero UI dependencies: compile it in NativeAOT APIs, background workers, CLI tools, or native .NET MAUI ViewModels.", "NativeAOT")
            ],
            CoreCodeTitle: "Klyroo.Query.Core · Typed Schema & InMemory / IQueryable",
            CoreCode: """
                using Klyroo.Query;

                var schema = DataListSchema.For<Product>(s => s
                    .RowKey(p => p.Id)
                    .Search(p => p.Name)
                    .Field(p => p.Price, f => f.Key("price").Sortable()));

                var request = new DataListRequest(
                    new DataListPaging.Offset(0, 25),
                    [new DataListSort("price", Descending: true)],
                    DataListFilterGroup.Empty,
                    new DataListSearch("Monitor"));

                var result = schema.ExecuteInMemory(products, request);
                """,
            UiCodeTitle: "Klyroo.Query · Reactive URL QueryState (Blazor & MAUI)",
            UiCode: """
                @inject QueryState Query

                @code {
                    // Two-way synchronized URL parameter (?page=3&sort=price:desc)
                    private void GoToPage(int page) => Query.Set("page", page.ToString());
                    private string CurrentPage => Query.Get("page") ?? "1";
                }
                """,
            Components:
            [
                new("DataListSchema", "Fluent typed schema builder for searchable, sortable and filterable fields.", "/query/docs", "Core"),
                new("DataListRequest", "Immutable paging (Offset/Cursor), sorting, filtering & search AST.", "/query/docs", "Core"),
                new("QueryCodecs", "Reversible URL search parameter serialization for complex filter trees.", "/query/docs", "Core"),
                new("EfAsyncQueryExecutor", "Translates DataListRequest directly to async SQL via Entity Framework Core.", "/query/docs", "Core"),
                new("QueryState", "Reactive URL state manager for Blazor Web and .NET MAUI BlazorWebView.", "/query/docs", "UI")
            ]),

        new(
            Slug: "table",
            Name: "Klyroo.Table",
            ShortName: "Table",
            Tagline: "Headless tree hierarchy engine & high-density enterprise KlyDataList",
            HeroHeadline: "Supercharged tables",
            HeroAccent: "& headless datagrids.",
            Description: "Headless-powered data tables and card grids for .NET: NativeAOT tree hierarchy (DataListTree<TItem>), visual grouping, footer aggregations, 2px loading progress line, automatic Sort/Hide HeaderMenu, 60fps virtualization, and responsive mobile summary rows.",
            AccentHex: "#00d8ff",
            BadgeColor: KlyColor.Info,
            RepoName: "klyroo-table",
            RepoPath: "klyroo-table",
            CorePackage: "Klyroo.Table.Core",
            UiPackage: "Klyroo.Table",
            DocsHref: "/table/docs",
            Platforms: ["Headless C# / NativeAOT", "Blazor Server / WASM / Auto", ".NET MAUI Blazor Hybrid", "Entity Framework Core"],
            Features:
            [
                ("Headless Tree Hierarchy (NativeAOT)", "Klyroo.Table.Core builds and flattens hierarchical trees (DataListTree<TItem>) with depth tracking and continuation rails without any UI dependency.", "Headless Tree"),
                ("Built-In UX Polish", "2px indeterminate progress line during reloads, automatic Sort Asc/Desc/Hide HeaderMenu, adaptive skeletons, and first-use EmptyState CTAs.", "UX Polish"),
                ("Grouping, Summaries & Pinning", "Visual row grouping (DataListGroupBy), footer Sum/Avg/Min/Max aggregations, column resizing, and left/right sticky column pinning.", "Enterprise Grid"),
                ("Responsive Summary Rows on Phone", "Automatically switches to scannable summary rows (DataListRowSummary) below 768px on iOS and Android phones.", "Mobile Ready")
            ],
            CoreCodeTitle: "Klyroo.Table.Core · Headless DataListTree<T> (NativeAOT)",
            CoreCode: """
                using Klyroo.Table;

                var tree = new DataListTree<Employee>
                {
                    Children = e => e.Reports,
                    DefaultExpandedDepth = 1
                };

                var (roots, allNodes) = tree.BuildTree(employees, schema);
                // Every node carries Depth, Parent, IsLast & AncestorContinuations
                """,
            UiCodeTitle: "Klyroo.Table · KlyDataList & KlyDataListTable",
            UiCode: """
                <KlyDataList TItem="Employee" Schema="@_schema" Items="@_employees" UrlSync="false">
                    <KlyDataListTable TItem="Employee"
                                     HeaderMenu="true"
                                     Summaries="@(new DataListSummaries<Employee>())">
                        <KlyDataListColumn TItem="Employee" Field="name" Title="Name" />
                        <KlyDataListColumn TItem="Employee" Field="salary" Title="Salary"
                                          Summary="DataListSummaryKind.Sum" />
                    </KlyDataListTable>
                </KlyDataList>
                """,
            Components:
            [
                new("DataListTree<TItem>", "Pure headless NativeAOT hierarchy builder with rail continuations & expansion.", "/table/docs/tree", "Core"),
                new("KlyDataList<TItem>", "Full-featured enterprise datagrid with table, grid, filter bar & pagination.", "/table/docs/data-list", "UI"),
                new("KlyDataListTable<TItem>", "High-density table with HeaderMenu, summaries, tree view and column resizing.", "/table/docs/columns", "UI"),
                new("DataListRowSummary<TItem>", "Automatic mobile summary-row presentation below the md breakpoint.", "/table/docs/mobile-summary", "UI")
            ]),

        new(
            Slug: "form",
            Name: "Klyroo.Form",
            ShortName: "Form",
            Tagline: "31-kind schema-driven form engine, reactive conditions, dirty diffing & wizards",
            HeroHeadline: "Schema-driven forms",
            HeroAccent: "without boilerplate.",
            Description: "Declarative, 100% NativeAOT form engine in Klyroo.Form.Core: 31 field kinds (FormFieldKind), reactive conditional visibility, dirty/touched diffing, output transforms, multi-step wizards with SkipWhen, sectioned FormPage layouts, and programmatic modal/drawer overlays via FormService.",
            AccentHex: "#f59e0b",
            BadgeColor: KlyColor.Warning,
            RepoName: "klyroo-form",
            RepoPath: "klyroo-form",
            CorePackage: "Klyroo.Form.Core",
            UiPackage: "Klyroo.Form",
            DocsHref: "/form/docs",
            Platforms: ["Headless C# / NativeAOT", ".NET MAUI Native C#", "Blazor Server / WASM / Auto", ".NET MAUI Blazor Hybrid"],
            Features:
            [
                ("31 Declarative Field Kinds", "Text, Password (with requirement checklist), Number, Select, AutoComplete, Checkbox, Switch, Slider, Repeatable Arrays, and Tabs driven by FormSchema.", "31 Field Kinds"),
                ("Reactive Conditions & Transforms", "Evaluates Condition = f => ... live, automatically skips hidden fields during validation and output building, and applies TransformOutput on submit.", "Reactive Engine"),
                ("Multi-Step Wizards with SkipWhen", "Split complex flows into FormStepSchema with conditional SkipWhen branching, per-step validation, and automatic dirty re-baselining after submit.", "Wizard Flow"),
                ("FormPage & Overlay FormService", "Sectioned pages with completion indicators (KlyFormPage) and programmatic modal/drawer form hosts via FormService + KlyFormProvider.", "Overlay Host")
            ],
            CoreCodeTitle: "Klyroo.Form.Core · Headless FormSchema & FormController",
            CoreCode: """

                var schema = new FormSchema
                {
                    FormKey = "profile",
                    Title = "User Profile",
                    Fields =
                    [
                        new FormFieldSchema { Key = "name", Label = "Full Name", Required = true, TransformOutput = v => v?.ToString()?.Trim() },
                        new FormFieldSchema { Key = "age", Label = "Age", Kind = FormFieldKind.Number, DefaultValue = 25.0 },
                        new FormFieldSchema { Key = "company", Label = "Company", Required = true, Condition = f => f.GetValue<double>("age") >= 21 }
                    ]
                };

                var form = new FormController(schema, new Dictionary<string, object?> { ["name"] = "Alice" });
                var ok = await form.SubmitAsync();
                """,
            UiCodeTitle: "Klyroo.Form · KlyFormEngine & KlyFormPage",
            UiCode: """
                <KlyFormEngine Form="@_form"
                              SubmitLabel="Save profile"
                              ShowReset="true"
                              OnSubmit="@HandleSubmit" />
                """,
            Components:
            [
                new("FormSchema", "Declarative schema with 31 field kinds, steps, sections, rules & transforms.", "/form/docs", "Core"),
                new("FormController", "Headless NativeAOT state machine for values, errors, dirty paths & steps.", "/form/docs", "Core"),
                new("FormService", "Programmatic overlay service to open forms in modals or drawers.", "/form/docs", "Core"),
                new("KlyFormEngine", "Automatic Blazor/MAUI renderer for FormSchema & multi-step wizards.", "/form/docs", "UI"),
                new("KlyFormPage", "Sectioned form page layout with live completion & dirty status.", "/form/docs", "UI")
            ]),

        new(
            Slug: "dashboard",
            Name: "Klyroo.Dashboard",
            ShortName: "Dashboard",
            Tagline: "Staged 3-wave analytical queries, global filters, comparison ranges & 13 KPI blocks",
            HeroHeadline: "Analytical dashboards",
            HeroAccent: "staged for speed.",
            Description: "Analytical dashboard orchestrator with 3-wave staged query loading (Essential -> Background -> Deferred), reactive global filter toolbar and views, leap-year-safe comparison date ranges, RFC 4180 UTF-8 BOM CSV export, and 13 ready-to-use KPI & visualization blocks.",
            AccentHex: "#8b5cf6",
            BadgeColor: KlyColor.Primary,
            RepoName: "klyroo-dashboard",
            RepoPath: "klyroo-dashboard",
            CorePackage: "Klyroo.Dashboard.Core",
            UiPackage: "Klyroo.Dashboard",
            DocsHref: "/dashboard/docs",
            Platforms: ["Headless C# / NativeAOT", "Blazor Server / WASM / Auto", ".NET MAUI Blazor Hybrid (Tablet, Desktop, Mobile)"],
            Features:
            [
                ("3-Wave Staged Query Orchestration", "DashboardController runs Essential KPIs first, Background charts second, and keeps Deferred resources idle until explicitly requested.", "Staged Waves"),
                ("Global Filters, Presets & Views", "Synchronized filters (DashboardFilterControl) with one-click Reset detection and tabbed DashboardViewMeta views with live count badges.", "Reactive Filters"),
                ("13 Analytical Visualization Blocks", "KlyDashboardPage, Grid, Card, Stat, Stats, SVG Gauge, Details, Bars, List, sortable Table with CSV export, Alerts, Feed, and Chart.", "13 Widgets"),
                ("Smart Formatters & CSV Export", "DashboardFormat formats Compact (12.6K), Delta (+14.2%), Points (+2.1 pts), Ratio (57%), leap-year date ranges, and locale-aware UTF-8 BOM CSV.", "Analytics Core")
            ],
            CoreCodeTitle: "Klyroo.Dashboard.Core · Staged DashboardController",
            CoreCode: """

                var controller = new DashboardController("analytics");
                controller.AddView("overview", "Overview", badge: "3");
                controller.AddFilter(new DashboardFilterControl("year", "Year",
                    items: [new("2025", "2025"), new("2026", "2026")], defaultValues: ["2026"]));

                var kpis = controller.RegisterResource(new DashboardResource<int>(
                    "summary", LoadSummaryAsync, DashboardQueryStage.Essential));
                var chart = controller.RegisterResource(new DashboardResource<double[]>(
                    "trend", LoadTrendAsync, DashboardQueryStage.Background));

                await controller.RefreshAsync(); // Runs Essential -> Background
                """,
            UiCodeTitle: "Klyroo.Dashboard · KlyDashboardPage & KPI Widgets",
            UiCode: """
                <KlyDashboardPage Dashboard="@_dashboard" Title="Revenue Analytics">
                    <KlyDashboardGrid Menu="true" Freshness="true">
                        <KlyDashboardStats Title="Key Metrics" Items="@_statItems" />
                        <KlyDashboardGauge Title="ARR Goal" Value="84" Max="100" Target="90"
                                          Format="DashboardFormatPreset.Percent" />
                        <KlyDashboardBars Title="Channels" Items="@_barItems"
                                         Format="DashboardFormatPreset.Compact" />
                    </KlyDashboardGrid>
                </KlyDashboardPage>
                """,
            Components:
            [
                new("DashboardController", "Staged 3-wave query coordinator (Essential/Background/Deferred), filters & views.", "/dashboard/docs", "Core"),
                new("DashboardFormat", "Compact/Delta/Points/Ratio formatters, leap-year ranges & BOM CSV exporter.", "/dashboard/docs", "Core"),
                new("KlyDashboardPage", "Full analytical dashboard shell with filter toolbar, views & refresh state.", "/dashboard/docs", "UI"),
                new("KlyDashboardStats", "Multi-KPI summary strip with delta badges, progress and status pills.", "/dashboard/docs", "UI"),
                new("KlyDashboardGauge", "SVG circular gauge with target marker and formatted center value.", "/dashboard/docs", "UI"),
                new("KlyDashboardTable", "Sortable analytical table block with inline bars, deltas & CSV export.", "/dashboard/docs", "UI")
            ]),

        new(
            Slug: "display",
            Name: "Klyroo.Display",
            ShortName: "Display",
            Tagline: "19 read-only display blocks, constrained audio/video players & modal file preview",
            HeroHeadline: "Rich display blocks",
            HeroAccent: "& constrained media.",
            Description: "19 read-only presentation blocks inside <KlyDisplay> (Heading, Alert, Facts, Stat with sparkline, Breakdown, Meter with thresholds, Diff, masked Code/Secret, Timeline, Accordion…), constrained Audio/Video players with clip Start/End bounds and MaxPlays quotas, plus a modal multi-format file previewer.",
            AccentHex: "#ec4899",
            BadgeColor: KlyColor.Secondary,
            RepoName: "klyroo-display",
            RepoPath: "klyroo-display",
            CorePackage: "Klyroo.Display.Core",
            UiPackage: "Klyroo.Display",
            DocsHref: "/display/docs",
            Platforms: ["Headless C# / NativeAOT", "Blazor Server / WASM / Auto", ".NET MAUI Blazor Hybrid"],
            Features:
            [
                ("19 Read-Only Display Blocks", "Heading, Text, expandable Alert, Callout, copyable Facts, Stat with sparkline, Breakdown, threshold Meter, Diff before/after, Timeline, and Secret Code.", "19 Blocks"),
                ("Constrained Audio & Video Players", "Supports Start/End clip slicing, MaxPlays play quotas (ideal for exams and language assessments), Seekable=false locking, and subtitle tracks.", "Media Quotas"),
                ("Multi-Format FilePreviewService", "Headless MIME/extension detection in .Core, built-in RFC 4180 CSV parser, and zoomable modal lightbox for images, PDFs, audio, video, and CSV.", "File Lightbox"),
                ("Secret Masking & One-Click Reveal", "DisplayKind.Code with Variant=\"secret\" automatically masks API keys (••••••••3210) with Reveal/Hide toggle and clipboard copy.", "Security UX")
            ],
            CoreCodeTitle: "Klyroo.Display.Core · FilePreviewService & CSV Parser",
            CoreCode: """

                // Headless MIME/extension kind detection & modal preview trigger
                filePreview.Open(
                    new FilePreviewItem("spec.pdf", "/files/spec.pdf"),
                    new FilePreviewItem("metrics.csv", "/files/metrics.csv"));

                // Built-in RFC 4180 CSV parser for instant preview tables
                var csv = FilePreviewService.ParseCsv("Region,MRR\nEurope,148900\nUS,192400");
                """,
            UiCodeTitle: "Klyroo.Display · KlyDisplay & KlyAudioPlayer",
            UiCode: """
                <KlyDisplay Kind="DisplayKind.Stat" Title="MRR" Value="€148,900"
                           Delta="+14.2%" Trend="@([102, 118, 134, 149])" />

                <KlyDisplay Kind="DisplayKind.Code" Variant="secret"
                           Value="sk_live_9876543210" VisibleChars="4" Copy="true" />

                <KlyAudioPlayer Src="/media/listening.mp3" Title="Listening Extract #1"
                               Start="15" End="45" MaxPlays="2" Seekable="false" />
                """,
            Components:
            [
                new("FilePreviewService", "Headless MIME detector, RFC 4180 CSV parser and lightbox queue state.", "/display/docs", "Core"),
                new("KlyDisplay", "19-kind read-only block component (Stat, Facts, Meter, Diff, Code/Secret, Timeline).", "/display/docs", "UI"),
                new("KlyAudioPlayer", "Constrained audio player with Start/End clip bounds, MaxPlays & Seekable guard.", "/display/docs", "UI"),
                new("KlyVideoPlayer", "Constrained video player with tracks, clip bounds and play-limit locking.", "/display/docs", "UI"),
                new("KlyFilePreviewProvider", "Modal lightbox host for images, PDFs, audio, video and parsed CSV files.", "/display/docs", "UI")
            ]),

        new(
            Slug: "spreadsheet",
            Name: "Klyroo.Spreadsheet",
            ShortName: "Spreadsheet",
            Tagline: "Headless CSV/TSV importer, 20-row header detection, 80%-pass suggestions & batch retry",
            HeroHeadline: "Intelligent CSV import",
            HeroAccent: "& row reconciliation.",
            Description: "100% NativeAOT spreadsheet import engine in Klyroo.Spreadsheet.Core: 20-row heuristic header layout detection, NFD accent-insensitive normalization, ≥80% pass-rate column suggestions, unknown value resolution questions, duplicate & existing record upsert policies, inline cell edits, and batched submission with rejected-row retry.",
            AccentHex: "#14b8a6",
            BadgeColor: KlyColor.Success,
            RepoName: "klyroo-spreadsheet",
            RepoPath: "klyroo-spreadsheet",
            CorePackage: "Klyroo.Spreadsheet.Core",
            UiPackage: "Klyroo.Spreadsheet",
            DocsHref: "/spreadsheet/docs",
            Platforms: ["Headless C# / NativeAOT", ".NET MAUI Native C#", "Blazor Server / WASM / Auto", ".NET MAUI Blazor Hybrid"],
            Features:
            [
                ("20-Row Heuristic Header Detection", "Automatically skips export banners and title rows across the first 20 rows of a CSV/TSV or clipboard paste to locate the real header row.", "Auto Header"),
                ("NFD Normalization & 80%-Pass Matching", "Normalizes headers (accents, *, (required)) and suggests unmapped columns only when ≥80% of sample values pass type parsing and validation rules.", "Smart Mapping"),
                ("Unknown Value Questions & Upsert", "Surfaces unknown select values as explicit mapping questions and resolves intra-file duplicates or updates against existing database records.", "Reconciliation"),
                ("Batch Submit & Retry Rejected Rows", "Submits validated rows in configurable batches and isolates server-rejected rows for inline correction and targeted retry.", "Batch Retry")
            ],
            CoreCodeTitle: "Klyroo.Spreadsheet.Core · Headless SpreadsheetImporter",
            CoreCode: """

                var schema = new SpreadsheetSchema
                {
                    Key = "rates",
                    Columns =
                    [
                        new SpreadsheetColumnSchema { Key = "code", Label = "Code", Required = true },
                        new SpreadsheetColumnSchema { Key = "price", Label = "Unit Price", Kind = SpreadsheetColumnKind.Number, DecimalSeparator = ',', Rules = [SpreadsheetRule.Min(0)] },
                        new SpreadsheetColumnSchema { Key = "currency", Label = "Currency", Kind = SpreadsheetColumnKind.Select, Options = [new("EUR", "Euro", ["€", "EUR"]), new("USD", "US Dollar", ["$", "USD"])] }
                    ]
                };

                var importer = new SpreadsheetImporter(schema);
                importer.LoadText(csvText, "rates.csv"); // Auto-detects header row & 80%-pass suggestions
                """,
            UiCodeTitle: "Klyroo.Spreadsheet · KlySpreadsheetImport Wizard",
            UiCode: """
                <KlySpreadsheetImport Importer="@_importer"
                                     Title="Import Product Rates"
                                     Wizard="true" />
                """,
            Components:
            [
                new("SpreadsheetSchema", "Column definitions, rules, options, duplicate policies & existing record lookup.", "/spreadsheet/docs", "Core"),
                new("SpreadsheetImporter", "Headless NativeAOT parser, layout detector, column suggester & batch runner.", "/spreadsheet/docs", "Core"),
                new("KlySpreadsheetImport", "Interactive 5-step Blazor/MAUI import wizard with inline cell editing.", "/spreadsheet/docs", "UI")
            ]),

        new(
            Slug: "agent",
            Name: "Klyroo.Agent",
            ShortName: "Agent",
            Tagline: "Semantic AI Agent Bridge (snapshot/query/act), WebMCP tools, observe stream & UndoStack",
            HeroHeadline: "AI-ready interfaces",
            HeroAccent: "& semantic WebMCP.",
            Description: "Inspired by ui-sdk.dev: makes Klyroo.Form, Klyroo.Dashboard, and Klyroo.Spreadsheet immediately inspectable and operable by AI agents without brittle CSS selectors. Provides semantic Snapshot/Query/Act, global permission policies, automatic [redacted] secret masking, WebMCP browser assets, and UndoStack<T>.",
            AccentHex: "#6366f1",
            BadgeColor: KlyColor.Info,
            RepoName: "klyroo-agent",
            RepoPath: "klyroo-agent",
            CorePackage: "Klyroo.Agent.Core",
            UiPackage: "Klyroo.Agent",
            DocsHref: "/agent/docs",
            Platforms: ["Headless C# / NativeAOT", "Blazor Server / WASM / Auto", ".NET MAUI Blazor Hybrid", "Browser WebMCP (navigator.modelContext)"],
            Features:
            [
                ("Semantic Snapshot, Query & Act", "Exposes up to 100 controls by stable ref (c1, c2…), queries select/filter choices without opening popups, and runs semantic actions (setValue, toggle, submit).", "Semantic Bridge"),
                ("Permission Gate & Confirmation Policy", "Global AllowPolicy or per-call gate with ConfirmRequired flags to approve or block consequential actions before execution.", "Policy Guard"),
                ("Observe Stream & Secret Redaction", "Unified ObservedChange stream (Agent/User source) where Password and OneTimeCode fields are always redacted as [redacted].", "Privacy First"),
                ("WebMCP JS Assets & UndoStack<T>", "Ships _content/Klyroo.Agent/klyroo-agent.js + klyroo-behaviors.js for WebMCP and a bounded generic UndoStack<T> in Klyroo.Agent.Core (NativeAOT).", "WebMCP + Undo")
            ],
            CoreCodeTitle: "Klyroo.Agent · AgentBridgeService & UndoStack<T>",
            CoreCode: """

                agent.RegisterForm(form, confirmSubmit: true);
                agent.RegisterDashboard(dashboard);

                // Inspect choices accent-insensitively without opening any popup
                var choices = agent.Query(new AgentQueryRequest { Within = "region", Text = "Europe" });

                // Execute semantic steps with policy checks & automatic secret redaction
                var result = await agent.ActAsync([
                    new AgentActStep("project name", "setValue", ["Atlas"]),
                    new AgentActStep("region", "setValue", ["eu-west"])
                ]);
                """,
            UiCodeTitle: "wwwroot/klyroo-agent.js · Browser WebMCP Bridge",
            UiCode: """
                // Served automatically via _content/Klyroo.Agent/klyroo-agent.js
                const snap = window.Klyroo.agent.snapshot();
                const res  = await window.Klyroo.agent.act([
                  { ref: 'Project name', action: 'setValue', args: ['Atlas'] }
                ]);

                // Privacy-preserving observation stream (passwords auto-masked as "[redacted]")
                const stop = window.Klyroo.agent.observe(evt => console.log(evt));
                """,
            Components:
            [
                new("AgentComponentSnapshot", "Compact NativeAOT semantic snapshot model with state, actions & flags.", "/agent/docs", "Core"),
                new("UndoStack<T>", "Generic bounded Undo/Redo stack with labels and state notifications.", "/agent/docs", "Core"),
                new("AgentBridgeService", "Connects FormController, DashboardController & SpreadsheetImporter to AI agents.", "/agent/docs", "UI"),
                new("klyroo-agent.js & klyroo-behaviors.js", "Static web assets for WebMCP (navigator.modelContext) and composable DOM behaviors.", "/agent/docs", "UI")
            ])
    ];

    public static KlyrooProductInfo? Find(string? slug) =>
        string.IsNullOrWhiteSpace(slug)
            ? null
            : All.FirstOrDefault(p => string.Equals(p.Slug, slug.Trim(), StringComparison.OrdinalIgnoreCase));

    /// <summary>Resolves which Klyroo library owns the current route path, or null on the Klyroo Hub / Templates.</summary>
    public static KlyrooProductInfo? ResolveFromPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        var clean = path.Split('?', '#')[0].Trim('/');
        if (clean.Length == 0 || clean is "stack") return null;
        if (clean == "templates") return Find("ui");

        var segs = clean.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segs.Length == 0) return null;

        if (segs[0].Equals("stack", StringComparison.OrdinalIgnoreCase) && segs.Length >= 2)
        {
            return Find(segs[1]);
        }
        if (segs[0].Equals("docs", StringComparison.OrdinalIgnoreCase))
        {
            if (segs.Length >= 2 && segs[1].StartsWith("klyroo-", StringComparison.OrdinalIgnoreCase))
            {
                return Find(segs[1]["klyroo-".Length..]);
            }
            return Find("ui");
        }
        if (segs[0].Equals("components", StringComparison.OrdinalIgnoreCase)
            || (segs.Length >= 2 && segs[1].Equals("components", StringComparison.OrdinalIgnoreCase)))
        {
            return Find("ui");
        }
        return Find(segs[0]);
    }
}
