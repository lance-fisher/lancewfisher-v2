# Consolidation Note 2026-04-13 Single Root

## Purpose

This note records the transition from multiple active Bloodlines folders to one main Bloodlines folder for future work.

## Main Working Root

The main Bloodlines working root is now:

- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`

All future canon, docs, gameplay code, Unity work, tests, tooling, and organizational changes should happen there first.

## Compatibility Paths

Legacy paths preserved for compatibility:

- `D:\ProjectsHome\Bloodlines`
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines`

Rules:

1. `D:\ProjectsHome\Bloodlines` is now a compatibility alias and must not be treated as a separate repo root.
2. `deploy\bloodlines` must not be treated as a separate Bloodlines project root.
3. If a future session is launched from a compatibility path, it should immediately resolve back to the main root.

## Preservation Work Completed

- The prior external Bloodlines repo was preserved under:
  - `D:\ProjectsHome\_archive\2026-04-13\bloodlines-single-root\external-repo-preconsolidation`
- The prior small repo-root Bloodlines mirror was preserved under:
  - `D:\ProjectsHome\_archive\2026-04-13\bloodlines-single-root\repo-root-mirror-preconsolidation`
- The full archive, runtime, data, docs, and Unity source shell were merged into the main root.

## Current Filesystem State

As of this note:

- `D:\ProjectsHome\Bloodlines` resolves to the main root through a junction.
- The repo-root `bloodlines\` folder contains the full project.
- `deploy\bloodlines` still exists as a compatibility copy because it was locked during the attempted filesystem swap in this session. It should be treated as non-authoritative.

## Practical Follow-On Rule

When in doubt:

1. open `bloodlines\README.md`
2. work in `bloodlines\`
3. treat every other Bloodlines path as legacy or compatibility only
