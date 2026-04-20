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
    /// MARRIAGE_GESTATION_IN_WORLD_DAYS = 280 (browser default).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MarriageDeathDissolutionSystem))]
    public partial struct MarriageGestationSystem : ISystem
    {
        public const float GestationInWorldDays = 280f;

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

                SpawnChild(em, entities[i], ref m);
                em.SetComponentData(entities[i], m);
            }

            entities.Dispose();
            comps.Dispose();
        }

        private static void SpawnChild(
            EntityManager em,
            Entity marriageEntity,
            ref MarriageComponent marriage)
        {
            Entity headFaction = FindFactionEntity(em, marriage.HeadFactionId);

            if (headFaction == Entity.Null || !em.HasBuffer<DynastyMemberRef>(headFaction))
            {
                marriage.ChildGenerated = true;
                return;
            }

            var mixedBloodline = new DynastyMixedBloodlineComponent
            {
                HeadHouseId = ResolveHouseId(em, headFaction),
                SpouseHouseId = ResolveHouseId(em, FindFactionEntity(em, marriage.SpouseFactionId)),
            };

            // Create child entity.
            var childEntity = em.CreateEntity();
            em.AddComponentData(childEntity, new FactionComponent { FactionId = marriage.HeadFactionId });
            em.AddComponentData(childEntity, new DynastyMemberComponent
            {
                MemberId  = BuildChildMemberId(marriage.MarriageId),
                Title     = BuildChildTitle(em, headFaction, marriage.HeadMemberId),
                Role      = DynastyRole.HeirDesignate,
                Path      = DynastyPath.Governance,
                AgeYears  = 0f,
                Status    = DynastyMemberStatus.Active,
                Renown    = 4f,
                Order     = 99,
                FallenAtWorldSeconds = -1f,
            });
            em.AddComponentData(childEntity, mixedBloodline);

            var buffer = em.GetBuffer<DynastyMemberRef>(headFaction);
            buffer.Add(new DynastyMemberRef { Member = childEntity });

            if (!em.HasBuffer<MarriageChildElement>(marriageEntity))
            {
                em.AddBuffer<MarriageChildElement>(marriageEntity);
            }

            em.GetBuffer<MarriageChildElement>(marriageEntity).Add(new MarriageChildElement
            {
                ChildMemberId = BuildChildMemberId(marriage.MarriageId),
            });

            marriage.ChildGenerated = true;
        }

        private static FixedString64Bytes BuildChildMemberId(FixedString64Bytes marriageId)
        {
            var s = new FixedString64Bytes("child-");
            s.Append(marriageId);
            return s;
        }

        private static FixedString64Bytes BuildChildTitle(
            EntityManager em,
            Entity headFaction,
            FixedString64Bytes headMemberId)
        {
            var title = ResolveMemberTitle(em, headFaction, headMemberId);

            var s = new FixedString64Bytes("Child of ");
            if (title.Length > 0)
            {
                s.Append(title);
                return s;
            }

            // Append first 20 chars of head member id to avoid overflow.
            for (int k = 0; k < math.min(headMemberId.Length, 20); k++)
                s.Append(headMemberId[k]);
            return s;
        }

        private static FixedString64Bytes ResolveMemberTitle(
            EntityManager em,
            Entity headFaction,
            FixedString64Bytes headMemberId)
        {
            if (headFaction == Entity.Null || !em.HasBuffer<DynastyMemberRef>(headFaction))
            {
                return default;
            }

            var members = em.GetBuffer<DynastyMemberRef>(headFaction);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null || !em.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.MemberId.Equals(headMemberId))
                {
                    return member.Title;
                }
            }

            return default;
        }

        private static Entity FindFactionEntity(EntityManager em, FixedString32Bytes factionId)
        {
            var factionQuery    = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
            var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            var factionComps    = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            factionQuery.Dispose();

            Entity headFaction = Entity.Null;
            for (int j = 0; j < factionEntities.Length; j++)
            {
                if (factionComps[j].FactionId.Equals(factionId))
                {
                    headFaction = factionEntities[j];
                    break;
                }
            }

            factionEntities.Dispose();
            factionComps.Dispose();
            return headFaction;
        }

        private static FixedString32Bytes ResolveHouseId(EntityManager em, Entity factionEntity)
        {
            if (factionEntity == Entity.Null || !em.HasComponent<FactionHouseComponent>(factionEntity))
            {
                return default;
            }

            return em.GetComponentData<FactionHouseComponent>(factionEntity).HouseId;
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
