# Bloodlines — Directory Map

This document explains the purpose of each folder and key files in the Bloodlines project archive.

*Last updated: 2026-03-18*

---

## Root Level

| File | Purpose |
|------|---------|
| `CLAUDE.md` | Claude Code project context. Auto-loaded at session start. Contains startup protocol and archival rules summary. |
| `README.md` | Project overview, folder table, and archival rules for human readers. |

---

## 00_ADMIN — Project Administration

Operational backbone of the project. Status tracking, workflow rules, directory map, and change history.

| File | Purpose |
|------|---------|
| `PROJECT_STATUS.md` | Current development phase, status by area, conflict flags, update log. |
| `DIRECTORY_MAP.md` | This file. Purpose of each folder and key files. |
| `WORKFLOW_RULES.md` | Additive archival rules, session workflow, naming conventions, modification authority. |
| `CHANGE_LOG.md` | Record of all documentation changes, file reviews, and consolidation decisions across sessions. |
| `CURRENT_GAME_STATE.md` | Status checkpoint: what is defined, what is skeletal, what is open. Updated as design advances. |
| `INGESTION_REPORT_2026-03-15_player-manual.md` | Implementation report for the Player Manual (60-part) ingestion. |

---

## 01_CANON — Canonical Design Documents

The authoritative source of truth for all design decisions. Everything here is settled unless marked otherwise in CANONICAL_RULES.md.

| File | Purpose |
|------|---------|
| `BLOODLINES_MASTER_MEMORY.md` | Cumulative design memory. 80 sections. The single most complete reference for all design history. Append-only. |
| `BLOODLINES_DESIGN_BIBLE.md` | Structured 15-section design document. Organized by domain. |
| `CANONICAL_RULES.md` | Decision tracker. Every design element is marked SETTLED, OPEN, PROPOSED, or CONFLICT. |
| `BLOODLINES_APPEND_ONLY_LOG.md` | Raw ingestion records with full session content. Append-only. |
| `BLOODLINES_STRUCTURE_INDEX.md` | Cross-reference index for navigating the canon files. |
| `BLOODLINES_SESSION_BOOT_PROMPT.md` | Boot context prompt for starting a session with full design awareness. |
| `DESIGN_GUIDE.md` | Central consolidated design overview. 29 sections synthesizing all core systems into one navigable reference. |

---

## 02_SESSION_INGESTIONS — Session Memory Archive

Raw session data and structured ingestion records. Nothing here is ever deleted or summarized.

| File | Purpose |
|------|---------|
| `SESSION_INGESTION_INDEX.md` | Chronological log of all ingestion sessions with outcomes. |
| `SESSION_2026-03-15_first-substantive-ingestion.md` | Structured summary of first 4-session consolidated ingestion. |
| `SESSION_2026-03-15_player-manual-ingestion.md` | Structured summary of Player Manual (60-part) ingestion. |
| `SESSION_2026-03-15_player-manual-raw-input.md` | Full verbatim text of the Player Manual. Parts 1-60. Preserved exactly as provided. |

---

## 03_PROMPTS — Prompt Archive and Input Tools

Reusable prompts and structured design input tools for AI-assisted design work.

| File | Purpose |
|------|---------|
| `PROJECT_STARTUP_PROMPT.md` | 7-step startup protocol. Use at the start of every design session. |
| `SESSION_MEMORY_INGESTION_PROMPT.md` | 6-step workflow for processing raw session content into structured memory. |
| `MASTER_PROMPTS.md` | Prompt repository with specialized templates for design expansion and validation. |
| `INPUT_TO_APPLY.md` | Design input inbox. Paste new thoughts, answers, or decisions here for processing in the next session. |
| `INPUT_WORKBOOK.md` | Structured design question workbook across 14 sections. Targets the biggest design gaps. |

---

## 04_SYSTEMS — Game Systems

Detailed definitions of the seven core mechanical systems. Each system has its own file.

| File | Purpose |
|------|---------|
| `SYSTEM_INDEX.md` | Overview of all seven systems and their interdependencies. |
| `CONVICTION_SYSTEM.md` | Conviction: behavioral morality spectrum and governing philosophy. |
| `FAITH_SYSTEM.md` | Faith: covenant mechanics, intensity, costs, alignment, manifestations. |
| `POPULATION_SYSTEM.md` | Population: unified pool model, growth, housing, morale, dark extremes. |
| `RESOURCE_SYSTEM.md` | Resources: five primaries (gold, food, water, wood, stone) plus influence. Economy philosophy. |
| `TERRITORY_SYSTEM.md` | Territory: two-part control model (military + loyalty), governance, voluntary integration. |
| `DYNASTIC_SYSTEM.md` | Dynastic: bloodline members, family tree, succession, capture, marriage, lesser houses. |
| `BORN_OF_SACRIFICE_SYSTEM.md` | Born of Sacrifice: elite army forging through sacrifice of existing forces. |

---

## 05_LORE — World Lore and History

Narrative foundations of the game world. Currently partially populated.

| File | Purpose |
|------|---------|
| `LORE_INDEX.md` | Overview of lore areas and cross-references. |
| `WORLD_HISTORY.md` | World history narrative. Awaiting content. |
| `TIMELINE.md` | Timeline of major world events. Awaiting content. |

