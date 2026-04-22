# CODEX MULTI-DAY DIRECTIVE: BLOODLINES UNITY PRODUCTION PUSH
# Date: 2026-04-22
# Duration: Use this prompt for the next 2-3 days of Codex sessions
# Owner: Lance Fisher
# Supersedes: CODEX_MULTI_DAY_DIRECTIVE_2026-04-21.md

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

## CURRENT BUILD STATE (as of contract revision 79, 2026-04-22)

All of the following are on master and validated:

**Core simulation:** Economy, construction, production, starvation/loyalty/cap-pressure, stability surplus, loyalty density, state snapshot/restore, dual-clock, five-stage match progression, world pressure escalation, victory conditions (CommandHallFall, TerritorialGovernance, DivineRight).

**Combat:** Melee, ranged projectile, explicit attack orders, attack-move, target acquisition with sight loss, group movement, soft unit separation, hold-position and retreat stances (8 smoke phases).

**Fortification and siege (sub-slices 1-9 plus follow-ups):** Full tier/reserve/breach/sealing/recovery chain, tier-scaled sealing costs, worker-locality gate, repair narrative.

**Dynasty systems:** Core, conviction scoring + bands, faith commitment + intensity, marriage parity (proposal/expiration/gestation/death-dissolution), lesser-house loyalty parity (drift + hostile breakaway spawning), minor-house levy parity (claim gates, retinue cap, unit profile selection).

**AI strategic layer (sub-slices 1-25):** Full AI command dispatch, strategic pressure, governance pressure, worker gather, siege orchestration, covert ops, build timer, marriage proposal/inbox/profile/effects/terms, lesser house promotion, pact proposal/break, narrative bridge, dynasty operations, captive state, missionary execution, holy war execution, divine right execution, captive rescue execution, captive ransom execution, missionary resolution.

**Scout raids and logistics interdiction:** RaidComponent, RaidMovementSystem, RaidResourceTheftSystem, abort-on-intercept, dedicated smoke.

**Player diplomacy lane (`PlayerDiplomacy/**`):** Marriage proposal + acceptance + death dissolution, holy war declaration, divine right declaration, missionary dispatch, pact proposal + break.

**Player covert ops lane (`PlayerCovertOps/**`):** Foundation (espionage dispatch + capacity gate + cost deduction), assassination + sabotage dispatch, counter-intelligence (level, decay, detection, intelligence report buffer).

**HUD lane (`HUD/**`):** RealmConditionHUDComponent + System, MatchProgressionHUDComponent + System, FortificationHUDComponent + System.

**Conviction band wiring:** LoyaltyProtectionMultiplier in RealmConditionCycleSystem, PopulationGrowthMultiplier in StarvationResponseSystem, CaptureMultiplier in AttackResolutionSystem.

**Your active lanes:**
- `player-diplomacy` (owns `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/**`)
- `player-covert-ops` (owns `unity/Assets/_Bloodlines/Code/PlayerCovertOps/**`)
- `hud-legibility` (owns `unity/Assets/_Bloodlines/Code/HUD/**`)

**Claude Code's active lane (do NOT touch):** `ai-strategic-layer` owns `unity/Assets/_Bloodlines/Code/AI/**` exclusively.

---

## WORK PRIORITY ORDER

Do not stop after a sub-slice. Complete it, validate it, push it, merge it, then immediately begin the next. Chain as many as your session allows.

---

### PRIORITY 1: Victory Condition Distance Readout HUD (complete Priority 5B from prior directive)

**Lane:** hud-legibility (existing lane, owned: `unity/Assets/_Bloodlines/Code/HUD/**`)

**Branch:** `codex/unity-hud-victory-distance-readout`

**Context:** `VictoryStateComponent` exists on master and tracks active victory conditions per faction. `MatchProgressionHUDComponent` is on master. The missing piece is a per-faction, per-condition progress surface that lets the player see how far each faction is from winning.

