# Bloodlines Graphics Lane Unity Vector Browser Ingest

Date: 2026-04-16
Track: graphics and visual-production lane
Scope: Unity 6.3 staging ingest for governed SVG concept sheets
Stage: `tooling_and_integration`
Review outcome: `integration_ready_for_staging`

## Summary

This pass completes the approved Unity-side vector import tooling step for the existing Bloodlines graphics lane without altering gameplay systems, lore, dynastic canon, or browser-runtime priorities.

The result is a working staged ingest path for the current SVG concept sheets:

- canonical SVG review sheets remain in graphics staging and Unity staging
- Unity can now generate PNG review boards from those sheets deterministically
- the preferred render path is headless local browser export for full SVG fidelity
- a Unity-local mesh raster fallback remains available if browser export is unavailable

## Files Added Or Updated

### Unity editor tooling

- `unity/Assets/_Bloodlines/Code/Editor/GraphicsConceptSheetVectorImport.cs`
- `unity/Assets/_Bloodlines/Shaders/Staging/VectorImport/BloodlinesVectorImport.shader`
- `unity/Assets/_Bloodlines/Shaders/Staging/VectorImport/BloodlinesVectorGradientImport.shader`
- `unity/Assets/_Bloodlines/Shaders/Staging/VectorImport/BloodlinesVectorDemultiply.shader`
- `unity/Assets/_Bloodlines/Shaders/Staging/VectorImport/BloodlinesVectorExpandEdges.shader`
- `unity/Assets/_Bloodlines/Shaders/Staging/VectorImport/BloodlinesVectorBlendMax.shader`
- `unity/Assets/_Bloodlines/Shaders/Staging/VectorImport/VectorGradient.cginc`

### Unity staging documentation

- `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
- `unity/Assets/_Bloodlines/Docs/VisualProduction/README.md`
- `unity/Assets/_Bloodlines/Art/Staging/README.md`
- `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/README.md`
- `unity/Assets/_Bloodlines/Shaders/Staging/VectorImport/README.md`

### Repeatable command-line entry point

- `scripts/Invoke-BloodlinesUnityGraphicsRasterize.ps1`

### Generated Unity staging outputs

- `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/*.png`

## Why The External Package Was Not Kept

- A dedicated tooling evaluation confirmed that adding the external `com.unity.vectorgraphics` package into this Unity 6.3 project produced assembly overlap with the built-in vector module surface.
- The graphics lane therefore stays on a Bloodlines-local staged raster path instead of reintroducing a package conflict into the project manifest.
- The project manifest now keeps valid Unity 6.3 compatible package versions for DOTS and rendering packages, without the external vector package.

## Unity 6.3 Workflow Now Supported

1. Author or update governed SVG concept sheets under `14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/`.
2. Mirror those sheets into Unity staging with `Bloodlines -> Graphics -> Sync Concept Sheets`.
3. Generate governed PNG review boards with:
   - `Bloodlines -> Graphics -> Sync And Rasterize Concept Sheets`, or
   - `Bloodlines -> Graphics -> Rasterize Concept Sheets`
4. Review the generated PNG boards under `unity/Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
5. Keep all outputs in staging until a later art-review decision produces integration-ready downstream assets.

## Validation Performed

- Unity 6.3 LTS batchmode executed successfully with:
  - `Bloodlines.EditorTools.GraphicsConceptSheetVectorImport.RunBatchSyncAndRasterizeConceptSheets`
- Successful log captured at:
  - `artifacts/unity-vector-browser-hybrid.log`
- PNG outputs were generated for all eight staged concept sheets now mirrored in Unity.
- The generated unit-sheet PNG was visually checked to confirm browser-first export preserved the full board layout and text.

## Non-Destructive Guarantees Followed

- No gameplay runtime files were redirected into graphics work.
- No lore, House canon, faith canon, economy canon, or system doctrine was rewritten.
- No existing files were deleted.
- No raw graphics-lane SVG source sheets were replaced with lossy derived outputs.
- No staging asset was promoted into runtime-facing art folders.

## Recommended Next Graphics-Lane Steps

1. Review the new PNG boards in Unity staging and tag each source sheet `approved`, `revise`, or `replace`.
2. Build the next concept batch around House-specific military silhouette sheets.
3. Build fortification kit decomposition sheets that can later drive modular 3D blockout.
4. If future staging batches need larger export boards, extend the raster script rather than bypassing the governed lane.
