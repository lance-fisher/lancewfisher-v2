using Bloodlines.Components;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Promotes the eldest heir to ruling when the current ruler is fallen,
    /// enters interregnum if no heir exists, and updates the dynasty state on
    /// the faction entity accordingly.
    ///
    /// Browser reference: backfillHeir and the surrounding succession helpers
    /// in simulation.js; specifically the invariant that a faction is either
    /// ruled or in interregnum, and that the ruling seat moves through the
    /// ordered active-member chain.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DynastyAgingSystem))]
    public partial struct DynastySuccessionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var factionQuery = SystemAPI.QueryBuilder()
                .WithAllRW<DynastyStateComponent>()
                .WithAll<DynastyMemberRef>()
                .Build();

            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int f = 0; f < factionEntities.Length; f++)
            {
                var factionEntity = factionEntities[f];
                var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
                var memberBuffer = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);

                Entity rulingEntity = Entity.Null;
                Entity preferredHeirEntity = Entity.Null;
                Entity heirEntity = Entity.Null;
                int heirOrder = int.MaxValue;
                bool clearPreferenceAfterSuccession = false;

                for (int i = 0; i < memberBuffer.Length; i++)
                {
                    var memberEntity = memberBuffer[i].Member;
                    if (!entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                    {
                        continue;
                    }

                    var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                    if (member.Status == DynastyMemberStatus.Ruling)
                    {
                        rulingEntity = memberEntity;
                    }
                    else if (member.Status == DynastyMemberStatus.Active && member.Order < heirOrder)
                    {
                        heirEntity = memberEntity;
                        heirOrder = member.Order;
                    }
                }

                if (entityManager.HasComponent<SuccessionPreferenceComponent>(factionEntity))
                {
                    var preference = entityManager.GetComponentData<SuccessionPreferenceComponent>(factionEntity);
                    if (preference.PreferredHeirEntity != Entity.Null &&
                        entityManager.Exists(preference.PreferredHeirEntity) &&
                        entityManager.HasComponent<DynastyMemberComponent>(preference.PreferredHeirEntity) &&
                        SuccessionPreferenceResolutionSystem.FactionContainsMember(
                            entityManager,
                            factionEntity,
                            preference.PreferredHeirEntity))
                    {
                        var preferredMember =
                            entityManager.GetComponentData<DynastyMemberComponent>(preference.PreferredHeirEntity);
                        if (preference.PreferredHeirMemberId.Equals(preferredMember.MemberId) &&
                            SuccessionPreferenceResolutionSystem.IsEligibleSuccessor(preferredMember))
                        {
                            preferredHeirEntity = preference.PreferredHeirEntity;
                        }
                    }

                    if (rulingEntity == Entity.Null || preferredHeirEntity == Entity.Null)
                    {
                        clearPreferenceAfterSuccession = true;
                    }
                }

                if (rulingEntity == Entity.Null)
                {
                    if (preferredHeirEntity != Entity.Null)
                    {
                        heirEntity = preferredHeirEntity;
                    }

                    if (heirEntity != Entity.Null)
                    {
                        var heir = entityManager.GetComponentData<DynastyMemberComponent>(heirEntity);
                        heir.Status = DynastyMemberStatus.Ruling;
                        heir.Role = DynastyRole.HeadOfBloodline;
                        entityManager.SetComponentData(heirEntity, heir);
                        dynasty.Interregnum = false;
                    }
                    else
                    {
                        dynasty.Interregnum = true;
                    }
                }
                else
                {
                    dynasty.Interregnum = false;
                }

                entityManager.SetComponentData(factionEntity, dynasty);
                if (clearPreferenceAfterSuccession &&
                    entityManager.HasComponent<SuccessionPreferenceComponent>(factionEntity))
                {
                    ecb.RemoveComponent<SuccessionPreferenceComponent>(factionEntity);
                }
            }

            ecb.Playback(entityManager);
        }
    }
}
