# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14  
Session: 4  
Canonical root: `D:\ProjectsHome\Bloodlines`

## Purpose

This document is a working diagnosis of the actual Bloodlines repository state as it exists after the reserve-cycling wave. It is not a pitch summary and not a reduced-scope interpretation. It is intended to answer four questions:

1. What is real in code and data right now
2. What is partial but structurally meaningful
3. What remains only in canon, doctrine, or roadmap
4. What the next implementation waves must do to continue building upward toward the full game

## Repository Reality

### Runtime lanes

- Browser reference simulation is the most advanced executable gameplay lane.
- Unity production lane exists as the canonical engine lane but remains blocked on editor-version alignment before ECS code should begin.
- The browser lane is currently the working gameplay specification for the Unity lane, not a throwaway prototype.

### Content inventory

- Houses in data: `9`
- Prototype-playable houses: `ironmark`
- Units in data: `8`
- Prototype-enabled units: `villager`, `militia`, `swordsman`, `bowman`, `tribal_raider`, `ram`
- Buildings in data: `14`
- Prototype-enabled buildings: `command_hall`, `dwelling`, `farm`, `well`, `lumber_camp`, `mine_works`, `quarry`, `iron_mine`, `barracks`, `wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1`, `tribal_camp`
- Faiths in data: `4`
- Playable map definitions in active browser lane: `1`

This confirms the project already has the canonical macro identity in data form, but only a first operational slice is live in simulation.

## System Diagnosis

### Overall architecture

- Strongest current architecture: data-driven browser simulation with JSON content, runtime validation, canon-linked documentation, and preserved continuity.
- Strongest structural decision: browser lane and Unity lane are now separated cleanly enough that the browser simulation can continue advancing canon while Unity is brought into production shape.
- Current weakness: the browser lane is still a single-map, single-house live slice; the Unity lane has importer and folder architecture but no ECS runtime yet.

### Gameplay foundation

- Real-time economy, worker gathering, unit training, construction, combat, territory capture, sacred-site exposure, dynasty assignment, fortification tiering, assault strain, and realm-condition cycling are all live.
- The game already feels like an RTS system stack rather than disconnected experiments.
- Missing foundation: formal strategic zoom, larger theatre continuity, second playable house, deeper production chains, siege logistics, and long-match clocking.

### Combat systems

- Combat is live for melee, ranged projectiles, siege-vs-structure multipliers, commander aura, and assault-failure penalties.
- Fortifications now matter mechanically; walls are not generic health bars.
- New reserve-cycling layer is live: fortified keeps now rotate wounded defenders back to triage and commit fresh reserves forward.
- Missing combat pillars: cavalry, elites, formation pressure, wall combat roles, engineer/sapper roles, layered breach stages, artillery suppression, multi-front timing, and developed defender sorties.

### Economy systems

- Live resources: gold, food, water, wood, stone, iron, influence.
- Canonical smelting dependency is live: iron output requires wood fuel at `0.5`.
- Control-point trickle and governor bonus already connect economy to territory.
- Missing economy pillars: trade network logic, logistics corridors, supply wagons, upkeep pressure, garrison-specific population commitments, engineer populations, and broader house-specific economic divergence.

### Population and housing systems

- Population total, cap, reserved population, growth cadence, famine, water crisis, and cap pressure are live.
- The 90-second realm cycle is functioning and now visible in HUD form.
- Missing depth: housing/infrastructure chains beyond dwellings, social order consequences, explicit outmigration modeling beyond cycle hits, specialist populations, and the full 11-state dashboard.

### Bloodline systems

- Bloodline roster state is real, not cosmetic.
- Live: head/heir/commander/governor chain, commander attachment to battlefield unit, succession cascade, legitimacy changes, captive ledger, fallen ledger, ransom influence trickle, captured/displaced/fallen outcomes.
- Missing depth: the 20-member full roster, marriages, children, rescue operations, ransom choice UI, specialty-role execution for diplomat/merchant/sorcerer/spymaster, and persistent cross-match dynasty consequences.

