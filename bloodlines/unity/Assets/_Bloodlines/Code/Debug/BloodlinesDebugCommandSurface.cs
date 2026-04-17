using System;
using System.Collections.Generic;
using System.Text;
using BattlefieldCameraController = Bloodlines.Camera.BloodlinesBattlefieldCameraController;
using Bloodlines.Authoring;
using Bloodlines.Components;
using Bloodlines.DataDefinitions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityCamera = UnityEngine.Camera;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug-only first interaction shell for the Unity battlefield lane.
    /// Supports basic selection, early control groups, framing, and right-click move so
    /// the first ECS bootstrap shell can exercise credible RTS command rhythm.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BloodlinesDebugCommandSurface : MonoBehaviour
    {
        private const float ConstructionStartHealthFraction = 0.18f;
        private static readonly string[] SupportedConstructionBuildingOrder =
        {
            "dwelling",
            "farm",
            "well",
            "lumber_camp",
            "mine_works",
            "quarry",
            "barracks",
            "stable",
            "siege_workshop",
            "supply_camp",
            "wayshrine",
            "wall_segment",
            "watch_tower",
            "gatehouse",
            "keep_tier_1",
        };

        [SerializeField] private UnityCamera controlledCamera = null;
        [SerializeField] private BattlefieldCameraController battlefieldCameraController = null;
        [SerializeField] private string controlledFactionId = "player";
        [SerializeField] private float singleSelectRadius = 3.25f;
        [SerializeField] private bool allowBuildingSelection = true;
        [SerializeField] private float buildingSelectRadius = 4.5f;
        [SerializeField] private float dragSelectThresholdPixels = 14f;
        [SerializeField] private float commandStoppingDistance = 0.6f;
        [SerializeField] private bool spreadMoveCommands = true;
        [SerializeField] private float formationSpacing = 1.8f;
        [SerializeField] private int formationMaxColumns = 6;
        [SerializeField] private bool showSelectionRectangle = true;
        [SerializeField] private Color selectionRectangleFill = new Color(0.23f, 0.56f, 0.96f, 0.14f);
        [SerializeField] private Color selectionRectangleBorder = new Color(0.48f, 0.76f, 1f, 0.9f);
        [SerializeField] private bool selectOnlyCombatUnits = false;
        [SerializeField] private bool enableControlGroups = true;
        [SerializeField] private int minimumControlGroupSlot = 2;
        [SerializeField] private int maximumControlGroupSlot = 5;
        [SerializeField] private bool allowShiftAdditiveGroupRecall = true;
        [SerializeField] private bool enableSelectionFraming = true;
        [SerializeField] private bool showBattlefieldHud = true;
        [SerializeField] private Vector2 hudMargin = new Vector2(16f, 16f);
        [SerializeField] private float hudWidth = 360f;
        [SerializeField] private Color hudBackground = new Color(0.08f, 0.1f, 0.12f, 0.82f);
        [SerializeField] private Color hudHeaderBackground = new Color(0.18f, 0.36f, 0.6f, 0.94f);
        [SerializeField] private Color hudBorderColor = new Color(0.52f, 0.78f, 1f, 0.92f);
        [SerializeField] private bool showProductionPanel = true;
        [SerializeField] private float productionPanelSpacing = 12f;
        [SerializeField] private float productionButtonHeight = 42f;
        [SerializeField] private bool showConstructionPanel = true;
        [SerializeField] private float constructionPanelSpacing = 12f;
        [SerializeField] private float constructionButtonHeight = 42f;
        [SerializeField] private bool showConstructionProgressPanel = true;
        [SerializeField] private float constructionProgressPanelSpacing = 12f;
        [SerializeField] private Color constructionProgressFillColor = new Color(0.82f, 0.76f, 0.54f, 0.95f);
        [SerializeField] private Color constructionProgressTrackColor = new Color(0.12f, 0.14f, 0.17f, 0.78f);

        private readonly List<Entity> selectedEntities = new();
        private readonly Dictionary<int, List<Entity>> controlGroups = new();
        private readonly Dictionary<string, BuildingDefinition> buildingDefinitionsById = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, UnitDefinition> unitDefinitionsById = new(StringComparer.OrdinalIgnoreCase);
        private Vector2 pointerDownScreenPosition;
        private bool leftMouseHeld;
        private bool dragSelecting;
        private BloodlinesMapBootstrapAuthoring bootstrapAuthoring;
        private string productionFeedbackMessage = string.Empty;
        private float productionFeedbackExpiresAt;
        private string pendingConstructionBuildingId = string.Empty;
        private string constructionFeedbackMessage = string.Empty;
        private float constructionFeedbackExpiresAt;
        private GUIStyle hudTitleStyle;
        private GUIStyle hudBodyStyle;
        private GUIStyle hudButtonStyle;

        private void Awake()
        {
            if (controlledCamera == null)
            {
                controlledCamera = UnityCamera.main;
            }

            ResolveCameraController();
        }

        private void Update()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return;
            }

            if (controlledCamera == null)
            {
                controlledCamera = UnityCamera.main;
                if (controlledCamera == null)
                {
                    return;
                }
            }

            ResolveCameraController();

            var entityManager = world.EntityManager;
            PruneSelection(entityManager);
            PruneControlGroups(entityManager);

            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                if (HasPendingConstructionPlacement())
                {
                    pendingConstructionBuildingId = string.Empty;
                    SetConstructionFeedback("Construction placement cancelled.", success: false);
                }
                else
                {
                    ClearSelection(entityManager);
                }
            }

            if (keyboard != null && keyboard.digit1Key.wasPressedThisFrame)
            {
                SelectAllControlledUnits(entityManager);
            }

            if (keyboard != null)
            {
                HandleControlGroupHotkeys(entityManager, keyboard);

                if (enableSelectionFraming && keyboard.fKey.wasPressedThisFrame)
                {
                    FrameSelection(entityManager);
                }
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                pointerDownScreenPosition = mouse.position.ReadValue();
                leftMouseHeld = true;
                dragSelecting = false;
            }

            if (leftMouseHeld && mouse.leftButton.isPressed && !dragSelecting)
            {
                float threshold = math.max(2f, dragSelectThresholdPixels);
                if ((mouse.position.ReadValue() - pointerDownScreenPosition).sqrMagnitude >= threshold * threshold)
                {
                    dragSelecting = true;
                }
            }

            if (mouse.leftButton.wasReleasedThisFrame && leftMouseHeld)
            {
                bool additive = keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed);
                var releasePosition = mouse.position.ReadValue();

                if (dragSelecting)
                {
                    SelectUnitsInScreenRect(entityManager, pointerDownScreenPosition, releasePosition, additive);
                }
                else if (TryGetGroundPoint(releasePosition, out var selectionPoint))
                {
                    SelectNearestControlledEntity(entityManager, selectionPoint, additive);
                }

                leftMouseHeld = false;
                dragSelecting = false;
            }

            if (mouse.rightButton.wasPressedThisFrame && TryGetGroundPoint(mouse.position.ReadValue(), out var destination))
            {
                if (HasPendingConstructionPlacement())
                {
                    if (TryPlacePendingConstruction(entityManager, destination, out var constructionMessage))
                    {
                        SetConstructionFeedback(constructionMessage, success: true);
                    }
                    else
                    {
                        SetConstructionFeedback(constructionMessage, success: false);
                    }
                }
                else if (selectedEntities.Count > 0)
                {
                    IssueMoveCommand(entityManager, destination);
                }
            }
        }

        private void OnGUI()
        {
            if (showBattlefieldHud)
            {
                DrawBattlefieldHud();
            }

            if (TryGetEntityManager(out var entityManager))
            {
                if (showProductionPanel)
                {
                    DrawProductionPanel(entityManager);
                }

                if (showConstructionPanel)
                {
                    DrawConstructionPanel(entityManager);
                }

                if (showConstructionProgressPanel)
                {
                    DrawConstructionProgressPanel(entityManager);
                }
            }

            if (!showSelectionRectangle || !leftMouseHeld || !dragSelecting)
            {
                return;
            }

            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            Rect screenRect = GetSelectionRect(pointerDownScreenPosition, mouse.position.ReadValue());
            Rect guiRect = new Rect(
                screenRect.xMin,
                Screen.height - screenRect.yMax,
                screenRect.width,
                screenRect.height);

            var previousColor = GUI.color;
            GUI.color = selectionRectangleFill;
            GUI.DrawTexture(guiRect, Texture2D.whiteTexture);

            GUI.color = selectionRectangleBorder;
            DrawRectangleBorder(guiRect, 2f);
            GUI.color = previousColor;
        }

        private void DrawBattlefieldHud()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return;
            }

            var entityManager = world.EntityManager;
            var factionSnapshot = GetControlledFactionSnapshot(entityManager);
            var territorySnapshot = GetTerritorySnapshot(entityManager);
            var selectionSnapshot = GetSelectionSnapshot(entityManager);

            EnsureHudStyles();

            string hudText = BuildHudText(factionSnapshot, territorySnapshot, selectionSnapshot);
            float height = 188f;
            var panelRect = new Rect(hudMargin.x, hudMargin.y, hudWidth, height);
            var headerRect = new Rect(panelRect.x, panelRect.y, panelRect.width, 28f);
            var bodyRect = new Rect(panelRect.x + 12f, panelRect.y + 38f, panelRect.width - 24f, panelRect.height - 48f);

            var previousColor = GUI.color;
            GUI.color = hudBackground;
            GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
            GUI.color = hudHeaderBackground;
            GUI.DrawTexture(headerRect, Texture2D.whiteTexture);
            GUI.color = hudBorderColor;
            DrawRectangleBorder(panelRect, 2f);
            GUI.color = previousColor;

            GUI.Label(
                new Rect(headerRect.x + 10f, headerRect.y + 4f, headerRect.width - 20f, headerRect.height - 8f),
                "Bloodlines Battlefield Shell",
                hudTitleStyle);
            GUI.Label(bodyRect, hudText, hudBodyStyle);
        }

        private void DrawProductionPanel(EntityManager entityManager)
        {
            if (!TryGetSelectedProductionBuildingSnapshot(entityManager, out var buildingSnapshot))
            {
                return;
            }

            EnsureHudStyles();
            var options = BuildProductionOptions(entityManager, in buildingSnapshot);
            var queueEntries = BuildProductionQueueEntries(entityManager, buildingSnapshot.Entity);
            float panelY = hudMargin.y + 188f + productionPanelSpacing;
            float queueHeight = queueEntries.Count > 0 ? 30f + (queueEntries.Count * 30f) : 20f;
            float feedbackHeight = HasActiveProductionFeedback() ? 28f : 0f;
            float height = 68f + queueHeight + (options.Count * (productionButtonHeight + 6f)) + feedbackHeight;

            var panelRect = new Rect(hudMargin.x, panelY, hudWidth, height);
            var headerRect = new Rect(panelRect.x, panelRect.y, panelRect.width, 28f);
            var previousColor = GUI.color;

            GUI.color = hudBackground;
            GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
            GUI.color = hudHeaderBackground;
            GUI.DrawTexture(headerRect, Texture2D.whiteTexture);
            GUI.color = hudBorderColor;
            DrawRectangleBorder(panelRect, 2f);
            GUI.color = previousColor;

            GUI.Label(
                new Rect(headerRect.x + 10f, headerRect.y + 4f, headerRect.width - 20f, headerRect.height - 8f),
                buildingSnapshot.DisplayName + " Production",
                hudTitleStyle);

            float cursorY = panelRect.y + 38f;
            string queueText = buildingSnapshot.QueueCount > 0
                ? $"Queue: {buildingSnapshot.QueueCount} item(s)"
                : "Queue: empty";
            GUI.Label(new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, 22f), queueText, hudBodyStyle);
            cursorY += 24f;

            if (queueEntries.Count > 0)
            {
                for (int i = 0; i < queueEntries.Count; i++)
                {
                    var queueEntry = queueEntries[i];
                    var labelRect = new Rect(panelRect.x + 12f, cursorY, panelRect.width - 92f, 24f);
                    var cancelRect = new Rect(panelRect.x + panelRect.width - 72f, cursorY - 1f, 60f, 24f);

                    GUI.Label(labelRect, queueEntry.Detail, hudBodyStyle);
                    if (GUI.Button(cancelRect, "Cancel"))
                    {
                        if (TryCancelProduction(entityManager, buildingSnapshot.Entity, queueEntry.QueueIndex, out var cancelMessage))
                        {
                            SetProductionFeedback(cancelMessage, success: true);
                        }
                        else
                        {
                            SetProductionFeedback(cancelMessage, success: false);
                        }
                    }

                    cursorY += 30f;
                }
            }
            else
            {
                cursorY += queueHeight - 24f;
            }

            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                var buttonRect = new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, productionButtonHeight);
                bool previousEnabled = GUI.enabled;
                GUI.enabled = option.Enabled;

                if (GUI.Button(buttonRect, option.Label + "\n" + option.Detail, hudButtonStyle))
                {
                    if (TryQueueProduction(entityManager, buildingSnapshot.Entity, option.UnitId, out var queueMessage))
                    {
                        SetProductionFeedback(queueMessage, success: true);
                    }
                    else
                    {
                        SetProductionFeedback(queueMessage, success: false);
                    }
                }

                GUI.enabled = previousEnabled;
                cursorY += productionButtonHeight + 6f;
            }

            if (HasActiveProductionFeedback())
            {
                GUI.Label(
                    new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, 24f),
                    productionFeedbackMessage,
                    hudBodyStyle);
            }
        }

        private void DrawConstructionPanel(EntityManager entityManager)
        {
            if (!TryGetSelectedWorkerSnapshot(entityManager, out var workerSnapshot))
            {
                return;
            }

            EnsureHudStyles();
            var options = BuildConstructionOptions(entityManager, in workerSnapshot);
            float panelX = hudMargin.x + hudWidth + constructionPanelSpacing;
            float panelY = hudMargin.y;
            float feedbackHeight = HasActiveConstructionFeedback() ? 30f : 0f;
            float placementHeight = HasPendingConstructionPlacement() ? 44f : 20f;
            float height = 68f + placementHeight + (options.Count * (constructionButtonHeight + 6f)) + feedbackHeight;

            var panelRect = new Rect(panelX, panelY, hudWidth, height);
            var headerRect = new Rect(panelRect.x, panelRect.y, panelRect.width, 28f);
            var previousColor = GUI.color;

            GUI.color = hudBackground;
            GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
            GUI.color = hudHeaderBackground;
            GUI.DrawTexture(headerRect, Texture2D.whiteTexture);
            GUI.color = hudBorderColor;
            DrawRectangleBorder(panelRect, 2f);
            GUI.color = previousColor;

            GUI.Label(
                new Rect(headerRect.x + 10f, headerRect.y + 4f, headerRect.width - 20f, headerRect.height - 8f),
                workerSnapshot.DisplayName + " Construction",
                hudTitleStyle);

            float cursorY = panelRect.y + 38f;
            string statusText = HasPendingConstructionPlacement()
                ? "Placement: RMB to place " + FormatIdentifier(pendingConstructionBuildingId) + "  |  Esc cancels"
                : $"Workers selected: {workerSnapshot.WorkerCount}";
            GUI.Label(new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, 30f), statusText, hudBodyStyle);
            cursorY += placementHeight;

            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                var buttonRect = new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, constructionButtonHeight);
                bool previousEnabled = GUI.enabled;
                GUI.enabled = option.Enabled;

                if (GUI.Button(buttonRect, option.Label + "\n" + option.Detail, hudButtonStyle))
                {
                    if (TryBeginConstructionPlacement(entityManager, option.BuildingId, out var constructionMessage))
                    {
                        SetConstructionFeedback(constructionMessage, success: true);
                    }
                    else
                    {
                        SetConstructionFeedback(constructionMessage, success: false);
                    }
                }

                GUI.enabled = previousEnabled;
                cursorY += constructionButtonHeight + 6f;
            }

            if (HasActiveConstructionFeedback())
            {
                GUI.Label(
                    new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, 24f),
                    constructionFeedbackMessage,
                    hudBodyStyle);
            }
        }

        private void DrawConstructionProgressPanel(EntityManager entityManager)
        {
            if (!TryGetSelectedConstructionSiteSnapshot(entityManager, out var siteSnapshot))
            {
                return;
            }

            EnsureHudStyles();
            float panelY = hudMargin.y + 188f + constructionProgressPanelSpacing;
            float height = 120f;
            var panelRect = new Rect(hudMargin.x, panelY, hudWidth, height);
            var headerRect = new Rect(panelRect.x, panelRect.y, panelRect.width, 28f);
            var previousColor = GUI.color;

            GUI.color = hudBackground;
            GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
            GUI.color = hudHeaderBackground;
            GUI.DrawTexture(headerRect, Texture2D.whiteTexture);
            GUI.color = hudBorderColor;
            DrawRectangleBorder(panelRect, 2f);
            GUI.color = previousColor;

            GUI.Label(
                new Rect(headerRect.x + 10f, headerRect.y + 4f, headerRect.width - 20f, headerRect.height - 8f),
                siteSnapshot.DisplayName + " Construction",
                hudTitleStyle);

            float cursorY = panelRect.y + 38f;
            int percent = Mathf.Clamp(Mathf.RoundToInt(siteSnapshot.ProgressRatio * 100f), 0, 100);
            string statusText =
                "Progress: " + percent + "%  |  " +
                siteSnapshot.RemainingSeconds.ToString("0.0") + "s remaining of " +
                siteSnapshot.TotalSeconds.ToString("0.0") + "s";
            GUI.Label(
                new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, 22f),
                statusText,
                hudBodyStyle);
            cursorY += 26f;

            var trackRect = new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, 14f);
            var fillRect = new Rect(trackRect.x, trackRect.y, trackRect.width * Mathf.Clamp01(siteSnapshot.ProgressRatio), trackRect.height);
            previousColor = GUI.color;
            GUI.color = constructionProgressTrackColor;
            GUI.DrawTexture(trackRect, Texture2D.whiteTexture);
            GUI.color = constructionProgressFillColor;
            GUI.DrawTexture(fillRect, Texture2D.whiteTexture);
            GUI.color = hudBorderColor;
            DrawRectangleBorder(trackRect, 1f);
            GUI.color = previousColor;
            cursorY += 22f;

            int healthPercent = siteSnapshot.MaxHealth > 0f
                ? Mathf.Clamp(Mathf.RoundToInt((siteSnapshot.Health / siteSnapshot.MaxHealth) * 100f), 0, 100)
                : 0;
            string healthText =
                "Integrity: " + healthPercent + "%  |  " +
                siteSnapshot.Health.ToString("0") + "/" + siteSnapshot.MaxHealth.ToString("0") + " HP";
            GUI.Label(
                new Rect(panelRect.x + 12f, cursorY, panelRect.width - 24f, 22f),
                healthText,
                hudBodyStyle);
        }

        private bool TryGetSelectedConstructionSiteSnapshot(EntityManager entityManager, out SelectedConstructionSiteSnapshot snapshot)
        {
            snapshot = default;
            if (selectedEntities.Count != 1)
            {
                return false;
            }

            var entity = selectedEntities[0];
            if (!IsSelectableAliveBuilding(entityManager, entity))
            {
                return false;
            }

            if (!entityManager.HasComponent<ConstructionStateComponent>(entity))
            {
                return false;
            }

            var construction = entityManager.GetComponentData<ConstructionStateComponent>(entity);
            var buildingType = entityManager.GetComponentData<BuildingTypeComponent>(entity);
            var faction = entityManager.GetComponentData<FactionComponent>(entity);
            var health = entityManager.GetComponentData<HealthComponent>(entity);
            string typeId = buildingType.TypeId.ToString();
            string displayName = TryResolveBuildingDefinition(typeId, out var definition) &&
                                 !string.IsNullOrWhiteSpace(definition.displayName)
                ? definition.displayName
                : typeId;

            float totalSeconds = math.max(0.1f, construction.TotalSeconds);
            float remainingSeconds = math.max(0f, construction.RemainingSeconds);
            float progressRatio = math.saturate(1f - (remainingSeconds / totalSeconds));

            snapshot = new SelectedConstructionSiteSnapshot
            {
                Entity = entity,
                FactionId = faction.FactionId.ToString(),
                TypeId = typeId,
                DisplayName = displayName,
                RemainingSeconds = remainingSeconds,
                TotalSeconds = totalSeconds,
                ProgressRatio = progressRatio,
                Health = health.Current,
                MaxHealth = health.Max,
            };
            return true;
        }

        private List<ProductionCommandOption> BuildProductionOptions(EntityManager entityManager, in SelectedProductionBuildingSnapshot buildingSnapshot)
        {
            var options = new List<ProductionCommandOption>();
            if (!TryGetFactionRuntimeSnapshot(entityManager, buildingSnapshot.FactionId, out var factionSnapshot))
            {
                return options;
            }

            foreach (var unitId in buildingSnapshot.Definition.trainableUnits ?? Array.Empty<string>())
            {
                if (!TryResolveUnitDefinition(unitId, out var unitDefinition))
                {
                    continue;
                }

                options.Add(EvaluateProductionOption(in factionSnapshot, unitDefinition));
            }

            return options;
        }

        private List<ConstructionCommandOption> BuildConstructionOptions(EntityManager entityManager, in SelectedWorkerSnapshot workerSnapshot)
        {
            var options = new List<ConstructionCommandOption>();
            if (!TryGetFactionRuntimeSnapshot(entityManager, workerSnapshot.FactionId, out var factionSnapshot))
            {
                return options;
            }

            foreach (var buildingId in SupportedConstructionBuildingOrder)
            {
                if (!TryResolveBuildingDefinition(buildingId, out var buildingDefinition))
                {
                    continue;
                }

                options.Add(EvaluateConstructionOption(in factionSnapshot, buildingDefinition));
            }

            return options;
        }

        private List<ProductionQueueEntryView> BuildProductionQueueEntries(EntityManager entityManager, Entity buildingEntity)
        {
            var entries = new List<ProductionQueueEntryView>();
            if (!entityManager.Exists(buildingEntity) || !entityManager.HasBuffer<ProductionQueueItemElement>(buildingEntity))
            {
                return entries;
            }

            var queue = entityManager.GetBuffer<ProductionQueueItemElement>(buildingEntity);
            for (int i = 0; i < queue.Length; i++)
            {
                var queueItem = queue[i];
                var detailBuilder = new StringBuilder(128);
                detailBuilder.Append(i == 0 ? "Active" : "Queued")
                    .Append(' ')
                    .Append(i + 1)
                    .Append(": ")
                    .Append(queueItem.DisplayName)
                    .Append(" (")
                    .Append(queueItem.RemainingSeconds.ToString("0.0"))
                    .Append('/')
                    .Append(queueItem.TotalSeconds.ToString("0.0"))
                    .Append("s)");

                detailBuilder.Append("  |  ")
                    .Append(FormatQueuedCostSummary(in queueItem))
                    .Append("  |  pop ")
                    .Append(queueItem.PopulationCost);

                if (queueItem.BloodPrice > 0)
                {
                    detailBuilder.Append("  |  blood ").Append(queueItem.BloodPrice);
                }

                if (queueItem.BloodLoadDelta > 0f)
                {
                    detailBuilder.Append("  |  load +").Append(queueItem.BloodLoadDelta.ToString("0.#"));
                }

                entries.Add(new ProductionQueueEntryView
                {
                    QueueIndex = i,
                    Detail = detailBuilder.ToString(),
                });
            }

            return entries;
        }

        private ProductionCommandOption EvaluateProductionOption(in FactionRuntimeSnapshot factionSnapshot, UnitDefinition unitDefinition)
        {
            string unitId = unitDefinition.id ?? string.Empty;
            string displayName = string.IsNullOrWhiteSpace(unitDefinition.displayName) ? unitId : unitDefinition.displayName;
            float durationSeconds = GetProductionDurationSeconds(unitDefinition);
            int bloodPrice = GetIronmarkBloodPrice(in factionSnapshot, unitDefinition);
            float bloodLoadDelta = GetBloodProductionLoadDelta(in factionSnapshot, unitDefinition);
            bool enabled = true;
            string reason = string.Empty;

            if (!unitDefinition.prototypeEnabled)
            {
                enabled = false;
                reason = $"{displayName} is not enabled in the prototype.";
            }
            else if (!string.IsNullOrWhiteSpace(unitDefinition.house) &&
                     !factionSnapshot.HouseId.Equals(unitDefinition.house, StringComparison.OrdinalIgnoreCase))
            {
                enabled = false;
                reason = $"{displayName} is reserved to {FormatIdentifier(unitDefinition.house)}.";
            }
            else if (string.Equals(unitDefinition.movementDomain, "water", StringComparison.OrdinalIgnoreCase))
            {
                enabled = false;
                reason = "Naval production is not yet wired to Unity water spawning.";
            }
            else if (!string.IsNullOrWhiteSpace(unitDefinition.faithId))
            {
                if (!TryResolveCovenantId(unitDefinition.faithId, out var requiredFaith) ||
                    factionSnapshot.FaithState.SelectedFaith != requiredFaith)
                {
                    enabled = false;
                    reason = $"Requires {FormatIdentifier(unitDefinition.faithId)} covenant commitment.";
                }
                else if (!string.IsNullOrWhiteSpace(unitDefinition.doctrinePath) &&
                         (!TryResolveDoctrinePath(unitDefinition.doctrinePath, out var requiredPath) ||
                          factionSnapshot.FaithState.DoctrinePath != requiredPath))
                {
                    enabled = false;
                    reason = $"Requires {FormatIdentifier(unitDefinition.doctrinePath)} doctrine.";
                }
                else if (unitDefinition.stage >= 5)
                {
                    enabled = false;
                    reason = "Apex covenant training remains locked until Covenant Test state exists in Unity.";
                }
            }

            if (enabled && !CanAffordCost(factionSnapshot.Resources, unitDefinition.cost))
            {
                enabled = false;
                reason = "Not enough resources.";
            }

            int requiredAvailablePopulation = unitDefinition.populationCost + bloodPrice;
            if (enabled && factionSnapshot.Population.Available < requiredAvailablePopulation)
            {
                enabled = false;
                reason = bloodPrice > 0
                    ? "Ironmark blood levy requires more living population."
                    : "No available population.";
            }

            var detailBuilder = new StringBuilder(160);
            detailBuilder.Append(FormatCostSummary(unitDefinition.cost))
                .Append("  |  pop ")
                .Append(unitDefinition.populationCost)
                .Append("  |  ")
                .Append(durationSeconds.ToString("0.#"))
                .Append('s');

            if (bloodPrice > 0)
            {
                detailBuilder.Append("  |  blood ").Append(bloodPrice);
            }

            if (bloodLoadDelta > 0f)
            {
                detailBuilder.Append("  |  load +").Append(bloodLoadDelta.ToString("0.#"));
            }

            if (!enabled && !string.IsNullOrWhiteSpace(reason))
            {
                detailBuilder.Append("  |  ").Append(reason);
            }

            return new ProductionCommandOption
            {
                UnitId = unitId,
                Label = "Train " + displayName,
                Detail = detailBuilder.ToString(),
                Enabled = enabled,
            };
        }

        private ConstructionCommandOption EvaluateConstructionOption(in FactionRuntimeSnapshot factionSnapshot, BuildingDefinition buildingDefinition)
        {
            string buildingId = buildingDefinition.id ?? string.Empty;
            string displayName = string.IsNullOrWhiteSpace(buildingDefinition.displayName) ? buildingId : buildingDefinition.displayName;
            bool enabled = true;
            string reason = string.Empty;

            if (!buildingDefinition.prototypeEnabled)
            {
                enabled = false;
                reason = $"{displayName} is not enabled in the prototype.";
            }
            else if (!IsSupportedConstructionBuilding(buildingDefinition))
            {
                enabled = false;
                reason = "This building is not yet wired into the Unity construction shell.";
            }
            else if (!CanAffordCost(factionSnapshot.Resources, buildingDefinition.cost))
            {
                enabled = false;
                reason = "Not enough resources.";
            }

            var detailBuilder = new StringBuilder(160);
            detailBuilder.Append(FormatCostSummary(buildingDefinition.cost))
                .Append("  |  ")
                .Append(math.max(0.1f, buildingDefinition.buildTime).ToString("0.#"))
                .Append('s');

            if (buildingDefinition.populationCapBonus > 0)
            {
                detailBuilder.Append("  |  cap +").Append(buildingDefinition.populationCapBonus);
            }

            int trainableCount = buildingDefinition.trainableUnits?.Length ?? 0;
            if (trainableCount > 0)
            {
                detailBuilder.Append("  |  roster ").Append(trainableCount);
            }

            if (!enabled && !string.IsNullOrWhiteSpace(reason))
            {
                detailBuilder.Append("  |  ").Append(reason);
            }

            return new ConstructionCommandOption
            {
                BuildingId = buildingId,
                Label = "Build " + displayName,
                Detail = detailBuilder.ToString(),
                Enabled = enabled,
            };
        }

        private bool TryQueueProductionFromControlledBuilding(EntityManager entityManager, string buildingTypeId, string unitId, out string message)
        {
            message = "Production building was not found.";
            if (!TrySelectControlledBuilding(entityManager, buildingTypeId, clearSelectionFirst: true))
            {
                return false;
            }

            if (!TryGetSelectedProductionBuildingSnapshot(entityManager, out var buildingSnapshot))
            {
                message = "Selected building does not support production.";
                return false;
            }

            return TryQueueProduction(entityManager, buildingSnapshot.Entity, unitId, out message);
        }

        private bool TryCancelProductionFromControlledBuilding(EntityManager entityManager, string buildingTypeId, int queueIndex, out string message)
        {
            message = "Production building was not found.";
            if (!TrySelectControlledBuilding(entityManager, buildingTypeId, clearSelectionFirst: true))
            {
                return false;
            }

            if (!TryGetSelectedProductionBuildingSnapshot(entityManager, out var buildingSnapshot))
            {
                message = "Selected building does not support production.";
                return false;
            }

            return TryCancelProduction(entityManager, buildingSnapshot.Entity, queueIndex, out message);
        }

        private bool TryQueueProduction(EntityManager entityManager, Entity buildingEntity, string unitId, out string message)
        {
            message = "Production request failed.";
            if (!entityManager.Exists(buildingEntity) || !entityManager.HasComponent<BuildingTypeComponent>(buildingEntity))
            {
                message = "Building is unavailable.";
                return false;
            }

            var buildingType = entityManager.GetComponentData<BuildingTypeComponent>(buildingEntity);
            if (!TryResolveBuildingDefinition(buildingType.TypeId.ToString(), out var buildingDefinition))
            {
                message = "Building definition is unavailable.";
                return false;
            }

            if (Array.IndexOf(buildingDefinition.trainableUnits ?? Array.Empty<string>(), unitId) < 0)
            {
                message = "Unit cannot be trained here.";
                return false;
            }

            if (!TryResolveUnitDefinition(unitId, out var unitDefinition))
            {
                message = "Unit definition is unavailable.";
                return false;
            }

            var factionId = entityManager.GetComponentData<FactionComponent>(buildingEntity).FactionId.ToString();
            if (!TryGetFactionRuntimeSnapshot(entityManager, factionId, out var factionSnapshot))
            {
                message = "Faction runtime state is unavailable.";
                return false;
            }

            var option = EvaluateProductionOption(in factionSnapshot, unitDefinition);
            if (!option.Enabled)
            {
                message = option.Detail;
                return false;
            }

            var resources = factionSnapshot.Resources;
            SpendCost(ref resources, unitDefinition.cost);
            entityManager.SetComponentData(factionSnapshot.Entity, resources);

            var population = factionSnapshot.Population;
            int bloodPrice = GetIronmarkBloodPrice(in factionSnapshot, unitDefinition);
            population.Total = math.max(0, population.Total - bloodPrice);
            population.Available = math.max(0, population.Available - unitDefinition.populationCost - bloodPrice);
            entityManager.SetComponentData(factionSnapshot.Entity, population);

            if (!entityManager.HasBuffer<ProductionQueueItemElement>(buildingEntity))
            {
                entityManager.AddBuffer<ProductionQueueItemElement>(buildingEntity);
            }

            float productionDurationSeconds = GetProductionDurationSeconds(unitDefinition);
            float bloodLoadDelta = GetBloodProductionLoadDelta(in factionSnapshot, unitDefinition);
            var queue = entityManager.GetBuffer<ProductionQueueItemElement>(buildingEntity);
            queue.Add(new ProductionQueueItemElement
            {
                UnitId = unitDefinition.id ?? string.Empty,
                DisplayName = unitDefinition.displayName ?? unitDefinition.id ?? string.Empty,
                RemainingSeconds = productionDurationSeconds,
                TotalSeconds = productionDurationSeconds,
                PopulationCost = unitDefinition.populationCost,
                BloodPrice = bloodPrice,
                BloodLoadDelta = bloodLoadDelta,
                MaxHealth = unitDefinition.health,
                MaxSpeed = unitDefinition.speed,
                Role = ResolveUnitRole(unitDefinition.role),
                SiegeClass = ResolveSiegeClass(unitDefinition.siegeClass),
                Stage = unitDefinition.stage,
                GoldCost = unitDefinition.cost?.gold ?? 0,
                FoodCost = unitDefinition.cost?.food ?? 0,
                WaterCost = unitDefinition.cost?.water ?? 0,
                WoodCost = unitDefinition.cost?.wood ?? 0,
                StoneCost = unitDefinition.cost?.stone ?? 0,
                IronCost = unitDefinition.cost?.iron ?? 0,
                InfluenceCost = unitDefinition.cost?.influence ?? 0,
            });

            message = (unitDefinition.displayName ?? unitDefinition.id ?? unitId) + " queued.";
            return true;
        }

        private bool TryCancelProduction(EntityManager entityManager, Entity buildingEntity, int queueIndex, out string message)
        {
            message = "Production cancellation failed.";
            if (!entityManager.Exists(buildingEntity) || !entityManager.HasComponent<BuildingTypeComponent>(buildingEntity))
            {
                message = "Building is unavailable.";
                return false;
            }

            if (!entityManager.HasBuffer<ProductionQueueItemElement>(buildingEntity))
            {
                message = "Production queue is unavailable.";
                return false;
            }

            var queue = entityManager.GetBuffer<ProductionQueueItemElement>(buildingEntity);
            if (queue.Length == 0)
            {
                message = "Production queue is already empty.";
                return false;
            }

            int normalizedQueueIndex = queueIndex < 0 ? queue.Length - 1 : queueIndex;
            if (normalizedQueueIndex < 0 || normalizedQueueIndex >= queue.Length)
            {
                message = "Queue index is out of range.";
                return false;
            }

            var factionId = entityManager.GetComponentData<FactionComponent>(buildingEntity).FactionId.ToString();
            if (!TryGetFactionRuntimeSnapshot(entityManager, factionId, out var factionSnapshot))
            {
                message = "Faction runtime state is unavailable.";
                return false;
            }

            var queueItem = queue[normalizedQueueIndex];
            var resources = factionSnapshot.Resources;
            RefundQueuedCost(ref resources, in queueItem);
            entityManager.SetComponentData(factionSnapshot.Entity, resources);

            var population = factionSnapshot.Population;
            RestoreQueuedPopulation(ref population, in queueItem);
            entityManager.SetComponentData(factionSnapshot.Entity, population);

            queue.RemoveAt(normalizedQueueIndex);
            message = queueItem.DisplayName + " canceled and refunded.";
            return true;
        }

        private bool TryBeginConstructionPlacement(EntityManager entityManager, string buildingId, out string message)
        {
            message = "Construction request failed.";
            if (!TryGetSelectedWorkerSnapshot(entityManager, out var workerSnapshot))
            {
                message = "Select at least one controlled worker.";
                return false;
            }

            if (!TryResolveBuildingDefinition(buildingId, out var buildingDefinition))
            {
                message = "Building definition is unavailable.";
                return false;
            }

            if (!TryGetFactionRuntimeSnapshot(entityManager, workerSnapshot.FactionId, out var factionSnapshot))
            {
                message = "Faction runtime state is unavailable.";
                return false;
            }

            var option = EvaluateConstructionOption(in factionSnapshot, buildingDefinition);
            if (!option.Enabled)
            {
                message = option.Detail;
                return false;
            }

            pendingConstructionBuildingId = buildingId;
            message = "Right-click to place " + (buildingDefinition.displayName ?? buildingDefinition.id ?? buildingId) + ".";
            return true;
        }

        private bool TryPlacePendingConstruction(EntityManager entityManager, float3 buildPosition, out string message)
        {
            message = "Construction placement is not active.";
            if (!HasPendingConstructionPlacement())
            {
                return false;
            }

            string buildingId = pendingConstructionBuildingId;
            if (!TryGetSelectedWorkerSnapshot(entityManager, out var workerSnapshot))
            {
                message = "Select at least one controlled worker before placing a building.";
                return false;
            }

            bool placed = TryPlaceConstruction(entityManager, workerSnapshot.Entity, buildingId, buildPosition, out message);
            if (placed)
            {
                pendingConstructionBuildingId = string.Empty;
            }

            return placed;
        }

        private bool TryPlaceConstruction(EntityManager entityManager, Entity workerEntity, string buildingId, float3 buildPosition, out string message)
        {
            message = "Construction placement failed.";
            if (!entityManager.Exists(workerEntity) ||
                !entityManager.HasComponent<UnitTypeComponent>(workerEntity) ||
                !entityManager.HasComponent<FactionComponent>(workerEntity) ||
                !entityManager.HasComponent<PositionComponent>(workerEntity))
            {
                message = "Selected worker is unavailable.";
                return false;
            }

            var workerType = entityManager.GetComponentData<UnitTypeComponent>(workerEntity);
            if (workerType.Role != UnitRole.Worker)
            {
                message = "Construction requires a worker.";
                return false;
            }

            if (!TryResolveBuildingDefinition(buildingId, out var buildingDefinition))
            {
                message = "Building definition is unavailable.";
                return false;
            }

            string factionId = entityManager.GetComponentData<FactionComponent>(workerEntity).FactionId.ToString();
            if (!TryGetFactionRuntimeSnapshot(entityManager, factionId, out var factionSnapshot))
            {
                message = "Faction runtime state is unavailable.";
                return false;
            }

            var option = EvaluateConstructionOption(in factionSnapshot, buildingDefinition);
            if (!option.Enabled)
            {
                message = option.Detail;
                return false;
            }

            if (!CanPlaceConstructionAt(entityManager, buildingDefinition, buildPosition, out var placementReason))
            {
                message = placementReason;
                return false;
            }

            var resources = factionSnapshot.Resources;
            SpendCost(ref resources, buildingDefinition.cost);
            entityManager.SetComponentData(factionSnapshot.Entity, resources);

            float maxHealth = math.max(1f, buildingDefinition.health);
            float startingHealth = math.max(1f, maxHealth * ConstructionStartHealthFraction);

            var buildingEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(buildingEntity, new FactionComponent { FactionId = factionSnapshot.FactionId });
            entityManager.AddComponentData(buildingEntity, new PositionComponent { Value = buildPosition });
            entityManager.AddComponentData(buildingEntity, new Unity.Transforms.LocalTransform
            {
                Position = buildPosition,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(buildingEntity, new HealthComponent
            {
                Current = startingHealth,
                Max = maxHealth,
            });
            entityManager.AddComponentData(buildingEntity, CreateBuildingTypeComponent(buildingDefinition));
            entityManager.AddComponentData(buildingEntity, new ConstructionStateComponent
            {
                RemainingSeconds = math.max(0.1f, buildingDefinition.buildTime),
                TotalSeconds = math.max(0.1f, buildingDefinition.buildTime),
                StartingHealth = startingHealth,
            });

            if ((buildingDefinition.trainableUnits?.Length ?? 0) > 0)
            {
                entityManager.AddComponentData(buildingEntity, new ProductionFacilityComponent
                {
                    SpawnSequence = 0,
                });
                entityManager.AddBuffer<ProductionQueueItemElement>(buildingEntity);
            }

            var trickle = buildingDefinition.resourceTrickle;
            if (trickle != null &&
                (trickle.gold > 0f || trickle.food > 0f || trickle.water > 0f ||
                 trickle.wood > 0f || trickle.stone > 0f || trickle.iron > 0f ||
                 trickle.influence > 0f))
            {
                entityManager.AddComponentData(buildingEntity, new ResourceTrickleBuildingComponent
                {
                    GoldPerSecond = trickle.gold,
                    FoodPerSecond = trickle.food,
                    WaterPerSecond = trickle.water,
                    WoodPerSecond = trickle.wood,
                    StonePerSecond = trickle.stone,
                    IronPerSecond = trickle.iron,
                    InfluencePerSecond = trickle.influence,
                });
            }

            message = (buildingDefinition.displayName ?? buildingDefinition.id ?? buildingId) + " construction started.";
            return true;
        }

        private bool TryDebugStartConstructionNearSelectedWorker(EntityManager entityManager, string buildingId, out string message)
        {
            message = "Construction request failed.";
            if (!TryResolveBuildingDefinition(buildingId, out var buildingDefinition))
            {
                message = "Building definition is unavailable.";
                return false;
            }

            if (!TryGetSelectedWorkerSnapshot(entityManager, out var workerSnapshot))
            {
                message = "Controlled worker was not found.";
                return false;
            }

            if (!TryFindConstructionPlacementNearWorker(entityManager, workerSnapshot.Entity, buildingDefinition, out var buildPosition, out message))
            {
                return false;
            }

            return TryPlaceConstruction(entityManager, workerSnapshot.Entity, buildingId, buildPosition, out message);
        }

        private bool TryFindConstructionPlacementNearWorker(
            EntityManager entityManager,
            Entity workerEntity,
            BuildingDefinition buildingDefinition,
            out float3 buildPosition,
            out string message)
        {
            message = "No clear construction site was found near the selected worker.";
            buildPosition = default;

            if (!entityManager.Exists(workerEntity) || !entityManager.HasComponent<PositionComponent>(workerEntity))
            {
                message = "Selected worker is unavailable.";
                return false;
            }

            float3 workerPosition = entityManager.GetComponentData<PositionComponent>(workerEntity).Value;
            float baseRadius = math.max(5.5f, GetFootprintRadius(buildingDefinition) + 2.5f);

            const int ringCount = 6;
            const int angularSteps = 16;

            for (int ring = 0; ring < ringCount; ring++)
            {
                float radius = baseRadius + (ring * 3.5f);
                for (int step = 0; step < angularSteps; step++)
                {
                    float angleRadians = math.radians((360f / angularSteps) * step);
                    float3 candidate = workerPosition + new float3(
                        math.cos(angleRadians) * radius,
                        0f,
                        math.sin(angleRadians) * radius);

                    if (CanPlaceConstructionAt(entityManager, buildingDefinition, candidate, out _))
                    {
                        buildPosition = candidate;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CanPlaceConstructionAt(EntityManager entityManager, BuildingDefinition buildingDefinition, float3 buildPosition, out string message)
        {
            message = string.Empty;
            if (!math.isfinite(buildPosition.x) || !math.isfinite(buildPosition.z))
            {
                message = "Build position is invalid.";
                return false;
            }

            float buildRadius = GetFootprintRadius(buildingDefinition);

            var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using (var positions = buildingQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp))
            using (var health = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp))
            using (var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp))
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    if (health[i].Current <= 0f)
                    {
                        continue;
                    }

                    float existingRadius = GetFootprintRadius(buildingTypes[i]);
                    if (math.distance(buildPosition, positions[i].Value) < buildRadius + existingRadius + 0.75f)
                    {
                        message = "Build site is obstructed by another structure.";
                        return false;
                    }
                }
            }

            var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using (var positions = settlementQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp))
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    if (math.distance(buildPosition, positions[i].Value) < buildRadius + 3f)
                    {
                        message = "Build site is too close to a settlement anchor.";
                        return false;
                    }
                }
            }

            var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using (var positions = controlPointQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp))
            using (var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp))
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    float protectedRadius = buildRadius + math.max(2f, controlPoints[i].RadiusTiles * 1.75f);
                    if (math.distance(buildPosition, positions[i].Value) < protectedRadius)
                    {
                        message = "Build site overlaps a control-point zone.";
                        return false;
                    }
                }
            }

            var resourceNodeQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceNodeComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using (var positions = resourceNodeQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp))
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    if (math.distance(buildPosition, positions[i].Value) < buildRadius + 1.2f)
                    {
                        message = "Build site overlaps a resource node.";
                        return false;
                    }
                }
            }

            return true;
        }

        private bool TrySelectControlledBuilding(EntityManager entityManager, string buildingTypeId, bool clearSelectionFirst)
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
                if (health[i].Current <= 0f || !factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                if (!buildingTypes[i].TypeId.Equals(buildingTypeId))
                {
                    continue;
                }

                if (clearSelectionFirst)
                {
                    ClearSelection(entityManager);
                }

                AddSelection(entityManager, entities[i]);
                return true;
            }

            return false;
        }

        private bool TrySelectControlledConstructionSite(EntityManager entityManager, string buildingTypeId, bool clearSelectionFirst)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<ConstructionStateComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f || !factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                if (!buildingTypes[i].TypeId.Equals(buildingTypeId))
                {
                    continue;
                }

                if (clearSelectionFirst)
                {
                    ClearSelection(entityManager);
                }

                AddSelection(entityManager, entities[i]);
                return true;
            }

            return false;
        }

        private int GetProductionQueueCount(EntityManager entityManager, string buildingTypeId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(controlledFactionId) ||
                    !buildingTypes[i].TypeId.Equals(buildingTypeId))
                {
                    continue;
                }

                if (!entityManager.HasBuffer<ProductionQueueItemElement>(entities[i]))
                {
                    return 0;
                }

                return entityManager.GetBuffer<ProductionQueueItemElement>(entities[i]).Length;
            }

            return 0;
        }

        private bool TryGetSelectedProductionBuildingSnapshot(EntityManager entityManager, out SelectedProductionBuildingSnapshot snapshot)
        {
            snapshot = default;
            if (selectedEntities.Count != 1)
            {
                return false;
            }

            var entity = selectedEntities[0];
            if (!IsSelectableAliveBuilding(entityManager, entity))
            {
                return false;
            }

            if (entityManager.HasComponent<ConstructionStateComponent>(entity))
            {
                return false;
            }

            var buildingType = entityManager.GetComponentData<BuildingTypeComponent>(entity);
            if (!TryResolveBuildingDefinition(buildingType.TypeId.ToString(), out var definition) ||
                (definition.trainableUnits?.Length ?? 0) == 0)
            {
                return false;
            }

            var queue = entityManager.HasBuffer<ProductionQueueItemElement>(entity)
                ? entityManager.GetBuffer<ProductionQueueItemElement>(entity)
                : default;

            snapshot = new SelectedProductionBuildingSnapshot
            {
                Entity = entity,
                FactionId = entityManager.GetComponentData<FactionComponent>(entity).FactionId.ToString(),
                TypeId = buildingType.TypeId.ToString(),
                DisplayName = string.IsNullOrWhiteSpace(definition.displayName) ? buildingType.TypeId.ToString() : definition.displayName,
                Definition = definition,
                QueueCount = queue.IsCreated ? queue.Length : 0,
                QueueFrontDisplayName = queue.IsCreated && queue.Length > 0 ? queue[0].DisplayName.ToString() : string.Empty,
                QueueFrontRemainingSeconds = queue.IsCreated && queue.Length > 0 ? queue[0].RemainingSeconds : 0f,
            };
            return true;
        }

        private bool TryGetFactionRuntimeSnapshot(EntityManager entityManager, string factionId, out FactionRuntimeSnapshot snapshot)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(ResourceStockpileComponent),
                typeof(PopulationComponent),
                typeof(FactionHouseComponent),
                typeof(FaithStateComponent));

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var resources = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);
            using var populations = query.ToComponentDataArray<PopulationComponent>(Allocator.Temp);
            using var houses = query.ToComponentDataArray<FactionHouseComponent>(Allocator.Temp);
            using var faithStates = query.ToComponentDataArray<FaithStateComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                snapshot = new FactionRuntimeSnapshot
                {
                    Entity = entities[i],
                    FactionId = factions[i].FactionId.ToString(),
                    HouseId = houses[i].HouseId.ToString(),
                    Resources = resources[i],
                    Population = populations[i],
                    FaithState = faithStates[i],
                };
                return true;
            }

            snapshot = default;
            return false;
        }

        private bool TryResolveBuildingDefinition(string buildingId, out BuildingDefinition definition)
        {
            EnsureDefinitionLookups();
            definition = null;
            return !string.IsNullOrWhiteSpace(buildingId) &&
                   buildingDefinitionsById.TryGetValue(buildingId, out definition) &&
                   definition != null;
        }

        private bool TryResolveUnitDefinition(string unitId, out UnitDefinition definition)
        {
            EnsureDefinitionLookups();
            definition = null;
            return !string.IsNullOrWhiteSpace(unitId) &&
                   unitDefinitionsById.TryGetValue(unitId, out definition) &&
                   definition != null;
        }

        private void EnsureDefinitionLookups()
        {
            if (bootstrapAuthoring == null)
            {
                bootstrapAuthoring = FindFirstObjectByType<BloodlinesMapBootstrapAuthoring>();
            }

            if (bootstrapAuthoring == null)
            {
                return;
            }

            if (buildingDefinitionsById.Count == (bootstrapAuthoring.BuildingDefinitions?.Length ?? 0) &&
                unitDefinitionsById.Count == (bootstrapAuthoring.UnitDefinitions?.Length ?? 0))
            {
                return;
            }

            buildingDefinitionsById.Clear();
            foreach (var definition in bootstrapAuthoring.BuildingDefinitions ?? Array.Empty<BuildingDefinition>())
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.id))
                {
                    continue;
                }

                buildingDefinitionsById[definition.id] = definition;
            }

            unitDefinitionsById.Clear();
            foreach (var definition in bootstrapAuthoring.UnitDefinitions ?? Array.Empty<UnitDefinition>())
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.id))
                {
                    continue;
                }

                unitDefinitionsById[definition.id] = definition;
            }
        }

        private static float GetProductionDurationSeconds(UnitDefinition unitDefinition)
        {
            return 9f + math.max(0, unitDefinition.populationCost) * 2f;
        }

        private static int GetIronmarkBloodPrice(in FactionRuntimeSnapshot factionSnapshot, UnitDefinition unitDefinition)
        {
            return factionSnapshot.HouseId.Equals("ironmark", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(unitDefinition.role, "worker", StringComparison.OrdinalIgnoreCase)
                ? math.max(0, unitDefinition.ironmarkBloodPrice > 0 ? unitDefinition.ironmarkBloodPrice : 1)
                : 0;
        }

        private static float GetBloodProductionLoadDelta(in FactionRuntimeSnapshot factionSnapshot, UnitDefinition unitDefinition)
        {
            return factionSnapshot.HouseId.Equals("ironmark", StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(unitDefinition.role, "worker", StringComparison.OrdinalIgnoreCase)
                ? math.max(0f, unitDefinition.bloodProductionLoadDelta > 0f ? unitDefinition.bloodProductionLoadDelta : 1.5f)
                : 0f;
        }

        private static bool CanAffordCost(ResourceStockpileComponent resources, ResourceAmountFields cost)
        {
            if (cost == null)
            {
                return true;
            }

            return resources.Gold >= cost.gold &&
                   resources.Food >= cost.food &&
                   resources.Water >= cost.water &&
                   resources.Wood >= cost.wood &&
                   resources.Stone >= cost.stone &&
                   resources.Iron >= cost.iron &&
                   resources.Influence >= cost.influence;
        }

        private static void SpendCost(ref ResourceStockpileComponent resources, ResourceAmountFields cost)
        {
            if (cost == null)
            {
                return;
            }

            resources.Gold -= cost.gold;
            resources.Food -= cost.food;
            resources.Water -= cost.water;
            resources.Wood -= cost.wood;
            resources.Stone -= cost.stone;
            resources.Iron -= cost.iron;
            resources.Influence -= cost.influence;
        }

        private static void RefundQueuedCost(ref ResourceStockpileComponent resources, in ProductionQueueItemElement queueItem)
        {
            resources.Gold += queueItem.GoldCost;
            resources.Food += queueItem.FoodCost;
            resources.Water += queueItem.WaterCost;
            resources.Wood += queueItem.WoodCost;
            resources.Stone += queueItem.StoneCost;
            resources.Iron += queueItem.IronCost;
            resources.Influence += queueItem.InfluenceCost;
        }

        private static void RestoreQueuedPopulation(ref PopulationComponent population, in ProductionQueueItemElement queueItem)
        {
            int restoredTotal = population.Total + math.max(0, queueItem.BloodPrice);
            int restoredAvailable = population.Available + math.max(0, queueItem.PopulationCost) + math.max(0, queueItem.BloodPrice);

            population.Total = math.min(population.Cap, restoredTotal);
            population.Available = math.min(population.Total, restoredAvailable);
        }

        private static string FormatCostSummary(ResourceAmountFields cost)
        {
            if (cost == null)
            {
                return "free";
            }

            var builder = new StringBuilder(80);
            AppendCostTerm(builder, "G", cost.gold);
            AppendCostTerm(builder, "F", cost.food);
            AppendCostTerm(builder, "W", cost.water);
            AppendCostTerm(builder, "Wood", cost.wood);
            AppendCostTerm(builder, "Stone", cost.stone);
            AppendCostTerm(builder, "Iron", cost.iron);
            AppendCostTerm(builder, "Inf", cost.influence);
            return builder.Length == 0 ? "free" : builder.ToString();
        }

        private static string FormatQueuedCostSummary(in ProductionQueueItemElement queueItem)
        {
            var builder = new StringBuilder(80);
            AppendCostTerm(builder, "G", queueItem.GoldCost);
            AppendCostTerm(builder, "F", queueItem.FoodCost);
            AppendCostTerm(builder, "W", queueItem.WaterCost);
            AppendCostTerm(builder, "Wood", queueItem.WoodCost);
            AppendCostTerm(builder, "Stone", queueItem.StoneCost);
            AppendCostTerm(builder, "Iron", queueItem.IronCost);
            AppendCostTerm(builder, "Inf", queueItem.InfluenceCost);
            return builder.Length == 0 ? "free" : builder.ToString();
        }

        private static void AppendCostTerm(StringBuilder builder, string label, int value)
        {
            if (value <= 0)
            {
                return;
            }

            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(label).Append(' ').Append(value);
        }

        private static bool TryResolveCovenantId(string faithId, out CovenantId covenantId)
        {
            if (string.Equals(faithId, "old_light", StringComparison.OrdinalIgnoreCase))
            {
                covenantId = CovenantId.OldLight;
                return true;
            }

            if (string.Equals(faithId, "blood_dominion", StringComparison.OrdinalIgnoreCase))
            {
                covenantId = CovenantId.BloodDominion;
                return true;
            }

            if (string.Equals(faithId, "the_order", StringComparison.OrdinalIgnoreCase))
            {
                covenantId = CovenantId.TheOrder;
                return true;
            }

            if (string.Equals(faithId, "the_wild", StringComparison.OrdinalIgnoreCase))
            {
                covenantId = CovenantId.TheWild;
                return true;
            }

            covenantId = CovenantId.None;
            return false;
        }

        private static bool TryResolveDoctrinePath(string doctrinePath, out DoctrinePath path)
        {
            if (string.Equals(doctrinePath, "light", StringComparison.OrdinalIgnoreCase))
            {
                path = DoctrinePath.Light;
                return true;
            }

            if (string.Equals(doctrinePath, "dark", StringComparison.OrdinalIgnoreCase))
            {
                path = DoctrinePath.Dark;
                return true;
            }

            path = DoctrinePath.Unassigned;
            return false;
        }

        private static string FormatIdentifier(string value)
        {
            return (value ?? string.Empty).Replace('_', ' ');
        }

        private static UnitRole ResolveUnitRole(string role)
        {
            if (string.Equals(role, "worker", StringComparison.OrdinalIgnoreCase)) return UnitRole.Worker;
            if (string.Equals(role, "melee", StringComparison.OrdinalIgnoreCase)) return UnitRole.Melee;
            if (string.Equals(role, "melee-recon", StringComparison.OrdinalIgnoreCase)) return UnitRole.MeleeRecon;
            if (string.Equals(role, "ranged", StringComparison.OrdinalIgnoreCase)) return UnitRole.Ranged;
            if (string.Equals(role, "unique-melee", StringComparison.OrdinalIgnoreCase)) return UnitRole.UniqueMelee;
            if (string.Equals(role, "light-cavalry", StringComparison.OrdinalIgnoreCase)) return UnitRole.LightCavalry;
            if (string.Equals(role, "siege-engine", StringComparison.OrdinalIgnoreCase)) return UnitRole.SiegeEngine;
            if (string.Equals(role, "siege-support", StringComparison.OrdinalIgnoreCase)) return UnitRole.SiegeSupport;
            if (string.Equals(role, "engineer-specialist", StringComparison.OrdinalIgnoreCase)) return UnitRole.EngineerSpecialist;
            if (string.Equals(role, "support", StringComparison.OrdinalIgnoreCase)) return UnitRole.Support;
            return UnitRole.Unknown;
        }

        private static SiegeClass ResolveSiegeClass(string siegeClass)
        {
            if (string.Equals(siegeClass, "ram", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Ram;
            if (string.Equals(siegeClass, "siege_tower", StringComparison.OrdinalIgnoreCase)) return SiegeClass.SiegeTower;
            if (string.Equals(siegeClass, "trebuchet", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Trebuchet;
            if (string.Equals(siegeClass, "ballista", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Ballista;
            if (string.Equals(siegeClass, "mantlet", StringComparison.OrdinalIgnoreCase)) return SiegeClass.Mantlet;
            return SiegeClass.None;
        }

        private void SetProductionFeedback(string message, bool success)
        {
            productionFeedbackMessage = success ? $"<b>Queued</b>: {message}" : $"<b>Blocked</b>: {message}";
            productionFeedbackExpiresAt = Time.unscaledTime + 4f;
        }

        private bool HasActiveProductionFeedback()
        {
            return !string.IsNullOrWhiteSpace(productionFeedbackMessage) &&
                   productionFeedbackExpiresAt > Time.unscaledTime;
        }

        private bool HasPendingConstructionPlacement()
        {
            return !string.IsNullOrWhiteSpace(pendingConstructionBuildingId);
        }

        private void SetConstructionFeedback(string message, bool success)
        {
            constructionFeedbackMessage = success ? $"<b>Build</b>: {message}" : $"<b>Blocked</b>: {message}";
            constructionFeedbackExpiresAt = Time.unscaledTime + 4f;
        }

        private bool HasActiveConstructionFeedback()
        {
            return !string.IsNullOrWhiteSpace(constructionFeedbackMessage) &&
                   constructionFeedbackExpiresAt > Time.unscaledTime;
        }

        private bool TryGetSelectedWorkerSnapshot(EntityManager entityManager, out SelectedWorkerSnapshot snapshot)
        {
            snapshot = default;

            Entity firstWorker = Entity.Null;
            string factionId = string.Empty;
            string displayName = "Worker";
            int workerCount = 0;

            for (int i = 0; i < selectedEntities.Count; i++)
            {
                var entity = selectedEntities[i];
                if (!IsSelectableAliveUnit(entityManager, entity))
                {
                    continue;
                }

                if (!entityManager.HasComponent<FactionComponent>(entity))
                {
                    continue;
                }

                var faction = entityManager.GetComponentData<FactionComponent>(entity);
                if (!faction.FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                var unitType = entityManager.GetComponentData<UnitTypeComponent>(entity);
                if (unitType.Role != UnitRole.Worker)
                {
                    continue;
                }

                if (workerCount == 0)
                {
                    firstWorker = entity;
                    factionId = faction.FactionId.ToString();
                    if (TryResolveUnitDefinition(unitType.TypeId.ToString(), out var definition) &&
                        !string.IsNullOrWhiteSpace(definition.displayName))
                    {
                        displayName = definition.displayName;
                    }
                    else if (!string.IsNullOrWhiteSpace(unitType.TypeId.ToString()))
                    {
                        displayName = FormatIdentifier(unitType.TypeId.ToString());
                    }
                }

                workerCount++;
            }

            if (workerCount <= 0 || firstWorker == Entity.Null)
            {
                return false;
            }

            snapshot = new SelectedWorkerSnapshot
            {
                Entity = firstWorker,
                FactionId = factionId,
                DisplayName = workerCount > 1 ? $"{displayName} x{workerCount}" : displayName,
                WorkerCount = workerCount,
            };
            return true;
        }

        private bool TrySelectFirstControlledWorker(EntityManager entityManager, bool clearSelectionFirst)
        {
            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f ||
                    !factions[i].FactionId.Equals(controlledFactionId) ||
                    unitTypes[i].Role != UnitRole.Worker)
                {
                    continue;
                }

                if (clearSelectionFirst)
                {
                    ClearSelection(entityManager);
                }

                AddSelection(entityManager, entities[i]);
                return true;
            }

            return false;
        }

        private bool IsSupportedConstructionBuilding(BuildingDefinition buildingDefinition)
        {
            string buildingId = buildingDefinition?.id ?? string.Empty;
            for (int i = 0; i < SupportedConstructionBuildingOrder.Length; i++)
            {
                if (string.Equals(SupportedConstructionBuildingOrder[i], buildingId, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private BuildingTypeComponent CreateBuildingTypeComponent(BuildingDefinition buildingDefinition)
        {
            return new BuildingTypeComponent
            {
                TypeId = buildingDefinition?.id ?? string.Empty,
                FortificationRole = ResolveFortificationRole(buildingDefinition?.fortificationRole),
                StructuralDamageMultiplier = buildingDefinition != null && buildingDefinition.structuralDamageMultiplier > 0f
                    ? buildingDefinition.structuralDamageMultiplier
                    : 1f,
                PopulationCapBonus = buildingDefinition?.populationCapBonus ?? 0,
                BlocksPassage = buildingDefinition?.blocksPassage ?? false,
                SupportsSiegePreparation = string.Equals(buildingDefinition?.id, "siege_workshop", StringComparison.OrdinalIgnoreCase),
                SupportsSiegeLogistics = string.Equals(buildingDefinition?.id, "supply_camp", StringComparison.OrdinalIgnoreCase),
            };
        }

        private static FortificationRole ResolveFortificationRole(string fortificationRole)
        {
            if (string.Equals(fortificationRole, "wall", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Wall;
            if (string.Equals(fortificationRole, "tower", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Tower;
            if (string.Equals(fortificationRole, "gate", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Gate;
            if (string.Equals(fortificationRole, "keep", StringComparison.OrdinalIgnoreCase)) return FortificationRole.Keep;
            return FortificationRole.None;
        }

        private float GetFootprintRadius(BuildingDefinition buildingDefinition)
        {
            int width = math.max(1, buildingDefinition?.footprint?.w ?? 1);
            int height = math.max(1, buildingDefinition?.footprint?.h ?? 1);
            return math.max(1.25f, math.max(width, height) * 0.85f);
        }

        private float GetFootprintRadius(BuildingTypeComponent buildingType)
        {
            return TryResolveBuildingDefinition(buildingType.TypeId.ToString(), out var definition)
                ? GetFootprintRadius(definition)
                : 1.5f;
        }

        public string ControlledFactionId => controlledFactionId;

        public bool SelectOnlyCombatUnits => selectOnlyCombatUnits;

        public bool TryDebugSelectAllControlledUnits()
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            SelectAllControlledUnits(entityManager);
            return GetSelectedCountForDebug() > 0;
        }

        public bool TryDebugSaveControlGroup(int slot)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            SaveSelectionToControlGroup(entityManager, slot);
            return GetControlGroupCountForDebug(slot) > 0;
        }

        public bool TryDebugRecallControlGroup(int slot, bool additive = false)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            RecallControlGroup(entityManager, slot, additive);
            return GetSelectedCountForDebug() > 0;
        }

        public bool TryDebugClearSelection()
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            ClearSelection(entityManager);
            return GetSelectedCountForDebug() == 0;
        }

        public bool TryDebugFrameSelection()
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            return FrameSelection(entityManager);
        }

        public bool TryDebugSelectControlledBuilding(string buildingTypeId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            return TrySelectControlledBuilding(entityManager, buildingTypeId, clearSelectionFirst: true);
        }

        public bool TryDebugQueueProduction(string buildingTypeId, string unitId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            return TryQueueProductionFromControlledBuilding(entityManager, buildingTypeId, unitId, out _);
        }

        public bool TryDebugCancelProduction(string buildingTypeId, int queueIndex = 0)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            return TryCancelProductionFromControlledBuilding(entityManager, buildingTypeId, queueIndex, out _);
        }

        public bool TryDebugStartConstruction(string buildingTypeId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (!TryGetSelectedWorkerSnapshot(entityManager, out _))
            {
                if (!TrySelectFirstControlledWorker(entityManager, clearSelectionFirst: true))
                {
                    return false;
                }
            }

            return TryDebugStartConstructionNearSelectedWorker(entityManager, buildingTypeId, out _);
        }

        public bool TryDebugSelectControlledConstructionSite(string buildingTypeId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            return TrySelectControlledConstructionSite(entityManager, buildingTypeId, clearSelectionFirst: true);
        }

        public bool TryDebugGetSelectedConstructionProgress(
            out float progressRatio,
            out float remainingSeconds,
            out float totalSeconds,
            out string buildingTypeId)
        {
            progressRatio = 0f;
            remainingSeconds = 0f;
            totalSeconds = 0f;
            buildingTypeId = string.Empty;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (!TryGetSelectedConstructionSiteSnapshot(entityManager, out var siteSnapshot))
            {
                return false;
            }

            progressRatio = siteSnapshot.ProgressRatio;
            remainingSeconds = siteSnapshot.RemainingSeconds;
            totalSeconds = siteSnapshot.TotalSeconds;
            buildingTypeId = siteSnapshot.TypeId;
            return true;
        }

        public int TryDebugAssignSelectedWorkersToGatherResource(string resourceId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return 0;
            }

            return AssignSelectedWorkersToGatherResource(entityManager, resourceId);
        }

        public bool TryDebugForceStarvationCycle(
            string factionId,
            bool includeWaterCrisis,
            out int previousPopulationTotal,
            out int expectedPopulationTotal)
        {
            previousPopulationTotal = 0;
            expectedPopulationTotal = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (!TryGetRealmCycleConfig(entityManager, out var cfg))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(RealmConditionComponent),
                typeof(PopulationComponent),
                typeof(ResourceStockpileComponent));

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var realms = query.ToComponentDataArray<RealmConditionComponent>(Allocator.Temp);
            using var populations = query.ToComponentDataArray<PopulationComponent>(Allocator.Temp);
            using var stockpiles = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                {
                    continue;
                }

                previousPopulationTotal = populations[i].Total;

                var realm = realms[i];
                realm.FoodStrainStreak = math.max(realm.FoodStrainStreak, math.max(1, cfg.FoodFamineConsecutiveCycles));
                if (includeWaterCrisis)
                {
                    realm.WaterStrainStreak = math.max(realm.WaterStrainStreak, math.max(1, cfg.WaterCrisisConsecutiveCycles));
                }
                realm.LastStarvationResponseCycle = realm.CycleCount;
                realm.CycleCount += 1;
                realm.CycleAccumulator = 0f;
                entityManager.SetComponentData(entities[i], realm);

                var stockpile = stockpiles[i];
                stockpile.Food = 0f;
                if (includeWaterCrisis)
                {
                    stockpile.Water = 0f;
                }
                entityManager.SetComponentData(entities[i], stockpile);

                int totalDecline = math.max(0, cfg.FaminePopulationDeclinePerCycle);
                if (includeWaterCrisis)
                {
                    totalDecline += math.max(0, cfg.WaterCrisisOutmigrationPerCycle);
                }
                expectedPopulationTotal = math.max(0, previousPopulationTotal - totalDecline);
                return true;
            }

            return false;
        }

        public bool TryDebugGetFactionLoyalty(string factionId, out float current, out float max, out float floor)
        {
            current = 0f;
            max = 0f;
            floor = 0f;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionLoyaltyComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var loyalties = query.ToComponentDataArray<FactionLoyaltyComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                {
                    continue;
                }

                current = loyalties[i].Current;
                max = loyalties[i].Max;
                floor = loyalties[i].Floor;
                return true;
            }

            return false;
        }

        public bool TryDebugGetFactionPopulation(string factionId, out int total, out int available, out int cap)
        {
            total = 0;
            available = 0;
            cap = 0;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (!TryGetFactionRuntimeSnapshot(entityManager, factionId, out var snapshot))
            {
                return false;
            }

            total = snapshot.Population.Total;
            available = snapshot.Population.Available;
            cap = snapshot.Population.Cap;
            return true;
        }

        private static bool TryGetRealmCycleConfig(EntityManager entityManager, out RealmCycleConfig cfg)
        {
            cfg = default;
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<RealmCycleConfig>());
            using var data = query.ToComponentDataArray<RealmCycleConfig>(Allocator.Temp);
            if (data.Length == 0)
            {
                return false;
            }

            cfg = data[0];
            return true;
        }

        public bool TryDebugGetFactionStockpile(
            string factionId,
            out float gold,
            out float wood,
            out float stone,
            out float iron,
            out float food,
            out float water,
            out float influence)
        {
            gold = wood = stone = iron = food = water = influence = 0f;
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (!TryGetFactionRuntimeSnapshot(entityManager, factionId, out var snapshot))
            {
                return false;
            }

            gold = snapshot.Resources.Gold;
            wood = snapshot.Resources.Wood;
            stone = snapshot.Resources.Stone;
            iron = snapshot.Resources.Iron;
            food = snapshot.Resources.Food;
            water = snapshot.Resources.Water;
            influence = snapshot.Resources.Influence;
            return true;
        }

        public int GetControlledWorkersWithActiveGatherCount()
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return 0;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<WorkerGatherComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var gathers = query.ToComponentDataArray<WorkerGatherComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            int active = 0;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                if (gathers[i].Phase != WorkerGatherPhase.Idle)
                {
                    active++;
                }
            }

            return active;
        }

        private int AssignSelectedWorkersToGatherResource(EntityManager entityManager, string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                return 0;
            }

            int selectedWorkerCount = 0;
            foreach (var entity in selectedEntities)
            {
                if (entityManager.Exists(entity) &&
                    entityManager.HasComponent<UnitTypeComponent>(entity) &&
                    entityManager.GetComponentData<UnitTypeComponent>(entity).Role == UnitRole.Worker)
                {
                    selectedWorkerCount++;
                }
            }

            if (selectedWorkerCount == 0)
            {
                if (!TrySelectFirstControlledWorker(entityManager, clearSelectionFirst: true))
                {
                    return 0;
                }
            }

            if (!TryFindNearestControlledResourceNode(entityManager, resourceId, out var nodeEntity, out var nodePosition))
            {
                return 0;
            }

            int assigned = 0;
            var resourceIdKey = new FixedString32Bytes(resourceId);
            foreach (var entity in selectedEntities)
            {
                if (!entityManager.Exists(entity) ||
                    !entityManager.HasComponent<UnitTypeComponent>(entity))
                {
                    continue;
                }

                var unitType = entityManager.GetComponentData<UnitTypeComponent>(entity);
                if (unitType.Role != UnitRole.Worker)
                {
                    continue;
                }

                string typeIdString = unitType.TypeId.ToString();
                if (!TryResolveUnitDefinition(typeIdString, out var unitDefinition))
                {
                    continue;
                }

                float capacity = math.max(1f, unitDefinition.carryCapacity > 0 ? unitDefinition.carryCapacity : 10f);
                float rate = math.max(0.1f, unitDefinition.gatherRate > 0f ? unitDefinition.gatherRate : 1f);

                var gather = new WorkerGatherComponent
                {
                    AssignedNode = nodeEntity,
                    AssignedResourceId = resourceIdKey,
                    CarryResourceId = default,
                    CarryAmount = 0f,
                    CarryCapacity = capacity,
                    GatherRate = rate,
                    Phase = WorkerGatherPhase.Seeking,
                    GatherRadius = 1.25f,
                    DepositRadius = 1.75f,
                };

                if (entityManager.HasComponent<WorkerGatherComponent>(entity))
                {
                    entityManager.SetComponentData(entity, gather);
                }
                else
                {
                    entityManager.AddComponentData(entity, gather);
                }

                assigned++;
            }

            return assigned;
        }

        private bool TryFindNearestControlledResourceNode(
            EntityManager entityManager,
            string resourceId,
            out Entity nodeEntity,
            out float3 nodePosition)
        {
            nodeEntity = Entity.Null;
            nodePosition = default;

            var resourceKey = new FixedString32Bytes(resourceId);
            float3 reference = default;
            bool referenceAvailable = false;

            if (selectedEntities.Count > 0 &&
                entityManager.Exists(selectedEntities[0]) &&
                entityManager.HasComponent<PositionComponent>(selectedEntities[0]))
            {
                reference = entityManager.GetComponentData<PositionComponent>(selectedEntities[0]).Value;
                referenceAvailable = true;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ResourceNodeComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var nodes = query.ToComponentDataArray<ResourceNodeComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            float bestDistanceSq = float.MaxValue;
            bool found = false;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!nodes[i].ResourceId.Equals(resourceKey) || nodes[i].Amount <= 0f)
                {
                    continue;
                }

                float distanceSq = referenceAvailable
                    ? math.distancesq(reference, positions[i].Value)
                    : 0f;
                if (distanceSq < bestDistanceSq)
                {
                    bestDistanceSq = distanceSq;
                    nodeEntity = entities[i];
                    nodePosition = positions[i].Value;
                    found = true;
                    if (!referenceAvailable)
                    {
                        break;
                    }
                }
            }

            return found;
        }

        public bool TryDebugGetSelectedProductionProgress(
            out float progressRatio,
            out float remainingSeconds,
            out float totalSeconds,
            out string unitId,
            out string buildingTypeId)
        {
            progressRatio = 0f;
            remainingSeconds = 0f;
            totalSeconds = 0f;
            unitId = string.Empty;
            buildingTypeId = string.Empty;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (selectedEntities.Count != 1)
            {
                return false;
            }

            var entity = selectedEntities[0];
            if (!IsSelectableAliveBuilding(entityManager, entity))
            {
                return false;
            }

            if (!entityManager.HasBuffer<ProductionQueueItemElement>(entity))
            {
                return false;
            }

            var queue = entityManager.GetBuffer<ProductionQueueItemElement>(entity);
            if (queue.Length == 0)
            {
                return false;
            }

            var active = queue[0];
            float total = math.max(0.1f, active.TotalSeconds);
            float remaining = math.max(0f, active.RemainingSeconds);
            progressRatio = math.saturate(1f - (remaining / total));
            remainingSeconds = remaining;
            totalSeconds = total;
            unitId = active.UnitId.ToString();

            var buildingType = entityManager.GetComponentData<BuildingTypeComponent>(entity);
            buildingTypeId = buildingType.TypeId.ToString();
            return true;
        }

        public int GetProductionQueueCountForDebug(string buildingTypeId)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return 0;
            }

            return GetProductionQueueCount(entityManager, buildingTypeId);
        }

        public int GetSelectedCountForDebug()
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return 0;
            }

            PruneSelection(entityManager);
            return selectedEntities.Count;
        }

        public int GetControlGroupCountForDebug(int slot)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return 0;
            }

            PruneControlGroups(entityManager);
            if (!controlGroups.TryGetValue(slot, out var group))
            {
                return 0;
            }

            return group.Count;
        }

        private void ResolveCameraController()
        {
            if (battlefieldCameraController == null && controlledCamera != null)
            {
                battlefieldCameraController = controlledCamera.GetComponentInParent<BattlefieldCameraController>();
            }

            if (battlefieldCameraController == null)
            {
                battlefieldCameraController = FindFirstObjectByType<BattlefieldCameraController>();
            }
        }

        private static bool TryGetEntityManager(out EntityManager entityManager)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                entityManager = default;
                return false;
            }

            entityManager = world.EntityManager;
            return true;
        }

        private bool TryGetGroundPoint(Vector2 screenPosition, out float3 worldPoint)
        {
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            var ray = controlledCamera.ScreenPointToRay(screenPosition);
            if (groundPlane.Raycast(ray, out float enter))
            {
                var point = ray.GetPoint(enter);
                worldPoint = new float3(point.x, 0f, point.z);
                return true;
            }

            worldPoint = default;
            return false;
        }

        private void SelectNearestControlledEntity(EntityManager entityManager, float3 selectionPoint, bool additive)
        {
            if (!additive)
            {
                ClearSelection(entityManager);
            }

            if (TrySelectNearestControlledUnit(entityManager, selectionPoint))
            {
                return;
            }

            if (allowBuildingSelection)
            {
                TrySelectNearestControlledBuilding(entityManager, selectionPoint);
            }
        }

        private bool TrySelectNearestControlledUnit(EntityManager entityManager, float3 selectionPoint)
        {
            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            int bestIndex = -1;
            float bestDistance = singleSelectRadius * singleSelectRadius;

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f)
                {
                    continue;
                }

                if (!factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                if (selectOnlyCombatUnits && unitTypes[i].Role == UnitRole.Worker)
                {
                    continue;
                }

                float distance = math.distancesq(positions[i].Value, selectionPoint);
                if (distance > bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestIndex = i;
            }

            if (bestIndex < 0)
            {
                return false;
            }

            AddSelection(entityManager, entities[bestIndex]);
            return true;
        }

        private bool TrySelectNearestControlledBuilding(EntityManager entityManager, float3 selectionPoint, bool clearSelectionFirst = false)
        {
            if (clearSelectionFirst)
            {
                ClearSelection(entityManager);
            }

            var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = buildingQuery.ToEntityArray(Allocator.Temp);
            using var positions = buildingQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            int bestIndex = -1;
            float bestDistance = buildingSelectRadius * buildingSelectRadius;

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f || !factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                float distance = math.distancesq(positions[i].Value, selectionPoint);
                if (distance > bestDistance)
                {
                    continue;
                }

                bestDistance = distance;
                bestIndex = i;
            }

            if (bestIndex < 0)
            {
                return false;
            }

            AddSelection(entityManager, entities[bestIndex]);
            return true;
        }

        private void SelectAllControlledUnits(EntityManager entityManager)
        {
            ClearSelection(entityManager);

            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f || !factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                if (selectOnlyCombatUnits && unitTypes[i].Role == UnitRole.Worker)
                {
                    continue;
                }

                AddSelection(entityManager, entities[i]);
            }
        }

        private void HandleControlGroupHotkeys(EntityManager entityManager, Keyboard keyboard)
        {
            if (!enableControlGroups)
            {
                return;
            }

            bool ctrlHeld = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
            bool shiftHeld = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;

            if (IsControlGroupSlotEnabled(2) && keyboard.digit2Key.wasPressedThisFrame)
            {
                HandleControlGroupKey(entityManager, 2, ctrlHeld, shiftHeld);
            }

            if (IsControlGroupSlotEnabled(3) && keyboard.digit3Key.wasPressedThisFrame)
            {
                HandleControlGroupKey(entityManager, 3, ctrlHeld, shiftHeld);
            }

            if (IsControlGroupSlotEnabled(4) && keyboard.digit4Key.wasPressedThisFrame)
            {
                HandleControlGroupKey(entityManager, 4, ctrlHeld, shiftHeld);
            }

            if (IsControlGroupSlotEnabled(5) && keyboard.digit5Key.wasPressedThisFrame)
            {
                HandleControlGroupKey(entityManager, 5, ctrlHeld, shiftHeld);
            }
        }

        private void HandleControlGroupKey(EntityManager entityManager, int slot, bool ctrlHeld, bool shiftHeld)
        {
            if (ctrlHeld)
            {
                SaveSelectionToControlGroup(entityManager, slot);
                return;
            }

            RecallControlGroup(entityManager, slot, allowShiftAdditiveGroupRecall && shiftHeld);
        }

        private bool IsControlGroupSlotEnabled(int slot)
        {
            return slot >= minimumControlGroupSlot && slot <= maximumControlGroupSlot;
        }

        private void SaveSelectionToControlGroup(EntityManager entityManager, int slot)
        {
            if (!controlGroups.TryGetValue(slot, out var group))
            {
                group = new List<Entity>();
                controlGroups[slot] = group;
            }

            group.Clear();
            for (int i = 0; i < selectedEntities.Count; i++)
            {
                var entity = selectedEntities[i];
                if (!IsSelectableAliveUnit(entityManager, entity))
                {
                    continue;
                }

                group.Add(entity);
            }
        }

        private void RecallControlGroup(EntityManager entityManager, int slot, bool additive)
        {
            if (!controlGroups.TryGetValue(slot, out var group))
            {
                return;
            }

            if (!additive)
            {
                ClearSelection(entityManager);
            }

            for (int i = group.Count - 1; i >= 0; i--)
            {
                var entity = group[i];
                if (!IsSelectableAliveUnit(entityManager, entity))
                {
                    group.RemoveAt(i);
                    continue;
                }

                AddSelection(entityManager, entity);
            }
        }

        private void SelectUnitsInScreenRect(EntityManager entityManager, Vector2 startScreenPosition, Vector2 endScreenPosition, bool additive)
        {
            if (!additive)
            {
                ClearSelection(entityManager);
            }

            Rect selectionRect = GetSelectionRect(startScreenPosition, endScreenPosition);
            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f || !factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                if (selectOnlyCombatUnits && unitTypes[i].Role == UnitRole.Worker)
                {
                    continue;
                }

                Vector3 worldPosition = positions[i].Value + new float3(0f, 0.6f, 0f);
                Vector3 screenPosition = controlledCamera.WorldToScreenPoint(worldPosition);
                if (screenPosition.z < 0f)
                {
                    continue;
                }

                if (!selectionRect.Contains(new Vector2(screenPosition.x, screenPosition.y)))
                {
                    continue;
                }

                AddSelection(entityManager, entities[i]);
            }
        }

        private void IssueMoveCommand(EntityManager entityManager, float3 destination)
        {
            var commandUnits = new List<CommandUnit>(selectedEntities.Count);

            for (int i = selectedEntities.Count - 1; i >= 0; i--)
            {
                var entity = selectedEntities[i];
                if (!entityManager.Exists(entity))
                {
                    selectedEntities.RemoveAt(i);
                    continue;
                }

                if (!IsSelectableAliveUnit(entityManager, entity))
                {
                    continue;
                }

                float3 startPosition = entityManager.HasComponent<PositionComponent>(entity)
                    ? entityManager.GetComponentData<PositionComponent>(entity).Value
                    : destination;
                commandUnits.Add(new CommandUnit
                {
                    Entity = entity,
                    Position = startPosition,
                });
            }

            if (commandUnits.Count == 0)
            {
                return;
            }

            if (spreadMoveCommands && commandUnits.Count > 1)
            {
                AssignFormationLayout(commandUnits, destination);
            }
            else
            {
                for (int i = 0; i < commandUnits.Count; i++)
                {
                    var command = commandUnits[i];
                    command.Destination = destination;
                    commandUnits[i] = command;
                }
            }

            for (int i = 0; i < commandUnits.Count; i++)
            {
                ApplyMoveCommand(entityManager, commandUnits[i]);
            }
        }

        private void AddSelection(EntityManager entityManager, Entity entity)
        {
            if (selectedEntities.Contains(entity))
            {
                return;
            }

            if (!entityManager.HasComponent<SelectedTag>(entity))
            {
                entityManager.AddComponent<SelectedTag>(entity);
            }

            selectedEntities.Add(entity);
        }

        private void ClearSelection(EntityManager entityManager)
        {
            for (int i = selectedEntities.Count - 1; i >= 0; i--)
            {
                var entity = selectedEntities[i];
                if (entityManager.Exists(entity) && entityManager.HasComponent<SelectedTag>(entity))
                {
                    entityManager.RemoveComponent<SelectedTag>(entity);
                }
            }

            selectedEntities.Clear();
        }

        private void PruneSelection(EntityManager entityManager)
        {
            for (int i = selectedEntities.Count - 1; i >= 0; i--)
            {
                var entity = selectedEntities[i];
                if (!IsSelectableEntity(entityManager, entity))
                {
                    selectedEntities.RemoveAt(i);
                }
            }
        }

        private void PruneControlGroups(EntityManager entityManager)
        {
            foreach (var group in controlGroups.Values)
            {
                for (int i = group.Count - 1; i >= 0; i--)
                {
                    if (!IsSelectableAliveUnit(entityManager, group[i]))
                    {
                        group.RemoveAt(i);
                    }
                }
            }
        }

        private bool FrameSelection(EntityManager entityManager)
        {
            if (battlefieldCameraController == null)
            {
                return false;
            }

            if (!TryGetSelectionFocusPoint(entityManager, out var focusPoint))
            {
                return false;
            }

            battlefieldCameraController.FocusWorldPosition(new Vector3(focusPoint.x, 0f, focusPoint.z));
            return true;
        }

        private bool TryGetSelectionFocusPoint(EntityManager entityManager, out float3 focusPoint)
        {
            float3 accumulated = float3.zero;
            int count = 0;

            for (int i = 0; i < selectedEntities.Count; i++)
            {
                var entity = selectedEntities[i];
                if (!IsSelectableEntity(entityManager, entity) || !entityManager.HasComponent<PositionComponent>(entity))
                {
                    continue;
                }

                accumulated += entityManager.GetComponentData<PositionComponent>(entity).Value;
                count++;
            }

            if (count == 0)
            {
                return TryGetControlledForceFocusPoint(entityManager, out focusPoint);
            }

            focusPoint = accumulated / count;
            return true;
        }

        private bool TryGetControlledForceFocusPoint(EntityManager entityManager, out float3 focusPoint)
        {
            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            float3 accumulated = float3.zero;
            int count = 0;

            for (int i = 0; i < positions.Length; i++)
            {
                if (health[i].Current <= 0f || !factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                accumulated += positions[i].Value;
                count++;
            }

            if (count == 0)
            {
                focusPoint = default;
                return false;
            }

            focusPoint = accumulated / count;
            return true;
        }

        private static bool IsSelectableAliveUnit(EntityManager entityManager, Entity entity)
        {
            return entityManager.Exists(entity)
                && !entityManager.HasComponent<DeadTag>(entity)
                && entityManager.HasComponent<UnitTypeComponent>(entity);
        }

        private static bool IsSelectableAliveBuilding(EntityManager entityManager, Entity entity)
        {
            return entityManager.Exists(entity)
                && !entityManager.HasComponent<DeadTag>(entity)
                && entityManager.HasComponent<BuildingTypeComponent>(entity)
                && (!entityManager.HasComponent<HealthComponent>(entity) ||
                    entityManager.GetComponentData<HealthComponent>(entity).Current > 0f);
        }

        private static bool IsSelectableEntity(EntityManager entityManager, Entity entity)
        {
            return IsSelectableAliveUnit(entityManager, entity) || IsSelectableAliveBuilding(entityManager, entity);
        }

        private void AssignFormationLayout(List<CommandUnit> commandUnits, float3 destination)
        {
            float3 centroid = float3.zero;
            for (int i = 0; i < commandUnits.Count; i++)
            {
                centroid += commandUnits[i].Position;
            }

            centroid /= commandUnits.Count;

            float3 forward = destination - centroid;
            forward.y = 0f;
            if (math.lengthsq(forward) < 0.001f)
            {
                var cameraForward = controlledCamera != null ? controlledCamera.transform.forward : Vector3.forward;
                forward = new float3(cameraForward.x, 0f, cameraForward.z);
            }

            if (math.lengthsq(forward) < 0.001f)
            {
                forward = new float3(0f, 0f, 1f);
            }

            forward = math.normalize(forward);
            float3 right = new float3(forward.z, 0f, -forward.x);
            if (math.lengthsq(right) < 0.001f)
            {
                right = new float3(1f, 0f, 0f);
            }
            else
            {
                right = math.normalize(right);
            }

            for (int i = 0; i < commandUnits.Count; i++)
            {
                var command = commandUnits[i];
                command.Lateral = math.dot(command.Position, right);
                command.Depth = math.dot(command.Position, forward);
                commandUnits[i] = command;
            }

            commandUnits.Sort(static (a, b) =>
            {
                int depthCompare = a.Depth.CompareTo(b.Depth);
                return depthCompare != 0 ? depthCompare : a.Lateral.CompareTo(b.Lateral);
            });

            int columns = math.max(1, math.min(formationMaxColumns, (int)math.ceil(math.sqrt(commandUnits.Count))));
            float spacing = math.max(0.5f, formationSpacing);

            for (int i = 0; i < commandUnits.Count; i++)
            {
                int row = i / columns;
                int column = i % columns;
                float centeredColumn = column - ((columns - 1) * 0.5f);
                float3 offset = (right * centeredColumn * spacing) - (forward * row * spacing);

                var command = commandUnits[i];
                command.Destination = destination + offset;
                commandUnits[i] = command;
            }
        }

        private void ApplyMoveCommand(EntityManager entityManager, CommandUnit command)
        {
            var moveCommand = new MoveCommandComponent
            {
                Destination = command.Destination,
                StoppingDistance = commandStoppingDistance,
                IsActive = true,
            };

            if (entityManager.HasComponent<MoveCommandComponent>(command.Entity))
            {
                entityManager.SetComponentData(command.Entity, moveCommand);
            }
            else
            {
                entityManager.AddComponentData(command.Entity, moveCommand);
            }
        }

        private static Rect GetSelectionRect(Vector2 startScreenPosition, Vector2 endScreenPosition)
        {
            float minX = math.min(startScreenPosition.x, endScreenPosition.x);
            float minY = math.min(startScreenPosition.y, endScreenPosition.y);
            float maxX = math.max(startScreenPosition.x, endScreenPosition.x);
            float maxY = math.max(startScreenPosition.y, endScreenPosition.y);
            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        private static void DrawRectangleBorder(Rect rect, float thickness)
        {
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
        }

        private void EnsureHudStyles()
        {
            if (hudTitleStyle == null)
            {
                hudTitleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    richText = false,
                    wordWrap = false
                };
                hudTitleStyle.normal.textColor = Color.white;
            }

            if (hudBodyStyle == null)
            {
                hudBodyStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    richText = true,
                    wordWrap = true
                };
                hudBodyStyle.normal.textColor = new Color(0.94f, 0.97f, 1f, 1f);
            }

            if (hudButtonStyle == null)
            {
                hudButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 11,
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    richText = false,
                    padding = new RectOffset(10, 10, 8, 8)
                };
            }
        }

        private string BuildHudText(FactionHudSnapshot factionSnapshot, TerritoryHudSnapshot territorySnapshot, SelectionHudSnapshot selectionSnapshot)
        {
            var builder = new StringBuilder(512);
            builder.Append("<b>Faction</b>: ").Append(controlledFactionId);

            if (factionSnapshot.Found)
            {
                builder.Append("    <b>Population</b>: ")
                    .Append(factionSnapshot.PopulationTotal)
                    .Append('/')
                    .Append(factionSnapshot.PopulationCap)
                    .Append(" (available ")
                    .Append(factionSnapshot.PopulationAvailable)
                    .Append(')');
            }
            else
            {
                builder.Append("    <b>Population</b>: unavailable");
            }

            builder.AppendLine();
            builder.Append("<b>Resources</b>: ");
            if (factionSnapshot.Found)
            {
                builder.Append("G ").Append(Mathf.RoundToInt(factionSnapshot.Gold))
                    .Append("  F ").Append(Mathf.RoundToInt(factionSnapshot.Food))
                    .Append("  W ").Append(Mathf.RoundToInt(factionSnapshot.Water))
                    .Append("  Wood ").Append(Mathf.RoundToInt(factionSnapshot.Wood))
                    .Append("  Stone ").Append(Mathf.RoundToInt(factionSnapshot.Stone))
                    .Append("  Iron ").Append(Mathf.RoundToInt(factionSnapshot.Iron))
                    .Append("  Inf ").Append(Mathf.RoundToInt(factionSnapshot.Influence));
            }
            else
            {
                builder.Append("unavailable");
            }

            builder.AppendLine();
            builder.Append("<b>Selection</b>: units ").Append(selectionSnapshot.TotalSelected)
                .Append("  |  buildings ").Append(selectionSnapshot.BuildingsSelected);
            if (selectionSnapshot.TotalSelected > 0)
            {
                builder.Append("  |  workers ").Append(selectionSnapshot.Workers)
                    .Append("  melee ").Append(selectionSnapshot.Melee)
                    .Append("  ranged ").Append(selectionSnapshot.Ranged)
                    .Append("  cavalry ").Append(selectionSnapshot.Cavalry)
                    .Append("  siege ").Append(selectionSnapshot.Siege)
                    .Append("  support ").Append(selectionSnapshot.Support)
                    .Append("  other ").Append(selectionSnapshot.Other);
            }
            else
            {
                builder.Append("  |  no units selected");
            }

            builder.AppendLine();
            builder.Append("<b>Control Points</b>: ours ").Append(territorySnapshot.Controlled)
                .Append("  hostile ").Append(territorySnapshot.Hostile)
                .Append("  neutral ").Append(territorySnapshot.Neutral)
                .Append("  contested ").Append(territorySnapshot.Contested);

            builder.AppendLine();
            builder.Append("<b>Groups</b>: ");
            AppendControlGroupStatus(builder, 2);
            AppendControlGroupStatus(builder, 3);
            AppendControlGroupStatus(builder, 4);
            AppendControlGroupStatus(builder, 5);

            builder.AppendLine();
            builder.Append("<b>Controls</b>: LMB unit/building select  |  Shift add  |  RMB move  |  1 select all  |  Ctrl+2-5 save  |  2-5 recall  |  F frame  |  Esc clear");

            return builder.ToString();
        }

        private void AppendControlGroupStatus(StringBuilder builder, int slot)
        {
            if (!IsControlGroupSlotEnabled(slot))
            {
                return;
            }

            int count = 0;
            if (controlGroups.TryGetValue(slot, out var group))
            {
                count = group.Count;
            }

            builder.Append(slot)
                .Append('[')
                .Append(count)
                .Append("]  ");
        }

        private FactionHudSnapshot GetControlledFactionSnapshot(EntityManager entityManager)
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
                if (!factions[i].FactionId.Equals(controlledFactionId))
                {
                    continue;
                }

                return new FactionHudSnapshot
                {
                    Found = true,
                    Gold = stockpiles[i].Gold,
                    Food = stockpiles[i].Food,
                    Water = stockpiles[i].Water,
                    Wood = stockpiles[i].Wood,
                    Stone = stockpiles[i].Stone,
                    Iron = stockpiles[i].Iron,
                    Influence = stockpiles[i].Influence,
                    PopulationTotal = populations[i].Total,
                    PopulationCap = populations[i].Cap,
                    PopulationAvailable = populations[i].Available,
                };
            }

            return default;
        }

        private TerritoryHudSnapshot GetTerritorySnapshot(EntityManager entityManager)
        {
            var snapshot = new TerritoryHudSnapshot();
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using var controlPoints = query.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            for (int i = 0; i < controlPoints.Length; i++)
            {
                var controlPoint = controlPoints[i];
                if (controlPoint.IsContested)
                {
                    snapshot.Contested++;
                    continue;
                }

                if (controlPoint.OwnerFactionId.Length == 0)
                {
                    snapshot.Neutral++;
                    continue;
                }

                if (controlPoint.OwnerFactionId.Equals(controlledFactionId))
                {
                    snapshot.Controlled++;
                }
                else
                {
                    snapshot.Hostile++;
                }
            }

            return snapshot;
        }

        private SelectionHudSnapshot GetSelectionSnapshot(EntityManager entityManager)
        {
            var snapshot = new SelectionHudSnapshot();

            for (int i = selectedEntities.Count - 1; i >= 0; i--)
            {
                var entity = selectedEntities[i];
                if (!entityManager.Exists(entity) || !entityManager.HasComponent<UnitTypeComponent>(entity))
                {
                    if (entityManager.Exists(entity) && entityManager.HasComponent<BuildingTypeComponent>(entity))
                    {
                        snapshot.BuildingsSelected++;
                    }
                    continue;
                }

                snapshot.TotalSelected++;
                var unitType = entityManager.GetComponentData<UnitTypeComponent>(entity);
                switch (unitType.Role)
                {
                    case UnitRole.Worker:
                        snapshot.Workers++;
                        break;
                    case UnitRole.Melee:
                    case UnitRole.MeleeRecon:
                    case UnitRole.UniqueMelee:
                        snapshot.Melee++;
                        break;
                    case UnitRole.Ranged:
                        snapshot.Ranged++;
                        break;
                    case UnitRole.LightCavalry:
                        snapshot.Cavalry++;
                        break;
                    case UnitRole.SiegeEngine:
                    case UnitRole.SiegeSupport:
                        snapshot.Siege++;
                        break;
                    case UnitRole.EngineerSpecialist:
                    case UnitRole.Support:
                        snapshot.Support++;
                        break;
                    default:
                        snapshot.Other++;
                        break;
                }
            }

            return snapshot;
        }

        private struct CommandUnit
        {
            public Entity Entity;
            public float3 Position;
            public float3 Destination;
            public float Lateral;
            public float Depth;
        }

        private struct FactionHudSnapshot
        {
            public bool Found;
            public float Gold;
            public float Food;
            public float Water;
            public float Wood;
            public float Stone;
            public float Iron;
            public float Influence;
            public int PopulationTotal;
            public int PopulationCap;
            public int PopulationAvailable;
        }

        private struct TerritoryHudSnapshot
        {
            public int Controlled;
            public int Hostile;
            public int Neutral;
            public int Contested;
        }

        private struct SelectionHudSnapshot
        {
            public int TotalSelected;
            public int BuildingsSelected;
            public int Workers;
            public int Melee;
            public int Ranged;
            public int Cavalry;
            public int Siege;
            public int Support;
            public int Other;
        }

        private struct SelectedProductionBuildingSnapshot
        {
            public Entity Entity;
            public string FactionId;
            public string TypeId;
            public string DisplayName;
            public BuildingDefinition Definition;
            public int QueueCount;
            public string QueueFrontDisplayName;
            public float QueueFrontRemainingSeconds;
        }

        private struct SelectedWorkerSnapshot
        {
            public Entity Entity;
            public string FactionId;
            public string DisplayName;
            public int WorkerCount;
        }

        private struct SelectedConstructionSiteSnapshot
        {
            public Entity Entity;
            public string FactionId;
            public string TypeId;
            public string DisplayName;
            public float RemainingSeconds;
            public float TotalSeconds;
            public float ProgressRatio;
            public float Health;
            public float MaxHealth;
        }

        private struct ProductionCommandOption
        {
            public string UnitId;
            public string Label;
            public string Detail;
            public bool Enabled;
        }

        private struct ConstructionCommandOption
        {
            public string BuildingId;
            public string Label;
            public string Detail;
            public bool Enabled;
        }

        private struct ProductionQueueEntryView
        {
            public int QueueIndex;
            public string Detail;
        }

        private struct FactionRuntimeSnapshot
        {
            public Entity Entity;
            public string FactionId;
            public string HouseId;
            public ResourceStockpileComponent Resources;
            public PopulationComponent Population;
            public FaithStateComponent FaithState;
        }
    }
}
