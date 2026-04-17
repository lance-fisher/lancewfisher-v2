# 2026-04-17 Unity Group Movement And Combat Stances

## Goal

Extend the merged combat + attack-move foundation with three connected runtime
layers:

1. group-aware movement orders that preserve local formation offsets instead of
   collapsing selected units onto one destination
2. lightweight same-faction unit separation so idle or converged units do not
   stack into the same point
3. explicit combat stances (HoldPosition, AttackMove, PursueInRange,
   RetreatOnLowHealth) so attack-move becomes one stance within a broader
   command model instead of a one-off debug flag

## Browser Reference

- `src/game/core/simulation.js:8012-8023` - nearest hostile selection inside
  effective range. Unity `AutoAcquireTargetSystem` stance gates are layered on
  top of this same acquire-in-range expectation.
- `src/game/core/simulation.js:8704-8710` and `8823-8858` - attack-range and
  target-distance enforcement. Unity HoldPosition now clamps pursuit to attack
  range instead of allowing free chase.
- `src/game/core/simulation.js:9289` - `issueAttackCommand(...)` explicit
  command precedence. Unity group move / attack-move orders stay aligned with
  that explicit-command model rather than reverting to pure passive acquire.
- `src/game/core/simulation.js:11933-11949` - low-health retreat threshold and
  fallback behavior in reserve-defense logic. Unity RetreatOnLowHealth uses the
  same broad "drop combat intent and move to a safer friendly anchor" semantic
  at the individual combat-unit layer.

## Canon / Prior Unity Reference

- `docs/unity/session-handoffs/2026-04-16-unity-selection-and-formation-move-shell.md`
- `docs/unity/session-handoffs/2026-04-17-unity-attack-orders-and-attack-move.md`
- `docs/unity/session-handoffs/2026-04-17-unity-target-acquisition-throttling-and-sight-loss.md`
- `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`

## Work Completed

### Group-aware movement

- `unity/Assets/_Bloodlines/Code/Combat/GroupMovementOrderComponent.cs`
  introduced per-unit group order payload:
  `GroupId`, `GroupCenter`, `LocalOffset`, `DestinationCenter`,
  `OrderTimestamp`, `IsAttackMove`.
- `unity/Assets/_Bloodlines/Code/Combat/GroupMovementResolutionSystem.cs`
  now runs before `AttackOrderResolutionSystem` and `UnitMovementSystem`. It
  translates group center + local offset into per-unit move destinations
  without re-centering surviving units mid-march.
- Formation footprint is capped through offset rescaling so larger selections
  preserve shape without exploding into oversized radii.
- Attack-move group orders resume through the existing attack-order seam by
  updating per-unit `AttackMoveDestination` to the offset destination.
- Debug surface additions landed in a new partial:
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Movement.cs`
  with:
  - `TryDebugIssueGroupMoveOrder(...)`
  - `TryDebugInspectGroupMovement(...)`
  - shared internal order issuance used by existing attack-order debug paths

### Soft unit separation

- `unity/Assets/_Bloodlines/Code/Combat/UnitSeparationComponent.cs`
  introduced runtime per-unit separation radius storage.
- `unity/Assets/_Bloodlines/Code/Combat/UnitSeparationSystem.cs` runs after
  `UnitMovementSystem` and before `PositionToLocalTransformSystem`, applying a
  capped positional nudge only for same-faction overlap pairs.
- Separation is suppressed when either unit is dying or still inside the recent
  projectile-impact recovery window so pushback does not jitter during death or
  immediate impact resolution.
- `unity/Assets/_Bloodlines/Code/Combat/RecentImpactComponent.cs` and
  `RecentImpactRecoverySystem.cs` provide that short-lived suppression window.
- `unity/Assets/_Bloodlines/Code/Definitions/UnitDefinition.cs` gained additive
  `SeparationRadius`, imported through
  `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`, with
  `CombatUnitRuntimeDefaults.ResolveSeparationRadius(...)` supplying sensible
  defaults for infantry, cavalry, and siege.

### Combat stances

- `unity/Assets/_Bloodlines/Code/Combat/CombatStanceComponent.cs` introduced:
  - `CombatStance` enum:
    `HoldPosition`, `AttackMove`, `PursueInRange`, `RetreatOnLowHealth`
  - `CombatStanceComponent`
  - `CombatStanceRuntimeComponent`
- `unity/Assets/_Bloodlines/Code/Combat/CombatStanceResolutionSystem.cs`
  now runs before auto-acquire and attack-order resolution:
  - `HoldPosition` suspends auto-acquire during direct move orders and refuses
    chase outside live attack range
  - `AttackMove` keeps the existing interrupt-and-resume behavior alive through
    the attack-order bridge
  - `PursueInRange` remains the default combat behavior for combat units
  - `RetreatOnLowHealth` clears combat intent, drops explicit targets, and
    moves toward the nearest live friendly building inside sight
- `unity/Assets/_Bloodlines/Code/Combat/CombatUnitRuntimeDefaults.cs` now owns
  default stance selection and low-health retreat thresholds. Workers default
  to `HoldPosition`; combat units default to `PursueInRange`.
- Spawn seams were updated additively so stances and separation exist on both
  baked and runtime-produced units:
  - `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs`
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`
  - `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
  - `unity/Assets/_Bloodlines/Code/Systems/UnitProductionSystem.cs`
- Debug stance API landed in
  `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Stances.cs`
  with:
  - `TryDebugSetStance(...)`
  - `TryDebugGetStance(...)`

### Smoke coverage and validation hardening

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesCombatSmokeValidation.cs`
  now contains eight green proof phases:
  - melee
  - projectile
  - explicit attack
  - attack-move
  - target visibility / sight loss
  - group movement
  - separation
  - stance behavior (hold-position + retreat-low-hp)
