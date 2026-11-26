using Klyroo.Docs.Site.Content;
using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Tests;

public class DesignTests
{
    public static TheoryData<string> Accents => ["#10b981", "#ff4154", "#00d8ff", "#f59e0b", "#8b5cf6", "#ec4899", "#14b8a6", "#6366f1"];

    [Theory]
    [MemberData(nameof(Accents))]
    public void The_accent_tones_pass_the_contrast_rules_of_their_mode(string accent)
    {
        var theme = AccentColor.Compute(accent);
        Assert.True(AccentColor.Contrast(theme.InkLight, AccentColor.LightDarkestSurface) >= 4.5, "link ink in light mode");
        Assert.True(AccentColor.Contrast(theme.InkDark, AccentColor.DarkLightestSurface) >= 4.5, "link ink in dark mode");
        Assert.True(AccentColor.Contrast(theme.DisplayLight, AccentColor.LightGround) >= 3, "headline in light mode");
        Assert.True(AccentColor.Contrast(theme.DisplayDark, AccentColor.DarkGround) >= 3, "headline in dark mode");
        Assert.True(AccentColor.Contrast(theme.Fill, theme.OnFill) >= 4.5, "label on the accent button");
    }

    private static FakeRepository Repository(string site) => new FakeRepository()
        .Write("README.md", """
            # Klyroo.Demo

            A demo library. It has a features list.

            ## Features

            - **Typed schema**: declare fields with `expressions`.
            - **Headless**: no UI dependency.

            ## Packages

            | Package | Description |
            | :--- | :--- |
            | [`Klyroo.Demo.Core`](https://www.nuget.org/packages/Klyroo.Demo.Core) | Engine, no UI. |
            | [`Klyroo.Demo`](https://www.nuget.org/packages/Klyroo.Demo) | The Blazor components. |

            ## Klyroo ecosystem

            | Library | Description |
            | :--- | :--- |
            | **[Klyroo.Demo](https://github.com/klyroo/klyroo-demo)** | This one. |
            | [Klyroo.Other](https://github.com/klyroo/klyroo-other) | The other one. |

            ## Installation

            Run it.
            """)
        .Write("docs/getting-started.md", "# Getting started\n\nInstall the demo in one step.\n\n## Install\n\n```csharp title=\"Program.cs\"\nvar x = 1;\n```\n")
        .Write("docs/site.yml", site);

    [Fact]
    public void The_new_keys_of_site_yml_are_read()
    {
        using var repo = Repository("""
            headline: |
              First line,
              second line.
            lead: A lead.
            accent: "#FF4154"
            texture: dot-matrix
            demo:
              file: Program.cs
              code: |
                var a = 1;
                var b = 2;
              focus: 2-2
              result:
                title: Output
                output: |
                  3
            install: |
              dotnet add package Klyroo.Demo.Core --prerelease   # the engine
              dotnet add package Klyroo.Demo --prerelease
            """);
        var settings = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries).Settings;

        Assert.Equal(["First line,", "second line."], settings.Headline);
        Assert.Equal("A lead.", settings.Lead);
        Assert.Equal("#ff4154", settings.Accent);
        Assert.Equal("dot-matrix", settings.Texture);
        Assert.Equal((2, 2), settings.Demo!.Focus);
        Assert.Equal("3", settings.Demo.Output);
        Assert.Equal("dotnet add package Klyroo.Demo --prerelease", settings.HeroInstall);
    }

    [Theory]
    [InlineData("accent: blue", "accent")]
    [InlineData("texture: stripes", "stripes")]
    [InlineData("headline: |\n  a\n  b\n  c\n", "3 lines")]
    [InlineData("demo:\n  board: nothing\n", "not a board")]
    [InlineData("demo:\n  code: x\n", "no 'result'")]
    [InlineData("demo:\n  colour: x\n", "colour")]
    public void A_wrong_value_is_an_error_that_names_it(string yaml, string expected)
    {
        using var repo = Repository(yaml);
        var error = Assert.Throws<ContentException>(() => LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries));
        Assert.Contains(expected, error.Message);
    }

    [Fact]
    public void Without_the_keys_the_site_has_the_accent_of_the_family_and_a_headline_from_the_title()
    {
        using var repo = Repository("title: Klyroo.Demo\n");
        var settings = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries).Settings;
        Assert.Equal("#10b981", settings.Accent);
        Assert.Equal("blueprint-grid", settings.Texture);
        Assert.Equal(["Klyroo.Demo", "documentation"], settings.Headline);
    }

    [Fact]
    public void The_features_packages_and_ecosystem_of_the_readme_become_home_content_and_leave_its_body()
    {
        using var repo = Repository("title: Klyroo.Demo\n");
        var site = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries);

        Assert.Equal(["Typed schema", "Headless"], site.HomeContent.Features.Select(f => f.Title));
        Assert.Contains("<code>expressions</code>", site.HomeContent.Features[0].Html);
        Assert.Equal([("Klyroo.Demo.Core", "Core"), ("Klyroo.Demo", "Blazor")], site.HomeContent.Packages.Select(p => (p.Id, p.Layer)));
        Assert.Equal("dotnet add package Klyroo.Demo.Core --prerelease", site.HomeContent.Packages[0].Install);
        Assert.Equal([("Klyroo.Demo", true), ("Klyroo.Other", false)], site.HomeContent.Ecosystem.Select(e => (e.Name, e.Current)));
        Assert.DoesNotContain("Typed schema", site.Home.Html);
        Assert.DoesNotContain("klyroo-ecosystem", site.Home.Html);
        Assert.Contains("id=\"installation\"", site.Home.Html);
    }

    [Fact]
    public void The_first_paragraph_of_a_page_is_its_lead_and_leaves_the_body()
    {
        using var repo = Repository("title: Klyroo.Demo\n");
        var page = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries).Find("getting-started")!;
        Assert.Equal("Install the demo in one step.", page.LeadHtml);
        Assert.DoesNotContain("Install the demo in one step.", page.Html);
    }

    [Fact]
    public void A_description_of_site_yml_is_the_lead_and_the_paragraph_stays()
    {
        using var repo = Repository("nav:\n  - page: getting-started\n    description: Short.\n");
        var page = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries).Find("getting-started")!;
        Assert.Equal("Short.", page.LeadHtml);
        Assert.Contains("Install the demo in one step.", page.Html);
    }

    [Fact]
    public void A_code_block_has_a_bar_with_the_language_or_the_file_name_and_a_copy_button()
    {
        using var repo = Repository("title: Klyroo.Demo\n");
        var html = LibraryDocs.Load(repo.Root, "klyroo-demo", FakeRepository.Libraries).Find("getting-started")!.Html;
        Assert.Contains("<span class=\"code-title\">Program.cs</span>", html);
        Assert.Contains("data-code-copy", html);
        Assert.Contains("<h2 id=\"install\">", html);
        Assert.Contains("class=\"heading-anchor\" href=\"getting-started/#install\"", html);

        var bash = CodeBlockHtml.Render("dotnet build", "bash");
        Assert.Contains("<span class=\"code-title\">Shell</span>", bash);
        Assert.DoesNotContain("BASH", bash);
        Assert.Contains("<span class=\"code-title\"></span>", CodeBlockHtml.Render("plain", null));
    }
}
