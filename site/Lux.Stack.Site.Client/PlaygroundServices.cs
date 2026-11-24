using Lux.UI;
using Lux.UI.Images;
using Lux.Stack;

namespace Lux.Stack.Site.Client;

/// <summary>
/// LuxStack configuration shared by the server and the WebAssembly client, so components look the same
/// whichever runtime renders them.
/// </summary>
public static class PlaygroundServices
{
    public static IServiceCollection AddLuxSite(this IServiceCollection services)
    {
        services.AddLuxStack(configureUi: o =>
        {
            o.Images.AddLocal(ImageEndpoint, makeDefault: true);
        });
        // A test hook of the data list examples: how many times the provider of a render was called.
        services.AddScoped<Examples.ProviderCallLog>();
        return services;
    }

    /// <summary>Where the server maps the image optimizer.</summary>
    public const string ImageEndpoint = LxLocalImageProvider.DefaultEndpoint;

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
