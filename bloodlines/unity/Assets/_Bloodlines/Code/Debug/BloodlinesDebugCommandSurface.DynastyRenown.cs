using System;
using System.Globalization;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetDynastyRenown(string factionId, out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(factionId))
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
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyRenownComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            query.Dispose();

            var factionKey = new FixedString32Bytes(factionId);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionKey))
                {
                    continue;
                }

                var renown = entityManager.GetComponentData<DynastyRenownComponent>(entities[i]);
                readout =
                    "DynastyRenown|FactionId=" + factions[i].FactionId +
                    "|Score=" + renown.RenownScore.ToString("0.000", CultureInfo.InvariantCulture) +
                    "|PeakRenown=" + renown.PeakRenown.ToString("0.000", CultureInfo.InvariantCulture) +
                    "|DecayRate=" + renown.RenownDecayRate.ToString("0.000", CultureInfo.InvariantCulture) +
                    "|LastUpdatedInWorldDays=" + renown.LastRenownUpdateInWorldDays.ToString("0.000", CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }
    }
}