### Faith systems

- Four covenants exist in data and live runtime.
- Sacred-site exposure, faith discovery, doctrine commitment, doctrine effect application, intensity growth, and UI surfacing are live.
- Missing depth: covenant structures, faction faith infrastructure, L3/L4/L5 rosters, Apex manifestations, faith-integrated fortification expressions, and event/prompt chains around doctrine divergence.

### Conviction systems

- Conviction buckets, derived score, and banding are live.
- Runtime actions already write to conviction through governance, conquest, doctrine, capture, fortification, and famine paths.
- Missing depth: larger event vocabulary, stronger late-game civilizational divergence, and consequential gameplay branches tied to conviction bands.

### Territorial systems

- Control points are live.
- Ownership, occupation, stabilization, loyalty, governor assignment, commander-assisted capture, and settlement-class metadata are real.
- Missing depth: province logic, multi-layer territorial administration, siege-state loyalty behavior, regional stabilization programs, and strategic map aggregation.

### Faction identity

- Canonically there are 9 houses in data.
- In real gameplay, Ironmark is the only prototype-playable house with a live blood levy distinction and current authored battlefield loop.
- Stonehelm functions as the first enemy kingdom identity, but not yet as a full second playable house.
- Missing depth: full mechanical divergence for all houses, unique production lanes, signature units, covenant preferences, diplomacy biases, and long-form identity arcs.

### UI / UX state

- Browser HUD now carries a real layered command surface: resource bar, realm-condition pills, dynasty panel, faith panel, message log, selection and command panels.
- Control-point labels and fortification state are visible in-world.
- Missing depth: always-visible top bloodline strip, contextual dynastic prompts, full 11-state realm dashboard, richer selection context, and strategic zoom continuity.

### Content pipeline maturity

- JSON content is strong enough to continue scaling the browser lane.
- Unity importer and ScriptableObject definitions are in place for the current data corpus.
- Missing pipeline support: broader authoring for houses, unit tiers, settlement templates, siege assets, map library, and future content import workflows that can handle large-scale growth cleanly.

### Simulation maturity

- Browser simulation has crossed from toy behavior into an actual multi-system reference model.
- Current maturity is best described as “systems-rich early battlefield slice.”
- Missing maturity: larger battles, stronger AI planning, siege preparation, garrison specialist populations, broader transport/logistics loops, world transformation, and long-match pacing layers.

### World progression maturity

- The match currently begins and escalates as a frontier battlefield with territorial contest and covenant discovery.
- Missing world progression: five-stage match structure, declared-time dual-clock, year-scale siege continuity, late-game civilizational transformation, and Trueborn / neutral trade-hub world systems if and where canon calls for them.

### Art and presentation pipeline

- Browser lane uses readable silhouettes and a clearer HUD than before.
- Unity asset root is organized for real production work.
- Missing pipeline: final battlefield art direction, unit/structure model pipeline, animation sets, VFX language, faction presentation package, and strategic-view rendering layer.

### Audio needs

- Audio middleware location is reserved in Unity.
- No substantive audio pipeline is live yet.
- Missing: battlefield response set, UI event set, faith/conviction tone language, dynasty event scoring, ambience, siege soundscape, and Wwise integration wave.

### Save / continuity systems

- Cross-session development continuity is strong.
- In-game save/load is not present in either the browser lane or Unity lane.
- Missing: runtime save-state serialization, campaign/session continuity rules, deterministic replay strategy if desired, and test fixtures for persisted state.

### AI architecture

- Enemy AI can gather, build, train, claim territory, commit to doctrine, and now refuse underforce keep assaults.
- Neutral tribal pressure exists.
- Missing AI depth: siege preparation, logistics, scouting, breach planning, reserve understanding on the attacker side, strategic retreat, and multi-front war logic.

### Tooling and authoring

- Governance, continuity, provenance, environment reporting, and importer structure are all above-average for the current stage.
- Missing tooling: Unity ECS authoring/baking runtime, map-seed authoring, content diff tools, stronger validation coverage for expanding JSON corpora, and automated crosswalk checks between browser/live content and Unity-imported assets.

