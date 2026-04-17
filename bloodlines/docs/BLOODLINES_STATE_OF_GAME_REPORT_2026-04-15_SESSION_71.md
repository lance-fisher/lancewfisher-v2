# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 71
Author: Codex

## Scope

Session 70 made Ironmark's unique unit honest for AI courts. Session 71 returns to the world-pressure lane and deepens it through source awareness. World pressure no longer exposes only score and severity. The simulation now records why a realm is drawing pressure, surfaces that cause in the existing world pill, and makes tribal world reaction strike off-home marches directly when continental overextension is the leading source.

## Changes Landed

### World-pressure source breakdown (`src/game/core/simulation.js`)

- Added `getWorldPressureSourceBreakdown(state, factionId)` as the shared seam for pressure composition.
- World pressure now resolves through explicit live sources:
  - `territoryExpansion`
  - `offHomeHoldings`
  - `holyWar`
  - `captives`
  - `hostileOperations`
  - `darkExtremes`
- `calculateWorldPressureScore(...)` now reads that shared breakdown instead of duplicating score logic.
- `getWorldPressureTargetProfile(...)` now carries:
  - total score
  - top source id
  - top source label
  - the full source contribution object
- `getRealmConditionSnapshot(...)` now exports that state through the world block, including `topPressureSourceId`, `topPressureSourceLabel`, and `pressureSourceBreakdown`.

### Source-aware tribal reaction (`src/game/core/ai.js`)

- `getWorldPressureRaidTarget(...)` now reads the live source breakdown instead of treating all pressure leaders the same.
- When `offHomeHoldings` is the leading source, tribes now hard-prioritize owned off-home marches before scoring ordinary home-continent marches.
- This is not a decorative message-only branch. It changes live tribal movement orders and therefore changes which march the world reacts against.
- The message log now explicitly states when tribes are striking off-home marches because continental overextension is sharpening pressure.

### Legibility (`src/game/main.js`)

- The existing world pill now surfaces the active leading source through the existing metadata line.
- This keeps the new depth inside a live command surface instead of adding a dead dashboard shell.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 71 runtime coverage proving:
  - off-home holdings contribute doubled source pressure,
  - territorial breadth still contributes separately,
  - off-home holdings resolve as the top source in the staged setup,
  - the world snapshot exposes both the top source label and the source breakdown,
  - tribal raiders move toward the off-home march rather than the nearer home march when overextension is the dominant source,
  - the message log exposes that off-home targeting,
  - restore preserves the leading source id and label.

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

Session 71 connects:

1. World pressure and continental holdings, because off-home expansion is now a named live source in score composition.
2. World pressure and neutral-world reactivity, because tribes now choose raid targets from the actual dominant source instead of only from realm identity.
3. World pressure and legibility, because the world pill and message log now surface why the pressure is happening.
4. World pressure and continuity, because source identity survives snapshot restore.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Source-aware tribal reaction to world-pressure composition | PARTIAL | LIVE (first layer, through source breakdown, off-home targeting, world-pill plus message-log legibility, and restore continuity) |

## Session 72 Next Action

Deepen source-aware world pressure into rival-kingdom response. Tribes now attack the cause of continental overextension. The next canonical layer is to make rival kingdoms read the same source breakdown and contest the dominant source directly, starting with off-home holdings when overextension is leading.
