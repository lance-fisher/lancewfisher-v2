# Unity AI Strategic Layer -- Sub-Slice 5: Siege Staging Orchestration

## Slice Metadata

- Date: 2026-04-19
- Branch: `claude/unity-ai-siege-staging`
- Lane: `ai-strategic-layer`
- Session Identifier: `claude-ai-siege-staging-2026-04-19`
- Contract Revision At Close: 17

## Goal

Port the AI siege staging decision tree from the browser runtime (`ai.js` attackTimer<=0 block, lines ~1825-2090) into a dedicated ECS system: `AISiegeOrchestrationSystem`. The system must evaluate all 16 readiness gate conditions in priority order and write a `SiegeOrchestrationPhase` tag. Actual unit movement and attack commands are deferred to the movement and combat systems; this system is a pure state machine.

## Work Completed

### New file: `unity/Assets/_Bloodlines/Code/AI/AISiegeOrchestrationComponent.cs`

`AISiegeOrchestrationComponent` IComponentData holding:
- `Phase` (SiegeOrchestrationPhase): written by the system each frame
- `ArmyCount` (int): total army; phase requires >= 3 to engage
- Siege readiness flags: `EnemyHasSiegeUnit`, `EngineeringReady`, `SupplyCampCompleted`, `SupplyLineReady`, `SuppliedSiegeReady`, `EscortArmyCount`, `PlayerVerdantWardenCount`, `FormalSiegeLinesFormed`, `ReliefArmyApproaching`
- Field water crisis flags: `FieldWaterDesertionRisk`, `FieldWaterAttritionActive`, `FieldWaterCriticalCount`
- Supply interdiction/recovery: `InterdictedWagonCount`, `RecoveringWagonCount`, `ConvoyRecoveringUnscreenedCount`
- Supply collapse: `SupplyReadyButCollapsed`
- Post-repulse: `CohesionPenaltyUntil`, `PostRepulseUntil`, `RepeatedAssaultAttempts`

`SiegeOrchestrationPhase` enum with 17 values (Inactive through Assaulting) matching the exact branch priority order in ai.js.

### New file: `unity/Assets/_Bloodlines/Code/AI/AISiegeOrchestrationSystem.cs`

ISystem with `[UpdateInGroup(typeof(SimulationSystemGroup))]` and `[UpdateAfter(typeof(AIWorkerGatherSystem))]`. Pure state machine: reads `AISiegeOrchestrationComponent` flags and `AIStrategyComponent.PlayerKeepFortified`, then writes `Phase` via `DeterminePhase`.

Branch priority (exact ai.js order preserved):
1. Inactive (keep not fortified or army < 3)
2. FieldWaterRetreat (any field water condition -- overrides all)
3. SiegeRefusal (no siege engines)
4. AwaitingEngineers
5. AwaitingSupplyCamp
6. AwaitingSupplyLine
7. SupplyInterdicted
8. SupplyRecoveringUnscreened (pull back)
9. SupplyRecoveringScreened (hold at stage)
10. AwaitingResupply (move to stage)
11. AwaitingEscort (escort threshold: 3 + min(2, verdantWardenCount))
12. StagingLines (move to stage, form lines)
13. ReliefHold (player relief column approaching)
14. PostRepulse (cohesion penalty active)
15. RepeatedAssault (post-repulse cooldown expired, attempts 1-3)
16. SupplyCollapse (mid-siege supply chain collapsed)
17. Assaulting (all gates clear)

### New file: `unity/Assets/_Bloodlines/Code/Editor/BloodlinesAISiegeOrchestrationSmokeValidation.cs`

Static editor class with entry point `RunBatchAISiegeOrchestrationSmokeValidation()`. Six phases:
- Phase 1: Keep not fortified -> Inactive
- Phase 2: FieldWaterDesertionRisk=true (all other flags set) -> FieldWaterRetreat (priority override)
- Phase 3: No siege engines -> SiegeRefusal
- Phase 4: All gates except FormalSiegeLinesFormed -> StagingLines
- Phase 5: All gates clear -> Assaulting
- Phase 6: CohesionPenaltyUntil=9999f -> PostRepulse

Artifact: `../artifacts/unity-ai-siege-orchestration-smoke.log`. Marker: `BLOODLINES_AI_SIEGE_ORCHESTRATION_SMOKE PASS`.

### New file: `scripts/Invoke-BloodlinesUnityAISiegeOrchestrationSmokeValidation.ps1`

Self-contained PowerShell runner. Entry method: `Bloodlines.EditorTools.BloodlinesAISiegeOrchestrationSmokeValidation.RunBatchAISiegeOrchestrationSmokeValidation`. Polls log for `BLOODLINES_AI_SIEGE_ORCHESTRATION_SMOKE PASS/FAIL` with 180s timeout and one retry.

### Modified: `unity/Assembly-CSharp.csproj`

Added `AISiegeOrchestrationComponent.cs` and `AISiegeOrchestrationSystem.cs`.

### Modified: `unity/Assembly-CSharp-Editor.csproj`

Added `BloodlinesAISiegeOrchestrationSmokeValidation.cs`.

## Verification Results

All 10 gates green before commit:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`: 0 errors
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`: 0 errors
3. Bootstrap runtime smoke: PASS (all prior proof fields present, including worldPressureChecked=True)
4. Combat smoke: exit 0, all 8 phases green
5. Scene shells: Bootstrap and Gameplay both green
6. AI governance pressure smoke: PASS (sub-slice 3 still green)
7. AI worker gather smoke: Phase 1-4 PASS (sub-slice 4 still green)
8. AI siege orchestration smoke: Phase 1-6 PASS
9. `node tests/data-validation.mjs`: exit 0
10. `node tests/runtime-bridge.mjs`: exit 0
11. Contract staleness check: revision 17 current

## Current Readiness

Branch `claude/unity-ai-siege-staging` is ready for merge to master. All validation gates green. Contract at revision 17.

## Next Action

Merge `claude/unity-ai-siege-staging` to master. Then continue to sub-slice 6: dynasty-aware covert ops (`src/game/core/ai.js` ~line 2681): assassination, marriage proposal, missionary dispatch on a new branch `claude/unity-ai-dynasty-covert-ops`.
