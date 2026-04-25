# UNITY-CANONICAL ADVANCEMENTS

Date: 2026-04-25
Status: CANONICAL

## Purpose

The Unity port has, by deliberate design direction, advanced past the
browser reference simulation in several mechanical areas. The browser
runtime in `src/game/core/simulation.js` was frozen as a behavioral
specification on 2026-04-17 and does not implement these systems.

This document is the canonical record of every mechanic that exists in
Unity but is intentionally absent from the browser. It exists so that a
session agent reading both surfaces does not infer the browser is the
ground truth in these areas, attempt to "harmonize" Unity downward, or
back-port the browser's silence into Unity's code.

If a mechanic is in this document, the Unity implementation is canonical.
The browser does not have it. The browser will not be updated to add it.

## Reading Order Reminder

Before treating any mechanic as canonical, the canonical reading order in
`CLAUDE.md` and `AGENTS.md` puts the active owner direction files
ahead of the browser. Specifically:

- `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`
- `governance/OWNER_DIRECTION_2026-04-17_FIDELITY_AND_STRATEGY_DEPTH.md`
- `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md`

The 2026-04-16 owner direction declares Unity 6.3 DOTS/ECS as the shipping
engine and the browser as a frozen specification. Mechanics added since
that freeze land in Unity only.

## Mechanic 1: 5% Active-Duty Labor Productivity

**Status:** Canonical Unity. Not in browser.

**What it is.** Population in militia squads on active duty contributes
5% labor productivity. A militia squad assigned any operational
assignment (`Patrol`, `Guard`, `Scout`, `Attack`, `Escort`, `HoldPosition`,
`DefendKeep`, `DefendWoodcutterCamp`, `DefendForagerCamp`) is on active
duty. Soldiers on active duty produce almost nothing economically. They
are fully committed to their operational role.

A faction that mobilizes its entire military experiences near-total
economic output collapse.

**Canon citation.** `04_SYSTEMS/POPULATION_SYSTEM.md` lines 145-146:
"Trained Active Duty: 5% productivity. Militia squads actively assigned
to an operational role... Active-duty soldiers produce almost nothing
economically." `04_SYSTEMS/SYSTEM_INDEX.md` line 90: "Population
productivity states (Civilian 100%/Untrained 75%/Reserve 50%/ActiveDuty 5%)".

**Unity implementation.**

- `unity/Assets/_Bloodlines/Code/Components/EarlyGameComponents.cs`:
  `PopulationProductivityComponent` carries `BaseProductivity` and
  `EffectiveProductivity`. `MilitiaSquadComponent` carries `DutyState`
  (Reserve / ActiveDuty) and `SquadAssignmentType`.
- `unity/Assets/_Bloodlines/Code/Systems/PopulationProductivitySystem.cs`:
  Stage 1 weighted average uses Civilian=1.00, Untrained=0.75,
  Reserve=0.50, ActiveDuty=0.05.
- `unity/Assets/_Bloodlines/Code/Systems/SquadDutySystem.cs`: derives
  duty state from assignment type. Any non-`None` assignment puts the
  squad on active duty.
- `unity/Assets/_Bloodlines/Code/Systems/EarlyGameConstants.cs`: single
  source of all productivity weights.

**What the browser says.** Nothing. The browser's population model has
no concept of active-duty labor cost. Browser units exist on the field
or in the population pool; they do not carry a productivity coefficient
that varies by assignment.

**Why it matters.** This is the core design lever that makes sustained
offensive operations economically expensive in Bloodlines. Without it,
mobilization is free and the player has no incentive to cycle units
between duty states.

## Mechanic 2: Military Draft Slider (0-100%, Step 5)

**Status:** Canonical Unity. Not in browser.

**What it is.** The player adjusts a draft rate slider from 0% to 100%
in 5% increments. Draft rate determines what fraction of the working
adult population is conscripted into the draft pool. Drafted but
untrained adults contribute 75% productivity (vs 100% civilian). Squads
trained from the draft pool transition through Trained -> Reserve ->
ActiveDuty as orders are issued.

**Canon citation.** `04_SYSTEMS/POPULATION_SYSTEM.md` line 178:
"Draft Rate: 0% to 100%, adjustable in increments of 5%."
`04_SYSTEMS/SYSTEM_INDEX.md` line 90 et seq.: "military draft system
(0-100% step-5, DraftPool/TrainedMilitary/UntrainedDrafted)".

