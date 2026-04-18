using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;

namespace Bloodlines.SaveLoad
{
    /// <summary>
    /// Restores a BloodlinesSnapshotPayload captured by BloodlinesSnapshotWriter.Capture
    /// into a target EntityManager. Creates one faction entity per FactionSnapshot and
    /// reconstitutes all companion components. Dynasty members are re-created as separate
    /// entities linked via DynastyMemberRef buffer, matching the live construction contract
    /// from DynastyBootstrap.AttachDynasty.
    ///
    /// Restore contract:
    ///   - Does not clear the world before restoring. Caller must ensure target world is empty
    ///     or that faction IDs do not collide with existing entities.
    ///   - Schema version 1 only. Unrecognised versions are rejected.
    ///   - FactionId is the stable identity key across save/restore.
    ///
    /// Browser reference: simulation.js:13989 restoreStateSnapshot
    /// </summary>
    public static class BloodlinesSnapshotRestorer
    {
        public static bool Apply(EntityManager em, BloodlinesSnapshotPayload payload, out string error)
        {
            if (payload == null)
            {
                error = "Restore failed: payload is null.";
                return false;
            }

            if (payload.SchemaVersion != 1)
            {
                error = "Restore failed: unsupported schema version " + payload.SchemaVersion + ".";
                return false;
            }

            // One pass: create faction entities and index by FactionId.
            var factionCount = payload.FactionSnapshots.Count;
            var factionEntities = new NativeHashMap<FixedString32Bytes, Entity>(factionCount, Allocator.Temp);

            try
            {
                foreach (var fs in payload.FactionSnapshots)
                {
                    var e = em.CreateEntity();
                    em.AddComponentData(e, new FactionComponent { FactionId = fs.FactionId });
                    factionEntities.Add(new FixedString32Bytes(fs.FactionId), e);
                }

                RestoreLoyalty(em, payload, factionEntities);
                RestoreResources(em, payload, factionEntities);
                RestoreRealmConditions(em, payload, factionEntities);
                RestoreConviction(em, payload, factionEntities);
                RestoreDynasty(em, payload, factionEntities);
                RestoreFaith(em, payload, factionEntities);
                RestorePopulation(em, payload, factionEntities);
            }
            finally
            {
                factionEntities.Dispose();
            }

            error = null;
            return true;
        }

        private static void RestoreLoyalty(EntityManager em, BloodlinesSnapshotPayload p,
            NativeHashMap<FixedString32Bytes, Entity> map)
        {
            foreach (var s in p.LoyaltySnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                em.AddComponentData(e, new FactionLoyaltyComponent
                {
                    Current = s.Current,
                    Max = s.Max,
                    Floor = s.Floor,
                });
            }
        }

        private static void RestoreResources(EntityManager em, BloodlinesSnapshotPayload p,
            NativeHashMap<FixedString32Bytes, Entity> map)
        {
            foreach (var s in p.ResourceSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                em.AddComponentData(e, new ResourceStockpileComponent
                {
                    Gold = s.Gold,
                    Food = s.Food,
                    Water = s.Water,
                    Wood = s.Wood,
                    Stone = s.Stone,
                    Iron = s.Iron,
                    Influence = s.Influence,
                });
            }
        }

        private static void RestoreRealmConditions(EntityManager em, BloodlinesSnapshotPayload p,
            NativeHashMap<FixedString32Bytes, Entity> map)
        {
            foreach (var s in p.RealmConditionSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                em.AddComponentData(e, new RealmConditionComponent
                {
                    CycleAccumulator = s.CycleAccumulator,
                    CycleCount = s.CycleCount,
                    FoodStrainStreak = s.FoodStrainStreak,
                    WaterStrainStreak = s.WaterStrainStreak,
                    AssaultFailureStrain = s.AssaultFailureStrain,
                    CohesionPenaltyUntil = s.CohesionPenaltyUntil,
                    LastStarvationResponseCycle = s.LastStarvationResponseCycle,
                    LastCapPressureResponseCycle = s.LastCapPressureResponseCycle,
                    LastStabilitySurplusResponseCycle = s.LastStabilitySurplusResponseCycle,
                });
            }
        }

