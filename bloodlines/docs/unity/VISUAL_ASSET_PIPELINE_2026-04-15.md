# Bloodlines Unity Visual Asset Pipeline

Established: 2026-04-15
Scope: Unity-side staging, import discipline, and approved-art organization for Bloodlines

Companion documents:

- `BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`
- `BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`

## 1. Purpose

This document defines how graphics-lane output enters the Unity production track without disturbing active systems work.

## 2. Rule Of Separation

Do not drop raw concept output directly into final runtime folders.

Use:

- `Assets/_Bloodlines/Art/Staging/`
- `Assets/_Bloodlines/Materials/Staging/`
- `Assets/_Bloodlines/Prefabs/Staging/`

Only approved or integration-ready assets move into runtime-facing category folders.

## 3. Folder Strategy

### Runtime-facing folders

- `Assets/_Bloodlines/Art/Units/`
- `Assets/_Bloodlines/Art/Buildings/`
- `Assets/_Bloodlines/Art/Terrain/`
- `Assets/_Bloodlines/Art/Environment/`
- `Assets/_Bloodlines/Art/Icons/`
- `Assets/_Bloodlines/Art/UI/`
- `Assets/_Bloodlines/Art/Concepts/`

### Staging folders

- `Assets/_Bloodlines/Art/Staging/`
- `Assets/_Bloodlines/Materials/Staging/`
- `Assets/_Bloodlines/Prefabs/Staging/`

### Logical grouping inside approved runtime folders

- `Shared/`
- `Dynasties/<house>/`
- `Faith/<covenant>/`
- `Neutral/`
- `Biomes/<biome>/`

## 4. Naming Conventions

Use the graphics-lane naming system:

`bl_[assetclass]_[lane]_[family]_[variant]_[stage]_v###`

Unity asset names may simplify stage tokens once the asset becomes final, but staging assets keep them.

## 5. Import Guidance

### SVG Concept Sheets In Unity 6.3

- The dedicated graphics lane currently uses deterministic `.svg` concept sheets for first-pass review batches.
- Mirror those files into `Assets/_Bloodlines/Art/Staging/ConceptSheets/` only.
- The approved Unity 6.3 path is now browser-first staged raster export, with Unity mesh raster fallback if browser export is unavailable.
- The raw `.svg` sheets remain the staging source of truth and must not be treated as runtime-ready world art or UI art.
- Do not misclassify mirrored concept sheets as runtime-ready world art or UI art.

### Concept Sheet Sync Menu

- Unity editor path: `Bloodlines -> Graphics -> Sync Concept Sheets`
- Source root: `../14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT`
- Target root: `Assets/_Bloodlines/Art/Staging/ConceptSheets`
- Purpose: keep the Unity-local staging mirror synchronized without manual folder drift.

### Concept Sheet Rasterization Menu

- Unity editor path: `Bloodlines -> Graphics -> Sync And Rasterize Concept Sheets`
- Alternate path: `Bloodlines -> Graphics -> Rasterize Concept Sheets`
- Raster output root: `Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized`
- Preferred engine path: headless local browser screenshot export of the staged SVG sheet at governed pixel dimensions
- Fallback path: Unity mesh rasterization through the built-in `Unity.VectorGraphics` module when browser export is unavailable

### Package Rule For Unity 6.3

- `com.unity.vectorgraphics` is intentionally not installed in this project.
- A dedicated tooling pass on 2026-04-16 confirmed that adding the external package conflicts with Unity 6.3's built-in vector module surface.
- Keep the project on the Bloodlines-local rasterization path instead of reintroducing that package unless a later explicitly approved migration replaces this pipeline.

### Command-Line Entry Point

- Script: `D:/ProjectsHome/Bloodlines/scripts/Invoke-BloodlinesUnityGraphicsRasterize.ps1`
- Default batch method: `Bloodlines.EditorTools.GraphicsConceptSheetVectorImport.RunBatchSyncAndRasterizeConceptSheets`
- Raster-only batch method: `Bloodlines.EditorTools.GraphicsConceptSheetVectorImport.RunBatchRasterizeConceptSheets`
- Default log file: `D:/ProjectsHome/Bloodlines/artifacts/unity-graphics-rasterize.log`
- Optional browser override: pass `-BrowserPath` to the script or set `BLOODLINES_VECTOR_BROWSER_PATH` for the process

