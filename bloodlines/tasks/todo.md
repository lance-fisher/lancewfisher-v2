# Bloodlines Active Plan

## Completed 2026-04-15 (Sessions 46-47) - Civilizational Stability Feedback and Breakaway Territorial Foothold

- [x] Add positive civilizational feedback so strong food and water surplus reinforces owned-march loyalty on the canonical realm cycle when cap pressure is not active.
- [x] Normalize territory-loyalty delta handling through a shared helper so crisis erosion and prosperity reinforcement use one path.
- [x] Complete the unfinished Session 46 runtime-bridge coverage and return the test suite to green.
- [x] Extend minor-house defection from registry presence plus founder unit into real territorial control with a stabilized border march.
- [x] Add save/restore support for dynamic control points so spawned breakaway territory survives snapshot round-trip.
- [x] Surface rival minor-house march count in the dynasty panel.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_46.md` and `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_47.md`.

## Completed 2026-04-15 (Sessions 48-49) - Minor-House AI Activation and Save-State Counter Continuity

- [x] Add first-layer `minor_house` AI so breakaway factions defend their claimed march, retake it if seized, and regroup after pressure clears.
- [x] Surface minor-house retinue count and defense posture in the dynasty panel.
- [x] Extend runtime coverage for hostile-pressure response, regroup behavior, and save/restore persistence of minor-house AI state.
- [x] Repair restore-path counter continuity so live prefix-based ids for buildings and dynasty entities do not collide after snapshot restore.
- [x] Extend runtime coverage to prove a restored match can place a new building with a fresh id.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_48.md` and `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_49.md`.

## Completed 2026-04-15 (Session 50) - Breakaway-March Territorial Levy and Retinue Growth

- [x] Implement breakaway-march territorial levy and local retinue growth for `minor_house` factions.
- [x] Make the levy touch territory control, loyalty, food and influence pressure, military presence, and save/restore continuity.
- [x] Surface levy state through the dynasty panel instead of adding dead UI.
- [x] Extend runtime coverage for levy completion, loyalty burden, resource spend, snapshot persistence, and fresh post-restore retinue ids.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_50.md`.

## Completed 2026-04-15 (Session 51) - Mixed-Bloodline Instability Weighting

- [x] Implement mixed-bloodline defection weighting inside the lesser-house instability pipeline.
- [x] Make mixed bloodline matter to live systems, not sit in genealogy text: marriage state, hostility, lesser-house drift, and dynasty-panel legibility.
- [x] Extend runtime coverage for mixed-bloodline child generation, cadet promotion, hostility-sensitive drift, and snapshot persistence.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_51.md`.

## Completed 2026-04-15 (Session 52) - Faith-Compatibility Weighting In AI Marriage Logic

- [x] Add faith-compatibility weighting to AI marriage proposal and acceptance logic.
- [x] Make covenant and doctrine fit touch live systems, not lore labels: AI proposal willingness, AI acceptance willingness, legitimacy-repair marriage pressure, and dynasty-panel legibility.
- [x] Extend runtime coverage for harmonious-faith proposal and acceptance plus fractured-faith refusal in both branches.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_52.md`.

## Completed 2026-04-15 (Session 53) - Marriage Death and Dissolution Consequence Layer

- [x] Add marriage dissolution driven by real bloodline-member death state.
- [x] Make dissolution touch live systems, not flavor text: active marriage records, legitimacy, conviction, gestation, and dynasty-panel legibility.
- [x] Extend runtime coverage for death-triggered dissolution and snapshot restore persistence.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_53.md`.

## Completed 2026-04-15 (Session 54) - Water-Infrastructure Tier 1 and Field-Army Sustainment

- [x] Implement first-layer water infrastructure for campaigning armies through owned marches, settlements, wells, supply camps, and linked wagons.
- [x] Make dehydration materially change simulation through speed and attack penalties, logistics-band pressure, and AI regroup behavior.
- [x] Extend save / restore continuity so field-water condition survives snapshot round-trip.
- [x] Extend runtime coverage for dehydration escalation, movement slowdown, wagon-linked resupply, restore persistence, and AI assault delay.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_54.md`.

## Completed 2026-04-15 (Session 55) - Espionage and Assassination Covert Operations

- [x] Implement espionage as a live dynasty operation that produces rival-court intelligence reports.
- [x] Implement assassination as a live dynasty operation that kills real bloodline members and applies command, legitimacy, succession, and hostility consequence.
- [x] Surface intelligence reports and covert actions through the dynasty panel instead of adding dead UI.
- [x] Extend Stonehelm AI so it can run espionage and then escalate into assassination against the player.
- [x] Extend save / restore continuity so `dynastyIntel` ids rebuild correctly after snapshot restore.
- [x] Extend runtime coverage for successful espionage, report snapshot exposure, report restore continuity, successful assassination, failed assassination, and AI covert reciprocity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_55.md`.

## Completed 2026-04-15 (Session 56) - Faith Operations, Missionary Pressure, and Holy War Declaration

- [x] Implement missionary conversion pressure as a live faith operation fueled by real faith intensity.
- [x] Implement holy war declaration as a live faith operation with persistent ongoing pressure state.
- [x] Make the new faith lane touch hostility, territorial loyalty or legitimacy pressure, AI reactivity, and faith-panel legibility.
- [x] Extend save / restore continuity so active holy wars preserve collision-safe `faithHolyWar` ids after snapshot round-trip.
- [x] Extend runtime coverage for missionary success, missionary failure against a warded Order court, holy war persistence, holy war restore continuity, and enemy AI faith escalation.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_56.md`.

## Completed 2026-04-15 (Session 57) - Marriage Governance By Head Of Household

- [x] Route marriage proposal and acceptance through live household authority.
- [x] Require a diplomatic envoy on the offering side of a marriage agreement.
- [x] Apply legitimacy and stewardship strain when marriage governance runs under regency.
- [x] Surface marriage authority, envoy, sanction provenance, and approval provenance in the dynasty panel.
- [x] Extend runtime coverage for direct head approval, heir regency, envoy-loss blocking, snapshot legibility, and restore continuity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_57.md`.

## Completed 2026-04-15 (Session 58) - Field-Water Attrition And Desertion

