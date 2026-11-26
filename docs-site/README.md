# Documentation sites of the Klyroo libraries

One site, `Klyroo.Docs.Site`, renders the documentation of any Klyroo library from its repository, and a static export turns the result into the files GitHub Pages serves. Each library is published at `https://klyroo.github.io/<repo>/` (`klyroo-table` at `https://klyroo.github.io/klyroo-table/`), by the workflow `.github/workflows/docs.yml` of its own repository.

The site is the Markdown docs template of Klyroo.UI (`klyroo-ui/templates/docs`): Markdig and YamlDotNet, the navigation tree, the table of contents, `KlyContentSearch` with a `search-index.json` file, color mode, static server rendering only and `PathBase`. The code in `Klyroo.Docs.Site/Content` is derived from the template's shared Markdown code. What changed is described under [What differs from the template](#what-differs-from-the-template).

## What a library provides

| File | Becomes |
| --- | --- |
| `README.md` | The home page, below a hero with the title, the tagline and the install command. The badges at the top are left out: the header links to GitHub and NuGet instead. |
| `docs/README.md` | The page `documentation`, the index of the docs. It comes first in the navigation. |
| `docs/<name>.md` | The page `<name>`. Folders below `docs/` make folders of addresses (`docs/a/b.md` is `a/b`). |
| `docs/site.yml` | The settings of the site (optional, described below). |

The files are read as they are, with no front matter. The title of a page is its first `#` heading, which is taken out of the body so that the page has one `h1`. The description for search engines and the search is its first paragraph. The docs files are never rewritten: the front matter, the routes and the links are produced in memory and in the output.

## Preview one library

From a folder that holds `klyroo-stack`, `klyroo-ui` and the library side by side:

```bash
dotnet run -c Release --project klyroo-stack/docs-site/Klyroo.Docs.Site -- --repo klyroo-table
```

Open `http://localhost:5000/`. The first build downloads the Tailwind CLI, the fonts and the icons, as every Klyroo.UI site does, and keeps them in caches. The site reads the files when it starts, so restart it after an edit. A relative `--repo` is relative to the folder you typed the command in (on Windows give a full path).

## Export the static site

```bash
dotnet run -c Release --project klyroo-stack/docs-site/Klyroo.Docs.Site -- export --repo klyroo-table --out _site
```

The command starts the site on a free local port, below the path it will have on GitHub Pages (`/klyroo-table/`, from the name of the repository folder, the `repo` setting of `site.yml` or `--name`), and writes it to `_site`:

- every page as `<route>/index.html`, which a static server answers for `/route` and `/route/`, and the home page as `index.html`;
- `404.html`, taken from the answer of the site to an unknown address, and `.nojekyll`;
- `search-index.json`;
- every file the pages refer to, found by looking inside the HTML (`href`, `src`, `srcset`, the import map, the list of JavaScript initializers that Blazor writes in a comment, `<style>` elements and `style` attributes), the CSS (`url()`, `@import`) and the JavaScript (`import`, `import()`, `new URL(..., import.meta.url)`): the fingerprinted stylesheet and scripts, the fonts, the files of `_content/Klyroo.UI`.

The export fails, lists what it found and writes nothing when:

- a page, a stylesheet, a script, a font or any other file the pages refer to does not answer 200;
- a link in a page leads to an address that does not exist, to a fragment (`#anchor`) the target page does not have, or outside the path base;
- a link in the Markdown leads to a file or folder that is not in the repository.

The Blazor script (`_framework/blazor.web.js`) is copied but not searched for more files: it names the files of runtimes this static site does not run, such as `dotnet.js` of WebAssembly.

To look at the result as GitHub Pages will serve it, put the export in a folder named like the site and serve the parent:

```bash
dotnet run -c Release --project klyroo-stack/docs-site/Klyroo.Docs.Site -- export --repo klyroo-table --out preview/klyroo-table
python3 -m http.server 8000 --directory preview     # http://localhost:8000/klyroo-table/
```

`python3 -m http.server` answers an unknown address with its own page, where GitHub Pages sends `404.html`.

## Settings: `docs/site.yml`

Every key is optional, and a key that does not exist is an error. A library keeps this file in its own `docs/` folder, so each site can be polished without touching this repository.

