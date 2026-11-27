using Klyroo.UI;

namespace Klyroo.Stack.Site.Client.Docs;

public sealed record ExampleEntry(string Title, Type Type, string? Description = null);

public sealed record ComponentDoc(string Slug, string Name, string Group, string Description, ExampleEntry[] Examples, Type[] Api);

/// <summary>Every documented component, in sidebar order.</summary>
public static partial class Catalog
{
    public static readonly string[] Groups =
        ["Elements", "Form", "Overlay", "Navigation", "Data", "Blocks", "Layout", "Page", "Dashboard", "Chat", "Editor"];

    /// <summary>Every documented component. Each family declares its entries in its own Catalog.*.cs file.</summary>
    // Built on first use: static fields of the other partial files may not be initialized yet at type load.
    public static ComponentDoc[] Components => _components ??= [.. Core, .. DataList, .. Blocks, .. Nav, .. Overlay2, .. Elements, .. Pickers, .. Form2, .. Page, .. Layout, .. Marketing, .. Dashboard, .. Chat, .. Editor, .. Chart, .. Content];

    private static ComponentDoc[]? _components;

    private static readonly ComponentDoc[] Core =
    [
        new("button", "Button", "Elements", "A button, or a link styled as one, in every color and variant.",
            [new("Variants", typeof(Examples.ButtonVariantsExample)), new("Sizes and icons", typeof(Examples.ButtonSizesExample)),
             new("States", typeof(Examples.ButtonStatesExample), "A button with OnClick stays disabled until the page is interactive.")],
            [typeof(KlyButton)]),
        new("badge", "Badge", "Elements", "A short label for a status, a count or a category.",
            [new("Variants", typeof(Examples.BadgeExample))], [typeof(KlyBadge)]),
        new("avatar", "Avatar", "Elements", "An image of a person, falling back to initials when there is none or it fails to load.",
            [new("Images and fallbacks", typeof(Examples.AvatarExample))], [typeof(KlyAvatar)]),
        new("card", "Card", "Elements", "A surface grouping a header, a body and a footer.",
            [new("Card", typeof(Examples.CardExample))], [typeof(KlyCard)]),
        new("alert", "Alert", "Elements", "A callout that draws attention to a message.",
            [new("Variants", typeof(Examples.AlertExample)), new("Actions and close", typeof(Examples.AlertActionsExample)),
             new("Details, action items and live region", typeof(Examples.AlertDetailsExample), "Details sits behind a Show details toggle, ActionItems draws KlyActionItem records, StatusIcon shows the icon of the color and Live (Polite or Assertive) makes it a live region for an alert inserted after the page loaded.")], [typeof(KlyAlert)]),

        new("input", "Input", "Form", "A single-line text field.",
            [new("Variants and icons", typeof(Examples.InputExample)),
             new("Debounce and keyboard", typeof(Examples.InputDebounceExample), "Debounce reports once per pause, Enter reports at once and Escape clears (ClearOnEscape). The browser does the waiting: Server mode makes one round trip per pause, not per key. Reset calls SetValueAsync, and the form validates after each pause."),
             new("Escape in a dialog", typeof(Examples.InputEscapeExample), "The Escape that clears text is consumed; Escape on an empty field closes the dialog.")],
            [typeof(KlyInput)]),
        new("textarea", "Textarea", "Form", "A multi-line text field that can grow with its content.",
            [new("Textarea", typeof(Examples.TextareaExample))], [typeof(KlyTextarea)]),
        new("checkbox", "Checkbox", "Form", "A native checkbox with a label and a description.",
            [new("Checkbox", typeof(Examples.CheckboxExample)),
             new("Indeterminate", typeof(Examples.CheckboxIndeterminateExample), "A select-all box in the mixed state; the parent owns it.")], [typeof(KlyCheckbox)]),
        new("switch", "Switch", "Form", "An on/off toggle built on a native checkbox, correct even without interactivity.",
            [new("Standalone", typeof(Examples.SwitchExample)), new("In an EditForm", typeof(Examples.SwitchFormExample), "Posts in static rendering too.")],
            [typeof(KlySwitch)]),
        new("radio-group", "RadioGroup", "Form", "A set of native radio buttons in a fieldset.",
            [new("Radio group", typeof(Examples.RadioGroupExample))], [typeof(KlyRadioGroup<string>)]),
        new("select", "Select", "Form", "A select-only combobox with keyboard navigation and typeahead; it posts through a hidden input.",
            [new("Select", typeof(Examples.SelectExample)),
             new("Counts, icons and groups", typeof(Examples.SelectItemsExample), "Count shows a number at the end of the row, Icon an icon before the label (a name, like every icon of Klyroo.UI) and Group lists the item under a heading. The headings are not options: the arrow keys and the typeahead skip them. Items without these properties render as before."),
             new("In a form", typeof(Examples.SelectFormExample), "A form to move through with a remote control: Select edits a field, Back leaves it."),
             new("Bound to an int foreign key", typeof(Examples.SelectIntExample), "The value of an item has the type of the bound property: KlySelectItems.From builds the items of a lookup table (the key is the value, a column the label), and AsNullable() makes them fit an int? property."),
             new("Bound to an enum", typeof(Examples.SelectEnumExample), "KlySelectItems.FromEnum lists the members of the enum in order, labeled by their name split into words or by a function you give it."),
             new("Bound to a Guid", typeof(Examples.SelectGuidExample), "Any value that has an invariant text works: it travels as that text and the control gives back the exact value of the option."),
             new("Bound to a DateTime", typeof(Examples.SelectDateTimeExample), "A DateTime travels as ISO 8601, so the option with that exact instant is the selected one.")], [typeof(KlySelect<string>)]),
        new("form-field", "FormField", "Form", "Label, description, hint and validation message wired to the control inside.",
            [new("A complete form", typeof(Examples.FormFieldExample))], [typeof(KlyFormField)]),

        new("modal", "Modal", "Overlay", "A native modal dialog: the page becomes inert and focus stays inside.",
            [new("Modal", typeof(Examples.ModalExample)), new("Confirmation", typeof(Examples.ModalConfirmExample), "Not dismissible: Escape and the backdrop do nothing."),
             new("Nested", typeof(Examples.ModalNestedExample), "A confirmation opened from a modal closes on its own."),
             new("With a select", typeof(Examples.ModalSelectExample), "Back closes the open list first, then the modal.")],
            [typeof(KlyModal)]),
        new("popover", "Popover", "Overlay", "A non-modal panel anchored to a button.",
            [new("Bound open state", typeof(Examples.PopoverExample)), new("Unbound", typeof(Examples.PopoverUnboundExample), "Opens and closes without leaving the browser.")],
            [typeof(KlyPopover)]),
        new("tooltip", "Tooltip", "Overlay", "A short hint shown on hover and keyboard focus.",
            [new("Tooltip", typeof(Examples.TooltipExample))], [typeof(KlyTooltip)]),
        new("dropdown-menu", "DropdownMenu", "Overlay", "A menu of actions and links, navigable with the keyboard.",
            [new("Dropdown menu", typeof(Examples.DropdownMenuExample)),
             new("Submenus", typeof(Examples.DropdownMenuSubmenuExample), "An item with Children opens a nested menu: right arrow, Enter, a click or a pause of the pointer opens it, left arrow or Escape closes it.")],
            [typeof(KlyDropdownMenu)]),
        new("toast", "Toast", "Overlay", "Brief notifications, raised from .NET through IToastService.",
            [new("Toasts", typeof(Examples.ToastExample))], [typeof(KlyToaster)]),

        new("tabs", "Tabs", "Navigation", "WAI-ARIA tabs with automatic activation.",
            [new("Pill", typeof(Examples.TabsExample)), new("Link", typeof(Examples.TabsLinkExample))], [typeof(KlyTabs)]),
        new("accordion", "Accordion", "Navigation", "Stacked headings that reveal their section.",
            [new("Accordion", typeof(Examples.AccordionExample)), new("With controls inside", typeof(Examples.AccordionNestedExample)),
             new("Text and DefaultOpen", typeof(Examples.AccordionTextExample), "An item takes a Text instead of content, and DefaultOpen opens it first (the first such item only, without Multiple).")], [typeof(KlyAccordion)]),
        new("table", "Table", "Data", "A data table with sortable columns.",
            [new("Sortable table", typeof(Examples.TableExample)),
             new("Footer, alignment and Show more", typeof(Examples.TableFooterExample), "A column takes a Footer (or a FooterTemplate), Align, Numeric and Mono; Max with Step cuts the rows behind a Show more button.")], [typeof(KlyTable<object>), typeof(KlyColumn<object, object>), typeof(KlyTemplateColumn<object>)]),
    ];

