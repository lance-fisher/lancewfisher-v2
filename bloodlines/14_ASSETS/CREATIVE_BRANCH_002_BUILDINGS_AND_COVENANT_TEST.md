# CREATIVE BRANCH 002 — BUILDINGS AND COVENANT TEST
**Status: PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON**
**Date: 2026-03-18, Creative Branch 002**

This document proposes a complete building technology tree for Bloodlines and a full specification for the Covenant Test across all four faiths and both doctrine paths. Nothing in this document modifies canonical files. All content is additive and exploratory until explicitly canonized.

---

# SECTION A: COMPLETE BUILDING TECHNOLOGY TREE

## Design Framework

Resource economy is structured around six primary resources: gold, food, water, wood, stone, iron. Iron is the scarcest early resource. The technology tree creates genuine tension at every level — players cannot build everything, and every structure choice has an opportunity cost.

**Build time scale (relative):**
- Very Short: 15-20 seconds
- Short: 30-45 seconds
- Medium: 60-90 seconds
- Long: 2-3 minutes
- Very Long: 4-6 minutes
- Grand: 8-12 minutes (match-defining investment)

Resource costs are expressed per build event, not per game cycle. Production/effect values represent output per standard game cycle (approximately 60 seconds of active play).

---

## CIVIC BUILDINGS

---

**Settlement Hall**
- Category: Civic
- Tier: 1 (starting structure — every dynasty begins with one)
- Prerequisites: None (starting structure)
- Resource cost: 0 gold / 0 food / 0 water / 0 wood / 0 stone / 0 iron (given at match start)
- Build time: N/A (placed at founding)
- Population capacity effect: Establishes base population capacity of 10. Serves as the administrative anchor of the settlement.
- Production/effect: Generates 5 gold per cycle passively. Enables all Tier 1 civic and economic construction. Acts as the respawn anchor for the dynasty leader unit.
- Upgrade path: Upgrades to Town Hall (Tier 2) — not a separate building but an in-place upgrade
- Strategic note: The Settlement Hall is the unconditional starting point. Losing it is a catastrophic event — it is the only building that cannot be rebuilt from scratch in the same location without a dedicated founding action.

---

**Town Hall**
- Category: Civic
- Tier: 2 (upgrade of Settlement Hall)
- Prerequisites: Settlement Hall (Tier 1), 1 Housing District, 1 Granary
- Resource cost: 80 gold / 0 food / 0 water / 40 wood / 60 stone / 0 iron
- Build time: Long
- Population capacity effect: Increases maximum population from 10 to 20. Unlocks the Population Ledger interface showing birth rate, death rate, and net growth projections.
- Production/effect: Generates 12 gold per cycle. Unlocks Tier 2 civic, economic, and military buildings. Enables the first dynasty title for the ruling house leader.
- Upgrade path: Upgrades to City Hall (Tier 3)
- Strategic note: The Town Hall upgrade is typically the first major investment decision. Players who rush it gain population headroom and gold generation early but delay food and military production. Players who delay it risk hitting population cap before they can support an army.

---

**City Hall**
- Category: Civic
- Tier: 3 (upgrade of Town Hall)
- Prerequisites: Town Hall (Tier 2), 1 Academy, 1 Market
- Resource cost: 200 gold / 0 food / 0 water / 80 wood / 150 stone / 20 iron
- Build time: Very Long
- Population capacity effect: Increases maximum population to 40. Unlocks the Political Events interface — allows the dynasty to issue edicts, form alliances, and respond to court events.
- Production/effect: Generates 25 gold per cycle. Unlocks all Tier 3 civic buildings. Unlocks the Dynasty Succession mechanic (heir designation). Required for Covenant Test eligibility.
- Upgrade path: None (apex civic structure)
- Strategic note: City Hall represents the dynasty's transformation from a settlement into a true civilization. The iron cost is intentionally painful at this stage — it forces players to choose between City Hall and military hardware.

---

**Housing District**
- Category: Civic
- Tier: 1
- Prerequisites: Settlement Hall
- Resource cost: 30 gold / 0 food / 0 water / 50 wood / 20 stone / 0 iron
- Build time: Medium
- Population capacity effect: +5 population capacity per Housing District. Multiple may be built; each adds +5.
- Production/effect: No direct resource production. Enables population growth above the base cap. Each Housing District also generates +1 happiness passively, which affects conviction pressure.
- Upgrade path: Can be upgraded to Expanded Housing District (Tier 2) — same build slot, costs 20 gold / 30 wood / 40 stone additional
- Strategic note: Housing Districts are the expansion valve for population. Players who neglect them will find armies they cannot grow. However, overbuilding housing before food is secured creates a population that starves, which tanks conviction and happiness simultaneously.

---

**Expanded Housing District**
- Category: Civic
- Tier: 2 (upgrade of Housing District)
- Prerequisites: Housing District (Tier 1), Town Hall
- Resource cost: 50 gold / 0 food / 0 water / 30 wood / 40 stone / 0 iron (upgrade cost on top of existing Housing District)
- Build time: Short
- Population capacity effect: Increases that district's capacity contribution from +5 to +12.
- Production/effect: Generates +2 happiness per cycle. Slightly reduces food consumption per population unit housed in this district.
- Upgrade path: None
- Strategic note: Expanded Housing Districts become efficient in the mid-game when population pressure is real. The upgrade cost is low enough that players can convert existing districts rather than building new ones, but the stone cost still competes with military construction.

---

**Well**
- Category: Civic
- Tier: 1
- Prerequisites: Settlement Hall
- Resource cost: 15 gold / 0 food / 0 water / 10 wood / 30 stone / 0 iron
- Build time: Short
- Population capacity effect: None directly. However, a settlement without a Well cannot sustain more than 5 population (population above 5 begins losing health and eventually dies without water access).
- Production/effect: Produces 8 water per cycle. Supplies up to 15 population with water needs met. Beyond 15 population, a second Well (or Cistern) is required to prevent water deficit.
- Upgrade path: Upgrades to Cistern (Tier 2)
- Strategic note: The Well is not optional — it is the first non-Hall structure most players build. A player who tries to skip it to rush an economic building will lose population to thirst within a few minutes. The stone cost is the constraint; stone supply determines how fast players can establish water security.

---

**Cistern**
- Category: Civic
- Tier: 2 (upgrade of Well)
- Prerequisites: Well (Tier 1), Town Hall
- Resource cost: 40 gold / 0 food / 0 water / 0 wood / 80 stone / 0 iron
- Build time: Medium
- Population capacity effect: None directly.
- Production/effect: Produces 20 water per cycle. Supplies up to 35 population. Excess water generation contributes to a small agricultural bonus — farms adjacent to a Cistern produce +15% food output.
- Upgrade path: Upgrades to Aqueduct (Tier 3)
- Strategic note: The Cistern is the mid-game water solution. Players expanding beyond 20 population need it. The stone cost is substantial and directly competes with Quarry output — players must decide whether to invest in water infrastructure or military stone consumption first.

---

**Aqueduct**
- Category: Civic
- Tier: 3 (upgrade of Cistern)
- Prerequisites: Cistern (Tier 2), City Hall
- Resource cost: 120 gold / 0 food / 0 water / 0 wood / 200 stone / 0 iron
- Build time: Very Long
- Population capacity effect: None directly, but enables population above 35 to be fully hydrated, removing the per-capita water constraint for up to 60 population.
- Production/effect: Produces 45 water per cycle. All farms within the settlement zone receive +25% food production. Removes the risk of water deficit for the dynasty's full population lifespan in most match scenarios.
- Upgrade path: None
- Strategic note: The Aqueduct is a late civic investment that pays off in sustained agricultural output rather than immediate impact. Players pursuing large population strategies will need it, but the 200 stone cost means it directly competes with Grand Sanctuary construction and late military walls.

---

**Granary**
- Category: Civic
- Tier: 1
- Prerequisites: Settlement Hall, at least 1 Farm
- Resource cost: 25 gold / 0 food / 0 water / 40 wood / 20 stone / 0 iron
- Build time: Short
- Population capacity effect: None directly. However, without a Granary, food production excess is lost — farms produce but the surplus is wasted each cycle.
- Production/effect: Stores up to 150 food. Prevents food spoilage. Enables food reserves — during famine cycles (no active farm production due to season or destruction), stored food feeds population for 2 full cycles before starvation begins.
- Upgrade path: Upgrades to Expanded Granary (Tier 2)
- Strategic note: The Granary is the second non-Hall civic structure most players build, immediately after the Well. Without it, Farm output is wasted. Players who delay it lose all early food surplus, which is a permanent setback.

---

**Expanded Granary**
- Category: Civic
- Tier: 2 (upgrade of Granary)
- Prerequisites: Granary (Tier 1), Town Hall
- Resource cost: 35 gold / 0 food / 0 water / 30 wood / 40 stone / 0 iron
- Build time: Short
- Population capacity effect: None.
- Production/effect: Stores up to 400 food. Extends famine buffer to 5 full cycles. Enables food trade at the Market if one exists. Enables the Rationing mechanic — player can reduce food consumption per population by 25% at a conviction cost.
- Upgrade path: None
- Strategic note: The Expanded Granary enables food as a strategic weapon — players with large reserves can survive sieges, outlast enemies in attrition wars, and use Rationing to squeeze more military spending during crises. Its moderate cost makes it an efficient mid-game upgrade.

---

## ECONOMIC BUILDINGS

---

**Farm**
- Category: Economic
- Tier: 1
- Prerequisites: Settlement Hall, Well
- Resource cost: 20 gold / 0 food / 0 water / 30 wood / 10 stone / 0 iron
- Build time: Short
- Population capacity effect: None.
- Production/effect: Produces 12 food per cycle. Each Farm feeds up to 8 population units. Multiple Farms stack — a dynasty of 30 population needs at least 4 Farms to sustain positive food balance.
- Upgrade path: Upgrades to Cultivated Farm (Tier 2)
- Strategic note: The Farm is the economic foundation of the early game. Players who under-build Farms will find their population growth strangled. Players who over-build them will lack gold and wood for military. Finding the minimum viable Farm count while expanding is the first true strategic challenge.

