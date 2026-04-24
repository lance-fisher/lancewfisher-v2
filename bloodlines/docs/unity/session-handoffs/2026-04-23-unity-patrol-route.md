# Unity Slice Handoff: patrol-route / unit patrol waypoint system

**Date:** 2026-04-23
**Lane:** patrol-route
**Branch:** claude/unity-combat-patrol-route
**Status:** Complete

## Goal

Implement per-unit patrol routes for garrison and perimeter defense. Units ordered to patrol alternate between two waypoints indefinitely. Patrol suspends automatically when an explicit attack order is active and resumes when the attack order clears. This is the primary garrison posture for both player and AI units.

## Browser Reference

Absent -- patrol route system not in simulation.js. Implemented from canonical garrison/perimeter design.

## Canon Reference

`CLAUDE.md` -- skirmish vs AI and multiplayer are the only shipping modes; garrison/perimeter patrol is standard RTS mechanics required for both.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Combat/PatrolRouteComponent.cs` -- `PatrolRouteComponent` (per-unit: WaypointA/B, CurrentTarget byte, IsPatrolling, ArrivalThreshold); `PlayerPatrolOrderRequestComponent` (one-shot: WaypointA/B); `PlayerPatrolCancelRequestComponent` (zero-size cancel tag)
- `unity/Assets/_Bloodlines/Code/Combat/PatrolOrderSystem.cs` -- consumes set requests (writes PatrolRouteComponent, issues MoveCommandComponent to WaypointA) and cancel requests (removes PatrolRouteComponent, deactivates MoveCommand)
- `unity/Assets/_Bloodlines/Code/Combat/PatrolMovementSystem.cs` -- `[UpdateBefore(AttackOrderResolutionSystem)]`; per frame flips CurrentTarget when within ArrivalThreshold; suspends when AttackOrderComponent.IsActive; reissues MoveCommand if it was cleared without patrol ending
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.Patrol.cs` -- TryDebugSetPatrol, TryDebugCancelPatrol, TryDebugGetPatrolRoute (all accept Entity)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesPatrolRouteSmokeValidation.cs` -- 3-phase smoke: waypoint flip at arrival threshold, suspend on attack order, resume on attack order clear
- `scripts/Invoke-BloodlinesUnityPatrolRouteSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 5 new .cs files
- `unity/Assembly-CSharp.csproj` -- 4 new Compile entries (PatrolRoute*.cs, systems, debug partial)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry

## Scope Discipline

- Did not implement patrol UI (visible path rendering is a graphics lane concern)
- Did not wire patrol to AI strategic layer (AI may write PlayerPatrolOrderRequestComponent independently)
- Did not implement patrol route persistence across sessions
- Did not add patrol to any existing AI systems

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (CS0006 only, fixed missing using Bloodlines.Systems)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (CS0006 only)
3-8. Unity batch-mode smokes -- SKIP-env
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=123)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed with P5 Trade Route System: new lane `world-trade-routes`, branch `claude/unity-world-trade-routes`.
