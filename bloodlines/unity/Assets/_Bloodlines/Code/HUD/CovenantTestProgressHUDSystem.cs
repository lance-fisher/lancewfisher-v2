using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.HUD
{
    /// <summary>
    /// Projects covenant-test readiness, cooldown, and qualification progress into
    /// a stable per-faction HUD bar without reopening the underlying faith lane.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct CovenantTestProgressHUDSystem : ISystem
    {
        private const float RefreshCadenceInWorldDays = 1f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<FactionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (faction, entity) in
                SystemAPI.Query<RefRO<FactionComponent>>().WithEntityAccess())
            {
                if (!entityManager.HasComponent<CovenantTestProgressHUDComponent>(entity))
                {
                    ecb.AddComponent(entity, CreateDefaultHud(faction.ValueRO.FactionId));
                }
            }

            ecb.Playback(entityManager);

            foreach (var (faction, hudRw, entity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRW<CovenantTestProgressHUDComponent>>().WithEntityAccess())
            {
                ref var hud = ref hudRw.ValueRW;
                if (!float.IsNaN(hud.LastRefreshInWorldDays) &&
                    inWorldDays - hud.LastRefreshInWorldDays < RefreshCadenceInWorldDays)
                {
                    continue;
                }

                hud = CreateDefaultHud(faction.ValueRO.FactionId);
                hud.LastRefreshInWorldDays = inWorldDays;

                if (!entityManager.HasComponent<CovenantTestStateComponent>(entity))
                {
                    continue;
                }

                var testState = entityManager.GetComponentData<CovenantTestStateComponent>(entity);
                hud.TestPhase = testState.TestPhase;
                hud.PhaseLabel = ResolvePhaseLabel(testState.TestPhase);
                hud.SuccessCount = testState.SuccessCount;

                if (entityManager.HasComponent<FaithStateComponent>(entity))
                {
                    hud.FaithId = entityManager.GetComponentData<FaithStateComponent>(entity).SelectedFaith;
                }

                float qualifyingDays = ResolveQualifyingDays(testState, inWorldDays);
                hud.QualifyingDays = qualifyingDays;
                hud.CooldownRemainingInWorldDays = ResolveCooldownRemaining(entityManager, entity, inWorldDays);

                if (testState.TestPhase == CovenantTestPhase.ReadyToTrigger ||
                    testState.TestPhase == CovenantTestPhase.InProgress ||
                    testState.TestPhase == CovenantTestPhase.Complete)
                {
                    hud.ProgressPct = 1f;
                    hud.QualifyingDays = CovenantTestRules.DurationInWorldDays;
                }
                else
                {
                    hud.ProgressPct = math.saturate(
                        qualifyingDays / math.max(1f, CovenantTestRules.DurationInWorldDays));
                }
            }
        }

        private static CovenantTestProgressHUDComponent CreateDefaultHud(FixedString32Bytes factionId)
        {
            return new CovenantTestProgressHUDComponent
            {
                FactionId = factionId,
                LastRefreshInWorldDays = float.NaN,
                FaithId = CovenantId.None,
                TestPhase = CovenantTestPhase.Inactive,
                PhaseLabel = new FixedString32Bytes("Inactive"),
                QualifyingDays = 0f,
                RequiredDays = CovenantTestRules.DurationInWorldDays,
                ProgressPct = 0f,
                CooldownRemainingInWorldDays = 0f,
                SuccessCount = 0,
            };
        }

        private static float ResolveQualifyingDays(
            CovenantTestStateComponent testState,
            float inWorldDays)
        {
            if (float.IsNaN(testState.IntensityThresholdMetAtInWorldDays))
            {
                return 0f;
            }

            return math.max(0f, inWorldDays - testState.IntensityThresholdMetAtInWorldDays);
        }

        private static float ResolveCooldownRemaining(
            EntityManager entityManager,
            Entity factionEntity,
            float inWorldDays)
        {
            if (!entityManager.HasBuffer<DynastyPoliticalEventComponent>(factionEntity))
            {
                return 0f;
            }

            var buffer = entityManager.GetBuffer<DynastyPoliticalEventComponent>(factionEntity);
            float remaining = 0f;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].EventType.Equals(DynastyPoliticalEventTypes.CovenantTestCooldown))
                {
                    continue;
                }

                remaining = math.max(remaining, buffer[i].ExpiresAtInWorldDays - inWorldDays);
            }

            return math.max(0f, remaining);
        }

        private static FixedString32Bytes ResolvePhaseLabel(CovenantTestPhase phase)
        {
            switch (phase)
            {
                case CovenantTestPhase.Qualifying:
                    return new FixedString32Bytes("Qualifying");
                case CovenantTestPhase.ReadyToTrigger:
                    return new FixedString32Bytes("Ready");
                case CovenantTestPhase.InProgress:
                    return new FixedString32Bytes("In Progress");
                case CovenantTestPhase.Complete:
                    return new FixedString32Bytes("Complete");
                case CovenantTestPhase.Failed:
                    return new FixedString32Bytes("Failed");
                default:
                    return new FixedString32Bytes("Inactive");
            }
        }
    }
}