- `unity/Assembly-CSharp.csproj` now explicitly includes the new movement,
  separation, recent-impact, and stance runtime files plus the new debug
  partials. Without this, the branch does not build outside the Unity-generated
  project state.
- Governed wrapper hardening landed in:
  - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1`
  - `scripts/Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1`
  - `scripts/Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1`
- Those wrappers now wait for explicit pass/fail markers in the Unity log and
  allow a single rerun after import/compile warm-up. This was required because
  the continuation-platform harness intermittently returns non-authoritative
  Unity exit codes before the child batch process writes its final validator
  outcome.

## Scope Discipline

- No edits to graphics-lane owned files:
  `Assets/_Bloodlines/Shaders/**`,
  `Assets/_Bloodlines/Code/Rendering/**`,
  `Assets/_Bloodlines/Code/Components/FactionTintComponent.cs`,
  `Assets/_Bloodlines/Art/Placeholders/**`,
  `Assets/_Bloodlines/Data/{Unit,Building}VisualDefinitions/**`,
  placeholder mesh generator, or graphics runtime validator.
- No edits to browser runtime behavior under `proto/**`, `ingestions/**`, or
  `01_CANON/**`.
- No changes to debug primitive proxy-count expectations or
  `expectedProxyMinimum`.
- Group movement intentionally does not re-center remaining units after deaths
  or casualties mid-march because that produces visible snap.
- `AttackOrderComponent.IsAttackMoveActive` remains as a compatibility bridge for
  this slice. Long-term source of truth should be `CombatStance.AttackMove`, but
  the flag is still read or written in:
  - `AttackOrderResolutionSystem`
  - `AutoAcquireTargetSystem`
  - `AttackResolutionSystem`
  - `DeathResolutionSystem`
  - `GroupMovementResolutionSystem`
  - `CombatStanceResolutionSystem`
  The next combat lane should retire that flag in one contained cleanup pass
  after updating all remaining readers/writers together.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - passed:
    `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
  - retreat proof line:
    `Combat smoke validation retreat sub-phase passed: initialDistance=0.5, finalDistance=7.5, finalRetreatDistance=0.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - passed with the preserved economy / AI proof set still present, including:
    `gatherDepositObserved=True`, `trickleGainObserved=True`,
    `starvationObserved=True`, `loyaltyDeclineObserved=True`,
    `capPressureObserved=True`, `aiActivityObserved=True`,
    `aiConstructionObserved=True`, `stabilitySurplusObserved=True`,
    `aiMilitaryOrdersIssued=2`,
    `productionProgressAdvancementVerified=True`,
    `constructionProgressAdvancementVerified=True`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - passed for both canonical scenes
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-movement -WrapperScript scripts/Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1`
  - passed:
    `Graphics runtime validation passed: unitProxies=16, buildingProxies=9, factionTintAttached=25, expectedUnitsWithDefinition=16, expectedBuildingsWithDefinition=9.`
- `node tests/data-validation.mjs`
- `node tests/runtime-bridge.mjs`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`

## Next Action

After this branch is merged:

1. retire `AttackOrderComponent.IsAttackMoveActive` in favor of
   `CombatStance.AttackMove` as the only long-lived attack-move state
2. keep future combat-lane work out of the graphics-owned presentation paths
3. stretch ideas still intentionally not landed here:
   formation presets, patrol waypoints, focus fire, unit-on-building soft
   collision, squad ergonomics

## Branch

- Branch: `codex/unity-group-movement-and-stances`
- Base: `master` at `9036d91` before the final documentation / wrapper-hardening
  commit
- Validation lock session: `codex-movement`
- Merge status: pushed for merge coordination only; not merged into `master`
