using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Economy
{
    /// <summary>
    /// Evaluates trade routes once per in-world day. For each faction, counts unique pairs
    /// of adjacent uncontested owned control points. Two control points are adjacent when
    /// the distance between them is at most the sum of their radii plus a 2-tile padding.
    /// Each qualifying pair contributes 5 gold per day to the faction stockpile.
    /// Writes a TradeRouteComponent summary to each faction entity.
    ///
    /// Browser equivalent: absent -- implemented from canonical trade route design.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct TradeRouteEvaluationSystem : ISystem
    {
        const float GoldPerRoutePerDay = 5f;
        const float AdjacencyPaddingTiles = 2f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<ControlPointComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var clock = SystemAPI.GetSingleton<DualClockComponent>();
            float currentDay = math.floor(clock.InWorldDays);

            var cpQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var cpComponents = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var cpPositions = cpQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            cpQuery.Dispose();

            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<ResourceStockpileComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionComponents = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            factionQuery.Dispose();

            for (int f = 0; f < factionEntities.Length; f++)
            {
                Entity factionEntity = factionEntities[f];
                var faction = factionComponents[f];

                if (em.HasComponent<TradeRouteComponent>(factionEntity))
                {
                    float lastUpdated = em.GetComponentData<TradeRouteComponent>(factionEntity).LastUpdatedAtInWorldDays;
                    if (lastUpdated >= currentDay)
                    {
                        continue;
                    }
                }

                var eligibleIndices = new NativeList<int>(Allocator.Temp);
                for (int c = 0; c < cpComponents.Length; c++)
                {
                    var cp = cpComponents[c];
                    if (cp.OwnerFactionId.Equals(faction.FactionId) && !cp.IsContested)
                    {
                        eligibleIndices.Add(c);
                    }
                }

                int routeCount = 0;
                for (int i = 0; i < eligibleIndices.Length; i++)
                {
                    int idxA = eligibleIndices[i];
                    float3 posA = cpPositions[idxA].Value;
                    float radiusA = cpComponents[idxA].RadiusTiles;

                    for (int j = i + 1; j < eligibleIndices.Length; j++)
                    {
                        int idxB = eligibleIndices[j];
                        float3 posB = cpPositions[idxB].Value;
                        float radiusB = cpComponents[idxB].RadiusTiles;

                        if (math.distance(posA, posB) <= radiusA + radiusB + AdjacencyPaddingTiles)
                        {
                            routeCount++;
                        }
                    }
                }

                eligibleIndices.Dispose();

                float goldYield = routeCount * GoldPerRoutePerDay;

                var tradeRoute = new TradeRouteComponent
                {
                    ActiveRouteCount = routeCount,
                    TotalGoldPerTickFromTrades = goldYield,
                    LastUpdatedAtInWorldDays = currentDay,
                };

                if (em.HasComponent<TradeRouteComponent>(factionEntity))
                {
                    em.SetComponentData(factionEntity, tradeRoute);
                }
                else
                {
                    em.AddComponentData(factionEntity, tradeRoute);
                }

                if (goldYield > 0f)
                {
                    var stockpile = em.GetComponentData<ResourceStockpileComponent>(factionEntity);
                    stockpile.Gold += goldYield;
                    em.SetComponentData(factionEntity, stockpile);
                }
            }
        }
    }
}