- [x] Deepen field-water sustainment from slowdown into longer-run attrition and desertion pressure.
- [x] Make dehydration collapse touch real force loss, commander-buffered discipline, logistics legibility, AI regroup behavior, and restore continuity.
- [x] Extend runtime coverage for attrition onset, desertion collapse, commander buffering, restore continuity, and AI assault delay under breaking-line pressure.
- [x] Harden holy-war continuation pressure so active faith war sustains territorial or legitimacy consequence during the verification suite.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_58.md`.

## Completed 2026-04-15 (Session 59) - Counter-Intelligence And Bloodline-Targeting Defense

- [x] Deepen espionage and assassination into a live counter-intelligence court-watch state.
- [x] Make covert defense touch bloodline safety, legitimacy, court loyalty, fortification and ward backing, AI reciprocity, and restore continuity.
- [x] Surface active watch state and rival-court protection in the dynasty panel instead of hiding it in covert math.
- [x] Extend runtime coverage for watch activation, covert-chance suppression, guarded bloodline shielding, interception tracking, restore continuity, fresh post-restore watch ids, and AI defensive reciprocity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_59.md`.

## Completed 2026-04-15 (Session 60) - World Pressure And Late-Stage Escalation

- [x] Deepen broader world-pressure and late-stage escalation now that covert offense and covert defense are both live enough to support stronger rival-world response.
- [x] Make world pressure touch live territory, legitimacy, AI timing, tribal behavior, dashboard legibility, and snapshot continuity instead of staying a dashboard label.
- [x] Extend runtime coverage for cycle-driven pressure escalation, restore continuity, Stonehelm timer compression, and tribal retargeting toward the dominant kingdom.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_60.md`.

## Completed 2026-04-15 (Session 61) - Counter-Intelligence Dossiers And Retaliation

- [x] Deepen covert-defense follow-up so successful interceptions create actionable runtime consequence instead of ending as passive defense only.
- [x] Add source-scoped counter-intelligence dossier generation and hostile-source interception history.
- [x] Make Stonehelm reuse dossier state for faster retaliation without reopening redundant espionage.
- [x] Surface dossier label, intercepted operation type, and network-hit count through the dynasty panel.
- [x] Extend runtime coverage for dossier creation, restore continuity, source-scoped watch history, and AI retaliation from dossier state.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_61.md`.

## Completed 2026-04-15 (Session 62) - Lesser-House Marital Anchoring And Branch Pressure

- [x] Deepen marriage follow-up so cadet-house loyalty reads live marriage state through explicit branch anchors instead of generic mixed-bloodline drift alone.
- [x] Add active, dissolved, strained, and fractured marriage-anchor consequence for mixed-bloodline lesser houses.
- [x] Surface branch anchor identity, status, pressure, and branch-child support through the dynasty panel.
- [x] Extend runtime coverage for active anchor support, death-ended anchor loss, hostility-driven fracture, and restore continuity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_62.md`.

## Completed 2026-04-15 (Session 63) - Hartvale Playability And House-Gated Unique-Unit Access

- [x] Deepen the house layer through Hartvale playability and house-gated unique-unit enablement.
- [x] Make the house layer touch real runtime, not just `prototypePlayable` data flags.
- [x] Preserve Ironmark and Stonehelm identity by filtering barracks training through simulation-side house ownership.
- [x] Surface house-specific unit availability honestly through the existing command panel.
- [x] Extend runtime and data validation coverage so off-house factions cannot surface or queue Hartvale's unique unit.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_63.md`.

## Completed 2026-04-15 (Session 64) - World-Pressure Internal Dynastic Destabilization

- [x] Deepen broader world-pressure follow-up through internal cadet-house destabilization under overextension.
- [x] Make the new pressure layer touch lesser-house loyalty drift, defection pressure, dynasty-panel legibility, world-pill legibility, and restore continuity.
- [x] Persist branch-level world-pressure severity and daily drift on active lesser houses.
- [x] Extend runtime coverage so targeted world pressure worsens cadet-house loyalty drift and survives snapshot restore.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_64.md`.

## Completed 2026-04-15 (Session 65) - World-Pressure Splinter Opportunism

- [x] Deepen broader world-pressure follow-up through hostile minor-house opportunism against pressured parent realms.
- [x] Make the new pressure layer touch breakaway levy tempo, retinue-cap growth, territorial retake behavior, dynasty-panel legibility, world-pill legibility, and restore continuity.
- [x] Persist splinter parent-pressure severity, levy tempo, retake tempo, and cap bonus on live minor-house AI state.
- [x] Extend runtime coverage so targeted world pressure accelerates splinter levy growth, shortens retake cadence, and survives snapshot restore.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_65.md`.

## Completed 2026-04-15 (Session 66) - Dossier-Backed Sabotage Retaliation

- [x] Deepen post-dossier covert follow-up through dossier-backed sabotage retaliation and court-counterplay.
- [x] Make the new covert layer touch live dossier state, sabotage subtype selection, sabotage target selection, retaliatory AI behavior, dynasty-panel legibility, and restore continuity.
- [x] Persist dossier provenance, dossier support bonus, and retaliation reason on sabotage operations.
- [x] Extend runtime coverage so restored dossiers drive smarter sabotage retaliation without reopening redundant espionage.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_66.md`.

## Completed 2026-04-15 (Session 67) - Player-Facing Dossier Sabotage Actionability

- [x] Expose dossier-backed sabotage as a real player action in the rival-court panel using live sabotage terms and the shared dossier profile.
- [x] Make the new covert layer touch live dossier state, shared sabotage-term logic, player-side dynasty-panel actionability, real sabotage launch consequence, and restore continuity.
- [x] Keep dossier sabotage honest by disabling the action when real sabotage terms are unavailable instead of adding decorative UI.
- [x] Extend runtime coverage so player-side dossier-backed sabotage survives the existing verification suite and snapshot round-trip.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_67.md`.

## Completed 2026-04-15 (Session 68) - Convergence-Tier World-Pressure Escalation

