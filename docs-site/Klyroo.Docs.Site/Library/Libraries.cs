using Klyroo.Docs.Site.Content;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Klyroo.Docs.Site.Library;

/// <summary>A Klyroo library that has a documentation site, with the accent color that makes its site recognizable among the others.</summary>
public sealed record LibraryInfo(string Repo, string Name, string? Accent = null, string? Playground = null)
{
    /// <summary>The name that follows <c>klyroo/</c> in the wordmark: <c>table</c> for <c>klyroo-table</c>.</summary>
    public string ShortName => Repo.StartsWith("klyroo-", StringComparison.OrdinalIgnoreCase) ? Repo["klyroo-".Length..] : Repo;
}

/// <summary>The Klyroo libraries listed in the header of every site (<c>libraries.yml</c>), and where their sites are.</summary>
public sealed class Libraries
{
    public required string Organization { get; init; }

    /// <summary>The address all the sites are below (<c>https://klyroo.github.io</c>), without a trailing slash.</summary>
    public required string PagesUrl { get; init; }

    /// <summary>The address of the playground app (<c>https://klyroo.github.io/klyroo-stack/playground/</c>), with a trailing slash. Null when there is none.</summary>
    public string? PlaygroundUrl { get; init; }

    public required IReadOnlyList<LibraryInfo> Items { get; init; }

    /// <summary>The address of the playground of a library (<c>.../playground/table/</c>), or null when the library has none or no playground is configured.</summary>
    public string? PlaygroundOf(string repo) => PlaygroundUrl is not null && Find(repo)?.Playground is { Length: > 0 } slug ? $"{PlaygroundUrl}{slug}/" : null;

    public LibraryInfo? Find(string repo) => Items.FirstOrDefault(l => l.Repo.Equals(repo, StringComparison.OrdinalIgnoreCase));

    /// <summary>The address of the site of a library, with a trailing slash.</summary>
    public string SiteUrl(string repo) => $"{PagesUrl}/{repo}/";

    public string GithubUrl(string repo) => $"https://github.com/{Organization}/{repo}";

    public static Libraries Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new ContentException($"{path}: the list of the Klyroo libraries is missing.");
        }
        var file = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).IgnoreUnmatchedProperties().Build().Deserialize<LibrariesFile>(File.ReadAllText(path));
        return new Libraries
        {
            Organization = file.Organization ?? "klyroo",
            PagesUrl = (file.Pages ?? "https://klyroo.github.io").TrimEnd('/'),
            PlaygroundUrl = file.Playground is { Length: > 0 } playground ? playground.TrimEnd('/') + "/" : null,
            Items = [.. (file.Libraries ?? []).Select(l => new LibraryInfo(l.Repo ?? throw new ContentException($"{path}: a library has no repo."), l.Name ?? l.Repo, l.Accent, l.Playground))],
        };
    }

    private sealed class LibrariesFile
    {
        public string? Organization { get; set; }
        public string? Pages { get; set; }
        public string? Playground { get; set; }
        public List<LibraryEntry>? Libraries { get; set; }
    }

    private sealed class LibraryEntry
    {
        public string? Repo { get; set; }
        public string? Name { get; set; }
        public string? Accent { get; set; }
        public string? Playground { get; set; }
    }
}
