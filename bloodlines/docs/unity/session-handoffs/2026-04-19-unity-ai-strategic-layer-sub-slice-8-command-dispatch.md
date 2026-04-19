# Unity AI Strategic Layer -- Sub-Slice 8: Worker And Territory Command Dispatch

## Slice Metadata

- Date: 2026-04-19
- Branch: `codex/unity-ai-command-dispatch`
- Lane: `ai-strategic-layer`
- Session Identifier: `codex-ai-command-dispatch-2026-04-19`
- Contract Revision At Close: 22

## Goal

Port the two ai.js command-dispatch seams that were still missing from the Unity ECS
strategic layer:

1. The idle-worker `issueGatherCommand` call at the end of the gather-selection loop
   (`src/game/core/ai.js` ~1243-1260).
2. The territory-expansion unit dispatch block that fires when `territoryTimer <= 0`
   and rivalry is unlocked (`src/game/core/ai.js` ~1575-1600).

The AI timers and target selection already existed in Unity. This slice makes those
decisions issue real unit orders.

## Work Completed

### New file: `unity/Assets/_Bloodlines/Code/Components/WorkerGatherOrderComponent.cs`

Small worker-order payload:
- `TargetNode` (`Entity`)
- `ResourceId` (`FixedString32Bytes`)

This lets the AI strategic layer mark a worker as dispatched to a specific node without
overloading the gather-state component itself.

### New file: `unity/Assets/_Bloodlines/Code/AI/AIWorkerCommandSystem.cs`

ISystem with:
- `[UpdateInGroup(typeof(SimulationSystemGroup))]`
- `[UpdateAfter(typeof(AICovertOpsSystem))]`
- `[UpdateBefore(typeof(WorkerGatherSystem))]`

Behavior:
- queries workers with `WorkerGatherComponent`
- when `Phase == Seeking` and `AssignedNode != Entity.Null`, issues
  `WorkerGatherOrderComponent`
- adds or refreshes `MoveCommandComponent` toward the assigned resource node
- flips `Phase` to `WorkerGatherPhase.Gathering` so the same worker is not
  re-dispatched every frame

Implementation detail:
- uses `EntityQuery` snapshots rather than mutating entities inside a `SystemAPI.Query`
  enumeration, so structural changes stay valid under ECS rules

### New file: `unity/Assets/_Bloodlines/Code/AI/AITerritoryDispatchSystem.cs`

ISystem with:
- `[UpdateInGroup(typeof(SimulationSystemGroup))]`
- `[UpdateAfter(typeof(AICovertOpsSystem))]`

Behavior:
- reads faction `AIStrategyComponent`
- when `TerritoryTimer <= 0f`, `RivalryUnlocked == true`, and
  `ExpansionTargetCpId` is set, resolves the target control-point position
- iterates the faction's live combat units using `CombatStatsComponent`
- excludes workers and siege roles (`Worker`, `SiegeEngine`, `SiegeSupport`)
- issues or refreshes `MoveCommandComponent` toward the control point
- increments `ExpansionOrdersIssued` and `TerritoryCommandsIssued`
- resets `TerritoryTimer` to the canonical 12 second default

Design note:
- the current Unity runtime uses `MoveCommandComponent`, not a separate
  `MoveOrderComponent`, so the dispatch system writes the existing movement payload

### New file: `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAICommandDispatchSmokeValidation.cs`

Four-phase smoke validator using the standard `SetupSimGroup` helper pattern:
- Phase 1: worker in `Seeking` with `AssignedNode` set -> after update,
  `Phase == Gathering` and `WorkerGatherOrderComponent` exists
- Phase 2: worker already in `Gathering` -> no re-dispatch, order payload unchanged
- Phase 3: `TerritoryTimer <= 0`, rivalry unlocked, expansion target set ->
  combat units receive active `MoveCommandComponent`
- Phase 4: `TerritoryTimer > 0` -> no territory dispatch

Artifact:
- `../artifacts/unity-ai-command-dispatch-smoke.log`

Marker:
- `BLOODLINES_AI_COMMAND_DISPATCH_SMOKE PASS`

### New file: `scripts/Invoke-BloodlinesUnityAICommandDispatchSmokeValidation.ps1`

Canonical-root runner for the new smoke validation:
- execute method:
  `Bloodlines.EditorTools.BloodlinesAICommandDispatchSmokeValidation.RunBatchAICommandDispatchSmokeValidation`
- artifact:
  `D:\ProjectsHome\Bloodlines\artifacts\unity-ai-command-dispatch-smoke.log`

### Modified file: `unity/Assets/_Bloodlines/Code/AI/EnemyAIStrategySystem.cs`

