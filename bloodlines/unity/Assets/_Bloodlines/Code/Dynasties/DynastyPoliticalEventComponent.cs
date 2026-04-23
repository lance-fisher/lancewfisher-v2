using Bloodlines.Components;
using Bloodlines.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Dynasties
{
    public struct DynastyPoliticalEventComponent : IBufferElementData
    {
        public FixedString32Bytes EventType;
        public float ExpiresAtInWorldDays;
        public float ResourceTrickleFactor;
        public float AttackMultiplier;
        public float StabilizationMultiplier;
    }

    public struct DynastyPoliticalEventAggregateComponent : IComponentData
    {
        public int ActiveEventCount;
        public float ResourceTrickleFactor;
        public float AttackMultiplier;
        public float StabilizationMultiplier;
        public float LatestExpiryInWorldDays;
    }

    public static class DynastyPoliticalEventTypes
    {
        public static readonly FixedString32Bytes CovenantTestCooldown = new("CovenantTestCooldown");
        public static readonly FixedString32Bytes DivineRightFailedCooldown = new("DivineRightFailedCooldown");
        public static readonly FixedString32Bytes SuccessionShock = new("SuccessionShock");
    }

    public static class DynastyPoliticalEventUtility
    {
        public const float DefaultResourceTrickleFactor = 1f;
        public const float DefaultAttackMultiplier = 1f;
        public const float DefaultStabilizationMultiplier = 1f;
        public const float DivineRightFailedCooldownSeconds = 75f;

        public static DynastyPoliticalEventAggregateComponent BuildAggregate(
            DynamicBuffer<DynastyPoliticalEventComponent> events,
            float inWorldDays)
        {
            var aggregate = CreateDefaultAggregate();
            for (int i = 0; i < events.Length; i++)
            {
                var entry = events[i];
                if (entry.ExpiresAtInWorldDays <= inWorldDays)
                {
                    continue;
                }

                aggregate.ActiveEventCount++;
                aggregate.ResourceTrickleFactor *= entry.ResourceTrickleFactor <= 0f
                    ? DefaultResourceTrickleFactor
                    : entry.ResourceTrickleFactor;
                aggregate.AttackMultiplier *= entry.AttackMultiplier <= 0f
                    ? DefaultAttackMultiplier
                    : entry.AttackMultiplier;
                aggregate.StabilizationMultiplier *= entry.StabilizationMultiplier <= 0f
                    ? DefaultStabilizationMultiplier
                    : entry.StabilizationMultiplier;
                aggregate.LatestExpiryInWorldDays = entry.ExpiresAtInWorldDays > aggregate.LatestExpiryInWorldDays
                    ? entry.ExpiresAtInWorldDays
                    : aggregate.LatestExpiryInWorldDays;
            }

            return aggregate;
        }

        public static DynastyPoliticalEventAggregateComponent CreateDefaultAggregate()
        {
            return new DynastyPoliticalEventAggregateComponent
            {
                ActiveEventCount = 0,
                ResourceTrickleFactor = DefaultResourceTrickleFactor,
                AttackMultiplier = DefaultAttackMultiplier,
                StabilizationMultiplier = DefaultStabilizationMultiplier,
                LatestExpiryInWorldDays = 0f,
            };
        }

        public static void AddOrRefreshEvent(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes eventType,
            float expiresAtInWorldDays,
            float resourceTrickleFactor = DefaultResourceTrickleFactor,
            float attackMultiplier = DefaultAttackMultiplier,
            float stabilizationMultiplier = DefaultStabilizationMultiplier)
        {
            if (!entityManager.HasBuffer<DynastyPoliticalEventComponent>(factionEntity))
            {
                entityManager.AddBuffer<DynastyPoliticalEventComponent>(factionEntity);
            }

            var buffer = entityManager.GetBuffer<DynastyPoliticalEventComponent>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (!buffer[i].EventType.Equals(eventType))
                {
                    continue;
                }

                buffer[i] = new DynastyPoliticalEventComponent
                {
                    EventType = eventType,
                    ExpiresAtInWorldDays = expiresAtInWorldDays,
                    ResourceTrickleFactor = resourceTrickleFactor,
                    AttackMultiplier = attackMultiplier,
                    StabilizationMultiplier = stabilizationMultiplier,
                };
                return;
            }

            buffer.Add(new DynastyPoliticalEventComponent
            {
                EventType = eventType,
                ExpiresAtInWorldDays = expiresAtInWorldDays,
                ResourceTrickleFactor = resourceTrickleFactor,
                AttackMultiplier = attackMultiplier,
                StabilizationMultiplier = stabilizationMultiplier,
            });
        }

        public static float ResolveDivineRightFailedCooldownInWorldDays(DualClockComponent dualClock)
        {
            return math.max(0f, dualClock.DaysPerRealSecond * DivineRightFailedCooldownSeconds);
        }

        public static bool HasActiveEvent(
            EntityManager entityManager,
            Entity factionEntity,
            FixedString32Bytes eventType,
            float inWorldDays)
        {
            if (!entityManager.HasBuffer<DynastyPoliticalEventComponent>(factionEntity))
            {
                return false;
            }

            var buffer = entityManager.GetBuffer<DynastyPoliticalEventComponent>(factionEntity);
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].EventType.Equals(eventType) &&
                    buffer[i].ExpiresAtInWorldDays > inWorldDays)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetAggregate(
            EntityManager entityManager,
            Entity factionEntity,
            out DynastyPoliticalEventAggregateComponent aggregate)
        {
            aggregate = default;
            if (!entityManager.HasComponent<DynastyPoliticalEventAggregateComponent>(factionEntity))
            {
                return false;
            }

            aggregate = entityManager.GetComponentData<DynastyPoliticalEventAggregateComponent>(factionEntity);
            return true;
        }

        public static bool TryGetAggregateByFactionId(
            EntityManager entityManager,
            FixedString32Bytes factionId,
            out DynastyPoliticalEventAggregateComponent aggregate)
        {
            aggregate = default;
            using var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<DynastyPoliticalEventAggregateComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(factionId))
                {
                    continue;
                }

                aggregate = entityManager.GetComponentData<DynastyPoliticalEventAggregateComponent>(entities[i]);
                return true;
            }

            return false;
        }
    }
}
