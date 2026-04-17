using Bloodlines.Components;
using Unity.Burst;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Ages every active, ruling, or captured dynasty member by a configurable
    /// in-world year rate. Fallen and dormant members do not age. The rate is a
    /// seam placeholder until the dual-clock + five-stage match progression
    /// slice lands; at that point the authoritative in-world clock replaces
    /// this per-tick scaling.
    ///
    /// Canonical browser reference: dynasty members carry an `age` field that is
    /// advanced as the dual clock ticks (simulation.js tickDualClock). The
    /// browser uses in-world days; Unity uses seconds here until the clock
    /// slice ports the conversion.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct DynastyAgingSystem : ISystem
    {
        private const float DefaultInWorldYearsPerRealSecond = 1f / 60f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DynastyMemberComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f)
            {
                return;
            }

            float yearsThisTick = dt * DefaultInWorldYearsPerRealSecond;
            foreach (var member in SystemAPI.Query<RefRW<DynastyMemberComponent>>())
            {
                var status = member.ValueRO.Status;
                if (status == DynastyMemberStatus.Fallen || status == DynastyMemberStatus.Dormant)
                {
                    continue;
                }

                member.ValueRW.AgeYears += yearsThisTick;
            }
        }
    }
}
