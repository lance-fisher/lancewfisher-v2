# 2026-04-16 Unity First Production Slice

## Goal

Land the first governed Unity building-production slice so the Bootstrap shell can select a controlled production building, queue a unit, spawn it correctly through ECS, and prove the result through the governed runtime-smoke path.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Definitions/UnitDefinition.cs` and `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` so the Unity lane now preserves the canonical JSON production-gating fields it needs:
  - `movementDomain`
  - `faithId`
  - `doctrinePath`
  - `ironmarkBloodPrice`
  - `bloodProductionLoadDelta`
- Added the first house-aware production runtime state:
  - `unity/Assets/_Bloodlines/Code/Components/FactionHouseComponent.cs`
  - `unity/Assets/_Bloodlines/Code/Components/ProductionComponents.cs`
- Extended `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` so factions retain house identity and production-capable buildings receive live production buffers.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` so the first shell now supports:
  - controlled-building selection
  - a production panel
  - house, faith, doctrine, and movement-domain gate evaluation
  - queue issuance into ECS production buffers
  - debug helpers for governed runtime-smoke control
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs` so selected buildings get a visible debug highlight in the first shell.
- Added `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs` as the first ECS production resolver, then repaired it to use `EndSimulationEntityCommandBufferSystem` so spawned units no longer trigger structural-change exceptions during iteration.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` so the governed runtime-smoke lane now validates:
  - controlled `command_hall` selection
  - `villager` queue issuance
  - unit-count growth after production
  - queue drain after spawn
  - controlled-unit growth
  - post-production select-all

## Verification

- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed.
- Preserved pass log: `artifacts/unity-bootstrap-runtime-smoke.log`
- Latest governed runtime-smoke pass proved:
  - factions `3`
  - buildings `9`
  - units `17`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `7`
  - command selection after select-all `7`
  - control group 2 count `7`
  - `command_hall -> villager` production succeeded
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the production-slice changes.
- `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` passed after the new definition-field import work.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

- The first Unity battlefield shell is now:
  - structurally scene-valid
  - runtime-smoke green
  - command-shell validated for selection, grouping, framing, and the first production slice
- The remaining high-value gap is manual in-editor feel verification of building selection, production panel usability, training feel, camera feel, and capture or trickle feel.

## Next Action

1. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify:
   - unit select and building select
   - drag-box selection
   - `1` select-all
   - `Ctrl+2-5` save
   - `2-5` recall
   - `F` frame selection
   - formation-aware move issuance
   - `command_hall -> villager` production-panel flow and queue feel
   - battlefield camera pan, rotate, zoom, and focus feel
3. Verify live control-point capture, contested decay, stabilization, and uncontested trickle in-editor.
4. Continue directly into construction placement, richer production queue UX, or broader production-roster verification. Do not prioritize attack-move until a real combat lane exists.
