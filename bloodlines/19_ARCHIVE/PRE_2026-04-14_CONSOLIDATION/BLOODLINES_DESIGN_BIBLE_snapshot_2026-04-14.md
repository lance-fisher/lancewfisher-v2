# Bloodlines — Design Bible

The authoritative game design document for Bloodlines. This document is structured by design domain. Each section grows over time as design decisions are made and refined. Content is appended, never replaced. See `CANONICAL_RULES.md` for what is considered settled versus open.

---

## 1. World and Lore Foundations

*The setting, history, and narrative context of the Bloodlines universe.*

Bloodlines is a medieval real-time strategy game designed in the spirit of older Command & Conquer style RTS games, with strong emphasis on base building, offense and defense, resource gathering, strategic freedom, and broad viability across multiple playstyles. The game evolved from an early title direction of Crown and Conquest into the locked project title Bloodlines.

The foundational world logic includes noble lineages, ancient faiths, population loyalty, and the possible re-emergence of ancient blood-based legitimacy in the form of the Trueborn return. The world is not just a battlefield. It is a living strategic society in which war, belief, inheritance, governance, and territory all interact.

The world contains ancient powers, long-standing bloodlines, and political factions whose identities shape the course of warfare and diplomacy. The game's narrative world includes a history of ancient civilizations, powerful faiths that shaped the world's early structure, and the later emergence of dominant dynastic houses that now control territories across the world.

The world supports conflict not only between factions but within ideologies, within families, and within the moral direction of each civilization. At the center of the world exists a neutral zone city associated with the Trueborn banner, serving as a stabilizing power.

Random world generation produces regional cultures that shape economy and military identity. Naval warfare exists with coastal trade, fishing orientation, and sea-faring possibilities.

See also: `05_LORE/WORLD_HISTORY.md`, `05_LORE/TIMELINE.md`

---

## 2. Factions and Great Houses

*The playable factions, their founding houses, political structures, and defining characteristics.*

The central playable powers are dynasties or bloodlines. These are not simple factions but living families whose members are born, trained, promoted, captured, killed, married, and expanded through generations.

Nine founding Houses exist with a late-game Trueborn re-emergence lore component.

**Canonical List (SETTLED):** Trueborn, Highborne, Ironmark, Goldgrave, Stonehelm, Westland, Hartvale, Whitehall, Oldcrest

**Design Principles for House Naming:** Two or three syllables maximum, distinct first letter shapes for UI readability, no repeated suffix patterns. All nine suffixes are unique: -born, -borne, -mark, -grave, -helm, -land, -vale, -hall, -crest.

Visual identity is expressed through distinct hair colors and styles per house, creating recognizable identity without dividing the world into separate ethnic groups.

The ruling noble house is the central strategic identity of the player. Houses are structural gameplay entities that interact with the population, faith, politics, diplomacy, and inheritance.

See also: `06_FACTIONS/FACTION_INDEX.md`, `06_FACTIONS/FOUNDING_HOUSES.md`

---

## 3. The Four Ancient Faiths

*The spiritual forces that shape the world, their tenets, and their mechanical impact on gameplay.*

The four ancient faiths predate the founding houses and shape the spiritual landscape of the game world. Faith is selected after the early exploration phase (end of Level 1 / Stage 1) when dynasties encounter tribes, ruins, and sacred sites. The dynasty does not create a faith; it aligns with an existing covenant.

**The Old Light (Covenant of the First Flame)** — Protection, mercy, divine guardianship, defense of innocents, enlightenment, unity, societal cohesion, disciplined governance, defensive stability.

**The Blood Dominion (The Red Covenant)** — Conquest, sacrifice, domination, belief that strength demonstrates divine favor. Darker in tone, carries moral cost and conviction consequences, provides strong military benefits.

**The Order (Covenant of the Sacred Mandate)** — Law, doctrine, institutional authority, structured governance under divine mandate, rigid hierarchy, administrative control.

**The Wild (Covenant of the Thorned Elder Roots)** — Nature mysticism, ancient spirits, balance of the natural world, adaptive survival, environmental strength, ancient rooted power.

Faith is governed by three interacting components: Covenant (the religion), Faith Intensity (degree of active practice), and Faith Alignment (light vs dark spectrum). Large alignment differences between dynasties of the same covenant can lead to internal religious conflict and schisms.

See also: `07_FAITHS/FAITH_INDEX.md`, `07_FAITHS/FOUR_ANCIENT_FAITHS.md`

---

## 4. Conviction System

*How belief, morale, and ideological commitment function as a game mechanic.*

Conviction is a behavioral morality spectrum shaped permanently by actions. It is not a cosmetic alignment label chosen in a menu. It is produced by what the player actually does during the match.

Conviction is distinct from Faith. Faith is chosen. Conviction is lived. Faith determines doctrine and worldview. Conviction determines what kind of civilization the player has actually become through action.

Actions influencing conviction include warfare practices, treatment of prisoners, enslavement, protection of civilians, pillage and destruction, sacrifice, and governance decisions.

Conviction permanently shapes player abilities, faith alignment interactions, kingdom development, and late game powers. Examples of conviction directions include conquest, prosperity through trade, faith authority, territorial governance, and dynastic prestige.

