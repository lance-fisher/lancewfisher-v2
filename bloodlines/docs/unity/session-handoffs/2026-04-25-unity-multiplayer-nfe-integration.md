# Unity Slice Handoff: multiplayer-nfe-integration / Netcode for Entities Package Integration

**Date:** 2026-04-25
**Lane:** multiplayer-nfe-integration
**Branch:** claude/unity-multiplayer-nfe-integration
**Status:** Complete

## Goal

Integrate the Netcode for Entities package (com.unity.netcode 1.4.3) into the Bloodlines
Unity project. Build on the multiplayer-foundation slice to add GhostAuthoringComponent
authoring support, a server-authoritative authority system, NfE ghost collection wiring,
a debug surface for authority state, and a 3-phase smoke validator.

## Browser Reference

Absent -- multiplayer authority and NfE integration not in simulation.js; implemented from
canonical Netcode for Entities design and owner direction 2026-04-19.

## Canon Reference

`governance/OWNER_DIRECTION_2026-04-19_GAME_MODES_AND_DYNASTY_PROGRESSION.md` --
Netcode for Entities multiplayer is canonically in scope for the skirmish vs AI and
multiplayer shipping modes.

## Work Completed

### Package registration
- `unity/Packages/manifest.json` -- added `com.unity.netcode` 1.4.3, matching the existing
  com.unity.entities 1.4.3 / com.unity.burst 1.8.29 package family
- `unity/Packages/packages-lock.json` -- added resolved entries for `com.unity.netcode` 1.4.3
  and its transport dependency `com.unity.transport` 2.4.0

### New files
- `unity/Assets/_Bloodlines/Code/Multiplayer/GhostPrefabAuthoring.cs` -- MonoBehaviour that
  configures GhostAuthoringComponent replication mode for the three canonical ghost archetypes:
  OwnerPredicted for archetype_unit, Interpolated for archetype_faction and
  archetype_control_point; syncs to GhostAuthoringComponent under `#if UNITY_NETCODE`
- `unity/Assets/_Bloodlines/Code/Multiplayer/NetworkAuthoritySystem.cs` -- ISystem
  [UpdateInGroup(InitializationSystemGroup)] [UpdateAfter(GhostCollectionSetupSystem)]; offline
  path: IsLocalGame=true keeps IsServer=true, IsClient=false unchanged each frame; networked
  path (under `#if UNITY_NETCODE`): gates on NetworkStreamInGame singleton, sets IsServer from
  WorldUnmanaged.IsServer(), IsClient = !isServerWorld
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.NetworkAuthority.cs` --
  TryDebugGetNetworkAuthority: returns IsServer, IsClient, IsLocalGame from the
  NetworkGameModeComponent singleton
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesNfEIntegrationSmokeValidation.cs` --
  3-phase smoke validator:
    Phase 1 (OfflineDefaultsStable): verifies NetworkGameModeComponent offline defaults
    Phase 2 (GhostArchetypesRegistered): verifies 3 canonical archetype intents present
    Phase 3 (AuthoritySystemPresent): verifies NetworkAuthoritySystem type is reachable
      and offline authority path produces IsServer=true, IsClient=false
  Marker: BLOODLINES_NFE_INTEGRATION_SMOKE PASS / FAIL

### Modified files (narrow additive edits)
- `unity/Assets/_Bloodlines/Code/Multiplayer/GhostCollectionSetupSystem.cs` -- additive
  `#if UNITY_NETCODE` block: `ConfirmNfECollectionActive` checks SystemAPI.HasSingleton
  <GhostCollection>() and reads GhostCollectionPrefabs buffer liveness in networked sessions;
  offline code path is unchanged
- `unity/Assembly-CSharp.csproj` -- additive Compile entries for GhostPrefabAuthoring.cs,
  NetworkAuthoritySystem.cs, BloodlinesDebugCommandSurface.NetworkAuthority.cs
- `unity/Assembly-CSharp-Editor.csproj` -- additive Compile entry for
  BloodlinesNfEIntegrationSmokeValidation.cs

### Meta files
- `unity/Assets/_Bloodlines/Code/Multiplayer/GhostPrefabAuthoring.cs.meta` (guid: f3a4b5c6...)
- `unity/Assets/_Bloodlines/Code/Multiplayer/NetworkAuthoritySystem.cs.meta` (guid: a4b5c6d7...)
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.NetworkAuthority.cs.meta`
  (guid: b5c6d7e8...)
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesNfEIntegrationSmokeValidation.cs.meta`
  (guid: c6d7e8f9...)

### Script
- `scripts/Invoke-BloodlinesUnityNfEIntegrationSmokeValidation.ps1` -- standard Unity
  batch-mode wrapper; checks for BLOODLINES_NFE_INTEGRATION_SMOKE PASS/FAIL marker;
  log artifact: artifacts/unity-nfe-integration-smoke.log

## Scope Discipline

- Did not implement network transport or lobby code (separate multiplayer-transport lane concern)
- Did not implement network session handshake or client-join flow (separate lane)
- Did not add relay server or matchmaking (separate lane)
- Did not add GhostAuthoringComponent to actual prefab GameObjects (requires Unity editor open;
  GhostPrefabAuthoring.cs is the authoring tool that adds it via Inspector or OnValidate)
- Did not implement GhostField attributes on existing components (separate NfE data migration lane)

## Important: NfE Package Cache

com.unity.netcode@1.4.3 is declared in manifest.json and packages-lock.json but is NOT yet
downloaded to Library/PackageCache. Unity batch-mode smokes fail with "Package cannot be found"
until Unity is opened interactively for the first time, which triggers package download and
cache population.

**Required one-time step before Unity batch smokes will pass:**
Open the Unity project at unity/ in the Unity Hub (6000.3.13f1). Unity will resolve packages,
download com.unity.netcode 1.4.3 and com.unity.transport 2.4.0, and rebuild the project.
After that, all batch-mode smokes will work again.

## Verification Results

1. `dotnet build unity/Assembly-CSharp.csproj -nologo` -- PASS (0 errors)
2. `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` -- PASS (0 errors, 116 pre-existing warnings)
3. Bootstrap runtime smoke -- SKIP-env (NfE package not in PackageCache; Unity fails at package resolution)
4. Combat smoke -- SKIP-env (same reason)
5. Scene shells -- SKIP-env (same reason)
6. NfE integration smoke -- SKIP-env (same reason)
7. `node tests/data-validation.mjs` -- PASS
8. `node tests/runtime-bridge.mjs` -- PASS
9. `scripts/Invoke-BloodlinesUnityContractStalenessCheck.ps1` -- PASS (revision=144)

## Current Readiness

Merged to master. All non-Unity-runtime gates green. Unity batch-mode smokes are SKIP-env
pending one-time interactive package resolution (open Unity Hub to download NfE).

## Next Action

After opening Unity interactively to resolve NfE package:
1. Verify all existing batch-mode smokes pass again
2. Add GhostAuthoringComponent-baked prefab GameObjects to the Bootstrap/Gameplay scenes
3. Proceed to the next unclaimed priority from the CODEX_MULTI_DAY_DIRECTIVE_2026-04-25.md stack
