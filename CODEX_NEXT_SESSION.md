# Codex Next Session Prompt

## Context

You are continuing work on `D:\ProjectsHome\FisherSovereign\lancewfisher-v2`, a static HTML/CSS/JS portfolio site for Lance W. Fisher / Fisher Sovereign Systems. The site is served from `deploy/` which is the authoritative source of truth. The root directory contains mirrors for git tracking. **All edits happen in `deploy/` first, then sync to root with `cp`. Never copy root -> deploy.**

The prior session consolidated two parallel sessions (booking interactivity + product ecosystem pages), then did a polish pass (card reorganization, canvas thumbnails, benefit text, wordmark consistency). Two commits were pushed to `origin/master`. Read `HANDOFF.md` for full details.

## Your Tasks

Complete the following in order. Read `CLAUDE.md` and `HANDOFF.md` first.

### Task 1: Full Visual and Responsive Review

Do a complete top-to-bottom review of the website. Serve `deploy/` on port 8077 (`python -m http.server 8077 --directory deploy`).

For each page below, verify:
- Page loads without console errors
- Layout looks correct at desktop (1280px), tablet (768px), and mobile (375px)
- Dark mode (default) renders correctly
- Light mode renders correctly (toggle or `document.documentElement.setAttribute('data-theme','light')`)
- No broken links, dead buttons, or missing assets
- Typography is consistent (Cormorant Garamond for headings, Inter for body, JetBrains Mono for code)
- Spacing between sections is consistent

Pages to review:
1. `deploy/index.html` (gateway)
2. `deploy/home.html` (main portfolio, 26 project cards)
3. `deploy/command-center/index.html` (6-tab dashboard)
4. `deploy/ai-orchestrator/index.html` (flow diagrams, model routing)
5. `deploy/governance/index.html` (tier table, session states)
6. `deploy/harmony/index.html` (4-step booking flow)
7. `deploy/noco/index.html` (phone mockup, 5 tabs)
8. `deploy/pip/index.html` (AI workstation)
9. `deploy/sovereign-hub/index.html` (project directory)
10. `deploy/sovereign-signal/index.html` (encrypted messenger)
11. `deploy/operator-console/index.html` (control plane)
12. `deploy/home-hub/index.html` (network monitor)

On `deploy/home.html` specifically:
- Verify all 26 project card detail panels expand/collapse when clicked
- Verify category filter buttons (All, Apps, Tools, Games, Trading) show the correct cards
- Verify the "View Full Portfolio" expand button reveals the 9 hidden cards
- Verify all 22 canvas thumbnails render (check for blank white/black rectangles)

Fix any issues you find. Document fixes.

### Task 2: Wordmark Visual Treatment on Sub-Pages

The company wordmark on the gateway (`deploy/index.html`) and home (`deploy/home.html`) uses a styled two-line treatment:

```html
<span class="fs-business-stack">
  <span class="fs-business-line1" data-text="Fisher">Fisher</span>
  <span class="fs-business-line2" data-text="Sovereign Systems">Sovereign Systems</span>
</span>
```

This has metallic gradient, letter-spacing, and subtle animation defined in `deploy/styles.css` under `.gw-company`, `.fs-business-stack`, `.fs-business-line1`, `.fs-business-line2`.

The 3 new pages (command-center, ai-orchestrator, governance) and other sub-pages use plain text "FISHER SOVEREIGN SYSTEMS" in their nav bars and footers. The user wants these to visually match the gateway/home treatment.

For each sub-page that has "Fisher Sovereign Systems" in its header or footer:
1. Replace the plain text with the `.fs-business-stack` HTML structure
2. Add the necessary CSS rules to the page's self-contained `<style>` block (extract from `deploy/styles.css`)
3. Keep the treatment appropriately sized for the context (nav bar vs hero vs footer)
4. Test in both dark mode and light mode
5. Do NOT add a dependency on `deploy/styles.css`. Each page's CSS must remain self-contained.

### Task 3: After All Edits

1. Verify no em-dashes in any changed file: `grep -r "\xe2\x80\x94" deploy/*.html deploy/*/*.html`
2. Verify wordmark consistency: `grep -r "Fisher Sovereign" deploy/ --include="*.html" | grep -v "Fisher Sovereign Systems"` should return nothing
3. Sync all changed deploy/ files to root: `cp deploy/<file> <file>` for each changed file
4. Commit with descriptive message
5. Push to origin

## Rules

- `deploy/` is authoritative. All edits there first, then sync to root.
- Never run `build-deploy.sh`.
- Never copy root -> deploy.
- No em-dashes anywhere. Use comma, colon, period, or restructure.
- No innerHTML. Use safe DOM methods.
- Every CSS change must work in both dark and light mode.
- Self-contained CSS in sub-pages is intentional. Do not add `<link>` to styles.css.
- Use `rgba(var(--fg), alpha)` for text colors when `--fg` is available. Never hardcode `rgba(240,235,224,...)`.
- Alpha guidance: 0.85+ primary, 0.65-0.75 secondary, 0.45-0.55 muted, below 0.40 decorative only.
