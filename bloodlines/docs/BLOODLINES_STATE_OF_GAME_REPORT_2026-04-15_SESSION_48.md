# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 48
Author: Codex

## Scope

Minor-house AI activation and territorial defense LIVE. The breakaway cadet branch now behaves as a real defensive actor: it detects hostile pressure near its claimed march, attacks nearby hostile combatants, regroups toward its claim after pressure clears, and exposes its current defensive posture in the dynasty panel.

## Changes Landed

### Minor-house AI (`src/game/core/ai.js`)

- Added `updateMinorHouseAi(state, dt)`.
- Minor houses now initialize their own AI state lazily and track:
  - `defenseTimer`,
  - `regroupTimer`,
  - `claimAlertUntil`,
  - `localDefenseStatus`.
- Added first operational behavior layer:
  - attack nearby hostile combat units threatening the claimed march,
  - retake the claim if it is seized,
  - regroup back to the march after pressure clears or the founder strays too far.
- Added message-log signals for:
  - defensive alert,
  - regroup,
  - retake march.

### Runtime wiring (`src/game/main.js`)

- The browser loop now calls `updateMinorHouseAi(state, dt)` alongside enemy and neutral AI.
- The dynasty panel now surfaces:
  - rival minor-house retinue count,
  - current posture, for example `on alert`, `holding march`, or `retaking march`.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 48 coverage proving:
  - nearby hostile pressure near the claim causes the founder retinue to attack,
  - pressure clearance causes the retinue to regroup toward the claimed march,
  - minor-house AI state survives snapshot round-trip.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.

## Canonical Interdependency Check

Session 48 connects:

1. Dynastic defection, the minor-house AI exists only because the lesser-house defection arc already spawned a faction, founder, and claim.
2. Territorial control, the AI defends and regroups around a real claimed march.
3. Military behavior, the founder unit now receives autonomous attack and movement orders.
4. Legibility, the dynasty panel and message log expose the operational state.
5. Save/resume, the AI state now survives snapshot round-trip.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Minor-house operational AI | DOCUMENTED | LIVE (first layer: defend claim, retake claim, regroup, UI legibility) |

## Session 49 Next Action

Save-state counter continuity. The new AI layer exposed a serialization seam: `restoreStateSnapshot` still rebuilt an obsolete `entityIdCounter`, while live runtime id creation now depends on `state.counters`. That had to be corrected before any further dynamic spawning work.
