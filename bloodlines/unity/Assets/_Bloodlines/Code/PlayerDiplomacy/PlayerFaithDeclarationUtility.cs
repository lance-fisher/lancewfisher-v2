using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.PlayerDiplomacy
{
    internal static class PlayerFaithDeclarationUtility
    {
        internal static Entity FindFactionEntity(EntityManager entityManager, FixedString32Bytes factionId)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            Entity fallback = Entity.Null;
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

                if (fallback == Entity.Null)
                {
                    fallback = entities[i];
                }
            }

            return fallback;
        }

        internal static bool IsKingdom(EntityManager entityManager, Entity factionEntity)
        {
            return factionEntity != Entity.Null &&
                   entityManager.HasComponent<FactionKindComponent>(factionEntity) &&
                   entityManager.GetComponentData<FactionKindComponent>(factionEntity).Kind == FactionKind.Kingdom;
        }

        internal static float GetInWorldDays(EntityManager entityManager)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            return query.IsEmpty ? 0f : query.GetSingleton<DualClockComponent>().InWorldDays;
        }

        internal static bool TryGetFaithOperator(
            EntityManager entityManager,
            Entity factionEntity,
            out FixedString64Bytes operatorMemberId,
            out FixedString64Bytes operatorTitle)
        {
            return TryGetFaithOperator(
                entityManager,
                factionEntity,
                out operatorMemberId,
                out operatorTitle,
                out _);
        }

        internal static bool TryGetFaithOperator(
            EntityManager entityManager,
            Entity factionEntity,
            out FixedString64Bytes operatorMemberId,
            out FixedString64Bytes operatorTitle,
            out float operatorRenown)
        {
            operatorMemberId = default;
            operatorTitle = default;
            operatorRenown = 0f;

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

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (!IsFaithOperatorRole(member.Role) ||
                    member.Status == DynastyMemberStatus.Fallen ||
                    member.Status == DynastyMemberStatus.Captured)
                {
                    continue;
                }

                operatorMemberId = member.MemberId;
                operatorTitle = member.Title;
                operatorRenown = member.Renown;
                return true;
            }

            return false;
        }

        internal static bool HasActiveOperation(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            DynastyOperationKind kind)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using var operations = query.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i].Active &&
                    operations[i].OperationKind == kind &&
                    operations[i].SourceFactionId.Equals(sourceFactionId))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool HasActiveOperationForTarget(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            DynastyOperationKind kind,
            FixedString32Bytes targetFactionId)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DynastyOperationComponent>());
            using var operations = query.ToComponentDataArray<DynastyOperationComponent>(Allocator.Temp);
            for (int i = 0; i < operations.Length; i++)
            {
                if (operations[i].Active &&
                    operations[i].OperationKind == kind &&
                    operations[i].SourceFactionId.Equals(sourceFactionId) &&
                    operations[i].TargetFactionId.Equals(targetFactionId))
                {
                    return true;
                }
            }

            return false;
        }

        internal static FixedString64Bytes SelectedFaithIdString(CovenantId selected)
        {
            switch (selected)
            {
                case CovenantId.OldLight: return new FixedString64Bytes("old_light");
                case CovenantId.BloodDominion: return new FixedString64Bytes("blood_dominion");
                case CovenantId.TheOrder: return new FixedString64Bytes("the_order");
                case CovenantId.TheWild: return new FixedString64Bytes("the_wild");
                default: return new FixedString64Bytes("none");
            }
        }

        internal static FixedString64Bytes DeriveHolyWarCompatibilityLabel(
            in FaithStateComponent sourceFaith,
            in FaithStateComponent targetFaith)
        {
            return sourceFaith.SelectedFaith == targetFaith.SelectedFaith
                ? new FixedString64Bytes("fractured")
                : new FixedString64Bytes("discordant");
        }

        internal static void PushHolyWarNarrative(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString64Bytes)" sends a holy war declaration toward ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");

            NarrativeMessageTone tone =
                sourceFactionId.Equals(new FixedString32Bytes("player")) ||
                targetFactionId.Equals(new FixedString32Bytes("player"))
                    ? NarrativeMessageTone.Warn
                    : NarrativeMessageTone.Info;
            NarrativeMessageBridge.Push(entityManager, message, tone);
        }

        internal static void PushDivineRightNarrative(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString64Bytes sourceFaithId,
            int durationInWorldDays)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" declares Divine Right under ");
            message.Append(sourceFaithId);
            message.Append((FixedString64Bytes)". The spread window opens for ");
            message.Append(durationInWorldDays);
            message.Append((FixedString32Bytes)" in-world days.");
            NarrativeMessageBridge.Push(entityManager, message, NarrativeMessageTone.Warn);
        }

        internal static void PushMissionaryNarrative(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            FixedString64Bytes sourceFaithId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" dispatches missionaries of ");
            message.Append(sourceFaithId);
            message.Append((FixedString32Bytes)" toward ");
            message.Append(targetFactionId);
            message.Append((FixedString32Bytes)".");
            NarrativeMessageBridge.Push(entityManager, message, NarrativeMessageTone.Info);
        }

        private static bool IsFaithOperatorRole(DynastyRole role)
        {
            return role == DynastyRole.IdeologicalLeader ||
                   role == DynastyRole.Spymaster ||
                   role == DynastyRole.HeadOfBloodline ||
                   role == DynastyRole.Diplomat;
        }
    }
}
