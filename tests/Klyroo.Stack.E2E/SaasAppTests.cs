namespace Klyroo.Stack.E2E;

/// <summary>
/// The typed account form of the app <c>dotnet new klyroo-saas</c> generates: a <c>FormSchema&lt;AccountForm&gt;</c> that <c>FormService</c> opens in a drawer
/// (New account, and Edit on a row). Fill, validate, submit and cancel in a real browser, with the circuit of the generated app (Interactive Server).
/// </summary>
public class SaasAppTests(PlaygroundServer server, GeneratedApps apps) : GeneratedAppTest(server, apps, "klyroo-saas")
{
    private ILocator NewAccount => Page.GetByRole(AriaRole.Button, new() { Name = "New account" });

    private static ILocator Company(ILocator dialog) => dialog.GetByRole(AriaRole.Textbox, new() { Name = "Company" });

    private static ILocator Owner(ILocator dialog) => dialog.GetByRole(AriaRole.Textbox, new() { Name = "Owner email" });

    private static ILocator Plan(ILocator dialog) => dialog.GetByRole(AriaRole.Combobox, new() { Name = "Plan" });

    private static ILocator Price(ILocator dialog) => dialog.GetByRole(AriaRole.Spinbutton, new() { Name = "Monthly price (EUR)" });

    private ILocator Row(string name) => Page.GetByRole(AriaRole.Row).Filter(new() { HasText = name });

    [Fact]
    public async Task The_new_account_form_lists_every_required_field_in_one_summary_and_puts_the_focus_on_the_first_invalid_one()
    {
        await OpenAsync("accounts");
        var dialog = await OpenDialogAsync(NewAccount, "New account");

        // The defaults of the typed model: the plan and the price come from the properties of AccountForm.
        await Assertions.Expect(Plan(dialog)).ToHaveValueAsync("Team");
        await Assertions.Expect(Price(dialog)).ToHaveValueAsync("199");
        await Assertions.Expect(Summary(dialog)).ToHaveCountAsync(0);

        await Submit(dialog).ClickAsync();

        await Assertions.Expect(Summary(dialog)).ToContainTextAsync("There are 2 errors to fix");
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Company: This field is required", "Owner email: This field is required"]);
        await Assertions.Expect(Company(dialog)).ToHaveAttributeAsync("aria-invalid", "true");
        await Assertions.Expect(Owner(dialog)).ToHaveAttributeAsync("aria-invalid", "true");
        await Assertions.Expect(Company(dialog)).ToBeFocusedAsync();
        // The form stays open: nothing was submitted.
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        // A link of the summary moves the focus to its field.
        await Summary(dialog).GetByRole(AriaRole.Button, new() { Name = "Owner email: This field is required" }).ClickAsync();
        await Assertions.Expect(Owner(dialog)).ToBeFocusedAsync();
        AssertNoConsoleErrors();
    }

    [Fact]
    public async Task The_price_is_checked_against_its_range_and_the_summary_names_the_field()
    {
        await OpenAsync("accounts");
        var dialog = await OpenDialogAsync(NewAccount, "New account");
        await Company(dialog).FillAsync(Unique("Range"));
        await Owner(dialog).FillAsync("range@example.com");

        await Price(dialog).FillAsync("-5");
        await Submit(dialog).ClickAsync();
        await Assertions.Expect(Summary(dialog)).ToContainTextAsync("There is 1 error to fix");
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Monthly price (EUR): Value must be at least 0"]);
        await Assertions.Expect(Price(dialog)).ToBeFocusedAsync();

        await Price(dialog).FillAsync("100001");
        await Submit(dialog).ClickAsync();
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Monthly price (EUR): Value must be at most 100000"]);
        await Assertions.Expect(Price(dialog)).ToBeFocusedAsync();

        // A value in the range clears the error: the form is valid again and the summary is gone.
        await Price(dialog).FillAsync("250");
        await Assertions.Expect(Summary(dialog)).ToHaveCountAsync(0);
        await Assertions.Expect(dialog).ToBeVisibleAsync();
        AssertNoConsoleErrors();
    }

    [Fact]
    public async Task The_company_name_field_stops_at_eighty_characters()
    {
        await OpenAsync("accounts");
        var dialog = await OpenDialogAsync(NewAccount, "New account");
        // MaxLength(80) of the schema is the maxlength of the input: the browser stops the text there, whatever is typed or pasted.
        await Assertions.Expect(Company(dialog)).ToHaveAttributeAsync("maxlength", "80");

        await Company(dialog).FillAsync(new string('x', 81));
        Assert.Equal(80, (await Company(dialog).InputValueAsync()).Length);

        await Owner(dialog).FillAsync("long@example.com");
        await Submit(dialog).ClickAsync();
        await Assertions.Expect(dialog).ToBeHiddenAsync();
        await Assertions.Expect(Row(new string('x', 80))).ToHaveCountAsync(1);
    }

