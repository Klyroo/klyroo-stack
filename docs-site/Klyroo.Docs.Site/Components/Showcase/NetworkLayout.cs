using System.Globalization;
using System.Text;
using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Components.Showcase;

public readonly record struct Pt(double X, double Y);

/// <summary>A station once it has a place on the map: where its tick is, where its label starts and how it is turned.</summary>
/// <param name="Distance">How far from the interchange along the line, in map units.</param>
/// <param name="Anchor"><c>start</c> or <c>end</c>: which end of the label touches the tick.</param>
/// <param name="Delay">Seconds after the page loads at which the line has been drawn up to this station.</param>
public sealed record PlacedStation(Station Station, Pt At, double Distance, Pt Label, double Rotation, string Anchor, double Delay);

public sealed record BadgeBox(double X, double Y, double Width, double Height, string Fill, string OnFill)
{
    public Pt Center => new(X + Width / 2, Y + Height / 2);
}

/// <summary>Where the camera of the scroll journey stands on a line: <c>translate(X Y) scale(Scale)</c> on the map.</summary>
public sealed record Camera(double Scale, double X, double Y)
{
    public string Transform => string.Create(CultureInfo.InvariantCulture, $"translate({X:0.##}px, {Y:0.##}px) scale({Scale:0.###})");
}

/// <summary>A line of the network with its geometry computed.</summary>
public sealed record PlacedLine(MetroLine Line, LibraryInfo Library, IReadOnlyList<Pt> Route, string Path, double Length, IReadOnlyList<PlacedStation> Stations, BadgeBox Badge, Camera Camera, double Delay)
{
    public string Key => Line.Key;
}

/// <summary>
/// The geometry of the network, computed from the typed data: the route of each line from the legs, rounded at the bends, the stations spread over the straight
/// stretches (never on a diagonal or close to a bend), the label of each station turned by the direction it sits on, the badge at the end, and the camera that frames the line.
/// The map is drawn around the interchange, which is the origin.
/// </summary>
public sealed class NetworkLayout
{
    /// <summary>The visible area of the map (<c>viewBox</c>): wide enough for the longest line, centered on the interchange.</summary>
    public const double Width = 920, Height = 760;
    public const double InterchangeRadius = 50;
    public const double BadgeHeight = 34;
    private const double BendRadius = 20, BendMargin = 30, EndMargin = 28;

    public IReadOnlyList<PlacedLine> Lines { get; }

    public double ViewX => -Width / 2;
    public double ViewY => -Height / 2;
    public string ViewBox => string.Create(CultureInfo.InvariantCulture, $"{ViewX:0.##} {ViewY:0.##} {Width:0.##} {Height:0.##}");

    /// <summary>The camera that stands between two chapters: close on the interchange, where the train arrives.</summary>
    public static Camera Hub { get; } = new(1, 0, 0);

    public static Camera Overview { get; } = new(1, 0, 0);

    private NetworkLayout(IReadOnlyList<PlacedLine> lines) => Lines = lines;

    public static NetworkLayout Build(Libraries libraries, IReadOnlyList<MetroLine>? data = null)
    {
        var lines = new List<PlacedLine>();
        var index = 0;
        foreach (var line in data ?? NetworkData.Lines)
        {
            var library = libraries.Find(line.Repo) ?? throw new InvalidOperationException($"The line {line.Repo} is not a library of libraries.yml.");
            lines.Add(Place(line, library, index++));
        }
        return new NetworkLayout(lines);
    }

    public static (double X, double Y) Vector(Heading heading) => heading switch
    {
        Heading.N => (0, -1),
        Heading.NE => (1, -1),
        Heading.E => (1, 0),
        Heading.SE => (1, 1),
        Heading.S => (0, 1),
        Heading.SW => (-1, 1),
        Heading.W => (-1, 0),
        _ => (-1, -1),
    };

    public static bool IsDiagonal(Heading heading) => Vector(heading) is { X: not 0, Y: not 0 };

