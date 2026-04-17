# Bloodlines — Naval System

Naval warfare in Bloodlines is not a separate game. It is a strategic extension of the land-based civilization into a dimension that changes the meaning of territory, trade, and military projection. A dynasty that controls the seas controls the connections between all land-based powers. A dynasty that ignores the seas accepts a permanent constraint on what it can project and protect.

### Added 2026-03-18, Fifth Session

---

## Design Philosophy

**The sea is a highway and a weapon.** Trade moves faster by sea than by land. Armies can be projected to distant shores without marching through hostile territory. Supply lines across sea routes are efficient when secure and catastrophic when disrupted.

**Naval capability is not universal.** Only dynasties with Coastal Zone territory can build and operate naval forces. A landlocked dynasty must either conquer coastal territory or accept permanent naval exclusion. This creates a strategic asymmetry that shapes how landlocked and coastal dynasties approach the map differently.

**The sea is not safe.** Unlike land territory, the sea does not belong to anyone. A fleet at sea is always exposed. Naval engagements are faster and more decisive than land battles — fleets don't hold ground, they control routes.

---

## Naval Access Requirements

To build and operate any naval unit, a dynasty must:

1. Control at least one Coastal Zone territory
2. Have built a **Harbor** building in that Coastal Zone (new building — added to Economic category, as it is primarily a trade and logistics infrastructure)
3. Have sufficient wood and iron for ship construction

A dynasty that loses all its Coastal Zone territory loses the ability to build new ships. Existing ships survive but cannot be repaired, resupplied, or replaced.

---

## The Harbor Building

**Category:** Economic (supports trade) / Military (supports naval warfare)
**Prerequisites:** Coastal Zone territory, Market built, Level 2 minimum
**Function:** Enables ship construction, naval unit training, sea trade route establishment, and fleet management
**Upgrade tiers:**
- Basic Harbor: Small vessels, coastal patrol, fishing fleet bonus to food production
- Expanded Harbor: War galleys, troop transports, sea trade route capacity doubled
- Grand Harbor: Capital ships, full naval warfare capability, long-distance trade route access

---

## Naval Unit Types

---

### Fishing Vessel

Not a military unit. The Fishing Vessel is an economic unit that operates automatically from a Basic Harbor and supplements food production for Coastal Zone territories. It cannot engage in combat. It can be sunk by enemy naval units during blockades.

**Purpose:** Food supplementation, passive economic presence at sea
**Requirement:** Basic Harbor

---

### Scout Vessel

Light, fast, single-masted ship designed for intelligence gathering at sea. Cannot fight a war galley and survive. Exceptional for route reconnaissance, monitoring enemy fleet movements, and probing harbor defenses.

**Role:** Naval reconnaissance and harassment
**Strength:** Speed (fastest naval unit); can evade most combat vessels; excellent sightlines at sea
**Weakness:** No combat capability against anything heavier; one hit from a war galley sinks it
**Requirement:** Basic Harbor, Expanded Harbor preferred

---

### War Galley

The backbone of mid-game naval warfare. Oar-propelled, iron-reinforced, capable of both ramming and boarding. A fleet of war galleys controls a sea route. A dynasty without war galleys cannot challenge one that has them.

**Role:** Naval combat, sea route control, harbor assault
**Combat mechanics:**
- Ramming attack: High damage on first contact, requires room to build speed
- Boarding attack: After ramming or grappling, infantry from the galley board the enemy vessel — naval combat becomes ground combat on a moving platform
- Archer support: War galleys carry Bowmen or Crossbowmen who fire during approach
**Strength:** Dominant against smaller vessels; effective against fortified harbor positions; can carry infantry for coastal raids
**Weakness:** Slow against strong winds; vulnerable to fire attack; poor in shallow coastal waters where larger vessels run aground
**Requirement:** Expanded Harbor, iron (reinforcement), wood (construction)

---

### Troop Transport

A wide-hulled vessel built to carry land armies across water, not to fight naval battles. The Troop Transport is what makes amphibious operations possible. Without Troop Transports, armies cannot cross significant bodies of water.

**Role:** Amphibious military projection
**Capacity:** Up to one full land army unit per Transport
**Combat:** No offensive capability; very difficult to sink with small vessels; vulnerable to War Galley attack
**Strategic importance:** A fleet of Troop Transports with War Galley escort can land armies anywhere on a coast — including enemy Coastal Zone territories, bypassing all land-based fortifications and terrain
**Requirement:** Expanded Harbor, wood (heavy), iron (reinforcing)

---

### Fire Ship

A one-use weapon. A sacrificed vessel loaded with flammable material and piloted into enemy ship formations or harbor structures. On detonation, it creates a spreading fire effect that damages multiple nearby vessels and can set harbor infrastructure ablaze.

**Role:** Anti-fleet weapon, harbor disruption
**Use case:** Concentrated enemy fleet formations; Harbor defense against blockading fleets; disrupting a Troop Transport convoy before landing
**Cost:** The vessel and its crew are consumed. This is a sacrifice mechanic — a Born of Sacrifice parallel in the naval context.
**Conviction interaction:** Using Fire Ships against a fleet carrying non-combatant Troop Transport passengers shifts conviction toward Cruel if those aboard are known to include civilian populations.
**Requirement:** Basic Harbor (production), Expanded Harbor recommended

---

### Capital Ship

The apex naval unit. A massive, multi-deck war vessel with heavy iron reinforcement, multiple siege weapon mounts (ballistae and catapults), and crew capacity for a large fighting force. One Capital Ship can engage and defeat several War Galleys simultaneously. A fleet anchored around Capital Ships is the most powerful naval force in the game.

