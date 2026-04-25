using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the state of any active AI counter-intelligence operation for the given
        /// source faction. Useful for smoke validators and debug consoles.
        /// </summary>
        public bool TryDebugGetAICounterIntelligenceOperationState(
            string sourceFactionId,
            out bool hasActiveOperation,
            out float resolveAtInWorldDays,
            out float watchStrength,
            out float escrowGold,
            out float escrowInfluence)
        {
            hasActiveOperation   = false;
            resolveAtInWorldDays = 0f;
            watchStrength        = 0f;
            escrowGold           = 0f;
            escrowInfluence      = 0f;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyOperationComponent>(),
                ComponentType.ReadOnly<DynastyOperationCounterIntelligenceComponent>());

            if (q.IsEmpty) { q.Dispose(); return false; }

            var ops = q.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            var cis = q.ToComponentDataArray<DynastyOperationCounterIntelligenceComponent>(Allocator.Temp);
            q.Dispose();

            var sourceId = new FixedString32Bytes(sourceFactionId);
            bool found   = false;

            for (int i = 0; i < ops.Length; i++)
            {
                if (!ops[i].Active) continue;
                if (!ops[i].SourceFactionId.Equals(sourceId)) continue;
                if (ops[i].OperationKind != DynastyOperationKind.CounterIntelligence) continue;

                hasActiveOperation   = true;
                resolveAtInWorldDays = cis[i].ResolveAtInWorldDays;
                watchStrength        = cis[i].SuccessScore;
                escrowGold           = cis[i].EscrowGold;
                escrowInfluence      = cis[i].EscrowInfluence;
                found = true;
                break;
            }

            ops.Dispose();
            cis.Dispose();
            return found;
        }

        /// <summary>
        /// Returns the number of active counter-intelligence watch entries on the
        /// given faction entity. Useful for smoke validators.
        /// </summary>
        public int DebugGetCounterIntelligenceWatchCount(string factionId)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            var entities = q.ToEntityArray(Allocator.Temp);
            var factions = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            q.Dispose();

            var fid = new FixedString32Bytes(factionId);
            int count = 0;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(fid)) continue;
                if (em.HasBuffer<DynastyCounterIntelligenceWatchElement>(entities[i]))
                    count = em.GetBuffer<DynastyCounterIntelligenceWatchElement>(entities[i]).Length;
                break;
            }

            entities.Dispose();
            factions.Dispose();
            return count;
        }
    }
}
