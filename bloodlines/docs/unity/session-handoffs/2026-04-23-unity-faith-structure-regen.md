# 2026-04-23 Unity Faith Structure Regen

## Slice Summary

- Lane: `faith-structure-regen`
- Branch: `codex/unity-faith-structure-regen`
- Browser references:
  - `getCompletedFaithBuildings`
  - `getFaithStructureIntensityRegenRate`
  - `updateFaithStructureIntensity`

## What Landed

- Added `FaithStructureRegenComponent` and `FaithStructureRegenRules` under
  `unity/Assets/_Bloodlines/Code/Faith/FaithStructureRegenComponent.cs` so
  committed factions now cache their last-applied regen day, current capped
  rate, building count, and last applied intensity delta in a dedicated ECS
  seam.
- Added `FaithStructureRegenSystem` under
  `unity/Assets/_Bloodlines/Code/Faith/FaithStructureRegenSystem.cs`. The
  system scans completed same-faction faith buildings by `BuildingTypeComponent`
  id, sums the canonical `wayshrine` / `covenant_hall` / `grand_sanctuary` /
  `apex_covenant` `faithIntensityRegenBonus` values, clamps the result at the
  browser `1.4` per-second ceiling, and applies the gain to committed faction
  intensity on whole in-world day boundaries while preserving browser
  per-second scaling through `DualClockComponent.DaysPerRealSecond`.
- Added `BloodlinesFaithStructureRegenSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityFaithStructureRegenSmokeValidation.ps1` to
  prove:
  - three completed faith structures regenerate committed intensity faster than
    a single wayshrine
  - stacked faith structures clamp at the canonical `1.4` regen-rate ceiling
    regardless of building count
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new runtime and editor files for this slice.

## Validation

- Dedicated faith structure regen smoke: PASS
  - `Faith structure regen smoke validation passed.`
- Runtime build: PASS
  - `Build succeeded.`
- Editor build: PASS
  - `Build succeeded.`
- Governed full validation chain: PASS
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

## Notes

- The browser seam sums `faithIntensityRegenBonus` from all completed faith
  buildings, including `apex_covenant`, so the Unity port resolves the bonus
  directly from the canonical building ids instead of introducing a parallel
  shared building-definition rewrite.
- Checked-in Unity wrappers are still mixed between worktree-relative and
  canonical-root project paths. The governed validation rerun therefore passed
  through `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1` using the local
  short-path worktree, while bootstrap runtime and scene-shell validation still
  executed against `D:\ProjectsHome\Bloodlines`.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Exact Next Action

1. Claim Priority 10 `player-captive-ransom-trickle` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-23.md`.
2. Open `codex/unity-player-captive-ransom-trickle` from updated `master`.
3. Port passive captive influence / renown trickle plus a dedicated smoke
   validator and the same governed 10-gate rerun.