**Browser references:**
- `src/game/core/simulation.js` `getVictoryConditionSnapshot` or `getVictoryReadout` (search): the browser object shape for condition progress
- `src/game/core/simulation.js` search for `TerritorialGovernance`, `DivineRight`, `CommandHallFall` condition evaluation logic to find the progress numerics
- `tests/runtime-bridge.mjs` search `victory` for any readout assertions

**Implement:**
- `VictoryConditionReadoutComponent.cs`: per-faction, DynamicBuffer. Each element: ConditionId (FixedString32), ProgressPct (float 0-1), IsLeading (bool), TimeRemainingEstimateInWorldDays (float, NaN if not computable)
- `VictoryConditionReadoutSystem.cs` (ISystem in presentation group): reads `VictoryStateComponent` and the underlying condition metrics per faction; populates the readout buffer once per N in-world days (same throttle pattern as RealmConditionHUDSystem)
- Add `TryDebugGetVictoryReadout(factionId)` to `BloodlinesDebugCommandSurface.cs` with a parseable string output: `VictoryReadout|FactionId=...|ConditionId=...|ProgressPct=...|IsLeading=...|`

**Smoke validator:** `BloodlinesVictoryReadoutSmokeValidation.cs` / `Invoke-BloodlinesUnityVictoryReadoutSmokeValidation.ps1`
- Phase 1: faction with majority territory claims shows TerritorialGovernance ProgressPct > 0
- Phase 2: faction with high faith intensity shows DivineRight ProgressPct > 0
- Phase 3: faction whose enemy CommandHall is destroyed shows CommandHallFall ProgressPct = 1.0
- Phase 4: leading faction has IsLeading = true on the dominant condition

---

### PRIORITY 2: Player Captive Rescue Dispatch

**Lane:** player-diplomacy (existing lane, owned: `unity/Assets/_Bloodlines/Code/PlayerDiplomacy/**`)

**Branch:** `codex/unity-player-captive-rescue`

**Context:** The AI lane has `AICaptiveRescueExecutionSystem.cs` which resolves ongoing rescue operations. The player-side equivalent is the dispatch layer that creates the rescue operation entity. The AI lane owns the execution; you own the player-initiated request surface.

**Browser references:**
- `src/game/core/simulation.js` search `rescueCaptive` or `startCaptiveRescue` or `initiateCaptiveRescue`: gate chain (active captive in ledger, available operative, resource cost), operation entity creation with captive rescue component shape
- `src/game/core/simulation.js` search `CAPTIVE_RESCUE_COST` for cost constants
- `src/game/core/simulation.js` search `CapturedMemberElement` for the captive ledger shape (cross-lane read on AI-owned component type -- read only)

**Implement:**
- `PlayerCaptiveRescueRequestComponent.cs`: CaptiveMemberId (int), TargetFactionId (Entity), OperativeRoleFilter (byte)
- `PlayerCaptiveRescueDispatchSystem.cs` (ISystem): consume `PlayerCaptiveRescueRequestComponent`; gate chain: captive must exist in faction's CapturedMemberElement buffer, operative member available at required role, influence/gold cost deducted; on success create the rescue operation entity (same component shape as AI rescue execution targets -- cross-lane read on the component type definition, you write a new entity)
- Add `TryDebugDispatchCaptiveRescue(factionId, captiveMemberId)` to `BloodlinesDebugCommandSurface.PlayerDiplomacy.cs`

**Smoke validator:** 3-phase: rescue dispatch succeeds and creates operation entity; no-operative gate blocks; captive-not-found gate blocks.

---

### PRIORITY 3: Player Captive Ransom Dispatch

**Lane:** player-diplomacy (existing lane)

**Branch:** `codex/unity-player-captive-ransom`

**Browser references:**
- `src/game/core/simulation.js` search `ransomCaptive` or `startCaptiveRansom`: gate chain (captive in ledger, ransom gold available, target faction not hostile), ransom cost, operation entity creation
- `src/game/core/simulation.js` search `CAPTIVE_RANSOM_COST` or `ransomGold`

