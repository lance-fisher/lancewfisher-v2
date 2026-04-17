# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-16
Session: 90
Author: Claude

## Scope

Session 90 added the first non-aggression pact diplomacy system, creating the missing diplomatic counterplay against alliance-threshold coalition pressure during the Territorial Governance sovereignty path. Canon: sovereignty requires statecraft, not just conquest. The player can now spend influence and gold to propose a pact that removes mutual hostility, reducing the alliance-threshold hostile count that slows acceptance growth. Breaking the pact has real consequences.

## Changes Landed

### Non-aggression pact system (`src/game/core/simulation.js`)

- **Constants**: `NON_AGGRESSION_PACT_INFLUENCE_COST = 50`, `NON_AGGRESSION_PACT_GOLD_COST = 80`, `NON_AGGRESSION_PACT_MINIMUM_DURATION_IN_WORLD_DAYS = 180`, `NON_AGGRESSION_PACT_BREAK_LEGITIMACY_COST = 8`.

- **`getNonAggressionPactTerms(state, factionId, targetFactionId)`**: Returns availability, cost, and minimum duration. Rejects: self-pacts, non-kingdoms, non-hostile targets, duplicate pacts, active holy wars between the parties, insufficient resources.

- **`proposeNonAggressionPact(state, factionId, targetFactionId)`**: Spends influence and gold, removes mutual hostility via `removeMutualHostility`, creates pact records on both factions, pushes a message, returns the pact ID.

- **`breakNonAggressionPact(state, factionId, targetFactionId)`**: Marks the pact as broken on both sides, restores mutual hostility via `ensureMutualHostility`, costs legitimacy (-8) and conviction (-2), pushes a message, reports whether the break was early.

- **`removeMutualHostility(state, factionIdA, factionIdB)`**: Internal helper that filters the target from both factions' hostileTo arrays.

- **`getActivePact(faction, targetFactionId)`**: Internal helper that finds a non-broken pact record.

### Diplomacy state on factions

- Factions now carry an optional `diplomacy.nonAggressionPacts` array of pact records.
- Each pact record includes: `id`, `factionId`, `targetFactionId`, `startedAt`, `startedAtInWorldDays`, `minimumExpiresAtInWorldDays`, `brokenAt`, `brokenByFactionId`.

### Sovereignty-path integration

- Because `proposeNonAggressionPact` calls `removeMutualHostility`, the already-live `hostileKingdomCount` in the acceptance profile automatically decreases. This directly eases the alliance-threshold acceptance-drag and the realm-cycle coalition loyalty/legitimacy erosion.
- The player now has a genuine diplomatic counterplay to the alliance-threshold coalition: spend 50 influence + 80 gold to form a non-aggression pact, which reduces the hostile count and accelerates the 60 to 65% acceptance push.

### Save/restore support (`src/game/core/simulation.js`)

- `exportStateSnapshot` now includes `diplomacy: shallowCopyMap(f.diplomacy ?? null)` in the faction export.
- `restoreStateSnapshot` now deep-copies `snapF.diplomacy` back onto the restored faction.

### Dynasty panel exposure (`src/game/main.js`)

- A new "Diplomacy" section in the dynasty panel lists active non-aggression pacts with target name and remaining minimum-duration days.
- The section only appears when at least one active pact exists.

### Runtime bridge test coverage (`tests/runtime-bridge.mjs`)

- Imported `getNonAggressionPactTerms`, `proposeNonAggressionPact`, `breakNonAggressionPact`.
- 13 new assertions covering:
  - Mutual hostility exists before pact.
  - Terms are available when hostile and can afford.
  - Proposal succeeds and returns a pact ID.
  - Hostility removed from both sides after pact.
  - Influence cost deducted.
  - Pact records exist on both factions.
  - Duplicate pact rejected.
  - Save/restore preserves active pact and de-hostilized state.
  - Breaking a pact restores mutual hostility.
  - Breaking a pact costs legitimacy.
  - Message log records both establishment and break.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed (including 13 new pact assertions).
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/renderer.js` passed.

## Canonical Interdependency Check

Session 90 does not introduce an isolated system. The non-aggression pact connects to:

1. **Alliance-threshold coalition pressure** (Session 88/89): Reducing the hostile count directly eases acceptance-drag and realm-cycle coalition erosion.
2. **Holy war system**: Active holy wars between the parties block pact formation, forcing the player to resolve faith conflicts before diplomacy.
3. **Legitimacy and conviction**: Breaking a pact costs legitimacy (-8) and conviction (-2), creating a real cost for diplomatic bad faith.
4. **Marriage diplomacy**: Non-aggression pacts complement the existing marriage lane as a second diplomatic tool for reducing inter-kingdom tension.
5. **World pressure**: Lower hostile count reduces world-pressure contribution from hostile operations, which feeds back into the acceptance profile.

## Gap Analysis Reclassification

- Non-aggression pact diplomacy: NEW, moved to LIVE with test coverage, save/restore, and dynasty-panel legibility.
- Alliance diplomacy for acceptance counterplay: moved from DOCUMENTED to LIVE through the pact system.
- The broader Territorial Governance victory family is now closer to LIVE. Remaining gaps: multi-kingdom neutral-power stage presence, Trueborn city arbitration, and the full canonical ~90% sovereignty doctrine.

## Session 91 Next Action

Priority order:

1. Add AI non-aggression pact awareness so Stonehelm can propose or accept pacts strategically under governance pressure.
2. If AI pact awareness blocks, deepen the naval-world integration or broader theatre-of-war expansion.
3. If those lanes block, open the Unity Play Mode verification shell.
