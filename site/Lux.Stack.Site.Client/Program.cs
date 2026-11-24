using Lux.Stack.Site.Client;
using Lux.Stack.Site.Client.Templates;
using Lux.UI;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddLuxSite();
builder.Services.AddSingleton(new TemplateSettings());
builder.Services.AddSingleton<ITemplateDemoState, ClientTemplateDemoState>();

var host = builder.Build();
// The client has no request: take the culture the server rendered with (meta element, cookie, lang attribute), so both pick the same locale.
await host.Services.ApplyServerCultureAsync(LxLocales.All.Select(locale => locale.Code).Concat(["en-GB", "fr-CA"]));
await host.RunAsync();

file sealed class ClientTemplateDemoState : ITemplateDemoState
{
    public string? StateOf(string name) => "ready";
}
