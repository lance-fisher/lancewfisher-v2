# Bloodlines Unity U1 Importer Scaffold

Date: 2026-04-13
Author: Codex

## What Landed

Inside the new single-root project path:
- `bloodlines/unity/Assets/_Bloodlines/Code/Definitions/BloodlinesDefinitions.cs`
- `bloodlines/unity/Assets/_Bloodlines/Code/Editor/JsonContentImporter.cs`

## Purpose

This is the first real U1 scaffold after the root consolidation.

It adds:
- ScriptableObject definitions for houses, resources, units, buildings, faiths, conviction states, bloodline roles, bloodline paths, victory conditions, and maps.
- Shared serializable data classes for costs, footprints, map seeds, control points, sacred sites, and faction start state.
- An editor menu action:
  - `Bloodlines/Import/Sync JSON Content`

## Importer Behavior

The importer resolves JSON from:
- `bloodlines/data/`

It upserts assets into:
- `Assets/_Bloodlines/Data/FactionDefinitions`
- `Assets/_Bloodlines/Data/ResourceDefinitions`
- `Assets/_Bloodlines/Data/UnitDefinitions`
- `Assets/_Bloodlines/Data/BuildingDefinitions`
- `Assets/_Bloodlines/Data/FaithDefinitions`
- `Assets/_Bloodlines/Data/ConvictionDefinitions`
- `Assets/_Bloodlines/Data/BloodlineRoleDefinitions`
- `Assets/_Bloodlines/Data/BloodlinePathDefinitions`
- `Assets/_Bloodlines/Data/VictoryConditionDefinitions`
- `Assets/_Bloodlines/Data/MapDefinitions`

## Verification Done

- The consolidated Unity root at `bloodlines/unity/` opens in batch mode strongly enough for Unity to rebuild `Library/`.
- `Packages/manifest.json`, `packages-lock.json`, and `ProjectSettings/` are present in the root project.
- No `error CS` compiler lines or package-resolution failure lines were present in the captured `bloodlines/artifacts/unity-root-open.log` output searched during this session.

## Verification Still Missing

- The importer menu action has not been executed in-editor yet.
- Generated `.meta` files for the new C# source files will be created by Unity on the next full editor import cycle.
- Asset upsert results have not yet been inspected in `Assets/_Bloodlines/Data/`.

## Best Next Step

1. Open `bloodlines/unity/` interactively in Unity.
2. Let Unity finish source import.
3. Run `Bloodlines/Import/Sync JSON Content`.
4. Inspect the generated assets under `Assets/_Bloodlines/Data/`.
5. If the import is clean, write the first U1 verification note and continue into SubScene/bootstrap work.
