# AGENTS.md: Bloodlines Canonical Root

This file governs AI work inside `D:\ProjectsHome\Bloodlines`.

## Canonical Root

`D:\ProjectsHome\Bloodlines` is the one canonical Bloodlines entry path for future sessions.

Important filesystem reality:

- `D:\ProjectsHome\Bloodlines` is the canonical session target.
- It currently resolves through a junction to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`.
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines` is the physical backing path, not a separate project root.
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines` is a preserved compatibility surface only.

Future Bloodlines sessions should be pointed at `D:\ProjectsHome\Bloodlines` and should behave as if no other Bloodlines root exists.

## Read Order

Read these files in order before making meaningful changes:

1. `D:\ProjectsHome\Bloodlines\AGENTS.md`
2. `D:\ProjectsHome\Bloodlines\README.md`
3. `D:\ProjectsHome\Bloodlines\CLAUDE.md`
4. `D:\ProjectsHome\Bloodlines\MASTER_PROJECT_INDEX.md`
5. `D:\ProjectsHome\Bloodlines\MASTER_BLOODLINES_CONTEXT.md`
6. `D:\ProjectsHome\Bloodlines\CURRENT_PROJECT_STATE.md`
7. `D:\ProjectsHome\Bloodlines\NEXT_SESSION_HANDOFF.md`
8. `D:\ProjectsHome\Bloodlines\SOURCE_PROVENANCE_MAP.md`
9. `D:\ProjectsHome\Bloodlines\continuity\PROJECT_STATE.json`

Then load subsystem detail as needed from the numbered canon folders and `docs/`.

## Non-Negotiables

- Preservation mode is the default. Delete nothing unless Lance explicitly authorizes it.
- Do not reduce, summarize away, compress, or silently replace Bloodlines source material.
- If two files differ, preserve both.
- If multiple versions compete, keep originals and document the relationship.
- Do not create a new parallel Bloodlines root elsewhere unless Lance explicitly instructs it.
- New Bloodlines work belongs inside this root.
- Imported outside material belongs under `archive_preserved_sources/` unless it is being integrated into active canon or active implementation.
- Continuity updates belong in `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/`.
- Governance updates belong in the root governance files or `governance/`.
- Prompts, scratch notes, and session prompts belong in `03_PROMPTS/` or `continuity/`, not in random temporary folders.

## Working Rules

- The numbered directories `00_ADMIN` through `19_ARCHIVE` remain valid and authoritative.
- `docs/` contains planning, reality reports, implementation roadmaps, Unity migration docs, and analytical material.
- `archive_preserved_sources/` contains preserved outside roots and compatibility surfaces pulled into this canonical root.
- `governance/` contains imported workspace rule overlays and parent-surface governance that affect continuity.
- `continuity/` contains machine-readable and human-readable cross-session continuation files.
- `reports/` contains dated operational reports supporting the root-level continuity files.

## Required Storage Discipline

When adding new material, store it here:

- Canon and settled design: `01_CANON/`, `04_SYSTEMS/`, `05_LORE/`, `06_FACTIONS/`, `07_FAITHS/`, `08_MECHANICS/`, `09_WORLD/`, `10_UNITS/`, `11_MATCHFLOW/`, `12_UI_UX/`, `13_AUDIO_VISUAL/`
- Raw prompts and reusable AI inputs: `03_PROMPTS/`
- Session ingestions and raw imported thinking: `02_SESSION_INGESTIONS/`
- Code and runtime: `src/`, `data/`, `tests/`, `play.html`, `unity/`
- Assets and exploratory design: `14_ASSETS/`, `15_PROTOTYPE/`
- Research and references: `16_RESEARCH/`
- Backlog and immediate work tracking: `17_TASKS/`, `tasks/`
- Long-term preserved historical artifacts: `19_ARCHIVE/`, `archive_preserved_sources/`
- Governance and continuity: root governance files, `governance/`, `continuity/`

## If You Find Outside Bloodlines Material

Do not leave it ungoverned.

1. Copy it into `archive_preserved_sources/` with a provenance-preserving name.
2. Update `SOURCE_PROVENANCE_MAP.md`.
3. Update `CONSOLIDATION_REPORT.md` and `FILE_MANIFEST.json` if the addition is meaningful.
4. State clearly whether the imported material is active canon, active implementation, preserved alternate, or compatibility-only.

## Completion Protocol

Before ending a meaningful work block:

1. Update `CURRENT_PROJECT_STATE.md`.
2. Update `NEXT_SESSION_HANDOFF.md`.
3. Update `continuity/PROJECT_STATE.json` if the project state changed materially.
4. Preserve new outside-source imports in `archive_preserved_sources/`.
5. Do not claim the project is consolidated if new external Bloodlines material was found and left undocumented.
