# 2026-04-22 Unity Faith Covenant Test

## Slice Summary

- Lane: `faith-covenant-test`
- Branch: `codex/unity-faith-covenant-test`
- Browser references:
  - `src/game/core/simulation.js` constants `COVENANT_TEST_INTENSITY_THRESHOLD`, `COVENANT_TEST_DURATION_IN_WORLD_DAYS`, and `COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS`
  - `src/game/core/simulation.js` covenant-test cost block around lines 134-141
  - `src/game/core/simulation.js` `performCovenantTestAction`
  - `src/game/core/simulation.js` `ensureFaithCovenantTestCompletionFromLegacyState`

## What Landed

- Added `CovenantTestStateComponent`, `PlayerCovenantTestRequestComponent`, `CovenantTestQualificationSystem`, and `CovenantTestResolutionSystem` under `Faith/**` so faction roots now track covenant-test qualification, explicit trigger requests, success completion, and failure cooldown application.
- Added `BloodlinesCovenantTestSmokeValidation` plus `Invoke-BloodlinesUnityCovenantTestSmokeValidation.ps1` to prove qualification after a full 180-day hold, successful trigger completion, failed trigger penalties plus cooldown creation, and retry blocking while `CovenantTestCooldown` is active.
- `BloodlinesDebugCommandSurface.Faith.cs` now exposes `TryDebugTriggerCovenantTest(...)`, `TryDebugGetCovenantTestState(...)`, and `TryDebugSetFaithIntensity(...)` for parseable validator and future UI seams.
- `PlayerDivineRightDeclarationSystem.cs` now requires `CovenantTestStateComponent.TestPhase == Complete` before the player divine-right declaration path can proceed.
- `BloodlinesPlayerHolyWarDivineRightSmokeValidation.cs` now seeds a completed covenant-test state so the already-landed divine-right validator continues to prove its intended seam rather than failing on the new prerequisite.
- `Assembly-CSharp.csproj` and `Assembly-CSharp-Editor.csproj` now include the new runtime/editor compile entries, and the stale Unity.Entities analyzer references were normalized to the canonical `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` root after local generated metadata still pointed at a dead outside checkout.

## Validation

- Dedicated covenant-test smoke: PASS
- Governed 10-gate chain: PASS
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

- The first dedicated validator pass exposed harness issues rather than gameplay regressions: the dual-clock query needed read-write access, the validator needed to preseed dynasty political-event aggregate state to avoid a structural-change trap inside `DynastyPoliticalEventSystem`, and phases 2/3 needed to prove a real 180-day hold instead of jumping straight to day 180.
- `CovenantTestRules.ResolveCostProfile(...)` currently carries explicit browser-parity costs for Blood Dominion light/dark only; other covenants fall back to zero-cost/default profile until their covenant-specific cost shapes are ported from the browser constants block.
- The next clean additive pickup from the directive stack is Priority 4 `territory-governor-specialization`.
