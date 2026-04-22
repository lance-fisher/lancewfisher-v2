using Bloodlines.Components;
using Bloodlines.Conviction;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    public struct PendingCommanderCaptureComponent : IComponentData
    {
        public FixedString32Bytes CaptorFactionId;
    }

    public static class CommanderCaptureUtility
    {
        public static bool TryGetFactionConvictionBand(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out ConvictionBand band)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<ConvictionComponent>());

            using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var convictions = query.ToComponentDataArray<ConvictionComponent>(Unity.Collections.Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                band = convictions[i].Band;
                return true;
            }

            band = ConvictionBand.Neutral;
            return false;
        }

        public static bool TryMarkPendingCommanderCapture(
            EntityManager entityManager,
            Entity attackerEntity,
            FixedString32Bytes attackerFactionId,
            Entity targetEntity,
            ConvictionBand attackerBand)
        {
            if (!entityManager.Exists(targetEntity) ||
                entityManager.HasComponent<DeadTag>(targetEntity) ||
                !entityManager.HasComponent<CommanderComponent>(targetEntity))
            {
                return false;
            }

            float captureChance = ResolveCommanderCaptureChance(attackerBand);
            if (captureChance <= 0f ||
                !ShouldCaptureCommander(attackerEntity.Index, targetEntity.Index, captureChance))
            {
                return false;
            }

            var pendingCapture = new PendingCommanderCaptureComponent
            {
                CaptorFactionId = attackerFactionId,
            };

            if (entityManager.HasComponent<PendingCommanderCaptureComponent>(targetEntity))
            {
                entityManager.SetComponentData(targetEntity, pendingCapture);
            }
            else
            {
                entityManager.AddComponentData(targetEntity, pendingCapture);
            }

            return true;
        }

        public static float ResolveCommanderCaptureChance(ConvictionBand band)
        {
            float captureMultiplier = ConvictionBandEffects.ForBand(band).CaptureMultiplier;
            return math.saturate(captureMultiplier - 1f);
        }

        public static bool ShouldCaptureCommander(int attackerEntityIndex, int targetEntityIndex, float captureChance)
        {
            if (captureChance <= 0f)
            {
                return false;
            }

            uint hash = math.hash(new uint2(
                (uint)math.max(0, attackerEntityIndex) + 1u,
                (uint)math.max(0, targetEntityIndex) + 1u));
            float roll = (hash & 0x00FFFFFFu) / 16777215f;
            return roll < math.saturate(captureChance);
        }
    }
}
