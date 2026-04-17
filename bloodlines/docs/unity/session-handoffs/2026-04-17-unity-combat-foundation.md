# 2026-04-17 Unity Combat Foundation

## Scope

Concurrent Codex lane for the first real Unity combat foundation under the file-scope contract:

- owned: `unity/Assets/_Bloodlines/Code/Combat/**`
- owned: `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
- owned: `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
- shared narrow edits only: bootstrap authoring, baker, runtime bootstrap spawn, unit production

## Delivered

- Added runtime combat payloads:
  - `CombatStatsComponent`
  - `AttackTargetComponent`
  - `HostilityComponent`
  - `UnitCombatDefinitionElement`
  - `MapFactionHostilitySeedElement`
- Added first combat systems:
  - `AutoAcquireTargetSystem`
  - `AttackResolutionSystem`
  - `DeathResolutionSystem`
- Wired combat stats into:
  - `MapUnitSeedElement`
  - bootstrap authoring and baking
  - skirmish bootstrap runtime spawning
  - produced-unit spawning through bootstrap combat-definition lookup
- Added a dedicated governed combat smoke validator and wrapper.

## Critical Integration Note

Canonical unit `attackRange` and `sight` values come from the frozen browser spec in pixel-scale terms, while the Unity battlefield uses tile or world coordinates from `MapDefinition`.

Without normalization, units auto-acquired and marched across the map during the existing bootstrap smoke lane. The fix in this slice was:

- normalize `attackRange` and `sight` by `map.tileSize` at the authoring and baker seam
- keep raw `attackDamage` and `attackCooldown`
- store normalized combat distances in the bootstrap seed and combat-definition lookup buffers

This preserves canonical data while keeping the live Unity battlefield shell stable.

## Validation

Green in the combat worktree at `D:\ProjectsHome\Bloodlines_codex_unity_combat\bloodlines`:

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
3. `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
4. direct worktree bootstrap runtime smoke using the same execute method as the governed wrapper:
   - `Bloodlines.EditorTools.BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation`
   - pass line confirms:
     - `buildings=11`
     - `units=18`
     - `controlledUnits=8`
     - `gatherDepositObserved=True`
5. `node tests/data-validation.mjs`
6. `node tests/runtime-bridge.mjs`

Also reran the unchanged governed canonical wrapper successfully:

- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`

Combat smoke pass line from `artifacts/unity-combat-smoke.log`:

- `Combat smoke validation passed: dead='enemy', survivorHealth=6/12, elapsedSeconds=1.2.`

## Branch State

- branch: `codex/unity-combat-foundation`
- code commit: `eb26040` (`Add Unity combat foundation systems`)

## What The Next Combat Slice Should Do

- add attack-move or explicit attack orders instead of pure passive auto-acquire
- split melee contact from ranged projectile delivery
- add line-of-sight and acquisition throttling so large battles stay performant
- add death presentation cleanup:
  - debug proxy death visuals
  - corpse or fade policy
  - selection cleanup polish
- add first renown or conviction combat hook on death and kill resolution
- add combat debug APIs through a new partial `BloodlinesDebugCommandSurface.Combat.cs` if command-surface inspection becomes necessary

## Merge / Coordination Note

This lane is green and should stop at branch state for merge coordination.
Do not extend Claude-owned economy files from this branch.
