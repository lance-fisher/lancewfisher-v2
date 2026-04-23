# 2026-04-23 Unity Faith Exposure Walker

## Slice Summary

- Lane: `faith-exposure-walker`
- Branch: `codex/unity-faith-exposure-walker-sacred-site`
- Browser references:
  - `updateFaithExposure`
  - `getWayshrineExposureMultiplierAt`

## What Landed

- Added `SacredSiteExposureSourceComponent`,
  `FaithExposureStructureComponent`,
  `FaithExposureStructureRole`,
  and
  `FaithExposureWalkerRules`
  under
  `unity/Assets/_Bloodlines/Code/Faith/FaithExposureWalkerComponent.cs`
  so sacred-site sources, multiplier-cap rules, and canonical wayshrine /
  covenant-hall / grand-sanctuary profiles live in one ECS-owned seam.
- Added `FaithExposureWalkerSystem` under
  `unity/Assets/_Bloodlines/Code/Faith/FaithExposureWalkerSystem.cs`.
  The system lazily tags completed faith structures, finds living same-faction
  units inside each sacred-site radius, rejects non-kingdom factions, applies
  browser-aligned wayshrine amplification with a four-contributor / `4.0x`
  clamp, and records exposure through `FaithScoring.RecordExposure`.
- Added
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.ExposureWalker.cs`
  so the live seam can be inspected through
  `TryDebugGetSacredSiteExposureSnapshot(...)`, including the stored exposure,
  discovery flag, multiplier, and contributor count.
- Added `BloodlinesFaithExposureWalkerSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityFaithExposureWalkerSmokeValidation.ps1` to
  prove:
  - a living kingdom unit inside a sacred site gains base exposure and
    discovers the faith
  - a completed wayshrine multiplies the same exposure from `10` to `18` with a
    `1.8x` multiplier
  - an under-construction shrine does not count and a non-kingdom faction gains
    no exposure
  - stacked shrine / hall / sanctuary contributors clamp at the canonical
    `4.0x` ceiling with four counted contributors
- Narrow shared-file seams now materialize sacred sites during bootstrap only:
  - `MapBootstrapComponents.cs` now carries a dedicated
    `MapSacredSiteSeedElement`
  - `BloodlinesMapBootstrapAuthoring.cs` and
    `BloodlinesMapBootstrapBaker.cs` now ingest and bake `map.sacredSites`
  - `SkirmishBootstrapSystem.cs` now spawns sacred-site ECS entities from that
    seed buffer
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new runtime, debug, and editor files for this slice.

## Validation

- Dedicated faith exposure walker smoke: PASS
  - `Faith exposure walker smoke validation passed.`
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

- The browser seam does not require new faith-building tags in shared
  definitions. The Unity port keeps the slice narrower by resolving
  `wayshrine`, `covenant_hall`, and `grand_sanctuary` directly from
  `BuildingTypeComponent.TypeId`, then lazily adding the local
  `FaithExposureStructureComponent` tag at runtime.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Exact Next Action

1. Claim Priority 9 `faith-structure-regen` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`.
2. Open `codex/unity-faith-structure-regen` from updated `master` or extend
   this lane if continuity pressure requires it.
3. Port passive faith-structure intensity regeneration plus a dedicated smoke
   validator and the same governed 10-gate rerun.
