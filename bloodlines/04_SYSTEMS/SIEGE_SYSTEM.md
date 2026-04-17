# Bloodlines — Siege System

**Status:** SETTLED CANON (originated 2026-04-14)
**Authority:** Defensive Fortification Doctrine (`01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`).
**Scope:** Attacker-side specification. Defender-side design lives in `FORTIFICATION_SYSTEM.md`.

## Overview

The Siege system governs how fortified settlements are assaulted and reduced. It is the canonical response to developed fortifications. Siege is an earned military operation, not a casual click action. This file is additive and grows as sessions deepen the specification.

## Canonical Posture

Siege is a campaign-scale commitment. The simulation must reward preparation and punish improvisation. Line infantry alone cannot reduce an elite fortress. An attacker who fails to prepare loses units and tempo with little to show for it.

## Required Commitment Categories

Any serious siege requires plural commitments from the attacker. A fortress-citadel may demand multiple items from each category.

### Siege engines

Canonical engine classes:

- **Ram** — short range, gate and wall-base breach. Requires escort.
- **Siege tower** — troop delivery at wall height. Slow, visible, expensive.
- **Trebuchet / counterweight engine** — ranged wall and tower degradation.
- **Ballista / bolt thrower** — anti-personnel, anti-wall-fixture, anti-siege-engine.
- **Mantlet / pavise line** — mobile cover that enables engineer advance under fire.
- **Late-game bombard / heavy trebuchet** — apex wall-breach capability; expensive and slow.

Canonical constraints:

- Siege engines have build cost in gold, wood, stone, and iron.
- Siege engines have movement limits: slow, road-dependent, terrain-sensitive.
- Siege engines require dedicated escort units to survive sorties and counter-fire.
- Siege engines can be sabotaged, burned, or captured.

### Engineer specialists

Engineers are a dedicated specialist population. Canonical functions:

- Siege engine operation and repair.
- Siege earthworks (trenches, saps, approach berms).
- Counter-fortification (siege lines that isolate the target).
- Mining (under-wall tunneling) and counter-mining (defensive countermeasures).
- Breach assessment and breach priority planning.
- Battering ram escort and ram crew rotation.

Engineers must be recruited, trained, and protected. They are not drawn from standard infantry pools.

### Logistics and supply continuity

A siege force runs on supply. Canonical logistics requirements:

- **Supply camps** within movement range of the siege line.
- **Supply wagons** carrying food, water, arrows, replacement parts, siege-engine lumber.
- **Foraging rights** or nearby farms to sustain the besieging force.
- **Rear-area security**: patrols to protect supply lines from interdiction.

Logistics failure collapses a siege. An attacker who loses supply loses the siege, regardless of army size.

### Scouting and intelligence

Serious siege requires intelligence on:

- **Weak wall sections** — where to prioritize siege-engine fire.
- **Garrison patterns** — when reserves are thin, when commanders are away.
- **Reserve locations** — where the defender will counter-attack from.
- **Faith and bloodline presence** — who is in the keep and what bonuses they confer.
- **Supply status** — how long the defender can hold out.

Scouting is a precondition for breach planning, not optional.

### Breach planning

Canonical breach planning includes:

- **Target section selection** — which wall segment to breach.
- **Approach preparation** — earthworks, saps, ram path.
- **Supporting fires** — trebuchet bombardment, ballista suppression.
- **Breach assault force** — elite shock troops ready to exploit the breach immediately.
- **Flooding reserves** — follow-on units ready to pour through.
- **Exploitation plan** — inner-works objectives beyond the breach.

A siege without breach planning devolves into wave-spam and loses per the doctrine.

### Elite units

First-wave breaching requires shock troops capable of surviving the teeth of the defense. Line infantry is not sufficient. Canonical elite classes:

- **Heavy assault infantry** (house-specific): Ironmark blood-raised shock troops, Stonehelm wall-breakers, Highborne household guards, etc.
- **Faith-blessed units** at L3+ intensity.
- **Sworn companions** of bloodline commanders.
- **Apex units** at L5 faith intensity.
- **Born of Sacrifice elite** with assault-consecrated specialization.

Without elite commitment, the breach seals before exploitation.

### Faith powers

Covenant-sanctioned siege operations alter the equation. Canonical faith-siege interactions:

- **Old Light** — Pyre sieges (burning the fortress from within via fire-blessed operations), vigilant siege wards against sabotage reversal.
- **Blood Dominion** — Blood-rite breach assaults, red-oath shock troops, siege blessings paid in sacrifice.
- **The Order** — Mandate decree sieges, edict-backed breaching formations, discipline bonuses that hold shock troops in the breach under fire.
- **The Wild** — Root sieges (fortress environment turning on defenders), beast assault waves, Thorned-Elder fracture of walls.

Doctrine path (Light / Dark) modifies behavior. Dark doctrines enable more aggressive and damaging siege operations at conviction cost.

### Sabotage

Covert operations before and during siege:

- **Gate-opening** — turning a garrison officer, bribing a sentry, infiltrating a gatecrew.
- **Fire-raising** inside the walls — supply destruction, morale collapse.
- **Poisoning supplies** — reducing defender endurance.
- **Turning defectors** — opening a section of wall, signaling weak points.
- **Counter-mining sabotage** — collapsing a defensive counter-mine on the defender.
- **Spymaster operations** — targeting specific bloodline members, commanders, or supply officers.

Sabotage is not a substitute for siege. It is an enabler.

### Coordinated multi-front timing

Splitting the defender's attention is canonical. Techniques:

- **Diversionary assaults** at a non-target section.
- **Multi-gate simultaneity** forcing reserve split.
- **Outside pressure** on nearby settlements that pull defender reserves elsewhere.
- **Dynastic-political timing** — launching assault during known defender succession events or internal unrest.
- **Weather or nightfall exploitation** — assault timing that stresses defender logistics.

### Isolation

Cutting the fortress off from relief. Canonical approaches:

- **Road interdiction** — blockade of supply routes.
- **Intercepting reinforcements** — destroying relief armies before they arrive.
- **Destroying nearby depots** — eliminating supply caches that could reach the besieged.
- **Political isolation** — diplomatic pressure to prevent coalition relief.
- **Faith isolation** — covenant operations that deny the defender access to relief faith powers.

Isolation converts a defender's apex fortress into a starvation scenario. It is the slow path. It is canonical.

## Assault Model (Canonical)

### Assault phases

1. **Preparation**: Siege lines, approach berms, engine deployment, bombardment of target section.
2. **Initial bombardment**: Wall-section degradation by trebuchets and heavy engines.
3. **Breach formation**: Wall segment reaches structural failure threshold.
4. **Assault launch**: Elite shock troops commit to the breach under supporting fire.
5. **Exploitation**: If breach holds, follow-on forces pour through; if breach fails, assault retreats with heavy losses.
6. **Reserve commitment**: Defender reserves respond; attacker must sustain pressure or withdraw.
7. **Resolution**: Either the attacker achieves inner-works objectives (continuing the siege-within-a-siege pattern) or the defender seals the breach and the attacker regroups.

### Assault failure consequences

Canonical explicit penalties on the attacker for a failed assault:

- **Elite unit loss** at high rates (the breaching force is at the teeth of defense).
- **Morale and cohesion degradation** on the assault force requiring time to reset.
- **Supply drain** — ammunition, ram parts, siege engine wear consumed without result.
- **Tempo penalty** — the attacker cannot immediately repeat the assault.
- **Visibility penalty** — successful defensive repulsion makes the defender's reserve patterns visible to their own side (defender gains intelligence).
- **Political penalty** — a visible failure at a siege can trigger coalition shift, internal unrest, or conviction consequences for the attacker's ruthlessness posture.

Repeated reckless assault compounds these penalties. Wave-spam is a canonically losing branch.

## Siege Tempo

Canonical tempo constraints:

- **No instant assault after arrival.** Preparation phases are real.
- **Assault cooldown** between attempts. Engines must be repaired, units must recover.
- **Siege duration** scales with fortification class. Border settlements fall quickly; fortress-citadels require long commitments.
- **Relief window**: Every siege includes windows when relief may arrive. The besieger must plan for this.
- **Match-flow integration**: Siege operations are stage-4 and stage-5 events in canonical match structure. Laying siege during stage 2 is typically premature and costly.

## Counter-Operations by the Defender

Attackers must plan for defensive counter-operations. Canonical defender responses:

- **Sorties** — garrison sally-out to damage engines, take supplies, harass engineers.
- **Counter-mining** — defensive tunneling to intercept attacker mines.
- **Faith counter-powers** — defender covenant structures countering attacker faith operations.
- **Signal-triggered relief** — attacker may face arriving allied armies mid-siege.
- **Dynastic appeals** — defender sending envoys to pull coalition into the siege on defensive side.
- **Sabotage counter-operations** — defender spymaster targeting attacker supply, leadership, or engineers.

