# 2026-04-17 Unity Stability-Surplus Loyalty Restoration

## Goal

Close the canonical realm-condition effect set by adding the fourth and final
per-cycle consequence from `data/realm-conditions.json`: stability surplus.
Before this slice, the ECS recognized famine, water crisis, and cap pressure
as loyalty-consuming events, but never restored loyalty when conditions were
GOOD. Now, when both food and water ratios cross the 1.75 threshold, the
faction recovers +1 loyalty per cycle up to 95, matching the canonical
`stabilitySurplus` effect block exactly.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Components/RealmConditionComponent.cs`:
  - `RealmConditionComponent` gains `LastStabilitySurplusResponseCycle` so
    the new responder applies exactly once per cycle and cannot race the
    starvation / cap-pressure responders.
  - `RealmCycleConfig` gains `StabilitySurplusFoodRatio` (1.75),
    `StabilitySurplusWaterRatio` (1.75),
    `StabilitySurplusLoyaltyDeltaPerCycle` (+1),
    `StabilitySurplusMaxLoyaltyToApply` (95). Canonical values from
    `data/realm-conditions.json` effect block.
- Extended `unity/Assets/_Bloodlines/Code/Systems/BloodlinesBootstrap.cs`:
  `RealmCycleConfig` seed now populates the four new stability-surplus fields.
- Added `unity/Assets/_Bloodlines/Code/Economy/StabilitySurplusResponseSystem.cs`:
  - `ISystem` in `SimulationSystemGroup` with
    `[UpdateAfter(typeof(RealmConditionCycleSystem))]` so it sees the
    current cycle's streak updates.
  - Once per new cycle, if `Food/Total >= StabilitySurplusFoodRatio` AND
    `Water/Total >= StabilitySurplusWaterRatio` AND current loyalty is
    below `StabilitySurplusMaxLoyaltyToApply`, applies
    `StabilitySurplusLoyaltyDeltaPerCycle` to `FactionLoyaltyComponent.Current`
    clamped to `[Floor, min(Max, MaxLoyaltyToApply)]`.
  - Runs independently from `StarvationResponseSystem` and
    `CapPressureResponseSystem` via its own `LastStabilitySurplusResponseCycle`
    marker.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  new `TryDebugForceStabilitySurplusCycle(factionId, out loyaltyBeforeCycle, out foodRatioAfter, out waterRatioAfter)`
  debug API that raises the faction's food and water stockpiles above the
  canonical threshold, zeros the strain streaks, and advances the cycle.
  Also marks `LastStarvationResponseCycle` and `LastCapPressureResponseCycle`
  beyond the new cycle so those responders do not re-apply during the same
  test window; only stability-surplus fires for the proof.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` gains `stabilitySurplusForced`,
    `stabilitySurplusLoyaltyBefore`, `stabilitySurplusLoyaltyLatest`,
    `stabilitySurplusLoyaltyExpectedMinimumAfter`,
    `stabilitySurplusFoodRatioAfter`, `stabilitySurplusWaterRatioAfter`,
    `stabilitySurplusForcedUtcTicks`, `stabilitySurplusTimeoutSeconds`
    (default 10), `stabilitySurplusObserved`.
  - New `ProbeStabilitySurplusResponse` phase runs after the AI
    construction probe. First call samples current loyalty, forces the
    stability-surplus cycle via the new debug API. Subsequent calls
    sample current loyalty; passes when loyalty rose above the
    configured minimum, fails after the timeout.
  - `aiFlexibleWindow` simplified to stay active for the entire
    post-baseline window (AI remains live and may continue producing
    buildings/units at any point; the strict global count gates are
    permanently widened once AI is enabled).
  - Final success diagnostics line carries `stabilitySurplusForced`,
    `stabilitySurplusLoyaltyBefore`, `stabilitySurplusLoyaltyLatest`,
    and `stabilitySurplusObserved`.
- Registered `StabilitySurplusResponseSystem.cs` in
  `unity/Assembly-CSharp.csproj`.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  passed via the wrapper lock. Success line now ends with:
  - `stabilitySurplusForced=True`
  - `stabilitySurplusLoyaltyBefore=59.00`
  - `stabilitySurplusLoyaltyLatest=60.00`
  - `stabilitySurplusObserved=True`
  alongside preserved `aiConstructionObserved=True`,
  `aiActivityObserved=True`, `capPressureObserved=True`,
  `loyaltyDeclineObserved=True`, `starvationObserved=True`,
  `trickleGainObserved=True`, `gatherDepositObserved=True`, and the
  construction + production progress fields.
  Loyalty rose by exactly +1, matching canonical
  `stabilitySurplus.loyaltyDeltaPerCycle`.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves the COMPLETE canonical
realm-condition effect set from `data/realm-conditions.json`:

- famine: `-1` population + `-4` loyalty when `FoodStrainStreak >= 2`
- water crisis: `-1` population + `-6` loyalty when `WaterStrainStreak >= 1`
- cap pressure: `-1` loyalty when `Total / Cap >= 0.95`
- **stability surplus: `+1` loyalty (capped at 95) when food/pop >= 1.75
  AND water/pop >= 1.75**

Only effect fields still to layer in as their dependent systems come
online: unit morale multiplier during famine, territory agriculture
multiplier during water crisis. Both are multipliers on systems that
do not yet exist in the Unity lane.

## Next Action

Still on `claude/unity-enemy-ai-economic-base`:

1. Loyalty + population density HUD readout in the battlefield shell.
2. Enemy AI militia posturing (moves toward threat anchors) once
   Codex's attack-move slice lands.

Concurrent work: Codex on `codex/unity-projectile-combat`.
