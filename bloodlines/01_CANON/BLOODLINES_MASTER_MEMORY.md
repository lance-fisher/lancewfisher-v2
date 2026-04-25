# Bloodlines — Master Memory Ledger

This is the cumulative, append-only memory of the Bloodlines project. Every design decision, brainstorm, exploration, and session summary is recorded here in chronological order. This document grows continuously and is never shortened, summarized, or truncated.

**Rules governing this document:**
- Content is appended, never replaced
- Nothing is deleted without explicit authorization
- Contradictions are preserved alongside their context
- Date stamps mark when each entry was added
- This document is loaded at the start of every design session

---

## Memory Entries

### 2026-03-15 — Project Initialization

**Event:** Bloodlines project directory structure and canonical files initialized.

**What was established:**
- 20-folder numbered directory structure covering admin, canon, systems, lore, factions, faiths, mechanics, world, units, matchflow, UI/UX, audio/visual, assets, prototype, research, tasks, exports, and archive
- Additive-only archival rules codified in WORKFLOW_RULES.md
- Design bible scaffolded with 15 major sections
- 7 core game systems identified and given individual files: Conviction, Faith, Population, Resource, Territory, Dynastic, Born of Sacrifice
- Session ingestion pipeline documented
- Project startup prompt created for future session continuity

**Design state at initialization:**
- All systems are at scaffold level (headings and structural placeholders)
- No mechanical details have been committed to canon yet
- The design bible sections represent the intended scope but contain no settled content
- The project is ready to receive its first substantive design session

---

*New entries are appended below this line. Do not modify entries above.*

---

### 2026-03-15 — First Substantive Design Ingestion (Sessions 1-4 Consolidated)

**Event:** Four design sessions consolidated and ingested into the persistent memory system. This is the first substantive content ingestion following Phase 0 scaffolding. All design content below is canonical and preserved in full.

**Sessions ingested:**
- Session 1: Foundational game guide creation, canonical verification, prompt generation
- Session 2: Core gameplay systems, resources, population, territory, buildings, military units, faith alignment
- Session 3: House naming conventions, suffix duplication concern
- Session 4: Bloodline family structure, conviction philosophy, recruitment, dynasty interactions, victory conditions

---

## 1. Project Identity

**Game Title:** Bloodlines (LOCKED — title may not be changed without explicit authorization)
**Genre:** Massively large-scale medieval real-time strategy game
**Design Inspiration:** Command & Conquer style RTS, with strong emphasis on base building, offense and defense, resource gathering, strategic freedom, and broad viability across multiple playstyles
**Design Philosophy:** The project direction explicitly values the artistic and structural lesson of Command & Conquer Generals in that the game does not need to be "fancier" than that in visual ambition if balance, playability, faction balance, and experimentation value are strong. The project is built around the belief that what makes a strategy game endure is not visual excess alone, but deep and balanced systems that allow players to repeatedly return and succeed through different approaches.
**Not:** Not Age of Empires building style. Not Total War combat style.
**Title History:** The project evolved from an early title direction of Crown and Conquest into the locked project title Bloodlines.
**Creative Intent:** The user explicitly stated that the game will likely be an award winner if seen through because it has all the elements of strategy games that keep people hooked.

---

## 2. Core Design Philosophy

The game is intended to feel like a medieval kingdom RTS while still maintaining the core strategic readability and base-building clarity associated with classic real-time strategy design. It is intended to support aggressive openings, defensive expansion, economic scaling, ideological consolidation, and other varied play patterns without collapsing into a single dominant strategy. The game is intended to stand the test of time and be balanced and enjoyable in a way that keeps players hooked and experimenting.

The game now centers not just on kingdoms and armies, but on hereditary power, dynastic identity, ideological divergence, moral trajectory, family continuity, and the social character of a civilization.

The foundational world logic includes noble lineages, ancient faiths, population loyalty, and the possible re-emergence of ancient blood-based legitimacy in the form of the Trueborn return. The world is not just a battlefield. It is a living strategic society in which war, belief, inheritance, governance, and territory all interact.

The design philosophy prioritizes clarity, balance, replayability, and long-term strategic depth.

**Core Design Pillars:**
The design of Bloodlines rests on four foundational pillars:
1. Territory
2. Population
3. Faith
4. Dynasty

These four pillars interact to shape the long-term evolution of a civilization.

---

## 3. Canonical Vocabulary and Definitions

- **Bloodline** = the family (the ruling dynasty)
- **Conviction** = the philosophy (governing philosophy shaped by actions)
- **Faith** = the religion (covenant alignment)

These three terms are distinct and must not be confused or merged in future sessions. Bloodline represents the family. Conviction represents the philosophy of rule. Faith represents the religion.

---

## 4. Match Scale, Duration, and Player Count

- Optional long matches 2 to 10+ hours
- Up to 10 players with AI kingdoms and minor tribes
- Large matches may extend well beyond ten hours in epic campaigns
- The game is structurally intended for large, slow-burn, civilization-shaping play, not just skirmish bursts
- Different strategic playstyles should remain viable
- The world is intended to function as a living political landscape

**Four-Stage Match Progression:**

**Stage 1 — Founding**
The player establishes their initial settlement, gathers resources, and begins population growth. At the end of this stage the player selects a Faith. The dynasty encounters tribes, ruins, and sacred sites.

**Stage 2 — Consolidation**
The kingdom expands infrastructure and military capability while stabilizing territory.

**Stage 3 — Ideological Expansion**
Faith systems begin influencing society and shaping political identity. Ideology increasingly influences society, conflict, and civilization identity.

**Stage 4 — Irreversible Divergence**
The civilization reaches its ideological apex and unlocks powerful late-game structures. This is the major irreversible divergence point.

**Historical Note:** Two framings exist in the archive. One uses "Level" terminology. The later one uses "Stage" terminology. Both are preserved as historical design expressions of the same structural progression.

---

## 5. World Generation and Regional Effects

- Random world generation
- Regional culture shaping economy and military identity
- The world contains ancient powers, long-standing bloodlines, and political factions whose identities shape the course of warfare and diplomacy
- The game's narrative world includes a history of ancient civilizations, powerful faiths that shaped the world's early structure, and the later emergence of dominant dynastic houses that now control territories across the world
- The world supports conflict not only between factions but within ideologies, within families, and within the moral direction of each civilization

---

## 6. Naval Warfare

- Naval warfare exists
- Coastal trade and fishing orientation
- Fish-diet vs plains-hunting tradeoff including lower strength or HP possibility
- Sea-faring rogue / guerrilla possibilities and weaker siege or open-field performance for coastal dynasties

---

## 7. Minor Tribes and AI Kingdoms

- AI kingdoms as active kingdom-level participants, not mere placeholders
- They occupy the world alongside players and minor entities and participate in a living political landscape
- AI kingdoms operate using the same core systems as player kingdoms
- AI factions control territory, develop infrastructure, and raise armies
- AI behavior may vary depending on faction personality and strategic tendencies
- AI dynasties can form alliances, join Trueborn coalitions, and react to rising hegemonic dynasties
- Minor tribes as world elements
- Lesser houses as optional AI-autonomous or player-controlled
- Not every match will necessarily include every house as a playable dynasty
- Unused houses may appear as AI kingdoms or minor factions
- Specific AI behavior models, doctrine systems, diplomacy logic, faith prioritization logic, or economy styles have not yet been fully defined

---

## 8. Resources and Economy

**Five Primary Resources:**

**Gold**
Gold represents currency, taxation, and economic power. Used for building construction, troop recruitment, diplomacy, trade, and ransom. Gold is generated through taxation, markets, and trade networks.

**Food**
Food supports population growth and sustains armies. Food shortages cause famine and declining loyalty. Produced through farms and ranches.

**Water**
Water represents fresh water supply and irrigation. Water is essential for population survival and agricultural production. Water is elevated to one of the game's primary resources rather than treated as a secondary terrain effect. This is a major point of distinction.

**Wood**
Wood is used for construction, fuel, and equipment crafting. Used for basic building construction, siege equipment, ships, and fuel for smelting and heating. Produced through lumber camps in forested territories. Wood is a renewable but location-dependent resource tied to forested regions.

**Stone**
Stone is used for construction and fortification. Used to build walls, towers, and major structures. Stone deposits are finite strategic resources.

**Secondary Resource:** Influence

The economy is intentionally grounded and legible. These five resources reflect a desire for strong core simplicity with deep interaction rather than bloated complexity for its own sake.

Resource control is tied directly to territorial control. Resource nodes across the map create natural conflict points between kingdoms.

**Currency Dominance (Late-Game Economic Victory Concept):**
A dynasty pursuing economic dominance can introduce its own currency into the global economy. Through successful trade networks and economic influence, the world can transition away from gold and onto the dynasty's currency standard. Economic victory occurs when the world economy becomes dependent on that dynasty's currency.

---

## 9. Water Access and Non-Trivial Denial Rules

- Water as essential but not a trivial hard-counter win condition
- Fish-diet vs plains-hunting tradeoffs including lower strength or HP possibility for fish-diet civilizations
- Denying water is impactful but should not trivially end a game

---

## 10. Population Model

Population exists as a unified pool representing the kingdom's civilians. Population is a single pooled realm-wide value.

Population growth depends primarily on food and water availability.

Population is not just a cap. It is a living strategic resource. It supplies the labor, military, civic, and ideological body of the civilization.

Population can be converted into workers, soldiers, specialists, or religious participants.

Population determines:
- Workforce availability
- Tax revenue
- Recruitment capacity
- Territorial loyalty

Population decline can occur due to:
- Famine
- Warfare
- Plagues
- Religious conflict

Population loyalty affects productivity. Low loyalty reduces tax income and increases the risk of sabotage and rebellion.

Population morale and loyalty are influenced by how the dynasty governs and how it treats soldiers and citizens. Reckless warfare that burns through soldiers without support lowers morale and can cause mutinies or defections. Population loyalty can influence recruitment effectiveness and stability within the dynasty's territories.

Population is also central to loyalty and territory consolidation because a territory is not truly controlled by military occupation alone. Population acceptance matters. Therefore population is not just a labor pool. It is also a political body.

---

## 11. Housing as Power-Replacement Scaling

Housing determines the maximum population capacity. Housing replaces the power grid mechanics common in many RTS games (replacing Command & Conquer style power plant concepts). Instead of generating electrical grid power, the player expands social and logistical capacity through housing, which in turn raises population capacity and supports social stability.

Adequate housing also contributes to societal stability.

---

## 12. Territory Control and Loyalty

Territory control in Bloodlines requires both territorial control and loyalty. The project memory explicitly says: territory control and loyalty both required.

**Two-Part System:**
1. Military control
2. Population loyalty

A region held only by force is unstable. It may revolt. A region becomes truly controlled only when the population accepts the player's rule.

The map is divided into territories. Each territory contains:
- Resources
- Population
- Infrastructure
- Loyalty level

Controlling a territory grants access to its resources. However loyalty must be maintained.

If loyalty declines:
- Tax revenue decreases
- Recruitment becomes difficult
- Sabotage risk increases
- Rebellions may occur

This is a defining design departure from many RTS games where a structure or outpost marker is enough to claim land.

Territory therefore involves questions of occupation, legitimacy, stability, loyalty, and governance. It also implies that expansion without consolidation can create internal weakness.

Control of territory depends on:
- Military presence
- Local loyalty
- Infrastructure
- Faith influence

Methods of loyalty maintenance include governance structures, faith influence, economic prosperity, and dynasty reputation.

---

## 13. Legitimacy and Cultural Perception

- Territorial governance attraction (late-game concept)
- Voluntary integration: In late game territorial governance paths, smaller territories may choose to join the dynasty willingly due to the prosperity and infrastructure of the dynasty
- Governance structures, faith influence, economic prosperity as tools

---

## 14. Bloodline System

Bloodline members are a major gameplay system. There are 20 active bloodline members, with additional members beyond 20 becoming dormant. This must remain exactly preserved because it was explicitly saved into project memory in that form: "bloodline members 20 active (at 20, beyond dormant), with training paths and dynastic roles, capture/enslavement/marriage diplomacy."

Bloodline members are not ordinary military units. They are family members within the ruling dynasty. They can occupy strategic roles, may have training paths, and influence the house and wider kingdom.

Bloodline members are born into the dynasty over time. When children are born into the bloodline their paths are chosen immediately. They are trained from childhood for their role rather than choosing a path upon reaching adulthood.

Training paths include:
- Military command
- Governance
- Religious leadership
- Diplomacy
- Covert operations
- Economic stewardship

Bloodline members become active leaders upon maturity.

Possible roles include military commanders, governors, diplomats, and ideological leaders.

Members trained in the art of war emerge as battlefield commanders. If they win exceptional wars and battles they may become celebrated war hero commanders.

---

## 15. Bloodline Mortality, Protection, and Replacement

- Capture, enslavement, killing, ransom, marriage outcomes for bloodline members
- Hybrid heirs and new dynastic branches possible through cross-dynasty marriages
- Assassination and covert warfare mechanics exist
- Bloodline members can be threatened, exploited, or strengthened in ways beyond direct combat

---

## 16. Bloodline Active Cap and Dormancy

- 20 active members maximum
- Beyond 20, members become dormant until activated
- Dormancy is a mechanical state, not permanent removal

---

## 17. Family Tree UI and Dynastic Visibility

The family tree is not a decorative feature. It is a critical gameplay-accessible system.

The player should always understand:
- Who is head of the family
- Who the heirs are
- Who is in the family
- What they specialize in
- What their heritage is made of
- How they are loved or hated by the population

The family tree interface should be a central gameplay element where the player can see the entire bloodline structure at all times.

---

## 18. Starting Leader Options

Starting leader options include:
- Father
- First oldest son
- Second oldest son
- Brother of the king
- Firstborn son of the king

---

## 19. Lesser Houses

Lesser houses are optional AI-autonomous or player-controlled. This is a distinctive system because it means lesser houses are not fixed to one implementation mode.

Exceptional bloodline members can found lesser houses by receiving titles and lands. When a member becomes a celebrated war hero or great leader, the dynasty may grant them lands and titles, creating a lesser house under the main dynasty.

Lesser houses remain loyal to the main bloodline but become their own branches within the dynastic family tree.

They may act independently as AI entities or be directly controlled by a player in some capacity. This implies subordination, vassal-like relationships, or secondary command structures may eventually matter.

---

## 20. War Heroes, Titles, and Land Grants

Exceptional leaders may become war heroes. War heroes may receive titles and land. This can create lesser houses.

These commanders may be granted titles and lands, creating lesser houses that strengthen the bloodline. Exceptional commanders and war heroes can elevate the prestige of the dynasty and establish lesser houses.

---

## 21. Recruitment Systems

Army recruitment is not chosen as a single doctrine but controlled through adjustable sliders. Dynasties can mix recruitment approaches including:
- Family obligations
- Paid soldiers
- Faith volunteers

Recruitment decisions include whether sons, daughters, or both serve in the army.

---

## 22. Sons, Daughters, Battlefield Roles, Healing, Sustainment, and Morale

Including daughters in the army may produce a negative attack buffer but a positive healing and sustainment buffer. This creates a meaningful strategic tradeoff in recruitment decisions.

---

## 23. Faith System

The faith system is one of the central pillars of Bloodlines and is not a decorative or secondary layer. It is a major strategic axis.

Faith represents the religious covenant that a dynasty aligns with. Faith is selected after the early exploration phase (at the end of Level 1 / Stage 1) when dynasties encounter the wider world, tribes, ruins, and sacred sites.

The dynasty does not create a faith. It aligns with an existing covenant. The Four Covenants already exist in the world.

Faith is governed by three interacting components:
1. **Covenant** — the religion the dynasty follows
2. **Faith Intensity** — the degree to which the population actively practices the faith
3. **Faith Alignment** — a sliding scale representing how the covenant is practiced along a light vs dark spectrum

---

## 24. The Four Covenants

**The Old Light (Covenant of the First Flame)**
A faith centered on protection, mercy, divine guardianship, and the defense of innocents. Also described as centered around enlightenment, unity, order, societal cohesion, disciplined governance, and defensive stability.

**The Blood Dominion (The Red Covenant)**
A faith centered on conquest, sacrifice, domination, and the belief that strength demonstrates divine favor. Darker in tone, carries moral cost and conviction consequences. Provides strong military benefits.

**The Order (Covenant of the Sacred Mandate)**
A faith built around law, doctrine, institutional authority, and structured governance under divine mandate. Emphasizes authority, rigid doctrine, hierarchy, and structured governance or administrative control.

**The Wild (Covenant of the Thorned Elder Roots)**
A primal faith rooted in nature mysticism, ancient spirits, and the balance of the natural world. Emphasizes adaptive survival, environmental strength, and ancient rooted power.

These names must be preserved exactly as historical project language because both paired naming forms have appeared in the project memory and later prompt outputs.

---

## 25. Faith Intensity, Reinforcement, and Resource Costs

The canonical project memory explicitly states: "Faith has intensity that must be maintained and costs resources and scales with population participation."

Faith is not a passive permanent buff. It is active, costly, population-scaled, and maintenance-dependent.

- Faith has intensity
- Faith intensity is not permanent without upkeep
- Maintaining strong faith costs resources
- Faith scales with population participation
- Higher faith intensity unlocks stronger abilities

Faith intensity grows through participation, rituals, structures, and engagement.

Faith actions that influence intensity and alignment include:
- Rituals
- Offerings
- Temple construction
- Priesthood expansion
- Religious festivals
- Sacrificial rites

Faith actions consume resources and may also consume captured units or population.

As faith grows stronger it can radicalize and significantly affect the dynasty's legitimacy and military recruitment.

---

## 26. Spiritual Manifestations and World Pressure from Extremes

- Spiritual manifestations such as angels, demons, cursed armies, healing, resurrection, plague removal, nature forces
- World-level pressure from extreme dark behavior
- Faith manifestations appearing at high intensity (future design idea)

---

## 27. Conviction System

Conviction is a behavioral morality spectrum shaped permanently by actions. This means conviction is not a cosmetic alignment label chosen in a menu. It is produced by what the player actually does during the match.

Conviction represents the moral trajectory of the kingdom or civilization. Actions influencing it include:
- Warfare practices
- Treatment of prisoners
- Enslavement
- Protection of civilians
- Pillage and destruction
- Sacrifice
- Governance decisions
- Other behavioral choices

Conviction is permanent and reflects the historical character of the civilization.

Conviction is distinct from Faith. Faith is chosen. Conviction is lived. Faith determines doctrine and worldview. Conviction determines what kind of civilization the player has actually become through action.

Conviction represents the moral character of the dynasty's rule. Faith Alignment represents religious practice. These two systems influence each other but remain separate.

Conviction evolves based on the actions of the dynasty and can shift over time as the dynasty adapts to circumstances.

Examples of conviction directions include:
- Conquest
- Prosperity through trade
- Faith authority
- Territorial governance
- Dynastic prestige

Conviction permanently shapes:
- Player abilities
- Faith alignment interactions
- Kingdom development
- Late game powers

---

## 28. Specialization, Level Structure, and Lockouts

Bloodlines uses a five-level progression system. Each level represents advancement in civilization and military development.

- Five total levels of advancement
- Level 1 represents the founding stage
- Level 4 represents a major divergence where dynasties commit to strategic directions such as faith dominance or military supremacy
- Level 4 is the point of no return
- Level 5 represents apex capabilities (grand endgame apex structure tier)
- Specialization lockouts exist at various levels

