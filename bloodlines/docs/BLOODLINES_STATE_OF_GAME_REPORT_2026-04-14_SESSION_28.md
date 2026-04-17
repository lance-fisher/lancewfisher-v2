# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 28
Author: Claude

## Scope

Canonical 6-class vessel roster complete. Harbor L2 (Great Harbor) LIVE. Master doctrine section XV naval data layer now fully populated in data.

## Changes Landed

### Vessel roster complete (Vector 5 + Vector 3)

- `data/units.json` — 3 new vessel classes:
  - **Transport Ship** (stage 2, 280 HP, speed 52, armor 5, `transportCapacity: 6`) — will carry land units once the embark/disembark layer lands.
  - **Fire Ship** (stage 3, 180 HP, speed 76, atk 140, range 36, `oneUseSacrifice: true`) — one-use detonation vessel; sacrifice combat mechanic remains DOCUMENTED.
  - **Capital Ship** (stage 4, 820 HP, atk 46, range 170, armor 14, cost includes influence) — apex naval class per master doctrine section XV.

### Harbor L2 (Vector 5)

- `data/buildings.json` — new `harbor_tier_2`:
  - `navalTier: 2`, 4x4 footprint, health 1600, armor 10
  - Dropoff chain now includes gold (canonical naval trade pressure)
  - Trains Fire Ship + Capital Ship (L1 still trains the first 4)
  - Cost gold 260, wood 240, stone 170, iron 60

### UI

- Harbor L2 added to worker build palette.
- `harbor_tier_1.trainableUnits` now also includes `transport_ship`.

### Test coverage

- `tests/data-validation.mjs` — full 6-vessel canonical roster existence + vesselClass/domain binding; Fire Ship `oneUseSacrifice` flag; Transport Ship capacity > 0; Harbor L2 exists with navalTier 2, trains fire_ship + capital_ship.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Canonical 6-class vessel roster | PARTIAL (3 of 6 LIVE) | LIVE (all 6 canonical vessel classes in data) |
| Harbor L2 tier | DOCUMENTED | LIVE |
| Fire Ship one-use sacrifice mechanic | DOCUMENTED | DATA-ONLY (flag present; runtime mechanic pending) |
| Transport embark/disembark mechanic | DOCUMENTED | DATA-ONLY (capacity present; runtime mechanic pending) |

## Session 29 Next Action

- Transport embark/disembark runtime (vessel picks up / drops off land units).
- Or: Fire Ship one-use detonation combat hook.
- Or: AI naval reactivity.
- Or: include dualClock + naval state in save/resume snapshot.

## Preservation

No canon reduced. 2 items moved DOCUMENTED → LIVE. 2 items moved DOCUMENTED → DATA-ONLY with explicit runtime follow-up layers logged. Tests green.
