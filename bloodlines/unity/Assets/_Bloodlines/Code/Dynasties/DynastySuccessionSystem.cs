using Bloodlines.Components;
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
            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadWrite<DynastyStateComponent>(),
                ComponentType.ReadOnly<DynastyMemberRef>());

            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            for (int f = 0; f < factionEntities.Length; f++)
            {
                var factionEntity = factionEntities[f];
                var dynasty = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
                var memberBuffer = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);

                Entity rulingEntity = Entity.Null;
                Entity heirEntity = Entity.Null;
                int heirOrder = int.MaxValue;

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

                if (rulingEntity == Entity.Null)
                {
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
            }
        }
    }
}
