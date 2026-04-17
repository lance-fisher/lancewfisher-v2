# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 52
Author: Codex

## Scope

Faith-compatibility weighting in AI marriage proposal and acceptance logic is now LIVE as the first real runtime layer. Covenant and doctrine fit now materially changes whether AI sees a dynastic union as acceptable pressure relief, legitimacy repair, or a fractured match that requires stronger strategic pressure.

## Changes Landed

### Shared marriage faith-compatibility runtime (`src/game/core/simulation.js`)

- Added `getMarriageFaithCompatibilityProfile(state, sourceFactionId, targetFactionId)` as a shared live profile for cross-dynasty covenant fit.
- The compatibility profile now reads real faction faith state:
  - selected covenant,
  - doctrine path,
  - covenant identity from `data/faiths.json`.
- Compatibility is now classified into live tiers:
  - `unbound`,
  - `harmonious`,
  - `sectarian`,
  - `strained`,
  - `fractured`.
- The helper also exposes whether compatibility is strong enough to support legitimacy-repair marriages and whether a fractured match should block weak one-signal proposals.

### AI marriage proposal and acceptance weighting (`src/game/core/ai.js`)

- Replaced the old pure hostility-or-population-deficit gate with a faith-aware strategic profile.
- AI marriage logic now evaluates:
  - hostility to the player,
  - population deficit versus the player,
  - dynastic legitimacy distress,
  - covenant and doctrine compatibility.
- Compatible covenant matches can now open a new live proposal and acceptance path when enemy legitimacy is weak enough, even without hostility or population deficit.
- Fractured covenant matches now block proposal or acceptance when only a single weak strategic signal exists.
- Proposal and acceptance decisions now stamp a live `aiEvaluation` payload onto the shared proposal record so the player can inspect:
  - compatibility tier,
  - compatibility label,
  - decision path,
  - signal count,
  - whether the match was blocked by faith.

### Legibility (`src/game/main.js`)

- The dynasty panel now surfaces covenant stance for:
  - incoming marriage proposals,
  - outgoing pending marriage offers,
  - the player-side marriage proposal action.
- Outgoing offers now show whether the enemy court is:
  - receptive because compatible covenant fit can repair legitimacy,
  - evaluating the match as strategic pressure relief,
  - resisting on covenant grounds,
  - or seeing no immediate strategic need.
- This closes the new AI marriage layer into an already-live surface instead of adding a dead panel.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 52 runtime coverage proving:
  - harmonious covenant alignment plus legitimacy distress lets AI propose without hostility or population deficit,
  - harmonious covenant alignment plus legitimacy distress lets AI accept a player proposal without hostility or population deficit,
  - fractured covenant alignment blocks AI proposal when only one hostility signal exists,
  - fractured covenant alignment blocks AI acceptance under the same weak-pressure condition,
  - proposal records persist the AI faith-evaluation metadata used by the dynasty panel.

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
- Static browser-lane verification confirmed served markup still contains `game-shell`, `resource-bar`, `dynasty-panel`, `faith-panel`, and `message-log`.

## Canonical Interdependency Check

Session 52 connects:

1. Faith and doctrine, covenant identity now shapes dynastic diplomacy instead of living only in doctrine and ward math.
2. Dynastic legitimacy, compatible faith can open a live legitimacy-repair marriage path.
3. AI diplomacy, proposal and acceptance behavior now diverges under harmonious versus fractured faith pairings.
4. Legibility, the dynasty panel now exposes covenant stance and AI court posture on pending unions.
5. Mixed-bloodline continuity, marriage filtering now changes which unions become active and therefore which mixed-bloodline consequences can emerge later.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Faith-compatibility weighting in AI marriage diplomacy | DOCUMENTED | LIVE (first layer, integrated into AI proposal and acceptance logic plus dynasty-panel legibility) |
| Polygamy restricted to Blood Dominion and Wild | DOCUMENTED | LIVE (foundation, enforced in marriage runtime) |

## Session 53 Next Action

Marriage death and dissolution mechanics with legitimacy, oathkeeping, and dynastic consequences. The marriage system is now strategically filtered by covenant fit, but active unions still persist forever once formed. The next canonical layer is to let bloodline death dissolve the union, change legitimacy, and ripple back into dynastic stability.
