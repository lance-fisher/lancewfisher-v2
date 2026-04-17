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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_62.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 60: world pressure now accumulates through realm cycles from territorial breadth, off-home holdings, active holy wars, held captives, hostile operations, and dark extremes; the dominant realm now suffers frontier-loyalty and legitimacy pressure, Stonehelm compresses offensive tempo against that leader, tribes retarget raids toward it, the dashboard exposes the state, and restore preserves the pressure fields.
- Session 61: successful covert interceptions now create counter-intelligence dossiers on hostile courts, preserve source-scoped interception history, surface dossier metadata in the dynasty panel, survive restore, and let Stonehelm retaliate without reopening redundant espionage first.
- Session 62: mixed-bloodline lesser houses now carry live marital anchors that can be active, dissolved, strained, or fractured, materially changing cadet-house loyalty drift, dynasty-panel legibility, and restore continuity.

Next required action:
Implement Hartvale playable-house follow-up with house-gated unique-unit enablement.

Requirements for that work:
- It must affect real runtime, not just `prototypePlayable` data flags.
- Hartvale playability must remain honest: no dead house-select option, no inert unique-unit label.
- House-specific unit access must be gated by the actual selected house so existing houses do not gain off-canon units.
- The new work must become legible through an existing UI surface.
- Extend runtime and data validation tests accordingly.
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
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_63.md
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
