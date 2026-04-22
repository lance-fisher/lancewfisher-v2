using System.Globalization;
using System.Text;
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
