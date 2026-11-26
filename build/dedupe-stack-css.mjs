#!/usr/bin/env node
// Removes from klyroo-stack.css every utility rule that klyroo.css already holds.
//
// Both sheets are compiled by Tailwind into `@layer utilities`, and an app loads klyroo.css first and klyroo-stack.css after it. A rule that both hold
// is declared twice in one layer, so the later copy wins over a rule of klyroo.css that sits between them: `.px-2.5` of the stack sheet came after the
// `.ps-9` of klyroo.css and took the start padding of the search field back, so the icon covered the placeholder. What the stack sheet has to carry is
// only what klyroo.css lacks.
//
//   node build/dedupe-stack-css.mjs [--check]
//
// Reads ../klyroo-ui/src/Klyroo.UI/wwwroot/klyroo.css and src/Klyroo.Stack/wwwroot/klyroo-stack.css (siblings of this repository, like the projects).
// Rewrites the stack sheet, or with --check only exits 1 when it would change (what CI and the tests use).
import { readFileSync, writeFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const uiPath = resolve(root, '../klyroo-ui/src/Klyroo.UI/wwwroot/klyroo.css');
const stackPath = resolve(root, 'src/Klyroo.Stack/wwwroot/klyroo-stack.css');

/** Splits CSS into its top-level statements: `a{...}` blocks and `@x ...;` statements, comments kept with the next one. */
export function statements(css) {
  const out = [];
  let depth = 0;
  let start = 0;
  let quote = null;
  for (let i = 0; i < css.length; i++) {
    const c = css[i];
    if (quote) {
      if (c === '\\') i++;
      else if (c === quote) quote = null;
      continue;
    }
    if (c === '"' || c === "'") quote = c;
    else if (c === '/' && css[i + 1] === '*') i = css.indexOf('*/', i + 2) + 1;
    else if (c === '{') depth++;
    else if (c === '}') {
      depth--;
      if (depth === 0) {
        out.push(css.slice(start, i + 1));
        start = i + 1;
      }
    } else if (c === ';' && depth === 0) {
      out.push(css.slice(start, i + 1));
      start = i + 1;
    }
  }
  if (css.slice(start).trim()) out.push(css.slice(start));
  return out;
}

const head = block => block.slice(0, block.indexOf('{')).trim();
const body = block => block.slice(block.indexOf('{') + 1, block.lastIndexOf('}'));

/** The rules of every `@layer utilities` block, each as its context (the at-rules around it) plus its text. */
export function utilityRules(css) {
  const rules = [];
  const walk = (list, context) => {
    for (const block of list) {
      const h = head(block);
      if (h.startsWith('@media') || h.startsWith('@supports') || h.startsWith('@container')) walk(statements(body(block)), context + h + '|');
      else if (block.includes('{')) rules.push({ key: context + block.trim(), block, context });
    }
  };
  for (const block of statements(css)) {
    if (head(block) === '@layer utilities') walk(statements(body(block)), '');
  }
  return rules;
}

/** The stack sheet without the utility rules that `ui` already holds. */
export function dedupe(stackCss, uiCss) {
  const known = new Set(utilityRules(uiCss).map(r => r.key));
  const prune = (list, context) => list
    .map(block => {
      const h = head(block);
      if (h.startsWith('@media') || h.startsWith('@supports') || h.startsWith('@container')) {
        const kept = prune(statements(body(block)), context + h + '|');
        return kept.length === 0 ? null : `${h}{${kept.join('')}}`;
      }
      return block.includes('{') && known.has(context + block.trim()) ? null : block;
    })
    .filter(block => block !== null);

  return statements(stackCss)
    .map(block => (head(block) === '@layer utilities' ? `@layer utilities{${prune(statements(body(block)), '').join('')}}` : block))
    .join('');
}

if (process.argv[1] === fileURLToPath(import.meta.url)) {
  const stack = readFileSync(stackPath, 'utf8');
  const result = dedupe(stack, readFileSync(uiPath, 'utf8'));
  if (process.argv.includes('--check')) {
    if (result !== stack) {
      console.error('klyroo-stack.css repeats rules of klyroo.css: run node build/dedupe-stack-css.mjs');
      process.exit(1);
    }
    console.log('klyroo-stack.css holds no rule of klyroo.css.');
  } else {
    writeFileSync(stackPath, result);
    console.log(`klyroo-stack.css: ${stack.length} -> ${result.length} bytes.`);
  }
}
