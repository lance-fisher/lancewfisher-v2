# Bloodlines — World Index

Geography, regions, terrain types, and map design documentation.

## Regions
> Awaiting first design session content.

## Terrain Types
> Awaiting first design session content.

## Strategic Locations
> Awaiting first design session content.

---

*World geography is documented as it is designed.*

---

# World Generation Parameters

### Added 2026-03-19, Sixth Session

---

## Part I: Generation Philosophy

The world generation system in Bloodlines exists to make every match a different game. Not superficially different — the same game with a different layout — but structurally different in ways that reshape the story the match will tell. Two players can choose the same house, adopt the same faith, and pursue the same grand strategy, and across two different generated maps they will face different decisions, different neighbors, different paths to power, and different crises. The generation system is not a random number applied to a static template. It is a set of geographic and historical logic rules that produce a plausible world, and plausible worlds create plausible emergent histories.

The generation system produces three things simultaneously. It produces a physical geography: terrain, rivers, elevation, coastlines, and the relationships between terrain types that make geographic sense. It produces a historical residue: the placement and density of Frost Ruins, the state of recovered land versus still-scarred land, the sacred sites that the old world's faiths built before everything collapsed. And it produces a political starting condition: starting positions, resource access, natural borders, and the proximity relationships that will define the first phase of contact between dynasties.

These three outputs are inseparable. The physical geography shapes where the historical residue appears. The historical residue shapes where populations settled during the Reclamation. Where populations settled shapes where dynasties began. The generator does not produce a map and then place players on it. It generates a world, and the players inhabit a position within that world that the world's own logic created.

Replayability in Bloodlines does not come from arbitrary variation. It comes from the fact that the same set of geographic and historical rules applied with different parameters and seeded differently produces radically different strategic landscapes. A match where two dynasties share a long river border will have a completely different diplomatic texture than a match where those same two dynasties are separated by a range of Stone Highlands. A match where the most productive Sacred Ground happens to fall equidistant between three dynasties will generate a three-way faith contest that defines the midgame. The generator cannot predict which stories a given map will produce. It only ensures that every map has the structural conditions for compelling stories to emerge.

The Frost Ruins are not decorative. They are the world's memory, and the generator treats them that way. The Great Frost destroyed what was arguably a more advanced civilization than the one currently recovering. The people of Year 80 live among the bones of something they only partially understand. The generator ensures that the ruins of the old world are always present and always distributed in ways that reflect where the old world actually was: concentrated in former River Valley settlements, appearing in old trade corridors, visible in the landscapes that the Age Before chose to inhabit for the same reasons the Age of Reclamation is choosing to inhabit them now. A dynasty that controls a major Frost Ruin complex controls not just a territory but an inheritance. The ruins carry lore, they carry salvageable infrastructure, and they carry the weight of what was lost. The generation system makes sure that weight is always somewhere on the map, always contested, and always meaningful.

The relationship between terrain and house identity is one of the most important things the generator manages. Each of the nine founding houses has evolved during the seventy years of the Reclamation in conditions that shaped their character. Stonehelm built its identity in Stone Highlands. Ironmark built its identity working iron. Hartvale's identity is shaped by river culture. The generator does not lock houses to their affinity terrain — a Stonehelm player is not required to start in Stone Highlands, and a skilled Stonehelm player can build a formidable dynasty anywhere. But the generator respects affinities when placing starting positions, creating natural fits between house identity and starting terrain that reward players who understand the match between their house and their landscape. And when the generator creates a mismatch — a Hartvale dynasty starting away from rivers, an Ironmark dynasty starting in coastal territory — it creates an adaptation challenge that becomes part of that match's specific narrative. The player's relationship to their terrain is not given; it is earned or discovered.

---

## Part II: Map Scale and Structure

The playable area in Bloodlines operates in abstract game units rather than real-world measurements, because the game is not a simulation of geography but a representation of political and military scale. In concrete terms: on a standard 6-player map configuration, the playable area is large enough that a marching army, moving without combat or supply interruption, would require approximately 45 in-game days to cross the map from one edge to the other. On the largest 10-player Continent configuration, that crossing time extends to approximately 90 in-game days. These distances are not trivial. A player on one side of the map will not feel the consequences of events on the other side of the map for hours of real-time play.

**Map size tiers by player count:**

| Configuration | Human Players | Map Scale Multiplier | Province Count | Approximate Crossing Time |
|---|---|---|---|---|
| Compact | 2-3 players | 0.6x | 80-110 provinces | 25-30 in-game days |
| Standard | 4-6 players | 1.0x | 140-180 provinces | 45 in-game days |
| Grand | 7-8 players | 1.4x | 200-250 provinces | 60-65 in-game days |
| Continent | 9-10 players | 2.0x | 300-360 provinces | 85-95 in-game days |

Province count scales with map size in a roughly linear relationship, but the distribution of provinces is not uniform across the map. The interior zones of the map — where most of the contested territory will eventually concentrate — are province-dense. The frontier regions, particularly the northern Tundra zones and the Badlands buffers, are province-sparse. This is intentional: provinces in the interior matter more strategically, and their density creates more decisions per unit of space. Frontier provinces exist as transition zones and strategic buffers rather than as primary governance targets.

**The Three-Layer Architecture**

The map is built from three distinct layers that operate simultaneously and interact continuously.

The terrain layer is the physical and visual world. It is the layer the player sees when they look at the map: the green of plains and forests, the grey of highlands, the rusted tones of Iron Ridges, the blue of rivers and coastlines, the pale of Frost Ruins. This layer governs movement speed, military effectiveness modifiers, resource production potential, and faith affinity. It is continuous — terrain transitions gradually across the map following geographic logic, not in abrupt tile-sized jumps. Moving from Reclaimed Plains into Ancient Forest is a gradual thickening of the treeline, not a sudden wall.

The province layer is the governance overlay. Provinces are distinct named territories with defined borders, population centers, loyalty ratings, resource production outputs, and diplomatic status. A province's character is determined by the terrain type that composes the majority of its area, but provinces can straddle terrain transitions — a province with a river running through Reclaimed Plains has both the river's water access and the plains' agricultural capacity. Province borders are derived from geographic features wherever possible: rivers form natural western and eastern borders, ridgelines form natural northern and southern borders, and in areas without strong natural features the generator uses proximity mathematics to assign province boundaries. The borders are not invisible administrative lines. They have physical meaning because they follow the landscape.

