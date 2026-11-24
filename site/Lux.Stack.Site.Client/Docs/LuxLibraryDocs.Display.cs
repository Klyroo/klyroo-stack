using Lux.Display;

namespace Lux.Stack.Site.Client.Docs;

public static partial class LuxLibraryDocs
{
    private static LibraryDocTree BuildDisplay() => new(
        "display", "Lux.Display", "#ec4899",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "19 read-only display blocks (LxDisplay), constrained Audio/Video media players with clip bounds and play quotas (LxAudioPlayer, LxVideoPlayer), and a multi-format modal file previewer (FilePreviewService & LxFilePreviewProvider).",
                [
                    new("overview", "What is Lux.Display?",
                        Paragraphs:
                        [
                            "Lux.Display gives your application a unified, accessible vocabulary for read-only business data, exam-grade media playback, and multi-format document lightboxes across Blazor Web and .NET MAUI Blazor Hybrid.",
                            "All MIME/extension kind detection, file size formatting, RFC 4180 CSV parsing, and media track/progress contracts live in 100% NativeAOT C# (Lux.Display.Core)."
                        ],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "Lux.Display · Live Stat, Threshold Meter, Secret Mask & Diff"),
                    new("pipeline", "How Lux.Display Works End-to-End (3 Subsystems)",
                        TableHeaders: ["Subsystem", "Core & UI Types", "What Happens"],
                        TableRows:
                        [
                            new("1. Structured Blocks", "LxDisplay + 19 DisplayKind values + DisplaySurface", "Renders typed read-only blocks (Stat, Meter, Facts, Diff, Code/Secret, Timeline, Accordion, List) with automatic Loading skeletons and threshold tones."),
                            new("2. Constrained Media", "LxAudioPlayer · LxVideoPlayer · MediaPlayerControl · VideoPlayerTrack", "Enforces Start/End clip bounds, Seekable=false locking, MaxPlays quotas with OnLimitReached, speed presets, and WebVTT caption tracks."),
                            new("3. File Lightbox", "FilePreviewService + <LxFilePreviewProvider />", "Auto-detects 10 FilePreviewKind formats via DetectKind(), picks Modal vs Fullscreen via DefaultContainerForKind(), parses CSVs via ParseCsv(), and supports multi-file galleries with loop navigation.")
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
                            new("Lux.Display.Core", "FilePreviewService, FilePreviewFile, FilePreviewOptions, ParsedCsvPreview, MediaPlayerControl, VideoPlayerTrack, MediaPlayerProgress", "Headless C# · IsAotCompatible=true"),
                            new("Lux.Display", "LxDisplay, LxAudioPlayer, LxVideoPlayer, LxFilePreviewProvider, AddLuxDisplay()", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Lux.Display, register FilePreviewService, and mount LxFilePreviewProvider once in your root layout.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Lux.Display.Core   # Headless NativeAOT MIME detector, CSV parser & media models
                                dotnet add package Lux.Display        # LxDisplay, LxAudioPlayer, LxVideoPlayer & Lightbox
                                """)
                        ]),
                    new("di", "2. Register Services & Mount Lightbox Host",
                        CodeSamples:
                        [
                            new("Program.cs / MauiProgram.cs", "builder.Services.AddLuxDisplay();"),
                            new("MainLayout.razor", "<LxFilePreviewProvider />"),
                            new("_Imports.razor", "@using Lux.Display")
                        ])
                ]),

            // =========================================================================
            // GROUP 2: DISPLAY BLOCKS
            // =========================================================================
            new("content-blocks", "Typography, Alerts, Facts & Timelines", "Display Blocks",
                "Render headings with counts, expandable status alerts, copyable key/value fact lists, timelines, accordions, and progressive lists.",
                [
                    new("surfaces", "DisplaySurface & Common Parameters",
                        Paragraphs: ["Every LxDisplay block shares Surface (Plain, Muted, Bordered, Elevated), Tone (semantic LxColor), Loading (skeleton), Title, Description, Chip (DisplayChip), Action, and Actions (DisplayAction)."],
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
                        LiveDemoTitle: "LxDisplay · Copyable Facts & Pipeline Timeline",
                        CodeSamples:
                        [
                            new("FactsAndTimeline.razor", """
                                <LxDisplay Kind="DisplayKind.Facts" Surface="DisplaySurface.Bordered" Title="Cluster Metadata" Facts="@([
                                    new DisplayFact { Label = "Environment", Value = "Production", Copy = true, Mono = true },
                                    new DisplayFact { Label = "Region", Value = "eu-west-3", Chip = new DisplayChip("Healthy", LxColor.Success) },
                                    new DisplayFact { Label = "Failover", Value = null }
                                ])" />

                                <LxDisplay Kind="DisplayKind.Timeline" Surface="DisplaySurface.Bordered" Title="Release Pipeline" TimelineItems="@([
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
                                <LxDisplay Kind="DisplayKind.Stat" Title="MRR" Value="€148,900" Delta="+14.2%" Trend="@([102.0, 118.0, 134.0, 148.9])" />
                                <LxDisplay Kind="DisplayKind.Meter" Title="Storage Quota" MeterValue="@_val" MeterMax="100" WarningThreshold="75" ErrorThreshold="90" />
                                <LxDisplay Kind="DisplayKind.Code" Variant="secret" Title="Webhook Secret" Value="sk_live_9876543210" VisibleChars="4" Copy="true" />
                                <LxDisplay Kind="DisplayKind.Diff" Title="Plan Upgrade" DiffItems="@([new DisplayDiffItem("Tier", "Starter", "Enterprise", Changed: true)])" />
                                """)
                        ]),
                    new("accordion-list", "Accordion & Progressive Reveal Lists",
                        LiveDemoKey: "display-accordion-list",
                        LiveDemoTitle: "Interactive Accordion & Progressive Reveal List",
                        CodeSamples:
                        [
                            new("AccordionAndList.razor", """
                                <LxDisplay Kind="DisplayKind.Accordion" Multiple="true" AccordionItems="@([
                                    new DisplayAccordionItem("NativeAOT Support", "Lux.Display.Core has zero UI dependencies."),
                                    new DisplayAccordionItem("Secret Masking", "Reveal/hide in place with clipboard copy.")
                                ])" />

                                <LxDisplay Kind="DisplayKind.List" Max="2" Step="2" ListItems="@_steps" />
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 3: MEDIA & FILE PREVIEW
            // =========================================================================
            new("media-players", "Constrained Audio & Video Players", "Media & Files",
                "Play bounded audio and video segments with play-count quotas, rate selection, WebVTT subtitles, and seek locking.",
                [
                    new("audio-video", "LxAudioPlayer & LxVideoPlayer Constraints",
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
                        LiveDemoTitle: "LxAudioPlayer · Bounded Clip (15s–45s) & MaxPlays=2",
                        CodeSamples:
                        [
                            new("ExamMedia.razor", """
                                <LxAudioPlayer Src="/media/listening-test.mp3" Title="Listening Comprehension · Extract 1"
                                               Start="15" End="45" MaxPlays="2" @bind-Plays="_playsUsed" Seekable="false"
                                               Hide="@([MediaPlayerControl.Download])" OnLimitReached="@HandleQuotaReached" />

                                <LxVideoPlayer Src="/media/safety-briefing.mp4" Title="Annual Safety Briefing"
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
                            new("FilePreviewKind", "Pdf, Image, Video, Audio, Csv, Markdown, Text, Office, Archive, Other", "Auto-detected via DetectKind(fileNameOrUrl, mime) in Lux.Display.Core"),
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
            // GROUP 4: INTEGRATIONS WITH OTHER LUXSTACK LIBRARIES
            // =========================================================================
            new("integrations-table-query", "Integration: Lux.Table & Lux.Query", "Integrations",
                "Open file lightboxes directly from Lux.Table row actions and summarize Lux.Query results and facet breakdowns with LxDisplay blocks.",
                [
                    new("table-lightbox", "1. Lux.Display + Lux.Table (Row Attachment Lightbox & Gallery)",
                        CodeSamples:
                        [
                            new("TableGalleryAction.cs", """
                                using Lux.Display;
                                using Lux.Table;

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
                    new("query-stats", "2. Lux.Display + Lux.Query (Live Result & Facet Summary Ribbon)",
                        CodeSamples:
                        [
                            new("QuerySummaryRibbon.razor", """
                                <LxDisplay Kind="DisplayKind.Stat" Title="Matching Records" Value="@(_result.TotalCount?.ToString("N0") ?? "—")" />
                                <LxDisplay Kind="DisplayKind.Facts" Title="Facet Counts" Facts="@([.. (_result.Facets?.FirstOrDefault()?.Options ?? [])
                                    .Select(o => new DisplayFact { Label = o.Value, Value = o.Count.ToString() })])" />
                                """)
                        ])
                ]),
            new("integrations-form-dashboard", "Integration: Lux.Form & Lux.Dashboard", "Integrations",
                "Review Lux.Form dirty diffs with DisplayKind.Diff before submission and preview Lux.Dashboard CSV exports inline with FilePreviewService.",
                [
                    new("form-diff", "1. Lux.Display + Lux.Form (Side-by-Side Change Diff Modal)",
                        CodeSamples:
                        [
                            new("FormChangeDiff.razor", """
                                <LxDisplay Kind="DisplayKind.Diff" Title="Review Changes Before Saving"
                                           DiffItems="@([.. _form.DirtyPaths.Select(key =>
                                               new DisplayDiffItem(key, _baseline[key]?.ToString(), _form.GetValue<object>(key)?.ToString(), Changed: true))])" />
                                """)
                        ]),
                    new("dashboard-csv", "2. Lux.Display + Lux.Dashboard (Inline CSV Export Preview)",
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
            new("integrations-spreadsheet-maui", "Integration: Lux.Spreadsheet & .NET MAUI", "Integrations",
                "Preview rejected CSV rows from Lux.Spreadsheet inside FilePreviewService and use Lux.Display.Core MIME/CSV utilities directly in native MAUI.",
                [
                    new("spreadsheet-errors", "1. Lux.Display + Lux.Spreadsheet (Preview Invalid Rows CSV)",
                        CodeSamples:
                        [
                            new("PreviewInvalidCsv.cs", """
                                using Lux.Display;
                                using Lux.Spreadsheet;

                                string invalidCsv = importer.ExportInvalidRowsCsv();
                                filePreview.Open(new FilePreviewFile
                                {
                                    Name = "rejected-rows.csv",
                                    Mime = "text/csv",
                                    Content = invalidCsv
                                });
                                """)
                        ]),
                    new("maui-core", "2. Headless Lux.Display.Core in Native .NET MAUI ViewModels",
                        Paragraphs: ["Use FilePreviewService.DetectKind, FormatFileSize, and ParseCsv directly in .NET MAUI ViewModels or background sync services with zero Blazor dependency."])
                ]),

            // =========================================================================
            // GROUP 5: PLATFORMS
            // =========================================================================
            MauiAndAotPage("Lux.Display", "Lux.Display.Core", "Lux.Display", "builder.Services.AddLuxDisplay();",
                "var kind = FilePreviewService.DetectKind(filePath); var csv = FilePreviewService.ParseCsv(rawText);"),

            // =========================================================================
            // GROUP 6: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive Lux.Display playgrounds exercising Stat/Meter/Secret/Diff blocks, Facts & Timelines, constrained exam audio playback, Accordions & progressive Lists, and headless FilePreviewService CSV/MIME inspection.",
                [
                    new("format-blocks", "1. Stat, Threshold-Reactive Meter, Masked Secret & Diff",
                        Paragraphs: ["Toggle Storage Quota across Normal (55%), Warning (82%), and Error (95%) thresholds alongside Stat sparklines, masked secret code blocks, and Diff rows."],
                        LiveDemoKey: "display-blocks",
                        LiveDemoTitle: "LxDisplay · Stat, Meter, Secret Code & Diff",
                        CodeSamples:
                        [
                            new("DisplayBlocks.razor", """
                                <LxDisplay Kind="DisplayKind.Stat" Title="MRR" Value="€148,900" Delta="+14.2%" Trend="@([102.0, 118.0, 134.0, 148.9])" />
                                <LxDisplay Kind="DisplayKind.Meter" Title="Storage Quota" MeterValue="@_val" MeterMax="100" WarningThreshold="75" ErrorThreshold="90" />
                                <LxDisplay Kind="DisplayKind.Code" Variant="secret" Title="API Token" Value="sk_live_luxstack_3210" VisibleChars="4" Copy="true" />
                                <LxDisplay Kind="DisplayKind.Diff" Title="Plan Upgrade" DiffItems="@([new DisplayDiffItem("Tier", "Starter", "Enterprise", Changed: true)])" />
                                """)
                        ]),
                    new("structured-inspector", "2. Copyable Facts & Release Pipeline Timeline",
                        Paragraphs: ["Combine DisplayKind.Facts (copyable monospace metadata, status chips, null-safe fallbacks) with DisplayKind.Timeline (Done, Current, Upcoming)."],
                        LiveDemoKey: "display-structured",
                        LiveDemoTitle: "LxDisplay · Facts & Timeline",
                        CodeSamples:
                        [
                            new("StructuredBlocks.razor", """
                                <LxDisplay Kind="DisplayKind.Facts" Surface="DisplaySurface.Bordered" Title="Cluster Metadata" Facts="@([
                                    new DisplayFact { Label = "Environment", Value = "Production", Copy = true, Mono = true },
                                    new DisplayFact { Label = "Region", Value = "eu-west-3", Chip = new DisplayChip("Healthy", LxColor.Success) }
                                ])" />
                                <LxDisplay Kind="DisplayKind.Timeline" Surface="DisplaySurface.Bordered" Title="Release Pipeline" TimelineItems="@([
                                    new DisplayTimelineItem("Build NativeAOT Core", Time: "09:12", State: DisplayTimelineState.Done),
                                    new DisplayTimelineItem("Rollout Canary", Time: "In progress", State: DisplayTimelineState.Current)
                                ])" />
                                """)
                        ]),
                    new("audio-player", "3. Constrained Exam LxAudioPlayer (Clip Bounds & MaxPlays)",
                        Paragraphs: ["Render a constrained exam-mode LxAudioPlayer (Start=15s, End=45s, MaxPlays=2, Seekable=false) alongside headless FilePreviewService utilities."],
                        LiveDemoKey: "display-audio",
                        LiveDemoTitle: "LxAudioPlayer & Headless FilePreviewService",
                        CodeSamples:
                        [
                            new("AudioAndPreview.razor", """
                                <LxAudioPlayer Src="/media/sample.mp3" Title="Exam Listening Extract"
                                               Start="15" End="45" MaxPlays="2" Seekable="false" />
                                """)
                        ]),
                    new("accordion-list", "4. Multi-Open Accordion & Progressive Reveal List",
                        Paragraphs: ["Expand multiple FAQ sections with DisplayKind.Accordion and reveal additional checklist rows incrementally using Max=\"2\" Step=\"2\"."],
                        LiveDemoKey: "display-accordion-list",
                        LiveDemoTitle: "LxDisplay · Accordion & Progressive List",
                        CodeSamples:
                        [
                            new("AccordionList.razor", """
                                <LxDisplay Kind="DisplayKind.Accordion" Multiple="true" AccordionItems="@_faqs" />
                                <LxDisplay Kind="DisplayKind.List" Max="2" Step="2" ListItems="@_checklist" />
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
                "Complete public API reference for Lux.Display.Core and Lux.Display.",
                [],
                ApiTypes: [typeof(LxDisplay), typeof(LxAudioPlayer), typeof(LxVideoPlayer), typeof(LxFilePreviewProvider), typeof(FilePreviewService), typeof(FilePreviewFile), typeof(FilePreviewOptions)])
        ]);
}