---

## 29. Level 4 Divergence and Level 5 Apex Structures

Level 4 is the major irreversible divergence unlocking grand faith structures and late-game apex.

Grand faith structures are powerful constructions unlocked in late game stages. They represent the culmination of faith alignment. These structures unlock unique abilities that define the late game strategy of each kingdom.

The specific structures for each faith have not yet been fully designed. However, their existence is already canon.

They are tied to late-game ideological culmination. They likely represent each civilization's final spiritual, political, or metaphysical institutional expression.

---

## 30. Neutral Trade Hub and Trueborn City

At the center of the world exists a neutral zone city associated with the Trueborn banner. This city serves as a stabilizing power within the world. The Trueborn house is the oldest lineage and plays a special role within the world through the neutral Trueborn city.

Throughout the game dynasties may contribute to the city through trade, diplomacy, construction, or protection. These contributions affect how the city perceives each dynasty.

---

## 31. Trueborn Coalition Response System

If the Trueborn city remains unconquered by the late game, it can choose to intervene when one dynasty begins approaching victory conditions too quickly. The city can raise the Trueborn banner and recruit other dynasties into a coalition against the emerging hegemon.

This system is designed to prevent early rush victories or dynasties "cheesing" their way to a fast win.

Contribution history to Trueborn city matters. How the city perceives each dynasty affects whether it will join a coalition against or alongside them.

---

## 32. Victory Conditions

Multiple victory paths exist:

**Military Conquest** — Conquest with instability risk. Reckless warfare that burns through soldiers without support lowers morale and can cause mutinies or defections. Military conquest is not simply "destroy everything."

**Economic / Currency Dominance** — Dynasty introduces own currency. World economy transitions to dynasty currency standard. Economic victory occurs when the world economy becomes dependent on that dynasty's currency.

**Faith Victory / Divine Right** — Faith-based victory path through religious influence and dominance.

**Territorial Governance Attraction** — Smaller territories choose to join the dynasty willingly due to prosperity and infrastructure. Voluntary integration rather than conquest.

**Dynastic Prestige** — Prestige as victory path with bloodline members on battlefield in prestige dispute wars.

Population loyalty was noted as not favored as a standalone victory path.

---

## 33. Currency Dominance

A dynasty pursuing economic dominance can introduce its own currency into the global economy. Through successful trade networks and economic influence, the world can transition away from gold and onto the dynasty's currency standard. Economic victory occurs when the world economy becomes dependent on that dynasty's currency.

---

## 34. Faith Victory and Divine Right

Faith-based victory path. Not defined in full mechanical detail in canonical design memory at this time.

---

## 35. Territorial Governance Attraction Victory

In late game territorial governance paths, smaller territories may choose to join the dynasty willingly due to the prosperity and infrastructure of the dynasty. This represents victory through attraction rather than conquest.

---

## 36. Dynastic Prestige and Prestige Dispute Wars

Prestige as a victory path. Dynastic prestige dispute wars involve bloodline members on the battlefield. The specific mechanical rules for prestige disputes are not fully defined in canonical design memory at this time.

---

## 37. Military Conquest and Instability Risk

Military conquest carries instability risk. Reckless warfare that burns through soldiers without support lowers morale and can cause mutinies or defections. Population morale consequences of reckless warfare are a balancing mechanic against pure military aggression.

---

## 38. Capture, Enslavement, Ransom, and Noble Politics

Dynasties can capture members of rival bloodlines. Captured bloodline members can be handled in multiple ways:
- Enslavement as workers
- Execution
- Ransom
- Marriage into the dynasty (if the bloodline practices polygamy)

The way a dynasty handles captured bloodline members over time can influence political dynamics.

---

## 39. Marriage Diplomacy, Hybrid Heirs, and New Dynastic Branches

If a dynasty repeatedly takes wives from rival bloodlines and fathers children, those children may form new dynastic branches. They may later declare loyalty to one bloodline or the other or form a new dynasty entirely.

Children from mixed bloodline marriages can create new dynastic branches. This creates emergent political complexity.

---

## 40. Rogue Operations, Covert Action, and Assassination

Assassination missions and covert warfare mechanics exist as part of the dynastic gameplay. Covert operations are a training path for bloodline members. Specific mechanical details not fully defined in canonical design memory at this time.

---

## 41. Born of Sacrifice Elite Army Forging

The canonical project memory explicitly records an "elite army-forging mechanic called Born of Sacrifice with player naming or random generator."

Born of Sacrifice is a late-game mechanic. Players may sacrifice multiple armies to create an elite army.

Example: Five standard armies may be sacrificed to produce a powerful elite force.

These elite armies may gain:
- Siege capability
- Enhanced armor
- Extreme morale

Born of Sacrifice creates identity-rich, memorable battlefield units tied to the ideological and dynastic character of the civilization. Units can be named by the player or assigned names through a random generator.

Later outputs described them as rare, extremely powerful, and often becoming legendary figures within a match.

---

## 42. Founding Houses

The core game includes 9 founding Houses with a late-game Trueborn re-emergence lore component.

**Canonical List (SETTLED 2026-03-15):**
Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest

This list was confirmed as canonical on 2026-03-15. All nine suffixes are unique.

**Historical Variants (Archived, superseded):**

Session 3 variant: Trueborn, Highborne, Ironmark, Hartborne, Whitehall, Westgrave, Goldgrave, Oldcrest

Session 3 speculative revision: Trueborn, Highmark, Ironmark, Hartvale, Whitehall, Westreach, Goldcrest, Oldcrest

These variants are preserved as historical record per append-only rules but are no longer the active list.

---

## 43. House Visual Identity and Hair Colors

- Locked lineage hair colors for each founding house
- All characters share a common ancestry and physical heritage
- The visual difference between houses is expressed through distinct hair colors and styles
- This creates recognizable identity without dividing the world into separate ethnic groups
- Dynastic cultural ancestry reflected through hair color differences among bloodlines

---

## 44. House Naming History and Canonical Name Rules

The project creator expressed dissatisfaction with repeated suffix structures in house naming conventions. Specifically:
- "-borne" appeared twice (Highborne, Hartborne)
- "-grave" appeared twice (Westgrave, Goldgrave)

**Design Principles for House Naming (established Session 3):**
1. Two syllables or three syllables maximum
2. Distinct first letter shapes for UI readability
3. No repeated suffix patterns

This principle was justified by referencing classic RTS design patterns (GDI, NOD, China, USA, GLA) in which factions have clear visual and linguistic distinction for quick recognition during gameplay.

The earlier corrected list (without duplicated suffixes) was discussed in a prior session but was never saved into the project memory. The corrected version is therefore not present in the canonical archive at this time and requires explicit resolution.

---

## 45. New Player Guide and Manual Material

A player-facing foundational guide was created describing Bloodlines as a large-scale medieval RTS focused on dynasty, belief systems, territorial expansion, and long-form strategic evolution. The guide described:
- The four foundations as territory, population, faith, and dynasty
- The player's role as not being a single character but a guiding authority of a ruling dynasty
- Bloodline members as strategic assets rather than ordinary units
- Population as a living resource rather than merely a cap
- Housing as replacing the traditional power-plant logic
- Territory requiring loyalty as well as military control
- The five resources as gold, food, water, wood, and stone
- The four faiths with paired naming and thematic identities
- Faith intensity as a scaling system
- Faith morality as a light versus dark orientation
- Conviction as behavioral morality
- Born of Sacrifice as a unique elite unit system
- Lesser houses as AI or player-controlled
- Multiplayer scale as up to ten players
- The Trueborn return as a late-game event
- Multiple strategic paths and victory conditions

This material is preserved as reference-grade project content and must not be treated as disposable commentary.

---

## 46. Practical New Player Handbook Material

Preserved within the guide:
- How Bloodlines, Faith, and Conviction are defined and how they differ
- How the early game works
- How Level 1 exposure shapes faith choice
- How each covenant should be interpreted in practice
- How specialization and divergence work
- How the Trueborn system works
- How each victory condition functions
- How recruitment, morale, sustainment, loyalty, and population behave
- What new players commonly misunderstand
- How long matches evolve
- Why Bloodlines is built around memory and dynastic consequence

---

## 47. Scenario Interpretations

Match stages play out as a civilizational arc:
- Early game focuses on expansion and survival
- Mid game introduces dynastic politics and religious development
- Late game introduces ideological wars and victory escalation
- Matches can evolve over long periods as dynasties grow across generations
- Late game events may include global coalitions against dominant dynasties

---

## 48. Coding and Implementation Prompts

**Deterministic RTS Architecture Bootstrap Prompt (Phase 1 Foundation):**

A prompt was generated to bootstrap the Bloodlines simulation architecture with the following goals:
- Clean separation between simulation and presentation
- Deterministic fixed-tick simulation loop (target 20 TPS)
- Seeded RNG module with deterministic consumption ordering
- Command buffer + command resolver pipeline for simulation mutations
- Simple event bus for simulation events
- Snapshot system for save/load and replay compatibility, schema versioned
- Per-tick deterministic hash function for desync detection
- Minimal "headless sim" harness for testing

Non-negotiables:
- No Math.random in sim
- No Date.now in sim
- No floats in sim state
- Stable iteration order everywhere
- Commands applied in stable deterministic order
- Seeded RNG consumption order must not depend on object key ordering

Example proof feature: TownCenter entity with Position, Stockpile (gold, food, water, stone), and Housing components. Economy system adding +1 food and +1 water every 5 ticks. SpendResource and AddResource commands. Run 200 ticks headless with identical hashes across two runs.

File structure: /src/sim with subfolders for core, ecs, commands, events, snapshot, and domains. Presentation layer reads snapshots only.

---

## 49. Memory System Rules

Full preservation rules governing this memory system:
1. Never remove anything
2. Never redact anything
3. Never reduce anything
4. Never summarize anything into a shorter replacement
5. Never minimize anything
6. Never compress earlier content to "save space"
7. Never replace detailed sections with concise versions
8. Never silently overwrite prior project material
9. Never delete historical context unless explicitly commanded
10. Even if direction changes, preserve prior material in archival form
11. The memory system must always become longer over time, never shorter
12. New additions must be additive only
13. If similar content appears more than once, preserve both and organize them
14. If new material refines or conflicts with prior material, preserve both and label clearly
15. Organization is allowed. Reduction is not allowed.
16. Cross-references are allowed. Shortening is not allowed.
17. Headings and indexing are allowed. Loss of detail is not allowed.

---

## 50. Later Additions, Clarifications, and Conflict Notes

### Resolved: Founding House List Variants
Multiple versions existed in the archive (see Section 42). Variant 1 (Session 1) was confirmed as canonical on 2026-03-15 with 9 founding houses. Historical variants preserved in Section 42.

### Conflict Note: Level vs Stage Terminology
Both "Level" and "Stage" terminology have been used to describe match progression. Both are preserved. No explicit instruction was given to discard either.

### Resolved: House Count
The archive originally recorded "8 founding Houses" but the Session 1 list contained nine names. Resolved on 2026-03-15: the game has 9 founding houses.

### Design Clarification: Faith Three-Component Model
Session 2 established that faith is governed by three interacting components: Covenant, Faith Intensity, and Faith Alignment (light vs dark spectrum). This expanded the earlier two-component model (covenant + intensity).

### Design Clarification: Scouts Removed
Session 2 explicitly removed scouts as a unit type. Swordsmen serve as both frontline infantry and reconnaissance units.

### Design Clarification: Recruitment Sliders
Session 4 confirmed that recruitment is not a hard-locked doctrine but controlled through adjustable sliders. This allows mixing recruitment approaches.

### Design Clarification: Bloodline Specialization at Birth
Session 4 clarified that when children are born into the bloodline, their paths are chosen immediately. They are trained from childhood for their role rather than choosing upon reaching adulthood.

---

## Building Infrastructure (Session 2)

Buildings support the development of the dynasty, organized into five categories:

### Civic Buildings
- **Settlement Hall** — Establishes control of the territory and collects taxes
- **Housing District** — Expands population capacity
- **Well** — Provides water supply
- **Granary** — Stores food reserves

### Economic Buildings
- **Farm** — Produces food through agriculture
- **Ranch** — Produces livestock food
- **Market** — Generates gold through trade
- **Lumber Camp** — Produces wood from forested territories
- **Quarry** — Produces stone

### Military Buildings
- **Barracks** — Trains infantry
- **Training Grounds** — Improves soldier training
- **Armory** — Provides equipment upgrades
- **Fortified Keep** — Major defensive stronghold
- **Watchtower** — Provides early warning of enemy approach

### Faith Buildings
- **Shrine** — Small religious structure increasing faith intensity
- **Temple** — Major religious structure enabling powerful rituals
- **Grand Sanctuary** — Late-game structure tied to faith victory

### Special Buildings
- **Dynasty Estate** — Residence of bloodline members
- **Academy** — Trains bloodline leaders
- **Treasury** — Improves financial control

---

## Army Structure — Level 1 Units (Session 2)

Level 1 represents the earliest military stage. Armies are small and drawn from the population.

**Militia**
Basic soldiers drawn directly from the population. Very cheap, weak combat ability, low morale. Used for emergency defense and early expansion.

**Swordsmen**
The first organized professional soldiers. Moderate attack and defense. Faster movement speed. Increased line of sight. Serve as both frontline infantry and reconnaissance units. Scouts are intentionally removed as a unit type. Swordsmen replace the scouting role.

**Spearmen**
Defensive infantry with higher defense than swordsmen. Slower movement. Effective at holding the line. Strong against cavalry.

**Hunters**
Light ranged units drawn from the population. Used for harassment and ambush. Weak in melee combat.

**Bowmen** *(Prior name — canonical name is Archers as of Seventeenth Session Canon 2026-04-25)*
Upgraded ranged units trained from hunters. Bowmen can specialize in two roles:
- **Offensive Bowmen** — Higher attack damage
- **Defensive Bowmen** — Increased range and improved defensive accuracy
Bowmen remain weak in close combat.

**Early Army Composition:**
Typical Level 1 army may include militia providing numbers, swordsmen providing mobility and scouting, spearmen holding defensive formations, and hunters or Archers (formerly Bowmen) providing ranged support.

---

## Core Ground Unit Progression — Locked 2026-04-25 (Seventeenth Session Canon)

The canonical base unit ladder available to all dynasties. Off/Def values are absolute role-based ratings, independent of the faith-tier progression in Levels 1-5.

**Offensive Infantry:** Militia (2/2) → Swordsmen (4/2) → Shieldbearer Swordsmen (5/3) → Heavy Swordsmen (6/4) → Greatsword Knight (8/3)

**Defensive Infantry:** Militia (2/2) → Spearmen (2/4, anti-cav x1.5) → Spear Guard (3/5, anti-cav x1.6) → Pikeguard (4/6, anti-cav x1.8) → Bulwark Guard (3/8)

**Ranged:** Archers (3/2) → Boltmen (5/2, armor-piercing x1.5)

**Cavalry:** Mounted Swordsmen (5/2) → Mounted Heavy Swordsmen (6/3) → Mounted Knight (7/5, grouped lance charge, can dismount) ⇄ Knight (6/6, dismounted form, can remount). Horse Archer (3/2) is a cavalry ranged variant, not on the main upgrade line.

**Ironmark unique:** Axeman (5/2) — Off/Def revised 2026-04-25 from Off 6/Def 4. All other Axeman identity preserved.

**Hartvale unique:** Verdant Warden (4/5). Locked 2026-04-07.

**Iron and forge:** Iron available from game start. Forge/Settlement Forge required from the beginning. Prior "early game low significance" framing is superseded.

**Cavalry infrastructure:** All cavalry requires Stable.

**Mounted Knight lance charge:** Grouped formation ability — requires multiple Mounted Knight units in formation. Countered by braced Spearmen/Spear Guard/Pikeguard. Ineffective in forest/tight terrain. Full mounted mechanics pending.

**Scrapped:** War Wagon (deferred), Siege Tower as a unit (deferred), light transport vehicles (scrapped entirely).

See `10_UNITS/UNIT_INDEX.md` (Core Ground Unit Progression section), `01_CANON/CANONICAL_RULES.md` (Seventeenth Session Canon), and `data/units.json` for full specifications and implementation data.

---

## Confirmed Design Gaps (as of 2026-03-15)

The following areas are confirmed as not yet fully designed:
- Specific unit classes and combat system beyond Level 1
- Specific building tree and technology tree progression
- Specific map control mechanics in detailed implementation form
- Specific faith structure progression and named grand faith structures for each covenant
- Specific dynasty character traits system
- Specific AI kingdom behavior logic
- Specific campaign progression structure beyond long match and stage progression concepts
- Specific diplomacy rules and interface details
- Specific resource gathering implementation details
- Specific multiplayer lobby and ruleset detail
- Specific prestige dispute war rules
- Specific currency dominance mechanical implementation
- ~~Final resolved founding house list without suffix duplication~~ (RESOLVED 2026-03-15: Variant 1 confirmed, 9 houses, all unique suffixes)
- Specific faith manifestation mechanics

---

## Session Prompt Archive

Four major prompts were generated during the ingested sessions and are preserved in full within the Append-Only Log (`01_CANON/BLOODLINES_APPEND_ONLY_LOG.md`):

1. **Prompt Block 1 — Canonical Memory Reconstruction and Design Verification Prompt** — Forces collection, reconstruction, verification, and preservation of the entire Bloodlines design state
2. **Prompt Block 2 — Session Memory Extraction Prompt** — Instructs receiving AI to output the entire known design state in full preservation mode
3. **Prompt Block 3 — Canonical Design Memory Prompt** — Reconstructs and preserves the complete design state as a baseline for future sessions
4. **Prompt Block 4 — Canonical Design Bible Prompt** — Master system prompt establishing the permanent development framework for all Bloodlines sessions

Additionally, a **Deterministic RTS Architecture Bootstrap Prompt** was generated for Phase 1 simulation foundation (see Section 48).

A **Session Memory Ingestion System Instruction** was also generated, defining the full extraction and preservation protocol for future ingestions (preserved in the Append-Only Log, Ingestion 003).

---

---

### 2026-03-15 — Player Manual Ingestion (60-Part Comprehensive Design Document)

**Event:** Ingestion of the Official Player Manual, a comprehensive 60-part game design document that significantly expands detail on all core systems, introduces new mechanical concepts, provides extensive new player guidance, and deepens the strategic philosophy of Bloodlines.

**Source document:** 02_SESSION_INGESTIONS/SESSION_2026-03-15_player-manual-raw-input.md

**Scope:** Parts 1-60 covering core definitions, world/map systems, resources, population, territory, bloodline management, founding houses, lesser houses, recruitment culture, faith depth, specialization/levels, neutral Trueborn city, war/capture/marriage/assassination, elite forces, victory condition late-game escalation, alliances, early/mid/late game pacing, new player guidance, covenant play guides, strategic philosophy, moral economy, and replayability.

---

## 51. Match Scale Expansion (Player Manual)

The Player Manual establishes more specific match parameters:
- Design target assumes extreme stress test games lasting six to ten hours that still feel coherent, balanced, and fair in the Command and Conquer sense
- Map scale is roughly ten times the scale of typical RTS maps
- The game supports matches as short as two hours but is intentionally built for long sessions: six, eight, and ten or more hours
- The game is explicitly balanced for six to ten hour stress tests where counter logic still functions and specialization still matters

