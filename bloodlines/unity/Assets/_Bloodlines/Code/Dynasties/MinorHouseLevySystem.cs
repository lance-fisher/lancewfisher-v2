using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Advances the levy accumulator on each minor-house faction and spawns a
    /// combat (militia) unit when the interval fires. Browser reference:
    /// simulation.js tickMinorHouseTerritorialLevies (~7060).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct MinorHouseLevySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MinorHouseLevyComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float dt = SystemAPI.Time.DeltaTime;
            if (dt <= 0f) dt = 0.016f;

            var query    = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>(),
                typeof(MinorHouseLevyComponent));

            var entities  = query.ToEntityArray(Allocator.Temp);
            var factions  = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            var kinds     = query.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            var levyComps = query.ToComponentDataArray<MinorHouseLevyComponent>(Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < entities.Length; i++)
            {
                if (kinds[i].Kind != FactionKind.MinorHouse)
                    continue;

                var levy = levyComps[i];
                levy.LevyAccumulator += dt;

                float interval = levy.LevyIntervalSeconds < 0.001f ? 0.001f : levy.LevyIntervalSeconds;
                if (levy.LevyAccumulator >= interval)
                {
                    levy.LevyAccumulator -= interval;
                    SpawnLevyUnit(em, factions[i].FactionId);
                    levy.LeviesIssued++;
                }

                em.SetComponentData(entities[i], levy);
            }

            entities.Dispose();
            factions.Dispose();
            kinds.Dispose();
            levyComps.Dispose();
        }

        private static void SpawnLevyUnit(EntityManager em, FixedString32Bytes factionId)
        {
            var unitEntity = em.CreateEntity();
            em.AddComponentData(unitEntity, new FactionComponent { FactionId = factionId });
            em.AddComponentData(unitEntity, new UnitTypeComponent
            {
                Role   = UnitRole.Melee,
                TypeId = new FixedString64Bytes("levy_militia"),
            });
            em.AddComponentData(unitEntity, new PositionComponent { Value = default });
            em.AddComponentData(unitEntity, new HealthComponent { Current = 50f, Max = 50f });
            em.AddComponentData(unitEntity, new MoveCommandComponent { IsActive = false });
        }
    }
}
