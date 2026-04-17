# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 26
Author: Claude

## Scope

Dual-clock declaration seam first canonical layer LIVE. Master doctrine section XIII (Determined Time System) timing foundation now operational. Every stepSimulation tick advances `state.dualClock.inWorldDays` by 2 days per real second (canonical default); canonical events declare additional in-world time jumps via `declareInWorldTime(state, days, reason)`.

## Changes Landed

- `src/game/core/simulation.js`:
  - New `ensureDualClockState(state)` bootstraps `state.dualClock = {inWorldDays, daysPerRealSecond, declarations}`.
  - New `tickDualClock(state, dt)` advances in-world days each tick.
  - New exported `declareInWorldTime(state, daysDelta, reason)` adds bonus declaration (tracked in rolling log bounded to last 32).
  - `stepSimulation` calls `tickDualClock` immediately after `state.meta.elapsed += dt`.
  - Territorial capture fires `declareInWorldTime(state, 14, "Captured ${name}")` — border takeovers canonically compress far more in-world time than the real-time capture action.
- `getRealmConditionSnapshot` now exposes `dualClock.{inWorldDays, inWorldYears, recentDeclarations}`.
- Canonical default: 2 in-world days per real second (720 days per 6 real minutes = ~2 years per medium skirmish, matching master doctrine's "1 to 2 years major battles" band).

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (fresh inWorldDays=0, post-tick inWorldDays>0, restore-from-snapshot still works even without dualClock in snapshot yet).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Dual-clock declaration seam (first canonical layer) | DOCUMENTED | LIVE (baseline tick advance + territorial declaration + snapshot legibility) |

## Remaining DOCUMENTED Dual-Clock Layers

- Explicit Declaration Phase between battles (post-battle UI summary showing in-world time jump).
- Events Queue during commitment windows.
- Full Phase Entry at stage boundaries of the canonical 5-stage match structure.
- Dual-clock state in save/resume snapshot (explicit round-trip).

## Session 27 Next Action

- Naval foundation (first canonical layer: water tiles + harbor + 1 vessel class).
- Or: Declaration Phase UI after battles.
- Or: Include dualClock in save/resume snapshot.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green.
