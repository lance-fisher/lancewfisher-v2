# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 77
Author: Codex

## Scope

Session 76 carried source-aware world pressure into the captive lane. Session 77 carries the same principle into broad territorial expansion. When `territoryExpansion`, not `offHomeHoldings`, is the dominant cause of world pressure, tribes and Stonehelm no longer react only through generic pressure cadence. They now punish stretched territorial breadth directly by driving onto the weakest exposed marches, and the world surface names that breadth explicitly.

## Changes Landed

### Source-aware territory-expansion backlash (`src/game/core/ai.js`)

- Extended `pickTerritoryTarget(...)` so `territoryExpansion` can become a first-class targeting source instead of falling back to generic nearest-hostile selection.
- Added source-aware territorial-expansion bias toward:
  - low-loyalty marches,
  - unstabilized marches,
  - actively contested marches.
- Stonehelm territorial backlash now surfaces a dedicated `territoryExpansion` message when broad expansion is the dominant pressure source.

### Source-aware tribal world reaction (`src/game/core/ai.js`)

- Extended `getWorldPressureRaidTarget(...)` so tribes also read `territoryExpansion` directly.
- Tribal raids now punish stretched marches under broad territorial expansion, not only off-home holdings.
- Message-log text now distinguishes territorial-expansion backlash from generic world pressure and from off-home overextension.

### World-pill legibility (`src/game/main.js`)

- The world pill already exposed the dominant source label.
- Added explicit breadth metadata from `pressureSourceBreakdown.territoryExpansion`, so the player can now read how much of the current pressure comes from territorial breadth itself.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 77 coverage proving:
  - a staged three-home-march setup resolves `territoryExpansion` as the dominant world-pressure source,
  - the world snapshot surfaces `territory expansion` as the leading source with the expected contribution,
  - tribes issue live movement orders onto the weakest stretched march under territorial-expansion-led pressure,
  - Stonehelm issues live territorial movement orders onto that same stretched weak march,
  - the message log surfaces territorial-expansion backlash explicitly,
  - restore preserves the source identity and allows both rival and tribal backlash to relaunch against the same stretched march.

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

Session 77 connects:

1. World pressure and territorial control, because broad territorial expansion now draws specific punishment onto stretched marches instead of only adding abstract score.
2. World pressure and loyalty-state legibility, because low-loyalty and unstable marches now become the operational targets of rival and tribal backlash.
3. World pressure and AI behavior, because both Stonehelm and frontier tribes now read the same source breakdown and redirect movement accordingly.
4. World pressure and command-surface readability, because the world pill now exposes territorial breadth contribution directly.
5. World pressure and continuity, because restore preserves the control-point conditions that recreate the same expansion-led backlash after reload.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Source-aware territory-expansion backlash under world pressure | PARTIAL | LIVE (first layer, through tribal and rival weakest-march targeting, message-log legibility, world-pill breadth exposure, and restore-safe runtime proof) |

## Session 78 Next Action

Return to the house vector and deepen an already-live unique-unit lane instead of inventing a new house shell. The next highest-leverage action is to make Hartvale's `verdant_warden` provide real settlement-defense and local loyalty support in simulation, surface that support through an existing legibility surface, and preserve the new support state through validation and restore continuity.
