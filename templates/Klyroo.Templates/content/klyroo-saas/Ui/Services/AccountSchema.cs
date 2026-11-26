using Klyroo.Query;

namespace KlyrooSaaSApp.Services;

/// <summary>
/// What the accounts list may ask for: which fields sort, which filter, the default sort and the page size. The same schema runs the request against
/// the in-memory data here, and would run it against a database with <c>schema.Apply(query, request)</c>.
/// </summary>
public static class AccountSchema
{
    public static readonly DataListSchema<Account> Schema = DataListSchema.For<Account>(s => s
        .RowKey(a => a.Id)
        .RowLabel(a => a.Name)
        .Search(a => a.Name, a => a.Owner)
        .Field(a => a.Name, f => f.Key("name").Sortable())
        .Field(a => a.Owner, f => f.Key("owner").Sortable())
        .Field(a => a.Plan, f => f.Key("plan").Sortable().Filter(DataListFilter.Option().WithFacets()))
        .Field(a => a.Status, f => f.Key("status").Sortable().Filter(DataListFilter.Option().WithFacets()))
        .Field(a => a.Mrr, f => f.Key("mrr").Sortable().Filter(DataListFilter.Number(min: 0)))
        .Field(a => a.Since, f => f.Key("since").Sortable().Filter(DataListFilter.Date()))
        .DefaultSort(a => a.Since, descending: true)
        .DefaultPageSize(8));
}
