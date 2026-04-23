using Bloodlines.Components;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// First fortification reserve loop for Unity. It mirrors the browser
    /// reserve-contract behavior at a narrow level: low-health frontline units
    /// retreat into triage, recovered defenders return to the ready pool, and
    /// ready reserves cycle forward when the active frontline drops below the
    /// tier-scaled target.
    /// Browser reference: simulation.js tickFortificationReserves (11875).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AdvanceFortificationTierSystem))]
    [UpdateAfter(typeof(FortificationDestructionResolutionSystem))]
    public partial struct FortificationReserveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FortificationComponent>();
            state.RequireForUpdate<FortificationReserveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            double elapsed = SystemAPI.Time.ElapsedTime;

            var linkedCombatantQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationCombatantTag>(),
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FortificationReserveAssignmentComponent>());

            var hostileQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationCombatantTag>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FactionComponent>());

            using var linkedEntities = linkedCombatantQuery.ToEntityArray(Allocator.Temp);
            using var linkedLinks = linkedCombatantQuery.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            using var linkedPositions = linkedCombatantQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var linkedHealths = linkedCombatantQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var linkedFactions = linkedCombatantQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            using var hostilePositions = hostileQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var hostileHealths = hostileQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var hostileFactions = hostileQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            foreach (var (fortificationRw, reserveRw, faction, settlementPosition, settlementEntity) in
                SystemAPI.Query<
                    RefRO<FortificationComponent>,
                    RefRW<FortificationReserveComponent>,
                    RefRO<FactionComponent>,
                    RefRO<PositionComponent>>()
                .WithEntityAccess())
            {
                var fortification = fortificationRw.ValueRO;
                var reserve = reserveRw.ValueRO;
                GovernorSpecializationComponent settlementGovernorProfile =
                    GovernorSpecializationCanon.GetSettlementProfile(entityManager, settlementEntity);
                float3 settlementCenter = settlementPosition.ValueRO.Value;

                int hostileCount = CountHostiles(
                    hostilePositions,
                    hostileHealths,
                    hostileFactions,
                    faction.ValueRO.FactionId,
                    settlementCenter,
                    fortification.ThreatRadiusTiles,
                    out float3 hostileCenter);

                reserve.ThreatActive = hostileCount > 0;
                reserve.LastCommittedCount = 0;

                for (int i = 0; i < linkedEntities.Length; i++)
                {
                    if (linkedLinks[i].SettlementEntity != settlementEntity ||
                        !linkedFactions[i].FactionId.Equals(faction.ValueRO.FactionId))
                    {
                        continue;
                    }

                    var health = entityManager.GetComponentData<HealthComponent>(linkedEntities[i]);
                    if (health.Current <= 0f || health.Max <= 0f)
                    {
                        continue;
                    }

                    var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(linkedEntities[i]);
                    float3 unitPosition = linkedPositions[i].Value;
                    float distanceToSettlement = math.distance(unitPosition, settlementCenter);
                    float healthRatio = health.Current / health.Max;
                    bool insideTriage = distanceToSettlement <= reserve.TriageRadiusTiles;

                    if (!reserve.ThreatActive && assignment.Duty == ReserveDutyState.Muster)
                    {
                        assignment.Duty = ReserveDutyState.Ready;
                    }

                    if (insideTriage &&
                        (assignment.Duty == ReserveDutyState.Fallback ||
                         assignment.Duty == ReserveDutyState.Recovering ||
                         !reserve.ThreatActive))
                    {
                        if (assignment.Duty == ReserveDutyState.Fallback)
                        {
                            assignment.Duty = ReserveDutyState.Recovering;
                        }

                        health.Current = math.min(
                            health.Max,
                            health.Current + reserve.ReserveHealPerSecond * settlementGovernorProfile.HealRegenMultiplier * dt);
                        healthRatio = health.Current / health.Max;
                        entityManager.SetComponentData(linkedEntities[i], health);

                        if (healthRatio >= reserve.RecoveryHealthRatio)
                        {
                            assignment.Duty = ReserveDutyState.Ready;
                        }
                    }

                    if (reserve.ThreatActive)
                    {
                        float distanceToHostiles = math.distance(unitPosition, hostileCenter);
                        if (healthRatio <= reserve.RetreatHealthRatio &&
                            distanceToHostiles <= fortification.ThreatRadiusTiles * 0.75f &&
                            assignment.Duty != ReserveDutyState.Fallback &&
                            assignment.Duty != ReserveDutyState.Recovering)
                        {
                            assignment.Duty = ReserveDutyState.Fallback;
                        }
                    }

                    entityManager.SetComponentData(linkedEntities[i], assignment);
                }

                int activeDefenderCount = 0;
                int engagedCount = 0;
                int readyCount = 0;
                int musteringCount = 0;
                int recoveringCount = 0;
                int fallbackCount = 0;

                for (int i = 0; i < linkedEntities.Length; i++)
                {
                    if (linkedLinks[i].SettlementEntity != settlementEntity ||
                        !linkedFactions[i].FactionId.Equals(faction.ValueRO.FactionId))
                    {
                        continue;
                    }

                    var health = entityManager.GetComponentData<HealthComponent>(linkedEntities[i]);
                    if (health.Current <= 0f)
                    {
                        continue;
                    }

                    if (math.distance(linkedPositions[i].Value, settlementCenter) > fortification.ReserveRadiusTiles)
                    {
                        continue;
                    }

                    activeDefenderCount++;
                    var duty = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(linkedEntities[i]).Duty;
                    switch (duty)
                    {
                        case ReserveDutyState.Engaged:
                            engagedCount++;
                            break;
                        case ReserveDutyState.Muster:
                            engagedCount++;
                            musteringCount++;
                            break;
                        case ReserveDutyState.Recovering:
                            recoveringCount++;
                            break;
                        case ReserveDutyState.Fallback:
                            fallbackCount++;
                            break;
                        default:
                            readyCount++;
                            break;
                    }
                }

                if (reserve.ThreatActive)
                {
                    int desiredFrontline = math.min(activeDefenderCount, math.max(1, 1 + fortification.Tier));
                    int neededReserves = math.max(0, desiredFrontline - engagedCount);
                    float musterIntervalSeconds = reserve.MusterIntervalSeconds /
                        math.max(0.1f, settlementGovernorProfile.ReserveRegenMultiplier);
                    if (neededReserves > 0 &&
                        (elapsed - reserve.LastCommitAt) >= musterIntervalSeconds)
                    {
                        int committed = 0;
                        for (int i = 0; i < linkedEntities.Length && committed < neededReserves; i++)
                        {
                            if (linkedLinks[i].SettlementEntity != settlementEntity ||
                                !linkedFactions[i].FactionId.Equals(faction.ValueRO.FactionId))
                            {
                                continue;
                            }

                            var health = entityManager.GetComponentData<HealthComponent>(linkedEntities[i]);
                            if (health.Current <= 0f || health.Max <= 0f)
                            {
                                continue;
                            }

                            var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(linkedEntities[i]);
                            if (assignment.Duty != ReserveDutyState.Ready)
                            {
                                continue;
                            }

                            float3 unitPosition = linkedPositions[i].Value;
                            float healthRatio = health.Current / health.Max;
                            if (healthRatio < reserve.RecoveryHealthRatio ||
                                math.distance(unitPosition, settlementCenter) > fortification.ReserveRadiusTiles * 0.55f ||
                                math.distance(unitPosition, hostileCenter) <= fortification.ThreatRadiusTiles * 0.55f)
                            {
                                continue;
                            }

                            assignment.Duty = ReserveDutyState.Muster;
                            entityManager.SetComponentData(linkedEntities[i], assignment);
                            committed++;
                        }

                        if (committed > 0)
                        {
                            reserve.LastCommitAt = elapsed;
                            reserve.LastCommittedCount = committed;
                            readyCount = math.max(0, readyCount - committed);
                            musteringCount += committed;
                        }
                    }
                }

                reserve.ReadyReserveCount = readyCount;
                reserve.MusteringReserveCount = musteringCount;
                reserve.RecoveringReserveCount = recoveringCount;
                reserve.FallbackReserveCount = fallbackCount;
                reserveRw.ValueRW = reserve;
            }
        }

        private static int CountHostiles(
            NativeArray<PositionComponent> hostilePositions,
            NativeArray<HealthComponent> hostileHealths,
            NativeArray<FactionComponent> hostileFactions,
            FixedString32Bytes owningFactionId,
            float3 settlementCenter,
            float threatRadiusTiles,
            out float3 hostileCenter)
        {
            int hostileCount = 0;
            float3 total = float3.zero;

            for (int i = 0; i < hostilePositions.Length; i++)
            {
                if (hostileHealths[i].Current <= 0f ||
                    hostileFactions[i].FactionId.Equals(owningFactionId) ||
                    math.distance(hostilePositions[i].Value, settlementCenter) > threatRadiusTiles)
                {
                    continue;
                }

                hostileCount++;
                total += hostilePositions[i].Value;
            }

            hostileCenter = hostileCount > 0 ? total / hostileCount : settlementCenter;
            return hostileCount;
        }
    }
}
