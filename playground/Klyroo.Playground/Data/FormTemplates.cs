using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Klyroo.Playground.Data;

public enum Topic { Sales, Support, Press }

public sealed class ContactModel
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public Topic Topic { get; set; }
    public string OrderNumber { get; set; } = "";
    public string Message { get; set; } = "";
}

/// <summary>The contact model with its rules written as attributes, read by <c>FromDataAnnotations()</c> of Klyroo.Form.DataAnnotations.</summary>
public sealed class AnnotatedContactModel : IValidatableObject
{
    [Required, StringLength(60), Display(Name = "Your name", Prompt = "Ada Lovelace", Order = 1)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, Display(Order = 2)]
    public string Email { get; set; } = "";

    [Display(Order = 3)]
    public Topic Topic { get; set; }

    [Required, StringLength(500, MinimumLength = 10), DataType(DataType.MultilineText), Display(Order = 4)]
    public string Message { get; set; } = "";

    public bool AcceptTerms { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (!AcceptTerms) yield return new ValidationResult("You must accept the terms", [nameof(AcceptTerms)]);
    }
}

public enum PlanTier { Free, Team }

public sealed class SignupModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Name { get; set; } = "";
    public string Country { get; set; } = "FR";
    public bool Newsletter { get; set; }
    public PlanTier Plan { get; set; }
    public string VatNumber { get; set; } = "";
}

public sealed class SettingsModel
{
    public string Name { get; set; } = "";
    public string Domain { get; set; } = "";
    public bool Notifications { get; set; }
    public string SlackChannel { get; set; } = "";
    public bool TwoFactor { get; set; }
    public int SessionMinutes { get; set; } = 60;
}

public sealed class OrderModel
{
    public string Customer { get; set; } = "";
    public bool Gift { get; set; }
    public string GiftMessage { get; set; } = "";
    public List<OrderLine> Lines { get; set; } = [];
}

public sealed class OrderLine
{
    public int Id { get; set; }
    public string Sku { get; set; } = "";
    public int Qty { get; set; } = 1;
    public decimal Price { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = true, UseStringEnumConverter = true)]
[JsonSerializable(typeof(ContactModel))]
[JsonSerializable(typeof(AnnotatedContactModel))]
[JsonSerializable(typeof(SignupModel))]
[JsonSerializable(typeof(SettingsModel))]
[JsonSerializable(typeof(OrderModel))]
public sealed partial class FormJson : JsonSerializerContext;

/// <summary>The four form schemas of the playground. Each takes the knobs that change its rules, so a new combination is a new schema.</summary>
public static class FormTemplates
{
    public static FormSchema<ContactModel> Contact(bool rules, bool conditions) => FormSchema.For<ContactModel>(s =>
    {
        s.FormKey = "contact";
        s.Title = "Contact us";
        s.Field(c => c.Name, f => { f.Label("Your name"); if (rules) f.Required(); });
        s.Field(c => c.Email, f => { f.Label("Email"); if (rules) f.Required().Rule(v => v.Contains('@'), "Enter a valid email address"); });
        var topic = s.Field(c => c.Topic, f => f.AsRadio());
        s.Field(c => c.OrderNumber, f =>
        {
            f.Label("Order number").Placeholder("ORD-1042");
            if (conditions) f.VisibleWhen(c => c.Get(topic) == Topic.Support).Required(c => c.Get(topic) == Topic.Support).DependsOn(topic);
        });
        s.Field(c => c.Message, f => { f.AsTextarea(); if (rules) f.Required().MaxLength(500); });
    });

    public static FormSchema<AnnotatedContactModel> AnnotatedContact() => FormSchema.For<AnnotatedContactModel>(s =>
    {
        s.FormKey = "contact-annotated";
        s.Title = "Contact us";
        s.FromDataAnnotations();
    });

