using Bloodlines.Components;
using Bloodlines.Rendering;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Assigns canonical visual identity to faction entities at match startup and
    /// propagates tint to newly spawned unit entities.
    ///
    /// For each faction entity without a FactionVisualComponent: resolves PrimaryTint
    /// from FactionTintPalette and assigns EmblemId as "emblem_" + factionId.
    ///
    /// For each unit entity with FactionComponent but no UnitFactionColorComponent:
    /// resolves the owning faction's FactionVisualComponent and writes a
    /// UnitFactionColorComponent with the matching FactionId and Tint.
    ///
    /// Browser equivalent: absent -- implemented from canonical faction visual design.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct FactionVisualAssignmentSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<FactionComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            // Assign FactionVisualComponent to faction entities that lack one.
            using var factionQuery = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionComponents = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int f = 0; f < factionEntities.Length; f++)
            {
                Entity factionEntity = factionEntities[f];
                if (em.HasComponent<FactionVisualComponent>(factionEntity))
                {
                    continue;
                }

                string factionIdStr = factionComponents[f].FactionId.ToString();
                float4 tint = FactionTintPalette.ResolveTint(factionIdStr);
                var emblemId = new FixedString64Bytes("emblem_" + factionIdStr.ToLowerInvariant());

                em.AddComponentData(factionEntity, new FactionVisualComponent
                {
                    PrimaryTint = tint,
                    EmblemId = emblemId,
                    IsAssigned = true,
                });
            }

            // Propagate tint to unit entities that have FactionComponent but no UnitFactionColorComponent.
            using var unitQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>());
            using var unitEntities = unitQuery.ToEntityArray(Allocator.Temp);
            using var unitFactions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int u = 0; u < unitEntities.Length; u++)
            {
                Entity unitEntity = unitEntities[u];
                if (em.HasComponent<UnitFactionColorComponent>(unitEntity))
                {
                    continue;
                }

                var unitFactionId = unitFactions[u].FactionId;
                float4 tint = new float4(0.72f, 0.72f, 0.72f, 1f);

                for (int f = 0; f < factionComponents.Length; f++)
                {
                    if (factionComponents[f].FactionId.Equals(unitFactionId) &&
                        em.HasComponent<FactionVisualComponent>(factionEntities[f]))
                    {
                        tint = em.GetComponentData<FactionVisualComponent>(factionEntities[f]).PrimaryTint;
                        break;
                    }
                }

                em.AddComponentData(unitEntity, new UnitFactionColorComponent
                {
                    FactionId = unitFactionId,
                    Tint = tint,
                });
            }
        }
    }
}
