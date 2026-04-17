using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityCamera = UnityEngine.Camera;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        private readonly List<Entity> attackMoveSelectionSnapshot = new();
        private bool attackMoveCursorMode;

        private void LateUpdate()
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return;
            }

            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null)
            {
                return;
            }

            if (controlledCamera == null)
            {
                controlledCamera = UnityCamera.main;
                if (controlledCamera == null)
                {
                    return;
                }
            }

            if (HasPendingConstructionPlacement())
            {
                if (attackMoveCursorMode)
                {
                    CancelAttackMoveMode(entityManager, restoreSelection: false);
                }

                return;
            }

            if (attackMoveCursorMode && keyboard.escapeKey.wasPressedThisFrame)
            {
                CancelAttackMoveMode(entityManager, restoreSelection: true);
                return;
            }

            if (attackMoveCursorMode && GetSelectedCombatUnitCount(entityManager) <= 0)
            {
                CancelAttackMoveMode(entityManager, restoreSelection: false);
                return;
            }

            if (keyboard.aKey.wasPressedThisFrame && GetSelectedCombatUnitCount(entityManager) > 0)
            {
                BeginAttackMoveMode();
            }

            if (!mouse.rightButton.wasPressedThisFrame ||
                !TryGetGroundPoint(mouse.position.ReadValue(), out var destination))
            {
                return;
            }

            if (attackMoveCursorMode)
            {
                if (IssueAttackMoveOrder(entityManager, destination) > 0)
                {
                    CancelAttackMoveMode(entityManager, restoreSelection: false);
                }

                return;
            }

            if (GetSelectedCombatUnitCount(entityManager) <= 0)
            {
                return;
            }

            if (TryFindNearestHostileUnit(entityManager, destination, out var targetEntity))
            {
                IssueExplicitAttackOrder(entityManager, targetEntity);
            }
        }

        public bool TryDebugIssueAttackOrder(Entity targetEntity)
        {
            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            return IssueExplicitAttackOrder(entityManager, targetEntity) > 0;
        }

        public bool TryDebugIssueAttackMove(float3 destination)
        {
            return TryDebugIssueGroupMoveOrder(destination, attackMove: true, out _);
        }

        private void BeginAttackMoveMode()
        {
            attackMoveCursorMode = true;
            attackMoveSelectionSnapshot.Clear();
            attackMoveSelectionSnapshot.AddRange(selectedEntities);
        }

        private void CancelAttackMoveMode(EntityManager entityManager, bool restoreSelection)
        {
            attackMoveCursorMode = false;

            if (restoreSelection)
            {
                ClearSelection(entityManager);
                for (int i = 0; i < attackMoveSelectionSnapshot.Count; i++)
                {
                    var entity = attackMoveSelectionSnapshot[i];
                    if (!entityManager.Exists(entity) || !IsSelectableAliveUnit(entityManager, entity))
                    {
                        continue;
                    }

                    AddSelection(entityManager, entity);
                }
            }

            attackMoveSelectionSnapshot.Clear();
        }

        private int IssueExplicitAttackOrder(EntityManager entityManager, Entity targetEntity)
        {
            if (!TryResolveHostileTarget(entityManager, targetEntity))
            {
                return 0;
            }

            int issued = 0;

            for (int i = 0; i < selectedEntities.Count; i++)
            {
                var entity = selectedEntities[i];
                if (!CanIssueCombatOrder(entityManager, entity))
                {
                    continue;
                }

                if (entityManager.HasComponent<MoveCommandComponent>(entity))
                {
                    var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(entity);
                    moveCommand.IsActive = false;
                    entityManager.SetComponentData(entity, moveCommand);
                }

                if (entityManager.HasComponent<GroupMovementOrderComponent>(entity))
                {
                    entityManager.RemoveComponent<GroupMovementOrderComponent>(entity);
                }

                if (entityManager.HasComponent<AttackTargetComponent>(entity))
                {
                    entityManager.RemoveComponent<AttackTargetComponent>(entity);
                }

                var attackOrder = new AttackOrderComponent
                {
                    ExplicitTargetEntity = targetEntity,
                    IsAttackMoveDestination = false,
                    AttackMoveDestination = float3.zero,
                    IsActive = true,
                };

                if (entityManager.HasComponent<AttackOrderComponent>(entity))
                {
                    entityManager.SetComponentData(entity, attackOrder);
                }
                else
                {
                    entityManager.AddComponentData(entity, attackOrder);
                }

                issued++;
            }

            return issued;
        }

        private int IssueAttackMoveOrder(EntityManager entityManager, float3 destination)
        {
            return IssueGroupMoveOrder(entityManager, destination, attackMove: true);
        }

        private int GetSelectedCombatUnitCount(EntityManager entityManager)
        {
            int count = 0;

            for (int i = 0; i < selectedEntities.Count; i++)
            {
                if (CanIssueCombatOrder(entityManager, selectedEntities[i]))
                {
                    count++;
                }
            }

            return count;
        }

        private bool CanIssueCombatOrder(EntityManager entityManager, Entity entity)
        {
            if (!entityManager.Exists(entity) ||
                entityManager.HasComponent<DeadTag>(entity) ||
                !entityManager.HasComponent<UnitTypeComponent>(entity) ||
                !entityManager.HasComponent<HealthComponent>(entity) ||
                !entityManager.HasComponent<CombatStatsComponent>(entity) ||
                !entityManager.HasComponent<FactionComponent>(entity))
            {
                return false;
            }

            if (!entityManager.GetComponentData<FactionComponent>(entity).FactionId.Equals(controlledFactionId))
            {
                return false;
            }

            if (entityManager.GetComponentData<HealthComponent>(entity).Current <= 0f)
            {
                return false;
            }

            var combat = entityManager.GetComponentData<CombatStatsComponent>(entity);
            return combat.AttackDamage > 0f && combat.AttackRange > 0f && combat.Sight > 0f;
        }

        private bool TryFindNearestHostileUnit(EntityManager entityManager, float3 selectionPoint, out Entity targetEntity)
        {
            targetEntity = Entity.Null;

            var unitQuery = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<UnitTypeComponent>(),
                ComponentType.ReadOnly<PositionComponent>(),
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HealthComponent>());

            using var entities = unitQuery.ToEntityArray(Allocator.Temp);
            using var positions = unitQuery.ToComponentDataArray<PositionComponent>(Allocator.Temp);
            using var factions = unitQuery.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var health = unitQuery.ToComponentDataArray<HealthComponent>(Allocator.Temp);

            int bestIndex = -1;
            float bestDistanceSq = singleSelectRadius * singleSelectRadius;

            for (int i = 0; i < entities.Length; i++)
            {
                if (health[i].Current <= 0f ||
                    factions[i].FactionId.Equals(controlledFactionId) ||
                    !IsFactionHostile(entityManager, controlledFactionId, factions[i].FactionId))
                {
                    continue;
                }

                float distanceSq = math.distancesq(positions[i].Value, selectionPoint);
                if (distanceSq > bestDistanceSq)
                {
                    continue;
                }

                bestDistanceSq = distanceSq;
                bestIndex = i;
            }

            if (bestIndex < 0)
            {
                return false;
            }

            targetEntity = entities[bestIndex];
            return true;
        }

        private bool TryResolveHostileTarget(EntityManager entityManager, Entity targetEntity)
        {
            if (targetEntity == Entity.Null ||
                !entityManager.Exists(targetEntity) ||
                entityManager.HasComponent<DeadTag>(targetEntity) ||
                !entityManager.HasComponent<UnitTypeComponent>(targetEntity) ||
                !entityManager.HasComponent<HealthComponent>(targetEntity) ||
                !entityManager.HasComponent<FactionComponent>(targetEntity))
            {
                return false;
            }

            var health = entityManager.GetComponentData<HealthComponent>(targetEntity);
            var faction = entityManager.GetComponentData<FactionComponent>(targetEntity);

            return health.Current > 0f &&
                !faction.FactionId.Equals(controlledFactionId) &&
                IsFactionHostile(entityManager, controlledFactionId, faction.FactionId);
        }

        private static bool IsFactionHostile(
            EntityManager entityManager,
            FixedString32Bytes sourceFactionId,
            FixedString32Bytes targetFactionId)
        {
            if (sourceFactionId.Equals(targetFactionId))
            {
                return false;
            }

            var query = entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<FactionComponent>(),
                ComponentType.ReadOnly<HostilityComponent>());

            using var factionEntities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < factionEntities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(sourceFactionId))
                {
                    continue;
                }

                var hostilityBuffer = entityManager.GetBuffer<HostilityComponent>(factionEntities[i]);
                for (int j = 0; j < hostilityBuffer.Length; j++)
                {
                    if (hostilityBuffer[j].HostileFactionId.Equals(targetFactionId))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }
    }
}