---

**Cultivated Farm**
- Category: Economic
- Tier: 2 (upgrade of Farm)
- Prerequisites: Farm (Tier 1), Town Hall, Granary
- Resource cost: 40 gold / 0 food / 0 water / 20 wood / 30 stone / 0 iron
- Build time: Short
- Population capacity effect: None.
- Production/effect: Produces 22 food per cycle (nearly double the basic Farm). Feeds up to 15 population units. Also produces 2 gold per cycle as surplus trade value.
- Upgrade path: None (specialized economic upgrade takes effect at Market instead)
- Strategic note: Cultivated Farms are the transition to food efficiency. Converting early Farms to Cultivated status in the mid-game reduces the total Farm footprint needed, freeing build slots for military and faith construction.

---

**Animal Ranch**
- Category: Economic
- Tier: 2
- Prerequisites: Town Hall, Farm (Tier 1)
- Resource cost: 60 gold / 0 food / 0 water / 50 wood / 20 stone / 0 iron
- Build time: Medium
- Population capacity effect: None.
- Production/effect: Produces 8 food per cycle (livestock food value, lower than farms but different composition). More critically: produces 1 horse unit per 3 cycles. Horses are the prerequisite resource for all cavalry units. Without an Animal Ranch, cavalry cannot be trained at any tier. Also produces 1 leather per cycle — leather is a soft material used in basic armor, reducing Barracks unit training costs by a small margin.
- Upgrade path: Upgrades to Breeding Grounds (Tier 3)
- Strategic note: The Animal Ranch is a pivotal mid-game economic decision. Players who want cavalry must build it early because horse production is time-gated. Players who build it late will find their cavalry ambitions delayed by 5-8 cycles of horse accumulation. The wood cost competes directly with Lumber Camp output during Tier 2.

---

**Breeding Grounds**
- Category: Economic
- Tier: 3 (upgrade of Animal Ranch)
- Prerequisites: Animal Ranch (Tier 2), City Hall, Armory
- Resource cost: 100 gold / 0 food / 0 water / 40 wood / 60 stone / 30 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Produces 1 horse unit per cycle (3x faster than Animal Ranch). Enables War Horse — a premium cavalry mount that increases cavalry unit combat ratings. Also produces 3 leather per cycle. Enables Beast Handler unit training (a specialized unit type that can tame local wildlife — minor independent mechanic).
- Upgrade path: None
- Strategic note: The Breeding Grounds is a commitment to cavalry as a primary military doctrine. The iron cost makes it a deliberate choice — players building Breeding Grounds are forgoing iron for other military uses. In return, they achieve cavalry tempo that opponents without this investment cannot match.

---

**Market**
- Category: Economic
- Tier: 2
- Prerequisites: Town Hall, Granary, at least 2 Farms or 1 Cultivated Farm
- Resource cost: 60 gold / 0 food / 0 water / 40 wood / 50 stone / 0 iron
- Build time: Medium
- Population capacity effect: None.
- Production/effect: Generates 15 gold per cycle (the highest raw gold income of any single economic building). Enables resource trading — surplus resources can be converted to gold at defined ratios (food: 3:1, wood: 4:1, stone: 5:1). Enables diplomatic trade routes with other dynasties — each active trade route generates +5 gold per cycle for both parties.
- Upgrade path: Upgrades to Grand Market (Tier 3)
- Strategic note: The Market is the economic backbone of mid-game gold income. Players without Markets will struggle to afford Tier 3 military and faith structures. However, the stone cost at a moment when stone is contested means the Market is often delayed one tier longer than players would prefer.

---

**Grand Market**
- Category: Economic
- Tier: 3 (upgrade of Market)
- Prerequisites: Market (Tier 2), City Hall
- Resource cost: 120 gold / 0 food / 0 water / 60 wood / 100 stone / 10 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Generates 30 gold per cycle. Enables Black Market (dark doctrine players or Cruel conviction players can access — resource conversions are less efficient but have no trade route requirement). Enables Caravan mechanic — player can dispatch caravans to neutral sites on the map for one-time resource windfalls. Enables political economic leverage — the dynasty with a Grand Market can impose trade embargoes on other dynasties, reducing their passive gold income by 5 per cycle during the embargo.
- Upgrade path: None
- Strategic note: The Grand Market transforms the dynasty into an economic power. It is particularly potent combined with diplomatic trade routes — a dynasty with 3 active trade routes and a Grand Market generates 45 gold per cycle from commerce alone, which is decisive for sustaining apex-tier military and faith construction simultaneously.

---

**Lumber Camp**
- Category: Economic
- Tier: 1
- Prerequisites: Settlement Hall, adjacent forest terrain (must be built near wood resource zone)
- Resource cost: 15 gold / 0 food / 0 water / 20 wood / 5 stone / 0 iron
- Build time: Very Short
- Population capacity effect: None.
- Production/effect: Produces 15 wood per cycle. Enables basic construction queue (all Tier 1 buildings require wood; Lumber Camp output directly limits build speed). Requires 2 population units assigned as laborers.
- Upgrade path: Upgrades to Timber Mill (Tier 2)
- Strategic note: The Lumber Camp is typically built in the first two minutes of any match. Wood scarcity in the opening phase determines how fast a dynasty can expand. Players who secure multiple wood resource zones can build Lumber Camps on each, generating wood income that sustains aggressive early construction.

---

**Timber Mill**
- Category: Economic
- Tier: 2 (upgrade of Lumber Camp)
- Prerequisites: Lumber Camp (Tier 1), Town Hall
- Resource cost: 40 gold / 0 food / 0 water / 30 wood / 30 stone / 0 iron
- Build time: Short
- Population capacity effect: None.
- Production/effect: Produces 28 wood per cycle. Reduces wood cost of all buildings by 10% (dynasty-wide passive). Unlocks Carpentry improvement — Siege Workshop and Harbor buildings cost 15% less wood when a Timber Mill is active.
- Upgrade path: None (lumber capacity scales by building additional Lumber Camps rather than upgrading existing ones)
- Strategic note: The Timber Mill pays for itself quickly in both raw output and the discount it applies to expensive late-game constructions. The Carpentry bonus is specifically valuable for naval players who need wood-intensive Harbor buildings.

---

**Quarry**
- Category: Economic
- Tier: 1
- Prerequisites: Settlement Hall, adjacent rock terrain (must be built near stone resource zone)
- Resource cost: 20 gold / 0 food / 0 water / 25 wood / 10 stone / 0 iron
- Build time: Short
- Population capacity effect: None.
- Production/effect: Produces 12 stone per cycle. Stone is the bottleneck resource for mid-game construction. Requires 2 population units assigned as laborers.
- Upgrade path: Upgrades to Stone Works (Tier 2)
- Strategic note: Stone scarcity is the defining constraint of the mid-game. Players who secure quarry sites early have a decisive advantage in construction pace. Quarry sites are limited and contested — early game territory expansion is often about reaching the nearest quarry before opponents do.

---

**Stone Works**
- Category: Economic
- Tier: 2 (upgrade of Quarry)
- Prerequisites: Quarry (Tier 1), Town Hall
- Resource cost: 50 gold / 0 food / 0 water / 20 wood / 30 stone / 0 iron
- Build time: Medium
- Population capacity effect: None.
- Production/effect: Produces 22 stone per cycle. Enables Dressed Stone — reduces stone cost of Tier 3 military and civic buildings by 12%. Unlocks the Stonecutting passive improvement, which increases the output of all adjacent Quarries by 20%.
- Upgrade path: None
- Strategic note: The Stone Works multiplies the output of the entire stone economy rather than replacing it. A player with a Stone Works and two Quarries effectively operates three quarries worth of output. This compounds into a massive construction advantage in the mid-game.

---

**Iron Mine**
- Category: Economic
- Tier: 2
- Prerequisites: Town Hall, adjacent iron ore terrain (must be built near iron vein resource zone), Quarry (Tier 1)
- Resource cost: 80 gold / 0 food / 0 water / 40 wood / 60 stone / 0 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Produces 6 iron per cycle. Iron is required for swordsmen, cavalry equipment, siege machinery, and advanced military hardware. Without iron, military development is permanently capped at basic spearmen and archers. Requires 3 population units assigned as miners.
- Upgrade path: Upgrades to Deep Iron Mine (Tier 3)
- Strategic note: The Iron Mine is the most strategically significant early-to-mid transition building in the game. The 60 stone cost at a time when stone is already contested makes it a deliberate sacrifice — players must choose to pursue iron or to continue other stone-dependent construction. Iron Mine sites are rare on most maps, making them priority military objectives. A dynasty without iron cannot field swordsmen, cavalry, or siege weapons.

---

**Deep Iron Mine**
- Category: Economic
- Tier: 3 (upgrade of Iron Mine)
- Prerequisites: Iron Mine (Tier 2), City Hall, Armory
- Resource cost: 140 gold / 0 food / 0 water / 30 wood / 80 stone / 20 iron (iron used to reinforce shaft supports)
- Build time: Very Long
- Population capacity effect: None.
- Production/effect: Produces 14 iron per cycle (more than double). Enables Refined Iron passive — reduces iron cost of all military units and buildings by 15% dynasty-wide. Unlocks the Ironmaster unit, a specialized support unit that repairs iron-dependent military equipment in the field.
- Upgrade path: None
- Strategic note: The Deep Iron Mine is a commitment to iron as the dynasty's military foundation. The upgrade cost uses iron itself, meaning the player must already have a stockpile before upgrading. Players who build it gain a permanent production advantage that compound over the late game into a decisive unit training speed advantage.

---

## MILITARY BUILDINGS

---

