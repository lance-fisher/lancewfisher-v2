using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.PlayerDiplomacy
{
    public static class PlayerPactUtility
    {
        public const float InfluenceCost = 50f;
        public const float GoldCost = 80f;
        public const float MinimumDurationInWorldDays = 180f;
        public const float LegitimacyCost = 8f;
        public const float OathkeepingPenalty = 2f;

        public static bool IsHostile(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes targetFactionId)
        {
            if (!entityManager.HasBuffer<HostilityComponent>(factionEntity))
            {
                return false;
            }

            var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = 0; i < hostilityBuffer.Length; i++)
            {
                if (hostilityBuffer[i].HostileFactionId.Equals(targetFactionId))
                {
                    return true;
                }
            }

            return false;
        }

        public static void DropHostility(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes targetFactionId)
        {
            if (!entityManager.HasBuffer<HostilityComponent>(factionEntity))
            {
                return;
            }

            var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(factionEntity);
            for (int i = hostilityBuffer.Length - 1; i >= 0; i--)
            {
                if (hostilityBuffer[i].HostileFactionId.Equals(targetFactionId))
                {
                    hostilityBuffer.RemoveAt(i);
                }
            }
        }

        public static void EnsureHostility(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var sourceEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, sourceFactionId);
            if (sourceEntity == Entity.Null)
            {
                return;
            }

            if (!entityManager.HasBuffer<HostilityComponent>(sourceEntity))
            {
                entityManager.AddBuffer<HostilityComponent>(sourceEntity);
            }

            var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(sourceEntity);
            for (int i = 0; i < hostilityBuffer.Length; i++)
            {
                if (hostilityBuffer[i].HostileFactionId.Equals(targetFactionId))
                {
                    return;
                }
            }

            hostilityBuffer.Add(new HostilityComponent
            {
                HostileFactionId = targetFactionId,
            });
        }

        public static bool TryFindActivePact(
            EntityManager entityManager,
            FixedString32Bytes factionAId,
            FixedString32Bytes factionBId,
            out Entity pactEntity,
            out PactComponent pact)
        {
            pactEntity = Entity.Null;
            pact = default;

            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<PactComponent>());
            if (query.IsEmpty)
            {
                return false;
            }

            using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
            using var pacts = query.ToComponentDataArray<PactComponent>(Unity.Collections.Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (pacts[i].Broken)
                {
                    continue;
                }

                bool forward = pacts[i].FactionAId.Equals(factionAId) && pacts[i].FactionBId.Equals(factionBId);
                bool reverse = pacts[i].FactionAId.Equals(factionBId) && pacts[i].FactionBId.Equals(factionAId);
                if (!forward && !reverse)
                {
                    continue;
                }

                pactEntity = entities[i];
                pact = pacts[i];
                return true;
            }

            return false;
        }

        public static FixedString64Bytes BuildPactId(
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId,
            float inWorldDays)
        {
            var id = new FixedString64Bytes("nap-");
            id.Append(sourceFactionId);
            id.Append("-");
            id.Append(targetFactionId);
            id.Append("-d");
            id.Append((int)inWorldDays);
            return id;
        }

        public static void ApplyBreakPenalties(
            EntityManager entityManager,
            FixedString32Bytes requestingFactionId)
        {
            var breakerEntity = PlayerFaithDeclarationUtility.FindFactionEntity(entityManager, requestingFactionId);
            if (breakerEntity == Entity.Null)
            {
                return;
            }

            if (entityManager.HasComponent<DynastyStateComponent>(breakerEntity))
            {
                var dynastyState = entityManager.GetComponentData<DynastyStateComponent>(breakerEntity);
                dynastyState.Legitimacy = math.clamp(dynastyState.Legitimacy - LegitimacyCost, 0f, 100f);
                entityManager.SetComponentData(breakerEntity, dynastyState);
            }

            if (entityManager.HasComponent<ConvictionComponent>(breakerEntity))
            {
                var conviction = entityManager.GetComponentData<ConvictionComponent>(breakerEntity);
                ConvictionScoring.ApplyEvent(ref conviction, ConvictionBucket.Oathkeeping, -OathkeepingPenalty);
                entityManager.SetComponentData(breakerEntity, conviction);
            }
        }

        public static void PushPactEnteredNarrative(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            var message = new FixedString128Bytes();
            message.Append(sourceFactionId);
            message.Append((FixedString32Bytes)" and ");
            message.Append(targetFactionId);
            message.Append((FixedString64Bytes)" enter a non-aggression pact. Hostility ceases for at least ");
            message.Append((int)MinimumDurationInWorldDays);
            message.Append((FixedString32Bytes)" in-world days.");

            var tone = sourceFactionId.Equals(new FixedString32Bytes("player"))
                ? NarrativeMessageTone.Good
                : NarrativeMessageTone.Info;
            NarrativeMessageBridge.Push(entityManager, message, tone);
        }

        public static void PushPactBrokenNarrative(
            EntityManager entityManager,
            FixedString32Bytes requestingFactionId,
            FixedString32Bytes targetFactionId,
            float minimumExpiresAtInWorldDays)
        {
            float currentInWorldDays = PlayerFaithDeclarationUtility.GetInWorldDays(entityManager);
            bool earlyBreak = currentInWorldDays < minimumExpiresAtInWorldDays;

            var message = new FixedString128Bytes();
            message.Append(requestingFactionId);
            message.Append((FixedString64Bytes)" breaks the non-aggression pact with ");
            message.Append(targetFactionId);
            message.Append(earlyBreak
                ? (FixedString64Bytes)". The early breach damages legitimacy and conviction."
                : (FixedString64Bytes)". Hostility resumes.");

            var tone = requestingFactionId.Equals(new FixedString32Bytes("player"))
                ? NarrativeMessageTone.Warn
                : NarrativeMessageTone.Info;
            NarrativeMessageBridge.Push(entityManager, message, tone);
        }
    }
}
