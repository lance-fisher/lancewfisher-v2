using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Expires pending marriage proposals whose in-world age exceeds the
    /// canonical expiration window. Browser reference: simulation.js
    /// tickMarriageProposalExpiration (~7274).
    /// MARRIAGE_PROPOSAL_EXPIRATION_IN_WORLD_DAYS = 30.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct MarriageProposalExpirationSystem : ISystem
    {
        public const float ExpirationInWorldDays = 30f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MarriageProposalComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            float inWorldDays = GetInWorldDays(em);

            var query    = em.CreateEntityQuery(typeof(MarriageProposalComponent));
            var entities = query.ToEntityArray(Allocator.Temp);
            var comps    = query.ToComponentDataArray<MarriageProposalComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (comps[i].Status != MarriageProposalStatus.Pending)
                    continue;

                if (inWorldDays - comps[i].ProposedAtInWorldDays < ExpirationInWorldDays)
                    continue;

                var p = comps[i];
                p.Status = MarriageProposalStatus.Expired;
                em.SetComponentData(entities[i], p);
            }

            entities.Dispose();
            comps.Dispose();
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
