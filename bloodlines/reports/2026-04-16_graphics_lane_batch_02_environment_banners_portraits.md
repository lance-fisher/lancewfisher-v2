# Bloodlines Graphics Lane Batch 02

Date: 2026-04-16
Track: graphics and visual-production lane
Batch: environment, fortification-state, banner, and portrait direction sheets
Stage: `first_pass_concept`
Review outcome: `placeholder_only`

## Summary

This batch extends the same governed first-pass concept lane created in Batch 01.

It adds terrain readability, fortification damage-state logic, House banner hierarchy, and bloodline-role portrait direction without disturbing runtime systems, canon files, or parallel continuation work.

## Files Created In Graphics Staging

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_terrain_shared_biomes_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_fortification_damage_and_breach_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_icon_house_banner_system_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_portrait_bloodline_roles_direction_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`

## Files Added Or Updated In Unity And Unity Documentation

- `unity/Assets/_Bloodlines/Code/Editor/GraphicsConceptSheetSync.cs`
- `unity/Assets/_Bloodlines/Docs/VisualProduction/README.md`
- `unity/Assets/_Bloodlines/Art/Staging/README.md`
- `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
- mirrored concept-sheet files in `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`

## What The Batch Covers

- Top-down terrain and biome readability for:
  - reclaimed plains
  - dry earth and trampled ground
  - river edge and shallow water
  - agricultural terrain
  - stone highlands
  - forest floor
  - marsh and wetland
  - roads and worn paths
- Fortification-state readability for:
  - intact wall
  - cracked wall
  - breached wall
  - intact gatehouse
  - burning or broken gatehouse
  - tower and keep damage read
- House banner hierarchy for all nine canonical Houses using current House names and palette direction from `data/houses.json`
- Portrait-direction lanes for the current bloodline role set from `data/bloodline-roles.json`

## Unity 6.3 Integration Notes

- A Unity editor helper now exists at `Bloodlines -> Graphics -> Sync Concept Sheets`.
- The helper mirrors the governed concept-sheet lane from `../14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT` into `Assets/_Bloodlines/Art/Staging/ConceptSheets`.
- The project manifest does not currently include `com.unity.vectorgraphics`.
- Because of that, the mirrored `.svg` files remain reference and review assets in Unity staging until they are exported to raster or vector import support is explicitly approved in a separate step.

## Constraints Followed

- No existing canon, lore, gameplay, browser runtime, or ECS implementation files were overwritten for design redirection.
- No House names, canonical roles, or settled unique-unit seams were rewritten.
- No final art was claimed.
- No new Unity package dependency was introduced silently.
- No staging art was promoted into final runtime art folders.

## Recommended Next Graphics-Lane Steps

1. Review Batch 01 and Batch 02 together in `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`.
2. Build House-specific unit silhouette sheets for the currently most useful Houses:
   - Ironmark
   - Stonehelm
   - Hartvale
   - one settled-visual-only House for contrast
3. Build a fortification kit decomposition batch:
   - wall corner
   - wall end cap
   - gate insert
   - tower variants
   - rubble blockers
4. Build terrain material follow-up sheets:
   - cliff transitions
   - shoreline transitions
   - resource-node ground treatments
   - biome edge blending
5. If Unity-side visual review needs inline rendering rather than reference-only sheets, export the current SVG sheets to raster review boards or explicitly evaluate vector import support in a dedicated approved tooling pass.
