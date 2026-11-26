using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Net.Http.Headers;

namespace Klyroo.Docs.Site.Content;

/// <summary>
/// The search index of the site as a JSON file, for <c>KlyContentSearch IndexUrl</c>: the browser fetches it the first time the search opens, so the
/// pages of the site do not each carry a copy. It is built once and sent with an <c>ETag</c>.
/// </summary>
public static class DocsSearchIndex
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
    private static readonly ConditionalWeakTable<DocsSite, Entry> Entries = new();

    private sealed record Entry(byte[] Body, string ETag);

    /// <summary>The JSON of the index: an array of pages, each with <c>title</c>, <c>href</c> and, when they have one, <c>description</c>, <c>group</c>, <c>keywords</c> and <c>headings</c>.</summary>
    public static byte[] Serialize(DocsSite site) => JsonSerializer.SerializeToUtf8Bytes(site.Search, Json);

    /// <summary>Serves the index at <paramref name="pattern"/>, below the path base like every other address.</summary>
    public static IEndpointConventionBuilder MapSearchIndex(this IEndpointRouteBuilder endpoints, string pattern = "/search-index.json") =>
        endpoints.MapMethods(pattern, ["GET", "HEAD"], (HttpContext context, DocsSite site) =>
        {
            var entry = Entries.GetValue(site, s =>
            {
                var body = Serialize(s);
                return new Entry(body, $"\"{Convert.ToHexString(SHA256.HashData(body))[..16]}\"");
            });
            var headers = context.Response.GetTypedHeaders();
            headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            headers.ETag = new EntityTagHeaderValue(entry.ETag);
            if (context.Request.Headers.IfNoneMatch.Contains(entry.ETag))
            {
                return Results.StatusCode(StatusCodes.Status304NotModified);
            }
            return Results.Bytes(entry.Body, "application/json");
        });
}
