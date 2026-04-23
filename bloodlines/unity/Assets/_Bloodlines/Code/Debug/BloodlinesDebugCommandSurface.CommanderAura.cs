using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetCommanderAura(
            string unitId,
            out CommanderAuraComponent aura)
        {
            aura = default;
            if (string.IsNullOrWhiteSpace(unitId))
            {
                return false;
            }

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return false;
            }

            var entityManager = world.EntityManager;
            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<CommanderComponent>(),
                ComponentType.ReadOnly<CommanderAuraComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var commanders = query.ToComponentDataArray<CommanderComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                bool memberMatch = commanders[i].MemberId.ToString() == unitId;
                bool typeMatch = entityManager.HasComponent<UnitTypeComponent>(entities[i]) &&
                    entityManager.GetComponentData<UnitTypeComponent>(entities[i]).TypeId.ToString() == unitId;
                if (!memberMatch && !typeMatch)
                {
                    continue;
                }

                aura = entityManager.GetComponentData<CommanderAuraComponent>(entities[i]);
                return true;
            }

            return false;
        }
    }
}
