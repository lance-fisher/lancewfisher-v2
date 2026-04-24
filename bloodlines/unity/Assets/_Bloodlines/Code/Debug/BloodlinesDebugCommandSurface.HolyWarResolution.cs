using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the count and summary of active holy wars declared by
        /// the given faction. Reads the ActiveHolyWarElement buffer on
        /// the faction entity. Browser equivalent: faction.faith.activeHolyWars.
        /// </summary>
        public bool TryDebugGetActiveHolyWars(
            string   factionId,
            out int  count,
            out string[] targetFactionIds,
            out float[]  expiresAtInWorldDays)
        {
            count                = 0;
            targetFactionIds     = System.Array.Empty<string>();
            expiresAtInWorldDays = System.Array.Empty<float>();

            var em  = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            var fid = new FixedString32Bytes(factionId);

            using var q = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            if (q.IsEmptyIgnoreFilter) return false;

            using var entities = q.ToEntityArray(Allocator.Temp);
            using var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(fid)) continue;

                if (!em.HasBuffer<ActiveHolyWarElement>(entities[i])) return true;

                var buffer = em.GetBuffer<ActiveHolyWarElement>(entities[i], true);
                count = buffer.Length;
                targetFactionIds     = new string[count];
                expiresAtInWorldDays = new float[count];
                for (int j = 0; j < count; j++)
                {
                    targetFactionIds[j]     = buffer[j].TargetFactionId.ToString();
                    expiresAtInWorldDays[j] = buffer[j].ExpiresAtInWorldDays;
                }
                return true;
            }
            return false;
        }
    }
}
