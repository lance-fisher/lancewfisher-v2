# CREATIVE BRANCH 002 — Population and Territory Systems
# PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON
# Design Content — 2026-03-18, Creative Branch 002

---

## SECTION A: Population System — Complete Mechanics

### Population as a Number

Population in Bloodlines is measured in **households** — each unit of population represents one extended family group of approximately 5 to 8 individuals (two adults, dependent children, and occasionally an elderly member or apprentice). This framing is deliberate: it keeps population numbers human-scaled enough to feel morally weighty, while allowing the numbers to remain manageable as a strategy resource. When the player reads "Population: 340," they should feel that this represents 340 families depending on their decisions, not an abstraction.

**Starting population at match start:** 80 households. This represents a newly recognized dynasty with a modest founding settlement, enough laborers to staff essential infrastructure and field a small initial force, but not enough to do everything at once. The pressure to grow is immediate.

**Population ceilings by match stage:**

- Founding stage: natural ceiling of approximately 200 households, enforced by housing capacity (which begins at a low baseline) and the absence of advanced food infrastructure.
- Consolidation stage: ceiling rises to approximately 500 to 800 households as housing expands and agricultural systems mature. The actual ceiling depends on how many housing structures have been built.
- Ideological Expansion stage: ceiling rises to 1,200 to 2,000 households. Multiple territories, each with their own housing infrastructure, now contribute. The unified pool begins to feel like an empire rather than a settlement.
- Irreversible Divergence stage: no hard ceiling beyond total housing capacity across all controlled territories. A dynasty that has expanded aggressively and invested in housing can theoretically reach 3,000 to 4,000 households, but sustaining that scale requires enormous food and water infrastructure and creates significant vulnerability to decline cascades.

**Display:** Population is shown as a single realm-wide integer in the top-bar resource panel alongside the six primary resources. The display reads the current number and the current cap (the total housing capacity currently available). Example: "Population: 340 / 580." A secondary tooltip on hover shows the current growth rate per cycle (positive or negative), the largest individual drain on population (military, assignments, or recent casualties), and a one-line status condition (Growing, Stable, Declining, Critical).

---

### Population Growth

Population grows through a **cycle-based increment system**. A cycle is a fixed game-time interval (approximately 90 seconds of real time at standard game speed). At the end of each cycle, the game evaluates growth conditions and adds or subtracts population accordingly. Growth is not continuous — it is a discrete pulse. This is intentional: it gives the player clear feedback moments and makes the rhythm of population expansion legible.

**Primary growth drivers and their contribution per cycle:**

Food surplus is the dominant driver. If food production exceeds food consumption by a ratio of 1.5:1 or better, the dynasty receives a +3 to +5 household base growth per cycle. If food is exactly sufficient (ratio between 1.0:1 and 1.5:1), growth is +1 to +2. If food is in deficit, growth halts entirely and decline pressure begins.

Water access is a secondary multiplier. If water supply is sufficient (1.0:1 or better supply-to-demand ratio), the growth multiplier is 1.0 (no penalty). If water is scarce (below 1.0:1), growth is multiplied by 0.5. If water is critically low (below 0.5:1), growth is fully suppressed and a passive -1 per cycle mortality penalty begins.

Housing capacity applies a hard cap. Population cannot exceed total housing capacity. As population approaches the housing cap, the growth rate is compressed using a soft pressure curve: at 90% of capacity, the base growth is halved. At 95%, growth is quartered. At 100%, growth stops entirely even if food and water are abundant. Building more housing directly unlocks further growth potential.

Conviction affects growth through two distinct mechanisms. Moral conviction generates a passive +1 per cycle bonus to population growth due to higher civic trust, cooperative labor arrangements, and voluntary population movement toward the dynasty's territories. Neutral conviction has no modifier. Cruel conviction imposes a -1 per cycle growth penalty (fear suppresses birth rates and accelerates emigration from border territories), but this penalty can be partially offset by Cruel-aligned faith practices that celebrate sacrifice and renewal.

Faith intensity affects growth indirectly. High faith intensity (above 70% of maximum) with a life-affirming faith tradition grants a +1 per cycle bonus. High faith intensity with a sacrificial faith tradition grants no growth bonus but may enable population-for-power exchange mechanics (see Born of Sacrifice). Faith intensity below 30% has no effect on growth.

**Growth rate mechanics:** Growth is linear within each stage — the per-cycle increment does not compound on itself. This prevents exponential runaway and keeps the relationship between infrastructure investment and population gain legible to the player. The maximum natural growth rate under ideal conditions (excellent food surplus, full water access, housing headroom, Moral conviction, high life-affirming faith) is +10 households per cycle. This is the theoretical ceiling. A well-managed dynasty will typically achieve +5 to +7 per cycle during the Consolidation stage under normal play.

