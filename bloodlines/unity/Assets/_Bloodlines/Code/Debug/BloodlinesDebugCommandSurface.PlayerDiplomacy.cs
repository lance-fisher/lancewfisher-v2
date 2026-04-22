using System.Globalization;
using System.Text;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugIssuePlayerMarriageProposal(string sourceFactionId, string targetMemberId)
        {
            if (string.IsNullOrWhiteSpace(sourceFactionId) ||
                string.IsNullOrWhiteSpace(targetMemberId) ||
                !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var sourceFactionKey = new FixedString32Bytes(sourceFactionId);
            var sourceFactionEntity = FindFactionEntity(entityManager, sourceFactionKey);
            if (sourceFactionEntity == Entity.Null)
            {
                return false;
            }

            if (!TryFindMemberOwner(
                    entityManager,
                    new FixedString64Bytes(targetMemberId),
                    out var targetFactionId))
            {
                return false;
            }

            if (sourceFactionKey.Equals(targetFactionId))
            {
                return false;
            }

            var requestEntity = entityManager.CreateEntity(typeof(PlayerMarriageProposalRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerMarriageProposalRequestComponent
            {
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionId,
                TargetMemberId = new FixedString64Bytes(targetMemberId),
            });

            return true;
        }

        public bool TryDebugIssuePlayerMarriageAccept(int proposalEntityIndex)
        {
            if (proposalEntityIndex < 0 || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var requestEntity = entityManager.CreateEntity(typeof(PlayerMarriageAcceptRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerMarriageAcceptRequestComponent
            {
                ProposalEntityIndex = proposalEntityIndex,
            });
            return true;
        }

        public bool TryDebugIssuePlayerHolyWarDeclaration(string sourceFactionId, string targetFactionId)
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

            var requestEntity = entityManager.CreateEntity(typeof(PlayerHolyWarDeclarationRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerHolyWarDeclarationRequestComponent
            {
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
            });
            return true;
        }

        public bool TryDebugIssuePlayerDivineRightDeclaration(string sourceFactionId)
        {
            if (string.IsNullOrWhiteSpace(sourceFactionId) || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var sourceFactionKey = new FixedString32Bytes(sourceFactionId);
            if (FindFactionEntity(entityManager, sourceFactionKey) == Entity.Null)
            {
                return false;
            }

            var requestEntity = entityManager.CreateEntity(typeof(PlayerDivineRightDeclarationRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerDivineRightDeclarationRequestComponent
            {
                SourceFactionId = sourceFactionKey,
            });
            return true;
        }

        public bool TryDebugIssuePlayerMissionaryDispatch(string sourceFactionId, string targetFactionId)
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

            var requestEntity = entityManager.CreateEntity(typeof(PlayerMissionaryDispatchRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerMissionaryDispatchRequestComponent
            {
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
            });
            return true;
        }

        public bool TryDebugIssuePlayerPactProposal(string sourceFactionId, string targetFactionId)
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

            var requestEntity = entityManager.CreateEntity(typeof(PlayerPactProposalRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerPactProposalRequestComponent
            {
                SourceFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
            });
            return true;
        }

        public bool TryDebugIssuePlayerPactBreak(string sourceFactionId, string targetFactionId)
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

            var requestEntity = entityManager.CreateEntity(typeof(PlayerPactBreakRequestComponent));
            entityManager.SetComponentData(requestEntity, new PlayerPactBreakRequestComponent
            {
                RequestingFactionId = sourceFactionKey,
                TargetFactionId = targetFactionKey,
            });
            return true;
        }

        public bool TryDebugGetPlayerMarriageProposals(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionId);
            var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MarriageProposalComponent>());
            using var proposalEntities = query.ToEntityArray(Allocator.Temp);
            using var proposals = query.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            query.Dispose();

            var builder = new StringBuilder();
            int pendingCount = 0;
            for (int i = 0; i < proposals.Length; i++)
            {
                var proposal = proposals[i];
                if (proposal.Status != MarriageProposalStatus.Pending)
                {
                    continue;
                }

                if (!proposal.SourceFactionId.Equals(factionKey) &&
                    !proposal.TargetFactionId.Equals(factionKey))
                {
                    continue;
                }

                pendingCount++;
            }

            if (pendingCount == 0)
            {
                readout = "PendingProposalCount=0";
                return true;
            }

            builder.Append("PendingProposalCount=").Append(pendingCount);
            for (int i = 0; i < proposals.Length; i++)
            {
                var proposal = proposals[i];
                if (proposal.Status != MarriageProposalStatus.Pending)
                {
                    continue;
                }

                if (!proposal.SourceFactionId.Equals(factionKey) &&
                    !proposal.TargetFactionId.Equals(factionKey))
                {
                    continue;
                }

                builder.AppendLine();
                builder.Append("Proposal|EntityIndex=").Append(proposalEntities[i].Index)
                    .Append("|Id=").Append(proposal.ProposalId)
                    .Append("|SourceFactionId=").Append(proposal.SourceFactionId)
                    .Append("|SourceMemberId=").Append(proposal.SourceMemberId)
                    .Append("|TargetFactionId=").Append(proposal.TargetFactionId)
                    .Append("|TargetMemberId=").Append(proposal.TargetMemberId)
                    .Append("|Status=").Append(proposal.Status)
                    .Append("|ProposedAt=").Append(proposal.ProposedAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|ExpiresAt=").Append(proposal.ExpiresAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture));
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetPlayerFaithDeclarationOperations(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionId);
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var operations = query.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);

            var builder = new StringBuilder();
            int count = 0;
            for (int i = 0; i < operations.Length; i++)
            {
                if (!operations[i].Active ||
                    !operations[i].SourceFactionId.Equals(factionKey) ||
                    (operations[i].OperationKind != DynastyOperationKind.HolyWar &&
                     operations[i].OperationKind != DynastyOperationKind.DivineRight &&
                     operations[i].OperationKind != DynastyOperationKind.Missionary))
                {
                    continue;
                }

                count++;
            }

            builder.Append("FaithDeclarationCount=").Append(count);
            for (int i = 0; i < operations.Length; i++)
            {
                if (!operations[i].Active ||
                    !operations[i].SourceFactionId.Equals(factionKey) ||
                    (operations[i].OperationKind != DynastyOperationKind.HolyWar &&
                     operations[i].OperationKind != DynastyOperationKind.DivineRight &&
                     operations[i].OperationKind != DynastyOperationKind.Missionary))
                {
                    continue;
                }

                builder.AppendLine();
                builder.Append("FaithDeclaration|EntityIndex=").Append(entities[i].Index)
                    .Append("|Kind=").Append(operations[i].OperationKind)
                    .Append("|TargetFactionId=").Append(operations[i].TargetFactionId)
                    .Append("|StartedAt=").Append(operations[i].StartedAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|Active=").Append(operations[i].Active);

                if (operations[i].OperationKind == DynastyOperationKind.HolyWar &&
                    entityManager.HasComponent<DynastyOperationHolyWarComponent>(entities[i]))
                {
                    var holyWar = entityManager.GetComponentData<DynastyOperationHolyWarComponent>(entities[i]);
                    builder.Append("|ResolveAt=").Append(holyWar.ResolveAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                        .Append("|WarExpiresAt=").Append(holyWar.WarExpiresAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                        .Append("|SourceFaithOperator=").Append(holyWar.OperatorMemberId)
                        .Append("|CompatibilityLabel=").Append(holyWar.CompatibilityLabel);
                }

                if (operations[i].OperationKind == DynastyOperationKind.DivineRight &&
                    entityManager.HasComponent<DynastyOperationDivineRightComponent>(entities[i]))
                {
                    var divineRight = entityManager.GetComponentData<DynastyOperationDivineRightComponent>(entities[i]);
                    builder.Append("|ResolveAt=").Append(divineRight.ResolveAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                        .Append("|SourceFaithId=").Append(divineRight.SourceFaithId)
                        .Append("|DoctrinePath=").Append(divineRight.DoctrinePath);
                }

                if (operations[i].OperationKind == DynastyOperationKind.Missionary &&
                    entityManager.HasComponent<DynastyOperationMissionaryComponent>(entities[i]))
                {
                    var missionary = entityManager.GetComponentData<DynastyOperationMissionaryComponent>(entities[i]);
                    builder.Append("|ResolveAt=").Append(missionary.ResolveAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                        .Append("|SourceFaithId=").Append(missionary.SourceFaithId)
                        .Append("|ExposureGain=").Append(missionary.ExposureGain.ToString("0.00", CultureInfo.InvariantCulture))
                        .Append("|IntensityErosion=").Append(missionary.IntensityErosion.ToString("0.00", CultureInfo.InvariantCulture))
                        .Append("|LoyaltyPressure=").Append(missionary.LoyaltyPressure.ToString("0.00", CultureInfo.InvariantCulture));
                }
            }

            readout = builder.ToString();
            return true;
        }

        public bool TryDebugGetPlayerPacts(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId) || !TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            var factionKey = new FixedString32Bytes(factionId);
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var pacts = query.ToComponentDataArray<PactComponent>(Allocator.Temp);

            var builder = new StringBuilder();
            int count = 0;
            for (int i = 0; i < pacts.Length; i++)
            {
                if (pacts[i].FactionAId.Equals(factionKey) || pacts[i].FactionBId.Equals(factionKey))
                {
                    count++;
                }
            }

            builder.Append("PactCount=").Append(count);
            for (int i = 0; i < pacts.Length; i++)
            {
                if (!pacts[i].FactionAId.Equals(factionKey) && !pacts[i].FactionBId.Equals(factionKey))
                {
                    continue;
                }

                builder.AppendLine();
                builder.Append("Pact|EntityIndex=").Append(entities[i].Index)
                    .Append("|PactId=").Append(pacts[i].PactId)
                    .Append("|FactionAId=").Append(pacts[i].FactionAId)
                    .Append("|FactionBId=").Append(pacts[i].FactionBId)
                    .Append("|StartedAt=").Append(pacts[i].StartedAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|MinimumExpiresAt=").Append(pacts[i].MinimumExpiresAtInWorldDays.ToString("0.00", CultureInfo.InvariantCulture))
                    .Append("|Broken=").Append(pacts[i].Broken)
                    .Append("|BrokenByFactionId=").Append(pacts[i].BrokenByFactionId);
            }

            readout = builder.ToString();
            return true;
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

        private static bool TryFindMemberOwner(
            EntityManager entityManager,
            FixedString64Bytes memberId,
            out FixedString32Bytes factionId)
        {
            factionId = default;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyMemberComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using var members = query.ToComponentDataArray<DynastyMemberComponent>(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();
            for (int i = 0; i < members.Length; i++)
            {
                if (!members[i].MemberId.Equals(memberId))
                {
                    continue;
                }

                factionId = factions[i].FactionId;
                return true;
            }

            return false;
        }
    }
}
