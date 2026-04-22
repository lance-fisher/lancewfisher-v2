using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Consolidates the per-faction dynasty renown HUD snapshots into a single
    /// ordered panel read-model for later UI binding.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(DynastyRenownHUDSystem))]
    public partial struct DynastyRenownLeaderboardHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 0.25f;
        private static readonly FixedString32Bytes PlayerFactionId = new("player");

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<DynastyRenownHUDComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            Entity singleton = EnsureSingleton(entityManager);

            var refresh = entityManager.GetComponentData<DynastyRenownLeaderboardHUDSingleton>(singleton);
            if (!ShouldRefresh(entityManager, refresh.LastRefreshInWorldDays, out float currentInWorldDays))
            {
                return;
            }

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyRenownHUDComponent>());
            using NativeArray<FactionComponent> factions = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<DynastyRenownHUDComponent> hudSnapshots =
                factionQuery.ToComponentDataArray<DynastyRenownHUDComponent>(Allocator.Temp);

            var candidates = new NativeArray<LeaderboardCandidate>(factions.Length, Allocator.Temp);
            try
            {
                int candidateCount = 0;
                for (int i = 0; i < factions.Length; i++)
                {
                    var snapshot = hudSnapshots[i];
                    candidates[candidateCount++] = new LeaderboardCandidate
                    {
                        FactionId = factions[i].FactionId,
                        RenownRank = snapshot.RenownRank,
                        RenownScore = snapshot.RenownScore,
                        PeakRenown = snapshot.PeakRenown,
                        IsHumanPlayer = factions[i].FactionId.Equals(PlayerFactionId),
                        IsLeadingDynasty = snapshot.IsLeadingDynasty,
                        Interregnum = snapshot.Interregnum,
                        RulerMemberId = snapshot.RulerMemberId,
                        RulerTitle = snapshot.RulerTitle,
                        BandLabel = snapshot.RenownBandLabel,
                        StatusLabel = snapshot.StatusLabel,
                        OriginalOrder = i,
                    };
                }

                SortCandidates(candidates, candidateCount);

                DynamicBuffer<DynastyRenownLeaderboardHUDComponent> leaderboard =
                    entityManager.GetBuffer<DynastyRenownLeaderboardHUDComponent>(singleton);
                leaderboard.Clear();
                for (int i = 0; i < candidateCount && i < 8; i++)
                {
                    leaderboard.Add(new DynastyRenownLeaderboardHUDComponent
                    {
                        FactionId = candidates[i].FactionId,
                        RenownRank = candidates[i].RenownRank,
                        RenownScore = candidates[i].RenownScore,
                        PeakRenown = candidates[i].PeakRenown,
                        IsHumanPlayer = candidates[i].IsHumanPlayer,
                        IsLeadingDynasty = candidates[i].IsLeadingDynasty,
                        Interregnum = candidates[i].Interregnum,
                        RulerMemberId = candidates[i].RulerMemberId,
                        RulerTitle = candidates[i].RulerTitle,
                        BandLabel = candidates[i].BandLabel,
                        StatusLabel = candidates[i].StatusLabel,
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
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDComponent>());
            if (!query.IsEmptyIgnoreFilter)
            {
                return query.GetSingletonEntity();
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            Entity entity = ecb.CreateEntity();
            ecb.AddComponent(entity, new DynastyRenownLeaderboardHUDSingleton
            {
                LastRefreshInWorldDays = float.NaN,
            });
            ecb.AddBuffer<DynastyRenownLeaderboardHUDComponent>(entity);
            ecb.Playback(entityManager);

            using var verifyQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDSingleton>(),
                ComponentType.ReadOnly<DynastyRenownLeaderboardHUDComponent>());
            return verifyQuery.GetSingletonEntity();
        }

        private static bool ShouldRefresh(
            EntityManager entityManager,
            float lastRefreshInWorldDays,
            out float currentInWorldDays)
        {
            currentInWorldDays = 0f;

            using var dualClockQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (dualClockQuery.IsEmptyIgnoreFilter)
            {
                return true;
            }

            currentInWorldDays = dualClockQuery.GetSingleton<DualClockComponent>().InWorldDays;
            return float.IsNaN(lastRefreshInWorldDays) ||
                   currentInWorldDays - lastRefreshInWorldDays >= RefreshCadenceInWorldDays;
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
            if (left.RenownScore > right.RenownScore + 0.0001f)
            {
                return -1;
            }

            if (right.RenownScore > left.RenownScore + 0.0001f)
            {
                return 1;
            }

            if (left.PeakRenown > right.PeakRenown + 0.0001f)
            {
                return -1;
            }

            if (right.PeakRenown > left.PeakRenown + 0.0001f)
            {
                return 1;
            }

            if (left.IsHumanPlayer != right.IsHumanPlayer)
            {
                return left.IsHumanPlayer ? -1 : 1;
            }

            if (left.RenownRank != right.RenownRank)
            {
                return left.RenownRank.CompareTo(right.RenownRank);
            }

            return left.OriginalOrder.CompareTo(right.OriginalOrder);
        }

        private struct LeaderboardCandidate
        {
            public FixedString32Bytes FactionId;
            public int RenownRank;
            public float RenownScore;
            public float PeakRenown;
            public bool IsHumanPlayer;
            public bool IsLeadingDynasty;
            public bool Interregnum;
            public FixedString64Bytes RulerMemberId;
            public FixedString64Bytes RulerTitle;
            public FixedString32Bytes BandLabel;
            public FixedString32Bytes StatusLabel;
            public int OriginalOrder;
        }
    }
}