**Implement:**
- `PlayerCaptiveRansomRequestComponent.cs`: CaptiveMemberId (int), TargetFactionId (Entity), RansomGoldAmount (int)
- `PlayerCaptiveRansomDispatchSystem.cs` (ISystem): consume request; gate chain: captive exists, gold available, target not hostile; deduct gold; create ransom operation entity targeting the correct AI-owned execution system
- Add `TryDebugDispatchCaptiveRansom(factionId, captiveMemberId, goldAmount)` to debug surface

**Smoke validator:** 3-phase: ransom dispatch succeeds with correct gold deduction; insufficient-gold gate blocks; hostile-faction gate blocks.

---

### PRIORITY 4: Renown and Prestige Scoring Surface

**Lane:** player-diplomacy or claim a new `renown-scoring` lane

**Branch:** `codex/unity-renown-scoring`

**Context:** Dynasty renown and prestige are mentioned throughout the design bible and browser spec but the ECS surface is minimal. This slice creates the scoring surface that downstream systems can read to weight diplomatic AI responses, marriage desirability, and victory condition proximity.

**Browser references:**
- `src/game/core/simulation.js` search `renown`, `prestige`, `dynastyRenown`, `dynastyPrestige`: find how the browser computes and stores these values
- `src/game/core/simulation.js` search `calculateRenown` or `updateRenown` if present
- If the browser has no explicit renown accumulation (it may be aspirational), implement the accumulation rules from the design canon at `04_SYSTEMS/DYNASTY_SYSTEM.md` and document the divergence in the handoff

**Implement:**
- `DynastyRenownComponent.cs`: per-faction -- RenownScore (float), LastRenownUpdateInWorldDays (float), RenownDecayRate (float), PeakRenown (float)
- `DynastyRenownAccumulationSystem.cs` (ISystem): per DualClock day, accumulate renown from: active victories (+weight), held territory above average (+weight), faith intensity > 60 (+weight), legitimate succession events (+weight), renown decay at base rate; update PeakRenown when current > peak
- `BloodlinesDebugCommandSurface` addition: `TryDebugGetDynastyRenown(factionId)` returning parseable `DynastyRenown|FactionId=...|Score=...|PeakRenown=...|`

**Smoke validator:** 4-phase: renown accumulates over tick, decay applies at correct rate, territory contribution scales correctly, peak is tracked.

---

### PRIORITY 5: HUD Victory Condition Panel (if time remains)

**Lane:** hud-legibility (existing lane)

**Branch:** `codex/unity-hud-victory-panel`

**Context:** Now that VictoryConditionReadoutComponent exists (Priority 1), wire it into a unified HUD panel output that consolidates victory proximity for all factions into a single readable surface.

**Browser references:**
- `src/game/core/simulation.js` `getMatchSummary` or `getLeaderboard` (search): the browser's consolidated match state snapshot

**Implement:**
- `VictoryLeaderboardHUDComponent.cs`: singleton; ordered list (up to 8 factions) by leading victory condition ProgressPct; each entry: FactionId (Entity), LeadingConditionId (FixedString32), ProgressPct (float), IsHumanPlayer (bool)
- `VictoryLeaderboardHUDSystem.cs` (ISystem): reads all VictoryConditionReadoutComponent buffers, sorts by max ProgressPct, populates singleton once per N in-world days
- `TryDebugGetVictoryLeaderboard()` on debug command surface

**Smoke validator:** 3-phase: leaderboard populated correctly for 2-faction scenario, human-player flag set correctly, leading faction is at top.

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
- Victory condition distance readout HUD on master
- Player captive rescue dispatch on master
- Player captive ransom dispatch on master
- Renown/prestige scoring surface on master
- Victory leaderboard HUD on master (if time permits)
- Contract revision advanced at least 5-8 increments from revision 79
- No regressions in any existing smoke validators
