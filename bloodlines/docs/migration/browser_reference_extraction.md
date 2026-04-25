# Browser Prototype Reference Extraction

**Date:** 2026-04-25
**Author:** Claude Code (claude-sonnet-4-6)
**Purpose:** Unity-facing extraction of useful Bloodlines design and simulation knowledge recovered from the browser prototype. Written for Unity DOTS/ECS implementation guidance, not as a description of the browser game.
**Source files:** `src/game/core/simulation.js`, `src/game/core/ai.js`, `src/game/main.js`, `data/*.json`

---

> This document is the Unity team's reference. It contains design intelligence, balance constants, and formulas extracted from the browser prototype that either confirm existing Unity canon or supplement it. Where the browser and Unity differ and Unity is ahead, Unity is explicitly the canonical source. Where the browser has information not yet confirmed in Unity, this document is the reference.

---

## Houses and Faction Identity

The nine founding houses are defined in `data/houses.json`. All 9 are canonically settled. Key data for Unity implementation:

| House | Color Identity | Unique Mechanic | Status |
|---|---|---|---|
| Trueborn | Gold/Silver | Neutral early game; Trueborn Rise arc (4 stages) at territorial dominance 0.70 | Data only; TruebornRiseArcSystem exists |
| Highborne | Purple/Gold | Faith-leading; covenants preferred | Data only |
| Ironmark | Crimson/Iron | Blood penalty modifier on population growth (extends interval 1.0x-1.8x at load 8-14+) | Data; blood growth penalty exists in sim.js |
| Goldgrave | Gold/Amber | Trade; economic strength | Data only |
| Stonehelm | Grey/Stone | `fortificationCostMultiplier=0.80`, `fortificationBuildSpeedMultiplier=1.20` | Data; NO system reads these yet |
| Westland | Green/Tan | Agricultural; food production leader | Data only |
| Hartvale | Brown/Red | Conflict with CB002 open decision | Data only |
| Whitehall | White/Blue | Diplomatic; alliance-threshold preferred | Data only |
| Oldcrest | Dark Blue/Silver | Ancient legacy; prestige-oriented | Data only |

**Unity recommendation:** Wire `HouseDefinition.fortificationCostMultiplier` and `fortificationBuildSpeedMultiplier` into `AdvanceFortificationTierSystem` and `ConstructionSystem` as faction-modifying lookups via `FactionHouseComponent`.

---

## Resources and Economy

### Seven Resource Types
- Gold, Food, Water, Wood, Stone, Iron, Influence (advanced resource)
- All in `data/resources.json`, mirrored to `ResourceDefinition` ScriptableObjects

### Passive Trickle Rates (from buildings.json / simulation.js)
```
Small Farm:     food  +0.50 /real-second
Well:           water +0.45 /real-second
Lumber Camp:    wood  (gather model, not trickle)
Mine Works:     stone (gather model, not trickle)
Iron Mine:      iron  (gather model + smelting)
```

### Worker-Node Gather Model (browser-canonical)
- Worker walks to nearest `state.world.resourceNodes` deposit
- Gather rate: `unitDef.gatherRate * dt`
- Progress threshold: 1.25 units gathered = one carry load
- Carry capacity: `unitDef.carryCapacity = 10` (for Villager)
- Deposit at nearest building with `isDropoff=true`
- Iron mine additionally consumes `smeltingFuelRatio=0.5` wood per iron unit gathered; insufficient wood returns ore to node

### Worker-Slot Building Model (Unity-canonical, NOT in browser sim)
- Buildings with `maxWorkerSlots` in buildings.json: lumber_camp, forager_camp, small_farm, mine_works
- Output formula: `BaseRatePerWorker × AssignedWorkers × EffectiveProductivity × dt`
- Accumulated per faction, applied to `ResourceStockpileComponent` each tick
- This is Unity's production backbone. It coexists with the node-gather model for visible worker units.

### Scout Raid Resource Losses (balance reference)
```
Resource     Loss per successful raid hit
wood         14
food         10-12 (variable)
gold         10
stone        12
iron         10
influence    2-4 (variable)
```

