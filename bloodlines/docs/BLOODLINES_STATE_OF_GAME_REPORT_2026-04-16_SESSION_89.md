# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-16
Session: 89
Author: Claude

## Scope

Session 89 deepened the Territorial Governance population-acceptance model with three new interactive factors that connect the sovereignty path to already-live conviction, faith, and tribal systems. These factors create meaningful strategic levers: the player must now maintain moral conviction, complete covenant commitments, and pacify tribal resistance to push acceptance toward the 65% sovereignty threshold. This addresses the neutral-power counterpressure gap identified in the Session 87 gap analysis.

## Changes Landed

### Conviction-based acceptance modifier (`src/game/core/simulation.js`)

- The acceptance profile now reads the faction's live conviction band and applies a direct modifier to the population-acceptance target:
  - `apex_moral`: +4 (canon: "Recognized Protector, neutral communities offer tribute without coercion")
  - `moral`: +2
  - `neutral`: 0
  - `cruel`: -2
  - `apex_cruel`: -4 (canon: cruel dynasties face world rejection)
- This creates a continuous strategic link between battlefield conduct, conviction ledger, and the sovereignty-acceptance path.
- The modifier is exposed through the acceptance profile as `convictionAcceptanceModifier` and `convictionBandId`.

### Covenant endorsement acceptance support (`src/game/core/simulation.js`)

- A passed Covenant Test now adds +3 to the acceptance target, representing the faith community's endorsement of the dynasty's right to govern.
- A completed Grand Sanctuary adds +2, representing the physical expression of covenant depth.
- Both factors combine: a faction with both passed covenant test and a standing Grand Sanctuary gains +5 total.
- Exposed through the acceptance profile as `covenantTestPassed`, `hasGrandSanctuary`, and `covenantEndorsement`.

### Tribal-backed acceptance friction (`src/game/core/simulation.js`)

- Active tribal raider units now impose a friction penalty on the acceptance target, capped at -4.
- Each live tribal unit contributes 0.8 friction, so 5 active raiders reach the cap.
- This represents the world's neutral resistance to unchecked dynastic sovereignty: the canonical Trueborn stabilizing force and broader world opinion that any dynasty must earn acceptance by pacifying the frontier, not just accumulating territory.
- Exposed through the acceptance profile as `activeTribeUnits` and `tribalFriction`.

### Dynasty panel and HUD legibility (`src/game/main.js`)

- The dynasty panel now shows a "World stance" detail line when any of the three new factors are non-zero, including:
  - conviction modifier (positive or negative)
  - covenant endorsement
  - tribal friction
- This appears below the acceptance line and above the seats/court line in the governance recognition section.

### Governance profile propagation (`src/game/core/simulation.js`)

- All three new acceptance factors are now propagated through:
  - `getTerritorialGovernanceAcceptanceProfile` return object
  - `getTerritorialGovernanceProfile` return object (via `...acceptanceProfile` spread)
  - `serializeTerritorialGovernanceRecognition` output (via explicit profile field copies)
  - `tickTerritorialGovernanceRecognition` (inherited from the profile refresh)

### Runtime bridge test coverage (`tests/runtime-bridge.mjs`)

- Added conviction acceptance modifier tests:
  - Apex Moral conviction provides a positive acceptance modifier.
  - Apex Moral conviction raises the acceptance target above the neutral baseline.
  - Apex Cruel conviction imposes a negative acceptance modifier.
  - Apex Cruel conviction lowers the acceptance target below the neutral baseline.
- Added covenant endorsement tests:
  - Covenant endorsement is at least +3 when the covenant test is passed.
  - Covenant test passed flag surfaces correctly in the acceptance profile.
- Added tribal friction tests:
  - Active tribal raiders impose non-zero tribal friction.
  - Active tribal raiders lower the acceptance target below the tribal-free baseline.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed (including 8 new conviction/covenant/tribal assertions).
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.

## Canonical Interdependency Check

Session 89 does not introduce isolated mechanics. The three new acceptance factors connect to existing live systems:

1. **Conviction modifier**: reads from the live conviction band that already drives stabilization, reserve heal, loyalty protection, capture, population growth, and attack modifiers.
2. **Covenant endorsement**: reads from the live Covenant Test and Grand Sanctuary systems that already gate apex faith units, Divine Right declarations, and stage-5 faith-unit recruitment.
3. **Tribal friction**: reads from the live tribe system that already generates world-pressure signals, frontier raids, and responds to dominant-kingdom overextension.

The population-acceptance path now interacts with battlefield conduct (conviction), religious commitment (covenant), and frontier pacification (tribes) in addition to the existing prosperity, dynasty, territory, marriage, and alliance-threshold systems.

## Gap Analysis Reclassification

- Population-acceptance interactive factors: moved from DOCUMENTED to LIVE. The acceptance target now responds to three new strategic levers beyond the existing prosperity, dynasty, and territory fundamentals.
- The broader Territorial Governance victory family remains PARTIAL because multi-kingdom diplomatic recognition mechanics, alliance diplomacy for hostile-count reduction, and the full canonical ~90% sovereignty doctrine are still not yet live.

## Session 90 Next Action

Priority order:

1. Implement alliance diplomacy so the player can reduce the hostile-kingdom count through non-aggression pacts or tribute offers, creating a direct diplomatic counterplay to coalition acceptance-drag.
2. If the alliance-diplomacy lane blocks, deepen multi-kingdom world pressure through neutral-power stage presence, third-kingdom introduction, or Trueborn city arbitration.
3. If those lanes block or are intentionally deprioritized, open the Unity Play Mode verification shell or continue into naval-world integration.
