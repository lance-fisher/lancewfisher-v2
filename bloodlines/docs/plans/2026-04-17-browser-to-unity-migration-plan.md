# Browser-to-Unity Migration Plan

Date: 2026-04-17
Author: claude (session following Codex target-acquisition-los merge)
Status: initial plan pending operator approval
Supersedes: nothing; additive to existing Unity slice roadmap

## Context

Bloodlines has two implementation surfaces:

1. **Browser runtime** (`src/`, `data/`, `play.html`, `tests/`) — frozen as of the owner direction at `governance/OWNER_DIRECTION_2026-04-16_FULL_CANON_UNITY.md`. It is no longer a shipping target. It remains the single richest behavioral specification in the repo: `src/game/core/simulation.js` is 14,868 LOC of production-quality systemic gameplay, and `src/game/core/ai.js` is 3,141 LOC of full strategic AI.

2. **Unity 6.3 LTS DOTS/ECS** (`unity/Assets/_Bloodlines/Code/`) — the active shipping lane. ~92 C# files, ~20,012 LOC. Through Session 127 the first-shell battlefield loop is green: bootstrap + authoring, worker gather-deposit economy, construction + production progress, projectile combat, explicit attack orders, attack-move, target acquisition throttling with sight loss, realm condition starvation and cap pressure, first-shell enemy AI economic base, HUD legibility, and the graphics infrastructure foundation (URP + faction tint + placeholder meshes + governed validator).

Roughly **80 to 85 percent** of canonical gameplay specified in the design bible and realized in browser code has no ECS translation yet. This plan inventories that gap and orders the work into reviewable slices.

## Scope

This plan covers the migration of prior-session work (browser runtime, design ingestions, system canon, AI logic) into the Unity implementation. It does not replace the per-slice Unity handoff discipline already in effect; it slots prioritized migration slices into the same lane system.

## Audit Findings Summary

### What is already migrated

- **Data layer.** All 11 canonical JSON files in `data/*` (`buildings.json` 519 LOC, `units.json` 936 LOC, `faiths.json`, `houses.json`, `conviction-states.json`, `bloodline-paths.json`, `bloodline-roles.json`, `realm-conditions.json`, `resources.json`, `settlement-classes.json`, `victory-conditions.json`) have ScriptableObject equivalents under `unity/Assets/_Bloodlines/Code/Definitions/`. No gap here.
- **First-shell RTS loop.** Bootstrap, authoring, skirmish spawn, combat foundation, projectile, attack-order, attack-move, target-acquisition-los, movement, selection, construction, production, worker gather-deposit, population growth, cap pressure, starvation response, control point capture, graphics-infrastructure foundation. All green on master as of commit `dc00fff`.
- **Canon.** `01_CANON/BLOODLINES_DESIGN_BIBLE.md` and `04_SYSTEMS/*.md` are the consolidated spec for every subsystem. The 2026-04-14 doctrine addendum and earlier ingestions are rolled up into `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`. Player manual ingestion is rolled up into subsystem docs.

### What is documented in canon but has no Unity code

Subsystems spec'd in `04_SYSTEMS/*` with zero ECS implementation:

- Faith (covenants, exposure, intensity, covenant tests, holy wars, divine right)
- Dynasty (members, roles, aging, succession, claim strength, promotions, fallen ledger)
- Conviction (component exists; scoring and band effects are missing)
- Born of Sacrifice elite-unit pipeline
- Fortification tiers and layered defense
- Siege (engineer corps, supply wagons, field water discipline)
- Territorial governance, divine right recognition, non-aggression pacts
- Marriages, lesser-house defection, minor-house levies
- Covert operations (espionage, sabotage, assassination, counter-intel)
- Five-stage match progression and dual-clock declaration seam
- World pressure escalation, Great Reckoning, Trueborn rise arc
- Naval embark/disembark (bible section 18)
- Terrain and geography (bible section 22)
- Realm condition legibility beyond starvation and cap pressure
- Save/load snapshot and restore
- Scout raid, resource harass, logistics interdiction, convoy escort

### Migration-relevant source artifacts still unmigrated

- **`src/game/core/simulation.js`** (14,868 LOC). Authoritative behavioral spec. Every Unity slice below should cross-reference the corresponding function block. Do not read it as legacy code; read it as an executable specification.
- **`src/game/core/ai.js`** (3,141 LOC). The Unity AI is currently a narrow slice (economic base, construction, militia posture). This file defines the full strategic layer. A canonical AI port is a Tier 1 item.
- **`02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-time-system-multiplayer-doctrine*.md`** (3 files, 501 LOC). The five-stage structure, dual-clock, and imminent-engagement warning system. Locked as canon and implemented in browser, but Unity has zero presence of any of it. This is the single most important unmigrated doctrine.
- **`02_SESSION_INGESTIONS/2026-04-14_design_doctrine_docx_ingestion/`** — raw ingestion appendix, already canonized into `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md`. Preserve; no additional migration action.

