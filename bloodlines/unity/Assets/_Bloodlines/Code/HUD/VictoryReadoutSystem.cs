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
    /// Projects faction-scoped victory-distance telemetry into a HUD-owned
    /// read-model. The first half of territorial-governance progress represents
    /// integration coverage; the second half represents sovereignty-hold progress.
    /// Divine-right progress averages the current level and intensity thresholds.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(VictoryConditionEvaluationSystem))]
    public partial struct VictoryReadoutSystem : ISystem
    {
        private static readonly FixedString32Bytes PlayerFactionId = new FixedString32Bytes("player");
        private static readonly FixedString64Bytes CommandHallTypeId = new FixedString64Bytes("command_hall");

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VictoryStateComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var victoryState = SystemAPI.GetSingleton<VictoryStateComponent>();
            float daysPerSecond = ResolveDaysPerSecond(entityManager);

            using var ensureEcb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (faction, kind, entity) in SystemAPI.Query<
                         RefRO<FactionComponent>,
                         RefRO<FactionKindComponent>>()
                     .WithEntityAccess())
            {
                if (kind.ValueRO.Kind != FactionKind.Kingdom)
                {
                    continue;
                }

                if (!entityManager.HasComponent<VictoryReadoutComponent>(entity))
                {
                    ensureEcb.AddComponent(entity, new VictoryReadoutComponent
                    {
                        FactionId = faction.ValueRO.FactionId,
                    });
                }

                if (!entityManager.HasBuffer<VictoryConditionReadoutElement>(entity))
                {
                    ensureEcb.AddBuffer<VictoryConditionReadoutElement>(entity);
                }
            }

            ensureEcb.Playback(entityManager);

            using var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>());
            using var buildingFactions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);

            using var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<ControlPointComponent>());
            using var controlPointFactions = controlPointQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            foreach (var (faction, kind, entity) in SystemAPI.Query<
                         RefRO<FactionComponent>,
                         RefRO<FactionKindComponent>>()
                     .WithEntityAccess())
            {
                if (kind.ValueRO.Kind != FactionKind.Kingdom ||
                    !entityManager.HasComponent<VictoryReadoutComponent>(entity) ||
                    !entityManager.HasBuffer<VictoryConditionReadoutElement>(entity))
                {
                    continue;
                }

                var factionId = faction.ValueRO.FactionId;
                var summary = new VictoryReadoutComponent
                {
                    FactionId = factionId,
                    MatchStatus = victoryState.Status,
                    ResolvedVictoryType = victoryState.VictoryType,
                    IsWinner = victoryState.WinnerFactionId.Equals(factionId),
                };
                entityManager.SetComponentData(entity, summary);

                var buffer = entityManager.GetBuffer<VictoryConditionReadoutElement>(entity);
                buffer.Clear();
                buffer.Add(BuildCommandHallReadout(
                    factionId,
                    victoryState,
                    buildingFactions,
                    buildingTypes));
                buffer.Add(BuildTerritorialGovernanceReadout(
                    factionId,
                    victoryState,
                    controlPointFactions,
                    controlPoints,
                    daysPerSecond));
                buffer.Add(BuildDivineRightReadout(
                    entityManager,
                    entity,
                    factionId,
                    victoryState));
            }
        }

        private static float ResolveDaysPerSecond(EntityManager entityManager)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return 2f;
            }

            float value = query.GetSingleton<DualClockComponent>().DaysPerRealSecond;
            return value > 0f ? value : 2f;
        }

        private static VictoryConditionReadoutElement BuildCommandHallReadout(
            FixedString32Bytes factionId,
            in VictoryStateComponent victoryState,
            NativeArray<FactionComponent> buildingFactions,
            NativeArray<BuildingTypeComponent> buildingTypes)
        {
            int ownLiveHalls = 0;
            int rivalLiveHalls = 0;
            for (int i = 0; i < buildingTypes.Length; i++)
            {
                if (!buildingTypes[i].TypeId.Equals(CommandHallTypeId))
                {
                    continue;
                }

                if (buildingFactions[i].FactionId.Equals(factionId))
                {
                    ownLiveHalls++;
                }
                else
                {
                    rivalLiveHalls++;
                }
            }

            int totalRivalHalls = rivalLiveHalls > 0 ? rivalLiveHalls : 1;
            int destroyedRivalHalls = math.max(0, totalRivalHalls - rivalLiveHalls);
            var status = rivalLiveHalls <= 0 ? VictoryReadoutStatus.Ready : VictoryReadoutStatus.Building;
            if (ownLiveHalls <= 0)
            {
                status = VictoryReadoutStatus.Blocked;
            }

            float progress = totalRivalHalls > 0
                ? math.saturate((float)destroyedRivalHalls / totalRivalHalls)
                : 1f;

            if (victoryState.Status != MatchStatus.Playing &&
                victoryState.VictoryType == VictoryConditionId.CommandHallFall &&
                victoryState.WinnerFactionId.Equals(factionId))
            {
                status = VictoryReadoutStatus.Completed;
                progress = 1f;
            }

            return new VictoryConditionReadoutElement
            {
                ConditionId = VictoryConditionId.CommandHallFall,
                Status = status,
                Progress01 = progress,
                TimeRemainingInWorldDays = -1f,
                CurrentCount = destroyedRivalHalls,
                RequiredCount = totalRivalHalls,
            };
        }

        private static VictoryConditionReadoutElement BuildTerritorialGovernanceReadout(
            FixedString32Bytes factionId,
            in VictoryStateComponent victoryState,
            NativeArray<FactionComponent> controlPointFactions,
            NativeArray<ControlPointComponent> controlPoints,
            float daysPerSecond)
        {
            int held = 0;
            int loyalHeld = 0;
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (!controlPointFactions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                held++;
                if (controlPoints[i].Loyalty >= VictoryConditionEvaluationSystem.TerritorialGovernanceLoyaltyThreshold)
                {
                    loyalHeld++;
                }
            }

            float integrationProgress = held > 0 ? (float)loyalHeld / held : 0f;
            bool ready = held > 0 && loyalHeld == held;
            float holdProgress = 0f;
            float remainingDays = -1f;
            if (ready && factionId.Equals(PlayerFactionId))
            {
                holdProgress = math.saturate(
                    victoryState.TerritorialGovernanceHoldSeconds /
                    math.max(1f, VictoryConditionEvaluationSystem.TerritorialGovernanceVictorySeconds));
                float remainingSeconds = math.max(
                    0f,
                    VictoryConditionEvaluationSystem.TerritorialGovernanceVictorySeconds -
                    victoryState.TerritorialGovernanceHoldSeconds);
                remainingDays = remainingSeconds * daysPerSecond;
            }

            var status = held <= 0 ? VictoryReadoutStatus.Blocked : VictoryReadoutStatus.Building;
            float progress = ready ? 0.5f + (0.5f * holdProgress) : 0.5f * integrationProgress;
            if (ready)
            {
                status = VictoryReadoutStatus.Ready;
            }

            if (victoryState.Status != MatchStatus.Playing &&
                victoryState.VictoryType == VictoryConditionId.TerritorialGovernance &&
                victoryState.WinnerFactionId.Equals(factionId))
            {
                status = VictoryReadoutStatus.Completed;
                progress = 1f;
                remainingDays = 0f;
            }

            return new VictoryConditionReadoutElement
            {
                ConditionId = VictoryConditionId.TerritorialGovernance,
                Status = status,
                Progress01 = math.saturate(progress),
                TimeRemainingInWorldDays = remainingDays,
                CurrentCount = loyalHeld,
                RequiredCount = held,
            };
        }

        private static VictoryConditionReadoutElement BuildDivineRightReadout(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes factionId,
            in VictoryStateComponent victoryState)
        {
            if (!entityManager.HasComponent<FaithStateComponent>(factionEntity))
            {
                return new VictoryConditionReadoutElement
                {
                    ConditionId = VictoryConditionId.DivinRight,
                    Status = VictoryReadoutStatus.Blocked,
                    Progress01 = 0f,
                    TimeRemainingInWorldDays = -1f,
                    CurrentCount = 0,
                    RequiredCount = VictoryConditionEvaluationSystem.DivinRightFaithLevel,
                };
            }

            var faith = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            if (faith.SelectedFaith == CovenantId.None)
            {
                return new VictoryConditionReadoutElement
                {
                    ConditionId = VictoryConditionId.DivinRight,
                    Status = VictoryReadoutStatus.Blocked,
                    Progress01 = 0f,
                    TimeRemainingInWorldDays = -1f,
                    CurrentCount = 0,
                    RequiredCount = VictoryConditionEvaluationSystem.DivinRightFaithLevel,
                };
            }

            float levelProgress = math.saturate((float)faith.Level / VictoryConditionEvaluationSystem.DivinRightFaithLevel);
            float intensityProgress = math.saturate(faith.Intensity / VictoryConditionEvaluationSystem.DivinRightIntensityThreshold);
            bool ready = faith.Level >= VictoryConditionEvaluationSystem.DivinRightFaithLevel &&
                         faith.Intensity >= VictoryConditionEvaluationSystem.DivinRightIntensityThreshold;

            var status = ready ? VictoryReadoutStatus.Ready : VictoryReadoutStatus.Building;
            float progress = (levelProgress + intensityProgress) * 0.5f;
            if (victoryState.Status != MatchStatus.Playing &&
                victoryState.VictoryType == VictoryConditionId.DivinRight &&
                victoryState.WinnerFactionId.Equals(factionId))
            {
                status = VictoryReadoutStatus.Completed;
                progress = 1f;
            }

            return new VictoryConditionReadoutElement
            {
                ConditionId = VictoryConditionId.DivinRight,
                Status = status,
                Progress01 = math.saturate(progress),
                TimeRemainingInWorldDays = ready ? 0f : -1f,
                CurrentCount = math.min(faith.Level, VictoryConditionEvaluationSystem.DivinRightFaithLevel),
                RequiredCount = VictoryConditionEvaluationSystem.DivinRightFaithLevel,
            };
        }
    }
}
