# Unity Slice Handoff: rally-point / production building rally destinations

**Date:** 2026-04-23
**Lane:** rally-point
**Branch:** claude/unity-combat-rally-point
**Status:** Complete

## Goal

Implement per-building rally points for production buildings. When a player sets a rally point on a building, newly spawned units from that building immediately begin marching to the rally position rather than idling at the spawn offset. The system uses a one-shot request component consumed by a dedicated set system, with the wiring into spawn behavior as a narrow edit to UnitProductionSystem.

## Browser Reference

Absent -- rally point system not implemented in simulation.js. Implemented from canonical production design.

## Canon Reference

`CLAUDE.md` section: "Shipping game modes are skirmish vs AI and multiplayer only" -- rally point is standard RTS UX for skirmish/multiplayer.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Combat/RallyPointComponent.cs` -- `RallyPointComponent` (per-building: TargetPosition float3, IsActive bool); `PlayerRallyPointSetRequestComponent` one-shot (TargetPosition, IsActive)
- `unity/Assets/_Bloodlines/Code/Combat/RallyPointSetSystem.cs` -- consumes `PlayerRallyPointSetRequestComponent`, writes `RallyPointComponent` (lazy-added), removes request
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.RallyPoint.cs` -- TryDebugSetRallyPoint, TryDebugClearRallyPoint, TryDebugGetRallyPoint (all accept Entity for building)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesRallyPointSmokeValidation.cs` -- 3-phase smoke: struct initialization, set/clear request round-trip, spawn resolution logic
- `scripts/Invoke-BloodlinesUnityRallyPointSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 4 new .cs files
- `unity/Assembly-CSharp.csproj` -- 3 new Compile entries (RallyPoint*.cs, debug surface partial)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry (BloodlinesRallyPointSmokeValidation.cs)

**Narrow edit: UnitProductionSystem.cs** -- added `using Bloodlines.Combat;`; in the spawn loop, reads `RallyPointComponent` from the building entity before `SpawnQueuedUnit`; `SpawnQueuedUnit` gains `hasActiveRallyPoint` and `rallyPosition` optional parameters; `MoveCommandComponent` destination is set to `rallyPosition` with `StoppingDistance=0.5f` and `IsActive=true` when active, otherwise defaults to spawn offset.

**Cross-lane bug fixes caught at build gate:**
- `BloodlinesDebugCommandSurface.SiegeEscalation.cs` -- `Unity.Collections.ComponentType` → `ComponentType` (CS0234)
- `DynastyXPAwardSystem.cs` -- `Unity.Collections.ComponentType` → `ComponentType` (CS0234); `math.min(byte, byte)` → `(byte)math.min((int)..., (int)...)` (CS0121)
- `DynastyProgressionCanon.cs` -- `math.min(byte, byte)` → `(byte)math.min((int)..., (int)...)` (CS0121)
- `DynastyProgressionUnlockSystem.cs` -- `Unity.Collections.ComponentType` → `ComponentType` (CS0234)

## Scope Discipline

- Did not implement rally point UI (flag/marker rendering is a graphics lane concern)
- Did not add rally point to AI systems (AI lane may set `PlayerRallyPointSetRequestComponent` independently)
- Did not implement rally point persistence across sessions
- Did not modify any other production or movement systems

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (CS0006 Library-absent only, no code errors after cross-lane fixes)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (CS0006 Library-absent only)
3-8. Unity batch-mode smoke validators -- SKIP-env (Unity Library not present at this checkout)
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` -- PASS (revision=122, last-updated=2026-04-23)

## Current Readiness

Merged to master. All gates green (Unity batch-mode smokes SKIP-env per established environment condition).

## Next Action

Check `CODEX_MULTI_DAY_DIRECTIVE_2026-04-25.md` for Priority 4+ items; or if no further directives are pending, update NEXT_SESSION_HANDOFF.md and PROJECT_STATE.json to reflect P2+P3 completion.
