using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Klyroo.Docs.Site.Content;

namespace Klyroo.Docs.Site.Export;

/// <summary>
/// Writes the running site as static files for GitHub Pages. It requests the home page, every page of the content, the search index and the 404 page, then
/// every same-origin address those pages refer to (the fingerprinted stylesheet, scripts, fonts, icons, the files of <c>_content/Klyroo.UI</c>), looking inside
/// the HTML, the CSS and the JavaScript it receives for more, until nothing new turns up. Everything lands below <paramref name="outFolder"/> as it will be
/// served below <paramref name="pathBase"/> (<c>/klyroo-table/</c>): a page <c>guides</c> is <c>guides/index.html</c>, which a static server serves for
/// <c>/guides</c> and <c>/guides/</c>, the 404 page is <c>404.html</c>, and <c>.nojekyll</c> keeps GitHub from processing the folder.
/// The export fails, and says what and where, on a page or an asset that does not answer 200, a link that leads nowhere or to a fragment the target does
/// not have, and a link to an address outside the path base.
/// </summary>
public sealed partial class StaticExporter(HttpClient http, string pathBase, string outFolder, DocsSite site)
{
    private const string NotFoundRoute = "not-found";

    private readonly Dictionary<string, Fetched> _fetched = new(StringComparer.Ordinal);
    private readonly Queue<(string Path, string From)> _queue = new();
    private readonly List<(string From, string Path, string Fragment, string Text)> _links = [];
    private readonly List<string> _errors = [];
    // The import map of the pages: the address a script names, and the fingerprinted file the browser asks for instead. Only the second is exported.
    private readonly Dictionary<string, string> _importMap = new(StringComparer.Ordinal);

    private sealed record Fetched(string ContentType, byte[] Body, HashSet<string>? Ids);

