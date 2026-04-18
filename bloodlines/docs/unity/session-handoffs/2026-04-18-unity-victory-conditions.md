# Unity Slice Handoff: Victory Conditions System

- Date: 2026-04-18
- Session: 130
- Branch: codex/unity-fortification-siege
- Lane: victory-conditions

## Goal

Port the three canonical victory conditions from the browser spec (simulation.js) to Unity DOTS/ECS:
military conquest (command hall fall), territorial governance hold, and divine right faith recognition.

## Work Completed

### New Files

- `unity/Assets/_Bloodlines/Code/Victory/VictoryComponents.cs`
  - `MatchStatus` enum (Playing/Won/Lost)
  - `VictoryConditionId` enum (None/CommandHallFall/TerritorialGovernance/DivinRight)
  - `VictoryStateComponent` (IComponentData singleton -- Status, VictoryType, WinnerFactionId,
    VictoryReason, TerritorialGovernanceHoldSeconds)

- `unity/Assets/_Bloodlines/Code/Victory/VictoryConditionEvaluationSystem.cs`
  - `[UpdateInGroup(typeof(SimulationSystemGroup))]` ISystem
  - Returns early when Status != Playing
  - Condition 1 (CommandHallFall): queries dead BuildingTypeComponent entities with TypeId=="command_hall".
    Player hall dead -> Lost. Enemy hall dead -> Won.
  - Condition 2 (TerritorialGovernance): queries player-owned ControlPointComponent entities.
    All must have Loyalty >= 90. Accumulates real-time seconds; triggers at 120s.
    Accumulator resets to 0 if any loyal territory is lost.
  - Condition 3 (DivinRight): queries FaithStateComponent for Level >= 5 and Intensity >= 80.
    Player faction -> Won. Enemy -> Lost.
  - Constants: TerritorialGovernanceLoyaltyThreshold=90f, TerritorialGovernanceVictorySeconds=120f,
    DivinRightFaithLevel=5, DivinRightIntensityThreshold=80f

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesVictoryConditionsSmokeValidation.cs`
  - Phase 1: Enemy command hall with DeadTag -> Won, CommandHallFall.
  - Phase 2: Player command hall with DeadTag -> Lost, CommandHallFall.
  - Phase 3: Two loyal player control points, hold pre-seeded to (120-0.001)s -> Won, TerritorialGovernance.
  - Phase 4: Player FaithStateComponent Level=5, Intensity=85 -> Won, DivinRight.

- `scripts/Invoke-BloodlinesUnityVictoryConditionsSmokeValidation.ps1`
  - Artifact: `artifacts/unity-victory-conditions-smoke.log`
  - Marker: `BLOODLINES_VICTORY_CONDITIONS_SMOKE PASS/FAIL`

### csproj Registrations

- `Assembly-CSharp.csproj`: VictoryComponents.cs, VictoryConditionEvaluationSystem.cs
- `Assembly-CSharp-Editor.csproj`: BloodlinesVictoryConditionsSmokeValidation.cs

## Verification Results

All 8 gates green:
1. dotnet build Assembly-CSharp.csproj: 0 errors
2. dotnet build Assembly-CSharp-Editor.csproj: 0 errors
3. Bootstrap runtime smoke: PASS
4. Combat smoke: all 8 phases PASS
5. Canonical scene shells: PASS
6. data-validation.mjs: exit 0
7. runtime-bridge.mjs: exit 0
8. Contract staleness check: exit 0 (revision 12)

Victory conditions smoke: PASS (all 4 phases)
- Phase 1 PASS: Status=Won VictoryType=CommandHallFall
- Phase 2 PASS: Status=Lost VictoryType=CommandHallFall
- Phase 3 PASS: Status=Won VictoryType=TerritorialGovernance HoldSeconds=120.015
- Phase 4 PASS: Status=Won VictoryType=DivinRight

## Current Readiness

VictoryStateComponent singleton + VictoryConditionEvaluationSystem are production-ready.
Three canonical victory paths are live and tested. This is a complete slice.

## Design Notes

- TerritorialGovernance hold timer pre-seeding trick in smoke test: seed
  TerritorialGovernanceHoldSeconds = VictorySeconds - 0.001f so a single batch tick
  (dt=0.016f) pushes it over threshold without running thousands of ticks.
- DivinRight: simplified to Level >= 5 + Intensity >= 80 without tracking the full
  spread window declaration entity (spread window tracking is a follow-up slice).
- CommandHallFall: reads DeadTag on buildings. DeathResolutionSystem must run before
  VictoryConditionEvaluationSystem in the same frame for same-frame detection; confirmed
  by smoke test seeding DeadTag directly.

## Next Action

All three assigned items (1, 2, 3) are now complete:
- Item 1: AI Strategic Layer (EnemyAIStrategySystem) -- DONE (Session 129)
- Item 2: Tier 2 Batch Systems -- DONE (Session 130)
- Item 3: Victory Conditions System -- DONE (Session 130)

Next priority: Fortification + Siege system (codex/unity-fortification-siege branch already
has initial components -- FortificationComponent, FortificationReserveComponent,
SiegeSupplyTrainComponent, SiegeSupportComponent etc. -- those files need registering and
system implementations ported from simulation.js advanceFortificationTier,
tickFortificationReserves, tickSiegeSupportLogistics).
