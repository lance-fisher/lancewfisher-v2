using Bloodlines.Components;
using Bloodlines.Fortification;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.TerritoryGovernance
{
    /// <summary>
    /// Browser reference: syncGovernorAssignments (simulation.js 5914-6004).
    /// Rebuilds seat assignments once per whole in-world day, projects them onto
    /// control points and settlements, and materializes the control-point
    /// specialization profile consumed by territory systems.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Bloodlines.Dynasties.DynastySuccessionSystem))]
    [UpdateBefore(typeof(Bloodlines.Systems.ControlPointCaptureSystem))]
    [UpdateBefore(typeof(Bloodlines.Systems.ControlPointResourceTrickleSystem))]
    [UpdateBefore(typeof(FortificationReserveSystem))]
    public partial struct GovernorSpecializationSystem : ISystem
    {
        private int _lastProcessedDay;
        private byte _initialized;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<ControlPointComponent>();
            state.RequireForUpdate<DynastyStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;
            int currentDay = (int)math.floor(SystemAPI.GetSingleton<DualClockComponent>().InWorldDays);
            if (_initialized != 0 && currentDay == _lastProcessedDay)
            {
                return;
            }

            _initialized = 1;
            _lastProcessedDay = currentDay;

            using var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<Entity> controlPointEntities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<ControlPointComponent> controlPoints =
                controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            using var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>());
            using NativeArray<Entity> settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> settlementFactions =
                settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<SettlementComponent> settlements =
                settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>(),
                ComponentType.ReadOnly<DynastyMemberRef>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions =
                factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            ClearExistingAssignments(entityManager, controlPointEntities, settlementEntities);

            for (int factionIndex = 0; factionIndex < factionEntities.Length; factionIndex++)
            {
                Entity factionEntity = factionEntities[factionIndex];
                FixedString32Bytes factionId = factions[factionIndex].FactionId;

                using var availableMembers = BuildAvailableMembers(entityManager, factionEntity);
                if (availableMembers.Length == 0)
                {
                    continue;
                }

                using var candidates = BuildSeatCandidates(
                    entityManager,
                    factionId,
                    currentDay,
                    controlPointEntities,
                    controlPoints,
                    settlementEntities,
                    settlementFactions,
                    settlements);
                if (candidates.Length == 0)
                {
                    continue;
                }

                SortCandidates(candidates);
                for (int i = 0; i < candidates.Length; i++)
                {
                    int memberIndex = SelectBestMemberIndex(availableMembers, candidates[i]);
                    if (memberIndex < 0)
                    {
                        break;
                    }

                    GovernanceSeatMember member = availableMembers[memberIndex];
                    RemoveMemberAtSwapBack(availableMembers, memberIndex);
                    AssignCandidate(entityManager, candidates[i], member, currentDay);
                }
            }
        }

        private static void ClearExistingAssignments(
            EntityManager entityManager,
            NativeArray<Entity> controlPointEntities,
            NativeArray<Entity> settlementEntities)
        {
            for (int i = 0; i < controlPointEntities.Length; i++)
            {
                Entity controlPointEntity = controlPointEntities[i];
                if (entityManager.HasComponent<GovernorSeatAssignmentComponent>(controlPointEntity))
                {
                    entityManager.RemoveComponent<GovernorSeatAssignmentComponent>(controlPointEntity);
                }

                if (entityManager.HasComponent<GovernorSpecializationComponent>(controlPointEntity))
                {
                    entityManager.RemoveComponent<GovernorSpecializationComponent>(controlPointEntity);
                }
            }

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                Entity settlementEntity = settlementEntities[i];
                if (entityManager.HasComponent<GovernorSeatAssignmentComponent>(settlementEntity))
                {
                    entityManager.RemoveComponent<GovernorSeatAssignmentComponent>(settlementEntity);
                }
            }
        }

        private static NativeList<GovernanceSeatMember> BuildAvailableMembers(
            EntityManager entityManager,
            Entity factionEntity)
        {
            var availableMembers = new NativeList<GovernanceSeatMember>(Allocator.Temp);
            DynamicBuffer<DynastyMemberRef> roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.Exists(memberEntity) ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                DynastyMemberComponent member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (!IsAvailableGovernanceMember(member))
                {
                    continue;
                }

                availableMembers.Add(new GovernanceSeatMember
                {
                    MemberId = member.MemberId,
                    Role = member.Role,
                    Renown = member.Renown,
                });
            }

            SortMembers(availableMembers);
            return availableMembers;
        }

        private static NativeList<GovernanceSeatCandidate> BuildSeatCandidates(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            int currentDay,
            NativeArray<Entity> controlPointEntities,
            NativeArray<ControlPointComponent> controlPoints,
            NativeArray<Entity> settlementEntities,
            NativeArray<FactionComponent> settlementFactions,
            NativeArray<SettlementComponent> settlements)
        {
            var candidates = new NativeList<GovernanceSeatCandidate>(Allocator.Temp);
            int ownedControlPointCount = 0;

            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(factionId))
                {
                    continue;
                }

                ownedControlPointCount++;
                GovernorSpecializationId specializationId =
                    GovernorSpecializationCanon.ResolveSpecialization(controlPoints[i].SettlementClassId);
                if (specializationId == GovernorSpecializationId.None)
                {
                    continue;
                }

                float urgency = controlPoints[i].ControlState != ControlState.Stabilized
                    ? 120f - controlPoints[i].Loyalty
                    : 74f - (controlPoints[i].Loyalty * 0.35f);
                float classBias = specializationId == GovernorSpecializationId.BorderMarshal
                    ? 8f
                    : specializationId == GovernorSpecializationId.CivicSteward
                        ? 6f
                        : 4f;

                candidates.Add(new GovernanceSeatCandidate
                {
                    AnchorEntity = controlPointEntities[i],
                    AnchorType = GovernanceAnchorType.ControlPoint,
                    AnchorId = controlPoints[i].ControlPointId,
                    SpecializationId = specializationId,
                    Priority = urgency + classBias,
                    Loyalty = controlPoints[i].Loyalty,
                    LastSyncedInWorldDays = currentDay,
                });
            }

            for (int i = 0; i < settlements.Length; i++)
            {
                if (!settlementFactions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                GovernorSpecializationId specializationId =
                    GovernorSpecializationCanon.ResolveSpecialization(settlements[i].SettlementClassId);
                if (specializationId == GovernorSpecializationId.None)
                {
                    continue;
                }

                bool threatActive =
                    entityManager.HasComponent<FortificationReserveComponent>(settlementEntities[i]) &&
                    entityManager.GetComponentData<FortificationReserveComponent>(settlementEntities[i]).ThreatActive;
                float keepThreatBonus = threatActive ? 36f : 0f;
                float noTerritoryBonus = ownedControlPointCount == 0 ? 40f : 0f;
                float fortPriority = 40f +
                    (settlements[i].FortificationTier * 6f) +
                    (settlements[i].SettlementClassId.Equals(new FixedString32Bytes("primary_dynastic_keep")) ? 10f : 0f) +
                    noTerritoryBonus +
                    keepThreatBonus;

                candidates.Add(new GovernanceSeatCandidate
                {
                    AnchorEntity = settlementEntities[i],
                    AnchorType = GovernanceAnchorType.Settlement,
                    AnchorId = settlements[i].SettlementId,
                    SpecializationId = specializationId,
                    Priority = fortPriority,
                    Loyalty = 100f,
                    LastSyncedInWorldDays = currentDay,
                });
            }

            return candidates;
        }

        private static void AssignCandidate(
            EntityManager entityManager,
            GovernanceSeatCandidate candidate,
            GovernanceSeatMember member,
            int currentDay)
        {
            var assignment = new GovernorSeatAssignmentComponent
            {
                GovernorMemberId = member.MemberId,
                SpecializationId = candidate.SpecializationId,
                AnchorType = candidate.AnchorType,
                PriorityScore = candidate.Priority,
                LastSyncedInWorldDays = currentDay,
            };

            entityManager.AddComponentData(candidate.AnchorEntity, assignment);

            if (candidate.AnchorType == GovernanceAnchorType.ControlPoint)
            {
                GovernorSpecializationComponent profile =
                    GovernorSpecializationCanon.GetProfile(candidate.SpecializationId, member.MemberId);
                entityManager.AddComponentData(candidate.AnchorEntity, profile);
            }
        }

        private static bool IsAvailableGovernanceMember(DynastyMemberComponent member)
        {
            if (member.Status == DynastyMemberStatus.Fallen ||
                member.Status == DynastyMemberStatus.Captured)
            {
                return false;
            }

            return member.Role == DynastyRole.Governor ||
                   member.Role == DynastyRole.HeadOfBloodline ||
                   member.Role == DynastyRole.HeirDesignate ||
                   member.Role == DynastyRole.Diplomat ||
                   member.Role == DynastyRole.Merchant;
        }

        private static int SelectBestMemberIndex(
            NativeList<GovernanceSeatMember> members,
            GovernanceSeatCandidate candidate)
        {
            int bestIndex = -1;
            float bestScore = float.MinValue;
            for (int i = 0; i < members.Length; i++)
            {
                float score = GovernorSpecializationCanon.GetGovernanceSeatMemberScore(
                    members[i].Role,
                    members[i].Renown,
                    candidate.SpecializationId,
                    candidate.AnchorType);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static void SortCandidates(NativeList<GovernanceSeatCandidate> candidates)
        {
            for (int i = 0; i < candidates.Length - 1; i++)
            {
                for (int j = i + 1; j < candidates.Length; j++)
                {
                    GovernanceSeatCandidate left = candidates[i];
                    GovernanceSeatCandidate right = candidates[j];
                    bool shouldSwap = right.Priority > left.Priority ||
                        (math.abs(right.Priority - left.Priority) < 0.0001f && right.Loyalty < left.Loyalty);
                    if (!shouldSwap)
                    {
                        continue;
                    }

                    candidates[i] = right;
                    candidates[j] = left;
                }
            }
        }

        private static void SortMembers(NativeList<GovernanceSeatMember> members)
        {
            for (int i = 0; i < members.Length - 1; i++)
            {
                for (int j = i + 1; j < members.Length; j++)
                {
                    GovernanceSeatMember left = members[i];
                    GovernanceSeatMember right = members[j];
                    bool shouldSwap =
                        GetRolePriority(right.Role) < GetRolePriority(left.Role) ||
                        (GetRolePriority(right.Role) == GetRolePriority(left.Role) && right.Renown > left.Renown) ||
                        (GetRolePriority(right.Role) == GetRolePriority(left.Role) &&
                         math.abs(right.Renown - left.Renown) < 0.0001f &&
                         string.CompareOrdinal(right.MemberId.ToString(), left.MemberId.ToString()) < 0);
                    if (!shouldSwap)
                    {
                        continue;
                    }

                    members[i] = right;
                    members[j] = left;
                }
            }
        }

        private static int GetRolePriority(DynastyRole role)
        {
            return role switch
            {
                DynastyRole.Governor => 0,
                DynastyRole.HeadOfBloodline => 1,
                DynastyRole.HeirDesignate => 2,
                DynastyRole.Diplomat => 3,
                DynastyRole.Merchant => 4,
                _ => 5,
            };
        }

        private static void RemoveMemberAtSwapBack(
            NativeList<GovernanceSeatMember> members,
            int index)
        {
            int lastIndex = members.Length - 1;
            members[index] = members[lastIndex];
            members.RemoveAt(lastIndex);
        }

        private struct GovernanceSeatMember
        {
            public FixedString64Bytes MemberId;
            public DynastyRole Role;
            public float Renown;
        }

        private struct GovernanceSeatCandidate
        {
            public Entity AnchorEntity;
            public GovernanceAnchorType AnchorType;
            public FixedString64Bytes AnchorId;
            public GovernorSpecializationId SpecializationId;
            public float Priority;
            public float Loyalty;
            public float LastSyncedInWorldDays;
        }
    }
}
