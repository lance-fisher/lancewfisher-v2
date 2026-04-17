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

---

## 16. Fortification System

*How settlements are made defensible. The defender-side specification for walls, towers, garrisons, and layered defense.*

Fortification is a first-class strategic path in Bloodlines, especially around primary dynastic keeps and bloodline seats. Depth of investment translates directly into measurable defensive leverage. A player who commits heavily to fortification must feel that investment matters and cannot be casually erased by wave attacks or suicide assaults.

Settlement classes set defensive ceilings: Border settlement, Military fort, Trade town, Regional stronghold, Primary dynastic keep, and Fortress-citadel (developed). Class is a property of the settlement. Investment deepens within-class but does not break class boundaries. A border settlement cannot become a fortress-citadel without material promotion to stronghold class first.

Fortifications are composed of three concentric layers. The outer works (berm, palisade, forward towers, gatehouse with killing bay) extract cost from the initial assault and channel attackers into known corridors. The inner ring (primary curtain walls, flanking towers, main gatehouse with murder holes and machicolations, shielded firing platforms, cisterns, granaries, reserve mustering yards) is the serious siege barrier; breach is a campaign achievement. The final defensive core (keep tower, bloodline seat, succession chamber, deep supply stores, faith inner sanctum) is final resistance; its fall is the fall of the house.

Defensive systems must function as a connected ecosystem rather than isolated health-bar objects. Walls, gates, towers, emplacements, garrisons, chokepoints, kill zones, signal systems, reserve mustering points, and inner fallback positions reinforce one another. Wave-spam and suicide erosion must not be a dominant answer. Failed assaults cost the attacker real military waste, tempo loss, and morale penalties.

See also: `04_SYSTEMS/FORTIFICATION_SYSTEM.md`, `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`

---

## 17. Siege System

*The attacker-side specification. How fortified settlements are assaulted and reduced. The canonical response to developed fortifications.*

Siege is an earned military operation, not a casual click action. It is a campaign-scale commitment. The simulation rewards preparation and punishes improvisation. Line infantry alone cannot reduce an elite fortress. An attacker who fails to prepare loses units and tempo with little to show for it.

Serious siege requires plural commitments across multiple categories. Siege engines include Ram, Siege tower, Trebuchet/counterweight engine, Ballista/bolt thrower, Mantlet/pavise line, and the late-game bombard/heavy trebuchet. Each has build cost in gold, wood, stone, and iron, movement limits, and escort requirements. Engineer specialists are a dedicated population trained separately from standard infantry; their functions include engine operation, earthworks, counter-fortification, mining and counter-mining, breach assessment, and ram crew rotation.

Logistics collapse ends sieges regardless of army size. Supply camps within movement range, supply wagons carrying food, water, arrows, and engine lumber, foraging rights or nearby farms, and rear-area security patrols are all required. Scouting and intelligence on weak wall sections, garrison patterns, reserve locations, faith and bloodline presence, and supply status shape breach priority planning.

Siege interacts with conviction, faith, and diplomacy. Sieging a seat of faith can generate Cruel conviction. Operations (sabotage, assassination, covert infiltration) can reduce siege time or trigger fortress collapse from within. Coordinated multi-front timing, isolation of the target, and faith powers may substitute for or amplify brute-force siege lines.

See also: `04_SYSTEMS/SIEGE_SYSTEM.md`, `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`

---

## 18. Naval System

*Naval warfare as a strategic extension of the land civilization. Trade projection, military deployment, and blockade at sea.*

Naval capability is not universal. Only dynasties with Coastal Zone territory can build and operate naval forces. A landlocked dynasty must either conquer coastal territory or accept permanent naval exclusion. This creates a strategic asymmetry that shapes how landlocked and coastal dynasties approach the map differently.

The sea is a highway and a weapon. Trade moves faster by sea than by land. Armies can be projected to distant shores without marching through hostile territory. Supply lines across sea routes are efficient when secure and catastrophic when disrupted. The sea does not belong to anyone. A fleet at sea is always exposed. Naval engagements are faster and more decisive than land battles; fleets do not hold ground, they control routes.

Naval access requires a Coastal Zone territory and a Harbor building. The Harbor upgrade tiers run Basic (small vessels, coastal patrol, fishing fleet bonus), Expanded (war galleys, troop transports, doubled sea trade capacity), and Grand (capital ships, full naval warfare capability, long-distance trade routes). A dynasty losing all Coastal Zones loses the ability to build new ships; existing ships survive but cannot be repaired or resupplied.

