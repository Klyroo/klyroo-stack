---
name: Klyroo library documentation sites
description: One family chrome for the eight klyroo/<name> docs sites, each proving its library with a code-to-result demonstration board in its own accent and ground texture.
colors:
  library-ui: "#10b981"
  library-query: "#ff4154"
  library-table: "#00d8ff"
  library-form: "#f59e0b"
  library-dashboard: "#8b5cf6"
  library-display: "#ec4899"
  library-spreadsheet: "#14b8a6"
  library-agent: "#6366f1"
  on-accent: "#090c11"
  accent-ink-default-light: "#067a56"
  accent-display-default-light: "#0a8f66"
  ground-light: "#f4f6f9"
  raised-light: "#ffffff"
  sunken-light: "#e6eaf0"
  code-bg-light: "#eef1f6"
  code-head-light: "#e7ebf1"
  text-light: "#1a2230"
  strong-light: "#0a1018"
  muted-light: "#465163"
  dim-light: "#5a6679"
  line-light: "#dbe1e9"
  line-strong-light: "#c1cad6"
  danger-light: "#b3321e"
  ground-dark: "#090c11"
  raised-dark: "#0f141b"
  sunken-dark: "#06080c"
  code-bg-dark: "#0c1117"
  code-head-dark: "#121922"
  text-dark: "#cdd5df"
  strong-dark: "#f1f4f8"
  muted-dark: "#96a2b3"
  dim-dark: "#8490a2"
  line-dark: "#1b222c"
  line-strong-dark: "#2b3544"
  danger-dark: "#ff8a75"
  syntax-comment-light: "#586272"
  syntax-string-light: "#0d6b3d"
  syntax-keyword-light: "#6b3fc4"
  syntax-property-light: "#9a4208"
  syntax-number-light: "#8a5a00"
  syntax-variable-light: "#a8321f"
  syntax-tag-light: "#1f55b0"
  syntax-type-light: "#0f6a7a"
  syntax-comment-dark: "#8a96a8"
  syntax-string-dark: "#7ee0a0"
  syntax-keyword-dark: "#c3a6ff"
  syntax-property-dark: "#ffa066"
  syntax-number-dark: "#ffd070"
  syntax-variable-dark: "#ff8a75"
  syntax-tag-dark: "#86b6ff"
  syntax-type-dark: "#6fd3e0"
typography:
  display:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "clamp(2.5rem, 6vw, 5.5rem)"
    fontWeight: 650
    lineHeight: 0.99
    letterSpacing: "-0.036em"
  headline:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "clamp(2rem, 1.4rem + 2.4vw, 3rem)"
    fontWeight: 650
    lineHeight: 1.08
    letterSpacing: "-0.032em"
  section:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "clamp(1.75rem, 1.25rem + 1.8vw, 2.5rem)"
    fontWeight: 650
    lineHeight: 1.1
    letterSpacing: "-0.03em"
  title:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "1.75rem"
    fontWeight: 650
    lineHeight: 1.22
    letterSpacing: "-0.028em"
  subtitle:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "1.3125rem"
    fontWeight: 620
    lineHeight: 1.22
    letterSpacing: "-0.022em"
  lead:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "clamp(1rem, 0.9rem + 0.25vw, 1.125rem)"
    fontWeight: 400
    lineHeight: 1.55
  body:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.6
  prose:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "1.0625rem"
    fontWeight: 400
    lineHeight: 1.72
  label:
    fontFamily: "Geist, ui-sans-serif, system-ui, sans-serif"
    fontSize: "0.8125rem"
    fontWeight: 600
    lineHeight: 1.45
  code:
    fontFamily: "Geist Mono, ui-monospace, SFMono-Regular, Menlo, monospace"
    fontSize: "0.8125rem"
    fontWeight: 400
    lineHeight: 1.7
  code-meta:
    fontFamily: "Geist Mono, ui-monospace, SFMono-Regular, Menlo, monospace"
    fontSize: "0.6875rem"
    fontWeight: 400
    lineHeight: 1.4
rounded:
  sm: "6px"
  control: "8px"
  md: "10px"
  lg: "14px"
  pill: "999px"
