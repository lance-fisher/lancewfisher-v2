# Unity Player Covert Ops Sub-Slice 3C: Counter-Intelligence And Intelligence Reports

- Date: 2026-04-21
- Lane: `player-covert-ops`
- Branch: `codex/unity-player-counter-intelligence`
- Status: complete on branch, pending merge to `master`

## Goal

Port the browser's player-facing counter-intelligence watch, intelligence-report
expiry, dossier interception, and defended espionage / assassination resolution
seams into ECS under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`, keep the
work inside the Codex-owned player covert-ops lane, extend the debug readout,
and prove the new watch/report lifecycle with a dedicated validator.

## Browser Reference

- `src/game/core/simulation.js`
  - `getActiveIntelligenceReport` (4096-4102)
  - `getActiveCounterIntelligence` (4104-4111)
  - `tickDynastyIntelligenceReports` (4113-4128)
  - `getCounterIntelligenceRoleGuardBonus` (4143-4157)
  - `createDynastyIntelligenceReport` (5348-5368)
  - `storeDynastyIntelligenceReport` (5370-5386)
  - counter-intelligence interception / dossier branch (5650-5838)
  - `buildCounterIntelligenceTerms` (9987-10084)
  - `getDynastyCounterIntelligenceProfile` (10086-10119)
  - `recordCounterIntelligenceInterception` (10121-10171)
  - `createCounterIntelligenceWatch` (10173-10203)
  - `getEspionageContest` (10205-10245)
  - `getAssassinationContest` (10247-10307)
  - `getCounterIntelligenceTerms` (10309-10360)
  - `startCounterIntelligenceOperation` (10836-10874)
- `tests/runtime-bridge.mjs`
  - counter-intelligence watch / dossier assertions (4130-4240)
  - dossier-backed sabotage support assertions (4884-4970)

## Work Completed

- Added player-owned report/watch data under
  `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`:
  - `IntelligenceReportElement`
  - `PlayerCounterIntelligenceComponent`
  - `PlayerCounterIntelligenceSystem`
- Extended `PlayerCovertOpsResolutionComponent` with watch duration, watch
  strength, ward label, guarded-role summary, and loyalty telemetry so the
  deferred covert-op entity can carry the browser watch terms through
  resolution.
- Reworked `PlayerCovertOpsSystem` so the player covert-op lane now also
  dispatches canonical counter-intelligence watches:
  - canonical cost `gold=60`, `influence=18`
  - canonical activation delay `18 / 86400f`
  - canonical watch duration `150 / 86400f`
  - operator resolution order `Spymaster -> Diplomat -> HeadOfBloodline`
  - duplicate active-watch and duplicate active-op blocking
  - watch-strength derivation from operator renown, fortification tier,
    ward presence, loyalty support, legitimacy support, and weakest-loyalty
    instability penalty
- Extended the player dispatch contests so hostile espionage and assassination
  now read active player-side counter-intelligence defenses:
  - espionage projected chance now includes ward defense plus active watch
    strength
  - assassination projected chance now includes ward defense, role-guard
    bonus, hostility support, and active watch strength
  - assassination intent now reads live report support from retained
    `IntelligenceReportElement` buffers instead of scanning active espionage ops
- Added `PlayerCounterIntelligenceSystem` as the DualClock-driven resolution
  and expiry seam for player covert ops:
  - expires intelligence reports at their browser duration window
  - expires counter-intelligence watches after their browser watch window
  - resolves counter-intelligence activation into a live
    `PlayerCounterIntelligenceComponent`
  - resolves successful espionage into a retained
    `IntelligenceReportElement`
  - resolves foiled espionage and foiled assassination into
    counter-intelligence dossiers, watch interception counters, defender
    legitimacy gain, hostility enforcement, and defender stewardship gain
  - resolves successful assassination into member death while preserving the
    existing dynasty/fallen-ledger path
- Extended `BloodlinesDebugCommandSurface.PlayerCovertOps.cs` with:
  - `TryDebugIssuePlayerCounterIntelligence(sourceFactionId)`
  - `TryDebugGetPlayerCounterIntelligence(factionId)`
  - `TryDebugGetIntelligenceReports(factionId)`
  - richer structured covert-op readout fields for watch duration, watch
    strength, guarded roles, and loyalty telemetry
- Fixed a real ECS-side entity-resolution bug uncovered by the validator:
  faction resolution now prefers the actual faction root over same-faction
  settlement entities, preventing counter-intelligence watches and reports from
  binding to the wrong entity when both carry `FactionComponent`.
- Removed invalid `UpdateBefore(...)` attributes from
  `PlayerCounterIntelligenceSystem` after Unity correctly warned that the
  targeted systems were not in the same update group.
- Added dedicated validation:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCounterIntelligenceSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityPlayerCounterIntelligenceSmokeValidation.ps1`
- The dedicated validator proves four phases:
  1. clean baseline with no active watch and no reports
  2. counter-intelligence activation, stewardship gain, and clean expiry
  3. espionage report creation plus clean report expiry
  4. defended hostile espionage lowers projected odds and yields dossier +
     watch interception telemetry on failure
- Ran the full governed validation gate in
  `D:\BLM13\bloodlines\bloodlines`.
  Temporary worktree-local copies were used only for the still-root-pinned
  bootstrap-runtime and canonical scene-shell wrapper scripts so validation ran
  against this clean worktree rather than the compatibility junction.

## Validation Proof Lines

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `113 Warning(s)`
  - `0 Error(s)`
- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.003, productionProgressLatestRatio=0.083, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.001, constructionProgressLatestRatio=0.914, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.72, trickleLatestFood=44.05, trickleInitialWater=31.95, trickleLatestWater=33.95, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
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
- Contract staleness before the revision-64 continuity pass:
  - `STALENESS CHECK PASSED: Contract revision=63, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
- Dedicated smoke:
  - `BLOODLINES_PLAYER_COUNTER_INTELLIGENCE_SMOKE PASS`
  - `Phase 1 PASS: no active watch and no intelligence reports at baseline.`
  - `Phase 2 PASS: watchId=dynastyCounter-player-player-2073627-25, strength=34, opId=player-counter-intel-player-to-player-25, lapsed cleanly.`
  - `Phase 3 PASS: espionage produced report on enemy court containing enemy-bloodline-marshal and expired cleanly.`
  - `Phase 4 PASS: baselineChance=0.651, defendedChance=0.331, watchId=dynastyCounter-player-player-2764827-25, legitimacy 70->71.`

## Unity-Side Simplifications Deferred

- The retained player-side watch/report state is still intentionally lane-local
  and does **not** widen Claude's AI-owned `DynastyOperationComponent` graph.
- Snapshot/save-load integration for `IntelligenceReportElement` buffers and
  `PlayerCounterIntelligenceComponent` watch state is not part of 3C.
- The active watch keeps compact last-source interception telemetry rather than
  a full per-source retained history buffer.
- Dossier-backed sabotage support remains browser-specified and was read during
  this slice, but sabotage resolution itself is still deferred; 3C extends only
  the watch/report seam that sabotage can later read.

## Exact Next Action

1. Stage the player covert ops 3C files plus continuity/contract updates and
   commit them on `codex/unity-player-counter-intelligence`.
2. Push the branch, merge it to `master`, and rerun the full governed
   validation gate on merged `master`.
3. After the landing continuity pass, close the player covert ops lane and
   claim the next Codex lane from the directive order, which is the player HUD
   / realm-condition legibility track.
