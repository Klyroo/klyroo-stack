using System.Text.Json;

namespace Klyroo.Playground.Shared;

public static class JsonText
{
    /// <summary>Indents a JSON text. Returns the text as it is when it is not JSON.</summary>
    public static string Indent(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                doc.WriteTo(writer);
            }
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }
        catch (JsonException)
        {
            return json;
        }
    }
}