Naval unit types span economic (Fishing Vessel), patrol and escort (coastal patrol craft), troop transport, war galley, and late-tier capital ships. Blockade mechanics tie directly to Resource Economy and Territory systems: a successful blockade chokes a coastal dynasty's food and trade, creating conditions for population unrest without a single land engagement.

See also: `11_MATCHFLOW/NAVAL_SYSTEM.md`

---

## 19. Diplomacy and War System

*Formal diplomatic states, agreements, and the moral weight of honoring or breaking them. Diplomacy as a primary strategic dimension.*

Diplomacy is not a sidebar. It interacts with conviction, faith, population loyalty, economic victory, and the Trueborn coalition mechanics. A dynasty that manages its diplomatic relationships well can achieve outcomes that superior military force alone cannot. Diplomacy is not safe. Every formal agreement carries moral weight. Breaking it has a cost. Honoring it when it would be more convenient to break it is how Moral conviction is built.

Every pair of dynasties exists in one of five formal states: War, Hostile, Neutral, Cordial, or Allied. Transitioning between states requires specific actions and carries specific costs. Moving from Allied to War in a single step is an Alliance Betrayal and carries the full conviction and diplomatic consequences of that event.

The foundational action is sending an envoy, a Bloodline Member or appointed diplomat who can open formal channels, propose agreements, gather intelligence, or deliver ultimatums. Envoys can be held hostage, expelled, or killed; killing an envoy triggers an automatic Hostile state and a significant conviction penalty universally regardless of faith.

Agreement types include non-aggression pacts, trade agreements, military alliances, tribute arrangements, marriage contracts, and coalition commitments. Each has maintenance costs in gold, political attention, and occasionally military deployment. Overextended diplomatic commitments collapse when they matter most. Every diplomatic interaction reveals information about the other dynasty's priorities, resource state, and likely next move, making diplomacy also an intelligence system.

See also: `08_MECHANICS/DIPLOMACY_SYSTEM.md`

---

## 20. Operations System (Covert, Faith, Military)

*Three categories of deliberate strategic action executed by specialist units and bloodline members. Intelligence, sabotage, rituals, and targeted military operations.*

Every operation follows the loop Commit → Navigate → Execute → Consequence. The player assigns an operative, physically controls or directs them through the world, meets the operation's execution conditions, and deals with both the immediate effect and the systemic ripple across conviction, diplomacy, faith intensity, and population reaction.

Covert Operations are executed by Rogue-class units and Spymaster-committed bloodline members. The roster covers four intelligence operations (Scout Defenses, Infiltrate Court, Map Network, Identify Bloodline), three sabotage operations, and three assassination/capture operations. Every covert operation has an execution condition (position, timing, or combination) and a failure consequence (capture, exposure, diplomatic incident).

Faith Operations are executed by Mystic-class units (covenant-specific) and Priest/Priestess bloodline members. They include offensive rituals, defensive wards, and utility divination. Some require cosmic timing, some require gathered components, some require positional alignment. Different covenants have distinct faith operation rosters reflecting their doctrine.

Military Operations are distinct attack types beyond "fight the enemy army." The roster includes plunder, raze, siege, night assault, and massacre. Each is chosen at the point of committing forces, with timing and method affecting both outcome and conviction consequence. Operations are not click-and-wait; they are player-controlled tactical micro-plays on the live map. Timing (time of day, moon phase, season, world events) is a universal modifier across all three categories.

See also: `08_MECHANICS/OPERATIONS_SYSTEM.md`

---

## 21. Political Events System

*State-triggered events that punctuate the match, force adaptive play, and serve as moral tests. Six categories of event responding to faith, economy, war, dynasty, territory, and the Trueborn city.*

Events are triggered by game state conditions, not scripted timelines. Every event is the world responding to what is actually happening economically, militarily, religiously, and diplomatically. Events are visible to the players they affect and may be partially visible to others through Whitehall espionage mechanics.

Events serve three functions. Narrative punctuation breaks the continuous flow of resource management with moments that require attention and decision, creating the feeling that the world is alive. Strategic disruption introduces conditions that force adaptive play and can reverse momentum in ways raw military and economic power cannot. Moral testing presents choices between a moral and a pragmatic response, accumulating as conviction record over time.

