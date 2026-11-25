using System.Text.Json;
using Klyroo.Stack.BundleGenerator;
using Klyroo.Stack.Mcp.Markdown;
using Klyroo.Stack.Mcp.Model;

// dotnet run --project build/Klyroo.Stack.DocsBundle -- [--repo <dir>] [--out <docs-bundle.json>] [--skill-reference]
//
//   --out              writes the docs bundle (the build of src/Klyroo.Stack.Mcp.Core does this and embeds the result).
//   --skill-reference  rewrites skills/klyroo-ui/references/components.md, the generated part of the agent skill.
var repo = Arg("--repo") ?? FindRepoRoot();
var output = Arg("--out");
var skill = args.Contains("--skill-reference");
if (output is null && !skill)
{
    Console.Error.WriteLine("Nothing to do: pass --out <file> and/or --skill-reference.");
    return 2;
}

// The sister packages whose themes the bundle lists next to those of Klyroo.UI.
var bundle = BundleBuilder.Build(repo, [typeof(Klyroo.Table.KlyrooTableOptions)]);
if (output is not null)
{
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
    File.WriteAllText(output, JsonSerializer.Serialize(bundle, DocsBundle.JsonOptions));
    Console.WriteLine($"docs bundle: {bundle.Components.Count} components, {bundle.Guides.Count} guides, {bundle.Icons.Count} icons -> {output}");
}
if (skill)
{
    var uiRoot = Path.GetFullPath(Path.Combine(repo, "..", "klyroo-ui"));
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
        if (File.Exists(Path.Combine(dir.FullName, "Klyroo.Stack.slnx"))) return dir.FullName;
    }
    throw new DirectoryNotFoundException("Klyroo.Stack.slnx not found above the current directory; pass --repo.");
}
