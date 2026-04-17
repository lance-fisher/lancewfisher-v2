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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_54.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 46: food and water surplus reinforces owned-march loyalty each realm cycle when cap pressure is not active.
- Session 47: defected minor house claims a real stabilized border march with food and influence trickle.
- Session 48: minor-house AI defends the claimed march, retakes it if seized, regroups after pressure clears, and surfaces posture in the dynasty panel.
- Session 49: restoreStateSnapshot rebuilds live prefix-based counters so post-restore dynamic ids do not collide.
- Session 50: held breakaway marches levy local militia, swordsmen, and archers by spending food and influence, lowering claim loyalty, and growing a real retinue that survives save and restore.
- Session 51: mixed-bloodline children now feed live lesser-house instability through marriage-aware and hostility-aware loyalty drift, and that pressure is legible in the dynasty panel.
- Session 52: AI marriage proposal and acceptance now read covenant and doctrine compatibility from live faith state.
- Session 53: bloodline death now dissolves marriages, applies legitimacy loss and oathkeeping mourning, halts gestation, and surfaces death-ended unions in the dynasty panel.
- Session 54: wells, owned marches, settlements, supply camps, and camp-linked wagons now sustain field armies; dehydration slows armies, weakens attacks, surfaces in logistics legibility, survives restore, and can delay Stonehelm assaults.

Next required action:
Implement assassination and espionage as additional covert operation types inside `dynasty.operations`.

Requirements for that work:
- It must affect runtime state, not only data or UI labels.
- It must interact with at least two already-live systems.
- It must touch bloodline members, command or legitimacy stakes, and AI or world response where feasible.
- It must be legible through an existing UI or message surface when relevant.
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
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_55.md
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
