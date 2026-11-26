using Klyroo.Docs.Site.Content;

namespace Klyroo.Docs.Site.Library;

/// <summary>
/// Decides where a link written in a file of the repository leads on the site, as the README and the docs of a library are written for GitHub:
/// <list type="bullet">
/// <item>a link to a page of the docs (<c>guides.md#anchor</c>, <c>docs/guides.md</c>, <c>../README.md</c>) becomes the route of that page, with its fragment;</item>
/// <item>a link to another file or folder of the repository (<c>../src/...</c>, <c>../CHANGELOG.md</c>) becomes its address on GitHub, on the configured branch;</item>
/// <item>a GitHub address of a page of this library or of another Klyroo library that has a site (<c>https://github.com/klyroo/klyroo-ui/blob/main/docs/guides.md#x</c>) becomes the address of the page on its site;</item>
/// <item>any other address is left as it is.</item>
/// </list>
/// A link to a file that is not in the repository is recorded in <see cref="Problems"/>.
/// </summary>
public sealed class LinkResolver
{
    private readonly string _repoRoot;
    private readonly string _sourceDirectory;
    private readonly string _sourcePath;
    private readonly IReadOnlyDictionary<string, string> _pages;
    private readonly string _github;
    private readonly string _branch;
    private readonly string _repoName;
    private readonly Libraries _libraries;
    private readonly List<string> _problems;

    /// <param name="repoRoot">The folder of the repository.</param>
    /// <param name="sourcePath">The file the link is written in, from the root of the repository (<c>docs/guides.md</c>).</param>
    /// <param name="pages">The pages of this site by their file, from the root of the repository (<c>docs/guides.md</c> is <c>guides</c>, <c>README.md</c> is the empty slug).</param>
    /// <param name="problems">Where links to nothing are recorded.</param>
    public LinkResolver(string repoRoot, string sourcePath, IReadOnlyDictionary<string, string> pages, string github, string branch, string repoName, Libraries libraries, List<string> problems)
    {
        _repoRoot = repoRoot;
        _sourcePath = sourcePath;
        _sourceDirectory = sourcePath.Contains('/') ? sourcePath[..sourcePath.LastIndexOf('/')] : "";
        _pages = pages;
        _github = github;
        _branch = branch;
        _repoName = repoName;
        _libraries = libraries;
        _problems = problems;
    }

    public List<string> Problems => _problems;

    public LinkTarget Resolve(string url)
    {
        var trimmed = url.Trim();
        if (trimmed.Length == 0 || trimmed[0] == '#')
        {
            return new LinkTarget(url);
        }
        if (ContentUrl.IsExternal(trimmed))
        {
            return Uri.TryCreate(trimmed, UriKind.Absolute, out var absolute) ? ResolveGithub(url, absolute) : new LinkTarget(url);
        }

        var cut = trimmed.IndexOfAny(['?', '#']);
        var path = (cut < 0 ? trimmed : trimmed[..cut]).Replace('\\', '/');
        var rest = cut < 0 ? "" : trimmed[cut..];
        var repoPath = Normalize(path.StartsWith('/') ? path : _sourceDirectory.Length == 0 ? path : _sourceDirectory + "/" + path);
        if (repoPath is null)
        {
            _problems.Add($"{_sourcePath}: the link '{url}' leaves the repository.");
            return new LinkTarget(url);
        }

        // The root of the repository is the home page and docs/ is the index of the documentation.
        var key = repoPath is "" ? "README.md" : repoPath is "docs" ? "docs/README.md" : repoPath;
        if (_pages.TryGetValue(key, out var slug))
        {
            return new LinkTarget(Route(slug) + rest, slug);
        }

        var onDisk = Path.Combine(_repoRoot, repoPath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(onDisk))
        {
            return new LinkTarget(GithubUrl("blob", repoPath) + rest);
        }
        if (Directory.Exists(onDisk))
        {
            return new LinkTarget(GithubUrl("tree", repoPath) + rest);
        }
        _problems.Add($"{_sourcePath}: the link '{url}' leads to '{repoPath}', which is not in the repository.");
        return new LinkTarget(GithubUrl("blob", repoPath) + rest);
    }

    // https://github.com/<org>/<repo>[/blob|tree/<branch>/<path>][#fragment] of a library that has a site is that library's page.
    private LinkTarget ResolveGithub(string url, Uri address)
    {
        if (!address.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(address.Query))
        {
            return new LinkTarget(url);
        }
        var parts = address.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !parts[0].Equals(_libraries.Organization, StringComparison.OrdinalIgnoreCase) || _libraries.Find(parts[1]) is not { } library)
        {
            return new LinkTarget(url);
        }

        string? path;
        if (parts.Length == 2)
        {
            path = "README.md";
        }
        else if (parts.Length >= 5 && parts[2] is "blob" or "tree")
        {
            path = string.Join('/', parts.Skip(4).Select(Uri.UnescapeDataString));
            if (path.Length == 0)
            {
                path = "README.md";
            }
        }
        else
        {
            return new LinkTarget(url);
        }

        var own = library.Repo.Equals(_repoName, StringComparison.OrdinalIgnoreCase);
        var fragment = address.Fragment;
        string? slug = null;
        if (path is "README.md")
        {
            slug = "";
        }
        else if (path is "docs/README.md" or "docs" or "docs/")
        {
            slug = "documentation";
        }
        else if (path.StartsWith("docs/", StringComparison.Ordinal) && path.EndsWith(".md", StringComparison.Ordinal))
        {
            slug = string.Join('/', path[5..^3].Split('/').Select(ContentUrl.Segment));
        }
        if (slug is null)
        {
            return new LinkTarget(url);
        }

        if (own)
        {
            if (!_pages.Values.Contains(slug))
            {
                _problems.Add($"{_sourcePath}: the link '{url}' leads to '{path}', which is not a page of the site.");
                return new LinkTarget(url);
            }
            return new LinkTarget(Route(slug) + fragment, slug);
        }
        // Another library: its own site, which is checked when that site is built.
        return new LinkTarget(_libraries.SiteUrl(library.Repo) + (slug.Length == 0 ? "" : slug + "/") + fragment);
    }

    private static string Route(string slug) => slug.Length == 0 ? "./" : slug + "/";

    private string GithubUrl(string kind, string repoPath) =>
        $"{_github}/{kind}/{Uri.EscapeDataString(_branch)}/{string.Join('/', repoPath.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(Uri.EscapeDataString))}";

    // "docs/../src/x.cs" as "src/x.cs"; null when the path climbs out of the repository. A trailing slash is kept.
    private static string? Normalize(string path)
    {
        var segments = new List<string>();
        foreach (var segment in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment == ".")
            {
                continue;
            }
            if (segment == "..")
            {
                if (segments.Count == 0)
                {
                    return null;
                }
                segments.RemoveAt(segments.Count - 1);
                continue;
            }
            segments.Add(Uri.UnescapeDataString(segment));
        }
        return string.Join('/', segments);
    }
}
