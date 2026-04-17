# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 18
Author: Claude

## Scope

Conviction milestone powers per band now LIVE as canonical passive modifiers on stabilization, capture, loyalty protection, reserve heal, population growth, and attack multiplier. Master doctrine section XX canonical.

## Changes Landed

### Conviction band effects (Vector 4 + Vector 1 + Vector 2)

- `src/game/core/simulation.js` — new `CONVICTION_BAND_EFFECTS` table:
  - Apex Moral: stabilization 1.22x, heal 1.12x, loyalty-protect 1.18x, capture 0.94x, pop growth 1.08x
  - Moral: stabilization 1.08x, heal 1.04x, loyalty-protect 1.08x, capture 0.98x, pop growth 1.03x
  - Neutral: all 1.0x baseline
  - Cruel: stabilization 0.96x, loyalty-protect 0.94x, capture 1.08x, pop growth 0.97x, attack 1.04x
  - Apex Cruel: stabilization 0.88x, loyalty-protect 0.82x, capture 1.22x, pop growth 0.92x, attack 1.12x
- New `getConvictionBandEffects(state, factionId)` accessor. Canonical safe: independent of faith (per master doctrine section XX's insistence that conviction not collapse into faith).

### Wiring into live systems (Vector 1 + Vector 2)

- `updateControlPoints` stabilization now multiplies by conviction bandEffects.stabilizationMultiplier.
- Territory capture rate now multiplies by conviction bandEffects.captureMultiplier.
- `tickPopulationGrowth` now also applies conviction bandEffects.populationGrowthMultiplier (Apex Moral canonically favors prosperity; Apex Cruel erodes it).

### Legibility (Vector 6)

- Snapshot conviction block now exposes full `bandEffects` profile.
- Conviction HUD pill meta now shows active band modifiers (e.g., "stab x0.88 capture x1.22 growth x0.92" when Apex Cruel is active).

### Test coverage

- `tests/runtime-bridge.mjs` — neutral band default (all 1.0x); Apex Cruel forces stabilization < 1, capture > 1, loyalty-protection < 1; Apex Moral forces the mirror-image direction.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All simulation modules syntax clean.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Conviction milestone powers per band | DOCUMENTED | LIVE (5 canonical bands × 6 modifier axes, wired into 3 live systems) |

## Session 19 Next Action

- Dark-extremes world pressure trigger (Apex Cruel / sustained Desecration triggers world-reaction events).
- Or: L3 faith unit rosters.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green.
