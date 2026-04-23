# 2026-04-23 Unity Dynasty Succession Crisis Rerun

## Slice Summary

- Lane: `dynasty-succession-crisis`
- Branch: `codex/unity-dynasty-succession-crisis-rerun`
- Browser references:
  - `SUCCESSION_CRISIS_SEVERITY_PROFILES`
  - `buildSuccessionCrisisTriggerProfile`
  - `startSuccessionCrisis`
  - `tickDynastyPoliticalEvents`
  - `getSuccessionCrisisTerms`
  - `consolidateSuccessionCrisis`

## What Landed

- Replayed the already-designed succession-crisis ECS slice onto the current
  master line instead of merging the stale historical
  `codex/unity-dynasty-succession-crisis` branch, which carried unrelated
  backwards diffs against current master.
- `SuccessionCrisisComponent`, `SuccessionCrisisEvaluationSystem`, and
  `SuccessionCrisisRecoverySystem` remain the runtime seam for ruler-change
  crisis watching, browser-aligned severity evaluation, opening loyalty shock,
  whole-day legitimacy and loyalty drain, resource throttling, and
  conviction-aware recovery removal.
- `BloodlinesSuccessionCrisisSmokeValidation` plus
  `Invoke-BloodlinesUnitySuccessionCrisisSmokeValidation.ps1` remain the
  dedicated proof surface. The wrapper is now hardened to poll the Unity log
  for explicit pass/fail markers and retry once if batchmode exits before the
  marker is flushed.
- Local `Assembly-CSharp.csproj` and `Assembly-CSharp-Editor.csproj` were
  repaired so the generated analyzer roots now point at the canonical
  `D:\ProjectsHome\Bloodlines\unity\Library\PackageCache` surface instead of
  the dead `D:\BLGOV\...` checkout.
- This worktree had no `unity/Library` directory, so the governed `dotnet build`
  gate required restoring a local junction from this worktree's `unity/Library`
  to `D:\ProjectsHome\Bloodlines\unity\Library` before `Library\ScriptAssemblies`
  references could resolve.

## Validation

- Dedicated succession crisis smoke: PASS
  - `Succession crisis smoke validation passed.`
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

- The prompt-requested `03_PROMPTS/CODEX_MULTI_DAY_DIRECTIVE_2026-04-23.md`
  is absent in the canonical root. `03_PROMPTS/CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`
  explicitly supersedes it and was the operative priority stack for this rerun.
- `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  still dirties during Unity validation and should remain unstaged.

## Exact Next Action

1. Merge this validated rerun branch to canonical `master` and push both refs.
2. Claim Priority 6 `fortification-postures` from
   `D:\ProjectsHome\Bloodlines\03_PROMPTS\CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md`.
3. Open `codex/unity-fortification-postures` from updated `master` and port
   imminent-engagement fortification postures plus a dedicated smoke validator.
