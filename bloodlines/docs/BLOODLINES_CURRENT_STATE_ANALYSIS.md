# BLOODLINES CURRENT STATE ANALYSIS

**Report date:** 2026-04-13
**Authoritative project root:** `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\deploy\bloodlines\`
**Previous archive locations (now legacy):** `D:\ProjectsHome\Bloodlines\`, `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines\`

This report is based on direct repository inspection, live runtime boot, data validation, JavaScript syntax checks, and a cross-read of every production planning document on 2026-04-13. It is not speculation. Every claim below is grounded in what was observed in the canonical deploy folder.

---

## 1. Verified Top-Line Findings

1. The Bloodlines project has been consolidated into `deploy/bloodlines/`. The old external archive at `D:\ProjectsHome\Bloodlines\` was merged into this folder, including canon, session ingestions, system files, exports, and all production docs.
2. A first browser-playable RTS slice exists and boots cleanly. `play.html` autoloads, `src/game/main.js` initializes the simulation through the data-loader, and the main loop ticks. Population growth, control-point capture, neutral tribal raids, sacred-site exposure, and enemy AI decisions are all observably firing.
3. The runtime architecture (data-driven content, JSON definitions, ES-module canvas runtime, no build step) matches the direction locked in `docs/DEVELOPMENT_REALITY_REPORT.md`, `docs/IMPLEMENTATION_ROADMAP.md`, and the Production Recommendation section.
4. The design corpus is enormous and cumulative. The active canon snapshot (`18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md`) is 445KB. The integrated continuity reference (`18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md`) is 1.74MB.
5. The design intent is preserved and authoritative. The settled systems in `01_CANON/CANONICAL_RULES.md` and `01_CANON/CANON_LOCK.md` have not been reduced or flattened. The September 2026 scope directive (no ceiling, depth is the goal) remains explicitly in force.
6. The root-level `bloodlines/` folder inside `lancewfisher-v2/` is a stale partial mirror. It lacks `01_CANON/`, `04_SYSTEMS/`, `07_FAITHS/`, `18_EXPORTS/`, and most design directories. Its `play.html` is 93 lines; the canonical `deploy/bloodlines/play.html` is 101 lines, adding the Dynasty and Faith panel DOM targets. The root copy should be treated as deprecated, not regenerated, unless the operator explicitly chooses to rebuild the mirror.

---

## 2. Verification Pass Results

### 2.1 JavaScript syntax

Every game-runtime module passes `node --check`:

- `src/game/main.js`
- `src/game/core/simulation.js`
- `src/game/core/renderer.js`
- `src/game/core/ai.js`
- `src/game/core/data-loader.js`
- `src/game/core/utils.js`

### 2.2 Data validation

`tests/data-validation.mjs` passes when run from `deploy/bloodlines/`:

```
cd deploy/bloodlines && node tests/data-validation.mjs
# -> "Bloodlines data validation passed."
```

All assertions succeed:
- 9 canonical houses present
- Ironmark marked `prototypePlayable: true`
- Iron and Influence present in resources
- 4 covenants (Old Light, Blood Dominion, The Order, The Wild)
- 8+ bloodline roles (9 defined: Head, Heir, Commander, Governor, Diplomat, Ideological Leader, Merchant, Sorcerer, Spymaster)
- 7 bloodline training paths (Military Command, Governance, Religious Leadership, Diplomacy, Covert Operations, Economic Stewardship/Trade, Mysticism)
- 5 active-canon victory conditions (Military Conquest, Economic Dominance, Faith Divine Right, Territorial Governance, Alliance Victory) plus Dynastic Prestige preserved as `preserved-historical-non-victory`
- 3 frontier control points on `ironmark-frontier.json` (Oakwatch March, Rivergate Ford, Stonefield Watch)
- 4 sacred sites on the map, one per covenant (First Flame Ruin, Red Covenant Stone, Pillar of Sacred Mandate, Elder Root Grove)
- Tribes faction present on the map

The runbook's instruction (`node deploy/bloodlines/tests/data-validation.mjs` from repo root) does not work because the test uses `process.cwd()`. Run from `deploy/bloodlines/` instead. This is a runbook correction, not a code bug.

### 2.3 Live runtime boot

Served via `python -m http.server 8078 --directory deploy/bloodlines`, `play.html` auto-starts the skirmish without requiring manual Start Skirmish interaction (the `?briefing=1` query flag re-enables the launch scene). Observed state after ~8 seconds of simulation:

- `launch-scene.hidden === true`, `game-shell.hidden === false`
- Debug HUD reports: `House: Ironmark`, `Status: playing`
- Ironmark population grew from 12 to 13 (food/water gated growth firing correctly)
- Stonehelm enemy AI captured `Stonefield Watch` control point
- Neutral tribe raiders were dispatched (`Frontier tribes are raiding the marches`)
- Resource bar, Dynasty panel, Faith panel all render populated content
- Canvas pixel inspection confirms the battlefield and minimap are drawing
- Zero console errors, zero failed network requests

### 2.4 Panel DOM bridge

`play.html` now includes `#dynasty-panel` and `#faith-panel` DOM slots, and `main.js` populates them through `renderDynastyPanel()` and `renderFaithPanel()`. The initial Dynasty panel shows `Legitimacy 58 • Active members 8/20` plus the bloodline member roster; the Faith panel shows Conviction band, no selected covenant, and four exposure tracks with progress bars. Both panels wire in, no crash observed.

