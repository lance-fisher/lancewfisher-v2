using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    /// <summary>
    /// Canon lift on top of the browser Trueborn rise / recognition seam:
    /// once the match reaches Stage 4 or 5 and a dominant kingdom is clear,
    /// the Trueborn issue a recognition ultimatum to that hegemon. Refusing
    /// the ultimatum keeps the normal rise pressure intact and, after the
    /// deadline, adds focused pressure to the target's weakest march plus a
    /// direct legitimacy strain on the ruling dynasty.
    ///
    /// Browser references:
    /// - simulation.js tickTruebornRiseArc / getTruebornRecognitionTerms
    ///
    /// Canon references:
    /// - 11_MATCHFLOW/MATCH_STRUCTURE.md (Stage 4/5 late-game Trueborn pressure)
    /// - 08_MECHANICS/DIPLOMACY_SYSTEM.md (ultimatum structure)
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MatchProgressionEvaluationSystem))]
    [UpdateAfter(typeof(TruebornRiseArcSystem))]
    public partial struct TruebornDiplomaticEscalationSystem : ISystem
    {
        private const float Stage4UltimatumDeadlineDays = 120f;
        private const float Stage5UltimatumDeadlineDays = 60f;
        private const float Stage4UltimatumLoyaltyPressure = 1.4f;
        private const float Stage4UltimatumLegitimacyPressure = 0.7f;
        private const float Stage5UltimatumLoyaltyPressure = 2.4f;
        private const float Stage5UltimatumLegitimacyPressure = 1.2f;

        private int _lastPressureDay;
        private byte _pressureInitialized;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TruebornRiseArcComponent>();
            state.RequireForUpdate<DualClockComponent>();
            state.RequireForUpdate<MatchProgressionComponent>();
            state.RequireForUpdate<FactionComponent>();
            state.RequireForUpdate<FactionKindComponent>();
            state.RequireForUpdate<ControlPointComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityManager entityManager = state.EntityManager;
            Entity arcEntity = SystemAPI.GetSingletonEntity<TruebornRiseArcComponent>();
            TruebornRiseArcComponent arc = entityManager.GetComponentData<TruebornRiseArcComponent>(arcEntity);
            float currentInWorldDays = SystemAPI.GetSingleton<DualClockComponent>().InWorldDays;
            int currentDay = (int)math.floor(currentInWorldDays);
            MatchProgressionComponent matchProgression = SystemAPI.GetSingleton<MatchProgressionComponent>();

            if (!TruebornRecognitionUtility.HasActiveRise(arc) ||
                !TruebornRecognitionUtility.HasTruebornCity(entityManager) ||
                matchProgression.StageNumber < 4)
            {
                if (TruebornRecognitionUtility.ClearUltimatum(ref arc))
                {
                    entityManager.SetComponentData(arcEntity, arc);
                }

                return;
            }

            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots =
                entityManager.GetBuffer<TruebornRiseFactionRecognitionSlotElement>(arcEntity);
            FixedString32Bytes targetFactionId = ResolveUltimatumTarget(in matchProgression);
            if (targetFactionId.Length == 0 ||
                TruebornRecognitionUtility.IsRecognized(
                    recognitionSlots,
                    arc.RecognizedFactionsBitmask,
                    targetFactionId) ||
                !TruebornRecognitionUtility.TryFindKingdomFactionEntity(
                    entityManager,
                    targetFactionId,
                    out Entity targetFactionEntity))
            {
                if (TruebornRecognitionUtility.ClearUltimatum(ref arc))
                {
                    entityManager.SetComponentData(arcEntity, arc);
                }

                return;
            }

            GetUltimatumProfile(
                matchProgression.StageNumber,
                out float deadlineDays,
                out float loyaltyPressurePerDay,
                out float legitimacyPressurePerDay);

            bool arcChanged = false;
            if (!arc.UltimatumTargetFactionId.Equals(targetFactionId))
            {
                arc.UltimatumTargetFactionId = targetFactionId;
                arc.UltimatumIssuedAtInWorldDays = currentInWorldDays;
                arc.UltimatumDeadlineInWorldDays = currentInWorldDays + deadlineDays;
                arc.UltimatumLoyaltyPressurePerDay = loyaltyPressurePerDay;
                arc.UltimatumLegitimacyPressurePerDay = legitimacyPressurePerDay;
                arc.UltimatumStageNumber = (byte)matchProgression.StageNumber;
                arcChanged = true;
            }
            else if (arc.UltimatumStageNumber != matchProgression.StageNumber ||
                math.abs(arc.UltimatumLoyaltyPressurePerDay - loyaltyPressurePerDay) > 0.001f ||
                math.abs(arc.UltimatumLegitimacyPressurePerDay - legitimacyPressurePerDay) > 0.001f)
            {
                arc.UltimatumStageNumber = (byte)matchProgression.StageNumber;
                arc.UltimatumDeadlineInWorldDays = math.min(
                    arc.UltimatumDeadlineInWorldDays,
                    currentInWorldDays + deadlineDays);
                arc.UltimatumLoyaltyPressurePerDay = loyaltyPressurePerDay;
                arc.UltimatumLegitimacyPressurePerDay = legitimacyPressurePerDay;
                arcChanged = true;
            }

            bool newPressureDay = _pressureInitialized == 0 || currentDay != _lastPressureDay;
            if (newPressureDay)
            {
                _pressureInitialized = 1;
                _lastPressureDay = currentDay;

                if (currentInWorldDays >= arc.UltimatumDeadlineInWorldDays)
                {
                    ApplyExpiredUltimatumPressure(
                        entityManager,
                        targetFactionEntity,
                        targetFactionId,
                        arc.UltimatumLoyaltyPressurePerDay,
                        arc.UltimatumLegitimacyPressurePerDay);
                }
            }

            if (arcChanged)
            {
                entityManager.SetComponentData(arcEntity, arc);
            }
        }

        private static FixedString32Bytes ResolveUltimatumTarget(
            in MatchProgressionComponent matchProgression)
        {
            if (matchProgression.GreatReckoningActive &&
                matchProgression.GreatReckoningTargetFactionId.Length > 0)
            {
                return matchProgression.GreatReckoningTargetFactionId;
            }

            return matchProgression.DominantKingdomId;
        }

        private static void GetUltimatumProfile(
            int stageNumber,
            out float deadlineDays,
            out float loyaltyPressurePerDay,
            out float legitimacyPressurePerDay)
        {
            if (stageNumber >= 5)
            {
                deadlineDays = Stage5UltimatumDeadlineDays;
                loyaltyPressurePerDay = Stage5UltimatumLoyaltyPressure;
                legitimacyPressurePerDay = Stage5UltimatumLegitimacyPressure;
                return;
            }

            deadlineDays = Stage4UltimatumDeadlineDays;
            loyaltyPressurePerDay = Stage4UltimatumLoyaltyPressure;
            legitimacyPressurePerDay = Stage4UltimatumLegitimacyPressure;
        }

        private static void ApplyExpiredUltimatumPressure(
            EntityManager entityManager,
            Entity targetFactionEntity,
            FixedString32Bytes targetFactionId,
            float loyaltyPressurePerDay,
            float legitimacyPressurePerDay)
        {
            using var controlPointQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<ControlPointComponent>());
            using NativeArray<Entity> controlPointEntities =
                controlPointQuery.ToEntityArray(Allocator.Temp);
            using NativeArray<ControlPointComponent> controlPoints =
                controlPointQuery.ToComponentDataArray<ControlPointComponent>(Allocator.Temp);

            int weakestControlPointIndex = -1;
            float weakestLoyalty = float.MaxValue;
            for (int i = 0; i < controlPoints.Length; i++)
            {
                if (!controlPoints[i].OwnerFactionId.Equals(targetFactionId))
                {
                    continue;
                }

                if (controlPoints[i].Loyalty < weakestLoyalty)
                {
                    weakestLoyalty = controlPoints[i].Loyalty;
                    weakestControlPointIndex = i;
                }
            }

            if (weakestControlPointIndex >= 0)
            {
                ControlPointComponent weakestControlPoint = controlPoints[weakestControlPointIndex];
                weakestControlPoint.Loyalty = math.clamp(
                    weakestControlPoint.Loyalty - loyaltyPressurePerDay,
                    0f,
                    100f);
                entityManager.SetComponentData(
                    controlPointEntities[weakestControlPointIndex],
                    weakestControlPoint);
            }

            if (entityManager.HasComponent<DynastyStateComponent>(targetFactionEntity))
            {
                DynastyStateComponent dynastyState =
                    entityManager.GetComponentData<DynastyStateComponent>(targetFactionEntity);
                dynastyState.Legitimacy = math.clamp(
                    dynastyState.Legitimacy - legitimacyPressurePerDay,
                    0f,
                    100f);
                entityManager.SetComponentData(targetFactionEntity, dynastyState);
            }
        }
    }
}
