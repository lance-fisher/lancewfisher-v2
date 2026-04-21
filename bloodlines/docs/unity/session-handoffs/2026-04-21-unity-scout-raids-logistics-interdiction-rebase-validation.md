# 2026-04-21 Unity Scout Raids And Logistics Interdiction Rebase Validation

## Goal

Rebase the existing Codex-owned `scout-raids-logistics-interdiction` lane onto
current `origin/master`, preserve the already-landed fortification closure state
through sub-slice 13, and rerun the full governed validation gate on the rebased
scout lane before merge.

## Base And Repo Reality

- Branch in flight: `codex/unity-scout-raids-logistics-interdiction`
- Rebased onto `origin/master` commit `8cefd4b`
- Contract revision advanced to `50`
- The multi-day directive's fortification Priority 1 is stale against the
  repository. `origin/master` already contains fortification sub-slices 10-13
  plus the queue-close handoff. This session therefore continued the already
  claimed scout-raids lane instead of reopening a closed fortification slice.

## Browser References

- `src/game/core/simulation.js`
  - `SCOUT_RAID_TARGET_RANGE` (35)
  - `SCOUT_RAID_RETREAT_DISTANCE` (36)
  - `SCOUT_RAID_LOYALTY_RADIUS` (37)
  - `isBuildingUnderScoutRaid` (2046)
  - `getRaidRetreatCommand` (2349)
  - `executeScoutRaid` (2362)
  - `executeScoutLogisticsInterdiction` (2515)
- `src/game/core/ai.js`
  - read-only confirmation only; no AI-lane writes

## Work Completed

- Rebasing the scout-raids branch onto `origin/master` produced runtime-clean
  results; only continuity docs and the contract conflicted.
- Conflict resolution preserved the latest fortification closure state from
  `master`, kept the scout-raids lane active, and updated
  `docs/unity/CONCURRENT_SESSION_CONTRACT.md` to revision `50`.
- Added this session handoff and linked it from the scout-raids lane authority
  list.
- Refreshed the local gitignored `unity/Assembly-CSharp-Editor.csproj` include
  list so the governed editor build sees three already-present fortification
  editor validators that were missing from the generated project file:
  - `BloodlinesBreachSealingTierScalingSmokeValidation.cs`
  - `BloodlinesBreachSealingWorkerLocalitySmokeValidation.cs`
  - `BloodlinesFortificationRepairNarrativeSmokeValidation.cs`
- Re-ran the full governed validation gate on the rebased worktree at
  `D:\BLAICD\bloodlines`.
- Used worktree-local temporary copies of the root-pinned bootstrap-runtime and
  scene-shell wrappers so the validation pass exercised the rebased scout branch
  instead of the canonical junction checkout.

## Validation Proof

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
   - `Build succeeded.`
   - `0 Warning(s)`
   - `0 Error(s)`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
   - `Build succeeded.`
   - `113 Warning(s)`
   - `0 Error(s)`
3. Bootstrap runtime smoke
   - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.003, productionProgressLatestRatio=0.083, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.001, constructionProgressLatestRatio=0.914, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.71, trickleLatestFood=44.05, trickleInitialWater=31.94, trickleLatestWater=33.94, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
4. Combat smoke
   - `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
5. Scene shells
   - `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
   - `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
6. Fortification smoke
   - `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. Baseline: tier=0, ceiling=3. TierAdvance: tier=1, ceiling=3, contributionApplied=1. ReserveMuster: retreatDuty=Fallback, reserveDuty=Muster, committed=1. ReserveRecovery: duty=Ready, healthRatio=0.95, readyReserveCount=1.`
7. Siege smoke
   - `Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. Baseline: strain=0, attrition=False, supportRefreshCount=0. Strain: status=Strained, strain=6.25, attackMultiplier=0.88, speedMultiplier=0.90. Recovery: supported=True, strain=3.88, supportRefreshCount=51. Support: engineerSupport=True, supplySupport=True, refreshCount=3, suppliedUntil=11.20.`
8. `node tests/data-validation.mjs`
   - `Bloodlines data validation passed.`
9. `node tests/runtime-bridge.mjs`
   - `Bloodlines runtime bridge validation passed.`
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`
   - `STALENESS CHECK PASSED: Contract revision=50, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
11. Dedicated scout raid smoke
   - `BLOODLINES_SCOUT_RAID_AND_INTERDICTION_SMOKE PASS`
   - `PhaseFarmRaid PASS: food=38.00 influence=18.00 loyalty=72.00 raidedUntil=25.00 retreatX=12.00.`
   - `PhaseWellRaidBlocksFieldWater PASS: support resumed after expiry; refreshCount=1 suppliedUntil=42.00 water=7.80.`
   - `PhaseDropoffReroute PASS: worker routed to clear hall at (42.00,0.00).`
   - `PhaseSupplyWagonInterdiction PASS: interdictedUntil=19.00 recoveryUntil=31.00 wagonMove=(160.00,100.00).`

## Unity-Side Simplifications Deferred

- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  path-pinned to `D:\ProjectsHome\Bloodlines\unity`, so clean worktrees still
  need temporary worktree-local copies for truthful validation.
- The Unity-generated `.csproj` files remain gitignored and can drift across
  branch switches. This session only refreshed the local editor registrations
  required to make the governed `dotnet build` gate truthful on this worktree.
- No gameplay/runtime scout-raid behavior changed during this rebase-validation
  pass beyond preserving the already-authored scout lane on the newer master
  base.

## Exact Next Action

1. Push `codex/unity-scout-raids-logistics-interdiction` to origin with
   `--force-with-lease` because the branch was rebased.
2. Merge it to `master` with `git merge --no-ff`.
3. Re-run the full governed validation gate on merged `master`.
4. Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and
   `continuity/PROJECT_STATE.json` with the merged scout-raids result.
