using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    public partial class BloodlinesDebugCommandSurface
    {
        public static bool TryDebugGetWorldPressure(string factionId, out WorldPressureComponent wp)
        {
            wp = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<WorldPressureComponent>());
            using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var wpData = query.ToComponentDataArray<WorldPressureComponent>(Unity.Collections.Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId == new Unity.Collections.FixedString32Bytes(factionId))
                {
                    wp = wpData[i];
                    return true;
                }
            }
            return false;
        }

        public static bool TryDebugGetWorldPressureCycleTracker(out WorldPressureCycleTrackerComponent tracker)
        {
            tracker = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<WorldPressureCycleTrackerComponent>());
            if (query.IsEmpty) { query.Dispose(); return false; }
            tracker = query.GetSingleton<WorldPressureCycleTrackerComponent>();
            query.Dispose();
            return true;
        }

        public static bool TryDebugGetGovernanceCoalitionState(out Unity.Collections.FixedString512Bytes readout)
        {
            readout = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<TerritorialGovernanceRecognitionComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var recognitions = query.ToComponentDataArray<TerritorialGovernanceRecognitionComponent>(Unity.Collections.Allocator.Temp);
            query.Dispose();

            int bestIndex = -1;
            float bestAcceptance = float.MinValue;
            for (int i = 0; i < factions.Length; i++)
            {
                var recognition = recognitions[i];
                if (!recognition.Active && !recognition.Completed && recognition.WorldPressureContribution <= 0)
                {
                    continue;
                }

                if (recognition.PopulationAcceptancePct >= bestAcceptance)
                {
                    bestAcceptance = recognition.PopulationAcceptancePct;
                    bestIndex = i;
                }
            }

            if (bestIndex < 0)
            {
                return false;
            }

            var faction = factions[bestIndex];
            var selected = recognitions[bestIndex];
            readout = new Unity.Collections.FixedString512Bytes(
                $"Faction={faction.FactionId}" +
                $"|Active={(selected.Active ? "true" : "false")}" +
                $"|Recognized={(selected.RecognitionEstablished ? "true" : "false")}" +
                $"|Completed={(selected.Completed ? "true" : "false")}" +
                $"|Acceptance={(int)math.round(selected.PopulationAcceptancePct)}" +
                $"|Target={(int)math.round(selected.PopulationAcceptanceTargetPct)}" +
                $"|AllianceReady={(selected.AllianceThresholdReady ? "true" : "false")}" +
                $"|AlliancePressure={(selected.AlliancePressureActive ? "true" : "false")}" +
                $"|Hostiles={selected.AlliancePressureHostileCount}" +
                $"|Cycles={selected.AlliancePressureCycles}" +
                $"|Weakest={selected.WeakestControlPointId}" +
                $"|WeakestLoyalty={(int)math.round(selected.WeakestControlPointLoyalty)}" +
                $"|WorldPressure={selected.WorldPressureContribution}");
            return true;
        }
    }
}