### 2.5 Faction behavior

Observable behaviors in the running simulation:
- Player Ironmark: gathers passively, no unit commands issued (expected; the player must drive orders).
- Enemy Stonehelm: gathers, expands, pushes for control points, escalates to command-hall attack runs once army ≥ 3 combat units.
- Neutral tribes: raid on a timer, move toward the nearest non-tribe control point or toward a player/enemy Command Hall if no contested march exists.

---

## 3. Systems Implementation Matrix

### 3.1 Legend

- **LIVE** — implemented in the running simulation and visible to the player
- **PARTIAL** — scaffolded in code or data but not fully integrated or surfaced
- **DATA-ONLY** — present in JSON/design docs but not wired into the simulation
- **DOCUMENTED** — present in canon/bible but no runtime artifact yet
- **CANON-OPEN** — intentionally unresolved, pending Lance decision
- **VOIDED** — previously designed but formally rescinded

### 3.2 Matrix

| System | Status | Evidence |
|---|---|---|
| RTS camera, selection, right-click command | LIVE | `main.js` pointer/keyboard handlers, box select, move/attack/gather commands |
| Worker gather-deposit loop (gold, wood) | LIVE | `simulation.js` `updateWorker` carries gather progress, capacity, dropoff search |
| Passive resource trickle (food via Farms, water via Wells) | LIVE | `tickPassiveResources` using `resourceTrickle` field on building defs |
| Housing-driven population cap with base+bonus | LIVE | `recalculatePopulationCaps` reads `populationCapBonus`, `createSimulation` sets baseCap |
| Population growth gated by food/water | LIVE | `tickPopulationGrowth` 18s interval, consumes 1 food and 1 water per growth tick |
| Building placement with footprint and collision | LIVE | `attemptPlaceBuilding`, `insideWorld`, `intersectsAnyBuilding` |
| Worker-built construction progress | LIVE | `updateWorker` build command ticks `buildProgress` vs `buildTime` |
| Barracks + Command Hall production queues | LIVE | `queueProduction`, `updateBuildings` queue ticker, `spawnUnitAtBuilding` |
| Melee combat with attack range and cooldown | LIVE | `updateCombatUnit` handles close-range damage application |
| Ranged combat via projectiles | LIVE | Projectile system with `projectileSpeed` from unit defs, `updateProjectiles` |
| Auto-acquire enemy in sight | LIVE | `findNearestEnemyInRange` uses `sight` field, assigns attack command |
| Control-point capture and loyalty | LIVE | `updateControlPoints` with contested detection, capture progress, loyalty decay, resource trickle for owner |
| Sacred-site faith exposure | LIVE | `updateFaithExposure` per-faction exposure accumulation, discovery messages, threshold for commitment |
| Faith commitment mechanic | LIVE | `chooseFaithCommitment` requires ≥100 exposure, sets doctrine path, faith intensity, tier label |
| Faith intensity growth over time (after commitment) | LIVE | `updateFaithExposure` tails 0.25/s intensity while in the selected faith's aura |
| Conviction state band readout | LIVE | `getConvictionBand` returns Apex Moral through Apex Cruel labels |
| Dynasty roster generation | LIVE | `createDynastyState` builds 8 named members per kingdom faction with role, path, age, renown |
| Dynasty panel rendering | LIVE | `renderDynastyPanel` shows Legitimacy, member cap, roles, paths |
| Faith panel rendering | LIVE | `renderFaithPanel` exposes progress bars for all 4 faiths, commit buttons once threshold met |
| Message log with tone (info/good/warn) | LIVE | `pushMessage` + `updateMessages` TTL decay, `MESSAGE_LIMIT` cap at 6 |
| Resource bar (gold, wood, influence, food, water, territory, available, population) | LIVE | `renderer.buildResourceBar` produces all 8 fields |
| Minimap with terrain, buildings, units, camera box | LIVE | `drawMinimap` in `renderer.js` |
| Enemy AI: economy, expansion, raiding, territory, faith pickup | LIVE | `updateEnemyAi` - worker assignment, building placement priority (barracks, dwellings, farms, wells), territory moves, attack escalation, faith commitment when a sacred site exposure ramps |
| Neutral tribes AI: raid timer, contested territory movement | LIVE | `updateNeutralAi` |
| Lose condition (player Command Hall destroyed) | LIVE | `applyDamage` flips `state.meta.status = "lost"` |
| Win condition (enemy Command Hall destroyed) | LIVE | same codepath sets `"won"` |
| Pause/resume | LIVE | HUD button wired to `uiState.paused` |
| Unit pathing beyond simple steering | PARTIAL | Uses `moveToward`. No true pathfinding. Units can snag on buildings or each other in crowded spaces |
| Formation stances / hold-ground / patrol | DOCUMENTED | Not in simulation; canon calls for them |
| Morale and rout | DOCUMENTED | Faction system has no morale scalar yet |
| Siege logic | DOCUMENTED | Combat works against buildings but there is no siege engine class, no wall object |
| Province/governance layer (on top of control points) | CANON-OPEN | `docs/DEFINITIVE_DECISIONS_REGISTER.md` Decision 6 proposes hybrid |
| Occupation vs stabilized control distinction | PARTIAL | Loyalty is tracked per control point but there is no occupation-only degraded state yet |
| Commander battlefield presence | DOCUMENTED | `docs/DEFINITIVE_DECISIONS_REGISTER.md` Decision 8 recommends battlefield-present; no runtime unit class yet |
| Bloodline member attachment to armies | DATA-ONLY | Roster exists in state and UI; no link to combat units |
| Commander death / capture / succession | DOCUMENTED | Dynasty has `status` field but death flow is not implemented |
| Marriage, inheritance, cross-dynasty children | DOCUMENTED | Designed in `04_SYSTEMS/DYNASTIC_SYSTEM.md`; no runtime |
| Succession mechanics | DOCUMENTED | Heir Designate role exists; flow not implemented |
| Doctrine Path divergence (Light/Dark) per covenant | PARTIAL | `doctrinePath` stored; chooseFaithCommitment defaults to `light`; no branching effects yet |
| Faith intensity tier effects (L1–L5) | PARTIAL | Tier label rendered; no gameplay effect per tier yet |
| Conviction behavior ledger (Ruthlessness, Stewardship, Oathkeeping, Desecration) | DOCUMENTED | `conviction-states.json` has band labels only; no event accrual in simulation |
| Faith unit classes (L3+, apex L5) | DOCUMENTED | Canon has 2-2-1 per covenant pattern; none in units.json |
| Doctrine-gated faith operations | DOCUMENTED | Faith ops canonicalized; no code |
| Ironmark Blood Production / blood cost loop | PARTIAL | Ironmark status is `fully-locked` in data, but the blood-cost mechanic is not in the simulation |
| Hartvale Verdant Warden | DATA-ONLY | Unit not present in `units.json`; house stance is `partially-locked` |
| Ironmark Axeman unique | DATA-ONLY | Defined in `units.json` with `prototypeEnabled: false` and `house: "ironmark"` |
| Scout Rider stage-2 cavalry | DATA-ONLY | Defined in `units.json`, `prototypeEnabled: false` |
| Stone resource | DATA-ONLY | Defined in `resources.json`, `enabledInPrototype: false` |
| Iron resource | DATA-ONLY | Defined in `resources.json`, `enabledInPrototype: false` |
| Influence resource as advanced | DATA-ONLY | In resource bar display; trickled from control points; not spent anywhere yet |
| 5 match stages (Founding → Final Convergence) | DOCUMENTED | Canon locks the five-stage model; runtime has single skirmish meta |
| 3 match phases (Emergence → Commitment → Resolution) | DOCUMENTED | Canon locks overlay; runtime has none |
| Multi-speed time (Battlefield / Campaign / Dynastic) | DOCUMENTED | Locked in canon; runtime has single real-time clock |
| Declared-time strategic layer (post-battle Declaration) | DOCUMENTED | Full Dual-Clock Architecture addendum exists; no runtime |
| Directive system (14 directives, 5 categories) | DOCUMENTED | Canon; no runtime |
| Attitude modifiers (7 modifiers) | DOCUMENTED | Canon; no runtime |
| Command capacity soft threshold | DOCUMENTED | Canon; no runtime |
| Naval warfare (6 vessel classes) | DOCUMENTED | Canon locks 6 classes; no water movement, no ship definitions |
| Continental world architecture (Home + secondary continents) | DOCUMENTED | Canon; ironmark-frontier.json is a single frontier map |
| Neutral Trueborn City activation arc | DOCUMENTED | Canon; no runtime |
| 9 founding houses playable | PARTIAL | All 9 present in `houses.json`; only Ironmark marked `fully-locked` and `prototypePlayable` |
| Mysticism path / Sorcerer role | DATA-ONLY | Role + path exist in JSON; no runtime effect |
| Polygamy for Blood Dominion / Wild | DOCUMENTED | Canon; no marriage system yet |
| Economic Dominance / Currency Dominance victory | CANON-OPEN | Decision 13 pending; not in simulation |
| Faith Divine Right declaration window | DOCUMENTED | Canon; no runtime |
| Territorial Governance threshold (~90%) | DOCUMENTED | Canon working assumption; no runtime |
| Alliance Victory conditions | DOCUMENTED | Canon locks alliance trigger rules; no multi-player runtime |
| Operations system (covert, faith, military) | DOCUMENTED | `08_MECHANICS/OPERATIONS_SYSTEM.md` exists at 50KB; no runtime |
| Political events (28+) | DOCUMENTED | `11_MATCHFLOW/POLITICAL_EVENTS.md` 102KB, duplicate 30KB in `08_MECHANICS/POLITICAL_EVENTS.md` (see fragmentation note below) |
| Save/resume for long matches | DOCUMENTED | Decision 16 pending; no save system |
| Match setup / lobby / house-select | DOCUMENTED | Hardcoded Ironmark vs Stonehelm skirmish |
| Tutorial / onboarding layer | DOCUMENTED | `launch-card` copy; no in-game tutorial |
| AI kingdoms beyond Stonehelm | DOCUMENTED | Only one AI dynasty in current map |
| Audio layer | DOCUMENTED | No audio in prototype |
| CB004 house profiles for 8 non-Ironmark houses | VOIDED | 2026-04-07; archived in `19_ARCHIVE/CB004_VOIDED_2026-04-07/` |
| Frost Elder mandatory death mechanic | VOIDED | 2026-03-19 |
| Separate Scout unit | VOIDED | Swordsman carries recon; Scout Rider is later tier |
| Dynastic Prestige as victory path | VOIDED | 2026-04-07; reframed as modifier system |

