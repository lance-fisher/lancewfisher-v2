# Unity AI Strategic Layer -- Sub-Slice 4: Worker Gather Dispatch

## Slice Metadata

- Date: 2026-04-18
- Branch: `claude/unity-ai-worker-gather`
- Lane: `ai-strategic-layer`
- Session Identifier: `claude-ai-worker-gather-2026-04-18`
- Contract Revision At Close: 16

## Goal

Port the AI idle-worker dispatch loop from the browser runtime (`ai.js` lines 1243-1251), resource priority rotation (`getEnemyGatherPriorities`, lines 885-922), and nearest-node selection (`chooseGatherNode`, lines 924-933) into a dedicated ECS system: `AIWorkerGatherSystem`. The system must be throttled by a configurable interval accumulator (default 5s), dispatch only idle workers, assign a resource node and resource ID, and avoid re-dispatching workers that are already in a non-idle gather phase.

## Work Completed

### New file: `unity/Assets/_Bloodlines/Code/AI/AIWorkerGatherSystem.cs`

ISystem with `[UpdateInGroup(typeof(SimulationSystemGroup))]` and `[UpdateAfter(typeof(EnemyAIStrategySystem))]`. Key behaviors:

- Per-faction accumulator throttle: `WorkerGatherAccumulator` on `AIStrategyComponent` is incremented by `deltaTime` each frame; dispatch fires only when the accumulator exceeds `WorkerGatherIntervalSeconds` (default 5s). The accumulator resets to 0 on each dispatch cycle.
- Idle worker query: entities with `WorkerGatherComponent` where `Phase == WorkerGatherPhase.Idle` and `FactionId == "enemy"`.
- Resource priority rotation: `workerIndex % 4` selects a starting resource slot from `{gold, wood, stone, iron}`. The system iterates from that slot forward (wrapping), selecting the first resource that has an available non-depleted node.
- `PlayerKeepFortified` flag on `AIStrategyComponent`: when `true`, the system skips stone as a gather priority (fortified-keep defense posture, mirrors `getEnemyGatherPriorities` ai.js:910-914 block).
- Nearest-node selection: for each candidate resource ID, scans all `ResourceNodeComponent` entities, filters `Amount > 0`, picks the node with minimum Euclidean distance to the worker's `PositionComponent.Value`.
- On dispatch: sets `WorkerGatherComponent.Phase = WorkerGatherPhase.Seeking`, `AssignedNode`, `AssignedResourceId`, `CarryResourceId` (matching `AssignedResourceId`).

### New file: `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAIWorkerGatherSmokeValidation.cs`

Static editor class with entry point `RunBatchAIWorkerGatherSmokeValidation()`. Four phases:

- Phase 1: Idle worker + non-depleted gold node. After one world update, worker Phase must be `Seeking` and `AssignedNode` must not be `Entity.Null`.
- Phase 2: All four resource nodes depleted (Amount=0). After one world update, worker Phase must remain `Idle` and `AssignedNode` must be `Entity.Null`.
- Phase 3: Worker already in `Gathering` phase. After one world update, Phase must remain `Gathering` (no re-dispatch).
- Phase 4: Two idle workers + gold node + wood node. After one world update, both workers must be `Seeking`, and the pair must cover both `"gold"` and `"wood"` assignments (rotation spread).

Artifact path: `../artifacts/unity-ai-worker-gather-smoke.log`. Marker: `BLOODLINES_AI_WORKER_GATHER_SMOKE PASS`.

### New file: `scripts/Invoke-BloodlinesUnityAIWorkerGatherSmokeValidation.ps1`

Self-contained PowerShell runner. Entry method: `Bloodlines.EditorTools.BloodlinesAIWorkerGatherSmokeValidation.RunBatchAIWorkerGatherSmokeValidation`. Polls log for `BLOODLINES_AI_WORKER_GATHER_SMOKE PASS/FAIL` marker with a 180s timeout and one retry pass.

### Modified: `unity/Assets/_Bloodlines/Code/AI/AIStrategyComponent.cs`

Added `WorkerGatherAccumulator` (float), `WorkerGatherIntervalSeconds` (float, default intent 5s), `IdleWorkerCount` (int), `WorkersDispatched` (int), `PlayerKeepFortified` (bool), plus 19 additional governance/event-context flag fields (added in sub-slice 3; sub-slice 4 uses `PlayerKeepFortified`).

### Modified: `unity/Assembly-CSharp.csproj`

Added `<Compile Include="Assets/_Bloodlines/Code/AI/AIWorkerGatherSystem.cs" />`.

### Modified: `unity/Assembly-CSharp-Editor.csproj`

Added `<Compile Include="Assets/_Bloodlines/Code/Editor/BloodlinesAIWorkerGatherSmokeValidation.cs" />`.

## Verification Results

All 10 gates green before commit:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`: 0 errors, 0 warnings
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: 0 errors, 0 warnings
3. Bootstrap runtime smoke: PASS (all prior proof fields present)
4. Combat smoke: exit 0, all 8 combat phases green
5. Scene shells: Bootstrap and Gameplay both green
6. AI governance pressure smoke: PASS (sub-slice 3 still green)
7. AI worker gather smoke: Phase 1 PASS, Phase 2 PASS, Phase 3 PASS, Phase 4 PASS
8. `node tests/data-validation.mjs`: exit 0
9. `node tests/runtime-bridge.mjs`: exit 0
10. Contract staleness check: revision 16 current

## Current Readiness

Branch `claude/unity-ai-worker-gather` is ready for merge to master. All validation gates green. Contract at revision 16.

## Next Action

Merge `claude/unity-ai-worker-gather` to master. Then claim `ai-strategic-layer-sub-slice-5-siege-staging` (see contract Next Unblocked Tier 1 Lanes) on a new branch `claude/unity-ai-siege-staging`, reading `src/game/core/ai.js` siege staging dispatch block before starting.
