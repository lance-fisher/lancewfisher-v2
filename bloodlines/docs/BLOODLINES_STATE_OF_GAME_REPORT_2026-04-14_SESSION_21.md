# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 21
Author: Claude

## Scope

Grand Sanctuary (L3 covenant building) and canonical 8-unit L4 faith roster now LIVE. Faith building progression now has three tiers operational (L1 Wayshrine / L2 Covenant Hall / L3 Grand Sanctuary); L4 Apex still DOCUMENTED.

## Changes Landed

### Grand Sanctuary L3 (Vector 4)

- `data/buildings.json` — new `grand_sanctuary` building:
  - `faithRole: "sanctuary"`, `faithTier: 3`, `requiresFaithCommitment: true`
  - 4x4 footprint, health 1400, buildTime 32, armor 10
  - Amp 3.0x (vs Hall 2.4x), radius 320 (vs Hall 240), intensity bonus 0.48
  - Ward hooks: wardSightBonusExtra 12, wardDefenderAttackBonusExtra 0.08
  - Cost gold 300 wood 180 stone 260 iron 90
  - Trains all 8 canonical L4 faith units

### L4 faith unit roster (Vector 4)

- `data/units.json` — 8 new stage-4 faith units:
  - **Old Light**: Flame Herald (L) / Pyre Sovereign (D)
  - **Blood Dominion**: Blood Consort (L) / Crimson Exarch (D)
  - **Order**: Mandate Paladin (L) / Edict Inquisitor (D)
  - **Wild**: Elder Grove Keeper (L, ranged 140) / Predator Ascendant (D, speed 88)
- Each 3 pop, faithId + doctrinePath tagged. Stats scale canonically beyond L3 counterparts.

### UI + renderer

- `src/game/main.js` — `grand_sanctuary` added to worker build palette.
- `src/game/core/renderer.js` — Sanctuary silhouette: tall central spire, flanking pillars, apex sigil, largest 64-tile aura ring.

### Test coverage

- `tests/data-validation.mjs` — Sanctuary existence + faithTier 3 + requiresFaithCommitment + amp exceeds Hall; all 8 L4 canonical units exist with correct faith/doctrine bindings at stage 4; Sanctuary lists all 8 in trainables.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Faith building progression L3 (Grand Sanctuary) | DOCUMENTED | LIVE |
| L4 faith unit rosters (8 units, 2 per covenant per path) | DOCUMENTED | LIVE |

## Canonical Faith Progression Status

- L1 Wayshrine: LIVE (Session 13)
- L2 Covenant Hall: LIVE (Session 17)
- L3 Grand Sanctuary: LIVE (Session 21)
- L4 Apex structure (1 per covenant): DOCUMENTED
- L3 faith units: LIVE (Session 20)
- L4 faith units: LIVE (Session 21)
- L5 Apex units (1 per covenant): DOCUMENTED

## Session 22 Next Action

- L4 Apex covenant structures (1 per covenant) + canonical L5 apex units.
- Or: Supply-protection patrols (remaining longer-siege AI DOCUMENTED layer).

## Preservation

No canon reduced. 2 items moved DOCUMENTED → LIVE. Tests green.
