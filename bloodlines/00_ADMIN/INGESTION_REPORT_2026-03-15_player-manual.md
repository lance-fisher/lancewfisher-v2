# Ingestion Report: Official Player Manual (60 Parts)

**Date:** 2026-03-15
**Source:** Official Player Manual, Parts 1-60 (~40,000 words)
**Type:** Comprehensive game design document covering all core systems, new player guidance, strategic philosophy, and covenant-specific play guides

---

## Directories Reviewed

| Directory | Purpose | Reviewed |
|-----------|---------|----------|
| 00_ADMIN/ | Project status, workflow rules, directory map | Yes |
| 01_CANON/ | Master memory, design bible, canonical rules, append-only log | Yes |
| 02_SESSION_INGESTIONS/ | Raw session data archive, ingestion index | Yes |
| 04_SYSTEMS/ | All 7 core system files | Yes |
| 06_FACTIONS/ | Founding houses | Yes |
| 07_FAITHS/ | Four ancient faiths | Yes |
| 08_MECHANICS/ | Mechanics index | Yes |
| 10_UNITS/ | Unit index | Yes |
| 11_MATCHFLOW/ | Match structure | Yes |

## Files Reviewed Before Changes

| File | Purpose |
|------|---------|
| 01_CANON/BLOODLINES_MASTER_MEMORY.md | Cumulative design memory (50 existing sections) |
| 01_CANON/CANONICAL_RULES.md | Settled vs open design decisions |
| 00_ADMIN/WORKFLOW_RULES.md | Archival rules and session workflow |
| 00_ADMIN/PROJECT_STATUS.md | Current development phase and status |
| 02_SESSION_INGESTIONS/SESSION_INGESTION_INDEX.md | Chronological ingestion log |
| 02_SESSION_INGESTIONS/SESSION_2026-03-15_first-substantive-ingestion.md | Format reference for session files |
| 04_SYSTEMS/FAITH_SYSTEM.md | Faith system design content |
| 04_SYSTEMS/CONVICTION_SYSTEM.md | Conviction system design content |
| 04_SYSTEMS/POPULATION_SYSTEM.md | Population system design content |
| 04_SYSTEMS/RESOURCE_SYSTEM.md | Resource system design content |
| 04_SYSTEMS/TERRITORY_SYSTEM.md | Territory system design content |
| 04_SYSTEMS/DYNASTIC_SYSTEM.md | Dynastic system design content |
| 04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md | Born of Sacrifice system design content |
| 06_FACTIONS/FOUNDING_HOUSES.md | Founding house list and visual identity |
| 07_FAITHS/FOUR_ANCIENT_FAITHS.md | Four covenant definitions |
| 10_UNITS/UNIT_INDEX.md | Unit types and army composition |
| 11_MATCHFLOW/MATCH_STRUCTURE.md | Match phases and victory conditions |
| 08_MECHANICS/MECHANICS_INDEX.md | Mechanics cross-reference |

## Files Updated

