# Unity Movement Foundation - 2026-04-16

## Scope

This pass advanced the Unity production lane with the first reusable ECS movement foundation and a continuity cleanup around preserved template confusion.

## Changes Landed

### ECS movement foundation

- Added `MoveCommandComponent` under `Assets/_Bloodlines/Code/Components/`.
- Added `MovementStatsComponent` under `Assets/_Bloodlines/Code/Components/`.
- Added `Bloodlines.Pathing.UnitMovementSystem` under `Assets/_Bloodlines/Code/Pathing/`.
- Added `Bloodlines.Pathing.PositionToLocalTransformSystem` under `Assets/_Bloodlines/Code/Pathing/`.

The intent is deliberately narrow and production-safe:

- keep browser-aligned move-order semantics
- avoid inventing a pathfinding stack before the command contract exists
- give future authoring and spawn work a stable movement target to feed
- keep ECS simulation position and Unity transforms aligned for hybrid presentation

### Unity continuity cleanup

- Rewrote `unity/README.md` to reflect the actual locked editor, actual package versions, completed first-open status, and the current Unity next-step target.
- Rewrote `unity/Assets/_Bloodlines/README.md` and `unity/Assets/_Bloodlines/Code/README.md` to remove stale version drift and document the new movement foundation.
- Added `unity/My project/STUB_TEMPLATE_NOTICE.md` to classify the preserved template project under `unity/` as non-canonical.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `dotnet build D:\ProjectsHome\Bloodlines\unity\Assembly-CSharp.csproj -nologo` passed with `0` warnings and `0` errors.
- `dotnet build D:\ProjectsHome\Bloodlines\unity\Assembly-CSharp-Editor.csproj -nologo` passed with `0` errors. Existing `JsonContentImporter.cs` CS0649 warnings remain, but this pass did not introduce new compile failures.
- Unity batch verification using `Unity.exe -batchmode -projectPath D:\ProjectsHome\Bloodlines\unity -executeMethod Bloodlines.EditorTools.JsonContentImporter.ImportAll` was blocked because another Unity instance already had the canonical project open.

## Immediate Next Unity Target

1. In the already-open Unity 6.3 editor, create `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`.
2. Create `Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity`.
3. Add the first governed authoring and baking layer that seeds faction, unit, building, resource-node, settlement, and control-point entities from `MapDefinitions/ironmark_frontier.asset`.
4. Feed that bootstrap output into the new movement components so move commands can be exercised in Play Mode.