**Barracks**
- Category: Military
- Tier: 1
- Prerequisites: Settlement Hall, Well, at least 1 Farm
- Resource cost: 40 gold / 0 food / 0 water / 50 wood / 30 stone / 0 iron
- Build time: Medium
- Population capacity effect: None (military units do not consume population capacity — they are drawn from population pool at training time and returned on death).
- Production/effect: Enables training of Tier 1 infantry: Levy Spearman, Peasant Archer. Training speed: 1 unit per 30 seconds with full resource supply. Provides garrison capacity for 10 units within the settlement.
- Upgrade path: None (Barracks stays at Tier 1; higher military buildings layer on top of it)
- Strategic note: The Barracks is the first military building for virtually every player. The pressure to build it comes from immediate threat of rival incursion in the first 3-5 minutes. Players who delay it risk being overwhelmed before they can defend. Players who rush it sacrifice food and housing stability.

---

**Training Grounds**
- Category: Military
- Tier: 2
- Prerequisites: Barracks (Tier 1), Town Hall
- Resource cost: 70 gold / 0 food / 0 water / 40 wood / 60 stone / 10 iron
- Build time: Medium
- Population capacity effect: None.
- Production/effect: Enables training of Tier 2 infantry and ranged units: Swordsman (requires 3 iron per unit), Crossbowman, and Mounted Scout (requires 1 horse from Animal Ranch). Increases all Barracks unit training speed by 20%. Enables the Unit Discipline passive — trained units gain a minor combat bonus in organized formations.
- Upgrade path: None (Training Grounds is the Tier 2 military anchor; Armory and Siege Workshop are parallel expansions)
- Strategic note: The Training Grounds is the iron gateway. The 10 iron build cost is the first time players must spend iron on infrastructure rather than units. Players who have secured an Iron Mine will build Training Grounds without hesitation. Players who have not will find themselves locked out of swordsmen even after building it.

---

**Armory**
- Category: Military
- Tier: 2
- Prerequisites: Training Grounds, Iron Mine
- Resource cost: 90 gold / 0 food / 0 water / 30 wood / 80 stone / 20 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Enables heavy armor upgrade for infantry — Swordsmen become Heavy Swordsmen (+25% defense, +15% cost). Enables mounted cavalry: Light Cavalry (requires 2 horses + 4 iron per unit). Produces 1 unit of refined equipment per 4 cycles — these equipment tokens reduce the iron cost of the next unit trained by 2. Enables Weapon Smithing passive — all military units gain +5% attack when Armory is active.
- Upgrade path: Upgrades to Grand Armory (Tier 3)
- Strategic note: The Armory is where the military economy consolidates. Its 80 stone and 20 iron cost is the steepest military building cost up to this point and directly competes with faith building investment at Level 3. Players building the Armory are signaling a military-primary strategy; players delaying it are betting on faith units carrying the late-game instead.

---

**Grand Armory**
- Category: Military
- Tier: 3 (upgrade of Armory)
- Prerequisites: Armory (Tier 2), City Hall, Deep Iron Mine
- Resource cost: 160 gold / 0 food / 0 water / 50 wood / 120 stone / 40 iron
- Build time: Very Long
- Population capacity effect: None.
- Production/effect: Enables Elite Cavalry (heavy mounted units with lance — requires 3 horses + 8 iron per unit). Enables Siege Preparation passive — all siege weapons gain +20% attack. Produces 2 refined equipment tokens per 3 cycles. Enables the Master Smith — a hero-grade NPC that can be assigned to increase unit production speed by 30% for any building he is attached to.
- Upgrade path: None
- Strategic note: The Grand Armory is the apex military industrial investment for non-faith military strategies. It is most powerful for players who delayed faith investment and instead built a pure steel-and-iron army. The Master Smith hero provides exceptional value but requires gold upkeep.

---

**Siege Workshop**
- Category: Military
- Tier: 2
- Prerequisites: Training Grounds, Iron Mine, Timber Mill
- Resource cost: 80 gold / 0 food / 0 water / 70 wood / 50 stone / 15 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Enables training of siege equipment: Battering Ram (wood-heavy, no iron), Ballista (requires 5 iron per unit, high range), Catapult (requires 8 iron + 10 wood per unit, area damage). Siege units are the only military units with a carry-time penalty — they move at half speed of infantry. Enables Siege Engineering passive — wall and gate destruction speed for siege units increases by 30%.
- Upgrade path: Upgrades to Siege Foundry (Tier 3)
- Strategic note: The Siege Workshop is necessary for cracking fortified positions. Players who neglect it will find walled opponents effectively impregnable. However, the wood and iron dual cost means players must have both Timber Mill and Iron Mine established before it is economically viable. Siege production requires ongoing resource commitment — each Catapult costs as much as two Swordsmen in iron alone.

---

**Siege Foundry**
- Category: Military
- Tier: 3 (upgrade of Siege Workshop)
- Prerequisites: Siege Workshop (Tier 2), City Hall, Grand Armory
- Resource cost: 140 gold / 0 food / 0 water / 80 wood / 100 stone / 35 iron
- Build time: Very Long
- Population capacity effect: None.
- Production/effect: Enables Trebuchet (the apex siege weapon — extreme range, high damage, requires 12 iron + 15 wood per unit). Enables Siege Tower (allows infantry to assault walls directly — costly but bypasses gate entirely). Reduces all siege unit training time by 25%. Enables the Siege Master officer unit — attaches to siege groups to boost accuracy and speed by 20%.
- Upgrade path: None
- Strategic note: The Siege Foundry signals total-war capability. A dynasty with a Siege Foundry and multiple Trebuchets can reduce any fortification to rubble. Rivals who observe a Siege Foundry going up should treat it as a declaration of offensive intent and adjust diplomatic positioning accordingly.

---

**Watchtower**
- Category: Military
- Tier: 1
- Prerequisites: Settlement Hall
- Resource cost: 20 gold / 0 food / 0 water / 30 wood / 20 stone / 0 iron
- Build time: Very Short
- Population capacity effect: None.
- Production/effect: Reveals fog of war in a radius around the tower. Garrison capacity: 3 ranged units. Garrisoned units fire at approaching enemies. Multiple Watchtowers create overlapping vision networks.
- Upgrade path: Upgrades to Stone Tower (Tier 2)
- Strategic note: Watchtowers are essential for map awareness and early warning. They are cheap enough to place at territory boundaries and resource sites in the early game. Players who skip them are flying blind and will be vulnerable to surprise raids.

---

**Stone Tower**
- Category: Military
- Tier: 2 (upgrade of Watchtower)
- Prerequisites: Watchtower (Tier 1), Town Hall
- Resource cost: 40 gold / 0 food / 0 water / 10 wood / 60 stone / 0 iron
- Build time: Medium
- Population capacity effect: None.
- Production/effect: Larger vision radius than Watchtower. Garrison capacity: 6 ranged units. Garrisoned units gain +20% attack range bonus from elevation. Inherently more durable — requires siege equipment to destroy efficiently.
- Upgrade path: Upgrades to Fortified Tower (Tier 3)
- Strategic note: Stone Towers become the backbone of border defense in the mid-game. The stone cost is meaningful but the defensive value is high — a contested resource zone with a Stone Tower and 6 crossbowmen garrisoned represents a significant military investment to dislodge.

---

**Fortified Tower**
- Category: Military
- Tier: 3 (upgrade of Stone Tower)
- Prerequisites: Stone Tower (Tier 2), Fortified Keep, City Hall
- Resource cost: 80 gold / 0 food / 0 water / 0 wood / 120 stone / 20 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Maximum garrison: 10 units. Garrisoned ranged units gain +35% attack bonus. Can mount a Ballista on top (requires 1 Ballista unit consumed) for automatic fire against siege equipment approaching the tower. Near-impervious to non-siege attack.
- Upgrade path: None
- Strategic note: Fortified Towers are the final defensive investment for a territory that must not fall. The stone and iron costs are steep — players typically build 2-3 at critical chokepoints rather than converting their entire perimeter. A Fortified Tower with a mounted Ballista can stop siege equipment before it reaches walls.

---

**Fortified Keep**
- Category: Military
- Tier: 2
- Prerequisites: Barracks, Training Grounds, Town Hall
- Resource cost: 120 gold / 0 food / 0 water / 60 wood / 100 stone / 20 iron
- Build time: Very Long
- Population capacity effect: None.
- Production/effect: Acts as a military command center — provides the Rally mechanic (all units within range regenerate morale faster after routing). Garrison capacity: 20 units. Enables Wall Construction (the dynasty can now build defensive wall segments around their settlement). Provides bonus armor to all units within a radius during siege defense. Required for City Hall, Fortified Towers, and all Tier 3 military buildings.
- Upgrade path: None (the Fortified Keep is the capstone military structure, not an upgrade)
- Strategic note: The Fortified Keep is the most expensive single military building and the gateway to the entire late-game military stack. It is the prerequisite for walls, City Hall, and Tier 3 military. Players who build it are committing to a defensive doctrine — its cost and build time mean that players who rush it early sacrifice offensive pressure. Players who build it too late find rivals with siege equipment already assembled.

---

## FAITH BUILDINGS

Faith buildings use generic tier names (Shrine, Temple, Grand Sanctuary) but are named specifically per covenant. The generic tier names represent the mechanical function; the covenant-specific names represent the cultural and doctrinal identity.

---

**Shrine (Generic Tier 1) / Wayshrine (Old Light) / Blood-Altar (Blood Dominion) / Lawpost (The Order) / Grove Marker (The Wild)**
- Category: Faith
- Tier: 1
- Prerequisites: Settlement Hall, Granary
- Resource cost: 25 gold / 0 food / 0 water / 30 wood / 20 stone / 0 iron
- Build time: Short
- Population capacity effect: None.
- Production/effect: Establishes the dynasty's faith affiliation. Generates 3 faith intensity per cycle. Enables Faith Rites interface — basic covenant prayers and observances can be performed (minor buffs lasting 1-2 cycles). Generates +1 conviction alignment pressure toward the covenant's natural moral axis. Provides population morale bonus: +1 happiness per cycle.
- Upgrade path: Upgrades to Temple (Tier 2)
- Strategic note: The Shrine is typically the third civic structure built after Well and Granary. It is inexpensive and its faith output compounds — every cycle without a Shrine is faith intensity the dynasty will never recover. Players who delay faith establishment fall behind on the doctrine path and risk being unable to access Level 3 faith units at the right time.

