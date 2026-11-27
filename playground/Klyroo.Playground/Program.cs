using Klyroo.Playground;
using Klyroo.Playground.Shell;
using Klyroo.Stack;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// The sample files are static files of the app, read with the base address of the page.
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddKlyrooStack();
builder.Services.AddScoped<Knobs>();

await builder.Build().RunAsync();
