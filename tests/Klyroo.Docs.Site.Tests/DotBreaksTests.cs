using Klyroo.Docs.Site.Components.Signage;

namespace Klyroo.Docs.Site.Tests;

public class DotBreaksTests
{
    [Theory]
    [InlineData("Klyroo.Query.EntityFrameworkCore", new[] { "Klyroo.", "Query.", "EntityFrameworkCore" })]
    [InlineData("Klyroo.Form.DataAnnotations", new[] { "Klyroo.", "Form.", "DataAnnotations" })]
    [InlineData("Klyroo.UI", new[] { "Klyroo.", "UI" })]
    [InlineData("Klyroo", new[] { "Klyroo" })]
    [InlineData("Klyroo.", new[] { "Klyroo." })]
    public void A_package_name_wraps_only_after_a_dot(string text, string[] pieces) =>
        Assert.Equal(pieces, DotBreaks.Split(text));

    [Fact]
    public void The_pieces_put_back_together_give_the_name() =>
        Assert.Equal("Klyroo.Form.Core", string.Concat(DotBreaks.Split("Klyroo.Form.Core")));
}
