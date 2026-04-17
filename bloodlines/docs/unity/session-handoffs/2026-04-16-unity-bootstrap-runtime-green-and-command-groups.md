# 2026-04-16 Unity Bootstrap Runtime Green And Command Groups

## Goal

Push the Unity battlefield shell past structural-only validation by making the governed Bootstrap runtime-smoke lane green, then strengthen the first command shell with battlefield HUD, control groups, framing, and automated validation coverage for those behaviors.

## Work Completed

- Hardened the governed Unity wrappers so the runtime-smoke and scene-validation scripts now wait on the real Unity process instead of returning early.
- Recovered the first live Bootstrap runtime lane:
  - `BloodlinesMapBootstrapAuthoring.cs` now injects canonical bootstrap data into Play Mode when the scene anchor is present and runtime bootstrap data is missing.
  - authoring-side dynamic buffer writes are now safe across structural changes.
  - `SkirmishBootstrapSystem.cs` now snapshots seed buffers before structural changes so spawn execution no longer trips invalidated-buffer exceptions or partial repeat loops.
  - `PositionToLocalTransformSystem.cs` now runs in `SimulationSystemGroup` after `UnitMovementSystem`, removing the remaining invalid Presentation ordering issue.
- Extended `BloodlinesDebugCommandSurface.cs` with:
  - compact battlefield HUD
  - control groups `2` through `5`
  - `Ctrl+2-5` save
  - `2-5` recall
  - `F` frame selection or controlled-force fallback
- Extended `BloodlinesBattlefieldCameraController.cs` with a focus method so command-surface framing can reposition the battlefield camera coherently.
- Extended `BloodlinesBootstrapRuntimeSmokeValidation.cs` so the governed runtime-smoke lane now validates:
  - live bootstrap spawn counts against canonical map expectations
  - debug presentation bridge presence
  - command-shell select-all
  - control-group 2 save/recall
  - command-surface framing

## Verification

- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed.
- Preserved pass log: `artifacts/unity-bootstrap-runtime-smoke.log`
- Latest governed runtime-smoke pass proved:
  - factions `3`
  - buildings `9`
  - units `16`
  - resource nodes `13`
  - control points `4`
  - settlements `2`
  - controlled units `6`
  - command selection after select-all `6`
  - control group 2 count `6`
  - command framing succeeded
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed again after the command-shell changes.
- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 errors and the same longstanding 105 `CS0649` warnings.

## Current Readiness

- The first Unity battlefield shell is now:
  - structurally scene-valid
  - runtime-smoke green
  - command-shell validated for select-all, control-group save/recall, and framing
- The remaining high-value gap is manual in-editor command feel verification plus the next gameplay slice beyond the shell.

## Next Action

1. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify:
   - drag-box selection
   - `1` select-all
   - `Ctrl+2-5` save
   - `2-5` recall
   - `F` frame selection
   - formation-aware move issuance
   - battlefield camera pan, rotate, zoom, and frame feel
3. Verify live control-point capture, contested decay, stabilization, and uncontested trickle in-editor.
4. Continue into construction, production, or deeper command-state work. Do not prioritize attack-move until a real combat lane exists.
