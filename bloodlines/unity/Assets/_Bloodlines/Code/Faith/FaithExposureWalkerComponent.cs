using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Static sacred-site authoring data baked into ECS for the faith exposure pass.
    /// Browser reference: state.world.sacredSites entries consumed by
    /// updateFaithExposure (simulation.js:8174).
    /// </summary>
    public struct SacredSiteExposureSourceComponent : IComponentData
    {
        public FixedString64Bytes SiteId;
        public CovenantId Faith;
        public float RadiusWorldUnits;
        public float ExposureRate;
    }

    /// <summary>
    /// Marker and authored profile for completed faith structures that amplify
    /// sacred-site exposure. The component is attached dynamically from
    /// BuildingTypeComponent so future construction completions inherit the same rule.
    /// </summary>
    public struct FaithExposureStructureComponent : IComponentData
    {
        public FaithExposureStructureRole Role;
        public float Amplification;
        public float RadiusWorldUnits;
    }

    public enum FaithExposureStructureRole : byte
    {
        None = 0,
        Shrine = 1,
        Hall = 2,
        Sanctuary = 3,
    }

    public static class FaithExposureWalkerRules
    {
        public const float MaxMultiplier = 4f;
        public const int MaxContributors = 4;

        private static readonly FixedString64Bytes WayshrineTypeId = new("wayshrine");
        private static readonly FixedString64Bytes CovenantHallTypeId = new("covenant_hall");
        private static readonly FixedString64Bytes GrandSanctuaryTypeId = new("grand_sanctuary");

        public static bool TryResolveStructure(
            in BuildingTypeComponent buildingType,
            out FaithExposureStructureComponent structure)
        {
            if (buildingType.TypeId.Equals(WayshrineTypeId))
            {
                structure = new FaithExposureStructureComponent
                {
                    Role = FaithExposureStructureRole.Shrine,
                    Amplification = 1.8f,
                    RadiusWorldUnits = 180f,
                };
                return true;
            }

            if (buildingType.TypeId.Equals(CovenantHallTypeId))
            {
                structure = new FaithExposureStructureComponent
                {
                    Role = FaithExposureStructureRole.Hall,
                    Amplification = 2.4f,
                    RadiusWorldUnits = 240f,
                };
                return true;
            }

            if (buildingType.TypeId.Equals(GrandSanctuaryTypeId))
            {
                structure = new FaithExposureStructureComponent
                {
                    Role = FaithExposureStructureRole.Sanctuary,
                    Amplification = 3f,
                    RadiusWorldUnits = 320f,
                };
                return true;
            }

            structure = default;
            return false;
        }

        public static CovenantId ResolveCovenantId(FixedString32Bytes faithId)
        {
            if (faithId.Equals(new FixedString32Bytes("old_light")))
            {
                return CovenantId.OldLight;
            }

            if (faithId.Equals(new FixedString32Bytes("blood_dominion")))
            {
                return CovenantId.BloodDominion;
            }

            if (faithId.Equals(new FixedString32Bytes("the_order")))
            {
                return CovenantId.TheOrder;
            }

            if (faithId.Equals(new FixedString32Bytes("the_wild")))
            {
                return CovenantId.TheWild;
            }

            return CovenantId.None;
        }

        public static float ResolveStructureMultiplier(
            FixedString32Bytes factionId,
            float3 sacredSitePosition,
            NativeArray<FactionComponent> structureFactions,
            NativeArray<PositionComponent> structurePositions,
            NativeArray<HealthComponent> structureHealths,
            NativeArray<FaithExposureStructureComponent> structures,
            out int contributorCount)
        {
            contributorCount = 0;
            float cumulative = 1f;

            for (int i = 0; i < structures.Length; i++)
            {
                if (!structureFactions[i].FactionId.Equals(factionId) ||
                    structureHealths[i].Current <= 0f)
                {
                    continue;
                }

                FaithExposureStructureComponent structure = structures[i];
                if (structure.Amplification <= 1f || structure.RadiusWorldUnits <= 0f)
                {
                    continue;
                }

                float distanceSq = math.distancesq(
                    structurePositions[i].Value.xz,
                    sacredSitePosition.xz);
                if (distanceSq > structure.RadiusWorldUnits * structure.RadiusWorldUnits)
                {
                    continue;
                }

                cumulative *= structure.Amplification;
                contributorCount++;
                if (contributorCount >= MaxContributors)
                {
                    break;
                }
            }

            return math.min(MaxMultiplier, cumulative);
        }
    }
}
