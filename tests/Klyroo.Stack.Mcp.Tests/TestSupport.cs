using Klyroo.Stack.Mcp.Model;

namespace Klyroo.Stack.Mcp.Tests;

internal static class TestSupport
{
    /// <summary>The bundle the build embedded: what the tool and the docs site serve.</summary>
    public static DocsBundle Bundle => DocsBundleLoader.Embedded;

    public static DocsService Service { get; } = new(DocsBundleLoader.Embedded);

    public static DocsTools Tools { get; } = new(Service);

    public static string RepoRoot { get; } = FindRepoRoot();

    public static string UiRepoRoot { get; } = Path.GetFullPath(Path.Combine(RepoRoot, "..", "klyroo-ui"));

    private static string FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Klyroo.Stack.slnx"))) return dir.FullName;
        }
        throw new DirectoryNotFoundException("Klyroo.Stack.slnx not found above the test output directory.");
    }

    /// <summary>The built stdio tool, next to the configuration of this test assembly.</summary>
    public static string ToolAssembly()
    {
        var configuration = new DirectoryInfo(Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory)).Parent!.Name; // bin/<Configuration>/net10.0
        var path = Path.Combine(RepoRoot, "src", "Klyroo.Stack.Mcp", "bin", configuration, "net10.0", "Klyroo.Stack.Mcp.dll");
        Assert.True(File.Exists(path), $"The tool is not built: {path}");
        return path;
    }
}
