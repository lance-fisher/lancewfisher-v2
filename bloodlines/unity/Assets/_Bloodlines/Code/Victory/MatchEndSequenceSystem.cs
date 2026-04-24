using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Victory
{
    /// <summary>
    /// Reacts the first frame VictoryStateComponent.Status != Playing.
    /// On that frame:
    ///   1. Creates the MatchEndStateComponent singleton recording the result.
    ///   2. Places DynastyXPAwardRequestComponent on every faction entity so
    ///      DynastyXPAwardSystem awards dynasty XP for this match.
    ///      - Winner:    150 XP, placement 1
    ///      - Runner-up: 75 XP,  placement 2  (faction with second-most territory)
    ///      - Others:    25 XP,  placement 3+
    ///   3. Pushes a narrative message via NarrativeMessageBridge.
    ///
    /// The fired guard prevents any repeat firing even if victory state is
    /// re-queried, following the once-on-event pattern used across the codebase.
    ///
    /// Browser equivalent: absent -- match-end result screen not in simulation.js.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(VictoryConditionEvaluationSystem))]
    public partial struct MatchEndSequenceSystem : ISystem
    {
        private bool _fired;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<VictoryStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (_fired)
                return;

            var victory = SystemAPI.GetSingleton<VictoryStateComponent>();
            if (victory.Status == MatchStatus.Playing)
                return;

            _fired = true;

            var em = state.EntityManager;

            // Read current in-world time.
            float inWorldDays = 0f;
            using (var clockQuery = em.CreateEntityQuery(ComponentType.ReadOnly<GameTime.DualClockComponent>()))
            {
                if (!clockQuery.IsEmptyIgnoreFilter)
                    inWorldDays = clockQuery.GetSingleton<GameTime.DualClockComponent>().InWorldDays;
            }

            // Build faction territory counts to determine placement order.
            using var cpQuery = em.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using var cpData = cpQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            // Count territory per faction.
            var territoryCounts = new NativeHashMap<FixedString32Bytes, int>(8, Allocator.Temp);
            for (int i = 0; i < cpData.Length; i++)
            {
                var owner = cpData[i].OwnerFactionId;
                if (owner.Length == 0) continue;
                territoryCounts.TryGetValue(owner, out int current);
                territoryCounts[owner] = current + 1;
            }

            // Find all faction entities and their territory counts.
            using var factionQuery = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionComponents = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            // Determine winner and runner-up by territory (winner is already known from victory state).
            FixedString32Bytes winnerFactionId = victory.WinnerFactionId;
            int runnerUpTerritory = -1;
            FixedString32Bytes runnerUpFactionId = default;

            for (int i = 0; i < factionComponents.Length; i++)
            {
                var fid = factionComponents[i].FactionId;
                if (fid.Equals(winnerFactionId)) continue;
                territoryCounts.TryGetValue(fid, out int count);
                if (count > runnerUpTerritory)
                {
                    runnerUpTerritory = count;
                    runnerUpFactionId = fid;
                }
            }

            // Place XP award requests.
            byte placement = 3;
            for (int i = 0; i < factionEntities.Length; i++)
            {
                Entity e = factionEntities[i];
                var fid = factionComponents[i].FactionId;

                float xp;
                if (fid.Equals(winnerFactionId))
                {
                    xp = 150f;
                    placement = 1;
                }
                else if (fid.Equals(runnerUpFactionId))
                {
                    xp = 75f;
                    placement = 2;
                }
                else
                {
                    xp = 25f;
                    placement = 3;
                }

                if (em.HasComponent<DynastyXPAwardRequestComponent>(e))
                    em.RemoveComponent<DynastyXPAwardRequestComponent>(e);

                em.AddComponentData(e, new DynastyXPAwardRequestComponent
                {
                    XPAmount = xp,
                    MatchPlacement = placement,
                });
            }

            territoryCounts.Dispose();

            // Create MatchEndStateComponent singleton.
            Entity matchEndEntity = em.CreateEntity(typeof(MatchEndStateComponent));
            em.SetComponentData(matchEndEntity, new MatchEndStateComponent
            {
                IsMatchEnded = true,
                WinnerFactionId = winnerFactionId,
                VictoryType = victory.VictoryType,
                VictoryReason = victory.VictoryReason,
                MatchEndTimeInWorldDays = inWorldDays,
                XPAwarded = true,
            });

            // Push narrative message.
            bool playerWon = victory.Status == MatchStatus.Won;
            var messageText = playerWon
                ? new FixedString128Bytes("Victory! Your dynasty's legacy endures.")
                : new FixedString128Bytes("Defeat. Your dynasty must rebuild.");
            var messageTone = playerWon ? NarrativeMessageTone.Good : NarrativeMessageTone.Warn;
            NarrativeMessageBridge.Push(em, messageText, messageTone);
        }
    }
}
