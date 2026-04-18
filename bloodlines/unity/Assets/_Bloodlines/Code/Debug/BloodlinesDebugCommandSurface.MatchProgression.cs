using Bloodlines.GameTime;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the current DualClockComponent singleton snapshot.
        /// Returns false if the world or singleton is not present.
        /// </summary>
        public static bool TryDebugGetDualClock(out DualClockComponent clock)
        {
            clock = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            using var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) return false;
            clock = q.GetSingleton<DualClockComponent>();
            return true;
        }

        /// <summary>
        /// Returns the current MatchProgressionComponent singleton snapshot.
        /// Returns false if the world or singleton is not present.
        /// </summary>
        public static bool TryDebugGetMatchProgression(out MatchProgressionComponent progression)
        {
            progression = default;
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return false;
            var em = world.EntityManager;
            using var q = em.CreateEntityQuery(ComponentType.ReadOnly<MatchProgressionComponent>());
            if (q.IsEmpty) return false;
            progression = q.GetSingleton<MatchProgressionComponent>();
            return true;
        }
    }
}
