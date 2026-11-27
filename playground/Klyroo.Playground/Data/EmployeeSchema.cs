namespace Klyroo.Playground.Data;

/// <summary>The schema of the sample directory, shared by the query and the dashboard playgrounds.</summary>
public static class EmployeeSchema
{
    public static readonly DataListSchema<Employee> Instance = DataListSchema.For<Employee>(s =>
    {
        s.RowKey(e => e.Id);
        s.Search(e => e.Name, e => e.Email, e => e.Title);
        s.Field(e => e.Name, f => f.Sortable().Filter(DataListFilter.Text()));
        s.Field(e => e.Department, f => f.Sortable().Filter(DataListFilter.Option().WithFacets()));
        s.Field(e => e.Title, f => f.Sortable());
        s.Field(e => e.Country, f => f.Sortable().Filter(DataListFilter.Option().WithFacets()));
        s.Field(e => e.Status, f => f.Sortable().Filter(DataListFilter.Option().WithFacets()));
        s.Field(e => e.Salary, f => f.Sortable().Filter(DataListFilter.Number(min: 0)));
        s.Field(e => e.Hired, f => f.Sortable().Filter(DataListFilter.Date()));
        s.DefaultSort(e => e.Name);
        s.DefaultPageSize(10);
    });
}
