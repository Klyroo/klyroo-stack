using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Tests;

/// <summary>A library repository on disk, as the site reads it: README.md, docs/*.md, docs/site.yml and the files the docs link to.</summary>
internal sealed class FakeRepository : IDisposable
{
    public FakeRepository(string name = "klyroo-demo")
    {
        Root = Path.Combine(Path.GetTempPath(), "klyroo-docs-tests-" + Guid.NewGuid().ToString("N"), name);
        Directory.CreateDirectory(Path.Combine(Root, "docs"));
    }

    public string Root { get; }

    public static Libraries Libraries { get; } = new()
    {
        Organization = "klyroo",
        PagesUrl = "https://klyroo.github.io",
        Items = [new("klyroo-demo", "Klyroo.Demo"), new("klyroo-other", "Klyroo.Other")],
    };

    public FakeRepository Write(string path, string text)
    {
        var full = Path.Combine(Root, path.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        File.WriteAllText(full, text);
        return this;
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(Path.GetDirectoryName(Root)!, recursive: true);
        }
        catch (IOException)
        {
        }
    }
}
