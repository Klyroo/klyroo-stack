using Klyroo.Form;
using Klyroo.Form.Dynamic;

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
                "A form bound to a C# model: 31 field kinds chosen from the member types, reactive visibility, disabled and required rules, dirty tracking, input and output transforms, repeatable rows, multi-step wizards with SkipWhen, sectioned form pages, programmatic FormService overlays, and a Dynamic mode for forms known only at run time.",
                [
                    new("overview", "What is Klyroo.Form?",
                        Paragraphs:
                        [
                            "Klyroo.Form replaces hand-coded Razor form markup and fragile validation state with a typed schema, FormSchema.For<TModel>, and a headless, NativeAOT FormController<TModel>. Fields are the members of your own model, so a wrong type or a renamed property is a compile error. Every rule (conditional visibility, required and disabled predicates, change hooks, password checklists, list rows, dirty tracking, server error injection and wizard step skipping) executes in plain C# with no UI dependency.",
                            "Render a FormController<TModel> inline with <KlyFormEngine>, organize long settings pages with <KlyFormPage>, or open Modal, Drawer and Fullscreen forms from anywhere with FormService.CreateFormAsync(). A form that is only known at run time uses the same engine through the Klyroo.Form.Dynamic types (DynamicFormSchema, DynamicFormController, KlyDynamicFormEngine)."
                        ],
                        LiveDemoKey: "form-reactive",
                        LiveDemoTitle: "Klyroo.Form · Live Reactive Condition, Password Rules & Dirty Tracking"),
                    new("pipeline", "How Klyroo.Form Works End-to-End (The 5-Stage Lifecycle)",
                        TableHeaders: ["Stage", "API / Type", "What Happens"],
                        TableRows:
                        [
                            new("1. Define Schema", "FormSchema.For<TModel> · s.Field · s.Step · s.Section · s.List", "Declares the fields of a model (31 FormFieldKind values chosen from the member types), defaults, transforms, conditions, validation triggers, steps or page sections, and returns an immutable FormSchema<TModel> you can share."),
                            new("2. Hydrate & Baseline", "new FormController<TModel>(schema, model)", "Applies Default and TransformInput to the values of the model, then captures an immutable baseline snapshot for IsDirty and IsFieldDirty."),
                            new("3. Reactive Edits", "Set · AddRow · OnChange · VisibleWhen", "Re-evaluates VisibleWhen, Required, DisabledWhen and Options live; hidden fields bypass validation and are not written to the model."),
                            new("4. Validate & Step", "Validate() · NextStepAsync() · SetError(field, message, blocking: true)", "Runs per-field and per-step rules, skips steps whose SkipWhen is true, invokes OnBeforeNext, and surfaces client or server blocking errors."),
                            new("5. Transform & Submit", "SubmitAsync(save, target) · CreateModel() · ApplyTo(model)", "Runs TransformOutput, omits OmitFromOutput and hidden fields, writes the model all or nothing, calls your save step, and re-baselines IsDirty to false on success.")
                        ]),
                    new("layers", "Two-Layer Architecture",
                        TableHeaders: ["Package", "Primary Exports", "Compatibility"],
                        TableRows:
                        [
                            new("Klyroo.Form.Core", "FormSchema, FormController<TModel>, FormField<TModel, TValue>, IFormRuntime, and the Dynamic types DynamicFormSchema, DynamicFieldSchema, DynamicFormController", "Headless C# · IsAotCompatible=true"),
                            new("Klyroo.Form", "KlyFormEngine<TModel>, KlyFormPage<TModel>, KlyFormProvider, FormService, KlyDynamicFormEngine, AddKlyrooForm()", "browser, ios, android, maccatalyst, windows"),
                            new("Klyroo.Form.DataAnnotations", "UseDataAnnotations(), FromDataAnnotations()", "Add-on: Klyroo.Stack does not include it; add it to read the attributes of your models")
                        ])
                ]),
            new("installation", "Installation & Setup", "Getting Started",
                "Install Klyroo.Form, register the form services, and mount KlyFormProvider once for programmatic Modal, Drawer, and Fullscreen overlays.",
                [
                    new("nuget", "1. Install Packages",
                        CodeSamples:
                        [
                            new("Terminal", """
                                dotnet add package Klyroo.Form.Core             # Headless NativeAOT schema & controller
                                dotnet add package Klyroo.Form                  # Blazor Web & .NET MAUI form components
                                dotnet add package Klyroo.Form.DataAnnotations  # Optional add-on: read [Required], [Range]... from your models
                                """)
                        ]),
                    new("di", "2. Register Services & Mount Overlay Provider",
                        Paragraphs: ["AddKlyrooForm() registers FormService and DynamicFormService as scoped services. Mount <KlyFormProvider /> once in your root layout so FormService.CreateFormAsync() can open Modal, Drawer, or Fullscreen forms from any page or service."],
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
                "Complete catalog of the 31 FormFieldKind values, the typed builder methods that choose them, and repeatable list rows.",
                [
                    new("all-kinds", "All 31 FormFieldKind Values",
                        TableHeaders: ["Category", "Exact FormFieldKind Values", "Capabilities"],
                        TableRows:
                        [
                            new("Text & Auth (5)", "Text, Password, Textarea, PhoneNumber, OneTimeCode", "string members: MaxLength, Placeholder, AsTextarea, AsPassword(requirements) with a live checklist, AsPhone, AsOneTimeCode"),
                            new("Numeric & Visual (4)", "Number, Slider, Rating, ColorPicker", "int, decimal and the other numeric members: Range, StepBy, AsSlider, AsRating, AsColor"),
                            new("Boolean & Choice (6)", "Checkbox, Switch, Radio, CheckboxGroup, Select, AutoComplete", "bool members, enums (one option per member) and Options(...) with FormOption<TValue>: AsSwitch, AsRadio, AsAutoComplete, OnCreateOption, MultiSelect"),
                            new("Temporal & Tags (5)", "Date, DateTime, DateRange, Time, Tag", "DateOnly, DateTime, TimeOnly and FormDateRange members, and a MultiSelect of strings for tags"),
                            new("Stateless Layout (6)", "Hidden, Info, Divider, Section, Button, Group", "Heading, Info (FormInfoTone), Divider, Button, Section and Group, which are not part of the model"),
                            new("Composite & Nested (5)", "Card, Tabs, Object, ArrayList, ArrayTable", "Card, Tabs, Group (a nested object member) and List (a collection member as repeatable rows)")
                        ]),
                    new("arrays", "Repeatable Rows (s.List)",
                        Paragraphs: ["Declare a collection member with s.List to add, edit and remove rows with live validation. The controller gives you the rows as FormRow<TItem> and AddRow and RemoveRow to change them."],
                        LiveDemoKey: "form-array",
                        LiveDemoTitle: "Interactive s.List Line Items",
                        CodeSamples:
                        [
                            new("ArrayFields.cs", """
                                public sealed class InvoiceLine
                                {
                                    public string Sku { get; set; } = "";
                                    public int Qty { get; set; }
                                }

                                public sealed class Invoice
                                {
                                    public List<InvoiceLine> Items { get; set; } = [];
                                }

                                var schema = FormSchema.For<Invoice>(s => s.List(m => m.Items, row =>
                                {
                                    row.Field(l => l.Sku, f => f.Label("SKU").Required());
                                    row.Field(l => l.Qty, f => f.Range(1, 999).Default(1));
                                }, addLabel: "Add line item", configure: f => f.Label("Invoice Line Items")));

                                var controller = new FormController<Invoice>(schema);
                                var items = schema.List(m => m.Items);

                                controller.AddRow(items);
                                var rows = controller.Rows(items);       // IReadOnlyList<FormRow<InvoiceLine>>
                                controller.RemoveRow(items, 0);
                                """)
                        ])
                ]),
            new("conditions", "Reactive Conditions & Transforms", "Schema & State",
                "Control visibility, required state, disabled state, dependent options, change hooks, and input and output normalization with typed callbacks.",
                [
                    new("reactive", "VisibleWhen, Required, DisabledWhen & Transforms",
                        Paragraphs:
                        [
                            "FormController<TModel> evaluates VisibleWhen, Required(when), DisabledWhen and the Options function live against the current state, and a later condition points at an earlier field through the handle that s.Field returned. When VisibleWhen returns false, the field is hidden, bypassed during Validate(), and not written to the model.",
                            "Use TransformInput to normalize a model when it is loaded and TransformOutput to sanitize the final value (trimming, lowercasing, formatting) before it is written."
                        ],
                        LiveDemoKey: "form-reactive",
                        LiveDemoTitle: "Set Age below 21 to watch Company hide and drop from validation",
                        CodeSamples:
                        [
                            new("ReactiveSchema.cs", """
                                public sealed class Profile
                                {
                                    public string Name { get; set; } = "";
                                    public string Slug { get; set; } = "";
                                    public int Age { get; set; }
                                    public string? Company { get; set; }
                                    public bool Locked { get; set; }
                                }

                                var schema = FormSchema.For<Profile>(s =>
                                {
                                    s.FormKey = "profile";
                                    s.Title = "User Profile";
                                    var slug = s.Field(m => m.Slug);
                                    var locked = s.Field(m => m.Locked);
                                    s.Field(m => m.Name, f => f.Required()
                                        .TransformOutput(v => v.Trim())
                                        .OnChange((name, form) => form.Set(slug, name.Trim().ToLowerInvariant().Replace(' ', '-'))));
                                    var age = s.Field(m => m.Age, f => f.Range(18, 120).Default(25));
                                    s.Field(m => m.Company, f => f
                                        .VisibleWhen(c => c.Get(age) >= 21)
                                        .Required(c => c.Get(age) >= 21)
                                        .DisabledWhen(c => c.Get(locked), "Locked by workspace administrator"));
                                });
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
                            new("FormValidationMode", "All, RequiredOnly, ValidatorsOnly, None", "Controls which validation passes run, set with s.ValidateMode on the builder"),
                            new("FormValidationTrigger", "Blur, Input, Submit", "Per-field trigger deciding when validation feedback surfaces")
                        ]),
                    new("dirty", "IsDirty, IsFieldDirty, ResetField & Submit Re-Baselining",
                        Paragraphs: ["FormController<TModel> compares the current state against the baseline snapshot to compute IsDirty and IsFieldDirty(field). Calling SubmitAsync() re-baselines on success so IsDirty resets to false."],
                        LiveDemoKey: "form-dirty-diff",
                        LiveDemoTitle: "Interactive Dirty Tracking, ResetField & CreateModel()",
                        CodeSamples:
                        [
                            new("DirtyAndServerErrors.cs", """
                                // Track the exact modified fields against the initial snapshot
                                var name = schema.Field(m => m.Name);
                                var email = schema.Field(m => m.Email);
                                controller.Set(name, "Claire", markTouched: true);
                                Assert.True(controller.IsDirty);
                                Assert.True(controller.IsFieldDirty(name));

                                // Attach server-side validation error returned from an API (409 Conflict / 422)
                                controller.SetError(email, "Email already registered", blocking: true);
                                Assert.True(controller.HasBlockingErrors);

                                // Revert one field or clear error
                                controller.ResetField(name);
                                controller.ClearErrors(email);
                                """)
                        ])
                ]),
            new("wizards-and-pages", "Wizards, KlyFormPage & FormService", "UI Components",
                "Build stepped wizards with SkipWhen, sectioned settings pages with completion badges, and imperative Modal/Drawer/Fullscreen overlays.",
                [
                    new("wizard", "Multi-Step Wizards with SkipWhen & OnBeforeNext",
                        Paragraphs: ["Declare steps with s.Step to enable stepped mode (controller.IsStepped). NextStepAsync() validates only the current step's visible fields, awaits OnBeforeNext, and automatically jumps past steps where skipWhen returns true."],
                        LiveDemoKey: "form-wizard",
                        LiveDemoTitle: "Interactive Multi-Step Wizard with SkipWhen",
                        CodeSamples:
                        [
                            new("OnboardingWizard.cs", """
                                public enum Plan { Free, Pro }

                                public sealed class Onboarding
                                {
                                    public Plan Plan { get; set; }
                                    public string Vat { get; set; } = "";
                                }

                                var wizardSchema = FormSchema.For<Onboarding>(s =>
                                {
                                    s.FormKey = "onboarding";
                                    s.Title = "Workspace Onboarding";
                                    s.ShowStepper = true;
                                    FormField<Onboarding, Plan>? plan = null;
                                    s.Step("account", "Account", st => plan = st.Field(m => m.Plan, f => f.Default(Plan.Free)));
                                    s.Step("billing", "Billing", st => st.Field(m => m.Vat, f => f.Label("VAT Number").Required()),
                                        skipWhen: c => c.Get(plan!) == Plan.Free);
                                });
                                """)
                        ]),
                    new("sections-service", "KlyFormPage Section Statuses & FormService.CreateFormAsync",
                        Paragraphs: ["Declare sections with s.Section and render <KlyFormPage Form=\"@_controller\" /> to track Complete, Pending, Invalid, and Optional statuses per section. Use FormService.CreateFormAsync() to open any schema in a Modal, Drawer, or Fullscreen overlay."],
                        LiveDemoKey: "form-sections-errors",
                        LiveDemoTitle: "Interactive FormPageSectionStatus & Server Blocking Errors",
                        CodeSamples:
                        [
                            new("OverlayCall.cs", """
                                FormSubmitOutcome<Invite> outcome = await formService.CreateFormAsync(
                                    inviteSchema,
                                    new Invite { Role = "Editor" },
                                    FormShellMode.Drawer);

                                if (outcome is { IsCompleted: true, Model: { } invite })
                                    await api.InviteAsync(invite);
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
                        Paragraphs: ["Launch FormService.CreateFormAsync(schema, model, FormShellMode.Drawer) directly from a Klyroo.Table toolbar 'New' button or row action 'Edit', then call _list.ReloadAsync() on submission."],
                        CodeSamples:
                        [
                            new("TableCrudDrawer.cs", """
                                var createOutcome = await formService.CreateFormAsync(
                                    EmployeeFormSchemas.Create, mode: FormShellMode.Drawer);

                                if (createOutcome is { IsCompleted: true, Model: { } employee })
                                {
                                    await employeeApi.CreateAsync(employee);
                                    await _dataList.ReloadAsync();
                                }
                                """)
                        ]),
                    new("query-search", "2. Klyroo.Form + Klyroo.Query (Advanced Filter Drawer -> URL QueryState)",
                        Paragraphs: ["Collect multi-field filter criteria in a validated Klyroo.Form drawer, convert the submitted model into FilterRule[] + DataListState, and push them to QueryState via DataListStateQuery.ToQuery."],
                        CodeSamples:
                        [
                            new("FormToQuery.cs", """
                                using Klyroo.Form;
                                using Klyroo.Query;

                                var outcome = await formService.CreateFormAsync(AdvancedSearchSchema, mode: FormShellMode.Modal);
                                if (outcome is not { IsCompleted: true, Model: { } search }) return;

                                var rules = new List<FilterRule>();
                                if (search.MinSalary is { } min)
                                    rules.Add(new FilterRule("salary", FilterOperator.Gte, new FilterValue.Number(min)));

                                var state = DataListState.Initial with { Filters = rules };
                                var pairs = DataListStateQuery.ToQuery(state, EmployeeSchemas.Directory, prefix: "emp")
                                    .Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value));
                                queryState.SetMany(pairs, QueryHistory.Push);
                                """)
                        ])
                ]),
            new("integrations-agent-dashboard", "Integration: Klyroo.Agent & Klyroo.Dashboard", "Integrations",
                "Expose any FormController<TModel> to AI copilots via Klyroo.Agent (sensitive fields are redacted and refused, and submit is behind a confirmation gate) and configure Klyroo.Dashboard presets with Klyroo.Form.",
                [
                    new("agent-bridge", "1. Klyroo.Form + Klyroo.Agent (AI Copilot Form Filling & Undo)",
                        Paragraphs: ["Call agent.RegisterForm(controller, confirmSubmit: true) to expose every visible field as semantic snapshot/query/act controls (it takes an IFormRuntime, so a FormController<TModel> and a DynamicFormController work the same). Password and OneTimeCode fields are redacted to \"[redacted]\" and the model cannot write them unless you name them in allowSensitiveWrites; a field of another step of a wizard is refused until the nextStep action moves there; AllowPolicy gates the final submit step."],
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
                "Preview form changes with Klyroo.Display Diff cards before submission and map ASP.NET Core ProblemDetails / ValidationProblemDetails errors back onto FormController<TModel>.",
                [
                    new("diff-review", "1. Klyroo.Form + Klyroo.Display (Pre-Submit Change Diff Review)",
                        CodeSamples:
                        [
                            new("DiffBeforeSave.razor", """
                                @using Klyroo.Form
                                @using Klyroo.Display

                                @* Show a clean side-by-side Klyroo.Display Diff of every dirty field before saving *@
                                <KlyDisplay Kind="DisplayKind.Diff" Title="Pending Changes" DiffItems="@Changes()" />

                                @code {
                                    private IReadOnlyList<DisplayDiffItem> Changes()
                                    {
                                        var items = new List<DisplayDiffItem>();
                                        if (_form.IsFieldDirty(_name)) items.Add(new DisplayDiffItem("Name", _initial.Name, _form.Get(_name), Changed: true));
                                        if (_form.IsFieldDirty(_plan)) items.Add(new DisplayDiffItem("Plan", _initial.Plan, _form.Get(_plan), Changed: true));
                                        return items;
                                    }
                                }
                                """)
                        ]),
                    new("server-422", "2. Mapping ASP.NET Core 422 ValidationProblemDetails to FormController<TModel>",
                        CodeSamples:
                        [
                            new("ServerValidationMap.cs", """
                                using var resp = await http.PostAsJsonAsync("/api/accounts", _form.CreateModel());
                                if (resp.StatusCode == HttpStatusCode.UnprocessableEntity)
                                {
                                    var problem = await resp.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
                                    foreach (var (field, messages) in problem?.Errors ?? [])
                                    {
                                        var key = char.ToUpperInvariant(field[0]) + field[1..];   // "email" is the member Email
                                        _form.SetError(_form.Schema.Item(key), messages[0], blocking: true);
                                    }
                                }
                                """)
                        ])
                ]),

            // =========================================================================
            // GROUP 4: PLATFORMS
            // =========================================================================
            MauiAndAotPage("Klyroo.Form", "Klyroo.Form.Core", "Klyroo.Form", "builder.Services.AddKlyrooForm();",
                "var form = new FormController<Config>(schema); if (await form.SubmitAsync()) Save(form.CreateModel());"),

            // =========================================================================
            // GROUP 5: LIVE EXAMPLES (5 INTERACTIVE DEMOS)
            // =========================================================================
            new("examples", "Live Examples", "Examples",
                "5 interactive Klyroo.Form playgrounds covering conditional visibility & password checklists, multi-step SkipWhen wizards, repeatable list rows, dirty field tracking, and section status + server error injection.",
                [
                    new("reactive-form", "1. Conditional Visibility, Password Checklist & Live Dirty Status",
                        Paragraphs: ["Set Age below 21 to watch Company hide and drop from validation, and type into Password to test live PasswordRequirements rules."],
                        LiveDemoKey: "form-reactive",
                        LiveDemoTitle: "KlyFormEngine · Conditional Rules & Password Checklist",
                        CodeSamples:
                        [
                            new("ProfileForm.cs", """
                                public sealed class ProfileSettings
                                {
                                    public string Name { get; set; } = "";
                                    public int Age { get; set; }
                                    public string Company { get; set; } = "";
                                    public string Password { get; set; } = "";
                                }

                                var schema = FormSchema.For<ProfileSettings>(s =>
                                {
                                    s.FormKey = "docs-reactive-demo";
                                    s.Title = "Profile Settings";
                                    s.Field(m => m.Name, f => f.Label("Full Name").Required().Default("Alice"));
                                    var age = s.Field(m => m.Age, f => f.Range(18, 120).Default(25));
                                    s.Field(m => m.Company, f => f.Required().VisibleWhen(c => c.Get(age) >= 21));
                                    s.Field(m => m.Password, f => f.AsPassword(new FormPasswordRequirement("len", "At least 8 characters", pw => pw.Length >= 8)));
                                });
                                """)
                        ]),
                    new("wizard-steps", "2. Multi-Step Onboarding Wizard with SkipWhen",
                        Paragraphs: ["Select 'Free' vs 'Pro' on Step 1 to see Step 2 (Billing) skipped dynamically via the skipWhen argument of s.Step."],
                        LiveDemoKey: "form-wizard",
                        LiveDemoTitle: "KlyFormEngine · Multi-Step Wizard",
                        CodeSamples:
                        [
                            new("OnboardingWizard.cs", """
                                var schema = FormSchema.For<Onboarding>(s =>
                                {
                                    s.FormKey = "docs-wizard-demo";
                                    s.ShowStepper = true;
                                    FormField<Onboarding, OnboardingPlan>? plan = null;
                                    s.Step("account", "Account", st => { /* org */ plan = st.Field(m => m.Plan); });
                                    s.Step("billing", "Billing", st => { /* vat */ }, skipWhen: c => c.Get(plan!) == OnboardingPlan.Free);
                                });
                                """)
                        ]),
                    new("repeating-array", "3. Dynamic Repeating List Line Items",
                        Paragraphs: ["Add, edit, and remove invoice line items dynamically using s.List and FormController<TModel>.Rows."],
                        LiveDemoKey: "form-array",
                        LiveDemoTitle: "FormController · Dynamic List Line Items",
                        CodeSamples:
                        [
                            new("InvoiceItems.cs", """
                                var items = schema.List(m => m.Items);
                                controller.AddRow(items, new InvoiceLine { Sku = "KLY-PRO", Qty = 2 });
                                var first = controller.Rows(items)[0];
                                controller.RemoveRow(items, 1);
                                """)
                        ]),
                    new("dirty-diff", "4. Dirty Tracking, ResetField & Output Transforms",
                        Paragraphs: ["Mutate fields programmatically, watch IsDirty and IsFieldDirty update, revert a single field with ResetField(), or SubmitAsync() to re-baseline."],
                        LiveDemoKey: "form-dirty-diff",
                        LiveDemoTitle: "FormController · IsDirty, IsFieldDirty & CreateModel()",
                        CodeSamples:
                        [
                            new("DirtyDiff.cs", """
                                controller.Set(name, "Claire", markTouched: true);
                                var dirty = controller.IsFieldDirty(name);   // true
                                controller.ResetField(name);
                                await controller.SubmitAsync();              // Re-baselines IsDirty -> false
                                var output = controller.CreateModel();       // Trimmed & transformed model
                                """)
                        ]),
                    new("sections-and-errors", "5. Section Completion Statuses & Server Blocking Errors",
                        Paragraphs: ["Inject a server-side blocking error onto Email and watch GetSectionStates() transition live between Pending, Invalid, Complete, and Optional."],
                        LiveDemoKey: "form-sections-errors",
                        LiveDemoTitle: "FormPageSectionStatus & SetError(blocking: true)",
                        CodeSamples:
                        [
                            new("SectionsAndErrors.cs", """
                                controller.SetError(email, "Email already registered on server", blocking: true);
                                // controller.HasBlockingErrors == true
                                // controller.GetSectionStates()[0].Status == FormPageSectionStatus.Invalid
                                controller.ClearErrors(email);
                                """)
                        ])
                ]),
            new("api", "API Reference", "Reference",
                "Complete public API reference for Klyroo.Form.Core and Klyroo.Form.",
                [],
                ApiTypes: [typeof(KlyFormEngine<object>), typeof(KlyFormPage<object>), typeof(KlyDynamicFormEngine), typeof(KlyDynamicFormPage), typeof(KlyFormProvider), typeof(FormController<object>), typeof(FormSchema<object>), typeof(DynamicFormController), typeof(DynamicFormSchema), typeof(DynamicFieldSchema), typeof(DynamicStepSchema), typeof(DynamicSectionSchema), typeof(FormService), typeof(DynamicFormService), typeof(IFormRuntime)])
        ]);
}
