using System.Text.Json;
using Klyroo.Display;
using Klyroo.Docs.Site.Content;
using Microsoft.AspNetCore.Components;

namespace Klyroo.Docs.Site.Components.Boards;

/// <summary>Helpers the result panels of the boards share.</summary>
public static class BoardMarkup
{
    /// <summary>The text with the colors of a language (<c>json</c>, <c>csharp</c>, <c>html</c>...) as markup: the panels show real output in the colors of the code panel.</summary>
    public static MarkupString Code(string text, string language = "json") => new(HighlightedCode.ToHtml(text, language));
}

/// <summary>A line of the code panel of a board. <paramref name="Features"/> lists the chips (separated by spaces) whose choice lights the line.</summary>
/// <param name="Text">The code of the line, as written (indentation included).</param>
/// <param name="Language">The highlighter of the line: <c>csharp</c>, <c>html</c> (markup) or <c>text</c>.</param>
public sealed record BoardLine(string Text, string Features = "", string Language = "csharp");

/// <summary>A feature the reader can pick on a board: its id (what <c>data-lines</c> and <c>data-target</c> name), its label and the sentence under the code while it is picked.</summary>
public sealed record BoardChip(string Id, string Label, string Caption);

/// <summary>
/// The code panel of a board: its language and the lines of each feature, and the colors of its <see cref="KlyCodeBlock"/>. The token classes are the <c>--tok-*</c> colors of the
/// site, the same ones the code blocks of the pages use (see <see cref="HighlightedCode"/>).
/// </summary>
public static class BoardCodeUi
{
    /// <summary>The colors of the code of a board and of a block of the showcase: the tokens of Klyroo.Display.Core on the colors of DESIGN.md.</summary>
    public static readonly CodeBlockUi Block = new()
    {
        TokenKeyword = "text-(--tok-keyword)",
        TokenString = "text-(--tok-string)",
        TokenNumber = "text-(--tok-number)",
        TokenComment = "text-(--tok-comment)",
        TokenType = "text-(--tok-type)",
        TokenFunction = "text-(--tok-command)",
        TokenProperty = "text-(--tok-property)",
        TokenTag = "text-(--tok-tag)",
        TokenPunctuation = "text-inherit",
    };

    /// <summary>The language that most non-empty lines are written in. Markup lines (<c>html</c>) win only when every line is markup, since the C# scanner reads a razor tag better than the HTML one reads C#.</summary>
    public static CodeLanguage? LanguageOf(IReadOnlyList<BoardLine> lines)
    {
        var named = lines.Where(line => line.Text.Trim().Length > 0).Select(line => line.Language).ToList();
        if (named.Count == 0)
        {
            return null;
        }
        var winner = named.All(language => HighlightedCode.Resolve(language) == CodeLanguage.Html)
            ? "html"
            : named.Where(language => HighlightedCode.Resolve(language) != CodeLanguage.Html).GroupBy(language => language).OrderByDescending(group => group.Count()).First().Key;
        return HighlightedCode.Resolve(winner);
    }

    /// <summary>The feature ids of the lines, each with the 1-based numbers of the lines that list it.</summary>
    public static string FeatureLines(IReadOnlyList<BoardLine> lines)
    {
        var features = new Dictionary<string, List<int>>();
        for (var index = 0; index < lines.Count; index++)
        {
            foreach (var feature in lines[index].Features.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                if (!features.TryGetValue(feature, out var numbers))
                {
                    features[feature] = numbers = [];
                }
                numbers.Add(index + 1);
            }
        }
        return JsonSerializer.Serialize(features);
    }
}