- [x] Deepen world pressure at `Convergence` so dominant-realm pressure drives sharper rival-kingdom military, covert, and faith tempo plus harsher tribal raid cadence than the prior `Severe` layer.
- [x] Surface the new convergence-pressure escalation honestly through the existing world pill.
- [x] Extend runtime coverage so `Convergence` changes simulation behavior measurably and survives snapshot continuity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_68.md`.

## Completed 2026-04-13 (Bridge Slice)

- [x] Audit the canonical Bloodlines tree, runtime, data layer, and Unity continuation lane.
- [x] Cross-check the live implementation against canon, roadmap, and state-analysis documents.
- [x] Add battlefield commander presence tied to dynasty members.
- [x] Add doctrine-path commitment choices with live low-tier effects.
- [x] Add conviction ledger buckets with runtime triggers and derived band updates.
- [x] Add occupation-versus-stabilized territory state and surface it in UI/debug output.
- [x] Update validation and continuity documents to reflect the new live systems.
- [x] Verify with targeted runtime/data tests and syntax checks.

## Completed 2026-04-14 (Dynasty Consequence Cascade)

- [x] Refactor applyDamage into pure damage + deferred finalizeDeaths with killedBy capture.
- [x] Implement commander capture vs kill resolution using proximity to hostile combat units.
- [x] Implement governor loss on territory flip (captured vs displaced based on context).
- [x] Implement role succession cascade when head / heir / commander falls or is captured.
- [x] Add captive ledger, fallen ledger, heir pointer, and interregnum state.
- [x] Add captive ransom influence trickle and capture-aware conviction events.
- [x] Surface captives, fallen, succession state in dynasty panel and debug overlay (DOM-safe build).
- [x] Extend `tests/runtime-bridge.mjs` with capture + succession assertions.
- [x] Re-run data, bridge, and syntax tests; verify live runtime.

## Completed 2026-04-14 (Defensive Fortification Doctrine)

- [x] Author `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md` (full canonical doctrine).
- [x] Author `04_SYSTEMS/FORTIFICATION_SYSTEM.md` (defender-side specification).
- [x] Author `04_SYSTEMS/SIEGE_SYSTEM.md` (attacker-side specification).
- [x] Extend `01_CANON/CANONICAL_RULES.md` and `01_CANON/CANON_LOCK.md` with fortification lock entries.
- [x] Extend `04_SYSTEMS/SYSTEM_INDEX.md`, `TERRITORY_SYSTEM.md`, `DYNASTIC_SYSTEM.md` additively.
- [x] Extend `08_MECHANICS/MECHANICS_INDEX.md`, `11_MATCHFLOW/MATCH_STRUCTURE.md`, `10_UNITS/UNIT_INDEX.md` additively.
- [x] Extend `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` with 2026-04-14 integration addendum.
- [x] Extend `docs/DEFINITIVE_DECISIONS_REGISTER.md` with Decision 21.
- [x] Extend `docs/unity/PHASE_PLAN.md` with U17-U22 fortification and siege phases.
- [x] Extend `docs/unity/SYSTEM_MAP.md` with Fortification and Siege ECS systems.
- [x] Update bible export to v3.3 and place canonical desktop copy at `D:/Lance/Desktop/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md`.
- [x] Add CHANGE_LOG entry for Session 14 integration.
- [x] Update continuity docs to match new ground truth.

## Completed 2026-04-14 (Preservation-First Canonical Root Consolidation)

- [x] Inventory all confirmed Bloodlines-related roots, compatibility copies, preserved archives, and governance overlays across the workspace.
- [x] Verify `D:\\ProjectsHome\\Bloodlines` is the canonical entry path and document its physical backing path.
- [x] Import outside Bloodlines source roots into `archive_preserved_sources/` without deleting originals.
- [x] Import Bloodlines-specific governance overlays and parent-repo Bloodlines continuity surfaces into `governance/`.
- [x] Add root-level governance and continuity files: `AGENTS.md`, `MASTER_PROJECT_INDEX.md`, `MASTER_BLOODLINES_CONTEXT.md`, `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `SOURCE_PROVENANCE_MAP.md`, `CONSOLIDATION_REPORT.md`.
- [x] Add machine-readable state under `continuity/PROJECT_STATE.json`.
- [x] Update `README.md`, `CLAUDE.md`, and `.claude` startup surfaces to point future sessions at the canonical root.
- [x] Reconcile `deploy/bloodlines` directly into the active canonical working tree by importing deploy-only files and promoting the newer deploy `index.html`.

## Completed 2026-04-14 (Session 2) - Fortification + Siege + Realm Cycle Foundation (Browser Reference)

- [x] Implement fortification tier metadata on control points and settlements (6 canonical classes, fortification tier 0-ceiling).
- [x] Implement assault failure penalty simulation (assault cohesion strain threshold 6, 0.85x penalty for 20s).
- [x] Implement siege engine unit class (Battering Ram with structural 3.5x + anti-unit 0.4x multipliers).
- [x] Implement first explicit water / food / logistics pressure slice (90-second canonical realm cycle with famine + water crisis + cap pressure).
- [x] Add `getRealmConditionSnapshot` helper returning the 11-state legibility snapshot.
- [x] Enable stone and iron resources in prototype with map nodes and building dropoffs.
- [x] Implement canonical smelting chain (iron_mine consumes wood at 0.5 ratio; ore returns to node if fuel insufficient).
- [x] Add fortification building class (`wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1`) with structural damage multipliers.
- [x] Thread structural + anti-unit multipliers through applyDamage + melee + projectile attack paths.
- [x] Write canonical wave plan: `docs/plans/2026-04-14-fortification-siege-population-legibility-wave.md`.

## Completed 2026-04-14 (Session 2) - Unity Environment Alignment

