# Klyroo (`klyroo-stack`)

*Part of [Klyroo](https://klyroo.dev): Build better. Compose freely.*

[![CI](https://github.com/klyroo/klyroo-stack/actions/workflows/ci.yml/badge.svg)](https://github.com/klyroo/klyroo-stack/actions/workflows/ci.yml) [![NuGet](https://img.shields.io/badge/NuGet-0.1.0--preview.1-004880.svg)](https://www.nuget.org/packages/Klyroo.Stack) [![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/) [![NativeAOT](https://img.shields.io/badge/NativeAOT-Compatible-10b981.svg)](https://klyroo.dev/why) [![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](./LICENSE)

> **Modular, headless-first Blazor & .NET 10 component ecosystem — engineered for Static SSR, Interactive Server, WebAssembly, .NET MAUI Blazor Hybrid, and NativeAOT.**

![Klyroo Interactive Tour](./docs/assets/klyroo-tour.gif)

**Klyroo** is an ecosystem of **8 independent, composable libraries** united under one architecture:

- **`Klyroo.<Product>.Core`** — 100% headless C#, zero UI dependencies, `<IsAotCompatible>true</IsAotCompatible>`, `<IsTrimmable>true</IsTrimmable>`.
- **`Klyroo.<Product>`** — Multiplatform Blazor UI (`Kly*` components, `--klyroo-*` CSS tokens, Tailwind v4) for Blazor Web (SSR, Server, WASM, Auto) and `.NET MAUI Blazor Hybrid`.

---

## Quick Start

### Option A — Full Ecosystem (`Klyroo.Stack`)
```bash
dotnet add package Klyroo.Stack --prerelease
```
```csharp
builder.Services.AddKlyrooStack();
```

### Option B — Headless NativeAOT Engines Only (`Klyroo.Stack.Core`)
```bash
dotnet add package Klyroo.Stack.Core --prerelease
```

### Option C — Scaffold a Ready-to-Run Project (`Klyroo.Templates`)
```bash
dotnet new install Klyroo.Templates::0.1.0-preview.1
dotnet new klyroo-saas -n MySaaSApp          # Blazor Web App wired to Klyroo.Stack + precompiled CSS
dotnet new klyroo-admin -n MyBackoffice      # Admin Backoffice (Dashboard + Table + Form + Preview)
dotnet new klyroo-headless -n MyAotWorker    # NativeAOT Console engine wired to Klyroo.Stack.Core
dotnet new klyroo-maui -n MyMobileApp        # .NET MAUI Blazor Hybrid (iOS, Android, macOS, Windows)
```

### Zero-Friction CSS (No Node or Tailwind Required)
Every UI package ships a precompiled, minified stylesheet in `wwwroot/`:
```html
<link rel="stylesheet" href="_content/Klyroo.UI/klyroo.preflight.css" />
<link rel="stylesheet" href="_content/Klyroo.UI/klyroo.css" />
<link rel="stylesheet" href="_content/Klyroo.Stack/klyroo-stack.css" />
<!-- Or individually: _content/Klyroo.Table/klyroo-table.css, _content/Klyroo.Form/klyroo-form.css, etc. -->
```

### Measured Benchmarks (`.NET 10` Release — [Full Report](./docs/benchmarks.md))
| Scenario | Median | Footprint |
| :--- | ---: | ---: |
| `Klyroo.Query` InMemory — **100,000 rows** (Filter + Sort + 5 Facets) | **16.61 ms** | 239.5 KB (`Klyroo.Query.Core.dll`) |
| `Klyroo.Spreadsheet` — **10,000 CSV rows** (Parse + NFD + Rules + Dedup) | **22.99 ms** | 92.5 KB (`Klyroo.Spreadsheet.Core.dll`) |
| `Klyroo.Table` — **2,020-node Tree** (`BuildTree`, 3 levels) | **0.68 ms** | 15.0 KB (`Klyroo.Table.Core.dll`) |
| `Klyroo.Display` — **RFC 4180 CSV Preview** (10k input, 1k cap) | **0.30 ms** | 33.5 KB (`Klyroo.Display.Core.dll`) |
| **All 8 Headless Core Engines Combined** | — | **536.0 KB** (pre-trim / pre-brotli) |

### Option D — AI Assistant Integration (`klyroo-stack-mcp`)
```bash
dotnet tool install --global Klyroo.Stack.Mcp --prerelease
klyroo-stack-mcp
```

---

## Ecosystem Directory

| Library | Headless Core (NativeAOT) | Blazor & MAUI UI | Key Primitives & Components | Live Docs |
| :--- | :--- | :--- | :--- | :--- |
| **Klyroo.UI** | `Klyroo.UI.Build.Tasks` | `Klyroo.UI`, `Klyroo.UI.Server`, `Klyroo.UI.Images` | 100+ `Kly*` components, Tailwind v4 tokens, icons, fonts, 12 locales | `/ui` |
| **Klyroo.Query** | `Klyroo.Query.Core` | `Klyroo.Query`, `Klyroo.Query.EntityFrameworkCore` | `DataListSchema`, 13 filter operators, facets, keyset cursors, `QueryState`, EF Core SQL | `/query/docs` |
| **Klyroo.Table** | `Klyroo.Table.Core` | `Klyroo.Table` | `KlyDataList`, `KlyDataListTable`, `KlyDataListGrid`, grouping, trees, summaries, saved views | `/table/docs` |
| **Klyroo.Form** | `Klyroo.Form.Core` | `Klyroo.Form` | `FormSchema`, `FormController`, `FormService`, `KlyFormEngine`, `KlyFormPage`, `KlyFormProvider` | `/form/docs` |
| **Klyroo.Dashboard** | `Klyroo.Dashboard.Core` | `Klyroo.Dashboard` | `DashboardController` (3-wave staging), `DashboardFormat` CSV, `KlyDashboardPage`, KPIs & charts | `/dashboard/docs` |
| **Klyroo.Display** | `Klyroo.Display.Core` | `Klyroo.Display` | 19-kind `KlyDisplay`, `KlyAudioPlayer`, `KlyVideoPlayer`, `FilePreviewService`, `KlyFilePreviewProvider` | `/display/docs` |
| **Klyroo.Spreadsheet** | `Klyroo.Spreadsheet.Core` | `Klyroo.Spreadsheet` | `SpreadsheetImporter` (NFD headers, 80% suggestions, duplicates/upsert, batch retry), `KlySpreadsheetImport` | `/spreadsheet/docs` |
| **Klyroo.Agent** | `Klyroo.Agent.Core` | `Klyroo.Agent` | `AgentBridgeService` (snapshot/query/act/observe, secret redaction, `AllowPolicy`), `UndoStack<T>` | `/agent/docs` |

---

## Explore Live

- **Unified 8-Library Interactive Showcase** (`/showcase`) — Revenue Ops backoffice combining Dashboard, Query, Table, Form Drawer, Spreadsheet, File Preview, AI Agent Copilot, and Klyroo.UI.
- **Why Klyroo?** (`/why`) — Architecture & feature comparison vs traditional monolithic Blazor suites.
- **Documentation Hub** — [Getting Started](./docs/getting-started.md) · [Architecture](./docs/architecture.md) · [API Reference](./docs/api-reference.md) · [MAUI & NativeAOT](./docs/maui-and-aot.md)

---

## Building, Testing & Packing

```bash
dotnet build
dotnet test
dotnet pack -c Release
```

## Contributing

Issues and pull requests are welcome. Read [CONTRIBUTING.md](./CONTRIBUTING.md) for the workflow (Conventional
Commits, tests, documentation), follow the [Code of Conduct](./CODE_OF_CONDUCT.md), and report vulnerabilities
privately as described in [SECURITY.md](./SECURITY.md).

## Migrating from Lux

This library was previously named `lux.stack`. [MIGRATION.md](./MIGRATION.md) lists every renamed package, namespace,
component, CSS token and build setting.

## License

MIT — see [LICENSE](./LICENSE).
