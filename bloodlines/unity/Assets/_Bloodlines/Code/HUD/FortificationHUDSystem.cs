using Bloodlines.Components;
using Bloodlines.Fortification;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects settlement fortification state into a HUD-owned read-model without
    /// mutating the underlying fortification or reserve systems.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FortificationDestructionResolutionSystem))]
    [UpdateAfter(typeof(FortificationReserveSystem))]
    [UpdateAfter(typeof(BreachSealingSystem))]
    [UpdateAfter(typeof(DestroyedCounterRecoverySystem))]
    public partial struct FortificationHUDSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FortificationComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (fortification, faction, entity) in SystemAPI.Query<
                         RefRO<FortificationComponent>,
                         RefRO<FactionComponent>>()
                     .WithNone<FortificationHUDComponent>()
                     .WithEntityAccess())
            {
                ecb.AddComponent(entity, new FortificationHUDComponent
                {
                    SettlementId = fortification.ValueRO.SettlementId,
                    OwnerFactionId = faction.ValueRO.FactionId,
                });
            }

            ecb.Playback(entityManager);

            using NativeArray<Entity> linkedEntities = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                    ComponentType.ReadOnly<FortificationReserveAssignmentComponent>(),
                    ComponentType.ReadOnly<PositionComponent>(),
                    ComponentType.ReadOnly<HealthComponent>(),
                    ComponentType.ReadOnly<FactionComponent>())
                .ToEntityArray(Allocator.Temp);

            foreach (var (hudRw, fortification, faction, entity) in SystemAPI.Query<
                         RefRW<FortificationHUDComponent>,
                         RefRO<FortificationComponent>,
                         RefRO<FactionComponent>>()
                     .WithEntityAccess())
            {
                int activeDefenderCount = 0;
                int musteredDefenderCount = 0;
                ResolveReserveTelemetry(
                    entityManager,
                    entity,
                    faction.ValueRO.FactionId,
                    linkedEntities,
                    out activeDefenderCount,
                    out musteredDefenderCount);

                var hud = hudRw.ValueRW;
                hud.SettlementId = fortification.ValueRO.SettlementId;
                hud.OwnerFactionId = faction.ValueRO.FactionId;
                hud.Tier = fortification.ValueRO.Tier;
                hud.OpenBreachCount = fortification.ValueRO.OpenBreachCount;
                hud.ReserveFrontage = math.min(activeDefenderCount, math.max(1, 1 + fortification.ValueRO.Tier));
                hud.MusteredDefenderCount = musteredDefenderCount;

                if (entityManager.HasComponent<FortificationReserveComponent>(entity))
                {
                    var reserve = entityManager.GetComponentData<FortificationReserveComponent>(entity);
                    hud.ReadyReserveCount = reserve.ReadyReserveCount;
                    hud.RecoveringReserveCount = reserve.RecoveringReserveCount;
                    hud.ThreatActive = reserve.ThreatActive;
                }
                else
                {
                    hud.ReadyReserveCount = 0;
                    hud.RecoveringReserveCount = 0;
                    hud.ThreatActive = false;
                }

                hud.SealingProgress01 = ResolveSealingProgress(entityManager, entity, fortification.ValueRO);
                hud.RecoveryProgress01 = ResolveRecoveryProgress(entityManager, entity, fortification.ValueRO);
                hudRw.ValueRW = hud;
            }
        }

        private static void ResolveReserveTelemetry(
            EntityManager entityManager,
            Entity settlementEntity,
            FixedString32Bytes ownerFactionId,
            NativeArray<Entity> linkedEntities,
            out int activeDefenderCount,
            out int musteredDefenderCount)
        {
            activeDefenderCount = 0;
            musteredDefenderCount = 0;

            for (int i = 0; i < linkedEntities.Length; i++)
            {
                Entity defenderEntity = linkedEntities[i];
                var link = entityManager.GetComponentData<FortificationSettlementLinkComponent>(defenderEntity);
                if (link.SettlementEntity != settlementEntity)
                {
                    continue;
                }

                var faction = entityManager.GetComponentData<FactionComponent>(defenderEntity);
                var health = entityManager.GetComponentData<HealthComponent>(defenderEntity);
                if (!faction.FactionId.Equals(ownerFactionId) || health.Current <= 0f)
                {
                    continue;
                }

                activeDefenderCount++;

                var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(defenderEntity);
                if (assignment.Duty == ReserveDutyState.Engaged ||
                    assignment.Duty == ReserveDutyState.Muster)
                {
                    musteredDefenderCount++;
                }
            }
        }

        private static float ResolveSealingProgress(
            EntityManager entityManager,
            Entity settlementEntity,
            in FortificationComponent fortification)
        {
            if (fortification.OpenBreachCount <= 0 ||
                !entityManager.HasComponent<BreachSealingProgressComponent>(settlementEntity))
            {
                return 0f;
            }

            float requiredWorkerHours = FortificationCanon.ResolveBreachSealingWorkerHoursPerBreach(fortification.Tier);
            if (requiredWorkerHours <= 0f)
            {
                return 0f;
            }

            var progress = entityManager.GetComponentData<BreachSealingProgressComponent>(settlementEntity);
            return math.saturate(progress.AccumulatedWorkerHours / requiredWorkerHours);
        }

        private static float ResolveRecoveryProgress(
            EntityManager entityManager,
            Entity settlementEntity,
            in FortificationComponent fortification)
        {
            if (fortification.OpenBreachCount > 0 ||
                !entityManager.HasComponent<DestroyedCounterRecoveryProgressComponent>(settlementEntity))
            {
                return 0f;
            }

            var progress = entityManager.GetComponentData<DestroyedCounterRecoveryProgressComponent>(settlementEntity);
            if (progress.TargetCounter == DestroyedCounterKind.None)
            {
                return 0f;
            }

            float requiredWorkerHours = FortificationCanon.DestroyedCounterRecoveryWorkerHoursPerSegment;
            if (progress.TargetCounter == DestroyedCounterKind.Keep)
            {
                requiredWorkerHours *= FortificationCanon.DestroyedCounterRecoveryKeepMultiplier;
            }

            if (requiredWorkerHours <= 0f)
            {
                return 0f;
            }

            return math.saturate(progress.AccumulatedWorkerHours / requiredWorkerHours);
        }
    }
}
