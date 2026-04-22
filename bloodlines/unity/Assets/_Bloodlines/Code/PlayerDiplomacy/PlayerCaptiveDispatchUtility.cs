using Bloodlines.AI;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    internal static class PlayerCaptiveDispatchUtility
    {
        internal static bool TryGetCapturedMember(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString64Bytes captiveMemberId,
            out DynastyMemberComponent member)
        {
            member = default;
            if (factionEntity == Entity.Null || !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                var memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (!candidate.MemberId.Equals(captiveMemberId) ||
                    candidate.Status != DynastyMemberStatus.Captured)
                {
                    continue;
                }

                member = candidate;
                return true;
            }

            return false;
        }

        internal static bool TryFindHeldCaptive(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes captiveMemberId,
            out FixedString64Bytes captiveMemberTitle,
            out FixedString32Bytes captorFactionId)
        {
            captiveMemberTitle = default;
            captorFactionId = default;

            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<CapturedMemberElement>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                var captives = entityManager.GetBuffer<CapturedMemberElement>(entities[i]);
                for (int j = 0; j < captives.Length; j++)
                {
                    var captive = captives[j];
                    if (captive.Status != CapturedMemberStatus.Held ||
                        !captive.OriginFactionId.Equals(sourceFactionId) ||
                        !captive.MemberId.Equals(captiveMemberId))
                    {
                        continue;
                    }

                    captiveMemberTitle = captive.MemberTitle;
                    captorFactionId = factions[i].FactionId;
                    return true;
                }
            }

            return false;
        }

        internal static FixedString64Bytes BuildRescueOperationId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes captiveMemberId,
            float inWorldDays,
            int requestIndex)
        {
            var id = new FixedString64Bytes("player-rescue-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(targetFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            id.Append("-");
            id.Append(requestIndex);
            return id;
        }

        internal static void PushRescueNarrative(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes captiveMemberTitle,
            FixedString32Bytes captorFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" dispatches covert agents to recover ");
            message.Append(captiveMemberTitle);
            message.Append((FixedString32Bytes)" from ");
            message.Append(captorFactionId);
            message.Append((FixedString32Bytes)".");
            NarrativeMessageBridge.Push(
                entityManager,
                message,
                sourceFactionId.Equals(new FixedString32Bytes("player"))
                    ? NarrativeMessageTone.Info
                    : NarrativeMessageTone.Good);
        }
    }
}
