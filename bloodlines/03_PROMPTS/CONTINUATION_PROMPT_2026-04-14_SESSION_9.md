# BLOODLINES — CONTINUATION PROMPT FOR NEXT SESSION

Use this prompt to continue Bloodlines from the end of 2026-04-14 Session 8.

---

You are continuing active development on BLOODLINES from the canonical root:

- `D:\ProjectsHome\Bloodlines`

This is not a planning-only session and not a scope-reduction session. Bloodlines continues to be built toward its full intended scale, with the browser/spec lane preserved as the working gameplay specification and the Unity lane preserved as the production lane once the Unity version decision is resolved.

## First Read Order

Read these files in order before meaningful changes:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
5. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
6. `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json`
7. `D:\ProjectsHome\Bloodlines\01_CANON\BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
8. `D:\ProjectsHome\Bloodlines\01_CANON\DEFENSIVE_FORTIFICATION_DOCTRINE.md`
9. `D:\ProjectsHome\Bloodlines\04_SYSTEMS\SIEGE_SYSTEM.md`
10. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
11. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`
12. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_6.md`
13. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_7.md`
14. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_8.md`
15. `D:\ProjectsHome\Bloodlines\tasks\todo.md`

## Current Live Browser/Spec State (End Of Session 8)

- dynasty cascade, captive ledger, rescue/ransom/captor-demand operations: live
- faith commitment, conviction ledger, covenant warded keeps: live
- territory capture, stabilization, governor specialization, governor rotation: live
- stone and iron economy with smelting chain: live
- fortification class (`wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1`) and settlement-tier advancement: live
- fortified reserve cycling, triage healing, keep-presence readout: live
- siege production infrastructure: live
- siege engine roster: `ram`, `siege_tower`, `trebuchet`
- engineer corps and sustainment chain: `siege_engineer`, `supply_wagon`, `supply_camp`
- unsupplied siege penalties and line interdiction through broken logistics: live
- Stonehelm AI now prepares siege, adds logistics, queues specialists, and delays unsupplied assault: live
- tests green

## Unity Lane Status

Do not begin ECS runtime implementation until the Unity version alignment decision is explicitly resolved.

Current blocker:

- approved target is Unity `6000.3.13f1` (6.3 LTS)
- current canonical Unity project still targets `6000.4.2f1`

Recommended path remains: downgrade `unity/` to 6.3 LTS, then open the project, sync JSON content, and begin ECS foundation work.

## Next Correct Browser/Spec Wave

The next strongest build direction is:

1. commander keep-presence expansion beyond the current reserve and fortification modifiers
2. sabotage and breach-enabling covert operations
3. next siege-support classes:
   - `ballista`
   - `mantlet`
4. longer siege AI logic:
   - supply protection
   - repeated assault windows
   - response after repulse
   - adaptation under supply disruption
5. full 11-state realm-condition dashboard

Do not regress the now-live sustainment chain. Build on it.

## Preservation Rules

- delete nothing without explicit authorization
- keep `D:\ProjectsHome\Bloodlines` as the only canonical Bloodlines session root
- keep the browser/spec lane alive in parallel with Unity
- update continuity at the end of the session:
  - `CURRENT_PROJECT_STATE.md`
  - `NEXT_SESSION_HANDOFF.md`
  - `continuity/PROJECT_STATE.json`
  - `tasks/todo.md`
  - `00_ADMIN/CHANGE_LOG.md`
- write the next addendum report and next continuation prompt

## Verification

Run:

```powershell
Set-Location D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js
```

## Directive

Continue building Bloodlines upward at full intended scale. Do not reduce the vision, do not flatten the dynastic or siege systems, and do not let Unity drift further behind the browser/spec lane once the version decision is resolved.

Begin from the current live sustainment truth and push the next major siege-and-command layer forward.
