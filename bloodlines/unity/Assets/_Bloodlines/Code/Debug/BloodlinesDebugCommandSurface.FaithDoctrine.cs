using Bloodlines.Components;
using Bloodlines.Faith;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetFaithDoctrineEffects(
            string factionId,
            out float auraAttackMultiplier,
            out float auraRadiusBonus,
            out float auraSightBonus,
            out float stabilizationMultiplier,
            out float captureMultiplier,
            out float populationGrowthMultiplier)
        {
            auraAttackMultiplier = 1f;
            auraRadiusBonus = 0f;
            auraSightBonus = 0f;
            stabilizationMultiplier = 1f;
            captureMultiplier = 1f;
            populationGrowthMultiplier = 1f;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FaithDoctrineEffectsComponent>());
            if (query.IsEmptyIgnoreFilter)
                return false;

            using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var effects = query.ToComponentDataArray<FaithDoctrineEffectsComponent>(Unity.Collections.Allocator.Temp);

            var targetId = new Unity.Collections.FixedString32Bytes(factionId);
            for (int i = 0; i < factions.Length; i++)
            {
                if (!factions[i].FactionId.Equals(targetId))
                    continue;

                auraAttackMultiplier = effects[i].AuraAttackMultiplier;
                auraRadiusBonus = effects[i].AuraRadiusBonus;
                auraSightBonus = effects[i].AuraSightBonus;
                stabilizationMultiplier = effects[i].StabilizationMultiplier;
                captureMultiplier = effects[i].CaptureMultiplier;
                populationGrowthMultiplier = effects[i].PopulationGrowthMultiplier;
                return true;
            }

            return false;
        }
    }
}
