# Unity Player Covenant Test Dispatch

- Date: 2026-04-23
- Lane: `faith-covenant-test`
- Branch: `codex/unity-player-covenant-test-dispatch`
- Status: validated on branch

## Goal

Replace the covenant-test request stub with a real player dispatch seam: the
player should see whether the covenant rite action is available, what it
costs, and whether a request is already queued, while the existing
`CovenantTestResolutionSystem` continues to consume the same one-shot request
entity.

## Browser Reference

- `src/game/core/simulation.js`
- Search anchors:
  - `performCovenantTestAction`
  - `evaluateCovenantTestProgress`
  - `blood_dominion_light_ceremony`
  - `blood_dominion_dark_binding`

## What Changed

- Added `unity/Assets/_Bloodlines/Code/Faith/PlayerCovenantTestDispatchStateComponent.cs`
  so the player faction root now carries covenant-test dispatch availability,
  affordability, cost fields, action label/detail, queued state, and pending
  request state.
- Added `unity/Assets/_Bloodlines/Code/Faith/PlayerCovenantTestDispatchSystem.cs`
  to refresh that dispatch-state surface after qualification, mirror the
  browser's direct-action gate for Blood Dominion light/dark rites, and emit
  `PlayerCovenantTestRequestComponent` only when the player has an available,
  affordable rite action with no request already pending.
- Extended
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Faith.cs`
  with `TryDebugQueueCovenantTestDispatch(...)` and
  `TryDebugGetCovenantTestDispatchState(...)` while preserving the older
  direct `TryDebugTriggerCovenantTest(...)` force-path for the legacy
  covenant-test validator.
- Added
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovenantTestDispatchSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityPlayerCovenantTestDispatchSmokeValidation.ps1`
  to prove ready-phase availability and cost display, queued request
  emission, resolution-system consumption, and unaffordable rite blocking.
- Added the required compile includes to `unity/Assembly-CSharp.csproj` and
  `unity/Assembly-CSharp-Editor.csproj`.

## Validation Proof

- Dedicated player covenant-test dispatch smoke:
  - `BLOODLINES_PLAYER_COVENANT_TEST_DISPATCH_SMOKE PASS`
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

1. Commit and push `codex/unity-player-covenant-test-dispatch`.
2. Merge it onto canonical `master` with `git merge --no-ff` from a clean
   landing worktree.
3. Rerun the full governed 10-gate chain plus the dedicated player
   covenant-test dispatch smoke on the merged result.
4. Move to Priority 19 `codex/unity-contested-territory-pressure`.
