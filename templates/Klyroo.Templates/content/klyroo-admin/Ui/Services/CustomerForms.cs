using Klyroo.Form;

namespace KlyrooAdminApp.Services;

/// <summary>What the customer form edits. The form reads and writes these properties, so its fields have the types the properties have.</summary>
public sealed class CustomerForm
{
    public string Name { get; set; } = "";

    public string Contact { get; set; } = "";

    public string Region { get; set; } = "EMEA";

    public string Tier { get; set; } = "Plus";

    public decimal Arr { get; set; } = 50_000m;
}

/// <summary>The form that creates and edits a customer. FormService opens it in a drawer and returns the <see cref="CustomerForm"/> the user filled in.</summary>
public static class CustomerForms
{
    public static FormSchema<CustomerForm> Schema(string title, bool editing) => FormSchema.For<CustomerForm>(s =>
    {
        s.FormKey = editing ? "edit-customer" : "new-customer";
        s.Title = title;
        s.Description = "Sample data: nothing is stored outside this session.";
        s.Field(c => c.Name, f => f.Label("Company").Required().MaxLength(80));
        s.Field(c => c.Contact, f => f.Label("Contact email").Required().Placeholder("name@company.com"));
        s.Field(c => c.Region, f => f.Required().Options(CustomerStore.Regions.Select(r => new FormOption<string>(r, r))));
        s.Field(c => c.Tier, f => f.Required().AsRadio().Options(CustomerStore.Tiers.Select(t => new FormOption<string>(t, t))));
        s.Field(c => c.Arr, f => f.Label("Annual revenue (EUR)").Required().Range(0m, 10_000_000m).StepBy(1000m));
    });

    /// <summary>The values of an existing customer, to show in the form.</summary>
    public static CustomerForm Of(Customer customer) => new()
    {
        Name = customer.Name,
        Contact = customer.Contact,
        Region = customer.Region,
        Tier = customer.Tier,
        Arr = customer.Arr,
    };
}
