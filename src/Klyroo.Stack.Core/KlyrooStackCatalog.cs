namespace Klyroo.Stack;

/// <summary>
/// Describes one repository/module in the Klyroo ecosystem. <see cref="RepositoryPath"/>, <see cref="SolutionFile"/> and
/// <see cref="SkillPath"/> are relative to the folder that holds the sibling Klyroo clones (for example <c>klyroo-ui/Klyroo.UI.slnx</c>).
/// </summary>
/// <param name="Id">The repository name, such as <c>klyroo-table</c>.</param>
/// <param name="Name">The library name, such as <c>Klyroo.Table</c>.</param>
/// <param name="RepositoryPath">The repository folder, relative to the folder that holds the sibling clones.</param>
/// <param name="SolutionFile">The solution file of the repository, relative to the same folder.</param>
/// <param name="CorePackage">The headless package of the library, or null when it has none.</param>
/// <param name="UiPackage">The Blazor package of the library.</param>
/// <param name="TemplateShortName">The short name of the <c>Klyroo.Templates</c> project template (<c>dotnet new</c>) that best shows the library: <c>klyroo-saas</c>, <c>klyroo-admin</c>, <c>klyroo-headless</c> or <c>klyroo-maui</c>.</param>
/// <param name="SkillPath">The agent skill file of the repository, relative to the folder that holds the sibling clones.</param>
/// <param name="Summary">A one-sentence description of what the library offers.</param>
/// <param name="KeyComponents">The main public types and components of the library, used by <see cref="KlyrooStackCatalog.ResolveComponentModule"/>.</param>
/// <param name="AccentHex">The accent color of the library, as a hex string.</param>
/// <param name="LandingPagePath">The path of the library page on the documentation site, such as <c>/table</c>.</param>
public sealed record KlyrooStackModule(
    string Id,
    string Name,
    string RepositoryPath,
    string SolutionFile,
    string? CorePackage,
    string UiPackage,
    string TemplateShortName,
    string SkillPath,
    string Summary,
    IReadOnlyList<string> KeyComponents,
    string AccentHex = "#ffd23f",
    string LandingPagePath = "");

