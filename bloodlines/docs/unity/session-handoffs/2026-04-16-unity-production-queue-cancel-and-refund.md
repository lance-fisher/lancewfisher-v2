# 2026-04-16 Unity Production Queue Cancel-And-Refund

## Goal

Deepen the first governed Unity production slice so the production shell can cancel queued units safely, refund the queued spend, and prove that behavior through the governed Bootstrap runtime-smoke lane before final unit completion.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Components/ProductionComponents.cs` so `ProductionQueueItemElement` now preserves:
  - queued resource costs
  - queued Ironmark blood price
  - queued blood-production load delta
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` so the first production panel now:
  - renders visible queue rows
  - exposes cancel buttons for queued entries
  - refunds queued resource and population spend on cancellation
  - exposes a debug cancel hook for governed validation
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` so the governed runtime-smoke lane now proves:
  - controlled `command_hall` selection
  - `villager` queue issuance
  - queued-entry cancellation before completion
  - refund-delta verification against the stored queue payload
  - final re-queue and successful spawned-unit completion

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate and output paths with `0` warnings and `0` errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate and output paths with `0` errors. Existing editor/importer `CS0649` warnings remain unchanged.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed.
- `artifacts/unity-bootstrap-runtime-smoke.log` now preserves a successful runtime-smoke line with:
  - `productionCancelVerified=True`
  - factions `3`
  - buildings `9`
  - units `17`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `7`
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

- The Unity first production shell is now compile-clean on the runtime side, governed-runtime green, and stronger than the earlier “queue only” slice.
- Manual in-editor feel verification is still the next real gate:
  - queue-row clarity
  - cancel-button feel
  - refund legibility
  - final training feel

## Next Action

1. Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify:
   - unit select and building select
   - drag-box select
   - `1` select-all
   - `Ctrl+2-5` save
   - `2-5` recall
   - `F` frame selection
   - formation-aware move issuance
   - queue-row visibility and cancel-button feel
   - `command_hall -> villager` queue, cancel, refund, and final spawn feel
3. After that, continue into construction placement, broader production-roster verification, deeper production-queue depth beyond cancel, or other command-state work. Do not prioritize attack-move until a real combat lane exists.
