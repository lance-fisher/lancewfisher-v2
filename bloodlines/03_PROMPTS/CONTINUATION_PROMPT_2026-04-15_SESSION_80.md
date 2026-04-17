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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_79.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 69: Ironmark Axeman is live with blood-production burden.
- Session 70: Ironmark AI now recruits Axeman through the same house-gated lane with blood-load-aware fallback.
- Session 71 through 77: world pressure now exposes a live source breakdown and drives source-aware tribal, rival, faith, covert, dark-extremes, captive, and stretched-march backlash.
- Session 78: Hartvale Verdant Warden now provides live settlement-defense and local-loyalty support.
- Session 79: Scout Rider is now live as a real stage-2 cavalry unit, trains from Stable, launches infrastructure raids, cuts live logistics and water support, shocks nearby hostile loyalty, surfaces raid pressure in UI, and survives restore.

Next required action:
Extend Scout Rider from infrastructure raids into direct worker and resource-node harassment, then add the first honest counter-raid response.

Requirements for that work:
- It must affect live gathering throughput or worker safety, not only building disable windows.
- It must interact with at least two already-live systems, including logistics, water sustainment, territorial pressure, or AI response where canon supports it.
- It must add a real counter-raid or local defensive reaction instead of leaving harassment as a one-sided player trick.
- It must be legible through an existing UI, overlay, message, or dashboard surface.
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
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_80.md
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
