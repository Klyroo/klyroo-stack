# Getting Started with LuxStack

Install the umbrella meta-package to pull in every library at once, or pick individual packages for minimal footprint.

```bash
dotnet add package Lux.Stack        # Full Blazor Web & .NET MAUI Hybrid ecosystem
dotnet add package Lux.Stack.Core   # Headless NativeAOT Core meta-package
```

Register all services in a single call in `Program.cs` or `MauiProgram.cs`:

```csharp
using Lux.Stack;

builder.Services.AddLuxStack();
```

Resolve any component or controller dynamically to its owning repository and package:

```csharp
var module = LuxStackCatalog.ResolveComponentModule("LxFormEngine");
// module.Name == "Lux.Form", module.CorePackage == "Lux.Form.Core", module.LandingPagePath == ".../lux-form/site/index.html"
```
