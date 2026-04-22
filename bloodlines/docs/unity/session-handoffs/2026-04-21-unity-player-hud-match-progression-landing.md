# 2026-04-21 Unity Player HUD Match Progression Landing

## Status

Merged to canonical `master` via `ed22484c`.

## What Landed

- `codex/unity-player-hud-match-progression` is now merged, so the second
  player-HUD slice is canonical on `master`.
- `unity/Assets/_Bloodlines/Code/HUD/MatchProgressionHUDComponent.cs` and
  `MatchProgressionHUDSystem.cs` now canonically project stage, phase,
  readiness, next-stage, declaration count, in-world time, dominant-leader
  telemetry, Great Reckoning telemetry, and resolved world-pressure state into
  a singleton HUD read-model.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs`
  now canonically exposes the parseable `MatchHUD|Key=Value|...` readout via
  `TryDebugGetMatchHUDSnapshot(...)`.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionHUDSmokeValidation.cs`
  plus
  `scripts/Invoke-BloodlinesUnityMatchProgressionHUDSmokeValidation.ps1`
  now canonically prove the match-progression HUD slice on `master`.
- The player HUD lane remains active after landing. Fortification legibility,
  victory readout, and any on-screen HUD rendering remain follow-up work inside
  the same lane.

## Merged-Master Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `113 Warning(s)`
  - `0 Error(s)`
- Dedicated HUD smoke:

```text
BLOODLINES_MATCH_PROGRESSION_HUD_SMOKE PASS
Phase 1 PASS: founding stage surfaces emergence phase, quiet world pressure, and InWorldDays=12.0.
Phase 2 PASS: stage-4 commitment snapshot surfaces readiness, next-stage label, and declaration count.
Phase 3 PASS: dominant leader `player` surfaces overwhelming world pressure level 2 and score 6.
Phase 4 PASS: Great Reckoning target `enemy` surfaces convergence pressure level 3 at share 0.720.
```

- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.005, productionProgressLatestRatio=0.085, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.002, constructionProgressLatestRatio=0.915, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.70, trickleLatestFood=44.03, trickleInitialWater=31.93, trickleLatestWater=33.93, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
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
  - `STALENESS CHECK PASSED: Contract revision=69, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`

## Exact Next Action

Keep the HUD lane active on canonical `master`, start the next slice from a
fresh Codex branch, and decide whether the next player-facing HUD increment is
fortification legibility or victory-distance readout before writing runtime
code.
