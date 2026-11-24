using System.Text.Json;
using Lux.Stack.BundleGenerator;
using Lux.Stack.Mcp.Markdown;
using Lux.Stack.Mcp.Model;

// dotnet run --project build/Lux.Stack.DocsBundle -- [--repo <dir>] [--out <docs-bundle.json>] [--skill-reference]
//
//   --out              writes the docs bundle (the build of src/Lux.Stack.Mcp.Core does this and embeds the result).
//   --skill-reference  rewrites skills/lux-ui/references/components.md, the generated part of the agent skill.
var repo = Arg("--repo") ?? FindRepoRoot();
var output = Arg("--out");
var skill = args.Contains("--skill-reference");
if (output is null && !skill)
{
    Console.Error.WriteLine("Nothing to do: pass --out <file> and/or --skill-reference.");
    return 2;
}

// The sister packages whose themes the bundle lists next to those of Lux.UI.
var bundle = BundleBuilder.Build(repo, [typeof(Lux.Table.LuxTableOptions)]);
if (output is not null)
{
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
    File.WriteAllText(output, JsonSerializer.Serialize(bundle, DocsBundle.JsonOptions));
    Console.WriteLine($"docs bundle: {bundle.Components.Count} components, {bundle.Guides.Count} guides, {bundle.Icons.Count} icons -> {output}");
}
if (skill)
{
    var uiRoot = Path.GetFullPath(Path.Combine(repo, "..", "lux-ui"));
    var path = Path.Combine(Directory.Exists(uiRoot) ? uiRoot : repo, SkillReference.RelativePath);
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    File.WriteAllText(path, SkillReference.Components(bundle));
    Console.WriteLine($"skill reference -> {path}");
}
return 0;

string? Arg(string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static string FindRepoRoot()
{
    for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir is not null; dir = dir.Parent)
    {
        if (File.Exists(Path.Combine(dir.FullName, "Lux.Stack.slnx"))) return dir.FullName;
    }
    throw new DirectoryNotFoundException("Lux.Stack.slnx not found above the current directory; pass --repo.");
}
