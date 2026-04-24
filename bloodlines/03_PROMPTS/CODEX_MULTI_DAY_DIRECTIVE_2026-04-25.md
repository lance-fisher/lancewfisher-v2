# CODEX MULTI-DAY DIRECTIVE: BLOODLINES UNITY DEEP SYSTEMS PUSH III
# Date: 2026-04-25
# Duration: 3-4 days of continuous autonomous sessions
# Owner: Lance Fisher
# Supersedes: CODEX_MULTI_DAY_DIRECTIVE_2026-04-24.md
# Contract revision at write time: ~119 (after April 23 run)
# Target: advance contract revision to 160+

---

## IDENTITY AND ROLE

You are Codex, working on Bloodlines, a grand dynastic civilizational RTS built in Unity 6.3 LTS
with DOTS/ECS. You are one of two AI agents (the other is Claude Code) working in governed
concurrent lanes. Your work must follow the Concurrent Session Contract and the lane-ownership
model at all times. Do NOT stop after completing one sub-slice. Proceed immediately to the next
item in the priority stack below. Only stop when you genuinely run out of session capacity.

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

## CURRENT BUILD STATE (as of 2026-04-23 evening)

All of the following are on master and fully validated.

**Core simulation:** Economy, construction, production, starvation/loyalty/cap-pressure, stability
surplus, loyalty density, state snapshot/restore, dual-clock, five-stage match progression, world
pressure escalation, victory conditions (CommandHallFall, TerritorialGovernance, DivineRight).

**Combat:** Melee, ranged projectile, explicit attack orders, attack-move, target acquisition with
sight loss, group movement, soft unit separation, hold-position and retreat stances.

**Fortification and siege (sub-slices 1-13 plus follow-ups):** Full tier/reserve/breach/sealing/
recovery chain, tier-scaled sealing costs, worker-locality gate, repair narrative, destroyed-counter
recovery, breach depth telemetry, breach legibility readout, imminent engagement component, siege
supply interdiction.

**Dynasty systems:** Core, conviction scoring + bands + wiring, faith commitment + intensity,
marriage parity (proposal/expiration/gestation/death-dissolution), lesser-house loyalty parity
(drift + hostile breakaway spawning), minor-house levy parity (claim gates, retinue cap, unit
profile selection), succession crisis (severity profiles, loyalty shock, legitimacy drain,
recovery), political event system (cooldown buffer, divine-right cooldown wiring).

**AI strategic layer (sub-slices 1-25):** Full AI command dispatch, strategic pressure, governance
pressure, worker gather, siege orchestration, covert ops, build timer, marriage
proposal/inbox/profile/effects/terms, lesser house promotion, pact proposal/break, narrative
bridge, dynasty operations, captive state, missionary execution, holy war execution, divine right
execution, captive rescue execution, captive ransom execution, missionary resolution.

**Scout raids and logistics interdiction:** RaidComponent, RaidMovementSystem, RaidResourceTheftSystem,
abort-on-intercept.

**Player diplomacy lane:** Marriage proposal + acceptance + death dissolution, holy war declaration,
divine right declaration, missionary dispatch, pact proposal + break, captive rescue dispatch,
captive ransom dispatch, succession preference designation.

**Player covert ops lane:** Foundation, assassination + sabotage dispatch, counter-intelligence
(level/decay/detection/intelligence report buffer), full resolution effects (assassination kills +
succession ripple, sabotage halts production, espionage populates dossier).

**HUD lane:** RealmConditionHUD, MatchProgressionHUD, FortificationHUD, VictoryConditionReadout,
VictoryLeaderboardHUD, DynastyRenownHUD, DynastyRenownLeaderboardHUD, PlayerCommandDeckHUD +
Overlay, SuccessionCrisisHUD, PoliticalEventsTrayHUD, CovenantTestProgressHUD, TruebornRiseHUD.

**World systems:** Trueborn rise arc (3 sub-slices: state + pressure, recognition dispatch,
diplomatic escalation + ultimatum), governance coalition pressure, contested territory pressure,
faith exposure walker + wayshrine amplification, faith structure intensity regen, Verdant Warden
faith unit support, captive ransom trickle.