Conviction represents the moral character of the dynasty's rule. Faith Alignment represents religious practice. These two systems influence each other but remain separate.

See also: `04_SYSTEMS/CONVICTION_SYSTEM.md`

---

## 5. Population System

*How population grows, migrates, is consumed by war, and drives economic and military capacity.*

Population exists as a unified pool representing the kingdom's civilians. It is a single pooled realm-wide value. Population growth depends primarily on food and water availability.

Population is not just a cap. It is a living strategic resource that supplies the labor, military, civic, and ideological body of the civilization. Population can be converted into workers, soldiers, specialists, or religious participants.

Population determines workforce availability, tax revenue, recruitment capacity, and territorial loyalty. Population decline can occur due to famine, warfare, plagues, and religious conflict.

Population loyalty affects productivity. Low loyalty reduces tax income and increases the risk of sabotage and rebellion. Population morale and loyalty are influenced by how the dynasty governs and how it treats soldiers and citizens. Reckless warfare lowers morale and can cause mutinies or defections.

Population is central to territory consolidation because a territory is not truly controlled by military occupation alone.

See also: `04_SYSTEMS/POPULATION_SYSTEM.md`

---

## 6. Resource Economy

*The resources that fuel faction growth, the systems of production and trade, and the economic levers available to players.*

Five primary resources drive the economy:

**Gold** — Currency, taxation, economic power. Used for building construction, troop recruitment, diplomacy, trade, and ransom. Generated through taxation, markets, and trade networks.

**Food** — Supports population growth and sustains armies. Shortages cause famine and declining loyalty. Produced through farms and ranches.

**Water** — Fresh water supply and irrigation. Essential for population survival and agricultural production. Elevated to a primary resource rather than a secondary terrain effect.

**Wood** — Construction material, fuel, and equipment crafting. Used for basic building construction, siege equipment, ships, and fuel. Produced through lumber camps in forested territories. Renewable but location-dependent.

**Stone** — Construction and fortification. Used for walls, towers, and major structures. Deposits are finite strategic resources.

**Secondary Resource:** Influence

**Late-Game Economic Concept:** Currency dominance victory. A dynasty can introduce its own currency into the global economy. Economic victory occurs when the world economy becomes dependent on that dynasty's currency.

See also: `04_SYSTEMS/RESOURCE_SYSTEM.md`

---

## 7. Territory Control

*How territory is claimed, contested, fortified, and lost. The relationship between land, population, and power.*

Territory control requires both military control and population loyalty. A region held only by force is unstable and may revolt.

The map is divided into territories, each containing resources, population, infrastructure, and a loyalty level. Controlling a territory grants access to its resources, but loyalty must be maintained.

If loyalty declines: tax revenue decreases, recruitment becomes difficult, sabotage risk increases, and rebellions may occur.

Control depends on military presence, local loyalty, infrastructure, and faith influence. Methods of loyalty maintenance include governance structures, faith influence, economic prosperity, and dynasty reputation.

Late-game territorial governance paths allow smaller territories to join a dynasty willingly due to prosperity and infrastructure (voluntary integration).

See also: `04_SYSTEMS/TERRITORY_SYSTEM.md`

---

## 8. Bloodline Members

*The named characters within a dynasty. Their traits, relationships, aging, death, and legacy mechanics.*

There are 20 active bloodline members maximum, with additional members beyond 20 becoming dormant. Bloodline members are not ordinary military units but family members within the ruling dynasty.

Members are born into the dynasty over time. Paths are chosen at birth and training begins in childhood. Training paths include military command, governance, religious leadership, diplomacy, covert operations, and economic stewardship.

Members become active leaders upon maturity and may serve as commanders, governors, diplomats, or ideological leaders. Exceptional leaders may become war heroes, receiving titles and lands that create lesser houses.

The family tree interface is a central gameplay element. Players must always know who leads the house, who the heirs are, specializations, heritage, and reputation with the population.

See also: `04_SYSTEMS/DYNASTIC_SYSTEM.md`

---

## 9. Dynastic Systems

*Marriage, succession, inheritance, bloodline traits, and the generational arc of a ruling house.*

The dynastic system is one of the four foundational design pillars alongside territory, population, and faith.

Dynastic gameplay includes family tree management, marriage alliances, succession disputes, bloodline capture, and creation of lesser houses.

Captured bloodline members may be enslaved, executed, ransomed, or married into the capturing dynasty. Cross-dynasty marriages can create new dynastic branches. Children may declare loyalty to either bloodline or form a new dynasty entirely.

Army recruitment uses adjustable sliders (not hard-locked doctrines). Dynasties mix family obligations, paid soldiers, and faith volunteers. Including daughters produces a negative attack buffer but positive healing and sustainment buffer.

Covert operations and assassination mechanics exist as strategic tools.

Starting leader options include: father, first oldest son, second oldest son, brother of the king, firstborn son of the king.

See also: `04_SYSTEMS/DYNASTIC_SYSTEM.md`

---

## 10. Army Structure

*How armies are raised, composed, commanded, and sustained. The relationship between population, resources, and military force.*

