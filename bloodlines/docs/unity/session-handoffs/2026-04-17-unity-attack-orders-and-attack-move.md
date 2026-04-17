# 2026-04-17 Unity Attack Orders And Attack-Move

## Goal

Give the player commandable combat on current Unity `master` by landing the missing explicit attack-order and attack-move slice without touching the active Claude graphics lane, the retired AI lane, or the governed bootstrap smoke surface.

## Delivered

- Added `unity/Assets/_Bloodlines/Code/Combat/AttackOrderComponent.cs` with the contract fields:
  - `ExplicitTargetEntity`
  - `AttackMoveDestination`
  - `IsAttackMoveActive`
  - `IsActive`
- Preserved compatibility with the already-merged older attack-order lane by keeping the additive compatibility property:
  - `IsAttackMoveDestination` forwards to `IsAttackMoveActive`
- Added `unity/Assets/_Bloodlines/Code/Combat/AttackOrderResolutionSystem.cs`:
  - runs in `SimulationSystemGroup`
  - updates before `AutoAcquireTargetSystem`
  - translates explicit orders and attack-move orders into the existing `MoveCommandComponent` and `AttackTargetComponent` flow
  - explicit target orders now chase commanded hostiles until death or invalidation
  - attack-move now marches toward destination, interrupts to fight acquired hostiles, then resumes marching after the hostile dies
- Extended `unity/Assets/_Bloodlines/Code/Combat/AutoAcquireTargetSystem.cs` minimally so:
  - active explicit targets are respected and written through as the live `AttackTargetComponent`
  - otherwise the previous nearest-hostile auto-acquire behavior stays intact
- Extended `unity/Assets/_Bloodlines/Code/Combat/DeathResolutionSystem.cs` so:
  - dead units lose any `AttackOrderComponent`
  - attackers targeting a killed explicit target have that explicit order cleared immediately
  - attack-move orders survive local kills so destination march can resume
- Marked the older already-merged `unity/Assets/_Bloodlines/Code/Combat/AttackOrderSystem.cs` as `[DisableAutoCreation]` so the rebased contract-shaped system is the only active attack-order resolver on current `master`
- Added `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Combat.cs` as the required new partial class file:
  - `TryDebugIssueAttackOrderOnNearestHostile(factionId, out bool issued)`
  - `TryDebugIssueAttackMove(float3 destination, out int orderedCount)`
  - helper that resolves the nearest hostile from the current combat selection without duplicating the already-merged `BloodlinesDebugCommandSurface.AttackOrders.cs` input surface
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs` from two phases to four:
  - melee phase
  - projectile phase
  - explicit attack-order phase
  - attack-move phase with a neutral decoy that must remain ignored

## Important Integration Notes

- Current `master` already contains an older merged attack-order lane from `codex/unity-attack-orders-attack-move`.
- This branch rebased that older lane onto current `master` additively instead of replacing it:
  - the existing `BloodlinesDebugCommandSurface.AttackOrders.cs` remains the input owner for right-click hostile targeting and `A` attack-move arming
  - the new `BloodlinesDebugCommandSurface.Combat.cs` only adds the governed debug APIs required by the contract
  - the existing `AttackOrderSystem` stays on disk but is disabled from auto-creation so it cannot compete with `AttackOrderResolutionSystem`
- The combat smoke and bootstrap runtime smoke were rerun through the canonical wrappers under the required wrapper lock session:
  - `codex-attack-move`
- No forbidden paths were touched:
  - no `Code/AI/**`
  - no `Code/Economy/**`
  - no `Code/Rendering/**`
  - no shader lane files
  - no bootstrap runtime smoke validator edits

## Validation

Green on `codex/unity-attack-move` after rebase onto `548d7804ce55766420d75184385b3bedb739a3ee`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session "codex-attack-move" -WrapperScript "scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1"`
4. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session "codex-attack-move" -WrapperScript "scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1"`
5. `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session "codex-attack-move" -WrapperScript "scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1"`
6. `node tests/data-validation.mjs`
7. `node tests/runtime-bridge.mjs`

Combat smoke pass lines from `artifacts/unity-combat-smoke.log`:

- `Combat smoke validation melee phase passed: dead='enemy', survivorHealth=6/12, elapsedSeconds=1.2.`
- `Combat smoke validation projectile phase passed: projectileObserved=True, dead='enemy', elapsedSeconds=0.9.`
- `Combat smoke validation explicit attack phase passed: explicitTargetObserved=True, chaseObserved=True, residualTarget=False, dead='enemy', elapsedSeconds=1.5.`
- `Combat smoke validation attack-move phase passed: hostileDead=True, neutralIgnored=True, destinationReached=True, elapsedSeconds=2.2.`
- `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True.`

Bootstrap runtime smoke preserved the current economy + AI + HUD proofs on the same branch tip:

- `gatherDepositObserved=True`
- `trickleGainObserved=True`
- `starvationObserved=True`
- `loyaltyDeclineObserved=True`
- `capPressureObserved=True`
- `aiActivityObserved=True`
- `aiConstructionObserved=True`
- `stabilitySurplusObserved=True`

## Branch State

- branch: `codex/unity-attack-move`
- head: `7759f84e1c00eeb8a1baf329ac33b38d0e074cbc`
- rebased master head: `548d7804ce55766420d75184385b3bedb739a3ee`

## Next Action

- Stop at merge coordination for this branch.
- Do not extend this branch into stances, patrol, guard, follow, AoE combat, AI consumption of attack orders, or HUD cursor polish.
- After merge coordination, the next combat follow-up can start from updated `master` rather than extending this slice in place.