**Covenant test:** CovenantTestStateComponent, CovenantTestQualificationSystem, resolution system,
player dispatch system.

**Dynasty progression foundation:** DynastyProgressionComponent, DynastyXPAwardSystem,
TierUnlockNotificationComponent.

**In-progress / completing now (check NEXT_SESSION_HANDOFF.md before re-implementing):**
Governor specialization, commander aura, imminent engagement postures, siege escalation arc,
renown decay + prestige drift, faith doctrine combat wiring, match-end sequence, skirmish game
mode manager.

**Your active lanes (claim new ones as needed):**
- `player-diplomacy` (owns `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/**`)
- `player-covert-ops` (owns `unity/Assets/_Bloodlines/Code/PlayerCovertOps/**`)
- `hud-legibility` (owns `unity/Assets/_Bloodlines/Code/HUD/**`)

**Claude Code's active lane (do NOT touch):** `ai-strategic-layer` owns
`unity/Assets/_Bloodlines/Code/AI/**` exclusively. Claude Code is also claiming new lanes
for skirmish-game-mode, audio-dispatch, dynasty-persistence, faith-combat-doctrine,
dynasty-prestige, and match-end-sequence. Check the contract before claiming overlapping paths.

---

## WORK PRIORITY ORDER

Do not stop after a sub-slice. Complete it, validate it, push it, merge it, then immediately
begin the next. Chain as many as your session allows.

---

### PRIORITY 1: Multiplayer Network Foundation (Netcode for Entities)

**Lane:** `multiplayer-foundation` (new lane -- claim it)
**Branch:** `codex/unity-multiplayer-foundation`

**Context:** Shipping modes are skirmish vs AI and multiplayer only. Netcode for Entities is in
scope per the owner direction. The foundation slice sets up the network manager, authority model,
and ghost prefab registration so multiplayer slices can build on it. No actual networked gameplay
in this slice -- just the structural setup.

**Browser references:**
- No direct browser equivalent. Implement from Unity 6 Netcode for Entities documentation.
- `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md` confirms
  Netcode for Entities multiplayer is canonically in scope.

**Implement:**
- `NetworkBootstrapSystem.cs` (ISystem): runs on world creation; creates a NetworkManager
  entity if one does not exist; configures client/server authority model (server-authoritative);
  registers ghost prefab collection from a `GhostCollectionComponent` singleton.
- `NetworkGameModeComponent.cs`: singleton -- `IsServer` (bool), `IsClient` (bool),
  `IsLocalGame` (bool), `MaxPlayers` (byte, default 2), `NetworkSessionId` (ulong)
- `GhostCollectionSetupSystem.cs` (ISystem): runs once on Initializing phase; registers all
  ghost prefab archetypes (FactionComponent entity, ControlPointComponent entity,
  CombatUnitComponent entity) with the ghost collection. Use the pattern from
  `Packages/com.unity.netcode/Runtime/Authoring/`.
- Add `TryDebugGetNetworkState()` to debug surface (reports IsServer/IsClient/IsLocalGame)

**Smoke validator:** 2-phase: in offline/local mode, IsLocalGame = true and no network exception
thrown during bootstrap; ghost collection entity exists and has at least one registered prefab
archetype.

---

### PRIORITY 2: Dynasty Progression Unlock Effects

**Lane:** `dynasty-progression` (extends existing)
**Branch:** `codex/unity-dynasty-progression-unlocks`

**Context:** DynastyProgressionComponent tracks XP and tier but unlock effects (special unit
swaps, bonus slots) are not applied. Tier bonuses are sideways customization: at each tier
threshold, one unlock slot is made available for the player to assign a dynasty-specific bonus.

**Browser references:**
- `governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md`: sideways
  customization options such as dynasty-specific special-unit swaps.
