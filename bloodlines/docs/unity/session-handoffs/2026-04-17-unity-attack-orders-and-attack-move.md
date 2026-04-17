# 2026-04-17 Unity Attack Orders And Attack-Move

## Scope

Concurrent Codex combat-lane follow-up after projectile combat under the contract in `docs/unity/CONCURRENT_SESSION_CONTRACT.md`:

- owned: `unity/Assets/_Bloodlines/Code/Combat/**`
- owned: `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- owned: `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- owned: `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AttackOrders.cs`
- shared narrow edits only: `unity/Assembly-CSharp.csproj`, `unity/Assembly-CSharp-Editor.csproj`

## Delivered

- Added explicit order payload:
  - `AttackOrderComponent`
- Added order-conversion runtime:
  - `AttackOrderSystem`
- Extended `AutoAcquireTargetSystem` so an active explicit attack order wins over passive auto-acquire whenever the hostile target is still alive and inside sight.
- Extended `DeathResolutionSystem` so dead units drop any residual `AttackOrderComponent` alongside `AttackTargetComponent`.
- Added a new debug-only partial:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AttackOrders.cs`
- Extended the dedicated combat smoke validator with a third phase that proves explicit attack orders through the debug API.

## Canonical Semantics Landed

- `AttackOrderComponent` now carries:
  - `ExplicitTargetEntity`
  - `IsAttackMoveDestination`
  - `AttackMoveDestination`
  - `IsActive`
- `AttackOrderSystem` now runs before `AutoAcquireTargetSystem` and converts combat orders into the existing `MoveCommandComponent` plus `AttackTargetComponent` flow.
- Explicit attack orders now:
  - bind to a specific hostile target
  - maintain that target while it remains alive and inside sight
  - stop pursuit, clear the attack target, and deactivate the order when the hostile leaves sight or dies
- Attack-move orders now:
  - store the issued ground destination on the unit
  - let the unit engage local hostiles through the normal combat systems
  - resume movement toward the stored destination after local engagement ends
  - deactivate once the unit reaches the attack-move destination
- Debug command bindings now exist for the first playable command path:
  - right-click hostile unit with selected combat units issues an explicit attack order
  - `A` enters attack-move cursor mode
  - next right-click on ground becomes an attack-move destination
  - `Esc` cancels attack-move mode and restores the pre-mode selection snapshot

## Validation

Green in the clean attack-order worktree at `D:\ao\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
4. clean-worktree bootstrap runtime smoke using the same execute method as the governed wrapper:
   - `Bloodlines.EditorTools.BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
5. clean-worktree bootstrap scene shell validation:
   - `Bloodlines.EditorTools.BloodlinesGameplaySceneBootstrap.RunBatchValidateBootstrapSceneShell`
6. clean-worktree gameplay scene shell validation:
   - `Bloodlines.EditorTools.BloodlinesGameplaySceneBootstrap.RunBatchValidateGameplaySceneShell`
7. `node tests/data-validation.mjs`
8. `node tests/runtime-bridge.mjs`

Combat smoke pass lines from `artifacts/unity-combat-smoke.log`:

- `Combat smoke validation melee phase passed: dead='enemy', survivorHealth=6/12, elapsedSeconds=1.2.`
- `Combat smoke validation projectile phase passed: projectileObserved=True, dead='enemy', elapsedSeconds=0.9.`
- `Combat smoke validation attack-order phase passed: explicitTargetObserved=True, cooldownObserved=True, residualTarget=False, dead='enemy', elapsedSeconds=0.6.`
- `Combat smoke validation passed: meleePhase=True, projectilePhase=True, attackOrderPhase=True.`

Bootstrap runtime smoke pass line confirms the economy proofs still remained green:

- `gatherDepositObserved=True`
- `trickleGainObserved=True`
- `starvationObserved=True`
- `loyaltyDeclineObserved=True`
- `capPressureObserved=True`

## Wrapper / Validation Note

- The checked-in `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` and `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` wrappers are still pinned to `D:\ProjectsHome\Bloodlines`.
- The canonical checkout was intentionally left alone because Claude's concurrent lane still had dirty shared files there.
- To keep validation honest without colliding with that checkout, the same Unity batch execute methods were rerun through clone-local temporary wrapper scripts under `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1`.

## Branch State

- branch: `codex/unity-attack-orders-attack-move`
- base master head: `aed6969b9152c630a67eacbd9f1759361ec28cdb`

## What The Next Combat Slice Should Do

- stop at merge coordination for this branch
- after merge, continue deeper combat control and feel through:
  - acquisition throttling
  - line-of-sight tuning
  - combat presentation cleanup
  - deeper command-surface combat ergonomics

## Merge / Coordination Note

This lane is green and should stop at branch state for merge coordination.
Do not widen this branch into AI, economy, renown, or death-presentation polish before merge.
