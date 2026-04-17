# Bloodlines Owner Direction - 2026-04-16

Owner: Lance Fisher

This file records the active non-negotiable project direction for Bloodlines. It is additive governance. It supersedes any older Bloodlines prompt, handoff, roadmap, plan, or recommendation that assumes MVP framing, phased-release reduction, or scope-cutting.

## Canonical Delivery Direction

Bloodlines is being built as the complete canonical realization of the full design bible inside `D:\ProjectsHome\Bloodlines`.

The shipping engine is:

- Unity 6.3 LTS
- DOTS / ECS

The target feel is:

- Command and Conquer Generals / Zero Hour
- Warcraft III
- scaled up massively in map size, faction count, faction depth, unit roster, building roster, late-game political mechanics, dynasty systems, faith systems, conviction systems, and world-pressure systems

## Required End State

The final shipped game keeps the full canonical scope intact. That means:

- all nine founding houses are realized in full
- all canonical units, buildings, faiths, mechanics, and late-game arcs are built
- art quality reaches full commercial polish
- audio quality reaches full commercial polish through Wwise
- UX quality reaches full polish across onboarding, tutorials, campaign, lobby, HUD, and in-game panels
- AI depth matches or exceeds the browser reference simulation
- multiplayer via Netcode for Entities remains in scope unless Lance explicitly removes it later

## Explicitly Forbidden

- MVP framing of any kind
- phased-release reasoning that ships a smaller Bloodlines first
- roadmap language that defers canonical systems as optional later cuts
- reduced house sets, reduced unit counts, reduced mechanic depth, or reduced late-game depth
- any recommendation to cut scope
- claims that the scope is too large or infeasible
- adding new systems to the browser reference simulation
- working in any Unity project other than `unity/` inside the Bloodlines root
- treating the `Bloodlines/` URP stub as an active work target

## Browser Reference Simulation Rule

The browser runtime under `src/game/*.js` is frozen as a behavioral specification.

Use it to understand:

- pacing
- system interaction
- state transitions
- pressure feel
- AI behavior targets

Do not treat it as a parallel production lane. Do not add new systems there. If a browser document or handoff suggests new browser implementation, treat that as historical context and port the intent into Unity instead.

## Prompting And Planning Rule

When producing a coding prompt, session prompt, plan, work order, or next-step proposal for Bloodlines:

- align to this direction
- treat older MVP or phased-delivery language as stale
- rewrite around full-canon Unity delivery
- if a task seems broad, choose the next concrete Unity implementation step that moves the full canonical vision forward
- do not preserve old reduction logic just because it exists in older docs

## Practical Execution Rule

Duration is not a constraint. Session count is not a constraint. The project stays aimed at the full elaborate Bloodlines specified by the canon.

The correct response to difficulty is:

- preserve the full target
- identify the next concrete Unity implementation step
- continue forward without scope reduction
