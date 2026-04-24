using Bloodlines.Victory;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Reads MatchEndStateComponent and writes MatchEndHUDComponent so the
    /// match result screen has a stable, display-ready data source.
    ///
    /// Browser equivalent: absent -- match-end result screen not in simulation.js.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MatchEndSequenceSystem))]
    public partial struct MatchEndHUDSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MatchEndStateComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var matchEnd = SystemAPI.GetSingleton<MatchEndStateComponent>();
            if (!matchEnd.IsMatchEnded)
                return;

            var em = state.EntityManager;

            using var hudQuery = em.CreateEntityQuery(ComponentType.ReadWrite<MatchEndHUDComponent>());
            if (hudQuery.IsEmptyIgnoreFilter)
            {
                em.CreateEntity(typeof(MatchEndHUDComponent));
            }

            using var writeQuery = em.CreateEntityQuery(ComponentType.ReadWrite<MatchEndHUDComponent>());
            var entity = writeQuery.GetSingletonEntity();
            em.SetComponentData(entity, new MatchEndHUDComponent
            {
                IsVisible = true,
                WinnerFactionId = matchEnd.WinnerFactionId,
                VictoryType = matchEnd.VictoryType,
                VictoryReason = matchEnd.VictoryReason,
                MatchEndTimeInWorldDays = matchEnd.MatchEndTimeInWorldDays,
                PlayerXPAwarded = matchEnd.XPAwarded,
            });
        }
    }
}