**Unity implementation.**

- `unity/Assets/_Bloodlines/Code/Components/EarlyGameComponents.cs`:
  `MilitaryDraftComponent` carries `DraftRatePct` (0-100 step 5),
  `DraftPool`, `TrainedMilitary`, `UntrainedDrafted`, `ReserveMilitary`,
  `ActiveDutyMilitary`, `OverDraftedFlag`.
- `unity/Assets/_Bloodlines/Code/Systems/MilitaryDraftSystem.cs`: clamps
  the draft rate to step-5 boundaries, derives the draft pool from
  population, tallies the four categories.
- `unity/Assets/_Bloodlines/Code/Components/EarlyGameHUDComponent.cs`:
  carries the draft slider read-model values for the HUD.
- Debug surface: `BloodlinesDebugCommandSurface.EarlyGame.cs` exposes
  set/read of the draft rate.

**What the browser says.** Nothing. The browser has no draft slider, no
DraftPool, no Untrained/Reserve/ActiveDuty separation, and no per-faction
adult-population-to-soldier accounting that varies with a player setting.

**Why it matters.** The draft slider is the player's primary economic
lever during peacetime preparation. It directly trades civilian
productivity for military readiness. Combined with active-duty's 5%
productivity, the draft rate determines the long-term productive
capacity of the faction.

## Mechanic 3: Cross-Match Dynasty XP and Tier Bonuses

**Status:** Canonical Unity. Not in browser.

