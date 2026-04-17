# Bloodlines Continuation Prompt, Session 48

Operate inside the canonical root only:

- `D:\ProjectsHome\Bloodlines`

This is Bloodlines, a grand dynastic civilizational RTS. No scope reduction is acceptable. Preserve canon, preserve prior live systems, and continue converting canon into runtime reality.

Before editing, read in order:

1. `AGENTS.md`
2. `README.md`
3. `CLAUDE.md`
4. `MASTER_PROJECT_INDEX.md`
5. `MASTER_BLOODLINES_CONTEXT.md`
6. `CURRENT_PROJECT_STATE.md`
7. `NEXT_SESSION_HANDOFF.md`
8. `SOURCE_PROVENANCE_MAP.md`
9. `continuity/PROJECT_STATE.json`
10. `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
11. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md`
12. `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md`
13. `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md`
14. `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md`
15. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_43.md`
16. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_44.md`
17. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_45.md`
18. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md`
19. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_47.md`
20. `tasks/todo.md`
21. `HANDOFF.md`

Current live state to preserve:

- Session 44: defected lesser house spawns a real `minor_house` faction entry with save/restore persistence.
- Session 45: defected minor house spawns a founder militia unit near the parent command hall, with commander attachment and consistent population.
- Session 46: the 90-second realm cycle now reinforces owned march loyalty under strong food and water surplus when cap pressure is not active.
- Session 47: the defected minor house now claims a real stabilized border march with food and influence trickle, visible in the dynasty panel and preserved through snapshot restore.

Highest-leverage next action:

Implement **minor-house AI activation and territorial defense** as the first operational layer for breakaway factions.

Concrete target:

1. Read `src/game/core/simulation.js`, `src/game/core/ai.js`, `src/game/main.js`, and the Session 44 through 47 reports.
2. Make the spawned `minor_house` capable of at least one real autonomous behavior:
   - defend its claimed march,
   - react to nearby hostile units,
   - preserve or regroup around its founder unit,
   - remain stable under existing save/restore.
3. Keep it additive. Do not replace existing kingdom AI. Do not flatten the lesser-house arc into a generic rebel token.
4. Surface the new behavior through existing legibility surfaces if possible, for example dynasty panel wording, debug overlay, message log, or control-point state text.
5. Extend `tests/runtime-bridge.mjs` and `tests/data-validation.mjs` if schema changes.

Non-negotiables:

- No scope reduction.
- No decorative dead UI.
- Every new live mechanic must touch at least two already-live systems.
- Browser simulation remains the authoritative live lane.
- Keep tests green.

Required verification before close:

```powershell
cd D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js
python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines
```

Then verify `http://localhost:8057/play.html` as far as the environment allows.

Before ending, update additively:

- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_48.md`
- `00_ADMIN/CHANGE_LOG.md`
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `continuity/PROJECT_STATE.json`
- `tasks/todo.md`
- `MASTER_BLOODLINES_CONTEXT.md`
- `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md`
- `HANDOFF.md`

Preserve everything. Advance the full vision.
