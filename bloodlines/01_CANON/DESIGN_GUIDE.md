# Bloodlines: Central Design Guide

**Document Purpose:** This is the central, authoritative overview of the Bloodlines game design as it currently exists. It consolidates material from the Master Memory, Design Bible, Canonical Rules, system files, faction documents, faith documents, match structure files, and session ingestions into a single navigable reference. This document does not replace any existing file. It serves as a comprehensive reading entry point and cross-reference hub for the project.

**Last Updated:** 2026-03-15
**Document Version:** 1.0
**Project Phase:** Phase 1, Design Content Population

---

## Table of Contents

1. [Project Identity and Design Purpose](#1-project-identity-and-design-purpose)
2. [High-Level Game Concept](#2-high-level-game-concept)
3. [Core Design Philosophy](#3-core-design-philosophy)
4. [The Four Foundational Pillars](#4-the-four-foundational-pillars)
5. [Core Gameplay Loop](#5-core-gameplay-loop)
6. [Match Structure and Progression Flow](#6-match-structure-and-progression-flow)
7. [The Founding Houses](#7-the-founding-houses)
8. [The Four Ancient Faiths](#8-the-four-ancient-faiths)
9. [The Conviction System](#9-the-conviction-system)
10. [Population Model](#10-population-model)
11. [Resource Systems](#11-resource-systems)
12. [Territory and Loyalty Systems](#12-territory-and-loyalty-systems)
13. [The Dynastic System and Bloodline Members](#13-the-dynastic-system-and-bloodline-members)
14. [Born of Sacrifice](#14-born-of-sacrifice)
15. [Army Structure and Military Systems](#15-army-structure-and-military-systems)
16. [Building Infrastructure](#16-building-infrastructure)
17. [Diplomatic and Dynastic Interactions](#17-diplomatic-and-dynastic-interactions)
18. [Level Progression and Irreversible Divergence](#18-level-progression-and-irreversible-divergence)
19. [Victory Conditions](#19-victory-conditions)
20. [The Trueborn City and Coalition System](#20-the-trueborn-city-and-coalition-system)
21. [World Generation and Regional Effects](#21-world-generation-and-regional-effects)
22. [Naval Warfare](#22-naval-warfare)
23. [AI Kingdoms and Minor Tribes](#23-ai-kingdoms-and-minor-tribes)
24. [Multiplayer and Long-Match Design](#24-multiplayer-and-long-match-design)
25. [How Systems Interconnect](#25-how-systems-interconnect)
26. [Canonical Vocabulary](#26-canonical-vocabulary)
27. [What Is Locked, What Is Open](#27-what-is-locked-what-is-open)
28. [Technical Architecture Direction](#28-technical-architecture-direction)
29. [Design Gaps and Future Work](#29-design-gaps-and-future-work)

---

## 1. Project Identity and Design Purpose

**Game Title:** Bloodlines (LOCKED, title may not be changed without explicit authorization)

**Title History:** The project evolved from an early title direction of "Crown and Conquest" into the locked project title "Bloodlines." The name change reflects the design's shift toward hereditary power, dynastic identity, and generational consequence as the central organizing themes.

**Genre:** Massively large-scale medieval real-time strategy game.

**Design Inspiration:** Command and Conquer style RTS, with strong emphasis on base building, offense and defense, resource gathering, strategic freedom, and broad viability across multiple playstyles. The project explicitly values the artistic and structural lesson of Command and Conquer Generals: the game does not need to be visually extravagant beyond that benchmark if balance, playability, faction balance, and experimentation value are strong.

**What Bloodlines Is Not:** Not Age of Empires building style. Not Total War combat style. Bloodlines is its own design, rooted in classic RTS clarity but layered with dynastic, ideological, and population-driven systems that go far beyond the genre's traditional scope.

**Creative Intent:** The game is intended to stand the test of time. It is built around the belief that what makes a strategy game endure is not visual excess alone, but deep and balanced systems that allow players to repeatedly return and succeed through different approaches. The project is treated as a serious long-form game design effort, not a lightweight concept sketch.

**Status Labels Used in This Document:**
- **LOCKED** = Settled canon, will not change without explicit revisitation and authorization
- **STRONGLY ESTABLISHED** = Consistent across multiple sessions, treated as near-final
- **PARTIALLY DEFINED** = Core concept exists but mechanical details are incomplete
- **OPEN DESIGN QUESTION** = Recognized gap where no decision has been made
- **PROPOSED** = Idea has been put forward but not confirmed as canon
- **HISTORICAL / ALTERNATE IDEA** = Prior version preserved per append-only rules

> Cross-reference: `01_CANON/CANONICAL_RULES.md` for the full table of settled, proposed, and open decisions.

---

## 2. High-Level Game Concept

Bloodlines is a medieval RTS where the player commands not just armies and buildings, but an entire dynastic civilization across generations. The game centers on hereditary power, dynastic identity, ideological divergence, moral trajectory, family continuity, and the social character of a civilization.

The foundational world logic includes noble lineages, ancient faiths, population loyalty, and the possible re-emergence of ancient blood-based legitimacy in the form of the Trueborn return. The world is not just a battlefield. It is a living strategic society in which war, belief, inheritance, governance, and territory all interact.

The player's role is not that of a single character. The player is the guiding authority of a ruling dynasty, shepherding a family through triumph and tragedy across the full arc of a match. Decisions compound. Mistakes leave marks. The dynasty remembers what it has done and what has been done to it. Every soldier who dies in battle is a farmer who no longer works the fields. Every sacrifice empowers the dynasty but diminishes the people.

The world contains ancient powers, long-standing bloodlines, and political factions whose identities shape the course of warfare and diplomacy. Conflict exists not only between factions but within ideologies, within families, and within the moral direction of each civilization.

> Cross-reference: Master Memory Sections 1-2, Design Bible Section 1

---

## 3. Core Design Philosophy

The design philosophy prioritizes:
- **Clarity** in presentation and strategic readability
- **Balance** across multiple viable playstyles
- **Replayability** through emergent narrative and system interaction
- **Long-term strategic depth** that sustains meaning and does not peak too early

The game is intended to feel like a medieval kingdom RTS while maintaining the core strategic readability and base-building clarity associated with classic RTS design. It supports aggressive openings, defensive expansion, economic scaling, ideological consolidation, and varied play patterns without collapsing into a single dominant strategy.

The economy is intentionally grounded and legible, favoring strong core simplicity with deep interaction rather than bloated complexity for its own sake. Every system is designed to interact with other systems in meaningful ways, creating strategic decisions that cascade across the civilization.

> Status: **LOCKED**

---

## 4. The Four Foundational Pillars

The design of Bloodlines rests on four foundational pillars that interact to shape the long-term evolution of a civilization:

1. **Territory** - The physical space a dynasty controls, the resources it grants, and the loyalty required to hold it
2. **Population** - The living resource that feeds the economy, the military, the faith, and the political body
3. **Faith** - The spiritual covenant that empowers and constrains, shaping how a civilization expresses its identity
4. **Dynasty** - The ruling bloodline whose members, marriages, succession, and legacy define the personal stakes of a strategy game

These four pillars are not independent systems. They are deeply interconnected:
- Territory provides the land for population to inhabit and resources to exploit
- Population provides the labor, soldiers, and faithful that make territory productive
- Faith shapes how the population views the dynasty and how territory is governed
- Dynasty provides the leadership that directs population, governs territory, and interprets faith

No single pillar can be neglected without consequences across the other three.

> Status: **LOCKED**
> Cross-reference: `04_SYSTEMS/SYSTEM_INDEX.md` for the seven core systems that implement these pillars

---

## 5. Core Gameplay Loop

The core gameplay loop in Bloodlines operates across multiple time horizons simultaneously:

**Immediate (tactical):** Gather resources, construct buildings, recruit and deploy armies, defend territory, attack opponents.

**Short-term (strategic):** Expand territory and manage loyalty, grow population and balance labor allocation, develop infrastructure, train bloodline members for leadership roles.

**Medium-term (civilizational):** Select and develop a faith covenant, manage conviction through governance and warfare decisions, establish trade networks, build alliances or rivalries through dynastic diplomacy.

**Long-term (generational):** Navigate succession as leaders age and die, manage the bloodline across multiple generations, pursue victory conditions that require sustained strategic commitment, experience the irreversible consequences of the civilization's moral and ideological trajectory.

At every level, the player faces the central tension: power costs people. Military expansion requires manpower drawn from the productive population. Faith requires investment of resources and participation. Born of Sacrifice demands lives. Growth requires stability and time. The player must constantly weigh short-term military needs against long-term demographic sustainability.

> Status: **STRONGLY ESTABLISHED** (broad shape defined, specific mechanical loops not yet fully detailed)

---

## 6. Match Structure and Progression Flow

Matches progress through four strategic stages, with a fifth apex tier. Two naming conventions exist in the project archive ("Level" and "Stage" terminology). Both are preserved as historical design expressions of the same structural progression.

### Stage 1 / Level 1: Founding

The player establishes their initial settlement, gathers resources, and begins population growth. The dynasty encounters tribes, ruins, and sacred sites during exploration. This is the survival and expansion phase.

**Key moment:** At the end of Stage 1, the player selects one of the four ancient faiths. This is the first major strategic commitment of the match.

### Stage 2 / Level 2: Consolidation

The kingdom expands infrastructure and military capability while stabilizing territory. Building construction, population growth, and territorial loyalty become primary concerns. The dynasty begins to develop its internal character.

### Stage 3 / Level 3: Ideological Expansion

Faith systems begin influencing society and shaping political identity. Ideology increasingly influences conflict and civilization identity. Dynastic politics and religious development intensify. The civilization's conviction becomes visible in its actions and standing.

### Stage 4 / Level 4: Irreversible Divergence

The civilization reaches its ideological apex and unlocks powerful late-game structures. This is the **major irreversible divergence point** (point of no return). Dynasties commit to strategic directions such as faith dominance, military supremacy, economic control, or governance attraction. Grand faith structures are unlocked. The late-game identity of the civilization crystallizes.

### Level 5: Apex

Grand endgame apex structure tier. The culmination of the civilization's development. Apex capabilities represent the final expression of the dynasty's strategic direction.

### Match Pacing and Escalation

- Early game focuses on expansion and survival
- Mid game introduces dynastic politics and religious development
- Late game introduces ideological wars and victory escalation
- Matches represent civilizational arcs, not just military escalation

A match is not just bigger armies over time. It is the evolution of a civilization's identity, faith, governance, and dynasty.

**Match Duration:** Optional long matches from 2 to 10+ hours. Large matches may extend well beyond ten hours in epic campaigns. The game's pacing, economy, dynasty systems, ideology systems, and population systems are designed to sustain long-term meaning and not peak too early.

> Status: Four stages **LOCKED**. Level 5 apex **LOCKED**. Detailed timing and pacing within each stage is an **OPEN DESIGN QUESTION**.
> Cross-reference: `11_MATCHFLOW/MATCH_STRUCTURE.md`

---

## 7. The Founding Houses

The central playable powers are dynasties or bloodlines. These are not simple factions but living families whose members are born, trained, promoted, captured, killed, married, and expanded through generations.

### The Nine Founding Houses

| House | Suffix | Thematic Identity |
|-------|--------|-------------------|
| **Trueborn** | -born | The oldest lineage, special neutral city role, late-game re-emergence lore |
| **Highborne** | -borne | Noble heritage, elevated bloodline |
| **Ironmark** | -mark | Martial strength, forged identity |
| **Goldgrave** | -grave | Wealth and buried power |
| **Stonehelm** | -helm | Fortification and endurance |
| **Westland** | -land | Frontier expansion, western territories |
| **Hartvale** | -vale | Heart of the valley, pastoral strength |
| **Whitehall** | -hall | Governance and institutional authority |
| **Oldcrest** | -crest | Ancient lineage, traditional power |

> Status: **LOCKED** (confirmed 2026-03-15, all nine suffixes unique)

### House Design Principles

1. Two or three syllables maximum
2. Distinct first letter shapes for UI readability
3. No repeated suffix patterns

These principles reference classic RTS faction naming (GDI, NOD, China, USA, GLA) where factions have clear visual and linguistic distinction for quick recognition during gameplay.

### House Visual Identity

All characters share a common ancestry and physical heritage. The visual difference between houses is expressed through distinct hair colors and styles per house, creating recognizable identity without dividing the world into separate ethnic groups. Dynastic cultural ancestry is reflected through hair color differences among bloodlines.

### House Role in Gameplay

The ruling noble house is the central strategic identity of the player. The player does not merely command a color-coded faction. The player guides a dynastic civilization. Noble houses represent blood, authority, continuity, specialization, political leverage, and legitimacy. Houses are structural gameplay entities that interact with population, faith, politics, diplomacy, and inheritance.

Not every match will necessarily include every house as a playable dynasty. Unused houses may appear as AI kingdoms or minor factions.

### Historical Note: House Name Variants

Multiple naming variants were explored during Sessions 1 and 3. Earlier variants included repeated suffixes ("-borne" appeared twice, "-grave" appeared twice). The design principle of unique suffixes was established and the canonical list was confirmed. Historical variants are preserved in `06_FACTIONS/FOUNDING_HOUSES.md` per append-only rules.

### Faction Asymmetry

> Status: **OPEN DESIGN QUESTION** - How houses differ mechanically beyond identity is not yet defined. This is a major design gap.

> Cross-reference: `06_FACTIONS/FOUNDING_HOUSES.md`, `06_FACTIONS/FACTION_INDEX.md`

---

## 8. The Four Ancient Faiths

The four ancient faiths predate the founding houses and shape the spiritual landscape of the game world. Faith is not a decorative or secondary layer. It is a major strategic axis.

Faith is selected after the early exploration phase (at the end of Stage 1 / Level 1) when dynasties encounter the wider world: tribes, ruins, and sacred sites. The dynasty does not create a faith. It aligns with an existing covenant that already exists in the world.

### The Old Light (Covenant of the First Flame)

A faith centered on protection, mercy, divine guardianship, and the defense of innocents. Also described as centered around enlightenment, unity, order, societal cohesion, disciplined governance, and defensive stability.

The Old Light emphasizes the preservation of life, communal welfare, and the belief that divine fire illuminates the path to righteous civilization. Followers seek to protect the weak and maintain societal order through compassionate governance.

### The Blood Dominion (The Red Covenant)

A faith centered on conquest, sacrifice, domination, and the belief that strength demonstrates divine favor. Darker in tone, this covenant carries moral cost and conviction consequences but provides strong military benefits.

The Blood Dominion holds that power is the truest measure of divine approval. Sacrifice, whether symbolic or literal, demonstrates commitment to the covenant. Followers gain military advantages but risk ideological isolation and conviction consequences.

### The Order (Covenant of the Sacred Mandate)

A faith built around law, doctrine, institutional authority, and structured governance under divine mandate. Emphasizes authority, rigid hierarchy, and administrative control.

The Order sees governance itself as a sacred act. Laws are divine instructions. Institutional structures are the physical expression of heavenly will. Followers gain administrative and governance advantages but are bound by doctrinal rigidity.

### The Wild (Covenant of the Thorned Elder Roots)

A primal faith rooted in nature mysticism, ancient spirits, and the balance of the natural world. Emphasizes adaptive survival, environmental strength, and ancient rooted power.

The Wild predates all other covenants. Its followers believe in the sentient power of the earth, forests, and waters. Strength comes from harmony with the natural world. Followers gain environmental and survival advantages but may struggle with industrialized expansion.

### Faith Mechanics: Three Interacting Components

Faith is governed by three interacting components:

1. **Covenant** - The religion the dynasty follows (one of the four above)
2. **Faith Intensity** - The degree to which the population actively practices the faith
3. **Faith Alignment** - A sliding scale representing how the covenant is practiced along a light vs dark spectrum

**Faith Intensity** is not a passive permanent buff. It is active, costly, population-scaled, and maintenance-dependent:
- Must be maintained through upkeep (costs resources)
- Scales with population participation
- Higher intensity unlocks stronger abilities
- Grows through participation, rituals, structures, and engagement
- As faith grows stronger it can radicalize, significantly affecting the dynasty's legitimacy and military recruitment

**Faith Alignment** allows the same covenant to be interpreted differently by different kingdoms. Example: A dynasty may follow the Blood Dominion but renounce human sacrifice and instead conduct symbolic or animal offerings (lighter interpretation). Another dynasty may practice brutal blood rites and sacrificial rituals (darker interpretation). Both remain within the same covenant but differ in alignment. Large alignment differences between dynasties of the same covenant can lead to internal religious conflict and schisms.

### Faith Actions

Faith actions consume resources and may also consume captured units or population:
- Rituals
- Offerings
- Temple construction
- Priesthood expansion
- Religious festivals
- Sacrificial rites

### Spiritual Manifestations (PROPOSED)

At extreme faith intensity, spiritual manifestations may appear: angels, demons, cursed armies, healing, resurrection, plague removal, nature forces. Extreme dark behavior may create world-level pressure.

> Status: Four covenants **LOCKED**. Three-component model **LOCKED**. Faith selection timing **LOCKED**. Specific mechanical implementations of each covenant's unique abilities are **OPEN DESIGN QUESTIONS**. Spiritual manifestations are **PROPOSED**.
> Cross-reference: `07_FAITHS/FOUR_ANCIENT_FAITHS.md`, `04_SYSTEMS/FAITH_SYSTEM.md`

---

## 9. The Conviction System

Conviction is a behavioral morality spectrum shaped permanently by actions. It is not a cosmetic alignment label chosen in a menu. It is produced by what the player actually does during the match.

### How Conviction Differs from Faith

This distinction is critical:
- **Faith** is chosen. **Conviction** is lived.
- **Faith** determines doctrine and worldview. **Conviction** determines what kind of civilization the player has actually become through action.
- **Faith Alignment** represents religious practice. **Conviction** represents the moral character of the dynasty's rule.
- These two systems influence each other but remain separate.

### Actions That Shape Conviction

- Warfare practices (how battles are fought, treatment of surrendered forces)
- Treatment of prisoners
- Enslavement of conquered peoples
- Protection of civilians during conflict
- Pillage and destruction of territory
- Sacrifice (of population, armies, or bloodline members)
- Governance decisions (taxation, infrastructure, welfare)

Conviction is permanent and reflects the historical character of the civilization. It evolves based on accumulated actions over the course of the match.

### What Conviction Shapes

Conviction permanently influences:
- Player abilities available
- Faith alignment interactions
- Kingdom development paths
- Late game powers
- How other dynasties and populations perceive the civilization

### Conviction Directions

Examples of where conviction can trend:
- Conquest (aggressive expansion, military dominance)
- Prosperity through trade (economic development, merchant culture)
- Faith authority (theocratic governance, religious primacy)
- Territorial governance (administrative excellence, civic development)
- Dynastic prestige (bloodline glory, heroic legacy)

### Design Tension

Conviction creates meaningful tension between what a player wants to do and what their people are willing to support. High conviction enables extraordinary actions (including Born of Sacrifice). Low conviction leads to desertion, unrest, and collapse from within.

> Status: Core concept **LOCKED**. Distinction from Faith **LOCKED**. Specific mechanical triggers, thresholds, visibility to player, measurable scoring system, and detailed interaction with other systems are **OPEN DESIGN QUESTIONS**.
> Cross-reference: `04_SYSTEMS/CONVICTION_SYSTEM.md`, INPUT_WORKBOOK.md Section 5

---

## 10. Population Model

Population exists as a unified pool representing the kingdom's civilians. It is a single pooled realm-wide value, not tracked per-territory (though population loyalty per territory does matter).

### Population as Living Resource

Population is not just a cap. It is a living strategic resource that supplies the labor, military, civic, and ideological body of the civilization. Population can be converted into:
- Workers (economic production)
- Soldiers (military recruitment)
- Specialists (governance, infrastructure)
- Religious participants (faith intensity)

Every conversion has a cost. Every soldier recruited is a laborer lost. This creates the central tension: power costs people.

### Population Growth

Growth depends primarily on food and water availability. Adequate housing determines maximum population capacity. Housing replaces the power grid mechanics common in many RTS games (replacing Command and Conquer style power plant concepts). Instead of generating electrical grid power, the player expands social and logistical capacity through housing, which in turn raises population capacity and supports social stability.

### Population Determines

- Workforce availability
- Tax revenue (via gold generation)
- Recruitment capacity (military manpower)
- Territorial loyalty (political stability)

### Population Decline

Population decline can occur due to:
- Famine (food shortage)
- Warfare (soldiers killed, civilians caught in conflict)
- Plagues
- Religious conflict

### Population Loyalty and Morale

Population loyalty affects productivity. Low loyalty reduces tax income and increases risk of sabotage and rebellion. Morale is influenced by how the dynasty governs and how it treats soldiers and citizens. Reckless warfare that burns through soldiers without support lowers morale and can cause mutinies or defections.

Population is also central to territory consolidation. A territory is not truly controlled by military occupation alone. Population acceptance matters. Population is therefore not just a labor pool but also a political body.

> Status: Unified pool model **LOCKED**. Housing as power replacement **LOCKED**. Detailed growth rates, conversion mechanics, and per-territory population tracking are **OPEN DESIGN QUESTIONS**.
> Cross-reference: `04_SYSTEMS/POPULATION_SYSTEM.md`

---

## 11. Resource Systems

Five primary resources drive the economy, plus one secondary resource.

### Primary Resources

**Gold** - Currency, taxation, economic power. Used for building construction, troop recruitment, diplomacy, trade, and ransom. Generated through taxation, markets, and trade networks.

**Food** - Supports population growth and sustains armies. Shortages cause famine and declining loyalty. Produced through farms and ranches.

**Water** - Fresh water supply and irrigation. Essential for population survival and agricultural production. Elevated to a primary resource rather than a secondary terrain effect. This is a major point of distinction in the design. Water is essential but not designed as a trivial hard-counter win condition. Denying water is impactful but should not trivially end a game.

**Wood** - Construction material, fuel, and equipment crafting. Used for basic building construction, siege equipment, ships, and fuel for smelting and heating. Produced through lumber camps in forested territories. Renewable but location-dependent.

**Stone** - Construction and fortification. Used for walls, towers, and major structures. Stone deposits are finite strategic resources.

### Secondary Resource

**Influence** - Proposed secondary resource beyond the five primaries.

> Status: Five primary resources **LOCKED**. Influence as secondary **PROPOSED**. Specific gathering rates, trade mechanics, and resource interaction formulas are **OPEN DESIGN QUESTIONS**.

### Design Philosophy

The economy is intentionally grounded and legible. Strong core simplicity with deep interaction rather than bloated complexity for its own sake. Resource control is tied directly to territorial control. Resource nodes across the map create natural conflict points between kingdoms.

### Water Design Considerations

The fish-diet vs plains-hunting tradeoff introduces regional variation: coastal civilizations may have food access through fishing but potentially lower military strength or HP. This creates meaningful geographic identity. Sea-faring rogue or guerrilla possibilities exist for coastal dynasties, along with potentially weaker siege or open-field performance.

### Late-Game Economic Concept: Currency Dominance (PROPOSED)

A dynasty pursuing economic dominance can introduce its own currency into the global economy. Through successful trade networks and economic influence, the world can transition away from gold and onto the dynasty's currency standard. Economic victory occurs when the world economy becomes dependent on that dynasty's currency.

> Cross-reference: `04_SYSTEMS/RESOURCE_SYSTEM.md`

---

## 12. Territory and Loyalty Systems

Territory control in Bloodlines requires both military control and population loyalty. This is a defining design departure from many RTS games where a structure or outpost marker is enough to claim land.

### Two-Part Control System

1. **Military Control** - Physical occupation and defense of territory
2. **Population Loyalty** - Acceptance of the dynasty's rule by the local population

A region held only by force is unstable. It may revolt. A region becomes truly controlled only when the population accepts the player's rule.

### Territory Composition

The map is divided into territories. Each territory contains:
- Resources (determined by geography and development)
- Population (local inhabitants)
- Infrastructure (buildings and improvements)
- Loyalty level (degree of acceptance of the ruling dynasty)

### Consequences of Low Loyalty

If loyalty declines:
- Tax revenue decreases
- Recruitment becomes difficult
- Sabotage risk increases
- Rebellions may occur

### Control Factors

Control of territory depends on:
- Military presence
- Local loyalty
- Infrastructure development
- Faith influence

### Loyalty Maintenance Methods

- Governance structures (administrative buildings and policies)
- Faith influence (religious structures and shared covenant)
- Economic prosperity (employment, trade, adequate food and water)
- Dynasty reputation (how the ruling house is perceived)

### Voluntary Integration (PROPOSED)

In late-game territorial governance paths, smaller territories may choose to join the dynasty willingly due to the prosperity and infrastructure of the dynasty. This represents governance attraction rather than military conquest. Territory control is therefore not only achieved through force but also through soft power.

Territory involves questions of occupation, legitimacy, stability, loyalty, and governance. Expansion without consolidation can create internal weakness.

> Status: Two-part control model **LOCKED**. Voluntary integration **PROPOSED**. Map structure (hex, province, continuous), specific loyalty mechanics, and territory influence zones are **OPEN DESIGN QUESTIONS**.
> Cross-reference: `04_SYSTEMS/TERRITORY_SYSTEM.md`

---

## 13. The Dynastic System and Bloodline Members

The dynastic system is the heart of Bloodlines and one of the four foundational design pillars. It is the source of the game's title and its emotional core.

### Bloodline Members

Bloodline members are not ordinary military units. They are family members within the ruling dynasty.

- **Active cap:** 20 members maximum at any time
- **Dormancy:** Beyond 20, members become dormant until activated (dormancy is a mechanical state, not permanent removal)
- **Birth:** Members are born into the dynasty over time
- **Specialization:** Paths are chosen at birth and training begins in childhood (not chosen upon reaching adulthood)

### Training Paths

- Military command
- Governance
- Religious leadership
- Diplomacy
- Covert operations
- Economic stewardship

### Active Roles

Bloodline members become active leaders upon maturity and may serve as:
- Commanders (lead armies, influence battlefield morale)
- Governors (manage territories)
- Diplomats (conduct negotiations, arrange marriages)
- Ideological leaders (advance faith, shape conviction)

### War Heroes and Lesser Houses

Exceptional commanders who win extraordinary battles may become celebrated war heroes. These war heroes may be granted titles and lands, creating **lesser houses** under the main dynasty. Lesser houses remain loyal to the main bloodline but become their own branches within the dynastic family tree.

Lesser houses are optional AI-autonomous or player-controlled. They may act independently as AI entities or be directly controlled by a player, implying subordination, vassal-like relationships, or secondary command structures.

### Family Tree Interface

The family tree is not a decorative feature. It is a critical gameplay-accessible system. The player must always understand:
- Who is head of the family
- Who the heirs are
- Who belongs to the dynasty
- What each member specializes in
- What their heritage is
- How they are loved or hated by the population

### Starting Leader Options

- Father
- First oldest son
- Second oldest son
- Brother of the king
- Firstborn son of the king

> Status: 20-member cap **LOCKED**. Birth specialization **LOCKED**. Training paths **STRONGLY ESTABLISHED**. Detailed trait inheritance, succession rules, lifespan model, and dormancy activation criteria are **OPEN DESIGN QUESTIONS**.
> Cross-reference: `04_SYSTEMS/DYNASTIC_SYSTEM.md`

---

## 14. Born of Sacrifice

Born of Sacrifice is the system through which extraordinary power is created at extraordinary cost. This is the system that gives Bloodlines its moral weight.

### The Mechanic

Players may sacrifice multiple armies to create a powerful elite force. Example ratio: five standard armies sacrificed to produce one elite force.

### Elite Army Benefits

- Siege capability
- Enhanced armor
- Extreme morale

### Identity and Naming

Units created through Born of Sacrifice can be named by the player or assigned names through a random generator. These units are rare, extremely powerful, and often become legendary figures within a match.

### Design Intent

Born of Sacrifice is not just a stat upgrade. It creates identity-rich, memorable battlefield units tied to the ideological and dynastic character of the civilization. The mechanic forces choices that matter and cannot be undone. Players who rely heavily on sacrifice will field terrifying armies but rule over hollowed-out populations.

Sacrifice is not free. It costs people. It may cost your own children. The power gained is real and significant, but the cost is permanent and visible.

> Status: Core mechanic **LOCKED**. Player naming **LOCKED**. Specific sacrifice ratios, faith-specific variations, cooldowns, limits, and diplomatic consequences of sacrifice are **OPEN DESIGN QUESTIONS**.
> Cross-reference: `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md`

---

## 15. Army Structure and Military Systems

Armies are composed of units drawn from the population and trained through military infrastructure. Armies require food supply, gold payment, leadership, and morale support.

### Level 1 Units (LOCKED)

Level 1 represents the earliest military stage. Armies are small and drawn from the population.

| Unit | Role | Key Traits |
|------|------|------------|
| **Militia** | Emergency defense | Very cheap, weak combat, low morale, drawn directly from population |
| **Swordsmen** | Frontline infantry + reconnaissance | Moderate attack/defense, faster movement, increased sight, replaces scouts |
| **Spearmen** | Defensive infantry | Higher defense, slower movement, holds the line, strong vs cavalry |
| **Hunters** | Harassment and ambush | Light ranged, drawn from population, weak in melee |
| **Bowmen** *(canonical name: Archers — updated 2026-04-25, Seventeenth Session Canon)* | Ranged support | Upgraded from hunters, specializes as Offensive (higher damage) or Defensive (increased range/accuracy), weak in close combat |

**Design Decision:** Scouts are intentionally removed as a unit type. Swordsmen serve as both frontline infantry and reconnaissance units.

### Levels 2-5 Units

> Status: **NOT DEFINED**. Units beyond Level 1 are a confirmed design gap.

### Recruitment System

Army recruitment is not a hard-locked doctrine but controlled through **adjustable sliders**. Dynasties can mix recruitment approaches:
- Family obligations
- Paid soldiers
- Faith volunteers

**Sons/Daughters Tradeoff (PROPOSED):** Including daughters in the army may produce a negative attack buffer but a positive healing and sustainment buffer. This creates a meaningful strategic tradeoff in recruitment decisions.

### Army Leadership

Armies can be led by bloodline commanders or appointed generals. Leadership provides battlefield advantages. Bloodline commanders may influence battlefield morale (future design idea).

### Typical Level 1 Army Composition

- Militia providing numbers
- Swordsmen providing mobility and scouting
- Spearmen holding defensive formations
- Hunters or Bowmen providing ranged support

> Status: Level 1 units **LOCKED**. Recruitment sliders **LOCKED**. Levels 2-5, combat system, unit classes, and detailed army mechanics are **OPEN DESIGN QUESTIONS**.
> Cross-reference: `10_UNITS/UNIT_INDEX.md`

---

## 16. Building Infrastructure

Buildings support the development of the dynasty, organized into five categories.

### Civic Buildings (LOCKED)

| Building | Function |
|----------|----------|
| **Settlement Hall** | Establishes control of territory, collects taxes |
| **Housing District** | Expands population capacity |
| **Well** | Provides water supply |
| **Granary** | Stores food reserves |

### Economic Buildings (LOCKED)

| Building | Function |
|----------|----------|
| **Farm** | Produces food through agriculture |
| **Ranch** | Produces livestock food |
| **Market** | Generates gold through trade |
| **Lumber Camp** | Produces wood from forested territories |
| **Quarry** | Produces stone |

### Military Buildings (LOCKED)

| Building | Function |
|----------|----------|
| **Barracks** | Trains infantry |
| **Training Grounds** | Improves soldier training |
| **Armory** | Provides equipment upgrades |
| **Fortified Keep** | Major defensive stronghold |
| **Watchtower** | Provides early warning of enemy approach |

### Faith Buildings (LOCKED)

| Building | Function |
|----------|----------|
| **Shrine** | Small religious structure, increases faith intensity |
| **Temple** | Major religious structure, enables powerful rituals |
| **Grand Sanctuary** | Late-game structure tied to faith victory |

### Special Buildings (LOCKED)

| Building | Function |
|----------|----------|
| **Dynasty Estate** | Residence of bloodline members |
| **Academy** | Trains bloodline leaders |
| **Treasury** | Improves financial control |

**Total:** 19 buildings across 5 categories.

> Status: Building categories and names **LOCKED**. Technology tree, upgrade paths, build costs, build times, and prerequisites are **OPEN DESIGN QUESTIONS**.

---

## 17. Diplomatic and Dynastic Interactions

The dynastic system enables a rich set of inter-dynasty interactions that go beyond simple alliances and wars.

### Captured Bloodline Members

Dynasties can capture members of rival bloodlines. Captured members can be handled in multiple ways:
- **Enslavement** as workers
- **Execution** (permanent removal)
- **Ransom** (returned for gold or political concessions)
- **Marriage** into the dynasty (if the bloodline practices polygamy)

How a dynasty handles captured bloodline members over time influences political dynamics and conviction.

### Marriage Diplomacy and Hybrid Heirs

Cross-dynasty marriages can create new dynastic branches. If a dynasty repeatedly takes wives from rival bloodlines and fathers children, those children may:
- Declare loyalty to the father's bloodline
- Declare loyalty to the mother's bloodline
- Form a new dynasty entirely

This creates emergent political complexity and is a major source of strategic depth.

### Covert Operations and Assassination

Assassination missions and covert warfare mechanics exist as part of dynastic gameplay. Covert operations is one of the training paths for bloodline members.

> Status: Capture/ransom/marriage mechanics **STRONGLY ESTABLISHED**. Assassination mechanics **PARTIALLY DEFINED**. Specific diplomatic interface, treaty types, alliance structures, and diplomatic AI behavior are **OPEN DESIGN QUESTIONS**.

---

## 18. Level Progression and Irreversible Divergence

Bloodlines uses a five-level progression system representing advancement in civilization and military development.

| Level | Name | Key Events |
|-------|------|------------|
| **Level 1** | Founding | Settlement, exploration, resource gathering. Faith selected at end. |
| **Level 2** | Consolidation | Infrastructure expansion, territory stabilization |
| **Level 3** | Ideological Expansion | Faith begins shaping society, conviction becomes visible |
| **Level 4** | Irreversible Divergence | **Point of no return.** Grand faith structures unlocked. Dynasties commit to strategic directions. |
| **Level 5** | Apex | Grand endgame structures. Apex capabilities. Final civilizational expression. |

### Specialization and Lockouts

Specialization lockouts exist at various levels. As the civilization progresses, certain paths close and others open. These lockouts are intentional design features that create strategic commitment and replayability.

### Grand Faith Structures

Grand faith structures are powerful constructions unlocked in the late game. They represent the culmination of faith alignment and are tied to late-game ideological culmination. They likely represent each civilization's final spiritual, political, or metaphysical institutional expression.

> Status: Five-level system **LOCKED**. Level 4 divergence **LOCKED**. Level 5 apex **LOCKED**. Specific structures, lockout details, and progression triggers are **OPEN DESIGN QUESTIONS**.

---

## 19. Victory Conditions

Multiple victory paths exist, reflecting the design's commitment to strategic diversity.

### Military Conquest (LOCKED)

Traditional military domination, but with a critical difference: conquest carries **instability risk**. Reckless warfare that burns through soldiers without support lowers morale and can cause mutinies or defections. Military conquest is not simply "destroy everything." It requires managing the human cost of war.

### Economic / Currency Dominance (PROPOSED)

A dynasty introduces its own currency into the global economy. Through successful trade networks and economic influence, the world transitions away from gold and onto the dynasty's currency standard. Victory occurs when the world economy becomes dependent on that dynasty's currency.

### Faith Victory / Divine Right (PROPOSED)

Faith-based victory through religious influence and dominance. Not yet defined in full mechanical detail.

### Territorial Governance Attraction (PROPOSED)

Smaller territories choose to join the dynasty willingly due to prosperity and infrastructure. Victory through attraction and voluntary integration rather than conquest.

### Dynastic Prestige (PROPOSED)

Prestige as a victory path involving bloodline members on the battlefield in prestige dispute wars. Specific mechanical rules not yet defined.

### Rejected Victory Path

**Population loyalty as standalone victory path:** Not favored. Population loyalty is critical to all victory paths but is not sufficient on its own.

> Status: Military conquest **LOCKED** (concept). Four additional paths **PROPOSED**. Detailed mechanical implementations for all victory conditions are **OPEN DESIGN QUESTIONS**.
> Cross-reference: `11_MATCHFLOW/MATCH_STRUCTURE.md`

---

## 20. The Trueborn City and Coalition System

At the center of the world exists a neutral zone city associated with the Trueborn banner. The Trueborn house is the oldest lineage and plays a special role.

### The Neutral City

The Trueborn city serves as a stabilizing power within the world and functions as a central trade hub. Throughout the game, dynasties may contribute to the city through trade, diplomacy, construction, or protection. These contributions affect how the city perceives each dynasty.

### Coalition Response System (LOCKED)

If the Trueborn city remains unconquered by the late game, it can choose to intervene when one dynasty begins approaching victory conditions too quickly. The city raises the Trueborn banner and recruits other dynasties into a coalition against the emerging hegemon.

This system is designed to prevent early rush victories or dynasties "cheesing" their way to a fast win. Contribution history to the Trueborn city matters. How the city perceives each dynasty affects whether it will join a coalition against or alongside them.

> Status: Trueborn city concept **LOCKED**. Coalition response **LOCKED**. Specific contribution tracking mechanics and coalition formation rules are **OPEN DESIGN QUESTIONS**.

---

## 21. World Generation and Regional Effects

- Random world generation produces regional cultures that shape economy and military identity
- The world contains ancient powers, long-standing bloodlines, and political factions
- The world supports conflict not only between factions but within ideologies, within families, and within the moral direction of each civilization

> Status: Random generation concept **LOCKED**. Map structure, terrain types, regional culture mechanics, and specific world generation rules are **OPEN DESIGN QUESTIONS**.

---

## 22. Naval Warfare

Naval warfare exists in the game world:
- Coastal trade and fishing orientation
- Fish-diet vs plains-hunting tradeoff including lower strength or HP possibility for fishing civilizations
- Sea-faring rogue / guerrilla possibilities and weaker siege or open-field performance for coastal dynasties

> Status: Naval warfare existence **LOCKED**. Specific naval unit types, ship classes, coastal mechanics, and naval combat rules are **OPEN DESIGN QUESTIONS**.

---

## 23. AI Kingdoms and Minor Tribes

### AI Kingdoms

AI kingdoms are active kingdom-level participants, not mere placeholders:
- Occupy the world alongside players and minor entities
- Participate in the living political landscape
- Operate using the same core systems as player kingdoms
- Control territory, develop infrastructure, raise armies
- Can form alliances, join Trueborn coalitions, react to rising hegemons

AI behavior may vary depending on faction personality and strategic tendencies.

### Minor Tribes

Minor tribes serve as world elements during exploration and early expansion. They are not full kingdoms but populate the world and create interaction opportunities.

### Lesser Houses (Player-Created)

Lesser houses created through titles and land grants are optional AI-autonomous or player-controlled. This dual nature is a distinctive design feature.

> Status: AI kingdoms as active participants **STRONGLY ESTABLISHED**. Specific AI behavior models, doctrine systems, diplomacy logic, faith prioritization logic, and economy styles are **OPEN DESIGN QUESTIONS**.

---

## 24. Multiplayer and Long-Match Design

- Up to 10 players with AI kingdoms and minor tribes
- Different strategic playstyles remain viable
- The world functions as a living political landscape
- Long match mode supports extended strategic campaigns

Detailed network rules, lobby systems, team structures, diplomacy toggles, and multiplayer matchmaking rules have not yet been fully defined.

> Status: Player count and long match support **LOCKED**. Multiplayer infrastructure details are **OPEN DESIGN QUESTIONS**.

---

## 25. How Systems Interconnect

This is arguably the most important section of the guide, because Bloodlines is defined not by individual systems but by how they interact.

### Population feeds everything

Population is the base resource that all other systems draw from:
- **Economy:** Population provides workers who produce resources
- **Military:** Population provides soldiers who fight wars (and dying soldiers reduce the productive population)
- **Faith:** Population provides the faithful whose participation drives faith intensity
- **Territory:** Population loyalty determines whether territory is truly controlled
- **Dynasty:** Population perception of the ruling house affects stability and recruitment

### Faith shapes population behavior

- Faith intensity affects population loyalty and morale
- Faith alignment affects how the population views dynastic actions
- Religious conflict can cause population decline
- Faith actions consume resources and sometimes population

### Conviction emerges from player actions and constrains future options

- Warfare practices shape conviction
- Conviction shapes available abilities and development paths
- Conviction interacts with faith alignment (a merciful dynasty following the Blood Dominion creates internal tension)
- Conviction affects how other dynasties and populations perceive the civilization

### Territory connects resources to population to power

- Territory provides the physical space for population and buildings
- Resource nodes within territory create economic foundations
- Territory loyalty depends on population acceptance
- Expansion without consolidation creates vulnerability

### Dynasty provides the human element that makes everything personal

- Bloodline commanders lead armies (military system)
- Bloodline governors manage territories (territory system)
- Bloodline religious leaders advance faith (faith system)
- Succession crises can destabilize all other systems
- Captured bloodline members create diplomatic leverage
- Marriage alliances create new strategic possibilities

### Housing mediates population growth

- Housing determines population capacity (replacing traditional power mechanics)
- More housing enables more population
- More population enables more of everything else
- Inadequate housing constrains all downstream systems

### Born of Sacrifice consumes systems to produce extraordinary power

- Consumes armies (population already converted to soldiers)
- Creates elite units with extreme capability
- The cost is permanent and visible
- Heavy use hollows out the population base that feeds everything else

---

## 26. Canonical Vocabulary

These three terms are distinct and must not be confused or merged:

| Term | Meaning | Nature |
|------|---------|--------|
| **Bloodline** | The family (the ruling dynasty) | Structural, generational |
| **Conviction** | The philosophy (governing philosophy shaped by actions) | Behavioral, emergent |
| **Faith** | The religion (covenant alignment) | Chosen, maintained |

> Status: **LOCKED**

---

## 27. What Is Locked, What Is Open

### Summary of Locked Decisions

The following elements are settled canon as of 2026-03-15:

- Game title: Bloodlines
- Genre: Massively large-scale medieval RTS
- Design inspiration: Command and Conquer style
- Four foundational pillars: Territory, Population, Faith, Dynasty
- Seven core systems: Conviction, Faith, Population, Resource, Territory, Dynastic, Born of Sacrifice
- Nine founding houses with unique suffixes
- Four ancient faiths (covenants)
- Faith three-component model (Covenant, Intensity, Alignment)
- Faith selection at end of Stage 1
- Five-level progression with Level 4 irreversible divergence
- Four match stages
- Population as unified pool
- Housing replacing power mechanics
- Territory requiring military control AND loyalty
- Five primary resources: gold, food, water, wood, stone
- 20 active bloodline member cap
- Birth specialization for bloodline members
- Recruitment via adjustable sliders
- Scouts removed (swordsmen serve as recon)
- Level 1 units: Militia, Swordsmen, Spearmen, Hunters, Bowmen
- Five building categories with 19 named buildings
- Trueborn neutral city and coalition response system
- Military conquest with instability risk
- Born of Sacrifice as elite army-forging mechanic
- Naval warfare exists
- Random world generation
- Match scale: 2-10+ hours, up to 10 players

### Summary of Proposed Elements

- Influence as secondary resource
- Currency dominance victory
- Territorial governance attraction victory
- Dynastic prestige dispute wars
- Sons/daughters recruitment tradeoff
- Spiritual manifestations at extreme faith intensity

### Major Open Design Questions

See `docs/OPEN_QUESTIONS.md` for the full catalog. Key gaps include:
- Faction asymmetry model
- Units beyond Level 1
- Technology tree
- Grand faith structures per covenant
- AI kingdom behavior
- Combat system
- Diplomacy interface and rules
- Map structure
- Succession rules
- Character trait system
- Victory condition mechanical details
- UI/UX design
- Audio/visual direction
- World history and lore timeline

> Cross-reference: `01_CANON/CANONICAL_RULES.md`, `docs/OPEN_QUESTIONS.md`

---

## 28. Technical Architecture Direction

A deterministic RTS architecture bootstrap prompt was generated during Session 2 with the following technical goals:

- Clean separation between simulation and presentation
- Deterministic fixed-tick simulation loop (target 20 TPS)
- Seeded RNG module with deterministic consumption ordering
- Command buffer + command resolver pipeline for simulation mutations
- Simple event bus for simulation events
- Snapshot system for save/load and replay compatibility, schema versioned
- Per-tick deterministic hash function for desync detection
- Minimal "headless sim" harness for testing

**Non-negotiables for the simulation layer:**
- No Math.random in sim
- No Date.now in sim
- No floats in sim state
- Stable iteration order everywhere
- Commands applied in stable deterministic order
- Seeded RNG consumption order must not depend on object key ordering

**Target file structure:** `/src/sim` with subfolders for core, ecs, commands, events, snapshot, and domains. Presentation layer reads snapshots only.

> Status: Architecture direction **STRONGLY ESTABLISHED**. No prototype code has been written yet.
> Cross-reference: Master Memory Section 48

---

## 29. Design Gaps and Future Work

The following areas are confirmed as not yet fully designed. These represent the frontier of design work that must be addressed before prototyping can begin in earnest.

**High Priority (blocks fundamental gameplay):**
- Unit classes and combat system beyond Level 1
- Building and technology tree progression
- Map control mechanics and territory structure
- Detailed victory condition mechanics

**Medium Priority (enriches core systems):**
- Grand faith structures and named structures per covenant
- Dynasty character traits and inheritance system
- Faith manifestation mechanics
- Detailed conviction mechanics (triggers, thresholds, visibility)
- Faction asymmetry (how houses feel mechanically different)

**Lower Priority (can be defined later):**
- AI kingdom behavior models and doctrine systems
- Campaign progression structure
- Diplomacy rules and interface details
- Resource gathering implementation rates
- Multiplayer lobby and ruleset detail
- Prestige dispute war rules
- Currency dominance mechanical implementation
- World history and timeline lore
- UI/UX design
- Audio/visual direction

> Cross-reference: `docs/NEXT_STEPS.md`, `docs/OPEN_QUESTIONS.md`, `docs/INPUT_WORKBOOK.md`

---

## Document Navigation

| Document | Purpose | Location |
|----------|---------|----------|
| This Guide | Central design overview | `docs/USER_GUIDE.md` |
| Current Game State | Status checkpoint | `docs/CURRENT_GAME_STATE.md` |
| Next Steps | Prioritized design work | `docs/NEXT_STEPS.md` |
| Open Questions | Full catalog of unresolved areas | `docs/OPEN_QUESTIONS.md` |
| Input Workbook | Structured questions for design input | `docs/INPUT_WORKBOOK.md` |
| Input Inbox | Raw input for future processing | `docs/INPUT_TO_APPLY.md` |
| Change Log | Consolidation work record | `docs/CHANGE_LOG.md` |
| Design Bible | Structured design reference | `01_CANON/BLOODLINES_DESIGN_BIBLE.md` |
| Master Memory | Full cumulative design memory | `01_CANON/BLOODLINES_MASTER_MEMORY.md` |
| Canonical Rules | Settled vs open decision tracker | `01_CANON/CANONICAL_RULES.md` |

---

*This document is additive. New sections are appended as the design expands. Existing content is never removed, only augmented with updated status labels when decisions are made.*
