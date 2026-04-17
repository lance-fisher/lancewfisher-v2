# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 43
Author: Claude

## Scope

Lesser-house defection event hook LIVE. Closes the consequence gap from S42's loyalty drift: a cadet branch whose loyalty falls to (or below) the canonical breaking point AND remains below for a 5 in-world-day grace period defects. Status flips to `"defected"`, departure timestamp recorded, parent loses 6 legitimacy and gains 1 ruthlessness conviction. Recovery during the grace window cancels the defection. Vector 2 advanced.

## Changes Landed

### Defection constants (`src/game/core/simulation.js`)

- `LESSER_HOUSE_DEFECTION_THRESHOLD = 0` — canonical breaking point.
- `LESSER_HOUSE_DEFECTION_GRACE_DAYS = 5` — corrective-action window.
- `LESSER_HOUSE_DEFECTION_LEGITIMACY_PENALTY = 6` — twice the +3 legitimacy bonus from founding (S35), so a defection actively damages the parent below the founding baseline.

### Defection block in `tickLesserHouseLoyaltyDrift`

- Restructured the per-cadet loop so the defection check runs even when no drift is applied this tick (previously the `elapsedDays <= 0` early-return blocked the defection check entirely; now drift is conditional and the defection check is unconditional).
- When `lh.status === "active"` AND `newLoyalty <= LESSER_HOUSE_DEFECTION_THRESHOLD`:
  - Sets `lh.brokeAtLoyaltyZeroInWorldDays` to current day if not already set (grace clock starts).
  - If the grace window has elapsed: flips status to `"defected"`, records `departedAtInWorldDays`, calls `adjustLegitimacy(parent, -6)`, calls `recordConvictionEvent(parent, "ruthlessness", +1, "lesser_house_defected")`, pushes canonical message.
- Recovery branch: when `lh.status === "active"` AND `newLoyalty > LESSER_HOUSE_DEFECTION_THRESHOLD` AND grace clock is set, clears `brokeAtLoyaltyZeroInWorldDays`. Active branches recover gracefully without a status flip.
- Already-`"defected"` branches skip via the early `if (lh.status !== "active") return;` so they cannot re-trigger or accumulate further legitimacy loss.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 43 defection block):
  - Engineer candidate + promotion. Force loyalty to 0 AND weaken parent (legitimacy 10 + ruthlessness 30) so drift cannot rescue.
  - Tick once: grace clock starts, status still active.
  - Push 30 in-world days + tick: status flips to `"defected"`, departed timestamp set, legitimacy dropped.
  - Push another 30 days + tick: legitimacy unchanged (no double-charge on already-defected branches).
- `tests/runtime-bridge.mjs` (Session 43 recovery block):
  - Engineer candidate + promotion. Force loyalty to 0; tick once → grace clock starts.
  - Recover loyalty to 50 (above threshold); tick once → grace clock cleared, status remains `"active"`.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (defection trigger + grace window + recovery + idempotency on already-defected).
- Syntax clean.

## Canonical Interdependency Check

Lesser-house defection now connects to:

1. **Loyalty drift** (S42): is the input signal.
2. **Legitimacy system**: parent loses 6 on defection.
3. **Conviction ledger**: ruthlessness +1 on defection (canonical: losing a vassal damages the parent's moral standing).
4. **Dual-clock**: grace clock and departure timestamp are in-world days; survives save/resume.
5. **Message system**: canonical defection event broadcast to player.
6. **Lesser-house pipeline** (S35-S40): defected branches are now distinct from active in all subsequent code paths (status check).

Satisfies Canonical Interdependency Mandate dramatically (≥5 live systems touched).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Lesser-house defection event hook | DOCUMENTED | LIVE (status flip + dual-clock departure timestamp + parent legitimacy/conviction penalties + grace window with recovery path + idempotency on already-defected) |
| Lesser-house full vassal-politics loop | NOT-STARTED | FOUNDATION (defected branches as new minor faction / rival vassal still DOCUMENTED for future session) |

## Lesser-House System Status After S35-S43

| Layer | Status |
|---|---|
| Candidate detection | LIVE (S35) |
| Player promotion | LIVE (S35) |
| AI promotion | LIVE (S38) |
| Battlefield-driven renown accumulation | LIVE (S40) |
| Loyalty drift | LIVE (S42) |
| Defection event hook | LIVE (S43) |
| Defected-branch as new minor faction | DOCUMENTED |
| Lesser-house unit-levy mechanic | DOCUMENTED |
| Lesser-house marriage candidacy | DOCUMENTED |
| Lesser-house political events | DOCUMENTED |

## Session 44 Next Action

- Defected-branch as new minor faction (the runtime payoff: defected cadets become independent minor houses on the map with their own stance toward parent).
- Or: Mixed-bloodline defection slider runtime (S33 children carry mixedBloodline metadata; runtime effect still DOCUMENTED).
- Or: Faith-compatibility weighting in AI marriage decisions.
- Or: Marriage death/dissolution mechanics.
- Or: Vector 1 Civilizational advance (lesser-house entry into the houses register as full first-class faction-equivalents).
- Or: Vector 4 Faith advance.

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE, 1 moved NOT-STARTED → FOUNDATION. Vector 2 advanced again. Tests green. Lesser houses now express full vassal-politics consequences: success reinforces, neglect erodes, total neglect breaks the bond.
