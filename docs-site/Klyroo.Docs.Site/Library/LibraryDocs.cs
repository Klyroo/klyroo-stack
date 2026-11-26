using System.Text.RegularExpressions;
using Klyroo.Docs.Site.Content;
using Klyroo.UI;

namespace Klyroo.Docs.Site.Library;

/// <summary>
/// Reads the documentation of a library from its repository: the <c>README.md</c> is the home page, every <c>.md</c> file below <c>docs/</c> is a page, and
/// <c>docs/site.yml</c> (<see cref="SiteConfig"/>) sets the name, the links and the order. The files are not changed and need no front matter: the title of a
/// page is its first <c>#</c> heading, which is taken out of the body so the page has one <c>h1</c>, and the description is its first paragraph.
/// </summary>
public static partial class LibraryDocs
{
    /// <summary>The slug of <c>docs/README.md</c>, the index of the documentation.</summary>
    public const string IndexSlug = "documentation";

    // A line of badges at the top of a README (CI status, NuGet version, license): images loaded from other sites, which the header replaces with links.
    [GeneratedRegex(@"^(?:[ \t]*\[!\[[^\]\n]*\]\([^)\n]*\)\]\([^)\n]*\)[ \t]*)+\r?$\n?", RegexOptions.Multiline)]
    private static partial Regex BadgeLine();

    public static DocsSite Load(string repoRoot, string? repoName, Libraries libraries, int searchTextLimit = 2000)
    {
        repoRoot = Path.GetFullPath(repoRoot);
        var readmePath = Path.Combine(repoRoot, "README.md");
        if (!File.Exists(readmePath))
        {
            throw new ContentException($"{readmePath}: the repository has no README.md, which is the home page of the site.");
        }
        var docs = Path.Combine(repoRoot, "docs");
        var configPath = Path.Combine(docs, "site.yml");
        var config = SiteConfig.Load(configPath);
        var name = repoName ?? config.Repo ?? Path.GetFileName(repoRoot.TrimEnd(Path.DirectorySeparatorChar));
        var github = config.Github ?? libraries.GithubUrl(name);

        // Every page by its file, from the root of the repository: the links of the files are resolved against this.
        var pagePaths = new Dictionary<string, string>(StringComparer.Ordinal) { ["README.md"] = "" };
        var files = new List<(string Path, string Slug)>();
        if (Directory.Exists(docs))
        {
            foreach (var file in Walk(docs).Order(StringComparer.Ordinal))
            {
                var relative = Path.GetRelativePath(repoRoot, file).Replace('\\', '/');
                var inDocs = relative["docs/".Length..];
                if (inDocs.Equals("README.md", StringComparison.Ordinal))
                {
                    pagePaths[relative] = IndexSlug;
                    files.Add((relative, IndexSlug));
                    continue;
                }
                var slug = string.Join('/', inDocs[..^3].Split('/').Select(ContentUrl.Segment));
                if (slug.Length == 0 || slug.Split('/').Any(s => s.Length == 0))
                {
                    throw new ContentException($"{relative}: the file name has no letter or digit to make an address from.");
                }
                if (slug == IndexSlug)
                {
                    throw new ContentException($"{relative}: the address '{IndexSlug}' is the one of docs/README.md. Rename the file.");
                }
                if (pagePaths.ContainsValue(slug))
                {
                    throw new ContentException($"{relative} and another file both make the address '{slug}'. Rename one of them.");
                }
                pagePaths[relative] = slug;
                files.Add((relative, slug));
            }
        }

        var problems = new List<string>();
        var renderer = new MarkdownRenderer();
        RenderedMarkdown Render(string repoPath, string slug)
        {
            var text = File.ReadAllText(Path.Combine(repoRoot, repoPath.Replace('/', Path.DirectorySeparatorChar))).TrimStart('﻿').Replace("\r\n", "\n");
            if (repoPath == "README.md")
            {
                text = BadgeLine().Replace(text, "");
            }
            var resolver = new LinkResolver(repoRoot, repoPath, pagePaths, github, config.Branch, name, libraries, problems);
            try
            {
                return renderer.Render(text, "", slug.Length == 0 ? "./" : slug + "/", new RenderOptions { TakeTitle = true, ResolveLink = resolver.Resolve });
            }
            catch (Exception error) when (error is not ContentException)
            {
                throw new ContentException($"{repoPath}: {error.Message}", error);
            }
        }

        var readme = Render("README.md", "");
        var title = config.Title ?? readme.Title ?? SlugTitle(name);
        var tagline = config.Tagline ?? Shorten(readme.Summary, 200) ?? title;
        var description = config.Description ?? readme.Summary ?? tagline;
        var settings = new SiteSettings(name, title, tagline, description, config.Install, github, config.Branch, config.Packages, config.Accent);

        var rendered = files.ToDictionary(f => f.Slug, f => (f.Path, Result: Render(f.Path, f.Slug)), StringComparer.Ordinal);

        // The pages in the order of site.yml, then the ones it does not list. The index of the documentation comes first.
        var listed = new HashSet<string>(StringComparer.Ordinal);
        var ordered = new List<DocPage>();
        var navigation = new List<KlyContentNavigationItem>();

        DocPage MakePage(string slug, NavEntry? entry, string? group)
        {
            var (path, result) = rendered[slug];
            var pageTitle = entry?.Title ?? result.Title ?? SlugTitle(slug.Split('/')[^1]);
            var meta = entry?.Description ?? Shorten(result.Summary, 200) ?? description;
            return new DocPage(slug, path, pageTitle, entry?.NavTitle ?? NavTitle(pageTitle, title), entry?.Description, meta, result.Html, result.Headings, result.Text,
                entry?.TocDepth ?? 3, entry?.IndexText ?? true, group);
        }

        KlyContentNavigationItem Item(DocPage page) => new(page.NavTitle) { Href = page.Href, Exact = true };

        if (rendered.ContainsKey(IndexSlug))
        {
            listed.Add(IndexSlug);
            var index = MakePage(IndexSlug, null, null);
            ordered.Add(index);
            navigation.Add(Item(index));
        }

        List<KlyContentNavigationItem> Entries(IReadOnlyList<NavEntry> entries, string? group)
        {
            var items = new List<KlyContentNavigationItem>();
            foreach (var entry in entries)
            {
                if (entry.Group is { } groupTitle)
                {
                    var children = Entries(entry.Children ?? [], groupTitle);
                    if (children.Count > 0)
                    {
                        items.Add(new KlyContentNavigationItem(groupTitle) { Children = children, Open = true });
                    }
                    continue;
                }
                var slug = (entry.Page ?? "").Trim('/');
                if (slug.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    slug = slug[..^3];
                }
                if (!rendered.ContainsKey(slug) || slug == IndexSlug)
                {
                    throw new ContentException($"{configPath}: the navigation lists '{entry.Page}', but there is no docs/{slug}.md. The pages are: {string.Join(", ", rendered.Keys.Where(k => k != IndexSlug).Order())}.");
                }
                if (!listed.Add(slug))
                {
                    throw new ContentException($"{configPath}: the navigation lists '{entry.Page}' twice.");
                }
                var page = MakePage(slug, entry, group);
                ordered.Add(page);
                items.Add(Item(page));
            }
            return items;
        }

        navigation.AddRange(Entries(config.Nav, null));
        foreach (var slug in rendered.Keys.Where(k => !listed.Contains(k)).Order(StringComparer.Ordinal))
        {
            var page = MakePage(slug, null, null);
            ordered.Add(page);
            navigation.Add(Item(page));
        }

        var home = new DocPage("", "README.md", title, title, null, description, readme.Html, readme.Headings, readme.Text, 3, true, null);
        var search = new List<KlyContentSearchPage> { ToSearch(home, tagline, searchTextLimit) };
        search.AddRange(ordered.Select(p => ToSearch(p, p.MetaDescription, searchTextLimit)));
        return new DocsSite(settings, home, ordered, navigation, search, problems.Distinct().ToList());
    }

