# Bloodlines: Next Steps

**Purpose:** A prioritized, specific action plan for the immediate design work that should happen before moving into implementation. These are not generic recommendations. They are derived from the actual state of the project after reviewing all existing files.

**Last Updated:** 2026-03-15

---

## Priority Framework

Steps are ordered by **blocking impact**: what prevents other work from proceeding. A design decision that blocks 5 downstream systems is higher priority than one that enriches 1 system.

---

## Priority 1: Define the First 10 Minutes of a Match

**Why this matters now:** Every other design decision about early game pacing, tutorial flow, build orders, and first impressions depends on having a concrete step-by-step picture of what the player does when a match begins. Without this, the team cannot prototype, test, or evaluate any early game mechanic.

**What needs to happen:**
- Walk through the player's experience from match start to first military engagement
- Define what the player sees, what choices they make, and in what order
- Specify the starting conditions: what buildings, units, and resources the player begins with
- Define when and how the first resource gathering, building, and recruitment actions occur
- Establish the pace: how long before the player encounters another faction or tribe?

**Dependencies this unblocks:**
- Prototype slice definition
- Tutorial design
- UI/UX for early game
- Build order balance testing

> See `docs/INPUT_WORKBOOK.md` Section 2 (Match Flow Clarification) for guided questions.

---

## Priority 2: Lock the Map and Territory Structure

**Why this matters now:** The map structure is a foundational technical decision. Whether the world uses hex grids, province maps, or continuous terrain affects rendering, pathfinding, territory boundaries, resource node placement, naval zones, and every system that references "territory." This cannot be deferred.

**What needs to happen:**
- Decide on map representation (hex, province, continuous, or hybrid)
- Define territory boundaries and how they are drawn
- Specify terrain types and their gameplay effects
- Define how resource nodes are distributed across the map
- Establish how territory size relates to governance difficulty
- Define neutral, contested, and sacred site territory types

**Dependencies this unblocks:**
- World generation implementation
- Territory loyalty mechanics
- Resource node placement
- Pathfinding architecture
- Naval zone definition

> See `docs/INPUT_WORKBOOK.md` Section 10 (World/Lore Clarification) for guided questions.

---

## Priority 3: Define the Combat Resolution Model

**Why this matters now:** The combat model determines the entire military UX. "Command and Conquer style" implies real-time direct control, but the dynastic and population systems suggest something more layered. This decision affects unit design, army composition, siege mechanics, and the role of bloodline commanders.

**What needs to happen:**
- Decide: real-time with direct unit control, auto-resolve, or hybrid?
- Define how armies engage (formation-based, free-form, zone control)
- Establish the role of terrain in combat
- Define how bloodline commanders affect battlefield outcomes
- Specify how morale, conviction, and faith affect combat performance
- Establish siege mechanics at a conceptual level

**Dependencies this unblocks:**
- Unit design beyond Level 1
- Army composition balance
- Bloodline commander mechanics
- Born of Sacrifice elite unit differentiation
- Military building progression

---

## Priority 4: Design Units Beyond Level 1

**Why this matters now:** Level 1 units are fully defined, but the game spans 5 levels. Without knowing what units appear at Levels 2-5, the progression system cannot be balanced, the technology tree cannot be designed, and the military experience across a full match is unknown.

**What needs to happen:**
- Define unit types for Levels 2, 3, 4, and 5
- Establish how unit progression works (upgrades, unlocks, specializations)
- Define cavalry (referenced in spearmen description but not yet designed as a unit)
- Design how faith-specific units work (if they exist)
- Design how house-specific units work (if faction asymmetry applies to military)
- Establish the role of siege units across levels

**Dependencies this unblocks:**
- Military balance across the full match arc
- Technology tree design
- Born of Sacrifice input requirements
- Building prerequisites for unit training

> See `docs/INPUT_WORKBOOK.md` Section 9 (Unit, Structure, and Military Doctrine) for guided questions.

---

## Priority 5: Build the Technology Tree

**Why this matters now:** The technology tree is the connective tissue between buildings, units, progression levels, and strategic choices. Without it, there is no clear path from Level 1 to Level 5.

**What needs to happen:**
- Define what is researched vs what is unlocked by level progression
- Map building prerequisites and unlock chains
- Define faith-specific technologies or abilities
- Establish economic upgrades and their progression
- Determine whether houses have unique technology branches
- Design the Level 4 divergence point as a technology/structure choice

