// WCAG 2.1 AA accessibility audit for Klyroo.Stack.Site using Playwright + @axe-core/playwright.
// Run in CI or locally: node build/verify-a11y.mjs [--url http://localhost:5180]
import { chromium } from 'playwright';
import AxeBuilder from '@axe-core/playwright';

const urlArgIdx = process.argv.indexOf('--url');
const baseUrl = urlArgIdx >= 0 && process.argv[urlArgIdx + 1]
  ? process.argv[urlArgIdx + 1]
  : process.env.KLYROO_SITE_URL || 'http://localhost:5180';

const routes = ['/', '/showcase', '/why', '/query/docs', '/table/docs', '/form/docs', '/dashboard/docs', '/display/docs', '/spreadsheet/docs', '/agent/docs'];

const browser = await chromium.launch();
// @axe-core/playwright needs a page created from an explicit browser context.
const context = await browser.newContext({ viewport: { width: 1440, height: 900 } });
const page = await context.newPage();

let totalViolations = 0;
for (const route of routes) {
  await page.goto(`${baseUrl}${route}`, { waitUntil: 'networkidle' });
  const results = await new AxeBuilder({ page })
    .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
    .analyze();

  const critical = results.violations.filter(v => v.impact === 'critical' || v.impact === 'serious');
  console.log(`${route.padEnd(20)} -> ${results.passes.length} rules passed, ${critical.length} critical/serious violations`);
  for (const v of critical) {
    console.error(`  [${v.impact}] ${v.id}: ${v.help} (${v.nodes.length} nodes)`);
  }
  totalViolations += critical.length;
}

await browser.close();
if (totalViolations > 0) {
  console.error(`Accessibility audit failed with ${totalViolations} critical/serious violations.`);
  process.exit(1);
}
console.log('WCAG 2.1 AA audit passed with 0 critical/serious violations!');