The boundary layer is the calculation substrate. It manages the hex or tile grid that underlies the province system and provides the mechanical resolution for proximity calculations, line-of-sight calculations, movement path calculations, and territory adjacency for diplomatic and military events. This layer is not directly visible to the player — it is the mechanical engine beneath the visual world. The boundary layer is what the game queries when determining whether a province shares a border, whether a river is crossable at a given point, or whether a forest provides concealment against a given attacking force.

**Province Boundary Logic**

Province boundaries follow a priority hierarchy when the generator places them. Rivers take first priority: a province border follows a river segment wherever a navigable river runs. This means river borders are not arbitrary — they follow the landscape, and crossing a river border is mechanically distinct from crossing a land border. Ridgelines take second priority: Stone Highlands and Iron Ridge terrain generates natural province boundaries where the terrain rises or falls. In flat terrain without rivers or ridgelines, the generator uses minimum-travel-time mathematics to assign boundary lines between adjacent settlement centers, producing Voronoi-style organic borders that have no strong natural correlate but are not arbitrary either.

**The Trueborn City Placement Rule**

The Trueborn City is not randomly placed. It is placed by a constrained algorithm that satisfies three conditions simultaneously. First, it must be accessible from all four cardinal directions via viable movement routes — it cannot sit in a terrain trap that makes it trivially defensible or trivially inaccessible. Second, it must be positioned such that no single starting dynasty has a direct line of march to it without crossing at least one other province that belongs to or is contested by another dynasty. Third, it must sit on or immediately adjacent to a River Valley — the lore establishes that the Trueborn City was built where three river systems converge, and the generator respects this even when it must adjust the river generation to produce the convergence point in a viable central location.

The Trueborn City is never trivially peripheral: it cannot be placed within two province-movements of any map edge. It is never trivially central: it cannot be placed at the exact geographic center of the map. The algorithm targets a zone roughly one-third to two-thirds of the distance from any map edge, and within that zone finds the River Valley position that best satisfies the crossing-required condition. The result is always a city that matters strategically — a crossroads that every dynasty can reach but no dynasty can approach without revealing its intentions to its neighbors.

---

## Part III: Terrain Distribution

The ten terrain types are distributed across a generated map according to a procedural logic that mirrors real-world geographic relationships. The generator does not place terrain types randomly. It generates them with the same spatial coherence that actual geography produces: highlands produce rivers, rivers produce valleys, forests grow where agriculture has not cleared, coastal zones appear wherever land meets water, and the historical damage of the Frost appears in distribution patterns that reflect where the Frost was most severe.

**Adjacency Rules**

Terrain types have defined adjacency compatibility ratings. Compatible pairs appear adjacent to each other frequently and naturally. Incompatible pairs cannot be immediately adjacent — the generator interposes at least one transitional terrain type between them. The adjacency matrix is:

- Reclaimed Plains borders freely with: River Valleys, Ancient Forest, Stone Highlands, Frost Ruins, Badlands
- Reclaimed Plains does not border directly with: Tundra, Iron Ridges (Iron Ridges are elevated terrain that transitions through Stone Highlands or directly from Badlands)
- Ancient Forest borders freely with: Reclaimed Plains, Stone Highlands, River Valleys, Sacred Ground, Frost Ruins
- Stone Highlands borders freely with: Iron Ridges, Ancient Forest, Reclaimed Plains, Badlands, Tundra
- Iron Ridges borders freely with: Stone Highlands, Badlands, Frost Ruins. Iron Ridges do not border Coastal Zones or River Valleys directly — iron deposits in geological terms are highland features, not lowland ones
- River Valleys border freely with: Reclaimed Plains, Ancient Forest, Coastal Zones, Frost Ruins, Sacred Ground
- Coastal Zones border freely with: River Valleys, Reclaimed Plains, Stone Highlands (rocky coast configuration), Ancient Forest
- Frost Ruins can border any terrain type — they represent where old civilization was, and old civilization appeared in all landscape types
- Badlands border freely with: Stone Highlands, Iron Ridges, Reclaimed Plains (recovered edge), Tundra
- Sacred Ground appears embedded within any terrain type — it is a quality of a specific location, not a contiguous terrain region
- Tundra borders freely with: Stone Highlands, Badlands, Frost Ruins (northern ruins). Tundra does not border River Valleys, Ancient Forest, or Coastal Zones except in edge cases at map extremes

**Sacred Ground Placement**

Sacred Ground is placed with the highest constraint density of any terrain type. Each map generates between 4 and 6 Sacred Ground zones depending on map size: Compact maps generate 4, Standard maps generate 5, Grand and Continent maps generate 6. Each Sacred Ground zone is affiliated with exactly one of the four covenants, and the generator ensures a distribution of at least one zone per covenant, with the remaining zones distributed by random selection weighted toward the covenants that have the fewest zones.

No two Sacred Ground zones may be adjacent. The minimum separation between any two Sacred Ground zones is three province-movements, ensuring that no dynasty can capture multiple Sacred Ground zones from a single territorial base without significant expansion. Each Sacred Ground zone must be positioned such that at least two different dynasties' starting positions are within eight province-movements — the generator verifies this condition and repositions zones that fail it. Sacred Ground is contested by map geometry, not by player aggression alone; its placement ensures the contest is structural.

Sacred Ground cannot be placed in Badlands or Tundra. Sacred Ground resists the most hostile landscape conditions — it is a place where the faith made enduring contact with something, and that something did not manifest in the world's most inhospitable zones.

**Frost Ruins Distribution**

Frost Ruins appear throughout the map but concentrate in specific zones that reflect the Frost's history. The generator distributes Frost Ruins in three tiers. The first tier is high-density ruin zones: former River Valley cities and the trade corridors that connected them. These appear in the central and southern portions of the map, where the old civilization was densest and where the thaw has reached. They represent entire ruined cities — province-scale Frost Ruin territories that contain large architectural remnants. The second tier is scattered individual ruin complexes: smaller sites away from the major urban zones, former farms, fortifications, waypoints, and secondary settlements. These appear broadly distributed across Reclaimed Plains and Ancient Forest, visible evidence of how thoroughly the old civilization had spread. The third tier is trace ruins: the northernmost extent of the old world's reach, appearing as isolated structures or foundation remnants in the edge zones where the Frost hit earliest and hardest. These are found in Badlands and the transition zones toward Tundra.

