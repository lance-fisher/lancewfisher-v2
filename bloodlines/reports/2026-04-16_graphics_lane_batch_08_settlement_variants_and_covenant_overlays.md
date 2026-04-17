# Bloodlines Graphics Lane Batch 08 Settlement Variants And Covenant Overlays

Date: 2026-04-16
Stage: `first_pass_concept`
Review outcome: `placeholder_only`
Scope: House overlay logic, trade and storage variants, denser housing compositions, and canonical covenant overlay architecture rules

## Purpose

This batch continues the additive Bloodlines graphics lane by deepening settlement-support and faith-overlay production usefulness after Batch 07.

It pushes the project closer to reviewable in-engine settlement-family coverage by adding first-pass concept boards for:

- House overlay treatment on support structures
- market and trade yard variants
- storehouse and granary variants
- housing cluster and courtyard variants
- canonical covenant overlay architecture variants

## Files Added

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_house_overlay_support_structures_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_market_and_trade_yard_variants_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_storehouse_and_granary_variants_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_housing_cluster_and_courtyard_variants_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_covenant_overlay_architecture_variants_sheet_a_first_pass_concept_v001.svg`

## Review Surface Updates

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`

The governed external review surface now includes eight current graphics batches.

## Unity Staging Updates

- The five new Batch 08 SVG source sheets were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- Browser-first raster export was used again to keep `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/` current without forcing the already-open Unity project closed.
- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` was extended so `MaterialLookdev_Testbed.unity` can stage a new settlement-variant board wall for:
  - House overlay support structures
  - market and trade yard variants
  - storehouse and granary variants
  - housing cluster and courtyard variants
  - covenant overlay architecture variants

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-runtime-obj-3/ -p:OutputPath=unity/Temp/codex-runtime-bin-3/`
  - passed with `0` warnings and `0` errors
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-editor-obj-3/ -p:OutputPath=unity/Temp/codex-editor-bin-3/`
  - passed with `0` errors
  - longstanding editor/importer `CS0649` warnings remain
- Batch 08 PNG review boards were generated directly into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/` from local headless Edge against the staged SVG source sheets.

## What Remains Pending

- The governed Unity populate pass has not yet been rerun after the Batch 08 `MaterialLookdev` helper extension because another Unity instance still has the canonical project open.
- The next safe in-editor step is:
  - `Bloodlines -> Graphics -> Populate Visual Testbed Scenes`

## What This Pass Left Untouched

This pass did not:

- promote any staged board into runtime art folders
- rename or delete older graphics-lane source boards
- alter gameplay systems, House canon, lore canon, or render-pipeline choices
- force-close the canonical Unity project

## Next Graphics-Lane Recommendations

- review Batches 01 through 08 together using the governed preview and review matrix
- rerun testbed population from the open Unity editor so the Batch 07 `VisualReadability` wall and Batch 08 `MaterialLookdev` wall become visible on disk
- continue settlement-support follow-ups through House-specific civic overlays, water-infrastructure replacements, and trade-hub upgrade-state boards
- continue faith-building follow-ups through covenant-specific sacred-complex variants only after the shared covenant overlay board is reviewed
