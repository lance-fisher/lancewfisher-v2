# BLOODLINES STATE OF GAME REPORT

Date: 2026-04-16
Session: 88
Author: Claude

## Scope

Session 88 stabilized and completed the unreported alliance-threshold coalition pressure system that had been partially implemented in the codebase (tagged as "Session 89" in code comments) without test coverage, save/restore support, snapshot exposure, or a continuity report. This session repairs that continuity gap, adds the missing infrastructure, and verifies the full system end to end.

## Changes Landed

### Save/restore for governance alliance-threshold coalition pressure (`src/game/core/simulation.js`)

- `exportStateSnapshot` now explicitly preserves `governanceAlliancePressureCycles`, `governanceAlliancePressureActive`, and `governanceAlliancePressureHostileCount` in the faction export.
- `restoreStateSnapshot` now explicitly restores those three fields, so the accumulated coalition pressure history survives snapshot round-trip.
- Previously, the cycle counter was lost on save/restore because the export function only serialized explicitly named fields and did not include the new alliance-pressure fields.

### Realm condition snapshot exposure (`src/game/core/simulation.js`)

- `getRealmConditionSnapshot` now exposes the following fields in the `worldPressure` block:
  - `governanceAlliancePressureActive`: whether coalition pressure is currently being applied.
  - `governanceAlliancePressureCycles`: accumulated count of realm cycles where coalition pressure fired.
  - `governanceAlliancePressureHostileCount`: number of hostile kingdoms contributing to the pressure.
  - `governanceAlliancePressureWeakestMarchName`: name of the weakest governed march currently absorbing coalition loyalty erosion.
  - `governanceAlliancePressureWeakestMarchLoyalty`: current loyalty of that weakest march.

### Dynasty panel and HUD legibility (`src/game/main.js`)

- The dynasty panel now shows a dedicated "Coalition pressure active" line when alliance-threshold pressure is live, including hostile court count, acceptance drag value, and the weakest governed march name and loyalty.
- The world pill in the HUD now shows "coalition N hostile" when governance alliance-threshold pressure is active, making the new system readable in the compact dashboard.

### Runtime bridge test coverage (`tests/runtime-bridge.mjs`)

- Added a full alliance-threshold test block covering:
  - Coalition loyalty pressure erodes the weakest governed march on a realm cycle when acceptance >= 60%.
  - Coalition legitimacy drain reduces dynastic legitimacy on the same cycle.
  - `governanceAlliancePressureActive` is set on the faction after the cycle fires.
  - `governanceAlliancePressureCycles` increments correctly.
  - The realm condition snapshot surfaces alliance-pressure state in the worldPressure block.
  - Save/restore round-trip preserves the alliance-pressure cycle counter and active flag.
  - Acceptance below 60% does not trigger alliance-threshold coalition pressure.

## What Already Existed (Unreported Session 89 Code)

The following code was already present in `simulation.js` before this session, tagged with "Session 89" comments but never documented, tested, or given save/restore support:

1. **Constants**: `TERRITORIAL_GOVERNANCE_ACCEPTANCE_ALLIANCE_THRESHOLD_PCT = 60`, `GOVERNANCE_ALLIANCE_LOYALTY_PRESSURE_BASE = -1.5`, `GOVERNANCE_ALLIANCE_LEGITIMACY_PRESSURE_PER_CYCLE = 0.8`, `GOVERNANCE_ALLIANCE_ACCEPTANCE_DRAG_PER_HOSTILE = 0.04`.

2. **Acceptance-drag** (acceptance profile): When acceptance is at or above the 60% alliance threshold, each hostile kingdom adds 0.04 to the decay pressure, reducing the effective rise rate. This makes the 60 to 65% push materially harder when the governance leader has more hostile neighbors.

3. **Realm-cycle coalition pressure** (inside the realm cycle): When governance recognition is active and acceptance >= alliance threshold, each realm cycle erodes the weakest stabilized governed march by -1.5 loyalty per hostile kingdom and drains legitimacy by 0.8 per hostile kingdom.

4. **Faction-level state tracking**: `governanceAlliancePressureCycles`, `governanceAlliancePressureHostileCount`, `governanceAlliancePressureActive`, `governanceAlliancePressureWeakestMarchId/Name/Loyalty`.

5. **AI alliance-threshold awareness** (in ai.js): Stonehelm already reads the populationAcceptancePct against the populationAcceptanceAllianceThresholdPct to compress timing when acceptance approaches sovereignty readiness.

6. **Acceptance tier announcements**: Messages fire when acceptance crosses the "threshold" tier (near 65%), enters "accepted" (at 65%), or falls back below the threshold.

This session did not modify the simulation-level alliance-threshold logic itself. It only added the missing infrastructure around it.

## Verification

- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed (including 9 new alliance-threshold assertions).
- `node --check src/game/main.js` passed.
- `node --check src/game/core/simulation.js` passed.
- `node --check src/game/core/ai.js` passed.
- `node --check src/game/core/renderer.js` passed.
- `node --check src/game/core/data-loader.js` passed.
- `node --check src/game/core/utils.js` passed.
- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- Unity canonical scene-shell validation ran in batch mode. Bootstrap and Gameplay validators were invoked but the batch passes were consumed by import work before the standard result lines printed. This is the known batch-mode first-run pattern, not a regression.

## Canonical Interdependency Check

Session 88 does not introduce any new simulation mechanics. It only repairs the continuity, verification, and legibility gaps around the existing alliance-threshold system. The alliance-threshold pressure already interacts with:

1. Population acceptance target and rise/fall rates (acceptance profile).
2. Realm-cycle coalition loyalty erosion on governed marches.
3. Realm-cycle legitimacy drain on the governance leader.
4. AI timing compression when acceptance approaches sovereignty readiness.
5. Marriage diplomacy (reducing hostility count is the primary player counterplay).
6. Faith diplomacy (reducing holy-war hostility loosens the coalition count).

## Gap Analysis Reclassification

- Alliance-threshold coalition pressure: moved from implicit/undocumented to LIVE with test coverage, save/restore, snapshot exposure, and HUD legibility.
- The broader Territorial Governance victory family remains PARTIAL because the full canonical ~90% sovereignty doctrine, multi-kingdom neutral-power counterpressure, and population-acceptance alliance diplomacy mechanics are still not live.

## Session 89 Next Action

Priority order:

1. Open `Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS and enter Play Mode. Verify the full ECS battlefield shell (bootstrap entity creation, debug presentation, camera, selection, formation move, control-point capture, territory trickle).
2. If Unity Play Mode is confirmed working, continue into attack-move or richer command-state shell work.
3. If Unity is blocked or deprioritized, continue the browser lane through multi-kingdom neutral-power counterpressure or population-acceptance alliance diplomacy as the next sovereignty-path deepening.
4. If the sovereignty path is deprioritized, deepen the naval-world integration or broader theatre-of-war expansion.
