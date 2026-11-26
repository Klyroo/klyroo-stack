using System.Diagnostics;
using Xunit;

namespace Klyroo.Stack.Tests;

/// <summary>
/// klyroo-stack.css is loaded after klyroo.css, and both put their utilities in one cascade layer. A rule that the two sheets share is declared twice in that
/// layer and the later copy wins over the rules between them, which shifted the nav items of the header and put the search icon over its placeholder.
/// </summary>
public sealed class StackStylesheetTests
{
    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Klyroo.Stack.slnx"))) directory = directory.Parent;
        return directory?.FullName ?? throw new InvalidOperationException("Klyroo.Stack.slnx was not found above the test output.");
    }

    [Fact]
    public async Task The_stack_stylesheet_holds_no_utility_rule_that_klyroo_css_already_holds()
    {
        var root = RepositoryRoot();
        Assert.SkipUnless(File.Exists(Path.Combine(root, "..", "klyroo-ui", "src", "Klyroo.UI", "wwwroot", "klyroo.css")), "klyroo-ui is not next to this repository.");

        ProcessStartInfo start = new("node", ["build/dedupe-stack-css.mjs", "--check"])
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        Process? process;
        try
        {
            process = Process.Start(start);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            Assert.Skip("node is not installed.");
            return;
        }

        using (process)
        {
            Assert.NotNull(process);
            var error = await process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
            await process.WaitForExitAsync(TestContext.Current.CancellationToken);
            Assert.True(process.ExitCode == 0, error);
        }
    }
}
