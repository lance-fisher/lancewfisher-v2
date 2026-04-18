using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Processes pending RenownAwardRequest components, routes the award to the
    /// best available dynasty member for the target faction, and marks the request
    /// consumed. Priority: Commander > HeadOfBloodline > MilitaryCommand path > any.
    /// Browser reference: findRenownAwardRecipient (simulation.js ~3062),
    /// awardRenownToFaction (simulation.js ~3080).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct RenownAwardSystem : ISystem
    {
        private const float RenownCap = 100f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RenownAwardRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var reqQuery = em.CreateEntityQuery(typeof(RenownAwardRequestComponent));
            var reqEntities = reqQuery.ToEntityArray(Allocator.Temp);
            var reqs        = reqQuery.ToComponentDataArray<RenownAwardRequestComponent>(Allocator.Temp);
            reqQuery.Dispose();

            for (int i = 0; i < reqEntities.Length; i++)
            {
                if (reqs[i].Consumed)
                    continue;

                var factionKey = reqs[i].FactionId;
                float amount   = reqs[i].Amount;

                // Find the faction entity for the DynastyMemberRef buffer.
                var factionQuery = em.CreateEntityQuery(
                    ComponentType.ReadOnly<FactionComponent>(),
                    ComponentType.ReadOnly<DynastyStateComponent>());
                var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
                var factionComps    = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
                factionQuery.Dispose();

                Entity factionEntity = Entity.Null;
                for (int j = 0; j < factionEntities.Length; j++)
                {
                    if (factionComps[j].FactionId.Equals(factionKey))
                    {
                        factionEntity = factionEntities[j];
                        break;
                    }
                }

                factionEntities.Dispose();
                factionComps.Dispose();

                if (factionEntity == Entity.Null)
                    continue;

                Entity recipient = FindRecipient(em, factionEntity);
                if (recipient == Entity.Null)
                    continue;

                var member = em.GetComponentData<DynastyMemberComponent>(recipient);
                member.Renown = System.Math.Min(RenownCap, member.Renown + amount);
                em.SetComponentData(recipient, member);

                var req = reqs[i];
                req.Consumed = true;
                em.SetComponentData(reqEntities[i], req);
            }

            reqEntities.Dispose();
            reqs.Dispose();
        }

        private static Entity FindRecipient(EntityManager em, Entity factionEntity)
        {
            if (!em.HasBuffer<DynastyMemberRef>(factionEntity))
                return Entity.Null;

            var refs = em.GetBuffer<DynastyMemberRef>(factionEntity, true);

            Entity commander        = Entity.Null;
            Entity headOfBloodline  = Entity.Null;
            Entity militaryPath     = Entity.Null;
            Entity anyAvailable     = Entity.Null;

            for (int i = 0; i < refs.Length; i++)
            {
                var e = refs[i].Member;
                if (!em.HasComponent<DynastyMemberComponent>(e))
                    continue;

                var m = em.GetComponentData<DynastyMemberComponent>(e);
                bool available = m.Status == DynastyMemberStatus.Active ||
                                 m.Status == DynastyMemberStatus.Ruling;
                if (!available)
                    continue;

                if (anyAvailable == Entity.Null)
                    anyAvailable = e;

                if (m.Role == DynastyRole.Commander && commander == Entity.Null)
                    commander = e;
                if (m.Role == DynastyRole.HeadOfBloodline && headOfBloodline == Entity.Null)
                    headOfBloodline = e;
                if (m.Path == DynastyPath.MilitaryCommand && militaryPath == Entity.Null)
                    militaryPath = e;
            }

            if (commander       != Entity.Null) return commander;
            if (headOfBloodline != Entity.Null) return headOfBloodline;
            if (militaryPath    != Entity.Null) return militaryPath;
            return anyAvailable;
        }
    }
}
