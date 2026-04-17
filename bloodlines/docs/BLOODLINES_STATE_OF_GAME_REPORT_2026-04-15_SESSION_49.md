# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 49
Author: Codex

## Scope

Save-state dynamic id counter continuity deepened to PARTIAL. Snapshot restore now rebuilds the live prefix-based counters that the runtime actually uses to mint new ids, preventing post-restore id collisions when buildings, units, marriages, lesser houses, or dynasty operations are created later in the match.

## Changes Landed

### Restore-path continuity (`src/game/core/simulation.js`)

- Replaced the stale restore-time `entityIdCounter` rebuild with live `state.counters` reconstruction.
- `restoreStateSnapshot` now rebuilds prefix maxima for:
  - `unit`,
  - `building`,
  - `projectile`,
  - `dynastyOperation`,
  - `lesser-house`,
  - `marriage-proposal`,
  - `marriage`,
  - `bloodline-child`.
- Counter maxima are computed from restored units, buildings, dynasty members, lesser houses, marriages, proposals, and dynasty-operation records.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 49 coverage proving that after snapshot restore:
  - a new building can be placed,
  - the newly created building receives a fresh id,
  - no duplicate building id is introduced.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.
- `python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines` served successfully.
- `http://localhost:8057/play.html` returned `200`, and markup fetch confirmed launch-shell surfaces including `game-shell`, `resource-bar`, `dynasty-panel`, and `faith-panel`.

## Canonical Interdependency Check

Session 49 connects:

1. Save/resume continuity, restored matches can keep creating live runtime entities without id collision.
2. Dynasty systems, lesser houses, marriages, proposals, children, and operations now preserve counter continuity.
3. Territorial and building systems, post-restore construction now mints a fresh building id.
4. Future minor-house growth, later retinue or levy spawning can build on a stable restore path.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Save / resume | DOCUMENTED | PARTIAL (structured restore path live, counter continuity repaired, still no player-facing save UX) |

## Session 50 Next Action

Minor-house territorial levy and retinue growth. The breakaway house can now defend and regroup, and the restore path can safely mint new ids after load. The next canonical layer is local population-backed retinue growth from the held march so the breakaway polity stops being a one-unit actor.
