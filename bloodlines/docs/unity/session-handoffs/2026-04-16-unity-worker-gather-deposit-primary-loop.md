# 2026-04-16 Unity Worker Gather-Deposit Primary Economy Loop

## Goal

Turn the Unity first shell from a simulation where resource nodes are decorative
into one where the canonical RTS primary economy loop is live. Before this slice,
workers could move, buildings existed, production queues drained, but no worker
had ever actually gathered a resource or deposited anything into the faction
stockpile. Resources were consumed at production time but never replenished. This
slice closes that loop end-to-end against canonical unit stats.

## Work Completed

- Added `unity/Assets/_Bloodlines/Code/Components/WorkerGatherComponent.cs`:
  - New `WorkerGatherPhase` enum: Idle, Seeking, Gathering, Returning, Depositing.
  - New `WorkerGatherComponent` with assigned node, assigned resource id, carry
    resource id, carry amount, carry capacity, gather rate, phase, gather radius,
    deposit radius. Mirrors the browser runtime's gatherer state.
- Added `unity/Assets/_Bloodlines/Code/Systems/WorkerGatherSystem.cs`:
  - Advances every controlled worker through the canonical loop each simulation
    tick: Seeking issues a `MoveCommandComponent` toward the assigned node and
    flips to Gathering on arrival; Gathering decrements the node's `Amount`
    and accrues carry at the unit's `gatherRate`; Returning issues a move to the
    nearest owned, completed, alive drop-off; Depositing accrues carry into the
    faction's `ResourceStockpileComponent` matching the carry resource id, then
    resumes on the same node if it still has resources or idles if depleted.
  - Drop-off targeting resolves the nearest `command_hall` owned by the worker's
    faction, filtering out construction sites and dead buildings.
  - Deposit resolution handles every canonical primary: gold, wood, stone, iron,
    food, water, influence; unknown resource ids fall back to gold so nothing
    drops silently on disk.
  - `UpdateAfter(typeof(UnitProductionSystem))` keeps the gather pass coherent
    with the existing production tick ordering.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  - New `TryDebugAssignSelectedWorkersToGatherResource(resourceId)` debug API
    that filters the selection to workers, locates the nearest resource node of
    the given type, and assigns each worker a fresh `WorkerGatherComponent` with
    its canonical `carryCapacity` and `gatherRate`. Returns the number assigned.
  - New `TryDebugGetFactionStockpile(factionId, out gold, wood, stone, iron, food, water, influence)`
    debug API so governed validators can observe stockpile motion without
    duplicating entity queries.
  - New `GetControlledWorkersWithActiveGatherCount()` for diagnostic counts of
    non-idle gatherers on the controlled faction.
  - New internal `TryFindNearestControlledResourceNode` helper that reuses the
    current selection position as the search reference.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` now persists `gatherResourceId` (default "gold"),
    `gatherAssigned`, `gatherAssignedWorkerCount`, `gatherInitialFactionGold`,
    `gatherMinimumDepositAmount` (default `5f`), `gatherDepositObserved`,
    `gatherLatestFactionGold`, `gatherAssignedUtcTicks`, and
    `gatherCycleTimeoutSeconds` (default `40f`).
  - New `ProbeWorkerGatherCycle` helper runs after the constructed
    `barracks -> militia` completion. It snapshots the controlled faction's gold,
    assigns all selected controlled workers to gather gold via the new debug API,
    persists state, then on subsequent probes watches the stockpile until either
    the deposit delta exceeds the minimum threshold (green) or the configured
    gather cycle timeout elapses without a deposit (fail).
  - Final success diagnostics line now carries `gatherResource`, `gatherAssigned`,
    `gatherAssignedWorkerCount`, `gatherInitialFactionGold`,
    `gatherLatestFactionGold`, and `gatherDepositObserved` alongside the
    construction and production summaries.
  - `StartupTimeoutSeconds` bumped from `75d` to `120d` so the full governed
    sequence (bootstrap + queue cancel-and-refund + mid-production progress +
    dwelling build + barracks build + militia train + gather cycle) completes
    with headroom.
- Updated `unity/Assembly-CSharp.csproj` to include the new component and system
  files so the dotnet build path compiles them without waiting for Unity's
  generator refresh.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors
  and 110 pre-existing warnings (no new warnings).
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed. The
  latest success line ends with:
  - `gatherResource='gold'`
  - `gatherAssigned=True`
  - `gatherAssignedWorkerCount=5`
  - `gatherInitialFactionGold=45.0`
  - `gatherLatestFactionGold=55.0`
  - `gatherDepositObserved=True`
  - along with the preserved `productionProgressInitialRatio=0.004`,
    `productionProgressLatestRatio=0.084`,
    `productionProgressAdvancementVerified=True`,
    `constructionProgressInitialRatio=0.001`,
    `constructionProgressLatestRatio=0.915`,
    `constructionProgressAdvancementVerified=True`,
    `buildings=11`, `units=18`, `controlledUnits=8`, `populationCap=24`,
    `constructedProductionUnitType='militia'`.
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves the full primary economy loop:

- command-shell selection, drag-box, control groups, framing, and formation-aware move
- two-deep production queue with cancel-and-refund and mid-production advancement
  observation
- worker-led dwelling construction with mid-construction progress observability
- worker-led barracks construction with post-completion `militia` continuity
- world-space construction and production progress bars visible above buildings
- **primary-economy gather-deposit loop: controlled workers assigned to a canonical
  resource node traverse Seeking, Gathering, Returning, and Depositing phases under
  `MoveCommandComponent` control, decrement the node's canonical amount, and accrue
  carry into the faction's `ResourceStockpileComponent`**

## Next Action

1. Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify the new gather loop via the debug API from a Play Mode console:
   - select controlled workers (drag-box or click)
   - call `TryDebugAssignSelectedWorkersToGatherResource("gold")` on the
     `BloodlinesDebugCommandSurface`
   - watch the workers walk to the gold node, gather to carry cap, and return
     to the command hall
   - confirm the faction gold stockpile rises in multiples of 10 (carry capacity)
3. Continue into right-click-on-node as a player-facing gather assignment, a
   carry indicator on worker proxies in the presentation bridge, wood / stone /
   iron node coverage (they already work in the backend but need command-surface
   parity), and a starvation-rate economy tick so food and water consumption
   begin flowing against housing and unit populations as canon specifies.
