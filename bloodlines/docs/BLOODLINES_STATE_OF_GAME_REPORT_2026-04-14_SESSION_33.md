# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 33
Author: Claude

## Scope

Marriage system first canonical layer LIVE. Vector 2 (Dynastic depth) advanced after stagnation since Session 14. Master doctrine section XVIII ("Marriage matters in governance, diplomacy, succession, and dynastic continuity") + canonical polygamy restriction (Blood Dominion + Wild only) now operational with cross-faction propose/accept, dual-clock gestation, and mixed-bloodline child generation.

## Changes Landed

### Marriage state (Vector 2)

- `src/game/core/simulation.js` — `createDynastyState` now seeds `marriages: []`, `marriageProposalsIn: []`, `marriageProposalsOut: []`.
- `ensureMarriageState(faction)` helper for safe access.
- `factionAllowsPolygamy(faction)` returns true only when committed faith is `blood_dominion` or `the_wild` (canonical master doctrine section XVIII).
- `memberHasActiveMarriage(faction, memberId)` enforces single-marriage constraint when polygamy is not allowed.

### `proposeMarriage(state, sourceFactionId, sourceMemberId, targetFactionId, targetMemberId)` exported

- Cross-faction only (master doctrine canonical: marriage is a diplomatic + dynastic act between houses).
- Members must be alive and "active" or "ruling".
- Polygamy gate: rejects existing-marriage second proposal unless committed faith permits.
- Records proposal in source's outbox + target's inbox with proposed-at simulation time and dual-clock day.
- Pushes canonical message.

### `acceptMarriage(state, proposalId)` exported

- Locates proposal in any faction's inbox.
- Creates two marriage records: source `isPrimaryRecord: true` (canonical child-generation responsibility), target mirror `isPrimaryRecord: false` (no duplication of children).
- Both records share marriageId for cross-reference; arrays are independent (no shared-reference bug).
- Diplomatic effect: each side drops the other from its `hostileTo` list (canonical: marriage de-escalates).
- Conviction effect: +2 oathkeeping for both factions.
- Legitimacy effect: +2 for both dynasties (capped at 100).
- Dual-clock declaration: 30 in-world days for the wedding ceremony.
- Pushes canonical message.

### Child generation (Vector 2 + Vector 5)

- New `tickMarriageGestation(state)` runs each tick (added to `stepSimulation` after `tickDynastyOperations`).
- Iterates faction marriages; only `isPrimaryRecord` marriages generate children.
- Gestation: `MARRIAGE_GESTATION_IN_WORLD_DAYS = 280` (canonical 9-month gestation in dual-clock days).
- Child added as new dynasty member: stage 0 age, status active, role heir_designate, path governance, renown 4, with `mixedBloodline: { headHouseId, spouseHouseId }` for canonical defection-slider lineage tracking.
- Pushes canonical birth message.

### Test coverage

- `tests/runtime-bridge.mjs`:
  - Cross-faction proposal succeeds and lands in target inbox.
  - Acceptance succeeds, both factions record marriage, hostile list updates, legitimacy applies.
  - Forced dual-clock advance past gestation triggers child generation; child carries `mixedBloodline`.
  - Polygamy gate test: second marriage proposal for same member rejected when no polygamy-permitting faith committed.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (full marriage round trip + polygamy gate).
- Syntax clean.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Marriage system first canonical layer | DOCUMENTED | LIVE (propose/accept + faith-gated polygamy + dual-clock gestation + mixed-bloodline child generation) |
| Mixed-bloodline children | DOCUMENTED | DATA-ONLY (children carry mixedBloodline metadata; defection-slider runtime still DOCUMENTED) |

## Remaining Marriage Canonical Layers

- Marriage UI (proposal panel, accept/reject buttons, marriage history).
- Marriage proposal expiration / decline path.
- Marriage death/dissolution mechanics.
- Mixed-bloodline defection slider runtime effect.
- Cross-house marriage diplomatic ripple beyond simple hostility removal (loyalty trickle, alliance candidacy).
- AI marriage proposals (Stonehelm proposes when alliance is strategically beneficial).

## Session 34 Next Action

- Marriage UI panel (legibility-follows-depth: surface within 2 sessions per master doctrine).
- Or: Lesser houses promotion pipeline.
- Or: AI marriage proposal logic.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE, 1 item moved DOCUMENTED → DATA-ONLY with explicit canonical follow-up. Vector 2 advanced significantly. Tests green.
