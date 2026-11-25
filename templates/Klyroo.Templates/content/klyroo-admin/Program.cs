using Klyroo.Stack;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddKlyrooStack();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<KlyrooAdminApp.Components.App>().AddInteractiveServerRenderMode();
app.Run();
