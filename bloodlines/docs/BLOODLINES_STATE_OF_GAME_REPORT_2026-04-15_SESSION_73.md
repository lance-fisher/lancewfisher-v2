# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 73
Author: Codex

## Scope

Session 72 carried source-aware world pressure into rival territorial response. Session 73 carries the same principle into the faith lane. When active holy war is the leading cause of world pressure, Stonehelm no longer reacts only through the generic pressure caps. It now accelerates missionary and holy-war timing more sharply, and that retaliation is surfaced through the live message log when it launches.

## Changes Landed

### Source-aware faith backlash (`src/game/core/ai.js`)

- Added a `holyWarSourceFocused` branch derived from the live world-pressure target profile.
- When `holyWar` is the leading source of the player's world pressure:
  - enemy missionary timing now compresses sharply,
  - enemy holy-war timing now compresses beyond the generic pressure branch.
- This means faith backlash is now tied to the cause of pressure, not only to the fact that pressure exists.

### Legibility (`src/game/core/ai.js`)

- Added message-log surfacing when Stonehelm turns that holy-war-led pressure into live retaliation:
  - renewed missionary backlash,
  - counter-holy-war declaration.
- This keeps the new layer visible through an already-live runtime surface.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 73 coverage proving:
  - active holy war resolves as the dominant world-pressure source in the staged setup,
  - holy-war-led pressure compresses missionary and holy-war timers more sharply than the generic branch,
  - enemy AI launches missionary backlash under that source state,
  - enemy AI launches counter-holy-war declaration under that source state,
  - the message log surfaces both forms of backlash,
  - restore preserves the timing behavior.

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

Session 73 connects:

1. World pressure and faith operations, because active holy war now drives sharper missionary and holy-war backlash.
2. World pressure and AI behavior, because Stonehelm faith timing now changes based on the dominant source rather than only the pressure level.
3. Faith operations and legibility, because missionary and holy-war backlash now surface in the message log.
4. World pressure and continuity, because restore preserves the source-aware timing behavior.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Source-aware faith backlash to holy-war-led world pressure | PARTIAL | LIVE (first layer, through source-aware missionary plus holy-war timing, launch legibility, and restore-safe runtime proof) |

## Session 74 Next Action

Deepen source-aware world-pressure response into the covert lane. The next canonical layer is to make enemy counter-intelligence and covert retaliation react more sharply when hostile operations are the leading source of world pressure.
