# CODEX MULTI-DAY DIRECTIVE: BLOODLINES UNITY PRODUCTION PUSH
# Date: 2026-04-21
# Duration: Use this prompt for the next 3-5 days of Codex sessions
# Owner: Lance Fisher
# Supersedes: CODEX_MULTI_DAY_DIRECTIVE_2026-04-20.md

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
3. `NEXT_SESSION_HANDOFF.md` -- read the first 100 lines at minimum; the "Last updated" and "Immediate next action" entries tell you exactly where to resume
4. `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- read the ENTIRE file; pay close attention to Active Lanes, owned paths, Shared Files, Forbidden Paths, Validation Gate, and Wrapper Lock
5. `docs/plans/2026-04-17-browser-to-unity-migration-plan.md`
6. `continuity/PROJECT_STATE.json`

Then read the specific browser reference files for whatever system you are about to port.

---

## CURRENT BUILD STATE (as of contract revision 63, 2026-04-21)

All of the following are on master and validated:

**Core simulation:** Economy, construction, production, starvation/loyalty/cap-pressure, stability surplus, loyalty density HUD, state snapshot/restore, dual-clock, five-stage match progression, world pressure escalation, victory conditions (CommandHallFall, TerritorialGovernance, DivinRight).

**Combat:** Melee, ranged projectile, explicit attack orders, attack-move, target acquisition with sight loss, group movement, soft unit separation, hold-position and retreat stances (8 smoke phases).

**Fortification and siege (sub-slices 1-9 plus 3 follow-ups today):** Tier/reserve system, siege support + field water, imminent engagement, siege supply interdiction, wall-segment destruction, breach assault pressure, breach legibility readout, breach sealing recovery, destroyed-counter recovery. Today also: tier-scaled sealing costs, worker-locality sealing gate, fortification repair narrative.

**Dynasty systems:** Core (8-member template, aging, succession, legitimacy, renown), conviction scoring + bands, faith commitment + intensity, dynasty marriage parity (90-day expiration, 280-day gestation, death dissolution with legitimacy/oathkeeping effects), lesser-house loyalty parity (mixed-bloodline, marital-anchor, world-pressure drift with hostile breakaway spawning), minor-house levy parity (claim ownership/stability gates, loyalty threshold, retinue cap, militia/swordsman/bowman profile selection).

**AI strategic layer (sub-slices 1-25):** Territory strategy, pressure, governance pressure, worker gather dispatch, siege orchestration, covert ops dispatch, build timer chain, marriage proposal/inbox/profile/effects/terms, lesser house promotion, pact proposal/break, narrative bridge, dynasty operations foundation, captive state, missionary execution, holy war execution, divine right execution, captive rescue execution, captive ransom execution, missionary resolution (Bundle 4).

**Scout raids and logistics interdiction:** Landed today. RaidComponent, RaidMovementSystem, RaidResourceTheftSystem, abort-on-intercept, dedicated smoke.

**Player diplomacy (new lane, landed today):** PlayerMarriageProposalSystem, PlayerMarriageAcceptSystem with legitimacy/hostility/conviction/DualClock effects, MarriageDeathDissolutionSystem; all 3 sub-slices on master.

**Player covert ops (new lane, landed today):** PlayerCovertOpsFoundation (espionage dispatch, capacity gate, cost deduction), player assassination + sabotage dispatch (target-member assassination, building sabotage, dedicated smoke).

**Your active lanes:**
- `fortification-siege-imminent-engagement` (follow-ups: sealing-cost tier scaling, worker-locality gate, repair narrative all landed today; lane can now be formally retired or kept open for HUD integration)
- `player-diplomacy` (owns `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/**`)
- `player-covert-ops` (owns `unity/Assets/_Bloodlines/Code/PlayerCovertOps/**`)
- `scout-raids` (owns `unity/Assets/_Bloodlines/Code/Raids/**`)

**Claude Code's active lane (do NOT touch):** `ai-strategic-layer` owns `unity/Assets/_Bloodlines/Code/AI/**` exclusively. The next Claude Code bundle is holy war resolution (sub-slice 26) and divine right resolution (sub-slice 27).

**Empty stub branch:** `codex/unity-player-counter-intelligence` exists locally but has no commits. This is your immediate next target (sub-slice 3C of the player-covert-ops lane).

---

## WORK PRIORITY ORDER

Do not stop after a sub-slice. Complete it, validate it, push it, merge it, then immediately begin the next. Chain as many as your session allows.

---

### PRIORITY 1: Player Counter-Intelligence (Sub-Slice 3C) -- your stub branch is ready

**Lane:** player-covert-ops (owned: `unity/Assets/_Bloodlines/Code/PlayerCovertOps/**`)

**Branch:** `codex/unity-player-counter-intelligence` (already exists locally, base it off current master)

**Browser references:**
- `src/game/core/simulation.js`:
  - `startCounterIntelligenceOperation` (search): gate chain, cost (`COUNTER_INTEL_COST`), operation entity creation
  - `tickDynastyCounterIntelligence` (search): per-tick counter-intel decay, detection probability applied to incoming hostile operations
  - `tickDynastyIntelligenceReports` (search): per-tick intelligence report accumulation on a faction; report shape: `{ type, sourceFactionId, timestamp, detail }`

**Implement:**
- `PlayerCounterIntelligenceComponent.cs`: per-faction counter-intel level, last-activated InWorldDays, decay rate
- `PlayerCounterIntelligenceSystem.cs` (ISystem): each DualClock tick, decay active counter-intel level by the browser rate; check incoming hostile DynastyOperationComponent entities (cross-lane read from AI/ path -- read only, no write) and apply detection gate: if SourceFactionId == enemy and CounterIntelLevel >= detection threshold, flip the operation's Active = false (abort it)
- `IntelligenceReportElement.cs` buffer: per-report shape matching browser `{ type, sourceFactionId, createdAtInWorldDays, detailToken }`
- `PlayerIntelligenceReportSystem.cs` (ISystem): generate one report per detected/expired operation per DualClock day; append to target faction's buffer (cap at browser CAPTIVE_LEDGER_LIMIT = 16 style trim)
- Add `TryDebugGetPlayerCounterIntelLevel(factionId)`, `TryDebugActivateCounterIntel(factionId)`, `TryDebugGetIntelligenceReports(factionId)` to `BloodlinesDebugCommandSurface.PlayerCovertOps.cs` partial

**Smoke validator:** `BloodlinesPlayerCounterIntelligenceSmokeValidation.cs` / `Invoke-BloodlinesUnityPlayerCounterIntelligenceSmokeValidation.ps1`
- Phase 1: activate counter-intel, level set correctly, not yet decayed
- Phase 2: after DualClock tick, level has decayed by browser rate
- Phase 3: hostile missionary operation present + counter-intel active -> operation aborted (Active flipped to false)
- Phase 4: aborted operation generates an intelligence report on the player faction's buffer

Run full 10-gate validation chain. Push. Merge.

---

### PRIORITY 2: Player-Side Holy War and Divine Right Declaration

**Lane:** player-diplomacy (owned: `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/**`)

**Branch:** `codex/unity-player-holy-war-divine-right`

**Note:** Claude Code's AI lane owns `AIHolyWarExecutionSystem.cs` and `DynastyOperationHolyWarComponent.cs`. You must NOT modify those files. However, you CAN read `DynastyOperationHolyWarComponent.cs` as a cross-lane read and create your OWN player-side dispatch system in `PlayerDiplomacy/`. The operation entities use the same component shape -- that is intentional and correct.

**Browser references:**
- `src/game/core/simulation.js`:
  - `startHolyWarDeclaration` (~10565-10602): player initiates a holy war, gate chain (faith intensity, incompatible-faith target, operation capacity), cost deduction (`HOLY_WAR_COST = { influence: 24 }`), operation entity creation with `DynastyOperationHolyWarComponent`
  - `startDivineRightDeclaration` (~10784-10835): player asserts divine right, gate chain (faith intensity >= 80, level >= 5, no active divine right already, per-faction one-active gate), operation entity creation with `DynastyOperationDivineRightComponent`
  - `getHolyWarDeclarationTerms` (~10424-10471): cost and compatibility profile (intensity cost, incompatible faith gate)
  - `getDivineRightDeclarationTerms` (~10604-10653): cost and eligibility

**Implement:**
- `PlayerHolyWarDeclarationSystem.cs` (ISystem): consume `PlayerHolyWarDeclarationRequestComponent`; apply gate chain; on success create a `DynastyOperationHolyWarComponent` entity (same shape as AI lane -- cross-lane read for the component type, you write a new entity)
- `PlayerDivineRightDeclarationSystem.cs` (ISystem): consume `PlayerDivineRightDeclarationRequestComponent`; apply gate chain (intensity >= 80, faith level >= 5, no existing active divine right operation for this faction); on success create a `DynastyOperationDivineRightComponent` entity
- `PlayerHolyWarDeclarationRequestComponent.cs`, `PlayerDivineRightDeclarationRequestComponent.cs`
- Add debug API to `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`

**Smoke validator:** `BloodlinesPlayerHolyWarDivineRightSmokeValidation.cs` / `Invoke-BloodlinesUnityPlayerHolyWarDivineRightSmokeValidation.ps1`
- Phase 1: holy war declaration succeeds for incompatible-faith target, operation entity created, influence deducted
- Phase 2: holy war gate blocks when target has same faith
- Phase 3: divine right declaration succeeds when intensity >= 80 and level >= 5
- Phase 4: divine right gate blocks when faith intensity < 80

Run full 10-gate validation. Push. Merge.

---

### PRIORITY 3: Player Missionary Dispatch

**Lane:** player-diplomacy

**Branch:** `codex/unity-player-missionary-dispatch`

**Browser reference:**
- `src/game/core/simulation.js` `startMissionaryOperation` (~10523-10563): gate chain, cost (`MISSIONARY_COST = { influence: 14 }`), duration (32 in-world days), operator selection (sorcerer/spymaster role), per-kind component creation with `DynastyOperationMissionaryComponent`

**Implement:**
- `PlayerMissionaryDispatchSystem.cs` (ISystem): consume `PlayerMissionaryDispatchRequestComponent`; apply gate chain (operation capacity, influence cost, operator member available); create `DynastyOperationMissionaryComponent` entity; push NarrativeMessageBridge entry (cross-lane read on `NarrativeMessageBridge` from AI path -- read-only)
- `PlayerMissionaryDispatchRequestComponent.cs`

**Smoke validator:** 3-phase: dispatch succeeds and creates operation entity with correct fields; insufficient-influence gate blocks; capacity-full gate blocks.

---

### PRIORITY 4: New Lane - Conviction Band Effects Wiring

**Lane:** conviction-band-wiring (claim this as a new lane)

**Owned paths:** Narrow additive edits to:
- `unity/Assets/_Bloodlines/Code/Economy/RealmConditionCycleSystem.cs` (add LoyaltyProtectionMultiplier application)
- `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs` (add PopulationGrowthMultiplier application)
- `unity/Assets/_Bloodlines/Code/Combat/AttackResolutionSystem.cs` (add CaptureMultiplier application when a unit is captured)

**Context:** `ConvictionBandEffects.ForBand(band)` already returns six canonical multipliers (`LoyaltyProtectionMultiplier`, `PopulationGrowthMultiplier`, `CaptureMultiplier`, `ResourceProductionMultiplier`, `TerritoryRetentionMultiplier`, `ConversionResistanceMultiplier`). None of them are wired into any downstream system yet. The browser doesn't apply them either (it reports them). This slice wires three of the six into live system paths.

**Browser reference:**
- `src/game/core/simulation.js` `CONVICTION_BAND_EFFECTS` (~line 1849): the six multipliers per band
- `src/game/core/simulation.js` search `loyaltyProtection`, `populationGrowth`, `captureMultiplier` to find how the browser intended to consume them (some may be aspirational/not-yet-applied in browser too -- document accurately)

**Implement:**
- In `RealmConditionCycleSystem`: after computing loyalty delta per cycle, multiply by `(1f - ConvictionBandEffects.ForBand(band).LoyaltyProtectionMultiplier)` so high-conviction factions resist loyalty loss
- In `StarvationResponseSystem`: multiply population decline by `(1f - ConvictionBandEffects.ForBand(band).PopulationGrowthMultiplier)` so high-conviction factions lose less population per famine cycle
- In `AttackResolutionSystem`: when a unit kill would normally resolve, check if the defender has a `CaptureMultiplier > 0` on its faction's conviction band; if so, with that probability spawn a `CapturedMemberElement` on the attacker's faction (cross-lane read on `CapturedMemberElement` from AI path) instead of destroying the unit
- Update `BloodlinesConvictionSmokeValidation` with a Phase 5 that proves `LoyaltyProtectionMultiplier` reduces the loyalty delta for a faction in `ApexMoral` band versus a `Neutral` band faction

**Smoke validator extension:** Extend existing `BloodlinesConvictionSmokeValidation.cs` (it's owned by the retired conviction-scoring lane; verify it is not actively claimed before editing -- if it is, create a new `BloodlinesConvictionBandWiringSmokeValidation.cs` instead).

---

### PRIORITY 5: New Lane - HUD and Realm Condition Legibility

**Lane:** hud-legibility (claim as new lane)

**Owned paths:** `unity/Assets/_Bloodlines/Code/HUD/**` (create this folder)

**Branch:** `codex/unity-hud-realm-condition`

**Context:** All the data exists in ECS components -- `RealmConditionComponent`, `FactionLoyaltyComponent`, `PopulationComponent`, `ConvictionComponent`, `FaithStateComponent`, `MatchProgressionComponent`, `WorldPressureComponent`, `FortificationComponent`, `VictoryStateComponent`. None of it is surfaced in a unified readable format for the player. This lane creates the HUD data layer.

**Browser reference:**
- `src/game/core/simulation.js` `getRealmConditionSnapshot` (search): the object shape the browser exposes to the UI
- `src/game/core/simulation.js` `getMatchProgressionSnapshot` (~13650): stage name, in-world days, phase label

**Sub-Slice 5A: Realm Condition HUD Component**
- Branch: `codex/unity-hud-realm-condition`
- `RealmConditionHUDComponent.cs`: per-faction snapshot struct: loyalty (current/max), population (current/cap), foodStatus (normal/famine/crisis), waterStatus, convictionBand (enum), faithLevel (int), stabilityScore (float)
- `RealmConditionHUDSystem.cs` (ISystem in `PresentationSystemGroup` or after `SimulationSystemGroup`): reads the live components and keeps `RealmConditionHUDComponent` synchronized once per N ticks (not every tick -- read from `DualClockComponent.InWorldDays` and gate on a LastHUDUpdateInWorldDays threshold)
- Add `TryDebugGetRealmConditionHUD(factionId)` to `BloodlinesDebugCommandSurface.cs` main surface

**Sub-Slice 5B: Match Progression and Victory Readout**
- Branch: `codex/unity-hud-match-progression`
- `MatchProgressionHUDComponent.cs`: singleton; stage (1-5), stageName (FixedString64), phaseLabel (FixedString32), inWorldDays (float), worldPressureLevel (int), dominantLeaderFactionId (Entity or Entity.Null)
- `MatchProgressionHUDSystem.cs` (ISystem): keeps the singleton synchronized from `MatchProgressionComponent` and `WorldPressureComponent` and `WorldPressureCycleTrackerComponent`
- `VictoryConditionReadoutComponent.cs`: per-faction, per-victory-condition: ConditionId, ProgressPct (0-1 float), TimeRemainingInWorldDays (nullable float); ISystem `VictoryConditionReadoutSystem` reads `VictoryStateComponent` and each condition's trigger thresholds and current values to compute distance-to-win
- Add `TryDebugGetMatchProgressionHUD()` and `TryDebugGetVictoryReadout(factionId)` to debug command surface

Smoke validators for each sub-slice. Push. Merge each independently.

---

### PRIORITY 6: New Lane - Player Non-Aggression Pact Proposal (Player-Side)

**Lane:** player-diplomacy (existing lane)

**Branch:** `codex/unity-player-pact-proposal`

**Context:** The AI lane has `AIPactProposalExecutionSystem.cs` and `PactBreakSystem.cs` under `Code/AI/`. The `PactComponent.cs` entity is also in the AI lane. The player-side equivalent needs to consume the same `PactComponent` shape (cross-lane read on the entity type; you CREATE a new entity of this type from your player-side system -- same pattern as the holy war dispatch above).

**Browser references:**
- `src/game/core/simulation.js` `proposeNonAggressionPact` (~5185-5222): gate chain (both Kingdom, not hostile after proposal, no existing pact, influence >= 50 + gold >= 80), cost deduction, PactComponent entity creation, hostility drop both ways, narrative push
- `src/game/core/simulation.js` `breakNonAggressionPact` (~5224-5257): early-break legitimacy cost -8, oathkeeping conviction -2, mutual hostility restoration

**Implement:**
- `PlayerPactProposalSystem.cs` (ISystem): player-side pact proposal with same gate chain as AI
- `PlayerPactBreakSystem.cs` (ISystem): player-initiated pact break with legitimacy/conviction penalty
- `PlayerPactProposalRequestComponent.cs`, `PlayerPactBreakRequestComponent.cs`
- Smoke validator: 4 phases (proposal success, already-pact gate, break with legitimacy penalty, insufficient-resource gate)

---

### PRIORITY 7: Fortification HUD Integration (if fortification lane still active)

**Branch:** `codex/unity-hud-fortification`

**Implement:**
- `FortificationHUDComponent.cs`: per-settlement, reads from the fortification lane's owned components (`FortificationComponent`, `FortificationReserveComponent`, `BreachSealingProgressComponent`, `DestroyedCounterRecoveryProgressComponent`) via cross-lane reads
- Fields: tier (int), openBreachCount (int), reserveFrontage (float), musteredDefenders (int), sealingProgress (float 0-1), recoveryProgress (float 0-1)
- `FortificationHUDSystem.cs` (ISystem)
- `TryDebugGetFortificationHUD(settlementEntityIndex)` on debug command surface

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

3. **Write a per-slice handoff** at `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<slice>.md`. Include: goal, browser reference line numbers, work completed, validation proof lines (quote the actual output -- not just "passed"), Unity-side simplifications deferred, and exact next action.

4. **Update `docs/unity/CONCURRENT_SESSION_CONTRACT.md` after every sub-slice**: bump Revision, set Last Updated to today (YYYY-MM-DD), set Last Updated By to `codex-<lane>-YYYY-MM-DD`, add or update the lane subsection with new owned paths and the handoff document reference.

5. **Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/PROJECT_STATE.json` after every sub-slice.** Append only -- never overwrite or touch another lane's entries. Rebase before updating.

6. **Create a new branch per sub-slice** following the pattern `codex/unity-<lane>-<description>`. Check out from `origin/master`, not from a prior working branch.

7. **Use the wrapper lock** for all Unity batch invocations: `scripts/Invoke-BloodlinesUnityWrapperWithLock.ps1 -Session codex-<identifier>`. Check for the lock, reclaim if > 15 minutes old, delete after completion.

8. **Push branches to origin after validation passes.** Merge to master with `git merge --no-ff <branch> -m "Merge <branch>: <one-line description>"`. Run all 10 gates again on master after merging.

9. **Do not modify browser runtime files** (`src/`, `data/`, `tests/`, `play.html`). Read-only behavioral specification.

10. **Follow Unity 6.3 DOTS/ECS idioms:** ISystem (not MonoBehaviour), SystemAPI, EntityCommandBuffer, NativeArray, `[BurstCompile]` on hot paths, `[UpdateAfter]`/`[UpdateBefore]` ordering where dependencies exist.

11. **Write a dedicated smoke validator** for every new system. Validators live in `unity/Assets/_Bloodlines/Code/Editor/` with a matching PowerShell wrapper in `scripts/`. Validators prove behavioral correctness with specific numeric assertions, not just "no exception thrown."

12. **Cross-reference browser simulation.js line numbers** in every handoff document. If Unity behavior diverges from the browser spec, document the divergence explicitly and justify it.

13. **Do NOT stop after one sub-slice.** Continue through the priority stack immediately. The priority list above is ordered -- work top to bottom. Only stop when session capacity genuinely runs out.

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

These files are the handshake to the next session. If you skip them, the next session starts blind.

---

## SUCCESS CRITERIA FOR THIS DIRECTIVE WINDOW

After 3-5 days:
- Player counter-intelligence on master (sub-slice 3C complete)
- Player holy war and divine right dispatch on master
- Player missionary dispatch on master
- Conviction band effects wired into 3 downstream systems
- HUD realm-condition layer on master with 2+ sub-slices
- Player pact proposal on master
- Contract revision advanced at least 8-12 increments from current revision 63
- No regressions in any existing smoke validators
