# Code

DOTS / ECS code root for the Bloodlines production game.

## Sub-Folder Contract

| Folder | Purpose | Typical Contents |
|---|---|---|
| `Components/` | `IComponentData` structs | Pure data - no logic. Per-entity state such as Position, Health, Faction, ResourceNode, MoveCommand, MovementStats, FortificationTier, and SettlementClass. |
| `Systems/` | `ISystem` / `SystemBase` tick logic | Core simulation systems such as realm cycle, population growth, bootstrap spawning, territory-yield trickle, combat, production, and capture. |
| `Aspects/` | `IAspect` accessors | Thin wrappers that compose components for ergonomic queries. |
| `Authoring/` | MonoBehaviour authoring components | Scene-placeable authoring that bakers convert into entities. `BloodlinesMapBootstrapAuthoring` is the first canonical bridge from a scene shell into map-seeded ECS runtime state. |
| `Baking/` | `Baker<T>` implementations | Converts authoring to runtime entities and components. The first bake slice now mirrors `MapDefinition` content into deterministic faction, building, unit, resource-node, settlement, and control-point seed buffers. |
| `AI/` | Faction / opponent / tribe AI | Goal or rules-driven decision layers. Must preserve canonical assault restraint and pressure response. |
| `Pathing/` | Movement planning | `UnitMovementSystem` is the first foundation. Flow-field or richer ECS-native pathing should layer on without changing move-command semantics. |
| `Economy/` | Gold, wood, stone, iron, influence | Resource accumulation, gather-deliver, smelting fuel chain, and trickle. The first territory-yield slice now exists through `ControlPointResourceTrickleSystem`, and it now runs after the first ownership-change pass in `ControlPointCaptureSystem`. |
| `Population/` | Pop cap, growth cycle, famine, water crisis | 90-second canonical realm cycle and its civilizational consequences. |
| `Faith/` | Four covenants, exposure, doctrine, intensity | Faith pressure, doctrine effects, and covenant structure consequence. |
| `Dynasties/` | Bloodline roster, succession, captive ledger | Head, heir, commander, governor, diplomat, ideological leader, merchant, spymaster, sorcerer. |
| `Combat/` | Attack, projectiles, damage resolution | Structural damage multipliers, siege anti-structural behavior, assault cohesion strain. |
| `Construction/` | Building placement, build progress | Fortification tier advancement and governed build behavior. |
| `Camera/` | Battlefield and strategic zoom cameras | `BloodlinesBattlefieldCameraController` is the first battlefield shell: pan, edge-scroll, middle-drag pan, Q/E rotation, and zoom for early Generals-like inspection. |
| `Input/` | Input System actions and command issue | Select, drag, right-click move, attack, build mode, and keybinds. |
| `Networking/` | Netcode for Entities (staged) | Deferred until the multiplayer lane activates. |
| `UI/` | HUD, panels, dashboards | Resource bar, selection, commands, dynasty panel, faith panel, and the 11-state realm-condition dashboard. |
| `SaveLoad/` | Save and resume for long matches | 6-10 hour match continuity is canonical. |
| `Utilities/` | Helpers, math, serialization | Shared helpers and glue code. |
| `Debug/` | Runtime debug overlays | `BloodlinesDebugEntityPresentationBridge` is the first ECS-shell visualization bridge. It is debug-only and must not be mistaken for final rendering architecture. |
| `Editor/` | Editor-only tooling | `JsonContentImporter.cs`, `BloodlinesGameplaySceneBootstrap.cs`, graphics helpers, and other editor-only governed tooling. |
| `Definitions/` | ScriptableObject definitions | `BloodlinesDefinitions.cs` and other runtime-facing definition types mirrored from canonical JSON. |

## Namespace Conventions

- `Bloodlines.Components`
- `Bloodlines.Systems`
- `Bloodlines.Aspects`
- `Bloodlines.Authoring`
- `Bloodlines.Baking`
- `Bloodlines.AI`
- `Bloodlines.Pathing`
- `Bloodlines.Economy`
- `Bloodlines.Population`
- `Bloodlines.Faith`
- `Bloodlines.Dynasties`
- `Bloodlines.Combat`
- `Bloodlines.Construction`
- `Bloodlines.Camera`
- `Bloodlines.Input`
- `Bloodlines.Networking`
- `Bloodlines.UI`
- `Bloodlines.SaveLoad`
- `Bloodlines.Utilities`
- `Bloodlines.Debug`
- `Bloodlines.EditorTools`
- `Bloodlines.DataDefinitions`

