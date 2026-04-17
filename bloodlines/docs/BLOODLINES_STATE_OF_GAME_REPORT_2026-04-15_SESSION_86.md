# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 86
Author: Codex

## Scope

Session 86 continued directly from the now-live `Succession Crisis` event spine. It implemented `Covenant Test` as the next real political-event family and then used the newly live Stage 5 faith gate to land the first runtime `Territorial Governance Recognition` layer with coalition counterplay.

This was not a label pass. The new work now affects faith intensity, legitimacy, loyalty, population, apex-covenant access, late-stage faith-unit access, world-pressure composition, Stonehelm timing, territorial targeting, snapshot continuity, and UI legibility.

## Changes Landed

### `Covenant Test` is now live across the covenant ladder (`src/game/core/simulation.js`)

- Added the first live covenant-trial event family on the dynasty political-event spine.
- `Covenant Test` now issues real mandates for all four covenants and both doctrine paths:
  - Old Light light memorial
  - Old Light dark purge
  - Blood Dominion light ceremony
  - Blood Dominion dark binding
  - The Order light codification
  - The Order dark expansion
  - The Wild light grove renewal
  - The Wild dark predator hunt
- Completed covenant structures now feed passive faith-intensity growth instead of leaving the ascent ladder as static doctrine.
- Active Covenant Tests now apply ongoing runtime strain through the shared political-effect lane, touching resource trickle, population growth, stabilization, and battlefield attack output while the trial remains unresolved.
- Success and failure now resolve honestly with intensity, legitimacy, and loyalty consequence plus retry cooldown and continuity history.
- `Covenant Test` is now a real gate for:
  - `Apex Covenant` placement
  - stage-5 apex faith-unit recruitment
  - Divine Right eligibility after older-save migration seams are normalized
- Snapshot export and restore now preserve active Covenant Tests, outcome history, and covenant-pass state.

### Direct rites and Stonehelm covenant ascent are now live (`src/game/core/simulation.js`, `src/game/core/ai.js`)

- Added live direct-rite completion seams where canon supports them:
  - Blood Dominion light ceremony now spends real food and influence.
  - Blood Dominion dark binding now spends real influence, population, and legitimacy.
- Stonehelm now climbs the covenant structure ladder honestly:
  - Wayshrine
  - Covenant Hall
  - Grand Sanctuary
  - Apex Covenant after recognition
- Stonehelm now musters stage-3, stage-4, and apex faith units through that same live ladder instead of leaving faith escalation as inert data.
- Rival AI now reads player Covenant Tests as live openings and compresses military, territorial, raid, and holy-war tempo around them.
- Enemy AI also performs its own direct rite when a live covenant mandate reaches an actionable state.

### First live `Territorial Governance Recognition` layer (`src/game/core/simulation.js`, `src/game/core/ai.js`, `src/game/main.js`)

- Added the first runtime Stage 5 territorial-governance state instead of leaving the lane as doctrine-only.
- Recognition now issues when a kingdom in Final Convergence:
  - holds at least 35 percent of total territory,
  - keeps every held territory stabilized,
  - keeps every held territory at 80 or higher loyalty.
- The sustained hold now tracks real runtime pressure:
  - no held territory may fall below 65 loyalty,
  - no held territory may remain contested,
  - no active holy war may remain open during the sustained window.
- Recognition now emits dual-clock declaration output, records the weakest governed march, tracks sustain seconds, establishes or collapses honestly, and writes last-outcome continuity.
- World-pressure source breakdown now includes `territorialGovernanceRecognition` as a real late-game coalition trigger.
- Stonehelm now reacts to player recognition through sharper attack, territorial, assassination, missionary, and holy-war timing, and redirects territorial pressure toward the weakest governed frontier.
- Enemy recognition is also AI-visible. Stonehelm can shift its own host toward the weakest governed march to preserve an active recognition state.

### Legibility and browser-lane verification (`src/game/main.js`, `src/game/styles.css`)