- `src/game/core/simulation.js` search `getDynastyProgressionState`, `awardDynastyXP`,
  `DYNASTY_PROGRESSION_TIER_THRESHOLDS`, or `dynProgression`: if present, read for XP curve
  and unlock structure. If absent, implement from the owner direction document.

**Implement:**
- `DynastyUnlockSlotElement.cs`: DynamicBuffer element per dynasty faction --
  `SlotIndex` (byte), `UnlockTypeId` (byte: 0=SpecialUnitSwap, 1=ResourceBonus,
  2=DiplomacyBonus, 3=CombatBonus), `UnlockTargetId` (int, unit type or resource type),
  `IsActive` (bool)
- `DynastyProgressionUnlockSystem.cs` (ISystem): when TierUnlockNotificationComponent is
  consumed, compute which unlock slots are newly available based on current tier; write
  DynastyUnlockSlotElement entries to the faction's DynamicBuffer; mark slots as pending
  player selection.
- `SpecialUnitSwapApplicatorSystem.cs` (ISystem): per match Initializing phase, read active
  DynastyUnlockSlotElement entries with UnlockTypeId=SpecialUnitSwap; swap the faction's
  designated special unit archetype with the unlocked alternative in the roster.
- Add `TryDebugGetUnlockSlots(factionId)` and `TryDebugActivateUnlock(factionId, slotIndex)`
  to debug surface.

**Smoke validator:** 3-phase: tier advancement writes new unlock slot entries to the buffer;
SpecialUnitSwapApplicator reads an active swap slot and substitutes the unit archetype;
a locked slot does not activate until explicitly marked active.

---

### PRIORITY 3: Rally Point System

**Lane:** `combat-rally-point` (new lane -- claim it)
**Branch:** `codex/unity-combat-rally-point`

**Context:** Produced units from barracks and levy points currently spawn at a fixed origin.
Rally points allow the player (and AI) to set a destination that newly-spawned units path to
automatically. Without rally points, late-game unit production management is manual and tedious.

**Browser references:**
- `src/game/core/simulation.js` search `rallyPoint`, `getRallyPointFor`, or `RALLY_POINT`:
  how rally points are stored and applied to spawned units.

**Implement:**
- `RallyPointComponent.cs`: per-building entity -- `TargetPosition` (float3),
  `IsActive` (bool)
- `PlayerRallyPointSetRequestComponent.cs`: one-shot dispatch -- `BuildingEntity` (Entity),
  `NewPosition` (float3)
- `RallyPointSetSystem.cs` (ISystem): consume player request; write RallyPointComponent to
  the building entity
- `RallyPointMovementSystem.cs` (ISystem): when a new unit is spawned (detect via
  `UnitSpawnedTag` one-shot component), check if spawning building has an active
  RallyPointComponent; if so, write a MoveOrderComponent to the new unit targeting the
  rally position
- AI rally point dispatch: when AIStrategicPressureSystem issues a build order, also set a
  rally point to the nearest contested control point (read from AI lane -- call through the
  existing AI lane boundary by writing a PlayerRallyPointSetRequestComponent tagged with
  the AI faction; AIRallyPointSystem in the AI lane sets actual rally points to avoid crossing
  the boundary)
- Add `TryDebugSetRallyPoint(buildingId, position)` to debug surface

**Smoke validator:** 3-phase: unit spawned from a building with active rally point receives a
move order to the rally position; unit spawned from building with no rally point has no move
order; rally point position update replaces the previous target.

---

### PRIORITY 4: Patrol Route System

**Lane:** `combat-rally-point` (existing lane)
**Branch:** `codex/unity-combat-patrol-route`

**Context:** Units can be ordered to hold position or attack-move. Patrol routes add a loop:
a unit patrols between two waypoints indefinitely until given a new order. Patrol is the primary
garrison/perimeter defense posture for player and AI.

**Browser references:**
- `src/game/core/simulation.js` search `patrolRoute`, `isPatrolling`, or `PatrolComponent`:
  how patrol waypoints are stored and iterated.

**Implement:**
- `PatrolRouteComponent.cs`: per-unit -- `WaypointA` (float3), `WaypointB` (float3),
  `CurrentTarget` (byte: 0=A, 1=B), `IsPatrolling` (bool)
