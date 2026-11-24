# LuxStack (`lux.stack`)

> **Modular headless-first architecture, engineered for .NET 10, Blazor, and .NET MAUI.**

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

## Hub Documentation

- [Getting Started with LuxStack](./docs/getting-started.md)
- [Architecture & Multi-Repo Strategy](./docs/architecture.md)
- [Catalog & Component Routing API](./docs/api-reference.md)
- [.NET MAUI & NativeAOT Matrix](./docs/maui-and-aot.md)

## Building & Testing

```bash
dotnet build
dotnet test
dotnet pack -c Release
```

## License

MIT — see [LICENSE](./LICENSE).
