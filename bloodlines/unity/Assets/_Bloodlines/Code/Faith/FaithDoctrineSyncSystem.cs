using Bloodlines.Components;
using Unity.Entities;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Reads FaithStateComponent per faction and writes FaithDoctrineEffectsComponent with
    /// the canonical doctrine effects for the faction's committed covenant and path.
    /// Runs before ControlPointCaptureSystem so downstream stabilization/capture reads see
    /// the current frame's doctrine.
    ///
    /// Browser reference: getFaithDoctrineEffects (simulation.js ~581)
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(Bloodlines.Systems.ControlPointCaptureSystem))]
    public partial struct FaithDoctrineSyncSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity e = entities[i];
                FaithDoctrineEffectsComponent effects;

                if (em.HasComponent<FaithStateComponent>(e))
                {
                    var faith = em.GetComponentData<FaithStateComponent>(e);
                    effects = FaithDoctrineCanon.Resolve(faith.SelectedFaith, faith.DoctrinePath);
                }
                else
                {
                    effects = FaithDoctrineEffectsComponent.Defaults();
                }

                if (em.HasComponent<FaithDoctrineEffectsComponent>(e))
                {
                    em.SetComponentData(e, effects);
                }
                else
                {
                    em.AddComponentData(e, effects);
                }
            }
        }
    }
}
