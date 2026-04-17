# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 38
Author: Claude

## Scope

AI lesser-house promotion logic LIVE. Closes the lesser-house pipeline reciprocity gap from Session 35: Stonehelm now auto-promotes its own eligible candidates rather than the player having no AI counterpart. Mirrors the architectural pattern from Sessions 36-37 where AI sides of player-created systems are layered in symmetrically. Vector 2 (Dynastic depth) advanced for the sixth consecutive session.

## Changes Landed

### AI lesser-house promotion hook

- `src/game/core/ai.js`:
  - Imported `promoteMemberToLesserHouse` from simulation.
  - New `enemy.ai.lesserHousePromotionTimer` cooldown initialized at 60 seconds, decrements per tick.
  - When timer expires, `tryAiPromoteLesserHouse(state)` runs.
  - On successful promotion: cooldown reset to 180 seconds (long, prevents cascade promotions in a single window).
  - On declined gate: cooldown reset to 45 seconds (medium retry).

### `tryAiPromoteLesserHouse(state)` strategic gates

All gates canonical and additive:

1. **Candidate availability**: Stonehelm's `dynasty.lesserHouseCandidates` (auto-flagged by the S35 `tickLesserHouseCandidates` shared simulation tick) must be non-empty.
2. **Legitimacy gate**: enemy `dynasty.legitimacy < 90`. A secure house has no need to spawn cadet branches that dilute the head's authority. This is the canonical strategic motivator — the AI promotes when legitimacy is the bottleneck, not as cosmetic expansion.
3. **Soft cap**: existing `lesserHouses.length < 3`. Prevents unbounded promotion that would dilute the dynasty register.
4. Promotes the FIRST candidate in the list (canonical: order is detection-order, which roughly correlates with seniority of accumulation).
5. Reuses `promoteMemberToLesserHouse` so all canonical effects fire automatically: lesser-house creation with dual-clock founding timestamp, founder member tagging, +3 legitimacy, +2 stewardship conviction, candidate-list pruning, canonical message.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 38 block):
  - **Promote-under-low-legitimacy test**: engineer enemy marshal as candidate, force timer to 0, verify exactly one lesser house created with the marshal as founder. Re-tick verifies AI does NOT promote a second time when no candidates remain (does not crash on empty list).
  - **Refuse-when-secure test**: engineer same candidate, set legitimacy to 95, force timer; verify AI does NOT promote because the strategic gate fails.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (full AI promotion lifecycle including refuse-when-secure path).
- Syntax clean.

## Canonical Interdependency Check

AI lesser-house promotion now connects to:

1. **Lesser-house pipeline** (S35): direct call into `promoteMemberToLesserHouse`, all canonical effects inherited.
2. **Candidate detection tick** (S35): reads from the same `dynasty.lesserHouseCandidates` array the player UI reads.
3. **Legitimacy system**: drives the strategic gate AND receives +3 reward.
4. **Conviction ledger**: +2 stewardship from `promoteMemberToLesserHouse`.
5. **Dual-clock**: founding timestamp on each cadet branch.
6. **AI cooldown framework**: pattern shared with sabotage (S11), naval (S28-29), marriage proposal (S36), marriage inbox (S37).

Satisfies Canonical Interdependency Mandate (≥2 live systems touched — exceeds dramatically).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| AI lesser-house promotion (Stonehelm cadet branch creation) | DOCUMENTED | LIVE (cooldown + legitimacy strategic gate + soft cap + symmetric reuse of player promotion path) |
| Lesser-house pipeline end-to-end bidirectionality | PARTIAL | LIVE (both player and AI exercise the same promotion path with mirrored logic) |

## Remaining Lesser-House Layers

- **Direct battlefield-hero renown award hook**: members earn renown from specific battle events (kills, commander victories, successful sorties) rather than via manual edit or accumulation. Currently DOCUMENTED.
- **Lesser-house independent marriage candidacy**: cadet branches as valid marriage partners with their own loyalty (currently only main factions participate in marriage).
- **Lesser-house loyalty drift**: loyalty rises on success, falls on neglect or parent defeat (currently fixed at 75 forever).
- **Lesser-house defection**: when loyalty collapses, cadet branches split, join rivals, or form independent minor houses on the map.
- **Lesser-house unit-levy mechanic**: cadet branches contribute small retinue units to parent faction.
- **Lesser-house political events**: marriages, deaths, scandals scoped to cadet branches.

## Vector 2 Status (Six Sessions of Continuous Advance)

Six sessions in a row have advanced Vector 2 (Dynastic depth):

- **S33**: Marriage system first canonical layer (propose/accept + polygamy gate + dual-clock gestation + mixed-bloodline child).
- **S34**: Marriage UI panel (legibility-follows-depth closure).
- **S35**: Lesser-house promotion pipeline (player path).
- **S36**: AI marriage proposal reciprocity.
- **S37**: AI marriage inbox processing (full bidirectionality).
- **S38**: AI lesser-house promotion logic (full bidirectionality on the cadet pipeline).

Vector 2 has advanced from "marriage system stagnant since Session 14" to a fully bidirectional dynasty-formation system with both marriage and cadet-branch creation operating end-to-end on both player AND AI sides.

## Session 39 Next Action

- Marriage proposal expiration timer (closes the "stale proposal" surface area; deferred from S33).
- Or: Direct battlefield-hero renown award hook (combat events grant renown, would activate the lesser-house pipeline more naturally over a real match).
- Or: Sortie cooldown real-time tick-down in UI (legibility polish for Session 27 sortie).
- Or: Lesser-house loyalty drift mechanic (foundation for future defection mechanics).
- Or: Faith-compatibility weighting in AI marriage decisions (extends S36-37 to incorporate Vector 4 Faith).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE, 1 moved PARTIAL → LIVE. Vector 2 advanced sixth consecutive session — longest single-vector advance streak in the project history. Tests green.
