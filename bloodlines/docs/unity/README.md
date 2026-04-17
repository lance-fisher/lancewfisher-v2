# Bloodlines Unity Continuation Pack

This tracked `docs/unity/` folder exists so Bloodlines Unity migration context is not trapped inside the ignored `deploy/bloodlines/` tree.

Canonical runtime root:
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines\`

Tracked continuity mirror:
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines\docs\unity\`

Current situation as of 2026-04-13:
- The browser RTS prototype in `deploy/bloodlines/play.html` plus `src/game/` remains the reference simulation.
- A Unity migration direction was settled in the 2026-04-13 Claude Code session.
- Unity 6 is installed locally.
- No `deploy/bloodlines/unity/` project exists yet.
- The canonical deploy tree contains `docs/unity/README.md` and `docs/unity/MIGRATION_PLAN.md`, but not the deeper reference files that plan points to.

This folder fills that gap so the next session can start from a complete migration package instead of re-discovering the architecture.

## Reading Order

1. `..\BLOODLINES_CURRENT_STATE_ANALYSIS.md`
2. `..\DEFINITIVE_DECISIONS_REGISTER.md`
3. `MIGRATION_PLAN.md`
4. `COMPONENT_MAP.md`
5. `SYSTEM_MAP.md`
6. `DATA_PIPELINE.md`
7. `PHASE_PLAN.md`
8. `session-handoffs\2026-04-13-unity-continuation.md`
9. `session-handoffs\2026-04-13-unity-bootstrap.md`
10. `session-handoffs\2026-04-13-u1-importer-scaffold.md`

## Files

- `MIGRATION_PLAN.md`: tracked mirror of the current Unity migration blueprint, plus clarified blockers and execution assumptions.
- `COMPONENT_MAP.md`: browser state to DOTS component mapping.
- `SYSTEM_MAP.md`: browser update functions to ECS systems, order, and dependencies.
- `DATA_PIPELINE.md`: JSON to ScriptableObject to entity import design.
- `PHASE_PLAN.md`: milestone sequence and concrete acceptance gates.
- `session-handoffs\`: dated continuation notes.
- `phase-reports\`: reserved for completed phase verification reports.

## Governance

- Additive only. Extend with dated appendices or new files.
- Do not treat this tracked mirror as the canonical runtime root.
- When work resumes in `deploy/bloodlines/`, keep these docs mirrored so git-visible continuity stays intact.
