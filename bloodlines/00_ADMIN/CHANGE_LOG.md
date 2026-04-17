# Bloodlines: Documentation Change Log

**Purpose:** Records all changes made to the Bloodlines documentation system, including file reviews, file creations, file updates, and consolidation decisions.

---

## 2026-04-16 (Session 88): Acceptance Clock Fix, Doctrine Preservation, Unity First-Open and JSON Sync

Author: Claude.

### Unity Milestone

- **Unity 6.3 LTS first-open completed.** The Bloodlines Unity project (`unity/`) was opened in Unity 6.3 LTS (6000.3.13f1) via batch mode. All 15 ECS components and 3 ECS systems compiled with zero errors.
- **JSON content sync completed.** `Bloodlines.EditorTools.JsonContentImporter.ImportAll()` executed successfully, generating **119 ScriptableObject assets** across 12 data categories: 41 units, 23 buildings, 9 factions, 9 bloodline roles, 7 bloodline paths, 7 resources, 6 settlement classes, 6 victory conditions, 5 conviction states, 4 faiths, 1 map definition, 1 realm condition config.
- The Unity production lane blocker ("Lance must open Unity and run JSON sync") is now **cleared**. ECS system-level implementation can proceed.

### Previous Session 88 Work

## 2026-04-16 (Session 88, earlier pass): Acceptance Clock Fix, Full Doctrine Preservation, Environment Verification

Author: Claude.

- `tests/runtime-bridge.mjs` - fixed the population-acceptance clock test that was left failing from the prior session's interrupted work. Root cause: three compounding issues. (1) Dynasty operations without `resolveAt` were immediately resolved by `tickDynastyOperations`, dropping world-pressure score below the Stage 5 threshold. (2) Stage regression caused `triggerReady` to fail, breaking governance recognition mid-step. (3) Player military units died from dehydration when parked far from settlements, dropping `playerMilitaryCount` below the Stage 3 threshold. Fix: persistent operations with far-future `resolveAt`, durable enemy pressure sources (`darkExtremesActive` plus `darkExtremesStreak`), dehydration-safe unit positioning near the player command hall, and clearing enemy governance to prevent AI timer clobbering via `Math.max`.
- `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine_FULL_VERBATIM.md` - new file preserving the complete unabridged match-structure doctrine including all imminent-engagement sub-sections that were condensed in the original _RAW.md version.
- `02_SESSION_INGESTIONS/SESSION_INGESTION_INDEX.md` - added entry for the full verbatim doctrine file.
- Full environment discovery and verification completed: Unity 6.3 LTS, DOTS packages, Blender 5.1, Git, Node v24.13.0 all confirmed present. Unity first-open still Lance-gated.

Session 88 unblocked the test suite from the prior session's interrupted acceptance-clock work, preserved the complete match-structure doctrine verbatim, and verified the development environment state.

---

## 2026-04-15 (Session 87): Territorial Governance Sovereignty Hold And Victory Resolution

Author: Codex.

- `src/game/core/simulation.js` - added multi-seat governor authority assignment across keeps or cities or frontier control points, deeper Territorial Governance profile evaluation for seat coverage or court loyalty or lesser-house anti-revolt pressure or incoming holy war, escalating governance world-pressure contribution, the final sovereignty-hold countdown, real Territorial Governance victory resolution, and restore-safe victory metadata across the live win paths.
- `src/game/core/ai.js` - Stonehelm now treats imminent Territorial Governance victory as an emergency state and compresses attack or territorial or raid or assassination or missionary or holy-war timing more sharply during the final sovereignty hold.
- `src/game/main.js` - the cycle header, dynasty panel, and debug overlay now distinguish governance sustain or held or victory-countdown or completed states and surface multi-seat governance depth plus court or revolt shortfalls.
- `tests/runtime-bridge.mjs` - updated governor-specialization expectations for multi-seat governance and added coverage for seat coverage, incoming-holy-war blocking, governance world-pressure escalation, emergency Stonehelm response, Territorial Governance victory resolution, and restore continuity for active and completed sovereignty state.
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_87.md` - new state-of-game report for the Session 87 sovereignty-resolution layer.

Session 87 moved Territorial Governance beyond recognition-only status and into the first honest browser-runtime sovereignty victory seam without falsely claiming the full alliance-threshold or population-acceptance doctrine is complete.

---

## 2026-04-15 (Session 86): Covenant Test Runtime And Territorial Governance Recognition

Author: Codex.

- `src/game/core/simulation.js` - added full `Covenant Test` mandate issuance across the four covenants and both doctrine paths, covenant-structure intensity growth, success and failure consequence, apex-covenant and late-faith-unit gating, the first live `Territorial Governance Recognition` state, world-pressure contribution for that governance lane, and restore continuity for both systems.
- `src/game/core/ai.js` - Stonehelm now climbs the covenant structure ladder, performs live Blood Dominion rite actions, reacts to player Covenant Tests through sharper faith and war tempo, and answers Territorial Governance Recognition with directed coalition pressure against the weakest governed frontier.
- `src/game/main.js` - the dynasty panel, cycle header, debug overlay, and world-pressure detail lane now surface Territorial Governance Recognition state while preserving the already-live Covenant Test and Divine Right faith legibility.
- `src/game/styles.css` - preserved the earlier shell boot fix with the enforced `[hidden] { display: none !important; }` rule, which remained verified during the Session 86 browser pass.
- `tests/runtime-bridge.mjs` - added coverage for Covenant Test issuance or completion or gating or restore continuity and for Territorial Governance Recognition issuance or AI backlash or restore continuity or post-recognition collapse.
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_86.md` - new state-of-game report for the Session 86 runtime realization layer.

Session 86 moved `Covenant Test` into live runtime and carried the first Stage 5 territorial-governance recognition layer out of doctrine-only status without falsely claiming the full victory seam is complete.

---

## 2026-04-15 (Session 85): First Live Political-Event Architecture Through Succession Crisis

Author: Codex.

- `src/game/core/simulation.js` - added dynasty political-event state with active and history tracks, the first live `Succession Crisis` event, claim-strength and severity logic, escalation timing, simulation penalties to loyalty, legitimacy, resource yield, stabilization, population growth, and combat output, plus consolidation resolution and restore continuity.
- `src/game/core/ai.js` - Stonehelm now reads player succession instability as actionable pressure, compresses military, territorial, and marriage tempo against a weakened court, and also consolidates its own succession crisis through the shared resolution seam.
- `src/game/main.js` - the dynasty panel now surfaces active succession-crisis severity, trigger reason, claimant count, escalation timing, runtime modifiers, hostile-court crisis state, and a real `Consolidate Succession` action.
- `tests/runtime-bridge.mjs` - added coverage for player crisis trigger and penalties, consolidation recovery, AI aggression response, enemy self-resolution, and restore continuity.
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_85.md` - new state-of-game report for the Session 85 runtime realization layer.

Session 85 moved the first political-event architecture out of doctrine-only status and into the live browser simulation through a real dynastic collapse and recovery lane.

---

## 2026-04-15 (Session 84): Imminent Engagement Warnings And Stage 5 Divine Right Windows

Author: Codex.

- `src/game/core/simulation.js` - added the first live imminent-engagement warning and response lane around threatened dynastic keeps, the first Stage 5 Divine Right declaration window with recognition-share and apex-structure gating, countdown failure and completion logic, restore continuity, and a fix for expired declaration windows failing to resolve.
- `src/game/core/ai.js` - enemy defenders now read imminent-engagement warnings, and rival AI now reacts to live Divine Right declarations through compressed military and faith tempo. Enemy AI can also open its own Divine Right declaration window when eligible.
- `src/game/main.js` - the faith panel now surfaces Divine Right readiness, active and incoming declaration state, last outcome, and a real declaration action; the cycle header and debug overlay now surface active or incoming Divine Right pressure.
- `tests/runtime-bridge.mjs` - added Stage 5 Divine Right coverage for start, failure, AI response, restore continuity, and victory resolution, alongside the already-live imminent-engagement coverage.
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_84.md` - new state-of-game report for the Session 84 runtime realization layer.

Session 84 moved the first Divine Right victory seam into live browser runtime, made imminent keep-threat timing actionable, and corrected the declaration-resolution seam so Final Convergence windows actually complete.

---

## 2026-04-15 (Session 83): First Live Match Progression, Dual-Clock Legibility, And Stage-Aware Stonehelm Restraint

Author: Codex.

- `src/game/core/simulation.js` - added readiness-gated five-stage match progression, phase state, next-stage shortfalls, stage-change announcements, dual-clock plus match-progression snapshot export and restore, and realm-snapshot exposure of the new progression fields.
- `src/game/core/ai.js` - Stonehelm now reads live match progression for early-war territorial and cavalry restraint, while already-live escalation systems can still override that restraint when world pressure, dark extremes, convergence, holy war, or hostile-operations backlash are active.
- `src/game/main.js` - the existing Cycle pill, dashboard header, and debug overlay now surface stage, phase, in-world year, declaration count, and next-stage shortfall.
- `tests/runtime-bridge.mjs` - added Stage 1, Stage 3, and Stage 4 progression coverage, AI gating coverage, match-progression restore coverage, and updated earlier stripped-state cavalry tests to satisfy the new stage doctrine honestly.
- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-15_SESSION_83.md` - new state-of-game report for the Session 83 runtime realization layer.

Session 82 doctrine ingestion is now reflected as completed continuity, and Session 83 moves the first match-structure and dual-clock layer from canon-only into live browser runtime.

---

## 2026-04-15 (Session 82, Part 2): Canonical Match Structure, Time System, and Multiplayer Engagement Doctrine Ingestion

Author: Claude.

- `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine.md` - ingestion summary with conflict analysis.
- `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine_RAW.md` - full verbatim canonical text preserved.
- `11_MATCHFLOW/MATCH_STRUCTURE.md` - additive Session 82 section locking five-stage structure, declared dual-clock model, live strategic layer, and multiplayer engagement doctrine.
- `01_CANON/CANON_LOCK.md` - declaration table reference ranges moved from PROPOSED to LOCKED; multiplayer engagement doctrine section added with 8 new lock entries.
- `04_SYSTEMS/SYSTEM_INDEX.md` - added Match Structure, Dual-Clock Time, and Multiplayer Engagement entries to core systems table.
- `02_SESSION_INGESTIONS/SESSION_INGESTION_INDEX.md` - new ingestion entry.

Two conflicts flagged: (1) four-stage vs five-stage naming (resolved: five-stage is canonical, four-stage preserved historically); (2) faith commitment timing at end of S1 vs in S2 (resolved: S2 is canonical per new material).

---

## 2026-04-15 (Session 82): Convoy Escort Discipline And Post-Interdiction Reconsolidation (Vector 3 + Vector 6)

Author: Claude.

- `src/game/core/simulation.js` - added `escortAssignedWagonId` and `convoyReconsolidatedAt` to all unit creation paths; `getFactionSiegeState` now computes `unscreenedRecoveringCount`, `convoyReconsolidated`, and `escortedSupplyWagonCount`; `readyForFormalAssault` now requires `convoyReconsolidated`; snapshot export and restore preserve the new escort-binding fields.
- `src/game/core/ai.js` - supply-patrol assignments now write `escortAssignedWagonId` on escort units; stale bindings cleaned each patrol cycle; recovering wagons stamped with `convoyReconsolidatedAt` when screened; assault-timing branch differentiates unscreened vs screened recovering convoys.
- `src/game/main.js` - logistics pill metadata now surfaces escorted wagon count, unscreened recovering count, and reconsolidation status.
- `tests/runtime-bridge.mjs` - added assertions for escort-binding state, reconsolidation snapshot fields, escort-assignment persistence through restore, and reconsolidation-flag correctness.

Vectors 3 and 6 deepened. Tests green and browser verification confirmed.

---

## 2026-04-15 (Session 81): Scout Rider Moving-Logistics Interception (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `data/units.json` - `supply_wagon` now advertises live moving-logistics interception metadata, including convoy-carrier identity, interdiction timing, and real raid-loss payloads.
- `src/game/core/simulation.js` - added Scout Rider interception against moving `supply_wagon`, convoy-interdiction state, forced convoy retreat, supply-store stripping, local loyalty shock, sustainment lapse in both siege and field-water lanes, and restore continuity for active convoy cuts.
- `src/game/core/ai.js` - Stonehelm Scout Riders now prioritize hostile convoys, local defenders now answer struck wagons through the same pressure-site logic used for other live harassment, and the assault line now recoils to screen a hit convoy before resuming breach pressure.
- `src/game/main.js` - right-click raid ordering now supports hostile moving logistics carriers, and the logistics pill now surfaces convoy cuts directly.
- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` - added schema and runtime coverage for convoy interception, supply and water lapse, AI convoy targeting, local counter-screen response, and restore continuity.

1 item moved to LIVE (Scout Rider moving-logistics interception, first layer). Vectors 1, 3, and 6 advanced. Tests green and browser verification confirmed live shell boot with zero runtime errors and zero failed requests.

---

## 2026-04-15 (Session 80): Scout Rider Worker And Seam Harassment With Counter-Raid Response (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `data/units.json` - `scout_rider` now carries live seam-harassment timing and worker-retreat metadata.
- `src/game/core/simulation.js` - added live resource-node harassment state, worker routing to refuge under cavalry pressure, restored post-harass gathering recovery, dedicated `raid_retreat` withdrawal behavior, and snapshot continuity for harried seams.
- `src/game/core/ai.js` - Stonehelm Scout Riders now target worked hostile seams, Stonehelm local defenders now react to live harried seams, and the message log now surfaces local counter-raid response.
- `src/game/main.js` - right-click raid ordering now supports hostile resource seams and workers, and the logistics pill now exposes harried seams plus routed workers.
- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` - added schema and runtime coverage for seam harassment, worker retreat and recovery, AI seam targeting, local counter-raid response, and restore continuity.

1 item moved to LIVE (Scout Rider worker and resource-node harassment plus first counter-raid response, first layer). Vectors 1, 3, and 6 advanced. Tests green and browser verification confirmed post-click shell transition with zero runtime errors and zero failed requests.

---

## 2026-04-15 (Session 79): Scout Rider Stage-2 Cavalry And Infrastructure Raiding (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `data/units.json` and `data/buildings.json` - `scout_rider` is now prototype-enabled as the first live stage-2 cavalry unit, `stable` is now a real production building, and soft civilian-logistics structures now advertise live raidable metadata.
- `src/game/core/simulation.js` - added the real Scout Rider raid command, infrastructure disable windows, resource stripping, loyalty shock, logistics sabotage integration, siege-anchor and water-support disruption, and restore continuity for raid state plus cooldown state.
- `src/game/core/ai.js` - Stonehelm now builds Stables, trains Scout Riders, selects soft hostile logistics targets, and launches live raid orders instead of leaving the cavalry lane dormant.
- `src/game/main.js` and `src/game/core/renderer.js` - the command surface now supports right-click raid orders, selection and dashboard legibility for raid pressure and cooldowns, and live raid-disable rendering on the battlefield.
- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` - added schema and runtime coverage for Scout Rider production, direct raid resolution, AI raid issuance, and restore continuity.
- `play.html` - added an inline empty favicon so browser verification no longer produces a failed `favicon.ico` request during shell boot.

1 item moved to LIVE (Scout Rider stage-2 cavalry and infrastructure raiding, first layer). Vectors 1, 3, and 6 advanced. Tests green and browser verification reached the live shell with zero failed requests.

---

## 2026-04-15 (Session 78): Hartvale Verdant Warden Local Support Layer (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added a shared Verdant Warden zone-support seam and wired it into settlement-defense attack leverage, reserve healing, reserve muster, loyalty protection, loyalty growth, and march stabilization.
- `src/game/core/ai.js` - Stonehelm now reads live Hartvale Warden keep-support strength and delays keep assault until escort mass is heavier when the player has Warden-backed defense.
- `src/game/main.js` - the Loyalty and Fort dashboard pills now expose live Verdant Warden coverage, protection, recovery, and keep-support bonuses.
- `tests/runtime-bridge.mjs` - added runtime coverage for Warden-backed reserve recovery, loyalty support, snapshot exposure, and restore continuity.

