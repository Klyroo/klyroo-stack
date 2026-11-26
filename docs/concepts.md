# Concepts

This page explains how the hub is built and why. It assumes you have read [Getting started](getting-started.md).

## The ecosystem and the hub

Klyroo is eight independent libraries, each in its own repository with its own tests, versions and releases: Klyroo.UI, Query, Table, Form, Dashboard, Display, Spreadsheet and Agent. This repository, `klyroo-stack`, does not add a ninth library with features of its own. It contains four things that sit on top of the other eight:

- **Two meta-packages**, `Klyroo.Stack` and `Klyroo.Stack.Core`, that reference the other libraries and register them together.
- **The MCP server**, `Klyroo.Stack.Mcp`, a `dotnet` tool that serves documentation to coding agents.
- **The templates**, `Klyroo.Templates`, that start a project with the meta-package.
- **The documentation site**, in `site/`, which documents the ecosystem and is the source of the MCP server's documentation.

## Two layers per library

Every library follows the same split. `Klyroo.<Library>.Core` is plain C# with no Razor and no ASP.NET Core reference: the `.Core` projects have no package references at all. It holds the rules, such as the query model, the form controller, the dashboard staging or the CSV parser. `Klyroo.<Library>` is the Blazor package: components, the registration method and a stylesheet where the library has components. The Blazor package references its `.Core` package.

The split matters for three reasons. A `.Core` package runs in a console app, a worker or a unit test without a browser. The `.Core` projects are marked `IsAotCompatible` and `IsTrimmable`, so NativeAOT and trimming analyzers check them. And a rule that lives in `.Core` behaves the same in every render mode, because the Blazor component only calls it.

```text
Klyroo.Stack (Blazor, MAUI Blazor Hybrid)
 |-- Klyroo.Stack.Core --> Klyroo.Query.Core, Table.Core, Form.Core, Dashboard.Core,
 |                         Display.Core, Spreadsheet.Core, Agent.Core
 |-- Klyroo.UI
 |-- Klyroo.Query     --> Query.Core
 |-- Klyroo.Table     --> Table.Core, Query.Core, Query, UI
 |-- Klyroo.Form      --> Form.Core, UI
 |-- Klyroo.Dashboard --> Dashboard.Core, UI
 |-- Klyroo.Display   --> Display.Core, UI
 |-- Klyroo.Spreadsheet --> Spreadsheet.Core, UI
 '-- Klyroo.Agent     --> Agent.Core, Form.Core, Dashboard.Core, Spreadsheet.Core
```

Klyroo.UI has no `.Core` package. The catalog lists `Klyroo.UI.Build.Tasks` in that slot, which is the MSBuild task package of Klyroo.UI and not a headless engine, so `Klyroo.Stack.Core` does not reference it.

A few packages are add-ons that the hub does not reference: `Klyroo.UI.Server`, `Klyroo.UI.Images`, `Klyroo.UI.Images.Local`, `Klyroo.Query.EntityFrameworkCore`, `Klyroo.Form.DataAnnotations` and `Klyroo.Agent.AI`. `KlyrooStackModule.AddOnPackages` lists them per library.

## What `AddKlyrooStack()` does

`AddKlyrooStack(configureUi, configureTable)` calls, in order, `AddKlyrooUi(configureUi)`, `AddKlyrooQuery()`, `AddKlyrooTable(configureTable)`, `AddKlyrooForm()`, `AddKlyrooDashboard()`, `AddKlyrooDisplay()`, `AddKlyrooSpreadsheet()` and `AddKlyrooAgent()`. Nothing else happens: there is no hidden state in the hub.

The services that these calls add are scoped where they hold per-user state (`QueryState`, `FormService`, `FilePreviewService`, `AgentBridgeService`) and singletons where they hold configuration (`KlyrooOptions`). Scoped means one instance per circuit in Interactive Server and one per browser tab in WebAssembly, so a user's form drawer or query state never leaks into another user's session.

Because `AddKlyrooUi` reuses the options object that an earlier call created, the individual registration methods can be called in any order and from any layer, and every `configure` callback applies to the same object. That is also why the options freeze: components cache what they resolve (the error that Klyroo.Table raises says the result of each locale is cached), so a callback that ran after the first render would have no effect.

## The catalog