---

## 52. End-of-Match XP and Replayability System (Player Manual)

A new system concept introduced:
- End-of-match XP rewards exist
- XP is earned even if you do not place first
- Being second, third, or fourth remains rewarding
- XP contributes to a separate progression system that can unlock starting advantages, variations, cosmetics, and early bonuses
- The intent is that Bloodlines is a game you live in and master over time, not a single win-or-lose ladder click
- Not winning outright is not a dead end

---

## 53. Resource Count Conflict Note (Player Manual)

**CONFLICT:** The Player Manual references four primary resources: gold, food, water, stone. The existing canon (Section 8) defines five primary resources: gold, food, water, wood, stone. Wood is entirely absent from the Player Manual's resource list. Both versions are preserved per archival rules. This requires explicit resolution in a future session.

The Player Manual also confirms Influence as a secondary resource representing political weight, diplomatic leverage, and the dynasty's capacity to sway events and people.

---

## 54. Water System Expansion (Player Manual)

Water mechanics expanded:
- Water is the lifeblood of population
- If a kingdom is cut off entirely from water and cannot trade for it, the population suffers major negative effects
- Water denial cannot become a trivial hard counter to military power
- The game must prevent "water denial equals instant defeat" cheese while still keeping water critical
- Water scarcity is intended to generate diplomacy, conflict, and infrastructure investment
- Water is a strategic vulnerability protected by mechanics that prevent trivial denial

---

## 55. Population and Housing Expansion (Player Manual)

Population mechanics expanded:
- Population destruction is not simply "collateral" — it is long-form damage that can break a dynasty's future
- Population is an imperative asset to protect
- Population supports training paths such as farming, military, and other roles
- A reckless war machine can burn through manpower and collapse
- A stable dynasty can survive defeats and come back stronger

Housing expansion:
- Housing is the scaling gate for larger armies and kingdoms
- Buildings can be upgraded later if prerequisites are met

Dark extremes and world pressure:
- If extreme dark behaviors occur at scale, irreversible population decline can occur unless other dynasties intervene
- The world can enter a state where dark faith excess has gotten out of control and must be stopped
- This is a systemic pressure mechanic creating late-game coalitions, forced wars, and global response

---

## 56. Founding House Conflict Note (Player Manual)

**CONFLICT:** The Player Manual lists eight founding Houses:
1. Trueborn (Silver hair)
2. Highborne (Darker gold with highlights hair)
3. Ironmark (Black hair)
4. Hartborne (Orange hair)
5. Whitehall (Brown hair)
6. Westgrave (Vivid red hair)
7. Goldgrave (Bright gold hair)
8. Oldcrest (Black, gray, pepper mixed hair)

The existing settled canon (Section 42) lists nine founding Houses: Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest.

The Player Manual's eight-house list matches the Session 3 variant previously archived as superseded. Both versions are preserved. This requires explicit resolution.

**New detail:** Specific hair color assignments are introduced as a primary visual identity marker for quick battlefield identification and dynastic recognition across generations. These are locked to the houses listed in the Player Manual version.

---

## 57. Trueborn House Expanded Lore (Player Manual)

Expanded Trueborn details:
- Trueborn is believed through lore to be no longer around, but in reality still exists and can influence random mechanics and late-game emergence
- Trueborn can re-emerge through tribes reforming or through the neutral zone trade hub re-dedicating allegiance to Trueborn if no player conquers or dominates that central zone
- The Trueborn city is associated with the Trueborn banner
- Throughout the game, dynasties can contribute to the Trueborn city or ignore it; contribution history matters
- If the Trueborn city has not been conquered by late game, it can choose to rise up early against the dynasty approaching victory conditions too quickly or in a destabilizing way
- It can recruit other dynasties under the Trueborn banner in a coalition response
- This mechanic eliminates early rush victories and prevents dynasties from cheesing a fast win
- It forces the approaching victor to withstand world reaction and proves legitimacy through survival of a global challenge
- The Trueborn coalition response is sensitive to world state and contribution history
- Dynasties that contributed may be more likely to answer the banner or be rewarded
- Dynasties that antagonized or ignored the Trueborn may be treated differently

---

## 58. Lesser Houses Expansion (Player Manual)

Expanded lesser house details:
- Lesser houses exist under the main dynasty in layered feudal fashion similar to Game of Thrones
- Can be AI-controlled semi-autonomous actors or fully controlled extensions of the player
- Player should have optional automation or deeper involvement
- As the realm expands, bloodline members can be delegated to govern lesser houses
- Exceptional individuals can emerge from lesser houses (tournament-winning champion knight, exceptional trader)
- Lesser houses can fracture under extreme conditions
- War heroes and bloodline champions can be granted titles and lands as a lesser house
- The granting of titles and land is a major dynastic act with significant positive effects
- Creation of lesser houses provides additional leadership capacity and reinforces dynastic power

---

## 59. Recruitment Culture Expansion (Player Manual)

Recruitment system expanded significantly:
- Recruitment is an adjustable slider system where multiple approaches can be blended and adjusted over time
- The dynasty can evolve its recruitment strategy depending on performance and circumstances
- A dynasty can mix firstborn obligation with paid soldiers or go all faith volunteers
- The system allows adaptation after defeats, reflecting dynastic learning, desperation, reform, and evolving culture

Recruitment approach components:
- **Family obligation mechanisms:** families commit firstborn children to military training, or all sons, or other degrees of obligation. Increases recruitment intensity but can strain population stability and long-term morale depending on war losses
- **Paid professional soldiers:** dynasty pays for men to fight. Creates professional forces but depends on economy and logistics. Economic instability can threaten loyalty
- **Faith militancy volunteers:** most loyal militants volunteer through faith. Recruitment capacity depends on faith intensity, covenant alignment, and religious legitimacy

Sons and daughters in military:
- Whether the dynasty expects or permits sons, daughters, or both to join military structures is a strategic choice
- Including daughters can impose a negative buffer on raw attack output but a positive buffer on healing, sustainment, and recovery
- This is about role distribution and battlefield endurance, not a simplistic weaker model

Sustainment as core mechanic:
- Combat must include sustainment and support dynamics
- A reckless war machine that lets soldiers die with little support burns through manpower quickly
- Morale, loyalty, and sustainment are core factors, not superficial bonuses
- Dynasties that neglect care for wounded, logistics, and support experience lowered morale and increased instability
- This ties directly into the conquest stability rule where neglect can cause mutiny or defection

---

## 60. Faith System Expansion (Player Manual)

Major faith system expansions:
- Faith replaces any earlier use of the word "arcane" — the spiritual system is called Faith
- Houses have no inherent leaning. Houses do not start aligned. This prevents pre-locking before the map context is known
- Faith is what you believe in, anchored to four covenant identities
- Each Covenant has definitive worldview lines and late-game apex outcomes
- There is no automatic drift toward neutrality beyond consequences of neglect
- Faith intensity changes by actions and maintenance, clearly understood by the player
- Faith costs can scale progressively based on how much of the population practices the faith
- Higher practicing population increases scale, cost, and consequences of major faith actions

Level 4 faith divergence:
- Level 4 is the major irreversible divergence point for faith
- At Level 4, a player can commit to hardline fanatical expression of their Covenant
- This triggers extreme behaviors and unlocks grand structures and apex mechanics
- Level 5 contains endgame grand structures and capabilities specific to prior choices

Spiritual manifestations expanded:
- Spirits of fallen bloodline generals returning during war as angels or demons
- Evil faith at high levels can spawn a demon that corrupts an army and turns it bloodlusted, sometimes going on campaigns on its own
- These manifestations are rare and powerful, tied to faith intensity and world state

---

## 61. Covenant Details Expansion (Player Manual)

**The Old Light (Covenant of the First Flame):**
- Based on praying to the old gods, moral and protective
- Emphasizes protecting children and the innocent, acting with integrity, not enslaving people
- Late game: holy armies, blessed warriors, Christ-like anointed characters capable of resurrection, healing, removing plague or disease
- Includes temples, unity, prayer, and sacred orders
- Not dependent on culture integration requirements
- Can be protective and moral without being obligated to integrate cultures
- About stewardship and protection
- Not passive — has late-game access to formidable sacred military power
- Strongest when paired with disciplined governance and real military competence

**The Blood Dominion (The Red Covenant):**
- Leans into enslavement, conquest, and sacrifice
- Believes in the power of war and conquering and enslaving and sometimes sacrificing people
- Inspired by brutal sacrificial civilizations within medieval theme
- Late game: summoning a demon or hell-equivalent manifestation, raising a cursed army
- Fanatical states may take no prisoners — only total murder in conquest
- Can include blood rites and plague-like effects
- Major downsides: isolation, trade refusal from others, forced conflict dynamics
- Strongest when played with disciplined brutality, not indiscriminate madness
- Most dangerous ruler knows when terror advances power and when it invites premature retaliation

**The Order (Covenant of the Sacred Mandate):**
- Structured doctrine and institutional belief
- Emphasizes authority, law, sacred mandate, enforced order
- Can create powerful institutional systems, courts, and conversion infrastructure
- Not good or evil in simple terms — represents doctrinal authority and structured conversion pressure
- Can serve as Vatican-style seat of belief and conversion system in late game
- Power comes from building structures of obedience and meaning that outlast individual engagements
- Well suited to dynasties that want to make the world feel administratively and spiritually claimed
- Danger: can become rigid or coercive if not supported by actual strength and competence

**The Wild (Covenant of the Thorned Elder Roots):**
- Focuses on mysticism and nature, calls on gods of nature and powerful mysticism
- Late game: recruit animals as uncontrolled guerrilla element (packs of aggressive wolves and bears)
- Includes weather and natural disasters
- Has resurrection and healing elements in a nature-based way
- Includes primal rites, groves, spiritual connection to the land
- Not a narrow "druid" theme — can become a terrifying strategic identity
- Especially powerful in certain terrains and for dynasties willing to let conflict become less orderly
- Best Wild dynasties understand how to align people, land, and bloodline with something older and more dangerous than institutions

---

## 62. Specialization and Grand Structure Lockouts (Player Manual)

Expanded specialization system:
- Players are not forced to choose one doctrine immediately, but advancement beyond a cap requires commitment
- The major point of no return is Level 4, not Level 5
- Hybrid builds should never defeat a fully committed specialist player
- The system must force specialization to reach the most powerful options

Grand structure lockout examples:
- Grand Trade Exchange: economic power, high war citadels locked
- Grand War Foundry: military state, advanced trade infrastructure locked
- High-end Faith structure: kingdom leans into faith, other infrastructure capped
- This concept must have multiple layers and levels of lockouts and escalating specialization

---

## 63. Neutral Zone Trade Hub Expansion (Player Manual)

Expanded neutral city mechanics:
- A major neutral zone exists that all sides must contribute to with money, resources, and potentially armies
- It becomes a central trade hub
- Capturing it provides a massive advantage and accelerator but is not itself a victory condition
- It increases ambition and forces other players to act by accelerating expansion, recruitment, and extracting percentage of money from trades beyond your own
- A player could attempt to attack and conquer it, but it is incredibly difficult
- The Trueborn city is a moral and structural test of a match
- It acts as a memory-bearing institution that remembers how dynasties treated it
- It is a barometer of whether the world will accept your rise

---

## 64. War, Capture, and Dynastic Consequences Expansion (Player Manual)

Major dynastic interaction expansions:
- Bloodline members can lead armies, and major actions require dynastic leadership involvement
- Failure in war can result in bloodline members being captured, killed, enslaved, sacrificed, or ransomed
- Captured bloodline members create significant penalties at home
- If the next heir is enslaved, homeland suffers large negative modifiers because the rightful leader is in bondage

Handling options for captured bloodline members:
- Keeping them enslaved as workers
- Taking them as wives if the capturing bloodline's culture supports polygamy
- Killing them
- Ransoming them
- Nobility can be taken as wives, husbands, or enslaved as servants

Marriage and dynasty emergence:
- Alliances can become stronger through marriage into other families
- Repeated patterns of prisoner treatment shape reputation, diplomacy, fear, alliances, and cultural identity over years and generations
- Children of mixed bloodline through marriage can branch off as their own dynasty
- They may announce loyalty to one bloodline or another due to heritage and upbringing
- Creates emergent dynastic webs, rival claims, legitimacy disputes, and alliance possibilities
- Late game can allow newly formed dynasties to emerge from royalty of older houses to make claims over current leaders

---

## 65. Covert Action and Assassination (Player Manual)

Expanded covert warfare:
- A rogue or assassination element exists
- Enemy bloodlines may attempt assassination of bloodline members or great leaders when they cannot win militarily
- Assassination operations can succeed or fail
- Operatives or bloodline-linked agents can be captured
- Successful or captured assassination attempts have major consequences
- Open up additional war categories, guerrilla campaigns, and strategic pivots
- The ability to conduct operations can be influenced by the dynasty's faith, conviction, and developed capabilities
- A powerful realm must defend more than borders — it must defend key bloodline members, legitimacy, succession certainty, and leadership figures
- A successful assassination can break a line of succession, destabilize a population, open a lesser-house fracture, create civil tension, or ruin timing
- A failed assassination can create diplomatic crises, expose intent, and justify retaliation
- Covert warfare ensures weaker dynasties still have tools to matter, retaliate, or reframe conflict

---

## 66. Elite Forces and Born of Sacrifice Expansion (Player Manual)

Expanded elite army details:
- Born of Sacrifice involves sacrificing existing forces
- Specific concept: sacrificing five armies with the right faith knowledge level to create an advanced-level army with heavy or siege-adjacent capability
- When created, the player can name it or a random name generator can name it
- Troops can be promoted after multiple successful battles
- Great champions can emerge from battle and continue as great generals or be granted land to form a lesser house

---

## 67. Victory Conditions — Late-Game Escalation Mechanics (Player Manual)

This is a major expansion of victory condition detail. Each victory condition now has specific late-game escalation mechanics.

**Victory conditions confirmed as core set:**
- Military conquest
- Economic dominance
- Faith influence
- Territorial governance
- Dynastic prestige

**Population loyalty removed:** Not favored as standalone victory condition because it overlaps with territorial governance and dynastic prestige. Design direction is to remove it or merge its function. Loyalty remains critical as a mechanic tied to territory control.

**Economic Dominance Late-Game Escalation:**
- Dynasty can attempt to change the economic standard away from simple gold into its own currency
- Through successful economic and trade campaigns, their currency gains adoption
- Eventually forces the world onto their standard
- Currency adoption can force wars
- If one dynasty nears currency dominance, other dynasties may refuse adoption on defined grounds and demand conflict
- A timer or date can be set where battle is waged to decide outcome

**Faith Influence Late-Game Escalation:**
- Faith victory is not merely population conversion
- In late game, if enough of the world is heavily invested in the dynasty's faith, the dynasty can elevate the bloodline claim
- The dynasty names their bloodline as blood descendants of the highest power or god of that faith
- The world comes to believe the bloodline has divine right to rule
- Faith victory is achieved when belief and legitimacy reach the threshold where the world accepts the bloodline's divine claim under that covenant

**Territorial Governance Late-Game Escalation:**
- Not just map control through conquest
- Includes a late-game dynamic where smaller territories begin joining the dynasty willingly and unprovoked
- Seeking prosperity and benefits of exceptional infrastructure and stability
- Infrastructure, governance, and prosperity become an attractor converting independent territories into voluntary integration
- Territory and loyalty must both be satisfied for true control

**Dynastic Prestige Late-Game Escalation:**
- Through ages and multiple generations, the population concludes the bloodline should be the rightful ruler
- Rival bloodlines do not have sufficient historical significance to dispute the claim
- Prestige disputes can still occur, triggering grand wars
- Major prestige dispute war may require approximately five to ten bloodline members from each disputing bloodline present on the battlefield to the death
- Losing bloodline may suffer such loss of descendants or key heirs that it can no longer credibly dispute the claim

**Military Conquest Stability Requirement:**
- If the dynasty neglects population stability, army sustainment, and morale, mutinies and defections can wreck the dynasty
- Rush conquest or careless war creates long-term internal instability
- Can prevent victory or cause collapse

---

## 68. Alliances and Diplomacy (Player Manual)

- Alliances exist and can be binding for a period and breakable after a set period
- Alliances can be strengthened by bloodline marriages
- Defection of bloodline members should only occur under extreme neglect and circumstances

---

## 69. Starting Leader Options Expansion (Player Manual)

Starting leader options expanded with lore justifications:
- Father, first oldest son, second oldest son, brother of the king, firstborn son of the king
- Each starting choice has unique advantages, playstyles, and disadvantages
- Justifications include: oldest unable to bear children, father old or sick, brother being an incredible war general
- Hidden synergies exist for replayability
- A random factor can be determined at match start and disclosed to Houses to help direct toward bonuses
- Example: a house historically from high plains but a second son drawn to the sea, shaping trade and expansion bonuses

---

## 70. Early Game Design Philosophy (Player Manual, Parts 21-24)

The early game is extensively documented as a critical phase with several simultaneous jobs:
1. Secure basic survival: food and water before everything else
2. Establish housing at a pace matching ambitions
3. Scout the world (not optional — faith choice depends on it)
4. Begin forming internal dynastic plan

Key early game principles:
- Early shortages ripple farther than expected due to pooled population
- A brief economic mistake can become a dynastic weakness visible hours later
- Inexperienced players who expand infrastructure too aggressively without housing can quietly destabilize
- Level 1 is the phase where the dynasty is still becoming itself in relation to the world
- Faith selection delayed until after exposure creates a more organic early game
- Players who treat Level 1 lazily choose covenant based on fantasy preference rather than strategic context

Starting position reading:
- A good start gives multiple viable openings without immediate collapse exposure
- Strong starts: reliable food, defensible water, room for housing, at least one economic path, at least one defensive fallback
- Dangerous starts: exposed water, slow food scaling, cramped housing, too many early hostile tribes, weak region with no compensating strength

---

## 71. Mid-Game and Late-Game Design Philosophy (Player Manual, Parts 28-31)

Mid-game transition:
- Begins when dynasty stops merely surviving and starts revealing its direction
- Where early decisions are tested for structural soundness
- Bloodline management becomes visibly important
- Player must stop thinking like conventional RTS player and start thinking like a ruler

Specialization philosophy:
- Early freedom is real but Bloodlines rejects late-game hybrid dominance
- By Level 4 the game confronts you with real irreversible choices
- Grand specialization structures close other high-end paths
- Purpose of specialization is to force identity, not narrow the game
- Late-game dynasties must feel distinct, not identical empires with different unit skins

Level 5 apex:
- Not where identity begins — where prior commitments bear exceptional fruit
- Dynasty shape should be obvious to everyone in the match
- Purpose is to culminate direction, not introduce it
- Match begins feeling historical rather than tactical
- World should feel like a civilization struggle

---

## 72. Strategic Philosophy Concepts (Player Manual, Parts 42-46)

**Reckless success punishment:** Early battlefield success can hide deep internal damage. Population strain, recruitment culture stress, morale damage, legitimacy erosion can all accumulate under a surface of tactical victory.