| Key | What it does |
| --- | --- |
| `title` | The name in the header and on the home page. Default: the first heading of the README. |
| `tagline` | The line under the title on the home page. Default: the first paragraph of the README, cut at 200 characters. |
| `description` | The meta description of the home page. Default: the first paragraph of the README. |
| `install` | The command shown next to the title on the home page. A block scalar (`install: \|`) shows several lines. |
| `packages` | The NuGet package ids, each linked from the header (`https://www.nuget.org/packages/<id>`). |
| `github` | The address of the repository. Default: `https://github.com/klyroo/<repo>`. |
| `branch` | The branch links to files of the repository and the "Edit this page" link point at. Default: `main`. |
| `accent` | The primary color: the name of a Tailwind palette (`blue`, `emerald`, `violet`...). Default: the Klyroo colors. |
| `repo` | The name of the repository, which is the path of the site. Default: the name of the folder. |
| `nav` | The pages in the order of the navigation (below). |

An entry of `nav` is a file name below `docs/` without `.md`, or a map:

```yaml
nav:
  - group: Tutorial              # a group of entries, collapsible in the navigation
    pages:
      - getting-started
  - group: Reference
    pages:
      - page: api-reference
        title: API reference     # replaces the first heading of the page
        navTitle: API            # the text of the link, when it should be shorter
        description: Every public type and member.   # the line under the title
        tocDepth: 4              # deepest heading in the table of contents and the search (2 to 6, default 3)
        indexText: false         # keep the text of the page out of the search, keep its headings
      - faq
```

Pages that are not listed follow in the order of their file names, and `docs/README.md` is always first. A page the file lists that does not exist stops the build and names the pages there are. A navigation label that starts with the name of the library (`Klyroo.Table API reference`) is shortened to what follows it (`API reference`).

## Links

The README and the docs are written for GitHub, and their links keep working there. On the site:

| In the Markdown | On the site |
| --- | --- |
| `guides.md#anchor`, `docs/guides.md`, `../README.md#installation`, `README.md` of the docs index | The route of the page with the fragment kept (`guides/#anchor`, `./#installation`, `documentation/`). |
| `../src/...`, `../samples/...`, `../CHANGELOG.md`, a folder | `https://github.com/klyroo/<repo>/blob/<branch>/<path>` (`tree` for a folder). A path that is not in the repository fails the export. |
| `https://github.com/klyroo/<this repo>/blob/main/docs/<page>.md#anchor` | The route of that page. |
| `https://github.com/klyroo/<another library>` or its `docs/<page>.md` | The page of that library's site, `https://klyroo.github.io/<another library>/<page>/`. The fragment is not checked: that site's own export checks its anchors. |
| Any other address | Unchanged. |

Heading ids follow GitHub (`## Install the CLI` is `#install-the-cli`, a repeat gets `-1`), so the anchors written for GitHub keep working. The addresses of pages end with a slash, so a static server answers them without a redirect.

## The API reference

`api-reference.md` is generated and very large (Klyroo.UI's is over 500 KB). It is one page, like the file: the table of contents lists the packages, the kinds and the types (`tocDepth: 4`) in a box that scrolls on its own, and the search has one entry per type, from its heading, without the text of the page (`indexText: false`). The search index of Klyroo.UI is about 43 KB (12 KB compressed).

## The libraries in the header

`Klyroo.Docs.Site/libraries.yml` lists the libraries every site links to in its header (`Klyroo` menu). To add a library, add it there and add its own `docs.yml` workflow (copy one from another repository).

## Publishing

`.github/workflows/docs-pages.yml` is a reusable workflow: it checks out the calling repository, `klyroo-stack` and `klyroo-ui` side by side, installs .NET 10, runs the export and publishes it with `actions/upload-pages-artifact` and `actions/deploy-pages`. Each library calls it from `.github/workflows/docs.yml` on a push to `main` that touches `docs/**` or `README.md`, or by hand:

```yaml
jobs:
  docs:
    uses: Klyroo/klyroo-stack/.github/workflows/docs-pages.yml@main
    with:
      repo: klyroo-table
```

The workflows do not change any setting. GitHub Pages has to be switched to the source "GitHub Actions" in the settings of each library repository once. A change to the site itself (this folder) is not republished by the libraries: run their `Docs` workflow by hand, or push to their docs.

## What differs from the template

- The pages come from a repository layout and `site.yml`, not from a `content/` folder with front matter: `LibraryDocs` instead of `MarkdownSite.Load`, with no `_dir.yml` and no file watcher.
- `MarkdownRenderer` can take the first `#` heading out of a page as its title, report the first paragraph, and ask a `LinkResolver` where each link leads (`RenderOptions`).
- The header has the libraries and NuGet menus and a GitHub button, and every page has an "Edit this page on GitHub" link.
- Enhanced navigation is switched off (`data-enhance-nav="false"`): Blazor fetches a page for it, and a static host and the script do not agree about the address.
- `Export/StaticExporter.cs` is new.

The tests are in `tests/Klyroo.Docs.Site.Tests` (a fake repository on disk and a fake server).
