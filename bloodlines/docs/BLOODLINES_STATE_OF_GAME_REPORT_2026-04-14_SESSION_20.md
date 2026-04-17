# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 20
Author: Claude

## Scope

Full canonical L3 faith unit roster (8 units, 2 per covenant per doctrine path) now LIVE in data, trained at Covenant Hall, gated by canonical faith commitment + doctrine path match. Master doctrine section XIX L3 unit layer LIVE.

## Changes Landed

### L3 faith units (Vector 4)

- `data/units.json` — 8 new units, all stage 3, role "faith-unit", prototypeEnabled:
  - **Old Light Light**: Flame Warden (145 HP / 17 atk / 20 range / armor 6)
  - **Old Light Dark**: Judgment Pyre (130 HP / 22 atk / 22 range / armor 4)
  - **Blood Dominion Light**: Consecrated Blade (150 HP / 18 atk / armor 5)
  - **Blood Dominion Dark**: Crimson Reaver (140 HP / 24 atk / armor 3)
  - **Order Light**: Mandate Sentinel (160 HP / 16 atk / armor 7)
  - **Order Dark**: Edict Enforcer (148 HP / 20 atk / armor 6)
  - **Wild Light**: Root Warden (138 HP / 17 atk / 122 range / armor 4) — ranged
  - **Wild Dark**: Predator Stalker (125 HP / 21 atk / speed 82 / armor 3) — fast melee
- Each unit tagged with `faithId` and `doctrinePath` for gating.

### Covenant Hall becomes recruitment seat

- `data/buildings.json` — covenant_hall `trainableUnits` now contains all 8 L3 faith units.

### Recruitment gating (Vector 4)

- `src/game/core/simulation.js` — `queueProduction` now rejects faith-unit training unless:
  - Faction has committed to the matching covenant (`faction.faith.selectedFaithId === unitDef.faithId`)
  - Faction's doctrine path matches (`faction.faith.doctrinePath === unitDef.doctrinePath`)
- Canonical master doctrine section XIX: faith-specific units are covenant-exclusive.

### Test coverage

- `tests/data-validation.mjs` — all 8 canonical L3 faith units exist, correct faith/doctrine bindings, role "faith-unit", prototypeEnabled true, each listed in Covenant Hall trainables.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| L3 faith-specific unit rosters (8 units, 2 per covenant per path) | DOCUMENTED | LIVE (data + Hall trainables + faith/doctrine gate) |

## Session 21 Next Action

- L4 faith unit roster (8 more units) — requires Grand Sanctuary L3 covenant building.
- Or: commander sortie UI real-time cooldown tick (legibility polish).
- Or: Supply-protection patrols (remaining DOCUMENTED longer-siege AI layer).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green.
