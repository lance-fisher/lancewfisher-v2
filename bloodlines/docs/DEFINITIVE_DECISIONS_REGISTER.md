# Definitive Decisions Register

## Purpose

This file lists the project decisions that must be explicitly locked if Bloodlines is going to move from promising prototype to completion-stage production. These are not brainstorming topics. They are convergence points.

Each decision includes:

- A short summary of what must be decided
- Context for why the decision matters
- A recommended default based on current canon and implementation state
- The cost of leaving the decision unresolved

## Decision 1: Product Mode Lock

Summary:
Bloodlines must formally lock whether the primary shipping product is a real-time skirmish RTS with optional long-match escalation, or a hybrid RTS-plus-persistent-grand-strategy product that requires an entirely different runtime and save model.

Context:
The canon supports long-form dynastic, faith, territorial, and strategic consequences, but the current implementation is a match-based RTS. If this is not locked, every other system risks being built twice.

Recommended default:
Ship Bloodlines as a match-based RTS first. Support standard 2-hour matches and optional extended 6-10+ hour settings inside the same match runtime. Treat persistent cross-match progression as post-foundation work, not core MVP.

If delayed:
UI, save/load, AI, pacing, victory conditions, bloodline persistence, and operations timing will all remain structurally unstable.

## Decision 2: Runtime Stack Lock

Summary:
The project must lock whether the current no-build browser runtime remains the production foundation, or whether the project migrates now to a more formal runtime such as Phaser plus Vite.

Context:
The current vanilla ES module plus canvas stack is fast to iterate and already playable. It will, however, become harder to maintain as pathfinding, UI orchestration, audio, effects, and larger battles grow.

Recommended default:
Keep the current runtime only through the next two core milestones: pathing/formations and commander integration. After that, make one explicit decision: stay vanilla and formalize internal engine modules, or migrate once to Phaser before content scale explodes.

If delayed:
Technical debt accumulates in rendering, input, pathing, effects, and scene/state management.

## Decision 3: Match Scale Lock

Summary:
The game needs explicit shipping targets for map size, unit count, player count, AI count, and expected hardware/browser performance.

Context:
Canon calls for large maps, 10-player matches, AI kingdoms, tribes, naval warfare, and long arcs. Those targets affect pathfinding, minimap design, AI budget, data streaming, and CPU limits.

Recommended default:
- Primary ship target: 1 human plus up to 5 AI on medium-large maps
- Stretch target: 10 total factions including neutrals
- Battle readability target: 150-250 live units before severe degradation

If delayed:
You cannot reliably make decisions on pathing, rendering, fog, AI budgets, or UI density.

## Decision 4: House Rollout Lock

Summary:
The project must lock the order and depth of house implementation.

Context:
All nine canonical houses are preserved, but only Ironmark currently has settled strategic certainty. The eight others remain partially reset after the 2026-04-07 canon ruling.

Recommended default:
- Ironmark: first fully playable and tuned house
- Stonehelm: second playable house for defensive contrast
- Highborne: third playable house for political and legitimacy contrast
- Remaining houses: preserve in data now, ship in phased implementation rather than all at once

If delayed:
Faction work will fragment into placeholder duplication without real gameplay identity.

## Decision 5: Early Roster Lock

Summary:
The stage-1 unit/building roster must be explicitly frozen for the first production milestone.

Context:
The prototype already has Villager, Militia, Swordsman, Bowman, Command Hall, Dwelling, Farm, Well, Barracks, Lumber Camp, and Mine Works. Without a locked early roster, balance and AI cannot settle.

Recommended default:
Lock the first milestone roster to:
- Units: Villager, Militia, Swordsman, Bowman
- Buildings: Command Hall, Dwelling, Farm, Well, Lumber Camp, Mine Works, Barracks
- Neutral: Tribal Camp, Tribal Raider

If delayed:
Every iteration changes the economy/combat baseline and invalidates balance work.

## Decision 6: Territory Model Lock

Summary:
The project must decide whether territory is control-point based, province-tile based, or hybrid.

Context:
Current implementation uses control points. Canon also expects governance, occupation, loyalty, and territorial assimilation on a much deeper level.

Recommended default:
Use a hybrid model:
- Tactical layer: capturable control points and influence zones
- Strategic layer: provinces composed of one or more control points plus settlements

If delayed:
Territory UI, loyalty math, AI priorities, and victory conditions will remain incompatible.

## Decision 7: Bloodline Scope Lock

