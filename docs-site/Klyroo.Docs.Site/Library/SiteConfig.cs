using Klyroo.Docs.Site.Content;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Klyroo.Docs.Site.Library;

/// <summary>One entry of the navigation of <c>docs/site.yml</c>: a page, or a group of entries.</summary>
/// <param name="Page">The page, as the file name below <c>docs/</c> without <c>.md</c> (<c>guides</c>, <c>reference/types</c>). Null for a group.</param>
/// <param name="Group">The title of a group of entries. Null for a page.</param>
/// <param name="Title">Replaces the title of the page (its first <c>#</c> heading): the <c>h1</c>, the title of the browser tab and the search.</param>
/// <param name="NavTitle">The text of the link in the navigation, when it should differ from the title.</param>
/// <param name="Description">The line under the title of the page. Without it nothing is shown there; the meta description is the first paragraph.</param>
/// <param name="TocDepth">The deepest heading level of the table of contents and of the search entries of the page (2 to 6, 3 by default).</param>
/// <param name="IndexText">False keeps the text of the page out of the search and leaves its headings: for a page of reference, where the types are the entries.</param>
/// <param name="Children">The entries of a group.</param>
public sealed record NavEntry(string? Page, string? Group, string? Title, string? NavTitle, string? Description, int? TocDepth, bool? IndexText, IReadOnlyList<NavEntry>? Children);

/// <summary>
/// The demonstration of the home page. <paramref name="Board"/> names an authored board (<c>table</c>, <c>form</c>, <c>query</c>, <c>spreadsheet</c>, <c>dashboard</c>, <c>display</c>, <c>agent</c>, <c>ui</c>: a component in <c>Components/Boards</c>) with chips
/// and several results. Without a board, <paramref name="Code"/> is shown in a code panel and one result panel shows <paramref name="Output"/> (lines of text) or
/// the authored <paramref name="Preview"/>, joined by a leader line that starts at the <paramref name="Focus"/> lines.
/// </summary>
public sealed record DemoConfig(string? Board, string? File, string Language, string? Code, (int From, int To)? Focus, string? ResultTitle, string? Output, string? Preview);

/// <summary>
/// What <c>docs/site.yml</c> of a library says about its documentation site. Every key is optional: the title is the first heading of the README, the tagline
/// is the first paragraph of the README, and the pages follow in the order of their file names.
/// </summary>
public sealed class SiteConfig
{
    private static readonly string[] Keys = ["title", "tagline", "description", "headline", "lead", "install", "github", "branch", "packages", "accent", "texture", "demo", "nav", "repo", "home"];

    /// <summary>The ground textures a site can have, one per library material (see <c>Styles/ground.css</c>).</summary>
    public static readonly IReadOnlyList<string> Textures = ["blueprint-grid", "dot-matrix", "ruled-rows", "baseline-grid", "chart-ticks", "halftone", "cell-grid", "node-dots"];

    /// <summary>The home pages a site can have: the documentation home (a hero with its demonstration board) or the showcase of the Klyroo network (<c>Components/Showcase</c>).</summary>
    public static readonly IReadOnlyList<string> Homes = ["docs", "showcase"];

    private static readonly string[] DemoKeys = ["board", "file", "language", "code", "focus", "result"];
    private static readonly string[] ResultKeys = ["title", "output", "preview"];

    /// <summary>The name of the library (<c>Klyroo.Table</c>): the brand in the header and the title of the home page.</summary>
    public string? Title { get; init; }

    /// <summary>The line under the title on the home page.</summary>
    public string? Tagline { get; init; }

    /// <summary>The meta description of the home page and of the pages without a paragraph of their own.</summary>
    public string? Description { get; init; }

    /// <summary>The headline of the home page: one or two lines, the last one in the accent color. Without it the title and the tagline.</summary>
    public IReadOnlyList<string> Headline { get; init; } = [];

    /// <summary>The paragraph under the headline of the home page. Without it the tagline.</summary>
    public string? Lead { get; init; }

    /// <summary>The ground texture behind the header and the hero (one of <see cref="Textures"/>). Without it the one of the library in <c>libraries.yml</c>.</summary>
    public string? Texture { get; init; }

    /// <summary>What the home page demonstrates: an authored board, or the code and the result of a simple hero. Without it the home page has no demonstration.</summary>
    public DemoConfig? Demo { get; init; }

    /// <summary>The command shown next to the title of the home page, usually <c>dotnet add package ... --prerelease</c>.</summary>
    public string? Install { get; init; }

    /// <summary>The address of the repository, without a trailing slash. Without it: <c>https://github.com/klyroo/&lt;repo&gt;</c>.</summary>
    public string? Github { get; init; }

    /// <summary>The branch that links to files of the repository point at.</summary>
    public string Branch { get; init; } = "main";

