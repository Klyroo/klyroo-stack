# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

- **Primary**: .NET 10 and Blazor technical leads, application architects, and full-stack .NET developers building production SaaS products, enterprise backoffices, admin panels, and .NET MAUI Blazor Hybrid apps.
- **Secondary**: AI coding agents (Cursor, Claude Code, GitHub Copilot, MCP clients) generating and maintaining Klyroo application code via `Klyroo.Stack.Mcp` and `llms.txt`.

## Product Purpose

`klyroo-stack` is the central hub, meta-package, tooling suite, and unified documentation/showcase site for the **Klyroo** ecosystem. It exists so a .NET developer or tech lead can evaluate all 8 libraries together in under a minute, scaffold a ready-to-run project with `dotnet new`, connect their AI coding assistant via MCP, or adopt either individual libraries or the full stack via `Klyroo.Stack`.

Success means a team can install a package or template, run an interactive backoffice or static SSR app with zero build friction, and trust the architecture for NativeAOT, accessibility, and long-term MIT open-source ownership.

## Positioning

Klyroo separates every domain capability into a 100% headless, NativeAOT-compatible C# engine (`Klyroo.<Product>.Core`) paired with zero-roundtrip Blazor Web (Static SSR, Interactive Server, WebAssembly, Auto) and .NET MAUI Blazor Hybrid components (`Klyroo.<Product>`), native Tailwind CSS v4 plus precompiled zero-friction stylesheets, and first-class AI operability (`Klyroo.Agent`, `Klyroo.Stack.Mcp`, `llms.txt`) — all under a single MIT license with zero proprietary runtime dependencies.

## Operating Context

- **Meta-packages**: `Klyroo.Stack.Core` (all headless engines) and `Klyroo.Stack` (full UI stack via `builder.Services.AddKlyrooStack()`).
- **Scaffolding & templates**: `Klyroo.Templates` (`dotnet new install Klyroo.Templates`) shipping `klyroo-saas`, `klyroo-admin`, `klyroo-headless`, and `klyroo-maui`.
- **AI tooling**: `Klyroo.Stack.Mcp` global .NET tool (`klyroo-stack-mcp` over stdio), `Klyroo.Stack.DocsBundle`, and `llms.txt` endpoints.
- **Unified site**: `site/Klyroo.Stack.Site` + `site/Klyroo.Stack.Site.Client` (ASP.NET Core `.NET 10` Blazor Interactive Auto + Static SSR), hosting the ecosystem landing (`/`), live 8-library Revenue Ops Backoffice (`/showcase`), comparison and benchmarks (`/why`), individual library overviews and docs, and 9 live embedded template demos from sibling `klyroo-ui/templates/`.
- **Multi-repo workspace**: Coordinates sibling repositories `klyroo-ui`, `klyroo-query`, `klyroo-table`, `klyroo-form`, `klyroo-dashboard`, `klyroo-display`, `klyroo-spreadsheet`, and `klyroo-agent`.

## Capabilities and Constraints

- Targets `.NET 10` across Blazor Static SSR, Interactive Server, WebAssembly, Interactive Auto, and `.NET MAUI Blazor Hybrid`.
- Every `*.Core` assembly enforces `<IsAotCompatible>true</IsAotCompatible>` and `<IsTrimmable>true</IsTrimmable>` with zero UI dependencies.
- Zero-friction precompiled CSS (`klyroo-stack.css`, 40.8 KB) must work in a fresh Blazor app without Node or Tailwind installed; Tailwind CSS v4 compilation remains first-class for custom apps.
- Static SSR fallback (links, native Popover API, cookies) must remain functional where interactive circuits are absent.
- 100% MIT open-source license; no proprietary lock-in or undisclosed commercial gating.
- Strict naming contract across packages, symbols, and tokens: `Klyroo`, `Klyroo.*`, `Kly*`, `--klyroo-*`.

## Brand Commitments

- **Name**: `Klyroo` (ecosystem) and `klyroo/stack` (site wordmark).
- **Voice**: Technical, direct, verifiable, and honest — grounded in architecture, code, and measured numbers rather than marketing hype.
- **Identity assets**: `site/Klyroo.Stack.Site/wwwroot/favicon.svg`, `docs/assets/klyroo-tour.gif`, `docs/assets/klyroo-home.png`, `docs/assets/klyroo-showcase.png`, `docs/assets/klyroo-why.png`.

## Evidence on Hand

- **Automated test suite**: 4,853 passing tests across the 9 repositories (4,800 .NET unit/bUnit/EF Core tests + 53 JS runtime tests).
- **Measured benchmarks** (`benchmarks/Klyroo.Stack.Benchmarks`, documented in `docs/benchmarks.md`, `.NET 10.0` Release x64):
  - `Klyroo.Query.Core` 100k rows (filter + sort + 5 facets): `16.6 ms` median.
  - `Klyroo.Spreadsheet.Core` 10k CSV rows (NFD + dedup): `23.0 ms` median.
  - Combined headless `*.Core` DLLs (pre-trim): `536 KB`.
  - Precompiled `klyroo-stack.css`: `40.8 KB`.
- **Interactive demonstrations**: Live 8-library Revenue Ops Backoffice at `/showcase` and 9 live runnable template demos (`landing`, `saas`, `dashboard`, `docs`, `changelog`, `editor`, `chat`, `portfolio`, `starter`).
- **Accessibility verification**: Automated WCAG/axe-core gate in `build/verify-a11y.mjs` and CI workflows.
- **Packaging**: `v0.1.0-preview.1` NuGet packages and `.snupkg` symbols prepared in CI release workflows.
- **Explicit absences (do not fabricate)**: No NuGet download counts, no third-party customer logos, no user testimonials, and no external production case studies exist yet.

## Product Principles

1. **Headless first, UI second**: Business logic, ASTs, validation, and orchestration live in NativeAOT `*.Core` assemblies; Blazor components remain thin, composable shells.
2. **Every Blazor render mode is first-class**: Static SSR, Interactive Server, WebAssembly, Interactive Auto, and MAUI Blazor Hybrid are all supported without forcing a single hosting model.
3. **Zero-friction adoption, incremental scale**: Developers can adopt one `*.Core` package in a Minimal API, one UI package with precompiled CSS, or the entire stack via `Klyroo.Stack`.
4. **Proof over claims**: Every performance figure, capability comparison, and demo is backed by runnable code, benchmarks, and tests inside the repository.
5. **Agent-operable by construction**: Documentation, API schemas, and live UI controls are machine-readable through `Klyroo.Stack.Mcp`, `llms.txt`, and `Klyroo.Agent`.

## Accessibility & Inclusion

- Durable standard: **WCAG 2.1 AA** across `Klyroo.Stack.Site` and all UI packages, verified via automated axe-core checks (`build/verify-a11y.mjs`) in CI.
- Semantic HTML landmarks (`<main id="main">`, skip links, proper table scopes, ARIA states), full keyboard navigation, and safe focus management on overlays/dialogs are mandatory.
