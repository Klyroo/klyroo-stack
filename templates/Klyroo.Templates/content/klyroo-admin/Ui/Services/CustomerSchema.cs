using Klyroo.Query;

namespace KlyrooAdminApp.Services;

/// <summary>What the customers list may ask for: which fields sort, which filter, the default sort and the page size.</summary>
public static class CustomerSchema
{
    public static readonly DataListSchema<Customer> Schema = DataListSchema.For<Customer>(s => s
        .RowKey(c => c.Id)
        .RowLabel(c => c.Name)
        .Search(c => c.Name, c => c.Contact)
        .Field(c => c.Name, f => f.Key("name").Sortable())
        .Field(c => c.Region, f => f.Key("region").Sortable().Filter(DataListFilter.Option().WithFacets()))
        .Field(c => c.Tier, f => f.Key("tier").Sortable().Filter(DataListFilter.Option().WithFacets()))
        .Field(c => c.Arr, f => f.Key("arr").Sortable().Filter(DataListFilter.Number(min: 0)))
        .Field(c => c.Health, f => f.Key("health").Sortable().Filter(DataListFilter.Number(min: 0, max: 100)))
        .Field(c => c.Since, f => f.Key("since").Sortable().Filter(DataListFilter.Date()))
        .DefaultSort(c => c.Arr, descending: true)
        .DefaultPageSize(10));
}