---

**Temple (Generic Tier 2) / Hall of Remembrance (Old Light) / Covenant Hall (Blood Dominion) / Hall of Mandate (The Order) / Spirit Lodge (The Wild)**
- Category: Faith
- Tier: 2 (upgrade of Shrine)
- Prerequisites: Shrine (Tier 1), Town Hall, at least Active faith intensity (20%)
- Resource cost: 80 gold / 0 food / 0 water / 60 wood / 80 stone / 0 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Generates 8 faith intensity per cycle (nearly 3x Shrine output). Unlocks Doctrine Selection interface — at this point the player must choose Light or Dark doctrine path. This choice is permanent and cannot be reversed. Enables Tier 2 faith rites — more powerful observances with multi-cycle effects. Enables assignment of a Priest/Covenant leader NPC to the temple, who generates +2 faith per cycle bonus and can be assigned to lead military units as a morale officer.
- Upgrade path: Upgrades to Grand Sanctuary (Tier 3)
- Strategic note: The Temple is the most significant faith investment before the Grand Sanctuary. The 80 stone cost at Level 2-3 directly competes with Armory and Iron Mine construction. Players who build the Temple early are declaring a faith-primary strategy; players who delay sacrifice faith intensity accumulation and may find themselves unable to reach Devout intensity before rivals have military superiority.

---

**Grand Sanctuary (Generic Tier 3) / The Eternal Cathedral (Old Light) / The Wound (Blood Dominion) / The Iron Spire (The Order) / The First Grove (The Wild)**
- Category: Faith
- Tier: 3 (upgrade of Temple)
- Prerequisites: Temple (Tier 2), City Hall, Fortified Keep, at least Fervent faith intensity (60%)
- Resource cost: 300 gold / 0 food / 0 water / 150 wood / 250 stone / 50 iron
- Build time: Grand (8-12 minutes)
- Population capacity effect: None.
- Production/effect: Generates 20 faith intensity per cycle. Unlocks Apex Faith Rites — the most powerful doctrine expressions, each unique per covenant. Triggers the Covenant Test eligibility window — the test fires when Level 4 progression is reached after this structure is standing. Enables training of the Level 5 apex unit for the dynasty's covenant. Provides a permanent +5 conviction alignment pressure toward the covenant's natural axis — strong enough to overwhelm moderate neutral conviction without active resistance. Acts as the spiritual anchor of the dynasty — its destruction is a catastrophic event that immediately drops faith intensity by 30% and triggers a conviction crisis.
- Upgrade path: None (apex faith structure)
- Strategic note: The Grand Sanctuary is the match-defining construction for faith-path dynasties. Its 300 gold, 250 stone, and 50 iron cost represents a massive simultaneous investment across three of the game's six resources. Players cannot build it without first establishing mature economic infrastructure. The Grand build time means rivals can attack during construction — defending the Grand Sanctuary as it rises is one of the most intense military challenges in the game. A completed Grand Sanctuary signals to all players that this dynasty is approaching apex capability.

---

## SPECIAL BUILDINGS

---

