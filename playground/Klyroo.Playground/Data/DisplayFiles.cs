using Klyroo.Display;
using Klyroo.UI;

namespace Klyroo.Playground.Data;

/// <summary>The files of the file list of the Display playground: text the preview renders from <c>Content</c>, an image drawn as a data URI, and the audio and video of the samples folder.</summary>
public static class DisplayFiles
{
    private const string Notes = "# Release notes\n\nVersion **0.2** highlights code in thirteen languages.\n\n- Line numbers\n- Emphasized lines\n- A masked secret block\n";
    private const string Metrics = "Region,Revenue,Orders\nEurope,148900,1204\nUS,192400,1788\nAsia,98750,901\n";
    private const string Program = "var builder = WebApplication.CreateBuilder(args);\nbuilder.Services.AddKlyrooStack();\n\nvar app = builder.Build();\napp.Run();\n";
    private const string Query = "SELECT id, total\nFROM orders\nWHERE status = 'paid'\n  AND total > 100;\n";

    // A small drawing: it needs no file in the app, and the image preview zooms and rotates it like a photo.
    private const string Logo = "data:image/svg+xml;utf8,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 320 200'%3E%3Crect width='320' height='200' fill='%23ec4899'/%3E%3Ccircle cx='160' cy='100' r='56' fill='white'/%3E%3C/svg%3E";

    public static IReadOnlyList<FileListItem> Sample { get; } =
    [
        new(new FilePreviewFile { Src = "files/notes.md", Name = "notes.md", Mime = "text/markdown", Size = 2_480, Content = Notes },
            Description: "Release notes", Icon: "note", Tone: KlyColor.Info),
        new(new FilePreviewFile { Src = "files/metrics.csv", Name = "metrics.csv", Mime = "text/csv", Size = 18_400, Content = Metrics },
            Meta: "Updated today"),
        new(new FilePreviewFile { Src = "files/Program.cs", Name = "Program.cs", Size = 1_024, Content = Program },
            Description: "Colored from its extension", Tone: KlyColor.Success),
        new(new FilePreviewFile { Src = "files/orders.sql", Name = "orders.sql", Size = 512, Content = Query },
            Description: "A query"),
        new(new FilePreviewFile { Src = Logo, Name = "logo.svg", Mime = "image/svg+xml", Size = 3_200 },
            Description: "Zoom and rotate", Icon: "image", Tone: KlyColor.Secondary),
        new(new FilePreviewFile { Src = "samples/sample-audio.ogg", Name = "listening.ogg", Mime = "audio/ogg", Size = 85_400 },
            Meta: "20 s"),
        new(new FilePreviewFile { Src = "samples/sample-video.webm", Name = "briefing.webm", Mime = "video/webm", Size = 91_900 },
            Meta: "8 s", Tone: KlyColor.Warning),
    ];
}
