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
                "19 read-only display blocks (KlyDisplay), constrained Audio/Video media players with clip bounds and play quotas (KlyAudioPlayer, KlyVideoPlayer), and a multi-format modal file previewer (FilePreviewService & KlyFilePreviewProvider).",
                [
                    new("overview", "What is Klyroo.Display?",
                        Paragraphs:
                        [
                            "Klyroo.Display gives your application a unified, accessible vocabulary for read-only business data, exam-grade media playback, and multi-format document lightboxes across Blazor Web and .NET MAUI Blazor Hybrid.",
                            "All MIME/extension kind detection, file size formatting, RFC 4180 CSV parsing, and media track/progress contracts live in 100% NativeAOT C# (Klyroo.Display.Core)."
                        ],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "Klyroo.Display · Live Stat, Threshold Meter, Secret Mask & Diff"),
                    new("pipeline", "How Klyroo.Display Works End-to-End (3 Subsystems)",
                        TableHeaders: ["Subsystem", "Core & UI Types", "What Happens"],
                        TableRows:
                        [
                            new("1. Structured Blocks", "KlyDisplay + 19 DisplayKind values + DisplaySurface", "Renders typed read-only blocks (Stat, Meter, Facts, Diff, Code/Secret, Timeline, Accordion, List) with automatic Loading skeletons and threshold tones."),
                            new("2. Constrained Media", "KlyAudioPlayer · KlyVideoPlayer · MediaPlayerControl · VideoPlayerTrack", "Enforces Start/End clip bounds, Seekable=false locking, MaxPlays quotas with OnLimitReached, speed presets, and WebVTT caption tracks."),
                            new("3. File Lightbox", "FilePreviewService + <KlyFilePreviewProvider />", "Auto-detects 10 FilePreviewKind formats via DetectKind(), picks Modal vs Fullscreen via DefaultContainerForKind(), parses CSVs via ParseCsv(), and supports multi-file galleries with loop navigation.")
                        ]),
                    new("all-kinds", "All 19 DisplayKind Values",
                        TableHeaders: ["Category", "Exact DisplayKind Values", "Highlights"],
                        TableRows:
                        [
                            new("Editorial & Status (4)", "Heading, Text, Alert, Empty", "Section titles with count pills, collapsible Alert details, empty states"),
                            new("Metrics & Visuals (5)", "Facts, Stat, Breakdown, Meter, Chart", "Copyable fact grids, sparklines, breakdown lines, threshold-colored meters"),
                            new("Structured Data (6)", "Table, Diff, Record, Person, List, Accordion", "Before/after diffs, record cards, progressive Reveal More lists, accordions"),
                            new("Rich & Technical (4)", "Timeline, Date, Media, Code", "Timeline states (Done/Current/Upcoming), relative dates, image galleries & Variant=\"secret\" code")
                        ]),
                    new("layers", "Two-Layer Architecture",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Klyroo.Display.Core", "FilePreviewService, FilePreviewFile, FilePreviewOptions, ParsedCsvPreview, MediaPlayerControl, VideoPlayerTrack, MediaPlayerProgress", "Headless C# · IsAotCompatible=true"),
                            new("Klyroo.Display", "KlyDisplay, KlyAudioPlayer, KlyVideoPlayer, KlyFilePreviewProvider, AddKlyrooDisplay()", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Klyroo.Display, register FilePreviewService, and mount KlyFilePreviewProvider once in your root layout.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Klyroo.Display.Core   # Headless NativeAOT MIME detector, CSV parser & media models
                                dotnet add package Klyroo.Display        # KlyDisplay, KlyAudioPlayer, KlyVideoPlayer & Lightbox
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
            new("content-blocks", "Typography, Alerts, Facts & Timelines", "Display Blocks",
                "Render headings with counts, expandable status alerts, copyable key/value fact lists, timelines, accordions, and progressive lists.",
                [
                    new("surfaces", "DisplaySurface & Common Parameters",
                        Paragraphs: ["Every KlyDisplay block shares Surface (Plain, Muted, Bordered, Elevated), Tone (semantic KlyColor), Loading (skeleton), Title, Description, Chip (DisplayChip), Action, and Actions (DisplayAction)."],
                        TableHeaders: ["DisplaySurface", "Visual Style", "Best For"],
                        TableRows:
                        [
                            new("Plain", "Transparent background, no border", "Inline embedding inside existing cards"),
                            new("Muted", "Soft tinted surface", "Callouts, alert details, subtle groupings"),
                            new("Bordered", "1px hairline border on card background", "Standalone KPI and fact blocks"),
                            new("Elevated", "Raised surface with subtle shadow", "Primary focus cards and featured summaries")
                        ]),
                    new("structured", "Facts & Release Timelines",
                        LiveDemoKey: "display-structured",
                        LiveDemoTitle: "KlyDisplay · Copyable Facts & Pipeline Timeline",
                        CodeSamples:
                        [
                            new("FactsAndTimeline.razor", """
                                <KlyDisplay Kind="DisplayKind.Facts" Surface="DisplaySurface.Bordered" Title="Cluster Metadata" Facts="@([
                                    new DisplayFact { Label = "Environment", Value = "Production", Copy = true, Mono = true },
                                    new DisplayFact { Label = "Region", Value = "eu-west-3", Chip = new DisplayChip("Healthy", KlyColor.Success) },
                                    new DisplayFact { Label = "Failover", Value = null }
                                ])" />

                                <KlyDisplay Kind="DisplayKind.Timeline" Surface="DisplaySurface.Bordered" Title="Release Pipeline" TimelineItems="@([
                                    new DisplayTimelineItem("Build NativeAOT Core", Time: "09:12", State: DisplayTimelineState.Done),
                                    new DisplayTimelineItem("Rollout Canary", Time: "In progress", State: DisplayTimelineState.Current),
                                    new DisplayTimelineItem("Global Promotion", Time: "Pending", State: DisplayTimelineState.Upcoming)
                                ])" />
                                """)
                        ])
                ]),
            new("metrics-and-code", "Stats, Threshold Meters, Secret Masking & Diffs", "Display Blocks",
                "Present inline KPIs with sparklines, threshold-reactive quota meters, masked secrets with reveal/copy controls, accordions, progressive lists, and before/after diffs.",
                [
                    new("metrics", "Stat, Meter, Secret Code & Diff",
                        Paragraphs:
                        [
                            "DisplayKind.Meter automatically transitions its tone to warning at WarningThreshold and error at ErrorThreshold.",
                            "DisplayKind.Code with Variant=\"secret\" masks leading characters while keeping the last VisibleChars visible, with built-in reveal toggle and unmasked clipboard copy."
                        ],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "Live Stat, Threshold Meter, Secret Mask & Diff",
                        CodeSamples:
                        [
                            new("MetricsAndSecret.razor", """
                                <KlyDisplay Kind="DisplayKind.Stat" Title="MRR" Value="€148,900" Delta="+14.2%" Trend="@([102.0, 118.0, 134.0, 148.9])" />
                                <KlyDisplay Kind="DisplayKind.Meter" Title="Storage Quota" MeterValue="@_val" MeterMax="100" WarningThreshold="75" ErrorThreshold="90" />
                                <KlyDisplay Kind="DisplayKind.Code" Variant="secret" Title="Webhook Secret" Value="sk_live_9876543210" VisibleChars="4" Copy="true" />
                                <KlyDisplay Kind="DisplayKind.Diff" Title="Plan Upgrade" DiffItems="@([new DisplayDiffItem("Tier", "Starter", "Enterprise", Changed: true)])" />
                                """)
                        ]),
                    new("accordion-list", "Accordion & Progressive Reveal Lists",
                        LiveDemoKey: "display-accordion-list",
                        LiveDemoTitle: "Interactive Accordion & Progressive Reveal List",
                        CodeSamples:
                        [
                            new("AccordionAndList.razor", """
                                <KlyDisplay Kind="DisplayKind.Accordion" Multiple="true" AccordionItems="@([
                                    new DisplayAccordionItem("NativeAOT Support", "Klyroo.Display.Core has zero UI dependencies."),
                                    new DisplayAccordionItem("Secret Masking", "Reveal/hide in place with clipboard copy.")
                                ])" />

                                <KlyDisplay Kind="DisplayKind.List" Max="2" Step="2" ListItems="@_steps" />
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
                "Open file lightboxes directly from Klyroo.Table row actions and summarize Klyroo.Query results and facet breakdowns with KlyDisplay blocks.",
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
                                <KlyDisplay Kind="DisplayKind.Stat" Title="Matching Records" Value="@(_result.TotalCount?.ToString("N0") ?? "—")" />
                                <KlyDisplay Kind="DisplayKind.Facts" Title="Facet Counts" Facts="@([.. (_result.Facets?.FirstOrDefault()?.Options ?? [])
                                    .Select(o => new DisplayFact { Label = o.Value, Value = o.Count.ToString() })])" />
                                """)
                        ])
                ]),
            new("integrations-form-dashboard", "Integration: Klyroo.Form & Klyroo.Dashboard", "Integrations",
                "Review Klyroo.Form dirty diffs with DisplayKind.Diff before submission and preview Klyroo.Dashboard CSV exports inline with FilePreviewService.",
                [
                    new("form-diff", "1. Klyroo.Display + Klyroo.Form (Side-by-Side Change Diff Modal)",
                        CodeSamples:
                        [
                            new("FormChangeDiff.razor", """
                                <KlyDisplay Kind="DisplayKind.Diff" Title="Review Changes Before Saving"
                                           DiffItems="@([.. _form.DirtyPaths.Select(key =>
                                               new DisplayDiffItem(key, _baseline[key]?.ToString(), _form.GetValue<object>(key)?.ToString(), Changed: true))])" />
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
                "5 interactive Klyroo.Display playgrounds exercising Stat/Meter/Secret/Diff blocks, Facts & Timelines, constrained exam audio playback, Accordions & progressive Lists, and headless FilePreviewService CSV/MIME inspection.",
                [
                    new("format-blocks", "1. Stat, Threshold-Reactive Meter, Masked Secret & Diff",
                        Paragraphs: ["Toggle Storage Quota across Normal (55%), Warning (82%), and Error (95%) thresholds alongside Stat sparklines, masked secret code blocks, and Diff rows."],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "KlyDisplay · Stat, Meter, Secret Code & Diff",
                        CodeSamples:
                        [
                            new("DisplayBlocks.razor", """
                                <KlyDisplay Kind="DisplayKind.Stat" Title="MRR" Value="€148,900" Delta="+14.2%" Trend="@([102.0, 118.0, 134.0, 148.9])" />
                                <KlyDisplay Kind="DisplayKind.Meter" Title="Storage Quota" MeterValue="@_val" MeterMax="100" WarningThreshold="75" ErrorThreshold="90" />
                                <KlyDisplay Kind="DisplayKind.Code" Variant="secret" Title="API Token" Value="sk_live_klyroo_3210" VisibleChars="4" Copy="true" />
                                <KlyDisplay Kind="DisplayKind.Diff" Title="Plan Upgrade" DiffItems="@([new DisplayDiffItem("Tier", "Starter", "Enterprise", Changed: true)])" />
                                """)
                        ]),
                    new("structured-inspector", "2. Copyable Facts & Release Pipeline Timeline",
                        Paragraphs: ["Combine DisplayKind.Facts (copyable monospace metadata, status chips, null-safe fallbacks) with DisplayKind.Timeline (Done, Current, Upcoming)."],
                        LiveDemoKey: "display-structured",
                        LiveDemoTitle: "KlyDisplay · Facts & Timeline",
                        CodeSamples:
                        [
                            new("StructuredBlocks.razor", """
                                <KlyDisplay Kind="DisplayKind.Facts" Surface="DisplaySurface.Bordered" Title="Cluster Metadata" Facts="@([
                                    new DisplayFact { Label = "Environment", Value = "Production", Copy = true, Mono = true },
                                    new DisplayFact { Label = "Region", Value = "eu-west-3", Chip = new DisplayChip("Healthy", KlyColor.Success) }
                                ])" />
                                <KlyDisplay Kind="DisplayKind.Timeline" Surface="DisplaySurface.Bordered" Title="Release Pipeline" TimelineItems="@([
                                    new DisplayTimelineItem("Build NativeAOT Core", Time: "09:12", State: DisplayTimelineState.Done),
                                    new DisplayTimelineItem("Rollout Canary", Time: "In progress", State: DisplayTimelineState.Current)
                                ])" />
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
                    new("accordion-list", "4. Multi-Open Accordion & Progressive Reveal List",
                        Paragraphs: ["Expand multiple FAQ sections with DisplayKind.Accordion and reveal additional checklist rows incrementally using Max=\"2\" Step=\"2\"."],
                        LiveDemoKey: "display-accordion-list",
                        LiveDemoTitle: "KlyDisplay · Accordion & Progressive List",
                        CodeSamples:
                        [
                            new("AccordionList.razor", """
                                <KlyDisplay Kind="DisplayKind.Accordion" Multiple="true" AccordionItems="@_faqs" />
                                <KlyDisplay Kind="DisplayKind.List" Max="2" Step="2" ListItems="@_checklist" />
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
                ApiTypes: [typeof(KlyDisplay), typeof(KlyAudioPlayer), typeof(KlyVideoPlayer), typeof(KlyFilePreviewProvider), typeof(FilePreviewService), typeof(FilePreviewFile), typeof(FilePreviewOptions)])
        ]);
}