- The dynasty panel now surfaces active or held `Territorial Governance Recognition`, weakest governed march, territory-share or loyalty state, sustain progress, shortfall, and last outcome.
- Rival-court dynasty readouts now surface hostile governance-recognition state instead of leaving the player blind to enemy late-game governance posture.
- The cycle header and debug overlay now surface governance-recognition timing beside the already-live Covenant Test and Divine Right late-game lines.
- The world-pressure detail lane now explicitly surfaces governance contribution when that source is active.
- Earlier in the same session wave, the hidden-state CSS regression on the launch scene was corrected by enforcing `[hidden] { display: none !important; }`. Session 86 browser verification confirmed that fix remained intact while the new runtime lane landed.

### Runtime coverage (`tests/runtime-bridge.mjs`)

- Added coverage for:
  - player Old Light Covenant Test issuance, completion, UI-legibility exposure, apex gate enforcement, and restore continuity
  - player Blood Dominion direct rite actionability and completion cost
  - enemy Blood Dominion covenant-ladder ascent, live mandate issuance, AI rite completion, and cost application
  - player `Territorial Governance Recognition` issuance, realm-snapshot exposure, world-pressure registration, Stonehelm backlash timing, restore continuity, recognition establishment, and post-recognition collapse

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
- Headless Chromium browser smoke against:
  - `http://127.0.0.1:8057/play.html`
  - `http://127.0.0.1:8057/play.html?briefing=1`
  confirmed:
  - launch scene hidden correctly after runtime boot,
  - resource bar present,
  - 11-pill realm dashboard present,
  - dynasty panel populated,
  - faith panel populated,
  - pause toggle still works,
  - zero console errors,
  - zero page errors,
  - zero failed requests.
- Verification screenshots were preserved at:
  - `reports/session-86-governance-shell-smoke.png`
  - `reports/session-86-governance-briefing-smoke.png`

## Canonical Interdependency Check

Session 86 does not leave either new lane isolated:

1. `Covenant Test` now binds faith intensity, covenant structures, loyalty, legitimacy, population, apex-covenant access, late-stage faith units, Divine Right pacing, AI behavior, and restore continuity.
2. `Territorial Governance Recognition` now binds Stage 5 match progression, territory share, loyalty, stabilization, holy-war state, world pressure, Stonehelm counterplay, message-log declarations, and dynasty-panel legibility.
3. The same cycle deepened both faith and world vectors while preserving dynastic continuity and browser readability.

## Gap Analysis Reclassification

Two Session 9 matrix areas moved materially in Session 86:

- Faith covenant tests and covenant-ascent gating moved from `DOCUMENTED` to `LIVE`. `Covenant Test` is now an active political event with mandate logic across the four covenants, runtime strain, success or failure consequence, direct rite actions where canon supports them, AI awareness, UI legibility, apex gating, and restore continuity.
- Territorial-governance recognition and coalition backlash moved from `DOCUMENTED` to `PARTIAL`. Final Convergence kingdoms can now enter and sustain a real recognition state with world-pressure consequence and AI backlash, but the full Territorial Governance victory seam is still unfinished because governor-seat coverage, Governor's House fulfillment, anti-revolt state, no-kingdom-at-war enforcement beyond current holy-war approximation, and final victory resolution are not yet live.

The broader political-event-architecture row remains `PARTIAL`, but Session 86 materially deepened it from a single dynastic crisis family into a second faith-side event family.

## Session 87 Next Action

Priority order:

1. Deepen `Territorial Governance Recognition` into the first honest Territorial Governance victory-resolution lane by adding the missing governor-seat or Governor's House coverage, anti-revolt validation, stronger no-war enforcement, and final resolution logic.
2. If the governor-coverage path blocks cleanly, implement alliance-threshold pressure and population-acceptance buildup as the parallel Stage 5 sovereignty follow-up.
3. After that, continue the broader political-event family beyond `Succession Crisis` and `Covenant Test`, or deepen multi-kingdom world pressure and neutral-power stage presence if the late-game victory lane stalls.
