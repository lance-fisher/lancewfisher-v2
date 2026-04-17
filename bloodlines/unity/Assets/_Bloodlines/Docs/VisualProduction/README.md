# Visual Production

This folder anchors the Unity-local continuation surface for the Bloodlines graphics lane.

Use it together with:

- `D:/ProjectsHome/Bloodlines/13_AUDIO_VISUAL/GRAPHICS_PIPELINE/`
- `D:/ProjectsHome/Bloodlines/03_PROMPTS/GRAPHICS_PIPELINE/`
- `D:/ProjectsHome/Bloodlines/14_ASSETS/GRAPHICS_PIPELINE/`
- `D:/ProjectsHome/Bloodlines/docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
- `D:/ProjectsHome/Bloodlines/docs/unity/BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`
- `D:/ProjectsHome/Bloodlines/docs/unity/BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`

## Local Rule

Do not move raw concept or placeholder assets directly into final runtime folders.
Use the staging folders first:

- `Assets/_Bloodlines/Art/Staging`
- `Assets/_Bloodlines/Materials/Staging`
- `Assets/_Bloodlines/Prefabs/Staging`

## Unity 6.3 Notes

- `Assets/_Bloodlines/Art/Staging/ConceptSheets/` is the governed mirror for the external graphics-lane concept sheets under `D:/ProjectsHome/Bloodlines/14_ASSETS/GRAPHICS_PIPELINE/02_FIRST_PASS_CONCEPT/`.
- Use the Unity editor menu `Bloodlines -> Graphics -> Sync Concept Sheets` to refresh the mirror after new concept sheets are added.
- Use `Bloodlines -> Graphics -> Sync And Rasterize Concept Sheets` to generate governed PNG review boards into `Assets/_Bloodlines/Art/Staging/ConceptSheetsRasterized/`.
- Use `Bloodlines -> Graphics -> Create Visual Testbed Scene Shells` to create the first governed scene shells under `Assets/_Bloodlines/Scenes/Testbeds/` without touching gameplay scenes.
- Use `Bloodlines -> Graphics -> Populate Visual Testbed Scenes` to populate the graphics testbeds with generated placeholder comparisons, staging-only material swatches, and staged board displays.
- CLI wrapper for lock-free batch use: `D:/ProjectsHome/Bloodlines/scripts/Invoke-BloodlinesUnityGraphicsPopulateTestbeds.ps1`.
- The governed testbed population pass is already live on disk. Current populated content now includes:
  - the nine-House readability grid
  - the Batch 07 support-structure wall in `VisualReadability`
  - shared and House material swatches
  - the Batch 06 settlement and material wall in `MaterialLookdev`
  - the Batch 08 settlement-variant wall in `MaterialLookdev`
  - terrain comparison strips
  - icon contrast boards
- The current project manifest does not include `com.unity.vectorgraphics`. That is intentional. A dedicated 2026-04-16 tooling pass confirmed the external package conflicts with Unity 6.3's built-in vector module surface in this project.
- The supported path is therefore:
  - staged `.svg` source sheets in `ConceptSheets/`
  - generated `.png` review boards in `ConceptSheetsRasterized/`
  - no promotion into runtime-facing folders until the review system marks a downstream asset integration-ready
