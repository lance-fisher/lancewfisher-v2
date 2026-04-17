using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileImpactSystem))]
    [UpdateBefore(typeof(DeathResolutionSystem))]
    public partial struct RecentImpactRecoverySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RecentImpactComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            foreach (var impactRw in SystemAPI.Query<RefRW<RecentImpactComponent>>())
            {
                var impact = impactRw.ValueRO;
                impact.SecondsRemaining = math.max(0f, impact.SecondsRemaining - dt);
                impactRw.ValueRW = impact;
            }
        }
    }
}
