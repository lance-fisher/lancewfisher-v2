# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 35
Author: Claude

## Scope

Lesser Houses promotion pipeline LIVE. Vector 2 (Dynastic depth) advanced again. War heroes with sufficient renown and qualifying service path can now be promoted to found cadet branches of the main house, extending the dynasty register, granting legitimacy, and recording canonical stewardship conviction. Detection is automatic; promotion is player-consented (canonical: new houses are a diplomatic act). This continues the Session 33-34 marriage arc by giving the realm a mechanism for new houses to enter the dynasty web, not only marriages between existing houses.

## Changes Landed

### Lesser houses state seeding (Vector 2)

- `src/game/core/simulation.js` — `createDynastyState` now seeds two new arrays:
  - `lesserHouses: []` — the active cadet branches.
  - `lesserHouseCandidates: []` — member ids auto-flagged as eligible founders.

### Canonical constants exported

- `LESSER_HOUSE_RENOWN_THRESHOLD = 30` — minimum renown to be flagged.
- `LESSER_HOUSE_MIN_PROMOTIONS = 1` — must have climbed at least once.
- `LESSER_HOUSE_LEGITIMACY_BONUS = 3` — parent house gain on founding.
- `LESSER_HOUSE_INITIAL_LOYALTY = 75` — starting cadet loyalty.
- `LESSER_HOUSE_QUALIFYING_PATHS` (internal Set) — `military_command`, `covert_operations`, `governance`. Priests and merchants hold influence but do not found cadet branches without additional mechanisms (deferred).

### Candidate detection tick

- New `tickLesserHouseCandidates(state)` runs every tick in `stepSimulation` after `tickMarriageGestation`.
- Iterates every faction's dynasty members.
- Flags any qualifying member via `memberIsLesserHouseCandidate`:
  - Must be available (not fallen/captured/dead/displaced).
  - Must NOT be head of bloodline (canonical: head continues the main line).
  - Must not already have founded a lesser house.
  - Renown `>= LESSER_HOUSE_RENOWN_THRESHOLD`.
  - `promotionHistory.length >= LESSER_HOUSE_MIN_PROMOTIONS`.
  - `pathId` in the qualifying-path set.
- Announces newly-eligible members once via canonical message ("has earned the right to a cadet house").
- Drops stale candidates who are no longer eligible so the list does not accrete.

### `promoteMemberToLesserHouse(state, factionId, memberId)` exported

- Player-driven promotion. All simulation-side guards honored:
  - Faction must have a dynasty.
  - Member must be findable.
  - Must pass `memberIsLesserHouseCandidate` (not head, not already promoted, above threshold, on qualifying path).
- Creates canonical lesser-house record:
  - `id`, `name` (derived from member title), `founderMemberId`, `founderTitle`, `foundedAtInWorldDays` (dual-clock timestamp), `parentFactionId`, `parentHouseId`, `loyalty: 75`, `status: "active"`.
- Marks founder member with `foundedLesserHouseId` and `foundedLesserHouseAtInWorldDays`.
- Rewards: `adjustLegitimacy(+3)` on parent faction (capped at 100) and `recordConvictionEvent(stewardship, +2, "lesser_house_founded")` logging the stewardship act in the conviction ledger.
- Removes founder from active candidate list.
- Pushes canonical message.

### Dynasty panel UI (Vector 6)

- `src/game/main.js`:
  - Imported `promoteMemberToLesserHouse`.
  - Two new sections in `renderDynastyPanel` (placed between marriage UI and member cards):
    - **Lesser houses roster**: lists each active cadet branch with name, rounded loyalty, and founder title.
    - **Lesser-house candidates**: lists eligible members with renown readout and a "Promote to Lesser House" action button that calls `promoteMemberToLesserHouse(state, "player", member.id)` and re-renders.
  - All buttons wire through real simulation functions per canonical no-decorative-UI mandate.

### Save/resume compatibility

- `exportStateSnapshot` already deep-clones `faction.dynasty` via `shallowCopyMap` (JSON round-trip), so `lesserHouses`, `lesserHouseCandidates`, and per-member `foundedLesserHouseId` automatically serialize and restore. No snapshot-shape changes required.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 35 block):
  - Initial state asserts both arrays present and empty.
  - Raising the marshal above threshold (+promotion entry) flags them on next tick.
  - Head of bloodline at renown 99 + promotion entry still NOT flagged (canonical exclusion).
  - Priest on `religious_leadership` path at renown 99 + promotion entry NOT flagged (canonical path restriction).
  - `promoteMemberToLesserHouse` succeeds: creates lesser house, marks founder, bumps legitimacy, bumps stewardship conviction.
  - Double-promotion guard rejects second attempt on same member.
  - Under-threshold guard rejects the envoy (low renown, no promotion history).

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (full lesser-house round trip + all canonical exclusions).
- Module export inspection: `promoteMemberToLesserHouse` resolves to function; `LESSER_HOUSE_RENOWN_THRESHOLD` resolves to 30.
- Syntax clean.

## Canonical Interdependency Check

Lesser houses now connect to:

1. **Dynasty member register** (renown + path + promotion history are the qualification signals).
2. **Legitimacy system** (founding a lesser house awards +3 legitimacy, capped at 100).
3. **Conviction ledger** (stewardship bucket +2 per founding, flows to band calculation, downstream to all conviction-modified systems).
4. **Dual-clock architecture** (`foundedAtInWorldDays` timestamps every branch).
5. **Save/resume** (automatic via existing JSON serialization).

Satisfies the Canonical Interdependency Mandate that every new system must touch ≥2 live systems.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Lesser houses promotion pipeline | DOCUMENTED | LIVE (candidate detection + player-consented promotion + UI + legitimacy/conviction rewards) |
| War hero identification for dynasty advancement | DOCUMENTED | PARTIAL (renown + path + promotion history used as proxy; direct battlefield-hero tagging still DOCUMENTED) |

## Remaining Lesser-House Canonical Layers

- Direct battlefield-hero tagging: members earn renown from specific battle events (kills, commander victories, successful sorties) rather than via manual edit.
- Lesser-house independent marriage candidacy (current marriage system only recognizes factions; cadet branches are strategic partners with their own loyalty).
- Lesser-house loyalty drift (loyalty should slowly rise on success, fall on neglect or parent-house defeat).
- Lesser-house defection (when loyalty collapses, cadet branches can split, join rival factions, or form independent minor houses on the map).
- Lesser-house unit-levy mechanic (cadet branches contribute small retinue units to parent faction).
- AI promotion logic: Stonehelm should promote its own eligible candidates rather than needing player intervention (current pipeline already tracks enemy candidates; AI decision hook missing).

## Session 36 Next Action

- AI marriage proposals (Stonehelm proposes to Player when alliance is strategically beneficial, canonical reciprocity for S33-34 marriage system).
- Or: AI lesser-house promotion logic (Stonehelm auto-promotes its own eligible candidates).
- Or: Sortie cooldown real-time tick-down in UI (legibility polish for the Session 27 commander sortie system).
- Or: Direct battlefield-hero renown award hook (wire combat events to grant renown to commanders and sortie leaders).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. 1 item moved DOCUMENTED → PARTIAL with explicit canonical follow-up. Vector 2 advanced for the third consecutive session. Tests green. Marriage + Lesser Houses together now form the first end-to-end player-actionable dynasty-formation loop.
