# Bloodlines — Mechanics Index

Player-facing gameplay mechanics documentation. Cross-references system files in 04_SYSTEMS/.

## Mechanical Areas

| Area | Systems Involved | Status |
|------|-----------------|--------|
| Combat | Territory, Units, Conviction | Not started |
| Economy | Resources, Population, Territory | Not started |
| Diplomacy | Factions, Faith, Dynasty | Not started |
| Progression | Dynasty, Faith, Conviction | Not started |
| Sacrifice | Born of Sacrifice, Population, Conviction | Not started |
| Fortification | Territory, Dynasty, Faith, Population, Resource | Canonical doctrine locked 2026-04-14; implementation pending |
| Siege | Units, Territory, Dynasty, Faith, Conviction | Canonical doctrine locked 2026-04-14; implementation pending |

---

*Mechanics are documented as they are designed.*

---

## 2026-03-26 — Early Game Design Philosophy

Source: SESSION_2026-03-26_dynastic-feedback-and-corrections.md

### Principle: Early Game Focus on NPC Tribes, Minimal Inter-House Conflict

A large part of the early game should be battling or taking over as ruler many of the NPC tribes in the area. Conflict between founding houses should be extremely minimal during the early stages of the game.

This is a deliberate design choice that serves multiple purposes:

1. **World-building through gameplay:** The early game establishes that the world is populated by many independent NPC tribes and minor factions. The player's first strategic challenge is consolidating their local region, not attacking rival houses. This mirrors the historical pattern of post-collapse civilizations rebuilding from fragmented populations.

2. **Dynasty development time:** The early game gives the player time to develop their dynasty, establish marriages, commit members to roles, and build their economic and faith infrastructure before facing the existential threat of inter-house warfare. A house that rushes to war before developing its dynasty is fragile even if it wins.

3. **Natural pacing:** By making NPC tribes the primary early-game adversary, the game creates a natural progression from local consolidation to regional power to inter-house conflict. This prevents the match from immediately devolving into direct confrontation between founding houses.

### Anti-Cheese Design Principle

It is all too common in RTS games for "blitz" or mass buildups of forces and abandonment of every other factor necessary to build a successful bloodline. Cheesing should be discouraged by game design.

Specific design countermeasures:

- **Population-constrained armies:** The Born of Sacrifice system means armies require real population investment. A blitz strategy that dumps all population into military leaves the dynasty without the civilian population needed for economic, faith, and governance systems.

- **Conviction consequences:** Aggressive early expansion without proper governance generates negative conviction effects. A dynasty that conquers territory it cannot govern faces loyalty crises and revolts that undermine the conquest.

- **Dynasty fragility:** A dynasty that neglects family management (marriages, heir development, role commitments) in favor of pure military buildup is one assassination or battlefield death away from a succession crisis with no viable successors.

- **Economic dependency:** Military operations require sustained economic output. A blitz that exhausts resources without establishing trade routes and production infrastructure collapses when the initial stockpile runs out.

- **Faith infrastructure requirement:** Level 3+ military units require faith selection and development. A player who skips faith infrastructure in favor of early aggression is locked out of the most powerful military units.

- **NPC tribe difficulty:** NPC tribes in the early game are not trivial opponents. They require real strategic engagement to defeat or absorb. A player cannot simply blitz past them to reach rival houses.

The design goal is that every viable strategy requires investment across multiple systems. There is no shortcut that bypasses dynasty management, economic development, faith infrastructure, or population management. A player who tries to win through military force alone will find that the game's interconnected systems make that approach increasingly costly and ultimately unsustainable.

---

## 2026-04-14 — Fortification and Siege Mechanics (Defensive Fortification Doctrine Integration)

Source: `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md` (locked 2026-04-14). Defender-side: `04_SYSTEMS/FORTIFICATION_SYSTEM.md`. Attacker-side: `04_SYSTEMS/SIEGE_SYSTEM.md`.

Fortification and Siege are now two first-class mechanical areas in Bloodlines. Each integrates with most of the other mechanical areas and with all seven core systems.

### Fortification mechanics (defender)

