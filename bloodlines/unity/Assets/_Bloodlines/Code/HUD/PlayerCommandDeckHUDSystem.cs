using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Produces a compact player command-deck summary by consuming the already
    /// settled HUD read-models owned by this lane.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(MatchProgressionHUDSystem))]
    [UpdateAfter(typeof(VictoryLeaderboardHUDSystem))]
    [UpdateAfter(typeof(DynastyRenownLeaderboardHUDSystem))]
    public partial struct PlayerCommandDeckHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 0.25f;
        private static readonly FixedString32Bytes PlayerFactionId = new("player");
        private static readonly FixedString32Bytes StableAlert = new("stable");
        private static readonly FixedString32Bytes GreatReckoningAlert = new("great_reckoning");
        private static readonly FixedString32Bytes FortificationThreatAlert = new("fortification_threat");
        private static readonly FixedString32Bytes LoyaltyCrisisAlert = new("loyalty_crisis");
        private static readonly FixedString32Bytes VictoryImminentAlert = new("victory_imminent");
        private static readonly FixedString32Bytes WorldPressureAlert = new("world_pressure");
        private static readonly FixedString32Bytes RedBand = new("red");

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<RealmConditionHUDComponent>();
            state.RequireForUpdate<MatchProgressionHUDComponent>();
            state.RequireForUpdate<DynastyRenownHUDComponent>();
            state.RequireForUpdate<VictoryConditionReadoutComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            Entity playerFactionEntity = FindPlayerFactionEntity(entityManager);
            if (playerFactionEntity == Entity.Null)
            {
                return;
            }

            if (!entityManager.HasComponent<PlayerCommandDeckHUDComponent>(playerFactionEntity))
            {
                entityManager.AddComponentData(playerFactionEntity, new PlayerCommandDeckHUDComponent
                {
                    FactionId = PlayerFactionId,
                    LastRefreshInWorldDays = float.NaN,
                    LeadingVictoryEtaInWorldDays = float.NaN,
                    PrimaryAlertLabel = StableAlert,
                });
            }

            var snapshot = entityManager.GetComponentData<PlayerCommandDeckHUDComponent>(playerFactionEntity);
            float currentInWorldDays = ResolveCurrentInWorldDays(entityManager);
            if (!float.IsNaN(snapshot.LastRefreshInWorldDays) &&
                currentInWorldDays - snapshot.LastRefreshInWorldDays < RefreshCadenceInWorldDays)
            {
                return;
            }

            if (!entityManager.HasComponent<RealmConditionHUDComponent>(playerFactionEntity) ||
                !entityManager.HasComponent<DynastyRenownHUDComponent>(playerFactionEntity) ||
                !entityManager.HasBuffer<VictoryConditionReadoutComponent>(playerFactionEntity))
            {
                return;
            }

            using var matchQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MatchProgressionHUDComponent>());
            if (matchQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            var match = matchQuery.GetSingleton<MatchProgressionHUDComponent>();
            var realm = entityManager.GetComponentData<RealmConditionHUDComponent>(playerFactionEntity);
            var renown = entityManager.GetComponentData<DynastyRenownHUDComponent>(playerFactionEntity);
            DynamicBuffer<VictoryConditionReadoutComponent> readout = entityManager.GetBuffer<VictoryConditionReadoutComponent>(playerFactionEntity);

            ResolveLeadingVictory(readout, out var victoryConditionId, out float victoryProgressPct, out float victoryEtaDays);
            ResolveVictoryLeaderboard(entityManager, out int victoryRank, out var victoryLeaderFactionId);
            ResolveRenownLeaderboard(entityManager, out int renownRank);
            bool fortificationThreatActive = ResolveFortificationThreat(entityManager);

            snapshot.FactionId = PlayerFactionId;
            snapshot.LastRefreshInWorldDays = currentInWorldDays;
            snapshot.StageLabel = match.StageLabel;
            snapshot.PhaseLabel = match.PhaseLabel;
            snapshot.WorldPressureLabel = match.WorldPressureLabel;
            snapshot.WorldPressureLevel = match.WorldPressureLevel;
            snapshot.GreatReckoningActive = match.GreatReckoningActive;
            snapshot.LeadingVictoryConditionId = victoryConditionId;
            snapshot.LeadingVictoryProgressPct = victoryProgressPct;
            snapshot.LeadingVictoryEtaInWorldDays = victoryEtaDays;
            snapshot.VictoryRank = victoryRank;
            snapshot.VictoryLeaderFactionId = victoryLeaderFactionId;
            snapshot.RenownRank = renownRank > 0 ? renownRank : renown.RenownRank;
            snapshot.RenownScore = renown.RenownScore;
            snapshot.RenownBandLabel = renown.RenownBandLabel;
            snapshot.PopulationBand = realm.PopulationBand;
            snapshot.LoyaltyBand = realm.LoyaltyBand;
            snapshot.FaithBand = realm.FaithBand;
            snapshot.FortificationThreatActive = fortificationThreatActive;
            snapshot.PrimaryAlertLabel = ResolvePrimaryAlert(
                match,
                realm,
                victoryProgressPct,
                victoryRank,
                fortificationThreatActive);

            entityManager.SetComponentData(playerFactionEntity, snapshot);
        }

        private static Entity FindPlayerFactionEntity(EntityManager entityManager)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(PlayerFactionId))
                {
                    return entities[i];
                }
            }

            return Entity.Null;
        }

        private static float ResolveCurrentInWorldDays(EntityManager entityManager)
        {
            using var dualClockQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (!dualClockQuery.IsEmptyIgnoreFilter)
            {
                return dualClockQuery.GetSingleton<DualClockComponent>().InWorldDays;
            }

            using var matchQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MatchProgressionHUDComponent>());
            if (!matchQuery.IsEmptyIgnoreFilter)
            {
                return matchQuery.GetSingleton<MatchProgressionHUDComponent>().InWorldDays;
            }

            return 0f;
        }

        private static void ResolveLeadingVictory(
            DynamicBuffer<VictoryConditionReadoutComponent> readout,
            out FixedString32Bytes conditionId,
            out float progressPct,
            out float etaDays)
        {
            conditionId = default;
            progressPct = 0f;
            etaDays = float.NaN;

            for (int i = 0; i < readout.Length; i++)
            {
                var candidate = readout[i];
                if (candidate.ProgressPct <= progressPct + 0.0001f)
                {
                    continue;
                }

                conditionId = candidate.ConditionId;
                progressPct = candidate.ProgressPct;
                etaDays = candidate.TimeRemainingEstimateInWorldDays;
            }
        }

        private static void ResolveVictoryLeaderboard(
            EntityManager entityManager,
            out int victoryRank,
            out FixedString32Bytes victoryLeaderFactionId)
        {
            victoryRank = 0;
            victoryLeaderFactionId = default;

            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<VictoryLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<VictoryLeaderboardHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return;
            }

            DynamicBuffer<VictoryLeaderboardHUDComponent> buffer =
                entityManager.GetBuffer<VictoryLeaderboardHUDComponent>(query.GetSingletonEntity());
            if (buffer.Length > 0)
            {
                victoryLeaderFactionId = buffer[0].FactionId;
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].FactionId.Equals(PlayerFactionId))
                {
                    victoryRank = i + 1;
                    return;
                }
            }
        }

        private static void ResolveRenownLeaderboard(EntityManager entityManager, out int renownRank)
        {
            renownRank = 0;

            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return;
            }

            DynamicBuffer<DynastyRenownLeaderboardHUDComponent> buffer =
                entityManager.GetBuffer<DynastyRenownLeaderboardHUDComponent>(query.GetSingletonEntity());
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].FactionId.Equals(PlayerFactionId))
                {
                    renownRank = i + 1;
                    return;
                }
            }
        }

        private static bool ResolveFortificationThreat(EntityManager entityManager)
        {
            using var query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FortificationHUDComponent>());
            using NativeArray<FortificationHUDComponent> fortifications = query.ToComponentDataArray<FortificationHUDComponent>(Allocator.Temp);
            for (int i = 0; i < fortifications.Length; i++)
            {
                if (fortifications[i].OwnerFactionId.Equals(PlayerFactionId) &&
                    fortifications[i].ThreatActive)
                {
                    return true;
                }
            }

            return false;
        }

        private static FixedString32Bytes ResolvePrimaryAlert(
            in MatchProgressionHUDComponent match,
            in RealmConditionHUDComponent realm,
            float victoryProgressPct,
            int victoryRank,
            bool fortificationThreatActive)
        {
            if (match.GreatReckoningActive)
            {
                return GreatReckoningAlert;
            }

            if (fortificationThreatActive)
            {
                return FortificationThreatAlert;
            }

            if (realm.LoyaltyBand.Equals(RedBand))
            {
                return LoyaltyCrisisAlert;
            }

            if (victoryRank == 1 && victoryProgressPct >= 0.85f)
            {
                return VictoryImminentAlert;
            }

            if (match.WorldPressureLevel >= 6)
            {
                return WorldPressureAlert;
            }

            return StableAlert;
        }
    }
}
