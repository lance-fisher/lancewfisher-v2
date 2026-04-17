# Bloodlines Next-Phase Execution Roadmap — Session 9

Date: 2026-04-14
Canonical root: `D:\ProjectsHome\Bloodlines`

## Purpose

This roadmap translates the continuation plan (`docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md`) into a concrete ordered execution sequence. It focuses on the immediate next wave and outlines the specific session cadence for what comes after.

This document is non-reductive. It does not propose cutting any canonical system. It selects sequencing and priority, not scope.

## Ordering Principle

Next items are ordered by:

1. **Unblocks downstream work** (high-leverage items first).
2. **Uses already-live architecture** (no new seam needed).
3. **Respects master doctrine legibility requirement** (live systems must become legible within bounded sessions).
4. **Preserves existing live state** (no regression).
5. **Advances at least one of the six vectors in the continuation plan** (civilizational / dynastic / military / faith-and-conviction / world / legibility).

## Session 9 Execution Wave

### Wave A — Full 11-state realm-condition dashboard (Vector 6)

**Problem.** `getRealmConditionSnapshot` exports all 11 canonical pressure states. HUD currently renders 6 pills. Legibility is below simulation. Master doctrine section XVI (player communication and strategic legibility) demands alignment.

**Canonical 11 states.**

1. cycle (campaign cycle progress)
2. population (ratio vs cap)
3. food (stock vs need)
4. water (stock vs need)
5. loyalty (territory-weighted)
6. fortification (primary keep tier + ceiling + threat + reserves + keep-presence + ward)
7. army (unit count + cohesion strain + engines)
8. faith (active covenant + doctrine path + intensity)
9. conviction (four-bucket ledger + derived band)
10. logistics (supplied engines vs unsupplied engines, camp count, wagon count)
11. world-pressure (tribe activity, neutral-hub state, world stage)

**Implementation plan.**

- Extend `getRealmConditionSnapshot` to surface faith, conviction, logistics, and world-pressure blocks if not already present.
- Extend HUD bar in `main.js`:
  - Expand `realm-condition-bar` from 6 pills to 11 pills.
  - Each pill shows a short label, a colored band, and a tooltip with detail.
  - Band color convention: green = healthy, yellow = strained, red = critical.
  - Pills ordered left-to-right in master doctrine priority: cycle, population, food, water, loyalty, fortification, army, faith, conviction, logistics, world-pressure.
- Ensure pill rendering does not break at narrow viewport widths.
- No simulation changes; only snapshot consumer and HUD rendering.

**Acceptance.**

