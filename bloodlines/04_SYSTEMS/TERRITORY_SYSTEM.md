# Bloodlines — Territory System

## Overview
Territory defines the physical space over which factions compete. Land is not just a number. Each territory has characteristics that affect resource production, population capacity, defensive value, and strategic importance. Controlling territory is the visible expression of a faction's power.

## Design Intent
Territory creates the spatial dimension of strategy. Players must decide where to expand, what to defend, and what to abandon. Territory connects the population system (people live on land), the resource system (land produces resources), and the military system (armies fight over land). Territorial control is one of the primary victory conditions.

## Open Design Questions
- How is the map structured? (Hex grid, province system, point-based?)
- What terrain types exist and how do they affect gameplay?
- How is territory claimed, contested, and lost?
- Can territory be fortified? How does fortification work?
- What is the relationship between territory size and governance difficulty?
- Are there neutral territories, contested zones, or sacred sites?
- How does territory affect faith spread and conviction?

---

*Detailed mechanics will be appended here as design sessions explore this system.*

### Design Content (Added 2026-03-15)

Territory control requires both military control and population loyalty. A region held only by force is unstable and may revolt.

**Two-Part System:**
1. Military control
2. Population loyalty

**Map Structure:** The map is divided into territories. Each territory contains resources, population, infrastructure, and a loyalty level.

**Loyalty Effects:** If loyalty declines, tax revenue decreases, recruitment becomes difficult, sabotage risk increases, and rebellions may occur.

**Control Factors:**
- Military presence
- Local loyalty
- Infrastructure
- Faith influence

**Loyalty Maintenance Methods:**
- Governance structures
- Faith influence
- Economic prosperity
- Dynasty reputation

**Late-Game Concept — Voluntary Integration:**
In late-game territorial governance paths, smaller territories may choose to join the dynasty willingly due to prosperity and infrastructure. Territory control is not only achieved through conquest but also through governance attraction.

**Civic Buildings:**
- Settlement Hall — establishes control and collects taxes
- Housing District — expands population capacity
- Well — provides water supply
- Granary — stores food reserves

---

### Player Manual Expansion (Added 2026-03-15)

**Loyalty Collapse Dynamics:** Loyalty collapse does not always arrive as a dramatic event. It can accumulate quietly: families less enthusiastic about contributing sons, sabotage more frequent, tax contribution weaker, then a captured heir or failed campaign adds shock, then heavy-handed behavior without legitimacy turns pressure into fracture.

**Territorial Governance Victory Expansion:** Not just map control through conquest. Includes late-game dynamic where smaller territories begin joining willingly and unprovoked, seeking prosperity and benefits of exceptional infrastructure and stability. Infrastructure, governance, and prosperity become attractors converting independent territories into voluntary integration. Territory and loyalty must both be satisfied for true control.

**Expansion vs Consolidation:** Expand when the next gain makes the realm stronger faster than it makes it more fragile. Consolidate when the current structure would bend under additional responsibility. Bloodlines punishes land greed detached from governance.

**Defensive Play as Viable Ideology:** Defensive build style is viable, potentially tied to populism, nationalism, and internal loyalty. A defensive dynasty can build power through fortification, high loyalty, coherent governance, population stability, and national feeling. A stable dynasty that cannot be broken often becomes the dynasty others must deal with on its terms.

---

### Full System Design Expansion — Sessions 6-9 (Added 2026-03-19)

## Map Architecture — Three-Layer System (SETTLED, Sixth Ingestion)

The map operates through three simultaneous layers. Understanding which layer you are operating in determines how rules apply.

**Layer 1 — Terrain Base:** The physical geography the player sees and moves through. Massive continuous terrain with direct unit control in the Warcraft 3 tradition. The ten canonical terrain types (Reclaimed Plains, Ancient Forest, Stone Highlands, Iron Ridges, River Valleys, Coastal Zones, Frost Ruins, Badlands, Sacred Ground, Tundra) exist in this layer. Movement, line of sight, resource node positions, and combat terrain bonuses operate here.

