using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetPrestigeDecayRate(
            string factionId,
            out float decayRate,
            out bool interregnumActive,
            out bool crisisActive,
            out string convictionBand)
        {
            decayRate = 0f;
            interregnumActive = false;
            crisisActive = false;
            convictionBand = string.Empty;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyRenownComponent>());
            if (query.IsEmptyIgnoreFilter) return false;

            using var entities   = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var factions   = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var renownData = query.ToComponentDataArray<DynastyRenownComponent>(Unity.Collections.Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(new Unity.Collections.FixedString32Bytes(factionId)))
                    continue;

                decayRate = renownData[i].RenownDecayRate;

                Entity e = entities[i];
                if (em.HasComponent<DynastyStateComponent>(e))
                    interregnumActive = em.GetComponentData<DynastyStateComponent>(e).Interregnum;

                if (em.HasComponent<SuccessionCrisisComponent>(e))
                {
                    var crisis = em.GetComponentData<SuccessionCrisisComponent>(e);
                    crisisActive = crisis.RecoveryProgressPct < 1f;
                }

                if (em.HasComponent<ConvictionComponent>(e))
                    convictionBand = em.GetComponentData<ConvictionComponent>(e).Band.ToString();

                return true;
            }

            return false;
        }
    }
}