**Accelerants beyond baseline:** The Healer's Hall (proposed building) grants +1 per cycle when staffed. Completing a Grand Festival event (conviction reward) grants a temporary +3 per cycle for 5 cycles. An Allied diplomatic state with a neighbor who produces food surpluses and is willing to trade can artificially boost the dynasty's food ratio, pushing growth above what internal infrastructure alone would allow.

---

### Population Consumption

Population is drawn from the pool by three categories of demand: military conscription, building staffing, and resource extraction assignments.

**Military conscription:** Raising a military unit costs population. Standard infantry units (levy spearmen, common archers) cost 4 households per unit. Specialized infantry and cavalry cost 6 to 8 households. Elite units cost 10 to 12 households. These households leave the civilian population and become the unit's manpower base. If the unit is destroyed in combat, those households are gone permanently unless Born of Sacrifice recycling applies (see below). If the unit is disbanded (the player manually disbands without engaging in sacrifice), a partial return of 50% of the original population cost flows back into the pool — representing soldiers returning to civilian life, some of whom were casualties of training or deserted during service.

For Moral conviction dynasties, volunteer mechanics partially replace conscription. A Moral dynasty with population loyalty above 70% can raise one volunteer levy per cycle at zero population cost (the households are "loaned" to the military and will return at a higher rate on disbandment — 80% return). This represents genuine belief in the dynasty's cause rather than forced service. Cruel conviction dynasties cannot access volunteer mechanics and must conscript at standard cost plus a 10% attrition premium (a Cruel ruler's levy loses 10% of its population cost to desertion within the first cycle of existence).

**Building staffing:** Certain production and civic buildings require assigned population to function at full capacity. A farm staffed at baseline produces at 100% efficiency. A farm with no assigned population produces at 20% (the bare minimum of passive tending by whatever labor wanders near it). Staffed buildings draw their workers from the population pool; those workers are not available for military service. The player manages the tension between assigning population to maximize economic output and holding it available for conscription. Typical staffing costs per building range from 2 to 6 households.

**Resource extraction assignments:** Mines, quarries, and deep wells (water-intensive infrastructure) require assigned population in the same manner as production buildings. A fully staffed iron mine produces at 100% efficiency. An unstaffed mine produces nothing. The demand for workers in extraction operations grows as the player expands their territory and opens more extraction sites, creating ongoing pressure on the population pool even as the pool grows.

