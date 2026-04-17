# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-14
Session: 19
Author: Claude

## Scope

Dark-extremes world pressure trigger now LIVE. Canonical master doctrine section XX: sustained Apex Cruel conviction attracts world reaction. First canonical layer: tribe AI raids 40% more often against any kingdom that sits at Apex Cruel for 3+ consecutive realm cycles.

## Changes Landed

### Dark-extremes streak tracking (Vector 4 + Vector 5)

- `src/game/core/simulation.js` — `tickRealmConditionCycle` extended: each cycle, if a faction's `conviction.bandId === "apex_cruel"`, increment `faction.darkExtremesStreak`. Otherwise decrement. When streak crosses threshold 3, set `faction.darkExtremesActive = true` and fire a canonical message ("The world takes notice of X's Apex Cruel descent. Tribal pressure intensifies.").
- Threshold 3 cycles = 270 seconds of sustained Apex Cruel. Canonical: dynastic atrocity must be sustained to draw world reaction, not transient.

### Tribe AI reactivity (Vector 5)

- `src/game/core/ai.js` `updateNeutralAi` — raid timer now scales 0.6x when any kingdom is `darkExtremesActive`. Tribes raid more frequently. Canonical: world pressure materially reacts to conviction.

### Legibility (Vector 6)

- Snapshot `worldPressure` block now exposes `darkExtremesActive` and `darkExtremesStreak`.

### Test coverage

- `tests/runtime-bridge.mjs` — fresh sim has darkExtremes inactive and streak 0; forcing Apex Cruel + streak 3 surfaces darkExtremesActive true in snapshot.

## Verification

- `node tests/data-validation.mjs` — passed.
- `node tests/runtime-bridge.mjs` — passed.
- All syntax checks pass.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Dark-extremes world pressure trigger | DOCUMENTED | LIVE (first canonical layer: tribe AI reacts to sustained Apex Cruel) |

## Session 20 Next Action

- L3 faith unit rosters (8 units: 2 per covenant per doctrine path) — Covenant Hall is now live and canonically unlocks L3 recruitment.
- Or: canonical 11th pressure state wiring (some of the snapshot fields are live but not all wired into alerts).

## Preservation

No canon reduced. 1 item moved DOCUMENTED → LIVE. Tests green.
