using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Bloodlines.Raids;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Resolves sabotage operations and ticks the temporary gate-exposure and
    /// burning windows they materialize on target buildings.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Bloodlines.Fortification.FortificationDestructionResolutionSystem))]
    [UpdateAfter(typeof(PlayerCounterIntelligenceSystem))]
    [UpdateBefore(typeof(Bloodlines.Siege.BreachAssaultPressureSystem))]
    public partial struct SabotageResolutionSystem : ISystem
    {
        private const double GateExposeDurationSeconds = 15d;
        private const double BurnDurationSeconds = 10d;
        private const double PoisonDurationSeconds = 20d;
        private const float BurnDamagePerSecond = 8f;
        private const float GateHealthFloorRatio = 0.2f;
        private const float SabotageConvictionGain = 2f;
        private const int WellPoisoningWaterStrainGain = 2;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerCovertOpsSystem.GetInWorldDays(entityManager);
            double elapsed = SystemAPI.Time.ElapsedTime;
            float dt = SystemAPI.Time.DeltaTime;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            try
            {
                TickActiveSabotageEffects(entityManager, ref ecb, elapsed, dt);

                var query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
                if (!query.IsEmpty)
                {
                    using var entities = query.ToEntityArray(Allocator.Temp);
                    using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
                    query.Dispose();

                    for (int i = 0; i < entities.Length; i++)
                    {
                        var operation = operations[i];
                        if (!operation.Active ||
                            operation.Kind != CovertOpKindPlayer.Sabotage ||
                            operation.ResolveAtInWorldDays > inWorldDays)
                        {
                            continue;
                        }

                        ResolveSabotageOperation(
                            entityManager,
                            ref ecb,
                            entities[i],
                            operation,
                            elapsed);
                    }
                }
                else
                {
                    query.Dispose();
                }

                ecb.Playback(entityManager);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        private static void ResolveSabotageOperation(
            EntityManager entityManager,
            ref EntityCommandBuffer ecb,
            Entity operationEntity,
            in PlayerCovertOpsResolutionComponent operation,
            double elapsed)
        {
            if (!PlayerCovertOpsSystem.TryResolveBuildingTarget(
                    entityManager,
                    operation.TargetEntityIndex,
                    out var targetBuildingEntity,
                    out _,
                    out var buildingFaction,
                    out var buildingHealth,
                    out _)
                || buildingHealth.Current <= 0f)
            {
                ecb.DestroyEntity(operationEntity);
                return;
            }

            var sourceFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.SourceFactionId);
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, buildingFaction.FactionId);
            if (targetFactionEntity == Entity.Null)
            {
                ecb.DestroyEntity(operationEntity);
                return;
            }

            if (operation.SuccessScore >= 0f)
            {
                ApplySabotageEffect(
                    entityManager,
                    targetBuildingEntity,
                    sourceFactionEntity,
                    targetFactionEntity,
                    operation.Subtype,
                    elapsed);
            }
            else
            {
                PlayerCounterIntelligenceSystem.ApplyStewardship(entityManager, targetFactionEntity, 1f);
            }

            ecb.DestroyEntity(operationEntity);
        }

        private static void ApplySabotageEffect(
            EntityManager entityManager,
            Entity targetBuildingEntity,
            Entity sourceFactionEntity,
            Entity targetFactionEntity,
            FixedString32Bytes subtype,
            double elapsed)
        {
            if (subtype.Equals(new FixedString32Bytes("gate_opening")))
            {
                var health = entityManager.GetComponentData<HealthComponent>(targetBuildingEntity);
                float healthFloor = math.max(0f, health.Max * GateHealthFloorRatio);
                health.Current = math.min(health.Current, healthFloor);
                entityManager.SetComponentData(targetBuildingEntity, health);

                SetSabotageEffect(
                    entityManager,
                    targetBuildingEntity,
                    gateExposedUntil: elapsed + GateExposeDurationSeconds,
                    burningUntil: 0d,
                    burnDamagePerSecond: 0f);
                PromoteTemporaryBreach(entityManager, targetBuildingEntity);
                ApplySuccessConviction(entityManager, sourceFactionEntity, subtype);
                return;
            }

            if (subtype.Equals(new FixedString32Bytes("fire_raising")))
            {
                SetSabotageEffect(
                    entityManager,
                    targetBuildingEntity,
                    gateExposedUntil: 0d,
                    burningUntil: elapsed + BurnDurationSeconds,
                    burnDamagePerSecond: BurnDamagePerSecond);
                ApplySuccessConviction(entityManager, sourceFactionEntity, subtype);
                return;
            }

            if (subtype.Equals(new FixedString32Bytes("supply_poisoning")))
            {
                UpsertRaidState(
                    entityManager,
                    targetBuildingEntity,
                    sourceFactionEntity,
                    elapsed + PoisonDurationSeconds);
                ApplySuccessConviction(entityManager, sourceFactionEntity, subtype);
                return;
            }

            if (subtype.Equals(new FixedString32Bytes("well_poisoning")))
            {
                if (entityManager.HasComponent<RealmConditionComponent>(targetFactionEntity))
                {
                    var realm = entityManager.GetComponentData<RealmConditionComponent>(targetFactionEntity);
                    realm.WaterStrainStreak += WellPoisoningWaterStrainGain;
                    entityManager.SetComponentData(targetFactionEntity, realm);
                }

                ApplySuccessConviction(entityManager, sourceFactionEntity, subtype);
            }
        }

        private static void TickActiveSabotageEffects(
            EntityManager entityManager,
            ref EntityCommandBuffer ecb,
            double elapsed,
            float dt)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerSabotageEffectComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var effects = query.ToComponentDataArray<PlayerSabotageEffectComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var effect = effects[i];
                var health = healthValues[i];
                bool keepEffect = false;

                if (effect.BurningUntil > elapsed &&
                    effect.BurnDamagePerSecond > 0f &&
                    health.Current > 0f)
                {
                    health.Current = math.max(0f, health.Current - (effect.BurnDamagePerSecond * dt));
                    entityManager.SetComponentData(entities[i], health);
                    keepEffect = true;
                }

                if (effect.GateExposedUntil > elapsed)
                {
                    keepEffect = true;
                    PromoteTemporaryBreach(entityManager, entities[i]);
                }

                if (!keepEffect)
                {
                    ecb.RemoveComponent<PlayerSabotageEffectComponent>(entities[i]);
                }
            }
        }

        private static void SetSabotageEffect(
            EntityManager entityManager,
            Entity targetBuildingEntity,
            double gateExposedUntil,
            double burningUntil,
            float burnDamagePerSecond)
        {
            var effect = entityManager.HasComponent<PlayerSabotageEffectComponent>(targetBuildingEntity)
                ? entityManager.GetComponentData<PlayerSabotageEffectComponent>(targetBuildingEntity)
                : default;

            effect.GateExposedUntil = math.max(effect.GateExposedUntil, gateExposedUntil);
            effect.BurningUntil = math.max(effect.BurningUntil, burningUntil);
            effect.BurnDamagePerSecond = math.max(effect.BurnDamagePerSecond, burnDamagePerSecond);

            if (entityManager.HasComponent<PlayerSabotageEffectComponent>(targetBuildingEntity))
            {
                entityManager.SetComponentData(targetBuildingEntity, effect);
            }
            else
            {
                entityManager.AddComponentData(targetBuildingEntity, effect);
            }
        }

        private static void PromoteTemporaryBreach(
            EntityManager entityManager,
            Entity targetBuildingEntity)
        {
            if (!entityManager.HasComponent<FortificationSettlementLinkComponent>(targetBuildingEntity))
            {
                return;
            }

            var link = entityManager.GetComponentData<FortificationSettlementLinkComponent>(targetBuildingEntity);
            if (link.SettlementEntity == Entity.Null ||
                !entityManager.Exists(link.SettlementEntity) ||
                !entityManager.HasComponent<FortificationComponent>(link.SettlementEntity))
            {
                return;
            }

            var fortification = entityManager.GetComponentData<FortificationComponent>(link.SettlementEntity);
            fortification.OpenBreachCount = math.max(fortification.OpenBreachCount, 1);
            entityManager.SetComponentData(link.SettlementEntity, fortification);
        }

        private static void UpsertRaidState(
            EntityManager entityManager,
            Entity targetBuildingEntity,
            Entity sourceFactionEntity,
            double poisonedUntil)
        {
            var raidState = entityManager.HasComponent<BuildingRaidStateComponent>(targetBuildingEntity)
                ? entityManager.GetComponentData<BuildingRaidStateComponent>(targetBuildingEntity)
                : default;

            raidState.RaidedUntil = math.max(raidState.RaidedUntil, poisonedUntil);
            raidState.LastRaidedAt = math.max(raidState.LastRaidedAt, poisonedUntil - PoisonDurationSeconds);
            raidState.RaidedByFactionId = sourceFactionEntity != Entity.Null &&
                                          entityManager.HasComponent<FactionComponent>(sourceFactionEntity)
                ? entityManager.GetComponentData<FactionComponent>(sourceFactionEntity).FactionId
                : default;

            if (entityManager.HasComponent<BuildingRaidStateComponent>(targetBuildingEntity))
            {
                entityManager.SetComponentData(targetBuildingEntity, raidState);
            }
            else
            {
                entityManager.AddComponentData(targetBuildingEntity, raidState);
            }
        }

        private static void ApplySuccessConviction(
            EntityManager entityManager,
            Entity sourceFactionEntity,
            FixedString32Bytes subtype)
        {
            var bucket =
                subtype.Equals(new FixedString32Bytes("well_poisoning")) ||
                subtype.Equals(new FixedString32Bytes("supply_poisoning"))
                    ? ConvictionBucket.Desecration
                    : ConvictionBucket.Ruthlessness;
            PlayerCounterIntelligenceSystem.ApplyConviction(
                entityManager,
                sourceFactionEntity,
                bucket,
                SabotageConvictionGain);
        }
    }
}
