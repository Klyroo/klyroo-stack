# Klyroo.Stack.Mcp

A local [Model Context Protocol](https://modelcontextprotocol.io) server that gives AI coding agents the Klyroo.UI documentation, so they read the real API instead of guessing it. It runs over stdio, offline, and answers from the documentation bundle embedded at build time: the components of Klyroo.UI and the Klyroo.Table components described on the documentation site, with parameters, events, slots, theme slots, example sources, guides and installation steps. It also carries the documentation pages (getting started, guides, concepts, platforms, FAQ and API reference) of Klyroo.Query, Table, Form, Dashboard, Display, Spreadsheet and Agent, at paths such as `/form/docs/guides`, and the pages of this hub (the templates, getting started, platforms and FAQ) at `/stack/docs/<page>`. The `resolve-module` tool answers which library, packages and `dotnet new` template own a component, a controller, a package, a repository or a template, from `KlyrooStackCatalog`.

## Install

Requires the .NET 10 runtime or newer.

```bash
dotnet tool install --global Klyroo.Stack.Mcp --prerelease
```

The tool command is `klyroo-stack-mcp`. Run `klyroo-stack-mcp --version` to check it.

## Configure a client

Claude Code:

```bash
claude mcp add --transport stdio klyroo -- klyroo-stack-mcp
```

Clients that read an `mcpServers` JSON file (Claude Desktop, Cursor) take the same command:

```json
{
  "mcpServers": {
    "klyroo": { "command": "klyroo-stack-mcp" }
  }
}
```

Append arguments with `"args": ["--tools", "search-components,get-component"]`. The server speaks MCP on stdin and stdout and writes its logs to stderr.

## What it exposes

- **Tools**, all read-only: `search-components`, `get-component`, `get-component-metadata`, `list-examples`, `get-example`, `search-documentation`, `get-documentation-page`, `search-icons`, `get-installation-guide`, `resolve-module`.
- **Prompts**: `find-component-for-usecase`, `implement-component-with-props`.
- **Resources**: `resource://klyroo-ui/components`, `resource://klyroo-ui/examples`, `resource://klyroo-ui/documentation-pages` and the template `resource://klyroo-ui/components/{slug}`.

## Options

| Option | Effect |
| :--- | :--- |
| `--tools <a,b>` | Exposes only these tools. An unknown name stops the start with exit code 2. |
| `--site-url <url>` | Puts this address in front of page paths in answers, for example `https://docs.example.com/components/modal`. |
| `--list-tools` | Prints the tool names and exits. |
| `--version` | Prints the version and exits. |
| `-h`, `--help` | Prints the usage. The examples in that text say `klyroo-stack-mcp`, but the installed command is `klyroo-stack-mcp`. |

See the [repository README](https://github.com/klyroo/klyroo-stack#readme) for the whole ecosystem.
