# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14  
Session: 5  
Canonical root: `D:\ProjectsHome\Bloodlines`

## Scope Of This Addendum

Session 4 established the broad repository diagnosis. Session 5 advances one specific structural frontier: the point where territorial governance, bloodline seat defense, covenant choice, and battlefield performance begin to operate as one connected defensive system instead of four parallel ones.

This addendum records what is newly live, what that changes about the project state, and what remains missing relative to the full canon.

## New Live State

### Governor specialization is now real in the simulation

- Governors no longer act as a flat boolean bonus.
- Assignment now resolves across two anchor types:
  - frontier control points
  - dynastic settlements
- Specialization is derived from governed anchor class:
  - `border` for frontier and military marches
  - `city` for trade and civic seats
  - `keep` for dynastic and citadel seats

### Governor assignment now behaves like territorial administration rather than a static modifier

- When no outer territory is held, the governor defaults to keep stewardship.
- When a frontier march is newly occupied and unstable, the governor rotates outward into border stewardship.
- Once the march is stabilized, the governor can rotate back to the keep.

This is important because it turns the governor role into a living territorial response system instead of a passive label.

### Faith-integrated fortification wards are now live

The selected covenant now materially changes fortified defense:

- `old_light`: Pyre ward / Judgment Pyre
- `blood_dominion`: Blood-altar reserve / Blood-altar surge
- `the_order`: Edict ward / Submission Edict
- `the_wild`: Root ward / Predator Root Ward

These wards now feed into keep defense through live modifiers rather than remaining doctrine-only text.

### Fortified combat around the keep is now more asymmetrical

- Defenders near a fortified friendly seat can gain sight and attack leverage from ward state plus keep presence.
- Hostile combatants entering the approach radius of Wild-warded defenses are slowed.
- Blood Dominion can now trigger sacrificial reserve surges during active defense, trading population for a stronger hold.

### Reserve cycling is now connected to dynasty, governor, and covenant state

Session 4 introduced reserve cycling. Session 5 deepens it:

- keep-governor stewardship affects reserve healing and reserve commitment tempo
- covenant ward type modifies reserve healing and reserve mustering
- commander and bloodline presence continue to influence defensive readiness

This means reserve cycling is no longer just a fortification mechanic. It is now a dynastic-faith-territorial mechanic.

### HUD and snapshot truth expanded

Primary keep state now surfaces:

- governor presence
- governor specialization label
- ward id / ward label
- blood-altar surge state

This makes the fortification HUD closer to a real command readout rather than a single tier counter.

## Updated Diagnosis

### Territorial system maturity

Territory is now stronger because governance is no longer binary. There is a real distinction between unstable frontier administration and defended dynastic-seat administration. The system is still early relative to full canon, but it is now architecturally pointed in the correct direction.

### Faith system maturity

Faith has crossed an important threshold. It no longer matters only in exposure, doctrine, and passive labels. Covenant choice now modifies real defensive outcomes at the keep. That is still only the beginning of the full faith architecture, but it is a legitimate bridge between covenant doctrine and battlefield consequence.

### Fortification maturity

Fortification is increasingly behaving like a system-of-systems:

- building class and tier ceilings
- assault-failure penalties
- reserve cycling
- keep presence
- covenant wards

What remains missing is still substantial, but the project has moved decisively away from generic “RTS structures with more stats.”

### Bloodline system maturity

Bloodline logic is now influencing defense in three ways:

- commander field aura
- bloodline-seat presence at the keep
- governor assignment rotation between march and keep

The deeper roster and role ecology are still absent, but the bloodline is no longer confined to succession and casualty consequence alone.

## Remaining Major Deficits After Session 5

### Rescue and ransom operations

Captive consequence is live, but player agency over rescue and ransom is still missing. Until those operations exist, the dynastic system still lacks one of its most consequential mid-match decision surfaces.

### Siege production infrastructure

The game still has only one live siege engine and no siege workshop. Serious attacker preparation remains too thin relative to canon.

### Advanced siege AI

Stonehelm can refuse a bad assault, but it still does not truly prepare one. Real staging, escort, and siege-building logic remain absent.

### Additional houses as live playable reality

The data corpus contains 9 houses, but live gameplay still belongs primarily to Ironmark. Full house asymmetry remains a major gap.

### Strategic continuity layer

The command experience still lacks the battlefield-to-theatre transition, aggregated strategic view, and wider operational war-space.

### Unity ECS runtime

The production engine lane is still structurally ready but not yet live in gameplay terms. The Unity version lock remains the gating issue.

## Next Correct Build Direction

### Browser/spec lane

The next highest-value construction path is:

1. captured-member rescue and ransom operations
2. siege production infrastructure and next engines (`siege_tower`, `trebuchet`)
3. AI siege preparation beyond refusal
4. full 11-state realm dashboard

These continue the current momentum because they connect to systems that are already real:

- rescue/ransom extends the live captive ledger
- new siege engines extend the live fortification and ram math
- AI siege preparation extends the live refusal behavior
- the 11-state dashboard extends the live realm-condition cycle and HUD

### Unity lane

Still blocked on the editor-version decision. Once resolved, the ECS foundation remains the correct path without detour:

1. content sync
2. ECS components and systems
3. authoring/baking
4. bootstrap and gameplay scenes
5. battlefield camera and input
6. bloodline-forward HUD

## Conclusion

Session 5 materially improves Bloodlines by making governance and covenant choice affect real fortification behavior. The project is still far from full completion, but it is moving in the correct direction: fewer isolated systems, more interlocked systems, and stronger alignment between canon and executable runtime.

Bloodlines remains a growing monumental RTS, not a compressed substitute. The correct next move is to keep extending the already-live dynastic, fortification, faith, and territorial mechanics into rescue, siege preparation, and broader command clarity.
