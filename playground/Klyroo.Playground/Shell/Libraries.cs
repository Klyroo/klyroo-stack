namespace Klyroo.Playground.Shell;

/// <summary>One library with a playground: the address of its page, its accent (the one of libraries.yml) and where its documentation lives.</summary>
public sealed record LibraryInfo(string Slug, string Name, string Repo, string Accent, string Texture, string Tagline, string Summary, IReadOnlyList<string> Features)
{
    public const string DocsBase = "https://klyroo.github.io/";

    public string DocsUrl => $"{DocsBase}{Repo}/";
    public string GuidesUrl => $"{DocsBase}{Repo}/guides/";
    public string ApiUrl => $"{DocsBase}{Repo}/api-reference/";
}

public static class Libraries
{
    public static readonly IReadOnlyList<LibraryInfo> All =
    [
        new("ui", "Klyroo.UI", "klyroo-ui", "#10b981", "blueprint-grid", "Components with a color mode and a theme",
            "Buttons, badges, alerts, fields, tabs and the rest of the component set, with the knobs a design system exposes: variant, size, color, states, color mode.",
            ["variant", "size", "color", "disabled", "loading", "color mode"]),
        new("query", "Klyroo.Query", "klyroo-query", "#ff4154", "dot-matrix", "One request for every list",
            "Build a DataListRequest with filters, groups, sorts, paging, search and facets, then read its JSON, its URL form and what it returns from rows in memory.",
            ["filters", "groups", "sort", "paging", "facets", "URL form"]),
        new("table", "Klyroo.Table", "klyroo-table", "#00d8ff", "ruled-rows", "A data list with every part",
            "A KlyDataList over 200 rows: search, filters, multi-sort, three paging modes, grouping, a tree, selection with a bulk action, column choice, CSV export.",
            ["filters", "multi-sort", "paging", "groups", "tree", "selection", "CSV"]),
        new("form", "Klyroo.Form", "klyroo-form", "#f59e0b", "baseline-grid", "Typed forms from a C# model",
            "Pick a model, turn rules, conditions and DataAnnotations on and off, fill the form and read the typed model and the errors it produces.",
            ["wizard", "sections", "repeatable rows", "conditions", "DataAnnotations"]),
        new("dashboard", "Klyroo.Dashboard", "klyroo-dashboard", "#8b5cf6", "chart-ticks", "Dashboards that load in stages",
            "A dashboard with a filter bar, statistics, charts and a table, staged loading you can slow down, drill-down and the filters in the address.",
            ["filters", "stats", "charts", "staged loading", "drill-down", "URL state"]),
        new("display", "Klyroo.Display", "klyroo-display", "#ec4899", "halftone", "Read-only blocks for facts, stats and files",
            "Every KlyDisplay kind with editable values and the variants it supports.",
            ["19 kinds", "variants", "editable values"]),
        new("spreadsheet", "Klyroo.Spreadsheet", "klyroo-spreadsheet", "#14b8a6", "cell-grid", "A guided import for messy files",
            "Load a sample CSV or Excel file or your own, map the columns, answer the value questions, review the bad rows and read the typed result.",
            ["CSV and XLSX", "column mapping", "value questions", "row issues"]),
        new("agent", "Klyroo.Agent", "klyroo-agent", "#6366f1", "node-dots", "A bridge between a model and your UI",
            "A simulated console, with no model and no network: send snapshot, query and act calls as JSON and see the results, the refusals and the confirmation policy.",
            ["snapshot", "query", "act", "refusals", "confirmation"]),
    ];

    public static LibraryInfo? Find(string? slug) => All.FirstOrDefault(l => string.Equals(l.Slug, slug, StringComparison.OrdinalIgnoreCase));
}
