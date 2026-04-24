# Unity Slice Handoff: skirmish-status-hud / skirmish match status panel

**Date:** 2026-04-23
**Lane:** skirmish-status-hud
**Branch:** claude/unity-hud-skirmish-match-status
**Status:** Complete

## Goal

Implement a skirmish match status HUD panel. The panel aggregates per-faction unit counts,
territory counts, territory share, gold stockpile, and trade route data into a sorted read-model
refreshed at 0.25 in-world day cadence. Rows are sorted by territory share descending with
1-based rank assignments. The human player faction is flagged for UI differentiation.

## Browser Reference

Absent -- skirmish match status HUD not in simulation.js. Implemented from canonical skirmish
match legibility design.

## Canon Reference

`CLAUDE.md` -- UX matches the Zero Hour / Warcraft III era; information-dense, readable, no
reliance on external wikis for canonical mechanics. The match status panel is standard RTS
design required for skirmish vs AI and multiplayer shipping modes.

## Work Completed

- `unity/Assets/_Bloodlines/Code/HUD/SkirmishStatusHUDComponent.cs` -- `SkirmishStatusHUDComponent` (singleton header: InWorldDays, ActiveFactionCount, TotalUnitCount, TotalTerritoryCount, LastRefreshInWorldDays); `SkirmishStatusFactionRowHUDComponent` (IBufferElementData with InternalBufferCapacity(8): FactionId, Rank, UnitCount, TerritoryCount, TerritoryShare, Gold, TradeRouteCount, TradeGoldPerDay, IsHumanPlayer)
- `unity/Assets/_Bloodlines/Code/HUD/SkirmishStatusHUDSystem.cs` -- `[UpdateInGroup(PresentationSystemGroup)]`; 0.25-day refresh cadence via DualClockComponent; aggregates unit counts (UnitTypeComponent+FactionComponent), territory counts (ControlPointComponent non-neutral owned), gold from ResourceStockpileComponent, trade routes from TradeRouteComponent; sorts by TerritoryShare descending; assigns 1-based ranks; lazy singleton entity with dynamic buffer
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SkirmishStatus.cs` -- TryDebugGetSkirmishStatusHeader, TryDebugGetSkirmishStatusRows
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSkirmishStatusHUDSmokeValidation.cs` -- 3-phase smoke: header struct init, row sort and rank assignment, IsHumanPlayer flag for "player" faction ID
- `scripts/Invoke-BloodlinesUnitySkirmishStatusHUDSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 4 new .cs files
- `unity/Assembly-CSharp.csproj` -- 3 new Compile entries (SkirmishStatus*.cs, debug partial)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry

## Scope Discipline

- Did not implement actual UI rendering (panel rendering is a graphics lane concern)
- Did not wire the HUD to any UI binding layer (MonoBehaviour consumers can read via EntityManager)
- Did not add unit type breakdown by UnitRole (aggregate count is sufficient for the status panel)
- Did not implement observer/spectator differentiation (IsHumanPlayer covers skirmish vs AI mode)

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
10. Contract staleness check -- PASS (revision=125)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed with P7 Faction Color and Dynasty Emblem Assignment: new lane `faction-visual-assignment`, branch `claude/unity-faction-visual-assignment`.
