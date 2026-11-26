using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Tests;

public class LinkResolverTests
{
    private static LinkResolver ResolverFor(FakeRepository repo, string source, List<string> problems) => new(
        repo.Root, source,
        new Dictionary<string, string> { ["README.md"] = "", ["docs/README.md"] = "documentation", ["docs/guides.md"] = "guides", ["docs/sub/deep.md"] = "sub/deep" },
        "https://github.com/klyroo/klyroo-demo", "main", "klyroo-demo", FakeRepository.Libraries, problems);

    [Theory]
    [InlineData("docs/guides.md", "guides.md#x", "guides/#x")]
    [InlineData("docs/guides.md", "./sub/deep.md", "sub/deep/")]
    [InlineData("docs/sub/deep.md", "../guides.md?q=1#top", "guides/?q=1#top")]
    [InlineData("docs/guides.md", "../README.md#installation", "overview/#installation")]
    [InlineData("docs/guides.md", "README.md", "documentation/")]
    [InlineData("README.md", "docs/guides.md#a", "guides/#a")]
    [InlineData("README.md", "docs/", "documentation/")]
    [InlineData("docs/guides.md", "#local", "#local")]
    [InlineData("docs/guides.md", "https://example.com/x", "https://example.com/x")]
    [InlineData("docs/guides.md", "mailto:a@b.c", "mailto:a@b.c")]
    public void Relative_links_to_pages_become_routes(string source, string link, string expected)
    {
        using var repo = new FakeRepository().Write("docs/guides.md", "").Write("docs/sub/deep.md", "").Write("README.md", "").Write("docs/README.md", "");
        var problems = new List<string>();
        Assert.Equal(expected, ResolverFor(repo, source, problems).Resolve(link).Url);
        Assert.Empty(problems);
    }

    [Fact]
    public void Files_and_folders_of_the_repository_link_to_github_on_the_branch()
    {
        using var repo = new FakeRepository().Write("CHANGELOG.md", "").Write("src/A B/x.cs", "");
        var problems = new List<string>();
        var resolver = ResolverFor(repo, "docs/guides.md", problems);
        Assert.Equal("https://github.com/klyroo/klyroo-demo/blob/main/CHANGELOG.md#unreleased", resolver.Resolve("../CHANGELOG.md#unreleased").Url);
        Assert.Equal("https://github.com/klyroo/klyroo-demo/blob/main/src/A%20B/x.cs", resolver.Resolve("../src/A%20B/x.cs").Url);
        Assert.Equal("https://github.com/klyroo/klyroo-demo/tree/main/src", resolver.Resolve("../src").Url);
        Assert.Empty(problems);
    }

    [Fact]
    public void Github_addresses_of_the_klyroo_libraries_become_the_pages_of_their_sites()
    {
        using var repo = new FakeRepository().Write("docs/guides.md", "").Write("README.md", "");
        var problems = new List<string>();
        var resolver = ResolverFor(repo, "README.md", problems);
        Assert.Equal("guides/#a", resolver.Resolve("https://github.com/klyroo/klyroo-demo/blob/main/docs/guides.md#a").Url);
        Assert.Equal("https://klyroo.github.io/klyroo-other/overview/#installation", resolver.Resolve("https://github.com/klyroo/klyroo-other#installation").Url);
        Assert.Equal("https://klyroo.github.io/klyroo-other/guides/#x", resolver.Resolve("https://github.com/klyroo/klyroo-other/blob/main/docs/guides.md#x").Url);
        // Not a page, or not a library with a site: left on GitHub.
        Assert.Equal("https://github.com/klyroo/klyroo-other/blob/main/CONTRIBUTING.md", resolver.Resolve("https://github.com/klyroo/klyroo-other/blob/main/CONTRIBUTING.md").Url);
        Assert.Equal("https://github.com/klyroo/klyroo-unknown", resolver.Resolve("https://github.com/klyroo/klyroo-unknown").Url);
        Assert.Empty(problems);
    }

    [Fact]
    public void A_link_to_nothing_or_out_of_the_repository_is_recorded()
    {
        using var repo = new FakeRepository();
        var problems = new List<string>();
        var resolver = ResolverFor(repo, "docs/guides.md", problems);
        resolver.Resolve("../nothing.md");
        resolver.Resolve("../../outside.md");
        Assert.Equal(2, problems.Count);
    }
}
