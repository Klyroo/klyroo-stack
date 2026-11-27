namespace Klyroo.Docs.Site.Components.Signage;

/// <summary>Where a long type or member name may wrap: between the words of its PascalCase, after a namespace dot, after a path separator and before its type arguments.</summary>
public static class NameBreaks
{
    /// <summary>The longest run of letters that <see cref="SplitCode"/> keeps in one piece: a name of this length or less is read as one word (<c>KlyFormEngine</c>), a longer one is cut between its words.</summary>
    public const int ShortWord = 16;

    /// <summary>The pieces of <paramref name="text"/> to set with a soft break between them: <c>DataListExportTooLargeException</c> gives <c>Data</c>, <c>List</c>, <c>Export</c>, <c>Too</c>, <c>Large</c>, <c>Exception</c>.</summary>
    public static IReadOnlyList<string> Split(string text) => Cut(text, humps: true);

    /// <summary>
    /// The pieces of an identifier written in running text (inline code). It wraps after a dot or a path separator and before type arguments or parentheses,
    /// and between the words of a long name, but a short name (<see cref="ShortWord"/> characters or fewer) stays whole: <c>KlyFormEngine&lt;TModel&gt;</c>
    /// gives <c>KlyFormEngine</c> and <c>&lt;TModel&gt;</c>, never <c>Kly</c> and <c>FormEngine</c>.
    /// </summary>
    public static IReadOnlyList<string> SplitCode(string text)
    {
        var pieces = new List<string>();
        foreach (var piece in Cut(text, humps: false))
        {
            if (piece.Length > ShortWord)
            {
                pieces.AddRange(Cut(piece, humps: true));
            }
            else
            {
                pieces.Add(piece);
            }
        }
        return pieces;
    }

    private static List<string> Cut(string text, bool humps)
    {
        var pieces = new List<string>();
        var start = 0;
        for (var i = 1; i < text.Length; i++)
        {
            var c = text[i];
            var previous = text[i - 1];
            var wordStart = humps && char.IsUpper(c) && (char.IsLower(previous) || char.IsDigit(previous));
            var argumentsStart = c is '<' or '(' && !char.IsWhiteSpace(previous) && !(c == '(' && i + 1 < text.Length && text[i + 1] == ')');
            var afterDot = previous == '.' && char.IsLetter(c);
            var afterSeparator = previous is '/' or '\\' or '_' && char.IsLetterOrDigit(c);
            if (wordStart || argumentsStart || afterDot || afterSeparator)
            {
                pieces.Add(text[start..i]);
                start = i;
            }
        }
        pieces.Add(text[start..]);
        return pieces;
    }
}
