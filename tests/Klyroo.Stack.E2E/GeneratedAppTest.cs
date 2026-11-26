namespace Klyroo.Stack.E2E;

/// <summary>
/// The base of the tests of an app generated from a project template (see <see cref="GeneratedApps"/>): a browser context and a page that open the app,
/// the helpers the typed forms need, and the log of the app attached to a failure. The app itself is shared by every test of the run, so a test creates
/// its own data under names no other test uses and never counts rows.
/// </summary>
public abstract class GeneratedAppTest(PlaygroundServer server, GeneratedApps apps, string template) : IAsyncLifetime
{
    private IBrowserContext _context = default!;
    private readonly List<string> _consoleErrors = [];

    protected GeneratedApp App { get; private set; } = default!;

    protected IPage Page { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        GeneratedApps.RequireFeed();
        App = await apps.GetAsync(template);
        _context = await server.Browser.NewContextAsync(new() { BaseURL = App.BaseUrl });
        Page = await _context.NewPageAsync();
        Page.Console += (_, message) => { if (message.Type == "error") lock (_consoleErrors) _consoleErrors.Add(message.Text); };
        Page.PageError += (_, error) => { lock (_consoleErrors) _consoleErrors.Add(error); };
    }

    public async ValueTask DisposeAsync()
    {
        if (_context is not null) await _context.DisposeAsync();
    }

    /// <summary>Opens a page of the app and waits until the Klyroo script has defined its elements.</summary>
    protected async Task OpenAsync(string path)
    {
        await Page.GotoAsync(path);
        await Page.WaitForFunctionAsync("() => !!customElements.get('kly-popover')");
    }

    /// <summary>
    /// Clicks <paramref name="trigger"/> until the dialog it opens is there. The page is prerendered before its circuit is up, and a click on a button the circuit
    /// does not own yet does nothing, so the first click can be lost; nothing on the page tells when the circuit is ready.
    /// </summary>
    protected async Task<ILocator> OpenDialogAsync(ILocator trigger, string dialogName)
    {
        var dialog = Page.GetByRole(AriaRole.Dialog, new() { Name = dialogName });
        for (var attempt = 0; attempt < 8; attempt++)
        {
            await trigger.ClickAsync();
            try
            {
                await dialog.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = attempt == 0 ? 6_000 : 3_000 });
                return dialog;
            }
            catch (PlaywrightException)
            {
                // Not interactive yet: click again.
            }
        }
        Assert.Fail($"The dialog '{dialogName}' did not open.\n{App.Log}");
        return dialog;
    }

    /// <summary>The error summary of the form in <paramref name="dialog"/>: one group above the fields, with a link to each invalid field.</summary>
    protected static ILocator Summary(ILocator dialog) => dialog.Locator("[data-slot=form-error-summary]");

    protected static ILocator SummaryItems(ILocator dialog) => dialog.Locator("[data-slot=form-error-summary-item]");

    protected static ILocator Submit(ILocator dialog) => dialog.Locator("[data-slot=form-action-submit]");

    /// <summary>No error reached the console of the page (the circuit stayed up, no unhandled exception).</summary>
    protected void AssertNoConsoleErrors()
    {
        lock (_consoleErrors) Assert.True(_consoleErrors.Count == 0, "Console errors:\n" + string.Join("\n", _consoleErrors) + "\n" + App.Log);
    }

    /// <summary>A name no other test uses.</summary>
    protected static string Unique(string prefix) => $"{prefix} {Guid.NewGuid().ToString("N")[..6]}";
}
