# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 69
Author: Codex

## Scope

The house vector had gone dormant after Hartvale's Session 63 enablement. Session 69 advances the next honest house-identity lane by making Ironmark's `axeman` a real runtime unit instead of inert data. The new lane now uses the existing house gate, applies a heavier Ironmark blood levy and blood-production burden than standard infantry, surfaces that cost honestly in the command panel, and survives validation plus restore continuity.

## Changes Landed

### Ironmark unique-unit activation (`data/units.json`, `data/buildings.json`)

- Enabled `axeman` as a live prototype unit.
- Preserved `house: "ironmark"` so simulation-side house ownership remains authoritative.
- Added:
  - `ironmarkBloodPrice: 2`
  - `bloodProductionLoadDelta: 3`
- Added `axeman` to the shared Barracks roster so the same roster can now serve Hartvale and Ironmark while simulation remains the actual gate.

### Blood-production coupling (`src/game/core/simulation.js`)

- Deepened `queueProduction(...)` so Ironmark's blood-production lane no longer assumes every non-worker combat unit costs the same blood levy.
- Training now reads per-unit fields:
  - `ironmarkBloodPrice`
  - `bloodProductionLoadDelta`
- `axeman` therefore now:
  - removes 2 living population immediately when queued,
  - adds 3 blood-production load,
  - still uses the same restore-safe production queue and realm-condition snapshot lane already in place.

### Command-surface honesty (`src/game/main.js`)

- Barracks training details now surface:
  - normal resource cost,
  - population cost,
  - Ironmark blood levy,
  - added blood-production load,
  - reserve failure when the player cannot safely pay the levy.
- This keeps the house unit legible through the existing command surface instead of adding decorative new UI.

### Validation and runtime proof (`tests/data-validation.mjs`, `tests/runtime-bridge.mjs`)

- Added schema validation proving:
  - `axeman` exists,
  - it is prototype-enabled,
  - it is marked `house: "ironmark"`,
  - it carries elevated blood levy and blood-production load,
  - Barracks exposes it in the shared trainable roster.
- Added runtime coverage proving:
  - Ironmark can surface and queue `axeman`,
  - Hartvale cannot surface or queue it,
  - Stonehelm cannot surface it,
  - queueing `axeman` immediately consumes the heavier blood levy,
  - queueing `axeman` increases live blood-production load,
  - the realm snapshot reflects that load,
  - restore preserves the queued Axeman and its blood-production consequence.

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

Session 69 connects:

1. House identity and production access, because `axeman` is now surfaced and queued through the same house-gated runtime seam used by Hartvale.
2. House identity and civilizational cost, because Axeman training now consumes a heavier live Ironmark blood levy and blood-production burden than generic infantry.
3. House identity and command-surface legibility, because the command panel now tells the player the real blood and load cost of the unit.
4. House identity and restore continuity, because the queued Axeman and resulting blood-production load survive snapshot round-trip.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Ironmark `axeman` unique-unit lane | PARTIAL | LIVE (first layer, through house-gated runtime access, blood-production coupling, command-surface honesty, and restore continuity) |

## Session 70 Next Action

Extend the same house lane into AI awareness. The next canonical layer is to make Ironmark-controlled AI actually recruit `axeman` through the same simulation gate, weigh that choice against live blood-production pressure, and expose the result through existing runtime legibility rather than leaving the new house unit as a player-only branch.