- **Tiered fortification construction** — basic, fortified, stronghold, keep, citadel. Each tier unlocks additional defensive structures and leverage forms.
- **Layered defense construction** — outer works, inner ring, final core. Each layer is a separate construction target with its own progression.
- **Defensive ecosystem building** — walls, gates, towers, emplacements, garrisons, chokepoints, kill zones, signal systems, reserve mustering yards, inner fallback positions. All interconnected.
- **Specialist population recruitment** — garrison, engineers, signal keepers, wall wardens, tower artillerists, keep guard.
- **Reserve cycling mechanic** — active-defense and triage-recovery rotations for garrison troops during sustained engagement.
- **Repair under siege** — damage segments rebuild over time based on engineer population, material availability, and control of the section.
- **Faith-warded inner sanctums** — covenant apex structures inside the keep produce defensive effects.
- **Signal chain and relief network** — cross-settlement warning infrastructure.
- **Bloodline presence bonuses** — dynasty members in the keep confer defensive leverage by role.

### Siege mechanics (attacker)

- **Siege engine construction** — rams, siege towers, trebuchets, ballistae, mantlets, bombards. Build cost, escort requirement, logistical commitment.
- **Engineer recruitment** — distinct from line infantry; required for engines, earthworks, mining, counter-mining.
- **Supply continuity tracking** — supply camps, wagons, foraging, rear-area security. Supply failure collapses siege.
- **Scouting and intelligence** — weaknesses, garrison patterns, reserve locations, bloodline presence, supply status.
- **Breach planning** — target selection, approach, supporting fires, breach assault force, exploitation reserve.
- **Elite breach forces** — shock troops capable of exploiting a breach; line infantry insufficient.
- **Faith-siege operations** — covenant-sanctioned siege powers at Fervent+ intensity.
- **Sabotage operations** — gate-opening, fire-raising, supply poisoning, defector turning, counter-mining sabotage, spymaster targeting.
- **Multi-front coordination** — diversions, feints, simultaneous multi-gate assaults.
- **Isolation** — road interdiction, reinforcement interception, depot destruction, political isolation, faith isolation.

### Cross-mechanics interaction

- **Fortification × Economy** — fortification competes with offensive infrastructure for stone, iron, wood. Population commitments to garrison and engineers compete with field army and civilian labor.
- **Fortification × Diplomacy** — fortified realms can stand against coalition pressure longer. A fortified hegemon is harder for the Trueborn coalition to reduce.
- **Fortification × Dynasty** — the primary keep is the bloodline seat. Dynasty members confer defensive leverage. Capture of a developed keep is a catastrophic dynasty event.
- **Siege × Conviction** — siege conduct feeds the four-bucket conviction ledger (per 2026-04-14 cascade). Honorable siege rewards oathkeeping; wave-spam adds desecration; breach massacre adds ruthlessness and desecration; sabotage-only victory adds desecration.
- **Siege × Operations** — covert operations system (documented; implementation pending) enables sabotage as a siege enabler.
- **Siege × Faith** — covenant-sanctioned siege operations at Fervent / Apex intensity. Apex covenant rites breach apex fortifications.
- **Siege × Progression** — dynasty commanders develop through siege commands; captured bloodline members are transferred via the captive ledger.

### Assault failure mathematics

The doctrine requires explicit simulation of wave-spam denial. Canonical simulation effects:

- Attacker loss-rate against developed fortification exceeds defender loss-rate by a fortification-tier factor.
- Attacker morale and cohesion degrade after a repulsed assault, requiring reset time before effective reuse.
- Attacker supply drains per assault attempt, pulling resources away from other fronts.
- Attacker tempo penalty: no immediate re-assault possible; regroup window required.
- Defender repair window: damaged sections rebuild under cover between assaults.
- Defender reinforcement window: signal chains may trigger relief arrival.

These effects are cumulative and compound under repeated reckless assault. The simulation refuses to reward wave-spam as a reliable path to structural collapse of developed fortifications.

### Implementation status

Canonical doctrine locked 2026-04-14. Implementation lives in future phases (browser runtime and Unity migration). The Dynasty Consequence Cascade implementation (2026-04-14) is the first bridge code that interacts with fortification concepts via captive ledger and governor loss.

**Fortification doctrine is canonical.** Mechanics implementations must honor the doctrine without reducing its pillars.
