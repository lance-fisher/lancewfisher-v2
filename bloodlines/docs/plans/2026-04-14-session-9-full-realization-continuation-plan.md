# Bloodlines Full Realization Continuation Plan — Session 9

Date: 2026-04-14
Canonical root: `D:\ProjectsHome\Bloodlines`

## Intent

This plan defines how Bloodlines continues from its 2026-04-14 state toward the full intended grand-scale realization the creator mandated in the Master Design Doctrine. It is additive. It does not cut. It does not replace earlier plans. It builds on top of `docs/IMPLEMENTATION_ROADMAP.md`, `docs/COMPLETION_STAGE_GATES.md`, `docs/plans/2026-04-14-fortification-siege-population-legibility-wave.md`, and every prior session's state-of-game report.

This document is the continuation plan deliverable for session 9. Its sibling document `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` provides the concrete ordered roadmap for the next several sessions.

## Posture

- **Preservation first.** The canonical scope stays intact. Nothing documented in `01_CANON/`, `04_SYSTEMS/`, `05_LORE/`, `06_FACTIONS/`, `07_FAITHS/`, `08_MECHANICS/`, `09_WORLD/`, `10_UNITS/`, `11_MATCHFLOW/`, `12_UI_UX/`, `13_AUDIO_VISUAL/`, or `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md` is simplified to fit runtime limits.
- **Additive runtime extension.** The browser reference simulation is the working spec. Unity is the production target. Both lanes advance in parallel rather than the Unity lane invalidating the browser lane.
- **Two working lanes, one direction.** Browser advances the simulation surface. Unity advances the production surface. Both converge on the full intended game.
- **Legibility on equal footing with depth.** Every system that becomes live in simulation becomes legible in UI within a bounded number of sessions. A system that is live in code but invisible to the player is implementation debt, not progress.
- **No reversion of status.** A system marked `LIVE` in `BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md` does not regress. Future sessions only move items from `DOCUMENTED` toward `LIVE`.

## Strategic Thesis

Bloodlines is a civilizational dynasty war game. Its full realization requires simultaneous depth in six vectors.

1. **Civilizational depth** — population, water, food, land, forestry, supply, logistics.
2. **Dynastic depth** — bloodline members as real actors, marriage, succession, lesser houses, captivity consequence.
3. **Military depth** — multi-dimensional land and naval doctrine, commanders and generals, delegated and direct command, fortification, siege.
4. **Faith and conviction depth** — four covenants with Light/Dark paths and 5-tier ladders, independent conviction spectrum, irreversible late-game divergence.
5. **World depth** — continental geography, five-stage match, multi-speed time with declared dual-clock, world pressure, Trueborn late return, up to 10 players plus AI kingdoms plus minor tribes.
6. **Legibility depth** — theatre-of-war strategic zoom, 11-state realm dashboard, ward iconography, governor iconography, naval-and-continental visual awareness, keep interior as core UI anchor.

The continuation plan advances each vector without letting any single vector sprint far ahead of the others. The goal is not to finish one vector and start the next. The goal is to keep all six vectors growing so the game stays coherent as it scales.

## Six-Vector Growth Model

### Vector 1: Civilizational depth

**Current live foundation.** Six primary resources, passive trickle, smelting chain, 90-second realm cycle, famine, water crisis, cap pressure, supply-camp-wagon siege sustainment, unsupplied-assault penalty.

**Next additions (ordered, additive, non-reducing).**

- Water infrastructure tiering: `reservoir_cistern`, `aqueduct_segment`, `running_water_hall`. Each tier increases water cap and reduces water-crisis risk. Tied to settlement class ceiling. Aqueducts connect multiple settlements.
- Army water sustainment outside siege: generic supply-wagon food/water throughput to nearby friendly military units, with unsupplied armies accruing upkeep strain.
- Food storage and transport: `granary` building tied to settlement class. Food can be transported between settlements via supply wagons.
- Land transformation: `forest` to `farmland` conversion over time tied to lumberer and farmer labor. Requires wood harvest threshold and worker investment.
- Forestry sustainability: replanting mechanic on cleared forest tiles. `forestry_camp` building. Long-term managed wood cycle.
- Negative upkeep and desertion: army-wide upkeep pool read from food/water stock; sustained negative upkeep deducts soldiers per cycle; morale thresholds lead to desertion.

### Vector 2: Dynastic depth

**Current live foundation.** Commander battlefield presence, capture-vs-kill cascade, succession chain, captive and fallen ledgers, dynasty operations (ransom, rescue, demand).

**Next additions.**