- [x] Machine discovery (Unity Hub, 6.3 LTS + 6.4 editors, VS 18 Community, VS Code, Blender 5.1, Git 2.46, GitHub Desktop, .NET 10.0.201).
- [x] Identify canonical Unity project at `<repo>/unity/` (full DOTS/ECS stack confirmed in manifest).
- [x] Identify `<repo>/Bloodlines/` as fresh URP 3D template stub; preserve with `STUB_TEMPLATE_NOTICE.md`.
- [x] Normalize `unity/Assets/_Bloodlines/` to the full approved baseline.
- [x] Seed structural READMEs at `_Bloodlines/`, `Code/`, and `Data/`.
- [x] Rewrite `unity/README.md` to reflect canonical root governance plus approved toolchain plus direction-of-play non-negotiables.
- [x] Extend `BloodlinesDefinitions.cs` with fortification, siege, settlement, and realm-condition canon fields.
- [x] Extend `JsonContentImporter.cs` to import settlement-classes.json and realm-conditions.json and populate the new fields.
- [x] Write `ENVIRONMENT_REPORT_2026-04-14.md` at repo root with toolchain inventory, project discovery, package alignment, Unity version decision point, and next execution targets.
- [x] Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`, and `00_ADMIN/CHANGE_LOG.md` additively.

## Completed 2026-04-14 (Session 3) - Browser Renderer, HUD, AI, and Validation Alignment

- [x] Extend `src/game/core/renderer.js` with distinct draw cases for stone nodes, iron nodes, wall segments, watch towers, gatehouses, inner keeps, Rams, and settlement-class plus fortification-tier labels on control points.
- [x] Extend `src/game/main.js` with the expanded worker build palette, 10-pill resource bar support, and a 6-pill realm-condition HUD bar driven by `getRealmConditionSnapshot`.
- [x] Extend `play.html` with live realm-condition HUD markup and update the launch-card slice summary.
- [x] Extend `src/game/core/ai.js` so the enemy AI refuses direct keep assaults when the target primary seat is fortified and no siege-class unit is present.
- [x] Extend `tests/data-validation.mjs` with assertions for enabled primary resources, fortification roles, ram schema, settlement classes, and realm conditions.
- [x] Extend `tests/runtime-bridge.mjs` with fortification-tier advancement, smelting stall/success, ram-vs-wall damage, famine trigger, and AI keep-assault refusal coverage.
- [x] Re-run validation and syntax checks; verify the browser/spec lane remains green.

## Completed 2026-04-14 (Session 4) - Fortified Reserve Cycling and State Diagnosis

- [x] Implement fortified-settlement reserve cycling so wounded defenders fall back to the keep and healthier reserves muster forward.
- [x] Add triage healing inside fortified seats and primary-keep presence detection for commander and bloodline-seat state.
- [x] Extend `getRealmConditionSnapshot` and the browser HUD to surface ready reserves, recovering reserves, threat state, and keep-presence metadata.
- [x] Add reserve-duty renderer markers for mustering and fallback/recovering defenders.
- [x] Extend `tests/runtime-bridge.mjs` with reserve-cycling and fortification-snapshot coverage.
- [x] Write a serious repository diagnosis and gap map at `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`.
- [x] Re-run runtime validation and syntax checks; verify the browser/spec lane remains green.

## Completed 2026-04-14 (Session 5) - Governor Specialization and Faith-Warded Keeps

- [x] Implement governor specialization profiles (`border`, `city`, `keep`) and anchor-aware governor rotation between held marches and dynastic settlements.
- [x] Extend territory logic so governor specialization now affects trickle output, stabilization tempo, capture resistance, and loyalty protection.
- [x] Implement faith-integrated fortification ward profiles for Old Light, Blood Dominion, Order, and Wild.
- [x] Implement Blood Dominion sacrificial reserve surge behavior under active keep defense.
- [x] Extend fortified combat so keep wards and keep presence alter defensive sight, attack leverage, and hostile approach speed.
- [x] Extend `getRealmConditionSnapshot`, dynasty UI, and faith UI so keep-governor specialization and active ward state are legible.
- [x] Extend `tests/runtime-bridge.mjs` with governor rotation and Old Light ward sight assertions.
- [x] Write the Session 5 state-of-game addendum at `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`.
- [x] Re-run runtime validation and syntax checks; verify the browser/spec lane remains green.

## Completed 2026-04-14 (Session 6) - Siege Workshop, Additional Engines, and Attacker Preparation

- [x] Add `siege_workshop` as dedicated siege-production infrastructure and move active siege-engine production out of Barracks.
- [x] Add `siege_tower` and `trebuchet` to the live browser/spec roster with differentiated siege roles.
- [x] Extend combat so Siege Towers materially support nearby allied structural assaults.
- [x] Extend renderer legibility for Siege Workshop, Siege Towers, and Trebuchets.
- [x] Extend Stonehelm AI from refusal into siege preparation: quarry, iron mine, workshop, first engine queue, and staged siege-line commitment.
- [x] Extend `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` for workshop schema, new engines, support behavior, and AI preparation.
- [x] Re-run runtime validation and syntax checks; verify the browser/spec lane remains green.

## Completed 2026-04-14 (Session 7) - Dynastic Rescue, Ransom, and Captivity Operations

- [x] Add `dynasty.operations` as live state with active and history tracks.
- [x] Implement negotiated ransom operations with escrowed resource commitment and timed release resolution.
- [x] Implement covert rescue operations with deterministic success logic derived from bloodline roles, renown, fortification depth, and active keep wards.
- [x] Implement captor-side ransom demand execution for held captives.
- [x] Extend the dynasty panel so captured bloodline members expose live release and rescue actions and held captives expose ransom-demand actions.
- [x] Extend `tests/runtime-bridge.mjs` for negotiated ransom, covert rescue, and captor-demand recovery coverage.
- [x] Re-run runtime validation and syntax checks; verify the browser/spec lane remains green.

## Completed 2026-04-14 (Session 10) - Unity 6.3 LTS Lock, ECS Foundation, Sabotage Operations, Commander Keep-Presence Sortie

### Unity production lane (unblocked)

- [x] Resolve Unity version alignment decision in favor of Option B (Unity 6.3 LTS).
- [x] Update `unity/ProjectSettings/ProjectVersion.txt` from `6000.4.2f1` to `6000.3.13f1`.
- [x] Align `unity/Packages/manifest.json` to 6.3 LTS-compatible package versions (Entities 1.4.0, Collections 2.5.7, Entities.Graphics 1.4.0, URP 17.3.0, Input System 1.11.2, Addressables 2.5.0).
- [x] Author 15 canonical ECS component files under `unity/Assets/_Bloodlines/Code/Components/`.
- [x] Author 3 canonical ECS systems (Bootstrap, RealmConditionCycle, PopulationGrowth) under `unity/Assets/_Bloodlines/Code/Systems/`.
- [x] Update `unity/README.md` with locked toolchain, first-open workflow, and Session 10 ECS Foundation inventory.
- [x] Update `ENVIRONMENT_REPORT_2026-04-14.md` Section 4 with Resolution block locking Option B.

### Browser reference simulation — sabotage operations

- [x] Add `startSabotageOperation(state, factionId, subtype, targetFactionId, targetBuildingId)` public entry.
- [x] Add four canonical sub-types: `gate_opening`, `fire_raising`, `supply_poisoning`, `well_poisoning` with canonical cost, duration, validation, success formula, and conviction bookkeeping.
- [x] Add `tickBuildingStatusEffects(state, dt)` in `stepSimulation` for burn DOT and expired-flag cleanup.
- [x] Extend `tickSiegeSupportLogistics` so poisoned supply camps are excluded from the wagon-link filter.
- [x] Extend `tickDynastyOperations` to dispatch sabotage operations independently of the captive-member validation path.
- [x] Spymaster-gate sabotage via `getDynastyMemberByRole`.

### Browser reference simulation — commander keep-presence sortie

- [x] Add `issueKeepSortie(state, factionId, settlementId)` public entry with SORTIE_DURATION_SECONDS (12) and SORTIE_COOLDOWN_SECONDS (60).
- [x] Extend `getFriendlyFortificationCombatProfile` to apply ×1.22 attack and +22 sight to defenders while sortie is active.
- [x] Extend `getRealmConditionSnapshot` fortification block with `commanderAtKeep`, `sortieActive`, `sortieCooldownRemaining`, `sortieReady`.

### Tests

- [x] Extend `tests/runtime-bridge.mjs` with sabotage subtype validation, target validation, fire_raising escrow, sortie refusal path, snapshot exposure of new fortification fields.
- [x] Verify `node tests/data-validation.mjs`, `node tests/runtime-bridge.mjs`, `node --check` on all simulation modules pass.
- [x] Verify live browser boot on `http://localhost:8057/play.html`: 11 pressure pills render, zero console errors, zero failed network requests.

