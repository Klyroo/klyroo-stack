namespace Klyroo.Docs.Site.Components.Signage;

/// <summary>Where a package or namespace name may wrap: after a dot and nowhere else, so <c>Klyroo.Query.EntityFrameworkCore</c> becomes <c>Klyroo.</c>, <c>Query.</c>, <c>EntityFrameworkCore</c>.</summary>
public static class DotBreaks
{
    /// <summary>The pieces of <paramref name="text"/>, each ending with the dot that follows it (the last one has none).</summary>
    public static IReadOnlyList<string> Split(string text)
    {
        var pieces = new List<string>();
        var start = 0;
        for (var i = 0; i < text.Length - 1; i++)
        {
            if (text[i] == '.' && char.IsLetter(text[i + 1]))
            {
                pieces.Add(text[start..(i + 1)]);
                start = i + 1;
            }
        }
        pieces.Add(text[start..]);
        return pieces;
    }
}