- Commander keep-presence expansion: when commander is at keep, defensive sight, ward potency, reserve muster rate, and sortie capability all increase materially.
- Generals as separate class from commanders: secondary bloodline role tied to delegated-resolution and unit-morale bonus. General at unit stack gives stack-level morale and speed bonus.
- Marriage system (minimal): head of household can propose or accept marriage with another house; child appears after an in-world time gap; loyalty and diplomatic state shift on marriage.
- Polygamy by faith: Blood Dominion and Wild allow multiple active spouses; other faiths do not.
- Cross-dynasty children: marriage between houses produces child with mixed bloodline metadata; defection slider logic per master doctrine.
- Succession interface panel: dedicated UI for heir designation, with visible impact metrics (legitimacy, loyalty, faith, conviction, age).
- Lesser houses from war heroes: promotion pipeline at commander renown threshold to become a lesser-house cadet branch; optionally AI-autonomous.
- Sorcerer / Mysticism runtime: Sorcerer role provides faith-amplified offensive/defensive abilities tied to covenant and intensity tier.

### Vector 3: Military depth

**Current live foundation.** Melee and ranged combat, projectiles, auto-acquire, assault cohesion strain, wall/tower/gatehouse/keep, ram/tower/trebuchet, engineer, supply camp, supply wagon, Stonehelm siege preparation + sustainment + refusal, faith-integrated wards.

**Next additions.**

- Ballista: ranged anti-personnel + light structural siege-support class.
- Mantlet: mobile cover that reduces ranged damage to nearby friendly siege crews and engineers.
- Sabotage operations: new dynasty operation types — gate opening, fire raising, supply poisoning, well poisoning. Each with different cost, risk, detection logic, and consequence.
- Longer-siege AI: Stonehelm gains relief-window awareness (delay assault if player army is approaching), repeated-assault window logic (after repulse, retreat to resupply before next attempt), and supply protection patrols.
- Multi-front coordination AI: Stonehelm can stage pressure on one front while siege lines form on another.
- Isolation / starvation operations: operation type that cuts a settlement's external supply; starvation becomes a parallel path to breach.
- Relief-army mechanic: if a friendly army reaches a besieged keep within the relief window, siege cohesion strain accrues on the besieger.
- Land doctrine axis (defensive / balanced / offensive) as player commitment with material consequences on unit caps, production speed, fortification cost, supply efficiency.
- Generals + commanders interaction with delegated resolution: dedicated Declaration seam introduces delegated outcomes for battles commanded by generals while commander is absent.
- Mining / counter-mining as siege-support operations.

### Vector 4: Faith and conviction depth

**Current live foundation.** Discovery and commitment, doctrine path Light/Dark, intensity growth, four-covenant ward profiles, Blood Dominion sacrificial reserve surge, conviction four-bucket ledger with derived band.

**Next additions.**

- Flip `faiths.json` prototypeEnabled true with per-faith building progression (Wayshrine → Hall → Grand Sanctuary).
- Faith-specific L3 unit roster (8 units: 2 per covenant, one per doctrine path).
- Faith-specific L4 unit roster (8 units: same pattern).
- Faith-specific L5 apex units (4 units: The Unbroken, The Sacrificed, The Mandate, The First Wild).
- Apex faith structures (one per covenant, tied to faith victory path, late-game-locked).
- Faith intensity 5-tier ladder mechanical consequences: each tier unlocks distinct bonuses or obligations.
- Conviction milestone powers per band: Apex Moral and Apex Cruel each have unique unlocks; Moral, Neutral, Cruel have graduated modifiers.
- Conviction-to-loyalty feedback: dark-aligned conviction near Apex Cruel reduces population loyalty in moderate-conviction territories.
- Dark-extremes world pressure: Apex Cruel or sustained Desecration triggers world-reaction events (coalition risk, faith intervention, tribe reaction).
- Covenant Test mechanics: faith-gated choices that lock or unlock doctrine path branches.
- Irreversible late-game commitments at apex: L5 apex commitment locks certain strategic paths.

### Vector 5: World depth

**Current live foundation.** 72×48 tile map, 3 control points, 2 sacred sites, 13 resource nodes, tribes faction, 90-second cycle, single real-time clock.

**Next additions.**