    [GeneratedRegex("""\s(?<name>href|src|poster|data-src|srcset|imagesrcset|content)\s*=\s*(?:"(?<v>[^"]*)"|'(?<v>[^']*)')""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex Attribute();

    [GeneratedRegex("""<a\b[^>]*?\shref\s*=\s*(?:"(?<v>[^"]*)"|'(?<v>[^']*)')""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex Anchor();

    [GeneratedRegex("""\s(?:id|name)\s*=\s*(?:"(?<v>[^"]*)"|'(?<v>[^']*)')""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex IdAttribute();

    [GeneratedRegex("""<style\b[^>]*>(?<css>.*?)</style>""", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex StyleElement();

    [GeneratedRegex("""\sstyle\s*=\s*(?:"(?<v>[^"]*)"|'(?<v>[^']*)')""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex StyleAttribute();

    [GeneratedRegex("""<script\b[^>]*type\s*=\s*["']importmap["'][^>]*>(?<json>.*?)</script>""", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex ImportMap();

    // Blazor lists the JavaScript initializers of the libraries (Klyroo.UI.lib.module.js) in a comment, as a base64 JSON array of addresses.
    [GeneratedRegex("""<!--Blazor-Web-Initializers:(?<b64>[A-Za-z0-9+/=]+)-->""", RegexOptions.CultureInvariant)]
    private static partial Regex Initializers();

    [GeneratedRegex("""url\(\s*(?:"(?<v>[^"]*)"|'(?<v>[^']*)'|(?<v>[^)\s"']*))\s*\)|@import\s+(?:"(?<v>[^"]*)"|'(?<v>[^']*)')""", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex CssUrl();

    // import "x", import x from "x", export * from "x", import("x"), new URL("x", import.meta.url). Only relative or root-relative specifiers are files of the site.
    [GeneratedRegex("""(?:\bfrom\s*|\bimport\s*\(\s*|\bimport\s+|new\s+URL\(\s*)(?:"(?<v>[^"\\\n]+)"|'(?<v>[^'\\\n]+)'|`(?<v>[^`\\\n$]+)`)""", RegexOptions.CultureInvariant)]
    private static partial Regex JsSpecifier();

    /// <summary>Runs the export. True when it is complete and has no error.</summary>
    public async Task<bool> RunAsync(TextWriter log)
    {
        _errors.AddRange(site.Problems);
        var origin = http.BaseAddress!;
        var baseUri = new Uri(origin, pathBase);

        // Where the pages are: the home page, the pages of the content, the search index, and the page that answers an address with nothing behind it.
        Enqueue(pathBase, "(start)");
        foreach (var page in site.Pages)
        {
            Enqueue(pathBase + page.Slug + "/", "(content)");
        }
        Enqueue(pathBase + "search-index.json", "(search)");

        while (_queue.Count > 0)
        {
            var (path, from) = _queue.Dequeue();
            if (_fetched.ContainsKey(path))
            {
                continue;
            }
            await FetchAsync(path, from, baseUri);
        }

        // An address with nothing behind it: the site answers 404 with the page of the 404 route, which becomes 404.html.
        using (var missing = await http.GetAsync(pathBase + "this-page-does-not-exist-" + Guid.NewGuid().ToString("N")[..8]))
        {
            var body = await missing.Content.ReadAsByteArrayAsync();
            if (missing.StatusCode != HttpStatusCode.NotFound || !Encoding.UTF8.GetString(body).Contains("<h1", StringComparison.Ordinal))
            {
                _errors.Add($"An unknown address answered {(int)missing.StatusCode} instead of the 404 page.");
            }
            else
            {
                _fetched[pathBase + "404.html"] = new Fetched("text/html", body, null);
                // Its stylesheet and scripts are found like the ones of any page.
                await ScanAsync(pathBase + "404.html", Encoding.UTF8.GetString(body), "text/html", baseUri);
                while (_queue.Count > 0)
                {
                    var (path, from) = _queue.Dequeue();
                    if (!_fetched.ContainsKey(path))
                    {
                        await FetchAsync(path, from, baseUri);
                    }
                }
            }
        }

        CheckLinks();
        if (_errors.Count > 0)
        {
            await log.WriteLineAsync($"Export failed with {_errors.Count} error(s):");
            foreach (var error in _errors.Distinct().Take(100))
            {
                await log.WriteLineAsync("  " + error);
            }
            return false;
        }

        Write();
        var written = _fetched.Values.Where(f => f.ContentType.Length > 0).ToList();
        var pages = written.Count(f => f.ContentType.StartsWith("text/html", StringComparison.Ordinal));
        await log.WriteLineAsync($"Exported {written.Count} files ({pages} HTML pages, {written.Count - pages} other files, {written.Sum(f => (long)f.Body.Length) / 1024} KiB) to {outFolder}, to be served below {pathBase}");
        return true;
    }

    private void Enqueue(string path, string from) => _queue.Enqueue((_importMap.GetValueOrDefault(path, path), from));

    private async Task FetchAsync(string path, string from, Uri baseUri)
    {
        HttpResponseMessage response;
        try
        {
            response = await http.GetAsync(path);
        }
        catch (HttpRequestException error)
        {
            _errors.Add($"{path} (from {from}): the request failed: {error.Message}");
            _fetched[path] = new Fetched("", [], null);
            return;
        }
        using (response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _errors.Add($"{path} (from {from}): {(int)response.StatusCode} {response.ReasonPhrase}");
                _fetched[path] = new Fetched("", [], null);
                return;
            }
            var body = await response.Content.ReadAsByteArrayAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            _fetched[path] = new Fetched(contentType, body, null);
            var text = IsText(contentType) ? Encoding.UTF8.GetString(body) : null;
            if (text is not null)
            {
                await ScanAsync(path, text, contentType, baseUri);
            }
        }
    }

    private static bool IsText(string contentType) =>
        contentType.StartsWith("text/", StringComparison.Ordinal) || contentType is "application/javascript" or "application/json" or "image/svg+xml";

    private void ScanCss(string css, Uri documentUri, string path)
    {
        foreach (Match url in CssUrl().Matches(css))
        {
            if (TryResolve(url.Groups["v"].Value.Trim(), documentUri, out var target, out _))
            {
                Enqueue(target, path);
            }
        }
    }

    // Looks in a file for the addresses it refers to, and queues the ones that are files of the site.
    private Task ScanAsync(string path, string text, string contentType, Uri baseUri)
    {
        var isHtml = contentType == "text/html";
        var isCss = contentType == "text/css";
        var isJs = contentType is "text/javascript" or "application/javascript";
        // Relative addresses of a page resolve against its <base href> (the path base); the ones of a stylesheet or a script, against the file.
        var documentUri = isHtml ? baseUri : new Uri(baseUri, path);

        if (isHtml)
        {
            var ids = new HashSet<string>(StringComparer.Ordinal);
            foreach (Match id in IdAttribute().Matches(text))
            {
                ids.Add(WebUtility.HtmlDecode(id.Groups["v"].Value));
            }
            _fetched[path] = _fetched[path] with { Ids = ids };

            foreach (Match anchor in Anchor().Matches(text))
            {
                var raw = WebUtility.HtmlDecode(anchor.Groups["v"].Value).Trim();
                if (TryResolve(raw, documentUri, out var target, out var fragment))
                {
                    _links.Add((path, target, fragment, raw));
                    Enqueue(target, path);
                }
            }
            foreach (Match attribute in Attribute().Matches(text))
            {
                var name = attribute.Groups["name"].Value.ToLowerInvariant();
                var value = WebUtility.HtmlDecode(attribute.Groups["v"].Value).Trim();
                if (name == "content")
                {
                    continue;
                }
                var candidates = name is "srcset" or "imagesrcset" ? value.Split(',').Select(c => c.Trim().Split(' ', 2)[0]) : [value];
                foreach (var candidate in candidates)
                {
                    if (TryResolve(candidate, documentUri, out var target, out _))
                    {
                        Enqueue(target, path);
                    }
                }
            }
            foreach (Match initializers in Initializers().Matches(text))
            {
                try
                {
                    using var json = JsonDocument.Parse(Convert.FromBase64String(initializers.Groups["b64"].Value));
                    foreach (var entry in json.RootElement.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String))
                    {
                        if (TryResolve(entry.GetString()!, documentUri, out var target, out _))
                        {
                            Enqueue(target, path + " (JavaScript initializers)");
                        }
                    }
                }
                catch (Exception error) when (error is JsonException or FormatException)
                {
                    _errors.Add($"{path}: the list of JavaScript initializers is not readable.");
                }
            }
            foreach (Match map in ImportMap().Matches(text))
            {
                try
                {
                    using var json = JsonDocument.Parse(map.Groups["json"].Value);
                    if (json.RootElement.TryGetProperty("imports", out var imports) && imports.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var entry in imports.EnumerateObject().Where(e => e.Value.ValueKind == JsonValueKind.String))
                        {
                            if (TryResolve(entry.Name, documentUri, out var from, out _) && TryResolve(entry.Value.GetString()!, documentUri, out var to, out _))
                            {
                                _importMap[from] = to;
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                    _errors.Add($"{path}: the import map is not valid JSON.");
                }
            }
        }

        if (isCss)
        {
            ScanCss(text, documentUri, path);
        }
        else if (isHtml)
        {
            // Only the CSS of the page itself: <style> elements and style attributes. The text of a code block that shows CSS is not a reference.
            foreach (Match style in StyleElement().Matches(text))
            {
                ScanCss(style.Groups["css"].Value, documentUri, path);
            }
            foreach (Match style in StyleAttribute().Matches(text))
            {
                ScanCss(WebUtility.HtmlDecode(style.Groups["v"].Value), documentUri, path);
            }
        }

        // The Blazor script (under _framework/) names the files of runtimes this site does not run (dotnet.js of WebAssembly): its specifiers are not looked at. The scripts of
        // the libraries, which the page does run, are.
        if (isJs && !path.StartsWith(pathBase + "_framework/", StringComparison.Ordinal))
        {
            foreach (Match specifier in JsSpecifier().Matches(text))
            {
                var value = specifier.Groups["v"].Value;
                if (value.StartsWith("./", StringComparison.Ordinal) || value.StartsWith("../", StringComparison.Ordinal))
                {
                    if (TryResolve(value, documentUri, out var target, out _))
                    {
                        Enqueue(target, path);
                    }
                }
            }
        }
        return Task.CompletedTask;
    }

    // An address of the same site, as an absolute path without the query and the fragment. False for an address that is elsewhere or that is not a file (mailto:, data:).
    private bool TryResolve(string raw, Uri documentUri, out string path, out string fragment)
    {
        path = "";
        fragment = "";
        if (raw.Length == 0 || raw[0] == '#' || raw.StartsWith("data:", StringComparison.OrdinalIgnoreCase) || raw.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)
            || raw.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) || raw.StartsWith("tel:", StringComparison.OrdinalIgnoreCase) || raw.StartsWith("blob:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        if (!Uri.TryCreate(documentUri, raw, out var absolute) || absolute.Scheme is not ("http" or "https"))
        {
            return false;
        }
        if (!absolute.Host.Equals(http.BaseAddress!.Host, StringComparison.OrdinalIgnoreCase) || absolute.Port != http.BaseAddress.Port)
        {
            return false;
        }
        path = absolute.AbsolutePath;
        fragment = Uri.UnescapeDataString(absolute.Fragment.TrimStart('#'));
        return true;
    }

    private void CheckLinks()
    {
        foreach (var (from, target, fragment, text) in _links.Distinct())
        {
            if (!target.StartsWith(pathBase, StringComparison.Ordinal) && target != pathBase.TrimEnd('/'))
            {
                _errors.Add($"{from}: the link '{text}' leads to '{target}', outside {pathBase}, where nothing is served on GitHub Pages.");
                continue;
            }
            if (!_fetched.TryGetValue(target, out var found) || found.Body.Length == 0 && found.ContentType.Length == 0)
            {
                // The request already reported it.
                continue;
            }
            if (fragment.Length > 0 && found.ContentType == "text/html" && found.Ids is { } ids && !ids.Contains(fragment))
            {
                _errors.Add($"{from}: the link '{text}' leads to the fragment '#{fragment}', which is not on {target}.");
            }
        }
    }

    // The output path of an address below the path base: a page becomes <path>/index.html, any other file keeps its name.
    private string FileFor(string path, Fetched file)
    {
        var relative = Uri.UnescapeDataString(path[pathBase.Length..]);
        if (file.ContentType == "text/html" && !relative.EndsWith(".html", StringComparison.Ordinal))
        {
            relative = (relative.TrimEnd('/') + "/index.html").TrimStart('/');
        }
        return Path.GetFullPath(Path.Combine(outFolder, relative.Replace('/', Path.DirectorySeparatorChar)));
    }

    private void Write()
    {
        if (Directory.Exists(outFolder) && Directory.EnumerateFileSystemEntries(outFolder).Any())
        {
            // Only an earlier export is replaced: it has this marker.
            if (!File.Exists(Path.Combine(outFolder, ".nojekyll")))
            {
                throw new InvalidOperationException($"{outFolder} is not empty and is not an earlier export (it has no .nojekyll file). Choose an empty folder.");
            }
            foreach (var entry in Directory.EnumerateFileSystemEntries(outFolder))
            {
                if (Directory.Exists(entry))
                {
                    Directory.Delete(entry, true);
                }
                else
                {
                    File.Delete(entry);
                }
            }
        }
        Directory.CreateDirectory(outFolder);
        var root = Path.GetFullPath(outFolder) + Path.DirectorySeparatorChar;
        foreach (var (path, file) in _fetched.OrderBy(f => f.Key, StringComparer.Ordinal))
        {
            if (!path.StartsWith(pathBase, StringComparison.Ordinal) || file.ContentType.Length == 0)
            {
                continue;
            }
            var target = FileFor(path, file);
            if (!target.StartsWith(root, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"{path} would be written outside {outFolder}.");
            }
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.WriteAllBytes(target, file.Body);
        }
        File.WriteAllText(Path.Combine(outFolder, ".nojekyll"), "");
    }
}