    /// <summary>The NuGet package ids of the library, each linked from the header.</summary>
    public IReadOnlyList<string> Packages { get; init; } = [];

    /// <summary>The accent color of the site, as <c>#rrggbb</c>. The text variants that pass the contrast rules in light and dark mode are computed from it. Without it the one of the library in <c>libraries.yml</c>.</summary>
    public string? Accent { get; init; }

    /// <summary>The name of the repository, which is the address of the site (<c>/&lt;repo&gt;/</c>). Without it the name of the folder of the repository.</summary>
    public string? Repo { get; init; }

    /// <summary>Which home page the site has, one of <see cref="Homes"/>. Without it <c>docs</c>.</summary>
    public string Home { get; init; } = "docs";

    /// <summary>The pages in the order of the navigation, with their groups. Pages that are not listed follow, by file name.</summary>
    public IReadOnlyList<NavEntry> Nav { get; init; } = [];

    /// <summary>Reads a <c>site.yml</c>. A missing file is an empty configuration; a key that is not known, or a value of the wrong kind, is an error that names the file.</summary>
    public static SiteConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            return new SiteConfig();
        }
        Dictionary<object, object?>? root;
        try
        {
            root = new DeserializerBuilder().Build().Deserialize<Dictionary<object, object?>>(File.ReadAllText(path));
        }
        catch (YamlException error)
        {
            throw new ContentException($"{path}: invalid YAML (line {error.Start.Line}): {error.InnerException?.Message ?? error.Message}", error);
        }
        root ??= [];
        foreach (var key in root.Keys)
        {
            if (!Keys.Contains(key.ToString(), StringComparer.Ordinal))
            {
                throw new ContentException($"{path}: '{key}' is not a setting of the site. The settings are: {string.Join(", ", Keys)}.");
            }
        }
        string? Text(string key) => root.TryGetValue(key, out var value) ? value?.ToString()?.Trim() is { Length: > 0 } text ? text : null : null;

        return new SiteConfig
        {
            Title = Text("title"),
            Tagline = Text("tagline"),
            Description = Text("description"),
            Install = Text("install"),
            Github = Text("github")?.TrimEnd('/'),
            Branch = Text("branch") ?? "main",
            Accent = AccentText(Text("accent"), path),
            Headline = HeadlineLines(root.GetValueOrDefault("headline"), path),
            Lead = Text("lead"),
            Texture = TextureText(Text("texture"), path),
            Demo = root.TryGetValue("demo", out var demo) && demo is not null ? ParseDemo(demo, path) : null,
            Repo = Text("repo"),
            Home = HomeText(Text("home"), path),
            Packages = root.TryGetValue("packages", out var packages) ? List(packages, "packages", path).Select(p => p?.ToString() ?? "").Where(p => p.Length > 0).ToList() : [],
            Nav = root.TryGetValue("nav", out var nav) ? ParseNav(nav, "nav", path) : [],
        };
    }

    private static string HomeText(string? home, string path) =>
        home is null ? "docs"
        : Homes.Contains(home, StringComparer.Ordinal) ? home
        : throw new ContentException($"{path}: '{home}' is not a home page. The home pages are: {string.Join(", ", Homes)}.");

    private static string? AccentText(string? accent, string path)
    {
        if (accent is not null && !AccentColor.TryParse(accent, out _))
        {
            throw new ContentException($"{path}: accent must be a color written as #rrggbb, not '{accent}'.");
        }
        return accent?.ToLowerInvariant();
    }

    private static string? TextureText(string? texture, string path) =>
        texture is null || Textures.Contains(texture, StringComparer.Ordinal)
            ? texture
            : throw new ContentException($"{path}: '{texture}' is not a texture. The textures are: {string.Join(", ", Textures)}.");

    // Two lines in one block scalar ("headline: |"), or a list of two strings.
    private static IReadOnlyList<string> HeadlineLines(object? value, string path)
    {
        var lines = value switch
        {
            null => [],
            string text => text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
            List<object> list => [.. list.Select(i => i?.ToString()?.Trim() ?? "").Where(l => l.Length > 0)],
            _ => throw new ContentException($"{path}: 'headline' is one or two lines of text."),
        };
        return lines.Length <= 2 ? lines : throw new ContentException($"{path}: 'headline' has {lines.Length} lines. It has at most two.");
    }

    private static DemoConfig ParseDemo(object value, string path)
    {
        if (value is not Dictionary<object, object> map)
        {
            throw new ContentException($"{path}: 'demo' is a map with {string.Join(", ", DemoKeys)}.");
        }
        var demo = map.ToDictionary(p => p.Key.ToString() ?? "", p => p.Value, StringComparer.Ordinal);
        foreach (var key in demo.Keys.Where(k => !DemoKeys.Contains(k, StringComparer.Ordinal)))
        {
            throw new ContentException($"{path}: '{key}' is not a setting of 'demo'. The settings are: {string.Join(", ", DemoKeys)}.");
        }
        string? Text(string key) => demo.TryGetValue(key, out var v) ? v?.ToString()?.TrimEnd() is { Length: > 0 } text ? text : null : null;

        string? resultTitle = null, output = null, preview = null;
        if (demo.GetValueOrDefault("result") is { } result)
        {
            if (result is not Dictionary<object, object> resultMap)
            {
                throw new ContentException($"{path}: 'demo.result' is a map with {string.Join(", ", ResultKeys)}.");
            }
            foreach (var key in resultMap.Keys.Select(k => k.ToString() ?? "").Where(k => !ResultKeys.Contains(k, StringComparer.Ordinal)))
            {
                throw new ContentException($"{path}: '{key}' is not a setting of 'demo.result'. The settings are: {string.Join(", ", ResultKeys)}.");
            }
            string? ResultText(string key) => resultMap.TryGetValue(key, out var v) ? v?.ToString()?.TrimEnd() is { Length: > 0 } text ? text : null : null;
            resultTitle = ResultText("title");
            output = ResultText("output");
            preview = ResultText("preview");
        }

        var board = Text("board");
        var code = Text("code");
        if (board is null && code is null)
        {
            throw new ContentException($"{path}: 'demo' needs a 'board' (an authored board) or a 'code' (the code of a simple hero).");
        }
        if (board is not null && code is not null)
        {
            throw new ContentException($"{path}: 'demo' has a 'board' and a 'code'. Use one of them.");
        }
        if (board is null && output is null && preview is null)
        {
            throw new ContentException($"{path}: 'demo' has a 'code' and no 'result' (an 'output' or a 'preview').");
        }
        (int From, int To)? focus = null;
        if (Text("focus") is { } range)
        {
            var parts = range.Split('-', 2);
            if (!int.TryParse(parts[0], out var from) || !int.TryParse(parts[^1], out var to) || from < 1 || to < from)
            {
                throw new ContentException($"{path}: demo.focus is a line number or a range of lines of the code, like 4-9, not '{range}'.");
            }
            focus = (from, to);
        }
        return new DemoConfig(board?.ToLowerInvariant(), Text("file"), Text("language") ?? "csharp", code, focus, resultTitle, output, preview);
    }

    private static IReadOnlyList<object?> List(object? value, string name, string path) =>
        value switch
        {
            null => [],
            List<object> list => list!,
            _ => throw new ContentException($"{path}: '{name}' must be a list."),
        };

    private static List<NavEntry> ParseNav(object? value, string name, string path)
    {
        var entries = new List<NavEntry>();
        foreach (var item in List(value, name, path))
        {
            switch (item)
            {
                case string page:
                    entries.Add(new NavEntry(page.Trim(), null, null, null, null, null, null, null));
                    break;
                case Dictionary<object, object> dictionary:
                    var map = dictionary.ToDictionary(p => p.Key.ToString() ?? "", p => p.Value, StringComparer.Ordinal);
                    foreach (var key in map.Keys)
                    {
                        if (key is not ("page" or "group" or "pages" or "title" or "navTitle" or "description" or "tocDepth" or "indexText"))
                        {
                            throw new ContentException($"{path}: '{key}' is not a setting of a navigation entry. The settings are: page, group, pages, title, navTitle, description, tocDepth, indexText.");
                        }
                    }
                    string? Text(string key) => map.TryGetValue(key, out var v) ? v?.ToString()?.Trim() is { Length: > 0 } text ? text : null : null;
                    int? tocDepth = null;
                    if (Text("tocDepth") is { } depth)
                    {
                        if (!int.TryParse(depth, out var parsed) || parsed is < 2 or > 6)
                        {
                            throw new ContentException($"{path}: tocDepth must be a number from 2 to 6, not '{depth}'.");
                        }
                        tocDepth = parsed;
                    }
                    bool? indexText = null;
                    if (Text("indexText") is { } index)
                    {
                        indexText = bool.TryParse(index, out var flag) ? flag : throw new ContentException($"{path}: indexText must be true or false, not '{index}'.");
                    }
                    if (Text("group") is { } group)
                    {
                        entries.Add(new NavEntry(null, group, null, null, null, null, null, ParseNav(map.GetValueOrDefault("pages"), "pages", path)));
                    }
                    else if (Text("page") is { } page)
                    {
                        entries.Add(new NavEntry(page, null, Text("title"), Text("navTitle"), Text("description"), tocDepth, indexText, null));
                    }
                    else
                    {
                        throw new ContentException($"{path}: an entry of the navigation needs a 'page' or a 'group'.");
                    }
                    break;
                default:
                    throw new ContentException($"{path}: an entry of the navigation is a page name or a map with 'page' or 'group'.");
            }
        }
        return entries;
    }
}