- Five-stage match structure: Founding, Expansion and Identity, Encounter and Establishment and Neutral City, War and Turning of Tides, Final Convergence. Stage transitions trigger events.
- Three overlay phases: Emergence, Commitment, Resolution. Overlay state read by operations and events.
- Phase Entry: match setup option to enter at a later stage with accelerated-build starting state.
- Dual-clock declaration seam (minimal): post-battle declaration of in-world elapsed time; events queue between battles; commitment phase during which players plan and the world advances.
- Dynastic clock: generational-time tracking tied to bloodline-member age and succession.
- Strategic-zoom theatre-of-war view: camera can pull back beyond battlefield grid into a theatre view showing operational fronts, fog of war scaled to zoom, bloodline seats visible.
- Continental architecture: introduce water bodies on map, distinguish home continent from secondary continent, require vessel for crossing.
- Neutral hub / trade center: non-faction settlement on the map with trade and diplomatic effect.
- Trueborn late-game return: if neutral city is not conquered and world conditions met, Trueborn ancient line re-emerges.
- Dark-extremes world pressure as a stage event.
- Up to 10 players + AI kingdoms + minor tribes: scale up data schema to support N-faction maps.
- Ten canonical terrain types: extend terrain grid to enforce Reclaimed Plains, Ancient Forest, Stone Highlands, Iron Ridges, River Valleys, Coastal Zones, Frost Ruins, Badlands, Sacred Ground, Tundra with terrain-specific resource profiles and movement costs.
- Four canonical historical eras: record match-time-sweep tied to dynastic clock; events and visual shift at era boundaries.
- Naval foundation: add water tiles, `harbor_tier_1` building, `fishing_boat` + `scout_vessel` + `war_galley` vessel types, naval combat hooks. Fire Ship and Capital Ship come in later passes.
- Naval doctrine axis as player commitment.
- Naval trade routes between harbors.

### Vector 6: Legibility depth

**Current live foundation.** 10-pill resource bar, 6-pill realm-condition bar, reserve-duty renderer markers, siege engine visual differentiation, fortification tier labels, dynasty panel, faith panel, message log.

**Next additions.**

- Full 11-state realm-condition dashboard. Expand HUD from 6 pills to 11 pills aligned with `getRealmConditionSnapshot`: cycle, population, food, water, loyalty, fortification, army, faith, conviction, logistics, world-pressure.
- Keep interior UI anchor: dedicated screen reflecting bloodline/faith/conviction/empire state per master doctrine `12_UI_UX/UI_NOTES.md` (60KB).
- Persistent top-bloodline-members HUD panel: always-visible strip of head of bloodline, heir, commander, governor with name, status, faith, conviction.
- Ward iconography on fortified settlements: visible pyre, altar, edict, root icons.
- Governor specialization iconography (border, city, keep badges).
- Territorial overlay: province-tier view that sits above control points; hybrid tactical-plus-strategic per Decision 6.
- Strategic-zoom theatre view with continental awareness, trade lanes, supply corridors, naval dominance zones.
- Match setup / lobby / house-select: player picks house, starting stage, opposing kingdoms, map.
- Tutorial / onboarding: guided first-match flow.
- Audio layer: covenant motifs, battle readable-feedback audio, strategic-zoom ambient shifts.

## Parallel Lane Strategy

### Browser reference simulation lane

The browser lane is the working spec. It stays live and green through every session. Every new canonical system must be validated in the browser lane before migrating into Unity.

Each session in this lane targets 3 to 7 specific additive runtime items. A session closes with:

- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` passing.
- `node --check` on every simulation module passing.
- `BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_N.md` appended.
- `tasks/todo.md` `Completed` section extended.
- `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`, `00_ADMIN/CHANGE_LOG.md` additively updated.
- `BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md` items reclassified as live items move from `DOCUMENTED` or `PARTIAL` toward `LIVE`.

### Unity production lane

The Unity lane is the production target. It advances once per user-decision gate or once per ready-to-migrate phase in `docs/unity/PHASE_PLAN.md`.

Session-level Unity work:

- Resolve version alignment (Option B recommended: 6.3 LTS).
- Open `unity/` and run `Bloodlines → Import → Sync JSON Content`. Commit generated ScriptableObjects.
- Write first ECS components (U1 through U4).
- Write first ECS systems (U5 through U8).
- Write authoring and baking.
- Bootstrap scene, Ironmark Frontier gameplay scene.
- Battlefield camera with pan and zoom, Input System action map.
- Persistent top-bloodline-members HUD panel.
- Strategic-zoom theatre view.
- Fortification Unity phases (U17 through U22).

### Documentation and canon lane

Every new system added in either lane must round-trip to canon documents additively. `02_SESSION_INGESTIONS/` holds raw notes. `04_SYSTEMS/` holds settled specs. `01_CANON/CANON_LOCK.md` records locks. `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.X.md` holds exports.

### Preservation and provenance lane

`archive_preserved_sources/` continues to absorb any new outside Bloodlines material. `SOURCE_PROVENANCE_MAP.md` is updated. `CONSOLIDATION_REPORT.md` records each consolidation. `governance/` continues to hold imported workspace overlays and parent-repo governance.

## Non-Negotiable Session-Level Rules

These rules apply to every session from session 9 forward.

1. No scope reduction.
2. No silent substitution of thinner mechanics for deeper canon mechanics.
3. No removal of preserved material. No deletion of `archive_preserved_sources/`, `19_ARCHIVE/`, or `governance/` content without explicit creator authorization.
4. Additive edits only for canon files. New material goes into `02_SESSION_INGESTIONS/` with a dated file, or into a dated section at the bottom of the relevant canon file.
5. Tests must remain green at session close. Do not merge work that breaks `data-validation.mjs` or `runtime-bridge.mjs`.
6. Continuity files must be updated before session close.
7. State analysis and gap analysis reports must be written for any session that materially changes the runtime surface.
8. Any system that becomes live in simulation must become legible in UI within at most two sessions.
9. Dual-lane discipline: the browser lane stays as working spec; the Unity lane does not skip the browser validation.
10. Preserve the 2026-04-14 master design doctrine as the canonical compass. Any new canon must cite or extend it, not contradict it.

## Exit Criteria for Each Vector

Bloodlines is complete when every vector reaches the following.

- **Civilizational depth complete** when water mastery progression, food and land transformation, forestry cycle, and supply-driven desertion all drive strategic decisions.
- **Dynastic depth complete** when marriage, inheritance, cross-dynasty children, polygamy-by-faith, lesser houses, and the full 9-role bloodline roster have material runtime effect.
- **Military depth complete** when land doctrine axis, naval doctrine axis, delegated-resolution and direct-command dual path, generals and commanders, fortification layered defense with apex tier, siege with full operational toolkit, and AI long-siege adaptation are all live.
- **Faith and conviction depth complete** when four covenants are fully operational with Light/Dark paths, 5-tier intensity, 16 faith units, 4 apex units, 4 apex structures, conviction milestone powers, and dark-extremes world pressure are live.
- **World depth complete** when the five-stage structure, three overlay phases, dual-clock architecture, strategic-zoom, continental architecture with 2+ secondary continents, Trueborn neutral city arc, up to 10 players plus AI kingdoms, and ten canonical terrain types are live.
- **Legibility depth complete** when full 11-state dashboard, keep interior UI, persistent bloodline HUD, ward iconography, governor iconography, territorial overlay, strategic-zoom theatre view, match setup, tutorial, and audio are all live.

## Session 9 Immediate Scope

The concrete session 9 implementation scope is defined in the execution roadmap (`docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md`). Summary:

- Expand HUD from 6 pills to 11 pills against the already-live `getRealmConditionSnapshot` export (Vector 6 step).
- Add `ballista` and `mantlet` to the siege roster (Vector 3 step).
- Wire simulation, renderer, and AI awareness of the new engines.
- Extend `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` additively.
- Update continuity files.

## Non-Exhaustive Later Sessions Outline

The sessions after 9 should aim roughly in this order, adjusting for creator direction at each turn. This is guidance, not a rigid plan.

- Session 10: sabotage operations type, commander keep-presence deepening.
- Session 11: second playable house (Stonehelm), house asymmetry verification.
- Session 12: longer-siege AI adaptation, relief-window logic.
- Session 13: Unity version decision landed, JSON sync, first ECS components.
- Session 14: Unity first ECS systems, bootstrap and gameplay scenes.
- Session 15: Unity battlefield camera, input system action map, top-bloodline-members panel.
- Session 16: faith prototype enablement, faith building progression, L3 unit roster.
- Session 17: Ironmark Blood Production deepening, Hartvale Verdant Warden entry.
- Session 18: marriage + succession interface panel.
- Session 19: dual-clock declaration seam minimal.
- Session 20: naval foundation on current map.
- Session 21: continental architecture seam.
- Session 22: ten-terrain extension, five-stage match structure scaffolding.
- Session 23: political events cascade minimal.
- Session 24: operations system (covert, faith, military) expansion.
- Session 25: L4 faith unit roster.
- Session 26: apex faith structures and L5 units.
- Session 27: conviction milestone powers, dark-extremes pressure.
- Session 28: born of sacrifice champion lifecycle scaffold.
- Session 29: lesser houses promotion pipeline.
- Session 30: save / resume.

This sequence will change. The point is not the specific number. The point is that the scope keeps growing toward the full Bloodlines vision without any canonical system being abandoned.

## Governance of This Plan

This plan is itself additive. It does not replace `docs/IMPLEMENTATION_ROADMAP.md` or `docs/COMPLETION_STAGE_GATES.md`. It layers above them with the session-by-session cadence and the six-vector growth model. If a future plan needs to update the six-vector model, it should be appended, not rewritten.
