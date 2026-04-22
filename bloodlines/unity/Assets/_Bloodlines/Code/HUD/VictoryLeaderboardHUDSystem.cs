using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Consolidates per-faction victory readout buffers into a single ordered HUD
    /// leaderboard without mutating the source victory read models.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(VictoryConditionReadoutSystem))]
    public partial struct VictoryLeaderboardHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 0.25f;
        private static readonly FixedString32Bytes PlayerFactionId = new FixedString32Bytes("player");

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<VictoryConditionReadoutComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var singleton = EnsureSingleton(entityManager);

            var refresh = entityManager.GetComponentData<VictoryLeaderboardHUDSingleton>(singleton);
            if (!ShouldRefresh(entityManager, refresh.LastRefreshInWorldDays, out float currentInWorldDays))
            {
                return;
            }

            using var factionQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var readoutLookup = SystemAPI.GetBufferLookup<VictoryConditionReadoutComponent>(true);

            var candidates = new NativeArray<LeaderboardCandidate>(factionEntities.Length, Allocator.Temp);
            try
            {
                int candidateCount = 0;
                for (int i = 0; i < factionEntities.Length; i++)
                {
                    if (!readoutLookup.HasBuffer(factionEntities[i]))
                    {
                        continue;
                    }

                    DynamicBuffer<VictoryConditionReadoutComponent> readout = readoutLookup[factionEntities[i]];
                    if (!TryResolveBestEntry(readout, out var bestConditionId, out float bestProgress))
                    {
                        continue;
                    }

                    candidates[candidateCount++] = new LeaderboardCandidate
                    {
                        FactionId = factions[i].FactionId,
                        LeadingConditionId = bestConditionId,
                        ProgressPct = bestProgress,
                        IsHumanPlayer = factions[i].FactionId.Equals(PlayerFactionId),
                        OriginalOrder = i,
                    };
                }

                SortCandidates(candidates, candidateCount);

                DynamicBuffer<VictoryLeaderboardHUDComponent> leaderboard =
                    entityManager.GetBuffer<VictoryLeaderboardHUDComponent>(singleton);
                leaderboard.Clear();
                for (int i = 0; i < candidateCount && i < 8; i++)
                {
                    leaderboard.Add(new VictoryLeaderboardHUDComponent
                    {
                        FactionId = candidates[i].FactionId,
                        LeadingConditionId = candidates[i].LeadingConditionId,
                        ProgressPct = candidates[i].ProgressPct,
                        IsHumanPlayer = candidates[i].IsHumanPlayer,
                    });
                }
            }
            finally
            {
                candidates.Dispose();
            }

            refresh.LastRefreshInWorldDays = currentInWorldDays;
            entityManager.SetComponentData(singleton, refresh);
        }

        private static Entity EnsureSingleton(EntityManager entityManager)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<VictoryLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<VictoryLeaderboardHUDComponent>());
            if (!query.IsEmptyIgnoreFilter)
            {
                return query.GetSingletonEntity();
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entity = ecb.CreateEntity();
            ecb.AddComponent(entity, new VictoryLeaderboardHUDSingleton
            {
                LastRefreshInWorldDays = float.NaN,
            });
            ecb.AddBuffer<VictoryLeaderboardHUDComponent>(entity);
            ecb.Playback(entityManager);

            using var verifyQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<VictoryLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<VictoryLeaderboardHUDComponent>());
            return verifyQuery.GetSingletonEntity();
        }

        private static bool ShouldRefresh(
            EntityManager entityManager,
            float lastRefreshInWorldDays,
            out float currentInWorldDays)
        {
            currentInWorldDays = 0f;

            using var dualClockQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (!dualClockQuery.IsEmptyIgnoreFilter)
            {
                currentInWorldDays = dualClockQuery.GetSingleton<DualClockComponent>().InWorldDays;
                return float.IsNaN(lastRefreshInWorldDays) ||
                       currentInWorldDays - lastRefreshInWorldDays >= RefreshCadenceInWorldDays;
            }

            using var matchQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<MatchProgressionComponent>());
            if (!matchQuery.IsEmptyIgnoreFilter)
            {
                currentInWorldDays = matchQuery.GetSingleton<MatchProgressionComponent>().InWorldDays;
                return float.IsNaN(lastRefreshInWorldDays) ||
                       currentInWorldDays - lastRefreshInWorldDays >= RefreshCadenceInWorldDays;
            }

            return true;
        }

        private static bool TryResolveBestEntry(
            DynamicBuffer<VictoryConditionReadoutComponent> readout,
            out FixedString32Bytes bestConditionId,
            out float bestProgress)
        {
            bestConditionId = default;
            bestProgress = 0f;

            if (readout.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < readout.Length; i++)
            {
                var entry = readout[i];
                if (i == 0 ||
                    entry.ProgressPct > bestProgress + 0.0001f ||
                    (math.abs(entry.ProgressPct - bestProgress) <= 0.0001f && entry.IsLeading))
                {
                    bestConditionId = entry.ConditionId;
                    bestProgress = entry.ProgressPct;
                }
            }

            return true;
        }

        private static void SortCandidates(NativeArray<LeaderboardCandidate> candidates, int count)
        {
            for (int i = 1; i < count; i++)
            {
                var candidate = candidates[i];
                int j = i - 1;
                while (j >= 0 && Compare(candidate, candidates[j]) < 0)
                {
                    candidates[j + 1] = candidates[j];
                    j--;
                }

                candidates[j + 1] = candidate;
            }
        }

        private static int Compare(in LeaderboardCandidate left, in LeaderboardCandidate right)
        {
            if (left.ProgressPct > right.ProgressPct + 0.0001f)
            {
                return -1;
            }

            if (right.ProgressPct > left.ProgressPct + 0.0001f)
            {
                return 1;
            }

            if (left.IsHumanPlayer != right.IsHumanPlayer)
            {
                return left.IsHumanPlayer ? -1 : 1;
            }

            return left.OriginalOrder.CompareTo(right.OriginalOrder);
        }

        private struct LeaderboardCandidate
        {
            public FixedString32Bytes FactionId;
            public FixedString32Bytes LeadingConditionId;
            public float ProgressPct;
            public bool IsHumanPlayer;
            public int OriginalOrder;
        }
    }
}
