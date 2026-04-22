# CODEX MULTI-DAY DIRECTIVE: BLOODLINES UNITY DEEP SYSTEMS PUSH
# Date: 2026-04-23
# Duration: 2-3 days of continuous autonomous sessions
# Owner: Lance Fisher
# Supersedes: CODEX_MULTI_DAY_DIRECTIVE_2026-04-22.md
# Contract revision at issue: 89
# Target: advance contract to revision 100+

---

## IDENTITY AND ROLE

You are Codex, working on Bloodlines, a grand dynastic civilizational RTS built in Unity 6.3 LTS with DOTS/ECS. You are one of two AI agents (the other is Claude Code) working in governed concurrent lanes. Your work must follow the Concurrent Session Contract and the lane-ownership model at all times. Do NOT stop after completing one sub-slice. Proceed immediately to the next item in the priority stack below. Only stop when you genuinely run out of session capacity.

---

## CANONICAL ROOT

`D:\ProjectsHome\Bloodlines` (junction to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`)

The Unity project lives at `unity/` inside that root.

---

## READ ORDER (EVERY SESSION, NO EXCEPTIONS)

Before doing any work, read these files in full and in this order:

1. `AGENTS.md`
2. `CLAUDE.md`
3. `NEXT_SESSION_HANDOFF.md` -- read the first 100 lines at minimum
4. `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- read the ENTIRE file
5. `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`
6. `continuity/PROJECT_STATE.json`

Then read the specific browser reference functions for the sub-slice you are about to implement.

---

## CURRENT BUILD STATE (contract revision 89, 2026-04-22)

All of the following are on master and fully validated:

**Core simulation:** Economy, construction, production, starvation/loyalty/cap-pressure, stability surplus, loyalty density, state snapshot/restore, dual-clock, five-stage match progression, world pressure escalation, victory conditions (CommandHallFall, TerritorialGovernance, DivineRight).

**Combat:** Melee, ranged projectile, explicit attack orders, attack-move, target acquisition with sight loss, group movement, soft unit separation, hold-position and retreat stances.

**Fortification and siege (sub-slices 1-13):** Full tier/reserve/breach/sealing/recovery chain, tier-scaled sealing costs, worker-locality gate, repair narrative, wall-segment destruction, imminent engagement component, siege supply interdiction.

**Dynasty systems:** Core, conviction scoring + bands + wiring, faith commitment + intensity, marriage parity (proposal/expiration/gestation/death-dissolution), lesser-house loyalty parity (drift + hostile breakaway spawning), minor-house levy parity (claim gates, retinue cap, unit profile selection).

**AI strategic layer (sub-slices 1-25):** Full AI command dispatch, strategic pressure, governance pressure, worker gather, siege orchestration, covert ops, build timer, marriage proposal/inbox/profile/effects/terms, lesser house promotion, pact proposal/break, narrative bridge, dynasty operations, captive state, missionary execution, holy war execution, divine right execution, captive rescue execution, captive ransom execution, missionary resolution.

**Scout raids and logistics interdiction:** RaidComponent, RaidMovementSystem, RaidResourceTheftSystem, abort-on-intercept.

**Player diplomacy lane:** Marriage proposal + acceptance + death dissolution, holy war declaration, divine right declaration, missionary dispatch, pact proposal + break, captive rescue dispatch, captive ransom dispatch.

**Player covert ops lane:** Foundation (espionage dispatch + capacity gate + cost deduction), assassination + sabotage dispatch, counter-intelligence (level, decay, detection, intelligence report buffer).

**HUD lane:** RealmConditionHUDComponent + System, MatchProgressionHUDComponent + System, FortificationHUDComponent + System, VictoryConditionReadoutComponent + System, VictoryLeaderboardHUDComponent + System, DynastyRenownHUDComponent + System, DynastyRenownLeaderboardHUDComponent, PlayerCommandDeckHUDComponent + System.

**Conviction band wiring:** LoyaltyProtectionMultiplier in RealmConditionCycleSystem, PopulationGrowthMultiplier in StarvationResponseSystem, CaptureMultiplier in AttackResolutionSystem.

**Dynasty renown/prestige:** DynastyRenownComponent + DynastyRenownAccumulationSystem on master.

**Your active lanes (claim new ones as needed for new systems):**
- `player-diplomacy` (owns `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/**`)
- `player-covert-ops` (owns `unity/Assets/_Bloodlines/Code/PlayerCovertOps/**`)
- `hud-legibility` (owns `unity/Assets/_Bloodlines/Code/HUD/**`)

**Claude Code's active lane (do NOT touch):** `ai-strategic-layer` owns `unity/Assets/_Bloodlines/Code/AI/**` exclusively.

---

## WORK PRIORITY ORDER

Do not stop after a sub-slice. Complete it, validate it, push it, merge it, then immediately begin the next. Chain as many as your session allows. The priorities below represent approximately 2-3 days of continuous work.

---

### PRIORITY 1: Succession Crisis System

**Lane:** `dynasty-succession-crisis` (new lane -- claim it)

**Branch:** `codex/unity-dynasty-succession-crisis`

**Context:** `DynastySuccessionSystem.cs` exists and handles member succession events (next heir promotion), but there is no system that models the political instability cascade that follows a head-of-bloodline death. The browser spec tracks crisis severity (minor/moderate/major/catastrophic), applies resource multipliers, and drives recovery costs. Without this, dynasty deaths are mechanical non-events; with it, they become genuine turning points that reshape the map.

**Browser references -- read these before implementing:**
- `src/game/core/simulation.js` search `SUCCESSION_CRISIS_SEVERITY_PROFILES` (~lines 55-112): the four severity tiers and their effect multipliers (resource trickle factor, loyalty shock, legitimacy drain rate, recovery cost)
- `src/game/core/simulation.js` search `getSuccessionCrisisTerms` (~line 4651): how severity is determined from member renown, active wars, held territories, and conviction band
- `src/game/core/simulation.js` search `consolidateSuccessionCrisis` (~line 4695): how crisis state resolves over time (recovery phase, legitimacy stabilization)
- `src/game/core/simulation.js` search `tickDynastyPoliticalEvents` (~line 3950): the per-tick crisis tick that applies multipliers and advances recovery

**Implement:**
- `SuccessionCrisisComponent.cs`: per-faction -- `CrisisSeverity` (byte: 0=None, 1=Minor, 2=Moderate, 3=Major, 4=Catastrophic), `CrisisStartedAtInWorldDays` (float), `RecoveryProgressPct` (float 0-1), `ResourceTrickleFactor` (float), `LoyaltyShockApplied` (bool), `LegitimacyDrainRatePerDay` (float)
- `SuccessionCrisisEvaluationSystem.cs` (ISystem, runs after DynastySuccessionSystem): when a head-of-bloodline succession event fires, evaluate `getSuccessionCrisisTerms` logic against current faction state and write `SuccessionCrisisComponent`; apply one-time loyalty shock to all control points in faction; begin legitimacy drain
- `SuccessionCrisisRecoverySystem.cs` (ISystem): per DualClock day, advance `RecoveryProgressPct`; remove component when recovery reaches 1.0; scale recovery speed by conviction band (moral conviction speeds recovery)
- Wire `ResourceTrickleFactor` into `EconomySystem` reads on `FactionComponent` resource trickle (multiplicative)
- Add `TryDebugGetSuccessionCrisis(factionId)` to `BloodlinesDebugCommandSurface.Dynasty.cs`

**Smoke validator:** `BloodlinesSuccessionCrisisSmokeValidation.cs` / `Invoke-BloodlinesUnitySuccessionCrisisSmokeValidation.ps1`
- Phase 1: head-of-bloodline death triggers crisis component with non-None severity
- Phase 2: loyalty shock applied to all faction control points on crisis start
- Phase 3: recovery progresses over ticks and removes component at completion
- Phase 4: catastrophic severity applies higher legitimacy drain than minor severity

---

### PRIORITY 2: Political Event System

**Lane:** `dynasty-political-events` (new lane -- claim it, adjacent to succession)

**Branch:** `codex/unity-dynasty-political-events`

**Context:** The browser spec tracks time-locked political state beyond succession -- covenant-test cooldowns, failed divine-right cooldowns, and general effect multipliers that gate certain diplomatic actions and apply asymmetric penalties. This is the generalized container for any timed political condition.

**Browser references:**
- `src/game/core/simulation.js` search `getDynastyPoliticalEventState` (~line 3126): the state shape (effect multipliers, cooldown timestamps)
- `src/game/core/simulation.js` search `tickDynastyPoliticalEvents` (~line 3950): the per-tick handler that decrements cooldowns and removes expired events
- `src/game/core/simulation.js` search `DIVINE_RIGHT_FAILED_COOLDOWN_IN_WORLD_DAYS` and `COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS`: the cooldown constants

**Implement:**
- `DynastyPoliticalEventComponent.cs`: DynamicBuffer element per faction -- `EventType` (FixedString32: "CovenantTestCooldown", "DivineRightFailedCooldown", "SuccessionShock", etc.), `ExpiresAtInWorldDays` (float), `ResourceTrickleFactor` (float, default 1.0), `AttackMultiplier` (float, default 1.0), `StabilizationMultiplier` (float, default 1.0)
- `DynastyPoliticalEventSystem.cs` (ISystem): per DualClock day, remove expired events from buffer; apply composite effect multipliers to relevant systems
- Wire divine-right-failed cooldown: when DivineRight declaration fails (check `PlayerDivineRightDeclarationSystem`), push a cooldown event onto the buffer
- Wire covenant-test cooldown: placeholder hook point (covenant test system lands in Priority 3)
- Add `TryDebugGetPoliticalEvents(factionId)` to debug surface

**Smoke validator:** 2-phase: event pushed to buffer applies multiplier; event expires and removes itself after correct world-day duration.

---

### PRIORITY 3: Covenant Test Execution

**Lane:** `faith-covenant-test` (new lane -- claim it)

**Branch:** `codex/unity-faith-covenant-test`

**Context:** Covenant tests are the high-stakes faith mechanic that gate divine-right declarations. A faction must sustain faith intensity above 80 for 180 in-world days, then voluntarily trigger the test action. Each faith covenant has a distinct cost structure and resolution. Failure applies intensity loss, legitimacy penalty, and pushes a political event cooldown (Priority 2). Success floors intensity at 82 and grants legitimacy. This is a load-bearing game mechanic -- without it, divine-right victories have no real prerequisite cost.

**Browser references -- read ALL of these before implementing:**
- `src/game/core/simulation.js` search `COVENANT_TEST_INTENSITY_THRESHOLD` (~line 119): value is 80
- `src/game/core/simulation.js` search `COVENANT_TEST_DURATION_IN_WORLD_DAYS` (~line 120): value is 180
- `src/game/core/simulation.js` search `COVENANT_TEST_RETRY_COOLDOWN_IN_WORLD_DAYS` (~line 121): cooldown on failure
- `src/game/core/simulation.js` search lines 134-141 for per-covenant cost structures (blood dominion, the order, the wild -- each has unique resource/legitimacy cost)
- `src/game/core/simulation.js` search `performCovenantTestAction` (exported function): the gate chain (intensity threshold, duration met, no active cooldown, faction has resources for cost)
- `src/game/core/simulation.js` search `ensureFaithCovenantTestCompletionFromLegacyState` (~line 773): legacy-state restoration hook (port the state shape it reveals)

**Implement:**
- `CovenantTestStateComponent.cs`: per-faction -- `IntensityThresholdMetAtInWorldDays` (float, NaN if not yet met), `TestPhase` (byte: 0=Inactive, 1=Qualifying, 2=ReadyToTrigger, 3=InProgress, 4=Complete, 5=Failed), `TestStartedAtInWorldDays` (float), `LastFailedAtInWorldDays` (float), `SuccessCount` (int)
- `CovenantTestQualificationSystem.cs` (ISystem): per DualClock day, if faith intensity >= 80 and `IntensityThresholdMetAtInWorldDays` is NaN, record it; if intensity drops below 80, reset; if qualifying duration >= 180 days and no cooldown active, advance to ReadyToTrigger
- `PlayerCovenantTestRequestComponent.cs`: one-shot dispatch tag (consumed by resolution system)
- `CovenantTestResolutionSystem.cs` (ISystem in player-covert-ops lane or faith-covenant-test lane): consume request; gate chain: TestPhase == ReadyToTrigger, faction has resources for covenant-specific cost; deduct cost; on success set TestPhase = Complete, floor intensity at 82, apply legitimacy bonus; on failure set TestPhase = Failed, apply intensity loss, legitimacy penalty, push political event cooldown via DynastyPoliticalEventSystem
- Add `TryDebugTriggerCovenantTest(factionId)` and `TryDebugGetCovenantTestState(factionId)` to debug surface
- Add `TryDebugSetFaithIntensity(factionId, value)` helper for testing if not already present

**Smoke validator:** `BloodlinesCovenantTestSmokeValidation.cs` / `Invoke-BloodlinesUnityCovenantTestSmokeValidation.ps1`
- Phase 1: faction reaching intensity >= 80 and holding it for 180 in-world days advances TestPhase to ReadyToTrigger
- Phase 2: test trigger succeeds when resources available -- TestPhase = Complete, intensity floored at 82
- Phase 3: test trigger fails when resources absent -- intensity loss applied, cooldown political event pushed
- Phase 4: retry blocked during cooldown window

---

### PRIORITY 4: Governor Specialization System

**Lane:** `territory-governor-specialization` (new lane -- claim it)

**Branch:** `codex/unity-territory-governor-specialization`

**Context:** In the browser spec, control points with an assigned governor gain a specialization profile that multiplies resource trickle, stabilization rate, capture resistance, and loyalty protection. Three specializations: Border Marshal (military/defensive focus), Civic Steward (economy/loyalty focus), Keep Castellan (fortification/healing focus). Specialization is auto-selected based on the governor's role and the settlement class. Without this, all control points are economically identical regardless of governance -- a significant gameplay flatness.

**Browser references:**
- `src/game/core/simulation.js` search `DEFAULT_GOVERNOR_SPECIALIZATION` (~line 310): fallback profile values
- `src/game/core/simulation.js` search `GOVERNOR_SPECIALIZATION_PROFILES` (~lines 321-352): Border Marshal, Civic Steward, Keep Castellan with their effect multipliers
- `src/game/core/simulation.js` search `getGovernorSpecializationIdForSettlementClass` (~line 2930): how settlement class determines which specialization is selected
- `src/game/core/simulation.js` search `getGovernorProfileForControlPoint` (~line 4493): the full profile lookup for a given control point

**Implement:**
- `GovernorSpecializationComponent.cs`: per-control-point -- `SpecializationId` (byte: 0=None, 1=BorderMarshal, 2=CivicSteward, 3=KeepCastellan), `ResourceTrickleMultiplier` (float), `StabilizationMultiplier` (float), `CaptureResistanceBonus` (float), `LoyaltyProtectionMultiplier` (float), `ReserveRegenMultiplier` (float), `HealRegenMultiplier` (float)
- `GovernorSpecializationSystem.cs` (ISystem): per DualClock day, for each control point that has an assigned governor (read DynastyMemberComponent governor assignment), look up settlement class, select specialization, write multipliers; remove component if governor unassigned
- Wire `ResourceTrickleMultiplier` into economy trickle reads (same pattern as SuccessionCrisis ResourceTrickleFactor)
- Wire `LoyaltyProtectionMultiplier` into RealmConditionCycleSystem loyalty reads
- Wire `CaptureResistanceBonus` into AttackResolutionSystem capture logic
- Add `TryDebugGetGovernorSpecialization(controlPointId)` to debug surface

**Smoke validator:** 3-phase: control point with governor gets correct specialization for its settlement class; multipliers are non-unity and within expected range; control point with no governor has no specialization component.

---

### PRIORITY 5: Commander Aura System

**Lane:** `combat-commander-aura` (new lane -- claim it)

**Branch:** `codex/unity-combat-commander-aura`

**Context:** `CommanderComponent.cs` exists on master but does nothing beyond tagging the entity. The browser spec grants commanders an aura radius that buffs nearby friendly units: attack bonus, sight range extension, movement speed bonus, and morale bonus (resistance to retreat). The aura scales with the commander's conviction band. This creates a meaningful elite-unit upgrade incentive.

**Browser references:**
- `src/game/core/simulation.js` search `getCommanderAuraProfile` (~line 6046): the per-role aura profile (radius, attack bonus, sight bonus, speed bonus, morale bonus)
- `src/game/core/simulation.js` search `COMMANDER_BASE_AURA_RADIUS` (~line 8): base radius value
- `src/game/core/simulation.js` search governor stabilization bonus (~line 10): how conviction band multiplies aura effectiveness
- `src/game/core/simulation.js` search faith doctrine aura bonuses: faith doctrine may extend aura radius or buff specific properties

**Implement:**
- `CommanderAuraComponent.cs`: per-commander unit entity -- `AuraRadius` (float), `AttackBonus` (float), `SightBonus` (float), `SpeedBonus` (float), `MoraleBonus` (float), `ConvictionBandMultiplier` (float)
- `CommanderAuraSystem.cs` (ISystem, [BurstCompile]): per simulation frame, for each commander, iterate nearby friendly units within AuraRadius using spatial query; write buffed values onto `CombatStatsComponent` and `MovementStatsComponent` of affected units; clear buffs when commander moves out of range or dies. Use NativeList + spatial proximity pattern consistent with UnitSeparationSystem.
- Wire conviction band into aura multiplier (read ConvictionComponent on faction entity)
- Add `TryDebugGetCommanderAura(unitId)` to debug surface

**Smoke validator:** 3-phase: friendly units within aura radius receive attack/sight/speed buffs; units outside radius receive no buff; commander death removes all active buffs.

---

### PRIORITY 6: Imminent Engagement Postures

**Lane:** `fortification-postures` (new lane -- claim it, extends `fortification-siege-imminent-engagement`)

**Branch:** `codex/unity-fortification-postures`

**Context:** `ImminentEngagementComponent` exists but posture selection and effect application are not implemented. The browser spec defines three defender postures: Brace (maximize reserve healing, reduce mustering speed), Steady (balanced defaults), and Counterstroke (maximize frontline bonus, reduce retreat threshold). Posture selection should be available to AI and player before imminent engagement resolves.

**Browser references:**
- `src/game/core/simulation.js` search `IMMINENT_ENGAGEMENT_POSTURES` (~lines 271-308): the three posture definitions with their effect values
- `src/game/core/simulation.js` search `tickImminentEngagementWarnings` (~line 11742): how posture feeds into the engagement warning tick

**Implement:**
- `ImminentEngagementPostureComponent.cs`: per-settlement -- `PostureId` (byte: 0=Steady, 1=Brace, 2=Counterstroke), `ReserveHealMultiplier` (float), `MusteringSpeedMultiplier` (float), `FrontlineBonusMultiplier` (float), `RetreatThresholdMultiplier` (float)
- `ImminentEngagementPostureSystem.cs` (ISystem): when ImminentEngagementComponent is active on a settlement, look up AI or player posture selection and write posture effects; wire multipliers into FortificationReserveSystem (healing) and AttackResolutionSystem (frontline bonus, retreat threshold)
- `PlayerImminentEngagementPostureRequestComponent.cs`: one-shot tag for player-initiated posture change
- Add `TryDebugSetImminentEngagementPosture(settlementId, postureId)` to debug surface

**Smoke validator:** 3-phase: Brace posture increases reserve heal rate; Counterstroke posture increases frontline bonus; posture effects removed when imminent engagement resolves.

---

### PRIORITY 7: Verdant Warden Faith Unit Support

**Lane:** `faith-verdant-warden` (new lane -- claim it)

**Branch:** `codex/unity-faith-verdant-warden`

**Context:** Verdant Warden units are faith-aligned support units that apply stacked loyalty, healing, and stabilization bonuses to nearby control points and siege lines. This is the mobile faith-unit multiplier that rewards building The Wild covenant units and deploying them strategically.

**Browser references:**
- `src/game/core/simulation.js` search `isVerdantWardenUnit` (~line 2877): unit type check
- `src/game/core/simulation.js` search `getVerdantWardenSupportProfile` (~line 2881): the zone support profile for a warden unit
- `src/game/core/simulation.js` search `getVerdantWardenZoneSupportProfile` (~line 2900): how multiple wardens stack (cap applies)
- `src/game/core/simulation.js` search `DEFAULT_VERDANT_WARDEN_SUPPORT` (~lines 420-431): the base values for loyalty bonus, healing bonus, stabilization bonus, radius, and stack cap

**Implement:**
- `VerdantWardenComponent.cs`: tag component on warden-type units -- `SupportRadius` (float), `LoyaltyBonusPerTick` (float), `HealingBonusPerTick` (float), `StabilizationBonusPerTick` (float)
- `VerdantWardenSupportSystem.cs` (ISystem, [BurstCompile]): per DualClock tick, for each warden unit, find control points within SupportRadius; apply loyalty/healing/stabilization bonuses up to the stack cap; write onto ControlPointComponent/FortificationComponent
- Add `TryDebugGetVerdantWardenCoverage(controlPointId)` to debug surface

**Smoke validator:** 2-phase: control point within warden radius receives loyalty/healing/stabilization bonus; stacking cap prevents bonus exceeding maximum at multiple wardens.

---

### PRIORITY 8: Sacred Site Exposure Walker + Wayshrine Amplification

**Lane:** `faith-exposure-walker` (new lane -- claim it)

**Branch:** `codex/unity-faith-exposure-walker`

**Context:** Sacred sites anchor faith exposure spread: they apply exposure gain to nearby territories at a rate proportional to faith commitment and intensity. When a control point's exposure reaches 100, it flips to committed state for that faith. Wayshrine buildings amplify the exposure gain multiplier for units within their radius. Together these create geographic faith-spread competition that makes faith choice strategically territorial.

**Browser references:**
- `src/game/core/simulation.js` search `updateFaithExposure` (~line 8174): the per-tick exposure update loop -- finds sacred sites, computes radius, applies exposure gain to control points in range, checks commitment threshold
- `src/game/core/simulation.js` search `getWayshrineExposureMultiplierAt` (~line 8246): how wayshrine buildings stack to increase exposure rate
- `src/game/core/simulation.js` search `SACRED_SITE_EXPOSURE_RADIUS` and `SACRED_SITE_BASE_EXPOSURE_RATE`: the constants for base radius and rate

**Implement:**
- `SacredSiteComponent.cs`: per-entity -- `FaithId` (byte matching FaithComponent FaithId), `ExposureRadius` (float), `BaseExposureRatePerDay` (float)
- `FaithExposureComponent.cs`: per-control-point -- `ExposurePct` (float 0-100 per faith, store as NativeArray\[4\] for 4 faiths), `CommittedFaithId` (byte, 255 = none)
- `FaithExposureWalkerSystem.cs` (ISystem, [BurstCompile]): per DualClock day, for each sacred site, find control points within ExposureRadius; apply exposure gain scaled by owning faction's faith intensity and commitment; check wayshrine amplification; flip control point when exposure reaches 100
- `WayshrineAmplificationComponent.cs`: tag + data on wayshrine building entities -- `AmplificationRadius` (float), `ExposureMultiplier` (float)
- Add `TryDebugGetFaithExposure(controlPointId)` to debug surface

**Smoke validator:** 3-phase: control point within sacred site radius gains exposure over ticks; exposure reaches 100 and flips commitment; wayshrine increases exposure rate vs. control without wayshrine.

---

### PRIORITY 9: Faith Structure Intensity Regen

**Lane:** `faith-structure-regen` (new lane -- claim it, or extend faith-exposure-walker)

**Branch:** `codex/unity-faith-structure-regen`

**Context:** Faith buildings (temples, covenant halls, grand sanctuaries) apply passive intensity regen to the owning faction. Regen scales with building count and caps at `COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND`. This is distinct from the covenant-test qualification logic; it is the passive recovery path that rewards faith-building investment.

**Browser references:**
- `src/game/core/simulation.js` search `getFaithStructureIntensityRegenRate` (~line 8226): how building count translates to regen rate
- `src/game/core/simulation.js` search `COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND` (~line 823): the cap value (~1.4)
- `src/game/core/simulation.js` search lines 823-828 for the regen-per-building curve

**Implement:**
- `FaithStructureRegenSystem.cs` (ISystem): per DualClock day, count faith buildings per faction (read BuildingTypeComponent); compute regen rate; clamp at cap; add to FaithComponent.Intensity
- Add faith building type tags to BuildingTypeComponent enum if not present (Temple, CovenantHall, GrandSanctuary)
- Wire into the existing faith intensity update path (do not duplicate intensity update logic -- add as an additive term)

**Smoke validator:** 2-phase: faction with 3 faith buildings regens intensity faster than faction with 1; regen is capped regardless of building count.

---

### PRIORITY 10: Captive Ransom Trickle System

**Lane:** `player-diplomacy` (existing lane)

**Branch:** `codex/unity-player-captive-ransom-trickle`

**Context:** Factions holding captives should passively earn influence and renown per tick, proportional to the captive's role and renown weight. This makes the decision to hold vs. ransom a genuine economic tradeoff.

**Browser references:**
- `src/game/core/simulation.js` search `updateCaptiveRansomTrickle` (~line 4885): the trickle calculator -- iterates captive buffer, sums influence/renown per tick
- `src/game/core/simulation.js` search `CAPTIVE_INFLUENCE_TRICKLE` (~line 12): base influence per captive per tick
- `src/game/core/simulation.js` search `CAPTIVE_RENOWN_WEIGHT` (~line 13): the renown multiplier by role

**Implement:**
- `CaptiveRansomTrickleSystem.cs` (ISystem in PlayerDiplomacy lane): per DualClock day, for each faction that has a CapturedMemberElement buffer, compute influence/renown trickle; add to FactionComponent resource pools
- Wire into FactionComponent: confirm influence and renown fields exist; add if absent
- Add `TryDebugGetCaptiveTrickle(factionId)` to debug surface

**Smoke validator:** 2-phase: faction holding a high-renown captive earns more influence per tick than faction holding low-renown captive; faction with no captives earns zero trickle.

---

### PRIORITY 11: Covert Ops Full Resolution Effects

**Lane:** `player-covert-ops` (existing lane)

**Branch:** `codex/unity-covert-ops-resolution-effects`

**Context:** Assassination and sabotage dispatch systems exist on master, but their resolution effects (member death with succession ripple, building damage halt, legitimacy penalty on failure) are not fully wired. Espionage generates an intelligence report buffer but the report content (member/building/resource snapshot) may be incomplete.

**Browser references:**
- `src/game/core/simulation.js` search `applyAssassinationEffect` (~line 5390): what happens on successful assassination -- member killed, succession triggered, loyalty shock, legitimacy loss, renown change
- `src/game/core/simulation.js` search lines 4743-4807: commander fall and governor loss legitimacy penalties (use the same values for assassination victim by role)
- `src/game/core/simulation.js` search sabotage operation resolution: building production halt duration, damage state transition
- `src/game/core/simulation.js` search espionage operation resolution: dossier generation, what intelligence fields are populated

**Implement:**
- `AssassinationResolutionSystem.cs` (ISystem, or extend existing assassination system): on operation success, kill target DynastyMember entity (set health to 0, trigger DeathResolutionSystem path), apply loyalty shock to all faction control points, apply legitimacy penalty scaled by victim role; on failure, apply counter-intelligence boost to defender
- `SabotageResolutionSystem.cs` (ISystem): on operation success, set target building to Damaged state (BuildingTypeComponent damage flag), halt production for N in-world days; on failure, apply legitimacy penalty to attacker
- `EspionageResolutionSystem.cs` (ISystem): on operation success, populate intelligence report buffer with faction's current member list (names/roles), building count by type, resource quantities (within detection range); on failure, decrement attacker counter-intelligence
- Add resolve debug entries to `BloodlinesDebugCommandSurface.PlayerCovertOps.cs` (or create if partial file)

**Smoke validator:** 4-phase: assassination success kills target and triggers succession ripple; sabotage success halts production on target building; espionage success populates intelligence report; all three operations apply correct legitimacy penalty on failure.

---

### PRIORITY 12: Governance Coalition Pressure

**Lane:** `world-governance-coalition` (new lane, extends world-pressure-escalation)

**Branch:** `codex/unity-governance-coalition-pressure`

**Context:** When the TerritorialGovernance victory condition acceptance crosses 60%, the browser spec applies coordinated frontier pressure from hostile kingdoms: loyalty erosion on the weakest march, legitimacy strain on the leading faction, and drag on acceptance growth. This creates a coalition-style counter-pressure mechanic that punishes runaway territorial leaders.

**Browser references:**
- `src/game/core/simulation.js` search `GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE` (~line 167): base loyalty erosion per cycle
- `src/game/core/simulation.js` search `GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE` (~line 168): legitimacy strain on leading faction
- `src/game/core/simulation.js` search `getTerritorialGovernanceWorldPressureContribution` (~line 13192): how acceptance pct maps to coalition activation threshold (60%)
- `src/game/core/simulation.js` search `GOVERNANCE_ALLIANCE_ACCEPTANCE_RISE_DRAG` if present: the drag on acceptance growth

**Implement:**
- `GovernanceCoalitionPressureSystem.cs` (ISystem): per DualClock day, check TerritorialGovernance acceptance pct across all factions; when leading faction exceeds 60%, apply loyalty erosion to its weakest march control point; apply legitimacy strain to leading faction; apply acceptance drag to its acceptance accumulator
- Wire into VictoryConditionEvaluationSystem acceptance tracking
- Add `TryDebugGetGovernanceCoalitionState()` to debug surface

**Smoke validator:** 2-phase: faction at 65% acceptance receives loyalty erosion and legitimacy strain; faction at 50% acceptance does not trigger coalition pressure.

---

### PRIORITY 13: Minor House Levy Validation and Completion

**Lane:** `dynasty-minor-house-levy` (new lane, extends existing minor-house parity)

**Branch:** `codex/unity-dynasty-minor-house-levy-complete`

**Context:** The minor-house levy framework was landed as part of the tier2-batch-dynasty-systems, but the levy-progress decay and the unit-tier gating by loyalty level need explicit validation and may be incomplete. Read the browser spec and verify Unity parity; fill any gaps.

**Browser references:**
- `src/game/core/simulation.js` search `ensureMinorHouseLevyState` (~line 488): the levy state initializer
- `src/game/core/simulation.js` search `pickMinorHouseLevyProfile` (~line 520): how unit profile is selected by loyalty
- `src/game/core/simulation.js` search `tickMinorHouseTerritorialLevies` (~line 7060): the per-tick levy accumulation and decay
- `src/game/core/simulation.js` search `MINOR_HOUSE_LEVY_PROFILES` (~lines 247-269): the profile table
- `src/game/core/simulation.js` search `MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND` (~line 245): the decay constant

**Implement:**
- Read the existing `MinorHouseLevySystem` (find it under `unity/Assets/_Bloodlines/Code/Dynasties/` or Components) and compare against the browser spec
- Port any missing fields: levy progress decay, loyalty-gated unit tier selection, claim gate (loyalty >= threshold before levying begins)
- Add `TryDebugGetMinorHouseLevyState(factionId)` to debug surface if not present

**Smoke validator:** 3-phase: minor house with high loyalty levies higher-tier unit profile; levy progress decays when no territory claimed; levy blocked when loyalty below claim gate.

---

### PRIORITY 14: Trueborn City Rise Arc (Sub-Slice 1 of 3)

**Lane:** `world-trueborn-rise` (new lane -- claim it)

**Branch:** `codex/unity-world-trueborn-rise-arc-1`

**Context:** The Trueborn City is a special NPC faction that applies escalating world pressure across all kingdoms in late-game play. Kingdoms can formally recognize the city's claim for standing bonus and exemption from pressure -- at a legitimacy cost. This is the late-game global pressure mechanic that drives urgency and coalition-or-submit decisions. Implement Sub-Slice 1 of 3 in this directive window: the rise-arc state component and the base pressure application. Sub-Slices 2 and 3 (recognition action, diplomatic escalation) will follow in the next directive.

**Browser references:**
- `src/game/core/simulation.js` search `updateTruebornRiseArc` (~line 13838): the per-tick rise arc tick -- stages, pressure values per stage, escalation timing
- `src/game/core/simulation.js` search `TRUEBORN_RISE_STAGE_*` constants: stage delays and pressure per stage
- `src/game/core/simulation.js` search `getTruebornRecognitionTerms` (~line 13042): what recognition costs per kingdom
- `src/game/core/simulation.js` search `recognizeTruebornClaim` (~line 13069): the recognition action effects

**Implement (Sub-Slice 1):**
- `TruebornRiseArcComponent.cs`: singleton -- `CurrentStage` (byte 0-4), `StageStartedAtInWorldDays` (float), `GlobalPressurePerDay` (float), `LoyaltyErosionPerDay` (float per non-recognizing kingdom), `RecognizedFactionsBitmask` (ulong for up to 64 factions)
- `TruebornRiseArcSystem.cs` (ISystem): per DualClock day, advance stage when stage duration elapsed; apply global pressure to all non-recognizing factions (loyalty erosion on frontier provinces, legitimacy strain); skip recognized factions
- Do NOT implement recognition action dispatch in this sub-slice (that is Sub-Slice 2)

**Smoke validator:** 2-phase: stage advances after correct world-day duration; non-recognizing factions receive loyalty erosion; recognized faction (manually set via debug) does not receive pressure.

---

## NON-NEGOTIABLE PROCESS RULES

1. **Read `docs/unity/CONCURRENT_SESSION_CONTRACT.md` before every session.** Do not touch any file outside your claimed lane's owned paths without a cross-lane read justification documented in the handoff.

2. **Every sub-slice must pass all 10 canonical validation gates**, run serially:
   - `dotnet build unity/Assembly-CSharp.csproj -nologo` -- 0 errors
   - `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- 0 errors
   - `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
   - `scripts/Invoke-BloodlinesUnityCombatSmokeValidation.ps1`
   - `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
   - `scripts/Invoke-BloodlinesUnityFortificationSmokeValidation.ps1`
   - `scripts/Invoke-BloodlinesUnitySiegeSmokeValidation.ps1`
   - `node tests/data-validation.mjs`
   - `node tests/runtime-bridge.mjs`
   - `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1`

3. **Write a per-slice handoff** at `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<slice>.md`. Include: goal, browser reference line numbers or search terms used, work completed, validation proof lines (quote actual output), Unity-side simplifications deferred, exact next action.

4. **Update `docs/unity/CONCURRENT_SESSION_CONTRACT.md` after every sub-slice**: bump Revision, set Last Updated to today (YYYY-MM-DD), set Last Updated By to `codex-<lane>-YYYY-MM-DD`, add or update the lane subsection with new owned paths and handoff reference.

5. **Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/PROJECT_STATE.json` after every sub-slice.** Append only -- never overwrite or touch another lane's entries.

6. **Create a new branch per sub-slice** following the pattern `codex/unity-<lane>-<description>`. Check out from `origin/master`, not from a prior working branch.

7. **Use the wrapper lock** for all Unity batch invocations: `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-<identifier>`. Check for the lock, reclaim if > 15 minutes old, delete after completion.

8. **Push branches to origin after validation passes.** Merge to master with `git merge --no-ff <branch> -m "Merge <branch>: <one-line description>"`. Run all 10 gates again on master after merging.

9. **Do not modify browser runtime files** (`src/`, `data/`, `tests/`, `play.html`). Read-only behavioral specification.

10. **Follow Unity 6.3 DOTS/ECS idioms:** ISystem (not MonoBehaviour), SystemAPI, EntityCommandBuffer, NativeArray, `[BurstCompile]` on hot paths.

11. **Write a dedicated smoke validator** for every new system in `unity/Assets/_Bloodlines/Code/Editor/` with matching PowerShell wrapper in `scripts/`.

12. **Cross-reference browser simulation.js search terms** in every handoff. If the browser has no equivalent (the system is aspirational), implement from the design canon and document the divergence.

13. **Do NOT stop after one sub-slice.** Continue through the priority stack immediately. Only stop when session capacity genuinely runs out.

---

## MERGE PROTOCOL

After completing each sub-slice:
1. Push the branch to `origin/<branch-name>`
2. Merge to master: `git merge --no-ff <branch> -m "Merge <branch>: <one-line description>"`
3. If conflicts: document them in the handoff and leave the merge pending -- do not force
4. Run the full 10-gate chain again on master after merging

---

## SESSION CONTINUITY (MANDATORY -- EVEN IF INTERRUPTED MID-SLICE)

Before ending any session:
1. Commit all WIP with a clear message indicating incomplete state
2. Push to origin
3. Update `NEXT_SESSION_HANDOFF.md` with exactly what was completed and the single specific next action
4. Update `continuity/PROJECT_STATE.json`
5. Write or update the per-slice handoff (partial is fine)

---

## SUCCESS CRITERIA FOR THIS DIRECTIVE WINDOW

After 2-3 days:
- Succession Crisis System on master (P1)
- Political Event System on master (P2)
- Covenant Test Execution on master (P3) -- this is the most gameplay-critical item
- Governor Specialization on master (P4)
- Commander Aura System on master (P5)
- Imminent Engagement Postures on master (P6)
- Verdant Warden Support on master (P7)
- Sacred Site Exposure Walker + Wayshrine Amplification on master (P8)
- Faith Structure Intensity Regen on master (P9)
- Captive Ransom Trickle on master (P10)
- Covert Ops Full Resolution Effects on master (P11)
- Governance Coalition Pressure on master (P12)
- Minor House Levy validated and completed on master (P13)
- Trueborn City Rise Arc Sub-Slice 1 on master (P14)
- Contract revision advanced at least 10-15 increments from revision 89
- No regressions in any existing smoke validators
