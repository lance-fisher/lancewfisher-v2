using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Skirmish;
using Bloodlines.Victory;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Drives the canonical skirmish phase lifecycle: Setup → Playing → PostMatch.
    ///
    /// Setup → Playing: fires when bootstrap is complete (no MapBootstrapPendingTag entities
    ///   remain) and VictoryStateComponent has been seeded. Records FactionCount,
    ///   IsPlayerVsAI (any Kingdom faction carries AIEconomyControllerComponent),
    ///   PlayerFactionId (first Kingdom faction without AI controller), and MatchStartInWorldDays.
    ///
    /// Playing → PostMatch: fires when MatchEndStateComponent.IsMatchEnded is true.
    ///   Records MatchEndInWorldDays from the MatchEndStateComponent.
    ///
    /// Browser equivalent: absent -- match-phase lifecycle is implicit in simulation.js
    /// initialization and victory state. This system makes it explicit for HUD and persistence.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MatchEndSequenceSystem))]
    public partial struct SkirmishGameModeManagerSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            if (!SystemAPI.HasSingleton<SkirmishGameModeComponent>())
            {
                var e = em.CreateEntity();
                em.AddComponentData(e, new SkirmishGameModeComponent { Phase = SkirmishPhase.Setup });
            }

            var modeEntity = SystemAPI.GetSingletonEntity<SkirmishGameModeComponent>();
            var mode = em.GetComponentData<SkirmishGameModeComponent>(modeEntity);

            switch (mode.Phase)
            {
                case SkirmishPhase.Setup:
                    TryTransitionToPlaying(em, modeEntity, ref mode);
                    break;
                case SkirmishPhase.Playing:
                    TryTransitionToPostMatch(em, modeEntity, ref mode);
                    break;
            }
        }

        static void TryTransitionToPlaying(EntityManager em, Entity modeEntity, ref SkirmishGameModeComponent mode)
        {
            using var pendingQuery = em.CreateEntityQuery(ComponentType.ReadOnly<MapBootstrapPendingTag>());
            if (!pendingQuery.IsEmptyIgnoreFilter)
                return;

            using var victoryQuery = em.CreateEntityQuery(ComponentType.ReadOnly<VictoryStateComponent>());
            if (victoryQuery.IsEmptyIgnoreFilter)
                return;

            using var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>());
            using var factionEntities   = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionComponents = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var kindComponents    = factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);

            byte kingdomCount = 0;
            bool hasAI = false;
            FixedString32Bytes playerFactionId = default;

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (kindComponents[i].Kind != FactionKind.Kingdom)
                    continue;
                kingdomCount++;
                if (em.HasComponent<AIEconomyControllerComponent>(factionEntities[i]))
                    hasAI = true;
                else
                    playerFactionId = factionComponents[i].FactionId;
            }

            float inWorldDays = 0f;
            using var clockQuery = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (!clockQuery.IsEmptyIgnoreFilter)
                inWorldDays = clockQuery.GetSingleton<DualClockComponent>().InWorldDays;

            mode.Phase = SkirmishPhase.Playing;
            mode.FactionCount = kingdomCount;
            mode.IsPlayerVsAI = hasAI;
            mode.PlayerFactionId = playerFactionId;
            mode.MatchStartInWorldDays = inWorldDays;
            em.SetComponentData(modeEntity, mode);
        }

        static void TryTransitionToPostMatch(EntityManager em, Entity modeEntity, ref SkirmishGameModeComponent mode)
        {
            using var matchEndQuery = em.CreateEntityQuery(ComponentType.ReadOnly<MatchEndStateComponent>());
            if (matchEndQuery.IsEmptyIgnoreFilter)
                return;

            var matchEnd = matchEndQuery.GetSingleton<MatchEndStateComponent>();
            if (!matchEnd.IsMatchEnded)
                return;

            mode.Phase = SkirmishPhase.PostMatch;
            mode.MatchEndInWorldDays = matchEnd.MatchEndTimeInWorldDays;
            em.SetComponentData(modeEntity, mode);
        }
    }
}
