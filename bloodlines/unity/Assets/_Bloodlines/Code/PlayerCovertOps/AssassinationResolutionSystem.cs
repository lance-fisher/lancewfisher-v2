using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Resolves ready assassination operations before dynasty follow-through systems
    /// so fallen rulers, commanders, and governors push the same-frame succession and
    /// fallout ripples expected by the canonical covert-ops spec.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCounterIntelligenceSystem))]
    [UpdateBefore(typeof(DynastySuccessionSystem))]
    [UpdateBefore(typeof(MarriageDeathDissolutionSystem))]
    [UpdateBefore(typeof(DeathResolutionSystem))]
    public partial struct AssassinationResolutionSystem : ISystem
    {
        private const float AttackerRuthlessnessGain = 2f;
        private const float HeadLegitimacyPenalty = 18f;
        private const float CommanderLegitimacyPenalty = 9f;
        private const float GovernorLegitimacyPenalty = 5f;
        private const float HeirLegitimacyPenalty = 8f;
        private const float CourtOfficerLegitimacyPenalty = 4f;
        private const float MerchantLegitimacyPenalty = 3f;
        private const float InterregnumLegitimacyPenalty = 14f;
        private const float SuccessionRecoveryLegitimacy = 7f;
        private const float SuccessorRenownGain = 6f;
        private const float HeadOathkeepingGain = 2f;
        private const float GovernorStewardshipLoss = -1f;
        private const float DefenderCounterIntelligenceBoost = 3f;
        private const float FailedAssassinationLegitimacyGain = 1f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerCovertOpsSystem.GetInWorldDays(entityManager);

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

            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            for (int i = 0; i < operationEntities.Length; i++)
            {
                var operation = operations[i];
                if (!operation.Active ||
                    operation.Kind != CovertOpKindPlayer.Assassination ||
                    operation.ResolveAtInWorldDays > inWorldDays)
                {
                    continue;
                }

                ResolveOperation(entityManager, ecb, operationEntities[i], operation, inWorldDays);
            }
        }

        private static void ResolveOperation(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            Entity operationEntity,
            in PlayerCovertOpsResolutionComponent operation,
            float inWorldDays)
        {
            var sourceFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.SourceFactionId);
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.TargetFactionId);
            if (sourceFactionEntity == Entity.Null ||
                targetFactionEntity == Entity.Null ||
                !PlayerCounterIntelligenceSystem.TryResolveMemberEntity(
                    entityManager,
                    targetFactionEntity,
                    operation.TargetMemberId,
                    out var targetMemberEntity,
                    out var targetMember) ||
                !PlayerCovertOpsSystem.IsAvailable(targetMember.Status))
            {
                ecb.DestroyEntity(operationEntity);
                return;
            }

            PlayerCounterIntelligenceSystem.EnsureMutualHostility(
                entityManager,
                operation.SourceFactionId,
                operation.TargetFactionId);

            float offenseRenown =
                PlayerCovertOpsSystem.TrySelectOperator(entityManager, sourceFactionEntity, out var operatorMember)
                    ? operatorMember.Renown
                    : 0f;
            bool intelSupport = PlayerCovertOpsSystem.HasActiveIntelligenceReport(
                entityManager,
                operation.SourceFactionId,
                operation.TargetFactionId,
                inWorldDays);
            var contest = PlayerCovertOpsSystem.BuildAssassinationContest(
                entityManager,
                operation.SourceFactionId,
                operation.TargetFactionId,
                targetMember,
                offenseRenown,
                intelSupport,
                inWorldDays);

            if (contest.SuccessScore >= 0f)
            {
                ApplyAssassinationEffect(
                    entityManager,
                    sourceFactionEntity,
                    targetFactionEntity,
                    targetMemberEntity,
                    targetMember,
                    inWorldDays);
            }
            else
            {
                bool intercepted = PlayerCounterIntelligenceSystem.RecordCounterIntelligenceInterception(
                    entityManager,
                    operation.TargetFactionId,
                    operation.SourceFactionId,
                    new FixedString32Bytes("assassination"),
                    operation.TargetMemberId,
                    inWorldDays);
                PlayerCounterIntelligenceSystem.RefundDefenderInfluence(
                    entityManager,
                    targetFactionEntity,
                    operation.EscrowInfluence);
                PlayerCounterIntelligenceSystem.ApplyStewardship(entityManager, targetFactionEntity, 1f);
                ApplyCounterIntelligenceBoost(
                    entityManager,
                    targetFactionEntity,
                    inWorldDays,
                    DefenderCounterIntelligenceBoost);
                if (!intercepted)
                {
                    PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                        entityManager,
                        targetFactionEntity,
                        FailedAssassinationLegitimacyGain);
                }
            }

            ecb.DestroyEntity(operationEntity);
        }

        private static void ApplyAssassinationEffect(
            EntityManager entityManager,
            Entity sourceFactionEntity,
            Entity targetFactionEntity,
            Entity memberEntity,
            DynastyMemberComponent member,
            float inWorldDays)
        {
            member.Status = DynastyMemberStatus.Fallen;
            member.FallenAtWorldSeconds = inWorldDays * 86400f;
            entityManager.SetComponentData(memberEntity, member);

            if (!entityManager.HasBuffer<DynastyFallenLedger>(targetFactionEntity))
            {
                entityManager.AddBuffer<DynastyFallenLedger>(targetFactionEntity);
            }

            entityManager.GetBuffer<DynastyFallenLedger>(targetFactionEntity).Add(new DynastyFallenLedger
            {
                MemberId = member.MemberId,
                Title = member.Title,
                Role = member.Role,
                FallenAtWorldSeconds = member.FallenAtWorldSeconds,
            });

            switch (member.Role)
            {
                case DynastyRole.HeadOfBloodline:
                    ApplyHeadOfBloodlineRipple(entityManager, targetFactionEntity, member.MemberId);
                    break;

                case DynastyRole.Commander:
                    PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                        entityManager,
                        targetFactionEntity,
                        -CommanderLegitimacyPenalty);
                    KillCommanderUnits(entityManager, member.MemberId);
                    break;

                case DynastyRole.Governor:
                    PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                        entityManager,
                        targetFactionEntity,
                        -GovernorLegitimacyPenalty);
                    ClearGovernorAssignments(entityManager, member.MemberId);
                    ApplyConvictionEvent(
                        entityManager,
                        targetFactionEntity,
                        ConvictionBucket.Stewardship,
                        GovernorStewardshipLoss);
                    break;

                case DynastyRole.HeirDesignate:
                    PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                        entityManager,
                        targetFactionEntity,
                        -HeirLegitimacyPenalty);
                    break;

                case DynastyRole.Diplomat:
                case DynastyRole.IdeologicalLeader:
                case DynastyRole.Spymaster:
                    PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                        entityManager,
                        targetFactionEntity,
                        -CourtOfficerLegitimacyPenalty);
                    break;

                case DynastyRole.Merchant:
                    PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                        entityManager,
                        targetFactionEntity,
                        -MerchantLegitimacyPenalty);
                    break;
            }

            PlayerCounterIntelligenceSystem.ApplyRuthlessness(
                entityManager,
                sourceFactionEntity,
                AttackerRuthlessnessGain);
        }

        private static void ApplyHeadOfBloodlineRipple(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes fallenMemberId)
        {
            PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                entityManager,
                factionEntity,
                -HeadLegitimacyPenalty);
            ApplyConvictionEvent(
                entityManager,
                factionEntity,
                ConvictionBucket.Oathkeeping,
                HeadOathkeepingGain);

            if (TryFindSuccessor(entityManager, factionEntity, fallenMemberId, out var successorEntity, out var successor))
            {
                successor.Renown = math.min(60f, successor.Renown + SuccessorRenownGain);
                entityManager.SetComponentData(successorEntity, successor);
                PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                    entityManager,
                    factionEntity,
                    SuccessionRecoveryLegitimacy);
                return;
            }

            if (entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
                dynasty.Interregnum = true;
                entityManager.SetComponentData(factionEntity, dynasty);
            }

            PlayerCounterIntelligenceSystem.AdjustLegitimacy(
                entityManager,
                factionEntity,
                -InterregnumLegitimacyPenalty);
        }

        private static bool TryFindSuccessor(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes fallenMemberId,
            out Entity successorEntity,
            out DynastyMemberComponent successor)
        {
            successorEntity = Entity.Null;
            successor = default;
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            int bestOrder = int.MaxValue;
            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                var candidateEntity = roster[i].Member;
                if (candidateEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(candidateEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(candidateEntity);
                if (candidate.MemberId.Equals(fallenMemberId) ||
                    candidate.Status != DynastyMemberStatus.Active ||
                    candidate.Order >= bestOrder)
                {
                    continue;
                }

                bestOrder = candidate.Order;
                successorEntity = candidateEntity;
                successor = candidate;
            }

            return successorEntity != Entity.Null;
        }

        private static void ApplyCounterIntelligenceBoost(
            EntityManager entityManager,
            Entity factionEntity,
            float inWorldDays,
            float amount)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<PlayerCounterIntelligenceComponent>(factionEntity))
            {
                return;
            }

            var watch = entityManager.GetComponentData<PlayerCounterIntelligenceComponent>(factionEntity);
            if (watch.ExpiresAtInWorldDays <= inWorldDays)
            {
                return;
            }

            watch.WatchStrength += amount;
            entityManager.SetComponentData(factionEntity, watch);
        }

        private static void KillCommanderUnits(
            EntityManager entityManager,
            FixedString64Bytes memberId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<CommanderComponent>(),
                ComponentType.ReadWrite<HealthComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var commanders = query.ToComponentDataArray<CommanderComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!commanders[i].MemberId.Equals(memberId) ||
                    healthValues[i].Current <= 0f)
                {
                    continue;
                }

                var health = healthValues[i];
                health.Current = 0f;
                entityManager.SetComponentData(entities[i], health);
            }
        }

        private static void ClearGovernorAssignments(
            EntityManager entityManager,
            FixedString64Bytes memberId)
        {
            var seatQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GovernorSeatAssignmentComponent>());
            if (!seatQuery.IsEmpty)
            {
                using var entities = seatQuery.ToEntityArray(Allocator.Temp);
                using var seats = seatQuery.ToComponentDataArray<GovernorSeatAssignmentComponent>(Allocator.Temp);
                seatQuery.Dispose();

                for (int i = 0; i < entities.Length; i++)
                {
                    if (!seats[i].GovernorMemberId.Equals(memberId))
                    {
                        continue;
                    }

                    entityManager.RemoveComponent<GovernorSeatAssignmentComponent>(entities[i]);
                    if (entityManager.HasComponent<GovernorSpecializationComponent>(entities[i]))
                    {
                        entityManager.RemoveComponent<GovernorSpecializationComponent>(entities[i]);
                    }
                }
            }
            else
            {
                seatQuery.Dispose();
            }

            var specializationQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GovernorSpecializationComponent>());
            if (!specializationQuery.IsEmpty)
            {
                using var entities = specializationQuery.ToEntityArray(Allocator.Temp);
                using var profiles = specializationQuery.ToComponentDataArray<GovernorSpecializationComponent>(Allocator.Temp);
                specializationQuery.Dispose();

                for (int i = 0; i < entities.Length; i++)
                {
                    if (profiles[i].GovernorMemberId.Equals(memberId))
                    {
                        entityManager.RemoveComponent<GovernorSpecializationComponent>(entities[i]);
                    }
                }
            }
            else
            {
                specializationQuery.Dispose();
            }
        }

        private static void ApplyConvictionEvent(
            EntityManager entityManager,
            Entity factionEntity,
            ConvictionBucket bucket,
            float amount)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                return;
            }

            var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            ConvictionScoring.ApplyEvent(ref conviction, bucket, amount);
            entityManager.SetComponentData(factionEntity, conviction);
        }
    }
}
