# Unity Session Handoff - 2026-04-16 - Selection And Formation-Move Shell

## Scope

Recover the already-present first Unity battlefield interaction shell into continuity, then improve it so multi-unit move commands preserve basic battlefield readability and the shell supports drag-box selection.

## Files Updated

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`
- `unity/README.md`
- `unity/Assets/_Bloodlines/Code/README.md`
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `HANDOFF.md`
- `continuity/PROJECT_STATE.json`

## Files Classified

- `unity/Assets/_Bloodlines/Code/Components/SelectionComponent.cs`
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesGameplaySceneBootstrap.cs`

## What Changed

### 1. Existing interaction shell recovered into continuity

- `BloodlinesDebugCommandSurface` was already present in the canonical Unity lane and already wired by `BloodlinesGameplaySceneBootstrap`.
- `SelectionComponent.cs` already provides `SelectedTag`, which the command shell uses for live selection state and which the debug presentation bridge already uses for selection highlighting.
- This means the first governed Bootstrap shell is intended to be interactive immediately, not only visible.

### 2. Formation-aware move issuance added

- Multi-unit right-click move commands no longer assign the exact same destination to every selected entity.
- The command shell now:
  - gathers valid selected entities
  - computes a ground-plane forward axis from group centroid toward the clicked destination
  - falls back to camera-forward when the click is too close to the current centroid
  - sorts units into a stable formation order
  - assigns simple row-and-column slots using configurable spacing and max columns
- Current first-shell controls are:
  - left-click single select
  - left-drag box select
  - `1` select all controlled units
  - `Escape` clear selection
  - right-click formation-aware move

### 3. Drag-box selection added

- The debug command shell now tracks left-button drag state and promotes a click into box selection once the pointer moves beyond a small threshold.
- Units inside the screen rectangle can now be selected together without relying only on `1` select-all or repeated single-click selection.
- A simple on-screen rectangle overlay now makes the drag gesture legible while the first shell is still debug-driven.

### 4. Why this matters

- Without this change, the first multi-unit shell would collapse into a single point and hide whether movement or territory control was behaving credibly.
- The move spread plus drag-box selection remain intentionally narrow, but together they are much closer to useful RTS battlefield command feel and make later Play Mode capture verification more readable.

## Verification

Verified through isolated Codex intermediate/output paths because the canonical Unity project was already open and holding the default `Temp\obj` outputs:

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`

Results:

- runtime assembly: `0 warnings`, `0 errors`
- editor assembly: `0 errors`, longstanding `CS0649` debt remains and currently surfaces as 105 warnings

## Current Blockers

- The canonical Unity project is still already open in another Unity instance, so the recovered command shell and its formation-plus-drag-box behavior have not yet been verified in Play Mode.
- Generated Bootstrap and Gameplay scenes still need to be created or re-verified inside that open editor session.
- Attack-move and richer command-state UX remain future follow-up rather than part of this first shell.

## Next Exact Steps

1. In the already-open Unity 6.3 editor, run `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells`.
2. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`.
3. Enter Play Mode and verify:
   - map bootstrap authoring bakes and spawns entities
   - debug presenter creates visible markers
   - battlefield camera pans, rotates, and zooms correctly
   - left-click single select works
   - left-drag box select works
   - `1` selects all controlled units
   - `Escape` clears selection
   - right-click issues formation-aware move destinations
   - uncontested units capture control points
   - contested units force capture decay
   - captured uncontested points produce trickle income
4. After that verification, continue into:
   - attack-move
   - richer command-surface UX only after the first live shell is proven
