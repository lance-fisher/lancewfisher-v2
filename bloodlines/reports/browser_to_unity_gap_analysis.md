# Browser-to-Unity Gap Analysis

**Date:** 2026-04-25
**Author:** Claude Code (claude-sonnet-4-6), session on branch `claude/unity-multiplayer-nfe-integration`
**Purpose:** Feature-by-feature comparison between the browser behavioral specification and the active Unity implementation. Identifies what is already covered, partially covered, missing, or requires design decision.
**Safety note:** No files were modified. This is a read-only reporting artifact.

---

## Scope

- Browser reference: `src/game/core/simulation.js` (14,868 LOC), `src/game/core/ai.js` (3,141 LOC), `data/*.json`
- Unity target: `unity/Assets/_Bloodlines/Code/` (~30 subsystem folders on branch `claude/unity-multiplayer-nfe-integration`)
- Prior audit baseline: `reports/2026-04-16_comprehensive_project_audit.md` (57 C# files, ~10,934 LOC on 2026-04-16)
- Current Unity: materially larger (30+ folders, 100+ .cs files per directory listing)

---

## Classification Legend

- **Complete** — Unity implementation matches or exceeds browser. No action needed.
- **Verify** — Unity has the system but specific constants or edge cases need cross-checking.
- **Partial** — Unity has core mechanics but specific sub-systems or paths are missing.
- **Not Started** — Absent from Unity. Exists in browser and/or canon docs only.
- **Unity-Canonical** — Unity model exceeds or replaces browser. Browser was not updated. Canon advancement.
- **Design Lock Needed** — Cannot be implemented without owner spec decision first.

---

## Master Gap Table

| System | Browser Status | Unity Status | Gap Classification | Gap Description | Priority |
|---|---|---|---|---|---|
| **9 Founding Houses** (Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest) | Data + partial faction simulation | `HouseDefinition` SO + `FactionHouseComponent` | Verify | Stonehelm `fortificationCostMultiplier=0.8` + `fortificationBuildSpeedMultiplier=1.2` in data, not yet tied to Construction/Fortification systems | Low |
| **Resources** (Gold, Food, Water, Wood, Stone, Iron, Influence) | Full (6 active; Influence advanced) | `ResourceStockpileComponent` + `ResourceDefinition` | Partial | Influence trickle + Iron/Stone consumption paths thin. Iron smelting fuel consumption (wood at 0.5 ratio) not in Unity trickle system. | Medium |
| **Population pool** | Single integer with cap + reserved | `PopulationComponent` + EarlyGameComponents model | Unity-Canonical | Unity has richer model: civilian/drafted-untrained/reserve/active-duty productivity weighting. Browser has no draft or duty-state model. | High (document canon) |
| **Worker slots** | JSON data only (not in browser sim) | `WorkerSlotBuildingComponent` + `WorkerSlotProductionSystem` | Partial | System exists but no player UI surface to set `AssignedWorkers`. Slot assignment is invisible to player. | High |
| **Worker node-gather** | Full (villager walks to node, carry 10) | `WorkerGatherSystem`, `WorkerGatherComponent`, `WorkerGatherOrderComponent` | Complete | Browser model fully ported. | — |
| **Soldier idle/active-duty contribution (5%)** | NOT in browser | `EarlyGameConstants.ProductivityTrainedActiveDuty=0.05f` + `PopulationProductivitySystem` + `SquadDutySystem` | Unity-Canonical | This rule originated from owner direction 2026-04-25. Browser is a frozen earlier spec. The 5% is canon. | High (document canon) |
| **Military draft slider** | NOT in browser | `MilitaryDraftComponent` + `MilitaryDraftSystem` (5% step, 0-100%) | Unity-Canonical | Draft mechanic is Unity-canonical advancement. Browser has no draft model. | High (document canon) |
| **Faith** (4 covenants, 5 tiers, exposure, doctrine, covenant test, faith units) | Full (sessions 1-96) | Full: `Faith/` folder (12 systems/components) | Complete | Holy war + divine right runtime ticking: verify economic/combat effects are ticked per-second, not just dispatched and resolved. | Low (verify) |
| **Conviction** (4 buckets, 5 bands, score formula, event recording) | Full | Full: `Conviction/` folder | Complete | — | — |
| **Dynasty** (members, roles, succession, legitimacy, renown, aging, marriage) | Full | Full: `Dynasties/` folder (20+ files) | Complete | — | — |
| **Cross-match XP progression** | NOT in browser | `DynastyProgressionCanon` tier thresholds {0,100,350,850,1850}, special unit swaps | Unity-Canonical | Canon advancement. Owner direction 2026-04-19. | High (document canon) |
| **Territory capture + loyalty** | Full (decay 2.5, stabilized 72, governance victory 90) | `FactionLoyaltyComponent`, `ControlPointCaptureSystem`, `TerritoryGovernance/` | Verify | Capture proximity radius 138 tiles: verify matches Unity. Governance victory loyalty threshold 90: confirmed in VictoryConditionEvaluationSystem. | Low (verify) |
| **Governance victory (recognition path)** | Full (65% acceptance sustained 90-120s) | `VictoryConditionEvaluationSystem`: TerritorialGovernanceLoyaltyThreshold=90, VictorySeconds=120 | Verify | Acceptance threshold 65% / 60% with ally: verify Unity has the alliance-loyalty pathway, not just flat 90% threshold. | Low (verify) |
| **Economy (trickle)** | Farm 0.5 food/s, well 0.45 water/s | `ResourceTrickleBuildingSystem` | Verify | Verify constants match browser values exactly. | Low (verify) |
| **Trade routes** | NOT in browser | `TradeRouteComponent` + `TradeRouteEvaluationSystem` (5 gold/route/day) | Unity-Canonical | Trade route system is a Unity-canonical advancement. | Medium (document) |
| **Starvation/Water/Cap realm conditions** | Full | `StarvationResponseSystem`, `CapPressureResponseSystem`, `StabilitySurplusResponseSystem` | Complete | — | — |
| **AI strategic behavior** | Full monolithic in ai.js (~1700 LOC) | 14+ per-operation execution/resolution lane pairs + `EnemyAIStrategySystem` | Partial | Browser AI is one dense function; Unity is factored into lanes. Behavioral parity audit not done. Specific AI "choose-which-unit-to-queue" logic may differ from browser's `chooseBarracksUnit`. | Medium |
| **Combat** (melee, ranged, siege, aura) | Full | Full: `Combat/` folder | Complete | — | — |
| **Fortification + reserves** | Full (field water, imminent engagement, reserve cycling, blood altar) | Full: `Fortification/` + `Siege/` folders | Verify | Blood altar surge (18s / 34s cooldown), reserve triage heal 5.5/s, field water critical multipliers 0.72/0.78: verify against Unity canons. | Low (verify) |
| **Siege supply logistics** | Full (supply convoy, interdiction, unsupplied attack 0.84x) | Full: `Siege/` folder | Verify | Convoy escort requirements and interdiction loss values: verify match browser. | Low (verify) |
| **Scout raids** | Full (node harass, worker harass, logistics interdiction) | `ScoutRaidResolutionSystem`, `ScoutRaidCanon`, `ScoutRaidComponents` | Verify | Raid resource loss values (wood 14, food 10-12, gold 10, stone 12, iron 10): verify match browser. | Low (verify) |
| **Covert ops** (assassination, espionage, sabotage, counter-intel, ransom/rescue) | Full | Full: `PlayerCovertOps/` + AI covert-op lanes | Complete | — | — |
| **Diplomacy** (marriage, missionary, holy war, divine right, pact) | Full | Full: `PlayerDiplomacy/` + AI diplomacy lanes | Complete | — | — |
| **Match phases** (5 stages, dual clock, Trueborn arc, world pressure) | Full | Full: `Time/`, `WorldPressure/` folders | Verify | Stage gate requirements in sim.js (stageTwoRequirements etc): verify Unity `MatchProgressionEvaluationSystem` uses same requirements. | Low (verify) |
| **Victory conditions** | 1 active (military_conquest) | 3 active (CommandHallFall, TerritorialGovernance, DivinRight) | Partial | Economic Dominance + Alliance Victory + Military Conquest (distinct from CommandHallFall) not implemented. Owner decision needed on ship scope. | Medium |
| **Naval** (6 vessel classes, transport, fire ship) | Full (sessions 27/28, 96) | Vessel definitions present; no naval-specific system folder | Partial | Embark/disembark layer (bible §18), fire ship one-use detonation, naval combat, fishing gather all unimplemented. | Medium |
| **Born of Sacrifice** | NOT implemented (zero browser code) | NOT implemented (zero Unity code) | Not Started | Canon exists in design docs. Requires spec lock first. | Medium-High |
| **Worker slot assignment HUD** | N/A (browser doesn't use slots) | Component + system exist; no UI panel | Not Started | Player needs a way to assign workers to `WorkerSlotBuildingComponent` buildings. | High |
| **Pathfinding** (grid/waypoint) | Simple vector steering | Simple steering in `PositionToLocalTransformSystem` + `UnitMovementSystem` | Not Started | No grid pathfinding. Will fail at scale. Unity migration plan phase U9 specifies this. | Medium (U9 target) |
| **Multiplayer (NfE)** | Not applicable | Foundation only (this branch) | Not Started | NfE package, ghost prefab population, 2-client loopback skirmish all pending. | Medium |
| **AI behavioral parity audit** | Baseline in ai.js | Not audited | Not Done | No scenario-driven cross-comparison report exists. | Medium |
| **Graphics (approved assets)** | N/A | Concept pass only (37 SVG sheets) | Not Started | 0 production-candidate assets. Graphics pipeline exists but no approved art. | Medium |
| **Audio (Wwise integration)** | N/A | Audio ECS scaffolding (this branch) | Not Started | `AudioEventBridge`, `AudioEventDispatchSystem` scaffolded but no Wwise package. | Low |

---

## High-Priority Gaps (Action Needed This Session or Next)

### 1. Worker Slot Assignment HUD (High)

**What exists:** `WorkerSlotBuildingComponent.MaxWorkerSlots / AssignedWorkers`, `WorkerSlotProductionSystem` consuming them, `EarlyGameHUDComponent` showing active-duty military counts.

**What is missing:** Any player-facing UI to assign workers. The slot mechanic is invisible. `buildings.json` tooltips reference "Assign workers to gather wood. Output = workerOutputPerSecond × assignedWorkers × effectiveProductivity" but no UI enforces this.

**Recommended Unity path:** Select a `WorkerSlotBuildingComponent`-bearing building → bottom HUD panel shows +/- worker assignment buttons → writes `AssignedWorkers` (clamped to MaxWorkerSlots) → `WorkerSlotProductionSystem` picks up immediately.

**Canonical UX model:** RA2 ore refinery style (per-building slot assignment), not StarCraft global priority.

---

### 2. Unity-Canonical Models Not In Browser (Documentation Needed)

Three Unity models are canonical but not present in the browser behavioral spec. These must be documented clearly so future sessions do not incorrectly treat the browser as the ground truth for these mechanics:

| Mechanic | Unity Source | Canon Note |
|---|---|---|
| ActiveDuty 5% productivity | `EarlyGameConstants.ProductivityTrainedActiveDuty=0.05f` | Owner direction 2026-04-25. Browser is frozen pre-this mechanic. |
| Military draft slider | `MilitaryDraftComponent` + `MilitaryDraftSystem` | Same owner direction. Browser has no draft model. |
| Cross-match dynasty XP | `DynastyProgressionCanon` tiers {0,100,350,850,1850} | Owner direction 2026-04-19. Browser is frozen. |
| Trade routes (5 gold/day) | `TradeRouteEvaluationSystem` | Unity-canonical advancement. |

---

### 3. Balance Constant Verification (Verify)

The browser has 80+ tuned numeric constants. Unity has canon files (EarlyGameConstants, FaithIntensityTiers, FortificationCanon, etc.) that may or may not match them. A focused audit comparing browser constant block (sim.js lines 1-450) to Unity canon values will catch any drift before balance testing begins.

---

### 4. Born of Sacrifice (Design Lock + First Slice)

Zero code anywhere. Canon in design docs. Owner spec needed for:
- Trigger (player-initiated veteran retirement vs. automatic on military upgrade?)
- Institutional memory mechanic (training cost reduction? unit-tier accelerator? renown meta-stat?)
- Faith/conviction modifiers for sacrifice variants
- Population and legitimacy costs

---

### 5. Naval Layer (Medium)

Vessel definitions exist in `units.json`. No naval-specific Unity systems. `transport_ship.transportCapacity=6`. `fire_ship.oneUseSacrifice=true`. Browser implemented full naval sessions 27/28 and 96. Unity must eventually implement: embark order dispatch, passenger buffer, shoreline disembark, fire ship detonation handler, naval combat lane, fishing gather. This is a medium scope slice.

---

## Verify Checklist (Constants Requiring Cross-Audit)

The following constants appear in browser `simulation.js` and must be confirmed against their Unity equivalents before balance testing:

| Constant | Browser Value | Unity Location | Status |
|---|---|---|---|
| `GrowthIntervalSeconds` | 18 | `EarlyGameConstants.GrowthIntervalSeconds` | Match confirmed (18) |
| `FoodPerGrowth` | 1 | `EarlyGameConstants.FoodPerGrowth` | Match confirmed (1) |
| `WaterPerGrowth` | 1 | `EarlyGameConstants.WaterPerGrowth` | Match confirmed (1) |
| `CONTROL_POINT_CAPTURE_DECAY` | 2.5 | `ControlPointCaptureSystem` — verify | Unverified |
| `FAITH_EXPOSURE_THRESHOLD` | 100 | `FaithExposureWalkerSystem` — verify | Unverified |
| `TERRITORY_STABILIZED_LOYALTY` | 72 | `GovernorSpecializationCanon` or `TerritoryGovernance` — verify | Unverified |
| `COMMANDER_BASE_AURA_RADIUS` | 126 | `CommanderAuraSystem` — verify | Unverified |
| `RENOWN_CAP` | 100 | `DynastyRenownComponent` — verify | Unverified |
| `MARRIAGE_PROPOSAL_EXPIRATION` | 90 world-days | `MarriageProposalExpirationSystem` — verify | Unverified |
| `MARRIAGE_GESTATION` | 280 world-days | `MarriageGestationSystem` — verify | Unverified |
| `COVENANT_TEST_INTENSITY_THRESHOLD` | 80 | `CovenantTestQualificationSystem` — verify | Unverified |
| `COVENANT_TEST_DURATION_IN_WORLD_DAYS` | 180 | `CovenantTestQualificationSystem` — verify | Unverified |
| `MINOR_HOUSE_LEVY_MIN_LOYALTY` | 48 | `MinorHouseLevySystem` — verify | Unverified |
| `GREAT_RECKONING_TRIGGER_SHARE` | 0.70 | `WorldPressureEscalationSystem` — verify | Unverified |
| `ASSAULT_STRAIN_THRESHOLD` | 6 | `FortificationCanon` — verify | Unverified |
| `FIELD_WATER_STRAIN_THRESHOLD` | 6 | `FieldWaterComponent` — verify | Unverified |
| `SIEGE_UNSUPPLIED_ATTACK_MULTIPLIER` | 0.84 | Siege canon — verify | Unverified |
| Farm trickle rate | 0.5 food/s | `ResourceTrickleBuildingSystem` — verify | Unverified |
| Well trickle rate | 0.45 water/s | `ResourceTrickleBuildingSystem` — verify | Unverified |

---

## Open Design Questions Requiring Owner Decision

1. **Born of Sacrifice mechanics**: trigger, institutional memory numerics, faith variants, conviction interaction, population/legitimacy costs.
2. **Worker assignment UX**: RA2 per-building slots vs. global priority dial?
3. **Victory condition scope**: ship 3 of 6 or all 6? Economic Dominance + Alliance Victory need spec before implementation.
4. **Naval UX**: embark/disembark interaction model for transport ships?
5. **Worker-slot vs. node-gather coexistence**: which buildings use which model? Does player interact with both simultaneously in one settlement?
6. **Trueborn Rise stage-3 gameplay**: political event only, or military intervention + world-pressure spike?
7. **AI behavioral parity**: do we accept that Unity AI is architecturally different from browser AI and just test behavioral outcomes, or do we want function-level parity?
8. **Stonehelm faction bonuses**: fortification cost 0.8x and build speed 1.2x are in data but no system reads them yet. When should these be wired?