### Migration noise (do not spend cycles on)

- **`03_PROMPTS/CONTINUATION_PROMPT_*.md`** — 51 files, all pure session-restart metadata. No gameplay content worth extracting.
- **`00_ADMIN/CURRENT_GAME_STATE.md`** — dated 2026-03-15, superseded by root `CURRENT_PROJECT_STATE.md`. Informational, preserve.
- **`03_PROMPTS/INPUT_TO_APPLY.md` and `INPUT_WORKBOOK.md`** — design Q&A already rolled into canon.

## Priority Buckets

### Tier 1 — blocks the first "Bloodlines" skirmish

These are the systems without which the current Unity build is a generic RTS rather than the canonical Bloodlines experience. Ship these before treating the first-shell slice as feature-complete.

1. **Dual-clock + five-stage match progression + declaration seam.** Reference: `simulation.js` `tickDualClock`, `updateMatchProgressionState`, `getMatchProgressionSnapshot`; doctrine at `02_SESSION_INGESTIONS/SESSION_2026-04-15_match-structure-*.md`. Two sub-slices: (a) clock state + stage transitions + HUD readout, (b) declaration seam wiring that affects hostility graph and imminent-engagement warnings.
2. **Dynasty system.** Reference: `simulation.js` `createDynastyState` through `backfillHeir`. Canon at `04_SYSTEMS/DYNASTIC_SYSTEM.md`. Subsystems: member lifecycle, role chain, aging, succession on ruler death, legitimacy score, fallen ledger. Backbone for every other dynastic mechanic; port first.
3. **Faith commitment + exposure + intensity tiers.** Reference: `simulation.js` `chooseFaithCommitment`, `updateFaithExposure`, `updateFaithStructureIntensity`. Canon at `04_SYSTEMS/FAITH_SYSTEM.md`. The five-stage gate (faith committed end of stage 2) is meaningless without these.
4. **Conviction scoring + bands + band effects.** Reference: `simulation.js` `deriveConvictionScore`, `refreshConvictionBand`, `getConvictionBandEffects`. Canon at `04_SYSTEMS/CONVICTION_SYSTEM.md`. Cheap to port because `ConvictionComponent` already exists; needs system and effect application.
5. **State snapshot + restore.** Reference: `simulation.js` `exportStateSnapshot`, `restoreStateSnapshot`. `unity/Assets/_Bloodlines/Code/SaveLoad/` is empty. Canonical slice gate references restore continuity but nothing implements it. Needed before a multi-session test loop is possible.
6. **Fortification tiers + siege doctrine + imminent-engagement warning.** Reference: `simulation.js` `advanceFortificationTier`, `tickImminentEngagementWarnings`, `tickFortificationReserves`, `tickSiegeSupportLogistics`. Canon at `04_SYSTEMS/TERRITORY_SYSTEM.md`, `governance/DEFENSIVE_FORTIFICATION_DOCTRINE.md`. Stage 4 (War and Turning of Tides) is impossible to ship without these.
7. **Enemy AI strategic layer.** Reference: `src/game/core/ai.js` full file. The current Unity AI is a narrow first-shell slice. Port the dynasty-aware, faith-aware, siege-aware strategic brain so that AI opponents can participate in stages 2 through 5.

### Tier 2 — required for full canon, not blocking a first playable

8. **Marriages, lesser-house defection, minor-house territorial levies.** Reference: `simulation.js` `proposeMarriage`, `acceptMarriage`, `tickMarriage*`, `tickLesserHouse*`, `spawnDefectedMinor*`, `tickMinorHouseTerritorialLevies`.
9. **Captive, ransom, rescue pipeline.** Reference: `simulation.js` `startRansom`, `startRescue`, `demandCaptiveRansom`, `transferMemberToCaptor`.
10. **Intelligence, dossiers, counter-intelligence, espionage.** Reference: `simulation.js` `startEspionageOperation`, `counterIntel` watchers, dossier tracking.
11. **Assassination and sabotage.** Reference: `simulation.js` `startAssassination`, `startSabotageOperation`, `applySabotageEffect`.
12. **Holy wars, missionary operations, divine right declaration, covenant tests.** Reference: `simulation.js` `tickFaithHolyWars`, `startMissionaryOperation`, `buildCovenantTestEvent`, `evaluateCovenantTestProgress`. Canon at `04_SYSTEMS/FAITH_SYSTEM.md`.
13. **Territorial governance, recognition, alliance acceptance drag.** Reference: `simulation.js` `tickTerritorialGovernanceRecognition` and helpers. Canon at `04_SYSTEMS/TERRITORY_SYSTEM.md`.
14. **World pressure escalation, Great Reckoning, Trueborn rise arc.** Reference: `simulation.js` `updateWorldPressureEscalation`, `tickTruebornRiseArc`, `getGreatReckoningProfile`.
15. **Scout raid, resource harass, convoy interdiction, field water discipline.** Reference: `simulation.js` `executeScoutRaid`, `executeResourceHarass`, `executeLogisticsInterdiction`, `tickFieldWaterLogistics`, `refreshFieldWaterSupport`.
16. **Realm condition legibility beyond starvation.** Loyalty band readouts, cap pressure reporting, water crisis telemetry. Canon at `04_SYSTEMS/RESOURCE_SYSTEM.md` plus realm-conditions.json.
17. **Non-aggression pacts and hostility graph.** Reference: `simulation.js` `getNonAggressionPactTerms`, `ensureMutualHostility`.

