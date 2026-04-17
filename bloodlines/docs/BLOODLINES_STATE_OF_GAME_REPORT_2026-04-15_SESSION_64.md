# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 64
Author: Codex

## Scope

World pressure has been deepened from external punishment into internal dynastic strain. Dominant realms already suffered frontier loyalty loss and legitimacy pressure. They now also pressure their active lesser houses, worsening cadet-house daily loyalty drift under overextension, surfacing that instability in both the dynasty panel and world-pressure snapshot, and preserving the new state through restore.

## Changes Landed

### Internal world-pressure instability (`src/game/core/simulation.js`)

- Added `getWorldPressureCadetInstabilityProfile(state, factionOrId)` as a shared pressure-to-dynasty seam.
- Cadet-house instability now activates only when:
  - the faction is the live world-pressure target,
  - world-pressure level is above zero,
  - the dynasty still has active lesser houses to strain.
- The new cadet-pressure term scales with:
  - current world-pressure level,
  - excess world-pressure score beyond the dominance threshold.
- `tickLesserHouseLoyaltyDrift` now applies that world-pressure term to active cadet branches alongside:
  - baseline legitimacy and conviction drift,
  - mixed-bloodline pressure,
  - marital-anchor consequence.
- Dynasty state now records `lastCadetWorldPressureStatus`, and the message log now fires when cadet houses newly come under or emerge from world-pressure strain.

### Lesser-house continuity (`src/game/core/simulation.js`)

- Active lesser houses now persist:
  - `worldPressureStatus`,
  - `worldPressurePressure`,
  - `worldPressureLevel`.
- Newly promoted lesser houses initialize against the current world-pressure profile instead of waiting for a later recalculation tick.
- Snapshot export and restore preserve the new cadet-pressure state through the existing dynasty deep-copy lane.

### Legibility (`src/game/main.js`)

- Lesser-house rows in the dynasty panel now surface:
  - current world-pressure severity,
  - cadet-specific daily drift from world pressure.
- The world pill meta now also reports:
  - cadet drift per day,
  - pressured lesser-house count,
  when the current faction is the dominant world-pressure target.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 64 runtime coverage proving:
  - a calm cadet branch drifts more slowly than one under severe world pressure,
  - targeted world pressure records severity, level, and negative cadet drift on the lesser-house state itself,
  - snapshot world-pressure legibility now exposes cadet-loyalty penalty and pressured cadet count,
  - snapshot export and restore preserve the new internal-pressure state.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.
- `python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines` served `play.html` successfully.
- Static browser-lane verification confirmed:
  - `play.html` returned `200`,
  - served markup still contains `game-shell`, `resource-bar`, `realm-condition-bar`, `dynasty-panel`, `faith-panel`, and `message-log`.

## Canonical Interdependency Check

Session 64 connects:

1. World pressure and lesser-house loyalty drift, because overextended dominance now worsens cadet-house daily stability.
2. World pressure and defection pressure, because worsened cadet drift now pushes branches toward break conditions faster.
3. World pressure and dynasty-panel legibility, because branch-level instability is now visible in the same panel that already surfaces cadet identity and marriage anchors.
4. World pressure and snapshot continuity, because the new internal-pressure state survives restore instead of evaporating after reload.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| World-pressure-driven internal dynastic destabilization | DOCUMENTED | LIVE (first layer, through cadet-house drift pressure, world-pill legibility, dynasty-panel legibility, and restore continuity) |

## Session 65 Next Action

Continue the world-pressure lane through minor-house opportunism. Breakaway branches already exist, claim territory, defend, and levy. The next canonical layer is to let world pressure on the parent realm embolden those hostile offshoots through stronger levy tempo or retake opportunism so late-stage overextension pressures both internal cadets and external splinters.
