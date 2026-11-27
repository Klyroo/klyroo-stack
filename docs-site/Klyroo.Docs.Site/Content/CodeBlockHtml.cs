using System.Net;
using System.Text.RegularExpressions;

namespace Klyroo.Docs.Site.Content;

/// <summary>
/// The markup of a code block: a bar with the language (or the file name) and the copy button, then the highlighted code. The Markdown renderer writes it for every
/// fenced block, and the home page for the snippets it shows, so every block of every page looks and behaves the same.
/// </summary>
public static partial class CodeBlockHtml
{
    // Two drawn icons in one stroke, a copy and a check: the stylesheet shows the check while the button says "Copied".
    private const string CopyIcons =
        "<svg class=\"code-copy-icon\" viewBox=\"0 0 16 16\" width=\"14\" height=\"14\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\"><rect x=\"5.5\" y=\"5.5\" width=\"8\" height=\"8\" rx=\"1.75\"/><path d=\"M10.5 3.5v-.25A1.75 1.75 0 0 0 8.75 1.5h-5A1.75 1.75 0 0 0 2 3.25v5C2 9.2 2.8 10 3.75 10h.25\"/></svg>" +
        "<svg class=\"code-check-icon\" viewBox=\"0 0 16 16\" width=\"14\" height=\"14\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.75\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\"><path d=\"m3.5 8.5 3 3 6-7\"/></svg>";

    [GeneratedRegex(@"title=""([^""]+)""")]
    private static partial Regex Title();

    /// <summary>The block for <paramref name="code"/>. <paramref name="info"/> is the info string of the fence (<c>csharp title="Program.cs"</c>), which can name a file for the bar.</summary>
    public static string Render(string code, string? language, string? info = null)
    {
        var label = language is not null && HighlightedCode.Resolve(language) is not null ? language.ToLowerInvariant() : "text";
        var title = info is not null && Title().Match(info) is { Success: true } named ? named.Groups[1].Value : DisplayName(language, label);
        return $"<div class=\"code-block\" data-language=\"{WebUtility.HtmlEncode(label)}\">" +
            $"<div class=\"code-head\"><span class=\"code-title\">{WebUtility.HtmlEncode(title)}</span>" +
            $"<button type=\"button\" class=\"code-copy\" data-code-copy aria-label=\"Copy code\">{CopyIcons}<span class=\"code-copy-label\">Copy</span></button></div>" +
            "<span class=\"code-copy-status\" role=\"status\" aria-live=\"polite\"></span>" +
            $"<pre tabindex=\"0\"><code>{HighlightedCode.ToHtml(code, language)}</code></pre></div>";
    }

    private static string DisplayName(string? language, string label) =>
        (language ?? "").ToLowerInvariant() switch
        {
            "csharp" or "cs" or "c#" => "C#",
            "razor" => "Razor",
            "bash" or "sh" or "shell" or "console" or "zsh" => "Shell",
            "json" or "jsonc" => "JSON",
            "yaml" or "yml" => "YAML",
            "toml" => "TOML",
            "html" => "HTML",
            "xml" => "XML",
            "svg" => "SVG",
            "css" => "CSS",
            "sql" => "SQL",
            "text" or "txt" or "" => "",
            var other => label == "text" && other.Length <= 14 ? other : "",
        };
}
