using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        public bool TryDebugIssueAttackOrderOnNearestHostile(string factionId, out bool issued)
        {
            issued = false;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            if (!TryFindNearestHostileForSelection(entityManager, factionId, out var targetEntity))
            {
                return false;
            }

            issued = IssueExplicitAttackOrder(entityManager, targetEntity) > 0;
            return issued;
        }

        public bool TryDebugIssueAttackMove(float3 destination, out int orderedCount)
        {
            return TryDebugIssueGroupMoveOrder(destination, attackMove: true, out orderedCount);
        }

        private bool TryFindNearestHostileForSelection(
            EntityManager entityManager,
            string factionId,
            out Entity targetEntity)
        {
            targetEntity = Entity.Null;
            var sourceFactionId = new FixedString32Bytes(factionId ?? string.Empty);

            var selectedCombatUnits = new List<Entity>();
            for (int i = 0; i < selectedEntities.Count; i++)
            {
                var entity = selectedEntities[i];
                if (!CanIssueCombatOrder(entityManager, entity))
                {
                    continue;
                }

                if (!entityManager.GetComponentData<FactionComponent>(entity).FactionId.Equals(sourceFactionId))
                {
                    continue;
                }

                selectedCombatUnits.Add(entity);
            }

            if (selectedCombatUnits.Count == 0)
            {
                return false;
            }

            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            float bestDistanceSq = float.MaxValue;

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f ||
                    factions[i].FactionId.Equals(sourceFactionId) ||
                    !IsFactionHostile(entityManager, sourceFactionId, factions[i].FactionId))
                {
                    continue;
                }

                for (int j = 0; j < selectedCombatUnits.Count; j++)
                {
                    float distanceSq = math.distancesq(
                        entityManager.GetComponentData<PositionComponent>(selectedCombatUnits[j]).Value,
                        positions[i].Value);

                    if (distanceSq >= bestDistanceSq)
                    {
                        continue;
                    }

                    bestDistanceSq = distanceSq;
                    targetEntity = entities[i];
                }
            }

            return targetEntity != Entity.Null;
        }
    }
}
