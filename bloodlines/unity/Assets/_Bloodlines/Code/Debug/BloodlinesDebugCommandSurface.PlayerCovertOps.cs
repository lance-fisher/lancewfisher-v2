using System.Globalization;
using System.Text;
using Bloodlines.Components;
using Bloodlines.PlayerCovertOps;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugIssuePlayerEspionage(string sourceFactionId, string targetFactionId)
        {
            if (string.IsNullOrWhiteSpace(sourceFactionId) ||
                string.IsNullOrWhiteSpace(targetFactionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var sourceFactionKey = new FixedString32Bytes(sourceFactionId);
            var targetFactionKey = new FixedString32Bytes(targetFactionId);
            if (sourceFactionKey.Equals(targetFactionKey) ||
                FindFactionRootEntity(entityManager, sourceFactionKey) == Entity.Null ||
                FindFactionRootEntity(entityManager, targetFactionKey) == Entity.Null)
            {
                return false;
            }

            return TryQueuePlayerCovertRequest(entityManager, new PlayerCovertOpsRequestComponent
            {
                Kind = CovertOpKindPlayer.Espionage,
                Subtype = default,
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
                TargetMemberId = default,
                TargetEntityIndex = -1,
            });
        }

        public bool TryDebugIssuePlayerAssassination(
            string sourceFactionId,
            string targetFactionId,
            string targetMemberId)
        {
            if (string.IsNullOrWhiteSpace(sourceFactionId) ||
                string.IsNullOrWhiteSpace(targetFactionId) ||
                string.IsNullOrWhiteSpace(targetMemberId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var sourceFactionKey = new FixedString32Bytes(sourceFactionId);
            var targetFactionKey = new FixedString32Bytes(targetFactionId);
            var targetMemberKey = new FixedString64Bytes(targetMemberId);
            if (sourceFactionKey.Equals(targetFactionKey))
            {
                return false;
            }

            var sourceFactionEntity = FindFactionRootEntity(entityManager, sourceFactionKey);
            var targetFactionEntity = FindFactionRootEntity(entityManager, targetFactionKey);
            if (sourceFactionEntity == Entity.Null ||
                targetFactionEntity == Entity.Null ||
                !TargetFactionHasMember(entityManager, targetFactionEntity, targetMemberKey))
            {
                return false;
            }

            return TryQueuePlayerCovertRequest(entityManager, new PlayerCovertOpsRequestComponent
            {
                Kind = CovertOpKindPlayer.Assassination,
                Subtype = default,
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
                TargetMemberId = targetMemberKey,
                TargetEntityIndex = -1,
            });
        }

        public bool TryDebugIssuePlayerSabotage(
            string sourceFactionId,
            string sabotageSubtype,
            string targetFactionId,
            int targetBuildingEntityIndex)
        {
            if (string.IsNullOrWhiteSpace(sourceFactionId) ||
                string.IsNullOrWhiteSpace(sabotageSubtype) ||
                string.IsNullOrWhiteSpace(targetFactionId) ||
                targetBuildingEntityIndex < 0 ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var sourceFactionKey = new FixedString32Bytes(sourceFactionId);
            var targetFactionKey = new FixedString32Bytes(targetFactionId);
            if (sourceFactionKey.Equals(targetFactionKey) ||
                FindFactionRootEntity(entityManager, sourceFactionKey) == Entity.Null ||
                FindFactionRootEntity(entityManager, targetFactionKey) == Entity.Null ||
                !BuildingEntityExists(entityManager, targetBuildingEntityIndex))
            {
                return false;
            }

            return TryQueuePlayerCovertRequest(entityManager, new PlayerCovertOpsRequestComponent
            {
                Kind = CovertOpKindPlayer.Sabotage,
                Subtype = new FixedString32Bytes(sabotageSubtype),
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
                TargetMemberId = default,
                TargetEntityIndex = targetBuildingEntityIndex,
            });
        }

        public bool TryDebugIssuePlayerCounterIntelligence(string sourceFactionId)
        {
            if (string.IsNullOrWhiteSpace(sourceFactionId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var sourceFactionKey = new FixedString32Bytes(sourceFactionId);
            if (FindFactionRootEntity(entityManager, sourceFactionKey) == Entity.Null)
            {
                return false;
            }

            return TryQueuePlayerCovertRequest(entityManager, new PlayerCovertOpsRequestComponent
            {
                Kind = CovertOpKindPlayer.CounterIntelligence,
                Subtype = default,
                SourceFactionId = sourceFactionKey,
                TargetFactionId = default,
                TargetMemberId = default,
                TargetEntityIndex = -1,
            });
        }

        public bool TryDebugGetPlayerCovertOps(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionId);
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerCovertOpsResolutionComponent>());
            using var operationEntities = query.ToEntityArray(Allocator.Temp);
            using var operations = query.ToComponentDataArray<PlayerCovertOpsResolutionComponent>(Allocator.Temp);
            query.Dispose();

            var builder = new StringBuilder();
            int activeCount = 0;
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i].Active && operations[i].SourceFactionId.Equals(factionKey))
                {
                    activeCount++;
                }
            }

            builder.Append("ActivePlayerCovertOpCount=").Append(activeCount);
            for (int i = 0; i < operations.Length; i++)
            {
                var operation = operations[i];
                if (!operation.Active || !operation.SourceFactionId.Equals(factionKey))
                {
                    continue;
                }

                builder.AppendLine();
                builder.Append("PlayerCovertOp|EntityIndex=").Append(operationEntities[i].Index)
                    .Append("|Id=").Append(operation.OperationId)
                    .Append("|Kind=").Append(operation.Kind)
                    .Append("|Subtype=").Append(operation.Subtype)
                    .Append("|SourceFactionId=").Append(operation.SourceFactionId)
                    .Append("|TargetFactionId=").Append(operation.TargetFactionId)
                    .Append("|TargetMemberId=").Append(operation.TargetMemberId)
                    .Append("|TargetEntityIndex=").Append(operation.TargetEntityIndex)
                    .Append("|OperatorMemberId=").Append(operation.OperatorMemberId)
                    .Append("|OperatorTitle=").Append(operation.OperatorTitle)
                    .Append("|TargetLabel=").Append(operation.TargetLabel)
                    .Append("|LocationLabel=").Append(operation.LocationLabel)
                    .Append("|ProjectedChance=").Append(operation.ProjectedChance.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append("|SuccessScore=").Append(operation.SuccessScore.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|ResolveAt=").Append(operation.ResolveAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                    .Append("|ReportExpiresAt=").Append(operation.ReportExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                    .Append("|IntelSupport=").Append(operation.IntelSupport ? "true" : "false")
                    .Append("|IntelSupportBonus=").Append(operation.IntelSupportBonus.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|CounterIntelligenceActive=").Append(operation.CounterIntelligenceActive ? "true" : "false")
                    .Append("|CounterIntelligenceDefense=").Append(operation.CounterIntelligenceDefense.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|BloodlineGuardBonus=").Append(operation.BloodlineGuardBonus.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|WatchDuration=").Append(operation.WatchDurationInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                    .Append("|WatchStrength=").Append(operation.WatchStrength.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|WardLabel=").Append(operation.WardLabel)
                    .Append("|GuardedRoles=").Append(operation.GuardedRoles)
                    .Append("|AverageLoyalty=").Append(operation.AverageLoyalty.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|WeakestLoyalty=").Append(operation.WeakestLoyalty.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|EscrowGold=").Append(operation.EscrowGold.ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("|EscrowInfluence=").Append(operation.EscrowInfluence.ToString("0.##", CultureInfo.InvariantCulture));
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetPlayerCounterIntelligence(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionId);
            var factionEntity = FindFactionRootEntity(entityManager, factionKey);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            var builder = new StringBuilder();
            if (!entityManager.HasComponent<PlayerCounterIntelligenceComponent>(factionEntity))
            {
                builder.Append("CounterIntelligenceActive=false");
                readout = builder.ToString();
                return true;
            }

            var watch = entityManager.GetComponentData<PlayerCounterIntelligenceComponent>(factionEntity);
            builder.Append("CounterIntelligenceActive=true")
                .Append("|WatchId=").Append(watch.WatchId)
                .Append("|ActivatedAt=").Append(watch.ActivatedAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|ExpiresAt=").Append(watch.ExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|OperatorMemberId=").Append(watch.OperatorMemberId)
                .Append("|OperatorTitle=").Append(watch.OperatorTitle)
                .Append("|WatchStrength=").Append(watch.WatchStrength.ToString("0.00", CultureInfo.InvariantCulture))
                .Append("|WardLabel=").Append(watch.WardLabel)
                .Append("|GuardedRoles=").Append(watch.GuardedRoles)
                .Append("|AverageLoyalty=").Append(watch.AverageLoyalty.ToString("0.00", CultureInfo.InvariantCulture))
                .Append("|WeakestLoyalty=").Append(watch.WeakestLoyalty.ToString("0.00", CultureInfo.InvariantCulture))
                .Append("|Interceptions=").Append(watch.Interceptions)
                .Append("|FoiledEspionage=").Append(watch.FoiledEspionage)
                .Append("|FoiledAssassinations=").Append(watch.FoiledAssassinations)
                .Append("|LastInterceptAt=").Append(watch.LastInterceptAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|LastInterceptType=").Append(watch.LastInterceptType)
                .Append("|LastSourceFactionId=").Append(watch.LastSourceFactionId)
                .Append("|LastTargetMemberId=").Append(watch.LastTargetMemberId)
                .Append("|LastSourceInterceptions=").Append(watch.LastSourceInterceptions)
                .Append("|LastSourceFoiledEspionage=").Append(watch.LastSourceFoiledEspionage)
                .Append("|LastSourceFoiledAssassinations=").Append(watch.LastSourceFoiledAssassinations)
                .Append("|LastDossierAt=").Append(watch.LastDossierAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture));
            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetPlayerSabotageStatus(int buildingEntityIndex, out string readout)
        {
            readout = string.Empty;
            if (buildingEntityIndex < 0 || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (!TryFindBuildingEntity(entityManager, buildingEntityIndex, out var buildingEntity))
            {
                return false;
            }

            var builder = new StringBuilder();
            if (!entityManager.HasComponent<PlayerSabotageStatusComponent>(buildingEntity))
            {
                builder.Append("SabotageStatusActive=false");
                readout = builder.ToString();
                return true;
            }

            var status = entityManager.GetComponentData<PlayerSabotageStatusComponent>(buildingEntity);
            builder.Append("SabotageStatusActive=true")
                .Append("|Subtype=").Append(status.Subtype)
                .Append("|AppliedAt=").Append(status.AppliedAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|EffectExpiresAt=").Append(status.EffectExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|ProductionHaltUntil=").Append(status.ProductionHaltExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|GateExposureUntil=").Append(status.GateExposureExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|BurnExpiresAt=").Append(status.BurnExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                .Append("|BurnDamagePerSecond=").Append(status.BurnDamagePerSecond.ToString("0.00", CultureInfo.InvariantCulture))
                .Append("|DamageFloorRatio=").Append(status.DamageFloorRatio.ToString("0.00", CultureInfo.InvariantCulture));

            if (entityManager.HasComponent<HealthComponent>(buildingEntity))
            {
                var health = entityManager.GetComponentData<HealthComponent>(buildingEntity);
                builder.Append("|Health=").Append(health.Current.ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("/").Append(health.Max.ToString("0.##", CultureInfo.InvariantCulture));
            }

            if (entityManager.HasBuffer<ProductionQueueItemElement>(buildingEntity))
            {
                var queue = entityManager.GetBuffer<ProductionQueueItemElement>(buildingEntity);
                builder.Append("|QueueLength=").Append(queue.Length);
                if (queue.Length > 0)
                {
                    builder.Append("|QueueUnit=").Append(queue[0].UnitId)
                        .Append("|QueueRemaining=").Append(queue[0].RemainingSeconds.ToString("0.000", CultureInfo.InvariantCulture));
                }
            }
            else
            {
                builder.Append("|QueueLength=0");
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetIntelligenceReports(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionId);
            var factionEntity = FindFactionRootEntity(entityManager, factionKey);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            var builder = new StringBuilder();
            if (!entityManager.HasBuffer<IntelligenceReportElement>(factionEntity))
            {
                builder.Append("IntelligenceReportCount=0");
                readout = builder.ToString();
                return true;
            }

            var reports = entityManager.GetBuffer<IntelligenceReportElement>(factionEntity);
            builder.Append("IntelligenceReportCount=").Append(reports.Length);
            for (int i = 0; i < reports.Length; i++)
            {
                builder.AppendLine();
                builder.Append("IntelligenceReport|Index=").Append(i)
                    .Append("|Id=").Append(reports[i].ReportId)
                    .Append("|SourceFactionId=").Append(reports[i].SourceFactionId)
                    .Append("|TargetFactionId=").Append(reports[i].TargetFactionId)
                    .Append("|SourceType=").Append(reports[i].SourceType)
                    .Append("|ReportLabel=").Append(reports[i].ReportLabel)
                    .Append("|InterceptType=").Append(reports[i].InterceptType)
                    .Append("|InterceptCount=").Append(reports[i].InterceptCount)
                    .Append("|CreatedAt=").Append(reports[i].CreatedAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                    .Append("|ExpiresAt=").Append(reports[i].ExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                    .Append("|TargetLegitimacy=").Append(reports[i].TargetLegitimacy)
                    .Append("|TargetActiveOperations=").Append(reports[i].TargetActiveOperations)
                    .Append("|TargetCaptiveCount=").Append(reports[i].TargetCaptiveCount)
                    .Append("|TargetLesserHouseCount=").Append(reports[i].TargetLesserHouseCount)
                    .Append("|MemberSummary=").Append(reports[i].MemberSummary)
                    .Append("|BuildingSummary=").Append(reports[i].BuildingSummary)
                    .Append("|ResourceSummary=").Append(reports[i].ResourceSummary);
            }

            readout = builder.ToString();
            return true;
        }

        private static bool TryQueuePlayerCovertRequest(
            EntityManager entityManager,
            PlayerCovertOpsRequestComponent request)
        {
            var requestEntity = entityManager.CreateEntity(typeof(PlayerCovertOpsRequestComponent));
            entityManager.SetComponentData(requestEntity, request);
            return true;
        }

        private static Entity FindFactionRootEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId)
        {
            var fallback = FindFactionEntity(entityManager, factionId);
            if (fallback != Entity.Null &&
                (entityManager.HasComponent<FactionKindComponent>(fallback) ||
                 entityManager.HasComponent<ResourceStockpileComponent>(fallback) ||
                 entityManager.HasComponent<DynastyStateComponent>(fallback)))
            {
                return fallback;
            }

            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                if (entityManager.HasComponent<FactionKindComponent>(entities[i]) ||
                    entityManager.HasComponent<ResourceStockpileComponent>(entities[i]) ||
                    entityManager.HasComponent<DynastyStateComponent>(entities[i]))
                {
                    return entities[i];
                }
            }

            return fallback;
        }

        private static bool TargetFactionHasMember(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes memberId)
        {
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

                if (entityManager.GetComponentData<DynastyMemberComponent>(memberEntity).MemberId.Equals(memberId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool BuildingEntityExists(EntityManager entityManager, int entityIndex)
        {
            return TryFindBuildingEntity(entityManager, entityIndex, out _);
        }

        private static bool TryFindBuildingEntity(
            EntityManager entityManager,
            int entityIndex,
            out Entity buildingEntity)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].Index != entityIndex)
                {
                    continue;
                }

                buildingEntity = entities[i];
                return true;
            }

            buildingEntity = Entity.Null;
            return false;
        }
    }
}