### Smelting Fuel Ratio
```
Iron mine: consumes 0.5 wood per iron unit gathered
If insufficient wood: ore returned to node (no iron output)
```

---

## Population and Labor

### Browser Population Model (reference)
- Single integer `total` per faction
- Fields: `total`, `cap`, `baseCap`, `reserved`, `growthProgress`
- `baseCap` = sum of `populationCapBonus` from all owned buildings
- `reserved` = population queued in production (units/buildings)

### Growth Formula (browser-canonical, confirmed in Unity)
```
growthThreshold = (GROWTH_INTERVAL_SECONDS=18 * bloodGrowthPenalty) /
                   max(0.5, doctrine.populationGrowthMultiplier
                          * conviction.populationGrowthMultiplier
                          * (politicalEffects.populationGrowthMultiplier ?? 1))

Trigger condition: food >= total+1 AND water >= total+1 AND total < cap
On trigger: total +=1, food -=1, water -=1

Ironmark blood penalty:
  bloodLoad <= 8: penalty = 1.0 (no effect)
  bloodLoad 8-14: penalty = 1.0 to 1.8 (linear interpolation)
  bloodLoad >= 14: penalty = 1.8 (max, interval 1.8x longer)
```

**Unity confirmation:** `EarlyGameConstants.GrowthIntervalSeconds=18`, `FoodPerGrowth=1`, `WaterPerGrowth=1`. Unity model is richer — population productivity is weighted by duty state.

### Unity Population Model (Unity-canonical, NOT in browser)

The following population productivity model is Unity-canonical per owner direction 2026-04-25:

```
Productivity weights:
  Civilian (not drafted):              1.00 (100%)
  Drafted/Untrained (reserve, not yet a squad): 0.75 (75%)
  Trained Reserve (squad, off-duty):   0.50 (50%)
  Trained Active Duty (squad, deployed): 0.05 (5%)

Effective base productivity:
  BaseProductivity = weighted average across all population by category

Settlement condition modifiers (applied after base):
  Food shortage:    × 0.70
  Water shortage:   × 0.65
  Housing shortage: × 0.85
```

**The 5% active-duty labor contribution is the canonical starting balance value.** It represents the reduced but non-zero economic output of soldiers who are deployed (patrolling, garrisoning, scouting) but not in direct combat. This creates a meaningful strategic choice: keep soldiers active (5% economic output, immediate readiness) or stand them down to reserve (50% economic output, mobilization delay).

### Military Draft Mechanic (Unity-canonical, NOT in browser)
```
Draft slider: 0-100% in DraftStepSize=5% increments
SquadSize = 5 per squad
At draft percentage X: X% of eligible population is conscripted
Untrained drafted (not yet a squad): productivity 75%
Once organized into squads: reserve (50%) or active duty (5%)
```

### Water Capacity Formula (Unity)
```
Wells:     WellPopulationSupport=50 population each
Keep:      KeepBaseWaterSupport=15 baseline
```

---

## Workers and Assignment

### Assignment Slot Buildings (from data/buildings.json)
| Building | maxWorkerSlots | workerOutputPerSecond |
|---|---|---|
| lumber_camp | (varies) | wood output per assigned worker |
| forager_camp | (varies) | food output per assigned worker |
| small_farm | (varies) | food output per assigned worker |
| mine_works | (varies) | stone/iron output per assigned worker |

### Canonical Worker Assignment UX
- RA2 ore-refinery style: per-building, explicit slot assignment
- Not StarCraft global priority dial
- Player selects building → HUD panel shows current assigned / max slots → +/- buttons
- `AssignedWorkers` field in `WorkerSlotBuildingComponent` is the write target
- `FillRatio = AssignedWorkers / MaxWorkerSlots` drives the production output scale

---

## Soldiers, Idle Contribution, and Readiness

### The Core Strategic Tension
Soldiers not in active combat are not idle in the canonical Bloodlines model. They have a duty state that determines both their economic contribution and their military readiness.

### Canonical Duty States (Unity ECS model)
| DutyState | Economic Productivity | Readiness | Typical Assignment |
|---|---|---|---|
| `Civilian` (not in military) | 100% | N/A | Default population |
| `DraftedUntrained` | 75% | Low (not yet a squad) | Recently conscripted, awaiting organization |
| `Reserve` | 50% | Medium (mobilizes with delay) | Trained squad standing down |
| `ActiveDuty` | 5% | High (immediately deployable) | Patrolling, garrisoning, scouting, escorting |

