# Unity Session Handoff - 2026-04-16 - Scene Scaffold, Camera, And Debug Shell

## Scope

Continue the Unity production lane beyond compile-only ECS infrastructure by adding:

- a governed scene-shell creation path
- the first battlefield camera controller
- the first debug-only ECS presentation bridge

## Files Added

- `unity/Assets/_Bloodlines/Code/Camera/BloodlinesBattlefieldCameraController.cs`
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGameplaySceneBootstrap.cs`
- `scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1`

## Files Updated

- `unity/README.md`
- `unity/Assets/_Bloodlines/Code/README.md`
- `unity/Assets/_Bloodlines/Scenes/Bootstrap/README.md`
- `unity/Assets/_Bloodlines/Scenes/Gameplay/README.md`

## What Changed

### 1. Governed scene creation

- `BloodlinesGameplaySceneBootstrap` now adds:
  - `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells`
  - `Bloodlines -> Scenes -> Open Gameplay Scenes Folder`
- The scene bootstrap creates:
  - `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`
  - `Assets/_Bloodlines/Scenes/Gameplay/IronmarkFrontier.unity`
- The Bootstrap scene shell includes:
  - metadata root
  - directional light
  - battlefield camera rig
  - debug entity presentation bridge
  - reference ground
  - `BloodlinesMapBootstrapAuthoring` wired to `Assets/_Bloodlines/Data/MapDefinitions/ironmark_frontier.asset`
- The Gameplay scene shell includes the same scene-support structure except the direct bootstrap authoring anchor, so it remains a clean gameplay shell rather than a duplicate bootstrap source.

### 2. First battlefield camera shell

- `BloodlinesBattlefieldCameraController` is the first controlled battlefield-view rig for the Unity production lane.
- Current controls:
  - `WASD` / arrow keys for pan
  - screen-edge pan
  - middle-mouse drag pan
  - `Q` / `E` rotate
  - mouse-wheel zoom
- It also preserves map bounds and map-start focus configuration from the canonical `MapDefinition`.

### 3. First visible ECS-shell presentation bridge

- `BloodlinesDebugEntityPresentationBridge` is a debug-only MonoBehaviour bridge that turns live ECS entities into simple primitive markers so the first bootstrap shell can be inspected visually before the production render path is in place.
- Presentable shells now include:
  - units
  - buildings
  - settlements
  - control points
  - resource nodes
- This is explicitly debug architecture only. It is not a substitute for the eventual production rendering path.

### 4. CLI wrapper

- `scripts/Invoke-BloodlinesUnityCreateGameplaySceneShells.ps1` now provides a governed batch entry point for future lock-free scene-shell generation.

## Verification

### Unity lane

Verified through isolated Codex intermediate/output paths:

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`

Results:

- runtime assembly: `0 warnings`, `0 errors`
- editor assembly: `0 warnings`, `0 errors`

## Current Blockers

- The canonical Unity project is still already open in another Unity instance, so the new scene-shell tool was compiled but not batch-executed in this pass.
- No actual `Bootstrap.unity` or `IronmarkFrontier.unity` files exist yet until the menu item or wrapper is run from a lock-free Unity session.
- The debug presenter is deliberately temporary. A real production render path still needs to replace it later.

## Next Exact Steps

1. In the already-open Unity 6.3 editor, run `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells`.
2. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`.
3. Enter Play Mode and verify:
   - map bootstrap authoring bakes and spawns entities
   - debug presenter creates visible markers
   - camera rig pans, rotates, and zooms correctly
4. Once that shell is live, decide the next runtime slice in this order:
   - control-point ownership and capture flow
   - construction or production logic
   - camera polish and selection/input shell
   - replacement of debug visualization with an early production render path