The total area covered by Frost Ruins terrain on a standard map is approximately 10-15% of the playable area. On maps with the Frost Legacy setting enabled, this rises to 20-25%.

**Tundra Placement**

Tundra is exclusively a northern or high-altitude terrain type. The generator produces a continuous Tundra band across the northern map edge, with depth varying between 2 and 5 province-widths. In map configurations with significant elevation variance, Tundra can also appear at extreme altitude in highland massifs that are positioned far from the northern edge — but this is uncommon and is always isolated rather than continuous. No Tundra province appears within the guaranteed starting zone of any dynasty.

**Iron Ridges Placement**

Iron Ridges are placed with the explicit design goal of creating competition. The generator does not place Iron Ridges in starting zones. Instead, it places Iron Ridges in zones that require expansion to reach, and specifically in positions where at least two and preferably three starting dynasties are within viable marching distance. The generator checks Iron Ridge placement against all starting positions and rejects placements where a single dynasty has uncontested geometric access. Iron is a mid-game catalyst for conflict; its placement must create conflict, not resolve it.

**Minimum Guarantees per Starting Zone**

Every starting zone — the cluster of provinces within which a dynasty begins the match — carries the following guarantees regardless of generated terrain:

- At least one Reclaimed Plains or River Valley province within the starting zone (food security)
- Access to water either through River Valley adjacency or a province with a natural water source
- At least one province of Ancient Forest or Stone Highlands within two province-movements (secondary resource access)
- No Tundra within the starting zone
- No more than one Badlands province within the starting zone boundary
- No Iron Ridges within the starting zone (iron is not a starting resource)

These guarantees are enforced by post-generation validation. If a generated starting zone fails any guarantee, the generator adjusts local terrain until the guarantee is satisfied before finalizing placement.

---

## Part IV: Resource Distribution

Resources in Bloodlines are not uniformly distributed and are not abstracted to a single generic category. Each terrain type produces specific resources in quantities that reflect what that landscape would logically provide. The distribution creates a world in which no single dynasty can achieve self-sufficiency through their starting territory alone — trade, conquest, and expansion are not optional late-game activities; they are built into the starting economic reality.

**Terrain-Resource Production Table**

| Terrain | Gold | Food | Water | Wood | Stone | Iron |
|---|---|---|---|---|---|---|
| Reclaimed Plains | Moderate | High | Moderate | Low | Low | None |
| Ancient Forest | Low | Moderate | Moderate | Very High | Low | Low |
| Stone Highlands | Low | Low | Moderate | Low | Very High | Moderate |
| Iron Ridges | Low* | Very Low | Low | Low | Moderate | Very High |
| River Valleys | High | High | Very High | Moderate | Low | Rare |
| Coastal Zones | Very High | High | Moderate | Moderate | Moderate | Low |
| Frost Ruins | Moderate† | Low | Low | Low | Low† | Low† |
| Badlands | Very Low | Very Low | Very Low | Very Low | Low | Trace |
| Sacred Ground | None | None | None | None | None | None |
| Tundra | None | None | None | None | Moderate | Occasional |

*Iron Ridges generate significant secondary gold through iron export once trade networks are established — this is not base production but derived value.
†Frost Ruins resource production is variable and discovery-dependent. The table represents the base before salvage activities unlock specific caches.

**Starting Zone Resource Balance**

The resource guarantee system works in conjunction with the starting zone terrain guarantees. Every dynasty's starting zone is validated to confirm that it provides: food sufficient to sustain beginning population without import (achieved by the Plains or River Valley guarantee); water access sufficient to begin construction without well investment (achieved by the water source guarantee); and at least one development resource — wood, stone, or gold — within the starting zone or immediately adjacent. The development resource is the path to building beyond subsistence level.

No starting zone has everything. The gap is always present. A dynasty starting in River Valley terrain has exceptional food and water but needs stone from elsewhere to build fortifications. A dynasty starting in Stone Highlands has stone in abundance but is perpetually dependent on food imports. A dynasty with Coastal Zone access has gold and trade potential but must secure its food supply through agricultural expansion or continued trade. The gap is not a flaw in the generation logic. The gap is the logic. It is the reason trade networks form, why expansion happens, and why peaceful neighbors sometimes decide to remain peaceful.

**Iron as Contested Resource**

Iron occupies a special design position. It does not appear in starting zones. It is found almost exclusively in Iron Ridges, with smaller deposits in Stone Highlands and trace amounts in Tundra. Its placement, as described in Part III, is specifically designed to create competition. The mid-game of most Bloodlines matches is shaped by the race to iron — which dynasty claims the nearest Iron Ridge, whether that claim goes unchallenged, and what diplomatic consequences follow from the contest.

The generator ensures that Iron Ridges are placed within viable marching range of multiple dynasties. Viable is defined as: reachable within 30 in-game days of marching from a dynasty's starting settlement, on a standard map, with no military opposition. Closer Iron Ridges are more contested. Distant Iron Ridges create longer campaigns. The generator targets a distribution where the nearest Iron Ridge to any dynasty's starting position is between 15 and 40 in-game days of marching distance — close enough to be a realistic early expansion target, far enough that reaching it requires leaving the starting zone and entering contested space.

**River Valleys and Food/Water**

River Valleys are the most resource-stable terrain type in the game. Their Very High water production and High food production make them the most resilient economic foundation available. The trade-off — low stone, rare iron, military exposure — is the reason every dynasty wants River Valley territory and not every dynasty can hold it without investment. River Valleys that also border Ancient Forest gain the forest's wood production as adjacent-territory yield, creating a combination that produces food, water, gold, and wood from a single province cluster. Dynasties that secure this combination early have an economic tempo advantage that is difficult to overcome.

**Ancient Forests and Wood Production**

Wood is the resource that scales most directly with construction. Every fortification, every building, every expansion of any dynasty's physical infrastructure requires wood. Ancient Forest is the primary wood source — it produces at a rate that no other terrain type matches. The generator ensures that Ancient Forest appears on every map in sufficient total area to supply multiple dynasties' construction needs, but does not guarantee proximity to starting zones. A dynasty that begins far from Ancient Forest must either establish a trade relationship with a dynasty that controls forest territory or plan an early expansion toward it. The scarcity of wood relative to the demand for construction is one of the primary economic pressures in the game's early and middle phases.