**Strong vs loud realm distinction:** A loud realm moves armies everywhere and looks intimidating. A strong realm has water security, food depth, housing matching expansion, loyalty supporting demands, bloodline planning, faith intensity, and sustainment. Loud realms sometimes win; strong realms usually endure.

**Defense as active ideology:** Defensive build style is viable, potentially tied to populism, nationalism, and internal loyalty. A defensive dynasty builds power through fortification, high loyalty, coherent governance, population stability, and national feeling.

**Population as body of realm:** Population is the bridge between ethics and strategy. Cruelty is not free. Disorder is not free. Neglect is not free.

**Moral economy:** The game creates a moral economy without becoming simplistic. No path is simply "good = rewarded" or "evil = punished." Every path has forms of power and forms of risk. The game asks what different forms of power cost, what they enable, and how populations interpret them under specific conditions. Faith and Conviction cannot be treated as simplistic black and white systems.

---

## 73. Loyalty Collapse Dynamics (Player Manual)

Loyalty collapse does not always arrive as a dramatic event. It can accumulate quietly:
- Families become less enthusiastic about contributing sons
- Sabotage incidents become more frequent
- Tax or resource contribution feels weaker
- A captured heir or failed campaign adds shock
- Heavy-handed behavior without sufficient legitimacy turns pressure into fracture
- By the time the player realizes, the collapse feels sudden but was forming gradually

This is why legitimacy, perception, morale, prisoner handling, war losses, and governance quality matter.

---

## 74. Recovery Mechanics and Match Endurance (Player Manual)

Recovery is a real part of gameplay in long matches:
- A severe defeat is not necessarily the end
- Dynasties can bend without immediately breaking if they respond intelligently
- After defeat, first priority is diagnosis not revenge
- Then stabilize: water, food, population protection, housing, morale, recruitment review, faith intensity, family tree review
- Only after stabilization should revenge become meaningful
- A player who learns recovery will outperform a player who only knows how to snowball

---

## 75. Match Rhythm for Ten-Hour Sessions (Player Manual)

The rhythm of a ten-hour match:
1. Opening phase: securing survival, reading the world, laying foundations
2. Early expansion phase: first real commitments to economy, bloodline structure, scouting-led identity
3. Covenant decision phase: transforms exploration into spiritual direction
4. Developing mid-game: reveals whether realm is structurally real or theatrically fragile
5. Pre-specialization phase: multiple futures still possible but not indefinitely
6. Level 4 divergence phase: broad contest becomes struggle among defined powers
7. Late game: currency wars, divine claims, prestige crises, territorial attraction, manifestations, hostage politics, Trueborn coalition response

The game is intended to change genre texture as it progresses: begins as a severe large-scale RTS, gradually becomes a dynastic civilizational struggle.

---

## 76. Memory as Core Game Theme (Player Manual)

Bloodlines is fundamentally a game about memory:
- Family tree remembers who the bloodline is
- Population remembers what kind of rule it has experienced
- Covenant system remembers whether faith was actually lived
- Conviction system remembers patterns of action
- World remembers excess and may demand intervention
- Trueborn city remembers contributions and neglect
- Prestige is built from remembered generations
- Marriage politics remember old bonds and contaminations of blood
- Prisoner handling remembers cruelty or restraint

The player is never only managing the present. They are constructing remembered legitimacy or remembered horror.

---

## 77. Common New Player Errors (Player Manual)

Documented common errors:
- Treating it like a simpler RTS and ignoring bloodline management
- Choosing covenant based on aesthetics rather than match context
- Underestimating water and overestimating gold
- Expanding faster than loyalty and housing can support
- Assuming military victory excuses neglect
- Failing to watch the Trueborn city and contribution history
- Trying to remain broad and undefined too long
- Assuming moral systems are soft flavor instead of real strategic architecture

---

## 78. Opponent Reading and Strategic Assessment (Player Manual)

Questions to assess an opponent:
- Are they overexpanding
- Are they underhoused
- Building deep food/water resilience or surface economy
- Investing in faith intensity seriously or not supporting their covenant
- Creating loyalty through legitimacy or forcing obedience through momentum
- Using bloodline members boldly or cautiously
- Building toward which victory path
- Ignoring the Trueborn city
- How they treat prisoners
- What future they appear to be building

---

## 79. Known Unknown Rule (Player Manual)

The Player Manual contains eight founding house names. If asked for additional dynasty lists beyond these eight, or for renamed variants not present in the manual, the response should be that they are not defined in this master memory and the user should provide the list for canonical incorporation.

---

## 80. Conflict Resolutions — 2026-03-18

### House Count: Nine Houses Confirmed

The canonical founding house count is nine. The Player Manual's 8-house variant was a drafting variant and does not supersede settled canon. The canonical nine founding houses are:

Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest

The Known Unknown Rule from section 79 is updated accordingly: the canonical nine above are the definitive list.

### Primary Resources: Five Confirmed Including Wood

The canonical primary resources are five: gold, food, water, wood, stone. The Player Manual's omission of wood does not supersede settled canon. Wood remains a primary resource alongside the four the Player Manual listed.

### House Hair Color Assignments Reconciled with Canonical Nine

Six houses have confirmed hair color assignments from the Player Manual:
Trueborn: Silver
Highborne: Darker gold with highlights
Ironmark: Black
Goldgrave: Bright gold
Whitehall: Brown
Oldcrest: Black, gray, pepper mixed

Two houses have mapped assignments from Player Manual near-equivalents (Hartborne mapped to Hartvale, Westgrave mapped to Westland):
Hartvale: Orange
Westland: Vivid red

One house has no assigned hair color:
Stonehelm: Unassigned

---

## 81. Second Design Ingestion — 2026-03-18

**Event:** Ten-section design content ingestion. All entries below are canonical and preserved in full. These decisions were locked during this session and must not be contradicted without explicit revisitation.

---

### Warcraft 3-Aligned RTS Model (LOCKED)

The game Bloodlines is explicitly aligned to a gameplay and presentation philosophy closely modeled after the RTS structure of Warcraft 3.

This alignment is intentional and foundational. It adds to the existing Command and Conquer design inspiration rather than replacing it. Warcraft 3 alignment governs unit count, engagement scale, and the role of elite units.

Characteristics:
* Lower unit count compared to large-scale RTS games
* Increased importance of individual units
* Emphasis on micro-management and positioning
* Combat engagements that are deliberate and meaningful
* Strong battlefield readability at all times
* Clean UI layout that supports layered systems
* Elite unit presence (Bloodline Members) as central gameplay anchors

Rationale:
* Prevents system dilution caused by large-scale unit chaos
* Preserves clarity while introducing complex layered systems (Faith, Conviction, Bloodline)
* Allows Bloodline Members to remain visible, meaningful, and impactful
* Supports longer-form matches without visual or cognitive overload

---

### Bloodline Members as Core Gameplay Entities (LOCKED)

Bloodline Members function as central units in gameplay, similar in structural importance to hero units in Warcraft 3, but with significantly expanded systemic depth and consequence.

Bloodline Members are:
* Persistent entities across the match
* Limited to 20 active members (with overflow entering dormant state)
* Present in both battlefield and meta-layer systems

Bloodline Members can:
* Lead armies
* Participate directly in combat
* Be captured by opposing players
* Be married into other houses (diplomatic function)
* Be sacrificed (depending on faith path)
* Be assigned roles that influence gameplay systems

Roles include: General (military leadership), Diplomat (political influence), Trader (economic influence), Religious figure (faith system influence).

They are NOT disposable. They are dynasty anchors, identity carriers, long-term strategic assets, and mechanically and emotionally significant. Their loss, capture, or sacrifice must have meaningful impact.

---

### Faith and Conviction as Separated Systems (LOCKED)

Faith and Conviction are explicitly defined as independent systems. This separation is a core design principle.

**Faith System:**
* Selection of one of four ancient faiths at the end of Level 1
* A measurable intensity level that must be maintained
* Resource costs tied to participation and scale
* Population involvement requirements
* Rituals and ceremonies tied to infrastructure
* Evolves over time and impacts visual identity, gameplay effects, and late-game divergence paths (Level 4)

**Faith Duality — Each faith contains both:**
* Dark expression path
* Light expression path

Example (Blood Dominion):
* Dark Path: ritual sacrifice of enemies, blood ceremonies, dedicated altars, population consuming blood during ritual
* Light Path: animal sacrifice instead of human, structured ceremonial worship, resource trade-offs (food impact), priest and priestess development

This duality applies to ALL faiths.

**Conviction System:**
* A behavioral morality spectrum
* Determined by player actions over time
* NOT tied to faith selection

Critical rule: A player may follow a dark faith path and still maintain positive conviction. A player may follow a light faith path and still maintain negative conviction. Faith and Conviction must never be forced into alignment.

---

### Visual Identity System (LOCKED)

Each Bloodline is defined by a unified identity system:
* Hair color (primary identifier)
* Color palette (primary, secondary, accent)
* Symbol family (not a single symbol — must support variation across UI, units, buildings, banners, rank indicators)
* Architectural design language
* Material tone
* Conviction-based visual modifiers
* Faith-based visual modifiers

**Hair Color System (LOCKED — all nine assigned as of 2026-03-18):**
* Trueborn: Silver
* Highborne: Dark Gold (with highlights)
* Ironmark: Black
* Goldgrave: Bright Gold
* Stonehelm: Brown
* Westland: Vivid Red
* Hartvale: Orange
* Whitehall: Neutral Brown
* Oldcrest: Black / Gray / Pepper

Stonehelm was resolved as Brown on 2026-03-18. Whitehall distinguished as Neutral Brown to differentiate from Stonehelm. All nine hair colors are now assigned.

**Symbol Family System (LOCKED):**
Each bloodline must have a core defining symbol and a family of symbol variations. Symbols must not be singular or static. Unused symbol variants may be assigned to lesser houses.

---

### Dynamic Visual Evolution (LOCKED)

All visual representation is influenced by three layers simultaneously:
1. Bloodline (base identity)
2. Faith (overlay identity)
3. Conviction (tone modifier)

**Conviction Visual Modification:**
* Dark Expression: sharper architecture, dim lighting, fire elements, aggressive visual tone
* Light Expression: open structures, natural lighting, clean geometry, balanced visual tone

**Faith Visual Modification:**
Faith introduces unique buildings, ritual structures, environmental props, and ceremony-specific layouts.

Example — Blood Dominion:
* Dark path: blood altars, ritual execution spaces
* Light path: ceremonial structures, organized worship areas

---

### Keep / Home UI Anchor System (LOCKED)

This is a core UI system and must be treated as foundational.

When opened, the player sees the interior of their Keep / Great Hall / Home. This environment dynamically reflects bloodline identity, faith alignment, conviction tone, and empire condition.

Visible characters: Main Bloodline Leader and immediate family (spouse, children, heirs).

Each family member visually reflects their role (General, Diplomat, Trader, etc.), age progression (child to adult to elder), and current status.

Role-specific visual indicators:
* General: armor integration
* Diplomat: refined attire
* Trader: practical wealth indicators

The Keep must visually reflect prosperity level, stability or damage, faith structures, and conviction tone.

This screen is not text-driven. It is visual-first.

The Keep acts as a visual summary of the player's civilization, a narrative anchor, and the UI identity core.

---

### Lesser House Naming System (LOCKED)

Lesser house names must include the parent bloodline root as a visible prefix or suffix. This decision was made after rejecting earlier naming approaches that lacked clarity and realism.

Rules:
* Root must be immediately visible in the name
* Names must be readable instantly
* Avoid abstract or overly complex constructions
* Avoid fantasy-style compound nonsense
* Maintain clarity over linguistic subtlety

Examples:
* Ironmark: Ironvale, Ironford, Blackiron
* Goldgrave: Goldreach, Goldhaven, Deepgold
* Hartvale: Hartwood, Greenhart
* Whitehall: Whiteford, Whitestone

Player name customization: players may customize first names of characters. Lineage tracking is maintained regardless of name changes.

---

### Character Visual Style — Realism Direction (LOCKED)

Characters must adhere to realistic proportions, natural anatomy, and non-cartoon presentation.

Explicitly rejected: oversized limbs, stylized exaggeration, cartoon-like proportions.

Accepted: grounded realism, historically inspired clothing, material-based design (leather, steel, cloth), subtle facial variation, visible age progression.

Despite realism, Bloodline Members must remain identifiable via:
* Hair color (primary identifier)
* Garment design (role + bloodline palette)
* Symbol accents
* Subtle silhouette differentiation

They must be instantly recognizable, visually distinct, and non-cartoonish.

---

### Gameplay and Visual Integration Philosophy

All systems must reinforce one another:
* Naming reinforces lineage recognition
* Hair color reinforces bloodline identity
* Symbols reinforce faction recognition
* Architecture reflects belief systems
* UI reflects dynasty and empire state

No system exists in isolation.

---

### Design Philosophy (Final)

The game must achieve:
* Warcraft 3 gameplay readability
* Realistic visual tone
* Strong bloodline identity
* Deep systemic interaction (faith, conviction, dynasty)
* Visual clarity at all times

Critical rule: every system must be understandable visually or intuitively within seconds.

---

## 82. Third Design Ingestion — 2026-03-18

**Event:** Third ingestion session. Content expands the resource system, canonical bloodline list, world structure, visual identity, faith doctrine path system, faith spread mechanics, conviction axis model, and the formal three-pillars civilizational model. All entries below are canonical and preserved in full.

---

### Core Resource System Update

A new core resource has been confirmed and added to the resource system.

The game now uses the following five primary resources: gold, food, water, stone, wood.

Wood is officially established as the fifth core resource. This entry coexists with all previous resource system discussions and does not replace prior historical references.

---

### Canonical Founding Bloodlines (Confirmed)

The official founding Bloodline Houses are permanently defined as:

1. Trueborn
2. Highborne
3. Ironmark
4. Goldgrave
5. Stonehelm
6. Westland
7. Hartvale
8. Whitehall
9. Oldcrest

These names are locked as canonical dynasty identities for the project.

---

### World Structure — Bloodlines

* There are 9 founding bloodline houses.
* Trueborn is the oldest and original ruling bloodline.
* Trueborn is tied to a neutral central city.
* Trueborn is not part of the standard early-game selection pool.
* Trueborn functions as a late-game re-emergence mechanic.
* The remaining houses represent active dynasties available to players depending on match setup, replay variation, and AI distribution.

---

### Bloodline Visual Identity — Confirmed Canonical Mapping

Each bloodline has a defined genetic visual identity expressed through hair color. This mapping must remain consistent across character models, portraits, cinematics, bloodline members, and dynasty systems.

Canonical hair color mapping:
* Trueborn: Silver
* Highborne: Dark Gold with Highlights
* Ironmark: Black
* Goldgrave: Bright Gold
* Stonehelm: Brown
* Westland: Vivid Red
* Hartvale: Orange
* Whitehall: Neutral Brown
* Oldcrest: Black / Gray / Pepper

This system exists to provide immediate visual identification of lineage.

---

### Faith System Core Structure (Confirmed)

Faith is a civilizational system representing worldview, belief structure, and societal philosophy. It is not a simple religion mechanic. It defines how a civilization understands power, order, and existence.

Faith is chosen at the end of Level 1 progression. This moment represents the point where a civilization forms its ideological identity.

The four ancient faiths:
1. The Old Light — Covenant of the First Flame
2. Blood Dominion — The Red Covenant
3. The Order — Covenant of the Sacred Mandate
4. The Wild — Covenant of the Thorned Elder Roots

These are ancient systems that existed before the current dynasties. Players choose which faith their civilization adopts and develops.

---

### Faith Intensity and Maintenance

Faith is not static. Faith has intensity which increases or decreases based on population participation, ritual activity, alignment with doctrine, and presence of structures.

Faith requires ongoing resource investment. Costs scale with population size, number of religious structures, and depth of participation.

Neglecting faith results in loss of cohesion, potential unrest, and weakened cultural identity.

---

### Faith Level 4 — Apex State

At Level 4 progression a Grand Faith Structure is constructed. The civilization reaches a final ideological state. This transformation is permanent and irreversible. It unlocks powerful late-game mechanics tied to the chosen faith.

---

### Doctrine Path System

Faith includes a second layer called the Doctrine Path. The Doctrine Path is the canonical term for the faith dark/light expression system established in the previous ingestion session (Section 81). Each Faith contains two interpretations: Light Doctrine and Dark Doctrine.

These represent different expressions of the same belief system. They are not separate faiths, but divergent cultural implementations.

The Doctrine Path system exists to: create diversity within the same faith, introduce internal ideological conflict, enable narrative depth, avoid forcing rigid playstyles, and support interaction with Conviction.

---

### Blood Dominion — Expanded Doctrine Detail

Dark Doctrine:
* Ritual sacrifice of enemies
* Elaborate blood ceremonies
* Construction of blood altars
* Population participation in blood rites
* Possible sacrifice of bloodline members

Ceremonies require specific structures, time to perform, and population involvement. These rituals can produce powerful effects.

Light Doctrine:
* Animal sacrifice instead of human sacrifice
* Ceremonial gatherings tied to celestial events
* Resource cost impacts such as reduced food supply
* Development of priesthood roles within the bloodline

Bloodline members may become priests or priestesses devoted to the faith. Large ritual structures may be constructed.

---

### Other Faiths — Doctrine Application

All four Faiths support Light and Dark interpretations. Each contains its own variation of ritual behavior, population involvement, mechanical impact, and cultural identity.

The Old Light:
* Dark — inquisitions, purges, forced control
* Light — guidance, protection, structured belief

The Order:
* Dark — authoritarian control, surveillance, rigid enforcement
* Light — efficient governance, disciplined systems

The Wild:
* Dark — primal domination, ritual hunts, nature unleashed
* Light — harmony, sustainability, balance with the land

---

### Faith Spread and Global Influence

Faith can spread across populations beyond a single civilization. Methods of spread include cultural influence, conquest, missionary activity, and shared borders.

If a single Faith becomes dominant globally, late-game religious conflict can occur. Possible outcomes: holy wars, doctrinal conflicts, civilizational division.

---

### Conviction — Core Principle and Axis Model

Faith and Conviction are separate systems. This separation is mandatory and foundational. Faith represents belief. Conviction represents behavior. These systems must never be directly tied together.

Civilizations are defined by two independent axes:

Faith Doctrine Axis: Light Doctrine / Dark Doctrine
Conviction Axis: High Conviction / Neutral Conviction / Low Conviction

A civilization can follow a Dark Doctrine and maintain High Conviction. A civilization can follow a Light Doctrine and have Low Conviction. Faith does not determine morality. Conviction does not determine belief.

Conviction measures: integrity, consistency, treatment of others, exercise of power. It reflects how a ruler behaves, not what they believe.

---

### Example Outcomes

Dark Faith + High Conviction: Structured, disciplined, internally respected societies. Harsh but principled.

Light Faith + Low Conviction: Corrupt, hypocritical, unstable societies. Externally righteous but internally broken.

---

### Final Design Structure — Three Pillars

Every civilization in Bloodlines is defined by three pillars:

1. Dynasty — defines lineage and political identity
2. Faith — defines worldview and cultural systems
3. Conviction — defines moral character and leadership integrity

These three pillars are independent of each other. Any combination is valid and produces a distinct civilization. Dynasty defines who you are. Faith defines what you believe. Conviction defines how you rule.

---

## 83. Fourth Design Ingestion — 2026-03-18

