using Bloodlines.Components;
using Bloodlines.Conviction;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Applies daily loyalty drift to every active lesser house in each
    /// faction's LesserHouseElement buffer, gated by DualClockComponent in-world
    /// days so exactly one drift is applied per in-world day elapsed.
    /// When loyalty drops to or below zero, marks the house as Defected and
    /// spawns a new minor-house faction entity.
    ///
    /// Structural change safety: buffer writes are completed BEFORE any entity
    /// creation so DynamicBuffer references remain valid throughout the loop.
    ///
    /// Browser reference: simulation.js tickLesserHouseLoyaltyDrift (~6631),
    /// spawnDefectedMinorFaction (~6851).
    /// Canonical inputs:
    /// - legitimacy/oathkeeping/ruthlessness/fallen-ledger base delta
    /// - mixed-bloodline pressure
    /// - marriage-anchor pressure, including dissolved and hostile states
    /// - world-pressure cadence for targeted realms
    /// - 5-day grace between loyalty reaching zero and actual defection
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct LesserHouseLoyaltyDriftSystem : ISystem
    {
        private const float HighLegitimacyDelta = 0.30f;
        private const float MidLegitimacyDelta = 0.15f;
        private const float LowLegitimacyPenalty = -0.40f;
        private const float ConvictionStepDelta = 0.20f;
        private const float FallenLedgerPenalty = -0.50f;
        private const float MixedBloodlineDelta = -0.25f;
        private const float ActiveMarriageAnchorDelta = 0.15f;
        private const float ActiveMarriageChildDelta = 0.06f;
        private const float ActiveMarriageChildDeltaCap = 0.12f;
        private const float DissolvedMarriageAnchorDelta = -0.12f;
        private const float DissolvedMarriageChildDelta = -0.04f;
        private const float DissolvedMarriageChildDeltaCap = -0.12f;
        private const float HostilityMarriagePenalty = -0.35f;
        private const float WorldPressureLevelDelta = -0.12f;
        private const float WorldPressureScoreStepDelta = -0.03f;
        private const float WorldPressureScoreBonusCap = -0.12f;
        private const float DefectionThreshold = 0f;
        private const int DefectionGraceDays = 5;
        private const float DefectionLegitimacyPenalty = 6f;
        private const float DefectionRuthlessnessGain = 1f;
        private const float UnsetInWorldDays = -1f;
        private const float MinorHouseClaimRadiusTiles = 2.8f;
        private const float MinorHouseClaimCaptureTimeSeconds = 9f;
        private const float MinorHouseClaimInitialLoyalty = 62f;
        private const float MinorHouseClaimFoodTrickle = 0.08f;
        private const float MinorHouseClaimInfluenceTrickle = 0.06f;
        private const float MinorHouseSpawnLegitimacy = 30f;
        private const float MinorHouseSpawnFaithIntensity = 20f;
        private const int MinorHouseSpawnFaithLevel = 1;
        private static readonly float2[] ClaimOffsets =
        {
            new float2(36f, 36f),
            new float2(-36f, 36f),
            new float2(36f, -36f),
            new float2(-36f, -36f),
            new float2(48f, 0f),
            new float2(-48f, 0f),
            new float2(0f, 48f),
            new float2(0f, -48f),
        };

        private struct CadetWorldPressureProfile
        {
            public LesserHouseWorldPressureStatus Status;
            public float Delta;
            public int Level;
        }

        private struct MaritalAnchorProfile
        {
            public FixedString32Bytes HouseId;
            public FixedString64Bytes MarriageId;
            public LesserHouseMaritalAnchorStatus Status;
            public float Delta;
            public int ChildCount;
        }

        private struct DefectionRecord
        {
            public Entity ParentFactionEntity;
            public LesserHouseElement LesserHouse;
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LesserHouseElement>();
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            float inWorldDays = GetInWorldDays(em);
            int currentDay = (int)math.floor(inWorldDays);

            var factionHouseQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionHouseComponent>());
            using var houseFactionIds = factionHouseQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var houseIds = factionHouseQuery.ToComponentDataArray<FactionHouseComponent>(Allocator.Temp);
            factionHouseQuery.Dispose();

            var marriageQuery = em.CreateEntityQuery(ComponentType.ReadOnly<MarriageComponent>());
            using var marriageEntities = marriageQuery.ToEntityArray(Allocator.Temp);
            using var marriages = marriageQuery.ToComponentDataArray<MarriageComponent>(Allocator.Temp);
            marriageQuery.Dispose();

            var factionQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                typeof(LesserHouseElement));
            using var factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using var factionComps = factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            factionQuery.Dispose();

            // Collect defection events so we can spawn after all buffer writes.
            var defections = new NativeList<DefectionRecord>(4, Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                var factionEntity = factionEntities[i];
                var factionId = factionComps[i].FactionId;
                var buffer = em.GetBuffer<LesserHouseElement>(factionEntity);
                if (buffer.Length == 0)
                {
                    continue;
                }

                float baseDailyDelta = ComputeBaseDailyDelta(em, factionEntity);
                int activeLesserHouseCount = CountActiveLesserHouses(buffer);
                var worldPressure = BuildCadetWorldPressureProfile(
                    em,
                    factionEntity,
                    activeLesserHouseCount);
                var parentHouseId = em.HasComponent<FactionHouseComponent>(factionEntity)
                    ? em.GetComponentData<FactionHouseComponent>(factionEntity).HouseId
                    : default;

                for (int j = 0; j < buffer.Length; j++)
                {
                    var lh = buffer[j];
                    if (lh.Defected)
                    {
                        continue;
                    }

                    var mixedHouseId = ResolveMixedBloodlineHouseId(
                        em,
                        factionEntity,
                        lh,
                        parentHouseId);
                    float mixedBloodlineDelta = mixedHouseId.Length > 0
                        ? MixedBloodlineDelta
                        : 0f;
                    var maritalAnchor = BuildMaritalAnchorProfile(
                        em,
                        factionEntity,
                        factionId,
                        mixedHouseId,
                        houseFactionIds,
                        houseIds,
                        marriageEntities,
                        marriages);
                    float dailyDelta = baseDailyDelta
                        + mixedBloodlineDelta
                        + maritalAnchor.Delta
                        + worldPressure.Delta;

                    lh.MixedBloodlineHouseId = mixedHouseId;
                    lh.MixedBloodlinePressure = mixedBloodlineDelta;
                    lh.MaritalAnchorHouseId = maritalAnchor.HouseId;
                    lh.MaritalAnchorMarriageId = maritalAnchor.MarriageId;
                    lh.MaritalAnchorStatus = maritalAnchor.Status;
                    lh.MaritalAnchorPressure = maritalAnchor.Delta;
                    lh.MaritalAnchorChildCount = maritalAnchor.ChildCount;
                    lh.WorldPressureStatus = worldPressure.Status;
                    lh.WorldPressurePressure = worldPressure.Delta;
                    lh.WorldPressureLevel = worldPressure.Level;
                    lh.CurrentDailyLoyaltyDelta = dailyDelta;
                    lh.LastMaritalAnchorStatus = maritalAnchor.Status;

                    int lastDay = lh.LastDriftAppliedInWorldDays >= 0f
                        ? (int)math.floor(lh.LastDriftAppliedInWorldDays)
                        : currentDay;
                    int elapsedDays = math.max(0, currentDay - lastDay);
                    float newLoyalty = lh.Loyalty;
                    if (elapsedDays > 0)
                    {
                        newLoyalty = math.clamp(
                            lh.Loyalty + dailyDelta * elapsedDays,
                            0f,
                            100f);
                        lh.Loyalty = newLoyalty;
                        lh.LastDriftAppliedInWorldDays = currentDay;
                    }

                    if (newLoyalty <= DefectionThreshold)
                    {
                        if (lh.BrokeAtLoyaltyZeroInWorldDays < 0f)
                        {
                            lh.BrokeAtLoyaltyZeroInWorldDays = currentDay;
                        }

                        int graceElapsed = currentDay - (int)math.floor(lh.BrokeAtLoyaltyZeroInWorldDays);
                        if (graceElapsed >= DefectionGraceDays)
                        {
                            lh.Loyalty = 0f;
                            lh.Defected = true;
                            lh.DepartedAtInWorldDays = currentDay;
                            ApplyDefectionConsequences(em, factionEntity);
                            defections.Add(new DefectionRecord
                            {
                                ParentFactionEntity = factionEntity,
                                LesserHouse = lh,
                            });
                        }
                    }
                    else
                    {
                        lh.BrokeAtLoyaltyZeroInWorldDays = UnsetInWorldDays;
                    }

                    buffer[j] = lh;
                }
            }

            // Now safe to spawn: all DynamicBuffer references have been released.
            for (int d = 0; d < defections.Length; d++)
            {
                SpawnDefectedMinorFaction(em, defections[d]);
            }

            defections.Dispose();
        }

        private static int CountActiveLesserHouses(DynamicBuffer<LesserHouseElement> buffer)
        {
            int count = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].Defected)
                {
                    count++;
                }
            }

            return count;
        }

        private static float ComputeBaseDailyDelta(EntityManager em, Entity factionEntity)
        {
            float delta = 0f;

            if (em.HasComponent<DynastyStateComponent>(factionEntity))
            {
                float legitimacy = em.GetComponentData<DynastyStateComponent>(factionEntity).Legitimacy;
                if (legitimacy >= 75f)
                {
                    delta += HighLegitimacyDelta;
                }
                else if (legitimacy >= 50f)
                {
                    delta += MidLegitimacyDelta;
                }

                if (legitimacy < 30f)
                {
                    delta += LowLegitimacyPenalty;
                }
            }

            if (em.HasComponent<ConvictionComponent>(factionEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(factionEntity);
                delta += math.floor(conviction.Oathkeeping / 5f) * ConvictionStepDelta;
                delta -= math.floor(conviction.Ruthlessness / 5f) * ConvictionStepDelta;
            }

            if (em.HasBuffer<DynastyFallenLedger>(factionEntity) &&
                em.GetBuffer<DynastyFallenLedger>(factionEntity).Length > 0)
            {
                delta += FallenLedgerPenalty;
            }

            return delta;
        }

        private static CadetWorldPressureProfile BuildCadetWorldPressureProfile(
            EntityManager em,
            Entity factionEntity,
            int activeLesserHouseCount)
        {
            var quiet = new CadetWorldPressureProfile
            {
                Status = LesserHouseWorldPressureStatus.Quiet,
                Delta = 0f,
                Level = 0,
            };

            if (activeLesserHouseCount <= 0 ||
                !em.HasComponent<WorldPressureComponent>(factionEntity))
            {
                return quiet;
            }

            var pressure = em.GetComponentData<WorldPressureComponent>(factionEntity);
            if (!pressure.Targeted || pressure.Level <= 0)
            {
                return quiet;
            }

            float scoreBonus = math.max(
                0f,
                math.min(
                    -WorldPressureScoreBonusCap,
                    math.max(0, pressure.Score - 4) * -WorldPressureScoreStepDelta));
            return new CadetWorldPressureProfile
            {
                Status = pressure.Level switch
                {
                    3 => LesserHouseWorldPressureStatus.Convergence,
                    2 => LesserHouseWorldPressureStatus.Overwhelming,
                    1 => LesserHouseWorldPressureStatus.Gathering,
                    _ => LesserHouseWorldPressureStatus.Quiet,
                },
                Delta = (pressure.Level * WorldPressureLevelDelta) - scoreBonus,
                Level = pressure.Level,
            };
        }

        private static FixedString32Bytes ResolveMixedBloodlineHouseId(
            EntityManager em,
            Entity factionEntity,
            in LesserHouseElement lesserHouse,
            FixedString32Bytes parentHouseId)
        {
            if (lesserHouse.MixedBloodlineHouseId.Length > 0 &&
                !lesserHouse.MixedBloodlineHouseId.Equals(parentHouseId))
            {
                return lesserHouse.MixedBloodlineHouseId;
            }

            if (factionEntity == Entity.Null || !em.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return default;
            }

            var members = em.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < members.Length; i++)
            {
                var memberEntity = members[i].Member;
                if (memberEntity == Entity.Null ||
                    !em.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = em.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (!member.MemberId.Equals(lesserHouse.FounderMemberId) ||
                    !em.HasComponent<DynastyMixedBloodlineComponent>(memberEntity))
                {
                    continue;
                }

                var mixedBloodline = em.GetComponentData<DynastyMixedBloodlineComponent>(memberEntity);
                if (mixedBloodline.SpouseHouseId.Length == 0 ||
                    mixedBloodline.SpouseHouseId.Equals(parentHouseId))
                {
                    return default;
                }

                return mixedBloodline.SpouseHouseId;
            }

            return default;
        }

        private static MaritalAnchorProfile BuildMaritalAnchorProfile(
            EntityManager em,
            Entity parentFactionEntity,
            FixedString32Bytes parentFactionId,
            FixedString32Bytes mixedHouseId,
            NativeArray<FactionComponent> factionIds,
            NativeArray<FactionHouseComponent> houseIds,
            NativeArray<Entity> marriageEntities,
            NativeArray<MarriageComponent> marriages)
        {
            if (mixedHouseId.Length == 0)
            {
                return new MaritalAnchorProfile
                {
                    Status = LesserHouseMaritalAnchorStatus.None,
                };
            }

            int selectedMarriageIndex = -1;
            float selectedRecency = float.MinValue;
            bool selectedMarriageActive = false;

            for (int i = 0; i < marriages.Length; i++)
            {
                var marriage = marriages[i];
                if (!TryResolveCounterpartyFactionId(parentFactionId, marriage, out var otherFactionId) ||
                    !TryResolveHouseId(otherFactionId, factionIds, houseIds, out var otherHouseId) ||
                    !otherHouseId.Equals(mixedHouseId))
                {
                    continue;
                }

                bool active = !marriage.Dissolved;
                float recency = active
                    ? marriage.MarriedAtInWorldDays
                    : math.max(marriage.DissolvedAtInWorldDays, marriage.MarriedAtInWorldDays);

                if (active)
                {
                    if (!selectedMarriageActive || recency > selectedRecency)
                    {
                        selectedMarriageIndex = i;
                        selectedRecency = recency;
                        selectedMarriageActive = true;
                    }
                }
                else if (!selectedMarriageActive && recency > selectedRecency)
                {
                    selectedMarriageIndex = i;
                    selectedRecency = recency;
                }
            }

            var anchor = new MaritalAnchorProfile
            {
                HouseId = mixedHouseId,
                Status = LesserHouseMaritalAnchorStatus.None,
                Delta = 0f,
                ChildCount = 0,
            };

            if (selectedMarriageIndex >= 0)
            {
                var marriage = marriages[selectedMarriageIndex];
                anchor.MarriageId = marriage.MarriageId;
                anchor.ChildCount = ResolveMarriageChildCount(
                    em,
                    marriage.MarriageId,
                    marriageEntities,
                    marriages);

                if (!marriage.Dissolved)
                {
                    anchor.Status = LesserHouseMaritalAnchorStatus.Active;
                    anchor.Delta += ActiveMarriageAnchorDelta;
                    anchor.Delta += math.min(
                        ActiveMarriageChildDeltaCap,
                        anchor.ChildCount * ActiveMarriageChildDelta);
                }
                else
                {
                    anchor.Status = LesserHouseMaritalAnchorStatus.Dissolved;
                    anchor.Delta += DissolvedMarriageAnchorDelta;
                    anchor.Delta += math.max(
                        DissolvedMarriageChildDeltaCap,
                        anchor.ChildCount * DissolvedMarriageChildDelta);
                }
            }

            if (HasHostilityToHouse(em, parentFactionEntity, mixedHouseId, factionIds, houseIds))
            {
                anchor.Delta += HostilityMarriagePenalty;
                anchor.Status = selectedMarriageIndex >= 0 && !marriages[selectedMarriageIndex].Dissolved
                    ? LesserHouseMaritalAnchorStatus.Strained
                    : LesserHouseMaritalAnchorStatus.Fractured;
            }

            return anchor;
        }

        private static bool TryResolveCounterpartyFactionId(
            FixedString32Bytes parentFactionId,
            in MarriageComponent marriage,
            out FixedString32Bytes otherFactionId)
        {
            otherFactionId = default;
            if (marriage.HeadFactionId.Equals(parentFactionId))
            {
                otherFactionId = marriage.SpouseFactionId;
                return true;
            }

            if (marriage.SpouseFactionId.Equals(parentFactionId))
            {
                otherFactionId = marriage.HeadFactionId;
                return true;
            }

            return false;
        }

        private static bool TryResolveHouseId(
            FixedString32Bytes factionId,
            NativeArray<FactionComponent> factionIds,
            NativeArray<FactionHouseComponent> houseIds,
            out FixedString32Bytes houseId)
        {
            for (int i = 0; i < factionIds.Length; i++)
            {
                if (factionIds[i].FactionId.Equals(factionId))
                {
                    houseId = houseIds[i].HouseId;
                    return true;
                }
            }

            houseId = default;
            return false;
        }

        private static int ResolveMarriageChildCount(
            EntityManager em,
            FixedString64Bytes marriageId,
            NativeArray<Entity> marriageEntities,
            NativeArray<MarriageComponent> marriages)
        {
            int childCount = 0;
            for (int i = 0; i < marriages.Length; i++)
            {
                if (!marriages[i].MarriageId.Equals(marriageId) ||
                    !em.HasBuffer<MarriageChildElement>(marriageEntities[i]))
                {
                    continue;
                }

                childCount = math.max(
                    childCount,
                    em.GetBuffer<MarriageChildElement>(marriageEntities[i]).Length);
            }

            return childCount;
        }

        private static bool HasHostilityToHouse(
            EntityManager em,
            Entity parentFactionEntity,
            FixedString32Bytes mixedHouseId,
            NativeArray<FactionComponent> factionIds,
            NativeArray<FactionHouseComponent> houseIds)
        {
            if (!em.HasBuffer<HostilityComponent>(parentFactionEntity))
            {
                return false;
            }

            var hostilityBuffer = em.GetBuffer<HostilityComponent>(parentFactionEntity);
            for (int i = 0; i < hostilityBuffer.Length; i++)
            {
                if (!TryResolveHouseId(hostilityBuffer[i].HostileFactionId, factionIds, houseIds, out var houseId))
                {
                    continue;
                }

                if (houseId.Equals(mixedHouseId))
                {
                    return true;
                }
            }

            return false;
        }

        private static void ApplyDefectionConsequences(EntityManager em, Entity factionEntity)
        {
            if (em.HasComponent<DynastyStateComponent>(factionEntity))
            {
                var dynasty = em.GetComponentData<DynastyStateComponent>(factionEntity);
                dynasty.Legitimacy = math.clamp(
                    dynasty.Legitimacy - DefectionLegitimacyPenalty,
                    0f,
                    100f);
                em.SetComponentData(factionEntity, dynasty);
            }

            if (em.HasComponent<ConvictionComponent>(factionEntity))
            {
                var conviction = em.GetComponentData<ConvictionComponent>(factionEntity);
                ConvictionScoring.ApplyEvent(
                    ref conviction,
                    ConvictionBucket.Ruthlessness,
                    DefectionRuthlessnessGain);
                em.SetComponentData(factionEntity, conviction);
            }
        }

        private static void SpawnDefectedMinorFaction(EntityManager em, in DefectionRecord defection)
        {
            var minorFactionId = new FixedString32Bytes("minor-");
            minorFactionId.Append(defection.LesserHouse.HouseId);

            var existingQuery = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>());
            using var existing = existingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            existingQuery.Dispose();
            for (int k = 0; k < existing.Length; k++)
            {
                if (existing[k].FactionId.Equals(minorFactionId))
                {
                    return;
                }
            }

            var minorEntity = em.CreateEntity();
            em.AddComponentData(minorEntity, new FactionComponent { FactionId = minorFactionId });
            em.AddComponentData(minorEntity, new FactionKindComponent { Kind = FactionKind.MinorHouse });
            em.AddComponentData(minorEntity, new FactionHouseComponent
            {
                HouseId = defection.LesserHouse.HouseId,
            });
            em.AddComponentData(minorEntity, new ResourceStockpileComponent());
            em.AddComponentData(minorEntity, new PopulationComponent());
            em.AddComponentData(minorEntity, new DynastyStateComponent
            {
                Legitimacy = MinorHouseSpawnLegitimacy,
            });
            if (em.HasComponent<FaithStateComponent>(defection.ParentFactionEntity))
            {
                var parentFaith = em.GetComponentData<FaithStateComponent>(defection.ParentFactionEntity);
                if (parentFaith.SelectedFaith != CovenantId.None)
                {
                    em.AddComponentData(minorEntity, new FaithStateComponent
                    {
                        SelectedFaith = parentFaith.SelectedFaith,
                        DoctrinePath = parentFaith.DoctrinePath,
                        Intensity = MinorHouseSpawnFaithIntensity,
                        Level = MinorHouseSpawnFaithLevel,
                    });
                }
            }
            em.AddComponentData(minorEntity, new AIEconomyControllerComponent { Enabled = true });

            FixedString32Bytes parentFactionId = em.GetComponentData<FactionComponent>(defection.ParentFactionEntity).FactionId;
            FixedString32Bytes claimControlPointId = EnsureMinorHouseClaim(
                em,
                defection.ParentFactionEntity,
                minorEntity,
                defection.LesserHouse);
            em.AddComponentData(minorEntity, new MinorHouseLevyComponent
            {
                OriginFactionId = parentFactionId,
                ClaimControlPointId = claimControlPointId,
                LevyIntervalSeconds = 22f,
                LevyAccumulator     = 0f,
                LeviesIssued        = 0,
                LevyStatus = MinorHouseLevyStatus.Forming,
                LevyUnitId = new FixedString64Bytes("militia"),
                RetinueCap = 1,
                RetinueCount = 0,
                LastLevyAtInWorldDays = UnsetInWorldDays,
                ParentPressureLevel = 0,
                ParentPressureStatus = LesserHouseWorldPressureStatus.Quiet,
                ParentPressureLevyTempo = 1f,
                ParentPressureRetakeTempo = 1f,
                ParentPressureRetinueBonus = 0,
            });

            EnsureMutualHostility(em, defection.ParentFactionEntity, minorEntity);
        }

        private static FixedString32Bytes EnsureMinorHouseClaim(
            EntityManager em,
            Entity parentFactionEntity,
            Entity minorFactionEntity,
            in LesserHouseElement lesserHouse)
        {
            var minorFactionId = em.GetComponentData<FactionComponent>(minorFactionEntity).FactionId;
            var claimId = new FixedString32Bytes(minorFactionId);
            claimId.Append("-claim");

            var controlPointQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var controlPointEntities = controlPointQuery.ToEntityArray(Allocator.Temp);
            using var controlPoints = controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);
            using var controlPointPositions = controlPointQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            controlPointQuery.Dispose();

            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (controlPoints[i].ControlPointId.Equals(claimId))
                {
                    return claimId;
                }
            }

            ResolveMinorHouseClaimAnchor(
                em,
                parentFactionEntity,
                controlPoints,
                controlPointPositions,
                out float3 anchorPosition,
                out FixedString32Bytes continentId);

            float3 claimPosition = ResolveMinorHouseClaimPosition(controlPointPositions, anchorPosition);
            var claimEntity = em.CreateEntity();
            em.AddComponentData(claimEntity, new PositionComponent
            {
                Value = claimPosition,
            });
            em.AddComponentData(claimEntity, new LocalTransform
            {
                Position = claimPosition,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            em.AddComponentData(claimEntity, new ControlPointComponent
            {
                ControlPointId = claimId,
                OwnerFactionId = minorFactionId,
                CaptureFactionId = default,
                ContinentId = continentId,
                ControlState = ControlState.Stabilized,
                IsContested = false,
                Loyalty = MinorHouseClaimInitialLoyalty,
                CaptureProgress = 0f,
                SettlementClassId = new FixedString32Bytes("border_settlement"),
                FortificationTier = 0,
                RadiusTiles = MinorHouseClaimRadiusTiles,
                CaptureTimeSeconds = MinorHouseClaimCaptureTimeSeconds,
                GoldTrickle = 0f,
                FoodTrickle = MinorHouseClaimFoodTrickle,
                WaterTrickle = 0f,
                WoodTrickle = 0f,
                StoneTrickle = 0f,
                IronTrickle = 0f,
                InfluenceTrickle = MinorHouseClaimInfluenceTrickle,
            });

            return claimId;
        }

        private static void ResolveMinorHouseClaimAnchor(
            EntityManager em,
            Entity parentFactionEntity,
            NativeArray<ControlPointComponent> controlPoints,
            NativeArray<PositionComponent> controlPointPositions,
            out float3 anchorPosition,
            out FixedString32Bytes continentId)
        {
            anchorPosition = float3.zero;
            continentId = new FixedString32Bytes("home");

            if (parentFactionEntity == Entity.Null || !em.HasComponent<FactionComponent>(parentFactionEntity))
            {
                return;
            }

            FixedString32Bytes parentFactionId = em.GetComponentData<FactionComponent>(parentFactionEntity).FactionId;
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(parentFactionId))
                {
                    continue;
                }

                anchorPosition = controlPointPositions[i].Value;
                if (controlPoints[i].ContinentId.Length > 0)
                {
                    continentId = controlPoints[i].ContinentId;
                }
                return;
            }

            var settlementQuery = em.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<SettlementComponent>(),
                ComponentType.ReadOnly<PositionComponent>());
            using var settlementFactions = settlementQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var settlementPositions = settlementQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            settlementQuery.Dispose();

            for (int i = 0; i < settlementFactions.Length; i++)
            {
                if (!settlementFactions[i].FactionId.Equals(parentFactionId))
                {
                    continue;
                }

                anchorPosition = settlementPositions[i].Value;
                return;
            }
        }

        private static float3 ResolveMinorHouseClaimPosition(
            NativeArray<PositionComponent> controlPointPositions,
            float3 anchorPosition)
        {
            float3 fallback = anchorPosition + new float3(ClaimOffsets[0].x, 0f, ClaimOffsets[0].y);
            if (controlPointPositions.Length == 0)
            {
                return fallback;
            }

            float bestNearestDistanceSq = float.NegativeInfinity;
            float3 bestPosition = fallback;

            for (int offsetIndex = 0; offsetIndex < ClaimOffsets.Length; offsetIndex++)
            {
                float3 candidate = anchorPosition + new float3(
                    ClaimOffsets[offsetIndex].x,
                    0f,
                    ClaimOffsets[offsetIndex].y);
                float nearestDistanceSq = float.PositiveInfinity;
                for (int i = 0; i < controlPointPositions.Length; i++)
                {
                    float distanceSq = math.lengthsq(candidate - controlPointPositions[i].Value);
                    nearestDistanceSq = math.min(nearestDistanceSq, distanceSq);
                }

                if (nearestDistanceSq > bestNearestDistanceSq)
                {
                    bestNearestDistanceSq = nearestDistanceSq;
                    bestPosition = candidate;
                }
            }

            return bestPosition;
        }

        private static void EnsureMutualHostility(EntityManager em, Entity leftFaction, Entity rightFaction)
        {
            if (leftFaction == Entity.Null ||
                rightFaction == Entity.Null ||
                !em.HasComponent<FactionComponent>(leftFaction) ||
                !em.HasComponent<FactionComponent>(rightFaction))
            {
                return;
            }

            EnsureHostilityEntry(
                em,
                leftFaction,
                em.GetComponentData<FactionComponent>(rightFaction).FactionId);
            EnsureHostilityEntry(
                em,
                rightFaction,
                em.GetComponentData<FactionComponent>(leftFaction).FactionId);
        }

        private static void EnsureHostilityEntry(
            EntityManager em,
            Entity factionEntity,
            FixedString32Bytes hostileFactionId)
        {
            DynamicBuffer<HostilityComponent> hostilityBuffer = em.HasBuffer<HostilityComponent>(factionEntity)
                ? em.GetBuffer<HostilityComponent>(factionEntity)
                : em.AddBuffer<HostilityComponent>(factionEntity);

            for (int i = 0; i < hostilityBuffer.Length; i++)
            {
                if (hostilityBuffer[i].HostileFactionId.Equals(hostileFactionId))
                {
                    return;
                }
            }

            hostilityBuffer.Add(new HostilityComponent
            {
                HostileFactionId = hostileFactionId,
            });
        }

        private static float GetInWorldDays(EntityManager em)
        {
            var q = em.CreateEntityQuery(ComponentType.ReadOnly<DualClockComponent>());
            if (q.IsEmpty) { q.Dispose(); return 0f; }
            float d = q.GetSingleton<DualClockComponent>().InWorldDays;
            q.Dispose();
            return d;
        }
    }
}
