# Visual Testbeds

This folder is reserved for additive scene-safe visual verification.

Use testbeds for:

- gameplay-height silhouette review
- terrain and material lookdev
- icon-size checks
- placeholder-to-final replacement validation

Recommended scene lanes:

- `VisualReadability`
- `TerrainLookdev`
- `MaterialLookdev`
- `IconLegibility`

Bootstrap helper:

- `Bloodlines -> Graphics -> Create Visual Testbed Scene Shells`

Population helper:

- `Bloodlines -> Graphics -> Populate Visual Testbed Scenes`

Current generated state:

- all four governed testbed scenes already contain tool-owned `GENERATED_TESTBED_CONTENT`
- reruns should refresh only that generated root and must not be used to overwrite unrelated hand-placed scene work
- the current population pass covers nine-House readability comparisons, terrain strips, shared and House material swatches, and staged review-board displays
- the current population pass also includes the Batch 07 support-structure wall in `VisualReadability` and the Batch 08 settlement-variant wall in `MaterialLookdev`

See also:

- `D:/ProjectsHome/Bloodlines/docs/unity/BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`

Do not repurpose testbeds as feature scenes or gameplay staging grounds.
