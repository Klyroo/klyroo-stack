using Klyroo.UI;
using Klyroo.UI.Images;
using Klyroo.Stack;

namespace Klyroo.Stack.Site.Client;

/// <summary>
/// Klyroo configuration shared by the server and the WebAssembly client, so components look the same
/// whichever runtime renders them.
/// </summary>
public static class PlaygroundServices
{
    public static IServiceCollection AddKlyrooSite(this IServiceCollection services)
    {
        services.AddKlyrooStack(configureUi: o =>
        {
            o.Images.AddLocal(ImageEndpoint, makeDefault: true);
        });
        // A test hook of the data list examples: how many times the provider of a render was called.
        services.AddScoped<Examples.ProviderCallLog>();
        return services;
    }

    /// <summary>Where the server maps the image optimizer.</summary>
    public const string ImageEndpoint = KlyLocalImageProvider.DefaultEndpoint;

    /// <summary>Maps the first URL segment to a render mode; anything else renders statically.</summary>
    public static string? ModeFromPath(string path) =>
        path.TrimStart('/').Split('/', 2)[0] switch
        {
            "server" => "server",
            "wasm" => "wasm",
            "auto" => "auto",
            "ssr" => "ssr",
            _ => null,
        };

    public static readonly string[] Modes = ["ssr", "server", "wasm", "auto"];
}
