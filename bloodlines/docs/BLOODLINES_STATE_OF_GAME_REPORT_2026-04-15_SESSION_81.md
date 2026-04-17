# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 81
Author: Codex

## Scope

Session 80 carried `Scout Rider` into worked seams and worker routing. Session 81 keeps the same cavalry lane moving into live moving-logistics warfare. Scout Riders can now strike hostile `supply_wagon` columns in motion, cut already-live siege and field-water sustainment, force convoy retreat, shock nearby hostile loyalty, trigger Stonehelm convoy targeting and local counter-screen response, surface the pressure through the logistics dashboard and message log, and preserve the new interdiction state through snapshot restore.

## Changes Landed

### Moving-logistics interception data and schema (`data/units.json`, `tests/data-validation.mjs`)

- Extended `supply_wagon` with explicit moving-logistics interception metadata:
  - `movingLogisticsCarrier`
  - `raidInterdictionSeconds`
  - `raidLoss`
- Updated data validation so the live convoy-interdiction fields remain present and positive.

### Live convoy interdiction runtime (`src/game/core/simulation.js`)

- Added Scout Rider interception against hostile moving logistics carriers, starting with `supply_wagon`.
- Added convoy-interdiction helpers for:
  - moving-carrier detection,
  - convoy-target raid gating,
  - linked supply-camp recovery routing,
  - convoy loss profile resolution,
  - active interdiction timing and expiry cleanup.
- Successful convoy strikes now:
  - set a real interdiction window on the wagon,
  - record the striking faction,
  - strip real `food`, `water`, and `wood`,
  - force the convoy into retreat toward a linked or fallback supply anchor,
  - shock nearby hostile march loyalty,
  - emit a live message-log entry instead of silent state mutation.
- Interdicted wagons now materially affect already-live systems:
  - `tickSiegeSupportLogistics(...)` excludes them from active siege sustainment and marks them as `interdicted`,
  - field-water support ignores actively interdicted wagons,
  - siege-state readiness now exposes both active and interdicted wagon counts and requires at least one non-interdicted wagon for formal-assault readiness.
- Snapshot continuity now preserves convoy state through:
  - `logisticsInterdictedUntil`
  - `interdictedByFactionId`
- Restore-safe unit creation now initializes the new convoy-interdiction state for fresh units as well as restored units.

### AI convoy targeting and counter-screen reaction (`src/game/core/ai.js`)

- Stonehelm Scout Riders now recognize hostile moving convoys as a preferred raid class before reverting to fixed logistics or seam harassment.
- Local AI response now treats an interdicted convoy as a live harass site, so defenders move or attack toward a struck wagon instead of ignoring the pressure.
- Stonehelm assault behavior now reacts to live convoy loss directly:
  - when a convoy is struck during keep pressure, the assault line can pull back to screen the convoy before resuming the breach.
- Supply patrol behavior now keeps escort units recentering around wagon movement instead of drifting away after a one-time order.

### Command-surface and dashboard legibility (`src/game/main.js`)

- Right-click raid orders for Scout Riders now accept hostile moving logistics carriers, not just buildings, seams, and workers.
- The logistics pill now exposes convoy cuts directly through:
  - `interdictedSupplyWagonCount`
  - live `convoy cut` text in the primary value
  - convoy-cut metadata in the detail line

### Runtime verification coverage (`tests/runtime-bridge.mjs`)

- Added runtime coverage for:
  - direct convoy interception and store stripping,
  - live convoy retreat orders,
  - local march loyalty shock from convoy strikes,
  - snapshot exposure of convoy cuts,
  - restore continuity for active wagon interdiction,
  - live lapse of ram sustainment after convoy loss,
  - field-water sustainment interruption in an isolated supply-only scenario,
  - Stonehelm Scout Rider convoy targeting,
  - Stonehelm local counter-screen response to a struck convoy.

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
- Headless verification confirmed:
  - `play.html` returned `200`,
  - the live shell is visible with the launch scene already hidden,
  - the resource bar is populated,
  - all 11 pressure pills are present,
  - dynasty panel is populated,
  - faith panel is populated,
  - zero runtime errors,
  - zero failed requests.

## Canonical Interdependency Check

Session 81 connects:

1. Stage-2 cavalry and siege logistics, because moving convoy cuts now directly degrade formal assault readiness and active siege sustainment.
2. Stage-2 cavalry and field-water sustainment, because interdicted wagons no longer hydrate already-live field units through the wagon support lane.
3. Stage-2 cavalry and territorial pressure, because convoy strikes now shock nearby hostile march loyalty instead of existing as isolated store damage.
4. Stage-2 cavalry and AI reactivity, because Stonehelm now raids hostile convoys, screens struck convoys locally, and can pull the assault line back to protect live sustainment.
5. Stage-2 cavalry and legibility, because convoy cuts now surface in the logistics pill and message log.
6. Stage-2 cavalry and continuity, because active convoy interdiction survives snapshot restore with faction provenance intact.

## Gap Analysis Reclassification

No full gap-matrix row moved to `LIVE` in Session 81. The session materially deepened existing `PARTIAL` military and civilizational rows by carrying Scout Rider pressure into moving convoys, live siege-supply lapse, field-water interruption, loyalty shock, AI convoy targeting, and local counter-screen response. Escort discipline and post-interdiction reconsolidation remain open.

## Session 82 Next Action

Keep the cavalry and logistics chain moving before pivoting away. The next highest-leverage unblocked action is convoy escort discipline and post-interdiction reconsolidation:

- bind Stonehelm escort screens more explicitly to active wagons or wagon groups,
- make escorts reform around retreating or recovering convoys,
- make assault timing read whether convoy protection and reconsolidation are complete,
- surface that reconsolidation state through existing message-log or logistics legibility,
- preserve the new escort behavior through runtime coverage and restore continuity where relevant.
