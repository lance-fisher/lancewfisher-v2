# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 85
Author: Codex

## Scope

Session 85 continued directly from the live match-progression, imminent-engagement, and Divine Right spine. It converted the first political-event lane from canon-only into live browser runtime through `Succession Crisis`.

This was not a decorative dynastic label pass. The new lane now touches real bloodline death, legitimacy, local loyalty, economy, stabilization, combat output, AI attack timing, AI marriage posture, player actionability, snapshot continuity, and dashboard legibility.

## Changes Landed

### First live dynasty political-event architecture (`src/game/core/simulation.js`)

- Added `dynasty.politicalEvents.active` and `dynasty.politicalEvents.history` as first-class runtime state on faction dynasties.
- Added shared severity profiles, trigger-profile construction, escalation timing, history retention, and serialization helpers for dynastic political events.
- Added faction-scoped political-event effect resolution so active events can change real simulation behavior instead of existing as inert ledger state.
- Snapshot export and restore now preserve active or historical dynasty political events and rebuild counter continuity for restored event ids.

### `Succession Crisis` trigger, pressure, and consolidation (`src/game/core/simulation.js`, `src/game/main.js`)

- Added live `Succession Crisis` triggering when a ruling `head_of_bloodline` falls and the resulting succession is weak, disputed, underage, or absent.
- Claim strength now reads real runtime context instead of fixed lore labels:
  - current ruler presence,
  - interregnum state,
  - age and maturity,
  - rival claimant count,
  - dynastic role weight.
- A live crisis now applies immediate territorial loyalty shock and then ongoing daily pressure through:
  - legitimacy drain,
  - local loyalty drain,
  - reduced passive and control-point resource trickle,
  - reduced population growth,
  - reduced march stabilization,
  - reduced battlefield attack output.
- Unresolved crises now escalate one severity step at fixed in-world intervals instead of remaining static.
- Added real consolidation terms and a live `Consolidate Succession` action. Resolution consumes actual gold and influence, restores legitimacy and local loyalty, writes dual-clock declaration text, and moves the event into dynastic history.
- The dynasty panel now surfaces the active crisis with severity, trigger reason, claimant count, runtime penalties, escalation timing, and the real consolidation action.
- Rival-court dynasty readouts now also expose hostile succession crisis state instead of leaving the player blind to the same pressure on the enemy court.

### AI and world response (`src/game/core/ai.js`)

- Stonehelm now reads a live player succession crisis as exploitable realm weakness instead of ignoring it.
- Player succession instability now compresses rival territorial and military tempo, and it also sharpens enemy marriage timing so the dynastic lane and diplomacy lane react to the same crisis.
- Enemy courts now also suffer their own succession crises honestly. An active enemy crisis slows offensive tempo, preserves self-stabilization priority, and drives AI self-resolution through the same consolidation terms the player uses.

### Runtime coverage (`tests/runtime-bridge.mjs`)

- Added player-crisis coverage for trigger path, immediate loyalty shock, passive-economy penalty, AI aggression response, snapshot exposure, and restore continuity.
- Added player consolidation coverage for cost payment, legitimacy recovery, local loyalty recovery, event-history persistence, and dual-clock declaration output.
- Added enemy self-resolution coverage proving AI consolidation of its own crisis through the shared terms seam.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.
- `python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines` served `play.html`.
- Headless Edge browser smoke against `http://127.0.0.1:8057/play.html` confirmed shell markup for the resource bar, realm dashboard, dynasty panel, faith panel, and command panel without app-level request failure.
- The browser smoke still emits the prior browser-internal fallback task-provider stderr line. No Bloodlines runtime failure surfaced from that run.

## Canonical Interdependency Check

Session 85 does not leave the political-event lane isolated:

1. `Succession Crisis` now binds bloodline death, succession, legitimacy, local loyalty, economy, stabilization, combat strength, and dual-clock declarations into one live system.
2. Rival AI now reads that dynastic instability as a real war and diplomacy opportunity, while enemy courts also have to manage the same instability internally.
3. The dynasty panel, realm snapshot, and debug surface now expose the new event state so the player can read both the cause and the consequences of dynastic fracture.

## Gap Analysis Reclassification

Two Session 9 matrix areas moved materially forward in Session 85:

- First political-event architecture on the match-progression and dual-clock spine moved from `DOCUMENTED` to `PARTIAL`. The browser reference simulation now has active and historical dynasty political events, first-event escalation logic, real simulation penalties, AI response, UI legibility, and restore continuity. Covenant Test, broader event queues, alliance fractures, and non-dynastic political-event families remain unfinished.
- Succession interface and succession-pressure legibility moved from `DOCUMENTED` to `PARTIAL`. The dynasty panel now surfaces active succession crisis severity, trigger reason, claimant count, escalation timing, runtime modifiers, consolidation terms, and hostile-court crisis state. A dedicated full five-impact succession interface still remains unfinished.

## Session 86 Next Action

Priority order:

1. Implement `Covenant Test` as the next live political-event follow-up on the new event spine, wiring it into faith intensity, conviction, unrest, loyalty, AI timing, and legibility.
2. Deepen Stage 5 victory follow-up after Divine Right through territorial-governance or alliance-threshold pressure and stronger coalition counterplay.
3. If those lanes block, continue deeper world-depth follow-up through multi-kingdom pressure, neutral-power stage presence, or naval-world integration.
