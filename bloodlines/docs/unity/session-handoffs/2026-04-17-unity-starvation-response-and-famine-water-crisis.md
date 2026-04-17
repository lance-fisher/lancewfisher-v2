# 2026-04-17 Unity Starvation Response + Famine and Water Crisis

## Goal

Give the canonical realm-cycle strain tracking real consequences. Before this
slice, `RealmConditionCycleSystem` correctly incremented `FoodStrainStreak`
and `WaterStrainStreak` every 90 seconds when the faction's food or water
ratio fell below the yellow threshold, but nothing in the ECS consumed those
streaks. Populations did not shrink. Famines and water crises were invisible
to the live simulation. This slice closes that end-to-end gap against the
canonical `data/realm-conditions.json` effects.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Components/RealmConditionComponent.cs`:
  - `RealmConditionComponent` gains `LastStarvationResponseCycle` so the
    starvation responder applies consequences exactly once per cycle.
  - `RealmCycleConfig` gains `FoodFamineConsecutiveCycles`,
    `WaterCrisisConsecutiveCycles`, `FaminePopulationDeclinePerCycle`, and
    `WaterCrisisOutmigrationPerCycle`, matching the thresholds and effects
    block from `data/realm-conditions.json`.
- Extended `unity/Assets/_Bloodlines/Code/Systems/BloodlinesBootstrap.cs`:
  `RealmCycleConfig` seed now populates the four new effect fields with the
  canonical defaults: famine 2 consecutive cycles, water crisis 1 consecutive
  cycle, famine population decline 1, water crisis outmigration 1.
- Added `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs`:
  - `ISystem` running in `SimulationSystemGroup` with
    `[UpdateAfter(typeof(RealmConditionCycleSystem))]` so it sees the current
    cycle's streak updates.
  - For each faction, if `CycleCount > LastStarvationResponseCycle` the
    system records the new cycle as responded-to and then sums any
    famine population decline (when `FoodStrainStreak >= FoodFamineConsecutiveCycles`)
    plus water-crisis outmigration (when
    `WaterStrainStreak >= WaterCrisisConsecutiveCycles`). Population never
    drops below zero and `PopulationComponent.Available` is clamped alongside
    `PopulationComponent.Total`.
  - Loyalty deltas, morale multipliers, and agriculture multipliers remain
    out of scope for this first starvation-response slice. They come online
    once the loyalty / morale / territory yield systems land.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  - New `TryDebugForceStarvationCycle(factionId, includeWaterCrisis, out previousTotal, out expectedTotal)`
    debug API that spikes `FoodStrainStreak` to the famine threshold (and
    optionally `WaterStrainStreak` to the water-crisis threshold), zeros
    the faction's food (and optionally water) stockpile, advances
    `CycleCount`, and reports the expected post-starvation population so
    governed validators can assert without duplicating query logic.
  - New `TryDebugGetFactionPopulation(factionId, out total, out available, out cap)`
    so the smoke can observe the live faction population without
    duplicating snapshot logic.
  - New internal `TryGetRealmCycleConfig` helper shared across the debug API.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` gains `starvationForced`,
    `starvationIncludeWaterCrisis` (default `true`),
    `starvationPreviousPopulationTotal`, `starvationExpectedPopulationTotal`,
    `starvationLatestPopulationTotal`, `starvationForcedUtcTicks`,
    `starvationTimeoutSeconds` (default 15), `starvationObserved`.
  - New `ProbeStarvationResponse` phase runs after the trickle-gain probe.
    First call forces a starvation cycle via the new debug API, records
    previous and expected population, and returns `NotReady`. Subsequent
    calls sample the current population and either mark the slice green
    (when `currentTotal <= expectedTotal`) or fail after the timeout.
  - Final success diagnostics line now carries `starvationForced`,
    `starvationIncludeWaterCrisis`, `starvationPreviousPopulation`,
    `starvationExpectedPopulation`, `starvationLatestPopulation`, and
    `starvationObserved`.
- Registered `StarvationResponseSystem.cs` in `unity/Assembly-CSharp.csproj`.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors
  and the pre-existing 110 editor-importer warnings.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed
  via the concurrent-session wrapper lock. Success line now ends with:
  - `starvationForced=True`
  - `starvationIncludeWaterCrisis=True`
  - `starvationPreviousPopulation=14`
  - `starvationExpectedPopulation=12`
  - `starvationLatestPopulation=12`
  - `starvationObserved=True`
  alongside preserved `trickleGainObserved=True`, `gatherDepositObserved=True`,
  `productionProgressAdvancementVerified=True`,
  `constructionProgressAdvancementVerified=True`, and the prior
  `buildings=11`, `units=18`, `controlledUnits=8`, `populationCap=24` totals.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves the canonical primary economy
loop together with its famine/water-crisis consequence:

- selection, drag-box, control groups, formation move
- two-deep production queue with cancel-and-refund and mid-production progress
- worker-led dwelling + barracks construction with progress bars
- constructed `barracks -> militia` continuity
- gold gather-deposit cycle
- farm/well resource trickle
- **famine + water-crisis population response: when the canonical food or
  water strain streak reaches its threshold, population declines by the
  canonical `populationDeclinePerCycle` and `outmigrationPerCycle` values**

## Next Action

Within the economy lane (still on `claude/unity-food-water-economy`):

1. Wire loyalty deltas and unit morale multipliers from the canonical
   famine and water-crisis effects blocks into the live loyalty /
   morale systems once those land.
2. Add a world-space starvation indicator on the faction HUD when the
   strain streak is above zero so players see pressure building.
3. Begin the canonical population-cap pressure effects slice: when
   `Total / Cap >= PopulationCapPressureRatio`, apply
   `capPressure.loyaltyDeltaPerCycle` each cycle.

Codex continues in parallel on `codex/unity-combat-foundation`.
