# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-16
Session: 96
Author: Claude

## Scope

Session 96 made the naval world live in the browser simulation. Vessels now move on water, fishing boats auto-gather food, naval combat resolves between hostile vessels, fire ships detonate on first strike through the oneUseSacrifice mechanic, and vessels spawn on water tiles adjacent to their producing harbor. This activates the full naval data foundation (6 vessel types, 2 harbor tiers) that has been data-defined since Sessions 27-28 but never had a runtime tick path.

## Changes Landed

### Vessel spawn at water tiles (`src/game/core/simulation.js`)

- Added `findNearestWaterSpawnPosition(state, building, buildingDef)` that scans the 1-tile ring around a building footprint for the nearest water tile.
- `spawnUnitAtBuilding` now checks `unitDef.role === "vessel"` and places the new vessel on the nearest water tile instead of the standard land offset. Falls back to the land position if no water is adjacent (which shouldn't happen for correctly placed harbors).

### Vessel update tick (`src/game/core/simulation.js`)

- Added `updateVessel(state, unit, unitDef, dt)` as a dedicated vessel tick alongside the existing `updateWorker`, `updateSupportUnit`, and `updateCombatUnit` dispatches.
- Vessel dispatch: `updateUnits` now routes `role === "vessel"` units to `updateVessel` before the support/combat fallthrough.

### Fishing boat auto-gather

- Fishing boats (`vesselClass: "fishing"`) now auto-gather food when idle on a water tile.
- Yield rate: `unitDef.gatherRate` (default 0.8) per second.
- No gather-deposit cycle needed: fishing boats produce directly into the faction's food stockpile, matching the canonical design where fishing is a passive water economy.

### Naval combat

- Vessels with `attack > 0` (war galley, fire ship, capital ship) can receive and execute attack commands against hostile units.
- Attack resolution: move toward target until within `attackRange`, then apply `attackDamage` per `attackCooldown` cycle.
- Auto-aggression: idle combat vessels engage hostile units within 1.2x attack range.
- Fire Ship sacrifice: vessels with `oneUseSacrifice: true` deal their full `attackDamage` on first strike and then self-destruct (health set to 0).

### Movement domain enforcement

- The existing Session 27 movement domain gate in `issueMoveCommand` already blocks vessels from land and land units from water. Session 96 activates this by ensuring vessels are actually created on water and have a tick path that processes their movement commands.

### Runtime bridge test coverage (`tests/runtime-bridge.mjs`)

- 7 new assertions covering:
  - Fishing boat on water generates food when idle.
  - Vessel move command accepted on water tile.
  - Vessel move command rejected for land tile.
  - Naval combat: war galley damages hostile vessel.
  - Save/restore preserves vessel units.
  - Restored fishing boat retains correct type.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed (including 7 new naval assertions).
- All syntax checks pass.

## Canonical Interdependency Check

The naval world integration activates:

1. **Economy**: Fishing boats create a water-based food income, giving coastal territory economic value beyond land resources.
2. **Transport**: The existing Session 30 embark/disembark system now has vessels that can be produced from harbors and move on water.
3. **Combat**: War galleys, fire ships, and capital ships can now engage in naval warfare, controlling sea lanes.
4. **Harbor placement**: The Session 27 coastal-adjacency gate ensures harbors are correctly positioned for vessel spawning.
5. **Match progression**: Naval capability adds a new expansion vector for Stage 2+ matches with coastal territory.

## Gap Analysis

- Naval vessel tick path: moved from DATA-ONLY to LIVE.
- Fishing boat economy: moved from DATA-ONLY to LIVE.
- Naval combat: moved from DATA-ONLY to LIVE.
- Fire ship sacrifice: moved from DOCUMENTED to LIVE.
- Remaining naval gaps: AI harbor construction and naval production, naval fog-of-war, naval-specific world-pressure contribution, trade-route naval interdiction.

## Session 97 Next Action

1. If continuing naval depth: add AI harbor construction and naval unit production.
2. If broadening: add trade-route naval interdiction or naval fog-of-war.
3. If pivoting: open the Unity Play Mode verification shell.
