using System.Text.RegularExpressions;

namespace Klyroo.Stack.E2E;

/// <summary>The Editor demo behind the docs host: the same app through the reverse proxy, with the documents of each visitor kept apart.</summary>
public class EditorDemoTests(PlaygroundServer server) : PlaygroundTest(server)
{
    private const string DemoPath = "/templates/editor/demo/";

    [Fact]
    public async Task The_demo_works_through_the_proxy_and_keeps_visitors_apart()
    {
        if (!TemplatesReady)
        {
            Assert.False(Environment.GetEnvironmentVariable("CI") is { Length: > 0 }, "The template demos are not running.");
            Assert.SkipWhen(true, "The docs run published with no KLYROO_TEMPLATES_PUBLISHED folder: there is no template demo to open.");
        }
        var requests = new List<string>();
        Page.Request += (_, request) => { lock (requests) requests.Add(request.Url); };
        await Page.GotoAsync(DemoPath.TrimStart('/'));
        await EditorChecks.WaitInteractive(Page);
        await EditorChecks.AssertEmptyHome(Page);
        await EditorChecks.AssertWritingAndAutosave(Page, "/templates/editor/demo");
        await EditorChecks.AssertContentIsSanitised(Page, "/templates/editor/demo");

        var origin = new Uri(BaseUrl).GetLeftPart(UriPartial.Authority);
        lock (requests)
        {
            var outside = requests.Where(url => url.StartsWith(origin + "/", StringComparison.Ordinal) && !url.Contains("/_framework/dotnet-browser-tools/", StringComparison.Ordinal))
                .Where(url => !url.StartsWith(origin + DemoPath, StringComparison.Ordinal)).ToList();
            Assert.True(outside.Count == 0, "Requests outside the demo:\n" + string.Join("\n", outside));
        }

        // Another visitor, another storage: nothing of the first one's documents.
        var other = await NewContext(new());
        var visitor = await other.NewPageAsync();
        await visitor.GotoAsync(DemoPath.TrimStart('/'));
        await EditorChecks.WaitInteractive(visitor);
        await Assertions.Expect(visitor.GetByTestId("documents-empty")).ToBeVisibleAsync();
    }
}
