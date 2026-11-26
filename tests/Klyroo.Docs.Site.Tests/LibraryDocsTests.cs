using Klyroo.Docs.Site.Content;
using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Tests;

public class LibraryDocsTests
{
    private static FakeRepository Repository() => new FakeRepository()
        .Write("README.md", """
            # Klyroo.Demo

            [![CI](https://example.com/badge.svg)](https://example.com/ci)

            A demo library for the tests. It has two pages.

            ## Installation

            Run it. See [the guides](docs/guides.md#first-guide) and [the changelog](CHANGELOG.md).

            ## Installation

            A second heading with the same words.
            """)
        .Write("CHANGELOG.md", "# Changelog\n")
        .Write("src/Demo/Demo.cs", "class Demo;\n")
        .Write("docs/README.md", "# Klyroo.Demo documentation\n\n- [Guides](guides.md): the guides.\n")
        .Write("docs/getting-started.md", "# Getting started\n\nInstall the demo. See the [guides](guides.md#first-guide).\n\n## Install\n")
        .Write("docs/guides.md", "# Guides\n\nEach guide solves one task.\n\n## First guide\n\n### Detail\n\nBack to [start](getting-started.md), the [readme](../README.md#installation) and [the code](../src/Demo/Demo.cs).\n")
        .Write("docs/api-reference.md", "# Klyroo.Demo API reference\n\n> Generated.\n\n## Klyroo.Demo\n\n#### `Thing`\n\nA thing.\n");

    [Fact]
    public void The_title_is_the_first_heading_and_the_body_has_no_h1()
    {
        using var repo = Repository();
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);

        var guides = site.Find("guides");
        Assert.NotNull(guides);
        Assert.Equal("Guides", guides.Title);
        Assert.DoesNotContain("<h1", guides.Html);
        Assert.Contains("<h2 id=\"first-guide\">", guides.Html);
        Assert.Equal("Each guide solves one task.", guides.MetaDescription);
        Assert.Equal("Klyroo.Demo", site.Settings.Title);
        Assert.DoesNotContain("<h1", site.Home.Html);
    }

    [Fact]
    public void The_navigation_follows_site_yml_then_the_unlisted_pages_and_the_index_comes_first()
    {
        using var repo = Repository().Write("docs/site.yml", """
            nav:
              - group: Reference
                pages:
                  - page: api-reference
                    tocDepth: 4
              - guides
            """);
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);

        Assert.Equal(["documentation", "api-reference", "guides", "getting-started"], site.Pages.Select(p => p.Slug));
        Assert.Equal("API reference", site.Find("api-reference")!.NavTitle);
        Assert.Equal(4, site.Find("api-reference")!.TocDepth);
        Assert.Equal("Reference", site.Find("api-reference")!.Group);
        var (previous, next) = site.Surround(site.Find("guides")!);
        Assert.Equal("API reference", previous!.Title);
        Assert.Equal("Getting started", next!.Title);
    }

    [Fact]
    public void Types_are_in_the_table_of_contents_and_the_search_down_to_the_configured_level()
    {
        using var repo = Repository().Write("docs/site.yml", "nav:\n  - page: api-reference\n    tocDepth: 4\n    indexText: false\n");
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);

        Assert.Contains(site.Find("api-reference")!.Toc, l => l.Id == "thing" && l.Level == 4);
        var entry = site.Search.Single(s => s.Href == "api-reference/");
        Assert.Null(entry.Keywords);
        Assert.Contains(entry.Headings!, h => h.Id == "thing");
    }

    [Fact]
    public void By_default_the_table_of_contents_stops_at_the_third_level()
    {
        using var repo = Repository();
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);
        Assert.Equal(["first-guide", "detail"], site.Find("guides")!.Toc.Select(l => l.Id));
        Assert.DoesNotContain(site.Find("api-reference")!.Toc, l => l.Id == "thing");
    }

    [Fact]
    public void Badges_are_not_on_the_home_page()
    {
        using var repo = Repository();
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);
        Assert.DoesNotContain("badge.svg", site.Home.Html);
        Assert.DoesNotContain("<img", site.Home.Html);
    }

    [Fact]
    public void Heading_ids_are_the_ones_of_github()
    {
        using var repo = Repository();
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);
        Assert.Equal(["installation", "installation-1"], site.Home.Headings.Select(h => h.Id));
    }

    [Fact]
    public void Links_between_files_become_routes_and_links_to_other_files_become_github_addresses()
    {
        using var repo = Repository();
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);

        var guides = site.Find("guides")!.Html;
        Assert.Contains("href=\"getting-started/\"", guides);
        Assert.Contains("href=\"./#installation\"", guides);
        Assert.Contains("href=\"https://github.com/klyroo/klyroo-demo/blob/main/src/Demo/Demo.cs\"", guides);
        Assert.Contains("href=\"guides/#first-guide\"", site.Home.Html);
        Assert.Contains("href=\"https://github.com/klyroo/klyroo-demo/blob/main/CHANGELOG.md\"", site.Home.Html);
        Assert.Empty(site.Problems);
    }

    [Fact]
    public void A_link_to_a_file_that_is_not_in_the_repository_is_a_problem()
    {
        using var repo = Repository().Write("docs/faq.md", "# FAQ\n\nSee [the missing file](../samples/Nothing.cs) and [a page](nowhere.md).\n");
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);
        Assert.Equal(2, site.Problems.Count);
        Assert.Contains(site.Problems, p => p.Contains("samples/Nothing.cs"));
        Assert.Contains(site.Problems, p => p.Contains("docs/nowhere.md"));
    }

    [Fact]
    public void A_page_that_site_yml_lists_but_does_not_exist_names_the_pages_there_are()
    {
        using var repo = Repository().Write("docs/site.yml", "nav:\n  - missing\n");
        var error = Assert.Throws<ContentException>(() => LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries));
        Assert.Contains("missing", error.Message);
        Assert.Contains("guides", error.Message);
    }

    [Fact]
    public void A_setting_that_does_not_exist_is_an_error_instead_of_being_ignored()
    {
        using var repo = Repository().Write("docs/site.yml", "titel: Klyroo.Demo\n");
        var error = Assert.Throws<ContentException>(() => LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries));
        Assert.Contains("titel", error.Message);
    }

    [Fact]
    public void Without_site_yml_the_site_still_has_a_title_and_all_the_pages()
    {
        using var repo = Repository();
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);
        Assert.Equal("Klyroo.Demo", site.Settings.Title);
        Assert.Equal("A demo library for the tests. It has two pages.", site.Settings.Description);
        Assert.Equal(4, site.Pages.Count);
    }
}