    [Fact]
    public async Task A_valid_account_is_added_to_the_list_with_a_toast_and_the_focus_returns_to_the_button()
    {
        await OpenAsync("accounts");
        var name = Unique("Zeta");
        var dialog = await OpenDialogAsync(NewAccount, "New account");
        await Company(dialog).FillAsync(name);
        await Owner(dialog).FillAsync("zeta@example.com");
        await Plan(dialog).SelectOptionAsync("Business");
        await Price(dialog).FillAsync("1234");
        await Submit(dialog).ClickAsync();

        await Assertions.Expect(dialog).ToBeHiddenAsync();
        var row = Row(name);
        await Assertions.Expect(row).ToHaveCountAsync(1);
        await Assertions.Expect(row).ToContainTextAsync("Business");
        await Assertions.Expect(row).ToContainTextAsync("1,234");
        await Assertions.Expect(row).ToContainTextAsync("Trial");
        await Assertions.Expect(Page.GetByText("Account created")).ToBeVisibleAsync();
        await Assertions.Expect(NewAccount).ToBeFocusedAsync();
        AssertNoConsoleErrors();
    }

    [Fact]
    public async Task Cancel_closes_the_drawer_without_adding_an_account_and_the_next_form_starts_from_the_defaults()
    {
        await OpenAsync("accounts");
        var name = Unique("Cancelled");
        var dialog = await OpenDialogAsync(NewAccount, "New account");
        await Company(dialog).FillAsync(name);
        await Plan(dialog).SelectOptionAsync("Starter");
        await dialog.Locator("[data-slot=form-action-cancel]").ClickAsync();

        await Assertions.Expect(dialog).ToBeHiddenAsync();
        await Assertions.Expect(Row(name)).ToHaveCountAsync(0);

        dialog = await OpenDialogAsync(NewAccount, "New account");
        await Assertions.Expect(Company(dialog)).ToHaveValueAsync("");
        await Assertions.Expect(Plan(dialog)).ToHaveValueAsync("Team");
        await Assertions.Expect(Summary(dialog)).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Escape_closes_the_drawer_the_way_Cancel_does()
    {
        await OpenAsync("accounts");
        var dialog = await OpenDialogAsync(NewAccount, "New account");
        await Company(dialog).FillAsync(Unique("Escaped"));
        await Page.Keyboard.PressAsync("Escape");

        await Assertions.Expect(dialog).ToBeHiddenAsync();
        await Assertions.Expect(NewAccount).ToBeFocusedAsync();
    }

    [Fact]
    public async Task Edit_opens_the_row_in_the_form_validates_it_and_saves_the_changes_to_the_list()
    {
        await OpenAsync("accounts");
        var edit = Row("Acme Corp").GetByRole(AriaRole.Button, new() { Name = "Edit" });
        var dialog = await OpenDialogAsync(edit, "Edit Acme Corp");

        // The form shows the values of the row, typed: a text, a choice and a number.
        await Assertions.Expect(Company(dialog)).ToHaveValueAsync("Acme Corp");
        await Assertions.Expect(Plan(dialog)).ToHaveValueAsync("Starter");
        await Assertions.Expect(Price(dialog)).ToHaveValueAsync("49");

        // Emptying a required field is refused, and the summary and the focus say where.
        await Company(dialog).FillAsync("");
        await Submit(dialog).ClickAsync();
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Company: This field is required"]);
        await Assertions.Expect(Company(dialog)).ToBeFocusedAsync();

        await Company(dialog).FillAsync("Acme Corporation");
        await Owner(dialog).FillAsync("billing@acme.example");
        await Plan(dialog).SelectOptionAsync("Business");
        await Price(dialog).FillAsync("500");
        await Submit(dialog).ClickAsync();

        await Assertions.Expect(dialog).ToBeHiddenAsync();
        var row = Row("Acme Corporation");
        await Assertions.Expect(row).ToHaveCountAsync(1);
        await Assertions.Expect(row).ToContainTextAsync("Business");
        await Assertions.Expect(row).ToContainTextAsync("500");
        await Assertions.Expect(Page.GetByText("Account saved")).ToBeVisibleAsync();

        // The saved values come back when the row is edited again.
        dialog = await OpenDialogAsync(Row("Acme Corporation").GetByRole(AriaRole.Button, new() { Name = "Edit" }), "Edit Acme Corporation");
        await Assertions.Expect(Owner(dialog)).ToHaveValueAsync("billing@acme.example");
        await Assertions.Expect(Plan(dialog)).ToHaveValueAsync("Business");
        AssertNoConsoleErrors();
    }
}
