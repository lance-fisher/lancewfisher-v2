# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 36
Author: Claude

## Scope

AI marriage proposal reciprocity LIVE. Closes the marriage system reciprocity gap from Sessions 33-34: Stonehelm now proposes to Player when materially pressured, making marriage a two-way diplomatic channel rather than a player-only tool. The marriage UI inbox built in Session 34 now receives AI-originated proposals and the Accept Marriage button works against them with no additional UI changes. Vector 2 (Dynastic depth) advanced for the fourth consecutive session.

## Changes Landed

### AI marriage proposal hook (Vector 2 + Vector 3)

- `src/game/core/ai.js`:
  - Imported `proposeMarriage` from simulation.
  - New `enemy.ai.marriageProposalTimer` cooldown initialized at 90 seconds, decrements per tick.
  - When timer expires, `tryAiMarriageProposal(state)` runs. Three terminal states:
    - **`"proposed"`** — successful proposal sent to player; cooldown reset to 240 seconds (long).
    - **`"skip"`** — strategic gate fails; cooldown reset to 60 seconds (short retry).
  - On success, `updateEnemyAi` returns early (single major action per tick rule).

### `tryAiMarriageProposal(state)` strategic gates

All gates canonical and additive:

1. Both factions must have intact dynasties.
2. **No-double-marriage gate**: skip if any in-force marriage between enemy and player already exists.
3. **No-spam gate**: skip if a pending proposal from enemy is already in player's marriage inbox.
4. **Strategic gate** (either signal sufficient):
   - **Hostility signal**: enemy currently lists player in `hostileTo` (canonical: marriage de-escalates).
   - **Population deficit signal**: enemy population < 85% of player population (canonical: marriage as alliance-of-the-pressured).
5. **Member availability gates**: both sides must have a non-head, available, non-captured bloodline member to participate.
6. Sends proposal via `proposeMarriage(state, "enemy", enemyMemberId, "player", playerMemberId)`.

### UI compatibility

No `main.js` changes needed. The Session 34 marriage UI panel already:

- Surfaces `marriageProposalsIn` with status `"pending"` regardless of source faction.
- Renders an Accept Marriage button per proposal.
- Wires the button to the same `acceptMarriage(state, proposal.id)` call.

This is the canonical reuse the Session 34 architecture was designed for.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 36 block):
  - Force timer to zero, ensure hostility signal active, set status to `"playing"`.
  - Verify inbox starts empty (0 proposals).
  - Tick `updateEnemyAi`; verify exactly one new pending proposal in player inbox with `sourceFactionId === "enemy"`.
  - Re-tick; verify AI does NOT double-propose while one is still pending.
  - Player accepts via existing `acceptMarriage`; force timer to zero again.
  - Re-tick; verify AI does NOT propose again because the no-double-marriage gate fires.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (full AI marriage proposal lifecycle including all gates).
- Syntax clean.

## Canonical Interdependency Check

AI marriage proposals now connect to:

1. **Marriage system** (S33): direct call into `proposeMarriage`, all polygamy + cross-faction + member-validity gates inherited.
2. **Marriage UI** (S34): proposals surface in the inbox section automatically.
3. **Hostility tracking**: `enemy.hostileTo` array drives the strategic gate.
4. **Population system**: enemy/player population ratio drives the alternate strategic gate.
5. **AI cooldown framework**: timer-based pattern shared with sabotage reciprocity (S11) and naval reactivity (S28-29).

Satisfies Canonical Interdependency Mandate (≥2 live systems touched).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| AI marriage proposals (Stonehelm reciprocity) | DOCUMENTED | LIVE (cooldown + dual strategic gate + no-double-marriage / no-spam guards + UI integration) |

## Remaining AI Marriage Layers

- **AI marriage decline path**: AI currently never sends a decline; player proposals to AI sit until the marriage system gets a decline or expiration mechanic (still DOCUMENTED).
- **AI marriage proposal expiration**: timer-based auto-decline (still DOCUMENTED).
- **AI strategic-priority weighting**: current gate is binary (hostility OR deficit). Could weight by faith compatibility, conviction band, or specific member quality (deferred).
- **AI accept logic for player-originated proposals**: AI never accepts player proposals; player proposals only land in enemy inbox and sit (the AI inbox is never processed in `updateEnemyAi`). This is the next reciprocity gap.

## Session 37 Next Action

- AI accept logic for player-originated marriage proposals (closes the reciprocity gap fully — AI must process its own inbox).
- Or: AI lesser-house promotion logic (Stonehelm auto-promotes its own eligible candidates per Session 35 pipeline).
- Or: Marriage proposal expiration / decline path (UI + simulation hook for both sides).
- Or: Sortie cooldown real-time tick-down in UI (legibility polish for Session 27 sortie).
- Or: Direct battlefield-hero renown award hook (wire combat events to grant renown).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Vector 2 advanced fourth consecutive session. Tests green. Marriage system is now a true two-way diplomatic channel.
