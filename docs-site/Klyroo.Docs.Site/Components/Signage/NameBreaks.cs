namespace Klyroo.Docs.Site.Components.Signage;

/// <summary>Where a long type or member name may wrap: between the words of its PascalCase, after a namespace dot and before its type arguments.</summary>
public static class NameBreaks
{
    /// <summary>The pieces of <paramref name="text"/> to set with a soft break between them: <c>DataListExportTooLargeException</c> gives <c>Data</c>, <c>List</c>, <c>Export</c>, <c>Too</c>, <c>Large</c>, <c>Exception</c>.</summary>
    public static IReadOnlyList<string> Split(string text)
    {
        var pieces = new List<string>();
        var start = 0;
        for (var i = 1; i < text.Length; i++)
        {
            var c = text[i];
            var previous = text[i - 1];
            var wordStart = char.IsUpper(c) && (char.IsLower(previous) || char.IsDigit(previous));
            var argumentsStart = c is '<' or '(' && !char.IsWhiteSpace(previous);
            var afterDot = previous == '.' && char.IsLetter(c);
            if (wordStart || argumentsStart || afterDot)
            {
                pieces.Add(text[start..i]);
                start = i;
            }
        }
        pieces.Add(text[start..]);
        return pieces;
    }
}
