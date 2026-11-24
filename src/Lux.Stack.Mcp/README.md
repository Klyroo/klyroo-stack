# Lux.Stack.Mcp

A local [Model Context Protocol](https://modelcontextprotocol.io) server for [Lux.UI](https://www.nuget.org/packages/Lux.UI),
for AI coding agents. It answers over stdio, offline, from the documentation of the Lux.UI version it was built with:
component search, parameters, events and slots, theme slots, example sources, guides and installation steps.

```bash
dotnet tool install -g Lux.Stack.Mcp --prerelease
claude mcp add --transport stdio lux-ui -- lux-ui-mcp
```

Tools: `search-components`, `get-component`, `get-component-metadata`, `list-examples`, `get-example`,
`search-documentation`, `get-documentation-page`, `search-icons`, `get-installation-guide`. Prompts:
`find-component-for-usecase`, `implement-component-with-props`. Resources: `resource://lux-ui/components`,
`resource://lux-ui/examples`, `resource://lux-ui/documentation-pages`.

Options: `--tools a,b` exposes only those tools, `--site-url https://...` prefixes page paths in answers, `--list-tools`, `--version`.
