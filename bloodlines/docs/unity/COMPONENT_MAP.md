# Bloodlines Browser to DOTS Component Map

This file translates the current browser simulation state into Unity Entities terms. It is grounded in the browser runtime that currently lives in:

- `deploy/bloodlines/src/game/core/simulation.js`
- `deploy/bloodlines/src/game/core/ai.js`
- `deploy/bloodlines/src/game/main.js`

## 1. Entity Categories

Primary runtime entity classes:
- Units
- Buildings
- Projectiles
- Resource nodes
- Control points
- Sacred sites
- Faction state entities
- Simulation singleton entities

## 2. Units

| Browser field | DOTS target |
|---|---|
| `unit.id` | Entity identity, plus optional `StableRuntimeId` |
| `unit.factionId` | `FactionId` |
| `unit.typeId` | `UnitTypeId` |
| `unit.x`, `unit.y` | `LocalTransform` |
| `unit.radius` | `CollisionRadius` |
| `unit.health` | `Health` |
| `unit.selected` | `SelectedTag` |
| `unit.command` | `UnitCommand` |
| `unit.attackCooldownRemaining` | `AttackCooldown` |
| `unit.gatherProgress` | `GatherProgress` |
| `unit.carrying` | `CarryState` |
| `unit.targetId` implicit in command | `CommandTargetEntity` or `CommandTargetPosition` |

Expected core components:
- `FactionId`
- `UnitTypeId`
- `LocalTransform`
- `CollisionRadius`
- `MoveSpeed`
- `Health`
- `SightRadius`
- `UnitCommand`
- `SelectionState`
- `AttackProfile`
- `AttackCooldown`
- `CarryState`
- `WorkerStats`
- `CombatStats`

Optional buffers and tags:
- `PathWaypointBuffer`
- `SelectedTag`
- `DeadTag`
- `CommanderAttachment`

## 3. Buildings

| Browser field | DOTS target |
|---|---|
| `building.id` | Entity identity, plus optional `StableRuntimeId` |
| `building.factionId` | `FactionId` |
| `building.typeId` | `BuildingTypeId` |
| `building.tileX`, `building.tileY` | `GridFootprint` plus `LocalTransform` |
| `building.health` | `Health` |
| `building.completed` | `ConstructionState` |
| `building.buildProgress` | `ConstructionState` |
| `building.productionQueue` | `DynamicBuffer<ProductionQueueEntry>` |

Expected building components:
- `FactionId`
- `BuildingTypeId`
- `LocalTransform`
- `GridFootprint`
- `Health`
- `ConstructionState`
- `ProductionQueueState`
- `DropoffProfile`
- `ResourceTrickleProfile`

## 4. Projectiles

Browser projectile state should become lean ECS entities:

| Browser concept | DOTS target |
|---|---|
| projectile position | `LocalTransform` |
| projectile velocity | `ProjectileVelocity` |
| source faction | `FactionId` |
| target entity id | `ProjectileTarget` |
| damage on hit | `ProjectileDamage` |

## 5. World Fixtures

### Resource Nodes

| Browser field | DOTS target |
|---|---|
| `node.id` | `StableRuntimeId` |
| `node.type` | `ResourceNodeType` |
| `node.amount` | `ResourceStock` |
| `node.x`, `node.y` | `LocalTransform` |

### Control Points

| Browser field | DOTS target |
|---|---|
| owner faction | `ControlPointOwnership` |
| contested state | `ControlPointContestState` |
| capture progress | `CaptureProgress` |
| loyalty | `LoyaltyState` |
| position | `LocalTransform` |

### Sacred Sites

| Browser field | DOTS target |
|---|---|
| covenant affinity | `FaithAffinity` |
| discovered-by state | `DynamicBuffer<DiscoveredByFaction>` |
| exposure radius intent | `SacredSiteAura` |

## 6. Faction State Entities

The browser stores faction state in dictionaries under `state.factions`. In DOTS, each faction should be a dedicated entity with buffers for variable-length state.

Core faction components:
- `FactionIdentity`
- `FactionResources`
- `FactionPopulation`
- `FactionFaithState`
- `FactionConvictionState`
- `FactionDynastyState`
- `FactionAIState`

Buffers:
- `DynamicBuffer<ResourceStockEntry>`
- `DynamicBuffer<FaithExposureEntry>`
- `DynamicBuffer<DynastyMemberEntry>`
- `DynamicBuffer<MessageReferenceEntry>` if message provenance is needed

## 7. Singleton / Global State

| Browser state | DOTS target |
|---|---|
| `state.meta.elapsed` | `SimulationTimeSingleton` |
| `state.meta.status` | `MatchStatusSingleton` |
| `state.messages[]` | `DynamicBuffer<MessageEntry>` on a singleton |
| `state.world.width`, `height`, `tileSize` | `WorldMapConfigSingleton` |
| content lookup tables | `BlobAsset` registries or singleton references |

## 8. Dynasty and Faith Bridge

The browser already generates dynasty members and faith exposure data. Unity should preserve those state shapes early so commander, governor, and doctrine work can attach cleanly later.

Early required representations:
- `DynamicBuffer<DynastyMemberEntry>` on faction entity
- `FactionFaithCommitment`
- `DynamicBuffer<FaithExposureEntry>`
- `FactionConvictionLedger`

Future runtime attachments:
- `CommanderAttachment`
- `GovernorAssignment`
- `SacredSiteClaim`
- `DoctrinePathState`

## 9. Design Guardrail

If a browser field exists and materially affects gameplay, it should exist in DOTS before parity is declared. Missing fields are migration debt, not simplification.