### The 5% Active Duty Rate
```
ActiveDutyLaborRate = 0.05  (canonical, from EarlyGameConstants.ProductivityTrainedActiveDuty)
```
This is the confirmed starting balance value per owner direction 2026-04-25. It means:
- An active-duty squad of 5 soldiers contributes the equivalent of 0.25 civilian workers to the economy
- A 20-squad army (100 active-duty soldiers) costs ~5 civilian-equivalent economic output vs. doing nothing
- Keeping the army in reserve instead (50% productivity) recaptures 45 economic output points per 100 soldiers at the cost of mobilization delay

### Military Readiness Concept
From the browser's fortification reserve cycling model:
```
Reserve retreat health threshold:   0.42 (unit retreats to reserve when health drops to 42%)
Reserve recovery health threshold:  0.82 (unit re-engages when health recovers to 82%)
Reserve triage heal rate:           5.5 health/second at triage radius 2.4 tiles
Reserve muster interval:            3.5 seconds
```

**Unity recommendation:** These values from the browser's fortification reserve system are analogous to the squad duty-state cycling. The 0.42/0.82 thresholds are battle-readiness expressions, not economic productivity values. Keep them in `FortificationCanon` where they currently live.

### Patrol, Scout, Garrison, Standby (Conceptual)
The browser's AI uses `buildSupplyPatrolAssignments` and dispatches scout units via `executeScoutRaid`. The Unity duty model should extend to support:
- `PatrolAssignment`: unit is actively covering territory (ActiveDuty state)
- `GarrisonAssignment`: unit is defending a structure (ActiveDuty state)
- `ScoutAssignment`: unit is ranging for intelligence (ActiveDuty state)
- `StandbyAssignment`: unit is at settlement, available but not tasked (Reserve state)
- `LaborSupportAssignment`: unit is contributing productive support (ActiveDuty state, 5%)

---

## Faith System

### Four Covenants
1. **Old Light / Covenant of the First Flame** — Light path: civilization, stability, order. Dark path: zealotry, purge.
2. **Blood Dominion / The Red Covenant** — Light path: warrior-ascension sacrifice. Dark path: blood terror and domination.
3. **The Order / Covenant of the Sacred Mandate** — Light path: discipline, law, codification. Dark path: inquisition.
4. **The Wild / Covenant of the Thorned Elder Roots** — Light path: nature, growth, verdant power. Dark path: entropy, rot.

### Faith Mechanics Reference

```
Commitment trigger: exposure >= FAITH_EXPOSURE_THRESHOLD=100
Starting commitment intensity: 20

Building exposure amplifiers:
  Wayshrine (L1):      1.8x
  Covenant Hall (L2):  2.4x
  Grand Sanctuary (L3):3.0x
  Apex Covenant (L4):  3.6x

Intensity tiers:
  Latent:   >= 1
  Active:   >= 20
  Devout:   >= 40
  Fervent:  >= 60
  Apex:     >= 80

Covenant Test:
  Trigger: intensity >= 80 (Apex tier)
  Duration: 180 in-world days
  Retry cooldown: 120 in-world days after failure
  
  Failure penalties:
    intensity -20
    legitimacy -8
    loyalty shock -6 (all controlled CPs)
    
  Success rewards:
    legitimacy +8
    intensity floor raised to 82 (prevents re-trigger immediately)
    
  Building regen rate (during test): max 1.4 intensity/second

Faith unit unlock stages:
  Stage 3 (CovenantHall): Tier-2 faith units
  Stage 4 (GrandSanctuary): Tier-3/apex faith units
  Stage 5 (ApexCovenant): Apex unit (one per match, per canon)
```

### Doctrine Effect Multipliers (faiths.json)
Each covenant has Light and Dark doctrine paths, each with 6 effect multipliers:
- `auraAttackMultiplier`: combat aura attack bonus
- `auraRadiusBonus`: commander aura radius extension
- `auraSightBonus`: commander sight range extension
- `stabilizationMultiplier`: territory stabilization rate
- `captureMultiplier`: control point capture rate
- `populationGrowthMultiplier`: population growth tick modifier