**Stone Highlands and Fortification Economy**

Stone is the resource that defines the late-game fortification race. A dynasty that controls Stone Highlands can build fortifications at a pace that stone-poor dynasties cannot match. The military implication — fortified provinces are dramatically harder to conquer — means that stone access is not just an economic question but a military one. The generator places Stone Highlands as elevated terrain features that require crossing lower terrain to reach. Dynasties in the map's flatlands must expand toward highlands to secure stone, or purchase it through trade at prices that favor the highlands-controlling dynasty.

**Faith-Resource Amplification on Sacred Ground**

Sacred Ground produces no material resources. This is non-negotiable — the terrain actively resists extraction. But it amplifies faith intensity in ways that function as an economic substitute within the faith system. A Shrine on Sacred Ground produces faith intensity at Temple rates. A Temple on Sacred Ground produces faith intensity at Grand Sanctuary rates. This amplification does not translate directly to material resources, but faith intensity drives conviction, and conviction is the resource that governs how populations respond to sacrifice, taxation, military levy, and territorial loyalty. Dynasties that control Sacred Ground affiliated with their covenant are not richer in material terms. They are more powerful in their ability to command their populations' compliance and sacrifice, which translates into military and governance efficiency that compounds across the match.

**Frost Ruins and Resource Discovery**

Frost Ruins carry latent resource potential that requires active discovery. The base production of a Frost Ruin province is low across all categories — the terrain is post-crisis land that has not been productively developed since the Frost. But the ruins contain infrastructure from the Age Before: warehouses, granaries, material stockpiles that were sealed when the Frost came and remain sealed until Siege Engineer units excavate them. Each major Frost Ruin province has a pool of discoverable caches — typically three to five unique finds per province — that may contain iron stores, preserved architectural stone, wood stockpiles, gold repositories, or Lore Caches that provide intelligence and faith bonuses. The discovery mechanic means that Frost Ruin territories are not static in their economic value. They reward the dynasty that invests in exploration and excavation, and they create a secondary economic layer that operates in parallel with regular resource production.

---

## Part V: Starting Position Generation

Placing starting dynasties is the most constrained operation the generator performs. The relative positions of starting dynasties determine the political geography of the match more than any other single variable. Two dynasties placed too close create a match that resolves into direct conflict too quickly. Two dynasties placed too far create a match where significant portions of the early game are spent in isolation. The generator targets a middle state: meaningful proximity, varied paths of contact, and no position that is geometrically dominant over all others.

**Placement Algorithm and Minimum Separation**

Starting positions are placed sequentially, each constrained by the positions already established. The first position is placed using simple valid zone criteria — within map bounds, satisfying terrain guarantees, not on a map edge. Each subsequent position must satisfy a minimum separation requirement from all previously placed positions.

Minimum separation is defined in province-movement terms rather than raw distance, because movement through the game world is terrain-dependent and province-movement more accurately reflects the real strategic distance. The minimum starting separation across all configurations is 8 province-movements. On Compact maps, this means starting positions are somewhat close to this minimum. On Continent maps, the additional map area allows greater natural separation, but the generator still enforces the minimum as a floor rather than a target. Average separation on Continent maps ranges from 12 to 18 province-movements.

**The Trueborn City Access Requirement**

No starting dynasty may have a direct line of march to the Trueborn City without crossing at least one province that belongs to, or is contested by, another dynasty. The generator validates this condition after all starting positions are placed. If any starting dynasty has an unobstructed path to the Trueborn City, the generator adjusts either the starting position or the province ownership of intervening territory until the condition is satisfied. This is not a trivial constraint — it is one of the most important structural guarantees the generator provides. The Trueborn City is supposed to be a crossroads that everyone can reach and no one can reach easily. Making every dynasty's path to it cross someone else's territory is the mechanical implementation of that design intent.

**Starting Zone Character**

Each dynasty's starting position is characterized by the terrain type that dominates its immediate starting cluster. This character is not cosmetic. It shapes what the dynasty can build first, what resources it has available in the early game, and therefore what its early strategic options are. A dynasty starting in River Valley terrain has economic strength and military exposure. A dynasty starting in Stone Highlands has defensive depth and economic limitation. A dynasty starting in Ancient Forest has resource advantages in wood and military advantages in forest warfare but limited cavalry effectiveness and limited food production from the forest floor alone.

The generator does not guarantee that every match produces the same starting zone character for every dynasty. It guarantees that every starting zone satisfies the minimum resource requirements described in Part III. Within those requirements, the generated terrain of the starting zone is the match's way of telling the player who they are at the moment the game begins. A Stonehelm player who begins in a River Valley is a Stonehelm dynasty displaced from its natural habitat, with an identity built around stone and defense that will need to adapt to the openness and economic vitality of river territory. That mismatch is not a failure of the generator. It is a starting condition that produces a different story than the default.

**House-Terrain Affinities**

The generator applies affinity weights when selecting starting terrain for each dynasty. These weights are not locks — they are probabilistic preferences that shape starting zone selection without determining it. When multiple valid positions are available, the generator selects the one whose terrain best matches the house's affinity profile. Affinity profiles are:

- Ironmark: Iron Ridges adjacent (primary), Stone Highlands adjacent (secondary). The Ironmark identity is built around iron extraction and the culture that grew around it. Starting near iron is not just thematic — it allows Ironmark players to develop their intended economic advantage earlier.
- Stonehelm: Stone Highlands (primary), Iron Ridges adjacent (secondary). Stonehelm's defensive identity requires stone. Starting in highlands gives them the natural chokepoints and stone production that their playstyle is designed to exploit.
- Goldgrave: Coastal Zones (primary), River Valleys adjacent (secondary). Goldgrave's trade and wealth identity is built around coast access. Starting with a harbor or the prospect of one enables the commercial expansion strategy.
- Hartvale: River Valleys (primary), Reclaimed Plains adjacent (secondary). Hartvale is the river culture house. Starting in River Valley terrain is almost always the most natural fit.
- Whitehall: Reclaimed Plains (primary), River Valleys adjacent (secondary). Whitehall's administrative and population-focused identity thrives in the most productive, most densely populated terrain.
- Westland: Ancient Forest adjacent (primary), Reclaimed Plains (secondary). Westland's identity carries frontier and border characteristics — starting at the edge of the agricultural world suits it.
- Highborne: River Valleys (primary), Coastal Zones (secondary). Highborne's aristocratic and cultural identity is built on wealth and legacy — terrain that provides both economic output and the historical weight of established settlement.
- Trueborn: The Trueborn dynasty, if played by a human player in multiplayer configurations that allow it, always starts at or immediately adjacent to the Trueborn City. This is the only fixed-position starting rule in the game.
- Oldcrest: Frost Ruins adjacent (primary), River Valleys (secondary). Oldcrest's memory and intelligence identity is built around the old world. Starting near Frost Ruins gives them early access to the discovery mechanics that their house bonuses amplify.