### Current 2026-04-16 Staging State

- the governed concept-sheet lane currently contains eight first-pass batches
- all current sheets are mirrored into `Assets/_Bloodlines/Art/Staging/ConceptSheets/`
- all current sheets also exist as generated PNG review boards in `Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`
- the latest refresh added Batch 07 through the approved browser-first raster fallback and added five new Batch 08 settlement-variant boards to staging
- the staging lane now includes Batch 06 boards for:
  - neutral settlement structures
  - faith structure families
  - civic support variants
  - shared foundation materials
  - House trim families
- the staging lane now also includes Batch 07 boards for:
  - market, storehouse, and granary families
  - housing tiers
  - well and water-support structures
  - dock, ferry, and landing families
  - covenant-site progression
- the staging lane now also includes Batch 08 boards for:
  - House overlay support structures
  - market and trade yard variants
  - storehouse and granary variants
  - housing cluster and courtyard variants
  - covenant overlay architecture variants
- the governed Unity testbeds have now been refreshed so:
  - `VisualReadability_Testbed.unity` carries the Batch 07 support-structure wall
  - `MaterialLookdev_Testbed.unity` carries the Batch 08 settlement-variant wall

### Models

- Use consistent real-world scale.
- Keep pivots practical for placement and animation.
- Separate animation-ready rigs from static props.
- Preserve House and faith identifiers in naming, not only in folders.

### Textures

- Shared tiling materials go to `Materials/` and shared texture stores.
- House-specific trims and decals stay separated by House.
- Terrain splats and biome masks stay grouped by biome family.
- Icon textures stay separated from world materials.
- Rasterized concept-sheet PNGs stay in staging and remain review boards, not shipping textures.

### Materials

Group by:

- `Shared`
- `Dynasties`
- `Terrain`
- `UI`
- `FX`

## 6. Placeholder Versus Approved Separation

Placeholder assets remain in staging even if usable in a test scene.

Only move an asset out of staging when:

- the review outcome is `integration_ready`
- the manifest marks the family as a production or final candidate
- scale and readability are proven in-engine

## 7. Prefab Organization

Approved prefab families should resolve into:

- `Prefabs/Units/Shared`
- `Prefabs/Units/Dynasties/<house>`
- `Prefabs/Buildings/Shared`
- `Prefabs/Buildings/Dynasties/<house>`
- `Prefabs/Environment/Biomes/<biome>`
- `Prefabs/UI/<family>`

Raw prefab experiments remain in `Prefabs/Staging/`.

## 8. Dynasty-Based Grouping

All House-specific approved art should group by House before subfamily, not the reverse.

Correct:

- `Art/Units/Dynasties/Ironmark/...`

Incorrect:

- `Art/Units/Axemen/Ironmark/...`

This keeps House identity work coherent for future continuation sessions.

## 9. Terrain And Biome Grouping

Terrain assets should group by biome family first, then by use:

- `Terrain/Biomes/ReclaimedPlains`
- `Terrain/Biomes/StoneHighlands`
- `Terrain/Biomes/FrostRuins`

Supporting props may live in `Environment/Biomes/...` when they are not terrain materials.

## 10. Icons And UI Art

Keep UI-facing art distinct from world-facing art.

- `Art/Icons/Units`
- `Art/Icons/Buildings`
- `Art/Icons/Resources`
- `Art/Icons/Faith`
- `Art/UI/Command`
- `Art/UI/Portraits`

## 11. Version-Safe Staging

Every staging asset should retain:

- source brief id
- stage token
- version number
- review outcome

Never overwrite a reviewed staging asset with a new concept variant under the same filename.

## 12. Safe Add Workflow

1. Confirm manifest entry.
2. Confirm stage label.
3. Drop asset into staging.
4. Record review outcome.
5. Test in engine.
6. Move to approved runtime folder only after `integration_ready`.

## 13. What This Does Not Change

This pipeline does not alter:

- JSON gameplay truth
- importer behavior
- ECS data model
- existing system docs
- active browser-lane implementation work
