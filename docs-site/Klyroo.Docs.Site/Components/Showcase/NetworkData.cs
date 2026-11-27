namespace Klyroo.Docs.Site.Components.Showcase;

/// <summary>The eight directions a leg of a line can take: the grammar of a transit diagram only knows 0 and 45 degrees.</summary>
public enum Heading { N, NE, E, SE, S, SW, W, NW }

/// <summary>Which side of the line its station labels sit on, as seen on the page (not along the direction of travel).</summary>
public enum LabelSide { Above, Below, Left, Right }

/// <summary>One straight stretch of a line. <paramref name="Extent"/> is its reach along the main axis of the heading (the horizontal reach of a diagonal), in map units.</summary>
public sealed record Leg(Heading Heading, double Extent);

/// <summary>
/// A station of a line: a feature the library really has. <paramref name="Label"/> is the short name on the map, <paramref name="FeatureTitle"/> the bold title of
/// the bullet of the "Features" list of the README of the library that it stands for (the tests check it there), <paramref name="Text"/> what the bullet says.
/// </summary>
public sealed record Station(string Label, string FeatureTitle, string Text);

/// <summary>A line of the network: the library <paramref name="Repo"/>, the legs that draw it from the interchange, and its stations, in the order of travel.</summary>
/// <param name="Tagline">The tagline of <c>docs/site.yml</c> of the library, as written there.</param>
/// <param name="Package">The NuGet package that the install line of the chapter adds.</param>
/// <param name="Start">How far from the interchange the first station may be, in map units: past the discs of the interchange, and clear of the labels of the lines beside it.</param>
public sealed record MetroLine(string Repo, IReadOnlyList<Leg> Legs, LabelSide Side, string Tagline, string Package, IReadOnlyList<Station> Stations, double Start = 62)
{
    /// <summary>The short name, <c>table</c> for <c>klyroo-table</c>: the key of the line in addresses, ids and CSS.</summary>
    public string Key => Repo.StartsWith("klyroo-", StringComparison.Ordinal) ? Repo["klyroo-".Length..] : Repo;

    public string InstallCommand => $"dotnet add package {Package} --prerelease";
}

/// <summary>One of the four rules of the network (the pillars the user chose), with the proof the product gives for it.</summary>
public sealed record Pillar(string Title, string Short, string Rule, string Proof)
{
    /// <summary>The id of its rule on the page: <c>headless-first</c>.</summary>
    public string Id => Title.ToLowerInvariant().Replace(' ', '-');
}

/// <summary>A <c>dotnet new</c> template of <c>Klyroo.Templates</c>, as the README of klyroo-stack lists it.</summary>
public sealed record TemplateInfo(string ShortName, string Creates);

