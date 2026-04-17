# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-16
Session: 93
Author: Claude

## Scope

Session 93 added the first Trueborn City neutral-faction foundation to Bloodlines, implementing the canonical neutral-power arbitration layer. The Trueborn City now exists as a live faction in the simulation, tracks trade relationships with each kingdom based on conviction, legitimacy, diplomacy, and hostility record, and feeds that standing into the population-acceptance profile as endorsement that materially affects the sovereignty path.

## Changes Landed

### Trueborn City neutral faction (`src/game/core/simulation.js`)

- `createSimulation` now automatically creates a `trueborn_city` faction with kind `trueborn_city`, Trueborn house identity, and a `tradeRelationships` object.
- The faction is non-combatant: no units, no buildings, no AI profile.
- A startup message now announces: "The Trueborn City watches from its neutral seat. Earn its endorsement through moral governance."

### Trade-relationship realm-cycle tick (`src/game/core/simulation.js`)

- Each realm cycle, the Trueborn City updates its trade standing with every kingdom faction based on:
  - **Conviction band**: Apex Moral +1.5/cycle, Moral +0.8, Neutral +0.1, Cruel -0.8, Apex Cruel -1.5
  - **Legitimacy bonus**: +0.3 if legitimacy >= 70
  - **Hostility penalty**: -0.4 if the kingdom is hostile to 3+ factions
  - **Pact bonus**: +0.2 if the kingdom has any active non-aggression pact
- Standing is clamped to [-20, 20].

### Acceptance profile integration (`src/game/core/simulation.js`)

- The acceptance profile now reads Trueborn standing and computes a `truebornEndorsement` modifier (standing * 0.25, clamped [-3, +5]).
- This modifier is added to the acceptance target formula alongside conviction, covenant, and tribal factors.
- A kingdom at maximum Trueborn standing (+20) gains +5 acceptance target. A kingdom at minimum standing (-20) suffers -3.
- Exposed through the acceptance profile as `truebornStanding` and `truebornEndorsement`.

### Profile propagation

- `getTerritorialGovernanceProfile`: inherits via `...acceptanceProfile` spread.
- `serializeTerritorialGovernanceRecognition`: explicitly copies `truebornStanding` and `truebornEndorsement`.
- Dynasty panel "World stance" line: now shows "Trueborn +N" or "Trueborn -N" when endorsement is non-zero.

### Save/restore support

- `exportStateSnapshot` now includes `tradeRelationships: shallowCopyMap(f.tradeRelationships ?? null)`.
- `restoreStateSnapshot` now deep-copies `snapF.tradeRelationships` back onto the restored faction.
- The Trueborn City faction itself is preserved through the existing faction export/restore loop since it's a regular faction entry.

### Runtime bridge test coverage (`tests/runtime-bridge.mjs`)

- 9 new assertions covering:
  - Trueborn City faction exists with correct kind and tradeRelationships.
  - Trade standing starts near zero.
  - Moral conviction raises standing over 4 realm cycles.
  - Cruel conviction lowers standing below moral baseline.
  - Trueborn endorsement surfaces in the acceptance profile as a number.
  - Positive standing produces positive endorsement.
  - Trueborn standing surfaces in the acceptance profile.
  - Save/restore preserves the Trueborn City faction.
  - Save/restore preserves trade standing values.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed (including 9 new Trueborn City assertions).
- All syntax checks pass.
- Browser preview: zero console errors, zero failed requests, Diplomacy section renders, startup message includes Trueborn City announcement.

## Canonical Interdependency Check

The Trueborn City connects to every sovereignty-path layer built in Sessions 88-92:

1. **Conviction** (Session 89): drives the primary standing delta per realm cycle.
2. **Legitimacy**: high legitimacy grants a bonus to Trueborn standing.
3. **Diplomacy** (Sessions 90-92): active non-aggression pacts grant a small standing bonus; excessive hostility penalizes it.
4. **Acceptance profile** (Sessions 88-89): Trueborn endorsement is a direct term in the target formula alongside conviction modifier, covenant endorsement, and tribal friction.
5. **World pressure**: kingdoms under heavy world pressure tend to have more hostiles and lower conviction, which naturally erodes Trueborn standing.

## Gap Analysis

- Trueborn City neutral-faction foundation: NEW, moved to LIVE with trade relationships, acceptance integration, save/restore, and test coverage.
- The canonical Trueborn City has much deeper functionality described in the design bible (intelligence conduit, diplomatic protection, Trueborn Rise arc, trade markets, conviction-based trade terms, Trueborn Summons). The current layer is the first foundation. Future sessions should deepen it incrementally.

## Session 94 Next Action

Priority order:

1. Continue browser-lane deepening through the Trueborn Rise arc (Trueborn activation when unchallenged) or naval-world integration.
2. If those lanes are deprioritized, open the Unity Play Mode verification shell.
3. If Unity is also deprioritized, continue into match-stage polish or broader theatre-of-war expansion.