**Dependencies this unblocks:**
- Full match progression from start to endgame
- Build order strategy
- Strategic specialization paths
- Grand faith structure prerequisites

---

## Priority 6: Formalize Conviction Mechanics

**Why this matters now:** Conviction is conceptually strong but mechanically undefined. It is described as "shaped permanently by actions," but there is no specification of which actions, how much each contributes, what thresholds exist, or how the player perceives their conviction state. Without this, conviction cannot be implemented or tested.

**What needs to happen:**
- Define the specific actions that increase or decrease conviction, and by how much
- Decide whether conviction is a single axis, multiple axes, or a multi-dimensional space
- Decide whether conviction is visible to the player as a number, bar, or inferred from effects
- Define conviction thresholds and what unlocks or locks at each threshold
- Specify how conviction interacts with faith alignment mechanically
- Define how conviction affects diplomacy and AI kingdom perception

> See `docs/INPUT_WORKBOOK.md` Section 5 (Conviction Design Clarification) for guided questions.

---

## Priority 7: Define How Houses Feel Different

**Why this matters now:** The nine houses have names, themes, and visual identity, but no mechanical differentiation. If all houses play identically except for cosmetics, a major design pillar loses its strategic weight. House asymmetry is what makes house selection meaningful.

**What needs to happen:**
- Decide the degree of asymmetry: cosmetic only, minor bonuses, or fully unique mechanics?
- Define starting conditions per house (if different)
- Define unique buildings, units, or abilities per house (if any)
- Define how house identity interacts with faith selection
- Establish whether certain houses have natural affinities for certain faiths

---

## Priority 8: Detail the Grand Faith Structures

**Why this matters now:** Grand faith structures are the capstone of the faith system and the centerpiece of Level 4 divergence. They are described as "each civilization's final spiritual, political, or metaphysical institutional expression," but no specific structures have been named or designed.

**What needs to happen:**
- Name and describe the grand faith structure for each of the four covenants
- Define what each structure unlocks or enables
- Establish build costs and prerequisites
- Define how these structures interact with the faith victory path
- Specify whether the structure is destructible and what happens if it falls

---

## Priority 9: Design the Diplomacy System

**Why this matters now:** Dynastic interactions (marriage, capture, ransom, assassination) are richly described, but the diplomatic framework around them is not. How do dynasties formally relate to each other? What treaty types exist? How does diplomatic AI work?

**What needs to happen:**
- Define treaty types (alliance, non-aggression, trade agreement, vassal, marriage pact)
- Establish diplomatic states between dynasties
- Define how trust/reputation is tracked
- Specify the diplomatic UI
- Establish AI diplomatic behavior models

---

## Priority 10: Define a First Playable Design Slice

**Why this matters now:** At some point, design must transition to prototyping. A first playable slice defines the smallest subset of the game that demonstrates the core experience.

**What needs to happen:**
- Identify the minimum viable set of systems for a playable slice
- Define a scope that can be built and tested in isolation
- Likely candidates: resource gathering, building, population growth, territory control, Level 1 military
- Decide whether faith and dynasty are in the first slice or added in a second iteration
- Define success criteria for the first playable

**Recommendation:** The deterministic RTS architecture bootstrap prompt (Master Memory Section 48) was already generated for this purpose. The first slice should validate that architecture with the core economic loop and Level 1 military.

---

## What Should NOT Be Prioritized Yet

The following areas are important but should not consume design time until the above priorities are addressed:

- **World history and lore timeline** - Rich lore is valuable but does not block gameplay mechanics
- **UI/UX detailed wireframes** - Need to know what the player does before designing how they see it
- **Audio/visual direction** - Aesthetic decisions are downstream of gameplay decisions
- **Campaign progression** - Single match design must be solid before campaign structure
- **Multiplayer infrastructure** - Core gameplay must work in single-player-plus-AI before networking

---

## Cross-References

| Document | Relevance |
|----------|-----------|
| `docs/USER_GUIDE.md` | Provides context for why each next step matters |
| `docs/INPUT_WORKBOOK.md` | Contains guided questions for addressing each priority |
| `docs/OPEN_QUESTIONS.md` | Full catalog of unresolved design areas |
| `docs/CURRENT_GAME_STATE.md` | Shows what already exists to build on |
| `01_CANON/CANONICAL_RULES.md` | Shows what is already locked vs open |

---

*Next steps are updated as priorities shift. Completed items are moved to a Completed section rather than deleted.*
