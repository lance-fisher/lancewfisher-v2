# Consolidation Note 2026-04-12

## Purpose

This note records the consolidation of the external Bloodlines archive at `D:\ProjectsHome\Bloodlines` into the canonical project root at `deploy/bloodlines/`.

## Result

`deploy/bloodlines/` now contains:

- The preserved design archive and canon corpus
- Session ingestions, prompt files, research files, prototype remnants, and historical archive material from the external folder
- The active browser RTS runtime, gameplay data, tests, and implementation docs

## Canonical Root

Going forward, `deploy/bloodlines/` is the only active Bloodlines project folder.

Other copies may remain on disk for preservation or compatibility, but they should not be treated as authoritative and should not be used as the primary working root.

## Merge Policy Applied

- Files present only in `D:\ProjectsHome\Bloodlines` were copied into `deploy/bloodlines/`
- Active runtime files already present in `deploy/bloodlines/` were preserved
- README and planning docs were updated to reflect the consolidation

## Follow-On Rule

Future work should update `deploy/bloodlines/` directly first. If compatibility mirrors are kept elsewhere, they should be treated as secondary copies rather than parallel project roots.
