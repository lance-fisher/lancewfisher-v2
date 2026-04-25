# Bloodlines — Structure Index

This document maps all major topics and design areas to their locations within the Bloodlines memory system. It serves as a navigation guide for future sessions to locate specific content quickly.

This index must be expanded whenever new categories appear. It must never be shortened. Organization is allowed. Reduction is not allowed.

---

## Master Memory Sections

The following sections exist within `01_CANON/BLOODLINES_MASTER_MEMORY.md`:

### 1. Project Identity
- Game title (locked as Bloodlines)
- Genre (massively large-scale medieval RTS)
- Design philosophy (Command & Conquer inspired)
- Core design pillars (Territory, Population, Faith, Dynasty)

### 2. Core Design Philosophy
- C&C style base building, offense/defense, resource gathering
- Multiple viable playstyles
- Balance, replayability, strategic depth
- Long-form match support

### 3. Canonical Vocabulary and Definitions
- Bloodline = family
- Conviction = philosophy
- Faith = religion covenant

### 4. Match Scale, Duration, and Player Count
- Optional long matches 2 to 10+ hours
- Up to 10 players with AI kingdoms and minor tribes
- Four-stage match progression

### 5. World Generation and Regional Effects
- Random world generation
- Regional culture shaping economy and military identity

### 6. Naval Warfare
- Naval warfare exists
- Coastal trade and fishing orientation
- Sea-faring rogue/guerrilla possibilities

### 7. Minor Tribes and AI Kingdoms
- AI kingdoms as active participants
- Minor tribes as world elements
- Lesser houses as optional AI-autonomous or player-controlled

### 8. Resources and Economy
- Four primary resources: gold, food, water, stone
- Secondary resource: influence
- Currency dominance victory concept

### 9. Water Access and Non-Trivial Denial Rules
- Water as essential but not trivial hard-counter
- Fish-diet vs plains-hunting tradeoffs

### 10. Population Model
- Single pooled realm-wide value
- Growth dependent on food and water
- Converts to workers, soldiers, specialists, religious participants

### 11. Housing as Power-Replacement Scaling
- Housing replaces C&C power plant mechanics
- Population capacity expansion
- Societal stability effects

### 12. Territory Control and Loyalty
- Military control AND population loyalty required
- Revolts possible without loyalty
- Legitimacy and cultural persuasion layers

### 13. Legitimacy and Cultural Perception
- Territorial governance attraction
- Voluntary integration late game
- Governance structures, faith influence, economic prosperity

### 14. Bloodline System
- Literal family and dynasty tree
- Up to 20 active members, beyond dormant
- Training paths and dynastic roles

### 15. Bloodline Mortality, Protection, and Replacement
- Capture, enslavement, killing, ransom, marriage outcomes
- Hybrid heirs and new dynastic branches

### 16. Bloodline Active Cap and Dormancy
- 20 active members maximum
- Dormancy beyond cap

### 17. Family Tree UI and Dynastic Visibility
- Family tree as core gameplay element
- Player must always know head, heirs, specializations, heritage, reputation

### 18. Starting Leader Options
- Father, first oldest son, second oldest son, brother of king, firstborn son of king

### 19. Lesser Houses
- Created through titles and land grants
- Optional AI-autonomous or player-controlled

### 20. War Heroes, Titles, and Land Grants
- Exceptional commanders earn titles and lands
- Creates lesser houses

### 21. Recruitment Systems
- Adjustable sliders not hard-locked doctrines
- Mix of family obligations, paid soldiers, faith volunteers

### 22. Sons, Daughters, Battlefield Roles, Healing, Sustainment, and Morale
- Including daughters: negative attack buffer, positive healing/sustainment buffer
- Recruitment decisions as strategic tradeoffs

### 23. Faith System
- Four covenants chosen at end of Level 1
- Faith as major strategic axis

