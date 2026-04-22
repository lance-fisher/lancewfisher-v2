# Unity Player Covert Ops Sub-Slice 3A: Foundation

- Date: 2026-04-21
- Lane: `player-covert-ops`
- Branch: `codex/unity-player-covert-ops-foundation`
- Status: complete on branch, pending merge to `master`

## Goal

Port the browser's player-facing `startEspionageOperation` dispatch seam into
Unity ECS under `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`, add a
player-owned covert-op entity shape that avoids Claude's AI-owned operation
files, expose a player debug surface, and prove the foundation with a dedicated
4-phase smoke validator.

## Browser Reference

- `src/game/core/simulation.js`
  - `DYNASTY_OPERATION_ACTIVE_LIMIT` (17)
  - `getActiveDynastyOperationForTargetFaction` (4084-4087)
  - `getActiveIntelligenceReport` (4097-4099)
  - `tickDynastyIntelligenceReports` (4106-4113)
  - `ESPIONAGE_COST` (9764)
  - `ESPIONAGE_DURATION_SECONDS` (9768)
  - `INTELLIGENCE_REPORT_DURATION_SECONDS` (9772)
  - `getEspionageContest` (10187-10212)
  - `getEspionageTerms` (10248-10281)
  - `startEspionageOperation` (10876-10910)
- `tests/runtime-bridge.mjs`
  - player espionage + report lifecycle assertions (3490-3543)

## Work Completed

- Added the player-owned covert-op lane under
  `unity/Assets/_Bloodlines/Code/PlayerCovertOps/`:
  - `CovertOpKindPlayer`
  - `PlayerCovertOpsRequestComponent`
  - `PlayerCovertOpsResolutionComponent`
  - `PlayerCovertOpsSystem`
- Ported the player-side espionage dispatch gates and creation path:
  - source and target factions must exist and differ
  - source faction must carry `ResourceStockpileComponent`
  - source and target factions must each carry a dynasty roster
  - a same-source same-target active espionage op is blocked
  - a live report window on the same target is blocked via
    `ReportExpiresAtInWorldDays`
  - canonical active-cap enforcement combines
    `DynastyOperationLimits.CountActiveForFaction(...)` with player-owned live
    covert-op entities, preserving the browser's limit `6` without widening the
    AI-owned `DynastyOperationComponent`
  - operator selection follows the browser role priority:
    `Spymaster`, then `Diplomat`, then `Merchant`
  - dispatch deducts the canonical espionage cost `gold=45` and
    `influence=16`
  - dispatch creates a player-owned live operation entity with start time,
    resolve time, report expiry, operator metadata, projected chance, success
    score, and escrow-cost telemetry
- Added `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.PlayerCovertOps.cs`
  with:
  - `TryDebugIssuePlayerEspionage(sourceFactionId, targetFactionId)`
  - `TryDebugGetPlayerCovertOps(factionId, out readout)`
- Added the dedicated validator
  `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPlayerCovertOpsSmokeValidation.cs`
  plus wrapper
  `scripts/Invoke-BloodlinesUnityPlayerCovertOpsSmokeValidation.ps1`
- The dedicated validator proves:
  1. clean baseline with no player covert ops
  2. successful espionage dispatch creates one op and deducts cost
  3. insufficient influence blocks dispatch and preserves resources
  4. active player espionage ops cannot exceed the canonical cap of `6`
- Ran the full governed 10-step validation gate in
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
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.003, productionProgressLatestRatio=0.083, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.001, constructionProgressLatestRatio=0.914, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.48, trickleLatestFood=43.81, trickleInitialWater=31.73, trickleLatestWater=33.73, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
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
- Contract staleness:
  - `STALENESS CHECK PASSED: Contract revision=59, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
- Dedicated smoke:
  - `BLOODLINES_PLAYER_COVERT_OPS_SMOKE PASS`
  - `Phase 1 PASS: ActivePlayerCovertOpCount=0`
  - `Phase 2 PASS: espionage created, gold=155, influence=64, readout='ActivePlayerCovertOpCount=1`
  - `Phase 3 PASS: insufficient influence blocked dispatch and preserved stockpile`
  - `Phase 4 PASS: active espionage ops capped at 6 with readout 'ActivePlayerCovertOpCount=6`

## Unity-Side Simplifications Deferred

- The player covert-op entities intentionally do **not** reuse
  `DynastyOperationComponent` or `DynastyOperationLimits.BeginOperation(...)`
  because `DynastyOperationKind` lives under Claude's AI-owned lane and does
  not currently include espionage/sabotage/assassination/counter-intelligence.
  This slice therefore reuses only the active-cap constant/helper and keeps the
  live player operation entities lane-local.
- `tickDynastyIntelligenceReports` is not fully ported yet. Sub-slice 3A stores
  `ReportExpiresAtInWorldDays` on the live op entity so duplicate dispatch
  gating can already mirror the browser behavior, but the per-faction
  intelligence-report buffer/readout is deferred to sub-slice 3C.
- `getEspionageContest` is partially simplified in Unity for this slice:
  offense uses operator renown + 32, defense uses target spymaster/diplomat
  renown * 0.55 plus fortification tier * 6, while ward defense and active
  counter-intelligence defense remain deferred to the later counter-intel lane
  work.
- The debug surface only exposes espionage in 3A. Sabotage, assassination, and
  counter-intelligence issue/readout paths remain reserved for sub-slices 3B
  and 3C.

## Exact Next Action

1. Stage the player covert ops foundation files plus continuity/contract
   updates and commit them on `codex/unity-player-covert-ops-foundation`.
2. Push the branch, merge it to `master`, and rerun the full governed
   validation gate on merged `master`.
3. After the landing continuity pass, start sub-slice 3B on fresh branch
   `codex/unity-player-assassination-sabotage`.
