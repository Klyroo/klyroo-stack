using Klyroo.Display;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class KlyrooLibraryDocs
{
    private static LibraryDocTree BuildDisplay() => new(
        "display", "Klyroo.Display", "#ec4899",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "Highlighted code with line numbers and emphasized lines (KlyCodeBlock), a masked secret (KlySecretBlock), a file list that opens the preview (KlyFileList), constrained Audio/Video media players with clip bounds and play quotas (KlyAudioPlayer, KlyVideoPlayer), and a multi-format modal file previewer (FilePreviewService & KlyFilePreviewProvider).",
                [
                    new("overview", "What is Klyroo.Display?",
                        Paragraphs:
                        [
                            "Klyroo.Display gives your application the rich content that Klyroo.UI does not draw: syntax-highlighted code, masked secrets, file lists with preview, exam-grade media playback and multi-format document lightboxes across Blazor Web and .NET MAUI Blazor Hybrid. The read-only blocks (stats, facts, meters, timelines and the rest) are typed components of Klyroo.UI.",
                            "All MIME/extension kind detection, file size formatting, RFC 4180 CSV parsing, and media track/progress contracts live in 100% NativeAOT C# (Klyroo.Display.Core)."
                        ],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "Klyroo.Display · Highlighted Code, Secret Mask & File List"),
                    new("pipeline", "How Klyroo.Display Works End-to-End (3 Subsystems)",
                        TableHeaders: ["Subsystem", "Core & UI Types", "What Happens"],
                        TableRows:
                        [
                            new("1. Code, Secrets & Files", "KlyCodeBlock + CodeLanguage + CodeLineRange · KlySecretBlock · KlyFileList + FileListItem", "Highlights code with a hand-written scanner (13 languages), numbers lines and emphasizes some of them, masks secrets with reveal and copy, and lists files that open in the preview."),
                            new("2. Constrained Media", "KlyAudioPlayer · KlyVideoPlayer · MediaPlayerControl · VideoPlayerTrack", "Enforces Start/End clip bounds, Seekable=false locking, MaxPlays quotas with OnLimitReached, speed presets, and WebVTT caption tracks."),
                            new("3. File Lightbox", "FilePreviewService + <KlyFilePreviewProvider />", "Auto-detects 10 FilePreviewKind formats via DetectKind(), picks Modal vs Fullscreen via DefaultContainerForKind(), parses CSVs via ParseCsv(), and supports multi-file galleries with loop navigation.")
                        ]),
                    new("where-blocks", "Where the Read-Only Blocks Went",
                        TableHeaders: ["Block", "Component", "Package"],
                        TableRows:
                        [
                            new("Stat, Meter, Facts, Breakdown", "KlyStat<TValue>, KlyStatGroup<TValue>, KlyMeter, KlyFacts, KlyBreakdown", "Klyroo.UI"),
                            new("Diff, Record, List, Date", "KlyDiff, KlyRecordList, KlyList, KlyDateTile", "Klyroo.UI"),
                            new("Heading, Text, Alert, Empty, Accordion", "KlyHeading, KlyText, KlyAlert, KlyEmpty, KlyAccordion", "Klyroo.UI"),
                            new("Chart, Table, Person, Timeline, Media", "KlyChart, KlyTable, KlyUser and KlyAvatarGroup, KlyTimeline, KlyImage and KlyImageGallery", "Klyroo.UI"),
                            new("Code and Secret", "KlyCodeBlock and KlySecretBlock", "Klyroo.Display"),
                            new("List of files", "KlyFileList", "Klyroo.Display")
                        ]),
                    new("layers", "Two-Layer Architecture",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Klyroo.Display.Core", "CodeHighlighter, CodeLanguage, CodeLineRange, FilePreviewService, FilePreviewFile, FilePreviewOptions, ParsedCsvPreview, MediaPlayerControl, VideoPlayerTrack, MediaPlayerProgress", "Headless C# · IsAotCompatible=true"),
                            new("Klyroo.Display", "KlyCodeBlock, KlySecretBlock, KlyFileList, KlyAudioPlayer, KlyVideoPlayer, KlyFilePreviewProvider, AddKlyrooDisplay()", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Klyroo.Display, register FilePreviewService, and mount KlyFilePreviewProvider once in your root layout.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Klyroo.Display.Core   # Headless NativeAOT highlighter, MIME detector, CSV parser & media models
                                dotnet add package Klyroo.Display        # KlyCodeBlock, KlySecretBlock, KlyFileList, players & Lightbox
                                """)
                        ]),
                    new("di", "2. Register Services & Mount Lightbox Host",
                        CodeSamples:
                        [
                            new("Program.cs / MauiProgram.cs", "builder.Services.AddKlyrooDisplay();"),
                            new("MainLayout.razor", "<KlyFilePreviewProvider />"),
                            new("_Imports.razor", "@using Klyroo.Display")
                        ])
                ]),

            // =========================================================================
            // GROUP 2: DISPLAY BLOCKS
            // =========================================================================
            new("code-blocks", "Highlighted Code, Line Numbers & Emphasized Lines", "Code, Secrets & Files",
                "Show a snippet with syntax colors, number its lines and draw the eye to some of them, or color code outside Blazor with the headless CodeHighlighter.",
                [
                    new("code-block", "KlyCodeBlock: Language, LineNumbers & HighlightLines",
                        Paragraphs:
                        [
                            "Language is a CodeLanguage (Bash, CSharp, Css, Html, JavaScript, Json, Less, Python, Scss, Sql, TypeScript, Xml, Yaml); without one the code is plain text. HighlightLines is a list of CodeLineRange counted from 1: an int converts to a one-line range and new CodeLineRange(4, 5) is a run.",
                            "The text content, the selection and the copy button always give the code as it is: line numbers are drawn with CSS and markup in the code is escaped."
                        ],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "KlyCodeBlock · Line Numbers & Emphasized Lines, KlySecretBlock, KlyFileList",
                        CodeSamples:
                        [
                            new("CodeBlock.razor", """
                                <KlyCodeBlock Label="Orders.sql" Code="@Query" Language="CodeLanguage.Sql" LineNumbers
                                              HighlightLines="@([2, new CodeLineRange(4, 5)])" />

                                @code {
                                    private const string Query = "SELECT id, total\nFROM orders\nWHERE status = 'paid'\n  AND total > 100\n  AND shipped_at IS NULL;\n";
                                }
                                """)
                        ]),
                    new("headless-highlighter", "CodeHighlighter: Highlight Outside Blazor",
                        Paragraphs: ["CodeHighlighter lives in Klyroo.Display.Core: Highlight returns escaped markup with data-token spans for an e-mail, a static page or a PDF, and Tokenize returns typed CodeToken values for your own rendering. Neither throws, and both work in NativeAOT."],
                        CodeSamples:
                        [
                            new("Highlighter.cs", """
                                var html = CodeHighlighter.Highlight("SELECT 1 -- one", CodeLanguage.Sql);
                                var language = CodeHighlighter.ResolveFileName("Program.cs"); // CodeLanguage.CSharp
                                foreach (var token in CodeHighlighter.Tokenize("var x = 1;", CodeLanguage.CSharp))
                                {
                                    Console.WriteLine($"{token.Kind}: {token.Text}");
                                }
                                """)
                        ])
                ]),
            new("secrets-and-lists", "Secrets & File Lists", "Code, Secrets & Files",
                "Mask a secret with a Reveal toggle and a copy button, and list attachments that open in the file preview, revealed a few at a time.",
                [
                    new("secret", "KlySecretBlock: Mask, Reveal & Copy",
                        Paragraphs:
                        [
                            "KlySecretBlock shows eight bullets followed by the last VisibleChars characters. While it is masked only those are in the page; the copy button copies the whole value.",
                            "The mask is a display feature and does not protect the value: do not send a secret to a client that must not hold it."
                        ],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "KlySecretBlock · Masked Secret with Reveal & Copy",
                        CodeSamples:
                        [
                            new("Secret.razor", """
                                <KlySecretBlock Label="Webhook secret" Value="sk_live_9876543210" VisibleChars="4" />
                                """)
                        ]),
                    new("file-list", "KlyFileList: Attachments That Open in the Preview",
                        Paragraphs: ["A FileListItem is a FilePreviewFile plus a Description, Meta, Icon and Tone; a plain FilePreviewFile converts to an item. A click opens the whole list as a gallery when a KlyFilePreviewProvider is mounted, and Max with Step reveals a long list a few files at a time."],
                        LiveDemoKey: "display-structured",
                        LiveDemoTitle: "KlyFileList · Progressive Reveal & Highlighted C#",
                        CodeSamples:
                        [
                            new("FileList.razor", """
                                <KlyFileList Title="Attachments" Files="_files" Max="3" Step="3" />

                                @code {
                                    private readonly IReadOnlyList<FileListItem> _files =
                                    [
                                        new FileListItem(new FilePreviewFile { Src = "/files/spec.pdf", Name = "Specification.pdf", Size = 1_572_864 },
                                            Description: "Version 3", Icon: "lock", Tone: KlyColor.Warning),
                                        new FilePreviewFile { Src = "/files/metrics.csv", Size = 18_400 },
                                    ];
                                }
                                """)
                        ]),
                    new("typed-blocks", "Read-Only Blocks Live in Klyroo.UI",
                        Paragraphs: ["Stats, meters, facts, breakdowns, diffs, record lists, timelines and the rest are typed Klyroo.UI components, each with only its own parameters, so a misuse does not compile."],
                        LiveDemoKey: "display-accordion-list",
                        LiveDemoTitle: "KlyAccordion & KlyList · Progressive Reveal",
                        CodeSamples:
                        [
                            new("TypedBlocks.razor", """
                                <KlyStat Title="MRR" Value="@_mrr" Format="C0" Delta="@_change" DeltaFormat="+0.0;-0.0" DeltaUnit="%" Trend="@(new double[] { 102, 118, 134, 148.9 })" />
                                <KlyMeter Title="Storage" Value="@_used" WarningThreshold="75" ErrorThreshold="90" />
                                <KlyFacts Items="@(new[] { new KlyFact("Environment") { Value = "Production", Copy = true, Mono = true } })" />
                                <KlyList Items="@(new[] { new KlyListItem("Compile") { Done = true }, new KlyListItem("Test") { Current = true } })" Variant="KlyListVariant.Steps" />
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 3: MEDIA & FILE PREVIEW
            // =========================================================================
            new("media-players", "Constrained Audio & Video Players", "Media & Files",
                "Play bounded audio and video segments with play-count quotas, rate selection, WebVTT subtitles, and seek locking.",
                [
                    new("audio-video", "KlyAudioPlayer & KlyVideoPlayer Constraints",
                        Paragraphs: ["Both players enforce Start/End clip bounds, Seekable=\"false\" scrubbing locks, MaxPlays quotas (two-way @bind-Plays + OnLimitReached), and fine-grained MediaPlayerControl hiding."],
                        TableHeaders: ["Parameter", "Type", "Purpose"],
                        TableRows:
                        [
                            new("Start / End", "double / double?", "Clip playback window in seconds"),
                            new("MaxPlays / Plays", "int / int", "Maximum allowed plays (0 = unlimited); fires OnLimitReached when reached"),
                            new("Seekable", "bool (true)", "Set false to block forward/backward scrubbing"),
                            new("Rate / Rates", "double / IReadOnlyList<double>?", "Playback speed options (e.g. 0.75x, 1x, 1.25x, 1.5x)"),
                            new("Hide", "IReadOnlyList<MediaPlayerControl>?", "Hide any of: Time, Volume, Rate, Captions, PictureInPicture, Fullscreen, Download"),
                            new("Tracks (Video)", "IReadOnlyList<VideoPlayerTrack>?", "WebVTT captions/subtitle tracks with default selection")
                        ],
                        LiveDemoKey: "display-audio",
                        LiveDemoTitle: "KlyAudioPlayer · Bounded Clip (15s–45s) & MaxPlays=2",
                        CodeSamples:
                        [
                            new("ExamMedia.razor", """
                                <KlyAudioPlayer Src="/media/listening-test.mp3" Title="Listening Comprehension · Extract 1"
                                               Start="15" End="45" MaxPlays="2" @bind-Plays="_playsUsed" Seekable="false"
                                               Hide="@([MediaPlayerControl.Download])" OnLimitReached="@HandleQuotaReached" />

                                <KlyVideoPlayer Src="/media/safety-briefing.mp4" Title="Annual Safety Briefing"
                                               Start="0" End="90" MaxPlays="3" Seekable="false"
                                               Tracks="@([new VideoPlayerTrack("/media/en.vtt", "English", "en", Default: true)])" />
                                """)
                        ])
                ]),
            new("file-preview", "FilePreviewService & Multi-Format Lightbox", "Media & Files",
                "Open single files or navigable galleries in Modal, Drawer, or Fullscreen lightboxes with headless MIME detection and RFC 4180 CSV parsing.",
                [
                    new("kinds", "All 10 FilePreviewKind Values & 3 Container Modes",
                        TableHeaders: ["Enum", "Exact Values", "Behavior"],
                        TableRows:
                        [
                            new("FilePreviewKind", "Pdf, Image, Video, Audio, Csv, Markdown, Text, Office, Archive, Other", "Auto-detected via DetectKind(fileNameOrUrl, mime) in Klyroo.Display.Core"),
                            new("FilePreviewContainer", "Modal, Drawer, Fullscreen", "DefaultContainerForKind() picks Fullscreen for PDFs/Office and Modal for Images/Media; override via FilePreviewOptions.Mode")
                        ],
                        LiveDemoKey: "display-csv-inspector",
                        LiveDemoTitle: "Interactive DetectKind, DefaultContainer & ParseCsv Inspector",
                        CodeSamples:
                        [
                            new("OpenGallery.cs", """
                                filePreview.OpenGallery(
                                    [
                                        new FilePreviewFile { Src = "/files/architecture.png", Name = "Architecture Diagram", Size = 248_900 },
                                        new FilePreviewFile { Src = "/files/spec-v2.pdf", Name = "Specification v2.pdf", Mime = "application/pdf" }
                                    ],
                                    new FilePreviewOptions { Index = 0, Mode = FilePreviewContainer.Modal, Loop = true, PdfPage = 1 });

                                var kind = FilePreviewService.DetectKind("report.csv");
                                var size = FilePreviewService.FormatFileSize(1_572_864);
                                var csv  = FilePreviewService.ParseCsv("Region,MRR\nEU,148900\nUS,192400", maxRows: 500);
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: INTEGRATIONS WITH OTHER KLYROO LIBRARIES
            // =========================================================================
            new("integrations-table-query", "Integration: Klyroo.Table & Klyroo.Query", "Integrations",
                "Open file lightboxes directly from Klyroo.Table row actions and summarize Klyroo.Query results and facet breakdowns with KlyStat and KlyFacts of Klyroo.UI.",
                [
                    new("table-lightbox", "1. Klyroo.Display + Klyroo.Table (Row Attachment Lightbox & Gallery)",
                        CodeSamples:
                        [
                            new("TableGalleryAction.cs", """
                                using Klyroo.Display;
                                using Klyroo.Table;

                                // Open the full current table page as a navigable gallery starting at the clicked row
                                void OpenRowInGallery(IReadOnlyList<DocumentRow> pageItems, int clickedIndex)
                                {
                                    filePreview.OpenGallery(
                                        [.. pageItems.Select(d => new FilePreviewFile
                                        {
                                            Src = d.DownloadUrl,
                                            Name = d.FileName,
                                            Size = d.SizeBytes,
                                            Details = [new FilePreviewDetail("Uploaded By", d.Author)]
                                        })],
                                        new FilePreviewOptions { Index = clickedIndex, Loop = true });
                                }
                                """)
                        ]),
                    new("query-stats", "2. Klyroo.Display + Klyroo.Query (Live Result & Facet Summary Ribbon)",
                        CodeSamples:
                        [
                            new("QuerySummaryRibbon.razor", """
                                <KlyStat Title="Matching Records" Value="@_result.TotalCount" Format="N0" />
                                <KlyFacts Items="@([.. (_result.Facets?.FirstOrDefault()?.Options ?? [])
                                    .Select(o => new KlyFact(o.Value) { Value = o.Count.ToString() })])" />
                                """)
                        ])
                ]),
            new("integrations-form-dashboard", "Integration: Klyroo.Form & Klyroo.Dashboard", "Integrations",
                "Review Klyroo.Form dirty diffs with KlyDiff before submission and preview Klyroo.Dashboard CSV exports inline with FilePreviewService.",
                [
                    new("form-diff", "1. Klyroo.Display + Klyroo.Form (Side-by-Side Change Diff Modal)",
                        CodeSamples:
                        [
                            new("FormChangeDiff.razor", """
                                <KlyDiff Title="Review Changes Before Saving" Items="@Changes()" />

                                @code {
                                    // One item per field whose IsFieldDirty is true: its label, the baseline text and the current text.
                                    private IReadOnlyList<KlyDiffItem> Changes() =>
                                    [
                                        .. new[] { (Label: "Name", Field: _name), (Label: "Plan", Field: _plan) }
                                            .Where(x => _form.IsFieldDirty(x.Field))
                                            .Select(x => new KlyDiffItem(x.Label, _baseline[x.Label], _form.Get(x.Field))),
                                    ];
                                }
                                """)
                        ]),
                    new("dashboard-csv", "2. Klyroo.Display + Klyroo.Dashboard (Inline CSV Export Preview)",
                        CodeSamples:
                        [
                            new("DashboardExportPreview.cs", """
                                var csv = DashboardFormat.ExportCsv(reportTable, CultureInfo.CurrentCulture);
                                filePreview.Open(new FilePreviewFile
                                {
                                    Name = DashboardFormat.ResolveFileName("Monthly Report"),
                                    Mime = "text/csv",
                                    Content = csv
                                });
                                """)
                        ])
                ]),
            new("integrations-spreadsheet-maui", "Integration: Klyroo.Spreadsheet & .NET MAUI", "Integrations",
                "Preview rejected CSV rows from Klyroo.Spreadsheet inside FilePreviewService and use Klyroo.Display.Core MIME/CSV utilities directly in native MAUI.",
                [
                    new("spreadsheet-errors", "1. Klyroo.Display + Klyroo.Spreadsheet (Preview Invalid Rows CSV)",
                        CodeSamples:
                        [
                            new("PreviewInvalidCsv.cs", """
                                using Klyroo.Display;
                                using Klyroo.Spreadsheet;

                                string invalidCsv = importer.ExportInvalidRowsCsv();
                                filePreview.Open(new FilePreviewFile
                                {
                                    Name = "rejected-rows.csv",
                                    Mime = "text/csv",
                                    Content = invalidCsv
                                });
                                """)
                        ]),
                    new("maui-core", "2. Headless Klyroo.Display.Core in Native .NET MAUI ViewModels",
                        Paragraphs: ["Use FilePreviewService.DetectKind, FormatFileSize, and ParseCsv directly in .NET MAUI ViewModels or background sync services with zero Blazor dependency."])
                ]),

            // =========================================================================
            // GROUP 5: PLATFORMS
            // =========================================================================
            MauiAndAotPage("Klyroo.Display", "Klyroo.Display.Core", "Klyroo.Display", "builder.Services.AddKlyrooDisplay();",
                "var kind = FilePreviewService.DetectKind(filePath); var csv = FilePreviewService.ParseCsv(rawText);"),

            // =========================================================================
            // GROUP 6: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive Klyroo.Display playgrounds exercising highlighted code with emphasized lines, the secret block and file lists, constrained exam audio playback, Klyroo.UI accordions and progressive lists, and headless FilePreviewService CSV/MIME inspection.",
                [
                    new("format-blocks", "1. Highlighted Code with Line Numbers & Emphasized Lines",
                        Paragraphs: ["Switch which lines of the query are emphasized: HighlightLines takes line numbers and CodeLineRange values, and the copied text stays the code as it is. A masked secret and a file list share the board."],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "KlyCodeBlock · KlySecretBlock · KlyFileList",
                        CodeSamples:
                        [
                            new("DisplayBlocks.razor", """
                                <KlyCodeBlock Label="Orders.sql" Code="@Query" Language="CodeLanguage.Sql" LineNumbers HighlightLines="@([new CodeLineRange(3, 4)])" />
                                <KlySecretBlock Label="API Token" Value="sk_live_klyroo_3210" VisibleChars="4" />
                                <KlyFileList Title="Attachments" Files="_files" />
                                """)
                        ]),
                    new("structured-inspector", "2. Progressive File List & Highlighted C#",
                        Paragraphs: ["KlyFileList with Max and Step reveals rows a few at a time; the C# sample is highlighted by the same headless CodeHighlighter that colors the documentation."],
                        LiveDemoKey: "display-structured",
                        LiveDemoTitle: "KlyFileList · Max & Step, KlyCodeBlock · C#",
                        CodeSamples:
                        [
                            new("StructuredBlocks.razor", """
                                <KlyFileList Title="Documents" Files="_files" Max="2" Step="2" />
                                <KlyCodeBlock Code="@Program" Language="CodeLanguage.CSharp" LineNumbers HighlightLines="@([3])" />
                                """)
                        ]),
                    new("audio-player", "3. Constrained Exam KlyAudioPlayer (Clip Bounds & MaxPlays)",
                        Paragraphs: ["Render a constrained exam-mode KlyAudioPlayer (Start=15s, End=45s, MaxPlays=2, Seekable=false) alongside headless FilePreviewService utilities."],
                        LiveDemoKey: "display-audio",
                        LiveDemoTitle: "KlyAudioPlayer & Headless FilePreviewService",
                        CodeSamples:
                        [
                            new("AudioAndPreview.razor", """
                                <KlyAudioPlayer Src="/media/sample.mp3" Title="Exam Listening Extract"
                                               Start="15" End="45" MaxPlays="2" Seekable="false" />
                                """)
                        ]),
                    new("accordion-list", "4. Klyroo.UI Accordion & Progressive Reveal List",
                        Paragraphs: ["The read-only blocks that used to be KlyDisplay kinds are Klyroo.UI components: expand several FAQ sections with KlyAccordion and reveal checklist rows with KlyList and Max=\"2\" Step=\"2\"."],
                        LiveDemoKey: "display-accordion-list",
                        LiveDemoTitle: "KlyAccordion & KlyList · Progressive Reveal",
                        CodeSamples:
                        [
                            new("AccordionList.razor", """
                                <KlyAccordion Multiple="true" Items="@_faqs" />
                                <KlyList Max="2" Step="2" Items="@_checklist" />
                                """)
                        ]),
                    new("csv-mime-inspector", "5. Live FilePreviewService MIME Detector & RFC 4180 CSV Parser",
                        Paragraphs: ["Type any filename (e.g. invoice.pdf, photo.png, clip.mp4, rates.csv, deck.pptx) to test DetectKind() and DefaultContainerForKind() live alongside ParseCsv()."],
                        LiveDemoKey: "display-csv-inspector",
                        LiveDemoTitle: "FilePreviewService · DetectKind, DefaultContainer & ParseCsv",
                        CodeSamples:
                        [
                            new("FileInspector.cs", """
                                var kind      = FilePreviewService.DetectKind("quarterly-report.pdf"); // FilePreviewKind.Pdf
                                var container = FilePreviewService.DefaultContainerForKind(kind);      // FilePreviewContainer.Fullscreen
                                var parsed    = FilePreviewService.ParseCsv(rawCsv, maxRows: 50);
                                """)
                        ])
                ]),
            new("api", "API Reference", "Reference",
                "Complete public API reference for Klyroo.Display.Core and Klyroo.Display.",
                [],
                ApiTypes: [typeof(KlyCodeBlock), typeof(KlySecretBlock), typeof(KlyFileList), typeof(CodeHighlighter), typeof(KlyAudioPlayer), typeof(KlyVideoPlayer), typeof(KlyFilePreviewProvider), typeof(FilePreviewService), typeof(FilePreviewFile), typeof(FilePreviewOptions)])
        ]);
}
