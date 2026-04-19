using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Fortification
{
    /// <summary>
    /// Browser reference: simulation.js tickImminentEngagementWarnings (11742-11873).
    /// This slice intentionally uses the reserve threat flag and other existing
    /// component state instead of widening into new spatial perception queries.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FortificationReserveSystem))]
    public partial struct ImminentEngagementWarningSystem : ISystem
    {
        private static readonly FixedString32Bytes PrimaryDynasticKeepClassId = new("primary_dynastic_keep");
        private static readonly FixedString32Bytes SteadyId = new("steady");
        private static readonly FixedString32Bytes HeadOfBloodlineRole = new("head_of_bloodline");
        private static readonly FixedString32Bytes HeirDesignateRole = new("heir_designate");
        private static readonly FixedString32Bytes CommanderRole = new("commander");

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SettlementComponent>();
            state.RequireForUpdate<FortificationComponent>();
            state.RequireForUpdate<FortificationReserveComponent>();
            state.RequireForUpdate<ImminentEngagementComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float elapsedTime = (float)SystemAPI.Time.ElapsedTime;

            var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionLoyaltyComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionIds = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var factionLoyalties = factionQuery.ToComponentDataArray<FactionLoyaltyComponent>(Allocator.Temp);

            var commanderQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<CommanderComponent>(),
                ComponentType.ReadOnly<HealthComponent>());
            using var commanderEntities = commanderQuery.ToEntityArray(Allocator.Temp);
            using var commanderFactions = commanderQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var commanderComponents = commanderQuery.ToComponentDataArray<CommanderComponent>(Allocator.Temp);
            using var commanderHealths = commanderQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            foreach (var (settlement, fortification, reserve, faction, engagementRw, settlementEntity) in
                SystemAPI.Query<
                    RefRO<SettlementComponent>,
                    RefRO<FortificationComponent>,
                    RefRO<FortificationReserveComponent>,
                    RefRO<FactionComponent>,
                    RefRW<ImminentEngagementComponent>>()
                .WithEntityAccess())
            {
                var engagement = engagementRw.ValueRO;
                bool isPrimaryDynasticKeep = engagement.IsPrimaryDynasticKeep ||
                    entityManager.HasComponent<PrimaryKeepTag>(settlementEntity) ||
                    settlement.ValueRO.SettlementClassId.Equals(PrimaryDynasticKeepClassId);

                EnsureSelectedResponseDefaults(ref engagement);
                engagement.SettlementId = settlement.ValueRO.SettlementId;
                engagement.IsPrimaryDynasticKeep = isPrimaryDynasticKeep;

                if (fortification.ValueRO.Tier <= 0)
                {
                    if (engagement.Active || engagement.WindowConsumed)
                    {
                        engagement = ResetState(
                            settlement.ValueRO.SettlementId,
                            isPrimaryDynasticKeep,
                            engagement.SelectedResponseId,
                            engagement.CommanderRecallIssuedAt,
                            engagement.BloodlineProtectionUntil);
                    }

                    engagementRw.ValueRW = engagement;
                    continue;
                }

                ResolveLeadershipState(
                    entityManager,
                    faction.ValueRO.FactionId,
                    isPrimaryDynasticKeep,
                    factionEntities,
                    factionIds,
                    commanderEntities,
                    commanderFactions,
                    commanderComponents,
                    commanderHealths,
                    out bool commanderPresent,
                    out bool commanderRecallAvailable,
                    out bool governorPresent,
                    out bool bloodlineAtRisk);

                float localLoyalty = ResolveFactionLoyalty(
                    faction.ValueRO.FactionId,
                    factionIds,
                    factionLoyalties);

                bool profileActive = reserve.ValueRO.ThreatActive;

                // FortificationReserveComponent currently exposes only a binary
                // threat flag. When a seeded hostile count already exists on the
                // engagement state, preserve it; otherwise default to one hostile
                // while the threat flag is active.
                int hostileCount = profileActive ? math.max(engagement.HostileCount, 1) : 0;
                int hostileSiegeCount = profileActive ? math.max(0, engagement.HostileSiegeCount) : 0;
                int hostileScoutCount = profileActive ? math.max(0, engagement.HostileScoutCount) : 0;
                float warningSeconds = ComputeWarningSeconds(
                    isPrimaryDynasticKeep,
                    fortification.ValueRO.Tier,
                    math.max(0, engagement.WatchtowerCount));

                engagement.CommanderPresent = commanderPresent;
                engagement.CommanderRecallAvailable = commanderRecallAvailable;
                engagement.GovernorPresent = governorPresent;
                engagement.BloodlineAtRisk = bloodlineAtRisk;
                engagement.HostileCount = hostileCount;
                engagement.HostileSiegeCount = hostileSiegeCount;
                engagement.HostileScoutCount = hostileScoutCount;
                engagement.WarningRadius = fortification.ValueRO.ThreatRadiusTiles +
                    FortificationCanon.ImminentEngagementWarningBufferTiles;
                engagement.LocalLoyalty = localLoyalty;
                engagement.LocalLoyaltyMin = localLoyalty;

                if (!profileActive)
                {
                    if (engagement.Active || engagement.WindowConsumed)
                    {
                        engagement = ResetState(
                            settlement.ValueRO.SettlementId,
                            isPrimaryDynasticKeep,
                            engagement.SelectedResponseId,
                            engagement.CommanderRecallIssuedAt,
                            engagement.BloodlineProtectionUntil);
                    }

                    engagementRw.ValueRW = engagement;
                    continue;
                }

                if (!engagement.Active && !engagement.WindowConsumed)
                {
                    string responseId = ChooseResponse(engagement, reserve.ValueRO);
                    var posture = ImminentEngagementCanon.GetPosture(responseId);

                    engagement.Active = true;
                    engagement.WindowConsumed = false;
                    engagement.TotalSeconds = warningSeconds;
                    engagement.ExpiresAt = elapsedTime + warningSeconds;
                    engagement.RemainingSeconds = warningSeconds;
                    engagement.StartedAt = elapsedTime;
                    engagement.LastActivationAt = elapsedTime;
                    engagement.SelectedResponseId = responseId;
                    engagement.SelectedResponseLabel = posture.Label;
                }

                if (engagement.Active)
                {
                    string desiredResponseId = ChooseResponse(engagement, reserve.ValueRO);
                    if (!engagement.SelectedResponseId.Equals(desiredResponseId))
                    {
                        var posture = ImminentEngagementCanon.GetPosture(desiredResponseId);
                        engagement.SelectedResponseId = desiredResponseId;
                        engagement.SelectedResponseLabel = posture.Label;
                    }

                    engagement.RemainingSeconds = math.max(0f, engagement.ExpiresAt - elapsedTime);
                    if (engagement.RemainingSeconds <= 0f)
                    {
                        engagement.Active = false;
                        engagement.WindowConsumed = true;
                        engagement.EngagedAt = elapsedTime;
                        engagement.RemainingSeconds = 0f;
                        // TODO: autoSortieOnExpiry -- deferred to sortie system lane.
                    }
                }

                engagementRw.ValueRW = engagement;
            }
        }

        private static float ComputeWarningSeconds(
            bool isPrimaryDynasticKeep,
            int tier,
            int watchtowerCount)
        {
            float baseSeconds = isPrimaryDynasticKeep
                ? FortificationCanon.ImminentEngagementKeepBaseSeconds
                : FortificationCanon.ImminentEngagementSettlementBaseSeconds;
            return math.clamp(
                baseSeconds +
                (watchtowerCount * 2f) +
                (math.max(0, tier - 1) * 1.5f),
                FortificationCanon.ImminentEngagementMinSeconds,
                FortificationCanon.ImminentEngagementMaxSeconds);
        }

        private static void EnsureSelectedResponseDefaults(ref ImminentEngagementComponent engagement)
        {
            if (engagement.SelectedResponseId.IsEmpty)
            {
                engagement.SelectedResponseId = SteadyId;
            }

            if (engagement.SelectedResponseLabel.IsEmpty)
            {
                engagement.SelectedResponseLabel =
                    ImminentEngagementCanon.GetPosture(engagement.SelectedResponseId.ToString()).Label;
            }
        }

        private static void ResolveLeadershipState(
            EntityManager entityManager,
            FixedString32Bytes settlementFactionId,
            bool isPrimaryDynasticKeep,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factionIds,
            NativeArray<Entity> commanderEntities,
            NativeArray<FactionComponent> commanderFactions,
            NativeArray<CommanderComponent> commanderComponents,
            NativeArray<HealthComponent> commanderHealths,
            out bool commanderPresent,
            out bool commanderRecallAvailable,
            out bool governorPresent,
            out bool bloodlineAtRisk)
        {
            commanderPresent = false;
            commanderRecallAvailable = false;
            governorPresent = false;
            bloodlineAtRisk = false;

            bool anyCommanderAlive = false;
            bool commandingBloodlineAtKeep = false;
            for (int i = 0; i < commanderEntities.Length; i++)
            {
                if (commanderHealths[i].Current <= 0f ||
                    !commanderFactions[i].FactionId.Equals(settlementFactionId))
                {
                    continue;
                }

                anyCommanderAlive = true;
                if (!entityManager.HasComponent<CommanderAtKeepTag>(commanderEntities[i]))
                {
                    continue;
                }

                commanderPresent = true;
                FixedString32Bytes commanderRole = commanderComponents[i].Role;
                if (commanderRole.Equals(HeadOfBloodlineRole) ||
                    commanderRole.Equals(HeirDesignateRole) ||
                    commanderRole.Equals(CommanderRole))
                {
                    commandingBloodlineAtKeep = true;
                }
            }

            commanderRecallAvailable = anyCommanderAlive && !commanderPresent;

            Entity factionEntity = FindFactionDynastyEntity(
                entityManager,
                settlementFactionId,
                factionEntities,
                factionIds);
            if (factionEntity == Entity.Null)
            {
                bloodlineAtRisk = isPrimaryDynasticKeep && commandingBloodlineAtKeep;
                return;
            }

            bool dynasticSeatPresent = false;
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
                bool presentForSeat = member.Status == DynastyMemberStatus.Active ||
                    member.Status == DynastyMemberStatus.Ruling;

                if (isPrimaryDynasticKeep &&
                    member.Role == DynastyRole.HeadOfBloodline &&
                    presentForSeat)
                {
                    dynasticSeatPresent = true;
                }

                if (member.Role == DynastyRole.Governor && presentForSeat)
                {
                    governorPresent = true;
                }
            }

            bloodlineAtRisk = (isPrimaryDynasticKeep && dynasticSeatPresent) || commandingBloodlineAtKeep;
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

        private static float ResolveFactionLoyalty(
            FixedString32Bytes factionId,
            NativeArray<FactionComponent> factionIds,
            NativeArray<FactionLoyaltyComponent> factionLoyalties)
        {
            for (int i = 0; i < factionIds.Length; i++)
            {
                if (factionIds[i].FactionId.Equals(factionId))
                {
                    return factionLoyalties[i].Current;
                }
            }

            return 50f;
        }

        private static string ChooseResponse(
            in ImminentEngagementComponent engagement,
            in FortificationReserveComponent reserve)
        {
            if (engagement.BloodlineAtRisk ||
                engagement.HostileCount > math.max(1, reserve.ReadyReserveCount + 1))
            {
                return "brace";
            }

            if (engagement.CommanderPresent &&
                engagement.HostileCount <= math.max(3, reserve.ReadyReserveCount + 1))
            {
                return "counterstroke";
            }

            return "steady";
        }

        private static ImminentEngagementComponent ResetState(
            FixedString64Bytes settlementId,
            bool isPrimaryDynasticKeep,
            FixedString32Bytes selectedResponseId,
            float commanderRecallIssuedAt,
            float bloodlineProtectionUntil)
        {
            if (selectedResponseId.IsEmpty)
            {
                selectedResponseId = SteadyId;
            }

            var posture = ImminentEngagementCanon.GetPosture(selectedResponseId.ToString());
            return new ImminentEngagementComponent
            {
                SettlementId = settlementId,
                SelectedResponseId = selectedResponseId,
                SelectedResponseLabel = posture.Label,
                CommanderRecallIssuedAt = commanderRecallIssuedAt,
                BloodlineProtectionUntil = bloodlineProtectionUntil,
                IsPrimaryDynasticKeep = isPrimaryDynasticKeep,
                LocalLoyalty = 50f,
                LocalLoyaltyMin = 50f,
                LastActivationAt = -999f,
            };
        }
    }
}
