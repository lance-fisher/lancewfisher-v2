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
                FindFactionEntity(entityManager, sourceFactionKey) == Entity.Null ||
                FindFactionEntity(entityManager, targetFactionKey) == Entity.Null)
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

            var sourceFactionEntity = FindFactionEntity(entityManager, sourceFactionKey);
            var targetFactionEntity = FindFactionEntity(entityManager, targetFactionKey);
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
                FindFactionEntity(entityManager, sourceFactionKey) == Entity.Null ||
                FindFactionEntity(entityManager, targetFactionKey) == Entity.Null ||
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
                    .Append("|EscrowGold=").Append(operation.EscrowGold.ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("|EscrowInfluence=").Append(operation.EscrowInfluence.ToString("0.##", CultureInfo.InvariantCulture));
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
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i].Index == entityIndex)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
