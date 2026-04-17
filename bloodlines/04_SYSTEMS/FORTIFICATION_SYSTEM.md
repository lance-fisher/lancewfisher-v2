# Bloodlines — Fortification System

**Status:** SETTLED CANON (originated 2026-04-14)
**Authority:** Defensive Fortification Doctrine (`01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`).
**Scope:** Defender-side specification. Attacker-side design lives in `SIEGE_SYSTEM.md`.

## Overview

The Fortification system governs how settlements are made defensible. It is a first-class strategic path for any faction, especially around primary dynastic keeps. Depth of investment directly translates into measurable defensive leverage. This file is additive and grows as sessions deepen the specification.

## Settlement Classes and Defensive Ceilings

| Class | Defensive ceiling | Canonical role | Expected defensive posture |
|------|-------------------|----------------|-----------------------------|
| Border settlement | Low | Warning and delay | Stockade, watch tower, local militia. Expected to warn and retreat. Loss under serious assault is normal. |
| Military fort | Mid | Hold against raids and modest sieges | Walls, rotating garrison, engineer assignment, supply for medium-term siege. |
| Trade town | Mid | Protect economic core | Gates, walls, civic guard, reserve mustering. Economic value exceeds defensive scaling. |
| Regional stronghold | High | Force a real siege | Layered defenses, inner ring, dedicated garrison, significant supply, tower networks, signal chains. |
| Primary dynastic keep | Apex | Bloodline seat | Full layered fortification, apex garrison, commander presence, faith infrastructure, succession line. |
| Fortress-citadel (developed) | Apex+ | Hardest target in the realm | Actively-invested primary keep with every leverage form maxed. |

Class is a property of the settlement. Investment deepens within-class but does not break class boundaries. A border settlement cannot be developed into a fortress-citadel without first being promoted to stronghold class through major material and political commitment.

## Layered Defense Architecture

Canonical fortifications are composed of three concentric layers. All three are design-targets for any settlement aspiring to stronghold-or-higher class.

### Outer works (first contact surface)

Components:
- Earthen berm or moat
- Palisade or outer wall
- Abatis, stakes, or other approach denial
- Forward towers
- Gatehouse with killing bay
- Signal fire or watch station

Purpose: extract cost from the initial assault. Channel attackers into known corridors. Warn the inner ring.

Expected fate in a serious siege: outer works fall first. Their fall does not determine the fate of the keep.

### Inner ring (main defensive enceinte)

Components:
- Primary curtain walls
- Flanking towers at wall junctions
- Main gatehouse with murder holes, machicolations, and drop gates
- Shielded firing platforms (archer / bowman positions with cover)
- Inner garrison barracks
- Cisterns and granaries
- Reserve mustering yards
- Supply depots
- Repair workshops

Purpose: serious siege barrier. An attacker breaching the inner ring has committed to a campaign-scale operation.

Expected fate in a serious siege: intact unless the attacker commits real siege preparation. Breach is a campaign achievement.

### Final defensive core (keep proper)

Components:
- Inner keep tower or great hall
- Bloodline seat (head of bloodline residence)
- Throne hall / succession chamber
- Deep supply stores (months of food, water, ammunition)
- Faith inner sanctum (apex covenant structure when present)
- Emergency reserve garrison
- Optional house-specific features (Ironmark blood reservoir, Old Light pyre chamber, Blood Dominion altar vault, Order scriptorium core, Wild root cellar)

Purpose: final resistance. The head of bloodline retreats here. Its fall is the fall of the house.

Expected fate: holds for a meaningful period even after the inner ring is breached. Designed for siege-within-a-siege.

## Defensive Ecosystem (Connective Tissue)

No component functions alone. Canonical connection patterns:

- **Walls × Towers**: Wall sections must be overwatched by towers that can range approach and flank breaches.
- **Gates × Killing bays**: Gate corridors must be overlooked by firing positions capable of devastating a bottled assault.
- **Garrison × Reserve mustering**: Garrisons cycle. Damaged units retire for triage; fresh units deploy forward. This requires reserve yards, repair workshops, and coordinated command.
- **Signal × Reserves**: Warning infrastructure alerts inner sections in time to reposition reserves and commit commanders before outer works fall silent.
- **Chokepoints × Kill zones**: Where the attacker cannot be stopped, they must be channeled through planned fire corridors.
- **Inner fallback × Final core**: Structured fallback lines (inner street barricades, redoubts, tower-anchored secondary walls) between the inner ring and the core prevent a breached settlement from immediately collapsing to the keep.
- **Supply × All of the above**: Fortification without supply continuity starves. Granaries, cisterns, arsenals, and house-specific reservoirs are part of the defensive system, not separate buildings.