**What it is.** Dynasties that perform well across matches accrue
experience points that unlock tier-based bonuses. Tier bonuses are
sideways customization options (example: swap a dynasty-specific
special unit for another from the same house's progression options) so
that non-#1 placements stay rewarding and the multiplayer power gradient
stays flat.

**Canon citation.** `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md`
line 96: "Cross-match XP for top dynasties. Dynasties that perform well
[...] accrue XP that unlocks tiers; tier bonuses are sideways
customization options [...] so non-#1 placements stay rewarding and
multiplayer power gradients stay flat."

**Unity implementation.**

- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionComponent.cs`:
  per-dynasty XP, tier, and unlock state.
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyXPAwardSystem.cs`:
  awards XP at match end based on placement.
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyProgressionUnlockSystem.cs`:
  evaluates XP thresholds, advances tiers, exposes unlock options.
- `unity/Assets/_Bloodlines/Code/Dynasties/SpecialUnitSwapApplicatorSystem.cs`:
  applies the sideways customization (dynasty-special-unit swap) at
  match start once a player selects an unlocked option.
- `unity/Assets/_Bloodlines/Code/Victory/MatchEndSequenceSystem.cs`:
  triggers XP award at match end as part of the canonical end-of-match
  sequence.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchEndSequenceSmokeValidation.cs`:
  smoke validator for the match-end XP / tier-unlock chain.

**What the browser says.** Nothing. The browser is a single-match
simulation with no persistent dynasty profile, no XP, no cross-match
state, and no tier system.

**Why it matters.** Cross-match progression is the canonical replayability
hook. Without it, multiplayer is single-shot; with it, dynasties have
identity and mastery progression that survives across sessions.

## Mechanic 4: Worker Slot Building Model

**Status:** Canonical Unity. Not in browser.

**What it is.** Resource production buildings (woodcutter camp, forager
camp, small farm, farm, well, mine) carry a fixed number of worker
slots. The player assigns workers to slots; each assigned worker
produces resources at a defined per-second rate scaled by faction
EffectiveProductivity. Output is the product of slot count, per-slot
rate, productivity, and tick delta. Buildings without worker slots
(housing, training yard, command hall, etc.) carry zero slots.

This replaces the browser's implicit "workers gather from nodes near
buildings" model with an explicit assignment surface.

**Canon citation.** `04_SYSTEMS/RESOURCE_SYSTEM.md` describes the
worker-slot model as the canonical Unity production surface (canon
locked 2026-04-25 per the early-game-foundation lane completion).
`04_SYSTEMS/SYSTEM_INDEX.md` references the worker-slot system under
the population/resource systems.

**Unity implementation.**

- `data/buildings.json`: every building carries `maxWorkerSlots`,
  `workerOutputPerSecond`, and `buildTier` fields. New buildings added
  in this canon: `housing`, `forager_camp`, `training_yard`,
  `small_farm`. `lumber_camp` renamed to "Woodcutter Camp" and given
  worker slots.
- `unity/Assets/_Bloodlines/Code/Definitions/BuildingDefinition.cs`:
  `maxWorkerSlots`, `workerOutputPerSecond`, `waterPopulationSupport`,
  `buildTier` fields.
- `unity/Assets/_Bloodlines/Code/Components/EarlyGameComponents.cs`:
  `WorkerSlotBuildingComponent` carries `MaxWorkerSlots`,
  `AssignedWorkers`, `FoodOutputPerWorkerPerSecond`,
  `WoodOutputPerWorkerPerSecond`, `FillRatio`.
- `unity/Assets/_Bloodlines/Code/Components/WorkerSlotHUDComponent.cs`:
  HUD-side mirror.
- `unity/Assets/_Bloodlines/Code/Systems/WorkerSlotProductionSystem.cs`:
  computes output per tick.
- `unity/Assets/_Bloodlines/Code/HUD/WorkerSlotHUDSystem.cs` and
  `Systems/WorkerSlotAssignmentSystem.cs`: HUD read-model and
  assignment-request resolution.
- `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`:
  imports the new JSON fields to the `BuildingDefinition` ScriptableObject.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs`
  and `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`:
  flow worker-slot fields through the spawn pipeline; conditionally
  attach `WorkerSlotBuildingComponent` when `MaxWorkerSlots > 0`.

**What the browser says.** Nothing. Browser buildings do not carry slot
counts; the browser worker model is implicit gather-from-adjacent-node.

**Why it matters.** The worker-slot model makes resource production a
visible, controllable surface. The player sees exactly how many workers
are on each building, and the per-faction `EffectiveProductivity` is
what scales output, not abstract building-level multipliers. This is the
mechanism that makes the population shortage modifiers (food 0.70,
water 0.65, housing 0.85) and the active-duty productivity collapse
visible in real-time output rates.

## Mechanic 5: Trade Routes (with Vessel Interdiction Hooks)

**Status:** Canonical Unity. Not in browser.

**What it is.** Factions can establish persistent trade routes between
their own settlements (and, with appropriate diplomacy, allied
settlements) that flow resources at a defined per-cycle rate. Routes are
modeled as discrete entities and can be interdicted by raids or naval
disruption (the latter ties into the naval-layer lane). Goldgrave's
house-specific bonus references trade-route capacity.

**Canon citation.** `04_SYSTEMS/SYSTEM_INDEX.md` line 42 references
"Goldgrave trade-route military cap" as part of the population system.
`04_SYSTEMS/RESOURCE_SYSTEM.md` discusses cross-settlement resource
flow.

**Unity implementation.**

- `unity/Assets/_Bloodlines/Code/Economy/TradeRouteComponent.cs`:
  per-route component carrying source/destination, resource type,
  flow rate, active state.
- `unity/Assets/_Bloodlines/Code/Economy/TradeRouteEvaluationSystem.cs`:
  per-tick evaluation that moves resources across routes.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.TradeRoutes.cs`:
  debug surface for inspecting and manipulating trade routes.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTradeRouteSmokeValidation.cs`:
  dedicated smoke validator.

**What the browser says.** Nothing. The browser has no trade-route
component, no per-tick cross-settlement resource flow, and no
interdiction surface.

**Why it matters.** Trade routes give the player a reason to defend
non-frontline settlements and make naval and raid systems matter
beyond direct military contact. Without this system, Goldgrave's
house-specific bonus has no effect to attach to.

## How to Add a New Entry

A new mechanic enters this document when ALL of the following are true:

1. The mechanic is implemented (or is being implemented) in Unity.
2. The browser source simulation does not implement it.
3. The mechanic is referenced in canon (`01_CANON/`, `04_SYSTEMS/`,
   `governance/OWNER_DIRECTION_*`) as canonical.

For each entry, record: status, what it is (one paragraph), canon
citation (file + line if available), Unity implementation pointers
(file paths under `unity/Assets/_Bloodlines/Code/`), what the browser
says ("Nothing"), and why it matters.

Append entries in priority order. Do not delete entries; if a mechanic
is later removed from canon, mark it `Status: RETIRED` and keep the
entry for historical context.
