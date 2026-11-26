using Klyroo.Form;

namespace KlyrooSaaSApp.Services;

/// <summary>What the account form edits. The form reads and writes these properties, so its fields have the types the properties have.</summary>
public sealed class AccountForm
{
    public string Name { get; set; } = "";

    public string Owner { get; set; } = "";

    public string Plan { get; set; } = "Team";

    public decimal Mrr { get; set; } = 199m;
}

/// <summary>The form that creates and edits an account. FormService opens it in a drawer and returns the <see cref="AccountForm"/> the user filled in.</summary>
public static class AccountForms
{
    public static FormSchema<AccountForm> Schema(string title, bool editing) => FormSchema.For<AccountForm>(s =>
    {
        s.FormKey = editing ? "edit-account" : "new-account";
        s.Title = title;
        s.Description = "Sample data: nothing is stored outside this session.";
        s.Field(a => a.Name, f => f.Label("Company").Required().MaxLength(80));
        s.Field(a => a.Owner, f => f.Label("Owner email").Required().Placeholder("name@company.com"));
        s.Field(a => a.Plan, f => f.Required().Options(AccountStore.Plans.Select(p => new FormOption<string>(p, p))));
        s.Field(a => a.Mrr, f => f.Label("Monthly price (EUR)").Required().Range(0m, 100_000m).StepBy(1m));
    });

    /// <summary>The values of an existing account, to show in the form.</summary>
    public static AccountForm Of(Account account) => new()
    {
        Name = account.Name,
        Owner = account.Owner,
        Plan = account.Plan,
        Mrr = account.Mrr,
    };
}
