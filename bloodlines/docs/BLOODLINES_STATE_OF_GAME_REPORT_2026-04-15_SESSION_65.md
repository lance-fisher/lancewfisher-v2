# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 65
Author: Codex

## Scope

World pressure has been deepened from internal cadet instability into external splinter opportunism. Breakaway minor houses already claimed territory, levied local retinue, and defended or retook their march. They now read live parent-realm world pressure, accelerate levy growth, expand retinue ceiling under severe overextension, sharpen retake cadence, surface that pressure in dynasty and world legibility, and preserve the new state through restore.

## Changes Landed

### Splinter opportunity profile (`src/game/core/simulation.js`)

- Added `getMinorHousePressureOpportunityProfile(state, factionOrId)` as the shared seam between:
  - parent-realm world pressure,
  - hostile minor-house levy tempo,
  - hostile minor-house retake tempo,
  - hostile minor-house retinue-cap escalation.
- The profile activates only when:
  - the faction is a live `minor_house`,
  - it still tracks an `originFactionId`,
  - the parent realm is the active dominant world-pressure target,
  - the parent world-pressure level is above zero.
- Severity scales with:
  - current world-pressure level,
  - excess world-pressure score beyond the dominance threshold.

### Breakaway levy and territorial escalation (`src/game/core/simulation.js`)

- Minor-house levy state now persists:
  - `parentPressureLevel`,
  - `parentPressureStatus`,
  - `parentPressureLevyTempo`,
  - `parentPressureRetakeTempo`,
  - `parentPressureRetinueBonus`.
- `tickMinorHouseTerritorialLevies` now:
  - raises breakaway retinue faster under parent overextension,
  - increases local retinue cap under severe and convergence pressure,
  - emits live message-log updates when splinter momentum begins or eases.

### Minor-house AI escalation (`src/game/core/ai.js`)

- Minor-house territorial AI now reads the shared splinter-opportunity profile directly.
- Threat acquisition expands under higher parent pressure.
- Defense and regroup timers now shrink under parent overextension, making retake behavior materially more aggressive instead of only changing text.

### Legibility (`src/game/main.js`, `src/game/core/simulation.js`)

- The dynasty panel now surfaces splinter opportunity state directly on hostile minor-house rows:
  - parent pressure severity,
  - levy acceleration,
  - retake acceleration,
  - pressure-driven retinue-cap bonus.
- The world pill now also exposes:
  - pressured splinter count,
  - splinter levy tempo,
  - splinter retake tempo,
  - splinter retinue-cap bonus,
  when the current realm is the dominant world-pressure target.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 65 runtime coverage proving:
  - severe parent world pressure raises hostile splinter levy tempo,
  - severe parent world pressure increases hostile splinter retinue cap,
  - pressured splinter marches build levy progress faster than calm ones,
  - pressured splinter retake cadence is shorter than calm retake cadence,
  - world-pressure snapshot legibility exposes splinter opportunism,
  - export and restore preserve the new splinter-pressure state.

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

Session 65 connects:

1. World pressure and breakaway levy growth, because hostile splinters now build retinue faster when the parent realm is the dominant target.
2. World pressure and breakaway territorial AI, because retake posture now becomes materially more aggressive under parent overextension.
3. World pressure and dynasty-panel legibility, because hostile splinter opportunity severity and tempo are visible where the player already reads cadet and minor-house state.
4. World pressure and world-pill legibility, because the dominant target now sees not only internal cadet strain but also external splinter acceleration.
5. World pressure and snapshot continuity, because the new splinter-pressure state survives restore instead of evaporating after reload.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| World-pressure-driven splinter opportunism against hostile parent realms | DOCUMENTED | LIVE (first layer, through levy acceleration, retake acceleration, retinue-cap escalation, world-pill legibility, dynasty-panel legibility, and restore continuity) |

## Session 66 Next Action

Return to post-dossier covert follow-up. Counter-intelligence dossiers already create actionable hostile-court knowledge, and assassination already reuses that knowledge. The next canonical layer is to let dossier-backed retaliation and court-counterplay drive smarter sabotage escalation against live infrastructure and live target classes instead of stopping at assassination reuse alone.
