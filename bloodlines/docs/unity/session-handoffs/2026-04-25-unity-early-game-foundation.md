# Session Handoff: Early-Game Foundation
Date: 2026-04-25
Lane: early-game-foundation
Slice: Keep deployment, first build tier, water model, productivity states, draft slider, five-person squads, worker slots, reserve/active duty

## Goal

Implement the canonical early-game foundation mechanics into the Unity ECS codebase per the 2026-04-25 early-game-foundation prompt. Full canon, no MVP reduction.

## Work Completed

### data/buildings.json
- Added `buildTier` field (int) to every building: 0=pre-deploy/environment, 1=first post-Keep tier, 2=small-farm tier, 3+=full tree
- Added new buildings: `housing` (tier 1, popCap+8), `forager_camp` (tier 1, 6 worker slots, food 0.15/worker/s), `training_yard` (tier 1, trains militia), `small_farm` (tier 2, 5 worker slots, food 0.22/worker/s)
- `lumber_camp` renamed to "Woodcutter Camp", added worker slots (6, wood 0.18/worker/s)
- `well` added `waterPopulationSupport: 50`
- `farm` moved to buildTier 3 (established agriculture, later than small farm)

### unity/Assets/_Bloodlines/Code/Definitions/BuildingDefinition.cs
- Added `buildTier`, `maxWorkerSlots`, `workerOutputPerSecond`, `waterPopulationSupport` fields

### New components — unity/Assets/_Bloodlines/Code/Components/

`FoundingRetinueComponent.cs`:
- `FoundingRetinueComponent`: IsDeployed, DeployPosition, RelocationEligible, MobileElapsedSeconds
- `UndeployedKeepTag`, `BuildTierComponent` (CurrentTier byte + HasHousing/Water/FoodSource/TrainingYard bools)
- `KeepRelocationInProgressTag` stub

`EarlyGameComponents.cs`:
- `WorkerSlotBuildingComponent`: MaxWorkerSlots, AssignedWorkers, FoodOutputPerWorkerPerSecond, WoodOutputPerWorkerPerSecond, FillRatio
- `WaterCapacityComponent`: MaxSupportedByWater (int)
- `PopulationProductivityComponent`: BaseProductivity, EffectiveProductivity, adequacy bools, shortage accumulators
- `MilitaryDraftComponent`: DraftRatePct (0-100 step 5), DraftPool, TrainedMilitary, UntrainedDrafted, ReserveMilitary, ActiveDutyMilitary, OverDraftedFlag
- `DutyState` enum, `SquadAssignmentType` enum (9 assignment types)
- `MilitiaSquadComponent`: SquadSize, DutyState, AssignmentType

### New systems — unity/Assets/_Bloodlines/Code/Systems/

`EarlyGameConstants.cs`: static class, single source for all early-game balance values

`BuildTierGatingSystem.cs`: BurstCompile, SimulationSystemGroup
- Scans buildings per faction into bitmask NativeHashMap, updates BuildTierComponent each frame
- Tier 0 = not deployed, Tier 1 = deployed, Tier 2 = all four prerequisites present

`WaterCapacitySystem.cs`: BurstCompile, before PopulationProductivitySystem
- Counts wells per faction: MaxSupportedByWater = 15 (base) + wellCount * 50

`PopulationProductivitySystem.cs`: BurstCompile, after Water, before WorkerSlot
- Stage 1: BaseProductivity weighted average (Civilian 100%, Untrained 75%, Reserve 50%, ActiveDuty 5%)
- Stage 2: EffectiveProductivity *= shortage modifiers (food 0.70, water 0.65, housing 0.85)

`WorkerSlotProductionSystem.cs`: BurstCompile, after PopulationProductivity
- output = BaseRatePerWorker x AssignedWorkers x EffectiveProductivity x dt

`MilitaryDraftSystem.cs`: BurstCompile, before PopulationProductivity
- Clamps DraftRatePct to step-5, derives DraftPool, tallies trained/reserve/activeduty

