using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Browser reference: simulation.js updateFaithExposure (8174) and
    /// getWayshrineExposureMultiplierAt (8246). Sacred sites grant exposure to
    /// nearby kingdom factions, while completed same-faction faith structures
    /// amplify the gain up to the canonical cap.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FaithExposureWalkerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<SacredSiteExposureSourceComponent>();
            state.RequireForUpdate<FaithStateComponent>();
            state.RequireForUpdate<UnitTypeComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            if (deltaTime <= 0f)
            {
                return;
            }

            var entityManager = state.EntityManager;
            EnsureFaithExposureStructureTags(entityManager);

            var sacredSiteQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SacredSiteExposureSourceComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>(),
                ComponentType.ReadOnly<FaithStateComponent>());
            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>());
            var structureQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FaithExposureStructureComponent>(),
                ComponentType.Exclude<ConstructionStateComponent>());

            using var sacredSites = sacredSiteQuery.ToComponentDataArray<SacredSiteExposureSourceComponent>(Allocator.Temp);
            using var sacredSitePositions = sacredSiteQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factions = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var factionKinds = factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var unitHealths = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var structureFactions = structureQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var structurePositions = structureQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var structureHealths = structureQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var structures = structureQuery.ToComponentDataArray<FaithExposureStructureComponent>(Allocator.Temp);

            if (sacredSites.Length == 0 || factionEntities.Length == 0 || unitFactions.Length == 0)
            {
                return;
            }

            for (int siteIndex = 0; siteIndex < sacredSites.Length; siteIndex++)
            {
                SacredSiteExposureSourceComponent sacredSite = sacredSites[siteIndex];
                if (sacredSite.Faith == CovenantId.None ||
                    sacredSite.ExposureRate <= 0f ||
                    sacredSite.RadiusWorldUnits <= 0f)
                {
                    continue;
                }

                var presentFlags = new NativeArray<byte>(factionEntities.Length, Allocator.Temp);
                try
                {
                    for (int unitIndex = 0; unitIndex < unitFactions.Length; unitIndex++)
                    {
                        if (unitHealths[unitIndex].Current <= 0f)
                        {
                            continue;
                        }

                        float distanceSq = math.distancesq(
                            unitPositions[unitIndex].Value.xz,
                            sacredSitePositions[siteIndex].Value.xz);
                        if (distanceSq > sacredSite.RadiusWorldUnits * sacredSite.RadiusWorldUnits)
                        {
                            continue;
                        }

                        int factionIndex = FindFactionIndex(
                            unitFactions[unitIndex].FactionId,
                            factions,
                            factionKinds);
                        if (factionIndex >= 0)
                        {
                            presentFlags[factionIndex] = 1;
                        }
                    }

                    for (int factionIndex = 0; factionIndex < factionEntities.Length; factionIndex++)
                    {
                        if (presentFlags[factionIndex] == 0 ||
                            factionKinds[factionIndex].Kind != FactionKind.Kingdom ||
                            !entityManager.HasBuffer<FaithExposureElement>(factionEntities[factionIndex]))
                        {
                            continue;
                        }

                        float multiplier = FaithExposureWalkerRules.ResolveStructureMultiplier(
                            factions[factionIndex].FactionId,
                            sacredSitePositions[siteIndex].Value,
                            structureFactions,
                            structurePositions,
                            structureHealths,
                            structures,
                            out _);
                        float exposureGain = sacredSite.ExposureRate * deltaTime * multiplier;
                        if (exposureGain <= 0f)
                        {
                            continue;
                        }

                        var exposureBuffer = entityManager.GetBuffer<FaithExposureElement>(factionEntities[factionIndex]);
                        FaithScoring.RecordExposure(exposureBuffer, sacredSite.Faith, exposureGain);
                    }
                }
                finally
                {
                    presentFlags.Dispose();
                }
            }
        }

        private static void EnsureFaithExposureStructureTags(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.Exclude<ConstructionStateComponent>(),
                ComponentType.Exclude<FaithExposureStructureComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            try
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    if (FaithExposureWalkerRules.TryResolveStructure(buildingTypes[i], out var structure))
                    {
                        ecb.AddComponent(entities[i], structure);
                    }
                }

                ecb.Playback(entityManager);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        private static int FindFactionIndex(
            FixedString32Bytes factionId,
            NativeArray<FactionComponent> factions,
            NativeArray<FactionKindComponent> factionKinds)
        {
            for (int i = 0; i < factions.Length; i++)
            {
                if (factionKinds[i].Kind == FactionKind.Kingdom &&
                    factions[i].FactionId.Equals(factionId))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
