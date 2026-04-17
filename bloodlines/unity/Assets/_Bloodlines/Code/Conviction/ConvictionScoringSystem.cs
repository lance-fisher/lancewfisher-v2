using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Conviction
{
    /// <summary>
    /// Keeps the Score and Band fields on ConvictionComponent synchronized with
    /// the four bucket fields for every faction entity. Runs in the simulation
    /// group so any downstream reader sees a consistent band within the same tick
    /// that buckets were mutated. Idempotent: if buckets haven't changed, the
    /// system still just writes the same Score and Band back, which is cheaper
    /// than tracking dirty state and is safe under Burst.
    ///
    /// Browser reference: simulation.js refreshConvictionBand (4233), called
    /// after every recordConvictionEvent and once per simulation tick for
    /// defensive consistency.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct ConvictionScoringSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConvictionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var conviction in SystemAPI.Query<RefRW<ConvictionComponent>>())
            {
                ConvictionScoring.Refresh(ref conviction.ValueRW);
            }
        }
    }
}