**Dynasty Estate**
- Category: Special
- Tier: 2
- Prerequisites: Town Hall, Expanded Housing District
- Resource cost: 100 gold / 0 food / 0 water / 80 wood / 80 stone / 0 iron
- Build time: Long
- Population capacity effect: +10 (the Estate houses the ruling dynasty's household retainers and their families).
- Production/effect: Enables the Dynasty Bloodline mechanic in full — without an Estate, the dynasty leader has no formal lineage anchor and succession is uncertain. Generates 10 gold per cycle (dynastic revenue from titles and land). Enables the Heir system — the player designates an heir who gains experience passively and can be trained as a combat unit. Enables dynastic marriages with other player dynasties as a diplomatic action. Provides +2 happiness per cycle to the entire settlement.
- Upgrade path: Upgrades to Grand Estate (Tier 3)
- Strategic note: The Dynasty Estate is the social and political heart of the dynasty. Players who neglect it will find their dynasty's succession mechanics inoperable — the death of the dynasty leader without an heir causes a significant conviction crisis. Its gold income also makes it economically meaningful in the mid-game.

---

**Grand Estate**
- Category: Special
- Tier: 3 (upgrade of Dynasty Estate)
- Prerequisites: Dynasty Estate (Tier 2), City Hall
- Resource cost: 180 gold / 0 food / 0 water / 100 wood / 150 stone / 0 iron
- Build time: Very Long
- Population capacity effect: +20.
- Production/effect: Generates 20 gold per cycle. Enables Political Court — the player can recruit courtiers who each provide passive bonuses (military commander: +10% training speed; steward: +5 gold/cycle; spymaster: reveals 1 rival building per 10 cycles; bishop: +2 faith/cycle). Up to 3 courtiers active at once. Enables Marriage Alliance mechanic — a formal dynastic alliance with another dynasty grants both parties +5 gold per cycle and a shared military vision radius.
- Upgrade path: None
- Strategic note: The Grand Estate transforms the dynasty from a military organization into a political civilization. Its courtier system provides highly scalable passive bonuses that accumulate over time. Players who invest in it early tend to outscale rivals through compound bonuses rather than through raw military output.

---

**Academy**
- Category: Special
- Tier: 2
- Prerequisites: Town Hall, Granary
- Resource cost: 90 gold / 0 food / 0 water / 70 wood / 60 stone / 10 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Enables the Research mechanic — each cycle, the Academy generates 1 Research Point. Research Points can be spent on dynasty-wide passive improvements from three tracks: Military (unit combat stats), Economic (production rates), or Civic (population happiness, growth rates). At any time, only one Research track can be active. Each improvement takes 5-15 Research Points. Enables the Scholar NPC — a unique unit that can be assigned to any building to generate bonus Research Points (2 per cycle while assigned). Enables Academy Diplomacy — research tech can be traded or sold to allied dynasties.
- Upgrade path: Upgrades to Grand Academy (Tier 3)
- Strategic note: The Academy is the dynasty's long-term scaling engine. Its research bonuses compound over time — a dynasty that builds it early and maintains consistent Research Point generation will have mid-game unit stats and production rates that opponents cannot match without their own Academy. The iron cost is the early friction point.

---

**Grand Academy**
- Category: Special
- Tier: 3 (upgrade of Academy)
- Prerequisites: Academy (Tier 2), City Hall
- Resource cost: 150 gold / 0 food / 0 water / 80 wood / 120 stone / 20 iron
- Build time: Very Long
- Population capacity effect: None.
- Production/effect: Generates 3 Research Points per cycle. Enables the fourth Research Track: Faith (faith intensity generation rate, covenant rite power, doctrine expressions). Enables dual-track research — two improvements can be developed simultaneously. Enables the Master Scholar NPC (upgraded version of Scholar) — generates 5 Research Points per cycle and can brief the player's commander units, granting them a one-time bonus to a specific combat encounter. Enables Knowledge Sharing diplomacy — any dynasty in a formal alliance with this player gains +1 Research Point per cycle as a secondary benefit.
- Upgrade path: None
- Strategic note: The Grand Academy is the apex intellectual investment. Its most potent feature is dual-track research, which means the dynasty can simultaneously improve military capability and faith intensity — removing the opportunity cost that limits single-track academies in the late game.

---

**Treasury**
- Category: Special
- Tier: 2
- Prerequisites: Town Hall, Market
- Resource cost: 80 gold / 0 food / 0 water / 40 wood / 100 stone / 10 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Stores up to 1,000 gold (the default cap without a Treasury is 400 gold — excess is lost each cycle). Generates 8 gold per cycle from reserve interest. Enables the Loan mechanic — the dynasty can take a gold loan of up to 200 gold, repaid at 1.5x over 5 cycles. Enables War Chest — at any time, the player can designate a War Chest amount (locked gold reserve that cannot be spent on construction) for military emergencies. War Chest can be released instantly when needed. Enables the Tribute mechanic — defeated dynasties or weaker neighbors can be forced to pay tribute, which flows into the Treasury as passive gold income.
- Upgrade path: Upgrades to Grand Treasury (Tier 3)
- Strategic note: The Treasury is essential for dynasties pursuing economic dominance rather than military conquest. Without it, gold accumulation is capped and production cycles above the cap are wasted. Players who establish a Treasury early can stockpile gold for late-game Grand Sanctuary construction without scrambling for resources during the build window.

---

**Grand Treasury**
- Category: Special
- Tier: 3 (upgrade of Treasury)
- Prerequisites: Treasury (Tier 2), City Hall, Grand Market
- Resource cost: 160 gold / 0 food / 0 water / 60 wood / 180 stone / 20 iron
- Build time: Very Long
- Population capacity effect: None.
- Production/effect: Stores up to 3,000 gold. Generates 18 gold per cycle from reserve interest. Enables the Economic War mechanic — the dynasty can invest gold to destabilize another dynasty's economy, reducing their gold generation by 10% for 5 cycles (can be extended with further investment). Enables Strategic Reserves — the Treasury can now store 300 units each of food, wood, and stone in addition to gold. Enables the Mint — the dynasty can spend 50 stone to mint coin tokens, which convert to 80 gold (a 1.6x multiplier on stone value, useful for dynasties with stone surplus and gold deficit).
- Upgrade path: None
- Strategic note: The Grand Treasury is the economic-warfare structure. Its Economic War mechanic is the only building-derived tool that can directly reduce a rival's resource income without military engagement. Players who build it are declaring themselves as economic combatants — other players should treat a Grand Treasury as a threat even without visible military aggression.

---

## HARBOR BUILDINGS

Harbor buildings require coastal or major river terrain. They are economic-military hybrids — they enable naval trade and naval military projection simultaneously.

---

**Basic Harbor**
- Category: Harbor (Economic + Military)
- Tier: 1
- Prerequisites: Town Hall, Market, coastal or major river map position
- Resource cost: 100 gold / 0 food / 0 water / 80 wood / 60 stone / 10 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Enables naval trade — each cycle, the Harbor generates 8 gold from import/export activity without requiring active trade routes. Enables training of basic naval units: River Raft (cheap, low combat value), Patrol Boat (light naval combat). Enables Water Supply route — if the dynasty is suffering water deficit, the Harbor can import 5 water per cycle from naval trade at a cost of 5 gold per cycle. Enables Naval Transport — up to 10 infantry units can be transported by water each 2 cycles.
- Upgrade path: Upgrades to Expanded Harbor (Tier 2)
- Strategic note: The Basic Harbor is a significant early investment but pays dividends quickly in coastal maps. The gold generation from passive trade means it often recovers its cost within 15-20 cycles. Its primary value is positioning — a dynasty with a Harbor controls naval routes and can use Naval Transport to project force across water terrain that landlocked rivals cannot cross.

---

**Expanded Harbor**
- Category: Harbor (Economic + Military)
- Tier: 2 (upgrade of Basic Harbor)
- Prerequisites: Basic Harbor (Tier 1), Armory, Timber Mill
- Resource cost: 140 gold / 0 food / 0 water / 100 wood / 80 stone / 20 iron
- Build time: Long
- Population capacity effect: None.
- Production/effect: Generates 18 gold per cycle from trade. Enables War Galley training (heavy naval combat unit, requires 8 iron + 20 wood per unit). Enables the Naval Blockade mechanic — the dynasty can blockade a rival's harbor, reducing their trade gold income by 15 per cycle and preventing naval transport. Naval Blockade requires 2 War Galleys assigned. Enables Naval Supply Line — allied land forces operating within a coastal zone receive +10% resource resupply per cycle.
- Upgrade path: Upgrades to Grand Harbor (Tier 3)
- Strategic note: The Expanded Harbor transforms the dynasty from a coastal settlement into a naval power. The War Galley's iron cost is steep — building 2 War Galleys for a blockade consumes 16 iron, equivalent to 4 Heavy Swordsmen. Naval players must accept this trade-off as a strategic specialization.

---

**Grand Harbor**
- Category: Harbor (Economic + Military)
- Tier: 3 (upgrade of Expanded Harbor)
- Prerequisites: Expanded Harbor (Tier 2), City Hall, Grand Market, Siege Workshop
- Resource cost: 280 gold / 0 food / 0 water / 200 wood / 180 stone / 60 iron
- Build time: Grand (8-10 minutes)
- Population capacity effect: None.
- Production/effect: Generates 40 gold per cycle from trade — the highest passive gold income of any single building in the game. Enables War Fleet — the dynasty can field a coordinated fleet of up to 8 naval units as a named formation. Enables Naval Siege — War Galleys can now bombard coastal land targets, dealing siege-equivalent damage to coastal buildings and walls. Enables the Trade Empire mechanic — for every active trade route combined with the Grand Harbor, generate an additional 3 gold per cycle. Enables Deepwater Passage — naval units can now traverse to otherwise inaccessible deep-sea map zones if the map includes them.
- Upgrade path: None
- Strategic note: The Grand Harbor is the naval equivalent of the Grand Sanctuary — a match-defining investment that signals the dynasty's intent to dominate through maritime trade and naval military power. Its 40 gold per cycle income combined with the Trade Empire mechanic means a dynasty with the Grand Harbor and 3 trade routes generates 49 gold per cycle from commerce alone. Naval Siege capability means the Grand Harbor dynasty can project military force against coastal opponents without land engagement. The wood cost is the primary constraint — players pursuing this path must control multiple Lumber Camps and ideally have a Timber Mill established well in advance.

---

## HOUSE-SPECIFIC BUILDINGS

The six Great Houses do not have universally confirmed house-specific buildings in canon. The following are proposed as Creative Branch 002 explorations and are explicitly not canon. Each house has one proposed house-exclusive building that reflects its identity.

**PROPOSED — NOT CANON:**

**Ashenvale Muster Hall** (proposed for Ashenvale dynasty)
- Category: Military (house-exclusive)
- Tier: 2
- Prerequisites: Barracks, Training Grounds
- Resource cost: 90 gold / 0 food / 0 water / 60 wood / 70 stone / 15 iron
- Production/effect: Enables the Warbanding mechanic — Ashenvale can muster units in half the normal training time during a declared war phase. Represents Ashenvale's tradition of rapid military mobilization.
- Strategic note: This building's value is entirely in timing — it is most powerful when a war is actively declared, making it a situational investment that rewards aggressive players.

**Iron Compact Hall** (proposed for The Order-aligned house)
- Category: Civic + Military hybrid (house-exclusive)
- Tier: 2
- Prerequisites: Town Hall, Barracks
- Resource cost: 80 gold / 0 food / 0 water / 40 wood / 90 stone / 25 iron
- Production/effect: Enables the Compact mechanic — the house can issue mandatory service edicts that instantly convert 5 population units into military units without training cost. Edict has a cooldown of 10 cycles. Represents the Order's doctrine that law compels service.
- Strategic note: The Compact is a powerful burst mechanic that can swing a defensive battle, but it permanently removes those population units from economic roles, creating a post-edict production dip.

**The Remembrance Hall** (proposed for Old Light-aligned house)
- Category: Faith + Civic hybrid (house-exclusive)
- Tier: 2
- Prerequisites: Shrine, Town Hall
- Resource cost: 70 gold / 0 food / 0 water / 50 wood / 60 stone / 0 iron
- Production/effect: Enables Ancestor Observance — once per 15 cycles, the dynasty can invoke a deceased dynasty leader's memory to grant all units in the field a +15% morale bonus for 2 cycles. Records the names and deeds of fallen units in a Remembrance Log (UI element). Generates +3 faith intensity per cycle.
- Strategic note: The morale bonus is most valuable in large sustained engagements. The Remembrance Log has no mechanical effect but contributes to the emotional identity of the dynasty as an Old Light follower.

---

# SECTION B: THE COVENANT TEST — FULL SPECIFICATION

## Overview

The Covenant Test is a match-defining event that fires when a dynasty reaches Level 4 progression with a Grand Sanctuary standing. It is the faith's moment of judgment: does this dynasty truly embody its doctrine, or does it merely use the faith as a tool? The test demands a specific grand act. Success unlocks apex mechanics. Failure imposes a cost proportional to the doctrine's severity.

All rivals receive a notification when a Covenant Test is issued: **"[Dynasty Name] faces [Covenant Name]'s judgment."** No further detail is revealed — rivals must infer what the test demands based on the covenant they know the dynasty follows. This creates a window of tactical opportunity and diplomatic maneuvering.

---

## OLD LIGHT — LIGHT DOCTRINE COVENANT TEST

**The Covenant Test of Mercy's Flame**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The Eternal Cathedral) is standing. Faith intensity is at Apex (80%+). Light Doctrine must have been declared at Temple construction.

The Demand:
The Eternal Cathedral speaks: The Flame does not consume to destroy — it purifies to preserve. The Old Light demands that this dynasty demonstrate its commitment to mercy even at cost to itself.

The test requires the following acts, all completed within a 10-cycle window:
1. Release all captured enemy units currently held as prisoners (if any) — this is a free action but removes potential prisoner leverage.
2. Heal at least 20 allied population units using Flame Wardens or Covenant Rites — this requires active deployment of healer units to the civilian population, pulling them from military assignment.
3. Construct a Sanctuary Beacon — a temporary structure that costs 60 gold / 40 wood / 50 stone and must be built within the 10-cycle window. The Sanctuary Beacon signals the dynasty's mercy to all players — any rival dynasty that approaches within its radius with non-hostile intent cannot be attacked for 2 cycles (a temporary ceasefire zone). The beacon stands permanently after the test but its ceasefire effect expires after 5 cycles post-test.
4. The dynasty must not initiate offensive military action during the 10-cycle window. They may defend if attacked.

Time window: 10 cycles (approximately 10 minutes of game time).
Other players notified: Yes — "The Old Light demands mercy of [Dynasty Name]. Their flame is tested."

Success reward:
- Unlocks The Unbroken (apex Level 5 unit) training at The Eternal Cathedral.
- Permanent Mercy's Light passive: all allied units heal 2 HP per cycle when within the settlement radius.
- Flame Purification rite becomes available: once per match, the dynasty can cleanse a territory of conviction corruption, resetting it to neutral.
- Population conviction alignment receives a permanent +10% pull toward Moral conviction.

Failure penalty:
- Faith intensity drops from Apex to Fervent (a 20-point reduction).
- Sanctuary Beacon construction fails if incomplete — no beacon placed.
- All Flame Wardens lose their healing ability for 5 cycles (the Flame withdraws from those who proved unworthy).
- A Schism event fires: 5 random population units become Faithless, reducing happiness by 5 per cycle until resolved through a Faith Rite.

Narrative framing:
The population of the Old Light dynasty experiences the test as a moment of divine silence — the flame in every Wayshrine dims, and the priests say that the Light is listening. Healers are sent into the streets. Enemies held in prison are released. Other dynasties watch smoke signals from the Cathedral and know something sacred is happening within those walls — something that stays the dynasty's sword during the window. Whether they exploit that mercy is the oldest question the Light has ever asked of the world.

Strategic implications:
The Light Doctrine Covenant Test is the most diplomatically complex test in the game. The prohibition on offensive action creates a 10-cycle vulnerability window that rivals can exploit if they choose. However, the Sanctuary Beacon's ceasefire zone creates a mutual deterrent — rivals who approach aggressively during the test and violate the beacon's sanctity receive a conviction penalty and risk coalition response from any third dynasty observing the violation. The test rewards dynasties who have established strong diplomatic networks — those with allies are safer during the window. Isolated dynasties face genuine danger.

---

## OLD LIGHT — DARK DOCTRINE COVENANT TEST

**The Covenant Test of the Consuming Flame**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The Eternal Cathedral) is standing. Faith intensity is at Apex (80%+). Dark Doctrine must have been declared at Temple construction.

