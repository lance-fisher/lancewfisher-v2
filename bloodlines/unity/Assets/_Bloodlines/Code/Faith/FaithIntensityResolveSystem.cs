using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Keeps FaithStateComponent.Level in lockstep with its Intensity field on
    /// every faction entity. Downstream readers (ward profile resolution,
    /// future AI faith decisions, UI) can always trust Level matches the
    /// canonical tier table without caring how Intensity got mutated.
    ///
    /// Browser reference: syncFaithIntensityState (simulation.js:1907), which
    /// the browser calls after every update that touches intensity.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct FaithIntensityResolveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FaithStateComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var faith in SystemAPI.Query<RefRW<FaithStateComponent>>())
            {
                FaithScoring.SyncLevel(ref faith.ValueRW);
            }
        }
    }
}
