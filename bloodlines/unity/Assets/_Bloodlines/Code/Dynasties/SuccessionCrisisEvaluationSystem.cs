using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Detects ruler changes after DynastySuccessionSystem and materializes the
    /// first succession-crisis slice using the browser's maturity and rival-claim
    /// heuristics.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DynastySuccessionSystem))]
    public partial struct SuccessionCrisisEvaluationSystem : ISystem
    {
        private const int AdultAge = 18;
        private const int MatureAge = 21;
        private const int ClaimGapThreshold = 4;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyStateComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;
            float inWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>(),
                ComponentType.ReadOnly<DynastyMemberRef>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity factionEntity = factionEntities[i];
                if (!entityManager.HasComponent<SuccessionCrisisWatchComponent>(factionEntity))
                {
                    ecb.AddComponent(factionEntity, new SuccessionCrisisWatchComponent
                    {
                        LastKnownRulerMemberId = FindCurrentRulerMemberId(entityManager, factionEntity),
                        Initialized = 1,
                    });
                    continue;
                }

                var watcher = entityManager.GetComponentData<SuccessionCrisisWatchComponent>(factionEntity);
                FixedString64Bytes currentRulerId = FindCurrentRulerMemberId(entityManager, factionEntity);
                if (watcher.Initialized == 0)
                {
                    watcher.LastKnownRulerMemberId = currentRulerId;
                    watcher.Initialized = 1;
                    entityManager.SetComponentData(factionEntity, watcher);
                    continue;
                }

                bool rulerChanged = !watcher.LastKnownRulerMemberId.Equals(currentRulerId);
                bool hadPreviousRuler = watcher.LastKnownRulerMemberId.Length > 0;
                if (!rulerChanged || !hadPreviousRuler)
                {
                    watcher.LastKnownRulerMemberId = currentRulerId;
                    entityManager.SetComponentData(factionEntity, watcher);
                    continue;
                }

                if (entityManager.HasComponent<SuccessionCrisisComponent>(factionEntity))
                {
                    watcher.LastKnownRulerMemberId = currentRulerId;
                    entityManager.SetComponentData(factionEntity, watcher);
                    continue;
                }

                var dynastyState = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
                SuccessionCrisisSeverity severity = EvaluateSeverity(
                    entityManager,
                    factionEntity,
                    watcher.LastKnownRulerMemberId,
                    dynastyState,
                    out float legitimacyDrainPerDay,
                    out float loyaltyDrainPerDay,
                    out float resourceTrickleFactor,
                    out float recoveryRatePerDay);

                if (severity != SuccessionCrisisSeverity.None)
                {
                    ApplyOpeningLoyaltyShock(entityManager, factionEntity, GetProfile(severity).LoyaltyShock);
                    ecb.AddComponent(factionEntity, new SuccessionCrisisComponent
                    {
                        CrisisSeverity = (byte)severity,
                        CrisisStartedAtInWorldDays = inWorldDays,
                        RecoveryProgressPct = 0f,
                        ResourceTrickleFactor = resourceTrickleFactor,
                        LoyaltyShockApplied = true,
                        LegitimacyDrainRatePerDay = legitimacyDrainPerDay,
                        LoyaltyDrainRatePerDay = loyaltyDrainPerDay,
                        LastDailyTickInWorldDays = math.floor(inWorldDays),
                        RecoveryRatePerDay = recoveryRatePerDay,
                        FallenMemberId = watcher.LastKnownRulerMemberId,
                        CurrentRulerMemberId = currentRulerId,
                    });
                }

                watcher.LastKnownRulerMemberId = currentRulerId;
                entityManager.SetComponentData(factionEntity, watcher);
            }

            ecb.Playback(entityManager);
        }

        private static SuccessionCrisisSeverity EvaluateSeverity(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes fallenMemberId,
            DynastyStateComponent dynastyState,
            out float legitimacyDrainPerDay,
            out float loyaltyDrainPerDay,
            out float resourceTrickleFactor,
            out float recoveryRatePerDay)
        {
            FixedString64Bytes currentRulerId = default;
            DynastyMemberComponent currentRuler = default;
            bool hasCurrentRuler = TryGetCurrentRuler(entityManager, factionEntity, out currentRulerId, out currentRuler);
            if (!hasCurrentRuler || dynastyState.Interregnum)
            {
                var profile = GetProfile(SuccessionCrisisSeverity.Catastrophic);
                legitimacyDrainPerDay = profile.LegitimacyDrainPerDay;
                loyaltyDrainPerDay = profile.LoyaltyDrainPerDay;
                resourceTrickleFactor = profile.ResourceTrickleMultiplier;
                recoveryRatePerDay = profile.RecoveryRatePerDay;
                return SuccessionCrisisSeverity.Catastrophic;
            }

            int rulerClaimStrength = GetClaimStrength(currentRuler, ruling: true);
            int rivalClaimants = 0;

            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.MemberId.Equals(fallenMemberId) ||
                    member.MemberId.Equals(currentRulerId) ||
                    !IsAvailable(member) ||
                    member.AgeYears < AdultAge)
                {
                    continue;
                }

                if (GetClaimStrength(member, ruling: false) >= (rulerClaimStrength - ClaimGapThreshold))
                {
                    rivalClaimants++;
                }
            }

            SuccessionCrisisSeverity severity = SuccessionCrisisSeverity.None;
            if (currentRuler.AgeYears < AdultAge)
            {
                severity = MoreSevere(severity, SuccessionCrisisSeverity.Major);
            }
            else if (currentRuler.AgeYears < MatureAge)
            {
                severity = MoreSevere(severity, SuccessionCrisisSeverity.Moderate);
            }

            if (rivalClaimants >= 2)
            {
                severity = MoreSevere(severity, SuccessionCrisisSeverity.Major);
            }
            else if (rivalClaimants == 1)
            {
                severity = MoreSevere(severity, SuccessionCrisisSeverity.Minor);
            }

            var crisisProfile = GetProfile(severity);
            legitimacyDrainPerDay = crisisProfile.LegitimacyDrainPerDay;
            loyaltyDrainPerDay = crisisProfile.LoyaltyDrainPerDay;
            resourceTrickleFactor = crisisProfile.ResourceTrickleMultiplier;
            recoveryRatePerDay = crisisProfile.RecoveryRatePerDay;
            return severity;
        }

        private static bool TryGetCurrentRuler(
            EntityManager entityManager,
            Entity factionEntity,
            out FixedString64Bytes currentRulerId,
            out DynastyMemberComponent currentRuler)
        {
            currentRulerId = default;
            currentRuler = default;
            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Status == DynastyMemberStatus.Ruling)
                {
                    currentRulerId = member.MemberId;
                    currentRuler = member;
                    return true;
                }
            }

            return false;
        }

        private static FixedString64Bytes FindCurrentRulerMemberId(EntityManager entityManager, Entity factionEntity)
        {
            return TryGetCurrentRuler(entityManager, factionEntity, out FixedString64Bytes memberId, out _)
                ? memberId
                : default;
        }

        private static bool IsAvailable(DynastyMemberComponent member)
        {
            return member.Status == DynastyMemberStatus.Active ||
                   member.Status == DynastyMemberStatus.Ruling ||
                   member.Status == DynastyMemberStatus.Captured;
        }

        private static int GetClaimStrength(DynastyMemberComponent member, bool ruling)
        {
            if (!IsAvailable(member))
            {
                return int.MinValue;
            }

            int roleWeight = member.Role switch
            {
                DynastyRole.HeadOfBloodline => 12,
                DynastyRole.HeirDesignate => 10,
                DynastyRole.Commander => 8,
                DynastyRole.Governor => 7,
                DynastyRole.Diplomat => 6,
                DynastyRole.IdeologicalLeader => 5,
                DynastyRole.Merchant => 4,
                DynastyRole.Spymaster => 4,
                _ => 3,
            };

            int maturityScore = member.AgeYears >= MatureAge
                ? 5
                : member.AgeYears >= AdultAge
                    ? 3
                    : 0;
            return roleWeight + maturityScore + (int)math.round(member.Renown / 3f) + (ruling ? 3 : 0);
        }

        private static SuccessionCrisisSeverity MoreSevere(SuccessionCrisisSeverity left, SuccessionCrisisSeverity right)
        {
            return (SuccessionCrisisSeverity)math.max((int)left, (int)right);
        }

        private static void ApplyOpeningLoyaltyShock(EntityManager entityManager, Entity factionEntity, float loyaltyShock)
        {
            if (loyaltyShock <= 0f || !entityManager.HasComponent<FactionComponent>(factionEntity))
            {
                return;
            }

            FixedString32Bytes factionId = entityManager.GetComponentData<FactionComponent>(factionEntity).FactionId;
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
                controlPoint.Loyalty = math.clamp(controlPoint.Loyalty - loyaltyShock, 0f, 100f);
                entityManager.SetComponentData(controlPointEntities[i], controlPoint);
            }
        }

        private static SuccessionCrisisProfile GetProfile(SuccessionCrisisSeverity severity)
        {
            return severity switch
            {
                SuccessionCrisisSeverity.Minor => new SuccessionCrisisProfile(3f, 0.12f, 0.08f, 0.96f, 1f / 45f),
                SuccessionCrisisSeverity.Moderate => new SuccessionCrisisProfile(5f, 0.20f, 0.14f, 0.90f, 1f / 60f),
                SuccessionCrisisSeverity.Major => new SuccessionCrisisProfile(7f, 0.34f, 0.24f, 0.84f, 1f / 90f),
                SuccessionCrisisSeverity.Catastrophic => new SuccessionCrisisProfile(10f, 0.50f, 0.34f, 0.74f, 1f / 120f),
                _ => new SuccessionCrisisProfile(0f, 0f, 0f, 1f, 0f),
            };
        }

        private readonly struct SuccessionCrisisProfile
        {
            public SuccessionCrisisProfile(
                float loyaltyShock,
                float loyaltyDrainPerDay,
                float legitimacyDrainPerDay,
                float resourceTrickleMultiplier,
                float recoveryRatePerDay)
            {
                LoyaltyShock = loyaltyShock;
                LoyaltyDrainPerDay = loyaltyDrainPerDay;
                LegitimacyDrainPerDay = legitimacyDrainPerDay;
                ResourceTrickleMultiplier = resourceTrickleMultiplier;
                RecoveryRatePerDay = recoveryRatePerDay;
            }

            public float LoyaltyShock { get; }
            public float LoyaltyDrainPerDay { get; }
            public float LegitimacyDrainPerDay { get; }
            public float ResourceTrickleMultiplier { get; }
            public float RecoveryRatePerDay { get; }
        }
    }
}
