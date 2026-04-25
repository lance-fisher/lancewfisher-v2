# Bloodlines Unity Naval Layer Slice 1: Embark

- Lane: `naval-layer`
- Branch: `claude/unity-naval-layer`
- Session: claude-naval-2026-04-25
- Date: 2026-04-25

## Goal

Land the first slice of the Bloodlines Unity naval layer: passenger embarkation onto a transport vessel. After this slice the simulation can accept an embark order on a friendly transport, validate it (capacity, faction, adjacency, vessel-vs-land), append the passenger to a transport-owned buffer, and tag the passenger as simulation-inactive so movement and gather systems leave it alone.

This slice is the foundation that S2 (disembark) and S3 (fire-ship detonation) will build on. It is intentionally small in surface area and silent on rejection (matching the browser runtime's quiet ignore behavior on capacity, faction, and adjacency failures).

## Work Completed

### New lane files (owned)

- `unity/Assets/_Bloodlines/Code/Naval/VesselClass.cs` -- canonical six-class vessel enum (Fishing, Scout, WarGalley, Transport, FireShip, CapitalShip).
- `unity/Assets/_Bloodlines/Code/Naval/NavalCanon.cs` -- single point of truth for naval magic numbers (embark radius multiplier 2.5, default transport capacity 6, validation default tile size).
- `unity/Assets/_Bloodlines/Code/Naval/NavalVesselComponent.cs` -- IComponentData identifying a vessel and carrying its class, transport capacity, and one-use-sacrifice flag.
- `unity/Assets/_Bloodlines/Code/Naval/EmbarkOrderComponent.cs` -- transient order issued by the command surface, consumed by EmbarkSystem, removed within the same tick.
- `unity/Assets/_Bloodlines/Code/Naval/PassengerBufferElement.cs` -- IBufferElementData, one entry per embarked passenger held on the transport entity.
- `unity/Assets/_Bloodlines/Code/Naval/EmbarkedPassengerTag.cs` -- zero-size tag marking a passenger as simulation-inactive while embarked.
- `unity/Assets/_Bloodlines/Code/Naval/PassengerTransportLinkComponent.cs` -- reverse pointer from passenger to transport for O(1) disembark and detonation lookups.
- `unity/Assets/_Bloodlines/Code/Naval/EmbarkSystem.cs` -- ISystem under SimulationSystemGroup. Validates each EmbarkOrderComponent against the target transport, commits passenger admissions through ECB, suppresses MoveCommandComponent, and tracks per-transport in-flight admissions so a single tick cannot overshoot capacity.

### New owned editor + script

- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesNavalSmokeValidation.cs` -- isolated test world that proves the embark-phase contract.
- `scripts/Invoke-BloodlinesUnityNavalSmokeValidation.ps1` -- batch wrapper modeled on the combat smoke wrapper.

### Shared-file narrow edits

All edits are additive. No existing values renamed, removed, reordered, or behaviorally changed.

- `unity/Assets/_Bloodlines/Code/Components/UnitTypeComponent.cs` -- appended `Vessel = 10` to `UnitRole`.
- `unity/Assets/_Bloodlines/Code/Components/MapBootstrapComponents.cs` -- appended `VesselClassId`, `TransportCapacity`, `OneUseSacrifice` to `MapUnitSeedElement`.
- `unity/Assets/_Bloodlines/Code/Definitions/UnitDefinition.cs` -- appended `vesselClass`, `transportCapacity`, `oneUseSacrifice` authored fields.
- `unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` -- appended same fields to `UnitRecord`, importer assignment, and `"vessel" -> UnitRole.Vessel` mapping.
- `unity/Assets/_Bloodlines/Code/Authoring/BloodlinesMapBootstrapAuthoring.cs` -- appended `"vessel"` mapping in the role resolver.
- `unity/Assets/_Bloodlines/Code/Editor/BloodlinesMapBootstrapBaker.cs` -- same mapping; baker now emits the three vessel fields into `MapUnitSeedElement`.
- `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs` -- after the existing projectile-factory branch, attach `NavalVesselComponent` and `PassengerBufferElement` buffer when `seed.Role == UnitRole.Vessel`. Existing land-unit spawn behavior unchanged.
- `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs` -- appended `"vessel"` mapping in the role resolver.
- `unity/Assembly-CSharp.csproj` -- added Compile entries for the eight new Naval/*.cs files.
- `unity/Assembly-CSharp-Editor.csproj` -- added Compile entry for `BloodlinesNavalSmokeValidation.cs`.

### Contract update

- `docs/unity/CONCURRENT_SESSION_CONTRACT.md` -- bumped Revision from 145 to 146, Last Updated 2026-04-25, Last Updated By `claude-naval-2026-04-25`. Added `Lane: naval-layer` subsection with full slice roadmap. Added `UnitTypeComponent.cs`, `MapBootstrapComponents.cs`, and `UnitDefinition.cs` to the Shared Files list with append-only enum/field rules.

## Verification Results

All 10 gates of the canonical Unity validation chain green:

| Gate | Result | Evidence |
|------|--------|----------|
| 1. `dotnet build Assembly-CSharp.csproj` | PASS | 0 errors, 0 warnings |
| 2. `dotnet build Assembly-CSharp-Editor.csproj` | PASS | 0 errors (123 pre-existing CS0649 warnings unchanged) |
| 3. Bootstrap runtime smoke | PASS | `Bootstrap runtime smoke validation passed` |
| 4. Combat smoke | PASS | `Combat smoke validation passed` |
| 5. Canonical scene shells | PASS | `Bloodlines canonical Unity scene-shell validation sequence completed successfully` |
| 6. Fortification smoke | PASS | `Fortification smoke validation passed` |
| 7. Siege smoke | PASS | `Siege smoke validation passed` |
| 8. `node tests/data-validation.mjs` | PASS | exit 0 |
| 9. `node tests/runtime-bridge.mjs` | PASS | exit 0 |
| 10. Naval smoke (NEW) | PASS | `Naval smoke validation passed: embarkPhase=True` |
| 11. Contract staleness check | PASS | `STALENESS CHECK PASSED: Contract revision=146` |

Naval smoke embark-phase line:

```
Naval smoke validation embark phase passed: passengerCount=6, embarkRejectedOverCapacity=1, embarkRejectedForeign=1, embarkRejectedOutOfRange=1, embarkedMovementSuppressed=True, elapsedSeconds=0.6.
```

The smoke proves all four contracts of S1: (a) capacity clamps at 6 even when 7 valid orders fire on the same tick, (b) cross-faction order rejected silently, (c) out-of-range order rejected silently, (d) embarked passengers stay put when issued an explicit move command.

## Browser Reference Anchors

- `src/game/core/simulation.js` `embarkUnitsOnTransport` (~7539-7574) -- canonical embark behavior, capacity gate, faction gate, distance gate.
- `src/game/core/simulation.js` `disembarkTransport` (~7576-7623) -- S2 target.
- `src/game/core/simulation.js` `isWaterTileAt` (~7627-7636) -- S2 target.
- `src/game/core/simulation.js` `updateVessel` (~8781-8864) -- S3 (fire ship), S4 (naval combat), S5 (fishing) targets.
- `data/units.json` `transport_ship.transportCapacity = 6`, `fire_ship.oneUseSacrifice = true` -- canonical numbers.

## Open Questions

1. **Embark radius source of truth.** S1 reads `MapBootstrapConfigComponent.TileSize` and falls back to `NavalCanon.ValidationDefaultTileSize = 1` when no config exists. Production worlds always have a config; validation worlds do not. If we ever spawn a vessel inside a validation world that does include a MapBootstrap config, the radius computation will scale. Defensible default for S1.

2. **Embarked-passenger query exclusion strategy.** S1 tags embarked passengers with `EmbarkedPassengerTag` and flips `MoveCommandComponent.IsActive=false`. The smoke proves embarked passengers do not drift even when ordered. We have not yet added `.WithNone<EmbarkedPassengerTag>()` to other lanes' queries (combat acquisition, gather, group movement). Decision: revisit only if a future smoke catches a leak. Adding the filter cross-lane would require coordinated edits and is deferred.

3. **What happens if the transport dies while a passenger is embarked?** S1 leaves the passenger tagged inactive. There is no DeathResolutionSystem-aware cleanup yet. S3 (fire-ship detonation) will introduce the canonical "passengers die with vessel" semantics, since fire ships are the most relevant vessel-death case. Transport-ship-dies-while-loaded edge case can be revisited there.

4. **Cross-vessel embarkation.** S1 explicitly rejects embarking a vessel onto another vessel. Browser runtime has the same guard. No design pressure to allow it.

## Current Readiness

- Lane branch `claude/unity-naval-layer` is ahead of `origin/master` by one slice commit (this one).
- All 10 canonical validation gates green.
- Contract is at revision 146; last-updated 2026-04-25.
- No state-document edits made in this slice (those happen after rebase per protocol).
- Lane is ready for S2 disembark.

## Next Action

S2 disembark slice. Targets:

- Bake terrain water tiles from `MapDefinition.terrainPatches` into a `MapWaterTileSeedElement` buffer (shared-file narrow edit to `MapBootstrapComponents.cs` + baker).
- New components: `DisembarkOrderComponent`, `DisembarkSystem`.
- Browser parity: shoreline detection ring around the transport tile, drop passengers on the first adjacent land tile, position offset matches `simulation.js:7587-7615`.
- Append a disembark phase to `BloodlinesNavalSmokeValidation`. Smoke proves: (a) successful drop on adjacent land tile, (b) failure when transport is in open water with no adjacent land, (c) embarked tag and link removed on drop, (d) passenger MoveCommand left clear so player or AI can issue the next order.

## State Document Updates (Pending Rebase)

After rebase onto `origin/master` and before push:

- Append slice entry to `CURRENT_PROJECT_STATE.md` under naval lane section.
- Append slice entry to `NEXT_SESSION_HANDOFF.md`.
- Append slice entry to `continuity/PROJECT_STATE.json`.