spacing:
  gutter: "24px"
  gutter-wide: "32px"
  header: "64px"
  page: "1408px"
  panel: "16px"
  board-gap: "72px"
components:
  button-accent:
    backgroundColor: "{colors.library-ui}"
    textColor: "{colors.on-accent}"
    rounded: "{rounded.md}"
    height: "42px"
    padding: "0 18px"
  button-outline:
    backgroundColor: "{colors.raised-light}"
    textColor: "{colors.strong-light}"
    rounded: "{rounded.md}"
    height: "42px"
    padding: "0 18px"
  install-chip:
    backgroundColor: "{colors.raised-light}"
    textColor: "{colors.strong-light}"
    typography: "{typography.code}"
    rounded: "{rounded.md}"
    height: "42px"
    padding: "0 12px 0 14px"
  feature-chip:
    textColor: "{colors.muted-light}"
    rounded: "{rounded.pill}"
    height: "30px"
    padding: "0 13px"
  feature-chip-pressed:
    backgroundColor: "{colors.library-ui}"
    textColor: "{colors.on-accent}"
    rounded: "{rounded.pill}"
    height: "30px"
  search-pill:
    backgroundColor: "{colors.raised-light}"
    textColor: "{colors.muted-light}"
    rounded: "{rounded.pill}"
    height: "38px"
    width: "448px"
  board-panel:
    backgroundColor: "{colors.raised-light}"
    rounded: "{rounded.lg}"
  board-code-panel:
    backgroundColor: "{colors.code-bg-light}"
    typography: "{typography.code}"
    rounded: "{rounded.lg}"
  code-block:
    backgroundColor: "{colors.code-bg-light}"
    typography: "{typography.code}"
    rounded: "{rounded.md}"
    padding: "16px 20px 18px"
  nav-link-current:
    textColor: "{colors.accent-ink-default-light}"
    rounded: "{rounded.sm}"
    padding: "5px 10px"
---

# Design System: Klyroo library documentation sites

Scope: the static docs sites built by `docs-site/Klyroo.Docs.Site` for klyroo-ui, -query, -table, -form, -dashboard, -display, -spreadsheet and -agent. The Klyroo.Stack showcase site under `site/` is a separate surface and is not covered here.

## Overview

**Creative North Star: "The Annotated Proof"**

Every site is the same calm, technical instrument: Geist on a near-black or cool off-white ground, hairline rules, Docus-style chrome (sticky header with a centered search pill, rail, column, table of contents). What changes per library is exactly three things, all driven by data in `libraries.yml` and each repo's `docs/site.yml`: the accent, the ground texture, and the demonstration board. The accent and the texture tell you which library you are on; the board tells you what it does.

The home page is built around proof. A two-line display headline (second line in the accent) sits left, the lead and the way in (accent button plus a copyable `dotnet add package` chip) sit right, and inside the first viewport at 1440x900 the board joins a C# code panel to two or three authored result panels with dotted accent leader lines. Selecting a feature chip lights the code lines and the result parts it produced. Everything below the board (showcases, the specification sheet, packages, the family list) is ruled by hairlines rather than boxed in cards. Doc pages are quieter still: the texture survives only as a band under the header.

Density is that of a developer tool, not a marketing page: small mono metadata, tabular numbers, short labels in sentence case, and results drawn as authored HTML marked "sample data" rather than screenshots.

**Key Characteristics:**
- One chromatic accent per site, derived into three contrast-checked tones (fill, ink, display).
- Both modes authored; dark is a near-black with a faint cool tint, light a cool off-white.
- Structure from 1px hairlines; only board panels and popovers are lifted.
- A ground texture taken from the library's own material, always masked away from running text.
- The demonstration board: code, chips, leader lines, lit results.

## Colors

A cool, nearly achromatic neutral system carrying a single library hue; the hue is data, the neutrals are fixed.

