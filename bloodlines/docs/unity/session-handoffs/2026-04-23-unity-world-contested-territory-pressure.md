# Unity World Contested Territory Pressure

- Date: 2026-04-23
- Lane: `world-contested-territory-pressure`
- Branch: `codex/unity-contested-territory-pressure-rerun`
- Status: validated on branch

## Goal

Port the contested-territory pressure seam as an additive ECS read-model that
matches the browser's realm-condition and territorial-governance behavior
without inventing a new loyalty-drain or world-pressure score source.

## Browser Reference

- `src/game/core/simulation.js`
- Search anchors:
  - `getRealmConditionSnapshot`
  - `getTerritorialGovernanceAcceptanceProfile`
  - `getTerritorialGovernanceWorldPressureContribution`
  - `getRivalContactProfile`
- `tests/runtime-bridge.mjs`
  - contested control-point setup in the territorial-governance stage-4 prep
  - realm-condition snapshot assertions for contested territories

## What Changed

- Added `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialPressureComponent.cs`
  so kingdom faction roots now carry external contested-territory count,
  owned contested-territory count, weakest owned contested march, and a
  governance-blocking flag.
- Added
  `unity/Assets/_Bloodlines/Code/WorldPressure/TerritorialPressureEvaluationSystem.cs`
  to project the browser's two contested-territory seams every frame:
  non-owned `captureProgress > 0` for realm-condition pressure, and owned
  contested marches for governance-recognition blocking.
- Extended
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs`
  with `TryDebugGetTerritorialPressureState(...)` and widened the existing
  governance readout with contested-count / hold-ready fields for smoke
  inspection.
- Added
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesContestedTerritoryPressureSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityContestedTerritoryPressureSmokeValidation.ps1`
  to prove:
  - external contested territories are counted without changing world-pressure score
  - owned contested marches block governance hold readiness while an otherwise
    equivalent uncontested peer remains ready
  - clearing the contest clears the read-model and the blocker
- Added the required compile includes to `unity/Assembly-CSharp.csproj` and
  `unity/Assembly-CSharp-Editor.csproj`.

## Validation Proof

- Dedicated contested-territory pressure smoke:
  - `Contested territory pressure smoke validation passed.`
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

## Notes

- The stale historical `codex/unity-contested-territory-pressure` branch was
  not reused. This rerun starts from the current canonical `master` line and
  keeps the port browser-faithful by treating contested territory as a
  read-model / readiness seam rather than a new direct loyalty-drain mechanic.
- Fresh automation worktrees may still need a local `unity/Library` junction
  to `D:\ProjectsHome\Bloodlines\unity\Library` before the governed `dotnet build`
  gates can resolve `Library\ScriptAssemblies`.
- Keep
  `unity/ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json`
  unstaged; Unity dirties it during validation and it is unrelated to this
  slice.

## Exact Next Action

1. Commit and push `codex/unity-contested-territory-pressure-rerun`.
2. Merge it onto canonical `master` with `git merge --no-ff`.
3. Record the landing handoff and clear the branch-in-flight state in the
   concurrent-session contract.
4. Claim the next unblocked non-AI Codex lane from the migration plan after
   master is refreshed.