- Eleven pills render live during `play.html` boot.
- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` remain green.
- `node --check` on `main.js` and `simulation.js` passes.

### Wave B — Ballista and mantlet siege-support classes (Vector 3)

**Problem.** Siege roster has ram (breach), siege_tower (support), trebuchet (bombardment). Canon next-wave identifies ballista and mantlet as the next siege-support classes. NEXT_SESSION_HANDOFF explicitly names them.

**Canonical role definition.**

- **Ballista:** ranged anti-personnel + light structural siege-support. Effective against infantry approaching the siege line and against lightly fortified structures. Requires workshop to train. Population-bearing.
- **Mantlet:** mobile cover that reduces ranged damage to nearby friendly siege crews and engineers. Does not attack. Provides a directional cover zone. Movable.

**Implementation plan.**

- `data/units.json`: add `ballista` and `mantlet` unit definitions. Both `prototypeEnabled: true`. Siege-class. Trained at `siege_workshop`.
- `data/buildings.json`: `siege_workshop.trains` list already contains `ram`, `siege_tower`, `trebuchet`, `siege_engineer`, `supply_wagon`. Add `ballista` and `mantlet` to that list.
- `src/game/core/simulation.js`:
  - Ballista combat behavior: ranged attack with moderate damage, moderate anti-structural multiplier. Similar projectile channel to bowman, different speed and damage profile.
  - Mantlet behavior: no attack. Emits a directional cover aura that reduces inbound ranged damage to friendly siege crew and engineer units within a radius. Mobile, commandable like other siege units.
  - Supply-chain awareness: both engines participate in `tickSiegeSupportLogistics` and lose efficiency when unsupplied.
- `src/game/core/renderer.js`: draw ballista and mantlet with distinct silhouettes so the battlefield remains readable. Mantlet renders a subtle facing-oriented cover outline when selected.
- `src/game/core/ai.js`: Stonehelm AI siege-line preparation extended to include ballista (anti-personnel support) and mantlet (engineer cover) when approaching a fortified keep. Queue order extends after first bombard engine (trebuchet) and engineer to include at least one mantlet for protection, then ballista for anti-personnel pressure.
- `tests/data-validation.mjs`: assert ballista and mantlet are in `units.json`, are trained at `siege_workshop`, have expected role fields.
- `tests/runtime-bridge.mjs`: assert ballista can be trained at a workshop, mantlet can be trained at a workshop, mantlet reduces inbound ranged damage to a nearby engineer unit, ballista deals damage to infantry.

**Acceptance.**

- Data validation passes with new assertions.
- Runtime bridge passes with new assertions.
- `node --check` on every simulation module passes.
- Live runtime shows workshop listing ballista and mantlet, player can train them, renderer shows distinct silhouettes.

### Wave C — Continuity and documentation round-trip

- Append session 9 entry to `00_ADMIN/CHANGE_LOG.md`.
- Update `CURRENT_PROJECT_STATE.md` with new live systems.
- Update `NEXT_SESSION_HANDOFF.md` to point at session 10 priorities.
- Update `continuity/PROJECT_STATE.json` additive fields.
- Append session 9 addendum to `MASTER_BLOODLINES_CONTEXT.md` additively.
- Reclassify affected items in `BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md` from `DOCUMENTED` or `PARTIAL` to `LIVE` where warranted.

## Post-Session-9 Execution Cadence

### Session 10 — Sabotage operation type + commander keep-presence expansion

**Sabotage operations (Vector 3).**

- Add sabotage as a new dynasty operation category in `dynasty.operations`.
- Sub-types: `gate_opening`, `fire_raising`, `supply_poisoning`, `well_poisoning`.
- Each has escrowed cost, detection risk, success formula tied to spymaster renown vs target fortification depth and ward state.
- Successful `gate_opening` opens a gatehouse for a window. Successful `fire_raising` applies burn damage over time to a target building. Successful `supply_poisoning` temporarily stops nearby food throughput. Successful `well_poisoning` applies water-crisis bias to a settlement.
- Counter-plays: patrols, spymaster presence, ward-active seats detect attempts.
- Validation: runtime-bridge assertions for each sub-type.

**Commander keep-presence (Vector 2 + Vector 3).**

- When commander is at the keep, augment ward potency, reserve muster rate, and sortie capability.
- Add `commander_at_keep` state to `getRealmConditionSnapshot` fortification block.
- Add sortie capability: commander-at-keep can order a timed sortie that temporarily pushes combat units outward with attack bonus.
- Validation: runtime-bridge assertions for commander-at-keep state, sortie bonus.

### Session 11 — Second playable house (Stonehelm) + house-select seam

- Enable `stonehelm` as `prototypePlayable: true` in `data/houses.json`.
- Ensure Stonehelm has at least one distinguishing mechanic even if initial (e.g., fortification cost discount per `TERRITORY_SYSTEM.md`).
- Add minimal URL-driven house-select: `play.html?house=stonehelm`.
- Extend tests.

### Session 12 — Longer-siege AI adaptation

- Stonehelm gains relief-window awareness (delay assault if player army approaching).
- Repeated-assault window logic (after repulse, retreat to resupply before next attempt).
- Supply protection patrols.
- Post-repulse tactical adjustment.
- Validation.

### Session 13 — Unity version decision + JSON sync + first ECS components

- Resolve Unity version (recommended: Option B, 6.3 LTS).
- Open `unity/`, run `Bloodlines → Import → Sync JSON Content`.
- Commit generated ScriptableObject `.asset` files.
- Write first ECS Components:
  - `PositionComponent`, `FactionComponent`, `HealthComponent`, `UnitTypeComponent`, `BuildingTypeComponent`, `ResourceNodeComponent`, `ControlPointComponent`, `SettlementComponent`.

### Session 14 — Unity first ECS systems

- `ResourceAccumulationSystem`, `PopulationGrowthSystem`, `RealmConditionCycleSystem`, `UnitMovementSystem`.
- Authoring + Baking layer begin.

### Session 15 — Unity bootstrap and gameplay scenes + battlefield camera

- `Scenes/Bootstrap/Bootstrap.unity` with subscene bootstrap.
- `Scenes/Gameplay/IronmarkFrontier.unity` seeded from `MapDefinitions/ironmark_frontier.asset`.
- Battlefield camera with pan and zoom (Generals/Zero Hour feel).
- Input System action map for select, drag-box, move, attack, build mode.

### Session 16 — Faith prototype enablement

- Flip `faiths.json` `prototypeEnabled: true` with safety (no runtime regression).
- Add per-covenant building progression: Wayshrine, Hall, Grand Sanctuary-equivalent.
- Add L3 faith unit roster (8 units, 2 per covenant, per doctrine path).
- Validation.

### Session 17 — Ironmark Blood Production deepening + Hartvale Verdant Warden entry

- Ironmark Blood Production: when Ironmark trains non-worker combat units, blood cost beyond the current levy event also temporarily depresses local population growth rate. Canon describes this as population cost under attritional war.
- Hartvale Verdant Warden: add to `data/units.json` with `house: "hartvale"`, `prototypeEnabled: false` until Hartvale is playable.

### Session 18 — Marriage + succession interface panel

- Marriage: head-of-household initiate / accept marriage with another house. Child appears after in-world time gap. Loyalty + diplomatic state shift.
- Polygamy restricted to Blood Dominion and Wild.
- Succession interface panel UI with five impact metrics.

### Session 19 — Dual-clock declaration seam (minimal)

- On battle resolve, generate a Declaration summary with declared in-world elapsed time (3 to 6 months skirmish, 1 to 2 years major, 3 to 5 years siege).
- Events queue between battles, commitment phase during which world advances.

### Session 20 — Naval foundation on current map

- Add water tiles to map.
- `harbor_tier_1` building.
- `fishing_boat`, `scout_vessel`, `war_galley` vessel types.
- Naval combat hooks.

### Session 21 — Continental architecture seam

- Add secondary landmass separated by water.
- Require vessel for crossing.
- Secondary continent counts toward victory thresholds per canon.

### Session 22 — Ten-terrain extension + five-stage match scaffolding

- Enforce ten canonical terrain types with per-terrain resource profiles and movement costs.
- Scaffold stage transitions: Founding → Expansion and Identity → Encounter and Establishment → War and Turning of Tides → Final Convergence.

### Session 23 — Political events cascade minimal

- Event firing architecture in `simulation.js`.
- Three initial events: Succession Crisis, Covenant Test, Holy War.
- Events read realm condition and fire on thresholds.

### Session 24 — Operations system expansion

- Covert operations: assassination, sabotage (already scaffolded), espionage.
- Faith operations: missionary, conversion, holy war declaration.
- Military operations: structured raid, ambush, supply-line strike.
- Detection and counterplay.

### Sessions 25–30 — Content depth waves

- L4 faith unit roster.
- L5 apex faith units.
- Apex faith structures.
- Conviction milestone powers + dark-extremes world pressure.
- Born of Sacrifice champion lifecycle scaffold.
- Lesser houses promotion pipeline.
- Save / resume.

## Completion-Stage Gate Mapping

| Gate | Session Addressed | Verdict At Session |
|---|---|---|
| 1. Battle Layer | S9, S12 | Deepened by ballista, mantlet, longer-siege AI |
| 2. Territory Layer | S10, S11 | Sabotage + Stonehelm playable |
| 3. House Layer | S11, S17 | Stonehelm playable, Hartvale entry |
| 4. Bloodline Layer | S10, S18, S29 | Keep-presence + marriage + lesser houses |
| 5. Faith and Conviction | S16, S25, S26, S27 | Prototype enablement + tier roster + apex + milestone powers |
| 6. Operations Layer | S10, S24 | Sabotage + full operations system |
| 7. World Pressure | S19, S20, S21, S22, S23 | Dual-clock + naval + continental + stages + events |
| 8. AI Layer | S12 | Longer siege + multi-front coordination |
| 9. UX Layer | S9, S15, S18 | 11-state HUD + Unity camera + succession panel |
| 10. Technical Layer | S13–S15, S30 | Unity ECS foundation + save/resume |
| 11. Content Layer | S16–S17, S20–S22 | Faith units + houses + terrains |
| 12. QA and Production | continuous | Tests green each session |

## Validation Workflow For Every Session

```powershell
Set-Location D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js
python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines
# Open http://localhost:8057/play.html to confirm live runtime
```

## Closing Statement

This roadmap does not reduce Bloodlines. It surfaces the canonical systems in runtime one layer at a time, alternating between depth additions and legibility additions, while preserving the full preserved design corpus unchanged. The direction of motion is always toward the full intended grand-scale game.
