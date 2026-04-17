#if UNITY_EDITOR
using System;
using Bloodlines.Authoring;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using Bloodlines.Debug;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Runs a governed Bootstrap-scene runtime smoke validation by entering Play Mode,
    /// waiting for the ECS world to settle, and comparing spawned entity counts against
    /// the canonical MapDefinition assigned to the Bootstrap authoring anchor.
    /// </summary>
    [InitializeOnLoad]
    public static class BloodlinesBootstrapRuntimeSmokeValidation
    {
        private const string BootstrapScenePath = "Assets/_Bloodlines/Scenes/Bootstrap/Bootstrap.unity";
        private const string PendingKey = "Bloodlines.EditorTools.BootstrapRuntimeSmokeValidation.Pending";
        private const string StateKey = "Bloodlines.EditorTools.BootstrapRuntimeSmokeValidation.State";
        private const string StartupTimeoutUtcTicksKey = "Bloodlines.EditorTools.BootstrapRuntimeSmokeValidation.TimeoutUtcTicks";
        private const string WarmupSeconds = "1.5";
        private const double StartupTimeoutSeconds = 120d;
        private const double ProbeLogIntervalSeconds = 5d;

        static BloodlinesBootstrapRuntimeSmokeValidation()
        {
            EditorApplication.update -= ContinuePendingValidation;
            EditorApplication.update += ContinuePendingValidation;
        }

        [MenuItem("Bloodlines/Scenes/Run Bootstrap Runtime Smoke Validation")]
        public static void RunBootstrapRuntimeSmokeValidation()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            StartValidation(batchMode: false);
        }

        public static void RunBatchBootstrapRuntimeSmokeValidation()
        {
            StartValidation(batchMode: true);
        }

        private static void StartValidation(bool batchMode)
        {
            try
            {
                if (!System.IO.File.Exists(BootstrapScenePath))
                {
                    CompleteImmediately(
                        batchMode,
                        success: false,
                        "Bootstrap runtime smoke validation could not find " + BootstrapScenePath + ".");
                    return;
                }

                EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);

                var authoring = UnityEngine.Object.FindFirstObjectByType<BloodlinesMapBootstrapAuthoring>();
                if (authoring == null)
                {
                    CompleteImmediately(
                        batchMode,
                        success: false,
                        "Bootstrap runtime smoke validation could not find BloodlinesMapBootstrapAuthoring in " +
                        BootstrapScenePath + ".");
                    return;
                }

                if (authoring.Map == null)
                {
                    CompleteImmediately(
                        batchMode,
                        success: false,
                        "Bootstrap runtime smoke validation found BloodlinesMapBootstrapAuthoring but its MapDefinition is null.");
                    return;
                }

                var debugBridge = UnityEngine.Object.FindFirstObjectByType<BloodlinesDebugEntityPresentationBridge>();
                var state = new RuntimeSmokeState
                {
                    batchMode = batchMode,
                    phase = "await_play_mode",
                    expectedFactionCount = authoring.Map.factions?.Length ?? 0,
                    expectedBuildingCount = SumBuildings(authoring.Map),
                    expectedUnitCount = SumUnits(authoring.Map),
                    expectedResourceNodeCount = authoring.Map.resourceNodes?.Length ?? 0,
                    expectedControlPointCount = authoring.Map.controlPoints?.Length ?? 0,
                    expectedSettlementCount = authoring.Map.settlements?.Length ?? 0,
                    expectedProxyMinimum = CalculateExpectedProxyMinimum(authoring.Map),
                    expectDebugBridge = debugBridge != null,
                    mapId = authoring.Map.id ?? string.Empty,
                    productionBuildingTypeId = "command_hall",
                    productionUnitId = "villager",
                    constructionBuildingTypeId = "dwelling",
                    constructionPopulationCapBonus = 6,
                    constructionProgressMinimumAdvancementRatio = 0.08f,
                    constructionProgressAdvancementWaitSeconds = 1.25f,
                    productionProgressMinimumAdvancementRatio = 0.08f,
                    productionProgressAdvancementWaitSeconds = 1.25f,
                    constructedProductionBuildingTypeId = "barracks",
                    constructedProductionUnitId = "militia",
                    gatherResourceId = "gold",
                    gatherMinimumDepositAmount = 5f,
                    gatherCycleTimeoutSeconds = 40f,
                    trickleMinimumFoodGain = 2f,
                    trickleMinimumWaterGain = 2f,
                    trickleTimeoutSeconds = 30f,
                    starvationIncludeWaterCrisis = true,
                    starvationTimeoutSeconds = 15f,
                    loyaltyMinimumDeltaMagnitude = 8f,
                    capPressureTimeoutSeconds = 10f,
                    aiFactionId = "enemy",
                    aiMinimumGoldDelta = 5f,
                    aiMinimumUnitDelta = 1,
                    aiActivityTimeoutSeconds = 35f,
                    aiMinimumBuildingGain = 1,
                    aiConstructionTimeoutSeconds = 30f,
                    stabilitySurplusTimeoutSeconds = 10f,
                };

                SaveState(state);
                SessionState.SetBool(PendingKey, true);
                SessionState.SetString(StartupTimeoutUtcTicksKey, DateTime.UtcNow.AddSeconds(StartupTimeoutSeconds).Ticks.ToString());
                EditorApplication.isPlaying = true;
            }
            catch (Exception exception)
            {
                CompleteImmediately(
                    batchMode,
                    success: false,
                    "Bootstrap runtime smoke validation failed during startup: " + exception);
            }
        }

        private static void ContinuePendingValidation()
        {
            if (!SessionState.GetBool(PendingKey, false))
            {
                return;
            }

            try
            {
                var state = LoadState();
                if (state == null)
                {
                    ClearPersistentState();
                    return;
                }

                if (HasTimedOut())
                {
                    var timeoutProbe = ProbeRuntime(state);
                    CompleteValidation(
                        state,
                        success: false,
                        "Bootstrap runtime smoke validation timed out before the ECS shell settled. " +
                        timeoutProbe.diagnostics);
                    return;
                }

                if (string.Equals(state.phase, "await_play_mode", StringComparison.Ordinal))
                {
                    if (EditorApplication.isPlaying)
                    {
                        state.phase = "running";
                        state.playModeEnteredUtcTicks = DateTime.UtcNow.Ticks;
                        SaveState(state);
                    }

                    return;
                }

                if (string.Equals(state.phase, "await_exit", StringComparison.Ordinal))
                {
                    if (!EditorApplication.isPlaying)
                    {
                        if (state.batchMode)
                        {
                            int exitCode = state.resultCode;
                            ClearPersistentState();
                            EditorApplication.Exit(exitCode);
                            return;
                        }

                        ClearPersistentState();
                    }

                    return;
                }

                if (!EditorApplication.isPlaying)
                {
                    CompleteValidation(
                        state,
                        success: false,
                        "Bootstrap runtime smoke validation observed Play Mode exit before runtime verification completed.");
                    return;
                }

                if (ElapsedSeconds(state.playModeEnteredUtcTicks) < ParseWarmupSeconds())
                {
                    return;
                }

                var probe = ProbeRuntime(state);
                if (!probe.ready)
                {
                    if (ShouldEmitProgressLog(state, probe.diagnostics))
                    {
                        state.lastProbeDiagnostics = probe.diagnostics;
                        state.lastProbeLogUtcTicks = DateTime.UtcNow.Ticks;
                        SaveState(state);
                        UnityEngine.Debug.Log("Bootstrap runtime smoke validation still waiting: " + probe.diagnostics);
                    }

                    return;
                }

                CompleteValidation(state, probe.success, probe.message);
            }
            catch (Exception exception)
            {
                var state = LoadState() ?? new RuntimeSmokeState { batchMode = Application.isBatchMode };
                CompleteValidation(
                    state,
                    success: false,
                    "Bootstrap runtime smoke validation crashed during runtime probing: " + exception);
            }
        }

        private static ProbeResult ProbeRuntime(RuntimeSmokeState state)
        {
            string activeScenePath = SceneManager.GetActiveScene().path;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return ProbeResult.NotReady(
                    "worldCreated=false, activeScene=" + Quote(activeScenePath) +
                    ", mapId=" + Quote(state.mapId) + ".");
            }

            var entityManager = world.EntityManager;

            int pendingBootstrapCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MapBootstrapPendingTag>()).CalculateEntityCount();
            int bootstrapConfigCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MapBootstrapConfigComponent>()).CalculateEntityCount();
            int realmCycleConfigCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<RealmCycleConfig>()).CalculateEntityCount();
            int factionCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>()).CalculateEntityCount();
            int buildingCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>()).CalculateEntityCount();
            int unitCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>()).CalculateEntityCount();
            int resourceNodeCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceNodeComponent>()).CalculateEntityCount();
            int controlPointCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>()).CalculateEntityCount();
            int settlementCount = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SettlementComponent>()).CalculateEntityCount();
            var debugBridge = UnityEngine.Object.FindFirstObjectByType<BloodlinesDebugEntityPresentationBridge>();
            var commandSurface = UnityEngine.Object.FindFirstObjectByType<BloodlinesDebugCommandSurface>();
            int debugBridgeChildCount = debugBridge != null ? debugBridge.transform.childCount : 0;
            int controlledUnitCount = 0;
            int commandSelectionCount = 0;
            int controlGroupTwoCount = 0;
            bool commandFrameSucceeded = false;
            int productionBuildingSelectionCount = 0;
            int productionQueueCount = 0;
            int expectedUnitCountForCurrentPhase =
                state.constructedProductionQueued && state.expectedUnitCountAfterConstructedProduction > 0
                    ? state.expectedUnitCountAfterConstructedProduction
                    : state.productionCancelVerified && state.productionQueued && state.expectedUnitCountAfterProduction > 0
                    ? state.expectedUnitCountAfterProduction
                    : state.expectedUnitCount;
            int expectedBuildingCountForCurrentPhase =
                state.constructedProductionBuildingPlaced && state.expectedBuildingCountAfterConstructedProductionBuilding > 0
                    ? state.expectedBuildingCountAfterConstructedProductionBuilding
                    : state.constructionPlaced && state.expectedBuildingCountAfterConstruction > 0
                    ? state.expectedBuildingCountAfterConstruction
                    : state.expectedBuildingCount;
            int expectedProxyMinimumForCurrentPhase =
                state.constructedProductionQueued && state.expectedProxyMinimumAfterConstructedProduction > 0
                    ? state.expectedProxyMinimumAfterConstructedProduction
                    : state.constructedProductionBuildingPlaced && state.expectedProxyMinimumAfterConstructedProductionBuilding > 0
                    ? state.expectedProxyMinimumAfterConstructedProductionBuilding
                    : state.constructionPlaced && state.expectedProxyMinimumAfterConstruction > 0
                    ? state.expectedProxyMinimumAfterConstruction
                    : state.expectedProxyMinimum;

            string diagnostics =
                "activeScene=" + Quote(activeScenePath) +
                ", world=" + Quote(world.Name) +
                ", mapId=" + Quote(state.mapId) +
                ", pendingBootstrap=" + pendingBootstrapCount +
                ", bootstrapConfig=" + bootstrapConfigCount +
                ", realmCycleConfig=" + realmCycleConfigCount +
                ", factions=" + factionCount + "/" + state.expectedFactionCount +
                ", buildings=" + buildingCount + "/" + expectedBuildingCountForCurrentPhase +
                ", units=" + unitCount + "/" + expectedUnitCountForCurrentPhase +
                ", resourceNodes=" + resourceNodeCount + "/" + state.expectedResourceNodeCount +
                ", controlPoints=" + controlPointCount + "/" + state.expectedControlPointCount +
                ", settlements=" + settlementCount + "/" + state.expectedSettlementCount +
                ", debugBridgePresent=" + (debugBridge != null) +
                ", debugBridgeChildren=" + debugBridgeChildCount +
                ", expectedProxyMinimum=" + expectedProxyMinimumForCurrentPhase +
                ", expectDebugBridge=" + state.expectDebugBridge +
                ", commandSurfacePresent=" + (commandSurface != null) + ".";

            if (pendingBootstrapCount > 0)
            {
                return ProbeResult.NotReady("Bootstrap seed entity is still pending. " + diagnostics);
            }

            bool midProductionObservationWindow =
                state.productionCancelVerified &&
                state.productionQueued &&
                !state.productionProgressAdvancementVerified &&
                unitCount < state.expectedUnitCountAfterProduction;
            bool aiObservationWindow =
                state.aiBaselineSampled && !state.aiActivityObserved;
            bool aiFlexibleWindow = state.aiBaselineSampled;
            bool unitCountFlexibleWindow = midProductionObservationWindow || aiFlexibleWindow;
            if (midProductionObservationWindow)
            {
                var midProductionProgressProbe = ProbeProductionProgress(
                    commandSurface,
                    state,
                    unitCount,
                    diagnostics);
                if (midProductionProgressProbe.HasValue)
                {
                    return midProductionProgressProbe.Value;
                }
            }

            if (factionCount < state.expectedFactionCount ||
                buildingCount < expectedBuildingCountForCurrentPhase ||
                (unitCount < expectedUnitCountForCurrentPhase && !unitCountFlexibleWindow) ||
                resourceNodeCount < state.expectedResourceNodeCount ||
                controlPointCount < state.expectedControlPointCount ||
                settlementCount < state.expectedSettlementCount)
            {
                return ProbeResult.NotReady("Spawn counts have not reached the expected floor yet. " + diagnostics);
            }

            if (bootstrapConfigCount != 1)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: expected exactly 1 MapBootstrapConfigComponent entity but found " +
                    bootstrapConfigCount + ". " + diagnostics);
            }

            if (realmCycleConfigCount != 1)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: expected exactly 1 RealmCycleConfig entity but found " +
                    realmCycleConfigCount + ". " + diagnostics);
            }

            if (factionCount != state.expectedFactionCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: faction count mismatch. Expected " +
                    state.expectedFactionCount + " but found " + factionCount + ". " + diagnostics);
            }

            if (buildingCount != expectedBuildingCountForCurrentPhase && !aiFlexibleWindow)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: building count mismatch. Expected " +
                    expectedBuildingCountForCurrentPhase + " but found " + buildingCount + ". " + diagnostics);
            }

            if (unitCount != expectedUnitCountForCurrentPhase && !unitCountFlexibleWindow)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: unit count mismatch. Expected " +
                    expectedUnitCountForCurrentPhase + " but found " + unitCount + ". " + diagnostics);
            }

            if (resourceNodeCount != state.expectedResourceNodeCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: resource-node count mismatch. Expected " +
                    state.expectedResourceNodeCount + " but found " + resourceNodeCount + ". " + diagnostics);
            }

            if (controlPointCount != state.expectedControlPointCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: control-point count mismatch. Expected " +
                    state.expectedControlPointCount + " but found " + controlPointCount + ". " + diagnostics);
            }

            if (settlementCount != state.expectedSettlementCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: settlement count mismatch. Expected " +
                    state.expectedSettlementCount + " but found " + settlementCount + ". " + diagnostics);
            }

            if (state.expectDebugBridge)
            {
                if (debugBridge == null)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: BloodlinesDebugEntityPresentationBridge disappeared in Play Mode. " +
                        diagnostics);
                }

                if (debugBridgeChildCount < expectedProxyMinimumForCurrentPhase)
                {
                    return ProbeResult.NotReady("Debug proxy count has not reached the expected minimum yet. " + diagnostics);
                }
            }

            if (commandSurface == null)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: BloodlinesDebugCommandSurface is missing in Play Mode. " +
                    diagnostics);
            }

            controlledUnitCount = CountControlledUnits(
                entityManager,
                commandSurface.ControlledFactionId,
                commandSurface.SelectOnlyCombatUnits);
            if (controlledUnitCount <= 0)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: no controlled units were available for command-shell validation. " +
                    diagnostics);
            }

            if (!commandSurface.TryDebugSelectAllControlledUnits())
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not select all controlled units. " +
                    diagnostics);
            }

            commandSelectionCount = commandSurface.GetSelectedCountForDebug();
            if (commandSelectionCount != controlledUnitCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command-shell select-all mismatch. Expected " +
                    controlledUnitCount + " but found " + commandSelectionCount + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugSaveControlGroup(2))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not save control group 2. " +
                    diagnostics);
            }

            controlGroupTwoCount = commandSurface.GetControlGroupCountForDebug(2);
            if (controlGroupTwoCount != controlledUnitCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: control group 2 count mismatch. Expected " +
                    controlledUnitCount + " but found " + controlGroupTwoCount + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugClearSelection())
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell did not clear selection cleanly. " +
                    diagnostics);
            }

            if (!commandSurface.TryDebugRecallControlGroup(2))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not recall control group 2. " +
                    diagnostics);
            }

            commandSelectionCount = commandSurface.GetSelectedCountForDebug();
            if (commandSelectionCount != controlledUnitCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: control-group recall mismatch. Expected " +
                    controlledUnitCount + " but found " + commandSelectionCount + ". " + diagnostics);
            }

            commandFrameSucceeded = commandSurface.TryDebugFrameSelection();
            if (!commandFrameSucceeded)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not frame the active selection. " +
                    diagnostics);
            }

            if (!state.productionCancelVerified)
            {
                if (!state.productionQueued)
                {
                    if (!commandSurface.TryDebugSelectControlledBuilding(state.productionBuildingTypeId))
                    {
                        return ProbeResult.Fail(
                            "Bootstrap runtime smoke validation failed: command shell could not select controlled building type '" +
                            state.productionBuildingTypeId + "'. " + diagnostics);
                    }

                    productionBuildingSelectionCount = commandSurface.GetSelectedCountForDebug();
                    if (productionBuildingSelectionCount != 1)
                    {
                        return ProbeResult.Fail(
                            "Bootstrap runtime smoke validation failed: expected exactly 1 selected production building but found " +
                            productionBuildingSelectionCount + ". " + diagnostics);
                    }

                    if (!commandSurface.TryDebugQueueProduction(state.productionBuildingTypeId, state.productionUnitId))
                    {
                        return ProbeResult.Fail(
                            "Bootstrap runtime smoke validation failed: command shell could not queue " +
                            state.productionUnitId + " from " + state.productionBuildingTypeId + ". " + diagnostics);
                    }

                    if (!commandSurface.TryDebugQueueProduction(state.productionBuildingTypeId, state.productionUnitId))
                    {
                        return ProbeResult.Fail(
                            "Bootstrap runtime smoke validation failed: command shell could not queue the second " +
                            state.productionUnitId + " from " + state.productionBuildingTypeId + ". " + diagnostics);
                    }

                    productionQueueCount = commandSurface.GetProductionQueueCountForDebug(state.productionBuildingTypeId);
                    if (productionQueueCount != 2)
                    {
                        return ProbeResult.Fail(
                            "Bootstrap runtime smoke validation failed: expected a two-deep production queue before cancellation verification but found " +
                            productionQueueCount + ". " +
                            diagnostics);
                    }

                    state.productionQueued = true;
                    state.expectedUnitCountAfterProduction = unitCount + 1;
                    state.expectedControlledUnitCountAfterProduction = controlledUnitCount + 1;
                    SaveState(state);
                    return ProbeResult.NotReady(
                        "Two-deep production queue issued for tail cancel-and-refund verification on " +
                        state.productionBuildingTypeId + " -> " + state.productionUnitId + ". " + diagnostics);
                }

                if (!TryGetControlledQueuedProduction(
                        entityManager,
                        commandSurface.ControlledFactionId,
                        state.productionBuildingTypeId,
                        1,
                        out var queuedItem))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: second queued production item could not be recovered before cancellation. " +
                        diagnostics);
                }

                if (!TryGetControlledFactionRuntimeSnapshot(
                        entityManager,
                        commandSurface.ControlledFactionId,
                        out var resourcesBeforeCancel,
                        out var populationBeforeCancel))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: controlled faction runtime snapshot was unavailable before cancellation. " +
                        diagnostics);
                }

                if (!commandSurface.TryDebugCancelProduction(state.productionBuildingTypeId, 1))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: command shell could not cancel the queued rear production entry from " +
                        state.productionBuildingTypeId + ". " + diagnostics);
                }

                productionQueueCount = commandSurface.GetProductionQueueCountForDebug(state.productionBuildingTypeId);
                if (productionQueueCount != 1)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: expected one active production entry to remain after tail cancellation but found queue count=" +
                        productionQueueCount + ". " + diagnostics);
                }

                if (unitCount != state.expectedUnitCount)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: unit count changed during cancellation verification. Expected " +
                        state.expectedUnitCount + " but found " + unitCount + ". " + diagnostics);
                }

                if (!TryGetControlledFactionRuntimeSnapshot(
                        entityManager,
                        commandSurface.ControlledFactionId,
                        out var resourcesAfterCancel,
                        out var populationAfterCancel))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: controlled faction runtime snapshot was unavailable after cancellation. " +
                        diagnostics);
                }

                if (!MatchesQueuedRefund(resourcesBeforeCancel, resourcesAfterCancel, in queuedItem))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: queued resource refund delta did not match the canceled unit cost. " +
                        diagnostics +
                        " queuedItem=" + DescribeQueuedItem(in queuedItem) +
                        ", before=" + DescribeResources(in resourcesBeforeCancel) +
                        ", after=" + DescribeResources(in resourcesAfterCancel) + ".");
                }

                if (!MatchesQueuedPopulationRefund(populationBeforeCancel, populationAfterCancel, in queuedItem))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: queued population refund delta did not match the canceled unit cost. " +
                        diagnostics +
                        " queuedItem=" + DescribeQueuedItem(in queuedItem) +
                        ", before=" + DescribePopulation(in populationBeforeCancel) +
                        ", after=" + DescribePopulation(in populationAfterCancel) + ".");
                }

                state.productionCancelVerified = true;
                state.productionQueued = true;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Production tail cancel-and-refund verification passed for " +
                    state.productionBuildingTypeId + " -> " + state.productionUnitId + ". " + diagnostics);
            }

            if (unitCount < state.expectedUnitCountAfterProduction)
            {
                return ProbeResult.NotReady(
                    "Waiting for queued production to complete. Expected unit count " +
                    state.expectedUnitCountAfterProduction + " but found " + unitCount + ". " + diagnostics);
            }

            if (!state.constructedProductionQueued && unitCount != state.expectedUnitCountAfterProduction)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: post-production unit count mismatch. Expected " +
                    state.expectedUnitCountAfterProduction + " but found " + unitCount + ". " + diagnostics);
            }

            productionQueueCount = commandSurface.GetProductionQueueCountForDebug(state.productionBuildingTypeId);
            if (productionQueueCount != 0)
            {
                return ProbeResult.NotReady(
                    "Waiting for production queue to drain after spawn. Active queue count=" +
                    productionQueueCount + ". " + diagnostics);
            }

            controlledUnitCount = CountControlledUnits(
                entityManager,
                commandSurface.ControlledFactionId,
                commandSurface.SelectOnlyCombatUnits);
            if (!state.constructedProductionQueued &&
                controlledUnitCount != state.expectedControlledUnitCountAfterProduction)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: controlled-unit count did not increase after production. Expected " +
                    state.expectedControlledUnitCountAfterProduction + " but found " + controlledUnitCount + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugSelectAllControlledUnits())
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not re-select all controlled units after production. " +
                    diagnostics);
            }

            commandSelectionCount = commandSurface.GetSelectedCountForDebug();
            if (commandSelectionCount != controlledUnitCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: post-production select-all mismatch. Expected " +
                    controlledUnitCount + " but found " + commandSelectionCount + ". " + diagnostics);
            }

            if (!state.constructionPlaced)
            {
                if (!TryGetControlledFactionRuntimeSnapshot(
                        entityManager,
                        commandSurface.ControlledFactionId,
                        out _,
                        out var populationBeforeConstruction))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: controlled faction population snapshot was unavailable before construction placement. " +
                        diagnostics);
                }

                int controlledConstructionBuildingCountBefore = CountControlledBuildingsOfType(
                    entityManager,
                    commandSurface.ControlledFactionId,
                    state.constructionBuildingTypeId);

                if (!commandSurface.TryDebugStartConstruction(state.constructionBuildingTypeId))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: command shell could not place construction for " +
                        state.constructionBuildingTypeId + ". " + diagnostics);
                }

                state.constructionPlaced = true;
                state.expectedBuildingCountAfterConstruction = buildingCount + 1;
                state.expectedProxyMinimumAfterConstruction = state.expectedProxyMinimum + 1;
                state.expectedPopulationCapAfterConstruction = populationBeforeConstruction.Cap + state.constructionPopulationCapBonus;
                state.expectedConstructionBuildingTypeCountAfterConstruction = controlledConstructionBuildingCountBefore + 1;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Construction placement issued for " + state.constructionBuildingTypeId +
                    " near a controlled worker. " + diagnostics);
            }

            int controlledConstructionBuildingCount = CountControlledBuildingsOfType(
                entityManager,
                commandSurface.ControlledFactionId,
                state.constructionBuildingTypeId);
            int controlledConstructionSiteCount = CountControlledConstructionSitesOfType(
                entityManager,
                commandSurface.ControlledFactionId,
                state.constructionBuildingTypeId);
            if (!TryGetControlledFactionRuntimeSnapshot(
                    entityManager,
                    commandSurface.ControlledFactionId,
                    out _,
                    out var populationAfterConstruction))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: controlled faction population snapshot was unavailable during construction verification. " +
                    diagnostics);
            }

            if (controlledConstructionBuildingCount < state.expectedConstructionBuildingTypeCountAfterConstruction)
            {
                return ProbeResult.NotReady(
                    "Waiting for construction building registration to appear. " + diagnostics +
                    " constructionBuildingType=" + Quote(state.constructionBuildingTypeId) +
                    ", controlledConstructionBuildings=" + controlledConstructionBuildingCount +
                    "/" + state.expectedConstructionBuildingTypeCountAfterConstruction + ".");
            }

            var progressProbe = ProbeConstructionProgress(
                commandSurface,
                state,
                controlledConstructionSiteCount,
                diagnostics);
            if (progressProbe.HasValue)
            {
                return progressProbe.Value;
            }

            if (controlledConstructionBuildingCount != state.expectedConstructionBuildingTypeCountAfterConstruction)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: controlled construction building count mismatch for " +
                    state.constructionBuildingTypeId + ". Expected " +
                    state.expectedConstructionBuildingTypeCountAfterConstruction + " but found " +
                    controlledConstructionBuildingCount + ". " + diagnostics);
            }

            if (controlledConstructionSiteCount > 0)
            {
                return ProbeResult.NotReady(
                    "Waiting for " + state.constructionBuildingTypeId + " construction to finish. " + diagnostics +
                    " constructionSites=" + controlledConstructionSiteCount +
                    ", populationCap=" + populationAfterConstruction.Cap +
                    "/" + state.expectedPopulationCapAfterConstruction + ".");
            }

            if (populationAfterConstruction.Cap < state.expectedPopulationCapAfterConstruction)
            {
                return ProbeResult.NotReady(
                    "Waiting for construction population-cap bonus to apply. " + diagnostics +
                    " constructionBuildingType=" + Quote(state.constructionBuildingTypeId) +
                    ", populationCap=" + populationAfterConstruction.Cap +
                    "/" + state.expectedPopulationCapAfterConstruction + ".");
            }

            if (populationAfterConstruction.Cap != state.expectedPopulationCapAfterConstruction)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: population cap mismatch after construction. Expected " +
                    state.expectedPopulationCapAfterConstruction + " but found " +
                    populationAfterConstruction.Cap + ". " + diagnostics);
            }

            controlledUnitCount = CountControlledUnits(
                entityManager,
                commandSurface.ControlledFactionId,
                commandSurface.SelectOnlyCombatUnits);

            if (!commandSurface.TryDebugSelectAllControlledUnits())
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not select all controlled units after construction. " +
                    diagnostics);
            }

            commandSelectionCount = commandSurface.GetSelectedCountForDebug();
            if (commandSelectionCount != controlledUnitCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: post-construction select-all mismatch. Expected " +
                    controlledUnitCount + " but found " + commandSelectionCount + ". " + diagnostics);
            }

            if (!state.constructedProductionBuildingPlaced)
            {
                int controlledConstructedProductionBuildingCountBefore = CountControlledBuildingsOfType(
                    entityManager,
                    commandSurface.ControlledFactionId,
                    state.constructedProductionBuildingTypeId);

                if (!commandSurface.TryDebugStartConstruction(state.constructedProductionBuildingTypeId))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: command shell could not place constructed production building type '" +
                        state.constructedProductionBuildingTypeId + "'. " + diagnostics);
                }

                state.constructedProductionBuildingPlaced = true;
                state.expectedBuildingCountAfterConstructedProductionBuilding = buildingCount + 1;
                state.expectedProxyMinimumAfterConstructedProductionBuilding = expectedProxyMinimumForCurrentPhase + 1;
                state.expectedConstructedProductionBuildingTypeCountAfterPlacement =
                    controlledConstructedProductionBuildingCountBefore + 1;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Construction placement issued for constructed production continuity on " +
                    state.constructedProductionBuildingTypeId + ". " + diagnostics);
            }

            int controlledConstructedProductionBuildingCount = CountControlledBuildingsOfType(
                entityManager,
                commandSurface.ControlledFactionId,
                state.constructedProductionBuildingTypeId);
            int controlledConstructedProductionSiteCount = CountControlledConstructionSitesOfType(
                entityManager,
                commandSurface.ControlledFactionId,
                state.constructedProductionBuildingTypeId);

            if (controlledConstructedProductionBuildingCount <
                state.expectedConstructedProductionBuildingTypeCountAfterPlacement)
            {
                return ProbeResult.NotReady(
                    "Waiting for constructed production building registration to appear. " + diagnostics +
                    " constructedProductionBuildingType=" + Quote(state.constructedProductionBuildingTypeId) +
                    ", controlledConstructedProductionBuildings=" + controlledConstructedProductionBuildingCount +
                    "/" + state.expectedConstructedProductionBuildingTypeCountAfterPlacement + ".");
            }

            if (controlledConstructedProductionBuildingCount !=
                state.expectedConstructedProductionBuildingTypeCountAfterPlacement)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: constructed production building count mismatch for '" +
                    state.constructedProductionBuildingTypeId + "'. Expected " +
                    state.expectedConstructedProductionBuildingTypeCountAfterPlacement + " but found " +
                    controlledConstructedProductionBuildingCount + ". " + diagnostics);
            }

            if (controlledConstructedProductionSiteCount > 0)
            {
                return ProbeResult.NotReady(
                    "Waiting for constructed production building '" + state.constructedProductionBuildingTypeId +
                    "' to finish. " + diagnostics +
                    " constructedProductionSites=" + controlledConstructedProductionSiteCount + ".");
            }

            if (!commandSurface.TryDebugSelectControlledBuilding(state.constructedProductionBuildingTypeId))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not select constructed production building type '" +
                    state.constructedProductionBuildingTypeId + "' after completion. " + diagnostics);
            }

            productionBuildingSelectionCount = commandSurface.GetSelectedCountForDebug();
            if (productionBuildingSelectionCount != 1)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: expected exactly 1 selected constructed production building but found " +
                    productionBuildingSelectionCount + ". " + diagnostics);
            }

            if (!state.constructedProductionQueued)
            {
                if (!commandSurface.TryDebugQueueProduction(
                        state.constructedProductionBuildingTypeId,
                        state.constructedProductionUnitId))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: constructed production building '" +
                        state.constructedProductionBuildingTypeId + "' could not queue unit '" +
                        state.constructedProductionUnitId + "'. " + diagnostics);
                }

                productionQueueCount = commandSurface.GetProductionQueueCountForDebug(
                    state.constructedProductionBuildingTypeId);
                if (productionQueueCount != 1)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: expected one queued unit on constructed production building '" +
                        state.constructedProductionBuildingTypeId + "' but found queue count=" +
                        productionQueueCount + ". " + diagnostics);
                }

                state.constructedProductionQueued = true;
                state.expectedUnitCountAfterConstructedProduction = unitCount + 1;
                state.expectedControlledUnitCountAfterConstructedProduction = controlledUnitCount + 1;
                state.expectedProxyMinimumAfterConstructedProduction =
                    state.expectedProxyMinimumAfterConstructedProductionBuilding + 1;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Constructed production queue issued for " + state.constructedProductionBuildingTypeId +
                    " -> " + state.constructedProductionUnitId + ". " + diagnostics);
            }

            if (unitCount < state.expectedUnitCountAfterConstructedProduction)
            {
                return ProbeResult.NotReady(
                    "Waiting for constructed production to complete. Expected unit count " +
                    state.expectedUnitCountAfterConstructedProduction + " but found " + unitCount + ". " +
                    diagnostics);
            }

            if (unitCount != state.expectedUnitCountAfterConstructedProduction && !unitCountFlexibleWindow)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: post-constructed-production unit count mismatch. Expected " +
                    state.expectedUnitCountAfterConstructedProduction + " but found " + unitCount + ". " +
                    diagnostics);
            }

            productionQueueCount = commandSurface.GetProductionQueueCountForDebug(
                state.constructedProductionBuildingTypeId);
            if (productionQueueCount != 0)
            {
                return ProbeResult.NotReady(
                    "Waiting for constructed production queue to drain after spawn. Active queue count=" +
                    productionQueueCount + ". " + diagnostics);
            }

            controlledUnitCount = CountControlledUnits(
                entityManager,
                commandSurface.ControlledFactionId,
                commandSurface.SelectOnlyCombatUnits);
            if (controlledUnitCount != state.expectedControlledUnitCountAfterConstructedProduction)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: controlled-unit count did not increase after constructed production. Expected " +
                    state.expectedControlledUnitCountAfterConstructedProduction + " but found " +
                    controlledUnitCount + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugSelectAllControlledUnits())
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not select all controlled units after constructed production. " +
                    diagnostics);
            }

            commandSelectionCount = commandSurface.GetSelectedCountForDebug();
            if (commandSelectionCount != controlledUnitCount)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: post-constructed-production select-all mismatch. Expected " +
                    controlledUnitCount + " but found " + commandSelectionCount + ". " + diagnostics);
            }

            var trickleBaselineProbe = ProbeResourceTrickleBaseline(commandSurface, state, diagnostics);
            if (trickleBaselineProbe.HasValue)
            {
                return trickleBaselineProbe.Value;
            }

            var gatherProbe = ProbeWorkerGatherCycle(commandSurface, state, diagnostics);
            if (gatherProbe.HasValue)
            {
                return gatherProbe.Value;
            }

            var trickleGainProbe = ProbeResourceTrickleGain(commandSurface, state, diagnostics);
            if (trickleGainProbe.HasValue)
            {
                return trickleGainProbe.Value;
            }

            var starvationProbe = ProbeStarvationResponse(commandSurface, state, diagnostics);
            if (starvationProbe.HasValue)
            {
                return starvationProbe.Value;
            }

            var capPressureProbe = ProbeCapPressureResponse(commandSurface, state, diagnostics);
            if (capPressureProbe.HasValue)
            {
                return capPressureProbe.Value;
            }

            var aiEconomyProbe = ProbeAIEconomyActivity(commandSurface, state, diagnostics);
            if (aiEconomyProbe.HasValue)
            {
                return aiEconomyProbe.Value;
            }

            var aiConstructionProbe = ProbeAIConstructionActivity(commandSurface, state, diagnostics);
            if (aiConstructionProbe.HasValue)
            {
                return aiConstructionProbe.Value;
            }

            var stabilitySurplusProbe = ProbeStabilitySurplusResponse(commandSurface, state, diagnostics);
            if (stabilitySurplusProbe.HasValue)
            {
                return stabilitySurplusProbe.Value;
            }

            diagnostics =
                diagnostics.TrimEnd('.') +
                ", controlledUnits=" + controlledUnitCount +
                ", commandSelection=" + commandSelectionCount +
                ", controlGroup2=" + controlGroupTwoCount +
                ", commandFrameSucceeded=" + commandFrameSucceeded +
                ", productionCancelVerified=" + state.productionCancelVerified +
                ", productionQueued=" + state.productionQueued +
                ", productionBuildingType=" + Quote(state.productionBuildingTypeId) +
                ", producedUnitType=" + Quote(state.productionUnitId) +
                ", productionProgressInitialRatio=" + state.productionProgressInitialRatio.ToString("0.000") +
                ", productionProgressLatestRatio=" + state.productionProgressLatestRatio.ToString("0.000") +
                ", productionProgressAdvancementVerified=" + state.productionProgressAdvancementVerified +
                ", constructionPlaced=" + state.constructionPlaced +
                ", constructionBuildingType=" + Quote(state.constructionBuildingTypeId) +
                ", constructionSites=" + controlledConstructionSiteCount +
                ", constructionProgressInitialRatio=" + state.constructionProgressInitialRatio.ToString("0.000") +
                ", constructionProgressLatestRatio=" + state.constructionProgressLatestRatio.ToString("0.000") +
                ", constructionProgressAdvancementVerified=" + state.constructionProgressAdvancementVerified +
                ", populationCap=" + populationAfterConstruction.Cap +
                ", constructedProductionBuildingPlaced=" + state.constructedProductionBuildingPlaced +
                ", constructedProductionBuildingType=" + Quote(state.constructedProductionBuildingTypeId) +
                ", constructedProductionSites=" + controlledConstructedProductionSiteCount +
                ", constructedProductionQueued=" + state.constructedProductionQueued +
                ", constructedProductionUnitType=" + Quote(state.constructedProductionUnitId) +
                ", gatherResource=" + Quote(state.gatherResourceId) +
                ", gatherAssigned=" + state.gatherAssigned +
                ", gatherAssignedWorkerCount=" + state.gatherAssignedWorkerCount +
                ", gatherInitialFactionGold=" + state.gatherInitialFactionGold.ToString("0.0") +
                ", gatherLatestFactionGold=" + state.gatherLatestFactionGold.ToString("0.0") +
                ", gatherDepositObserved=" + state.gatherDepositObserved +
                ", trickleInitialFood=" + state.trickleInitialFood.ToString("0.00") +
                ", trickleLatestFood=" + state.trickleLatestFood.ToString("0.00") +
                ", trickleInitialWater=" + state.trickleInitialWater.ToString("0.00") +
                ", trickleLatestWater=" + state.trickleLatestWater.ToString("0.00") +
                ", trickleGainObserved=" + state.trickleGainObserved +
                ", starvationForced=" + state.starvationForced +
                ", starvationIncludeWaterCrisis=" + state.starvationIncludeWaterCrisis +
                ", starvationPreviousPopulation=" + state.starvationPreviousPopulationTotal +
                ", starvationExpectedPopulation=" + state.starvationExpectedPopulationTotal +
                ", starvationLatestPopulation=" + state.starvationLatestPopulationTotal +
                ", starvationObserved=" + state.starvationObserved +
                ", loyaltyPreviousValue=" + state.loyaltyPreviousValue.ToString("0.00") +
                ", loyaltyLatestValue=" + state.loyaltyLatestValue.ToString("0.00") +
                ", loyaltyExpectedMaximumAfterCycle=" + state.loyaltyExpectedMaximumAfterCycle.ToString("0.00") +
                ", loyaltyDeclineObserved=" + state.loyaltyDeclineObserved +
                ", capPressureForced=" + state.capPressureForced +
                ", capPressureTotalAfterSpike=" + state.capPressureTotalAfterSpike +
                ", capPressureCapAfterSpike=" + state.capPressureCapAfterSpike +
                ", capPressureLoyaltyBeforeCycle=" + state.capPressureLoyaltyBeforeCycle.ToString("0.00") +
                ", capPressureLatestLoyalty=" + state.capPressureLatestLoyalty.ToString("0.00") +
                ", capPressureObserved=" + state.capPressureObserved +
                ", aiFaction=" + Quote(state.aiFactionId) +
                ", aiInitialGold=" + state.aiInitialGold.ToString("0.00") +
                ", aiLatestGold=" + state.aiLatestGold.ToString("0.00") +
                ", aiInitialUnitCount=" + state.aiInitialUnitCount +
                ", aiLatestUnitCount=" + state.aiLatestUnitCount +
                ", aiGoldGainObserved=" + state.aiGoldGainObserved +
                ", aiUnitGainObserved=" + state.aiUnitGainObserved +
                ", aiActivityObserved=" + state.aiActivityObserved +
                ", aiInitialBuildingCount=" + state.aiInitialBuildingCount +
                ", aiLatestBuildingCount=" + state.aiLatestBuildingCount +
                ", aiConstructionObserved=" + state.aiConstructionObserved +
                ", stabilitySurplusForced=" + state.stabilitySurplusForced +
                ", stabilitySurplusLoyaltyBefore=" + state.stabilitySurplusLoyaltyBefore.ToString("0.00") +
                ", stabilitySurplusLoyaltyLatest=" + state.stabilitySurplusLoyaltyLatest.ToString("0.00") +
                ", stabilitySurplusObserved=" + state.stabilitySurplusObserved + ".";

            return ProbeResult.Pass(
                "Bootstrap runtime smoke validation passed for " + BootstrapScenePath +
                " on map " + state.mapId +
                ". Counts: factions=" + factionCount +
                ", buildings=" + buildingCount +
                ", units=" + unitCount +
                ", resourceNodes=" + resourceNodeCount +
                ", controlPoints=" + controlPointCount +
                ", settlements=" + settlementCount + ". " + diagnostics);
        }

        private static int CountControlledUnits(EntityManager entityManager, string factionId, bool selectOnlyCombatUnits)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var unitTypes = query.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            int count = 0;
            for (int i = 0; i < factions.Length; i++)
            {
                if (health[i].Current <= 0f || !factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                if (selectOnlyCombatUnits && unitTypes[i].Role == UnitRole.Worker)
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private static int CountControlledBuildingsOfType(EntityManager entityManager, string factionId, string buildingTypeId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            int count = 0;
            for (int i = 0; i < factions.Length; i++)
            {
                if (health[i].Current <= 0f ||
                    !factions[i].FactionId.Equals(factionId) ||
                    !buildingTypes[i].TypeId.Equals(buildingTypeId))
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private static ProbeResult? ProbeStabilitySurplusResponse(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (state.stabilitySurplusObserved)
            {
                return null;
            }

            if (!state.stabilitySurplusForced)
            {
                if (!commandSurface.TryDebugGetFactionLoyalty(
                    commandSurface.ControlledFactionId,
                    out float loyaltyBefore,
                    out _,
                    out _))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: could not read loyalty before stability-surplus cycle. " +
                        diagnostics);
                }

                if (!commandSurface.TryDebugForceStabilitySurplusCycle(
                    commandSurface.ControlledFactionId,
                    out float _,
                    out float foodRatioAfter,
                    out float waterRatioAfter))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: could not force stability-surplus cycle for " +
                        Quote(commandSurface.ControlledFactionId) + ". " + diagnostics);
                }

                state.stabilitySurplusForced = true;
                state.stabilitySurplusLoyaltyBefore = loyaltyBefore;
                state.stabilitySurplusLoyaltyLatest = loyaltyBefore;
                state.stabilitySurplusLoyaltyExpectedMinimumAfter = loyaltyBefore + 0.5f;
                state.stabilitySurplusFoodRatioAfter = foodRatioAfter;
                state.stabilitySurplusWaterRatioAfter = waterRatioAfter;
                state.stabilitySurplusForcedUtcTicks = DateTime.UtcNow.Ticks;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Stability-surplus cycle forced for controlled faction. loyaltyBefore=" +
                    loyaltyBefore.ToString("0.00") +
                    ", foodRatioAfter=" + foodRatioAfter.ToString("0.00") +
                    ", waterRatioAfter=" + waterRatioAfter.ToString("0.00") + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugGetFactionLoyalty(
                commandSurface.ControlledFactionId,
                out float currentLoyalty,
                out _,
                out _))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: could not read loyalty during stability-surplus observation. " +
                    diagnostics);
            }

            state.stabilitySurplusLoyaltyLatest = currentLoyalty;

            if (currentLoyalty >= state.stabilitySurplusLoyaltyExpectedMinimumAfter)
            {
                state.stabilitySurplusObserved = true;
                SaveState(state);
                return null;
            }

            double elapsedSeconds = ElapsedSeconds(state.stabilitySurplusForcedUtcTicks);
            if (elapsedSeconds >= state.stabilitySurplusTimeoutSeconds)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: stability-surplus loyalty restoration did not apply within " +
                    state.stabilitySurplusTimeoutSeconds.ToString("0.0") + "s. loyaltyBefore=" +
                    state.stabilitySurplusLoyaltyBefore.ToString("0.00") +
                    ", currentLoyalty=" + currentLoyalty.ToString("0.00") +
                    ", expectedMinimum=" + state.stabilitySurplusLoyaltyExpectedMinimumAfter.ToString("0.00") + ". " +
                    diagnostics);
            }

            return ProbeResult.NotReady(
                "Waiting for stability-surplus loyalty restoration. loyaltyBefore=" +
                state.stabilitySurplusLoyaltyBefore.ToString("0.00") +
                ", currentLoyalty=" + currentLoyalty.ToString("0.00") +
                ", expectedMinimum=" + state.stabilitySurplusLoyaltyExpectedMinimumAfter.ToString("0.00") +
                ", elapsedSeconds=" + elapsedSeconds.ToString("0.0") + "/" +
                state.stabilitySurplusTimeoutSeconds.ToString("0.0") + ". " + diagnostics);
        }

        private static ProbeResult? ProbeAIConstructionActivity(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (state.aiConstructionObserved)
            {
                return null;
            }

            string factionId = string.IsNullOrWhiteSpace(state.aiFactionId) ? "enemy" : state.aiFactionId;

            if (!state.aiConstructionBaselineSampled)
            {
                int initialBuildingCount = commandSurface.CountFactionBuildings(factionId);
                state.aiConstructionBaselineSampled = true;
                state.aiInitialBuildingCount = initialBuildingCount;
                state.aiLatestBuildingCount = initialBuildingCount;
                state.aiConstructionBaselineUtcTicks = DateTime.UtcNow.Ticks;
                SaveState(state);
                return ProbeResult.NotReady(
                    "AI construction baseline captured: initialBuildingCount=" + initialBuildingCount +
                    " for faction " + Quote(factionId) + ". " + diagnostics);
            }

            int currentBuildingCount = commandSurface.CountFactionBuildings(factionId);
            state.aiLatestBuildingCount = currentBuildingCount;
            int gain = currentBuildingCount - state.aiInitialBuildingCount;
            int requiredGain = Mathf.Max(1, state.aiMinimumBuildingGain);

            if (gain >= requiredGain)
            {
                state.aiConstructionObserved = true;
                SaveState(state);
                return null;
            }

            double elapsedSeconds = ElapsedSeconds(state.aiConstructionBaselineUtcTicks);
            if (elapsedSeconds >= state.aiConstructionTimeoutSeconds)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: AI construction activity did not reach threshold within " +
                    state.aiConstructionTimeoutSeconds.ToString("0.0") + "s. faction=" + Quote(factionId) +
                    ", initialBuildingCount=" + state.aiInitialBuildingCount + ", currentBuildingCount=" + currentBuildingCount +
                    ", gain=" + gain + "/" + requiredGain + ". " + diagnostics);
            }

            return ProbeResult.NotReady(
                "Waiting for AI construction on faction " + Quote(factionId) +
                ": buildingGain=" + gain + "/" + requiredGain +
                ", elapsedSeconds=" + elapsedSeconds.ToString("0.0") + "/" +
                state.aiConstructionTimeoutSeconds.ToString("0.0") + ". " + diagnostics);
        }

        private static ProbeResult? ProbeAIEconomyActivity(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (state.aiActivityObserved)
            {
                return null;
            }

            string factionId = string.IsNullOrWhiteSpace(state.aiFactionId) ? "enemy" : state.aiFactionId;

            if (!state.aiBaselineSampled)
            {
                if (!commandSurface.TryDebugGetFactionStockpile(
                    factionId,
                    out float initialGold,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _,
                    out _))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: could not sample AI faction " +
                        Quote(factionId) + " stockpile before AI activity proof. " + diagnostics);
                }

                if (!commandSurface.TryDebugEnableAIForFaction(factionId, enabled: true))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: could not enable AI controller for faction " +
                        Quote(factionId) + ". " + diagnostics);
                }

                int initialUnitCount = commandSurface.CountFactionUnits(factionId);
                int initialBuildingCount = commandSurface.CountFactionBuildings(factionId);
                state.aiBaselineSampled = true;
                state.aiInitialGold = initialGold;
                state.aiInitialUnitCount = initialUnitCount;
                state.aiLatestGold = initialGold;
                state.aiLatestUnitCount = initialUnitCount;
                state.aiBaselineUtcTicks = DateTime.UtcNow.Ticks;
                state.aiConstructionBaselineSampled = true;
                state.aiInitialBuildingCount = initialBuildingCount;
                state.aiLatestBuildingCount = initialBuildingCount;
                state.aiConstructionBaselineUtcTicks = DateTime.UtcNow.Ticks;
                SaveState(state);
                return ProbeResult.NotReady(
                    "AI faction " + Quote(factionId) + " baseline captured: initialGold=" +
                    initialGold.ToString("0.0") + ", initialUnitCount=" + initialUnitCount + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugGetFactionStockpile(
                factionId,
                out float currentGold,
                out _,
                out _,
                out _,
                out _,
                out _,
                out _))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: could not read AI faction " +
                    Quote(factionId) + " stockpile during AI activity proof. " + diagnostics);
            }

            int currentUnitCount = commandSurface.CountFactionUnits(factionId);
            state.aiLatestGold = currentGold;
            state.aiLatestUnitCount = currentUnitCount;

            float goldGain = currentGold - state.aiInitialGold;
            int unitGain = currentUnitCount - state.aiInitialUnitCount;
            float requiredGoldGain = Mathf.Max(0.5f, state.aiMinimumGoldDelta);
            int requiredUnitGain = Mathf.Max(1, state.aiMinimumUnitDelta);

            if (goldGain >= requiredGoldGain)
            {
                state.aiGoldGainObserved = true;
            }
            if (unitGain >= requiredUnitGain)
            {
                state.aiUnitGainObserved = true;
            }

            if (state.aiGoldGainObserved || state.aiUnitGainObserved)
            {
                state.aiActivityObserved = true;
                SaveState(state);
                return null;
            }

            double elapsedSeconds = ElapsedSeconds(state.aiBaselineUtcTicks);
            if (elapsedSeconds >= state.aiActivityTimeoutSeconds)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: AI economic activity did not reach thresholds within " +
                    state.aiActivityTimeoutSeconds.ToString("0.0") + "s. faction=" + Quote(factionId) +
                    ", initialGold=" + state.aiInitialGold.ToString("0.00") + ", currentGold=" + currentGold.ToString("0.00") +
                    ", goldGain=" + goldGain.ToString("0.00") + "/" + requiredGoldGain.ToString("0.00") +
                    ", initialUnitCount=" + state.aiInitialUnitCount + ", currentUnitCount=" + currentUnitCount +
                    ", unitGain=" + unitGain + "/" + requiredUnitGain +
                    ", aiGoldGainObserved=" + state.aiGoldGainObserved +
                    ", aiUnitGainObserved=" + state.aiUnitGainObserved + ". " + diagnostics);
            }

            return ProbeResult.NotReady(
                "Waiting for AI economy on faction " + Quote(factionId) +
                ": goldGain=" + goldGain.ToString("0.00") + "/" + requiredGoldGain.ToString("0.00") +
                ", unitGain=" + unitGain + "/" + requiredUnitGain +
                ", elapsedSeconds=" + elapsedSeconds.ToString("0.0") + "/" +
                state.aiActivityTimeoutSeconds.ToString("0.0") + ". " + diagnostics);
        }

        private static ProbeResult? ProbeCapPressureResponse(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (state.capPressureObserved)
            {
                return null;
            }

            if (!state.capPressureForced)
            {
                if (!commandSurface.TryDebugForceCapPressureCycle(
                    commandSurface.ControlledFactionId,
                    out int totalAfterSpike,
                    out int capAfterSpike,
                    out float loyaltyBeforeCycle))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: command shell could not force cap-pressure cycle for " +
                        Quote(commandSurface.ControlledFactionId) + ". " + diagnostics);
                }

                state.capPressureForced = true;
                state.capPressureTotalAfterSpike = totalAfterSpike;
                state.capPressureCapAfterSpike = capAfterSpike;
                state.capPressureLoyaltyBeforeCycle = loyaltyBeforeCycle;
                state.capPressureLatestLoyalty = loyaltyBeforeCycle;
                state.capPressureExpectedMaximumAfterCycle = loyaltyBeforeCycle - 0.5f;
                state.capPressureForcedUtcTicks = DateTime.UtcNow.Ticks;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Cap-pressure cycle forced. populationTotal=" + totalAfterSpike +
                    "/" + capAfterSpike + ", loyaltyBeforeCycle=" + loyaltyBeforeCycle.ToString("0.00") + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugGetFactionLoyalty(
                commandSurface.ControlledFactionId,
                out float currentLoyalty,
                out _,
                out _))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: could not read loyalty after forced cap-pressure cycle. " +
                    diagnostics);
            }

            state.capPressureLatestLoyalty = currentLoyalty;

            if (currentLoyalty <= state.capPressureExpectedMaximumAfterCycle)
            {
                state.capPressureObserved = true;
                SaveState(state);
                return null;
            }

            double elapsedSeconds = ElapsedSeconds(state.capPressureForcedUtcTicks);
            if (elapsedSeconds >= state.capPressureTimeoutSeconds)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: cap-pressure loyalty delta did not apply within " +
                    state.capPressureTimeoutSeconds.ToString("0.0") + "s. loyaltyBeforeCycle=" +
                    state.capPressureLoyaltyBeforeCycle.ToString("0.00") + ", currentLoyalty=" + currentLoyalty.ToString("0.00") +
                    ", expectedMaximumAfterCycle=" + state.capPressureExpectedMaximumAfterCycle.ToString("0.00") + ". " +
                    diagnostics);
            }

            return ProbeResult.NotReady(
                "Waiting for CapPressureResponseSystem to reduce loyalty. loyaltyBeforeCycle=" +
                state.capPressureLoyaltyBeforeCycle.ToString("0.00") + ", currentLoyalty=" + currentLoyalty.ToString("0.00") +
                ", expectedMaximumAfterCycle=" + state.capPressureExpectedMaximumAfterCycle.ToString("0.00") +
                ", elapsedSeconds=" + elapsedSeconds.ToString("0.0") + "/" +
                state.capPressureTimeoutSeconds.ToString("0.0") + ". " + diagnostics);
        }

        private static ProbeResult? ProbeStarvationResponse(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (state.starvationObserved)
            {
                return null;
            }

            if (!state.starvationForced)
            {
                if (!commandSurface.TryDebugGetFactionLoyalty(
                    commandSurface.ControlledFactionId,
                    out float previousLoyalty,
                    out _,
                    out _))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: command shell could not read loyalty before forced starvation cycle for " +
                        Quote(commandSurface.ControlledFactionId) + ". " + diagnostics);
                }

                if (!commandSurface.TryDebugForceStarvationCycle(
                    commandSurface.ControlledFactionId,
                    state.starvationIncludeWaterCrisis,
                    out int previousTotal,
                    out int expectedTotal))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: command shell could not force starvation cycle for " +
                        Quote(commandSurface.ControlledFactionId) + ". " + diagnostics);
                }

                state.starvationForced = true;
                state.starvationPreviousPopulationTotal = previousTotal;
                state.starvationExpectedPopulationTotal = expectedTotal;
                state.starvationLatestPopulationTotal = previousTotal;
                state.starvationForcedUtcTicks = DateTime.UtcNow.Ticks;
                state.loyaltyPreviousValue = previousLoyalty;
                state.loyaltyLatestValue = previousLoyalty;
                state.loyaltyExpectedMaximumAfterCycle =
                    previousLoyalty - Mathf.Max(0f, state.loyaltyMinimumDeltaMagnitude);
                SaveState(state);
                return ProbeResult.NotReady(
                    "Starvation cycle forced for controlled faction. previousPopulationTotal=" +
                    previousTotal + ", expectedPopulationTotal=" + expectedTotal +
                    ", previousLoyalty=" + previousLoyalty.ToString("0.00") + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugGetFactionPopulation(
                commandSurface.ControlledFactionId,
                out int currentTotal,
                out _,
                out _))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: could not read population after forced starvation cycle. " +
                    diagnostics);
            }

            state.starvationLatestPopulationTotal = currentTotal;

            if (currentTotal <= state.starvationExpectedPopulationTotal)
            {
                if (!commandSurface.TryDebugGetFactionLoyalty(
                    commandSurface.ControlledFactionId,
                    out float currentLoyalty,
                    out _,
                    out _))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: could not read loyalty after starvation population decline. " +
                        diagnostics);
                }

                state.loyaltyLatestValue = currentLoyalty;

                if (currentLoyalty > state.loyaltyExpectedMaximumAfterCycle)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: starvation loyalty delta below minimum magnitude. previousLoyalty=" +
                        state.loyaltyPreviousValue.ToString("0.00") + ", currentLoyalty=" + currentLoyalty.ToString("0.00") +
                        ", expectedMaximumAfterCycle=" + state.loyaltyExpectedMaximumAfterCycle.ToString("0.00") +
                        ", minimumDeltaMagnitude=" + state.loyaltyMinimumDeltaMagnitude.ToString("0.00") + ". " + diagnostics);
                }

                state.starvationObserved = true;
                state.loyaltyDeclineObserved = true;
                SaveState(state);
                return null;
            }

            double elapsedSeconds = ElapsedSeconds(state.starvationForcedUtcTicks);
            if (elapsedSeconds >= state.starvationTimeoutSeconds)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: starvation response did not reduce population within " +
                    state.starvationTimeoutSeconds.ToString("0.0") + "s. previousPopulationTotal=" +
                    state.starvationPreviousPopulationTotal + ", expectedPopulationTotal=" +
                    state.starvationExpectedPopulationTotal + ", currentTotal=" + currentTotal +
                    ". " + diagnostics);
            }

            return ProbeResult.NotReady(
                "Waiting for StarvationResponseSystem to reduce population. previousPopulationTotal=" +
                state.starvationPreviousPopulationTotal + ", expectedPopulationTotal=" +
                state.starvationExpectedPopulationTotal + ", currentTotal=" + currentTotal +
                ", elapsedSeconds=" + elapsedSeconds.ToString("0.0") + "/" +
                state.starvationTimeoutSeconds.ToString("0.0") + ". " + diagnostics);
        }

        private static ProbeResult? ProbeResourceTrickleBaseline(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (state.trickleBaselineSampled)
            {
                return null;
            }

            if (!commandSurface.TryDebugGetFactionStockpile(
                commandSurface.ControlledFactionId,
                out _,
                out _,
                out _,
                out _,
                out float initialFood,
                out float initialWater,
                out _))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: could not sample controlled faction food and water baseline before trickle proof. " +
                    diagnostics);
            }

            state.trickleBaselineSampled = true;
            state.trickleInitialFood = initialFood;
            state.trickleInitialWater = initialWater;
            state.trickleSampledUtcTicks = DateTime.UtcNow.Ticks;
            SaveState(state);
            return null;
        }

        private static ProbeResult? ProbeResourceTrickleGain(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (!state.trickleBaselineSampled)
            {
                return null;
            }

            if (state.trickleGainObserved)
            {
                return null;
            }

            if (!commandSurface.TryDebugGetFactionStockpile(
                commandSurface.ControlledFactionId,
                out _,
                out _,
                out _,
                out _,
                out float currentFood,
                out float currentWater,
                out _))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: could not sample controlled faction food and water during trickle verification. " +
                    diagnostics);
            }

            state.trickleLatestFood = currentFood;
            state.trickleLatestWater = currentWater;

            float foodGain = currentFood - state.trickleInitialFood;
            float waterGain = currentWater - state.trickleInitialWater;
            float requiredFoodGain = Mathf.Max(0.25f, state.trickleMinimumFoodGain);
            float requiredWaterGain = Mathf.Max(0.25f, state.trickleMinimumWaterGain);

            if (foodGain >= requiredFoodGain && waterGain >= requiredWaterGain)
            {
                state.trickleGainObserved = true;
                SaveState(state);
                return null;
            }

            double elapsedSeconds = ElapsedSeconds(state.trickleSampledUtcTicks);
            if (elapsedSeconds >= state.trickleTimeoutSeconds)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: building resource trickle did not raise food and water above minimum gains within " +
                    state.trickleTimeoutSeconds.ToString("0.0") + "s. initialFood=" +
                    state.trickleInitialFood.ToString("0.00") + ", currentFood=" + currentFood.ToString("0.00") +
                    ", foodGain=" + foodGain.ToString("0.00") + ", requiredFoodGain=" + requiredFoodGain.ToString("0.00") +
                    ", initialWater=" + state.trickleInitialWater.ToString("0.00") + ", currentWater=" + currentWater.ToString("0.00") +
                    ", waterGain=" + waterGain.ToString("0.00") + ", requiredWaterGain=" + requiredWaterGain.ToString("0.00") +
                    ". " + diagnostics);
            }

            return ProbeResult.NotReady(
                "Waiting for building resource trickle to raise food and water. initialFood=" +
                state.trickleInitialFood.ToString("0.00") + ", currentFood=" + currentFood.ToString("0.00") +
                ", foodGain=" + foodGain.ToString("0.00") + "/" + requiredFoodGain.ToString("0.00") +
                ", initialWater=" + state.trickleInitialWater.ToString("0.00") + ", currentWater=" + currentWater.ToString("0.00") +
                ", waterGain=" + waterGain.ToString("0.00") + "/" + requiredWaterGain.ToString("0.00") +
                ", elapsedSeconds=" + elapsedSeconds.ToString("0.0") + "/" + state.trickleTimeoutSeconds.ToString("0.0") +
                ". " + diagnostics);
        }

        private static ProbeResult? ProbeWorkerGatherCycle(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            string diagnostics)
        {
            if (!state.gatherAssigned)
            {
                if (!commandSurface.TryDebugGetFactionStockpile(
                    commandSurface.ControlledFactionId,
                    out float initialGold,
                    out _, out _, out _, out _, out _, out _))
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: could not read controlled faction stockpile before gather assignment. " +
                        diagnostics);
                }

                int assigned = commandSurface.TryDebugAssignSelectedWorkersToGatherResource(state.gatherResourceId);
                if (assigned <= 0)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: command shell could not assign any controlled workers to gather '" +
                        state.gatherResourceId + "'. " + diagnostics);
                }

                state.gatherAssigned = true;
                state.gatherAssignedWorkerCount = assigned;
                state.gatherInitialFactionGold = initialGold;
                state.gatherLatestFactionGold = initialGold;
                state.gatherAssignedUtcTicks = DateTime.UtcNow.Ticks;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Worker gather cycle assigned: workerCount=" + assigned +
                    ", resource=" + Quote(state.gatherResourceId) +
                    ", initialFactionGold=" + initialGold.ToString("0.0") + ". " + diagnostics);
            }

            if (!commandSurface.TryDebugGetFactionStockpile(
                commandSurface.ControlledFactionId,
                out float currentGold,
                out _, out _, out _, out _, out _, out _))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: could not read controlled faction stockpile during gather wait. " +
                    diagnostics);
            }

            state.gatherLatestFactionGold = currentGold;

            if (state.gatherDepositObserved)
            {
                return null;
            }

            int activeGatherers = commandSurface.GetControlledWorkersWithActiveGatherCount();
            float depositDelta = currentGold - state.gatherInitialFactionGold;
            float requiredDeposit = Mathf.Max(0.5f, state.gatherMinimumDepositAmount);

            if (depositDelta >= requiredDeposit)
            {
                state.gatherDepositObserved = true;
                SaveState(state);
                return null;
            }

            double elapsedSeconds = ElapsedSeconds(state.gatherAssignedUtcTicks);
            if (elapsedSeconds >= state.gatherCycleTimeoutSeconds)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: worker gather cycle did not deposit expected resources within " +
                    state.gatherCycleTimeoutSeconds.ToString("0.0") + "s. initialGold=" +
                    state.gatherInitialFactionGold.ToString("0.0") + ", currentGold=" +
                    currentGold.ToString("0.0") + ", delta=" + depositDelta.ToString("0.00") +
                    ", activeGatherers=" + activeGatherers + ", requiredDeposit=" +
                    requiredDeposit.ToString("0.0") + ". " + diagnostics);
            }

            return ProbeResult.NotReady(
                "Waiting for worker gather deposit. initialGold=" +
                state.gatherInitialFactionGold.ToString("0.0") + ", currentGold=" +
                currentGold.ToString("0.0") + ", delta=" + depositDelta.ToString("0.00") +
                ", activeGatherers=" + activeGatherers + ", requiredDeposit=" +
                requiredDeposit.ToString("0.0") + ", elapsedSeconds=" +
                elapsedSeconds.ToString("0.0") + "/" + state.gatherCycleTimeoutSeconds.ToString("0.0") + ". " +
                diagnostics);
        }

        private static ProbeResult? ProbeProductionProgress(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            int unitCount,
            string diagnostics)
        {
            if (!state.productionQueued)
            {
                return null;
            }

            if (unitCount >= state.expectedUnitCountAfterProduction)
            {
                return null;
            }

            if (state.productionProgressAdvancementVerified)
            {
                return null;
            }

            if (!commandSurface.TryDebugSelectControlledBuilding(state.productionBuildingTypeId))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not re-select controlled '" +
                    state.productionBuildingTypeId + "' for production progress observation. " + diagnostics);
            }

            if (!commandSurface.TryDebugGetSelectedProductionProgress(
                out float currentRatio,
                out float currentRemaining,
                out float currentTotal,
                out string observedUnitId,
                out string observedBuildingTypeId))
            {
                return ProbeResult.NotReady(
                    "Waiting for active production queue to become observable on " +
                    state.productionBuildingTypeId + ". " + diagnostics);
            }

            if (!string.Equals(observedBuildingTypeId, state.productionBuildingTypeId, StringComparison.Ordinal))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: observed production progress building type '" +
                    observedBuildingTypeId + "' did not match expected '" +
                    state.productionBuildingTypeId + "'. " + diagnostics);
            }

            if (!string.Equals(observedUnitId, state.productionUnitId, StringComparison.Ordinal))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: observed active production unit '" +
                    observedUnitId + "' did not match expected '" +
                    state.productionUnitId + "'. " + diagnostics);
            }

            if (currentTotal <= 0f)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: production total seconds must be positive but was " +
                    currentTotal.ToString("0.000") + ". " + diagnostics);
            }

            if (currentRatio < 0f || currentRatio > 1f)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: production progress ratio out of range. Observed " +
                    currentRatio.ToString("0.000") + ". " + diagnostics);
            }

            state.productionProgressLatestRatio = currentRatio;
            state.productionProgressLatestRemainingSeconds = currentRemaining;

            if (!state.productionProgressInitialSampled)
            {
                if (currentRatio >= 1f)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: production progress already complete at first sample, advancement cannot be proven. " +
                        diagnostics);
                }

                state.productionProgressInitialSampled = true;
                state.productionProgressInitialRatio = currentRatio;
                state.productionProgressInitialRemainingSeconds = currentRemaining;
                state.productionProgressInitialTotalSeconds = currentTotal;
                state.productionProgressInitialUtcTicks = DateTime.UtcNow.Ticks;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Production progress initial sample captured: ratio=" + currentRatio.ToString("0.000") +
                    ", remainingSeconds=" + currentRemaining.ToString("0.000") +
                    ", totalSeconds=" + currentTotal.ToString("0.000") + ". " + diagnostics);
            }

            double elapsedSeconds = ElapsedSeconds(state.productionProgressInitialUtcTicks);
            float advancement = currentRatio - state.productionProgressInitialRatio;
            float requiredAdvancement = Mathf.Max(0f, state.productionProgressMinimumAdvancementRatio);

            if (advancement < requiredAdvancement &&
                elapsedSeconds < state.productionProgressAdvancementWaitSeconds)
            {
                return ProbeResult.NotReady(
                    "Waiting for production progress to advance beyond initial sample. " + diagnostics +
                    " initialRatio=" + state.productionProgressInitialRatio.ToString("0.000") +
                    ", latestRatio=" + currentRatio.ToString("0.000") +
                    ", minimumAdvancement=" + requiredAdvancement.ToString("0.000") + ".");
            }

            if (advancement <= 0f)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: production progress did not advance between samples. " +
                    "initialRatio=" + state.productionProgressInitialRatio.ToString("0.000") +
                    ", latestRatio=" + currentRatio.ToString("0.000") +
                    ", elapsedSeconds=" + elapsedSeconds.ToString("0.000") + ". " + diagnostics);
            }

            state.productionProgressAdvancementVerified = true;
            SaveState(state);
            return null;
        }

        private static ProbeResult? ProbeConstructionProgress(
            BloodlinesDebugCommandSurface commandSurface,
            RuntimeSmokeState state,
            int controlledConstructionSiteCount,
            string diagnostics)
        {
            if (controlledConstructionSiteCount <= 0)
            {
                return null;
            }

            if (!commandSurface.TryDebugSelectControlledConstructionSite(state.constructionBuildingTypeId))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: command shell could not select an active " +
                    state.constructionBuildingTypeId + " construction site for progress observation. " + diagnostics);
            }

            if (!commandSurface.TryDebugGetSelectedConstructionProgress(
                out float currentRatio,
                out float currentRemaining,
                out float currentTotal,
                out string observedBuildingTypeId))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: construction progress readout was unavailable after selecting an active " +
                    state.constructionBuildingTypeId + " construction site. " + diagnostics);
            }

            if (!string.Equals(observedBuildingTypeId, state.constructionBuildingTypeId, StringComparison.Ordinal))
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: observed construction progress building type '" +
                    observedBuildingTypeId + "' did not match expected '" +
                    state.constructionBuildingTypeId + "'. " + diagnostics);
            }

            if (currentTotal <= 0f)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: construction total seconds must be positive but was " +
                    currentTotal.ToString("0.000") + ". " + diagnostics);
            }

            if (currentRatio < 0f || currentRatio > 1f)
            {
                return ProbeResult.Fail(
                    "Bootstrap runtime smoke validation failed: construction progress ratio out of range. Observed " +
                    currentRatio.ToString("0.000") + ". " + diagnostics);
            }

            state.constructionProgressLatestRatio = currentRatio;
            state.constructionProgressLatestRemainingSeconds = currentRemaining;

            if (!state.constructionProgressInitialSampled)
            {
                if (currentRatio >= 1f)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: construction progress already complete at first sample, advancement cannot be proven. " +
                        diagnostics);
                }

                state.constructionProgressInitialSampled = true;
                state.constructionProgressInitialRatio = currentRatio;
                state.constructionProgressInitialRemainingSeconds = currentRemaining;
                state.constructionProgressInitialTotalSeconds = currentTotal;
                state.constructionProgressInitialUtcTicks = DateTime.UtcNow.Ticks;
                SaveState(state);
                return ProbeResult.NotReady(
                    "Construction progress initial sample captured: ratio=" + currentRatio.ToString("0.000") +
                    ", remainingSeconds=" + currentRemaining.ToString("0.000") +
                    ", totalSeconds=" + currentTotal.ToString("0.000") + ". " + diagnostics);
            }

            if (!state.constructionProgressAdvancementVerified)
            {
                double elapsedSeconds = ElapsedSeconds(state.constructionProgressInitialUtcTicks);
                float advancement = currentRatio - state.constructionProgressInitialRatio;
                float requiredAdvancement = Mathf.Max(0f, state.constructionProgressMinimumAdvancementRatio);

                if (advancement < requiredAdvancement &&
                    elapsedSeconds < state.constructionProgressAdvancementWaitSeconds)
                {
                    return ProbeResult.NotReady(
                        "Waiting for construction progress to advance beyond initial sample. " + diagnostics +
                        " initialRatio=" + state.constructionProgressInitialRatio.ToString("0.000") +
                        ", latestRatio=" + currentRatio.ToString("0.000") +
                        ", minimumAdvancement=" + requiredAdvancement.ToString("0.000") + ".");
                }

                if (advancement <= 0f)
                {
                    return ProbeResult.Fail(
                        "Bootstrap runtime smoke validation failed: construction progress did not advance between samples. " +
                        "initialRatio=" + state.constructionProgressInitialRatio.ToString("0.000") +
                        ", latestRatio=" + currentRatio.ToString("0.000") +
                        ", elapsedSeconds=" + elapsedSeconds.ToString("0.000") + ". " + diagnostics);
                }

                state.constructionProgressAdvancementVerified = true;
                SaveState(state);
            }

            return null;
        }

        private static int CountControlledConstructionSitesOfType(EntityManager entityManager, string factionId, string buildingTypeId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            int count = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f ||
                    !factions[i].FactionId.Equals(factionId) ||
                    !buildingTypes[i].TypeId.Equals(buildingTypeId) ||
                    !entityManager.HasComponent<ConstructionStateComponent>(entities[i]))
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private static bool TryGetControlledQueuedProduction(
            EntityManager entityManager,
            string factionId,
            string buildingTypeId,
            int queueIndex,
            out ProductionQueueItemElement queueItem)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f ||
                    !factions[i].FactionId.Equals(factionId) ||
                    !buildingTypes[i].TypeId.Equals(buildingTypeId) ||
                    !entityManager.HasBuffer<ProductionQueueItemElement>(entities[i]))
                {
                    continue;
                }

                var queue = entityManager.GetBuffer<ProductionQueueItemElement>(entities[i]);
                if (queueIndex < 0 || queueIndex >= queue.Length)
                {
                    continue;
                }

                queueItem = queue[queueIndex];
                return true;
            }

            queueItem = default;
            return false;
        }

        private static bool TryGetControlledFactionRuntimeSnapshot(
            EntityManager entityManager,
            string factionId,
            out ResourceStockpileComponent resources,
            out PopulationComponent population)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<ResourceStockpileComponent>(),
                ComponentType.ReadOnly<PopulationComponent>());

            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);
            using var populations = query.ToComponentDataArray<PopulationComponent>(Allocator.Temp);

            for (int i = 0; i < factions.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                resources = stockpiles[i];
                population = populations[i];
                return true;
            }

            resources = default;
            population = default;
            return false;
        }

        private static bool MatchesQueuedRefund(
            in ResourceStockpileComponent before,
            in ResourceStockpileComponent after,
            in ProductionQueueItemElement queueItem)
        {
            return ApproximatelyEqual(after.Gold - before.Gold, queueItem.GoldCost) &&
                   ApproximatelyEqual(after.Food - before.Food, queueItem.FoodCost) &&
                   ApproximatelyEqual(after.Water - before.Water, queueItem.WaterCost) &&
                   ApproximatelyEqual(after.Wood - before.Wood, queueItem.WoodCost) &&
                   ApproximatelyEqual(after.Stone - before.Stone, queueItem.StoneCost) &&
                   ApproximatelyEqual(after.Iron - before.Iron, queueItem.IronCost) &&
                   ApproximatelyEqual(after.Influence - before.Influence, queueItem.InfluenceCost);
        }

        private static bool MatchesQueuedPopulationRefund(
            in PopulationComponent before,
            in PopulationComponent after,
            in ProductionQueueItemElement queueItem)
        {
            return after.Total - before.Total == queueItem.BloodPrice &&
                   after.Available - before.Available == queueItem.PopulationCost + queueItem.BloodPrice;
        }

        private static bool ApproximatelyEqual(float actual, float expected)
        {
            return Math.Abs(actual - expected) <= 0.001f;
        }

        private static string DescribeQueuedItem(in ProductionQueueItemElement queueItem)
        {
            return "{unit=" + Quote(queueItem.UnitId.ToString()) +
                   ", gold=" + queueItem.GoldCost +
                   ", food=" + queueItem.FoodCost +
                   ", water=" + queueItem.WaterCost +
                   ", wood=" + queueItem.WoodCost +
                   ", stone=" + queueItem.StoneCost +
                   ", iron=" + queueItem.IronCost +
                   ", influence=" + queueItem.InfluenceCost +
                   ", pop=" + queueItem.PopulationCost +
                   ", blood=" + queueItem.BloodPrice + "}";
        }

        private static string DescribeResources(in ResourceStockpileComponent resources)
        {
            return "{gold=" + resources.Gold.ToString("0.###") +
                   ", food=" + resources.Food.ToString("0.###") +
                   ", water=" + resources.Water.ToString("0.###") +
                   ", wood=" + resources.Wood.ToString("0.###") +
                   ", stone=" + resources.Stone.ToString("0.###") +
                   ", iron=" + resources.Iron.ToString("0.###") +
                   ", influence=" + resources.Influence.ToString("0.###") + "}";
        }

        private static string DescribePopulation(in PopulationComponent population)
        {
            return "{total=" + population.Total +
                   ", available=" + population.Available +
                   ", cap=" + population.Cap + "}";
        }

        private static void CompleteValidation(RuntimeSmokeState state, bool success, string message)
        {
            state.resultCode = success ? 0 : 1;
            state.phase = "await_exit";
            SaveState(state);

            if (success)
            {
                UnityEngine.Debug.Log(message);
            }
            else
            {
                UnityEngine.Debug.LogError(message);
            }

            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            if (state.batchMode)
            {
                int exitCode = state.resultCode;
                ClearPersistentState();
                EditorApplication.Exit(exitCode);
                return;
            }

            ClearPersistentState();
        }

        private static void CompleteImmediately(bool batchMode, bool success, string message)
        {
            if (success)
            {
                UnityEngine.Debug.Log(message);
            }
            else
            {
                UnityEngine.Debug.LogError(message);
            }

            ClearPersistentState();

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static void SaveState(RuntimeSmokeState state)
        {
            SessionState.SetString(StateKey, JsonUtility.ToJson(state));
        }

        private static RuntimeSmokeState LoadState()
        {
            string json = SessionState.GetString(StateKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonUtility.FromJson<RuntimeSmokeState>(json);
        }

        private static void ClearPersistentState()
        {
            SessionState.SetBool(PendingKey, false);
            SessionState.EraseString(StateKey);
            SessionState.EraseString(StartupTimeoutUtcTicksKey);
        }

        private static bool HasTimedOut()
        {
            string timeoutTicksString = SessionState.GetString(StartupTimeoutUtcTicksKey, string.Empty);
            if (!long.TryParse(timeoutTicksString, out long timeoutTicks))
            {
                return false;
            }

            return DateTime.UtcNow.Ticks > timeoutTicks;
        }

        private static double ElapsedSeconds(long utcTicks)
        {
            if (utcTicks <= 0)
            {
                return 0d;
            }

            return (DateTime.UtcNow - new DateTime(utcTicks, DateTimeKind.Utc)).TotalSeconds;
        }

        private static double ParseWarmupSeconds()
        {
            return double.TryParse(WarmupSeconds, out double warmupSeconds)
                ? warmupSeconds
                : 1.5d;
        }

        private static bool ShouldEmitProgressLog(RuntimeSmokeState state, string diagnostics)
        {
            if (string.IsNullOrWhiteSpace(diagnostics))
            {
                return false;
            }

            if (!string.Equals(state.lastProbeDiagnostics, diagnostics, StringComparison.Ordinal))
            {
                return true;
            }

            return ElapsedSeconds(state.lastProbeLogUtcTicks) >= ProbeLogIntervalSeconds;
        }

        private static string Quote(string value)
        {
            return "'" + (value ?? string.Empty) + "'";
        }

        private static int SumBuildings(MapDefinition map)
        {
            int total = 0;
            foreach (var faction in map.factions ?? Array.Empty<FactionSeedData>())
            {
                total += faction.buildings?.Length ?? 0;
            }

            return total;
        }

        private static int SumUnits(MapDefinition map)
        {
            int total = 0;
            foreach (var faction in map.factions ?? Array.Empty<FactionSeedData>())
            {
                total += faction.units?.Length ?? 0;
            }

            return total;
        }

        private static int CalculateExpectedProxyMinimum(MapDefinition map)
        {
            return SumUnits(map) +
                   SumBuildings(map) +
                   (map.settlements?.Length ?? 0) +
                   (map.controlPoints?.Length ?? 0) +
                   (map.resourceNodes?.Length ?? 0);
        }

        [Serializable]
        private sealed class RuntimeSmokeState
        {
            public bool batchMode;
            public string phase;
            public string mapId;
            public long playModeEnteredUtcTicks;
            public int resultCode;
            public int expectedFactionCount;
            public int expectedBuildingCount;
            public int expectedUnitCount;
            public int expectedResourceNodeCount;
            public int expectedControlPointCount;
            public int expectedSettlementCount;
            public int expectedProxyMinimum;
            public bool expectDebugBridge;
            public long lastProbeLogUtcTicks;
            public string lastProbeDiagnostics;
            public bool productionCancelVerified;
            public bool productionQueued;
            public int expectedUnitCountAfterProduction;
            public int expectedControlledUnitCountAfterProduction;
            public string productionBuildingTypeId = "command_hall";
            public string productionUnitId = "villager";
            public bool productionProgressInitialSampled;
            public float productionProgressInitialRatio;
            public float productionProgressInitialRemainingSeconds;
            public float productionProgressInitialTotalSeconds;
            public long productionProgressInitialUtcTicks;
            public bool productionProgressAdvancementVerified;
            public float productionProgressLatestRatio;
            public float productionProgressLatestRemainingSeconds;
            public float productionProgressMinimumAdvancementRatio;
            public float productionProgressAdvancementWaitSeconds;
            public bool constructionPlaced;
            public int expectedBuildingCountAfterConstruction;
            public int expectedProxyMinimumAfterConstruction;
            public int expectedPopulationCapAfterConstruction;
            public int expectedConstructionBuildingTypeCountAfterConstruction;
            public string constructionBuildingTypeId = "dwelling";
            public int constructionPopulationCapBonus = 6;
            public bool constructionProgressInitialSampled;
            public float constructionProgressInitialRatio;
            public float constructionProgressInitialRemainingSeconds;
            public float constructionProgressInitialTotalSeconds;
            public long constructionProgressInitialUtcTicks;
            public bool constructionProgressAdvancementVerified;
            public float constructionProgressLatestRatio;
            public float constructionProgressLatestRemainingSeconds;
            public float constructionProgressMinimumAdvancementRatio;
            public float constructionProgressAdvancementWaitSeconds;
            public bool constructedProductionBuildingPlaced;
            public int expectedBuildingCountAfterConstructedProductionBuilding;
            public int expectedProxyMinimumAfterConstructedProductionBuilding;
            public int expectedConstructedProductionBuildingTypeCountAfterPlacement;
            public bool constructedProductionQueued;
            public int expectedUnitCountAfterConstructedProduction;
            public int expectedControlledUnitCountAfterConstructedProduction;
            public int expectedProxyMinimumAfterConstructedProduction;
            public string constructedProductionBuildingTypeId = "barracks";
            public string constructedProductionUnitId = "militia";
            public string gatherResourceId = "gold";
            public bool gatherAssigned;
            public int gatherAssignedWorkerCount;
            public float gatherInitialFactionGold;
            public float gatherMinimumDepositAmount;
            public bool gatherDepositObserved;
            public float gatherLatestFactionGold;
            public long gatherAssignedUtcTicks;
            public float gatherCycleTimeoutSeconds;
            public bool trickleBaselineSampled;
            public float trickleInitialFood;
            public float trickleInitialWater;
            public long trickleSampledUtcTicks;
            public float trickleLatestFood;
            public float trickleLatestWater;
            public float trickleMinimumFoodGain;
            public float trickleMinimumWaterGain;
            public float trickleTimeoutSeconds;
            public bool trickleGainObserved;
            public bool starvationForced;
            public bool starvationIncludeWaterCrisis;
            public int starvationPreviousPopulationTotal;
            public int starvationExpectedPopulationTotal;
            public int starvationLatestPopulationTotal;
            public long starvationForcedUtcTicks;
            public float starvationTimeoutSeconds;
            public bool starvationObserved;
            public float loyaltyPreviousValue;
            public float loyaltyExpectedMaximumAfterCycle;
            public float loyaltyLatestValue;
            public float loyaltyMinimumDeltaMagnitude;
            public bool loyaltyDeclineObserved;
            public bool capPressureForced;
            public int capPressureTotalAfterSpike;
            public int capPressureCapAfterSpike;
            public float capPressureLoyaltyBeforeCycle;
            public float capPressureExpectedMaximumAfterCycle;
            public float capPressureLatestLoyalty;
            public long capPressureForcedUtcTicks;
            public float capPressureTimeoutSeconds;
            public bool capPressureObserved;
            public string aiFactionId = "enemy";
            public bool aiBaselineSampled;
            public float aiInitialGold;
            public int aiInitialUnitCount;
            public float aiLatestGold;
            public int aiLatestUnitCount;
            public long aiBaselineUtcTicks;
            public float aiMinimumGoldDelta;
            public int aiMinimumUnitDelta;
            public float aiActivityTimeoutSeconds;
            public bool aiActivityObserved;
            public bool aiGoldGainObserved;
            public bool aiUnitGainObserved;
            public bool aiConstructionBaselineSampled;
            public int aiInitialBuildingCount;
            public int aiLatestBuildingCount;
            public long aiConstructionBaselineUtcTicks;
            public int aiMinimumBuildingGain;
            public float aiConstructionTimeoutSeconds;
            public bool aiConstructionObserved;
            public bool stabilitySurplusForced;
            public float stabilitySurplusLoyaltyBefore;
            public float stabilitySurplusLoyaltyLatest;
            public float stabilitySurplusLoyaltyExpectedMinimumAfter;
            public float stabilitySurplusFoodRatioAfter;
            public float stabilitySurplusWaterRatioAfter;
            public long stabilitySurplusForcedUtcTicks;
            public float stabilitySurplusTimeoutSeconds;
            public bool stabilitySurplusObserved;
        }

        private readonly struct ProbeResult
        {
            public readonly bool ready;
            public readonly bool success;
            public readonly string message;
            public readonly string diagnostics;

            private ProbeResult(bool ready, bool success, string message, string diagnostics)
            {
                this.ready = ready;
                this.success = success;
                this.message = message;
                this.diagnostics = diagnostics;
            }

            public static ProbeResult NotReady(string diagnostics)
            {
                return new ProbeResult(false, false, string.Empty, diagnostics);
            }

            public static ProbeResult Pass(string message)
            {
                return new ProbeResult(true, true, message, message);
            }

            public static ProbeResult Fail(string message)
            {
                return new ProbeResult(true, false, message, message);
            }
        }
    }
}
#endif
