# CODEX MULTI-DAY DIRECTIVE: BLOODLINES UNITY DEEP SYSTEMS PUSH II
# Date: 2026-04-24
# Duration: 4-5 days of continuous autonomous sessions
# Owner: Lance Fisher
# Supersedes: CODEX_MULTI_DAY_DIRECTIVE_2026-04-23.md
# Contract revision at write time: 42 (file), ~105 effective (estimated after April 21-22 run)
# Target: advance contract file to revision 130+

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

## CURRENT BUILD STATE (as of 2026-04-22)

All of the following are on master and fully validated:

**Core simulation:** Economy, construction, production, starvation/loyalty/cap-pressure, stability surplus, loyalty density, state snapshot/restore, dual-clock, five-stage match progression, world pressure escalation, victory conditions (CommandHallFall, TerritorialGovernance, DivineRight).

**Combat:** Melee, ranged projectile, explicit attack orders, attack-move, target acquisition with sight loss, group movement, soft unit separation, hold-position and retreat stances.

**Fortification and siege (sub-slices 1-13 plus follow-ups):** Full tier/reserve/breach/sealing/recovery chain, tier-scaled sealing costs, worker-locality gate, repair narrative, destroyed-counter recovery, breach depth telemetry, breach legibility readout, imminent engagement component, siege supply interdiction.

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

Do not stop after a sub-slice. Complete it, validate it, push it, merge it, then immediately begin the next. Chain as many as your session allows. The priorities below represent approximately 4-5 days of continuous work.

---

### PRIORITY 1: Succession Crisis System

**Lane:** `dynasty-succession-crisis` (new lane -- claim it)

**Branch:** `codex/unity-dynasty-succession-crisis`

**Context:** `DynastySuccessionSystem.cs` exists and handles member succession events, but there is no system that models the political instability cascade that follows a head-of-bloodline death. The browser spec tracks crisis severity (minor/moderate/major/catastrophic), applies resource multipliers, and drives recovery costs. Without this, dynasty deaths are mechanical non-events; with it, they become genuine turning points that reshape the map.

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

**Context:** Covenant tests are the high-stakes faith mechanic that gate divine-right declarations. A faction must sustain faith intensity above 80 for 180 in-world days, then voluntarily trigger the test action. Each faith covenant has a distinct cost structure and resolution. Failure applies intensity loss, legitimacy penalty, and pushes a political event cooldown. Success floors intensity at 82 and grants legitimacy. This is a load-bearing game mechanic -- without it, divine-right victories have no real prerequisite cost.

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
- `CovenantTestResolutionSystem.cs` (ISystem in faith-covenant-test lane): consume request; gate chain: TestPhase == ReadyToTrigger, faction has resources for covenant-specific cost; deduct cost; on success set TestPhase = Complete, floor intensity at 82, apply legitimacy bonus; on failure set TestPhase = Failed, apply intensity loss, legitimacy penalty, push political event cooldown via DynastyPoliticalEventSystem
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

**Context:** Control points with an assigned governor gain a specialization profile that multiplies resource trickle, stabilization rate, capture resistance, and loyalty protection. Three specializations: Border Marshal (military/defensive), Civic Steward (economy/loyalty), Keep Castellan (fortification/healing). Specialization is auto-selected based on the governor's role and the settlement class. Without this, all control points are economically identical regardless of governance.

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
- `src/game/core/simulation.js` search conviction band multiplier for aura effectiveness: how conviction band multiplies aura values
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

**Context:** `ImminentEngagementComponent` exists but posture selection and effect application are not implemented. Three defender postures: Brace (maximize reserve healing, reduce mustering speed), Steady (balanced defaults), Counterstroke (maximize frontline bonus, reduce retreat threshold). Posture selection should be available to AI and player before imminent engagement resolves.

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

**Context:** Sacred sites anchor faith exposure spread: they apply exposure gain to nearby territories proportional to faith commitment and intensity. When a control point's exposure reaches 100, it flips to committed state. Wayshrine buildings amplify the exposure gain multiplier. Together these create geographic faith-spread competition.

**Browser references:**
- `src/game/core/simulation.js` search `updateFaithExposure` (~line 8174): the per-tick exposure update loop
- `src/game/core/simulation.js` search `getWayshrineExposureMultiplierAt` (~line 8246): how wayshrine buildings stack to increase exposure rate
- `src/game/core/simulation.js` search `SACRED_SITE_EXPOSURE_RADIUS` and `SACRED_SITE_BASE_EXPOSURE_RATE`: the base constants

