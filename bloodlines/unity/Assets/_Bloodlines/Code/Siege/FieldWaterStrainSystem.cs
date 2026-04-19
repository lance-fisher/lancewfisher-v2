using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Per-delta field-water strain / recovery and operational-stat application.
    /// Also resolves the live unsupplied siege-engine combat and movement penalties
    /// so combat and movement systems consume already-adjusted runtime stats.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FieldWaterSupportScanSystem))]
    public partial struct FieldWaterStrainSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FieldWaterComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            double elapsed = SystemAPI.Time.ElapsedTime;
            var entityManager = state.EntityManager;

            var commanders = new List<CommanderAuraRecord>(8);
            foreach (var (commander, faction, position, health) in
                SystemAPI.Query<
                    RefRO<CommanderComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>,
                    RefRO<HealthComponent>>()
                .WithNone<DeadTag>())
            {
                if (health.ValueRO.Current <= 0f)
                {
                    continue;
                }

                commanders.Add(new CommanderAuraRecord
                {
                    FactionId = faction.ValueRO.FactionId,
                    Position = position.ValueRO.Value,
                    AuraRadius = FieldWaterCanon.ResolveCommanderAuraRadius(commander.ValueRO.Renown),
                });
            }

            foreach (var (fieldWaterRw, combatRw, movementRw, healthRw, faction, position, entity) in
                SystemAPI.Query<
                    RefRW<FieldWaterComponent>,
                    RefRW<CombatStatsComponent>,
                    RefRW<MovementStatsComponent>,
                    RefRW<HealthComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                ref var fieldWater = ref fieldWaterRw.ValueRW;
                ref var combat = ref combatRw.ValueRW;
                ref var movement = ref movementRw.ValueRW;
                ref var health = ref healthRw.ValueRW;

                if (health.Current <= 0f)
                {
                    continue;
                }

                if (fieldWater.BaseAttackDamage <= 0f && combat.AttackDamage > 0f)
                {
                    fieldWater.BaseAttackDamage = combat.AttackDamage;
                }

                if (fieldWater.BaseMaxSpeed <= 0f && movement.MaxSpeed > 0f)
                {
                    fieldWater.BaseMaxSpeed = movement.MaxSpeed;
                }

                bool supported = fieldWater.SuppliedUntil > elapsed;
                if (supported)
                {
                    fieldWater.Strain = math.max(
                        0f,
                        fieldWater.Strain - dt * FieldWaterCanon.FieldWaterRecoveryPerSecond);
                }
                else
                {
                    fieldWater.Strain = math.min(
                        FieldWaterCanon.FieldWaterCriticalThreshold * 2f,
                        fieldWater.Strain + dt * FieldWaterCanon.FieldWaterStrainPerSecond);
                }

                if (fieldWater.Strain >= FieldWaterCanon.FieldWaterCriticalThreshold)
                {
                    fieldWater.Status = FieldWaterStatus.Critical;
                }
                else if (fieldWater.Strain >= FieldWaterCanon.FieldWaterStrainThreshold)
                {
                    fieldWater.Status = FieldWaterStatus.Strained;
                }
                else if (supported)
                {
                    fieldWater.Status = fieldWater.Strain > 0.2f
                        ? FieldWaterStatus.Recovering
                        : FieldWaterStatus.Steady;
                }
                else
                {
                    fieldWater.Status = FieldWaterStatus.Dry;
                }

                bool commanderAuraActive = HasCommanderAura(
                    commanders,
                    faction.ValueRO.FactionId,
                    position.ValueRO.Value);
                float attritionThreshold = FieldWaterCanon.FieldWaterAttritionThresholdSeconds +
                    (commanderAuraActive ? FieldWaterCanon.FieldWaterCommanderDisciplineBufferSeconds : 0f);
                float desertionThreshold = FieldWaterCanon.FieldWaterDesertionThresholdSeconds +
                    (commanderAuraActive ? FieldWaterCanon.FieldWaterCommanderDisciplineBufferSeconds : 0f);
                float attritionDamagePerSecond = FieldWaterCanon.FieldWaterAttritionDamagePerSecond *
                    (commanderAuraActive ? FieldWaterCanon.FieldWaterCommanderAttritionMultiplier : 1f);

                if (fieldWater.Status == FieldWaterStatus.Critical)
                {
                    fieldWater.CriticalDuration = math.min(
                        desertionThreshold + 18f,
                        fieldWater.CriticalDuration + dt);
                }
                else
                {
                    fieldWater.CriticalDuration = math.max(
                        0f,
                        fieldWater.CriticalDuration - dt * FieldWaterCanon.FieldWaterCriticalRecoveryPerSecond);
                }

                fieldWater.AttritionActive =
                    fieldWater.Status == FieldWaterStatus.Critical &&
                    fieldWater.CriticalDuration >= attritionThreshold;
                fieldWater.DesertionRisk =
                    fieldWater.Status == FieldWaterStatus.Critical &&
                    fieldWater.CriticalDuration >= desertionThreshold;

                if (fieldWater.AttritionActive)
                {
                    health.Current = math.max(0f, health.Current - dt * attritionDamagePerSecond);
                }

                if (health.Current > 0f && fieldWater.DesertionRisk)
                {
                    float desertionFloor = math.max(0f, health.Max) * FieldWaterCanon.FieldWaterDesertionHealthRatio;
                    if (health.Current <= desertionFloor)
                    {
                        health.Current = 0f;
                    }
                }

                float siegeAttackMultiplier = 1f;
                float siegeSpeedMultiplier = 1f;
                if (entityManager.HasComponent<SiegeSupportComponent>(entity))
                {
                    var siegeSupport = entityManager.GetComponentData<SiegeSupportComponent>(entity);
                    siegeSupport.HasSupplyTrainSupport = siegeSupport.SuppliedUntil > elapsed;
                    siegeSupport.HasEngineerSupport = siegeSupport.EngineerSupportUntil > elapsed;
                    siegeSupport.OperationalAttackMultiplier = SiegeSupportCanon.ResolveAttackMultiplier(
                        siegeSupport.HasSupplyTrainSupport);
                    siegeSupport.OperationalSpeedMultiplier = SiegeSupportCanon.ResolveSpeedMultiplier(
                        siegeSupport.HasSupplyTrainSupport);

                    if (siegeSupport.IsSiegeEngine)
                    {
                        siegeSupport.Status = !siegeSupport.HasSupplyTrainSupport
                            ? SiegeSupportStatus.Starved
                            : siegeSupport.HasEngineerSupport
                                ? SiegeSupportStatus.Supporting
                                : SiegeSupportStatus.Idle;
                    }
                    else if (siegeSupport.IsSupplyWagon &&
                             siegeSupport.Status != SiegeSupportStatus.Interdicted &&
                             siegeSupport.Status != SiegeSupportStatus.RecoveringUnscreened &&
                             siegeSupport.Status != SiegeSupportStatus.RecoveringScreened &&
                             !siegeSupport.HasLinkedSupplyCamp)
                    {
                        siegeSupport.Status = SiegeSupportStatus.CutOff;
                    }

                    siegeAttackMultiplier = siegeSupport.OperationalAttackMultiplier;
                    siegeSpeedMultiplier = siegeSupport.OperationalSpeedMultiplier;
                    entityManager.SetComponentData(entity, siegeSupport);
                }

                float breachAttackMultiplier = fieldWater.BreachAssaultAdvantageActive
                    ? math.max(1f, fieldWater.BreachAssaultAttackMultiplier)
                    : 1f;
                float breachSpeedMultiplier = fieldWater.BreachAssaultAdvantageActive
                    ? math.max(1f, fieldWater.BreachAssaultSpeedMultiplier)
                    : 1f;

                fieldWater.IsSupported = supported;
                fieldWater.OperationalAttackMultiplier =
                    FieldWaterCanon.ResolveAttackMultiplier(fieldWater.Strain) *
                    siegeAttackMultiplier *
                    breachAttackMultiplier;
                fieldWater.OperationalSpeedMultiplier =
                    FieldWaterCanon.ResolveSpeedMultiplier(fieldWater.Strain) *
                    siegeSpeedMultiplier *
                    breachSpeedMultiplier;

                if (health.Current <= 0f)
                {
                    combat.AttackDamage = 0f;
                    movement.MaxSpeed = 0f;
                    continue;
                }

                combat.AttackDamage = fieldWater.BaseAttackDamage * fieldWater.OperationalAttackMultiplier;
                movement.MaxSpeed = fieldWater.BaseMaxSpeed * fieldWater.OperationalSpeedMultiplier;
            }
        }

        private static bool HasCommanderAura(
            List<CommanderAuraRecord> commanders,
            FixedString32Bytes factionId,
            float3 unitPosition)
        {
            for (int i = 0; i < commanders.Count; i++)
            {
                if (!commanders[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                if (Distance2D(commanders[i].Position, unitPosition) <= commanders[i].AuraRadius)
                {
                    return true;
                }
            }

            return false;
        }

        private static float Distance2D(float3 a, float3 b)
        {
            return math.distance(a.xz, b.xz);
        }

        private struct CommanderAuraRecord
        {
            public FixedString32Bytes FactionId;
            public float3 Position;
            public float AuraRadius;
        }
    }
}
