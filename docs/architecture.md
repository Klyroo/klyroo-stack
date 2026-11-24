# LuxStack Architecture

LuxStack splits an enterprise UI & data platform into 8 independent, composable repositories:

1. **Zero UI lock-in**: Every `.Core` assembly (`Lux.Query.Core`, `Lux.Table.Core`, `Lux.Form.Core`, `Lux.Dashboard.Core`, `Lux.Display.Core`, `Lux.Spreadsheet.Core`, `Lux.Agent.Core`, `Lux.Stack.Core`) compiles cleanly with `<IsAotCompatible>true</IsAotCompatible>` and zero dependency on Razor or ASP.NET Core.
2. **Consistent Blazor & MAUI Hybrid support**: Every UI RCL declares `<SupportedPlatform>` for `browser`, `ios`, `android`, `maccatalyst`, and `windows`.
3. **Zero-roundtrip DOM interactions**: Primitives in `Lux.UI` and behaviors in `Lux.Agent` run popovers, dialogs, focus traps, and roving tabindex directly in the browser without SignalR latency.
4. **AI-native by design**: `Lux.Agent` bridges `Lux.Form`, `Lux.Dashboard`, and `Lux.Spreadsheet` to AI agents and WebMCP (`navigator.modelContext`) with automatic `[redacted]` secret protection.
