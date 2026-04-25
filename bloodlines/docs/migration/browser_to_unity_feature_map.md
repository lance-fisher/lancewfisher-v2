# Browser-to-Unity Feature Map

**Date:** 2026-04-25
**Author:** Claude Code (claude-sonnet-4-6)
**Purpose:** One-table mapping from every major browser feature/file to its Unity equivalent (or lack thereof), with migration status and recommended next action.

---

## Reading Key

| Status | Meaning |
|---|---|
| Complete | Unity implementation matches or exceeds browser. Done. |
| Verify | Unity has it; specific constants or edge cases need cross-checking. |
| Partial | Core system exists; specific sub-paths are missing. |
| Unity-Canonical | Unity model exceeds browser; browser is frozen earlier spec. |
| Not Started | Absent from Unity. Backlog item needed. |
| Browser-Only | No Unity equivalent needed. Browser-specific artifact. |

---

## Feature Map Table

| Browser Feature | Browser Source File | Unity Equivalent | Status | Migration Action | Notes |
|---|---|---|---|---|---|
| **Population pool (total/cap/reserved)** | `simulation.js: tickPopulationGrowth` | `Components/PopulationComponent.cs` | Complete | Verify growth interval=18s in EarlyGameConstants | Browser model is simpler (single int). Unity model is richer (see Unity-Canonical row below). |
| **Population growth formula** | `simulation.js: tickPopulationGrowth` | `Systems/PopulationGrowthSystem.cs` | Verify | Confirm 18s interval, 1 food + 1 water cost match Unity | Ironmark blood penalty in browser; verify Unity has equivalent modifier. |
| **4-tier productivity model (5% active duty)** | NOT IN BROWSER | `Systems/PopulationProductivitySystem.cs` + `EarlyGameConstants` | Unity-Canonical | Document in `01_CANON/UNITY_CANONICAL_ADVANCEMENTS_2026-04-25.md` | Owner direction 2026-04-25. 5% active-duty labor rate is canonical starting value. |
| **Military draft slider** | NOT IN BROWSER | `Systems/MilitaryDraftSystem.cs` + `EarlyGameComponents.MilitaryDraftComponent` | Unity-Canonical | Document as Unity-canonical advancement | Same owner direction as above. |
| **Passive resource trickle (farm, well)** | `simulation.js: tickPassiveResources` | `Economy/ResourceTrickleBuildingSystem.cs` | Verify | Confirm farm=0.5 food/s, well=0.45 water/s against Unity values | Critical balance constants. |
| **Worker node-gather (villager walk to deposit)** | `simulation.js: updateWorkerUnit` | `Systems/WorkerGatherSystem.cs` + `WorkerGatherComponent` | Complete | None | gatherRate * dt, carryCapacity=10 model ported. |
| **Worker slot buildings (data only in browser)** | `data/buildings.json` (fields unused by browser sim) | `Systems/WorkerSlotProductionSystem.cs` + `EarlyGameComponents.WorkerSlotBuildingComponent` | Unity-Canonical | Document; implement Worker Slot Assignment HUD (T2-A) | Slot model is Unity-canonical. Buildings.json has maxWorkerSlots/workerOutputPerSecond. No player UI exists yet. |
| **Iron mine smelting (wood fuel cost)** | `simulation.js` smelting logic | No system yet | Partial | Add smelting fuel consumption to `WorkerGatherSystem` or `WorkerSlotProductionSystem` | smeltingFuelRatio=0.5 wood per iron. Currently no Unity system enforces this. |
| **Building construction progress** | `simulation.js: attemptPlaceBuilding` | `Construction/ConstructionSystem.cs` | Complete | None | |
| **Building tier gating** | `simulation.js` build unlock logic | `Systems/BuildTierGatingSystem.cs` | Complete | None | New this branch. |
| **Faith: exposure accrual from sacred sites** | `simulation.js: updateFaithExposure` | `Faith/FaithExposureWalkerSystem.cs` | Complete | Verify exposure threshold=100, building amplifiers (1.8x/2.4x/3.0x/3.6x) | |
| **Faith: intensity tier resolution** | `simulation.js: updateFaithStructureIntensity` | `Faith/FaithIntensityResolveSystem.cs` | Complete | Verify tier thresholds: Apex=80/Fervent=60/Devout=40/Active=20/Latent=1 | |
| **Faith: commitment at threshold** | `simulation.js: chooseFaithCommitment` | `Faith/FaithExposureWalkerSystem.cs` (triggers commit) | Complete | Verify starting intensity=20 on first commit | |
| **Faith: doctrine effects (6 multipliers)** | `data/faiths.json` | `Faith/FaithDoctrineCanon.cs` + `FaithDoctrineSyncSystem.cs` | Complete | Verify individual doctrine multiplier values match faiths.json | |
| **Faith: covenant test** | `simulation.js: updateCovenantTests` | `Faith/CovenantTestQualificationSystem.cs` + `CovenantTestResolutionSystem.cs` | Complete | Verify: threshold=80, duration=180d, cooldown=120d, failure=-20 intensity/-8 leg/-6 loyalty, success floor=82/+8 leg | |
| **Faith: building regen rate** | `simulation.js: updateFaithStructureIntensity` | `Faith/FaithStructureRegenSystem.cs` | Complete | Verify max regen=1.4/s | Modified this branch. |
| **Faith: holy war runtime effects** | `simulation.js: tickFaithHolyWars` | Unverified — may be in `AI/AIHolyWarContextSystem.cs` | Partial | Verify per-tick combat/economic effects are applied during active holy war, not just at declaration/resolution | T2-D: verify gap exists, then implement if missing. |
| **Faith: divine right runtime** | `simulation.js: tickFaithDivineRightDeclarations` | Unverified | Partial | Same as holy war — verify runtime ticking | |
| **Faith: Verdant Warden** | `simulation.js` Verdant Warden logic | `Faith/VerdantWardenComponent.cs` + `VerdantWardenSupportSystem.cs` | Complete | None | |
| **Faith: player dispatch** | `main.js` UI faith controls | `Faith/PlayerCovenantTestDispatchSystem.cs` | Complete | None | |
| **Conviction: score and bands** | `simulation.js: getConvictionBand, recordConvictionEvent` | `Conviction/ConvictionScoring.cs` + `ConvictionScoringSystem.cs` | Complete | Verify band thresholds: Apex Moral≥75, Moral≥25, Neutral≥-24, Cruel≥-74 | |
| **Conviction: band effects** | `simulation.js: CONVICTION_BAND_EFFECTS` | `Conviction/ConvictionBandEffects.cs` | Complete | Verify pop growth multipliers: ApexMoral=1.08, Moral=1.03, Neutral=1.00, Cruel=0.97, ApexCruel=0.92 | |
| **Dynasty: member creation + roles** | `simulation.js: createDynastyState` | `Dynasties/DynastyBootstrap.cs` + `DynastyTemplates.cs` | Complete | None | |
| **Dynasty: aging** | `simulation.js` member age tick | `Dynasties/DynastyAgingSystem.cs` | Complete | None | |
| **Dynasty: succession chain** | `simulation.js: findAvailableSuccessor` | `Dynasties/DynastySuccessionSystem.cs` | Complete | Verify succession chain order: head→heir→commander→governor→diplomat→ideological→spymaster→merchant | |
| **Dynasty: legitimacy events** | `simulation.js: adjustLegitimacy` | `Dynasties/` (legitimacy adjustments in multiple systems) | Verify | Verify: commander killed=-9, capture=-12, head fall=-18, governor loss=-5, interregnum=-14, succession+7, ransom+4, rescue+6 | |
| **Dynasty: renown (cap=100)** | `simulation.js: awardRenownToFaction` | `Dynasties/DynastyRenownComponent.cs` + `RenownAwardSystem.cs` | Complete | Verify RENOWN_CAP=100, combat kill=+1, fortification kill=+2 | |
| **Dynasty: marriage (90d proposal, 280d gestation)** | `simulation.js: proposeMarriage` | `Dynasties/MarriageProposalExpirationSystem.cs` + `MarriageGestationSystem.cs` | Complete | Verify 90-day expiration, 280-day gestation in world-days | |
| **Dynasty: fallen ledger** | `simulation.js: appendFallenLedger` | Distributed in dynasty event systems | Verify | Confirm killed/captured/enslaved/ransomed member records are tracked | |
| **Dynasty: political events** | `simulation.js: dynasty political logic` | `Dynasties/DynastyPoliticalEventSystem.cs` | Complete | None | |
| **Dynasty: lesser house loyalty** | `simulation.js: updateLesserHouseLoyalty` | `Dynasties/LesserHouseLoyaltyDriftSystem.cs` | Complete | None | |
| **Dynasty: minor house levy** | `simulation.js` minor house mechanics | `Dynasties/MinorHouseLevySystem.cs` | Complete | Verify min loyalty=48 | |
| **Dynasty: succession crisis** | `simulation.js: succession crisis` | `Dynasties/SuccessionCrisisEvaluationSystem.cs` + `RecoverySystem.cs` | Complete | None | |
| **Dynasty: cross-match XP progression** | NOT IN BROWSER | `Dynasties/DynastyProgressionCanon.cs` + `DynastyProgressionUnlockSystem.cs` + `SpecialUnitSwapApplicatorSystem.cs` | Unity-Canonical | Document in `01_CANON/` | Owner direction 2026-04-19. Tier thresholds {0,100,350,850,1850}. |
| **Territory: CP capture + loyalty** | `simulation.js: updateControlPoints` | `Components/ControlPointComponent.cs` + `Systems/ControlPointCaptureSystem.cs` | Verify | Verify capture decay=2.5, proximity radius=138 tiles | |
| **Territory: governance recognition** | `simulation.js: tickTerritorialGovernanceRecognition` | `WorldPressure/TerritorialGovernanceRecognitionComponent.cs` + related systems | Complete | Verify acceptance thresholds: 65% non-allied, 60% allied, loyalty floor 80, victory 90, sustained 120s | |
| **Territory: governor specialization** | `simulation.js: GOVERNOR_STABILIZATION_BONUS` | `TerritoryGovernance/GovernorSpecializationSystem.cs` | Verify | Verify stabilization bonus=1.30, trickle bonus=1.22 | |
| **AI: enemy faction behavior** | `ai.js: updateEnemyAi` | `AI/EnemyAIStrategySystem.cs` + 14 per-operation lanes | Partial | Behavioral parity audit needed; architectural parity not required | |
| **AI: worker gather priority** | `ai.js: getEnemyGatherPriorities` | `AI/AIWorkerGatherSystem.cs` | Complete | None | |
| **AI: build order** | `ai.js: chooseBarracksUnit` + build queue | `AI/AIBuildOrderSystem.cs` | Verify | Verify AI build priority order matches WORKER_BUILD_ORDER from main.js | |
| **AI: assassination** | `ai.js` covert op dispatch | `AI/AIAssassinationExecutionSystem.cs` + `ResolutionSystem.cs` | Complete | None | |
| **AI: espionage** | `ai.js` espionage dispatch | `AI/AIEspionageExecutionSystem.cs` + `ResolutionSystem.cs` | Complete | None | |
| **AI: sabotage** | `ai.js` sabotage dispatch | Paired execution/resolution system | Complete | None | |
| **AI: marriage strategy** | `ai.js: proposeMarriage` decisions | `AI/AIMarriageStrategicProfileSystem.cs` + `AIMarriageInboxAcceptSystem.cs` | Complete | None | |
| **AI: faith commitment** | `ai.js: chooseFaithForAI` | `AI/AIFaithCommitmentSystem.cs` | Complete | None | |
| **AI: holy war hostility** | `ai.js: tickFaithHolyWars` AI side | `AI/AIHolyWarContextSystem.cs` | Complete | None | |
| **AI: divine right** | `ai.js` divine right timing | `AI/AIPlayerDivineRightContextSystem.cs` | Complete | None | |
| **AI: succession crisis exploitation** | `ai.js` succession response | `AI/AISuccessionCrisisConsolidationSystem.cs` + `ContextSystem.cs` | Complete | None | |
| **AI: neutral faction behavior** | `ai.js: updateNeutralAi` | Unverified | Partial | Check if Unity has neutral faction AI; browser has a dedicated neutral driver | |
| **AI: minor house behavior** | `ai.js: updateMinorHouseAi` | `AI/AILesserHousePromotionSystem.cs` | Partial | Verify minor house AI covers all browser minor-house behaviors | |
| **Match: 5-stage progression** | `simulation.js: updateMatchProgressionState` | `Time/MatchProgressionComponent.cs` + `MatchProgressionEvaluationSystem.cs` | Verify | Cross-audit stage gate requirements (T2-C action) | |
| **Match: dual clock** | `simulation.js: tickDualClock` | `Time/DualClockComponent.cs` + `DualClockTickSystem.cs` | Complete | None | |
| **Match: Trueborn Rise arc** | `simulation.js: tickTruebornRiseArc` (4 stages) | `WorldPressure/TruebornRiseArcSystem.cs` + `TruebornDiplomaticEscalationSystem.cs` | Verify | Verify trigger=0.70, release=0.66, 4-stage arc progression | |
| **Match: World pressure + Great Reckoning** | `simulation.js: updateWorldPressureEscalation` | `WorldPressure/WorldPressureEscalationSystem.cs` | Verify | Verify pressure score=4 coalition threshold | |
| **Fortification: tier advance** | `simulation.js: advanceFortificationTier` | `Fortification/AdvanceFortificationTierSystem.cs` | Complete | None | |
| **Fortification: reserve cycling** | `simulation.js: tickFortificationReserves` | `Fortification/FortificationReserveAssignmentComponent.cs` + system | Verify | Verify retreat=0.42, recovery=0.82, triage heal=5.5/s, muster=3.5s | |
| **Fortification: imminent engagement** | `simulation.js: tickImminentEngagementWarnings` | `Fortification/ImminentEngagementCanon.cs` + `WarningSystem.cs` | Verify | Verify warning buffer=4 tiles, watchtower radius=14, min=10s, max=30s | |
| **Fortification: breach assault** | `simulation.js: blood altar surge + breach` | `Siege/` + `Fortification/` systems | Complete | None | |
| **Siege: supply logistics** | `simulation.js: tickSiegeSupportLogistics` | `Siege/SiegeEscalationSystem.cs` + supply camp/convoy systems | Verify | Verify unsupplied attack=0.84, speed=0.88, escort minimum=2 | |
| **Siege: field water** | `simulation.js: tickFieldWaterLogistics` | `Siege/FieldWaterCanon.cs` + related systems | Verify | Verify field water critical multipliers (attack=0.72, speed=0.78) | |
| **Scout raids** | `simulation.js: executeScoutRaid*` | `Raids/ScoutRaidResolutionSystem.cs` | Complete | Verify raid resource loss values | |
| **Covert ops: all player operations** | `simulation.js` + `main.js` covert op flows | `PlayerCovertOps/` folder | Complete | None | |
| **Diplomacy: all player operations** | `main.js` diplomacy UI | `PlayerDiplomacy/` folder | Complete | None | |
| **Economy: trade routes** | NOT IN BROWSER | `Economy/TradeRouteEvaluationSystem.cs` | Unity-Canonical | Document as Unity-canonical advancement (5 gold/route/day) | |
| **Economy: starvation/cap/stability realm conditions** | `simulation.js` realm conditions | `Economy/StarvationResponseSystem.cs`, `CapPressureResponseSystem.cs`, `StabilitySurplusResponseSystem.cs` | Complete | None | |
| **Victory: CommandHallFall** | `data/victory-conditions.json: military_conquest` | `Victory/VictoryConditionEvaluationSystem.cs` | Complete | None | |
| **Victory: Territorial Governance** | `data/victory-conditions.json: territorial_governance` | `Victory/VictoryConditionEvaluationSystem.cs` | Complete | Verify loyalty=90, sustained=120s | |
| **Victory: Divine Right** | `data/victory-conditions.json: divine_right` | `Victory/VictoryConditionEvaluationSystem.cs` | Complete | Verify faith level=5, intensity=80 | |
| **Victory: Economic Dominance** | `data/victory-conditions.json: economic_dominance` (disabled) | Not implemented | Not Started | T3: spec lock before implementing | |
| **Victory: Alliance Victory** | `data/victory-conditions.json: alliance_victory` (disabled) | Not implemented | Not Started | T3: spec lock before implementing | |
| **Save/Load state snapshot** | `simulation.js: exportStateSnapshot / restoreStateSnapshot` | `SaveLoad/BloodlinesSnapshotWriter.cs` + `Restorer.cs` | Complete | None | |
| **Naval: vessel definitions** | `data/units.json` (6 vessel types) | `UnitDefinition` ScriptableObjects | Partial | Definitions exist; no naval systems | T3: spec embark/disembark UX first |
| **Naval: embark/disembark** | Browser implemented sessions 27/28, 96 | Not implemented | Not Started | T3: spec lock before implementing | transport_ship.transportCapacity=6 |
| **Naval: fire ship detonation** | Browser implemented | Not implemented | Not Started | T3: spec lock before implementing | fire_ship.oneUseSacrifice=true |
| **Born of Sacrifice** | NOT IN BROWSER (design doc only) | Not implemented | Not Started | T3: spec lock before implementing | Design in 04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md |
| **Worker slot assignment HUD** | N/A | Not implemented | Not Started | T2-A: implement next session | Component+system exist; UI missing |
| **Stonehelm faction bonuses** | `data/houses.json` (data only) | Not implemented | Not Started | T2-B: small additive wiring | fortificationCostMultiplier=0.80 |
| **Pathfinding (grid/waypoint)** | Simple vector steering | Simple steering only | Not Started | U9 target from migration plan | Required for scale |
| **Multiplayer (NfE)** | Not applicable | Foundation only (this branch) | Not Started | Next P1 slice after this branch merges | Package install needed |
| **Graphics (approved assets)** | N/A | Concept pass only | Not Started | Graphics pipeline exists; no approved art | |
| **Audio (Wwise)** | N/A | ECS scaffolding only | Not Started | Wwise package not installed | |
| **main.js browser shell** | `main.js` | N/A | Browser-Only | Archive only | |
| **renderer.js Canvas2D** | `renderer.js` | N/A | Browser-Only | Archive only | |
| **data-loader.js** | `data-loader.js` | N/A | Browser-Only | Archive only | |
