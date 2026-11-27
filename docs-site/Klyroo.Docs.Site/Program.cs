using Klyroo.Docs.Site;
using Klyroo.Docs.Site.Components;
using Klyroo.Docs.Site.Content;
using Klyroo.Docs.Site.Export;
using Klyroo.Docs.Site.Library;
using Klyroo.Display;
using Klyroo.UI;

// Two ways to run: serve the site of one library, or write it as static files.
//
//   dotnet run -- --repo ../klyroo-table [--name klyroo-table] [--PathBase /klyroo-table]
//   dotnet run -- export --repo ../klyroo-table --out ./_site [--name klyroo-table]
//
// --repo is the folder of the library repository (README.md, docs/ and docs/site.yml). --name is the name of the repository on GitHub, which is the address of the site
// (https://klyroo.github.io/<name>/); it defaults to the site.yml "repo" setting, then to the name of the folder.
// `dotnet run --project` starts the app in the folder of the project. A relative --repo or --out means the folder the command was typed in, which a POSIX shell
// keeps in PWD; without it (Windows) the paths are relative to the project, so give absolute ones there.
static string FromShell(string path) =>
    Path.IsPathRooted(path) ? path : Path.GetFullPath(path, Environment.GetEnvironmentVariable("PWD") is { Length: > 0 } pwd && Directory.Exists(pwd) ? pwd : Directory.GetCurrentDirectory());

const string Usage = "Usage: dotnet run --project docs-site/Klyroo.Docs.Site -- [export --out <folder>] --repo <library repository> [--name <repository name>]";

var export = args.Length > 0 && args[0] == "export";
var appArgs = export ? args[1..] : args;
// The settings of this tool are read from the command line only: an environment variable called NAME or OUT (NAME is the machine name on some systems) must not be taken for them.
var cli = new ConfigurationBuilder().AddCommandLine(appArgs).Build();
var builder = WebApplication.CreateBuilder(appArgs);

if (cli["repo"] is not { Length: > 0 } repoArgument)
{
    Console.Error.WriteLine(Usage);
    return 2;
}
var repoPath = FromShell(repoArgument);
var outFolder = cli["out"] is { Length: > 0 } outArgument ? FromShell(outArgument) : null;
if (export && outFolder is null)
{
    Console.Error.WriteLine(Usage);
    return 2;
}

Libraries libraries;
DocsSite site;
try
{
    libraries = Libraries.Load(Path.Combine(AppContext.BaseDirectory, "libraries.yml"));
    site = LibraryDocs.Load(repoPath, cli["name"], libraries);
}
catch (ContentException error)
{
    Console.Error.WriteLine(error.Message);
    return 1;
}

// Allows running from the repository, without a publish, in any environment: the static web assets of Klyroo.UI are served from where they are built.
builder.WebHost.UseStaticWebAssets();
if (export)
{
    // The site is served in this process, on a free port, below the path it will have on GitHub Pages: <base href> and every address follow it.
    builder.WebHost.UseUrls("http://127.0.0.1:0");
    builder.Configuration["PathBase"] = site.Settings.RepoName;
    builder.Logging.SetMinimumLevel(LogLevel.Warning);
}

// The accent of the site is written by the page (Components/App.razor) as CSS variables, so the Klyroo.UI colors need no change here.
// AddKlyrooDisplay registers Klyroo.UI too: the showcase home page draws its code blocks with KlyDisplay.
builder.Services.AddKlyrooDisplay();
// Static server rendering only: no circuit, no WebAssembly. Everything that moves runs in the browser (Klyroo's own script).
builder.Services.AddRazorComponents();
builder.Services.AddSingleton(site);
builder.Services.AddSingleton(libraries);

WebApplication app;
try
{
    app = builder.Build();
}
catch (ContentException error)
{
    Console.Error.WriteLine(error.Message);
    return 1;
}

foreach (var problem in site.Problems)
{
    app.Logger.LogWarning("{Problem}", problem);
}

// PathBase serves the whole site below a prefix ("/klyroo-table"), as GitHub Pages does. UseRouting has to come right after UsePathBase, because WebApplication
// would otherwise add it first and the prefix would never be stripped before routing. App.razor writes the matching <base href>.
if (builder.Configuration["PathBase"] is { Length: > 0 } pathBase)
{
    app.UsePathBase("/" + pathBase.Trim('/'));
    app.UseRouting();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapSearchIndex();
app.MapRazorComponents<App>();

if (!export)
{
    app.Run();
    return 0;
}

await app.StartAsync();
try
{
    var address = app.Urls.First();
    using var http = new HttpClient { BaseAddress = new Uri(address) };
    var exporter = new StaticExporter(http, "/" + site.Settings.RepoName.Trim('/') + "/", outFolder!, site);
    return await exporter.RunAsync(Console.Out) ? 0 : 1;
}
finally
{
    await app.StopAsync();
}
