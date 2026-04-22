using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Victory;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects live victory progress into per-faction HUD buffers without mutating
    /// the underlying victory-condition systems.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct VictoryConditionReadoutSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 0.25f;

        private static readonly FixedString32Bytes CommandHallFallConditionId = new FixedString32Bytes("CommandHallFall");
        private static readonly FixedString32Bytes TerritorialGovernanceConditionId = new FixedString32Bytes("TerritorialGovernance");
        private static readonly FixedString32Bytes DivineRightConditionId = new FixedString32Bytes("DivineRight");
        private static readonly FixedString32Bytes PlayerFactionId = new FixedString32Bytes("player");
        private static readonly FixedString64Bytes CommandHallTypeId = new FixedString64Bytes("command_hall");

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VictoryStateComponent>();
            state.RequireForUpdate<FactionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (_, entity) in SystemAPI.Query<RefRO<FactionComponent>>().WithEntityAccess())
            {
                if (!entityManager.HasBuffer<VictoryConditionReadoutComponent>(entity))
                {
                    ecb.AddBuffer<VictoryConditionReadoutComponent>(entity);
                }

                if (!entityManager.HasComponent<VictoryConditionReadoutRefreshComponent>(entity))
                {
                    ecb.AddComponent(entity, new VictoryConditionReadoutRefreshComponent
                    {
                        LastRefreshInWorldDays = float.NaN,
                    });
                }
            }

            ecb.Playback(entityManager);

            float currentInWorldDays = 0f;
            float daysPerRealSecond = 0f;
            bool hasCurrentInWorldDays = TryResolveCurrentInWorldDays(entityManager, out currentInWorldDays, out daysPerRealSecond);
            bool requiresRefresh = !hasCurrentInWorldDays;

            using var factionQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            if (factionEntities.Length == 0)
            {
                return;
            }

            for (int i = 0; i < factionEntities.Length && !requiresRefresh; i++)
            {
                var refresh = entityManager.GetComponentData<VictoryConditionReadoutRefreshComponent>(factionEntities[i]);
                if (float.IsNaN(refresh.LastRefreshInWorldDays) ||
                    currentInWorldDays - refresh.LastRefreshInWorldDays >= RefreshCadenceInWorldDays)
                {
                    requiresRefresh = true;
                }
            }

            if (!requiresRefresh)
            {
                return;
            }

            var victory = SystemAPI.GetSingleton<VictoryStateComponent>();

            using var controlPointQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<ControlPointComponent> controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            using var faithQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FaithStateComponent>());
            using NativeArray<FactionComponent> faithFactions = faithQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<FaithStateComponent> faithStates = faithQuery.ToComponentDataArray<FaithStateComponent>(Allocator.Temp);

            using var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using NativeArray<Entity> buildingEntities = buildingQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<BuildingTypeComponent> buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using NativeArray<FactionComponent> buildingFactions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            NativeArray<float> territorialProgress = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            NativeArray<float> territorialRemainingDays = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            NativeArray<float> divineRightProgress = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            NativeArray<float> divineRightRemainingDays = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            NativeArray<float> commandHallFallProgress = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            NativeArray<float> commandHallFallRemainingDays = new NativeArray<float>(factionEntities.Length, Allocator.Temp);

            try
            {
                for (int i = 0; i < factionEntities.Length; i++)
                {
                    territorialRemainingDays[i] = float.NaN;
                    divineRightRemainingDays[i] = float.NaN;
                    commandHallFallRemainingDays[i] = float.NaN;

                    var factionId = factions[i].FactionId;
                    territorialProgress[i] = ResolveTerritorialGovernanceProgress(
                        factionId,
                        in victory,
                        controlPoints,
                        daysPerRealSecond,
                        out float territorialEtaDays);
                    territorialRemainingDays[i] = territorialEtaDays;

                    divineRightProgress[i] = ResolveDivineRightProgress(
                        factionId,
                        in victory,
                        faithFactions,
                        faithStates,
                        out float divineRightEtaDays);
                    divineRightRemainingDays[i] = divineRightEtaDays;

                    commandHallFallProgress[i] = ResolveCommandHallFallProgress(
                        factionId,
                        in victory,
                        entityManager,
                        buildingEntities,
                        buildingTypes,
                        buildingFactions,
                        out float commandHallEtaDays);
                    commandHallFallRemainingDays[i] = commandHallEtaDays;
                }

                float maxTerritorialProgress = MaxOf(territorialProgress);
                float maxDivineRightProgress = MaxOf(divineRightProgress);
                float maxCommandHallFallProgress = MaxOf(commandHallFallProgress);

                for (int i = 0; i < factionEntities.Length; i++)
                {
                    DynamicBuffer<VictoryConditionReadoutComponent> buffer =
                        entityManager.GetBuffer<VictoryConditionReadoutComponent>(factionEntities[i]);
                    buffer.Clear();
                    buffer.Add(new VictoryConditionReadoutComponent
                    {
                        ConditionId = TerritorialGovernanceConditionId,
                        ProgressPct = territorialProgress[i],
                        IsLeading = territorialProgress[i] > 0f && math.abs(territorialProgress[i] - maxTerritorialProgress) <= 0.0001f,
                        TimeRemainingEstimateInWorldDays = territorialRemainingDays[i],
                    });
                    buffer.Add(new VictoryConditionReadoutComponent
                    {
                        ConditionId = DivineRightConditionId,
                        ProgressPct = divineRightProgress[i],
                        IsLeading = divineRightProgress[i] > 0f && math.abs(divineRightProgress[i] - maxDivineRightProgress) <= 0.0001f,
                        TimeRemainingEstimateInWorldDays = divineRightRemainingDays[i],
                    });
                    buffer.Add(new VictoryConditionReadoutComponent
                    {
                        ConditionId = CommandHallFallConditionId,
                        ProgressPct = commandHallFallProgress[i],
                        IsLeading = commandHallFallProgress[i] > 0f && math.abs(commandHallFallProgress[i] - maxCommandHallFallProgress) <= 0.0001f,
                        TimeRemainingEstimateInWorldDays = commandHallFallRemainingDays[i],
                    });

                    var refresh = entityManager.GetComponentData<VictoryConditionReadoutRefreshComponent>(factionEntities[i]);
                    refresh.LastRefreshInWorldDays = hasCurrentInWorldDays ? currentInWorldDays : 0f;
                    entityManager.SetComponentData(factionEntities[i], refresh);
                }
            }
            finally
            {
                territorialProgress.Dispose();
                territorialRemainingDays.Dispose();
                divineRightProgress.Dispose();
                divineRightRemainingDays.Dispose();
                commandHallFallProgress.Dispose();
                commandHallFallRemainingDays.Dispose();
            }
        }

        private static bool TryResolveCurrentInWorldDays(
            EntityManager entityManager,
            out float currentInWorldDays,
            out float daysPerRealSecond)
        {
            currentInWorldDays = 0f;
            daysPerRealSecond = 0f;

            using var dualClockQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (!dualClockQuery.IsEmptyIgnoreFilter)
            {
                var dualClock = dualClockQuery.GetSingleton<DualClockComponent>();
                currentInWorldDays = dualClock.InWorldDays;
                daysPerRealSecond = dualClock.DaysPerRealSecond;
                return true;
            }

            using var matchQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MatchProgressionComponent>());
            if (!matchQuery.IsEmptyIgnoreFilter)
            {
                currentInWorldDays = matchQuery.GetSingleton<MatchProgressionComponent>().InWorldDays;
                return true;
            }

            return false;
        }

        private static float ResolveTerritorialGovernanceProgress(
            FixedString32Bytes factionId,
            in VictoryStateComponent victory,
            NativeArray<ControlPointComponent> controlPoints,
            float daysPerRealSecond,
            out float remainingInWorldDays)
        {
            remainingInWorldDays = float.NaN;

            if (victory.Status == MatchStatus.Won &&
                victory.VictoryType == VictoryConditionId.TerritorialGovernance &&
                victory.WinnerFactionId.Equals(factionId))
            {
                remainingInWorldDays = 0f;
                return 1f;
            }

            int heldCount = 0;
            int loyalCount = 0;
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(factionId))
                {
                    continue;
                }

                heldCount++;
                if (controlPoints[i].Loyalty >= VictoryConditionEvaluationSystem.TerritorialGovernanceLoyaltyThreshold)
                {
                    loyalCount++;
                }
            }

            if (heldCount <= 0)
            {
                return 0f;
            }

            float integrationProgress = (float)loyalCount / heldCount;
            if (loyalCount < heldCount)
            {
                return 0.5f * integrationProgress;
            }

            if (factionId.Equals(PlayerFactionId))
            {
                float holdProgress = math.saturate(
                    victory.TerritorialGovernanceHoldSeconds /
                    math.max(1f, VictoryConditionEvaluationSystem.TerritorialGovernanceVictorySeconds));
                if (daysPerRealSecond > 0f)
                {
                    float remainingSeconds = math.max(
                        0f,
                        VictoryConditionEvaluationSystem.TerritorialGovernanceVictorySeconds -
                        victory.TerritorialGovernanceHoldSeconds);
                    remainingInWorldDays = remainingSeconds * daysPerRealSecond;
                }

                return 0.5f + (0.5f * holdProgress);
            }

            return 0.5f;
        }

        private static float ResolveDivineRightProgress(
            FixedString32Bytes factionId,
            in VictoryStateComponent victory,
            NativeArray<FactionComponent> faithFactions,
            NativeArray<FaithStateComponent> faithStates,
            out float remainingInWorldDays)
        {
            remainingInWorldDays = float.NaN;

            if (victory.Status == MatchStatus.Won &&
                victory.VictoryType == VictoryConditionId.DivinRight &&
                victory.WinnerFactionId.Equals(factionId))
            {
                remainingInWorldDays = 0f;
                return 1f;
            }

            for (int i = 0; i < faithFactions.Length; i++)
            {
                if (!faithFactions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                float levelProgress = math.saturate(
                    faithStates[i].Level / (float)VictoryConditionEvaluationSystem.DivinRightFaithLevel);
                float intensityProgress = math.saturate(
                    faithStates[i].Intensity / math.max(1f, VictoryConditionEvaluationSystem.DivinRightIntensityThreshold));

                if (faithStates[i].Level >= VictoryConditionEvaluationSystem.DivinRightFaithLevel &&
                    faithStates[i].Intensity >= VictoryConditionEvaluationSystem.DivinRightIntensityThreshold)
                {
                    remainingInWorldDays = 0f;
                    return 1f;
                }

                return math.saturate((levelProgress + intensityProgress) * 0.5f);
            }

            return 0f;
        }

        private static float ResolveCommandHallFallProgress(
            FixedString32Bytes factionId,
            in VictoryStateComponent victory,
            EntityManager entityManager,
            NativeArray<Entity> buildingEntities,
            NativeArray<BuildingTypeComponent> buildingTypes,
            NativeArray<FactionComponent> buildingFactions,
            out float remainingInWorldDays)
        {
            remainingInWorldDays = float.NaN;

            if (victory.Status == MatchStatus.Won &&
                victory.VictoryType == VictoryConditionId.CommandHallFall &&
                victory.WinnerFactionId.Equals(factionId))
            {
                remainingInWorldDays = 0f;
                return 1f;
            }

            int hostileHallCount = 0;
            int hostileDeadHallCount = 0;
            for (int i = 0; i < buildingEntities.Length; i++)
            {
                if (!buildingTypes[i].TypeId.Equals(CommandHallTypeId) ||
                    buildingFactions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                hostileHallCount++;
                if (entityManager.HasComponent<DeadTag>(buildingEntities[i]))
                {
                    hostileDeadHallCount++;
                }
            }

            if (hostileHallCount <= 0)
            {
                return 0f;
            }

            if (hostileDeadHallCount >= hostileHallCount)
            {
                remainingInWorldDays = 0f;
            }

            return math.saturate((float)hostileDeadHallCount / hostileHallCount);
        }

        private static float MaxOf(NativeArray<float> values)
        {
            float maxValue = 0f;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > maxValue)
                {
                    maxValue = values[i];
                }
            }

            return maxValue;
        }
    }
}
