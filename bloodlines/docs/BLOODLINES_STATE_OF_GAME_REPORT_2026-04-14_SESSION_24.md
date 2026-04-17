# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 24
Author: Claude

## Scope

Save-state serialization primer LIVE. `exportStateSnapshot(state)` returns a deterministic structured snapshot covering the full live simulation state (meta, realm cycle, world, factions, units, buildings). Canonical Decision 16 (save/resume) gets its foundational structure. Restore-from-snapshot is NOT yet wired — that is a later session.

## Changes Landed

- `src/game/core/simulation.js` — new exported `exportStateSnapshot(state)`:
  - `version: 1` for forward-compatibility
  - `exportedAt`, `status`, `realmCycleAccumulator`, `realmCycleCount`
  - `world.{width, height, tileSize, controlPoints, settlements, resourceNodes}` — control-point owner/loyalty/capture-progress/control-state/fort-tier/governor persisted; settlement sortie timers persisted; resource node amounts persisted
  - `factions[id]` — every faction's resources, population, faith, conviction, dynasty, AI state, assault strain, cohesion penalty, strain streaks, blood production load, dark-extremes state
  - `units` — id, typeId, faction, position (rounded 0.1), health, commander binding, siege supply timers, engineer support timers, reserve duty, command
  - `buildings` — id, typeId, faction, tile coords, build progress, completed flag, health, production queue, burn state, poison state, sabotage gate exposed timer
  - `projectileCount` — ephemeral count only; projectiles reset on restore
- `tests/runtime-bridge.mjs` — version check, key blocks present, default Ironmark persists, JSON round-trip stable, snapshot is non-trivial length.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (including snapshot round-trip).
- Syntax clean.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Save-state serialization primer | DOCUMENTED | LIVE (export only; round-trip restore still PARTIAL) |
| Full save/resume | DOCUMENTED | PARTIAL (export LIVE; restore DOCUMENTED) |

## Session 25 Next Action

- `restoreStateSnapshot(content, snapshot)` (round-trip restore).
- Or: Naval foundation (water tiles + harbor + 1 vessel class).
- Or: Dual-clock declaration seam.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE (export side); full save/resume now PARTIAL with clear next layer. Tests green.
