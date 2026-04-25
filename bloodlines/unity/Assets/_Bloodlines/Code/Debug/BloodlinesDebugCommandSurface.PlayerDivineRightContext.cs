using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns PlayerDivineRightActive from both AIStrategyComponent and
        /// AICovertOpsComponent for the named AI faction.
        /// </summary>
        public bool TryDebugGetPlayerDivineRightFlags(
            string aiFactionId,
            out bool strategyFlag,
            out bool covertOpsFlag)
        {
            strategyFlag  = false;
            covertOpsFlag = false;

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
                strategyFlag = strategies[i].PlayerDivineRightActive;
                if (em.HasComponent<AICovertOpsComponent>(entities[i]))
                    covertOpsFlag = em.GetComponentData<AICovertOpsComponent>(entities[i]).PlayerDivineRightActive;
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
