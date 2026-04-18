using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Bloodlines.SaveLoad
{
    /// <summary>
    /// Captures all live faction-scoped ECS simulation state into a
    /// BloodlinesSnapshotPayload. Keyed by FactionId (FixedString32Bytes)
    /// rather than entity index for stable identity across restore.
    ///
    /// Browser reference: simulation.js:13822 exportStateSnapshot
    /// (faction, conviction, dynasty, faith, population sub-objects)
    ///
    /// Snapshot discipline: simulation state only. No rendering proxies,
    /// no HUD state, no camera -- mirrors the browser snapshot contract.
    /// </summary>
    public static class BloodlinesSnapshotWriter
    {
        public static void Capture(EntityManager em, out BloodlinesSnapshotPayload payload)
        {
            payload = new BloodlinesSnapshotPayload
            {
                CapturedAtElapsedSeconds = (float)em.WorldUnmanaged.Time.ElapsedTime,
            };

            CaptureFactions(em, payload);
            CaptureDynastyMembers(em, payload);
        }

        public static void CaptureIndented(EntityManager em, out string json)
        {
            Capture(em, out var payload);
            json = JsonUtility.ToJson(payload, prettyPrint: true);
        }

        public static string Serialize(BloodlinesSnapshotPayload payload)
        {
            return JsonUtility.ToJson(payload);
        }

        public static BloodlinesSnapshotPayload Deserialize(string json)
        {
            return JsonUtility.FromJson<BloodlinesSnapshotPayload>(json);
        }

        private static void CaptureFactions(EntityManager em, BloodlinesSnapshotPayload payload)
        {
            // Exclude DynastyMemberComponent to skip member entities that also carry FactionComponent.
            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.Exclude<DynastyMemberComponent>());
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                var e = factionEntities[i];
                var faction = em.GetComponentData<FactionComponent>(e);
                var factionId = faction.FactionId.ToString();

                // Faction identity
                payload.FactionSnapshots.Add(new FactionSnapshot
                {
                    FactionId = factionId,
                    HouseId = em.HasComponent<FactionHouseComponent>(e)
                        ? em.GetComponentData<FactionHouseComponent>(e).HouseId.ToString()
                        : null,
                    Kind = em.HasComponent<FactionKindComponent>(e)
                        ? em.GetComponentData<FactionKindComponent>(e).Kind.ToString()
                        : null,
                });

                // Loyalty
                if (em.HasComponent<FactionLoyaltyComponent>(e))
                {
                    var c = em.GetComponentData<FactionLoyaltyComponent>(e);
                    payload.LoyaltySnapshots.Add(new LoyaltySnapshot
                    {
                        FactionId = factionId,
                        Current = c.Current,
                        Max = c.Max,
                        Floor = c.Floor,
                    });
                }

                // Resources
                if (em.HasComponent<ResourceStockpileComponent>(e))
                {
                    var c = em.GetComponentData<ResourceStockpileComponent>(e);
                    payload.ResourceSnapshots.Add(new ResourceSnapshot
                    {
                        FactionId = factionId,
                        Gold = c.Gold,
                        Food = c.Food,
                        Water = c.Water,
                        Wood = c.Wood,
                        Stone = c.Stone,
                        Iron = c.Iron,
                        Influence = c.Influence,
                    });
                }

                // Realm conditions
                if (em.HasComponent<RealmConditionComponent>(e))
                {
                    var c = em.GetComponentData<RealmConditionComponent>(e);
                    payload.RealmConditionSnapshots.Add(new RealmConditionSnapshot
                    {
                        FactionId = factionId,
                        CycleAccumulator = c.CycleAccumulator,
                        CycleCount = c.CycleCount,
                        FoodStrainStreak = c.FoodStrainStreak,
                        WaterStrainStreak = c.WaterStrainStreak,
                        AssaultFailureStrain = c.AssaultFailureStrain,
                        CohesionPenaltyUntil = c.CohesionPenaltyUntil,
                        LastStarvationResponseCycle = c.LastStarvationResponseCycle,
                        LastCapPressureResponseCycle = c.LastCapPressureResponseCycle,
                        LastStabilitySurplusResponseCycle = c.LastStabilitySurplusResponseCycle,
                    });
                }

                // Conviction
                if (em.HasComponent<ConvictionComponent>(e))
                {
                    var c = em.GetComponentData<ConvictionComponent>(e);
                    payload.ConvictionSnapshots.Add(new ConvictionSnapshot
                    {
                        FactionId = factionId,
                        Ruthlessness = c.Ruthlessness,
                        Stewardship = c.Stewardship,
                        Oathkeeping = c.Oathkeeping,
                        Desecration = c.Desecration,
                        Score = c.Score,
                        Band = c.Band,
                    });
                }

                // Dynasty state
                if (em.HasComponent<DynastyStateComponent>(e))
                {
                    var c = em.GetComponentData<DynastyStateComponent>(e);
                    payload.DynastyStateSnapshots.Add(new DynastyStateSnapshot
                    {
                        FactionId = factionId,
                        ActiveMemberCap = c.ActiveMemberCap,
                        DormantReserve = c.DormantReserve,
                        Legitimacy = c.Legitimacy,
                        LoyaltyPressure = c.LoyaltyPressure,
                        Interregnum = c.Interregnum,
                    });

                    // Dynasty member references (member entities are separate)
                    if (em.HasBuffer<DynastyMemberRef>(e))
                    {
                        var memberBuf = em.GetBuffer<DynastyMemberRef>(e);
                        for (int m = 0; m < memberBuf.Length; m++)
                        {
                            var memberEntity = memberBuf[m].Member;
                            if (!em.HasComponent<DynastyMemberComponent>(memberEntity)) continue;
                            var mc = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                            payload.DynastyMemberSnapshots.Add(new DynastyMemberSnapshot
                            {
                                FactionId = factionId,
                                MemberId = mc.MemberId.ToString(),
                                Title = mc.Title.ToString(),
                                Role = mc.Role,
                                Path = mc.Path,
                                AgeYears = mc.AgeYears,
                                Status = mc.Status,
                                Renown = mc.Renown,
                                Order = mc.Order,
                                FallenAtWorldSeconds = mc.FallenAtWorldSeconds,
                            });
                        }
                    }

                    // Fallen ledger
                    if (em.HasBuffer<DynastyFallenLedger>(e))
                    {
                        var fallen = em.GetBuffer<DynastyFallenLedger>(e);
                        for (int m = 0; m < fallen.Length; m++)
                        {
                            var entry = fallen[m];
                            payload.FallenLedgerSnapshots.Add(new DynastyFallenLedgerEntrySnapshot
                            {
                                FactionId = factionId,
                                MemberId = entry.MemberId.ToString(),
                                Title = entry.Title.ToString(),
                                Role = entry.Role,
                                FallenAtWorldSeconds = entry.FallenAtWorldSeconds,
                            });
                        }
                    }
                }

                // Faith state
                if (em.HasComponent<FaithStateComponent>(e))
                {
                    var c = em.GetComponentData<FaithStateComponent>(e);
                    payload.FaithStateSnapshots.Add(new FaithStateSnapshot
                    {
                        FactionId = factionId,
                        SelectedFaith = c.SelectedFaith,
                        DoctrinePath = c.DoctrinePath,
                        Intensity = c.Intensity,
                        Level = c.Level,
                    });

                    if (em.HasBuffer<FaithExposureElement>(e))
                    {
                        var exposureBuf = em.GetBuffer<FaithExposureElement>(e);
                        for (int f = 0; f < exposureBuf.Length; f++)
                        {
                            var entry = exposureBuf[f];
                            payload.FaithExposureSnapshots.Add(new FaithExposureSnapshot
                            {
                                FactionId = factionId,
                                Faith = entry.Faith,
                                Exposure = entry.Exposure,
                                Discovered = entry.Discovered,
                            });
                        }
                    }
                }

                // Population
                if (em.HasComponent<PopulationComponent>(e))
                {
                    var c = em.GetComponentData<PopulationComponent>(e);
                    payload.PopulationSnapshots.Add(new PopulationSnapshot
                    {
                        FactionId = factionId,
                        Total = c.Total,
                        Available = c.Available,
                        Cap = c.Cap,
                        BaseCap = c.BaseCap,
                        CapBonus = c.CapBonus,
                        GrowthAccumulator = c.GrowthAccumulator,
                    });
                }
            }
        }

        private static void CaptureDynastyMembers(EntityManager em, BloodlinesSnapshotPayload payload)
        {
            // Dynasty members are already captured via faction's DynastyMemberRef buffer
            // in CaptureFactions. This method exists as a hook for future standalone
            // member-entity queries if member entities gain additional components.
        }
    }
}
