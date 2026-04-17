# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-15
Session: 57
Author: Codex

## Scope

Marriage governance by head of household is now LIVE as the next dynastic-authority layer. Marriage proposals and approvals no longer operate as flat member-to-member toggles. They now run through live household authority, require an offering envoy, create legitimacy strain under regency, surface through the dynasty panel, and persist through restore.

## Changes Landed

### Marriage-governance runtime (`src/game/core/simulation.js`)

- Added shared marriage-governance helpers:
  - `getMarriageGovernanceStatus`
  - `getMarriageProposalTerms`
  - `getMarriageAcceptanceTerms`
- Marriage authority now resolves through a live dynastic chain:
  - direct `head_of_bloodline` approval when the head is available,
  - `heir_designate` regency when the head is unavailable,
  - `diplomat` regency as the final governed fallback.
- Offering a marriage now also requires a live diplomatic envoy from the offering dynasty.
- Proposal records now persist governance metadata:
  - offering household authority,
  - offering envoy,
  - target-household approval preview.
- Accepted marriage records now persist the authority that approved the union.
- Regency is no longer decorative. Proposals or approvals made without the head in place now apply measurable legitimacy strain and stewardship consequence.

### Dynastic legibility (`src/game/main.js`)

- The dynasty panel now surfaces live marriage governance status for the player court:
  - current household authority,
  - current offering envoy.
- Incoming marriage proposals now expose:
  - offering household sanction,
  - offering envoy,
  - the player court authority that would approve the union.
- Outgoing marriage offers now expose:
  - who sanctioned the offer,
  - who carries it diplomatically,
  - whether the target court is currently unable to approve because its household authority is disrupted.
- Active marriages now retain authority provenance in the dynasty panel instead of dropping it after acceptance.
- The player-side proposal action now reads the live governance terms before dispatch, including blocked envoy cases and regency strain.

### Existing AI lane now obeys governance (`src/game/core/ai.js`)

- No new AI branch was required.
- Stonehelm already uses the shared `proposeMarriage` and `acceptMarriage` runtime path, so the new governance and envoy gates now constrain AI marriage behavior automatically.
- This means capture, assassination, succession disruption, and envoy loss now materially alter AI marriage capacity without requiring a separate fake AI-only rule.

### Test coverage (`tests/runtime-bridge.mjs`)

- Added Session 57 runtime coverage proving:
  - fresh courts expose direct head approval and diplomatic envoy state,
  - faction snapshot now surfaces marriage governance for dynastic legibility,
  - direct head-approved proposals persist sanction and envoy metadata without legitimacy penalty,
  - captured heads force heir regency on new proposals,
  - heir-regency proposals apply real legitimacy loss,
  - missing diplomatic envoy blocks marriage offers,
  - incoming proposals can be approved under heir regency when the head is unavailable,
  - restore preserves marriage-governance authority history on accepted marriages.

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

Session 57 connects:

1. Dynasty authority and marriage, unions now require household sanction instead of bypassing the ruling structure.
2. Diplomacy and bloodline roles, an offering envoy is now required to carry a live marriage agreement.
3. Legitimacy and succession pressure, regency-based marriage governance now costs legitimacy and stewardship.
4. Capture and bloodline disruption, loss of the head or envoy now changes marriage capacity immediately.
5. Legibility and continuity, governance state now surfaces in the dynasty panel and persists through restore.

## Gap Analysis Reclassification

| System | Previous | Current |
|---|---|---|
| Marriage control by head of household | DOCUMENTED | LIVE (first layer, with household authority chain, envoy requirement, regency legitimacy strain, dynasty-panel legibility, and restore continuity) |

## Session 58 Next Action

Field-water attrition and desertion. Water sustainment is already live as a movement and combat pressure layer. The next highest-leverage military-civilizational seam is to let prolonged dehydration start costing armies bodies, cohesion, and field presence instead of remaining only a performance debuff.
