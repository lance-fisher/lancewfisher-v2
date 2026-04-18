# Unity Slice Handoff: World Pressure Escalation
**Date:** 2026-04-18
**Lane:** world-pressure-escalation
**Branch:** claude/unity-world-pressure
**Session:** claude-world-pressure-2026-04-18

---

## Goal

Port `updateWorldPressureEscalation` and `applyWorldPressureConsequences` (simulation.js:13709, 13695) into Unity ECS. Wire the stage 5 convergence signal into `MatchProgressionEvaluationSystem`. Add a 4-phase smoke validator with batch-mode entry point.

---

## Work Completed

### New files
- `unity/Assets/_Bloodlines/Code/WorldPressure/WorldPressureComponent.cs` -- `WorldPressureComponent` (per-faction pressure state: Score, Streak, Level, Label, Targeted, TerritoryExpansionScore, GreatReckoningScore) and `WorldPressureCycleTrackerComponent` (singleton: Accumulator, CycleSeconds=90)
- `unity/Assets/_Bloodlines/Code/WorldPressure/WorldPressureEscalationSystem.cs` -- ISystem in SimulationSystemGroup, UpdateBefore MatchProgressionEvaluationSystem. Score/Targeted updated every frame; Streak/Level/consequences cycle-gated at 90s. ApplyConsequences: lowest-loyalty CP loses [level] loyalty; legitimacy drain at level >= 2
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.WorldPressure.cs` -- TryDebugGetWorldPressure, TryDebugGetWorldPressureCycleTracker
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesWorldPressureSmokeValidation.cs` -- 4-phase batch smoke validator
- `scripts/Invoke-BloodlinesUnityWorldPressureSmokeValidation.ps1` -- batch runner

### Modified files (narrow edits)
- `unity/Assembly-CSharp.csproj` -- added WorldPressureComponent.cs, WorldPressureEscalationSystem.cs, BloodlinesDebugCommandSurface.WorldPressure.cs
- `unity/Assembly-CSharp-Editor.csproj` -- added BloodlinesWorldPressureSmokeValidation.cs
- `unity/Assets/_Bloodlines/Code/Time/MatchProgressionEvaluationSystem.cs` -- added `playerWorldPressureConvergence` block; updated stage 5 convergence check to include WorldPressure.Targeted && Level >= 3
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs` -- added `worldPressureChecked` field to RuntimeSmokeState; added ProbeWorldPressureIntegrity; wired into diagnostics string
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- revision 7 -> 8; added world-pressure-escalation active lane

---

## Score Sources Ported (simulation.js)

| Source | Browser ref | Value |
|---|---|---|
| territoryExpansion | sim:13193 | max(0, ownedTerritories - 2) |
| greatReckoning | sim:13186 | 4 when faction is GR target |

Remaining sources (divineRightDeclaration, offHomeHoldings, holyWar, captives, hostileOperations, darkExtremes) deferred until those systems come online.

---

## Verification Results

All 8 gates green:

1. `dotnet build unity/Assembly-CSharp.csproj` -- 0 errors
2. `dotnet build unity/Assembly-CSharp-Editor.csproj` -- 0 errors
3. Bootstrap runtime smoke -- PASS (worldPressureChecked=True in proof line)
4. Combat smoke -- exit 0
5. Scene shells -- Bootstrap + Gameplay green
6. `node tests/data-validation.mjs` -- exit 0
7. `node tests/runtime-bridge.mjs` -- exit 0
8. Contract staleness check -- revision=8, exit 0

World pressure smoke (new gate, not yet in the 8-gate sequence):
- Phase 1 PASS: CycleTracker defaults; Accumulator=0 CycleSeconds=90
- Phase 2 PASS: Score=3 TerritoryExpansionScore=3 Level=0
- Phase 3 PASS: Score=4 Targeted=True Streak=6 Level=3 Label=Convergence
- Phase 4 PASS: lowest-loyalty CP drained by Level=3; loyalty=47.0 (was 50)

---

## Key Implementation Notes

**Cycle gating.** Score/TerritoryExpansionScore/Targeted update every frame (HUD accuracy). Streak/Level/consequences only advance when `accumulator >= cycleSeconds` (canonical 90s realm cycle). Accumulator wraps by subtracting cycleSeconds rather than resetting to 0.

**Dominant leader.** Score >= 4 (GREAT_RECKONING_PRESSURE_SCORE) AND strictly highest (vs secondScore). Two-pass approach: first pass builds scores array, second pass finds dominant and updates all WorldPressureComponents.

**Singleton init.** `OnCreate` guards tracker creation with `SystemAPI.HasSingleton<WorldPressureCycleTrackerComponent>()` before creating the entity, then calls `state.RequireForUpdate<WorldPressureCycleTrackerComponent>()`. Smoke tests must call `GetOrCreateSystem` before seeding tracker to avoid double-singleton (RequireForUpdate would see 2 entities and fail the singleton contract).

**Smoke test pattern.** Bare worlds in batch mode require explicit SimulationSystemGroup setup (GetOrCreateSystemManaged + AddSystemToUpdateList). This differs from MatchProgression smoke (no RequireForUpdate = always updates) but matches the combat/conviction pattern.

---

## Current Readiness

Lane complete. Contract revision 8 reflects active status. After this commit, contract should be updated to retire the lane (revision 8 -> 9).

---

## Next Action

1. Update contract: world-pressure-escalation lane active -> retired (revision 8 -> 9)
2. Commit all world-pressure lane files to `claude/unity-world-pressure`
3. Merge to master, push to origin

Follow-up (not blocking):
- Add `WorldPressureComponent` to `SkirmishBootstrapSystem` so gameplay faction spawn seeds it automatically
- Add world pressure smoke to the canonical 8-gate validation sequence in CLAUDE.md
