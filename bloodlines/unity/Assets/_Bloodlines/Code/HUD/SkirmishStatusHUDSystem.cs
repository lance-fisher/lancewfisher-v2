using Bloodlines.Components;
using Bloodlines.Economy;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects live simulation state into the skirmish match status panel.
    /// Aggregates per-faction unit counts, territory counts, gold, and trade routes.
    /// Runs every 0.25 in-world days in the PresentationSystemGroup.
    ///
    /// Browser equivalent: absent -- implemented from canonical skirmish status design.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct SkirmishStatusHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 0.25f;
        private static readonly FixedString32Bytes PlayerFactionId = new("player");

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            Entity singleton = EnsureSingleton(em);

            var header = em.GetComponentData<SkirmishStatusHUDComponent>(singleton);
            if (!ShouldRefresh(em, header.LastRefreshInWorldDays, out float currentInWorldDays))
            {
                return;
            }

            // Gather faction entities.
            using var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<ResourceStockpileComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionComponents = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = factionQuery.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);

            // Count units per faction.
            using var unitQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            // Count territory (owned non-neutral CPs) per faction.
            using var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using var cpComponents = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            int factionCount = factionComponents.Length;
            var rows = new NativeArray<SkirmishStatusFactionRowHUDComponent>(factionCount, Allocator.Temp);

            int totalUnits = 0;
            int totalTerritory = 0;

            for (int f = 0; f < factionCount; f++)
            {
                var factionId = factionComponents[f].FactionId;

                int unitCount = 0;
                for (int u = 0; u < unitFactions.Length; u++)
                {
                    if (unitFactions[u].FactionId.Equals(factionId))
                    {
                        unitCount++;
                    }
                }

                int territoryCount = 0;
                for (int c = 0; c < cpComponents.Length; c++)
                {
                    var cp = cpComponents[c];
                    if (cp.OwnerFactionId.Equals(factionId) &&
                        cp.ControlState != ControlState.Neutral)
                    {
                        territoryCount++;
                    }
                }

                int tradeRouteCount = 0;
                float tradeGoldPerDay = 0f;
                Entity factionEntity = factionEntities[f];
                if (em.HasComponent<TradeRouteComponent>(factionEntity))
                {
                    var tr = em.GetComponentData<TradeRouteComponent>(factionEntity);
                    tradeRouteCount = tr.ActiveRouteCount;
                    tradeGoldPerDay = tr.TotalGoldPerTickFromTrades;
                }

                rows[f] = new SkirmishStatusFactionRowHUDComponent
                {
                    FactionId = factionId,
                    Rank = 0,
                    UnitCount = unitCount,
                    TerritoryCount = territoryCount,
                    TerritoryShare = 0f,
                    Gold = stockpiles[f].Gold,
                    TradeRouteCount = tradeRouteCount,
                    TradeGoldPerDay = tradeGoldPerDay,
                    IsHumanPlayer = factionId.Equals(PlayerFactionId),
                };

                totalUnits += unitCount;
                totalTerritory += territoryCount;
            }

            // Compute territory shares and ranks.
            for (int f = 0; f < factionCount; f++)
            {
                var row = rows[f];
                row.TerritoryShare = totalTerritory > 0
                    ? (float)row.TerritoryCount / totalTerritory
                    : 0f;
                rows[f] = row;
            }

            SortByTerritoryShareDescending(rows, factionCount);

            for (int f = 0; f < factionCount; f++)
            {
                var row = rows[f];
                row.Rank = f + 1;
                rows[f] = row;
            }

            var buffer = em.GetBuffer<SkirmishStatusFactionRowHUDComponent>(singleton);
            buffer.Clear();
            for (int f = 0; f < factionCount && f < 8; f++)
            {
                buffer.Add(rows[f]);
            }

            rows.Dispose();

            header.InWorldDays = currentInWorldDays;
            header.ActiveFactionCount = factionCount;
            header.TotalUnitCount = totalUnits;
            header.TotalTerritoryCount = totalTerritory;
            header.LastRefreshInWorldDays = currentInWorldDays;
            em.SetComponentData(singleton, header);
        }

        private static Entity EnsureSingleton(EntityManager em)
        {
            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<SkirmishStatusHUDComponent>(),
                ComponentType.ReadOnly<SkirmishStatusFactionRowHUDComponent>());
            if (!query.IsEmptyIgnoreFilter)
            {
                return query.GetSingletonEntity();
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            Entity entity = ecb.CreateEntity();
            ecb.AddComponent(entity, new SkirmishStatusHUDComponent
            {
                LastRefreshInWorldDays = float.NaN,
            });
            ecb.AddBuffer<SkirmishStatusFactionRowHUDComponent>(entity);
            ecb.Playback(em);

            using var verifyQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<SkirmishStatusHUDComponent>(),
                ComponentType.ReadOnly<SkirmishStatusFactionRowHUDComponent>());
            return verifyQuery.GetSingletonEntity();
        }

        private static bool ShouldRefresh(
            EntityManager em,
            float lastRefreshInWorldDays,
            out float currentInWorldDays)
        {
            currentInWorldDays = 0f;
            using var clockQuery = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (clockQuery.IsEmptyIgnoreFilter)
            {
                return true;
            }

            currentInWorldDays = clockQuery.GetSingleton<DualClockComponent>().InWorldDays;
            return float.IsNaN(lastRefreshInWorldDays) ||
                   currentInWorldDays - lastRefreshInWorldDays >= RefreshCadenceInWorldDays;
        }

        private static void SortByTerritoryShareDescending(NativeArray<SkirmishStatusFactionRowHUDComponent> rows, int count)
        {
            for (int i = 1; i < count; i++)
            {
                var current = rows[i];
                int j = i - 1;
                while (j >= 0 && rows[j].TerritoryShare < current.TerritoryShare - 0.0001f)
                {
                    rows[j + 1] = rows[j];
                    j--;
                }

                rows[j + 1] = current;
            }
        }
    }
}
