using Klyroo.Stack.Mcp.Markdown;
using Klyroo.Stack.Mcp.Model;

namespace Klyroo.Stack.Mcp.Tests;

/// <summary>Components of Klyroo.Table are documented next to those of Klyroo.UI, and every output says which package to add.</summary>
public class ToolsPackageTests
{
    private static BundleComponent Empty => TestSupport.Service.FindComponent("data-list-empty")!;

    [Fact]
    public void The_placeholder_is_in_the_bundle_with_its_package_and_its_theme()
    {
        Assert.Equal("Klyroo.Table", Empty.Package);
        Assert.Equal("dotnet add package Klyroo.Table --prerelease", Empty.InstallCommand());
        Assert.Equal("AddKlyrooTable", Empty.RegistrationMethod());

        var api = Assert.Single(Empty.Api);
        Assert.Equal("KlyDataListEmpty", api.Name);
        Assert.Contains(api.Parameters, p => p.Name == "Title" && p.Description is not null);
        Assert.Contains(api.Slots, s => s.Name == "Actions");
        Assert.Equal("DataListEmpty", api.Theme!.Property);
        Assert.Equal(["root", "icon", "title", "description", "actions"], api.Theme.Slots.Keys);
        Assert.NotNull(api.Ui);
    }

    [Fact]
    public void Components_of_Klyroo_UI_have_no_package()
    {
        Assert.All(TestSupport.Bundle.Components.Where(c => !c.Slug.StartsWith("data-list", StringComparison.Ordinal)), c => Assert.Null(c.Package));
    }

    [Fact]
    public void The_icon_of_the_package_is_in_the_icon_list()
    {
        Assert.Contains("search-x", TestSupport.Bundle.Icons);
        Assert.Contains("search", TestSupport.Bundle.Icons);
    }

    [Fact]
    public void The_data_list_is_in_the_bundle_with_its_parts_and_its_theme()
    {
        var list = TestSupport.Service.FindComponent("data-list")!;

        Assert.Equal("Klyroo.Table", list.Package);
        Assert.Equal(["KlyDataList<TItem>", "KlyDataListTable<TItem>", "KlyDataListColumn<TItem, TValue>", "KlyDataListFieldColumn<TItem>", "KlyDataListTemplateColumn<TItem>", "KlyDataListPagination<TItem>", "KlyDataListResultCount<TItem>", "KlyDataListRefresh<TItem>", "KlyDataListErrorState<TItem>", "KlyDataListSearch<TItem>"], list.Api.Select(a => a.Name));
        var root = list.Api[0];
        Assert.Contains(root.Parameters, p => p.Name == "UrlSync" && p.Description is not null);
        Assert.Contains(root.Parameters, p => p.Name == "ResultJson" && p.Description is not null);
        Assert.Equal("DataList", root.Theme!.Property);
        Assert.Contains("tableRoot", root.Theme.Slots.Keys);
        Assert.Contains("### DataList", File.ReadAllText(Path.Combine(TestSupport.UiRepoRoot, SkillReference.RelativePath)));
    }

    [Fact]
    public void The_data_list_pages_are_in_the_bundle_each_with_its_parts_and_the_package()
    {
        var pages = new Dictionary<string, string[]>
        {
            ["data-list-filters"] = ["KlyDataListFilterTags<TItem>", "KlyDataListAddFilter<TItem>", "KlyDataListClearFilters<TItem>"],
            ["data-list-filter-panel"] = ["KlyDataListFilterPanel<TItem>"],
            ["data-list-facets"] = ["KlyDataListFilterEditor<TItem>"],
            ["data-list-selection"] = ["KlyDataListSelectionActions<TItem>", "KlyDataListActionsDropdown<TItem>", "KlyDataListRowActions<TItem>"],
            ["data-list-cursor"] = ["KlyDataListInfiniteLoader<TItem>"],
            ["data-list-columns"] = ["KlyDataListColumnPanel<TItem>"],
        };
        foreach (var (slug, api) in pages)
        {
            var page = TestSupport.Service.FindComponent(slug)!;
            Assert.Equal("Klyroo.Table", page.Package);
            Assert.Equal(api, page.Api.Select(a => a.Name));
            Assert.NotEmpty(page.Examples);
        }
        var selection = TestSupport.Service.FindComponent("data-list-selection")!;
        Assert.Contains(selection.Api[0].Parameters, p => p.Name == "MaxVisible" && p.Description is not null);
        Assert.Contains(TestSupport.Service.FindComponent("data-list")!.Api[0].Parameters, p => p.Name == "SelectedKeys" && p.Description is not null);
        Assert.Contains("### DataListSelection", File.ReadAllText(Path.Combine(TestSupport.UiRepoRoot, SkillReference.RelativePath)));
    }

    [Fact]
    public void The_component_tools_name_the_package()
    {
        Assert.Contains("dotnet add package Klyroo.Table --prerelease", TestSupport.Tools.GetComponent("DataListEmpty"));
        Assert.Contains("AddKlyrooTable()", TestSupport.Tools.GetComponent("KlyDataListEmpty"));
        Assert.Contains("AddKlyrooTable(o => o.<Property>", TestSupport.Tools.GetComponent("data-list-empty", "theme"));
        Assert.Contains("Package Klyroo.Table", TestSupport.Tools.SearchComponents("data list empty"));
        Assert.Contains("\"package\":\"Klyroo.Table\"", TestSupport.Tools.GetComponentMetadata("data-list-empty"));
        Assert.DoesNotContain("Package ", TestSupport.Tools.SearchComponents("button"));
    }

    [Fact]
    public void Llms_files_and_the_skill_reference_name_the_package()
    {
        Assert.Contains("Package Klyroo.Table: `dotnet add package Klyroo.Table --prerelease`", LlmsText.Index(TestSupport.Bundle));
        Assert.Contains("dotnet add package Klyroo.Table --prerelease", LlmsText.Full(TestSupport.Bundle));
        Assert.Contains("Package Klyroo.Table, not part of Klyroo.UI: `dotnet add package Klyroo.Table --prerelease`", SkillReference.Components(TestSupport.Bundle));
    }

    [Fact]
    public void The_committed_skill_reference_has_the_package()
    {
        var committed = File.ReadAllText(Path.Combine(TestSupport.UiRepoRoot, SkillReference.RelativePath)).Replace("\r\n", "\n");

        Assert.Contains("### DataListEmpty", committed);
        Assert.Contains("`dotnet add package Klyroo.Table --prerelease`", committed);
    }
}
