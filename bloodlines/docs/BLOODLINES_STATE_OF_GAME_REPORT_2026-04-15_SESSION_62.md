# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 62
Author: Codex

## Scope

Marriage and cadet-house systems have been advanced from indirect interaction into an explicit branch-stability layer. Mixed-bloodline lesser houses now carry a live marital-anchor profile derived from the marriage ties that created them, active anchors materially soften branch drift, dissolved anchors become a loyalty penalty, renewed hostility fractures the branch harder, the dynasty panel exposes the new state, and restore preserves the added cadet-house continuity.

## Changes Landed

### Cadet-house anchor runtime (`src/game/core/simulation.js`)

- Added `getRelevantMarriageWithHouse(faction, houseId)` to locate the active or most recent dynastic union tying a parent house to the outside house carried in a mixed-bloodline cadet branch.
- Added `getLesserHouseMaritalAnchorProfile(state, parentFaction, lesserHouse)` to convert that marriage state into a live branch-stability profile:
  - anchor house identity,
  - anchor marriage identity,
  - anchor status,
  - loyalty drift modifier,
  - branch-child support count.
- Mixed-bloodline drift is now split into two layers:
  - raw outside-bloodline pressure remains a baseline negative pull,
  - marriage-anchor state now adds or subtracts a second live pressure term.
- Active anchor now grants positive branch stability, strengthened by existing branch children.
- Dissolved anchor now becomes a negative drift term instead of simply removing a hidden buffer.
- Renewed hostility against the outside house now fractures the branch harder and changes the anchor state accordingly.
- Lesser-house records now persist:
  - `maritalAnchorHouseId`,
  - `maritalAnchorMarriageId`,
  - `maritalAnchorStatus`,
  - `maritalAnchorPressure`,
  - `maritalAnchorChildCount`,
  - `lastMaritalAnchorStatus`.
- Loyalty-drift resolution now emits one-time status-change messages when a branch becomes newly anchored, strained, dissolved, or fractured.

### Promotion-time continuity (`src/game/core/simulation.js`)

- Cadet-house promotion now initializes marital-anchor state immediately when the founder already carries a mixed-bloodline branch tie.
- The new cadet-house anchor fields survive snapshot export and restore through the existing dynasty-state persistence lane.

### Legibility (`src/game/main.js`)

- Lesser-house rows now surface:
  - mixed-bloodline drift,
  - marriage-anchor house,
  - marriage-anchor status,
  - marriage-anchor drift,
  - branch-child count when present.
- This keeps the new cadet-house stability layer inside the existing dynasty panel rather than hiding it in silent loyalty math.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 62 runtime coverage proving:
  - a mixed-bloodline cadet branch records an active marriage anchor,
  - active anchor adds positive loyalty support,
  - death-ended marriage flips the anchor to dissolved and worsens branch drift,
  - renewed hostility fractures the anchor and worsens pressure further,
  - snapshot export and restore preserve the new cadet-house anchor state.

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

Session 62 connects:

1. Marriage and lesser-house loyalty drift, because cadet stability now reads active, dissolved, or fractured marriage ties directly.
2. Marriage death consequence and cadet-house pressure, because death-ended unions now worsen branch drift instead of only dissolving the marriage record.
3. Mixed-bloodline identity and cadet-house stability, because raw outside-house pressure is now separated from marriage-anchor control.
4. Dynastic legibility and branch governance, because the dynasty panel now surfaces branch anchor status, pressure, and child support.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Lesser-house marital anchoring and controlled mixed-bloodline branch pressure | DOCUMENTED | LIVE (first layer, with active or dissolved branch anchors, hostility-driven fracture, dynasty-panel legibility, and restore continuity) |

## Session 63 Next Action

Return to the house layer, specifically Hartvale playability and house-gated unique-unit enablement. Stonehelm is already playable, Hartvale's Verdant Warden already exists in data, and the remaining missing layer is real runtime gating so additional houses do not pretend to be playable through dead data alone.
