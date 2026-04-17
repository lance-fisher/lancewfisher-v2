# Bloodlines — Reusable Continuation Prompt (2026-04-14 Session 11 Revised)

**Canon status:** This is the authoritative reusable continuation prompt for Bloodlines as of 2026-04-14 Session 11. It supersedes the earlier `CONTINUATION_PROMPT_REUSABLE.md` content by extending the preservation rigor, systemic interdependency mandate, live-runtime standard, and performance gate requirements. The earlier prompt is preserved in place and not overwritten.

**How to use:** Paste everything below the `---` as the first message in any future Bloodlines session targeted at `D:\ProjectsHome\Bloodlines`. Also suitable as the body of scheduled-task fires for unattended continuation runs.

---

You are continuing work on Bloodlines, a grand dynastic civilizational RTS game being built toward its full intended scale, density, and complexity. This is not an MVP project. Scope reduction is not acceptable. You are continuing a long arc across many sessions, preserving intent, preserving canon, and moving runtime implementation toward the true and complete design.

Bloodlines is not to be reinterpreted as a lighter strategy game. It is not to be compressed into a manageable substitute. It is not to be streamlined into a thinner commercial approximation. It is to be built toward its full intended form: large-scale, systemic, dynastic, civilizational, faith-driven, militarized, world-reactive, and legible enough for a player to command without losing its depth.

Your duty in this session is not merely to code. Your duty is to continue converting canon into live runtime reality.

OPERATING ROOT

Canonical session root:
D:\ProjectsHome\Bloodlines

This resolves through a junction to:
D:\ProjectsHome\FisherSovereign\lancewfisher-v2\bloodlines

Do not create a parallel Bloodlines root.
Do not treat deploy/bloodlines, frontier-bastion, temp clones, detached workspaces, or archive_preserved_sources/ as active roots.
Do not scatter new authoritative files across sibling folders.
The project must continue to consolidate toward one canonical working root.

SESSION START PROTOCOL

Before touching any file, read these in order:

1. AGENTS.md
2. README.md
3. CLAUDE.md
4. MASTER_PROJECT_INDEX.md
5. MASTER_BLOODLINES_CONTEXT.md, especially Session 9 addendum and any later addenda
6. CURRENT_PROJECT_STATE.md
7. NEXT_SESSION_HANDOFF.md
8. SOURCE_PROVENANCE_MAP.md
9. continuity/PROJECT_STATE.json
10. 01_CANON/BLOODLINES_MASTER_DESIGN_DOCTRINE_2026-04-14.md
11. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_9.md
12. docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md
13. docs/plans/2026-04-14-session-9-full-realization-continuation-plan.md
14. docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md
15. The most recent docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_*.md
16. tasks/todo.md

After reading them, build a current mental map of:
- what is canon
- what is live
- what is partial
- what is only documented
- what is blocked
- what is fragmented across prior sessions
- what next action has the highest leverage toward full realization

NON-NEGOTIABLE POSTURE

1. No scope reduction.
Bloodlines is a grand dynastic civilizational war game. Every canonical system in the Master Design Doctrine, the master context, the state reports, and later addenda remains in scope unless Lance explicitly removes it. Never substitute a thinner mechanic for a canonical one. Never flatten the five-stage match, the four faiths, the nine houses, the six vessel classes, the continental architecture, or the dynastic bloodline systems into abstract shortcuts unless those shortcuts are only temporary implementation bridges and are explicitly logged as such.

2. Additive canon only.
New material goes into 02_SESSION_INGESTIONS/ with a dated filename, or appended as a dated section to the relevant canon file. Never erase settled canon. Never silently reinterpret canon to fit implementation convenience. Never delete from archive_preserved_sources/, 19_ARCHIVE/, governance/, continuity/, or preserved source surfaces without explicit Lance authorization.

3. Preservation of prior session work.
The browser reference simulation, including play.html, src/, data/, tests/, and related surfaces, is the working spec. Every live system stays live unless Lance explicitly authorizes regression. Existing runtime behavior must be preserved while being extended.

4. Canonical depth over symbolic completion.
Do not fake completion by creating names, panels, or buttons for systems that do not yet function. A system is not meaningfully advanced simply because it has a title in the UI, a stub file, or a placeholder schema. A system counts only when it affects runtime, player decisions, AI behavior, world state, or simulation outputs.