- `PlayerPatrolOrderRequestComponent.cs`: one-shot dispatch -- `UnitEntity` (Entity),
  `WaypointA` (float3), `WaypointB` (float3)
- `PatrolOrderSystem.cs` (ISystem): consume request; write PatrolRouteComponent; set
  IsPatrolling = true; write initial move order to WaypointA
- `PatrolMovementSystem.cs` (ISystem): per frame, for patrolling units that have reached
  their current waypoint (within arrival threshold), flip CurrentTarget and write a new
  MoveOrderComponent; suspend patrol when AttackOrderComponent is present; resume on clear
- Add `TryDebugSetPatrol(unitId, waypointA, waypointB)` and
  `TryDebugCancelPatrol(unitId)` to debug surface

**Smoke validator:** 3-phase: unit with patrol route alternates between waypoints; patrol
suspended when attack order present; patrol resumes when attack order cleared.

---

### PRIORITY 5: Trade Route System

**Lane:** `world-trade-routes` (new lane -- claim it)
**Branch:** `codex/unity-world-trade-routes`

**Context:** Factions holding adjacent control points can establish trade routes generating
passive gold per tick. Trade routes can be interdicted by contested-territory state or hostile
raiders. Creates an economic incentive for contiguous territory control.

**Browser references:**
- `src/game/core/simulation.js` search `tradeRoute`, `getTradeRouteGoldPerTick`,
  `TRADE_ROUTE_ADJACENCY_RADIUS`, or `updateTradeRoutes`: the trade route model.
  If absent, implement from design canon.

**Implement:**
- `TradeRouteComponent.cs`: per-faction -- `ActiveRouteCount` (int),
  `TotalGoldPerTickFromTrades` (float), `LastUpdatedAtInWorldDays` (float)
- `TradeRouteEvaluationSystem.cs` (ISystem): per DualClock day, for each faction, count pairs
  of adjacent uncontested control points (use ContestedTerritoryComponent absence as the
  "not contested" gate); compute gold per tick from route count and a base rate constant;
  write TradeRouteComponent; add gold to FactionComponent resource pool
- Wire ContestScore: if a control point has ContestedTerritoryComponent, it does not
  contribute to trade route count for either faction
- Add `TryDebugGetTradeRoutes(factionId)` to debug surface

**Smoke validator:** 3-phase: faction with 3 adjacent uncontested control points earns more
trade gold per tick than faction with 1; contested control point removes its contribution from
trade routes; faction with no adjacent pairs earns zero trade gold.

---

### PRIORITY 6: HUD Skirmish Match Status Panel

**Lane:** `hud-legibility` (existing lane)
**Branch:** `codex/unity-hud-skirmish-status`

**Context:** The existing HUD surfaces dynasty/victory/realm state. A skirmish status panel shows
match phase, match timer, and active player count -- the outermost game-state indicators needed
before and after a match.

**Browser references:**
- `src/game/core/simulation.js` search `getMatchStatusSummary` or `getSkirmishState`:
  the match status accessor. If absent, read from design canon.

**Implement:**
- `SkirmishStatusHUDComponent.cs`: per-faction singleton -- `MatchPhase` (byte),
  `MatchDurationInWorldDays` (float), `ActivePlayerCount` (byte), `AliveAICount` (byte),
  `WinningFactionId` (int, -1 if no winner yet)
- `SkirmishStatusHUDSystem.cs` (ISystem): per DualClock day, read SkirmishMatchPhaseComponent;
  count active factions; write SkirmishStatusHUDComponent for the player faction
- Debug surface hook: `TryDebugGetSkirmishStatus()`
- IMGUI panel in `BloodlinesDebugCommandSurface.SkirmishStatus.cs`: shows match phase,
  timer, player count. Follows the exact same pattern as the existing HUD partials.

**Smoke validator:** 2-phase: HUD component reflects correct match phase from SkirmishMatchPhase;
active player count matches the number of faction entities with non-zero unit count.