The Demand:
The Cathedral burns bright — and the Dark Doctrine teaches that the Flame does not preserve. It consumes the unclean so that the pure may inherit. The Old Light demands that this dynasty demonstrate the righteous fire of the Inquisition.

The test requires the following acts, all completed within a 12-cycle window:
1. Identify a Heresy Target — the dynasty must designate one rival dynasty as Heretical. This cannot be undone. The designated dynasty is publicly revealed to all players as the target of an Inquisition.
2. Destroy at least 1 faith building (Shrine, Temple, or Grand Sanctuary tier) belonging to the Heresy Target. This requires military assault on the target's territory.
3. Purge the Heretic — the dynasty's Inquisitor units must kill at least 8 enemy military units belonging to the Heresy Target in the field (not building kills — unit kills).
4. Construct the Inquisitor's Post — a special structure (100 gold / 60 wood / 80 stone / 15 iron) that permanently marks the campaign against heresy and generates 2 faith intensity per cycle.

Time window: 12 cycles.
Other players notified: Yes — "[Dynasty Name] has declared [Target Dynasty] heretical. The Consuming Flame is lit." The target dynasty is publicly shamed before all players.

Success reward:
- Unlocks The Unbroken (apex Level 5 unit) training at The Eternal Cathedral.
- Permanent Inquisitor's Will passive: Inquisitor units deal +30% damage and gain the ability to convert enemy units with low conviction alignment (forcing them to join the dynasty temporarily).
- Purge Decree becomes available: once per match, the dynasty can issue a Purge Decree against any territory they control — all Faithless and Low Conviction population units in that territory are removed (dies), and faith intensity receives +10% instantly.
- Population conviction is pulled +15% toward Cruel conviction permanently.

Failure penalty:
- Faith intensity drops from Apex to Active (a 60-point collapse — severe).
- The Heresy Target declaration stands — the dynasty has publicly declared an enemy without following through. All other dynasties receive a +5 diplomatic reputation bonus against the declaring dynasty (they appear weak and impulsive).
- All Inquisitor units are disbanded for 5 cycles — the Flame recalls its servants for having failed the Purge.
- A Crisis of Faith event fires: the dynasty leader loses their title for 3 cycles as the court questions their worthiness.

Narrative framing:
The Dark Flame test is public and violent. When the Inquisition is declared, every dynasty in the match receives word of the naming — the heretical dynasty knows they are the target of a holy war. The attacking dynasty's soldiers fight with zealot conviction, their morale unnaturally high. The target dynasty's population near the conflict zones experiences dread and increased desertion rates. Other dynasties watch and choose their position — ally with the inquisitors, defend the heretics, or stay neutral while the fire burns.

Strategic implications:
The Dark Doctrine Covenant Test is the most militarily aggressive test in the game. It forces the dynasty to go to war within the window, specifically against a designated target. The public naming creates political consequences for all parties — the target will likely seek allies, and other dynasties must decide whether the inquisition is an opportunity for a coalition strike or a threat that should be diplomatically supported. The 12-cycle window is tighter than it appears because military operations take time to mobilize, execute, and confirm. Dynasties that begin war preparations before the test fires will complete it comfortably. Dynasties that are caught flat-footed by the trigger timing may fail despite having military capability.

---

## BLOOD DOMINION — LIGHT DOCTRINE COVENANT TEST

**The Covenant Test of the Shared Covenant**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The Wound) is standing. Faith intensity is at Apex (80%+). Light Doctrine must have been declared at Temple construction.

The Demand:
The Wound is the place where the covenant is renewed — not through violence but through the willing gift of blood. The Blood Dominion, in its light doctrine, teaches that sacrifice is love made manifest. The faith demands that this dynasty prove its covenant by binding itself to those it leads.

The test requires the following acts, all completed within an 8-cycle window:
1. Covenant Bond Offering — the dynasty's Covenant Priest units must perform a binding rite on at least 15 allied population units. The rite uses 5 food per unit bound (75 food total) as a ceremonial feast component. This represents genuine sacrifice of resources for the benefit of the people.
2. Bloodline Renewal — the dynasty heir must be declared (Dynasty Estate required) and must participate in the covenant renewal ceremony at The Wound. The heir is temporarily vulnerable during this ceremony — they cannot be assigned to military units for 3 cycles.
3. Build a Covenant Hearth — a special structure (80 gold / 50 wood / 60 stone) built adjacent to The Wound. The Covenant Hearth represents the communal bond and generates 5 happiness per cycle permanently.
4. The dynasty must feed all its population units at full food ration for the entire 8-cycle window — no rationing allowed during the test.

Time window: 8 cycles.
Other players notified: Yes — "The Blood Dominion calls [Dynasty Name] to covenant. The feast is prepared." Rivals can observe heavy food consumption and the Covenant Hearth construction.

Success reward:
- Unlocks The Sacrificed (apex Level 5 unit) training at The Wound.
- Covenant Bond becomes a permanent mechanic: any unit that a Covenant Priest has bonded with gains +20% combat resilience and 50% reduced morale loss from fear effects.
- Bloodline Renewal passive: when the dynasty leader dies, the heir inherits all active Research improvements and 75% of accumulated faith intensity (instead of the default 50% inheritance).
- Population conviction pulled +15% toward Moral conviction permanently.

Failure penalty:
- Faith intensity drops from Apex to Devout (a 40-point reduction).
- The Covenant Hearth cannot be built — the bond is incomplete.
- Population happiness drops by 8 per cycle for 5 cycles as the feast that was promised was not delivered.
- Covenant Priest units lose their bonding ability for 6 cycles.

Narrative framing:
The covenant feast is a visible, communal act. The dynasty's settlement smells of cooking and ceremony. Covenant Priests move through the population with bowls of shared food and ritual words. The heir stands at The Wound and speaks the binding oaths. Other dynasties see smoke from the feast fires and the Covenant Hearth rising — they know the bond is being renewed. In Blood Dominion theology, this moment is the warmest — the faith at its most human, its most loving. The people who receive the covenant bond carry it in their bodies as a living mark.

Strategic implications:
The Light Doctrine Covenant Test is a resource-management challenge. The 75 food cost for the rite is significant, and the full ration requirement means the dynasty cannot use the Rationing mechanic during the window to free up gold for other spending. The 8-cycle window is the shortest test window in the game — it requires immediate action. The heir's vulnerability during the ceremony creates a brief window for assassination-style play by rivals. The overall test is favorable for dynasties with strong food surpluses; hostile for dynasties already running food deficits.

---

## BLOOD DOMINION — DARK DOCTRINE COVENANT TEST

**The Covenant Test of Dominion's Price**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The Wound) is standing. Faith intensity is at Apex (80%+). Dark Doctrine must have been declared at Temple construction.

The Demand:
The Wound is the altar of truth. In its dark doctrine, the Blood Dominion teaches that covenant is not given — it is taken. Blood rites are not metaphor. They are payment. The faith demands that this dynasty prove its dominion by making the ultimate offering at the altar.

The test requires the following acts, all completed within a 10-cycle window:
1. Blood Rite of Claiming — the dynasty must sacrifice 3 of its own military units at The Wound. These units are permanently destroyed. The sacrifice must be performed by a Blood Rider unit assigned to the altar.
2. Dominion Assertion — the dynasty must subjugate at least 1 rival dynasty's territory during the test window, claiming at least 2 resource production buildings (Farms, Quarries, Iron Mines, or Lumber Camps). The subjugated buildings are now under the dynasty's control.
3. The Mark of Dominion — at least 10 of the dynasty's population units must be branded with the Blood Dominion's mark. Branding is a permanent action — branded population units produce +2 gold per cycle each (20 gold/cycle bonus total) but cannot be healed by any means other than Blood Rider units.
4. The Blood Rider commander must survive the entire 10-cycle window. If the Blood Rider performing the rite dies, the test fails regardless of other completions.

Time window: 10 cycles.
Other players notified: Yes — "The Wound demands tribute. [Dynasty Name] has opened the altar." All players can see that the Blood Dominion test has fired but not the specific demands.

Success reward:
- Unlocks The Sacrificed (apex Level 5 unit) training at The Wound.
- Blood Sovereignty passive: all branded population units generate +3 gold per cycle (upgraded from the test's +2).
- Dominion Chains mechanic: subjugated territories continue producing resources for the dynasty at 80% efficiency even without garrison (normally, subjugated territories require garrison to function).
- Blood Rider units gain the Unstoppable passive: they cannot be routed and fight to full death without morale checks.
- Population conviction pulled +20% toward Cruel conviction permanently.

Failure penalty:
- Faith intensity collapses from Apex to Latent (a catastrophic 80-point drop — the most severe failure in any covenant test).
- The 3 sacrificed units are still lost (the sacrifice was made but the covenant rejected it).
- A Rejection of Blood event fires: the population is terrified. Happiness drops by 12 per cycle for 8 cycles. Conviction pressure shifts 30% toward Moral (the opposite of intended) as the population recoils from the failed blood rite.
- Blood Rider units go dormant for 8 cycles — the covenant has withdrawn its blessing from those who failed.

Narrative framing:
The Blood Dominion dark test is witnessed in fire and horror. The altar at The Wound is lit. The Blood Rider stands over the kneeling sacrifices. The population watches — some in reverence, some in dread. Far across the map, rival dynasties smell the smoke and see messengers running between settlements, their faces pale. In the Blood Dominion's dark theology, this moment is not cruelty — it is clarity. The covenant is real. Its price is real. The question the faith asks is whether this dynasty is willing to pay. If the dynasty fails — if the Blood Rider falls, if the subjugation does not happen, if the rite is incomplete — the altar goes cold, and the covenant withdraws. The terrified population does not experience religious crisis. They experience the far worse thing: discovering that the god was real, and they failed it.

Strategic implications:
The Dark Dominion Covenant Test is the highest-risk, highest-reward test in the game. Its failure penalty is uniquely catastrophic — a drop from Apex to Latent is the longest possible faith recovery journey, and with it goes all Apex-tier benefits during the recovery period. The test requires simultaneous military aggression (territory subjugation) and internal ceremony (altar rite), which means the dynasty must divide attention between offense and home defense at the same time. The Blood Rider survival requirement adds a personal stakes element — protect that unit at all costs during the window, because losing it means total failure regardless of other achievements. Rivals who identify the Blood Rider can target it specifically to deny the test.

---

## THE ORDER — LIGHT DOCTRINE COVENANT TEST

**The Covenant Test of the Living Mandate**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The Iron Spire) is standing. Faith intensity is at Apex (80%+). Light Doctrine must have been declared at Temple construction.