### Session 10 continuity

- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_10.md`.
- [x] Append Session 10 entry to `00_ADMIN/CHANGE_LOG.md`.
- [x] Update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`.

## Completed 2026-04-14 (Session 9) - Full-Realization Continuation, 11-State Dashboard, Ballista and Mantlet

- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md` (full-project state analysis preserving full protected scope).
- [x] Produce `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md` (system-by-system matrix under LIVE / PARTIAL / DATA-ONLY / DOCUMENTED / CANON-LOCKED / VOIDED / CANON-OPEN classification, with implementation-debt priority).
- [x] Produce `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md` (six-vector growth model, parallel lane strategy, non-negotiable session rules, exit criteria per vector).
- [x] Produce `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` (sessions 9 through 30 plus ordered sequencing).
- [x] Additively append Session 9 addendum to `MASTER_BLOODLINES_CONTEXT.md`.
- [x] Add `ballista` to `data/units.json` (siege-engine role, siegeClass ballista, ranged anti-personnel, structuralDamageMultiplier 1.6, antiUnitDamageMultiplier 1.2).
- [x] Add `mantlet` to `data/units.json` (siege-support role, siegeClass mantlet, no attack, coverRadius 92, coverInboundRangedMultiplier 0.55).
- [x] Wire `ballista` and `mantlet` into `siege_workshop.trainableUnits`.
- [x] Refactor `getFactionSiegeUnits` default filter to `role === "siege-engine"` so siege-support units are not counted as attacking engines.
- [x] Refactor `getFactionEngineerUnits` and `getFactionSupplyWagons` to filter directly by role.
- [x] Add `getFactionMantlets` helper.
- [x] Add `getIncomingRangedCoverMultiplier` helper and apply in `updateProjectiles` for unit-targeted projectiles.
- [x] Extend `getRealmConditionSnapshot` with `cycle`, `faith`, `conviction`, `logistics`, `worldPressure` blocks plus `band` fields on fortification and military (full canonical 11 pressure states).
- [x] Add distinct `ballista` silhouette in `src/game/core/renderer.js`.
- [x] Add distinct `mantlet` silhouette with cover aura indicator in `src/game/core/renderer.js`.
- [x] Extend Stonehelm AI siege-line production order to include mantlet and ballista between the bombard/engineer/wagon core and the ram/tower finishers.
- [x] Refine `hasSiegeUnit` in `ai.js` so mantlet alone does not satisfy keep-assault gating.
- [x] Rewrite `renderRealmConditionBar` in `src/game/main.js` to emit 11 pressure pills (Cycle, Pop, Food, Water, Loyalty, Fort, Army, Faith, Conviction, Logistics, World).
- [x] Reduce `.pressure-pill` min-width in `src/game/styles.css` from 132px to 116px to accommodate 11 pills.
- [x] Extend `tests/data-validation.mjs` with ballista and mantlet schema assertions.
- [x] Extend `tests/runtime-bridge.mjs` with mantlet cover reduction assertion, ballista workshop trainability assertion, and full 11-state snapshot assertions.
- [x] Verify `node tests/data-validation.mjs`, `node tests/runtime-bridge.mjs`, and `node --check` on all simulation modules pass.
- [x] Verify live browser boot on `http://localhost:8057/play.html`: 11 pressure pills render, zero console errors, zero failed network requests.
- [x] Update `00_ADMIN/CHANGE_LOG.md`, `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`, and `tasks/todo.md`.

## Completed 2026-04-14 (Session 8) - Engineer Corps, Siege Supply Continuity, and Sustained Assault

- [x] Add `supply_camp` as a forward logistics anchor in the live browser/spec roster.
- [x] Add `siege_engineer` and `supply_wagon` to the live workshop roster.
- [x] Implement engineer-assisted breach support and in-line siege-engine repair.
- [x] Implement siege supply continuity through linked camps and wagons with operational penalties when the chain is cut.
- [x] Extend Stonehelm AI with supply-camp buildout, engineer and wagon queueing, and delay on unsupplied formal assaults.
- [x] Extend HUD/debug legibility with live siege sustainment state.
- [x] Extend validation for engineer breach support, engineer repair, supply sustainment, AI support queueing, AI supply-camp buildout, and AI unsupplied-assault delay.
- [x] Re-run runtime validation and syntax checks; verify the browser/spec lane remains green.

## Next Session Priorities

### Unity production lane

