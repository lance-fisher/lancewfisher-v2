# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 13
Author: Claude
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope

Session 13 opened Vector 4 (Faith and Conviction depth) with its first canonical layer: all four covenants are now `prototypeEnabled: true`, and the canonical Wayshrine building is live in data, simulation, renderer, build palette, and tests. Wayshrines amplify faith exposure accrual at nearby sacred sites and boost faith intensity regen for the committed covenant once selected, canonically strengthening ward profiles downstream.

Author: Claude (third session in the Claude/Codex alternation chain; follows Sessions 11 and 12 by Claude, preparing handoff for Codex's overnight run).

## Top-Line Verdict

Faith has moved from lore-ornament to dynasty-controllable amplifier. A player can now actively invest in faith: build a Wayshrine near a discovered sacred site, increase faith exposure accrual there 1.8x per shrine (capped at 3.0x cumulative), commit to a covenant sooner, reach ward-strengthening intensity faster. This satisfies the Canonical Interdependency Mandate: faith connects to building system, sacred site map data, covenant commitment flow, ward profiles (Session 5), intensity tier thresholds, and build-order player choice.

## Browser Reference Simulation — Session 13 Additions

### Faith prototype enablement (Vector 4)

- `data/faiths.json` — all four covenants flipped from `prototypeEnabled: false` to `prototypeEnabled: true`. Old Light, Blood Dominion, The Order, The Wild are now active in prototype runtime per master doctrine section XIX. Light/Dark doctrine effects, ward profiles, and intensity growth continue to function as Sessions 5 through 10 established.

### Wayshrine canonical shrine building

- `data/buildings.json` — new `wayshrine` building:
  - `faithRole: "shrine"`, `faithTier: 1` (first-tier per master doctrine section XIX)
  - `footprint 2x2`, `health 420`, `buildTime 14`, `armor 4`
  - `faithExposureAmplification: 1.8` — multiplier applied to faith exposure accrual at nearby sacred sites
  - `faithExposureRadius: 180` — distance within which the shrine amplifies sacred-site exposure
  - `faithIntensityRegenBonus: 0.18` — canonical regen multiplier when covenant is committed
  - `cost: gold 60, wood 40, stone 50` — accessible early-to-mid game
  - Canonical notes reference L2 Hall, L3 Grand Sanctuary, L4 Apex structures as explicitly DOCUMENTED follow-up layers (no placeholder completion).

### Simulation integration

- `src/game/core/simulation.js`:
  - `updateFaithExposure` extended: for each faction present at a sacred site, computes a Wayshrine exposure multiplier via new helper `getWayshrineExposureMultiplierAt(state, factionId, sacredSitePosition)`.
  - Multiplier caps at 3.0 (3 shrines meaningfully contribute; canonical diminishing returns beyond).
  - Exposure accrual scales with the multiplier.
  - Faith intensity regen for the committed covenant also scales: +`(multiplier - 1) * 0.18` on top of the canonical 0.25/s base, so Wayshrines strengthen ward profiles on nearby fortifications per master doctrine.

### UI integration

- `src/game/main.js` — `wayshrine` added to `WORKER_BUILD_ORDER` so workers can build it alongside dwellings, farms, wells, and fortifications.
- `src/game/core/renderer.js` — Wayshrine silhouette rendering:
  - Dark stone base with accent-colored outline.
  - Central pillar.
  - Accent-colored flame/sigil at the top.
  - Subtle faint aura ring indicating the exposure-amplification radius.
  - Name label positioned below the building like other canonical structures.

### AI integration

- AI sabotage reciprocity now has one more valid target (Wayshrines are player-owned buildings; fire_raising is canonically valid against them). No AI code changes needed; the existing `pickAiSabotageTarget` fallback to `fire_raising` against any player building covers it naturally.

### Test coverage

- `tests/data-validation.mjs` — asserts all four faiths are `prototypeEnabled: true`, Wayshrine exists with `faithRole: "shrine"`, `faithExposureAmplification > 1`, `faithExposureRadius > 0`.
- `tests/runtime-bridge.mjs` — baseline vs amplified test: same player worker at same sacred site, one simulation with no shrine, another with a Wayshrine placed near the site. After 3 seconds of simulation, the amplified exposure is at least 20% higher than baseline (actually ~80% higher in practice due to the 1.8x amp). Confirms the amplification path is live.

## Verification

```
node tests/data-validation.mjs       → Bloodlines data validation passed.
node tests/runtime-bridge.mjs        → Bloodlines runtime bridge validation passed.
node --check src/game/main.js                   → OK
node --check src/game/core/simulation.js        → OK
node --check src/game/core/renderer.js          → OK
node --check src/game/core/ai.js                → OK
node --check src/game/core/data-loader.js       → OK
node --check src/game/core/utils.js             → OK
```

## Gap Analysis Reclassification (Session 13)

| System | Previous | Current |
|---|---|---|
| Faith prototype enablement (covenant flags) | DOCUMENTED (all 4 faiths prototypeEnabled false) | LIVE (all 4 prototypeEnabled true) |
| Faith building progression (first tier) | DOCUMENTED | PARTIAL (Wayshrine L1 LIVE; L2 Hall / L3 Grand Sanctuary / L4 Apex still DOCUMENTED) |
| Faith amplification by player investment | DOCUMENTED | LIVE (Wayshrines amplify exposure + intensity regen per canonical formula) |

## Top Remaining Structural Deficits After Session 13

1. **Faith L2-L4 buildings** (Hall, Grand Sanctuary, Apex) still DOCUMENTED.
2. **L3 faith-specific unit rosters** (8 units, 2 per covenant per doctrine path) still DOCUMENTED.
3. **L4 faith units + L5 apex units** still DOCUMENTED.
4. **Conviction milestone powers per band** still DOCUMENTED.
5. **Dark-extremes world pressure trigger** still DOCUMENTED.
6. **Longer-siege AI subsequent layers**: repeated-assault windows, post-repulse adjustment, supply protection patrols still PARTIAL.
7. **Continental / naval foundation** still DOCUMENTED.
8. **Dual-clock declaration seam** still DOCUMENTED.
9. **Ironmark Blood Production loop depth** still PARTIAL.

## Session 14 Next Action (for Claude or Codex, whichever runs next)

Priority by leverage:

1. **Longer-siege AI next layer: post-repulse adjustment** — when Stonehelm's formal assault is repulsed (cohesion strain threshold hit or supplied-engine count drops below 1), retreat to stage point, recompose, re-attempt rather than continuing blindly.
2. **Faith Hall (L2)** — second-tier covenant building, built on top of a committed-covenant Wayshrine. Unlocks L3 faith unit recruitment seat.
3. **Conviction milestone powers per band** — Apex Moral, Moral, Cruel, Apex Cruel each unlock distinct abilities per canonical spec in `04_SYSTEMS/CONVICTION_SYSTEM.md`.
4. **Save-state serialization primer** — begin formal structured state export so long matches survive the growing complexity.
5. **Dual-clock declaration seam minimal** — post-battle in-world time declaration.

## Alternation Chain Note

- Session 13 is Claude authored. Claude has now run Sessions 11, 12, 13 consecutively in this REPL.
- User indicated Codex should pick up when Claude's usage exhausts.
- Scheduled Claude resume at 1:00 AM local tonight via `bloodlines-claude-resume-after-codex` task.
- Codex's handoff will write `Author: Codex` in its next state-of-game report header.

## Preservation Statement

No canonical system was reduced, substituted, or sidelined. Session 13 moved 2 items from DOCUMENTED to LIVE (faith prototype enablement + Wayshrine canonical shrine building) and 1 from DOCUMENTED to PARTIAL (faith building progression; 3 of 4 tiers still DOCUMENTED with explicit canonical follow-up logged). All tests green.
