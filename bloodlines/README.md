# Bloodlines

Bloodlines is a dynasty-driven RTS project whose sole canonical working root is `deploy/bloodlines/`.

This folder now combines:

- The preserved Bloodlines design archive and canon corpus previously maintained in `D:\ProjectsHome\Bloodlines`
- The active browser-playable RTS prototype and implementation layer

The current prototype now includes:

- Ironmark as the playable house.
- Stonehelm as the skirmish opponent.
- Neutral frontier tribe camp pressure in the center marches.
- Capturable control points that generate territorial income and influence.

## Project Root

The authoritative Bloodlines root is:

- `deploy/bloodlines/`

No other Bloodlines folder should be treated as an active project root going forward. Historical sibling copies may remain on disk for preservation, but all future canon, docs, tooling, and gameplay work should happen here.

## Current Runtime Surfaces

- `index.html` - Bloodlines design archive viewer.
- `play.html` - playable Ironmark RTS prototype.

## Run

From the repository root:

```bash
python -m http.server 8077 --directory deploy
```

Then open:

- `http://localhost:8077/bloodlines/`
- `http://localhost:8077/bloodlines/play.html`

## Validate

From the repository root:

```bash
node deploy/bloodlines/tests/data-validation.mjs
```

## Structure

- `00_ADMIN` - project status, workflow, change tracking, ingestion history.
- `01_CANON` - canon lock, master memory, design guide, additive log, structure index.
- `02_SESSION_INGESTIONS` - preserved raw session ingestions and processed session memory.
- `03_PROMPTS` - startup prompts, handoff prompts, workbook prompts, AI-assist prompts.
- `04_SYSTEMS` to `13_AUDIO_VISUAL` - preserved game bible and subsystem canon.
- `14_ASSETS` to `19_ARCHIVE` - creative branches, prototype remnants, research, tasks, exports, and historical artifacts.
- `docs/` - reality report, roadmap, definitive decisions register, completion-stage gates, inventory, runbook, known issues.
- `data/` - data-first house, unit, building, faith, role, and map definitions.
- `src/game/` - browser RTS prototype code.
- `tests/` - lightweight data validation.

## Archive Rules

1. Nothing in the design archive should be deleted without explicit authorization.
2. Historical design context should be preserved even when later canon supersedes it.
3. New implementation should extend and connect to the preserved bible rather than flatten it into a generic RTS.
4. `deploy/bloodlines/` is now the one canonical location for both archive and implementation work.
