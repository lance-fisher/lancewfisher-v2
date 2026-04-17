# Bloodlines Unity Phase Plan

This file turns the migration direction into executable milestones.

## U0. Bootstrap

Goal:
- Create the Unity project and repository shell.

Done when:
- `deploy/bloodlines/unity/` exists.
- Project opens in the installed Unity 6 editor.
- Nested git repo is initialized.
- `.gitignore`, `.gitattributes`, and a local project README exist.
- Baseline project state is committed.

## U1. Data Import

Goal:
- Import all canonical JSON into ScriptableObjects.

Done when:
- importer creates definition assets for all current JSON families
- importer is idempotent
- changing a JSON stat updates the related asset on re-import

## U2. Camera and Render Shell

Goal:
- Stand up a playable empty battlefield shell.

Done when:
- camera pans and zooms
- world bounds clamp correctly
- placeholder units render on terrain at 60 FPS

## U3. Spawn and Selection

Goal:
- Spawn units and buildings from imported content and interact with them.

Done when:
- initial skirmish loads from imported map data
- single-select and box-select work
- right-click move works

## U4. Economy

Goal:
- Match browser worker and economy behavior.

Done when:
- workers gather and deposit
- farms and wells provide passive trickle
- population cap and available population update correctly

## U5. Construction and Production

Goal:
- Match browser building progress and queue behavior.

Done when:
- workers construct buildings over time
- dwellings raise population cap
- command hall and barracks queues produce units

## U6. Combat

Goal:
- Match browser melee, ranged, projectile, and win/loss behavior.

Done when:
- units auto-acquire enemies
- melee and ranged both resolve correctly
- command hall destruction ends the match

## U7. Territory and Faith

Goal:
- Match browser control-point and sacred-site behavior.

Done when:
- control points contest and flip
- loyalty updates
- sacred-site exposure accumulates
- faith commitment becomes available and succeeds

## U8. AI

Goal:
- Match browser enemy and tribe behavior.

Done when:
- enemy expands, builds, trains, contests, and attacks
- tribes raid on timer and choose sensible targets

## U9. Pathing

Goal:
- Surpass browser movement quality without changing high-level commands.

Done when:
- 150 to 250 unit maneuvers avoid major snags
- building congestion is materially improved over browser steering

## U10. Commander Presence

Goal:
- Attach dynasty commanders to battlefield reality.

Done when:
- commander unit is tied to a dynasty member id
- aura is visible in gameplay
- commander death or capture is represented in state and UI

## U11. Faith Doctrine and Conviction Ledger

Goal:
- Turn faith and conviction from display state into gameplay consequence.

Done when:
- at least one doctrine path effect changes battlefield behavior
- all four conviction buckets have at least one runtime trigger

## U12. Ironmark Blood Production

Goal:
- Land Ironmark's defining asymmetry.

Done when:
- training or upkeep reflects blood cost
- Ironmark no longer feels identical to generic shared-roster houses

## U13. Additional Houses

Goal:
- Add the next player-selectable houses in the settled rollout order.

Done when:
- Stonehelm is selectable
- third playable house is introduced only if canon state allows it

## U14. Declaration / Dual-Clock Seam

Goal:
- Start the bridge from battle time to declared in-world time.

Done when:
- post-battle declaration surface reports elapsed in-world time
- future strategic hooks have a concrete seam

## U15. Save / Resume

Goal:
- Support long-form sessions.

Done when:
- simulation state can be serialized and restored reliably

## U16. Networking Foundation

Goal:
- Prepare for synchronized multiplayer after local parity is stable.

Done when:
- Netcode package is introduced without breaking single-player flow
- ghosting strategy is defined for core entities

## U17. Fortification Foundation

Goal:
- Land the Defensive Fortification Doctrine's first-tier canonical structures in Unity DOTS.

Done when:
- control point / settlement entities carry fortification tier metadata (unfortified, basic, fortified, stronghold, keep, citadel)
- wall, tower, and gate structural entities exist with segment-level damage
- layered defense architecture (outer works, inner ring, final core) is modeled at the data layer
- basic garrison reserve yard exists

## U18. Fortification Ecosystem and Leverage

Goal:
- Connect defensive components into the canonical ecosystem and produce measurable defensive leverage.

Done when:
- reserve cycling for garrisons is live (active-defense vs triage-recovery)
- signal chain infrastructure alerts adjacent settlements and allied realms
- bloodline presence bonuses at the keep fire per dynasty member role
- failed assault penalty math is live (attrition asymmetry, morale degradation, tempo penalty)
- wave-spam is demonstrably a losing branch against a developed fortress in a scripted scenario

## U19. Siege System Attacker Specification

Goal:
- Make siege an earned military operation per Decision 21.

Done when:
- siege engine unit classes exist (ram, siege tower, trebuchet, ballista, mantlet)
- engineer specialist population is recruited and trained distinctly from line infantry
- siege supply continuity tracking is live (supply camps, wagons, foraging)
- scouting and intelligence surface for siege planning exists
- breach planning state machine resolves target section, preparation, assault launch, exploitation

## U20. Faith-Integrated Fortification

Goal:
- Activate covenant-specific defensive expressions inside fortifications.

Done when:
- Old Light pyre wards confer sight and counter-sabotage visibility bonuses
- Blood Dominion blood-altar reserves fuel defensive rites
- The Order edict wards enforce garrison discipline bonuses
- The Wild root wards entangle approach
- at least one apex-tier faith building inside a keep produces measurable defensive effects

## U21. Late-Game Apex Fortification

Goal:
- Unlock fortress-citadel tier defensive expressions per the canonical late-game requirements.

Done when:
- grand wall projects, citadel keeps, shielded inner cores are reachable through sustained investment
- specialized garrison classes (keep guard, wall warden, tower artillerist) are produceable
- apex offense (bombards, apex faith rites) and apex defense produce balanced outcomes
- a scripted 6-10 hour match can land a fortress-citadel and resist a non-trivial siege

## U22. Fortification-Aware AI

Goal:
- Teach attacking AI to prepare siege and reject insufficient-force assaults.

Done when:
- AI identifies target fortification tier and adjusts approach
- AI refuses to attack elite fortifications with line infantry alone
- AI stages siege engines, recruits engineers, protects supply lines, scouts, and commits elites
- AI recognizes repulsed assaults as losing branches and switches tactics
- AI coordinates multi-front and isolation operations when available

## Session Discipline

For every phase:
- record end-state in `session-handoffs/`
- when a phase is actually complete, write a report into `phase-reports/`
- do not mark a phase done because code exists but has not been verified
