# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 23
Author: Claude

## Scope

Supply-protection patrols now LIVE. Last DOCUMENTED longer-siege AI layer closed. When Stonehelm has a live siege chain (completed supply camp + live supply wagons), AI assigns up to 2 combat units per wagon as escort patrol to counter player sabotage and supply raiding.

## Changes Landed

- `src/game/core/ai.js` — new supply-protection patrol block at the end of `updateEnemyAi`:
  - Gate: `supplyCampCompleted && supplyWagons.length > 0 && army.length >= 5`
  - Capacity: up to 2 combat units per wagon, limited to `army.length - 3` (keeps assault force intact)
  - Candidate filter: no workers, no siege units, no already-attacking, no already-patrolling
  - Assignment: move to mean position of all wagons
  - State tracking: `enemy.ai.supplyPatrolUnitIds` persists across ticks; purged if unit dies or chain breaks
  - Canonical message on new assignments: "Stonehelm assigns N combat units to patrol the siege supply line."
  - Cleanup: when no live supply chain, `supplyPatrolUnitIds` cleared.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- Syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Supply-protection patrols (longer-siege AI) | DOCUMENTED | LIVE |

## Longer-Siege AI Status — Fully Canonical Layer Complete

All session 9 longer-siege AI items now LIVE:

- Relief-window awareness: LIVE (S12)
- Post-repulse retreat: LIVE (S14)
- Supply-collapse retreat: LIVE (S14)
- Repeated-assault window: LIVE (S16)
- Supply-protection patrols: LIVE (S23)

Only weather/night mid-siege tactics (which are a deeper canon surface, not in the core session-9 list) remain DOCUMENTED.

## Session 24 Next Action

- Naval foundation first canonical layer (water tiles + harbor + 1 vessel class).
- Or: Save-state serialization primer.
- Or: Dual-clock declaration seam.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green.
