using Klyroo.Spreadsheet;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class KlyrooLibraryDocs
{
    private static LibraryDocTree BuildSpreadsheet() => new(
        "spreadsheet", "Klyroo.Spreadsheet", "#14b8a6",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "Headless NativeAOT CSV/TSV/workbook importer with 20-row header layout detection, NFD normalization, >=80% pass-rate column suggestions, unknown value questions, duplicate/upsert reconciliation, inline cell editing, CSV template/error exports, and batched submission with rejected-row retry.",
                [
                    new("overview", "What is Klyroo.Spreadsheet?",
                        Paragraphs:
                        [
                            "Production spreadsheet uploads from Excel, ERPs, and Google Sheets rarely match a clean schema: title rows appear above headers, accented column names carry asterisks or '(required)', European decimals use commas, and select values contain typos or legacy codes.",
                            "Klyroo.Spreadsheet puts the entire parsing, layout detection, NFD header normalization, >=80% sample validation scoring, reconciliation, review filtering, and batch retry state machine inside 100% NativeAOT C# (Klyroo.Spreadsheet.Core), paired with the 5-step KlySpreadsheetImport wizard for Blazor Web and .NET MAUI Blazor Hybrid."
                        ],
                        LiveDemoKey: "spreadsheet-wizard",
                        LiveDemoTitle: "Klyroo.Spreadsheet · Live 5-Step Import Wizard"),
                    new("pipeline", "How Klyroo.Spreadsheet Works End-to-End (The 5-Step State Machine)",
                        TableHeaders: ["Step", "Capabilities", "Key SpreadsheetImporter Members"],
                        TableRows:
                        [
                            new("1. Source & Layout", "CSV, TSV, clipboard Paste, multi-sheet Workbook, 20-row header & delimiter detection", "LoadText, Paste, LoadWorkbook, SetSheet, SetHeaderRow, HeaderRowNumber"),
                            new("2. Column Mapping", "Unicode NFD normalization, alias matching, >=80% sample-validated Suggestions", "Fields, MissingFields, AssignColumn, IgnoreColumn, ResetColumn"),
                            new("3. Unknown Values", "Collect distinct unrecognized Select strings and answer globally (Map/LeaveEmpty/SkipRows/Create)", "Questions, OpenQuestions, AnswerValue, ClearAnswer"),
                            new("4. Review & Edit", "Intra-file duplicates, ExistingRecords upsert detection, review tabs/levels, inline EditCell & Discard", "AllRows, ImportableRows, InvalidRows, VisibleReviewRows, EditCell, RevertCell, DiscardRows"),
                            new("5. Batch Submit", "Chunked async OnSubmit, progress tracking, targeted RetryRejectedAsync & CSV exports", "SubmitAsync, RetryRejectedAsync, ExportTemplateCsv, ExportInvalidRowsCsv")
                        ]),
                    new("layers", "Two-Layer Architecture",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Klyroo.Spreadsheet.Core", "SpreadsheetImporter, SpreadsheetSchema, SpreadsheetColumnSchema, SpreadsheetRule, SpreadsheetOption, SpreadsheetRow", "Headless C# · IsAotCompatible=true"),
                            new("Klyroo.Spreadsheet", "KlySpreadsheetImport, AddKlyrooSpreadsheet()", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Klyroo.Spreadsheet and register services.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Klyroo.Spreadsheet.Core   # Headless NativeAOT import engine
                                dotnet add package Klyroo.Spreadsheet        # 5-step Blazor Web & .NET MAUI wizard
                                """)
                        ]),
                    new("di", "2. Register Services & Imports",
                        CodeSamples:
                        [
                            new("Program.cs / MauiProgram.cs", "builder.Services.AddKlyrooSpreadsheet();"),
                            new("_Imports.razor", "@using Klyroo.Spreadsheet")
                        ])
                ]),

            // =========================================================================
            // GROUP 2: IMPORT PIPELINE DEEP DIVE
            // =========================================================================
            new("schema", "Schema, Columns & Validation Rules", "Import Pipeline",
                "Configure column kinds, header aliases, decimal separators, date formats, built-in validation rules, custom parsers, and cross-field row validators.",
                [
                    new("kinds", "All 5 SpreadsheetColumnKind Values & Built-In SpreadsheetRule Factories",
                        TableHeaders: ["Column Kind / Rule", "Configuration", "Behavior"],
                        TableRows:
                        [
                            new("SpreadsheetColumnKind.Text", "SpreadsheetRule.Email(), Pattern(regex)", "Trimmed string with regex/email validation and Error or Warning IssueLevel"),
                            new("SpreadsheetColumnKind.Number", "DecimalSeparator ('.' or ','), Min, Max, Between", "Locale-aware number parsing with numeric bounds"),
                            new("SpreadsheetColumnKind.Date", "DateFormats (custom format list)", "Exact and ISO date parsing"),
                            new("SpreadsheetColumnKind.Boolean", "DefaultValue / HasDefault", "Parses true/false, yes/no, 1/0"),
                            new("SpreadsheetColumnKind.Select", "Options, UnknownPolicy, OnCreateOption", "Matches Value, Label, or Aliases case/accent-insensitively")
                        ]),
                    new("example", "Full SpreadsheetSchema with Cross-Field ValidateRow & Batch OnSubmit",
                        CodeSamples:
                        [
                            new("RatesSchema.cs", """
                                var schema = new SpreadsheetSchema
                                {
                                    Key = "rates",
                                    MaxRows = 5000,
                                    RowKeyField = "code",
                                    Duplicates = SpreadsheetDuplicatesPolicy.Error,
                                    ExistingAction = SpreadsheetExistingAction.Update,
                                    BatchSize = 100,
                                    Columns =
                                    [
                                        new SpreadsheetColumnSchema { Key = "code", Label = "Code", Headers = ["SKU", "Item Code"], Required = true },
                                        new SpreadsheetColumnSchema
                                        {
                                            Key = "price", Label = "Unit Price", Kind = SpreadsheetColumnKind.Number,
                                            DecimalSeparator = ',', Required = true,
                                            Rules = [SpreadsheetRule.Between(0, 100_000, "Price must be between 0 and 100,000")]
                                        },
                                        new SpreadsheetColumnSchema
                                        {
                                            Key = "currency", Label = "Currency", Kind = SpreadsheetColumnKind.Select,
                                            Required = true, UnknownPolicy = SpreadsheetUnknownValuePolicy.Ask,
                                            Options =
                                            [
                                                new SpreadsheetOption("EUR", "Euro", ["€", "EUR"]),
                                                new SpreadsheetOption("USD", "US Dollar", ["$", "USD"])
                                            ]
                                        }
                                    ],
                                    ValidateRow = data =>
                                        (double?)data.GetValueOrDefault("price") == 0
                                            ? [new SpreadsheetRowIssue("price", "Zero-price item marked promotional", SpreadsheetIssueLevel.Warning)]
                                            : [],
                                    OnSubmit = async batch => await api.UpsertRatesBatchAsync(batch)
                                };
                                """)
                        ])
                ]),
            new("detection-and-mapping", "Header Detection, NFD & 80% Suggestions", "Import Pipeline",
                "Scan the first 20 rows for the real header line, match multilingual accented headers via Unicode NFD, and propose missing mappings at >=80% sample pass rate.",
                [
                    new("nfd", "Unicode NFD Header Normalization & Column Statuses",
                        Paragraphs:
                        [
                            "LoadText() and Paste() auto-detect comma, semicolon, or tab delimiters and score the first 20 rows against column keys, labels, and Headers aliases.",
                            "SpreadsheetImporter.NormalizeText() decomposes Unicode (FormD), strips diacritics, removes '*' and '(required)' markers, and collapses whitespace. Any still-unmapped column gets a Suggestion ONLY when >=80% of non-empty sample cells parse and pass validation rules."
                        ],
                        TableHeaders: ["SpreadsheetColumnStatus", "Meaning", "Importer Method"],
                        TableRows:
                        [
                            new("Matched", "Bound to a source column (exact, alias, or manual)", "ResetColumn(key) clears manual override"),
                            new("Default", "Unmapped column populated from DefaultValue", "AssignColumn(key, headerIndex) overrides default"),
                            new("Missing", "Required column still unmapped (check .Suggestion)", "AssignColumn(key, suggestion.HeaderIndex)"),
                            new("Unmatched", "Optional unmapped or explicitly ignored column", "IgnoreColumn(key)")
                        ],
                        LiveDemoKey: "spreadsheet-nfd",
                        LiveDemoTitle: "Interactive Unicode NFD NormalizeText & ExportTemplateCsv()")
                ]),
            new("reconciliation-and-retry", "Unknown Values, Reconciliation, Editing & Batch Retry", "Import Pipeline",
                "Resolve unknown Select strings globally, reconcile duplicates & existing records, edit cells inline, export invalid rows, and retry only server-rejected rows.",
                [
                    new("reconcile", "Reconciliation Policies & Unknown Value Questions",
                        TableHeaders: ["Enum", "Exact Values", "Purpose"],
                        TableRows:
                        [
                            new("SpreadsheetUnknownValuePolicy", "Ask, Error, SkipRows, LeaveEmpty, Create", "How unrecognized Select cell values are handled"),
                            new("SpreadsheetDuplicatesPolicy", "Error, KeepFirst, KeepLast", "Handles repeated RowKeyField keys inside the uploaded file"),
                            new("SpreadsheetExistingAction", "Update, Skip, Error", "Handles rows matching Schema.ExistingRecords (sets Mode = Create/Update/Skip)"),
                            new("SpreadsheetRowStatus", "Valid, Warning, Blocking, Discarded", "Computed validation state per row"),
                            new("SpreadsheetDiscardReason", "Manual, Value, Limit, Duplicate, Existing", "Why a row was excluded from ImportableRows")
                        ],
                        LiveDemoKey: "spreadsheet-reconcile",
                        LiveDemoTitle: "Interactive Unknown Value Answer, Duplicate Fix & Upsert Detection"),
                    new("edit-retry", "Inline Cell Repair, Invalid CSV Export & Targeted Batch Retry",
                        Paragraphs: ["When a backend rejects specific rows via SpreadsheetRejectedRow(RowIndex, Message, Field), already-succeeded batches stay committed. Edit only the rejected cells and call RetryRejectedAsync()."],
                        LiveDemoKey: "spreadsheet-batch-submit",
                        LiveDemoTitle: "Interactive Chunked SubmitAsync() & RetryRejectedAsync()")
                ]),

            // =========================================================================
            // GROUP 3: INTEGRATIONS WITH OTHER KLYROO LIBRARIES
            // =========================================================================
            new("integrations-table-query", "Integration: Klyroo.Query & Klyroo.Table", "Integrations",
                "Filter 10,000+ parsed rows before commit with Klyroo.Query and refresh your Klyroo.Table catalog immediately after bulk import completion.",
                [
                    new("query-filter-rows", "1. Klyroo.Spreadsheet + Klyroo.Query (Search & Filter Parsed Rows)",
                        CodeSamples:
                        [
                            new("QueryImportedRows.cs", """
                                using Klyroo.Query;
                                using Klyroo.Spreadsheet;

                                var rowSchema = DataListSchema.For<SpreadsheetRow>(s => s
                                    .RowKey(r => r.Index)
                                    .Field(r => r.Status, f => f.Key("status").Sortable().Filter(DataListFilter.Option()))
                                    .Field(r => r.Mode, f => f.Key("mode").Filter(DataListFilter.Option())));

                                var blockingOnly = rowSchema.ExecuteInMemory(
                                    importer.AllRows,
                                    new DataListRequest(
                                        paging: DataListPaging.Offset.ForPage(1, 50),
                                        sort: [],
                                        filter: DataListFilterGroup.And(
                                            new FilterRule("status", FilterOperator.Is, new FilterValue.Text("Blocking")))));
                                """)
                        ]),
                    new("table-refresh", "2. Klyroo.Spreadsheet + Klyroo.Table (Import Modal -> Reload DataList)",
                        CodeSamples:
                        [
                            new("ImportIntoTable.razor", """
                                <KlyModal @bind-Open="_showImport" Title="Import Rates CSV">
                                    <KlySpreadsheetImport Importer="@_importer" Wizard="true" />
                                </KlyModal>

                                <KlyDataList @ref="_tableRef" TItem="ProductRate" Schema="@RateSchema" Provider="@LoadRatesAsync">
                                    <KlyDataListTable TItem="ProductRate" ShowProgressLine="true" />
                                </KlyDataList>
                                """)
                        ])
                ]),
            new("integrations-agent-display", "Integration: Klyroo.Agent & Klyroo.Display", "Integrations",
                "Let AI copilots inspect and repair spreadsheet import errors via Klyroo.Agent (RegisterSpreadsheet) and preview exported CSVs in Klyroo.Display lightboxes.",
                [
                    new("agent-spreadsheet", "1. Klyroo.Spreadsheet + Klyroo.Agent (AI-Assisted Error Repair)",
                        Paragraphs: ["Call agent.RegisterSpreadsheet(importer) to expose editCell(rowIndex, fieldKey, text), nextStep, previousStep, submit, and clearFile to AgentBridgeService with full AllowPolicy confirmation gating on submit."],
                        CodeSamples:
                        [
                            new("AgentSpreadsheet.cs", """
                                using Klyroo.Agent;
                                using Klyroo.Spreadsheet;

                                agent.RegisterSpreadsheet(ratesImporter);

                                // AI assistant repairs an invalid cell and advances the wizard
                                await agent.ActAsync([
                                    new AgentActStep("rates", "editCell", [0, "price", "49,90"]),
                                    new AgentActStep("rates", "nextStep")
                                ]);
                                """)
                        ]),
                    new("display-preview", "2. Klyroo.Spreadsheet + Klyroo.Display (Preview Template or Invalid CSV)",
                        CodeSamples:
                        [
                            new("PreviewExportedCsv.cs", """
                                filePreview.Open(new FilePreviewFile
                                {
                                    Name = "invalid-rows.csv",
                                    Mime = "text/csv",
                                    Content = importer.ExportInvalidRowsCsv()
                                });
                                """)
                        ])
                ]),
            new("integrations-ef-maui", "Integration: EF Core Batch Upsert & .NET MAUI", "Integrations",
                "Persist validated batches idempotently with EF Core returning per-row SpreadsheetRejectedRow diagnostics, and feed native MAUI FilePicker streams directly into LoadText.",
                [
                    new("ef-upsert", "1. EF Core Chunked Upsert Handler Returning SpreadsheetRejectedRow",
                        CodeSamples:
                        [
                            new("EfBatchUpsert.cs", """
                                OnSubmit = async batch =>
                                {
                                    var rejected = new List<SpreadsheetRejectedRow>();
                                    foreach (var row in batch)
                                    {
                                        var code = row.Data["code"]!.ToString()!;
                                        if (await db.Products.AnyAsync(p => p.Code == code && p.Locked))
                                        {
                                            rejected.Add(new SpreadsheetRejectedRow(row.Index, "SKU is locked in ERP", "code"));
                                            continue;
                                        }
                                        await UpsertProductAsync(db, row.Data);
                                    }
                                    await db.SaveChangesAsync();
                                    return rejected;
                                }
                                """)
                        ]),
                    new("maui-picker", "2. .NET MAUI FilePicker -> SpreadsheetImporter.LoadText",
                        CodeSamples:
                        [
                            new("MauiCsvPicker.cs", """
                                var file = await FilePicker.Default.PickAsync();
                                if (file is not null)
                                {
                                    using var reader = new StreamReader(await file.OpenReadAsync());
                                    importer.LoadText(await reader.ReadToEndAsync(), file.FileName);
                                }
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: PLATFORMS
            // =========================================================================
            MauiAndAotPage("Klyroo.Spreadsheet", "Klyroo.Spreadsheet.Core", "Klyroo.Spreadsheet", "builder.Services.AddKlyrooSpreadsheet();",
                "var importer = new SpreadsheetImporter(schema); importer.LoadText(csvText, \"rates.csv\");"),

            // =========================================================================
            // GROUP 5: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive spreadsheet import playgrounds covering the full 5-step wizard, unknown value & duplicate reconciliation, Unicode NFD normalization, inline cell editing & invalid CSV export, and chunked batch submit with targeted retry.",
                [
                    new("import-wizard", "1. Full 5-Step KlySpreadsheetImport Wizard",
                        Paragraphs: ["Walk through Source/Layout detection, Column Mapping, Unknown Values, Review & Inline Editing, and Batch Submit with preloaded CSV data."],
                        LiveDemoKey: "spreadsheet-wizard",
                        LiveDemoTitle: "KlySpreadsheetImport · 5-Step Import Wizard",
                        CodeSamples:
                        [
                            new("RatesImporter.razor", """
                                <KlySpreadsheetImport Importer="@_importer" Title="Import Product Rates" Wizard="true" />
                                """)
                        ]),
                    new("reference-reconcile", "2. Unknown Value Questions, Duplicates & ExistingRecords Upsert",
                        Paragraphs: ["Answer an open unknown Select question ('C1-Advanced' -> 'B2'), fix a duplicate email row inline, and watch ExistingRecords mark Row 2 as Mode = Update."],
                        LiveDemoKey: "spreadsheet-reconcile",
                        LiveDemoTitle: "SpreadsheetImporter · Unknown Values, Duplicates & Upsert",
                        CodeSamples:
                        [
                            new("ReconcileLookup.cs", """
                                new SpreadsheetColumnSchema
                                {
                                    Key = "level", Label = "Level", Kind = SpreadsheetColumnKind.Select,
                                    UnknownPolicy = SpreadsheetUnknownValuePolicy.Ask,
                                    Options = [new SpreadsheetOption("B2", "B2", ["b2", "upper-intermediate"])]
                                };
                                importer.AnswerValue("level", "C1-Advanced", new SpreadsheetValueAnswer(SpreadsheetValueAnswerAction.Map, "B2", "B2"));
                                """)
                        ]),
                    new("unicode-nfd", "3. Accent-Insensitive (Unicode NFD) Normalization & Template Export",
                        Paragraphs: ["Type any messy localized header (e.g. 'Prénom * (required)' or 'Référence Produit') to inspect SpreadsheetImporter.NormalizeText() live alongside ExportTemplateCsv()."],
                        LiveDemoKey: "spreadsheet-nfd",
                        LiveDemoTitle: "Unicode NFD · NormalizeText & ExportTemplateCsv()",
                        CodeSamples:
                        [
                            new("NfdMapping.cs", """
                                string normalized = SpreadsheetImporter.NormalizeText("Prénom * (required)"); // "prenom"
                                string template   = importer.ExportTemplateCsv();
                                """)
                        ]),
                    new("inline-edit-export", "4. Inline EditCell, RevertCell, DiscardRows & ExportInvalidRowsCsv",
                        Paragraphs: ["Repair an invalid negative price cell with EditCell(), revert it with RevertCell(), discard/restore rows, and inspect the live ExportInvalidRowsCsv() output."],
                        LiveDemoKey: "spreadsheet-edit-retry",
                        LiveDemoTitle: "EditCell, RevertCell, DiscardRows & Invalid CSV Export",
                        CodeSamples:
                        [
                            new("InlineEdit.cs", """
                                importer.EditCell(rowIndex: 0, fieldKey: "price", rawText: "49,90");
                                importer.RevertCell(rowIndex: 0, fieldKey: "price");
                                importer.DiscardRows([1]);
                                string invalidCsv = importer.ExportInvalidRowsCsv();
                                """)
                        ]),
                    new("batch-retry", "5. Chunked SubmitAsync() & Targeted RetryRejectedAsync()",
                        Paragraphs: ["Run SubmitAsync() against a simulated server that rejects Row 1 ('Duplicate SKU'), observe already-succeeded rows stay imported, then repair Row 1 and call RetryRejectedAsync()."],
                        LiveDemoKey: "spreadsheet-batch-submit",
                        LiveDemoTitle: "Chunked SubmitAsync() & RetryRejectedAsync()",
                        CodeSamples:
                        [
                            new("BatchRetry.cs", """
                                await importer.SubmitAsync();
                                if (importer.RejectedCount > 0)
                                {
                                    importer.EditCell(rowIndex: 1, fieldKey: "code", rawText: "SKU-200-FIXED");
                                    await importer.RetryRejectedAsync();
                                }
                                """)
                        ])
                ]),
            new("api", "API Reference", "Reference",
                "Complete public API reference for Klyroo.Spreadsheet.Core and Klyroo.Spreadsheet.",
                [],
                ApiTypes: [typeof(KlySpreadsheetImport), typeof(SpreadsheetImporter), typeof(SpreadsheetSchema), typeof(SpreadsheetColumnSchema), typeof(SpreadsheetRule), typeof(SpreadsheetRow)])
        ]);
}
