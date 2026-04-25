using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Writes AIStrategyComponent.HolyWarActive for each AI faction based on
    /// whether any active DynastyOperationKind.HolyWar operation involves that
    /// faction (as source or target).
    ///
    /// AIStrategicPressureSystem reads HolyWarActive to apply attack and territory
    /// timer caps (ai.js updateEnemyAi lines 1129-1132). Any holy war in either
    /// direction triggers these clamps: the AI pulls attack pressure forward when
    /// a holy war is active.
    ///
    /// Browser equivalent: ai.js updateEnemyAi lines 1129-1132.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AISuccessionCrisisContextSystem))]
    public partial struct AIHolyWarContextSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIStrategyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            // Collect faction IDs involved in any active holy war operation.
            var opQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>());
            using var ops = opQuery.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            opQuery.Dispose();

            var involvedFactions = new NativeHashSet<FixedString32Bytes>(8, Allocator.Temp);
            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (ops[i].OperationKind != DynastyOperationKind.HolyWar) continue;
                involvedFactions.Add(ops[i].SourceFactionId);
                if (ops[i].TargetFactionId.Length > 0)
                    involvedFactions.Add(ops[i].TargetFactionId);
            }

            // Update each AI faction.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(AIStrategyComponent));
            using var entities   = factionQuery.ToEntityArray(Allocator.Temp);
            using var factions   = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var strategies = factionQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var s = strategies[i];
                s.HolyWarActive = involvedFactions.Contains(factions[i].FactionId);
                em.SetComponentData(entities[i], s);
            }

            involvedFactions.Dispose();
        }
    }
}
