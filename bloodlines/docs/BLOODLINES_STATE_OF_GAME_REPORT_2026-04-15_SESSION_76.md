# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 76
Author: Codex

## Scope

Session 75 carried source-aware world pressure into the dark-extremes lane. Session 76 carries the same principle into the captive lane. When `captives` is the dominant cause of world pressure, Stonehelm no longer reacts only through generic hostility or passive timer compression. It now accelerates captive recovery through the live dynasty-operations system, choosing between rescue and ransom from the actual captive state already present in runtime.

## Changes Landed

### Source-aware captive backlash (`src/game/core/ai.js`)

- Added a `captivesSourceFocused` branch derived from the live world-pressure target profile.
- Added `pickAiCaptiveRecoveryTarget(...)` so Stonehelm can prioritize its own strategically critical captured bloodline members.
- When `captives` is the leading source of the player's world pressure:
  - enemy captive-recovery timing now compresses sharply,
  - Stonehelm now launches live rescue or ransom response through the existing dynasty-operations lane.

### Dynastic recovery choice (`src/game/core/ai.js`)

- Stonehelm now chooses between:
  - covert rescue for high-priority or openly hostile recovery cases,
  - immediate ransom negotiation when rescue is not prioritized and the captive lane is open.
- This keeps captive backlash inside already-live dynastic systems instead of inventing a parallel mechanic.

### Legibility (`src/game/core/ai.js`)

- Added message-log surfacing for:
  - covert recovery backlash,
  - immediate ransom backlash.
- The world pill already exposes `captives` as the leading source, so the message log now makes the rival recovery behavior visible in the same live chain.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 76 coverage proving:
  - held captives resolve as the dominant world-pressure source in the staged setup,
  - captive-led pressure compresses Stonehelm's new captive-recovery timer,
  - Stonehelm launches rescue backlash for strategically critical captives,
  - Stonehelm launches ransom backlash when rescue is not prioritized,
  - the message log surfaces both forms of captive backlash,
  - restore preserves the captive-pressure state well enough for Stonehelm to relaunch rescue after snapshot round-trip.

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

Session 76 connects:

1. World pressure and captivity, because held captives now cause specific rival recovery action instead of only contributing score.
2. World pressure and dynasty operations, because source-aware backlash now launches real rescue or ransom operations.
3. Captivity and AI behavior, because Stonehelm now evaluates captured bloodline priority and hostility when choosing recovery method.
4. World pressure and legibility, because the world pill exposes the source and the message log exposes the recovery response.
5. World pressure and continuity, because restore preserves enough captive state for the rival to relaunch rescue after reload.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Source-aware captive backlash under world pressure | PARTIAL | LIVE (first layer, through AI recovery timing, rescue-or-ransom launch behavior, message-log legibility, and restore-safe runtime proof) |

## Session 77 Next Action

Deepen source-aware world-pressure response into the territory-expansion branch. The next canonical layer is to make world or rival reaction sharpen when broad territorial expansion, not off-home holdings specifically, is the dominant source of world pressure.
