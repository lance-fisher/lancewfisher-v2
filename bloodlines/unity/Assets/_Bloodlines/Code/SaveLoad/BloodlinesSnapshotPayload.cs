using System;
using System.Collections.Generic;
using Bloodlines.Components;

namespace Bloodlines.SaveLoad
{
    /// <summary>
    /// Serializable snapshot of all live ECS faction-scoped simulation state.
    /// Schema version 1: faction ledgers, loyalty, resources, realm conditions,
    /// conviction, dynasty (state + members + fallen ledger), faith (state + exposure),
    /// population. Not included in v1: units, buildings, control points, marriages,
    /// holy wars, captives, lesser houses, intelligence dossiers (documented as gaps).
    ///
    /// Browser reference: simulation.js:13822 exportStateSnapshot
    /// (faction, conviction, dynasty, faith, population sub-objects)
    /// </summary>
    [Serializable]
    public class BloodlinesSnapshotPayload
    {
        public int SchemaVersion = 1;
        public float CapturedAtElapsedSeconds;

        public List<FactionSnapshot> FactionSnapshots = new();
        public List<LoyaltySnapshot> LoyaltySnapshots = new();
        public List<ResourceSnapshot> ResourceSnapshots = new();
        public List<RealmConditionSnapshot> RealmConditionSnapshots = new();
        public List<ConvictionSnapshot> ConvictionSnapshots = new();
        public List<DynastyStateSnapshot> DynastyStateSnapshots = new();
        public List<DynastyMemberSnapshot> DynastyMemberSnapshots = new();
        public List<DynastyFallenLedgerEntrySnapshot> FallenLedgerSnapshots = new();
        public List<FaithStateSnapshot> FaithStateSnapshots = new();
        public List<FaithExposureSnapshot> FaithExposureSnapshots = new();
        public List<PopulationSnapshot> PopulationSnapshots = new();

        // V1 snapshot gaps (no ECS source component yet):
        // - Units (position, health, command, field-water state, siege logistics)
        // - Buildings (progress, production queue, burn/raid/sabotage timers)
        // - Control points (capture state, fortification tier)
        // - Marriages, holy wars, captives, lesser houses, intelligence dossiers
        // - Political events, coalition pressure, diplomacy pacts
        // These fields will land in snapshot-extension slices when their components land.
    }

    [Serializable]
    public class FactionSnapshot
    {
        public string FactionId;
        public string HouseId;
        public string Kind;
    }

    [Serializable]
    public class LoyaltySnapshot
    {
        public string FactionId;
        public float Current;
        public float Max;
        public float Floor;
    }

    [Serializable]
    public class ResourceSnapshot
    {
        public string FactionId;
        public float Gold;
        public float Food;
        public float Water;
        public float Wood;
        public float Stone;
        public float Iron;
        public float Influence;
    }

    [Serializable]
    public class RealmConditionSnapshot
    {
        public string FactionId;
        public float CycleAccumulator;
        public int CycleCount;
        public int FoodStrainStreak;
        public int WaterStrainStreak;
        public float AssaultFailureStrain;
        public double CohesionPenaltyUntil;
        public int LastStarvationResponseCycle;
        public int LastCapPressureResponseCycle;
        public int LastStabilitySurplusResponseCycle;
    }

    [Serializable]
    public class ConvictionSnapshot
    {
        public string FactionId;
        public float Ruthlessness;
        public float Stewardship;
        public float Oathkeeping;
        public float Desecration;
        public float Score;
        public ConvictionBand Band;
    }

    [Serializable]
    public class DynastyStateSnapshot
    {
        public string FactionId;
        public int ActiveMemberCap;
        public int DormantReserve;
        public float Legitimacy;
        public float LoyaltyPressure;
        public bool Interregnum;
    }

    [Serializable]
    public class DynastyMemberSnapshot
    {
        public string FactionId;
        public string MemberId;
        public string Title;
        public DynastyRole Role;
        public DynastyPath Path;
        public float AgeYears;
        public DynastyMemberStatus Status;
        public float Renown;
        public int Order;
        public float FallenAtWorldSeconds;
    }

    [Serializable]
    public class DynastyFallenLedgerEntrySnapshot
    {
        public string FactionId;
        public string MemberId;
        public string Title;
        public DynastyRole Role;
        public float FallenAtWorldSeconds;
    }

    [Serializable]
    public class FaithStateSnapshot
    {
        public string FactionId;
        public CovenantId SelectedFaith;
        public DoctrinePath DoctrinePath;
        public float Intensity;
        public int Level;
    }

    [Serializable]
    public class FaithExposureSnapshot
    {
        public string FactionId;
        public CovenantId Faith;
        public float Exposure;
        public bool Discovered;
    }

    [Serializable]
    public class PopulationSnapshot
    {
        public string FactionId;
        public int Total;
        public int Available;
        public int Cap;
        public int BaseCap;
        public int CapBonus;
        public float GrowthAccumulator;
    }
}
