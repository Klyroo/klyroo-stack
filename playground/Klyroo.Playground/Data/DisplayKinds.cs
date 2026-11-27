namespace Klyroo.Playground.Data;

public enum DisplayFieldType { Text, Multi, Bool, Choice }

/// <summary>One editable value of a KlyDisplay kind. <paramref name="OnlyFor"/> limits it to some variants.</summary>
public sealed record DisplayField(string Key, string Label, string Default, DisplayFieldType Type = DisplayFieldType.Text, string? Hint = null, string[]? Choices = null, DisplayVariant[]? OnlyFor = null);

public sealed record DisplayKindSpec(DisplayKind Kind, string Summary, DisplayVariant[] Variants, DisplayField[] Fields);

/// <summary>The 19 kinds of KlyDisplay with sample values to edit and the variants each one supports (DisplayVariant lists which kind a variant belongs to).</summary>
public static class DisplayKinds
{
    private static DisplayField T(string key, string label, string def, string? hint = null, DisplayVariant[]? only = null) => new(key, label, def, DisplayFieldType.Text, hint, null, only);
    private static DisplayField M(string key, string label, string def, string? hint = null, DisplayVariant[]? only = null) => new(key, label, def, DisplayFieldType.Multi, hint, null, only);
    private static DisplayField B(string key, string label, bool def) => new(key, label, def ? "1" : "0", DisplayFieldType.Bool);
    private static DisplayField C(string key, string label, string def, params string[] choices) => new(key, label, def, DisplayFieldType.Choice, null, choices);
    private static DisplayVariant[] V(params DisplayVariant[] v) => v;

