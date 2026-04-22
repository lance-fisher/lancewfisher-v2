# Unity Player HUD Fortification Readout

Date: 2026-04-22
Branch: `codex/unity-player-hud-fortification-readout`
Lane: `player-hud-realm-condition-legibility`
Status: branch-complete, blocked on worktree `dotnet build` gate

## Goal

Port the fortification legibility block from the browser's realm-condition snapshot into the active Codex HUD lane so settlements expose breach, reserve, sealing, and destroyed-counter recovery state through a dedicated Unity ECS read-model.

## Browser Reference

- `src/game/core/simulation.js`
  - `getRealmConditionSnapshot` (`14291-14764`)
  - fortification block (`14568-14654`)
  - primary keep selection and fortification context (`14346-14362`)
- `tests/runtime-bridge.mjs`
  - fortification snapshot shape assertions (`1344-1364`)
  - commander / sortie fortification assertions (`1438-1444`)
  - reserve snapshot assertions (`710-713`)
  - imminent-engagement fortification assertions (`8006-8053`, `8075-8084`)

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDComponent.cs` as a settlement-level read-model carrying:
  - settlement identity and ownership
  - tier / ceiling
  - breach and destroyed-counter state
  - reserve frontage, mustered defenders, reserve buckets, and threat state
  - sealing progress and destroyed-counter recovery progress with live worker-hour and stone telemetry
- Added `unity/Assets/_Bloodlines/Code/HUD/FortificationHUDSystem.cs`.
  - Uses `ISystem`, `EntityCommandBuffer`, and `NativeArray` snapshots of linked fortification combatants.
  - Projects fortification state without mutating simulation ownership surfaces.
  - Computes reserve frontage from live linked defenders in radius, matching the existing breach debug seam's frontline logic.
  - Reuses the existing fortification canon constants for sealing and recovery cost/progress projection.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs` with:
  - `TryDebugGetFortificationHUDSnapshot(string settlementId, out string readout)`
  - parseable `FortHUD|Key=Value|...` output for validator and future HUD integration work
- Added dedicated validator:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationHUDSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityFortificationHUDSmokeValidation.ps1`
- Updated `unity/Assembly-CSharp.csproj` and `unity/Assembly-CSharp-Editor.csproj` compile includes for the new HUD runtime/editor files.

## Validation

### Slice Proof

- Dedicated fortification HUD smoke:
  - `BLOODLINES_FORTIFICATION_HUD_SMOKE PASS`
  - `Phase 1 PASS: primary keep surfaces tier 2, reserve frontage 2, mustered defenders 2, and active reserve pressure.`
  - `Phase 2 PASS: open breach projects sealing progress 0.500 with tier-3 cost 18 worker-hours and 135 stone.`
  - `Phase 3 PASS: destroyed wall recovery projects progress 0.500 with canonical 14 worker-hours and 90 stone.`
  - `Phase 4 PASS: primary keep recovery projects keep-multiplied progress 0.500 with canonical 28 worker-hours and 180 stone.`

### Governed Gates

- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `Bootstrap runtime smoke validation passed.`
- `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - Unity wrapper exited `0`; existing combat smoke remained green.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - `Bootstrap scene shell validation passed.`
  - `Gameplay scene shell validation passed.`
- `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - `Fortification smoke validation passed.`
- `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
  - Unity wrapper exited `0`; existing siege smoke remained green.
- `node tests/data-validation.mjs`
  - `Bloodlines data validation passed.`
- `node tests/runtime-bridge.mjs`
  - `Bloodlines runtime bridge validation passed.`
- `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
  - `STALENESS CHECK PASSED: Contract revision=70, last-updated=2026-04-21 is current.`

### Blocker

- `dotnet build unity/Assembly-CSharp.csproj -nologo`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`

Both `dotnet build` gates fail in this worktree before slice-specific compilation, with broad unresolved Unity assembly references (`Unity.Entities`, `Unity.Mathematics`, etc.) across many pre-existing files. Unity's own compile path is green for this slice, but the canonical branch gate remains red until the worktree's generated C# project references build cleanly under `dotnet`.

## Unity-Side Simplifications Deferred

- This slice is still a read-model and debug-surface seam only. It does not yet render an on-screen fortification panel.
- The HUD read-model currently focuses on the fields needed for branch proof and future UI integration; it does not yet mirror every browser fortification subfield such as sortie timers, ward bonus percentages, or imminent-engagement posture data because those already exist on the fortification debug seam and can be folded into a later HUD presentation pass.
- No victory-distance readout was attempted in this slice.

## Immediate Next Action

1. Repair or regenerate the worktree `dotnet` project-reference state so `unity/Assembly-CSharp*.csproj` builds stop failing on unresolved Unity assemblies.
2. Re-run the full 10-gate chain on `codex/unity-player-hud-fortification-readout`.
3. If the branch goes fully green, commit and push this fortification HUD slice; otherwise preserve it as WIP and continue with the same blocker documented.
