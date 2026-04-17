# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 66
Author: Codex

## Scope

Counter-intelligence dossiers have been deepened from assassination-only reuse into sabotage-backed retaliation and court counterplay. Hostile courts already created source-scoped dossiers when they foiled espionage or assassination. They now use that same live dossier state to choose sharper sabotage targets, carry dossier-backed covert support into sabotage math, surface retaliatory rationale in the dynasty panel, and preserve the new state through restore.

## Changes Landed

### Dossier-backed sabotage profile (`src/game/core/simulation.js`)

- Added `getDossierBackedSabotageProfile(state, factionId, targetFactionId, reportOrId)` as the shared seam between:
  - live counter-intelligence dossiers,
  - sabotage subtype selection,
  - sabotage target selection,
  - sabotage intelligence support.
- The profile activates only when:
  - the report is a live `counter_intelligence` dossier,
  - the dossier still targets the hostile court in question,
  - the dossier has not expired,
  - the hostile court still has a valid sabotage target.
- Intercept type now changes retaliation posture:
  - intercepted `assassination` pushes retaliation toward court-seat disruption,
  - intercepted `espionage` pushes retaliation toward court-working infrastructure,
  - other dossier cases fall back to exposed logistics and governance targets.

### Sabotage math and operation provenance (`src/game/core/simulation.js`)

- `startSabotageOperation` and sabotage-term resolution now accept dossier-backed support metadata.
- Sabotage operations now preserve:
  - `intelligenceReportId`,
  - `intelligenceSupportBonus`,
  - `retaliationReason`,
  - `retaliationInterceptType`.
- Dossier-backed sabotage now gains a real offensive bonus instead of only using better target selection.
- Launch messages now state when sabotage is guided by a counter-intelligence dossier.

### Retaliatory AI behavior (`src/game/core/ai.js`)

- Stonehelm sabotage no longer uses only a generic building priority once a live dossier exists.
- With a counter-intelligence dossier on the player, Stonehelm now:
  - picks sabotage subtype from the dossier,
  - picks a dossier-specific target building,
  - passes dossier provenance and support into the launched sabotage operation,
  - continues to skip redundant espionage before retaliating.

### Legibility (`src/game/main.js`)

- Active sabotage operations now surface dossier provenance directly in operation text through:
  - intercepted operation type,
  - dossier-backed support bonus.
- Counter-intelligence dossier rows in the dynasty panel now surface:
  - recommended retaliation subtype,
  - recommended retaliation target,
  - dossier-backed support bonus.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 66 runtime coverage proving:
  - a restored counter-intelligence dossier can still drive AI sabotage retaliation,
  - dossier-backed sabotage preserves intelligence-report provenance,
  - dossier-backed sabotage carries a real support bonus,
  - intercepted espionage steers retaliation into `fire_raising` on the hostile command hall when available,
  - dossier-backed sabotage does not reopen redundant espionage first.

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

Session 66 connects:

1. Counter-intelligence dossiers and sabotage targeting, because intercepted hostile activity now changes what sabotage target and subtype the retaliating court chooses.
2. Counter-intelligence dossiers and sabotage resolution math, because dossier-backed knowledge now gives sabotage a real offensive support bonus.
3. Counter-intelligence dossiers and retaliatory AI behavior, because Stonehelm now launches smarter sabotage from dossier knowledge without reopening espionage first.
4. Counter-intelligence dossiers and dynasty-panel legibility, because retaliation recommendation and dossier support now surface in the same court-intelligence UI the player already reads.
5. Counter-intelligence dossiers and snapshot continuity, because restored dossier state still drives dossier-backed sabotage correctly after reload.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Dossier-backed sabotage retaliation and covert court-counterplay | PARTIAL | LIVE (first layer, through dossier-guided sabotage target selection, sabotage support bonus, dynasty-panel legibility, AI retaliation reuse, and restore continuity) |

## Session 67 Next Action

Make dossier-backed sabotage actionable for the player, not only the AI. The sabotage recommendation and provenance are now live. The next canonical layer is to expose a real player-side sabotage action from the rival-court panel using the same shared dossier profile and real sabotage terms so covert counterplay becomes a player decision surface instead of an AI-only privilege.
