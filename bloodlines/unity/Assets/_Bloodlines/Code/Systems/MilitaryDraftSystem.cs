using Bloodlines.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Maintains per-faction military draft state each frame.
    ///
    /// Responsibilities:
    ///   1. Clamps DraftRatePct to valid range (0–100, multiples of 5).
    ///   2. Derives DraftPool from Total population × DraftRatePct.
    ///   3. Counts trained squads per faction (queries MilitiaSquadComponent).
    ///   4. Splits trained military into Reserve and ActiveDuty counts.
    ///   5. Derives UntrainedDrafted = max(0, DraftPool − TrainedMilitary).
    ///   6. Sets OverDraftedFlag when trained > DraftPool.
    ///
    /// Does NOT modify DraftRatePct (that is player/AI input).
    /// Does NOT train squads (that is UnitProductionSystem's job).
    ///
    /// SquadSize is canonical 5; fractional remainders are floored.
    ///
    /// Canon: early-game-foundation prompt 2026-04-25, Utopia-inspired draft model.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(PopulationProductivitySystem))]
    public partial struct MilitaryDraftSystem : ISystem
    {
        // Per-faction trained squad counts. Built from MilitiaSquadComponent query.
        // Key = factionId hash, Value = (totalSquads, reserveSquads, activeDutySquads)
        struct SquadCounts
        {
            public int Total;
            public int Reserve;
            public int ActiveDuty;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Tally squads per faction.
            using var squadCounts = new NativeHashMap<int, SquadCounts>(16, Allocator.Temp);

            foreach (var (faction, squad) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<MilitiaSquadComponent>>())
            {
                int key = faction.ValueRO.FactionId.GetHashCode();
                squadCounts.TryGetValue(key, out var counts);

                counts.Total++;
                if (squad.ValueRO.DutyState == DutyState.Reserve)
                    counts.Reserve++;
                else
                    counts.ActiveDuty++;

                squadCounts.Remove(key);
                squadCounts.Add(key, counts);
            }

            // Update draft state on each faction entity.
            foreach (var (draftRw, population, faction) in
                SystemAPI.Query<
                    RefRW<MilitaryDraftComponent>,
                    RefRO<PopulationComponent>,
                    RefRO<FactionComponent>>())
            {
                ref var draft = ref draftRw.ValueRW;

                // Clamp and snap rate to multiples of step size.
                draft.DraftRatePct = math.clamp(
                    (draft.DraftRatePct / EarlyGameConstants.DraftStepSize) * EarlyGameConstants.DraftStepSize,
                    EarlyGameConstants.DraftMin,
                    EarlyGameConstants.DraftMax);

                int total = population.ValueRO.Total;
                draft.DraftPool = (total * draft.DraftRatePct) / 100;

                int key = faction.ValueRO.FactionId.GetHashCode();
                squadCounts.TryGetValue(key, out var counts);

                int trainedMilitary = counts.Total * EarlyGameConstants.SquadSize;
                draft.TrainedMilitary    = trainedMilitary;
                draft.ReserveMilitary    = counts.Reserve   * EarlyGameConstants.SquadSize;
                draft.ActiveDutyMilitary = counts.ActiveDuty * EarlyGameConstants.SquadSize;
                draft.UntrainedDrafted   = math.max(0, draft.DraftPool - trainedMilitary);
                draft.OverDraftedFlag    = trainedMilitary > draft.DraftPool;
            }
        }
    }
}
