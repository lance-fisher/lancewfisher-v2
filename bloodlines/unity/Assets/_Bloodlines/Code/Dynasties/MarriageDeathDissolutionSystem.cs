using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Dissolves live marriages when either spouse has fallen. Browser
    /// reference: simulation.js dissolveMarriageFromDeath (~6382) and
    /// tickMarriageDissolutionFromDeath (~7471).
    /// Canonical death-dissolution effects:
    /// - legitimacy -2 on each affected dynasty
    /// - oathkeeping +1 on each affected conviction ledger
    /// - dissolvedAtInWorldDays recorded on both mirror records
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(MarriageGestationSystem))]
    public partial struct MarriageDeathDissolutionSystem : ISystem
    {
        public const float LegitimacyLoss = 2f;
        public const float OathkeepingGain = 1f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MarriageComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);

            var query = em.CreateEntityQuery(typeof(MarriageComponent));
            var entities = query.ToEntityArray(Allocator.Temp);
            var marriages = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var marriage = marriages[i];
                if (!marriage.IsPrimary || marriage.Dissolved)
                {
                    continue;
                }

                bool headFallen = TryGetMember(
                    em,
                    marriage.HeadFactionId,
                    marriage.HeadMemberId,
                    out var headMember)
                    && headMember.Status == DynastyMemberStatus.Fallen;
                bool spouseFallen = TryGetMember(
                    em,
                    marriage.SpouseFactionId,
                    marriage.SpouseMemberId,
                    out var spouseMember)
                    && spouseMember.Status == DynastyMemberStatus.Fallen;

                if (!headFallen && !spouseFallen)
                {
                    continue;
                }

                MarkMarriageRecordsDissolved(
                    em,
                    marriages,
                    entities,
                    marriage.MarriageId,
                    inWorldDays);

                ApplyFactionEffects(em, marriage.HeadFactionId);
                if (!marriage.SpouseFactionId.Equals(marriage.HeadFactionId))
                {
                    ApplyFactionEffects(em, marriage.SpouseFactionId);
                }
            }

            entities.Dispose();
            marriages.Dispose();
        }

        private static void MarkMarriageRecordsDissolved(
            EntityManager em,
            NativeArray<MarriageComponent> marriages,
            NativeArray<Entity> entities,
            FixedString64Bytes marriageId,
            float inWorldDays)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                if (!marriages[i].MarriageId.Equals(marriageId))
                {
                    continue;
                }

                var marriage = marriages[i];
                if (marriage.Dissolved)
                {
                    continue;
                }

                marriage.Dissolved = true;
                marriage.DissolvedAtInWorldDays = inWorldDays;
                marriages[i] = marriage;
                em.SetComponentData(entities[i], marriage);
            }
        }

        private static void ApplyFactionEffects(
            EntityManager em,
            FixedString32Bytes factionId)
        {
            var factionEntity = FindFactionEntity(em, factionId);
            if (factionEntity == Entity.Null)
            {
                return;
            }

            if (em.HasComponent<DynastyStateComponent>(factionEntity))
            {
                var dynasty = em.GetComponentData<DynastyStateComponent>(factionEntity);
                dynasty.Legitimacy = math.clamp(dynasty.Legitimacy - LegitimacyLoss, 0f, 100f);
                em.SetComponentData(factionEntity, dynasty);
            }

            if (em.HasComponent<ConvictionComponent>(factionEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(factionEntity);
                ConvictionScoring.ApplyEvent(
                    ref conviction,
                    ConvictionBucket.Oathkeeping,
                    OathkeepingGain);
                em.SetComponentData(factionEntity, conviction);
            }
        }

        private static bool TryGetMember(
            EntityManager em,
            FixedString32Bytes factionId,
            FixedString64Bytes memberId,
            out DynastyMemberComponent member)
        {
            member = default;
            var factionEntity = FindFactionEntity(em, factionId);
            if (factionEntity == Entity.Null || !em.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return false;
            }

            var members = em.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null || !em.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var candidate = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (candidate.MemberId.Equals(memberId))
                {
                    member = candidate;
                    return true;
                }
            }

            return false;
        }

        private static Entity FindFactionEntity(
            EntityManager em,
            FixedString32Bytes factionId)
        {
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
            var entities = query.ToEntityArray(Allocator.Temp);
            var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            Entity match = Entity.Null;
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(factionId))
                {
                    match = entities[i];
                    break;
                }
            }

            entities.Dispose();
            factions.Dispose();
            return match;
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (query.IsEmpty)
            {
                query.Dispose();
                return 0f;
            }

            float inWorldDays = query.GetSingleton<DualClockComponent>().InWorldDays;
            query.Dispose();
            return inWorldDays;
        }
    }
}
