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

    /// <summary>The authored boards a <c>site.yml</c> can name in <c>demo.board</c>: the components of <c>Components/Boards</c>.</summary>
    public static readonly IReadOnlyList<string> Boards = ["table", "form"];

    /// <summary>The authored previews a simple hero can show as its result (<c>demo.result.preview</c>): drawn components of <c>Components/Boards/QuickBoard.razor</c>.</summary>
    public static readonly IReadOnlyList<string> Previews = ["ui-settings", "display-stat", "spreadsheet-rows"];

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
        RenderedMarkdown RenderText(string text, string repoPath, string slug)
        {
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

        string ReadText(string repoPath) =>
            File.ReadAllText(Path.Combine(repoRoot, repoPath.Replace('/', Path.DirectorySeparatorChar))).TrimStart('\uFEFF').Replace("\r\n", "\n");

        RenderedMarkdown Render(string repoPath, string slug) => RenderText(ReadText(repoPath), repoPath, slug);

        // The README is the home page, but its Features, Packages and ecosystem sections are set by the home page in its own layout, so what is rendered
        // as the body is the README without them. The home page gives their ids to the blocks that replace them, so links to them still land.
        var readmeText = BadgeLine().Replace(ReadText("README.md"), "");
        var (intro, sections) = SplitSections(readmeText);
        var homeContent = ParseHome(sections, RenderText, name, libraries, out var replaced);
        var bodyText = intro + string.Concat(sections.Where(section => !replaced.Contains(section.Id)).Select(section => section.Text));
        var readme = RenderText(bodyText, "README.md", "");
        var title = config.Title ?? readme.Title ?? SlugTitle(name);
        var tagline = config.Tagline ?? Shorten(readme.Summary, 200) ?? title;
        var description = config.Description ?? readme.Summary ?? tagline;
        var library = libraries.Find(name);
        var accent = config.Accent ?? library?.Accent ?? "#10b981";
        var texture = config.Texture ?? library?.Texture ?? "blueprint-grid";
        var headline = config.Headline.Count > 0 ? config.Headline : [title, "documentation"];
        if (config.Demo?.Board is { } board && !Boards.Contains(board))
        {
            throw new ContentException($"{configPath}: '{board}' is not a board. The boards are: {string.Join(", ", Boards)}.");
        }
        if (config.Demo?.Preview is { } preview && !Previews.Contains(preview))
        {
            throw new ContentException($"{configPath}: '{preview}' is not a preview. The previews are: {string.Join(", ", Previews)}.");
        }
        var settings = new SiteSettings(name, title, tagline, description, config.Install, github, config.Branch, config.Packages, accent, texture, headline,
            config.Lead ?? tagline, HeroInstall(config.Install, title, config.Packages), config.Demo);

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
            var (lead, body) = SplitLead(result.Html, entry?.Description);
            return new DocPage(slug, path, pageTitle, entry?.NavTitle ?? NavTitle(pageTitle, title), entry?.Description, meta, body, result.Headings, result.Text,
                entry?.TocDepth ?? 3, entry?.IndexText ?? true, group, lead);
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
        return new DocsSite(settings, home, ordered, navigation, search, problems.Distinct().ToList(), homeContent);
    }


    [GeneratedRegex(@"\A\s*<p>(?<lead>.*?)</p>\s*", RegexOptions.Singleline)]
    private static partial Regex LeadParagraph();

    // The paragraph under the title of a page is its lead: the description of site.yml when there is one, else the first paragraph, which leaves the body.
    // A paragraph that is long, or that holds more than a sentence or two of markup, stays in the body.
    private static (string? Lead, string Body) SplitLead(string html, string? description)
    {
        if (description is { Length: > 0 })
        {
            return (System.Net.WebUtility.HtmlEncode(description), html);
        }
        if (LeadParagraph().Match(html) is { Success: true } match && Tags().Replace(match.Groups["lead"].Value, "").Length <= 320)
        {
            return (match.Groups["lead"].Value, html[match.Length..]);
        }
        return (null, html);
    }

    private sealed record Section(string Id, string Heading, string Text);

    // The README as the text before its first "## " heading and the sections from there on, each with its heading line. A "## " inside a code fence is not a heading.
    private static (string Intro, List<Section> Sections) SplitSections(string text)
    {
        var intro = new System.Text.StringBuilder();
        var sections = new List<(string Heading, System.Text.StringBuilder Text)>();
        var fence = false;
        foreach (var line in text.Split('\n'))
        {
            if (line.StartsWith("```", StringComparison.Ordinal) || line.StartsWith("~~~", StringComparison.Ordinal))
            {
                fence = !fence;
            }
            if (!fence && line.StartsWith("## ", StringComparison.Ordinal))
            {
                sections.Add((line[3..].Trim(), new System.Text.StringBuilder()));
            }
            (sections.Count == 0 ? intro : sections[^1].Text).Append(line).Append('\n');
        }
        return (intro.ToString(), [.. sections.Select(s => new Section(s.Heading.ToLowerInvariant(), s.Heading, s.Text.ToString()))]);
    }

    [GeneratedRegex("""<li>\s*<strong>(?<title>.*?)</strong>:?\s*(?<body>.*?)</li>""", RegexOptions.Singleline)]
    private static partial Regex FeatureItem();

    [GeneratedRegex("""<tr>\s*<td[^>]*>(?<first>.*?)</td>\s*<td[^>]*>(?<second>.*?)</td>\s*</tr>""", RegexOptions.Singleline)]
    private static partial Regex TableRow();

    [GeneratedRegex("""<a\b[^>]*\bhref="(?<href>[^"]*)"[^>]*>(?<text>.*?)</a>""", RegexOptions.Singleline)]
    private static partial Regex Anchor();

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex Tags();

    // Reads the Features list, the Packages table and the ecosystem table of the README as data. A section that gives nothing stays in the body of the page.
    private static HomeContent ParseHome(List<Section> sections, Func<string, string, string, RenderedMarkdown> render, string repo, Libraries libraries, out HashSet<string> replaced)
    {
        replaced = [];
        string? Html(string id, string heading)
        {
            var section = sections.FirstOrDefault(s => s.Id == id);
            return section is null ? null : render(section.Text, "README.md", "").Html;
        }
        static string PlainText(string html) => System.Net.WebUtility.HtmlDecode(Tags().Replace(html, "")).Trim();

        var features = new List<HomeFeature>();
        if (Html("features", "Features") is { } featuresHtml)
        {
            foreach (Match item in FeatureItem().Matches(featuresHtml))
            {
                features.Add(new HomeFeature(PlainText(item.Groups["title"].Value), item.Groups["body"].Value.Trim()));
            }
            if (features.Count > 0)
            {
                replaced.Add("features");
            }
        }

        var packages = new List<HomePackage>();
        if (Html("packages", "Packages") is { } packagesHtml)
        {
            foreach (Match row in TableRow().Matches(packagesHtml))
            {
                var id = PlainText(row.Groups["first"].Value);
                if (id.Length == 0 || !id.All(c => char.IsLetterOrDigit(c) || c is '.' or '_' or '-'))
                {
                    continue;
                }
                var description = row.Groups["second"].Value.Trim();
                var layer = id.EndsWith(".Core", StringComparison.OrdinalIgnoreCase) ? "Core"
                    : PlainText(description).Contains("Blazor", StringComparison.OrdinalIgnoreCase) ? "Blazor"
                    : "Add-on";
                packages.Add(new HomePackage(id, layer, description, $"dotnet add package {id} --prerelease", "https://www.nuget.org/packages/" + Uri.EscapeDataString(id)));
            }
            if (packages.Count > 0)
            {
                replaced.Add("packages");
            }
        }

        var ecosystem = new List<HomeLibrary>();
        if (Html("klyroo ecosystem", "Klyroo ecosystem") is { } ecosystemHtml)
        {
            foreach (Match row in TableRow().Matches(ecosystemHtml))
            {
                if (Anchor().Match(row.Groups["first"].Value) is not { Success: true } link)
                {
                    continue;
                }
                var libraryName = PlainText(link.Groups["text"].Value);
                var known = libraries.Items.FirstOrDefault(l => l.Name.Equals(libraryName, StringComparison.OrdinalIgnoreCase));
                ecosystem.Add(new HomeLibrary(libraryName, link.Groups["href"].Value, row.Groups["second"].Value.Trim(), known?.Accent, known is not null && known.Repo.Equals(repo, StringComparison.OrdinalIgnoreCase)));
            }
            if (ecosystem.Count > 0)
            {
                replaced.Add("klyroo ecosystem");
            }
        }
        return new HomeContent(features, packages, ecosystem);
    }

    // The command of the copyable chip: the line of "install" for the main package (the one named like the library), else its first line, without a trailing "# comment".
    private static string? HeroInstall(string? install, string title, IReadOnlyList<string> packages)
    {
        var lines = (install ?? "").Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(l => Regex.Replace(l, @"\s+#.*$", "")).Where(l => l.Length > 0).ToList();
        var main = lines.FirstOrDefault(l => l.Contains("package " + title + " ", StringComparison.OrdinalIgnoreCase) || l.EndsWith("package " + title, StringComparison.OrdinalIgnoreCase));
        return main ?? lines.FirstOrDefault() ?? (packages.Count > 0 ? $"dotnet add package {packages[^1]} --prerelease" : null);
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
