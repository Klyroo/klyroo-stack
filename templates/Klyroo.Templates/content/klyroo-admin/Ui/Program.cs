using Klyroo.Stack;
using Klyroo.UI;
using KlyrooAdminApp.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// The same two calls as the server Program.cs: an app that renders on the server and then in the browser must not change when the runtime switches.
builder.Services.AddKlyrooStack();
builder.Services.AddAppServices();

var host = builder.Build();
// The client has no request: it takes the culture the server rendered with, so both pick the same locale.
await host.Services.ApplyServerCultureAsync(KlyLocales.All.Select(locale => locale.Code));
await host.RunAsync();
