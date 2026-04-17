# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 83
Author: Codex

## Scope

Session 82 completed the canon ingestion for match structure, time system, and multiplayer engagement doctrine. Session 83 converts that newly locked doctrine into the first live browser-lane runtime layer: readiness-gated five-stage match progression, dual-clock and stage legibility in the existing dashboard lane, stage-aware Stonehelm early-war restraint, and snapshot continuity for the new progression state.

## Changes Landed

### First live match-progression layer (`src/game/core/simulation.js`)

- Added canonical `MATCH_STAGE_DEFINITIONS` and phase labels for the five-stage match structure.
- Added live progression computation through `computeMatchProgressionState(...)`, `updateMatchProgressionState(...)`, and `getMatchProgressionSnapshot(...)`.
- Match stage now resolves from already-live runtime conditions instead of dead labels:
  - Stage 2: food surplus, water surplus, defended home seat, and founding buildout.
  - Stage 3: covenant commitment, territorial expansion, and field-army strength.
  - Stage 4: rival contact, contested border or active pressure, and sustained war pressure.
  - Stage 5: convergence pressure, true sovereignty contender, and late dynastic time.
- Stage changes now emit a live message-log announcement instead of remaining silent simulation state.

### Dual-clock and snapshot continuity (`src/game/core/simulation.js`)

- `exportStateSnapshot(...)` now serializes both `dualClock` and `matchProgression`.
- `restoreStateSnapshot(...)` now restores both structures and recomputes progression without replaying announcement spam.
- `getRealmConditionSnapshot(...)` now exposes stage id, stage number, stage label, phase, readiness, next-stage target, shortfalls, declaration count, and the full `matchProgression` block beside the existing `dualClock` block.

### Stage-aware Stonehelm restraint and escalation (`src/game/core/ai.js`)

- Stonehelm now reads live match progression before opening territorial rivalry or Scout Rider raid tempo.
- Ordinary early-game behavior is restrained:
  - territorial pressure is held back before Stage 2,
  - Scout Rider production and raid launches are held back before Stage 3.
- Existing live pressure systems still override that restraint when the simulation has already escalated for real:
  - targeted world pressure,
  - dark-extremes backlash,
  - convergence pressure,
  - holy war pressure,
  - hostile-operations backlash.
- This preserves canonical early-war tempo without breaking already-live retaliation lanes.

### Dashboard and debug legibility (`src/game/main.js`)

- The existing 11-pill realm dashboard now exposes match stage and dual-clock state through the Cycle pill instead of only cycle number.
- Cycle legibility now shows:
  - stage number,
  - phase label,
  - in-world year,
  - declaration count,
  - first next-stage shortfall.
- The realm dashboard header is now updated at render time with stage, phase, year, and next-cycle timing.
- The debug overlay now includes a dedicated match line so stage, year, and declaration context are visible during runtime inspection.

### Runtime coverage (`tests/runtime-bridge.mjs`)

- Updated dual-clock round-trip coverage so snapshot restore now asserts actual dual-clock persistence.
- Added crafted progression-state coverage for:
  - forced Stage 1 founding shortfall state,
  - forced Stage 3 commitment state,
  - forced Stage 4 rival-contact and war-pressure state,
  - snapshot restore continuity for `matchProgression`.
- Added AI gating coverage for:
  - Stage 1 raid and territory restraint,
  - Stage 3 Scout Rider raid unlock.
- Added helper state builders for progression-gated test scenarios and updated older stripped-state cavalry tests so they satisfy the new stage doctrine instead of bypassing it implicitly.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.
- `python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines` served `play.html`.
- Local fetch verification confirmed:
  - `play.html` returned `200`,
  - resource bar markup present,
  - realm-condition bar markup present,
  - dynasty panel markup present,
  - faith panel markup present.

## Canonical Interdependency Check

Session 83 connects:

1. Match structure and civilizational depth, because founding, expansion, and encounter stages now read food, water, buildout, territory, and field-army condition from live simulation state.
2. Match structure and military pressure, because Stonehelm territorial tempo and cavalry tempo now wait on stage progression unless already-live escalation systems have honestly opened the war.
3. Match structure and dual-clock continuity, because stage and phase now persist through snapshot export and restore instead of resetting to implicit fresh-match assumptions.
4. Match structure and legibility, because the existing 11-pill dashboard and debug overlay now explain where the match sits in stage, phase, year, and next-stage shortfall terms.
5. Match structure and world pressure, because targeted escalation systems can now override early-war restraint instead of being silently flattened by the new gate.

## Gap Analysis Reclassification

One world-depth row moved materially forward in Session 83:

- Five-stage match progression, declared dual-clock legibility, and first runtime stage gating moved from canon-only / documented doctrine into `PARTIAL` live implementation. Stage-specific declaration windows, event chains, Great Reckoning logic, and multiplayer warning surfaces still remain unfinished.

## Session 84 Next Action

The next highest-leverage action is no longer doctrine ingestion. That canon is now preserved and its first runtime layer is live.

Priority order:

1. Deepen the new match-progression lane into stage-specific declaration or event pressure, especially the first real Stage 4 to Stage 5 escalation hooks.
2. Implement the first live imminent-engagement warning layer from the now-ingested multiplayer and time doctrine so the locked canon begins affecting command timing directly.
3. If that lane blocks, continue deeper world-depth follow-up through multi-kingdom pressure, neutral-power stage presence, or naval-world integration.
