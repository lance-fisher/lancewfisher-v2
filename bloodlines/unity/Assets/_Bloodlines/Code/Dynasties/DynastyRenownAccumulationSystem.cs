using Bloodlines.Components;
using Bloodlines.GameTime;
using Bloodlines.Victory;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    /// <summary>
    /// Daily dynasty-level renown accumulation derived from live strategic state.
    /// Browser parity note: the browser currently only maintains per-member renown.
    /// This Unity slice lifts that into a dynasty-facing prestige surface using the
    /// design-bible prestige sources plus already-landed ECS gameplay state.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DynastyRenownAccumulationSystem : ISystem
    {
        private const float DefaultDecayRatePerDay = 0.45f;
        private const float TerritoryAdvantageWeightPerDay = 0.65f;
        private const float FaithIntensityWeightPerDay = 0.85f;
        private const float VictoryMomentumWeightPerDay = 1.15f;
        private const float WonMatchWeightPerDay = 3.0f;
        private const float LegitimateSuccessionBonus = 5.0f;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<DualClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            float inWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyStateComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factionComponents =
                factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            if (factionEntities.Length == 0)
            {
                return;
            }

            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < factionEntities.Length; i++)
            {
                var factionEntity = factionEntities[i];
                if (entityManager.HasComponent<DynastyRenownComponent>(factionEntity))
                {
                    continue;
                }

                ecb.AddComponent(factionEntity, new DynastyRenownComponent
                {
                    RenownScore = 0f,
                    LastRenownUpdateInWorldDays = inWorldDays,
                    RenownDecayRate = DefaultDecayRatePerDay,
                    PeakRenown = 0f,
                    LastRulingMemberId = default,
                    Initialized = 0,
                });
            }

            ecb.Playback(entityManager);

            var heldCounts = new NativeArray<int>(factionEntities.Length, Allocator.Temp);
            var loyalHeldCounts = new NativeArray<int>(factionEntities.Length, Allocator.Temp);
            var divineRightProgress = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            var commandHallFallProgress = new NativeArray<float>(factionEntities.Length, Allocator.Temp);
            var rulingMemberIds = new NativeArray<FixedString64Bytes>(factionEntities.Length, Allocator.Temp);

            int totalHeldCount = 0;
            for (int i = 0; i < factionEntities.Length; i++)
            {
                var factionId = factionComponents[i].FactionId;
                CountTerritory(entityManager, factionId, out int heldCount, out int loyalCount);
                heldCounts[i] = heldCount;
                loyalHeldCounts[i] = loyalCount;
                totalHeldCount += heldCount;
                divineRightProgress[i] = ComputeDivineRightProgress(entityManager, factionEntities[i]);
                commandHallFallProgress[i] = ComputeCommandHallFallProgress(entityManager, factionId);
                rulingMemberIds[i] = GetCurrentRulingMemberId(entityManager, factionEntities[i]);
            }

            float averageHeldCount = totalHeldCount > 0
                ? (float)totalHeldCount / factionEntities.Length
                : 0f;
            VictoryStateComponent victoryState = TryGetVictoryState(entityManager, out var resolvedVictory)
                ? resolvedVictory
                : default;

            try
            {
                for (int i = 0; i < factionEntities.Length; i++)
                {
                    var factionEntity = factionEntities[i];
                    if (!entityManager.HasComponent<DynastyRenownComponent>(factionEntity))
                    {
                        continue;
                    }

                    var renown = entityManager.GetComponentData<DynastyRenownComponent>(factionEntity);
                    float elapsedDays = inWorldDays - renown.LastRenownUpdateInWorldDays;
                    int wholeDays = (int)math.floor(elapsedDays);
                    if (wholeDays <= 0)
                    {
                        if (renown.Initialized == 0)
                        {
                            renown.LastRulingMemberId = rulingMemberIds[i];
                            renown.Initialized = 1;
                            entityManager.SetComponentData(factionEntity, renown);
                        }

                        continue;
                    }

                    float territoryContribution =
                        math.max(0f, heldCounts[i] - averageHeldCount) * TerritoryAdvantageWeightPerDay * wholeDays;

                    float faithContribution = 0f;
                    if (entityManager.HasComponent<FaithStateComponent>(factionEntity))
                    {
                        var faith = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
                        if (faith.SelectedFaith != CovenantId.None && faith.Intensity > 60f)
                        {
                            float normalizedIntensity = math.saturate((faith.Intensity - 60f) / 40f);
                            faithContribution = normalizedIntensity * FaithIntensityWeightPerDay * wholeDays;
                        }
                    }

                    float territorialProgress = heldCounts[i] > 0
                        ? (float)loyalHeldCounts[i] / heldCounts[i]
                        : 0f;
                    float victoryMomentum = math.max(territorialProgress, math.max(divineRightProgress[i], commandHallFallProgress[i]));
                    float victoryContribution =
                        factionComponents[i].FactionId.Equals(victoryState.WinnerFactionId) && victoryState.Status == MatchStatus.Won
                            ? WonMatchWeightPerDay * wholeDays
                            : math.select(0f, VictoryMomentumWeightPerDay * victoryMomentum * wholeDays, victoryMomentum >= 0.5f);

                    float successionBonus = 0f;
                    if (renown.Initialized != 0 &&
                        renown.LastRulingMemberId.Length > 0 &&
                        rulingMemberIds[i].Length > 0 &&
                        !renown.LastRulingMemberId.Equals(rulingMemberIds[i]) &&
                        entityManager.GetComponentData<DynastyStateComponent>(factionEntity).Interregnum == false)
                    {
                        successionBonus = LegitimateSuccessionBonus;
                    }

                    float decay = math.max(0f, renown.RenownDecayRate) * wholeDays;
                    renown.RenownScore = math.max(
                        0f,
                        renown.RenownScore + territoryContribution + faithContribution + victoryContribution + successionBonus - decay);
                    renown.PeakRenown = math.max(renown.PeakRenown, renown.RenownScore);
                    renown.LastRenownUpdateInWorldDays += wholeDays;
                    renown.LastRulingMemberId = rulingMemberIds[i];
                    renown.Initialized = 1;
                    entityManager.SetComponentData(factionEntity, renown);
                }
            }
            finally
            {
                heldCounts.Dispose();
                loyalHeldCounts.Dispose();
                divineRightProgress.Dispose();
                commandHallFallProgress.Dispose();
                rulingMemberIds.Dispose();
            }
        }

        private static bool TryGetVictoryState(EntityManager entityManager, out VictoryStateComponent victoryState)
        {
            using var victoryQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<VictoryStateComponent>());
            if (victoryQuery.IsEmpty)
            {
                victoryState = default;
                return false;
            }

            victoryState = victoryQuery.GetSingleton<VictoryStateComponent>();
            return true;
        }

        private static void CountTerritory(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out int heldCount,
            out int loyalHeldCount)
        {
            heldCount = 0;
            loyalHeldCount = 0;

            using var controlPointQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<ControlPointComponent> controlPoints =
                controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(factionId))
                {
                    continue;
                }

                heldCount++;
                if (controlPoints[i].Loyalty >= VictoryConditionEvaluationSystem.TerritorialGovernanceLoyaltyThreshold)
                {
                    loyalHeldCount++;
                }
            }
        }

        private static float ComputeDivineRightProgress(EntityManager entityManager, Entity factionEntity)
        {
            if (!entityManager.HasComponent<FaithStateComponent>(factionEntity))
            {
                return 0f;
            }

            var faith = entityManager.GetComponentData<FaithStateComponent>(factionEntity);
            if (faith.SelectedFaith == CovenantId.None)
            {
                return 0f;
            }

            float levelProgress =
                math.saturate((float)faith.Level / VictoryConditionEvaluationSystem.DivinRightFaithLevel);
            float intensityProgress =
                math.saturate(faith.Intensity / VictoryConditionEvaluationSystem.DivinRightIntensityThreshold);
            return (levelProgress + intensityProgress) * 0.5f;
        }

        private static float ComputeCommandHallFallProgress(EntityManager entityManager, FixedString32Bytes factionId)
        {
            using var buildingQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BuildingTypeComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DeadTag>());
            using NativeArray<BuildingTypeComponent> buildingTypes =
                buildingQuery.ToComponentDataArray<BuildingTypeComponent>(Allocator.Temp);
            using NativeArray<FactionComponent> buildingFactions =
                buildingQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < buildingTypes.Length; i++)
            {
                if (buildingTypes[i].TypeId.Equals(new FixedString64Bytes("command_hall")) &&
                    !buildingFactions[i].FactionId.Equals(factionId))
                {
                    return 1f;
                }
            }

            return 0f;
        }

        private static FixedString64Bytes GetCurrentRulingMemberId(EntityManager entityManager, Entity factionEntity)
        {
            if (!entityManager.HasBuffer<DynastyMemberRef>(factionEntity))
            {
                return default;
            }

            var roster = entityManager.GetBuffer<DynastyMemberRef>(factionEntity);
            for (int i = 0; i < roster.Length; i++)
            {
                Entity memberEntity = roster[i].Member;
                if (memberEntity == Entity.Null ||
                    !entityManager.HasComponent<DynastyMemberComponent>(memberEntity))
                {
                    continue;
                }

                var member = entityManager.GetComponentData<DynastyMemberComponent>(memberEntity);
                if (member.Status == DynastyMemberStatus.Ruling)
                {
                    return member.MemberId;
                }
            }

            return default;
        }
    }
}
