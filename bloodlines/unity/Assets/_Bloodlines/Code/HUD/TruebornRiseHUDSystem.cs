using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Mirrors the shared Trueborn rise-arc singleton plus per-faction recognition
    /// state into a compact kingdom-scoped HUD read-model.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct TruebornRiseHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 1f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<TruebornRiseArcComponent>();
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<FactionKindComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (faction, factionKind, entity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<FactionKindComponent>>().WithEntityAccess())
            {
                if (factionKind.ValueRO.Kind != FactionKind.Kingdom ||
                    entityManager.HasComponent<TruebornRiseHUDComponent>(entity))
                {
                    continue;
                }

                ecb.AddComponent(entity, CreateDefaultHud(faction.ValueRO.FactionId));
            }

            ecb.Playback(entityManager);

            var arc = SystemAPI.GetSingleton<TruebornRiseArcComponent>();
            Entity arcEntity = SystemAPI.GetSingletonEntity<TruebornRiseArcComponent>();
            var recognitionSlots =
                entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);
            int recognizedCount = TruebornRecognitionUtility.CountRecognized(
                recognitionSlots,
                arc.RecognizedFactionsBitmask);

            foreach (var (faction, factionKind, hudRw) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<FactionKindComponent>, RefRW<TruebornRiseHUDComponent>>())
            {
                if (factionKind.ValueRO.Kind != FactionKind.Kingdom)
                {
                    continue;
                }

                ref var hud = ref hudRw.ValueRW;
                if (!float.IsNaN(hud.LastRefreshInWorldDays) &&
                    inWorldDays - hud.LastRefreshInWorldDays < RefreshCadenceInWorldDays)
                {
                    continue;
                }

                hud = CreateDefaultHud(faction.ValueRO.FactionId);
                hud.LastRefreshInWorldDays = inWorldDays;
                hud.CurrentStage = arc.CurrentStage;
                hud.StageLabel = ResolveStageLabel(arc.CurrentStage);
                hud.RiseActive = arc.CurrentStage > 0;
                hud.Recognized = TruebornRecognitionUtility.IsRecognized(
                    recognitionSlots,
                    arc.RecognizedFactionsBitmask,
                    faction.ValueRO.FactionId);
                hud.RecognizedFactionCount = recognizedCount;
                hud.ChallengeLevel = arc.ChallengeLevel;
                hud.GlobalPressurePerDay = arc.GlobalPressurePerDay;
                hud.LoyaltyErosionPerDay = arc.LoyaltyErosionPerDay;
            }
        }

        private static TruebornRiseHUDComponent CreateDefaultHud(FixedString32Bytes factionId)
        {
            return new TruebornRiseHUDComponent
            {
                FactionId = factionId,
                LastRefreshInWorldDays = float.NaN,
                CurrentStage = 0,
                StageLabel = new FixedString64Bytes("Dormant"),
                RiseActive = false,
                Recognized = false,
                RecognizedFactionCount = 0,
                ChallengeLevel = 0,
                GlobalPressurePerDay = 0f,
                LoyaltyErosionPerDay = 0f,
            };
        }

        private static FixedString64Bytes ResolveStageLabel(byte stage)
        {
            switch (stage)
            {
                case 1:
                    return new FixedString64Bytes("Banner Raised");
                case 2:
                    return new FixedString64Bytes("Escalation");
                case 3:
                    return new FixedString64Bytes("Restoration");
                default:
                    return new FixedString64Bytes("Dormant");
            }
        }
    }
}
