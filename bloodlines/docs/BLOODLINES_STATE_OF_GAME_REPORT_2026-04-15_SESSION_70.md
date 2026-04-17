# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 70
Author: Codex

## Scope

Session 69 made Ironmark's `axeman` honest for the player. Session 70 carries that same house lane into AI awareness so the unit stops being a player-only privilege. Ironmark-controlled AI now recruits Axemen through the same simulation-side barracks gate, backs off into lighter infantry when blood-production burden is already high, and surfaces that choice through the live message log.

## Changes Landed

### House-aware barracks selection (`src/game/core/ai.js`)

- Imported `getTrainableUnitIdsForBuilding(...)` so AI now reads the same runtime trainable roster the player command surface and production queue already use.
- Added `chooseBarracksUnit(...)` as a house-aware barracks chooser.
- Ironmark-controlled AI now prefers `axeman` when:
  - the siege line needs escort mass,
  - `axeman` is actually available through the house gate,
  - blood-production load is still below the slowdown threshold.
- When Ironmark blood-production burden is already high, AI now falls back to `swordsman` instead of blindly deepening the heavier Axeman levy.

### Legibility (`src/game/core/ai.js`)

- Added message-log surfacing for the new AI branch:
  - Axeman recruitment is announced as a blood-fueled muster.
  - blood-strain fallback is announced as a deliberate decision to rein in Axemen levies and use Swordsmen instead.
- This keeps the new AI behavior legible through an already-live runtime surface instead of inventing a dead new panel.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 70 coverage proving:
  - Ironmark AI queues `axeman` through the shared house gate when blood load is stable,
  - that queue immediately applies the heavier living-population levy and blood-production load,
  - the message log exposes the Axeman muster,
  - restore preserves the queued AI Axeman,
  - Ironmark AI falls back to `swordsman` when blood-production load is already high,
  - that fallback is also legible in the message log,
  - Stonehelm AI remains locked out of `axeman`.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.
- `python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines` served `play.html` successfully.
- Static browser-lane verification confirmed:
  - `play.html` returned `200`,
  - served markup still contains `game-shell`, `resource-bar`, `realm-condition-bar`, `dynasty-panel`, `faith-panel`, and `message-log`.

## Canonical Interdependency Check

Session 70 connects:

1. House identity and AI production, because Ironmark-controlled AI now recruits its live unique unit through the same shared barracks gate used by the player.
2. House identity and civilizational pressure, because AI Axeman recruitment now respects live blood-production burden and falls back once Ironmark has crossed into growth-slowing load.
3. House identity and runtime legibility, because the message log now reports both Axeman muster and burden-driven fallback.
4. House identity and restore continuity, because the queued AI Axeman survives snapshot round-trip.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Ironmark `axeman` AI awareness | PARTIAL | LIVE (first layer, through shared house-gated AI recruitment, blood-load-aware fallback, message-log legibility, and restore continuity) |

## Session 71 Next Action

Deepen world pressure through source-aware targeting and legibility. The next canonical layer is to expose why a realm is under pressure, then make that source matter operationally by steering tribal world reaction toward off-home holdings when continental overextension is the leading pressure source.
