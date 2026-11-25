using System.IO.Compression;
using Klyroo.Stack.Site;
using Klyroo.Stack.Site.Client;
using Klyroo.UI;
using Klyroo.UI.Server;
using Klyroo.Stack.Site.Components;
using Klyroo.Stack.Site.Templates;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);
// Allow running Release builds in Production environment directly from the repo checkout without missing RCL/font static web assets.
builder.WebHost.UseStaticWebAssets();

// Compress HTML, CSS, JS, JSON, SVG, and WASM payloads (Brotli + Gzip) for low-latency mobile loads.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
        "image/svg+xml",
        "application/wasm",
        "application/json",
        "text/plain"
    ]);
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Optimal);

// Add services to the container.
builder.Services.AddKlyrooSite();
// The server reads the time zone cookie in static renders and prerenders (IKlyRequestCookies), so the data list cuts days like the interactive render.
builder.Services.AddKlyrooServer();
builder.Services.AddKlyrooAi(builder.Configuration);
// The live demos of the templates: child processes behind /templates/{name}/demo/ (Templates/TemplateDemos.cs).
builder.Services.AddTemplateDemos(builder.Configuration);
// Resizes and converts the images of wwwroot on demand, so the docs show real srcset candidates. No remote host is allowed.
builder.Services.AddKlyrooImagesLocal(o => o.Endpoint = PlaygroundServices.ImageEndpoint);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
// PathBase serves the whole site below a prefix ("/sub"), as a reverse proxy or the live demos of the templates do: the E2E suite runs a second
// instance this way to check that the library never assumes it lives at "/". UseRouting has to come right after UsePathBase, because
// WebApplication would otherwise add it first and the prefix would never be stripped before routing. App.razor writes the matching <base href>.
if (builder.Configuration["PathBase"] is { Length: > 0 } pathBase)
{
    app.UsePathBase("/" + pathBase.Trim('/'));
    app.UseRouting();
}
app.UseResponseCompression();
app.Use(async (context, next) =>
{
    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});
app.UseKlyrooAi();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// Not for /templates/{name}/demo/: a demo answers its own 404s.
app.UseTemplateDemoResponsesAsIs();
// Reads the query string, the culture cookie that KlyLocaleSelect writes and Accept-Language (the default provider list).
// Klyroo components follow the UI culture on their own: the supported UI cultures are the codes of the built-in locales.
// The supported cultures add two regional ones, so that dates and clocks keep the format of an en-GB or fr-CA user.
var uiCultures = KlyLocales.All.Select(locale => locale.Code).ToArray();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures([.. uiCultures, "en-GB", "fr-CA"])
    .AddSupportedUICultures(uiCultures));
app.UseAntiforgery();

app.MapGet("/favicon.ico", () => Results.File(Path.Combine(app.Environment.WebRootPath, "favicon.svg"), "image/svg+xml"));
app.MapStaticAssets();
app.MapKlyrooAi();
app.MapKlyrooImages();
app.MapTemplateDemos();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Klyroo.Stack.Site.Client._Imports).Assembly);

app.Run();