**Event:** Fourth ingestion session. Major updates to Conviction, resources, Born of Sacrifice, military systems, and victory conditions. All entries below are canonical and preserved in full per archival rules.

---

### Conviction Terminology — Moral/Cruel Axis (LOCKED)

The Conviction Axis terminology is updated.

Prior terminology: High / Neutral / Low
New canonical terminology: Moral / Neutral / Cruel

"High" and "Low" are deprecated. "Moral" and "Cruel" are the canonical terms for the ends of the Conviction Axis going forward. This applies to all system files, design documents, and future design sessions.

The axis model entry in Section 82 (High Conviction / Neutral Conviction / Low Conviction) is preserved as historical record. This section supersedes it for canonical terminology.

---

### Conviction as a Central Game Feature

Conviction is not a passive background system. It is a prominent, featured aspect of gameplay with significant mechanical weight and visible consequences across multiple interconnected systems.

**Three primary impact areas:**

Population: Moral conviction generates loyalty, volunteer recruitment, and willing contribution. Cruel conviction generates fear compliance with higher desertion risk, lower birth rates, and no volunteer contribution. Population remembers accumulated patterns of rule, not isolated acts.

Trueborn City: The Trueborn city maintains a conviction record across the match. Extreme cruel conviction risks triggering the Trueborn coalition response. Sustained moral conviction earns trade bonuses, diplomatic access, and coalition membership eligibility.

Other Dynasty Leaders: Leaders perceive and react to conviction record throughout the match. Moral rulers attract alliances and voluntary population defections. Cruel rulers face diplomatic isolation, espionage risk, and coalition formation against them.

---

### Conviction Milestones

Both ends of the Moral/Cruel axis unlock milestone powers. Neither direction is a trap. Both carry genuine strategic value appropriate to the playstyle.

Moral milestones: population morale bonus, volunteer recruitment, trade trust bonus, Trueborn diplomatic favor, light doctrine amplification, divine manifestation eligibility at extreme threshold, favorable enemy surrender behavior.

Cruel milestones: fear compliance, enemy morale penalty on contact, prisoner execution resource return, dark doctrine amplification, unbeliever sacrifice unlock at extreme threshold, Trueborn coalition preparation trigger, pre-emptive enemy coalition formation out of calculated fear.

---

### What Conviction Offers

For the ruler: Moral conviction builds legitimacy, opens diplomatic victory paths, enables voluntary territorial integration. Cruel conviction opens dark power paths, sacrifice mechanics, and extreme dark doctrine escalations. Conviction record shapes which victory paths are viable throughout the match.

For the people: Moral conviction produces faster population growth, higher voluntary contribution, higher achievable faith intensity, and volunteer military units. Cruel conviction produces slower population growth, compliance-only contribution, lower faith intensity caps, and no volunteer forces.

---

### Iron — Sixth Primary Resource (LOCKED)

Iron is added as the sixth primary resource.

Updated canonical resource list: gold, food, water, wood, stone, iron.

The prior five-resource count is superseded. Iron is now settled canon alongside the original five.

Iron timeline:
- Early game: **Updated per Seventeenth Session Canon 2026-04-25.** Iron is available from game start. Forge/Settlement Forge is required from the beginning. The prior framing below ("low significance in early game") is superseded. ~~Early game: low significance. Wood and stone dominate early construction. Initial army composition (militia, spearmen, bowmen) requires minimal iron.~~ Correct canon: iron access is available from game start; early armies may be iron-light by player choice but iron is not gated to mid-game. Wood and stone still dominate early construction but Forge is a required building from the start.
- Mid game: iron demand rises sharply as swordsmen and cavalry become the primary military force. Iron node control creates compounding military advantage.
- Late game: iron is critical. Advanced armaments, cavalry equipment (horseshoes, barding, stirrups), siege equipment, fortification ironwork, and naval construction all require iron at scale.

Production: Iron Mine building (added to Economic category). Location-dependent on fixed iron deposit terrain features. Smelting Furnace required at higher production volumes. Wood fuels smelting — creating a wood-to-iron dependency chain.

---

### Cavalry — Major Military System

War horses and cavalry are a major aspect of military strategy in Bloodlines. Cavalry is not a minor unit type. It is a full tactical dimension of mid-to-late game military composition.

Cavalry requires:
- Animal husbandry infrastructure (Stable building — added to Economic or Military category)
- Iron for horseshoes, barding, stirrups, and lance equipment
- Population to train riders

Cavalry effectiveness: high mobility, shock impact on infantry formations, flanking power, pursuit capability after routing enemies. Cavalry is one of the heaviest iron consumers per unit in the game. A cavalry-focused military strategy requires substantially higher iron production than an infantry-based strategy.

War horses as a resource: horses are a product of the ranch and stable infrastructure. They are a prerequisite for cavalry recruitment. Horse supply can be targeted and disrupted by enemies.

---

### Wild Faith — Animal Cavalry

The Wild faith has a distinct cavalry system not based on horses. Wild faith adherents can ride bears and other animals as mounts.

This provides a faith-specific military advantage with different tactical properties than horse cavalry. Animal mounts may have distinct strengths (raw strength, terrain traversal, intimidation effects) and weaknesses (lower speed, different resource requirements) compared to war horses.

Wild faith animal cavalry is a doctrine mechanic — accessible through faith investment in the Wild covenant, not through generic military buildings.

---

### Born of Sacrifice — Redesign

The Born of Sacrifice system is redesigned from its original framing. The original placeholder concept (sacrifice five armies to instantly produce one overpowered elite force) is archived and preserved as historical record. It is not deleted.

The true design intent:

Born of Sacrifice is a population-constrained army lifecycle system. Because population is finite and tightly controlled, players must periodically recycle veteran troops back into the population pool in order to train new, more modern forces.

Unit development: units gain experience through battle victories, kingdom defense (sieges repelled, territory held), and caravan escort duty. Experienced veteran troops are not interchangeable with fresh recruits.

The recycling decision: when army composition is no longer matched to the strategic situation — or when population capacity limits are reached — veteran units are returned to the population pool. This frees capacity for new recruitment of a different composition or updated doctrine.

Why "Born of Sacrifice": the new soldiers are born from the sacrifice of those who came before. The new generation inherits the institutional memory and trained legacy of the retired veteran force. This creates narrative weight: every wave of soldiers carries the history of the sacrifices that made room for them.

Elite quality is not purchased through a ritual button. It emerges from multiple army lifecycle generations — an army produced through several cycles of Born of Sacrifice carries accumulated institutional depth that fresh recruitment cannot replicate.

Conviction interaction: how a dynasty treats its retiring veterans is a conviction-shaping act. Honoring veterans with land or position (moral); discarding them as obsolete (cruel). The population notices.

---

### Dark Faith — Unbeliever Sacrifice

At the intersection of extreme cruel conviction and high dark doctrine faith intensity, dark faith adherents can sacrifice population members who do not follow their faith (unbelievers).

This is not available early. It requires:
- Specific faith buildings constructed and maintained
- High dark doctrine faith intensity reached
- Extreme cruel conviction threshold crossed
- The sacrifice action is a deliberate player choice, not automatic

Consequences: this act horrifies the Trueborn city and foreign populations. Diplomatic consequences are severe. The Trueborn coalition formation probability rises sharply. Other dynasty populations may flee, revolt, or join opposing coalitions. The power gained is real — but the political cost and coalition risk are equally real.

Dark unbeliever sacrifice is the most extreme late-game expression of the Dark Doctrine + Cruel Conviction civilization type.

---

### Global Faith Population Tracking

The game tracks a global breakdown of which faith each population segment follows, and the conviction posture of those populations.

This macro-level tracking affects:
- Holy war triggers (if one faith reaches dominant global share)
- Doctrinal conflict events
- Coalition formation motivation (populations of shared faith may resist conquest by dynasties of opposing faiths)
- Civilizational division events
- Diplomatic relations between dynasties of aligned and opposing faiths

Players can observe the global faith distribution. This creates strategic information value — knowing that the world's population is majority Old Light creates different pressure than a world split evenly among all four faiths.

---

### Economic Victory — Currency Standard (Expanded)

Economic victory is achieved by displacing gold as the world's economic standard and replacing it with the dynasty's own currency.

As trade network expands, currency gains adoption. Other kingdoms accept it, prefer it, then depend on it. At currency dominance threshold, the dynasty can buy off entire enemy armies, withhold currency access to starve rivals, and force political and military outcomes through economic leverage rather than direct military force.

Counter-play: dynasties may refuse adoption on political grounds and trigger a resolution battle — a military confrontation over economic legitimacy.

Requirements: sustained trade infrastructure investment, moral or neutral conviction record (cruel dynasties face trade partner rejection), protection of trade networks, diplomatic management of coalition resistance before dominance is reached.

Economic victory requires patience. It is the longest-build victory path. It is also the one least dependent on military superiority.

---

### Faith Buildings — Investment Requirement (SETTLED)

All four faiths require resource expenditure, time, and dedicated building construction to develop faith intensity. Faith is not free. Faith infrastructure competes with economic and military building priorities throughout the match.

A player who invests heavily in faith early gains intensity advantages that compound — but at the cost of economic or military development. A player who ignores faith until mid-game may never reach the faith intensity thresholds required for late-game doctrine mechanics.

Faith investment is a match-long commitment, not a late-game switch.

---

## 84. Scope and Scale Directive — 2026-03-18

**This directive is non-negotiable and applies to every aspect of the project in perpetuity.**

The size and scope of Bloodlines is unconstrained. There is no ceiling. There is no point at which the project is "too big." There is no development stage at which richness should be reduced or depth trimmed to meet conventional expectations.

This is not a project that will be cut down to a feasible prototype and shipped. It is a project that will be built to the full vision. The full vision is the target. Everything else is a waypoint.

**What this means in practice:**

- No system is too complex to include if it serves the design
- No lore expansion is too deep to pursue
- No content category has a cap
- No mechanic is "too much" for the player to handle — the player is assumed to be capable of handling the full depth
- Conventional game development constraints (scope creep, feature cuts, shipping windows) do not apply as design constraints here
- Depth is not an obstacle. Depth is the point.

**This directive applies to:**
- System design (no system should be simplified for feasibility reasons)
- Lore and world-building (no lore thread should be abandoned because it makes the world feel "too big")
- Unit and building content (no artificial caps on content categories)
- Match duration and scale (the game is designed for ten-hour sessions minimum; longer is a feature, not a problem)
- Map scale (roughly ten times a typical RTS map; that is not an upper bound, it is a starting estimate)
- Player count (up to ten players with AI kingdoms; more is not excluded)
- Faction depth (all nine houses deserve full development; no house is a lesser priority)
- Faith depth (all four covenants deserve full doctrine elaboration; no covenant is a shortcut)

**What richness means:**
Every system the player touches should feel like it has been thought through five levels deeper than they can see. The surface of the game should be intuitive. The depth beneath it should be, in principle, inexhaustible.

The goal is for a player who has spent a hundred hours with Bloodlines to still be discovering things. Not through procedural randomness — through designed depth that rewards attention.

**What this means for AI sessions:**
Any AI working on this project must treat scope expansion as the goal, not scope management. If something can be richer, make it richer. If a system can go deeper, go deeper. If a lore thread opens ten new threads, pursue all ten.

The risk in this project is not building too much. The risk is building too little.

---

## 85. Fifth Session Content Summary — 2026-03-18

**Event:** Major autonomous expansion session. Filled all confirmed design gaps with deep, canon-consistent content. The following areas were developed and committed to their respective system files. All content below has been appended to the canonical record.

---

### 85.1 — World History (Written in Full)

The world history of Bloodlines was written in WORLD_HISTORY.md covering four eras:

**The Age Before:** The pre-Frost world. Four civilizations existed in fragile equilibrium. The four faiths predate recorded history — The Old Light, Blood Dominion, The Wild, and The Order each emerged from different relationships with the land. The Trueborn dynasty occupied the crossroads city and served as neutral intermediary. The Great Frost was not a sudden event; it was a gradual descent that most civilizations misread until too late.

**The Great Frost:** A 20-year descent into crisis followed by 150 years of peak cold. The world did not end — populations adapted, contracted, and rebuilt around survival. Each faith responded differently: The Wild faith practitioners welcomed the change as natural order. Blood Dominion practitioners saw it as a call for sacrifice. The Order imposed discipline and rationing. The Old Light became the faith of memory and preservation. The Great Frost permanently changed the geography of the world — coastlines, forests, and fertile land shifted. Iron deposits became more accessible as glaciers stripped surface terrain. The Trueborn city survived by maintaining strict neutrality; all factions knew attacking it meant losing the only place where all survivors could trade.

**The Age of Survival:** The 150 years of peak cold. Civilizations reduced to tribal structures. Faith as the primary social organizer. The bloodlines of the nine future founding houses existed in proto-form — some as nomadic warlord clans, some as settled survivors, some as merchant networks that crossed frost lines. Iron was rediscovered and began reshaping the military potential of recovering populations.

**The Age of Reclamation:** The final thaw. Nine bloodlines asserted dominance simultaneously. The Trueborn City Compact formalized their positions and established the city as permanent neutral ground. The game begins in Year 80 of the Reclamation — old enough that the Frost is living memory for elders but distant legend for the young. The founding houses are established but the world is still contested.

Chronology table documented from Year -200 (pre-Frost) through Year 80 of Reclamation (match start).

---

### 85.2 — Full Unit Progression (Levels 2-5)

All unit tiers designed and appended to UNIT_INDEX.md.

**Level 2 — Iron Age Transition:**
Men-at-Arms *(prior name — canonical: Heavy Swordsmen line per Seventeenth Session Canon 2026-04-25)*, Pike Square *(prior name — canonical: Pikeguard)*, Scout Rider, Crossbowmen *(prior name — canonical: Boltmen)*, Siege Engineer. All faith-neutral.

**Level 3 — Ideological Expansion Era:**
Base units: Knight *(naming distinction 2026-04-25: "Knight" now = dismounted form; "Mounted Knight" = mounted form)*, Longbowmen, Shield Wall *(prior name — canonical: Bulwark Guard)*, Siege Ballista.
Faith units (8 total, 2 per faith):
- Old Light: Flame Warden (light), Inquisitor (dark)
- Blood Dominion: Blood Rider (dark), Covenant Priest (light)
- The Order: Iron Legionary (light), Warden-Enforcer (dark)
- The Wild: Thornrider (light), Bear Rider (dark)

**Level 4 — Irreversible Divergence:**
All units at this tier are faith-specific. 8 units total (one per faith per doctrine):
Sunblade Champion / Purge Templar (Old Light), Bloodbound / Covenant Scion (Blood Dominion), Iron Sentinel / Dominion Arbiter (The Order), Forest Sovereign / Bonebreaker (The Wild).

**Level 5 — Apex (One Per Faith):**
- The Unbroken (Old Light): A light-doctrine apex unit that cannot be killed while faith-linked allies surround it. The last defender of a line. When it falls, it falls alone.
- The Sacrificed (Blood Dominion): A dark-doctrine apex force born from a mass sacrifice event. Terrifying, singular, permanent until destroyed. The most powerful individual unit in the game.
- The Mandate (The Order): An army-within-an-army. A formation, not a single unit. Iron Legionaries at their absolute apex — the Order's ultimate military expression of law.
- The First Wild (The Wild): The oldest of the animals. Not ridden — followed. A creature of apex faith intensity that moves independently and answers only to Wild doctrine commands. Attacks structures and armies alike.

---

### 85.3 — Great House Strategic Profiles (All 9)

Full profiles written to FOUNDING_HOUSES.md for all nine founding houses. Each profile includes: strategic identity summary, strengths, weaknesses, and a flavor statement describing the civilization's character.

Summary of strategic identities:
- **Trueborn (Silver):** Diplomats and survivors. Begin with the highest Trueborn City trust. Best positioned for coalition work and multi-party diplomacy.
- **Highborne (Dark Gold):** Aristocratic and old-money. Population loyalty advantages. Slower military development but exceptional Bloodline Member quality.
- **Ironmark (Black):** Military specialists. Strongest base army quality. Fast iron access. Weak economic base — must conquer to sustain.
- **Goldgrave (Bright Gold):** Economic dominators. Natural currency victory path. Trade network bonuses. Weak military — must buy allies or avoid war.
- **Stonehelm (Brown):** Defensive lords of the highlands. Fortification advantages. Population density bonuses in stone terrain. Slow to expand but nearly impossible to dislodge.
- **Westland (Vivid Red):** Aggressive frontiersmen. Early war advantages. High military attrition tolerance. Poor long-term governance — must win before the game enters late stages.
- **Hartvale (Orange):** Agricultural cultivators. Exceptional food and population growth. Strongest base for long-match endurance. Military is serviceable but not exceptional.
- **Whitehall (Neutral Brown):** Intelligence specialists. Passive diplomatic information network. Espionage advantages. Can see what other dynasties cannot.
- **Oldcrest (Pepper):** The rememberers. Oldest bloodline memory. Faith intensity advantages at start. Access to Frost Ruin archaeology mechanics. Lore-rich gameplay.

Inter-house political relationship table written documenting natural allies, natural rivals, and neutral pairs at match start.

---

### 85.4 — Terrain System (All 10 Types)

Full terrain system written to TERRAIN_SYSTEM.md. Design philosophy: terrain shapes strategy without determining it. Every terrain type has meaningful trade-offs across all three dimensions (economic, military, faith).

Ten terrain types with full resource, military, and faith properties:
1. **Reclaimed Plains** — High food, moderate population; open field combat; weak faith affinity
2. **Ancient Forest** — Wood production, hidden movement; cavalry negated, infantry advantage; Wild covenant affinity
3. **Stone Highlands** — Stone, iron access; exceptional fortification; Old Light, Order affinity
4. **Iron Ridges** — Maximum iron density; siege weapon terrain; no dominant faith
5. **River Valleys** — All resources; mobility corridors; fishing bonuses; no dominant faith
6. **Coastal Zones** — Trade and naval access; amphibious threat; Old Light, Wild affinity
7. **Frost Ruins** — Archaeology caches (wood, iron, gold); haunted mechanics; Old Light faith resonance
8. **Badlands** — No reliable resources; chokepoint fortifications; no faith
9. **Sacred Ground** — Faith intensity amplification (all covenants); cannot be fortified; contested by all
10. **Tundra** — Minimal resources; memory of the Great Frost; Wild dark doctrine affinity

Full terrain interaction matrix and world generation principles documented.

---

### 85.5 — Political Events System

Full events system written to POLITICAL_EVENTS.md. 25+ named events across 6 categories. All events have trigger conditions, player options, mechanical effects.

Event categories and key named events:
- **Faith Events:** The Schism, The Calling, The Heresy, Holy War Declaration, The Covenant Test
- **Dynastic Events:** Succession Crisis, The Champion, Blood Betrayal, The Prodigy, The Elder's Wisdom
- **Economic Events:** The Iron Drought, Trade Collapse, The Abundant Harvest, Famine, Market War
- **Military Events:** The Rout, The Stand, The Desertion, The Siege Breaks
- **Diplomatic Events:** Marriage Proposal, Trueborn Summons, Alliance Betrayal, Neutral Arbitration
- **World Events:** The Frost Echo, The Old Light Returns, Trueborn Summons (military), The Great Reckoning

---

### 85.6 — Diplomacy System

