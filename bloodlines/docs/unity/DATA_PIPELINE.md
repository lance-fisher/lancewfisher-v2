# Bloodlines Unity Data Pipeline

Unity must consume the existing Bloodlines JSON definitions without creating a second source of gameplay truth.

Authoritative gameplay data source:
- `deploy/bloodlines/data/`

Current files of record:
- `houses.json`
- `resources.json`
- `units.json`
- `buildings.json`
- `faiths.json`
- `conviction-states.json`
- `bloodline-roles.json`
- `bloodline-paths.json`
- `victory-conditions.json`
- `maps/ironmark-frontier.json`

## 1. Pipeline Shape

The migration pipeline is:

```text
JSON in deploy/bloodlines/data/
  -> Unity importer
  -> ScriptableObject definitions
  -> Bakers / bootstrap conversion
  -> ECS entities and BlobAssets
```

## 2. Importer Responsibilities

Importer target:
- `Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`

Importer requirements:
- Read the canonical JSON files from `../data/` relative to the Unity project root.
- Create or update ScriptableObjects in deterministic locations.
- Preserve Unity-only fields such as prefab references, icons, materials, audio clips, and notes.
- Re-run safely after any JSON edit.
- Log created, updated, unchanged, and orphaned assets.

## 3. ScriptableObject Families

Expected definition groups:
- `FactionDefinition`
- `ResourceDefinition`
- `UnitDefinition`
- `BuildingDefinition`
- `FaithDefinition`
- `ConvictionBandDefinition`
- `BloodlineRoleDefinition`
- `BloodlinePathDefinition`
- `VictoryConditionDefinition`
- `MapDefinition`

Expected directories:
- `Assets/_Bloodlines/Data/FactionDefinitions/`
- `Assets/_Bloodlines/Data/UnitDefinitions/`
- `Assets/_Bloodlines/Data/BuildingDefinitions/`
- `Assets/_Bloodlines/Data/FaithDefinitions/`
- `Assets/_Bloodlines/Data/ConvictionDefinitions/`
- `Assets/_Bloodlines/Data/BloodlineRoleDefinitions/`
- `Assets/_Bloodlines/Data/BloodlinePathDefinitions/`
- `Assets/_Bloodlines/Data/VictoryConditionDefinitions/`
- `Assets/_Bloodlines/Data/MapDefinitions/`

## 4. ScriptableObject Rules

Synced fields:
- ids
- names
- costs
- stats
- prototype flags
- house metadata
- covenant metadata
- map positions and ownership seeds

Manual Unity-only fields:
- prefab references
- material references
- sprites and icons
- audio hooks
- VFX hooks
- editor notes

The importer must never wipe manual Unity-only fields just because the JSON does not contain them.

## 5. Bake Strategy

Use two conversion modes:

1. Lookup-only content:
- Convert definitions into BlobAssets or singleton registries for fast, Burst-safe reads.

2. Map-authored runtime entities:
- Use SubScene authoring plus Bakers to convert placed units, buildings, control points, sacred sites, and nodes into ECS entities.

## 6. Map Handling

`ironmark-frontier.json` becomes:
- one `MapDefinition` asset
- one gameplay scene / SubScene using that asset

Map import should preserve:
- world width and height
- tile size
- faction start positions
- control points
- sacred sites
- neutral tribe camp
- resource node layout

## 7. Validation

Importer validation should fail loudly if:
- a referenced faction id does not exist
- a unit or building definition duplicates an id
- a map references a missing unit, building, or faith id
- prototype-required resources are absent

The browser-side `tests/data-validation.mjs` remains useful and should stay part of the workflow even after Unity import exists.

## 8. Round-Trip Discipline

Required working loop:
1. Change JSON in canonical data files.
2. Run importer.
3. Review ScriptableObject updates.
4. Enter Play Mode and verify the imported result.

Avoid this anti-pattern:
- edit Unity assets manually for gameplay numbers
- forget to back-port to JSON
- create silent divergence between browser and Unity

## 9. First Import Targets

U1 should import at minimum:
- all faction definitions
- all unit definitions
- all building definitions
- all faith definitions
- all role/path/conviction/victory definitions
- the Ironmark Frontier map

Nothing downstream should be hand-authored until that import path is working.
