using Bloodlines.Components;
using Bloodlines.Rendering;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Attaches a FactionTintComponent to any entity that has a FactionComponent
    /// and a UnitTypeComponent or BuildingTypeComponent but no FactionTint yet.
    /// Resolved from FactionTintPalette so player/enemy/tribes/neutral all get
    /// a canonical starting color without per-entity manual seeding.
    ///
    /// Runs in InitializationSystemGroup so the tint is set before the visual
    /// presentation bridge reads it. Uses EndSimulationEntityCommandBufferSystem
    /// for the structural adds so it can run against many entities per frame
    /// without interleaving writes.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct FactionTintAttachSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (faction, unit, entity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<UnitTypeComponent>>()
                    .WithNone<FactionTintComponent>()
                    .WithEntityAccess())
            {
                var tint = FactionTintPalette.ResolveTint(faction.ValueRO.FactionId.ToString());
                ecb.AddComponent(entity, new FactionTintComponent { Tint = tint });
            }

            foreach (var (faction, building, entity) in
                SystemAPI.Query<RefRO<FactionComponent>, RefRO<BuildingTypeComponent>>()
                    .WithNone<FactionTintComponent>()
                    .WithEntityAccess())
            {
                var tint = FactionTintPalette.ResolveTint(faction.ValueRO.FactionId.ToString());
                ecb.AddComponent(entity, new FactionTintComponent { Tint = tint });
            }
        }
    }
}
