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
/// What <c>docs/site.yml</c> of a library says about its documentation site. Every key is optional: the title is the first heading of the README, the tagline
/// is the first paragraph of the README, and the pages follow in the order of their file names.
/// </summary>
public sealed class SiteConfig
{
    private static readonly string[] Keys = ["title", "tagline", "description", "install", "github", "branch", "packages", "accent", "nav", "repo"];

    /// <summary>The name of the library (<c>Klyroo.Table</c>): the brand in the header and the title of the home page.</summary>
    public string? Title { get; init; }

    /// <summary>The line under the title on the home page.</summary>
    public string? Tagline { get; init; }

    /// <summary>The meta description of the home page and of the pages without a paragraph of their own.</summary>
    public string? Description { get; init; }

    /// <summary>The command shown next to the title of the home page, usually <c>dotnet add package ... --prerelease</c>.</summary>
    public string? Install { get; init; }

    /// <summary>The address of the repository, without a trailing slash. Without it: <c>https://github.com/klyroo/&lt;repo&gt;</c>.</summary>
    public string? Github { get; init; }

    /// <summary>The branch that links to files of the repository point at.</summary>
    public string Branch { get; init; } = "main";

    /// <summary>The NuGet package ids of the library, each linked from the header.</summary>
    public IReadOnlyList<string> Packages { get; init; } = [];

    /// <summary>The primary color: the name of a Tailwind palette (<c>blue</c>, <c>emerald</c>...). Without it the Klyroo colors.</summary>
    public string? Accent { get; init; }

    /// <summary>The name of the repository, which is the address of the site (<c>/&lt;repo&gt;/</c>). Without it the name of the folder of the repository.</summary>
    public string? Repo { get; init; }

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
            Accent = Text("accent"),
            Repo = Text("repo"),
            Packages = root.TryGetValue("packages", out var packages) ? List(packages, "packages", path).Select(p => p?.ToString() ?? "").Where(p => p.Length > 0).ToList() : [],
            Nav = root.TryGetValue("nav", out var nav) ? ParseNav(nav, "nav", path) : [],
        };
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
