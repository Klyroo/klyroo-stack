using Klyroo.UI;

namespace Klyroo.Stack.Site.Client.Docs;

public static partial class Catalog
{
    private static readonly ComponentDoc[] Elements =
    [
        new("kbd", "Kbd", "Elements", "A keyboard key or shortcut, in four variants and five sizes.",
            [new("Variants, sizes and shortcuts", typeof(Examples.KbdExample))], [typeof(KlyKbd)]),
        new("separator", "Separator", "Elements", "A line between sections, optionally with a label or an icon, horizontal or vertical.",
            [new("Horizontal", typeof(Examples.SeparatorExample)), new("Vertical", typeof(Examples.SeparatorVerticalExample))], [typeof(KlySeparator)]),
        new("skeleton", "Skeleton", "Elements", "A pulsing placeholder for content that is loading. Mark the loading region with aria-busy.",
            [new("Loading profile", typeof(Examples.SkeletonExample), "The placeholders are hidden from assistive technology; the busy region is announced once.")], [typeof(KlySkeleton)]),
        new("infinite-scroll", "InfiniteScroll", "Elements", "A sentinel that asks for the next items when it comes into view, with a button and a link for keyboards, screen readers and pages without .NET.",
            [new("Closest scrollable ancestor", typeof(Examples.InfiniteScrollExample), "OnLoadMore runs once each time the sentinel is reached, and not again until the load is over (an async handler counts as running until it returns). Static rendering and the moments before the page is interactive have no handler: the button is a link to Href and the next page loads from the address. The \"Load more\" button is always in the tab order and does the same by hand."),
             new("Explicit root and Loading", typeof(Examples.InfiniteScrollControlledExample), "Root names the scrolling element by selector, and the parent owns Loading: the sentinel is checked again when it turns false, so a sentinel still in view loads the next items. Without Href, the button waits for the page to be interactive."),
             new("A load that adds nothing", typeof(Examples.InfiniteScrollEmptyExample), "After a load that did not grow the content (nothing to add, or it failed) nothing is asked again until the sentinel has left the view and come back, or the button is used: a sentinel that stays in view would loop. A real handler sets HasMore to false, or shows a retry."),
             new("The page scrolls", typeof(Examples.InfiniteScrollWrapperExample), "A wrapper with overflow-x-hidden has a computed overflow-y of auto though nothing scrolls in it: only an ancestor that really scrolls is taken as the root, else the viewport. Put aria-busy on your list while it loads: the component does not, because it would hide its own announcement.")],
            [typeof(KlyInfiniteScroll)]),
        new("chip", "Chip", "Elements", "A small dot or count placed on a corner of another element.",
            [new("Positions and states", typeof(Examples.ChipExample))], [typeof(KlyChip)]),
        new("link", "Link", "Elements", "An anchor that knows whether it points at the current page and says so with aria-current.",
            [new("Active state", typeof(Examples.LinkExample), "Matching ignores the query string and the fragment; the active state follows navigation in interactive pages.")], [typeof(KlyLink)]),
        new("icon", "Icon", "Elements", "Any Iconify icon by name (i-ph-house, mdi:home, a bare Lucide name), bundled into the app at build time, with the Lucide icons that components use built in.",
            [new("Built-in icons", typeof(Examples.IconExample)),
             new("Iconify collections", typeof(Examples.IconCollectionsExample), "The build finds these names in the example's source, downloads them from the Iconify API and compiles them into the app. custom:klyroo comes from a folder of SVG files. See the Icons guide."),
             new("Names built at runtime", typeof(Examples.IconDynamicExample), "The scan reads literals only: the icons such a name can produce are listed in KlyrooIcon items of the project file.")],
            [typeof(KlyIcon)]),
        new("container", "Container", "Elements", "A centered wrapper with a maximum width and responsive padding.",
            [new("Container", typeof(Examples.ContainerExample))], [typeof(KlyContainer)]),
        new("progress", "Progress", "Elements", "A determinate or indeterminate progress bar, with optional step labels.",
            [new("Determinate and indeterminate", typeof(Examples.ProgressExample), "The indeterminate animation stops when the user prefers reduced motion."),
             new("Steps", typeof(Examples.ProgressStepsExample), "The value is the index of the current step; a button with OnClick stays disabled until the page is interactive.")],
            [typeof(KlyProgress)]),
        new("banner", "Banner", "Elements", "An announcement bar. Its close button remembers the dismissal per Id in the browser, static pages included.",
            [new("Banner", typeof(Examples.BannerExample), "Close it and reload the page: it stays hidden.")], [typeof(KlyBanner)]),
        new("empty", "Empty", "Elements", "An empty state with an icon, a title, a description and actions.",
            [new("Empty state", typeof(Examples.EmptyExample)), new("Action items", typeof(Examples.EmptyActionsExample), "ActionItems draws KlyActionItem records as small buttons, links included.")], [typeof(KlyEmpty)]),
        new("user", "User", "Elements", "An avatar with a name and a description, optionally a link.",
            [new("Sizes, link and vertical", typeof(Examples.UserExample)), new("Badge, contacts and a person icon", typeof(Examples.UserBadgeExample), "A user takes a Badge and Contacts (a link or a copy button each), and shows a person icon without a name and a picture. KlyAvatarGroup puts the description and badge of each person in its tooltips.")], [typeof(KlyUser)]),
        new("avatar-group", "AvatarGroup", "Elements", "A stack of avatars with a +N avatar for the overflow.",
            [new("Max and overflow", typeof(Examples.AvatarGroupExample))], [typeof(KlyAvatarGroup)]),
        new("image", "Image", "Elements", "An img with the width, height, srcset, sizes and loading a provider fills in.",
            [new("Size, format and fit", typeof(Examples.ImageExample), "Served by the local optimizer (Klyroo.UI.Images.Local): open the network tab to see the resized WebP files."),
             new("Responsive sizes", typeof(Examples.ImageResponsiveExample), "Sizes take space-separated responsive entries: an unprefixed entry is the default, and each breakpoint applies from its width upward."),
             new("Placeholder and priority", typeof(Examples.ImagePlaceholderExample)),
             new("Caption", typeof(Examples.ImageCaptionExample), "A Caption wraps the image in a figure with a figcaption; nothing is wrapped without it.")],
            [typeof(KlyImage)]),
        new("field-group", "FieldGroup", "Elements", "Attaches buttons and fields to each other: the children lose the inner radii, with CSS only.",
            [new("Horizontal and vertical", typeof(Examples.FieldGroupExample))], [typeof(KlyFieldGroup)]),
    ];
}
