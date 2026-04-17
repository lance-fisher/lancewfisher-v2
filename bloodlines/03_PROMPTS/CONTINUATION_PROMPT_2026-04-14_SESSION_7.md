# BLOODLINES — CONTINUATION PROMPT FOR NEXT SESSION

You are continuing active development on Bloodlines at `D:\ProjectsHome\Bloodlines`.

This is a live local development workspace, not a planning-only session and not a scope-reduction session. Bloodlines remains governed by the full canonical doctrine and must continue upward toward its intended large-scale RTS form.

## Read First

Read, in order:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
5. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
6. `D:\ProjectsHome\Bloodlines\ENVIRONMENT_REPORT_2026-04-14.md`
7. `D:\ProjectsHome\Bloodlines\01_CANON\BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
8. `D:\ProjectsHome\Bloodlines\18_EXPORTS\BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
9. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\SIEGE_SYSTEM.md`
10. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
11. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`
12. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_6.md`
13. `D:\ProjectsHome\Bloodlines\tasks\todo.md`
14. `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json`

## Current Live State

### Browser/spec lane

Live:

- dynasty consequence cascade
- commander aura
- territory capture and stabilization
- conviction ledger
- covenant exposure and doctrine commitment
- stone and iron economy with smelting
- fortification class (`wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1`)
- settlement class and fortification tier advancement
- assault cohesion strain
- canonical 90-second realm cycle
- fortified reserve cycling
- governor specialization rotation
- faith-integrated keep wards
- dedicated `siege_workshop`
- live `ram`, `siege_tower`, and `trebuchet`
- Siege Tower allied assault support
- Stonehelm AI siege refusal, siege infrastructure buildout, workshop queueing, and siege-line staging

Validation currently green:

- `node tests/data-validation.mjs`
- `node tests/runtime-bridge.mjs`
- `node --check src/game/main.js`
- `node --check src/game/core/simulation.js`
- `node --check src/game/core/renderer.js`
- `node --check src/game/core/ai.js`
- `node --check src/game/core/data-loader.js`
- `node --check src/game/core/utils.js`

### Unity lane

Still blocked on the Unity version alignment decision.

- Approved target: Unity `6000.3.13f1` (6.3 LTS)
- Current project target: Unity `6000.4.2f1`
- Do not write ECS runtime code until the version lock is resolved

## Immediate Next Work

### First

If the user has still not answered the Unity version question, ask for a direct decision before touching the ECS lane.

### Then continue the browser/spec lane with the next highest-value systems:

1. captured-member rescue and ransom operations
2. engineer specialists for siege operation, repair, and earthworks
3. siege supply continuity and line interdiction
4. commander keep-presence bonuses beyond the current reserve layer
5. full 11-state realm-condition dashboard
6. next siege-support classes such as `ballista` and `mantlet`

## Preservation Rules

- Additive changes only
- Delete nothing unless explicitly authorized
- Keep the browser reference lane active as the working spec
- Do not open `D:\ProjectsHome\Bloodlines\Bloodlines` as the primary Unity project
- Update continuity files at the end of the work block:
  - `CURRENT_PROJECT_STATE.md`
  - `NEXT_SESSION_HANDOFF.md`
  - `continuity/PROJECT_STATE.json`
  - `tasks/todo.md`
  - `00_ADMIN/CHANGE_LOG.md`

## End-Of-Session Requirement

If you complete meaningful work, write the next continuation prompt in `03_PROMPTS/` with the next session number and keep tests green.
