using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    /// <summary>
    /// Debug command surface extension for the dynasty lane. Gives the governed
    /// dynasty smoke validator and future UI a controlled way to inspect
    /// member state and simulate fallen events so succession can be observed
    /// deterministically.
    ///
    /// Browser reference: dynasty member state inspection and the fallen-member
    /// transition path in simulation.js (transferMemberToCaptor et al.).
    /// </summary>
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public int TryDebugGetDynastyMemberCount(string factionId)
        {
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return 0;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return 0;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntityByDynasty(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return 0;
            }

            return entityManager.GetBuffer<DynastyMemberRef>(factionEntity).Length;
        }

        public bool TryDebugGetDynastyState(string factionId, out DynastyStateComponent dynastyState)
        {
            dynastyState = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntityByDynasty(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<DynastyStateComponent>(factionEntity))
            {
                return false;
            }

            dynastyState = entityManager.GetComponentData<DynastyStateComponent>(factionEntity);
            return true;
        }

        public bool TryDebugGetSuccessionCrisis(string factionId, out SuccessionCrisisComponent crisis)
        {
            crisis = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntityByDynasty(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasComponent<SuccessionCrisisComponent>(factionEntity))
            {
                return false;
            }

            crisis = entityManager.GetComponentData<SuccessionCrisisComponent>(factionEntity);
            return true;
        }

        public bool TryDebugGetPoliticalEvents(string factionId, out FixedString512Bytes eventsSummary)
        {
            eventsSummary = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntityByDynasty(entityManager, factionId);
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<DynastyPoliticalEventComponent>(factionEntity))
            {
                return false;
            }

            var events = entityManager.GetBuffer<DynastyPoliticalEventComponent>(factionEntity);
            for (int i = 0; i < events.Length; i++)
            {
                if (i > 0)
                {
                    eventsSummary.Append("|");
                }

                eventsSummary.Append(events[i].EventType);
                eventsSummary.Append("@");
                eventsSummary.Append(math.round(events[i].ExpiresAtInWorldDays));
            }

            return events.Length > 0;
        }

        public bool TryDebugGetMinorHouseLevyState(
            string factionId,
            out FixedString512Bytes levySummary)
        {
            levySummary = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<MinorHouseLevyComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var levies = query.ToComponentDataArray<MinorHouseLevyComponent>(Allocator.Temp);
            query.Dispose();

            var target = new FixedString32Bytes(factionId);
            for (int i = 0; i < factions.Length; i++)
            {
                if (!factions[i].FactionId.Equals(target))
                {
                    continue;
                }

                MinorHouseLevyComponent levy = levies[i];
                levySummary = new FixedString512Bytes(
                    $"MinorHouseLevy|FactionId={factions[i].FactionId}" +
                    $"|Claim={levy.ClaimControlPointId}" +
                    $"|Status={levy.LevyStatus}" +
                    $"|Progress={math.round(levy.LevyAccumulator * 100f) / 100f}" +
                    $"|SecondsRequired={math.round(levy.LevyIntervalSeconds * 100f) / 100f}" +
                    $"|Unit={levy.LevyUnitId}" +
                    $"|Retinue={levy.RetinueCount}/{levy.RetinueCap}" +
                    $"|Levies={levy.LeviesIssued}" +
                    $"|LastUnit={levy.LastLevyUnitId}" +
                    $"|PressureLevel={levy.ParentPressureLevel}" +
                    $"|PressureStatus={levy.ParentPressureStatus}" +
                    $"|LevyTempo={math.round(levy.ParentPressureLevyTempo * 100f) / 100f}");
                return true;
            }

            return false;
        }

        public bool TryDebugGetDynastyMember(
            string factionId,
            DynastyRole role,
            out DynastyMemberComponent member)
        {
            member = default;
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntityByDynasty(entityManager, factionId);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            var memberBuffer = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            bool foundAny = false;
            DynastyMemberComponent fallback = default;
            for (int i = 0; i < memberBuffer.Length; i++)
            {
                var memberEntity = memberBuffer[i].Member;
                if (!entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (candidate.Role != role)
                {
                    continue;
                }

                if (candidate.Status == DynastyMemberStatus.Ruling ||
                    candidate.Status == DynastyMemberStatus.Active ||
                    candidate.Status == DynastyMemberStatus.Captured)
                {
                    member = candidate;
                    return true;
                }

                if (!foundAny)
                {
                    fallback = candidate;
                    foundAny = true;
                }
            }

            if (foundAny)
            {
                member = fallback;
                return true;
            }

            return false;
        }

        public bool TryDebugFellDynastyMember(string factionId, DynastyRole role)
        {
            if (string.IsNullOrWhiteSpace(factionId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var factionEntity = FindFactionEntityByDynasty(entityManager, factionId);
            if (factionEntity == Entity.Null)
            {
                return false;
            }

            var memberBuffer = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < memberBuffer.Length; i++)
            {
                var memberEntity = memberBuffer[i].Member;
                if (!entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var candidate = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (candidate.Role == role && candidate.Status != DynastyMemberStatus.Fallen)
                {
                    candidate.Status = DynastyMemberStatus.Fallen;
                    candidate.FallenAtWorldSeconds = (float)world.Time.ElapsedTime;
                    entityManager.SetComponentData(memberEntity, candidate);

                    var ledger = entityManager.GetBuffer<DynastyFallenLedger>(factionEntity);
                    ledger.Add(new DynastyFallenLedger
                    {
                        MemberId = candidate.MemberId,
                        Title = candidate.Title,
                        Role = role,
                        FallenAtWorldSeconds = candidate.FallenAtWorldSeconds,
                    });
                    return true;
                }
            }

            return false;
        }

        private static Entity FindFactionEntityByDynasty(EntityManager entityManager, string factionId)
        {
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.ToString() == factionId)
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }
    }
}