The Demand:
The Iron Spire does not merely stand as a monument. It stands as a reminder that law protects those who cannot protect themselves. The Order's light doctrine teaches that the mandate exists for the people — not the other way around. The faith demands that this dynasty demonstrate that it governs justly even when justice is costly.

The test requires the following acts, all completed within a 10-cycle window:
1. Justice Declaration — the dynasty must resolve 2 active grievances in the Political Events interface (if none are active, the system generates 2 test-specific grievances: one internal population dispute and one border tension with a neutral party). Resolving grievances has a gold cost (50 gold per grievance) and a choice — resolve justly (costs more resources but increases happiness and conviction) or resolve expediently (cheaper but no conviction benefit). The test requires both grievances resolved justly.
2. The Mandate Wall — the dynasty must construct a section of defensive wall protecting at least one allied settlement or neutral population zone that is not their own primary settlement. This extends protection to others, not just themselves. Cost: 40 gold / 60 stone per wall segment, minimum 3 segments.
3. Iron Legionary Protection — assign at least 6 Iron Legionary units to patrol a defined territory border for the full 10-cycle window. These units cannot be reassigned during the window. They are committed to the mandate of protection.
4. The dynasty must not impose any economic penalties, tribute demands, or rationing on its own population during the window.

Time window: 10 cycles.
Other players notified: Yes — "The Iron Spire demands that [Dynasty Name] uphold the Mandate. Law is being tested." Other dynasties observe the Mandate Wall construction and the deployment of Iron Legionaries.

Success reward:
- Unlocks The Mandate (apex Level 5 unit) training at The Iron Spire.
- Mandate Authority passive: Iron Legionary units gain the ability to suppress conviction corruption in territories they patrol — their presence slowly pulls conviction toward Moral in any zone they occupy.
- Justice Mandate: the dynasty can issue one binding Order edict per 15 cycles that all allied neutral parties in the region must respect (no mechanical enforcement — but refusal triggers a conviction penalty on the refusing party if they are Order-adjacent).
- The Mandate Wall segments built during the test become permanent fortifications with +50% durability compared to standard wall segments.
- Population conviction pulled +15% toward Moral conviction permanently.

Failure penalty:
- Faith intensity drops from Apex to Active (a 60-point reduction).
- The grievances resolved expediently (or unresolved) create a Law Crisis: the political events system generates 3 additional grievances immediately, each requiring resolution.
- Iron Legionary units lose the Formation Discipline passive for 6 cycles — they fight as individual units rather than organized formations.
- A Mandate Broken event fires: all neutral parties in the map region lose trust in the dynasty, making diplomatic actions cost 20% more gold for 10 cycles.

Narrative framing:
The Living Mandate test is visible to everyone in the region as an exercise of organized, principled authority. Iron Legionaries march to borders and hold position. Judges are dispatched from the Hall of Mandate to settle disputes in the streets. The Mandate Wall rises in places other than the dynasty's own home — a deliberate act of protection extended outward. Rival dynasties observe this and must decide: is this dynasty becoming a regional power through law and protection, making it politically costly to attack? Or is its army spread thin across patrol commitments, creating a military window? The Order's test is simultaneously the most visible act of strength and the most tempting moment of apparent weakness.

Strategic implications:
The Light Doctrine Order test distributes military assets into patrol roles during the window, thinning the army available for rapid response. This is the exploit vector for rivals. However, the Mandate Wall construction during the test creates fortifications that will make the dynasty harder to attack after the test succeeds. Players who understand the test rhythm will position Iron Legionaries in patrol routes that also serve as defensive lines — the patrol requirement and the defensive deployment can overlap if planned correctly. The gold cost for the grievances (100 gold minimum) is the economic pressure; dynasties running low on gold mid-game must ensure their Treasury has reserves before the test fires.

---

## THE ORDER — DARK DOCTRINE COVENANT TEST

**The Covenant Test of the Iron Judgment**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The Iron Spire) is standing. Faith intensity is at Apex (80%+). Dark Doctrine must have been declared at Temple construction.

The Demand:
The Iron Spire's dark doctrine does not speak of protection. It speaks of enforcement. Law is not offered — it is imposed. The mandate is absolute, and those who defy it are made examples. The Order demands that this dynasty prove that its law has teeth, and that defiance has consequences.

The test requires the following acts, all completed within a 12-cycle window:
1. Judgment Declaration — the dynasty must designate a Judgment Target: one rival dynasty or one map-neutral faction. The Judgment Target is announced publicly to all players.
2. Enforcement Campaign — the dynasty must destroy at least 5 buildings belonging to the Judgment Target. Any building type qualifies. Warden-Enforcer units are required for at least 3 of the 5 destructions (they must deliver the killing blow on the building).
3. Iron Decree — the dynasty must impose tribute on at least 2 map settlements it controls or has subjugated. Each tribute imposition costs the affected settlement 20 gold per cycle (transferred to the dynasty). Tribute impositions must be maintained for the full remainder of the test window once imposed.
4. No Mercy Policy — during the test window, the dynasty cannot release any captured enemy units, cannot negotiate ceasefire agreements, and cannot accept diplomatic overtures from the Judgment Target. All such attempts must be rejected.

Time window: 12 cycles.
Other players notified: Yes — "[Dynasty Name] has issued Iron Judgment against [Target]. The Spire demands enforcement." The target is publicly named, as with the Old Light Inquisition.

Success reward:
- Unlocks The Mandate (apex Level 5 unit) training at The Iron Spire.
- Iron Law passive: all buildings owned by the dynasty gain +25% structural durability (they are harder to destroy permanently).
- Enforcer's Writ: once per match, the dynasty can declare one economic resource zone as Iron Territory — no other dynasty may extract resources from it without triggering an automatic Judgment Declaration against them.
- Tribute Empire: all active tribute impositions generate 25% more gold per cycle (upgraded from the base rate).
- Population conviction pulled +20% toward Cruel conviction permanently.

Failure penalty:
- Faith intensity drops from Apex to Devout (a 40-point reduction).
- The Judgment Declaration stands but goes unfulfilled — the dynasty is publicly seen as issuing threats it cannot deliver on. All other dynasties gain a 10% diplomatic leverage bonus against this dynasty for 15 cycles.
- Warden-Enforcer units gain the Disillusioned penalty for 6 cycles — their combat stats drop by 20% as they question the mandate they serve.
- The tribute impositions are automatically lifted — subjugated settlements resist the collection.

Narrative framing:
The Iron Judgment test is theater as much as warfare. The Spire sends its declaration across the map, and every court receives it. The Warden-Enforcers march in formation — not as soldiers but as officers of a law they enforce by force. Buildings fall not by siege but by deliberate destruction: the law has judged this structure unworthy of standing. Other dynasties watch and make their calculations. Is the Order becoming too powerful to be allowed to complete this test? Or is the Judgment Target the real threat, and this enforcement action the right outcome? The dark doctrine of the Order is persuasive precisely because it is organized — this is not chaos. This is terrifyingly structured violence.

Strategic implications:
The Dark Doctrine Order test is the most politically observable test in the game. The public naming of a Judgment Target invites coalition responses — the target will seek allies, and other dynasties will assess whether they want to be next. The 12-cycle window requires decisive military action from the first cycle. The No Mercy Policy is the constraint that creates tension — the dynasty cannot negotiate during the window, which means if a rival launches a surprise attack while the Enforcement Campaign is ongoing, the dynasty must fight on two fronts without the option of buying time through diplomacy. Players must secure their home before beginning the Enforcement Campaign.

---

## THE WILD — LIGHT DOCTRINE COVENANT TEST

**The Covenant Test of the Living World**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The First Grove) is standing. Faith intensity is at Apex (80%+). Light Doctrine must have been declared at Temple construction.

The Demand:
The First Grove remembers when the world was entirely alive. The Wild's light doctrine does not ask for conquest or sacrifice — it asks for harmony. The faith demands that this dynasty demonstrate that it has not consumed the land it inhabits, but tended it.

The test requires the following acts, all completed within a 10-cycle window:
1. Grove Renewal — the dynasty must plant at least 3 Grove Markers in new locations not previously used (these are the Tier 1 faith structures, reused here as a renewal mechanic). Each Grove Marker placed during the test generates +5 faith intensity immediately and marks a new sanctified zone.
2. The Stewardship Audit — the dynasty must not have destroyed any forest, grassland, or river terrain features since the match began. The game tracks terrain modification — if the dynasty has consumed more than 30% of available natural terrain in their zone, they fail the Stewardship Audit automatically. (This requires The Wild player to actively avoid terrain overconsumption throughout the match, not just during the test window.)
3. Offer of Harmony — the dynasty must make a non-aggression offering to at least 1 wild animal map faction (if present on the map) or, if none are present, must make a diplomatic Harmony Offering to one rival dynasty — a gift of 50 food and 25 gold that must be accepted or rejected by the recipient. If rejected, the Offer of Harmony fails and the dynasty must find an alternative within the remaining window.
4. Thornrider Ceremony — at least 4 Thornrider units must be stationed at The First Grove for the final 3 cycles of the window. They cannot be reassigned during those 3 cycles.

