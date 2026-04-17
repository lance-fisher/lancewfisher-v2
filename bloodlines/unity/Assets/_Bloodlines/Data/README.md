# Data

ScriptableObject assets synced from the canonical JSON layer at `<repo>/data/` via `Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`.

## Sync Direction

`<repo>/data/*.json` → `Assets/_Bloodlines/Data/*/*.asset`

Import via the Unity menu: **Bloodlines → Import → Sync JSON Content**.

The JSON side at `<repo>/data/` is **canonical**. The ScriptableObjects here are the Unity mirror, regenerated on import. Do not hand-edit the `.asset` files — edit the JSON and re-sync.

## Folders

| Folder | Source JSON | Notes |
|---|---|---|
| `FactionDefinitions/` | `houses.json` | 9 canonical houses: Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest |
| `ResourceDefinitions/` | `resources.json` | 6 primary (gold, food, water, wood, stone, iron) + influence |
| `UnitDefinitions/` | `units.json` | Roster including siege engines (Ram) with structural multipliers |
| `BuildingDefinitions/` | `buildings.json` | Canonical buildings + fortification class (wall, tower, gatehouse, keep) |
| `FaithDefinitions/` | `faiths.json` | 4 covenants with light/dark doctrine effects |
| `ConvictionDefinitions/` | `conviction-states.json` | 5 bands: apex-moral → apex-cruel |
| `BloodlineRoleDefinitions/` | `bloodline-roles.json` | Head, heir, commander, governor, diplomat, ideological leader, merchant, sorcerer, spymaster |
| `BloodlinePathDefinitions/` | `bloodline-paths.json` | 7 training paths |
| `VictoryConditionDefinitions/` | `victory-conditions.json` | Military, economic, faith, territorial, alliance |
| `MapDefinitions/` | `maps/*.json` | Ironmark Frontier initial map |
| `SettlementClassDefinitions/` | `settlement-classes.json` | 6 canonical classes: border → fortress-citadel |
| `RealmConditionDefinitions/` | `realm-conditions.json` | 90-second realm cycle thresholds + legibility bands |
| `TerrainDefinitions/` | _(future)_ | 10 canonical terrain types (reclaimed plains → sacred ground) |
| `TechDefinitions/` | _(future)_ | Tech/research tree structure |
| `AudioDefinitions/` | _(future)_ | Wwise-prep audio object definitions |
| `DynastyDefinitions/` | _(future)_ | Dynasty archetype definitions for starting leader profiles |
| `ScriptableDefinitions/` | _(meta)_ | Generic reusable definition shells |

## Canonical JSON Layer (Read-Only from Unity's Perspective)

The JSON files under `<repo>/data/` are authoritative. When the browser prototype (`<repo>/play.html`) changes a data field, this Unity project picks up the change on the next `Sync JSON Content` invocation. This is the single-source-of-truth pattern — do not fork the data layer.
