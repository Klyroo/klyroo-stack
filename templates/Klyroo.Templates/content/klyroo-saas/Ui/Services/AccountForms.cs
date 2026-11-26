using Klyroo.Form;

namespace KlyrooSaaSApp.Services;

/// <summary>The form that creates and edits an account. FormService opens it in a drawer and returns what the user typed.</summary>
public static class AccountForms
{
    public static FormSchema Schema(string title, bool editing) => new()
    {
        FormKey = editing ? "edit-account" : "new-account",
        Title = title,
        Description = "Sample data: nothing is stored outside this session.",
        Fields =
        [
            new FormFieldSchema { Key = "name", Label = "Company", Required = true, MaxLength = 80 },
            new FormFieldSchema { Key = "owner", Label = "Owner email", Required = true, Placeholder = "name@company.com" },
            new FormFieldSchema
            {
                Key = "plan",
                Label = "Plan",
                Kind = FormFieldKind.Select,
                Required = true,
                DefaultValue = "Team",
                Options = [.. AccountStore.Plans.Select(p => new FormOptionItem(p, p))],
            },
            new FormFieldSchema
            {
                Key = "mrr",
                Label = "Monthly price (EUR)",
                Kind = FormFieldKind.Number,
                Required = true,
                Min = 0,
                Max = 100_000,
                Step = 1,
                DefaultValue = 199.0,
            },
        ],
    };

    /// <summary>The values of an existing account, to show in the form.</summary>
    public static Dictionary<string, object?> Input(Account account) => new()
    {
        ["name"] = account.Name,
        ["owner"] = account.Owner,
        ["plan"] = account.Plan,
        ["mrr"] = (double)account.Mrr,
    };

    /// <summary>Reads the values of a submitted form. The number field gives a double, so the price goes through Convert.ToDecimal.</summary>
    public static (string Name, string Owner, string Plan, decimal Mrr) Read(IReadOnlyDictionary<string, object?> data) => (
        data["name"]?.ToString() ?? "",
        data["owner"]?.ToString() ?? "",
        data["plan"]?.ToString() ?? "Team",
        Convert.ToDecimal(data["mrr"] ?? 0.0));
}