Summary:
Bloodline members need a locked MVP role set for the first real implementation pass.

Context:
The full canon includes heirs, governors, generals, faith leaders, envoys, capture, death, inheritance, marriage, and dynastic legitimacy. That is too broad for a first shipping pass unless scoped.

Recommended default:
Lock MVP bloodline roles to:
- Ruler
- Heir
- Commander
- Governor
- Envoy

Reserve faith leader, merchant lord, spymaster, and sorcerer expansion for later phases unless required by another system.

If delayed:
The dynasty layer will stay theoretical and impossible to connect cleanly to RTS gameplay.

## Decision 8: Commander Presence Lock

Summary:
The project must decide whether commanders are always represented as battlefield units, optional attachments, or off-map strategic roles.

Context:
Bloodlines needs Warcraft III readability without collapsing into hero-only combat. Commanders are the bridge between RTS readability and dynasty consequences.

Recommended default:
Commanders should be battlefield-present attachable leaders with limited personal combat power but strong aura, morale, command, and territorial bonuses. They should be killable, capturable, and consequential.

If delayed:
The link between army play and bloodline play remains abstract.

## Decision 9: Faith Entry Lock

Summary:
The game must lock when and how a player first chooses or discovers a faith path.

Context:
Canon says faith is not immediate faction selection. It is exposed through tribes, ruins, sacred sites, or world interactions and becomes a meaningful commitment later.

Recommended default:
Stage 1 faith entry should come from map interaction:
- Tribal sites
- Sacred ruins
- Neutral shrines
- Event-driven exposure

Do not turn faith into a start-of-match lobby checkbox.

If delayed:
Faith UI, map design, event design, and AI religious behavior stay disconnected.

## Decision 10: Conviction Model Lock

Summary:
Conviction must be locked as a tracked behavioral system rather than vague narrative flavor.

Context:
Conviction affects loyalty, sabotage risk, diplomacy, and internal stability. It should not duplicate faith and should not just be a morality meter with no systemic teeth.

Recommended default:
Track conviction through repeated player actions in four live buckets:
- Ruthlessness
- Stewardship
- Oathkeeping
- Desecration

Derive public conviction posture from those buckets rather than using a single slider.

If delayed:
Conviction will likely become hand-wavy and non-playable.

## Decision 11: Operations Scope Lock

Summary:
Operations must be scoped so they matter strategically without replacing armies.

Context:
The design explicitly forbids non-military victory play that ignores armies entirely.

Recommended default:
Operations should be support systems with hard limits:
- Require population, infrastructure, or specialist commitment
- Have cooldowns and detection risk
- Modify logistics, loyalty, religion, or intel
- Never directly replace the need to hold territory and field an army

If delayed:
Operations risk becoming either irrelevant or dominant.

## Decision 12: Neutral Systems Lock

Summary:
The game must lock the roles of tribes, neutral city, and Trueborn intervention as distinct systems.

Context:
These are currently easy to blur together, but they should produce different pressures:
- Tribes: local frontier friction
- Neutral city/hub: trade and world-pressure center
- Trueborn: late anti-hegemon power

Recommended default:
Keep them separate in code, data, AI, and UI.

If delayed:
Late-game world pressure becomes muddy and mechanically weak.

## Decision 13: Victory Condition Lock

Summary:
The project must convert the preserved victory concepts into a finite, measurable ship set.

Context:
Active canon holds five main victory paths with dynastic prestige as a modifier system, not a standalone path.

Recommended default:
Ship the first complete ruleset with these active victory tracks:
- Military conquest
- Economic dominance
- Faith supremacy
- Territorial governance
- Population loyalty

Keep dynastic prestige, trade-standardization/currency, and Trueborn intervention as modifier or scenario systems until formally promoted into the main ruleset.

If delayed:
AI priorities, match pacing, UI feedback, and win/loss testing remain incoherent.

## Decision 14: Naval Scope Lock

Summary:
Naval warfare must be explicitly scheduled as either ship-blocking for launch or post-launch major expansion.

Context:
Canon includes naval warfare, but current implementation has no water movement model, ship classes, transport rules, or coastal economy.

Recommended default:
Do not treat naval as ship-blocking for the first completion-stage milestone. Preserve it in the roadmap as a major expansion phase after core land warfare, bloodline play, and faith systems are stable.

If delayed:
Map architecture and transport rules will stay unstable, but trying to fully solve naval now will slow every other system.

