# Bloodlines

Bloodlines is a dynasty-driven strategy game project whose one canonical session root is:

- `D:\ProjectsHome\Bloodlines`

This root combines:

- the preserved design archive and canon corpus
- the frozen browser behavioral-spec runtime
- the Unity shipping lane
- imported prompts, archived source roots, governance overlays, and continuity files needed for future sessions

## Canonical Root Rule

Future Bloodlines sessions should start from `D:\ProjectsHome\Bloodlines`.

Filesystem reality:

- `D:\ProjectsHome\Bloodlines` is the canonical session path.
- It currently resolves to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines` through a junction.
- `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines` is preserved as a compatibility copy only.

Do not treat the physical backing path or deploy copy as separate active project roots.

## What This Root Contains

- full numbered design corpus from `00_ADMIN` through `19_ARCHIVE`
- browser behavioral-spec code in `src/`, `data/`, `tests/`, and `play.html`
- Unity shipping work in `unity/` and `docs/unity/`
- imported preserved source roots in `archive_preserved_sources/`
- imported governance surfaces in `governance/`
- root continuity files plus machine-readable state in `continuity/`

## Current Production Direction

- Bloodlines is being built as the full canonical no-compromise realization of the design bible.
- The shipping engine is Unity 6.3 LTS with DOTS / ECS.
- The browser runtime is frozen as a behavioral specification only. Do not add new systems there.
- Work in Unity belongs only inside `unity/` under this root. The `Bloodlines/` URP stub is not an active work target.
- MVP framing, phased-release reduction, and scope-cutting are stale and forbidden for future Bloodlines planning.
- The canonical owner-direction file is:
  - `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`

## Current Runtime Surfaces

- `index.html`: Bloodlines design archive viewer
- `play.html`: frozen browser reference simulation for behavioral comparison, not a production lane

## Run

From anywhere:

```powershell
python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines
```

Open:

- `http://localhost:8057/`
- `http://localhost:8057/play.html`

## Validate

```powershell
Set-Location D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
```

## Structure

- `00_ADMIN` through `19_ARCHIVE`: preserved design corpus, canon, lore, systems, prompts, exports, research, archive
- `docs/`: current-state analysis, implementation roadmap, decisions register, Unity migration docs, prior consolidation notes
- `data/`: data-driven gameplay definitions
- `src/`: browser runtime code
- `tests/`: runtime and data validation
- `unity/`: Unity continuation lane
- `archive_preserved_sources/`: outside Bloodlines source roots preserved in-root
- `governance/`: imported overlays and governance surfaces relevant to Bloodlines continuity
- `continuity/`: machine-readable and human-readable continuation layer
- `reports/`: dated operational reports supporting the continuity layer

## Read Order For Future Sessions

1. `AGENTS.md`
2. `README.md`
3. `CLAUDE.md`
4. `MASTER_PROJECT_INDEX.md`
5. `MASTER_BLOODLINES_CONTEXT.md`
6. `CURRENT_PROJECT_STATE.md`
7. `NEXT_SESSION_HANDOFF.md`
8. `SOURCE_PROVENANCE_MAP.md`

## Preservation Rules

1. Nothing relevant gets deleted without explicit authorization.
2. Historical context stays preserved even when later canon supersedes it.
3. New implementation extends the preserved bible, it does not flatten it.
4. Outside Bloodlines sources get imported into `archive_preserved_sources/` rather than left ungoverned.
5. New future-session prompts, notes, and handoffs belong in this root.