### Tier 3 — systemic polish, post-first-playable

18. **Naval transport.** Embark, disembark, vessel movement. Bible section 18. Browser implementation is skeletal; spec is light.
19. **Terrain and geography system.** Bible section 22. Not implemented in browser either; spec-only.
20. **Future expansion concepts.** Bible section 15. Explicitly flagged as future.
21. **Graphics pipeline prompt packs.** `03_PROMPTS/GRAPHICS_PIPELINE/*` — already in use for concept art. No code migration; keep updated alongside Unity placeholder refinement.

## Execution Model

Each Tier 1 item becomes a lane using the existing concurrent-session contract (`docs/unity/CONCURRENT_SESSION_CONTRACT.md`). Lane rules:

- One lane, one branch, one agent. No cross-agent file overlap without explicit contract revision.
- Per-slice handoff at `docs/unity/session-handoffs/YYYY-MM-DD-unity-<lane>-<slice>.md`.
- Full validation gate before merge (the canonical 8-step gate in `CLAUDE.md` Unity Validation Gate section).
- Browser reference file path and function cluster cited in every slice handoff.

### Reference discipline

Each slice must include in its handoff:

- **Browser reference** — exact file and function cluster that specifies the behavior.
- **Canon reference** — section of the design bible or `04_SYSTEMS/*.md` that governs.
- **Behavioral parity target** — a short list of observable behaviors the Unity slice must reproduce.

This prevents drift between the frozen browser spec and the Unity implementation.

### Suggested initial slice sequencing

Working backward from Tier 1 dependencies:

1. Dynasty core (item 2) — no dependencies. Unblocks faith commitment, marriages, succession.
2. State snapshot + restore (item 5) — no dependencies. Unblocks multi-session testing for every subsequent lane.
3. Conviction scoring (item 4) — lightweight, uses existing `ConvictionComponent`; informs many downstream modifiers.
4. Faith commitment + exposure (item 3) — depends on dynasty for member-level commitment tracking.
5. Dual-clock + stage progression (item 1) — depends on dynasty for ruler-age stage gating and faith for stage 2 gate.
6. Fortification + siege + imminent engagement (item 6) — depends on dual-clock for stage 4 triggers.
7. AI strategic layer port (item 7) — depends on all of the above because the AI must understand every system it plays with.

Tier 2 slices can start in parallel once Tier 1 items they depend on have landed.

## Outstanding Cleanup Items (pre-existing, not blocking this plan)

Identified during the 24-hour audit, actionable in small slices:

- **`claude/unity-ai-expansion-and-faith` branch (tip `684bcc4`).** Session 125 AI barracks WIP, parked unvalidated. Needs: rebase onto current master, apply bootstrap smoke timeout bump (120s to 180s), run full validation gate, then merge or retire.
- **Workspace-root `HANDOFF.md`** (last updated 2026-04-17 03:51) — references `codex/unity-attack-orders-attack-move` which merged into master via `5167a0b`. Stale; should be archived.
- **Local-only branch `codex/unity-group-movement-and-stances`.** 0 commits ahead of master. Placeholder with no content; prune.
- **Stashes `stash@{0}` through `stash@{3}`.** Predate or parallel the recent merges; contain continuation-platform work (`stash@{0}`, `stash@{3}`) and possibly-superseded Session 116 combat work (`stash@{1}`, `stash@{2}`). Each should be diffed against current master before dropping.

## Non-Goals

- This plan does **not** reopen the browser runtime for new gameplay work. It remains frozen as behavioral specification.
- This plan does **not** touch scope: Wwise audio, Netcode for Entities multiplayer, and full commercial polish remain in force per the owner direction.
- This plan does **not** rewrite canon. Canon is the source of truth; this plan operationalizes canon into Unity slices.

## Open Questions for Owner

- Tier 1 sequencing: begin with dynasty, or with the dual-clock and match progression? Dynasty unlocks more downstream work; match progression produces the most visible player-facing change.
- AI strategic layer port (item 7): port as a single large slice against the full `ai.js`, or split into per-stage AI (stage 1 economic brain is partially there; port stage 2 faith/dynasty brain next)?
- Save/load (item 5): port the browser snapshot format 1:1 and translate at load, or design a new Unity-native serialization now and treat the browser format as read-only reference?

## Changelog

- 2026-04-17 — initial plan drafted after merge of `codex/unity-target-acquisition-los` into master at commit `dc00fff`, following a 24-hour audit and full browser-era session inventory.
