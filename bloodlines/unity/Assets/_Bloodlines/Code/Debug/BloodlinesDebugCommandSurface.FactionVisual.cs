using Bloodlines.Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetFactionVisual(string factionId, out float4 tint, out string emblemId)
        {
            tint = float4.zero;
            emblemId = null;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            using var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionVisualComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Unity.Collections.Allocator.Temp);
            using var visuals = query.ToComponentDataArray<FactionVisualComponent>(Unity.Collections.Allocator.Temp);
            query.Dispose();

            for (int i = 0; i < factions.Length; i++)
            {
                if (factions[i].FactionId.ToString() == factionId)
                {
                    tint = visuals[i].PrimaryTint;
                    emblemId = visuals[i].EmblemId.ToString();
                    return true;
                }
            }

            return false;
        }

        public bool TryDebugGetUnitFactionColor(Entity unit, out string factionId, out float4 tint)
        {
            factionId = null;
            tint = float4.zero;

            var em = World.DefaultGameObjectInjectionWorld?.EntityManager ?? default;
            if (!em.HasComponent<UnitFactionColorComponent>(unit))
            {
                return false;
            }

            var color = em.GetComponentData<UnitFactionColorComponent>(unit);
            factionId = color.FactionId.ToString();
            tint = color.Tint;
            return true;
        }
    }
}
