using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns whether the given faction has an active divine right operation
        /// and its current resolution state. Browser equivalent: faction.faith.divineRightDeclaration.
        /// </summary>
        public bool TryDebugGetDivineRightOperationState(
            string      factionId,
            out bool    hasActiveOperation,
            out float   resolveAtInWorldDays,
            out string  sourceFaithId,
            out bool    darkDoctrine)
        {
            hasActiveOperation  = false;
            resolveAtInWorldDays = 0f;
            sourceFaithId       = string.Empty;
            darkDoctrine        = false;

            var em  = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            var fid = new FixedString32Bytes(factionId);

            using var q = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationDivineRightComponent>());
            if (q.IsEmptyIgnoreFilter) return false;

            using var entities = q.ToEntityArray(Allocator.Temp);
            using var ops      = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            using var drs      = q.ToComponentDataArray<DynastyOperationDivineRightComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].SourceFactionId.Equals(fid)) continue;
                if (!ops[i].Active) continue;

                hasActiveOperation   = true;
                resolveAtInWorldDays = drs[i].ResolveAtInWorldDays;
                sourceFaithId        = drs[i].SourceFaithId.ToString();
                darkDoctrine         = drs[i].DoctrinePath == DoctrinePath.Dark;
                return true;
            }

            return true;
        }
    }
}