Full diplomacy system written to DIPLOMACY_SYSTEM.md. Five diplomatic states (War, Hostile, Neutral, Cordial, Allied), seven agreement types, formal ultimatum mechanics, and a structured war declaration process.

Key design elements:
- Alliance Betrayal as a formal event with conviction and reputation consequences
- Trueborn Arbitration as the most binding form of peace resolution
- Envoy mechanics including intelligence gathering and covert operation capability
- Faith alliances that can override political alliances in the population's perception
- Conviction-shaped diplomacy: moral dynasties get trust bonuses; cruel dynasties get fear leverage

---

### 85.7 — Naval System

Full naval system written to NAVAL_SYSTEM.md. Six vessel types, three harbor tiers, blockade mechanics, amphibious operations, and faith-specific naval doctrine.

Vessel types: Fishing Vessel, Scout Vessel, War Galley, Troop Transport, Fire Ship, Capital Ship.

Key design elements:
- Only Coastal Zone territory can support harbors and naval forces
- Capital Ships are named, tracked, and their loss is felt
- Amphibious operations bypass all land-based fortifications and terrain advantages
- Blockade starves naval trade and fishing production simultaneously
- Weather affects all vessels; Wild faith at high intensity can influence weather events at sea
- Each of the four faiths has specific naval doctrine advantages

---

### 85.8 — Faith Doctrine Deep Expansion

All four covenants fully expanded in FOUR_ANCIENT_FAITHS.md with complete doctrine mechanics, ritual systems, and faith intensity maintenance design.

**For each faith, written:**
- Theological identity (full narrative and design philosophy)
- Faith building progression — specific building names unique to each faith (not generic "shrine/temple"):
  - Old Light: Wayshrine → Hall of Remembrance → The Eternal Cathedral
  - Blood Dominion: Blood-Altar → Covenant Hall → The Wound
  - The Order: Lawpost → Hall of Mandate → The Iron Spire
  - The Wild: Grove Marker → Spirit Lodge → The First Grove
- Full ritual system (3-6 named rituals per faith, light and dark variants each with cost and effect)
- Conviction-faith combination interaction profiles
- Divine/dark manifestation event descriptions for extreme threshold combinations

**Faith Intensity Maintenance System:**
Five-tier threshold structure:
| Tier | Label | Access |
|------|-------|--------|
| 0-20% | Latent | Faith exists in name only; no mechanics active |
| 20-40% | Active | Basic ritual access; minor bonuses |
| 40-60% | Devout | Full ritual access; faith units available at Level 3 |
| 60-80% | Fervent | Faith elite units; major bonuses; manifestation probability rises |
| 80-100% | Apex | All faith mechanics unlocked; Covenant Test triggers; Level 5 apex units available |

Faith intensity generated by: building presence, active ritual completion, territory coverage, Bloodline Member devotion investment, population faith composition.

Faith intensity decays through: time without maintenance rituals, faith-hostile building presence (enemy structures), military defeat in home territory, Bloodline Member death or apostasy.

---

### 85.9 — Conviction Action Weight Framework

Full CP framework appended to CONVICTION_SYSTEM.md. Pattern-based tracking with amplification rules.

Key moral shifts (selected): Honor peace when breaking it would be advantageous (+5), food aid to starving population (+6), protect civilian zones (+3/territory).

Key cruel shifts (selected): Sacrifice unbeliever population (-15), execute bloodline member of defeated dynasty (-8), raid water infrastructure (-7), enslave conquered population (-6).

Pattern amplification: first occurrence at full value; second occurrence same stage at 1.5x; third and subsequent at 2x (the realm reads repeated behavior as policy, not incident).

Five conviction band structure: Apex Moral / Moral / Neutral / Cruel / Apex Cruel — each with distinct mechanical access.

---

### 85.10 — Files Created or Expanded This Session

New files created:
- 05_LORE/WORLD_HISTORY.md — Full world history across four eras
- 09_WORLD/TERRAIN_SYSTEM.md — Ten terrain types, full properties
- 08_MECHANICS/POLITICAL_EVENTS.md — 25+ named events, six categories
- 08_MECHANICS/DIPLOMACY_SYSTEM.md — Full diplomacy and war system
- 11_MATCHFLOW/NAVAL_SYSTEM.md — Full naval warfare system
- 19_ARCHIVE/MILESTONE_001_2026-03-18_DESIGN_FOUNDATION.md — Milestone snapshot
- 14_ASSETS/CREATIVE_BRANCH_001_WORLD_EXPANSION.md — Creative branch (speculative)

Files significantly expanded:
- 07_FAITHS/FOUR_ANCIENT_FAITHS.md — Full faith doctrine mechanics for all four covenants
- 10_UNITS/UNIT_INDEX.md — Full unit progression Levels 2-5 with faith units
- 06_FACTIONS/FOUNDING_HOUSES.md — Full strategic profiles for all nine houses
- 04_SYSTEMS/CONVICTION_SYSTEM.md — CP weight framework appended
- 04_SYSTEMS/RESOURCE_SYSTEM.md — Iron as sixth resource, currency victory full mechanics
- 04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md — Full redesign appended

---

## 86. Sixth Design Ingestion — 2026-03-18

**Event:** Direct design session with Lance. Four open design questions resolved and committed to settled canon.

---

### 86.1 — Faction Asymmetry Model (SETTLED)

The nine founding houses use hybrid asymmetry. All houses share the full tech tree and unit roster — no house is locked out of any unit class or building type. Asymmetry is expressed through two layers:

**Layer 1 — Base stat variations:** All units have an Offensive rating and a Defensive rating. The neutral baseline is Off 5 / Def 5. House-specific deviations apply subtle differences to their versions of shared units: 5/6 (defensive lean), 6/5 (offensive lean), 4/6 (glass cannon defensive), etc. No deviation is large enough to dominate at match start.

**Layer 2 — Unique units:** Each house has one or more unique units not available to other houses. These units fit the house's thematic and visual identity. Confirmed examples:
- One house with axe iconography (likely Ironmark, black hair) has access to an Axeman unit unavailable to other houses
- One house has a Pikeman variant with a defensively stronger base stat than the standard version

Full house-by-house stat and unique unit assignments are a confirmed design gap, to be defined in a future session. The framework is locked; the data layer is open.

**Design philosophy:** The hybrid model means a player learning Ironmark is still learning the universal game, but the subtle advantages and unique unit create a distinct feeling without breaking balance. Early-game differences are intentionally small. Mid-to-late game specialization divergence comes primarily from faith, conviction, and match decisions — not house selection.

---

### 86.2 — Map Structure (SETTLED)

The map combines all three structural models simultaneously:

**Continuous terrain** (primary experience): Full real-time RTS movement across open terrain. Units walk, ride, and march across a continuous landscape. Warcraft 3-style direct unit control. Weather, terrain type, and elevation affect movement speed, combat bonuses, and visibility.

**Province regions** (governance layer): The continuous terrain is divided into named province regions. Provinces are the unit of territory control for the loyalty, conviction, and diplomatic systems. A province has a loyalty level, a population count, a resource profile, and a faith composition. Military presence in a province without sufficient loyalty triggers the instability mechanics.

**Underlying hex/tile grid** (calculation layer): Territory boundary calculations, resource node placement, and area-of-effect mechanics use a hex or tile grid underneath the visual terrain. This layer is primarily invisible to the player but defines precise territorial edges and ensures deterministic behavior.

**Scale:** Roughly ten times a typical RTS map. No upper ceiling. The map scale is a feature, not a constraint.

---

### 86.3 — Bloodline Member Aging and Death (SETTLED)

Bloodline members do not age out automatically under normal conditions. Death comes through action and event:
- Combat (leading armies, siege exposure, battlefield presence)
- Assassination (covert operations by rivals)
- Illness events (political events system)
- Sacrifice (faith mechanics, Born of Sacrifice adjacency)
- Execution by a capturing dynasty

**The Frost Elder Exception:** The initial starting leader options (Father, Uncle, equivalent) are old. They were born before or during The Great Frost. They remember the world before the ice. They carry that weight visibly — in their dialogue, their advice, their authority. They will die during the match. This is intentional and designed. Their death is a narrative milestone, not a random loss. The player knows from session start that this figure is borrowed time. Plan accordingly.

No other bloodline member ages to natural death except through extraordinary circumstances that would be event-defined.

**Design implication:** The Frost Elder starting leader creates an immediate succession pressure in the background of every match. Players who ignore it will face an unplanned succession moment. Players who prepare will have an heir already shaped for the role. This is the generational pulse that makes the dynasty feel alive without requiring all members to age.

---

### 86.4 — Faith Victory: Divine Right Declaration (SETTLED)

Faith victory is declared, not accumulated. The dynasty must make a formal in-game declaration of Divine Right: that their bloodline carries the blood of their covenant's highest divine power and therefore holds legitimate claim to rule the known world under that faith.

**The declaration process:**

1. Pre-declaration requirements must be met (and are visible to all players throughout the match): faith intensity threshold, global population faith share, specific apex faith structures constructed. Other dynasties can work to prevent these thresholds from being reached.

2. Once requirements are met, the declaring dynasty makes the formal announcement. The declaration spreads across the world as in-game information propagates through population, trade networks, and diplomatic channels. This spread takes real match time.

3. During the spread window, rival dynasties receive full visibility of the clock and the milestone requirements. They may form coalitions, coordinate attacks, and mount a military and political challenge.

4. If the declaring dynasty survives the opposition window and faith conditions are maintained, the world recognizes the claim and faith victory is awarded.

5. If the coalition defeats the dynasty — kills key bloodline members, captures the capital, reduces faith intensity below threshold — the claim collapses. The dynasty may attempt a second declaration later if conditions are rebuilt, but the political cost of a failed claim is significant.

**Transparency as a design pillar:** All milestone requirements and the spread clock are fully visible to all players from the beginning of the match. The drama comes from the race and the coalition dynamics — not from hidden information. A dynasty approaching the conditions for declaration is visible long before the declaration is made, giving rivals time to act.

**The resulting experience:** Faith victory is the most publicized, most contested, most politically charged path in the game. A dynasty that wins through Divine Right has earned it against an aware, organized opposition. The match history of that declaration — who allied against it, who defected, what sacrifices were made — becomes the narrative memory of that session.

---

## 87. Seventh Design Ingestion — 2026-03-19

**Event:** Direct design session with Lance. Significant feedback on multiple systems. Several prior decisions reversed or superseded. New systems introduced.

---

### 87.1 — Frost Elder System: ABANDONED

The Frost Elder mechanic (guaranteed-death starting leader who lived through The Great Frost) is abandoned per direct instruction. The prior Sixth Ingestion decision is superseded. Creative branch content referencing Frost Elder is void. Starting leader options (Father, Uncle, or equivalent) may remain as narrative flavor but carry no mandatory death mechanics.

---

### 87.2 — Bloodline Succession: Player Choice System

When the primary head of bloodline dies, the player actively chooses the successor. This is a designed dramatic moment, not an automatic handoff.

**The succession interface** shows each eligible candidate with projected impact metrics on the bloodline as a whole — Political Legitimacy, Capability Profile, Bloodline Impact Projection, and a Revolt Risk Indicator naming specific lesser houses or territories that will rise against each specific choice. The player knows what they're buying before they choose.

**Revolt consequences:** Some successor choices trigger uprisings. Lesser houses or territories loyal to the previous head declare independence. The player must reconquer or reclaim them, negotiate them back, or accept the loss. Five resolution paths exist: tribute, marriage offer, faith mediation, show of force, military reconquest.

**No countdown timer:** The pressure during succession is the live world map displaying in the background — enemy armies moving while the player deliberates. External pressure replaces artificial time pressure.

**Starting configurations replacing Frost Elder:** Three options at match start:
- Established Dynasty: head with spouse, adult children already active
- New Union: head recently married, no children yet — blank slate
- Widowed Patriarch: head alone with children of various ages — immediately dramatic

---

### 87.3 — Bloodline Family Depth

Marriages, children, and generational offspring are a SIGNIFICANT core mechanic. The bloodline is a living multi-generational family across the match duration. Family management — political marriages between bloodlines and lesser houses, number of children, which children commit to which roles — is deep and central to the game identity.

**Generational scale:** Children progress through Childhood, Adolescent, and Adult stages before consuming an active member slot. A large family does not immediately pressure the 20-member cap. The intended experience: a grandfather watching his grandson take first command is a moment the game is designed to produce.

**Three-tier management model proposed:**
- Committed members (8-12): Fully individual management, high player attention
- Active uncommitted roster (up to 20): Individually tracked, subject to event-driven culling
- Dormant extended family: Managed as nested branches, exceptional members surface automatically

**Open design question:** Mechanics for managing potentially hundreds of living family members across generations remain unsolved. Event-driven culling (long winters, famine, plague, war) is the leading candidate for natural population thinning. This is a high-priority design gap.

---

### 87.4 — Bloodline Member Functional Roles

Bloodline members have commitment roles determining their active contribution. Members do not need to physically occupy buildings or spaces.

**Confirmed role categories:**
- **War General:** Member functions as army commander in battle. Their presence provides command bonuses to the army. Armies fighting without a committed general lose these bonuses.
- **Trade Anchor:** Member travels with or anchors a trade caravan. Provides economic bonuses to the route. Required for high-value diplomatic trade.
- **Territory Governor:** Member anchors a specific claimed territory or region. Provides loyalty stability, suppresses revolts, generates presence bonus.
- **Faith Leader (light path):** Performs rituals that generate faith intensity. Their commitment is personally costly — Grand Rites age the character over the match.
- **Covenant Officiant (dark path):** Performs dark doctrine rites with real costs including potential sacrifice of named bloodline members.
- **Spymaster, Diplomatic Envoy, Heir Designate:** Additional confirmed roles.

---

### 87.5 — Victory Difficulty Standard

All victory paths are VERY difficult. Extended match timetable applies across the board. Individual dynasty victory over all rivals is designed to be rare. Most matches are expected to trend toward stalemate, attrition, coalition formation, and political negotiation. This is not a gap — it is a design commitment reflecting the historical reality that absolute medieval dominance was exceptional, not normal.

---

### 87.6 — Alliance Victory Path (NEW)

A sixth victory path: two or more bloodlines agree to terms of shared rulership with one ultimate ruler recognized by the coalition. Requires binding agreement through the diplomatic interface. After agreement, a Population Acceptance Clock begins — the world must reach X% (proposed: 65%) population acceptance of the coalition's terms, held for 20+ game cycles.

**Why it's very difficult:**
- Coalition partners have competing interests and will defect when they see advantage
- External enemies attack both coalition partners continuously
- Population revolts are continuous — the clock is contested, not a timer
- Partner defection resets the clock to zero
- The agreed supreme ruler must be alive and in control of core territories when threshold is reached

**The Trueborn Compact option:** Rather than conquering the Trueborn City, a coalition may bring the Trueborn in as a recognized historical authority — their endorsement accelerates population acceptance significantly. This is a diplomatic path to handling the city rather than a military one.

---

### 87.7 — The Sovereignty Seat

Every victory path is a route to the same claim: the recognized supreme ruler of all lands. The game's functional equivalent of the Iron Throne concept (not named that in-game). No bloodline wins by being second. All six paths lead to this single outcome, through different methods.

---

### 87.8 — Trueborn City: Active Rise Arc

**New mechanic:** If no bloodline has conquered or seriously challenged the Trueborn City by a defined threshold (time + power threshold — all factions at Level 3+ and city unchallenged), the Trueborn activate. They raise their banners and begin asserting themselves as the legitimate authority. They become a major NPC faction pursuing their own claim to rule all lands.

**The rise is staged:** Three distinct phases across the first hour of activation — diplomatic assertions, economic leverage, then military mobilization.

**Trueborn strategy:** They pursue Restoration, not raw conquest. They seek to back a winner who acknowledges their historical authority rather than crushing everyone directly.

**Conquest is not a victory condition.** Conquering the Trueborn City provides significant bonuses to the conquering bloodline's pursuit of their chosen victory path — it accelerates all six paths through the Sovereignty Anchor and other mechanical advantages. It does not win the match. It is a powerful strategic objective, not an endpoint.

**Conquest requirements:** Extremely difficult. 800+ strength units against Tier 5 fortifications. Civilian participation in defense. Trueborn-aligned relief forces attacking the siege ring from outside. Holding the city grants the Sovereignty Anchor — no territory the holder controls can revolt through standard events. This immediately makes the holder the target of every other dynasty.

---

### 87.9 — Late-Game Escalation Pressures

Three mechanics designed to prevent indefinite stalemate:
- **The Great Exhaustion:** Global resource drain forcing dynasties to choose sustainable fronts
- **The Succession Crisis:** Internal fragmentation for dynasties without formalized succession
- **The Legitimacy Erosion:** Final forcing function — loyalty and faith begin global downward drift regardless of governance quality, making passive defense eventually untenable

**Exile as a match state:** A dynasty that loses core territories but retains bloodline members and forces isn't eliminated — they're exiled. Hobbled but present. Can negotiate back or attempt reconquest.

---

### 87.10 — Faith Doctrine Tone Standard

Faith doctrine mechanics, rituals, and events are serious, adult, and R-rated in design sensibility. The prior creative branch faith doctrine document (vigil-keeping, candle lighting, memorial inscriptions) is explicitly superseded and rejected. The four covenants are treated as genuinely serious worldviews with real demands, real costs, and real darkness. A complete rewrite is commissioned under Creative Branch 003.

---

### 87.11 — Creative Branch 003 Commissioned

Three documents commissioned under Creative Branch 003 to supersede prior versions:
- CREATIVE_BRANCH_003_BLOODLINE_FAMILY_SYSTEM.md — Replaces CB002 bloodline members doc
- CREATIVE_BRANCH_003_FAITH_DOCTRINE_MECHANICS.md — Replaces CB002 faith doctrine doc
- CREATIVE_BRANCH_003_VICTORY_CONDITIONS.md — Replaces CB002 victory conditions doc

---

## Section 89 — Creative Branch 004: Full Bloodline Expansion (2026-03-19)

**Ingestion Source:** CB004 documents — autonomous expansion session, 2026-03-19.
**Status:** PROPOSED / CREATIVE BRANCH 004 — Awaiting Lance review before canonicalization.
**Files:**
- `14_ASSETS/CB004_BLOODLINE_PROFILES_PART1.md` — Trueborn, Highborne, Goldgrave
- `14_ASSETS/CB004_BLOODLINE_PROFILES_PART2.md` — Stonehelm, Westland, Hartvale
- `14_ASSETS/CB004_BLOODLINE_PROFILES_PART3.md` — Whitehall, Oldcrest
- `14_ASSETS/CB004_SYSTEM_INTEGRATION.md` — Cross-system reference

---

### 89.1 — CB004 Squad Trait System (All Nine Houses)

Each founding house has a UNIQUE squad trait that modifies the global squad model (5-member shared health pool with visual degradation). These traits are distinct from Ironmark's confirmed canonical "Ferocity Under Loss." All CB004 traits are PROPOSED.

