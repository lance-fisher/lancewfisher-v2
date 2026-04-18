using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Advances the DualClockComponent singleton each simulation frame.
    /// Browser equivalent: tickDualClock (simulation.js:13795).
    /// One real second = DaysPerRealSecond in-world days (canonical default: 2).
    /// Runs in SimulationSystemGroup before MatchProgressionEvaluationSystem.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(MatchProgressionEvaluationSystem))]
    [BurstCompile]
    public partial struct DualClockTickSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<DualClockComponent>())
            {
                var entity = state.EntityManager.CreateEntity(typeof(DualClockComponent));
                state.EntityManager.SetName(entity, "DualClock");
                state.EntityManager.SetComponentData(entity, new DualClockComponent
                {
                    InWorldDays = 0f,
                    DaysPerRealSecond = 2f,
                    DeclarationCount = 0,
                });
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            var clock = SystemAPI.GetSingletonRW<DualClockComponent>();
            clock.ValueRW.InWorldDays += dt * clock.ValueRO.DaysPerRealSecond;
        }
    }
}
