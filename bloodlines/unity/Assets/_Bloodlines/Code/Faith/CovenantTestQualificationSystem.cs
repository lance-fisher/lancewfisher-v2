using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Faith
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FaithIntensityResolveSystem))]
    public partial struct CovenantTestQualificationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<FaithStateComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FaithStateComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var faithStates = query.ToComponentDataArray<FaithStateComponent>(Allocator.Temp);
            using var ecb = new EntityCommandBuffer(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                var faithState = faithStates[i];
                CovenantTestStateComponent testState = entityManager.HasComponent<CovenantTestStateComponent>(entity)
                    ? entityManager.GetComponentData<CovenantTestStateComponent>(entity)
                    : CovenantTestRules.CreateDefaultState();

                if (faithState.SelectedFaith == CovenantId.None)
                {
                    if (testState.TestPhase != CovenantTestPhase.Complete)
                    {
                        testState = CovenantTestRules.CreateDefaultState();
                    }

                    EnsureState(entityManager, ecb, entity, testState);
                    continue;
                }

                if (testState.TestPhase == CovenantTestPhase.Complete)
                {
                    EnsureState(entityManager, ecb, entity, testState);
                    continue;
                }

                bool cooldownActive = DynastyPoliticalEventUtility.HasActiveEvent(
                    entityManager,
                    entity,
                    DynastyPoliticalEventTypes.CovenantTestCooldown,
                    inWorldDays);

                if (faithState.Intensity >= CovenantTestRules.IntensityThreshold)
                {
                    if (float.IsNaN(testState.IntensityThresholdMetAtInWorldDays))
                    {
                        testState.IntensityThresholdMetAtInWorldDays = inWorldDays;
                    }

                    float qualifyingDays = inWorldDays - testState.IntensityThresholdMetAtInWorldDays;
                    if (!cooldownActive && qualifyingDays >= CovenantTestRules.DurationInWorldDays)
                    {
                        testState.TestPhase = CovenantTestPhase.ReadyToTrigger;
                    }
                    else if (testState.TestPhase != CovenantTestPhase.ReadyToTrigger &&
                             testState.TestPhase != CovenantTestPhase.InProgress)
                    {
                        testState.TestPhase = cooldownActive
                            ? CovenantTestPhase.Failed
                            : CovenantTestPhase.Qualifying;
                    }
                }
                else if (testState.TestPhase != CovenantTestPhase.InProgress)
                {
                    testState.IntensityThresholdMetAtInWorldDays = float.NaN;
                    testState.TestPhase = cooldownActive
                        ? CovenantTestPhase.Failed
                        : CovenantTestPhase.Inactive;
                }

                EnsureState(entityManager, ecb, entity, testState);
            }

            ecb.Playback(entityManager);
        }

        static void EnsureState(
            EntityManager entityManager,
            EntityCommandBuffer ecb,
            Entity entity,
            in CovenantTestStateComponent state)
        {
            if (entityManager.HasComponent<CovenantTestStateComponent>(entity))
            {
                entityManager.SetComponentData(entity, state);
            }
            else
            {
                ecb.AddComponent(entity, state);
            }
        }
    }
}
