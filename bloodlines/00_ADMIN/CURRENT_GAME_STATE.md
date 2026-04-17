# Bloodlines: Current Game State

**Purpose:** A structured overview of where the Bloodlines project stands right now. This document answers: what is defined, what is rich, what is skeletal, what is close to final, and what remains wide open.

**Last Updated:** 2026-03-15
**Project Phase:** Phase 1, Design Content Population

---

## Project Timeline

| Date | Event |
|------|-------|
| 2026-03-15 | Project initialization, 20-folder structure, 35 files scaffolded |
| 2026-03-15 | First substantive design ingestion (4 sessions consolidated) |
| 2026-03-15 | All 7 core system files populated with detailed content |
| 2026-03-15 | 40+ canonical decisions recorded in CANONICAL_RULES.md |
| 2026-03-15 | Founding house naming resolved (9 houses, unique suffixes) |
| 2026-03-15 | Documentation consolidation: docs/ directory and central guide created |

---

## System-by-System Status

### Richly Defined Systems

These systems have substantial design content across multiple files and multiple sessions. Core concepts are locked. Design intent is clear. Mechanical details exist at a meaningful level.

| System | Key Files | Confidence |
|--------|-----------|------------|
| **Faith System** | `04_SYSTEMS/FAITH_SYSTEM.md`, `07_FAITHS/FOUR_ANCIENT_FAITHS.md` | High. Four covenants defined with thematic depth. Three-component model locked. Intensity/alignment mechanics described. Faith actions cataloged. Spiritual manifestations proposed. |
| **Population System** | `04_SYSTEMS/POPULATION_SYSTEM.md` | High. Unified pool model locked. Growth drivers identified. Loyalty/morale mechanics described. Housing as power replacement locked. Central tensions articulated. |
| **Resource System** | `04_SYSTEMS/RESOURCE_SYSTEM.md` | High. Five primary resources locked. Economic philosophy stated. Building-to-resource connections defined. Currency dominance concept proposed. |
| **Territory System** | `04_SYSTEMS/TERRITORY_SYSTEM.md` | High. Two-part control model locked. Loyalty consequences defined. Voluntary integration proposed. |
| **Dynastic System** | `04_SYSTEMS/DYNASTIC_SYSTEM.md` | High. 20-member cap locked. Training paths, roles, family tree UI, capture mechanics, marriage diplomacy, lesser houses all described with meaningful depth. |

### Partially Defined Systems

These systems have core concepts established but significant mechanical gaps remain.

| System | Key Files | What Exists | What Is Missing |
|--------|-----------|-------------|-----------------|
| **Conviction System** | `04_SYSTEMS/CONVICTION_SYSTEM.md` | Core concept locked. Distinction from Faith articulated. Action categories identified. Directions listed. | Specific triggers and thresholds. Measurable scoring. Visibility to player. Interaction formulas with other systems. Per-faction, per-territory, or per-army scope. |
| **Born of Sacrifice** | `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md` | Core mechanic locked. Example ratio given (5:1). Benefits listed. Naming system described. Design intent clear. | Specific sacrifice requirements. Faith-specific variations. Cooldowns/limits. Diplomatic consequences. Relationship to conviction. Whether bloodline members can be sacrificed. |
| **Army/Military** | `10_UNITS/UNIT_INDEX.md` | Level 1 units fully defined (5 types). Recruitment slider system locked. Army requirements listed. | All units beyond Level 1. Combat system. Unit counters. Army morale in battle. Siege mechanics. Cavalry (referenced but not defined). |
| **Match Structure** | `11_MATCHFLOW/MATCH_STRUCTURE.md` | Four stages locked. Pacing philosophy clear. Victory conditions identified. Long match design intent stated. | Specific timing within stages. Transition triggers. Early/mid/late game economic benchmarks. Match options and settings. |

### Scaffold-Only Areas

These areas have files and folder structures created but contain no substantive design content yet.

