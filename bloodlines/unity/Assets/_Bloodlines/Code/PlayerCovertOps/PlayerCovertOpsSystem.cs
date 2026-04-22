using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Player-side covert operation dispatch foundation. This slice ports
    /// simulation.js getEspionageTerms/startEspionageOperation only and keeps
    /// the active-operation entities under the player-owned lane instead of the
    /// AI-owned DynastyOperationComponent graph.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerCovertOpsSystem : ISystem
    {
        public const float EspionageCostGold = 45f;
        public const float EspionageCostInfluence = 16f;
        public const float EspionageDurationInWorldDays = 30f / 86400f;
        public const float IntelligenceReportDurationInWorldDays = 120f / 86400f;
        private const float MinimumProjectedChance = 0.08f;
        private const float MaximumProjectedChance = 0.92f;
        private const float BaseOffense = 32f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerCovertOpsRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = GetInWorldDays(entityManager);

            var requestQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsRequestComponent>());
            using var requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using var requests = requestQuery.ToComponentDataArray<PlayerCovertOpsRequestComponent>(Allocator.Temp);
            requestQuery.Dispose();

            for (int i = 0; i < requestEntities.Length; i++)
            {
                TryDispatchEspionage(entityManager, requests[i], inWorldDays);
                entityManager.DestroyEntity(requestEntities[i]);
            }
        }

        private static void TryDispatchEspionage(
            EntityManager entityManager,
            PlayerCovertOpsRequestComponent request,
            float inWorldDays)
        {
            if (request.Kind != CovertOpKindPlayer.Espionage ||
                request.SourceFactionId.Length == 0 ||
                request.TargetFactionId.Length == 0 ||
                request.SourceFactionId.Equals(request.TargetFactionId))
            {
                return;
            }

            var sourceFactionEntity = FindFactionEntity(entityManager, request.SourceFactionId);
            var targetFactionEntity = FindFactionEntity(entityManager, request.TargetFactionId);
            if (sourceFactionEntity == Entity.Null || targetFactionEntity == Entity.Null)
            {
                return;
            }

            if (!entityManager.HasComponent<ResourceStockpileComponent>(sourceFactionEntity) ||
                !entityManager.HasBuffer<DynastyMemberRef>(sourceFactionEntity) ||
                !entityManager.HasBuffer<DynastyMemberRef>(targetFactionEntity))
            {
                return;
            }

            if (HasActiveEspionageForTarget(entityManager, request.SourceFactionId, request.TargetFactionId) ||
                HasActiveIntelligenceReport(entityManager, request.SourceFactionId, request.TargetFactionId, inWorldDays))
            {
                return;
            }

            int activeCount =
                DynastyOperationLimits.CountActiveForFaction(entityManager, request.SourceFactionId) +
                CountActivePlayerOperations(entityManager, request.SourceFactionId);
            if (activeCount >= DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT)
            {
                return;
            }

            if (!TrySelectOperator(
                    entityManager,
                    sourceFactionEntity,
                    out var operatorMember))
            {
                return;
            }

            var stockpile = entityManager.GetComponentData<ResourceStockpileComponent>(sourceFactionEntity);
            if (stockpile.Gold < EspionageCostGold ||
                stockpile.Influence < EspionageCostInfluence)
            {
                return;
            }

            var contest = BuildEspionageContest(
                entityManager,
                targetFactionEntity,
                operatorMember.Renown);

            stockpile.Gold -= EspionageCostGold;
            stockpile.Influence -= EspionageCostInfluence;
            entityManager.SetComponentData(sourceFactionEntity, stockpile);

            var operationEntity = entityManager.CreateEntity(typeof(PlayerCovertOpsResolutionComponent));
            entityManager.SetComponentData(operationEntity, new PlayerCovertOpsResolutionComponent
            {
                OperationId = BuildOperationId(request.SourceFactionId, request.TargetFactionId, operationEntity.Index),
                Kind = CovertOpKindPlayer.Espionage,
                SourceFactionId = request.SourceFactionId,
                TargetFactionId = request.TargetFactionId,
                TargetMemberId = request.TargetMemberId,
                TargetEntityIndex = request.TargetEntityIndex,
                StartedAtInWorldDays = inWorldDays,
                ResolveAtInWorldDays = inWorldDays + EspionageDurationInWorldDays,
                ReportExpiresAtInWorldDays = inWorldDays + EspionageDurationInWorldDays + IntelligenceReportDurationInWorldDays,
                OperatorMemberId = operatorMember.MemberId,
                OperatorTitle = operatorMember.Title,
                SuccessScore = contest.SuccessScore,
                ProjectedChance = contest.ProjectedChance,
                CounterIntelligenceDefense = contest.CounterIntelligenceDefense,
                CounterIntelligenceActive = false,
                Active = true,
                EscrowGold = EspionageCostGold,
                EscrowInfluence = EspionageCostInfluence,
            });
        }

        private static int CountActivePlayerOperations(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0;
            }

            using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            query.Dispose();

            int count = 0;
            for (int i = 0; i < operations.Length; i++)
            {
                if (!operations[i].Active)
                {
                    continue;
                }

                if (operations[i].SourceFactionId.Equals(factionId))
                {
                    count++;
                }
            }

            return count;
        }

        private static bool HasActiveEspionageForTarget(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < operations.Length; i++)
            {
                var operation = operations[i];
                if (!operation.Active ||
                    operation.Kind != CovertOpKindPlayer.Espionage)
                {
                    continue;
                }

                if (operation.SourceFactionId.Equals(sourceFactionId) &&
                    operation.TargetFactionId.Equals(targetFactionId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasActiveIntelligenceReport(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < operations.Length; i++)
            {
                var operation = operations[i];
                if (operation.Kind != CovertOpKindPlayer.Espionage ||
                    !operation.SourceFactionId.Equals(sourceFactionId) ||
                    !operation.TargetFactionId.Equals(targetFactionId))
                {
                    continue;
                }

                if (operation.ReportExpiresAtInWorldDays > inWorldDays)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TrySelectOperator(
            EntityManager entityManager,
            Entity factionEntity,
            out DynastyMemberComponent member)
        {
            member = default;
            return TrySelectOperatorByRole(entityManager, factionEntity, DynastyRole.Spymaster, out member) ||
                   TrySelectOperatorByRole(entityManager, factionEntity, DynastyRole.Diplomat, out member) ||
                   TrySelectOperatorByRole(entityManager, factionEntity, DynastyRole.Merchant, out member);
        }

        private static bool TrySelectOperatorByRole(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyRole role,
            out DynastyMemberComponent member)
        {
            member = default;
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (candidate.Role != role || !IsAvailable(candidate.Status))
                {
                    continue;
                }

                member = candidate;
                return true;
            }

            return false;
        }

        private static EspionageContest BuildEspionageContest(
            EntityManager entityManager,
            Entity targetFactionEntity,
            float offenseRenown)
        {
            float offense = BaseOffense + offenseRenown;
            float defense = ResolveDefenseRenown(entityManager, targetFactionEntity) +
                            ResolveFortificationDefense(entityManager, targetFactionEntity);
            float successScore = offense - defense;
            float projectedChance = math.clamp(
                0.5f + (successScore / 100f),
                MinimumProjectedChance,
                MaximumProjectedChance);

            return new EspionageContest
            {
                SuccessScore = successScore,
                ProjectedChance = projectedChance,
                CounterIntelligenceDefense = 0f,
            };
        }

        private static float ResolveDefenseRenown(
            EntityManager entityManager,
            Entity targetFactionEntity)
        {
            if (!entityManager.HasBuffer<DynastyMemberRef>(targetFactionEntity))
            {
                return 0f;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(targetFactionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if ((member.Role == DynastyRole.Spymaster || member.Role == DynastyRole.Diplomat) &&
                    IsAvailable(member.Status))
                {
                    return member.Renown * 0.55f;
                }
            }

            return 0f;
        }

        private static float ResolveFortificationDefense(
            EntityManager entityManager,
            Entity targetFactionEntity)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0f;
            }

            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlements = query.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            query.Dispose();

            int highestTier = 0;
            var targetFactionId = entityManager.GetComponentData<FactionComponent>(targetFactionEntity).FactionId;
            for (int i = 0; i < settlements.Length; i++)
            {
                if (factions[i].FactionId.Equals(targetFactionId))
                {
                    highestTier = math.max(highestTier, settlements[i].FortificationTier);
                }
            }

            return highestTier * 6f;
        }

        private static Entity FindFactionEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static float GetInWorldDays(EntityManager entityManager)
        {
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0f;
            }

            float inWorldDays = query.GetSingleton<DualClockComponent>().InWorldDays;
            query.Dispose();
            return inWorldDays;
        }

        private static bool IsAvailable(DynastyMemberStatus status)
        {
            return status == DynastyMemberStatus.Active ||
                   status == DynastyMemberStatus.Ruling;
        }

        private static FixedString64Bytes BuildOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            int entityIndex)
        {
            var id = new FixedString64Bytes("player-espionage-");
            id.Append(sourceFactionId);
            id.Append("-to-");
            id.Append(targetFactionId);
            id.Append("-");
            id.Append(entityIndex);
            return id;
        }

        private struct EspionageContest
        {
            public float SuccessScore;
            public float ProjectedChance;
            public float CounterIntelligenceDefense;
        }
    }
}