### Primary
- **The Library Accent** (one of `library-ui` emerald, `library-query` signal red, `library-table` electric cyan, `library-form` amber, `library-dashboard` violet, `library-display` pink, `library-spreadsheet` teal, `library-agent` indigo): written per site as `--accent` from `site.yml`. Used as a fill only: the Get started button, the mark tile, a pressed feature chip, the gutter dot on a lit code line, leader-line end dots, switches and step markers in board miniatures, sparkline points.
- **On-Accent Ink** (`on-accent`): near-black text on an accent fill; white when the accent is too dark for it (the derivation also darkens a mid-tone accent such as indigo until white reaches 4.5:1).
- **Accent Ink** (computed per library and mode; `accent-ink-default-light` is the fallback for Klyroo.UI): links, the current nav item, focus rings, the current TOC marker, leader-line strokes, `Core` package badges. Moved along lightness until it reaches 4.6:1 on the darkest surface text can sit on.
- **Accent Display** (computed; `accent-display-default-light` is the fallback): the second line of the hero headline only, held at 3.2:1 so it stays closer to the raw hue than the ink.

### Neutral
- **Ground** (`ground-light` / `ground-dark`): the page.
- **Raised** (`raised-light` / `raised-dark`): board result panels, menus, prev/next cards on hover, callout base.
- **Sunken** (`sunken-light` / `sunken-dark`): meter tracks, footer tint, the base of inline code.
- **Code Ground and Code Head** (`code-bg-*`, `code-head-*`): code blocks, the board code panel, the bar above a code block, table headers.
- **Strong / Text / Muted / Dim** (`strong-*`, `text-*`, `muted-*`, `dim-*`): headings and emphasis; body; leads, secondary copy and nav; line numbers, separators and mono notes.
- **Line and Line Strong** (`line-*`, `line-strong-*`): every hairline; the stronger one for control borders, chips and table header rules.
- **Danger** (`danger-*`): validation errors in board miniatures, invalid control borders, the over-budget meter fill.

### Tertiary: syntax and callouts
The syntax palette (`syntax-*-light` / `syntax-*-dark`) is fixed across the family, each tone at least 4.5:1 on code ground and raised. Callout kinds reuse four of its values: info uses the tag blue, success the string green, warning the number amber, error the variable red; the default callout uses the accent ink.

### Named Rules
**The One Library Hue Rule.** A site carries exactly one chromatic accent, the one in its `site.yml`. Other libraries' accents appear only as the small dots of the ecosystem menu, footer and family list.

**The Three Tones Rule.** The raw accent is for fills, never for text. Text and strokes use the accent ink, the headline's second line uses the accent display tone, and both are computed by `AccentColor` rather than picked by hand.

**The Neutral Lit Rule.** A lit code line, row, stage or block is a neutral fill (6% of strong). The accent appears only in the gutter dot and the leader lines, so a lit result never reads as an error or a selection.

## Typography

**Display Font:** Geist (with ui-sans-serif, system-ui)
**Body Font:** Geist
**Label/Mono Font:** Geist Mono (with ui-monospace, SFMono-Regular, Menlo)

**Character:** One grotesque at two temperatures: tight, heavy-ish display (weight 650 with negative tracking) over open, unhurried reading text, with Geist Mono carrying everything that is code or metadata about code.

### Hierarchy
- **Display** (650, up to 5.5rem, 0.99): the two-line hero headline, balanced, second line in the accent display tone.
- **Headline** (650, up to 3rem, 1.08): doc page title, max 24ch. **Section** (650, up to 2.5rem, 1.1) heads home sections; the close uses a 3.5rem variant at 1.04.
- **Title / Subtitle** (650 at 1.75rem; 620 at 1.3125rem; 1.22): Markdown h2 and h3. API types (h4 holding code) start under a hairline at 1.1875rem.
- **Lead** (400, 1-1.125rem, 1.55) on the home, 1.0625-1.1875rem at 1.6 on doc pages, in muted.
- **Body / Prose** (400, 1rem at 1.6 in the chrome; 1.0625rem at 1.72 in Markdown): running text held to 72ch.
- **Label** (600, 0.8125rem): panel titles, TOC heading, field labels, table headers. Sentence case, no tracking.
- **Code** (Geist Mono, 0.78-0.8125rem, 1.7) and **Code meta** (Geist Mono, 0.6875-0.75rem): file names, panel notes ("sample data", "16 lines"), feature chips, the install command, annotations.

### Named Rules
**The Tracking Follows Size Rule.** Negative tracking grows with size (-0.036em at display, -0.03em at section, -0.02em at small titles) and is zero at label size and below.

