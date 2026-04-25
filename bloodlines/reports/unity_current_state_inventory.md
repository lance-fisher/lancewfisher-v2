# Unity Current State Inventory

**Date:** 2026-04-25
**Author:** Claude Code (claude-sonnet-4-6), session on branch `claude/unity-multiplayer-nfe-integration`
**Purpose:** Detailed inventory of the active Unity DOTS/ECS implementation state.
**Scope:** `D:\ProjectsHome\Bloodlines\unity\Assets\_Bloodlines\Code\`
**Safety note:** No Unity files were modified. This is a read-only reporting artifact.

---

## Executive Summary

The Unity project is mid-early production quality as of 2026-04-25. It is NOT scaffolding. It has materially advanced across ~30 subsystem folders with Burst-compiled ECS systems covering AI, faith, conviction, dynasty, combat, fortification, siege, covert ops, diplomacy, match progression, territory governance, world pressure, and save/load. Several areas are explicitly ahead of the browser reference simulation. The most significant gaps relative to the full shipping game are: worker-slot UI, Born of Sacrifice, naval embark/disembark, full 6-condition victory evaluation, and broad multiplayer.

**Branch:** `claude/unity-multiplayer-nfe-integration`
**Recent commits (last 10):** Multiplayer NfE foundation, audio ECS system, AI strategic layer sub-slices 34-38 (divine right, holy war, succession crisis context, faith commitment, espionage, counter-intelligence).

---

## Subsystem Inventory

### AI/ — Implemented (Extensive, P1 complete)

| File / System | Purpose | Status |
|---|---|---|
| `EnemyAIStrategySystem` | Territory expansion, scout/harass, world-pressure posture, reinforcement dispatch | Green |
| `AITerritoryDispatchSystem` | Territory expansion order issuance | Green |
| `AIWorkerCommandSystem` | Idle worker gather prioritization | Green |
| `AIWorkerGatherSystem` | Worker gather loop for AI faction | Green |
| `AIBuildOrderSystem` + `AIBuildOrderComponent` | Construction queue management | Green |
| 14 paired `AI*ExecutionSystem` + `AI*ResolutionSystem` files | Assassination, captive ransom/rescue, counter-intel, divine right, espionage, holy war, marriage accept/propose, missionary, pact, sabotage, siege orchestration | Green |
| `AIMarriageStrategicProfileSystem` + `AIMarriageInboxAcceptSystem` + `AIMarriageAcceptEffectsSystem` | Full AI marriage pipeline | Green |
| `AIFaithCommitmentSystem` | AI covenant commitment decisions | Green |
| `AIPactProposalExecutionSystem` | Non-aggression pact AI | Green |
| `AISiegeOrchestrationSystem` | Siege timing and coordination | Green |
| `AIStrategicPressureSystem` | World-pressure response posture | Green |
| `AISuccessionCrisisConsolidationSystem` + `AISuccessionCrisisContextSystem` | Succession crisis exploitation | Green |
| `AILesserHousePromotionSystem` | Lesser house loyalty promotion | Green |
| `AICovertOpsSystem` | Covert ops dispatch scheduling | Green |
| `AIPlayerDivineRightContextSystem` | Divine right context evaluation vs player | Green |
| `AIHolyWarContextSystem` | Holy war target evaluation | Green |

**Assessment:** AI layer is one of the most complete subsystems. Behavioral parity audit against browser ai.js is recommended but not blocking.

---

### Economy/ — Implemented (Light, functional)

| File / System | Purpose | Status |
|---|---|---|
| `ResourceTrickleBuildingSystem` + Component | Passive building output (farm food, well water) | Green |
| `StarvationResponseSystem` | Food shortage realm condition | Green |
| `CapPressureResponseSystem` | Population cap pressure realm condition | Green |
| `StabilitySurplusResponseSystem` | Stability surplus reward | Green |
| `FactionLoyaltyComponent` | Loyalty state (Current/Max/Floor, default 70) | Green |
| `TradeRouteComponent` + `TradeRouteEvaluationSystem` | Same-faction adjacent CP trade routes, 5 gold/route/day | Green |

**Gap:** No Influence resource trickle system. No Iron/Stone consumption paths beyond stockpile and building costs. Worker-slot system is in `Systems/` (not `Economy/`). Consider relocating for discoverability.

---

### Population/ — EMPTY FOLDER

No files in this folder. Population mechanics are distributed across:
- `Components/PopulationComponent.cs` (total, cap, reserved)
- `Components/EarlyGameComponents.cs` (draft slider, squad duty state, militia squads, ActiveDutyMilitary)
- `Systems/MilitaryDraftSystem.cs` (draft calculation, active duty counts)
- `Systems/PopulationProductivitySystem.cs` (weighted-average productivity formula)
- `Systems/SquadDutySystem.cs` (squad assignment → DutyState mapping)
- `Systems/WorkerSlotProductionSystem.cs` (slot-based building production)
- `Systems/WaterCapacitySystem.cs` (water capacity from wells and keeps)

**Assessment:** Functional but scattered. Consider whether Population/ should own these files for clarity in future sessions.

---

### Faith/ — Implemented (Deep, browser-parity confirmed)

| File / System | Purpose | Status |
|---|---|---|
| `FaithDoctrineCanon` | 1:1 mapping from faiths.json (4 covenants × 2 paths × 6 effects) | Green |
| `FaithIntensityTiers` | 5-tier model: Apex(80)/Fervent(60)/Devout(40)/Active(20)/Latent(1) | Green |
| `FaithDoctrineSyncSystem` | Syncs doctrine effects to component on intensity change | Green |
| `FaithExposureWalkerSystem` | Sacred site proximity exposure accrual, building amplification | Green (modified this session) |
| `FaithIntensityResolveSystem` | Tier transitions | Green |
| `FaithStructureRegenSystem` | Building intensity regen at max 1.4/s | Green (modified this session) |
| `CovenantTestQualificationSystem` | Test qualification at intensity 80 | Green |
| `CovenantTestResolutionSystem` | Test outcome: fail (−20 intensity, −8 legitimacy, −6 loyalty) / pass (+8 legitimacy, floor 82) | Green |
| `PlayerCovenantTestDispatchSystem` + Component | Player-side covenant test dispatch | Green |
| `VerdantWardenComponent` + `VerdantWardenSupportSystem` | The Wild covenant special mechanic | Green |
| `FaithScoring` | Faction faith score utility | Green |

**Assessment:** Complete relative to browser specification. Holy war and divine right runtime-effect ticking should be verified.

---

### Conviction/ — Implemented (Complete model)

| File / System | Purpose | Status |
|---|---|---|
| `ConvictionScoring` | DeriveScore + ResolveBand + ApplyEvent | Green |
| `ConvictionScoringSystem` | Per-tick conviction band evaluation | Green |
| `ConvictionBandEffects` | Band effect multipliers (pop growth, production) | Green |

**Assessment:** Browser parity confirmed. No gaps.

---

### Dynasties/ — Implemented (Deep + Unity-canonical cross-match XP)

| File / System | Purpose | Status |
|---|---|---|
| `DynastyAgingSystem` | Member age progression | Green |
| `DynastyBootstrap` | Initial dynasty spawn from HouseDefinition | Green |
| `DynastyTemplates` | Named member templates | Green |
| `DynastyPoliticalEventSystem` + Component | Political events from legitimacy crises | Green |
| `DynastyPrestigeDecayModulatorSystem` | Prestige decay pacing | Green |
| `DynastyProgressionCanon` | Cross-match XP tier thresholds {0,100,350,850,1850} | Green (Unity-canonical) |
| `DynastyProgressionComponent` + `DynastyProgressionUnlockSystem` | XP accumulation + tier unlock | Green |
| `SpecialUnitSwapApplicatorSystem` | Tier unlock: dynasty-specific special unit swaps | Green |
| `DynastyRenownComponent` + `DynastyRenownAccumulationSystem` | Renown accrual (cap=100) | Green |
| `RenownAwardSystem` + Component | Award dispatch (combat kill=1, fortification kill=2) | Green |
| `DynastySuccessionSystem` | Succession chain evaluation | Green |
| `DynastyXPAwardSystem` | Match-end XP award per placement | Green |
| `LesserHouseLoyaltyDriftSystem` | Lesser house loyalty drift | Green |
| `MinorHouseLevySystem` | Minor house levy (min loyalty 48) | Green |
| `SuccessionCrisisEvaluationSystem` + `RecoverySystem` + Component | Succession crisis evaluation and recovery | Green |
| `MarriageComponents` | Marriage state (proposal, gestation, dissolution) | Green |
| `MarriageProposalExpirationSystem` | 90-day expiration | Green |
| `MarriageGestationSystem` | 280-day gestation | Green |
| `MarriageDeathDissolutionSystem` | Dissolution on member death | Green |

**Assessment:** Complete and extends browser with cross-match dynasty XP progression system.

---

### Combat/ — Implemented (Extensive)

Confirmed subsystems (folder listing): auto-acquire, attack-target, attack-orders, attack-resolution, projectile movement/impact, melee + ranged + projectile-siege paths, group movement, unit separation, combat stances, recent-impact recovery, commander aura, death resolution, hostility, patrol movement, rally points, pending-commander-capture.

**Assessment:** Functionally complete for the current game scope. No gaps blocking delivery.

---

### Victory/ — Implemented (3 of 6 conditions)

| File / System | Purpose | Status |
|---|---|---|
| `VictoryConditionEvaluationSystem` | Evaluates 3 active conditions per tick | Green |
| `MatchEndSequenceSystem` | Match end sequencing on victory detect | Green |
| `MatchEndStateComponent` | Match end state carrier | Green |
| `VictoryComponents` | Victory condition component types | Green |

**Active conditions:** CommandHallFall, TerritorialGovernance (loyalty 90 sustained 120s), DivinRight (faith level 5 + intensity 80).
**Not implemented:** Economic Dominance, Alliance Victory, Military Conquest (separate from CommandHallFall). These are declared `prototypeEnabled: false` in the browser data.

---

### Definitions/ — Implemented (ScriptableObject layer)

Full binding: HouseDefinition, UnitDefinition, BuildingDefinition, BloodlinePathDefinition, BloodlineRoleDefinition, ConvictionStateDefinition, FaithDefinition, MapDefinition, RealmConditionDefinition, ResourceDefinition, SettlementClassDefinition, VictoryConditionDefinition, BuildingVisualDefinition, UnitVisualDefinition, plus `BloodlinesDefinitions` registry singleton.

**Assessment:** Complete. The registry pattern enables compile-time-safe data access from all systems.

---

### Components/ — Implemented (Extensive)

All major state-carrying components confirmed present:
- Audio: `AudioEventDispatchComponent`, `AudioEventElement`
- Bloodline: `BloodlineRoleComponent`
- Building: `BuildingTypeComponent`, fortification/siege/construction components
- Combat: `HealthComponent`, `CommanderComponent`, `ImminentEngagementComponent`
- Conviction: `ConvictionComponent`
- Dynasty: `DynastyMemberComponent`
- Early game: `EarlyGameComponents` (PopulationProductivityComponent, MilitaryDraftComponent, WorkerSlotBuildingComponent, MilitiaSquadComponent)
- Faction: `FactionComponent`, `FactionHouseComponent`, `FactionLoyaltyComponent`, `FactionTintComponent`, `FactionVisualComponent`
- Faith: `FaithComponent`
- Field: `FieldWaterComponent`
- Movement: `MoveCommandComponent`, `MovementStatsComponent`, `PositionComponent`
- Multiplayer: `SnapshotVersionComponent`
- Population: `PopulationComponent`
- Production: `ProductionComponents`
- Realm: `RealmConditionComponent`
- Resource: `ResourceNodeComponent`
- Selection: `SelectionComponent`
- Settlement: `SettlementComponent`
- Siege: `SiegeComponent`, `SiegeSupplyTrainComponent`, `SiegeSupportComponent`
- Unit: `UnitTypeComponent`
- Worker: `WorkerGatherComponent`, `WorkerGatherOrderComponent`
- New (this branch): `FoundingRetinueComponent`, `AudioEventDispatchComponent`, `AudioEventElement`

**EarlyGameConstants (confirmed values):**
```
DraftStepSize=5, DraftMin=0, DraftMax=100, SquadSize=5
ProductivityCivilian=1.00
ProductivityDraftedUntrained=0.75
ProductivityTrainedReserve=0.50
ProductivityTrainedActiveDuty=0.05  ← canonical 5% active duty labor contribution
FoodShortageModifier=0.70
WaterShortageModifier=0.65
HousingShortageModifier=0.85
ShortageShortDurationSeconds=30
ShortageSustainedDurationSeconds=120
WellPopulationSupport=50
KeepBaseWaterSupport=15
GrowthIntervalSeconds=18
FoodPerGrowth=1
WaterPerGrowth=1
```

---

### TerritoryGovernance/ — Implemented (Governor lane)

`GovernorSeatAssignmentComponent`, `GovernorSpecializationCanon`, `GovernorSpecializationComponent`, `GovernorSpecializationSystem`. Recognition state lives in `WorldPressure/TerritorialGovernanceRecognitionComponent`.

---

### WorldPressure/ — Implemented (Complete)

World pressure escalation, governance coalition pressure, territorial governance recognition, territorial pressure evaluation, Trueborn Rise arc (4 stages), Trueborn diplomatic escalation, Trueborn recognition resolution. Full player-side Trueborn recognition request component.

---

### Siege/ — Implemented (Complete)

Breach assault, field water (canon/strain/support scan), siege escalation (system + canon), siege supply camp + interdiction, siege support refresh + canon. `SiegeComponentInitializationSystem`.

---

### Fortification/ — Implemented (Complete)

`AdvanceFortificationTierSystem`, `BreachSealingSystem`, `DestroyedCounterRecoverySystem`, fortification building contribution, canon, reserve assignment, settlement link, structure resolution, imminent engagement (posture, warning, canon). Player imminent engagement posture request.

---

### Raids/ — Implemented (Light, functional)

`ScoutRaidCanon`, `ScoutRaidComponents`, `ScoutRaidResolutionSystem`. Building raid plus supply-wagon interdiction covered.

---

### Pathing/ — Thin (Steering only)

`PositionToLocalTransformSystem`, `UnitMovementSystem`. No grid/waypoint pathfinding. Simple steering-vector approach. Will cap unit count at scale.

---

### PlayerDiplomacy/ — Implemented (Extensive)

Full player surface for: marriage propose/accept, missionary dispatch, holy war declaration, divine right declaration, captive ransom/rescue dispatch, non-aggression pact propose/break, succession preference.

---

### PlayerCovertOps/ — Implemented

Assassination, espionage, sabotage resolution; counter-intelligence; sabotage effect/status; intelligence reports.

---

### Time/ — Implemented (Complete)

`DualClockComponent`, `DualClockTickSystem`, `DualClockDeclarationSystem`, `DualClockDeclarationBridgeSystem`, `DeclareInWorldTimeRequest`, `MatchProgressionComponent`, `MatchProgressionEvaluationSystem`. Five-stage match progression evaluation is here.

---

### Skirmish/ — Stub + Bootstrap

`SkirmishGameModeComponent` (stub). `SkirmishBootstrapSystem` (active, seeds initial game state from ScriptableObject definitions) is in `Systems/`. `SkirmishGameModeManagerSystem` also in `Systems/`.

---

### SaveLoad/ — Implemented

`BloodlinesSnapshotPayload` (schema v1, 11 list fields), `BloodlinesSnapshotWriter`, `BloodlinesSnapshotRestorer`.

---

### Multiplayer/ + Networking/ — Foundation only (this branch)

`NetworkGameModeComponent` singleton (server/client/local + max players), `GhostPrefabArchetypeElement` buffer, `NetworkBootstrapSystem`, `GhostCollectionSetupSystem`. NfE package not installed yet. `BloodlinesBootstrap` (new this branch) and ghost prefab authoring meta files present.

---

### UI/ + HUD/ — Shell-level implemented

`SkirmishStatusHUDComponent`, `RealmConditionHUDSystem`, `EarlyGameHUDComponent` (exposes ActiveDutyMilitary, ReserveMilitary, ActiveDutySquads), construction/production progress panels, `DynastyRenownLeaderboardHUDSystem` (modified this branch).

**Gap:** No worker-slot assignment panel. No full match-stage transition UI. No faith/conviction readout in HUD.

---

### Systems/ (root group)

`SkirmishBootstrapSystem`, `MilitaryDraftSystem`, `PopulationProductivitySystem`, `SquadDutySystem`, `WorkerGatherSystem`, `WorkerSlotProductionSystem`, `EarlyGameConstants`, `WaterCapacitySystem`. Also new this branch: `AudioEventDispatchSystem`, `BuildTierGatingSystem`, `BloodlinesBootstrap`.

---

### Other folders

- `Aspects/`: ECS Aspects (read-only view helpers) — present
- `Authoring/` + `Baking/`: Authoring components and Bakers for Unity Editor → DOTS conversion — present
- `Camera/`, `Input/`, `Debug/`, `Rendering/`: Editor + presentation systems — present
- `Construction/`: Building construction pipeline — present
- `Audio/`: `AudioEventBridge` (new this branch, modified) — light implementation

---

## Summary: What Unity Has vs. What Remains

| Category | Unity Status |
|---|---|
| ECS architecture, DOTS/Burst | Fully established |
| Faith (all 4 covenants, 5 tiers, covenant tests) | Complete |
| Conviction (4 buckets, 5 bands) | Complete |
| Dynasty (aging, succession, marriage, legitimacy, renown) | Complete |
| Cross-match dynasty XP progression | Complete (Unity-canonical) |
| AI strategic layer (14+ operation lanes) | Complete |
| Combat (melee, ranged, siege, commander aura) | Complete |
| Fortification + siege | Complete |
| World pressure + Trueborn rise | Complete |
| Covert ops + diplomacy | Complete |
| Match phases (5 stages, dual clock) | Complete |
| Territory + loyalty | Complete |
| Economy (trickle, worker-slot, worker-gather) | Complete |
| Save/load | Complete |
| ActiveDuty 5% labor contribution model | Complete (Unity-canonical) |
| Worker slot production system | Complete |
| Victory conditions (3 of 6) | Partial |
| Naval layer (embark/disembark/combat) | Partial |
| Born of Sacrifice | Not started |
| Worker slot HUD panel | Not started |
| Full pathfinding (grid/waypoint) | Not started |
| AI behavioral parity audit | Not done |
| Multiplayer (NfE) | Foundation only |
| Graphics (approved assets) | Concept pass only |
| Audio (Wwise) | Scaffolded only |
