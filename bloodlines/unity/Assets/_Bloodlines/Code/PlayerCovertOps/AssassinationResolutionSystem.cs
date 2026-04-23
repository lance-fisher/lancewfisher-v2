using Bloodlines.Components;
using Bloodlines.Combat;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Bloodlines.TerritoryGovernance;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Resolves player assassination operations and applies the immediate bloodline,
    /// legitimacy, conviction, and attachment-clear follow-through that the browser
    /// runtime lands when a covert kill succeeds.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCounterIntelligenceSystem))]
    public partial struct AssassinationResolutionSystem : ISystem
    {
        private const float CommanderLegitimacyLoss = 9f;
        private const float GovernorLegitimacyLoss = 5f;
        private const float AssassinationRuthlessnessGain = 2f;
        private const float GovernorStewardshipLoss = -1f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerCovertOpsSystem.GetInWorldDays(entityManager);
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            try
            {
                var query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
                if (query.IsEmpty)
                {
                    query.Dispose();
                    return;
                }

                using var entities = query.ToEntityArray(Allocator.Temp);
                using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
                query.Dispose();

                for (int i = 0; i < entities.Length; i++)
                {
                    var operation = operations[i];
                    if (!operation.Active ||
                        operation.Kind != CovertOpKindPlayer.Assassination ||
                        operation.ResolveAtInWorldDays > inWorldDays)
                    {
                        continue;
                    }

                    ResolveAssassinationOperation(
                        entityManager,
                        ref ecb,
                        entities[i],
                        operation,
                        inWorldDays);
                }

                ecb.Playback(entityManager);
            }
            finally
            {
                ecb.Dispose();
            }
        }

        private static void ResolveAssassinationOperation(
            EntityManager entityManager,
            ref EntityCommandBuffer ecb,
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

            if (operation.SuccessScore >= 0f)
            {
                ApplyAssassinationEffect(
                    entityManager,
                    ref ecb,
                    operation.SourceFactionId,
                    operation.TargetFactionId,
                    sourceFactionEntity,
                    targetFactionEntity,
                    targetMemberEntity,
                    targetMember,
                    inWorldDays);
            }
            else
            {
                PlayerCounterIntelligenceSystem.RecordCounterIntelligenceInterception(
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
            }

            ecb.DestroyEntity(operationEntity);
        }

        private static void ApplyAssassinationEffect(
            EntityManager entityManager,
            ref EntityCommandBuffer ecb,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            Entity sourceFactionEntity,
            Entity targetFactionEntity,
            Entity targetMemberEntity,
            DynastyMemberComponent targetMember,
            float inWorldDays)
        {
            targetMember.Status = DynastyMemberStatus.Fallen;
            targetMember.FallenAtWorldSeconds = inWorldDays * 86400f;
            entityManager.SetComponentData(targetMemberEntity, targetMember);

            if (targetMember.Role == DynastyRole.Commander)
            {
                PlayerCounterIntelligenceSystem.AdjustLegitimacy(entityManager, targetFactionEntity, -CommanderLegitimacyLoss);
            }

            if (targetMember.Role == DynastyRole.Governor)
            {
                PlayerCounterIntelligenceSystem.AdjustLegitimacy(entityManager, targetFactionEntity, -GovernorLegitimacyLoss);
                PlayerCounterIntelligenceSystem.ApplyStewardship(entityManager, targetFactionEntity, GovernorStewardshipLoss);
            }

            if (sourceFactionEntity != Entity.Null)
            {
                PlayerCounterIntelligenceSystem.ApplyConviction(
                    entityManager,
                    sourceFactionEntity,
                    ConvictionBucket.Ruthlessness,
                    AssassinationRuthlessnessGain);
            }

            if (entityManager.HasBuffer<DynastyFallenLedger>(targetFactionEntity))
            {
                var fallen = entityManager.GetBuffer<DynastyFallenLedger>(targetFactionEntity);
                fallen.Add(new DynastyFallenLedger
                {
                    MemberId = targetMember.MemberId,
                    Title = targetMember.Title,
                    Role = targetMember.Role,
                    FallenAtWorldSeconds = targetMember.FallenAtWorldSeconds,
                });
            }

            ClearCommanderLinks(entityManager, ref ecb, targetFactionId, targetMember.MemberId);
            ClearGovernorLinks(entityManager, ref ecb, targetMember.MemberId);
        }

        private static void ClearCommanderLinks(
            EntityManager entityManager,
            ref EntityCommandBuffer ecb,
            FixedString32Bytes factionId,
            FixedString64Bytes memberId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<CommanderComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var commanders = query.ToComponentDataArray<CommanderComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId) ||
                    !commanders[i].MemberId.Equals(memberId))
                {
                    continue;
                }

                ecb.RemoveComponent<CommanderComponent>(entities[i]);
                if (entityManager.HasComponent<CommanderAuraComponent>(entities[i]))
                {
                    ecb.RemoveComponent<CommanderAuraComponent>(entities[i]);
                }

                if (entityManager.HasComponent<CommanderAuraRecipientComponent>(entities[i]))
                {
                    ecb.RemoveComponent<CommanderAuraRecipientComponent>(entities[i]);
                }

                if (entityManager.HasComponent<CommanderAtKeepTag>(entities[i]))
                {
                    ecb.RemoveComponent<CommanderAtKeepTag>(entities[i]);
                }
            }
        }

        private static void ClearGovernorLinks(
            EntityManager entityManager,
            ref EntityCommandBuffer ecb,
            FixedString64Bytes memberId)
        {
            var assignmentQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GovernorSeatAssignmentComponent>());
            if (!assignmentQuery.IsEmpty)
            {
                using var assignmentEntities = assignmentQuery.ToEntityArray(Allocator.Temp);
                using var assignments = assignmentQuery.ToComponentDataArray<GovernorSeatAssignmentComponent>(Allocator.Temp);
                assignmentQuery.Dispose();

                for (int i = 0; i < assignmentEntities.Length; i++)
                {
                    if (assignments[i].GovernorMemberId.Equals(memberId))
                    {
                        ecb.RemoveComponent<GovernorSeatAssignmentComponent>(assignmentEntities[i]);
                    }
                }
            }
            else
            {
                assignmentQuery.Dispose();
            }

            var specializationQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<GovernorSpecializationComponent>());
            if (!specializationQuery.IsEmpty)
            {
                using var specializationEntities = specializationQuery.ToEntityArray(Allocator.Temp);
                using var specializations = specializationQuery.ToComponentDataArray<GovernorSpecializationComponent>(Allocator.Temp);
                specializationQuery.Dispose();

                for (int i = 0; i < specializationEntities.Length; i++)
                {
                    if (specializations[i].GovernorMemberId.Equals(memberId))
                    {
                        ecb.RemoveComponent<GovernorSpecializationComponent>(specializationEntities[i]);
                    }
                }
            }
            else
            {
                specializationQuery.Dispose();
            }
        }
    }
}
