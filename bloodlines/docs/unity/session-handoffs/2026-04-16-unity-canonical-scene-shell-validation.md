# 2026-04-16 Unity Canonical Scene-Shell Validation Coverage

## Goal

Extend the governed Unity scene-shell tooling so both canonical scene shells have explicit structural validation coverage, not just `Bootstrap.unity`, and preserve the result outside chat.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGameplaySceneBootstrap.cs` so scene validation now covers both:
  - `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
  - `Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity`
- Added the governed Gameplay scene validation entry points:
  - menu: `Bloodlines -> Scenes -> Validate Gameplay Scene Shell`
  - batch method: `RunBatchValidateGameplaySceneShell()`
- Refactored scene validation into one shared structural validator that now checks:
  - required scene metadata root and descriptor object
  - battlefield camera shell
  - debug presentation bridge
  - debug command surface
  - reference ground
  - Bootstrap-only map authoring and canonical map assignment
  - Gameplay-only absence of bootstrap authoring
- Added the governed scene-validation wrappers:
  - `scripts/Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
- Hardened the Bootstrap and Gameplay validation wrappers so they rerun once automatically if the first Unity batch pass is consumed by script compilation or import work before a validation outcome is logged.

## Verification

- `scripts/Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1` passed and logged:
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- `scripts/Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1` passed and logged:
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` completed successfully.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed through isolated Codex intermediate/output paths with 0 errors and the same longstanding 105 `CS0649` warnings.

## Current Readiness

- The canonical Unity scene-shell lane is now structurally validated in batch mode.
- The highest-value remaining blocker is still live Play Mode verification inside the Unity editor.

## Next Action

1. Run `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` whenever structural shell integrity needs a quick preflight.
2. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
3. Enter Play Mode and verify live spawning, camera behavior, selection, formation-aware move, capture flow, and uncontested trickle together as one shell.
4. After that first live shell is confirmed, continue directly into attack-move or richer command-state UX.
