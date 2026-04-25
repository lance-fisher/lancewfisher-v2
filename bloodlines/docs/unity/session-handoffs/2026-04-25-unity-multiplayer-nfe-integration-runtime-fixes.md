# Session Handoff: multiplayer-nfe-integration runtime fixes (2026-04-25)

## Goal

Resolve all runtime errors and validation gate failures blocking the NfE integration slice from passing the full Bloodlines validation suite. The prior session added the NfE package and bootstrap override but left four bugs that prevented smokes from completing cleanly.

## Work Completed

### Bug fixes

1. **`BuildTierGatingSystem.cs` — CS1654 compile error**
   - `using var prereqMap` + `prereqMap[key] = bits` triggered CS1654 (cannot modify member of using variable)
   - Fixed: replaced `prereqMap[key] = bits` with `prereqMap.Remove(key); prereqMap.Add(key, bits)`

2. **`DynastyPoliticalEventSystem.cs` — structural change during entity iteration**
   - `EnsureAggregate` called `entityManager.AddComponentData` inside a `SystemAPI.Query<>` foreach loop
   - Fixed: introduced `EntityCommandBuffer` in `OnUpdate`, passed by ref to `EnsureAggregate`, replaced `AddComponentData` with `ecb.AddComponent`, added `ecb.Playback` + `ecb.Dispose` after the loop

3. **`CommanderAuraSystem.cs` — circular dependency (ComponentSystemSorter crash)**
   - 6-system cycle: `FieldWaterStrainSystem → CommanderAuraSystem → UnitMovementSystem → ScoutRaidResolutionSystem → SiegeSupportRefreshSystem → FieldWaterSupportScanSystem → FieldWaterStrainSystem`
   - CommanderAura doesn't consume water strain data — ordering was defensive rather than functional
   - Fixed: removed `[UpdateAfter(typeof(Bloodlines.Siege.FieldWaterStrainSystem))]`

4. **`DynastyRenownLeaderboardHUDSystem.cs` — cross-group ordering violation**
   - `[UpdateAfter(typeof(DynastyRenownHUDSystem))]` on a `PresentationSystemGroup` system where `DynastyRenownHUDSystem` lives in `SimulationSystemGroup`
   - Fixed: removed the cross-group `[UpdateAfter]` attribute

### dotnet build csproj fix

- `Assembly-CSharp.csproj` had 410 explicit Compile includes covering only 410 of the 550 `_Bloodlines` .cs files
- Replaced all explicit entries with `<Compile Include="Assets\_Bloodlines\**\*.cs" />` glob
- Pre-created `Temp\bin\Debug` and `Temp\obj\Assembly-CSharp` directories so dotnet build can write output

## Verification Results

All 9 validation gates pass:

| Gate | Result |
|------|--------|
| `dotnet build unity/Assembly-CSharp.csproj -nologo` | 0 errors |
| `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` | 0 errors |
| Bootstrap runtime smoke | PASS — factions=3/3, buildings=13/11, units=19/18; all proof fields present; no InvalidOperationException |
| Combat smoke | PASS |
| Scene shells | PASS |
| NfE integration smoke | PASS |
| `node tests/data-validation.mjs` | PASS |
| `node tests/runtime-bridge.mjs` | PASS |
| Contract staleness check | PASS (revision=145) |

**Bootstrap proof line (representative):**
```
Bootstrap runtime smoke validation passed for Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity on map ironmark_frontier. Counts: factions=3, buildings=13, units=19, resourceNodes=13, controlPoints=4, settlements=2. ... commandFrameSucceeded=True, productionProgressAdvancementVerified=True, constructionProgressAdvancementVerified=True, gatherDepositObserved=True, trickleGainObserved=True, starvationObserved=True, loyaltyDeclineObserved=True, capPressureObserved=True, aiActivityObserved=True, aiConstructionObserved=True, stabilitySurplusObserved=True, snapshotIntegrityChecked=True, matchProgressionChecked=True, worldPressureChecked=True
```

## Current Readiness

Branch `claude/unity-multiplayer-nfe-integration` is ready to merge. All validation gates green. The NfE integration slice is complete.

Note: Unity's first batch pass may still trigger package resolution (attempting `com.unity.netcode@1.4.3` which Unity tries to find when `com.unity.entities@1.4.3` is active). The second pass uses already-compiled assemblies and passes. This is a known behavior of the packages-lock.json / registry-vs-hash-cache mismatch introduced when NfE was installed from a git hash. Resolves automatically after a normal interactive editor open.

## Next Action

Merge `claude/unity-multiplayer-nfe-integration` to master, then begin the next slice.

## Files Modified

- `unity/Assets/_Bloodlines/Code/Systems/BuildTierGatingSystem.cs`
- `unity/Assets/_Bloodlines/Code/Dynasties/DynastyPoliticalEventSystem.cs`
- `unity/Assets/_Bloodlines/Code/Combat/CommanderAuraSystem.cs`
- `unity/Assets/_Bloodlines/Code/HUD/DynastyRenownLeaderboardHUDSystem.cs`
- `unity/Assembly-CSharp.csproj`
- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` (revision 144 → 145)
- `docs/unity/session-handoffs/2026-04-25-unity-multiplayer-nfe-integration-runtime-fixes.md` (this file)
