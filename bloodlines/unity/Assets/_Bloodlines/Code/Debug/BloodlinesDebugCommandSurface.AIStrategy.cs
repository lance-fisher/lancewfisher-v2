using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Public debug API for the AI strategic layer. Strategy logic lives in
    /// EnemyAIStrategySystem (ISystem). This partial only exposes the inspector
    /// read path used by tests and external callers.
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetAIStrategy(
            string factionId,
            out AIStrategicPosture posture,
            out int expansionOrders,
            out int harassOrders,
            out int reinforcementOrders,
            out int worldPressureLevel,
            out string expansionTargetCpId,
            out string harassTargetCpId)
        {
            posture             = AIStrategicPosture.Expand;
            expansionOrders     = 0;
            harassOrders        = 0;
            reinforcementOrders = 0;
            worldPressureLevel  = 0;
            expansionTargetCpId = string.Empty;
            harassTargetCpId    = string.Empty;

            if (!TryGetEntityManager(out var entityManager))
                return false;

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AIStrategyComponent>());

            using var factions   = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var strategies = query.ToComponentDataArray<AIStrategyComponent>(Allocator.Temp);

            var key = new FixedString32Bytes(factionId ?? string.Empty);
            for (int i = 0; i < factions.Length; i++)
            {
                if (!factions[i].FactionId.Equals(key))
                    continue;

                var s = strategies[i];
                posture             = s.CurrentPosture;
                expansionOrders     = s.ExpansionOrdersIssued;
                harassOrders        = s.ScoutHarassOrdersIssued;
                reinforcementOrders = s.ReinforcementOrdersIssued;
                worldPressureLevel  = s.WorldPressureLevelCached;
                expansionTargetCpId = s.ExpansionTargetCpId.ToString();
                harassTargetCpId    = s.HarassTargetCpId.ToString();
                return true;
            }

            return false;
        }
    }
}
