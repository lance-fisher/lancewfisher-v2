using Bloodlines.Components;

namespace Bloodlines.Faith
{
    /// <summary>
    /// Pure static lookup mirroring faiths.json prototypeEffects. Drives FaithDoctrineSyncSystem
    /// and any system that needs doctrine effects without reading FaithDoctrineEffectsComponent.
    ///
    /// Browser reference: faiths.json prototypeEffects / getFaithDoctrineEffects
    /// (simulation.js ~581, data/faiths.json)
    /// </summary>
    public static class FaithDoctrineCanon
    {
        public static FaithDoctrineEffectsComponent Resolve(CovenantId covenant, DoctrinePath path)
        {
            if (covenant == CovenantId.None || path == DoctrinePath.Unassigned)
                return FaithDoctrineEffectsComponent.Defaults();

            return covenant switch
            {
                CovenantId.OldLight => path == DoctrinePath.Light
                    ? new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.05f, AuraRadiusBonus = 18f, AuraSightBonus = 20f,
                        StabilizationMultiplier = 1.34f, CaptureMultiplier = 1f, PopulationGrowthMultiplier = 1.08f,
                    }
                    : new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.14f, AuraRadiusBonus = 10f, AuraSightBonus = 12f,
                        StabilizationMultiplier = 1.04f, CaptureMultiplier = 1.12f, PopulationGrowthMultiplier = 0.97f,
                    },
                CovenantId.BloodDominion => path == DoctrinePath.Light
                    ? new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.08f, AuraRadiusBonus = 14f, AuraSightBonus = 10f,
                        StabilizationMultiplier = 1.16f, CaptureMultiplier = 1.06f, PopulationGrowthMultiplier = 1.02f,
                    }
                    : new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.18f, AuraRadiusBonus = 8f, AuraSightBonus = 8f,
                        StabilizationMultiplier = 0.96f, CaptureMultiplier = 1.24f, PopulationGrowthMultiplier = 0.93f,
                    },
                CovenantId.TheOrder => path == DoctrinePath.Light
                    ? new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.04f, AuraRadiusBonus = 20f, AuraSightBonus = 16f,
                        StabilizationMultiplier = 1.38f, CaptureMultiplier = 1.02f, PopulationGrowthMultiplier = 1.06f,
                    }
                    : new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.11f, AuraRadiusBonus = 12f, AuraSightBonus = 10f,
                        StabilizationMultiplier = 1.08f, CaptureMultiplier = 1.2f, PopulationGrowthMultiplier = 0.98f,
                    },
                CovenantId.TheWild => path == DoctrinePath.Light
                    ? new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.06f, AuraRadiusBonus = 22f, AuraSightBonus = 22f,
                        StabilizationMultiplier = 1.2f, CaptureMultiplier = 1.04f, PopulationGrowthMultiplier = 1.1f,
                    }
                    : new FaithDoctrineEffectsComponent
                    {
                        AuraAttackMultiplier = 1.16f, AuraRadiusBonus = 14f, AuraSightBonus = 14f,
                        StabilizationMultiplier = 0.94f, CaptureMultiplier = 1.22f, PopulationGrowthMultiplier = 0.95f,
                    },
                _ => FaithDoctrineEffectsComponent.Defaults(),
            };
        }
    }
}
