using Microsoft.Extensions.DependencyInjection;

namespace KlyrooAdminApp.Services;

/// <summary>Registers the services of the app. The server and, when there is one, the WebAssembly client call it, so both register the same services.</summary>
public static class ServiceRegistration
{
    public static IServiceCollection AddAppServices(this IServiceCollection services) =>
        // One store per circuit, or per browser tab in WebAssembly: the sample data starts again with every visit.
        services.AddScoped<CustomerStore>();
}
