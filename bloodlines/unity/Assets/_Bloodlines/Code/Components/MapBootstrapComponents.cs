using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Components
{
    public struct MapBootstrapPendingTag : IComponentData
    {
    }

    public struct MapBootstrapConfigComponent : IComponentData
    {
        public FixedString64Bytes MapId;
        public FixedString128Bytes MapDisplayName;
        public int TileSize;
        public int Width;
        public int Height;
        public float2 CameraStart;
        public bool SpawnFactions;
        public bool SpawnBuildings;
        public bool SpawnUnits;
        public bool SpawnResourceNodes;
        public bool SpawnControlPoints;
        public bool SpawnSettlements;
    }

    public struct MapFactionSeedElement : IBufferElementData
    {
        public FixedString32Bytes FactionId;
        public FixedString32Bytes HouseId;
        public FactionKind Kind;
        public float Gold;
        public float Food;
        public float Water;
        public float Wood;
        public float Stone;
        public float Iron;
        public float Influence;
        public int PopulationTotal;
        public int PopulationCap;
        public int PopulationReserved;
    }

    public struct MapBuildingSeedElement : IBufferElementData
    {
        public FixedString64Bytes RuntimeId;
        public FixedString32Bytes FactionId;
        public FixedString64Bytes TypeId;
        public float3 Position;
        public bool Completed;
        public float MaxHealth;
        public FortificationRole FortificationRole;
        public float StructuralDamageMultiplier;
        public int PopulationCapBonus;
        public bool BlocksPassage;
        public bool SupportsSiegePreparation;
        public bool SupportsSiegeLogistics;
        public float GoldTrickle;
        public float FoodTrickle;
        public float WaterTrickle;
        public float WoodTrickle;
        public float StoneTrickle;
        public float IronTrickle;
        public float InfluenceTrickle;
    }

    public struct MapUnitSeedElement : IBufferElementData
    {
        public FixedString64Bytes RuntimeId;
        public FixedString32Bytes FactionId;
        public FixedString64Bytes TypeId;
        public float3 Position;
        public float MaxHealth;
        public float MaxSpeed;
        public float AttackDamage;
        public float AttackRange;
        public float AttackCooldown;
        public float Sight;
        public float ProjectileSpeed;
        public float ProjectileMaxLifetimeSeconds;
        public float ProjectileArrivalRadius;
        public UnitRole Role;
        public SiegeClass SiegeClass;
        public int PopulationCost;
        public int Stage;
    }

    public struct MapResourceNodeSeedElement : IBufferElementData
    {
        public FixedString64Bytes RuntimeId;
        public FixedString32Bytes ResourceId;
        public float3 Position;
        public float Amount;
    }

    public struct MapControlPointSeedElement : IBufferElementData
    {
        public FixedString32Bytes RuntimeId;
        public FixedString32Bytes SettlementClassId;
        public FixedString32Bytes ContinentId;
        public float3 Position;
        public float RadiusTiles;
        public float CaptureTimeSeconds;
        public float GoldTrickle;
        public float FoodTrickle;
        public float WaterTrickle;
        public float WoodTrickle;
        public float StoneTrickle;
        public float IronTrickle;
        public float InfluenceTrickle;
    }

    public struct MapSettlementSeedElement : IBufferElementData
    {
        public FixedString64Bytes RuntimeId;
        public FixedString32Bytes FactionId;
        public FixedString32Bytes SettlementClassId;
        public FixedString64Bytes AnchorBuildingId;
        public float3 Position;
        public int FortificationTier;
        public int FortificationCeiling;
        public bool IsPrimaryKeep;
    }
}
