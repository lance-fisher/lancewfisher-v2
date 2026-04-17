# 2026-04-17 Unity Projectile Combat

## Scope

Concurrent Codex lane for the first real Unity projectile lifecycle under the file-scope contract:

- owned: `unity/Assets/_Bloodlines/Code/Combat/**`
- owned: `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- owned: `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- shared narrow edits only: bootstrap authoring, bootstrap baker, bootstrap seed payloads, skirmish bootstrap spawn, produced-unit spawn, debug presentation bridge

## Delivered

- Added projectile runtime payloads:
  - `ProjectileComponent`
  - `ProjectileFactoryComponent`
- Added projectile systems:
  - `ProjectileMovementSystem`
  - `ProjectileImpactSystem`
- Extended `AttackResolutionSystem` so:
  - melee units still apply instant damage directly
  - units carrying `ProjectileFactoryComponent` now spawn projectile entities instead of deducting health immediately
- Wired projectile parameters through shared spawn seams:
  - `MapUnitSeedElement`
  - `BloodlinesMapBootstrapAuthoring`
  - `BloodlinesMapBootstrapBaker`
  - `SkirmishBootstrapSystem`
  - `UnitProductionSystem`
- Extended the debug presentation bridge so live projectile entities render distinct sphere proxies in Play Mode.
- Extended the dedicated combat smoke validator so it now proves both:
  - melee instant-hit combat still kills correctly
  - ranged projectile combat produces an in-flight projectile before impact and then kills correctly on arrival

## Canonical Semantics Landed

- Units with `UnitRole.Ranged` now resolve attacks through projectile entities.
- Units with projectile-capable siege classes now receive projectile factories through the same spawn path.
- Projectiles carry:
  - launch position
  - last known target position
  - target entity
  - owner entity and owner faction
  - damage
  - speed
  - max lifetime
  - elapsed lifetime
  - arrival radius
- Projectile movement homes on the target entity while it is still alive and present.
- If the target dies or disappears, the projectile continues toward the last recorded target position.
- Impact applies damage through `HealthComponent` only and leaves death cleanup to the existing `DeathResolutionSystem`.

## Critical Integration Notes

- Projectile speed is normalized at the same authoring/baker seam as combat distance so browser-spec values survive into Unity world space without breaking battlefield scale.
- Default projectile fallback values are:
  - speed `14f`
  - max lifetime `4f`
  - arrival radius `0.4f`
- Current `master` still references Claude-lane AI economy attachment in `SkirmishBootstrapSystem` before that component is merged. This slice keeps the shared file buildable by attaching that AI component reflectively when it exists, instead of taking a hard compile-time dependency on `Code/AI/**`.

## Validation

Green in the clean projectile worktree at `D:\ProjectsHome\lancewfisher-v2_codex_unity_projectile\bloodlines`:

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
- `Combat smoke validation passed: meleePhase=True, projectilePhase=True.`

Bootstrap runtime smoke pass line confirms the pre-existing economy proofs remained green:

- `gatherDepositObserved=True`
- `trickleGainObserved=True`
- `starvationObserved=True`
- `loyaltyDeclineObserved=True`
- `capPressureObserved=True`

## Branch State

- branch: `codex/unity-projectile-combat`
- base master head: `a8dd55315285e868ba7e53afc5646ddf0cf5de18`

## What The Next Combat Slice Should Do

- stop at merge coordination for this branch
- after merge, take the next combat follow-up through explicit attack orders or attack-move
- only after that, continue into acquisition throttling, line-of-sight gating, death presentation polish, and combat-hook surfaces such as renown

## Merge / Coordination Note

This lane is green and should stop at branch state for merge coordination.
Do not extend Claude-owned AI or economy files from this branch.
