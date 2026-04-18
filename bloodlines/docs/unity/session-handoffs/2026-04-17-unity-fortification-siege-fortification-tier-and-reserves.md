# 2026-04-17 Unity Fortification Siege Fortification Tier And Reserves

## Goal

First sub-slice of the `fortification-siege-imminent-engagement` Tier 1 lane.
Ports the defender-side fortification floor from the browser runtime into ECS:

1. deterministic fortification tier advancement from completed linked
   structures
2. reserve-duty cycling for damaged defenders and ready reserve mustering
3. the neutral fortification faith-ward seam required by the browser contract,
   without wiring its combat multipliers yet

## Browser Reference

- `src/game/core/simulation.js:189-238` - fortification / siege canon constants
  block (`FORTIFICATION_ECOSYSTEM_RADIUS_TILES`,
  `FORTIFICATION_AURA_RADIUS_TILES`, `FORTIFICATION_THREAT_RADIUS_TILES`,
  reserve thresholds, triage healing, field-water constants, assault strain).
- `src/game/core/simulation.js:408-418` - `DEFAULT_FORTIFICATION_WARD`.
- `src/game/core/simulation.js:11227` - `advanceFortificationTier(...)`.
- `src/game/core/simulation.js:11253` - `getFortificationFaithWardProfile(...)`
  interface and returned ward-profile fields.
- `src/game/core/simulation.js:11689` -
  `commitSettlementReadyReserves(...)`.
- `src/game/core/simulation.js:11875` - `tickFortificationReserves(...)`.

## Canon Reference

- `04_SYSTEMS/TERRITORY_SYSTEM.md` - `Design Content (Added 2026-04-14) —
  Defensive Fortification Doctrine Integration`.
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md` - `Pillar 2 implications —
  Connected defensive ecosystem`, `Pillar 3 implications — Defensive leverage`,
  `Pillar 4 implications — Bloodline keeps as apex defensive structures`, and
  `Implications for AI Planning`.
- `04_SYSTEMS/FORTIFICATION_SYSTEM.md` - `Settlement Classes and Defensive
  Ceilings`, `Defensive Ecosystem (Connective Tissue)`, `Bloodline Presence
  Bonuses`, and `Implementation Milestones (Future Work)` Phase F1 / F3.

## Work Completed

### Fortification canon and component seam

- `unity/Assets/_Bloodlines/Code/Fortification/FortificationCanon.cs` now ports
  the browser fortification and siege constants block into Unity with the same
  numeric defaults for fortification radii, reserve thresholds, field-water
  strain, and assault strain.
- `unity/Assets/_Bloodlines/Code/Components/FortificationComponent.cs` now
  carries the settlement-side fortification profile:
  `Tier`, `Ceiling`, ecosystem / aura / threat / reserve radii, keep presence
  radius, plus a neutral fortification faith-ward seam mirroring the browser
  ward-profile fields (`FaithWardId`, sight bonus, defender / reserve /
  loyalty / enemy-speed multipliers, surge flag).
- `unity/Assets/_Bloodlines/Code/Components/FortificationReserveComponent.cs`
  now carries reserve runtime state and tuning:
  muster interval, reserve heal rate, retreat threshold, recovery threshold,
  triage radius, threat-active flag, ready / mustering / recovering / fallback
  counts, and `LastCommittedCount`.

### Fortification runtime systems

- `unity/Assets/_Bloodlines/Code/Fortification/AdvanceFortificationTierSystem.cs`
  resolves fortification tier deterministically from completed
  `FortificationBuildingContributionComponent` links and syncs the result back
  onto `SettlementComponent.FortificationTier` / `FortificationCeiling`.
- `unity/Assets/_Bloodlines/Code/Fortification/FortificationReserveSystem.cs`
  implements the first governed reserve loop:
  - low-health engaged defenders fall back when hostiles are inside the
    fortification threat radius
  - fallback / recovering defenders heal while inside the triage radius
  - recovered defenders return to the ready pool after clearing the recovery
    threshold
  - ready reserves muster forward when the frontline falls below the
    tier-scaled target
- New support data under `unity/Assets/_Bloodlines/Code/Fortification/`:
  - `FortificationBuildingContributionComponent.cs`
  - `FortificationSettlementLinkComponent.cs`
  - `FortificationReserveAssignmentComponent.cs`
  - `FortificationCombatantTag.cs`

### Debug surface

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Fortification.cs`
  adds the lane-owned fortification inspection and control seam:
  - `TryDebugGetFortificationTier(...)`
  - `TryDebugGetReserveCount(...)`
  - `TryDebugForceMuster(...)`

