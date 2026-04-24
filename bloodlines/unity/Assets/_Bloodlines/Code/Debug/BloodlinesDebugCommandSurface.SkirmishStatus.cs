using Bloodlines.HUD;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetSkirmishStatusHeader(out SkirmishStatusHUDComponent header)
        {
            header = default;
            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(ComponentType.ReadOnly<SkirmishStatusHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            header = query.GetSingleton<SkirmishStatusHUDComponent>();
            return true;
        }

        public bool TryDebugGetSkirmishStatusRows(
            out SkirmishStatusFactionRowHUDComponent[] rows)
        {
            rows = null;
            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<SkirmishStatusHUDComponent>(),
                ComponentType.ReadOnly<SkirmishStatusFactionRowHUDComponent>());
            if (query.IsEmptyIgnoreFilter)
            {
                return false;
            }

            Entity singleton = query.GetSingletonEntity();
            var buffer = em.GetBuffer<SkirmishStatusFactionRowHUDComponent>(singleton, isReadOnly: true);
            rows = new SkirmishStatusFactionRowHUDComponent[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                rows[i] = buffer[i];
            }

            return true;
        }
    }
}
