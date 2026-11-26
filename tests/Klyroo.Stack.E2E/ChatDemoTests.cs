using System.Text.RegularExpressions;

namespace Klyroo.Stack.E2E;

/// <summary>The Chat demo behind the docs host: the same app through the reverse proxy, answering from canned replies, with the conversations of each visitor kept apart.</summary>
public class ChatDemoTests(PlaygroundServer server) : PlaygroundTest(server)
{
    private const string DemoPath = "/templates/chat/demo/";

    [Fact]
    public async Task The_demo_answers_from_canned_replies_through_the_proxy_and_keeps_visitors_apart()
    {
        if (!TemplatesReady)
        {
            Assert.False(Environment.GetEnvironmentVariable("CI") is { Length: > 0 }, "The template demos are not running.");
            Assert.SkipWhen(true, "The docs run published with no KLYROO_TEMPLATES_PUBLISHED folder: there is no template demo to open.");
        }
        await Page.Context.GrantPermissionsAsync(["clipboard-read", "clipboard-write"]);
        var requests = new List<string>();
        Page.Request += (_, request) => { lock (requests) requests.Add(request.Url); };
        await Page.GotoAsync(DemoPath.TrimStart('/'));
        await ChatChecks.WaitInteractive(Page);
        await ChatChecks.AssertNewConversation(Page);
        await ChatChecks.AssertFirstConversation(Page, "/templates/chat/demo");
        await ChatChecks.AssertAnswersAreSanitised(Page);

        // The configuration says Simulated: the model list is the canned one, and only it.
        var models = await Page.Context.APIRequest.GetAsync(DemoPath.TrimStart('/') + "api/assistant/models");
        Assert.Contains("demo-thoughtful", await models.TextAsync());

        // Every request of the page, the endpoint of the host and the circuit included, is below the path base of the demo.
        var origin = new Uri(BaseUrl).GetLeftPart(UriPartial.Authority);
        lock (requests)
        {
            var outside = requests.Where(url => url.StartsWith(origin + "/", StringComparison.Ordinal) && !url.Contains("/_framework/dotnet-browser-tools/", StringComparison.Ordinal))
                .Where(url => !url.StartsWith(origin + DemoPath, StringComparison.Ordinal)).ToList();
            Assert.True(outside.Count == 0, "Requests outside the demo:\n" + string.Join("\n", outside));
        }

        // Another visitor, another storage: nothing of the first one's conversations.
        var other = await NewContext(new());
        var visitor = await other.NewPageAsync();
        await visitor.GotoAsync(DemoPath.TrimStart('/'));
        await ChatChecks.WaitInteractive(visitor);
        await Assertions.Expect(visitor.GetByTestId("conversations-empty")).ToBeVisibleAsync();
    }
}
