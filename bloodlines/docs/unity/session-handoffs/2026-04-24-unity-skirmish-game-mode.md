# Unity Slice Handoff: skirmish-game-mode / Skirmish Game Mode Manager

**Date:** 2026-04-24
**Lane:** skirmish-game-mode
**Branch:** claude/unity-skirmish-game-mode
**Status:** Complete

## Goal

Implement a canonical `SkirmishGameModeComponent` singleton and `SkirmishGameModeManagerSystem` that make the skirmish match phase lifecycle explicit in Unity ECS: Setup → Playing → PostMatch. The browser runtime leaves this lifecycle implicit in simulation.js initialization and victory state; Unity HUD and persistence systems need it as an authoritative first-class signal.

## Browser Reference

Absent -- match-phase lifecycle is implicit in simulation.js initialization (world seeding) and victory state. The system is Unity-native, designed from the owner direction that skirmish vs AI and multiplayer are the only shipping modes.

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md` -- skirmish vs AI and multiplayer are the only shipping game modes; no campaign, no tutorial.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Skirmish/SkirmishGameModeComponent.cs` -- `SkirmishPhase` enum (Setup=0, Playing=1, PostMatch=2); `SkirmishGameModeComponent` (singleton IComponentData: Phase, FactionCount, IsPlayerVsAI, PlayerFactionId, MatchStartInWorldDays, MatchEndInWorldDays)
- `unity/Assets/_Bloodlines/Code/Skirmish.meta` and `unity/Assets/_Bloodlines/Code/Skirmish/SkirmishGameModeComponent.cs.meta` -- new Skirmish folder and file meta
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishGameModeManagerSystem.cs` -- `[UpdateInGroup(SimulationSystemGroup), UpdateAfter(MatchEndSequenceSystem)]`; lazy-creates singleton; Setup→Playing fires when no MapBootstrapPendingTag and VictoryStateComponent is seeded (enumerates Kingdom factions, sets FactionCount, IsPlayerVsAI, PlayerFactionId from AIEconomyControllerComponent presence, MatchStartInWorldDays from DualClockComponent); Playing→PostMatch fires when MatchEndStateComponent.IsMatchEnded=true (sets MatchEndInWorldDays)
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishGameModeManagerSystem.cs.meta`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SkirmishGameMode.cs` -- `TryDebugGetSkirmishGameMode` (phase, factionCount, isPlayerVsAI, playerFactionId, matchStartInWorldDays, matchEndInWorldDays)
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SkirmishGameMode.cs.meta`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSkirmishGameModeSmokeValidation.cs` -- 3-phase smoke: (1) default Setup init and zero match fields; (2) Setup→Playing sets Phase=Playing, FactionCount positive, IsPlayerVsAI=true, PlayerFactionId non-empty, MatchStartInWorldDays non-negative; (3) Playing→PostMatch sets Phase=PostMatch and carries MatchEndInWorldDays from MatchEndStateComponent
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSkirmishGameModeSmokeValidation.cs.meta`
- `scripts/Invoke-BloodlinesUnitySkirmishGameModeSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assembly-CSharp.csproj` -- 3 new Compile entries (SkirmishGameModeComponent.cs, SkirmishGameModeManagerSystem.cs, BloodlinesDebugCommandSurface.SkirmishGameMode.cs)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (BloodlinesSkirmishGameModeSmokeValidation.cs)
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision 129→130; added skirmish-game-mode lane entry under Active (completed) lanes

## Scope Discipline

- Did not implement a lobby/pre-match UI or team assignment screen (separate lane concern; no UI system exists yet)
- Did not wire starting resource grants or spawn-position assignment beyond what SkirmishBootstrapSystem already seeds
- Did not implement multiplayer-specific setup phases (multiplayer-foundation lane is separate)
- Did not modify VictoryConditionEvaluationSystem or MatchEndSequenceSystem (read-only cross-lane references)
- Did not add per-faction "match-ready" signaling (no networking layer exists yet)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env
4. Combat smoke -- PASS
5. Scene shells -- PASS
6. Conviction smoke -- PASS
7. Dynasty smoke -- PASS
8. Faith smoke -- PASS
8a. Skirmish game mode smoke -- PASS (3 phases green)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=130)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed to next unclaimed lane: dynasty prestige (renown decay + prestige drift distinct from the already-implemented renown accumulation system) or audio dispatch foundation (Wwise scaffolding).
