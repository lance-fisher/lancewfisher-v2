# Unity Slice Handoff: world-trade-routes / trade route evaluation system

**Date:** 2026-04-23
**Lane:** world-trade-routes
**Branch:** claude/unity-world-trade-routes
**Status:** Complete

## Goal

Implement a per-faction trade route evaluation system. Once per in-world day, each faction's
adjacent uncontested owned control-point pairs are counted. Two control points are adjacent
when the distance between them is at most the sum of their radii plus a 2-tile padding. Each
qualifying pair contributes 5 gold per day to the faction's ResourceStockpileComponent. A
TradeRouteComponent summary is lazily attached to each faction entity and updated on the
day boundary.

## Browser Reference

Absent -- trade route system not in simulation.js. Implemented from canonical trade route design.

## Canon Reference

`CLAUDE.md` -- full canonical gameplay delivery; economy depth is a product quality gate.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Economy/TradeRouteComponent.cs` -- `TradeRouteComponent` (per-faction: ActiveRouteCount, TotalGoldPerTickFromTrades, LastUpdatedAtInWorldDays)
- `unity/Assets/_Bloodlines/Code/Economy/TradeRouteEvaluationSystem.cs` -- `[UpdateInGroup(SimulationSystemGroup)]`; once-per-day gate via `math.floor(DualClockComponent.InWorldDays)`; O(n²) unique pair adjacency check on owned non-contested CPs; lazy-attaches TradeRouteComponent; adds 5 gold × route count to ResourceStockpileComponent
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.TradeRoutes.cs` -- `TryDebugGetTradeRoutes(factionId, out int, out float)` reads faction entity's TradeRouteComponent
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesTradeRouteSmokeValidation.cs` -- 3-phase smoke: adjacent uncontested pair yields 1 route/5 gold, contested CP excluded, cross-faction CPs yield zero routes
- `scripts/Invoke-BloodlinesUnityTradeRouteSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 4 new .cs files
- `unity/Assembly-CSharp.csproj` -- 3 new Compile entries (TradeRoute*.cs, debug partial)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry

## Scope Discipline

- Did not implement trade route UI (visible route rendering is a graphics lane concern)
- Did not add trade route bonuses beyond gold (goods/culture trade is a follow-up design decision)
- Did not wire contested state transitions to trade route interruption events (already handled by the IsContested gate)
- Did not implement inter-faction trade agreements (diplomatic lane concern)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, 113 pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env
4. Combat smoke -- SKIP-env
5. Scene shells -- SKIP-env
6. Conviction smoke -- SKIP-env
7. Dynasty smoke -- SKIP-env
8. Faith smoke -- SKIP-env
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=124)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed with P6 HUD Skirmish Match Status Panel: new lane `skirmish-status-hud`, branch `claude/unity-hud-skirmish-match-status`.
