# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 72
Author: Codex

## Scope

Session 71 made world pressure explain itself and turned tribal raids toward off-home marches when continental overextension was the leading source. Session 72 carries that same source-aware logic into rival-kingdom behavior. Stonehelm no longer reacts to overextended world-pressure targets only through faster generic timers. It now redirects live territorial pressure onto the off-home marches that are actually causing the pressure.

## Changes Landed

### Source-aware rival territorial response (`src/game/core/ai.js`)

- Deepened `pickTerritoryTarget(...)` so it can read a targeted rival faction and the leading world-pressure source.
- When the pressured rival's dominant source is `offHomeHoldings`, Stonehelm now hard-prioritizes that rival's off-home marches before scoring generic nearby territory.
- This keeps the reaction tied to the cause of pressure instead of only to the identity of the pressured realm.

### Legibility (`src/game/core/ai.js`)

- Added message-log surfacing for the new rival-pressure branch.
- When Stonehelm redirects onto off-home marches because continental overextension is the active source, the message log now states that explicitly.
- No new decorative panel was introduced. The new behavior is visible through an already-live runtime surface.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 72 coverage proving:
  - enemy territorial pressure issues live movement orders,
  - those orders move toward the off-home march rather than nearer home-continent marches when off-home holdings are the leading pressure source,
  - the message log exposes that source-aware rival response,
  - restore preserves the conditions required for the same off-home redirect to happen again after reload.

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

Session 72 connects:

1. World pressure and rival military behavior, because Stonehelm now attacks the territorial cause of overextension.
2. World pressure and continental architecture, because off-home holdings are now materially punished by hostile territorial response.
3. World pressure and legibility, because the message log names the source-aware enemy redirect directly.
4. World pressure and restore continuity, because the same redirect logic survives snapshot round-trip without hidden manual state.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Source-aware rival territorial response to off-home overextension | PARTIAL | LIVE (first layer, through source-aware march selection, message-log legibility, and restore-safe runtime proof) |

## Session 73 Next Action

Deepen source-aware world-pressure response beyond continental breadth. The next canonical layer is to make live faith backlash read the same source breakdown, starting with stronger enemy missionary and holy-war response when active holy war is the leading cause of world pressure.