### 3.3 Data-vs-code alignment warnings

- `Hartvale Verdant Warden` is settled in canon (CANONICAL_RULES.md) and in `CANON_LOCK.md`, but `units.json` does not yet contain a `verdant_warden` entry. Hartvale remains `partially-locked`. No blocker, just a data gap to close when Hartvale moves into runtime.
- `Axeman` is the Ironmark unique melee and is in `units.json` with `prototypeEnabled: false`, meaning the prototype currently fields Militia, Swordsman, Bowman only. This matches the locked early roster in Decision 5.
- `Ironmark Blood Production` is the locked Ironmark strategic cost and it is not yet enforced in simulation. Ironmark plays identically to any shared-roster house in the current runtime. This is the single most important missing piece of Ironmark identity.

---

## 4. Document / Canon State

### 4.1 Authoritative document stack (reading order for any session)

1. `docs/DEVELOPMENT_REALITY_REPORT.md` — what actually exists vs what is documented
2. `docs/IMPLEMENTATION_ROADMAP.md` — 12-phase shipping plan
3. `docs/DEFINITIVE_DECISIONS_REGISTER.md` — 20 convergence decisions and recommended defaults
4. `docs/COMPLETION_STAGE_GATES.md` — 12 gates defining completion-stage production
5. `01_CANON/CANONICAL_RULES.md` — full granular Settled / Proposed / Open ledger
6. `01_CANON/CANON_LOCK.md` — high-level lock summary, additive format
7. `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.2.md` — current active bible (445KB)
8. `18_EXPORTS/BLOODLINES_COMPLETE_UNIFIED_v1.0.md` — integrated continuity reference (1.74MB)
9. `01_CANON/BLOODLINES_MASTER_MEMORY.md` — cumulative historical memory
10. Subsystem depth: `04_SYSTEMS/`, `07_FAITHS/`, `08_MECHANICS/`, `10_UNITS/`, `11_MATCHFLOW/`

