# Unity Player Covert Ops Sub-Slice 3B: Assassination And Sabotage

- Date: 2026-04-21
- Lane: `player-covert-ops`
- Branch: `codex/unity-player-assassination-sabotage`
- Status: complete on branch, pending merge to `master`

## Goal

Port the browser's player-facing `startAssassinationOperation` and
`startSabotageOperation` dispatch seams into ECS under
`unity/Assets/_Bloodlines/Code/PlayerCovertOps/`, keep the work inside the
Codex-owned player covert-ops lane, extend the debug readout, and prove the new
targeting paths with the dedicated player covert ops smoke validator.

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

## Work Completed

- Extended `PlayerCovertOpsRequestComponent` with `Subtype` so one lane-local
  request shape can drive espionage, assassination, and sabotage.
- Extended `PlayerCovertOpsResolutionComponent` with subtype, location, target,
  and defense telemetry fields so the debug surface and smoke validator can
  assert structured sabotage and assassination metadata.
- Reworked `PlayerCovertOpsSystem` so one system now dispatches:
  - espionage from the existing 3A path
  - assassination with canonical cost `gold=85`, `influence=28`, duration
    `34 / 86400f`
  - sabotage with browser subtype costs and durations:
    - `gate_opening`: `gold=60`, `influence=18`, `28 / 86400f`
    - `fire_raising`: `gold=40`, `influence=12`, `24 / 86400f`
    - `supply_poisoning`: `gold=50`, `influence=15`, `30 / 86400f`
    - `well_poisoning`: `gold=70`, `influence=20`, `32 / 86400f`
- Ported the new dispatch gates and creation path:
  - source and target factions must exist and differ
  - combined active dynasty-operation cap still honors the browser limit `6`
    by combining `DynastyOperationLimits.CountActiveForFaction(...)` with
    player-owned live op entities
  - assassination requires a live enemy dynasty member in the target faction's
    `DynastyMemberRef` roster buffer and blocks duplicate active assassination
    ops against the same `TargetMemberId`
  - sabotage requires a live enemy building entity, validates subtype legality
    from `BuildingTypeComponent`, and rejects invalid pairings like
    `gate_opening` against non-gate structures
  - assassination operator priority follows the browser's player path:
    `Spymaster`, then `Diplomat`, then `Merchant`
  - sabotage operator priority follows the browser's available covert roster in
    Unity: `Spymaster`, then `Diplomat`, then any available dynasty member on
    `DynastyPath.CovertOperations`
- Extended `BloodlinesDebugCommandSurface.PlayerCovertOps.cs` with:
  - `TryDebugIssuePlayerAssassination(sourceFactionId, targetFactionId, targetMemberId)`
  - `TryDebugIssuePlayerSabotage(sourceFactionId, sabotageSubtype, targetFactionId, targetBuildingEntityIndex)`
  - richer structured readout fields for subtype, target entity index, target
    member id, target label, location label, intelligence-support state, and
    defense telemetry
