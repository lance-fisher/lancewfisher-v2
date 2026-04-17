# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 14
Author: Claude
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope

Session 14 extended longer-siege AI adaptation with two post-assault branches and surfaced their consequence in player-visible legibility.

## Changes Landed

### Post-repulse AI adjustment (Vector 3)

- `src/game/core/ai.js` — new `else if` branch in the keep-assault commit path: when `enemy.cohesionPenaltyUntil > state.meta.elapsed` (the wave-spam denial penalty is active), Stonehelm retreats siege forces to the stage point rather than blindly recommitting. Stores `enemy.ai.postRepulseUntil` for downstream consumption. Canonical message: "Stonehelm reels from the repulsed assault, pulls back to the siege stage, and reorganizes the line."
- New canonical rule: failed assaults cost the attacker tempo (master doctrine section X). Wave-spam denial math (Session 2) no longer only denies damage; it now denies initiative.

### Supply-collapse AI adjustment (Vector 3)

- Second new `else if` branch: when `suppliedSiegeReady` was true but `suppliedSiegeArmy.length === 0` (player destroyed or isolated the supply chain mid-assault), AI pulls back rather than pushing hollow engines. Canonical message: "Stonehelm's siege supply has collapsed mid-approach. Engines fall back to the stage to re-link."

### Hostile post-repulse legibility (Vector 6)

- `src/game/core/simulation.js` — `getRealmConditionSnapshot` now surfaces:
  - `worldPressure.hostilePostRepulseActive` (boolean)
  - `worldPressure.hostilePostRepulseRemaining` (seconds, rounded)
  - `worldPressure.hostileCohesionStrain` (max strain across hostile kingdoms)
- `src/game/main.js` — World pill now displays "Rival repulsed {Xs}" (green band) when hostile post-repulse is active, showing the player exactly when their defensive success is costing the attacker tempo. Meta line includes rival strain figure when nonzero.

### Test coverage

- `tests/runtime-bridge.mjs` — snapshot exposes hostilePostRepulse fields; setting enemy `cohesionPenaltyUntil` to a future timestamp surfaces as active in the snapshot with positive remaining time and strain figure.

## Verification

- `node tests/data-validation.mjs` — passed
- `node tests/runtime-bridge.mjs` — passed
- All simulation modules `node --check` pass

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Longer-siege AI post-repulse adjustment | DOCUMENTED | LIVE |
| Longer-siege AI mid-siege supply-collapse response | DOCUMENTED | LIVE |
| Hostile post-repulse legibility (attacker-state surfacing to defender) | DOCUMENTED | LIVE |

## Remaining DOCUMENTED Items In Longer-Siege AI

- Repeated-assault window logic (after post-repulse cooldown expires, AI should actually re-attempt, not re-commit blindly nor stall forever).
- Supply-protection patrols (AI assigns escort units to supply chain).
- Weather / night mid-siege tactics.

## Session 15 Next Action

Highest leverage: **Repeated-assault window logic** — after post-repulse cooldown expires, Stonehelm should explicitly re-queue the assault with slight repositioning. Or: **Faith Hall (L2)** as second-tier covenant building. Or: **Ironmark Blood Production loop depth**.

## Preservation Statement

No canon reduced. Session 14 moved 3 items from DOCUMENTED to LIVE. Tests green.
