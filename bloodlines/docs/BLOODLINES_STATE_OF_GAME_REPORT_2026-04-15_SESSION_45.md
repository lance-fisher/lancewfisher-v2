# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 45
Author: Claude

## Scope

Minor-faction founder unit spawn LIVE. Extends Session 44: when a defected lesser branch spawns its minor-house faction, a single militia unit now appears on the map near the parent's command hall, carrying the founder's identity via `commanderMemberId`. The minor faction stops being a registry ghost and becomes a physical actor visible to renderer, targetable by AI, and counted in `population.total`. Vector 3 (Military) advances, Vector 5 (World) advances.

## Changes Landed

### Founder-unit spawn in `spawnDefectedMinorFaction` (`src/game/core/simulation.js`)

- Locates parent's `command_hall` (canonical: the founder walks out of the parent's seat).
- Spawns a militia unit at `center + 64,64` offset from the hall, assigned to the minor faction id.
- Attaches founder via `commanderMemberId = founderCopy.id`, links through `minor.dynasty.attachments.commanderUnitId`.
- Sets `minor.population.total = 1`, `minor.population.cap = 1` so the minor faction's population is internally consistent.
- Militia chosen as first canonical layer; richer retinues and house-specific unique units are explicit follow-ups.

### Test coverage

- Force defection, assert `units.length` grew, exactly one minor-faction unit exists, typeId is "militia", commander attachment wired, population total is 1.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.

## Canonical Interdependency Check

Founder-unit spawn connects to:
1. **Minor-faction spawn (S44)**: extends the spawn path.
2. **Combat system**: the unit is a valid `applyDamage` target with `role !== "worker"`, so killing it grants renown to the killer (S40) — the minor faction can now be fought.
3. **Population system**: minor's `population.total` is consistent.
4. **Command-attachment system**: `commanderMemberId` links the physical unit to the minor's head of bloodline.
5. **Renderer**: unit appears with the minor faction's presentation colors (copied from parent in S44).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Minor-house founder unit | DOCUMENTED | LIVE (single militia at parent's command hall, commander attached, population consistent) |

## Session 46 Next Action

- Minor-house territorial footprint (claim a tile region so the minor has a place, not just a unit).
- Or: Vector 1 Civilizational advance.
- Or: Mixed-bloodline defection slider runtime.
- Or: Faith-compatibility weighting in AI marriage.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Vectors 3 and 5 both advanced. Tests green.