**Role:** Naval dominance, siege support from sea, fleet anchor
**Combat:** Onboard siege weapons (ballistae, catapults) allow engagement at range before ramming or boarding; can bombard coastal fortifications and harbor structures from sea range; cannot be rammed effectively — the hull is too massive
**Weakness:** Slow; cannot navigate shallow waters at all; requires deep harbor (Grand Harbor minimum) to dock and resupply; extremely expensive to build and maintain in iron and wood
**Requirement:** Grand Harbor, very high iron and wood investment
**Design note:** Capital Ships exist in small numbers. A dynasty that fields two Capital Ships is exceptional. Three is near-unprecedented. These vessels are named, tracked, and their loss is felt.

---

## Naval Tactical Mechanics

### Sea Route Control

A sea route is a path between two ports across open water. Sea routes are not controlled by occupying them in the land-territory sense — they are controlled by maintaining naval presence along them.

A fleet that can intercept ships along a route effectively controls that route. Controlling a sea route means:
- Enemy trade ships using the route can be seized or sunk
- Enemy Troop Transports using the route can be intercepted before landing
- The controlling dynasty's own trade route generates full income

### Blockade

A blockade is the sustained naval investment in preventing access to an enemy harbor. A successful blockade:
- Prevents enemy ship construction from continuing (ships in construction are halted)
- Stops the enemy's sea trade routes generating income
- Prevents resupply of enemy ships at sea
- Cuts Coastal Zone food production (Fishing Vessels cannot operate under blockade)

**Blockade requirements:** The blockading fleet must maintain presence in the blockade zone. If the blockading fleet is driven off — even temporarily — the blockade breaks and must be re-established.

**Conviction interaction:** A blockade that starves a civilian coastal population over time shifts conviction toward Cruel. A blockade that is purely military (targeting ships, not fishermen) carries no conviction cost.

### Amphibious Operations

When a dynasty lands armies on an enemy coast via Troop Transport:

- The landing force disembarks over one game cycle (vulnerability window)
- During disembarkation, the force is at reduced combat capacity
- Once landed, the force fights as normal land units in whatever terrain the coast presents
- The Troop Transports remain offshore — if they are sunk during the operation, the landed army has no retreat path

**Amphibious operation significance:** This is the most surprising form of military projection in the game. A landlocked enemy — one who has invested entirely in land fortifications — has no defense against an amphibious landing if they have no naval forces of their own. The landing bypasses all terrain advantages, all road networks, and all land fortifications. The only counter is naval.

### Weather

Sea conditions affect naval operations. Heavy weather:
- Reduces all ship movement speed
- Increases damage taken by non-Capital ships
- Prevents disembarkation from Troop Transports (the sea is too rough to safely land)
- Makes Scout Vessel reconnaissance unreliable

**Wild faith interaction:** The Wild covenant at high faith intensity has access to weather-influence mechanics. A Wild dynasty at peak faith can influence weather events — at sea, this means the potential to summon storms that target enemy fleets specifically.

---

## Naval Trade Routes

Sea trade routes are faster and higher-volume than land trade routes. Once a Harbor is established and a sea trade route formalized between two dynasties:

- Gold income from the route exceeds the equivalent land trade route by a significant multiplier
- The route operates passively — no recurring maintenance action required except naval protection
- The route can be disrupted by blockade or by sinking the trade ships

**Strategic importance of sea trade:** A dynasty pursuing the Economic Victory path through currency dominance benefits dramatically from sea trade routes — the volume of currency circulation that sea routes enable accelerates the adoption of their currency standard. Goldgrave's natural economic affinity and any coastal territory make the combination potent.

---

## Naval Faith Mechanics

**The Wild:** The sea is within The Wild's domain — it is a force of nature, ancient and powerful. At high Wild faith intensity:
- Naval units move faster in rough weather (they are in harmony with it rather than fighting it)
- Weather-influence events have naval expressions
- Sea creatures may respond to Wild rituals — a Wild dynasty at apex faith can summon creatures from the deep as a one-time naval disruption event

**Blood Dominion:** The sea has a sacrificial history in Blood Dominion doctrine — the deep water as a place of offering. At high Blood Dominion intensity, Fire Ships become more powerful (the fire itself is consecrated and burns longer). Naval raiding missions produce additional resource returns.

**Old Light:** The sea as a place of passage and connection. Old Light at high intensity generates navigation bonuses — Scout Vessels are more effective, Troop Transport landings are safer and faster. The Old Light has a historical relationship with lighthouse traditions — harbor defense structures benefit from faith intensity.

**The Order:** The sea as a route to administer. The Order at high intensity generates sea trade route bonuses — they are better at formalizing and protecting trade than any other covenant. The Order's naval doctrine emphasizes capital ship dominance — structured, overwhelming force over the chaos of War Galley skirmishing.

---

## Naval Victory Interactions

**Economic Victory:** Sea trade routes are a primary accelerant. The dynasty that controls the major sea routes between all coastal territories is positioned to convert trade dominance into currency dominance faster than any land-based economic strategy.

**Military Conquest:** Amphibious operations unlock the ability to attack from directions the enemy cannot fortify against. A military conquest path that includes naval capability is qualitatively different from a purely land-based campaign.

**Faith Victory:** Coastal Sacred Ground sites can only be accessed by dynasties with naval capability. Certain faith intensity advantages exist only in or near Coastal Zones. A Faith Victory that includes the sea dimensions of faith is stronger than one confined to land.

**Territorial Governance:** A dynasty pursuing voluntary territorial integration benefits from sea trade routes as proof of economic benefit — populations in coastal territories adjacent to a prosperous trade network see what joining the dynasty's economic system would mean.

---
