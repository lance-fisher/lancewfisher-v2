# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 32
Author: Claude

## Scope

Continental architecture LIVE. Master doctrine section XIV (continental separation) now has a true canonical secondary continent on the map: water-isolated southern landmass with its own canonical control point Cliffsong Outpost. All Session 27-31 naval foundation work now has a destination: cross-water campaigns are mechanically required and rewarded.

## Changes Landed

### Map (Vector 5)

- `data/maps/ironmark-frontier.json`:
  - Three new water terrain patches forming a continental divide that isolates the south continent (38,40-53,41 north band; 38,42-39,47 west; 52,42-53,47 east; map edge at y=48 closes the south).
  - New control point `cliffsong_outpost` at (45.5, 45.0): canonical secondary-continent border settlement, 12-second capture time, gold + food + influence trickle (canonical: secondary continents canonically yield trade resources).
  - Existing `stonefield_watch` tagged `continentId: "home"` for canonical reciprocity.

### Simulation (Vector 5 + Vector 6)

- `src/game/core/simulation.js`:
  - Territorial capture branch now fires extra `declareInWorldTime(state, 28, ...)` for non-home continent captures (canonical: cross-water campaign compresses far more in-world time).
  - Canonical message on cross-continental capture: "X establishes a foothold on the {continentId} continent at {name}."
  - `getRealmConditionSnapshot.worldPressure` now exposes `continentalHoldings` (per-continent count) and `offHomeContinentHoldings` (total non-home).

### UI (Vector 6)

- `src/game/main.js` — World pill meta now shows `off-home holdings N` when player has any non-home continental control points.

### Test coverage

- `tests/data-validation.mjs` — Cliffsong Outpost existence + south continentId + at least 3 continent-divide water patches.
- `tests/runtime-bridge.mjs` — snapshot exposes continentalHoldings + offHomeContinentHoldings; capturing south CP surfaces holding count.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Continental architecture (water separation + secondary continent) | PARTIAL (water foundation only) | LIVE (true secondary continent + control point + capture-time canon + legibility) |
| Cross-water campaign mechanic | DOCUMENTED | LIVE (transport + secondary continent + canonical declaration on capture) |

## Session 33 Next Action

- Marriage + succession interface panel (Vector 2 long-standing DOCUMENTED item, hasn't been advanced since Session 14).
- Or: Lesser houses promotion pipeline.
- Or: AI awareness of cross-continent expansion.

## Preservation

No canon reduced. 2 items moved (one from PARTIAL → LIVE, one from DOCUMENTED → LIVE). Tests green.
