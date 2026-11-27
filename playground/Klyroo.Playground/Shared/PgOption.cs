namespace Klyroo.Playground.Shared;

/// <summary>One choice of a select knob: the value, its label and the text it has in the address.</summary>
public sealed record PgOption<T>(T Value, string Label, string Token);

public static class PgOptions
{
    public static IReadOnlyList<PgOption<TEnum>> Of<TEnum>() where TEnum : struct, Enum =>
        [.. Enum.GetValues<TEnum>().Select(v => new PgOption<TEnum>(v, v.ToString(), v.ToString()))];

    public static IReadOnlyList<PgOption<string>> Of(params string[] values) =>
        [.. values.Select(v => new PgOption<string>(v, v, v))];

    public static IReadOnlyList<PgOption<string>> Of(params (string Value, string Label)[] values) =>
        [.. values.Select(v => new PgOption<string>(v.Value, v.Label, v.Value))];
}