## Active First-Shell Tooling

- `Editor/BloodlinesGameplaySceneBootstrap.cs`
  Creates, repairs, and validates `Bootstrap.unity` and `IronmarkFrontier.unity` scene shells without hand-writing scene YAML.
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`
  Governed Play Mode smoke validator for the Bootstrap scene. It now proves bootstrap spawn counts, debug presentation presence, command-shell select-all, control-group save/recall, selection framing, controlled-building selection, two-deep `command_hall -> villager` queue issuance, rear-entry cancel and refund verification, surviving front-entry completion, worker-led `dwelling` placement, construction-site completion, final population-cap growth, and `barracks -> militia` continuity from a newly completed constructed production building.
- `scripts/Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1`, `scripts/Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1`, and `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1`
  Governed batch entry points for structural scene-shell validation. The per-scene wrappers now rerun once automatically if the first Unity batch pass is consumed by compilation or import work before a validation result is logged.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  Governed batch entry point for runtime smoke validation of the Bootstrap shell. Run Unity wrappers serially against the same project.
- `Camera/BloodlinesBattlefieldCameraController.cs`
  First battlefield camera shell for inspection and early command-feel validation. It now also exposes a focus hook for command-surface reframing.
- `Debug/BloodlinesDebugEntityPresentationBridge.cs`
  First visible ECS-shell debug bridge so spawned entities can be inspected before the production render path exists.
- `Debug/BloodlinesDebugCommandSurface.cs`
  First interactive battlefield command shell for the Unity lane. Supports single-select, controlled-building select, drag-box selection, select-all, clear-selection, control groups `2` through `5`, `Ctrl+2-5` save, `2-5` recall, `F` frame selection, formation-aware right-click move issuance, a compact battlefield HUD, the first building production panel plus queue issue and queue cancel path with per-entry control, and the first worker-aware construction panel with pending-placement mode and right-click placement that now feeds the governed constructed-production continuity lane.

## Active Battlefield State Foundation

- `Systems/SkirmishBootstrapSystem.cs`
  Seeds the first ECS factions, units, buildings, settlements, control points, and resource nodes from the canonical `MapDefinition`.
- `Components/SelectionComponent.cs`
  Provides `SelectedTag` so early battlefield interaction can mark and highlight live ECS selections.
- `Systems/ControlPointCaptureSystem.cs`
  First ownership-and-capture bridge for control points. Handles claimant detection, contested state, progress decay, stabilization, and ownership flips.
- `Systems/ControlPointResourceTrickleSystem.cs`
  First economy bridge that turns owned, uncontested control points into stockpile income.
- `Components/FactionHouseComponent.cs` and `Components/ProductionComponents.cs`
  First house-aware production state for factions and production-capable buildings. `ProductionQueueItemElement` now stores refund-safe queued cost metadata as well as the queued Ironmark blood-price and blood-load values.
- `Systems/UnitProductionSystem.cs`
  First ECS training resolver. It consumes queued production entries and spawns units through `EndSimulationEntityCommandBufferSystem` so the live shell can complete training without structural-change exceptions.
- `Components/ConstructionComponents.cs` and `Systems/ConstructionSystem.cs`
  First ECS construction-progress lane. Buildings can now enter an under-construction state, complete over time, and apply their completion effects, with `dwelling` population-cap gain and `barracks` completion already proven in the governed runtime-smoke path.
- `Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`
  The current governed runtime-smoke lane now proves that the first live shell can spawn correctly, execute the narrow command-shell behaviors that already exist, issue a two-deep `command_hall -> villager` queue, cancel and refund the rear queued production entry while the front entry survives, complete that remaining front-entry production slice, place plus complete a `dwelling` construction site with the expected population-cap increase, and then place plus complete a `barracks` that immediately trains `militia` from the newly completed production seat.


## Rules

1. Components hold data and systems hold logic. Keep that boundary sharp.
2. Use Burst-compatible types for hot paths.
3. Prefer scalable ECS patterns for battlefield loops; large unit counts are canonical.
4. Do not place runtime simulation into `Authoring/` or `Baking/`; those are conversion layers only.
5. Do not couple UI into simulation. UI should read ECS state, not own gameplay truth.
6. Editor-only code stays under `Editor/` so it does not leak into player builds.

## Reference Simulation

The browser prototype at `<repo>/src/game/core/simulation.js` is the canonical runtime behavior reference. The canonical design corpus at `<repo>/01_CANON/` remains the authoritative source of truth for game rules.
