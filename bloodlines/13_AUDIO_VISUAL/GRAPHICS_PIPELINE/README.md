# Bloodlines Graphics Pipeline

This folder is the dedicated Bloodlines graphics and visual-production lane established on 2026-04-15.

It exists to let visual work progress without colliding with:

- gameplay implementation
- lore and canon consolidation
- dynasty systems
- faith systems
- UI system work
- ongoing continuation prompts and reports

## Preservation Boundary

This lane is additive. It does not replace:

- `13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md`
- `13_AUDIO_VISUAL/MASTER_DOCTRINE_AUDIO_VISUAL_INTEGRATION_2026-04-14.md`
- `06_FACTIONS/*`
- `09_WORLD/*`
- `10_UNITS/*`
- `12_UI_UX/*`
- `docs/unity/*`

Those files remain authoritative source context. This lane converts that context into production-ready visual infrastructure.

## Audit Summary

Graphics-lane audit results before creation:

- Existing active visual doctrine was present but fragmented across audio/visual, faction, terrain, unit, UI, and Unity docs.
- Active Unity art, prefab, and material lanes were structurally seeded but contained no production art assets yet.
- No active image, texture, model, or concept sheet inventory existed in the canonical root outside preserved archive surfaces.
- Archived imagery exists inside preserved sources and review artifacts, but not as governed active Bloodlines production assets.

## What Lives Here

- `BLOODLINES_VISUAL_PRODUCTION_BIBLE_2026-04-15.md`
- `ASSET_MANIFEST_SCHEMA.md`
- `HOUSE_VISUAL_IDENTITY_PACKS_2026-04-15.md`
- `MAP_AND_TERRAIN_VISUAL_TRACK_2026-04-15.md`
- `PRODUCTION_STAGE_LADDER_2026-04-15.md`
- `ART_REVIEW_AND_APPROVAL_SYSTEM_2026-04-15.md`
- `UNIT_VISUAL_DIRECTION_PACKS_2026-04-16.md`
- `BUILDING_FAMILY_DIRECTION_PACKS_2026-04-16.md`
- `TERRAIN_AND_BIOME_DIRECTION_PACKS_2026-04-16.md`
- `VISUAL_REVIEW_MATRIX_2026-04-16.md`
- `GRAPHICS_BATCH_REVIEW_LEDGER_2026-04-16.md`
- `MANIFESTS/`

## Connected Surfaces

- Prompt and concept framework: `03_PROMPTS/GRAPHICS_PIPELINE/`
- Asset staging ladder: `14_ASSETS/GRAPHICS_PIPELINE/`
- Unity ingestion and staging: `docs/unity/VISUAL_ASSET_PIPELINE_2026-04-15.md`
- Unity implementation and testbed guidance:
  - `docs/unity/BLOODLINES_UNITY_6_3_VISUAL_IMPLEMENTATION_GUIDE_2026-04-16.md`
  - `docs/unity/BLOODLINES_VISUAL_TESTBED_PLAN_2026-04-16.md`
- Unity-local continuation surface: `unity/Assets/_Bloodlines/Docs/VisualProduction/`

## Working Rule

Assets move through this lane in order:

1. doctrine and House identity
2. manifest registration
3. concept brief and prompt generation
4. staging under `14_ASSETS/GRAPHICS_PIPELINE/`
5. review and approval
6. Unity staging
7. approved runtime integration

Nothing skips manifesting, review tagging, or stage labeling.
