namespace Klyroo.Stack.Site.Client.Templates;

public enum TemplateStatus
{
    /// <summary>The template exists in <c>templates/</c> and runs as a live demo of the docs site.</summary>
    Available,

    /// <summary>Planned: listed on the page, with no demo and no source.</summary>
    ComingSoon,
}

/// <summary>
/// A project template of the gallery. <see cref="WebProject"/> and <see cref="WebAssembly"/> are what the docs host needs to run the demo
/// (an available template only), the rest is what the /templates page shows.
/// </summary>
public sealed record TemplateInfo(
    string Name,
    string Title,
    string Description,
    IReadOnlyList<string> Features,
    TemplateStatus Status,
    string? WebProject = null,
    string? WebAssembly = null,
    string Category = "Apps",
    string RenderMode = "Interactive",
    string? DotnetNewCommand = null)
{
    public bool HasDemo => Status == TemplateStatus.Available;

    /// <summary>The path of the live demo below the docs site, with a trailing slash. Relative to the site root, never to the page.</summary>
    public string DemoPath => $"templates/{Name}/demo/";

    /// <summary>The name of the repository the template is published as, below <see cref="TemplateSettings.RepositoryBase"/>.</summary>
    public string RepositoryName => $"klyroo-template-{Name}";

    /// <summary>The path of the screenshot in the static files of the site, for "light" or "dark".</summary>
    public string Screenshot(string mode) => $"img/template-{Name}-{mode}.png";

    public string RunCommand => DotnetNewCommand ?? $"dotnet run --project templates/{Name}/{WebProject}";
}

/// <summary>Every template of the gallery, in the order of the page. Shared by the page and by the supervisor of the demos of the docs host.</summary>
public static class TemplateRegistry
{
    public static IReadOnlyList<TemplateInfo> All { get; } =
    [
        new("dashboard", "Dashboard",
            "An admin workspace with a collapsible sidebar, KPI cards and charts, a customers table, an inbox and multi-page settings.",
            ["Resizable sidebar with team switcher, user menu and command palette", "Customers table with search, filters, sorting, pagination and row actions", "Charts, inbox with reading pane and four settings views"],
            TemplateStatus.Available, WebProject: "src/Dashboard.Web", WebAssembly: "Dashboard.Web",
            Category: "Apps", RenderMode: "Auto + MAUI", DotnetNewCommand: "dotnet new klyroo-admin -n MyDashboard"),
        new("saas", "SaaS",
            "A full product site with landing hero, monthly/yearly pricing switch, Markdown blog, docs, changelog and auth forms.",
            ["Landing, pricing, blog, docs and changelog authored in Markdown", "Sign-in and sign-up flows with validation", "100% Static SSR: zero SignalR circuit, zero WebAssembly"],
            TemplateStatus.Available, WebProject: "src/SaaS.Web", WebAssembly: "SaaS.Web",
            Category: "Marketing", RenderMode: "Static SSR", DotnetNewCommand: "dotnet new klyroo-saas -n MySaas"),
        new("landing", "Landing",
            "A focused product marketing site with hero, feature grid, testimonials, CSS-only pricing toggle, FAQ and footer.",
            ["Rendered by the server with zero circuit and zero WebAssembly", "Zero-JS monthly/yearly pricing switch via native radio inputs", "All marketing copy centralized in one strongly typed C# file"],
            TemplateStatus.Available, WebProject: "src/Landing.Web", WebAssembly: "Landing.Web",
            Category: "Marketing", RenderMode: "Static SSR"),
        new("chat", "Chat",
            "An AI assistant interface with conversation history, streamed Markdown replies with code blocks, model picker and prompts.",
            ["Speaks any OpenAI-compatible completions API with server-side keys", "Browser-persisted conversations: search, rename, delete, grouped by day", "Built-in simulated responder when no API key is configured"],
            TemplateStatus.Available, WebProject: "src/Chat.Web", WebAssembly: "Chat.Web",
            Category: "Apps", RenderMode: "Interactive"),
        new("editor", "Editor",
            "A document workspace with sidebar library, rich text toolbar, floating selection menu, slash commands and autosave.",
            ["Headings, task lists, quotes, code blocks, links and slash commands", "Local document persistence with live word count and instant search", "Strict HTML allow-list sanitization on stored documents"],
            TemplateStatus.Available, WebProject: "src/Editor.Web", WebAssembly: "Editor.Web",
            Category: "Apps", RenderMode: "Interactive"),
        new("docs", "Docs",
            "A technical documentation portal with automatic sidebar navigation, instant search, table of contents and code copy.",
            ["Markdown pages compiled and rendered directly on the server", "Navigation, TOC and Ctrl+K search generated from content files", "Syntax-highlighted code blocks with copy buttons and callouts"],
            TemplateStatus.Available, WebProject: "src/Docs.Web", WebAssembly: "Docs.Web",
            Category: "Content", RenderMode: "Static SSR"),
        new("portfolio", "Portfolio",
            "A personal site for engineers and designers featuring selected work, timeline bio, writing, speaking and contact form.",
            ["Projects and articles authored as Markdown with dedicated detail routes", "Validated contact form ready to wire to your mail provider", "100% Static SSR with instant page transitions"],
            TemplateStatus.Available, WebProject: "src/Portfolio.Web", WebAssembly: "Portfolio.Web",
            Category: "Marketing", RenderMode: "Static SSR"),
        new("changelog", "Changelog",
            "A live release notes timeline powered by GitHub Releases API with per-release pages and a built-in Atom feed.",
            ["Fetches and caches GitHub releases automatically in memory", "Timeline view, individual release routes and Atom syndication feed", "Sanitizes untrusted Markdown release notes safely"],
            TemplateStatus.Available, WebProject: "src/Changelog.Web", WebAssembly: "Changelog.Web",
            Category: "Content", RenderMode: "Static SSR"),
        new("starter", "Starter",
            "A clean multiplatform foundation with navigation header, 12-locale switcher, dark mode, about page and MAUI head.",
            ["Interactive Server + WebAssembly + .NET MAUI Blazor Hybrid in one solution", "Tailwind CSS v4 compiled automatically by the .NET build, zero Node", "12 built-in locales, RTL support and semantic --klyroo-* tokens"],
            TemplateStatus.Available, WebProject: "src/Starter.Web", WebAssembly: "Starter.Web",
            Category: "Apps", RenderMode: "Auto + MAUI", DotnetNewCommand: "dotnet new klyroo-maui -n MyApp"),
    ];

    public static IReadOnlyList<(string ShortName, string Title, string Target, string Command)> OfficialDotnetTemplates { get; } =
    [
        ("klyroo-saas", "Full SaaS Web App", "Blazor Web · Server, WebAssembly or Auto", "dotnet new klyroo-saas -n MySaas"),
        ("klyroo-admin", "Enterprise Admin Backoffice", "Table + Form + Dashboard + Spreadsheet import", "dotnet new klyroo-admin -n MyBackoffice"),
        ("klyroo-headless", "NativeAOT API / Worker", "100% Headless *.Core · Zero UI", "dotnet new klyroo-headless -n MyApi"),
        ("klyroo-maui", "Cross-Platform MAUI Hybrid", "iOS · Android · macOS · Windows", "dotnet new klyroo-maui -n MyMobileApp"),
    ];

    public static TemplateInfo? Find(string name) => All.FirstOrDefault(t => t.Name == name);
}
