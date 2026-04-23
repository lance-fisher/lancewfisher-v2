using Bloodlines.Components;
using Bloodlines.Dynasties;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Systems
{
    public static class TruebornRecognitionUtility
    {
        public const float InfluenceCost = 40f;
        public const float GoldCost = 60f;
        public const float LegitimacyCost = 5f;
        public const float StandingBonusRenown = 6f;
        public const float DefaultRenownDecayRate = 0.45f;

        private static readonly FixedString32Bytes TruebornCityFactionId =
            new("trueborn_city");

        public static bool HasActiveRise(in TruebornRiseArcComponent arc) => arc.CurrentStage >= 1;

        public static int FindRecognitionSlot(
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

        public static bool IsRecognized(
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

        public static bool TrySetRecognition(
            ref TruebornRiseArcComponent arc,
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots,
            FixedString32Bytes factionId,
            bool recognized)
        {
            int slotIndex = FindRecognitionSlot(recognitionSlots, factionId);
            if (slotIndex < 0)
            {
                if (recognitionSlots.Length >= 64)
                {
                    return false;
                }

                recognitionSlots.Add(new TruebornRiseFactionRecognitionSlotElement
                {
                    FactionId = factionId,
                });
                slotIndex = recognitionSlots.Length - 1;
            }

            ulong slotMask = 1UL << slotIndex;
            if (recognized)
            {
                arc.RecognizedFactionsBitmask |= slotMask;
            }
            else
            {
                arc.RecognizedFactionsBitmask &= ~slotMask;
            }

            return true;
        }

        public static int CountRecognized(
            DynamicBuffer<TruebornRiseFactionRecognitionSlotElement> recognitionSlots,
            ulong recognizedFactionsBitmask)
        {
            int count = 0;
            for (int i = 0; i < recognitionSlots.Length && i < 64; i++)
            {
                ulong slotMask = 1UL << i;
                if ((recognizedFactionsBitmask & slotMask) != 0UL)
                {
                    count++;
                }
            }

            return count;
        }

        public static bool HasTruebornCity(EntityManager entityManager)
        {
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<FactionKindComponent>());
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var factionKinds = query.ToComponentDataArray<FactionKindComponent>(Allocator.Temp);
            for (int i = 0; i < factions.Length; i++)
            {
                if (factionKinds[i].Kind == FactionKind.Neutral &&
                    factions[i].FactionId.Equals(TruebornCityFactionId))
                {
                    return true;
                }
            }

            return false;
        }

        public static void ApplyStandingBonus(
            EntityManager entityManager,
            Entity factionEntity,
            float currentInWorldDays)
        {
            if (entityManager.HasComponent<DynastyRenownComponent>(factionEntity))
            {
                var renown = entityManager.GetComponentData<DynastyRenownComponent>(factionEntity);
                renown.RenownScore += StandingBonusRenown;
                renown.PeakRenown = math.max(renown.PeakRenown, renown.RenownScore);
                renown.LastRenownUpdateInWorldDays = currentInWorldDays;
                renown.Initialized = 1;
                entityManager.SetComponentData(factionEntity, renown);
                return;
            }

            entityManager.AddComponentData(factionEntity, new DynastyRenownComponent
            {
                RenownScore = StandingBonusRenown,
                LastRenownUpdateInWorldDays = currentInWorldDays,
                RenownDecayRate = DefaultRenownDecayRate,
                PeakRenown = StandingBonusRenown,
                LastRulingMemberId = default,
                Initialized = 1,
            });
        }

        public static void ClearRecognitionCooldowns(
            EntityManager entityManager,
            Entity factionEntity,
            float currentInWorldDays)
        {
            if (!entityManager.HasBuffer<DynastyPoliticalEventComponent>(factionEntity))
            {
                return;
            }

            var buffer = entityManager.GetBuffer<DynastyPoliticalEventComponent>(factionEntity);
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                FixedString32Bytes eventType = buffer[i].EventType;
                if (eventType.Equals(DynastyPoliticalEventTypes.CovenantTestCooldown) ||
                    eventType.Equals(DynastyPoliticalEventTypes.DivineRightFailedCooldown))
                {
                    buffer.RemoveAt(i);
                }
            }

            var aggregate = DynastyPoliticalEventUtility.BuildAggregate(buffer, currentInWorldDays);
            if (entityManager.HasComponent<DynastyPoliticalEventAggregateComponent>(factionEntity))
            {
                entityManager.SetComponentData(factionEntity, aggregate);
            }
            else
            {
                entityManager.AddComponentData(factionEntity, aggregate);
            }
        }
    }
}
