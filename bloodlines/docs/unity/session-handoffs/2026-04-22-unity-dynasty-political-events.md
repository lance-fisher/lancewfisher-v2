# 2026-04-22 Unity Dynasty Political Events

## Slice Summary

- Lane: `dynasty-political-events`
- Branch: `codex/unity-dynasty-political-events`
- Browser references:
  - `src/game/core/simulation.js` `getDynastyPoliticalEventState` (~3126)
  - `src/game/core/simulation.js` `tickDynastyPoliticalEvents` (~3950)
  - `src/game/core/simulation.js` `getFactionPoliticalEventEffects` (~3833)
  - `src/game/core/simulation.js` `failDivineRightDeclaration` (~10691)
  - `src/game/core/simulation.js` constants around `COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS` and the divine-right failure penalties/cooldown block (~119-121, ~9783-9785)

## What Landed

- Added `DynastyPoliticalEventComponent` as a faction-root dynamic buffer plus `DynastyPoliticalEventAggregateComponent` as the composite read-model for active timed dynasty effects.
- Added `DynastyPoliticalEventSystem` to expire timed events, project aggregate multipliers, and convert failed divine-right upkeep into a `DivineRightFailedCooldown` political event when faith intensity or faith level drops below the declaration thresholds.
- Added `BloodlinesDynastyPoliticalEventsSmokeValidation` plus `Invoke-BloodlinesUnityDynastyPoliticalEventsSmokeValidation.ps1` to prove aggregate application, divine-right failure cooldown creation, and expiry/reset behavior in isolated ECS worlds.
- Narrow shared-file hooks landed:
  - `PlayerDivineRightDeclarationSystem.cs` now blocks new declarations while the failed divine-right cooldown event is active.
  - `AttackResolutionSystem.cs` now multiplies outgoing attack damage by the faction political-event aggregate attack multiplier.
  - `ResourceTrickleBuildingSystem.cs` and `ControlPointResourceTrickleSystem.cs` now multiply territorial/building trickle by the faction political-event aggregate resource factor.
  - `ControlPointCaptureSystem.cs` now multiplies passive and active stabilization by the faction political-event aggregate stabilization multiplier.
  - `BloodlinesDebugCommandSurface.Dynasty.cs` now exposes `TryDebugGetPoliticalEvents(...)` with a parseable `type@expiry` summary.

## Validation

- Dedicated dynasty political events smoke: PASS
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

- The first dedicated smoke run exposed a validator harness bug rather than runtime logic: the ECS system was never added to the validation world's simulation group, and the batch execute method did not explicitly exit Unity on success. Both issues were corrected before final validation.
- The current slice keeps covenant-test cooldown as a placeholder hook point only. The next clean pickup from the directive stack is `codex/unity-faith-covenant-test`.