**Implement:**
- `SacredSiteComponent.cs`: per-entity -- `FaithId` (byte), `ExposureRadius` (float), `BaseExposureRatePerDay` (float)
- `FaithExposureComponent.cs`: per-control-point -- `ExposurePct` (float 0-100 per faith, NativeArray[4] for 4 faiths), `CommittedFaithId` (byte, 255 = none)
- `FaithExposureWalkerSystem.cs` (ISystem, [BurstCompile]): per DualClock day, for each sacred site, find control points within ExposureRadius; apply exposure gain scaled by owning faction's faith intensity and commitment; check wayshrine amplification; flip control point when exposure reaches 100
- `WayshrineAmplificationComponent.cs`: tag + data on wayshrine building entities -- `AmplificationRadius` (float), `ExposureMultiplier` (float)
- Add `TryDebugGetFaithExposure(controlPointId)` to debug surface

**Smoke validator:** 3-phase: control point within sacred site radius gains exposure over ticks; exposure reaches 100 and flips commitment; wayshrine increases exposure rate vs. control without wayshrine.

---

### PRIORITY 9: Faith Structure Intensity Regen

**Lane:** `faith-structure-regen` (new lane -- claim it, or extend faith-exposure-walker)

**Branch:** `codex/unity-faith-structure-regen`

**Context:** Faith buildings (temples, covenant halls, grand sanctuaries) apply passive intensity regen to the owning faction. Regen scales with building count and caps at `COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND`. This is the passive recovery path that rewards faith-building investment.

**Browser references:**
- `src/game/core/simulation.js` search `getFaithStructureIntensityRegenRate` (~line 8226): how building count translates to regen rate
- `src/game/core/simulation.js` search `COVENANT_TEST_MAX_FAITH_BUILDING_REGEN_PER_SECOND` (~line 823): the cap value (~1.4)
- `src/game/core/simulation.js` search lines 823-828 for the regen-per-building curve

**Implement:**
- `FaithStructureRegenSystem.cs` (ISystem): per DualClock day, count faith buildings per faction (read BuildingTypeComponent); compute regen rate; clamp at cap; add to FaithComponent.Intensity
- Add faith building type tags to BuildingTypeComponent enum if not present (Temple, CovenantHall, GrandSanctuary)
- Wire into the existing faith intensity update path as an additive term

**Smoke validator:** 2-phase: faction with 3 faith buildings regens intensity faster than faction with 1; regen is capped regardless of building count.

---

### PRIORITY 10: Captive Ransom Trickle System

**Lane:** `player-diplomacy` (existing lane)

**Branch:** `codex/unity-player-captive-ransom-trickle`

**Context:** Factions holding captives should passively earn influence and renown per tick, proportional to the captive's role and renown weight. Makes the hold-vs-ransom decision a genuine economic tradeoff.

**Browser references:**
- `src/game/core/simulation.js` search `updateCaptiveRansomTrickle` (~line 4885): the trickle calculator
- `src/game/core/simulation.js` search `CAPTIVE_INFLUENCE_TRICKLE` (~line 12): base influence per captive per tick
- `src/game/core/simulation.js` search `CAPTIVE_RENOWN_WEIGHT` (~line 13): the renown multiplier by role

**Implement:**
- `CaptiveRansomTrickleSystem.cs` (ISystem in PlayerDiplomacy lane): per DualClock day, for each faction that has a CapturedMemberElement buffer, compute influence/renown trickle; add to FactionComponent resource pools
- Confirm influence and renown fields exist on FactionComponent; add if absent
- Add `TryDebugGetCaptiveTrickle(factionId)` to debug surface

**Smoke validator:** 2-phase: faction holding a high-renown captive earns more influence per tick than faction holding low-renown captive; faction with no captives earns zero trickle.

---

### PRIORITY 11: Covert Ops Full Resolution Effects

**Lane:** `player-covert-ops` (existing lane)

**Branch:** `codex/unity-covert-ops-resolution-effects`

**Context:** Assassination and sabotage dispatch systems exist on master, but their resolution effects are not fully wired. Member death with succession ripple, building damage halt, legitimacy penalty on failure, and full espionage dossier content are incomplete.

**Browser references:**
- `src/game/core/simulation.js` search `applyAssassinationEffect` (~line 5390): member killed, succession triggered, loyalty shock, legitimacy loss, renown change
- `src/game/core/simulation.js` search lines 4743-4807: commander fall and governor loss legitimacy penalties
- `src/game/core/simulation.js` search sabotage operation resolution: building production halt duration, damage state transition
- `src/game/core/simulation.js` search espionage operation resolution: dossier generation, intelligence fields populated

**Implement:**
- `AssassinationResolutionSystem.cs` (ISystem): on success, kill target DynastyMember entity (trigger DeathResolutionSystem path), apply loyalty shock to all faction control points, apply legitimacy penalty scaled by victim role; on failure, apply counter-intelligence boost to defender
- `SabotageResolutionSystem.cs` (ISystem): on success, set target building to Damaged state, halt production for N in-world days; on failure, apply legitimacy penalty to attacker
- `EspionageResolutionSystem.cs` (ISystem): on success, populate intelligence report buffer with faction's current member list (names/roles), building count by type, resource quantities; on failure, decrement attacker counter-intelligence
- Add resolve debug entries to `BloodlinesDebugCommandSurface.PlayerCovertOps.cs`

