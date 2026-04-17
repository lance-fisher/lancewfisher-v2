# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 42
Author: Claude

## Scope

Lesser-house loyalty drift mechanic LIVE. Closes the "cadet branches are inert decorations" gap from Sessions 35/38: each lesser house now drifts in loyalty per in-world day based on the parent faction's current standing. Healthy parents (high legitimacy, oathkeeping, no recent fallen) reinforce loyalty; weak parents (low legitimacy, ruthlessness band, bleeding ledger) erode it. Threshold-crossing messages fire ONCE per drop into wavering / dangerously thin / ready-to-break states. Foundation laid for future defection mechanics. Vector 2 advanced ninth (non-consecutive) session of the run.

## Changes Landed

### Loyalty drift constants and helpers (`src/game/core/simulation.js`)

- New exported constants:
  - `LESSER_HOUSE_LOYALTY_MIN = 0`
  - `LESSER_HOUSE_LOYALTY_MAX = 100`
- New internal `computeLesserHouseDailyLoyaltyDelta(state, parentFaction)`:
  - +0.30 if legitimacy ≥ 75 (parent looks strong and just)
  - +0.15 if legitimacy ≥ 50 (modestly strong)
  - −0.40 if legitimacy < 30 (weak — defection becomes thinkable)
  - +0.20 per oathkeeping bucket step above neutral (every 5 oathkeeping = one band step)
  - −0.20 per ruthlessness bucket step above neutral (parent feared, not loved)
  - −0.50 if any fallen members exist in the recent ledger (the realm is bleeding)
- New `tickLesserHouseLoyaltyDrift(state)`:
  - Iterates each faction's lesser houses each tick.
  - Uses `Math.floor(inWorldDays)` for snapshot timestamps so drift fires on integer day crossings (sub-day idempotent).
  - Tracks per-cadet `lastLoyaltyTickInWorldDays` and applies `delta * elapsedDays`.
  - Clamps loyalty to `[LESSER_HOUSE_LOYALTY_MIN, LESSER_HOUSE_LOYALTY_MAX]`.
  - Emits canonical messages at thresholds 25 / 10 / 0, deduped via per-cadet `announcedLoyaltyThresholds` array so each crossing fires ONCE.

### Wired into stepSimulation

- `tickLesserHouseLoyaltyDrift(state)` runs each tick after `tickLesserHouseCandidates`. No new global state, no new performance-relevant data structures — reads from existing legitimacy, conviction buckets, fallen ledger.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 42 block):
  - Promote a candidate to seed a lesser house. Snapshot baseline loyalty.
  - **Healthy-parent test**: legitimacy 90, push 30 in-world days, verify loyalty did NOT drop.
  - **Weak-parent test**: legitimacy 20, ruthlessness 30, fallen ledger seeded, push 30 days, verify loyalty fell below the healthy-state value.
  - **Floor clamp**: collapse to legitimacy 0 + ruthlessness 100 + push 365 days; verify loyalty ≥ 0.
  - **Cap clamp**: restore to legitimacy 100 + oathkeeping 50, reset baseline, push 365 days; verify loyalty ≤ 100.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (drift direction + clamp invariants).
- Syntax clean.

## Canonical Interdependency Check

Lesser-house loyalty drift now connects to:

1. **Lesser-house pipeline** (S35): updates the same `lesserHouses[].loyalty` field set by `promoteMemberToLesserHouse`.
2. **Legitimacy system**: drives healthy/weak parent classification.
3. **Conviction system**: oathkeeping + ruthlessness buckets feed drift directly.
4. **Fallen ledger** (existing dynasty attachments): seeds the bleeding-realm penalty.
5. **Dual-clock** (S25): `inWorldDays` floor drives day-crossing snapshots; survives save/resume.
6. **Message system**: canonical threshold crossings emit deduped warnings.

Satisfies Canonical Interdependency Mandate dramatically (≥6 live systems touched).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Lesser-house loyalty drift | DOCUMENTED | LIVE (per-day deltas driven by legitimacy + conviction + fallen ledger; clamped 0-100; threshold messages deduped) |
| Lesser-house defection foundation | NOT-STARTED | FOUNDATION (loyalty can now reach 0; defection event hook still DOCUMENTED) |

## Lesser-House System Status After S35-S42

| Layer | Status |
|---|---|
| Candidate detection | LIVE (S35) |
| Player promotion | LIVE (S35) |
| AI promotion | LIVE (S38) |
| Battlefield-driven renown accumulation | LIVE (S40) |
| Loyalty drift | LIVE (S42) |
| Defection event hook | DOCUMENTED (foundation laid this session) |
| Lesser-house unit-levy mechanic | DOCUMENTED |
| Lesser-house marriage candidacy | DOCUMENTED |
| Lesser-house political events | DOCUMENTED |

## Session 43 Next Action

- Lesser-house defection event hook (loyalty ≤ 0 → cadet branch leaves parent, joins rival or splits independent).
- Or: Mixed-bloodline defection slider runtime (S33 children carry mixedBloodline metadata; runtime effect still DOCUMENTED).
- Or: Faith-compatibility weighting in AI marriage decisions.
- Or: Marriage death/dissolution mechanics.
- Or: Vector 1 Civilizational advance.
- Or: Vector 4 Faith advance (faith-driven AI behavior beyond marriage gate).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE, 1 moved NOT-STARTED → FOUNDATION. Vector 2 advanced again. Tests green. Lesser houses now express vassal politics: success reinforces loyalty, neglect erodes it.
