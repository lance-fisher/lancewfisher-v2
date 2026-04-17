# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 80
Author: Codex

## Scope

Session 79 made `Scout Rider` live as a stage-2 cavalry and infrastructure-raiding lane. Session 80 carries that cavalry seam into direct economic warfare. Scout Riders now harry worked resource seams, route nearby workers toward refuge, depress hostile march loyalty around the seam, expose active harassment through the logistics dashboard, trigger local counter-raid behavior from Stonehelm, and preserve the new pressure through snapshot restore.

## Changes Landed

### Scout Rider harassment data and schema (`data/units.json`, `tests/data-validation.mjs`)

- Added live worker-and-seam harassment fields to `scout_rider`:
  - `nodeHarassDurationSeconds`
  - `workerHarassRadius`
  - `workerRetreatSeconds`
- Extended schema validation so Scout Rider harassment timing and radius must exist and remain positive.

### Live worker and resource-node harassment (`src/game/core/simulation.js`)

- Added live resource-node harassment state:
  - `harassedUntil`
  - `harassedTargetFactionId`
  - `harassedByFactionId`
- Scout Riders can now issue direct raid orders against hostile worked resource seams, not only buildings.
- Harassed seams now:
  - route nearby hostile workers toward the nearest friendly refuge or fallback retreat point,
  - suspend direct gathering on the seam during the harassment window,
  - depress nearby hostile march loyalty through the same local territorial-pressure lane already used by raid shock,
  - reopen to gathering after the harassment window clears.
- Added a dedicated `raid_retreat` combat command so Scout Riders actually withdraw after a harassment pass instead of immediately falling through into idle target reacquisition.
- Added no-reacquire behavior for Scout Riders on raid cooldown against workers and buildings, so harassment resolves as cavalry pressure instead of an unintended post-raid execution loop.
- Snapshot continuity now preserves:
  - active node harassment windows,
  - harassed target-faction identity,
  - harassing faction identity,
  - runtime-created resource seams restored from snapshot payload,
  - the already-live cavalry cooldown state.

### AI harassment and local counter-raid response (`src/game/core/ai.js`)

- Stonehelm Scout Riders now prioritize worked hostile resource seams as a live target class when player workers are exposed.
- Enemy gather-node choice now skips actively harried seams for that faction where alternatives exist.
- Added live harass-site recognition for Stonehelm:
  - active harried resource seams are now tracked as real local pressure sites,
  - nearby non-siege defenders move or attack into that pressure site,
  - the message log surfaces the local response through: `Stonehelm throws local defenders toward the harried seam.`
- Counter-raid behavior now reacts to the same live seam-harassment state the logistics surface reads, so the system is not player-only and not a decorative AI message.

### Command-surface and dashboard legibility (`src/game/main.js`)

- Right-click raid ordering now supports Scout Riders against:
  - hostile resource seams,
  - hostile workers,
  - existing hostile raidable infrastructure.
- The 11-pill logistics dashboard now surfaces live Scout Rider harassment consequence:
  - `harassedResourceNodeCount`
  - `harassedWorkerCount`
  - `nodes harried`
  - `workers routed`
- The logistics pill now shifts:
  - to yellow for active harried seams,
  - to red when workers are actively routed by harassment.

### Runtime verification coverage (`tests/runtime-bridge.mjs`)

- Added runtime coverage for:
  - direct Scout Rider harassment of a worked hostile seam,
  - worker retreat under active node harassment,
  - local march loyalty loss from seam harassment,
  - logistics snapshot exposure of harried seams and routed workers,
  - restore continuity for active node-harassment state,
  - resumed gathering after harassment clears,
  - Stonehelm AI selection of hostile worked seams as cavalry targets,
  - Stonehelm local counter-raid response and message-log legibility.
- Isolated the new Session 80 scenarios from unrelated tribal-board state so the runtime bridge reflects the live cavalry seam rather than ambient sandbox noise.

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
  - launch transitions into the game shell after `Start Skirmish`,
  - resource bar is visible and populated,
  - all 11 pressure pills are present,
  - dynasty panel is populated,
  - faith panel is populated,
  - zero runtime errors,
  - zero failed requests.

## Canonical Interdependency Check

Session 80 connects:

1. Stage-2 cavalry and live gathering throughput, because Scout Riders now deny worked seams and route nearby workers instead of only disabling buildings.
2. Stage-2 cavalry and territorial pressure, because seam harassment shocks nearby hostile march loyalty through the already-live local control-pressure lane.
3. Stage-2 cavalry and logistics legibility, because the 11-pill dashboard now reports harried seams and routed workers directly.
4. Stage-2 cavalry and AI or world reactivity, because Stonehelm now chooses worked seams as targets and throws local defenders toward active harassment sites.
5. Stage-2 cavalry and restore continuity, because node harassment windows and targeted-faction metadata now survive snapshot round-trip.

## Gap Analysis Reclassification

No full gap-matrix row moved from `PARTIAL` to `LIVE` in Session 80 because broader ambush, convoy interception, and mobile supply-route warfare still remain open. Session 80 materially deepened these existing `PARTIAL` rows:

| System | Previous | Current |
|---|---|---|
| Raid, blockade, encirclement, infrastructure sabotage as viable tools | PARTIAL | PARTIAL, but now strengthened through live Scout Rider worker and resource-seam harassment |
| Military operations: raid, ambush, supply-line strike | PARTIAL | PARTIAL, with structured seam harassment and local counter-raid response now live first layer, while ambush and convoy interception remain open |
| Water denial as emergent strategy | PARTIAL | PARTIAL, because seam harassment now pressures the same logistics economy that already feeds water-support structures and supply continuity |

## Session 81 Next Action

Keep the cavalry chain moving before pivoting away. The next highest-leverage unblocked action is to extend Scout Rider into direct interception of `supply_wagon` and other live field-logistics carriers so cavalry can strike moving sustainment, not only fixed infrastructure and worked seams. That next layer should touch siege-supply continuity, field-water sustainment, AI escort or counter-screen behavior, dashboard or message-log legibility, and restore continuity where relevant.
