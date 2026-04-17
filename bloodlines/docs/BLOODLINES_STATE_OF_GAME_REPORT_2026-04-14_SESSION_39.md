# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 39
Author: Claude

## Scope

Marriage proposal expiration timer + explicit decline path LIVE. Closes the deferred-from-S33 stale-proposal surface area: pending marriage offers now lapse to "expired" after 90 in-world days, and either side can call `declineMarriage` to resolve immediately. The marriage UI surfaces a Decline button alongside Accept. Vector 2 (Dynastic depth) advanced for the seventh consecutive session.

## Changes Landed

### Expiration constant + tick (`src/game/core/simulation.js`)

- New exported constant `MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 90`. Canonical: 3x the wedding-window (30 days) gives both sides ample time to weigh strategic value but bounds the diplomatic register.
- New `tickMarriageProposalExpiration(state)` runs in `stepSimulation` after `tickMarriageGestation` and before `tickLesserHouseCandidates`.
- Iterates every faction's `marriageProposalsIn` and `marriageProposalsOut`.
- For each proposal with status `"pending"` whose `proposedAtInWorldDays` is at least `MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS` in the past:
  - Status flipped to `"expired"`.
  - `expiredAtInWorldDays` recorded.
  - Single canonical message emitted, deduplicated by checking the source-side record only.

### Explicit decline (`declineMarriage(state, proposalId)` exported)

- Locates proposal in any faction's inbox.
- Refuses if status is not `"pending"` (already accepted/declined/expired).
- Marks both inbox AND source-side outbox records as `"declined"` with `declinedAtInWorldDays`.
- Pushes canonical message.
- Frees both members for new arrangements (since `memberHasActiveMarriage` only counts marriages, not pending or declined proposals — already correct from S33 design).

### Marriage UI Decline button (`src/game/main.js`)

- Imported `declineMarriage`.
- For each pending inbound proposal, the Accept Marriage button is now joined by a Decline button labeled "Frees both members; offer lapses now".
- Both buttons wire through real simulation functions per canonical no-decorative-UI mandate.

### Save/resume compatibility

- New proposal fields (`expiredAtInWorldDays`, `declinedAtInWorldDays`, status enum extended with `"expired"` and `"declined"`) are part of the deep-cloned `dynasty` snapshot, so they restore automatically.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 39 expiration block):
  - Initial proposal succeeds.
  - At expiration_threshold - 5 days, status remains pending.
  - At expiration_threshold + 5 days, status flips to "expired", outbox mirror also flips.
  - After expiration, the same member can be re-proposed (proves expiration frees the member).
- `tests/runtime-bridge.mjs` (Session 39 decline block):
  - Enemy proposes to player.
  - Player calls `declineMarriage`; both inbox and outbox flip to "declined".
  - Re-declining a resolved proposal fails.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (expiration timing both sides of threshold + explicit decline + double-decline guard).
- Syntax clean.

## Canonical Interdependency Check

Marriage proposal lifecycle now connects to:

1. **Marriage system** (S33): proposal records gain expired/declined status.
2. **Dual-clock** (S25): expiration measured in in-world days; survives save/resume.
3. **Marriage UI** (S34): Decline button surfaces explicit player consent path.
4. **AI marriage proposal** (S36): expired/declined proposals are not "pending", so the AI's no-spam gate (which checks pending status) correctly allows AI to re-propose after lapse.
5. **AI marriage inbox** (S37): same — symmetric reciprocity preserved.

Satisfies Canonical Interdependency Mandate (≥2 live systems touched).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Marriage proposal expiration | DOCUMENTED | LIVE (90 in-world days, dual-clock-driven, free-the-member behavior verified) |
| Marriage explicit decline path | DOCUMENTED | LIVE (`declineMarriage` exported + UI button + double-resolution guard) |

## Marriage System Final Layer Status After S33-S39

| Layer | Status |
|---|---|
| Cross-faction proposal/accept | LIVE (S33) |
| Polygamy gate (faith-restricted) | LIVE (S33) |
| Dual-clock gestation + child generation | LIVE (S33) |
| Mixed-bloodline child metadata | LIVE (S33), defection runtime DOCUMENTED |
| Marriage UI panel | LIVE (S34) |
| AI marriage proposal | LIVE (S36) |
| AI marriage inbox processing | LIVE (S37) |
| Marriage proposal expiration | LIVE (S39) |
| Marriage explicit decline | LIVE (S39) |
| Marriage death/dissolution | DOCUMENTED |
| Lesser-house marriage candidacy | DOCUMENTED |
| Cross-house diplomatic ripple beyond hostility removal | DOCUMENTED |
| Faith-compatibility weighting in AI marriage | DOCUMENTED |

The marriage system is now functionally complete on its first canonical layer (propose → accept/decline/expire → child generation, both sides automated). Remaining items are downstream extensions that depend on systems not yet built (lesser-house marriage requires lesser-house faction-equivalence, dissolution requires death events, etc.).

## Session 40 Next Action

- Direct battlefield-hero renown award hook (combat events grant renown — would activate the lesser-house pipeline naturally over a real match without manual member edits).
- Or: Sortie cooldown real-time tick-down in UI (legibility polish for Session 27 sortie).
- Or: Lesser-house loyalty drift mechanic (foundation for future defection mechanics).
- Or: Faith-compatibility weighting in AI marriage decisions.
- Or: Mixed-bloodline defection slider runtime (children with cross-house metadata could shift faction loyalty).

## Preservation

No canon reduced. 2 items moved DOCUMENTED → LIVE. Vector 2 advanced seventh consecutive session. Marriage system reaches functional completion on its canonical first layer. Tests green.
