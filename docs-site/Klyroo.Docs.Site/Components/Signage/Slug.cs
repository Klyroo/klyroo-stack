using System.Text;

namespace Klyroo.Docs.Site.Components.Signage;

/// <summary>The ids of the parts of the home page that links point at.</summary>
public static class Slug
{
    /// <summary>The id of a feature of the README on the home page: <c>feature-state-in-the-url</c>.</summary>
    public static string Feature(string title)
    {
        var text = new StringBuilder("feature");
        var gap = true;
        foreach (var c in title.ToLowerInvariant())
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                text.Append(gap ? "-" : "").Append(c);
                gap = false;
            }
            else
            {
                gap = true;
            }
        }
        return text.ToString();
    }
}
