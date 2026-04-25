using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Resolves militia squad duty state from their current assignment.
    ///
    /// Rule:
    ///   SquadAssignmentType.None → DutyState.Reserve (squad stands down)
    ///   Any other assignment     → DutyState.ActiveDuty
    ///
    /// Active assignments (all are militia-squad orders, no separate Scout unit):
    ///   Guard, Scout, Patrol, Attack, Escort, HoldPosition,
    ///   DefendKeep, DefendWoodcutterCamp, DefendForagerCamp
    ///
    /// Scouting is SquadAssignmentType.Scout on a militia squad. There is no
    /// dedicated Scout unit (owner direction 2026-04-25).
    ///
    /// Productivity implication (driven by PopulationProductivitySystem):
    ///   Reserve:    50% base productivity
    ///   ActiveDuty:  5% base productivity
    ///
    /// This system only maintains DutyState. Movement, targeting, and combat behavior
    /// for each assignment type are implemented in their own systems (future slices).
    /// Stub: assignment orders are currently set externally (debug surface / player input
    /// bridge — not yet wired). System runs and transitions state correctly once set.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(MilitaryDraftSystem))]
    public partial struct SquadDutySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var squadRw in SystemAPI.Query<RefRW<MilitiaSquadComponent>>())
            {
                ref var squad = ref squadRw.ValueRW;

                squad.DutyState = squad.AssignmentType == SquadAssignmentType.None
                    ? DutyState.Reserve
                    : DutyState.ActiveDuty;
            }
        }
    }
}
