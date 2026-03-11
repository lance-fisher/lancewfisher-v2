# lancewfisher-v2 — TODO

**Context:** Light theme implementation ~80% complete on `integration/master-merge` branch (11+ commits ahead of master).
**STOPPOINT:** `D:\ProjectsHome\lancewfisher-v2\STOPPOINT.md` — read this first for full technical context.
**IMPORTANT:** Do NOT switch branches. Stay on `integration/master-merge`. All changes are unstaged/uncommitted.

---

## Phase 1: Finish Light Theme Validation

- [ ] **Visual validation — light mode (resume from #sec-now)**
  - Start preview server (config in `.claude/launch.json`, port 8070/8077)
  - Force light theme: `document.documentElement.setAttribute('data-theme', 'light')`
  - Hero and About sections already validated — look good
  - Validate remaining sections in order:
    - `#sec-now` (Now / Current Focus)
    - `#sec-method` (Method)
    - `#sec-projects` / `#projectGallery` (Projects with canvas thumbnails)
    - `#ftda-section` (FTDA card)
    - `#sec-ventures` (Ventures)
    - `#sec-noco` (NoCo)
    - `#sec-brand` (One-Three brand)
    - `#sec-architecture` (Architecture diagram SVG)
    - `#sec-principles` (Principles)
    - `#sec-sovereignty` (Sovereignty)
    - `#sec-contact` (Contact form)
  - Fix any sections that don't look right in light mode

- [ ] **Dark mode regression check**
  - Switch back to dark and verify nothing broke
  - All canvas changes are behind `if (light)` / `if (p.light)` guards so risk is low
  - Still needs visual confirmation

- [ ] **Present light theme to user for approval**
  - Take screenshots of all sections in light mode
  - Get explicit approval before committing

## Phase 2: Waiting on User

- [ ] **Background watermark image**
  - User saw current CSS-only Roman statue watermark (24 layered radial gradients at 0.25 opacity)
  - User said they would provide a replacement image — not yet provided
  - Can proceed with deploy without this (current one works)

## Phase 3: Ship It

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
| `styles.css` | 2 blocks: project-thumb-wrap light bg + architecture SVG/principles light overrides |
| `.htaccess` | 1 edit: CSP Google Fonts domains |
| `ftda/ftda.js` | 5 edits: bg/glow/grid theme-aware, doc mockup/text/stats theme-aware, initFtdaCanvas global |

## Key Technical Notes
- **Theme in canvas:** `pal()` helper returns `.light` boolean. FTDA uses local `var light = document.documentElement.getAttribute('data-theme') === 'light'`
- **Canvas re-render on toggle:** `main.js` line 126-127 calls `initProjectCanvases()` + `initFtdaCanvas()` after `setTheme()`
- **Light palette:** Silver and brass. User rejected pure white and warm cream. Base `#c5c6c8`, brass accent `#7a6345`
