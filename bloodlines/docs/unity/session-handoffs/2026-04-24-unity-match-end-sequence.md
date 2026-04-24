# Unity Slice Handoff: match-end-sequence / Match End Sequence

**Date:** 2026-04-24
**Lane:** match-end-sequence
**Branch:** claude/unity-match-end-sequence
**Status:** Complete

## Goal

Close the gameplay loop by reacting when VictoryStateComponent.Status transitions from Playing to Won or Lost. Prior to this slice, VictoryConditionEvaluationSystem correctly set the status but nothing consumed that signal: no XP was awarded, no narrative message was pushed, and no match-result data was made available to the HUD. This slice delivers all three.

## Browser Reference

Absent -- match-end result screen not in simulation.js. Implemented from canonical dynasty progression and narrative message designs.

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md` -- cross-match dynasty XP and tier bonuses are canonically in scope. Match end is the event that triggers XP award.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Victory/MatchEndStateComponent.cs` -- `MatchEndStateComponent` (IComponentData singleton: IsMatchEnded bool, WinnerFactionId FixedString32Bytes, VictoryType VictoryConditionId, VictoryReason FixedString128Bytes, MatchEndTimeInWorldDays float, XPAwarded bool)
- `unity/Assets/_Bloodlines/Code/Victory/MatchEndSequenceSystem.cs` -- `[UpdateInGroup(SimulationSystemGroup)]` `[UpdateAfter(VictoryConditionEvaluationSystem)]`; `private bool _fired` once-on-event guard; reads VictoryStateComponent; on first Status != Playing: creates MatchEndStateComponent singleton, places DynastyXPAwardRequestComponent on all faction entities (winner 150 XP placement=1 by WinnerFactionId, runner-up 75 XP placement=2 by territory count, others 25 XP placement=3+), calls NarrativeMessageBridge.Push (Good tone for Won, Warn tone for Lost)
- `unity/Assets/_Bloodlines/Code/HUD/MatchEndHUDComponent.cs` -- `MatchEndHUDComponent` (IComponentData singleton: IsVisible bool, WinnerFactionId, VictoryType, VictoryReason, MatchEndTimeInWorldDays, PlayerXPAwarded bool)
- `unity/Assets/_Bloodlines/Code/HUD/MatchEndHUDSystem.cs` -- `[UpdateInGroup(SimulationSystemGroup)]` `[UpdateAfter(MatchEndSequenceSystem)]`; requires MatchEndStateComponent; lazy-creates and writes MatchEndHUDComponent singleton each tick IsMatchEnded is true
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.MatchEnd.cs` -- TryDebugGetMatchEndState (IsMatchEnded, WinnerFactionId, VictoryType, VictoryReason, MatchEndTimeInWorldDays, XPAwarded); TryDebugGetMatchEndHUD (IsVisible, WinnerFactionId, VictoryType, PlayerXPAwarded)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMatchEndSequenceSmokeValidation.cs` -- 3-phase smoke: (1) MatchEndStateComponent initialized with IsMatchEnded=true and XPAwarded=true; (2) XP ordering winner 150 > runner-up 75 > other 25, placements 1/2/3+; (3) narrative text non-empty, Good and Warn tones distinct
- `scripts/Invoke-BloodlinesUnityMatchEndSequenceSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 6 new .cs files
- `unity/Assembly-CSharp.csproj` -- 5 new Compile entries
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry

## Scope Discipline

- Did not implement match-end persistence or save-game integration (dynasty-persistence lane concern)
- Did not implement lobby return flow or session reset after match end (skirmish-game-mode-manager concern)
- Did not add per-faction XP visibility in HUD rows (follow-up HUD slice concern)
- Did not implement match replay or postgame stats (out of scope per owner direction)
- Did not add audio trigger for match end (audio-dispatch lane concern)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env
4. Combat smoke -- PASS
5. Scene shells -- PASS
6. Conviction smoke -- PASS
7. Dynasty smoke -- PASS
8. Faith smoke -- PASS
8a. Match-end sequence smoke -- PASS (3 phases green)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=128)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed to the next unclaimed priority from the active directive stack: skirmish game mode manager, audio dispatch foundation, dynasty prestige (renown decay + prestige drift), or faith doctrine combat wiring.
