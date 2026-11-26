using Klyroo.Agent;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class KlyrooLibraryDocs
{
    private static LibraryDocTree BuildAgent() => new(
        "agent", "Klyroo.Agent", "#6366f1",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "Semantic AI Agent Bridge (snapshot, query, act, observe), a browser runtime (klyroo-agent.js), WebMCP tool registration (navigator.modelContext), automatic [redacted] secret masking, permission policies, and bounded UndoStack<T>.",
                [
                    new("overview", "What is Klyroo.Agent?",
                        Paragraphs:
                        [
                            "Classic UI automation scrapes fragile CSS selectors, burns tokens on raw DOM/screenshots, cannot inspect closed dropdown options without clicking popups, and risks leaking password inputs into LLM transcripts.",
                            "Klyroo.Agent provides a deterministic semantic bridge for .NET, Blazor, and .NET MAUI: controllers register their controls once and expose three concise tools (snapshot, query, act) plus a unified Observe change stream with guaranteed [redacted] secret masking, explicit AllowPolicy permission gates, and bounded UndoStack<T> time-travel."
                        ],
                        LiveDemoKey: "agent-bridge",
                        LiveDemoTitle: "Klyroo.Agent · Live Semantic Snapshot, Query & ActAsync"),
                    new("pipeline", "How Klyroo.Agent Works End-to-End (The 5-Step Agent Loop)",
                        TableHeaders: ["Step", "API / Mechanism", "What Happens"],
                        TableRows:
                        [
                            new("1. Register", "RegisterForm · RegisterDashboard · RegisterSpreadsheet", "Binds headless Klyroo controllers to AgentBridgeService with stable refs (c1, c2...) and confirmSubmit flags. A form is any IFormRuntime: a FormController<TModel> or a DynamicFormController; a spreadsheet is any SpreadsheetImporter, typed or not."),
                            new("2. Snapshot", "agent.Snapshot(includeOffPage: false)", "Emits up to 100 concise AgentComponentSnapshot records; Password & OneTimeCode values are ALWAYS replaced with \"[redacted]\", and the model cannot change those fields."),
                            new("3. Query", "agent.Query(new AgentQueryRequest { Within, Text, Kind })", "Searches labels and closed select/option choices accent-insensitively ('region' matches 'Région') without opening UI popups."),
                            new("4. Policy Gate & Act", "agent.AllowPolicy + await agent.ActAsync(steps)", "Resolves targets by Ref or normalized Label, enforces AllowPolicy on consequential steps, executes actions sequentially, or halts with a clear diagnostic error."),
                            new("5. Observe & Undo", "agent.Observe(cb) + UndoStack<T>", "Broadcasts ObservedChange events tagged with AgentChangeSource (Agent, Keyboard, Programmatic...) and supports instant Undo/Redo.")
                        ]),
                    new("layers", "Two-Layer Package Split",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Klyroo.Agent.Core", "AgentComponentSnapshot, AgentValue, AgentArgument, AgentQueryRequest, AgentQueryMatch, AgentRecord, AgentChoiceItem, AgentActStep, AgentActResult, AgentToolResult, ObservedChange, AgentToolDefinition, AgentChangeSource, UndoStack<T>", "Zero dependencies · IsAotCompatible=true"),
                            new("Klyroo.Agent", "AgentBridgeService, AddKlyrooAgent(), _content/Klyroo.Agent/klyroo-agent.js", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Klyroo.Agent, register AgentBridgeService in DI, and optionally load the static browser ES modules for WebMCP.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Klyroo.Agent.Core   # Zero-dependency NativeAOT models & UndoStack<T>
                                dotnet add package Klyroo.Agent        # AgentBridgeService + WebMCP & Behaviors JS assets
                                """)
                        ]),
                    new("di", "2. Register Services & Imports",
                        CodeSamples:
                        [
                            new("Program.cs / MauiProgram.cs", "builder.Services.AddKlyrooAgent();"),
                            new("_Imports.razor", "@using Klyroo.Agent")
                        ]),
                    new("assets", "3. Static Web Assets (Optional Browser Bridge)",
                        Paragraphs: ["Klyroo.Agent serves a dependency-free ES module at _content/Klyroo.Agent/klyroo-agent.js for browser-side snapshot, query and act, and WebMCP registration (navigator.modelContext). Custom widgets defined with Klyroo.definePrimitive from the Klyroo.UI script are discovered through their data-kly-primitive attribute."])
                ]),

            // =========================================================================
            // GROUP 2: SEMANTIC BRIDGE & SAFETY
            // =========================================================================
            new("semantic-tools", "Snapshot, Query, Act & Diagnostics", "Semantic Bridge",
                "Expose three standard MCP / WebMCP tools via AgentBridgeService.GetToolDefinitions() with accent-insensitive label resolution and deterministic error diagnostics.",
                [
                    new("tools", "Tool Contracts & Safety Hints",
                        TableHeaders: ["Tool", "Safety Hints", "Behavior & Limits"],
                        TableRows:
                        [
                            new("snapshot", "ReadOnlyHint=true, ConsequentialHint=false, UntrustedContentHint=true", "Returns up to 100 AgentComponentSnapshot entries (Ref, Component, Label, Value, Actions, Disabled, OffPage, ConfirmRequired)"),
                            new("query", "ReadOnlyHint=true, ConsequentialHint=false, UntrustedContentHint=true", "Finds controls accent-insensitively by Within, Text, or Kind and lists up to 50 AgentChoiceItem entries (+ More overflow count) without opening popups"),
                            new("act", "ReadOnlyHint=false, ConsequentialHint=true, UntrustedContentHint=true", "Executes AgentActStep[] sequentially by Ref (c1) or normalized Label; halts with clear diagnostic on not-found, ambiguity, disabled, unsupported action, or policy refusal")
                        ],
                        LiveDemoKey: "agent-tools-diagnostics",
                        LiveDemoTitle: "Interactive Tool Definitions & Deterministic Failure Diagnostics",
                        CodeSamples:
                        [
                            new("SemanticLoop.cs", """
                                IReadOnlyList<AgentToolDefinition> tools = AgentBridgeService.GetToolDefinitions();

                                var snap    = agent.Snapshot(includeOffPage: false);
                                var matches = agent.Query(new AgentQueryRequest { Within = "region", Text = "Europe" });

                                AgentActResult res = await agent.ActAsync([
                                    new AgentActStep("project name", "setValue", ["Atlas"]),
                                    new AgentActStep("region", "setValue", ["eu-west"])
                                ]);
                                """)
                        ])
                ]),
            new("policies-and-observe", "AllowPolicy, Observe Stream & Secret Redaction", "Semantic Bridge",
                "Gate consequential actions globally or per call, stream ObservedChange events with provenance, and guarantee [redacted] secret masking.",
                [
                    new("gate", "Global AllowPolicy & Per-Call allow Override",
                        Paragraphs: ["Before executing every AgentActStep, AgentBridgeService awaits both agent.AllowPolicy (if set) and any per-call allow delegate passed to ActAsync(steps, allow), receiving (AgentActStep step, bool confirmRequired)."],
                        LiveDemoKey: "agent-policy",
                        LiveDemoTitle: "Interactive AllowPolicy Confirmation Gate",
                        CodeSamples:
                        [
                            new("AllowPolicy.cs", """
                                agent.AllowPolicy = async (step, confirmRequired) =>
                                {
                                    if (!confirmRequired) return true;
                                    return await confirmDialog.AskAsync($"Allow AI assistant to run '{step.Action}' on '{step.Ref}'?");
                                };

                                // Per-call dry-run policy:
                                var dryRun = await agent.ActAsync(steps, allow: (step, _) => Task.FromResult(false));
                                """)
                        ]),
                    new("sources", "All 7 AgentChangeSource Values & Automatic [redacted] Masking",
                        TableHeaders: ["AgentChangeSource", "Origin", "Usage"],
                        TableRows:
                        [
                            new("Keyboard / Mouse / Touch / Pen", "Direct human input device", "Distinguish user edits from automated mutations"),
                            new("Virtual", "Assistive technology / screen reader", "Preserves accessibility provenance"),
                            new("Programmatic", "Application C# or JS code", "Identifies state changes triggered by app logic"),
                            new("Agent", "AgentBridgeService.ActAsync()", "Automatically tagged on every agent-driven step")
                        ],
                        Callout: "Password and OneTimeCode fields ALWAYS emit AgentBridgeService.RedactedValue (\"[redacted]\") in Snapshot(), ActResult.Values, and ObservedChange — plaintext secrets are never exposed to observers or LLMs. ActAsync also refuses every write to a sensitive field unless the application names it in RegisterForm(form, allowSensitiveWrites: [\"Code\"]), and refuses a write to a field of another step of a wizard (move there with the nextStep action first).")
                ]),
            new("undo-and-webmcp", "UndoStack<T>, WebMCP & DOM Behaviors", "State & Browser",
                "Zero-dependency generic UndoStack<T> in Klyroo.Agent.Core, plus browser WebMCP registration. The headless DOM behaviors ship in the Klyroo.UI script as Klyroo.behaviors.",
                [
                    new("undo", "Bounded Generic UndoStack<T>",
                        LiveDemoKey: "agent-undo",
                        LiveDemoTitle: "Interactive UndoStack<T> · Record, Undo, Redo & Clear",
                        CodeSamples:
                        [
                            new("UndoHistory.cs", """
                                var history = new UndoStack<string>("Initial", limit: 50);
                                history.Record("Draft v2");
                                history.Record("Published v3");
                                var prev = history.Undo(); // "Draft v2"
                                var next = history.Redo(); // "Published v3"
                                """)
                        ]),
                    new("behaviors", "Klyroo.behaviors (Klyroo.UI script) & WebMCP",
                        TableHeaders: ["Exported Function", "Module", "Purpose"],
                        TableRows:
                        [
                            new("announce / createUndoStack", "Klyroo.UI script", "Screen-reader live announcements & browser-side undo/redo stack"),
                            new("createRovingFocus / createGridFocus / createTypeahead", "Klyroo.UI script", "Toolbar/menu arrow roving tabindex, 2D data-grid arrow focus, buffered typeahead"),
                            new("createSelection / createPopup / createReorder / createResize", "Klyroo.UI script", "Single/multi selection, positioned popups, drag/keyboard reorder, pane/column resize"),
                            new("createViewport / createInlineEdit / definePrimitive", "Klyroo.UI script", "Zoomable canvas, inline cell editing, and declarative custom-element composition"),
                            new("createAgent / registerWebMcp / observe", "klyroo-agent.js", "Registers snapshot, query, and act directly on browser navigator.modelContext")
                        ],
                        CodeSamples:
                        [
                            new("WebMcpSetup.js", """
                                import { createAgent, registerWebMcp, observe } from './_content/Klyroo.Agent/klyroo-agent.js';

                                const agent = createAgent({ allow: (step, confirm) => !confirm || window.confirm(`Run ${step.action}?`) });
                                registerWebMcp(agent); // Exposes snapshot, query, act on navigator.modelContext
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 3: INTEGRATIONS WITH OTHER KLYROO LIBRARIES
            // =========================================================================
            new("integrations-controllers", "Integration: Klyroo.Form, Klyroo.Dashboard & Klyroo.Spreadsheet", "Integrations",
                "Register multiple Klyroo controllers onto one AgentBridgeService so a single AI agent plan can fill forms, switch dashboard views, and repair spreadsheet cells atomically.",
                [
                    new("matrix", "Exact Components & Actions Exposed per Klyroo Controller",
                        TableHeaders: ["Registration Call", "Exposed Component Kinds", "Supported Semantic Actions"],
                        TableRows:
                        [
                            new("agent.RegisterForm(form, confirmSubmit: true)", "input, select, checkbox, number, form", "Fields: setValue, clear, toggle, reset · Form root: submit, reset, nextStep, previousStep"),
                            new("agent.RegisterDashboard(dashboard)", "select (non-headless filters), dashboard", "Filters: setValue, toggle, clear, reset · Dashboard root: setView, resetFilters, refresh"),
                            new("agent.RegisterSpreadsheet(importer)", "spreadsheet", "editCell(rowIndex, fieldKey, text), nextStep, previousStep, submit, clearFile")
                        ],
                        LiveDemoKey: "agent-multi-controller",
                        LiveDemoTitle: "Interactive Cross-Controller Agent Plan (Klyroo.Form + Klyroo.Dashboard)",
                        CodeSamples:
                        [
                            new("MultiControllerAgent.cs", """
                                using Klyroo.Agent;
                                using Klyroo.Form;
                                using Klyroo.Dashboard;
                                using Klyroo.Spreadsheet;

                                agent.RegisterForm(campaignForm, confirmSubmit: true);
                                agent.RegisterDashboard(revenueDashboard);
                                agent.RegisterSpreadsheet(ratesImporter);

                                // One atomic plan spanning Form + Dashboard + Spreadsheet
                                var result = await agent.ActAsync([
                                    new AgentActStep("Campaign Title", "setValue", ["Enterprise Launch 2026"]),
                                    new AgentActStep("revenue", "setView", ["cohorts"]),
                                    new AgentActStep("rates", "editCell", [0, "price", "49,90"])
                                ]);
                                """)
                        ])
                ]),
            new("integrations-query-ui", "Integration: Klyroo.Query, Klyroo.Table & Klyroo.UI Confirmation Modals", "Integrations",
                "Pair Klyroo.Agent AllowPolicy with Klyroo.UI KlyModal human-in-the-loop confirmations and trigger URL-synced Klyroo.Query / Klyroo.Table updates safely.",
                [
                    new("modal-confirm", "1. Klyroo.Agent + Klyroo.UI (Human-in-the-Loop KlyModal Approval Gate)",
                        CodeSamples:
                        [
                            new("AgentApprovalHost.razor", """
                                @inject AgentBridgeService Agent

                                <KlyModal @bind-Open="_askOpen" Title="Approve AI Action?">
                                    <p>Allow assistant to run <code>@_pendingStep?.Action</code> on <strong>@_pendingStep?.Ref</strong>?</p>
                                    <div class="mt-4 flex justify-end gap-2">
                                        <KlyButton Label="Reject" Variant="ButtonVariant.Outline" OnClick="@(() => Resolve(false))" />
                                        <KlyButton Label="Approve" Color="KlyColor.Primary" OnClick="@(() => Resolve(true))" />
                                    </div>
                                </KlyModal>

                                @code {
                                    private bool _askOpen;
                                    private AgentActStep? _pendingStep;
                                    private TaskCompletionSource<bool>? _tcs;

                                    protected override void OnInitialized() => Agent.AllowPolicy = async (step, confirmRequired) =>
                                    {
                                        if (!confirmRequired) return true;
                                        _pendingStep = step; _askOpen = true; _tcs = new();
                                        await InvokeAsync(StateHasChanged);
                                        return await _tcs.Task;
                                    };

                                    private void Resolve(bool ok) { _askOpen = false; _tcs?.TrySetResult(ok); }
                                }
                                """)
                        ]),
                    new("query-sync", "2. Klyroo.Agent + Klyroo.Query / Klyroo.Table (Safe Schema-Sanitized URL Updates)",
                        Paragraphs: ["When an AI assistant filters a table via QueryState + DataListStateQuery, every key and operator is validated by DataListSchema.Sanitize(), guaranteeing the agent can never inject invalid filter keys or oversized page requests."])
                ]),
            new("integrations-llm-mcp", "Integration: LLM Tool Calling & MCP Servers", "Integrations",
                "Connect AgentBridgeService.GetToolDefinitions() directly to Microsoft.Extensions.AI, OpenAI/Anthropic/Gemini function calling, or ModelContextProtocol servers.",
                [
                    new("mcp-wire", "Wiring snapshot, query & act Into an AI Tool Dispatcher",
                        CodeSamples:
                        [
                            new("LlmToolDispatcher.cs", """
                                using System.Text.Json;
                                using Klyroo.Agent;

                                // Dispatch incoming LLM tool calls directly to the scoped AgentBridgeService
                                public async Task<object> DispatchToolAsync(string toolName, JsonElement args) => toolName switch
                                {
                                    "snapshot" => agent.Snapshot(includeOffPage: false),
                                    "query" => agent.Query(new AgentQueryRequest
                                    {
                                        Within = args.TryGetProperty("within", out var w) ? w.GetString() : null,
                                        Text   = args.TryGetProperty("text", out var t) ? t.GetString() : null,
                                        Kind   = args.TryGetProperty("kind", out var k) ? k.GetString() : null
                                    }),
                                    "act" => await agent.ActAsync(JsonSerializer.Deserialize<AgentActStep[]>(args.GetProperty("steps"))!),
                                    _ => throw new InvalidOperationException($"Unknown tool: {toolName}")
                                };
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: PLATFORMS & TESTING
            // =========================================================================
            MauiAndAotPage("Klyroo.Agent", "Klyroo.Agent.Core", "Klyroo.Agent", "builder.Services.AddKlyrooAgent();",
                "var history = new UndoStack<DocumentState>(initial, limit: 50);"),
            new("recipes", "Testing & Recipes", "Platforms",
                "Verify secret redaction, accent-insensitive queries, ambiguity rejection, and policy gates in plain xUnit.",
                [
                    new("xunit", "Complete xUnit Verification Suite",
                        CodeSamples:
                        [
                            new("AgentTests.cs", """
                                var services = new ServiceCollection();
                                services.AddKlyrooAgent();
                                using var provider = services.BuildServiceProvider();
                                var agent = provider.GetRequiredService<AgentBridgeService>();

                                agent.RegisterForm(form, confirmSubmit: true);
                                agent.RegisterDashboard(dashboard);

                                Assert.Equal(AgentBridgeService.RedactedValue, agent.Snapshot().First(s => s.Label == "API Secret").Value);
                                Assert.Equal("eu-west", agent.Query(new AgentQueryRequest { Within = "region", Text = "Europe" })[0].Items[0].Value);

                                agent.AllowPolicy = (_, confirmRequired) => Task.FromResult(!confirmRequired);
                                Assert.False((await agent.ActAsync([new AgentActStep("Project Settings", "submit")])).Ok);
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 5: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive Klyroo.Agent sandboxes demonstrating Snapshot/Query/Act with [redacted] secrets, AllowPolicy safety gates, UndoStack<T> time-travel, cross-controller plans (Form + Dashboard), and deterministic error diagnostics.",
                [
                    new("bridge-sandbox", "1. Live Snapshot, Accent-Insensitive Query & Act Pipeline",
                        Paragraphs: ["Run ActAsync to mutate the Project field, run accent-insensitive Query('region', 'Europe') without opening popups, and verify Password is always \"[redacted]\"."],
                        LiveDemoKey: "agent-bridge",
                        LiveDemoTitle: "AgentBridgeService · Snapshot, Query & Act",
                        CodeSamples:
                        [
                            new("AgentBridgeExample.cs", """
                                var snapshot = agent.Snapshot();
                                var matches  = agent.Query(new AgentQueryRequest { Within = "region", Text = "Europe" });

                                var result = await agent.ActAsync([
                                    new AgentActStep("Project", "setValue", ["Atlas"])
                                ]);
                                """)
                        ]),
                    new("policy-gate", "2. Consequential Action Policy Gate (AllowPolicy)",
                        Paragraphs: ["Toggle AllowPolicy between blocking and approving confirmRequired actions, then execute ActAsync([submit])."],
                        LiveDemoKey: "agent-policy",
                        LiveDemoTitle: "AgentBridgeService.AllowPolicy · Safety Gate",
                        CodeSamples:
                        [
                            new("PolicyGate.cs", """
                                var outcome = await agent.ActAsync(
                                    [new AgentActStep("Project Settings", "submit")],
                                    allow: (step, confirmRequired) => Task.FromResult(!confirmRequired || userApproved));
                                """)
                        ]),
                    new("undo-stack", "3. Immutable UndoStack<T> State Time-Travel",
                        Paragraphs: ["Record snapshots into UndoStack<T>, step backward with Undo(), forward with Redo(), and clear history with Clear()."],
                        LiveDemoKey: "agent-undo",
                        LiveDemoTitle: "UndoStack<T> · Record, Undo, Redo & Clear",
                        CodeSamples:
                        [
                            new("UndoHistory.cs", """
                                var history = new UndoStack<string>("Initial", limit: 50);
                                history.Record("Snapshot #1");
                                if (history.CanUndo) history.Undo();
                                if (history.CanRedo) history.Redo();
                                """)
                        ]),
                    new("multi-controller", "4. Cross-Controller Plan (Klyroo.Form + Klyroo.Dashboard Together)",
                        Paragraphs: ["Register both a FormController<TModel> and a DashboardController on one AgentBridgeService and mutate both in a single ActAsync() batch."],
                        LiveDemoKey: "agent-multi-controller",
                        LiveDemoTitle: "Unified Multi-Controller Snapshot & ActAsync",
                        CodeSamples:
                        [
                            new("MultiController.cs", """
                                agent.RegisterForm(campaignForm, confirmSubmit: false);
                                agent.RegisterDashboard(revenueDashboard);

                                await agent.ActAsync([
                                    new AgentActStep("Campaign Title", "setValue", ["Enterprise Launch 2026"]),
                                    new AgentActStep("multi-dash", "setView", ["cohorts"])
                                ]);
                                """)
                        ]),
                    new("tools-diagnostics", "5. GetToolDefinitions() & Deterministic Failure Diagnostics",
                        Paragraphs: ["Inspect the 3 MCP tool definitions (snapshot, query, act) and trigger Not Found, Unsupported Action, and Policy Refusal errors to see the exact self-correction diagnostics returned to LLMs."],
                        LiveDemoKey: "agent-tools-diagnostics",
                        LiveDemoTitle: "GetToolDefinitions() & AgentActResult.Error Diagnostics",
                        CodeSamples:
                        [
                            new("Diagnostics.cs", """
                                var tools = AgentBridgeService.GetToolDefinitions();
                                var err   = await agent.ActAsync([new AgentActStep("unknown-control", "setValue", ["x"])]);
                                // err.Ok == false, err.Error == "Step 1: component \"unknown-control\" not found."
                                """)
                        ])
                ]),
            new("api", "API Reference", "Reference",
                "Complete public API reference for Klyroo.Agent.Core and Klyroo.Agent.",
                [],
                ApiTypes: [typeof(AgentBridgeService), typeof(AgentComponentSnapshot), typeof(AgentQueryRequest), typeof(AgentQueryMatch), typeof(AgentChoiceItem), typeof(AgentActStep), typeof(AgentActResult), typeof(ObservedChange), typeof(AgentToolDefinition), typeof(UndoStack<object>)])
        ]);
}