Time window: 10 cycles.
Other players notified: Yes — "The First Grove stirs. [Dynasty Name] seeks the covenant of the Living World." Other dynasties observe the new Grove Markers appearing and the withdrawal of Thornriders to the sacred site.

Success reward:
- Unlocks The First Wild (apex Level 5 unit) training at The First Grove.
- Living World passive: all Grove Markers on the map generate 2 food per cycle (the land feeds those who tend it). Combined with multiple Grove Markers, this creates a distributed food income that does not rely on Farms.
- Harmony's Call: once per match, the dynasty can call all neutral wildlife map entities to their side as temporary combat allies for 3 cycles.
- All Thornrider units gain the Terrain Affinity passive permanently: they move at full speed through forest and difficult terrain (instead of the normal movement penalty).
- Population conviction pulled +15% toward Moral conviction permanently.

Failure penalty:
- Faith intensity drops from Apex to Fervent (a 20-point reduction).
- The Stewardship Audit failure (if applicable) additionally generates a Terrain Scar event — the dynasty's primary settlement zone suffers reduced Farm output by 20% for 8 cycles as the land registers the dynasty's overconsumption.
- Thornrider units become Unsettled for 5 cycles — they cannot initiate charges, which removes their primary combat strength.
- A Grove Rejection event fires: The First Grove goes dormant, generating no faith intensity for 4 cycles.

Narrative framing:
The Wild's light test is quiet and spreading. New Grove Markers appear across the land — sanctified spots that shimmer in ways other players' scouts notice but cannot explain. Thornriders walk their mounts to the First Grove and stand in silence. The Harmony Offering is sent across the border — a basket of food and gold, carried by a rider with no weapons. Other dynasties see all of this and must interpret it. Is this generosity? Is this vulnerability? The wild is not passive — it is watching. The land is recording everything. Other dynasties who have ravaged their terrain observe with something like envy: the dynasty of The Wild has left their world alive.

Strategic implications:
The Living World Covenant Test is the only test with a retroactive condition — the Stewardship Audit checks the dynasty's terrain behavior throughout the entire match, not just during the 10-cycle window. This means Wild Light players must play the entire game with terrain preservation as an active consideration. This is a meaningful constraint that shapes early economic decisions: Wild players cannot clear-cut forest for Lumber Camps without consequence, which means they must find alternative wood supply strategies. The withdrawal of Thornriders for the final 3 cycles is the tactical vulnerability — opponents who know the test is occurring can time a strike to hit when the cavalry is ceremonially pinned at the Grove.

---

## THE WILD — DARK DOCTRINE COVENANT TEST

**The Covenant Test of Nature's Indifference**

Trigger: Dynasty reaches Level 4 progression. Grand Sanctuary (The First Grove) is standing. Faith intensity is at Apex (80%+). Dark Doctrine must have been declared at Temple construction.

The Demand:
The First Grove in its dark doctrine does not speak of harmony. The Wild is not kind. The forest does not mourn what it consumes. Nature is not indifferent to suffering — it is built from suffering. The dynasty that serves the dark Wild must prove it understands this: strength inherits the earth. Weakness is compost.

The test requires the following acts, all completed within a 12-cycle window:
1. Consumption Campaign — the dynasty must destroy at least 3 enemy economic buildings (Farms, Quarries, Lumber Camps, Animal Ranches, or Iron Mines). The destruction must be performed primarily by Bear Rider or Bonebreaker units — at least 2 of the 3 destructions must use these unit types as the primary attacking force.
2. Territory Consume — the dynasty must claim at least 1 rival territory zone entirely — all buildings in the zone must either be captured or destroyed. No structure may remain under rival control in the claimed zone. This zone becomes Wild territory.
3. The Cull — within the dynasty's own population, the lowest-happiness population district must be Culled — 5 population units from that district are removed (killed). The Cull is a deliberate internal sacrifice representing the doctrine that weakness within the dynasty is as unacceptable as weakness in enemies. The removed population cannot be recovered.
4. First Grove Dominance — at the conclusion of the window, the dynasty must have the highest army size (total unit count) of any dynasty on the map. If another dynasty has more units at the window close, the test fails.

Time window: 12 cycles.
Other players notified: Yes — "The First Grove demands proof of strength. [Dynasty Name] walks the dark path of the Wild." All dynasties see the Bear Riders mobilizing and the consumption campaign beginning.

Success reward:
- Unlocks The First Wild (apex Level 5 unit) training at The First Grove.
- Natural Predator passive: Bear Rider and Bonebreaker units gain +25% attack and +15% movement speed permanently. They are apex predators — they no longer have any terrain movement penalty anywhere on the map.
- Consume the Weak: any enemy unit killed by a Bear Rider or Bonebreaker generates 1 food resource (the natural cycle completes). This creates a self-sustaining food generation loop during prolonged military engagement.
- Grove Dominance: the claimed territory from the test generates resources at 110% normal output — the Wild has marked it as thriving, and the land responds.
- Population conviction pulled +25% toward Cruel conviction permanently — the strongest conviction pull of any covenant test.

Failure penalty:
- Faith intensity drops from Apex to Active (a 60-point reduction).
- The Cull happens regardless (population units were already removed) — but without the success rewards, this is purely a loss.
- Bear Rider and Bonebreaker units become Feral for 6 cycles — they cannot receive orders and move autonomously based on proximity to enemies. This is extremely dangerous in a complex tactical situation.
- A Weakness Exposed event fires: all rival dynasties gain +5% combat bonus against this dynasty's units for 8 cycles, as the test failure signals that the dynasty was found wanting by its own god.

Narrative framing:
The dark Wild test is the most primal event in the game. Bear Riders crash through enemy farmland. Bonebreakers shatter walls that took months to build. From The First Grove, the earth does not weep for what is consumed — it absorbs it. The Cull within the dynasty's own ranks is the moment other dynasties' spies report with confusion: why did they kill their own? The answer is the doctrine. Weakness anywhere is weakness everywhere. The dynasty that survives this purge intact is terrifying not despite the Cull but because of it — they proved they would turn their strength on themselves before they would tolerate softness. The First Wild, when it walks at the end of this, is the embodiment of a world that was always this honest.

Strategic implications:
The Dark Wild test is the most aggressive total-war test, comparable to the Blood Dominion dark test in its lethality but different in structure. The army size requirement at the window's close is the unique constraint — it means the dynasty must fight aggressively (destroying buildings, claiming territory, Culling internally) while also maintaining the largest force on the map. These two goals are in tension: military operations consume units, and maintaining largest force requires either training replacements faster than losses or avoiding heavy casualties. The Feral failure penalty is unpredictable and potentially catastrophic in a late-game tactical situation — Bear Riders and Bonebreakers roaming without orders can trigger coalition responses from neighboring dynasties.

---

# DESIGN NOTES: BUILDING ECONOMY PHILOSOPHY

## Resource Scarcity as the Primary Decision Engine

The building technology tree in Bloodlines is designed so that resource scarcity is not a temporary early-game problem that resolves as the match progresses. It is the permanent condition of play. In the early game, wood and stone are the binding constraints — players cannot build everything they need, and every building delays another. In the mid-game, stone and iron become the primary contest. Iron is specifically designed to be scarce in absolute terms: even a fully operational Iron Mine produces only 6 iron per cycle, and a Deep Iron Mine produces 14. A single Heavy Swordsman costs 3 iron to train. A Ballista costs 5. A Grand Armory costs 40 iron to build. A Grand Sanctuary costs 50 iron. Players are constantly in an iron deficit state, and every iron expenditure is a priority decision. This is intentional. Iron scarcity forces players to commit to specific military doctrines — you cannot field elite cavalry, siege equipment, and heavy infantry simultaneously with one Iron Mine. You must choose a primary military identity and accept its limitations.

## The Mid-Game Competition Between Military and Faith

The deepest tension in the technology tree is the competition between military and faith construction in the Level 3-4 window. The Temple requires 80 stone and is built at the same time players are constructing Armories (80 stone, 20 iron) and Fortified Keeps (100 stone, 20 iron). Players cannot build all three without a Stone Works and significant quarry investment established well in advance. This means the mid-game forces a real and painful choice: invest in faith infrastructure now to accelerate doctrine path and faith unit availability, or invest in military infrastructure to have combat power for the territory wars of Level 3. Players who split the difference — building Temple and Armory simultaneously — will have weaker versions of both. Players who commit fully to one path will be strong in that dimension and vulnerable in the other. This creates the strategic identity divergence that makes dynasties feel distinct from one another even within the same match.

## Grand Structures as Match-Defining Events

The Grand Sanctuary and Grand Harbor are designed to feel like turning points rather than natural extensions of the technology tree. Their costs (300 gold, 250 stone, 50 iron for the Grand Sanctuary; 280 gold, 200 wood, 180 stone, 60 iron for the Grand Harbor) exceed anything a dynasty will have accumulated without deliberate economic preparation. The Grand build time (8-12 minutes) means the entire map knows it is happening and has a window to respond. The decision to begin a Grand structure is a declaration — it announces the dynasty's strategic intent, commits their economy to a single outcome, and creates a period of vulnerability during construction when military defense must compensate for economic suspension. Grand structures should feel like the dynasty betting everything on a belief. That belief should be either vindicated or punished by the match outcome.

## The Covenant Test as the Economic Culmination

The Covenant Test does not stand apart from the building economy — it is the culmination of it. Tests require specific structures (Covenant Hearth, Sanctuary Beacon, Inquisitor's Post, Mandate Wall), specific resources (food for Blood Dominion light, gold for Order light, stone for multiple tests), and specific unit deployments that take military capacity offline during the window. This means players must plan for the Covenant Test not just as a spiritual moment but as an economic event. A dynasty that enters the Covenant Test window without adequate food reserves will fail the Blood Dominion light test. A dynasty that enters without Iron Legionaries trained will fail the Order light test. The test is the exam for which the entire game has been the study. The building tree is the curriculum.

---

*PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON*
*(Design Content — 2026-03-18, Creative Branch 002)*
