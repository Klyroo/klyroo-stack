using Klyroo.Docs.Site.Content;

namespace Klyroo.Docs.Site.Tests;

public class MarkdownPagesTests
{
    private static string Html(string markdown) => new MarkdownRenderer().Render(markdown, "", "page/").Html;

    [Fact]
    public void Inline_code_gets_soft_breaks_between_the_words_of_a_long_name_and_keeps_its_text()
    {
        var html = Html("Call `DataListExportTooLargeException(long count)` now.");
        Assert.Contains("<code>Data<wbr>List<wbr>Export<wbr>Too<wbr>Large<wbr>Exception<wbr>(long count)</code>", html);
    }

    [Fact]
    public void A_short_name_in_inline_code_stays_whole()
    {
        Assert.Contains("<code>KlyFormEngine<wbr>&lt;TModel&gt;</code>", Html("`KlyFormEngine<TModel>`"));
    }

    [Fact]
    public void A_code_block_gets_no_soft_breaks()
    {
        Assert.DoesNotContain("<wbr>", Html("```csharp\nDataListExportTooLargeException x;\n```"));
    }

    [Fact]
    public void A_table_whose_header_cells_are_all_empty_has_no_header()
    {
        var html = Html("| | |\n|---|---|\n| a | b |\n");
        Assert.DoesNotContain("<thead>", html);
        Assert.Contains("<td>a</td>", html);
    }

    [Fact]
    public void A_table_with_one_empty_header_cell_keeps_its_header()
    {
        Assert.Contains("<thead>", Html("| | Core |\n|---|---|\n| a | b |\n"));
    }

    [Fact]
    public void A_column_of_sentences_is_marked_so_the_table_scrolls_instead_of_squeezing_it()
    {
        var html = Html("| Name | What it does |\n|---|---|\n| a | A sentence long enough to be read as prose and not as a value. |\n| b | short |\n");
        Assert.Contains("<th class=\"col-prose\">What it does</th>", html);
        Assert.DoesNotContain("<th class=\"col-prose\">Name</th>", html);
    }
}
