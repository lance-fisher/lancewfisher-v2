# Bloodlines — Population System

## Overview
Population is the foundational resource of Bloodlines. People are not abstract numbers. They are the workforce, the army, the tax base, and the source of conviction. Every soldier who dies in battle is a farmer who no longer works the fields. Every sacrifice empowers the dynasty but diminishes the people.

## Design Intent
Population creates the central tension of the game: power costs people. Military expansion requires manpower drawn from the productive population. The Born of Sacrifice system demands lives. Growth requires stability and time. The player must constantly weigh short-term military needs against long-term demographic sustainability.

## Open Design Questions
- How does population grow? (Birth rate, immigration, conquest absorption?)
- How is population consumed? (Military recruitment, sacrifice, starvation, plague?)
- Is population tracked globally or per-territory?
- Do different population groups have different attributes (skilled, faithful, rebellious)?
- How does population loss affect economic output?
- What happens to conquered populations? (Assimilate, enslave, exile, sacrifice?)
- Is there a population cap per territory based on infrastructure?

---

*Detailed mechanics will be appended here as design sessions explore this system.*

### Design Content (Added 2026-03-15)

Population exists as a unified pool representing the kingdom's civilians. It is a single pooled realm-wide value.

**Growth:** Depends primarily on food and water availability.

**Role:** Population is not just a cap. It is a living strategic resource that supplies the labor, military, civic, and ideological body of the civilization.

**Conversion:** Population can be converted into workers, soldiers, specialists, or religious participants.

**Population determines:**
- Workforce availability
- Tax revenue
- Recruitment capacity
- Territorial loyalty

**Population decline causes:**
- Famine
- Warfare
- Plagues
- Religious conflict

**Loyalty effects:** Low loyalty reduces tax income and increases risk of sabotage and rebellion.

**Morale:** Population morale is influenced by governance and treatment of soldiers/citizens. Reckless warfare lowers morale, can cause mutinies or defections. Population loyalty influences recruitment effectiveness and territorial stability.

**Political dimension:** Population is central to territory consolidation. A territory is not truly controlled by military occupation alone. Population acceptance matters.

**Housing:** Housing determines maximum population capacity. Housing replaces traditional RTS power grid mechanics. Adequate housing contributes to societal stability.

---

### Player Manual Expansion (Added 2026-03-15)

**Population as Imperative Asset:** Population destruction is not simply "collateral." It is long-form damage that can break a dynasty's future. Protecting population is imperative. Population is the body of the realm and the bridge between ethics and strategy. Cruelty is not free. Disorder is not free. Neglect is not free.

**Population Training Paths:** Population supports training paths such as farming, military, and other roles.

**Population Recovery:** A reckless war machine can burn through manpower and collapse. A stable dynasty can survive defeats and come back stronger.

**Dark Extremes and World Pressure:** If extreme dark behaviors occur at scale, irreversible population decline can occur unless other dynasties intervene. The world can enter a state where dark faith excess feels like it has gotten out of control and must be stopped. This is a systemic pressure mechanic creating late-game coalitions, forced wars, and global response.

**Expansion and Housing Timing:** Expanding faster than loyalty and housing can support creates internal fragility that only becomes obvious later. Rushing army size without housing, loyalty, sustainment, or population depth can make a realm look dangerous in the first hour and doomed by the fourth.

---

### Full System Design Expansion — Sessions 4-9 (Added 2026-03-19)

## The 90-Second Cycle

Population operates on a 90-second game cycle. Every 90 seconds, the game calculates population growth or decline based on current conditions: food availability, water supply, housing capacity, loyalty level, and active events (plague, famine, war losses). This cycle is the heartbeat of the population system. It is also the timescale for Born of Sacrifice recycling decisions — a dynasty that needs to recompose its army must account for the cycle's population replenishment rate when planning the transition.

The cycle produces a net population change number that the player can see as a positive or negative rate indicator. A dynasty running at +3 population per cycle is growing. A dynasty at -2 is declining. The player's decisions in any given 90-second window shape the next cycle's outcome.

## Housing as Core Mechanic

Housing determines the maximum population capacity of the realm. This is the mechanic that replaced the traditional RTS power plant or resource building structure. Where a conventional RTS might cap unit production by energy, Bloodlines caps it by people — and people require housing to exist.

Housing Districts are Civic buildings that expand the population ceiling. A realm that has not invested in Housing Districts will hit its population cap regardless of how much food and water it produces. The population surplus that cannot be housed has nowhere to go: it generates immigration events (surplus population leaving for territories with better conditions), reduces morale, and begins lowering productivity through crowding effects.

The strategic implication: expanding military aggressively without building housing eventually creates a population cliff. The army is consuming people who were never replaced because no room was made for them. The dynasty that plans housing ahead of military need maintains a deeper population reservoir than the one that treats housing as an afterthought.

## Population Pools and Allocation

The unified population pool allocates across competing needs simultaneously. Every person in the population is implicitly assigned to one of several categories:

**Agricultural labor** — Operates farms, ranches, and food infrastructure. Minimum threshold required to maintain food production. Below this threshold, famine events begin triggering regardless of stockpiles.

