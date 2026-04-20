using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Ports the browser's splinter-levy cadence for active minor houses.
    /// Browser reference: simulation.js tickMinorHouseTerritorialLevies (~7060).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Bloodlines.Systems.ControlPointResourceTrickleSystem))]
    public partial struct MinorHouseLevySystem : ISystem
    {
        private const float MinimumClaimLoyalty = 48f;
        private const float ProgressDecayPerSecond = 0.6f;
        private const float PressureTempoPerLevel = 0.22f;
        private const float PressureRetakeTempoPerLevel = 0.28f;
        private const float PressureScoreBonusPerPoint = 0.03f;
        private const float MaxPressureScoreBonus = 0.18f;
        private const float ProjectileMaxLifetimeSeconds = 4f;
        private const float ProjectileArrivalRadius = 0.4f;

        private static readonly LevyProfile MilitiaProfile = new LevyProfile
        {
            UnitId = new FixedString64Bytes("militia"),
            Role = UnitRole.Melee,
            FoodCost = 6f,
            InfluenceCost = 5f,
            LoyaltyCost = 3f,
            SecondsRequired = 22f,
            PopulationCost = 1,
            MaxHealth = 80f,
            MaxSpeed = 64f,
            AttackDamage = 9f,
            AttackRange = 18f,
            AttackCooldown = 0.95f,
            Sight = 118f,
            SeparationRadius = 0f,
        };

        private static readonly LevyProfile SwordsmanProfile = new LevyProfile
        {
            UnitId = new FixedString64Bytes("swordsman"),
            Role = UnitRole.MeleeRecon,
            FoodCost = 8f,
            InfluenceCost = 7f,
            LoyaltyCost = 4f,
            SecondsRequired = 28f,
            PopulationCost = 1,
            MaxHealth = 100f,
            MaxSpeed = 70f,
            AttackDamage = 12f,
            AttackRange = 18f,
            AttackCooldown = 0.9f,
            Sight = 145f,
            SeparationRadius = 0f,
        };

        private static readonly LevyProfile BowmanProfile = new LevyProfile
        {
            UnitId = new FixedString64Bytes("bowman"),
            Role = UnitRole.Ranged,
            FoodCost = 7f,
            InfluenceCost = 8f,
            LoyaltyCost = 4f,
            SecondsRequired = 26f,
            PopulationCost = 1,
            MaxHealth = 70f,
            MaxSpeed = 62f,
            AttackDamage = 10f,
            AttackRange = 122f,
            AttackCooldown = 1.15f,
            Sight = 150f,
            ProjectileSpeed = 280f,
            SeparationRadius = 0f,
        };

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MinorHouseLevyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
            {
                dt = 0.016f;
            }

            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>(),
                typeof(MinorHouseLevyComponent));
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var factionKinds = factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            using var levyComponents = factionQuery.ToComponentDataArray<MinorHouseLevyComponent>(Allocator.Temp);
            factionQuery.Dispose();

            var controlPointQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var controlPointEntities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var controlPointPositions = controlPointQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            controlPointQuery.Dispose();

            var unitQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var unitHealth = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            unitQuery.Dispose();

            var pressureQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<WorldPressureComponent>());
            using var pressureFactionIds = pressureQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var worldPressure = pressureQuery.ToComponentDataArray<WorldPressureComponent>(Allocator.Temp);
            pressureQuery.Dispose();

            float currentDay = GetInWorldDays(em);
            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.MinorHouse)
                {
                    continue;
                }

                Entity factionEntity = factionEntities[i];
                FixedString32Bytes factionId = factionIds[i].FactionId;
                var levy = levyComponents[i];

                EnsureSupportingComponents(em, factionEntity);

                var pressure = BuildParentPressureProfile(
                    pressureFactionIds,
                    worldPressure,
                    levy.OriginFactionId);
                levy.ParentPressureLevel = pressure.Level;
                levy.ParentPressureStatus = pressure.Status;
                levy.ParentPressureLevyTempo = pressure.LevyTempo;
                levy.ParentPressureRetakeTempo = pressure.RetakeTempo;
                levy.ParentPressureRetinueBonus = pressure.RetinueCapBonus;

                int retinueCount = CountRetinueUnits(unitFactions, unitTypes, unitHealth, factionId);
                levy.RetinueCount = retinueCount;

                bool hasClaim = TryResolveClaim(
                    controlPointEntities,
                    controlPoints,
                    controlPointPositions,
                    factionId,
                    ref levy,
                    out Entity claimEntity,
                    out ControlPointComponent claim,
                    out float3 claimPosition,
                    out int territoryCount);

                levy.RetinueCap = hasClaim
                    ? ResolveRetinueCap(claim, territoryCount, levy.ParentPressureRetinueBonus)
                    : math.max(1, 1 + levy.ParentPressureRetinueBonus);

                if (!hasClaim)
                {
                    levy.LevyStatus = MinorHouseLevyStatus.Landless;
                    DecayProgress(ref levy, dt, 1f);
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                if (!claim.OwnerFactionId.Equals(factionId))
                {
                    levy.LevyStatus = MinorHouseLevyStatus.Dispossessed;
                    DecayProgress(ref levy, dt, 1f);
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                if (claim.IsContested)
                {
                    levy.LevyStatus = MinorHouseLevyStatus.Contested;
                    DecayProgress(ref levy, dt, 1f);
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                if (claim.Loyalty < MinimumClaimLoyalty)
                {
                    levy.LevyStatus = MinorHouseLevyStatus.Unsettled;
                    DecayProgress(ref levy, dt, 1f);
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                if (retinueCount >= levy.RetinueCap)
                {
                    levy.LevyStatus = MinorHouseLevyStatus.Mustered;
                    levy.LevyAccumulator = 0f;
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                LevyProfile profile = ResolveLevyProfile(claim.Loyalty, retinueCount);
                levy.LevyUnitId = profile.UnitId;
                levy.LevyIntervalSeconds = profile.SecondsRequired;

                var resources = em.GetComponentData<ResourceStockpileComponent>(factionEntity);
                bool hasFood = resources.Food >= profile.FoodCost;
                bool hasInfluence = resources.Influence >= profile.InfluenceCost;
                if (!hasFood || !hasInfluence)
                {
                    levy.LevyStatus = hasFood
                        ? MinorHouseLevyStatus.AwaitingInfluence
                        : MinorHouseLevyStatus.AwaitingFood;
                    DecayProgress(ref levy, dt, 0.4f);
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                float loyaltyTempo = 1f + math.max(0f, claim.Loyalty - MinimumClaimLoyalty) / 80f;
                levy.LevyAccumulator += dt * loyaltyTempo * levy.ParentPressureLevyTempo;
                levy.LevyStatus = MinorHouseLevyStatus.Levying;

                if (levy.LevyAccumulator < profile.SecondsRequired)
                {
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                resources.Food = math.max(0f, resources.Food - profile.FoodCost);
                resources.Influence = math.max(0f, resources.Influence - profile.InfluenceCost);
                em.SetComponentData(factionEntity, resources);

                claim.Loyalty = math.max(0f, claim.Loyalty - profile.LoyaltyCost);
                em.SetComponentData(claimEntity, claim);

                if (!TrySpawnRetinueUnit(em, factionId, claimPosition, retinueCount, profile))
                {
                    levy.LevyStatus = MinorHouseLevyStatus.Stalled;
                    levy.LevyAccumulator = 0f;
                    em.SetComponentData(factionEntity, levy);
                    continue;
                }

                var population = em.GetComponentData<PopulationComponent>(factionEntity);
                UpdatePopulationAfterLevy(ref population, retinueCount + profile.PopulationCost);
                em.SetComponentData(factionEntity, population);

                levy.LeviesIssued += 1;
                levy.LevyStatus = MinorHouseLevyStatus.Raised;
                levy.LevyAccumulator = 0f;
                levy.LastLevyAtInWorldDays = currentDay;
                levy.LastLevyUnitId = profile.UnitId;
                levy.RetinueCount = retinueCount + 1;
                em.SetComponentData(factionEntity, levy);
            }
        }

        private static void EnsureSupportingComponents(EntityManager em, Entity factionEntity)
        {
            if (!em.HasComponent<ResourceStockpileComponent>(factionEntity))
            {
                em.AddComponentData(factionEntity, new ResourceStockpileComponent());
            }

            if (!em.HasComponent<PopulationComponent>(factionEntity))
            {
                em.AddComponentData(factionEntity, new PopulationComponent());
            }
        }

        private static bool TryResolveClaim(
            NativeArray<Entity> controlPointEntities,
            NativeArray<ControlPointComponent> controlPoints,
            NativeArray<PositionComponent> controlPointPositions,
            FixedString32Bytes factionId,
            ref MinorHouseLevyComponent levy,
            out Entity claimEntity,
            out ControlPointComponent claim,
            out float3 claimPosition,
            out int territoryCount)
        {
            claimEntity = Entity.Null;
            claim = default;
            claimPosition = float3.zero;
            territoryCount = 0;

            int explicitIndex = -1;
            int ownedIndex = -1;

            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].OwnerFactionId.Equals(factionId))
                {
                    territoryCount += 1;
                    if (ownedIndex < 0)
                    {
                        ownedIndex = i;
                    }
                }

                if (levy.ClaimControlPointId.Length > 0 &&
                    controlPoints[i].ControlPointId.Equals(levy.ClaimControlPointId))
                {
                    explicitIndex = i;
                }
            }

            int resolvedIndex = explicitIndex >= 0 ? explicitIndex : ownedIndex;
            if (resolvedIndex < 0)
            {
                return false;
            }

            levy.ClaimControlPointId = controlPoints[resolvedIndex].ControlPointId;
            claimEntity = controlPointEntities[resolvedIndex];
            claim = controlPoints[resolvedIndex];
            claimPosition = controlPointPositions[resolvedIndex].Value;
            return true;
        }

        private static int CountRetinueUnits(
            NativeArray<FactionComponent> unitFactions,
            NativeArray<UnitTypeComponent> unitTypes,
            NativeArray<HealthComponent> unitHealth,
            FixedString32Bytes factionId)
        {
            int count = 0;
            for (int i = 0; i < unitFactions.Length; i++)
            {
                if (!unitFactions[i].FactionId.Equals(factionId) ||
                    unitHealth[i].Current <= 0f ||
                    !IsRetinueRole(unitTypes[i].Role))
                {
                    continue;
                }

                count += 1;
            }

            return count;
        }

        private static bool IsRetinueRole(UnitRole role)
        {
            return role != UnitRole.Worker &&
                   role != UnitRole.EngineerSpecialist &&
                   role != UnitRole.Support &&
                   role != UnitRole.Unknown;
        }

        private static ParentPressureProfile BuildParentPressureProfile(
            NativeArray<FactionComponent> factionIds,
            NativeArray<WorldPressureComponent> pressures,
            FixedString32Bytes originFactionId)
        {
            var quiet = new ParentPressureProfile
            {
                Level = 0,
                Status = LesserHouseWorldPressureStatus.Quiet,
                LevyTempo = 1f,
                RetakeTempo = 1f,
                RetinueCapBonus = 0,
            };

            if (originFactionId.Length == 0)
            {
                return quiet;
            }

            for (int i = 0; i < factionIds.Length; i++)
            {
                if (!factionIds[i].FactionId.Equals(originFactionId))
                {
                    continue;
                }

                var pressure = pressures[i];
                if (!pressure.Targeted || pressure.Level <= 0)
                {
                    return quiet;
                }

                float scoreBonus = math.clamp(
                    (pressure.Score - 4) * PressureScoreBonusPerPoint,
                    0f,
                    MaxPressureScoreBonus);

                return new ParentPressureProfile
                {
                    Level = pressure.Level,
                    Status = ResolvePressureStatus(pressure.Level),
                    LevyTempo = math.round((1f + (pressure.Level * PressureTempoPerLevel) + scoreBonus) * 100f) / 100f,
                    RetakeTempo = math.round((1f + (pressure.Level * PressureRetakeTempoPerLevel) + scoreBonus) * 100f) / 100f,
                    RetinueCapBonus = pressure.Level >= 3 ? 2 : pressure.Level >= 2 ? 1 : 0,
                };
            }

            return quiet;
        }

        private static LesserHouseWorldPressureStatus ResolvePressureStatus(int level)
        {
            return level switch
            {
                >= 3 => LesserHouseWorldPressureStatus.Convergence,
                2 => LesserHouseWorldPressureStatus.Overwhelming,
                1 => LesserHouseWorldPressureStatus.Gathering,
                _ => LesserHouseWorldPressureStatus.Quiet,
            };
        }

        private static int ResolveRetinueCap(
            in ControlPointComponent claim,
            int territoryCount,
            int pressureRetinueBonus)
        {
            int cap = 1;
            if (claim.ControlState == ControlState.Stabilized && claim.Loyalty >= 55f)
            {
                cap += 1;
            }

            if (claim.ControlState == ControlState.Stabilized && claim.Loyalty >= 72f)
            {
                cap += 1;
            }

            cap += math.max(0, territoryCount - 1);
            cap += math.max(0, pressureRetinueBonus);
            return math.clamp(cap, 1, 6);
        }

        private static LevyProfile ResolveLevyProfile(float claimLoyalty, int currentRetinueCount)
        {
            if (claimLoyalty >= 80f && currentRetinueCount >= 2)
            {
                return BowmanProfile;
            }

            if (claimLoyalty >= 68f)
            {
                return SwordsmanProfile;
            }

            return MilitiaProfile;
        }

        private static void DecayProgress(
            ref MinorHouseLevyComponent levy,
            float dt,
            float multiplier)
        {
            levy.LevyAccumulator = math.max(
                0f,
                levy.LevyAccumulator - (dt * ProgressDecayPerSecond * multiplier));
        }

        private static bool TrySpawnRetinueUnit(
            EntityManager em,
            FixedString32Bytes factionId,
            float3 claimPosition,
            int existingRetinueCount,
            in LevyProfile profile)
        {
            if (profile.UnitId.Length == 0)
            {
                return false;
            }

            int offsetSeed = existingRetinueCount + 1;
            float angle = (offsetSeed % 8) * (math.PI / 4f);
            float offsetDistance = 30f + ((offsetSeed % 3) * 12f);
            float3 spawnPosition = claimPosition + new float3(
                math.cos(angle) * offsetDistance,
                0f,
                math.sin(angle) * offsetDistance);

            var unitEntity = em.CreateEntity();
            em.AddComponentData(unitEntity, new FactionComponent { FactionId = factionId });
            em.AddComponentData(unitEntity, new PositionComponent { Value = spawnPosition });
            em.AddComponentData(unitEntity, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            em.AddComponentData(unitEntity, new HealthComponent
            {
                Current = profile.MaxHealth,
                Max = profile.MaxHealth,
            });
            em.AddComponentData(unitEntity, new UnitTypeComponent
            {
                TypeId = profile.UnitId,
                Role = profile.Role,
                SiegeClass = SiegeClass.None,
                PopulationCost = profile.PopulationCost,
                Stage = 1,
            });
            em.AddComponentData(unitEntity, new MovementStatsComponent
            {
                MaxSpeed = profile.MaxSpeed,
            });
            em.AddComponentData(unitEntity, new CombatStatsComponent
            {
                AttackDamage = profile.AttackDamage,
                AttackRange = profile.AttackRange,
                AttackCooldown = profile.AttackCooldown,
                Sight = profile.Sight,
                CooldownRemaining = 0f,
            });
            em.AddComponentData(unitEntity, new UnitSeparationComponent
            {
                Radius = CombatUnitRuntimeDefaults.ResolveSeparationRadius(
                    profile.Role,
                    SiegeClass.None,
                    profile.SeparationRadius),
            });
            em.AddComponentData(unitEntity, new CombatStanceComponent
            {
                Stance = CombatUnitRuntimeDefaults.ResolveDefaultStance(profile.Role),
                LowHealthRetreatThreshold = CombatUnitRuntimeDefaults.DefaultLowHealthRetreatThreshold,
            });
            em.AddComponentData(unitEntity, new CombatStanceRuntimeComponent());
            em.AddComponentData(unitEntity, new RecentImpactComponent());
            em.AddComponentData(unitEntity, new MoveCommandComponent
            {
                Destination = spawnPosition,
                StoppingDistance = 0.2f,
                IsActive = false,
            });

            if (profile.ProjectileSpeed > 0f)
            {
                em.AddComponentData(unitEntity, new ProjectileFactoryComponent
                {
                    ProjectileSpeed = profile.ProjectileSpeed,
                    ProjectileMaxLifetimeSeconds = ProjectileMaxLifetimeSeconds,
                    ProjectileArrivalRadius = ProjectileArrivalRadius,
                });
            }

            return true;
        }

        private static void UpdatePopulationAfterLevy(
            ref PopulationComponent population,
            int usedPopulation)
        {
            population.Total = math.max(population.Total, usedPopulation);
            population.BaseCap = math.max(population.BaseCap, population.Total);
            population.Cap = math.max(math.max(population.Cap, population.BaseCap), population.Total);
            population.Available = math.max(0, population.Cap - usedPopulation);
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0f;
            }

            float inWorldDays = query.GetSingleton<DualClockComponent>().InWorldDays;
            query.Dispose();
            return inWorldDays;
        }

        private struct LevyProfile
        {
            public FixedString64Bytes UnitId;
            public UnitRole Role;
            public float FoodCost;
            public float InfluenceCost;
            public float LoyaltyCost;
            public float SecondsRequired;
            public int PopulationCost;
            public float MaxHealth;
            public float MaxSpeed;
            public float AttackDamage;
            public float AttackRange;
            public float AttackCooldown;
            public float Sight;
            public float ProjectileSpeed;
            public float SeparationRadius;
        }

        private struct ParentPressureProfile
        {
            public int Level;
            public LesserHouseWorldPressureStatus Status;
            public float LevyTempo;
            public float RetakeTempo;
            public int RetinueCapBonus;
        }
    }
}