Events are organized into six categories: Faith Events (The Schism, The Calling, The Covenant Test, The Heresy, The Great Reckoning), Economic Events, War Events, Dynastic Events, Territorial Events, and Trueborn City Events. Each category has distinct trigger logic and distinct relationships to game systems. Faith events trigger when faith system conditions reach thresholds; economic events trigger on resource curves; war events trigger on scale of engagement; dynastic events trigger on marriage, birth, succession, and death; territorial events trigger on expansion, loss, and loyalty shifts; Trueborn events trigger on hegemon-emergence conditions and coalition responses.

Unresolved events escalate. A Schism between two covenant dynasties becomes a Holy War Declaration eventually. An ignored Calling accumulates population drift that destabilizes neutral territories. The world does not wait for the player to decide; it responds to whether they act.

See also: `08_MECHANICS/POLITICAL_EVENTS.md`, `11_MATCHFLOW/POLITICAL_EVENTS.md`

---

## 22. Terrain and Geography System

*The ten terrain types and how each interacts with resources, military movement, faith affinity, and conviction. Terrain as mechanical weight, not decoration.*

The world of Bloodlines is shaped by the Great Frost. Every terrain type bears the mark of what the world went through and what it is still becoming. Terrain has mechanical weight that interacts with every major game system: resources, military, faith, population, and conviction.

Terrain serves three simultaneous functions. The strategic resource function produces different yields on different terrain, creating natural economic differentiation between starting positions and contested territories. The military function shapes army movement, cavalry effectiveness, formation viability, and unit-type home terrain advantage. The faith function gives each covenant natural affinity with specific terrain types: The Wild is strongest in forests, The Order in administrative centers. This is not a hard restriction but a gradient that skilled players exploit.

The ten terrain types are Reclaimed Plains, Ancient Forest, Iron Ridges, Coastal Zone, Frost Ruins, Marshland, River Valley, Arid Basin, Sacred Site, and Urban Core. Each has defined resource production profiles (Food, Water, Gold, Wood, Stone, Iron), military properties (cavalry effectiveness, cover, siege deployability), and faith affinities across the four covenants.

Terrain interacts with the Naval System (Coastal Zone enables harbors and fleets), the Fortification System (Iron Ridges and Sacred Sites support layered fortification; Marshland and River Valley shape approach geometry), and the Operations System (Ancient Forest and night-time conditions improve concealment; clear daylight on Reclaimed Plains increases detection). Terrain is the physical substrate on which every other system operates.

See also: `09_WORLD/TERRAIN_SYSTEM.md`, `09_WORLD/WORLD_INDEX.md`

---

## 23. Governance Framework and Source Layers

*How Bloodlines source material is managed across sessions and AI models. Active canon, historical archive, and the open design reservoir.*

Bloodlines source material exists across three simultaneous layers. These layers coexist at all times. One does not replace another. A newer document does not retroactively deprecate the contents of an older one unless Lance Fisher has explicitly ordered removal.

The Active Canon Snapshot is the current best organized state of the project. It is the working reference for active design sessions. It is not the total picture and it is not permission to erase what preceded it. The Historical Design Archive contains everything discussed across the full history of Bloodlines work: earlier bible versions, experimental branches, alternative formulations, mechanics raised once and never fully integrated but never explicitly killed, lore fragments, discarded systems that remain potentially valuable, and unresolved design tensions. The Open Design Reservoir is a living pool of ideas that have been discussed, floated, or partially developed without being definitively canonized or definitively removed. These are concepts in orbit, available for revival, not dead.

Governing rules: Never condense prior systems into fewer categories for neatness. If the project has seven types of a thing, document all seven. Never reduce victory routes into a smaller number for manageability. Never omit previously raised mechanics because they complicate the architecture. Never assume an older concept is deprecated because a later document does not include it. Never wipe alternatives. Preserve tensions and unresolved choices explicitly. If material from prior sessions is missing from the current context, flag the gap explicitly. Do not silently omit and do not fill with invention. Only Lance determines what is canon, deprecated, or removed.

See also: `01_CANON/CANONICAL_RULES.md`, `01_CANON/CANON_LOCK.md`, `01_CANON/DESIGN_GUIDE.md`

---

## 24. Development State and Implementation Tracking

*The engineering-side view of the project. Current state analysis, decisions register, stage gates, known issues, and Unity implementation plans.*