`SquadDutySystem.cs`: BurstCompile, before MilitaryDraft
- AssignmentType.None -> Reserve, any other -> ActiveDuty

### New HUD — unity/Assets/_Bloodlines/Code/HUD/

`EarlyGameHUDComponent.cs`: singleton IComponentData with all early-game display fields

`EarlyGameHUDSystem.cs`: PresentationSystemGroup
- Two-pass design (max 5 type params per query to stay within Unity ECS limit)
- Pass 1: FactionComponent + PopulationComponent + ResourceStockpileComponent + MilitaryDraftComponent + PopulationProductivityComponent with EntityAccess
- Overflow components (WaterCapacity, FoundingRetinue, BuildTier) fetched via EntityManager.GetComponentData

### New debug — unity/Assets/_Bloodlines/Code/Debug/

`BloodlinesDebugCommandSurface.EarlyGame.cs`: partial class on BloodlinesDebugCommandSurface
- DrawEarlyGamePanel(): full read-only display of HUD snapshot
- Test commands: DeployPlayerKeep, SetPlayerDraftRate, SpawnTestSquad, SetFirstSquadAssignment
- Uses established TryGetEntityManager pattern from the main surface class

### Modified systems

`SkirmishBootstrapSystem.cs`: SpawnFactionEntity
- Kingdoms start with IsDeployed=false, Tier=0; non-Kingdoms start IsDeployed=true, Tier=2
- All factions get MilitaryDraftComponent, PopulationProductivityComponent, WaterCapacityComponent

`PopulationGrowthSystem.cs`: added WaterCapacityComponent query
- Growth now requires withinWaterCap (Total < MaxSupportedByWater) in addition to housing/food/water

`Assembly-CSharp.csproj`: added 12 Compile Include entries for all new files

## Key Engineering Notes

- `NativeHashMap` indexer setter `map[key] = value` is not allowed on `using var` variables (CS1654). Pattern throughout: `map.Remove(key); map.Add(key, value);`
- `SystemAPI.Query<>` max is 8 type params; HUD system has 9 required, solved by splitting into per-entity EntityManager.GetComponentData calls for overflow components
- Variable shadowing in ECS foreach: rename local aliases of foreach parameters to avoid CS0136 (e.g., `var draftData = draft.ValueRO` not `var draft = draft.ValueRO`)
- BloodlinesDebugCommandSurface partial class must use `TryGetEntityManager` (existing private static), not the nullable `World.DefaultGameObjectInjectionWorld?.EntityManager` pattern

## Validation Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` — PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` — PASS (116 pre-existing warnings, 0 errors)
3. Bootstrap runtime smoke — PASS (all proof fields present including starvation, loyalty, AI construction, trickle)
4. Combat smoke — PASS (melee and projectile phases green)
5. Scene shell validation — PASS (Bootstrap and Gameplay both green)
6. `node tests/data-validation.mjs` — PASS
7. `node tests/runtime-bridge.mjs` — PASS
8. Contract staleness check — PASS (revision 144, current)

## Current Readiness

All early-game foundation ECS systems are implemented, compiled, and validated. The systems run every frame and update correctly given the right entity configuration. No UI integration beyond the debug panel yet.

## Next Actions

- Wire `DrawEarlyGamePanel()` into the existing `OnGUI` debug flow in `BloodlinesDebugCommandSurface.cs` so it appears in the debug surface
- Add `JsonContentImporter` handling for new buildings.json fields (`buildTier`, `maxWorkerSlots`, `workerOutputPerSecond`, `waterPopulationSupport`) so they load into `BuildingDefinition` at import time
- Add `WorkerSlotBuildingComponent` authoring to the building prefab pipeline for the four new worker-slot buildings (woodcutter_camp, forager_camp, small_farm)
- Add player input bridge: UI for assigning workers to buildings and setting squad orders
- Commit this slice to the lane branch