## Gap Map Against Full Canon

### Live pillars already anchored

- Multi-resource economy including stone and iron
- Realm pressure cycle with famine, water crisis, and cap pressure
- Territory capture plus loyalty stabilization
- Bloodline consequence chain
- Covenant discovery and doctrine commitment
- Fortification classes and settlement defense ceilings
- First siege engine
- Assault-failure penalty logic
- HUD surfacing for core pressure states

### Major missing pillars

#### Unity ECS battlefield lane

- Missing capability: actual ECS components, systems, authoring, baking, scene bootstrap, camera/input, and HUD runtime.
- Why it matters: Unity is the production engine; without the ECS lane the canonical project is not yet a playable production build.
- Required path: resolve editor version, run importer sync, then begin ECS foundation exactly in the approved component/system order.

#### Fortification ecosystem beyond first-tier response

- Missing capability: engineer assignment, repair workshops, layered defensive rings beyond the current building tier counters, specialist garrison populations, and faith-warded defensive expressions.
- Why it matters: Bloodlines defense must remain viable deep into long matches, not collapse into generic static buildings.
- Required path: continue reserve cycling into specialist garrison lanes, add keep-specific bonuses, then add faith-integrated fortification responses and advanced fortification infrastructure.

#### Siege preparation depth

- Missing capability: siege workshop, siege tower, trebuchet, logistics, scouting, breach planning, and attacker-side staging logic.
- Why it matters: canonical AI and player siege are supposed to be campaign-scale commitments, not direct rushes with a single ram.
- Required path: add siege production infrastructure and next engine classes in the browser lane, then teach the AI to prepare rather than merely refuse.

#### House-scale asymmetry

- Missing capability: more than one genuinely playable house with real divergence.
- Why it matters: the full game is a major-house RTS, not a single-faction battlefield demo.
- Required path: author Stonehelm as the second full playable house with strategic identity, unit mix, economy bias, and dynastic flavor.

#### Strategic layer continuity

- Missing capability: battlefield-to-theatre zoom continuity, province aggregation, scouting intelligence abstraction, and wider fog-of-war rules.
- Why it matters: the game promise includes commanding the same war at multiple scales.
- Required path: begin in Unity once camera/input/HUD foundations exist; preserve browser lane as the systems truth.

#### Full bloodline command presence

- Missing capability: always-visible top bloodline members HUD, richer role activity, rescue/ransom prompts, and expanded roster logic.
- Why it matters: bloodline command presence is one of Bloodlines’ core differentiators.
- Required path: next browser wave should add rescue/ransom operations; Unity HUD should surface top bloodline presence as soon as the ECS lane is live.

## Immediate Build Direction After Session 4

### Browser lane

1. Extend reserve cycling into explicit governor specialization and commander keep-presence bonuses.
2. Add faith-integrated fortification responses at the keep.
3. Add siege production infrastructure plus `siege_tower` and `trebuchet`.
4. Deepen enemy AI from refusal into real siege preparation and escorted commitment.
5. Expand the realm-condition HUD toward the full 11-state view.

### Unity lane

1. Resolve the editor-version lock with Lance.
2. Open `unity/` in the chosen editor and run `Bloodlines -> Import -> Sync JSON Content`.
3. Start the ECS foundation exactly from components through systems, then authoring/baking, then bootstrap scene, then battlefield camera/input/HUD.

## Conclusion

Bloodlines is no longer in a “blank slate” state. It now has a real executable reference lane with meaningful dynastic, economic, territorial, faith, fortification, and realm-pressure behavior. The biggest present deficit is not lack of concept. It is the distance between the current rich battlefield slice and the eventual full-scale production game: more houses, more siege depth, more strategic continuity, more bloodline execution, and the still-unstarted Unity ECS runtime.

The correct posture is to keep using the browser lane to harden the canon into executable systems while the Unity lane is brought online in ECS form. The project should continue expanding outward from the systems already made real, not collapsing inward into a smaller substitute.
