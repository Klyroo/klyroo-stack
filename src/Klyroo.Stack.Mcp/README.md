# Klyroo.Stack.Mcp

A local [Model Context Protocol](https://modelcontextprotocol.io) server for [Klyroo.UI](https://www.nuget.org/packages/Klyroo.UI),
for AI coding agents. It answers over stdio, offline, from the documentation of the Klyroo.UI version it was built with:
component search, parameters, events and slots, theme slots, example sources, guides and installation steps.

```bash
dotnet tool install -g Klyroo.Stack.Mcp --prerelease
claude mcp add --transport stdio klyroo-ui -- klyroo-ui-mcp
```

Tools: `search-components`, `get-component`, `get-component-metadata`, `list-examples`, `get-example`,
`search-documentation`, `get-documentation-page`, `search-icons`, `get-installation-guide`. Prompts:
`find-component-for-usecase`, `implement-component-with-props`. Resources: `resource://klyroo-ui/components`,
`resource://klyroo-ui/examples`, `resource://klyroo-ui/documentation-pages`.

Options: `--tools a,b` exposes only those tools, `--site-url https://...` prefixes page paths in answers, `--list-tools`, `--version`.