        private static void RestoreConviction(EntityManager em, BloodlinesSnapshotPayload p,
            NativeHashMap<FixedString32Bytes, Entity> map)
        {
            foreach (var s in p.ConvictionSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                em.AddComponentData(e, new ConvictionComponent
                {
                    Ruthlessness = s.Ruthlessness,
                    Stewardship = s.Stewardship,
                    Oathkeeping = s.Oathkeeping,
                    Desecration = s.Desecration,
                    Score = s.Score,
                    Band = s.Band,
                });
            }
        }

        private static void RestoreDynasty(EntityManager em, BloodlinesSnapshotPayload p,
            NativeHashMap<FixedString32Bytes, Entity> map)
        {
            // Dynasty state
            foreach (var s in p.DynastyStateSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                em.AddComponentData(e, new DynastyStateComponent
                {
                    ActiveMemberCap = s.ActiveMemberCap,
                    DormantReserve = s.DormantReserve,
                    Legitimacy = s.Legitimacy,
                    LoyaltyPressure = s.LoyaltyPressure,
                    Interregnum = s.Interregnum,
                });
                em.AddBuffer<DynastyMemberRef>(e);
                em.AddBuffer<DynastyFallenLedger>(e);
            }

            // Dynasty members: create member entities and link via buffer
            foreach (var s in p.DynastyMemberSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var factionEntity)) continue;
                if (!em.HasBuffer<DynastyMemberRef>(factionEntity)) continue;

                var memberEntity = em.CreateEntity();
                em.AddComponentData(memberEntity, new FactionComponent { FactionId = s.FactionId });
                em.AddComponentData(memberEntity, new DynastyMemberComponent
                {
                    MemberId = s.MemberId,
                    Title = s.Title,
                    Role = s.Role,
                    Path = s.Path,
                    AgeYears = s.AgeYears,
                    Status = s.Status,
                    Renown = s.Renown,
                    Order = s.Order,
                    FallenAtWorldSeconds = s.FallenAtWorldSeconds,
                });
                em.GetBuffer<DynastyMemberRef>(factionEntity).Add(new DynastyMemberRef { Member = memberEntity });
            }

            // Fallen ledger
            foreach (var s in p.FallenLedgerSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var factionEntity)) continue;
                if (!em.HasBuffer<DynastyFallenLedger>(factionEntity)) continue;
                em.GetBuffer<DynastyFallenLedger>(factionEntity).Add(new DynastyFallenLedger
                {
                    MemberId = s.MemberId,
                    Title = s.Title,
                    Role = s.Role,
                    FallenAtWorldSeconds = s.FallenAtWorldSeconds,
                });
            }
        }

        private static void RestoreFaith(EntityManager em, BloodlinesSnapshotPayload p,
            NativeHashMap<FixedString32Bytes, Entity> map)
        {
            foreach (var s in p.FaithStateSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                em.AddComponentData(e, new FaithStateComponent
                {
                    SelectedFaith = s.SelectedFaith,
                    DoctrinePath = s.DoctrinePath,
                    Intensity = s.Intensity,
                    Level = s.Level,
                });
                em.AddBuffer<FaithExposureElement>(e);
            }

            foreach (var s in p.FaithExposureSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                if (!em.HasBuffer<FaithExposureElement>(e)) continue;
                em.GetBuffer<FaithExposureElement>(e).Add(new FaithExposureElement
                {
                    Faith = s.Faith,
                    Exposure = s.Exposure,
                    Discovered = s.Discovered,
                });
            }
        }

        private static void RestorePopulation(EntityManager em, BloodlinesSnapshotPayload p,
            NativeHashMap<FixedString32Bytes, Entity> map)
        {
            foreach (var s in p.PopulationSnapshots)
            {
                if (!map.TryGetValue(new FixedString32Bytes(s.FactionId), out var e)) continue;
                em.AddComponentData(e, new PopulationComponent
                {
                    Total = s.Total,
                    Available = s.Available,
                    Cap = s.Cap,
                    BaseCap = s.BaseCap,
                    CapBonus = s.CapBonus,
                    GrowthAccumulator = s.GrowthAccumulator,
                });
            }
        }
    }
}
