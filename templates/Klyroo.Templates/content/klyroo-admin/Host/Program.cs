using Klyroo.Stack;
using Klyroo.UI;
using Klyroo.UI.Server;
using KlyrooAdminApp.Components;
using KlyrooAdminApp.Services;

var builder = WebApplication.CreateBuilder(args);

// The libraries of the stack, and the sample data of the app. In an app with a WebAssembly client, the client Program.cs makes the same two calls.
builder.Services.AddKlyrooStack();
builder.Services.AddAppServices();
// The cookies of the request, for the components that render before the browser is there. Server only: the WebAssembly client has no request.
builder.Services.AddKlyrooServer();

builder.Services.AddRazorComponents()
//#if (Interactivity == "Server")
    .AddInteractiveServerComponents();
//#elseif (Interactivity == "WebAssembly")
    .AddInteractiveWebAssemblyComponents();
//#else
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
//#endif

var app = builder.Build();

//#if (UseClient)
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//#else
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//#endif
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Reads the query string, the culture cookie that KlyLocaleSelect writes and Accept-Language. Klyroo components follow the UI culture on their own.
var cultures = KlyLocales.All.Select(locale => locale.Code).ToArray();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(cultures)
    .AddSupportedUICultures(cultures));
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
//#if (Interactivity == "Server")
    .AddInteractiveServerRenderMode();
//#elseif (Interactivity == "WebAssembly")
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(KlyrooAdminApp.Client._Imports).Assembly);
//#else
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(KlyrooAdminApp.Client._Imports).Assembly);
//#endif

app.Run();
