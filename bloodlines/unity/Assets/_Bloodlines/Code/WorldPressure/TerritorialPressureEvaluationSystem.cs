using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Builds the faction-level contested-territory read-model without mutating
    /// world-pressure score sources or capture resolution behavior.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GovernanceCoalitionPressureSystem))]
    [UpdateBefore(typeof(WorldPressureEscalationSystem))]
    public partial struct TerritorialPressureEvaluationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<FactionKindComponent>();
            state.RequireForUpdate<ControlPointComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;
            EntityQuery factionQuery = SystemAPI.QueryBuilder()
                .WithAll<FactionComponent, FactionKindComponent>()
                .Build();
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions =
                factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<FactionKindComponent> factionKinds =
                factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);

            if (factionEntities.Length == 0)
            {
                return;
            }

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            int kingdomCount = 0;
            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom)
                {
                    continue;
                }

                kingdomCount++;
                if (!entityManager.HasComponent<TerritorialPressureComponent>(factionEntities[i]))
                {
                    ecb.AddComponent(factionEntities[i], new TerritorialPressureComponent());
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();

            if (kingdomCount == 0)
            {
                return;
            }

            EntityQuery controlPointQuery = SystemAPI.QueryBuilder()
                .WithAll<ControlPointComponent>()
                .Build();
            using NativeArray<ControlPointComponent> controlPoints =
                controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom)
                {
                    continue;
                }

                FixedString32Bytes factionId = factions[i].FactionId;
                int externalContestedTerritoryCount = 0;
                int ownedContestedTerritoryCount = 0;
                bool foundWeakestOwnedContested = false;
                FixedString32Bytes weakestOwnedContestedControlPointId = default;
                float weakestOwnedContestedLoyalty = 100f;

                for (int j = 0; j < controlPoints.Length; j++)
                {
                    ControlPointComponent controlPoint = controlPoints[j];
                    bool contested = controlPoint.IsContested || controlPoint.CaptureProgress > 0f;

                    if (controlPoint.OwnerFactionId.Equals(factionId))
                    {
                        if (!contested)
                        {
                            continue;
                        }

                        ownedContestedTerritoryCount++;
                        if (!foundWeakestOwnedContested ||
                            controlPoint.Loyalty < weakestOwnedContestedLoyalty)
                        {
                            foundWeakestOwnedContested = true;
                            weakestOwnedContestedControlPointId = controlPoint.ControlPointId;
                            weakestOwnedContestedLoyalty = controlPoint.Loyalty;
                        }

                        continue;
                    }

                    if (controlPoint.CaptureProgress > 0f)
                    {
                        externalContestedTerritoryCount++;
                    }
                }

                bool governanceContestBlockingActive = false;
                if (entityManager.HasComponent<TerritorialGovernanceRecognitionComponent>(factionEntities[i]))
                {
                    TerritorialGovernanceRecognitionComponent recognition =
                        entityManager.GetComponentData<TerritorialGovernanceRecognitionComponent>(
                            factionEntities[i]);
                    governanceContestBlockingActive =
                        recognition.StageReady &&
                        recognition.ShareReady &&
                        recognition.ContestedTerritoryCount > 0;
                }

                entityManager.SetComponentData(factionEntities[i], new TerritorialPressureComponent
                {
                    ExternalContestedTerritoryCount = externalContestedTerritoryCount,
                    OwnedContestedTerritoryCount = ownedContestedTerritoryCount,
                    WeakestOwnedContestedControlPointId = weakestOwnedContestedControlPointId,
                    WeakestOwnedContestedLoyalty = foundWeakestOwnedContested
                        ? weakestOwnedContestedLoyalty
                        : 0f,
                    GovernanceContestBlockingActive = governanceContestBlockingActive,
                });
            }
        }
    }
}