    private static PlacedLine Place(MetroLine line, LibraryInfo library, int index)
    {
        var route = new List<Pt> { new(0, 0) };
        var lengths = new List<double>();
        foreach (var leg in line.Legs)
        {
            var (vx, vy) = Vector(leg.Heading);
            var last = route[^1];
            route.Add(new Pt(last.X + vx * leg.Extent, last.Y + vy * leg.Extent));
            lengths.Add(Math.Sqrt(vx * vx + vy * vy) * leg.Extent);
        }
        var total = lengths.Sum();

        // The straight stretches a station may stand on: not on a diagonal, not within a margin of a bend, not under the interchange or the badge.
        var usable = new List<(int Leg, double From, double To, double Start)>();
        var start = 0.0;
        for (var i = 0; i < line.Legs.Count; i++)
        {
            if (!IsDiagonal(line.Legs[i].Heading))
            {
                var from = i == 0 ? line.Start : BendMargin;
                var to = lengths[i] - (i == line.Legs.Count - 1 ? EndMargin : BendMargin);
                if (to > from)
                {
                    usable.Add((i, from, to, start));
                }
            }
            start += lengths[i];
        }
        var room = usable.Sum(u => u.To - u.From);
        var stations = new List<PlacedStation>();
        for (var k = 0; k < line.Stations.Count; k++)
        {
            var u = room * (k + 0.5) / line.Stations.Count;
            foreach (var (leg, from, to, legStart) in usable)
            {
                if (u > to - from)
                {
                    u -= to - from;
                    continue;
                }
                var local = from + u;
                var heading = line.Legs[leg].Heading;
                var (vx, vy) = Vector(heading);
                var at = new Pt(route[leg].X + vx * local, route[leg].Y + vy * local);
                var distance = legStart + local;
                var (label, rotation, anchor) = Label(heading, line.Side, at);
                stations.Add(new PlacedStation(line.Stations[k], at, distance, label, rotation, anchor, 0.15 * index + 1.1 * distance / total));
                break;
            }
        }

        var end = route[^1];
        var badge = Badge(line.Legs[^1].Heading, end, library.Name, library.Accent ?? "#10b981");
        return new PlacedLine(line, library, route, PathData(route, lengths), total, stations, badge, CameraFor(route, badge), 0.15 * index);
    }

    private static (Pt Label, double Rotation, string Anchor) Label(Heading heading, LabelSide side, Pt at)
    {
        var horizontal = heading is Heading.E or Heading.W;
        if (horizontal)
        {
            var above = side is LabelSide.Above or LabelSide.Left;
            return heading == Heading.E
                ? (above ? (new Pt(at.X - 2, at.Y - 17), -45, "start") : (new Pt(at.X - 4, at.Y + 19), 45, "start"))
                : (above ? (new Pt(at.X + 6, at.Y - 16), 45, "end") : (new Pt(at.X + 4, at.Y + 20), -45, "end"));
        }
        return side == LabelSide.Left ? (new Pt(at.X - 22, at.Y + 5), 0, "end") : (new Pt(at.X + 22, at.Y + 5), 0, "start");
    }

    private static BadgeBox Badge(Heading heading, Pt end, string name, string accent)
    {
        var width = Math.Round(26 + name.Length * 8.3);
        var theme = AccentColor.Compute(accent);
        var (x, y) = heading switch
        {
            Heading.E => (end.X, end.Y - BadgeHeight / 2),
            Heading.W => (end.X - width, end.Y - BadgeHeight / 2),
            Heading.N => (end.X - width / 2, end.Y - BadgeHeight),
            _ => (end.X - width / 2, end.Y),
        };
        return new BadgeBox(x, y, width, BadgeHeight, theme.Fill, theme.OnFill);
    }

    // A polyline with the bends cut and rounded: the straight stretches keep their length, the corner becomes a quadratic curve.
    private static string PathData(IReadOnlyList<Pt> route, IReadOnlyList<double> lengths)
    {
        var d = new StringBuilder();
        string N(double v) => v.ToString("0.##", CultureInfo.InvariantCulture);
        d.Append('M').Append(N(route[0].X)).Append(' ').Append(N(route[0].Y));
        for (var i = 1; i < route.Count - 1; i++)
        {
            var p = route[i];
            var t = Math.Min(BendRadius, Math.Min(lengths[i - 1], lengths[i]) / 2);
            Pt Toward(Pt from, double distance, double length) => new(p.X + (from.X - p.X) / length * distance, p.Y + (from.Y - p.Y) / length * distance);
            var a = Toward(route[i - 1], t, lengths[i - 1]);
            var b = Toward(route[i + 1], t, lengths[i]);
            d.Append(" L").Append(N(a.X)).Append(' ').Append(N(a.Y));
            d.Append(" Q").Append(N(p.X)).Append(' ').Append(N(p.Y)).Append(' ').Append(N(b.X)).Append(' ').Append(N(b.Y));
        }
        d.Append(" L").Append(N(route[^1].X)).Append(' ').Append(N(route[^1].Y));
        return d.ToString();
    }