### 4.2 Known document fragmentation / drift points

The project has grown cumulatively, which is correct, but cumulative growth created these observable fragmentation points. None are blockers. All should be reconciled during future archival passes, not by deleting material.

- `POLITICAL_EVENTS.md` exists twice: `11_MATCHFLOW/POLITICAL_EVENTS.md` (102KB, authoritative-looking) and `08_MECHANICS/POLITICAL_EVENTS.md` (30KB, narrower). Both contain real content. Reconcile by deciding which is canonical and moving or cross-linking the other rather than deleting.
- Root-level documents at the deploy/bloodlines root (`BLOODLINES_ADDENDUM_A.md`, `BLOODLINES_ADDITIVE_INTEGRATION.md`, `BLOODLINES_ADDITIVE_INTEGRATION2.md`, `BLOODLINES_CLAUDECODE_HANDOFF.md`, `BLOODLINES_DESIGN_ADDITIONS.md`, `BLOODLINES_FIVE_STAGES.md`) are session-era integration drafts. `BLOODLINES_ADDENDUM_A.md` at root duplicates `02_SESSION_INGESTIONS/BLOODLINES_ADDENDUM_A_2026-04-07.md`. Root copies can stay per additive archival rules; they should not be modified. New additive material should go into `02_SESSION_INGESTIONS/` with a date suffix.
- `CB004_VOIDED_2026-04-07/` under `19_ARCHIVE/` is the correct archival outcome of the 2026-04-07 canon reset. The 8 non-Ironmark house CB004 strategic profiles are preserved but out of active canon. This is working as intended.
- The root-level `bloodlines/` mirror inside `lancewfisher-v2/` is drift, not archive. Many deploy-only directories and files never made it back to root. This is not a problem as long as the operator treats the root mirror as deprecated. The portfolio's `deploy → root` sync rule should not be used to try to reconcile the root mirror into a functional Bloodlines tree; that would risk pushing stale or incomplete state into git.

