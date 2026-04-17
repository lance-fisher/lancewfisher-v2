# 2026-04-16 Unity Production Progress Observability And World-Space Bar

## Goal

Extend the Session 107 construction-progress observability pattern into the production
queue. Before this slice, under-construction buildings carried both a HUD progress panel
and a world-space progress bar and the governed runtime smoke proved mid-construction
advancement; production queues, however, only surfaced their remaining seconds inside the
production panel queue rows and had no world-space indicator, and the governed runtime
smoke did not prove mid-production queue advancement before completion.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs`:
  - New per-entity world-space production progress bar consisting of a track cube and a
    fill cube parented under the bridge and cached in a `ProductionProgressProxy` map,
    mirroring the construction-progress proxy pattern.
  - New `SyncProductionProgressBar` pass invoked from `SyncBuildings` when a building is
    not under construction and has at least one active `ProductionQueueItemElement`.
    The fill width tracks `1 - queue[0].RemainingSeconds / queue[0].TotalSeconds`.
  - New `RemoveStaleProductionProgressProxies` pass mirroring the construction-proxy
    cleanup so buildings that drain their queue, finish production, or die release their
    bars without leaking GameObjects.
  - `ClearAllProxies` now also disposes any outstanding production progress proxies so
    the bridge never retains orphaned world-space bars across world rebinds.
  - New serialized fields `presentProductionProgress`, `productionProgressTrackColor`,
    `productionProgressFillColor`, `productionProgressBarWidth`,
    `productionProgressBarHeight`, and `productionProgressBarVerticalOffset` so the bar is
    toggleable and themable without code edits. Defaults use a distinct blue fill so the
    production bar reads separately from the gold construction bar when both are visible.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  - New `TryDebugGetSelectedProductionProgress(out ratio, out remaining, out total, out unitId, out buildingTypeId)`
    debug API that reads the selected controlled building's active queue head without
    duplicating the bridge's query logic, and returns false when the selected entity is
    not a building, has no `ProductionQueueItemElement` buffer, or the buffer is empty.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` now persists `productionProgressInitialSampled`,
    `productionProgressInitialRatio`,
    `productionProgressInitialRemainingSeconds`,
    `productionProgressInitialTotalSeconds`,
    `productionProgressInitialUtcTicks`,
    `productionProgressAdvancementVerified`,
    `productionProgressLatestRatio`,
    `productionProgressLatestRemainingSeconds`,
    `productionProgressMinimumAdvancementRatio` (default `0.08`), and
    `productionProgressAdvancementWaitSeconds` (default `1.25`).
  - New `ProbeProductionProgress` helper mirrors the construction helper. It runs during a
    dedicated mid-production observation window between cancel-and-refund verification and
    the final unit-count check. It re-selects the controlled production building, reads
    live queue head progress, validates total seconds positive, ratio in `[0, 1]`, the
    observed building type matches expected, the observed active unit id matches expected,
    records the initial sample, and on subsequent probes asserts advancement of at least
    the configured minimum ratio within the configured wait window (or at least positive
    advancement once the window has elapsed).
  - The pre-existing spawn-floor gate and the strict phase-equality gate now carry a
    `midProductionObservationWindow` bypass so the probe can sample live progress before
    the queued villager spawns without tripping the floor or equality checks. All other
    counts (factions, buildings, resource nodes, control points, settlements) remain
    strictly validated during the window so bootstrap failures cannot masquerade as
    progress observation.
  - Final success diagnostics line now carries `productionProgressInitialRatio`,
    `productionProgressLatestRatio`, and `productionProgressAdvancementVerified` alongside
    the existing production and construction summaries, so
    `artifacts/unity-bootstrap-runtime-smoke.log` is now a direct witness to live
    mid-production queue progress behavior.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors. Existing
  editor/importer `CS0649` debt remained at 110 warnings (no new warnings introduced).
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed. The latest
  success line ends with:
  - `productionProgressInitialRatio=0.004`
  - `productionProgressLatestRatio=0.084`
  - `productionProgressAdvancementVerified=True`
  - `constructionProgressInitialRatio=0.001`,
    `constructionProgressLatestRatio=0.915`,
    `constructionProgressAdvancementVerified=True`
  - `constructionPlaced=True`, `constructionSites=0`, `populationCap=24`
  - `constructedProductionBuildingPlaced=True`, `constructedProductionSites=0`,
    `constructedProductionQueued=True`, `constructedProductionUnitType='militia'`
  - final counts: `factions=3`, `buildings=11`, `units=18`, `controlledUnits=8`
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves:

- command-shell selection, drag-box, control groups, framing, and formation-aware move
- two-deep `command_hall -> villager` queue issue, rear-entry cancel, refund, surviving
  front-entry **mid-production advancement observation**, and completion
- worker-led `dwelling` placement with mid-construction progress observability through to
  completion and final population-cap gain
- worker-led `barracks` construction completion with immediate post-completion
  `militia` production continuity

The world-space shell now shows, at a glance:

- a gold construction progress bar above every under-construction building
- a blue production progress bar above every building with an active training queue

## Next Action

1. Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify:
   - existing `dwelling` placement, construction progress readout, world-space bar, and
     final population-cap increase
   - existing `barracks` completion and `militia` production flow
   - new world-space production progress bar above `command_hall` while the villager
     queue is active, and above `barracks` while the militia queue is active
   - new production progress readout accessible via the debug API after selecting the
     producing building
3. Continue into broader production-roster progress verification (swordsman, bowman,
   axeman, verdant_warden from barracks; additional worker training from command_hall),
   a construction-progress HUD panel extension for selected workers with pending
   placement, deeper build-placement UX, or more house-distinct rosters after the
   in-editor shell is confirmed.
