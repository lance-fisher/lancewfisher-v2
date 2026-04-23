using Bloodlines.Components;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Browser reference: simulation.js getVerdantWardenZoneSupportProfile,
    /// getVerdantWardenControlPointSupportProfile, and the settlement-defense-state
    /// Verdant Warden block. Resolves live support coverage onto control points and
    /// fortified settlements each simulation tick.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ControlPointCaptureSystem))]
    [UpdateBefore(typeof(FortificationReserveSystem))]
    [UpdateBefore(typeof(AttackResolutionSystem))]
    public partial struct VerdantWardenSupportSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<UnitTypeComponent>();
            state.RequireForUpdate<ControlPointComponent>();
            state.RequireForUpdate<FortificationComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            EnsureVerdantWardenTags(entityManager);

            float dt = SystemAPI.Time.DeltaTime;
            float tileSize = SystemAPI.TryGetSingleton<MapBootstrapConfigComponent>(out var mapConfig)
                ? math.max(1f, mapConfig.TileSize)
                : 32f;

            var wardenQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<VerdantWardenComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using var wardenFactions = wardenQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var wardenPositions = wardenQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var wardenHealths = wardenQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var wardenProfiles = wardenQuery.ToComponentDataArray<VerdantWardenComponent>(Allocator.Temp);

            var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadWrite<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var controlPointEntities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var controlPointPositions = controlPointQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            for (int i = 0; i < controlPointEntities.Length; i++)
            {
                ControlPointComponent controlPoint = controlPoints[i];
                VerdantWardenCoverageProfile support = ResolveControlPointCoverage(
                    in controlPoint,
                    controlPointPositions[i].Value,
                    tileSize,
                    wardenFactions,
                    wardenPositions,
                    wardenHealths,
                    wardenProfiles);

                VerdantWardenRules.ApplyTo(ref controlPoint, support);

                if (controlPoint.OwnerFactionId.Length > 0 &&
                    !controlPoint.IsContested &&
                    (controlPoint.ControlState == ControlState.Occupied ||
                     controlPoint.ControlState == ControlState.Stabilized) &&
                    support.LoyaltyBonusPerTick > 0f)
                {
                    controlPoint.Loyalty = math.clamp(
                        controlPoint.Loyalty + dt * support.LoyaltyBonusPerTick,
                        0f,
                        100f);
                }

                entityManager.SetComponentData(controlPointEntities[i], controlPoint);
            }

            var fortificationQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadWrite<FortificationComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var fortificationEntities = fortificationQuery.ToEntityArray(Allocator.Temp);
            using var fortifications = fortificationQuery.ToComponentDataArray<FortificationComponent>(Allocator.Temp);
            using var fortificationFactions = fortificationQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var fortificationPositions = fortificationQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            for (int i = 0; i < fortificationEntities.Length; i++)
            {
                FortificationComponent fortification = fortifications[i];
                VerdantWardenCoverageProfile support = ResolveFortificationCoverage(
                    fortificationFactions[i].FactionId,
                    fortificationPositions[i].Value,
                    wardenFactions,
                    wardenPositions,
                    wardenHealths,
                    wardenProfiles);
                VerdantWardenRules.ApplyTo(ref fortification, support);
                entityManager.SetComponentData(fortificationEntities[i], fortification);
            }
        }

        private static void EnsureVerdantWardenTags(EntityManager entityManager)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            try
            {
                var query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<UnitTypeComponent>(),
                    ComponentType.ReadOnly<HealthComponent>());
                using var entities = query.ToEntityArray(Allocator.Temp);
                using var unitTypes = query.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);

                for (int i = 0; i < entities.Length; i++)
                {
                    if (!entityManager.HasComponent<VerdantWardenComponent>(entities[i]) &&
                        VerdantWardenRules.IsVerdantWarden(unitTypes[i]))
                    {
                        ecb.AddComponent(entities[i], VerdantWardenRules.CreateDefaultComponent());
                    }
                }

                ecb.Playback(entityManager);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        private static VerdantWardenCoverageProfile ResolveControlPointCoverage(
            in ControlPointComponent controlPoint,
            float3 position,
            float tileSize,
            NativeArray<FactionComponent> wardenFactions,
            NativeArray<PositionComponent> wardenPositions,
            NativeArray<HealthComponent> wardenHealths,
            NativeArray<VerdantWardenComponent> wardenProfiles)
        {
            if (controlPoint.OwnerFactionId.Length == 0)
            {
                return VerdantWardenRules.DefaultCoverage;
            }

            int count = 0;
            float supportRadius = VerdantWardenRules.DefaultSupportRadius;
            for (int i = 0; i < wardenProfiles.Length; i++)
            {
                if (wardenHealths[i].Current <= 0f ||
                    !wardenFactions[i].FactionId.Equals(controlPoint.OwnerFactionId))
                {
                    continue;
                }

                supportRadius = wardenProfiles[i].SupportRadius > 0f
                    ? wardenProfiles[i].SupportRadius
                    : VerdantWardenRules.DefaultSupportRadius;
                float radius = VerdantWardenRules.ResolveControlPointSupportRadius(
                    controlPoint,
                    tileSize,
                    supportRadius);
                float distance = math.distance(wardenPositions[i].Value.xz, position.xz);
                if (distance <= radius)
                {
                    count++;
                }
            }

            VerdantWardenComponent profile = wardenProfiles.Length > 0
                ? wardenProfiles[0]
                : VerdantWardenRules.CreateDefaultComponent();
            return VerdantWardenRules.ResolveProfile(profile, count);
        }

        private static VerdantWardenCoverageProfile ResolveFortificationCoverage(
            FixedString32Bytes factionId,
            float3 position,
            NativeArray<FactionComponent> wardenFactions,
            NativeArray<PositionComponent> wardenPositions,
            NativeArray<HealthComponent> wardenHealths,
            NativeArray<VerdantWardenComponent> wardenProfiles)
        {
            if (factionId.Length == 0)
            {
                return VerdantWardenRules.DefaultCoverage;
            }

            int count = 0;
            VerdantWardenComponent profile = VerdantWardenRules.CreateDefaultComponent();
            for (int i = 0; i < wardenProfiles.Length; i++)
            {
                if (wardenHealths[i].Current <= 0f ||
                    !wardenFactions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                profile = wardenProfiles[i];
                float supportRadius = profile.SupportRadius > 0f
                    ? profile.SupportRadius
                    : VerdantWardenRules.DefaultSupportRadius;
                float distance = math.distance(wardenPositions[i].Value.xz, position.xz);
                if (distance <= supportRadius)
                {
                    count++;
                }
            }

            return VerdantWardenRules.ResolveProfile(profile, count);
        }
    }
}