**The Mono Means Code Rule.** Geist Mono is reserved for things a developer would type or read in an editor: code, file names, package commands, chips that name API features, and notes about the code. Prose labels stay in Geist.

## Layout

A centered page of 88rem with a 1.5rem gutter (2rem from 1024px), under a sticky 4rem header split into three columns (brand, a search pill up to 28rem, actions). Below 768px the search collapses to an icon beside the actions and the ecosystem menu becomes a full-width sheet.

**Home.** The hero row is a 1.55fr / 1fr grid aligned on the bottom edge (stacked below 1024px). The board follows as a 5fr / 7fr grid with a 4.5rem column gap reserved for leader lines; "quick" boards with more code give the code at least 31rem and an even split. Sections below open with clamp(4rem, 8vw, 7rem) of top space. Showcases alternate code-and-text against a sticky result panel (5fr / 7fr, flipped every other one).

**Doc page.** One column on mobile with a sticky subbar for where you are; 15rem rail plus column from 1024px; rail, column and 14.5rem table of contents from 1280px. Rail and TOC are sticky under the header and scroll on their own.

**Rhythm.** Spacing is quiet and consistent rather than tokenized: 1rem inside panels, 1.25rem row padding in ruled lists, 2.5rem between showcases, 3.5rem above prose h2.

## Elevation & Depth

Flat by default, ruled by hairlines, with two lifted layers. Board panels (and the sticky showcase result) use a soft, wide, low-opacity panel shadow; menus use a slightly deeper pop shadow. In dark mode both shadows add a 1px inner top highlight so the panel edge catches light on near-black. The accent button carries a faint inner top highlight and an accent-tinted glow that lifts 1px on hover. The sticky header and mobile subbar use a 14px backdrop blur over an 82-90% ground so scrolling content stays legible beneath them.

### Shadow Vocabulary
- **Panel** (light `0 1px 2px rgb(15 23 42 / 0.05), 0 14px 32px -18px rgb(15 23 42 / 0.24)`; dark `0 1px 0 rgb(255 255 255 / 0.035) inset, 0 22px 44px -24px rgb(0 0 0 / 0.85)`): board panels and showcase results.
- **Pop** (light `0 2px 4px rgb(15 23 42 / 0.06), 0 20px 44px -16px rgb(15 23 42 / 0.3)`; dark `0 1px 0 rgb(255 255 255 / 0.05) inset, 0 28px 56px -20px rgb(0 0 0 / 0.9)`): the ecosystem menu and menus drawn inside boards.

### Named Rules
**The Hairline First Rule.** Lists of things (features, packages, showcases, family) are separated by a 1px line, not wrapped in cards. Only the board's panels and floating menus earn a shadow.

**The Ground Stays Behind Rule.** The library texture never sits behind running text. On doc pages it is a 6rem band under the header that fades out before the breadcrumb; on the home it lives in the gutters and below the top of the board, masked clear beneath the headline and lead. Its strongest line is 13% (light) or 15% (dark) of the accent.

## Shapes

Gently rounded, never soft. Three radii carry the chrome: 6px for nav items, inline code and small buttons; 10px for buttons, the install chip, code blocks, callouts and tables; 14px for board panels. Board miniatures (fields, menu buttons, the send button) use 8px, a step between. Pills (999px) are reserved for the search, feature chips, tags, facets, layer badges and annotations. Circles mark state: accent dots on lit lines, library dots in menus, radios and steps. Leader lines are dotted (1.5 on, 5 off, round caps, 1.5px) and end in an accent dot ringed in the ground color.

Each library's ground texture is drawn from its own material: blueprint grid (ui), dot matrix (query), ruled rows (table), field baseline grid (form), chart ticks (dashboard), halftone (display), cell grid paper (spreadsheet), node dots on a sparse graph (agent).

## Components

### Buttons
- **Shape:** gently curved (10px), 2.625rem tall, 1.125rem side padding, 0.9375rem at 600, optional trailing arrow icon.
- **Accent:** accent fill with on-accent text, inner highlight and accent glow; rises 1px on hover.
- **Outline:** line-strong border over a translucent raised fill, strong text; border shifts toward the accent ink on hover.
- **Focus:** 2px accent-ink outline, 3px offset.
- **Install chip:** the outline button's shape holding the `dotnet add package` command in Geist Mono and a copy icon that turns into an accent-ink check when copied.

