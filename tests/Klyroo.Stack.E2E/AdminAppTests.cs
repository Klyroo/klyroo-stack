namespace Klyroo.Stack.E2E;

/// <summary>
/// The typed customer form of the app <c>dotnet new klyroo-admin</c> generates: a <c>FormSchema&lt;CustomerForm&gt;</c> with a text, an email-like text, a
/// choice, a radio group and a number, opened in a drawer from the customer list (New customer) and from the page of one customer (Edit). Fill, validate,
/// submit and cancel in a real browser, with the circuit of the generated app (Interactive Server).
/// </summary>
public class AdminAppTests(PlaygroundServer server, GeneratedApps apps) : GeneratedAppTest(server, apps, "klyroo-admin")
{
    private ILocator NewCustomer => Page.GetByRole(AriaRole.Button, new() { Name = "New customer" });

    private static ILocator Company(ILocator dialog) => dialog.GetByRole(AriaRole.Textbox, new() { Name = "Company" });

    private static ILocator Contact(ILocator dialog) => dialog.GetByRole(AriaRole.Textbox, new() { Name = "Contact email" });

    private static ILocator Region(ILocator dialog) => dialog.GetByRole(AriaRole.Combobox, new() { Name = "Region" });

    private static ILocator Tier(ILocator dialog, string name) => dialog.GetByRole(AriaRole.Radio, new() { Name = name });

    private static ILocator Revenue(ILocator dialog) => dialog.GetByRole(AriaRole.Spinbutton, new() { Name = "Annual revenue (EUR)" });

    private ILocator Row(string name) => Page.GetByRole(AriaRole.Row).Filter(new() { HasText = name });

    [Fact]
    public async Task The_new_customer_form_starts_from_the_defaults_of_the_model_and_refuses_an_empty_one_with_one_summary()
    {
        await OpenAsync("customers");
        var dialog = await OpenDialogAsync(NewCustomer, "New customer");

        await Assertions.Expect(Region(dialog)).ToHaveValueAsync("EMEA");
        await Assertions.Expect(Tier(dialog, "Plus")).ToBeCheckedAsync();
        await Assertions.Expect(Revenue(dialog)).ToHaveValueAsync("50000");

        await Submit(dialog).ClickAsync();

        await Assertions.Expect(Summary(dialog)).ToContainTextAsync("There are 2 errors to fix");
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Company: This field is required", "Contact email: This field is required"]);
        await Assertions.Expect(Company(dialog)).ToBeFocusedAsync();
        await Assertions.Expect(Company(dialog)).ToHaveAttributeAsync("aria-invalid", "true");
        await Assertions.Expect(dialog).ToBeVisibleAsync();

        await Summary(dialog).GetByRole(AriaRole.Button, new() { Name = "Contact email: This field is required" }).ClickAsync();
        await Assertions.Expect(Contact(dialog)).ToBeFocusedAsync();
        AssertNoConsoleErrors();
    }

    [Fact]
    public async Task The_annual_revenue_is_checked_against_its_range()
    {
        await OpenAsync("customers");
        var dialog = await OpenDialogAsync(NewCustomer, "New customer");
        await Company(dialog).FillAsync(Unique("Range"));
        await Contact(dialog).FillAsync("range@example.com");

        await Revenue(dialog).FillAsync("-1");
        await Submit(dialog).ClickAsync();
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Annual revenue (EUR): Value must be at least 0"]);
        await Assertions.Expect(Revenue(dialog)).ToBeFocusedAsync();

        await Revenue(dialog).FillAsync("10000001");
        await Submit(dialog).ClickAsync();
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Annual revenue (EUR): Value must be at most 10000000"]);
        await Assertions.Expect(Revenue(dialog)).ToBeFocusedAsync();

        await Revenue(dialog).FillAsync("120000");
        await Assertions.Expect(Summary(dialog)).ToHaveCountAsync(0);
        AssertNoConsoleErrors();
    }

