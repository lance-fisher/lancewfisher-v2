# Unity Session Handoff - 2026-04-16 - Control-Point Capture Foundation

## Scope

Continue the Unity production lane beyond bootstrap plus territory-yield plumbing by adding the first ECS control-point ownership-and-capture pass.

## Files Added

- `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs`
- `unity/Assets/_Bloodlines/Code/Systems/ControlPointCaptureSystem.cs.meta`

## Files Updated

- `unity/Assets/_Bloodlines/Code/Systems/ControlPointResourceTrickleSystem.cs`
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`
- `unity/README.md`
- `unity/Assets/_Bloodlines/Code/README.md`
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `HANDOFF.md`
- `continuity/PROJECT_STATE.json`

## What Changed

### 1. First ECS capture lane

- `ControlPointCaptureSystem` is now the first Unity runtime bridge for battlefield territory contention.
- Current parity intentionally stays narrow but structurally important:
  - living non-worker units can claim a point
  - mixed claimants mark the point contested
  - contested or empty points decay capture progress
  - owned points stabilize over time
  - stabilized points fall back to occupied when loyalty is driven below the threshold
  - successful claimants flip ownership and reset loyalty into the occupied band

### 2. Correct system ordering

- `ControlPointCaptureSystem` runs before `ControlPointResourceTrickleSystem`.
- Territory yield therefore now depends on the frame's resolved ownership and uncontested-state result rather than stale pre-capture data.

### 3. Unity asset identity repaired

- The new system had no `.meta` file when discovered in this continuation pass.
- `ControlPointCaptureSystem.cs.meta` is now present so Unity refresh/import preserves a stable asset identity.

## Verification

Verified through isolated Codex intermediate/output paths because the canonical Unity project was already open and holding the default `Temp\obj` outputs:

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=...codex-obj... -p:MSBuildProjectExtensionsPath=...codex-obj... -p:OutputPath=...codex-bin...`

Results:

- runtime assembly: `0 warnings`, `0 errors`
- editor assembly: `0 errors`, longstanding `CS0649` warnings remain in existing editor/importer helpers

## Current Blockers

- The canonical Unity project is still already open in another Unity instance, so the new capture flow has not yet been verified in Play Mode.
- No governed scene shell has been executed yet, so spawned factions and units are not yet being exercised live through the capture path.
- Doctrine, governor, commander, and political-event modifiers for territory capture remain future follow-up rather than part of this first parity slice.

## Next Exact Steps

1. In the already-open Unity 6.3 editor, run `Bloodlines -> Scenes -> Create Bootstrap And Gameplay Scene Shells`.
2. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity`.
3. Enter Play Mode and verify:
   - map bootstrap authoring bakes and spawns entities
   - debug presenter creates visible markers
   - battlefield camera pans, rotates, and zooms correctly
   - uncontested units capture neutral control points
   - opposed units force contested decay
   - captured uncontested points produce trickle income after ownership flips
4. After that validation, continue into:
   - selection/input shell work
   - move-command issuance from the live scene shell
   - richer territory modifiers only after the live first-shell loop is proven
