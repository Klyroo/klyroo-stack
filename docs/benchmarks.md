# LuxStack Performance & Footprint Benchmarks
- Runtime: `.NET 10.0.15` (X64)
- Build: `Release` | Date: `2026-11-24`

## Runtime Benchmarks (Median)
| Scenario | Median Time | Allocated |
| :--- | ---: | ---: |
| Lux.Query InMemory — **100,000 rows** (Filter + Sort + 5 Facets) | **16.61 ms** | 7,035 KB |
| Lux.Query EF Core SQLite — **50,000 rows Deep OFFSET** (page 999) | **0.80 ms** | 40 KB |
| Lux.Query EF Core SQLite — **50,000 rows Keyset Cursor** (row 49,900+) | **0.79 ms** | 64 KB |
| Lux.Spreadsheet — **10,000 CSV rows** (Parse + NFD + Rules + Dedup) | **22.99 ms** | 17,031 KB |
| Lux.Table — **2,020-node Tree** (`BuildTree`, 3 levels) | **0.68 ms** | 568 KB |
| Lux.Display — **RFC 4180 CSV Preview** (10k input, 1k cap) | **0.30 ms** | 297 KB |

**Keyset Cursor Speedup at row 49,900 (SQLite)**: **1.0x faster** than `OFFSET`!

## Headless Core Assembly Footprint (Release IL, pre-trim / pre-brotli)
| Assembly | Size (KB) |
| :--- | ---: |
| `Lux.Agent.Core.dll` | 25.0 KB |
| `Lux.Dashboard.Core.dll` | 56.5 KB |
| `Lux.Display.Core.dll` | 33.5 KB |
| `Lux.Form.Core.dll` | 50.5 KB |
| `Lux.Query.Core.dll` | 239.5 KB |
| `Lux.Spreadsheet.Core.dll` | 92.5 KB |
| `Lux.Stack.Core.dll` | 23.5 KB |
| `Lux.Table.Core.dll` | 15.0 KB |
| **Total (all 8 Core engines)** | **536.0 KB** |
