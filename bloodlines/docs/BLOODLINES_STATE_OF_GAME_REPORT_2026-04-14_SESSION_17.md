# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 17
Author: Claude

## Scope

Covenant Hall (L2) canonical second-tier covenant building, built on top of a committed-covenant Wayshrine stack. Master doctrine section XIX L2 layer now LIVE.

## Changes Landed

### Covenant Hall L2 (Vector 4)

- `data/buildings.json` — new `covenant_hall` building:
  - `faithRole: "hall"`, `faithTier: 2`, `requiresFaithCommitment: true`
  - Footprint 3x3 (larger than Wayshrine 2x2), health 840, buildTime 22
  - `faithExposureAmplification: 2.4` (vs Wayshrine 1.8x), radius 240 (vs 180)
  - `wardSightBonusExtra: 6`, `wardDefenderAttackBonusExtra: 0.04` (ward strengthening hooks for future sessions)
  - Cost: gold 160, wood 100, stone 140, iron 30
- `src/game/core/simulation.js` — `attemptPlaceBuilding` now checks `buildingDef.requiresFaithCommitment` and rejects placement unless the faction has `faith.selectedFaithId`. Canonical gate per master doctrine section XIX (no covenant, no hall).
- `getWayshrineExposureMultiplierAt` extended to accept shrine + hall + sanctuary roles. Ceiling raised from 3.0x to 4.0x to accommodate Hall contribution; canonical diminishing returns retained.
- `src/game/main.js` — `covenant_hall` added to worker build palette after wayshrine.
- `src/game/core/renderer.js` — Hall silhouette: larger footprint, triple-pillar facade, apex sigil, extended 48-tile aura ring (vs Wayshrine 36).

### Test coverage

- `tests/data-validation.mjs` — asserts Covenant Hall existence, `faithTier: 2`, `requiresFaithCommitment: true`, and amplification exceeds Wayshrine's.

## Verification

- All syntax checks pass.
- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Faith building progression (L2) | DOCUMENTED | LIVE (L2 Hall joins L1 Wayshrine; L3 Grand Sanctuary, L4 Apex still DOCUMENTED) |

## Session 18 Next Action

- L3 faith unit rosters (8 units, 2 per covenant per doctrine path) — now that Hall is live and would canonically be the recruitment seat for L3 units.
- Or: Conviction milestone powers per band.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green.
