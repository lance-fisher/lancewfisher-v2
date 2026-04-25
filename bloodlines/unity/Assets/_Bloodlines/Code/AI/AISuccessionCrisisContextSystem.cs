using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.AI
{
    /// <summary>
    /// Writes succession-crisis context flags into AIStrategyComponent each frame
    /// so AIStrategicPressureSystem can apply the correct timer clamps.
    ///
    /// Flags written:
    ///   PlayerSuccessionCrisisActive -- player faction has an active crisis (severity > 0)
    ///   PlayerSuccessionCrisisHigh   -- player crisis severity >= 3 (Major or Catastrophic)
    ///   EnemySuccessionCrisisActive  -- this AI faction has an active crisis
    ///   EnemySuccessionCrisisSevere  -- this AI faction's severity >= 3
    ///
    /// Browser equivalent: ai.js updateEnemyAi lines 1161-1185. The browser reads
    /// playerSuccessionCrisis and enemySuccessionCrisis from the game snapshot each
    /// frame; this system does the same scan in the ECS world.
    ///
    /// AIStrategicPressureSystem reads these flags to apply attack/territory timer
    /// clamps and the enemy-crisis consolidation accumulator.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AIFaithCommitmentSystem))]
    public partial struct AISuccessionCrisisContextSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIStrategyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            // Step 1: Read player succession crisis state.
            bool playerCrisisActive = false;
            bool playerCrisisHigh   = false;

            var allFactionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>());
            using var allEntities  = allFactionQuery.ToEntityArray(Allocator.Temp);
            using var allFactions  = allFactionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            allFactionQuery.Dispose();

            for (int i = 0; i < allEntities.Length; i++)
            {
                if (allFactions[i].FactionId != (FixedString32Bytes)"player") continue;
                if (!em.HasComponent<SuccessionCrisisComponent>(allEntities[i])) break;
                var crisis = em.GetComponentData<SuccessionCrisisComponent>(allEntities[i]);
                playerCrisisActive = crisis.CrisisSeverity > 0;
                playerCrisisHigh   = crisis.CrisisSeverity >= 3;
                break;
            }

            // Step 2: Update each AI faction's flags.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(AIStrategyComponent));
            using var entities   = factionQuery.ToEntityArray(Allocator.Temp);
            using var strategies = factionQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                bool enemyCrisisActive = false;
                bool enemyCrisisSevere = false;

                if (em.HasComponent<SuccessionCrisisComponent>(entities[i]))
                {
                    var crisis = em.GetComponentData<SuccessionCrisisComponent>(entities[i]);
                    enemyCrisisActive = crisis.CrisisSeverity > 0;
                    enemyCrisisSevere = crisis.CrisisSeverity >= 3;
                }

                var s = strategies[i];
                s.PlayerSuccessionCrisisActive = playerCrisisActive;
                s.PlayerSuccessionCrisisHigh   = playerCrisisHigh;
                s.EnemySuccessionCrisisActive  = enemyCrisisActive;
                s.EnemySuccessionCrisisSevere  = enemyCrisisSevere;
                em.SetComponentData(entities[i], s);
            }
        }
    }
}
