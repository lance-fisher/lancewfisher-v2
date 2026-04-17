using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Shared dynasty spawning helpers. Called from the skirmish bootstrap
    /// path and from the governed dynasty smoke validator. Centralizing the
    /// spawn logic means both paths guarantee the canonical eight-member
    /// set matches the browser exactly, and downstream systems can rely on
    /// a single construction seam.
    /// </summary>
    public static class DynastyBootstrap
    {
        public static void AttachDynasty(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes factionId)
        {
            if (entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                return;
            }

            entityManager.AddComponentData(factionEntity, new DynastyStateComponent
            {
                ActiveMemberCap = DynastyTemplates.InitialActiveMemberCap,
                DormantReserve = 0,
                Legitimacy = DynastyTemplates.InitialLegitimacy,
                LoyaltyPressure = 0f,
                Interregnum = false,
            });

            entityManager.AddBuffer<DynastyMemberRef>(factionEntity);
            entityManager.AddBuffer<DynastyFallenLedger>(factionEntity);

            var templates = DynastyTemplates.Canonical;
            var memberEntities = new NativeArray<Entity>(templates.Length, Allocator.Temp);
            try
            {
                for (int i = 0; i < templates.Length; i++)
                {
                    var template = templates[i];
                    var memberEntity = entityManager.CreateEntity();
                    entityManager.AddComponentData(memberEntity, new FactionComponent
                    {
                        FactionId = factionId,
                    });
                    entityManager.AddComponentData(memberEntity, new DynastyMemberComponent
                    {
                        MemberId = BuildMemberId(factionId, template.Suffix),
                        Title = template.Title,
                        Role = template.Role,
                        Path = template.Path,
                        AgeYears = template.AgeYears,
                        Status = template.Status,
                        Renown = template.Renown,
                        Order = i,
                        FallenAtWorldSeconds = -1f,
                    });
                    memberEntities[i] = memberEntity;
                }

                var memberBuffer = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
                for (int i = 0; i < memberEntities.Length; i++)
                {
                    memberBuffer.Add(new DynastyMemberRef { Member = memberEntities[i] });
                }
            }
            finally
            {
                memberEntities.Dispose();
            }
        }

        public static FixedString64Bytes BuildMemberId(FixedString32Bytes factionId, string suffix)
        {
            var id = new FixedString64Bytes();
            id.Append(factionId);
            id.Append("-bloodline-");
            id.Append(suffix);
            return id;
        }
    }
}
