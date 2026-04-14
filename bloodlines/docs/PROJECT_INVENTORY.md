# Bloodlines Project Inventory

## Root Mapping

- Authoritative project root: `deploy/bloodlines/`
- Legacy compatibility copies may exist elsewhere, but they are no longer authoritative
- Broader repository root: `D:/ProjectsHome/FisherSovereign/lancewfisher-v2/`

## Consolidation Status

- As of 2026-04-12, the external archive at `D:\ProjectsHome\Bloodlines` has been merged into `deploy/bloodlines/`
- `deploy/bloodlines/` should now be treated as the sole working project folder

## What Existed Before This Session

### Playable/runtime artifacts

| Path | Type | Purpose |
|---|---|---|
| `index.html` | Static HTML app | Password-gated archive viewer for the Bloodlines design corpus |
| `api/submit.php` | PHP endpoint | Idea inbox/changelog support for the archive viewer |

### Canon and design corpus

| Path | Approx. lines | Role |
|---|---:|---|
| `18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md` | 9,402 | Full cross-source continuity document; includes source governance rules and integrated design corpus |
| `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md` | 1,764 | Active design bible snapshot referenced as the authoritative bible version |
| `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE.md` | 1,764 | Same integrated export lineage kept for continuity |
| `01_CANON/CANONICAL_RULES.md` | 311 | Canon lock, deprecations, supersessions, latest settled design decisions |
| `01_CANON/BLOODLINES_MASTER_MEMORY.md` | 1,886 | Historical memory ledger and session-ingestion continuity source |
| `00_ADMIN/PROJECT_STATUS.md` | 127 | Project status history and prior design-phase milestone log |
| `04_SYSTEMS/*` | 72-419 each | Deep dives for faith, conviction, dynastic, resource, territory, population, Born of Sacrifice |
| `05_LORE/*` | 75-125 each | Timeline and world-history canon |
| `06_FACTIONS/FOUNDING_HOUSES.md` | 241 | Founding house canon, profiles, historical variants |
| `07_FAITHS/FOUR_ANCIENT_FAITHS.md` | 1,171 | Full covenant detail |
| `08_MECHANICS/*` | 133-272 each | Operations, diplomacy, and related strategic mechanics |
| `10_UNITS/UNIT_INDEX.md` | 317 | Unit roster and tier progression |
| `11_MATCHFLOW/*` | 126-543 each | Match flow, naval warfare, political events |
| `12_UI_UX/UI_NOTES.md` | 141 | RTS/dynasty UI direction |
| `13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md` | 109 | Visual and audio direction |
| `19_ARCHIVE/LEGACY_ARCHIVE_INDEX.md` | 34 | References pre-structure legacy files and preserved historical content |

## What Was Missing Before This Session

- No playable RTS implementation.
- No `src/`, `data/`, `tests/`, or Bloodlines-specific `docs/` runtime foundation.
- No skirmish map, no game loop, no units/buildings/resources simulation.
- No Bloodlines-specific runbook or implementation roadmap focused on shipping a game.

## What This Session Adds

### Documentation

- `docs/DEVELOPMENT_REALITY_REPORT.md`
- `docs/DEFINITIVE_DECISIONS_REGISTER.md`
- `docs/IMPLEMENTATION_ROADMAP.md`
- `docs/KNOWN_ISSUES.md`
- `docs/COMPLETION_STAGE_GATES.md`
- `docs/PROJECT_INVENTORY.md`
- `docs/RUNBOOK.md`
- `README.md`

### Data-first gameplay foundation

- `data/houses.json`
- `data/resources.json`
- `data/units.json`
- `data/buildings.json`
- `data/faiths.json`
- `data/conviction-states.json`
- `data/bloodline-roles.json`
- `data/victory-conditions.json`
- `data/maps/ironmark-frontier.json`

### Runtime code

- `play.html`
- `src/game/styles.css`
- `src/game/main.js`
- `src/game/core/*.js`

### Validation

- `tests/data-validation.mjs`

## Current State After This Session

- The archive viewer remains intact.
- The design corpus remains intact.
- Bloodlines now has a first playable browser RTS vertical slice using the existing no-build static web stack.
