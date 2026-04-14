# Bloodlines Web Session Handoff — 2026-04-13

## Context

This handoff captures the state of the Bloodlines audit and build session at the moment it was committed and pushed so the operator can continue on web. The original desktop session was invoked with an "analyze, expand bible, rebuild portable context, define parallel protocol, issue next directions, and continue advancing implementation" directive. Partial delivery landed; the remainder is scoped below for the continuation session.

## Canonical Root

`D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines\`

All Bloodlines work happens here. The root-level `lancewfisher-v2/bloodlines/` is a stale mirror used only for git tracking (the portfolio's `.gitignore` excludes `deploy/`). Edit in `deploy/bloodlines/`, then `cp` the changed files into `bloodlines/`.

## What This Session Landed

1. **Full audit** of the canonical deploy/bloodlines tree, including:
   - Every numbered archive directory (00_ADMIN through 19_ARCHIVE)
   - All canon files (CANONICAL_RULES.md, CANON_LOCK.md, MASTER_MEMORY.md, Design Bible v3.2)
   - All production docs (Development Reality Report, Implementation Roadmap, Definitive Decisions Register, Completion Stage Gates, Known Issues, Runbook, Project Inventory, Consolidation Note)
   - Runtime modules: play.html, src/game/main.js, src/game/core/{simulation,renderer,ai,data-loader,utils}.js, src/game/styles.css
   - All data JSON: houses, units, buildings, resources, faiths, conviction-states, bloodline-roles, bloodline-paths, victory-conditions, maps/ironmark-frontier

2. **Runtime validation (2026-04-13)**:
   - `node --check` passed on every JS module.
   - `node tests/data-validation.mjs` passed when run from `deploy/bloodlines/`. (Runbook note: the runbook tells you to run from repo root, but the test uses `process.cwd()`. Run from `deploy/bloodlines/` instead.)
   - `python -m http.server 8078 --directory deploy/bloodlines` serves the prototype cleanly.
   - `play.html` autoloads, game shell visible in ~2s, population ticks, food and water trickle, Stonehelm AI captures Stonefield Watch, frontier tribes raid on timer, Dynasty and Faith panels render populated content. Zero console errors, zero failed network requests.

3. **State analysis report** written at:
   - `deploy/bloodlines/docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md`
   - `bloodlines/docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` (root mirror for git)

   The report includes:
   - Verified top-line findings
   - JS syntax, data-validation, runtime boot verification results
   - A full Systems Implementation Matrix mapping every canonical system to LIVE / PARTIAL / DATA-ONLY / DOCUMENTED / CANON-OPEN / VOIDED
   - Authoritative document stack and fragmentation notes
   - Completion Stage Gates status as of 2026-04-13
   - What was recently attempted but not fully validated
   - Risks, contradictions, open gaps
   - Immediate next engineering focus

## What Still Needs To Land (Web Session Work)

The original desktop session was asked to produce five deliverables. Only one landed before the commit-and-push request. The remaining four are scoped here so the web session can continue without re-discovery.

### Deliverable 2: Expanded Working Bible

Target: extend `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md` OR create `BLOODLINES_BIBLE_WORKING_EXPANDED.md` alongside it.

What to add, additively (do not rewrite):
- Implementation-Aware Notes section per locked system, pointing to the runtime location (file:line) where each system is wired, and what portion is still documentation-only.
- Cross-reference markers to `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` Section 3 (Systems Implementation Matrix).
- A "Runtime Bridge Appendix" describing how the data-driven content pipeline (JSON in `data/`, loader in `src/game/core/data-loader.js`, simulation in `src/game/core/simulation.js`) maps to canonical systems.

### Deliverable 3: Full Portable Context

Target: `docs/BLOODLINES_FULL_PORTABLE_CONTEXT.md` (new file).

Must be self-contained and usable by any external model or engineering session. Structure:
1. What Bloodlines is (one dense paragraph, no marketing voice).
2. Core pillars (Dynasty, Faith, Conviction) preserved exactly as canonically locked.
3. Nine founding houses with current canon status and prototype playability.
4. Four covenants (Old Light, Blood Dominion, The Order, The Wild) with Doctrine Path architecture.
5. Seven core systems (Conviction, Faith, Population, Resource, Territory, Dynastic, Born of Sacrifice).
6. Five match stages, three match phases, multi-speed time, dual-clock architecture.
7. Five victory paths plus the voided Dynastic Prestige (now modifier).
8. Current implementation state (pull directly from the State Analysis report's matrix).
9. Technical architecture (static site, ES modules, canvas, no build step, JSON-driven content).
10. Known gaps and open decisions (pull from `DEFINITIVE_DECISIONS_REGISTER.md`).
11. Active constraints (no scope reduction, additive archival rules, `deploy/bloodlines/` as canonical root, dual-mode dark/light rule where relevant on portfolio integration points).
12. What should be built next (pull from State Analysis Section 8).
13. What not to misunderstand: Bloodlines is not an MVP, is not a generic RTS, and is not a dynasty manager — it is a medieval RTS with dynastic, faith, territorial, and strategic consequences across long-form matches.

### Deliverable 4: Parallel Execution Protocol

Target: `docs/BLOODLINES_PARALLEL_EXECUTION_PROTOCOL.md` (new file).

Must define how Claude Code and Codex can work concurrently on Bloodlines without collision. Recommended structure:

- Lane Ownership Model
  - Claude Code lanes: design continuity, bible updates, portable context, architecture mapping, system specification, data schema, integration validation, next-direction curation.
  - Codex lanes: subsystem scaffolding, engine/system coding, mechanical prototypes, UI/HUD implementation, data-model integration, forward feature advancement.
- File Ownership (non-exclusive defaults, can be overridden per task):
  - Claude-first: `docs/*.md`, `01_CANON/*`, `18_EXPORTS/*`, `02_SESSION_INGESTIONS/*`, `docs/BLOODLINES_*.md`.
  - Codex-first: `src/game/**/*.js`, `src/game/styles.css`, `data/**/*.json`, `tests/*.mjs`, `play.html`.
  - Shared with coordination: `docs/DEFINITIVE_DECISIONS_REGISTER.md`, `docs/IMPLEMENTATION_ROADMAP.md`, `docs/KNOWN_ISSUES.md`, `docs/COMPLETION_STAGE_GATES.md`.
- Branch strategy: short-lived topic branches off `master`; phone-sessions continue to land on `claude/*` branches per the existing Bloodlines pattern.
- Anti-collision rules: before editing a shared file, write a 1-line intent to `docs/BLOODLINES_IN_FLIGHT.md` (new rolling register). After edit, clear the intent. If more than one agent writes to the same file in the same day, require a manual reconciliation pass.
- Handoff discipline: every session writes a dated `SESSION_HANDOFF_YYYY-MM-DD.md` into `docs/` recording files changed, what's now green, what's now red, and what the next most-useful step is.
- Continuity: on session start, the reader follows the doc order defined in State Analysis Section 4.1.

### Deliverable 5: Next Directions File

Target: `docs/BLOODLINES_NEXT_DIRECTIONS_AFTER_CODEX_REVIEW.md` (new file).

Write repository-aware, dependency-aware next steps. Draft ordering to refine:

1. (Claude) Bible expansion with Implementation-Aware Notes (Deliverable 2).
2. (Claude) Full Portable Context (Deliverable 3).
3. (Claude) Parallel Execution Protocol (Deliverable 4).
4. (Codex) Ironmark Blood Production cost loop in `simulation.js`, tying combat training/upkeep to a population-cost modifier.
5. (Codex) Pathing pass replacing `moveToward`-only steering with waypoint or grid-based avoidance for 72×48 maps at 150–250 unit scale.
6. (Codex) Commander battlefield unit tied to `dynasty.members[].id`, with aura, kill/capture, name display.
7. (Codex) Faith Doctrine Path effects: at minimum, +combat-aura while within the sacred site of the committed faith, and a doctrine-gated operation stub.
8. (Codex) Conviction ledger accumulator with the four canonical buckets (Ruthlessness, Stewardship, Oathkeeping, Desecration) with at least one player action wired into each bucket.
9. (Codex) Second playable house via minimal house-select or URL-parameter selection (`?house=stonehelm`).
10. (Claude) Document reconciliation for `POLITICAL_EVENTS.md` fragmentation (two copies in 11_MATCHFLOW and 08_MECHANICS).
11. (Codex) Occupation-vs-stabilized-control distinction on territory: occupied points degrade owner loyalty before flipping.
12. (Codex) Dual-Clock Declaration scaffolding: at least a post-battle Declaration screen announcing elapsed in-world time.

### Deliverable 6 (work still in flight from operator handoff): Continue Advancing Full Bloodlines

Per the operator's direction, the intended focus for near-term engineering is dynasty/faith runtime state bridging. This did not land in the desktop session. Priority tasks deferred to web:

- Attach `dynasty.members` by role to runtime entities (Commanders to armies, Governors to control points, Envoys to diplomacy state).
- Surface selected-faith effects on the battlefield (aura, unit availability gate, operation unlock).
- Extend Dynasty panel with clickable members that focus the camera on their runtime attachment.
- Extend Faith panel to display active Doctrine Path effects and sacred site linkage.
- Message log should fire events for: member death, faith commitment, doctrine change, sacred-site discovery, occupation vs seize, and all existing events.

## Files Changed This Commit

Portfolio (lancewfisher-v2):
- Bloodlines prototype root mirror (data/, docs/, src/, tests/, play.html, README.md added/updated)
- `deploy/bloodlines/docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` (new, inside deploy which is gitignored — tracked copy is in `bloodlines/docs/`)
- `bloodlines/docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` (new, root mirror, tracked)
- `bloodlines/docs/WEB_SESSION_HANDOFF_2026-04-13.md` (new, this file's root mirror)
- Prior uncommitted portfolio work (home, product pages, styles, main.js) from Session 4-6 carryover — see per-file diff for specifics.

Explicitly NOT committed (ephemeral/scratch):
- `HANDOFF.md`, `HANDOFF_SESSION_A.md`, `HANDOFF_SESSION_B.md`
- `CONSOLIDATION_PROMPT.md`, `CODEX_NEXT_SESSION.md`, `NEXT_SESSION_PROMPT.md`
- `SESSION_STATUS.md`, `SOVEREIGN_PRODUCTS_INTEGRATION.md`
- `_brand_patch.py`, `_brand_strip_fix.py`, `brand-sampler.html`, `brand-sampler-v4.html`
- `bloodlines/artifacts-autostart.png`, `bloodlines/artifacts-launch.png`

## Verify On Web

After checkout on web:
```bash
cd deploy/bloodlines && node tests/data-validation.mjs
# -> "Bloodlines data validation passed."

python -m http.server 8078 --directory deploy/bloodlines
# Open http://localhost:8078/play.html
# Expected: skirmish auto-starts, population grows, Stonehelm contests Stonefield Watch, tribes raid, no console errors
```

## Do Not Regress

- `deploy/bloodlines/` is authoritative. Never `cp root -> deploy`.
- Additive archival rules stand for all canon material.
- No scope reduction. Treat absent systems as implementation debt, not design downgrades.
- The 2026-04-07 canon reset (CB004 voided for 8 houses, Dynastic Prestige demoted) stands.
- Ironmark is the only `prototypePlayable: true` house until a new house is explicitly settled.
- Data validation must pass before committing any data JSON edit.
- All JS changes must pass `node --check` on the affected module.