| House | Squad Trait Name | Core Mechanic |
|-------|-----------------|---------------|
| Trueborn | By Precedent (Diplomatic Poise) | Squads in contested territory suppress morale-suppression from enemy force concentration — they do not rout as quickly when outnumbered |
| Highborne | As It Was in the Beginning (Covenant Bond) | Squads gain compounding morale bonus in territories where Highborne faith intensity is Devout+; penalty in low-faith territory |
| Ironmark | Ferocity Under Loss | (CANONICAL LOCKED — progressive combat bonuses as squad size decreases) |
| Goldgrave | Find the Spread (Market Intelligence) | Squads near resource nodes/trade routes generate passive resource yield bonus to nearest Goldgrave economic building |
| Stonehelm | Fortified Ground (Compression) | Squads fighting from/adjacent to Stonehelm-built defensive structures get +2 Def; no bonus in open field |
| Westland | Pursuit Momentum | Stacking speed/attack bonus for each consecutive movement-and-charge action in an engagement sequence; resets when squad holds position |
| Hartvale | Verdant Recovery (Resupply) | Squads recover one member per combat cycle when not actively engaged and within proximity of Hartvale territory/agricultural buildings |
| Whitehall | Sustained Operations | 20% lower supply consumption, 20% lower restoration costs, 50% lower non-combat attrition — no in-combat enhancement, pure logistics advantage |
| Oldcrest | Tactical Memory | Compounding combat bonus vs. specific enemy unit types previously engaged (+3%/+6%/+10%/+13% at 2nd/3rd/4th/5th+ engagement). Resets each match. |

---

### 89.2 — CB004 Societal Advantages and Required Disadvantages

| House | Societal Advantage | Required Disadvantage |
|-------|-------------------|-----------------------|
| Trueborn | Diplomatic Efficiency — agreements ratify faster, neutral settlement conversion without combat (via Mediator), Trueborn City trade bonuses | Thin Military Ceiling — all units at 5/5 baseline, no offensive spikes; Mediator is fragile; military advantage requires diplomatic infrastructure that takes time to build |
| Highborne | Early Faith Infrastructure — reaches Devout faith tier earlier than any other house; Cantor support unit available earlier | Faith Dependency — effectiveness depends on faith intensity in operating territory; extended campaigns beyond faith coverage area degrade performance |
| Ironmark | Early Iron Access — 20-30% faster iron infrastructure, Axeman unique unit comes online before equivalent heavy infantry for others | Blood Production — higher combat casualties, reinforcement inefficiency (forge/military labor competition), population strain under sustained operations |
| Goldgrave | Trade Network Reach — Gold accumulation rate higher than any house; Coin Sergeant enables hired auxiliaries; economic buildings cheaper to construct | No Military Identity — all units 5/5, no offensive capability; military force is expensive and reluctant; hired auxiliaries are weaker than native units |
| Stonehelm | Fortified Ground Compounds — defensive infrastructure advantages grow over time; fortifications built early produce late-game chokepoints competitors can't quickly overcome | Speed Penalty — slowest army in the game; structurally incapable of rapid response; deliberate positioning only |
| Westland | Frontier Tempo — permanent movement speed modifier across all army movement; Outrider available before standard cavalry | No Depth — extended attrition and siege warfare actively degrade performance; cannot outlast opponents in a war of exhaustion |
| Hartvale | Population Engine — more Militia produced faster and cheaper; faster neutral settlement integration; Hearthmasters extend supply chains dramatically | Quality Ceiling — all units precisely 5/5 with no upward deviation anywhere; Hearthmasters are non-combat and high-priority targets |
| Whitehall | Administrative Efficiency — 25% faster settlement integration, infrastructure building costs 15% lower, flexible strategic lane switching at lower cost | No Dominant Identity — 5/5 neutral on all units; requires reading the match and committing to a direction; weaker at everything than specialists in their specialty |
| Oldcrest | Compound Development — Bloodline Member development, territory loyalty, and squad effectiveness all compound over time through the Oathkeeper and Tactical Memory systems | Slow Early Game — deliberately disadvantaged in early match; advantages require TIME to manifest; competes poorly in short matches |

---

### 89.3 — CB004 Victory Path Affinity Matrix

Ratings: **Strong** / **Moderate** / **Weak** / **Against Grain**
Paths: Military Conquest (MC) / Economic Dominance (ED) / Faith Divine Right (FDR) / Territorial Governance (TG) / Dynastic Prestige (DP) / Alliance Victory (AV)

| House | MC | ED | FDR | TG | DP | AV |
|-------|----|----|-----|----|----|-----|
| Trueborn | Weak | Moderate | Moderate | Strong | Strong | Strong |
| Highborne | Weak | Weak | Strong | Moderate | Moderate | Strong |
| Ironmark | Strong | Moderate | Against Grain | Weak | Moderate | Weak |
| Goldgrave | Against Grain | Strong | Weak | Moderate | Weak | Strong |
| Stonehelm | Moderate | Weak | Weak | Strong | Moderate | Moderate |
| Westland | Strong | Weak | Against Grain | Weak | Weak | Weak |
| Hartvale | Weak | Strong | Weak | Strong | Moderate | Strong |
| Whitehall | Moderate | Moderate | Moderate | Moderate | Moderate | Moderate |
| Oldcrest | Weak | Weak | Moderate | Moderate | Strong | Moderate |

**Design note:** Westland is the only house with five Weak/Against Grain ratings and one Strong — the most specialized house in the game. Whitehall is the only house with all Moderate ratings — the adaptive generalist design stated plainly as a matrix. Hartvale is strong in three paths and weak in two — a population-based house that enables many approaches through demographic depth. Ironmark and Stonehelm mirror each other (both Strong in Military/Territorial respectively with corresponding weaknesses) which reinforces their historical conflict.

---

### 89.4 — CB004 Unique Units Summary

| House | Unique Unit | Category | Key Mechanic | Weakness |
|-------|-------------|----------|--------------|----------|
| Trueborn | The Mediator | Mounted support | Converts neutral settlements diplomatically; brief morale suppression aura near engaged armies | Extremely fragile, cannot survive extended combat |
| Highborne | The Cantor | Embedded faith support | Restores morale to nearby units; reduces Conviction cost of aggressive actions temporarily | Killed easily; must stay inside formations |
| Ironmark | The Axeman | Heavy infantry/structure-breaker | Off 6/Def 4; bonus damage vs. armor and fortifications; Ferocity Under Loss most pronounced on this unit | Poor vs. fast units (cavalry, skirmishers); flanking exposure |
| Goldgrave | The Coin Sergeant | Support/coordinator | Enables temporary hiring of neutral/rogue units; increases gold cost efficiency | Hired units weaker than native; cannot garrison structures |
| Stonehelm | The Rampart Spear | Defensive infantry variant | Braces against cavalry charges (dramatically reduces cavalry effectiveness); anchors multiple attacker defense | Slower than standard Spearmen; poor in pursuit |
| Westland | The Outrider | Light cavalry/ranged hybrid | Available before standard cavalry; minimal iron requirement; extremely fast; harasses/kites/intercepts | Cannot stand in direct combat vs. armored infantry |
| Hartvale | The Hearthmasters | Non-combat logistics | Extends supply lines, reduces non-combat attrition, accelerates settlement integration | Stat 2/2 (effectively non-combat); priority target; death causes severe logistics penalty |
| Whitehall | The Liaison Officer | Military diplomat/intelligence | Reveals wider radius of opponent unit composition; morale bonus vs. previously-engaged enemy types | Fragile, non-combatant, high-priority target |
| Oldcrest | The Oathkeeper | Veteran bound soldier | Combat capability scales with proximity to Bloodline Members (oath activation near members = fighting with full conviction) | Standard capability away from Bloodline Members; dependency creates formation requirements |

---

### 89.5 — CB004 Historical Rivalry and Alliance Mechanical Starting States

