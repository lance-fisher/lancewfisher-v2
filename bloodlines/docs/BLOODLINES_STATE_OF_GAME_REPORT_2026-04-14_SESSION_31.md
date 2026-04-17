# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 31
Author: Claude

## Scope

Fire Ship one-use sacrifice combat mechanic LIVE. Master doctrine section XV canonical sacrifice vessel now operational: Fire Ship detonates on its first strike, the target takes full damage, the vessel is destroyed.

## Changes Landed

- `src/game/core/simulation.js` — `updateCombatUnit` attack path: after damage is applied (melee or projectile-launched), if the attacker's unit def has `oneUseSacrifice: true`, the attacker's health is set to zero. The normal dead-unit filter in `stepSimulation` removes the vessel on the next tick. Canonical message fires on detonation.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed. Test: Fire Ship attacks enemy War Galley, after 1.5s Fire Ship is destroyed (removed from state.units), Galley takes meaningful damage.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Fire Ship one-use sacrifice combat mechanic | DATA-ONLY (Session 28 flag) | LIVE |

## Session 32 Next Action

- Continental secondary landmass (canonical section XIV): add a secondary continent tile block with one control point on it, requiring transport crossing.
- Or: include dualClock + naval fields in save/resume snapshot.
- Or: marriage + succession interface panel (Vector 2 long-standing DOCUMENTED item).

## Preservation

No canon reduced. 1 item moved DATA-ONLY → LIVE. Tests green.