    // The camera frames the route and its badge with room for the labels, as close as the area allows (never closer than 1.9 times).
    private static Camera CameraFor(IReadOnlyList<Pt> route, BadgeBox badge)
    {
        var xs = route.Select(p => p.X).Concat([badge.X, badge.X + badge.Width]).ToList();
        var ys = route.Select(p => p.Y).Concat([badge.Y, badge.Y + badge.Height]).ToList();
        const double pad = 70;
        var (minX, maxX, minY, maxY) = (xs.Min() - pad, xs.Max() + pad, ys.Min() - pad, ys.Max() + pad);
        var scale = Math.Clamp(Math.Min(Width * 0.8 / (maxX - minX), Height * 0.8 / (maxY - minY)), 1.12, 1.9);
        var (cx, cy) = ((minX + maxX) / 2, (minY + maxY) / 2);
        return new Camera(Math.Round(scale, 3), Math.Round(-cx * scale, 1), Math.Round(-cy * scale, 1));
    }

    /// <summary>
    /// The CSS of the scroll journey, as keyframes computed from the camera of each line: the stage rides line after line while the page scrolls through the chapters
    /// (the page is nine screens: the first one is the opening, then one per chapter). <c>--journey</c> is the view timeline of the whole journey, declared on the section.
    /// </summary>
    public string JourneyCss()
    {
        var n = Lines.Count;
        string P(double fraction) => (fraction * 100).ToString("0.##", CultureInfo.InvariantCulture) + "%";
        double Center(int i) => (i + 1d) / n;
        double Between(int i) => (i + 0.5) / n;
        const double Hold = 0.02, Dim = 0.16;
        var css = new StringBuilder();

        css.Append("@keyframes journey-camera{0%,1.2%{transform:").Append(Overview.Transform).Append('}');
        for (var i = 0; i < n; i++)
        {
            css.Append(P(Between(i))).Append("{transform:").Append(Hub.Transform).Append('}');
            css.Append(P(Center(i) - Hold)).Append(',').Append(P(i == n - 1 ? 1 : Center(i) + Hold)).Append("{transform:").Append(Lines[i].Camera.Transform).Append('}');
        }
        css.Append('}');

        for (var i = 0; i < n; i++)
        {
            var key = Lines[i].Key;
            css.Append("@keyframes journey-line-").Append(key).Append("{0%,1.2%{opacity:1}");
            for (var j = 0; j < n; j++)
            {
                var adjacent = i == j || i == j - 1;
                css.Append(P(Between(j))).Append("{opacity:").Append(adjacent ? "0.55" : Dim.ToString(CultureInfo.InvariantCulture)).Append('}');
                css.Append(P(Center(j) - Hold)).Append(',').Append(P(Center(j) + Hold)).Append("{opacity:").Append(i == j ? "1" : Dim.ToString(CultureInfo.InvariantCulture)).Append('}');
            }
            css.Append('}');

            // The rider: the yellow-headed train that carries the reader along the line, and brings the reader back to the interchange before the next chapter.
            var c = Center(i);
            css.Append("@keyframes journey-rider-").Append(key).Append('{');
            css.Append("0%,").Append(P(c - 0.052)).Append("{opacity:0;offset-distance:0%}");
            css.Append(P(c - 0.046)).Append("{opacity:1;offset-distance:0%}");
            css.Append(P(c - 0.006)).Append(',').Append(P(c + 0.022)).Append("{opacity:1;offset-distance:100%}");
            if (i < n - 1)
            {
                css.Append(P(Between(i + 1))).Append("{opacity:1;offset-distance:0%}");
                css.Append(P(Between(i + 1) + 0.004)).Append(",100%{opacity:0;offset-distance:0%}");
            }
            else
            {
                css.Append("100%{opacity:1;offset-distance:100%}");
            }
            css.Append('}');

            css.Append(".sc-map [data-line=\"").Append(key).Append("\"]{--line-anim:journey-line-").Append(key).Append(";--rider-anim:journey-rider-").Append(key).Append('}');

            // Its stations light up one after the other while the chapter is read.
            for (var s = 0; s < Lines[i].Stations.Count; s++)
            {
                var from = c - 0.045 + s * 0.065 / Lines[i].Stations.Count;
                css.Append(".sc-map [data-line=\"").Append(key).Append("\"] [data-station=\"").Append(s).Append("\"]{--a:")
                    .Append((from * 100).ToString("0.##", CultureInfo.InvariantCulture)).Append(";--b:")
                    .Append(((from + 0.014) * 100).ToString("0.##", CultureInfo.InvariantCulture)).Append('}');
            }
        }
        return css.ToString();
    }
}
