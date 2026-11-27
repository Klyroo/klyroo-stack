using Klyroo.UI;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class Catalog
{
    private static readonly ComponentDoc[] Chart =
    [
        new("chart", "Chart", "Data", "A line, area, bar or donut chart drawn as SVG by the server, with a tooltip, a legend and a data table.",
            [new("Lines and areas", typeof(Examples.ChartLineExample), "Hover a column, or focus a chart and use the arrow keys, Home and End. The values are announced as the focus moves."),
             new("Bars", typeof(Examples.ChartBarExample), "Grouped, stacked and horizontal. The data table is behind a disclosure; by default it is visually hidden but still read by screen readers."),
             new("Donut", typeof(Examples.ChartDonutExample), "One slice per row, sized by the first category. Series differ by fill pattern as well as by color."),
             new("Funnel", typeof(Examples.ChartFunnelExample), "Type=Funnel draws one bar per row, sized by the first category relative to the largest step, with the share of the previous step, as a list of steps."),
             new("Selection", typeof(Examples.ChartSelectExample), "OnSelect reports the row, its index, the series, the value and the row item when a bar, point or slice is clicked, or chosen with Enter or Space on the row the arrow keys are on. Only a chart with a handler is clickable: it gets a pointer cursor and a keyboard hint that says Enter selects.")],
            [typeof(KlyChart<object, object>)]),
    ];
}
