using Klyroo.Docs.Site.Library;

namespace Klyroo.Docs.Site.Tests;

public class HomeTextTests
{
    [Theory]
    [InlineData("required and typed", "Required and typed")]
    [InlineData("five filter kinds", "Five filter kinds")]
    [InlineData("Already a sentence", "Already a sentence")]
    [InlineData("<code>ExecuteInMemory</code> runs a request", "<code>ExecuteInMemory</code> runs a request")]
    [InlineData("12 languages", "12 languages")]
    [InlineData("", "")]
    public void A_feature_description_starts_with_a_capital_unless_it_opens_with_markup(string html, string expected) =>
        Assert.Equal(expected, HomeText.Capitalize(html));
}
