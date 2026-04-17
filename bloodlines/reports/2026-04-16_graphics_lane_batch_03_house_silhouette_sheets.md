# Bloodlines Graphics Lane Batch 03

Date: 2026-04-16
Track: graphics and visual-production lane
Batch: house-specific military silhouette sheets
Stage: `first_pass_concept`
Review outcome: `placeholder_only`

## Summary

This batch adds the first House-specific military treatment boards to the existing Bloodlines graphics lane.

The new sheets are intentionally framed as silhouette and material-direction studies rather than gameplay-roster rewrites. They exist to support future concept prompting, 3D blockout, House distinction review, and battlefield-read testing while preserving canon boundaries.

## Files Created In Graphics Staging

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_unit_ironmark_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_unit_stonehelm_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_unit_hartvale_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/bl_unit_trueborn_house_military_silhouettes_sheet_a_first_pass_concept_v001.svg`

## Files Updated For Review Surfaces

- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/INDEX.md`
- `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/preview.html`

## Unity Staging Mirror

The new SVG sheets were also mirrored into:

- `unity/Assets/_Bloodlines/Art/Staging/ConceptSheets/`

This keeps the House-specific lane aligned with the governed Unity staging mirror established in earlier batches.

## What The Batch Covers

### Ironmark

- forged mass
- ember accent discipline
- Axeman treatment
- command authority through forge marks rather than ornament

### Stonehelm

- shield-heavy defensive mass
- wall and tower service posture
- keep-linked command identity
- warm stone-and-garrison palette discipline

### Hartvale

- defended agrarian levy treatment
- Verdant Warden separation without wild-fantasy drift
- harvest and storehouse visual warmth
- steward-command and settlement-defense identity

### Trueborn

- settled-visual-only contrast house
- administrative nobility rather than luxury-house excess
- silver-blue composure with restrained gold
- seal-guard and marshal authority without paladin drift

## Constraints Followed

- No House canon, roster canon, or strategic doctrine was rewritten.
- No new final units were declared.
- No lore, systems, economy, dynasty, faith, or runtime files were redirected into art-first changes.
- No existing files were deleted or collapsed.
- All sheets remain clearly tagged as `first_pass_concept` and `placeholder_only`.

## Unity Note

- The governed Unity rasterization path already exists and remains the preferred way to produce PNG review boards.
- During this pass, the Unity project was already open in another editor instance, so a new batchmode raster pass for the Batch 03 sheets was not forced in order to avoid disrupting active work.
- The next available Unity staging run should use either:
  - `Bloodlines -> Graphics -> Sync And Rasterize Concept Sheets`, or
  - `scripts/Invoke-BloodlinesUnityGraphicsRasterize.ps1`

## Recommended Next Graphics-Lane Steps

1. Review batches 01 through 03 together in the preview surface and in Unity staging.
2. Tag each House sheet `approved`, `revise`, or `replace` before deeper per-House expansion.
3. Build the fortification kit decomposition batch next.
4. Follow that with terrain transition and resource-ground treatment sheets.