**Smoke validator:** 4-phase: assassination success kills target and triggers succession ripple; sabotage success halts production on target building; espionage success populates intelligence report; all three operations apply correct legitimacy penalty on failure.

---

### PRIORITY 12: Governance Coalition Pressure

**Lane:** `world-governance-coalition` (new lane -- claim it, extends world-pressure-escalation)

**Branch:** `codex/unity-governance-coalition-pressure`

**Context:** When the TerritorialGovernance victory condition acceptance crosses 60%, the browser spec applies coordinated frontier pressure from hostile kingdoms: loyalty erosion on the weakest march, legitimacy strain on the leading faction, and drag on acceptance growth. Creates a coalition-style counter-pressure mechanic.

**Browser references:**
- `src/game/core/simulation.js` search `GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE` (~line 167): base loyalty erosion per cycle
- `src/game/core/simulation.js` search `GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE` (~line 168): legitimacy strain on leading faction
- `src/game/core/simulation.js` search `getTerritorialGovernanceWorldPressureContribution` (~line 13192): how acceptance pct maps to coalition activation threshold (60%)

**Implement:**
- `GovernanceCoalitionPressureSystem.cs` (ISystem): per DualClock day, check TerritorialGovernance acceptance pct across all factions; when leading faction exceeds 60%, apply loyalty erosion to its weakest march control point; apply legitimacy strain to leading faction; apply acceptance drag to its acceptance accumulator
- Wire into VictoryConditionEvaluationSystem acceptance tracking
- Add `TryDebugGetGovernanceCoalitionState()` to debug surface

**Smoke validator:** 2-phase: faction at 65% acceptance receives loyalty erosion and legitimacy strain; faction at 50% acceptance does not trigger coalition pressure.

---

### PRIORITY 13: Minor House Levy Validation and Completion

**Lane:** `dynasty-minor-house-levy` (new lane -- claim it, extends existing minor-house parity)

**Branch:** `codex/unity-dynasty-minor-house-levy-complete`

**Context:** The minor-house levy framework was landed as part of the tier2-batch-dynasty-systems, but levy-progress decay and unit-tier gating by loyalty level need explicit validation. Read the browser spec and verify Unity parity; fill any gaps.

**Browser references:**
- `src/game/core/simulation.js` search `ensureMinorHouseLevyState` (~line 488): the levy state initializer
- `src/game/core/simulation.js` search `pickMinorHouseLevyProfile` (~line 520): how unit profile is selected by loyalty
- `src/game/core/simulation.js` search `tickMinorHouseTerritorialLevies` (~line 7060): the per-tick levy accumulation and decay
- `src/game/core/simulation.js` search `MINOR_HOUSE_LEVY_PROFILES` (~lines 247-269): the profile table
- `src/game/core/simulation.js` search `MINOR_HOUSE_LEVY_PROGRESS_DECAY_PER_SECOND` (~line 245): the decay constant

**Implement:**
- Read the existing `MinorHouseLevySystem` and compare against the browser spec
- Port any missing fields: levy progress decay, loyalty-gated unit tier selection, claim gate (loyalty >= threshold before levying begins)
- Add `TryDebugGetMinorHouseLevyState(factionId)` to debug surface if not present

**Smoke validator:** 3-phase: minor house with high loyalty levies higher-tier unit profile; levy progress decays when no territory claimed; levy blocked when loyalty below claim gate.

---

### PRIORITY 14: Trueborn City Rise Arc -- Sub-Slice 1: State and Base Pressure

**Lane:** `world-trueborn-rise` (new lane -- claim it)

**Branch:** `codex/unity-world-trueborn-rise-arc-1`

**Context:** The Trueborn City is a special NPC faction applying escalating world pressure in late game. Kingdoms can recognize its claim to gain exemption from pressure at a legitimacy cost. Sub-Slice 1: rise-arc state component and base pressure application only. Recognition dispatch and diplomatic escalation land in P15-P16.

**Browser references:**
- `src/game/core/simulation.js` search `updateTruebornRiseArc` (~line 13838): the per-tick rise arc tick -- stages, pressure values per stage, escalation timing
- `src/game/core/simulation.js` search `TRUEBORN_RISE_STAGE_*` constants: stage delays and pressure per stage
- `src/game/core/simulation.js` search `getTruebornRecognitionTerms` (~line 13042): what recognition costs per kingdom (needed to understand the pressure exception)
- `src/game/core/simulation.js` search `recognizeTruebornClaim` (~line 13069): the recognition action effects

**Implement (Sub-Slice 1 only):**
- `TruebornRiseArcComponent.cs`: singleton -- `CurrentStage` (byte 0-4), `StageStartedAtInWorldDays` (float), `GlobalPressurePerDay` (float), `LoyaltyErosionPerDay` (float per non-recognizing kingdom), `RecognizedFactionsBitmask` (ulong for up to 64 factions)
- `TruebornRiseArcSystem.cs` (ISystem): per DualClock day, advance stage when stage duration elapsed; apply global pressure to all non-recognizing factions (loyalty erosion on frontier provinces, legitimacy strain); skip recognized factions
- Do NOT implement recognition action dispatch in this sub-slice

