# Completion Stage Gates

## Purpose

This file defines what Bloodlines must contain before it can honestly be treated as entering completion stage rather than open-ended prototyping.

Completion stage does not mean the project is finished. It means the major architectural and design questions are locked and the game is moving through implementation, tuning, content fill, and QA toward a real release candidate.

## Gate 1: Battle Layer Complete

Required:

- Stable camera, selection, command, and production controls
- Reliable unit movement that does not routinely break on common layouts
- Readable melee, ranged, and commander combat
- Worker gathering and drop-off loops for the primary economy
- Population, housing, and shortage pressure
- Clear win/loss states for at least one full ruleset

Not enough:

- A toy skirmish with placeholder AI and no tactical reliability

## Gate 2: Territory Layer Complete

Required:

- Capturable control points
- Territory ownership and loyalty values
- Occupation versus stabilized control distinction
- Territory-linked income, pressure, or governance effects
- AI that understands territorial priorities

Not enough:

- Decorative map markers that only add passive resource ticks

## Gate 3: House Layer Complete

Required:

- At least 3 houses with meaningful identity and readable presentation
- Shared roster baseline plus real asymmetry
- House-select and match setup support
- Visual and UI faction readability

Not enough:

- Pure palette swaps with no gameplay consequences

## Gate 4: Bloodline Layer Complete

Required:

- A live bloodline roster
- Named family members with roles
- Commanders on the battlefield
- Heir logic and succession consequences
- Capture, death, and legitimacy pressure in at least early form

Not enough:

- Static flavor names without gameplay consequence

## Gate 5: Faith and Conviction Layer Complete

Required:

- Faith discovery/exposure during play
- Formal covenant commitment
- Faith intensity and maintenance
- Low-tier divergence or doctrine pressure
- Conviction tracked through player behavior with real downstream effects

Not enough:

- A lore panel with passive faith bonuses only

## Gate 6: Operations Layer Complete

Required:

- At least 3 strategic operation classes
- Clear cost, risk, counterplay, and cooldown rules
- Detection and response mechanics
- Operations integrated with loyalty, faith, trade, or warfare

Not enough:

- One-off spells that bypass the army layer

## Gate 7: World Pressure Layer Complete

Required:

- Minor tribes with multiple outcomes beyond simple aggro
- Neutral hub or city pressure
- At least early Trueborn intervention scaffolding
- Match events or pressures that prevent static snowballing

Not enough:

- A single neutral camp with no diplomacy or world consequence

## Gate 8: AI Layer Complete

Required:

- AI that expands, defends, attacks, and contests territory
- AI that uses commanders and preserves key bloodline figures
- AI that can pursue more than one victory logic
- AI that understands faith or conviction pressure at a basic level

Not enough:

- Pure build-order plus attack-wave scripting

## Gate 9: UX Layer Complete

Required:

- Battle HUD
- Alerts and event feed
- Bloodline panel
- Faith/conviction panel
- Territory/governance overlay
- Match settings and onboarding flow

Not enough:

- Debug text replacing core UI surfaces

## Gate 10: Technical Layer Complete

Required:

- Consistent data loading and validation
- Save/resume for long matches
- Deterministic enough simulation for reliable debugging
- Profiling pass for target scale
- Clear content/data boundaries

Not enough:

- Prototype code that only works for one demo map

## Gate 11: Content Layer Complete

Required:

- At least one polished skirmish ruleset
- Multiple maps
- Three production houses
- Faith entry content
- Commander and bloodline content
- Sufficient audio and visual readability to support real play

Not enough:

- One test map and a large backlog of design notes

## Gate 12: QA and Production Layer Complete

Required:

- Smoke test checklist
- Known-issues discipline
- Balance passes with tracked changes
- Build/run documentation
- Release packaging path

Not enough:

- Informal "it seems to work" iteration

## Current Status Against Gates

Current status as of 2026-04-12:

- Gate 1 Battle Layer: partially met
- Gate 2 Territory Layer: early prototype only
- Gate 3 House Layer: not met
- Gate 4 Bloodline Layer: not met
- Gate 5 Faith and Conviction Layer: not met
- Gate 6 Operations Layer: not met
- Gate 7 World Pressure Layer: early prototype only
- Gate 8 AI Layer: early prototype only
- Gate 9 UX Layer: early prototype only
- Gate 10 Technical Layer: partially met
- Gate 11 Content Layer: not met
- Gate 12 QA and Production Layer: partially met

## Next Push To Reach Completion Stage

The shortest credible route into completion stage is:

1. Fix pathing and tactical readability
2. Finish true territorial contesting and governance consequences
3. Add Stonehelm and Highborne as real playable houses
4. Add commanders and the first live bloodline layer
5. Add faith discovery and low-tier covenant commitment
6. Upgrade AI from scripted waves to territorial strategic play
7. Add save/resume and completion-grade UI surfaces
