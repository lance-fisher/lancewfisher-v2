# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 56
Author: Codex

## Scope

Faith operations are now LIVE as a first operational extension of the committed-covenant lane. Session 56 adds missionary conversion pressure and holy war declaration as runtime systems that consume real faith intensity, alter hostility and territorial pressure, pulse ongoing zeal, survive restore, surface through the faith panel, and trigger AI response.

## Changes Landed

### Faith operations runtime (`src/game/core/simulation.js`)

- Added `getMissionaryTerms`, `startMissionaryOperation`, `getHolyWarDeclarationTerms`, and `startHolyWarDeclaration`.
- Added timed `missionary` and `holy_war` resolution inside `tickDynastyOperations(state)`.
- Missionary success now:
  - raises target exposure to the attacking covenant,
  - erodes rival covenant intensity when the target is already committed elsewhere,
  - pressures real territorial loyalty when the target holds marches,
  - falls back to legitimacy pressure when no march is held.
- Missionary failure now creates counter-pressure instead of disappearing silently, and warded or Order-protected targets can escalate the exposed faith operation into hostility.
- Holy war declaration now:
  - forces mutual hostility,
  - creates a live `faith.activeHolyWars` state entry,
  - immediately pressures rival territorial loyalty or legitimacy,
  - pulses continuing loyalty pressure and zeal regeneration over time,
  - survives export and restore with fresh `faithHolyWar` id continuity.

### Faith-panel legibility (`src/game/main.js`)

- Faith panel now surfaces:
  - active missionary and holy-war operations,
  - outbound holy wars,
  - incoming holy wars,
  - live action buttons for missionary pressure and holy war declaration against rival courts.
- Operation summaries now correctly distinguish missionary and holy-war actions instead of collapsing into captivity text.

### AI and world reactivity (`src/game/core/ai.js`)

- Stonehelm can now launch missionary pressure against the player when covenant identities diverge.
- Stonehelm can now escalate into holy war declaration when covenant fracture and dynastic pressure are both live.
- Active holy war state now shortens Stonehelm territorial and attack timers so faith war changes world behavior rather than sitting only in panel text.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 56 runtime coverage for:
  - successful missionary conversion pressure,
  - failed missionary pressure against a warded Order court,
  - holy war declaration as a live persistent faith state,
  - outgoing and incoming holy war snapshot exposure,
  - holy war pulse continuation over time,
  - holy war restore persistence and fresh post-restore `faithHolyWar` ids,
  - enemy AI missionary launch,
  - enemy AI holy war escalation.

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
- Static browser-lane verification confirmed served markup still contains `game-shell`, `resource-bar`, `realm-condition-bar`, `dynasty-panel`, `faith-panel`, and `message-log`.

## Canonical Interdependency Check

Session 56 connects:

1. Faith commitment and operational pressure, covenant choice now drives timed operations instead of passive exposure only.
2. Diplomacy and hostility, exposed missionary work and holy war declaration now change faction posture directly.
3. Territory and governance, missionary pressure and holy war now push real march loyalty or dynastic legitimacy.
4. AI behavior, Stonehelm now launches missionary pressure and holy war in response to live faith fracture.
5. Save continuity, active holy wars survive restore and keep minting collision-safe ids.
6. Legibility, the faith panel and snapshot surfaces now expose outgoing and incoming covenant war pressure.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Faith operations, missionary conversion and holy war declaration | DOCUMENTED | LIVE (first layer, with faith-intensity fuel, territorial or legitimacy pressure, hostility consequence, AI reactivity, faith-panel legibility, and restore continuity) |

## Session 57 Next Action

Marriage-governance controls by head of household. Marriage proposal and acceptance are live, mixed-bloodline consequence is live, faith compatibility is live, and death-driven dissolution is live. The next highest-leverage dynastic seam is to make marriage authority run through the head of bloodline so unions become governed political acts instead of free-floating bilateral toggles.
