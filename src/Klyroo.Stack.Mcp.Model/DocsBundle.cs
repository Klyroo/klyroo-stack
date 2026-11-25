using System.Text.Json;

namespace Klyroo.Stack.Mcp.Model;

/// <summary>
/// Everything the AI tooling knows about Klyroo.UI: the docs catalog, the API of every component read from the library,
/// the source of every example and the text of the guides. Generated at build time by <c>build/Klyroo.Stack.DocsBundle</c>
/// and embedded in this assembly, so the MCP server, <c>llms.txt</c> and the skill reference all read one source.
/// </summary>
public sealed record DocsBundle(
    string Name,
    string Version,
    IReadOnlyList<string> Groups,
    IReadOnlyList<BundleComponent> Components,
    IReadOnlyList<BundleGuide> Guides,
    IReadOnlyList<string> Icons)
{
    public static JsonSerializerOptions JsonOptions { get; } = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };
}

/// <summary>
/// A documented component: one page of the docs site, at <c>/components/{Slug}</c>. <see cref="Package"/> is the NuGet package to add when
/// it is not Klyroo.UI itself (<c>Klyroo.Table</c>); null for the components of Klyroo.UI.
/// </summary>
public sealed record BundleComponent(
    string Slug,
    string Name,
    string Group,
    string Description,
    IReadOnlyList<BundleExample> Examples,
    IReadOnlyList<BundleApiType> Api,
    string? Package = null)
{
    /// <summary>The command that adds the package of a component that is not in Klyroo.UI (<c>dotnet add package Klyroo.Table --prerelease</c>), null otherwise.</summary>
    public string? InstallCommand() => Package is null ? null : $"dotnet add package {Package} --prerelease";

    /// <summary>
    /// The registration method of the package of a component that is not in Klyroo.UI: <c>AddKlyrooTable</c> for <c>Klyroo.Table</c>.
    /// Its options object is the one that configures the theme of the component, null for a component of Klyroo.UI.
    /// </summary>
    public string? RegistrationMethod() =>
        Package is null ? null
        : Package.StartsWith("Klyroo.UI.", StringComparison.Ordinal) ? "AddKlyrooUi" + Package["Klyroo.UI.".Length..].Replace(".", "")
        : Package.StartsWith("Klyroo.", StringComparison.Ordinal) ? "Add" + Package.Replace(".", "")
        : null;
}

/// <summary>A live example of the docs site. <see cref="Id"/> is the name of its Razor component.</summary>
public sealed record BundleExample(string Id, string Title, string? Description, string Source);

/// <summary>One component class of a page and everything it accepts.</summary>
public sealed record BundleApiType(
    string Name,
    string? Summary,
    IReadOnlyList<BundleParameter> Parameters,
    IReadOnlyList<BundleParameter> Events,
    IReadOnlyList<BundleParameter> Slots,
    BundleUi? Ui,
    BundleTheme? Theme);

/// <summary>A <c>[Parameter]</c>. Events are <c>EventCallback</c> parameters and slots are <c>RenderFragment</c> ones.</summary>
public sealed record BundleParameter(
    string Name,
    string Type,
    string? Default,
    string? Description,
    bool Required,
    bool Bindable,
    IReadOnlyList<string>? Values);

/// <summary>The per-instance <c>Ui</c> record of a component: one property per slot.</summary>
public sealed record BundleUi(string Type, IReadOnlyList<BundleUiSlot> Slots);

public sealed record BundleUiSlot(string Name, string? Description);

/// <summary>
/// The global theme of a component: classes per slot, per variant value, and the defaults. <see cref="Property"/> is the
/// property of <c>KlyrooOptions</c> that configures it (<c>AddKlyrooUi(o => o.Button.Slot(...))</c>).
/// </summary>
public sealed record BundleTheme(
    string Name,
    string Property,
    IReadOnlyDictionary<string, string> Slots,
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>> Variants,
    IReadOnlyDictionary<string, string> Defaults,
    IReadOnlyList<BundleCompound> Compounds);

public sealed record BundleCompound(IReadOnlyDictionary<string, string> When, IReadOnlyDictionary<string, string> Classes);

/// <summary>A guide page of the docs site, converted to Markdown.</summary>
public sealed record BundleGuide(string Path, string Title, string Section, string Markdown);