1 item moved to LIVE (Hartvale Verdant Warden settlement-defense and loyalty support, first layer). Vectors 1, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 77): Source-Aware Territory-Expansion Backlash (Vector 3 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - tribes and Stonehelm now read `territoryExpansion` as a distinct world-pressure source and redirect onto stretched weak marches instead of reacting through generic territorial pressure alone.
- `src/game/main.js` - the world pill now surfaces explicit territorial-breadth contribution from the live world-pressure source breakdown.
- `tests/runtime-bridge.mjs` - added runtime coverage for territory-expansion source identification, tribal and rival weakest-march backlash, message-log legibility, world-pill snapshot exposure, and restore continuity.

1 item moved to LIVE (source-aware territory-expansion backlash under world pressure, first layer). Vectors 3, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 76): Source-Aware Captive Backlash (Vector 2 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - enemy captive-recovery timing now reacts more sharply when held captives are the dominant world-pressure source, and Stonehelm now launches live rescue or ransom backlash through the dynasty-operations lane.
- `tests/runtime-bridge.mjs` - added runtime coverage for captive-source identification, sharper captive-recovery timing, rescue backlash, ransom backlash, message-log legibility, and restore continuity.

1 item moved to LIVE (source-aware captive backlash under world pressure, first layer). Vectors 2, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 75): Source-Aware Dark-Extremes Backlash (Vector 2 + Vector 3 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - enemy territorial and assassination timing now react more sharply when dark extremes are the dominant world-pressure source, and punitive backlash now redirects onto the weakest player-held marches instead of only nearest hostile ground.
- `tests/runtime-bridge.mjs` - added runtime coverage for dark-extremes source identification, punitive weak-march targeting, sharper territorial and assassination timing, message-log legibility, and restore continuity.

1 item moved to LIVE (source-aware dark-extremes backlash under world pressure, first layer). Vectors 2, 3, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 74): Source-Aware Covert Backlash Under Hostile-Operations Pressure (Vector 2 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - enemy counter-intelligence and sabotage timers now react more sharply when hostile operations are the dominant world-pressure source, and both retaliation branches now surface source-aware covert backlash through the message log.
- `tests/runtime-bridge.mjs` - added runtime coverage for hostile-operations source identification, sharper covert-timer compression, counter-intelligence launch, retaliatory sabotage, message-log legibility, and restore continuity.

1 item moved to LIVE (source-aware covert backlash to hostile-operations-led world pressure, first layer). Vectors 2, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 73): Source-Aware Faith Backlash Under Holy-War-Led Pressure (Vector 4 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - enemy missionary and holy-war timers now react more sharply when active holy war is the dominant world-pressure source, and both retaliation branches now surface source-aware faith backlash through the message log.
- `tests/runtime-bridge.mjs` - added runtime coverage for holy-war source identification, sharper faith-timer compression, missionary backlash, counter-holy-war declaration, message-log legibility, and restore continuity.

1 item moved to LIVE (source-aware faith backlash to holy-war-led world pressure, first layer). Vectors 4, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 72): Source-Aware Rival Response To Off-Home Overextension (Vector 3 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - deepened enemy territorial target selection so Stonehelm now reads the live world-pressure source breakdown and redirects onto off-home player marches when continental overextension is the dominant cause.
- `src/game/core/ai.js` - added message-log legibility for source-aware rival territorial response.
- `tests/runtime-bridge.mjs` - added runtime coverage for off-home rival redirect, live movement orders, message-log legibility, and restore-safe reapplication after snapshot round-trip.

1 item moved to LIVE (source-aware rival territorial response to off-home overextension, first layer). Vectors 3, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 71): World-Pressure Source Breakdown And Off-Home Tribal Targeting (Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added a shared `getWorldPressureSourceBreakdown(...)` seam, moved world-pressure score composition onto that seam, and exposed top-source plus source-breakdown state through the realm snapshot.
- `src/game/core/ai.js` - tribes now read the live world-pressure source breakdown and hard-prioritize off-home marches when continental overextension is the leading pressure source.
- `src/game/main.js` - the world pill now surfaces the dominant pressure source through the existing metadata line.
- `tests/runtime-bridge.mjs` - added runtime coverage for off-home source weighting, world-snapshot exposure, off-home tribal targeting, message-log legibility, and restore continuity.

1 item moved to LIVE (source-aware tribal reaction to world-pressure composition, first layer). Vectors 5 and 6 advanced. Tests green.

---

## 2026-04-15 (Session 70): Ironmark Axeman AI Awareness (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - imported the shared barracks trainable-roster seam, added house-aware barracks selection, and made Ironmark-controlled AI recruit `axeman` only when live blood-production pressure is still tolerable.
- `src/game/core/ai.js` - added message-log legibility for both blood-fueled Axeman muster and burden-driven fallback into Swordsmen.
- `tests/runtime-bridge.mjs` - added runtime coverage for Ironmark AI Axeman recruitment, blood-load-aware fallback, off-house lockout, message-log legibility, and restore continuity.

1 item moved to LIVE (Ironmark `axeman` AI awareness, first layer). Vectors 1, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 69): Ironmark Axeman Unique-Unit Lane (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `data/units.json` - enabled `axeman` as a live prototype unit, preserved Ironmark house ownership, and added elevated `ironmarkBloodPrice` plus `bloodProductionLoadDelta` so the unit carries a real civilizational cost.
- `data/buildings.json` - added `axeman` to the shared Barracks roster so the house gate continues to live in simulation instead of fragmented per-building data.
- `src/game/core/simulation.js` - deepened `queueProduction(...)` so Ironmark blood levy and blood-production burden can vary per combat unit, making Axeman materially heavier than generic infantry.
- `src/game/main.js` - command-panel training details now show real Ironmark blood levy and added blood-production load instead of only generic cost plus population text.
- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` - added schema and runtime proof for Ironmark-only access, heavier blood levy, live blood-production load, and restore continuity.

1 item moved to LIVE (Ironmark `axeman` unique-unit lane, first layer). Vectors 1, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 68): Convergence-Tier World-Pressure Escalation (Vector 3 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added a shared `getWorldPressureConvergenceProfile(...)` seam and exposed convergence-only rival tempo caps plus tribal raid cadence through the world-pressure snapshot.
- `src/game/core/ai.js` - Stonehelm now compresses military, covert, and faith tempo more sharply under `Convergence`, and tribal raid cadence now sharpens beyond the prior level-3 pressure branch.
- `src/game/main.js` - the world pill now surfaces convergence sabotage, espionage, holy-war, and tribal-cadence pressure when the dominant realm reaches `Convergence`.
- `tests/runtime-bridge.mjs` - added runtime coverage for convergence-profile activation, sharper rival tempo than `Severe`, harsher tribal cadence than `Severe`, world-pill legibility, and restore continuity.

1 item moved to LIVE (convergence-tier world-pressure rival tempo and tribal escalation, first layer). Vectors 3, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 67): Player-Facing Dossier Sabotage Actionability (Vector 2 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/main.js` - rival-court rows now expose a real `Launch Dossier Sabotage` action when the player holds a live counter-intelligence dossier on that hostile court, using shared sabotage terms, real projected chance, real target selection, and honest disabled-state handling.
- `src/game/core/simulation.js` - exported `getSabotageOperationTerms(...)` so player UI and AI retaliation use the same dossier-backed sabotage-term seam and no sabotage math is duplicated in the panel layer.
- `tests/runtime-bridge.mjs` - added runtime coverage for player-raised counter-intelligence, intercepted hostile espionage, player dossier sabotage target resolution, live sabotage launch, and restore continuity of dossier provenance plus support bonus.

1 item moved to LIVE (player-facing dossier-backed sabotage actionability, first layer). Vectors 2, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 66): Dossier-Backed Sabotage Retaliation (Vector 2 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added a shared dossier-backed sabotage profile, extended sabotage launch terms and operations with dossier provenance plus sabotage-support bonus, and preserved the new retaliation metadata through the existing operation snapshot lane.
- `src/game/core/ai.js` - Stonehelm sabotage now reuses live counter-intelligence dossiers for subtype selection, target selection, and sabotage-support carry-through instead of using only the generic sabotage picker.
- `src/game/main.js` - counter-intelligence dossier rows now surface recommended sabotage retaliation and dossier support, and active sabotage operations now expose dossier provenance in the dynasty panel.
- `tests/runtime-bridge.mjs` - added runtime coverage for restored dossier-driven sabotage retaliation, command-hall target selection after intercepted espionage, dossier provenance preservation, sabotage support bonus, and no redundant espionage reopen.

1 item moved to LIVE (dossier-backed sabotage retaliation and covert court-counterplay, first layer). Vectors 2, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 65): World-Pressure Splinter Opportunism (Vector 2 + Vector 3 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added a shared parent-pressure splinter-opportunity profile, applied it to hostile minor-house levy tempo and retinue-cap growth, surfaced live message-log pressure changes, and exposed splinter-opportunity state through the world-pressure snapshot.
- `src/game/core/ai.js` - hostile minor-house territorial AI now reads parent-realm world pressure, expands threat response, and shortens retake regroup timing under overextended parent pressure.
- `src/game/main.js` - hostile minor-house rows now surface parent-pressure severity, levy tempo, retake tempo, and cap bonus, and the world pill now reports pressured splinter count plus splinter levy and retake tempo.
- `tests/runtime-bridge.mjs` - added runtime coverage for pressure-driven splinter levy acceleration, retinue-cap escalation, faster retake cadence, world-pill legibility, and restore continuity.

1 item moved to LIVE (world-pressure-driven splinter opportunism against hostile parent realms, first layer). Vectors 2, 3, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 64): World-Pressure Internal Dynastic Destabilization (Vector 2 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added a shared world-pressure-to-cadet instability profile, applied it inside lesser-house loyalty drift, persisted cadet pressure severity on active lesser houses, surfaced cadence messages for changing internal strain, and exposed cadet penalty plus pressured-branch count through the world-pressure snapshot.
- `src/game/main.js` - dynasty lesser-house rows now surface world-pressure severity and daily cadet drift, and the world pill meta now exposes cadet drift and pressured cadet count when the current realm is the dominant target.
- `tests/runtime-bridge.mjs` - added runtime coverage for worsened cadet loyalty drift under severe world pressure, world-pressure snapshot legibility of the new cadet penalty, and restore continuity of the new internal-pressure state.

1 item moved to LIVE (world-pressure-driven internal dynastic destabilization, first layer). Vectors 2, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 63): Hartvale Playability And House-Gated Unique-Unit Access (Vector 2 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added shared house-gated unit-trainability helpers, exposed `getTrainableUnitIdsForBuilding`, and made `queueProduction` enforce prototype and house ownership instead of trusting raw building rosters.
- `src/game/main.js` - command-panel training now reads the simulation-filtered trainable roster, so the UI only surfaces units the current house can actually queue.
- `data/houses.json`, `data/units.json`, `data/buildings.json` - Hartvale is now prototype-playable, Verdant Warden is now prototype-enabled, and Barracks now includes the Hartvale unit in the shared roster that simulation gates by house.
- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` - added coverage proving Hartvale can surface and queue Verdant Warden while Ironmark and Stonehelm cannot.

2 items moved to LIVE (Hartvale playable-house enablement, house-gated unique-unit availability, first layer). Vectors 2, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 62): Lesser-House Marital Anchoring And Branch Pressure (Vector 2 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - mixed-bloodline cadet branches now carry a live marriage-anchor profile, including active, dissolved, strained, and fractured states; that anchor materially changes daily cadet loyalty drift, emits state-change messages, and persists through restore.
- `src/game/main.js` - lesser-house rows now surface mixed-bloodline drift plus marriage-anchor house, status, drift, and branch-child support inside the dynasty panel.
- `tests/runtime-bridge.mjs` - added runtime coverage for active branch anchor support, death-driven anchor loss, hostility-driven branch fracture, and restore continuity of the new cadet-house state.

1 item moved DOCUMENTED -> LIVE (lesser-house marital anchoring and controlled mixed-bloodline branch pressure, first layer). Vectors 2 and 6 advanced. Tests green.

---

## 2026-04-15 (Session 61): Counter-Intelligence Dossiers And Retaliatory Reuse (Vector 2 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - successful covert interceptions now create source-scoped counter-intelligence dossiers on hostile courts, preserve interception history by hostile source faction, and allow dossier and espionage report types to coexist on the same target without silent replacement.
- `src/game/core/ai.js` - Stonehelm now recognizes counter-intelligence dossiers as live retaliatory knowledge, accelerates sabotage and assassination timing from those dossiers, and avoids reopening redundant espionage before retaliating.
- `src/game/main.js` - dynasty intelligence-report rows now surface real dossier labels, intercepted operation type, and hostile network-hit count.
- `tests/runtime-bridge.mjs` - added runtime coverage for interception-dossier creation, restore continuity of source-scoped watch history, and retaliation assassination launched from dossier state without fresh espionage.

1 item moved DOCUMENTED -> LIVE (counter-intelligence interception-network consequence and retaliation, first layer). Vectors 2, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 60): World Pressure And Late-Stage Escalation (Vector 3 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added live kingdom-level world-pressure score, streak, and escalation state, dominant-leader detection, frontier-loyalty and legitimacy consequence, snapshot exposure of the new pressure fields, and restore continuity for the pressure state.
- `src/game/core/ai.js` - Stonehelm now compresses military and covert timers against the world-pressure leader, holy-war timing now reads target-world-pressure state, and tribes now preferentially raid the dominant kingdom with faster cadence under higher pressure.
- `src/game/main.js` - the 11-pill world-pressure surface now shows pressure labels for the current target and exposes score, streak, frontier-loyalty penalty, legitimacy pressure, and dominant-leader meta instead of relying only on generic signal counts.
- `tests/runtime-bridge.mjs` - added runtime coverage for cycle-driven world-pressure escalation, frontier consequence, legitimacy pressure, restore continuity, Stonehelm timer compression, and tribal retargeting toward the dominant kingdom.

1 item moved DOCUMENTED -> LIVE (broader world pressure and late-stage escalation, first layer). Vectors 3, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 59): Counter-Intelligence And Bloodline-Targeting Defense (Vector 2 + Vector 3 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added live `dynasty.counterIntelligence` watch state, counter-intelligence terms and launch entry, timed watch resolution, interception tracking, role-targeted bloodline defense, legitimacy reinforcement on successful interceptions, restore-safe `dynastyCounter` counter reconstruction, and live covert-resolution math that now reads defensive watch state at resolve time.
- `src/game/core/ai.js` - Stonehelm now raises counter-intelligence when the player already holds hostile intelligence, when player covert pressure is in flight, or when hostility plus dynastic weakness makes covert defense urgent.
- `src/game/main.js` - dynasty panel now surfaces active counter-intelligence watch state, guarded roles, ward support, interception history, player-side raise-watch action, and rival-court covert protection in espionage and assassination details.
- `tests/runtime-bridge.mjs` - added runtime coverage for watch activation, projected covert-chance suppression, guarded bloodline shielding, foiled espionage and assassination interception tracking, legitimacy reinforcement, restore continuity of live watch state, fresh post-restore `dynastyCounter` ids, and AI defensive reciprocity.

1 item moved DOCUMENTED -> LIVE (covert counter-intelligence and bloodline-targeting defense, first layer). Vectors 2, 3, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 58): Field-Water Attrition And Desertion (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - prolonged critical dehydration now accumulates a live collapse timer, applies real field-water attrition damage, escalates into desertion risk, buffers that collapse through commander presence, preserves the new state through snapshot export and restore, and now keeps active holy war pressure meaningful through sustained legitimacy drain alongside territorial strain.
- `src/game/core/ai.js` - Stonehelm now distinguishes between critical dehydration, active attrition, and breaking lines at desertion risk, regrouping more aggressively and extending assault delay when the water line is collapsing.
- `src/game/main.js` - the 11-pill logistics dashboard now surfaces attrition and desertion-risk state, and the debug overlay now exposes the expanded field-water collapse profile for runtime legibility.
- `tests/runtime-bridge.mjs` - added runtime coverage for attrition onset, health loss, desertion collapse, commander-buffered discipline, restore continuity of collapse state, AI break-off behavior under dehydration collapse, and sustained holy-war pressure through territorial or legitimacy consequence.

1 item moved DOCUMENTED -> LIVE (longer-run field-water attrition and desertion, first layer). Vectors 1, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 57): Marriage Governance By Head Of Household (Vector 2 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added shared marriage-governance status and terms, routed proposal and acceptance through live household authority, required a diplomatic envoy on the offering side, applied regency legitimacy strain, persisted governance metadata on proposals and marriages, and surfaced marriage governance through faction snapshot state.
- `src/game/main.js` - dynasty panel now exposes household marriage authority, offering envoy, sanction and approval provenance on proposals and marriages, blocked target-household approval, and governed proposal terms on the player action.
- `tests/runtime-bridge.mjs` - added runtime coverage for direct head approval, heir-regency proposal strain, envoy-loss blocking, regency acceptance, snapshot legibility, and restore continuity of governance metadata.

1 item moved DOCUMENTED -> LIVE (marriage control by head of household, first layer). Vectors 2 and 6 advanced. Tests green.

---

## 2026-04-15 (Session 56): Faith Operations, Missionary Pressure, and Holy War Declaration (Vector 2 + Vector 4 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added live faith operations, missionary conversion pressure, holy war declaration, ongoing holy-war pulse state, faith-fueled operation costs, territorial or legitimacy pressure, and restore-safe `faithHolyWar` id continuity.
- `src/game/core/ai.js` - Stonehelm now launches missionary pressure against rival covenant identity and can escalate into holy war declaration under live dynastic and territorial pressure.
- `src/game/main.js` - faith panel now surfaces active faith operations, outbound holy wars, incoming holy wars, and live action buttons for missionary work and holy war declaration.
- `tests/runtime-bridge.mjs` - added runtime coverage for successful missionary pressure, failed missionary work against a warded Order court, holy war persistence and restore continuity, and enemy AI faith-operation escalation.

1 item moved DOCUMENTED -> LIVE (faith operations, missionary conversion and holy war declaration, first layer). Vectors 2, 4, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 55): Espionage and Assassination Covert Operations (Vector 2 + Vector 4 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added espionage and assassination as live `dynasty.operations`, including rival-court intelligence reports, bloodline-targeted assassination resolution, hostility consequence, command and legitimacy fallout, snapshot exposure, and `dynastyIntel` restore-counter continuity.
- `src/game/core/ai.js` - Stonehelm now runs espionage against the player, then escalates into assassination once a live intelligence report exists.
- `src/game/main.js` - dynasty panel now surfaces intelligence reports, rival-court covert pressure, espionage actions, assassination actions, and correct operation summaries for sabotage, espionage, and assassination.
- `tests/runtime-bridge.mjs` - added runtime coverage for successful espionage, snapshot legibility, report restore continuity, fresh post-restore `dynastyIntel` ids, successful assassination, failed assassination, and AI covert reciprocity.

1 item moved DOCUMENTED -> LIVE (additional covert-operation depth, assassination and espionage, first layer). Vectors 2, 4, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 54): Water-Infrastructure Tier 1 and Field-Army Sustainment (Vector 1 + Vector 3 + Vector 6)

Author: Codex.

- `data/buildings.json` - wells and supply camps now expose first-layer army water-support radius and duration so water infrastructure becomes real runtime support rather than descriptive data only.
- `src/game/core/simulation.js` - added field-water sustainment state for land armies, support resolution from owned marches, settlements, wells, supply camps, and linked supply wagons, dehydration strain penalties to speed and attack output, snapshot persistence, and logistics-snapshot exposure.
- `src/game/core/ai.js` - Stonehelm now delays assaults and regroups toward a live water anchor when its assault column is critically dehydrated.
- `src/game/main.js` - logistics pill now surfaces hydrated ratio, strained and critical field armies, and water-anchor depth alongside existing siege-line state.
- `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` - added schema coverage for army water-support building fields plus runtime coverage for dehydration escalation, movement slowdown, wagon-linked rehydration, snapshot persistence, and AI regroup behavior.

1 item moved DOCUMENTED -> LIVE (water-infrastructure tier 1 and field-army water sustainment, first layer). Vectors 1, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 53): Marriage Death and Dissolution Consequence Layer (Vector 2 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added death-driven marriage dissolution on the real bloodline fall path, stamping shared marriage records with dissolution timing and deceased-spouse identity, applying legitimacy loss and oathkeeping mourning, and stopping further gestation on dissolved unions.
- `src/game/main.js` - dynasty panel now separates active marriages from marriages ended by death and surfaces recent widowing details instead of leaving the consequence hidden in raw state.
- `tests/runtime-bridge.mjs` - added runtime coverage for death-triggered dissolution, legitimacy and oathkeeping consequence, gestation halt, and snapshot restore persistence.

1 item moved DOCUMENTED -> LIVE (marriage death and dissolution, first layer). Vectors 2 and 6 advanced. Tests green.

---

## 2026-04-15 (Session 52): Faith-Compatibility Weighting In AI Marriage Logic (Vector 2 + Vector 4 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added shared `getMarriageFaithCompatibilityProfile`, classifying cross-dynasty covenant fit from live faith and doctrine state as `unbound`, `harmonious`, `sectarian`, `strained`, or `fractured`.
- `src/game/core/ai.js` - replaced the old hostility-or-population-deficit-only marriage gate with a faith-aware strategic profile. Compatible covenant fit can now open a legitimacy-repair marriage path, while fractured matches block weak one-signal proposals or acceptances.
- `src/game/main.js` - dynasty panel now exposes covenant stance for incoming proposals, outgoing player offers, and the player proposal action. Pending offers also surface enemy-court posture from AI evaluation metadata.
- `tests/runtime-bridge.mjs` - added harmonious-faith proposal and acceptance coverage plus fractured-faith refusal coverage for both proposal and acceptance branches.

2 items moved DOCUMENTED -> LIVE (faith-compatibility weighting in AI marriage diplomacy, first layer; Blood Dominion / Wild polygamy restriction as enforced runtime rule). Vectors 2, 4, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 51): Mixed-Bloodline Defection Weighting (Vector 2 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - lesser-house promotion now preserves mixed-bloodline provenance from the founder, and `tickLesserHouseLoyaltyDrift` now adds a house-specific mixed-bloodline pressure term driven by active marriage ties and renewed hostility toward the outside bloodline house.
- `src/game/main.js` - dynasty panel lesser-house rows now surface mixed-house drift pressure through the existing legibility lane.
- `tests/runtime-bridge.mjs` - added coverage for mixed-bloodline child generation, promotion into a lesser house, hostility-sensitive loyalty drift, and snapshot persistence of mixed-bloodline provenance.

1 item moved DOCUMENTED -> LIVE (mixed-bloodline defection weighting, first layer). Vectors 2 and 6 advanced. Tests green.

---

## 2026-04-15 (Session 50): Breakaway-March Territorial Levy and Retinue Growth (Vector 2 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added `tickMinorHouseTerritorialLevies(state, dt)`. Held `minor_house` marches now raise local `militia`, `swordsman`, or `bowman` retinue based on claim loyalty and existing retinue depth. Levy consumes food and influence, burdens local loyalty, respects a loyalty-sensitive retinue cap, and spawns real battlefield units from the claim.
- `src/game/core/ai.js` - extended `ensureMinorHouseAi` so levy-state fields (`levyProgress`, `levyStatus`, `levyUnitId`, `retinueCap`, `retinueCount`, `lastLevyUnitId`) are restored additively alongside the already-live defense and regroup state.
- `src/game/main.js` - dynasty panel rival minor-house rows now surface levy state through the existing legibility lane.
- `tests/runtime-bridge.mjs` - added live levy assertions for food and influence spend, loyalty burden, retinue expansion, snapshot round-trip persistence, and fresh post-restore unit ids.

1 item moved DOCUMENTED -> LIVE (breakaway minor-house territorial levy and local retinue growth, first layer). Vectors 2, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 49): Save-State Counter Continuity (Vector 5)

Author: Codex.

- `src/game/core/simulation.js` - `restoreStateSnapshot` now rebuilds live prefix-based counters instead of the obsolete `entityIdCounter`. Prefix maxima now recover for `unit`, `building`, `dynastyOperation`, `lesser-house`, `marriage-proposal`, `marriage`, and `bloodline-child`.
- `tests/runtime-bridge.mjs` - added a post-restore building-placement assertion proving a restored match can mint a fresh building id without duplicating an existing one.
- Browser verification surface: local `http.server` returned `200` for `play.html`, and markup fetch confirmed the launch-to-shell surface is present.

1 item moved DOCUMENTED -> PARTIAL (save / resume continuity). Vector 5 advanced through the restore lane. Tests green.

---

## 2026-04-15 (Session 48): Minor-House AI Activation and Territorial Defense (Vector 2 + Vector 3 + Vector 6)

Author: Codex.

- `src/game/core/ai.js` - added `updateMinorHouseAi(state, dt)` plus lazy AI-state initialization for `minor_house` factions. Breakaway houses now attack nearby hostile combat units threatening their claimed march, retake their claim if dispossessed, and regroup toward the march after pressure clears.
- `src/game/main.js` - browser loop now runs minor-house AI each frame; dynasty panel now surfaces rival minor-house retinue count and live defense posture.
- `tests/runtime-bridge.mjs` - added hostile-pressure and regroup assertions plus save/restore persistence of minor-house AI state.

1 item moved DOCUMENTED -> LIVE (minor-house operational AI, first layer). Vectors 2, 3, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 47): Minor-House Territorial Foothold (Vector 2 + Vector 5 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - `spawnDefectedMinorFaction` now creates a real stabilized border march via `spawnDefectedMinorTerritoryClaim`. The spawned `minor_house` gains a deterministic control point id, inherited continent placement, a non-overlapping nearby claim location, and first-layer food plus influence trickle. Dynamic control-point snapshot export/restore now preserves spatial, settlement-class, trickle, and contested fields so breakaway territory survives save/resume.
- `src/game/main.js` - Dynasty panel rival-minor-house rows now surface current march count so breakaway territorial footprint is legible without a fake new panel.
- Robustness: `createEntityId` now initializes previously unseen prefixes instead of emitting `NaN` ids when new runtime entity families are introduced.
- Test: defection fires -> minor claim exists -> claim owned by minor -> claim is stabilized border settlement -> lesser-house back-reference set -> snapshot round-trip preserves claim.

1 item moved DOCUMENTED -> LIVE (minor-house territory, first layer). Vectors 2, 5, and 6 advanced. Tests green.

---

## 2026-04-15 (Session 46): Civilizational Stability Feedback (Vector 1 + Vector 6)

Author: Codex.

- `src/game/core/simulation.js` - added shared loyalty helper `applyControlPointLoyaltyDelta`, then wired the 90-second realm cycle so strong food and water surplus reinforces owned march loyalty when cap pressure is not active. Crisis paths for famine, water crisis, and cap pressure remain intact.
- `data/realm-conditions.json` - added canonical `effects.stabilitySurplus` thresholds and loyalty reinforcement values.
- `tests/data-validation.mjs` - asserts the new surplus thresholds.
- `tests/runtime-bridge.mjs` - completed the unfinished Session 46 branch by seeding an owned march, proving starvation lowers loyalty, then proving surplus restores loyalty once food, water, and cap pressure conditions are cleared.

No previously documented row moved fully to LIVE, but Vector 1 deepened materially: abundance now affects governance, not only scarcity. Tests green.

---

## 2026-04-15 (Session 45): Minor-House Founder Unit Spawn (Vector 3 + Vector 5)

Author: Claude.

- `src/game/core/simulation.js` — `spawnDefectedMinorFaction` now locates parent's `command_hall` and spawns a militia unit at `center + 64,64` offset assigned to the minor faction id. Commander attachment via `commanderMemberId` links the founder dynasty record to the physical unit; `minor.dynasty.attachments.commanderUnitId` stores the unit id. Minor `population.total` and `cap` set to 1.
- Test: defection fires → units grew by 1 → minor has exactly one militia unit → commander attachment wired → population consistent.

1 item moved DOCUMENTED → LIVE (founder unit spawn). Vectors 3 and 5 advanced. Tests green.

---

## 2026-04-15 (Session 44): Defected Branch As Minor Faction (Vector 2 + Vector 5 + Vector 6)

Author: Claude.

- `src/game/core/simulation.js` — new helper `spawnDefectedMinorFaction(state, lesserHouse, parentFaction)` called from the defection block inside `tickLesserHouseLoyaltyDrift`. Creates a full-shape entry in `state.factions` with `kind: "minor_house"`, hostile to parent, deterministic id `minor-${lh.id}`, founder COPIED (not moved) from parent dynasty with origin back-references, inheriting parent faith commitment at intensity 20, fresh conviction with founding ledger entry, zero population/economy, no AI (first canonical layer). Mirror back-ref `lh.defectedAsFactionId`. Idempotency guard via existing-id check.
- `exportStateSnapshot` — captures `kind`, `originFactionId`, `originLesserHouseId`, `foundedAtInWorldDays` per faction. `restoreStateSnapshot` — when snapshot has a faction that `createSimulation` did not rebuild (mid-match minor houses), constructs a minimal shell before the field-by-field restore so minor houses survive save/resume.
- `src/game/main.js` — Dynasty panel: defected lesser-house rows now show `DEFECTED` status explicitly; new "Rival minor houses (N)" section iterates `state.factions` for `kind === "minor_house"` entries originated from the player so the consequence is visible.
- Test: spawn, hostility, founder copy semantics, idempotency, save/restore round-trip preserves minor faction and its origin.

1 item moved DOCUMENTED → LIVE (defected-branch-as-minor-faction registry spawn). 1 moved NOT-STARTED → FIRST-LAYER LIVE (minor-house presence on world register). Vectors 2, 5, 6 all advanced in one session. Tests green.

---

## 2026-04-14 (Session 43): Lesser-House Defection Event Hook (Vector 2)

Author: Claude.

- `src/game/core/simulation.js` — new exported constants `LESSER_HOUSE_DEFECTION_THRESHOLD = 0`, `LESSER_HOUSE_DEFECTION_GRACE_DAYS = 5`, `LESSER_HOUSE_DEFECTION_LEGITIMACY_PENALTY = 6`. Restructured `tickLesserHouseLoyaltyDrift` so defection check runs unconditionally per tick; loyalty <= threshold starts grace clock; grace window elapsed flips status to `"defected"`, records `departedAtInWorldDays`, applies legitimacy −6 + ruthlessness +1 to parent, pushes canonical message. Recovery above threshold during grace clears the clock.
- Test: defection trigger after grace under sustained zero-loyalty; recovery cancels grace; idempotency on already-defected branches.

1 item moved DOCUMENTED → LIVE (defection event hook). 1 moved NOT-STARTED → FOUNDATION (defected-as-minor-faction). Vector 2 advanced. Tests green.

---

## 2026-04-14 (Session 42): Lesser-House Loyalty Drift Mechanic (Vector 2)

Author: Claude.

- `src/game/core/simulation.js` — new exported constants `LESSER_HOUSE_LOYALTY_MIN = 0`, `LESSER_HOUSE_LOYALTY_MAX = 100`. New helper `computeLesserHouseDailyLoyaltyDelta` reads parent's legitimacy (+0.30 ≥75, +0.15 ≥50, −0.40 <30), oathkeeping (+0.20 per 5-bucket step), ruthlessness (−0.20 per 5-bucket step), recent fallen ledger (−0.50 if any). New `tickLesserHouseLoyaltyDrift` runs each tick after `tickLesserHouseCandidates`; uses `Math.floor(inWorldDays)` for snapshot timestamps so drift fires on integer day crossings. Threshold messages at 25/10/0 fire ONCE per crossing via per-cadet `announcedLoyaltyThresholds` dedupe.
- Test: healthy-parent preserves loyalty, weak-parent erodes loyalty, floor and cap clamps verified.

1 item moved DOCUMENTED → LIVE (loyalty drift). 1 moved NOT-STARTED → FOUNDATION (defection foundation laid). Vector 2 advanced again. Tests green.

---

## 2026-04-14 (Session 41): Sortie Cooldown UI Tick-Down (Vector 6)

Author: Claude.

- `src/game/core/simulation.js` — realm-condition snapshot fortification block now exposes `sortieActiveRemaining`, `sortieDurationSeconds`, and `sortieCooldownSeconds` so the UI can render progress percentages without importing constants directly.
- `src/game/main.js` — sortie button label now reads "Ns of 12s remaining at {keep} (P% of burst spent)" during active state and "Ns until ready • P% recharged" during cooldown. Tick-down comes from the existing per-frame `renderPanels()` call in `performFrame`.
- Test: snapshot exposure + ordering invariants (cooldown > duration).

1 item moved DOCUMENTED → LIVE (sortie UI polish). Vector 6 advanced after eight-session Vector 2 streak. Tests green.

---

## 2026-04-14 (Session 40): Battlefield-Hero Renown Award Hook (Vector 2)

Author: Claude.

- `src/game/core/simulation.js` — new exported constants `RENOWN_CAP = 100`, `RENOWN_AWARD_COMBAT_KILL = 1`, `RENOWN_AWARD_FORTIFICATION_KILL = 2`. New helpers `findRenownAwardRecipient` (preference: commander → head → military_command path → any) and `awardRenownToFaction` (caps at 100, appends to per-member `renownLedger` with reason + dual-clock `atInWorldDays`, truncates to 12 entries). `applyDamage` now hooks on confirmed kills: combat-unit kill grants +1 to attacker faction's recipient, fortification-roled building kill grants +2; worker kills canonically excluded.
- Test: combat-kill grants renown + ledger entry; worker-kill does NOT grant.

1 item moved DOCUMENTED → LIVE (battlefield renown hook). 1 moved PARTIAL → LIVE (lesser-house pipeline organic activation). Vector 2 advanced eighth consecutive session — extends longest single-vector streak in project history. Tests green.

---

## 2026-04-14 (Session 39): Marriage Proposal Expiration + Explicit Decline (Vector 2)

Author: Claude.

- `src/game/core/simulation.js` — new exported constant `MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 90`. New `tickMarriageProposalExpiration(state)` runs in `stepSimulation` after `tickMarriageGestation`. Iterates inbox + outbox per faction; pending proposals past the threshold flip to `"expired"` with `expiredAtInWorldDays` recorded; single canonical message per proposal (deduped via source-side check). New exported `declineMarriage(state, proposalId)` marks both inbox + outbox `"declined"` with `declinedAtInWorldDays`, refuses re-decline of resolved proposals, frees both members.
- `src/game/main.js` — Decline button added to each pending inbound proposal in the marriage UI panel, alongside Accept Marriage. Both wire real simulation calls.
- Test: at threshold-5 days pending; at threshold+5 days expired; member free for new proposal post-expiration; explicit decline marks both records; double-decline refused.

2 items moved DOCUMENTED → LIVE (proposal expiration + explicit decline). Marriage system reaches functional completion on its canonical first layer. Vector 2 advanced seventh consecutive session. Tests green.

---

## 2026-04-14 (Session 38): AI Lesser-House Promotion Logic (Vector 2)

Author: Claude.

- `src/game/core/ai.js` — imports `promoteMemberToLesserHouse`. New `enemy.ai.lesserHousePromotionTimer` initialized at 60s. On expiry, `tryAiPromoteLesserHouse(state)` checks: candidate-list non-empty, legitimacy < 90 (canonical: secure house has no urgency to dilute head authority), `lesserHouses.length < 3` soft cap. Promotes first eligible candidate via shared `promoteMemberToLesserHouse`. Long cooldown (180s) on success, medium retry (45s) on declined gate. Reuses S35 player promotion path so all canonical effects fire (legitimacy +3, stewardship +2, dual-clock founding timestamp, candidate pruning).
- Test: promote-when-low-legitimacy, no-double-promote-when-no-candidates, refuse-when-secure (legitimacy 95).

1 item moved DOCUMENTED → LIVE (AI lesser-house promotion). 1 moved PARTIAL → LIVE (lesser-house pipeline bidirectionality). Vector 2 advanced sixth consecutive session — longest single-vector streak in project history. Tests green.

---

## 2026-04-14 (Session 37): AI Marriage Inbox Processing (Vector 2 + Vector 3)

Author: Claude.

- `src/game/core/ai.js` — imports `acceptMarriage`. New `enemy.ai.marriageInboxTimer` initialized at 30s, decrements per tick, independent of proposal cooldown. On expiry, `tryAiAcceptIncomingMarriage(state)` finds first pending proposal where `sourceFactionId === "player"`, runs `shouldAiAcceptMarriage` gate (mirror of S36 proposal gate: hostility OR pop deficit < 85%), accepts via `acceptMarriage` if gate passes. Long cooldown (180s) on success, fast retry (30s) on declined gate.
- `shouldAiAcceptMarriage` is a pure helper shared symmetrically with proposal logic to prevent drift between "AI offers" and "AI accepts" criteria.
- Test: accept-under-hostility verifies marriage in force after AI tick; refuse-when-no-gate verifies proposal stays pending under cleared hostility + equalized populations.

1 item moved DOCUMENTED → LIVE (AI inbox processing). 1 moved PARTIAL → LIVE (marriage end-to-end bidirectionality). Vector 2 advanced fifth consecutive session. Tests green.

---

## 2026-04-14 (Session 36): AI Marriage Proposal Reciprocity (Vector 2 + Vector 3)

Author: Claude.

- `src/game/core/ai.js` — imports `proposeMarriage`. New `enemy.ai.marriageProposalTimer` initialized at 90s, decrements per tick. On expiry, `tryAiMarriageProposal(state)` runs. Strategic gate: hostility-currently-active OR enemy population < 85% of player population (either signal sufficient). Hard guards: no double-marriage between same factions, no spam if pending proposal already in player inbox, valid non-head members on both sides. Long cooldown (240s) on success, short retry (60s) on declined gate.
- No `main.js` changes — Session 34 marriage UI inbox surfaces AI-originated proposals automatically with the same Accept Marriage button.
- Test: full AI proposal lifecycle including no-double-propose-while-pending guard and no-propose-after-marriage-in-force guard.

1 item moved DOCUMENTED → LIVE (AI marriage reciprocity). Vector 2 advanced fourth consecutive session. Marriage now a true two-way diplomatic channel. Tests green.

---

## 2026-04-14 (Session 35): Lesser Houses Promotion Pipeline (Vector 2)

Author: Claude.

- `src/game/core/simulation.js` — `createDynastyState` seeds `lesserHouses: []` and `lesserHouseCandidates: []`. New exports: `promoteMemberToLesserHouse`, `LESSER_HOUSE_RENOWN_THRESHOLD` (30), `LESSER_HOUSE_MIN_PROMOTIONS` (1), `LESSER_HOUSE_LEGITIMACY_BONUS` (3), `LESSER_HOUSE_INITIAL_LOYALTY` (75). Internal qualifying-path set: `military_command`, `covert_operations`, `governance`. Head of bloodline canonically excluded.
- New `tickLesserHouseCandidates(state)` runs in `stepSimulation` after `tickMarriageGestation`. Auto-flags eligible members, announces once via canonical message, drops stale ids.
- `promoteMemberToLesserHouse` creates lesser house with founder metadata + dual-clock timestamp, marks founder member, awards +3 legitimacy and +2 stewardship conviction, removes from candidate list, pushes canonical message.
- `src/game/main.js` — Dynasty panel surfaces active lesser houses (name + loyalty + founder) and per-candidate "Promote to Lesser House" action button. All buttons wire real simulation calls.
- Test: full pipeline including head-of-bloodline exclusion, religious_leadership exclusion, threshold gate, double-promotion guard, legitimacy + conviction rewards.

1 item moved DOCUMENTED → LIVE (lesser houses pipeline). 1 moved DOCUMENTED → PARTIAL (battlefield-hero renown wiring still pending). Vector 2 advanced third consecutive session. Tests green.

---

## 2026-04-14 (Session 34): Marriage UI Panel (Legibility-Follows-Depth)

Author: Claude.

- `src/game/main.js` — imported `proposeMarriage` and `acceptMarriage` from simulation. Three new sections in `renderDynastyPanel`: active marriages list (player member title + spouse house description + child status), marriage proposals received (each with an Accept Marriage action button calling `acceptMarriage(state, proposal.id)`), and a Propose Marriage button surfacing when player has an eligible member and no inbound proposal queued (targets enemy faction's first available member via canonical first-layer convenience, calls `proposeMarriage`).
- All buttons wire through real simulation functions per canonical "no decorative UI" mandate.
- Closes Session 33's marriage system within the canonical 2-session legibility-follows-depth window.

1 item moved DOCUMENTED → LIVE (marriage UI surface). Marriage now end-to-end player-actionable. Tests green.

---

## 2026-04-14 (Session 33): Marriage System First Canonical Layer (Vector 2 Advance)

Author: Claude.

- `src/game/core/simulation.js` — marriage state seeded in `createDynastyState` (marriages, marriageProposalsIn, marriageProposalsOut). New exported `proposeMarriage` and `acceptMarriage`. Cross-faction only. Polygamy canonically gated (Blood Dominion + Wild only via `POLYGAMY_FAITHS`). Source/target keep separate marriage records (source `isPrimaryRecord: true` for child generation).
- New `tickMarriageGestation(state)` runs in `stepSimulation` after `tickDynastyOperations`. After 280 in-world days (canonical 9-month gestation), generates child member with `mixedBloodline` metadata, pushes canonical birth message.
- Marriage acceptance drops mutual hostility, +2 oathkeeping conviction both sides, +2 legitimacy both sides, declares 30 in-world days for the wedding.
- Test: full propose → accept → diplomatic detente → gestation → child creation; second-marriage polygamy gate rejection.

1 item moved DOCUMENTED → LIVE (marriage layer), 1 moved DOCUMENTED → DATA-ONLY (mixed-bloodline defection slider). Vector 2 advanced after 19 sessions of stagnation. Tests green.

---

## 2026-04-14 (Session 32): Continental Architecture (Secondary Continent + Cross-Water Campaign)

Author: Claude.

- `data/maps/ironmark-frontier.json` — 3 new water patches isolating south continent; new control point `cliffsong_outpost` with `continentId: "south"`; existing `stonefield_watch` tagged `continentId: "home"`.
- `src/game/core/simulation.js` — capture branch fires extra dual-clock declaration (28 days) on cross-continental capture + canonical message; snapshot worldPressure exposes continentalHoldings + offHomeContinentHoldings.
- `src/game/main.js` — World pill meta surfaces off-home holdings.
- Tests: continent CP existence + divide patches + snapshot fields + holding count after capture.

2 items moved: continental architecture PARTIAL → LIVE, cross-water campaign DOCUMENTED → LIVE.

---

## 2026-04-14 (Session 31): Fire Ship One-Use Sacrifice Combat

Author: Claude.

- `src/game/core/simulation.js` — `updateCombatUnit` sets attacker health to 0 after damage application when `unitDef.oneUseSacrifice === true`. Canonical detonation message fires.
- Fire Ship removed from state on next tick via existing dead-unit filter.
- Test: Fire Ship attacks War Galley, Fire Ship destroyed, Galley wounded.

1 item moved DATA-ONLY → LIVE. Tests green.

---

## 2026-04-14 (Session 30): Transport Embark/Disembark Runtime

Author: Claude.

- `src/game/core/simulation.js` — exported `embarkUnitsOnTransport` and `disembarkTransport`. Embark requires same-faction, land-domain units within 2.5 tiles, respects capacity. Disembark scans 1-tile ring for land tile, ejects with small offset per unit.
- `updateUnits` skips embarked units.
- Test: full embark → move → disembark round trip with militia + swordsman on Transport Ship.

1 item moved DATA-ONLY → LIVE. Tests green.

---

## 2026-04-14 (Session 29): AI Naval Reactivity

Author: Claude.

- `src/game/core/ai.js` — new naval reactivity branch: Stonehelm detects player harbor/vessel presence, places `harbor_tier_1` near NE coastal bay, then queues scout_vessel first and war_galley second when player is naval-active. Canonical message on placement. Rate-limited.
- Satisfies AI reactivity mandate: naval no longer a player sandbox.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 28): Canonical 6-Vessel Roster Complete + Harbor L2

Author: Claude.

- `data/units.json` — Transport Ship (capacity 6), Fire Ship (oneUseSacrifice flag, atk 140), Capital Ship (apex, atk 46 / range 170 / armor 14).
- `data/buildings.json` — Harbor L2 (Great Harbor, 4x4, trains fire_ship + capital_ship, dropoff includes gold); Harbor L1 adds transport_ship to trainables.
- Build palette extended.
- Tests: 6-class vessel roster + Fire Ship flag + Transport capacity + Harbor L2 existence and trainables.

2 items moved DOCUMENTED → LIVE (6-vessel roster + Harbor L2). 2 items moved DOCUMENTED → DATA-ONLY with explicit runtime follow-up (Fire Ship sacrifice mechanic, Transport embark/disembark). Tests green.

---

## 2026-04-14 (Session 27): Naval Foundation First Canonical Layer

Author: Claude.

- `data/buildings.json` — `harbor_tier_1` (navalRole harbor, navalTier 1, requiresCoastalAdjacency, 3x3, food+wood dropoff, trains 3 vessels).
- `data/units.json` — 3 canonical vessels (fishing_boat, scout_vessel, war_galley), all role `vessel`, movementDomain `water`.
- `data/maps/ironmark-frontier.json` — two coastal water patches (NW bay + NE bay) + existing river for vessel navigation.
- `src/game/core/simulation.js` — `isWaterTileAt`, `isFootprintAdjacentToWater` helpers; coastal adjacency gate in `attemptPlaceBuilding`; movement-domain gate in `issueMoveCommand` (vessels→water, land→land).
- `src/game/core/renderer.js` — harbor silhouette (stone + pier + moored vessel glyph + flag); vessel silhouette (hull + mast + sail + water ripple); water terrain rendering on main view + minimap.
- `src/game/main.js` — harbor added to worker build palette.
- Tests: harbor + 3 vessels + coastal adjacency + movementDomain + map-has-water checks.

1 item moved DOCUMENTED → LIVE. 1 item moved DOCUMENTED → PARTIAL (continental architecture now has water foundation). Tests green.

---

## 2026-04-14 (Session 26): Dual-Clock Declaration Seam (First Canonical Layer)

Author: Claude.

- `state.dualClock` with inWorldDays, daysPerRealSecond (default 2), rolling declarations log (cap 32).
- `tickDualClock` called in stepSimulation; `declareInWorldTime(state, days, reason)` fires bonus declarations.
- Territorial capture fires `declareInWorldTime(14, "Captured X")`.
- Snapshot exposes dualClock.{inWorldDays, inWorldYears, recentDeclarations}.
- Canonical default matches master doctrine "1 to 2 years major battles" band.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 25): Save/Resume Round Trip Complete

Author: Claude.

- `src/game/core/simulation.js` — new `restoreStateSnapshot(content, snapshot)` rebuilds full live state from versioned export. Restores meta, realm cycle, world (cp + settlements + nodes), all factions (resources/pop/faith/conviction/dynasty/ai/strain/blood/dark), units (position/health/commander/timers/command), buildings (progress/health/queue/burn/poison/gate). Projectiles reset (ephemeral). Entity id counter advances past max seen.
- Test: round-trip with mutations survives (gold, blood load, dark streak) and restored state accepts simulation ticks.

1 item moved PARTIAL → LIVE. Canonical Decision 16 save/resume end-to-end operational.

---

## 2026-04-14 (Session 24): Save-State Serialization Primer

Author: Claude.

- `src/game/core/simulation.js` — new `exportStateSnapshot(state)` returns versioned deterministic JSON covering meta, realm cycle, world (control points + settlements + resource nodes), factions (including faith/conviction/dynasty/ai/strain/blood-load/dark-extremes), units (with timers + commands), buildings (with burn/poison/gate-exposed state). Version 1 for forward compat.
- Test: version check + key blocks + JSON round-trip + non-trivial length.

1 item moved DOCUMENTED → LIVE (export side). Full save/resume now PARTIAL pending restore.

---

## 2026-04-14 (Session 23): Supply-Protection Patrols (Longer-Siege AI Canonical Layer Complete)

Author: Claude.

- `src/game/core/ai.js` — Stonehelm assigns up to 2 combat units per live supply wagon as escort patrol when siege chain is live and army has capacity (>= 5, leaves 3 for assault). `supplyPatrolUnitIds` persists; cleared when chain breaks. Canonical message on assignment.
- All 4 canonical session-9 longer-siege layers now LIVE.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 22): Apex Covenant L4 + L5 Apex Units (Full Faith Progression Complete)

Author: Claude.

- `data/buildings.json` — `apex_covenant` (faithTier 4, requiresFaithIntensity 80, amp 3.6x, radius 400, 5x5 footprint, biggest cost).
- `data/units.json` — 4 canonical L5 apex units (The Unbroken / The Sacrificed / The Mandate / The First Wild), stage 5, role faith-apex, faith-bound.
- `src/game/core/simulation.js` — intensity gate wired into `attemptPlaceBuilding`.
- `src/game/main.js` — Apex added to worker palette.
- `src/game/core/renderer.js` — towering obelisk silhouette with 82-tile aura.
- Full canonical L1→L4 faith progression now end-to-end LIVE.

2 items moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 21): Grand Sanctuary L3 + Canonical L4 Faith Unit Roster

Author: Claude.

- `data/buildings.json` — `grand_sanctuary` (faithTier 3, amp 3.0x, radius 320, ward bonuses, 4x4 footprint, 8 L4 units in trainables).
- `data/units.json` — 8 canonical L4 faith units (Flame Herald / Pyre Sovereign / Blood Consort / Crimson Exarch / Mandate Paladin / Edict Inquisitor / Elder Grove Keeper / Predator Ascendant).
- `src/game/main.js` — Sanctuary added to worker palette.
- `src/game/core/renderer.js` — Sanctuary silhouette with tall spire + pillars + 64-tile aura.
- `tests/data-validation.mjs` — Sanctuary + 8 L4 units + Sanctuary trainables coverage.

2 items moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 20): Full Canonical L3 Faith Unit Roster

Author: Claude.

- `data/units.json` — 8 canonical L3 faith units: Flame Warden + Judgment Pyre (Old Light L/D), Consecrated Blade + Crimson Reaver (Blood Dominion L/D), Mandate Sentinel + Edict Enforcer (Order L/D), Root Warden + Predator Stalker (Wild L/D). All stage 3, role "faith-unit", bound via faithId + doctrinePath.
- `data/buildings.json` — covenant_hall `trainableUnits` now contains all 8.
- `src/game/core/simulation.js` — `queueProduction` rejects faith-unit training unless faction has committed to matching covenant AND doctrine path.
- `tests/data-validation.mjs` — full 8-unit roster existence + binding + role + prototypeEnabled + Hall trainables check.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 19): Dark-Extremes World Pressure Trigger

Author: Claude.

- `tickRealmConditionCycle` tracks `darkExtremesStreak` per faction; 3+ consecutive apex_cruel cycles sets `darkExtremesActive = true`.
- `updateNeutralAi` scales tribe raid timer 0.6x (40% more frequent) when any kingdom is darkExtremesActive.
- Snapshot worldPressure exposes darkExtremesActive + darkExtremesStreak.
- Canonical message fires on transition to active.
- Tests: fresh inactive default, forced Apex Cruel + streak 3 surfaces active.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 18): Conviction Milestone Powers Per Band

Author: Claude.

- `src/game/core/simulation.js` — `CONVICTION_BAND_EFFECTS` table for 5 canonical bands (apex_moral, moral, neutral, cruel, apex_cruel) with 6 modifier axes each (stabilization, capture, loyalty-protect, heal, growth, attack).
- `getConvictionBandEffects` accessor; INDEPENDENT of faith per master doctrine XX.
- Wired into `updateControlPoints` (stabilization + capture), `tickPopulationGrowth` (growth multiplier).
- Snapshot conviction block exposes `bandEffects`; Conviction HUD pill meta surfaces active modifiers.
- Tests: neutral baseline, Apex Cruel direction, Apex Moral mirror.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 17): Covenant Hall L2 Canonical Covenant Building

Author: Claude.

- `data/buildings.json` — new `covenant_hall` (faithRole hall, faithTier 2, requiresFaithCommitment true, footprint 3x3, amp 2.4x, radius 240, cost 160/100/140/30).
- `src/game/core/simulation.js` — `attemptPlaceBuilding` gates on `faith.selectedFaithId` when building requires commitment.
- `getWayshrineExposureMultiplierAt` now accepts shrine/hall/sanctuary roles; ceiling raised 3→4.
- `src/game/main.js` — added to worker build palette.
- `src/game/core/renderer.js` — triple-pillar Hall silhouette with 48-tile aura ring.
- `tests/data-validation.mjs` — Hall existence + faithTier 2 + requiresFaithCommitment + amp>Wayshrine.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 16): Longer-Siege AI Repeated-Assault Window Logic

Author: Claude.

- `src/game/core/ai.js` — new `else if` branch: after post-repulse cooldown expires and `repeatedAssaultAttempts > 0`, re-issue attack-on-hall with angular offset per attempt count. Cap 4 attempts. Canonical message "Stonehelm renews the assault from a new angle (attempt N)."
- Post-repulse branch now increments `repeatedAssaultAttempts` counter.
- All four canonical Session 9 longer-siege items now LIVE or PARTIAL.

1 item moved DOCUMENTED → LIVE. Tests green.

---

## 2026-04-14 (Session 15): Ironmark Blood Production Deepening (Sustained-War Load + Growth Depress + Drift)

Author: Claude.

- Ironmark blood levies now increment `faction.bloodProductionLoad` by 1.5. Load > 12 with 40s cooldown triggers +2 ruthlessness drift events. Load decays 2.5 per realm cycle.
- `tickPopulationGrowth` stretches growth interval up to 1.8x for Ironmark when load > 8.
- Snapshot population block exposes `bloodProductionLoad` and `bloodProductionActive` (Ironmark + load > 8).
- Population HUD pill surfaces "blood levy load X • growth slowed" with yellow band when active.
- Tests: load tracking, threshold surfacing, non-Ironmark guard.

1 item moved PARTIAL → LIVE. Tests green.

---

## 2026-04-14 (Session 14): Longer-Siege AI Post-Repulse + Supply-Collapse Adjustments + Hostile Post-Repulse Legibility

Author: Claude.

- `src/game/core/ai.js` — two new `else if` branches in the keep-assault commit path: post-repulse retreat (cohesion penalty active) and supply-collapse retreat (siege was ready but supplied engines dropped to 0 mid-approach). Both pull siege force back to stage point with canonical messages.
- `src/game/core/simulation.js` — `getRealmConditionSnapshot` worldPressure block now exposes `hostilePostRepulseActive`, `hostilePostRepulseRemaining`, `hostileCohesionStrain`.
- `src/game/main.js` — World pill displays "Rival repulsed Xs" (green band) when hostile post-repulse active; meta line includes rival strain.
- `tests/runtime-bridge.mjs` — hostile-post-repulse snapshot field presence + positive-case assertion.

All tests green. 3 items moved DOCUMENTED → LIVE. Session 14 honored preservation-first posture.

---

## 2026-04-14 (Session 13): Faith Prototype Enablement + Wayshrine Canonical Shrine Building

### Scope

Opened Vector 4 (Faith and Conviction depth) with first canonical layer. All four covenants flipped to `prototypeEnabled: true`. Canonical Wayshrine L1 shrine building live in data, simulation, renderer, build palette, and tests. Amplifies faith exposure accrual 1.8x per shrine at nearby sacred sites (capped 3x cumulative), and boosts faith intensity regen for committed covenants, strengthening ward profiles downstream.

Author: Claude

### Runtime Surface Updates

- `data/faiths.json` — Old Light, Blood Dominion, The Order, The Wild all `prototypeEnabled: true`
- `data/buildings.json` — new `wayshrine` building (faithRole shrine, faithTier 1, footprint 2x2, health 420, buildTime 14, amp 1.8, radius 180, intensity bonus 0.18, cost gold 60 wood 40 stone 50)
- `src/game/core/simulation.js` — `updateFaithExposure` extended with Wayshrine amplification; new `getWayshrineExposureMultiplierAt` helper
- `src/game/main.js` — `wayshrine` added to `WORKER_BUILD_ORDER`
- `src/game/core/renderer.js` — Wayshrine silhouette (stone base + central pillar + accent sigil + faint aura ring); label positioning updated to accommodate faithRole

### Tests

- `tests/data-validation.mjs` — all 4 faiths prototypeEnabled, Wayshrine existence + fields
- `tests/runtime-bridge.mjs` — baseline vs amplified exposure test (Wayshrine presence yields measurably higher accrual at same sacred site over 3 seconds)

### Verification

- All syntax checks pass
- `node tests/data-validation.mjs` - passed
- `node tests/runtime-bridge.mjs` - passed

### Preservation Statement

Session 13 moved 2 items from DOCUMENTED to LIVE and 1 from DOCUMENTED to PARTIAL. L2 Hall, L3 Grand Sanctuary, L4 Apex structures remain DOCUMENTED with explicit canonical follow-up logged.

---

## 2026-04-14 (Session 12): Commander Sortie UI + Longer-Siege AI Relief-Window Awareness

### Scope

Session 12 closed the legibility-follows-depth debt from Session 10 (commander keep-presence sortie had no UI surface) and advanced longer-siege AI with the first canonical layer of mid-siege adaptation (relief-window awareness). Both changes connect to multiple live systems per the Canonical Interdependency Mandate.

Author: Claude

### Runtime Surface Updates

- `src/game/main.js` — `issueKeepSortie` imported. New action row in `renderDynastyPanel` (after governor line) renders a live-state sortie button with six canonical labels (Call Sortie / Sortie Active / Sortie On Cooldown / Sortie Requires Commander / Sortie No Threat / Sortie Unavailable) driven by `getRealmConditionSnapshot().fortification` flags.
- `src/game/core/ai.js` — new `isReliefArmyApproaching(state, targetFactionId, siegeStagePoint, reliefRadius=380)` helper; new `else if` branch in the formal-siege commit path that holds the final assault when a player relief column is approaching (3+ combat units within 380 tiles of stage, excluding already-engaged d<90). New canonical message: "Stonehelm spots a player relief column and holds the breach commit until the field is secured."

### Tests

- `tests/runtime-bridge.mjs` — sortie happy path (commander at keep + threat injected + cooldown cleared + invocation succeeds + sortieActive set + second invocation denied during cooldown).
- `tests/runtime-bridge.mjs` — relief-window AI awareness (fortified player keep + 4 player relief units near siege stage + updateEnemyAi tick + no new attack-on-hall commands issued).

### Verification

- `node tests/data-validation.mjs` - passed
- `node tests/runtime-bridge.mjs` - passed
- `node --check` on all simulation modules - passed

### Preservation Statement

No canonical system reduced. Session 12 moved 1 item from DOCUMENTED to LIVE (sortie UI) and 1 from DOCUMENTED to PARTIAL (longer-siege AI relief-window layer landed; repeated-assault + post-repulse + supply-protection layers still pending). All tests green.

---

## 2026-04-14 (Session 11): Stonehelm Playable + Hartvale Verdant Warden + AI Sabotage Reciprocity + Overnight Alternation Infrastructure

### Scope

Session 11 extended the house-roster layer (Stonehelm as second playable house with canonical fortification-discount mechanics, Hartvale Verdant Warden entered in data at Off 4 / Def 5 canon), extended AI reciprocity (Stonehelm AI now runs sabotage against the player, eliminating the Session 10 asymmetry), verified the Unity 6.3 LTS lane post first-compile, and established the Claude/Codex overnight alternation infrastructure (revised canonical continuation prompt, Codex-side brief, scheduled-task cron every 5 hours).

Author: Claude

### Runtime Surface Updates

- `data/houses.json`
  - Stonehelm raised from `settled-visual-only` to `partially-locked`, `prototypePlayable: true`, societal advantage documented, canonical `mechanics` block (`fortificationCostMultiplier: 0.8`, `fortificationBuildSpeedMultiplier: 1.2`)
  - Hartvale notes cross-reference the new Verdant Warden data entry
- `data/units.json`
  - New `verdant_warden` canonical Hartvale unique (health 135, speed 62, attackDamage 12, armor 5, defenseRating 5, offenseRating 4, prototypeEnabled false pending Hartvale playability)
- `src/game/core/simulation.js`
  - `getEffectiveBuildingCost(state, faction, buildingDef)` and `getEffectiveBuildTime(state, faction, buildingDef)` apply house mechanics to fortification-role buildings
  - `attemptPlaceBuilding` uses the effective cost path
  - `updateBuildings` worker-tick path multiplies buildRate by `fortificationBuildSpeedMultiplier` when applicable
- `src/game/main.js`
  - `applyHouseSelectOverride(contentRef)` reads `?house=` from URL and swaps player/enemy houseIds on the loaded map before `createSimulation`
- `src/game/core/data-loader.js`
  - Cache-bypass on all data fetches via `cache: "no-store"` and per-load `?cb=` query string, ensuring live data edits reflect on every fresh page load
- `src/game/core/ai.js`
  - `startSabotageOperation` imported from simulation
  - Stonehelm AI runs sabotage with `enemy.ai.sabotageTimer` (45s default), budget check (60 gold + 12 influence), target selection (`pickAiSabotageTarget`: supply_camp → gatehouse → well → farm → barracks), cooldown (85s on success, 25s on hold)

### Tests

- `tests/data-validation.mjs` — Stonehelm prototypePlayable + mechanics bounds, Verdant Warden existence + house + Off 4 / Def 5 profile assertions
- `tests/runtime-bridge.mjs` — Stonehelm swap persists into simulation state, Stonehelm watch_tower placement consumes 20% less stone than canonical, enemy AI triggers sabotage against player when budget + target allow

### Unity Production Lane (Verified Post Session 10)

- `unity/ProjectSettings/ProjectVersion.txt` locked at `6000.3.13f1` (6.3 LTS)
- `unity/Packages/manifest.json` reflects resolved versions (Entities 6.4.0 as the 6.3 LTS companion, Collections 2.5.7, URP 17.3.0, Input System 1.11.2, Addressables 2.5.0)
- `Library/` populated — first compile completed
- Canonical `unity/` project added to Unity Hub
- 15 ECS components + 3 ECS systems from Session 10 on disk, awaiting `Bloodlines → Import → Sync JSON Content` menu run

### Overnight Alternation Infrastructure

- `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_11_REVISED.md` — revised canonical preservation-first continuation prompt (supersedes earlier reusable prompt, preserved additively)
- `03_PROMPTS/CODEX_CONTINUATION_BRIEF.md` — Codex-side alternation brief
- Scheduled task `bloodlines-claude-alternation` registered (`mcp__scheduled-tasks`): cron `12 */5 * * *` fires at 00:12, 05:12, 10:12, 15:12, 20:12 local. Durable, recurring, notifyOnCompletion=true. Each fire creates a dedicated Claude session that reads handoff, advances roadmap, verifies, hands off.

### Verification

- `node tests/data-validation.mjs` - passed
- `node tests/runtime-bridge.mjs` - passed
- `node --check` on all simulation modules - passed
- Live browser boot on `http://localhost:8057/play.html`: 11 pressure pills, zero console errors, zero failed network requests
- House-select visual verification deferred due to browser ES module cache persistence; disk fix is correct (confirmed by served-file inspection)

### Preservation Statement

No canonical system reduced, substituted, or sidelined. Session 11 moved 4 items from DOCUMENTED to LIVE and 1 item from DOCUMENTED to DATA-ONLY with canonical follow-up logged. All tests green.

---

## 2026-04-14 (Session 10): Unity 6.3 LTS Lock + ECS Foundation + Sabotage Operations + Commander Sortie

### Scope

Resolved the longest-standing open blocker (Unity version alignment) and advanced the browser reference simulation on two canonical vectors simultaneously: Military (sabotage as first covert-operation type) and Dynastic (commander keep-presence sortie).

### Unity Production Lane

- `unity/ProjectSettings/ProjectVersion.txt` updated from `6000.4.2f1` to `6000.3.13f1` (Option B, approved by Lance 2026-04-14).
- `unity/Packages/manifest.json` aligned to 6.3 LTS-compatible package versions (Entities 1.4.0, Collections 2.5.7, Entities.Graphics 1.4.0, URP 17.3.0, Input System 1.11.2, Addressables 2.5.0; Burst/Mathematics/Test Framework/Timeline/UGUI/IDE packages retained).
- Authored 15 canonical ECS component files under `unity/Assets/_Bloodlines/Code/Components/`: Position, Faction + FactionKind, Health + DeadTag, UnitType (with Role and SiegeClass enums), BuildingType (with FortificationRole enum), ResourceNode, ControlPoint (with ControlState enum), Settlement + PrimaryKeepTag, Commander + CommanderAtKeepTag, BloodlineMember (with BloodlineRole / Status / Path enums), FaithState + FaithExposureElement buffer + FaithWardedSettlementTag (with CovenantId / DoctrinePath enums), Conviction (with ConvictionBand enum), RealmCondition + RealmCycleConfig, Population + ResourceStockpile, SiegeEngineState + MantletCover.
- Authored 3 canonical ECS systems under `unity/Assets/_Bloodlines/Code/Systems/`: BloodlinesBootstrap (one-shot RealmCycleConfig singleton seeding), RealmConditionCycleSystem (canonical 90-second cycle, famine/water-strain accrual, cohesion decay), PopulationGrowthSystem (canonical 18s growth gated by housing + food + water).
- Updated `unity/README.md` Approved Toolchain section + added "First-Open Workflow" and "Session 10 ECS Foundation" sections.
- Updated `ENVIRONMENT_REPORT_2026-04-14.md` Section 4 with Resolution block marking Option B locked.

### Browser Reference Simulation — Sabotage Operations

- Added `startSabotageOperation(state, factionId, subtype, targetFactionId, targetBuildingId)` as public entry point in `src/game/core/simulation.js`.
- Added four canonical sub-types: `gate_opening`, `fire_raising`, `supply_poisoning`, `well_poisoning`. Each has canonical cost (gold + influence), duration (24–32s), validation (subtype must match target role), and success formula (spymaster renown + 45 offense vs fortification tier × 12 + ward active × 15 + target spymaster × 10 defense).
- Added `tickBuildingStatusEffects(state, dt)` between `updateBuildings` and `tickFortificationReserves` in `stepSimulation`. Applies burn damage from fire_raising and cleans expired status windows.
- Extended `tickSiegeSupportLogistics` so poisoned supply camps are excluded from the wagon-link filter, interdicting siege supply until the poison window expires.
- Extended `tickDynastyOperations` to dispatch sabotage operations independently of the captive-member validation path.
- Conviction bookkeeping: supply_poisoning and well_poisoning record desecration on success; gate_opening and fire_raising record ruthlessness. Failed sabotage records +1 stewardship on the target faction.
- Requires spymaster (via `getDynastyMemberByRole` with canonical role list `["spymaster", "diplomat", "covert_operations"]`).

### Browser Reference Simulation — Commander Keep-Presence Sortie

- Added `issueKeepSortie(state, factionId, settlementId)` as public entry point.
- Sortie requires commander at keep AND active threat AND cooldown not active. Duration 12s. Cooldown 60s.
- During active sortie, `getFriendlyFortificationCombatProfile` returns attack multiplier ×1.22 and sight bonus +22 on top of ward-and-presence buffs for friendly defenders.
- Records +1 oathkeeping conviction event on sortie invocation.
- Exported constants: `SORTIE_DURATION_SECONDS`, `SORTIE_COOLDOWN_SECONDS`.

### Snapshot Expansion

`getRealmConditionSnapshot` fortification block now also exposes: `commanderAtKeep`, `sortieActive`, `sortieCooldownRemaining`, `sortieReady`.

### Tests

- `tests/runtime-bridge.mjs` added: sabotage rejects unknown subtype; `gate_opening` against non-gate rejects; `fire_raising` against command hall succeeds and escrows gold + influence; sortie refuses without commander at keep; snapshot exposes all four new fortification fields with `sortieActive` defaulting false.

### Verification

- `node tests/data-validation.mjs` - passed
- `node tests/runtime-bridge.mjs` - passed
- `node --check` on all simulation modules - passed
- Live browser boot on `http://localhost:8057/play.html`: 11 pressure pills render, zero console errors, zero failed network requests.

### Preservation Statement

No canonical system reduced, substituted, or sidelined. Session 10 moved items from PARTIAL / DOCUMENTED toward LIVE. Unity production lane is now unblocked for first open.

---

## 2026-04-14 (Session 9): Full-Realization Continuation + Ballista + Mantlet + 11-State Dashboard

### Scope

Session 9 launched under the creator's full-project state analysis and continuation directive. The directive reaffirmed the Master Design Doctrine preservation posture: no scope reduction, no MVP substitution, full-scale Bloodlines.

Produced the five demanded deliverables (state analysis, system gap analysis, full continuation plan, next-phase execution roadmap, master continuation reference addendum). Advanced the browser reference simulation on two canonical lanes: full 11-state realm-condition dashboard (Vector 6, Legibility) and ballista + mantlet siege-support classes (Vector 3, Military).

### Analysis + Planning Surfaces

- `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md` — comprehensive state analysis, preserves full protected scope, reconciles canon vs runtime vs data.
- `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md` — system-by-system gap matrix under LIVE / PARTIAL / DATA-ONLY / DOCUMENTED / CANON-LOCKED-IMPLEMENTATION-PENDING / VOIDED legend.
- `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md` — six-vector growth model (civilizational, dynastic, military, faith-and-conviction, world, legibility), parallel lane strategy, non-negotiable session-level rules, exit criteria per vector.
- `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` — concrete ordering for sessions 9 through 30+, wave-by-wave implementation progression.
- `MASTER_BLOODLINES_CONTEXT.md` — Session 9 addendum appended additively.

### Runtime Surface Updates

- `data/units.json`
  - added `ballista` (stage 2, siege-engine, ranged anti-personnel support with moderate structural pressure, attackRange 170, antiUnitDamageMultiplier 1.2, structuralDamageMultiplier 1.6, trained at siege workshop)
  - added `mantlet` (stage 2, siege-support, mobile cover with coverRadius 92 and coverInboundRangedMultiplier 0.55, no attack, trained at siege workshop)
- `data/buildings.json`
  - `siege_workshop.trainableUnits` extended to include `ballista` and `mantlet`
- `src/game/core/simulation.js`
  - adjusted `getFactionSiegeUnits` default filter from `Boolean(siegeClass)` to `role === "siege-engine"` so siege-support units like mantlet are not counted as attacking engines
  - refactored `getFactionEngineerUnits` and `getFactionSupplyWagons` to filter directly by role
  - added `getFactionMantlets` helper
  - added `getIncomingRangedCoverMultiplier` helper that applies mantlet cover to friendly units
  - extended `updateProjectiles` to apply mantlet cover on inbound ranged damage against unit targets
  - extended `getRealmConditionSnapshot` with `cycle`, `faith`, `conviction`, `logistics`, and `worldPressure` blocks plus `band` fields on fortification and military, producing the full canonical 11 pressure states
- `src/game/core/renderer.js`
  - added distinct silhouette rendering for `ballista` (crossed rail, projecting bolt channel, low wheels)
  - added distinct silhouette rendering for `mantlet` (shield-wall slab, cover aura, reinforcement slats, low wheels)
- `src/game/core/ai.js`
  - extended Stonehelm siege-line queue order: trebuchet → engineer → supply wagon → mantlet → ballista → ram → siege tower
  - refined `hasSiegeUnit` to only count role === "siege-engine" so mantlet alone does not satisfy keep-assault gating
- `src/game/main.js`
  - rewrote `renderRealmConditionBar` to emit 11 pressure pills aligned with the canonical state order: Cycle, Pop, Food, Water, Loyalty, Fort, Army, Faith, Conviction, Logistics, World
- `src/game/styles.css`
  - reduced `.pressure-pill` min-width from 132px to 116px to accommodate 11 pills at standard viewport widths
- `tests/data-validation.mjs`
  - added assertions for ballista schema (role, siege class, attack range, anti-unit multiplier, trainability)
  - added assertions for mantlet schema (role, siege class, cover radius, cover reduction multiplier, zero attack, trainability)
- `tests/runtime-bridge.mjs`
  - added mantlet cover reduction assertion (bowman against exposed militia vs mantlet-covered militia)
  - added ballista trainable-at-workshop assertion
  - added 11-state snapshot assertions (cycle, population, food, water, loyalty, fortification, military, faith, conviction, logistics, worldPressure blocks present with canonical band colors)

### Verification

- `node tests/data-validation.mjs` - passed
- `node tests/runtime-bridge.mjs` - passed
- `node --check src/game/main.js` - passed
- `node --check src/game/core/simulation.js` - passed
- `node --check src/game/core/renderer.js` - passed
- `node --check src/game/core/ai.js` - passed
- `node --check src/game/core/data-loader.js` - passed
- `node --check src/game/core/utils.js` - passed
- Live browser boot on `http://localhost:8057/play.html`: launch scene transitions to game shell, 11 pressure pills render correctly with canonical labels (Cycle, Pop, Food, Water, Loyalty, Fort, Army, Faith, Conviction, Logistics, World), zero console errors, zero failed network requests, dynasty and faith panels populate, debug overlay matches new snapshot fields.

### Preservation Statement

No canonical system was reduced, substituted, or sidelined. All nine founding houses, four faiths, six vessel classes, continental world architecture, five-stage match structure, dual-clock architecture, Decision 21 fortification doctrine, and every other preserved pillar remain protected. Session 9 moved items from PARTIAL / DOCUMENTED toward LIVE. Nothing regressed.

---

## 2026-04-14 (Session 7): Dynastic Rescue + Ransom + Captivity Operations

### Scope

Advanced the browser/spec lane from passive captivity consequence into live dynastic action. Captured members can now be negotiated home, extracted through covert rescue, or released by the captor through direct ransom demand. The dynasty panel now exposes those decisions and operation progress directly.

### Runtime Surface Updates

- `src/game/core/simulation.js`
  - added `dynasty.operations` state with active and history tracks
  - added negotiated ransom operations with escrowed cost and timed resolution
  - added covert rescue operations with deterministic success pressure from spymaster / envoy / commander support against captive value, fortification depth, active ward, and captor covert pressure
  - added captor-side ransom demand resolution
  - extended faction snapshots so the dynasty panel can read captivity options, active operations, and held-captive demand terms
- `src/game/main.js`
  - extended the dynasty panel with active-dynasty-operation readouts
  - added `Negotiate Release` and `Send Rescue Cell` actions for captured bloodline members
  - added `Demand Ransom` actions for held captives
- `tests/runtime-bridge.mjs`
  - added negotiated-ransom recovery coverage
  - added covert-rescue recovery coverage
  - added captor-side ransom-demand recovery coverage

### Verification

- `node tests/data-validation.mjs` - passed
- `node tests/runtime-bridge.mjs` - passed
- `node --check src/game/main.js` - passed
- `node --check src/game/core/simulation.js` - passed
- `node --check src/game/core/renderer.js` - passed
- `node --check src/game/core/ai.js` - passed
- `node --check src/game/core/data-loader.js` - passed
- `node --check src/game/core/utils.js` - passed

### Continuity Updates

- Updated `CURRENT_PROJECT_STATE.md`
- Updated `NEXT_SESSION_HANDOFF.md`
- Updated `continuity/PROJECT_STATE.json`
- Updated `tasks/todo.md`
- Added `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_7.md`
- Added `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_8.md`

## 2026-04-14 (Session 6): Siege Workshop + Additional Engines + Attacker Preparation

### Scope

Advanced the browser/spec lane from first-engine siege support into real attacker preparation: dedicated workshop infrastructure, differentiated siege-engine roles, and Stonehelm AI that can prepare and stage a siege instead of only refusing an underforce assault.

### Runtime Surface Updates

- `data/buildings.json`
  - added `siege_workshop` as dedicated siege-production infrastructure
  - removed Ram training from Barracks
- `data/units.json`
  - added `siege_tower`
  - added `trebuchet`
- `src/game/core/simulation.js`
  - added Siege Tower allied-assault support so nearby friendly attackers gain better wall pressure and assault reach
- `src/game/core/renderer.js`
  - added distinct draw cases for `siege_workshop`, `siege_tower`, and `trebuchet`
- `src/game/main.js`
  - added `siege_workshop` to the worker build palette
- `src/game/core/ai.js`
  - extended Stonehelm AI into real siege preparation:
    - quarry + iron mine + siege workshop buildout
    - workshop engine production
    - siege-line staging before the keep assault

### Validation Updates

- `tests/data-validation.mjs`
  - added workshop-schema and new-engine assertions
  - asserted Ram production now routes through the workshop instead of Barracks
- `tests/runtime-bridge.mjs`
  - added trebuchet wall-pressure coverage
  - added Siege Tower support-assault coverage
  - added AI siege-infrastructure buildout coverage
  - added workshop engine-queue coverage
  - added staged siege-line commitment coverage

### Verification

- `node tests/data-validation.mjs` — passed
- `node tests/runtime-bridge.mjs` — passed
- `node --check src/game/main.js` — passed
- `node --check src/game/core/simulation.js` — passed
- `node --check src/game/core/renderer.js` — passed
- `node --check src/game/core/ai.js` — passed

### Continuity Updates

- Updated `CURRENT_PROJECT_STATE.md`
- Updated `NEXT_SESSION_HANDOFF.md`
- Updated `continuity/PROJECT_STATE.json`
- Updated `tasks/todo.md`
- Added `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_6.md`
- Added `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_7.md`

## 2026-04-14 (Session 3): Browser Renderer + HUD + AI Legibility Wave

### Scope

Completed the browser/spec lane follow-through that the prior session intentionally left pending: renderer silhouettes, HUD surfacing, command-panel coverage, AI keep-assault refusal, and end-to-end validation for the new fortification/siege/population systems.

### Runtime Surface Updates

- `play.html`
  - added live markup for the realm-condition HUD bar directly under the header
  - updated the launch-card slice summary so it reflects stone, iron, fortifications, and battering rams
- `src/game/styles.css`
  - added styling for the realm-condition bar, pressure pills, cycle meta, and disabled action buttons
  - tightened resource-pill sizing so the 10-pill bar still fits cleanly
- `src/game/main.js`
  - wired the HUD to `getRealmConditionSnapshot`
  - added the 6-pill realm-condition bar (population, food, water, loyalty, fortification, army)
  - expanded the worker build palette to include `quarry`, `iron_mine`, `wall_segment`, `watch_tower`, `gatehouse`, and `keep_tier_1`
  - added affordability gating for build/train buttons so the command panel communicates resource pressure more clearly
  - extended debug output to include stone, iron, and realm-condition status
- `src/game/core/renderer.js`
  - added distinct draw cases for stone nodes, iron nodes, wall segments, watch towers, gatehouses, inner keeps, and Rams
  - extended control-point rendering with settlement-class plus fortification-tier labels
  - expanded the resource bar payload to 10 pills: gold, food, water, wood, stone, iron, influence, available pop, total pop, territory
- `src/game/core/ai.js`
  - added fortified-keep refusal logic so Stonehelm will not issue a direct hall assault when the target primary seat is fortified and no siege-class unit is present

### Validation Updates

- `tests/data-validation.mjs`
  - added assertions for enabled primary resources, fortification-role schemas, ram schema, settlement classes, and realm conditions
- `tests/runtime-bridge.mjs`
  - added fortification-tier advancement test
  - added smelting stall/success test
  - added ram-vs-wall damage comparison
  - added famine trigger test under starvation conditions
  - added AI keep-assault refusal / approval tests

### Verification

- `node tests/data-validation.mjs` — passed
- `node tests/runtime-bridge.mjs` — passed
- `node --check src/game/main.js` — passed
- `node --check src/game/core/renderer.js` — passed
- `node --check src/game/core/ai.js` — passed
- `node --check tests/data-validation.mjs` — passed
- `node --check tests/runtime-bridge.mjs` — passed

### Continuity Updates

- Updated `CURRENT_PROJECT_STATE.md`
- Updated `NEXT_SESSION_HANDOFF.md`
- Updated `continuity/PROJECT_STATE.json`
- Updated `tasks/todo.md`
- Added `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_4.md`

### Open Blocker

Unity version alignment remains unresolved. ECS code is still blocked on user direction for 6.3 LTS downgrade vs 6.4 acceptance vs dual-editor.

## 2026-04-14 (Session 2): Browser Runtime Extension + Unity Environment Alignment

### Scope

Two coordinated waves in one session: (a) browser runtime expansion to close the most load-bearing canon-vs-runtime gaps, and (b) Unity production project discovery, alignment, and structural normalization to the approved Unity 6.3 LTS + DOTS/ECS architecture.

### Wave A — Browser Runtime Expansion (Canonical Fortification + Siege + Realm Cycle Foundation)

**Data layer additions (all preserved-additive, existing fields unchanged):**

- `data/resources.json` — stone, iron, influence flipped to `enabledInPrototype: true`
- `data/maps/ironmark-frontier.json` — 4 new stone nodes, 2 new iron nodes; control points now carry canonical `settlementClass`; new `settlements` array seeds primary dynastic keeps for player and enemy
- `data/buildings.json` — added `quarry`, `iron_mine` (with `smeltingFuelResource: wood` + `smeltingFuelRatio: 0.5`), `wall_segment`, `watch_tower`, `gatehouse`, `keep_tier_1` (full fortification building class with `fortificationRole`, `fortificationTierContribution`, `structuralDamageMultiplier`, `armor`, `blocksPassage`, `sightBonusRadius`, `auraAttackMultiplier`, `auraRadius`). Barracks trainable list now includes `ram`.
- `data/units.json` — added `ram` (first siege engine, Stage 2, `siegeClass: ram`, `structuralDamageMultiplier: 3.5`, `antiUnitDamageMultiplier: 0.4`, pop cost 2)
- `data/settlement-classes.json` — NEW file with 6 canonical classes (border_settlement, military_fort, trade_town, regional_stronghold, primary_dynastic_keep, fortress_citadel) and per-class defensive ceiling
- `data/realm-conditions.json` — NEW file with 90-second cycle, famine/water-crisis/cap-pressure thresholds, legibility bands for food/water/population/loyalty

**Simulation layer additions (`src/game/core/simulation.js`, all additive, step dispatch extended):**

- New constants block: `FORTIFICATION_ECOSYSTEM_RADIUS_TILES`, `ASSAULT_STRAIN_THRESHOLD`, `ASSAULT_STRAIN_DECAY_PER_SECOND`, `ASSAULT_COHESION_PENALTY_DURATION`, `ASSAULT_COHESION_PENALTY_MULTIPLIER`, `FORTIFICATION_AURA_RADIUS_TILES`, `REALM_CYCLE_DEFAULT_SECONDS`
- `applyDamage` extended to accept `attackerContext` and apply canonical structural + anti-unit multipliers (wall damage math: infantry × 0.2; ram × 0.7; ram-vs-unit × 0.4)
- Melee + projectile attack paths thread `attackerContext` with unit's structural/anti-unit multipliers
- `isUnitNearHostileFortification` + assault failure strain accrual on unit death near hostile fortification buildings
- `getAssaultCohesionMultiplier` + `tickAssaultCohesion` — wave-spam denial math per 2026-04-14 doctrine
- Worker dropoff path extended for smelting fuel consumption at `iron_mine` (returns ore to node + stalls worker if fuel insufficient)
- Building completion path now calls `advanceFortificationTier` for buildings with `fortificationRole`, capped at settlement's canonical `defensiveCeiling`
- World state includes `settlements` array plus `fortificationTier` on each control point
- `tickRealmConditionCycle` — canonical 90-second heartbeat with famine detection (2 consecutive cycles), water crisis (1 cycle, triggers outmigration), cap pressure
- `applyFamine`, `applyWaterCrisis`, `applyCapPressure` helpers with canonical effects (population decline, loyalty erosion, conviction events)
- `getRealmConditionSnapshot` — exported for UI surfacing; returns the 11-state legibility snapshot (population band, food band, water band, loyalty band, fortification band, military band, assault strain, cycle progress)
- `stepSimulation` dispatch extended with `tickAssaultCohesion` + `tickRealmConditionCycle`

**Data loader:** `src/game/core/data-loader.js` — added `settlementClasses` and `realmConditions` to the load set + byId index.

**Tests:** `node tests/data-validation.mjs` and `node tests/runtime-bridge.mjs` remain GREEN after all changes. New assertions for the additions will be added in the next wave (renderer + UI completion).

**Plan document:** `docs/plans/2026-04-14-fortification-siege-population-legibility-wave.md` written with 8-task breakdown covering data + simulation (done this session) and renderer + UI + test expansion (next session).

### Wave B — Unity Environment Alignment

**Machine discovery:**

- Unity Hub installed; two Unity editors present: **6000.3.13f1 (Unity 6.3 LTS, approved)** and **6000.4.2f1 (Unity 6.4)**
- Visual Studio 18 Community at `C:\Program Files\Microsoft Visual Studio\18\Community\` (VS 2022 legacy artifacts in x86 path but no full install)
- VS Code installed; Blender 5.1 installed; Git 2.46.0; GitHub Desktop installed; .NET SDK 10.0.201 + 6/7/8/10 runtimes
- Perforce, JetBrains Rider, and Wwise NOT installed (all acceptable per directive)

**Project discovery:**

- `<repo>/unity/` confirmed as the TRUE canonical Unity project: full DOTS/ECS stack (Entities 6.4.0, Burst 1.8.29, Collections 6.4.0, Mathematics 1.3.3, Entities.Graphics 6.4.0), URP 17.4.0, Input System 1.19.0, Addressables 2.9.1
- `<repo>/Bloodlines/` identified as a fresh URP 3D template stub — NO DOTS packages, no Bloodlines content, preserved with `STUB_TEMPLATE_NOTICE.md` per preservation mandate (not deleted)
- Existing Unity content: `Code/Definitions/BloodlinesDefinitions.cs` (full ScriptableObject definitions for Houses, Resources, Units, Buildings, Faiths, Conviction, Roles, Paths, Victory, Maps) and `Code/Editor/JsonContentImporter.cs` (JSON → ScriptableObject bridge reading from `<repo>/data/`)

**Structural normalization (all additive; no existing content touched):**

Under `<repo>/unity/Assets/_Bloodlines/` created the full approved baseline tree:

- `Art/` Characters, Units, Buildings, Environment, Terrain, FX, UI, Icons, Concepts
- `Audio/` Music, SFX, Voice, Middleware
- `Code/` new subfolders: Aspects, Authoring, Baking, AI, Pathing, Economy, Population, Faith, Dynasties, Combat, Construction, Camera, Input, Networking, UI, SaveLoad, Utilities, Debug (preserving existing Components, Systems, Definitions, Editor)
- `Data/` new subfolders: ScriptableDefinitions, FactionDefinitions, UnitDefinitions, BuildingDefinitions, TerrainDefinitions, TechDefinitions, AudioDefinitions, DynastyDefinitions, FaithDefinitions, ResourceDefinitions, ConvictionDefinitions, BloodlineRoleDefinitions, BloodlinePathDefinitions, VictoryConditionDefinitions, MapDefinitions, SettlementClassDefinitions, RealmConditionDefinitions
- `Prefabs/` Units, Buildings, Environment, UI
- `Scenes/` Bootstrap, Sandbox, Gameplay, Testbeds, Strategic
- `Materials/`, `Shaders/`, `Animation/`
- `Docs/` Technical, Gameplay, Systems, Decisions, UIUX, Continuity

**Documentation seeded:**

- `<repo>/unity/Assets/_Bloodlines/README.md` — master asset-tree contract + direction-of-play reminders
- `<repo>/unity/Assets/_Bloodlines/Code/README.md` — per-folder code responsibilities + namespace conventions
- `<repo>/unity/Assets/_Bloodlines/Data/README.md` — JSON sync direction + per-folder data sources
- `<repo>/unity/README.md` — rewritten to reflect canonical root governance, approved toolchain, reference simulation location, non-negotiable direction of play. (Old statement pointing at `deploy\bloodlines\` preserved historically but superseded.)

**ScriptableObject definitions extended** (`Code/Definitions/BloodlinesDefinitions.cs`):

- `UnitDefinition`: added `siegeClass`, `structuralDamageMultiplier`, `antiUnitDamageMultiplier`
- `BuildingDefinition`: added `fortificationRole`, `fortificationTierContribution`, `structuralDamageMultiplier`, `armor`, `blocksPassage`, `sightBonusRadius`, `auraAttackMultiplier`, `auraRadius`, `smeltingFuelResource`, `smeltingFuelRatio`
- `ControlPointData`: added `settlementClass`
- NEW: `SettlementSeedData`, `SettlementClassDefinition`, `RealmConditionDefinition`, `RealmConditionThresholdsData`, `RealmConditionFamineData`, `RealmConditionWaterCrisisData`, `RealmConditionCapPressureData`, `RealmConditionEffectsData`, `RealmConditionLegibilityData`
- `MapDefinition`: added `settlements` array

**JSON importer extended** (`Code/Editor/JsonContentImporter.cs`):

- Imports `settlement-classes.json` → `SettlementClassDefinitions/`
- Imports `realm-conditions.json` → `RealmConditionDefinitions/realm_conditions.asset`
- Populates new fortification/siege/smelting fields on Unit/Building definitions
- Populates `settlementClass` on ControlPoint and `settlements` array on Map

**Environment report:** `ENVIRONMENT_REPORT_2026-04-14.md` created at repo root with:

- Full toolchain inventory
- Two-folder project identification
- Package alignment table
- Unity 6.3 LTS vs 6.4 version alignment decision point (awaits user direction)
- Next-execution targets for ECS foundation

### Open Decision

**Unity version alignment:** installed editors are 6.3 LTS (`6000.3.13f1`) and 6.4 (`6000.4.2f1`); the `unity/` project currently targets 6.4. Approved architecture calls for 6.3 LTS. User direction requested before writing ECS code (downgrade vs accept 6.4 vs dual-editor).

### Non-Destructive Guarantee

- Nothing deleted.
- `Bloodlines/` stub folder preserved in place with `STUB_TEMPLATE_NOTICE.md`.
- Every existing `.cs` file extended additively; all prior fields retained.
- Every existing JSON schema extended additively; all prior fields retained.
- Stale `deploy\bloodlines\` references in old Unity README rewritten; the historical statement remains documented in the rewrite as "preserved historically but superseded by 2026-04-14 governance".
- Tests remain GREEN.

---

## 2026-03-15: Documentation Consolidation (Session: Central Guide Creation)

### Scope
Created a centralized documentation set within `docs/` to consolidate the design state of the Bloodlines project across all existing files into a navigable, cross-referenced system.

### Files Reviewed During Consolidation

The following files were read and analyzed to inform the consolidated documents:

**Canon Files:**
- `01_CANON/BLOODLINES_MASTER_MEMORY.md` (50 sections, ~900 lines, richest single file)
- `01_CANON/BLOODLINES_DESIGN_BIBLE.md` (15 sections, fully populated)
- `01_CANON/CANONICAL_RULES.md` (40+ settled/proposed/open decisions)
- `01_CANON/BLOODLINES_APPEND_ONLY_LOG.md` (4 ingestion records with full raw content)
- `01_CANON/BLOODLINES_STRUCTURE_INDEX.md` (cross-reference index)
- `01_CANON/BLOODLINES_SESSION_BOOT_PROMPT.md` (referenced, not read in full)

**Admin Files:**
- `00_ADMIN/PROJECT_STATUS.md` (Phase 1 status, update log)
- `00_ADMIN/WORKFLOW_RULES.md` (additive archival rules, session workflow)
- `00_ADMIN/DIRECTORY_MAP.md` (referenced)

**System Files (all 7 read in full):**
- `04_SYSTEMS/SYSTEM_INDEX.md`
- `04_SYSTEMS/CONVICTION_SYSTEM.md`
- `04_SYSTEMS/FAITH_SYSTEM.md`
- `04_SYSTEMS/POPULATION_SYSTEM.md`
- `04_SYSTEMS/RESOURCE_SYSTEM.md`
- `04_SYSTEMS/TERRITORY_SYSTEM.md`
- `04_SYSTEMS/DYNASTIC_SYSTEM.md`
- `04_SYSTEMS/BORN_OF_SACRIFICE_SYSTEM.md`

**Content Files:**
- `06_FACTIONS/FOUNDING_HOUSES.md` (9 houses, naming history, visual identity)
- `07_FAITHS/FOUR_ANCIENT_FAITHS.md` (4 covenants with mechanics)
- `10_UNITS/UNIT_INDEX.md` (Level 1 units, army composition, Born of Sacrifice units)
- `11_MATCHFLOW/MATCH_STRUCTURE.md` (4 stages, victory conditions, pacing)

**Session Ingestions:**
- `02_SESSION_INGESTIONS/SESSION_2026-03-15_first-substantive-ingestion.md`

**Scaffold-Only Files (reviewed, confirmed empty):**
- `05_LORE/WORLD_HISTORY.md` - Awaiting content
- `05_LORE/TIMELINE.md` - Awaiting content
- `12_UI_UX/UI_NOTES.md` - Awaiting content
- `13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md` - Awaiting content
- `08_MECHANICS/MECHANICS_INDEX.md` - Index only
- `09_WORLD/WORLD_INDEX.md` - Index only

**Other:**
- `17_TASKS/TASK_BACKLOG.md` (partially stale, some items completed but not checked off)
- `CLAUDE.md` (project context for Claude Code sessions)
- `HANDOFF.md` (previous session handoff)
- `README.md` (project overview)

### Files Created

| File | Purpose | Size |
|------|---------|------|
| `docs/USER_GUIDE.md` | Central consolidated design overview, 29 sections | ~800 lines |
| `docs/CURRENT_GAME_STATE.md` | Status checkpoint showing what is defined vs open | ~150 lines |
| `docs/NEXT_STEPS.md` | Prioritized action plan with 10 next steps | ~200 lines |
| `docs/INPUT_WORKBOOK.md` | Structured design questions across 14 sections | ~400 lines |
| `docs/OPEN_QUESTIONS.md` | Full catalog of unresolved design questions | ~200 lines |
| `docs/CHANGE_LOG.md` | This file | -- |
| `docs/INPUT_TO_APPLY.md` | Inbox for future raw design input | ~50 lines |

### Design Material Centralized

The USER_GUIDE.md consolidates the following content that was previously distributed across multiple files:
- Project identity and philosophy (from Master Memory Sections 1-2, Design Bible Section 1)
- Founding houses (from Founding Houses file, Master Memory Sections 42-44)
- Four faiths (from Four Ancient Faiths file, Faith System file, Master Memory Sections 23-26)
- Conviction (from Conviction System file, Master Memory Section 27)
- Population model (from Population System file, Master Memory Sections 10-11)
- Resources (from Resource System file, Master Memory Section 8)
- Territory (from Territory System file, Master Memory Sections 12-13)
- Dynasty (from Dynastic System file, Master Memory Sections 14-22)
- Born of Sacrifice (from Born of Sacrifice System file, Master Memory Section 41)
- Military (from Unit Index, Master Memory army sections)
- Buildings (from Master Memory building sections)
- Match structure (from Match Structure file, Master Memory Section 4)
- Victory conditions (from Match Structure file, Master Memory Sections 32-37)
- Trueborn city (from Master Memory Sections 30-31)
- System interconnections (newly synthesized from all system files)

### What Remains Fragmented or Unresolved

1. **Task Backlog** (`17_TASKS/TASK_BACKLOG.md`) is partially stale. Several items listed as TODO were completed during the first substantive ingestion. Recommend updating.
2. **Structure Index** (`01_CANON/BLOODLINES_STRUCTURE_INDEX.md`) Section 42 still references "Eight founding houses" though the count was resolved to nine. The structure index could be updated.
3. **Resource count discrepancy:** The instructions reference "four primary resources (gold, food, water, stone)" but the actual canonical design has five: gold, food, water, wood, and stone. Wood was added during Session 2. The five-resource model is canonical.
4. **World History and Timeline** remain completely empty. These need creative input before they can be populated.
5. **UI/UX and Audio/Visual** remain completely empty. These are downstream of gameplay design decisions.

---

*Future change log entries are appended below this line.*

---

## 2026-04-14: Dynasty Consequence Cascade + Defensive Fortification Doctrine

### Scope
Session 14 landed two major workstreams additively:

1. **Dynasty Consequence Cascade** — first runtime expansion that makes the bloodline roster consequential in live simulation. Commander capture versus kill resolution, captive ledger, role succession cascade, governor loss on territory flip, ransom influence trickle, conviction ledger integration, UI surfaces for captives and fallen, extended runtime-bridge test coverage.

2. **Defensive Fortification Doctrine** — canonical strategic pillar locked by Lance W. Fisher direction. Full doctrine at `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`. Defender specification at `04_SYSTEMS/FORTIFICATION_SYSTEM.md`. Attacker specification at `04_SYSTEMS/SIEGE_SYSTEM.md`. Design-bible integration at Sections 82-85 in `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md` (bible bumped v3.2 → v3.3; canonical desktop copy at `D:/Lance/Desktop/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md`).

### Files Created

- `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`
- `04_SYSTEMS/FORTIFICATION_SYSTEM.md`
- `04_SYSTEMS/SIEGE_SYSTEM.md`
- `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md`
- `D:/Lance/Desktop/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md` (desktop copy)

### Files Extended (all additive)

- `01_CANON/CANONICAL_RULES.md` — Fourteenth Session Canon section with fortification lock entries
- `01_CANON/CANON_LOCK.md` — Defensive Fortification section with lock entries
- `04_SYSTEMS/SYSTEM_INDEX.md` — Fortification and Siege systems added to core systems table
- `04_SYSTEMS/TERRITORY_SYSTEM.md` — fortification doctrine integration addendum
- `04_SYSTEMS/DYNASTIC_SYSTEM.md` — keep-as-bloodline-seat integration addendum
- `08_MECHANICS/MECHANICS_INDEX.md` — fortification and siege mechanics area
- `11_MATCHFLOW/MATCH_STRUCTURE.md` — fortress pacing across the five stages
- `10_UNITS/UNIT_INDEX.md` — fortification and siege unit class architecture
- `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md` — 2026-04-14 integration addendum
- `docs/DEFINITIVE_DECISIONS_REGISTER.md` — Decision 21 (Defensive Fortification Doctrine Lock)
- `docs/unity/PHASE_PLAN.md` — U17-U22 fortification and siege milestones
- `docs/unity/SYSTEM_MAP.md` — Fortification and siege ECS system entries
- `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md` — appended Part XVI (Sections 82-85); internal version bumped to v3.3

### Runtime Code Changes

- `data/faiths.json` — doctrine effect blocks (light/dark) for all four faiths
- `src/game/core/simulation.js` — doctrine lookup, conviction ledger (4 buckets), dynasty commander and governor attachments, captives and fallen ledgers, succession cascade, handleCommanderFall, handleGovernorLoss, transferMemberToCaptor, runSuccessionCascade, backfillHeir, updateCaptiveRansomTrickle, restoreDisplacedMembers, finalizeUnitDeaths, finalizeBuildingDeaths, Ironmark blood levy logic, getFactionSnapshot extended to expose captives/fallen/heir/interregnum
- `src/game/main.js` — DOM-safe renderDynastyPanel rebuild showing heir line, captives, fallen ledger; debug overlay extended with Dynasty line
- `src/game/core/renderer.js` — commander aura ring, commander title, occupied/stabilized control point state, governor marker
- `tests/data-validation.mjs` — extended with doctrine-shape assertions
- `tests/runtime-bridge.mjs` — new test; validates commander attachment, doctrine commitment conviction events, capture path, captive ledger, ransom influence trickle, head-of-bloodline succession

### Verification

- `node tests/data-validation.mjs` — passed
- `node tests/runtime-bridge.mjs` — passed
- `node --check` on simulation.js, main.js, renderer.js — passed
- Live browser runtime at http://localhost:8057/play.html — no console errors, no failed network requests, dynasty panel and debug overlay render new state correctly, commander attached to field unit on boot

### Canonical Notes

- The Defensive Fortification Doctrine is NON-NEGOTIABLE per Lance W. Fisher direction. Its ten pillars bind the design bible, combat systems, stronghold systems, AI planning, siege logic, economy tradeoffs, match flow, and Unity implementation plans.
- All changes are additive. No content was deleted or summarized. Prior canon layers remain intact.
- The bible version bump (v3.2 → v3.3) preserves the prior v3.2 file; both v3.2 and v3.3 now exist in `18_EXPORTS/` with the v3.2 file carrying the new content internally for backward-compatible references, and the v3.3 file as the named reference going forward.

### Handoff

Next session should pick up with (a) implementation debt tracking for Fortification Doctrine phases U17-U22 and browser-runtime equivalents; (b) ransom / rescue operations layer building on the 2026-04-14 captive ledger; (c) AI siege awareness development; (d) one of the priority items from Section 11.5 of `docs/BLOODLINES_CURRENT_STATE_ANALYSIS.md`.

---

## 2026-04-14: Preservation-First Canonical Root Consolidation

### Scope

This session completed a preservation-first continuity pass so future work can continue from one canonical Bloodlines root without reconstructing fragmented history from the wider workspace.

### Canonical Root Decision

- Canonical session path: `D:\ProjectsHome\Bloodlines`
- Physical backing path today: `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- Compatibility copy preserved but non-canonical: `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines`

### Sources Imported Into The Canonical Root

- `_archive\2026-03-22\bloodlines game`
- `_archive\2026-04-13\bloodlines-single-root\external-repo-preconsolidation`
- `_archive\2026-04-13\bloodlines-single-root\repo-root-mirror-preconsolidation`
- `FisherSovereign\lancewfisher-v2\deploy\bloodlines`
- `frontier-bastion`
- Bloodlines-specific parent-site web surfaces
- workspace Bloodlines governance overlay and reviewer agent

### New Files Added

- `AGENTS.md`
- `MASTER_PROJECT_INDEX.md`
- `SOURCE_PROVENANCE_MAP.md`
- `MASTER_BLOODLINES_CONTEXT.md`
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `CONSOLIDATION_REPORT.md`
- `continuity/SESSION_CONTINUITY.md`
- `continuity/CURRENT_STATUS_AND_NEXT_STEPS.md`
- `continuity/PROJECT_STATE.json`
- `reports/2026-04-14_source_audit.md`

### Files Updated

- `README.md`
- `CLAUDE.md`
- `.claude/settings.json`
- `.claude/launch.json`
- `tasks/todo.md`

### Snapshot Preservation Before Governance Refresh

- `19_ARCHIVE/PRE_2026-04-14_GOVERNANCE_REFRESH/README_snapshot_2026-04-14.md`
- `19_ARCHIVE/PRE_2026-04-14_GOVERNANCE_REFRESH/CLAUDE_snapshot_2026-04-14.md`
- `19_ARCHIVE/PRE_2026-04-14_GOVERNANCE_REFRESH/settings_snapshot_2026-04-14.json`
- `19_ARCHIVE/PRE_2026-04-14_GOVERNANCE_REFRESH/launch_snapshot_2026-04-14.json`

### Notes

- No confirmed Bloodlines-related content was intentionally deleted.
- The project now carries its preserved outside source roots and continuity instructions inside the canonical root.
- Future sessions should enter `D:\ProjectsHome\Bloodlines` and treat that path as the one authoritative Bloodlines folder.
- After the initial preservation import, a direct deploy-to-root reconciliation also landed:
  - `88` deploy-only files were copied into the active canonical working tree.
  - deploy's newer `index.html` was promoted into the canonical root after backing up the prior root version.
  - the deploy-side single-root note was preserved as a sidecar variant instead of overwriting the richer in-root note.

---

## 2026-04-14: External Master Design Doctrine Ingestion

### Scope

Absorbed two externally supplied Bloodlines doctrine DOCX files into the canonical root in full preservation mode, integrated their content into canon and continuity, and advanced the curated design bible to v3.4.

### Source Artifacts

- `D:\Lance\Downloads\Bloodlines_Master_Design_Doctrine.docx`
- `D:\Lance\Downloads\Bloodlines_Master_Design_Doctrine2.docx`

### Preservation Actions

- both DOCX files copied into `archive_preserved_sources/2026-04-14_downloads_bloodlines_design_doctrine_docx/`
- both packages exploded and text-extracted under `02_SESSION_INGESTIONS/2026-04-14_design_doctrine_docx_ingestion/`
- combined raw appendix created
- canonical readable doctrine source created at `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`

### Canon and Continuity Surfaces Added

- `docs/BLOODLINES_CANON_CONFLICT_NOTES_2026-04-14_DOCTRINE_INGESTION.md`
- `docs/BLOODLINES_SYSTEM_CROSSWALK_2026-04-14_DOCTRINE_INGESTION.md`
- `04_SYSTEMS/MASTER_DOCTRINE_SYSTEM_INTEGRATION_2026-04-14.md`
- `11_MATCHFLOW/MASTER_DOCTRINE_MATCHFLOW_INTEGRATION_2026-04-14.md`
- `06_FACTIONS/HOUSE_IDENTITY_DOCTRINE_ADDENDUM_2026-04-14.md`
- `09_WORLD/WORLD_SCALE_DOCTRINE_ADDENDUM_2026-04-14.md`
- `12_UI_UX/MASTER_DOCTRINE_UI_INTEGRATION_2026-04-14.md`
- `13_AUDIO_VISUAL/MASTER_DOCTRINE_AUDIO_VISUAL_INTEGRATION_2026-04-14.md`
- `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`
- `D:\Lance\Desktop\BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.4.md`

### Canon Files Updated

- `01_CANON/BLOODLINES_DESIGN_BIBLE.md`
- `01_CANON/BLOODLINES_MASTER_MEMORY.md`
- `01_CANON/BLOODLINES_APPEND_ONLY_LOG.md`
- `01_CANON/CANONICAL_RULES.md`
- `01_CANON/CANON_LOCK.md`

### Continuity Files Updated

- `MASTER_PROJECT_INDEX.md`
- `MASTER_BLOODLINES_CONTEXT.md`
- `CURRENT_PROJECT_STATE.md`
- `NEXT_SESSION_HANDOFF.md`
- `SOURCE_PROVENANCE_MAP.md`
- `CONSOLIDATION_REPORT.md`

### Export Refresh

- pre-rebuild snapshots stored in `19_ARCHIVE/PRE_2026-04-14_DOCTRINE_MASTER_REBUILD/`
- `18_EXPORTS/BLOODLINES_COMPLETE_BIBLE_ALL_v2.0_2026-04-14.md` rebuilt
- `18_EXPORTS/BLOODLINES_COMPLETE_MASTER_2026-04-14.md` rebuilt
- desktop copies of rebuilt exports refreshed

### Canonical Notes

- the two incoming DOCX files were byte-identical and preserved separately anyway
- no major canon contradiction was found
- the doctrine's "determined time" wording was mapped to the existing declared-time model rather than creating a competing time system
- no relevant information from the incoming doctrine set was intentionally excluded

---

## 2026-04-14: Session 4 Reserve Cycling And State Diagnosis

### Scope

Extended the browser/spec lane from fortification tiers into the first live defensive reserve ecosystem and wrote a current-state diagnosis that maps the repository against the full-scale canon.

### Browser Runtime Changes

- added fortified-settlement reserve cycling in `src/game/core/simulation.js`
- wounded defenders now fall back to fortified keeps for triage instead of simply dying in place
- healthier defenders at the keep now muster forward to threatened sections
- triage healing now occurs inside fortified seats
- primary-keep snapshot state now exposes ready reserves, recovering reserves, threat state, commander keep-presence, and bloodline-seat presence
- fortification HUD pill now surfaces that reserve state in `src/game/main.js`
- reserve-duty state is now visually marked in `src/game/core/renderer.js`

### Validation

- extended `tests/runtime-bridge.mjs` with reserve-cycling and fortification-snapshot assertions
- re-ran:
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - `node --check src/game/main.js`
  - `node --check src/game/core/simulation.js`
  - `node --check src/game/core/renderer.js`
  - `node --check src/game/core/ai.js`
  - `node --check src/game/core/data-loader.js`
  - `node --check src/game/core/utils.js`

### Documentation And Continuity

- added `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_4.md`
- updated `CURRENT_PROJECT_STATE.md`
- updated `NEXT_SESSION_HANDOFF.md`
- updated `continuity/PROJECT_STATE.json`
- updated `tasks/todo.md`
- added next-session prompt `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_5.md`

### Notes

- Unity ECS work remains blocked on the unresolved editor-version decision.
- The browser lane continues to function as the live systems-specification lane for the eventual Unity implementation.

---

## 2026-04-14: Session 5 Governor Specialization And Faith-Warded Keeps

### Scope

Extended the browser/spec lane so that territorial governance and covenant choice now materially affect fortified defense behavior.

### Browser Runtime Changes

- added anchor-aware governor rotation between control points and dynastic settlements
- added governor specialization profiles: `border`, `city`, `keep`
- extended territory stabilization, resource trickle, capture resistance, and loyalty protection with governor specialization logic
- added live fortification ward profiles for Old Light, Blood Dominion, Order, and Wild
- added Blood Dominion sacrificial reserve-surge behavior under active keep defense
- extended defensive combat around fortified seats with ward-driven sight, attack, and approach modifiers
- extended fortification HUD state, dynasty panel, and faith panel with governor specialization and ward readouts

### Validation

- extended `tests/runtime-bridge.mjs` with:
  - keep-governor default stewardship assertion
  - governor rotation from occupied march back to keep after stabilization
  - Old Light pyre-ward defensive sight assertion
- re-ran:
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - `node --check src/game/main.js`
  - `node --check src/game/core/simulation.js`
  - `node --check src/game/core/renderer.js`
  - `node --check src/game/core/ai.js`
  - `node --check src/game/core/data-loader.js`
  - `node --check src/game/core/utils.js`
  - `node --check tests/runtime-bridge.mjs`

### Documentation And Continuity

- added `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_5.md`
- updated `CURRENT_PROJECT_STATE.md`
- updated `NEXT_SESSION_HANDOFF.md`
- updated `continuity/PROJECT_STATE.json`
- updated `tasks/todo.md`
- added next-session prompt `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_6.md`

### Notes

- Unity ECS work remains blocked on the unresolved editor-version decision.
- The browser lane now carries a more authentic bridge between dynasty, covenant, territory administration, and keep defense.

---

## 2026-04-14: Session 8 Engineer Corps, Siege Supply Continuity, and Sustained Assault

### Scope

Extended the browser/spec lane so formal siege now depends on specialist labor and a vulnerable logistics chain rather than only on engine presence.

### Browser Runtime Changes

- added `supply_camp` as a forward logistics seat for formal siege lines
- added `siege_engineer` and `supply_wagon` to the live workshop roster
- added engineer-assisted breach support and in-line siege-engine repair
- added camp-and-wagon supply continuity for siege engines, including operational penalties when supply is cut
- extended the HUD/debug readout with live siege sustainment state
- extended Stonehelm AI with forward supply-camp buildout, engineer and wagon queueing, and refusal to commit a formal keep assault while the siege train remains unsupplied

### Validation

- extended `tests/data-validation.mjs` with supply-camp, engineer, and wagon schema assertions
- extended `tests/runtime-bridge.mjs` with:
  - supplied-vs-unsupplied ram pressure
  - engineer-assisted ram pressure
  - engineer repair throughput
  - AI supply-camp buildout
  - AI engineer and wagon queueing
  - AI unsupplied-assault delay
- re-ran:
  - `node tests/data-validation.mjs`
  - `node tests/runtime-bridge.mjs`
  - `node --check src/game/main.js`
  - `node --check src/game/core/simulation.js`
  - `node --check src/game/core/renderer.js`
  - `node --check src/game/core/ai.js`
  - `node --check src/game/core/data-loader.js`
  - `node --check src/game/core/utils.js`

### Documentation And Continuity

- added `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_8.md`
- updated `CURRENT_PROJECT_STATE.md`
- updated `NEXT_SESSION_HANDOFF.md`
- updated `continuity/PROJECT_STATE.json`
- updated `tasks/todo.md`
- added next-session prompt `03_PROMPTS/CONTINUATION_PROMPT_2026-04-14_SESSION_9.md`

### Notes

- Unity ECS work remains blocked on the unresolved editor-version decision.
- The browser/spec lane now treats siege as a sustained operational chain rather than a simple engine check.
