using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the state of any active captive rescue operation for the given
        /// source faction. Useful for smoke validators and debug consoles.
        /// </summary>
        public bool TryDebugGetCaptiveRescueOperationState(
            string sourceFactionId,
            out bool hasActiveOperation,
            out float resolveAtInWorldDays,
            out string captiveMemberId,
            out float successScore)
        {
            hasActiveOperation  = false;
            resolveAtInWorldDays = 0f;
            captiveMemberId     = string.Empty;
            successScore        = 0f;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationCaptiveRescueComponent>());

            var entities = q.ToEntityArray(Allocator.Temp);
            var ops      = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var rescues  = q.ToComponentDataArray<DynastyOperationCaptiveRescueComponent>(Allocator.Temp);
            q.Dispose();

            var srcId = new FixedString32Bytes(sourceFactionId);
            bool found = false;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(srcId)) continue;
                hasActiveOperation   = true;
                resolveAtInWorldDays = rescues[i].ResolveAtInWorldDays;
                captiveMemberId      = rescues[i].CaptiveMemberId.ToString();
                successScore         = rescues[i].SuccessScore;
                found = true;
                break;
            }

            entities.Dispose();
            ops.Dispose();
            rescues.Dispose();
            return found;
        }
    }
}
