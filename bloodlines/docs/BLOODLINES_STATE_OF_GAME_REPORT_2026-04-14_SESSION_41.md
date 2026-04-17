# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 41
Author: Claude

## Scope

Sortie cooldown UI tick-down LIVE — Vector 6 (Legibility) polish session breaking the eight-session Vector 2 streak. The Session 27 commander sortie system already had a cooldown timer in state and a button label that displayed remaining seconds, but the label was bare ("Ns until next sortie is ready") and lacked any sense of progress. The active-window state had no remaining-seconds exposure at all. This session adds a percentage-recharged readout to the cooldown, a percentage-spent readout to the active window, and exposes the canonical durations on the realm-condition snapshot so the UI no longer needs to know the constants.

## Changes Landed

### Realm-condition snapshot expansion (`src/game/core/simulation.js`)

Three new fields under the snapshot's `fortification` block:

- `sortieActiveRemaining` — seconds remaining in the burst window (mirror of `sortieCooldownRemaining` for the active state). Was implicit before; now first-class.
- `sortieDurationSeconds` — value of `SORTIE_DURATION_SECONDS` (canonical 12s).
- `sortieCooldownSeconds` — value of `SORTIE_COOLDOWN_SECONDS` (canonical 60s).

These are exposed so the UI can render progress percentages without importing constants directly. Maintains the snapshot-as-truth pattern used elsewhere.

### Sortie button label rewrite (`src/game/main.js`)

- **Active state**: now reads "Ns of 12s remaining at {keep} (P% of burst spent)" instead of just "Defenders surging at {keep}".
- **Cooldown state**: now reads "Ns until ready • P% recharged" instead of just "Ns until next sortie is ready".
- Both percentages computed from the snapshot fields. Rounded to integer percent for clean readability.

The render loop (`performFrame` in `main.js:1004`) calls `renderPanels()` every animation frame, so the percentage updates smoothly at 60fps without any additional timer plumbing. The "tick-down" effect comes for free from the existing per-frame render — what was missing was the right text and the data to compute it.

### Test coverage

- `tests/runtime-bridge.mjs` (Session 41 block):
  - Snapshot includes `sortieDurationSeconds`, `sortieCooldownSeconds`, `sortieActiveRemaining`.
  - All are numbers.
  - `sortieDurationSeconds > 0`.
  - `sortieCooldownSeconds > sortieDurationSeconds` (canonical: 60 > 12).
  - `sortieActiveRemaining >= 0` (never negative).

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed (snapshot field exposure + ordering invariants).
- Syntax clean.

## Canonical Interdependency Check

Sortie UI tick-down now connects to:

1. **Sortie system** (S27): reads `sortieActiveUntil` and `sortieCooldownUntil` via the snapshot.
2. **Realm-condition snapshot** (S6+): canonical pattern of UI reading from snapshot rather than state directly.
3. **Render loop** (`performFrame`): re-renders panels per frame at 60fps so the displayed percentage moves smoothly.
4. **Vector 6 Legibility**: every numeric in-progress mechanic should read as a gauge to the player; this brings sortie into compliance with that posture.

Satisfies Canonical Interdependency Mandate (≥2 live systems touched).

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Sortie cooldown UI tick-down (Vector 6 polish) | DOCUMENTED | LIVE (active-remaining + cooldown-recharged percentages, snapshot duration constants exposed, smooth per-frame update) |

## Vector Distribution After Session 41

| Vector | Last Advanced | Status |
|---|---|---|
| 1 Civilizational | Earlier sessions | Established |
| 2 Dynastic | S33-S40 (8 consecutive) | Functional first layer complete |
| 3 Military | Earlier sessions | Established |
| 4 Faith/Conviction | Earlier sessions | Established |
| 5 World/Map | Earlier sessions (S32 continental) | Established |
| 6 Legibility | S41 (this session) | Polished |

Vector 6 was overdue for advance — the Vector 2 streak (S33-S40) added significant systems but each session's UI changes were minimal (single button or panel section). This session catches up Vector 6 on a high-traffic mechanic.

## Session 42 Next Action

- Lesser-house loyalty drift mechanic (foundation for future defection mechanics; cadet-branch loyalty currently fixed at 75 forever).
- Or: Mixed-bloodline defection slider runtime (children with cross-house metadata could shift faction loyalty).
- Or: Faith-compatibility weighting in AI marriage decisions.
- Or: Lesser-house unit-levy mechanic.
- Or: Marriage death/dissolution mechanics.
- Or: Vector 1 Civilizational advance (lesser-house entry into the houses register as full first-class faction-equivalents).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Vector 6 advanced after a long stretch of Vector 2 focus. Tests green. Sortie button now reads as a true gauge, completing the legibility-follows-depth posture for the Session 27 sortie system.
