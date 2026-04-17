# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 75
Author: Codex

## Scope

Session 74 carried source-aware world pressure into the covert lane. Session 75 carries the same principle into the dark-extremes lane. When `darkExtremes` is the dominant cause of world pressure, Stonehelm no longer reacts only through generic pressure tempo. It now redirects punitive territorial pressure onto the weakest player-held marches and sharpens bloodline decapitation pressure when it already holds court intelligence.

## Changes Landed

### Source-aware dark-extremes backlash (`src/game/core/ai.js`)

- Added a `darkExtremesSourceFocused` branch derived from the live world-pressure target profile.
- When `darkExtremes` is the leading source of the player's world pressure:
  - enemy attack timing now compresses into punitive-war cadence,
  - enemy territory timing now compresses more sharply,
  - enemy assassination timing now compresses into a live bloodline-backlash lane.

### Punitive territorial targeting (`src/game/core/ai.js`)

- Extended `pickTerritoryTarget(...)` so source-aware dark-extremes response now prefers the targeted rival's weakest-held marches instead of only nearest hostile ground.
- Weak loyalty and unstable control state now create explicit punitive-target bias when dark extremes are the dominant pressure source.

### Legibility (`src/game/core/ai.js`)

- Added message-log surfacing for:
  - punitive territorial backlash against weakened marches,
  - source-aware bloodline decapitation backlash.
- The world pill already exposes `dark extremes` as the leading source, so the new message-log layer makes the reaction legible without adding dead UI.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 75 coverage proving:
  - sustained `Apex Cruel` resolves as the dominant world-pressure source in the staged setup,
  - dark-extremes-led pressure compresses attack, territorial, and assassination timing beyond the generic pressure branch,
  - Stonehelm redirects live territorial movement toward the weakest player-held march,
  - Stonehelm launches assassination backlash when court intelligence is available,
  - the message log surfaces both dark-extremes backlash branches,
  - restore preserves punitive targeting and timer-compression behavior.

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

Session 75 connects:

1. World pressure and conviction, because sustained `Apex Cruel` now causes a specific retaliatory branch instead of only contributing score.
2. World pressure and territorial control, because dark-extremes-led backlash now drives Stonehelm toward the weakest player-held march.
3. World pressure and dynastic bloodline consequence, because source-aware assassination backlash now escalates when a cruel court is already under observation.
4. World pressure and legibility, because the world pill exposes the source and the message log exposes the punitive response.
5. World pressure and continuity, because restore preserves the targeting and timing behavior.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Source-aware dark-extremes backlash under world pressure | PARTIAL | LIVE (first layer, through punitive territorial targeting, assassination backlash timing, message-log legibility, and restore-safe runtime proof) |

## Session 76 Next Action

Deepen source-aware world-pressure response into the captives branch. The next canonical layer is to make rival reaction sharpen when held captives are the dominant source of world pressure.
