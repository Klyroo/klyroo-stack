using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

[assembly: AssemblyFixture(typeof(Klyroo.Stack.E2E.GeneratedApps))]

namespace Klyroo.Stack.E2E;

/// <summary>
/// The apps a user gets from <c>dotnet new klyroo-saas</c> and <c>dotnet new klyroo-admin</c>, as the tests of the typed forms need them: generated from
/// <c>Klyroo.Templates</c> and built against a local feed that holds the nine repositories packed (<c>build/pack-feed.sh &lt;feed&gt;</c>), then started with
/// <c>dotnet run</c> on a free port. Nothing is generated until a test asks for an app, and everything (the generated folders, the copy of the NuGet packages
/// of this run, the template hive) is deleted when the run ends.
/// <para>
/// <c>KLYROO_E2E_FEED</c> is the feed folder. Without it the tests of these apps are skipped on a developer machine and fail in CI, where a test that cannot
/// run proves nothing. The template pack is installed in a hive of its own (<c>--debug:custom-hive</c>), so the templates installed for the user are never touched,
/// and the packages restore into a folder of their own, so a Klyroo package of the same version left in the global cache never stands in for the one of the feed.
/// </para>
/// </summary>
public sealed class GeneratedApps : IAsyncLifetime
{
    private readonly object _gate = new();
    private readonly SemaphoreSlim _workspaceGate = new(1, 1);
    private readonly Dictionary<string, Lazy<Task<GeneratedApp>>> _apps = [];
    private string? _work;

    /// <summary>The folder with the packed repositories, or null when this run has none.</summary>
    public static string? Feed => Environment.GetEnvironmentVariable("KLYROO_E2E_FEED") is { Length: > 0 } feed ? Path.GetFullPath(feed) : null;

    /// <summary>Skips the test when there is no feed to generate from (it fails in CI).</summary>
    public static void RequireFeed()
    {
        if (Feed is not null) return;
        Assert.False(Environment.GetEnvironmentVariable("CI") is { Length: > 0 }, "KLYROO_E2E_FEED is not set: the generated apps need the packed repositories (build/pack-feed.sh).");
        Assert.SkipWhen(true, "KLYROO_E2E_FEED is not set: build/pack-feed.sh <folder> packs the nine repositories, then set KLYROO_E2E_FEED to that folder.");
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    /// <summary>The app generated from <paramref name="shortName"/> (<c>klyroo-saas</c>), built and running; one instance per template for the whole run.</summary>
    public Task<GeneratedApp> GetAsync(string shortName)
    {
        RequireFeed();
        Lazy<Task<GeneratedApp>> app;
        lock (_gate)
        {
            if (!_apps.TryGetValue(shortName, out app!))
            {
                app = new Lazy<Task<GeneratedApp>>(() => Task.Run(() => CreateAsync(shortName)));
                _apps[shortName] = app;
            }
        }
        return app.Value;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var app in _apps.Values)
        {
            if (!app.IsValueCreated) continue;
            try
            {
                await (await app.Value).DisposeAsync();
            }
            catch (Exception)
            {
                // The app never started: there is nothing to stop.
            }
        }
        if (_work is not null && Directory.Exists(_work))
        {
            try
            {
                Directory.Delete(_work, recursive: true);
            }
            catch (IOException)
            {
                // A process still holds a file: the folder is in the temporary folder of the machine.
            }
        }
    }

    private async Task<GeneratedApp> CreateAsync(string shortName)
    {
        var work = await EnsureWorkspaceAsync();
        var name = "E2e" + string.Concat(shortName.Split('-').Select(p => char.ToUpperInvariant(p[0]) + p[1..]));
        var folder = Path.Combine(work, "apps", name);
        var environment = ToolEnvironment(work);
        var hive = Path.Combine(work, "hive");

        await Run("dotnet", ["new", shortName, "-n", name, "-o", folder, "--debug:custom-hive", hive], work, environment);
        // Release, and not --no-restore: the restore of the generated app is part of what is checked, against the feed alone.
        await Run("dotnet", ["build", Path.Combine(folder, name + ".csproj"), "-c", "Release", "--nologo"], folder, environment);

        var port = FreePort();
        var url = $"http://127.0.0.1:{port}";
        var start = new ProcessStartInfo("dotnet", ["run", "--no-build", "-c", "Release", "--no-launch-profile", "--urls", url]) { WorkingDirectory = folder };
        foreach (var (key, value) in environment) start.Environment[key] = value;
        start.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        var app = GeneratedApp.Start(start, url);
        await app.WaitUntilReadyAsync();
        return app;
    }

