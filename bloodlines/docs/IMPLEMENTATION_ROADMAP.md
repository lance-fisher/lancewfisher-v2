# Implementation Roadmap

## Guiding Principle

Bloodlines should grow from a readable RTS battle core into the full dynasty-faith-territory game. Each phase must leave behind runnable game code, not just more text.

Two companion files now govern convergence:

- `docs/DEFINITIVE_DECISIONS_REGISTER.md` for decisions that must be locked
- `docs/COMPLETION_STAGE_GATES.md` for the requirements that define completion-stage production

## Phase 0: Project Cleanup, Inventory, Build/Run Verification

- Lock `deploy/bloodlines/` as the authoritative root.
- Preserve archive documents and map their authority.
- Add Bloodlines-specific README, runbook, known issues, inventory, and reality report.
- Verify static serving and zero-build runtime path.
- Add data validation for gameplay definitions.

## Phase 1: Basic RTS Prototype

- Launch scene and skirmish entry point.
- Canvas playfield plus DOM HUD.
- Camera pan, selection, right-click commands.
- One map with two bases and resource pressure.
- One playable house: Ironmark.
- Basic enemy AI.

## Phase 2: Base Building and Resource Gathering

- Worker gather-return loop for gold and wood.
- Passive food and water support structures.
- Housing as population-cap infrastructure.
- Construction system with placement validation and build progress.
- Drop-off behavior, build queues, and economy feedback.

## Phase 3: Unit Production and Combat

- Command Hall and Barracks production.
- Militia, Swordsmen, Bowmen available in prototype layer.
- Infantry and ranged combat tuning.
- Attack waves, defensive response, death handling, basic morale hooks.
- Early combat readability pass.

## Phase 4: Territory and Population Systems

- Province overlay and territory ownership.
- Loyalty per territory.
- Population growth and shortage penalties tied to food/water.
- Capture logic that distinguishes occupation from stable control.
- Expansion pressure around neutral and tribal zones.

## Phase 5: House Selection and Faction Identity

- House-select screen.
- Subtle shared-roster Off/Def variations per house.
- Ironmark full implementation first.
- Reintroduce other eight houses only at settled-canon depth unless new directives extend them.
- House color, icon, banner, and HUD identity pass.

## Phase 6: Bloodline Character System

- Bloodline roster model.
- Commanders, Governors, Diplomats, Ideological Leaders, Merchants, Sorcerers.
- Named member generation and role assignment.
- Bloodline member bonuses on armies and territories.
- Early succession and death handling.

## Phase 7: Faith and Conviction Systems

- Stage-end faith selection.
- Intensity tracking and maintenance cost.
- Light/dark doctrine scaffolding.
- Conviction event accounting and threshold bands.
- Covenant buildings and low-tier faith operations.

## Phase 8: Operations System

- Rogue and Mystic unit classes.
- Covert, faith, and military operations as strategic commitments.
- Detection, failure states, and conviction/diplomacy consequences.
- Timing system scaffolding for cycles, moons, and seasonal modifiers.

## Phase 9: AI Kingdoms and Minor Tribes

- Tribe camps, neutral hostility, tribute/alliance/conquest outcomes.
- More than one enemy kingdom.
- AI house behavior models and counter-coalition behavior.
- Diplomatic state machine and emergent front pressure.

## Phase 10: Neutral City and Trueborn Intervention

- Neutral hub economy and world intelligence layer.
- Trueborn favor/trust model.
- Late-match activation threshold.
- Trueborn rise arc and anti-hegemon response.

## Phase 11: UI Polish and Match Setup

- House select, match settings, map setup.
- Minimap upgrade, notifications, queue visibility, alerts.
- Bloodline and faith screens.
- Territory/governance overlays.
- Tutorial and onboarding for the battle layer.

## Phase 12: Balancing, Testing, Packaging, and Production Readiness

- Data-driven tuning pass.
- Smoke tests and regression checks.
- Save/load and replay planning.
- Browser compatibility review.
- Asset replacement plan for placeholder visuals.
- Packaging and deployment hardening.

## Production Lock Milestones

- Milestone A: lock product mode, runtime stack direction, match scale, house rollout, early roster, and territory model.
- Milestone B: lock bloodline MVP scope, commander presence, faith entry, conviction model, operations scope, and victory-condition ruleset.
- Milestone C: lock AI launch scope, save/session model, UI surface list, content pipeline rules, and completion-stage release gates.

## Immediate Next Slice Objectives

The current vertical slice should be hardened in this order:

1. Improve unit pathing and formation readability.
2. Turn territory capture into full territorial contesting with loyalty and occupation effects.
3. Add Stonehelm as the second fully playable house and Highborne as the third-house target.
4. Add named bloodline commanders as tactical anchors.
5. Add Stage 1 faith discovery and low-tier faith scaffolding.
6. Replace wave-only AI with territory-aware strategic AI.
