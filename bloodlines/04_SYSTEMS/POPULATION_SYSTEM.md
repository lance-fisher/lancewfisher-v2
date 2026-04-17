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
