using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Combat
{
    /// <summary>
    /// Consumes PlayerRallyPointSetRequestComponent on production building entities and
    /// writes the requested state into RallyPointComponent (lazy-added if absent).
    /// The request component is removed after processing so the next frame is clean.
    ///
    /// Browser equivalent: absent -- implemented from canonical production design.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct RallyPointSetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerRallyPointSetRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            var requestQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerRallyPointSetRequestComponent>());
            using var requestEntities = requestQuery.ToEntityArray(Allocator.Temp);
            using var requests = requestQuery.ToComponentDataArray<PlayerRallyPointSetRequestComponent>(Allocator.Temp);
            requestQuery.Dispose();

            for (int i = 0; i < requestEntities.Length; i++)
            {
                Entity buildingEntity = requestEntities[i];
                var request = requests[i];

                var rallyPoint = new RallyPointComponent
                {
                    TargetPosition = request.TargetPosition,
                    IsActive = request.IsActive,
                };

                if (em.HasComponent<RallyPointComponent>(buildingEntity))
                {
                    em.SetComponentData(buildingEntity, rallyPoint);
                }
                else
                {
                    em.AddComponentData(buildingEntity, rallyPoint);
                }

                em.RemoveComponent<PlayerRallyPointSetRequestComponent>(buildingEntity);
            }
        }
    }
}