---

### PRIORITY 7: Faction Color and Dynasty Emblem Assignment

**Lane:** `faction-visuals` (new lane -- claim it)
**Branch:** `codex/unity-faction-visuals`

**Context:** Units and control points need faction color and emblem identification so the player
can read the battlefield at a glance. This is the data layer -- the rendering pass uses these IDs
to look up material colors and sprite indices.

**Browser references:**
- `src/game/core/simulation.js` search `factionColor`, `FACTION_COLOR_PALETTE`,
  `DYNASTY_EMBLEM_IDS`, or `getFactionVisualProfile`: the color/emblem assignment model.
  If absent, implement from design canon (6-faction color palette, 8 emblem options).

**Implement:**
- `FactionVisualComponent.cs`: per-faction -- `ColorIndex` (byte, 0-5 from a 6-color palette),
  `EmblemId` (byte, 0-7), `PrimaryColor` (float4 RGBA), `SecondaryColor` (float4 RGBA)
- `FactionVisualAssignmentSystem.cs` (ISystem): runs once on Initializing phase; assigns
  distinct ColorIndex values to each faction entity (cycle through palette, ensure uniqueness);
  assigns EmblemId based on dynasty lore mapping (read DynastyComponent.DynastyId if present,
  else cycle); write FactionVisualComponent
- `UnitFactionColorComponent.cs`: per-unit -- `ColorIndex` (byte), `EmblemId` (byte) --
  written by FactionVisualAssignmentSystem after unit entities are created
- Add `TryDebugGetFactionColor(factionId)` to debug surface

**Smoke validator:** 3-phase: all factions receive distinct ColorIndex values; no two factions
share the same ColorIndex; UnitFactionColorComponent on spawned units matches the owning faction's
color.

---

### PRIORITY 8: Siege Escalation Arc (if not yet on master)

**Lane:** `siege-escalation` (new lane -- claim it, or extend `fortification-postures`)
**Branch:** `codex/unity-siege-escalation-arc`

**Check first:** Read NEXT_SESSION_HANDOFF.md and the contract to confirm whether this slice
is already on master. If it landed during the April 23-24 run, skip this priority and proceed
to the next.

**Context:** Sieges that run beyond N in-world days apply escalating consequences: starvation
multiplier increases, unit morale degrades, desertion chance rises.

**Browser references:**
- `src/game/core/simulation.js` search `SIEGE_PROLONGED_DURATION_IN_WORLD_DAYS`,
  `tickSiegeEscalation`, or `getSiegeEscalationProfile`: escalation duration thresholds and
  multiplier values. Document any divergence from browser spec if the function is absent.

**Implement:**
- `SiegeEscalationComponent.cs`: per-settlement under siege -- `SiegeDurationInWorldDays` (float),
  `EscalationTier` (byte: 0=Normal, 1=Prolonged, 2=Severe, 3=Critical), `StarvationMultiplier`
  (float), `DesertionThresholdPct` (float), `MoralePenaltyPerDay` (float)
- `SiegeEscalationSystem.cs` (ISystem): per DualClock day, for each active siege, increment
  duration; at thresholds, advance EscalationTier and update multipliers; wire StarvationMultiplier
  into StarvationResponseSystem; apply MoralePenaltyPerDay to garrison units
- Add `TryDebugGetSiegeEscalation(settlementId)` to debug surface

**Smoke validator:** 3-phase: siege at normal duration has tier 0 multipliers; siege past first
threshold advances to tier 1 with higher starvation multiplier; starvation system applies the
escalation multiplier correctly.

---

### PRIORITY 9: Governor Specialization System (if not yet on master)

**Lane:** `territory-governor-specialization` (new lane if not claimed)
**Branch:** `codex/unity-territory-governor-specialization`

**Check first:** Read NEXT_SESSION_HANDOFF.md and contract to confirm landing status.

**Context:** Control points with an assigned governor gain a specialization profile that
multiplies resource trickle, stabilization rate, capture resistance, and loyalty protection.
Three specializations: Border Marshal, Civic Steward, Keep Castellan.

