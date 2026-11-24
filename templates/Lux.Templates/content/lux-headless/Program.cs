using Lux.Query;

var schema = DataListSchema.For<Item>(s => s.RowKey(i => i.Id).Search(i => i.Name).Field(i => i.Price, f => f.Key("price").Sortable()));
var page = schema.ExecuteInMemory([new Item(1, "Keyboard", 120m), new Item(2, "Monitor", 350m)],
    new DataListRequest(DataListPaging.Offset.ForPage(1, 10), [new DataListSort("price", Descending: true)], DataListFilterGroup.Empty));

Console.WriteLine($"LuxStack NativeAOT matched {page.TotalCount} items. Top: {page.Items[0].Name}");
sealed record Item(int Id, string Name, decimal Price);
