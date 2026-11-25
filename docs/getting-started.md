# Getting Started with Klyroo

Install the umbrella meta-package to pull in every library at once, or pick individual packages for minimal footprint.

```bash
dotnet add package Klyroo.Stack        # Full Blazor Web & .NET MAUI Hybrid ecosystem
dotnet add package Klyroo.Stack.Core   # Headless NativeAOT Core meta-package
```

Register all services in a single call in `Program.cs` or `MauiProgram.cs`:

```csharp
using Klyroo.Stack;

builder.Services.AddKlyrooStack();
```

Resolve any component or controller dynamically to its owning repository and package:

```csharp
var module = KlyrooStackCatalog.ResolveComponentModule("KlyFormEngine");
// module.Name == "Klyroo.Form", module.CorePackage == "Klyroo.Form.Core", module.LandingPagePath == ".../klyroo-form/site/index.html"
```