/// <summary>
/// What the showcase says, as data: the eight lines with their stations, the four rules and the templates. Every line is a library of
/// <c>libraries.yml</c>; every station is a bullet of the README of its library. Nothing here is a number the repositories do not state.
/// </summary>
public static class NetworkData
{
    public static IReadOnlyList<MetroLine> Lines { get; } =
    [
        new("klyroo-table", [new(Heading.NE, 70), new(Heading.E, 220)], LabelSide.Above,
            "Data lists for Blazor and .NET MAUI Blazor Hybrid", "Klyroo.Table",
            [
                new("Schema", "One schema for search, filters and sort", "A DataListSchema<T> declares which fields sort, filter and search, and requests are sanitized against it before they reach the data."),
                new("URL state", "State in the URL", "Search, filters, sort, page, page size and layout are written to the query string, so Back, shared links and static server rendering work."),
                new("Paging", "Three paging modes", "Numbered pages, keyset cursor paging with an infinite loader, or no paging."),
                new("Selection", "Selection and actions", "A checkbox column, a bulk action bar that can target every result of a query, and per-row actions with confirmation dialogs."),
                new("Columns", "Column control", "Show, hide, reorder, pin and resize columns, with preferences kept in a cookie."),
            ]),
        new("klyroo-query", [new(Heading.E, 140), new(Heading.SE, 40), new(Heading.E, 110)], LabelSide.Below,
            "A typed query model for data lists in .NET", "Klyroo.Query",
            [
                new("Typed schema", "Typed schema", "Sortable fields, filters, search fields and defaults are declared with expressions such as c => c.Country!.Name, so there are no field names to mistype."),
                new("Filter tree", "Filter tree", "Five filter kinds, 13 operators, nested AND/OR groups, and an include-empty flag on a rule."),
                new("Facets", "Paging and facets", "Offset paging, keyset cursor paging with a stable sort, and option counts per filter."),
                new("Validation", "Request validation", "Sanitize drops unknown keys, operators the filter does not allow, malformed values and oversized pages before a request reaches your data."),
            ], Start: 75),
        new("klyroo-form", [new(Heading.N, 250), new(Heading.NW, 60), new(Heading.N, 30)], LabelSide.Right,
            "Schema-driven forms, validation and wizards for .NET developers, with a headless engine and Blazor components", "Klyroo.Form",
            [
                new("Typed fields", "Typed schema", "Fields are declared from the members of a C# class, and a field of the wrong kind does not compile."),
                new("Rules", "Validation", "Required and required-when checks, number bounds, typed Rule and RuleAsync rules with a pending state, four validation modes and three triggers."),
                new("Steps", "Wizards and pages", "Multi-step forms with skippable steps and async before-next hooks, and sectioned pages that show a completion status per section."),
                new("Drafts", "Unsaved changes", "KlyFormNavigationGuard blocks navigation while the form is dirty, and ExportDraft() and RestoreDraft() keep a draft across a reload."),
            ], Start: 90),
        new("klyroo-dashboard", [new(Heading.NW, 55), new(Heading.W, 205)], LabelSide.Above,
            "Analytical dashboards for .NET developers", "Klyroo.Dashboard",
            [
                new("Stages", "Staged loading", "A resource is Essential, Background or Deferred, and a new load cancels the one it supersedes."),
                new("Block states", "Per-block states", "Every block shows a skeleton while loading, the error with a retry button, and an empty state, so one failing query leaves the rest of the page intact."),
                new("Drill-down", "Drill-down", "OnSelect on bars and charts and OnRowClick on tables and lists hand the pick to your code, with real buttons the keyboard reaches."),
                new("Blocks", "Ten blocks on a grid of 12 columns by default", "Stat, stats, gauge, bars, chart, list, table, details, feed and alerts, wrapped by a page, a grid and a card."),
            ]),
        new("klyroo-agent", [new(Heading.W, 290)], LabelSide.Below,
            "A semantic bridge that lets AI agents read and operate Klyroo forms, dashboards and spreadsheet imports", "Klyroo.Agent",
            [
                new("Snapshot", "Snapshot", "Lists every registered component with a stable reference, its label, current value and the actions it accepts."),
                new("Query", "Query", "Lists the choices of a select or filter, filtered by text, without opening a popup."),
                new("Act", "Act", "Runs named actions such as setValue, toggle, submit and nextStep by reference or label, and reports why a step failed."),
                new("Permissions", "Permission gate", "AllowPolicy and a per-call allow callback can refuse any step, and steps that need confirmation are refused when no policy is set."),
            ]),
        new("klyroo-spreadsheet", [new(Heading.SW, 90), new(Heading.S, 240)], LabelSide.Left,
            "Import CSV, TSV, Excel and pasted spreadsheet data into a .NET application", "Klyroo.Spreadsheet",
            [
                new("Header detection", "Header and sheet detection", "Scans the first 20 rows of a sheet and picks the row where most schema columns match, so title rows above the header are skipped."),
                new("Column matching", "Forgiving column matching", "Compares headers ignoring case, accents, extra spaces and a trailing asterisk, against the column key, its label and its aliases."),
                new("Typed rules", "Typed parsing and rules", "Text, number, date, boolean and select columns, with built-in Email, Pattern, Min, Max and Between rules and custom rules."),
                new("Review", "Inline correction", "Edits cells, discards and restores rows, and filters the review by tab, severity, issue and text, without touching the source file."),
            ]),
        new("klyroo-display", [new(Heading.S, 260)], LabelSide.Right,
            "Read-only display blocks, audio and video control bars, and a file preview overlay for Blazor", "Klyroo.Display",
            [
                new("Block kinds", "One component, 19 block kinds", "KlyDisplay renders headings, text, alerts, facts, statistics, meters, charts, tables, diffs, records, lists, timelines, code values and more."),
                new("Code", "Syntax highlighting", "The code block colors C#, JavaScript, TypeScript, JSON, HTML, XML, CSS, SQL, Python, Bash and YAML from its Language."),
                new("Secrets", "Secret values", "The secret variant of the code block masks all but the last characters, with a Reveal toggle and a copy button."),
                new("Preview", "File preview overlay", "KlyFilePreviewProvider shows PDF, images, video, audio, CSV, Markdown and text in a modal, drawer or fullscreen panel."),
            ], Start: 120),
        new("klyroo-ui", [new(Heading.SE, 130), new(Heading.S, 200)], LabelSide.Right,
            "Accessible Blazor components styled with Tailwind CSS v4", "Klyroo.UI",
            [
                new("Components", "143 components", "Elements, form controls, overlays, navigation, data, layout, page sections, dashboards, chat and a rich text editor."),
                new("Render modes", "Every render mode", "The same markup in static server rendering, Interactive Server, WebAssembly and Auto, because UI state lives in custom elements in the browser."),
                new("Theme", "Design tokens and palettes", "Map the semantic colors to any Tailwind palette in C#, with the contrast rules checked, or override --klyroo-* CSS variables."),
                new("Icons", "Build-time icons and fonts", "Any Iconify icon works by name and fonts are self-hosted, with no request to a third party when the page runs."),
            ]),
    ];

