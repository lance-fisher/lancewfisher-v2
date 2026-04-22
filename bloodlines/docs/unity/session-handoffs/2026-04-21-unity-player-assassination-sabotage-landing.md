# Unity Player Covert Ops Sub-Slice 3B Landing: Assassination And Sabotage

- Date: 2026-04-21
- Lane: `player-covert-ops`
- Branch: `master`
- Merge Commit: `2892c583`
- Merged Feature Branch: `codex/unity-player-assassination-sabotage`
- Status: landed on `master`

## Goal

Record the `master` landing of player covert ops sub-slice 3B after the merged
branch re-passed the governed validation gate in the clean `D:\BLM13` worktree.

## Browser Reference

- `src/game/core/simulation.js`
  - `SABOTAGE_COSTS` (9739-9744)
  - `SABOTAGE_DURATIONS` (9746-9751)
  - `ASSASSINATION_COST` (9765)
  - `ASSASSINATION_DURATION_SECONDS` (9769)
  - `validateSabotageTarget` (9795-9815)
  - `getSabotageTerms` (9900-9958)
  - `getAssassinationContest` (10214-10282)
  - `getAssassinationTerms` (10284-10323)
  - `startAssassinationOperation` (10912-10950)
  - `startSabotageOperation` (10952-10991)
- `tests/runtime-bridge.mjs`
  - sabotage subtype + target legality assertions (1378-1412)
  - espionage-to-assassination player flow assertions (3490-3628)

## What Landed

- `codex/unity-player-assassination-sabotage` is now merged to canonical
  `master` via `2892c583`.
- `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsSystem.cs`
  now lands the player-side assassination and sabotage dispatch seams alongside
  the previously landed espionage path.
- `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsRequestComponent.cs`
  and
  `unity/Assets/_Bloodlines/Code/PlayerCovertOps/PlayerCovertOpsResolutionComponent.cs`
  now carry the subtype and telemetry required for structured assassination and
  sabotage readout.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  now exposes player-issued assassination and sabotage debug commands and
  returns richer structured covert-op readouts.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`
  now includes the 3B target-validation phases, so the canonical player covert
  ops smoke on `master` proves assassination target legality and sabotage
  target legality in addition to the 3A espionage coverage.

## Validation Proof Lines

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `113 Warning(s)`
  - `0 Error(s)`
- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.004, productionProgressLatestRatio=0.084, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.001, constructionProgressLatestRatio=0.915, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.71, trickleLatestFood=44.04, trickleInitialWater=31.94, trickleLatestWater=33.94, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
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
- Dedicated smoke:
  - `BLOODLINES_PLAYER_COVERT_OPS_SMOKE PASS`
  - `Phase 5 PASS: assassination targeted memberId=enemy-bloodline-marshal, title=War Captain, gold=215, influence=152.`
  - `Phase 6 PASS: sabotage targeted entityIndex=22, subtype=gate_opening, gold=200, influence=102.`

## Unity-Side Simplifications Deferred

- The landed player covert-op entities still intentionally remain lane-local
  rather than widening the AI-owned `DynastyOperationComponent` graph.
- Counter-intelligence watch, dossier accumulation, retaliation metadata, and
  explicit bloodline-guard defense remain the next follow-up in sub-slice 3C.
- Assassination and sabotage resolution effects remain outside this slice; 3B
  lands the player dispatch seam only, matching the directive scope.

## Exact Next Action

1. Start sub-slice 3C on fresh branch `codex/unity-player-counter-intelligence`.
2. Port `tickDynastyCounterIntelligence` and `tickDynastyIntelligenceReports`
   under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`.
3. Extend the dedicated player covert ops smoke with counter-intelligence and
   intelligence-report assertions without reopening `unity/Assets/_Bloodlines/Code/AI/**`.
