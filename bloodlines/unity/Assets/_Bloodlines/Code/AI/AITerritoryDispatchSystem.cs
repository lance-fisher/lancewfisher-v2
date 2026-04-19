using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Converts AI territorial target selection into concrete move commands.
    ///
    /// Browser reference: ai.js territory dispatch block (~1711-1821) when
    /// territoryTimer <= 0 and rivalry is unlocked.
    ///
    /// EnemyAIStrategySystem chooses ExpansionTargetCpId. This bridge reads that
    /// target plus the TerritoryTimer gate, issues march orders to faction combat
    /// units, and resets TerritoryTimer to the Unity lane's canonical default.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AICovertOpsSystem))]
    public partial struct AITerritoryDispatchSystem : ISystem
    {
        private const float TerritoryTimerResetSeconds = 12f;
        private const float CaptureStoppingDistance = 1.5f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIStrategyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;

            var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var controlPointPositions = controlPointQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            controlPointQuery.Dispose();

            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(AIStrategyComponent));
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionComponents = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var economyComponents = factionQuery.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);
            using var strategyComponents = factionQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<CombatStatsComponent>());
            using var unitEntities = unitQuery.ToEntityArray(Allocator.Temp);
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var unitHealth = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            unitQuery.Dispose();

            for (int factionIndex = 0; factionIndex < factionEntities.Length; factionIndex++)
            {
                if (!economyComponents[factionIndex].Enabled)
                {
                    continue;
                }

                var strategy = strategyComponents[factionIndex];
                if (strategy.TerritoryTimer > 0f ||
                    !strategy.RivalryUnlocked ||
                    strategy.ExpansionTargetCpId.Length == 0)
                {
                    continue;
                }

                if (!TryResolveControlPointPosition(
                        strategy.ExpansionTargetCpId,
                        controlPoints,
                        controlPointPositions,
                        out float3 destination))
                {
                    continue;
                }

                int dispatched = 0;
                FixedString32Bytes factionId = factionComponents[factionIndex].FactionId;
                for (int unitIndex = 0; unitIndex < unitEntities.Length; unitIndex++)
                {
                    if (!unitFactions[unitIndex].FactionId.Equals(factionId) ||
                        unitHealth[unitIndex].Current <= 0f)
                    {
                        continue;
                    }

                    var role = unitTypes[unitIndex].Role;
                    if (role == UnitRole.Worker ||
                        role == UnitRole.SiegeEngine ||
                        role == UnitRole.SiegeSupport)
                    {
                        continue;
                    }

                    var moveCommand = new MoveCommandComponent
                    {
                        Destination = destination,
                        StoppingDistance = CaptureStoppingDistance,
                        IsActive = true,
                    };

                    if (entityManager.HasComponent<MoveCommandComponent>(unitEntities[unitIndex]))
                    {
                        entityManager.SetComponentData(unitEntities[unitIndex], moveCommand);
                    }
                    else
                    {
                        entityManager.AddComponentData(unitEntities[unitIndex], moveCommand);
                    }

                    dispatched++;
                }

                strategy.ExpansionOrdersIssued += dispatched;
                strategy.TerritoryCommandsIssued += dispatched;
                strategy.TerritoryTimer = TerritoryTimerResetSeconds;
                entityManager.SetComponentData(factionEntities[factionIndex], strategy);
            }
        }

        private static bool TryResolveControlPointPosition(
            FixedString32Bytes controlPointId,
            NativeArray<ControlPointComponent> controlPoints,
            NativeArray<PositionComponent> positions,
            out float3 position)
        {
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].ControlPointId.Equals(controlPointId))
                {
                    position = positions[i].Value;
                    return true;
                }
            }

            position = default;
            return false;
        }
    }
}
