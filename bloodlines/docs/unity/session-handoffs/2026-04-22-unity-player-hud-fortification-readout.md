# Unity Player HUD Fortification Readout

Date: 2026-04-22
Branch: `codex/unity-player-hud-fortification-readout-rerun`
Lane: `player-hud-realm-condition-legibility`
Status: complete on branch, all governed gates green

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
  - `Unity exited with code 0`
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
  - `STALENESS CHECK PASSED: Contract revision=71, last-updated=2026-04-22 is current.`
- `dotnet build unity/Assembly-CSharp.csproj -nologo`
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`

### Worktree Repair Note

- The original blocked branch carried generated Unity project files whose
  analyzer paths still pointed at another Codex worktree
  (`...worktrees\\b6c2\\...`) and this worktree had not yet hydrated
  `unity/Library/PackageCache`.
- Running Unity batch validation in the current worktree materialized the local
  `Library` / `PackageCache` state, after which both canonical
  `dotnet build unity/Assembly-CSharp*.csproj -nologo` gates passed cleanly
  without runtime-code changes.
- This rerun branch supersedes the earlier blocked state and is the
  commit-ready continuation of the fortification HUD slice.

## Unity-Side Simplifications Deferred

- This slice is still a read-model and debug-surface seam only. It does not yet render an on-screen fortification panel.
- The HUD read-model currently focuses on the fields needed for branch proof and future UI integration; it does not yet mirror every browser fortification subfield such as sortie timers, ward bonus percentages, or imminent-engagement posture data because those already exist on the fortification debug seam and can be folded into a later HUD presentation pass.
- No victory-distance readout was attempted in this slice.

## Immediate Next Action

1. Commit and push `codex/unity-player-hud-fortification-readout-rerun` as the validated fortification HUD slice.
2. After push, keep the player-HUD lane active and start the remaining victory-distance readout follow-up on a fresh Codex branch.
3. Continue treating `unity/Assets/_Bloodlines/Code/AI/**` as read-only while the HUD lane consumes those systems only through cross-lane reads.
