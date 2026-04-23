using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Ports the territorial-governance recognition acceptance curve and the
    /// alliance-threshold coalition pressure seam from the browser runtime.
    /// Browser references:
    ///   - getTerritorialGovernanceAcceptanceProfile
    ///   - shouldIssueTerritorialGovernanceRecognition / tickTerritorialGovernanceRecognition
    ///   - getTerritorialGovernanceWorldPressureContribution
    ///   - governanceAlliancePressureActive block in tickRealmConditionCycle
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GovernorSpecializationSystem))]
    [UpdateBefore(typeof(WorldPressureEscalationSystem))]
    [UpdateBefore(typeof(Bloodlines.Victory.VictoryConditionEvaluationSystem))]
    public partial struct GovernanceCoalitionPressureSystem : ISystem
    {
        private const int TerritorialGovernanceMinStage = 5;
        private const float TerritorialGovernanceMinTerritoryShare = 0.35f;
        private const float TerritorialGovernanceLoyaltyThreshold = 80f;
        private const float TerritorialGovernanceVictoryLoyaltyThreshold = 90f;
        private const float TerritorialGovernanceBreakLoyaltyThreshold = 65f;
        private const float TerritorialGovernanceSustainSeconds = 90f;
        private const float TerritorialGovernanceVictorySeconds = 120f;
        private const float TerritorialGovernanceAcceptanceThresholdPct = 65f;
        private const float TerritorialGovernanceAcceptanceAllianceThresholdPct = 60f;
        private const float GovernanceAllianceLoyaltyPressureBase = -1.5f;
        private const float GovernanceAllianceLegitimacyPressurePerCycle = 0.8f;
        private const float GovernanceAllianceAcceptanceDragPerHostile = 0.04f;
        private const float GovernanceAllianceCycleSeconds = 90f;
        private const int TerritorialGovernanceWorldPressureScore = 3;
        private const int TerritorialGovernanceRecognizedWorldPressureScore = 5;
        private const int TerritorialGovernanceThresholdWorldPressureScore = 6;
        private const int TerritorialGovernanceVictoryWorldPressureScore = 7;

        private static readonly FixedString32Bytes PlayerFactionId = new FixedString32Bytes("player");
        private static readonly FixedString32Bytes PrimaryDynasticKeepClassId =
            new FixedString32Bytes("primary_dynastic_keep");

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MatchProgressionComponent>();
            state.RequireForUpdate<ControlPointComponent>();
            state.RequireForUpdate<DynastyStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
            {
                dt = 0.016f;
            }

            float currentInWorldDays = SystemAPI.HasSingleton<DualClockComponent>()
                ? SystemAPI.GetSingleton<DualClockComponent>().InWorldDays
                : 0f;
            int stageNumber = SystemAPI.GetSingleton<MatchProgressionComponent>().StageNumber;

            using NativeArray<Entity> kingdomEntities = EnsureRecognitionComponents(entityManager);
            if (kingdomEntities.Length == 0)
            {
                return;
            }

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>(),
                ComponentType.ReadWrite<TerritorialGovernanceRecognitionComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions =
                factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<FactionKindComponent> factionKinds =
                factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            using NativeArray<DynastyStateComponent> dynastyStates =
                factionQuery.ToComponentDataArray<DynastyStateComponent>(Allocator.Temp);

            using var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<Entity> controlPointEntities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<ControlPointComponent> controlPoints =
                controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            using var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>());
            using NativeArray<Entity> settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> settlementFactions =
                settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<SettlementComponent> settlements =
                settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);

            using var holyWarQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationHolyWarComponent>());
            using NativeArray<DynastyOperationComponent> holyWarOperations =
                holyWarQuery.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            using NativeArray<DynastyOperationHolyWarComponent> holyWarStates =
                holyWarQuery.ToComponentDataArray<DynastyOperationHolyWarComponent>(Allocator.Temp);

            var kingdomIdLookup = new NativeHashMap<FixedString32Bytes, byte>(factionEntities.Length + 1, Allocator.Temp);
            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind == FactionKind.Kingdom)
                {
                    kingdomIdLookup.TryAdd(factions[i].FactionId, 1);
                }
            }

            TerritorialGovernanceRecognitionComponent playerRecognition = default;
            bool hasPlayerRecognition = false;
            int totalTerritoryCount = math.max(1, controlPoints.Length);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom)
                {
                    continue;
                }

                Entity factionEntity = factionEntities[i];
                FixedString32Bytes factionId = factions[i].FactionId;
                var dynastyState = dynastyStates[i];
                TerritorialGovernanceRecognitionComponent recognition =
                    entityManager.GetComponentData<TerritorialGovernanceRecognitionComponent>(factionEntity);

                int territoryCount = 0;
                int loyalTerritoryCount = 0;
                int victoryLoyalTerritoryCount = 0;
                int contestedTerritoryCount = 0;
                int governedTerritoryCount = 0;
                bool allHeldTerritoriesLoyal = true;
                bool allHeldTerritoriesIntegrated = true;
                bool noHeldTerritoryBelowBreakFloor = true;
                bool foundWeakestTerritory = false;
                FixedString32Bytes weakestTerritoryId = default;
                float weakestTerritoryLoyalty = 100f;

                for (int j = 0; j < controlPoints.Length; j++)
                {
                    ControlPointComponent controlPoint = controlPoints[j];
                    if (!controlPoint.OwnerFactionId.Equals(factionId))
                    {
                        continue;
                    }

                    territoryCount++;
                    bool stabilized =
                        controlPoint.ControlState == ControlState.Stabilized &&
                        !controlPoint.IsContested &&
                        controlPoint.CaptureProgress <= 0f;
                    bool governed = entityManager.HasComponent<GovernorSeatAssignmentComponent>(controlPointEntities[j]);
                    bool loyal = stabilized && controlPoint.Loyalty >= TerritorialGovernanceLoyaltyThreshold;
                    bool victoryLoyal =
                        stabilized &&
                        controlPoint.Loyalty >= TerritorialGovernanceVictoryLoyaltyThreshold &&
                        governed;

                    if (governed)
                    {
                        governedTerritoryCount++;
                    }
                    if (loyal)
                    {
                        loyalTerritoryCount++;
                    }
                    if (victoryLoyal)
                    {
                        victoryLoyalTerritoryCount++;
                    }
                    if (controlPoint.IsContested || controlPoint.CaptureProgress > 0f)
                    {
                        contestedTerritoryCount++;
                    }
                    if (!loyal)
                    {
                        allHeldTerritoriesLoyal = false;
                    }
                    if (!victoryLoyal)
                    {
                        allHeldTerritoriesIntegrated = false;
                    }
                    if (controlPoint.Loyalty < TerritorialGovernanceBreakLoyaltyThreshold)
                    {
                        noHeldTerritoryBelowBreakFloor = false;
                    }
                    if (!foundWeakestTerritory || controlPoint.Loyalty < weakestTerritoryLoyalty)
                    {
                        foundWeakestTerritory = true;
                        weakestTerritoryId = controlPoint.ControlPointId;
                        weakestTerritoryLoyalty = controlPoint.Loyalty;
                    }
                }

                if (territoryCount == 0)
                {
                    allHeldTerritoriesLoyal = false;
                    allHeldTerritoriesIntegrated = false;
                }

                bool hasPrimarySeat = false;
                bool primarySeatGoverned = false;
                for (int j = 0; j < settlements.Length; j++)
                {
                    if (!settlementFactions[j].FactionId.Equals(factionId))
                    {
                        continue;
                    }

                    bool isPrimarySeat =
                        entityManager.HasComponent<PrimaryKeepTag>(settlementEntities[j]) ||
                        settlements[j].SettlementClassId.Equals(PrimaryDynasticKeepClassId);
                    if (!isPrimarySeat)
                    {
                        continue;
                    }

                    hasPrimarySeat = true;
                    primarySeatGoverned = entityManager.HasComponent<GovernorSeatAssignmentComponent>(settlementEntities[j]);
                    break;
                }

                bool seatCoverageReady =
                    territoryCount > 0 &&
                    governedTerritoryCount == territoryCount &&
                    (!hasPrimarySeat || primarySeatGoverned);

                bool warStateClear = true;
                for (int j = 0; j < holyWarOperations.Length; j++)
                {
                    DynastyOperationComponent operation = holyWarOperations[j];
                    if (!operation.Active)
                    {
                        continue;
                    }

                    if (!operation.SourceFactionId.Equals(factionId) &&
                        !operation.TargetFactionId.Equals(factionId))
                    {
                        continue;
                    }

                    if (currentInWorldDays < holyWarStates[j].WarExpiresAtInWorldDays)
                    {
                        warStateClear = false;
                        break;
                    }
                }

                int hostileKingdomCount = 0;
                if (entityManager.HasBuffer<HostilityComponent>(factionEntity))
                {
                    DynamicBuffer<HostilityComponent> hostility = entityManager.GetBuffer<HostilityComponent>(factionEntity);
                    for (int j = 0; j < hostility.Length; j++)
                    {
                        FixedString32Bytes hostileFactionId = hostility[j].HostileFactionId;
                        if (!hostileFactionId.Equals(factionId) &&
                            kingdomIdLookup.ContainsKey(hostileFactionId))
                        {
                            hostileKingdomCount++;
                        }
                    }
                }

                bool successionCrisisActive =
                    entityManager.HasComponent<SuccessionCrisisComponent>(factionEntity);
                bool antiRevoltReady = !dynastyState.Interregnum && !successionCrisisActive;

                float territoryShare = (float)territoryCount / totalTerritoryCount;
                float territorySharePct = math.round(territoryShare * 1000f) / 10f;
                bool stageReady = stageNumber >= TerritorialGovernanceMinStage;
                bool shareReady = territoryShare >= TerritorialGovernanceMinTerritoryShare;
                bool triggerReady = stageReady && shareReady && allHeldTerritoriesLoyal;
                bool holdReady =
                    triggerReady &&
                    noHeldTerritoryBelowBreakFloor &&
                    contestedTerritoryCount == 0 &&
                    warStateClear &&
                    seatCoverageReady &&
                    antiRevoltReady;
                bool integrationReady =
                    stageReady &&
                    shareReady &&
                    allHeldTerritoriesIntegrated &&
                    noHeldTerritoryBelowBreakFloor &&
                    contestedTerritoryCount == 0 &&
                    warStateClear &&
                    seatCoverageReady &&
                    antiRevoltReady;

                float legitimacy = math.clamp(dynastyState.Legitimacy, 0f, 100f);
                float worldPressureLevel = 0f;
                if (entityManager.HasComponent<WorldPressureComponent>(factionEntity))
                {
                    worldPressureLevel =
                        entityManager.GetComponentData<WorldPressureComponent>(factionEntity).Level;
                }

                CovenantId selectedFaith = CovenantId.None;
                float faithIntensity = 0f;
                if (entityManager.HasComponent<FaithStateComponent>(factionEntity))
                {
                    FaithStateComponent faith = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
                    selectedFaith = faith.SelectedFaith;
                    faithIntensity = faith.Intensity;
                }

                ConvictionBand convictionBand = ConvictionBand.Neutral;
                if (entityManager.HasComponent<ConvictionComponent>(factionEntity))
                {
                    convictionBand =
                        entityManager.GetComponentData<ConvictionComponent>(factionEntity).Band;
                }

                float population = 1f;
                float foodRatio = 1f;
                float waterRatio = 1f;
                if (entityManager.HasComponent<PopulationComponent>(factionEntity))
                {
                    population = math.max(1f, entityManager.GetComponentData<PopulationComponent>(factionEntity).Total);
                }
                if (entityManager.HasComponent<ResourceStockpileComponent>(factionEntity))
                {
                    ResourceStockpileComponent resources =
                        entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
                    foodRatio = resources.Food / population;
                    waterRatio = resources.Water / population;
                }

                float loyalShare = territoryCount > 0 ? (float)loyalTerritoryCount / territoryCount : 0f;
                float integratedShare = territoryCount > 0 ? (float)victoryLoyalTerritoryCount / territoryCount : 0f;
                float territoryShareNormalized = math.clamp(
                    territoryShare / math.max(0.0001f, TerritorialGovernanceMinTerritoryShare),
                    0f,
                    1f);
                float prosperitySupport =
                    math.clamp((foodRatio - 0.85f) / 0.55f, 0f, 1f) * 6f +
                    math.clamp((waterRatio - 0.85f) / 0.55f, 0f, 1f) * 6f;
                float territorySupport =
                    (loyalShare * 8f) +
                    (integratedShare * 10f) +
                    (territoryShareNormalized * 5f);
                float legitimacySupport = math.clamp(legitimacy / 100f, 0f, 1f) * 6f;
                float faithSupport =
                    selectedFaith != CovenantId.None
                        ? (faithIntensity >= 25f && faithIntensity <= 80f ? 2f : 1f)
                        : 0f;
                float integrationMomentum =
                    territoryCount > 0 && victoryLoyalTerritoryCount == territoryCount ? 6f : 0f;
                float convictionAcceptanceModifier = convictionBand switch
                {
                    ConvictionBand.ApexMoral => 4f,
                    ConvictionBand.Moral => 2f,
                    ConvictionBand.Cruel => -2f,
                    ConvictionBand.ApexCruel => -4f,
                    _ => 0f,
                };
                float conflictPenalty = math.min(
                    14f,
                    (contestedTerritoryCount * 4f) +
                    (hostileKingdomCount * 1.5f) +
                    (worldPressureLevel * 1.5f) +
                    (warStateClear ? 0f : 4f));
                float dynasticPenalty =
                    (successionCrisisActive ? 6f : 0f) +
                    (dynastyState.Interregnum ? 8f : 0f);
                float targetAcceptancePct = math.clamp(
                    14f +
                    territorySupport +
                    prosperitySupport +
                    legitimacySupport +
                    faithSupport +
                    integrationMomentum +
                    convictionAcceptanceModifier -
                    conflictPenalty -
                    dynasticPenalty,
                    0f,
                    100f);
                float seededAcceptancePct = math.clamp(
                    targetAcceptancePct - 10f,
                    40f,
                    math.min(62f, targetAcceptancePct));
                float currentAcceptancePct =
                    recognition.Active || recognition.Completed
                        ? recognition.PopulationAcceptancePct
                        : seededAcceptancePct;
                currentAcceptancePct = math.clamp(currentAcceptancePct, 0f, 100f);
                float growthMomentum =
                    (prosperitySupport >= 10f ? 0.15f : 0f) +
                    (legitimacySupport >= 4f ? 0.1f : 0f) +
                    (seatCoverageReady ? 0.1f : 0f);
                bool atOrAboveAllianceThreshold =
                    currentAcceptancePct >= TerritorialGovernanceAcceptanceAllianceThresholdPct;
                float allianceDrag =
                    atOrAboveAllianceThreshold
                        ? hostileKingdomCount * GovernanceAllianceAcceptanceDragPerHostile
                        : 0f;
                float decayPressure =
                    (worldPressureLevel >= 2f ? 0.1f : 0f) +
                    (contestedTerritoryCount > 0 ? 0.08f : 0f) +
                    (warStateClear ? 0f : 0.08f) +
                    allianceDrag;
                float riseRate = math.clamp(0.35f + growthMomentum - decayPressure, 0.2f, 0.95f);
                float fallRate = math.clamp(
                    0.55f +
                    (contestedTerritoryCount * 0.08f) +
                    (hostileKingdomCount * 0.03f) +
                    (successionCrisisActive ? 0.16f : 0f) +
                    (dynastyState.Interregnum ? 0.2f : 0f),
                    0.45f,
                    1.25f);
                float acceptanceDelta = targetAcceptancePct - currentAcceptancePct;
                float acceptanceRate = acceptanceDelta >= 0f ? riseRate : fallRate;
                float nextAcceptancePct = math.clamp(
                    currentAcceptancePct +
                    (math.sign(acceptanceDelta) * math.min(math.abs(acceptanceDelta), acceptanceRate * dt)),
                    0f,
                    100f);
                float trendPerSecond =
                    acceptanceDelta > 0.05f
                        ? riseRate
                        : acceptanceDelta < -0.05f
                            ? -fallRate
                            : 0f;

                if (!recognition.Active && triggerReady)
                {
                    recognition.Active = true;
                    recognition.RecognitionEstablished = false;
                    recognition.Completed = false;
                    recognition.StartedAtInWorldDays = currentInWorldDays;
                    recognition.RecognizedAtInWorldDays = 0f;
                    recognition.CompletedAtInWorldDays = 0f;
                    recognition.RequiredSustainSeconds = TerritorialGovernanceSustainSeconds;
                    recognition.SustainedSeconds = 0f;
                    recognition.RequiredVictorySeconds = TerritorialGovernanceVictorySeconds;
                    recognition.VictoryHoldSeconds = 0f;
                    recognition.AlliancePressureAccumulatorSeconds = 0f;
                    currentAcceptancePct = seededAcceptancePct;
                    nextAcceptancePct = seededAcceptancePct;
                }

                recognition.StageReady = stageReady;
                recognition.ShareReady = shareReady;
                recognition.TriggerReady = triggerReady;
                recognition.HoldReady = holdReady;
                recognition.IntegrationReady = integrationReady;
                recognition.TerritoryCount = territoryCount;
                recognition.LoyalTerritoryCount = loyalTerritoryCount;
                recognition.VictoryLoyalTerritoryCount = victoryLoyalTerritoryCount;
                recognition.ContestedTerritoryCount = contestedTerritoryCount;
                recognition.TerritoryShare = territoryShare;
                recognition.TerritorySharePct = territorySharePct;
                recognition.PopulationAcceptanceSeedPct = seededAcceptancePct;
                recognition.PopulationAcceptanceTargetPct = math.round(targetAcceptancePct * 10f) / 10f;
                recognition.PopulationAcceptanceThresholdPct = TerritorialGovernanceAcceptanceThresholdPct;
                recognition.PopulationAcceptanceAllianceThresholdPct =
                    TerritorialGovernanceAcceptanceAllianceThresholdPct;
                recognition.PopulationAcceptanceRiseRate = math.round(riseRate * 10f) / 10f;
                recognition.PopulationAcceptanceFallRate = math.round(fallRate * 10f) / 10f;
                recognition.PopulationAcceptanceTrendPerSecond = math.round(trendPerSecond * 10f) / 10f;
                recognition.WeakestControlPointId = weakestTerritoryId;
                recognition.WeakestControlPointLoyalty = math.round(weakestTerritoryLoyalty);

                if (recognition.Active)
                {
                    recognition.PopulationAcceptancePct = math.round(nextAcceptancePct * 10f) / 10f;

                    if (!triggerReady)
                    {
                        BreakRecognition(ref recognition);
                    }
                    else
                    {
                        if (holdReady)
                        {
                            recognition.SustainedSeconds = math.min(
                                recognition.RequiredSustainSeconds,
                                recognition.SustainedSeconds + dt);
                            if (!recognition.RecognitionEstablished &&
                                recognition.SustainedSeconds >= recognition.RequiredSustainSeconds)
                            {
                                recognition.RecognitionEstablished = true;
                                recognition.RecognizedAtInWorldDays = currentInWorldDays;
                            }
                        }
                        else if (!recognition.RecognitionEstablished)
                        {
                            recognition.SustainedSeconds = 0f;
                        }
                        else
                        {
                            BreakRecognition(ref recognition);
                        }

                        if (recognition.Active &&
                            recognition.RecognitionEstablished &&
                            !recognition.Completed)
                        {
                            if (integrationReady &&
                                recognition.PopulationAcceptancePct >= TerritorialGovernanceAcceptanceThresholdPct)
                            {
                                recognition.VictoryHoldSeconds = math.min(
                                    recognition.RequiredVictorySeconds,
                                    recognition.VictoryHoldSeconds + dt);
                                if (recognition.VictoryHoldSeconds >= recognition.RequiredVictorySeconds)
                                {
                                    recognition.Completed = true;
                                    recognition.CompletedAtInWorldDays = currentInWorldDays;
                                }
                            }
                            else
                            {
                                recognition.VictoryHoldSeconds = 0f;
                            }
                        }
                    }
                }
                else
                {
                    recognition.RecognitionEstablished = false;
                    recognition.Completed = false;
                    recognition.RequiredSustainSeconds = TerritorialGovernanceSustainSeconds;
                    recognition.RequiredVictorySeconds = TerritorialGovernanceVictorySeconds;
                    recognition.SustainedSeconds = 0f;
                    recognition.VictoryHoldSeconds = 0f;
                    recognition.AlliancePressureAccumulatorSeconds = 0f;
                    recognition.PopulationAcceptancePct = math.round(seededAcceptancePct * 10f) / 10f;
                }

                recognition.PopulationAcceptanceReady =
                    recognition.PopulationAcceptancePct >= TerritorialGovernanceAcceptanceThresholdPct;
                recognition.AllianceThresholdReady =
                    recognition.PopulationAcceptancePct >= TerritorialGovernanceAcceptanceAllianceThresholdPct;
                recognition.VictoryReady =
                    recognition.IntegrationReady &&
                    recognition.PopulationAcceptanceReady;
                recognition.PopulationAcceptanceGapPct = math.round(
                    math.max(0f, TerritorialGovernanceAcceptanceThresholdPct - recognition.PopulationAcceptancePct) *
                    10f) / 10f;
                recognition.AlliancePressureDrag = math.round(allianceDrag * 1000f) / 1000f;
                recognition.AlliancePressureHostileCount =
                    recognition.AllianceThresholdReady ? hostileKingdomCount : 0;
                recognition.AlliancePressureActive =
                    recognition.Active &&
                    !recognition.Completed &&
                    recognition.AllianceThresholdReady &&
                    hostileKingdomCount > 0;

                if (recognition.AlliancePressureActive)
                {
                    recognition.AlliancePressureAccumulatorSeconds += dt;
                    while (recognition.AlliancePressureAccumulatorSeconds >= GovernanceAllianceCycleSeconds)
                    {
                        recognition.AlliancePressureAccumulatorSeconds -= GovernanceAllianceCycleSeconds;
                        ApplyCoalitionPressure(
                            entityManager,
                            factionEntity,
                            factionId,
                            controlPointEntities,
                            controlPoints,
                            hostileKingdomCount,
                            ref recognition);
                    }
                }
                else if (!recognition.Completed)
                {
                    recognition.AlliancePressureAccumulatorSeconds = 0f;
                }

                recognition.WorldPressureContribution = ResolveWorldPressureContribution(recognition);
                entityManager.SetComponentData(factionEntity, recognition);

                if (factionId.Equals(PlayerFactionId))
                {
                    playerRecognition = recognition;
                    hasPlayerRecognition = true;
                }
            }

            kingdomIdLookup.Dispose();

            WriteAIGovernanceFlags(entityManager, hasPlayerRecognition, playerRecognition);
        }

        private static NativeArray<Entity> EnsureRecognitionComponents(EntityManager entityManager)
        {
            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
            using NativeArray<Entity> entities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionKindComponent> kinds =
                factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            bool modified = false;
            for (int i = 0; i < entities.Length; i++)
            {
                if (kinds[i].Kind != FactionKind.Kingdom ||
                    entityManager.HasComponent<TerritorialGovernanceRecognitionComponent>(entities[i]))
                {
                    continue;
                }

                ecb.AddComponent(entities[i], CreateDefaultRecognitionState());
                modified = true;
            }

            if (modified)
            {
                ecb.Playback(entityManager);
            }

            return entities;
        }

        private static TerritorialGovernanceRecognitionComponent CreateDefaultRecognitionState()
        {
            return new TerritorialGovernanceRecognitionComponent
            {
                RequiredSustainSeconds = TerritorialGovernanceSustainSeconds,
                RequiredVictorySeconds = TerritorialGovernanceVictorySeconds,
                PopulationAcceptanceThresholdPct = TerritorialGovernanceAcceptanceThresholdPct,
                PopulationAcceptanceAllianceThresholdPct =
                    TerritorialGovernanceAcceptanceAllianceThresholdPct,
            };
        }

        private static void BreakRecognition(ref TerritorialGovernanceRecognitionComponent recognition)
        {
            recognition.Active = false;
            recognition.RecognitionEstablished = false;
            recognition.Completed = false;
            recognition.StartedAtInWorldDays = 0f;
            recognition.RecognizedAtInWorldDays = 0f;
            recognition.CompletedAtInWorldDays = 0f;
            recognition.SustainedSeconds = 0f;
            recognition.VictoryHoldSeconds = 0f;
            recognition.AlliancePressureActive = false;
            recognition.AlliancePressureHostileCount = 0;
            recognition.AlliancePressureDrag = 0f;
            recognition.AlliancePressureAccumulatorSeconds = 0f;
            recognition.WorldPressureContribution = 0;
        }

        private static int ResolveWorldPressureContribution(
            in TerritorialGovernanceRecognitionComponent recognition)
        {
            if (!recognition.Active && !recognition.Completed)
            {
                return 0;
            }

            if (recognition.VictoryReady || recognition.Completed)
            {
                return TerritorialGovernanceVictoryWorldPressureScore;
            }

            if (recognition.IntegrationReady || recognition.AllianceThresholdReady)
            {
                return TerritorialGovernanceThresholdWorldPressureScore;
            }

            if (recognition.RecognitionEstablished)
            {
                return TerritorialGovernanceRecognizedWorldPressureScore;
            }

            return TerritorialGovernanceWorldPressureScore;
        }

        private static void ApplyCoalitionPressure(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes factionId,
            NativeArray<Entity> controlPointEntities,
            NativeArray<ControlPointComponent> controlPoints,
            int hostileKingdomCount,
            ref TerritorialGovernanceRecognitionComponent recognition)
        {
            int weakestGovernedIndex = -1;
            float weakestGovernedLoyalty = float.MaxValue;
            for (int i = 0; i < controlPoints.Length; i++)
            {
                ControlPointComponent controlPoint = controlPoints[i];
                if (!controlPoint.OwnerFactionId.Equals(factionId) ||
                    controlPoint.ControlState != ControlState.Stabilized)
                {
                    continue;
                }

                if (controlPoint.Loyalty < weakestGovernedLoyalty)
                {
                    weakestGovernedLoyalty = controlPoint.Loyalty;
                    weakestGovernedIndex = i;
                }
            }

            if (weakestGovernedIndex >= 0)
            {
                ControlPointComponent weakenedPoint = controlPoints[weakestGovernedIndex];
                float loyaltyPressure = GovernanceAllianceLoyaltyPressureBase * hostileKingdomCount;
                weakenedPoint.Loyalty = math.max(0f, weakenedPoint.Loyalty + loyaltyPressure);
                entityManager.SetComponentData(controlPointEntities[weakestGovernedIndex], weakenedPoint);
                controlPoints[weakestGovernedIndex] = weakenedPoint;
                recognition.WeakestControlPointId = weakenedPoint.ControlPointId;
                recognition.WeakestControlPointLoyalty = math.round(weakenedPoint.Loyalty);
            }

            DynastyStateComponent dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            float legitimacyPressure =
                GovernanceAllianceLegitimacyPressurePerCycle * hostileKingdomCount;
            dynasty.Legitimacy = math.max(0f, dynasty.Legitimacy - legitimacyPressure);
            entityManager.SetComponentData(factionEntity, dynasty);

            recognition.AlliancePressureCycles += 1;
        }

        private static void WriteAIGovernanceFlags(
            EntityManager entityManager,
            bool hasPlayerRecognition,
            TerritorialGovernanceRecognitionComponent playerRecognition)
        {
            using var aiQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<AIStrategyComponent>());
            using NativeArray<Entity> aiEntities = aiQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> aiFactions =
                aiQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<AIStrategyComponent> strategies =
                aiQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);

            bool playerGovernanceActive =
                hasPlayerRecognition && playerRecognition.Active && !playerRecognition.Completed;
            bool playerGovernanceVictoryPressure =
                playerGovernanceActive && playerRecognition.VictoryReady;
            bool playerGovernanceAlliancePressure =
                playerGovernanceActive &&
                !playerGovernanceVictoryPressure &&
                (playerRecognition.IntegrationReady || playerRecognition.AllianceThresholdReady);

            for (int i = 0; i < aiEntities.Length; i++)
            {
                AIStrategyComponent strategy = strategies[i];
                strategy.PlayerGovernanceActive = playerGovernanceActive;
                strategy.PlayerGovernanceVictoryPressure = playerGovernanceVictoryPressure;
                strategy.PlayerGovernanceAlliancePressure = playerGovernanceAlliancePressure;

                bool enemyGovernanceActive = false;
                bool enemyGovernanceVictoryPressure = false;
                bool enemyGovernanceAlliancePressure = false;
                bool enemyGovernanceHasTargetPoint = false;
                if (entityManager.HasComponent<TerritorialGovernanceRecognitionComponent>(aiEntities[i]))
                {
                    TerritorialGovernanceRecognitionComponent ownRecognition =
                        entityManager.GetComponentData<TerritorialGovernanceRecognitionComponent>(aiEntities[i]);
                    enemyGovernanceActive = ownRecognition.Active && !ownRecognition.Completed;
                    enemyGovernanceVictoryPressure =
                        enemyGovernanceActive && ownRecognition.VictoryReady;
                    enemyGovernanceAlliancePressure =
                        enemyGovernanceActive &&
                        !enemyGovernanceVictoryPressure &&
                        (ownRecognition.IntegrationReady || ownRecognition.AllianceThresholdReady);
                    enemyGovernanceHasTargetPoint = ownRecognition.WeakestControlPointId.Length > 0;
                }

                strategy.EnemyGovernanceActive = enemyGovernanceActive;
                strategy.EnemyGovernanceVictoryPressure = enemyGovernanceVictoryPressure;
                strategy.EnemyGovernanceAlliancePressure = enemyGovernanceAlliancePressure;
                strategy.EnemyGovernanceHasTargetPoint = enemyGovernanceHasTargetPoint;
                entityManager.SetComponentData(aiEntities[i], strategy);
            }
        }
    }
}
