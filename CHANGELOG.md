# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.0-preview.1] - 2026-11-24

### Added

- `Klyroo.Stack` meta-package for Blazor Web and .NET MAUI Blazor Hybrid: `AddKlyrooStack()` registers Klyroo.UI, Query, Table, Form, Dashboard, Display, Spreadsheet and Agent in one call and accepts the option callbacks of Klyroo.UI and Klyroo.Table.
- `Klyroo.Stack.Core` headless meta-package that references the seven `.Core` engines, marked `IsAotCompatible` and `IsTrimmable`.
- `KlyrooStackCatalog`, which maps a component or controller name to the library, packages, template short name and agent skill that own it.
- `Klyroo.Stack.Mcp`, a `dotnet` tool (`klyroo-stack-mcp`) that serves the Klyroo.UI documentation to MCP clients over stdio: nine read-only tools, two prompts and four resources, with `--tools` and `--site-url` options.
- `Klyroo.Templates` with the `klyroo-saas`, `klyroo-admin`, `klyroo-headless` and `klyroo-maui` project templates.
- `klyroo-stack.css`, a precompiled stylesheet shipped with `Klyroo.Stack`, so apps need neither Node nor Tailwind.
- The `klyroo-stack` agent skill, which routes a coding agent to the skill of each library.

[Unreleased]: https://github.com/klyroo/klyroo-stack/compare/v0.1.0-preview.1...HEAD
[0.1.0-preview.1]: https://github.com/klyroo/klyroo-stack/releases/tag/v0.1.0-preview.1