These are affinities, not requirements. The generator applies them as weighted preferences in position selection. In matches where the available valid positions do not include strong affinity matches for all houses, some houses will begin in non-affinity terrain. This is an intended outcome.

---

## Part VI: Minor Tribe and AI Kingdom Placement

The world of Bloodlines at Year 80 of the Reclamation is not neatly divided between the nine founding houses. The old world had populations that were not affiliated with the great houses — communities that survived the Frost through means other than the founding dynasties' methods, tribes that exist in the margins of reclaimed civilization, and groups whose identity is more tied to the terrain they inhabit than to any political structure. These minor tribes are present on every generated map.

**Minor Tribe Territory Assignment**

Minor tribes are generated with a terrain archetype that determines where they settle and how they fight. The generator assigns each minor tribe a home terrain type and places their territory within that terrain type. The archetypes and their home terrain are:

- Forest Dwellers: Ancient Forest. These are communities that survived the Frost by deep integration with pre-existing forest ecosystems. They know the forest in ways that even Wild covenant adherents acknowledge.
- River Communities: River Valleys. Smaller, non-dynastic populations that settled along rivers during the Reclamation and have built stable but politically unaffiliated communities.
- Highland Clans: Stone Highlands. Isolated mountain communities with strong internal governance and deep suspicion of dynastic expansion.
- Coastal Traders: Coastal Zones. Independent maritime communities whose economy is built on trade rather than territory.
- Ruin Dwellers: Frost Ruins. Populations that settled in the ruins and developed cultures shaped by their proximity to the old world. Often possessing unique knowledge but deeply wary of outside contact.
- Badlands Wanderers: Badlands and the edges of Tundra. Mobile communities that have adapted to harsh conditions. Low resource output but significant information value — they know the difficult terrain intimately.

**Tribe Count and Buffer Zones**

The generator places a minimum of one minor tribe territory per dynasty on Standard and larger maps. This is a floor. Actual tribe placement produces significantly more than the minimum in most generations: a standard 6-player map will typically contain 8-12 distinct minor tribe territories. Tribes are distributed across all terrain types rather than concentrated in any single area, but their placement follows the archetype logic described above.

Minor tribe territories are not uniformly hostile. They serve as natural buffers between starting dynasty positions — independent territories that a dynasty must either diplomatically engage or militarily overcome before expanding further. A dynasty that rushes military expansion against minor tribes early gains territory but spends military resource and potentially conviction. A dynasty that invests in diplomatic contact with minor tribes may gain allies, trade relationships, or population units that join the dynasty through agreement rather than conquest. The choice between these approaches is one of the first meaningful strategic decisions in the early game.

The generator validates that at least one minor tribe territory exists between every adjacent pair of starting dynasty positions. No two dynasties should be able to directly border each other from their starting positions without a tribal buffer in between. This is a geographic implementation of the design principle that early contact should be mediated rather than immediate.

**AI Kingdom Placement**

In matches with fewer than ten human players, the remaining house slots are filled by AI kingdoms. AI kingdoms follow the same starting position rules as human kingdoms — minimum separation, terrain guarantees, affinity weighting, Trueborn City access requirements. An AI kingdom playing Goldgrave will be placed near Coastal Zones or River Valleys by the same logic that would apply to a human Goldgrave player. The generator does not distinguish between human and AI positions during the placement phase.

The number of total kingdoms — human plus AI — follows the player count configuration for map size. A match with 4 human players on a Grand map configuration (designed for 7-8) will have 3-4 AI kingdoms filling the remaining slots. The generator fills kingdoms from the house roster in a random order that excludes houses already selected by human players, maintaining the nine-house roster integrity regardless of how many humans are playing.

---

## Part VII: Regional Culture Generation

Every province in Bloodlines generates with a cultural character that reflects the history of the land and the people who have inhabited it during the Reclamation. Culture is not a cosmetic property. It has direct mechanical consequences for how provinces respond to dynastic governance, how loyal populations are to rulers whose conviction posture conflicts with local values, and how hard certain territories are to absorb versus how naturally they integrate.

The generation of regional culture works from two inputs: terrain type and historical residue. Terrain type provides the base cultural disposition — communities in River Valleys have developed cultures shaped by trade, community gathering, and reliable subsistence; communities in Stone Highlands have developed cultures shaped by isolation, self-reliance, and defensive necessity. Historical residue modifies this base — a River Valley province that contains Frost Ruins carries a cultural memory of what was lost there, and that memory shapes how the population responds to different approaches to governance and faith.

**Cultural Clusters**

Adjacent provinces with similar terrain type generate compatible cultural profiles. This is a feature of geographic reality — communities that share a landscape develop shared values, shared practices, and shared references. The game represents this as cultural compatibility: dynasties that absorb culturally compatible territories pay lower loyalty costs, integrate the new population faster, and are less likely to experience resistance events after conquest.

Cultural clusters create natural integration zones for expanding dynasties. A Hartvale dynasty whose River Valley starting provinces are surrounded by other River Valley and Reclaimed Plains provinces can expand through culturally compatible territory before encountering friction zones. The economic case for early expansion stays aligned with the governance ease. This is not guaranteed — the generator can produce starting zones adjacent to cultural friction zones — but the clustering logic makes same-terrain expansion more common than cross-terrain expansion as an early phase.

**Cultural Friction Zones**

Where terrain transitions, culture transitions. The edges of Ancient Forest and Reclaimed Plains are not just ecological boundaries; they are cultural ones. A farming community at the forest edge and a forest community fifty meters inside the treeline have developed distinct values over seventy years of the Reclamation. A dynasty absorbing both types of territory will find that governing them simultaneously creates friction: what satisfies the farming community's expectations may offend the forest community's values. The generator creates cultural friction zones at terrain boundaries, flagging these provinces as higher-governance-cost territories that require more deliberate loyalty management.

