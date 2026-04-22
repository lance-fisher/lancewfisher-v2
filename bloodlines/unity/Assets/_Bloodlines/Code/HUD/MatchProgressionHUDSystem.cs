using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects the live match-progression singleton into a player-facing HUD snapshot.
    /// Runs in presentation so it reads settled simulation state after match and
    /// world-pressure evaluation have already completed.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct MatchProgressionHUDSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MatchProgressionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var progressionEntity = SystemAPI.GetSingletonEntity<MatchProgressionComponent>();
            if (!entityManager.HasComponent<MatchProgressionHUDComponent>(progressionEntity))
            {
                entityManager.AddComponentData(progressionEntity, default(MatchProgressionHUDComponent));
            }

            var progression = SystemAPI.GetSingleton<MatchProgressionComponent>();
            var pressureFactionId = progression.DominantKingdomId.Length > 0
                ? progression.DominantKingdomId
                : progression.GreatReckoningTargetFactionId;

            var hud = new MatchProgressionHUDComponent
            {
                StageNumber = progression.StageNumber,
                StageId = progression.StageId,
                StageLabel = progression.StageLabel,
                PhaseId = progression.PhaseId,
                PhaseLabel = progression.PhaseLabel,
                StageReadiness = progression.StageReadiness,
                NextStageId = progression.NextStageId,
                NextStageLabel = progression.NextStageLabel,
                InWorldDays = progression.InWorldDays,
                InWorldYears = progression.InWorldYears,
                DeclarationCount = progression.DeclarationCount,
                WorldPressureLevel = 0,
                WorldPressureLabel = new FixedString32Bytes("quiet"),
                WorldPressureScore = 0,
                WorldPressureTargeted = false,
                DominantLeaderFactionId = progression.DominantKingdomId,
                DominantLeaderTerritoryShare = progression.DominantTerritoryShare,
                GreatReckoningActive = progression.GreatReckoningActive,
                GreatReckoningTargetFactionId = progression.GreatReckoningTargetFactionId,
                GreatReckoningShare = progression.GreatReckoningShare,
            };

            if (pressureFactionId.Length > 0)
            {
                foreach (var (faction, pressure) in SystemAPI.Query<
                             RefRO<FactionComponent>,
                             RefRO<WorldPressureComponent>>())
                {
                    if (faction.ValueRO.FactionId != pressureFactionId)
                    {
                        continue;
                    }

                    hud.WorldPressureLevel = pressure.ValueRO.Level;
                    hud.WorldPressureLabel = pressure.ValueRO.Label;
                    hud.WorldPressureScore = pressure.ValueRO.Score;
                    hud.WorldPressureTargeted = pressure.ValueRO.Targeted;
                    break;
                }
            }

            entityManager.SetComponentData(progressionEntity, hud);
        }
    }
}
