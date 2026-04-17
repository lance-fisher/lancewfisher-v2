# 2026-04-17 Unity Cap-Pressure Response

## Goal

Close the third canonical realm-condition effect from
`data/realm-conditions.json`: cap pressure. Before this slice, the ECS
recognized population density (`Total` vs `Cap`) but no system read the
density ratio against the canonical `populationCapPressureRatio = 0.95`
threshold, so `capPressure.loyaltyDeltaPerCycle = -1` never fired.
Civilizational-density consequences were invisible. This slice completes
the realm-condition effect trio (famine, water crisis, cap pressure)
started in Sessions 116 and 117.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Components/RealmConditionComponent.cs`:
  - `RealmConditionComponent` gains `LastCapPressureResponseCycle` so the
    cap-pressure responder applies exactly once per cycle and does not
    race the existing `StarvationResponseSystem` which uses its own
    marker.
  - `RealmCycleConfig` gains `PopulationCapPressureRatio` (float, default
    0.95) and `CapPressureLoyaltyDeltaPerCycle` (int, default -1).
- Extended `unity/Assets/_Bloodlines/Code/Systems/BloodlinesBootstrap.cs`:
  `RealmCycleConfig` seed now populates the two new cap-pressure fields
  with the canonical defaults.
- Added `unity/Assets/_Bloodlines/Code/Economy/CapPressureResponseSystem.cs`:
  - `ISystem` running in `SimulationSystemGroup` with
    `[UpdateAfter(typeof(RealmConditionCycleSystem))]`.
  - For each faction with `RealmConditionComponent`, `PopulationComponent`,
    and `FactionLoyaltyComponent`: if `CycleCount > LastCapPressureResponseCycle`
    and `Total / Cap >= PopulationCapPressureRatio`, applies
    `CapPressureLoyaltyDeltaPerCycle` to
    `FactionLoyaltyComponent.Current`, clamped to `[Floor, Max]`.
  - Runs independently from `StarvationResponseSystem` via its own cycle
    marker so the two responders cannot race or double-apply.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  new `TryDebugForceCapPressureCycle(factionId, out totalAfterSpike, out capAfterSpike, out loyaltyBeforeCycle)`
  debug API that:
  - computes the minimum population needed to cross the canonical
    threshold and raises `PopulationComponent.Total` to that floor
  - clears `FoodStrainStreak` + `WaterStrainStreak` and sets
    `LastStarvationResponseCycle` to the incremented cycle so the
    cap-pressure proof is surgical (only cap-pressure applies in that
    cycle; famine + water-crisis streaks do not double-fire)
  - advances `CycleCount` so `CapPressureResponseSystem` picks the new
    cycle up on its next tick
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` gains `capPressureForced`, `capPressureTotalAfterSpike`,
    `capPressureCapAfterSpike`, `capPressureLoyaltyBeforeCycle`,
    `capPressureExpectedMaximumAfterCycle`, `capPressureLatestLoyalty`,
    `capPressureForcedUtcTicks`, `capPressureTimeoutSeconds` (default 10),
    `capPressureObserved`.
  - New `ProbeCapPressureResponse` phase runs after the existing
    starvation probe. First call forces the cap-pressure cycle,
    snapshots loyalty, returns `NotReady`. Subsequent calls sample
    current loyalty and fail if loyalty did not decline within the
    configured timeout.
  - Final success diagnostics line carries `capPressureForced`,
    `capPressureTotalAfterSpike`, `capPressureCapAfterSpike`,
    `capPressureLoyaltyBeforeCycle`, `capPressureLatestLoyalty`,
    `capPressureObserved`.
- Registered `CapPressureResponseSystem.cs` in
  `unity/Assembly-CSharp.csproj`.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with
  0 errors and the pre-existing 110 editor-importer warnings.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  passed via the wrapper lock. Success line now ends with:
  - `capPressureForced=True`
  - `capPressureTotalAfterSpike=23`
  - `capPressureCapAfterSpike=24`
  - `capPressureLoyaltyBeforeCycle=60.00`
  - `capPressureLatestLoyalty=59.00`
  - `capPressureObserved=True`
  The observed delta is exactly `-1`, matching canonical
  `capPressure.loyaltyDeltaPerCycle`. Preserved fields from earlier
  sessions remain green: `loyaltyDeclineObserved`,
  `starvationObserved`, `trickleGainObserved`, `gatherDepositObserved`,
  `productionProgressAdvancementVerified`,
  `constructionProgressAdvancementVerified`.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now carries the complete canonical
realm-condition effect trio from `data/realm-conditions.json`:

- famine: `-1` population + `-4` loyalty when `FoodStrainStreak >= 2`
- water crisis: `-1` population + `-6` loyalty when `WaterStrainStreak >= 1`
- cap pressure: `-1` loyalty when `Total / Cap >= 0.95`

Other canonical per-cycle effects still to layer in as their dependent
systems come online: stability-surplus loyalty restoration,
agriculture multipliers, unit morale multipliers.

## Next Action

Still on `claude/unity-food-water-economy`:

1. Stability-surplus loyalty restoration (`food / pop >= 1.75` AND
   `water / pop >= 1.75` restores `+1` loyalty per cycle up to 95).
2. Loyalty + population density HUD readout in the battlefield shell so
   motion is visible in Play Mode without the debug console.

Coordination with Codex's combat lane (`codex/unity-combat-foundation`
@ `8710141`): both branches now in merge-coordination state.
