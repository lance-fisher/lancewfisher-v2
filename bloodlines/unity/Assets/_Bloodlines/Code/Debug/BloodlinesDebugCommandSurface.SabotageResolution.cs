using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the state of any active AI sabotage operation for the given
        /// source faction. Useful for smoke validators and debug consoles.
        /// </summary>
        public bool TryDebugGetAISabotageOperationState(
            string sourceFactionId,
            out bool hasActiveOperation,
            out float resolveAtInWorldDays,
            out string subtype,
            out int targetBuildingEntityIndex,
            out float successScore,
            out float escrowInfluence)
        {
            hasActiveOperation      = false;
            resolveAtInWorldDays    = 0f;
            subtype                 = string.Empty;
            targetBuildingEntityIndex = -1;
            successScore            = 0f;
            escrowInfluence         = 0f;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationSabotageComponent>());

            if (q.IsEmpty) { q.Dispose(); return false; }

            var ops       = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var sabotages = q.ToComponentDataArray<DynastyOperationSabotageComponent>(Allocator.Temp);
            q.Dispose();

            var sourceId = new FixedString32Bytes(sourceFactionId);
            bool found   = false;

            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(sourceId)) continue;
                if (ops[i].OperationKind != DynastyOperationKind.Sabotage) continue;

                hasActiveOperation        = true;
                resolveAtInWorldDays      = sabotages[i].ResolveAtInWorldDays;
                subtype                   = sabotages[i].Subtype.ToString();
                targetBuildingEntityIndex = sabotages[i].TargetBuildingEntityIndex;
                successScore              = sabotages[i].SuccessScore;
                escrowInfluence           = sabotages[i].EscrowInfluence;
                found = true;
                break;
            }

            ops.Dispose();
            sabotages.Dispose();
            return found;
        }
    }
}
