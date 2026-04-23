using System.Globalization;
using Bloodlines.Components;
using Bloodlines.Faith;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugGetSacredSiteExposureSnapshot(
            string factionId,
            string siteId,
            out string readout)
        {
            readout = string.Empty;
            if (string.IsNullOrWhiteSpace(siteId))
            {
                return false;
            }

            var factionEntity = FindFactionEntityByFaith(factionId, out var entityManager);
            if (factionEntity == Entity.Null ||
                !entityManager.HasBuffer<FaithExposureElement>(factionEntity))
            {
                return false;
            }

            if (!TryFindSacredSite(entityManager, siteId, out var site, out var sitePosition))
            {
                return false;
            }

            int contributorCount = 0;
            float multiplier = 1f;
            var faction = entityManager.GetComponentData<FactionComponent>(factionEntity);

            var structureQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<HealthComponent>(),
                ComponentType.ReadOnly<FaithExposureStructureComponent>(),
                ComponentType.Exclude<ConstructionStateComponent>());
            using var structureFactions = structureQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var structurePositions = structureQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var structureHealths = structureQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);
            using var structures = structureQuery.ToComponentDataArray<FaithExposureStructureComponent>(Allocator.Temp);

            if (structures.Length > 0)
            {
                multiplier = FaithExposureWalkerRules.ResolveStructureMultiplier(
                    faction.FactionId,
                    sitePosition.Value,
                    structureFactions,
                    structurePositions,
                    structureHealths,
                    structures,
                    out contributorCount);
            }

            var exposureBuffer = entityManager.GetBuffer<FaithExposureElement>(factionEntity);
            float exposure = FaithScoring.GetExposure(exposureBuffer, site.Faith);
            bool discovered = false;
            for (int i = 0; i < exposureBuffer.Length; i++)
            {
                if (exposureBuffer[i].Faith == site.Faith)
                {
                    discovered = exposureBuffer[i].Discovered;
                    break;
                }
            }

            readout =
                "SacredSiteExposureSnapshot|FactionId=" + factionId +
                "|SiteId=" + siteId +
                "|Faith=" + site.Faith +
                "|Exposure=" + exposure.ToString("0.###", CultureInfo.InvariantCulture) +
                "|Discovered=" + discovered.ToString().ToLowerInvariant() +
                "|Multiplier=" + multiplier.ToString("0.###", CultureInfo.InvariantCulture) +
                "|ContributorCount=" + contributorCount.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        private static bool TryFindSacredSite(
            EntityManager entityManager,
            string siteId,
            out SacredSiteExposureSourceComponent site,
            out PositionComponent sitePosition)
        {
            site = default;
            sitePosition = default;

            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<SacredSiteExposureSourceComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var sites = query.ToComponentDataArray<SacredSiteExposureSourceComponent>(Allocator.Temp);
            using var positions = query.ToComponentDataArray<PositionComponent>(Allocator.Temp);

            for (int i = 0; i < sites.Length; i++)
            {
                if (sites[i].SiteId.ToString() != siteId)
                {
                    continue;
                }

                site = sites[i];
                sitePosition = positions[i];
                return true;
            }

            return false;
        }
    }
}
