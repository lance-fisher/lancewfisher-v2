# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 15
Author: Claude

## Scope

Ironmark Blood Production deepening — Session 4's partial implementation (per-unit pop + ruthlessness event) extended with a canonical sustained-war load tracker that depresses growth tempo and triggers drift events under prolonged attritional levy. Connects Ironmark identity to three live systems: population (tempo), conviction (drift), and realm snapshot (legibility).

## Changes Landed

### Blood Production load tracking (Vector 1 + Vector 2)

- `src/game/core/simulation.js` — `queueProduction` blood-levy branch now increments `faction.bloodProductionLoad` by 1.5 per levy. If load crosses 12 AND at least 40 simulation seconds since the last drift event, an additional +2 ruthlessness event fires ("Sustained Ironmark blood levy under attritional war"). Canonical master doctrine sections IV and VIII.

### Growth tempo depression (Vector 1)

- `tickPopulationGrowth` now computes `bloodGrowthPenalty` for Ironmark factions: 1x below load 8, scaling linearly to 1.8x at load 14+. Growth interval stretches accordingly, so sustained levy canonically slows population regeneration.

### Realm cycle decay (Vector 1)

- `tickRealmConditionCycle` decays `bloodProductionLoad` by 2.5 per 90-second realm cycle, canonically representing dynasty recovery when training slows.

### Snapshot + HUD legibility (Vector 6)

- Snapshot `population` block now exposes `bloodProductionLoad` and `bloodProductionActive` (true only when Ironmark AND load > 8).
- Population HUD pill now shows "blood levy load X" in meta when nonzero, "growth slowed" when active, and bands yellow when active.

### Test coverage

- `tests/runtime-bridge.mjs` — load starts at 0; setting load to 10 on Ironmark surfaces bloodProductionActive true; load at 4 surfaces false; non-Ironmark factions never surface active regardless of load.

## Verification

- All syntax checks pass.
- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Ironmark Blood Production loop depth | PARTIAL (per-unit pop + flat event) | LIVE (sustained-war load + growth tempo depression + drift event + legibility) |

## Session 16 Next Action

- Repeated-assault window logic (Stonehelm re-attempts after post-repulse cooldown with repositioning) or Faith Hall L2.

## Preservation

No canon reduced. 1 item moved PARTIAL → LIVE. Tests green.
