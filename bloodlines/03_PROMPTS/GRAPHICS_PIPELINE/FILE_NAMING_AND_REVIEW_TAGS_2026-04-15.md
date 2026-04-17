# File Naming And Review Tags

## Naming Standard

Use:

`bl_[assetclass]_[lane]_[family]_[variant]_[stage]_v###`

Examples:

- `bl_unit_ironmark_axeman_a_placeholder_v001`
- `bl_building_universal_siege_workshop_main_approved_direction_v003`
- `bl_icon_hartvale_banner_set_a_first_pass_concept_v002`

## Asset Classes

- `unit`
- `building`
- `terrain`
- `biome`
- `setpiece`
- `icon`
- `portrait`
- `material`
- `prefab`

## Lane Values

- House id such as `ironmark`
- `universal`
- `shared`
- `neutral`
- `faith_oldlight`
- `faith_blooddominion`
- `faith_order`
- `faith_wild`

## Stage Tokens

- `placeholder`
- `first_pass_concept`
- `approved_direction`
- `production_candidate`
- `in_engine_test_asset`
- `refinement_candidate`
- `near_final`
- `final`

## Approval Tags

- `APPROVED`
- `REVISE`
- `REPLACE`
- `PLACEHOLDER_ONLY`
- `INTEGRATION_READY`
- `NOT_INTEGRATION_READY`

## Review Issue Tags

- `STYLE_DRIFT`
- `READABILITY`
- `DYNASTY_CONFUSION`
- `TEXTURE_MATERIAL`
- `SCALE`
- `SILHOUETTE`

## Rejection Rule

If a file does not carry both a stage token and a review outcome, it is not ready to move forward in the pipeline.