Armies are composed of units drawn from the population and trained through military infrastructure. Armies require food supply, gold payment, leadership, and morale support. Armies can be led by bloodline commanders or appointed generals. Leadership provides battlefield advantages.

**Level 1 Units (earliest military stage):**
- **Militia** — Basic soldiers from population, cheap, weak, low morale, emergency defense
- **Swordsmen** — First professional soldiers, moderate stats, faster movement, increased sight, serve as recon (scouts intentionally removed)
- **Spearmen** — Defensive infantry, higher defense, slower, effective at holding the line, strong vs cavalry
- **Hunters** — Light ranged, harassment and ambush, weak in melee
- **Bowmen** — Upgraded from hunters, specialize as Offensive (higher damage) or Defensive (increased range/accuracy), weak in close combat

Units beyond Level 1 are not yet fully designed.

See also: `10_UNITS/UNIT_INDEX.md`

---

## 11. Elite Units and Born of Sacrifice

*The extraordinary units created through the Born of Sacrifice system. Their cost, power, and narrative weight.*

Born of Sacrifice is a late-game elite army-forging mechanic. Players may sacrifice multiple armies to create a powerful elite force (e.g., five standard armies sacrificed to produce one elite force).

Elite armies may gain siege capability, enhanced armor, and extreme morale. Units can be named by the player or assigned names through a random generator.

These units are rare, extremely powerful, and often become legendary figures within a match. The mechanic creates identity-rich, memorable battlefield units tied to the ideological and dynastic character of the civilization.

See also: `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md`, `10_UNITS/UNIT_INDEX.md`

---

## 12. Level Progression and Divergence

*How factions, leaders, and units grow in power over the course of a match. Branching progression paths and meaningful divergence points.*

Bloodlines uses a five-level progression system. Each level represents advancement in civilization and military development.

- **Level 1** — Founding stage. Faith selected at end of Level 1.
- **Level 2** — Consolidation and infrastructure expansion.
- **Level 3** — Ideological expansion and faith influence.
- **Level 4** — Major irreversible divergence (point of no return). Unlocks grand faith structures and late-game powers. Dynasties commit to strategic directions.
- **Level 5** — Grand endgame apex structure tier. Apex capabilities.

Specialization lockouts exist at various levels. Grand faith structures represent each civilization's final spiritual, political, or metaphysical institutional expression. Specific structures for each faith are not yet fully designed.

---

## 13. Match Structure and Scale

*How a match begins, escalates, and resolves. Phase structure, timing, and the arc of a single game session.*

Matches progress through four strategic stages: Founding, Consolidation, Ideological Expansion, and Irreversible Divergence.

Optional long matches run from 2 to 10+ hours. Up to 10 players with AI kingdoms and minor tribes. Large matches may extend well beyond ten hours in epic campaigns.

Early game focuses on expansion and survival. Mid game introduces dynastic politics and religious development. Late game introduces ideological wars and victory escalation. Late game events may include global coalitions against dominant dynasties via the Trueborn coalition response system.

Victory conditions include military conquest (with instability risk), economic/currency dominance, faith divine right, territorial governance attraction (voluntary integration), and dynastic prestige (with prestige dispute wars).

See also: `11_MATCHFLOW/MATCH_STRUCTURE.md`

---

## 14. Multiplayer and Long Match Design

*Design considerations for multiplayer matches, long-form games, alliance systems, and competitive balance.*

Up to 10 players with AI kingdoms and minor tribes in a dynamic political environment. Different strategic playstyles remain viable. The world functions as a living political landscape.

Long match mode supports extended strategic campaigns within a single match, emphasizing territory development, dynastic expansion, faith influence, and long-term strategic planning.

The Trueborn coalition response system prevents early rush victories by allowing the neutral Trueborn city to raise a coalition against emerging hegemons. Contribution history to the Trueborn city matters.

Detailed network rules, lobby systems, team structures, diplomacy toggles, and multiplayer matchmaking rules have not yet been fully defined.

---

## 15. Future Expansion Concepts

*Ideas for post-launch content, expansions, new factions, new faiths, and mechanical additions. Speculative and exploratory.*

Recognized design gaps and future work priorities include:
- Complete unit structure beyond Level 1
- Building and technology tree progression
- Map control system in detailed implementation
- Faith structure buildings and named grand faith structures per covenant
- Dynasty character traits system
- Unit classes and combat system
- Territory influence zones
- Faith structure progression
- AI kingdom behavior models and doctrine systems
- Campaign progression structure
- Diplomacy rules and interface details
- Resource gathering implementation details
- Multiplayer lobby and ruleset detail
- Specific faith manifestation mechanics
- Prestige dispute war rules
- Currency dominance mechanical implementation
- ~~Final resolution of founding house naming~~ (RESOLVED: Variant 1 confirmed, 9 houses)

Future design ideas from sessions:
- Bloodline commanders influencing battlefield morale
- Faith manifestations appearing at high intensity
- Grand religious structures serving as ideological centers
- Religious civil wars between dynasties of the same covenant

---

*New sections are appended below this line as the design expands beyond the initial scope.*
