/*
  The browser side of the documentation site, small and optional: everything the pages show is there without it.
    - the modifier key of the search shortcut follows the platform
    - the ecosystem menu closes on Escape and on a click elsewhere
    - the table of contents marks the heading being read
    - the demonstration board of the home page: chips pick a feature, which lights its lines of code and the parts of the results it produces,
      joined by dotted leader lines that draw themselves (the one authored motion of the site, off under prefers-reduced-motion)
*/
(() => {
  const reduceMotion = () => matchMedia("(prefers-reduced-motion: reduce)").matches;

  // The key of the shortcut: Cmd on Apple platforms, Ctrl elsewhere (the markup says Ctrl).
  if (/Mac|iPhone|iPad/.test(navigator.platform || navigator.userAgent)) {
    document.querySelectorAll("[data-mod-key]").forEach((key) => (key.textContent = "Cmd"));
  }

  // Ecosystem menu
  const eco = document.querySelector("[data-eco]");
  if (eco) {
    document.addEventListener("click", (event) => {
      if (eco.open && !eco.contains(event.target)) eco.removeAttribute("open");
    });
    eco.addEventListener("keydown", (event) => {
      if (event.key === "Escape" && eco.open) {
        eco.removeAttribute("open");
        eco.querySelector("summary").focus();
      }
    });
  }

  // Table of contents: the link of the heading being read is the first heading visible in the upper part of the page, else the last one above it.
  const toc = document.querySelector("[data-toc]");
  if (toc && "IntersectionObserver" in window) {
    const links = [...toc.querySelectorAll("a[href*='#']")];
    const entries = links
      .map((link) => ({ link, heading: document.getElementById(decodeURIComponent(link.getAttribute("href").split("#")[1] || "")) }))
      .filter((entry) => entry.heading);
    const visible = new Set();
    let current = null;
    const mark = (entry) => {
      if (entry === current) return;
      current?.link.removeAttribute("aria-current");
      current = entry;
      if (!entry) return;
      entry.link.setAttribute("aria-current", "location");
      const box = entry.link.closest(".doc-toc");
      if (box && box.scrollHeight > box.clientHeight) {
        const top = entry.link.offsetTop, bottom = top + entry.link.offsetHeight;
        if (top < box.scrollTop + 48 || bottom > box.scrollTop + box.clientHeight - 48) box.scrollTo({ top: Math.max(0, top - box.clientHeight / 3), behavior: reduceMotion() ? "auto" : "smooth" });
      }
    };
    const update = () => {
      const first = entries.find((entry) => visible.has(entry.heading));
      if (first) return mark(first);
      const line = (parseFloat(getComputedStyle(document.documentElement).scrollPaddingTop) || 80) + 8;
      let above = null;
      for (const entry of entries) {
        if (entry.heading.getBoundingClientRect().top < line) above = entry;
        else break;
      }
      mark(above || entries[0] || null);
    };
    const observer = new IntersectionObserver((changes) => {
      for (const change of changes) (change.isIntersecting ? visible.add(change.target) : visible.delete(change.target));
      update();
    }, { rootMargin: "-72px 0px -65% 0px" });
    entries.forEach((entry) => observer.observe(entry.heading));
    update();
  }

  // The board
  const SVG = "http://www.w3.org/2000/svg";
  const wide = matchMedia("(min-width: 1024px)");
  const has = (element, attribute, feature) => (element.getAttribute(attribute) || "").split(/\s+/).includes(feature);
  let uid = 0;

  function initBoard(board) {
    const chips = [...board.querySelectorAll("[data-chip]")];
    // The lines of the code are the ones of a KlyCodeBlock (data-slot="code-line", the number in data-line). The page is static, so a chip cannot change HighlightLines: the script sets
    // data-highlighted, the attribute the component writes on an emphasized line, from data-code-lines ({"feature": [line numbers]}).
    const lines = [...board.querySelectorAll('[data-slot="code-line"]')];
    let featureLines = {};
    try { featureLines = JSON.parse(board.querySelector("[data-code-lines]")?.dataset.codeLines || "{}"); } catch { featureLines = {}; }
    const targets = [...board.querySelectorAll("[data-target]")];
    const annots = [...board.querySelectorAll("[data-annot-for]")];
    const code = board.querySelector("[data-board-code]");
    const svg = board.querySelector("[data-leaders]");
    const caption = board.querySelector("[data-caption]");
    let active = chips.find((chip) => chip.getAttribute("aria-pressed") === "true")?.dataset.chip || chips[0]?.dataset.chip || "result";

    const el = (name, attributes) => {
      const node = document.createElementNS(SVG, name);
      for (const key in attributes) node.setAttribute(key, attributes[key]);
      return node;
    };

    function draw(animate) {
      svg.replaceChildren();
      if (!wide.matches || !code) return;
      const litLines = lines.filter((line) => line.dataset.highlighted === "true");
      const litTargets = targets.filter((target) => target.classList.contains("is-lit"));
      if (litLines.length === 0 || litTargets.length === 0) return;

      const frame = board.getBoundingClientRect();
      const codeBox = code.getBoundingClientRect();
      const rects = litLines.map((line) => line.getBoundingClientRect());
      const top = Math.min(...rects.map((r) => r.top)), bottom = Math.max(...rects.map((r) => r.bottom));
      // The leader starts in the middle of the lit lines; when those are not next to each other the middle can fall on a line that is not lit, so it starts at the lit line nearest to it.
      const middleOfLines = (top + bottom) / 2;
      const inside = rects.some((r) => r.top <= middleOfLines && middleOfLines <= r.bottom);
      const nearest = rects.map((r) => (r.top + r.bottom) / 2).sort((a, b) => Math.abs(a - middleOfLines) - Math.abs(b - middleOfLines))[0];
      const y1 = (inside ? middleOfLines : nearest) - frame.top;
      const x1 = codeBox.right - frame.left;
      svg.setAttribute("viewBox", `0 0 ${frame.width} ${frame.height}`);

      const defs = el("defs", {});
      svg.append(defs);
      // One leader per panel the feature touches; inside the panel every lit part shows what the code wrote there.
      const panels = new Map();
      for (const target of litTargets) {
        const panel = target.closest("[data-panel]") || target;
        panels.set(panel, [...(panels.get(panel) || []), target]);
      }
      [...panels].forEach(([panel, group], index) => {
        const panelBox = panel.getBoundingClientRect();
        const boxes = group.map((target) => target.getBoundingClientRect());
        const middle = (Math.min(...boxes.map((r) => r.top)) + Math.max(...boxes.map((r) => r.bottom))) / 2;
        const x2 = panelBox.left - frame.left;
        const y2 = Math.min(Math.max(middle, panelBox.top + 14), panelBox.bottom - 14) - frame.top;
        const gap = x2 - x1;
        const xm = x1 + gap * (0.3 + 0.4 * (panels.size === 1 ? 0.5 : index / (panels.size - 1)));
        const d = `M${x1} ${y1} H${xm} V${y2} H${x2}`;
        const id = `leader-mask-${++uid}`;
        const animated = animate && !reduceMotion();
        const reveal = el("path", { d, pathLength: "1", fill: "none", stroke: "#fff", "stroke-width": "6", "stroke-dasharray": "1", "stroke-dashoffset": animated ? "1" : "0" });
        const mask = el("mask", { id, maskUnits: "userSpaceOnUse", x: "0", y: "0", width: String(frame.width), height: String(frame.height) });
        mask.append(reveal);
        defs.append(mask);
        svg.append(el("path", { d, fill: "none", class: "leader", mask: `url(#${id})` }));
        const start = el("circle", { cx: x1, cy: y1, r: "3.5", class: "leader-dot" });
        const end = el("circle", { cx: x2, cy: y2, r: "3.5", class: "leader-dot" });
        svg.append(start, end);
        if (animated) {
          end.style.opacity = "0";
          requestAnimationFrame(() => requestAnimationFrame(() => {
            reveal.style.transition = "stroke-dashoffset 720ms cubic-bezier(0.16, 1, 0.3, 1)";
            reveal.setAttribute("stroke-dashoffset", "0");
            end.style.transition = "opacity 260ms cubic-bezier(0.16, 1, 0.3, 1) 520ms";
            end.style.opacity = "1";
          }));
        }
      });
    }

    function select(feature, animate) {
      active = feature;
      for (const chip of chips) chip.setAttribute("aria-pressed", String(chip.dataset.chip === feature));
      const lit = featureLines[feature] || [];
      for (const line of lines) {
        if (lit.includes(Number(line.dataset.line))) line.setAttribute("data-highlighted", "true");
        else line.removeAttribute("data-highlighted");
      }
      for (const target of targets) target.classList.toggle("is-lit", has(target, "data-target", feature));
      for (const note of annots) note.classList.toggle("is-lit", has(note, "data-annot-for", feature));
      const chip = chips.find((c) => c.dataset.chip === feature);
      if (caption && chip?.dataset.hint) caption.textContent = chip.dataset.hint;
      draw(animate);
    }

    for (const chip of chips) chip.addEventListener("click", () => select(chip.dataset.chip, true));
    board.setAttribute("data-live", "");
    select(active, true);
    const redraw = () => draw(false);
    new ResizeObserver(redraw).observe(board);
    wide.addEventListener("change", redraw);
    document.fonts?.ready.then(redraw);
  }

  document.querySelectorAll("[data-board]").forEach(initBoard);
})();