    public static IReadOnlyList<Pillar> Pillars { get; } =
    [
        new("Headless first", "Headless first",
            "Every domain capability is an engine with no UI; the Blazor components are thin shells over it.",
            "Klyroo.Stack.Core references the seven Core engines, none of which depends on Razor or ASP.NET Core, and is marked IsAotCompatible and IsTrimmable."),
        new("Typed end to end", "Typed end to end",
            "Fields, filters and sorts are C# expressions, so a name you mistype does not compile.",
            "A Klyroo.Form schema is declared from the members of your class, and a Klyroo.Query schema from expressions such as c => c.Country!.Name."),
        new("Every render mode", "Every render mode",
            "Static server rendering, Interactive Server, WebAssembly, Auto and .NET MAUI Blazor Hybrid are all first-class.",
            "This page is rendered once as static HTML, with no circuit and no WebAssembly, and its tabs, tooltips, popovers and search are Klyroo.UI components."),
        new("Operable by AI", "Operable by AI",
            "Agents get the documentation and the live controls through machine-readable surfaces.",
            "Klyroo.Stack.Mcp serves the Klyroo.UI documentation over stdio, the site generates llms.txt, and Klyroo.Agent lets an agent snapshot, query and act on forms, dashboards and imports."),
    ];

    public static IReadOnlyList<TemplateInfo> Templates { get; } =
    [
        new("klyroo-saas", "A Blazor Web App for a SaaS product: a header with navigation and a color mode button, a dashboard, a filterable accounts list with a form drawer, and a usage page of display blocks."),
        new("klyroo-admin", "A Blazor Web App for a back office: a sidebar shell, a dashboard, a customer list with selection and bulk actions, a customer page, and a spreadsheet import."),
        new("klyroo-headless", "A console app with PublishAot that uses Klyroo.Stack.Core only."),
        new("klyroo-maui", "A .NET MAUI Blazor Hybrid app for Android, iOS, Mac Catalyst and Windows."),
    ];
}
