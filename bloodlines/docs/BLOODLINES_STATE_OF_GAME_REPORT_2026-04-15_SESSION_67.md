# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 67
Author: Codex

## Scope

Counter-intelligence dossiers no longer stop at AI-side retaliation advice. The player can now use a live hostile-court dossier to launch real sabotage from the rival-court panel using the same dossier profile, the same sabotage-term math, the same operation pipeline, and the same restore-safe provenance already used by AI retaliation.

## Changes Landed

### Player-facing dossier sabotage action (`src/game/main.js`)

- The rival-court panel now resolves dossier-backed sabotage directly from live `counter_intelligence` reports.
- When the player holds a valid dossier on a rival court, the existing covert action row now surfaces a real `Launch Dossier Sabotage` action instead of only recommendation text.
- The action reads:
  - live dossier target selection,
  - live sabotage subtype,
  - live cost,
  - live duration,
  - live projected success chance,
  - live dossier support bonus.
- The action is disabled when real sabotage terms are unavailable, so the dynasty panel remains honest and introduces no decorative dead control.

### Shared sabotage-term reuse (`src/game/core/simulation.js`)

- Added `getSabotageOperationTerms(...)` as a public wrapper over the existing sabotage-term resolution seam.
- This keeps player UI, AI launch logic, and restore-safe sabotage provenance on the same runtime path instead of duplicating sabotage calculations in the panel layer.

### Runtime proof (`tests/runtime-bridge.mjs`)

- Added Session 67 coverage proving:
  - player counter-intelligence can intercept hostile espionage into a live dossier on the rival court,
  - the dossier resolves into a real sabotage target and subtype,
  - dossier-backed sabotage terms preserve support bonus and projected success,
  - the player can launch dossier-backed sabotage as a real active dynasty operation,
  - the launched operation preserves `intelligenceReportId` and `intelligenceSupportBonus`,
  - restore preserves the player-launched dossier sabotage operation and its dossier provenance.

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
  - served markup still contains `game-shell`, `resource-bar`, `realm-condition-bar`, `dynasty-panel`, `faith-panel`, and `message-log`,
  - `data/houses.json` still exposes Hartvale as `prototypePlayable: true`.

## Canonical Interdependency Check

Session 67 connects:

1. Counter-intelligence dossiers and player decision-making, because intercepted hostile covert pressure now creates a real sabotage action for the player instead of AI-only privilege.
2. Counter-intelligence dossiers and sabotage resolution math, because the player action uses the same dossier support bonus and sabotage-term pipeline already used by retaliation logic.
3. Counter-intelligence dossiers and dynasty-panel legibility, because the rival-court panel now shows whether the dossier produces a real actionable sabotage line with real cost, duration, chance, and target.
4. Counter-intelligence dossiers and dynasty operations, because the player action creates a real `sabotage` operation with persisted dossier provenance.
5. Counter-intelligence dossiers and snapshot continuity, because the launched dossier-backed sabotage survives export and restore without losing dossier linkage.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Player-facing dossier-backed sabotage actionability | PARTIAL | LIVE (first layer, through rival-court actionability, shared sabotage terms, live operation launch, dynasty-panel legibility, and restore continuity) |

## Session 68 Next Action

Deepen world pressure at the `Convergence` tier. The next canonical layer is to make severe dominance drive sharper rival-kingdom covert and faith tempo plus harsher tribal raid cadence through a shared convergence profile, expose that pressure in the world pill, and preserve it through restore.