Cutting any connection weakens the whole. AI and player development must be scored on ecosystem coherence, not raw building count.

## Defensive Leverage (Measurable Effects)

Canonical leverage forms a developed fortification must produce at simulation level:

1. **Attritional cost multiplier**: Attackers take losses materially greater than losses inflicted, proportional to fortification depth.
2. **Tempo drag**: A developed fortress slows the attacker's broader campaign. Time spent besieging is time not spent expanding.
3. **Siege requirement escalation**: The more developed the fortification, the more siege preparation is required. Line infantry alone cannot take an elite fortress.
4. **Operational risk imposition**: Laying siege exposes the attacker to sorties, relief armies, coalition intervention, faith counter-powers, and unrest at home.
5. **Defender recovery**: Repulsed assaults produce morale swings, reinforcement arrival windows, and fortification repair under cover.
6. **Assault failure penalties**: Failed assaults cost the attacker in units, morale, cohesion, and supply.

## Failure Penalty Model (Canonical)

A failed assault against an intact fortified position produces, at minimum:

- **Unit loss** scaling with the fortification depth and the defensive leverage.
- **Morale and cohesion degradation** on the assault force that must be reset before effective reuse.
- **Supply drain** on the besieging force.
- **Repair window** for the defender (during the time the attacker regroups).
- **Possible reinforcement arrival** for the defender (signal infrastructure triggers relief, if relief is available).

Repeated reckless assaults compound these penalties. The simulation must make wave-spam a losing branch against developed fortifications.

## Bloodline Presence Bonuses

Dynasty members in a primary keep confer defensive bonuses:

- **Head of bloodline present**: Garrison cohesion bonus, loyalty surge, reserve availability bonus, symbolic weight for attackers to weigh.
- **Heir designate present**: Succession-intact resolve bonus; loss penalties if the heir is lost in the defense.
- **Commander present**: Aura bonus extended to the defensive garrison, faster reserve commitment, stronger flanking.
- **Governor present**: Economy-under-siege bonus (supply holds longer), loyalty protection, repair tempo increase.
- **Ideological leader present**: Faith intensity bonus, morale hold, access to covenant defensive operations.
- **Diplomat present**: Reduced coalition-forming difficulty for relief; faster enemy-unrest triggers.
- **Spymaster present**: Counter-sabotage bonus; visibility into attacker siege preparation.
- **Merchant present**: Supply acquisition bonus under siege; faster contraband-style resupply through intact trade routes.
- **Sorcerer present (mysticism path)**: Access to specialist faith or mystic defensive operations per doctrine.

The keep that holds its bloodline fights differently from a keep emptied of bloodline. This canonical asymmetry is the bridge between the Fortification system and the Dynastic system.

## Faith Integration Inside Fortifications

Faith buildings placed inside a fortified enceinte alter the defensive equation. Canonical forms:

- **Old Light (Covenant of the First Flame)**: Pyre wards that project vigilance; defensive sight bonus; counter-sabotage visibility.
- **Blood Dominion (Red Covenant)**: Blood-altar reserves that fuel defensive rites; morale surge at cost of sacrifice.
- **The Order (Sacred Mandate)**: Edict wards that enforce garrison discipline; reserve cohesion bonus.
- **The Wild (Thorned Elder Roots)**: Root wards that entangle approach; defensive terrain advantage around the keep.

Doctrine path (Light / Dark) modifies defensive behavior. Light doctrines favor resilience and recovery. Dark doctrines favor lethality and deterrence.

Apex-tier faith buildings (L5 apex structures) inside a keep produce canonically powerful defensive effects. These are late-game apex expressions.

## Specialist Population Categories

Fortification demands dedicated population commitments:

- **Garrison troops**: Drawn from the military population pool. Rotating duty, reserve yards, replacement pipeline.
- **Engineers / sappers**: Construction of fortifications, repair under siege, mining (defensive counter-mining), trap emplacements.
- **Signal keepers**: Watch towers, bell ringers, horn relay crew, fire signal keepers.
- **Wall wardens**: Specialist infantry trained for wall combat.
- **Tower artillerists (late-game)**: Crew for advanced tower weapons, trebuchets, bombards.

