# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 68
Author: Codex

## Scope

World pressure already created frontier loyalty loss, legitimacy strain, cadet instability, splinter opportunism, rival timer compression, and tribal retargeting. Session 68 deepens the top tier of that lane. `Convergence` pressure now has its own shared escalation profile that sharpens rival-kingdom tempo beyond `Severe`, accelerates tribal raid cadence beyond the old level-3 behavior, exposes those caps in the world snapshot, and surfaces them in the world pill.

## Changes Landed

### Shared convergence profile (`src/game/core/simulation.js`)

- Added `getWorldPressureConvergenceProfile(state, factionId)` as the shared high-tier escalation seam.
- The profile activates only when a kingdom is both:
  - the live world-pressure target,
  - at level `3` (`Convergence`).
- The profile now exports:
  - rival attack tempo cap,
  - rival territory tempo cap,
  - sabotage tempo cap,
  - espionage tempo cap,
  - assassination tempo cap,
  - missionary tempo cap,
  - holy-war tempo cap,
  - tribal raid timer multiplier.

### Rival-kingdom and tribal escalation (`src/game/core/ai.js`)

- Stonehelm now reads the shared convergence profile and compresses:
  - military tempo,
  - territory pressure tempo,
  - sabotage tempo,
  - espionage tempo,
  - assassination tempo,
  - missionary tempo,
  - holy-war tempo.
- Frontier tribes now read the same convergence profile to shorten their next raid cadence beyond the prior level-3 multiplier.
- Tribal warning text now distinguishes normal pressure convergence from true world-pressure `Convergence`.

### Legibility (`src/game/core/simulation.js`, `src/game/main.js`)

- `getRealmConditionSnapshot` now exposes convergence-only world-pressure fields:
  - `convergenceActive`,
  - rival tempo caps,
  - tribal raid cadence multiplier.
- The world pill now surfaces convergence tempo directly when the player is the targeted dominant realm.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 68 coverage proving:
  - a level-3 dominant realm activates the convergence profile,
  - `Convergence` compresses rival timers more sharply than `Severe`,
  - `Convergence` shortens tribal raid cadence more sharply than `Severe`,
  - the world snapshot exposes convergence tempo caps and raid cadence,
  - restore preserves convergence-state legibility.

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

Session 68 connects:

1. World pressure and rival AI tempo, because `Convergence` now sharpens enemy attack, territory, covert, and faith pressure beyond the prior shared compression tier.
2. World pressure and tribal world reactivity, because tribes now accelerate raid cadence more sharply at `Convergence`.
3. World pressure and dashboard legibility, because the world pill now exposes the convergence-only tempo profile.
4. World pressure and snapshot continuity, because restore preserves the convergence state and derived legibility.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Convergence-tier world-pressure rival tempo and tribal escalation | PARTIAL | LIVE (first layer, through shared convergence profile, sharper rival tempo, harsher tribal cadence, world-pill legibility, and restore continuity) |

## Session 69 Next Action

Advance the lagging house vector through Ironmark’s dormant unique-unit lane. The next canonical layer is to make `axeman` a real Ironmark-only unit with honest command-surface access, live blood-production coupling, and validation coverage rather than leaving that house identity trapped in disabled data.