**Craft and economic labor** — Operates markets, lumber camps, quarries, iron mines, and economic infrastructure. Output of economic buildings scales with the labor pool assigned to them.

**Military recruitment** — Active soldiers drawn from the population. When a squad is recruited, those five people leave the productive civilian pool and enter the military pool. When they die, they are gone. When they are retired through Born of Sacrifice recycling, they return to the productive civilian pool.

**Faith participation** — Population segments actively engaged in faith practice. Faith intensity is partly a function of what fraction of the population is actively practicing. A dynasty can have faith buildings but if its population has low faith engagement, intensity will not climb above Active tier.

**Governance and administration** — A small but consistent population draw that scales with territory size. Larger realms require more administrative population to function at full efficiency.

These allocations are not hard-locked sliders — they are calculated dynamically by the system based on available labor, active buildings, and recruitment decisions. The player's influence comes through building choices, recruitment orders, and which infrastructure they prioritize.

## Faith Distribution and Global Tracking

The game tracks which covenant each segment of the population follows. This is not simply "the ruling dynasty's faith" applied uniformly. Populations have their own faith histories. Conquered populations may follow a different covenant than the conquering dynasty. Frost Ruin communities may have maintained older faith practices that predate the Reclamation. Minor tribe populations have their own covenant affiliations.

The global faith distribution — what percentage of total world population follows each of the four covenants — is tracked and visible through the Trueborn City's information network. This macro-level data affects holy war triggers, doctrinal conflict events, coalition formation, and the Divine Right victory path's spread mechanics.

A dynasty whose population follows a faith different from the ruling dynasty's generates internal friction. The magnitude of this friction depends on whether the ruling dynasty is practicing tolerance (allowing the population's faith), conversion pressure (attempting to shift it), or persecution (actively suppressing it). Each posture has distinct conviction costs and population loyalty effects.

## Population as Conviction Mirror

The population reads the dynasty's conviction and responds to it. This is not an abstraction — it is one of the most direct feedback mechanisms in the game.

Under Moral conviction governance, populations produce civic engagement events: volunteer recruitment bonuses, infrastructure construction that exceeds what the dynasty ordered, loyalty that grows beyond what simple prosperity would explain. The population believes in something. Belief is economically productive.

Under Neutral conviction governance, populations perform. They farm, recruit, pay taxes, and maintain the infrastructure. No surplus of belief, no deficit of it. The relationship is functional.

Under Cruel conviction governance, populations comply through fear. Fear-compliance produces similar output metrics in the short term. It produces catastrophic loyalty collapse in the long term when the fear mechanism weakens — when a major military defeat removes the force that was maintaining compliance, the dynasty discovers that loyalty it never built cannot hold a realm together.

The Dark Extremes world pressure mechanic fires when extreme Cruel conviction governance is creating population suffering at scale. Irreversible population decline begins. Other dynasties observe the suffering population through the Trueborn City's information network. Coalition formation against the cruel dynasty becomes the world's self-defense mechanism.

## House-Specific Population Notes (CB004 — PROPOSED)

**Hartvale:** Population recovery 25% faster than any other house. The house that nearly died and built an obsession with life has institutional knowledge of population resilience. Their population bounces back from losses that would permanently scar another dynasty's demographic base.

**Whitehall:** No specific population bonus. Their 5/5 neutral unit values and "competence ceiling" disadvantage apply here too — they manage population better than average but cannot spike in any category.

**Goldgrave:** Military recruitment hard-capped by active trade route count. This is the Network Dependency disadvantage expressed at the population level — Goldgrave's population is economically oriented, and military conscription without an economic basis for supporting soldiers produces resistance.

---

### Early-Game Foundation — Population Productivity States and Military Draft System (Canon Locked 2026-04-25)

## Population Productivity States

Every person in the population exists in one of four productivity states. These states determine their contribution to economic output. The aggregate weighted average of all population states produces the faction's Base Productivity each frame.

**Civilian (not drafted): 100% productivity**
The default state. Civilians are available for worker slot assignment, agricultural labor, craft work, and governance. A faction that has not drafted any military maintains full productivity on its entire population pool.

**Drafted but Untrained: 75% productivity**
Population that has been called up under the draft but has not yet been formed into trained squads. Untrained draftees are less economically productive than pure civilians because they have been pulled from normal economic roles into military preparation without yet achieving full military effectiveness. UntrainedDrafted = max(0, DraftPool - TrainedMilitary).

**Trained Reserve: 50% productivity**
Militia squads in Reserve duty state. Reserve squads are trained fighters standing down from active assignment. They remain available for rapid deployment but have returned to partial civic life — maintaining equipment, performing local guard functions, and doing limited productive work. Productivity is significantly reduced from civilian baseline because military readiness imposes a constant overhead even in peacetime.

**Trained Active Duty: 5% productivity**
Militia squads actively assigned to an operational role (Patrol, Guard, Scout, Attack, Escort, HoldPosition, DefendKeep, DefendWoodcutterCamp, DefendForagerCamp). Active-duty soldiers produce almost nothing economically. They are fully committed to their operational role. A faction that puts its entire military on active duty simultaneously experiences near-total economic output collapse. This is the design mechanism that makes sustained offensive operations economically costly and forces the player to cycle units between duty states.