These specialists are distinct from field army populations and have their own recruitment, maintenance, and replacement costs.

## Recovery and Repair

Canonical recovery systems:

- **Damage during siege is partial, not binary**. Walls have structural segments that degrade and can be reinforced.
- **Repair rate** is a function of engineer population, stone and wood supply, and the defender's control over the relevant section.
- **Post-assault repair window** lets the defender rebuild damaged sections between assaults.
- **Apex fortifications** recover faster than basic fortifications due to specialized infrastructure.

## Late-Game Apex Fortification

Canonical late-game expressions:

- **Grand wall projects**: Multiple concentric curtain wall upgrades, reinforced against elite siege weapons.
- **Citadel keeps**: Inner keep fortifications at apex tier.
- **Shielded inner cores**: Final defensive cores with faith and mystic wards.
- **Reinforced gatehouses**: Multi-portcullis gates with murder bays and layered defenses.
- **Hardened towers**: Tower weapons at apex tier; specialist garrison.
- **Specialized garrison barracks**: Production of defensive specialist unit classes.
- **Faith-warded inner sanctums**: Apex covenant structures inside the keep.

These unlocks require sustained investment over the course of a long match. They are not a reward for passive play — they are the product of deliberate fortress-path commitment.

## Economy Tradeoffs (Canonical)

A fortress-path realm accepts:

- **Slower map expansion**: Fewer provinces held directly; more reliance on allied buffer zones.
- **Smaller field army**: Resources diverted to fortifications; army sizes constrained by garrison commitments.
- **Economy tempo reduction**: Stone, iron, and wood flows biased toward fortification construction.
- **Commander commitment**: Commander roles biased toward defensive governance rather than offensive operations.

In exchange, the realm gets:

- **A canonically apex defensive position**.
- **Measurable siege-denial** against ordinary attackers.
- **Coalition-defiance capability** that unfortified realms lack.
- **Dynasty preservation under siege** — bloodline and succession survive assaults that would otherwise destroy the house.

Both paths are canonically viable.

## Relationship to Existing Systems

- **Territory**: Fortification is an attribute of territorial control. See `TERRITORY_SYSTEM.md` addendum.
- **Dynastic**: The keep is the bloodline seat. See `DYNASTIC_SYSTEM.md` addendum and the 2026-04-14 Dynasty Consequence Cascade.
- **Faith**: Faith buildings inside fortifications are defensive instruments. See `FAITH_SYSTEM.md`.
- **Conviction**: Defensive commitment correlates with stewardship and oathkeeping. Treacherous sabotage correlates with desecration. Siege massacre correlates with ruthlessness.
- **Population**: Garrison, engineer, and signal populations are distinct commitment categories.
- **Resource**: Stone, iron, and wood are the primary fortification currencies. Specialist resources (blood for Ironmark, sacred materials for faith-warded sections) are secondary.
- **Born of Sacrifice**: Defensive specialist elite classes can be produced through the Born of Sacrifice pipeline with defensive-aligned consecrations.

## Implementation Milestones (Future Work)

This canon is design-first. Code implementation will occur in later Unity and browser-runtime phases. Anchor milestones:

- **Phase F1** — Control point fortification tier metadata (basic, fortified, stronghold, keep, citadel).
- **Phase F2** — Wall, tower, and gate structural entities with segment-level damage.
- **Phase F3** — Reserve garrison cycling and repair workshops.
- **Phase F4** — Bloodline presence bonuses at keeps.
- **Phase F5** — Faith-warded inner sanctum bonuses.
- **Phase F6** — Assault failure penalty system and wave-spam denial math.
- **Phase F7** — Late-game apex fortification unlocks.
- **Phase F8** — Fortress-path AI specialization.

## Canonical Non-Negotiables

1. Layered defense (outer works, inner ring, final core) is the canonical architecture.
2. Defensive components do not function alone; the ecosystem matters.
3. Deep investment produces measurable leverage beyond raw hit points.
4. Bloodline keeps are the canonical apex defensive structure.
5. Wave-spam is not a canonical path to structural collapse of developed fortifications.
6. Settlement classes have distinct and meaningful defensive ceilings.
7. Apex fortifications scale with apex offense. Apex matches apex.
8. Fortified realms feel different — mechanically, numerically, visually, narratively — from unfortified ones.
9. Defensive commitment has real tradeoffs AND real strategic reward.
10. This system is a canonical strategic pillar, not optional flavor.

This file is additive per Bloodlines archival rules.
