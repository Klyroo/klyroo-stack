# Klyroo Architecture

Klyroo splits an enterprise UI & data platform into 8 independent, composable repositories:

1. **Zero UI lock-in**: Every `.Core` assembly (`Klyroo.Query.Core`, `Klyroo.Table.Core`, `Klyroo.Form.Core`, `Klyroo.Dashboard.Core`, `Klyroo.Display.Core`, `Klyroo.Spreadsheet.Core`, `Klyroo.Agent.Core`, `Klyroo.Stack.Core`) compiles cleanly with `<IsAotCompatible>true</IsAotCompatible>` and zero dependency on Razor or ASP.NET Core.
2. **Consistent Blazor & MAUI Hybrid support**: Every UI RCL declares `<SupportedPlatform>` for `browser`, `ios`, `android`, `maccatalyst`, and `windows`.
3. **Zero-roundtrip DOM interactions**: Primitives in `Klyroo.UI` and behaviors in `Klyroo.Agent` run popovers, dialogs, focus traps, and roving tabindex directly in the browser without SignalR latency.
4. **AI-native by design**: `Klyroo.Agent` bridges `Klyroo.Form`, `Klyroo.Dashboard`, and `Klyroo.Spreadsheet` to AI agents and WebMCP (`navigator.modelContext`) with automatic `[redacted]` secret protection.
