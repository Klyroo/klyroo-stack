using System.Text.Json.Serialization;

namespace Klyroo.Playground.Data;

public enum ProductCategory { Hardware, Software, Service }

/// <summary>The typed row the spreadsheet playground turns every importable line of a file into.</summary>
public sealed class ProductRow
{
    public string? Sku { get; set; }
    public string? Name { get; set; }
    public ProductCategory Category { get; set; }
    public decimal Price { get; set; }
    public int? Stock { get; set; }
    public DateOnly? Launch { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = true, UseStringEnumConverter = true)]
[JsonSerializable(typeof(List<ProductRow>))]
public sealed partial class ProductJson : JsonSerializerContext;