**Layer 2 — Province Overlay:** Province regions define territory/loyalty zones for governance, conviction, and diplomatic systems. Provinces map onto the terrain layer using terrain features as natural boundaries — rivers divide provinces, highlands separate them, roads connect them. A dynasty controls a province when it holds both military control and sufficient population loyalty. Province boundaries are visible on the strategic map view and invisible in the tactical viewport.

**Layer 3 — Boundary Layer:** An underlying tile or hex structure that handles territory boundary calculations, faith spread computations, loyalty diffusion, and resource yield tracking. Players never interact with this layer directly — it is the calculation engine beneath the other two.

## Territory Control — Mechanics

Claiming territory is not instantaneous. A dynasty that occupies a province militarily begins a control clock. The time required to achieve functional control depends on: the province's prior loyalty (neutral, allied to a different dynasty, actively hostile), the infrastructure present (a Settlement Hall accelerates control), the current occupying dynasty's conviction posture (Moral conviction reduces resistance, Cruel conviction accelerates fear-compliance), and whether a Bloodline Member with a Defense Commitment is stationed in the province.

Control percentages govern what the province actually produces. A province at 30% control produces a fraction of its resource output, provides partial tax revenue, and contributes minimal recruitment capacity. A province at 100% control operates at full efficiency and begins generating loyalty overflow that can spread to adjacent provinces.

**Contested provinces** sit between two dynasties' claimed borders. Both dynasties exert influence. Population in contested provinces experience elevated anxiety, reduced economic output, and increased faith sensitivity — they often embrace or reject a specific covenant as a means of identity anchoring when political identity is unstable.

## Faith Influence on Territory

Faith spreads through territory like a slow-moving resource. A dynasty with high faith intensity in its home territory generates faith pressure that diffuses outward through adjacent provinces over time. The rate of diffusion depends on population movement, caravan traffic, and whether faith buildings exist in the territory receiving the influence.

Sacred Ground provinces amplify faith intensity dramatically for any shrine placed within them. Multiple faiths can compete for the same Sacred Ground — the faith with the strongest presence wins the intensity bonus, but the competition itself generates political event triggers.

Faith influence on territory affects governance: a province whose population has converted to a faith the ruling dynasty does not practice generates loyalty friction. The dynasty can resolve this through tolerance (allow the population to maintain their faith, generating a positive conviction shift) or through conversion pressure (attempt to bring the population to the ruling faith, with risk of resistance events).

## Territorial Governance Victory Path — Mechanics

The Territorial Governance victory requires genuine development of every province under the dynasty's control, including all tribal territories. "Development" is not military occupation — it is the full expression of governance: loyalty above 80%, infrastructure at settlement tier or higher, active economic production, and a population that has had time under stable governance to develop organic attachment to the dynasty's rule.

The difficulty of this path scales with territory size. A small dynasty holding ten provinces faces a governance challenge. A large dynasty holding forty has created a governance crisis even before enemy action is considered. Revolts are constant. Newly acquired territories resist. Tribal lands are the hardest — their populations have managed their own affairs for decades and do not transfer loyalty easily.

What makes this path significant when achieved: every corner of the known world has accepted this dynasty's authority not because it feared the alternative but because it chose this dynasty. This is the only victory that requires the consent of the governed.

## House-Specific Territory Notes (CB004 — PROPOSED)

**Stonehelm:** Defensive structures built 20% faster. Stonehelm provinces develop fortification infrastructure at the highest rate of any house. Once fortified, Stonehelm territory is the most difficult to take by force in the game.

**Whitehall:** Territory integration 25% faster. Whitehall's administrative competence means newly acquired provinces reach functional control more quickly. Their governance identity builds on what already exists rather than replacing it.

**Hartvale:** Population recovery 25% faster in Hartvale-held territory. Population losses from war, famine, or event culling regenerate more quickly. This translates into faster recruitment recovery and faster economic rebound after disasters.

**Oldcrest:** Territory loyalty decay 20% reduced. Oldcrest's depth and age translates into populations that, once integrated, stay integrated. Their loyalty is slower to acquire but more durable under pressure.
