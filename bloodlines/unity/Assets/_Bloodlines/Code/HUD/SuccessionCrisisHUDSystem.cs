using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Mirrors active succession-crisis severity and recovery into a lightweight
    /// per-faction HUD badge without mutating the dynasty runtime seam.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct SuccessionCrisisHUDSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (faction, entity) in
                SystemAPI.Query<RefRO<FactionComponent>>().WithEntityAccess())
            {
                if (!entityManager.HasComponent<SuccessionCrisisHUDComponent>(entity))
                {
                    ecb.AddComponent(entity, CreateDefaultHud(faction.ValueRO.FactionId));
                }
            }

            ecb.Playback(entityManager);

            foreach (var (faction, hudRw, entity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRW<SuccessionCrisisHUDComponent>>().WithEntityAccess())
            {
                ref var hud = ref hudRw.ValueRW;
                hud = CreateDefaultHud(faction.ValueRO.FactionId);

                if (!entityManager.HasComponent<SuccessionCrisisComponent>(entity))
                {
                    continue;
                }

                var crisis = entityManager.GetComponentData<SuccessionCrisisComponent>(entity);
                var severity = (SuccessionCrisisSeverity)crisis.CrisisSeverity;
                hud.CrisisActive = severity != SuccessionCrisisSeverity.None;
                hud.CrisisSeverity = severity;
                hud.SeverityLabel = ResolveSeverityLabel(severity);
                hud.SeverityColor = ResolveSeverityColor(severity);
                hud.RecoveryProgressPct = math.saturate(crisis.RecoveryProgressPct);
                hud.ResourceTrickleFactor = crisis.ResourceTrickleFactor > 0f
                    ? crisis.ResourceTrickleFactor
                    : 1f;
                hud.LegitimacyDrainRatePerDay = crisis.LegitimacyDrainRatePerDay;
            }
        }

        private static SuccessionCrisisHUDComponent CreateDefaultHud(FixedString32Bytes factionId)
        {
            return new SuccessionCrisisHUDComponent
            {
                FactionId = factionId,
                CrisisActive = false,
                CrisisSeverity = SuccessionCrisisSeverity.None,
                SeverityLabel = new FixedString32Bytes("None"),
                SeverityColor = new FixedString32Bytes("green"),
                RecoveryProgressPct = 0f,
                ResourceTrickleFactor = 1f,
                LegitimacyDrainRatePerDay = 0f,
            };
        }

        private static FixedString32Bytes ResolveSeverityLabel(SuccessionCrisisSeverity severity)
        {
            switch (severity)
            {
                case SuccessionCrisisSeverity.Minor:
                    return new FixedString32Bytes("Minor");
                case SuccessionCrisisSeverity.Moderate:
                    return new FixedString32Bytes("Moderate");
                case SuccessionCrisisSeverity.Major:
                    return new FixedString32Bytes("Major");
                case SuccessionCrisisSeverity.Catastrophic:
                    return new FixedString32Bytes("Catastrophic");
                default:
                    return new FixedString32Bytes("None");
            }
        }

        private static FixedString32Bytes ResolveSeverityColor(SuccessionCrisisSeverity severity)
        {
            switch (severity)
            {
                case SuccessionCrisisSeverity.Minor:
                case SuccessionCrisisSeverity.Moderate:
                    return new FixedString32Bytes("yellow");
                case SuccessionCrisisSeverity.Major:
                case SuccessionCrisisSeverity.Catastrophic:
                    return new FixedString32Bytes("red");
                default:
                    return new FixedString32Bytes("green");
            }
        }
    }
}