Example (Old Light, Light doctrine):
```
auraAttackMultiplier: 1.05
auraRadiusBonus: 18
auraSightBonus: 20
stabilizationMultiplier: 1.34
captureMultiplier: 1.0
populationGrowthMultiplier: 1.08
```

---

## Conviction System

### Distinct from Faith
Faith is spiritual covenant alignment. Conviction is behavioral morality shaped by player actions. Do not merge these into one generic morality system.

### Formula
```
score = (Stewardship + Oathkeeping) - (Ruthlessness + Desecration)

Bands:
  Apex Moral:  score >= 75
  Moral:       score >= 25
  Neutral:     score >= -24
  Cruel:       score >= -74
  Apex Cruel:  score < -75
```

### Event Triggers (browser reference)
```
Action                           Bucket       Delta
Complete dwelling/farm/well      Stewardship  +1
Raise fortification tier         Stewardship  +2
Pay Ironmark blood levy          Ruthlessness +1
Sustained attritional blood levy Ruthlessness +2
Destroy enemy ruling hall        Ruthlessness +5
Execute captive                  Ruthlessness +3 (estimated from canon)
Enslave captive                  Ruthlessness +2 (estimated from canon)
Ransom captive honorably         Oathkeeping  +1 (estimated from canon)
Release captive                  Oathkeeping  +2 (estimated from canon)
Breach pact                      Ruthlessness +4 (estimated from canon)
```

### Band Effects (from conviction-states.json)
| Band | Population Growth Multiplier | Notes |
|---|---|---|
| Apex Moral | 1.08 | Maximum civic compliance |
| Moral | 1.03 | Positive civic trust |
| Neutral | 1.00 | Baseline |
| Cruel | 0.97 | Fear and flight risk |
| Apex Cruel | 0.92 | Severe compliance breakdown |

---

## Dynasty and Bloodline Systems

### Succession Chain (9 roles, in priority order)
```
1. head_of_bloodline
2. heir_designate
3. commander
4. governor
5. diplomat
6. ideological_leader
7. spymaster
8. merchant
(9th: sorcerer — non-succession path role)
```

### Key Balance Constants
```
RENOWN_CAP = 100
Renown awards:
  Combat kill:       +1
  Fortification kill: +2

Legitimacy loss events:
  Commander killed:      -9
  Commander captured:    -12
  Head of bloodline killed: -18
  Governor territory lost: -5
  Interregnum (no heir):   -14
  
Legitimacy recovery events:
  Successful succession:  +7
  Captive ransomed:       +4
  Captive rescued:        +6

Marriage timeline:
  Proposal expiration:    90 in-world days
  Gestation period:      280 in-world days

Captive events:
  Ransom base duration:   16 real seconds
  Ransom renown modifier: 0.55 (higher renown = longer duration)
  Ransom base gold cost:  70 gold + 18 influence
  Rescue base duration:   20 real seconds
  Rescue renown modifier: 0.70
  Rescue base gold cost:  42 gold + 26 influence
```

### Cross-Match Dynasty XP Progression (Unity-canonical)
```
Tier XP Thresholds: {0, 100, 350, 850, 1850}
Max Tier: 4

Placement XP awards:
  1st place: 50 XP
  2nd place: 35 XP
  3rd place: 20 XP
  Floor:     10 XP (all participants)

Tier unlocks: dynasty-specific special unit swaps (sideways customization, not power escalation)
  Tier 1: first swap option unlocked
  Tier 2: second swap option unlocked
  ...etc.
```

---

## Territory and Loyalty

```
Control Point capture:
  Capture decay rate:  2.5% per second while contested
  Proximity radius:    138 tiles for capture contribution

Loyalty thresholds (governance):
  Court loyalty (stabilized):      72
  Governance victory floor:        80 (sustained for recognition)
  Governance victory threshold:    90 (sustained for 120s with 65% acceptance)
  Alliance acceptance:             60% (with allied faction)
  Non-allied acceptance:           65%
  Lesser house loyalty break:      25
  
  Sustain time:       90 real seconds (recognition phase)
  Victory time:      120 real seconds

Governance coalition pressure constants:
  Loyalty pressure/cycle:  -1.5 per hostile coalition faction
  Legitimacy pressure:      0.8 per cycle
  Acceptance drag:          0.04 per hostile
```

