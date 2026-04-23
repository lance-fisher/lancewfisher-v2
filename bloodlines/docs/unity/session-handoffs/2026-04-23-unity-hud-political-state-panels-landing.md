# Unity HUD Political State Panels Landing

- Date: 2026-04-23
- Lane: `player-hud-realm-condition-legibility`
- Branch: `codex/unity-hud-political-state-panels`
- Status: merged to canonical `master` and revalidated

## Goal

Land the validated HUD political-state panels slice onto canonical `master`,
rerun the governed validation chain on the merged result, and leave the next
Codex pickup pointed at Priority 18 `player-covenant-test-dispatch`.

## What Landed On Master

- `unity/Assets/_Bloodlines/Code/HUD/SuccessionCrisisHUDComponent.cs` /
  `SuccessionCrisisHUDSystem.cs` now surface live succession severity,
  recovery, and resource pressure on faction-root HUD entities.
- `unity/Assets/_Bloodlines/Code/HUD/PoliticalEventsTrayHUDComponent.cs` /
  `PoliticalEventsTrayHUDSystem.cs` now sort and cap active dynasty political
  events for the player tray without widening runtime ownership beyond the HUD
  lane.
- `unity/Assets/_Bloodlines/Code/HUD/CovenantTestProgressHUDComponent.cs` /
  `CovenantTestProgressHUDSystem.cs` now expose covenant-test phase, elapsed
  progress, and cooldown windows in the player HUD.
- `unity/Assets/_Bloodlines/Code/HUD/TruebornRiseHUDComponent.cs` /
  `TruebornRiseHUDSystem.cs` now project Trueborn stage, recognition, and
  pressure state into the player HUD, including the post-ECB buffer reacquire
  fix surfaced by the new smoke.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now exposes parseable snapshot helpers for all four political-state panels.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPoliticalStateHUDSmokeValidation.cs`
  plus `scripts/Invoke-BloodlinesUnityPoliticalStateHUDSmokeValidation.ps1`
  now live on canonical `master` as the dedicated proof surface.
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj`
  now retain the new compile includes and canonical
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` analyzer roots on
  the merged master-compatible line.

## Validation Proof On Merge Result

- Runtime build:
  - `Build succeeded.`
- Editor build:
  - `Build succeeded.` with existing repo-wide warnings only
- Bootstrap runtime:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Combat smoke validation passed.`
- Scene shells:
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
  - `Bloodlines canonical Unity scene-shell validation sequence completed successfully.`
- Fortification smoke:
  - `Fortification smoke validation passed.`
- Siege smoke:
  - `Siege smoke validation passed.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Dedicated smoke on merge result:
  - `Political state HUD smoke validation passed.`

## Exact Next Action

1. Start the next fresh `codex/unity-player-covenant-test-dispatch` branch
   from the updated canonical `master`.
2. Read the covenant dispatch seam in `src/game/core/simulation.js` and the
   current Unity faith/player-dispatch surfaces, then port the next additive
   non-AI slice with its own dedicated smoke validator.
