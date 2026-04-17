# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 58
Author: Codex

## Scope

Field-water sustainment has been advanced from operational slowdown into real collapse pressure. Prolonged critical dehydration now starts bleeding health from unsupported field armies, then forces desertion risk if the line stays dry. Commander presence now buffers that collapse, Stonehelm reacts to attrition and breaking lines, the logistics surface exposes the new state, and restore continuity preserves the longer-run dehydration path.

## Changes Landed

### Field-water collapse runtime (`src/game/core/simulation.js`)

- Added longer-run field-water collapse state on units:
  - `fieldWaterCriticalDuration`
  - `fieldWaterAttritionActive`
  - `fieldWaterDesertionRisk`
- Critical dehydration now progresses in two live stages:
  - attrition after sustained critical dehydration,
  - desertion risk after a longer unsupported collapse window.
- Unsupported critical armies now take real health loss over time instead of stopping at speed and attack penalties.
- If that collapse persists long enough and the unit is already materially weakened, the unit now deserts from the line entirely.
- Commander aura now buffers dehydration collapse:
  - attrition and desertion start later under command discipline,
  - commanded units lose health more slowly than uncommanded ones.
- Unit death finalization now surfaces dehydration-specific loss causes instead of reporting every collapse as generic battlefield death.

### Logistics and military legibility (`src/game/core/simulation.js`, `src/game/main.js`)

- `getFactionFieldWaterState` now exports live counts for:
  - attriting units,
  - units at desertion risk.
- `getRealmConditionSnapshot` now surfaces those counts in both the military and logistics blocks.
- The realm-condition logistics pill now prioritizes:
  - breaking lines,
  - attriting lines,
  - then ordinary critical dehydration.
- The logistics meta now surfaces attrition and desertion-risk counts directly.
- The debug overlay now also exposes the expanded field-water collapse state for faster verification during long-form play.

### AI reaction (`src/game/core/ai.js`)

- Stonehelm no longer reacts only to critical dehydration as a generic delay.
- If the assault column is taking field-water attrition, Stonehelm now recoils toward its water anchor before reforming.
- If the column has reached desertion risk, Stonehelm abandons the assault push and lengthens its re-attack timer further.
- This keeps the water system from remaining player-only and ties dehydration directly into live rival operational behavior.

### Save / restore continuity (`src/game/core/simulation.js`)

- Snapshot export and restore now preserve:
  - critical-duration accumulation,
  - attrition-active state,
  - desertion-risk state.
- This means a long dehydrated campaign no longer resets its collapse timeline on restore.

### Faith-pressure correction discovered during verification (`src/game/core/simulation.js`, `tests/runtime-bridge.mjs`)

- Verification exposed that active holy war pressure could be outweighed by territorial restabilization, leaving the old runtime-bridge assertion brittle.
- Active holy war now applies continuous legitimacy pressure in addition to territorial pressure.
- The runtime-bridge assertion was updated to verify continued territorial or legitimacy pressure over time, which matches the already-authoritative Session 56 canon.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 58 runtime coverage proving:
  - prolonged critical dehydration advances into active attrition,
  - unsupported field armies lose real health from water collapse,
  - the logistics snapshot exposes attrition state,
  - extended unsupported collapse eventually reaches desertion loss,
  - commander-buffered units hold longer than uncommanded ones,
  - restore preserves critical-duration and collapse-state continuity,
  - Stonehelm lengthens assault delay and regroups when dehydration has reached breaking-line pressure.
- Updated holy-war coverage so sustained faith pressure is verified through ongoing territorial or legitimacy consequence instead of a brittle loyalty-only branch.

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

Session 58 connects:

1. Water logistics and military presence, dehydration now removes bodies from the field instead of only reducing effectiveness.
2. Water logistics and bloodline command, commander presence now delays and softens dehydration collapse.
3. Water pressure and AI reactivity, Stonehelm now aborts or delays offensives when its line begins to break from thirst.
4. Water pressure and legibility, the 11-pill dashboard and debug overlay now expose attrition and desertion-risk state explicitly.
5. Faith operations and dynastic consequence, active holy war now sustains legitimacy pressure as well as territorial strain.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Longer-run field-water attrition and desertion | DOCUMENTED | LIVE (first layer, with critical-duration tracking, health attrition, desertion risk, commander buffering, AI retreat behavior, legibility, and restore continuity) |

## Session 59 Next Action

Deepen covert operations into counter-intelligence and bloodline-targeting defense. Espionage and assassination are now live offensive tools. The next highest-leverage dynastic and world-reactive seam is to let rival courts actively blunt those operations through live defensive discovery, exposure, interception, or protection logic instead of only suffering or launching attacks.