- [ ] Open `<repo>/unity/` in Unity 6.3 LTS; run `Bloodlines -> Import -> Sync JSON Content`; commit generated ScriptableObject `.asset` files.
- [ ] Verify the authored Session 10 ECS foundation compiles cleanly on first open.
- [ ] Continue ECS foundation after first-open verification: movement, gather, smelting, production, construction, combat, territory, faith, and dynasty systems.
- [ ] Write Authoring + Baking for scene-to-entity conversion.
- [ ] Create `Scenes/Bootstrap/Bootstrap.unity` with subscene bootstrap and `Scenes/Gameplay/IronmarkFrontier.unity`.
- [ ] Establish battlefield camera, top-bloodline HUD, 11-state dashboard, and strategic-zoom theatre seam in Unity after the import layer is real.

## Completed 2026-04-15 (Session 69) - Ironmark Axeman Unique-Unit Lane

- [x] Advance Ironmark's dormant `axeman` lane into a real unique-unit path with honest simulation-side house gating.
- [x] Make the Ironmark unit lane touch blood-production pressure instead of existing as a pure stat bump.
- [x] Surface the new Ironmark unit honestly through the existing command surface and extend validation so non-Ironmark houses remain locked out.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_69.md`.

## Completed 2026-04-15 (Session 70) - Ironmark Axeman AI Awareness

- [x] Extend the live Ironmark `axeman` lane into AI awareness so Ironmark-controlled AI can recruit it through the same simulation-side house gate.
- [x] Make the Ironmark AI branch read blood-production pressure instead of blindly self-draining population growth.
- [x] Surface Ironmark AI unique-unit behavior through the existing message log.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_70.md`.

## Completed 2026-04-15 (Session 71) - World-Pressure Source Breakdown And Off-Home Tribal Targeting

- [x] Deepen world pressure through source-aware pressure breakdown and legibility.
- [x] Make continental or off-home overextension materially affect world reaction when it is the leading pressure source.
- [x] Preserve restore continuity for the new pressure-source layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_71.md`.

## Completed 2026-04-15 (Session 72) - Source-Aware Rival Response To Off-Home Overextension

- [x] Deepen world pressure through source-aware rival-kingdom response and territorial target selection.
- [x] Make rival kingdoms attack or contest the dominant pressure source, starting with off-home holdings when continental overextension is leading.
- [x] Preserve legibility and restore continuity for the new source-aware response layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_72.md`.

## Completed 2026-04-15 (Session 73) - Source-Aware Faith Backlash Under Holy-War-Led Pressure

- [x] Deepen world pressure through source-aware faith backlash and enemy faith-operation response.
- [x] Make enemy missionary and holy-war behavior react more sharply when active holy war is the dominant source of world pressure.
- [x] Preserve legibility and restore continuity for the new source-aware faith-response layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_73.md`.

## Completed 2026-04-15 (Session 74) - Source-Aware Covert Backlash Under Hostile-Operations Pressure

- [x] Deepen world pressure through source-aware covert backlash and enemy covert-response timing.
- [x] Make counter-intelligence and covert retaliation react more sharply when hostile operations are the dominant source of world pressure.
- [x] Preserve legibility and restore continuity for the new source-aware covert-response layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_74.md`.

## Completed 2026-04-15 (Session 75) - Source-Aware Dark-Extremes Backlash

- [x] Deepen world pressure through source-aware dark-extremes backlash and punitive rival-response timing.
- [x] Make rival reaction sharpen when dark extremes are the dominant source of world pressure.
- [x] Preserve legibility and restore continuity for the new source-aware dark-extremes-response layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_75.md`.

## Completed 2026-04-15 (Session 76) - Source-Aware Captive Backlash

- [x] Deepen world pressure through source-aware captive backlash and rival recovery timing.
- [x] Make rival reaction sharpen when held captives are the dominant source of world pressure.
- [x] Preserve legibility and restore continuity for the new source-aware captive-response layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_76.md`.

## Completed 2026-04-15 (Session 77) - Source-Aware Territory-Expansion Backlash

- [x] Deepen world pressure through source-aware territory-expansion backlash and stretched-march retaliation.
- [x] Make tribes and rival kingdoms react directly when broad territorial expansion is the dominant source of world pressure.
- [x] Preserve legibility and restore continuity for the new source-aware territory-expansion-response layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_77.md`.

## Completed 2026-04-15 (Session 78) - Hartvale Verdant Warden Local Support Layer

- [x] Deepen Hartvale's Verdant Warden into a real settlement-defense and local loyalty-support lane.
- [x] Make the already-live Hartvale unique unit matter to live settlement defense, local loyalty support, and existing runtime legibility.
- [x] Preserve legibility and restore continuity for the new Hartvale support layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_78.md`.

## Completed 2026-04-15 (Session 79) - Scout Rider Stage-2 Cavalry And Infrastructure Raiding

- [x] Advance `Scout Rider` into a live stage-2 cavalry and raiding lane instead of leaving it as dormant data.
- [x] Make the first stage-2 cavalry lane touch real logistics disruption, infrastructure harassment, territorial pressure, and AI behavior.
- [x] Preserve legibility and restore continuity for the new cavalry and raiding layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_79.md`.

## Completed 2026-04-15 (Session 80) - Scout Rider Worker And Resource-Seam Harassment

- [x] Extend `Scout Rider` from infrastructure raids into direct worker and resource-node harassment.
- [x] Add the first honest counter-raid response so local defenders and AI react to live cavalry harassment instead of only absorbing raid damage passively.
- [x] Preserve legibility and restore continuity for the next cavalry follow-up layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_80.md`.

## Completed 2026-04-15 (Session 82) - Convoy Escort Discipline And Post-Interdiction Reconsolidation