    [Fact]
    public async Task The_radio_group_follows_the_keyboard_and_the_choice_is_what_the_list_shows_after_submitting()
    {
        await OpenAsync("customers");
        var name = Unique("Zeta");
        var dialog = await OpenDialogAsync(NewCustomer, "New customer");
        await Company(dialog).FillAsync(name);
        await Contact(dialog).FillAsync("zeta@example.com");
        await Region(dialog).SelectOptionAsync("APAC");

        // Plus is the default: the arrow moves the choice to the next radio and checks it.
        await Tier(dialog, "Plus").FocusAsync();
        await Page.Keyboard.PressAsync("ArrowRight");
        await Assertions.Expect(Tier(dialog, "Enterprise")).ToBeCheckedAsync();
        await Assertions.Expect(Tier(dialog, "Plus")).Not.ToBeCheckedAsync();

        // The biggest revenue sorts first (the list is sorted by revenue, descending).
        await Revenue(dialog).FillAsync("9000000");
        await Submit(dialog).ClickAsync();

        await Assertions.Expect(dialog).ToBeHiddenAsync();
        var row = Row(name);
        await Assertions.Expect(row).ToHaveCountAsync(1);
        await Assertions.Expect(row).ToContainTextAsync("APAC");
        await Assertions.Expect(row).ToContainTextAsync("9,000,000");
        await Assertions.Expect(Page.GetByText("Customer created")).ToBeVisibleAsync();
        AssertNoConsoleErrors();
    }

    [Fact]
    public async Task Cancel_and_Escape_close_the_drawer_without_adding_a_customer()
    {
        await OpenAsync("customers");
        var name = Unique("Cancelled");
        var dialog = await OpenDialogAsync(NewCustomer, "New customer");
        await Company(dialog).FillAsync(name);
        await Tier(dialog, "Standard").CheckAsync();
        await dialog.Locator("[data-slot=form-action-cancel]").ClickAsync();
        await Assertions.Expect(dialog).ToBeHiddenAsync();

        dialog = await OpenDialogAsync(NewCustomer, "New customer");
        await Assertions.Expect(Company(dialog)).ToHaveValueAsync("");
        await Assertions.Expect(Tier(dialog, "Plus")).ToBeCheckedAsync();
        await Page.Keyboard.PressAsync("Escape");
        await Assertions.Expect(dialog).ToBeHiddenAsync();
        await Assertions.Expect(Row(name)).ToHaveCountAsync(0);
    }

    [Fact]
    public async Task Edit_on_the_customer_page_shows_the_saved_values_validates_them_and_updates_the_page()
    {
        await OpenAsync("customers/1");
        var name = await Page.GetByRole(AriaRole.Heading, new() { Level = 1 }).InnerTextAsync();
        var dialog = await OpenDialogAsync(Page.GetByRole(AriaRole.Button, new() { Name = "Edit" }), $"Edit {name}");

        await Assertions.Expect(Company(dialog)).ToHaveValueAsync(name);
        await Assertions.Expect(Contact(dialog)).Not.ToHaveValueAsync("");

        await Contact(dialog).FillAsync("");
        await Submit(dialog).ClickAsync();
        await Assertions.Expect(SummaryItems(dialog)).ToHaveTextAsync(["Contact email: This field is required"]);
        await Assertions.Expect(Contact(dialog)).ToBeFocusedAsync();

        var contact = $"{Guid.NewGuid().ToString("N")[..6]}@edited.example";
        await Contact(dialog).FillAsync(contact);
        await Tier(dialog, "Enterprise").CheckAsync();
        await Submit(dialog).ClickAsync();

        await Assertions.Expect(dialog).ToBeHiddenAsync();
        await Assertions.Expect(Page.GetByText(contact).First).ToBeVisibleAsync();
        await Assertions.Expect(Page.GetByText("Customer saved")).ToBeVisibleAsync();
        await Assertions.Expect(Page.GetByRole(AriaRole.Heading, new() { Level = 1 })).ToHaveTextAsync(name);
        AssertNoConsoleErrors();
    }
}
