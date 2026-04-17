# Graphics Batch Review Ledger

Established: 2026-04-16
Scope: governed review inventory for Bloodlines graphics batches 01 through 08

## Purpose

This ledger turns the existing graphics batches into a reviewable production surface.

It does not overwrite earlier reports.
It records the current governed state of each batch after staging, raster generation, and Unity testbed placement work.

## Review Record Rules

- `review outcome` reflects the current accepted stage, not a final-art verdict
- `integration gate` remains `not_integration_ready` until a later directed review explicitly upgrades it
- empty issue-tag lists mean no issue has been formally logged yet, not that the batch is final
- use this ledger together with:
  - `13_AUDIO_VISUAL/GRAPHICS_PIPELINE/VISUAL_REVIEW_MATRIX_2026-04-16.md`
  - `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`
  - the Unity testbed scenes under `unity/Assets/_Bloodlines/Scenes/Testbeds/`

## Batch 01

- batch id: `batch_01_first_pass_sheets`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `VisualReadability_Testbed.unity` carries the shared roster board
  - `IconLegibility_Testbed.unity` carries the shared resource-command and founding-emblem boards
- current value:
  - baseline live roster silhouette direction
  - baseline icon and emblem readability direction
- next action:
  - review against later House and icon work for consistency and drift

## Batch 02

- batch id: `batch_02_environment_banners_portraits`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `TerrainLookdev_Testbed.unity` carries the biome board
  - `MaterialLookdev_Testbed.unity` carries banner and emblem support boards
  - `IconLegibility_Testbed.unity` carries the banner board
- current value:
  - terrain readability baseline
  - banner hierarchy direction
  - portrait direction reference
- next action:
  - review banner and emblem contrast against later House and covenant overlays

## Batch 03

- batch id: `batch_03_first_house_silhouette_sheets`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `VisualReadability_Testbed.unity` carries Ironmark, Stonehelm, Hartvale, and Trueborn board displays
- current value:
  - first House-specific military silhouette lane
- next action:
  - review against the full nine-House wall now that later Houses are present

## Batch 04

- batch id: `batch_04_fortification_terrain_water_followups`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `TerrainLookdev_Testbed.unity` carries cliff-transition, resource-ground, and bridge or water-infrastructure boards
- current value:
  - fortification kit direction
  - terrain transition and water-read direction
- next action:
  - review against construction and breach-state priorities before any runtime placeholder promotion

## Batch 05

- batch id: `batch_05_remaining_house_silhouette_sheets`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `VisualReadability_Testbed.unity` now carries the full nine-House board wall
- current value:
  - completes the first full nine-House military silhouette study lane
- next action:
  - run explicit dynasty-confusion review across all nine Houses in one pass

## Batch 06

- batch id: `batch_06_neutral_faith_civic_and_material_foundation_boards`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `MaterialLookdev_Testbed.unity` carries neutral-settlement, faith-structure, civic-support, shared-foundation, and House-trim boards
- current value:
  - first settlement-family and material-family comparison wall
- next action:
  - review material value control before any placeholder material-family promotion

## Batch 07

- batch id: `batch_07_settlement_support_and_water_followups`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `VisualReadability_Testbed.unity` now carries the support-structure board wall after the governed populate refresh
- current value:
  - market or granary or housing or water-support or covenant-progression family coverage
- next action:
  - directed support-structure review for role recognition at gameplay height

## Batch 08

- batch id: `batch_08_settlement_variants_and_covenant_overlays`
- review outcome: `placeholder_only`
- integration gate: `not_integration_ready`
- issue tags: none formally logged
- Unity evidence:
  - staged SVG and PNG review boards exist
  - `MaterialLookdev_Testbed.unity` now carries the settlement-variant wall after the governed populate refresh
- current value:
  - House overlay rules for support structures
  - deeper trade, storage, and housing variants
  - canonical covenant overlay architecture direction
- next action:
  - directed House-overlay and covenant-overlay review against House identity packs and faith doctrine

## Immediate Graphics-Lane Review Priority

1. Review the nine-House military wall in `VisualReadability_Testbed.unity` for `dynasty_confusion`, `silhouette_issue`, and `scale_issue`.
2. Review the support-structure wall in `VisualReadability_Testbed.unity` for function recognition at gameplay height.
3. Review the settlement-variant wall in `MaterialLookdev_Testbed.unity` for House overlay discipline and covenant overlay plausibility.
4. Record the first formal `approved`, `revise`, or `replace` verdicts only after those directed passes happen.

## Non-Destructive Status

All eight batches remain preserved in staging.
None have been promoted into runtime-facing art folders in this ledger pass.
