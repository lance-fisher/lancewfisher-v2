using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Processes pending DeclareInWorldTimeRequest buffer elements on the DualClock
    /// singleton entity each frame. Accumulates DaysDelta into InWorldDays and
    /// increments DeclarationCount. Clears the buffer after processing.
    /// Browser equivalent: declareInWorldTime (simulation.js:13800).
    ///
    /// Runs after DualClockTickSystem (baseline tick already applied) and before
    /// MatchProgressionEvaluationSystem (so InWorldDays includes declarations this frame).
    /// Only runs when the DualClock singleton and its request buffer are present.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DualClockTickSystem))]
    [UpdateBefore(typeof(MatchProgressionEvaluationSystem))]
    [BurstCompile]
    public partial struct DualClockDeclarationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // Add the request buffer to the DualClock singleton when it exists.
            // If the singleton doesn't exist yet (DualClockTickSystem hasn't run OnCreate),
            // we simply do nothing -- the buffer is added lazily on first update.
            state.RequireForUpdate<DualClockComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var clockEntity = SystemAPI.GetSingletonEntity<DualClockComponent>();

            if (!em.HasBuffer<DeclareInWorldTimeRequest>(clockEntity))
            {
                em.AddBuffer<DeclareInWorldTimeRequest>(clockEntity);
                return;
            }

            var requests = em.GetBuffer<DeclareInWorldTimeRequest>(clockEntity);
            if (requests.Length == 0) return;

            var clock = em.GetComponentData<DualClockComponent>(clockEntity);
            for (int i = 0; i < requests.Length; i++)
            {
                if (requests[i].DaysDelta > 0f)
                {
                    clock.InWorldDays += requests[i].DaysDelta;
                    clock.DeclarationCount++;
                }
            }
            em.SetComponentData(clockEntity, clock);
            requests.Clear();
        }
    }
}