| File | What Was Appended | Why This Destination |
|------|-------------------|---------------------|
| 01_CANON/BLOODLINES_MASTER_MEMORY.md | 29 new sections (51-79) covering all major new concepts | This is the cumulative design memory, the primary destination for all design content |
| 01_CANON/CANONICAL_RULES.md | 22 new entries (17 PROPOSED, 2 CONFLICT, 3 in existing tables) | Tracks settled vs open design decisions |
| 04_SYSTEMS/FAITH_SYSTEM.md | Player Manual expansion: terminology, house neutrality, progressive scaling, Level 4 divergence, manifestations, covenant gameplay depth | Faith system is a core system file for all faith-related mechanics |
| 04_SYSTEMS/CONVICTION_SYSTEM.md | Player Manual expansion: dual definition, pattern-based tracking, legitimacy interaction | Conviction system is a core system file for conviction mechanics |
| 04_SYSTEMS/POPULATION_SYSTEM.md | Player Manual expansion: imperative asset, training paths, recovery, dark extremes, expansion timing | Population system is a core system file for population mechanics |
| 04_SYSTEMS/RESOURCE_SYSTEM.md | Player Manual expansion: resource count conflict note, influence expansion, long-form economy, currency dominance expansion | Resource system is a core system file for economic mechanics |
| 04_SYSTEMS/TERRITORY_SYSTEM.md | Player Manual expansion: loyalty collapse dynamics, governance victory expansion, expansion vs consolidation, defensive ideology | Territory system is a core system file for territorial mechanics |
| 04_SYSTEMS/DYNASTIC_SYSTEM.md | Player Manual expansion: battlefield presence, captured heir consequences, war heroes, mixed bloodline dynasties, starting leader lore, lesser house fracture | Dynastic system is a core system file for dynasty mechanics |
| 04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md | Player Manual expansion: specific creation concept, troop promotion, champion emergence | Born of Sacrifice is a core system file for elite army mechanics |
| 06_FACTIONS/FOUNDING_HOUSES.md | Player Manual 8-house list with hair colors, Trueborn expanded lore, conflict documentation | This is the canonical location for founding house information |
| 07_FAITHS/FOUR_ANCIENT_FAITHS.md | Expanded gameplay identity for all four covenants with strategic depth | This is the canonical location for covenant descriptions |
| 10_UNITS/UNIT_INDEX.md | Troop promotion, champion emergence, Born of Sacrifice creation concept, sustainment dynamics, bloodline commander presence | This is the canonical location for unit and army composition information |
| 11_MATCHFLOW/MATCH_STRUCTURE.md | Match scale, 10-hour rhythm, early/mid/late game philosophy, victory escalation, end-of-match rewards, recovery mechanics | This is the canonical location for match flow and pacing |
| 02_SESSION_INGESTIONS/SESSION_INGESTION_INDEX.md | Two new session log entries for the Player Manual ingestion | Chronological session tracking |
| 00_ADMIN/PROJECT_STATUS.md | Updated phase description, development focus, status summary, update log entry | Current project state documentation |

## New Files Created

| File | Purpose | Why Created |
|------|---------|-------------|
| 02_SESSION_INGESTIONS/SESSION_2026-03-15_player-manual-ingestion.md | Structured session summary | Required by ingestion workflow: every session gets a summary file |
| 02_SESSION_INGESTIONS/SESSION_2026-03-15_player-manual-raw-input.md | Full verbatim raw input preservation | Required by archival rules: raw input must be preserved in full |
| 00_ADMIN/INGESTION_REPORT_2026-03-15_player-manual.md | This implementation report | Required by ingestion protocol |

## Confirmation: No Existing Content Removed or Replaced

All changes were additive only. Every edit used the append pattern: matching the last line of existing content and adding new content after it. No existing text was modified, removed, summarized, condensed, or replaced in any file.

## Conflicts Identified

### Conflict 1: Founding House Count and Names
- **Existing canon (SETTLED):** 9 founding houses — Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest
- **Player Manual:** 8 founding houses — Trueborn, Highborne, Ironmark, Hartborne, Whitehall, Westgrave, Goldgrave, Oldcrest
- **Differences:** Player Manual omits Stonehelm. Player Manual uses Hartborne (not Hartvale) and Westgrave (not Westland). Player Manual list matches the Session 3 variant previously archived as superseded.
- **Resolution needed:** Explicit decision on house count (8 or 9) and names
- **Both preserved:** Yes, in FOUNDING_HOUSES.md, MASTER_MEMORY.md, and CANONICAL_RULES.md

### Conflict 2: Primary Resource Count
- **Existing canon (SETTLED):** 5 primary resources — gold, food, water, wood, stone
- **Player Manual:** 4 primary resources — gold, food, water, stone (wood absent)
- **Resolution needed:** Explicit decision on whether wood is a primary resource
- **Both preserved:** Yes, in RESOURCE_SYSTEM.md, MASTER_MEMORY.md, and CANONICAL_RULES.md

