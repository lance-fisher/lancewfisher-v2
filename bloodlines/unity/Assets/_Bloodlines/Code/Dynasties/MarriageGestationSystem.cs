using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Generates a child dynasty member when an active marriage reaches
    /// its expected child date. Only the primary marriage record generates
    /// children to prevent duplication. Browser reference: simulation.js
    /// tickMarriageGestation (~7496).
    /// MARRIAGE_GESTATION_IN_WORLD_DAYS = 60 (browser default).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct MarriageGestationSystem : ISystem
    {
        public const float GestationInWorldDays = 60f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MarriageComponent>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            float inWorldDays = GetInWorldDays(em);

            var query    = em.CreateEntityQuery(typeof(MarriageComponent));
            var entities = query.ToEntityArray(Allocator.Temp);
            var comps    = query.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                var m = comps[i];
                if (!m.IsPrimary || m.ChildGenerated || m.Dissolved)
                    continue;
                if (inWorldDays < m.ExpectedChildAtInWorldDays)
                    continue;

                SpawnChild(em, ref m);
                em.SetComponentData(entities[i], m);
            }

            entities.Dispose();
            comps.Dispose();
        }

        private static void SpawnChild(EntityManager em, ref MarriageComponent marriage)
        {
            // Find the head faction entity to add the child member to.
            var factionQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
            var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            var factionComps    = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            factionQuery.Dispose();

            Entity headFaction = Entity.Null;
            for (int j = 0; j < factionEntities.Length; j++)
            {
                if (factionComps[j].FactionId.Equals(marriage.HeadFactionId))
                {
                    headFaction = factionEntities[j];
                    break;
                }
            }

            factionEntities.Dispose();
            factionComps.Dispose();

            if (headFaction == Entity.Null || !em.HasBuffer<DynastyMemberRef>(headFaction))
            {
                marriage.ChildGenerated = true;
                return;
            }

            // Create child entity.
            var childEntity = em.CreateEntity();
            em.AddComponentData(childEntity, new FactionComponent { FactionId = marriage.HeadFactionId });
            em.AddComponentData(childEntity, new DynastyMemberComponent
            {
                MemberId  = BuildChildMemberId(marriage.MarriageId),
                Title     = BuildChildTitle(marriage.HeadMemberId),
                Role      = DynastyRole.HeirDesignate,
                Path      = DynastyPath.Governance,
                AgeYears  = 0f,
                Status    = DynastyMemberStatus.Active,
                Renown    = 4f,
                Order     = 99,
                FallenAtWorldSeconds = -1f,
            });

            var buffer = em.GetBuffer<DynastyMemberRef>(headFaction);
            buffer.Add(new DynastyMemberRef { Member = childEntity });

            marriage.ChildGenerated = true;
        }

        private static FixedString64Bytes BuildChildMemberId(FixedString64Bytes marriageId)
        {
            var s = new FixedString64Bytes("child-");
            s.Append(marriageId);
            return s;
        }

        private static FixedString64Bytes BuildChildTitle(FixedString64Bytes headMemberId)
        {
            var s = new FixedString64Bytes("Child of ");
            // Append first 20 chars of head member id to avoid overflow.
            for (int k = 0; k < math.min(headMemberId.Length, 20); k++)
                s.Append(headMemberId[k]);
            return s;
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
