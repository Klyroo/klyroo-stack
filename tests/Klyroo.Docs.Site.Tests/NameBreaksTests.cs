using Klyroo.Docs.Site.Components.Signage;

namespace Klyroo.Docs.Site.Tests;

public class NameBreaksTests
{
    [Theory]
    [InlineData("DataListExportTooLargeException", new[] { "Data", "List", "Export", "Too", "Large", "Exception" })]
    [InlineData("DataListActionPartBase<TItem>", new[] { "Data", "List", "Action", "Part", "Base", "<TItem>" })]
    [InlineData("Klyroo.Table.Server", new[] { "Klyroo.", "Table.", "Server" })]
    [InlineData("HTTPClient", new[] { "HTTPClient" })]
    [InlineData("Install the package", new[] { "Install the package" })]
    [InlineData("Step 1. Install", new[] { "Step 1. Install" })]
    public void A_name_wraps_only_between_its_words(string text, string[] pieces) =>
        Assert.Equal(pieces, NameBreaks.Split(text));

    [Theory]
    [InlineData("KlyFormEngine<TModel>", new[] { "KlyFormEngine", "<TModel>" })]
    [InlineData("IReadOnlyList<KlyrooStackModule>", new[] { "IReadOnlyList", "<Klyroo", "Stack", "Module>" })]
    [InlineData("OpenReadStream(maxAllowedSize)", new[] { "OpenReadStream", "(maxAllowedSize)" })]
    [InlineData("AddKlyrooStack()", new[] { "AddKlyrooStack()" })]
    [InlineData("klyroo.Display.Core.dll", new[] { "klyroo.", "Display.", "Core.", "dll" })]
    [InlineData("DataListExportTooLargeException", new[] { "Data", "List", "Export", "Too", "Large", "Exception" })]
    [InlineData("src/Klyroo.Table/Klyroo_Table.csproj", new[] { "src/", "Klyroo.", "Table/", "Klyroo_", "Table.", "csproj" })]
    public void Inline_code_wraps_at_dots_and_arguments_and_keeps_a_short_name_whole(string text, string[] pieces) =>
        Assert.Equal(pieces, NameBreaks.SplitCode(text));

    [Fact]
    public void The_pieces_put_back_together_give_the_name()
    {
        Assert.Equal("KlyTable<TItem>.Columns", string.Concat(NameBreaks.Split("KlyTable<TItem>.Columns")));
        Assert.Equal("KlyTable<TItem>.Columns", string.Concat(NameBreaks.SplitCode("KlyTable<TItem>.Columns")));
    }

    [Theory]
    [InlineData("KlyDynamicFormEngine", new[] { "KlyDynamic", "Form", "Engine" })]
    [InlineData("KlyDashboardPageSyncUrl", new[] { "KlyDashboard", "Page", "SyncUrl" })]
    public void A_short_prefix_of_a_long_name_stays_with_the_next_word(string text, string[] pieces) =>
        Assert.Equal(pieces, NameBreaks.SplitCode(text));
}
