# lancewfisher-v2 — TODO

**Context:** Light theme implementation ~95% complete on `integration/master-merge` branch (11+ commits ahead of master).
**STOPPOINT:** `STOPPOINT.md` in project root — read this for prior session context.
**IMPORTANT:** Do NOT switch branches. Stay on `integration/master-merge`. All changes are unstaged/uncommitted.

---

## Phase 1: Finish Light Theme Validation

- [x] **CSS audit of all light mode overrides (2026-04-01)**
  - Systematic audit of every section from `#sec-now` through `#sec-contact`
  - Cross-referenced all hardcoded `rgba(240, 235, 224, ...)` text colors against `[data-theme="light"]` overrides
  - Verified all CSS custom properties (`--warm-white`, `--warm-white-dim`, `--fg`, `--gold-1776`) resolve correctly in light mode
  - Verified 130+ existing light mode rules cover all major sections

- [x] **Fixed 4 missing light mode overrides (2026-04-01)**
  1. `.project-card:hover .p-desc` -- was near-white on hover in light mode, now dark text
  2. `.booking-select:focus` -- missing focus state styling in light mode
  3. `.booking-select option` -- dropdown options had same bg and text color (#1a1a1c) in light mode, now readable
  4. `.p-status` (base) -- FTDA "Protected" badge had invisible text in light mode

- [x] **Dark mode regression check (2026-04-01)**
  - All 4 fixes are scoped under `[data-theme="light"]` selectors, zero dark mode impact
  - No existing rules were modified, only new rules added
  - All canvas changes from prior session remain behind `if (light)` / `if (p.light)` guards

- [ ] **Visual validation via live preview**
  - CSS audit complete; live browser validation recommended before deploy
  - Start preview server, toggle light mode, scroll through all sections
  - Pay attention to: project card hovers, contact form select dropdown, FTDA "Protected" badge

- [ ] **Present light theme to user for approval**
  - Take screenshots of all sections in light mode
  - Get explicit approval before committing

## Phase 2: Waiting on User

- [ ] **Background watermark image**
  - User saw current CSS-only Roman statue watermark (24 layered radial gradients at 0.25 opacity)
  - User said they would provide a replacement image — not yet provided
  - Can proceed with deploy without this (current one works)

## Phase 3: Ship It

- [ ] **Sync root to deploy/** — `styles.css` has diverged (4 new light mode rules in root not in deploy)
  - `main.js` and `index.html` are in sync
  - Run: `cp styles.css deploy/styles.css`

- [ ] **Commit all changes on `integration/master-merge`**

- [ ] **Merge to `master`**

- [ ] **Push both branches to GitHub**

- [ ] **Deploy to Hostinger**
  - SFTP or git push to hosting
  - Post-deploy verification: fonts, contact form, theme toggle, sub-pages, HTTPS, 404

## Files Modified (Uncommitted)

| File | Changes |
|---|---|
| `main.js` | 5 edits: drawPlatformer var fix, drawFisher13/drawNoCo/drawMarketDash/drawAuton theme-aware bg |
| `styles.css` | Light mode: project-thumb-wrap, arch SVG, principles, + 4 new fixes (hover p-desc, select focus/option, p-status base) |
| `.htaccess` | 1 edit: CSP Google Fonts domains |
| `ftda/ftda.js` | 5 edits: bg/glow/grid theme-aware, doc mockup/text/stats theme-aware, initFtdaCanvas global |

## Key Technical Notes
- **Theme in canvas:** `pal()` helper returns `.light` boolean. FTDA uses local `var light = document.documentElement.getAttribute('data-theme') === 'light'`
- **Canvas re-render on toggle:** `main.js` line 126-127 calls `initProjectCanvases()` + `initFtdaCanvas()` after `setTheme()`
- **Light palette:** Silver and brass. User rejected pure white and warm cream. Base `#c5c6c8`, brass accent `#7a6345`
- **Deploy sync:** Root files are the source of truth. `deploy/` must be synced via `cp` before shipping.