    private static KlyContentSearchPage ToSearch(DocPage page, string description, int textLimit) =>
        new(page.Title, page.Href)
        {
            Description = Shorten(description, 140),
            Group = page.Group,
            Keywords = page.IndexText ? Snippet(page.Text, textLimit) : null,
            Headings = [.. page.Headings.Where(h => h.Level >= 2 && h.Level <= page.TocDepth).Select(h => new KlyContentSearchHeading(h.Text, h.Id))],
        };

    // "Klyroo.Table API reference" is "API reference" in the navigation of the Klyroo.Table site.
    private static string NavTitle(string pageTitle, string libraryTitle)
    {
        if (pageTitle.StartsWith(libraryTitle + " ", StringComparison.OrdinalIgnoreCase) && pageTitle.Length > libraryTitle.Length + 1)
        {
            var rest = pageTitle[(libraryTitle.Length + 1)..].Trim();
            return char.ToUpperInvariant(rest[0]) + rest[1..];
        }
        return pageTitle;
    }

    private static string SlugTitle(string slug)
    {
        var words = slug.Replace('-', ' ').Replace('_', ' ').Trim();
        return words.Length == 0 ? slug : char.ToUpperInvariant(words[0]) + words[1..];
    }

    private static IEnumerable<string> Walk(string folder)
    {
        foreach (var file in Directory.EnumerateFiles(folder, "*.md"))
        {
            yield return file;
        }
        foreach (var sub in Directory.EnumerateDirectories(folder))
        {
            if (!Path.GetFileName(sub).StartsWith('.'))
            {
                foreach (var file in Walk(sub))
                {
                    yield return file;
                }
            }
        }
    }

    /// <summary>The text cut at the end of a sentence or, failing that, at a word, within <paramref name="limit"/> characters.</summary>
    private static string? Shorten(string? text, int limit)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }
        text = text.Trim();
        if (text.Length <= limit)
        {
            return text;
        }
        var sentence = text.LastIndexOfAny(['.', '!', '?'], limit - 1);
        if (sentence > limit / 2)
        {
            return text[..(sentence + 1)];
        }
        var cut = text.LastIndexOf(' ', limit - 1);
        return text[..(cut > 0 ? cut : limit)].TrimEnd(',', ';', ':') + "…";
    }

    private static string? Snippet(string text, int limit)
    {
        if (limit <= 0 || text.Length == 0)
        {
            return null;
        }
        if (text.Length <= limit)
        {
            return text;
        }
        var cut = text.LastIndexOf(' ', limit);
        return text[..(cut > 0 ? cut : limit)];
    }
}
