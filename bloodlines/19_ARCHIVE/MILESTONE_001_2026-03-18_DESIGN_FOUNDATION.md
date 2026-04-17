# MILESTONE 001 — Design Foundation
**Date:** 2026-03-18
**Status:** Canonical snapshot. This milestone marks the state of the design after four ingestion sessions. All creative branches from this point forward are speculative and separate from canon.

---

## What Is Settled at This Milestone

This milestone represents the completion of the design foundation phase. All items below are LOCKED or SETTLED canon as of this date. Creative exploration documents may branch from this point. To revert to this canonical state, disregard any creative branch documents and treat the files listed below as authoritative.

### Core Architecture
- 20-folder numbered project structure
- Additive-only archival rules (nothing deleted, summarized, or replaced)
- Seven core game systems: Conviction, Faith, Population, Resource, Territory, Dynastic, Born of Sacrifice

### World and Identity
- Title: Bloodlines (locked)
- Genre: Massively large-scale medieval RTS, Warcraft 3-aligned
- World origin: The Great Frost (canonical lore, verbatim text preserved in Master Memory)
- Nine Founding Houses: Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest
- All nine hair colors assigned and locked
- Four Ancient Covenants: The Old Light, Blood Dominion, The Order, The Wild
- All four faiths have Light Doctrine and Dark Doctrine paths (fully specified)

### Three Pillars of Every Civilization
- Dynasty (lineage and political identity)
- Faith (worldview and cultural systems)
- Conviction (moral character and leadership integrity)
- All three independent of each other. Any combination valid.

### Conviction System
- Axis: Moral / Neutral / Cruel (High/Low deprecated)
- Conviction is a prominent featured system, not a background modifier
- Milestones at both ends carry genuine strategic power
- Faith and Conviction are explicitly independent (never forced into alignment)

### Resources — Six Primary (LOCKED)
1. Gold
2. Food
3. Water
4. Wood
5. Stone
6. Iron (added fourth ingestion — mid-to-late game critical)

### Military
- Level 1 units locked: Militia, Swordsmen, Spearmen, Hunters, Bowmen
- Scouts removed (Swordsmen serve dual purpose)
- Cavalry confirmed as major military system
- Wild faith: animal cavalry (bears and other mounts)
- Levels 2-5 units: OPEN DESIGN GAP

### Born of Sacrifice
- Redesigned: population-constrained army lifecycle system
- Unit recycling due to finite population — not one-time ritual sacrifice
- Units gain experience through battle, defense, caravan escort

### Buildings — 21 Total
- Civic (4): Settlement Hall, Housing District, Well, Granary
- Economic (7): Farm, Ranch, Market, Lumber Camp, Quarry, Iron Mine, Stable
- Military (5): Barracks, Training Grounds, Armory, Fortified Keep, Watchtower
- Faith (3): Shrine, Temple, Grand Sanctuary
- Special (3): Dynasty Estate, Academy, Treasury

### Match Structure
- Five levels (Level 4 = irreversible divergence, Level 5 = apex)
- Match scale: 2-10+ hours, up to 10 players
- Bloodline members cap: 20 active

### Victory Conditions
- Military Conquest: LOCKED
- Economic/Currency Dominance: SETTLED
- Faith/Divine Right: PROPOSED
- Territorial Governance Attraction: PROPOSED
- Dynastic Prestige: PROPOSED

### Technical Direction
- Deterministic RTS architecture: fixed-tick sim, seeded RNG, command buffer
- No Math.random or floats in sim layer

---

## What Is Still Open at This Milestone

The following are confirmed design gaps — areas where the design has not yet committed to specifics. Creative branches may propose directions for these.

- Unit types for Levels 2-5
- All building costs, build times, prerequisites
- Unit stats and combat numbers
- Population growth rate specifics
- Resource production rates
- Map design and territory types
- Terrain system
- World generation parameters
- AI kingdom behavior
- Multiplayer structure and lobby design
- Great House strategic identities (names and hair locked, personality not specified)
- Political event system
- Conviction action weights (what actions, how much shift)
- Specific faith ritual mechanics, costs, and durations
- Naval unit types and combat
- Tech/upgrade tree structure

---

## Primary Canon Files at This Milestone

| File | Content |
|------|---------|
| `01_CANON/BLOODLINES_MASTER_MEMORY.md` | Cumulative append-only design memory, Sections 1-83 |
| `01_CANON/CANONICAL_RULES.md` | All SETTLED/PROPOSED/OPEN decisions, four ingestion tables |
| `01_CANON/BLOODLINES_DESIGN_BIBLE.md` | 15-section structured design reference |
| `01_CANON/DESIGN_GUIDE.md` | 29-section central design overview |
| `04_SYSTEMS/CONVICTION_SYSTEM.md` | Full conviction system with Moral/Cruel axis, milestones |
| `04_SYSTEMS/FAITH_SYSTEM.md` | All four faiths, both doctrine paths each |
| `04_SYSTEMS/POPULATION_SYSTEM.md` | Population model |
| `04_SYSTEMS/RESOURCE_SYSTEM.md` | Six resources including iron, currency dominance |
| `04_SYSTEMS/TERRITORY_SYSTEM.md` | Territory and loyalty |
| `04_SYSTEMS/DYNASTIC_SYSTEM.md` | Dynasty mechanics |
| `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md` | Redesigned lifecycle mechanic |
| `06_FACTIONS/FOUNDING_HOUSES.md` | Nine houses, hair colors, naming systems |
| `07_FAITHS/FOUR_ANCIENT_FAITHS.md` | Faith system details |
| `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE.md` | Complete synthesized 40-section bible |

---

## Creative Branch Documents Spawned From This Milestone

| Document | Status | Description |
|----------|--------|-------------|
| `14_ASSETS/CREATIVE_BRANCH_001_WORLD_EXPANSION.md` | Active | Unit progression, house identities, territory types, event system |

---

*This milestone is a read-only reference point. The canon files above continue to evolve. If a creative branch is rejected, the canon files remain untouched. The branch document is the only thing that changes.*
