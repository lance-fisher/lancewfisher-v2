# Bloodlines Unity 6.3 Visual Implementation Guide

Established: 2026-04-16
Scope: additive Unity-side guidance for art insertion, staging, grouping, and in-engine readability testing

## 1. Purpose

This guide moves the Bloodlines graphics lane from foundation into practical Unity 6.3 implementation readiness.

It does not replace:

- `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
- `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/*`
- existing gameplay, lore, dynasty, UI, or system documentation

It defines how future art should actually enter and live in the Unity project without colliding with parallel workstreams.

## 2. Current Readiness Classification

### Strong

- visual doctrine and art bible
- House visual identity packs
- asset manifests
- graphics prompt framework
- concept-sheet staging and review surface
- Unity 6.3 SVG mirror and PNG review-board generation
- governed Unity visual testbed scenes with generated staging content

### Partial

- runtime-facing art folder guidance
- material family grouping
- prefab grouping expectations
- visual testbed organization
- per-family unit and building direction packs
- mature review matrix with issue handling by asset family

### Missing Or Intentionally Deferred

- final runtime art assets
- settled material libraries
- production prefabs
- approved House icon families
- final structure-upgrade art chains

## 2A. Current On-Disk Unity State

The graphics lane is no longer only documentation and staging theory.

Current governed Unity 6.3 state already includes:

- mirrored SVG concept sheets for all current batches
- generated PNG review boards for all current batches
- populated testbed scenes under `Assets/_Bloodlines/Scenes/Testbeds/`
- nine-House readability comparisons in `VisualReadability_Testbed.unity`
- shared-versus-House material swatches plus staged board wall in `MaterialLookdev_Testbed.unity`
- terrain strips and board references in `TerrainLookdev_Testbed.unity`
- icon-size contrast rows in `IconLegibility_Testbed.unity`

These remain staging-only and review-only surfaces. They are not approved runtime art or feature scenes.

The graphics lane should advance these as additive layers, not by replacing existing foundations.

## 3. Unity Folder Contract

### Source-of-truth staging lane

- `Assets/_Bloodlines/Art/Staging/ConceptSheets/`
- `Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`
- `Assets/_Bloodlines/Materials/Staging/`
- `Assets/_Bloodlines/Prefabs/Staging/`

Everything in staging remains replaceable.

### Runtime-facing art lane

- `Assets/_Bloodlines/Art/Units/`
- `Assets/_Bloodlines/Art/Buildings/`
- `Assets/_Bloodlines/Art/Terrain/`
- `Assets/_Bloodlines/Art/Environment/`
- `Assets/_Bloodlines/Art/Icons/`
- `Assets/_Bloodlines/Art/UI/`
- `Assets/_Bloodlines/Art/Characters/`
- `Assets/_Bloodlines/Art/FX/`

These folders should receive assets only after review outcome `integration_ready`.

### Runtime-facing material lane

- `Assets/_Bloodlines/Materials/Shared/`
- `Assets/_Bloodlines/Materials/Dynasties/`
- `Assets/_Bloodlines/Materials/Terrain/`
- `Assets/_Bloodlines/Materials/UI/`
- `Assets/_Bloodlines/Materials/FX/`

### Runtime-facing prefab lane

- `Assets/_Bloodlines/Prefabs/Units/`
- `Assets/_Bloodlines/Prefabs/Buildings/`
- `Assets/_Bloodlines/Prefabs/Environment/`
- `Assets/_Bloodlines/Prefabs/UI/`

### Scene-safe visual testbed lane

- `Assets/_Bloodlines/Scenes/Testbeds/`

Use testbeds for readability, material checks, and replacement validation. Do not use them as gameplay-scene dumping grounds.

## 4. Runtime Asset Grouping Rules

### Units

Group approved unit assets by House before class.

Correct:

- `Art/Units/Shared/Infantry/`
- `Art/Units/Dynasties/Ironmark/`
- `Art/Units/Dynasties/Hartvale/`

Incorrect:

- `Art/Units/Axemen/Ironmark/`
- `Art/Units/HeavyInfantry/AllHouses/`

The House read is more important to future continuation than a class-only directory tree.

### Buildings

Group approved buildings by function family first, then by universal or House-specific treatment.

Suggested shape:

- `Art/Buildings/Shared/Civic/`
- `Art/Buildings/Shared/Military/`
- `Art/Buildings/Shared/Fortification/`
- `Art/Buildings/Dynasties/<house>/`
- `Art/Buildings/Faith/<covenant>/`
- `Art/Buildings/Neutral/`

### Terrain

Group by biome family and transition family.

Suggested shape:

- `Art/Terrain/Biomes/ReclaimedPlains/`
- `Art/Terrain/Biomes/StoneHighlands/`
- `Art/Terrain/Biomes/RiverValleys/`
- `Art/Terrain/Transitions/`
- `Art/Terrain/ResourceGround/`
- `Art/Terrain/Roads/`

### Environment

Use this lane for world-setpiece and support props that are not terrain materials.

Suggested shape:

- `Art/Environment/Setpieces/Bridges/`
- `Art/Environment/Setpieces/Waterworks/`
- `Art/Environment/Setpieces/Ruins/`
- `Art/Environment/Logistics/`
- `Art/Environment/Biomes/<biome>/`

### Icons And UI

Keep world art and interface art fully separate.

Suggested shape:

- `Art/Icons/Units/`
- `Art/Icons/Buildings/`
- `Art/Icons/Resources/`
- `Art/Icons/Faith/`
- `Art/Icons/Dynasties/`
- `Art/UI/Command/`
- `Art/UI/Portraits/`
- `Art/UI/Alerts/`

## 5. Material Family Rules

### Shared

Use for common trims, neutral plaster, neutral wood, shared iron, and generic cloth values.

### Dynasties

Use for House-specific color trims, banner cloth, heraldic accent surfaces, and approved dynasty material overrides.

### Terrain

Use for ground, cliff, shoreline, mud, river-edge, road, and resource-ground families.

### UI

Use only for interface materials and atlas-support surfaces.

### FX

Use for flames, smoke, signal-fire, ward, and special readability overlays if later approved.

## 6. Placeholder-To-Final Replacement Contract

Every asset family should move through these stages:

1. concept source
2. raster review board
3. placeholder test asset
4. in-engine readability check
5. approved direction
6. production candidate
7. final integration

Rules:

- never silently replace an asset with a different House read
- keep placeholder filenames stage-tagged
- remove stage tags only after the family is truly final
- preserve old staging artifacts when the next version changes silhouette or faction read

## 7. Visual Testbed Guidance

### Required testbeds

The project should maintain additive scene-safe testbeds for:

- `VisualReadability`
- `TerrainLookdev`
- `MaterialLookdev`
- `IconLegibility`

### VisualReadability

Use this to compare:

- unit class silhouettes
- House distinction at gameplay height
- banner visibility
- fortification and siege readability

### TerrainLookdev

Use this to compare:

- cliff and shoreline transitions
- roads and fords
- resource-ground treatments
- prop clutter levels

### MaterialLookdev

Use this to compare:

- House trim values
- neutral stone, wood, and iron families
- mud, dust, and weathering intensity
- emissive or fire effects kept within readability discipline

### IconLegibility

Use this to compare:

- small-size icon clarity
- alert marker contrast
- resource symbol readability
- dynasty confusion risks

## 8. Import And Review Sequence

For any future asset insertion:

1. confirm the manifest entry
2. confirm the stage label
3. place the source asset in staging
4. generate or mirror the Unity staging representation
5. test in the relevant visual testbed
6. record review outcome
7. move to runtime-facing lane only when `integration_ready`

## 9. Readability Rules For In-Engine Checks

Every in-engine art check should answer:

- can the asset be identified at gameplay height
- can the player tell House or universal ownership quickly
- does the silhouette remain readable against Bloodlines terrain values
- does damage or upgrade state read without clutter
- does the asset look structurally credible
- does prop density interfere with selection or path comprehension

If the answer fails on silhouette or scale, the asset returns to revision before texture or polish work continues.

## 10. What Future Sessions Should Not Do

- do not place raw concept sheets into runtime folders
- do not use `Art/Characters/` as a catch-all overflow lane
- do not create one-off folders per batch inside runtime-facing lanes
- do not mix icon atlases with world textures
- do not put House-specific materials into `Shared`
- do not skip testbed verification before calling an asset integration-ready

## 11. Immediate Safe Expansion Points

The next additive Unity-side graphics improvements should be:

- runtime-facing folder readmes and contracts
- testbed readmes and intended use
- per-family unit and building direction packs linked from the graphics lane
- a stronger visual review matrix tied to Unity test results
- placeholder prefab conventions once actual meshes or sprites begin entering the project
