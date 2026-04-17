using System;
using System.Collections.Generic;
using Bloodlines.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Bloodlines.Debug
{
    public sealed partial class BloodlinesDebugCommandSurface
    {
        [SerializeField] private float maxGroupFormationRadius = 6f;

        public bool TryDebugIssueGroupMoveOrder(float3 destination, bool attackMove, out int memberCount)
        {
            memberCount = 0;

            if (!TryGetEntityManager(out var entityManager))
            {
                return false;
            }

            memberCount = IssueGroupMoveOrder(entityManager, destination, attackMove);
            return memberCount > 0;
        }

        public bool TryDebugInspectGroupMovement(
            Entity entity,
            out FixedString32Bytes groupId,
            out float3 localOffset,
            out float3 destinationCenter)
        {
            groupId = default;
            localOffset = float3.zero;
            destinationCenter = float3.zero;

            if (!TryGetEntityManager(out var entityManager) ||
                !entityManager.Exists(entity) ||
                !entityManager.HasComponent<GroupMovementOrderComponent>(entity))
            {
                return false;
            }

            var groupMovement = entityManager.GetComponentData<GroupMovementOrderComponent>(entity);
            groupId = groupMovement.GroupId;
            localOffset = groupMovement.LocalOffset;
            destinationCenter = groupMovement.DestinationCenter;
            return true;
        }

        private int IssueGroupMoveOrder(EntityManager entityManager, float3 destination, bool attackMove)
        {
            var commandUnits = BuildSelectedCombatCommandUnits(entityManager);
            if (commandUnits.Count == 0)
            {
                return 0;
            }

            float3 centroid = float3.zero;
            for (int i = 0; i < commandUnits.Count; i++)
            {
                centroid += commandUnits[i].Position;
            }

            centroid /= commandUnits.Count;

            float maximumRadius = 0f;
            for (int i = 0; i < commandUnits.Count; i++)
            {
                float radius = math.distance(commandUnits[i].Position, centroid);
                if (radius > maximumRadius)
                {
                    maximumRadius = radius;
                }
            }

            float maximumAllowedRadius = math.max(0.5f, maxGroupFormationRadius);
            float offsetScale = maximumRadius > maximumAllowedRadius && maximumRadius > 0.001f
                ? maximumAllowedRadius / maximumRadius
                : 1f;

            string rawGroupId = Guid.NewGuid().ToString("N");
            var groupId = new FixedString32Bytes("gm_" + rawGroupId.Substring(0, 12));
            double orderTimestamp = Time.realtimeSinceStartupAsDouble;
            float stoppingDistance = math.max(0.1f, commandStoppingDistance);

            for (int i = 0; i < commandUnits.Count; i++)
            {
                var command = commandUnits[i];
                float3 localOffset = (command.Position - centroid) * offsetScale;
                float3 memberDestination = destination + localOffset;

                var groupMovementOrder = new GroupMovementOrderComponent
                {
                    GroupId = groupId,
                    GroupCenter = centroid,
                    LocalOffset = localOffset,
                    DestinationCenter = destination,
                    OrderTimestamp = orderTimestamp,
                    IsAttackMove = attackMove,
                };

                if (entityManager.HasComponent<GroupMovementOrderComponent>(command.Entity))
                {
                    entityManager.SetComponentData(command.Entity, groupMovementOrder);
                }
                else
                {
                    entityManager.AddComponentData(command.Entity, groupMovementOrder);
                }

                if (entityManager.HasComponent<AttackTargetComponent>(command.Entity))
                {
                    entityManager.RemoveComponent<AttackTargetComponent>(command.Entity);
                }

                var moveCommand = new MoveCommandComponent
                {
                    Destination = memberDestination,
                    StoppingDistance = stoppingDistance,
                    IsActive = true,
                };

                if (entityManager.HasComponent<MoveCommandComponent>(command.Entity))
                {
                    entityManager.SetComponentData(command.Entity, moveCommand);
                }
                else
                {
                    entityManager.AddComponentData(command.Entity, moveCommand);
                }

                if (attackMove)
                {
                    if (entityManager.HasComponent<CombatStanceComponent>(command.Entity))
                    {
                        var stance = entityManager.GetComponentData<CombatStanceComponent>(command.Entity);
                        stance.Stance = CombatStance.AttackMove;
                        if (stance.LowHealthRetreatThreshold <= 0f)
                        {
                            stance.LowHealthRetreatThreshold = CombatUnitRuntimeDefaults.DefaultLowHealthRetreatThreshold;
                        }

                        entityManager.SetComponentData(command.Entity, stance);
                    }
                    else
                    {
                        entityManager.AddComponentData(command.Entity, new CombatStanceComponent
                        {
                            Stance = CombatStance.AttackMove,
                            LowHealthRetreatThreshold = CombatUnitRuntimeDefaults.DefaultLowHealthRetreatThreshold,
                        });
                    }

                    if (!entityManager.HasComponent<CombatStanceRuntimeComponent>(command.Entity))
                    {
                        entityManager.AddComponentData(command.Entity, new CombatStanceRuntimeComponent());
                    }

                    var attackOrder = new AttackOrderComponent
                    {
                        ExplicitTargetEntity = Entity.Null,
                        IsAttackMoveDestination = true,
                        AttackMoveDestination = memberDestination,
                        IsActive = true,
                    };

                    if (entityManager.HasComponent<AttackOrderComponent>(command.Entity))
                    {
                        entityManager.SetComponentData(command.Entity, attackOrder);
                    }
                    else
                    {
                        entityManager.AddComponentData(command.Entity, attackOrder);
                    }
                }
                else if (entityManager.HasComponent<AttackOrderComponent>(command.Entity))
                {
                    entityManager.RemoveComponent<AttackOrderComponent>(command.Entity);
                }
            }

            return commandUnits.Count;
        }

        private List<CommandUnit> BuildSelectedCombatCommandUnits(EntityManager entityManager)
        {
            var commandUnits = new List<CommandUnit>(selectedEntities.Count);

            for (int i = selectedEntities.Count - 1; i >= 0; i--)
            {
                var entity = selectedEntities[i];
                if (!entityManager.Exists(entity))
                {
                    selectedEntities.RemoveAt(i);
                    continue;
                }

                if (!CanIssueCombatOrder(entityManager, entity))
                {
                    continue;
                }

                float3 startPosition = entityManager.HasComponent<PositionComponent>(entity)
                    ? entityManager.GetComponentData<PositionComponent>(entity).Value
                    : float3.zero;

                commandUnits.Add(new CommandUnit
                {
                    Entity = entity,
                    Position = startPosition,
                    Destination = startPosition,
                });
            }

            commandUnits.Reverse();
            return commandUnits;
        }
    }
}
