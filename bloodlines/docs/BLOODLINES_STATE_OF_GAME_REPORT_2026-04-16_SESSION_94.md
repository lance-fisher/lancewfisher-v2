# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-16
Session: 94
Author: Claude

## Scope

Session 94 implemented the canonical Trueborn Rise arc, one of Bloodlines' core anti-snowball mechanics. The Trueborn City now activates as a major NPC-level authority if no dynasty has meaningfully challenged it by a defined match threshold, escalating through three stages of increasing world pressure. This creates a ticking clock that prevents passive turtling and forces engagement with the world's central authority.

## Changes Landed

### Trueborn Rise arc activation and three-stage escalation (`src/game/core/simulation.js`)

- **Constants**:
  - `TRUEBORN_RISE_UNCHALLENGED_THRESHOLD_DAYS = 365 * 8` (8 in-world years before activation)
  - `TRUEBORN_RISE_STAGE_2_DELAY_DAYS = 365 * 2` (2 years from Stage 1 to Stage 2)
  - `TRUEBORN_RISE_STAGE_3_DELAY_DAYS = 365 * 3` (3 years from Stage 2 to Stage 3)
  - Stage 1 loyalty pressure: -0.8 per realm cycle on all kingdom territories
  - Stage 2 loyalty pressure: -1.8 plus -0.6 legitimacy per realm cycle
  - Stage 3 loyalty pressure: -3.2 plus -1.4 legitimacy per realm cycle
  - Challenge threshold: 5 (challenge level from kingdoms that delays activation)

- **Challenge level computation** (`getTruebornChallengeLevel`): Measures how much the world is engaging with the Trueborn City. Each kingdom contributes challenge through:
  - Negative standing (standing <= -5): +2 challenge
  - Territorial expansion (3+ territories): +1 challenge
  - Direct hostility to the city: +3 challenge

- **Three-stage escalation** (`tickTruebornRiseArc`):
  - **Stage 0** (dormant): Challenge level tracked. Unchallenged cycles accumulate. Activation requires both the 8-year threshold AND 3+ unchallenged realm cycles.
  - **Stage 1** (Claims): Passive loyalty pressure on all kingdom-owned territories. Canon: "The city establishing formal governance claims over territories within its immediate sphere."
  - **Stage 2** (Recognition): Stronger loyalty pressure plus legitimacy drain. Canon: "Missions of recognition travel to every founding house. Houses that refuse face accelerating loyalty erosion."
  - **Stage 3** (Restoration): Maximum loyalty pressure plus heavy legitimacy drain. Canon: "The Trueborn City fields military forces and pursues territorial reconquest."

- **Realm cycle integration**: `tickTruebornRiseArc` runs each realm cycle after the trade-relationship tick and before match-progression state update.

### HUD and snapshot exposure (`src/game/core/simulation.js`, `src/game/main.js`)

- `getRealmConditionSnapshot` now exposes: `truebornRiseStage`, `truebornRiseChallengeLevel`, `truebornRiseUnchallengedCycles`, `truebornRiseActivatedAtInWorldDays`.
- The world pill in the HUD now shows "Trueborn Rise Claims/Recognition/Restoration" when the arc is active.

### Save/restore support

- `exportStateSnapshot` now includes `riseArc: shallowCopyMap(f.riseArc ?? null)`.
- `restoreStateSnapshot` now deep-copies `snapF.riseArc` back onto the restored faction.

### Runtime bridge test coverage (`tests/runtime-bridge.mjs`)

- 10 new assertions covering:
  - Trueborn City faction exists with initial stage 0.
  - Stage 1 activates after unchallenged threshold (8 years + low challenge).
  - Stage 1 applies loyalty pressure to kingdom-owned territories.
  - Snapshot exposes Rise stage.
  - Save/restore preserves Rise stage.
  - Stage 2 escalates after 2-year delay.
  - Stage 2 applies legitimacy pressure.
  - Stage 3 escalates after 3-year delay.
  - Message log records Rise activation and escalation.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed (including 10 new Rise arc assertions).
- All syntax checks pass.
- Browser preview: zero console errors, zero failed requests.

## Canonical Interdependency Check

The Trueborn Rise arc connects to:

1. **Trade relationships** (Session 93): Kingdoms with high trade standing can challenge the city passively. Negative standing increases challenge level.
2. **Conviction/acceptance** (Sessions 89, 93): Rise pressure erodes loyalty and legitimacy, which both feed into the acceptance profile and make sovereignty harder.
3. **World pressure**: Rise pressure acts as a universal kingdom penalty, making the late game harder if the Trueborn City is ignored.
4. **Non-aggression pacts** (Session 90): Pacts don't affect Trueborn hostility since the city isn't a kingdom, so diplomatic evasion of Rise pressure requires engaging the city directly.
5. **Match progression**: Rise activation is time-gated at 8 in-world years, which falls in Stage 3-4 territory for most matches.

## Gap Analysis

- Trueborn Rise arc: NEW, moved to LIVE with three-stage escalation, loyalty/legitimacy pressure, challenge tracking, save/restore, snapshot exposure, HUD legibility, and test coverage.
- The canonical Rise arc includes deeper mechanics not yet live: military unit spawning for Stage 3, formal recognition diplomacy actions, Trueborn Summons mechanic. These are future-session targets.

## Session 95 Next Action

Priority order:

1. If continuing the world-depth lane: implement Trueborn recognition diplomacy so kingdoms can formally recognize the Trueborn claim to gain standing bonuses and reduce Rise pressure.
2. If pivoting: continue into naval-world integration, broader theatre-of-war expansion, or trade-network intelligence.
3. If those are deprioritized: open the Unity Play Mode verification shell.
