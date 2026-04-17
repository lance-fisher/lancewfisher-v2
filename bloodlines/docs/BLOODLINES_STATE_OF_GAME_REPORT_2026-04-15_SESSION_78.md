# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 78
Author: Codex

## Scope

Session 77 returned the world-pressure lane to the house vector. Session 78 makes Hartvale's already-live unique unit matter in runtime. The `verdant_warden` is no longer only a house-gated roster entry with favorable stats. It now strengthens settlement defense, accelerates reserve recovery and muster, and reinforces local territorial loyalty in the zones it protects.

## Changes Landed

### Verdant Warden local support profile (`src/game/core/simulation.js`)

- Added a shared Verdant Warden zone-support seam:
  - `isVerdantWardenUnit(...)`
  - `getVerdantWardenSupportProfile(...)`
  - `getVerdantWardenZoneSupportProfile(...)`
  - `getVerdantWardenControlPointSupportProfile(...)`
- The support profile now scales by nearby Warden count and exposes:
  - defender attack multiplier,
  - reserve-heal multiplier,
  - reserve-muster multiplier,
  - loyalty-protection multiplier,
  - loyalty-gain multiplier,
  - stabilization multiplier,
  - desired-frontline bonus.

### Settlement-defense integration (`src/game/core/simulation.js`)

- `getSettlementDefenseState(...)` now reads live Verdant Warden presence around the settlement and exposes that support through the existing defense-state seam.
- `tickFortificationReserves(...)` now uses that support to:
  - heal recovering defenders faster,
  - muster reserves forward faster,
  - demand a larger healthy frontline when Warden-backed defense is present.
- `getFriendlyFortificationCombatProfile(...)` now increases defender attack output when Verdant Wardens are supporting the keep.

### Loyalty and stabilization integration (`src/game/core/simulation.js`)

- `applyControlPointLoyaltyDelta(...)` now makes positive loyalty growth stronger and negative loyalty erosion weaker when local Verdant Warden support exists.
- `updateControlPoints(...)` now makes both passive stabilization and owner-present stabilization resolve faster when Verdant Wardens are anchoring the local zone.

### Realm-snapshot and HUD legibility (`src/game/core/simulation.js`, `src/game/main.js`)

- The loyalty snapshot now exposes:
  - supported territory count,
  - peak Warden coverage,
  - loyalty-gain bonus,
  - loyalty-protection bonus.
- The fortification snapshot now exposes:
  - local Verdant Warden count,
  - keep-defense attack bonus,
  - reserve-heal bonus,
  - reserve-muster bonus.
- The 11-pill dashboard now surfaces that support directly in the Loyalty and Fort pills instead of leaving the new Hartvale lane hidden in simulation only.

### AI awareness (`src/game/core/ai.js`)

- Stonehelm now reads the player's live Verdant Warden keep-defense count through `getRealmConditionSnapshot(...)`.
- Enemy keep-assault staging now demands heavier escort mass before committing into Hartvale Warden-backed defenses.
- The message log now names Hartvale wardens when that support is the reason Stonehelm delays a keep assault.

### Runtime verification (`tests/runtime-bridge.mjs`)

- Added Session 78 coverage proving:
  - local Verdant Warden support measurably increases keep-reserve recovery,
  - local Verdant Warden support measurably improves threatened march loyalty recovery,
  - the realm snapshot surfaces the new Hartvale support metrics,
  - snapshot restore preserves the support state and lets both healing and loyalty support resume after reload.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.

## Canonical Interdependency Check

Session 78 connects:

1. House identity and settlement defense, because Hartvale's unique unit now materially changes keep-defense attack leverage, reserve recovery, and frontline depth.
2. House identity and territorial loyalty, because the same Warden presence now accelerates loyalty recovery, softens loyalty loss, and improves march stabilization.
3. House identity and AI behavior, because Stonehelm now recognizes Warden-backed keeps as harder assault targets and delays until escort mass is heavier.
4. House identity and legibility, because the existing 11-pill dashboard now exposes Warden support in the Loyalty and Fort surfaces.
5. House identity and continuity, because snapshot export and restore preserve the same support-producing unit presence and allow the support effects to resume after reload.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Hartvale Verdant Warden settlement-defense and loyalty support | PARTIAL | LIVE (first layer, through keep-defense leverage, reserve support, loyalty protection and growth, dashboard legibility, AI awareness, and restore-safe runtime proof) |

## Session 79 Next Action

Do not invent another non-settled house-specific unit lane. Hartvale is the only additional non-Ironmark house with a settled unique-unit seam already locked into canon. The next highest-leverage unblocked action is to advance the standard military lane through `Scout Rider` as the first honest stage-2 cavalry and raiding unit, then wire it into logistics disruption, territorial harassment, AI use, and snapshot continuity.