---

## AI Behavior Patterns

### AI Decision Timer Accumulators (browser reference)
The browser AI uses multiple timer accumulators per faction. When timers expire, the AI executes a decision. Unity factors these into per-operation execution/resolution lanes but the underlying logic is the same.

```
attackTimer:      fires attack dispatch logic
buildTimer:       fires building queue evaluation
territoryTimer:   fires territory capture priority evaluation
raidTimer:        fires scout raid dispatch
operationTimers:  per-operation (assassination, missionary, holy war, etc.)
```

### AI Worker Logic
```
Priority ordering for idle workers:
1. Assign to resource nodes with lowest fill percentage (prioritize critical resources)
2. Return to base if carrying a load
3. Build queued buildings if available
4. Gather from nearest accessible node
```

### AI Build Priority (WORKER_BUILD_ORDER, main.js line 96)
The browser's AI canonical build order (21 types):
```
1. dwelling, 2. small_farm, 3. well, 4. barracks, 5. lumber_camp, 6. mine_works,
7. market, 8. stable, 9. archery_range, 10. smithy, 11. siege_workshop,
12. wayshrine, 13. watchtower, 14. wall_segment, 15. gate,
16. covenant_hall, 17. grand_sanctuary, 18. apex_covenant,
19. harbor, 20. shipyard, 21. fortified_tower
```

### Governance Victory AI Response
```
When governance victory is within triggering range:
  Collapse all AI operation timers to 2-5 second intervals
  Prioritize military + territory operations
  Attempt coalition formation if player is close to recognition threshold
```

### AI Succession Crisis Exploitation
When a target faction is in succession crisis:
- AI increases attack pressure
- AI attempts assassination of vulnerable heirs
- AI proposes marriage to capture legitimacy benefit

---

## Match Structure and Pacing

### Five-Stage Match Structure
```
Stage 1: founding
  — Expansion and settlement establishment
  — All 9 houses present (if non-Trueborn)

Stage 2: expansion_identity
  — Faith commitment available
  — Economy differentiation begins

Stage 3: encounter_establishment
  — First military encounters
  — Faith units unlock at CovenantHall

Stage 4: war_turning_of_tides
  — Full military doctrine active
  — Governance pressure begins accumulating
  — World pressure possible

Stage 5: final_convergence
  — Trueborn Rise if any faction >0.70 territory share
  — Coalition mechanics fully active
  — Apex faith units unlock at ApexCovenant
  — All victory conditions checkable
```

### Stage Gate Requirements (browser reference — verify against Unity)
Stage transitions in the browser require:
- stageTwoRequirements: minimum building count, minimum territory count
- stageThreeRequirements: military encounter, faith exposure active
- stageFourRequirements: at least 2 factions with active military campaigns
- stageFiveRequirements: sustained territorial dominance, or game-time threshold

### Dual Clock
```
Real-time: seconds since match start
In-world: day counter (independent of speed)
In-world day rate: configurable via speed controls
Phase declarations: `DualClockDeclarationSystem` fires on stage transitions
```

### Trueborn Rise Arc
```
Stage 0: dormant (no player at >0.70 share)
Stage 1: awakening (trigger at 0.70 share — first diplomatic contact)
Stage 2: rising pressure (coalition formation offers to other factions)
Stage 3: great reckoning (coalition active, Trueborn military intervention)

Trigger threshold: 0.70 territory share
Release threshold: 0.66 territory share (arc reverses)
Great Reckoning pressure score: 4 (triggers coalition)
```

---

## Victory Conditions Reference

From `data/victory-conditions.json` (6 total):

