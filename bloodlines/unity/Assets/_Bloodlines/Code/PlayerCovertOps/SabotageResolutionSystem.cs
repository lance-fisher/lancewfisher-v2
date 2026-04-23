using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Resolves ready sabotage operations and owns the additive live sabotage timers
    /// on buildings. The production pause is implemented locally by compensating the
    /// queue after UnitProductionSystem runs, so no foreign runtime files need edits.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCounterIntelligenceSystem))]
    [UpdateAfter(typeof(UnitProductionSystem))]
    [UpdateBefore(typeof(DeathResolutionSystem))]
    public partial struct SabotageResolutionSystem : ISystem
    {
        private const float SabotageFailureLegitimacyPenalty = 2f;
        private const float FireDamagePerSecond = 8f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var clock = SystemAPI.GetSingleton<DualClockComponent>();
            float inWorldDays = clock.InWorldDays;
            float deltaTime = SystemAPI.Time.DeltaTime;

            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            TickActiveSabotageEffects(entityManager, ecb, inWorldDays, deltaTime);

            var operationQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            if (operationQuery.IsEmpty)
            {
                operationQuery.Dispose();
                return;
            }

            using var operationEntities = operationQuery.ToEntityArray(Allocator.Temp);
            using var operations = operationQuery.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            operationQuery.Dispose();

            for (int i = 0; i < operationEntities.Length; i++)
            {
                var operation = operations[i];
                if (!operation.Active ||
                    operation.Kind != CovertOpKindPlayer.Sabotage ||
                    operation.ResolveAtInWorldDays > inWorldDays)
                {
                    continue;
                }

                ResolveOperation(
                    entityManager,
                    ecb,
                    operationEntities[i],
                    operation,
                    inWorldDays,
                    clock.DaysPerRealSecond,
                    deltaTime);
            }
        }

        private static void ResolveOperation(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            Entity operationEntity,
            in PlayerCovertOpsResolutionComponent operation,
            float inWorldDays,
            float daysPerRealSecond,
            float deltaTime)
        {
            var sourceFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.SourceFactionId);
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.TargetFactionId);

            if (!TryResolveBuildingTarget(
                    entityManager,
                    operation.TargetEntityIndex,
                    out var targetBuildingEntity,
                    out var buildingFaction,
                    out var buildingHealth) ||
                !buildingFaction.FactionId.Equals(operation.TargetFactionId) ||
                buildingHealth.Current <= 0f ||
                entityManager.HasComponent<DeadTag>(targetBuildingEntity))
            {
                ecb.DestroyEntity(operationEntity);
                return;
            }

            if (operation.SuccessScore >= 0f)
            {
                var profile = ResolveSabotageProfile(operation.Subtype, daysPerRealSecond);
                ApplySabotageSuccess(
                    entityManager,
                    targetBuildingEntity,
                    operation.SourceFactionId,
                    operation.TargetFactionId,
                    operation.Subtype,
                    inWorldDays,
                    deltaTime,
                    profile);

                if (targetFactionEntity != Entity.Null &&
                    operation.Subtype.Equals(new FixedString32Bytes("well_poisoning")) &&
                    entityManager.HasComponent<RealmConditionComponent>(targetFactionEntity))
                {
                    var realm = entityManager.GetComponentData<RealmConditionComponent>(targetFactionEntity);
                    realm.WaterStrainStreak += 2;
                    entityManager.SetComponentData(targetFactionEntity, realm);
                }

                ApplySubtypeConviction(entityManager, sourceFactionEntity, operation.Subtype);
            }
            else
            {
                PlayerCounterIntelligenceSystem.ApplyStewardship(entityManager, targetFactionEntity, 1f);
                PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                    entityManager,
                    sourceFactionEntity,
                    -SabotageFailureLegitimacyPenalty);
            }

            ecb.DestroyEntity(operationEntity);
        }

        private static void TickActiveSabotageEffects(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            float inWorldDays,
            float deltaTime)
        {
            var statusQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerSabotageStatusComponent>());
            if (statusQuery.IsEmpty)
            {
                statusQuery.Dispose();
                return;
            }

            using var entities = statusQuery.ToEntityArray(Allocator.Temp);
            using var statuses = statusQuery.ToComponentDataArray<PlayerSabotageStatusComponent>(Allocator.Temp);
            statusQuery.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var status = statuses[i];
                bool stillActive =
                    status.EffectExpiresAtInWorldDays > inWorldDays ||
                    status.ProductionHaltExpiresAtInWorldDays > inWorldDays ||
                    status.GateExposureExpiresAtInWorldDays > inWorldDays ||
                    status.BurnExpiresAtInWorldDays > inWorldDays;

                if (status.ProductionHaltExpiresAtInWorldDays > inWorldDays &&
                    entityManager.HasBuffer<ProductionQueueItemElement>(entities[i]))
                {
                    var queue = entityManager.GetBuffer<ProductionQueueItemElement>(entities[i]);
                    if (queue.Length > 0)
                    {
                        var queueItem = queue[0];
                        queueItem.RemainingSeconds = math.min(queueItem.TotalSeconds, queueItem.RemainingSeconds + deltaTime);
                        queue[0] = queueItem;
                    }
                }

                if (status.BurnExpiresAtInWorldDays > inWorldDays &&
                    status.BurnDamagePerSecond > 0f &&
                    entityManager.HasComponent<HealthComponent>(entities[i]) &&
                    !entityManager.HasComponent<DeadTag>(entities[i]))
                {
                    var health = entityManager.GetComponentData<HealthComponent>(entities[i]);
                    if (health.Current > 0f)
                    {
                        health.Current = math.max(0f, health.Current - (status.BurnDamagePerSecond * deltaTime));
                        entityManager.SetComponentData(entities[i], health);
                    }
                }

                if (!stillActive)
                {
                    ecb.RemoveComponent<PlayerSabotageStatusComponent>(entities[i]);
                }
            }
        }

        private static void ApplySabotageSuccess(
            EntityManager entityManager,
            Entity buildingEntity,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString32Bytes subtype,
            float inWorldDays,
            float deltaTime,
            in SabotageEffectProfile profile)
        {
            if (entityManager.HasComponent<HealthComponent>(buildingEntity))
            {
                var health = entityManager.GetComponentData<HealthComponent>(buildingEntity);
                float damageFloor = math.max(1f, health.Max * profile.DamageFloorRatio);
                health.Current = math.min(health.Current, damageFloor);
                entityManager.SetComponentData(buildingEntity, health);
            }

            var status = new PlayerSabotageStatusComponent
            {
                SourceFactionId = sourceFactionId,
                TargetFactionId = targetFactionId,
                Subtype = subtype,
                AppliedAtInWorldDays = inWorldDays,
                EffectExpiresAtInWorldDays = inWorldDays + profile.EffectDurationInWorldDays,
                ProductionHaltExpiresAtInWorldDays = inWorldDays + profile.ProductionHaltDurationInWorldDays,
                GateExposureExpiresAtInWorldDays = inWorldDays + profile.GateExposureDurationInWorldDays,
                BurnExpiresAtInWorldDays = inWorldDays + profile.BurnDurationInWorldDays,
                BurnDamagePerSecond = profile.BurnDamagePerSecond,
                DamageFloorRatio = profile.DamageFloorRatio,
            };

            if (entityManager.HasComponent<PlayerSabotageStatusComponent>(buildingEntity))
            {
                entityManager.SetComponentData(buildingEntity, status);
            }
            else
            {
                entityManager.AddComponentData(buildingEntity, status);
            }

            if (profile.ProductionHaltDurationInWorldDays > 0f &&
                entityManager.HasBuffer<ProductionQueueItemElement>(buildingEntity))
            {
                var queue = entityManager.GetBuffer<ProductionQueueItemElement>(buildingEntity);
                if (queue.Length > 0)
                {
                    var queueItem = queue[0];
                    queueItem.RemainingSeconds = math.min(queueItem.TotalSeconds, queueItem.RemainingSeconds + deltaTime);
                    queue[0] = queueItem;
                }
            }
        }

        private static void ApplySubtypeConviction(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes subtype)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                return;
            }

            var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            var bucket =
                subtype.Equals(new FixedString32Bytes("well_poisoning")) ||
                subtype.Equals(new FixedString32Bytes("supply_poisoning"))
                    ? ConvictionBucket.Desecration
                    : ConvictionBucket.Ruthlessness;
            ConvictionScoring.ApplyEvent(ref conviction, bucket, 2f);
            entityManager.SetComponentData(factionEntity, conviction);
        }

        private static bool TryResolveBuildingTarget(
            EntityManager entityManager,
            int entityIndex,
            out Entity targetEntity,
            out FactionComponent buildingFaction,
            out HealthComponent buildingHealth)
        {
            targetEntity = Entity.Null;
            buildingFaction = default;
            buildingHealth = default;

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].Index != entityIndex)
                {
                    continue;
                }

                targetEntity = entities[i];
                buildingFaction = factions[i];
                buildingHealth = healthValues[i];
                return true;
            }

            return false;
        }

        private static SabotageEffectProfile ResolveSabotageProfile(
            FixedString32Bytes subtype,
            float daysPerRealSecond)
        {
            if (subtype.Equals(new FixedString32Bytes("gate_opening")))
            {
                float duration = SecondsToInWorldDays(15f, daysPerRealSecond);
                return new SabotageEffectProfile
                {
                    DamageFloorRatio = 0.20f,
                    EffectDurationInWorldDays = duration,
                    ProductionHaltDurationInWorldDays = duration,
                    GateExposureDurationInWorldDays = duration,
                };
            }

            if (subtype.Equals(new FixedString32Bytes("fire_raising")))
            {
                float duration = SecondsToInWorldDays(10f, daysPerRealSecond);
                return new SabotageEffectProfile
                {
                    DamageFloorRatio = 0.70f,
                    EffectDurationInWorldDays = duration,
                    ProductionHaltDurationInWorldDays = duration,
                    BurnDurationInWorldDays = duration,
                    BurnDamagePerSecond = FireDamagePerSecond,
                };
            }

            if (subtype.Equals(new FixedString32Bytes("supply_poisoning")))
            {
                float duration = SecondsToInWorldDays(20f, daysPerRealSecond);
                return new SabotageEffectProfile
                {
                    DamageFloorRatio = 0.80f,
                    EffectDurationInWorldDays = duration,
                    ProductionHaltDurationInWorldDays = duration,
                };
            }

            float defaultDuration = SecondsToInWorldDays(20f, daysPerRealSecond);
            return new SabotageEffectProfile
            {
                DamageFloorRatio = 0.85f,
                EffectDurationInWorldDays = defaultDuration,
                ProductionHaltDurationInWorldDays = defaultDuration,
            };
        }

        private static float SecondsToInWorldDays(float seconds, float daysPerRealSecond)
        {
            return seconds * math.max(0.0001f, daysPerRealSecond) / 86400f;
        }

        private struct SabotageEffectProfile
        {
            public float DamageFloorRatio;
            public float EffectDurationInWorldDays;
            public float ProductionHaltDurationInWorldDays;
            public float GateExposureDurationInWorldDays;
            public float BurnDurationInWorldDays;
            public float BurnDamagePerSecond;
        }
    }
}
