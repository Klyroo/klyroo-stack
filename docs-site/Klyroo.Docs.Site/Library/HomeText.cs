namespace Klyroo.Docs.Site.Library;

/// <summary>Small fixes to the sentences the home page takes from a README.</summary>
public static class HomeText
{
    /// <summary>
    /// The description of a feature starts a sentence under its title (<c>**Title**: required and...</c>), so its first letter is a capital. A description that opens with markup
    /// (a <c>&lt;code&gt;</c> identifier, a link) or a character that is not a letter is left as the README wrote it, since an identifier's case is part of its name.
    /// </summary>
    public static string Capitalize(string html) =>
        html.Length > 0 && char.IsLower(html[0]) ? char.ToUpperInvariant(html[0]) + html[1..] : html;
}