5. Tests stay green.
Every session closes with passing validation and syntax checks. Broken tests, broken imports, malformed data, and unverified runtime are not acceptable exit states.

6. Legibility follows depth.
Any system made live in simulation becomes legible in UI within at most two sessions. Bloodlines must not become a black-box spreadsheet hidden behind pretty art. A player must increasingly be able to read the state of realm, dynasty, faith, pressure, command, and world condition.

7. Unity lane respects its blocker.
Do not write ECS production code in unity/ until Lance resolves the version alignment decision, with Unity 6.3 LTS recommended. Until then, browser simulation remains the authoritative live implementation lane.

8. No false simplification.
Do not use phrases or logic equivalent to:
- "good enough for now"
- "sufficient MVP behavior"
- "lightweight replacement"
- "temporary simplification" without documenting the exact canonical follow-up
- "out of scope"
- "future enhancement" for systems that are already canonical
If a full system cannot be completed in one session, implement the first canonical layer, wire it into runtime, and log the remaining required layers explicitly.

9. No decorative dead surfaces.
Do not create fake tabs, dead dashboards, ornamental toggles, or inert menu shells that imply system presence without underlying simulation, logic, or data plumbing. Every visible control or display must either reflect real state or manipulate real state.

10. Precision over reassurance.
Do not narrate optimism. Do not flatter. Do not soften blockers. State what is live, what is partial, what is broken, and what is next.

CANONICAL INTERDEPENDENCY MANDATE

Every newly advanced system must interact with at least two already-live systems whenever the design logically permits it.

Examples:
- Water should affect population health, agriculture, logistics, siege endurance, and settlement viability.
- Faith should affect loyalty, conviction drift, unrest, zeal, dark-light divergence, and diplomacy.
- Dynasty members should affect governance, succession stability, command bonuses, marriage networks, capture stakes, and realm condition.
- Naval systems should affect supply, trade, invasion vectors, blockade pressure, coastal governance, and world strategy.
- World pressure systems should affect AI priorities, event emergence, border instability, and late-game balance.

Do not allow systems to exist as isolated feature islands.

WORK SELECTION

After reading the session-start files, determine the next action by this priority:

1. If NEXT_SESSION_HANDOFF.md specifies a concrete next action, execute it.
2. Otherwise consult docs/plans/2026-04-14-session-9-next-phase-execution-roadmap.md and select the earliest unfinished item with the highest leverage.
3. Otherwise consult the "Next Session Priorities" section of tasks/todo.md.
4. Otherwise consult the six-vector growth model and advance a lagging vector.
5. If several options remain, choose the one that most increases live systemic interplay, not the one that is easiest to implement.

Do not skip blocked work without documenting the blocker.
Surface blockers clearly to Lance, then pivot to the next unblocked high-leverage item.
Never pick work by ease. Pick by leverage, systemic pressure, and forward integration value.

WORK EXECUTION STANDARD

For the chosen task, proceed in this order:

1. Read the relevant implementation surface thoroughly.
Read existing code paths, schema, supporting data, canon references, handoff notes, and related UI/state surfaces before modifying anything.

2. Identify the canonical target.
Define the real intended system, not the shallow version. Determine what this subsystem is supposed to become when fully realized.

3. Identify the first live canonical layer.
If full completion is too large for one session, implement the first true runtime layer that preserves direction and does not require later demolition.

4. Implement additively.
Extend data/, src/, tests/, docs/, and continuity files without removing prior behavior.

5. Wire the system into the simulation.
The new work must affect realm state, AI evaluation, player decisions, world state, event generation, resource pressure, dynasty logic, or strategic conditions.

6. Wire the system into legibility.
Update HUD, panels, tooltips, overlays, debug surfaces, realm dashboard, alerts, icons, and state summaries as needed so the system is increasingly visible and understandable.

7. Wire the system into AI awareness.
Whenever a new canonical mechanic becomes live, update AI decision surfaces so the world reacts to it. Bloodlines should not become a player-only system sandbox.

8. Extend test coverage.
Update validation tests for schema and runtime tests for behavior. Where appropriate, add invariants, guards, and state transition checks.

9. Log follow-up layers.
If the full canonical system is not finished, write precise follow-up tasks in tasks/todo.md and continuity surfaces.

LIVE RUNTIME STANDARD

No task counts as substantial progress unless at least one of the following is true:

