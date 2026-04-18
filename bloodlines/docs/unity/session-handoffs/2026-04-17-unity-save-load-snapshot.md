# 2026-04-17 Unity Save-Load Snapshot (state-snapshot-restore lane)

## Goal

Implement ECS save/load snapshot infrastructure: capture all live faction-scoped
simulation state into a serializable payload, restore it into a fresh world with
full component fidelity, and prove both directions via governed smoke validation.
Browser specification: simulation.js:13822 exportStateSnapshot and
simulation.js:13989 restoreStateSnapshot.

## Work Completed

### Sub-slice 1: Snapshot capture

- `unity/Assets/_Bloodlines/Code/SaveLoad/BloodlinesSnapshotPayload.cs`
  - `BloodlinesSnapshotPayload` class (schema v1), 11 `List<T>` fields:
    FactionSnapshots, LoyaltySnapshots, ResourceSnapshots, RealmConditionSnapshots,
    ConvictionSnapshots, DynastyStateSnapshots, DynastyMemberSnapshots,
    FallenLedgerSnapshots, FaithStateSnapshots, FaithExposureSnapshots,
    PopulationSnapshots.
  - 11 `[Serializable]` snapshot record classes with string FactionId keys.
  - `JsonUtility`-compatible (all public fields, Unity serialization contract).

- `unity/Assets/_Bloodlines/Code/SaveLoad/BloodlinesSnapshotWriter.cs`
  - Static `Capture(EntityManager, out BloodlinesSnapshotPayload)`.
  - Faction query excludes `DynastyMemberComponent` so member entities (which
    also carry FactionComponent) are not treated as factions.
  - `Serialize` / `Deserialize` via `JsonUtility.ToJson` / `FromJson`.

- `unity/Assets/_Bloodlines/Code/Components/SnapshotVersionComponent.cs`
  - `public struct SnapshotVersionComponent : IComponentData { public int Version; }`

- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.SaveLoad.cs`
  - `TryDebugCaptureSnapshot(out string json)` and
    `TryDebugGetSnapshotPayload(out BloodlinesSnapshotPayload payload)`.

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesSaveLoadSmokeValidation.cs`
  - Phases 1-3: empty world, single faction, full faction (conviction + dynasty
    8 members + fallen ledger + faith + exposure).

- `scripts/Invoke-BloodlinesUnitySaveLoadSmokeValidation.ps1` -- PS1 wrapper.

### Sub-slice 2: Restore and round-trip

- `unity/Assets/_Bloodlines/Code/SaveLoad/BloodlinesSnapshotRestorer.cs`
  - `Apply(EntityManager, BloodlinesSnapshotPayload, out error)` creates faction
    entities keyed by FactionId string, restores all companion components,
    re-creates dynasty member entities as separate entities with `DynastyMemberRef`
    buffer links.

- Extended `BloodlinesSaveLoadSmokeValidation.cs` with phases 4-6:
  - Phase 4: round-trip deep-equal (all list counts match after capture/restore/capture).
  - Phase 5: conviction band=Cruel and ruthlessness=10 correct after restore.
  - Phase 6: dynasty member count=8 and legitimacy=85 correct after restore.

### Sub-slice 3: Bootstrap runtime snapshot-integrity probe

- Narrow additive edit to `BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - Added `using Bloodlines.SaveLoad`.
  - Added `snapshotIntegrityChecked` field to `RuntimeSmokeState`.
  - Added `ProbeSnapshotIntegrity` method: after stabilitySurplus phase,
    calls `TryDebugGetSnapshotPayload`, asserts FactionSnapshots.Count > 0,
    RealmConditionSnapshots present, ConvictionSnapshots present.
  - Added diagnostic field `snapshotIntegrityChecked` to pass-line output.

## Verification Results

All 10 validation gates green:
1. `dotnet build Assembly-CSharp.csproj` -- 0 errors
2. `dotnet build Assembly-CSharp-Editor.csproj` -- 0 errors
3. Bootstrap runtime smoke -- passed, snapshotIntegrityChecked=True
4. Combat smoke -- passed (all 8 phases)
5. Scene shells -- passed (Bootstrap + Gameplay)
6. `node tests/data-validation.mjs` -- exit 0
7. `node tests/runtime-bridge.mjs` -- exit 0
8. Contract staleness check -- passed (revision=4, 2026-04-17)
9. Save-load smoke -- passed (all 6 phases)
10. Combat smoke (re-run after sub-slice 2) -- passed

Save-load smoke pass line:
`Save-load smoke validation passed: emptyWorldPhase=True, singleFactionPhase=True,
fullFactionPhase=True, roundTripPhase=True, convictionRestorePhase=True, dynastyRestorePhase=True.`

Bootstrap pass line (excerpt):
`snapshotIntegrityChecked=True`

## Current Readiness

Lane complete. All 3 sub-slices merged to master. The snapshot system is production-ready
for schema v1 scope (faction-level ECS components). V2 gaps documented in
BloodlinesSnapshotPayload.cs: units, buildings, control points, marriages, holy wars,
captives, lesser houses, intelligence dossiers.

## Next Action

Lane closed. Update CONCURRENT_SESSION_CONTRACT.md: move state-snapshot-restore from
Active Lanes to Completed Lanes. Next Unity work targets any unblocked Tier 1 lane from
the contract.

## Browser Reference

- simulation.js:13822 `exportStateSnapshot` -- capture
- simulation.js:13989 `restoreStateSnapshot` -- restore