*Note: The Great Frost origin lore is currently in `memory/Bloodlines_Canonical_Design_Memory.md` Section 2. Future lore sessions should route content to both locations.*

---

## 06_FACTIONS — Factions and Great Houses

The nine founding houses, their identities, hair colors, and dynastic structures.

| File | Purpose |
|------|---------|
| `FACTION_INDEX.md` | Overview of faction structure and cross-references. |
| `FOUNDING_HOUSES.md` | All nine founding houses with naming history, hair colors, Trueborn lore, and conflict resolutions. |

---

## 07_FAITHS — The Ancient Faiths

The four ancient covenants with thematic depth and gameplay identity.

| File | Purpose |
|------|---------|
| `FAITH_INDEX.md` | Overview of the faith system and four covenants. |
| `FOUR_ANCIENT_FAITHS.md` | Full definitions of The Old Light, Blood Dominion, The Order, and The Wild with expanded gameplay identities. |

---

## 08_MECHANICS — Gameplay Mechanics

Player-facing mechanics documentation. Currently scaffold only.

| File | Purpose |
|------|---------|
| `MECHANICS_INDEX.md` | Index of mechanics areas. Awaiting content on combat, diplomacy, and economy mechanics. |

---

## 09_WORLD — World Geography and Map Design

Physical world, terrain, and map design. Currently scaffold only.

| File | Purpose |
|------|---------|
| `WORLD_INDEX.md` | Index of world design areas. Awaiting content on map structure, terrain, and geography. |

---

## 10_UNITS — Unit Types and Army Composition

Unit definitions and army rules.

| File | Purpose |
|------|---------|
| `UNIT_INDEX.md` | Core Ground Unit Progression (Locked 2026-04-25, Seventeenth Session Canon) — full 17-unit canonical ladder with Off/Def ratings, all four progression lines, Mounted Knight mechanics; Level 1-5 faith-tier unit definitions; army composition, recruitment, military buildings, troop promotion, champion emergence, Born of Sacrifice creation. Canonical unit names: Archers (not Bowmen), Boltmen (not Crossbowmen), Pikeguard (not Pike Square), Bulwark Guard (not Shield Wall). |

---

## 11_MATCHFLOW — Match Structure and Pacing

How a match begins, progresses, escalates, and resolves.

| File | Purpose |
|------|---------|
| `MATCH_STRUCTURE.md` | Four stages, victory conditions (5 paths with late-game escalation mechanics), 10-hour rhythm, early/mid/late game philosophy, end-of-match XP, recovery mechanics. |

---

## 12_UI_UX — Interface Design

User interface and UX notes. Currently scaffold only.

| File | Purpose |
|------|---------|
| `UI_NOTES.md` | UI design notes. Awaiting content. |

---

## 13_AUDIO_VISUAL — Art and Sound Direction

Visual identity and sound design direction. Currently scaffold only.

| File | Purpose |
|------|---------|
| `AUDIO_VISUAL_DIRECTION.md` | Art and audio direction notes. Awaiting content. |

---

## 14_ASSETS — Art and Reference Assets

Working directory for visual assets. Currently empty.

- `Concepts/` — Concept art and sketches
- `References/` — Reference images and inspiration
- `Temp/` — Temporary working files (safe to clean periodically)

---

## 15_PROTOTYPE — Prototype Code and Data

Working directory for prototype development. Currently empty. Future phase.

- `Design/` — Design mockups and interactive prototypes
- `Code/` — Source code for playable prototypes
- `Data/` — Data files, configs, and test datasets

---

## 16_RESEARCH — Research and References

Competitive analysis, reference game studies, and other research materials.

| File | Purpose |
|------|---------|
| `RESEARCH_INDEX.md` | Index of research areas. Awaiting content. |

---

## 17_TASKS — Task Tracking and Design Gaps

Active task backlog, next steps, and open design questions.

| File | Purpose |
|------|---------|
| `TASK_BACKLOG.md` | Prioritized development task backlog. |
| `NEXT_STEPS.md` | 10 prioritized design work items ordered by blocking impact. |
| `OPEN_QUESTIONS.md` | Full catalog of unresolved design questions by system area, with severity labels (BLOCKING, IMPORTANT, ENRICHING, FUTURE). |

---

## 18_EXPORTS — Exported Materials

Finished documents and materials for external use.

| File | Purpose |
|------|---------|
| `workbook.html` | HTML version of the design input workbook. |

---

## 19_ARCHIVE — Historical Archive

Superseded documents and historical artifacts. Nothing here is ever deleted.

| File | Purpose |
|------|---------|
| `HANDOFF_2026-03-15_initialization.md` | Session handoff from the project initialization session (2026-03-15). All initialization work confirmed complete. |

---

## memory/ — External Canonical Design Memory

A secondary canonical design memory maintained per external ingestion instruction. Uses a 15-section structure organized by world and system domain. This is separate from the primary canon in `01_CANON/`.

| File | Purpose |
|------|---------|
| `Bloodlines_Canonical_Design_Memory.md` | 15-section canonical memory. Currently contains: Section 2 — The Great Frost world origin and dynasty rebirth doctrine. |

---

*This map is updated whenever files are added, moved, or the directory structure changes.*