**Smoke validator:** 2-phase: stage advances after correct world-day duration; non-recognizing factions receive loyalty erosion; recognized faction (set via debug) does not receive pressure.

---

### PRIORITY 15: Trueborn City Rise Arc -- Sub-Slice 2: Recognition Action Dispatch

**Lane:** `world-trueborn-rise` (existing lane)

**Branch:** `codex/unity-world-trueborn-rise-arc-2`

**Context:** Player and AI can formally recognize the Trueborn claim at a legitimacy cost to gain permanent exemption from rise-arc pressure. Wire the player dispatch and the recognition resolution into the existing rise-arc system from P14.

**Browser references:**
- `src/game/core/simulation.js` search `getTruebornRecognitionTerms` (~line 13042): cost structure (legitimacy cost, influence cost) per recognition
- `src/game/core/simulation.js` search `recognizeTruebornClaim` (~line 13069): the recognition action effects -- sets recognized flag, deducts cost, applies standing bonus

**Implement:**
- `PlayerTruebornRecognitionRequestComponent.cs`: one-shot dispatch tag consumed by recognition resolution system
- `TruebornRecognitionResolutionSystem.cs` (ISystem): consume player request; gate chain: faction has legitimacy >= cost, not already recognized; deduct legitimacy and influence; set bit in TruebornRiseArcComponent.RecognizedFactionsBitmask for this faction; apply standing bonus (small renown grant, political event cooldown cleared)
- Wire AI recognition: AIStrategicPressureSystem can push a recognition request when pressure cost-benefit favors recognition (read-only from AI lane boundary -- AI system pushes recognition request component; resolution system consumes it without touching AI paths)
- Add `TryDebugRecognizeTrueborn(factionId)` and `TryDebugGetTruebornRecognitionState(factionId)` to debug surface

**Smoke validator:** 3-phase: recognition request deducts correct legitimacy cost; recognized faction bit is set in bitmask; recognized faction receives no loyalty erosion on subsequent pressure ticks.

---

### PRIORITY 16: Trueborn City Rise Arc -- Sub-Slice 3: Diplomatic Escalation and Late-Stage Ultimatum

**Lane:** `world-trueborn-rise` (existing lane)

**Branch:** `codex/unity-world-trueborn-rise-arc-3`

**Context:** At stage 4 the Trueborn City issues ultimatums to unrecognizing kingdoms: recognize within N in-world days or face doubled pressure. At stage 5 doubled pressure is permanent and the DivinRight victory condition gains an additional qualifier check. This is the end-game pressure ratchet that forces a decision point for every player.

**Browser references:**
- `src/game/core/simulation.js` search `updateTruebornRiseArc` stage 4 and 5 branches: ultimatum timing, doubled pressure values
- `src/game/core/simulation.js` search `TRUEBORN_ULTIMATUM_DEADLINE_IN_WORLD_DAYS`: the deadline duration after stage 4 begins
- `src/game/core/simulation.js` search DivinRight victory condition for Trueborn qualifier: whether recognizing Trueborn blocks or modifies the DivineRight victory path