Bloodlines has crossed from pure design documentation into implementation planning as of 2026-04-12. Active development tracking lives in `docs/` at the project root. The Current State Analysis, Definitive Decisions Register, Completion Stage Gates, Development Reality Report, Implementation Roadmap, Known Issues list, Project Inventory, Runbook, and session handoffs are maintained there as the project moves toward prototype.

The Unity implementation track (`docs/unity/`) contains the Component Map, Data Pipeline, Migration Plan, System Map, Phase Plan, and session command briefs. These documents describe how canonical design maps onto executable systems without collapsing the canon itself.

The single-root consolidation (2026-04-13) established that `D:/ProjectsHome/FisherSovereign/lancewfisher-v2/bloodlines/` is the main working root. `D:/ProjectsHome/Bloodlines/` is a compatibility alias. `deploy/bloodlines/` is a compatibility surface for the web viewer, not a separate project root. Future canon, docs, tooling, browser runtime, and Unity work should happen in the main root.

This section is a pointer, not a replacement. Implementation status changes frequently. Consult `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` for the current snapshot and `docs/DEFINITIVE_DECISIONS_REGISTER.md` for the locked decisions ledger. The complete unified master (`18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_2026-04-14.md`) absorbs all of these documents verbatim for cross-session and cross-model continuity.

See also: `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md`, `docs/DEFINITIVE_DECISIONS_REGISTER.md`, `docs/COMPLETION_STAGE_GATES.md`, `docs/IMPLEMENTATION_ROADMAP.md`, `docs/unity/PHASE_PLAN.md`

---

*Additions 2026-04-14: Sections 16 through 24 appended in consolidation session covering the Siege, Fortification, Naval, Diplomacy, Operations, Political Events, and Terrain systems (canon since 2026-03-18 through 2026-04-14), the Governance Framework from unified v1.1, and a pointer to the active Development State tracking. Prior 15 sections preserved verbatim. See `19_ARCHIVE/PRE_2026-04-14_CONSOLIDATION/` for pre-consolidation snapshot.*

---

## 25. Master Design Doctrine and Continuity Enforcement

*Added 2026-04-14 from externally supplied doctrine source now preserved at `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`.*

Bloodlines is not to be treated as a standard medieval RTS shell with dynastic flavor attached afterward. The project remains a grand dynastic civilizational war game where the player governs a living bloodline across real-time warfare, multi-generational continuity, logistics, geography, fortification, faith, conviction, and long-horizon realm development. Command and Conquer remains an inspiration for clarity, pressure, readability, and "one more match" energy, but not a ceiling on systemic depth.

Population remains foundational. People are not decorative supply. They are the source of army recruitment, labor, construction, faith participation, territorial recovery, dynastic continuity, and economic output. Water remains a central strategic pillar, not a folded-away utility. Food security, land transformation, forestry stewardship, and transport protection remain necessary parts of civilizational growth rather than background economy noise. A realm that loses water, food, or logistics capacity must become meaningfully weaker in population stability and military sustainment. Strategic strangulation, negative upkeep pressure, and eventual desertion remain canonical.

Military doctrine remains multi-dimensional. Land posture, naval posture, commanders, delegated warfare, fortification, siege preparation, and multi-front planning all matter. A heavily fortified bloodline seat must behave as a real defensive ecosystem, not a larger health bar. Siege remains an earned operation. Continental separation, real naval strategy, and water-separated starts remain part of the world model. Houses remain true dynastic civilizations with military, civic, economic, spiritual, and aesthetic identity, not color-swapped factions.

Faith and conviction remain distinct foundational systems. Faith is covenantal and civilizational. Conviction is behavioral morality expressed through action. Late-game divergence remains meaningful and partially irreversible so that dynasties cannot become everything at once. Replayability continues to come from interdependent systems, asymmetry with readability, shifting geography, bloodline continuity, and different strategic development paths, not from cosmetic variance alone.

Player communication remains a design requirement, not a simplification excuse. The player must be able to read water strain, food strain, population condition, loyalty, legitimacy, doctrine posture, and bloodline state clearly without flattening the systems that produce those conditions. Graphics exist to serve clarity, atmosphere, house identity, and strategic readability. The project does not need photorealism to succeed. It does need visual discipline.

See also:

- `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`
- `docs/BLOODLINES_SYSTEM_CROSSWALK_2026-04-14_DOCTRINE_INGESTION.md`
- `docs/BLOODLINES_CANON_CONFLICT_NOTES_2026-04-14_DOCTRINE_INGESTION.md`
