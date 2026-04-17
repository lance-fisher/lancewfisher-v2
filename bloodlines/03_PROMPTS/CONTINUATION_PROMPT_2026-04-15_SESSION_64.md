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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_63.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 61: successful covert interceptions now create counter-intelligence dossiers on hostile courts, preserve source-scoped interception history, surface dossier metadata in the dynasty panel, survive restore, and let Stonehelm retaliate without reopening redundant espionage first.
- Session 62: mixed-bloodline lesser houses now carry live marital anchors that can be active, dissolved, strained, or fractured, materially changing cadet-house loyalty drift, dynasty-panel legibility, and restore continuity.
- Session 63: Hartvale is now prototype-playable through the existing house-select seam, Verdant Warden is live behind simulation-side house gating, the command panel only surfaces units the current house can truly queue, and off-house unique-unit attempts fail explicitly.

Next required action:
Implement deeper world-pressure consequence through internal dynastic destabilization.

Requirements for that work:
- It must affect real runtime, not only dashboard labels.
- It must connect world pressure to at least two already-live systems.
- One of those systems must be lesser-house loyalty or defection pressure.
- It must become legible through an existing UI surface.
- It must survive save and restore.
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
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_64.md
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
