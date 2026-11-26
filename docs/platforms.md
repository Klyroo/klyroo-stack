# Platforms

What runs where. Every statement here comes from the project files and code of this repository or from the documentation of the library it concerns, and each one says which. Where something has not been run, the page says so.

## What each package declares

| Package | Kind | Platform declarations in the project file |
| :--- | :--- | :--- |
| `Klyroo.Stack.Core` | Class library, `net10.0` | `IsAotCompatible` and `IsTrimmable` are `true`. No platform restriction: it runs wherever .NET 10 runs |
| `Klyroo.Stack` | Razor class library, `net10.0` | `IsTrimmable` is `true`. `SupportedPlatform` lists `browser`, `ios`, `android`, `maccatalyst` and `windows`. `IsAotCompatible` is not set |
| `Klyroo.Stack.Mcp` | `dotnet` tool, `net10.0` | `RollForward` is `Major`, so a newer installed runtime runs it |
| `Klyroo.Templates` | Template pack | Not a library |

The libraries that the meta-packages reference declare their own settings. Every `.Core` project of the seven headless libraries sets `IsAotCompatible` and `IsTrimmable` and has no package or framework reference. Their Blazor packages set `IsTrimmable` and list the same five supported platforms, except `Klyroo.UI`, which lists `browser` only in its project file. That difference does not stop Klyroo.UI from running in a MAUI `BlazorWebView`, which hosts web content, but it is why the status table below comes from its documentation and not from the declaration.

## Render modes

The meta-package carries no render-mode code of its own. What it registers works in the modes that Klyroo.UI documents:

| Mode | Status |
| :--- | :--- |
| Static SSR | Documented as supported by Klyroo.UI. Components that need a .NET event handler (`OnClick` on `KlyButton`) do nothing until the page is interactive, so use `Href` or a form post on static pages. |
| Interactive Server | Verified when this documentation was written: the tutorial app of [Getting started](getting-started.md) prerenders and serves with `AddInteractiveServerComponents()`. |
| WebAssembly | Documented as supported by Klyroo.UI. The documentation site in `site/` runs a WebAssembly client. Register the stack in the server and in the client with the same options. |
| Auto | Documented as supported by Klyroo.UI; same registration rule as WebAssembly. |

In a mode with a WebAssembly client, call `AddKlyrooStack()` in both `Program.cs` files, ideally through one shared method (see [the guide](guides.md#share-options-between-the-server-and-the-webassembly-client)). If the server and the client register different options, components change appearance when the client takes over.

For static server rendering that reads the user's time zone (data lists cut days in the user's zone), the site registers `AddKlyrooServer()` from the `Klyroo.UI.Server` package. `Klyroo.Stack` does not include that package: add it when you render data lists statically.

## .NET MAUI Blazor Hybrid

`Klyroo.Stack` declares the four MAUI platforms, and the `klyroo-maui` template registers it in `MauiProgram.cs`:

```csharp
using Klyroo.Stack;

builder.Services.AddMauiBlazorWebView();
builder.Services.AddKlyrooStack();
```

The Klyroo.UI documentation (the summary that this repository's `llms.txt` serves) states the current status:

- Android has been tested on an emulator.
- iOS 17 or later, Windows and macOS (Mac Catalyst) have CI build jobs that have not run yet, and they are untested on devices.
- Android TV is experimental and Tizen is not supported.
- The local image optimizer of Klyroo.UI is web only.

None of it was run on a MAUI target while this page was written: the MAUI workloads for the iOS and Mac Catalyst targets were not available on the machine.

The `klyroo-maui` template is a complete MAUI Blazor Hybrid app built on the .NET `maui-blazor` template: `App`, a `MainPage` that hosts the `BlazorWebView` with a `HeadOutlet` root component for `KlyHead`, a layout with `KlyApp`, `KlyFormProvider` and `KlyFilePreviewProvider` (see [the guide](guides.md#open-a-form-or-a-file-preview-in-the-maui-template)), an `index.html` that loads the Klyroo stylesheets and an `app.css`, the platform folders, and an icon and splash screen. It targets `net10.0-android`, plus `net10.0-ios` and `net10.0-maccatalyst` outside Linux and a Windows target on Windows, like the .NET template. A generated project was built for Android; the other targets need their workloads and were not built.

## NativeAOT and trimming

- **`Klyroo.Stack.Core` and the `.Core` engines** are marked `IsAotCompatible` and `IsTrimmable`. CI builds each `src/*.Core` project of this repository with `EnableAotAnalyzer` and `EnableTrimAnalyzer`, and the `klyroo-headless` template was published with `PublishAot` on linux-x64 and ran: it executes a query, a spreadsheet import, a CSV preview, a form controller, a KPI format and an undo stack from one native binary.
- **`Klyroo.Stack`** is `IsTrimmable` but not declared AOT compatible. Blazor apps are not NativeAOT-published; the trimming flag matters for the trimmed WebAssembly build.
- **Headless schemas.** The `klyroo-headless` template builds a `DataListSchema` from Klyroo.Query, a typed `SpreadsheetSchema` and a typed `FormSchema` and runs them under `PublishAot`, so those paths work in a native binary: the schemas read the members of your own classes through expressions that are never compiled. `Klyroo.Form.DataAnnotations` is trim-safe but not NativeAOT-safe, since it reads attributes by reflection. For the rest of the API, see the repository of each library.

## Limitations

- The meta-package does not include `Klyroo.Query.EntityFrameworkCore`, `Klyroo.UI.Server`, `Klyroo.UI.Images` or `Klyroo.Form.DataAnnotations`.
- The MCP server runs on the .NET runtime, not as a native binary.
- The tool and the templates require the .NET 10 runtime or newer.
