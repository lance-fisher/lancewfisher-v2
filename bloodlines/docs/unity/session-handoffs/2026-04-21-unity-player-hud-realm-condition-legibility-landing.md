# 2026-04-21 Unity Player HUD Realm-Condition Legibility Landing

## Status

Merged to canonical `master` via `dfcbcec9`.

## What Landed

- `unity/Assets/_Bloodlines/Code/HUD/RealmConditionHUDComponent.cs`
  and
  `RealmConditionHUDSystem.cs`
  are now canonical `master` content for the first player-HUD slice.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now canonically exposes the parseable
  `TryDebugGetRealmConditionHUDSnapshot(...)` seam on `master`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesRealmConditionHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityRealmConditionHUDSmokeValidation.ps1`
  now canonically prove the first HUD slice on `master`.
- The player HUD lane remains active after landing; only the first realm-condition
  slice is complete. Match progression, fortification, victory, and broader
  world-pressure HUD surfaces remain follow-up work under the same lane.

## Merged-Master Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Error(s)`
- Editor build:
  - `113 Warning(s)`
  - `0 Error(s)`
- Dedicated HUD smoke:
  - `BLOODLINES_REALM_CONDITION_HUD_SMOKE PASS`
  - `Phase 1 PASS: stable baseline surfaces green realm bands, neutral conviction, red uncommitted faith, CycleProgress=0.500.`
  - `Phase 2 PASS: cap pressure, food strain, water strain, and loyalty distress all surface red with visible strain streaks.`
  - `Phase 3 PASS: ConvictionBand=ApexMoral, ConvictionLabel=Apex Moral, ConvictionScore=80.0.`
  - `Phase 4 PASS: FaithId=OldLight, DoctrinePath=Light, FaithLevel=4, FaithTier=Fervent, FaithBand=green.`
- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ...`
- Combat smoke:
  - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- Bootstrap scene shell:
  - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
- Gameplay scene shell:
  - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- Fortification smoke:
  - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. Baseline: tier=0, ceiling=3. TierAdvance: tier=1, ceiling=3, contributionApplied=1. ReserveMuster: retreatDuty=Fallback, reserveDuty=Muster, committed=1. ReserveRecovery: duty=Ready, healthRatio=0.95, readyReserveCount=1.`
- Siege smoke:
  - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. Baseline: strain=0, attrition=False, supportRefreshCount=0. Strain: status=Strained, strain=6.25, attackMultiplier=0.88, speedMultiplier=0.90. Recovery: supported=True, strain=3.88, supportRefreshCount=51. Support: engineerSupport=True, supplySupport=True, refreshCount=3, suppliedUntil=11.20.`
- Data validation:
  - `Bloodlines data validation passed.`
- Runtime bridge:
  - `Bloodlines runtime bridge validation passed.`
- Contract staleness on merged master before the landing continuity bump:
  - `STALENESS CHECK PASSED: Contract revision=67, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`

## Exact Next Action

Create a fresh Codex branch from the updated `master` and implement the next HUD slice:
`MatchProgressionHUDComponent`, `MatchProgressionHUDSystem`, and
`TryDebugGetMatchHUDSnapshot`.
