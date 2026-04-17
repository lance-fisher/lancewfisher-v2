# 2026-04-17 Unity Target Acquisition Throttling And Sight Loss

## Goal

Tighten the first Unity combat loop so passive targeting no longer scans every frame and no longer chases stale hostiles indefinitely after they leave sight. This slice stays entirely inside Codex-owned combat scope and builds directly on the completed `codex/unity-attack-move` branch.

## Delivered

- Extended `unity/Assets/_Bloodlines/Code/Combat/CombatStatsComponent.cs` with additive targeting cadence and sight-retention state:
  - `TargetAcquireIntervalSeconds`
  - `AcquireCooldownRemaining`
  - `TargetSightGraceSeconds`
  - `TargetOutOfSightSeconds`
  - helper resolvers for default acquire interval and sight-loss grace
- Extended `unity/Assets/_Bloodlines/Code/Combat/AutoAcquireTargetSystem.cs` so passive acquisition now:
  - decrements a reacquire cooldown every tick
  - only scans for a new passive hostile when the cooldown reaches zero
  - rearms the acquire cooldown after both successful acquires and empty scans
  - leaves explicit attack-order targeting behavior unchanged
- Extended `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs` so non-explicit targets now:
  - track time spent beyond the unit's sight radius
  - keep the hostile for a small grace window instead of dropping immediately on the first out-of-sight frame
  - clear the passive target after the grace window
  - stop stale chase movement when the target is cleared
  - arm the reacquire cooldown after stale-target loss or other invalid target cleanup
- Preserved explicit attack-order semantics:
  - active explicit targets still remain valid beyond sight loss and continue chasing until death or invalidation
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs` with a fifth governed proof phase:
  - passive unit acquires a first hostile
  - the hostile is moved beyond sight
  - stale target clears
  - chase movement stops
  - reacquire cooldown is observed
  - a replacement hostile is acquired only after the throttle delay

## Integration Notes

- This slice is stacked on `codex/unity-attack-move` because Lance delegated merge coordination for the attack-move slice separately to Claude.
- No shared gameplay files outside Codex combat ownership were touched.
- No AI, economy, rendering, shader, or bootstrap runtime smoke files were modified.
- The slice is additive:
  - no existing combat systems were deleted
  - no prior smoke phases were removed or reordered
  - explicit attack orders and attack-move behavior remain intact

## Validation

Green on `codex/unity-target-acquisition-los` with the lane stacked above `codex/unity-attack-move`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session "codex-target-acquisition-los" -WrapperScript "scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1"`
4. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session "codex-target-acquisition-los" -WrapperScript "scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1"`
5. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session "codex-target-acquisition-los" -WrapperScript "scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1"`
6. `node tests/data-validation.mjs`
7. `node tests/runtime-bridge.mjs`

Combat smoke pass lines from `artifacts/unity-combat-smoke.log`:

- `Combat smoke validation melee phase passed: dead='enemy', survivorHealth=6/12, elapsedSeconds=1.2.`
- `Combat smoke validation projectile phase passed: projectileObserved=True, dead='enemy', elapsedSeconds=0.9.`
- `Combat smoke validation explicit attack phase passed: explicitTargetObserved=True, chaseObserved=True, residualTarget=False, dead='enemy', elapsedSeconds=1.5.`
- `Combat smoke validation attack-move phase passed: hostileDead=True, neutralIgnored=True, destinationReached=True, elapsedSeconds=2.2.`
- `Combat smoke validation target-visibility phase passed: sightLossCleared=True, chaseStopped=True, reacquireCooldownObserved=True, replacementTargetObserved=True, reacquireDelayed=True, elapsedSeconds=0.8.`
- `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True.`

Bootstrap runtime smoke stayed green with the full current economy + AI + HUD proofs, including:

- `gatherDepositObserved=True`
- `trickleGainObserved=True`
- `starvationObserved=True`
- `loyaltyDeclineObserved=True`
- `capPressureObserved=True`
- `aiActivityObserved=True`
- `aiConstructionObserved=True`
- `stabilitySurplusObserved=True`

## Branch State

- branch: `codex/unity-target-acquisition-los`
- stacked on: `codex/unity-attack-move`
- stacked base head at slice start: `80e9204d47961411a1b6ca65ca3a689453992a2d`

## Next Action

- Claude or Lance should merge `codex/unity-attack-move` first.
- Then rebase or merge `codex/unity-target-acquisition-los` onto the refreshed `master` tip.
- Do not widen this stacked branch into death presentation, renown or conviction kill hooks, or any shared presentation work without a fresh contract lane for that follow-up.
