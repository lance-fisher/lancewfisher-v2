using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Browser reference: simulation.js tickTruebornRiseArc / getTruebornChallengeLevel.
    ///
    /// Unity sub-slice 1 keeps the rise arc on a singleton and ports the stage timing
    /// plus base loyalty / legitimacy pressure. Two intentional follow-up seams remain:
    /// - browser trade-relationship standing is not yet available on the Unity master
    ///   line, so stage-0 challenge only counts kingdom territory size and hostility
    /// - recognized kingdoms are fully exempt here, matching the current directive's
    ///   "skip recognized factions" wording; the browser's reduced-pressure nuance can
    ///   widen in the later recognition slice if Lance keeps it
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GovernanceCoalitionPressureSystem))]
    [UpdateBefore(typeof(WorldPressureEscalationSystem))]
    public partial struct TruebornRiseArcSystem : ISystem
    {
        private const float TruebornRiseUnchallengedThresholdDays = 365f * 8f;
        private const float TruebornRiseStage2DelayDays = 365f * 2f;
        private const float TruebornRiseStage3DelayDays = 365f * 3f;
        private const float TruebornRiseStage1LoyaltyPressure = 0.8f;
        private const float TruebornRiseStage2LoyaltyPressure = 1.8f;
        private const float TruebornRiseStage3LoyaltyPressure = 3.2f;
        private const float TruebornRiseStage2LegitimacyPressure = 0.6f;
        private const float TruebornRiseStage3LegitimacyPressure = 1.4f;
        private const int TruebornRiseChallengeThreshold = 5;
        private static readonly FixedString32Bytes TruebornCityFactionId =
            new FixedString32Bytes("trueborn_city");

        private int _lastProcessedDay;
        private byte _initialized;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<TruebornRiseArcComponent>())
            {
                Entity singleton = state.EntityManager.CreateEntity(typeof(TruebornRiseArcComponent));
                state.EntityManager.SetName(singleton, "TruebornRiseArc");
                state.EntityManager.SetComponentData(singleton, default(TruebornRiseArcComponent));
                state.EntityManager.AddBuffer<TruebornRiseFactionRecognitionSlotElement>(singleton);
            }

            state.RequireForUpdate<TruebornRiseArcComponent>();
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<FactionKindComponent>();
            state.RequireForUpdate<ControlPointComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int currentDay = (int)math.floor(SystemAPI.GetSingleton<DualClockComponent>().InWorldDays);
            if (_initialized != 0 && currentDay == _lastProcessedDay)
            {
                return;
            }

            _initialized = 1;
            _lastProcessedDay = currentDay;

            var entityManager = state.EntityManager;
            float currentInWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;
            Entity arcEntity = SystemAPI.GetSingletonEntity<TruebornRiseArcComponent>();
            TruebornRiseArcComponent arc = entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);

            using var factionQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>());
            using NativeArray<Entity> factionEntities = factionQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<FactionComponent> factions =
                factionQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using NativeArray<FactionKindComponent> factionKinds =
                factionQuery.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);

            using var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<Entity> controlPointEntities =
                controlPointQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<ControlPointComponent> controlPoints =
                controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            bool hasTruebornCity = false;
            var territoryCounts = new NativeHashMap<FixedString32Bytes, int>(math.max(8, controlPoints.Length), Allocator.Temp);
            try
            {
                for (int i = 0; i < factions.Length; i++)
                {
                    if (factions[i].FactionId.Equals(TruebornCityFactionId))
                    {
                        hasTruebornCity = true;
                        break;
                    }
                }

                for (int i = 0; i < controlPoints.Length; i++)
                {
                    FixedString32Bytes ownerFactionId = controlPoints[i].OwnerFactionId;
                    if (ownerFactionId.Length == 0)
                    {
                        continue;
                    }

                    if (!territoryCounts.TryGetValue(ownerFactionId, out int count))
                    {
                        count = 0;
                    }

                    territoryCounts[ownerFactionId] = count + 1;
                }

                DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots =
                    entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);
                EnsureRecognitionSlots(recognitionSlots, factions, factionKinds);

                if (!hasTruebornCity)
                {
                    ResetArc(ref arc);
                    entityManager.SetComponentData(arcEntity, arc);
                    return;
                }

                arc.ChallengeLevel = ComputeChallengeLevel(
                    entityManager,
                    factionEntities,
                    factions,
                    factionKinds,
                    territoryCounts);

                ApplyStageProfile(ref arc);

                if (arc.CurrentStage == 0)
                {
                    if (arc.ChallengeLevel < TruebornRiseChallengeThreshold)
                    {
                        arc.UnchallengedCycles += 1;
                    }
                    else
                    {
                        arc.UnchallengedCycles = math.max(0, arc.UnchallengedCycles - 2);
                    }

                    if (currentInWorldDays >= TruebornRiseUnchallengedThresholdDays &&
                        arc.UnchallengedCycles >= 3)
                    {
                        arc.CurrentStage = 1;
                        arc.StageStartedAtInWorldDays = currentInWorldDays;
                        ApplyStageProfile(ref arc);
                    }

                    entityManager.SetComponentData(arcEntity, arc);
                    return;
                }

                ApplyPressure(
                    entityManager,
                    factionEntities,
                    factions,
                    factionKinds,
                    controlPointEntities,
                    controlPoints,
                    recognitionSlots,
                    arc);

                if (arc.CurrentStage == 1 &&
                    currentInWorldDays >= arc.StageStartedAtInWorldDays + TruebornRiseStage2DelayDays)
                {
                    arc.CurrentStage = 2;
                    arc.StageStartedAtInWorldDays = currentInWorldDays;
                    ApplyStageProfile(ref arc);
                }
                else if (arc.CurrentStage == 2 &&
                    currentInWorldDays >= arc.StageStartedAtInWorldDays + TruebornRiseStage3DelayDays)
                {
                    arc.CurrentStage = 3;
                    arc.StageStartedAtInWorldDays = currentInWorldDays;
                    ApplyStageProfile(ref arc);
                }

                entityManager.SetComponentData(arcEntity, arc);
            }
            finally
            {
                if (territoryCounts.IsCreated)
                {
                    territoryCounts.Dispose();
                }
            }
        }

        private static void ApplyStageProfile(ref TruebornRiseArcComponent arc)
        {
            switch (arc.CurrentStage)
            {
                case 1:
                    arc.GlobalPressurePerDay = 0f;
                    arc.LoyaltyErosionPerDay = TruebornRiseStage1LoyaltyPressure;
                    break;
                case 2:
                    arc.GlobalPressurePerDay = TruebornRiseStage2LegitimacyPressure;
                    arc.LoyaltyErosionPerDay = TruebornRiseStage2LoyaltyPressure;
                    break;
                case 3:
                default:
                    if (arc.CurrentStage >= 3)
                    {
                        arc.GlobalPressurePerDay = TruebornRiseStage3LegitimacyPressure;
                        arc.LoyaltyErosionPerDay = TruebornRiseStage3LoyaltyPressure;
                        break;
                    }

                    arc.GlobalPressurePerDay = 0f;
                    arc.LoyaltyErosionPerDay = 0f;
                    break;
            }
        }

        private static void ResetArc(ref TruebornRiseArcComponent arc)
        {
            arc.CurrentStage = 0;
            arc.StageStartedAtInWorldDays = 0f;
            arc.GlobalPressurePerDay = 0f;
            arc.LoyaltyErosionPerDay = 0f;
            arc.RecognizedFactionsBitmask = 0UL;
            arc.ChallengeLevel = 0;
            arc.UnchallengedCycles = 0;
        }

        private static void EnsureRecognitionSlots(
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots,
            NativeArray<FactionComponent> factions,
            NativeArray<FactionKindComponent> factionKinds)
        {
            for (int i = 0; i < factions.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom)
                {
                    continue;
                }

                FixedString32Bytes factionId = factions[i].FactionId;
                if (FindRecognitionSlot(recognitionSlots, factionId) >= 0)
                {
                    continue;
                }

                if (recognitionSlots.Length >= 64)
                {
                    break;
                }

                recognitionSlots.Add(new TruebornRiseFactionRecognitionSlotElement
                {
                    FactionId = factionId,
                });
            }
        }

        private static int ComputeChallengeLevel(
            EntityManager entityManager,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factions,
            NativeArray<FactionKindComponent> factionKinds,
            NativeHashMap<FixedString32Bytes, int> territoryCounts)
        {
            int challengeLevel = 0;

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (factionKinds[i].Kind != FactionKind.Kingdom)
                {
                    continue;
                }

                FixedString32Bytes factionId = factions[i].FactionId;
                if (territoryCounts.TryGetValue(factionId, out int territoryCount) &&
                    territoryCount >= 3)
                {
                    challengeLevel += 1;
                }

                if (!entityManager.HasBuffer<HostilityComponent>(factionEntities[i]))
                {
                    continue;
                }

                DynamicBuffer<HostilityComponent> hostilities =
                    entityManager.GetBuffer<HostilityComponent>(factionEntities[i]);
                for (int j = 0; j < hostilities.Length; j++)
                {
                    if (hostilities[j].HostileFactionId.Equals(TruebornCityFactionId))
                    {
                        challengeLevel += 3;
                        break;
                    }
                }
            }

            return challengeLevel;
        }

        private static void ApplyPressure(
            EntityManager entityManager,
            NativeArray<Entity> factionEntities,
            NativeArray<FactionComponent> factions,
            NativeArray<FactionKindComponent> factionKinds,
            NativeArray<Entity> controlPointEntities,
            NativeArray<ControlPointComponent> controlPoints,
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots,
            TruebornRiseArcComponent arc)
        {
            if (arc.CurrentStage == 0 ||
                arc.LoyaltyErosionPerDay <= 0f && arc.GlobalPressurePerDay <= 0f)
            {
                return;
            }

            var kingdomLookup = new NativeHashMap<FixedString32Bytes, Entity>(factionEntities.Length, Allocator.Temp);
            try
            {
                for (int i = 0; i < factionEntities.Length; i++)
                {
                    if (factionKinds[i].Kind == FactionKind.Kingdom)
                    {
                        kingdomLookup.TryAdd(factions[i].FactionId, factionEntities[i]);
                    }
                }

                for (int i = 0; i < controlPoints.Length; i++)
                {
                    ControlPointComponent controlPoint = controlPoints[i];
                    if (controlPoint.OwnerFactionId.Length == 0 ||
                        !kingdomLookup.ContainsKey(controlPoint.OwnerFactionId) ||
                        IsRecognized(recognitionSlots, arc.RecognizedFactionsBitmask, controlPoint.OwnerFactionId))
                    {
                        continue;
                    }

                    controlPoint.Loyalty = math.clamp(
                        controlPoint.Loyalty - arc.LoyaltyErosionPerDay,
                        0f,
                        100f);
                    entityManager.SetComponentData(controlPointEntities[i], controlPoint);
                }

                if (arc.GlobalPressurePerDay <= 0f)
                {
                    return;
                }

                for (int i = 0; i < factionEntities.Length; i++)
                {
                    if (factionKinds[i].Kind != FactionKind.Kingdom ||
                        IsRecognized(recognitionSlots, arc.RecognizedFactionsBitmask, factions[i].FactionId) ||
                        !entityManager.HasComponent<DynastyStateComponent>(factionEntities[i]))
                    {
                        continue;
                    }

                    DynastyStateComponent dynastyState =
                        entityManager.GetComponentData<DynastyStateComponent>(factionEntities[i]);
                    dynastyState.Legitimacy = math.clamp(
                        dynastyState.Legitimacy - arc.GlobalPressurePerDay,
                        0f,
                        100f);
                    entityManager.SetComponentData(factionEntities[i], dynastyState);
                }
            }
            finally
            {
                if (kingdomLookup.IsCreated)
                {
                    kingdomLookup.Dispose();
                }
            }
        }

        private static bool IsRecognized(
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots,
            ulong recognizedFactionsBitmask,
            FixedString32Bytes factionId)
        {
            int slotIndex = FindRecognitionSlot(recognitionSlots, factionId);
            if (slotIndex < 0 || slotIndex >= 64)
            {
                return false;
            }

            ulong slotMask = 1UL << slotIndex;
            return (recognizedFactionsBitmask & slotMask) != 0UL;
        }

        private static int FindRecognitionSlot(
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots,
            FixedString32Bytes factionId)
        {
            for (int i = 0; i < recognitionSlots.Length; i++)
            {
                if (recognitionSlots[i].FactionId.Equals(factionId))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
