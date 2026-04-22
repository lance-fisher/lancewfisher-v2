# Unity Player Marriage Diplomacy Sub-Slice 2C: Dissolution Validation

- Date: 2026-04-21
- Lane: `player-marriage-diplomacy`
- Branch: `codex/unity-player-marriage-dissolution`
- Status: complete on branch, pending merge to `master`

## Goal

Close the remaining player-marriage diplomacy requirement without duplicating
the already-landed death-dissolution runtime from the paused
`dynasty-house-parity` lane.

Repo-reality note:

- `unity/Assets/_Bloodlines/Code/Dynasties/MarriageDeathDissolutionSystem.cs`
  is already on `master` via the 2026-04-20 dynasty marriage parity slice.
- This sub-slice therefore adds the missing dedicated player-facing smoke
  surface and validates succession compatibility against the existing runtime
  instead of cloning the dissolution system under `PlayerDiplomacy/`.

## Browser Reference

- `src/game/core/simulation.js`
  - `acceptMarriage` (7388-7469)
  - `tickMarriageDissolutionFromDeath` (7471-7494)
  - `tickMarriageGestation` (7496-7530)
- `tests/runtime-bridge.mjs`
  - death-driven dissolution + restore assertions (3234-3298)

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerMarriageDissolutionSmokeValidation.cs`.
- Added `scripts/Invoke-BloodlinesUnityPlayerMarriageDissolutionSmokeValidation.ps1`.
- The new validator reuses the already-landed cross-lane systems:
  - `MarriageDeathDissolutionSystem`
  - `DynastySuccessionSystem`
  - `MarriageGestationSystem`
  - `PlayerMarriageAcceptSystem`
- The dedicated 3-phase smoke now proves:
  1. an accepted marriage remains active while both members are alive
  2. ruler death dissolves both mirror marriage records and succession promotes
     the heir in the same validation world
  3. active marriages still gestate a child when no death intervenes
- Updated the concurrent session contract and continuity surfaces so the player
  marriage lane now records the dedicated dissolution proof slice and the
  cross-lane reuse of the dynasty-parity runtime.

## Validation Proof Lines

- Runtime build: `Build succeeded.` / `0 Error(s)`
- Editor build: `0 Error(s)` with existing editor warnings
- Bootstrap runtime smoke: `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ...`
- Combat smoke: `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell: `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell: `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke: `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. Baseline: tier=0, ceiling=3. TierAdvance: tier=1, ceiling=3, contributionApplied=1. ReserveMuster: retreatDuty=Fallback, reserveDuty=Muster, committed=1. ReserveRecovery: duty=Ready, healthRatio=0.95, readyReserveCount=1.`
- Siege smoke: `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. Baseline: strain=0, attrition=False, supportRefreshCount=0. Strain: status=Strained, strain=6.25, attackMultiplier=0.88, speedMultiplier=0.90. Recovery: supported=True, strain=3.88, supportRefreshCount=51. Support: engineerSupport=True, supplySupport=True, refreshCount=3, suppliedUntil=11.20.`
- Data validation: `Bloodlines data validation passed.`
- Runtime bridge: `Bloodlines runtime bridge validation passed.`
- Contract staleness: `STALENESS CHECK PASSED: Contract revision=57, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
- Dedicated smoke marker: `BLOODLINES_PLAYER_MARRIAGE_DISSOLUTION_SMOKE PASS`
- Dedicated smoke success proof:
  - `Phase 1 PASS: alive marriage remained active with MarriageId=marriage-11210497115101494597108105118101451099711411410597`
  - `Phase 2 PASS: ruler death dissolved MarriageId=marriage-1121049711510150451141171081011144510010197116104 at day=50.00 and promoted player-bloodline-heir`
  - `Phase 3 PASS: live marriage gestated childId=child-marriage-11210497115101514510310111511697116105111110 for headFaction=enemy`

## Unity-Side Simplifications Deferred

- No new runtime dissolution system was added under `PlayerDiplomacy/` because
  the canonical death-dissolution behavior was already landed under the paused
  dynasty parity lane; duplicating it would create conflicting authority over
  marriage teardown.
- The dedicated player-lane validator seeds pending proposals directly through
  `MarriageProposalComponent` plus `PlayerMarriageAcceptRequestComponent`
  instead of routing through a public debug command surface, because this slice
  is validating succession-aware dissolution rather than proposal UX.
- The existing dynasty runtime still stores only `Dissolved` and
  `DissolvedAtInWorldDays`; browser fields such as dissolution reason and
  deceased member id remain part of the broader dynasty-parity fidelity gap and
  were not widened in this player-lane proof slice.

## Exact Next Action

1. Stage the dissolution validator, wrapper, and continuity updates and commit
   them on `codex/unity-player-marriage-dissolution`.
2. Push the branch, merge it to `master`, and rerun the full governed
   validation gate on merged `master`.
3. After the landing continuity pass, start Priority 3 on a fresh
   `codex/unity-player-covert-ops-foundation` branch.
