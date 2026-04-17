# BLOODLINES — CONTINUATION PROMPT FOR NEXT SESSION

You are continuing active development on the RTS game project BLOODLINES from the canonical root:

- `D:\ProjectsHome\Bloodlines`

This is not a scope-reduction session. Bloodlines remains under full-scale development toward its complete intended form: dynasty-heavy, faith-divergent, territory-driven, siege-serious, population-governed, identity-rich RTS at grand scale.

## Read First

Read in this order before making meaningful changes:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
5. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
6. `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json`
7. `D:\ProjectsHome\Bloodlines\01_CANON\BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
8. `D:\ProjectsHome\Bloodlines\18_EXPORTS\BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
9. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
10. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`
11. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_6.md`
12. `D:\ProjectsHome\Bloodlines\docs\BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_7.md`
13. `D:\ProjectsHome\Bloodlines\tasks\todo.md`

## Current Live Browser/Spec State

The browser reference simulation is the active working spec and currently has all of the following live:

- dynasty consequence cascade
- commander capture vs kill
- heir succession cascade
- captive ledger and fallen ledger
- negotiated ransom operations
- covert rescue operations
- captor-side ransom demand resolution
- commander aura
- territory capture and stabilization
- governor specialization and governor rotation
- conviction ledger
- covenant exposure and doctrine commitment
- stone and iron economy
- smelting chain at `iron_mine`
- fortification building class
- fortified reserve cycling
- faith-integrated keep wards
- blood-altar reserve surge
- `siege_workshop`
- `ram`, `siege_tower`, `trebuchet`
- Stonehelm AI keep-assault refusal
- Stonehelm AI siege preparation and staging
- canonical 90-second realm cycle
- 10-pill resource bar
- 6-pill realm-condition bar
- dynasty panel captivity actions and active-operation progress

Tests are green:

- `node tests/data-validation.mjs`
- `node tests/runtime-bridge.mjs`
- `node --check src/game/main.js`
- `node --check src/game/core/simulation.js`
- `node --check src/game/core/renderer.js`
- `node --check src/game/core/ai.js`
- `node --check src/game/core/data-loader.js`
- `node --check src/game/core/utils.js`

## Unity State

The Unity production lane remains structurally aligned but blocked on the version lock.

- Canonical Unity project: `D:\ProjectsHome\Bloodlines\unity`
- Approved target: Unity `6000.3.13f1`
- Current project target in files: `6000.4.2f1`

Do not start ECS runtime implementation until the version decision is explicitly resolved.

## Correct Next Direction

The next browser/spec wave should deepen the operational siege layer now that capture, recovery, fortification, and attacker preparation are all live.

Build next:

1. engineer specialists for siege operation, repair, earthworks, mining, and counter-mining
2. siege supply continuity: camps, wagons, line interdiction, attrition pressure
3. sabotage and breach-enabling operations integrated with the covert lane
4. commander keep-presence expansion beyond reserve tempo
5. full 11-state realm-condition dashboard
6. next siege-support classes such as `ballista` and `mantlet`

## Preservation Rules

- Delete nothing unless explicitly authorized.
- Keep all canon additive.
- Keep Unity untouched until the version lock is resolved.
- Preserve the browser/spec lane as the live reference implementation.
- Update continuity files at the end of the work block:
  - `CURRENT_PROJECT_STATE.md`
  - `NEXT_SESSION_HANDOFF.md`
  - `continuity/PROJECT_STATE.json`
  - `00_ADMIN/CHANGE_LOG.md`
  - `tasks/todo.md`

## Final Directive

Continue Bloodlines upward from its current truth. Do not reduce scope. Do not flatten dynastic, fortification, siege, faith, or territorial logic. Build the next operational layer with the same additive seriousness used in sessions 4 through 7.
