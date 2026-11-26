using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace KlyrooSaaSApp.Services;

/// <summary>
/// The render mode of the whole app: App.razor gives it to HeadOutlet and Routes, and the dashboard pages give it to DashboardPersistence so that the
/// data of the prerender reaches the interactive render. Change it here and in Program.cs, nowhere else.
/// </summary>
public static class AppRender
{
    //#if (Interactivity == "Server")
    /// <summary>Interactive Server: the pages run on the server, over a SignalR connection.</summary>
    public static IComponentRenderMode Mode { get; } = RenderMode.InteractiveServer;
    //#elseif (Interactivity == "WebAssembly")
    /// <summary>Interactive WebAssembly: the pages are prerendered on the server, then run in the browser.</summary>
    public static IComponentRenderMode Mode { get; } = RenderMode.InteractiveWebAssembly;
    //#else
    /// <summary>Interactive Auto: the pages run on the server at first, and in the browser once the WebAssembly files are downloaded.</summary>
    public static IComponentRenderMode Mode { get; } = RenderMode.InteractiveAuto;
    //#endif
}
