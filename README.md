# LuxStack (`lux.stack`)

> **The TanStack architecture, engineered for .NET 10, Blazor, and .NET MAUI.**

**LuxStack** is a modular ecosystem of 8 specialized repositories sharing a strict two-layer architecture:
- **`Lux.<Product>.Core`** — 100% headless C#, zero UI dependencies, `<IsAotCompatible>true</IsAotCompatible>`, `<IsTrimmable>true</IsTrimmable>`.
- **`Lux.<Product>`** — Multiplatform Blazor Web (Static SSR, Interactive Server, WebAssembly, Auto) and `.NET MAUI Blazor Hybrid` (`browser`, `ios`, `android`, `maccatalyst`, `windows`).

## Install the Entire Stack (or Pick One Library)

```bash
dotnet add package Lux.Stack        # Full Blazor Web + .NET MAUI Hybrid suite
dotnet add package Lux.Stack.Core   # All 7 headless NativeAOT Core engines + LuxStackCatalog
```

```csharp
builder.Services.AddLuxStack();
```

## Ecosystem Directory & Exhaustive Documentation

| Library | Headless Core (NativeAOT) | Blazor & MAUI UI | Documentation & Landing Page |
| :--- | :--- | :--- | :--- |
| **Lux.UI** | `Blux.UI.Build.Tasks` | `Blux.UI` | [README](../blux-ui/README.md) · [Playground Docs](../blux-ui/playground/Blux.Playground/Blux.Playground.Client/Pages/Docs/Introduction.razor) · [Landing](../blux-ui/site/index.html) |
| **Lux.Query** | `Lux.Query.Core` | `Lux.Query` | [README](../lux-query/README.md) · [Docs](../lux-query/docs/getting-started.md) · [API](../lux-query/docs/api-reference.md) · [Landing](../lux-query/site/index.html) |
| **Lux.Table** | `Lux.Table.Core` | `Lux.Table` | [README](../lux-table/README.md) · [Docs](../lux-table/docs/getting-started.md) · [API](../lux-table/docs/api-reference.md) · [Landing](../lux-table/site/index.html) |
| **Lux.Form** | `Lux.Form.Core` | `Lux.Form` | [README](../lux-form/README.md) · [Docs](../lux-form/docs/getting-started.md) · [API](../lux-form/docs/api-reference.md) · [Landing](../lux-form/site/index.html) |
| **Lux.Dashboard** | `Lux.Dashboard.Core` | `Lux.Dashboard` | [README](../lux-dashboard/README.md) · [Docs](../lux-dashboard/docs/getting-started.md) · [API](../lux-dashboard/docs/api-reference.md) · [Landing](../lux-dashboard/site/index.html) |
| **Lux.Display** | `Lux.Display.Core` | `Lux.Display` | [README](../lux-display/README.md) · [Docs](../lux-display/docs/getting-started.md) · [API](../lux-display/docs/api-reference.md) · [Landing](../lux-display/site/index.html) |
| **Lux.Spreadsheet** | `Lux.Spreadsheet.Core` | `Lux.Spreadsheet` | [README](../lux-spreadsheet/README.md) · [Docs](../lux-spreadsheet/docs/getting-started.md) · [API](../lux-spreadsheet/docs/api-reference.md) · [Landing](../lux-spreadsheet/site/index.html) |
| **Lux.Agent** | `Lux.Agent.Core` | `Lux.Agent` | [README](../lux-agent/README.md) · [Docs](../lux-agent/docs/getting-started.md) · [API](../lux-agent/docs/api-reference.md) · [Landing](../lux-agent/site/index.html) |

## Hub Documentation

- [Getting Started with LuxStack](./docs/getting-started.md)
- [Architecture & Multi-Repo Strategy](./docs/architecture.md)
- [Catalog & Component Routing API](./docs/api-reference.md)
- [.NET MAUI & NativeAOT Matrix](./docs/maui-and-aot.md)
- [Interactive Hub Landing Page](./site/index.html)

## Building & Testing

```bash
dotnet build
dotnet test
dotnet pack -c Release
```

## License

MIT — see [LICENSE](./LICENSE).
