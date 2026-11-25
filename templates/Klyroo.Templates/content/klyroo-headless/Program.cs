using System.Globalization;
using Klyroo.Agent;
using Klyroo.Dashboard;
using Klyroo.Display;
using Klyroo.Form;
using Klyroo.Query;
using Klyroo.Spreadsheet;
using Klyroo.Stack;

// 1. Klyroo.Query.Core (NativeAOT DataListSchema)
var schema = DataListSchema.For<Item>(s => s.RowKey(i => i.Id).Search(i => i.Name).Field(i => i.Price, f => f.Key("price").Sortable()));
var page = schema.ExecuteInMemory([new Item(1, "Keyboard", 120m), new Item(2, "Monitor", 350m)],
    new DataListRequest(DataListPaging.Offset.ForPage(1, 10), [new DataListSort("price", Descending: true)], DataListFilterGroup.Empty));

// 2. Klyroo.Spreadsheet.Core + Klyroo.Display.Core
var importer = new SpreadsheetImporter(new SpreadsheetSchema
{
    Key = "items",
    Columns = [new SpreadsheetColumnSchema { Key = "name", Label = "Name", Required = true }]
});
importer.LoadText("Name\nAlpha\nBeta", "items.csv");
var csv = FilePreviewService.ParseCsv("Name,Price\nKeyboard,120\nMonitor,350");

// 3. Klyroo.Form.Core + Klyroo.Dashboard.Core + Klyroo.Agent.Core
var form = new FormController(new FormSchema { FormKey = "cfg", Fields = [new FormFieldSchema { Key = "org", Label = "Org", DefaultValue = "Klyroo" }] });
var formatted = DashboardFormat.Format(125_000, DashboardFormatPreset.Compact, CultureInfo.InvariantCulture);
var undo = new UndoStack<string>("v1");
undo.Record("v2");

Console.WriteLine($"Klyroo NativeAOT OK | Modules={KlyrooStackCatalog.Modules.Count} | Top={page.Items[0].Name} | Sheet={importer.ImportableRows.Count} | CSV={csv.Rows.Count} | Form={form.GetValue<string>("org")} | KPI={formatted} | Undo={undo.Undo()}");

sealed record Item(int Id, string Name, decimal Price);
