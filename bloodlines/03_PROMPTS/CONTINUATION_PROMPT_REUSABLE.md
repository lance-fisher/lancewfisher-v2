# Bloodlines — Reusable Continuation Prompt

**Purpose:** This prompt is designed to be pasted unchanged into any future session to continue advancing the Bloodlines strategy game project toward full realization. It anchors against filesystem artifacts (handoff, continuation plan, execution roadmap, continuity files) that each session updates as part of its close protocol, so the prompt itself stays evergreen.

**How to use:** Paste the body below (the section starting with `---` and ending at the end of this file) as the first message in a new session against `D:\ProjectsHome\Bloodlines`. The agent will read current state, pick up the next outstanding work, advance it additively, verify, and update continuity files.

---

You are continuing work on Bloodlines, a grand dynastic civilizational RTS game being built toward its full intended scale and complexity. This is not an MVP project. Scope reduction is not acceptable. You are continuing a long arc across many sessions, preserving intent, and moving runtime implementation toward the canonical design.

## Your Operating Root

- Canonical session root: `D:\ProjectsHome\Bloodlines`
- This resolves through a junction to `D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines`
- Do not create a parallel Bloodlines root. Do not treat `deploy/bloodlines`, `frontier-bastion`, or `archive_preserved_sources/` as active roots.

## Session Start Protocol

Before touching any file, read these in order. Do not skim. If a file is large, read the parts that describe canon, current state, and the next action. Load more as needed.