| ID | Name | Browser Active | Unity Implemented | Notes |
|---|---|---|---|---|
| military_conquest | Destroy all enemy Command Halls | Prototype only | Yes (CommandHallFall) | Primary path |
| territorial_governance | Recognition by 65% of factions at loyalty 90 | Yes | Yes (TerritorialGovernance) | Diplomacy path |
| divine_right | Faith level 5 + intensity 80 | Yes | Yes (DivinRight) | Faith path |
| economic_dominance | Resource stockpile thresholds | Disabled | No | Needs spec |
| alliance_victory | Allied faction reaches victory | Disabled | No | Needs spec |
| military_conquest_extended | (Variant) | — | — | May be same as CommandHallFall |

---

## Born of Sacrifice (Design Canon Only — Not Yet In Code)

**Zero implementation in browser. Zero implementation in Unity. Canon only.**

Design canon source: `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md`, `01_CANON/BLOODLINES_MASTER_MEMORY.md` §41, §66, §1963.

### Confirmed Canon Direction
The mechanic was redesigned from an early "5:1 population sacrifice for elite unit" concept to a **population-constrained army lifecycle** model:

1. Veterans retire (player-triggered or automatic on military upgrade)
2. New wave is trained from the population pool (cost: time + population)
3. The returning generation inherits institutional memory (mechanic TBD)
4. This creates generational army continuity with measurable progression

### Open Questions Requiring Owner Spec
- What is the institutional memory mechanic numerically?
- What is the trigger? Player-initiated veteran retirement vs. auto on upgrade?
- Faith-specific sacrifice variants?
- Cooldowns and limits?
- Relationship to conviction (does sacrificing veterans shift conviction?)
- Can bloodline members be involved?

### Recommended Unity Implementation Path (when spec arrives)
```
BornOfSacrificeRequestComponent (player intent)
BornOfSacrificeResolutionSystem (veteran retirement + population cost)
InstitutionalMemoryComponent (on newly-trained units)
BornOfSacrificeEffectsCanon (numeric modifiers)
```

---

## Balance Constants and Formulas (Full Reference)

All values extracted from `simulation.js` constant block (lines 1-450). These are browser-prototype values. Treat as starting reference. Some may already be overridden by Unity canon values.