## Decision 15: AI Scope Lock

Summary:
The project must define what AI is required for launch-quality single-player skirmish.

Context:
Current AI gathers, builds, and attacks. Completion-stage Bloodlines needs economy, territorial contesting, faith behavior, operations awareness, and bloodline-preserving decisions.

Recommended default:
Launch-quality AI should be required to:
- Build and expand competently
- Contest resources and control points
- Defend trade and territory
- Use commanders
- React to faith and conviction pressure
- Pursue at least two victory paths beyond brute-force rushes

If delayed:
The game may look mechanically deep but fail as an actual opponent experience.

## Decision 16: Save and Session Model Lock

Summary:
The project must decide whether long matches are resumable and what exactly gets serialized.

Context:
Bloodlines is designed for 2-hour normal sessions and optional 6-10+ hour matches. That implies save/resume for practical play.

Recommended default:
Support in-match save/resume for all long-match settings. Serialize only simulation state, not renderer state.

If delayed:
Long-match mode becomes impractical and hard to QA.

## Decision 17: UI Surface Lock

Summary:
The game must lock the core UI surfaces required for completion.

Context:
Bloodlines needs RTS clarity and management depth. If the UI hierarchy stays undefined, advanced systems will bury the battle layer.

Recommended default:
Ship with these core UI surfaces:
- Battle HUD
- Command/production panel
- Territory/governance overlay
- Bloodline roster and family tree panel
- Faith/conviction panel
- Match summary and alerts feed

If delayed:
Later systems will accumulate without readable access paths.

## Decision 18: Audio/Art Production Lock

Summary:
The project must decide whether the first completion target uses placeholder presentation, stylized low-fi production art, or a fully bespoke art pass.

Context:
Current visuals are geometric and debug-forward. The game will eventually need readable silhouettes, faction identity, audio feedback, and map atmosphere.

Recommended default:
Target a stylized prototype-production pass first:
- Clean faction-color silhouettes
- Distinct unit classes
- Impactful combat and UI sounds
- Readable terrain and control-point markers

Do not wait for final premium art before finishing gameplay systems.

If delayed:
Readability, feel, and player comprehension remain weaker than the mechanics deserve.

## Decision 19: Content Pipeline Lock

Summary:
The project must decide which gameplay content is fully data-driven and which systems are allowed to remain code-defined.

Context:
The current project already benefits from JSON-driven houses, buildings, units, faiths, roles, victory conditions, and maps.

Recommended default:
Keep these fully data-driven:
- Houses
- Units
- Buildings
- Maps
- Faiths
- Victory conditions
- Bloodline roles
- Match presets

Allow only low-level simulation rules and rendering details to remain code-defined.

If delayed:
Balancing, content iteration, and future tooling will become slower and more brittle.

## Decision 20: Release Gate Lock

Summary:
The project must explicitly define what "completion stage" means.

Context:
Without a release gate, the game risks endless feature accumulation because the design archive contains many legitimate future systems.

Recommended default:
Treat completion stage as:
- Core land RTS loop solid and fun
- At least 3 houses meaningfully playable
- Bloodline commanders active
- Faith entry and low-tier divergence active
- Territory and loyalty active
- Neutral tribes and hub pressure active
- At least one AI opponent type that can pursue multiple strategies
- Save/resume, onboarding, balancing, and QA gates in place

If delayed:
The project will keep expanding laterally without ever reaching a shippable stage.

## Recommended Lock Order

These decisions should be resolved in this order:

1. Product Mode Lock
2. Runtime Stack Lock
3. Match Scale Lock
4. House Rollout Lock
5. Early Roster Lock
6. Territory Model Lock
7. Bloodline Scope Lock
8. Commander Presence Lock
9. Faith Entry Lock
10. Conviction Model Lock
11. Operations Scope Lock
12. Victory Condition Lock
13. AI Scope Lock
14. Save and Session Model Lock
15. UI Surface Lock
16. Release Gate Lock

## Immediate Recommendation

If only a few decisions are going to be locked now, lock these first:

- Bloodlines ships first as a match-based RTS, not a persistent grand-strategy sandbox
- The current browser runtime remains in place only until pathing plus commanders prove out
- Ironmark, Stonehelm, and Highborne are the first three true production houses
- Territory uses a hybrid control-point-plus-province model
- Commanders are battlefield-present bloodline members
- Faith is discovered through map interaction, not chosen at lobby start
- Naval warfare is not required for the first completion-stage milestone
