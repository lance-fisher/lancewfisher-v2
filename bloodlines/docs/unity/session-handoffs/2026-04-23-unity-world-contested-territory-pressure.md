# Unity Contested Territory Pressure

- Date: 2026-04-23
- Lane: `world-contested-territory`
- Branch: `codex/unity-contested-territory-pressure`
- Status: validated on branch

## Goal

Port the directive-defined contested-territory seam into Unity ECS so owned
frontier control points become meaningfully unstable when two or more hostile
factions project live claim pressure into the same radius.

## Browser Reference

- `src/game/core/simulation.js`
- Search anchors attempted:
  - `CONTESTED_TERRITORY_STABILITY_PENALTY`
  - `getContestScore`
  - `updateContestedTerritories`
  - `isContested`
  - `contestedTerritory`
- Result:
  - no direct contested-territory pressure function was present in the frozen
    browser runtime surface, so this slice follows the design canon recorded in
    the 2026-04-24 multi-day directive while preserving the browser control
    point capture/stabilization structure.

## What Changed

- Added
  `unity/Assets/_Bloodlines/Code/WorldPressure/ContestedTerritoryComponent.cs`
  so owned control points can carry live contested-frontier pressure state:
  hostile-faction count, per-day stability penalty, loyalty volatility
  multiplier, and the in-world day the contest began.
- Added
  `unity/Assets/_Bloodlines/Code/WorldPressure/ContestedTerritoryEvaluationSystem.cs`
  to scan live combat units around each owned control point, count distinct
  hostile factions inside claim radius, materialize/remove the contested state,
  and preserve the original contest start day while pressure remains active.
- Extended
  `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs` so
  contested owned points now lose loyalty in proportion to the contested
  stability penalty and apply the contested loyalty-volatility multiplier
  through the existing governor and Verdant Warden protection seams without
  touching `unity/Assets/_Bloodlines/Code/AI/**`.
- Extended
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs`
  with `TryDebugGetContestState(controlPointId)` for parseable contested
  frontier readouts.
- Added the dedicated validator
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesContestedTerritoryPressureSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityContestedTerritoryPressureSmokeValidation.ps1`
  to prove hostile-flank detection, contested loyalty degradation versus an
  uncontested peer, and automatic contest clearing when hostile forces
  withdraw.
- Added the required compile includes to `unity/Assembly-CSharp.csproj` and
  `unity/Assembly-CSharp-Editor.csproj`.

## Validation Proof

- Dedicated contested-territory smoke:
  - `BLOODLINES_CONTESTED_TERRITORY_PRESSURE_SMOKE PASS`
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

1. Commit and push `codex/unity-contested-territory-pressure`.
2. Merge it onto canonical `master` with `git merge --no-ff` from a clean
   landing worktree.
3. Rerun the full governed 10-gate chain plus the dedicated contested-territory
   smoke on the merged result.
4. Move to Priority 20 `codex/unity-player-succession-influence`.
