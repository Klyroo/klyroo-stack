namespace Klyroo.Playground.Shared;

/// <summary>Finds the lines of a code that a change of a knob wrote: the lines of the new text that the old text did not have, as ranges of line numbers counted from 1.</summary>
public static class CodeDiff
{
    /// <summary>
    /// The lines of <paramref name="after"/> that are not part of the longest run of lines the two texts share, in order. A change that rewrites most of the code (another component, another
    /// template) is a replacement, not an edit, so it marks nothing.
    /// </summary>
    public static IReadOnlyList<CodeLineRange> ChangedLines(string before, string after)
    {
        var old = Lines(before);
        var now = Lines(after);
        if (now.Length == 0)
        {
            return [];
        }

        // Longest common subsequence of the lines: what is in it was already there, the rest is what the knob wrote.
        var table = new int[old.Length + 1, now.Length + 1];
        for (var i = old.Length - 1; i >= 0; i--)
        {
            for (var j = now.Length - 1; j >= 0; j--)
            {
                table[i, j] = old[i] == now[j] ? table[i + 1, j + 1] + 1 : Math.Max(table[i + 1, j], table[i, j + 1]);
            }
        }

        var kept = new bool[now.Length];
        for (int i = 0, j = 0; i < old.Length && j < now.Length;)
        {
            if (old[i] == now[j])
            {
                kept[j] = true;
                i++;
                j++;
            }
            else if (table[i + 1, j] >= table[i, j + 1])
            {
                i++;
            }
            else
            {
                j++;
            }
        }

        var changed = kept.Count(k => !k);
        if (changed == 0 || (now.Length > 4 && changed * 10 > now.Length * 7))
        {
            return [];
        }

        var ranges = new List<CodeLineRange>();
        for (var line = 0; line < now.Length; line++)
        {
            if (kept[line])
            {
                continue;
            }
            var start = line;
            while (line + 1 < now.Length && !kept[line + 1])
            {
                line++;
            }
            ranges.Add(new CodeLineRange(start + 1, line + 1));
        }
        return ranges;
    }

    private static string[] Lines(string text) =>
        text.Length == 0 ? [] : text.TrimEnd('\n').Replace("\r\n", "\n").Split('\n');
}
