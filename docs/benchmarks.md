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

Measured on 2026-11-26 with .NET 10.0.15 (X64, Release) on an Intel Core Ultra 9 185H under WSL2 on Linux. Each figure is the median of eight complete runs of the program. Other hardware and other runs give different numbers; the sub-millisecond rows change by a factor of two between runs, and the machine was shared, so read the spreadsheet row against the comparison below rather than against another machine.

| Scenario | Median time | Allocated |
| :--- | ---: | ---: |
| Klyroo.Query in memory: 100,000 rows, filter, sort and 5 facets | 12.45 ms | 7,035 KB |
| Klyroo.Query on EF Core SQLite: 50,000 rows, offset paging at page 999 | 0.74 ms | 40 KB |
| Klyroo.Query on EF Core SQLite: 50,000 rows, keyset cursor from row 49,900 | 0.52 ms | 56 KB |
| Klyroo.Spreadsheet: 10,000 CSV rows, parse, header normalization, rules and duplicate check | 24.86 ms | 12,373 KB |
| Klyroo.Table: `BuildTree` on 2,020 nodes, 3 levels | 0.60 ms | 568 KB |
| Klyroo.Display: `FilePreviewService.ParseCsv`, 10,000-row input capped at 1,000 rows | 0.33 ms | 297 KB |

The two SQLite rows are single queries on an in-memory database that finish in about a millisecond or less. Across the eight runs the offset row ranged from 0.62 to 1.12 ms and the cursor row from 0.41 to 0.78 ms, so the difference between the two rows is within run-to-run variation here and the report should not be read as a speed-up claim.

### The spreadsheet scenario and the typed columns

When the spreadsheet importer moved to typed rows, this scenario got slower, and it was profiled with `dotnet-trace`. A typed number column read each cell three times: a regular expression to check its shape, a split of the text into sign, digits and exponent to know whether it was a number, and a second split plus a read back through the text of the decimal to prove that no digit had been rounded. Each pass allocated, and the decimal was formatted twice. The importer now cleans the cell into a stack buffer, checks its shape with a scan that gives the same answers as the old patterns, splits it once without allocating, reads it into the property type, and reads a decimal back only when its text has more than 28 digits. The parsed values of a row also travel in an array in column order instead of a dictionary, so building the typed row and finding the row key no longer hash a string per column. The row is still built at load, so an exception of the host still becomes a row issue.

The three versions measured back to back on the same machine, ten interleaved runs of this program, median time and allocation of the spreadsheet scenario:

| Version | Median time | Allocated |
| :--- | ---: | ---: |
| Before the typed libraries (untyped rows) | 36.70 ms | 14,226 KB |
| Typed libraries, before the fix | 52.08 ms | 17,598 KB |
| Typed libraries, after the fix | 27.27 ms | 12,373 KB |

The typed read is now 0.74 times the time of the untyped one, and allocates 13 percent less. The absolute times of this table are higher than the ones above because the three versions ran in turn while other work shared the machine.

## Assembly size

Size of the compiled `.Core` assemblies in the benchmark output folder (Release IL, before trimming and compression), now and with the libraries before the typing work, both built the same day:

| Assembly | Before typing | Now |
| :--- | ---: | ---: |
| `Klyroo.Agent.Core.dll` | 33.5 KB | 68.5 KB |
| `Klyroo.Dashboard.Core.dll` | 82.5 KB | 85.5 KB |
| `Klyroo.Display.Core.dll` | 67.5 KB | 68.0 KB |
| `Klyroo.Form.Core.dll` | 125.0 KB | 300.5 KB |
| `Klyroo.Query.Core.dll` | 242.5 KB | 243.0 KB |
| `Klyroo.Spreadsheet.Core.dll` | 179.0 KB | 215.5 KB |
| `Klyroo.Stack.Core.dll` | 23.0 KB | 27.0 KB |
| `Klyroo.Table.Core.dll` | 24.5 KB | 24.5 KB |
| Total, 8 assemblies | 777.5 KB | 1,032.5 KB |

The sizes change with every build of the sibling repositories. The growth is three assemblies: `Klyroo.Form.Core` (the typed schema, the codecs and the Dynamic mode next to the engine), `Klyroo.Spreadsheet.Core` (the typed columns and builders) and `Klyroo.Agent.Core` (the typed records of the bridge). Intermediate language is only a third of it. The rest is metadata: `Klyroo.Form.Core` went from 825 to about 2,300 methods, and about 1,400 of them are under 20 bytes of IL, which are property accessors, constructors and the members the compiler writes for records. Public records and properties are the typed API, so they stay. Eight internal helper types that nothing compares or prints (seven that were records, and the new number reader) are plain types, which saved about 10 KB and paid for the new reading of number cells, so the total went from 1,038.5 KB when the typed libraries were first measured to 1,032.5 KB. The rest of the growth is new code: the dead internal code the assemblies still hold is at most about 8 KB of IL.