```
Population / Growth:
  GROWTH_INTERVAL_SECONDS = 18
  Food per growth = 1; Water per growth = 1

Combat geometry:
  CONTROL_POINT_CAPTURE_DECAY = 2.5 (%/s while contested)
  CAPTURE_PROXIMITY_RADIUS = 138 tiles
  COMMANDER_BASE_AURA_RADIUS = 126 tiles
  SCOUT_RAID_TARGET_RANGE = 24 tiles
  SCOUT_RAID_RETREAT_DISTANCE = 84 tiles
  SCOUT_RAID_LOYALTY_RADIUS = 240 tiles
  SCOUT_NODE_HARASS_RADIUS = 120 tiles
  SCOUT_NODE_RETREAT_SECONDS = 10s

Captive economics:
  CAPTIVE_INFLUENCE_TRICKLE = 0.022 /s
  CAPTIVE_RENOWN_WEIGHT = 0.0014

Legitimacy events:
  LEGITIMACY_LOSS_COMMANDER_KILL = 9
  LEGITIMACY_LOSS_COMMANDER_CAPTURE = 12
  LEGITIMACY_LOSS_HEAD_FALL = 18
  LEGITIMACY_LOSS_GOVERNOR_LOSS = 5
  LEGITIMACY_LOSS_INTERREGNUM = 14
  LEGITIMACY_RECOVERY_ON_SUCCESSION = 7
  LEGITIMACY_RECOVERY_ON_RANSOM = 4
  LEGITIMACY_RECOVERY_ON_RESCUE = 6

Ransom / Rescue:
  RANSOM_BASE_DURATION_SECONDS = 16
  RANSOM_DURATION_RENOWN_MULTIPLIER = 0.55
  RESCUE_BASE_DURATION_SECONDS = 20
  RESCUE_DURATION_RENOWN_MULTIPLIER = 0.70
  RANSOM_BASE_GOLD_COST = 70; INFLUENCE_COST = 18
  RESCUE_BASE_GOLD_COST = 42; INFLUENCE_COST = 26

Succession crisis:
  SUCCESSION_CRISIS_ADULT_AGE = 18
  SUCCESSION_CRISIS_MATURE_AGE = 21
  SUCCESSION_CRISIS_ESCALATION_IN_WORLD_DAYS = 120
  SUCCESSION_CRISIS_CLAIM_GAP_THRESHOLD = 4

Covenant test:
  COVENANT_TEST_INTENSITY_THRESHOLD = 80
  COVENANT_TEST_DURATION_IN_WORLD_DAYS = 180
  COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS = 120
  COVENANT_TEST_FAILURE_INTENSITY_LOSS = 20
  COVENANT_TEST_FAILURE_LEGITIMACY_LOSS = 8
  COVENANT_TEST_FAILURE_LOYALTY_SHOCK = 6
  COVENANT_TEST_SUCCESS_INTENSITY_FLOOR = 82
  COVENANT_TEST_SUCCESS_LEGITIMACY_BONUS = 8
  COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND = 1.4

Territory governance:
  TERRITORIAL_GOVERNANCE_MIN_STAGE = 5
  TERRITORIAL_GOVERNANCE_MIN_TERRITORY_SHARE = 0.35
  TERRITORIAL_GOVERNANCE_LOYALTY_THRESHOLD = 80
  TERRITORIAL_GOVERNANCE_VICTORY_LOYALTY_THRESHOLD = 90
  TERRITORIAL_GOVERNANCE_BREAK_LOYALTY_THRESHOLD = 65
  TERRITORIAL_GOVERNANCE_COURT_LOYALTY_THRESHOLD = 72
  TERRITORIAL_GOVERNANCE_LESSER_HOUSE_LOYALTY_THRESHOLD = 25
  TERRITORIAL_GOVERNANCE_SUSTAIN_SECONDS = 90
  TERRITORIAL_GOVERNANCE_VICTORY_SECONDS = 120
  TERRITORIAL_GOVERNANCE_ACCEPTANCE_THRESHOLD_PCT = 65
  TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT = 60

Coalition / World pressure:
  GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE = -1.5
  GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE = 0.8
  GOVERNANCE_ALLIANCE_ACCEPTANCE_DRAG_PER_HOSTILE = 0.04
  GREAT_RECKONING_TRIGGER_SHARE = 0.70
  GREAT_RECKONING_RELEASE_SHARE = 0.66
  GREAT_RECKONING_PRESSURE_SCORE = 4

Governor bonuses:
  GOVERNOR_STABILIZATION_BONUS = 1.30
  GOVERNOR_TRICKLE_BONUS = 1.22

Minor house levy:
  MINOR_HOUSE_LEVY_MIN_LOYALTY = 48
  MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND = 0.60

Renown:
  RENOWN_CAP = 100
  RENOWN_AWARD_COMBAT_KILL = 1
  RENOWN_AWARD_FORTIFICATION_KILL = 2

Marriage:
  MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 90
  MARRIAGE_GESTATION_IN_WORLD_DAYS = 280

Fortification:
  FORTIFICATION_ECOSYSTEM_RADIUS_TILES = 9
  FORTIFICATION_AURA_RADIUS_TILES = 10
  FORTIFICATION_THREAT_RADIUS_TILES = 8
  FORTIFICATION_RESERVE_RADIUS_TILES = 12
  FORTIFICATION_TRIAGE_RADIUS_TILES = 2.4
  FORTIFICATION_KEEP_PRESENCE_RADIUS_TILES = 6

Assault/cohesion:
  ASSAULT_STRAIN_THRESHOLD = 6
  ASSAULT_STRAIN_DECAY_PER_SECOND = 0.12
  ASSAULT_COHESION_PENALTY_DURATION = 20 seconds
  ASSAULT_COHESION_PENALTY_MULTIPLIER = 0.85

Imminent engagement:
  IMMINENT_ENGAGEMENT_WARNING_BUFFER_TILES = 4
  IMMINENT_ENGAGEMENT_WATCHTOWER_RADIUS_TILES = 14
  IMMINENT_ENGAGEMENT_MIN_SECONDS = 10
  IMMINENT_ENGAGEMENT_MAX_SECONDS = 30
  IMMINENT_ENGAGEMENT_KEEP_BASE_SECONDS = 14
  IMMINENT_ENGAGEMENT_SETTLEMENT_BASE_SECONDS = 11
  IMMINENT_ENGAGEMENT_REINFORCEMENT_SURGE_SECONDS = 18

Reserve cycling:
  RESERVE_RETREAT_HEALTH_RATIO = 0.42
  RESERVE_RECOVERY_HEALTH_RATIO = 0.82
  RESERVE_TRIAGE_HEAL_PER_SECOND = 5.5
  RESERVE_MUSTER_INTERVAL_SECONDS = 3.5

Blood altar (Blood Dominion):
  BLOOD_ALTAR_SURGE_DURATION_SECONDS = 18
  BLOOD_ALTAR_SURGE_COOLDOWN_SECONDS = 34

Realm conditions:
  REALM_CYCLE_DEFAULT_SECONDS = 90

Siege logistics:
  SIEGE_SUPPORT_REFRESH_SECONDS = 1.25
  SIEGE_UNSUPPLIED_ATTACK_MULTIPLIER = 0.84
  SIEGE_UNSUPPLIED_SPEED_MULTIPLIER = 0.88
  CONVOY_RECOVERY_DURATION_SECONDS = 12
  CONVOY_ESCORT_SCREEN_RADIUS = 86
  CONVOY_ESCORT_MIN_ESCORTS = 2

Field water:
  FIELD_WATER_LOCAL_SUPPORT_RADIUS = 132 tiles
  FIELD_WATER_SETTLEMENT_SUPPORT_RADIUS = 156 tiles
  FIELD_WATER_SUPPORT_DURATION_SECONDS = 14
  FIELD_WATER_TRANSFER_INTERVAL_SECONDS = 4
  FIELD_WATER_TRANSFER_COST = 0.20
  FIELD_WATER_STRAIN_PER_SECOND = 0.85
  FIELD_WATER_RECOVERY_PER_SECOND = 1.25
  FIELD_WATER_STRAIN_THRESHOLD = 6
  FIELD_WATER_CRITICAL_THRESHOLD = 12
  FIELD_WATER_STRAIN_ATTACK_MULTIPLIER = 0.88
  FIELD_WATER_STRAIN_SPEED_MULTIPLIER = 0.90
  FIELD_WATER_CRITICAL_ATTACK_MULTIPLIER = 0.72
  FIELD_WATER_CRITICAL_SPEED_MULTIPLIER = 0.78
  FIELD_WATER_ATTRITION_THRESHOLD_SECONDS = 4
  FIELD_WATER_DESERTION_THRESHOLD_SECONDS = 10
  FIELD_WATER_ATTRITION_DAMAGE_PER_SECOND = 6
  FIELD_WATER_DESERTION_HEALTH_RATIO = 0.45

Verdant Warden (The Wild):
  VERDANT_WARDEN_ZONE_RADIUS = 184 tiles
  VERDANT_WARDEN_MAX_SUPPORT_STACK = 3
```

