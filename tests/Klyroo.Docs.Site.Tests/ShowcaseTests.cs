using System.Text.RegularExpressions;
using Klyroo.Docs.Site.Components.Showcase;
using Klyroo.Docs.Site.Content;
using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Tests;

public class ShowcaseTests
{
    private static Libraries RealLibraries() => Libraries.Load(Path.Combine(AppContext.BaseDirectory, "libraries.yml"));

    // The folder that holds the nine repositories side by side, when the tests run from a workspace checkout.
    private static string? Workspace()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, "klyroo-ui")) && Directory.Exists(Path.Combine(dir.FullName, "klyroo-stack")))
            {
                return dir.FullName;
            }
        }
        return null;
    }

    [Fact]
    public void The_home_setting_defaults_to_docs_and_accepts_showcase()
    {
        using var repo = new FakeRepository().Write("README.md", "# Demo\n\nText.\n");
        Assert.Equal("docs", LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries).Settings.Home);
        repo.Write("docs/site.yml", "home: showcase\n");
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);
        Assert.Equal("showcase", site.Settings.Home);
        Assert.True(site.Settings.IsShowcase);
    }

    [Fact]
    public void An_unknown_home_is_an_error_that_lists_the_homes()
    {
        using var repo = new FakeRepository().Write("README.md", "# Demo\n\nText.\n").Write("docs/site.yml", "home: carousel\n");
        var error = Assert.Throws<ContentException>(() => LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries));
        Assert.Contains("carousel", error.Message);
        Assert.Contains("docs, showcase", error.Message);
    }

    [Fact]
    public void Every_library_has_exactly_one_line_and_every_line_is_a_library()
    {
        var libraries = RealLibraries();
        Assert.Equal(libraries.Items.Select(l => l.Repo).Order(), NetworkData.Lines.Select(l => l.Repo).Order());
        Assert.Equal(NetworkData.Lines.Count, NetworkData.Lines.Select(l => l.Key).Distinct().Count());
        Assert.Equal(NetworkData.Lines.Count, NetworkData.Lines.Select(l => l.Legs[0].Heading).Distinct().Count());
    }

    [Fact]
    public void Every_line_has_stations_with_a_feature_and_a_text()
    {
        foreach (var line in NetworkData.Lines)
        {
            Assert.InRange(line.Stations.Count, 3, 6);
            Assert.Equal(line.Stations.Count, line.Stations.Select(s => s.Label).Distinct().Count());
            Assert.All(line.Stations, s => Assert.False(string.IsNullOrWhiteSpace(s.FeatureTitle) || string.IsNullOrWhiteSpace(s.Text)));
        }
    }

    [Fact]
    public void Every_station_is_a_feature_of_the_readme_of_its_library_and_the_tagline_is_the_one_of_site_yml()
    {
        if (Workspace() is not { } workspace)
        {
            Assert.Skip("The repositories are not checked out side by side.");
            return;
        }
        foreach (var line in NetworkData.Lines)
        {
            var readme = File.ReadAllText(Path.Combine(workspace, line.Repo, "README.md"));
            var features = Regex.Matches(readme, @"^- \*\*(?<title>[^*]+)\*\*", RegexOptions.Multiline).Select(m => m.Groups["title"].Value.TrimEnd(':')).ToList();
            foreach (var station in line.Stations)
            {
                Assert.True(features.Contains(station.FeatureTitle), $"{line.Repo}: '{station.FeatureTitle}' is not a feature title of its README.");
            }
            Assert.Contains("tagline: " + line.Tagline, File.ReadAllText(Path.Combine(workspace, line.Repo, "docs", "site.yml")));
        }
    }

    [Fact]
    public void The_stations_stand_on_the_straight_stretches_of_their_line_inside_the_map()
    {
        var layout = NetworkLayout.Build(RealLibraries());
        Assert.Equal(NetworkData.Lines.Count, layout.Lines.Count);
        foreach (var line in layout.Lines)
        {
            Assert.Equal(line.Line.Stations.Count, line.Stations.Count);
            Assert.True(line.Stations.Zip(line.Stations.Skip(1)).All(p => p.Second.Distance > p.First.Distance), $"{line.Key}: stations out of order");
            foreach (var station in line.Stations)
            {
                Assert.True(DistanceToRoute(line.Route, station.At) < 0.01, $"{line.Key}/{station.Station.Label} is off its line");
                Assert.True(Math.Abs(station.At.X) < NetworkLayout.Width / 2 && Math.Abs(station.At.Y) < NetworkLayout.Height / 2);
                Assert.True(Math.Sqrt(station.At.X * station.At.X + station.At.Y * station.At.Y) > NetworkLayout.InterchangeRadius + 10, $"{line.Key}/{station.Station.Label} is under the interchange");
            }
            Assert.InRange(line.Badge.X, -NetworkLayout.Width / 2, NetworkLayout.Width / 2 - line.Badge.Width);
            Assert.InRange(line.Badge.Y, -NetworkLayout.Height / 2, NetworkLayout.Height / 2 - line.Badge.Height);
        }
    }

    [Fact]
    public void The_text_of_every_badge_has_at_least_4_5_to_1_on_its_fill()
    {
        foreach (var line in NetworkLayout.Build(RealLibraries()).Lines)
        {
            Assert.True(AccentColor.Contrast(line.Badge.Fill, line.Badge.OnFill) >= 4.5, $"{line.Key}: {line.Badge.OnFill} on {line.Badge.Fill}");
        }
    }

    [Fact]
    public void The_journey_has_keyframes_for_the_camera_and_every_line()
    {
        var layout = NetworkLayout.Build(RealLibraries());
        var css = layout.JourneyCss();
        Assert.Contains("@keyframes journey-camera", css);
        foreach (var line in layout.Lines)
        {
            Assert.Contains($"@keyframes journey-line-{line.Key}", css);
            Assert.Contains($"@keyframes journey-rider-{line.Key}", css);
        }
    }

    [Fact]
    public void No_two_station_labels_overlap_anywhere_on_the_map()
    {
        var boxes = NetworkLayout.Build(RealLibraries()).Lines
            .SelectMany(l => l.Stations.Select(st => (Name: $"{l.Key}/{st.Station.Label}", Box: st.LabelBox())))
            .ToList();
        for (var i = 0; i < boxes.Count; i++)
        {
            for (var j = i + 1; j < boxes.Count; j++)
            {
                Assert.False(Overlap(boxes[i].Box, boxes[j].Box), $"The labels {boxes[i].Name} and {boxes[j].Name} overlap.");
            }
        }
    }

    [Fact]
    public void The_framed_view_of_the_close_holds_every_badge_and_label_and_the_compact_view_every_stub()
    {
        var layout = NetworkLayout.Build(RealLibraries());
        var v = layout.FramedViewBox().Split(' ').Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
        bool Inside(Pt p) => p.X >= v[0] && p.X <= v[0] + v[2] && p.Y >= v[1] && p.Y <= v[1] + v[3];
        foreach (var line in layout.Lines)
        {
            Assert.True(Inside(new Pt(line.Badge.X, line.Badge.Y)) && Inside(new Pt(line.Badge.X + line.Badge.Width, line.Badge.Y + line.Badge.Height)), $"{line.Key}: badge outside the frame");
            Assert.All(line.Stations.SelectMany(st => st.LabelBox()), p => Assert.True(Inside(p), $"{line.Key}: a label is outside the frame"));
        }
        var c = NetworkLayout.CompactViewBox.Split(' ').Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
        Assert.Equal(layout.Lines.Count, layout.CompactStubs().Count);
        foreach (var stub in layout.CompactStubs())
        {
            Assert.True(stub.Badge.X >= c[0] && stub.Badge.X + stub.Badge.Width <= c[0] + c[2] && stub.Badge.Y >= c[1] && stub.Badge.Y + stub.Badge.Height <= c[1] + c[3], $"{stub.Line.Key}: compact badge outside the view");
        }
    }

    // Separating axis test of two oriented rectangles.
    private static bool Overlap(IReadOnlyList<Pt> a, IReadOnlyList<Pt> b)
    {
        foreach (var poly in new[] { a, b })
        {
            for (var i = 0; i < 4; i++)
            {
                var (p, q) = (poly[i], poly[(i + 1) % 4]);
                var (nx, ny) = (q.Y - p.Y, p.X - q.X);
                double Min(IReadOnlyList<Pt> r) => r.Min(t => t.X * nx + t.Y * ny);
                double Max(IReadOnlyList<Pt> r) => r.Max(t => t.X * nx + t.Y * ny);
                if (Max(a) < Min(b) || Max(b) < Min(a))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static double DistanceToRoute(IReadOnlyList<Pt> route, Pt p)
    {
        var best = double.MaxValue;
        for (var i = 0; i + 1 < route.Count; i++)
        {
            var (a, b) = (route[i], route[i + 1]);
            var (dx, dy) = (b.X - a.X, b.Y - a.Y);
            var t = Math.Clamp(((p.X - a.X) * dx + (p.Y - a.Y) * dy) / (dx * dx + dy * dy), 0, 1);
            best = Math.Min(best, Math.Sqrt(Math.Pow(p.X - (a.X + t * dx), 2) + Math.Pow(p.Y - (a.Y + t * dy), 2)));
        }
        return best;
    }
}