| Area | Files | Status |
|------|-------|--------|
| **World History / Lore** | `05_LORE/WORLD_HISTORY.md`, `05_LORE/TIMELINE.md` | Awaiting first content |
| **UI/UX Design** | `12_UI_UX/UI_NOTES.md` | Awaiting first content |
| **Audio/Visual Direction** | `13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md` | Awaiting first content |
| **Combat Mechanics** | `08_MECHANICS/MECHANICS_INDEX.md` | Index only, no content |
| **World Geography** | `09_WORLD/WORLD_INDEX.md` | Index only, no content |
| **Research** | `16_RESEARCH/RESEARCH_INDEX.md` | Index only, no content |
| **Prototype** | `15_PROTOTYPE/` | Empty, future phase |
| **Assets** | `14_ASSETS/` | Empty, future phase |

---

## Decision Confidence Levels

### Decisions That Appear Close to Final

These have been consistent across all four ingested sessions and are marked SETTLED in canonical rules:

- The nine founding houses and their naming principles
- The four covenants and their thematic identities
- Five primary resources (gold, food, water, wood, stone)
- Population as unified pool with housing as capacity driver
- Territory requiring military control AND population loyalty
- Five-level progression with Level 4 as irreversible divergence
- 20 active bloodline members with dormancy beyond cap
- Born of Sacrifice as late-game elite army forging
- Four match stages with civilizational arc pacing
- Recruitment via adjustable sliders, not hard doctrines
- Scouts removed, swordsmen serve dual role
- Trueborn neutral city with coalition response

### Decisions That Remain Clearly Unresolved

- How houses differ mechanically (faction asymmetry)
- What units exist beyond Level 1
- The technology tree and progression path
- Specific grand faith structures per covenant
- The combat system
- Map structure (hex, province, continuous)
- Succession rules when the dynasty leader dies
- Character trait inheritance model
- Detailed conviction measurement and scoring
- Victory condition mechanical implementations
- AI kingdom behavior and doctrine
- Multiplayer infrastructure and matchmaking
- Diplomacy interface and treaty mechanics

### Areas Needing Clarification Before Building

These are not open questions in the creative sense but structural ambiguities that need resolution before any prototype work:

1. **Match flow specifics:** What are the exact player actions in the first 10 minutes? This determines what gets prototyped first.
2. **Map structure:** Hex grid, province map, or continuous terrain? This is a foundational technical decision.
3. **Combat resolution:** Real-time with direct control, auto-resolve, or hybrid? This determines the entire military UX.
4. **Population scope:** Is population tracked globally (current design) or per-territory? This affects every system that touches population.
5. **Conviction visibility:** Does the player see their conviction score, or is it partially hidden? This affects UI design and strategic transparency.

---

## Central Reference Files

The following files currently serve as the primary design references:

| File | Role |
|------|------|
| `docs/USER_GUIDE.md` | Central consolidated overview (this companion) |
| `01_CANON/BLOODLINES_MASTER_MEMORY.md` | Full cumulative design memory (50 sections, richest single file) |
| `01_CANON/BLOODLINES_DESIGN_BIBLE.md` | Structured 15-section design document |
| `01_CANON/CANONICAL_RULES.md` | Decision tracker (SETTLED/OPEN/PROPOSED) |
| `00_ADMIN/PROJECT_STATUS.md` | Phase tracking and update log |
| `04_SYSTEMS/*.md` | Seven individual system deep-dives |

---

## What Has Changed Since Project Initialization

Between initialization and now:
- All 7 core system files transitioned from scaffold to populated
- Master Memory grew from 1 initialization entry to 50 detailed sections
- Design Bible grew from headings-only to 15 fully populated sections
- Canonical Rules grew from empty to 40+ tracked decisions
- Session ingestion pipeline was tested with 4-session consolidated ingestion
- Founding house naming was debated and resolved
- `docs/` directory created with consolidated guide and supporting documents

---

## What This Project Needs Next

The project is in a strong foundation state. The design identity is clear. Core systems are well-articulated at the conceptual level. The archival infrastructure is solid.

What it needs now is **mechanical depth** in the areas that block prototyping:
1. The first 10 minutes of gameplay, step by step
2. Map and territory structure
3. Combat resolution model
4. Technology and progression tree
5. Units beyond Level 1

See `docs/NEXT_STEPS.md` for the prioritized action plan.
See `docs/INPUT_WORKBOOK.md` for structured questions that close the biggest design gaps.

---

*This document is updated as the project advances. New status information is appended, not used to replace historical state.*
