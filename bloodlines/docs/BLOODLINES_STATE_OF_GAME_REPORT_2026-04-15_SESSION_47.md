# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 47
Author: Codex

## Scope

Minor-house territorial foothold LIVE. Extends Sessions 44 and 45: a defected cadet branch no longer exists only as a world-register entry plus militia founder. It now claims a real stabilized border march on the map, receives resource trickle from that march, renders on the main map and minimap, and survives save/restore as a dynamic control point.

## Changes Landed

### Territorial-claim spawn (`src/game/core/simulation.js`)

- Added `getNearestControlPointContinentId`, `findDefectedMinorClaimPosition`, and `spawnDefectedMinorTerritoryClaim`.
- `spawnDefectedMinorFaction` now claims a march after the founder-unit spawn resolves.
- The new control point:
  - uses deterministic id `${minor.id}-claim`,
  - uses the lesser-house name as its march identity,
  - spawns near the parent seat but not on top of an existing control point,
  - starts as `border_settlement`,
  - starts owned by the minor faction,
  - starts `stabilized`,
  - starts with live food and influence trickle,
  - records `lesserHouse.defectedTerritoryId` for lineage reconstruction.

### Save/resume continuity (`src/game/core/simulation.js`)

- `exportStateSnapshot` now preserves the spatial and definition fields needed for dynamic control points:
  - `name`, `x`, `y`, `radiusTiles`, `captureTime`, `resourceTrickle`, `settlementClass`, `continentId`, `contested`.
- `restoreStateSnapshot` now reconstructs missing control points from snapshot data before restoring state fields.
- This closes the continuity gap that would otherwise erase a dynamically spawned breakaway march on reload.

### Legibility (`src/game/main.js`)

- The dynasty panel's "Rival minor houses" rows now show how many marches each breakaway house currently holds.
- No decorative panel was added. The legibility change rides on the already-live dynasty panel and the already-live control-point renderer.

### Robustness

- `createEntityId` now initializes unknown prefixes instead of producing `NaN` sequences. This directly hardens lesser-house id generation and any future additive entity prefix introduced by continuation work.

### Test coverage

- `tests/runtime-bridge.mjs` Session 47 block now verifies:
  - the control-point claim exists,
  - the claim belongs to the spawned minor faction,
  - the claim is a stabilized border settlement,
  - the lesser-house record points back to the claim,
  - save/restore preserves the dynamic territorial foothold.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.

## Canonical Interdependency Check

Session 47 connects:

1. Lesser-house defection, the territorial claim is spawned directly from the defection event.
2. World control layer, the minor becomes a real owner in `state.world.controlPoints`.
3. Resource economy, the new march produces food and influence trickle.
4. Renderer and minimap, the new march is visible through the already-live control-point layer.
5. Save/resume, the dynamic march now survives snapshot round-trip.
6. Hostility and combat, the claim belongs to a hostile faction and can be contested or retaken.
7. Dynasty legibility, the player can see the breakaway house and its territorial footprint in the dynasty panel.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Minor-house territory | DOCUMENTED | LIVE (first layer: spawned stabilized border march, resource trickle, save/restore persistence, dynasty-panel legibility) |

## Session 48 Next Action

Minor-house AI activation and territorial defense. The breakaway branch now has faction identity, a founder militia, and a march. It still lacks autonomous behavior, local retinue growth, and a territorial-defense routine, which leaves the new world actor structurally live but operationally passive.

## Preservation

No canon reduced. No prior defection layer regressed. The territorial foothold is the first real world-state expression of breakaway politics and is explicitly a first layer, not a replacement for the later AI, diplomacy, retinue, or economy layers.