**Born of Sacrifice recycling:** When veteran units complete a sacrifice cycle (the Born of Sacrifice system's periodic recycling mechanism), a fraction of the unit's population cost returns to the civilian pool. The return fraction depends on conviction alignment and the specific sacrifice rite performed.

For standard recycling (non-sacrifice disbandment of veterans): 60% of the original population cost returns. The returning households carry the "Veteran Reserve" tag — they are available for civilian assignment but can be re-conscripted at a 30% lower population cost than a fresh draft, representing experienced warriors returning to the fight.

For Born of Sacrifice ritual recycling: The sacrifice consumes the unit entirely in exchange for a power effect. 0% of the population cost returns. These households are gone. The power gained must be weighed against this permanent loss. A player who over-relies on sacrifice warfare will find their population pool shrinking even as their army grows temporarily powerful — this is the intended design tension.

For partial sacrifice (wounding a veteran unit in a sacrifice rite to gain a lesser power effect): 30% of the population cost returns to the pool without the Veteran Reserve tag.

**Opportunity cost of large armies:** A dynasty maintaining a military of 200 population-equivalent (roughly 25 to 30 standard units) is committing approximately 17% to 25% of a typical mid-game population pool to military service. That same population, if kept in the civilian workforce, would produce enough staffing assignments to run 30 to 40 buildings at full capacity. This is not a trivial tradeoff. The player cannot maintain maximum military and maximum economic output simultaneously — the population pool enforces this constraint without requiring an arbitrary rule. The pool is the rule.

---

### Population Decline

Population declines through a symmetric set of forces that mirror the growth drivers. Decline, like growth, is resolved in cycle-based pulses.

**Food shortage:** If food production falls below consumption demand, the dynasty enters deficit. A mild deficit (0.8:1 supply-to-demand) causes -1 per cycle. A moderate deficit (0.6:1) causes -3 per cycle. A severe deficit (below 0.5:1) causes -6 per cycle and triggers a Famine condition that compounds: buildings begin losing staffing as workers weaken, which further reduces food production, which deepens the deficit. Breaking a famine spiral requires immediate resource acquisition (trade, raid, or territory capture) and is one of the most dangerous emergent situations in the game.

**Disease:** A discrete event type. Disease outbreaks are triggered probabilistically when population is high and water supply is low (poor sanitation), when a newly conquered territory's population is integrated before a Healer's Hall is built, or as a rare random event during certain faith calendar periods. A disease outbreak causes -2 to -5 per cycle for 3 to 6 cycles and cannot be halted by food surplus alone — it requires either a Healer's Hall response (halves the rate and duration), a faith ritual response (available to specific faiths), or endurance. Disease is the primary source of sudden unexpected population drop in a well-managed dynasty.

**Military casualties:** Every military unit destroyed in combat removes its full remaining population cost from the pool permanently. This is unrecoverable. Routing units (fleeing but not destroyed) return 40% of their population cost to the pool as the soldiers flee home. Captured units (an enemy has the capacity to take prisoners, a mechanic tied to certain diplomatic states) result in the population being held in a "prisoner" state — unavailable but potentially recovered through ransom, diplomatic resolution, or prisoner exchange.

**Cruel conviction effects:** A dynasty operating in Cruel conviction territory suffers a passive -1 per cycle population growth penalty (mentioned above in growth) and additionally generates periodic population loss events through the "Suppression" mechanic. When Conviction crosses into deeply Cruel territory (the bottom quarter of the axis), random Suppression events fire every 4 to 6 cycles, each removing 2 to 8 households and generating loyalty damage in the territory where they occur. These represent purges, public executions, and the kind of systemic brutality that empties villages. A Cruel dynasty is powerful in specific ways — it generates fear-based military bonuses and enables certain Born of Sacrifice mechanics unavailable to Moral dynasties — but it pays for this power in constant population attrition. The Cruel path is viable, but it requires the player to compensate with conquest and expansion because internal growth cannot keep pace with internal loss.

**Sacrifice mechanics:** Every use of the Born of Sacrifice system (as described above) permanently removes the sacrificed population from the pool. Unlike casualties, which are accidental losses, sacrifice losses are deliberate. They should feel deliberate. The game does not editorialize — but the number goes down, and it stays down.

**Can population reach zero?** Yes. A dynasty whose population reaches zero loses the match. This is the game's ultimate failure state beyond losing the capital. A dynasty can be militarily intact — holding territory, with structures standing — and still lose to population collapse if famine, disease, and sacrifice combine to drain the pool faster than it can recover. This is an intentional design choice: Bloodlines is a game about the cost of power, and the ultimate cost is the erasure of the people you were supposed to be protecting.

**Floor mechanics:** A dynasty that falls below 20 households enters the Extinction Spiral condition. Growth is suppressed entirely (too few households to sustain normal birth rates), military conscription becomes impossible (no surplus households to draw from), and building staffing begins failing passively at -1 assigned worker per cycle. The player has a narrow window to reverse this through emergency trade or absorbing a neutral population from a territory. If the dynasty falls to 0, the match is over.

---

### Population Composition

Population is not homogeneous. Each unit of population carries three composition attributes: faith alignment, conviction attitude, and origin status.

**Faith alignment:** Each household belongs to one of the four ancient faiths, or is classified as "faithless" (a rare category representing displaced or syncretized populations). The distribution of faith alignment across the population pool directly affects the dynasty's available faith intensity. A dynasty whose population is 80% aligned with the ruling faith has very high faith intensity potential. A dynasty whose population is 40% aligned due to conquest has weaker faith intensity and must choose between conversion efforts (expensive and slow) or tolerance (preserves loyalty but limits faith power).

Faith alignment of the population is tracked as a percentage distribution across the pool, not as individual household flags. The player sees something like: "Population faith composition: 64% Veldrath, 22% Ashren, 14% Kaelborn." This distribution shifts slowly over time based on conversion efforts, incoming population from conquered territories, and faith event outcomes.

**Conviction attitude:** The population carries a collective conviction attitude that is distinct from (but influenced by) the dynasty's formal Conviction alignment. A Moral dynasty with a recent history of cruelty will have a population whose attitude lags behind — they remember, even if the dynasty has nominally reformed. Conviction attitude is tracked on the same axis as dynasty Conviction but moves at 25% of the speed of the dynasty's own Conviction shifts. A population that has been governed cruelly for a long time will not immediately trust a reformed dynasty. This lag is intentional and creates interesting long-game scenarios.

**Origin status:** Households are tagged as either Native (born within the dynasty's control and acclimated), Integrated (formerly conquered population that has passed through the loyalty integration process), or Recent Conquest (captured population that has not yet integrated). These tags affect loyalty generation speed, revolt risk, and the cost of faith conversion.

**Conquered population handling:** When a territory is captured, its population does not automatically join the conqueror's pool. The population remains physically in the territory but enters a "Provisional" state. They consume local food and water but do not contribute to production until loyalty thresholds are met. At Loyalty 40% (the "Compliant" threshold), they begin contributing to production at 50% efficiency. At Loyalty 65% (the "Loyal" threshold), they contribute at 100% efficiency and their origin status transitions from Recent Conquest to Integrated. At Loyalty 80%+, they are effectively indistinguishable from Native population in all mechanical respects.

A recently conquered population that is governed with Moral conviction and matching faith will reach Integrated status in approximately 8 to 12 cycles. A conquered population governed with Cruel conviction and mismatched faith may never reach Integrated status without forced conversion, which carries its own loyalty costs.

**Faith conversion:** The player can invest in a conversion push through temple infrastructure and faith event choices. Active conversion changes faith alignment in the target population at a rate of approximately 2% to 4% of the population pool per cycle during the conversion period. Conversion is expensive (gold and faith resources consumed), generates temporary loyalty damage (-5 to -15 loyalty in the territory being converted), and has an incomplete success rate — a typical conversion push will shift perhaps 60% to 70% of its target population before natural resistance slows the rate. A Moral dynasty that forces conversion will take a significant Conviction penalty. A Cruel dynasty can force conversion more aggressively but at higher loyalty cost.

---

### Population and Territory

The unified population pool exists as a realm-wide number, but population is physically distributed across the dynasty's controlled territories. Each territory has a local population count that is a component of the total. When the player builds housing in a territory, they increase that territory's housing capacity, which increases the total pool ceiling and draws new population growth into that territory specifically.

**Population distribution:** Population grows preferentially in territories with the best food, water, and housing infrastructure. A player who heavily invests in one core territory will find that territory accumulating a disproportionate share of the population. This is intentional — it creates a political geography within the dynasty where the core is rich and populated and frontier territories are sparse and vulnerable.

**Territory capture and population:** When a territory is captured, its local population count does not transfer to the conquering dynasty's pool — it enters the Provisional state described above. The conqueror gains potential population, not immediate population. This means that rapid conquest of many territories simultaneously can actually strain the dynasty: you've created many Provisional populations that consume your food and water without contributing, while your actual productive population pool may be depleted from military conscription.

**Loyal vs Compliant vs Resentful:** These are the three meaningful loyalty states for gameplay purposes (the loyalty scale has more granularity, but these are the thresholds that trigger distinct behavior).

Loyal population (65%+ loyalty): Contributes fully to production, generates passive civic bonus (+5% to economic output in the territory), and is eligible to be tapped for volunteer units in Moral conviction dynasties. Loyal territories do not require garrison to maintain control — military presence is helpful but not mandatory.

Compliant population (40% to 64% loyalty): Contributes to production at reduced efficiency (50% to 90%, scaling with loyalty score). Does not volunteer. Does not revolt actively. Does not report enemy movements. Requires a minimum garrison to prevent the loyalty score from drifting downward. A Compliant territory that loses its garrison will begin declining toward Resentful at -2 loyalty per cycle.

Resentful population (below 40% loyalty): Contributes to production at near-zero efficiency. May begin passive resistance (sabotage events, reduced output beyond the loyalty penalty). At below 25% loyalty, revolt risk activates. Resentful territories are a military drain — they require heavier garrison to prevent revolt than a Loyal territory requires to stay Loyal.

---

### Population Display and Player Information

The player sees the following population information:

Primary display (always visible, top bar): Current population, housing capacity, and a directional indicator (upward arrow for growing, dash for stable, downward arrow for declining, red pulsing indicator for critical decline).

Population report panel (accessible on demand): Full breakdown of current growth rate contributors, full breakdown of current consumption by category (military, buildings, extraction), current faith composition percentages, current conviction attitude score, and projected population in 10 cycles at current trajectory.

Territory overlay (accessible via map layer toggle): Each controlled territory shows its local population count and local loyalty score. Color-coded by loyalty state (Loyal = blue, Compliant = yellow, Resentful = red, Provisional = grey).

Warning thresholds: The game generates advisor warnings at the following conditions:
- Population decline for 3 consecutive cycles: "Your people are dying faster than they are being born."
- Population below 50 households: "Your dynasty is at risk of extinction."
- Any territory at Resentful status for more than 5 cycles: "Territory [name] is on the edge of revolt."
- Food supply dropping below 1.2:1 ratio: "Food reserves are thinning."
- Housing capacity at 95% of current population: "Your people have nowhere to grow."

What is hidden: The precise revolt probability calculation is not shown (the player sees the warning but not the percentage). The faith conversion resistance curve is not shown. The exact timing of disease outbreak risks is not shown. These are intentional opacity choices — the player has enough information to make informed decisions without the game becoming a spreadsheet exercise.

---

## SECTION B: Territory System — Complete Mechanics

### Territory as a Unit

Territories are defined by the province overlay — one of three map layers (the others being terrain and resource). Province boundaries are fixed at match generation and do not change during the match. Territories are not resized by player action. Boundaries are set by geography: rivers, mountain ranges, and coastlines create natural province edges that the province layer formalizes.

**Territory size variation:** Territories are not standardized. They vary in size based on terrain. Plains territories tend to be large (high resource potential, easy to traverse, hard to defend). Mountain territories are small (limited resource production, natural chokepoints, defensible). Forested territories are medium and irregular. The variation in size and terrain type is a core source of strategic asymmetry.

**Minimum viable territory:** Yes, a dynasty can hold a single territory indefinitely. The starting territory is designed to be self-sufficient at minimum viable conditions: it can produce enough food, water, and population to sustain a small military and grow slowly. A dynasty that loses all but its capital territory is in a desperate but not immediately fatal position — they have typically 10 to 20 cycles before resource pressure forces a decision.

**Territory resource production:** Each territory's resource output is determined by its terrain type. The ten terrain types establish the baseline output profile:

Plains: High food production, moderate water, no stone or iron, some wood. Best population growth territory type.
Forest: High wood, moderate food (game and forage), no stone, no iron, moderate water from streams.
Highland: Moderate stone, moderate iron, low food, low water. Strong military infrastructure territory, poor population growth.
Mountain: High stone, high iron, very low food, very low water. Strategically critical but difficult to hold and grow.
River Valley: Very high water, high food, moderate wood, no stone, no iron. Excellent civilian development territory.
Coastal: Moderate food (fishing), high trade access, moderate water, no stone, no iron. Trade and diplomatic infrastructure multiplier.
Desert: Very low food, very low water, occasional gold deposits. Extremely difficult to hold without support. High reward if infrastructure is invested.
Wetland: High water, high food (specialized), high disease risk, no stone, no iron. Growth potential is high but disease events are more frequent.
Volcanic: High iron, high gold deposits, very low food, low water. High value extraction territory, volatile (periodic disaster events).
Tundra: Very low food, very low water, high wood (boreal), no gold, some stone. Extreme difficulty. Holdable only with significant investment. Strategic positioning value.

Terrain baseline is not the final output — it can be improved by building investment (see Territory Development below).

---

### Military Control

Military control is established through **garrison presence and structure capture**, not merely unit presence.

**Establishing military control:** To claim military control of a territory, a dynasty must (1) defeat or drive out any enemy military presence, (2) capture the territory's central structure (the Fortified Keep if one exists, or the primary settlement structure if no Keep is built), and (3) leave at minimum one military unit in the territory. A dynasty that defeats the enemy but immediately moves all forces forward has not established military control — the territory is in a contested state.

**Minimum garrison requirement:** Military control requires at least 1 unit garrisoned in or actively patrolling the territory. The Fortified Keep, when built, extends the control radius of the garrisoning force: a Keep-garrisoned unit controls the territory even when it occasionally moves to intercept approaching enemies. Without a Keep, the unit must be physically present in the territory center at all times.

**Military control decay:** If all military units leave a territory, military control begins decaying at -5 control per cycle (control is tracked on a 0-100 scale). At control 50%, enemy scouts can move through the territory uncontested. At control 25%, a neutral territory (one with no dynasty claiming it) may begin generating unaligned militia units that resist re-entry. At control 0%, the territory is fully uncontrolled — any dynasty can claim it by entering with a military force.

**Militarily controlled but loyalty-hostile:** This is one of the most interesting territory states in the game. A dynasty has soldiers in the territory, the territory's production infrastructure is captured, but the population is deeply resentful (below 25% loyalty). The territory produces minimal output (resentful populations contribute poorly). Sabotage events fire periodically, damaging buildings. The garrison is required at increased strength (2 to 3 units instead of 1) to prevent active revolt. Enemy intelligence may be fed to opposing dynasties by the resentful population. The territory is a net drain — consuming military resources and generating problems while producing little. This state represents the fundamental problem of conquest without governance.

**The Fortified Keep:** The Fortified Keep is the anchor structure of territorial control. It extends garrison efficiency (1 unit in a Keep provides control equivalent to 2 units without a Keep), provides passive defense bonuses to any force defending from it, acts as the respawn/reinforcement point for the controlling dynasty's units in that territory, and contributes a small but consistent loyalty generation bonus (+1 loyalty per cycle simply by existing, representing the visible commitment of the controlling dynasty to the territory). Building a Keep is a declaration of intent to hold a territory long-term and is one of the single most impactful investments a dynasty can make in a contested region.

---

### Population Loyalty

Loyalty is tracked on a **numerical scale of 0 to 100** for each territory, representing the aggregate sentiment of that territory's population toward the controlling dynasty. The scale translates to five experiential tiers:

- 0 to 24: Revolt Risk. Territory is actively dangerous to hold. Revolt events can fire at any time. Production is near-zero.
- 25 to 39: Resentful. Population is hostile but contained by garrison presence. Production at 20% to 40% efficiency. Passive resistance events.
- 40 to 64: Compliant. Population tolerates control. Production at 50% to 90% efficiency. No active problems, but no active contribution either.
- 65 to 79: Loyal. Population supports the dynasty. Production at 100% efficiency. Passive civic bonuses. No revolt risk.
- 80 to 100: Devoted. Population identifies with the dynasty. Production at 110% efficiency (willing labor multiplier). Volunteer units available in Moral dynasties. Acts as an intelligence source (player receives limited warnings about enemy movements through this territory).

**Loyalty generation in a captured territory:** Loyalty starts at 10 for a freshly conquered territory. The following factors drive loyalty upward over time:

Time (passive): +0.5 per cycle simply from continued control. Slow but automatic. Represents normalization.
Food and water adequacy: If the territory's local food and water supply is sufficient for its population, +1 per cycle. If there is a surplus, +2 per cycle.
Building investment: Each Civic Structure built in the territory (Market, Temple, Healer's Hall, Council Hall) adds +1 per cycle permanently. Civic investment is the fastest reliable path to loyalty.
Faith alignment match: If the territory's majority faith matches the dynasty's ruling faith, +1.5 per cycle. If they are related faiths (theologically compatible), +0.5 per cycle. If they are opposed faiths, -1 per cycle.
Conviction: Moral conviction in the controlling dynasty generates +1 per cycle in all territories (people trust a Moral ruler more readily). Cruel conviction generates -1 per cycle in all territories.
Diplomatic demonstration: Completing a Reconciliation event (a special diplomatic event type available in Consolidation stage and later) can generate a one-time +10 to +20 loyalty in a specified territory.

**Loyalty destruction factors:**

Pillage: Ordering a pillage action (available in Hostile and War diplomatic states against the territory's origin dynasty) deals -20 to -40 loyalty instantly and destroys buildings. The damage is permanent to the building stock; the loyalty damage heals slowly.
Cruel conviction suppression events: -5 to -15 loyalty in the territory where the suppression fires.
Religious persecution: Actively suppressing the minority faith in a territory (an action available at Cruel conviction) generates -10 to -25 loyalty among the persecuted faith population and -5 among the majority if conviction attitude is Neutral or higher.
Heavy taxation: The player can set a territory's taxation rate. Standard rate: 0 loyalty effect. Heavy rate: -2 per cycle, increased gold yield. Extractive rate: -4 per cycle, maximum gold yield. Extractive taxation in a territory below 65% loyalty will push it rapidly toward revolt.
Garrison withdrawal: If a garrison is removed from a territory below 65% loyalty, -2 per cycle loyalty drift until either garrison returns or the territory revolts.

**Territorial governance attraction (voluntary integration):** A neighboring territory controlled by a rival dynasty may voluntarily seek to join the player's dynasty if conditions are sufficiently favorable. Voluntary integration triggers when: (1) the target territory's loyalty to its current ruler falls below 25% (Revolt Risk), (2) the target territory shares faith alignment with the player's dynasty, and (3) the player's dynasty has a Conviction score of Neutral or higher. When these conditions are met, a diplomatic event fires — the territory's population sends an emissary. The player can accept the voluntary integration, reject it, or propose conditions. If accepted, the territory immediately enters the player's control at Loyalty 55% (Compliant, trending toward Loyal) and its population enters the player's pool as Integrated (not Recent Conquest). No military action required. The cost is diplomatic: the territory's previous controller will move to Hostile diplomatic state at minimum, and possibly War. The gain is a ready-integrated population and territory with no conquest loyalty penalty.

---

### Territory Transition Scenarios

**Conquest (Day 1 after capture):** The capturing dynasty's military controls the territory's central structure. All buildings remain intact unless pillage was ordered (pillage is an active choice, not automatic). The population enters Provisional state — they do not produce at full capacity, they do not volunteer, and they may generate passive resentment events. Loyalty is set to 10. The previous dynasty's garrison structures (Fortified Keep, War Towers) pass to the conqueror's control but operate at 50% efficiency until the conqueror's own military engineering units inspect and re-certify them (a process that takes 2 to 3 cycles). The first 5 cycles after conquest are the most vulnerable: the garrison is present but new, loyalty is minimal, and the previous ruler may attempt a counterattack knowing their former population will not resist.

**Voluntary integration:** The territory arrives with its full building stock intact, its population already integrated, and its loyalty at 55%. The transition is peaceful but politically costly (see above). There is a 3-cycle period during which the newly integrated territory is in a "reception" state — building some additional civic structure in this window generates a one-time +10 loyalty bonus as a "welcome investment" signal.

**Revolt:** Revolt triggers when a territory's loyalty falls below 15% and has been below 25% for at least 3 cycles (preventing revolt from firing on a temporary dip). When revolt fires, the territory's population generates a rebel militia force scaled to the local population count — typically 2 to 4 units of light infantry. These rebels immediately attack the garrison. If the garrison is insufficient to defeat the rebels within 5 cycles, the territory is liberated: it enters a neutral state (no dynasty controls it), the controlling dynasty's military is expelled, and the territory's loyalty resets to 35% (representing the relief of having expelled the oppressor, but not full stability). A territory that has revolted once is harder to re-conquer peacefully — the next military capture starts at loyalty 5 instead of 10.

**Abandonment:** If a dynasty withdraws all forces and intentionally abandons a territory (a selectable action in the territory panel), the territory enters a grace period of 5 cycles during which the abandoning dynasty can return without resistance. After 5 cycles, the territory becomes fully neutral — its loyalty to the previous dynasty is set to 0, its buildings remain but begin deteriorating at -5% efficiency per cycle (without maintenance, infrastructure decays). A neutral abandoned territory is open for any dynasty to claim by military entry. The abandoning dynasty takes a small Conviction penalty if Moral (abandoning your people is not a Moral act) and no penalty if Cruel (the population was disposable anyway).

**Contested territory:** Yes, two dynasties can simultaneously claim partial control of a territory in a transition state. This occurs when one dynasty's forces enter a territory controlled by another but do not yet hold the central structure. During contested control, both dynasties' control scores are tracked (the attacker builds from 0 upward, the defender decays from their current score). Production in the territory is suspended entirely during full contestation (no one is farming when armies are clashing). The territory resolves to one dynasty's control when either the attacker captures the central structure or the attacker withdraws.

---

### Territory Development

Territories begin at their terrain baseline output and can be developed beyond that baseline through building investment. Development is permanent — built infrastructure raises the territory's output ceiling for the remainder of the match. There is no mechanism for undeveloping a territory except pillage or disaster events.

**Building investment and output scaling:** Each tier of production building (Basic, Advanced, Grand) multiplies the terrain baseline by a scalar. A Plains territory at terrain baseline produces 10 food per cycle. A Plains territory with a Basic Farm produces 15 food per cycle. With an Advanced Farm, 22 food per cycle. With a Grand Farm, 30 food per cycle. The terrain type sets the floor; the building investment raises the ceiling.

**Permanent improvements beyond terrain baseline:** Irrigation networks (a special infrastructure project requiring wood, stone, and 3 cycles to complete) permanently improve a territory's effective terrain classification for food production. A Plains territory with irrigation produces as though it were a River Valley for food purposes. This is one of the few mechanics that moves a territory permanently beyond its terrain type — and it requires meaningful resource investment.

**Territory development and loyalty generation:** Every additional Civic Structure built in a territory generates +1 loyalty per cycle. This relationship is direct and additive. A territory with 4 civic structures is generating +4 loyalty per cycle from investment alone, which stacks with all other loyalty factors. Heavy investment in a recently conquered territory is the fastest path to making it Loyal — and it is also the path that most clearly communicates to the player that governance means investment, not just control.

---

### Territory and the Three Pillars

**Conviction and loyalty generation:** Moral conviction grants +1 per cycle to all territories and removes the garrison requirement for maintaining Loyal territories (Loyal territories under Moral rule self-sustain their loyalty score without a garrison). Neutral conviction has no modifier. Cruel conviction imposes -1 per cycle across all territories, increases garrison requirements at every loyalty tier, and means that Devoted status (80%+ loyalty) is mechanically unachievable — a Cruel ruler's population cannot reach Devoted because the fear-based relationship prevents the emotional identification that Devoted represents.

**Faith alignment match and loyalty:** The match between the dynasty's faith and the territory's population faith is the second most powerful loyalty variable after civic investment. A dynasty expanding into territories of a different faith must decide between conversion (expensive, slow, generates loyalty damage in the short term) and tolerance (maintains stability but limits faith intensity, and in certain faith traditions is itself a conviction cost). A dynasty that has built a multi-faith empire through tolerance will have broadly Loyal or Compliant territories but may lack the faith intensity to access high-tier Born of Sacrifice mechanics. This is an intended strategic identity choice.

**Dynasty lineage and prestige in adjacent regions:** The Dynastic system's prestige score (accumulated through victories, grand structures, succession events, and historical events) generates a passive "regional reputation" effect. Territories adjacent to the dynasty's controlled land that are not currently controlled receive a subtle loyalty modifier — if the dynasty's prestige is high, those territories' populations are slightly more favorable toward the dynasty even before contact, making voluntary integration more likely and post-conquest loyalty recovery faster. High-prestige dynasties are not just stronger militarily — they are easier to govern, because their reputation precedes them. The prestige adjacency effect applies within 2 territory steps of the dynasty's border and scales from +0 to +5 loyalty modifier at the time of first capture (not a per-cycle effect, just a starting loyalty bonus at capture).

---

### The Grand Structure Lockout Mechanic

Grand Structures are the highest tier of building investment — each one is a match-defining commitment of resources and time, and each one makes a declaration about the dynasty's strategic identity. The lockout system ensures that these declarations are mutually exclusive: you cannot be everything. A dynasty must choose what kind of power it wants to be, and that choice closes off other forms of power permanently.

**Grand Trade Exchange locks War Citadels:** The Grand Trade Exchange is the apex commercial structure. It maximizes the territory's trade output, generates diplomatic bonuses with all Cordial and Allied dynasties, and pulls the territory's loyalty toward Devoted through prosperity. But its construction sends a signal: this dynasty has committed to trade as its primary power model. War Citadels (the apex military fortification structure) cannot be built in any territory within 2 territory steps of the Grand Trade Exchange. Mechanically, when the Grand Trade Exchange is completed, the War Citadel build option becomes grayed out in the affected territories with the tooltip: "A dynasty cannot be both merchant and conqueror in the same heartland. Your people will not build a war citadel in the shadow of your trade halls." The restriction is regional, not global — a dynasty that builds the Grand Trade Exchange in its core can still build War Citadels on distant frontiers. But the core is committed.

**Grand War Foundry locks trade:** The Grand War Foundry is the apex military production structure. It maximizes iron consumption and military unit output, generates fear-based compliance bonuses in adjacent territories, and allows the construction of the most powerful siege weapons in the game. Its construction locks the Trade Exchange (not just the Grand tier — the entire Trade Exchange building line) in the territory where it is built and all adjacent territories. The Grand War Foundry poisons the commercial economy in its shadow. It also imposes a -10 modifier to Cordial and Allied diplomatic relationship generation from any dynasty that has the Grand Trade Exchange active (the merchants do not trust the warlords). This diplomatic penalty applies globally, not just regionally. A dynasty that builds the Grand War Foundry is making an aggressive signal that affects all diplomatic relationships for the rest of the match.

**Grand Sacrifice Altar locks the Grand Healer's Hall:** The Grand Sacrifice Altar is the apex Born of Sacrifice structure — it amplifies sacrifice mechanics, reduces the population cost of sacrifice by 20%, and enables the most powerful sacrifice rite options in the game. Its construction locks the Grand Healer's Hall (the apex population recovery and disease prevention structure) in any territory within 3 territory steps. You cannot simultaneously optimize for killing your people for power and healing your people for survival. A dynasty that builds the Grand Sacrifice Altar is committing to a sacrifice economy — they accept higher population attrition in exchange for greater military power. A dynasty that builds the Grand Healer's Hall first has protected their population and foreclosed the apex sacrifice path in that region. This lockout is the moral core of the Grand Structure system: the most explicit statement the game makes about what kind of civilization you are building.

**Grand Faith Citadel locks Grand Secular Council Hall:** The Grand Faith Citadel is the apex religious authority structure — it maximizes faith intensity, enables the highest-tier faith ritual events, and generates divine mandate bonuses to the dynasty's legitimacy in all territories sharing its faith. Its construction locks the Grand Secular Council Hall (the apex civic governance structure, which maximizes loyalty generation from governance and enables the most favorable voluntary integration conditions) in the same territory and adjacent territories. You cannot simultaneously be a theocratic civilization and a civic-governance civilization at your apex. The theocrat rules through divine authority; the civic governor rules through deliberative consent. Both are viable paths to Devoted loyalty — they are not compatible at the Grand tier in the same region.

---

## Design Notes: Population as the Game's Moral Register

The unified population pool is not an abstraction. It is the game's conscience. Every number in that pool represents a family — people eating, sleeping, raising children, burying elders, attending to faith. The pool grows when a ruler provides for these people. It shrinks when a ruler exploits, sacrifices, or neglects them. A player who treats the population as a resource to be spent will eventually encounter the consequences of that treatment in the starkest possible terms: the number goes down, and at some point it stops coming back up. No advisor speech is needed. No moral score is displayed. The number falling is enough.

The unified pool creates a genuine tradeoff between military ambition and civilian welfare that no amount of optimization can fully resolve. A ruler who wants a large standing army has a small workforce. A ruler who wants a productive economy has a small army. A ruler who wants both must grow the pool, and growing the pool requires time, food, water, housing, and good governance — the exact resources that a wartime economy competes for. The pool is not a puzzle with a solution. It is a permanent tension that the player must navigate, not solve. Every match presents the same underlying dilemma in a different configuration, and the player's answers to that dilemma define what kind of dynasty they are building.

The Born of Sacrifice recycling system adds a third term to this tension: the veteran army. Veterans are the population's choice made flesh. When a dynasty recycles its veteran forces back into the population pool — releasing soldiers to return home, training new conscripts in their place — it is acknowledging that the people who fought have lives worth returning to. The veteran reserve tag they carry when they return is not just a mechanical bonus; it is the game's record that these people have already given once. When a player looks at their population of 600 households and knows that 80 of them are veteran reserves, they know something about their dynasty's history that no tooltip can fully communicate. And when the Born of Sacrifice ritual fires and those veterans are gone entirely — turned to ash and power — the pool remembers that too. The number is smaller. It does not grow back.

---

*PROPOSED / CREATIVE BRANCH 002 — NOT YET CANON*
*Design Content — 2026-03-18, Creative Branch 002*
