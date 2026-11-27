using Klyroo.Display;

namespace Klyroo.Playground.Data;

/// <summary>A snippet for each language of <see cref="CodeHighlighter"/>, a few lines long so that line numbers and emphasized lines have something to show.</summary>
public static class CodeSamples
{
    public static string Of(CodeLanguage? language) => language switch
    {
        CodeLanguage.Bash => "#!/usr/bin/env bash\nset -euo pipefail\n\nfor file in \"$@\"; do\n  echo \"Processing ${file}\" | tee -a build.log\ndone\n",
        CodeLanguage.CSharp => "public record Order(int Id, decimal Total)\n{\n    public string Label => $\"Order {Id}\";\n\n    public bool IsLarge(decimal threshold = 100m) => Total > threshold;\n}\n",
        CodeLanguage.Css => ".card {\n  display: grid;\n  gap: 0.75rem;\n  padding: 1rem;\n  border: 1px solid var(--line);\n  border-radius: 8px;\n}\n",
        CodeLanguage.Html => "<section class=\"card\">\n  <h2 id=\"title\">Orders</h2>\n  <button type=\"button\" disabled>Export</button>\n  <!-- the list is rendered by Blazor -->\n</section>\n",
        CodeLanguage.JavaScript => "const total = orders\n  .filter((order) => order.status === \"paid\")\n  .reduce((sum, order) => sum + order.total, 0);\n\nconsole.log(`Paid: ${total}`);\n",
        CodeLanguage.Json => "{\n  \"name\": \"orders\",\n  \"private\": true,\n  \"retries\": 3,\n  \"tags\": [\"billing\", null]\n}\n",
        CodeLanguage.Less => "@accent: #ec4899;\n\n.button {\n  color: @accent;\n  &:hover { color: darken(@accent, 10%); }\n}\n",
        CodeLanguage.Python => "def total(orders):\n    \"\"\"Sum of the paid orders.\"\"\"\n    return sum(o.total for o in orders if o.status == \"paid\")\n\nprint(total(load_orders()))\n",
        CodeLanguage.Scss => "$accent: #ec4899;\n\n.button {\n  color: $accent;\n  &:hover { color: darken($accent, 10%); }\n}\n",
        CodeLanguage.Sql => "SELECT id, total\nFROM orders\nWHERE status = 'paid'\n  AND total > 100\n  AND shipped_at IS NULL;\n",
        CodeLanguage.TypeScript => "interface Order {\n  id: number;\n  total: number;\n}\n\nexport const label = (order: Order): string => `Order ${order.id}`;\n",
        CodeLanguage.Xml => "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Project Sdk=\"Microsoft.NET.Sdk.Razor\">\n  <PropertyGroup>\n    <TargetFramework>net10.0</TargetFramework>\n  </PropertyGroup>\n</Project>\n",
        CodeLanguage.Yaml => "name: build\non: [push]\njobs:\n  test:\n    runs-on: ubuntu-latest\n    steps:\n      - run: dotnet test\n",
        _ => "Plain text has no colors:\nthe code is shown as it is,\nline numbers and emphasized lines still work.\n",
    };

    /// <summary>A file name that suits the language, for the label of the block.</summary>
    public static string FileName(CodeLanguage? language) => language switch
    {
        CodeLanguage.Bash => "build.sh",
        CodeLanguage.CSharp => "Order.cs",
        CodeLanguage.Css => "card.css",
        CodeLanguage.Html => "Orders.html",
        CodeLanguage.JavaScript => "total.js",
        CodeLanguage.Json => "package.json",
        CodeLanguage.Less => "button.less",
        CodeLanguage.Python => "orders.py",
        CodeLanguage.Scss => "button.scss",
        CodeLanguage.Sql => "Orders.sql",
        CodeLanguage.TypeScript => "order.ts",
        CodeLanguage.Xml => "App.csproj",
        CodeLanguage.Yaml => "build.yml",
        _ => "notes.txt",
    };

    /// <summary>Reads emphasized lines as people write them, <c>2, 4-5</c>: numbers and runs separated by commas. Whatever is not a number or a run is ignored.</summary>
    public static IReadOnlyList<CodeLineRange> ParseLines(string? text)
    {
        var ranges = new List<CodeLineRange>();
        foreach (var part in (text ?? "").Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var dash = part.IndexOf('-');
            if (dash < 0 && int.TryParse(part, out var one) && one > 0)
            {
                ranges.Add(new CodeLineRange(one));
            }
            else if (dash > 0 && int.TryParse(part[..dash], out var start) && int.TryParse(part[(dash + 1)..], out var end) && start > 0 && end >= start)
            {
                ranges.Add(new CodeLineRange(start, end));
            }
        }
        return ranges;
    }

    /// <summary>The ranges as the C# of a collection expression: <c>[2, new CodeLineRange(4, 5)]</c>.</summary>
    public static string Literal(IReadOnlyList<CodeLineRange> ranges) =>
        "[" + string.Join(", ", ranges.Select(r => r.Start == r.End ? r.Start.ToString() : $"new CodeLineRange({r.Start}, {r.End})")) + "]";
}