1. `AGENTS.md`
2. `README.md`
3. `CLAUDE.md`
4. `MASTER_PROJECT_INDEX.md`
5. `MASTER_BLOODLINES_CONTEXT.md` (especially the 2026-04-14 Session 9 addendum and any later addenda)
6. `CURRENT_PROJECT_STATE.md`
7. `NEXT_SESSION_HANDOFF.md`
8. `SOURCE_PROVENANCE_MAP.md`
9. `continuity/PROJECT_STATE.json`
10. `01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md` (the creator's preservation directive)
11. `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md` (the session 9 state analysis)
12. `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md` (the session 9 gap matrix)
13. `docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md` (six-vector growth model)
14. `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` (ordered sequencing for sessions 9 through 30 plus)
15. The most recent `docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_*.md` (whatever the latest session produced)
16. `tasks/todo.md` (running checklist)

If later sessions have produced additional state-of-game reports, gap analyses, or plans, read those as well.

## Non-Negotiable Posture

1. **No scope reduction.** Bloodlines is a grand dynastic civilizational war game. Every canonical system listed in the Master Design Doctrine and the Session 9 State Analysis stays intact. Never substitute a thinner mechanic for a canonical one because implementation is harder. Never flatten the game into a generic RTS. Never collapse the 5-stage match into fewer stages. Never reduce the four faiths, nine houses, six vessel classes, or continental architecture.
2. **Additive canon only.** New material goes into `02_SESSION_INGESTIONS/` with a dated filename, or into a dated section appended to the relevant canon file. Never rewrite settled canon. Never delete from `archive_preserved_sources/`, `19_ARCHIVE/`, or `governance/` without explicit Lance authorization.
3. **Preservation of prior session work.** The browser reference simulation (`play.html`, `src/`, `data/`, `tests/`) is the working spec. Every live system stays live across sessions unless Lance explicitly authorizes a regression.
4. **Tests stay green.** Every session closes with `node tests/data-validation.mjs`, `node tests/runtime-bridge.mjs`, and `node --check` on every simulation module passing.
5. **Legibility follows depth.** Any system made live in simulation becomes legible in UI within at most two sessions. Do not let the runtime silently run ahead of the HUD.
6. **Unity lane respects its blocker.** Do not write ECS code in `unity/` until the version alignment decision (6.3 LTS vs 6.4) is explicitly resolved by Lance. Recommended option remains Unity 6.3 LTS (`6000.3.13f1`).

## Work Selection

After reading the session start files, determine the next action using this priority order:

1. If `NEXT_SESSION_HANDOFF.md` specifies a concrete next action, execute that next action.
2. Otherwise, consult `docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md` and select the earliest unfinished session item.
3. Otherwise, consult the "Next Session Priorities" section of `tasks/todo.md`.
4. Otherwise, consult the six-vector growth model in the continuation plan and pick an item that advances a lagging vector.

Do not skip blocked work. If the next item is blocked on a Lance decision (Unity version alignment, CB004 rebuild for a specific house, etc.), surface that to Lance, ask for resolution, and pivot to the next unblocked item.

Never pick an item by what is easiest. Pick by what unlocks the most downstream work. The execution roadmap already sequences by leverage.

## Work Execution

When executing the selected item, follow this pattern:

1. **Read the existing implementation surface.** Find the code paths, data files, and canon references that the new work will extend.
2. **Implement additively.** Extend `data/*.json`, `src/game/core/*.js`, `src/game/main.js`, `src/game/core/renderer.js`, `src/game/core/ai.js`, and tests without removing prior behavior.
3. **Preserve canonical intent.** If a system has canonical depth documented in `04_SYSTEMS/` or `07_FAITHS/` or `08_MECHANICS/`, implement at the canonical depth, not a thinner substitute. If full canonical depth is too much for one session, implement the first canonical layer and explicitly leave the rest for the next session with a tracking entry in `tasks/todo.md`.
4. **Update `getRealmConditionSnapshot` and HUD** whenever you add a system that the player should see. Legibility is a first-class responsibility.
5. **Update AI awareness** whenever you add a mechanic the opponent should react to. Do not let Stonehelm become oblivious to new canonical mechanics.
6. **Extend tests.** Add assertions to `tests/data-validation.mjs` for new data schema and to `tests/runtime-bridge.mjs` for new runtime behavior.

## Verification (Required Before Session Close)

Run all of the following. Do not mark the session complete unless all pass.

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
```

Then boot the runtime and verify zero console errors and that the HUD reflects any new systems:

```powershell
python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines
```

Open `http://localhost:8057/play.html`. Confirm:

- Launch scene transitions to game shell (autostart is default).
- Resource bar shows all canonical pills.
- Realm-condition bar shows all 11 canonical pressure states (Cycle, Pop, Food, Water, Loyalty, Fort, Army, Faith, Conviction, Logistics, World).
- Dynasty panel populates.
- Faith panel populates.
- No console errors.
- No failed network requests.

## Session Close Protocol

Before ending the session, update every one of these files additively:

1. **`docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_N.md`** (new file for this session, N = next unused number).
   - What you did.
   - What is newly live.
   - What remains.
   - Top risks and drift watches.
   - Verification results.
2. **`00_ADMIN/CHANGE_LOG.md`** — prepend a new dated entry with the scope, runtime surface updates, verification results, and preservation statement.
3. **`CURRENT_PROJECT_STATE.md`** — update the "Last updated" line and add a new session subsection listing new live systems.
4. **`NEXT_SESSION_HANDOFF.md`** — update the "Last updated" line, refresh "Next Recommended Action", and add the new session report to "New Analysis Surfaces".
5. **`continuity/PROJECT_STATE.json`** — update `updated_at`, append entries to `browser_reference_simulation_live_systems`, update `browser_reference_simulation_next_wave`, refresh `latest_state_of_game_report`.
6. **`tasks/todo.md`** — move the completed item from "Next Session Priorities" into a new "Completed" section and append any follow-up items.
7. **`MASTER_BLOODLINES_CONTEXT.md`** — if the session introduced a meaningful new continuation surface, append a new addendum at the bottom. Do not rewrite prior content.
8. **Reclassify `docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md`** if the session moved items from `DOCUMENTED` to `PARTIAL` or from `PARTIAL` to `LIVE`. The classification direction is always from less-live toward more-live. Never reverse.

## Deliverables Expected From Every Session

At minimum every session produces:

- Working code that passes all tests.
- An updated state-of-game report.
- Updated `CURRENT_PROJECT_STATE.md`, `NEXT_SESSION_HANDOFF.md`, `continuity/PROJECT_STATE.json`, `00_ADMIN/CHANGE_LOG.md`, `tasks/todo.md`.
- No regressions in prior live systems.
- No console errors in the live runtime.

Sessions that introduce architectural shifts, absorb new external material, or produce comprehensive analysis also produce:

- A gap analysis refresh.
- A continuation plan addendum.
- An execution roadmap refresh.
- A `MASTER_BLOODLINES_CONTEXT.md` addendum.

## Six Vectors That Must All Keep Advancing

Do not let any vector stagnate indefinitely. If a vector has not been advanced in three or more sessions, select its next roadmap item even if another vector would be faster.

1. **Civilizational depth** — population, water, food, land, forestry, supply, logistics.
2. **Dynastic depth** — bloodline members as actors, marriage, succession, lesser houses, captivity consequence.
3. **Military depth** — land + naval doctrine axes, commanders + generals, delegated + direct command, fortification, siege.
4. **Faith + conviction depth** — four covenants, Light/Dark paths, 5-tier ladders, apex structures, conviction spectrum, dark-extremes pressure.
5. **World depth** — continental geography, 5-stage match, multi-speed time, dual-clock declaration, world pressure, Trueborn late return, up to 10 players + AI kingdoms + minor tribes.
6. **Legibility depth** — theatre-of-war zoom, 11-state dashboard, ward iconography, governor iconography, naval and continental visualization, keep interior as core UI anchor.

## Exit Criterion For The Whole Project

Bloodlines is not complete until every canonical system in the Master Design Doctrine and the session 9 state analysis has moved from `DOCUMENTED` to `LIVE` in the browser reference simulation AND the Unity production lane has reached feature parity with the browser lane on the approved ECS architecture.

This is a multi-session arc. Each session's job is to preserve the full intent, advance at least one vector, keep tests green, and hand the next session a clean pickup point.

## Communication Style

- Be direct. Match the creator's preference for precision over reassurance.
- Do not hedge. If the next action is blocked, say so and pivot.
- Do not summarize away canonical depth. Preserve specificity.
- Never use em dashes in written output (use comma, colon, period, or restructure).
- Never reduce or compress without explicit authorization.

## Closing Instruction

Read the session-start files. Determine the next action. Advance it additively. Verify. Update continuity files. Preserve the full vision. Hand off cleanly.

Begin.

---

**End of reusable continuation prompt. Paste above content verbatim into any future Bloodlines session.**
