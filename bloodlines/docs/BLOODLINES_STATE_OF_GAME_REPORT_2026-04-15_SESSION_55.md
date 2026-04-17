# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 55
Author: Codex

## Scope

Assassination and espionage are now LIVE as additional covert-operation types inside `dynasty.operations`. Sabotage was already live as a building-targeted covert lane. Session 55 extends that lane into rival-court surveillance and bloodline-targeted killing with real dynasty consequence, AI reciprocity, save continuity, and dynasty-panel legibility.

## Changes Landed

### Covert dynasty operations runtime (`src/game/core/simulation.js`)

- Added `startEspionageOperation(state, factionId, targetFactionId)`.
- Added `startAssassinationOperation(state, factionId, targetFactionId, targetMemberId)`.
- Added timed resolution for:
  - `espionage`,
  - `assassination`,
  inside `tickDynastyOperations(state)`.
- Espionage success now creates a live intelligence report on the rival court, including:
  - target legitimacy,
  - captive count,
  - lesser-house count,
  - active rival bloodline members,
  - location and exposure profile for those members.
- Assassination success now:
  - kills the target bloodline member,
  - clears commander or governor links where relevant,
  - applies legitimacy damage where role consequence requires it,
  - writes assassination into the fallen ledger,
  - triggers succession ripple,
  - forces mutual hostility between the rival houses.
- Assassination failure and exposed espionage now generate counter-intelligence consequence instead of disappearing silently.

### Save and restore continuity (`src/game/core/simulation.js`)

- `getFactionSnapshot` now exposes live intelligence reports for dynasty-panel legibility.
- Restore counter reconstruction now includes `dynastyIntel` ids, preventing post-restore intelligence-report collisions.
- Runtime coverage now proves restored states can mint a fresh intelligence report id after the previous report expires.

### Dynasty-panel legibility (`src/game/main.js`)

- Dynasty panel now exposes:
  - active espionage reports,
  - rival-court covert summary,
  - live espionage launch actions,
  - live assassination launch actions,
  - active covert-operation progress text for sabotage, espionage, and assassination.
- This keeps covert bloodline pressure readable through the existing command surface instead of adding dead UI.

### AI reciprocity (`src/game/core/ai.js`)

- Stonehelm now runs espionage against the player when no live report exists.
- Once Stonehelm holds a live report on the player, it can escalate into assassination against a prioritized bloodline target.
- This closes the player-only gap for the new covert lane.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 55 runtime coverage for:
  - successful espionage,
  - dynasty-panel snapshot exposure of intelligence reports,
  - restore persistence of intelligence reports,
  - fresh post-restore `dynastyIntel` id creation,
  - successful assassination with legitimacy and commander-link consequence,
  - failed assassination history and hostility consequence,
  - AI espionage reciprocity,
  - AI assassination escalation after live intelligence is established.

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

Session 55 connects:

1. Dynasty operations and bloodline mortality, covert action now kills real bloodline members instead of only targeting buildings.
2. Command and governance structure, assassination clears commander and governor links and destabilizes rival dynastic control.
3. Legitimacy and succession, assassination now triggers legitimacy loss and succession ripple.
4. Diplomacy and hostility, exposed covert action and assassination now force mutual hostility.
5. AI reactivity, Stonehelm now uses the same covert lane against the player.
6. Legibility and continuity, intelligence reports are visible in the dynasty panel and survive save / restore.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Additional covert-operation depth, assassination and espionage | DOCUMENTED | LIVE (first layer, with intelligence reports, bloodline killing, command and legitimacy consequence, AI reciprocity, dynasty-panel legibility, and restore continuity) |

## Session 56 Next Action

Faith operations, specifically missionary conversion and holy war declaration, as the next expansion of the already-live faith and dynasty lanes. The covert lane now has bloodline consequence and AI reciprocity. The next highest-leverage adjacent seam is to make covenant commitment produce real cross-faction operational pressure beyond passive exposure and ward behavior.