- [x] Add `escortAssignedWagonId` and `convoyReconsolidatedAt` to all unit creation, export, and restore paths.
- [x] Deepen `getFactionSiegeState` with `unscreenedRecoveringCount`, `convoyReconsolidated`, `escortedSupplyWagonCount`, and tighten `readyForFormalAssault` to require `convoyReconsolidated`.
- [x] Split assault-timing branch into unscreened (pull back) vs screened (hold stage point) paths for recovering convoys.
- [x] Write `escortAssignedWagonId` from AI patrol assignments and stamp `convoyReconsolidatedAt` when recovering wagons become screened.
- [x] Surface escort-discipline state in the logistics snapshot and dashboard pill.
- [x] Extend runtime-bridge tests for escort-binding persistence, reconsolidation flags, and restore continuity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_82.md`.

## Completed 2026-04-15 (Session 83) - First Live Match Progression And Stage-Aware Stonehelm Restraint

- [x] Convert the Session 82 match-structure, time-system, and multiplayer doctrine ingestion into the first live runtime match-progression layer.
- [x] Add readiness-gated five-stage match progression, phase state, next-stage shortfalls, and stage-change announcements in `src/game/core/simulation.js`.
- [x] Preserve `dualClock` and `matchProgression` through snapshot export and restore and surface both through `getRealmConditionSnapshot(...)`.
- [x] Wire stage and in-world-time legibility into the existing Cycle pill, realm-dashboard header, and debug overlay.
- [x] Gate Stonehelm territorial rivalry and Scout Rider tempo behind live match stages while still allowing already-live escalation systems to override restraint honestly.
- [x] Extend runtime coverage for Stage 1, Stage 3, and Stage 4 progression state, AI gating, and snapshot continuity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_83.md`.

## Completed 2026-04-15 (Session 84) - Imminent Engagement Warnings And Divine Right Windows

- [x] Implement the first live imminent-engagement warning layer from the multiplayer and time doctrine.
- [x] Deepen the five-stage match-progression spine with the first live Stage 5 Divine Right declaration window.
- [x] Wire both systems into live runtime behavior, UI legibility, AI awareness, and snapshot restore continuity.
- [x] Correct the declaration-resolution seam so expired Divine Right windows actually resolve.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_84.md`.

## Completed 2026-04-15 (Session 85) - First Live Political-Event Architecture Through Succession Crisis

- [x] Implement the first live political-event architecture on top of the progression and dual-clock spine through `Succession Crisis`.
- [x] Make the event touch real bloodline death, succession legitimacy, local loyalty, economy, stabilization, combat output, AI behavior, and snapshot restore continuity.
- [x] Surface live crisis severity, claim pressure, runtime penalties, and a real `Consolidate Succession` action through the dynasty panel instead of decorative labels.
- [x] Extend runtime coverage for trigger, penalty, AI exploitation, AI self-resolution, player consolidation, and restore continuity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_85.md`.

## Completed 2026-04-15 (Session 87) - Territorial Governance Sovereignty Hold And Victory Resolution

- [x] Deepen the first live Stage 5 `Territorial Governance Recognition` lane into the first honest sovereignty-resolution seam through multi-seat governor authority coverage, anti-revolt validation, stronger no-war enforcement, and final resolution logic.
- [x] Make the sovereignty lane touch real dynastic seat coverage, court loyalty, lesser-house stability, world-pressure escalation, AI backlash, UI legibility, and snapshot restore continuity.
- [x] Extend runtime coverage for multi-seat governance, sovereignty-hold escalation, Stonehelm emergency anti-sovereignty response, victory resolution, and restore continuity.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_87.md`.

## Completed 2026-04-15 (Session 86) - Covenant Test Runtime And Territorial Governance Recognition

- [x] Implement `Covenant Test` as the next live political-event follow-up on the now-live dynasty event spine.
- [x] Make the covenant lane touch real faith intensity growth, runtime penalties, direct rite actions, apex-covenant gating, late faith-unit access, AI behavior, UI legibility, and snapshot restore continuity.
- [x] Land the first live Stage 5 `Territorial Governance Recognition` layer with real sustain logic, world-pressure consequence, coalition backlash, dynasty-panel legibility, and restore continuity.
- [x] Extend runtime coverage for Covenant Test issuance or completion or gating plus Territorial Governance Recognition issuance or establishment or collapse.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_86.md`.

## Completed 2026-04-15 (Session 81) - Scout Rider Moving-Logistics Interception

- [x] Extend `Scout Rider` from fixed infrastructure and worked seams into direct interception of `supply_wagon` and other moving field-logistics carriers where canon supports it.
- [x] Make moving cavalry interception materially affect already-live siege-supply continuity, field-water sustainment, and local territorial pressure.
- [x] Add AI convoy targeting, local counter-screen response, and assault recoil so moving-logistics interception is not a one-sided cavalry trick.
- [x] Preserve legibility and restore continuity for the convoy-interdiction layer and extend runtime coverage accordingly.
- [x] Produce `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_81.md`.

### Browser reference simulation (frozen behavioral spec, realize remaining canon in Unity)

- [x] Record that the browser runtime is now frozen as behavioral specification only.
- [x] Treat the remaining open items in this block as canonical gaps to realize in `unity/`, not as permission to add new browser systems.

- [x] Ingest the canonical match-structure, time-system, and multiplayer engagement doctrine material into `02_SESSION_INGESTIONS/` and cross-reference against existing design surfaces.
- [x] Deepen the new five-stage match-progression runtime spine with stage-specific declaration or event pressure.
- [x] Implement the first live imminent-engagement warning layer from the multiplayer and time doctrine.
- [x] Implement the first live political-event architecture on top of the progression and dual-clock spine, starting with `Succession Crisis`.
- [x] Implement `Covenant Test` as the next live political-event follow-up on the now-live dynasty event spine.
- [ ] Deepen Stage 5 sovereignty follow-up beyond the now-live Territorial Governance victory seam into alliance-threshold pressure, population-acceptance buildup, and stronger coalition counterplay.
- [ ] Continue future house-identity follow-up only when additional unique-unit lanes are canon-settled instead of reviving voided house profiles.
- [ ] Consider player-side escort commands for the cavalry lane if AI-only escort discipline is insufficient.
- [ ] Continue deeper multi-kingdom world pressure or naval depth as lagging six-vector areas.

## Status