Territory expansion is now decision-only again:
- keeps picking and storing `ExpansionTargetCpId`
- stops issuing direct movement commands itself

That matches the browser split more cleanly and makes `AITerritoryDispatchSystem` the
single territory-order issuance seam.

### Modified file: `unity/Assets/_Bloodlines/Code/Systems/WorkerGatherSystem.cs`

Gathering now requires arrival:
- if the worker is outside `GatherRadius`, the system keeps an active move command toward
  the node and returns early
- harvesting only starts after the worker reaches the node radius

Without this change, immediately promoting AI-dispatched workers from `Seeking` to
`Gathering` would have allowed off-node harvesting.

## Verification Results

All 10 validation gates green in the clean `D:\BLAICD\bloodlines` worktree:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS, 0 errors
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS, 0 errors
3. Bootstrap runtime smoke -- PASS (direct execute-method run against worktree project)
4. Combat smoke -- PASS
5. Canonical scene shells -- PASS (Bootstrap + Gameplay direct execute-method runs against worktree project)
6. Fortification smoke -- PASS (direct execute-method run against worktree project)
7. Siege smoke -- PASS
8. `node tests/data-validation.mjs` -- PASS
9. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS at revision 22

Dedicated command-dispatch smoke:
- Phase 1 PASS: `Seeking -> Gathering`, `WorkerGatherOrderComponent` present
- Phase 2 PASS: existing gather phase/order not duplicated
- Phase 3 PASS: territory dispatch adds `MoveCommandComponent` to eligible combat units
- Phase 4 PASS: positive timer prevents territory dispatch

Validation-path note:
- several checked-in Unity wrappers are still pinned to `D:\ProjectsHome\Bloodlines`
- because this slice was built and validated in the clean `D:\BLAICD\bloodlines` worktree,
  bootstrap runtime, scene-shell, fortification, and command-dispatch smokes were run
  directly through Unity execute methods against the worktree project path

## Key Design Notes

**Update order anchor.**
Both new systems use `[UpdateAfter(typeof(AICovertOpsSystem))]`, not
`AIBuildOrderSystem`, so the branch stays editor-build-clean on contract revision 18.

**Movement payload reuse.**
Territory dispatch writes `MoveCommandComponent`, which is the live movement seam already
used throughout the Unity runtime. No parallel move-order type was introduced.

**Worker gather now has an actual travel requirement.**
`AIWorkerCommandSystem` closes the decision-to-order gap, but
`WorkerGatherSystem` closes the order-to-arrival gap. The pair is required for correct
behavior.

## Post-Rebase Update (Revision 22)

- Rebased `codex/unity-ai-command-dispatch` onto `origin/master`
  `00847e77ab8d7e085adcbf5de4ac10baa584bcba` after Claude landed
  ai-strategic-layer sub-slices 8 and 9 on `master`.
- Updated `docs/unity/CONCURRENT_SESSION_CONTRACT.md` from revision 21 to 22 with
  the rev-21 master base preserved and the command-dispatch ownership layered on top.
- Reran the full 10-gate chain on the rebased `D:\BLAICD\bloodlines` worktree:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo` PASS
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` PASS
  - Bootstrap runtime smoke PASS via the authoritative worktree artifact
    `artifacts/unity-bootstrap-runtime-smoke.log`
  - Combat smoke PASS
  - Bootstrap scene shell PASS via `artifacts/unity-bootstrap-scene-batch-rev22.log`
  - Gameplay scene shell PASS via `artifacts/unity-gameplay-scene-batch-rev22.log`
  - Fortification smoke PASS
  - Siege smoke PASS
  - `node tests/data-validation.mjs` PASS
  - `node tests/runtime-bridge.mjs` PASS
  - Contract staleness check PASS at revision 22
- Dedicated AI command dispatch smoke rerun PASS on the rebased head via
  `artifacts/unity-ai-command-dispatch-batch-rev22.log`.
- Kept `[UpdateAfter(typeof(AICovertOpsSystem))]` on both new systems during the
  rebase. No `AIBuildOrderSystem` retargeting was introduced.

## Current Readiness

Branch `codex/unity-ai-command-dispatch` is rebased to the revision-21 `master`
base at `34b8d694c4726345a63ce1577d63e34d4bdce5e1`, all gates are green, and the
slice is ready for force-with-lease push plus merge to `master`.

## Next Action

1. Force-push `codex/unity-ai-command-dispatch`.
2. Merge `codex/unity-ai-command-dispatch` to `master`.
3. Rebase `codex/unity-fortification-siege` onto the new revision-22 `master` base
   and bump the contract to revision 23.
