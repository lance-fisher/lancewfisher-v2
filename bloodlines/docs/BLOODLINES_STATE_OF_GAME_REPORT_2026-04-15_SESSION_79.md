# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 79
Author: Codex

## Scope

Session 78 deepened Hartvale's local-defense support seam. Session 79 returns to the standard military lane and activates `Scout Rider` as the first live stage-2 cavalry and infrastructure-raiding unit. This is not a roster-only unlock. Scout Riders now train from a real `stable`, disable hostile infrastructure for a live duration, strip enemy stores, shock nearby marches, cut logistics and water support, trigger Stonehelm raid behavior, surface raid pressure in the command and dashboard layers, and survive snapshot restore.

## Changes Landed

### Scout Rider data and production lane (`data/units.json`, `data/buildings.json`)

- `scout_rider` is now `prototypeEnabled: true`.
- Added live raid fields to `scout_rider`:
  - `raidDurationSeconds`
  - `raidCooldownSeconds`
  - `raidLoyaltyDamage`
  - `raidDefenseRadius`
- Added `stable` as a real buildable production seat with live cost, health, build time, footprint, and `scout_rider` trainability.
- Marked vulnerable logistics and sustainment structures as `scoutRaidable` and assigned concrete `raidLoss` payloads:
  - `farm`
  - `well`
  - `lumber_camp`
  - `mine_works`
  - `quarry`
  - `iron_mine`
  - `supply_camp`

### Live raid runtime (`src/game/core/simulation.js`)

- Added a real raid command path through `issueRaidCommand(...)`.
- Added scout-rider raid helpers for:
  - target positioning,
  - defense screening,
  - retreat orders,
  - local loyalty shock,
  - resource-store stripping,
  - raid-status formatting.
- Scout Riders now:
  - move into raid range,
  - refuse defended targets,
  - disable hostile raidable buildings for a live time window,
  - apply direct resource loss,
  - apply local march loyalty loss near the strike,
  - retreat after the raid instead of remaining in place.
- Active raid state now changes other already-live systems:
  - raided `supply_camp` stops counting as a live siege-logistics anchor,
  - raided `well` and other water-support structures stop contributing to field-water sustainment,
  - raided dropoff buildings stop serving as live gather-drop points,
  - the logistics realm snapshot now exposes raid pressure directly.
- Raid state now persists through save and restore:
  - units serialize `raidCooldownRemaining`,
  - buildings serialize `raidedUntil`.

### AI cavalry lane (`src/game/core/ai.js`)

- Stonehelm now recognizes `stable` as a live follow-up production seat after the initial siege shell is in place.
- Enemy AI now queues `scout_rider` from a completed `stable`.
- Added live raid-target selection for the enemy:
  - prioritizes supply camps, wells, and extractive infrastructure,
  - prefers softer targets,
  - issues the real scout-rider raid command,
  - surfaces raid launches in the message log.
- Relaxed the command-hall dependency so scout-rider raids can still launch from AI even when the siege shell is absent.

### Command-surface and renderer legibility (`src/game/main.js`, `src/game/core/renderer.js`, `play.html`)

- Worker build order now includes `stable`.
- Right-click now issues a real raid order when the selection is composed of Scout Riders and the target is hostile raidable infrastructure.
- Selection readout now surfaces scout-rider raid cooldown.
- Building readout now surfaces active raid disable time.
- The 11-pill logistics surface now reports live raid pressure:
  - raided infrastructure count,
  - raided supply-camp count,
  - raided water-anchor count,
  - raided dropoff count.
- Renderer now includes:
  - `stable` silhouette,
  - `light-cavalry` unit silhouette,
  - visible raided-building overlay state.
- `play.html` now declares an inline empty favicon to prevent a browser-surface `favicon.ico` 404 during verification.

### Verification coverage (`tests/data-validation.mjs`, `tests/runtime-bridge.mjs`)

- Data validation now asserts:
  - live Scout Rider schema,
  - live Stable trainability,
  - raidable infrastructure metadata.
- Runtime bridge now covers:
  - Stable production of Scout Riders,
  - direct raid disable window,
  - raid-driven resource loss,
  - local march loyalty shock,
  - logistics snapshot legibility for raid pressure,
  - restore continuity for active raid state,
  - Stonehelm AI raid-order issuance through the live command seam.

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
- Headless Edge CDP verification confirmed:
  - launch auto-transitions into the game shell,
  - resource bar is populated,
  - 11 pressure pills are present,
  - dynasty panel is populated,
  - faith panel is populated,
  - zero runtime exceptions,
  - zero failed requests after the favicon fix.

## Canonical Interdependency Check

Session 79 connects:

1. Stage-2 cavalry and logistics, because Scout Riders can now disable supply camps, cut dropoffs, and strip stores instead of only adding another combat body.
2. Stage-2 cavalry and water sustainment, because raided wells and other water-support structures no longer feed the live field-water lane while the raid window holds.
3. Stage-2 cavalry and territorial pressure, because successful raids now directly shock loyalty in nearby hostile marches.
4. Stage-2 cavalry and AI or world reactivity, because Stonehelm now builds `stable`, produces Scout Riders, and launches real raids through the same runtime seam available to the player.
5. Stage-2 cavalry and legibility, because raid cooldown, building disable state, raid overlays, message-log announcements, and logistics-pill raid pressure are all live.
6. Stage-2 cavalry and continuity, because both unit cooldown and building disable state survive snapshot restore.

## Gap Analysis Reclassification

No full gap-matrix row moved from `PARTIAL` to `LIVE` in Session 79 because ambush, blockade, and broader supply-route warfare still remain open debt. Session 79 materially advanced these rows:

| System | Previous | Current |
|---|---|---|
| Water denial as emergent strategy | PARTIAL | PARTIAL, but now strengthened through live Scout Rider well and sustainment-structure raids |
| Raid, blockade, encirclement, infrastructure sabotage as viable tools | PARTIAL | PARTIAL, but now strengthened through live Scout Rider infrastructure raids that disable hostile buildings and strip stores |
| Military operations: raid, ambush, supply-line strike | PARTIAL (tribe raids), MISSING (structured operations) | PARTIAL, with structured Scout Rider raid and supply-line strike now live first layer, while ambush remains missing |

## Session 80 Next Action

Keep the new cavalry lane moving before pivoting away. The next highest-leverage unblocked action is to extend Scout Rider from infrastructure raids into direct resource-node and worker harassment, then add the first honest counter-raid response so gathering pressure, local defense, and raid detection all matter in the same live lane.