**Implement:**
- Add `UltimatumDeadlineInWorldDays` (float) and `UltimatumIssuedAtInWorldDays` (float) to `TruebornRiseArcComponent.cs`
- `TruebornEscalationSystem.cs` (ISystem): on stage 4 entry, record ultimatum issued time for each non-recognizing faction; when deadline passes without recognition, double loyalty erosion rate for that faction; at stage 5, apply permanent doubled pressure; check DivineRight qualifier (if any -- document divergence if not present in browser spec)
- Add HUD notification hook: write a NarrativeMessageBridge.Push entry when ultimatum is issued (caller side only; NarrativeBridge is in Claude's lane boundary -- push through existing bridge)
- Add `TryDebugAdvanceTruebornStage(stage)` to debug surface

**Smoke validator:** 3-phase: faction that misses deadline receives doubled loyalty erosion after deadline; faction that recognizes before deadline avoids doubled pressure; stage 5 entry confirms permanent pressure state.

---

### PRIORITY 17: HUD Political State Panels

**Lane:** `hud-legibility` (existing lane)

**Branch:** `codex/unity-hud-political-state-panels`

**Context:** The new deep systems (succession crisis, political events, covenant test, Trueborn rise arc) have no HUD surface yet. Players need to see crisis severity, active political event cooldowns, covenant test progress, and Trueborn stage without hunting through debug commands.

**Browser references:**
- `src/game/core/simulation.js` search the state accessors for each system implemented in P1-P14. The HUD reads current state rather than having its own browser equivalent.

**Implement:**
- `SuccessionCrisisHUDComponent.cs` + `SuccessionCrisisHUDSystem.cs` (ISystem): per-frame, mirror SuccessionCrisisComponent severity and recovery progress for the player faction's HUD badge
- `PoliticalEventsTrayHUDComponent.cs` + `PoliticalEventsTrayHUDSystem.cs` (ISystem): per DualClock day, enumerate active DynastyPoliticalEventComponent events for the player faction; write event type and time-remaining to HUD tray slots (up to 4 simultaneous events)
- `CovenantTestProgressHUDComponent.cs` + `CovenantTestProgressHUDSystem.cs` (ISystem): per DualClock day, mirror CovenantTestStateComponent.TestPhase and qualifying duration vs. threshold for the player faction's HUD progress bar
- `TruebornRiseHUDComponent.cs` + `TruebornRiseHUDSystem.cs` (ISystem): per DualClock day, mirror TruebornRiseArcComponent.CurrentStage, GlobalPressurePerDay, and whether the player faction is recognized
- All HUD components follow the exact same patterns as the existing HUD lane components (RealmConditionHUDComponent is the canonical reference)

**Smoke validator:** 4-phase: succession crisis HUD reflects correct severity; political events tray shows correct count; covenant test HUD shows correct TestPhase; Trueborn HUD shows correct stage.

---

### PRIORITY 18: Player Covenant Test Trigger Dispatch

**Lane:** `faith-covenant-test` (existing lane)

**Branch:** `codex/unity-player-covenant-test-dispatch`

**Context:** The covenant test system (P3) includes `PlayerCovenantTestRequestComponent` as a stub. Wire the actual player-side dispatch: a UI action available when TestPhase == ReadyToTrigger, consumed by the resolution system already built in P3.

**Browser references:**
- `src/game/core/simulation.js` search `performCovenantTestAction`: the exported action function whose gate chain P3 mirrors

**Implement:**
- `PlayerCovenantTestDispatchSystem.cs` (ISystem in faith-covenant-test or player-diplomacy lane): when player faction's CovenantTestStateComponent.TestPhase == ReadyToTrigger and no PlayerCovenantTestRequestComponent already exists, allow player to push the request; add cost display logic (read covenant-specific cost from CovenantTestStateComponent.FaithId)
- Verify `CovenantTestResolutionSystem.cs` from P3 consumes the request correctly end-to-end; fix any wiring gaps
- Add `TryDebugDispatchCovenantTest(factionId)` to debug surface

**Smoke validator:** 2-phase: player dispatch component pushed when conditions met is consumed and resolved by ResolutionSystem; dispatch is blocked when TestPhase != ReadyToTrigger.

---

### PRIORITY 19: Contested Territory Pressure System

**Lane:** `world-contested-territory` (new lane -- claim it)

**Branch:** `codex/unity-contested-territory-pressure`

**Context:** Control points simultaneously within claim range of two or more hostile factions should enter a contested state that degrades stability and inflates loyalty volatility. Without this, border territories feel static -- holding the frontier line has no active cost.

**Browser references:**
- `src/game/core/simulation.js` search `CONTESTED_TERRITORY_STABILITY_PENALTY` or `getContestScore` or `updateContestedTerritories` (~lines 9100-9200 approximate): the contest scoring logic and stability penalty values. If no direct match, search `isContested` or `contestedTerritory`
- Document any divergence from browser spec if the function is absent (implement from design canon)

**Implement:**
- `ContestedTerritoryComponent.cs`: per-control-point -- `ContestingFactionCount` (byte), `StabilityPenaltyPerDay` (float), `LoyaltyVolatilityMultiplier` (float), `ContestStartedAtInWorldDays` (float)
- `ContestedTerritoryEvaluationSystem.cs` (ISystem): per DualClock day, for each control point, count how many hostile factions have units within claim radius; if >= 2, write ContestScore and penalty values; remove component when contested state ends
- Wire `StabilityPenaltyPerDay` into RealmConditionCycleSystem stabilization reads (subtractive)
- Wire `LoyaltyVolatilityMultiplier` into loyalty update logic
- Add `TryDebugGetContestState(controlPointId)` to debug surface

**Smoke validator:** 3-phase: control point flanked by 2+ hostile factions receives contested component with non-zero penalty; stability of contested control point degrades faster than uncontested peer; contested state clears when hostile forces withdraw.

---

### PRIORITY 20: Player Succession Influence Action

**Lane:** `player-diplomacy` (existing lane)

**Branch:** `codex/unity-player-succession-influence`

**Context:** Before a head-of-bloodline death, the player can invest legitimacy and gold to designate a preferred heir, overriding the default succession order. This makes succession planning an active strategic layer rather than an automatic outcome.

**Browser references:**
- `src/game/core/simulation.js` search `PlayerSuccessionPreference`, `appointHeir`, or `getSuccessionPreferenceState` (~lines 4600-4660 approximate): the preference dispatch and state shape. If no direct match, search `preferredHeirId` or `successionOverride`
- Document any divergence from browser spec (implement from design canon if absent)

**Implement:**
- `SuccessionPreferenceComponent.cs`: per-faction -- `PreferredHeirMemberId` (Entity), `DesignationCostPaid` (bool), `DesignationExpiresAtInWorldDays` (float)
- `PlayerSuccessionPreferenceRequestComponent.cs`: one-shot dispatch tag -- `TargetMemberId` (Entity)
- `SuccessionPreferenceResolutionSystem.cs` (ISystem): consume request; gate chain: target member is alive, eligible for succession, player faction has legitimacy >= cost; deduct cost; write SuccessionPreferenceComponent; clear on head-of-bloodline death after consumed
- Wire into `DynastySuccessionSystem.cs`: when succession fires, check for SuccessionPreferenceComponent; if present and preferred member is eligible, select them
- Add `TryDebugSetSuccessionPreference(factionId, memberId)` to debug surface

**Smoke validator:** 3-phase: preference designation deducts legitimacy cost and writes component; succession with valid preference selects the preferred heir; succession with invalid preference (member dead) falls back to default order.

---

### PRIORITY 21: Siege Escalation Arc (Prolonged Siege Effects)

**Lane:** `fortification-postures` (existing lane, or claim `siege-escalation` as sub-lane)

**Branch:** `codex/unity-siege-escalation-arc`

**Context:** Sieges that run beyond N in-world days should apply escalating consequences: starvation multiplier increases, unit morale degrades, and desertion chance rises. This makes prolonged sieges a risk on both sides -- the besieger must commit; the defender faces attrition.

**Browser references:**
- `src/game/core/simulation.js` search `SIEGE_PROLONGED_DURATION_IN_WORLD_DAYS` or `tickSiegeEscalation` or `getSiegeEscalationProfile` (~lines 11700-11800 approximate): escalation duration thresholds and multiplier values
- Document any divergence from browser spec if the function is absent

**Implement:**
- `SiegeEscalationComponent.cs`: per-siege-entity or per-settlement under siege -- `SiegeDurationInWorldDays` (float), `EscalationTier` (byte: 0=Normal, 1=Prolonged, 2=Severe, 3=Critical), `StarvationMultiplier` (float), `DesertionThresholdPct` (float), `MoralePenaltyPerDay` (float)
- `SiegeEscalationSystem.cs` (ISystem): per DualClock day, for each active siege, increment duration; at thresholds, advance EscalationTier and update multipliers; wire StarvationMultiplier into StarvationResponseSystem; apply MoralePenaltyPerDay to garrison units
- Add `TryDebugGetSiegeEscalation(settlementId)` to debug surface

**Smoke validator:** 3-phase: siege at normal duration has tier 0 multipliers; siege past first threshold advances to tier 1 with higher starvation multiplier; starvation system applies the escalation multiplier correctly.

---

### PRIORITY 22: Cross-Match Dynasty Progression Foundation

**Lane:** `dynasty-progression` (new lane -- claim it)

**Branch:** `codex/unity-dynasty-progression-foundation`

**Context:** The design specifies a cross-match dynasty progression system: top-performing dynasties accrue XP that unlocks tiers, granting sideways customization bonuses (special unit swaps, etc.) so non-#1 placements stay rewarding. This is the data structure and accumulation foundation. Unlock effects land in a follow-up slice.

**Browser references:**
- `src/game/core/simulation.js` search `getDynastyProgressionState`, `awardDynastyXP`, `DYNASTY_PROGRESSION_TIER_THRESHOLDS`, or `dynProgression` (~lines 14000+ approximate if present): XP accumulation curve, tier thresholds, and unlock slot structure
- If not present in browser spec, implement from the owner direction document at `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md` which canonically specifies this system

**Implement:**
- `DynastyProgressionComponent.cs`: per-player-dynasty singleton -- `TotalXP` (float), `CurrentTier` (byte 0-5), `XPToNextTier` (float), `UnlockedBonusesBitmask` (uint, up to 32 bonus slots), `LastAwardedMatchResultHash` (ulong, deduplication guard)
- `DynastyXPAwardSystem.cs` (ISystem): runs once on VictoryConfirmedComponent presence; compute XP award based on: victory condition achieved (highest XP), territory held at match end (% of map), renown rank (top-3 bonus), match duration (diminishing returns); write to DynastyProgressionComponent; advance tier if XP crosses threshold; flag newly unlocked bonus slot
- `TierUnlockNotificationComponent.cs`: one-shot per tier unlock -- consumed by HUD to surface the unlock event
- Add `TryDebugGetDynastyProgression()` and `TryDebugAwardXP(amount)` to debug surface

**Smoke validator:** 3-phase: XP awarded on victory confirmation matches expected formula; tier advances when XP crosses threshold; duplicate award for same match is blocked by hash guard.

---

### PRIORITY 23: Match-End Sequence and Postgame Report

**Lane:** `match-end-sequence` (new lane -- claim it)

**Branch:** `codex/unity-match-end-sequence`

**Context:** Victory conditions write their confirmed state but there is no system that generates a postgame snapshot for UI display. The postgame report feeds the dynasty XP calculation (P22) and future UI work.

**Browser references:**
- `src/game/core/simulation.js` search `generateMatchReport`, `getMatchSummary`, or `buildPostgameReport` (~lines 14500+ approximate if present): the match-summary state shape and stat collection
- If absent, implement from design canon: territory count, armies destroyed, renown earned, match duration, victory condition type

**Implement:**
- `PostgameReportComponent.cs`: singleton -- `VictoryConditionType` (byte), `WinningFactionId` (int), `MatchDurationInWorldDays` (float), `TerritoriesHeld` (int), `ArmiesDestroyed` (int), `RenownEarned` (float), `BuildingsConstructed` (int), `GeneratedAtInWorldDays` (float)
- `PostgameReportGenerationSystem.cs` (ISystem): triggers on VictoryConfirmedComponent presence; collects stats from FactionComponent, ControlPointComponent, and CombatStatsComponent snapshots; writes PostgameReportComponent singleton; fires once and disables itself
- Wire into `DynastyXPAwardSystem.cs` from P22: XP system reads PostgameReportComponent as input rather than querying stats directly
- Add `TryDebugGetPostgameReport()` to debug surface

**Smoke validator:** 2-phase: report component generated on victory confirmed with correct winning faction and victory type; stats fields (territories, renown, duration) are non-zero and within plausible range for a simulated match.

---

### PRIORITY 24: Faith Doctrine Combat Bonus Wiring

**Lane:** `faith-combat-doctrine` (new lane -- claim it)

**Branch:** `codex/unity-faith-doctrine-combat-wiring`

**Context:** Faith commitment systems are on master (FaithCommitmentIntensitySystem, FaithComponent). Doctrine-specific combat bonuses (attack, defense, morale, aura) are not yet wired into AttackResolutionSystem and CommanderAuraSystem. This creates visible, differentiated gameplay between Blood Dominion (aggressive bonuses), The Order (defensive bonuses), and The Wild (nature unit and healing bonuses).

**Browser references:**
- `src/game/core/simulation.js` search `getFaithDoctrineCombatBonus` or `getFaithDoctrineAuraModifier`: doctrine-specific attack, defense, morale, aura radius bonuses by faith covenant type
- `src/game/core/simulation.js` search `BLOOD_DOMINION_ATTACK_BONUS`, `THE_ORDER_DEFENSE_BONUS`, `THE_WILD_HEALING_BONUS` or similar constants: per-doctrine combat multiplier values

**Implement:**
- `FaithDoctrineModifierComponent.cs` (or add fields to FaithComponent if already on the right entity): per-faction -- `AttackBonus` (float), `DefenseBonus` (float), `MoraleBonus` (float), `AuraRadiusBonus` (float), `HealBonusMultiplier` (float) -- written by FaithDoctrineModifierSystem
- `FaithDoctrineModifierSystem.cs` (ISystem): per DualClock day, read faction's committed faith covenant type and intensity; look up doctrine combat profile; write modifier component
- Wire `AttackBonus` and `DefenseBonus` into `AttackResolutionSystem` (additive/multiplicative per browser spec pattern)
- Wire `AuraRadiusBonus` into `CommanderAuraSystem` from P5 (extends commander aura radius for Wild-faith factions)
- Wire `HealBonusMultiplier` into `FortificationReserveSystem` healing rate (The Wild grants enhanced healing)
- Add `TryDebugGetFaithDoctrineModifiers(factionId)` to debug surface

**Smoke validator:** 4-phase: Blood Dominion faction receives positive attack bonus; The Order faction receives positive defense bonus; The Wild faction receives positive heal bonus; bonuses scale with faith intensity (higher intensity = stronger bonus).

---

### PRIORITY 25: Renown Decay and Dynasty Prestige Drift

**Lane:** `dynasty-succession-crisis` or `dynasty-prestige` (new sub-lane -- claim it)

**Branch:** `codex/unity-dynasty-renown-decay-prestige`

**Context:** DynastyRenownComponent and DynastyRenownAccumulationSystem are on master. Two missing pieces: (1) passive renown decay for inactive dynasties (no combat dispatch, no diplomatic action for N days), and (2) prestige drift for dynasties that sustain top-3 renown standing -- a multiplier that makes high-renown dynasties more visible in the leaderboard and more attractive as marriage targets.

**Browser references:**
- `src/game/core/simulation.js` search `DYNASTY_RENOWN_DECAY_PER_DAY` or `getRenownDecayRate` (~lines 3050-3100 approximate): the per-day decay rate and inactivity threshold
- `src/game/core/simulation.js` search `getDynastyPrestigeModifier` or `PRESTIGE_DRIFT_RATE` (~lines 3110-3130 approximate): how prestige accumulates and what it affects
- Document any divergence from browser spec if absent

**Implement:**
- `DynastyInactivityTrackerComponent.cs`: per-faction -- `LastActiveInWorldDays` (float), updated whenever a combat, diplomacy, or covert-op dispatch occurs
- `RenownDecaySystem.cs` (ISystem): per DualClock day, for each dynasty that has been inactive for > threshold days, apply passive renown decay (small fraction of current renown, clamped to a floor of 10); skip factions with active combat or recent dispatch
- `DynastyPrestigeComponent.cs`: per-faction -- `PrestigeTier` (byte 0-3), `ConsecutiveDaysInTop3` (float), `PrestigeMultiplier` (float)
- `PrestigeDriftSystem.cs` (ISystem): per DualClock day, rank all factions by current renown; for top-3 factions, increment ConsecutiveDaysInTop3; advance PrestigeTier at thresholds; apply PrestigeMultiplier to marriage desirability evaluation and renown leaderboard display weight
- Wire `LastActiveInWorldDays` update into all relevant dispatch systems (AssassinationDispatchSystem, PlayerMarriageProposalSystem, etc.)
- Add `TryDebugGetPrestigeState(factionId)` and `TryDebugSimulateInactivity(factionId, days)` to debug surface

**Smoke validator:** 4-phase: dynasty with no dispatch for N days accumulates renown decay; decay is blocked for recently active faction; top-3 dynasty advances prestige tier at correct consecutive-day threshold; prestige multiplier is present and non-unity on qualifying dynasty.

---

## NON-NEGOTIABLE PROCESS RULES

1. **Read `docs/unity/CONCURRENT_SESSION_CONTRACT.md` before every session.** Do not touch any file outside your claimed lane's owned paths without a cross-lane read justification documented in the handoff.

2. **Every sub-slice must pass all 10 canonical validation gates**, run serially (Unity holds a project lock -- never parallelize):
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

3. **Write a per-slice handoff** at `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<slice>.md`. Include: goal, browser reference search terms used (or divergence note), work completed, validation proof lines (quote actual output), deferred simplifications, exact next action.

4. **Update `docs/unity/CONCURRENT_SESSION_CONTRACT.md` after every sub-slice**: bump Revision by 1, set Last Updated to today (YYYY-MM-DD), set Last Updated By to `codex-<lane>-YYYY-MM-DD`, add or update the lane subsection with new owned paths and handoff reference.

5. **Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/PROJECT_STATE.json` after every sub-slice.** Append only -- never overwrite or touch another lane's entries.

6. **Create a new branch per sub-slice** following the pattern `codex/unity-<lane>-<description>`. Check out from `origin/master`, not from a prior working branch.

7. **Use the wrapper lock** for all Unity batch invocations: `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-<identifier>`. Check for the lock, reclaim if > 15 minutes old, delete after completion.

8. **Push branches to origin after validation passes.** Merge to master with `git merge --no-ff <branch> -m "Merge <branch>: <one-line description>"`. Run all 10 gates again on master after merging.

9. **Do not modify browser runtime files** (`src/`, `data/`, `tests/`, `play.html`). Read-only behavioral specification.

10. **Follow Unity 6.3 DOTS/ECS idioms:** ISystem (not MonoBehaviour), SystemAPI, EntityCommandBuffer, NativeArray, `[BurstCompile]` on hot paths.

11. **Write a dedicated smoke validator** for every new system in `unity/Assets/_Bloodlines/Code/Editor/` with matching PowerShell wrapper in `scripts/`.

12. **Cross-reference browser simulation.js search terms** in every handoff. If the browser has no equivalent, implement from design canon and document the divergence explicitly.

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

After 4-5 days:
- P1 Succession Crisis System on master
- P2 Political Event System on master
- P3 Covenant Test Execution on master (most gameplay-critical item)
- P4 Governor Specialization on master
- P5 Commander Aura System on master
- P6 Imminent Engagement Postures on master
- P7 Verdant Warden Faith Unit Support on master
- P8 Sacred Site Exposure Walker + Wayshrine on master
- P9 Faith Structure Intensity Regen on master
- P10 Captive Ransom Trickle on master
- P11 Covert Ops Full Resolution Effects on master
- P12 Governance Coalition Pressure on master
- P13 Minor House Levy complete and validated on master
- P14 Trueborn Rise Arc Sub-Slice 1 on master
- P15 Trueborn Rise Arc Sub-Slice 2 (Recognition Dispatch) on master
- P16 Trueborn Rise Arc Sub-Slice 3 (Diplomatic Escalation) on master
- P17 HUD Political State Panels on master
- P18 Player Covenant Test Trigger Dispatch on master
- P19 Contested Territory Pressure on master
- P20 Player Succession Influence Action on master
- P21 Siege Escalation Arc on master
- P22 Cross-Match Dynasty Progression Foundation on master
- P23 Match-End Sequence and Postgame Report on master
- P24 Faith Doctrine Combat Bonus Wiring on master
- P25 Renown Decay and Dynasty Prestige Drift on master
- Contract file advanced at least 25 revisions from current
- No regressions in any existing smoke validators
