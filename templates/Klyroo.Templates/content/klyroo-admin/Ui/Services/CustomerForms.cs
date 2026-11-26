using Klyroo.Form;

namespace KlyrooAdminApp.Services;

/// <summary>The form that creates and edits a customer. FormService opens it in a drawer and returns what the user typed.</summary>
public static class CustomerForms
{
    public static FormSchema Schema(string title, bool editing) => new()
    {
        FormKey = editing ? "edit-customer" : "new-customer",
        Title = title,
        Description = "Sample data: nothing is stored outside this session.",
        Fields =
        [
            new FormFieldSchema { Key = "name", Label = "Company", Required = true, MaxLength = 80 },
            new FormFieldSchema { Key = "contact", Label = "Contact email", Required = true, Placeholder = "name@company.com" },
            new FormFieldSchema
            {
                Key = "region",
                Label = "Region",
                Kind = FormFieldKind.Select,
                Required = true,
                DefaultValue = "EMEA",
                Options = [.. CustomerStore.Regions.Select(r => new FormOptionItem(r, r))],
            },
            new FormFieldSchema
            {
                Key = "tier",
                Label = "Tier",
                Kind = FormFieldKind.Radio,
                Required = true,
                DefaultValue = "Plus",
                Options = [.. CustomerStore.Tiers.Select(t => new FormOptionItem(t, t))],
            },
            new FormFieldSchema { Key = "arr", Label = "Annual revenue (EUR)", Kind = FormFieldKind.Number, Required = true, Min = 0, Max = 10_000_000, Step = 1000, DefaultValue = 50_000.0 },
        ],
    };

    /// <summary>The values of an existing customer, to show in the form.</summary>
    public static Dictionary<string, object?> Input(Customer customer) => new()
    {
        ["name"] = customer.Name,
        ["contact"] = customer.Contact,
        ["region"] = customer.Region,
        ["tier"] = customer.Tier,
        ["arr"] = (double)customer.Arr,
    };

    /// <summary>Reads the values of a submitted form. The number field gives a double, so the amount goes through Convert.ToDecimal.</summary>
    public static (string Name, string Contact, string Region, string Tier, decimal Arr) Read(IReadOnlyDictionary<string, object?> data) => (
        data["name"]?.ToString() ?? "",
        data["contact"]?.ToString() ?? "",
        data["region"]?.ToString() ?? "EMEA",
        data["tier"]?.ToString() ?? "Plus",
        Convert.ToDecimal(data["arr"] ?? 0.0));
}
