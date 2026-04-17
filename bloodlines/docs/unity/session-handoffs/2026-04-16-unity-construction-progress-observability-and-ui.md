# 2026-04-16 Unity Construction Progress Observability and UI

## Goal

Deepen the Unity first-shell construction lane from "construction placed -> construction
completed" to "construction progress is continuously observable in the HUD, in the world,
and through a governed debug getter that proves live progression between samples before
completion". Before this slice, under-construction buildings were visually distinguished
by a shorter gold-tinted proxy, but the HUD exposed no percent-complete readout, the
world-space presentation had no progress indicator, and the governed runtime smoke only
proved the completion boundary rather than the intermediate progression behavior.

## Work Completed

- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugCommandSurface.cs`:
  - New `SelectedConstructionSiteSnapshot` struct plus `TryGetSelectedConstructionSiteSnapshot`
    helper that reads `ConstructionStateComponent` alongside building type, faction, health,
    total seconds, remaining seconds, and a saturate-clamped progress ratio.
  - New `DrawConstructionProgressPanel(EntityManager)` OnGUI panel that renders a dedicated
    construction-progress card when a single under-construction controlled building is
    selected. It shows display name, percent complete, remaining and total seconds, a
    horizontal progress bar, and an integrity readout against current and max health.
  - New `TryDebugSelectControlledConstructionSite(buildingTypeId)` and
    `TryDebugGetSelectedConstructionProgress(out ratio, out remaining, out total, out typeId)`
    debug API entry points so governed validators can observe progress without duplicating
    entity query logic.
  - New `TrySelectControlledConstructionSite` internal selector that filters on
    `ConstructionStateComponent` presence so the progress panel targets active sites even
    when a completed building of the same type exists.
  - New serialized fields `showConstructionProgressPanel`, `constructionProgressPanelSpacing`,
    `constructionProgressFillColor`, and `constructionProgressTrackColor` so the panel is
    toggleable and themable without code edits.
- Extended `unity/Assets/_Bloodlines/Code/Debug/BloodlinesDebugEntityPresentationBridge.cs`:
  - New per-entity world-space construction progress bar consisting of a track cube and a
    fill cube parented under the bridge and cached in a `ConstructionProgressProxy` map.
  - New `SyncConstructionProgressBar` pass invoked from `SyncBuildings` when a building has
    `ConstructionStateComponent`, scaling the fill width by progress, positioning both cubes
    above the building proxy, and coloring them from the new serialized track and fill
    colors.
  - New `RemoveStaleConstructionProgressProxies` pass mirroring the existing proxy cleanup
    so completed or destroyed construction sites drop their bars without leaking GameObjects.
  - `ClearAllProxies` now also disposes any outstanding construction progress proxies so
    the bridge never retains orphaned world-space bars across world rebinds.
- Extended `unity/Assets/_Bloodlines/Code/Editor/BloodlinesBootstrapRuntimeSmokeValidation.cs`:
  - `RuntimeSmokeState` now persists `constructionProgressInitialSampled`,
    `constructionProgressInitialRatio`, `constructionProgressInitialRemainingSeconds`,
    `constructionProgressInitialTotalSeconds`, `constructionProgressInitialUtcTicks`,
    `constructionProgressAdvancementVerified`, `constructionProgressLatestRatio`,
    `constructionProgressLatestRemainingSeconds`,
    `constructionProgressMinimumAdvancementRatio` (default `0.08`), and
    `constructionProgressAdvancementWaitSeconds` (default `1.25`).
  - New `ProbeConstructionProgress` helper runs between construction placement and the
    pre-existing completion gate. It selects the active construction site via the new
    debug API, reads live progress, validates total seconds positive, ratio in `[0, 1]`,
    building type matches expected, records the initial sample, then on subsequent probes
    asserts either an advancement of at least the configured minimum ratio or a floor-case
    positive advancement once the wait window has elapsed.
  - Final success diagnostics line now carries
    `constructionProgressInitialRatio`, `constructionProgressLatestRatio`, and
    `constructionProgressAdvancementVerified` alongside the existing construction summary,
    so `artifacts/unity-bootstrap-runtime-smoke.log` is now a direct witness to live
    mid-construction progress behavior.

## Verification

- `dotnet build unity/Assembly-CSharp.csproj -nologo` passed with 0 warnings and 0 errors.
- `dotnet build unity/Assembly-CSharp-Editor.csproj -nologo` passed with 0 errors. Existing
  editor/importer `CS0649` debt remained at 110 warnings (no new warnings introduced).
- `scripts/Invoke-BloodlinesUnityBootstrapRuntimeSmokeValidation.ps1` passed. The latest
  success line ends with:
  - `constructionProgressInitialRatio=0.002`
  - `constructionProgressLatestRatio=0.916`
  - `constructionProgressAdvancementVerified=True`
  - `constructionPlaced=True`, `constructionSites=0`, `populationCap=24`
  - `constructedProductionBuildingPlaced=True`,
    `constructedProductionSites=0`, `constructedProductionQueued=True`,
    `constructedProductionUnitType='militia'`
  - final counts: `factions=3`, `buildings=11`, `units=18`, `controlledUnits=8`
- `scripts/Invoke-BloodlinesUnityValidateCanonicalSceneShells.ps1` passed.
- `node tests/data-validation.mjs` passed.
- `node tests/runtime-bridge.mjs` passed.

## Current Readiness

The governed Unity first shell now proves:

- command-shell selection, drag-box, control groups, framing, and formation-aware move
- two-deep `command_hall -> villager` queue issue, rear-entry cancel, refund, and surviving
  front-entry completion
- worker-led `dwelling` placement with **mid-construction progress observability**
  (percent-complete readout, world-space bar, governed advancement assertion between
  samples) through to completion and final population-cap gain
- worker-led `barracks` construction completion with immediate post-completion
  `militia` production continuity

The remaining real gate for this shell is still manual in-editor feel verification in
`Bootstrap.unity`.

## Next Action

1. Open `unity/Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity` in Unity 6.3 LTS.
2. Manually verify:
   - `dwelling` placement feel, obstruction response, the new construction progress panel
     readout (percent, remaining seconds, integrity bar), the new world-space progress bar
     above the construction proxy, smooth advancement toward completion, and final
     population-cap increase
   - `barracks` completion, selection, `militia` queue visibility, training completion, and
     controlled-unit growth feel
   - existing command-shell behaviors: selection, drag-box, control groups, framing, move
     feel, production cancel feel, camera feel, and control-point capture behavior
3. Continue into broader construction-roster progress verification (farm, well,
   lumber_camp, quarry, mine_works), a world-space progress bar applied to the barracks
   construction phase as well, production-completion progress overlay for queued trainings,
   deeper build-placement UX, or more house-distinct construction rosters after the
   in-editor shell is confirmed.
