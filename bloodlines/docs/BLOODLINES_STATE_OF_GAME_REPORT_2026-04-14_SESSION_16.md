# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 16
Author: Claude

## Scope

Longer-siege AI: repeated-assault window logic. After Session 14's post-repulse retreat, Stonehelm was stuck at the stage point indefinitely. Session 16 completes the loop: once cohesion-penalty cooldown expires, AI re-attempts the assault from a slightly repositioned angle. Capped at 4 attempts to prevent infinite loops and preserve canonical balance.

## Changes Landed

### Repeated-assault window (Vector 3)

- `src/game/core/ai.js` — new `else if` branch sits between post-repulse and supply-collapse. Fires when:
  - keep is fortified
  - `enemy.ai.postRepulseUntil` is in the past (cooldown cleared)
  - `enemy.ai.repeatedAssaultAttempts > 0` (has actually been repulsed)
  - attempts < 4 (canonical ceiling)
- Computes an offset angle based on attempt count (0.7 radians per attempt), places a virtual offset stage point 60 tiles from canonical stage. Re-issues attack-on-hall command. Canonical message: "Stonehelm renews the assault from a new angle (attempt N)."
- Clears `postRepulseUntil` after re-attempt to prevent per-tick loop.
- Post-repulse branch increments `repeatedAssaultAttempts` counter each time it fires, feeding the cycle.

After 4 attempts the AI falls through to normal commit logic, which will likely end up in the siege-stage branch if strain re-triggers, eventually cycling back through post-repulse. This is canonical: an attacker that can't crack the keep after multiple attempts should naturally shift focus or sustain the siege.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Longer-siege AI repeated-assault window | DOCUMENTED | LIVE |

## Longer-Siege AI Status After Session 16

All four canonical Session 9 longer-siege items are now either LIVE or PARTIAL:

- Relief-window awareness: LIVE (Session 12)
- Post-repulse retreat: LIVE (Session 14)
- Supply-collapse retreat: LIVE (Session 14)
- Repeated-assault window: LIVE (Session 16)
- Supply-protection patrols: still DOCUMENTED
- Weather / night tactics: still DOCUMENTED

## Session 17 Next Action

Highest leverage: **Faith Hall (L2)** as second-tier covenant building, unlocking L3 faith unit recruitment. Or: **Conviction milestone powers per band**.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green.
