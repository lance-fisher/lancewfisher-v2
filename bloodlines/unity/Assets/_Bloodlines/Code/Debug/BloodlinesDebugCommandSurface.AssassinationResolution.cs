using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the state of any active assassination operation for the given
        /// source faction. Useful for smoke validators and debug consoles.
        /// </summary>
        public bool TryDebugGetAssassinationOperationState(
            string sourceFactionId,
            out bool hasActiveOperation,
            out float resolveAtInWorldDays,
            out string targetMemberId,
            out float successScore,
            out float escrowInfluence)
        {
            hasActiveOperation   = false;
            resolveAtInWorldDays = 0f;
            targetMemberId       = string.Empty;
            successScore         = 0f;
            escrowInfluence      = 0f;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationAssassinationComponent>());

            if (q.IsEmpty) { q.Dispose(); return false; }

            var ops     = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var assasns = q.ToComponentDataArray<DynastyOperationAssassinationComponent>(Allocator.Temp);
            q.Dispose();

            var sourceId = new FixedString32Bytes(sourceFactionId);
            bool found   = false;

            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(sourceId)) continue;
                if (ops[i].OperationKind != DynastyOperationKind.Assassination) continue;

                hasActiveOperation   = true;
                resolveAtInWorldDays = assasns[i].ResolveAtInWorldDays;
                targetMemberId       = assasns[i].TargetMemberId.ToString();
                successScore         = assasns[i].SuccessScore;
                escrowInfluence      = assasns[i].EscrowInfluence;
                found = true;
                break;
            }

            ops.Dispose();
            assasns.Dispose();
            return found;
        }
    }
}