### 4.3 Content categories preserved from the design corpus

The full design material is preserved and must continue to be treated as cumulative. Key depth areas (not exhaustive):

- `04_SYSTEMS/` (total ~290KB): Conviction, Faith (112KB), Population, Resource, Territory, Dynastic (66KB), Born of Sacrifice (52KB). These are full specification files.
- `07_FAITHS/FOUR_ANCIENT_FAITHS.md` (273KB): Deep covenant architecture including Doctrine Path asymmetry, L1–L5 faith ladders, apex units, light vs dark practice, maintenance, intensity decay, apocalyptic escalation pathways.
- `08_MECHANICS/OPERATIONS_SYSTEM.md` (50KB): Covert, faith, military, and hybrid operations with detection, cooldowns, counterplay.
- `10_UNITS/UNIT_INDEX.md` (33KB): Full roster canon including 5-tier progression and unit promotion pipeline.
- `11_MATCHFLOW/MATCH_STRUCTURE.md` (24KB) + `NAVAL_SYSTEM.md` (13KB): Stage/phase architecture, dual-clock, commander system, directives, attitudes.
- `12_UI_UX/UI_NOTES.md` (61KB): Surface-level and deep UI architecture notes.
- `13_AUDIO_VISUAL/AUDIO_VISUAL_DIRECTION.md` (40KB): Art and audio direction before any asset production.
- `14_ASSETS/CREATIVE_BRANCH_002*` and `CREATIVE_BRANCH_003*`: Exploratory reservoir content. These are branches, not active canon. Preserve them; do not cite them as if they were active canon.

---

## 5. Gates Status (Completion Stage Gates)

Per `docs/COMPLETION_STAGE_GATES.md`, as observed on 2026-04-13:

| Gate | Status | Notes grounded in runtime |
|---|---|---|
| 1: Battle Layer | PARTIAL → near-complete | Selection, economy, housing, production, combat, win/loss are wired. Pathing and formations remain the gap. |
| 2: Territory Layer | PARTIAL | Control points, loyalty, capture timer, owner-only trickle, contested/neutral states all fire. Province/governance consequences still pending. |
| 3: House Layer | NOT MET | Only Ironmark and Stonehelm-as-enemy on the map. No house-select. No second/third strategic identity yet. |
| 4: Bloodline Layer | PARTIAL (roster only) | Roster and panel exist. No battlefield attachment, no succession flow, no death propagation. |
| 5: Faith & Conviction | PARTIAL | Exposure, discovery, commit mechanic, tier, intensity, sacred-site auras all work. Doctrine-Path divergence and conviction ledger accrual are not yet wired. |
| 6: Operations Layer | NOT MET | Documented only. |
| 7: World Pressure | PARTIAL | Neutral tribes raid and contest. Neutral hub and Trueborn intervention are absent. |
| 8: AI Layer | PARTIAL | Stonehelm builds, expands, contests, raids, and converts to faith. Still scripted; no multi-victory-path reasoning. |
| 9: UX Layer | PARTIAL | Battle HUD, selection, command, dynasty, faith, message log present. Territory overlay + match setup + onboarding still pending. |
| 10: Technical Layer | PARTIAL | Data-driven content, JSON loaders, validation test, ES-module split working. No save/resume, no profiling, no deterministic replay hook. |
| 11: Content Layer | NOT MET | One map, three playable unit types, eight placeholder houses. |
| 12: QA & Production | PARTIAL | Runbook, reality report, known issues, roadmap, decisions register, completion gates all exist. No smoke-test checklist automation, no balance-change log. |

