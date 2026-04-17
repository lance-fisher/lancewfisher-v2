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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_77.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 63: Hartvale is prototype-playable and Verdant Warden is live behind house-gated training.
- Session 69: Ironmark Axeman is live with blood-production burden.
- Session 70: Ironmark AI now recruits Axeman through the same house-gated lane with blood-load-aware fallback.
- Session 71: world pressure now exposes a live source breakdown and tribes hard-prioritize off-home marches when continental overextension is dominant.
- Session 72: rival kingdoms now also redirect territorial pressure onto off-home marches when overextension is dominant.
- Session 73: enemy missionary and holy-war timing now react more sharply when active holy war is the leading world-pressure source.
- Session 74: enemy counter-intelligence and sabotage timing now react more sharply when hostile operations are the leading world-pressure source.
- Session 75: enemy punitive territorial targeting and bloodline assassination timing now react more sharply when dark extremes are the leading world-pressure source.
- Session 76: enemy captive-recovery timing now reacts more sharply when held captives are the leading world-pressure source, and Stonehelm launches live rescue or ransom backlash.
- Session 77: tribes and Stonehelm now punish broad territorial expansion directly by driving onto the weakest stretched marches, and the world pill now exposes territorial breadth contribution explicitly.

Next required action:
Deepen Hartvale's Verdant Warden into a real house-support system by making it provide live settlement-defense and local loyalty support in simulation.

Requirements for that work:
- It must affect runtime state, not only unit stats or labels.
- It must touch at least two already-live systems, including settlement defense, loyalty, population, or governance pressure where canon supports it.
- It must be legible through an existing UI or message surface.
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

Then verify play.html as far as the environment allows.

Before ending, update additively:
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_78.md
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
