using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Klyroo.Playground.Shell;

/// <summary>
/// The knobs of a playground, kept in the address (<c>?k.group=1</c>) so a reload or a shared link restores them. A knob that equals its
/// default is left out of the address. The address is rewritten without a history entry and keeps every other key (the data list writes its own).
/// </summary>
public sealed class Knobs(NavigationManager navigation)
{
    public const string Prefix = "k.";

    private readonly Dictionary<string, string> _values = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _defaults = new(StringComparer.Ordinal);

    /// <summary>Reads the knobs of the current address and forgets the defaults of the previous page.</summary>
    public void Load()
    {
        _values.Clear();
        _defaults.Clear();
        var query = new Uri(navigation.Uri).Query.TrimStart('?');
        foreach (var part in query.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var at = part.IndexOf('=');
            var key = Uri.UnescapeDataString((at < 0 ? part : part[..at]).Replace('+', ' '));
            if (!key.StartsWith(Prefix, StringComparison.Ordinal)) continue;
            var value = at < 0 ? "" : Uri.UnescapeDataString(part[(at + 1)..].Replace('+', ' '));
            _values[key[Prefix.Length..]] = value;
        }
    }

    public string Str(string key, string def)
    {
        _defaults[key] = def;
        return _values.TryGetValue(key, out var v) ? v : def;
    }

    public bool Bool(string key, bool def)
    {
        _defaults[key] = def ? "1" : "0";
        return _values.TryGetValue(key, out var v) ? v == "1" : def;
    }

    public int Int(string key, int def, int min, int max)
    {
        _defaults[key] = def.ToString(CultureInfo.InvariantCulture);
        return _values.TryGetValue(key, out var v) && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? Math.Clamp(n, min, max) : def;
    }

    public TEnum Enum<TEnum>(string key, TEnum def) where TEnum : struct, Enum
    {
        _defaults[key] = def.ToString();
        return _values.TryGetValue(key, out var v) && System.Enum.TryParse<TEnum>(v, true, out var e) && System.Enum.IsDefined(e) ? e : def;
    }

    /// <summary>Stores a knob and rewrites the address. The text is the form the knob has in the address.</summary>
    public void Put(string key, string value)
    {
        var isDefault = _defaults.TryGetValue(key, out var def) && def == value;
        if (isDefault) _values.Remove(key); else _values[key] = value;
        Push(new Dictionary<string, object?> { [Prefix + key] = isDefault ? null : value });
    }

    /// <summary>Drops every knob from the address.</summary>
    public void Reset()
    {
        var clear = _values.Keys.ToDictionary(k => Prefix + k, _ => (object?)null);
        _values.Clear();
        if (clear.Count > 0) Push(clear);
    }

    private void Push(Dictionary<string, object?> change) =>
        navigation.NavigateTo(navigation.GetUriWithQueryParameters(change), new NavigationOptions { ReplaceHistoryEntry = true });
}
