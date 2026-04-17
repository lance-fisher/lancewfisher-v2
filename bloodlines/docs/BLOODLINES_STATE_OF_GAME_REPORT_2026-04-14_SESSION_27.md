# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 27
Author: Claude

## Scope

Naval foundation first canonical layer LIVE. Master doctrine section XV (naval strategy must be real) and section XIV (continental separation by water) gain their implementation anchor. Harbor building, three canonical vessel classes, map water patches, and movement-domain gating all operational.

## Changes Landed

### Data layer (Vector 5 + Vector 3)

- `data/buildings.json` — new `harbor_tier_1`:
  - `navalRole: "harbor"`, `navalTier: 1`, `requiresCoastalAdjacency: true`
  - 3x3 footprint, health 950, buildTime 20, armor 6
  - Food + wood dropoff (canonical coastal economy)
  - Trains 3 vessel classes
  - Cost gold 130 wood 120 stone 70
- `data/units.json` — 3 canonical vessel classes (first 3 of 6 per master doctrine):
  - **Fishing Boat** (stage 1, gatherRate 1.2, carryCapacity 18, cost 60g/60w) — first economic vessel
  - **Scout Vessel** (stage 1, speed 92, sight 200, cost 70g/50w) — fast recon
  - **War Galley** (stage 2, health 360, atk 22, range 130, armor 8, cost 140g/130w/60iron) — first naval combatant
  - All tagged `role: "vessel"`, `vesselClass`, `movementDomain: "water"`
- `data/maps/ironmark-frontier.json` — two new water patches (northwest bay 0,36→8,48 and northeast bay 64,0→72,10) marked `isCoastal: true` so harbors can be placed canonically near both player and enemy starting areas. Existing 48-tile river remains navigable by vessels.

### Simulation (Vector 3 + Vector 5)

- `src/game/core/simulation.js`:
  - New `isWaterTileAt(state, tileX, tileY)` checks water/river terrain patches.
  - New `isFootprintAdjacentToWater(state, tileX, tileY, footprint)` inspects 1-tile ring for water adjacency.
  - `attemptPlaceBuilding` now rejects harbor placement unless coastal adjacency.
  - `issueMoveCommand` now gates by `movementDomain`: vessels rejected onto land; land units rejected onto water. Canonical: ships do not walk, infantry do not swim.

### Renderer (Vector 6)

- New harbor silhouette: stone base, wooden pier extending down, moored vessel glyph, coastal flag.
- New vessel silhouette: elongated hull, mast + triangular sail, water-ripple indicator.
- Water terrain patches render with canonical blue (distinct from river's slightly lighter tone).
- Minimap also renders water patches distinctly from river.

### UI

- `wayshrine, covenant_hall, grand_sanctuary, apex_covenant, harbor_tier_1` now in worker build palette.

### Test coverage

- `tests/data-validation.mjs` — harbor existence, navalRole, coastal adjacency gate, 3 vessel defs with role/movementDomain/prototypeEnabled, harbor trainables include all 3 vessels, map has at least 1 water patch.
- `tests/runtime-bridge.mjs` — fishing_boat and war_galley have `movementDomain: "water"` wired.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Naval foundation (harbor + vessels + water movement) | DOCUMENTED | LIVE (first canonical layer: harbor, 3 of 6 vessel classes, coastal gate, domain movement) |
| Continental architecture (water separation) | DOCUMENTED | PARTIAL (water patches + vessels exist; continental crossing still DOCUMENTED) |

## Remaining Naval DOCUMENTED Layers

- 3 of the 6 canonical vessel classes: Transport, Fire Ship (one-use sacrifice), Capital Ship (apex).
- Harbor L2/L3 upgrade tiers.
- Naval trade routes.
- Naval combat balancing (war_galley vs war_galley).
- Vessel → land transport mechanic.
- True continental separation (secondary continent + required vessel crossing).
- Naval doctrine axis (defensive / balanced / offensive).

## Session 28 Next Action

- Transport + Fire Ship + Capital Ship vessels (complete canonical 6-class roster) + Harbor L2 upgrade.
- Or: AI naval reactivity (Stonehelm builds harbor + vessels when player fortifies waters).
- Or: include dualClock in save/resume snapshot.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. 1 item moved DOCUMENTED → PARTIAL with explicit remaining layers logged. Tests green.
