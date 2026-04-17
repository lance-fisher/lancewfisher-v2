# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 53
Author: Codex

## Scope

Marriage death and dissolution mechanics are now LIVE as the first real mortality-consequence layer inside the dynastic marriage system. A union no longer persists forever once formed. Real bloodline death now dissolves the shared marriage record, ends further gestation, alters legitimacy and conviction, and becomes legible in the dynasty panel.

## Changes Landed

### Death-driven marriage dissolution runtime (`src/game/core/simulation.js`)

- Added first-layer marriage dissolution constants for dynastic legitimacy loss and oathkeeping mourning response.
- Added shared marriage-participant and mirror-record helpers so the runtime modifies the existing marriage record pair instead of creating a parallel death ledger.
- Added `tickMarriageDissolutionFromDeath(state)` and wired it into `stepSimulation` after battlefield death resolution.
- Marriage dissolution now triggers when either spouse has truly fallen in runtime state.
- Dissolution stamps the live marriage records with:
  - `dissolvedAt`,
  - `dissolvedAtInWorldDays`,
  - `dissolutionReason`,
  - `deceasedMemberId`,
  - `deceasedMemberIds`.
- Both houses now take a dynastic legitimacy hit when the union is broken by death.
- Both houses now record an oathkeeping conviction event for mourning rites and dynastic closure.
- Death-driven dissolution now pushes a real message-log event instead of silently mutating dynasty state.

### Gestation and dynastic consequence integration (`src/game/core/simulation.js`)

- Marriage gestation now halts on dissolved unions.
- This makes bloodline death matter beyond flavor:
  - posthumous child generation is blocked,
  - `memberHasActiveMarriage` and `factionHasActiveMarriageWithHouse` already stop counting the union because both were previously keyed to `!dissolvedAt`,
  - mixed-bloodline buffering from an active cross-house marriage now falls away automatically after dissolution.

### Legibility (`src/game/main.js`)

- The dynasty panel now distinguishes:
  - active marriages,
  - marriages ended by death.
- Active marriage rows now pluralize child count correctly.
- Death-ended marriages now show:
  - which union ended,
  - that the cause was death,
  - recorded child count,
  - dissolution timing in in-world days.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 53 runtime coverage proving:
  - a real spouse death through the commander-fall battlefield path dissolves the marriage,
  - both mirror records carry dissolution state,
  - the surviving house loses legitimacy,
  - the surviving house gains an oathkeeping mourning response,
  - gestation stops after dissolution,
  - dissolved marriage state persists through snapshot export and restore.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.
- `python -m http.server 8057 --directory D:/ProjectsHome/Bloodlines` served `play.html` successfully.
- Static browser-lane verification confirmed served markup still contains `game-shell`, `resource-bar`, `realm-condition-bar`, `dynasty-panel`, `faith-panel`, and `message-log`.

## Canonical Interdependency Check

Session 53 connects:

1. Dynastic death and battlefield mortality, bloodline death now terminates a live marriage rather than only filling the fallen ledger.
2. Dynastic legitimacy, both houses lose legitimacy when the union breaks by death.
3. Conviction and oathkeeping, both houses now register mourning rites in the live conviction ledger.
4. Mixed-bloodline stability, active-marriage buffering can now disappear because dissolution is real state, not a lore note.
5. Legibility, the dynasty panel now surfaces widowed and death-ended unions directly.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Marriage death and dissolution from live bloodline death | DOCUMENTED | LIVE (first layer, with legitimacy loss, oathkeeping response, gestation halt, UI legibility, and snapshot persistence) |

## Session 54 Next Action

Water-infrastructure tier 1 and army sustainment beyond the siege-engine chain. The dynastic lane now has a real death-consequence layer. The next highest-leverage civilizational and military seam is to make water infrastructure and field-army sustainment interact outside the existing siege-engine supply chain.
