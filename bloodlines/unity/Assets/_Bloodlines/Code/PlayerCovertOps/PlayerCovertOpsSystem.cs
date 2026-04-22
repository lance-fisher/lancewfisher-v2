using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// Player-side covert operation dispatch foundation plus assassination and sabotage
    /// follow-through. The lane keeps active-operation entities under PlayerCovertOps
    /// instead of widening Claude's AI-owned DynastyOperationComponent graph.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct PlayerCovertOpsSystem : ISystem
    {
        public const float EspionageCostGold = 45f;
        public const float EspionageCostInfluence = 16f;
        public const float EspionageDurationInWorldDays = 30f / 86400f;
        public const float IntelligenceReportDurationInWorldDays = 120f / 86400f;

        public const float AssassinationCostGold = 85f;
        public const float AssassinationCostInfluence = 28f;
        public const float AssassinationDurationInWorldDays = 34f / 86400f;

        private const float EspionageMinimumProjectedChance = 0.08f;
        private const float EspionageMaximumProjectedChance = 0.92f;
        private const float AssassinationMinimumProjectedChance = 0.06f;
        private const float AssassinationMaximumProjectedChance = 0.90f;
        private const float SabotageMinimumProjectedChance = 0.05f;
        private const float SabotageMaximumProjectedChance = 0.95f;

        private const float EspionageBaseOffense = 32f;
        private const float AssassinationBaseOffense = 36f;
        private const float AssassinationIntelSupportBonus = 12f;
        private const float SabotageBaseOffense = 45f;

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
                switch (requests[i].Kind)
                {
                    case CovertOpKindPlayer.Espionage:
                        TryDispatchEspionage(entityManager, requests[i], inWorldDays);
                        break;

                    case CovertOpKindPlayer.Assassination:
                        TryDispatchAssassination(entityManager, requests[i], inWorldDays);
                        break;

                    case CovertOpKindPlayer.Sabotage:
                        TryDispatchSabotage(entityManager, requests[i], inWorldDays);
                        break;
                }

                entityManager.DestroyEntity(requestEntities[i]);
            }
        }

        private static void TryDispatchEspionage(
            EntityManager entityManager,
            PlayerCovertOpsRequestComponent request,
            float inWorldDays)
        {
            if (request.SourceFactionId.Length == 0 ||
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

            if (!HasPlayerOperationCapacity(entityManager, request.SourceFactionId))
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
                request.TargetFactionId,
                operatorMember.Renown);

            stockpile.Gold -= EspionageCostGold;
            stockpile.Influence -= EspionageCostInfluence;
            entityManager.SetComponentData(sourceFactionEntity, stockpile);

            var operationEntity = entityManager.CreateEntity(typeof(PlayerCovertOpsResolutionComponent));
            entityManager.SetComponentData(operationEntity, new PlayerCovertOpsResolutionComponent
            {
                OperationId = BuildOperationId("player-espionage-", request.SourceFactionId, request.TargetFactionId, operationEntity.Index),
                Kind = CovertOpKindPlayer.Espionage,
                Subtype = default,
                SourceFactionId = request.SourceFactionId,
                TargetFactionId = request.TargetFactionId,
                TargetMemberId = default,
                TargetEntityIndex = -1,
                StartedAtInWorldDays = inWorldDays,
                ResolveAtInWorldDays = inWorldDays + EspionageDurationInWorldDays,
                ReportExpiresAtInWorldDays = inWorldDays + EspionageDurationInWorldDays + IntelligenceReportDurationInWorldDays,
                OperatorMemberId = operatorMember.MemberId,
                OperatorTitle = operatorMember.Title,
                TargetLabel = default,
                LocationLabel = default,
                SuccessScore = contest.SuccessScore,
                ProjectedChance = contest.ProjectedChance,
                IntelSupport = false,
                IntelSupportBonus = 0f,
                CounterIntelligenceDefense = contest.CounterIntelligenceDefense,
                CounterIntelligenceActive = false,
                BloodlineGuardBonus = 0f,
                Active = true,
                EscrowGold = EspionageCostGold,
                EscrowInfluence = EspionageCostInfluence,
            });
        }

        private static void TryDispatchAssassination(
            EntityManager entityManager,
            PlayerCovertOpsRequestComponent request,
            float inWorldDays)
        {
            if (request.SourceFactionId.Length == 0 ||
                request.TargetFactionId.Length == 0 ||
                request.TargetMemberId.Length == 0 ||
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

            if (!TryResolveDynastyMember(
                    entityManager,
                    targetFactionEntity,
                    request.TargetMemberId,
                    out var targetMember))
            {
                return;
            }

            if (!IsAvailable(targetMember.Status) ||
                HasActiveAssassinationForTarget(
                    entityManager,
                    request.SourceFactionId,
                    request.TargetFactionId,
                    request.TargetMemberId))
            {
                return;
            }

            if (!HasPlayerOperationCapacity(entityManager, request.SourceFactionId) ||
                !TrySelectOperator(entityManager, sourceFactionEntity, out var operatorMember))
            {
                return;
            }

            var stockpile = entityManager.GetComponentData<ResourceStockpileComponent>(sourceFactionEntity);
            if (stockpile.Gold < AssassinationCostGold ||
                stockpile.Influence < AssassinationCostInfluence)
            {
                return;
            }

            bool intelSupport = HasActiveIntelligenceReport(
                entityManager,
                request.SourceFactionId,
                request.TargetFactionId,
                inWorldDays);
            var contest = BuildAssassinationContest(
                entityManager,
                request.TargetFactionId,
                targetMember,
                operatorMember.Renown,
                intelSupport);

            stockpile.Gold -= AssassinationCostGold;
            stockpile.Influence -= AssassinationCostInfluence;
            entityManager.SetComponentData(sourceFactionEntity, stockpile);

            var operationEntity = entityManager.CreateEntity(typeof(PlayerCovertOpsResolutionComponent));
            entityManager.SetComponentData(operationEntity, new PlayerCovertOpsResolutionComponent
            {
                OperationId = BuildOperationId("player-assassination-", request.SourceFactionId, request.TargetFactionId, operationEntity.Index),
                Kind = CovertOpKindPlayer.Assassination,
                Subtype = default,
                SourceFactionId = request.SourceFactionId,
                TargetFactionId = request.TargetFactionId,
                TargetMemberId = request.TargetMemberId,
                TargetEntityIndex = -1,
                StartedAtInWorldDays = inWorldDays,
                ResolveAtInWorldDays = inWorldDays + AssassinationDurationInWorldDays,
                ReportExpiresAtInWorldDays = 0f,
                OperatorMemberId = operatorMember.MemberId,
                OperatorTitle = operatorMember.Title,
                TargetLabel = targetMember.Title.Length > 0 ? targetMember.Title : targetMember.MemberId,
                LocationLabel = contest.LocationLabel,
                SuccessScore = contest.SuccessScore,
                ProjectedChance = contest.ProjectedChance,
                IntelSupport = intelSupport,
                IntelSupportBonus = contest.IntelSupportBonus,
                CounterIntelligenceDefense = contest.CounterIntelligenceDefense,
                CounterIntelligenceActive = contest.CounterIntelligenceActive,
                BloodlineGuardBonus = contest.BloodlineGuardBonus,
                Active = true,
                EscrowGold = AssassinationCostGold,
                EscrowInfluence = AssassinationCostInfluence,
            });
        }

        private static void TryDispatchSabotage(
            EntityManager entityManager,
            PlayerCovertOpsRequestComponent request,
            float inWorldDays)
        {
            if (request.SourceFactionId.Length == 0 ||
                request.TargetFactionId.Length == 0 ||
                request.Subtype.Length == 0 ||
                request.TargetEntityIndex < 0 ||
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
                !entityManager.HasBuffer<DynastyMemberRef>(sourceFactionEntity))
            {
                return;
            }

            if (!TryResolveBuildingTarget(
                    entityManager,
                    request.TargetEntityIndex,
                    out var targetBuildingEntity,
                    out var buildingType,
                    out var buildingFaction,
                    out var buildingHealth,
                    out var buildingPosition))
            {
                return;
            }

            if (!buildingFaction.FactionId.Equals(request.TargetFactionId) ||
                buildingHealth.Current <= 0f ||
                entityManager.HasComponent<DeadTag>(targetBuildingEntity))
            {
                return;
            }

            if (!TryResolveSabotageTerms(request.Subtype, buildingType, out var sabotageTerms))
            {
                return;
            }

            if (!HasPlayerOperationCapacity(entityManager, request.SourceFactionId) ||
                !TrySelectSabotageOperator(entityManager, sourceFactionEntity, out var operatorMember))
            {
                return;
            }

            var stockpile = entityManager.GetComponentData<ResourceStockpileComponent>(sourceFactionEntity);
            if (stockpile.Gold < sabotageTerms.GoldCost ||
                stockpile.Influence < sabotageTerms.InfluenceCost)
            {
                return;
            }

            var contest = BuildSabotageContest(
                entityManager,
                request.TargetFactionId,
                buildingPosition,
                operatorMember.Renown);

            stockpile.Gold -= sabotageTerms.GoldCost;
            stockpile.Influence -= sabotageTerms.InfluenceCost;
            entityManager.SetComponentData(sourceFactionEntity, stockpile);

            var operationEntity = entityManager.CreateEntity(typeof(PlayerCovertOpsResolutionComponent));
            entityManager.SetComponentData(operationEntity, new PlayerCovertOpsResolutionComponent
            {
                OperationId = BuildOperationId("player-sabotage-", request.SourceFactionId, request.TargetFactionId, operationEntity.Index),
                Kind = CovertOpKindPlayer.Sabotage,
                Subtype = request.Subtype,
                SourceFactionId = request.SourceFactionId,
                TargetFactionId = request.TargetFactionId,
                TargetMemberId = default,
                TargetEntityIndex = targetBuildingEntity.Index,
                StartedAtInWorldDays = inWorldDays,
                ResolveAtInWorldDays = inWorldDays + sabotageTerms.DurationInWorldDays,
                ReportExpiresAtInWorldDays = 0f,
                OperatorMemberId = operatorMember.MemberId,
                OperatorTitle = operatorMember.Title,
                TargetLabel = buildingType.TypeId,
                LocationLabel = default,
                SuccessScore = contest.SuccessScore,
                ProjectedChance = contest.ProjectedChance,
                IntelSupport = false,
                IntelSupportBonus = 0f,
                CounterIntelligenceDefense = 0f,
                CounterIntelligenceActive = false,
                BloodlineGuardBonus = 0f,
                Active = true,
                EscrowGold = sabotageTerms.GoldCost,
                EscrowInfluence = sabotageTerms.InfluenceCost,
            });
        }

        private static bool HasPlayerOperationCapacity(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId)
        {
            int activeCount =
                DynastyOperationLimits.CountActiveForFaction(entityManager, sourceFactionId) +
                CountActivePlayerOperations(entityManager, sourceFactionId);
            return activeCount < DynastyOperationLimits.DYNASTY_OPERATION_ACTIVE_LIMIT;
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

        private static bool HasActiveAssassinationForTarget(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes targetMemberId)
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
                    operation.Kind != CovertOpKindPlayer.Assassination)
                {
                    continue;
                }

                if (operation.SourceFactionId.Equals(sourceFactionId) &&
                    operation.TargetFactionId.Equals(targetFactionId) &&
                    operation.TargetMemberId.Equals(targetMemberId))
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

        private static bool TrySelectSabotageOperator(
            EntityManager entityManager,
            Entity factionEntity,
            out DynastyMemberComponent member)
        {
            member = default;
            return TrySelectOperatorByRole(entityManager, factionEntity, DynastyRole.Spymaster, out member) ||
                   TrySelectOperatorByRole(entityManager, factionEntity, DynastyRole.Diplomat, out member) ||
                   TrySelectOperatorByPath(entityManager, factionEntity, DynastyPath.CovertOperations, out member);
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

        private static bool TrySelectOperatorByPath(
            EntityManager entityManager,
            Entity factionEntity,
            DynastyPath path,
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
                if (candidate.Path != path || !IsAvailable(candidate.Status))
                {
                    continue;
                }

                member = candidate;
                return true;
            }

            return false;
        }

        private static bool TryResolveDynastyMember(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes memberId,
            out DynastyMemberComponent member)
        {
            member = default;
            if (factionEntity == Entity.Null ||
                memberId.Length == 0 ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
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
                if (!candidate.MemberId.Equals(memberId))
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
            FixedString32Bytes targetFactionId,
            float offenseRenown)
        {
            float offense = EspionageBaseOffense + offenseRenown;
            float defense = ResolveDefenseRenown(entityManager, targetFactionId) +
                            ResolveHighestSettlementFortificationTier(entityManager, targetFactionId) * 6f;
            float successScore = offense - defense;
            float projectedChance = math.clamp(
                0.5f + (successScore / 100f),
                EspionageMinimumProjectedChance,
                EspionageMaximumProjectedChance);

            return new EspionageContest
            {
                SuccessScore = successScore,
                ProjectedChance = projectedChance,
                CounterIntelligenceDefense = 0f,
            };
        }

        private static AssassinationContest BuildAssassinationContest(
            EntityManager entityManager,
            FixedString32Bytes targetFactionId,
            DynastyMemberComponent targetMember,
            float offenseRenown,
            bool intelSupport)
        {
            var location = ResolveAssassinationLocationProfile(entityManager, targetFactionId, targetMember.Role);
            float offense = offenseRenown +
                            AssassinationBaseOffense +
                            (intelSupport ? AssassinationIntelSupportBonus : 0f) +
                            location.ExposureBonus;
            float defense = ResolveDefenseRenown(entityManager, targetFactionId) +
                            location.KeepTier * 7f +
                            (targetMember.Role == DynastyRole.HeadOfBloodline ? 8f : 0f);
            float successScore = offense - defense;
            float projectedChance = math.clamp(
                0.5f + (successScore / 100f),
                AssassinationMinimumProjectedChance,
                AssassinationMaximumProjectedChance);

            return new AssassinationContest
            {
                SuccessScore = successScore,
                ProjectedChance = projectedChance,
                LocationLabel = location.Label,
                IntelSupportBonus = intelSupport ? AssassinationIntelSupportBonus : 0f,
                CounterIntelligenceDefense = 0f,
                CounterIntelligenceActive = false,
                BloodlineGuardBonus = 0f,
            };
        }

        private static SabotageContest BuildSabotageContest(
            EntityManager entityManager,
            FixedString32Bytes targetFactionId,
            float3 targetBuildingPosition,
            float offenseRenown)
        {
            int fortificationTier = ResolveNearestSettlementFortificationTier(
                entityManager,
                targetFactionId,
                targetBuildingPosition);
            float defense = fortificationTier * 12f +
                            (ResolveDefenseRenown(entityManager, targetFactionId) > 0f ? 10f : 0f);
            float successScore = (offenseRenown + SabotageBaseOffense) - defense;
            float projectedChance = math.clamp(
                0.5f + (successScore / 100f),
                SabotageMinimumProjectedChance,
                SabotageMaximumProjectedChance);

            return new SabotageContest
            {
                SuccessScore = successScore,
                ProjectedChance = projectedChance,
            };
        }

        private static float ResolveDefenseRenown(
            EntityManager entityManager,
            FixedString32Bytes targetFactionId)
        {
            var targetFactionEntity = FindFactionEntity(entityManager, targetFactionId);
            if (targetFactionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyMemberRef>(targetFactionEntity))
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

        private static int ResolveHighestSettlementFortificationTier(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0;
            }

            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlements = query.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            query.Dispose();

            int highestTier = 0;
            for (int i = 0; i < settlements.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    highestTier = math.max(highestTier, settlements[i].FortificationTier);
                }
            }

            return highestTier;
        }

        private static int ResolveNearestSettlementFortificationTier(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            float3 buildingPosition)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0;
            }

            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlements = query.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            query.Dispose();

            float bestDistanceSq = float.MaxValue;
            int bestTier = 0;
            for (int i = 0; i < settlements.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                float distanceSq = math.distancesq(positions[i].Value, buildingPosition);
                if (distanceSq >= bestDistanceSq)
                {
                    continue;
                }

                bestDistanceSq = distanceSq;
                bestTier = settlements[i].FortificationTier;
            }

            return bestTier;
        }

        private static AssassinationLocationProfile ResolveAssassinationLocationProfile(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            DynastyRole targetRole)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return BuildFallbackAssassinationLocation(targetRole);
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlements = query.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            query.Dispose();

            int bestIndex = -1;
            int highestTier = -1;
            for (int i = 0; i < settlements.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                if (entityManager.HasComponent<PrimaryKeepTag>(entities[i]))
                {
                    bestIndex = i;
                    break;
                }

                if (settlements[i].FortificationTier > highestTier)
                {
                    highestTier = settlements[i].FortificationTier;
                    bestIndex = i;
                }
            }

            if (bestIndex < 0)
            {
                return BuildFallbackAssassinationLocation(targetRole);
            }

            var label = settlements[bestIndex].SettlementId.Length > 0
                ? settlements[bestIndex].SettlementId
                : BuildFallbackAssassinationLocation(targetRole).Label;
            return new AssassinationLocationProfile
            {
                Label = label,
                KeepTier = settlements[bestIndex].FortificationTier,
                ExposureBonus = ResolveExposureBonus(targetRole),
            };
        }

        private static AssassinationLocationProfile BuildFallbackAssassinationLocation(DynastyRole targetRole)
        {
            return new AssassinationLocationProfile
            {
                Label = targetRole == DynastyRole.Commander
                    ? new FixedString64Bytes("field-command")
                    : targetRole == DynastyRole.Governor
                        ? new FixedString64Bytes("governor-seat")
                        : new FixedString64Bytes("rival-court"),
                KeepTier = 0,
                ExposureBonus = ResolveExposureBonus(targetRole),
            };
        }

        private static float ResolveExposureBonus(DynastyRole targetRole)
        {
            return targetRole switch
            {
                DynastyRole.Commander => 6f,
                DynastyRole.Governor => 4f,
                DynastyRole.Diplomat => 3f,
                DynastyRole.Merchant => 3f,
                DynastyRole.Spymaster => 2f,
                _ => 0f,
            };
        }

        private static bool TryResolveSabotageTerms(
            FixedString32Bytes subtype,
            in BuildingTypeComponent targetBuilding,
            out SabotageTerms terms)
        {
            terms = default;

            if (subtype.Equals(new FixedString32Bytes("gate_opening")))
            {
                if (targetBuilding.FortificationRole != FortificationRole.Gate)
                {
                    return false;
                }

                terms = new SabotageTerms
                {
                    GoldCost = 60f,
                    InfluenceCost = 18f,
                    DurationInWorldDays = 28f / 86400f,
                };
                return true;
            }

            if (subtype.Equals(new FixedString32Bytes("fire_raising")))
            {
                terms = new SabotageTerms
                {
                    GoldCost = 40f,
                    InfluenceCost = 12f,
                    DurationInWorldDays = 24f / 86400f,
                };
                return true;
            }

            if (subtype.Equals(new FixedString32Bytes("supply_poisoning")))
            {
                if (!targetBuilding.SupportsSiegeLogistics &&
                    !targetBuilding.TypeId.Equals(new FixedString64Bytes("supply_camp")))
                {
                    return false;
                }

                terms = new SabotageTerms
                {
                    GoldCost = 50f,
                    InfluenceCost = 15f,
                    DurationInWorldDays = 30f / 86400f,
                };
                return true;
            }

            if (subtype.Equals(new FixedString32Bytes("well_poisoning")))
            {
                terms = new SabotageTerms
                {
                    GoldCost = 70f,
                    InfluenceCost = 20f,
                    DurationInWorldDays = 32f / 86400f,
                };
                return true;
            }

            return false;
        }

        private static bool TryResolveBuildingTarget(
            EntityManager entityManager,
            int entityIndex,
            out Entity targetEntity,
            out BuildingTypeComponent buildingType,
            out FactionComponent buildingFaction,
            out HealthComponent buildingHealth,
            out float3 buildingPosition)
        {
            targetEntity = Entity.Null;
            buildingType = default;
            buildingFaction = default;
            buildingHealth = default;
            buildingPosition = float3.zero;

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return false;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var buildingTypes = query.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var healthValues = query.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].Index != entityIndex)
                {
                    continue;
                }

                targetEntity = entities[i];
                buildingFaction = factions[i];
                buildingType = buildingTypes[i];
                buildingHealth = healthValues[i];
                buildingPosition = positions[i].Value;
                return true;
            }

            return false;
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
            string prefix,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            int entityIndex)
        {
            var id = new FixedString64Bytes(prefix);
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

        private struct AssassinationContest
        {
            public float SuccessScore;
            public float ProjectedChance;
            public FixedString64Bytes LocationLabel;
            public float IntelSupportBonus;
            public float CounterIntelligenceDefense;
            public bool CounterIntelligenceActive;
            public float BloodlineGuardBonus;
        }

        private struct SabotageContest
        {
            public float SuccessScore;
            public float ProjectedChance;
        }

        private struct SabotageTerms
        {
            public float GoldCost;
            public float InfluenceCost;
            public float DurationInWorldDays;
        }

        private struct AssassinationLocationProfile
        {
            public FixedString64Bytes Label;
            public int KeepTier;
            public float ExposureBonus;
        }
    }
}