/// <summary>
/// The registry of the eight libraries of the Klyroo ecosystem. It maps a component or controller name to the library, packages,
/// <c>dotnet new</c> short name and agent skill that own it.
/// </summary>
public static class KlyrooStackCatalog
{
    /// <summary>One entry for each of the eight libraries of the Klyroo ecosystem.</summary>
    public static IReadOnlyList<KlyrooStackModule> Modules { get; } =
    [
        new(
            Id: "klyroo-ui",
            Name: "Klyroo.UI",
            RepositoryPath: "klyroo-ui",
            SolutionFile: "klyroo-ui/Klyroo.UI.slnx",
            CorePackage: "Klyroo.UI.Build.Tasks",
            UiPackage: "Klyroo.UI",
            TemplateShortName: "klyroo-saas",
            SkillPath: "klyroo-ui/skills/klyroo-ui/SKILL.md",
            Summary: "Foundation Design System (100+ Blazor components, Tailwind v4 engine, 12 locales, SSR/WASM/MAUI Hybrid, Images & Server cookies).",
            KeyComponents: ["KlyApp", "KlyButton", "KlyInput", "KlySelect", "KlyModal", "KlyDrawer", "KlyTabs", "KlyCard", "KlyCalendar", "KlyChart", "KlyChat", "KlyEditor"],
            AccentHex: "#10b981",
            LandingPagePath: "/ui"),

        new(
            Id: "klyroo-query",
            Name: "Klyroo.Query",
            RepositoryPath: "klyroo-query",
            SolutionFile: "klyroo-query/Klyroo.Query.slnx",
            CorePackage: "Klyroo.Query.Core",
            UiPackage: "Klyroo.Query",
            TemplateShortName: "klyroo-headless",
            SkillPath: "klyroo-query/skills/klyroo-query/SKILL.md",
            Summary: "Headless NativeAOT query AST, URL search parameter codecs, IQueryable applier, InMemory engine, Blazor QueryState and EF Core async executor.",
            KeyComponents: ["DataListSchema", "DataListRequest", "DataListFilterGroup", "QueryCodec", "QueryState", "EfAsyncQueryExecutor"],
            AccentHex: "#ff4154",
            LandingPagePath: "/query"),

        new(
            Id: "klyroo-table",
            Name: "Klyroo.Table",
            RepositoryPath: "klyroo-table",
            SolutionFile: "klyroo-table/Klyroo.Table.slnx",
            CorePackage: "Klyroo.Table.Core",
            UiPackage: "Klyroo.Table",
            TemplateShortName: "klyroo-admin",
            SkillPath: "klyroo-table/skills/klyroo-table/SKILL.md",
            Summary: "Data lists with table and card-grid layouts, filters, selection, actions, grouping, tree hierarchy and summary rows, built on Klyroo.Query and Klyroo.UI.",
            KeyComponents: ["KlyDataList", "KlyDataListTable", "KlyDataListGrid", "KlyDataListColumn", "KlyDataListToolbar", "KlyDataListFilterBar", "KlyDataListPagination", "DataListTree", "DataListGroupBy", "DataListSummaries"],
            AccentHex: "#00d8ff",
            LandingPagePath: "/table"),

        new(
            Id: "klyroo-form",
            Name: "Klyroo.Form",
            RepositoryPath: "klyroo-form",
            SolutionFile: "klyroo-form/Klyroo.Form.slnx",
            CorePackage: "Klyroo.Form.Core",
            UiPackage: "Klyroo.Form",
            TemplateShortName: "klyroo-saas",
            SkillPath: "klyroo-form/skills/klyroo-form/SKILL.md",
            Summary: "Schema-driven forms: field kinds, reactive conditions, dirty tracking, validation triggers, multi-step wizards with SkipWhen, page sections and overlay forms through FormService.",
            KeyComponents: ["FormSchema", "FormController", "FormService", "KlyFormEngine", "KlyFormPage", "KlyFormProvider"],
            AccentHex: "#f59e0b",
            LandingPagePath: "/form"),

        new(
            Id: "klyroo-dashboard",
            Name: "Klyroo.Dashboard",
            RepositoryPath: "klyroo-dashboard",
            SolutionFile: "klyroo-dashboard/Klyroo.Dashboard.slnx",
            CorePackage: "Klyroo.Dashboard.Core",
            UiPackage: "Klyroo.Dashboard",
            TemplateShortName: "klyroo-admin",
            SkillPath: "klyroo-dashboard/skills/klyroo-dashboard/SKILL.md",
            Summary: "Analytical Dashboard Engine (staged Essential/Background/Deferred queries, filters & presets, views, comparison date ranges, CSV export, KPI & chart cards).",
            KeyComponents: ["DashboardController", "DashboardResource", "DashboardFilterControl", "DashboardFormat", "KlyDashboardPage", "KlyDashboardGrid", "KlyDashboardCard", "KlyDashboardStat", "KlyDashboardStats", "KlyDashboardGauge", "KlyDashboardTable", "KlyDashboardChart", "KlyDashboardBars", "KlyDashboardList", "KlyDashboardAlerts", "KlyDashboardFeed"],
            AccentHex: "#8b5cf6",
            LandingPagePath: "/dashboard"),

        new(
            Id: "klyroo-display",
            Name: "Klyroo.Display",
            RepositoryPath: "klyroo-display",
            SolutionFile: "klyroo-display/Klyroo.Display.slnx",
            CorePackage: "Klyroo.Display.Core",
            UiPackage: "Klyroo.Display",
            TemplateShortName: "klyroo-saas",
            SkillPath: "klyroo-display/skills/klyroo-display/SKILL.md",
            Summary: "Read-only display blocks (19 kinds), audio and video player controls with clip and play-quota rules, and a file preview overlay.",
            KeyComponents: ["KlyDisplay", "KlyAudioPlayer", "KlyVideoPlayer", "KlyFilePreviewProvider", "FilePreviewService"],
            AccentHex: "#ec4899",
            LandingPagePath: "/display"),

        new(
            Id: "klyroo-spreadsheet",
            Name: "Klyroo.Spreadsheet",
            RepositoryPath: "klyroo-spreadsheet",
            SolutionFile: "klyroo-spreadsheet/Klyroo.Spreadsheet.slnx",
            CorePackage: "Klyroo.Spreadsheet.Core",
            UiPackage: "Klyroo.Spreadsheet",
            TemplateShortName: "klyroo-headless",
            SkillPath: "klyroo-spreadsheet/skills/klyroo-spreadsheet/SKILL.md",
            Summary: "Headless workbook/CSV/TSV importer (layout detection, NFD header normalization, 80%-pass column suggestions, unknown value questions, duplicates/upsert, inline edits & wizard).",
            KeyComponents: ["SpreadsheetSchema", "SpreadsheetImporter", "KlySpreadsheetImport"],
            AccentHex: "#14b8a6",
            LandingPagePath: "/spreadsheet"),

        new(
            Id: "klyroo-agent",
            Name: "Klyroo.Agent",
            RepositoryPath: "klyroo-agent",
            SolutionFile: "klyroo-agent/Klyroo.Agent.slnx",
            CorePackage: "Klyroo.Agent.Core",
            UiPackage: "Klyroo.Agent",
            TemplateShortName: "klyroo-headless",
            SkillPath: "klyroo-agent/skills/klyroo-agent/SKILL.md",
            Summary: "Agent bridge that lets AI agents read and act on Klyroo forms, dashboards and imports (snapshot, query, act, observe with secret redaction, UndoStack), plus browser behaviors and WebMCP tools in JavaScript.",
            KeyComponents: ["AgentBridgeService", "UndoStack", "ObservedChange", "AgentComponentSnapshot", "AgentActStep"],
            AccentHex: "#6366f1",
            LandingPagePath: "/agent"),
    ];

    /// <summary>Finds the owning <see cref="KlyrooStackModule"/> for a given component or controller name (case-insensitive).</summary>
    /// <param name="componentOrControllerName">A name from <see cref="KlyrooStackModule.KeyComponents"/>; generic arguments (<c>KlyDataList&lt;Person&gt;</c>) are ignored.</param>
    /// <returns>The module that lists the name, or null when none does.</returns>
    public static KlyrooStackModule? ResolveComponentModule(string componentOrControllerName)
    {
        if (string.IsNullOrWhiteSpace(componentOrControllerName)) return null;
        var trimmed = componentOrControllerName.Split('<')[0].Trim();
        return Modules.FirstOrDefault(m =>
            m.KeyComponents.Any(c => string.Equals(c, trimmed, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>Finds a module by its repository ID (e.g. <c>"klyroo-table"</c>) or package name (e.g. <c>"Klyroo.Table"</c>).</summary>
    /// <param name="idOrName">A repository ID, a library name, or the name of its core or Blazor package (case-insensitive).</param>
    /// <returns>The matching module, or null.</returns>
    public static KlyrooStackModule? GetModule(string idOrName) =>
        Modules.FirstOrDefault(m =>
            string.Equals(m.Id, idOrName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(m.Name, idOrName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(m.CorePackage, idOrName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(m.UiPackage, idOrName, StringComparison.OrdinalIgnoreCase));
}
