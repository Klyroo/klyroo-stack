using Klyroo.UI;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class Catalog
{
    /// <summary>The read-only blocks: each one is a typed component with only its own parameters, a <c>Surface</c>, a <c>Loading</c> skeleton and a <c>Ui</c> record.</summary>
    private static readonly ComponentDoc[] Blocks =
    [
        new("heading", "Heading", "Blocks", "A section heading with a count.",
            [new("Heading", typeof(Examples.HeadingExample))], [typeof(KlyHeading)]),
        new("text", "Text", "Blocks", "Paragraphs, a quote, clamped text.",
            [new("Clamped text and a quote", typeof(Examples.TextExample))], [typeof(KlyText)]),
        new("stat", "Stat", "Blocks", "A number with a delta and a sparkline.",
            [new("Typed numbers", typeof(Examples.StatExample), "Value and Delta are numbers, formatted with Format and Culture; the sign of the delta picks its color unless DeltaDirection is set. Write a number as an expression (Value=\"@_revenue\"): a bare attribute value is text. A nullable value shows a dash.")],
            [typeof(KlyStat<decimal>)]),
        new("stat-group", "StatGroup", "Blocks", "Small metrics side by side.",
            [new("Metrics in a row", typeof(Examples.StatGroupExample))], [typeof(KlyStatGroup<int>)]),
        new("facts", "Facts", "Blocks", "Label and value rows.",
            [new("Facts and groups", typeof(Examples.FactsExample), "Copy buttons write to the clipboard from .NET, so they are disabled until the page is interactive. Max and Step show a Show more button.")],
            [typeof(KlyFacts)]),
        new("breakdown", "Breakdown", "Blocks", "Amounts with subtotals and a total.",
            [new("Invoice and comparison", typeof(Examples.BreakdownExample), "Amounts are decimals, formatted with Format and Culture. Two amounts per line switch the block to two columns, and Collapsible hides the lines while keeping the total.")],
            [typeof(KlyBreakdown)]),
        new("diff", "Diff", "Blocks", "Before and after values.",
            [new("Typed comparisons", typeof(Examples.DiffExample), "KlyDiffItem.Compare builds an item from two typed values and decides Changed by equality. The values are formatted when the block renders, with the Culture of the block.")],
            [typeof(KlyDiff)]),
        new("record-list", "RecordList", "Blocks", "Entity rows with a trailing value.",
            [new("Invoices", typeof(Examples.RecordListExample))], [typeof(KlyRecordList)]),
        new("list", "List", "Blocks", "Plain, stepped or checked items.",
            [new("Plain, steps and checks", typeof(Examples.ListExample))], [typeof(KlyList)]),
        new("date-tile", "DateTile", "Blocks", "A calendar tile.",
            [new("Date, countdown and range", typeof(Examples.DateTileExample), "Pass Now for a stable server render.")], [typeof(KlyDateTile)]),
        new("meter", "Meter", "Blocks", "A gauge: bar, ring or scale.",
            [new("Bar, ring, parts and scale", typeof(Examples.MeterExample), "KlyMeter is a role=\"meter\" measure (disk used, quota), not the progress of a task: use KlyProgress for that.")],
            [typeof(KlyMeter)]),
        new("image-gallery", "ImageGallery", "Blocks", "A grid of images.",
            [new("Gallery", typeof(Examples.ImageGalleryExample))], [typeof(KlyImageGallery)]),
    ];
}
