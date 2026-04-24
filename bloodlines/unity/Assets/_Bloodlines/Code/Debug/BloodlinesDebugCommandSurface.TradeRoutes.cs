using Bloodlines.Components;
using Bloodlines.Economy;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetTradeRoutes(string factionId, out int activeRouteCount, out float goldPerDay)
        {
            activeRouteCount = 0;
            goldPerDay = 0f;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<TradeRouteComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var routes = query.ToComponentDataArray<TradeRouteComponent>(Unity.Collections.Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.ToString() == factionId)
                {
                    activeRouteCount = routes[i].ActiveRouteCount;
                    goldPerDay = routes[i].TotalGoldPerTickFromTrades;
                    return true;
                }
            }

            return false;
        }
    }
}
