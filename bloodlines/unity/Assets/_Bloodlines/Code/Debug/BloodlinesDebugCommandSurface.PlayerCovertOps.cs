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

            var requestEntity = entityManager.CreateEntity(typeof(PlayerCovertOpsRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerCovertOpsRequestComponent
            {
                Kind = CovertOpKindPlayer.Espionage,
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
                TargetMemberId = default,
                TargetEntityIndex = -1,
            });
            return true;
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
                    .Append("|SourceFactionId=").Append(operation.SourceFactionId)
                    .Append("|TargetFactionId=").Append(operation.TargetFactionId)
                    .Append("|OperatorMemberId=").Append(operation.OperatorMemberId)
                    .Append("|OperatorTitle=").Append(operation.OperatorTitle)
                    .Append("|ProjectedChance=").Append(operation.ProjectedChance.ToString("0.000", CultureInfo.InvariantCulture))
                    .Append("|SuccessScore=").Append(operation.SuccessScore.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|ResolveAt=").Append(operation.ResolveAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                    .Append("|ReportExpiresAt=").Append(operation.ReportExpiresAtInWorldDays.ToString("0.000000", CultureInfo.InvariantCulture))
                    .Append("|EscrowGold=").Append(operation.EscrowGold.ToString("0.##", CultureInfo.InvariantCulture))
                    .Append("|EscrowInfluence=").Append(operation.EscrowInfluence.ToString("0.##", CultureInfo.InvariantCulture));
            }

            readout = builder.ToString();
            return true;
        }
    }
}
