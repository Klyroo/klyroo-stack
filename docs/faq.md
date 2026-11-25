# FAQ

Symptoms, causes and fixes that follow from the code of this repository, then known limitations and common questions.

## Troubleshooting

**The solution does not restore or build: a project reference cannot be found.**
The projects reference the sibling repositories by relative path (`..\..\..\klyroo-ui\...`). Clone `klyroo-ui`, `klyroo-query`, `klyroo-table`, `klyroo-form`, `klyroo-dashboard`, `klyroo-display`, `klyroo-spreadsheet` and `klyroo-agent` in the same parent folder as `klyroo-stack`, then build again. [CONTRIBUTING.md](../CONTRIBUTING.md) shows the layout.

**The page renders without styles, or the components look unstyled.**
The stylesheets are not loaded. Add `klyroo.preflight.css` and `klyroo.css` from `_content/Klyroo.UI/` and `klyroo-stack.css` from `_content/Klyroo.Stack/` to the `<head>` of `App.razor`, as in [Getting started](getting-started.md#load-the-stylesheets). If the stylesheets load but spacing or colors are wrong, check for Bootstrap or another stylesheet that sits outside CSS layers: it overrides the layered CSS of Tailwind v4. Remove it or import it into a layer (`@import url(bootstrap.css) layer(bootstrap);`).

**A `KlyButton` with `OnClick` does nothing.**
In static server rendering there is no circuit, so a .NET click handler cannot run. Render the page interactively (`@rendermode InteractiveServer`, WebAssembly or Auto), or use `Href` or a form post for the action.

**Components change appearance after the page loads, in an app with a WebAssembly client.**
The server and the client registered different options. Call `AddKlyrooStack()` in both `Program.cs` files with the same configuration, ideally through one shared method: [Share options between the server and the WebAssembly client](guides.md#share-options-between-the-server-and-the-webassembly-client).

**`InvalidOperationException` when changing Klyroo.Table messages or themes.**
The options freeze once components start resolving themes and texts, because each locale's result is cached. Move the change into the `configureUi` or `configureTable` callback of `AddKlyrooStack()`, before the app renders.

**A form drawer or file preview never appears.**
The overlays render through `KlyFormProvider` and `KlyFilePreviewProvider`. Put both inside `<KlyApp>` in `Routes.razor`, as the templates do.

**`dotnet new klyroo-maui` creates a project that does not build.**
The template is incomplete in this preview: `MauiProgram.cs` uses an `App` class that the template does not contain. See [Platforms](platforms.md#net-maui-blazor-hybrid) for what to do instead.

**`klyroo-stack-mcp --help` shows `klyroo-ui-mcp` in its examples.**
The usage text is out of date. The installed command is `klyroo-stack-mcp`; register that one in your client.

**The MCP server exits immediately with exit code 2.**
You passed an option it does not know, or a name in `--tools` that is not a tool. Run `klyroo-stack-mcp --list-tools` for the valid names. The message on stderr says which argument was rejected.

**The MCP server says "No component named ...".**
The component is not in the bundle. The bundle covers Klyroo.UI and the Klyroo.Table components that the documentation site describes. Ask for it with `search-components` by intent, or read the skill of the library that owns it.

## Known limitations

- The MCP server documents Klyroo.UI and the Klyroo.Table components only. It does not cover Klyroo.Form, Dashboard, Display, Spreadsheet or Agent.
- The MCP server answers from the documentation bundled at build time. It does not see newer components until the tool is rebuilt and republished.
- `Klyroo.Stack` does not include `Klyroo.Query.EntityFrameworkCore`, `Klyroo.UI.Server` or `Klyroo.UI.Images`. Add them when you need EF Core execution, the server-side cookies for static rendering, or the image pipeline.
- `KlyrooStackCatalog` lists the main types of each library, not every component, and its `TemplateShortName` values are not all `dotnet new` templates that exist.
- The documentation site is not deployed. Run it locally, as described in the [README](../README.md#run-the-documentation-site-locally).
- MAUI Blazor Hybrid support is verified on Android only, according to the Klyroo.UI documentation. See [Platforms](platforms.md).

## Questions

**Should I install `Klyroo.Stack` or the libraries one by one?**
Install `Klyroo.Stack` for an app that uses most of the libraries, and individual libraries when you need one or two. The registration of the meta-package is the same list of calls you would write by hand, so you can start with it and switch later without changing code.

**Does `Klyroo.Stack.Core` work without Blazor?**
Yes. It references only the `.Core` projects, which have no package or framework references, so it works in a console app, a worker or a test project.

**Which versions go together?**
The hub and the libraries are versioned independently. This hub is at `0.1.0-preview.1` (`build/Version.props`) and the templates reference `Klyroo.Stack` at that version. While the version is `0.x`, minor versions may contain breaking changes, listed in the [changelog](../CHANGELOG.md).

**Where are the benchmarks?**
In [Benchmarks](benchmarks.md), with the command that reproduces them.