---

## Browser-Only Items (Do Not Migrate)

| File / Feature | Reason |
|---|---|
| `main.js` browser shell and game loop | Browser-specific `requestAnimationFrame`, DOM, canvas context |
| `renderer.js` Canvas2D rendering | No relevance to Unity renderer |
| `data-loader.js` JSON fetch | Unity uses ScriptableObject registry |
| `state.ui.*` fields in simulation | Browser UI state machine — no Unity equivalent |
| HTML/CSS/layout logic | Not applicable |
| `localStorage` usage | Not applicable |
| `dispatchEvent` browser events | Not applicable |

---

## Open Questions Requiring Owner Decision

1. **Born of Sacrifice**: trigger, institutional memory numerics, faith variants, conviction cost, population/legitimacy cost
2. **Worker assignment UX**: per-building slot vs. global priority?
3. **Victory conditions 4-6**: ship 3 or all 6? When?
4. **Naval UX**: embark/disembark interaction model?
5. **Worker-slot vs. node-gather coexistence**: which buildings use each model?
6. **AI behavioral parity**: architectural parity vs. behavioral outcome parity?
7. **Stonehelm faction bonuses**: when to wire to Construction/Fortification systems?
8. **Trueborn Rise stage-3 actual gameplay**: political event only, or military intervention?
