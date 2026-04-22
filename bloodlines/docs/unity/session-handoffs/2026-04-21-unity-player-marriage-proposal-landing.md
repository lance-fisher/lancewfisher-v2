# 2026-04-21 Unity Player Marriage Proposal Landing

## Goal

Land the validated `codex/unity-player-marriage-proposal` branch onto `master`,
rerun the governed validation gate on merged `master`, and update continuity so
the next Codex session starts sub-slice 2B acceptance instead of re-landing
proposal execution.

## Merge State

- Merge commit on `master`: `21550da3`
- Merged branch: `codex/unity-player-marriage-proposal`
- Contract revision advanced: `53 -> 54`

## Browser References

- `src/game/core/simulation.js`
  - `MARRIAGE_REGENCY_LEGITIMACY_COSTS` (6091-6095)
  - `getMarriageAuthorityProfile` (6134-6190)
  - `getMarriageEnvoyProfile` (6192-6215)
  - `buildMarriageGovernanceStatus` (6217-6230)
  - `applyMarriageGovernanceLegitimacyCost` (6232-6245)
  - `getMarriageProposalContext` (6247-6274)
  - `getMarriageProposalTerms` (6296-6325)
  - `factionAllowsPolygamy` (6431-6433)
  - `memberHasActiveMarriage` (7260-7264)
  - `proposeMarriage` (7340-7386)
- `tests/runtime-bridge.mjs`
  - marriage proposal / acceptance proof block (2072-2113)

## Work Completed

- Merged `codex/unity-player-marriage-proposal` into `master` with
  `git merge --no-ff`.
- Re-ran the governed validation gate on merged `master` in
  `D:\BLM13\bloodlines\bloodlines`.
- Re-ran the dedicated player-marriage proposal smoke on merged `master` so the
  lane has branch-proof and merged-master proof for the same proposal behavior.
- Updated continuity surfaces and the concurrent-session contract so the lane is
  now represented as sub-slice 2A landed on `master`, with sub-slice 2B
  acceptance as the next action on a fresh branch.

## Validation Proof

1. `dotnet build unity/Assembly-CSharp.csproj -nologo`
   - `Build succeeded.`
   - `0 Warning(s)`
   - `0 Error(s)`
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
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
   - `STALENESS CHECK PASSED: Contract revision=53, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
11. Dedicated player-marriage proposal smoke
   - `BLOODLINES_PLAYER_MARRIAGE_PROPOSAL_SMOKE PASS`
   - `Phase 1 PASS: PendingProposalCount=0`
   - `Phase 2 PASS: PendingProposalCount=1, source=player-bloodline-heir, target=enemy-bloodline-heir, legitimacy=57`
   - `Phase 3 PASS: duplicate request preserved a single pending proposal`
   - `Phase 4 PASS: active marriage blocked proposal creation`

## Unity-Side Simplifications Deferred

- The checked-in bootstrap-runtime and canonical scene-shell wrappers are still
  path-pinned to `D:\ProjectsHome\Bloodlines`; clean worktrees still need
  temporary local copies for truthful validation.
- Proposal entities still reuse the existing `MarriageProposalComponent` shape
  only; the richer browser-side governance preview object is still deferred.
- The duplicate guard still blocks the exact source-member / target-member
  pending pair only, not a broader inbox-wide dedupe policy.

## Exact Next Action

1. Create fresh branch `codex/unity-player-marriage-acceptance` from current
   `master`.
2. Port `acceptMarriage` and `getMarriageAcceptanceTerms` into
   `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/`.
3. Add the dedicated acceptance smoke proving marriage creation, legitimacy
   deltas, no-pending-proposal rejection, and heir-regency cost deduction.