The most significant friction zones in the game are typically the boundaries between Reclaimed Plains and Ancient Forest, between River Valleys and Stone Highlands, and between any civilized terrain type and Badlands. These are not arbitrary difficulty spikes. They represent genuine cultural discontinuities in the world's history — places where the Frost created divergent survival paths and those divergent paths produced divergent peoples.

**Frost Ruin Cultural Memory**

Frost Ruin provinces carry a distinctive cultural property that no other terrain type shares. The populations of Frost Ruin territories — whether they are the descendants of survivors from the Age Before or communities that moved into the ruins during the Reclamation — are shaped by the presence of the old world's remains. This cultural memory expresses itself in three specific ways that the game system tracks.

First, Frost Ruin populations have a heightened awareness of loss. They have grown up among the evidence of what was destroyed. This awareness creates a particular sensitivity to conviction postures that minimize or ignore the Frost's legacy. Dynasties that project a conviction of triumphalist progress — "we build what was never built before" — generate friction with Frost Ruin populations whose daily visual environment tells a different story. Dynasties that acknowledge the Frost's legacy and build their conviction around reclamation, memory, and honoring what was lost generate natural loyalty resonance with these populations.

Second, Frost Ruin populations often carry specific historical knowledge: the location of caches, the function of preserved infrastructure, the meaning of symbols in the ruins. The Oldcrest dynasty bonus amplifies this knowledge explicitly, but any dynasty that governs Frost Ruin territories and invests in scholarly infrastructure can access the cultural memory as an intelligence and discovery resource.

Third, Frost Ruin populations have a complicated relationship with the four ancient covenants. The Old Light was one of the dominant faiths of the Age Before — its presence is written into the ruins themselves, in architectural form language and in recovered texts. Frost Ruin populations often feel a cultural pull toward Old Light even without active missionary presence, because the ruins are its evidence. The Wild feels the ruins as reclaimed space — territory that belonged to human civilization, was abandoned, and is now being claimed back by nature. This tension between the ruins as human heritage and the ruins as natural reclamation is a source of faith conflict that the generator builds into every Frost Ruin province's cultural profile.

**Culture and the Population System Interaction**

Regional culture directly affects how quickly a dynasty can meaningfully absorb new provinces into its governance structure. A province with a compatible cultural profile integrates within 5-8 seasons of governance — the population accepts the dynasty's authority relatively quickly. A province with a friction-zone cultural profile may take 12-20 seasons of governance to fully integrate, during which it produces loyalty penalties and elevated resistance event probability. Frost Ruin provinces with strong memory characteristics have their own integration timeline that is not determined by terrain compatibility but by the dynasty's conviction posture match with local cultural values — a dynasty whose conviction deeply resonates with the ruin culture may integrate a Frost Ruin province faster than it integrates a same-terrain culturally neutral province.

---

## Part VIII: Sacred Ground and Special Features

Sacred Ground is the most mechanically significant terrain feature the generator places. It is not the most common terrain type — it is among the rarest. Its rarity is part of its design: if Sacred Ground were abundant, the faith amplification it provides would be a routine feature of dynastic governance rather than a contested prize. The generator places between 4 and 6 Sacred Ground zones per map, and each placement is constrained to maximize contestation. Sacred Ground is designed to be fought over, politically maneuvered around, and culturally significant to the faiths that recognize it.

**Sacred Ground Generation Rules**

The generator places Sacred Ground zones using a post-terrain placement algorithm that runs after all terrain and resource generation is complete. The algorithm has the following hard constraints:

- Maximum 6 Sacred Ground zones on any map size.
- Minimum 4 Sacred Ground zones on any map size.
- No two Sacred Ground zones may be adjacent or within two province-movements of each other.
- Each zone must be geometrically contested by at least two dynasty starting positions — meaning the travel distance from the zone to each of the two nearest starting positions must not differ by more than 4 province-movements.
- No Sacred Ground zone may be placed within the starting zone guarantee radius of any dynasty.
- One zone must be placed within 6 province-movements of the Trueborn City, but not within the Trueborn City's immediate province cluster.

The covenant affiliation of each zone is assigned after placement. The generator ensures that all four covenants receive at least one affiliated zone per map. Remaining zones (the 5th and 6th on larger maps) are assigned by weighted random selection. The weights favor covenants whose affiliated zones are positioned in lower-affinity terrain — balancing the geographic distribution of covenant advantage.

**Sacred Ground and the Trueborn Compact**

The Trueborn Compact explicitly establishes that Sacred Ground cannot be fortified. This is not a game balance decision layered onto the lore — it is lore-derived and mechanically enforced. The four covenant faiths, despite their deep conflicts, reached agreement on this point because each faith understands that Sacred Ground fortified by an enemy covenant represents a permanent desecration. The Compact's prohibition exists because all four faiths preferred neutral restriction over the alternative.

In mechanical terms: no construction of military nature is permitted on Sacred Ground provinces. Walls, towers, garrison quarters, supply caches, and siege emplacements cannot be built there. Shrines, Temples, and Grand Sanctuaries can be built — these are not military constructions under the Compact's framework. Armies can move through Sacred Ground and can establish temporary field positions, but permanent military presence is prohibited. A dynasty that attempts to permanently garrison Sacred Ground will find that the Compact triggers a diplomatic response from all other dynasties regardless of their political relationship — one of the few universal diplomatic events in the game.

**Ancient Battlefields**

Ancient battlefields are special provinces that carry Conviction weight. They are sites where large-scale violence occurred either before the Frost or in the early Reclamation period, and the land itself holds that history in a way that generates mechanical conviction effects for armies operating in or near the province. The generator places 2-4 ancient battlefields per standard map. Armies that camp in an ancient battlefield province experience a morale event — typically a significant temporary boost to martial conviction and a minor faith resonance event tied to the covenant most associated with sacrifice and death in the local cultural context.

Ancient battlefields are not exclusive territory — they can occur in any terrain type and do not have special governance rules. They are points of mechanical significance that create narrative moments during military campaigns.

**Old Trade Ruins**