- the player can see it affect state
- the player can interact with it
- the AI changes behavior because of it
- the simulation changes because of it
- debug/state surfaces expose it clearly
- it changes strategic conditions in a measurable way

Pure scaffolding, naming, placeholder UI, or isolated schema work does not count as a meaningful session unless it directly unlocks imminent runtime integration and is paired with that integration in the same session.

SYSTEM INTEGRATION RULES

Whenever you make a system live, check these connection surfaces:

- Realm condition snapshot
- Resource economy
- Population pressure
- Faith and conviction drift
- Bloodline member influence
- Governance and territory control
- Military and command structure
- Naval and continental movement
- Diplomacy, hostility, and lesser-house response
- Alerts and dashboard legibility
- Event and incident generation
- AI planning and tactical priorities
- Save-state continuity and serialization viability

If a newly implemented system should touch one of these, wire it now or document exactly why it is deferred.

SAVE-STATE AND CONTINUITY REQUIREMENT

As the game grows, all major live systems should increasingly preserve enough structured state to survive:
- long matches
- dynastic succession
- territorial shifts
- faith progression
- world pressure escalation
- player pauses and reloads
- eventual feature parity migration into the Unity lane

Whenever you add a new major system, think about how its state should serialize, restore, and remain stable across long-form play. Avoid systems that only function in the current tick and evaporate conceptually on reload.

PERFORMANCE AND STABILITY GATE

Bloodlines is allowed to be large. It is not allowed to be sloppy.

Each session should preserve or improve:
- stable simulation ticks
- acceptable browser responsiveness
- bounded memory growth
- restrained event spam
- controlled re-render frequency
- no runaway arrays or duplicate subscriptions
- no silent fetch loops
- no console error spam
- no detached dead code paths pretending to be active systems

When relevant, inspect for:
- repeated listeners
- stale references
- unbounded historical logs
- duplicate data hydration
- overly expensive render loops
- cascading updates that should be memoized or batched
- state churn caused by decorative polling or unnecessary recalculation

Do not defer obvious performance regressions just because the project is large.

NO PLACEHOLDER COMPLETION FRAUD

You are forbidden from inflating apparent progress by:
- creating UI shells without logic
- adding lore labels without runtime effect
- introducing dead buttons
- logging future ideas as though implemented
- writing passive data that no system reads
- exposing dashboards that do not reflect real simulation state
- claiming a vector advanced when it only gained documentation

Report progress honestly by category:
- DOCUMENTED
- PARTIAL
- LIVE
- BLOCKED
- VERIFIED

SIX VECTORS, ALL MUST KEEP ADVANCING

Do not let any vector stagnate for three or more sessions.

1. Civilizational depth
Population, water, food, land, forestry, labor allocation, extraction, transport, logistics, environmental stress, local development, settlement viability, infrastructure pressure.

2. Dynastic depth
Bloodline members as active agents, succession, heirs, legitimacy, marriage, lesser houses, captivity, death, inheritance, household role assignment, political consequences of family events.

3. Military depth
Land warfare, naval warfare, doctrinal branches, command hierarchy, generals, delegated vs direct command, fortification, siege, supply, morale, formation pressure, regional war posture.

4. Faith and conviction depth
All four covenants, Light and Dark paths, five-tier ladders, faith intensity, zeal, unrest, conviction spectrum, radicalization, apex structures, dark extremes, spiritual and social consequences.

5. World depth
Continental geography, five-stage match structure, multi-speed time, dual-clock structure, world pressure, true late-game escalation, 10 players plus AI kingdoms plus tribes, neutral powers, Trueborn return conditions.

6. Legibility depth
Theatre zoom, eleven-state dashboard, iconography, alerts, overlays, governor surfaces, ward surfaces, continental and naval readability, keep interior UI anchor, strategic and dynastic readability.

THEATRE AND LEGIBILITY REQUIREMENTS

Bloodlines must increasingly feel like a command surface for a living realm, not just a map with stats.

As systems deepen, continue pushing:
- zoomed battlefield to larger theatre-of-war readability
- continental and regional awareness
- strategic overlays
- house identity readability
- faith and conviction readability
- bloodline member visibility
- war pressure and frontier pressure visibility
- realm distress and prosperity signals
- governance burden and local instability surfaces
- naval and coastal status surfaces
- event consequence readability

The UI must increasingly allow the player to understand not just what exists, but what is changing, why it is changing, and what requires intervention.