    public static ComponentDoc? Find(string slug) => Components.FirstOrDefault(c => c.Slug == slug);

    /// <summary>The guide pages, in sidebar order. Each is a Razor page declaring the same route (found by the docs bundle).</summary>
    public static readonly (string Href, string Title, string Section)[] Guides =
    [
        ("/docs", "Introduction", "Getting started"),
        ("/docs/installation", "Installation", "Getting started"),
        ("/docs/theming", "Theming", "Getting started"),
        ("/docs/design-system", "Design system", "Getting started"),
        ("/docs/css-variables", "CSS variables", "Getting started"),
        ("/docs/tailwind", "Tailwind", "Getting started"),
        ("/docs/fonts", "Fonts", "Getting started"),
        ("/docs/icons", "Icons", "Getting started"),
        ("/docs/internationalization", "Internationalization", "Getting started"),
        ("/docs/images", "Images", "Getting started"),
        ("/docs/render-modes", "Render modes", "Getting started"),
        ("/docs/maui", "MAUI and Blazor Hybrid", "Getting started"),
        ("/docs/url-state", "URL state", "Data"),
        ("/docs/data-list", "Data list sources and counts", "Data"),
        ("/docs/mcp", "MCP server", "AI"),
        ("/docs/llms-txt", "llms.txt", "AI"),
        ("/docs/skills", "Agent skill", "AI"),
    ];
}
