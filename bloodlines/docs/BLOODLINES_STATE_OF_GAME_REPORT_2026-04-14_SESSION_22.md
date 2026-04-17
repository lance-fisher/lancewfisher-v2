# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 22
Author: Claude

## Scope

L4 Apex covenant structure and canonical 4-unit L5 apex unit roster now LIVE. Faith progression is complete: L1 Wayshrine, L2 Covenant Hall, L3 Grand Sanctuary, L4 Apex Covenant all canonical and operational. The four named apex units (The Unbroken, The Sacrificed, The Mandate, The First Wild) per master doctrine section XIX are all in data and trainable gated on faith commitment + intensity.

## Changes Landed

### Apex Covenant L4 (Vector 4)

- `data/buildings.json` — new `apex_covenant`:
  - `faithRole: "apex"`, `faithTier: 4`
  - `requiresFaithCommitment: true` AND `requiresFaithIntensity: 80` (Fervent tier canonical gate)
  - 5x5 footprint (largest building), health 2400, buildTime 48, armor 14
  - Amp 3.6x (highest), radius 400, intensity bonus 0.72
  - Ward hooks: wardSightBonusExtra 20, wardDefenderAttackBonusExtra 0.14
  - `apexUnitBinding` map: each covenant binds to its canonical L5 apex unit
  - Cost gold 600 wood 320 stone 480 iron 200 influence 80 (highest cost + influence requirement)
  - Trains all 4 apex units (only 1 accessible per match via faithId gate)

### Canonical L5 apex units (Vector 4)

- `data/units.json` — 4 canonical apex units, stage 5, role "faith-apex":
  - **The Unbroken** (Old Light): 380 HP / 44 atk / armor 14
  - **The Sacrificed** (Blood Dominion): 360 HP / 52 atk / speed 74 / armor 12
  - **The Mandate** (Order): 420 HP / 40 atk / armor 16
  - **The First Wild** (Wild): 350 HP / 48 atk / speed 92 / sight 220 / armor 10
- Each 5 pop. Gated by faithId only (not doctrinePath — apex transcends L/D branching per canon).

### Intensity gate (Vector 4)

- `src/game/core/simulation.js` — `attemptPlaceBuilding` now checks `buildingDef.requiresFaithIntensity` against `faction.faith.intensity`. Apex Covenant requires sustained Fervent-tier (80+) faith. Canonical anti-snowball + late-game divergence lock per master doctrine section XXI.

### UI + renderer

- `src/game/main.js` — `apex_covenant` added to worker build palette.
- `src/game/core/renderer.js` — Apex silhouette: towering central obelisk, four flanking pillars, apex sigil with inner ring, widest 82-tile aura.

### Test coverage

- `tests/data-validation.mjs` — Apex existence + faithTier 4 + faith commitment gate + intensity gate >= 80 + amp exceeds Sanctuary; all 4 L5 apex units at stage 5 role faith-apex, faith-bound, prototypeEnabled; Apex trainables include all 4.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| L4 Apex covenant structures (1 per covenant) | DOCUMENTED | LIVE (single `apex_covenant` building with per-faith apex unit binding) |
| L5 Apex units (The Unbroken, The Sacrificed, The Mandate, The First Wild) | DOCUMENTED | LIVE |

## Full Faith Progression Status — Now Complete

- L1 Wayshrine: LIVE (Session 13)
- L2 Covenant Hall: LIVE (Session 17)
- L3 Grand Sanctuary: LIVE (Session 21)
- L4 Apex Covenant: LIVE (Session 22)
- L3 faith units (8): LIVE (Session 20)
- L4 faith units (8): LIVE (Session 21)
- L5 apex units (4): LIVE (Session 22)

Full canonical faith system now end-to-end operational from discovery through apex.

## Session 23 Next Action

- Supply-protection patrols (remaining DOCUMENTED longer-siege AI layer).
- Or: sortie cooldown real-time tick-down in UI.
- Or: naval foundation first canonical layer.

## Preservation

No canon reduced. 2 items moved DOCUMENTED → LIVE. Full faith progression canon-to-runtime complete. Tests green.
