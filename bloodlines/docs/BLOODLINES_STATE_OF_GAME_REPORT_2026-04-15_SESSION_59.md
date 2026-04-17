# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 59
Author: Codex

## Scope

Covert operations have been advanced from offense-only pressure into a live defensive court-watch layer. Rival courts can now raise counter-intelligence, that watch survives save and restore, it directly lowers espionage and assassination success, it gives guarded bloodline roles additional protection, it records foiled operations, the dynasty panel exposes the state, and Stonehelm now reacts by raising its own watch once hostile intelligence pressure is live.

## Changes Landed

### Counter-intelligence runtime (`src/game/core/simulation.js`)

- Added a live dynasty watch state under `dynasty.counterIntelligence`.
- Added `getCounterIntelligenceTerms(state, factionId)` and `startCounterIntelligenceOperation(state, factionId)`.
- Counter-intelligence now resolves as a real dynasty operation, then becomes an active timed watch around the bloodline court.
- Watch strength is not flat flavor text. It is built from:
  - spymaster or court-operator renown,
  - primary keep fortification depth,
  - active ward profile,
  - court loyalty support,
  - dynastic legitimacy support,
  - instability penalty from weak territorial loyalty.
- Espionage and assassination term calculation now read live counter-intelligence defense instead of treating covert defense as static launch-time math only.
- Assassination now also reads bloodline-role guard bonuses for protected roles such as head, heir, commander, spymaster, and governor.

### Resolution and consequence (`src/game/core/simulation.js`)

- Espionage resolution now re-evaluates against live counter-intelligence state at resolve time.
- Assassination resolution now re-evaluates against live counter-intelligence state and bloodline-role guarding at resolve time.
- Foiled espionage and assassination attempts now record interceptions on the defending watch.
- Successful interceptions now reinforce defending-house legitimacy, making covert defense touch both dynastic survival and political stability.
- Counter-intelligence watch expiry now cleans itself up in simulation time instead of remaining dead state.

### Legibility (`src/game/main.js`)

- Dynasty operations summary now includes counter-intelligence watch buildup.
- The dynasty panel now exposes:
  - active counter-intelligence watch state,
  - remaining duration,
  - watch strength,
  - guarded bloodline roles,
  - ward backing,
  - foiled-operation count,
  - most recent intercepted source when available.
- The player now has a live `Raise Counter-Intelligence` action in the dynasty panel.
- Rival-court covert actions now show when espionage or assassination is pushing into an active hostile watch, including the added defense burden and bloodline shield where relevant.

### AI reciprocity (`src/game/core/ai.js`)

- Stonehelm now raises counter-intelligence when the player already holds live intelligence on the enemy court, when active player covert pressure is in flight, or when hostility plus dynastic weakness makes covert defense urgent.
- This keeps the defensive covert layer world-reactive instead of leaving it as a player-only protection button.

### Save / restore continuity (`src/game/core/simulation.js`)

- Snapshot round-trip already carried raw dynasty state, but Session 59 extends restore continuity fully by rebuilding the `dynastyCounter` id prefix counter from restored watch ids.
- Fresh post-restore counter-intelligence watches now mint collision-safe ids.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 59 runtime coverage proving:
  - counter-intelligence can be raised as a live operation and becomes an active watch,
  - active watch reduces projected espionage success,
  - active watch reduces projected assassination success,
  - guarded bloodline roles receive explicit assassination defense,
  - foiled espionage increments watch interception state and reinforces legitimacy,
  - foiled assassination protects the target and increments watch interception state,
  - restore preserves live watch state,
  - fresh post-restore watch ids are collision-safe,
  - Stonehelm raises counter-intelligence when the player already holds live hostile intelligence.

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

Session 59 connects:

1. Covert defense and dynasty safety, because bloodline-role protection now changes assassination viability directly.
2. Covert defense and territorial condition, because court loyalty now strengthens or weakens defensive watch quality.
3. Covert defense and legitimacy, because successful interceptions now reinforce dynastic standing.
4. Covert defense and fortification-faith depth, because keep tier and ward backing feed directly into watch quality.
5. Covert defense and AI reactivity, because Stonehelm now raises watch when hostile intelligence pressure becomes real.
6. Covert defense and legibility, because the dynasty panel now surfaces active watch state and rival-court protection instead of hiding it in opaque success math.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Covert counter-intelligence and bloodline-targeting defense | DOCUMENTED | LIVE (first layer, with timed watch state, interception tracking, bloodline-role shielding, AI reciprocity, dynasty-panel legibility, and restore-safe id continuity) |

## Session 60 Next Action

Return to world pressure and late-stage escalation. The dynastic covert lane now has both offensive and defensive depth. The next highest-leverage adjacent layer is to make the wider world react more aggressively to player or rival consolidation through stronger late-stage pressure, rival escalation signals, and legible world-pressure state.
