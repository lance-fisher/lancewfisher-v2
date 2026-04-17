# Bloodlines Next Session Command Brief

Date: 2026-04-13
Audience: the next Codex or Claude session continuing Bloodlines from the new single-root layout

## Primary Command

Continue building Bloodlines from the main project root:

- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`

Do not restart discovery. Do not re-split the project across multiple roots. Pick up from the current Unity U1 scaffold and push toward full-system realization without reducing scope.

## Cold Start Reading Order

1. `README.md`
2. `docs/CONSOLIDATION_NOTE_2026-04-13_SINGLE_ROOT.md`
3. `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md`
4. `docs/DEFINITIVE_DECISIONS_REGISTER.md`
5. `docs/unity/README.md`
6. `docs/unity/MIGRATION_PLAN.md`
7. `docs/unity/DATA_PIPELINE.md`
8. `docs/unity/PHASE_PLAN.md`
9. `docs/unity/session-handoffs/2026-04-13-u1-importer-scaffold.md`

## Current Ground Truth

- The full Bloodlines archive, browser RTS prototype, and Unity project shell now live under `bloodlines/`.
- `D:\ProjectsHome\Bloodlines` is only a compatibility alias.
- `deploy\bloodlines` is only a compatibility surface and should not be treated as the source of truth.
- The browser prototype remains the executable reference simulation.
- The Unity root exists at `bloodlines/unity/`.
- U1 importer scaffold exists:
  - `unity/Assets/_Bloodlines/Code/Definitions/BloodlinesDefinitions.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`

## Immediate Objective

Execute U1 for real, then move into the first gameplay bootstrap layers.

### First tasks

1. Open `bloodlines/unity/` interactively in Unity.
2. Let Unity finish import and generate `.meta` files for the new C# sources.
3. Run `Bloodlines/Import/Sync JSON Content`.
4. Verify ScriptableObjects are created under `Assets/_Bloodlines/Data/`.
5. Write a dated U1 verification handoff.
6. If U1 is green, start U2:
   - camera shell
   - scene/bootstrap
   - placeholder rendering

## Build Direction

The next session should not stop at importer success if there is time left. After U1 verification, push immediately into:

- root gameplay scene structure
- RTS camera controller
- `_Bloodlines` scene/layout cleanup
- first placeholder render pass
- any supporting definition refinements needed to make U2 practical

## Non-Negotiables

- No scope reduction.
- Browser prototype remains the reference simulation.
- JSON in `bloodlines/data/` remains the balance source of truth.
- Keep the work additive and production-minded.
- Do not treat absent systems as reasons to simplify the game.
- Continue from `bloodlines/`, not from compatibility paths.

## If There Is Extra Time

After U2 starts, the highest-value next moves are:

1. build the first bootstrap scene
2. wire map dimensions from `ironmark-frontier.json`
3. stand up selection-ready unit placeholders
4. prepare for worker economy parity

## Deliverable Standard For The Next Session

The next session should leave behind:

- code changes in the Unity root
- a dated Unity handoff in `docs/unity/session-handoffs/`
- explicit verification notes
- a clear statement of what is green, what is red, and the exact next step
