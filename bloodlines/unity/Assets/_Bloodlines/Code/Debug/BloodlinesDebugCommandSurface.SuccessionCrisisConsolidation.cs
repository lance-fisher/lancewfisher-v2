using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        /// <summary>
        /// Returns the AI succession-crisis consolidation timer for the given
        /// faction. Useful for smoke validators and debug consoles.
        /// </summary>
        public bool TryDebugGetAISuccessionCrisisConsolidationState(
            string factionId,
            out float timer,
            out bool hasCrisis,
            out byte crisisSeverity)
        {
            timer         = 0f;
            hasCrisis     = false;
            crisisSeverity = 0;

            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var q  = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<AISuccessionCrisisConsolidationComponent>());

            if (q.IsEmpty) { q.Dispose(); return false; }

            var entities   = q.ToEntityArray(Allocator.Temp);
            var factions   = q.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var timers     = q.ToComponentDataArray<AISuccessionCrisisConsolidationComponent>(Allocator.Temp);
            q.Dispose();

            var targetId = new FixedString32Bytes(factionId);
            bool found   = false;

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(targetId)) continue;
                timer = timers[i].SuccessionCrisisTimer;
                if (em.HasComponent<SuccessionCrisisComponent>(entities[i]))
                {
                    var crisis = em.GetComponentData<SuccessionCrisisComponent>(entities[i]);
                    hasCrisis      = crisis.CrisisSeverity != 0;
                    crisisSeverity = crisis.CrisisSeverity;
                }
                found = true;
                break;
            }

            entities.Dispose();
            factions.Dispose();
            timers.Dispose();
            return found;
        }

        /// <summary>Batch entry point used by BloodlinesSuccessionCrisisConsolidationSmokeValidation.</summary>
        public static void RunBatchSuccessionCrisisConsolidationSmokeValidation()
        {
            var surface = new BloodlinesDebugCommandSurface();
            surface.TryDebugGetAISuccessionCrisisConsolidationState("enemy", out _, out _, out _);
        }
    }
}
