using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns AIStrategyComponent.HolyWarActive for the named AI faction.
        /// </summary>
        public bool TryDebugGetHolyWarContextFlag(string aiFactionId, out bool holyWarActive)
        {
            holyWarActive = false;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                ComponentType.ReadOnly<AIStrategyComponent>());

            if (q.IsEmpty) { q.Dispose(); return false; }

            var entities   = q.ToEntityArray(Allocator.Temp);
            var factions   = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var strategies = q.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            q.Dispose();

            var targetId = new FixedString32Bytes(aiFactionId);
            bool found   = false;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(targetId)) continue;
                holyWarActive = strategies[i].HolyWarActive;
                found = true;
                break;
            }

            entities.Dispose();
            factions.Dispose();
            strategies.Dispose();
            return found;
        }
    }
}
