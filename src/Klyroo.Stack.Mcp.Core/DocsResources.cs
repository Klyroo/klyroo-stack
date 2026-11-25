using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace Klyroo.Stack.Mcp;

/// <summary>Read-only listings of the documentation, for clients that browse resources instead of calling tools.</summary>
[McpServerResourceType]
public sealed class DocsResources(DocsService docs)
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = true };

    /// <summary>The <c>resource://klyroo-ui/components</c> resource: every component with slug, name, family, description and docs path, as JSON.</summary>
    [McpServerResource(UriTemplate = "resource://klyroo-ui/components", Name = "components", Title = "Components", MimeType = "application/json")]
    [Description("Every component: slug, name, family, description and docs path.")]
    public string Components() => JsonSerializer.Serialize(
        docs.Components.Select(c => new { c.Slug, c.Name, c.Group, c.Description, path = $"/components/{c.Slug}" }), Json);

    /// <summary>The <c>resource://klyroo-ui/examples</c> resource: every example with id, title, component and description, as JSON.</summary>
    [McpServerResource(UriTemplate = "resource://klyroo-ui/examples", Name = "examples", Title = "Examples", MimeType = "application/json")]
    [Description("Every example of the docs site: id, title, component and description. Read the source with the get-example tool.")]
    public string Examples() => JsonSerializer.Serialize(
        docs.AllExamples.Select(i => new { i.Example.Id, i.Example.Title, component = i.Component.Slug, i.Example.Description }), Json);

    /// <summary>The <c>resource://klyroo-ui/documentation-pages</c> resource: the guides and component pages with their paths, as JSON.</summary>
    [McpServerResource(UriTemplate = "resource://klyroo-ui/documentation-pages", Name = "documentation-pages", Title = "Documentation pages", MimeType = "application/json")]
    [Description("The guides and the component pages of the docs site, with their paths.")]
    public string DocumentationPages() => JsonSerializer.Serialize(
        docs.Bundle.Guides.Select(g => new { path = g.Path, title = g.Title, section = g.Section })
            .Concat(docs.Components.Select(c => new { path = $"/components/{c.Slug}", title = c.Name, section = c.Group })), Json);

    /// <summary>The <c>resource://klyroo-ui/components/{slug}</c> resource: the usage, examples and API of one component as Markdown.</summary>
    /// <param name="slug">The slug of the component.</param>
    /// <returns>The Markdown page.</returns>
    /// <exception cref="McpException">No component has that slug.</exception>
    [McpServerResource(UriTemplate = "resource://klyroo-ui/components/{slug}", Name = "component", Title = "Component page", MimeType = "text/markdown")]
    [Description("The page of one component (usage, examples, API) as Markdown.")]
    public string Component([Description("The slug of the component, such as button or dropdown-menu.")] string slug)
    {
        var component = docs.FindComponent(slug)
            ?? throw new McpException($"No component '{slug}'. Read resource://klyroo-ui/components for the list.");
        return ComponentMarkdown.Page(component, ["usage", "examples", "api"], docs.SiteUrl);
    }
}
