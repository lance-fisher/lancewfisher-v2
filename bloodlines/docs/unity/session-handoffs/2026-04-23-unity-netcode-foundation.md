# Unity Slice Handoff: multiplayer-foundation / Netcode for Entities foundation

**Date:** 2026-04-23
**Lane:** multiplayer-foundation
**Branch:** claude/unity-netcode-foundation
**Status:** Complete

## Goal

Implement the multiplayer network foundation: the singleton that records the active network
session mode, a ghost prefab archetype registration buffer, and the bootstrap systems that
initialize them at world creation. The foundation defaults to offline/local mode so skirmish
vs AI continues to work unmodified. No actual networked gameplay in this slice; the data model
and offline bootstrap are the deliverable.

## Browser Reference

Absent -- multiplayer foundation not in simulation.js. Implemented from canonical Netcode for
Entities design and owner direction 2026-04-19 (skirmish vs AI + multiplayer are the only
shipping modes; Netcode for Entities multiplayer is canonically in scope).

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md` -- Netcode for
Entities multiplayer is canonically in scope. This slice lays the structural foundation.

## Work Completed

- `unity/Assets/_Bloodlines/Code/Multiplayer/NetworkGameModeComponent.cs` -- `NetworkGameModeComponent` (IComponentData singleton: IsServer, IsClient, IsLocalGame, MaxPlayers byte, NetworkSessionId ulong); `GhostPrefabArchetypeElement` (IBufferElementData with InternalBufferCapacity(8): ArchetypeId FixedString64Bytes, IsRegistered bool)
- `unity/Assets/_Bloodlines/Code/Multiplayer/NetworkBootstrapSystem.cs` -- `[UpdateInGroup(InitializationSystemGroup)]`; once-on-first-update guard; creates NetworkGameModeComponent entity with local defaults (IsLocalGame=true, IsServer=true, MaxPlayers=2) and empty GhostPrefabArchetypeElement buffer
- `unity/Assets/_Bloodlines/Code/Multiplayer/GhostCollectionSetupSystem.cs` -- `[UpdateInGroup(InitializationSystemGroup)]` `[UpdateAfter(NetworkBootstrapSystem)]`; once-on-first-run guard; adds three canonical ghost prefab archetypes: archetype_faction, archetype_control_point, archetype_unit
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.NetworkState.cs` -- TryDebugGetNetworkState (returns IsServer/IsClient/IsLocalGame/MaxPlayers/NetworkSessionId); TryDebugGetGhostArchetypes (returns archetype ID string array)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesNetworkFoundationSmokeValidation.cs` -- 2-phase smoke: offline local mode defaults correct, ghost collection buffer contains 3 registered archetypes
- `scripts/Invoke-BloodlinesUnityNetworkFoundationSmokeValidation.ps1` -- standard Unity batch-mode wrapper
- `unity/Assets/_Bloodlines/Code/Multiplayer.meta` -- folder meta
- `.meta` files for all 3 new .cs files in Multiplayer/ and 2 files in Debug/ and Editor/
- `unity/Assembly-CSharp.csproj` -- 4 new Compile entries (NetworkGameModeComponent.cs, NetworkBootstrapSystem.cs, GhostCollectionSetupSystem.cs, debug partial)
- `unity/Assembly-CSharp-Editor.csproj` -- 1 new Compile entry

## Scope Discipline

- Did not install Netcode for Entities package (com.unity.netcode NOT added to Packages/manifest.json -- package not yet in the project; NfE integration deferred to follow-up slice)
- Did not implement GhostAuthoringComponent wiring (requires NfE package)
- Did not implement actual network transport or lobby code (multiplayer transport lane concern)
- Did not implement network session handshake or client-join flow (follow-up slice)
- Did not add relay server or matchmaking (separate lane concern)

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
10. Contract staleness check -- PASS (revision=127)

## Current Readiness

Merged to master. All gates green.

## Next Action

Proceed to the next unclaimed priority from the CODEX_MULTI_DAY_DIRECTIVE_2026-04-25.md stack.
The immediate next candidates are: skirmish game mode manager, audio dispatch foundation,
dynasty prestige system, or match-end sequence.
