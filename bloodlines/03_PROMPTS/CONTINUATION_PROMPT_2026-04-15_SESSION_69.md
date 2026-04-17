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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_68.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 67: the player can launch dossier-backed sabotage from the rival-court panel using live dossier targeting, shared sabotage terms, real dynasty operations, and restore-safe dossier provenance.
- Session 68: `Convergence` world pressure now drives sharper rival-kingdom military, covert, and faith tempo plus harsher tribal raid cadence, exposes those caps in the world pill, and survives restore.

Next required action:
Implement Ironmark’s `axeman` as a real house-identity lane.

Requirements for that work:
- It must be a real runtime unit, not dead data.
- It must remain Ironmark-only through simulation-side gating.
- It must touch at least two already-live systems, including Ironmark blood-production pressure.
- It must be legible through the existing command surface or another already-live UI surface.
- Extend validation and runtime tests accordingly.
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
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_69.md
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