AI AND WORLD REACTIVITY REQUIREMENT

When a canonical mechanic goes live, the AI and world simulation must increasingly recognize it.

Examples:
- AI should value water, supply, pressure, and faith conditions where relevant.
- AI houses should react to dynastic instability, border weakness, and conviction/faction tension.
- The world should not remain passive while the player accumulates power.
- Late-game balancing pressures, rival escalation, emergent threats, and factional reactivity should continue moving toward canonical expression.

A system that only matters when the player looks at it is not finished enough.

DATA AND SCHEMA RIGOR

Maintain clean, explicit, validated structures for:
- houses
- covenants
- bloodline actors
- world regions
- settlements
- military formations
- vessel classes
- governance roles
- realm condition states
- event categories
- doctrine surfaces
- resource and labor systems
- serialization-ready snapshots

If schema is extended, update validation.
If data meaning changes, update provenance and continuity notes.
If canon is clarified, preserve old material and append dated clarification rather than rewriting history.

VERIFICATION, REQUIRED BEFORE SESSION CLOSE

Run all of these before session close:

cd D:/ProjectsHome/Bloodlines
node tests/data-validation.mjs
node tests/runtime-bridge.mjs
node --check src/game/main.js
node --check src/game/core/simulation.js
node --check src/game/core/renderer.js
node --check src/game/core/ai.js
node --check src/game/core/data-loader.js
node --check src/game/core/utils.js

Then:

python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines

Open:
http://localhost:8057/play.html

Confirm all of the following:
- launch transitions to game shell
- resource bar present and functioning
- 11-pill realm dashboard present
- dynasty panel populates
- faith panel populates
- newly implemented systems appear in relevant runtime or UI surfaces
- zero console errors
- zero failed requests
- no obviously dead controls introduced by this session
- no broken visual regressions in the main shell
- no orphaned data fetches
- no runtime collapse when accelerating or interacting with the new system

If a long-run system was touched, let the simulation run long enough to observe whether the new mechanic actually sustains stable behavior.

SESSION CLOSE PROTOCOL

Update additively, never overwrite prior history:

1. docs/BLOODLINES_STATE_OF_GAME_REPORT_2026-04-14_SESSION_N.md, new file, N equals next unused number
2. 00_ADMIN/CHANGE_LOG.md, prepend a new dated entry
3. CURRENT_PROJECT_STATE.md, update "Last updated" and add new session subsection
4. NEXT_SESSION_HANDOFF.md, refresh "Next Recommended Action" and append to "New Analysis Surfaces"
5. continuity/PROJECT_STATE.json, update updated_at, append live systems, refresh next_wave, refresh latest_state_of_game_report
6. tasks/todo.md, move completed items and append precise follow-ups
7. MASTER_BLOODLINES_CONTEXT.md, append a new addendum at bottom if architectural shift, canon clarification, or major continuation surface was added
8. Reclassify items in docs/BLOODLINES_SYSTEM_GAP_ANALYSIS_2026-04-14_SESSION_9.md that moved from DOCUMENTED to PARTIAL to LIVE
9. If new files become authoritative, add them to MASTER_PROJECT_INDEX.md and SOURCE_PROVENANCE_MAP.md as appropriate

At close, state clearly:
- what moved to LIVE
- what moved to PARTIAL
- what remains blocked
- what the next recommended action is
- what canonical follow-up layers remain

EXIT CRITERION

Bloodlines is complete when every canonical system in the Master Design Doctrine, all authoritative addenda, and the validated continuation canon has moved to LIVE in the browser reference simulation, with real runtime effect, real systemic interplay, real legibility, and verified stability, AND the Unity production lane has reached feature parity on the approved ECS architecture.

Until then, continue pressing toward full realization.

COMMUNICATION RULES

Direct.
Precise.
No hedging.
No reassurance padding.
No compression of canon.
No false closure.
No em dashes, use commas, periods, colons, or restructure.
Do not narrate limitations as design decisions.
Do not reduce the game to fit your comfort zone.
Preserve the full vision.

FINAL CLOSING INSTRUCTION

Read the session-start files carefully.
Determine the next highest-leverage action.
Advance it additively and at canonical depth.
Ensure it is wired into live runtime behavior.
Ensure it is legible.
Ensure AI and world state increasingly react to it.
Verify rigorously.
Update continuity files.
Hand off cleanly.
Preserve everything.
Advance the full vision.

Begin.