## Base and Effective Productivity Calculation

**Base Productivity** is calculated each frame as a weighted average:

```
BaseProductivity = (
    civilians    * 1.00 +
    untrainedDrafted * 0.75 +
    reserveMilitary  * 0.50 +
    activeDutyMilitary * 0.05
) / totalPopulation
```

**Effective Productivity** applies settlement condition modifiers to Base Productivity:

```
EffectiveProductivity = BaseProductivity
    × (food shortage?    0.70 : 1.0)
    × (water shortage?   0.65 : 1.0)
    × (housing at cap?   0.85 : 1.0)
```

Modifiers are multiplicative and stack. A faction experiencing simultaneous food shortage, water shortage, and at-housing-cap conditions operates at 0.70 × 0.65 × 0.85 ≈ 38.7% of its Base Productivity. This represents the combined effect of malnutrition, dehydration, and crowded living conditions on worker output.

Shortage duration is tracked via accumulators (FoodShortageAccumulator, WaterShortageAccumulator). These are reserved for future escalation hooks: prolonged food shortage triggers sickness events; prolonged water shortage triggers desertion and population outmigration beyond simple productivity penalty.

## Military Draft System

The draft slider allows the player to designate what fraction of the total population is committed to military service. The system is inspired by classic 4X draft mechanics (Utopia/Master of Orion style) adapted to Bloodlines' squad-based unit model.

**Draft Rate:** 0% to 100%, adjustable in increments of 5%.

**Draft Pool:** `floor(TotalPopulation × DraftRatePct / 100)`. This is the number of people the player has committed to military readiness.

**Trained Military:** Active militia squads × 5 (canonical squad size). Each squad represents exactly five people drawn from the population.

**Trained Reserve:** Squads in Reserve duty state × 5.

**Trained Active Duty:** Squads in Active Duty state × 5.

**Untrained Drafted:** `max(0, DraftPool - TrainedMilitary)`. People the player has committed to military service who have not yet been formed into trained squads. They are drafted but unorganized — less productive than civilians, not yet militarily effective.

**Over-Drafted Flag:** Set when TrainedMilitary > DraftPool. This indicates the player has more soldiers trained than their current draft rate authorizes. It does not force immediate squad disbanding — it is a warning state that future escalation systems can act on (reduced squad morale, loyalty events, economic pressure).

The draft rate does not automatically recruit squads. It creates the population commitment headroom that the unit production system fills. A 40% draft rate on a population of 50 creates a DraftPool of 20 — room for four squads of five. The player must still recruit squads through Training Yard production.

## Squad System

The canonical squad is a five-person military unit. This scale was chosen because it is large enough to represent a coherent tactical element but small enough that individual squad losses are felt — losing one squad is losing five people from an already-committed population.

**Duty States:**
- Reserve: Squad standing down from active assignment. Contributes 50% productivity. Immediately available for deployment.
- Active Duty: Squad assigned to an operational role. Contributes 5% productivity. Committed to their current order.

**Assignment Types (9 canonical types):**
- None (triggers Reserve state)
- Guard (stationary post defense)
- Scout (reconnaissance of territory beyond the settlement perimeter)
- Patrol (mobile perimeter coverage)
- Attack (offensive engagement order)
- Escort (protection of a moving asset: caravan, Bloodline Member, supply column)
- HoldPosition (static defensive hold at a specific map point)
- DefendKeep (priority defense of the Keep structure)
- DefendWoodcutterCamp (escort/guard of wood production infrastructure)
- DefendForagerCamp (escort/guard of food production infrastructure)

The distinction between Guard, Patrol, HoldPosition, and Defend variants matters for AI interpretation and future behavior system wiring. Guard and Patrol are general-purpose defensive postures; DefendKeep/DefendWoodcutterCamp/DefendForagerCamp are structure-specific assignments that bind the squad to a building's survival.

Scouting is performed by a militia squad assigned to Scout duty — there is no separate dedicated Scout unit. The scout assignment type is intentionally placed on the general-purpose militia squad because Bloodlines' design does not support a fragile expendable-unit economy of specialized scouts. A squad scouting is a squad not defending.

## The Productivity-Military Tradeoff

The productivity state system is the primary mechanism through which military buildup creates economic cost. In most RTS games, military cost is paid once at recruitment. In Bloodlines, military cost is paid continuously in reduced economic output for as long as those soldiers exist in an active state.

A fully mobilized faction — all squads on active duty — is running at 5% of its baseline productivity on the military population segment. This is near-zero economic contribution from those people. The faction must win quickly or it will exhaust its resources before the military can achieve its objective.

A partially mobilized faction with squads in Reserve maintains 50% productivity on the military segment — still a significant reduction from civilian baseline, but sustainable over longer campaigns. The design pressure this creates: putting squads "on standby" in Reserve is not free. It is cheaper than Active Duty but still a meaningful economic sacrifice compared to returning them to civilian status entirely.

This tradeoff — between military readiness, economic output, and the speed at which military commitment must translate into victory — is the design intent behind the productivity state system.
