using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace Klyroo.Stack.Tests;

/// <summary>
/// Checks of the template sources that need no <c>dotnet new</c>: what a template says about itself, and that the markup only uses classes a stylesheet it
/// loads actually defines. The apps load precompiled sheets, not a Tailwind build of their own markup, so a utility class that none of them holds does nothing.
/// </summary>
public sealed partial class TemplateContentTests
{
    private static readonly string Root = FindRoot();

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Klyroo.Stack.slnx"))) directory = directory.Parent;
        return directory?.FullName ?? throw new InvalidOperationException("Klyroo.Stack.slnx was not found above the test output.");
    }

    private static string ContentDirectory => Path.Combine(Root, "templates", "Klyroo.Templates", "content");

    public static TheoryData<string> Templates() => new(Directory.GetDirectories(ContentDirectory).Select(d => Path.GetFileName(d)!).Order());

    [Theory]
    [MemberData(nameof(Templates))]
    public void A_template_names_itself_and_takes_the_package_version_from_one_placeholder(string template)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(ContentDirectory, template, ".template.config", "template.json")));
        var root = json.RootElement;

        Assert.Equal(template, root.GetProperty("shortName").GetString());
        Assert.StartsWith("Klyroo.Templates.", root.GetProperty("identity").GetString());
        Assert.True(root.TryGetProperty("sourceName", out _));

        // The pre-pack target of Klyroo.Templates.csproj replaces this value with the version being packed. A second place that carries a version would
        // go stale at the next release.
        var version = root.GetProperty("symbols").GetProperty("KlyrooVersion");
        Assert.Equal("generated", version.GetProperty("type").GetString());
        Assert.Equal("constant", version.GetProperty("generator").GetString());
        Assert.Equal("0.0.0-unstamped", version.GetProperty("parameters").GetProperty("value").GetString());
        Assert.Equal("KLYROO_VERSION", version.GetProperty("replaces").GetString());
    }

    [Theory]
    [MemberData(nameof(Templates))]
    public void No_template_file_has_a_hard_coded_package_version(string template)
    {
        foreach (var file in Directory.EnumerateFiles(Path.Combine(ContentDirectory, template), "*", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}.template.config{Path.DirectorySeparatorChar}") || !IsText(file)) continue;
            var text = File.ReadAllText(file);
            Assert.DoesNotMatch(@"Klyroo\.[A-Za-z.]+"" Version=""\d", text);
            Assert.DoesNotContain("0.1.0-preview", text);
        }
    }

    [Theory]
    [InlineData("klyroo-saas")]
    [InlineData("klyroo-admin")]
    [InlineData("klyroo-maui")]
    public void The_markup_only_uses_classes_that_a_loaded_stylesheet_defines(string template)
    {
        var ui = Path.Combine(Root, "..", "klyroo-ui", "src", "Klyroo.UI", "wwwroot", "klyroo.css");
        Assert.SkipUnless(File.Exists(ui), "klyroo-ui is not next to this repository.");

        var defined = ClassesOf(ui);
        defined.UnionWith(ClassesOf(Path.Combine(Root, "src", "Klyroo.Stack", "wwwroot", "klyroo-stack.css")));
        foreach (var css in Directory.EnumerateFiles(Path.Combine(ContentDirectory, template), "*.css", SearchOption.AllDirectories)) defined.UnionWith(ClassesOf(css));

        var missing = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var file in Directory.EnumerateFiles(Path.Combine(ContentDirectory, template), "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".razor") || f.EndsWith(".html")))
        {
            foreach (Match attribute in ClassAttribute().Matches(File.ReadAllText(file)))
            {
                foreach (var token in attribute.Groups["value"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                {
                    // Names the Blazor framework or a script of the page uses, not styles.
                    if (token is "reload" or "dismiss") continue;
                    if (!defined.Contains(token)) missing.Add($"{Path.GetFileName(file)}: {token}");
                }
            }
        }

        Assert.True(missing.Count == 0, "Classes that no stylesheet of the template defines:\n" + string.Join('\n', missing));
    }

    private static bool IsText(string file) => Path.GetExtension(file) is not (".png" or ".ico" or ".jpg" or ".svg");

    private static HashSet<string> ClassesOf(string cssFile)
    {
        var classes = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match match in CssClass().Matches(File.ReadAllText(cssFile))) classes.Add(Unescape().Replace(match.Groups[1].Value, "$1"));
        return classes;
    }

    // class="a b" and Class="a b" with no Razor expression inside.
    [GeneratedRegex(@"\b[cC]lass=""(?<value>[^""@{}]*)""")]
    private static partial Regex ClassAttribute();

    [GeneratedRegex(@"\.((?:\\.|[A-Za-z0-9_-])+)")]
    private static partial Regex CssClass();

    [GeneratedRegex(@"\\(.)")]
    private static partial Regex Unescape();
}
