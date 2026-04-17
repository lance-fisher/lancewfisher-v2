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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_67.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 65: hostile minor houses exploit parent-realm world pressure through faster levy growth, higher local retinue cap, sharper retake cadence, dynasty-panel legibility, world-pill legibility, and restore continuity.
- Session 66: counter-intelligence dossiers drive smarter sabotage retaliation, sabotage-support carry-through, dynasty-panel legibility, and restore continuity.
- Session 67: the player can now launch dossier-backed sabotage from the rival-court panel using live dossier targeting, shared sabotage terms, real dynasty operations, and restore-safe dossier provenance.

Next required action:
Implement convergence-tier world pressure escalation so a dominant realm at live `Convergence` pressure drives sharper rival-kingdom covert or faith tempo and harsher tribal raid cadence through a shared runtime profile.

Requirements for that work:
- It must affect live simulation, not only labels.
- It must touch at least two already-live systems.
- It must make AI or world actors react differently at higher world-pressure severity than they do now.
- It must become legible through the existing world surface or another already-live UI surface.
- It must preserve snapshot continuity where relevant.
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
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_68.md
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
