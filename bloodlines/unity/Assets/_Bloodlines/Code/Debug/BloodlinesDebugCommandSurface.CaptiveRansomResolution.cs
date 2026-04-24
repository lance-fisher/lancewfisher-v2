using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the state of any active captive ransom operation for the given
        /// source faction. Useful for smoke validators and debug consoles.
        /// </summary>
        public bool TryDebugGetCaptiveRansomOperationState(
            string sourceFactionId,
            out bool hasActiveOperation,
            out float resolveAtInWorldDays,
            out string captiveMemberId,
            out float escrowGold,
            out float escrowInfluence)
        {
            hasActiveOperation   = false;
            resolveAtInWorldDays = 0f;
            captiveMemberId      = string.Empty;
            escrowGold           = 0f;
            escrowInfluence      = 0f;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationCaptiveRansomComponent>());

            var entities = q.ToEntityArray(Allocator.Temp);
            var ops      = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var ransoms  = q.ToComponentDataArray<DynastyOperationCaptiveRansomComponent>(Allocator.Temp);
            q.Dispose();

            var srcId = new FixedString32Bytes(sourceFactionId);
            bool found = false;
            for (int i = 0; i < entities.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(srcId)) continue;
                hasActiveOperation   = true;
                resolveAtInWorldDays = ransoms[i].ResolveAtInWorldDays;
                captiveMemberId      = ransoms[i].CaptiveMemberId.ToString();
                escrowGold           = ransoms[i].EscrowCost.Gold;
                escrowInfluence      = ransoms[i].EscrowCost.Influence;
                found = true;
                break;
            }

            entities.Dispose();
            ops.Dispose();
            ransoms.Dispose();
            return found;
        }
    }
}
