# 2026-04-17 Unity Enemy AI Construction Pass

## Goal

Extend the Session 120 enemy AI from "train more workers" to "train workers
AND expand the base." Before this slice the AI could gather and produce but
couldn't build new structures, so its growth was capped at the starting
population and starting housing. Enemy faction now places dwellings, farms,
and wells against the same canonical build rules and costs the player uses.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/AI/AIEconomyControllerComponent.cs`:
  - New `ConstructionAccumulator` / `ConstructionIntervalSeconds` (default 5s)
  - New `TargetDwellingCount` (default 2), `TargetFarmCount` (default 2),
    `TargetWellCount` (default 1)
  - New cached counts `ControlledDwellingCountCached`,
    `ControlledFarmCountCached`, `ControlledWellCountCached`
  - New telemetry `ConstructionPlacementsAttempted` and
    `ConstructionPlacementsSucceeded` for observability
- Extended `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`:
  `SpawnFactionEntity` now seeds the new construction-target fields on
  AI-eligible factions.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs`:
  - `TickAIFactions` now advances a third accumulator and fires a
    construction pass on its own interval, independent of gather + production.
  - `RefreshAIStatsCache` now walks the building query and counts per-type
    (dwelling, farm, well) so the construction planner has live data.
  - `AIRunConstructionPlan` picks a priority target building (dwelling first,
    then farm, then well), resolves the building definition and faction
    snapshot, confirms cost affordability, finds an idle (or fallback live)
    AI worker, computes a valid build position via `TryPickAIBuildPosition`,
    and reuses the existing `TryPlaceConstruction` path so placement cost,
    `ConstructionStateComponent`, and `ResourceTrickleBuildingComponent`
    wiring all match the player-facing construction flow.
  - `TryPickAIBuildPosition` samples radial rings around the faction's
    nearest owned `command_hall` until it finds a position
    `CanPlaceConstructionAt` accepts. Keeps enemy bases spatially coherent
    instead of scattering buildings randomly.
  - `TryFindIdleAIWorker` prefers workers with no `WorkerGatherComponent` or
    `Phase == Idle`; falls back to any live AI worker if none are idle.
  - New `CountFactionBuildings(factionId)` debug helper alongside the
    existing `CountFactionUnits` for governed validators.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` gains `aiConstructionBaselineSampled`,
    `aiInitialBuildingCount`, `aiLatestBuildingCount`,
    `aiConstructionBaselineUtcTicks`, `aiMinimumBuildingGain` (default 1),
    `aiConstructionTimeoutSeconds` (default 30), `aiConstructionObserved`.
  - The AI economy probe now samples the enemy building count at AI-enable
    time (shared baseline) so the subsequent construction probe sees true
    pre-AI-activity building count.
  - `ProbeAIConstructionActivity` runs after the economy probe. Passes when
    enemy building count rises by at least the configured minimum within
    the configured timeout; fails with explicit diagnostic otherwise.
  - AI economy probe relaxed from "gold AND unit gain" to "gold OR unit
    gain" since construction spending reliably drives net gold negative
    while the economy is clearly alive.
  - New `aiFlexibleWindow` composed flag widens the global strict
    building- and unit-count gates whenever the AI is between baseline and
    full observation, so AI spawns and builds do not trip the player-phase
    count invariants. Other invariants (factions, resource nodes, control
    points, settlements) still strictly enforced.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed
  via the wrapper lock. Success line now ends with:
  - `aiFaction='enemy'`
  - `aiInitialGold=220.00`
  - `aiLatestGold=155.00`
  - `aiInitialUnitCount=6`
  - `aiLatestUnitCount=7`
  - `aiGoldGainObserved=False`
  - `aiUnitGainObserved=True`
  - `aiActivityObserved=True`
  - `aiInitialBuildingCount=4`
  - `aiLatestBuildingCount=6`
  - `aiConstructionObserved=True`
  alongside preserved `capPressureObserved=True`,
  `loyaltyDeclineObserved=True`, `starvationObserved=True`,
  `trickleGainObserved=True`, `gatherDepositObserved=True`, and the
  production + construction progress fields.
  Enemy built two new buildings (4 -> 6) and trained one new villager
  while gold dropped from 220 to 155 (65 gold spent on construction +
  training), exactly matching canonical cost rules.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves:

- player selection, control groups, formation move
- player production queue with cancel/refund and progress observability
- player construction with progress bars
- player gold gather-deposit cycle
- passive resource trickle from farms and wells
- canonical realm-condition effect trio (famine, water crisis, cap pressure)
- enemy AI economic base (gather + train villagers + train militia)
- **enemy AI expansion (build dwellings + farms + wells against canonical
  rules and cost structure)**

## Next Action

Still on `claude/unity-enemy-ai-economic-base`:

1. Stability-surplus loyalty restoration: `food/pop >= 1.75 AND water/pop >= 1.75`
   restores `+1` loyalty per cycle capped at 95. Completes the canonical
   realm-condition effect set alongside the existing famine/water-crisis/
   cap-pressure consequences.
2. Loyalty + population density HUD readout in the battlefield shell.
3. Enemy AI combat posturing (militia moves toward threat anchors) once
   Codex's attack-move slice lands.

Concurrent work: Codex on `codex/unity-projectile-combat`.