- Dynasty Consequence Cascade: live, tests green, live runtime verified.
- Fortification + Siege Foundation (browser): live in simulation, renderer, HUD, AI, and tests.
- Fortified reserve cycling (browser): live in simulation, HUD legibility, renderer state markers, and runtime validation.
- Governor specialization and faith-warded keep defense (browser): live in simulation, HUD/state readouts, and runtime validation.
- Siege Workshop, Siege Tower, Trebuchet, and Stonehelm siege-line preparation (browser): live in simulation, renderer, AI, and runtime validation.
- Dynastic rescue, ransom, and captor-demand captivity operations (browser): live in simulation, dynasty UI, and runtime validation.
- Engineer corps, supply camps, supply wagons, and supply-aware siege sustainment (browser): live in simulation, UI state, AI, and runtime validation.
- Canonical 90-Second Realm Cycle (browser): live with famine, water crisis, cap pressure, and HUD surfacing.
- Unity canonical project structure: aligned to approved baseline; ScriptableObject layer extended for all new canon; awaits Unity version alignment decision before ECS work.
- `Bloodlines/` stub: preserved with notice.
- Bible: v3.4 exported, canonical desktop copy placed.
- Civilizational stability feedback: live as of Session 46.
- Minor-house territorial foothold: live as of Session 47.
- Minor-house operational AI: live as of Session 48.
- Save-state counter continuity: partial as of Session 49.
- Breakaway-march territorial levy growth: live as of Session 50.
- Mixed-bloodline instability weighting: live as of Session 51.
- Faith-aware AI marriage logic: live as of Session 52.
- Marriage death and dissolution consequence: live as of Session 53.
- Field-water sustainment and hydration penalties: live as of Session 54.
- Field-water attrition and desertion consequence layer: live as of Session 58.
- Espionage and assassination covert pressure: live as of Session 55.
- Counter-intelligence and bloodline-targeting defense: live as of Session 59.
- World pressure and late-stage escalation: live as of Session 60.
- Counter-intelligence interception-network consequence and retaliation: live as of Session 61.
- Lesser-house marital anchoring and controlled mixed-bloodline branch pressure: live as of Session 62.
- Hartvale playability and house-gated unique-unit access: live as of Session 63.
- Hartvale Verdant Warden settlement-defense and local loyalty support: live as of Session 78.
- Ironmark axeman unique-unit lane: live as of Session 69.
- Ironmark axeman AI awareness: live as of Session 70.
- World-pressure internal dynastic destabilization: live as of Session 64.
- World-pressure splinter opportunism against hostile parent realms: live as of Session 65.
- Dossier-backed sabotage retaliation and covert court-counterplay: live as of Session 66.
- Player-facing dossier-backed sabotage actionability: live as of Session 67.
- Convergence-tier world-pressure rival tempo and tribal escalation: live as of Session 68.
- Missionary pressure and holy war declaration: live as of Session 56.
- Marriage governance by head of household: live as of Session 57.
- Scout Rider moving-logistics interception and convoy sustainment disruption: live as of Session 81.
- Convoy escort discipline and post-interdiction reconsolidation: live as of Session 82.
- First live match progression, dual-clock legibility, and stage-aware Stonehelm restraint: live or partial as of Session 83.
- Imminent-engagement warnings and Stage 5 Divine Right declaration windows: live as of Session 84.
- First live political-event architecture through Succession Crisis: live or partial as of Session 85.
- Covenant Test faith-event lane, apex-covenant gating, and Stonehelm covenant ascent: live as of Session 86.
- Territorial Governance sovereignty seam with multi-seat governor coverage, anti-revolt gating, coalition backlash, and victory resolution: partial as of Session 87.
- Continuity docs: refreshed through 2026-04-15 session 87.
- Environment: Unity Hub + 6.3 LTS + 6.4 editors + VS 18 + Blender 5.1 + Git + GitHub Desktop + VS Code + .NET 10 present and verified.
- No known red state.

## In Progress 2026-04-16 - Bloodlines Continuation Platform Thin Vertical Slice

- [x] Write the ecosystem audit and architecture decisions for the offline continuation platform inside the canonical Bloodlines root.
- [x] Create a contained platform workspace at `continuation-platform/` so the app stays inside the Bloodlines single-root governance model.
- [x] Capture actual Ollama inventory, runtime choice, WSL/Windows boundary, retrieval decision, orchestration decision, and bot integration decisions from the live machine state.
- [x] Seed the platform with a minimal canonical source subset: the current design bible, Bloodlines governance, Bloodlines continuity, and a short authoritative source registry.
- [x] Implement a local continuity store with journaled scan state, file hashes, source classification, last-good-state candidates, and append-only operations journal output.
- [x] Implement read-only-by-default posture with session-memory-only unlock checks tied to the Boss tier phrases and refusal telemetry for insufficient tier.
- [x] Implement one working agent mode, `resume_last_good_state`, backed by the actual local model inventory and grounded context assembly.
- [x] Implement one working GUI slice with a Dashboard and Agent Workspace surfaced from the local server.
- [x] Add Windows-first launch path, scan controls, rescan/change detection, model routing visibility, provenance visibility, and degraded-mode surfacing for the thin slice.
- [x] Validate the slice end to end, then update `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, and `continuity/PROJECT_STATE.json` with the continuation-platform state.
- [x] Expand the canonical registry from the thin subset into broader Bloodlines authority scoring and conflict detection across the discovered corpus.

## Completed 2026-04-16 - Bloodlines Continuation Platform Product-Ready Pass

- [x] Add a first-class diff and change-awareness surface that highlights the most meaningful recent file movements, likely authoritative updates, and frontier-session crossover.
- [x] Expand telemetry from summary counters into operator-usable drilldowns with provenance distribution, routing counts, degraded-mode history, and write-gate outcomes.
- [x] Replace the handoff-preview-only surface with a true handoff builder that packages continuity delta, open work, doctrine, canonical sources, and frontier re-entry briefing.
- [x] Polish the continuation-platform operator flow so the dashboard, agent workspace, tasks, timeline, memory, diff, telemetry, config, and handoff surfaces feel cohesive and ready for daily use.
- [x] Re-validate the product-ready platform end to end and sync the resulting state back into the Bloodlines continuity documents.

## Completed 2026-04-16 - Bloodlines Continuation Platform Quality-Of-Life Pass

- [x] Add operator-speed improvements for daily use, including persistent view state, lighter action feedback, and lower-friction copy or jump controls.
- [x] Add in-app filtering or focus tools so large memory, diff, telemetry, and task surfaces are easier to scan without leaving the governed local UI.
- [x] Re-validate the QoL pass end to end and sync the resulting state back into the Bloodlines continuity documents.

## Completed 2026-04-16 - Canonical Full-Realization Direction Refresh

- [x] Record Lance Fisher's new non-negotiable full-canon Unity direction in Bloodlines governance and continuity files.
- [x] Update the continuation platform doctrine so future local continuation sessions inherit the same no-MVP, no-scope-cut, Unity-only production direction.
- [x] Sync the refreshed direction into the next-session handoff and machine-readable continuity state.