### 24. The Four Covenants
- The Old Light (Covenant of the First Flame)
- The Blood Dominion (The Red Covenant)
- The Order (Covenant of the Sacred Mandate)
- The Wild (Covenant of the Thorned Elder Roots)

### 25. Faith Intensity, Reinforcement, and Resource Costs
- Faith intensity must be maintained
- Costs resources
- Scales with population participation

### 26. Spiritual Manifestations and World Pressure from Extremes
- Angels, demons, cursed armies, healing, resurrection, plague removal, nature forces
- World-level pressure from extreme dark behavior

### 27. Conviction System
- Behavioral morality spectrum shaped permanently by actions
- Separate from faith
- Cultural identity and governance philosophy

### 28. Specialization, Level Structure, and Lockouts
- Five total levels of advancement
- Level 4 as point of no return
- Specialization lockouts

### 29. Level 4 Divergence and Level 5 Apex Structures
- Grand faith structures unlocked at Level 4
- Late-game apex power at Level 5

### 30. Neutral Trade Hub and Trueborn City
- Central trade hub
- Trueborn city as world stabilizer

### 31. Trueborn Coalition Response System
- Anti-rush system
- Contribution history to Trueborn city matters
- Coalition against emerging hegemons

### 32. Victory Conditions
- Military conquest (with instability risk)
- Economic/currency dominance
- Faith divine right
- Territorial governance attraction
- Dynastic prestige (with prestige dispute wars)

### 33. Currency Dominance
- Dynasty introduces own currency
- World economy transitions to dynasty currency standard

### 34. Faith Victory and Divine Right
- Faith-based victory path

### 35. Territorial Governance Attraction Victory
- Smaller territories join willingly
- Prosperity and infrastructure attracting voluntary integration

### 36. Dynastic Prestige and Prestige Dispute Wars
- Prestige as victory path
- Bloodline members on battlefield in prestige disputes

### 37. Military Conquest and Instability Risk
- Mutiny and defection risk
- Population morale consequences of reckless warfare

### 38. Capture, Enslavement, Ransom, and Noble Politics
- Multiple handling options for captured bloodline members
- Political dynamics of repeated capture/marriage

### 39. Marriage Diplomacy, Hybrid Heirs, and New Dynastic Branches
- Cross-dynasty marriages
- Children may declare loyalty to either bloodline or form new dynasty

### 40. Rogue Operations, Covert Action, and Assassination
- Assassination missions
- Covert warfare mechanics

### 41. Born of Sacrifice Elite Army Forging
- Sacrifice of multiple armies to create elite force
- Player naming or random generator
- Siege capability, enhanced armor, extreme morale

### 42. Founding Houses
- Eight founding houses (canonical list with historical conflict noted)

### 43. House Visual Identity and Hair Colors
- Locked lineage hair colors
- Visual distinction between houses

### 44. House Naming History and Canonical Name Rules
- Suffix duplication concern ("-borne", "-grave")
- Design goal: no repeated suffix patterns
- Two or three syllable maximum
- Distinct first letter shapes for UI readability

### 45. New Player Guide and Manual Material
- Full player-facing guide content preserved

### 46. Practical New Player Handbook Material
- How each system works in practice

### 47. Scenario Interpretations
- How match stages play out

### 48. Coding and Implementation Prompts
- Deterministic RTS architecture bootstrap prompt
- Simulation/presentation separation
- Fixed-tick simulation loop, seeded RNG, command buffer

### 49. Memory System Rules
- Full preservation rules
- Append-only protocol
- No summarization, reduction, or removal

### 50. Later Additions, Clarifications, and Conflict Notes
- Session-by-session additions labeled by type

---

## Building Infrastructure (from Session 2)

Located in Master Memory under Army Structure and Building Infrastructure sections:

### Civic Buildings
- Settlement Hall, Housing District, Well, Granary

### Economic Buildings
- Farm, Ranch, Market, Quarry

### Military Buildings
- Barracks, Training Grounds, Armory, Fortified Keep, Watchtower

### Faith Buildings
- Shrine, Temple, Grand Sanctuary