---

## 6. What Was Recently Attempted But Not Fully Validated

Per the operator handoff, the following runtime edits happened in the in-progress window and should be considered validated:

- Data-loader paths using `new URL(path, import.meta.url)` — **confirmed working** via 200s from `/data/*.json` during live boot.
- Startup error surfacing via `window.error` and `unhandledrejection` handlers — **confirmed wired** in `main.js`.
- Autostart behavior for direct `play.html` entry (bypass briefing via absence of `?briefing=1`) — **confirmed working**; game autostarts on load.
- Renderer HUD territory crash fix — **confirmed**; debug overlay reads `state.world.controlPoints` without throwing.
- Neutral tribe raids + control-point contesting by Stonehelm — **confirmed firing** via observable messages and owner state changes.
- Dynasty/Faith panels now DOM-backed in `play.html` and rendered by `main.js` — **confirmed** in live DOM.

No partially-applied or broken patches were found in the deploy canonical tree during this audit.

---

## 7. Risks, Contradictions, and Open Gaps

1. **Ironmark identity in runtime**. Ironmark is the only strategically locked house in canon and the only `prototypePlayable: true` house in data, but the Blood Production cost loop — the defining canonical asymmetry of Ironmark — is not enforced in simulation. This is the highest-priority identity gap.
2. **Pathing**. Steering-only movement means dense unit groups in tight terrain can get stuck. Canon expects Warcraft III-level tactical readability; current pathing is below that bar. Decision 2 (Runtime Stack Lock) recommends keeping vanilla through the next two milestones, with pathing as a milestone driver.
3. **Faith Doctrine Path effect vacuum**. Faith commitment defaults to `light` and doctrine choice has no consequences in simulation. This is surfaced in the UI but not yet branching. Canon demands asymmetric Light/Dark doctrine paths.
4. **Conviction is a label only**. Band (Apex Moral → Apex Cruel) renders correctly, but no player action updates the score. The canonical four-bucket model (Ruthlessness, Stewardship, Oathkeeping, Desecration) is documented and not implemented.
5. **Bloodline members are rosters, not actors**. Dynasty panel shows 8 members for the player faction, but those members have no battlefield presence, no death propagation, no succession, no consequence. This is the primary bridge between RTS and dynasty simulation and is the commander-presence decision in Decision 8.
6. **Single-map, single-house playable**. No map rotation, no lobby, no house-select, no match setup. Blocking for any internal demo that is not "Ironmark vs Stonehelm on the frontier."
7. **Strategic layer is absent**. The Dual-Clock Architecture addendum (Declaration Phase, Events Queue, Commitment Phase) is fully designed and not started in code. Canon treats this as the rhythm of a Bloodlines session.
8. **Naval is canonically locked but runtime-absent**. Decision 14 recommends not blocking launch on naval, which the roadmap honors; the risk is only that map architecture must be designed with water transport in mind so it does not need to be retrofitted.
9. **CB004 reset gap**. Hair color and color theme are the only settled strategic elements for 8 of 9 houses. Any new strategic design for those houses must be explicit and additive; don't cite CB004 content as canon.
10. **Root mirror drift**. Not a runtime risk, but a continuity risk: anyone reading `bloodlines/` in the repo root will see incomplete state and may assume the whole project is a tiny prototype, not the full archive.

---

## 8. Immediate Next Engineering Focus

Ordered by dependency and by the operator handoff's stated priorities.

1. **Stabilize runtime**: lock input, render, data loading. Confirm zero console errors on boot, confirm autostart clean, confirm pause/resume and return-to-menu round-trip cleanly. *(Already green as of 2026-04-13.)*
2. **Runtime dynasty/faith state bridge (expansion phase)**: Make the Dynasty roster consequential by linking Commander and Governor roles to armies and control points. Wire at least one faith Doctrine Path effect so committed faith matters on the battlefield. Track conviction score via an event accumulator rather than as a static label.
3. **Sacred site interaction polish**: expose ruin/shrine discovery in the world as explicit events in the message log with faith name and exposure gain; surface committed covenants on the minimap.
4. **Ironmark Blood Production cost loop**: implement the locked Ironmark asymmetry by tying combat unit training or upkeep to a population-cost modifier that reflects blood cost under attritional warfare.
5. **Territory → governance**: introduce occupation vs stabilized loyalty distinction so captured points degrade a hostile owner's loyalty before flipping, and so stable owners accrue governance benefits (e.g., bonus influence, reduced shortage penalties).
6. **Pathing pass**: replace pure steering with a grid + waypoint approach sufficient to avoid building snags with 150–250 live units on a 72×48 map.
7. **Second playable house (Stonehelm) as player-selectable**: introduce a minimal match-setup or URL-driven house select so the skirmish can launch with Stonehelm's defensive asymmetry available.
8. **Commander presence (Decision 8)**: prototype a Commander as a battlefield unit tied to a bloodline member id, with aura bonus, killable, capturable, and name displayed.
9. **Dual-clock scaffolding**: introduce the Declaration seam. Even a minimal post-battle summary with declared in-world elapsed time is enough to prove the rhythm.

