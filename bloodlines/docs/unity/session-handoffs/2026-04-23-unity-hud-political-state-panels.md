# Unity HUD Political State Panels

- Date: 2026-04-23
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-hud-political-state-panels`
- Status: validated on branch

## Goal

Project the browser dynasty political-state seam into the player HUD without
touching `unity/Assets/_Bloodlines/Code/AI/**`: succession crisis severity,
active political events, covenant test progress, and Trueborn rise pressure
all need player-facing read-models plus a dedicated Unity smoke validator.

## Browser Reference

- `src/game/core/simulation.js`
- Search anchors:
  - `getDynastyPoliticalEventState`
  - `getActiveSuccessionCrisis`
  - `getActiveCovenantTest`
  - `tickTruebornRiseArc`

## What Changed

- Added four additive HUD read-model pairs under
  `unity/Assets/_Bloodlines/Code/HUD/`:
  - `SuccessionCrisisHUDComponent` / `SuccessionCrisisHUDSystem`
  - `PoliticalEventsTrayHUDComponent` / `PoliticalEventsTrayHUDSystem`
  - `CovenantTestProgressHUDComponent` / `CovenantTestProgressHUDSystem`
  - `TruebornRiseHUDComponent` / `TruebornRiseHUDSystem`
- Extended
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  with parseable snapshot helpers for all four panel sections.
- Added
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPoliticalStateHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPoliticalStateHUDSmokeValidation.ps1`
  to prove succession severity/recovery, event-tray sort and cap, covenant
  cooldown/phase state, and Trueborn recognition pressure.
- Repaired this worktree's build surface by restoring `unity/Library` as a
  junction to `D:\ProjectsHome\Bloodlines\unity\Library` and canonicalizing
  stale `Assembly-CSharp*.csproj` analyzer roots back to
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache`.
- Fixed a real runtime issue exposed by the new smoke:
  `TruebornRiseHUDSystem` now reacquires the recognition buffer after ECB
  playback instead of reading an invalidated pre-structural-change handle.

## Validation Proof

- Dedicated political-state HUD smoke:
  - `BLOODLINES_POLITICAL_STATE_HUD_SMOKE PASS`
- Governed validation chain:
  - `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`

## Exact Next Action

1. Commit and push `codex/unity-hud-political-state-panels`.
2. Merge it onto canonical `master` with `git merge --no-ff` from a clean
   landing worktree.
3. Rerun the full governed 10-gate chain plus the dedicated political-state
   HUD smoke on the merged result.
4. Move to Priority 18 `codex/unity-player-covenant-test-dispatch`.
