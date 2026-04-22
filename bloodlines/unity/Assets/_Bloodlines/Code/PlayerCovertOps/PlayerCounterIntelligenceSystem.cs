using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerCovertOps
{
    /// <summary>
    /// DualClock-driven covert-op resolution for player espionage, assassination,
    /// and counter-intelligence watches. Also owns report/watch expiry pruning so
    /// the player lane mirrors browser-side live surveillance windows.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlayerCovertOpsSystem))]
    public partial struct PlayerCounterIntelligenceSystem : ISystem
    {
        private const int IntelligenceReportLimit = 6;
        private const float CounterIntelligenceStewardshipGain = 1f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = PlayerCovertOpsSystem.GetInWorldDays(entityManager);

            ExpireIntelligenceReports(entityManager, inWorldDays);
            ExpireCounterIntelligenceWatches(entityManager, inWorldDays);

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

            for (int i = 0; i < operationEntities.Length; i++)
            {
                var operation = operations[i];
                if (!operation.Active ||
                    operation.ResolveAtInWorldDays > inWorldDays)
                {
                    continue;
                }

                switch (operation.Kind)
                {
                    case CovertOpKindPlayer.CounterIntelligence:
                        ResolveCounterIntelligenceOperation(
                            entityManager,
                            operationEntities[i],
                            operation,
                            inWorldDays);
                        break;

                    case CovertOpKindPlayer.Espionage:
                        ResolveEspionageOperation(
                            entityManager,
                            operationEntities[i],
                            operation,
                            inWorldDays);
                        break;

                    case CovertOpKindPlayer.Assassination:
                        ResolveAssassinationOperation(
                            entityManager,
                            operationEntities[i],
                            operation,
                            inWorldDays);
                        break;
                }
            }
        }

        private static void ResolveCounterIntelligenceOperation(
            EntityManager entityManager,
            Entity operationEntity,
            in PlayerCovertOpsResolutionComponent operation,
            float inWorldDays)
        {
            var factionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.SourceFactionId);
            if (factionEntity == Entity.Null)
            {
                entityManager.DestroyEntity(operationEntity);
                return;
            }

            var watch = new PlayerCounterIntelligenceComponent
            {
                WatchId = BuildDerivedId("dynastyCounter-", operation.SourceFactionId, operation.SourceFactionId, inWorldDays, operationEntity.Index),
                ActivatedAtInWorldDays = inWorldDays,
                ExpiresAtInWorldDays = inWorldDays + operation.WatchDurationInWorldDays,
                OperatorMemberId = operation.OperatorMemberId,
                OperatorTitle = operation.OperatorTitle,
                WatchStrength = operation.WatchStrength,
                WardLabel = operation.WardLabel,
                GuardedRoles = operation.GuardedRoles,
                AverageLoyalty = operation.AverageLoyalty,
                WeakestLoyalty = operation.WeakestLoyalty,
                Interceptions = 0,
                FoiledEspionage = 0,
                FoiledAssassinations = 0,
                LastInterceptAtInWorldDays = 0f,
                LastInterceptType = default,
                LastSourceFactionId = default,
                LastTargetMemberId = default,
                LastSourceInterceptions = 0,
                LastSourceFoiledEspionage = 0,
                LastSourceFoiledAssassinations = 0,
                LastDossierAtInWorldDays = 0f,
            };

            if (entityManager.HasComponent<PlayerCounterIntelligenceComponent>(factionEntity))
            {
                entityManager.SetComponentData(factionEntity, watch);
            }
            else
            {
                entityManager.AddComponentData(factionEntity, watch);
            }

            ApplyStewardship(entityManager, factionEntity, CounterIntelligenceStewardshipGain);
            entityManager.DestroyEntity(operationEntity);
        }

        private static void ResolveEspionageOperation(
            EntityManager entityManager,
            Entity operationEntity,
            in PlayerCovertOpsResolutionComponent operation,
            float inWorldDays)
        {
            var sourceFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.SourceFactionId);
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.TargetFactionId);
            if (sourceFactionEntity == Entity.Null || targetFactionEntity == Entity.Null)
            {
                entityManager.DestroyEntity(operationEntity);
                return;
            }

            float offenseRenown =
                PlayerCovertOpsSystem.TrySelectOperator(entityManager, sourceFactionEntity, out var operatorMember)
                    ? operatorMember.Renown
                    : 0f;
            var contest = PlayerCovertOpsSystem.BuildEspionageContest(
                entityManager,
                operation.SourceFactionId,
                operation.TargetFactionId,
                offenseRenown,
                inWorldDays);

            if (contest.SuccessScore >= 0f &&
                TryCreateIntelligenceReport(
                    entityManager,
                    operation.SourceFactionId,
                    operation.TargetFactionId,
                    new FixedString32Bytes("espionage"),
                    new FixedString64Bytes("Court report"),
                    default,
                    0,
                    inWorldDays,
                    operation.ReportExpiresAtInWorldDays,
                    operationEntity.Index,
                    out var report))
            {
                StoreIntelligenceReport(entityManager, sourceFactionEntity, report);
                ApplyStewardship(entityManager, sourceFactionEntity, CounterIntelligenceStewardshipGain);
            }
            else
            {
                RecordCounterIntelligenceInterception(
                    entityManager,
                    operation.TargetFactionId,
                    operation.SourceFactionId,
                    new FixedString32Bytes("espionage"),
                    default,
                    inWorldDays);
                EnsureMutualHostility(
                    entityManager,
                    operation.SourceFactionId,
                    operation.TargetFactionId);
                ApplyStewardship(entityManager, targetFactionEntity, CounterIntelligenceStewardshipGain);
            }

            entityManager.DestroyEntity(operationEntity);
        }

        private static void ResolveAssassinationOperation(
            EntityManager entityManager,
            Entity operationEntity,
            in PlayerCovertOpsResolutionComponent operation,
            float inWorldDays)
        {
            var sourceFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.SourceFactionId);
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, operation.TargetFactionId);
            if (sourceFactionEntity == Entity.Null ||
                targetFactionEntity == Entity.Null ||
                !TryResolveMemberEntity(
                    entityManager,
                    targetFactionEntity,
                    operation.TargetMemberId,
                    out var targetMemberEntity,
                    out var targetMember) ||
                !PlayerCovertOpsSystem.IsAvailable(targetMember.Status))
            {
                entityManager.DestroyEntity(operationEntity);
                return;
            }

            EnsureMutualHostility(
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
                    targetFactionEntity,
                    targetMemberEntity,
                    targetMember,
                    inWorldDays);
            }
            else
            {
                RecordCounterIntelligenceInterception(
                    entityManager,
                    operation.TargetFactionId,
                    operation.SourceFactionId,
                    new FixedString32Bytes("assassination"),
                    operation.TargetMemberId,
                    inWorldDays);
                RefundDefenderInfluence(entityManager, targetFactionEntity, operation.EscrowInfluence);
                ApplyStewardship(entityManager, targetFactionEntity, CounterIntelligenceStewardshipGain);
            }

            entityManager.DestroyEntity(operationEntity);
        }

        private static void ExpireIntelligenceReports(
            EntityManager entityManager,
            float inWorldDays)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadWrite<IntelligenceReportElement>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var reports = entityManager.GetBuffer<IntelligenceReportElement>(entities[i]);
                for (int j = reports.Length - 1; j >= 0; j--)
                {
                    if (reports[j].ExpiresAtInWorldDays <= inWorldDays)
                    {
                        reports.RemoveAt(j);
                    }
                }
            }
        }

        private static void ExpireCounterIntelligenceWatches(
            EntityManager entityManager,
            float inWorldDays)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCounterIntelligenceComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return;
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var watches = query.ToComponentDataArray<PlayerCounterIntelligenceComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (watches[i].ExpiresAtInWorldDays <= inWorldDays)
                {
                    entityManager.RemoveComponent<PlayerCounterIntelligenceComponent>(entities[i]);
                }
            }
        }

        private static bool TryCreateIntelligenceReport(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString32Bytes sourceType,
            FixedString64Bytes reportLabel,
            FixedString32Bytes interceptType,
            int interceptCount,
            float createdAtInWorldDays,
            float expiresAtInWorldDays,
            int seed,
            out IntelligenceReportElement report)
        {
            report = default;
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, targetFactionId);
            if (targetFactionEntity == Entity.Null)
            {
                return false;
            }

            int legitimacy = 0;
            if (entityManager.HasComponent<DynastyStateComponent>(targetFactionEntity))
            {
                legitimacy = (int)math.round(entityManager.GetComponentData<DynastyStateComponent>(targetFactionEntity).Legitimacy);
            }

            report = new IntelligenceReportElement
            {
                ReportId = BuildDerivedId("dynastyIntel-", sourceFactionId, targetFactionId, createdAtInWorldDays, seed),
                SourceFactionId = sourceFactionId,
                TargetFactionId = targetFactionId,
                SourceType = sourceType,
                ReportLabel = reportLabel,
                InterceptType = interceptType,
                InterceptCount = interceptCount,
                CreatedAtInWorldDays = createdAtInWorldDays,
                ExpiresAtInWorldDays = expiresAtInWorldDays,
                TargetLegitimacy = legitimacy,
                TargetActiveOperations =
                    DynastyOperationLimits.CountActiveForFaction(entityManager, targetFactionId) +
                    PlayerCovertOpsSystem.CountActivePlayerOperations(entityManager, targetFactionId),
                TargetCaptiveCount = CountCaptives(entityManager, targetFactionEntity),
                TargetLesserHouseCount = entityManager.HasBuffer<LesserHouseElement>(targetFactionEntity)
                    ? entityManager.GetBuffer<LesserHouseElement>(targetFactionEntity).Length
                    : 0,
                MemberSummary = BuildMemberSummary(entityManager, targetFactionId, targetFactionEntity),
            };

            return true;
        }

        private static void StoreIntelligenceReport(
            EntityManager entityManager,
            Entity factionEntity,
            in IntelligenceReportElement report)
        {
            EnsureIntelligenceReportBuffer(entityManager, factionEntity);
            var reports = entityManager.GetBuffer<IntelligenceReportElement>(factionEntity);
            using var retained = new NativeList<IntelligenceReportElement>(Allocator.Temp);

            for (int i = 0; i < reports.Length; i++)
            {
                if (reports[i].TargetFactionId.Equals(report.TargetFactionId) &&
                    reports[i].SourceType.Equals(report.SourceType))
                {
                    continue;
                }

                retained.Add(reports[i]);
            }

            reports.Clear();
            reports.Add(report);
            for (int i = 0; i < retained.Length && reports.Length < IntelligenceReportLimit; i++)
            {
                reports.Add(retained[i]);
            }
        }

        private static bool RecordCounterIntelligenceInterception(
            EntityManager entityManager,
            FixedString32Bytes targetFactionId,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes interceptType,
            FixedString64Bytes targetMemberId,
            float inWorldDays)
        {
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, targetFactionId);
            if (targetFactionEntity == Entity.Null ||
                !entityManager.HasComponent<PlayerCounterIntelligenceComponent>(targetFactionEntity))
            {
                return false;
            }

            var watch = entityManager.GetComponentData<PlayerCounterIntelligenceComponent>(targetFactionEntity);
            if (watch.ExpiresAtInWorldDays <= inWorldDays)
            {
                return false;
            }

            bool sameSource = watch.LastSourceFactionId.Equals(sourceFactionId);
            watch.Interceptions += 1;
            watch.LastInterceptAtInWorldDays = inWorldDays;
            watch.LastInterceptType = interceptType;
            watch.LastSourceFactionId = sourceFactionId;
            watch.LastTargetMemberId = targetMemberId;
            watch.LastSourceInterceptions = sameSource ? watch.LastSourceInterceptions + 1 : 1;

            if (interceptType.Equals(new FixedString32Bytes("assassination")))
            {
                watch.FoiledAssassinations += 1;
                watch.LastSourceFoiledAssassinations = sameSource ? watch.LastSourceFoiledAssassinations + 1 : 1;
                watch.LastSourceFoiledEspionage = sameSource ? watch.LastSourceFoiledEspionage : 0;
                AdjustLegitimacy(entityManager, targetFactionEntity, 2f);
            }
            else if (interceptType.Equals(new FixedString32Bytes("espionage")))
            {
                watch.FoiledEspionage += 1;
                watch.LastSourceFoiledEspionage = sameSource ? watch.LastSourceFoiledEspionage + 1 : 1;
                watch.LastSourceFoiledAssassinations = sameSource ? watch.LastSourceFoiledAssassinations : 0;
                AdjustLegitimacy(entityManager, targetFactionEntity, 1f);
            }

            if (sourceFactionId.Length > 0 &&
                TryCreateIntelligenceReport(
                    entityManager,
                    targetFactionId,
                    sourceFactionId,
                    new FixedString32Bytes("counter_intelligence"),
                    new FixedString64Bytes("Counter-intelligence dossier"),
                    interceptType,
                    watch.LastSourceInterceptions,
                    inWorldDays,
                    inWorldDays + PlayerCovertOpsSystem.IntelligenceReportDurationInWorldDays,
                    watch.Interceptions,
                    out var dossier))
            {
                StoreIntelligenceReport(entityManager, targetFactionEntity, dossier);
                watch.LastDossierAtInWorldDays = inWorldDays;
            }

            entityManager.SetComponentData(targetFactionEntity, watch);
            return true;
        }

        private static void EnsureMutualHostility(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var sourceFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, sourceFactionId);
            var targetFactionEntity = PlayerCovertOpsSystem.FindFactionEntity(entityManager, targetFactionId);
            if (sourceFactionEntity == Entity.Null || targetFactionEntity == Entity.Null)
            {
                return;
            }

            EnsureHostility(entityManager, sourceFactionEntity, targetFactionId);
            EnsureHostility(entityManager, targetFactionEntity, sourceFactionId);
        }

        private static void EnsureHostility(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes hostileFactionId)
        {
            if (!entityManager.HasBuffer<HostilityComponent>(factionEntity))
            {
                entityManager.AddBuffer<HostilityComponent>(factionEntity);
            }

            var hostility = entityManager.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < hostility.Length; i++)
            {
                if (hostility[i].HostileFactionId.Equals(hostileFactionId))
                {
                    return;
                }
            }

            hostility.Add(new HostilityComponent
            {
                HostileFactionId = hostileFactionId,
            });
        }

        private static void ApplyAssassinationEffect(
            EntityManager entityManager,
            Entity factionEntity,
            Entity memberEntity,
            DynastyMemberComponent member,
            float inWorldDays)
        {
            member.Status = DynastyMemberStatus.Fallen;
            member.FallenAtWorldSeconds = inWorldDays * 86400f;
            entityManager.SetComponentData(memberEntity, member);

            if (entityManager.HasBuffer<DynastyFallenLedger>(factionEntity))
            {
                var fallen = entityManager.GetBuffer<DynastyFallenLedger>(factionEntity);
                fallen.Add(new DynastyFallenLedger
                {
                    MemberId = member.MemberId,
                    Title = member.Title,
                    Role = member.Role,
                    FallenAtWorldSeconds = member.FallenAtWorldSeconds,
                });
            }
        }

        private static void RefundDefenderInfluence(
            EntityManager entityManager,
            Entity factionEntity,
            float escrowInfluence)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<ResourceStockpileComponent>(factionEntity))
            {
                return;
            }

            var stockpile = entityManager.GetComponentData<ResourceStockpileComponent>(factionEntity);
            stockpile.Influence += math.max(8f, math.round(escrowInfluence * 0.3f));
            entityManager.SetComponentData(factionEntity, stockpile);
        }

        private static void AdjustLegitimacy(
            EntityManager entityManager,
            Entity factionEntity,
            float delta)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                return;
            }

            var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            dynasty.Legitimacy = math.clamp(dynasty.Legitimacy + delta, 0f, 100f);
            entityManager.SetComponentData(factionEntity, dynasty);
        }

        private static void ApplyStewardship(
            EntityManager entityManager,
            Entity factionEntity,
            float amount)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<ConvictionComponent>(factionEntity))
            {
                return;
            }

            var conviction = entityManager.GetComponentData<ConvictionComponent>(factionEntity);
            ConvictionScoring.ApplyEvent(
                ref conviction,
                ConvictionBucket.Stewardship,
                amount);
            entityManager.SetComponentData(factionEntity, conviction);
        }

        private static void EnsureIntelligenceReportBuffer(
            EntityManager entityManager,
            Entity factionEntity)
        {
            if (factionEntity != Entity.Null &&
                !entityManager.HasBuffer<IntelligenceReportElement>(factionEntity))
            {
                entityManager.AddBuffer<IntelligenceReportElement>(factionEntity);
            }
        }

        private static bool TryResolveMemberEntity(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes memberId,
            out Entity memberEntity,
            out DynastyMemberComponent member)
        {
            memberEntity = Entity.Null;
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
                if (members[i].Member == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(members[i].Member))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(members[i].Member);
                if (!candidate.MemberId.Equals(memberId))
                {
                    continue;
                }

                memberEntity = members[i].Member;
                member = candidate;
                return true;
            }

            return false;
        }

        private static int CountCaptives(
            EntityManager entityManager,
            Entity factionEntity)
        {
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<CapturedMemberElement>(factionEntity))
            {
                return 0;
            }

            var captives = entityManager.GetBuffer<CapturedMemberElement>(factionEntity);
            int count = 0;
            for (int i = 0; i < captives.Length; i++)
            {
                if (captives[i].Status != CapturedMemberStatus.Released &&
                    captives[i].Status != CapturedMemberStatus.Executed)
                {
                    count++;
                }
            }

            return count;
        }

        private static FixedString512Bytes BuildMemberSummary(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            Entity factionEntity)
        {
            var summary = new FixedString512Bytes();
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return summary;
            }

            var members = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].Member == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(members[i].Member))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(members[i].Member);
                if (!PlayerCovertOpsSystem.IsAvailable(member.Status))
                {
                    continue;
                }

                if (summary.Length > 0)
                {
                    summary.Append(";");
                }

                summary.Append(member.MemberId);
                summary.Append(",");
                summary.Append(member.Title);
                summary.Append(",");
                summary.Append(ResolveRoleId(member.Role));
                summary.Append(",");
                summary.Append(ResolveStatusId(member.Status));
                summary.Append(",");
                summary.Append(((int)math.round(member.Renown)).ToString());
                summary.Append(",");
                summary.Append(ResolveLocationLabel(entityManager, factionId, member.Role));
                summary.Append(",");
                summary.Append(((int)math.round(ResolveExposureBonus(member.Role))).ToString());
            }

            return summary;
        }

        private static FixedString64Bytes ResolveLocationLabel(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            DynastyRole role)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return role == DynastyRole.Commander
                    ? new FixedString64Bytes("field-command")
                    : new FixedString64Bytes("rival-court");
            }

            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlements = query.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < settlements.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                if (entityManager.HasComponent<PrimaryKeepTag>(entities[i]))
                {
                    return settlements[i].SettlementId.Length > 0
                        ? settlements[i].SettlementId
                        : new FixedString64Bytes("rival-court");
                }
            }

            return role == DynastyRole.Commander
                ? new FixedString64Bytes("field-command")
                : new FixedString64Bytes("rival-court");
        }

        private static float ResolveExposureBonus(DynastyRole role)
        {
            return role switch
            {
                DynastyRole.Commander => 6f,
                DynastyRole.Governor => 4f,
                DynastyRole.Diplomat => 3f,
                DynastyRole.Merchant => 3f,
                DynastyRole.Spymaster => 2f,
                _ => 0f,
            };
        }

        private static string ResolveRoleId(DynastyRole role)
        {
            return role switch
            {
                DynastyRole.HeadOfBloodline => "head_of_bloodline",
                DynastyRole.HeirDesignate => "heir_designate",
                DynastyRole.Commander => "commander",
                DynastyRole.Governor => "governor",
                DynastyRole.Diplomat => "diplomat",
                DynastyRole.IdeologicalLeader => "ideological_leader",
                DynastyRole.Merchant => "merchant",
                DynastyRole.Spymaster => "spymaster",
                _ => "unknown",
            };
        }

        private static string ResolveStatusId(DynastyMemberStatus status)
        {
            return status switch
            {
                DynastyMemberStatus.Active => "active",
                DynastyMemberStatus.Ruling => "ruling",
                DynastyMemberStatus.Dormant => "dormant",
                DynastyMemberStatus.Fallen => "fallen",
                DynastyMemberStatus.Captured => "captured",
                _ => "unknown",
            };
        }

        private static FixedString64Bytes BuildDerivedId(
            string prefix,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays,
            int seed)
        {
            var id = new FixedString64Bytes(prefix);
            id.Append(sourceFactionId);
            if (targetFactionId.Length > 0)
            {
                id.Append("-");
                id.Append(targetFactionId);
            }

            id.Append("-");
            id.Append(((int)math.round(inWorldDays * 86400f)).ToString());
            id.Append("-");
            id.Append(seed.ToString());
            return id;
        }
    }
}
