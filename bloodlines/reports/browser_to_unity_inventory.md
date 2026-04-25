# Browser-to-Unity Inventory

**Date:** 2026-04-25
**Author:** Claude Code (claude-sonnet-4-6), session on branch `claude/unity-multiplayer-nfe-integration`
**Purpose:** Detailed inventory of all browser-era files and the useful systems they contain, classified for Unity migration relevance.
**Scope:** `D:\ProjectsHome\Bloodlines\src\game\` and associated JSON data files.
**Safety note:** No browser files were modified. This is a read-only reporting artifact.

---

## Executive Summary

The browser implementation contains ~21,867 LOC across 5 JavaScript source files and 11 JSON data files. The simulation engine is a single-object functional state machine with 47 ordered sub-ticks per frame. The browser prototype was the primary development surface through Session 96 (2026-04-16). As of 2026-04-25, the Unity project (branch `claude/unity-multiplayer-nfe-integration`) is materially ahead of the browser in several areas (active-duty labor contribution, worker slot model, cross-match dynasty XP progression, multiplayer foundation). The browser remains the best reference for balance constant values, match-stage gating logic, and AI strategic decision logic.

---

## File Inventory Table

| File Path | Lines | Purpose | Migration Classification | Risk Level |
|---|---|---|---|---|
| `src/game/core/simulation.js` | 14,868 | Authoritative behavioral specification. All game simulation in one functional file. | Data-portable + Conceptually portable | Low (reference only) |
| `src/game/core/ai.js` | 3,141 | Enemy, neutral, and minor-house AI drivers. Strategic decision logic. | Conceptually portable | Low |
| `src/game/main.js` | 2,818 | Browser shell, game loop, UI state machine, HUD event handlers | Documentation-only (browser-specific shell) | None (do not port) |
| `src/game/core/renderer.js` | 956 | Canvas2D presentation layer, drawing only | Obsolete/browser-only | None |
| `src/game/core/data-loader.js` | 53 | Async JSON fetch + index builder | Obsolete/browser-only | None |

---

## Detailed System Inventory: simulation.js

**Major system clusters and their migration status:**

### Population and Growth
- `tickPopulationGrowth`, `recalculatePopulationCaps`, `getUsedPopulation`, `getAvailablePopulation`
- Model: single integer `total` with `cap`, `baseCap`, `reserved`, `growthProgress` per faction
- Growth formula: every `GROWTH_INTERVAL_SECONDS=18` real seconds, food -1 and water -1, population +1, if food/water available and total < cap
- Ironmark blood-penalty modifier extends interval up to 1.8x at blood load 14+
- **Unity status:** Superseded by richer model in `PopulationComponent`, `PopulationProductivitySystem`, `MilitaryDraftSystem`. Unity model is strictly more complete.
- **Action:** Preserve growth formula as canon reference. Confirm Unity matches same 18s interval and 1:1 food/water cost per population.

### Resources and Economy
- `tickPassiveResources`: building.resourceTrickle per second (farm food 0.5/s, well water 0.45/s)
- `updateUnits` (worker gather): `gatherRate * dt` to deposit node until `carryCapacity=10` threshold, then walk to dropoff
- Iron mine: consumes wood at `smeltingFuelRatio=0.5` wood per iron gathered
- `spendResources`, `canAfford`: faction resource pool checks
- **Unity status:** All three paths implemented: `ResourceTrickleBuildingSystem` (passive), `WorkerGatherSystem` (node gather), `WorkerSlotProductionSystem` (slot-based building output). Unity is ahead of browser.
- **Action:** Verify farm 0.5 food/s and well 0.45 water/s constants match Unity `ResourceTrickleBuildingComponent` values.

### Buildings and Construction
- `attemptPlaceBuilding`: costs, build progress at `buildRate * dt`
- `populationCapBonus`: summed from all buildings
- 23 building types defined in `data/buildings.json`
- **Unity status:** `BuildingDefinition`, `ConstructionSystem`, `BuildTierGatingSystem`, `ResourceTrickleBuildingSystem` all present
- **Action:** None needed. Verify building data JSON is in sync with ScriptableObjects.

### Combat
- `findNearestEnemyInRange`, `applyDamage` with structural/anti-unit multipliers
- Damage formula: building damage = `base * structuralDamageMultiplier * attacker.structuralDamageMultiplier`; unit damage = `base * antiUnitDamageMultiplier`
- Wall multiplier 0.2; ram structural 3.5 (net 0.7x wall); anti-unit ram 0.4
- Commander aura: `getCommanderAuraProfile` with radius 126 base
- **Unity status:** Full combat system in `Combat/` folder (melee, ranged, projectile, commander aura, stances)
- **Action:** Verify multiplier constants match. Wall 0.2, ram 3.5 structural are balance-critical.

### Worker Logic
- `updateWorkerUnit`: walk to resource node, gather up to carryCapacity=10, walk to nearest dropoff
- Workers are ECS-style entities with `gatherRate`, `carryCapacity` from `unitDef`
- **Unity status:** `WorkerGatherSystem`, `WorkerGatherComponent`, `WorkerGatherOrderComponent`. Unity ALSO has `WorkerSlotProductionSystem` for building-slot model (not in browser).
- **Action:** Coexistence of both models is intentional. No action needed.

### Faith System
- `updateFaithExposure`: accrues per second from sacred site proximity, scaled by building amplifier (Wayshrine 1.8x, CovenantHall 2.4x, GrandSanctuary 3.0x, ApexCovenant 3.6x)
- `chooseFaithCommitment`: triggered when exposure >= FAITH_EXPOSURE_THRESHOLD=100
- `tickFaithHolyWars`, `tickFaithDivineRightDeclarations`: runtime effects during holy war / divine right active states
- `updateFaithStructureIntensity`: building regen at up to `COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND=1.4`
- Five intensity tiers: Apex(80)/Fervent(60)/Devout(40)/Active(20)/Latent(1)
- **Unity status:** Full implementation in `Faith/` folder. Browser-parity confirmed.
- **Action:** Verify holy war and divine right runtime ticking (economic/combat effects) are covered in Unity, not just declaration/resolution.

### Conviction System
- `getConvictionBand`, `recordConvictionEvent`
- 4 buckets: Stewardship, Oathkeeping, Ruthlessness, Desecration
- Score = (Stewardship + Oathkeeping) - (Ruthlessness + Desecration)
- Bands: ApexMoral(>=75), Moral(>=25), Neutral(>=-24), Cruel(>=-74), ApexCruel
- Events: dwelling/farm/well built +1 stewardship; fortification +2 stewardship; blood levy +1 ruthlessness; sustained attritional levy +2 ruthlessness; ruling hall destroyed +5 ruthlessness
- **Unity status:** Full in `Conviction/` folder. Browser-parity confirmed.
- **Action:** None.

### Dynasty and Bloodline
- `createDynastyState`: 9-role succession chain, RENOWN_CAP=100
- `findAvailableSuccessor`: head → heir → commander → governor → diplomat → ideological → spymaster → merchant
- `adjustLegitimacy`: event-driven legitimacy delta
- `appendFallenLedger`: killed/captured/enslaved/ransomed member records
- Renown awards: combat kill=1, fortification kill=2
- Marriage: 90-day proposal expiration, 280-day gestation
- **Unity status:** Full in `Dynasties/` folder. Cross-match XP progression (Tier 1-4) is Unity-canonical advancement beyond browser.
- **Action:** Verify legitimacy loss constants are canon-locked in Unity systems.

### Territory and Loyalty
- `updateControlPoints`: proximity-based capture, decay=2.5
- `applyControlPointLoyaltyDelta`: loyalty shifts from capture events
- Stabilized loyalty threshold: 72 (governor court loyalty)
- Governance victory: loyalty 90 sustained 120 seconds, 65% acceptance, 60% with ally
- **Unity status:** `FactionLoyaltyComponent`, `ControlPointCaptureSystem`, `ControlPointResourceTrickleSystem`, `TerritoryGovernance/`, `WorldPressure/`. 90-threshold loyalty victory confirmed in `VictoryConditionEvaluationSystem`.
- **Action:** Verify capture decay=2.5 matches Unity. Confirm stabilized loyalty floor=72 matches `TERRITORIAL_GOVERNANCE_COURT_LOYALTY_THRESHOLD`.

### Match Phases and Dual Clock
- 5 stages: founding, expansion_identity, encounter_establishment, war_turning_of_tides, final_convergence
- 3 phases per stage: emergence, commitment, resolution
- Dual clock: real-time seconds + in-world days
- Stage requirement gating (stageTwoRequirements, etc.)
- `GREAT_RECKONING_TRIGGER_SHARE=0.7` (Trueborn Rise trigger)
- **Unity status:** Full in `Time/` folder: `DualClockComponent`, `DualClockTickSystem`, `MatchProgressionComponent`, `MatchProgressionEvaluationSystem`.
- **Action:** Cross-audit stage-gate requirement logic between browser and Unity.

### Fortification and Siege
- `advanceFortificationTier`, `tickFortificationReserves`, `tickSiegeSupportLogistics`, `tickFieldWaterLogistics`
- `tickImminentEngagementWarnings`: 10-30s warning buffer (keep: 14s base, settlement: 11s base)
- Blood altar surge: 18s duration, 34s cooldown
- Field water: local radius 132, settlement radius 156, strain threshold 6, critical threshold 12
- Reserve cycling: retreat health 0.42, recovery health 0.82, triage heal 5.5/s
- **Unity status:** Full in `Fortification/` and `Siege/` folders.
- **Action:** Verify numeric constants. Imminent engagement 10-30s range, field water strain multipliers 0.88/0.78 are balance-critical.

### Covert Operations and Diplomacy
- Espionage, counter-intel, assassination, sabotage, ransom/rescue, missionary ops
- Non-aggression pacts, governance pressure
- **Unity status:** Full in `PlayerCovertOps/`, `PlayerDiplomacy/`, `AI/` (14+ paired execution/resolution systems)
- **Action:** None.

### AI Behavior (ai.js)
- Three drivers: `updateEnemyAi`, `updateNeutralAi`, `updateMinorHouseAi`
- Timer accumulators: attack, build, territory, raid, operation-specific
- Decision categories: worker gather priority, barracks + faith building construction, scout dispatch, marriage proposals, espionage, covenant test targeting, governance pressure response
- Key helpers: `chooseBarracksUnit`, `chooseFaithUnitForBuilding`, `pickTerritoryTarget`, `pickScoutRaidTarget`, `getEnemyGatherPriorities`
- **Unity status:** `EnemyAIStrategySystem` + 14+ per-operation lanes. Factored very differently from browser's monolithic function.
- **Action:** Behavioral parity audit recommended (see Open Design Questions in gap analysis).

### World Pressure and Trueborn Rise
- `updateWorldPressureEscalation`: great reckoning trigger at territory share 0.7
- `tickTruebornRiseArc`: 4-stage arc (0/1/2/3) with escalating diplomatic pressure
- `getGreatReckoningProfile`: coalition formation profiles at pressure score 4
- **Unity status:** Full in `WorldPressure/`: `WorldPressureEscalationSystem`, `TruebornRiseArcSystem`, `TruebornDiplomaticEscalationSystem`, etc.
- **Action:** Verify great-reckoning territory share threshold 0.70 matches Unity.

### Scout Raids
- `executeScoutRaid`, `executeScoutResourceHarass`, `executeScoutWorkerHarass`, `executeScoutLogisticsInterdiction`
- Raid loss per hit: wood 14, food 10-12, gold 10, stone 12, iron 10, influence 2-4
- Target range 24 tiles, retreat distance 84 tiles
- **Unity status:** `Raids/ScoutRaidResolutionSystem`, `ScoutRaidCanon`, `ScoutRaidComponents`.
- **Action:** Verify raid loss values match browser constants.

---

## Data File Inventory

| File | Rows | Browser Uses | Unity Mirror | Status |
|---|---|---|---|---|
| `data/houses.json` | 9 | Yes | `HouseDefinition` ScriptableObject | Matched. 2 playable, 6 settled-visual-only, 1 neutral (Trueborn). |
| `data/units.json` | 38 | Yes | `UnitDefinition` ScriptableObject | Matched. All 5 stages, faith/apex units, naval (6), siege (5+2). |
| `data/buildings.json` | 23 | Partial | `BuildingDefinition` ScriptableObject | `maxWorkerSlots` / `workerOutputPerSecond` fields present in JSON but ignored by browser, consumed by Unity `WorkerSlotProductionSystem`. |
| `data/resources.json` | 7 | Yes | `ResourceDefinition` ScriptableObject | Matched. Gold, Food, Water, Wood, Stone, Iron, Influence. |
| `data/faiths.json` | 4 | Yes | `FaithDefinition` + `FaithDoctrineCanon` | Matched. All 4 covenants, Light/Dark paths, 6 doctrine effect multipliers each. |
| `data/conviction-states.json` | 5 | Yes | `ConvictionStateDefinition` + `ConvictionScoring` | Matched. 5 bands, bucket math. |
| `data/victory-conditions.json` | 6 | 1 active | `VictoryConditionDefinition` | 1 enabled in browser (military_conquest). Unity implements 3 (CommandHallFall, TerritorialGovernance, DivinRight). 3 unimplemented. |
| `data/bloodline-roles.json` | 9 | Yes | `BloodlineRoleDefinition` | Matched. |
| `data/bloodline-paths.json` | 7 | Yes | `BloodlinePathDefinition` | Matched. |
| `data/realm-conditions.json` | varies | Yes | `RealmConditionDefinition` | Matched via `StarvationResponseSystem`, `CapPressureResponseSystem`. |
| `data/settlement-classes.json` | varies | Yes | `SettlementClassDefinition` | Matched. |

---

## Summary Classification

| Classification | Browser Files/Systems | Action |
|---|---|---|
| Already covered in Unity (no action) | Faith, Conviction, Dynasty, Fortification, Siege, Covert Ops, Diplomacy, Match Phases, Dual Clock, Data files | Validate constant parity only |
| Unity ahead of browser | ActiveDuty 5% labor model, WorkerSlot model, Cross-match XP progression | Document as Unity-canonical; freeze browser |
| Partial coverage (gap exists) | Victory conditions (3 of 6), Naval layer, AI behavioral parity | Medium priority |
| Not implemented anywhere | Born of Sacrifice | Spec lock first, then first slice |
| Browser-only (do not port) | main.js shell, renderer.js, data-loader.js | Archive reference only |
| Balance constants to audit | 80+ numeric constants in simulation.js | Cross-check audit against Unity canon values |
