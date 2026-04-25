using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Naval
{
    /// <summary>
    /// Resolves pending fire-ship detonations. For each vessel carrying
    /// <see cref="NavalVesselComponent"/> with OneUseSacrifice=true and a
    /// <see cref="FireShipDetonationPendingTag"/>, sets HealthComponent.Current
    /// to 0 and adds <see cref="DeadTag"/>. Removes the pending tag.
    ///
    /// Browser parity: simulation.js updateVessel (~8836-8840). After the strike
    /// damage is applied, oneUseSacrifice vessels self-destroy on the same tick.
    /// In Unity, AttackResolutionSystem queues the pending tag on the vessel
    /// after damage; this system processes the tag one tick later (or same tick
    /// if SystemAPI ordering allows). Either timing matches the browser
    /// behavior because the vessel is dead by the next decision point.
    ///
    /// Conviction events on detonation deferred to a follow-up slice (the
    /// browser pushes a narrative message; that maps to a future Unity
    /// narrative system).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(Bloodlines.Systems.AttackResolutionSystem))]
    public partial struct FireShipDetonationSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FireShipDetonationPendingTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FireShipDetonationPendingTag>(),
                ComponentType.ReadOnly<NavalVesselComponent>());

            using var entities = query.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                ecb.RemoveComponent<FireShipDetonationPendingTag>(entity);

                var vessel = em.GetComponentData<NavalVesselComponent>(entity);
                if (!vessel.OneUseSacrifice) continue;

                if (em.HasComponent<HealthComponent>(entity))
                {
                    var health = em.GetComponentData<HealthComponent>(entity);
                    health.Current = 0f;
                    ecb.SetComponent(entity, health);
                }
                if (!em.HasComponent<DeadTag>(entity))
                {
                    ecb.AddComponent<DeadTag>(entity);
                }
            }
        }
    }
}
