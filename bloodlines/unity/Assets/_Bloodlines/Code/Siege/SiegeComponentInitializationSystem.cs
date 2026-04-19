using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Entities;

namespace Bloodlines.Siege
{
    /// <summary>
    /// Lazy runtime initializer for the first siege / field-water slice.
    /// This keeps the lane additive-only by attaching the new logistics
    /// components after spawn instead of widening shared bootstrap seams.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(SiegeSupportRefreshSystem))]
    public partial struct SiegeComponentInitializationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitTypeComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var fieldWaterCandidates = new List<FieldWaterCandidate>(32);
            var siegeSupportCandidates = new List<SiegeSupportCandidate>(16);
            var supplyTrainCandidates = new List<Entity>(8);
            var supplyCampCandidates = new List<Entity>(4);

            foreach (var (unitType, combatStats, movementStats, entity) in
                SystemAPI.Query<
                    RefRO<UnitTypeComponent>,
                    RefRO<CombatStatsComponent>,
                    RefRO<MovementStatsComponent>>()
                .WithNone<DeadTag>()
                .WithEntityAccess())
            {
                if (FieldWaterCanon.ShouldTrackFieldWater(unitType.ValueRO) &&
                    !entityManager.HasComponent<FieldWaterComponent>(entity))
                {
                    fieldWaterCandidates.Add(new FieldWaterCandidate
                    {
                        Entity = entity,
                        BaseAttackDamage = combatStats.ValueRO.AttackDamage,
                        BaseMaxSpeed = movementStats.ValueRO.MaxSpeed,
                    });
                }

                if (SiegeSupportCanon.ShouldTrackSupport(unitType.ValueRO) &&
                    !entityManager.HasComponent<SiegeSupportComponent>(entity))
                {
                    siegeSupportCandidates.Add(new SiegeSupportCandidate
                    {
                        Entity = entity,
                        IsSiegeEngine = SiegeSupportCanon.IsSiegeEngine(unitType.ValueRO),
                        IsEngineer = SiegeSupportCanon.IsSiegeEngineer(unitType.ValueRO),
                        IsSupplyWagon = SiegeSupportCanon.IsSupplyWagon(unitType.ValueRO),
                    });
                }

                if (SiegeSupportCanon.IsSupplyWagon(unitType.ValueRO) &&
                    !entityManager.HasComponent<SiegeSupplyTrainComponent>(entity))
                {
                    supplyTrainCandidates.Add(entity);
                }
            }

            foreach (var (buildingType, entity) in
                SystemAPI.Query<RefRO<BuildingTypeComponent>>().WithEntityAccess())
            {
                if (!buildingType.ValueRO.SupportsSiegeLogistics ||
                    entityManager.HasComponent<SiegeSupplyCampComponent>(entity))
                {
                    continue;
                }

                supplyCampCandidates.Add(entity);
            }

            for (int i = 0; i < fieldWaterCandidates.Count; i++)
            {
                var candidate = fieldWaterCandidates[i];
                entityManager.AddComponentData(candidate.Entity, new FieldWaterComponent
                {
                    BaseAttackDamage = candidate.BaseAttackDamage,
                    BaseMaxSpeed = candidate.BaseMaxSpeed,
                    SuppliedUntil = 0d,
                    LastTransferAt = -999d,
                    LastSupportRefreshAt = -999d,
                    Strain = 0f,
                    CriticalDuration = 0f,
                    SupportRefreshCount = 0,
                    IsSupported = false,
                    AttritionActive = false,
                    DesertionRisk = false,
                    OperationalAttackMultiplier = 1f,
                    OperationalSpeedMultiplier = 1f,
                    Status = FieldWaterStatus.Steady,
                });
            }

            for (int i = 0; i < siegeSupportCandidates.Count; i++)
            {
                var candidate = siegeSupportCandidates[i];
                entityManager.AddComponentData(candidate.Entity, new SiegeSupportComponent
                {
                    SuppliedUntil = 0d,
                    EngineerSupportUntil = 0d,
                    LastRefreshAt = -999d,
                    RefreshCount = 0,
                    IsSiegeEngine = candidate.IsSiegeEngine,
                    IsEngineer = candidate.IsEngineer,
                    IsSupplyWagon = candidate.IsSupplyWagon,
                    HasLinkedSupplyCamp = false,
                    HasSupplyTrainSupport = false,
                    HasEngineerSupport = false,
                    OperationalAttackMultiplier = 1f,
                    OperationalSpeedMultiplier = 1f,
                    Status = SiegeSupportStatus.Idle,
                });
            }

            for (int i = 0; i < supplyTrainCandidates.Count; i++)
            {
                entityManager.AddComponentData(supplyTrainCandidates[i], new SiegeSupplyTrainComponent
                {
                    LinkedCampEntity = Entity.Null,
                    LastSupplyTransferAt = -999d,
                    LogisticsInterdictedUntil = 0d,
                    ConvoyRecoveryUntil = 0d,
                    ConvoyReconsolidatedAt = 0d,
                    InterdictedByFactionId = default,
                    EscortCount = 0,
                    RequiredEscortCount = 1,
                    EscortScreened = false,
                });
            }

            for (int i = 0; i < supplyCampCandidates.Count; i++)
            {
                entityManager.AddComponentData(supplyCampCandidates[i], new SiegeSupplyCampComponent
                {
                    Stockpile = SiegeSupplyInterdictionCanon.SupplyCampMaxStockpile,
                    MaxStockpile = SiegeSupplyInterdictionCanon.SupplyCampMaxStockpile,
                    OperationalThreshold = SiegeSupplyInterdictionCanon.SupplyCampOperationalThreshold,
                    NearbyRaiderCount = 0,
                    LastInterdictedAt = -999d,
                    LastRecoveredAt = -999d,
                    InterdictedByFactionId = default,
                });
            }
        }

        private struct FieldWaterCandidate
        {
            public Entity Entity;
            public float BaseAttackDamage;
            public float BaseMaxSpeed;
        }

        private struct SiegeSupportCandidate
        {
            public Entity Entity;
            public bool IsSiegeEngine;
            public bool IsEngineer;
            public bool IsSupplyWagon;
        }
    }
}
