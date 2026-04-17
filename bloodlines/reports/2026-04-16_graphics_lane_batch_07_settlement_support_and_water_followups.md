# Bloodlines Graphics Lane Batch 07 Settlement Support And Water Follow-Ups

Date: 2026-04-16
Stage: `first_pass_concept`
Review outcome: `placeholder_only`
Scope: market-storage support, housing tiers, water infrastructure, river-edge logistics, and covenant-site progression

## Purpose

This batch continues the additive Bloodlines graphics lane by extending core settlement-support and water-support structure coverage.

It pushes the project closer to full RTS environment-family readiness by adding first-pass concept boards for:

- market, storehouse, and granary families
- housing tiers and density escalation
- well and water-support structures
- dock, ferry, landing, and river-logistics structures
- covenant-site progression from wayshrine to apex tier

## Files Added

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_market_storehouse_and_granary_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_housing_tiers_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_well_and_water_support_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_environment_shared_dock_ferry_and_landing_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_covenant_site_progression_sheet_a_first_pass_concept_v001.svg`

## Review Surface Updates

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`

The governed external review surface now includes seven current graphics batches.

## Unity Staging Updates

- The five new Batch 07 SVG source sheets were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` was extended so `VisualReadability_Testbed.unity` can now stage a support-structure board wall for:
  - market or storehouse or granary
  - housing tiers
  - well and water support
  - dock or ferry or landing
  - covenant-site progression

## Verification And Blockers

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-runtime-obj-2/ -p:OutputPath=unity/Temp/codex-runtime-bin-2/`
  - passed with `0` warnings and `0` errors
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-editor-obj-2/ -p:OutputPath=unity/Temp/codex-editor-bin-2/`
  - passed with `0` errors
  - longstanding editor/importer `CS0649` warnings remain
- A new runtime compile blocker was discovered in `BloodlinesDebugCommandSurface.cs` during this pass because a local queue-entry view type had been lost; that helper was restored.
- Unity batch rasterize and populate were then blocked because another Unity instance already had `D:/ProjectsHome/Bloodlines/unity` open.

## Browser-First Raster Fallback

Because Unity batch mode was blocked by the open editor instance, the approved browser-first raster path was used directly through local Edge headless export.

Batch 07 PNG review boards now exist in `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/` for:

- `bl_building_shared_market_storehouse_and_granary_sheet_a_first_pass_concept_v001.png`
- `bl_building_shared_housing_tiers_sheet_a_first_pass_concept_v001.png`
- `bl_building_shared_well_and_water_support_sheet_a_first_pass_concept_v001.png`
- `bl_environment_shared_dock_ferry_and_landing_sheet_a_first_pass_concept_v001.png`
- `bl_building_shared_covenant_site_progression_sheet_a_first_pass_concept_v001.png`

This keeps the Unity review-board lane current without forcing the canonical Unity editor closed.

## What Remains Pending

- The governed testbed populate pass has not yet been rerun after the Batch 07 board-wall helper extension because the canonical Unity project remained open in another instance.
- The next safe in-editor step is:
  - `Bloodlines -> Graphics -> Populate Visual Testbed Scenes`

## What This Pass Left Untouched

This pass did not:

- promote any staged board into runtime art folders
- delete or rename older graphics-lane source boards
- alter gameplay systems, lore, House canon, or ECS production logic
- force-close the open Unity project

## Next Graphics-Lane Recommendations

- review Batches 01 through 07 together using the governed preview and review matrix
- rerun testbed population from the open Unity editor so the new support-structure board wall becomes visible in `VisualReadability_Testbed.unity`
- continue deeper settlement-support follow-ups through granary variants, storehouse variants, housing overlays by House lane, and civic water-infrastructure replacements
- continue faith-building follow-ups with covenant-specific variant sheets only after the generic covenant-site progression board is reviewed
