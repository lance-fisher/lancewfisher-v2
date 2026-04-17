# First Pass Concept Batches

Batch dates: 2026-04-15 through 2026-04-16
Track: Bloodlines graphics lane
Stage: `first_pass_concept`
Review outcome: `placeholder_only`

## Purpose

This batch establishes real, usable graphics-lane starting assets for the live and near-live Bloodlines surfaces without pretending they are approved final art.

These files are:

- additive
- concept-facing
- review-tagged
- safe to replace later

## Batch 01 Files

- `bl_unit_shared_live_roster_sheet_a_first_pass_concept_v001.svg`
- `bl_building_shared_core_structures_sheet_a_first_pass_concept_v001.svg`
- `bl_icon_shared_resource_command_sheet_a_first_pass_concept_v001.svg`
- `bl_icon_house_founding_emblems_sheet_a_first_pass_concept_v001.svg`

## Batch 02 Files

- `bl_terrain_shared_biomes_sheet_a_first_pass_concept_v001.svg`
- `bl_building_shared_fortification_damage_and_breach_sheet_a_first_pass_concept_v001.svg`
- `bl_icon_house_banner_system_sheet_a_first_pass_concept_v001.svg`
- `bl_portrait_bloodline_roles_direction_sheet_a_first_pass_concept_v001.svg`
- `preview.html`

## Batch 03 Files

- `bl_unit_ironmark_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `bl_unit_stonehelm_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `bl_unit_hartvale_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `bl_unit_trueborn_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`

## Batch 04 Files

- `bl_building_shared_fortification_kit_decomposition_sheet_a_first_pass_concept_v001.svg`
- `bl_terrain_shared_cliff_and_shoreline_transition_sheet_a_first_pass_concept_v001.svg`
- `bl_terrain_shared_resource_ground_and_edge_blend_sheet_a_first_pass_concept_v001.svg`
- `bl_environment_shared_bridge_and_water_infrastructure_sheet_a_first_pass_concept_v001.svg`
- `bl_environment_shared_logistics_and_setpieces_sheet_a_first_pass_concept_v001.svg`

## Batch 05 Files

- `bl_unit_highborne_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `bl_unit_goldgrave_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `bl_unit_westland_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `bl_unit_whitehall_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `bl_unit_oldcrest_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`

## Batch 06 Files

- `bl_building_shared_neutral_settlement_structures_sheet_a_first_pass_concept_v001.svg`
- `bl_building_shared_faith_structure_families_sheet_a_first_pass_concept_v001.svg`
- `bl_building_shared_civic_support_variants_sheet_a_first_pass_concept_v001.svg`
- `bl_material_shared_foundation_boards_sheet_a_first_pass_concept_v001.svg`
- `bl_material_house_trim_families_sheet_a_first_pass_concept_v001.svg`

## Notes

- These are vector-first concept and placeholder sheets because the canonical graphics source lane remains SVG-based even after the Unity staging raster pipeline was added.
- House emblems in the heraldry sheet are concept candidates, not settled canon marks.
- House banner panels use canonical House names and color direction, but they do not settle heraldic marks that remain open.
- The live roster and building sheets prioritize gameplay-height silhouette and palette direction over detailed rendering.
- The terrain, breach, banner, and portrait sheets extend the same governed graphics lane without changing canon, gameplay logic, or runtime code.
- Unity 6.3 now supports a governed browser-first raster staging path for these sheets under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`, while keeping the SVG files as source-of-truth review boards.
- The House-specific silhouette sheets are treatment studies for Ironmark, Stonehelm, Hartvale, and Trueborn. They do not settle new gameplay roster seams.
- The new fortification and terrain follow-up sheets extend the production lane into modular wall-kit thinking, cliff and shoreline transitions, economy-site ground-treatment logic, bridge or water-infrastructure readability, and the first logistics or setpiece prop lane.
- Batch 05 completes the first full nine-House silhouette-study lane by adding Highborne, Goldgrave, Westland, Whitehall, and Oldcrest treatment boards.
- Batch 06 extends the lane into neutral settlement structures, faith structure families, civic support variants, shared foundation material boards, and House trim-family control boards.
- Batch 06 is intended to support the next Unity 6.3 material-lookdev and settlement-readability pass without promoting any sheet directly into runtime art.
- Nothing in this batch supersedes the graphics production bible, House identity packs, or any canon source file.
