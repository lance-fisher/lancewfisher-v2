# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 51
Author: Codex

## Scope

Mixed-bloodline defection weighting is now LIVE as the first real runtime layer. Marriage-generated outside-house bloodline lineage now affects lesser-house loyalty drift and instability, with active marriage ties and renewed hostility to the outside house pulling that pressure in opposite directions.

## Changes Landed

### Mixed-bloodline instability runtime (`src/game/core/simulation.js`)

- Extended lesser-house state so promotion records preserve:
  - `mixedBloodline`,
  - `mixedBloodlineHouseId`,
  - `mixedBloodlinePressure`,
  - `currentDailyLoyaltyDelta`.
- Reworked lesser-house loyalty drift so per-house daily drift now equals:
  - parent-wide drift from legitimacy, conviction, and recent fallen pressure,
  - plus a mixed-bloodline pressure term when the founder carries outside-house lineage.
- Mixed-bloodline pressure now reacts to two already-live systems:
  - active marriage state with the outside house buffers instability,
  - renewed hostility toward the outside house worsens instability.

### Legibility (`src/game/main.js`)

- Player lesser-house rows in the dynasty panel now expose mixed-bloodline drift directly through the existing dynasty surface, including outside-house identity and current per-day pressure when present.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 51 runtime coverage proving:
  - a cross-house marriage produces a mixed-bloodline child,
  - that child can found a lesser house,
  - active marriage calm partly buffers instability,
  - renewed hostility to the outside house worsens lesser-house loyalty,
  - mixed-bloodline provenance survives snapshot restore.

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

Session 51 connects:

1. Marriage and child generation, mixed-bloodline metadata now matters after birth instead of remaining passive record text.
2. Lesser-house loyalty drift, cadet-branch instability now differs by bloodline composition.
3. Diplomacy and hostility, renewed hostility to the outside house increases the instability pull.
4. Legibility, dynasty-panel lesser-house rows expose outside-house pull without a new fake panel.
5. Save and restore continuity, mixed-bloodline lesser-house provenance survives snapshot round-trip.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Mixed-bloodline defection slider | DOCUMENTED | LIVE (first layer, integrated into lesser-house daily loyalty drift and hostility-aware instability) |

## Session 52 Next Action

Faith-compatibility weighting in AI marriage proposal and acceptance logic. Marriage is already live, AI reciprocity is already live, and mixed-bloodline consequences are now live. The next canonical layer is to make covenant and doctrine compatibility influence whether AI sees a marriage as strategically acceptable.