    /// <summary>The temporary folder of this run: a nuget.config that takes every Klyroo.* package from the feed and the template pack installed in a hive.</summary>
    private async Task<string> EnsureWorkspaceAsync()
    {
        await _workspaceGate.WaitAsync();
        try
        {
            if (_work is not null) return _work;
            var feed = Feed!;
            var work = Path.Combine(Path.GetTempPath(), "klyroo-e2e-apps-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(work);
            File.WriteAllText(Path.Combine(work, "nuget.config"), $"""
                <?xml version="1.0" encoding="utf-8"?>
                <configuration>
                  <packageSources>
                    <clear />
                    <add key="klyroo-local" value="{feed}" />
                    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
                  </packageSources>
                  <packageSourceMapping>
                    <packageSource key="klyroo-local"><package pattern="Klyroo.*" /></packageSource>
                    <packageSource key="nuget.org"><package pattern="*" /></packageSource>
                  </packageSourceMapping>
                </configuration>
                """);
            var pack = Directory.EnumerateFiles(feed, "Klyroo.Templates.*.nupkg").FirstOrDefault()
                ?? throw new FileNotFoundException($"No Klyroo.Templates package in {feed}: run build/pack-feed.sh on the nine repositories.");
            _work = work;
            await Run("dotnet", ["new", "install", pack, "--debug:custom-hive", Path.Combine(work, "hive")], work, ToolEnvironment(work));
            return work;
        }
        finally
        {
            _workspaceGate.Release();
        }
    }

    private static Dictionary<string, string> ToolEnvironment(string work) => new()
    {
        ["NUGET_PACKAGES"] = Path.Combine(work, "packages"),
        ["DOTNET_NOLOGO"] = "1",
        ["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1",
        ["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1",
    };

    /// <summary>Runs a command to its end and returns its output; a failure stops the run with everything the command printed.</summary>
    private static async Task Run(string fileName, string[] arguments, string workingDirectory, Dictionary<string, string> environment)
    {
        var start = new ProcessStartInfo(fileName, arguments) { WorkingDirectory = workingDirectory, RedirectStandardOutput = true, RedirectStandardError = true };
        foreach (var (key, value) in environment) start.Environment[key] = value;
        using var process = Process.Start(start) ?? throw new InvalidOperationException($"Could not start {fileName}.");
        var output = new List<string>();
        process.OutputDataReceived += (_, e) => { if (e.Data is not null) lock (output) output.Add(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) lock (output) output.Add(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        await process.WaitForExitAsync(timeout.Token);
        if (process.ExitCode != 0)
        {
            string text;
            lock (output) text = string.Join("\n", output);
            throw new InvalidOperationException($"{fileName} {string.Join(' ', arguments)} exited with {process.ExitCode}:\n{text}");
        }
    }

    internal static int FreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
}

/// <summary>A generated app that is running: its address, and what it printed, for the message of a test that fails.</summary>
public sealed class GeneratedApp : IAsyncDisposable
{
    private readonly Process _process;
    private readonly List<string> _log = [];

    private GeneratedApp(Process process, string baseUrl)
    {
        _process = process;
        BaseUrl = baseUrl;
    }

    /// <summary>The address of the app, with a trailing slash.</summary>
    public string BaseUrl { get; }

    internal static GeneratedApp Start(ProcessStartInfo start, string url)
    {
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        var process = Process.Start(start) ?? throw new InvalidOperationException("Could not start the generated app.");
        var app = new GeneratedApp(process, url + "/");
        process.OutputDataReceived += (_, e) => { if (e.Data is not null) lock (app._log) app._log.Add(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data is not null) lock (app._log) app._log.Add(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return app;
    }

    /// <summary>What the app printed so far.</summary>
    public string Log
    {
        get
        {
            lock (_log) return string.Join("\n", _log);
        }
    }

    internal async Task WaitUntilReadyAsync()
    {
        using var http = new HttpClient();
        var deadline = DateTime.UtcNow.AddSeconds(120);
        while (DateTime.UtcNow < deadline)
        {
            if (_process.HasExited) throw new InvalidOperationException($"The generated app exited with code {_process.ExitCode}:\n{Log}");
            try
            {
                if ((await http.GetAsync(BaseUrl)).StatusCode == HttpStatusCode.OK) return;
            }
            catch (HttpRequestException)
            {
                // Not listening yet.
            }
            await Task.Delay(250);
        }
        throw new TimeoutException($"The generated app did not answer at {BaseUrl} within 120 seconds:\n{Log}");
    }

    public async ValueTask DisposeAsync()
    {
        if (!_process.HasExited)
        {
            _process.Kill(entireProcessTree: true);
            await _process.WaitForExitAsync();
        }
        _process.Dispose();
    }
}