This order does not reduce Bloodlines scope. It surfaces the canonical systems in runtime one layer at a time, from RTS stability into dynasty simulation into strategic layer.

---

## 9. Handoff Notes for Next Claude / Codex Sessions

- Assume `deploy/bloodlines/` is canonical. Do not treat `bloodlines/` at repo root or `D:\ProjectsHome\Bloodlines\` as authoritative.
- Do not reduce scope. If a canonical system is absent from code, treat its absence as implementation debt, not as a reason to redesign smaller.
- Additive edits only for canon/bible files. Put new material into `02_SESSION_INGESTIONS/` with a dated file, or into a new dated section in the appropriate canon file, never a rewrite.
- Validate after structural changes: run `tests/data-validation.mjs` from `deploy/bloodlines/`, boot `play.html` against a local server, confirm console clean.
- When multiple agents are active, split by the lane protocol in `docs/BLOODLINES_PARALLEL_EXECUTION_PROTOCOL.md`.
- Next directions list lives in `docs/BLOODLINES_NEXT_DIRECTIONS_AFTER_CODEX_REVIEW.md` and is kept in sync with the roadmap.

---

## 10. Summary

Bloodlines is in a healthier shape than any prior audit would suggest. The combination of a full archival canon, a working browser RTS core, meaningful data-driven content, active enemy and neutral AI, operable faith and dynasty runtime state, and explicit completion gates means the project has moved from "design archive" into "prototype-with-convergence-plan". The immediate risk is not scope creep. The immediate risk is treating the prototype as a toy because it is now playable, and shrinking the vision to match what already runs. Bloodlines is not a toy, and the audit found no reason to reduce scope. The forward path is to thread the documented systems into the runtime one well-scoped milestone at a time, while preserving the full canon.

---

## 11. 2026-04-14 Integration Addendum — Dynasty Consequence Cascade and Fortification Doctrine

This addendum records the state changes produced by the 2026-04-14 session. The consolidated canonical root is now `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines` (per `docs/CONSOLIDATION_NOTE_2026-04-13_SINGLE_ROOT.md`). References in earlier sections of this analysis to `deploy/bloodlines/` as the canonical root should be read against that consolidation note; the canonical code, tests, and docs now live in `bloodlines/`.

### 11.1 Dynasty Consequence Cascade — Implemented

The implementation matrix (Section 3.2) is updated by the following canonical runtime additions:

| System | Previous status | Current status (2026-04-14) |
|---|---|---|
| Commander battlefield presence | DOCUMENTED | LIVE (commander attachment to combat unit with aura bonus, death registered) |
| Bloodline member attachment to armies | DATA-ONLY | LIVE (commander role resolves to unit; tested) |
| Commander death / capture / succession | DOCUMENTED | PARTIAL → LIVE (capture vs kill resolution, succession cascade, heir backfill, interregnum state) |
| Governor fall on territorial loss | not previously classified | LIVE (captured or displaced based on hostile presence at flip) |
| Captive ledger | not previously classified | LIVE (faction.dynasty.captives with ransom influence trickle) |
| Fallen ledger | not previously classified | LIVE (faction.dynasty.attachments.fallenMembers with disposition) |
| Doctrine Path divergence (Light/Dark) per covenant | PARTIAL | LIVE (doctrine effects applied to capture, stabilization, growth, aura; commit UI surfaces Light/Dark choices) |
| Conviction behavior ledger (Ruthlessness, Stewardship, Oathkeeping, Desecration) | DOCUMENTED | LIVE (four-bucket accrual, derived band label, runtime triggers) |
| Occupation vs stabilized control distinction | PARTIAL | LIVE (occupied → stabilized transition at loyalty threshold) |
| Ironmark Blood Production / blood cost loop | PARTIAL | LIVE (basic: non-worker training consumes a blood-levy pop + ruthlessness event) |

Verification: `tests/data-validation.mjs` and `tests/runtime-bridge.mjs` both pass from `bloodlines/`. All runtime modules pass `node --check`. Live browser boot on `http://localhost:8057/play.html` shows dynasty panel, debug overlay, heir line, captives block rendering without console or network errors.

