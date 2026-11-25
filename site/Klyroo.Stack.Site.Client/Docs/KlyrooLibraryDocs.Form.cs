using Klyroo.Form;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class KlyrooLibraryDocs
{
    private static LibraryDocTree BuildForm() => new(
        "form", "Klyroo.Form", "#f59e0b",
        [
            // =========================================================================
            // GROUP 1: GETTING STARTED
            // =========================================================================
            new("", "Introduction & How It Works", "Getting Started",
                "Declarative, NativeAOT schema-driven form engine with 31 field kinds, reactive visibility/disabled/required rules, dirty path diffing, input/output transforms, repeatable arrays, multi-step wizards with SkipWhen, sectioned FormPage, and programmatic FormService overlays.",
                [
                    new("overview", "What is Klyroo.Form?",
                        Paragraphs:
                        [
                            "Klyroo.Form replaces hand-coded Razor form markup and fragile validation state with a declarative FormSchema and a 100% headless, NativeAOT FormController. Every rule — conditional visibility, dynamic required/disabled predicates, watch side-effects, password checklists, array row mutations, dirty diffing, server error injection, and wizard step skipping — executes in plain C# with zero UI dependencies.",
                            "Render any FormController inline with <KlyFormEngine>, organize long settings pages with <KlyFormPage>, or launch Modal, Drawer, and Fullscreen forms imperatively from anywhere via FormService.CreateFormAsync()."
                        ],
                        LiveDemoKey: "form-reactive",
                        LiveDemoTitle: "Klyroo.Form · Live Reactive Condition, Password Rules & Dirty Tracking"),
                    new("pipeline", "How Klyroo.Form Works End-to-End (The 5-Stage Lifecycle)",
                        TableHeaders: ["Stage", "API / Type", "What Happens"],
                        TableRows:
                        [
                            new("1. Define Schema", "FormSchema · FormFieldSchema · FormStepSchema · FormPageSectionSchema", "Declares fields (31 FormFieldKind values), defaults, transforms, conditions, validation triggers, steps, or page sections."),
                            new("2. Hydrate & Baseline", "new FormController(schema, initialInput)", "Applies DefaultValue/DefaultFactory and TransformInput, then captures an immutable baseline snapshot for IsDirty and DirtyPaths."),
                            new("3. Reactive Edits", "SetValue · AddArrayItem · Watch · Condition", "Re-evaluates Condition, RequiredWhen, DisabledWhen, and OptionsFactory live; hidden fields bypass validation and drop from output."),
                            new("4. Validate & Step", "Validate() · NextStepAsync() · SetError(blocking: true)", "Runs per-field/step validators, skips steps whose SkipWhen is true, invokes OnBeforeNext, and surfaces client or server blocking errors."),
                            new("5. Transform & Submit", "SubmitAsync() · BuildOutput() · FormSubmitOutcome", "Runs TransformOutput, omits OmitFromOutput & hidden fields, re-baselines IsDirty to false, and returns the clean output dictionary.")
                        ]),
                    new("layers", "Two-Layer Architecture",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Klyroo.Form.Core", "FormSchema, FormFieldSchema, FormStepSchema, FormPageSectionSchema, FormController, FormService, FormSubmitOutcome", "Headless C# · IsAotCompatible=true"),
                            new("Klyroo.Form", "KlyFormEngine, KlyFormPage, KlyFormProvider, AddKlyrooForm()", "browser, ios, android, maccatalyst, windows")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Klyroo.Form, register FormService, and mount KlyFormProvider once for programmatic Modal, Drawer, and Fullscreen overlays.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Klyroo.Form.Core   # Headless NativeAOT schema & controller
                                dotnet add package Klyroo.Form        # Blazor Web & .NET MAUI form components
                                """)
                        ]),
                    new("di", "2. Register Services & Mount Overlay Provider",
                        Paragraphs: ["AddKlyrooForm() registers FormService as a scoped service. Mount <KlyFormProvider /> once in your root layout so FormService.CreateFormAsync() can open Modal, Drawer, or Fullscreen forms from any page or service."],
                        CodeSamples:
                        [
                            new("Program.cs / MauiProgram.cs", "builder.Services.AddKlyrooForm();"),
                            new("MainLayout.razor", "<KlyFormProvider />"),
                            new("_Imports.razor", "@using Klyroo.Form")
                        ])
                ]),

            // =========================================================================
            // GROUP 2: SCHEMA & STATE ENGINE
            // =========================================================================
            new("field-kinds", "FormSchema & 31 Field Kinds", "Schema & State",
                "Complete catalog of all 31 declarative FormFieldKind values, layout options, and repeatable ArrayList / ArrayTable rows.",
                [
                    new("all-kinds", "All 31 FormFieldKind Values",
                        TableHeaders: ["Category", "Exact FormFieldKind Values", "Capabilities"],
                        TableRows:
                        [
                            new("Text & Auth (5)", "Text, Password, Textarea, PhoneNumber, OneTimeCode", "MaxLength, Placeholder, live PasswordRequirements checklist, OTP digits"),
                            new("Numeric & Visual (4)", "Number, Slider, Rating, ColorPicker", "Min, Max, Step precision, visual slider/star/swatch pickers"),
                            new("Boolean & Choice (6)", "Checkbox, Switch, Radio, CheckboxGroup, Select, AutoComplete", "Options, dynamic OptionsFactory, Multiple, Clearable, OnCreateOption"),
                            new("Temporal & Tags (5)", "Date, DateTime, DateRange, Time, Tag", "Calendar/time inputs, ranges, and freeform tag chip editors"),
                            new("Stateless Layout (6)", "Hidden, Info, Divider, Section, Button, Group", "DisplayText/DisplayTone callouts, Collapsible/DefaultCollapsed sections (excluded from BuildOutput)"),
                            new("Composite & Nested (5)", "Card, Tabs, Object, ArrayList, ArrayTable", "FormTabItemSchema tabs, nested child Fields, repeatable AddItemLabel rows")
                        ]),
                    new("arrays", "Repeatable Arrays (ArrayList & ArrayTable)",
                        Paragraphs: ["Use FormFieldKind.ArrayList or ArrayTable with child Fields to add, edit, and remove structured rows with live validation and programmatic FormController helpers."],
                        LiveDemoKey: "form-array",
                        LiveDemoTitle: "Interactive FormFieldKind.ArrayTable Line Items",
                        CodeSamples:
                        [
                            new("ArrayFields.cs", """
                                var lineItemsField = new FormFieldSchema
                                {
                                    Key = "items",
                                    Label = "Invoice Line Items",
                                    Kind = FormFieldKind.ArrayTable,
                                    AddItemLabel = "Add line item",
                                    Fields =
                                    [
                                        new FormFieldSchema { Key = "sku", Label = "SKU", Required = true },
                                        new FormFieldSchema { Key = "qty", Label = "Qty", Kind = FormFieldKind.Number, Min = 1, DefaultValue = 1.0 }
                                    ]
                                };

                                controller.AddArrayItem(lineItemsField);
                                controller.SetArrayItemValue("items", index: 0, childKey: "sku", value: "KLY-PRO");
                                var rows = controller.GetArrayItems("items");
                                controller.RemoveArrayItem("items", index: 0);
                                """)
                        ])
                ]),
            new("conditions", "Reactive Conditions & Transforms", "Schema & State",
                "Control visibility, required state, disabled state, dependent options, Watch hooks, and input/output normalization dynamically.",
                [
                    new("reactive", "Condition, RequiredWhen, DisabledWhen & Transforms",
                        Paragraphs:
                        [
                            "FormController evaluates Condition, RequiredWhen, DisabledWhen, and OptionsFactory live against current state. When Condition returns false, the field is automatically hidden, bypassed during Validate(), and excluded from BuildOutput().",
                            "Use TransformInput to normalize external payloads when hydrating and TransformOutput to sanitize final values (trimming, lowercasing, formatting) before persistence."
                        ],
                        LiveDemoKey: "form-reactive",
                        LiveDemoTitle: "Set Age below 21 to watch Company hide and drop from validation",
                        CodeSamples:
                        [
                            new("ReactiveSchema.cs", """
                                var schema = new FormSchema
                                {
                                    FormKey = "profile",
                                    Title = "User Profile",
                                    Fields =
                                    [
                                        new FormFieldSchema
                                        {
                                            Key = "name", Label = "Full Name", Required = true,
                                            TransformOutput = v => v?.ToString()?.Trim(),
                                            Watch = (val, form) => form.SetValue("slug", val?.ToString()?.Trim().ToLowerInvariant().Replace(' ', '-'))
                                        },
                                        new FormFieldSchema { Key = "age", Label = "Age", Kind = FormFieldKind.Number, Min = 18, Max = 120, DefaultValue = 25.0 },
                                        new FormFieldSchema
                                        {
                                            Key = "company", Label = "Company",
                                            Condition = f => f.GetValue<double>("age") >= 21,
                                            RequiredWhen = f => f.GetValue<double>("age") >= 21,
                                            DisabledWhen = f => f.GetValue<bool>("locked"),
                                            DisabledReason = "Locked by workspace administrator"
                                        }
                                    ]
                                };
                                """)
                        ])
                ]),
            new("validation-and-dirty", "Validation, Server Errors & Dirty Diffing", "Schema & State",
                "Combine synchronous rules, password checklists, server-side blocking errors, and baseline dirty tracking.",
                [
                    new("enums", "Validation Modes & Triggers",
                        TableHeaders: ["Enum", "Exact Values", "Behavior"],
                        TableRows:
                        [
                            new("FormValidationMode", "All, RequiredOnly, ValidatorsOnly, None", "Controls which validation passes run on FormSchema.ValidateMode"),
                            new("FormValidationTrigger", "Blur, Input, Submit", "Per-field trigger deciding when validation feedback surfaces")
                        ]),
                    new("dirty", "IsDirty, DirtyPaths, ResetField & Submit Re-Baselining",
                        Paragraphs: ["FormController compares current state against the baseline snapshot to compute IsDirty and DirtyPaths. Calling SubmitAsync() automatically re-baselines on success so IsDirty resets to false."],
                        LiveDemoKey: "form-dirty-diff",
                        LiveDemoTitle: "Interactive Dirty Tracking, ResetField & BuildOutput()",
                        CodeSamples:
                        [
                            new("DirtyAndServerErrors.cs", """
                                // Track exact modified paths against initial snapshot
                                controller.SetValue("name", "Claire", markTouched: true);
                                Assert.True(controller.IsDirty);
                                Assert.Contains("name", controller.DirtyPaths);

                                // Attach server-side validation error returned from an API (409 Conflict / 422)
                                controller.SetError("email", "Email already registered", blocking: true);
                                Assert.True(controller.HasBlockingErrors);

                                // Revert one field or clear error
                                controller.ResetField("name");
                                controller.ClearError("email");
                                """)
                        ])
                ]),
            new("wizards-and-pages", "Wizards, KlyFormPage & FormService", "UI Components",
                "Build stepped wizards with SkipWhen, sectioned settings pages with completion badges, and imperative Modal/Drawer/Fullscreen overlays.",
                [
                    new("wizard", "Multi-Step Wizards with SkipWhen & OnBeforeNext",
                        Paragraphs: ["Populate Steps on FormSchema to enable stepped mode (controller.IsStepped). NextStepAsync() validates only the current step's visible fields, awaits OnBeforeNext, and automatically jumps past steps where SkipWhen returns true."],
                        LiveDemoKey: "form-wizard",
                        LiveDemoTitle: "Interactive Multi-Step Wizard with SkipWhen",
                        CodeSamples:
                        [
                            new("OnboardingWizard.cs", """
                                var wizardSchema = new FormSchema
                                {
                                    FormKey = "onboarding",
                                    Title = "Workspace Onboarding",
                                    ShowStepper = true,
                                    Steps =
                                    [
                                        new FormStepSchema
                                        {
                                            Key = "account", Title = "Account",
                                            Fields = [new FormFieldSchema { Key = "plan", Label = "Plan", Kind = FormFieldKind.Select, DefaultValue = "free", Options = [new("free", "Free"), new("pro", "Pro")] }]
                                        },
                                        new FormStepSchema
                                        {
                                            Key = "billing", Title = "Billing",
                                            SkipWhen = f => f.GetValue<string>("plan") == "free",
                                            Fields = [new FormFieldSchema { Key = "vat", Label = "VAT Number", Required = true }]
                                        }
                                    ]
                                };
                                """)
                        ]),
                    new("sections-service", "KlyFormPage Section Statuses & FormService.CreateFormAsync",
                        Paragraphs: ["Define Sections on FormSchema and render <KlyFormPage Form=\"@_controller\" /> to track Complete, Pending, Invalid, and Optional statuses per section. Use FormService.CreateFormAsync() to open any schema in a Modal, Drawer, or Fullscreen overlay."],
                        LiveDemoKey: "form-sections-errors",
                        LiveDemoTitle: "Interactive FormPageSectionStatus & Server Blocking Errors",
                        CodeSamples:
                        [
                            new("OverlayCall.cs", """
                                FormSubmitOutcome outcome = await formService.CreateFormAsync(
                                    inviteSchema,
                                    input: new Dictionary<string, object?> { ["role"] = "Editor" },
                                    mode: FormShellMode.Drawer);

                                if (outcome.Submitted)
                                    await api.InviteAsync(outcome.Data);
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 3: INTEGRATIONS WITH OTHER KLYROO LIBRARIES
            // =========================================================================
            new("integrations-table-query", "Integration: Klyroo.Table & Klyroo.Query", "Integrations",
                "Use Klyroo.Form for row-level CRUD drawers in Klyroo.Table and for complex advanced-search drawers that emit Klyroo.Query FilterRules into QueryState.",
                [
                    new("table-crud", "1. Klyroo.Form + Klyroo.Table (Create / Edit Drawers)",
                        Paragraphs: ["Launch FormService.CreateFormAsync(schema, initialValues, FormShellMode.Drawer) directly from a Klyroo.Table toolbar 'New' button or row action 'Edit', then call _list.ReloadAsync() on submission."],
                        CodeSamples:
                        [
                            new("TableCrudDrawer.cs", """
                                var createOutcome = await formService.CreateFormAsync(
                                    EmployeeFormSchemas.Create, mode: FormShellMode.Drawer);

                                if (createOutcome.Submitted)
                                {
                                    await employeeApi.CreateAsync(createOutcome.Data);
                                    await _dataList.ReloadAsync();
                                }
                                """)
                        ]),
                    new("query-search", "2. Klyroo.Form + Klyroo.Query (Advanced Filter Drawer -> URL QueryState)",
                        Paragraphs: ["Collect multi-field filter criteria in a validated Klyroo.Form drawer, convert BuildOutput() into FilterRule[] + DataListState, and push them to QueryState via DataListStateQuery.ToQuery."],
                        CodeSamples:
                        [
                            new("FormToQuery.cs", """
                                using Klyroo.Form;
                                using Klyroo.Query;

                                var outcome = await formService.CreateFormAsync(AdvancedSearchSchema, mode: FormShellMode.Modal);
                                if (!outcome.Submitted) return;

                                var rules = new List<FilterRule>();
                                if (outcome.Data.TryGetValue("minSalary", out var min) && min is double d)
                                    rules.Add(new FilterRule("salary", FilterOperator.Gte, new FilterValue.Number((decimal)d)));

                                var state = DataListState.Initial with { Filters = rules };
                                var pairs = DataListStateQuery.ToQuery(state, EmployeeSchemas.Directory, prefix: "emp")
                                    .Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value));
                                queryState.SetMany(pairs, QueryHistory.Push);
                                """)
                        ])
                ]),
            new("integrations-agent-dashboard", "Integration: Klyroo.Agent & Klyroo.Dashboard", "Integrations",
                "Expose any FormController to AI copilots via Klyroo.Agent (with automatic password redaction and submit confirmation gates) and configure Klyroo.Dashboard presets with Klyroo.Form.",
                [
                    new("agent-bridge", "1. Klyroo.Form + Klyroo.Agent (AI Copilot Form Filling & Undo)",
                        Paragraphs: ["Call agent.RegisterForm(controller, confirmSubmit: true) to expose every visible field as semantic snapshot/query/act controls. Password and OneTimeCode fields are automatically redacted to \"[redacted]\", and AllowPolicy gates the final submit step."],
                        CodeSamples:
                        [
                            new("AgentFormBridge.cs", """
                                using Klyroo.Agent;
                                using Klyroo.Form;

                                agent.RegisterForm(onboardingController, confirmSubmit: true);
                                agent.AllowPolicy = async (step, confirmRequired) =>
                                    !confirmRequired || await confirmModal.AskAsync($"Allow AI to run '{step.Action}'?");

                                // AI assistant fills fields & advances wizard steps safely
                                await agent.ActAsync([
                                    new AgentActStep("Organization", "setValue", ["Klyroo Labs"]),
                                    new AgentActStep("Plan", "setValue", ["pro"]),
                                    new AgentActStep("Workspace Onboarding", "nextStep")
                                ]);
                                """)
                        ]),
                    new("dashboard-config", "2. Klyroo.Form + Klyroo.Dashboard (Saved Dashboard Views & Alert Rules)",
                        Paragraphs: ["Use FormService.CreateFormAsync(..., FormShellMode.Modal) from a KlyDashboardPage toolbar action to create custom saved views or configure alert thresholds, then add the new view to DashboardController."])
                ]),
            new("integrations-display-api", "Integration: Klyroo.Display, API Validation & Persistence", "Integrations",
                "Preview form changes with Klyroo.Display Diff cards before submission and map ASP.NET Core ProblemDetails / ValidationProblemDetails errors back onto FormController.",
                [
                    new("diff-review", "1. Klyroo.Form + Klyroo.Display (Pre-Submit Change Diff Review)",
                        CodeSamples:
                        [
                            new("DiffBeforeSave.razor", """
                                @using Klyroo.Form
                                @using Klyroo.Display

                                @* Show a clean side-by-side Klyroo.Display Diff of every DirtyPath before saving *@
                                <KlyDisplay Kind="DisplayKind.Diff"
                                           Title="Pending Changes"
                                           DiffItems="@([.. _form.DirtyPaths.Select(k =>
                                               new DisplayDiffItem(k, _initial.GetValueOrDefault(k)?.ToString(), _form.GetValue<object>(k)?.ToString(), Changed: true))])" />
                                """)
                        ]),
                    new("server-422", "2. Mapping ASP.NET Core 422 ValidationProblemDetails to FormController",
                        CodeSamples:
                        [
                            new("ServerValidationMap.cs", """
                                using var resp = await http.PostAsJsonAsync("/api/accounts", _form.BuildOutput());
                                if (resp.StatusCode == HttpStatusCode.UnprocessableEntity)
                                {
                                    var problem = await resp.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
                                    foreach (var (field, messages) in problem?.Errors ?? [])
                                        _form.SetError(field.ToLowerInvariant(), messages[0], blocking: true);
                                }
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: PLATFORMS
            // =========================================================================
            MauiAndAotPage("Klyroo.Form", "Klyroo.Form.Core", "Klyroo.Form", "builder.Services.AddKlyrooForm();",
                "var form = new FormController(schema); if (await form.SubmitAsync()) Save(form.BuildOutput());"),

            // =========================================================================
            // GROUP 5: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive Klyroo.Form playgrounds covering conditional visibility & password checklists, multi-step SkipWhen wizards, repeatable ArrayTable rows, dirty path diffing, and section status + server error injection.",
                [
                    new("reactive-form", "1. Conditional Visibility, Password Checklist & Live Dirty Status",
                        Paragraphs: ["Set Age below 21 to watch Company hide and drop from validation, and type into Password to test live PasswordRequirements rules."],
                        LiveDemoKey: "form-reactive",
                        LiveDemoTitle: "KlyFormEngine · Conditional Rules & Password Checklist",
                        CodeSamples:
                        [
                            new("ProfileForm.cs", """
                                var schema = new FormSchema
                                {
                                    FormKey = "docs-reactive-demo",
                                    Title = "Profile Settings",
                                    Fields =
                                    [
                                        new FormFieldSchema { Key = "name", Label = "Full Name", Required = true, DefaultValue = "Alice" },
                                        new FormFieldSchema { Key = "age", Label = "Age", Kind = FormFieldKind.Number, Min = 18, Max = 120, DefaultValue = 25.0 },
                                        new FormFieldSchema { Key = "company", Label = "Company", Required = true, Condition = f => f.GetValue<double>("age") >= 21 },
                                        new FormFieldSchema { Key = "password", Label = "Password", Kind = FormFieldKind.Password,
                                            PasswordRequirements = [new("len", "At least 8 characters", s => s.Length >= 8)] }
                                    ]
                                };
                                """)
                        ]),
                    new("wizard-steps", "2. Multi-Step Onboarding Wizard with SkipWhen",
                        Paragraphs: ["Select 'Free' vs 'Pro' on Step 1 to see Step 2 (Billing) skipped dynamically via FormStepSchema.SkipWhen."],
                        LiveDemoKey: "form-wizard",
                        LiveDemoTitle: "KlyFormEngine · Multi-Step FormStepSchema Wizard",
                        CodeSamples:
                        [
                            new("OnboardingWizard.cs", """
                                var schema = new FormSchema
                                {
                                    FormKey = "docs-wizard-demo",
                                    ShowStepper = true,
                                    Steps =
                                    [
                                        new FormStepSchema { Key = "account", Title = "Account", Fields = [ /* org, plan */ ] },
                                        new FormStepSchema { Key = "billing", Title = "Billing", SkipWhen = f => f.GetValue<string>("plan") == "free", Fields = [ /* vat */ ] }
                                    ]
                                };
                                """)
                        ]),
                    new("repeating-array", "3. Dynamic Repeating ArrayTable Line Items",
                        Paragraphs: ["Add, edit, and remove invoice line items dynamically using FormFieldKind.ArrayTable and FormController.GetArrayItems(\"items\")."],
                        LiveDemoKey: "form-array",
                        LiveDemoTitle: "FormController · Dynamic ArrayTable Line Items",
                        CodeSamples:
                        [
                            new("InvoiceItems.cs", """
                                controller.AddArrayItem(lineItemsField);
                                controller.SetArrayItemValue("items", index: 0, childKey: "sku", value: "KLY-PRO");
                                controller.RemoveArrayItem("items", index: 1);
                                """)
                        ]),
                    new("dirty-diff", "4. Dirty Tracking, ResetField & Output Transforms",
                        Paragraphs: ["Mutate fields programmatically, watch IsDirty and DirtyPaths update, revert a single field with ResetField(), or SubmitAsync() to re-baseline."],
                        LiveDemoKey: "form-dirty-diff",
                        LiveDemoTitle: "FormController · IsDirty, DirtyPaths & BuildOutput()",
                        CodeSamples:
                        [
                            new("DirtyDiff.cs", """
                                controller.SetValue("name", "Claire", markTouched: true);
                                var dirty = controller.DirtyPaths;     // ["name"]
                                controller.ResetField("name");
                                await controller.SubmitAsync();        // Re-baselines IsDirty -> false
                                var output = controller.BuildOutput(); // Trimmed & transformed dictionary
                                """)
                        ]),
                    new("sections-and-errors", "5. Section Completion Statuses & Server Blocking Errors",
                        Paragraphs: ["Inject a server-side blocking error onto 'email' and watch GetSectionStates() transition live between Pending, Invalid, Complete, and Optional."],
                        LiveDemoKey: "form-sections-errors",
                        LiveDemoTitle: "FormPageSectionStatus & SetError(blocking: true)",
                        CodeSamples:
                        [
                            new("SectionsAndErrors.cs", """
                                controller.SetError("email", "Email already registered on server", blocking: true);
                                // controller.HasBlockingErrors == true
                                // controller.GetSectionStates()[0].Status == FormPageSectionStatus.Invalid
                                controller.ClearError("email");
                                """)
                        ])
                ]),
            new("api", "API Reference", "Reference",
                "Complete public API reference for Klyroo.Form.Core and Klyroo.Form.",
                [],
                ApiTypes: [typeof(KlyFormEngine), typeof(KlyFormPage), typeof(KlyFormProvider), typeof(FormController), typeof(FormSchema), typeof(FormFieldSchema), typeof(FormStepSchema), typeof(FormPageSectionSchema), typeof(FormService)])
        ]);
}