### Governed validator and wrapper

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFortificationSmokeValidation.cs`
  proves four isolated ECS phases:
  1. tier-0 baseline
  2. deterministic tier advancement to tier 1 from a linked completed building
  3. retreat + reserve-muster cycle
  4. triage healing back to the ready threshold
- `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1` now provides
  the lane-specific governed wrapper through the canonical Unity wrapper lock.

## Scope Discipline

- No live bootstrap-scene integration. All verification for this slice runs in
  isolated ECS worlds only, per lane instruction.
- No edits under `unity/Assets/_Bloodlines/Code/Combat/**`.
- No siege logistics yet. `tickSiegeSupportLogistics(...)` and field-water
  attrition remain sub-slice 2.
- No imminent-engagement scanning or posture changes yet. Those remain
  sub-slice 3.
- No fortification faith-ward multiplier application. The browser ward-profile
  seam now exists in `FortificationComponent`, but combat, loyalty, and reserve
  systems still treat it as neutral until the later cross-lane wiring slice.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  - passed:
    `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.003, productionProgressLatestRatio=0.083, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.001, constructionProgressLatestRatio=0.915, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.38, trickleLatestFood=43.45, trickleInitialWater=36.84, trickleLatestWater=40.41, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
  - passed:
    `Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityGraphicsRuntimeValidation.ps1`
  - passed:
    `Graphics runtime validation passed: unitProxies=16, buildingProxies=9, factionTintAttached=25, expectedUnitsWithDefinition=16, expectedBuildingsWithDefinition=9.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  - passed:
    `Bootstrap scene shell validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity with canonical map Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset.`
  - passed:
    `Gameplay scene shell validation passed for Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityConvictionSmokeValidation.ps1`
  - passed:
    `Conviction smoke validation passed: neutralPhase=True, moralAscentPhase=True, cruelDescentPhase=True, bandEffectsPhase=True. Baseline: band=Neutral, score=0. Moral ascent: moralBand=Moral@50, apexBand=ApexMoral@90. Cruel descent: cruelBand=Cruel@-30, apexBand=ApexCruel@-130. Band effects: apexMoral.stabilization=1.22, apexCruel.capture=1.22, neutral.stabilization=1.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityDynastySmokeValidation.ps1`
  - passed:
    `Dynasty smoke validation passed: spawnPhase=True, agingPhase=True, successionPhase=True, interregnumPhase=True. Spawn: memberCount=8, headAge=38, legitimacy=58. Aging: initialAge=38, finalAge=40, delta=2. Succession: newRulerTitle=Eldest Heir, age=19. Interregnum: felledThroughChain=8, interregnum=True.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityFaithSmokeValidation.ps1`
  - passed:
    `Faith smoke validation passed: baselinePhase=True, exposureThresholdPhase=True, commitmentPhase=True, intensityTierPhase=True. Baseline: selected=None, intensity=0, level=Unawakened. Threshold: exposureAtBlock=60, requiredThreshold=100, result=ExposureBelowThreshold. Commit: selected=BloodDominion, path=Dark, intensity=20, level=2, recommitBlock=AlreadyCommitted. IntensityTier: apex@80=level5, fervent@60=level4, clamped@150=100.`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-fortification-siege-2026-04-17 -WrapperScript scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
  - passed:
    `Fortification smoke validation passed: baselinePhase=True, tierAdvancePhase=True, reserveMusterPhase=True, reserveRecoveryPhase=True. Baseline: tier=0, ceiling=3. TierAdvance: tier=1, ceiling=3, contributionApplied=1. ReserveMuster: retreatDuty=Fallback, reserveDuty=Muster, committed=1. ReserveRecovery: duty=Ready, healthRatio=0.95, readyReserveCount=1.`
- `node tests/data-validation.mjs`
- `node tests/runtime-bridge.mjs`
- `powershell -ExecutionPolicy Bypass -File scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`

## Next Action

Proceed to sub-slice 2 on the same lane:

1. add `SiegeStateComponent` and `SiegeSupplyLogisticsComponent`
2. port the browser siege support / field-water strain loop from
   `tickSiegeSupportLogistics(...)`
3. add `BloodlinesSiegeSmokeValidation.cs` plus
   `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
4. keep all verification isolated to validator worlds until the later
   bootstrap-integration slice

## Branch

- Branch: `codex/unity-fortification-siege`
- Validation lock session: `codex-fortification-siege-2026-04-17`
- Lane status after this sub-slice: active
