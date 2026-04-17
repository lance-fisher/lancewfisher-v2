# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 34
Author: Claude

## Scope

Marriage UI panel LIVE. Closes the legibility-follows-depth requirement for Session 33's marriage system within the canonical 2-session window. Player can now propose marriages, accept inbound proposals, and see active marriages with child status, all from the Dynasty panel.

## Changes Landed

### Marriage UI in Dynasty panel (Vector 6)

- `src/game/main.js`:
  - Imported `proposeMarriage` and `acceptMarriage` from simulation.
  - Three new sections in `renderDynastyPanel`:
    - **Active marriages**: lists each marriage with the player member's title, a description of the spouse house ("wed to a {HouseName} bloodline"), and child status ("awaiting child" or "{N} child").
    - **Marriage proposals received**: each pending inbound proposal with source faction + member identity and an "Accept Marriage" action button. Action calls `acceptMarriage(state, proposal.id)` and re-renders.
    - **Propose Marriage button**: surfaces when player has eligible members and no inbound proposals queued. Targets the enemy faction's first available member (canonical first-layer convenience; richer chooser deferred). Action calls `proposeMarriage(state, "player", playerMemberId, "enemy", enemyMemberId)`.
  - All actions wire through real simulation functions, no decorative buttons.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- Syntax clean.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Marriage UI surface (legibility) | DOCUMENTED | LIVE |

## Remaining Marriage Canonical Layers

- Richer marriage chooser (player picks both source AND target member explicitly rather than canonical first-of-each).
- Marriage decline path (current UI has Accept only; reject defers to natural expiration).
- Marriage proposal expiration timer.
- Marriage death/dissolution mechanics.
- Mixed-bloodline defection slider runtime effect.
- AI marriage proposals (Stonehelm proposes when alliance is strategically beneficial).

## Session 35 Next Action

- Lesser houses promotion pipeline (Vector 2 continuation: war heroes earn lesser-house cadet branches).
- Or: AI marriage proposal logic.
- Or: Sortie cooldown real-time tick-down in UI (legibility polish).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green. Marriage system now end-to-end player-actionable.