## New Concepts Introduced (Not Previously in Canon)

| Concept | Source Parts | Routed To |
|---------|-------------|-----------|
| End-of-match XP and progression system | Parts 3, 54 | Master Memory §52, Match Structure |
| Map scale (~10x typical RTS) | Parts 1, 3 | Master Memory §51, Match Structure |
| 6-10 hour stress test as design target | Parts 1, 3, 57 | Master Memory §51, Match Structure |
| Grand structure lockouts (Trade Exchange vs War Foundry) | Part 12 | Master Memory §62, Canonical Rules |
| Loyalty collapse dynamics (gradual accumulation) | Part 52 | Master Memory §73, Territory System |
| Recovery as gameplay mechanic | Part 53 | Master Memory §74, Match Structure |
| 10-hour match rhythm (7 phases) | Part 57 | Master Memory §75, Match Structure |
| Memory as core game theme | Part 58 | Master Memory §76, Canonical Rules |
| Defense as active ideology | Part 44 | Master Memory §72, Territory System, Canonical Rules |
| Moral economy concept | Part 46 | Master Memory §72 |
| Strong vs loud realm distinction | Part 43 | Master Memory §72 |
| Reckless success punishment | Part 42 | Master Memory §72 |
| Opponent reading guide | Part 56 | Master Memory §78 |
| Common new player errors | Part 55 | Master Memory §77 |
| Troop promotion after battles | Part 16 | Master Memory §66, Unit Index, Born of Sacrifice System |
| Champion emergence from battle | Part 16 | Master Memory §66, Unit Index, Born of Sacrifice System |
| Captured heir homeland penalties | Part 14, 34 | Master Memory §64, Dynastic System, Canonical Rules |
| Mixed bloodline new dynasty emergence | Part 14 | Master Memory §64, Dynastic System, Canonical Rules |
| Lesser house fracture under extreme conditions | Part 9 | Master Memory §58, Dynastic System, Canonical Rules |
| Starting leader hidden synergies | Part 7 | Master Memory §69, Dynastic System, Canonical Rules |
| Conviction dual definition | Part 2, 27 | Master Memory §72, Conviction System, Canonical Rules |
| Faith replaces "arcane" | Part 11 | Faith System, Canonical Rules |
| Houses start unaligned | Part 11 | Faith System, Canonical Rules |
| Currency adoption forcing wars | Parts 17, 37 | Master Memory §67, Resource System |
| Prestige dispute wars (5-10 bloodline members) | Parts 17, 40 | Master Memory §67, Canonical Rules |
| Faith divine claim to rule | Parts 17, 38 | Master Memory §67, Canonical Rules |
| Specific hair color assignments per house | Part 8 | Master Memory §56, Founding Houses |
| Known Unknown Rule | Part 19 | Master Memory §79 |
| Dark extremes causing irreversible world population decline | Parts 5, 11 | Master Memory §55, Population System, Faith System, Canonical Rules |
| Water denial prevention mechanics | Part 4 | Master Memory §54, Canonical Rules |

## Organizational Notes for Future Work

1. The Player Manual content is significantly more detailed than existing canon for most systems. Future sessions should reference both the Master Memory sections and the system file expansions.
2. The two CONFLICT entries in CANONICAL_RULES.md should be resolved in the next design session with the project owner.
3. The Player Manual introduces extensive new player guidance material (Parts 20-60) that could eventually become the basis for an actual in-game tutorial or manual. This material is currently preserved in the raw input file and summarized across Master Memory sections 70-78.
4. The "Known Unknown Rule" (Part 19, Master Memory §79) establishes a protocol for handling requests about house names not present in the canonical archive.
5. Hair color assignments in the Player Manual are tied to the 8-house list, not the settled 9-house list. When house list conflict is resolved, hair colors may need reassignment if the final list differs.

---

*This report was generated as part of the Player Manual ingestion on 2026-03-15. It documents only the changes made during this ingestion and must not be modified after creation.*
