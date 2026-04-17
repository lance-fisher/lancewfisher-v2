# 2026-04-17 Unity Building Resource Trickle + Concurrent Session Contract

## Goal

Port the browser runtime's `tickPassiveResources` loop to Unity ECS. Before this
slice, farms and wells existed as building entities but were economically inert:
canonical `resourceTrickle` values like `farm.food = 0.5` and `well.water = 0.45`
were either truncated to zero by an integer-typed `ResourceAmountFields` at
import, or never applied at runtime even when present. This slice closes that
gap end-to-end and also establishes the concurrent-session contract so that
Claude (economy lane) and Codex (combat lane) can work in parallel without
stepping on each other.

## Work Completed

- Added `docs/unity/CONCURRENT_SESSION_CONTRACT.md`: canonical coordination
  document defining lane ownership, file scope, Unity wrapper lock protocol,
  branch and merge discipline, validation gates, and handoff discipline.
- Added `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1`: helper that
  enforces the wrapper lock at `.unity-wrapper-lock` before invoking any other
  governed Unity PS1 wrapper. Honors a 15-minute stale-lock reclaim window,
  polls every 30 seconds up to 10 minutes, writes UTF-8 lock state with session
  name, timestamp, and target script.
- Fixed `unity/Assets/_Bloodlines/Code/Definitions/BuildingDefinition.cs` so
  `resourceTrickle` is now `ResourceTrickleFields` (float) instead of
  `ResourceAmountFields` (int). Canonical values like `farm.food = 0.5` and
  `well.water = 0.45` now survive round-trip through the ScriptableObject layer
  instead of being truncated to `0`.
- Fixed `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`:
  - `BuildingRecord.resourceTrickle` is now `ResourceTrickleFields`.
  - Importer writes `asset.resourceTrickle = item.resourceTrickle ?? new ResourceTrickleFields()`.
- Regenerated all 23 building ScriptableObject definitions via
  `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1`. Verified farm asset now
  carries `resourceTrickle.food: 0.5` and well asset carries
  `resourceTrickle.water: 0.45`.
- Added `unity/Assets/_Bloodlines/Code/Economy/ResourceTrickleBuildingComponent.cs`:
  per-building component storing `GoldPerSecond`, `FoodPerSecond`, `WaterPerSecond`,
  `WoodPerSecond`, `StonePerSecond`, `IronPerSecond`, `InfluencePerSecond`.
- Added `unity/Assets/_Bloodlines/Code/Economy/ResourceTrickleBuildingSystem.cs`:
  `ISystem` that runs every simulation tick before `PopulationGrowthSystem`.
  For each controlled building entity with a trickle component, alive
  (`HealthComponent.Current > 0`), and not under construction
  (`!HasComponent<ConstructionStateComponent>`), it accumulates
  `rate * dt` per resource into a per-faction `NativeParallelHashMap`, then
  applies the accumulated delta to each owning faction's
  `ResourceStockpileComponent`. Structural-changes-during-iteration are avoided
  by collecting into the hash map and then writing back in a second pass.
- Extended `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs`:
  `MapBuildingSeedElement` now carries `GoldTrickle`, `FoodTrickle`,
  `WaterTrickle`, `WoodTrickle`, `StoneTrickle`, `IronTrickle`, `InfluenceTrickle`.
- Extended `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
  and `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`: both
  now populate the new trickle fields from each building's
  `BuildingDefinition.resourceTrickle`.
- Extended `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`:
  when spawning a building from a seed element, attaches
  `ResourceTrickleBuildingComponent` whenever any trickle field is positive.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  the runtime construction-placement path now attaches
  `ResourceTrickleBuildingComponent` on the new construction site so that
  player-built farms / wells / other trickle buildings start contributing
  economy on completion (the existing construction -> completion lane drops
  the `ConstructionStateComponent`; the trickle system correctly ignores
  under-construction sites via `HasComponent<ConstructionStateComponent>`).
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  `RuntimeSmokeState` gains `trickleBaselineSampled`, `trickleInitialFood`,
  `trickleInitialWater`, `trickleSampledUtcTicks`, `trickleLatestFood`,
  `trickleLatestWater`, `trickleMinimumFoodGain` (default 2), `trickleMinimumWaterGain`
  (default 2), `trickleTimeoutSeconds` (default 30), `trickleGainObserved`.
  Two new helpers:
  - `ProbeResourceTrickleBaseline` snapshots the controlled faction's initial
    food and water just before the existing gather-cycle probe.
  - `ProbeResourceTrickleGain` runs after the gather probe and asserts both
    food and water have risen by at least their configured minimum gain
    within the configured timeout.
  Final success diagnostics now carries `trickleInitialFood`, `trickleLatestFood`,
  `trickleInitialWater`, `trickleLatestWater`, `trickleGainObserved`.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors.
- `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` passed (via wrapper lock).
  Farm and well assets verified to carry the canonical float trickle values.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed
  (via wrapper lock). Success line now ends with:
  - `trickleInitialFood=39.70`
  - `trickleLatestFood=43.77`
  - `trickleInitialWater=37.13`
  - `trickleLatestWater=40.69`
  - `trickleGainObserved=True`
  alongside preserved `gatherDepositObserved=True`,
  `productionProgressAdvancementVerified=True`,
  `constructionProgressAdvancementVerified=True`, and the prior `buildings=11`,
  `units=18`, `controlledUnits=8`, `populationCap=24` totals.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed (via wrapper lock).
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves the complete canonical primary
economy loop end to end:

- command-shell selection, drag-box, control groups, framing, formation move
- two-deep production queue with cancel-and-refund and mid-production
  advancement observation
- worker-led dwelling construction with mid-construction progress observability
- worker-led barracks construction with post-completion militia continuity
- world-space construction and production progress bars
- primary-economy gather-deposit loop (Session 112)
- **passive building resource trickle: farms and wells raise faction food and
  water on every simulation tick while both buildings are alive, completed, and
  not under construction**

Concurrent-session infrastructure is now in place:

- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` governs lane ownership, file scope,
  and wrapper serialization
- `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1` enforces the wrapper lock
  at `.unity-wrapper-lock`
- Claude owns `Code/Economy/**` and bootstrap smoke extensions;
  Codex owns `Code/Combat/**` and its dedicated combat smoke validator

## Next Action

Within the economy lane:

1. Add a worker-built farm and a worker-built well to the smoke proof so the
   Session 111 right-click construction path is also exercised against the new
   trickle-on-spawn wiring for runtime-placed buildings.
2. Extend the debug command surface with a right-click gather assignment so
   players can issue `Select workers -> right-click resource node -> gather`
   without the governed debug API.
3. Render a small world-space carry indicator on workers via the presentation
   bridge so full gather cycles are visually legible in Play Mode.
4. Begin the water-and-food starvation response slice (population decline per
   cycle when food or water hits zero) per `realm-conditions.json` canon.
5. Add wood and stone worker assignment UI once the debug command surface gains
   a resource-type picker.

Beyond the economy lane: see
`docs/unity/CONCURRENT_SESSION_CONTRACT.md` for Codex's combat-lane scope,
which runs concurrently.
