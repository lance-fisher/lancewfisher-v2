using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Writes PlayerDivineRightActive into both AIStrategyComponent and
    /// AICovertOpsComponent when the player faction has an active
    /// DynastyOperationKind.DivineRight operation.
    ///
    /// AIStrategicPressureSystem uses AIStrategyComponent.PlayerDivineRightActive
    /// to apply attack, territory, and raid timer caps (ai.js lines 1156-1160).
    /// AICovertOpsComponent.PlayerDivineRightActive blocks the AI from launching
    /// its own divine right declaration while the player has one in flight.
    ///
    /// Browser equivalent: ai.js updateEnemyAi lines 1156-1160.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIHolyWarContextSystem))]
    public partial struct AIPlayerDivineRightContextSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIStrategyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            // Detect any active player DivineRight operation.
            bool hasPlayerDivineRight = false;
            var opQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>());
            using var ops = opQuery.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            opQuery.Dispose();

            var playerFactionId = new FixedString32Bytes("player");
            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (ops[i].OperationKind != DynastyOperationKind.DivineRight) continue;
                if (ops[i].SourceFactionId != playerFactionId) continue;
                hasPlayerDivineRight = true;
                break;
            }

            // Update each AI faction.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(AIStrategyComponent));
            using var entities   = factionQuery.ToEntityArray(Allocator.Temp);
            using var strategies = factionQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var s = strategies[i];
                s.PlayerDivineRightActive = hasPlayerDivineRight;
                em.SetComponentData(entities[i], s);

                if (em.HasComponent<AICovertOpsComponent>(entities[i]))
                {
                    var covertOps = em.GetComponentData<AICovertOpsComponent>(entities[i]);
                    covertOps.PlayerDivineRightActive = hasPlayerDivineRight;
                    em.SetComponentData(entities[i], covertOps);
                }
            }
        }
    }
}
