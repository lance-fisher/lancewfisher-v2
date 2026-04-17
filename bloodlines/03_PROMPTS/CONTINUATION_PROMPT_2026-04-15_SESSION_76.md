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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_75.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 71: world pressure now exposes a live source breakdown, the world pill surfaces the leading source, and tribes hard-prioritize off-home marches when continental overextension is the dominant source.
- Session 72: rival kingdoms now also read that source breakdown for territorial pressure and redirect onto off-home marches when overextension is the dominant source.
- Session 73: enemy missionary and holy-war timing now react more sharply when active holy war is the leading source of world pressure.
- Session 74: enemy counter-intelligence and sabotage timing now react more sharply when hostile operations are the leading source of world pressure.
- Session 75: enemy punitive territorial targeting and bloodline assassination timing now react more sharply when dark extremes are the leading source of world pressure.

Next required action:
Implement source-aware captive backlash when held captives are the leading source of world pressure.

Requirements for that work:
- It must affect world or AI reaction in simulation.
- It must read the real world-pressure source breakdown, not only score or level.
- It must interact with at least two already-live systems.
- It must be legible through an existing UI or message surface.
- Extend runtime tests accordingly.
- Preserve snapshot continuity where relevant.
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

Then verify play.html as far as the environment allows.

Before ending, update additively:
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_76.md
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
