using Bloodlines.Components;
using Unity.Entities;

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
    }
}
