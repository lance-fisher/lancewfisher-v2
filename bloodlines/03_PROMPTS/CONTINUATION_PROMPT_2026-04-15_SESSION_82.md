Operate only in D:\ProjectsHome\Bloodlines.

Bloodlines is a grand dynastic civilizational RTS. No scope reduction, no decorative dead UI, no canon compression. Preserve all live systems and continue converting canon into runtime reality.

Before editing, read in order:
1. AGENTS.md
2. README.md
3. CLAUDE.md
4. MASTER_PROJECT_INDEX.md
5. MASTER_BLOODLINES_CONTEXT.md
6. CURRENT_PROJECT_STATE.md
7. NEXT_SESSION_HANDOFF.md
8. SOURCE_PROVENANCE_MAP.md
9. continuity/PROJECT_STATE.json
10. 01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md
11. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md
12. docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md
13. docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md
14. docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_81.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 79: Scout Rider is now live as a real stage-2 cavalry unit, trains from Stable, launches infrastructure raids, cuts live logistics and water support, shocks nearby hostile loyalty, surfaces raid pressure in UI, and survives restore.
- Session 80: Scout Rider now directly harasses worked resource seams, routes workers to refuge, depresses hostile march loyalty, exposes harried seams and routed workers in the logistics dashboard, triggers Stonehelm local counter-raids, and preserves the new harassment state through restore.
- Session 81: Scout Rider now intercepts moving hostile `supply_wagon`, strips convoy stores, forces convoy retreat, cuts already-live siege and field-water sustainment, shocks nearby hostile march loyalty, exposes convoy cuts in the logistics pill, triggers Stonehelm convoy targeting plus local counter-screen response, and preserves convoy-interdiction state through restore.

Next required action:
Implement convoy escort discipline and post-interdiction reconsolidation around the now-live moving-logistics interception seam.

Requirements for that work:
- It must affect AI escort or convoy-defense behavior in live simulation.
- It must make recovering convoys or escorts matter to assault timing, logistics readiness, or both.
- It must interact with at least two already-live systems.
- It must be legible through an existing UI or message surface when relevant.
- It must preserve or extend restore continuity where relevant.
- Extend runtime tests accordingly.
- Browser simulation remains the authoritative implementation lane.

Before closing, run:
- node tests/data-validation.mjs
- node tests/runtime-bridge.mjs
- node --check src/game/main.js
- node --check src/game/core/simulation.js
- node --check src/game/core/renderer.js
- node --check src/game/core/ai.js
- node --check src/game/core/data-loader.js
- node --check src/game/core/utils.js
- python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines

Then verify `play.html` as far as the environment allows.

Before ending, update additively:
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_82.md
- 00_ADMIN/CHANGE_LOG.md
- CURRENT_PROJECT_STATE.md
- NEXT_SESSION_HANDOFF.md
- continuity/PROJECT_STATE.json
- tasks/todo.md
- MASTER_BLOODLINES_CONTEXT.md
- docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md
- MASTER_PROJECT_INDEX.md
- SOURCE_PROVENANCE_MAP.md
- HANDOFF.md
