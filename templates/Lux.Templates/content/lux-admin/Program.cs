using Lux.Stack;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLuxStack();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<LuxAdminApp.Components.App>().AddInteractiveServerRenderMode();
app.Run();
