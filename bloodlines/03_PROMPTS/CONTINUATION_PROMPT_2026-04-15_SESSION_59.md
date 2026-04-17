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
15. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md through SESSION_58.md
16. tasks/todo.md
17. HANDOFF.md

Current live state to preserve:
- Session 46: food and water surplus reinforces owned-march loyalty each realm cycle when cap pressure is not active.
- Session 47: defected minor house claims a real stabilized border march with food and influence trickle.
- Session 48: minor-house AI defends the claimed march, retakes it if seized, regroups after pressure clears, and surfaces posture in the dynasty panel.
- Session 49: restoreStateSnapshot rebuilds live prefix-based counters so post-restore dynamic ids do not collide.
- Session 50: held breakaway marches levy local militia, swordsmen, and archers by spending food and influence, lowering claim loyalty, and growing a real retinue that survives save and restore.
- Session 51: mixed-bloodline children now feed live lesser-house instability through marriage-aware and hostility-aware loyalty drift, and that pressure is legible in the dynasty panel.
- Session 52: AI marriage proposal and acceptance now weighs real covenant and doctrine compatibility.
- Session 53: real bloodline death dissolves marriages, applies legitimacy and conviction consequence, and halts gestation.
- Session 54: owned marches, settlements, wells, supply camps, and linked wagons sustain field armies; dehydration slows and weakens armies and surfaces in logistics.
- Session 55: espionage and assassination are live, with rival-court intelligence reports, bloodline-targeted killing, legitimacy and succession consequence, dynasty-panel legibility, AI reciprocity, and restore continuity.
- Session 56: missionary pressure and holy war declaration are live, with active faith operations, hostility consequence, territorial or legitimacy pressure, AI faith escalation, and restore continuity.
- Session 57: marriage governance now runs through live household authority, requires an offering envoy, applies regency legitimacy strain, surfaces in the dynasty panel, and survives restore.
- Session 58: prolonged critical dehydration now causes real field-water attrition and eventual desertion risk, commander presence buffers that collapse, Stonehelm recoils when its line begins to break, logistics legibility exposes the new state, and restore preserves the collapse timeline.

Next required action:
Implement covert counter-intelligence and bloodline-targeting defense as the next live covert layer.

Requirements for that work:
- It must affect live espionage or assassination outcomes in simulation.
- It must interact with at least two already-live systems.
- It must be legible through an existing UI or message surface when relevant.
- AI must recognize or use the new defensive covert layer where practical.
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
- docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_59.md
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