### Special Buildings
- Dynasty Estate, Academy, Treasury

---

## Level 1 Military Units (from Session 2)

Located in Master Memory under Army Structure — Level 1 Units:

- Militia
- Swordsmen (also serve as reconnaissance; scouts intentionally removed)
- Spearmen
- Hunters
- Bowmen *(prior name — canonical: Archers as of Seventeenth Session Canon 2026-04-25)* (offensive and defensive specialization)

---

## Append-Only Log Entries

The following ingestion records exist in `01_CANON/BLOODLINES_APPEND_ONLY_LOG.md`:

| Entry | Date | Source | Classification |
|-------|------|--------|---------------|
| Ingestion 001 | 2026-03-15 | Session 1: Foundational game guide, canonical verification, prompt generation | canonical design extraction, player manual content, prompt archive |
| Ingestion 002 | 2026-03-15 | Session 2: Core gameplay systems, resources, population, territory, buildings, military units, faith alignment | system clarification, implementation guidance, lore extraction |
| Ingestion 003 | 2026-03-15 | Session 3: House naming conventions, suffix duplication concern | naming rule, faction identity |
| Ingestion 004 | 2026-03-15 | Session 4: Bloodline family structure, conviction, recruitment, dynasty interactions, victory conditions | canonical design extraction, system clarification, lore extraction |

---

## Cross-Reference: Design Bible Sections

| Design Bible Section | Master Memory Sections | System Files |
|---------------------|----------------------|-------------|
| 1. World and Lore Foundations | 1, 2, 5, 6 | — |
| 2. Factions and Great Houses | 42, 43, 44 | — |
| 3. The Four Ancient Faiths | 23, 24 | `04_SYSTEMS/FAITH_SYSTEM.md` |
| 4. Conviction System | 27 | `04_SYSTEMS/CONVICTION_SYSTEM.md` |
| 5. Population System | 10 | `04_SYSTEMS/POPULATION_SYSTEM.md` |
| 6. Resource Economy | 8 | `04_SYSTEMS/RESOURCE_SYSTEM.md` |
| 7. Territory Control | 12, 13 | `04_SYSTEMS/TERRITORY_SYSTEM.md` |
| 8. Bloodline Members | 14, 15, 16, 17 | `04_SYSTEMS/DYNASTIC_SYSTEM.md` |
| 9. Dynastic Systems | 18, 19, 20, 38, 39, 40 | `04_SYSTEMS/DYNASTIC_SYSTEM.md` |
| 10. Army Structure | 21, 22, Level 1 Units | — |
| 11. Elite Units and Born of Sacrifice | 41 | `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md` |
| 12. Level Progression and Divergence | 28, 29 | — |
| 13. Match Structure and Scale | 4 | — |
| 14. Multiplayer and Long Match Design | 4 | — |
| 15. Future Expansion Concepts | Future Design Ideas | — |

---

## Cross-Reference: Prompt Archive

| Prompt | Location |
|--------|----------|
| Project Startup Prompt | `03_PROMPTS/PROJECT_STARTUP_PROMPT.md` |
| Session Memory Ingestion Prompt | `03_PROMPTS/SESSION_MEMORY_INGESTION_PROMPT.md` |
| Master Prompts Index | `03_PROMPTS/MASTER_PROMPTS.md` |
| Canonical Reconstruction Prompt (Block 1) | Master Memory, Session Prompt Archive |
| Session Memory Extraction Prompt (Block 2) | Master Memory, Session Prompt Archive |
| Canonical Design Memory Prompt (Block 3) | Master Memory, Session Prompt Archive |
| Canonical Design Bible Prompt (Block 4) | Master Memory, Session Prompt Archive |
| Deterministic RTS Bootstrap Prompt | Master Memory, Coding and Implementation Prompts |

---

*This index must be expanded whenever new content is added to the memory system. It must never be shortened. New categories, sections, and cross-references are added as they appear.*
