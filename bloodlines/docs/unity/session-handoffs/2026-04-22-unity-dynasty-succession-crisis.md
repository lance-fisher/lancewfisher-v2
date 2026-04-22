# Unity Dynasty Slice: Succession Crisis System

- Date: 2026-04-22
- Lane: `dynasty-succession-crisis`
- Branch: `codex/unity-dynasty-succession-crisis`
- Status: validated on branch

## Goal

Port the browser succession-crisis trigger and recovery seam under
`unity/Assets/_Bloodlines/Code/Dynasties/` without touching the
AI-owned `unity/Assets/_Bloodlines/Code/AI/**` lane, and prove crisis
triggering, loyalty shock, legitimacy drain, resource throttling, and
recovery removal with a dedicated smoke validator plus the full governed
validation chain.

## Browser Reference

- `src/game/core/simulation.js`
  - `SUCCESSION_CRISIS_SEVERITY_PROFILES`
  - `buildSuccessionCrisisTriggerProfile`
  - `startSuccessionCrisis`
  - `tickDynastyPoliticalEvents`
  - `getSuccessionCrisisTerms`
  - `consolidateSuccessionCrisis`

## What Landed On Branch

- `unity/Assets/_Bloodlines/Code/Dynasties/SuccessionCrisisComponent.cs`
  now defines the ECS crisis state: severity, start day, recovery progress,
  resource trickle factor, legitimacy drain, opening-shock guard, and the
  deterministic daily tick fields required to mirror the browser cadence.
- `unity/Assets/_Bloodlines/Code/Dynasties/SuccessionCrisisEvaluationSystem.cs`
  now watches dynasty ruler transitions after succession resolution,
  evaluates claimant pressure and ruler age into minor/moderate/major/
  catastrophic crisis bands, applies the opening loyalty shock to owned
  control points, and starts the crisis component.
- `unity/Assets/_Bloodlines/Code/Dynasties/SuccessionCrisisRecoverySystem.cs`
  now advances whole-day crisis ticking from the dual clock, drains
  legitimacy plus control-point loyalty, scales recovery by conviction band,
  and removes the crisis component once recovery completes.
- `unity/Assets/_Bloodlines/Code/Systems/ControlPointResourceTrickleSystem.cs`
  now reads faction succession-crisis state and applies the crisis resource
  multiplier to the existing control-point yield path without widening the
  system's ownership.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Dynasty.cs`
  now exposes `TryDebugGetSuccessionCrisis(...)` for smoke validation and
  operator inspection.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSuccessionCrisisSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnitySuccessionCrisisSmokeValidation.ps1`
  now prove four phases:
  1. a contested succession starts a moderate-or-worse crisis
  2. the opening loyalty shock lands on owned control points
  3. recovery completion removes the crisis component
  4. catastrophic crises drain more legitimacy than minor crises

## Validation Proof

- Runtime build:
  - `Build succeeded.`
- Editor build:
  - `Build succeeded.`
- Dedicated smoke:
  - `Succession crisis smoke validation passed.`
- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed.`
- Combat smoke:
  - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Scene shells:
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True.`
- Siege smoke:
  - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Contract staleness:
  - `STALENESS CHECK PASSED: Contract revision=90, last-updated=2026-04-22 is current.`

## Unity-Side Simplifications Deferred

- Crisis severity currently derives from ruler age and claimant count only;
  the browser's broader narrative and diplomacy consequences remain for the
  later dynasty political-events slice.
- Recovery currently uses conviction-band acceleration but does not yet
  surface crisis status in the player HUD.
- The opening loyalty shock applies once at crisis start; downstream unrest
  and revolt gameplay remain deferred.

## Exact Next Action

1. Stage the succession-crisis slice files plus contract and continuity
   updates, commit them on `codex/unity-dynasty-succession-crisis`, and push
   to `origin`.
2. Merge the validated branch through the governed flow, then rerun the full
   10-gate validation chain on merged `master`.
3. Resume the next incomplete dynasty item from the directive, with dynasty
   political events the next clean additive slice.
