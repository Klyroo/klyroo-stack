using Klyroo.UI;

namespace Klyroo.Stack.Site.Client.Docs;

/// <summary>The navigation data of the site, built from the catalog: sidebar, search, previous and next page, table of contents.</summary>
public static class DocsNavigation
{
    /// <summary>
    /// The components in the order of the sidebar: group by group, in catalog order inside each group. The previous
    /// and next links of a component page follow it, so they agree with what the reader sees on the left.
    /// </summary>
    public static IReadOnlyList<ComponentDoc> InSidebarOrder { get; } =
        [.. Catalog.Groups.SelectMany(group => Catalog.Components.Where(c => c.Group == group))];

    /// <summary>
    /// The sidebar. <paramref name="prefix"/> is the render mode prefix of the page ("/server", or "" for the default):
    /// component links keep it, so the reader stays in the mode they chose and the current page is found by address
    /// (the guides exist only in the default mode).
    /// </summary>
    public static IReadOnlyList<KlyContentNavigationItem> Sidebar(string prefix, string? activeLibrarySlug = null)
    {
        // Every non-UI Klyroo library gets its own 100% independent multi-page documentation sidebar
        // (Getting Started, Guides, Reference) with zero cross-library clutter.
        if (activeLibrarySlug is not null && !string.Equals(activeLibrarySlug, "ui", StringComparison.OrdinalIgnoreCase))
        {
            var tree = KlyrooLibraryDocs.FindLibrary(activeLibrarySlug);
            if (tree is not null)
            {
                return
                [
                    .. tree.Pages
                        .Select(p => p.Group)
                        .Distinct()
                        .Select(group => new KlyContentNavigationItem(group)
                        {
                            Open = true,
                            Children =
                            [
                                .. tree.Pages
                                    .Where(p => p.Group == group)
                                    .Select(p => new KlyContentNavigationItem(p.Title)
                                    {
                                        Href = KlyrooLibraryDocs.PageHref(tree.LibrarySlug, p.Slug),
                                        Exact = true,
                                    }),
                            ],
                        }),
                ];
            }
        }

        // Klyroo.UI independent documentation sidebar: full Klyroo.UI guides + all 125+ components.
        var items = new List<KlyContentNavigationItem>();
        items.AddRange(Catalog.Guides
            .Select(g => g.Section)
            .Distinct()
            .Select(section => new KlyContentNavigationItem(section)
            {
                Open = true,
                Children = [.. Catalog.Guides.Where(g => g.Section == section).Select(g => new KlyContentNavigationItem(g.Title) { Href = g.Href, Exact = true })],
            }));
        items.AddRange(Catalog.Groups.Where(group => Catalog.Components.Any(c => c.Group == group)).Select(group => new KlyContentNavigationItem(group)
        {
            Open = true,
            Children = [.. Catalog.Components.Where(c => c.Group == group)
                .Select(c => new KlyContentNavigationItem(c.Name) { Href = $"{prefix}/components/{c.Slug}", Exact = true })],
        }));
        return items;
    }

    private static readonly Dictionary<string, IReadOnlyList<KlyContentSearchPage>> SearchPagesByKey = [];

    /// <summary>
    /// Header search pages. When browsing a specific Klyroo library, search is strictly scoped to that library
    /// (zero leakage of Klyroo.UI components into Klyroo.Query, Klyroo.Table, Klyroo.Form, etc.).
    /// </summary>
    public static IReadOnlyList<KlyContentSearchPage> SearchPages(string prefix, string? activeLibrarySlug = null)
    {
        var cacheKey = $"{prefix}|{activeLibrarySlug ?? "hub"}";
        lock (SearchPagesByKey)
        {
            if (!SearchPagesByKey.TryGetValue(cacheKey, out var pages))
            {
                if (activeLibrarySlug is not null && !string.Equals(activeLibrarySlug, "ui", StringComparison.OrdinalIgnoreCase))
                {
                    var lib = KlyrooLibraryDocs.FindLibrary(activeLibrarySlug);
                    pages = lib is null
                        ? []
                        : [
                            new KlyContentSearchPage($"{lib.LibraryName} Overview", $"/{lib.LibrarySlug}") { Group = lib.LibraryName, Icon = "" },
                            .. lib.Pages.Select(p => new KlyContentSearchPage(p.Title, KlyrooLibraryDocs.PageHref(lib.LibrarySlug, p.Slug))
                            {
                                Group = p.Group,
                                Icon = "",
                                Keywords = $"{p.Slug} {p.Lead}",
                            }),
                        ];
                }
                else if (string.Equals(activeLibrarySlug, "ui", StringComparison.OrdinalIgnoreCase))
                {
                    pages =
                    [
                        new KlyContentSearchPage("Klyroo.UI Overview", "/ui") { Group = "Klyroo.UI", Icon = "" },
                        .. Catalog.Guides.Select(g => new KlyContentSearchPage(g.Title, g.Href) { Group = g.Section, Icon = "" }),
                        new KlyContentSearchPage("Templates", "templates") { Group = "Gallery", Icon = "", Keywords = "starter dashboard landing live demo" },
                        .. InSidebarOrder.Select(c => new KlyContentSearchPage(c.Name, $"{prefix}/components/{c.Slug}")
                        {
                            Group = c.Group,
                            Icon = "",
                            Keywords = c.Slug.Replace('-', ' '),
                        }),
                    ];
                }
                else
                {
                    pages =
                    [
                        new KlyContentSearchPage("Klyroo Hub", "/") { Group = "Klyroo", Icon = "", Keywords = "klyroo hub libraries" },
                        .. KlyrooStackProducts.All.Select(p => new KlyContentSearchPage($"{p.Name} Overview", $"/{p.Slug}") { Group = "Libraries", Icon = "", Keywords = $"{p.RepoName} {p.CorePackage} {p.UiPackage} {p.Tagline}" }),
                        .. KlyrooStackProducts.All.Select(p => new KlyContentSearchPage($"{p.Name} Documentation", p.DocsHref) { Group = "Documentation", Icon = "", Keywords = $"{p.Slug} docs reference" }),
                    ];
                }

                SearchPagesByKey[cacheKey] = pages;
            }
            return pages;
        }
    }

    /// <summary>The links to the components before and after <paramref name="slug"/>, in sidebar order.</summary>
    public static (KlyContentSurroundLink? Previous, KlyContentSurroundLink? Next) Surround(string slug, string prefix)
    {
        var index = -1;
        for (var i = 0; i < InSidebarOrder.Count; i++)
        {
            if (InSidebarOrder[i].Slug == slug) index = i;
        }
        if (index < 0) return (null, null);
        KlyContentSurroundLink Link(ComponentDoc doc) => new(doc.Name, $"{prefix}/components/{doc.Slug}") { Description = doc.Description };
        return (index > 0 ? Link(InSidebarOrder[index - 1]) : null, index < InSidebarOrder.Count - 1 ? Link(InSidebarOrder[index + 1]) : null);
    }

    /// <summary>The id of the section of an example, as <c>Example.razor</c> gives it.</summary>
    public static string ExampleAnchor(string title) => title.ToLowerInvariant().Replace(' ', '-');

    /// <summary>The table of contents of a component page: its examples, then the API.</summary>
    public static IReadOnlyList<KlyContentTocLink> Toc(ComponentDoc doc) =>
    [
        .. doc.Examples.Select(e => new KlyContentTocLink(ExampleAnchor(e.Title), e.Title)),
        new KlyContentTocLink("api", "API"),
    ];
}
