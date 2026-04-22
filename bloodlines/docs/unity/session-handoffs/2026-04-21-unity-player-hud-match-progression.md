# 2026-04-21 Unity Player HUD Match Progression

## Goal

Land the next player-HUD slice for Priority 5 by projecting the browser's match-progression snapshot into a Unity ECS HUD read-model, exposing a parseable debug seam, and proving the stage / phase / world-pressure / Great Reckoning readout with a dedicated smoke validator.

## Browser Reference

- `src/game/core/simulation.js`
  - `computeMatchProgressionState` lines `13426-13555`
  - `updateMatchProgressionState` lines `13557-13648`
  - `getMatchProgressionSnapshot` lines `13650-13658`
  - `declareInWorldTime` lines `13800-13809`
- `tests/runtime-bridge.mjs`
  - line `7521` snapshot export seam
  - lines `7773-7871` stage / phase / declaration / restore assertions
  - lines `7923-7975` Great Reckoning and world-pressure assertions
  - line `8133` Stage 1 AI-gating assertion
  - line `8185` Stage 3 raid-unlock assertion

## Work Completed

- Added the lane-owned runtime HUD read-model under `unity/Assets/_Bloodlines/Code/HUD/`:
  - `MatchProgressionHUDComponent.cs`
  - `MatchProgressionHUDSystem.cs`
- `MatchProgressionHUDSystem` now:
  - attaches `MatchProgressionHUDComponent` to the existing match-progression singleton entity if missing
  - projects stage number / id / label, phase id / label, readiness, next-stage id / label, in-world days / years, declaration count, dominant-leader telemetry, Great Reckoning telemetry, and resolved world-pressure telemetry into a single HUD read-model
  - resolves world pressure from the dominant-leader faction when present, otherwise from the active Great Reckoning target, without mutating simulation state
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.HUD.cs` with:
  - `TryDebugGetMatchHUDSnapshot(out string readout)`
  - structured output shaped as a single parseable `MatchHUD|Key=Value|...` string
- Added dedicated validation surfaces:
  - `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchProgressionHUDSmokeValidation.cs`
  - `scripts/Invoke-BloodlinesUnityMatchProgressionHUDSmokeValidation.ps1`
- The dedicated HUD smoke now proves four phases:
  1. founding baseline with emergence phase and quiet world pressure
  2. Stage 4 commitment projection with readiness, next-stage label, and declaration count
  3. dominant-leader world-pressure projection
  4. Great Reckoning convergence projection
- Narrow shared-file edits applied:
  - `unity/Assembly-CSharp.csproj`
  - `unity/Assembly-CSharp-Editor.csproj`

## Validation Proof

- Runtime build:
  - `Build succeeded.`
  - `0 Warning(s)`
  - `0 Error(s)`
- Editor build:
  - `113 Warning(s)`
  - `0 Error(s)`
- Dedicated HUD smoke:
  - `BLOODLINES_MATCH_PROGRESSION_HUD_SMOKE PASS`
  - `Phase 1 PASS: founding stage surfaces emergence phase, quiet world pressure, and InWorldDays=12.0.`
  - `Phase 2 PASS: stage-4 commitment snapshot surfaces readiness, next-stage label, and declaration count.`
  - `Phase 3 PASS: dominant leader 'player' surfaces overwhelming world pressure level 2 and score 6.`
  - `Phase 4 PASS: Great Reckoning target 'enemy' surfaces convergence pressure level 3 at share 0.720.`
- Bootstrap runtime smoke:
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.003, productionProgressLatestRatio=0.083, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.001, constructionProgressLatestRatio=0.915, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.70, trickleLatestFood=44.04, trickleInitialWater=31.93, trickleLatestWater=33.93, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
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

## Unity-Side Simplifications Deferred

- This slice creates the match HUD read-model and proof seam only; it does not yet render a new on-screen player HUD panel.
- The HUD read-model lives on the match-progression singleton entity rather than a dedicated player-UI presentation entity.
- World-pressure source breakdown, fortification legibility, and victory-distance readouts remain follow-up slices inside the same lane.
- The still-root-pinned bootstrap-runtime and scene-shell wrappers are not yet repaired in-repo, so worktree-local wrapper copies remain necessary for truthful clean-worktree validation.

## Exact Next Action

Stage the match-progression HUD slice plus continuity updates on `codex/unity-player-hud-match-progression`, push the branch, merge it to `master`, rerun the governed merged-master gate plus the dedicated match-progression HUD smoke, then write the landing handoff and revision bump.
