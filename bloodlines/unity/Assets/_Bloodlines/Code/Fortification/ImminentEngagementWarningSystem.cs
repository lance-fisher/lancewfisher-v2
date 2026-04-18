using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Ports the browser imminent-engagement warning window onto fortified
    /// settlement entities without widening shared bootstrap seams.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FortificationReserveSystem))]
    public partial struct ImminentEngagementWarningSystem : ISystem
    {
        private const float TileSize = 64f;
        private static readonly FixedString32Bytes PlayerFactionId = new("player");
        private static readonly FixedString32Bytes PrimaryDynasticKeepClassId = new("primary_dynastic_keep");
        private static readonly FixedString64Bytes WatchTowerTypeId = new("watch_tower");
        private static readonly FixedString64Bytes ScoutRiderTypeId = new("scout_rider");

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SettlementComponent>();
            state.RequireForUpdate<FortificationComponent>();
            state.RequireForUpdate<FortificationReserveComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float elapsed = (float)SystemAPI.Time.ElapsedTime;

            var settlementQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<FortificationComponent>(),
                ComponentType.ReadOnly<FortificationReserveComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>());

            var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.Exclude<ConstructionStateComponent>());

            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            var commanderQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<CommanderComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>());

            var linkedReserveQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FortificationSettlementLinkComponent>(),
                ComponentType.ReadOnly<FortificationReserveAssignmentComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var settlementEntities = settlementQuery.ToEntityArray(Allocator.Temp);
            using var settlements = settlementQuery.ToComponentDataArray<SettlementComponent>(Allocator.Temp);
            using var fortifications = settlementQuery.ToComponentDataArray<FortificationComponent>(Allocator.Temp);
            using var reserves = settlementQuery.ToComponentDataArray<FortificationReserveComponent>(Allocator.Temp);
            using var settlementFactions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlementPositions = settlementQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            using var buildingTypes = buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using var buildingFactions = buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var buildingPositions = buildingQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var buildingHealths = buildingQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            using var unitTypes = unitQuery.ToComponentDataArray<UnitTypeComponent>(Allocator.Temp);
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var unitPositions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var unitHealths = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            using var commanderComponents = commanderQuery.ToComponentDataArray<CommanderComponent>(Allocator.Temp);
            using var commanderFactions = commanderQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var commanderPositions = commanderQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var commanderHealths = commanderQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            using var linkedReserveEntities = linkedReserveQuery.ToEntityArray(Allocator.Temp);
            using var linkedReserveLinks = linkedReserveQuery.ToComponentDataArray<FortificationSettlementLinkComponent>(Allocator.Temp);
            using var linkedReserveAssignments = linkedReserveQuery.ToComponentDataArray<FortificationReserveAssignmentComponent>(Allocator.Temp);
            using var linkedReserveFactions = linkedReserveQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var linkedReservePositions = linkedReserveQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var linkedReserveHealths = linkedReserveQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            for (int i = 0; i < settlementEntities.Length; i++)
            {
                var settlementEntity = settlementEntities[i];
                var settlement = settlements[i];
                var fortification = fortifications[i];

                ImminentEngagementComponent engagement;
                bool hasEngagement = entityManager.HasComponent<ImminentEngagementComponent>(settlementEntity);
                if (hasEngagement)
                {
                    engagement = entityManager.GetComponentData<ImminentEngagementComponent>(settlementEntity);
                }
                else
                {
                    engagement = CreateDefaultComponent();
                }

                if (fortification.Tier <= 0)
                {
                    if (hasEngagement)
                    {
                        ResetForInactiveThreat(ref engagement);
                        entityManager.SetComponentData(settlementEntity, engagement);
                    }

                    continue;
                }

                if (!hasEngagement)
                {
                    entityManager.AddComponentData(settlementEntity, engagement);
                }

                var profile = BuildProfile(
                    entityManager,
                    settlement,
                    fortification,
                    reserves[i],
                    settlementFactions[i],
                    settlementPositions[i].Value,
                    buildingTypes,
                    buildingFactions,
                    buildingPositions,
                    buildingHealths,
                    unitTypes,
                    unitFactions,
                    unitPositions,
                    unitHealths,
                    commanderComponents,
                    commanderFactions,
                    commanderPositions,
                    commanderHealths,
                    factionEntities,
                    factionIds);

                if (!profile.Active)
                {
                    if (engagement.Active || engagement.WindowConsumed)
                    {
                        ResetForInactiveThreat(ref engagement);
                    }

                    entityManager.SetComponentData(settlementEntity, engagement);
                    continue;
                }

                bool isPlayerSettlement = settlementFactions[i].FactionId.Equals(PlayerFactionId);
                if (!engagement.Active && !engagement.WindowConsumed)
                {
                    engagement.Active = true;
                    engagement.WindowConsumed = false;
                    engagement.WarningRadius = profile.WarningRadius;
                    engagement.TotalSeconds = profile.WarningSeconds;
                    engagement.RemainingSeconds = profile.WarningSeconds;
                    engagement.ExpiresAt = elapsed + profile.WarningSeconds;
                    engagement.LastActivationAt = elapsed;
                    engagement.EngagedAt = 0f;
                    engagement.SelectedPosture = isPlayerSettlement
                        ? engagement.SelectedPosture
                        : ChooseAiPosture(profile);
                }

                engagement.HostileCount = profile.HostileCount;
                engagement.HostileSiegeCount = profile.HostileSiegeCount;
                engagement.HostileScoutCount = profile.HostileScoutCount;
                engagement.WatchtowerCount = profile.WatchtowerCount;
                engagement.WarningRadius = profile.WarningRadius;
                engagement.BloodlineAtRisk = profile.BloodlineAtRisk;
                engagement.CommanderPresent = profile.CommanderPresent;
                engagement.GovernorPresent = profile.GovernorPresent;

                if (engagement.Active)
                {
                    if (!isPlayerSettlement)
                    {
                        engagement.SelectedPosture = ChooseAiPosture(profile);
                    }

                    engagement.RemainingSeconds = math.max(0f, engagement.ExpiresAt - elapsed);
                    if (engagement.RemainingSeconds <= 0f)
                    {
                        engagement.Active = false;
                        engagement.WindowConsumed = true;
                        engagement.EngagedAt = elapsed;
                        engagement.RemainingSeconds = 0f;

                        if (engagement.SelectedPosture == ImminentEngagementPosture.Counterstroke &&
                            settlement.SettlementClassId.Equals(PrimaryDynasticKeepClassId))
                        {
                            IssueCounterstrokeSortie(
                                entityManager,
                                settlementEntity,
                                settlementFactions[i].FactionId,
                                settlementPositions[i].Value,
                                fortification,
                                profile.HostileCenter,
                                linkedReserveEntities,
                                linkedReserveLinks,
                                linkedReserveAssignments,
                                linkedReserveFactions,
                                linkedReservePositions,
                                linkedReserveHealths,
                                elapsed);
                        }
                    }
                }

                entityManager.SetComponentData(settlementEntity, engagement);
            }
        }

        private static EngagementProfile BuildProfile(
            EntityManager entityManager,
            in SettlementComponent settlement,
            in FortificationComponent fortification,
            in FortificationReserveComponent reserve,
            in FactionComponent settlementFaction,
            float3 settlementPosition,
            NativeArray<BuildingTypeComponent> buildingTypes,
            NativeArray<FactionComponent> buildingFactions,
            NativeArray<PositionComponent> buildingPositions,
            NativeArray<HealthComponent> buildingHealths,
            NativeArray<UnitTypeComponent> unitTypes,
            NativeArray<FactionComponent> unitFactions,
            NativeArray<PositionComponent> unitPositions,
            NativeArray<HealthComponent> unitHealths,
            NativeArray<CommanderComponent> commanderComponents,
            NativeArray<FactionComponent> commanderFactions,
            NativeArray<PositionComponent> commanderPositions,
            NativeArray<HealthComponent> commanderHealths,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factionIds)
        {
            float reserveRadius = fortification.ReserveRadiusTiles * TileSize;
            float threatRadius = fortification.ThreatRadiusTiles * TileSize;
            float watchtowerRadius = math.max(
                reserveRadius,
                ImminentEngagementCanon.WatchtowerRadiusTiles * TileSize);
            int watchtowerCount = CountWatchtowers(
                settlementFaction.FactionId,
                settlementPosition,
                watchtowerRadius,
                buildingTypes,
                buildingFactions,
                buildingPositions,
                buildingHealths);

            float warningRadius = threatRadius + (
                ImminentEngagementCanon.WarningBufferTiles +
                (watchtowerCount * 1.25f) +
                (math.max(0, fortification.Tier - 1) * 0.6f)) * TileSize;

            int hostileCount = 0;
            int hostileSiegeCount = 0;
            int hostileScoutCount = 0;
            float3 hostileTotal = float3.zero;

            for (int i = 0; i < unitTypes.Length; i++)
            {
                if (unitHealths[i].Current <= 0f ||
                    unitFactions[i].FactionId.Equals(settlementFaction.FactionId) ||
                    unitTypes[i].Role == UnitRole.Worker)
                {
                    continue;
                }

                if (DistanceXZ(unitPositions[i].Value, settlementPosition) > warningRadius)
                {
                    continue;
                }

                hostileCount++;
                hostileTotal += unitPositions[i].Value;

                if (unitTypes[i].SiegeClass != SiegeClass.None)
                {
                    hostileSiegeCount++;
                }

                if (unitTypes[i].TypeId.Equals(ScoutRiderTypeId))
                {
                    hostileScoutCount++;
                }
            }

            ResolveKeepPresence(
                entityManager,
                settlement,
                settlementFaction.FactionId,
                settlementPosition,
                fortification,
                commanderComponents,
                commanderFactions,
                commanderPositions,
                commanderHealths,
                factionEntities,
                factionIds,
                out bool commanderPresent,
                out bool governorPresent,
                out bool bloodlineAtRisk);

            float baseSeconds = settlement.SettlementClassId.Equals(PrimaryDynasticKeepClassId)
                ? ImminentEngagementCanon.KeepBaseSeconds
                : ImminentEngagementCanon.SettlementBaseSeconds;
            float warningSeconds = math.clamp(
                baseSeconds +
                (watchtowerCount * 2f) +
                (math.max(0, fortification.Tier - 1) * 1.5f) -
                (hostileScoutCount * 1.5f) -
                hostileSiegeCount,
                ImminentEngagementCanon.MinSeconds,
                ImminentEngagementCanon.MaxSeconds);

            return new EngagementProfile
            {
                Active = hostileCount > 0,
                WarningRadius = warningRadius,
                WarningSeconds = warningSeconds,
                HostileCount = hostileCount,
                HostileSiegeCount = hostileSiegeCount,
                HostileScoutCount = hostileScoutCount,
                WatchtowerCount = watchtowerCount,
                CommanderPresent = commanderPresent,
                GovernorPresent = governorPresent,
                BloodlineAtRisk = bloodlineAtRisk,
                HostileCenter = hostileCount > 0 ? hostileTotal / hostileCount : settlementPosition,
                ReadyReserves = reserve.ReadyReserveCount,
            };
        }

        private static int CountWatchtowers(
            FixedString32Bytes settlementFactionId,
            float3 settlementPosition,
            float radius,
            NativeArray<BuildingTypeComponent> buildingTypes,
            NativeArray<FactionComponent> buildingFactions,
            NativeArray<PositionComponent> buildingPositions,
            NativeArray<HealthComponent> buildingHealths)
        {
            int count = 0;
            for (int i = 0; i < buildingTypes.Length; i++)
            {
                if (buildingHealths[i].Current <= 0f ||
                    !buildingFactions[i].FactionId.Equals(settlementFactionId) ||
                    !buildingTypes[i].TypeId.Equals(WatchTowerTypeId) ||
                    DistanceXZ(buildingPositions[i].Value, settlementPosition) > radius)
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private static void ResolveKeepPresence(
            EntityManager entityManager,
            in SettlementComponent settlement,
            FixedString32Bytes settlementFactionId,
            float3 settlementPosition,
            in FortificationComponent fortification,
            NativeArray<CommanderComponent> commanderComponents,
            NativeArray<FactionComponent> commanderFactions,
            NativeArray<PositionComponent> commanderPositions,
            NativeArray<HealthComponent> commanderHealths,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factionIds,
            out bool commanderPresent,
            out bool governorPresent,
            out bool bloodlineAtRisk)
        {
            commanderPresent = false;
            governorPresent = false;
            bloodlineAtRisk = false;
            FixedString64Bytes commanderMemberId = default;

            float keepPresenceRadius = fortification.KeepPresenceRadiusTiles * TileSize;
            for (int i = 0; i < commanderComponents.Length; i++)
            {
                if (commanderHealths[i].Current <= 0f ||
                    !commanderFactions[i].FactionId.Equals(settlementFactionId) ||
                    DistanceXZ(commanderPositions[i].Value, settlementPosition) > keepPresenceRadius)
                {
                    continue;
                }

                commanderPresent = true;
                commanderMemberId = commanderComponents[i].MemberId;
                break;
            }

            Entity factionEntity = FindFactionDynastyEntity(entityManager, settlementFactionId, factionEntities, factionIds);
            if (factionEntity == Entity.Null)
            {
                return;
            }

            bool dynasticSeatPresence = false;
            bool commandingBloodlinePresence = false;
            var memberBuffer = entityManager.GetBuffer<DynastyMemberRef>(factionEntity, true);
            for (int i = 0; i < memberBuffer.Length; i++)
            {
                Entity memberEntity = memberBuffer[i].Member;
                if (!entityManager.Exists(memberEntity) ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                bool presentForSeat = member.Status == DynastyMemberStatus.Active || member.Status == DynastyMemberStatus.Ruling;
                if (settlement.SettlementClassId.Equals(PrimaryDynasticKeepClassId) &&
                    member.Role == DynastyRole.HeadOfBloodline &&
                    member.Status == DynastyMemberStatus.Ruling)
                {
                    dynasticSeatPresence = true;
                }

                if (member.Role == DynastyRole.Governor && presentForSeat)
                {
                    governorPresent = true;
                }

                if (commanderPresent &&
                    member.MemberId.Equals(commanderMemberId) &&
                    presentForSeat &&
                    (member.Role == DynastyRole.HeadOfBloodline ||
                     member.Role == DynastyRole.HeirDesignate ||
                     member.Role == DynastyRole.Commander))
                {
                    commandingBloodlinePresence = true;
                }
            }

            bloodlineAtRisk = dynasticSeatPresence || commandingBloodlinePresence;
        }

        private static Entity FindFactionDynastyEntity(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factionIds)
        {
            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (!factionIds[i].FactionId.Equals(factionId) ||
                    !entityManager.HasBuffer<DynastyMemberRef>(factionEntities[i]))
                {
                    continue;
                }

                return factionEntities[i];
            }

            return Entity.Null;
        }

        private static ImminentEngagementPosture ChooseAiPosture(in EngagementProfile profile)
        {
            if (profile.BloodlineAtRisk || profile.HostileCount > math.max(1, profile.ReadyReserves + 1))
            {
                return ImminentEngagementPosture.Brace;
            }

            if (profile.CommanderPresent && profile.HostileCount <= math.max(3, profile.ReadyReserves + 1))
            {
                return ImminentEngagementPosture.Counterstroke;
            }

            return ImminentEngagementPosture.Steady;
        }

        private static void IssueCounterstrokeSortie(
            EntityManager entityManager,
            Entity settlementEntity,
            FixedString32Bytes settlementFactionId,
            float3 settlementPosition,
            in FortificationComponent fortification,
            float3 hostileCenter,
            NativeArray<Entity> linkedReserveEntities,
            NativeArray<FortificationSettlementLinkComponent> linkedReserveLinks,
            NativeArray<FortificationReserveAssignmentComponent> linkedReserveAssignments,
            NativeArray<FactionComponent> linkedReserveFactions,
            NativeArray<PositionComponent> linkedReservePositions,
            NativeArray<HealthComponent> linkedReserveHealths,
            float elapsed)
        {
            int committedCount = 0;
            float reserveRadius = fortification.ReserveRadiusTiles * TileSize;
            float threatRadius = fortification.ThreatRadiusTiles * TileSize;

            for (int i = 0; i < linkedReserveEntities.Length; i++)
            {
                if (linkedReserveLinks[i].SettlementEntity != settlementEntity ||
                    !linkedReserveFactions[i].FactionId.Equals(settlementFactionId) ||
                    linkedReserveHealths[i].Current <= 0f ||
                    linkedReserveAssignments[i].Duty != ReserveDutyState.Ready)
                {
                    continue;
                }

                if (DistanceXZ(linkedReservePositions[i].Value, settlementPosition) > reserveRadius * 0.55f ||
                    DistanceXZ(linkedReservePositions[i].Value, hostileCenter) <= threatRadius * 0.55f)
                {
                    continue;
                }

                var assignment = entityManager.GetComponentData<FortificationReserveAssignmentComponent>(linkedReserveEntities[i]);
                assignment.Duty = ReserveDutyState.Muster;
                entityManager.SetComponentData(linkedReserveEntities[i], assignment);

                if (entityManager.HasComponent<MoveCommandComponent>(linkedReserveEntities[i]))
                {
                    var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(linkedReserveEntities[i]);
                    moveCommand.Destination = hostileCenter;
                    moveCommand.StoppingDistance = math.max(0.75f, moveCommand.StoppingDistance);
                    moveCommand.IsActive = true;
                    entityManager.SetComponentData(linkedReserveEntities[i], moveCommand);
                }

                committedCount++;
            }

            if (committedCount <= 0 ||
                !entityManager.HasComponent<FortificationReserveComponent>(settlementEntity))
            {
                return;
            }

            var reserve = entityManager.GetComponentData<FortificationReserveComponent>(settlementEntity);
            reserve.LastCommitAt = elapsed;
            reserve.LastCommittedCount = committedCount;
            reserve.ReadyReserveCount = math.max(0, reserve.ReadyReserveCount - committedCount);
            reserve.MusteringReserveCount += committedCount;
            entityManager.SetComponentData(settlementEntity, reserve);
        }

        private static void ResetForInactiveThreat(ref ImminentEngagementComponent engagement)
        {
            engagement.Active = false;
            engagement.WindowConsumed = false;
            engagement.WarningRadius = 0f;
            engagement.TotalSeconds = 0f;
            engagement.RemainingSeconds = 0f;
            engagement.ExpiresAt = 0f;
            engagement.HostileCount = 0;
            engagement.HostileSiegeCount = 0;
            engagement.HostileScoutCount = 0;
            engagement.WatchtowerCount = 0;
            engagement.BloodlineAtRisk = false;
            engagement.CommanderPresent = false;
            engagement.GovernorPresent = false;
        }

        private static ImminentEngagementComponent CreateDefaultComponent()
        {
            return new ImminentEngagementComponent
            {
                Active = false,
                WindowConsumed = false,
                SelectedPosture = ImminentEngagementPosture.Steady,
                LastActivationAt = -999f,
                EngagedAt = 0f,
            };
        }

        private static float DistanceXZ(float3 left, float3 right)
        {
            return math.distance(left.xz, right.xz);
        }

        private struct EngagementProfile
        {
            public bool Active;
            public float WarningRadius;
            public float WarningSeconds;
            public int HostileCount;
            public int HostileSiegeCount;
            public int HostileScoutCount;
            public int WatchtowerCount;
            public bool CommanderPresent;
            public bool GovernorPresent;
            public bool BloodlineAtRisk;
            public float3 HostileCenter;
            public int ReadyReserves;
        }
    }
}