### Chips
- **Feature chip (board):** a mono pill with a line-strong border and muted text; hover brightens text and tints the border; pressed fills with the accent and switches to on-accent text. Inert until the board script is live.
- **Tags, facets, layer badges:** small pills with a line-strong border on the ground; the `Core` layer badge takes the accent ink.

### Cards / Containers
- **Board panel:** 14px radius, 1px line border, raised fill (code panel on code ground), panel shadow. A 2.25rem head bar on a code-head tint carries a label-size title or a mono file name, and a mono dim note on the right ("sample data", "16 lines", "URL form of the same state").
- **Prev/next:** 10px radius, 1px line, translucent raised fill, accent-ink arrow; border tints toward the accent on hover.
- **Callout:** 10px radius, a 30% hairline and a 7% tint of its kind color over raised, the title in that color at 650.

### Inputs / Fields
- **Search pill:** 999px radius, line-strong border, translucent raised fill, muted label and Ctrl K keycaps; hover moves the border toward the accent ink.
- **Board miniatures:** 8px fields on the ground with line-strong borders; invalid fields take a danger border and a danger message below; radios, switches and steps fill with the accent when on.

### Navigation
- **Header:** mark tile in the accent with `klyroo/<name>` wordmark (dim slash), centered search, then Docs, API, the Klyroo ecosystem menu (each library with its accent dot), GitHub and theme toggle as 2.25rem icon buttons with an accent-soft hover.
- **Rail:** groups with icons and chevrons, pages hanging off a 1px line; hover is an accent-soft fill; the current page is accent ink at 600 on accent-soft with a 1px accent-ink marker on the line.
- **TOC:** 0.8125rem muted links on a 1px line, the current section in accent ink with the same 1px marker.
- **Mobile:** a sticky subbar under the header names the current page and opens the rail; the TOC becomes a collapsible box above the article.

### Code Block
10px radius on code ground with a code-head bar (mono file title left, ghost copy button right); 0.8125rem Geist Mono at 1.7; comments italic. Copy turns accent ink with a check when done.

### Demonstration Board (signature)
A code panel with numbered lines and a strip of feature chips with a one-line caption, beside two or three result panels. Choosing a chip lights the producing lines (neutral fill, accent gutter dot, bright line number) and the matching result parts, draws dotted accent leader lines from the lit lines to them across the column gap, and may show a mono annotation pill under the result. Results are authored HTML in the site's own look, labelled sample data. Leader lines hide below 1024px; the board is complete without its script. Lit transitions run 360ms on the shared ease-out curve, and annotation motion is dropped under reduced motion.

## Do's and Don'ts

### Do:
- **Do** take the accent and texture from `libraries.yml` / `site.yml` and let `AccentColor` derive the ink, display and on-accent tones for both modes.
- **Do** keep the board, headline, lead and install chip inside the first viewport at 1440x900.
- **Do** mark lit results with the neutral lit fill and put the accent only in gutter dots, leader lines and fills.
- **Do** separate list content with 1px hairlines and keep running text to 72ch.
- **Do** label drawn results as sample data in the panel head note, and draw them as HTML in the site's look.
- **Do** use the shared ease-out curve (cubic-bezier(0.16, 1, 0.3, 1)) at 140-200ms for hover and 360ms for lit states.

### Don't:
- **Don't** set text in the raw accent; use the accent ink or, for the headline's second line only, the accent display tone.
- **Don't** introduce a second chromatic accent on a site, or reuse another library's accent beyond its menu dot.
- **Don't** let the ground texture run behind the headline, lead, breadcrumb or prose.
- **Don't** wrap features, packages or showcases in shadowed cards; only board panels and menus are lifted.
- **Don't** use hard offset shadows; elevation here is soft and diffuse.
- **Don't** add uppercase tracked labels or eyebrows above headings; labels are sentence case at 600.
- **Don't** put screenshots in the board or invent numbers in it.
