using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Runtime fortification-structure seam for the live world. It mirrors the browser
    /// nearestSettlementForBuilding + structural-destruction path by:
    ///
    ///   1. linking fortification-role buildings to the nearest same-faction settlement
    ///      inside the fortification ecosystem radius
    ///   2. ensuring each linked structure exposes the tier contribution needed by the
    ///      existing AdvanceFortificationTierSystem
    ///   3. resolving destroyed structural counts and breach counts back onto the owning
    ///      settlement's FortificationComponent so later slices can read breach state
    ///
    /// Browser references:
    ///   simulation.js nearestSettlementForBuilding (11182)
    ///   simulation.js advanceFortificationTier (11227)
    ///   simulation.js applyDamage structural kill flow (7694-7755)
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AdvanceFortificationTierSystem))]
    public partial struct FortificationStructureLinkSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FortificationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<FortificationComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var settlementData = settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            using var settlementFactions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlementPositions = settlementQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            foreach (var (buildingType, faction, position, entity) in
                SystemAPI.Query<
                    RefRO<BuildingTypeComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>>()
                .WithEntityAccess())
            {
                bool hasLink = entityManager.HasComponent<FortificationSettlementLinkComponent>(entity);
                bool hasContribution = entityManager.HasComponent<FortificationBuildingContributionComponent>(entity);
                var role = buildingType.ValueRO.FortificationRole;

                if (role == FortificationRole.None)
                {
                    if (hasContribution)
                    {
                        ecb.RemoveComponent<FortificationBuildingContributionComponent>(entity);
                    }

                    if (hasLink)
                    {
                        ecb.RemoveComponent<FortificationSettlementLinkComponent>(entity);
                    }

                    continue;
                }

                var anchor = ResolveNearestSettlement(
                    settlementEntities,
                    settlementData,
                    settlementFactions,
                    settlementPositions,
                    faction.ValueRO.FactionId,
                    position.ValueRO.Value);

                if (anchor.SettlementEntity == Entity.Null)
                {
                    if (hasContribution)
                    {
                        ecb.RemoveComponent<FortificationBuildingContributionComponent>(entity);
                    }

                    if (hasLink)
                    {
                        ecb.RemoveComponent<FortificationSettlementLinkComponent>(entity);
                    }

                    continue;
                }

                var link = new FortificationSettlementLinkComponent
                {
                    SettlementEntity = anchor.SettlementEntity,
                    SettlementId = anchor.SettlementId,
                };

                if (hasLink)
                {
                    ecb.SetComponent(entity, link);
                }
                else
                {
                    ecb.AddComponent(entity, link);
                }

                var contribution = new FortificationBuildingContributionComponent
                {
                    TierContribution = ResolveTierContribution(role),
                };

                if (hasContribution)
                {
                    ecb.SetComponent(entity, contribution);
                }
                else
                {
                    ecb.AddComponent(entity, contribution);
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
        }

        private static SettlementAnchor ResolveNearestSettlement(
            NativeArray<Entity> settlementEntities,
            NativeArray<SettlementComponent> settlementData,
            NativeArray<FactionComponent> settlementFactions,
            NativeArray<PositionComponent> settlementPositions,
            FixedString32Bytes buildingFactionId,
            float3 buildingPosition)
        {
            float bestDistanceSq = float.MaxValue;
            var anchor = default(SettlementAnchor);
            float maxDistanceSq = FortificationCanon.EcosystemRadiusTiles * FortificationCanon.EcosystemRadiusTiles;

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                if (!settlementFactions[i].FactionId.Equals(buildingFactionId))
                {
                    continue;
                }

                float distanceSq = math.distancesq(settlementPositions[i].Value, buildingPosition);
                if (distanceSq > maxDistanceSq || distanceSq >= bestDistanceSq)
                {
                    continue;
                }

                bestDistanceSq = distanceSq;
                anchor = new SettlementAnchor
                {
                    SettlementEntity = settlementEntities[i],
                    SettlementId = settlementData[i].SettlementId,
                };
            }

            return anchor;
        }

        private static int ResolveTierContribution(FortificationRole role)
        {
            return role switch
            {
                FortificationRole.Wall => 1,
                FortificationRole.Tower => 1,
                FortificationRole.Gate => 2,
                FortificationRole.Keep => 2,
                _ => 0,
            };
        }

        private struct SettlementAnchor
        {
            public Entity SettlementEntity;
            public FixedString64Bytes SettlementId;
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AdvanceFortificationTierSystem))]
    public partial struct FortificationDestructionResolutionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FortificationComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var buildingLinks = buildingQuery.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            using var buildingHealth = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            foreach (var (fortificationRw, settlementEntity) in
                SystemAPI.Query<RefRW<FortificationComponent>>().WithEntityAccess())
            {
                int destroyedWalls = 0;
                int destroyedTowers = 0;
                int destroyedGates = 0;
                int destroyedKeeps = 0;

                for (int i = 0; i < buildingTypes.Length; i++)
                {
                    if (buildingLinks[i].SettlementEntity != settlementEntity ||
                        buildingTypes[i].FortificationRole == FortificationRole.None ||
                        buildingHealth[i].Current > 0f)
                    {
                        continue;
                    }

                    switch (buildingTypes[i].FortificationRole)
                    {
                        case FortificationRole.Wall:
                            destroyedWalls++;
                            break;
                        case FortificationRole.Tower:
                            destroyedTowers++;
                            break;
                        case FortificationRole.Gate:
                            destroyedGates++;
                            break;
                        case FortificationRole.Keep:
                            destroyedKeeps++;
                            break;
                    }
                }

                var fortification = fortificationRw.ValueRO;
                fortification.DestroyedWallSegmentCount = destroyedWalls;
                fortification.DestroyedTowerCount = destroyedTowers;
                fortification.DestroyedGateCount = destroyedGates;
                fortification.DestroyedKeepCount = destroyedKeeps;
                fortification.OpenBreachCount = destroyedWalls + destroyedGates;
                fortificationRw.ValueRW = fortification;
            }
        }
    }
}