    public static FormSchema<SignupModel> Signup(bool rules, bool conditions) => FormSchema.For<SignupModel>(s =>
    {
        s.FormKey = "signup";
        s.Title = "Create your account";
        s.Step("account", "Account", step =>
        {
            step.Field(m => m.Email, f => { f.Label("Email"); if (rules) f.Required().Rule(v => v.Contains('@'), "Enter a valid email address"); });
            step.Field(m => m.Password, f => { f.AsPassword(); if (rules) f.Required().Rule(v => v.Length >= 8, "Use at least 8 characters"); });
        });
        s.Step("profile", "Profile", step =>
        {
            step.Field(m => m.Name, f => { f.Label("Full name"); if (rules) f.Required(); });
            step.Field(m => m.Country, f => f.Options([new FormOption<string>("FR", "France"), new FormOption<string>("DE", "Germany"), new FormOption<string>("US", "United States")]));
            step.Field(m => m.Newsletter, f => f.Label("Send me the newsletter").AsSwitch());
        });
        FormField<SignupModel, PlanTier> plan = null!;
        s.Step("plan", "Plan", step => plan = step.Field(m => m.Plan, f => f.AsRadio()));
        s.Step("billing", "Billing", step =>
        {
            step.Field(m => m.VatNumber, f => { f.Label("VAT number"); if (rules) f.Required(); });
        }, skipWhen: conditions ? c => c.Get(plan) == PlanTier.Free : null);
    });

    public static FormSchema<SettingsModel> Settings(bool rules, bool conditions) => FormSchema.For<SettingsModel>(s =>
    {
        s.FormKey = "settings";
        s.Title = "Workspace settings";
        s.Section("general", "General", section =>
        {
            section.Field(m => m.Name, f => { f.Label("Workspace name"); if (rules) f.Required(); });
            section.Field(m => m.Domain, f => { f.Label("Domain").Placeholder("acme.example"); if (rules) f.Required().Rule(v => v.Contains('.'), "Enter a domain such as acme.example"); });
        });
        FormField<SettingsModel, bool> notifications = null!;
        s.Section("notifications", "Notifications", section =>
        {
            notifications = section.Field(m => m.Notifications, f => f.Label("Send notifications").AsSwitch());
            section.Field(m => m.SlackChannel, f =>
            {
                f.Label("Slack channel").Placeholder("#general");
                if (conditions) f.VisibleWhen(c => c.Get(notifications)).Required(c => c.Get(notifications)).DependsOn(notifications);
            });
        }, optional: true);
        s.Section("security", "Security", section =>
        {
            section.Field(m => m.TwoFactor, f => f.Label("Require two-factor sign-in").AsSwitch());
            section.Field(m => m.SessionMinutes, f => { f.Label("Session length (minutes)"); if (rules) f.Range(5, 240); });
        }, optional: true);
    });

    public static FormSchema<OrderModel> Order(bool rules, bool conditions) => FormSchema.For<OrderModel>(s =>
    {
        s.FormKey = "order";
        s.Title = "New order";
        s.Field(m => m.Customer, f => { f.Label("Customer"); if (rules) f.Required(); });
        var gift = s.Field(m => m.Gift, f => f.Label("This is a gift").AsSwitch());
        s.Field(m => m.GiftMessage, f =>
        {
            f.Label("Gift message").AsTextarea();
            if (conditions) f.VisibleWhen(c => c.Get(gift)).DependsOn(gift);
        });
        s.List(m => m.Lines, row =>
        {
            row.Field(l => l.Sku, f => { f.Label("SKU"); if (rules) f.Required().MaxLength(12); });
            row.Field(l => l.Qty, f => { f.Label("Quantity"); if (rules) f.Range(1, 99); });
            row.Field(l => l.Price, f => { f.Label("Unit price"); if (rules) f.Rule(p => p > 0, "The price must be above zero"); });
        }, addLabel: "Add line", configure: f => { if (rules) f.Required(); });
    });

    public static ContactModel ContactSample() => new() { Name = "Ada Lovelace", Email = "ada@example.test", Topic = Topic.Support, OrderNumber = "ORD-1042", Message = "My order arrived without the second box." };
    public static SignupModel SignupSample() => new() { Email = "ada@example.test", Name = "Ada Lovelace", Country = "FR", Plan = PlanTier.Team };
    public static SettingsModel SettingsSample() => new() { Name = "Acme", Domain = "acme.example", Notifications = true, SlackChannel = "#ops", SessionMinutes = 90 };
    public static OrderModel OrderSample() => new() { Customer = "Globex", Lines = [new() { Id = 1, Sku = "A-100", Qty = 2, Price = 19.9m }, new() { Id = 2, Sku = "B-220", Qty = 1, Price = 49m }] };
}