Siege is a contest, not a one-sided countdown.

## AI Behavior Requirements

Attacking AI must be able to:

- Identify whether a target is fortified and at what class.
- Refuse to attack elite fortifications with insufficient force.
- Plan siege preparation before assaulting.
- Protect supply lines.
- Time assaults for defender weakness windows.
- Recognize repulsed assaults as losing branches and switch tactics.
- Coordinate multi-front and isolation operations where available.
- Seek faith and sabotage force multipliers before committing to assault.

AI that rushes line infantry at fortress-citadels must be explicitly penalized by the simulation so emergent behavior selects against that branch.

## Conviction and Morality Consequences

Canonical conviction effects for attacker:

- **Committing a serious siege**: +ruthlessness proportional to scale.
- **Massacre after a successful breach**: strong +ruthlessness, possible +desecration (faith-dependent).
- **Sparing the defender after victory**: +oathkeeping, possible +stewardship.
- **Failing through wave-spam**: minor +desecration (dishonor of the fallen by reckless waste).
- **Winning through sabotage alone**: +desecration under most doctrines.
- **Winning through honorable siege (earned military operation)**: +oathkeeping.

These conviction effects feed the existing Conviction ledger (2026-04-14 cascade) so siege outcomes materially shape the attacker's moral posture in ways the game surfaces.

## Late-Game Siege Scaling

Canonical apex-siege expressions:

- **Bombards and heavy trebuchets** reducing apex wall tiers.
- **Multiple coordinated engineer teams** under a dedicated siege commander.
- **Faith apex rites** (L5 covenant apex intervention) used as siege force multipliers.
- **Dynastic commanders** committed to the siege command role with full commander presence aura.
- **Coalition siege** where multiple allied dynasties contribute siege capability.
- **Extended campaign logistics** — supply depots, replacement pipelines, forward bases.

## Relationship to Existing Systems

- **Territory**: Siege is the contested-territory endgame. See `TERRITORY_SYSTEM.md` addendum.
- **Dynastic**: Commanders committed to siege commands accrue renown and risk capture themselves. Captured siege commanders feed the Dynasty Consequence Cascade captive ledger.
- **Faith**: Covenant-sanctioned siege operations are canonical.
- **Conviction**: Siege conduct feeds the four-bucket ledger.
- **Population**: Siege drains population via engineer training, elite unit creation, and battlefield losses.
- **Resource**: Siege engines consume wood, iron, and stone at scale. Supply consumes food and water.
- **Born of Sacrifice**: Assault-consecrated Born of Sacrifice pipelines produce breach-assault elites.

## Implementation Milestones (Future Work)

This canon is design-first. Code implementation will occur in later Unity and browser-runtime phases. Anchor milestones:

- **Phase S1** — Siege engine unit class and build pipeline.
- **Phase S2** — Engineer specialist population and siege earthwork mechanics.
- **Phase S3** — Wall-segment damage model with breach thresholds.
- **Phase S4** — Assault phase state machine with failure penalty simulation.
- **Phase S5** — Siege supply line model.
- **Phase S6** — Scouting and intelligence surface for siege planning.
- **Phase S7** — Multi-front coordination and isolation operations.
- **Phase S8** — Faith-siege integration.
- **Phase S9** — Sabotage and covert siege-enabling operations.
- **Phase S10** — AI siege planning behavior.

## Canonical Non-Negotiables

1. Siege is an earned military operation, not a casual click action.
2. Multiple commitment categories are required for serious siege (engines, engineers, logistics, scouting, elites, faith, sabotage, multi-front, isolation).
3. Assault failure produces real costs on the attacker.
4. Wave-spam is canonically a losing branch against developed fortifications.
5. Siege is a contest between attacker preparation and defender preparation, not a one-sided countdown.
6. Apex sieges match apex fortifications; apex matches apex.
7. Conviction consequences flow from siege conduct and feed the existing Conviction ledger.
8. AI must learn to reject insufficient-force siege attempts.
9. Siege operations anchor stage 4 and stage 5 of the canonical match structure.
10. This system is a canonical strategic pillar, not optional flavor.

This file is additive per Bloodlines archival rules.
