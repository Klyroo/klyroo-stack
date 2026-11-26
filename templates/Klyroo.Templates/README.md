# Klyroo.Templates

`dotnet new` templates that start a project on the Klyroo libraries: Klyroo.UI components, data lists, forms, dashboards, display blocks, spreadsheet import and the agent bridge, wired together by `Klyroo.Stack`.

```bash
dotnet new install Klyroo.Templates
dotnet new klyroo-saas -o MyApp
cd MyApp
dotnet run
```

| Template | Short name | What you get |
| --- | --- | --- |
| Klyroo SaaS Web App | `klyroo-saas` | A Blazor Web App for a SaaS product: a header shell, a dashboard, a filterable accounts list with a form drawer, and a usage page. |
| Klyroo Admin Backoffice | `klyroo-admin` | A Blazor Web App for a back office: a sidebar shell, a dashboard, a customer list with bulk actions, a customer page and a spreadsheet import. |
| Klyroo Headless NativeAOT App | `klyroo-headless` | A console app that uses the headless `.Core` engines and publishes with NativeAOT. |
| Klyroo .NET MAUI Blazor Hybrid App | `klyroo-maui` | A .NET MAUI Blazor Hybrid app for Android, iOS, Mac Catalyst and Windows. |

The two web templates take `--interactivity Server|WebAssembly|Auto` (a `.Client` project is generated for the last two) and `--product-name "Acme Cloud"`. They load the precompiled Klyroo stylesheets, so they need neither Node nor a Tailwind build.

Requirements: the .NET 10 SDK, and for `klyroo-maui` the .NET MAUI workload (`dotnet workload install maui`).

Documentation: [Klyroo.Stack on GitHub](https://github.com/klyroo/klyroo-stack), with the [guide to the templates](https://github.com/klyroo/klyroo-stack/blob/main/docs/guides.md).
