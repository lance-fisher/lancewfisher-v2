# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 63
Author: Codex

## Scope

The house layer has been advanced from data-presence into honest runtime access. Hartvale is now prototype-playable through the existing house-select seam, the Verdant Warden is now live behind real house-gated training rather than dead JSON, the command panel only surfaces units the current house can actually train, off-house factions are blocked from queuing Hartvale's unique unit, and the new gate is covered by validation and runtime tests.

## Changes Landed

### House-gated training runtime (`src/game/core/simulation.js`)

- Added `isUnitPrototypeTrainableForFaction(content, faction, unitDef)` so prototype trainability now reads both:
  - unit prototype enablement,
  - house ownership for house-specific units.
- Added `getTrainableUnitIdsForBuilding(state, buildingId)` so UI and other callers can ask the simulation for the real trainable roster instead of trusting raw building JSON.
- Extended `queueProduction` to reject off-house or prototype-disabled units even if a building carries the unit id in its trainable roster.
- Off-house failure now names the required house directly, which keeps the gate legible instead of silently failing.

### House data and production-seat enablement (`data/houses.json`, `data/units.json`, `data/buildings.json`)

- Hartvale now sets `prototypePlayable: true`.
- Verdant Warden now sets `prototypeEnabled: true`.
- Barracks now includes `verdant_warden` in the trainable roster.
- This preserves one shared barracks roster while moving actual access control into simulation-side house gating, which is the correct seam for future unique-unit expansion.

### Command-surface legibility (`src/game/main.js`)

- The command panel no longer iterates raw `buildingDef.trainableUnits`.
- It now calls `getTrainableUnitIdsForBuilding(state, building.id)`, which means:
  - Hartvale sees Verdant Warden in the barracks training surface,
  - Ironmark and Stonehelm do not see the Hartvale unit,
  - the UI cannot imply availability that runtime will later deny.

### Validation and runtime coverage (`tests/data-validation.mjs`, `tests/runtime-bridge.mjs`)

- Added validation assertions proving:
  - Hartvale is prototype-playable,
  - Verdant Warden is prototype-enabled,
  - Barracks exposes Verdant Warden in the shared roster.
- Added runtime coverage proving:
  - Hartvale barracks surfaces Verdant Warden,
  - Hartvale can queue Verdant Warden,
  - Ironmark cannot surface or queue Verdant Warden,
  - Stonehelm cannot surface Verdant Warden,
  - off-house queue failure names Hartvale explicitly.

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
  - served markup still contains `game-shell`, `resource-bar`, `realm-condition-bar`, `dynasty-panel`, `faith-panel`, and `message-log`,
  - served `data/houses.json` now reports Hartvale as `prototypePlayable: true`.

## Canonical Interdependency Check

Session 63 connects:

1. House identity and military production, because unique-unit access now keys off the active ruling house inside real production logic.
2. House identity and command-surface legibility, because the barracks panel now exposes only the units the current house can truly field.
3. House data and runtime enforcement, because shared barracks schema no longer bypasses canonical house ownership.
4. House expansion and validation continuity, because Hartvale playability and unique-unit access are now locked into the green verification suite.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Hartvale playable-house enablement | PARTIAL | LIVE (first layer, through house-select continuity, real runtime training access, command-surface legibility, and verification coverage) |
| House-gated unique-unit availability | DOCUMENTED | LIVE (first layer, simulation-side gate with command-panel filtering and off-house enforcement) |

## Session 64 Next Action

Return to the world-pressure lane and deepen internal-dynasty destabilization. Dominant realms already lose march loyalty and legitimacy under sustained world pressure. The next canonical layer is to make that overextension also strain lesser-house loyalty so late-stage dominance pushes internal cadet fracture, not only frontier erosion.