- Expanded `BloodlinesPlayerCovertOpsSmokeValidation` from 4 phases to 6:
  1. clean baseline with no player covert ops
  2. successful espionage dispatch creates one op and deducts cost
  3. insufficient influence blocks dispatch and preserves resources
  4. active player ops cannot exceed the canonical cap of `6`
  5. assassination must target a live enemy dynasty member and deduct cost
  6. sabotage must target a live enemy building and preserve canonical subtype
     legality plus cost deduction
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
  - `Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. activeScene='Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity', world='Default World', mapId='ironmark_frontier', pendingBootstrap=0, bootstrapConfig=1, realmCycleConfig=1, factions=3/3, buildings=13/11, units=19/18, resourceNodes=13/13, controlPoints=4/4, settlements=2/2, debugBridgePresent=True, debugBridgeChildren=55, expectedProxyMinimum=47, expectDebugBridge=True, commandSurfacePresent=True, controlledUnits=8, commandSelection=8, controlGroup2=8, commandFrameSucceeded=True, productionCancelVerified=True, productionQueued=True, productionBuildingType='command_hall', producedUnitType='villager', productionProgressInitialRatio=0.003, productionProgressLatestRatio=0.084, productionProgressAdvancementVerified=True, constructionPlaced=True, constructionBuildingType='dwelling', constructionSites=0, constructionProgressInitialRatio=0.001, constructionProgressLatestRatio=0.915, constructionProgressAdvancementVerified=True, populationCap=24, constructedProductionBuildingPlaced=True, constructedProductionBuildingType='barracks', constructedProductionSites=0, constructedProductionQueued=True, constructedProductionUnitType='militia', gatherResource='gold', gatherAssigned=True, gatherAssignedWorkerCount=5, gatherInitialFactionGold=45.0, gatherLatestFactionGold=145.0, gatherDepositObserved=True, trickleInitialFood=39.71, trickleLatestFood=44.04, trickleInitialWater=31.94, trickleLatestWater=33.94, trickleGainObserved=True, starvationForced=True, starvationIncludeWaterCrisis=True, starvationPreviousPopulation=14, starvationExpectedPopulation=12, starvationLatestPopulation=12, starvationObserved=True, loyaltyPreviousValue=70.00, loyaltyLatestValue=60.00, loyaltyExpectedMaximumAfterCycle=62.00, loyaltyDeclineObserved=True, capPressureForced=True, capPressureTotalAfterSpike=23, capPressureCapAfterSpike=24, capPressureLoyaltyBeforeCycle=60.00, capPressureLatestLoyalty=59.00, capPressureObserved=True, aiFaction='enemy', aiInitialGold=220.00, aiLatestGold=100.00, aiInitialUnitCount=6, aiLatestUnitCount=7, aiGoldGainObserved=False, aiUnitGainObserved=True, aiActivityObserved=True, aiInitialBuildingCount=4, aiLatestBuildingCount=6, aiConstructionObserved=True, stabilitySurplusForced=True, stabilitySurplusLoyaltyBefore=59.00, stabilitySurplusLoyaltyLatest=60.00, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True, aiMilitaryOrdersIssued=2, aiDwellings=2, aiFarms=1, aiWells=1, aiBarracks=1.`
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
- Contract staleness before the revision-62 continuity pass:
  - `STALENESS CHECK PASSED: Contract revision=61, last-updated=2026-04-21 is current. Latest handoff: 2026-04-21-unity-fortification-repair-narrative.md (2026-04-21).`
- Dedicated smoke:
  - `BLOODLINES_PLAYER_COVERT_OPS_SMOKE PASS`
  - `Phase 1 PASS: ActivePlayerCovertOpCount=0`
  - `Phase 2 PASS: espionage created, gold=155, influence=64, readout='ActivePlayerCovertOpCount=1`
  - `Phase 3 PASS: insufficient influence blocked dispatch and preserved stockpile`
  - `Phase 4 PASS: active espionage ops capped at 6 with readout 'ActivePlayerCovertOpCount=6`
  - `Phase 5 PASS: assassination targeted memberId=enemy-bloodline-marshal, title=War Captain, gold=215, influence=152.`
  - `Phase 6 PASS: sabotage targeted entityIndex=22, subtype=gate_opening, gold=200, influence=102.`

## Unity-Side Simplifications Deferred

- The player covert-op entities still intentionally do **not** reuse
  `DynastyOperationComponent` because `DynastyOperationKind` remains inside
  Claude's AI-owned lane and still does not expose player espionage,
  assassination, sabotage, or counter-intelligence kinds.
- `getAssassinationContest` is only partially mirrored here:
  - Unity now reads defender spymaster renown, target-role exposure, nearest
    same-faction keep tier, and head-of-bloodline protection
  - active counter-intelligence watch, ward defense, and explicit bloodline
    guard bonuses remain deferred to sub-slice 3C
- `getSabotageTerms` is also partially simplified:
  - Unity validates subtype legality and computes a projected chance from
    operator renown plus nearby settlement fortification tier
  - dossier-driven support, retaliation metadata, and live intelligence-report
    bonuses remain deferred to sub-slice 3C
- The browser's sabotage operation history, retaliation routing, and
  resolution-time hostility effects are not part of 3B. This slice ports only
  the player dispatch gates, cost deduction, and live operation creation that
  the directive explicitly scoped.

## Exact Next Action

1. Stage the player covert ops 3B files plus continuity/contract updates and
   commit them on `codex/unity-player-assassination-sabotage`.
2. Push the branch, merge it to `master`, and rerun the full governed
   validation gate on merged `master`.
3. After the landing continuity pass, start sub-slice 3C on a fresh branch for
   player counter-intelligence and intelligence-report buffers.
