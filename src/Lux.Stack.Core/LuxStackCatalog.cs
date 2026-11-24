namespace Lux.Stack;

/// <summary>Describes one repository/module in the LuxStack ecosystem.</summary>
public sealed record LuxStackModule(
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
/// Central registry and router for the 8 repositories of the LuxStack ecosystem.
/// Resolves any component, headless controller, landing page, template, or skill to its owning repository and package.
/// </summary>
public static class LuxStackCatalog
{
    public static IReadOnlyList<LuxStackModule> Modules { get; } =
    [
        new(
            Id: "lux-ui",
            Name: "Lux.UI",
            RepositoryPath: "/home/gpoumarat/lux-ui",
            SolutionFile: "/home/gpoumarat/lux-ui/Lux.UI.slnx",
            CorePackage: "Lux.UI.Build.Tasks",
            UiPackage: "Lux.UI",
            TemplateShortName: "lux-starter",
            SkillPath: "/home/gpoumarat/lux-ui/skills/lux-ui/SKILL.md",
            Summary: "Foundation Design System (100+ Blazor components, Tailwind v4 engine, 12 locales, SSR/WASM/MAUI Hybrid, Images & Server cookies).",
            KeyComponents: ["LxApp", "LxButton", "LxInput", "LxSelect", "LxModal", "LxDrawer", "LxTabs", "LxCard", "LxCalendar", "LxChart", "LxChat", "LxEditor"],
            AccentHex: "#10b981",
            LandingPagePath: "/ui"),

        new(
            Id: "lux-query",
            Name: "Lux.Query",
            RepositoryPath: "/home/gpoumarat/lux-query",
            SolutionFile: "/home/gpoumarat/lux-query/Lux.Query.slnx",
            CorePackage: "Lux.Query.Core",
            UiPackage: "Lux.Query",
            TemplateShortName: "lux-query",
            SkillPath: "/home/gpoumarat/lux-query/skills/lux-query/SKILL.md",
            Summary: "Headless NativeAOT query AST, URL search parameter codecs, IQueryable applier, InMemory engine, Blazor QueryState and EF Core async executor.",
            KeyComponents: ["DataListSchema", "DataListRequest", "DataListFilterGroup", "QueryCodec", "QueryState", "EfAsyncQueryExecutor"],
            AccentHex: "#ff4154",
            LandingPagePath: "/query"),

        new(
            Id: "lux-table",
            Name: "Lux.Table",
            RepositoryPath: "/home/gpoumarat/lux-table",
            SolutionFile: "/home/gpoumarat/lux-table/Lux.Table.slnx",
            CorePackage: "Lux.Table.Core",
            UiPackage: "Lux.Table",
            TemplateShortName: "lux-table-admin",
            SkillPath: "/home/gpoumarat/lux-table/skills/lux-table/SKILL.md",
            Summary: "Full-featured DataList (Table, Grid, Virtualization, Pinning, Resizing, Grouping, Tree hierarchy, Summary rows, Footer aggregations, Saved Views, Export).",
            KeyComponents: ["LxDataList", "LxDataListTable", "LxDataListGrid", "LxDataListColumn", "LxDataListToolbar", "LxDataListFilterBar", "LxDataListPagination", "DataListTree", "DataListGroupBy", "DataListSummaries"],
            AccentHex: "#00d8ff",
            LandingPagePath: "/table"),

        new(
            Id: "lux-form",
            Name: "Lux.Form",
            RepositoryPath: "/home/gpoumarat/lux-form",
            SolutionFile: "/home/gpoumarat/lux-form/Lux.Form.slnx",
            CorePackage: "Lux.Form.Core",
            UiPackage: "Lux.Form",
            TemplateShortName: "lux-form-wizard",
            SkillPath: "/home/gpoumarat/lux-form/skills/lux-form/SKILL.md",
            Summary: "Schema-driven Form Engine (31 field kinds, reactive conditions, dirty diffing, validation triggers, multi-step wizard with SkipWhen, FormPage sections & overlay FormService).",
            KeyComponents: ["FormSchema", "FormController", "FormService", "LxFormEngine", "LxFormPage", "LxFormProvider"],
            AccentHex: "#f59e0b",
            LandingPagePath: "/form"),

        new(
            Id: "lux-dashboard",
            Name: "Lux.Dashboard",
            RepositoryPath: "/home/gpoumarat/lux-dashboard",
            SolutionFile: "/home/gpoumarat/lux-dashboard/Lux.Dashboard.slnx",
            CorePackage: "Lux.Dashboard.Core",
            UiPackage: "Lux.Dashboard",
            TemplateShortName: "lux-analytics",
            SkillPath: "/home/gpoumarat/lux-dashboard/skills/lux-dashboard/SKILL.md",
            Summary: "Analytical Dashboard Engine (staged Essential/Background/Deferred queries, filters & presets, views, comparison date ranges, CSV export, KPI & chart cards).",
            KeyComponents: ["DashboardController", "DashboardResource", "DashboardFilterControl", "DashboardFormat", "LxDashboardPage", "LxDashboardGrid", "LxDashboardCard", "LxDashboardStat", "LxDashboardStats", "LxDashboardGauge", "LxDashboardTable", "LxDashboardChart", "LxDashboardBars", "LxDashboardList", "LxDashboardAlerts", "LxDashboardFeed"],
            AccentHex: "#8b5cf6",
            LandingPagePath: "/dashboard"),

        new(
            Id: "lux-display",
            Name: "Lux.Display",
            RepositoryPath: "/home/gpoumarat/lux-display",
            SolutionFile: "/home/gpoumarat/lux-display/Lux.Display.slnx",
            CorePackage: "Lux.Display.Core",
            UiPackage: "Lux.Display",
            TemplateShortName: "lux-display-portal",
            SkillPath: "/home/gpoumarat/lux-display/skills/lux-display/SKILL.md",
            Summary: "19-kind read-only Display blocks, Audio/Video media players with clip & play-quota rules, and multi-format FilePreview overlay provider.",
            KeyComponents: ["LxDisplay", "LxAudioPlayer", "LxVideoPlayer", "LxFilePreviewProvider", "FilePreviewService"],
            AccentHex: "#ec4899",
            LandingPagePath: "/display"),

        new(
            Id: "lux-spreadsheet",
            Name: "Lux.Spreadsheet",
            RepositoryPath: "/home/gpoumarat/lux-spreadsheet",
            SolutionFile: "/home/gpoumarat/lux-spreadsheet/Lux.Spreadsheet.slnx",
            CorePackage: "Lux.Spreadsheet.Core",
            UiPackage: "Lux.Spreadsheet",
            TemplateShortName: "lux-spreadsheet-app",
            SkillPath: "/home/gpoumarat/lux-spreadsheet/skills/lux-spreadsheet/SKILL.md",
            Summary: "Headless workbook/CSV/TSV importer (layout detection, NFD header normalization, 80%-pass column suggestions, unknown value questions, duplicates/upsert, inline edits & wizard).",
            KeyComponents: ["SpreadsheetSchema", "SpreadsheetImporter", "LxSpreadsheetImport"],
            AccentHex: "#14b8a6",
            LandingPagePath: "/spreadsheet"),

        new(
            Id: "lux-agent",
            Name: "Lux.Agent",
            RepositoryPath: "/home/gpoumarat/lux-agent",
            SolutionFile: "/home/gpoumarat/lux-agent/Lux.Agent.slnx",
            CorePackage: "Lux.Agent.Core",
            UiPackage: "Lux.Agent",
            TemplateShortName: "lux-agent-copilot",
            SkillPath: "/home/gpoumarat/lux-agent/skills/lux-agent/SKILL.md",
            Summary: "Composable headless UI behaviors (@lux/behaviors) and Semantic Agent Bridge (snapshot, query, act, observe with secret redaction, WebMCP & UndoStack) inspired by ui-sdk.dev.",
            KeyComponents: ["AgentBridgeService", "UndoStack", "ObservedChange", "AgentComponentSnapshot", "AgentActStep"],
            AccentHex: "#6366f1",
            LandingPagePath: "/agent"),
    ];

    /// <summary>Finds the owning <see cref="LuxStackModule"/> for a given component or controller name (case-insensitive).</summary>
    public static LuxStackModule? ResolveComponentModule(string componentOrControllerName)
    {
        if (string.IsNullOrWhiteSpace(componentOrControllerName)) return null;
        var trimmed = componentOrControllerName.Split('<')[0].Trim();
        return Modules.FirstOrDefault(m =>
            m.KeyComponents.Any(c => string.Equals(c, trimmed, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>Finds a module by its repository ID (e.g. <c>"lux-table"</c>) or package name (e.g. <c>"Lux.Table"</c>).</summary>
    public static LuxStackModule? GetModule(string idOrName) =>
        Modules.FirstOrDefault(m =>
            string.Equals(m.Id, idOrName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(m.Name, idOrName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(m.CorePackage, idOrName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(m.UiPackage, idOrName, StringComparison.OrdinalIgnoreCase));
}
