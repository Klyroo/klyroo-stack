using Klyroo.Docs.Site.Content;
using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Components.Showcase;

/// <summary>Where the buttons of the showcase lead: the docs of each library, its playground, and the organisation.</summary>
public sealed class ShowcaseUrls(Libraries libraries, SiteSettings settings)
{
    /// <summary>The site of the library (<c>https://klyroo.github.io/klyroo-table/</c>).</summary>
    public string Docs(MetroLine line) => libraries.SiteUrl(line.Repo);

    /// <summary>The WebAssembly playground of the library, below this site (<c>https://klyroo.github.io/klyroo-stack/playground/table/</c>).</summary>
    public string Playground(MetroLine line) => Playground() + line.Key + "/";

    public string Playground() => libraries.SiteUrl(settings.RepoName) + "playground/";

    public string Organization => $"https://github.com/{libraries.Organization}";

    public string Repository => settings.GithubUrl;

    public string NuGet(string package) => "https://www.nuget.org/packages/" + Uri.EscapeDataString(package);
}
