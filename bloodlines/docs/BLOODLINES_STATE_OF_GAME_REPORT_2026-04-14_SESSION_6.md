# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14  
Session: 6  
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope Of This Addendum

Session 6 moves the attacker side of Bloodlines from siege refusal into real siege preparation. The project already had fortifications, reserve cycling, governor specialization, and faith-warded keep defense. What it lacked was a meaningful attacker-side production and staging answer. This addendum records that new state and the remaining deficits that still separate the runtime from the full canon.

## New Live State

### Siege now has a dedicated production lane

- `siege_workshop` is live as a buildable structure.
- Rams no longer emerge from Barracks.
- The browser/spec lane now exposes a distinct production institution for formal siege preparation rather than treating siege as an infantry-side appendage.

This matters because Bloodlines canon treats siege as a separate commitment category with its own infrastructure burden. Session 6 is the first runtime expression of that burden.

### The siege-engine roster now has real internal differentiation

- `ram`: close breach engine, high wall pressure, weak anti-unit output.
- `siege_tower`: live support engine that improves nearby allied structural assault reach and pressure.
- `trebuchet`: long-range bombard engine for wall and tower degradation before direct commitment.

The important change is not just roster count. The important change is that siege engines no longer behave as one generic class with only different costs. There is now a live distinction between breach, support, and bombardment roles.

### Stonehelm AI now prepares a siege instead of only rejecting one

- When Ironmark's keep is fortified, Stonehelm can now build `quarry`, `iron_mine`, and `siege_workshop`.
- Once the workshop exists, Stonehelm begins engine production from that workshop.
- Once siege assets and escorts exist, Stonehelm forms siege lines before the assault rather than immediately collapsing into an attack order.

This is a meaningful AI maturity step. The attacker is no longer only aware of what *not* to do. It now has the start of a positive siege branch.

## Updated Diagnosis

### Siege system maturity

The browser/spec lane now has the beginnings of a proper siege stack:

- dedicated infrastructure
- multiple engine roles
- AI preparation behavior
- staged commitment before assault

That is still far from the full doctrine, but it is a genuine systems transition. Siege is now a structured preparation problem, not only a combat multiplier problem.

### Combat-system maturity

Combat around fortifications is now richer on both sides:

- defenders have reserve cycling, wards, keep presence, and territorial governance effects
- attackers now have breach, support, and bombardment roles

This reduces the old asymmetry where the defender side had layered doctrine and the attacker side had only one engine plus refusal logic.

### Simulation architecture maturity

The runtime remains additive and data-driven:

- new buildings and units were added through JSON rather than hard-coded one-offs
- the new engine behavior extends existing production, movement, and damage paths
- the AI expansion layered onto the existing state machine rather than replacing it

This keeps the browser lane aligned with the requirement to build upward toward the final game rather than trapping Bloodlines in a narrow temporary structure.

## Remaining Major Deficits After Session 6

### Rescue and ransom operations

The captive ledger is live, but player-directed rescue and negotiated ransom are still absent. The dynasty system still lacks one of its most consequential active decision surfaces.

### Engineer specialists and siege logistics

Siege engines now exist, but engineers, supply camps, supply wagons, and siege-line continuity do not. The attacker can prepare a siege, but not yet sustain one through the longer canonical operational arc.

### Sabotage and breach enabling

There is still no covert gate-opening, fire-raising, defender-supply poisoning, or spymaster-side siege enablement. Siege remains too clean relative to canon.

### Full command legibility

The HUD still surfaces only the six heaviest realm pressures. The full 11-state dashboard and deeper attacker-preparation readouts are still missing.

### Unity production runtime

The ECS lane remains structurally prepared but still blocked by the unresolved Unity version lock.

## Next Correct Build Direction

### Browser/spec lane

The highest-value continuation path after Session 6 is:

1. captured-member rescue and ransom operations
2. engineer specialists plus siege supply continuity
3. commander keep-presence expansion and deeper bloodline defensive leverage
4. full 11-state realm-condition dashboard
5. next siege-support classes such as ballista and mantlet

These are the strongest next steps because they connect directly to systems that are already real rather than opening a disconnected side branch.

### Unity lane

Still blocked on the editor-version decision. Once locked, the ECS path remains unchanged:

1. open and sync JSON content
2. generate ScriptableObject assets
3. write the first ECS component and system foundation
4. seed the first playable scene
5. surface the bloodline-forward HUD

## Conclusion

Session 6 materially advances Bloodlines by giving the attacker a real siege lane with infrastructure, multiple engine roles, and staged AI commitment. The project is still far from full completion, but the simulation now behaves more like a war between prepared fortresses and prepared besiegers rather than a one-sided fortification doctrine.

Bloodlines remains on the intended path: additive convergence toward a monumental RTS, not a compressed substitute.
