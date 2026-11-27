using Klyroo.Docs.Site.Library;
using Klyroo.UI;

namespace Klyroo.Docs.Site.Content;

/// <summary>A page of the documentation of a library, rendered.</summary>
/// <param name="Slug">The address of the page below the site, without a leading slash: <c>guides</c> for <c>docs/guides.md</c>. The home page has an empty slug.</param>
/// <param name="SourcePath">The file the page was read from, from the root of the repository.</param>
/// <param name="Title">The title of the page: its first <c>#</c> heading, or the one of <c>site.yml</c>.</param>
/// <param name="NavTitle">The text of the link in the navigation.</param>
/// <param name="Description">The line shown under the title. Null when <c>site.yml</c> gives none: the first paragraph of the page is already there.</param>
/// <param name="MetaDescription">The description for search engines and the search: the one of <c>site.yml</c>, or the first paragraph of the page.</param>
/// <param name="TocDepth">The deepest heading level of the table of contents and of the search entries.</param>
/// <param name="LeadHtml">The larger paragraph under the title: the description of <c>site.yml</c>, or else the first paragraph of the page, which is then not in <paramref name="Html"/>.</param>
public sealed record DocPage(
    string Slug,
    string SourcePath,
    string Title,
    string NavTitle,
    string? Description,
    string MetaDescription,
    string Html,
    IReadOnlyList<MarkdownHeading> Headings,
    string Text,
    int TocDepth,
    bool IndexText,
    string? Group,
    string? LeadHtml = null)
{
    /// <summary>
    /// The address to link to: relative to the <c>&lt;base href&gt;</c> of the site, so it is right below a path base too. A page is a folder with an <c>index.html</c>
    /// in the export, so its address ends with a slash: a static server answers it directly, where it would redirect the address without one.
    /// </summary>
    public string Href => Slug.Length == 0 ? "./" : Slug + "/";

    /// <summary>The table of contents: the headings from <c>h2</c> down to <see cref="TocDepth"/>, with the ids that <see cref="KlyContentToc"/> links to.</summary>
    public IReadOnlyList<KlyContentTocLink> Toc =>
        [.. Headings.Where(h => h.Level >= 2 && h.Level <= TocDepth).Select(h => new KlyContentTocLink(h.Id, h.Text) { Level = h.Level })];
}

/// <summary>The documentation of one library, read once: the home page, the pages, the navigation and the search index.</summary>
public sealed class DocsSite
{
    private readonly Dictionary<string, DocPage> _bySlug;
    private readonly Dictionary<string, int> _position;

    public DocsSite(
        SiteSettings settings,
        DocPage home,
        IReadOnlyList<DocPage> pages,
        IReadOnlyList<KlyContentNavigationItem> navigation,
        IReadOnlyList<KlyContentSearchPage> search,
        IReadOnlyList<string> problems,
        HomeContent? homeContent = null)
    {
        HomeContent = homeContent ?? new HomeContent([], [], []);
        Settings = settings;
        Home = home;
        Pages = pages;
        Navigation = navigation;
        Search = search;
        Problems = problems;
        _bySlug = pages.ToDictionary(p => p.Slug, StringComparer.Ordinal);
        _position = pages.Select((p, i) => (p.Slug, i)).ToDictionary(x => x.Slug, x => x.i, StringComparer.Ordinal);
    }

    public SiteSettings Settings { get; }

    /// <summary>What the home page shows below the demonstration, taken from the sections of the README it replaces.</summary>
    public HomeContent HomeContent { get; }

    /// <summary>The README of the repository.</summary>
    public DocPage Home { get; }

    /// <summary>The pages in the order of the navigation, which is the order of previous and next. The home page is not one of them.</summary>
    public IReadOnlyList<DocPage> Pages { get; }

    /// <summary>The tree for <see cref="KlyContentNavigation"/>: the home page, then the pages, with their groups.</summary>
    public IReadOnlyList<KlyContentNavigationItem> Navigation { get; }

    /// <summary>The pages and their headings for <see cref="KlyContentSearch"/>.</summary>
    public IReadOnlyList<KlyContentSearchPage> Search { get; }

    /// <summary>Links that lead nowhere. The server logs them; the export stops on them.</summary>
    public IReadOnlyList<string> Problems { get; }

    /// <summary>Finds a page by its address. Null when there is none.</summary>
    public DocPage? Find(string? slug) => _bySlug.GetValueOrDefault((slug ?? "").Trim('/'));

    /// <summary>The page before and the page after, in the order of the navigation. Either is null at the ends.</summary>
    public (KlyContentSurroundLink? Previous, KlyContentSurroundLink? Next) Surround(DocPage page)
    {
        if (!_position.TryGetValue(page.Slug, out var index))
        {
            return (null, null);
        }
        KlyContentSurroundLink? Link(int i) => i >= 0 && i < Pages.Count ? new KlyContentSurroundLink(Pages[i].NavTitle, Pages[i].Href) : null;
        return (Link(index - 1), Link(index + 1));
    }
}

/// <summary>An item of the Features list of a README: the bold lead as text, and the rest of the item as HTML.</summary>
public sealed record HomeFeature(string Title, string Html);

/// <summary>A package of the Packages table of a README, with the layer it belongs to (<c>Core</c>, <c>Blazor</c> or <c>Add-on</c>), its description as HTML and its install command.</summary>
public sealed record HomePackage(string Id, string Layer, string DescriptionHtml, string Install, string NugetUrl);

/// <summary>A row of the ecosystem table of a README: the library, where its site or repository is, and what it is (HTML). <paramref name="Accent"/> is null for a project that is not a library of the family.</summary>
public sealed record HomeLibrary(string Name, string Href, string DescriptionHtml, string? Accent, bool Current);

/// <summary>The Features, Packages and ecosystem sections of a README, parsed so the home page can set them in its own layout. Empty when the README has no such section.</summary>
public sealed record HomeContent(IReadOnlyList<HomeFeature> Features, IReadOnlyList<HomePackage> Packages, IReadOnlyList<HomeLibrary> Ecosystem);

/// <summary>What the site says about the library: <c>site.yml</c> with the gaps filled from the README and from <c>libraries.yml</c>.</summary>
/// <param name="Accent">The accent color as <c>#rrggbb</c>: <c>site.yml</c>, else <c>libraries.yml</c>, else the Klyroo.UI green.</param>
/// <param name="Headline">The two lines of the headline of the home page (the second marked by the line color). One line when <c>site.yml</c> gives one.</param>
/// <param name="Lead">The paragraph under the headline.</param>
/// <param name="Home">The home page: <c>docs</c> (the hero with its demonstration board) or <c>showcase</c> (the metro map of the Klyroo network).</param>
/// <param name="HeroInstall">The command of the copyable chip in the hero: the line of <c>install</c> for the main package, without its trailing comment.</param>
public sealed record SiteSettings(
    string RepoName,
    string Title,
    string Tagline,
    string Description,
    string? Install,
    string GithubUrl,
    string Branch,
    IReadOnlyList<string> Packages,
    string Accent,
    IReadOnlyList<string> Headline,
    string Lead,
    string? HeroInstall,
    DemoConfig? Demo,
    string Home = "docs")
{
    /// <summary>Whether the home page is the showcase of the network.</summary>
    public bool IsShowcase => Home == "showcase";

    /// <summary>The name that follows <c>klyroo/</c> in the wordmark.</summary>
    public string ShortName => RepoName.StartsWith("klyroo-", StringComparison.OrdinalIgnoreCase) ? RepoName["klyroo-".Length..] : RepoName;
}