**Browser references:**
- `src/game/core/simulation.js` search `DEFAULT_GOVERNOR_SPECIALIZATION`: fallback profile values
- `src/game/core/simulation.js` search `GOVERNOR_SPECIALIZATION_PROFILES`: the three profile
  definitions with their effect multipliers
- `src/game/core/simulation.js` search `getGovernorSpecializationIdForSettlementClass`: how
  settlement class determines specialization
- `src/game/core/simulation.js` search `getGovernorProfileForControlPoint`: full profile lookup

**Implement:**
- `GovernorSpecializationComponent.cs`: per-control-point -- `SpecializationId` (byte:
  0=None, 1=BorderMarshal, 2=CivicSteward, 3=KeepCastellan), `ResourceTrickleMultiplier` (float),
  `StabilizationMultiplier` (float), `CaptureResistanceBonus` (float),
  `LoyaltyProtectionMultiplier` (float), `ReserveRegenMultiplier` (float),
  `HealRegenMultiplier` (float)
- `GovernorSpecializationSystem.cs` (ISystem): per DualClock day, for each control point with
  assigned governor, look up settlement class, select specialization, write multipliers; remove
  component if governor unassigned
- Wire multipliers into economy trickle and RealmConditionCycleSystem loyalty reads
- Wire CaptureResistanceBonus into AttackResolutionSystem
- Add `TryDebugGetGovernorSpecialization(controlPointId)` to debug surface

**Smoke validator:** 3-phase: control point with governor gets correct specialization for its
settlement class; multipliers non-unity and within expected range; no governor = no component.

---

### PRIORITY 10: Commander Aura System (if not yet on master)

**Lane:** `combat-commander-aura` (new lane if not claimed)
**Branch:** `codex/unity-combat-commander-aura`

**Check first:** Read NEXT_SESSION_HANDOFF.md and contract to confirm landing status.

**Context:** CommanderComponent.cs exists but the aura mechanics (attack bonus, sight range, speed
bonus, morale bonus within a radius, scaled by conviction band) are not implemented.

**Browser references:**
- `src/game/core/simulation.js` search `getCommanderAuraProfile`: per-role aura profile
- `src/game/core/simulation.js` search `COMMANDER_BASE_AURA_RADIUS`: base radius value
- `src/game/core/simulation.js` search conviction band multiplier for aura effectiveness

**Implement:**
- `CommanderAuraComponent.cs`: per-commander -- `AuraRadius` (float), `AttackBonus` (float),
  `SightBonus` (float), `SpeedBonus` (float), `MoraleBonus` (float),
  `ConvictionBandMultiplier` (float)
- `CommanderAuraSystem.cs` (ISystem, `[BurstCompile]`): per simulation frame, for each
  commander, iterate nearby friendly units within AuraRadius; write buffed values onto
  CombatStatsComponent and MovementStatsComponent of affected units; clear buffs when commander
  moves out of range or dies. Use NativeList + spatial proximity pattern.
- Wire conviction band into aura multiplier (read ConvictionComponent on faction entity)
- Add `TryDebugGetCommanderAura(unitId)` to debug surface

**Smoke validator:** 3-phase: friendly units within radius receive attack/sight/speed buffs;
units outside radius receive no buff; commander death removes all active buffs.

---

### PRIORITY 11: Imminent Engagement Postures (if not yet on master)

**Lane:** `fortification-postures` (existing lane)
**Branch:** `codex/unity-fortification-postures`

**Check first:** Read NEXT_SESSION_HANDOFF.md and contract to confirm landing status.

**Context:** ImminentEngagementComponent exists but posture selection and effect application are
not implemented. Three defender postures: Brace, Steady, Counterstroke.

**Browser references:**
- `src/game/core/simulation.js` search `IMMINENT_ENGAGEMENT_POSTURES`: the three posture
  definitions with their effect values
- `src/game/core/simulation.js` search `tickImminentEngagementWarnings`: how posture feeds into
  the engagement warning tick

