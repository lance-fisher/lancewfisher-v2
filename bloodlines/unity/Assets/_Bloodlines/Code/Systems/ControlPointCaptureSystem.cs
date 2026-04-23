using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.TerritoryGovernance;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// First ECS control-point capture and stabilization pass.
    /// Browser runtime equivalent: the capture/stabilization branches in
    /// `updateControlPoints` inside the reference simulation.
    ///
    /// This slice intentionally keeps parity narrow but structurally aligned:
    ///   - non-worker living units can contest and capture
    ///   - capture progress decays when no claimant holds the point
    ///   - owned points stabilize over time
    ///   - stabilized points fall back to occupied when loyalty is driven down
    ///
    /// Doctrine, commander, governor, and political-event modifiers remain future follow-up.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ControlPointResourceTrickleSystem))]
    public partial struct ControlPointCaptureSystem : ISystem
    {
        const float CaptureDecayPerSecond = 2.5f;
        const float OpposingCaptureResetPerSecond = 6f;
        const float ActiveCapturePerSecond = 1f;
        const float PassiveStabilizationPerSecond = 0.4f;
        const float ActiveStabilizationPerSecond = 1.6f;
        const float LoyaltyDrainPerSecond = 4.5f;
        const float StabilizedLoyaltyThreshold = 72f;
        const float CaptureResetFloor = 34f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ControlPointComponent>();
            state.RequireForUpdate<UnitTypeComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var entityManager = state.EntityManager;
            var unitQuery = SystemAPI.QueryBuilder()
                .WithAll<UnitTypeComponent, PositionComponent, FactionComponent, HealthComponent>()
                .Build();

            using var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var unitHealth = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            foreach (var (controlPointRw, position, controlPointEntity) in
                SystemAPI.Query<RefRW<ControlPointComponent>, RefRO<PositionComponent>>()
                    .WithEntityAccess())
            {
                ref var controlPoint = ref controlPointRw.ValueRW;

                FixedString32Bytes claimantFactionId = default;
                bool multipleClaimants = false;
                bool claimantPresent = false;
                float captureRadiusSquared = math.max(1f, controlPoint.RadiusTiles * controlPoint.RadiusTiles);

                for (int i = 0; i < unitPositions.Length; i++)
                {
                    if (unitHealth[i].Current <= 0f || unitTypes[i].Role == UnitRole.Worker)
                    {
                        continue;
                    }

                    float distanceSquared = math.distancesq(unitPositions[i].Value, position.ValueRO.Value);
                    if (distanceSquared > captureRadiusSquared)
                    {
                        continue;
                    }

                    var factionId = unitFactions[i].FactionId;
                    if (!claimantPresent)
                    {
                        claimantFactionId = factionId;
                        claimantPresent = true;
                    }
                    else if (!claimantFactionId.Equals(factionId))
                    {
                        multipleClaimants = true;
                        break;
                    }
                }

                controlPoint.IsContested = multipleClaimants;

                if (controlPoint.OwnerFactionId.Length > 0 && !controlPoint.IsContested)
                {
                    float passiveStabilization = PassiveStabilizationPerSecond *
                        ResolveStabilizationMultiplier(entityManager, controlPoint.OwnerFactionId) *
                        ResolveGovernorStabilizationMultiplier(entityManager, controlPointRw.ValueRO, controlPointEntity);
                    controlPoint.Loyalty = math.min(100f, controlPoint.Loyalty + dt * passiveStabilization);
                    if (controlPoint.ControlState == ControlState.Occupied &&
                        controlPoint.Loyalty >= StabilizedLoyaltyThreshold)
                    {
                        controlPoint.ControlState = ControlState.Stabilized;
                    }
                }

                if (controlPoint.IsContested || !claimantPresent)
                {
                    controlPoint.CaptureProgress = math.max(0f, controlPoint.CaptureProgress - dt * CaptureDecayPerSecond);
                    if (!claimantPresent)
                    {
                        controlPoint.CaptureFactionId = default;
                    }

                    continue;
                }

                if (controlPoint.OwnerFactionId.Equals(claimantFactionId))
                {
                    controlPoint.CaptureFactionId = default;
                    controlPoint.CaptureProgress = 0f;
                    float activeStabilization = ActiveStabilizationPerSecond *
                        ResolveStabilizationMultiplier(entityManager, claimantFactionId) *
                        ResolveGovernorStabilizationMultiplier(entityManager, controlPointRw.ValueRO, controlPointEntity);
                    controlPoint.Loyalty = math.min(100f, controlPoint.Loyalty + dt * activeStabilization);
                    if (controlPoint.ControlState == ControlState.Occupied &&
                        controlPoint.Loyalty >= StabilizedLoyaltyThreshold)
                    {
                        controlPoint.ControlState = ControlState.Stabilized;
                    }

                    continue;
                }

                if (controlPoint.CaptureFactionId.Length > 0 && !controlPoint.CaptureFactionId.Equals(claimantFactionId))
                {
                    controlPoint.CaptureProgress = math.max(
                        0f,
                        controlPoint.CaptureProgress -
                        dt * OpposingCaptureResetPerSecond / ResolveCaptureResistanceMultiplier(entityManager, controlPointEntity));
                    if (controlPoint.CaptureProgress > 0f)
                    {
                        if (controlPoint.ControlState == ControlState.Stabilized &&
                            controlPoint.Loyalty < StabilizedLoyaltyThreshold)
                        {
                            controlPoint.ControlState = ControlState.Occupied;
                        }

                        continue;
                    }
                }

                controlPoint.CaptureFactionId = claimantFactionId;
                controlPoint.CaptureProgress += dt * ActiveCapturePerSecond /
                    ResolveCaptureResistanceMultiplier(entityManager, controlPointEntity);
                float loyaltyDelta = ResolveGovernorLoyaltyDelta(
                    entityManager,
                    controlPointEntity,
                    -dt * LoyaltyDrainPerSecond);
                controlPoint.Loyalty = math.max(0f, controlPoint.Loyalty + loyaltyDelta);

                if (controlPoint.ControlState == ControlState.Stabilized &&
                    controlPoint.Loyalty < StabilizedLoyaltyThreshold)
                {
                    controlPoint.ControlState = ControlState.Occupied;
                }

                float captureTime = math.max(1f, controlPoint.CaptureTimeSeconds);
                if (controlPoint.CaptureProgress < captureTime)
                {
                    continue;
                }

                controlPoint.OwnerFactionId = claimantFactionId;
                controlPoint.CaptureFactionId = default;
                controlPoint.CaptureProgress = 0f;
                controlPoint.Loyalty = CaptureResetFloor;
                controlPoint.ControlState = ControlState.Occupied;
                controlPoint.IsContested = false;
            }
        }

        static float ResolveStabilizationMultiplier(EntityManager entityManager, FixedString32Bytes factionId)
        {
            return DynastyPoliticalEventUtility.TryGetAggregateByFactionId(
                entityManager,
                factionId,
                out var aggregate)
                ? math.max(0.1f, aggregate.StabilizationMultiplier)
                : 1f;
        }

        static float ResolveGovernorStabilizationMultiplier(
            EntityManager entityManager,
            in ControlPointComponent controlPoint,
            Entity controlPointEntity)
        {
            if (controlPoint.OwnerFactionId.Length == 0 ||
                !entityManager.HasComponent<GovernorSpecializationComponent>(controlPointEntity))
            {
                return 1f;
            }

            GovernorSpecializationComponent specialization =
                entityManager.GetComponentData<GovernorSpecializationComponent>(controlPointEntity);
            return GovernorSpecializationCanon.GovernorStabilizationBaseBonus *
                math.max(1f, specialization.StabilizationMultiplier);
        }

        static float ResolveCaptureResistanceMultiplier(EntityManager entityManager, Entity controlPointEntity)
        {
            if (!entityManager.HasComponent<GovernorSpecializationComponent>(controlPointEntity))
            {
                return 1f;
            }

            GovernorSpecializationComponent specialization =
                entityManager.GetComponentData<GovernorSpecializationComponent>(controlPointEntity);
            return math.max(1f, specialization.CaptureResistanceBonus);
        }

        static float ResolveGovernorLoyaltyDelta(
            EntityManager entityManager,
            Entity controlPointEntity,
            float loyaltyDelta)
        {
            if (!entityManager.HasComponent<GovernorSpecializationComponent>(controlPointEntity))
            {
                return loyaltyDelta;
            }

            GovernorSpecializationComponent specialization =
                entityManager.GetComponentData<GovernorSpecializationComponent>(controlPointEntity);
            return GovernorSpecializationCanon.ApplyLoyaltyProtection(
                loyaltyDelta,
                specialization.LoyaltyProtectionMultiplier);
        }
    }
}
