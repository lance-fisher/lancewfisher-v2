# SESSION_CONTINUITY

This file defines how cross-session Bloodlines continuity must be maintained.

## Canonical Session Target

Always target:

- `D:\ProjectsHome\Bloodlines`

Do not start future Bloodlines sessions from:

- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines`
- `D:\ProjectsHome\frontier-bastion`

unless Lance explicitly instructs it for a special purpose.

## End-Of-Session Updates

Before ending a meaningful session:

1. Update `CURRENT_PROJECT_STATE.md`.
2. Update `NEXT_SESSION_HANDOFF.md`.
3. Update `continuity/PROJECT_STATE.json` if root identity, preservation imports, or implementation status changed.
4. If outside Bloodlines material was imported, update `SOURCE_PROVENANCE_MAP.md` and `CONSOLIDATION_REPORT.md`.
5. Regenerate `FILE_MANIFEST.json` after major structural changes.

## Storage Rules

- New canon belongs in the numbered canon folders.
- New runtime work belongs in `src/`, `data/`, `tests/`, `unity/`, and supporting docs.
- New preserved outside sources belong in `archive_preserved_sources/`.
- New governance overlays or imported rules belong in `governance/`.
- New cross-session notes belong in `continuity/` and the root handoff files.

## Preservation Rule

Nothing in `archive_preserved_sources/`, `19_ARCHIVE/`, or `governance/` should be deleted or silently replaced.
