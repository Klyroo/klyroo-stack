using System.Globalization;

namespace Klyroo.Docs.Site.Library;

/// <summary>
/// The colors a site derives from its accent: the accent itself for fills, the text color on a fill, and the tones to use for text and links on
/// the ground of each mode. Each tone is the accent moved along its lightness, just far enough to pass the contrast it is meant for, so the
/// site keeps the hue of the library where it can.
/// </summary>
/// <param name="Fill">The accent as written (<c>#rrggbb</c>): buttons, marks, the chart of the board.</param>
/// <param name="OnFill">The text color on <paramref name="Fill"/>: near-black or white, whichever has more contrast (at least 4.5:1 for every accent of the family).</param>
/// <param name="InkLight">Text and links in light mode: at least 4.5:1 on the darkest surface a link can sit on.</param>
/// <param name="InkDark">Text and links in dark mode: at least 4.5:1 on the lightest surface a link can sit on.</param>
/// <param name="DisplayLight">Large display text in light mode (the headline): at least 3:1 on the ground, so it stays closer to the accent than <paramref name="InkLight"/>.</param>
/// <param name="DisplayDark">Large display text in dark mode: at least 3:1 on the ground.</param>
public sealed record AccentTheme(string Fill, string OnFill, string InkLight, string InkDark, string DisplayLight, string DisplayDark);

/// <summary>Parses and measures colors and computes the <see cref="AccentTheme"/> of an accent.</summary>
public static class AccentColor
{
    // The surfaces of Styles/tokens.css, the lightest and darkest ones a text can sit on in each mode, and the near-black ink used on a fill.
    // Keep them in step with the stylesheet: the tests measure the tones against these.
    public const string LightGround = "#f4f6f9";
    public const string LightDarkestSurface = "#e6eaf0";
    public const string DarkGround = "#090c11";
    public const string DarkLightestSurface = "#1a212b";
    public const string Ink = "#090c11";

    private const double TextContrast = 4.6;
    private const double DisplayContrast = 3.2;

    public static bool TryParse(string? text, out (byte R, byte G, byte B) color)
    {
        color = default;
        if (text is not { Length: 7 } || text[0] != '#' ||
            !byte.TryParse(text.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var r) ||
            !byte.TryParse(text.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var g) ||
            !byte.TryParse(text.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var b))
        {
            return false;
        }
        color = (r, g, b);
        return true;
    }

    /// <summary>The relative luminance of a color (WCAG 2).</summary>
    public static double Luminance(string hex)
    {
        TryParse(hex, out var c);
        static double Channel(byte v)
        {
            var s = v / 255.0;
            return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
        }
        return 0.2126 * Channel(c.R) + 0.7152 * Channel(c.G) + 0.0722 * Channel(c.B);
    }

    /// <summary>The contrast ratio of two colors (WCAG 2), from 1 to 21.</summary>
    public static double Contrast(string a, string b)
    {
        var (x, y) = (Luminance(a), Luminance(b));
        return (Math.Max(x, y) + 0.05) / (Math.Min(x, y) + 0.05);
    }

    public static AccentTheme Compute(string hex)
    {
        var onFill = Contrast(hex, Ink) >= Contrast(hex, "#ffffff") ? Ink : "#ffffff";
        // An accent between the two (indigo) has less than 4.5:1 with either: the fill moves darker, a few steps, until white reads on it.
        if (Math.Max(Contrast(hex, Ink), Contrast(hex, "#ffffff")) < 4.5)
        {
            hex = Shift(hex, "#ffffff", 4.55, darker: true);
            onFill = "#ffffff";
        }
        return new AccentTheme(
            hex,
            onFill,
            Shift(hex, LightDarkestSurface, TextContrast, darker: true),
            Shift(hex, DarkLightestSurface, TextContrast, darker: false),
            Shift(hex, LightGround, DisplayContrast, darker: true),
            Shift(hex, DarkGround, DisplayContrast, darker: false));
    }

    // Moves the lightness of the color toward black or white until it has the contrast on the background; the color is left alone when it already has it.
    private static string Shift(string hex, string background, double target, bool darker)
    {
        if (Contrast(hex, background) >= target)
        {
            return hex;
        }
        var (h, s, l) = ToHsl(hex);
        var candidate = hex;
        for (var step = 1; step <= 100; step++)
        {
            var lightness = darker ? l - step * 0.01 : l + step * 0.01;
            if (lightness is < 0 or > 1)
            {
                break;
            }
            candidate = FromHsl(h, s, lightness);
            if (Contrast(candidate, background) >= target)
            {
                return candidate;
            }
        }
        return darker ? "#000000" : "#ffffff";
    }

    private static (double H, double S, double L) ToHsl(string hex)
    {
        TryParse(hex, out var c);
        double r = c.R / 255.0, g = c.G / 255.0, b = c.B / 255.0;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var l = (max + min) / 2;
        if (max == min)
        {
            return (0, 0, l);
        }
        var d = max - min;
        var s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
        var h = max == r ? (g - b) / d + (g < b ? 6 : 0) : max == g ? (b - r) / d + 2 : (r - g) / d + 4;
        return (h / 6, s, l);
    }

    private static string FromHsl(double h, double s, double l)
    {
        double Hue(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            return t < 1.0 / 6 ? p + (q - p) * 6 * t : t < 0.5 ? q : t < 2.0 / 3 ? p + (q - p) * (2.0 / 3 - t) * 6 : p;
        }
        double r, g, b;
        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;
            r = Hue(p, q, h + 1.0 / 3);
            g = Hue(p, q, h);
            b = Hue(p, q, h - 1.0 / 3);
        }
        static string Byte(double v) => ((int)Math.Round(Math.Clamp(v, 0, 1) * 255)).ToString("x2", CultureInfo.InvariantCulture);
        return "#" + Byte(r) + Byte(g) + Byte(b);
    }
}