### 11.2 Defensive Fortification Doctrine — Canonical Lock

New canonical strategic pillar locked 2026-04-14. Full doctrine at `01_CANON/DEFENSIVE_FORTIFICATION_DOCTRINE.md`. Defender specification: `04_SYSTEMS/FORTIFICATION_SYSTEM.md`. Attacker specification: `04_SYSTEMS/SIEGE_SYSTEM.md`. Design-bible integration at Sections 82-85 in `18_EXPORTS/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md` (bible bumped v3.2 → v3.3, canonical desktop copy at `D:/Lance/Desktop/BLOODLINES_COMPLETE_DESIGN_BIBLE_v3.3.md`).

Implications on the implementation matrix (Section 3.2):

| System | Doctrine implication |
|---|---|
| Layered fortification (outer works, inner ring, final core) | CANONICAL, implementation pending |
| Defensive ecosystem components (walls, gates, towers, garrisons, chokepoints, kill zones, signal systems, reserve mustering, fallback positions) | CANONICAL, implementation pending |
| Settlement class hierarchy (border, military fort, trade town, regional stronghold, primary keep, fortress-citadel) | CANONICAL, implementation pending |
| Fortification specialist populations (garrison, engineers, signal keepers, wall wardens, tower artillerists, keep guard) | CANONICAL, implementation pending |
| Siege engines and engineer specialists | CANONICAL, implementation pending |
| Siege supply continuity and scouting | CANONICAL, implementation pending |
| Assault failure penalty mathematics (wave-spam denial) | CANONICAL, implementation pending |
| Faith-integrated fortifications (Old Light pyre wards, Blood Dominion altar reserves, Order edict wards, Wild root wards) | CANONICAL, implementation pending |
| Bloodline presence bonuses at keep | CANONICAL, implementation pending; partial runtime via 2026-04-14 cascade |
| Late-game apex fortifications | CANONICAL, implementation pending |

### 11.3 Gates Status Update

Per `docs/COMPLETION_STAGE_GATES.md`, current 2026-04-14 delta:

| Gate | Previous | Updated (2026-04-14) |
|---|---|---|
| 2: Territory Layer | PARTIAL | PARTIAL → broader (occupation/stabilized distinction live; governor loss live; fortification tier + layered defense CANONICAL, implementation pending) |
| 4: Bloodline Layer | PARTIAL (roster only) | PARTIAL → deeper (commander presence live, capture live, succession cascade live, interregnum live, captive ledger live; marriage/inheritance still DOCUMENTED) |
| 5: Faith & Conviction | PARTIAL | PARTIAL → deeper (doctrine path effects live, conviction ledger live) |

No previously-closed gate is re-opened by this addendum.

### 11.4 Top New Risks

1. **Fortification implementation debt.** The Defensive Fortification Doctrine is canonical but not yet implemented. Any future implementation work against fortifications must honor the ten pillars. Scope reduction is explicitly not permitted.
2. **Wave-spam denial math must be tunable.** The assault failure penalty mathematics are canonical; the specific tuning values (fortification-tier factor, morale decay rate, supply drain per assault) remain to be specified in the data layer.
3. **AI siege planning.** The doctrine requires AI attacking a developed fortress to not throw line infantry at walls. The current AI (in `ai.js`) has no fortification awareness. Implementation must introduce siege-preparation decision trees before the AI can coexist with canonical fortifications.
4. **Captive ledger growth cap.** Captives currently have no explicit cap beyond the 16-entry ledger limit. Future ransom / rescue flows may need a formal capacity model.

### 11.5 Next Engineering Focus (Updated)

Ordered by canonical priority and upward-building progression:

1. **Fortification tier metadata on control points and settlements.** Smallest canonical unlock; introduces tier attribute and begins the wall/tower progression pipeline.
2. **Assault failure penalty simulation.** Wave-spam denial math is a canonical non-negotiable and unlocks AI fortification awareness.
3. **Siege engine unit class.** First attacker-side concrete entity.
4. **Reserve cycling for garrisons.** Makes the defensive ecosystem a live artifact rather than a building list.
5. **Captured member rescue / ransom operations.** Builds on the 2026-04-14 cascade.
6. **Governor specialization (city / border / keep).** Makes the Defense role a live specialization pipeline.
7. **Faith-integrated fortification bonuses.** Activates covenant-specific defensive expressions.
8. **Commander presence bonuses at the keep.** Extends the 2026-04-14 commander aura into fortification-specific leverage.

These items honor both the Dynasty Consequence Cascade runtime surface and the Fortification Doctrine canonical lock. Neither scope is reducible.
