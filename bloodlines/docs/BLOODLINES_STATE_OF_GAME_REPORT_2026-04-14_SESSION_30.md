# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 30
Author: Claude

## Scope

Transport embark/disembark runtime LIVE. Canonical continental-crossing mechanic now has its core runtime: Transport Ships carry up to 6 land units across water and disembark them on adjacent land. Master doctrine section XV requirement that naval transport materially enable cross-water campaigns now satisfied.

## Changes Landed

- `src/game/core/simulation.js`:
  - New exported `embarkUnitsOnTransport(state, transportUnitId, unitIds)`:
    - Requires transport with `transportCapacity > 0`.
    - Candidates must be same-faction, not already embarked, land-domain only (vessels do not board vessels), within 2.5 tiles of transport.
    - Limited by remaining capacity.
    - Sets `unit.embarkedInTransportId = transport.id` and pushes to transport's `embarkedUnitIds`.
    - Clears embarked unit's command.
    - Canonical message on success.
  - New exported `disembarkTransport(state, transportUnitId)`:
    - Scans 1-tile ring around transport for a non-water tile.
    - Places each disembarked unit at the land tile center with small offset.
    - Clears `embarkedInTransportId` and embarked list.
    - Canonical message on success.
  - `updateUnits` now skips embarked units (they are simulation-inactive: no gather, no attack, no move).

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed. Test covers: place transport on water, place militia + swordsman on adjacent land, embark both, move transport to NE bay edge, disembark, verify both ejected onto land tile with `embarkedInTransportId` cleared.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Transport embark/disembark mechanic | DATA-ONLY (Session 28 added capacity flag) | LIVE (embark + disembark runtime operational) |

## Session 31 Next Action

- Fire Ship sacrifice combat mechanic (one-use detonation destroys both target and vessel).
- Or: Continental crossing as explicit canonical test (seed AI with transport + land units on secondary continent).
- Or: Dual-clock in save/resume snapshot.

## Preservation

No canon reduced. 1 item moved DATA-ONLY → LIVE. Tests green.
