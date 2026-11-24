# LuxStack (`lux.stack`)

> **Modular, headless-first Blazor & .NET 10 component ecosystem — engineered for Static SSR, Interactive Server, WebAssembly, .NET MAUI Blazor Hybrid, and NativeAOT.**

**LuxStack** is an ecosystem of **8 independent, composable libraries** united under one architecture:

- **`Lux.<Product>.Core`** — 100% headless C#, zero UI dependencies, `<IsAotCompatible>true</IsAotCompatible>`, `<IsTrimmable>true</IsTrimmable>`.
- **`Lux.<Product>`** — Multiplatform Blazor UI (`Lx*` components, `--lux-*` CSS tokens, Tailwind v4) for Blazor Web (SSR, Server, WASM, Auto) and `.NET MAUI Blazor Hybrid`.

---

## Quick Start

### Option A — Full Ecosystem (`Lux.Stack`)
```bash
dotnet add package Lux.Stack --prerelease
```
```csharp
builder.Services.AddLuxStack();
```

### Option B — Headless NativeAOT Engines Only (`Lux.Stack.Core`)
```bash
dotnet add package Lux.Stack.Core --prerelease
```

### Option C — Scaffold a Ready-to-Run Project (`Lux.Templates`)
```bash
dotnet new install Lux.Templates::0.1.0-preview.1
dotnet new lux-saas -n MySaaSApp          # Full Blazor Web App wired to Lux.Stack
dotnet new lux-headless -n MyAotWorker    # NativeAOT Console engine wired to Lux.Stack.Core
```

### Option D — AI Assistant Integration (`lux-stack-mcp`)
```bash
dotnet tool install --global Lux.Stack.Mcp --prerelease
lux-stack-mcp
```

---

## Ecosystem Directory

| Library | Headless Core (NativeAOT) | Blazor & MAUI UI | Key Primitives & Components | Live Docs |
| :--- | :--- | :--- | :--- | :--- |
| **Lux.UI** | `Lux.UI.Build.Tasks` | `Lux.UI`, `Lux.UI.Server`, `Lux.UI.Images` | 100+ `Lx*` components, Tailwind v4 tokens, icons, fonts, 12 locales | `/ui` |
| **Lux.Query** | `Lux.Query.Core` | `Lux.Query`, `Lux.Query.EntityFrameworkCore` | `DataListSchema`, 13 filter operators, facets, keyset cursors, `QueryState`, EF Core SQL | `/query/docs` |
| **Lux.Table** | `Lux.Table.Core` | `Lux.Table` | `LxDataList`, `LxDataListTable`, `LxDataListGrid`, grouping, trees, summaries, saved views | `/table/docs` |
| **Lux.Form** | `Lux.Form.Core` | `Lux.Form` | `FormSchema`, `FormController`, `FormService`, `LxFormEngine`, `LxFormPage`, `LxFormProvider` | `/form/docs` |
| **Lux.Dashboard** | `Lux.Dashboard.Core` | `Lux.Dashboard` | `DashboardController` (3-wave staging), `DashboardFormat` CSV, `LxDashboardPage`, KPIs & charts | `/dashboard/docs` |
| **Lux.Display** | `Lux.Display.Core` | `Lux.Display` | 19-kind `LxDisplay`, `LxAudioPlayer`, `LxVideoPlayer`, `FilePreviewService`, `LxFilePreviewProvider` | `/display/docs` |
| **Lux.Spreadsheet** | `Lux.Spreadsheet.Core` | `Lux.Spreadsheet` | `SpreadsheetImporter` (NFD headers, 80% suggestions, duplicates/upsert, batch retry), `LxSpreadsheetImport` | `/spreadsheet/docs` |
| **Lux.Agent** | `Lux.Agent.Core` | `Lux.Agent` | `AgentBridgeService` (snapshot/query/act/observe, secret redaction, `AllowPolicy`), `UndoStack<T>` | `/agent/docs` |

---

## Explore Live

- **Unified 8-Library Interactive Showcase** (`/showcase`) — Revenue Ops backoffice combining Dashboard, Query, Table, Form Drawer, Spreadsheet, File Preview, AI Agent Copilot, and Lux.UI.
- **Why LuxStack?** (`/why`) — Architecture & feature comparison vs traditional monolithic Blazor suites.
- **Documentation Hub** — [Getting Started](./docs/getting-started.md) · [Architecture](./docs/architecture.md) · [API Reference](./docs/api-reference.md) · [MAUI & NativeAOT](./docs/maui-and-aot.md)

---

## Building, Testing & Packing

```bash
dotnet build
dotnet test
dotnet pack -c Release
```

## License

MIT — see [LICENSE](./LICENSE).
