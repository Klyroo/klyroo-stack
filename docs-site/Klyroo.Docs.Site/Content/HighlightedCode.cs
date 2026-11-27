using System.Net;
using System.Text;
using Klyroo.Display;

namespace Klyroo.Docs.Site.Content;

/// <summary>
/// The code of a page as HTML, colored by the <see cref="CodeHighlighter"/> of Klyroo.Display.Core: each colored token is a <c>&lt;span class="tok-..."&gt;</c> and nothing else, so the colors
/// live in the stylesheet (<c>--tok-*</c> in <c>tokens.css</c>, which follow light and dark mode). The highlighter knows 13 languages; any other language is shown without color.
/// The output is HTML-encoded.
/// </summary>
public static class HighlightedCode
{
    /// <summary>The language of a fence or a board line (<c>csharp</c>, <c>cs</c>, <c>razor</c>, <c>sh</c>...), or null when the highlighter does not know it.</summary>
    public static CodeLanguage? Resolve(string? language) => CodeHighlighter.ResolveLanguage(language);

    /// <summary>The class of the stylesheet that colors a token category, or null for text that stays in the text color. A function takes the color of a command, as the shell commands of the previous highlighter did.</summary>
    public static string? ClassOf(CodeTokenKind kind) => kind switch
    {
        CodeTokenKind.Keyword => "tok-keyword",
        CodeTokenKind.String => "tok-string",
        CodeTokenKind.Number => "tok-number",
        CodeTokenKind.Comment => "tok-comment",
        CodeTokenKind.Type => "tok-type",
        CodeTokenKind.Function => "tok-command",
        CodeTokenKind.Property => "tok-property",
        CodeTokenKind.Tag => "tok-tag",
        _ => null,
    };

    /// <summary>The code as HTML: encoded, with the tokens of a known language in spans.</summary>
    public static string ToHtml(string code, string? language)
    {
        var resolved = Resolve(language);
        if (resolved is null)
        {
            return WebUtility.HtmlEncode(code);
        }

        var html = new StringBuilder(code.Length * 2);
        foreach (var token in CodeHighlighter.Tokenize(code, resolved))
        {
            var text = WebUtility.HtmlEncode(token.Text);
            if (ClassOf(token.Kind) is { } cssClass)
            {
                html.Append("<span class=\"").Append(cssClass).Append("\">").Append(text).Append("</span>");
            }
            else
            {
                html.Append(text);
            }
        }
        return html.ToString();
    }
}
