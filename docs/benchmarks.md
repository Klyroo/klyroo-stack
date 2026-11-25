# Benchmarks

Measurements of the headless engines, produced by the program in `benchmarks/Klyroo.Stack.Benchmarks`. They are single-process timings on one machine, meant to show orders of magnitude and to catch regressions, not to compare libraries. Run them yourself before relying on any number.

## Reproduce

The benchmark project is not part of the solution. It references the sibling repositories, so clone them next to this repository first (see [CONTRIBUTING.md](../CONTRIBUTING.md)), then:

```bash
dotnet build benchmarks/Klyroo.Stack.Benchmarks -c Release
dotnet benchmarks/Klyroo.Stack.Benchmarks/bin/Release/net10.0/Klyroo.Stack.Benchmarks.dll
```

The program prints a Markdown report to standard output. Each scenario runs once as a warm-up, then a fixed number of times (5 to 25, depending on the scenario), and the report shows the median time and the bytes allocated by the last run.

## Results

Measured on 2026-11-25 with .NET 10.0.15 (X64, Release) on an Intel Core Ultra 9 185H under WSL2 on Linux. Other hardware and other runs give different numbers; the two sub-millisecond rows change by a factor of two or three between runs.

| Scenario | Median time | Allocated |
| :--- | ---: | ---: |
| Klyroo.Query in memory: 100,000 rows, filter, sort and 5 facets | 16.25 ms | 7,035 KB |
| Klyroo.Query on EF Core SQLite: 50,000 rows, offset paging at page 999 | 1.04 ms | 40 KB |
| Klyroo.Query on EF Core SQLite: 50,000 rows, keyset cursor from row 49,900 | 0.35 ms | 64 KB |
| Klyroo.Spreadsheet: 10,000 CSV rows, parse, header normalization, rules and duplicate check | 19.94 ms | 17,029 KB |
| Klyroo.Table: `BuildTree` on 2,020 nodes, 3 levels | 0.54 ms | 568 KB |
| Klyroo.Display: `FilePreviewService.ParseCsv`, 10,000-row input capped at 1,000 rows | 0.28 ms | 297 KB |

The two SQLite rows are single queries on an in-memory database that finish in about a millisecond or less. Two runs of this program gave 1.0 ms and 0.8 ms for offset paging and 0.35 ms and 0.79 ms for the cursor, so the difference between the two rows is within run-to-run variation here and the report should not be read as a speed-up claim.

## Assembly size

Size of the compiled `.Core` assemblies in the benchmark output folder (Release IL, before trimming and compression):

| Assembly | Size |
| :--- | ---: |
| `Klyroo.Agent.Core.dll` | 25.0 KB |
| `Klyroo.Dashboard.Core.dll` | 56.5 KB |
| `Klyroo.Display.Core.dll` | 34.0 KB |
| `Klyroo.Form.Core.dll` | 50.5 KB |
| `Klyroo.Query.Core.dll` | 239.5 KB |
| `Klyroo.Spreadsheet.Core.dll` | 92.5 KB |
| `Klyroo.Stack.Core.dll` | 23.0 KB |
| `Klyroo.Table.Core.dll` | 15.0 KB |
| Total, 8 assemblies | 536.0 KB |

The sizes change with every build of the sibling repositories.
