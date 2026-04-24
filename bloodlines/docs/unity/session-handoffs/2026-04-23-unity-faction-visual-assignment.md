# Unity Slice Handoff: faction-visual-assignment / faction color and dynasty emblem assignment

**Date:** 2026-04-23
**Lane:** faction-visual-assignment
**Branch:** claude/unity-faction-visual-assignment
**Status:** Complete

## Goal

Implement a canonical per-faction visual identity system. Each faction entity receives a
FactionVisualComponent at match startup holding the palette-resolved primary tint and a
canonical dynasty emblem ID (convention: "emblem_" + factionId). Unit entities receive a
UnitFactionColorComponent that caches the FactionId and resolved tint so downstream rendering
and HUD systems can identify faction color without traversing faction entity queries.

## Browser Reference

Absent -- faction visual assignment not in simulation.js. Implemented from canonical faction
identity design (house colors from FactionTintPalette and 06_FACTIONS/FOUNDING_HOUSES.md).

## Canon Reference

`CLAUDE.md` -- UX at Zero Hour / Warcraft III era fidelity; faction color and emblem
differentiation are standard RTS requirements for skirmish vs AI and multiplayer modes.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Components/FactionVisualComponent.cs` -- `FactionVisualComponent` (per-faction: PrimaryTint float4, EmblemId FixedString64Bytes, IsAssigned bool); `UnitFactionColorComponent` (per-unit: FactionId FixedString32Bytes, Tint float4)
- `unity/Assets/_Bloodlines/Code/Systems/FactionVisualAssignmentSystem.cs` -- `[UpdateInGroup(SimulationSystemGroup)]`; lazy-attaches `FactionVisualComponent` to faction entities using `FactionTintPalette.ResolveTint`, EmblemId = "emblem_" + factionId.ToLowerInvariant(); propagates `UnitFactionColorComponent` to unit entities (UnitTypeComponent+FactionComponent) that lack it on each update
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.FactionVisual.cs` -- TryDebugGetFactionVisual(factionId, out tint, out emblemId), TryDebugGetUnitFactionColor(entity, out factionId, out tint)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesFactionVisualAssignmentSmokeValidation.cs` -- 3-phase smoke: palette tint resolution for "player" confirms non-grey house color, emblem ID convention holds for named and unknown factions, UnitFactionColorComponent struct stores FactionId and Tint correctly
- `scripts/Invoke-BloodlinesUnityFactionVisualAssignmentSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `.meta` files for all 4 new .cs files
- `unity/Assembly-CSharp.csproj` -- 3 new Compile entries (FactionVisualComponent.cs, FactionVisualAssignmentSystem.cs, debug partial)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry

## Scope Discipline

- Did not replace FactionTintComponent (per-entity tint already used by BloodlinesVisualPresentationBridge); FactionVisualComponent is additive and complementary
- Did not modify FactionTintPalette (palette is infrastructure-only per its doc comment)
- Did not implement emblem sprite loading (asset registry binding is a graphics lane concern)
- Did not add building faction color propagation (building entities do not use UnitTypeComponent; follow-up scope)

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env
4. Combat smoke -- SKIP-env
5. Scene shells -- SKIP-env
6. Conviction smoke -- SKIP-env
7. Dynasty smoke -- SKIP-env
8. Faith smoke -- SKIP-env
9a. `node tests/data-validation.mjs` -- PASS
9b. `node tests/runtime-bridge.mjs` -- PASS
10. Contract staleness check -- PASS (revision=126)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed with P1 Multiplayer Network Foundation: lane `netcode-foundation`, branch `claude/unity-netcode-foundation`.
