# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 37
Author: Claude

## Scope

AI marriage inbox processing LIVE. Closes the marriage system reciprocity gap fully: player-originated proposals to Stonehelm now get processed by the AI rather than sitting indefinitely. The accept criterion mirrors the proposal criterion — AI accepts what AI would have proposed — so AI marriage behavior is symmetric and prevents drift between "AI offers generously but accepts nothing" or its inverse. Combined with Sessions 33-36, marriage is now a fully bidirectional diplomatic system with both sides able to initiate AND respond.

## Changes Landed

### AI inbox processing hook (Vector 2 + Vector 3)

- `src/game/core/ai.js`:
  - Imported `acceptMarriage` from simulation.
  - New `enemy.ai.marriageInboxTimer` cooldown initialized at 30 seconds, decrements per tick.
  - When timer expires, `tryAiAcceptIncomingMarriage(state)` runs.
  - On successful accept: cooldown reset to 180 seconds (long, prevents immediate re-evaluation).
  - On declined gate: cooldown reset to 30 seconds (fast retry, since strategic conditions may shift).
  - Cooldown is INDEPENDENT of the proposal cooldown so an accept can fire on the very next tick after a player proposal lands.

### `shouldAiAcceptMarriage(state)` pure helper

- Hostility signal: enemy lists player in `hostileTo`.
- Population-deficit signal: enemy population < 85% of player population.
- Either signal sufficient.
- Identical criterion to the proposal-side gate (S36 `tryAiMarriageProposal`). Symmetric design prevents drift.

### `tryAiAcceptIncomingMarriage(state)`

- Reads enemy's marriage inbox.
- Finds first pending proposal where `sourceFactionId === "player"`.
- If none pending: return false.
- If `shouldAiAcceptMarriage` returns false: return false (proposal stays pending — player can wait for strategic conditions to shift).
- Otherwise calls `acceptMarriage(state, proposal.id)` — inherits all canonical effects: mutual hostility drop, oathkeeping conviction +2 both sides, legitimacy +2 both sides, dual-clock 30-day wedding declaration.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 37 block):
  - **Accept-under-hostility test**: player sends proposal, hostility is set, AI inbox timer forced to 0; verify AI accepts on next tick, marriage is in force, proposal status flips from "pending" to non-pending.
  - **Refuse-when-no-gate test**: hostility cleared, populations equalized; player sends proposal; AI tick runs; verify proposal STAYS pending and no marriage is recorded.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (full AI inbox processing with both accept and refuse paths).
- Syntax clean.

## Canonical Interdependency Check

AI marriage acceptance now connects to:

1. **Marriage system** (S33): direct call into `acceptMarriage`, all canonical effects inherited.
2. **AI proposal mirror** (S36): symmetric strategic gate prevents drift.
3. **Hostility tracking**: drives gate.
4. **Population system**: drives alternate gate.
5. **Marriage UI inbox** (S34): proposals AI accepts disappear from player's view of "AI's open offers" since they're now in force.
6. **Conviction ledger**: oathkeeping +2 each side from `acceptMarriage`.
7. **Legitimacy system**: +2 each side from `acceptMarriage`.
8. **Dual-clock**: wedding declaration from `acceptMarriage`.

Satisfies Canonical Interdependency Mandate (≥2 live systems touched — exceeds it dramatically).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| AI accept of player-originated marriage proposals | DOCUMENTED | LIVE (cooldown + symmetric strategic gate + symmetric refusal) |
| Marriage system end-to-end bidirectionality | PARTIAL | LIVE (both sides can propose AND respond; previously asymmetric) |

## Remaining Marriage Layers

- Marriage proposal expiration / decline path (UI button + simulation hook for explicit decline; current state: AI implicitly "declines" by holding pending under failed gate, but can reverse later if strategic conditions shift).
- AI strategic-priority weighting beyond the binary hostility/deficit gate (faith compatibility, conviction band, member quality).
- Marriage death/dissolution mechanics.
- Mixed-bloodline defection slider runtime effect (mixed-bloodline children carry metadata since S33 but defection runtime effect still DOCUMENTED).
- Lesser-house independent marriage candidacy (cadet branches becoming valid marriage partners).
- Cross-house marriage diplomatic ripple (loyalty trickle, alliance candidacy beyond simple hostility removal).

## Session 38 Next Action

- AI lesser-house promotion logic (Stonehelm auto-promotes its own eligible candidates per Session 35 pipeline).
- Or: Marriage proposal expiration timer (deferred from S33; closes the "stale proposal" surface area).
- Or: Sortie cooldown real-time tick-down in UI (legibility polish for Session 27 sortie).
- Or: Direct battlefield-hero renown award hook (wire combat events to grant renown).
- Or: Faith-compatibility weighting in AI marriage decisions (extends S36-37 to incorporate Vector 4 Faith).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE, 1 item moved PARTIAL → LIVE. Vector 2 advanced fifth consecutive session. Marriage system is now a fully bidirectional diplomatic channel with symmetric AI behavior. Tests green.
