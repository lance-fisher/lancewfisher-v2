# 2026-04-17 Unity Faction Loyalty + Famine and Water Crisis Loyalty Deltas

## Goal

Introduce the canonical per-faction civilizational loyalty to the ECS and wire
the `famine.loyaltyDeltaPerCycle = -4` and
`waterCrisis.loyaltyDeltaPerCycle = -6` effects from
`data/realm-conditions.json` into the Session 116 starvation response. Before
this slice, only population declined when food or water strain crossed its
threshold; loyalty (the canonical civilizational cohesion variable) did not
exist as a persistent ECS component, so every downstream system that depends
on it (defection risk, succession-crisis pressure, breakaway march gating,
recruitment hesitation) had nothing to read.

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/Economy/FactionLoyaltyComponent.cs`:
  - `Current`, `Max`, `Floor` stored as float. Canonical 0..100 range.
  - Seed default: `Current = 70`, `Max = 100`, `Floor = 0`.
  - Documentation records the canonical consumer list (defection risk,
    succession-crisis pressure, breakaway-march gating, recruitment
    hesitation) that will layer onto this component as those systems come
    online.
- Extended `unity/Assets/_Bloodlines/Code/Systems/SkirmishBootstrapSystem.cs`:
  `SpawnFactionEntity` now attaches `FactionLoyaltyComponent` with the
  canonical defaults on each faction spawn.
- Extended `unity/Assets/_Bloodlines/Code/Components/RealmConditionComponent.cs`:
  `RealmCycleConfig` gains `FamineLoyaltyDeltaPerCycle` and
  `WaterCrisisLoyaltyDeltaPerCycle`, matching the canonical effect values from
  `data/realm-conditions.json`.
- Extended `unity/Assets/_Bloodlines/Code/Systems/BloodlinesBootstrap.cs`:
  `RealmCycleConfig` seed now also sets `FamineLoyaltyDeltaPerCycle = -4` and
  `WaterCrisisLoyaltyDeltaPerCycle = -6`.
- Extended `unity/Assets/_Bloodlines/Code/Economy/StarvationResponseSystem.cs`:
  - Query now includes `RefRW<FactionLoyaltyComponent>` alongside the
    existing `RefRW<RealmConditionComponent>` and `RefRW<PopulationComponent>`.
  - Per active cycle, the system sums the famine and water-crisis loyalty
    deltas into `totalLoyaltyDelta` and applies to
    `FactionLoyaltyComponent.Current`, clamped to `[Floor, Max]`.
  - Population decline path preserved as before.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  new `TryDebugGetFactionLoyalty(factionId, out current, out max, out floor)`
  debug API so governed validators can sample loyalty without duplicating
  entity-query logic.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  `RuntimeSmokeState` gains `loyaltyPreviousValue`,
  `loyaltyExpectedMaximumAfterCycle`, `loyaltyLatestValue`,
  `loyaltyMinimumDeltaMagnitude` (default 8), `loyaltyDeclineObserved`.
  `ProbeStarvationResponse` now:
  - samples loyalty before calling `TryDebugForceStarvationCycle` and stores
    `loyaltyPreviousValue` + `loyaltyExpectedMaximumAfterCycle`
  - on the same probe pass that observes population decline, samples the
    current loyalty and fails if loyalty is above the configured
    post-cycle maximum; otherwise marks both `starvationObserved` and
    `loyaltyDeclineObserved` green simultaneously.
  Final success diagnostics line carries `loyaltyPreviousValue`,
  `loyaltyLatestValue`, `loyaltyExpectedMaximumAfterCycle`, and
  `loyaltyDeclineObserved`.
- Registered `FactionLoyaltyComponent.cs` in `unity/Assembly-CSharp.csproj`.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors
  and the pre-existing 110 editor-importer warnings.
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed
  via the wrapper lock. Success line now ends with:
  - `loyaltyPreviousValue=70.00`
  - `loyaltyLatestValue=60.00`
  - `loyaltyExpectedMaximumAfterCycle=62.00`
  - `loyaltyDeclineObserved=True`
  alongside the preserved `starvationObserved=True`, `trickleGainObserved=True`,
  `gatherDepositObserved=True`, and earlier progress fields. Loyalty fell by
  exactly 10 (`famine -4` plus `waterCrisis -6`), matching canon.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now carries:

- selection, drag-box, control groups, formation move
- production queue with cancel/refund and mid-production progress
- worker-led construction with progress bars
- constructed `barracks -> militia` continuity
- gold gather-deposit cycle
- farm/well resource trickle
- famine + water-crisis population response
- **famine + water-crisis loyalty decline**

## Next Action

Still on `claude/unity-food-water-economy`:

1. Cap-pressure loyalty tick: when `Total / Cap >= PopulationCapPressureRatio`
   (0.95), apply `capPressure.loyaltyDeltaPerCycle = -1` per cycle. Needs
   `PopulationCapPressureRatio` + `CapPressureLoyaltyDeltaPerCycle` added to
   `RealmCycleConfig` and a new `CapPressureResponseSystem` that reads
   population vs cap at cycle close.
2. Stability-surplus loyalty restoration: when `food / pop >= 1.75` AND
   `water / pop >= 1.75`, restore `+1` loyalty per cycle (cap at 95).
3. Loyalty readout on the battlefield HUD so the player can see loyalty
   motion in Play Mode without the debug console.

Codex continues in parallel on `codex/unity-combat-foundation`.
