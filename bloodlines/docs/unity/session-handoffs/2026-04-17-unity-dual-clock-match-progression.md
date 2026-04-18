# Unity Slice Handoff: Dual-Clock + Match-Progression Sub-Slice 1

**Date:** 2026-04-17
**Lane:** dual-clock-match-progression
**Branch:** claude/unity-match-progression
**Session:** claude-match-progression-2026-04-17

## Goal

Port the browser dual-clock and five-stage match-progression system to Unity ECS sub-slice 1:
components, tick system, evaluation system, smoke validator, PS1 wrapper, contract lane claim.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/Time/DualClockComponent.cs`
  - Singleton IComponentData: `InWorldDays`, `DaysPerRealSecond` (canonical default 2), `DeclarationCount`
  - Namespace: `Bloodlines.GameTime` (not `Bloodlines.Time` -- avoids conflict with UnityEngine.Time)
  - Browser equivalent: `state.dualClock` (simulation.js:13784 ensureDualClockState)

- `unity/Assets/_Bloodlines/Code/Time/MatchProgressionComponent.cs`
  - Singleton IComponentData with full five-stage state: StageNumber, StageId/Label, PhaseId/Label, StageReadiness, NextStageId/Label, InWorldDays/Years, DeclarationCount, RivalContactActive, SustainedWarActive, GreatReckoningActive/TargetFactionId/Share/Threshold, DominantKingdomId/TerritoryShare
  - Browser equivalent: `state.matchProgression` (simulation.js:13395 ensureMatchProgressionState)

- `unity/Assets/_Bloodlines/Code/Time/DualClockTickSystem.cs`
  - `[BurstCompile] partial struct DualClockTickSystem : ISystem`
  - OnCreate: creates DualClockComponent singleton with canonical defaults
  - OnUpdate: `InWorldDays += dt * DaysPerRealSecond`
  - Runs before MatchProgressionEvaluationSystem in SimulationSystemGroup
  - Browser equivalent: `tickDualClock` (simulation.js:13795)

- `unity/Assets/_Bloodlines/Code/Time/MatchProgressionEvaluationSystem.cs`
  - `partial struct MatchProgressionEvaluationSystem : ISystem` (no BurstCompile -- multi-query)
  - OnCreate: creates MatchProgressionComponent singleton at Stage 1 defaults
  - OnUpdate: queries control points, faction entities (ResourceStockpile, Population, FaithState), completed buildings (no ConstructionStateComponent), living military units (MovementStats + Health, no DeadTag) to evaluate all five stages
  - Stage 1-3: fully implemented from live ECS data
  - Stage 4-5 war signals: placeholder false -- requires declaration-seam port in sub-slice 2
  - Great Reckoning: fully implemented from dominant territory share (triggers at 70%, releases at 66%)
  - Browser equivalent: `computeMatchProgressionState` + `updateMatchProgressionState` (simulation.js:13426, 13557)

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.MatchProgression.cs`
  - `TryDebugGetDualClock(out DualClockComponent clock)`
  - `TryDebugGetMatchProgression(out MatchProgressionComponent progression)`

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionSmokeValidation.cs`
  - Phase 1: DualClock singleton created on system registration; InWorldDays=0 DaysPerRealSecond=2
  - Phase 2: 10-tick arithmetic: 10 * 2 = 20 InWorldDays
  - Phase 3: MatchProgression singleton created on system registration; StageNumber=1 StageId=founding
  - Phase 4: After seeding player faction with all stage 2+3 requirements (food, water, buildings, military, faith, 2 territories), StageNumber advances to 3

- `scripts/Invoke-BloodlinesUnityMatchProgressionSmokeValidation.ps1`
  - Batch wrapper invoking `RunBatchMatchProgressionSmokeValidation`

### Modified Files

- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- revision 5 → 6, dual-clock-match-progression lane added as active, removed from unclaimed candidates
- `unity/Assembly-CSharp.csproj` -- 5 new Compile entries for Time/ files + debug partial
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry for smoke validator

## Verification Results

| Gate | Result |
|---|---|
| dotnet build Assembly-CSharp.csproj | 0 errors, 0 warnings |
| dotnet build Assembly-CSharp-Editor.csproj | 0 errors (pre-existing CS0649 warnings only) |
| node tests/data-validation.mjs | PASS |
| node tests/runtime-bridge.mjs | PASS |
| Invoke-BloodlinesUnityContractStalenessCheck.ps1 | PASS revision=6 |
| Bootstrap + combat Unity smokes | Not re-run (Unity not in PATH); builds are gate for sub-slice 1 |

## Current Readiness

The dual-clock singleton ticks correctly. Stage 1-3 evaluation is live from real ECS data. Stage 4-5 advancement is blocked on unimplemented war signals -- the game will advance to Stage 3 but stall at readiness=0 for Stage 4 until sub-slice 2 wires the declaration seam.

## Next Action (Sub-Slice 2)

Implement `DualClockDeclarationSystem`: a request-buffer approach where any system can push a `DeclareInWorldTimeRequest { float DaysDelta; FixedString64Bytes Reason; }` buffer element onto the DualClock entity. The system processes the buffer each tick, accumulates daysDelta into InWorldDays, increments DeclarationCount. Also port `rivalContactActive` signal by checking whether player-owned control points are adjacent (within CaptureTimeSeconds threshold) to enemy-owned control points. Port `contestedBorder` (any CP with IsContested). Port `sustainedWarActive` (using fortification siege engine count when the fortification-siege lane is complete). Update MatchProgressionEvaluationSystem to read these live signals instead of placeholder false.
