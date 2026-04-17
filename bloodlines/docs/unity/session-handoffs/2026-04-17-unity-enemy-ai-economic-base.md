# 2026-04-17 Unity Enemy AI Economic Base

## Goal

Give the enemy faction a real economic loop so the simulation moves from "sandbox
with progress bars" to "living opposed match." Before this slice, the enemy
faction existed in the bootstrap but did nothing: idle workers, no training,
stockpile static, population frozen. The first AI opponent now gathers gold,
trains villagers up to a worker target, and trains militia once the worker
target is met, against the same canonical economy the player uses.

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/AI/AIEconomyControllerComponent.cs`:
  - `Enabled`, `PrimaryGatherResourceId` (default "gold"),
    `TargetWorkerCount` (default 6), `TargetMilitiaCount` (default 4),
    two independent accumulators with second-scale interval fields
    (`GatherAssignmentIntervalSeconds`, `ProductionIntervalSeconds`, both
    default 2.5), and cached stats for diagnostic use
    (`ControlledWorkerCountCached`, `ControlledMilitiaCountCached`,
    `IdleWorkerCountCached`, `ProductionQueueCountCached`).
  - Scope is deliberately economic base. Construction, combat orders,
    expansion, and diplomacy come in later AI slices.
- Converted `BloodlinesDebugCommandSurface` to
  `public sealed partial class` and added a new partial-class file:
  - `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.AI.cs`
  - `TickAIFactions` runs each Update, queries all
    `AIEconomyControllerComponent` entities, refreshes per-faction stats
    cache, and advances two independent accumulators. When either hits
    its interval, the matching AI pass fires.
  - `AIAssignIdleWorkersToGather` counts idle workers for the AI faction
    and assigns them a `WorkerGatherComponent` targeted at the nearest
    resource node of `PrimaryGatherResourceId` (radius 1.25 gather,
    1.75 deposit, canonical `carryCapacity` and `gatherRate` from the
    shared unit definition cache).
  - `AIRunProductionPlan` computes `wantsVillager` vs `wantsMilitia`
    from the cached counts and target counts. Villager trains at the
    faction's `command_hall`, militia at `barracks`, both gated on cost
    affordability from the AI faction's own `ResourceStockpileComponent`
    and available population. Only one queue entry at a time per
    building so the AI does not over-queue during tight budgets.
  - Reuses existing `TryResolveUnitDefinition`,
    `TryGetFactionRuntimeSnapshot`, `CanAffordCost`, `SpendCost`,
    `ResolveUnitRole`, `ResolveSiegeClass`, and
    `GetProductionDurationSeconds` helpers on the debug surface, and
    uses faction-scoped variants of the building-lookup paths so the
    AI code does not collide with the player-facing command surface.
  - Adds `TryDebugEnableAIForFaction(factionId, enabled)` so governed
    validators (and the continuation platform later) can opt the AI
    loop in or out at will.
  - Adds `TryDebugGetAIEconomyStats(factionId, out ...)` for diagnostic
    reads of the cached AI stats.
  - Adds `CountFactionUnits(factionId)` helper for fast non-player unit
    counts used by the smoke.
- Extended `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`:
  `SpawnFactionEntity` now attaches an `AIEconomyControllerComponent` at
  `Enabled=false` for non-player non-neutral factions. The new
  `ShouldAttachAIEconomyController` helper gates the attach. Starting
  disabled keeps the AI from perturbing earlier smoke-proof phases; the
  smoke enables the controller explicitly when its AI probe fires.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` gains `aiFactionId` (default "enemy"),
    `aiBaselineSampled`, `aiInitialGold`, `aiInitialUnitCount`,
    `aiLatestGold`, `aiLatestUnitCount`, `aiBaselineUtcTicks`,
    `aiMinimumGoldDelta` (default 5), `aiMinimumUnitDelta` (default 1),
    `aiActivityTimeoutSeconds` (default 35), `aiActivityObserved`,
    `aiGoldGainObserved`, `aiUnitGainObserved`.
  - New `ProbeAIEconomyActivity` phase runs after the cap-pressure
    probe. First call snapshots enemy gold + unit count, enables the
    enemy AI via the new debug API, records utc ticks. Subsequent
    calls watch stockpile + unit count until both gold and unit gains
    reach their minimum thresholds or the timeout elapses.
  - A new `aiObservationWindow` bypass is applied to the strict
    global-unit-count gates alongside the pre-existing
    `midProductionObservationWindow` bypass so the smoke does not
    trip when the AI legitimately grows the enemy roster. Other count
    invariants (factions, buildings, resource nodes, control points,
    settlements) remain strictly enforced during the AI window.
  - Final success diagnostics line now carries `aiFaction`,
    `aiInitialGold`, `aiLatestGold`, `aiInitialUnitCount`,
    `aiLatestUnitCount`, `aiGoldGainObserved`, `aiUnitGainObserved`,
    and `aiActivityObserved` alongside preserved earlier fields.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with
  0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed
  with 0 errors.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1`
  passed via the wrapper lock. Success line now ends with:
  - `aiFaction='enemy'`
  - `aiInitialGold=220.00`
  - `aiLatestGold=230.00`
  - `aiInitialUnitCount=6`
  - `aiLatestUnitCount=8`
  - `aiGoldGainObserved=True`
  - `aiUnitGainObserved=True`
  - `aiActivityObserved=True`
  alongside preserved `capPressureObserved=True`,
  `loyaltyDeclineObserved=True`, `starvationObserved=True`,
  `trickleGainObserved=True`, `gatherDepositObserved=True`,
  and the existing construction + production progress proofs.
  The observed AI behavior matches canon: enemy faction
  gathered 10+ gold, spent cost on two new villagers, and grew
  its unit count from 6 to 8 inside a single observation window.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves a living opposed match shape:

- player selection, control groups, formation move
- player production queue with cancel/refund and progress observability
- player construction with progress bars
- player gold gather-deposit cycle
- passive resource trickle from farms and wells
- canonical realm-condition effect trio (famine, water crisis, cap pressure)
- **enemy AI gathering gold and training villagers/militia against the
  same canonical economy and cost rules the player uses**

## Next Action

Still on `claude/unity-enemy-ai-economic-base` (or follow-up branches):

1. Enemy AI construction pass: when the enemy faction is short on
   food, water, or housing, its workers place farms, wells, or
   dwellings per the same construction lane the player uses.
2. Enemy AI militia posturing: when militia count is above a threshold,
   move them toward a threat anchor (nearest hostile unit or nearest
   uncontrolled control point). This ties into Codex's attack lane.
3. Player-side stability-surplus loyalty restoration (economy polish).
4. Loyalty + density HUD readout on the battlefield shell.

Concurrent work: Codex is still on `codex/unity-projectile-combat` for
projectile delivery in ranged combat. Do not touch Codex's combat
files; merge both lanes into master once Codex reports complete.
