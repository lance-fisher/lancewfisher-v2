# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 82
Author: Claude

## Scope

Session 81 carried `Scout Rider` into moving-logistics warfare with convoy interception. Session 82 deepens the same cavalry and logistics chain through convoy escort discipline and post-interdiction reconsolidation. Escort units now carry explicit wagon-binding state, AI differentiates between screened and unscreened recovering convoys for assault timing, the siege-state readiness gate now requires convoy reconsolidation before the breach can resume, and the logistics snapshot and dashboard expose escort discipline directly.

## Changes Landed

### Escort-binding state on units (`src/game/core/simulation.js`)

- Added `escortAssignedWagonId` and `convoyReconsolidatedAt` to all unit creation paths: primary `createUnit`, minor-house founder spawn, production-queue spawn, map-init spawn, and snapshot restore.
- Escort units now carry their assigned wagon id as persistent unit state rather than relying solely on ephemeral AI metadata. This survives snapshot export and restore.
- `convoyReconsolidatedAt` timestamps when a recovering wagon first becomes fully screened by its assigned escorts, providing a legible temporal anchor for when the convoy reformed.

### Convoy reconsolidation readiness in siege state (`src/game/core/simulation.js`)

- `getFactionSiegeState` now computes `unscreenedRecoveringCount`, `convoyReconsolidated`, and `escortedSupplyWagonCount`.
- `readyForFormalAssault` now requires `convoyReconsolidated` instead of the prior `recoveringSupplyWagons.length === 0` gate. This means recovering wagons that are fully screened by escorts no longer block assault readiness, but unscreened recovering convoys do.
- The distinction ensures the breach can resume when escorts close around a recovering convoy, but not before.

### Graded assault timing for screened vs unscreened recovering convoys (`src/game/core/ai.js`)

- The assault-timing branch for recovering convoys is now split into two paths:
  - Unscreened recovering wagons: Stonehelm pulls the assault force back to the regroup anchor and issues a "reforms wagon escort" message. The breach cannot resume.
  - Screened recovering wagons: Stonehelm holds the assault force at the siege stage point and issues a "holds the breach while convoy recovers under escort screen" message. The breach holds position instead of retreating.
- This graded behavior means escort discipline directly affects siege tempo: an unescorted struck convoy costs Stonehelm more assault momentum than a well-screened one.

### AI escort-binding writes and reconsolidation stamping (`src/game/core/ai.js`)

- The supply-patrol assignment loop now writes `escortAssignedWagonId` on each escort unit when assigning it to a wagon.
- Stale escort bindings (dead wagon targets) are cleaned up at the start of each patrol cycle.
- Recovering wagons that become fully screened receive a `convoyReconsolidatedAt` timestamp. The timestamp resets to 0 when the wagon exits recovery.

### Logistics snapshot and dashboard legibility (`src/game/core/simulation.js`, `src/game/main.js`)

- `getRealmConditionSnapshot` logistics block now exposes:
  - `escortedSupplyWagonCount`: number of non-interdicted wagons with at least one screening escort.
  - `unscreenedRecoveringCount`: number of recovering wagons without sufficient escort screen.
  - `convoyReconsolidated`: boolean flag indicating whether all recovering wagons have been screened.
- The logistics pill metadata now surfaces:
  - `escorted N/M` when escorted wagons exist.
  - `unscreened N` when recovering wagons lack escort.
  - `reconsolidating` when `convoyReconsolidated` is false.

### Snapshot export and restore continuity (`src/game/core/simulation.js`)

- `exportStateSnapshot` now includes `escortAssignedWagonId` and `convoyReconsolidatedAt` per unit.
- `restoreStateSnapshot` now restores both fields from the snapshot, preserving escort-discipline state through save/load round-trip.

### Runtime verification coverage (`tests/runtime-bridge.mjs`)

- Added assertions for:
  - `escortedSupplyWagonCount`, `unscreenedRecoveringCount`, and `convoyReconsolidated` presence and type in the realm condition snapshot.
  - Recovering wagons with full escort screen produce `convoyReconsolidated === true`.
  - At least one enemy escort unit carries `escortAssignedWagonId` matching the recovering wagon after AI patrol runs.
  - Escort-binding state (`escortAssignedWagonId`) survives snapshot export and restore.
  - `convoyReconsolidatedAt` survives snapshot export and restore.
  - `escortedSupplyWagonCount` and `convoyReconsolidated` survive in the restored logistics snapshot.

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
- Browser verification confirmed:
  - `play.html` returned `200`,
  - the live shell is visible with the launch scene already hidden,
  - the resource bar is populated,
  - all 11 pressure pills are present,
  - dynasty panel is populated,
  - faith panel is populated,
  - zero runtime errors,
  - zero failed requests.

## Canonical Interdependency Check

Session 82 connects:

1. Escort discipline and siege logistics, because recovering convoys without escort screen now hold the assault line back while screened convoys allow the breach to hold position.
2. Escort discipline and assault timing, because Stonehelm now differentiates between pulling back (unscreened recovery) and holding forward (screened recovery), so the cost of cavalry pressure depends directly on how well the defender disrupts the escort screen.
3. Escort discipline and unit state, because escort units now carry persistent wagon-binding that survives save/restore instead of relying only on ephemeral AI metadata.
4. Escort discipline and legibility, because the logistics pill now exposes escorted wagon count, unscreened recovering count, and reconsolidation status directly.
5. Escort discipline and continuity, because all new unit-level state survives snapshot export, restore, and re-enters the simulation cleanly.

## Gap Analysis Reclassification

No full gap-matrix row moved to `LIVE` in Session 82. The session materially deepened the existing `PARTIAL` military row by carrying escort-binding, reconsolidation readiness, graded assault timing, and logistics legibility into the live convoy-defense chain. The cavalry and logistics integration chain is now deeper but not yet complete: future work could add player-side escort commands, deeper multi-wagon escort formation, and explicit escort-loss consequences.

## Session 83 Next Action

The convoy escort discipline chain is now live. The next highest-leverage action depends on whether the user provides new canonical material to ingest or whether the browser-lane implementation chain should continue.

Candidates in priority order:

1. If new canonical material has been provided (match structure, time system, multiplayer engagement doctrine), ingest it into the canon corpus, cross-reference against existing design surfaces, and flag conflicts for resolution.
2. Continue the cavalry lane into deeper player-side escort command surface or multi-wagon formation if the AI-only chain is considered sufficient.
3. Pivot to a lagging six-vector area if the cavalry lane is considered complete for now: naval depth, deeper multi-kingdom world pressure, or population/settlement infrastructure.
