# Bloodlines Graphics Lane Batch 06 Neutral, Faith, Civic, And Material Boards

Date: 2026-04-16
Stage: `first_pass_concept`
Review outcome: `placeholder_only`
Scope: neutral-settlement, faith-structure, civic-support, and material-family direction boards with Unity 6.3 staging refresh

## Purpose

This batch extends the governed Bloodlines graphics lane beyond House unit studies and terrain follow-ups into the next environment and material families needed for real RTS production planning.

It adds first-pass concept boards for:

- neutral settlement structures
- faith structure families
- civic support variants
- shared foundation material control
- House trim-family overlays

These boards are additive and staging-only. They do not promote any asset into runtime art or alter canon, gameplay rules, or House lore.

## Files Added

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_neutral_settlement_structures_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_faith_structure_families_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_building_shared_civic_support_variants_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_material_shared_foundation_boards_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_material_house_trim_families_sheet_a_first_pass_concept_v001.svg`

## Review Surface Updates

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`

The governed external review surface now includes all six current graphics batches.

## Unity 6.3 Staging Updates

- The five new Batch 06 SVG source sheets were mirrored into `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`.
- The governed Unity raster pipeline was rerun successfully through `scripts/Invoke-BloodlinesUnityGraphicsRasterize.ps1`.
- The raster log confirms: `Generated or updated: 5 | Skipped current: 22`.
- Batch 06 PNG review boards now exist under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.

## Unity Testbed Updates

- `unity/Assets/_Bloodlines/Code/Editor/GraphicsVisualTestbedPopulate.cs` was extended so `MaterialLookdev_Testbed.unity` now stages:
  - neutral settlement board
  - faith structure board
  - civic support board
  - shared foundation material board
  - House trim-family board
- The governed testbed population pass was rerun successfully through `scripts/Invoke-BloodlinesUnityGraphicsPopulateTestbeds.ps1`.
- `unity/Assets/_Bloodlines/Scenes/Testbeds/MaterialLookdev/MaterialLookdev_Testbed.unity` now contains:
  - `NeutralSettlementBoard`
  - `FaithStructureBoard`
  - `CivicSupportBoard`
  - `FoundationBoards`
  - `HouseTrimBoards`

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-runtime-obj/ -p:OutputPath=unity/Temp/codex-runtime-bin/`
  - passed with `0` warnings and `0` errors
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo -p:BaseIntermediateOutputPath=unity/Temp/codex-editor-obj/ -p:OutputPath=unity/Temp/codex-editor-bin/`
  - passed with `0` errors
  - longstanding `JsonContentImporter.cs` and editor-helper `CS0649` warnings remain unchanged in category
- Unity batch logs:
  - `artifacts/unity-graphics-rasterize.log`
  - `artifacts/unity-graphics-testbed-populate.log`

## Additional Continuity Corrections In This Pass

- The graphics lane was re-audited against the actual project state.
- This confirmed that:
  - Batch 05 already completed the full nine-House silhouette-study lane
  - governed Unity testbed scene population had already succeeded on disk
  - continuity files were stale in reporting Batch 04 as current and describing testbed population as pending
- Root continuity and handoff files were updated in this same pass to match the actual graphics-lane state.

## What This Pass Left Untouched

This pass did not:

- move or promote any staged board into runtime art folders
- alter gameplay scenes or ECS logic beyond preserving graphics-tool execution compatibility already required by the lane
- change House canon, unit stats, faith rules, or economy systems
- alter render-pipeline choices
- delete or replace prior graphics-lane reports, prompts, manifests, or source boards

## Next Graphics-Lane Recommendations

- run a review and tagging pass across Batches 01 through 06 using `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/VISUAL_REVIEW_MATRIX_2026-04-16.md`
- deepen neutral and faith structure follow-up boards into dynasty-adjacent replacement candidates
- expand the civic lane through market, storehouse, granary, housing-tier, and water-infrastructure follow-ups
- continue the material-board wave with terrain-transition, shield-face, leather-kit, and siege-wood follow-ups
- begin the first integration-ready placeholder prefab plan for one tightly-scoped asset family only after review tagging is complete
