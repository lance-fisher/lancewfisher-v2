# Bloodlines Unity (Production Game Track)

This Unity project is the production-game track for Bloodlines. It targets Unity 6.3 LTS with DOTS / ECS, Burst, Collections, Mathematics, URP, and the new Input System.

## Canonical Root

- Canonical session root: `D:\ProjectsHome\Bloodlines`
- Physical backing path: `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- Active Unity project: `<canonical-root>/unity/`

Historical note: older documents that pointed at `deploy\bloodlines\` as canonical are superseded by the 2026-04-14 single-root governance pass.

## Approved Toolchain

- **Engine:** Unity 6.3 LTS (`6000.3.13f1`) locked in `ProjectSettings/ProjectVersion.txt`
- **Core architecture:** DOTS / ECS (`com.unity.entities` 1.4.3), Burst 1.8.29, Collections currently resolves as 2.6.5 (`manifest.json` still pins `2.5.7`), Mathematics 1.3.3
- **Rendering:** URP 17.3.0
- **Input:** Input System 1.11.2
- **Addressables:** 2.5.0
- **IDE:** Visual Studio Community 2026 at `C:\Program Files\Microsoft Visual Studio\18\Community`, with VS Code as fallback
- **3D:** Blender 5.1
- **VCS:** Git 2.46 + GitHub Desktop
- **Audio (staged):** Wwise not installed yet; `Assets/_Bloodlines/Audio/Middleware/` remains reserved
- **Networking (staged):** Netcode for Entities not yet added to the manifest

Editor-open status:

- First batch open under Unity 6.3 LTS completed on 2026-04-16.
- `Bloodlines -> Import -> Sync JSON Content` completed in batch mode and generated the current ScriptableObject mirror under `Assets/_Bloodlines/Data/*`.

Preserved non-canonical Unity stubs still on disk:

- `<repo>/Bloodlines/`
- `<repo>/unity/My project/`

They remain preserved only. Do not use them as the production Unity lane.

## Asset Layout

See `Assets/_Bloodlines/README.md` for the full structural contract.

```text
Assets/
  _Bloodlines/
    Art/   Audio/   Code/   Data/   Prefabs/   Scenes/
    Materials/   Shaders/   Animation/   Docs/
  DefaultVolumeProfile.asset
  UniversalRenderPipelineGlobalSettings.asset
```

## JSON -> ScriptableObject Bridge

`Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs` reads from `<repo>/data/` and creates or updates ScriptableObject assets in `Assets/_Bloodlines/Data/*`. Invoke via `Bloodlines -> Import -> Sync JSON Content`.

The JSON layer is canonical. The ScriptableObjects in Unity are a mirror, not a fork.

## Reference Simulation

The browser prototype at:

- `<repo>/play.html`
- `<repo>/src/game/*`
- `<repo>/data/*`

...is preserved as the reference simulation. Every Unity implementation pass should trace back to the browser runtime's behavior and the canonical design corpus before it invents new logic.

## Migration Docs

- `<repo>/docs/unity/PHASE_PLAN.md`
- `<repo>/docs/unity/SYSTEM_MAP.md`
- `<repo>/docs/unity/COMPONENT_MAP.md`
- `<repo>/docs/unity/MIGRATION_PLAN.md`
- `<repo>/docs/unity/DATA_PIPELINE.md`

## Direction Of Play

1. **Close-in battlefield feel** - readable, responsive, legible silhouettes, base-building, and unit-production similar in spirit to Command & Conquer: Generals / Zero Hour
2. **Expanded command layer** - more menus, prompts, next-step guidance, dynasty decisions, and system interplay than classic C&C
3. **Scale transition** - support zoom out into a broader theatre-of-war with operational fronts and scaled fog-of-war while preserving continuity with the battlefield view
4. **Bloodline command presence** - top bloodline members must be structurally visible in the UI

## Open Project

```text
Unity Hub -> Open -> D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines\unity
```

Use Unity 6.3 LTS `6000.3.13f1`.

## Immediate Next Unity Targets

1. Run `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` if you want a governed structural preflight for `Bootstrap.unity` and `IronmarkFrontier.unity` before live editor work.
2. Run `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` if you want a governed runtime preflight for the first live shell. It now proves two-deep queue issue, rear-entry queue cancel-and-refund, surviving front-entry `command_hall -> villager` completion, `dwelling` construction completion to `24` population cap, and `barracks -> militia` continuity from a newly completed constructed production building, ending at `11` total buildings, `18` total units, and `8` controlled units. Run Unity wrappers serially against this project, not in parallel.
3. Run `scripts/Invoke-BloodlinesUnitySyncJsonContent.ps1` whenever the canonical JSON mirror should be refreshed before editor work.
4. Open `Scenes/Bootstrap/Bootstrap.unity`, enter Play Mode, and verify that the map bootstrap plus debug presentation bridge create a visible first ECS battlefield shell.
5. Validate the first debug command layer against the spawned shell:
   - left-click single select
   - controlled-building select
   - left-drag box select
   - `1` selects all controlled units
   - `Ctrl+2-5` saves control groups
   - `2-5` recalls control groups
   - `F` frames the current selection or controlled-force fallback
   - `Escape` clears selection
   - right-click issues the new formation-aware move command
6. Validate the first production and construction layers against the spawned shell:
   - production panel visibility on a controlled `command_hall`
   - visible queue rows and cancel-button behavior
   - `command_hall -> villager` two-deep queue issue, rear-entry cancel, refund, and surviving front-entry completion feel
   - queue drain and post-production select-all behavior
   - construction panel visibility when a controlled worker is selected
   - pending-placement mode plus right-click placement for supported structures
   - obstruction feedback when a site is invalid
   - `dwelling` completion and final population-cap increase
   - `barracks` completion plus `militia` training from the newly completed building
7. Validate the new `BloodlinesBattlefieldCameraController` pan, edge-scroll, drag-pan, rotate, zoom, and focus behavior against the spawned shell.
8. Continue system-level implementation from the live foundation: validate the new capture/ownership flow in Play Mode, then continue with broader construction-roster work, construction progress UI, deeper build-placement UX, production from additional newly completed buildings, broader production-roster work, or deeper command-state work. Do not prioritize attack-move until a real combat lane exists.
9. Keep `Assets/_Bloodlines/Data/*` synced from `<repo>/data/*` and do not fork gameplay truth into Unity-only balance data.

## Current ECS Foundation

Authored in `Assets/_Bloodlines/Code/Components/`:

- `PositionComponent`
- `FactionComponent` + `FactionKindComponent`
- `HealthComponent` + `DeadTag`
- `UnitTypeComponent`
- `BuildingTypeComponent`
- `ResourceNodeComponent`
- `ControlPointComponent`
- `SettlementComponent` + `PrimaryKeepTag`
- `CommanderComponent` + `CommanderAtKeepTag`
- `BloodlineMemberComponent`
- `FaithStateComponent` + `FaithExposureElement` + `FaithWardedSettlementTag`
- `ConvictionComponent`
- `RealmConditionComponent` + `RealmCycleConfig`
- `PopulationComponent` + `ResourceStockpileComponent`
- `SiegeEngineStateComponent` + `MantletCoverComponent`
- `MoveCommandComponent` + `MovementStatsComponent`
- `FactionHouseComponent`
- `ProductionFacilityComponent` + `ProductionQueueItemElement`
- `ConstructionStateComponent`

Authored in `Assets/_Bloodlines/Code/Systems/` and `Assets/_Bloodlines/Code/Pathing/`:

- `BloodlinesBootstrapSystem`
- `SkirmishBootstrapSystem`
- `ControlPointCaptureSystem`
- `ControlPointResourceTrickleSystem`
- `UnitProductionSystem`
- `ConstructionSystem`
- `RealmConditionCycleSystem`
- `PopulationGrowthSystem`
- `Bloodlines.Pathing.UnitMovementSystem`
- `Bloodlines.Pathing.PositionToLocalTransformSystem`

`ControlPointCaptureSystem` is the first narrow parity bridge for battlefield territory contention. It already handles non-worker claimants, contested points, capture decay, ownership flips, and stabilized-versus-occupied transitions. Governor, doctrine, commander, and political-event modifiers remain future follow-up.

`BloodlinesDebugCommandSurface` is the first interactive battlefield command shell. It already supports single-select, drag-box selection, `1` select-all, control groups `2` through `5`, `Ctrl+2-5` save, `2-5` recall, `F` frame selection, clear-selection, formation-aware right-click move issuance, and a compact battlefield HUD for the spawned ECS shell. Attack-move remains future follow-up once a real combat lane exists.

`UnitProductionSystem` is the first ECS production resolver. Together with `ProductionFacilityComponent`, the refund-safe `ProductionQueueItemElement`, and the extended debug command surface, the Unity lane can now select a controlled `command_hall`, issue a two-deep `villager` queue, cancel and refund the rear queue entry safely, complete the surviving front entry, and then train `militia` from a newly completed worker-built `barracks` through the governed Bootstrap runtime-smoke lane.

`ConstructionStateComponent` and `ConstructionSystem` are the first ECS construction resolver path. Together with the extended debug command surface, the Unity lane can now select a controlled worker, place supported buildings, advance them from under-construction state to operational state, and prove both `dwelling` completion plus population-cap gain and `barracks` completion plus post-construction production continuity through the governed Bootstrap runtime-smoke lane.

Next systems should extend scene execution, richer selection/input, gathering, smelting fuel, broader construction coverage, combat, richer territory modifiers, faith exposure, dynasty cascade, and scene-driven bootstrap verification.
