using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.AI
{
    /// <summary>
    /// Strategic AI decision layer for non-player Kingdom factions. Runs territory
    /// expansion, scout/harass dispatch, world-pressure posture response, and
    /// reinforcement routing on governed per-faction timers.
    ///
    /// Requires AIEconomyControllerComponent.Enabled == true to activate per faction.
    /// Reads AIStrategyComponent for per-faction state and writes back updated state.
    ///
    /// Browser equivalents:
    ///   pickTerritoryTarget        (ai.js ~747)
    ///   pickScoutHarassTarget      (ai.js ~412)
    ///   getWorldPressureRaidTarget (ai.js ~817)
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WorldPressureEscalationSystem))]
    public partial struct EnemyAIStrategySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AIStrategyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
                dt = 0.016f; // batch-mode fallback: 1 frame at 60 fps

            // Query all AI-enabled kingdom factions.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIEconomyControllerComponent>(),
                typeof(AIStrategyComponent));

            var entities    = factionQuery.ToEntityArray(Allocator.Temp);
            var factions    = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var economics   = factionQuery.ToComponentDataArray<AIEconomyControllerComponent>(Allocator.Temp);
            var strategies  = factionQuery.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            // Snapshot world pressure for all factions (read-only pass).
            var wpQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<WorldPressureComponent>());
            var wpFactions = wpQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var wpComps    = wpQuery.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
            wpQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!economics[i].Enabled)
                    continue;

                string factionId = factions[i].FactionId.ToString();
                var strategy = strategies[i];

                // Advance accumulators.
                strategy.ExpansionAccumulator             += dt;
                strategy.ScoutHarassAccumulator           += dt;
                strategy.WorldPressureResponseAccumulator += dt;
                strategy.ReinforcementAccumulator         += dt;

                float expansionInterval   = math.max(0.001f, strategy.ExpansionIntervalSeconds);
                float harassInterval      = math.max(0.001f, strategy.ScoutHarassIntervalSeconds);
                float pressureInterval    = math.max(0.001f, strategy.WorldPressureResponseIntervalSeconds);
                float reinforceInterval   = math.max(0.001f, strategy.ReinforcementIntervalSeconds);

                // World pressure posture (always first).
                if (strategy.WorldPressureResponseAccumulator >= pressureInterval)
                {
                    strategy.WorldPressureResponseAccumulator = 0f;
                    UpdatePosture(factions[i].FactionId, wpFactions, wpComps, ref strategy);
                }

                // Territory expansion: Expand posture only.
                if (strategy.CurrentPosture == AIStrategicPosture.Expand &&
                    strategy.ExpansionAccumulator >= expansionInterval)
                {
                    strategy.ExpansionAccumulator = 0f;
                    RunTerritoryExpansion(em, factionId, economics[i].MilitaryPostureMinimumMilitiaCount, ref strategy);
                }

                // Scout/harass: Expand or Consolidate.
                if (strategy.CurrentPosture != AIStrategicPosture.Defend &&
                    strategy.ScoutHarassAccumulator >= harassInterval)
                {
                    strategy.ScoutHarassAccumulator = 0f;
                    RunScoutHarass(em, factionId, ref strategy);
                }

                // Reinforcement: always active.
                if (strategy.ReinforcementAccumulator >= reinforceInterval)
                {
                    strategy.ReinforcementAccumulator = 0f;
                    RunReinforcement(em, factionId, ref strategy);
                }

                em.SetComponentData(entities[i], strategy);
            }

            entities.Dispose();
            factions.Dispose();
            economics.Dispose();
            strategies.Dispose();
            wpFactions.Dispose();
            wpComps.Dispose();
        }

        // ------------------------------------------------------------------ //
        //  Posture update from world pressure
        // ------------------------------------------------------------------ //

        private static void UpdatePosture(
            FixedString32Bytes factionId,
            NativeArray<FactionComponent> wpFactions,
            NativeArray<WorldPressureComponent> wpComps,
            ref AIStrategyComponent strategy)
        {
            int level    = 0;
            bool targeted = false;
            for (int i = 0; i < wpFactions.Length; i++)
            {
                if (wpFactions[i].FactionId.Equals(factionId))
                {
                    level    = wpComps[i].Level;
                    targeted = wpComps[i].Targeted;
                    break;
                }
            }
            strategy.WorldPressureLevelCached      = level;
            strategy.IsWorldPressureTargetedCached = targeted;
            strategy.CurrentPosture = level switch
            {
                >= 3 => AIStrategicPosture.Defend,
                >= 2 => AIStrategicPosture.Consolidate,
                _    => AIStrategicPosture.Expand,
            };
        }

        // ------------------------------------------------------------------ //
        //  Territory expansion
        // ------------------------------------------------------------------ //

        private static void RunTerritoryExpansion(
            EntityManager em,
            string factionId,
            int militiaThreshold,
            ref AIStrategyComponent strategy)
        {
            if (CountCombatUnits(em, factionId) < militiaThreshold)
                return;

            if (!TryPickExpansionTarget(em, factionId, out float3 targetPos, out var targetCpId))
                return;

            strategy.ExpansionTargetCpId = targetCpId;

            var factionKey = new FixedString32Bytes(factionId);
            var unitQuery  = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                typeof(MoveCommandComponent));

            var unitEntities  = unitQuery.ToEntityArray(Allocator.Temp);
            var unitFactions  = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var unitTypes     = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            var unitHealth    = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            var moveComps     = unitQuery.ToComponentDataArray<MoveCommandComponent>(Allocator.Temp);
            unitQuery.Dispose();

            const float captureRadius = 1.5f;
            int ordered = 0;
            for (int i = 0; i < unitEntities.Length; i++)
            {
                if (!unitFactions[i].FactionId.Equals(factionKey) || unitHealth[i].Current <= 0f)
                    continue;
                var role = unitTypes[i].Role;
                if (role == UnitRole.Worker || role == UnitRole.SiegeEngine ||
                    role == UnitRole.SiegeSupport || role == UnitRole.Support)
                    continue;

                if (moveComps[i].IsActive &&
                    math.distancesq(moveComps[i].Destination, targetPos) <= captureRadius * captureRadius)
                    continue;

                if (math.distancesq(unitPositions[i].Value, targetPos) <= captureRadius * captureRadius)
                    continue;

                em.SetComponentData(unitEntities[i], new MoveCommandComponent
                {
                    Destination     = targetPos,
                    StoppingDistance = captureRadius,
                    IsActive        = true,
                });
                ordered++;
            }

            unitEntities.Dispose();
            unitFactions.Dispose();
            unitTypes.Dispose();
            unitPositions.Dispose();
            unitHealth.Dispose();
            moveComps.Dispose();

            strategy.ExpansionOrdersIssued   += ordered;
            strategy.TerritoryCommandsIssued += ordered;
        }

        private static bool TryPickExpansionTarget(
            EntityManager em,
            string factionId,
            out float3 position,
            out FixedString32Bytes cpId)
        {
            position = default;
            cpId     = default;

            if (!TryGetCommandHallPosition(em, factionId, out float3 basePos))
                return false;

            var factionKey = new FixedString32Bytes(factionId);
            var cpQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            var cpPos  = cpQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            cpQuery.Dispose();

            float bestScore = float.MinValue;
            bool  found     = false;

            for (int i = 0; i < cpData.Length; i++)
            {
                if (cpData[i].OwnerFactionId.Equals(factionKey))
                    continue;

                float dist  = math.distance(basePos, cpPos[i].Value);
                float score = -dist * 0.5f - cpData[i].Loyalty * 0.1f;
                if (score > bestScore)
                {
                    bestScore = score;
                    position  = cpPos[i].Value;
                    cpId      = cpData[i].ControlPointId;
                    found     = true;
                }
            }

            cpData.Dispose();
            cpPos.Dispose();
            return found;
        }

        // ------------------------------------------------------------------ //
        //  Scout / harass dispatch
        // ------------------------------------------------------------------ //

        private static void RunScoutHarass(
            EntityManager em,
            string factionId,
            ref AIStrategyComponent strategy)
        {
            if (!TryPickHarassTarget(em, factionId, out float3 harassPos, out var harassCpId))
                return;

            strategy.HarassTargetCpId = harassCpId;

            var factionKey = new FixedString32Bytes(factionId);
            var unitQuery  = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                typeof(MoveCommandComponent));

            var unitEntities  = unitQuery.ToEntityArray(Allocator.Temp);
            var unitFactions  = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var unitTypes     = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            var unitHealth    = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            unitQuery.Dispose();

            const float harassRadius = 1.2f;
            int dispatched = 0;
            for (int i = 0; i < unitEntities.Length; i++)
            {
                if (!unitFactions[i].FactionId.Equals(factionKey) || unitHealth[i].Current <= 0f)
                    continue;
                var role = unitTypes[i].Role;
                if (role != UnitRole.LightCavalry && role != UnitRole.MeleeRecon)
                    continue;

                if (math.distancesq(unitPositions[i].Value, harassPos) <= harassRadius * harassRadius)
                    continue;

                em.SetComponentData(unitEntities[i], new MoveCommandComponent
                {
                    Destination     = harassPos,
                    StoppingDistance = harassRadius,
                    IsActive        = true,
                });
                dispatched++;
            }

            unitEntities.Dispose();
            unitFactions.Dispose();
            unitTypes.Dispose();
            unitPositions.Dispose();
            unitHealth.Dispose();

            strategy.ScoutHarassOrdersIssued += dispatched;
        }

        private static bool TryPickHarassTarget(
            EntityManager em,
            string factionId,
            out float3 position,
            out FixedString32Bytes cpId)
        {
            position = default;
            cpId     = default;

            if (!TryGetCommandHallPosition(em, factionId, out float3 basePos))
                return false;

            var factionKey = new FixedString32Bytes(factionId);
            var cpQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            var cpPos  = cpQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            cpQuery.Dispose();

            float bestScore = float.MinValue;
            bool  found     = false;

            for (int i = 0; i < cpData.Length; i++)
            {
                if (cpData[i].OwnerFactionId.Length == 0 || cpData[i].OwnerFactionId.Equals(factionKey))
                    continue;

                float dist  = math.distance(basePos, cpPos[i].Value);
                float score = -(cpData[i].Loyalty * 0.8f) - dist * 0.3f;
                if (score > bestScore)
                {
                    bestScore = score;
                    position  = cpPos[i].Value;
                    cpId      = cpData[i].ControlPointId;
                    found     = true;
                }
            }

            cpData.Dispose();
            cpPos.Dispose();
            return found;
        }

        // ------------------------------------------------------------------ //
        //  Reinforcement routing
        // ------------------------------------------------------------------ //

        private static void RunReinforcement(
            EntityManager em,
            string factionId,
            ref AIStrategyComponent strategy)
        {
            var factionKey = new FixedString32Bytes(factionId);
            var cpQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            var cpPos  = cpQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            cpQuery.Dispose();

            float3 reinforcePos   = default;
            bool   found          = false;
            float  lowestLoyalty  = float.MaxValue;

            for (int i = 0; i < cpData.Length; i++)
            {
                if (!cpData[i].OwnerFactionId.Equals(factionKey))
                    continue;
                if (cpData[i].Loyalty < lowestLoyalty)
                {
                    lowestLoyalty = cpData[i].Loyalty;
                    reinforcePos  = cpPos[i].Value;
                    found         = true;
                }
            }

            cpData.Dispose();
            cpPos.Dispose();

            if (!found || lowestLoyalty > 60f)
                return;

            var unitQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                typeof(MoveCommandComponent));

            var unitEntities  = unitQuery.ToEntityArray(Allocator.Temp);
            var unitFactions  = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var unitTypes     = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            var unitHealth    = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            var moveComps     = unitQuery.ToComponentDataArray<MoveCommandComponent>(Allocator.Temp);
            unitQuery.Dispose();

            const float reinforceRadius = 1.5f;
            int ordered = 0;
            for (int i = 0; i < unitEntities.Length; i++)
            {
                if (!unitFactions[i].FactionId.Equals(factionKey) || unitHealth[i].Current <= 0f)
                    continue;
                var role = unitTypes[i].Role;
                if (role == UnitRole.Worker || role == UnitRole.SiegeEngine ||
                    role == UnitRole.SiegeSupport || role == UnitRole.Support)
                    continue;
                if (moveComps[i].IsActive)
                    continue;
                if (math.distancesq(unitPositions[i].Value, reinforcePos) <= reinforceRadius * reinforceRadius)
                    continue;

                em.SetComponentData(unitEntities[i], new MoveCommandComponent
                {
                    Destination     = reinforcePos,
                    StoppingDistance = reinforceRadius,
                    IsActive        = true,
                });
                ordered++;
            }

            unitEntities.Dispose();
            unitFactions.Dispose();
            unitTypes.Dispose();
            unitPositions.Dispose();
            unitHealth.Dispose();
            moveComps.Dispose();

            strategy.ReinforcementOrdersIssued += ordered;
        }

        // ------------------------------------------------------------------ //
        //  Shared query helpers
        // ------------------------------------------------------------------ //

        private static bool TryGetCommandHallPosition(
            EntityManager em,
            string factionId,
            out float3 position)
        {
            position = default;
            var factionKey  = new FixedString32Bytes(factionId);
            var hallTypeKey = new FixedString64Bytes("command_hall");

            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            var qFactions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var qTypes    = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            var qPos      = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            var qHealth   = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < qFactions.Length; i++)
            {
                if (qFactions[i].FactionId.Equals(factionKey) &&
                    qTypes[i].TypeId.Equals(hallTypeKey) &&
                    qHealth[i].Current > 0f)
                {
                    position = qPos[i].Value;
                    qFactions.Dispose(); qTypes.Dispose(); qPos.Dispose(); qHealth.Dispose();
                    return true;
                }
            }

            qFactions.Dispose(); qTypes.Dispose(); qPos.Dispose(); qHealth.Dispose();
            return false;
        }

        private static int CountCombatUnits(EntityManager em, string factionId)
        {
            var factionKey = new FixedString32Bytes(factionId);
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            var qFactions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var qTypes    = query.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            var qHealth   = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            query.Dispose();

            int count = 0;
            for (int i = 0; i < qFactions.Length; i++)
            {
                if (!qFactions[i].FactionId.Equals(factionKey) || qHealth[i].Current <= 0f)
                    continue;
                var role = qTypes[i].Role;
                if (role != UnitRole.Worker && role != UnitRole.SiegeEngine &&
                    role != UnitRole.SiegeSupport && role != UnitRole.Support && role != UnitRole.Unknown)
                    count++;
            }

            qFactions.Dispose(); qTypes.Dispose(); qHealth.Dispose();
            return count;
        }
    }
}
