using Lux.Stack.Mcp.Markdown;
using Lux.Stack.Mcp.Model;

namespace Lux.Stack.Mcp.Tests;

/// <summary>Components of Lux.Table are documented next to those of Lux.UI, and every output says which package to add.</summary>
public class ToolsPackageTests
{
    private static BundleComponent Empty => TestSupport.Service.FindComponent("data-list-empty")!;

    [Fact]
    public void The_placeholder_is_in_the_bundle_with_its_package_and_its_theme()
    {
        Assert.Equal("Lux.Table", Empty.Package);
        Assert.Equal("dotnet add package Lux.Table --prerelease", Empty.InstallCommand());
        Assert.Equal("AddLuxTable", Empty.RegistrationMethod());

        var api = Assert.Single(Empty.Api);
        Assert.Equal("LxDataListEmpty", api.Name);
        Assert.Contains(api.Parameters, p => p.Name == "Title" && p.Description is not null);
        Assert.Contains(api.Slots, s => s.Name == "Actions");
        Assert.Equal("DataListEmpty", api.Theme!.Property);
        Assert.Equal(["root", "icon", "title", "description", "actions"], api.Theme.Slots.Keys);
        Assert.NotNull(api.Ui);
    }

    [Fact]
    public void Components_of_Lux_UI_have_no_package()
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

        Assert.Equal("Lux.Table", list.Package);
        Assert.Equal(["LxDataList<TItem>", "LxDataListTable<TItem>", "LxDataListColumn<TItem>", "LxDataListPagination<TItem>", "LxDataListResultCount<TItem>", "LxDataListRefresh<TItem>", "LxDataListErrorState<TItem>", "LxDataListSearch<TItem>"], list.Api.Select(a => a.Name));
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
            ["data-list-filters"] = ["LxDataListFilterTags<TItem>", "LxDataListAddFilter<TItem>", "LxDataListClearFilters<TItem>"],
            ["data-list-filter-panel"] = ["LxDataListFilterPanel<TItem>"],
            ["data-list-facets"] = ["LxDataListFilterEditor<TItem>"],
            ["data-list-selection"] = ["LxDataListSelectionActions<TItem>", "LxDataListActionsDropdown<TItem>", "LxDataListRowActions<TItem>"],
            ["data-list-cursor"] = ["LxDataListInfiniteLoader<TItem>"],
            ["data-list-columns"] = ["LxDataListColumnPanel<TItem>"],
        };
        foreach (var (slug, api) in pages)
        {
            var page = TestSupport.Service.FindComponent(slug)!;
            Assert.Equal("Lux.Table", page.Package);
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
        Assert.Contains("dotnet add package Lux.Table --prerelease", TestSupport.Tools.GetComponent("DataListEmpty"));
        Assert.Contains("AddLuxTable()", TestSupport.Tools.GetComponent("LxDataListEmpty"));
        Assert.Contains("AddLuxTable(o => o.<Property>", TestSupport.Tools.GetComponent("data-list-empty", "theme"));
        Assert.Contains("Package Lux.Table", TestSupport.Tools.SearchComponents("data list empty"));
        Assert.Contains("\"package\":\"Lux.Table\"", TestSupport.Tools.GetComponentMetadata("data-list-empty"));
        Assert.DoesNotContain("Package ", TestSupport.Tools.SearchComponents("button"));
    }

    [Fact]
    public void Llms_files_and_the_skill_reference_name_the_package()
    {
        Assert.Contains("Package Lux.Table: `dotnet add package Lux.Table --prerelease`", LlmsText.Index(TestSupport.Bundle));
        Assert.Contains("dotnet add package Lux.Table --prerelease", LlmsText.Full(TestSupport.Bundle));
        Assert.Contains("Package Lux.Table, not part of Lux.UI: `dotnet add package Lux.Table --prerelease`", SkillReference.Components(TestSupport.Bundle));
    }

    [Fact]
    public void The_committed_skill_reference_has_the_package()
    {
        var committed = File.ReadAllText(Path.Combine(TestSupport.UiRepoRoot, SkillReference.RelativePath)).Replace("\r\n", "\n");

        Assert.Contains("### DataListEmpty", committed);
        Assert.Contains("`dotnet add package Lux.Table --prerelease`", committed);
    }
}