Old trade ruins are specific Frost Ruin province variants that were commercial rather than residential in their Age Before function. They carried warehouses, market infrastructure, and financial records. The generator flags 3-6 Frost Ruin provinces per standard map as old trade ruins, giving them enhanced gold discovery potential when excavated and access to a unique historical trade network intelligence mechanic. A dynasty that excavates an old trade ruin learns which other Frost Ruin sites were connected to it in the old world's trade network, providing intelligence about the geographic layout of Age Before commerce — which translates into intelligence about where other discovery caches are likely to exist.

**Prophesied Lands**

Prophesied lands are a rare special feature: provinces that have been identified within one of the four covenant's sacred texts as sites of future significance. The game generates 2-3 prophesied land designations per map, assigning each to a specific covenant. The province itself produces no material benefit from the designation — it is not mechanically distinct from its base terrain until a dynasty affiliated with the relevant covenant places a faith structure there. When that structure is placed, the prophesied land designation triggers a faith intensity bonus and a conviction event visible to all players: the world acknowledges that the covenant's prophecy has been fulfilled. Any dynasty can discover and occupy a prophesied land, but only the affiliated covenant's faith structures can trigger the bonus. A Blood Dominion dynasty occupying an Old Light prophesied land generates the resources but denies the fulfillment event to any Old Light dynasty — a denial without benefit.

---

## Part IX: Match Start Parameters

The match begins in Year 80 of the Age of Reclamation. This is not an arbitrary number. Seventy years is enough time for the world to have recovered substantially from the worst of the Frost's damage — the great ice sheets that covered the central and southern map are gone, the major river systems have returned to their courses, and the primary terrain types are established in their current forms. But it is not so much time that the Frost's legacy has been forgotten or fully overcome. The northern zones are still Tundra. The great ruin complexes are still ruins, not rebuilt cities. The scars on the landscape are visible. The world is recovering, not recovered.

**Physical World State at Year 80**

The central and southern portions of the map — roughly the southern two-thirds — are in active recovery. Reclaimed Plains are productive, River Valleys are stable, and Ancient Forests have returned to their pre-Frost extent in most regions. Frost Ruins are accessible and partially explored by Reclamation-era settlers. Badlands are present as scars from the Frost's worst impacts but are not expanding.

The northern third of the map retains Frost conditions. Tundra dominates the northern edge. The transition zone between Tundra and the rest of the map — Stone Highlands and Badlands at elevation, beginning to soften into recoverable terrain — is where the Frost's active edge currently sits. This transition zone is not a clean line. It is a ragged frontier where seasons are harsh, where Frost Echoes occur most frequently, and where the boundary between the recovered world and the still-frozen world is genuinely uncertain.

The Trueborn City exists as the single most complete surviving structure of the Age Before. It was not destroyed by the Frost — it survived through the compound effect of its geographic location, the Trueborn dynasty's particular pre-Frost actions, and circumstances that remain contested in historical interpretation. At Year 80, it functions as a trade hub, a diplomatic neutral ground, and a visible monument to what the world was. It is the only place in the game where all four covenants maintain permanent shrine presence simultaneously — the Compact guaranteed this, and the Trueborn dynasty has enforced it.

**Starting Population Ranges**

Each dynasty begins the match with a starting population that varies by starting terrain and house identity. Starting populations are measured in thousands and represent the political community that has gathered around the dynasty's founding settlement.

| Starting Terrain Type | Starting Population Range |
|---|---|
| River Valleys | 18,000 - 24,000 |
| Reclaimed Plains | 14,000 - 20,000 |
| Coastal Zones | 15,000 - 21,000 |
| Ancient Forest | 10,000 - 15,000 |
| Stone Highlands | 8,000 - 13,000 |
| Iron Ridges | 7,000 - 12,000 |

House identity applies a modifier to this range: Whitehall receives a +15% population bonus regardless of starting terrain (their administrative identity is built around population management, and this is their starting advantage). Stonehelm receives a -10% population penalty but a +20% starting loyalty bonus (smaller, more committed population). All other houses receive no modifier.

**Starting Resource Stockpiles**

Every dynasty begins with an identical initial resource stockpile, regardless of starting position:

- Gold: 500 coins
- Food: 800 units
- Water: 400 units
- Wood: 300 units
- Stone: 200 units
- Iron: 50 units

These stockpiles represent what the dynasty carried into the Reclamation from its founding moment — resources accumulated before the match's story begins. They are a leveling factor. The economic divergence between dynasties begins with their starting terrain's production rates, not with their starting stockpiles. This means all dynasties begin with approximately the same initial capability but immediately begin to diverge based on what their terrain produces each season.

**Starting Diplomatic States**

The nine houses do not begin the match at identical diplomatic relationships. The Reclamation produced seventy years of interaction, cooperation, conflict, and memory. The generator applies the following starting diplomatic modifiers:

- Hartvale and Westland: -15 diplomatic modifier. Historical rivalry rooted in competing river and frontier claims during the early Reclamation.
- Trueborn and Oldcrest: -15 diplomatic modifier. Historical tension between the dynasty that survived the Frost at the City and the dynasty whose identity is built around preserving the memory of what that survival cost others.
- Highborne and Goldgrave: +10 alliance bonus. Both houses have aristocratic and commercial identities that found alignment in the early Reclamation's trade establishment.
- Stonehelm and Hartvale: +10 alliance bonus. Historical cooperation between the highland defensive house and the river culture house — Stonehelm's stone and Hartvale's food made them natural trade partners in the early Reclamation.

All other house pairs begin at Neutral (0). The starting modifiers do not create instant alliance or instant war — they are diplomatic starting positions that make cooperation easier or harder to establish. The -15 pairs begin with a diplomatic headwind that must be overcome through play. The +10 pairs begin with a diplomatic tailwind that can be reinforced into a formal alliance or eroded by competition.

Matches using the Historical starting relationship setting apply all modifiers above. Matches using the Neutral setting apply no starting modifiers — all dynasties begin at 0. Matches using the Random setting reroll all relationship pairs with a range of -20 to +20, creating unexpected starting alliances and rivalries that are unrelated to house lore.

**What the Trueborn City Provides at Match Start**

The Trueborn City begins the match as an active, functioning institution. It is not a ruin to be reclaimed — it is the most advanced active settlement in the world at Year 80. Any dynasty whose territory borders or approaches the Trueborn City gains:

- Access to the world trade network (moderate gold generation bonus from trade route activity)
- Diplomatic communication radius extension (the City's neutrality makes it a message relay point — diplomatic overtures to distant dynasties conducted through the City have higher acceptance probability than direct overtures)
- World information access (the City maintains records of all known territory, and the dynasty that controls access to the City can periodically query these records for intelligence on known territories outside their normal vision range)

The Trueborn City does not belong to any dynasty at match start unless the Trueborn dynasty is a player. It is governed by the Trueborn Compact as neutral territory. A dynasty can claim political influence over the City through sustained diplomatic presence, through housing its rulers in the City (if the Trueborn faction remains neutral), or through military conquest — which will trigger a universal diplomatic response from all other dynasties, echoing the Compact's violation protocols.

---

## Part X: Generation Variants and Settings

Bloodlines gives players meaningful control over what kind of world they play in before the match begins. These settings are not difficulty sliders. They are world-shaping decisions that change the texture of the match — what economic pressures dominate, how much history is present in the landscape, how the faiths contest territory, and what relationships dynasties begin with. The default settings produce a balanced and historically accurate world. The variant settings produce different games.

**Map Size Tiers**

The three primary map size configurations are described in Part II's player count table. Players select their map tier when forming a match. Compact is for smaller player counts (2-3) or for players who want a faster-developing game with quicker contact and conflict. Standard is the intended baseline experience for 4-6 players. Grand and Continent are for 7-10 players and produce games where the world genuinely feels enormous — where significant portions of the map may remain unexplored for the first several hours, where a dynasty can exist for a long time without ever having military contact with distant powers.

**Terrain Density Settings**

Three terrain density profiles are available:

Balanced (default): Resource production from all terrain types operates at the defined rates in Part IV. Starting zones satisfy the minimum guarantees. No overall resource scarcity or abundance modifier is applied. This is the intended baseline that produces the game's designed economic pressures and trade incentives.

Resource-Scarce (Hard Mode): All terrain production rates are reduced by 25%. Starting stockpiles are reduced by 30%. Starting populations are reduced by 15%. The minimum guarantee thresholds are unchanged — starting zones still satisfy survival requirements — but the margin above survival is narrower. This setting intensifies every economic decision: every expansion is more necessary, every trade relationship more valuable, every conquest more tempting. The game's politics sharpen because resources are shorter. Recommended for experienced players who want a more punishing economic game.

Resource-Rich (Political Mode): All terrain production rates are increased by 25%. Starting stockpiles are increased by 50%. Starting populations are increased by 20%. This setting reduces survival pressure and shifts the game toward political and diplomatic play. Dynasties can afford to invest in faith, culture, and governance infrastructure rather than spending the early game managing subsistence. Conflict still occurs — but it is more often driven by political ambition and cultural friction than by resource scarcity. Recommended for players more interested in the diplomatic and conviction systems than in resource management.

**Frost Legacy Setting**

Frost Legacy controls how much of the Frost's physical and cultural damage remains visible and mechanically active in the world.

Standard Reclamation (default): Frost Ruins appear at 10-15% terrain coverage. Tundra is confined to the northern third. Frost Echo events are possible but not frequent. The world shows the Frost's history without being dominated by it.

High Frost Legacy: Frost Ruins coverage rises to 20-25%. Tundra extends further south, consuming parts of what would otherwise be Stone Highlands and Badlands. Frost Echo events are significantly more frequent. The old world is more visible and more active as a mechanical element. The discovery economy from Frost Ruins becomes more significant. Faith intensity around Old Light and The Wild (which both have high affinity with Frost Ruins) is elevated throughout the map. Oldcrest's historical memory bonus becomes more powerful. This setting is recommended for players who want the match's history to feel heavier and more present.

Low Frost Legacy: Frost Ruins coverage drops to 5-8%. Tundra is confined to a narrow northern strip. Frost Echo events are rare. The world has recovered more completely — the Age of Reclamation feels closer to completion. This setting produces a world that feels younger and more forward-looking, with less mechanical weight from the past. The Frost is history rather than present reality.

**Sacred Ground Concentration**

Two settings control how many Sacred Ground zones appear and how they are distributed.

Standard Distribution (default): 4-6 Sacred Ground zones per map, following the placement rules in Part VIII. Each covenant has at least one affiliated zone. Contested placement by geometry.

Concentrated Faith (High): Maximum 6 Sacred Ground zones, but clustered geographically rather than distributed. This creates a faith contest zone where most of the Sacred Ground is concentrated in a specific map region, making that region the single most important faith battleground in the game. The rest of the map has less Sacred Ground presence. Matches with this setting develop a "faith heartland" that shapes military and diplomatic strategy differently than standard distribution.

Distributed Faith (Low): Maximum 4 Sacred Ground zones, each placed near a map edge rather than in contested interior positions. This makes Sacred Ground harder for any dynasty to claim and more strategically peripheral. Faith systems still operate, but the amplification bonus is more isolated and harder to project. This setting produces matches where material resources dominate and faith plays a supporting role rather than a central one.

**Starting Relationship Setting**

Three options govern how dynasty relationships begin.

Historical (default): The starting diplomatic modifiers described in Part IX apply. Hartvale/Westland and Trueborn/Oldcrest begin with tension. Highborne/Goldgrave and Stonehelm/Hartvale begin with alignment. All other pairs begin at Neutral.

Neutral: All dynasty pairs begin at 0. No historical modifiers apply. Every diplomatic relationship is a blank slate. This produces matches where alliances and rivalries develop entirely from the match's own events rather than from inherited history. Suitable for players who prefer to generate their own political history rather than beginning within an established one.

Random: All house pair relationships are rerolled at match start with a random value in the range of -20 to +20. This can produce unexpected combinations: ancient rivals beginning as allies, traditional allies beginning as rivals. The randomized starting relationships create diplomatic uncertainty that players must probe before they can make confident political commitments. This is the highest-variance diplomatic setting.

**Trueborn City: Always Present**

The Trueborn City cannot be disabled, removed, or replaced with an alternative structure in any variant or setting. It is the one fixed element of every generated world. Every match, regardless of configuration, begins with the Trueborn City at its position in the map's interior, functioning as a trade hub, diplomatic neutral ground, and cultural monument. This is not a settings limitation — it is a design declaration. The Trueborn City is what makes the world of Bloodlines this world and not another one. A generated map without the Trueborn City is not a Bloodlines map.
