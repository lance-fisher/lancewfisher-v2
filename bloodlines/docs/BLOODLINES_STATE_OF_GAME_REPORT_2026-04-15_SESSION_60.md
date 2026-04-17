# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 60
Author: Codex

## Scope

World pressure and late-stage escalation have been advanced from a mostly passive signal count into a live dominance-response layer. Overextended kingdoms now accumulate pressure score and escalation streaks, the dominant realm suffers frontier-loyalty and legitimacy consequence, Stonehelm accelerates covert and military pressure against that leader, tribes preferentially raid the pressured kingdom, the 11-pill dashboard exposes the state, and snapshot restore preserves the new pressure fields.

## Changes Landed

### World-pressure runtime (`src/game/core/simulation.js`)

- Added live world-pressure score, streak, and level state on kingdom factions.
- Added `getWorldPressureLeaderProfile(state)` and `getWorldPressureTargetProfile(state, factionId)`.
- Pressure score now reads real already-live conditions:
  - territorial breadth beyond the initial holding base,
  - off-home-continent expansion,
  - active holy wars,
  - held captives,
  - hostile covert or holy-war operations in flight,
  - sustained dark-extremes descent.
- Realm-cycle resolution now updates a dominant-pressure leader, increments or decays streaks, maps streaks into `Watchful`, `Severe`, and `Convergence` pressure levels, and emits escalation or easing messages.
- Sustained world pressure now applies live consequence:
  - the weakest owned march loses loyalty each realm cycle,
  - severe pressure and above also reduce dynastic legitimacy.

### Snapshot and restore continuity (`src/game/core/simulation.js`)

- Snapshot export now preserves `worldPressureScore`, `worldPressureStreak`, and `worldPressureLevel` for every faction.
- Restore now reconstructs those fields for both standard and dynamically recreated factions.
- `getRealmConditionSnapshot` world-pressure output now exposes:
  - score,
  - pressure level,
  - pressure label,
  - streak,
  - whether this realm is the current world target,
  - leader faction identity,
  - leader score and streak,
  - live frontier loyalty penalty,
  - live legitimacy pressure.

### AI and world reactivity (`src/game/core/ai.js`)

- Stonehelm now compresses attack, territory, sabotage, espionage, assassination, and holy-war timers when the player is the active world-pressure target.
- Holy-war declaration logic now also treats target-world-pressure state as a meaningful escalation signal.
- Tribal AI now reads the dominant world-pressure leader, preferentially raids that leader's marches, and shortens raid cadence as global pressure intensifies.

### Legibility (`src/game/main.js`)

- The `World` pill now shows a pressure label when the player is the current target instead of collapsing everything into a generic signal count.
- World meta now surfaces the dominant-leader score or, when targeted, the player's own pressure score, streak, frontier-loyalty penalty, and legitimacy pressure alongside the existing tribe, contested-territory, and operations counts.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 60 runtime coverage proving:
  - sustained dominant expansion becomes a live world-pressure target,
  - pressure reaches severe level through realm-cycle accumulation,
  - weakest-frontier pressure remains measurably lower than the rest of the realm,
  - legitimacy drops under severe pressure,
  - snapshot export and restore preserve score, streak, level, and target state,
  - Stonehelm compresses offensive timers against the pressure leader,
  - tribal AI redirects raids toward the dominant world-pressure leader and accelerates its raid cadence.

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

Session 60 connects:

1. World pressure and territory control, because the weakest controlled march now takes live loyalty damage under sustained dominance.
2. World pressure and dynastic legitimacy, because severe pressure now drains legitimacy directly.
3. World pressure and faith-war escalation, because active holy war contributes to dominance score and pressure now feeds holy-war AI timing.
4. World pressure and covert pressure, because Stonehelm now accelerates sabotage, espionage, and assassination against the world-pressure leader.
5. World pressure and neutral-world behavior, because tribes now preferentially raid the dominant kingdom.
6. World pressure and command readability, because the 11-pill dashboard now exposes the pressure label, score, streak, and penalties instead of generic signal text.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Broader world pressure and late-stage escalation | DOCUMENTED | LIVE (first layer, with realm-cycle escalation, frontier-loyalty and legitimacy consequence, AI tempo compression, tribal retargeting, dashboard legibility, and snapshot continuity) |

## Session 61 Next Action

Return to covert-defense follow-up, specifically interception-network consequence and retaliation. Session 60 now makes the world-pressure leader a more frequent covert target, so the next highest-leverage adjacent layer is to let successful counter-intelligence interceptions generate actionable retaliatory knowledge instead of ending only as passive defense.
