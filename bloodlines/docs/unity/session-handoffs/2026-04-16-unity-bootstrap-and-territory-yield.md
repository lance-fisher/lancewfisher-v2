# Unity Session Handoff - 2026-04-16 - Bootstrap And Territory Yield Foundation

## Scope

Continue the Unity production lane from the earlier movement foundation by:

- creating the first `MapDefinition` -> ECS bootstrap bridge
- correcting map-definition fidelity drift for control-point trickle and continent data
- preserving the first territorial resource-yield path in ECS runtime state

## Files Added

- `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`
- `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs`
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
- `unity/Assets/_Bloodlines/Code/Systems/ControlPointResourceTrickleSystem.cs`

## Files Updated

- `unity/Assets/_Bloodlines/Code/Definitions/BloodlinesDefinitions.cs`
- `unity/Assets/_Bloodlines/Code/Components/ControlPointComponent.cs`
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
- `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
- `unity/Assets/_Bloodlines/Code/README.md`
- `unity/Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset`

## What Changed

### 1. Bootstrap bridge

- `BloodlinesMapBootstrapAuthoring` is now the scene-facing authoring anchor for a Bootstrap scene.
- `BloodlinesMapBootstrapBaker` loads canonical ScriptableObject definitions and bakes:
  - faction seeds
  - building seeds
  - unit seeds
  - resource-node seeds
  - control-point seeds
  - settlement seeds
- `SkirmishBootstrapSystem` now consumes those buffers once and spawns the first live ECS entities for factions, settlements, control points, resource nodes, buildings, and units.

### 2. Map data fidelity fix

- `data/maps/ironmark-frontier.json` defines fractional control-point `resourceTrickle` values such as `0.18` and `0.22`.
- The Unity definitions previously typed `ControlPointData.resourceTrickle` as `ResourceAmountFields` (ints), which collapsed those values to zero in `ironmark_frontier.asset`.
- `BloodlinesDefinitions.cs` now uses `ResourceTrickleFields` (floats) for control-point trickle.
- The same file now also preserves:
  - `TerrainPatchData.isCoastal`
  - `TerrainPatchData.continentDivide`
  - `ControlPointData.continentId`
- `ironmark_frontier.asset` was patched to restore the missing control-point trickle values and the newly preserved continent metadata.

### 3. Runtime territory-yield preservation

- `ControlPointComponent` now preserves:
  - `ContinentId`
  - `GoldTrickle`
  - `FoodTrickle`
  - `WaterTrickle`
  - `WoodTrickle`
  - `StoneTrickle`
  - `IronTrickle`
  - `InfluenceTrickle`
- `ControlPointResourceTrickleSystem` now applies canonical first-pass owned-territory yield:
  - `Occupied` control points pay `42%`
  - `Stabilized` control points pay `100%`
  - neutral or contested states pay nothing
- This is intentionally narrow parity. Governor, doctrine, and political-event multipliers remain future follow-up work.

## Verification

### Browser lane

- `node tests/data-validation.mjs` passed
- `node tests/runtime-bridge.mjs` passed

### Unity lane

Because the canonical Unity project was already open and holding the default `Temp\\obj` outputs, direct `dotnet build` against the default intermediate path was lock-blocked. Verification succeeded by using isolated Codex intermediate/output paths:

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`

Results:

- runtime assembly: `0 warnings`, `0 errors`
- editor assembly: `0 errors`, longstanding `JsonContentImporter.cs` warning debt remains

## Current Blockers

- No canonical Bootstrap or Gameplay scenes exist yet, so the new authoring and bootstrap path is not exercised in Play Mode.
- Control-point yield is now wired, but ownership/capture logic is still needed before it becomes visible in runtime play.
- The project was already open in another Unity instance, so a fresh batchmode editor pass was not used for this increment.

## Next Exact Steps

1. In the already-open Unity 6.3 editor, create:
   - `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
   - `Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity`
2. Add a GameObject in `Bootstrap.unity` with `BloodlinesMapBootstrapAuthoring` and assign `Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset`.
3. Enter Play Mode and verify that the bootstrap creates:
   - faction entities
   - building entities
   - unit entities
   - resource nodes
   - settlements
   - control points
4. Add the first battlefield camera shell immediately after the bootstrap is scene-live.
5. After scene verification, deepen control-point parity with capture/ownership flow and the still-missing doctrine/governor yield modifiers.
