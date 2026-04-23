# 2026-04-23 Unity Faith Verdant Warden

## Slice Summary

- Lane: `faith-verdant-warden`
- Branch: `codex/unity-faith-verdant-warden`
- Browser references:
  - `isVerdantWardenUnit`
  - `getVerdantWardenSupportProfile`
  - `getVerdantWardenZoneSupportProfile`
  - `DEFAULT_VERDANT_WARDEN_SUPPORT`

## What Landed

- Added `VerdantWardenComponent`, `VerdantWardenCoverageProfile`, and
  `VerdantWardenRules` under
  `unity/Assets/_Bloodlines/Code/Faith/VerdantWardenComponent.cs` so Verdant
  Warden units now resolve browser-aligned support radius, capped stacking, and
  explicit multipliers for loyalty, stabilization, reserve healing, reserve
  muster, and frontline defender attack.
- Added `VerdantWardenSupportSystem` under
  `unity/Assets/_Bloodlines/Code/Faith/VerdantWardenSupportSystem.cs`. The
  system lazily tags `verdant_warden` units, counts same-faction living wardens
  around control points and fortified settlements, and writes cached support
  state directly onto `ControlPointComponent` and `FortificationComponent` each
  simulation tick.
- Added
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.VerdantWarden.cs`
  so coverage can be read by control-point id through the explicit
  `TryDebugGetVerdantWardenCoverage(...)` debug seam.
- Added `BloodlinesVerdantWardenSmokeValidation` plus
  `scripts/Invoke-BloodlinesUnityVerdantWardenSmokeValidation.ps1` to prove:
  - a single nearby warden increases owned control-point loyalty and fortification
    reserve healing while remaining visible through the debug readout
  - four nearby wardens store `Count=4` but clamp all applied support math to the
    canonical three-stack cap, including frontline defender attack
- Narrow shared-file seams now consume the live Verdant Warden support state:
  - `ControlPointCaptureSystem.cs` multiplies stabilization and loyalty
    protection with the cached control-point support profile
  - `FortificationReserveSystem.cs` multiplies reserve healing and muster tempo,
    and bumps desired frontline count through the cached fortification support
    profile
  - `AttackResolutionSystem.cs` applies the capped defender attack multiplier to
    engaged fortification-linked defenders only
  - `ControlPointComponent.cs` and `FortificationComponent.cs` now expose the
    cached Verdant Warden count and multiplier fields explicitly instead of
    hiding them in ad hoc transient queries
- `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` now
  explicitly include the new runtime, debug, and editor files for this slice.

## Validation

- Dedicated Verdant Warden smoke: PASS
  - `Verdant Warden smoke validation passed.`
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

- The browser runtime uses loyalty-gain and stabilization multipliers rather than
  a standalone direct loyalty tick, but the directive explicitly called for a
  loyalty bonus. The Unity port keeps the browser multiplier surface and also
  stores a cached per-tick loyalty bonus so the control-point delta remains
  explicit and debuggable in ECS state.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Exact Next Action

1. Claim Priority 8 `faith-exposure-walker` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`.
2. Open `codex/unity-faith-exposure-walker` from updated `master`.
3. Port sacred-site exposure spread plus wayshrine amplification with a
   dedicated smoke validator and the same governed 10-gate rerun.
