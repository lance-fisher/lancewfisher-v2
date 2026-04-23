using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Applies the daily ongoing effects of an active succession crisis and
    /// resolves it once recovery progress reaches completion.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SuccessionCrisisEvaluationSystem))]
    public partial struct SuccessionCrisisRecoverySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;
            float currentDay = math.floor(SystemAPI.GetSingleton<DualClockComponent>().InWorldDays);

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<DynastyStateComponent>(),
                ComponentType.ReadWrite<SuccessionCrisisComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity factionEntity = factionEntities[i];
                var crisis = entityManager.GetComponentData<SuccessionCrisisComponent>(factionEntity);
                int elapsedWholeDays = (int)(currentDay - math.floor(crisis.LastDailyTickInWorldDays));
                if (elapsedWholeDays <= 0)
                {
                    continue;
                }

                var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
                dynasty.Legitimacy = math.clamp(
                    dynasty.Legitimacy - (crisis.LegitimacyDrainRatePerDay * elapsedWholeDays),
                    0f,
                    100f);
                entityManager.SetComponentData(factionEntity, dynasty);

                FixedString32Bytes factionId = entityManager.GetComponentData<FactionComponent>(factionEntity).FactionId;
                ApplyDailyLoyaltyDrain(entityManager, factionId, crisis.LoyaltyDrainRatePerDay * elapsedWholeDays);

                float convictionRecoveryMultiplier = ResolveRecoveryMultiplier(entityManager, factionEntity);
                crisis.RecoveryProgressPct = math.saturate(
                    crisis.RecoveryProgressPct + (crisis.RecoveryRatePerDay * convictionRecoveryMultiplier * elapsedWholeDays));
                crisis.LastDailyTickInWorldDays = currentDay;

                if (crisis.RecoveryProgressPct >= 1f)
                {
                    ecb.RemoveComponent<SuccessionCrisisComponent>(factionEntity);
                    continue;
                }

                entityManager.SetComponentData(factionEntity, crisis);
            }

            ecb.Playback(entityManager);
        }

        private static void ApplyDailyLoyaltyDrain(EntityManager entityManager, FixedString32Bytes factionId, float loyaltyDelta)
        {
            if (loyaltyDelta <= 0f)
            {
                return;
            }

            using var controlPointQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<Entity> controlPointEntities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<ControlPointComponent> controlPoints =
                controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            for (int i = 0; i < controlPointEntities.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(factionId))
                {
                    continue;
                }

                var controlPoint = controlPoints[i];
                controlPoint.Loyalty = math.clamp(controlPoint.Loyalty - loyaltyDelta, 0f, 100f);
                entityManager.SetComponentData(controlPointEntities[i], controlPoint);
            }
        }

        private static float ResolveRecoveryMultiplier(EntityManager entityManager, Entity factionEntity)
        {
            if (!entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                return 1f;
            }

            ConvictionBand band = entityManager.GetComponentData<ConvictionComponent>(factionEntity).Band;
            return band switch
            {
                ConvictionBand.ApexMoral => 1.22f,
                ConvictionBand.Moral => 1.08f,
                ConvictionBand.Cruel => 0.96f,
                ConvictionBand.ApexCruel => 0.88f,
                _ => 1f,
            };
        }
    }
}