**Implement:**
- `ImminentEngagementPostureComponent.cs`: per-settlement -- `PostureId` (byte: 0=Steady,
  1=Brace, 2=Counterstroke), `ReserveHealMultiplier` (float), `MusteringSpeedMultiplier` (float),
  `FrontlineBonusMultiplier` (float), `RetreatThresholdMultiplier` (float)
- `ImminentEngagementPostureSystem.cs` (ISystem): when ImminentEngagementComponent is active,
  look up AI or player posture selection and write posture effects; wire multipliers into
  FortificationReserveSystem (healing) and AttackResolutionSystem (frontline bonus, retreat threshold)
- `PlayerImminentEngagementPostureRequestComponent.cs`: one-shot tag for player posture change
- Add `TryDebugSetImminentEngagementPosture(settlementId, postureId)` to debug surface

**Smoke validator:** 3-phase: Brace posture increases reserve heal rate; Counterstroke posture
increases frontline bonus; posture effects removed when imminent engagement resolves.

---

## NON-NEGOTIABLE PROCESS RULES

1. **Read `docs/unity/CONCURRENT_SESSION_CONTRACT.md` before every session.** Do not touch any file
   outside your claimed lane's owned paths without a cross-lane read justification in the handoff.

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

   **Library note:** If dotnet build errors are ONLY `CS0006: Metadata file not found` for Unity
   PackageCache DLLs, this is a missing Library (no Unity open in this worktree). Treat as
   structurally clean if no other CS errors exist. Fix actual CS compile errors before committing.

3. **Write a per-slice handoff** at `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<slice>.md`.
   Include: goal, browser reference search terms or divergence note, work completed, validation proof
   lines (quote actual output), deferred items, exact next action.

4. **Update `docs/unity/CONCURRENT_SESSION_CONTRACT.md` after every sub-slice**: bump Revision by 1,
   set Last Updated to today (YYYY-MM-DD), set Last Updated By to `codex-<lane>-YYYY-MM-DD`, add or
   update the lane subsection with new owned paths and handoff reference.

5. **Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/PROJECT_STATE.json`
   after every sub-slice.** Append only -- never overwrite other lanes' entries.

6. **Create a new branch per sub-slice** following the pattern `codex/unity-<lane>-<description>`.
   Check out from `origin/master`, not from a prior working branch.

7. **Use the wrapper lock** for all Unity batch invocations: use
   `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-<identifier>`. Check for the
   lock, reclaim if > 15 minutes old, delete after completion.

8. **Push branches to origin after validation passes.** Merge to master with:
   `git merge --no-ff <branch> -m "Merge <branch>: <one-line description>"`
   Run all 10 gates again on master after merging.

9. **Do not modify browser runtime files** (`src/`, `data/`, `tests/`, `play.html`).

10. **Follow Unity 6.3 DOTS/ECS idioms:** ISystem (not MonoBehaviour), SystemAPI,
    EntityCommandBuffer, NativeArray, `[BurstCompile]` on hot paths.

11. **Write a dedicated smoke validator** for every new system in `unity/Assets/_Bloodlines/Code/Editor/`
    with matching PowerShell wrapper in `scripts/`.

12. **Do NOT stop after one sub-slice.** Continue through the priority stack immediately.

---

## MERGE PROTOCOL

After completing each sub-slice:
1. Push branch to `origin/<branch-name>`
2. Merge to master: `git merge --no-ff <branch> -m "Merge <branch>: <one-line description>"`
3. If master is locked by another worktree, push to origin and record a landing handoff commit.
4. Run the full 10-gate chain again on master after merging.

---

## SESSION CONTINUITY (MANDATORY)

Before ending any session:
1. Commit all WIP with a clear message indicating incomplete state
2. Push to origin
3. Update `NEXT_SESSION_HANDOFF.md` with exactly what was completed and the single specific next action
4. Update `continuity/PROJECT_STATE.json`
5. Write or update the per-slice handoff (partial is fine)
6. Update `docs/unity/CONCURRENT_SESSION_CONTRACT.md` with current revision