**Starting diplomatic modifiers (all are baseline adjustments to the Neutral state):**
- Trueborn–Oldcrest: -15 (deep historical — Oldcrest predates Trueborn's claim to authority)
- Trueborn–Highborne: -10 (governance-vs-faith worldview incompatibility)
- Stonehelm–Westland: -10 (border conflict, historical)
- Hartvale–Westland: -15 (deep resentment — Westland treats population as conquest; Hartvale treats it as sacred)
- Whitehall–Oldcrest: -5 (condescension from Oldcrest vs. economic leverage from Whitehall)
- Highborne–Goldgrave: +10 (natural alliance — economic infrastructure enables faith sustainability)
- Stonehelm–Hartvale: +10 (natural alliance — fortifications protect population; population gives fortifications meaning)
- Oldcrest–Highborne: +8 (natural alliance — both operate on deep time horizons)
- Trueborn–Ironmark: -8 (governance vs. force — process vs. result)
- Ironmark–Westland: -5 (complicated — both aggressive, historical clashes, wary distance)

---

### 89.6 — CB004 Design Notes

**CB004 expands prior creative branches in the following ways:**
- CB002 house profiles: superseded for any house where CB004 provides deeper treatment; CB002 baseline unit values confirmed where not overridden by Ninth Ingestion (Ironmark lock)
- CB003 house lore: preserved and built upon in CB004; CB004 adds mechanical depth to CB003's historical and narrative content
- Ironmark color system (Ninth Ingestion): Charcoal Iron primary / Ember Red accent confirmed; CB004 does not apply this specific override to other houses but establishes the precedent that each house's color system must be "earned" rather than generically assigned

**Pending after Lance review:**
- All nine house color system specifications need Lance confirmation
- All nine symbol systems need Lance confirmation
- Squad traits need Lance balance review
- Off/Def unit deviations need full review (some houses have CB002 values, CB004 refines several)

---

## Section 88 — Ninth Design Ingestion: Ironmark Bloodline Canonical Lock (2026-03-19)

**Ingestion Source:** User-provided locked design data block, Session 8 / continuation of Session 7.
**Status:** LOCKED CANONICAL DATA — supersedes any prior CB002/CB003 content where conflicts exist.

---

### 88.1 — Ironmark Identity Statement

**Final identity (locked):** "Ironmark is the Bloodline that reaches power first and pays for it in blood."

This is not a flavor statement. It is a design specification. Every mechanical decision for Ironmark must reinforce this duality: accelerated power acquisition paired with compounding human cost. No Ironmark mechanic should provide early-game advantage without a corresponding population, casualty, or sustainability penalty. The tradeoff is the identity.

---

### 88.2 — Color System (Locked Override)

The Ironmark color system overrides any prior assignment or creative branch proposal.

**Primary color:** Charcoal Iron — NOT pure black. The distinction is critical. Pure black is absolute and carries theatrical connotations. Charcoal Iron is the real color of worked iron, forge-smoke, and soot-stained stone. It reads as earned darkness, not symbolic darkness.

**Accent color:** Ember Red — used sparingly and with intention. Ember Red represents the heat inside the forge, the blood cost of production, the consequence embedded in the house identity. It is not a secondary color in the conventional heraldic sense. It is a signal: when Ember Red appears on an Ironmark unit, structure, or banner, it marks something important — a veteran unit, a sacrifice, a critical structure, a moment of escalation. Its rarity is what makes it mean something.

**Visual design implication:** Ironmark should read as a house of dark iron and controlled fire, not as the "evil" house or the "black house" of generic fantasy convention. The palette communicates craft, industry, and cost — not villainy.

---

### 88.3 — Symbol System (Locked)

**Dominant symbol:** The Axe — not as a weapon of war but as a tool of industry that became a weapon of war. The Axe is the symbol of Ironmark because it predates their military identity. Their ancestors used axes to fell timber for forge fuel, to split stone, to shape the physical world before they used them to fight.

**Controlled symbol variations (five canonical forms):**
1. **War Axe** — full single-blade war axe on a long haft. Military contexts: unit banners, armor markings, siege equipment branding.
2. **Split Axe** — a double-headed axe variant. Political and diplomatic contexts: council chambers, formal correspondence, treaty documents.
3. **Forging Axe** — a shorter, heavier single-blade with a pronounced poll (hammer-back). Economic and infrastructure contexts: forge buildings, resource facilities, trade goods.
4. **Broken Axe** — a war axe with a fractured haft. Memorial contexts: gravestones, sacrifice monuments, the names of fallen veterans.
5. **Raised Axe** — a war axe held vertical with the blade at the top, used as an active symbol of ongoing military commitment. Command flags, campaign declarations, unit mobilization orders.

**Symbol design rule:** The variation used must match the context. Using a War Axe on a trade document or a Forging Axe on a unit banner is a mechanical signal that something is wrong — a propaganda piece, a disguise, a forgery.

---

### 88.4 — Societal Advantage and Required Disadvantage

**Societal advantage — Early Iron Access:**
Ironmark reaches iron infrastructure 20-30% faster than the standard tech progression timeline. This is not a minor edge — it represents a meaningful window during the early-to-mid game transition where Ironmark has access to iron-dependent units, structures, and upgrades before opponents can match them. The Axeman unique unit, iron-reinforced fortifications, cavalry iron equipment, and early armory upgrades are all unlocked ahead of the field.

This advantage creates power spikes, not sustained dominance. The window is real and significant, but it closes as other bloodlines catch the iron curve. Ironmark's strategic challenge is translating early iron access into structural advantages that persist beyond the window.

**Required disadvantage — Blood Production (three components):**

1. **Increased casualties in combat.** Ironmark units engage more aggressively than their Off/Def ratings suggest — the forge culture pushes forward rather than withdrawing, which means casualty rates in won battles are higher than for comparable houses. Ironmark wins fights but pays more blood for them than the same fight would cost Stonehelm or Trueborn.

2. **Reinforcement inefficiency.** Ironmark military production is powerful but its replacement pipeline is slower than expected for a military-focused house. The reason is forge labor: the same population segment that trains soldiers also works the forges. When military losses require rapid replacement, forge production slows. When forge production is maximized, the replacement pipeline is under-resourced. The player must constantly manage this tension.

3. **Population strain under sustained military operations.** Ironmark's combination of higher casualties and reinforcement inefficiency means that extended military campaigns produce compounding population pressure. The longer Ironmark fights, the more it costs in population — not just the soldiers lost, but the economic disruption of pulling forge workers into military replacement cycles. A short decisive war is manageable. A long war of attrition works against Ironmark even when it is winning.

---

### 88.5 — Squad Model and Ironmark-Specific Combat Trait

**Global squad model (confirmed, applies to all bloodlines):**
One selection = one squad = five individual unit-characters. The squad has a shared health pool that displays as a single health bar but degrades visually — as the squad takes damage, individual members fall. At four remaining: the squad looks intact with one gap. At three: the formation visibly tightens. At two: the survivors are the veterans who outlasted everyone else. At one: a single figure fighting alone. At zero: the squad is gone.

**Ironmark squad trait — Ferocity Under Loss:**
Ironmark squads do not simply degrade as members fall. As squad size decreases from five toward one, the remaining members gain progressive combat bonuses: increased attack speed, reduced routing tendency, and a fear-aura that suppresses morale in nearby enemy units. At full squad strength, Ironmark fights like any other house. At two remaining members, an Ironmark squad is significantly more dangerous per individual than at full strength.

**Design intent:** This converts what would normally be a pure disadvantage (taking casualties) into a partial advantage. It does not eliminate the population cost — the dead members are still gone, still pulling from the population pool, still part of the reinforcement bottleneck. But in-battle, Ironmark losses make the survivors fight harder. This reflects the forge culture's psychology: the people who survive the worst conditions come out harder, not broken.

**Balance implication:** This trait interacts with the casualty disadvantage in a specific way. Ironmark wants to take some losses — enough to activate the ferocity bonuses — without burning through squads entirely. Managing the degradation curve is a skill layer unique to Ironmark play.

---

### 88.6 — Unit Values (Locked)

**Standard baseline (all bloodlines):** *(Prior framing — Off/Def is now absolute, not relative to a 5/5 neutral. Seventeenth Session Canon 2026-04-25 supersedes the 5/5 neutral baseline framing.)* Swordsman Off 5/Def 5 was the prior "neutral" reference point.

**Ironmark unit values** *(Ninth Ingestion Canon values below; Axeman Off/Def revised per Seventeenth Session Canon 2026-04-25)*:
- Militia: 6/5 (pending absolute-scale data-layer tuning)
- Swordsmen: 5/5 (locked at neutral)
- Spearmen: 5/5 (pending tuning)
- Hunters: 5/5 (pending tuning)
- Bowmen: 5/5 *(prior name — canonical: Archers)* (pending tuning)
- **Axeman (Ironmark exclusive): Off 5 / Def 2** *(Updated 2026-04-25 from Off 6/Def 4)*

**Axeman specifics** *(updated Off/Def 2026-04-25; all other identity preserved)*:
- Category: Heavy infantry / structure-breaker
- Off 5 / Def 2: High offensive output, low defensive rating. The Axeman hits hard but is fragile if flanked or caught by fast units.
- Special application: Exceptional against armored units and structures. Axemen deal bonus damage to fortifications, siege equipment, and heavily armored infantry. Against light, fast units (cavalry, skirmishers), their effectiveness drops significantly.
- Interaction with Ferocity Under Loss: The Axeman ferocity bonus is the most pronounced of any Ironmark unit type. A single remaining Axeman is genuinely dangerous in a way that a single remaining Swordsman is not.

---

### 88.7 — Strategic Identity Summary

**Early game:** Ironmark builds iron infrastructure faster than anyone. Their opening army is smaller but the Axeman comes online before equivalent heavy infantry for other bloodlines. They can contest chokepoints and deny resource nodes with force that costs more than it looks like it should.

**Mid game:** The window. Iron advantage is in full effect. Ironmark cavalry comes online with better iron-equipment than competitors. Axeman units do siege work that other bloodlines need additional tech to match. The population strain from early aggressive play is starting to register — the player must decide how hard to press before the population curve bends against them.

**Late game:** Ironmark's late game is determined by how well the player managed the casualty and population tradeoffs. A controlled Ironmark that took decisive victories in the window arrives late game with experienced squads, iron infrastructure, and production depth. An Ironmark that bled freely and relied on ferocity bonuses is running short on population and facing reinforcement bottlenecks. The late game rewards disciplined players; it punishes reckless ones.

---

## Section 90 — Session 9 Summary (Added 2026-03-19)

Session 9 was the most productive single session in the project's history. Seven parallel agents ran simultaneously at peak. Across two context windows in this session, the following work was completed:

### 90.1 — Core System Files — Session 9 Expansions

All seven core system files were brought to substantive depth. Before Session 9, most were foundational placeholders. After Session 9, all seven contain full mechanical documentation.

| System File | Lines Before | Lines After | Status |
|-------------|-------------|-------------|--------|
| CONVICTION_SYSTEM.md | ~200 | 334 | SOLID (pre-Session 9) |
| FAITH_SYSTEM.md | 229 | 749 | EXPANDED — full covenant theology, unit roster, doctrine mechanics, adult tone |
| POPULATION_SYSTEM.md | 64 | 126 | EXPANDED — 90-second cycle, housing mechanic, pool allocation, conviction mirror |
| RESOURCE_SYSTEM.md | 138 | 228 | EXPANDED — trade network, production chains, caravan system, house notes |
| TERRITORY_SYSTEM.md | 65 | 113 | EXPANDED — three-layer map, control clock, faith diffusion, governance victory |
| DYNASTIC_SYSTEM.md | 0 | 291 | CREATED — three-tier family management, eight commitment roles, succession |
| BORN_OF_SACRIFICE_SYSTEM.md | 145 | 313 | EXPANDED — full lifecycle, champion emergence, faith consecration per covenant |

SYSTEM_INDEX.md was also expanded from a 27-line stub to a full system reference document with line counts, content summaries, and a full cross-system interdependency map.

### 90.2 — New Files Created in Session 9

| File | Lines | Content |
|------|-------|---------|
| 05_LORE/TIMELINE.md | 169 | Full world chronology from Age Before through Year 80 match start |
| 11_MATCHFLOW/POLITICAL_EVENTS.md | 980 | All 28+ named political events with triggers, effects, resolution paths, counter-play |
| 13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md | 194 | Full visual and audio design direction — house architecture, faith aesthetics, unit design, music system, UI audio |

### 90.3 — Existing Files Significantly Expanded in Session 9

| File | Lines Before | Lines After | Primary Content Added |
|------|-------------|-------------|----------------------|
| 07_FAITHS/FOUR_ANCIENT_FAITHS.md | 327 | 1,778 | Full CB003 adult-tone faith doctrine mechanics — all four covenants, both doctrine paths, full ritual specifications with exact resource costs and participant requirements |
| 09_WORLD/WORLD_INDEX.md | 16 | 449 | Full procedural world generation parameters — terrain distribution, resource placement, starting positions, tribe placement, regional culture, generation variants |
| 12_UI_UX/UI_NOTES.md | 49 | 275 | All UI screens beyond Keep interior — tactical viewport, HUD, unit command, bloodline management, faith interface, diplomacy, territory map, event system, victory tracking, match end |
| 11_MATCHFLOW/MATCH_STRUCTURE.md | 121 | 252 | Six victory paths expanded, sovereignty seat concept, Trueborn rise arc timing, world events reference table, all four stages detailed |
| 18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE.md | 1,805 | ~3,000+ (in progress) | Sections 45-62: Match Structure, Victory Paths, Nine Houses at full CB004 depth, Closing Manifesto |

### 90.4 — Design Decisions Formalized in Session 9

All prior canonical decisions remain locked. Session 9 produced no contradictions with earlier ingestions. Key formalized concepts:

**Sovereignty Seat:** The canonical framing of what every victory path is pursuing — recognition as the supreme authority over all lands. No bloodline wins by being second.

**Eight Commitment Roles:** War, Trade/Economic, Defense, Governance, Faith, Scholarly/Advisory, Covert Operations, Diplomatic. These are the confirmed functional roles for bloodline members.

**Three-Tier Hybrid Family Model:** Tier 1 (8-12 committed, fully managed), Tier 2 (up to 20 active, uncommitted, individually tracked), Tier 3 (dormant beyond active cap, branch management via Nested Family Units model).

**CB003 Faith Doctrine as Authoritative:** The CB003 faith doctrine mechanics document (866 lines, written at adult/serious tone) is the authoritative source for all four covenants and both doctrine paths. All prior CB002 faith doctrine content is superseded. CB003 content is now appended to FOUR_ANCIENT_FAITHS.md in full.

**World Generation Parameters:** The map scales with player count. 2-4 players: 150-200 territory nodes. 5-7 players: 200-280. 8-10 players: 280-400+. Sacred Ground appears at 1-2 nodes per covenant (4-8 total per map). The Trueborn City is always present at the map's geographic center.

### 90.5 — Outstanding Design Work After Session 9

| Item | Status | Notes |
|------|--------|-------|
| Design Bible Sections 53-62 | IN PROGRESS | Agent writing Nine Houses + Closing Manifesto; estimated completion same session |
| CB004 Review by Lance | AWAITING LANCE | All 8 remaining bloodline profiles proposed; Hartvale unit conflict unresolved |
| FOUNDING_HOUSES.md CB004 Integration | BLOCKED | Awaiting Lance approval of CB004 content |
| Personality Traits Implementation | OPEN | 21 traits in CB003; not yet in any system file |
| Lesser House Mechanics | OPEN | Referenced throughout; not formally designed |
| Prototype Phase | NOT STARTED | Future phase after design foundation complete |

### 90.6 — Session 9 File Count

After Session 9, the total corpus of significant design files in the Bloodlines project stands at:
- 7 core system files (all expanded)
- 1 master memory file (2,689+ lines)
- 1 canonical rules file (full ingestion history)
- 1 complete design bible (3,000+ lines when complete)
- 4 creative branch documents (4 parts/indices in 14_ASSETS/)
- 3 creative branch 002 subdocuments
- 3 creative branch 003 subdocuments
- All 9 house profiles at Ironmark depth (CB004)
- Full world generation parameters, UI/UX design, audio/visual direction, political events system, timeline, match structure, faith doctrine, and system index

The project has accumulated more than 15,000 lines of original design documentation across all files.

---

### Section 91 — Session 2026-03-26: Dynastic Feedback and Corrections

**Event:** Lance provided direct design corrections and new content for the Dynastic System, Born of Sacrifice system, and early game philosophy. Ingested from SESSION_2026-03-26_dynastic-feedback-and-corrections.md.

**Corrections Applied:**

1. **Marriage and Hybrid Heirs:** Corrected earlier text that implied children of cross-dynasty marriages could freely declare loyalty or form new dynasties. Confirmed that all branching and loyalty declarations are controlled by the head of the household/player. Branching can be PROPOSED in-game with advantages/disadvantages but is ultimately determined by the player.

2. **Mixed Bloodline Dynasties:** Corrected earlier text that implied children could freely branch off or announce loyalty to other bloodlines. Confirmed that ample notification is provided about member status, and the head of the bloodline makes the final decision. Added the Bloodline Loyalty Slider mechanic tracking member defection intent (Loyal/Neutral/Seeking to defect). Reverse defection (others seeking to join your bloodline) requires successful diplomatic action.

**New Content Added:**

3. **Training Path — Mysticism:** Added as the seventh training path. Distinct from Religious Leadership in that Mysticism focuses on offensive/defensive faith abilities in war rather than institutional faith expansion. Members trained on this path deploy as Sorcerers.

4. **Roles Update:** Confirmed full role list: Commanders, Governors, Diplomats, Ideological Leader (Priest/Priestess), Merchant, Sorcerer. Sorcerer role is the combat-faith deployment path.

5. **Polygamy Rules Clarified:** Polygamy confirmed only for Blood Dominion and The Wild. Other faiths can withhold members from marriage until capture/enslavement provides opportunity. Marriage is always player-controlled, never automatic.

6. **Marriage Control Principle:** Marriage does not happen naturally in Bloodlines. No automatic matchmaking. Every marriage is a deliberate player decision by the head of the household.

7. **Starting Leader Age Ranges:** Confirmed specific ages. Father: 30-40. Eldest Son: 10-20. Second Eldest: under 10. Uncle: 25-35. Uncle's first eldest: 12-16.

8. **War Hero Promotion to Lesser House System:** Combat units earn promotions through 5 stages of field experience. Beyond stage 5, may be promoted to command (general/commander). After multiple successful campaigns, may receive lands and titles to found a lesser house. Promoted units gain cumulative combat advantages that can overcome base unit-type matchups. Added to BORN_OF_SACRIFICE_SYSTEM.md as the intersection point between military lifecycle and dynastic expansion.

9. **Early Game Design Philosophy:** Early game should focus on NPC tribe consolidation with minimal inter-house conflict. Anti-cheese principle: blitz and mass-buildup strategies that abandon non-military systems should be discouraged by design. Every viable strategy requires investment across dynasty, economy, faith, and population. Added to 08_MECHANICS/MECHANICS_INDEX.md.

**Files Modified:**
- 04_SYSTEMS/DYNASTIC_SYSTEM.md — correction notes, training paths, roles, polygamy, marriage control, defection slider, starting leader ages
- 04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md — war hero promotion to lesser house pipeline
- 08_MECHANICS/MECHANICS_INDEX.md — early game design philosophy and anti-cheese principle
- 01_CANON/CANONICAL_RULES.md — 13 new canonical entries (Eleventh Session Canon)
- 01_CANON/BLOODLINES_MASTER_MEMORY.md — this section (Section 91)
- 00_ADMIN/PROJECT_STATUS.md — update log entry

**Canonical Decisions Made This Session:** 13 entries added. All marked SETTLED. Key locks: marriage player control, hybrid heir loyalty control, polygamy faith restriction, Mysticism training path, Sorcerer role, war hero promotion pipeline, early game NPC focus, anti-cheese principle, starting leader age ranges.

---

## Section 92 — Defensive Fortification Doctrine, Siege System, Fortification System (2026-04-14)

**Event Date:** 2026-04-14

**Source:** Session of 2026-04-14 under Lance W. Fisher direction. Canonical doctrine drop addressing wave-spam vulnerability in keep defense, followed by creation of full defender-side and attacker-side specifications.

**Scope:** Establishment of defensive fortification as a major canonical strategic pillar. Three new canon documents locked on the same day.

**Files Added:**
- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md` — the authoritative doctrine statement. Ten pillars spanning layering, system interaction, deep investment reward, bloodline keep resilience, anti-wave-spam, earned siege requirement, late-game defensive relevance, tradeoff acceptance, settlement class differentiation, and fortification felt-difference.
- `04_SYSTEMS/FORTIFICATION_SYSTEM.md` — defender-side specification. Six settlement classes with defensive ceilings (Border settlement, Military fort, Trade town, Regional stronghold, Primary dynastic keep, Fortress-citadel). Three-layer architecture (Outer works, Inner ring, Final defensive core). Component inventories for each layer. House-specific keep features (Ironmark blood reservoir, Old Light pyre chamber, Blood Dominion altar vault, Order scriptorium core, Wild root cellar).
- `04_SYSTEMS/SIEGE_SYSTEM.md` — attacker-side specification. Six canonical siege engine classes (Ram, Siege tower, Trebuchet, Ballista, Mantlet line, Late-game bombard). Engineer specialist population. Logistics and supply continuity. Scouting and intelligence categories. Multi-front timing, isolation, and faith-power substitution for brute force.

**Canonical Posture:**
- Fortification is a first-class strategic path, especially around primary dynastic keeps.
- Wave-spam and suicide erosion must not be a dominant answer to heavy fortification. Failed assaults cost real military waste.
- Siege is an earned military operation, not a casual click action. Line infantry alone cannot reduce an elite fortress.
- A heavily fortified bloodline seat must not behave like a normal structure with a larger health pool. It must function as the center of an integrated defensive network.
- Late-game defense must remain relevant. Offense must not scale past the viability of fortress-centered strategy.

**Related Updates Same Day:**
- `04_SYSTEMS/DYNASTIC_SYSTEM.md` — additional detail on family structure, bloodline member management, training paths, marriage and succession mechanics.
- `04_SYSTEMS/TERRITORY_SYSTEM.md` — additional detail on military occupation, loyalty requirements, consolidation mechanics.
- `01_CANON/CANONICAL_RULES.md` — entries added for defensive fortification doctrine, siege as earned operation, wave-spam suppression, settlement class differentiation.
- `01_CANON/CANON_LOCK.md` — new lock entries matching the doctrine above.
- `08_MECHANICS/MECHANICS_INDEX.md` — cross-reference updates for fortification and siege.
- `10_UNITS/UNIT_INDEX.md` — Engineer specialist class added; siege engine operation and repair clarified as engineer function.
- `11_MATCHFLOW/MATCH_STRUCTURE.md` — late-game phase acknowledges fortress-centered strategy as a viable win axis.

---

## Section 93 — Consolidation Session: Bible Expansion, Master Unified Rebuild, Deploy Sync (2026-04-14)

**Event Date:** 2026-04-14

**Source:** Consolidation session under Lance W. Fisher direction to bring the Design Bible, Master Memory, Append-Only Log, and the complete unified master into alignment with all work completed across 2026-03-26, 2026-04-07, 2026-04-12, 2026-04-13, and 2026-04-14 sessions.

**Scope:** Pre-consolidation snapshots archived. Short bible expanded from 15 sections to 24 sections. Complete unified master rebuilt to include EVERY design artifact with no exclusions. Windows Desktop copy placed. Deploy folder prepared for upload.

**Actions Taken:**

1. **Pre-consolidation snapshots archived** to `19_ARCHIVE/PRE_2026-04-14_CONSOLIDATION/`:
   - `BLOODLINES_DESIGN_BIBLE_snapshot_2026-04-14.md` (pre-expansion 289-line version)
   - `BLOODLINES_MASTER_MEMORY_snapshot_2026-04-14.md` (pre-append 2807-line version)
   - `BLOODLINES_APPEND_ONLY_LOG_snapshot_2026-04-14.md` (pre-append 1794-line version)
   - `BLOODLINES_COMPLETE_UNIFIED_v1.0_snapshot_2026-04-14.md` (pre-rebuild 15447-line 1.7MB version)

2. **Short bible expanded** (`01_CANON/BLOODLINES_DESIGN_BIBLE.md`) with nine new sections appended after the original 15 (existing sections preserved verbatim):
   - Section 16: Fortification System
   - Section 17: Siege System
   - Section 18: Naval System
   - Section 19: Diplomacy and War System
   - Section 20: Operations System (Covert, Faith, Military)
   - Section 21: Political Events System
   - Section 22: Terrain and Geography System
   - Section 23: Governance Framework and Source Layers
   - Section 24: Development State and Implementation Tracking (pointer to docs/ folder)

3. **Complete unified master rebuilt** at `18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_2026-04-14.md`. This file absorbs every canon directory, every creative branch, the archive, research, tasks, 18_EXPORTS prior versions, docs/ development state, docs/unity/ implementation planning, and all top-level working files. Nothing is excluded.

4. **Desktop copy placed** at `C:\Users\lance\Desktop\BLOODLINES_MASTER.md` for direct access outside the project tree.

5. **Viewer HTML corrected and updated** (`scripts/bloodlines_viewer.html` and `deploy/bloodlines/scripts/bloodlines_viewer.html`): malformed duplicated nav-item on the Complete Design Bible line was repaired. New nav entries added for Siege System, Fortification System, Defensive Fortification Doctrine, Canon Lock, and the new ALL v2.0 master.

6. **Deploy folder mirrored** from `bloodlines/` working root. `deploy/bloodlines/` now reflects current canon across `01_CANON/`, `04_SYSTEMS/`, `08_MECHANICS/`, `09_WORLD/`, `11_MATCHFLOW/`, `18_EXPORTS/` and the updated viewer HTML. Prepared for upload to Hostinger.

7. **Sync script updated** (`scripts/sync-to-web.sh`): SIEGE_SYSTEM.md, FORTIFICATION_SYSTEM.md, DEFENSIVE_FORTIFICATION_DOCTRINE.md, DESIGN_GUIDE.md, CANON_LOCK.md, and the new ALL v2.0 master added to the sync list.

**Canonical Posture Reaffirmed:**

The three-layer source model (Active Canon Snapshot, Historical Design Archive, Open Design Reservoir) remains in force. This consolidation session is additive in full: no prior content was deleted, summarized, or reduced. Prior unified v1.0 remains in place alongside the new v2.0 ALL master; both are available. All pre-session canon is preserved verbatim in the `PRE_2026-04-14_CONSOLIDATION` archive folder and remains retrievable.

**Files Modified:**
- `01_CANON/BLOODLINES_DESIGN_BIBLE.md` — sections 16-24 appended
- `01_CANON/BLOODLINES_MASTER_MEMORY.md` — this section and Section 92 appended
- `01_CANON/BLOODLINES_APPEND_ONLY_LOG.md` — consolidation ingestion block appended
- `18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md` — pointer note added referencing the new v2.0 master (content preserved)
- `18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_2026-04-14.md` — NEW comprehensive master file
- `scripts/bloodlines_viewer.html` — malformed nav repaired; new entries added
- `scripts/sync-to-web.sh` — new files added to sync list
- `scripts/build_complete_bible.py` — NEW build script that regenerates the v2.0 master from current canon state
- `19_ARCHIVE/PRE_2026-04-14_CONSOLIDATION/` — four snapshot files archived

**No Canonical Decisions Changed.** This session is structural and consolidative, not design-altering. Design canon from Section 91 and before remains in full force.

---

## Section 94 - Master Design Doctrine DOCX Ingestion and Continuity Hardening (2026-04-14)

**Event Date:** 2026-04-14

**Source:** Externally supplied doctrine files `D:\Lance\Downloads\Bloodlines_Master_Design_Doctrine.docx` and `D:\Lance\Downloads\Bloodlines_Master_Design_Doctrine2.docx`.

**Preservation Result:** Both source DOCX files were copied into `archive_preserved_sources/2026-04-14_downloads_bloodlines_design_doctrine_docx/`. Their full package contents were exploded into `02_SESSION_INGESTIONS/2026-04-14_design_doctrine_docx_ingestion/`. Full text extracts were preserved for both source filenames. The two DOCX files were verified byte-identical by SHA256 and preserved separately anyway because they were supplied separately.

**Canonical Files Created:**
- `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md` - full readable in-project doctrine source
- `02_SESSION_INGESTIONS/2026-04-14_design_doctrine_docx_ingestion/BLOODLINES_MASTER_DESIGN_DOCTRINE_RAW_INGESTION_APPENDIX_2026-04-14.md` - raw ingestion appendix
- `docs/BLOODLINES_CANON_CONFLICT_NOTES_2026-04-14_DOCTRINE_INGESTION.md`
- `docs/BLOODLINES_SYSTEM_CROSSWALK_2026-04-14_DOCTRINE_INGESTION.md`

**Core doctrinal reaffirmations recorded by this ingestion:**

1. Bloodlines is a grand dynastic civilizational war game. The player governs a dynasty, not merely a base.
2. Command and Conquer is a clarity and replayability inspiration, not a limit on systemic scale.
3. Graphics serve readability, house identity, atmosphere, and strategic legibility. Photorealism is not the win condition.
4. Population is foundational. People drive army, labor, infrastructure, construction, faith participation, recovery, and long-term resilience.
5. Water remains a central strategic pillar. It must stay visible, contestable, defendable, attackable, and civilizationally decisive.
6. Food, land development, forestry stewardship, and agricultural transformation remain core to long-form realm growth.
7. Supply disruption, negative upkeep, and desertion are canonical consequences. Strategic strangulation remains valid war.
8. Military doctrine remains multi-dimensional across offense, defense, fortification, commanders, delegation, and navy.
9. The already-settled declared time model must not drift, even though the incoming doctrine refers to it with "determined time" wording.
10. Continents, oceans, and real naval strategy remain part of Bloodlines' world architecture.
11. Houses remain true dynastic civilizations rather than cosmetic faction skins.
12. Faith remains covenantal and civilizational. Conviction remains behavioral morality distinct from faith.
13. Late-game divergence remains meaningful and partially irreversible.
14. Future sessions must preserve full strategic scale rather than shrinking Bloodlines toward convenience.

**Conflict note:** No major system contradiction was found. The only notable terminology drift is the incoming doctrine's "determined time system" phrasing, which maps to the already-settled `declared time model` / `dual-clock architecture` rather than superseding it with a new design.

**Integration surfaces updated in response to this ingestion:**
- short bible
- canonical rules and canon lock
- population, resource, dynastic, territory, faith, and conviction system docs
- faction, naval, UI, and audio/visual docs
- current-state, provenance, handoff, and continuity files
- versioned design-bible export and desktop copy

**Practical effect:** The design did not become smaller, simpler, or more filtered. The doctrinal floor rose. Future sessions now have a stronger anti-drift reference point for water, population, logistics, houses, commanders, fortification, world scale, and UI legibility.

---
