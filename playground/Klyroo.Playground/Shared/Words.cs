using System.Text;

namespace Klyroo.Playground.Shared;

/// <summary>Small text helpers shared by the playgrounds: one way to write an enum value, a count and a date.</summary>
public static class Words
{
    /// <summary><c>OnLeave</c> becomes <c>On leave</c>.</summary>
    public static string Humanize(string name)
    {
        var sb = new StringBuilder(name.Length + 4);
        for (var i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1])) sb.Append(' ').Append(char.ToLowerInvariant(name[i]));
            else sb.Append(name[i]);
        }
        return sb.ToString();
    }

    public static string Plural(int n, string one, string? many = null) => $"{n} {(n == 1 ? one : many ?? one + "s")}";

    /// <summary>The date format of every playground.</summary>
    public const string Date = "yyyy-MM-dd";
}