    public static readonly IReadOnlyList<DisplayKindSpec> All =
    [
        new(DisplayKind.Heading, "A section heading with a count and a description.", V(DisplayVariant.Eyebrow),
            [T("title", "Title", "Orders"), T("description", "Description", "Everything placed this quarter"), T("count", "Count", "128")]),
        new(DisplayKind.Text, "Paragraphs, or a quote with its author.", V(DisplayVariant.Quote),
            [M("paragraphs", "Paragraphs", "Orders are packed within one working day.\nTracking links are sent by email."), T("author", "Author", "Support team", only: V(DisplayVariant.Quote)), T("meta", "Meta", "Updated Nov 2026", only: V(DisplayVariant.Quote)), T("maxlines", "Clamp to this many lines", "", "Empty for no clamp; a Read more toggle appears.")]),
        new(DisplayKind.Alert, "A message with an icon from its tone, details and actions.", V(),
            [T("title", "Title", "Payment failed"), T("description", "Description", "The card was declined."), T("details", "Details", "Error 51: insufficient funds."), C("tone", "Tone", "Error", "Primary", "Info", "Success", "Warning", "Error"), T("action", "Action label", "Try another card")]),
        new(DisplayKind.Facts, "Label and value rows. An empty value shows a dash.", V(),
            [M("facts", "Rows, one per line: label | value", "Reference | ORD-1042\nStatus | Paid\nCountry | France\nShipping address |", "Add :copy after a value for a copy button.")]),
        new(DisplayKind.Stat, "A highlighted value with a delta and a sparkline, or several metrics in a row.", V(DisplayVariant.Row),
            [T("title", "Title", "Monthly revenue"), T("value", "Value", "148,900", only: V(DisplayVariant.Default)), T("unit", "Unit", "EUR", only: V(DisplayVariant.Default)), T("delta", "Delta", "+14.2%", only: V(DisplayVariant.Default)), C("direction", "Delta direction", "Up", "Up", "Down", "Flat"), T("trend", "Trend, numbers separated by commas", "102,98,115,110,132,128,149,141,158", only: V(DisplayVariant.Default)), M("items", "Metrics, one per line: label | value | caption", "Orders | 1,204 | +8%\nRefunds | 31 | -2%\nAverage | 124 EUR", only: V(DisplayVariant.Row))]),
        new(DisplayKind.Breakdown, "Labelled amounts with subtotals and a total, one or two columns.", V(DisplayVariant.Collapsible),
            [M("lines", "Lines: label | amount | compare amount", "Subtotal | 120.00 | 100.00\nShipping | 8.00 | 8.00\nDiscount | -12.00 | 0.00"), T("total", "Total: label | amount | compare", "Total | 116.00 | 108.00"), T("compare", "Column titles, with a comma", "This month, Last month", "Empty for one column.")]),
        new(DisplayKind.Meter, "A horizontal meter with thresholds, a ring or a scale of levels.", V(DisplayVariant.Ring, DisplayVariant.Scale),
            [T("title", "Title", "Storage"), T("value", "Value", "46"), T("max", "Maximum", "50"), T("warning", "Warning threshold", "40", only: V(DisplayVariant.Default, DisplayVariant.Ring)), T("error", "Error threshold", "45", only: V(DisplayVariant.Default, DisplayVariant.Ring)), T("description", "Description", "46 of 50 GB used"), T("levels", "Levels, with a comma", "Low, Medium, High, Critical", only: V(DisplayVariant.Scale)), T("current", "Current level (0 based)", "2", only: V(DisplayVariant.Scale))]),
        new(DisplayKind.Chart, "A small bar chart, a line or a funnel.", V(DisplayVariant.Line, DisplayVariant.Funnel),
            [T("title", "Title", "Visits this week"), M("data", "Points: label | value", "Mon | 12\nTue | 18\nWed | 9\nThu | 22\nFri | 15")]),
        new(DisplayKind.Table, "A plain table of text cells, with a sticky header option.", V(DisplayVariant.Sticky),
            [T("columns", "Columns, with a comma", "Name, Role, Seats"), M("rows", "Rows: cells separated by |", "Ada | Admin | 3\nGrace | Editor | 12\nAlan | Viewer | 40")]),
        new(DisplayKind.Diff, "Before and after values per field.", V(),
            [M("items", "Fields: label | before | after", "Plan | Free | Team\nSeats | 3 | 3\nRegion | EU | US"), B("unchanged", "Show unchanged fields", false)]),
        new(DisplayKind.Record, "Entity rows with a reference, a secondary line and a trailing value.", V(),
            [M("records", "Rows: reference | title | meta | trailing", "ORD-1042 | Keyboard and mouse | Shipped Nov 12 | 129.00\nORD-1043 | Monitor stand | Packed Nov 13 | 59.00")]),
        new(DisplayKind.Person, "A person with contacts, or a group of avatars.", V(DisplayVariant.Group),
            [M("people", "People: name | description", "Ada Lovelace | Engineer\nGrace Hopper | Admiral\nAlan Turing | Mathematician\nLinus Torvalds | Maintainer"), M("contacts", "Contacts, one per line", "ada@example.test\n+33 1 23 45 67 89", only: V(DisplayVariant.Default))]),
        new(DisplayKind.List, "A list, numbered steps or a checklist.", V(DisplayVariant.Steps, DisplayVariant.Checks),
            [M("items", "Items, one per line", "Install the package\nRegister the services\nAdd the component", "Start a line with x to mark it done in steps and checks.")]),
        new(DisplayKind.Timeline, "Events with a state, in a column, a pipeline, a log or an agenda.", V(DisplayVariant.Pipeline, DisplayVariant.Log, DisplayVariant.Schedule),
            [M("events", "Events: title | time | description", "Order placed | 09:12 | Payment accepted\nPacked | 11:40 | Two boxes\nShipped | 15:05 | Tracking sent\nDelivered | |")]),
        new(DisplayKind.Date, "A calendar tile, a countdown or a range.", V(DisplayVariant.Countdown, DisplayVariant.Range),
            [T("title", "Title", "Winter break"), T("date", "Date, yyyy-MM-dd", "2026-12-25"), T("end", "End date, yyyy-MM-dd", "2027-01-05", only: V(DisplayVariant.Range))]),
        new(DisplayKind.Media, "An image, or a gallery with a +N tile.", V(DisplayVariant.Gallery),
            [T("count", "Number of images in the gallery", "5", only: V(DisplayVariant.Gallery))]),
        new(DisplayKind.Code, "A preformatted value with a copy button, colored from its language, or a masked secret.", V(DisplayVariant.Secret),
            [M("value", "Value", "public record Order(int Id)\n{\n    public string Label => $\"Order {Id}\";\n}"), C("language", "Language", "csharp", "csharp", "json", "html", "sql", "bash", "yaml", "typescript"), T("visible", "Characters left visible", "4", only: V(DisplayVariant.Secret))]),
        new(DisplayKind.Empty, "An empty state with an action.", V(),
            [T("title", "Title", "No orders yet"), T("description", "Description", "Orders you place appear here."), T("action", "Action label", "Create an order")]),
        new(DisplayKind.Accordion, "Collapsible sections.", V(),
            [M("items", "Sections: label | content", "Shipping | Orders ship within one working day.\nReturns | Return anything within 30 days.\nWarranty | Two years on every device."), B("multiple", "Several open at once", false)]),
    ];

    public static DisplayKindSpec Of(DisplayKind kind) => All.First(k => k.Kind == kind);
}