`KlyrooStackCatalog.Modules` is a list of eight `KlyrooStackModule` records. Each one holds the repository id, the library name, the headless and Blazor package names, the template short name, the agent skill path, a summary and a list of key component names. The catalog exists so that tools do not hard-code the layout of the ecosystem: the home page of the documentation site uses it, and your own tooling can answer "which package do I add for `KlyFormEngine`" from one place.

The catalog is data written by hand next to the code. Tests check that every module has an id, a name, a UI package and key components, that ids are unique, and that every `TemplateShortName` is a template of `Klyroo.Templates`. They cannot check that a package exists on NuGet, so treat `LandingPagePath` as a hint.

## The MCP server and the documentation bundle

The MCP server does not read live files. Building `src/Klyroo.Stack.Mcp.Core` runs `build/Klyroo.Stack.DocsBundle`, which reads the sources of the documentation site and of Klyroo.UI, extracts every component's parameters, events and slots, the Razor source of every example, the theme slots and the text of each guide, and writes one `docs-bundle.json`. That file is embedded in the assembly.

```text
site/Klyroo.Stack.Site.Client (pages, examples)  \
                                                  +--> DocsBundle --> docs-bundle.json (embedded)
../klyroo-ui/src/Klyroo.UI (component API)       /                       |
                                                    +--------------------+--------------------+
                                                    |                    |                    |
                                          klyroo-stack-mcp (stdio)   /mcp (site, HTTP)   /llms.txt, /llms-full.txt
```

One bundle feeds the stdio tool, the HTTP endpoint that the site maps at `/mcp`, the `llms.txt` files and the generated skill reference, so the answers cannot drift apart. The trade-off is that the tool is only as current as its build: a new Klyroo.UI parameter appears in answers after the bundle is regenerated and the tool is republished. The tool needs no network, and it exposes only read-only tools.

The bundle documents Klyroo.UI and the Klyroo.Table components that the site describes, plus the documentation pages (getting started, guides, concepts, platforms, FAQ and API reference) of Klyroo.Query, Table, Form, Dashboard, Display, Spreadsheet and Agent, at paths such as `/form/docs/guides`; the build reads them from the sibling repositories, and skips a repository that is not cloned. It carries a `Package` field per component for the ones outside Klyroo.UI, so an agent that asks about a data list is told to add `Klyroo.Table`.

## Render modes and where state lives

The Klyroo components run UI behavior such as popovers, focus and keyboard navigation in the browser, and tell .NET only about state you bind with `@bind-Value` or `@bind-Open`. That is why the same markup works in static server rendering, Interactive Server, WebAssembly and Auto. In static rendering there is no circuit and no WebAssembly runtime, so anything that needs a .NET event handler, such as `OnClick` on a `KlyButton`, cannot run until the page is interactive; use `Href` or a form post for those. [Platforms](platforms.md) lists what to expect in each mode.

With a WebAssembly client the same registration must run in both projects, because the server prerenders the page and the client renders it again. If the two differ, components change appearance when the client takes over.

## Threading and errors

The hub has no threads, timers or static state of its own. The `.Core` engines are ordinary objects that the Blazor components create and call, so give each user session its own controller instances (the scoped services above do this for you) and read the repository of each library for its concurrency rules. The catalog is immutable data and safe to read from any thread. Klyroo.Dashboard stages its queries as essential, background and deferred; see its repository for the details.

Misuse raises standard exceptions: the registration methods throw `ArgumentNullException` for a null service collection, and Klyroo.Table options throw `InvalidOperationException` when changed after they froze. The MCP server reports an unknown component, example, page, section or tool as an MCP error that lists the closest matches, and the tool exits with code 2 when started with an unknown option or tool name.

## Design decisions

- **Relative project references.** The projects reference the sibling repositories by relative path (`../klyroo-ui/...`) so a change in a library is tested against the hub immediately. The price is that you must clone the siblings next to this repository to build it, and CI checks them out side by side. When packed, each reference becomes a NuGet dependency.
- **One registration call, many small ones.** `AddKlyrooStack()` exists for convenience, but the libraries stay installable one by one, so a team that needs only queries does not take Blazor components with it.
- **A bundle instead of live introspection.** The MCP server answers from a generated file so it starts fast, works offline and cannot be asked to load code. It cannot show documentation for a library version other than the one it was built with.
- **A hand-written catalog.** It is simple and fast, and it can go stale; the test suite catches missing data, not wrong data.
