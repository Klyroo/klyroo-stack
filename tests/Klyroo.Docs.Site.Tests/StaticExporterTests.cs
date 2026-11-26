using System.Net;
using System.Text;
using Klyroo.Docs.Site.Content;
using Klyroo.Docs.Site.Export;
using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Tests;

public class StaticExporterTests : IDisposable
{
    private readonly string _out = Path.Combine(Path.GetTempPath(), "klyroo-export-tests-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_out))
        {
            Directory.Delete(_out, recursive: true);
        }
    }

    // A server that answers from a table of path to (status, content type, body), below /klyroo-demo/. The 404 page answers anything else.
    private sealed class FakeServer(Dictionary<string, (HttpStatusCode Status, string Type, string Body)> files) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri!.AbsolutePath;
            var (status, type, body) = files.TryGetValue(path, out var found) ? found : (HttpStatusCode.NotFound, "text/html", "<html><head><base href=\"/klyroo-demo/\"></head><body><h1>Not found</h1></body></html>");
            return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, type) });
        }
    }

    private static string Page(string body) =>
        $"<html><head><base href=\"/klyroo-demo/\" /><link rel=\"stylesheet\" href=\"app.abc.css\" /><script src=\"app.abc.js\"></script></head><body><h1>Title</h1>{body}</body></html>";

    private static DocsSite Site()
    {
        using var repo = new FakeRepository().Write("README.md", "# Klyroo.Demo\n\nText.\n").Write("docs/guides.md", "# Guides\n\n## One\n");
        return LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);
    }

    private static Dictionary<string, (HttpStatusCode, string, string)> Files(string homeBody, string guidesBody = "<h2 id=\"one\">One</h2>") => new()
    {
        ["/klyroo-demo/"] = (HttpStatusCode.OK, "text/html", Page(homeBody)),
        ["/klyroo-demo/guides/"] = (HttpStatusCode.OK, "text/html", Page(guidesBody)),
        ["/klyroo-demo/overview/"] = (HttpStatusCode.OK, "text/html", Page("<p>The rest of the README.</p>")),
        ["/klyroo-demo/search-index.json"] = (HttpStatusCode.OK, "application/json", "[]"),
        ["/klyroo-demo/app.abc.css"] = (HttpStatusCode.OK, "text/css", "body{background:url(\"font.woff2\")}"),
        ["/klyroo-demo/font.woff2"] = (HttpStatusCode.OK, "font/woff2", "font"),
        ["/klyroo-demo/app.abc.js"] = (HttpStatusCode.OK, "text/javascript", "import './chunk.js'; export const a = 1;"),
        ["/klyroo-demo/chunk.js"] = (HttpStatusCode.OK, "text/javascript", "export const b = 2;"),
    };

    private async Task<(bool Ok, string Log)> ExportAsync(Dictionary<string, (HttpStatusCode, string, string)> files)
    {
        using var http = new HttpClient(new FakeServer(files)) { BaseAddress = new Uri("http://localhost:5000") };
        var log = new StringWriter();
        var ok = await new StaticExporter(http, "/klyroo-demo/", _out, Site()).RunAsync(log);
        return (ok, log.ToString());
    }

    [Fact]
    public async Task Writes_pages_as_folders_with_the_assets_the_html_css_and_scripts_refer_to()
    {
        var (ok, log) = await ExportAsync(Files("<a href=\"guides/#one\">guides</a><a href=\"./\">home</a><a href=\"https://example.com/\">out</a>"));

        Assert.True(ok, log);
        Assert.True(File.Exists(Path.Combine(_out, "index.html")));
        Assert.True(File.Exists(Path.Combine(_out, "guides", "index.html")));
        Assert.True(File.Exists(Path.Combine(_out, "404.html")));
        Assert.True(File.Exists(Path.Combine(_out, ".nojekyll")));
        Assert.True(File.Exists(Path.Combine(_out, "search-index.json")));
        Assert.True(File.Exists(Path.Combine(_out, "app.abc.css")));
        Assert.True(File.Exists(Path.Combine(_out, "font.woff2")), "a font named in the CSS");
        Assert.True(File.Exists(Path.Combine(_out, "chunk.js")), "a module imported by a script");
    }

    [Fact]
    public async Task Fails_on_a_link_to_a_page_that_does_not_exist()
    {
        var (ok, log) = await ExportAsync(Files("<a href=\"nowhere/\">x</a>"));
        Assert.False(ok);
        Assert.Contains("/klyroo-demo/nowhere/", log);
        Assert.False(Directory.Exists(_out), "nothing is written when the export fails");
    }

    [Fact]
    public async Task Fails_on_a_link_to_a_fragment_the_page_does_not_have()
    {
        var (ok, log) = await ExportAsync(Files("<a href=\"guides/#two\">x</a>"));
        Assert.False(ok);
        Assert.Contains("#two", log);
    }

    [Fact]
    public async Task Fails_on_a_stylesheet_that_does_not_answer()
    {
        var files = Files("<p>x</p>");
        files.Remove("/klyroo-demo/app.abc.css");
        var (ok, log) = await ExportAsync(files);
        Assert.False(ok);
        Assert.Contains("app.abc.css", log);
    }

    [Fact]
    public async Task Fails_on_a_link_outside_the_path_base()
    {
        var (ok, log) = await ExportAsync(Files("<a href=\"/elsewhere\">x</a>"));
        Assert.False(ok);
        Assert.Contains("outside /klyroo-demo/", log);
    }

    [Fact]
    public async Task Does_not_take_the_text_of_a_code_block_for_a_reference()
    {
        var (ok, log) = await ExportAsync(Files("<pre><code>background: url(&quot;/img/team.jpg&quot;); src=&quot;x.png&quot;</code></pre>"));
        Assert.True(ok, log);
    }

    [Fact]
    public async Task Refuses_to_replace_a_folder_that_is_not_an_earlier_export()
    {
        Directory.CreateDirectory(_out);
        File.WriteAllText(Path.Combine(_out, "precious.txt"), "keep me");
        await Assert.ThrowsAsync<InvalidOperationException>(() => ExportAsync(Files("<p>x</p>")));
        Assert.True(File.Exists(Path.Combine(_out, "precious.txt")));
    }
}
