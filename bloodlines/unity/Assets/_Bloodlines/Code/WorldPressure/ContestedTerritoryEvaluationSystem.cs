using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Evaluates multi-hostile frontier pressure on owned control points.
    /// Browser contested-territory implementation was not present in the frozen
    /// spec surface, so this ECS slice follows the directive canon: two or more
    /// hostile factions inside claim radius create a live contested-territory
    /// component that downstream control-point loyalty logic can consume.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ControlPointCaptureSystem))]
    public partial struct ContestedTerritoryEvaluationSystem : ISystem
    {
        private const int MinimumContestingFactions = 2;
        private const float BaseStabilityPenaltyPerDay = 1.4f;
        private const float AdditionalStabilityPenaltyPerDay = 0.6f;
        private const float BaseLoyaltyVolatilityMultiplier = 1.15f;
        private const float AdditionalLoyaltyVolatilityMultiplier = 0.1f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ControlPointComponent>();
            state.RequireForUpdate<PositionComponent>();
            state.RequireForUpdate<UnitTypeComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float currentInWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factionIds =
                factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            var factionEntityLookup =
                new NativeHashMap<FixedString32Bytes, Entity>(factionEntities.Length + 1, Allocator.Temp);
            for (int i = 0; i < factionEntities.Length; i++)
            {
                factionEntityLookup.TryAdd(factionIds[i].FactionId, factionEntities[i]);
            }

            using var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using NativeArray<PositionComponent> unitPositions =
                unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using NativeArray<FactionComponent> unitFactions =
                unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<UnitTypeComponent> unitTypes =
                unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using NativeArray<HealthComponent> unitHealth =
                unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            var contestingFactions = new NativeList<FixedString32Bytes>(16, Allocator.Temp);

            foreach (var (controlPoint, position, entity) in
                SystemAPI.Query<RefRO<ControlPointComponent>, RefRO<PositionComponent>>()
                    .WithEntityAccess())
            {
                FixedString32Bytes ownerFactionId = controlPoint.ValueRO.OwnerFactionId;
                bool hadContest = entityManager.HasComponent<ContestedTerritoryComponent>(entity);

                if (ownerFactionId.Length == 0 ||
                    !factionEntityLookup.TryGetValue(ownerFactionId, out Entity ownerFactionEntity) ||
                    !entityManager.HasBuffer<HostilityComponent>(ownerFactionEntity))
                {
                    if (hadContest)
                    {
                        ecb.RemoveComponent<ContestedTerritoryComponent>(entity);
                    }

                    continue;
                }

                contestingFactions.Clear();
                DynamicBuffer<HostilityComponent> hostility =
                    entityManager.GetBuffer<HostilityComponent>(ownerFactionEntity);
                float claimRadiusSquared =
                    math.max(1f, controlPoint.ValueRO.RadiusTiles * controlPoint.ValueRO.RadiusTiles);

                for (int i = 0; i < unitPositions.Length; i++)
                {
                    if (unitHealth[i].Current <= 0f ||
                        unitTypes[i].Role == UnitRole.Worker)
                    {
                        continue;
                    }

                    FixedString32Bytes unitFactionId = unitFactions[i].FactionId;
                    if (unitFactionId.Equals(ownerFactionId) ||
                        !IsHostile(hostility, unitFactionId))
                    {
                        continue;
                    }

                    if (math.distancesq(unitPositions[i].Value, position.ValueRO.Value) > claimRadiusSquared ||
                        ContainsFaction(contestingFactions, unitFactionId))
                    {
                        continue;
                    }

                    contestingFactions.Add(unitFactionId);
                }

                if (contestingFactions.Length < MinimumContestingFactions)
                {
                    if (hadContest)
                    {
                        ecb.RemoveComponent<ContestedTerritoryComponent>(entity);
                    }

                    continue;
                }

                int contestingFactionCount = contestingFactions.Length;
                int additionalHostiles = math.max(0, contestingFactionCount - MinimumContestingFactions);
                var contestedTerritory = hadContest
                    ? entityManager.GetComponentData<ContestedTerritoryComponent>(entity)
                    : new ContestedTerritoryComponent
                    {
                        ContestStartedAtInWorldDays = currentInWorldDays,
                    };
                contestedTerritory.ContestingFactionCount = (byte)math.min(255, contestingFactionCount);
                contestedTerritory.StabilityPenaltyPerDay =
                    BaseStabilityPenaltyPerDay +
                    (additionalHostiles * AdditionalStabilityPenaltyPerDay);
                contestedTerritory.LoyaltyVolatilityMultiplier =
                    BaseLoyaltyVolatilityMultiplier +
                    (additionalHostiles * AdditionalLoyaltyVolatilityMultiplier);

                if (hadContest)
                {
                    ecb.SetComponent(entity, contestedTerritory);
                }
                else
                {
                    ecb.AddComponent(entity, contestedTerritory);
                }
            }

            ecb.Playback(entityManager);
            contestingFactions.Dispose();
            factionEntityLookup.Dispose();
        }

        private static bool IsHostile(
            DynamicBuffer<HostilityComponent> hostility,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < hostility.Length; i++)
            {
                if (hostility[i].HostileFactionId.Equals(factionId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsFaction(
            NativeList<FixedString32Bytes> factions,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].Equals(factionId))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
