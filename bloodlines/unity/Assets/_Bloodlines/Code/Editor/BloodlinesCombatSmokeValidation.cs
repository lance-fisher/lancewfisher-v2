#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Pathing;
using Bloodlines.Systems;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated combat smoke validation that runs in an isolated test world.
    /// It proves the first combat lane can auto-acquire, resolve damage, finalize death,
    /// and honor explicit attack / attack-move commands.
    /// </summary>
    public static class BloodlinesCombatSmokeValidation
    {
        private const double TimeoutSeconds = 10d;
        private const float StepSeconds = 0.1f;

        [MenuItem("Bloodlines/Validation/Run Combat Smoke Validation")]
        public static void RunCombatSmokeValidation()
        {
            ExecuteValidation(exitOnComplete: false);
        }

        public static void RunBatchCombatSmokeValidation()
        {
            ExecuteValidation(exitOnComplete: true);
        }

        private static void ExecuteValidation(bool exitOnComplete)
        {
            bool success = false;
            string message;

            try
            {
                success = RunValidation(out message);
                if (success)
                {
                    UnityDebug.Log(message);
                }
                else
                {
                    UnityDebug.LogError(message);
                }
            }
            catch (Exception exception)
            {
                message = "Combat smoke validation failed with an exception: " + exception;
                UnityDebug.LogError(message);
            }

            if (exitOnComplete)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunValidation(out string message)
        {
            if (!RunMeleePhase(out string meleeMessage))
            {
                message = meleeMessage;
                return false;
            }

            UnityDebug.Log(meleeMessage);

            if (!RunProjectilePhase(out string projectileMessage))
            {
                message = projectileMessage;
                return false;
            }

            UnityDebug.Log(projectileMessage);

            if (!RunAttackOrderPhase(out string attackOrderMessage))
            {
                message = attackOrderMessage;
                return false;
            }

            UnityDebug.Log(attackOrderMessage);

            if (!RunAttackMovePhase(out string attackMoveMessage))
            {
                message = attackMoveMessage;
                return false;
            }

            UnityDebug.Log(attackMoveMessage);
            
            if (!RunTargetVisibilityPhase(out string targetVisibilityMessage))
            {
                message = targetVisibilityMessage;
                return false;
            }

            UnityDebug.Log(targetVisibilityMessage);
            
            if (!RunGroupMovementPhase(out string groupMovementMessage))
            {
                message = groupMovementMessage;
                return false;
            }

            UnityDebug.Log(groupMovementMessage);
            
            if (!RunSeparationPhase(out string separationMessage))
            {
                message = separationMessage;
                return false;
            }

            UnityDebug.Log(separationMessage);

            if (!RunStancePhase(out string stanceMessage))
            {
                message = stanceMessage;
                return false;
            }

            UnityDebug.Log(stanceMessage);
            message =
                "Combat smoke validation passed: meleePhase=True, projectilePhase=True, explicitAttackPhase=True, attackMovePhase=True, targetVisibilityPhase=True, groupMovementPhase=True, separationPhase=True, stancePhase=True.";
            return true;
        }

        private static bool RunMeleePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_Melee");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", "enemy");
            CreateFaction(entityManager, "enemy", "player");

            Entity playerUnit = CreateCombatUnit(
                entityManager,
                "player",
                "militia",
                new float3(0f, 0f, 0f),
                maxHealth: 12f,
                attackDamage: 3f,
                attackRange: 1.5f,
                attackCooldown: 0.4f,
                sight: 8f);

            Entity enemyUnit = CreateCombatUnit(
                entityManager,
                "enemy",
                "militia",
                new float3(1.1f, 0f, 0f),
                maxHealth: 9f,
                attackDamage: 2f,
                attackRange: 1.5f,
                attackCooldown: 0.4f,
                sight: 8f);

            return RunUntilSingleDeath(
                world,
                entityManager,
                playerUnit,
                enemyUnit,
                "Combat smoke validation melee phase passed",
                out message);
        }

        private static bool RunProjectilePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_Projectile");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", "enemy");
            CreateFaction(entityManager, "enemy", "player");

            Entity bowman = CreateCombatUnit(
                entityManager,
                "player",
                "bowman",
                new float3(0f, 0f, 0f),
                maxHealth: 10f,
                attackDamage: 10f,
                attackRange: 4f,
                attackCooldown: 0.8f,
                sight: 8f,
                role: UnitRole.Ranged,
                projectileSpeed: 4.5f,
                projectileMaxLifetimeSeconds: 4f,
                projectileArrivalRadius: 0.2f);

            Entity villager = CreateCombatUnit(
                entityManager,
                "enemy",
                "villager",
                new float3(3.2f, 0f, 0f),
                maxHealth: 9f,
                attackDamage: 0f,
                attackRange: 0.5f,
                attackCooldown: 1f,
                sight: 6f,
                role: UnitRole.Worker);

            bool sawProjectileInFlight = false;
            double elapsed = 0d;

            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                using var projectileQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ProjectileComponent>());
                int projectileCount = projectileQuery.CalculateEntityCount();
                if (projectileCount > 0 && !entityManager.HasComponent<DeadTag>(villager))
                {
                    sawProjectileInFlight = true;
                }

                if (!entityManager.HasComponent<DeadTag>(villager))
                {
                    continue;
                }

                if (!sawProjectileInFlight)
                {
                    message = "Combat smoke validation failed: projectile phase never observed an in-flight projectile.";
                    return false;
                }

                var villagerHealth = entityManager.GetComponentData<HealthComponent>(villager);
                if (villagerHealth.Current > 0f)
                {
                    message = "Combat smoke validation failed: projectile phase marked the target dead before health reached zero.";
                    return false;
                }

                message =
                    "Combat smoke validation projectile phase passed: projectileObserved=True, dead=" +
                    Quote(entityManager.GetComponentData<FactionComponent>(villager).FactionId.ToString()) +
                    ", elapsedSeconds=" + elapsed.ToString("0.###") + ".";
                return true;
            }

            message = "Combat smoke validation failed: timeout waiting for the ranged projectile phase to resolve.";
            return false;
        }

        private static bool RunAttackOrderPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_AttackOrders");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", "enemy");
            CreateFaction(entityManager, "enemy", "player");

            Entity playerMilitia = CreateCombatUnit(
                entityManager,
                "player",
                "militia",
                new float3(0f, 0f, 0f),
                maxHealth: 12f,
                attackDamage: 3f,
                attackRange: 1.5f,
                attackCooldown: 0.4f,
                sight: 8f);

            Entity enemyMilitia = CreateCombatUnit(
                entityManager,
                "enemy",
                "villager",
                new float3(6f, 0f, 0f),
                maxHealth: 6f,
                attackDamage: 0f,
                attackRange: 0.5f,
                attackCooldown: 1f,
                sight: 6f,
                role: UnitRole.Worker);

            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            if (!commandSurface.TryDebugSelectAllControlledUnits())
            {
                message = "Combat smoke validation failed: attack-order phase could not select the controlled combat unit.";
                return false;
            }

            if (!commandSurface.TryDebugIssueAttackOrderOnNearestHostile("player", out bool issued) || !issued)
            {
                message = "Combat smoke validation failed: attack-order phase could not issue an explicit hostile target order.";
                return false;
            }

            bool sawExplicitTarget = false;
            bool sawChase = false;
            double elapsed = 0d;

            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                if (entityManager.HasComponent<AttackTargetComponent>(playerMilitia))
                {
                    var attackTarget = entityManager.GetComponentData<AttackTargetComponent>(playerMilitia);
                    if (attackTarget.TargetEntity == enemyMilitia)
                    {
                        sawExplicitTarget = true;
                    }
                }

                if (entityManager.GetComponentData<MoveCommandComponent>(playerMilitia).IsActive)
                {
                    sawChase = true;
                }

                if (!entityManager.HasComponent<DeadTag>(enemyMilitia))
                {
                    continue;
                }

                if (!sawExplicitTarget)
                {
                    message = "Combat smoke validation failed: attack-order phase never locked onto the commanded hostile target.";
                    return false;
                }

                if (!sawChase)
                {
                    message = "Combat smoke validation failed: attack-order phase never observed chase movement toward the explicit target.";
                    return false;
                }

                if (entityManager.HasComponent<AttackTargetComponent>(playerMilitia))
                {
                    message = "Combat smoke validation failed: attack-order phase left a residual AttackTargetComponent on the attacker.";
                    return false;
                }

                if (entityManager.HasComponent<AttackOrderComponent>(playerMilitia) &&
                    entityManager.GetComponentData<AttackOrderComponent>(playerMilitia).IsActive)
                {
                    message = "Combat smoke validation failed: attack-order phase left the explicit attack order active after target death.";
                    return false;
                }

                var enemyHealth = entityManager.GetComponentData<HealthComponent>(enemyMilitia);
                if (enemyHealth.Current > 0f)
                {
                    message = "Combat smoke validation failed: attack-order phase marked the explicit target dead before health reached zero.";
                    return false;
                }

                message =
                    "Combat smoke validation explicit attack phase passed: explicitTargetObserved=True, chaseObserved=True, residualTarget=False, dead=" +
                    Quote(entityManager.GetComponentData<FactionComponent>(enemyMilitia).FactionId.ToString()) +
                    ", elapsedSeconds=" + elapsed.ToString("0.###") + ".";
                return true;
            }

            message = "Combat smoke validation failed: timeout waiting for the explicit attack-order phase to resolve.";
            return false;
        }

        private static bool RunAttackMovePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_AttackMove");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", "enemy");
            CreateFaction(entityManager, "enemy", "player");
            CreateFaction(entityManager, "neutral");

            Entity playerMilitia = CreateCombatUnit(
                entityManager,
                "player",
                "militia",
                new float3(0f, 0f, 0f),
                maxHealth: 12f,
                attackDamage: 3f,
                attackRange: 1.5f,
                attackCooldown: 0.4f,
                sight: 3.6f);

            Entity neutralDecoy = CreateCombatUnit(
                entityManager,
                "neutral",
                "villager",
                new float3(2.4f, 0f, 0f),
                maxHealth: 10f,
                attackDamage: 0f,
                attackRange: 0.5f,
                attackCooldown: 1f,
                sight: 2f,
                role: UnitRole.Worker);

            Entity hostileTarget = CreateCombatUnit(
                entityManager,
                "enemy",
                "villager",
                new float3(6.2f, 0f, 0f),
                maxHealth: 6f,
                attackDamage: 0f,
                attackRange: 0.5f,
                attackCooldown: 1f,
                sight: 2f,
                role: UnitRole.Worker);

            float3 destination = new float3(9f, 0f, 0f);

            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            if (!commandSurface.TryDebugSelectAllControlledUnits())
            {
                message = "Combat smoke validation failed: attack-move phase could not select the controlled combat unit.";
                return false;
            }

            if (!commandSurface.TryDebugIssueAttackMove(destination, out int orderedCount) || orderedCount <= 0)
            {
                message = "Combat smoke validation failed: attack-move phase could not issue an attack-move order.";
                return false;
            }

            bool sawInitialMarch = false;
            bool sawHostileEngagement = false;
            bool sawResumeTowardDestination = false;
            double elapsed = 0d;

            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                var militiaPosition = entityManager.GetComponentData<PositionComponent>(playerMilitia).Value;
                var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(playerMilitia);

                if (!entityManager.HasComponent<AttackTargetComponent>(playerMilitia) &&
                    moveCommand.IsActive &&
                    math.distancesq(militiaPosition, float3.zero) > 0.25f)
                {
                    sawInitialMarch = true;
                }

                if (entityManager.HasComponent<AttackTargetComponent>(playerMilitia) &&
                    entityManager.GetComponentData<AttackTargetComponent>(playerMilitia).TargetEntity == hostileTarget)
                {
                    sawHostileEngagement = true;
                }

                if (entityManager.HasComponent<DeadTag>(hostileTarget))
                {
                    if (moveCommand.IsActive &&
                        math.distancesq(moveCommand.Destination, destination) <= 0.25f * 0.25f)
                    {
                        sawResumeTowardDestination = true;
                    }

                    if (math.distancesq(militiaPosition, destination) <= 0.25f * 0.25f &&
                        !moveCommand.IsActive)
                    {
                        if (!sawInitialMarch)
                        {
                            message = "Combat smoke validation failed: attack-move phase never observed the initial destination march.";
                            return false;
                        }

                        if (!sawHostileEngagement)
                        {
                            message = "Combat smoke validation failed: attack-move phase never observed the hostile engagement interrupt.";
                            return false;
                        }

                        if (!sawResumeTowardDestination)
                        {
                            message = "Combat smoke validation failed: attack-move phase never observed the militia resume marching after the kill.";
                            return false;
                        }

                        if (entityManager.HasComponent<DeadTag>(neutralDecoy))
                        {
                            message = "Combat smoke validation failed: attack-move phase attacked a neutral decoy that should have remained ignored.";
                            return false;
                        }

                        message =
                            "Combat smoke validation attack-move phase passed: hostileDead=True, neutralIgnored=True, destinationReached=True, elapsedSeconds=" +
                            elapsed.ToString("0.###") + ".";
                        return true;
                    }
                }
            }

            float3 finalPosition = entityManager.GetComponentData<PositionComponent>(playerMilitia).Value;
            var finalMoveCommand = entityManager.GetComponentData<MoveCommandComponent>(playerMilitia);
            bool hostileDead = entityManager.HasComponent<DeadTag>(hostileTarget);
            bool neutralDead = entityManager.HasComponent<DeadTag>(neutralDecoy);
            bool hasAttackTarget = entityManager.HasComponent<AttackTargetComponent>(playerMilitia);
            bool hasAttackOrder = entityManager.HasComponent<AttackOrderComponent>(playerMilitia);

            message =
                "Combat smoke validation failed: timeout waiting for the attack-move phase to resolve. " +
                "initialMarchObserved=" + sawInitialMarch +
                ", hostileEngagementObserved=" + sawHostileEngagement +
                ", resumedTowardDestinationObserved=" + sawResumeTowardDestination +
                ", hostileDead=" + hostileDead +
                ", neutralDead=" + neutralDead +
                ", hasAttackTarget=" + hasAttackTarget +
                ", hasAttackOrder=" + hasAttackOrder +
                ", moveActive=" + finalMoveCommand.IsActive +
                ", moveDestination=" + finalMoveCommand.Destination +
                ", finalPosition=" + finalPosition + ".";
            return false;
        }

        private static bool RunTargetVisibilityPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_TargetVisibility");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", "enemy");
            CreateFaction(entityManager, "enemy", "player");

            Entity playerMilitia = CreateCombatUnit(
                entityManager,
                "player",
                "militia",
                new float3(0f, 0f, 0f),
                maxHealth: 12f,
                attackDamage: 3f,
                attackRange: 1.5f,
                attackCooldown: 0.4f,
                sight: 3.5f);

            Entity firstEnemy = CreateCombatUnit(
                entityManager,
                "enemy",
                "villager",
                new float3(2.8f, 0f, 0f),
                maxHealth: 20f,
                attackDamage: 0f,
                attackRange: 0.5f,
                attackCooldown: 1f,
                sight: 2f,
                role: UnitRole.Worker);

            Entity replacementEnemy = CreateCombatUnit(
                entityManager,
                "enemy",
                "villager",
                new float3(8f, 0f, 0f),
                maxHealth: 20f,
                attackDamage: 0f,
                attackRange: 0.5f,
                attackCooldown: 1f,
                sight: 2f,
                role: UnitRole.Worker);

            bool initialAcquireObserved = false;
            bool targetsRepositioned = false;
            bool sightLossCleared = false;
            bool chaseStoppedOnLoss = false;
            bool reacquireCooldownObserved = false;
            bool replacementTargetObserved = false;
            bool reacquireDelayed = false;
            double sightLossElapsed = -1d;
            double elapsed = 0d;

            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                var combatStats = entityManager.GetComponentData<CombatStatsComponent>(playerMilitia);
                var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(playerMilitia);

                if (!targetsRepositioned &&
                    entityManager.HasComponent<AttackTargetComponent>(playerMilitia) &&
                    entityManager.GetComponentData<AttackTargetComponent>(playerMilitia).TargetEntity == firstEnemy)
                {
                    initialAcquireObserved = true;
                    targetsRepositioned = true;
                    SetUnitPosition(entityManager, firstEnemy, new float3(20f, 0f, 0f));
                    SetUnitPosition(entityManager, replacementEnemy, new float3(2.9f, 0f, 0f));
                    continue;
                }

                if (!targetsRepositioned)
                {
                    continue;
                }

                if (!entityManager.HasComponent<AttackTargetComponent>(playerMilitia))
                {
                    if (!sightLossCleared)
                    {
                        sightLossCleared = true;
                        sightLossElapsed = elapsed;
                        chaseStoppedOnLoss = !moveCommand.IsActive;
                        reacquireCooldownObserved = combatStats.AcquireCooldownRemaining > 0f;
                    }

                    continue;
                }

                if (!sightLossCleared)
                {
                    continue;
                }

                if (entityManager.GetComponentData<AttackTargetComponent>(playerMilitia).TargetEntity != replacementEnemy)
                {
                    continue;
                }

                replacementTargetObserved = true;
                reacquireDelayed = elapsed - sightLossElapsed >= 0.2d;

                if (!initialAcquireObserved)
                {
                    message = "Combat smoke validation failed: target-visibility phase never observed the initial passive acquire.";
                    return false;
                }

                if (!chaseStoppedOnLoss)
                {
                    message = "Combat smoke validation failed: target-visibility phase did not stop chase movement after sight loss.";
                    return false;
                }

                if (!reacquireCooldownObserved)
                {
                    message = "Combat smoke validation failed: target-visibility phase never observed a reacquire cooldown after sight loss.";
                    return false;
                }

                if (!reacquireDelayed)
                {
                    message = "Combat smoke validation failed: target-visibility phase reacquired a new hostile without the expected throttle delay.";
                    return false;
                }

                message =
                    "Combat smoke validation target-visibility phase passed: sightLossCleared=True, chaseStopped=True, reacquireCooldownObserved=True, replacementTargetObserved=True, reacquireDelayed=True, elapsedSeconds=" +
                    elapsed.ToString("0.###") + ".";
                return true;
            }

            message =
                "Combat smoke validation failed: timeout waiting for the target-visibility phase to resolve. " +
                "initialAcquireObserved=" + initialAcquireObserved +
                ", sightLossCleared=" + sightLossCleared +
                ", chaseStoppedOnLoss=" + chaseStoppedOnLoss +
                ", reacquireCooldownObserved=" + reacquireCooldownObserved +
                ", replacementTargetObserved=" + replacementTargetObserved +
                ", reacquireDelayed=" + reacquireDelayed +
                ", lastAcquireCooldownRemaining=" + entityManager.GetComponentData<CombatStatsComponent>(playerMilitia).AcquireCooldownRemaining.ToString("0.###") + ".";
            return false;
        }

        private static bool RunGroupMovementPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_GroupMovement");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player");

            var startingPositions = new[]
            {
                new float3(-1.4f, 0f, -0.8f),
                new float3(0.2f, 0f, 0.4f),
                new float3(1.5f, 0f, -0.2f),
                new float3(2.3f, 0f, 1.3f),
            };

            var units = new[]
            {
                CreateCombatUnit(entityManager, "player", "militia", startingPositions[0], 12f, 3f, 1.5f, 0.4f, 8f),
                CreateCombatUnit(entityManager, "player", "militia", startingPositions[1], 12f, 3f, 1.5f, 0.4f, 8f),
                CreateCombatUnit(entityManager, "player", "militia", startingPositions[2], 12f, 3f, 1.5f, 0.4f, 8f),
                CreateCombatUnit(entityManager, "player", "militia", startingPositions[3], 12f, 3f, 1.5f, 0.4f, 8f),
            };

            float[] initialPairwiseDistances = CapturePairwiseDistances(startingPositions);
            float3 destination = new float3(11.5f, 0f, 7.75f);

            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            if (!commandSurface.TryDebugSelectAllControlledUnits())
            {
                message = "Combat smoke validation failed: group-movement phase could not select the controlled units.";
                return false;
            }

            if (!commandSurface.TryDebugIssueGroupMoveOrder(destination, attackMove: false, out int memberCount) ||
                memberCount != units.Length)
            {
                message = "Combat smoke validation failed: group-movement phase could not issue a group move order to the full selection.";
                return false;
            }

            if (!commandSurface.TryDebugInspectGroupMovement(
                    units[0],
                    out FixedString32Bytes groupId,
                    out float3 localOffset,
                    out float3 destinationCenter) ||
                groupId.IsEmpty ||
                math.distancesq(destinationCenter, destination) > 0.01f)
            {
                message = "Combat smoke validation failed: group-movement phase did not stamp inspectable per-unit group metadata.";
                return false;
            }

            double elapsed = 0d;
            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                bool allAtRest = true;
                for (int i = 0; i < units.Length; i++)
                {
                    if (entityManager.GetComponentData<MoveCommandComponent>(units[i]).IsActive)
                    {
                        allAtRest = false;
                        break;
                    }
                }

                if (!allAtRest)
                {
                    continue;
                }

                float3[] finalPositions = CapturePositions(entityManager, units);
                float3 finalCentroid = ComputeCentroid(finalPositions);
                if (math.distance(finalCentroid, destination) > 0.6f)
                {
                    message =
                        "Combat smoke validation failed: group-movement phase reached rest with the group centroid too far from the destination. " +
                        "centroid=" + finalCentroid +
                        ", destination=" + destination + ".";
                    return false;
                }

                if (!PairwiseDistancesMatch(finalPositions, initialPairwiseDistances, 0.6f, out float maxDeviation))
                {
                    message =
                        "Combat smoke validation failed: group-movement phase distorted the preserved formation beyond tolerance. " +
                        "maxDeviation=" + maxDeviation.ToString("0.###") + ".";
                    return false;
                }

                if (HasCoincidentPositions(finalPositions, 0.05f, out float minimumDistance))
                {
                    message =
                        "Combat smoke validation failed: group-movement phase left two units occupying the same resting position. " +
                        "minimumDistance=" + minimumDistance.ToString("0.###") + ".";
                    return false;
                }

                if (math.length(localOffset) <= 0.001f)
                {
                    message = "Combat smoke validation failed: group-movement phase expected a non-zero preserved local offset for the inspected unit.";
                    return false;
                }

                message =
                    "Combat smoke validation group-movement phase passed: memberCount=" + memberCount +
                    ", centroidDistanceToDestination=" + math.distance(finalCentroid, destination).ToString("0.###") +
                    ", maxPairwiseDeviation=" + maxDeviation.ToString("0.###") +
                    ", elapsedSeconds=" + elapsed.ToString("0.###") + ".";
                return true;
            }

            message = "Combat smoke validation failed: timeout waiting for the group-movement phase to settle.";
            return false;
        }

        private static bool RunSeparationPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_Separation");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player");

            float3 spawnPosition = new float3(2f, 0f, -1f);
            var units = new Entity[6];
            for (int i = 0; i < units.Length; i++)
            {
                units[i] = CreateCombatUnit(
                    entityManager,
                    "player",
                    "militia",
                    spawnPosition,
                    maxHealth: 12f,
                    attackDamage: 3f,
                    attackRange: 1.5f,
                    attackCooldown: 0.4f,
                    sight: 8f);
            }

            double elapsed = 0d;
            while (elapsed < 3.5d)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;
            }

            float3[] finalPositions = CapturePositions(entityManager, units);
            float3 finalCentroid = ComputeCentroid(finalPositions);
            float separationRadius = entityManager.GetComponentData<UnitSeparationComponent>(units[0]).Radius;

            if (HasCoincidentPositions(finalPositions, separationRadius * 0.9f, out float minimumDistance))
            {
                message =
                    "Combat smoke validation failed: separation phase left units inside their minimum personal space. " +
                    "minimumDistance=" + minimumDistance.ToString("0.###") +
                    ", requiredMinimum=" + (separationRadius * 0.9f).ToString("0.###") + ".";
                return false;
            }

            if (math.distance(finalCentroid, spawnPosition) > 3f)
            {
                message =
                    "Combat smoke validation failed: separation phase drifted the spawn centroid too far. " +
                    "spawnCentroid=" + spawnPosition +
                    ", finalCentroid=" + finalCentroid + ".";
                return false;
            }

            message =
                "Combat smoke validation separation phase passed: unitCount=" + units.Length +
                ", minimumDistance=" + minimumDistance.ToString("0.###") +
                ", centroidDrift=" + math.distance(finalCentroid, spawnPosition).ToString("0.###") + ".";
            return true;
        }

        private static bool RunStancePhase(out string message)
        {
            if (!RunHoldPositionStanceSubPhase(out string holdMessage))
            {
                message = holdMessage;
                return false;
            }

            UnityDebug.Log(holdMessage);

            if (!RunRetreatStanceSubPhase(out string retreatMessage))
            {
                message = retreatMessage;
                return false;
            }

            message =
                "Combat smoke validation stance phase passed: holdPosition=True, retreatOnLowHealth=True. " +
                retreatMessage;
            return true;
        }

        private static bool RunHoldPositionStanceSubPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_HoldPosition");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", "enemy");
            CreateFaction(entityManager, "enemy", "player");

            Entity playerMilitia = CreateCombatUnit(
                entityManager,
                "player",
                "militia",
                new float3(0f, 0f, 0f),
                maxHealth: 12f,
                attackDamage: 3f,
                attackRange: 1.5f,
                attackCooldown: 0.4f,
                sight: 8f);

            Entity enemyMilitia = CreateCombatUnit(
                entityManager,
                "enemy",
                "militia",
                new float3(5f, 0f, 0f),
                maxHealth: 14f,
                attackDamage: 1f,
                attackRange: 1.5f,
                attackCooldown: 0.6f,
                sight: 8f);

            var initialHealth = entityManager.GetComponentData<HealthComponent>(enemyMilitia);
            var enemyMove = entityManager.GetComponentData<MoveCommandComponent>(enemyMilitia);
            enemyMove.Destination = new float3(0.4f, 0f, 0f);
            enemyMove.StoppingDistance = 0.2f;
            enemyMove.IsActive = true;
            entityManager.SetComponentData(enemyMilitia, enemyMove);

            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            if (!commandSurface.TryDebugSetStance(playerMilitia, CombatStance.HoldPosition))
            {
                message = "Combat smoke validation failed: hold-position phase could not set the militia stance.";
                return false;
            }

            double elapsed = 0d;
            while (elapsed < 4d)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;
            }

            float movementFromOrigin = math.distance(
                entityManager.GetComponentData<PositionComponent>(playerMilitia).Value,
                float3.zero);
            float enemyHealthDelta = initialHealth.Current - entityManager.GetComponentData<HealthComponent>(enemyMilitia).Current;

            if (movementFromOrigin > 0.5f)
            {
                message =
                    "Combat smoke validation failed: hold-position phase let the militia chase outside its anchor point. " +
                    "movementFromOrigin=" + movementFromOrigin.ToString("0.###") + ".";
                return false;
            }

            if (enemyHealthDelta <= 0f)
            {
                message =
                    "Combat smoke validation failed: hold-position phase never fired back once the hostile walked into range.";
                return false;
            }

            message =
                "Combat smoke validation hold-position sub-phase passed: movementFromOrigin=" +
                movementFromOrigin.ToString("0.###") +
                ", enemyHealthDelta=" + enemyHealthDelta.ToString("0.###") + ".";
            return true;
        }

        private static bool RunRetreatStanceSubPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesCombatSmokeValidation_Retreat");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player", "enemy");
            CreateFaction(entityManager, "enemy", "player");

            Entity playerMilitia = CreateCombatUnit(
                entityManager,
                "player",
                "militia",
                new float3(0f, 0f, 0f),
                maxHealth: 12f,
                attackDamage: 3f,
                attackRange: 1.5f,
                attackCooldown: 0.4f,
                sight: 8f);

            var damagedHealth = entityManager.GetComponentData<HealthComponent>(playerMilitia);
            damagedHealth.Current = damagedHealth.Max * 0.2f;
            entityManager.SetComponentData(playerMilitia, damagedHealth);

            Entity enemyMilitia = CreateCombatUnit(
                entityManager,
                "enemy",
                "militia",
                new float3(0.5f, 0f, 0f),
                maxHealth: 14f,
                attackDamage: 1f,
                attackRange: 1.5f,
                attackCooldown: 0.6f,
                sight: 8f);

            float3 retreatAnchor = new float3(8f, 0f, 0f);
            CreateFriendlyBuilding(
                entityManager,
                "player",
                "command_hall",
                retreatAnchor,
                maxHealth: 50f);

            float initialEnemyDistance = math.distance(
                entityManager.GetComponentData<PositionComponent>(playerMilitia).Value,
                entityManager.GetComponentData<PositionComponent>(enemyMilitia).Value);
            float initialRetreatDistance = math.distance(
                entityManager.GetComponentData<PositionComponent>(playerMilitia).Value,
                retreatAnchor);

            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var commandSurface = commandSurfaceScope.CommandSurface;
            if (!commandSurface.TryDebugSetStance(enemyMilitia, CombatStance.HoldPosition))
            {
                message = "Combat smoke validation failed: retreat phase could not anchor the hostile reference unit.";
                return false;
            }

            if (!commandSurface.TryDebugSetStance(
                    playerMilitia,
                    CombatStance.RetreatOnLowHealth,
                    lowHealthThreshold: 0.25f))
            {
                message = "Combat smoke validation failed: retreat phase could not set the militia stance.";
                return false;
            }

            double elapsed = 0d;
            while (elapsed < 5d)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;
            }

            float finalEnemyDistance = math.distance(
                entityManager.GetComponentData<PositionComponent>(playerMilitia).Value,
                entityManager.GetComponentData<PositionComponent>(enemyMilitia).Value);
            float finalRetreatDistance = math.distance(
                entityManager.GetComponentData<PositionComponent>(playerMilitia).Value,
                retreatAnchor);
            bool stillAttacking = entityManager.HasComponent<AttackTargetComponent>(playerMilitia);
            var stance = entityManager.GetComponentData<CombatStanceComponent>(playerMilitia);

            if (stillAttacking)
            {
                message = "Combat smoke validation failed: retreat phase left the low-health militia actively attacking.";
                return false;
            }

            if (finalRetreatDistance >= initialRetreatDistance - 4f)
            {
                message =
                    "Combat smoke validation failed: retreat phase did not move meaningfully toward the friendly command hall. " +
                    "initialRetreatDistance=" + initialRetreatDistance.ToString("0.###") +
                    ", finalRetreatDistance=" + finalRetreatDistance.ToString("0.###") + ".";
                return false;
            }

            if (finalEnemyDistance < initialEnemyDistance + 5f)
            {
                message =
                    "Combat smoke validation failed: retreat phase did not create enough separation from the original hostile. " +
                    "initialDistance=" + initialEnemyDistance.ToString("0.###") +
                    ", finalDistance=" + finalEnemyDistance.ToString("0.###") + ".";
                return false;
            }

            if (stance.Stance != CombatStance.RetreatOnLowHealth &&
                stance.Stance != CombatStance.PursueInRange)
            {
                message =
                    "Combat smoke validation failed: retreat phase left the unit in an unexpected stance state " +
                    stance.Stance + ".";
                return false;
            }

            message =
                "Combat smoke validation retreat sub-phase passed: initialDistance=" +
                initialEnemyDistance.ToString("0.###") +
                ", finalDistance=" + finalEnemyDistance.ToString("0.###") +
                ", finalRetreatDistance=" + finalRetreatDistance.ToString("0.###") + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            var endSimulation = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            simulationGroup.AddSystemToUpdateList(endSimulation);
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<GroupMovementResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CombatStanceResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AttackOrderResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AutoAcquireTargetSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AttackResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<UnitMovementSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<UnitSeparationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PositionToLocalTransformSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ProjectileMovementSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ProjectileImpactSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<RecentImpactRecoverySystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DeathResolutionSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static bool RunUntilSingleDeath(
            World world,
            EntityManager entityManager,
            Entity firstEntity,
            Entity secondEntity,
            string successLabel,
            out string message)
        {
            double elapsed = 0d;
            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                bool firstDead = entityManager.HasComponent<DeadTag>(firstEntity);
                bool secondDead = entityManager.HasComponent<DeadTag>(secondEntity);
                if (!firstDead && !secondDead)
                {
                    continue;
                }

                if (firstDead == secondDead)
                {
                    message = "Combat smoke validation failed: both combatants resolved to the same death state.";
                    return false;
                }

                var survivingEntity = firstDead ? secondEntity : firstEntity;
                var deadEntity = firstDead ? firstEntity : secondEntity;
                var survivingHealth = entityManager.GetComponentData<HealthComponent>(survivingEntity);
                var deadHealth = entityManager.GetComponentData<HealthComponent>(deadEntity);

                if (survivingHealth.Current >= survivingHealth.Max)
                {
                    message = "Combat smoke validation failed: surviving unit never took damage.";
                    return false;
                }

                if (!entityManager.HasComponent<DeadTag>(deadEntity) || deadHealth.Current > 0f)
                {
                    message = "Combat smoke validation failed: dead unit was not finalized correctly.";
                    return false;
                }

                message =
                    successLabel + ": dead=" + Quote(entityManager.GetComponentData<FactionComponent>(deadEntity).FactionId.ToString()) +
                    ", survivorHealth=" + survivingHealth.Current.ToString("0.###") +
                    "/" + survivingHealth.Max.ToString("0.###") +
                    ", elapsedSeconds=" + elapsed.ToString("0.###") + ".";
                return true;
            }

            message = "Combat smoke validation failed: timeout waiting for one combatant to die.";
            return false;
        }

        private static void CreateFaction(EntityManager entityManager, string factionId, params string[] hostileFactionIds)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new FactionKindComponent { Kind = FactionKind.Kingdom });
            entityManager.AddComponentData(entity, new PopulationComponent
            {
                Total = 10,
                Cap = 10,
                BaseCap = 10,
                CapBonus = 0,
                Available = 8,
                GrowthAccumulator = 0f,
            });

            var hostility = entityManager.AddBuffer<HostilityComponent>(entity);
            for (int i = 0; i < hostileFactionIds.Length; i++)
            {
                hostility.Add(new HostilityComponent { HostileFactionId = hostileFactionIds[i] });
            }
        }

        private static Entity CreateCombatUnit(
            EntityManager entityManager,
            string factionId,
            string typeId,
            float3 position,
            float maxHealth,
            float attackDamage,
            float attackRange,
            float attackCooldown,
            float sight,
            UnitRole role = UnitRole.Melee,
            SiegeClass siegeClass = SiegeClass.None,
            float projectileSpeed = 0f,
            float projectileMaxLifetimeSeconds = 4f,
            float projectileArrivalRadius = 0.4f)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new PositionComponent { Value = position });
            entityManager.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = maxHealth,
                Max = maxHealth,
            });
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = typeId,
                Role = role,
                SiegeClass = siegeClass,
                PopulationCost = 1,
                Stage = 1,
            });
            entityManager.AddComponentData(entity, new MovementStatsComponent { MaxSpeed = 4.5f });
            entityManager.AddComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = attackDamage,
                AttackRange = attackRange,
                AttackCooldown = attackCooldown,
                Sight = sight,
                CooldownRemaining = 0f,
                TargetAcquireIntervalSeconds = 0.25f,
                AcquireCooldownRemaining = 0f,
                TargetSightGraceSeconds = 0.35f,
                TargetOutOfSightSeconds = 0f,
            });
            entityManager.AddComponentData(entity, new UnitSeparationComponent
            {
                Radius = CombatUnitRuntimeDefaults.ResolveSeparationRadius(role, siegeClass),
            });
            entityManager.AddComponentData(entity, new CombatStanceComponent
            {
                Stance = CombatUnitRuntimeDefaults.ResolveDefaultStance(role),
                LowHealthRetreatThreshold = CombatUnitRuntimeDefaults.DefaultLowHealthRetreatThreshold,
            });
            entityManager.AddComponentData(entity, new CombatStanceRuntimeComponent());
            entityManager.AddComponentData(entity, new RecentImpactComponent());
            entityManager.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = attackRange,
                IsActive = false,
            });

            if (projectileSpeed > 0f)
            {
                entityManager.AddComponentData(entity, new ProjectileFactoryComponent
                {
                    ProjectileSpeed = projectileSpeed,
                    ProjectileMaxLifetimeSeconds = projectileMaxLifetimeSeconds,
                    ProjectileArrivalRadius = projectileArrivalRadius,
                });
            }

            return entity;
        }

        private static Entity CreateFriendlyBuilding(
            EntityManager entityManager,
            string factionId,
            string buildingTypeId,
            float3 position,
            float maxHealth = 30f)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            entityManager.AddComponentData(entity, new PositionComponent { Value = position });
            entityManager.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = maxHealth,
                Max = maxHealth,
            });
            entityManager.AddComponentData(entity, new BuildingTypeComponent
            {
                TypeId = buildingTypeId,
                FortificationRole = FortificationRole.None,
                StructuralDamageMultiplier = 1f,
                PopulationCapBonus = 0,
                BlocksPassage = false,
                SupportsSiegePreparation = false,
                SupportsSiegeLogistics = false,
            });
            return entity;
        }

        private static string Quote(string value)
        {
            return "'" + (value ?? string.Empty) + "'";
        }

        private static float3[] CapturePositions(EntityManager entityManager, IReadOnlyList<Entity> entities)
        {
            var positions = new float3[entities.Count];
            for (int i = 0; i < entities.Count; i++)
            {
                positions[i] = entityManager.GetComponentData<PositionComponent>(entities[i]).Value;
            }

            return positions;
        }

        private static float[] CapturePairwiseDistances(IReadOnlyList<float3> positions)
        {
            int pairCount = (positions.Count * (positions.Count - 1)) / 2;
            var pairwiseDistances = new float[pairCount];
            int index = 0;

            for (int i = 0; i < positions.Count; i++)
            {
                for (int j = i + 1; j < positions.Count; j++)
                {
                    pairwiseDistances[index++] = math.distance(positions[i], positions[j]);
                }
            }

            return pairwiseDistances;
        }

        private static float3 ComputeCentroid(IReadOnlyList<float3> positions)
        {
            float3 centroid = float3.zero;
            for (int i = 0; i < positions.Count; i++)
            {
                centroid += positions[i];
            }

            return centroid / math.max(1, positions.Count);
        }

        private static bool PairwiseDistancesMatch(
            IReadOnlyList<float3> positions,
            IReadOnlyList<float> expectedDistances,
            float tolerance,
            out float maxDeviation)
        {
            maxDeviation = 0f;
            int index = 0;

            for (int i = 0; i < positions.Count; i++)
            {
                for (int j = i + 1; j < positions.Count; j++)
                {
                    float distance = math.distance(positions[i], positions[j]);
                    float deviation = math.abs(distance - expectedDistances[index++]);
                    if (deviation > maxDeviation)
                    {
                        maxDeviation = deviation;
                    }

                    if (deviation > tolerance)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool HasCoincidentPositions(
            IReadOnlyList<float3> positions,
            float minimumAllowedDistance,
            out float minimumDistance)
        {
            minimumDistance = float.MaxValue;

            for (int i = 0; i < positions.Count; i++)
            {
                for (int j = i + 1; j < positions.Count; j++)
                {
                    float distance = math.distance(positions[i], positions[j]);
                    if (distance < minimumDistance)
                    {
                        minimumDistance = distance;
                    }

                    if (distance <= minimumAllowedDistance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void SetUnitPosition(EntityManager entityManager, Entity entity, float3 position)
        {
            entityManager.SetComponentData(entity, new PositionComponent { Value = position });

            if (!entityManager.HasComponent<LocalTransform>(entity))
            {
                return;
            }

            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            transform.Position = position;
            entityManager.SetComponentData(entity, transform);
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject commandSurfaceObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                commandSurfaceObject = new GameObject("BloodlinesCombatSmokeValidation_CommandSurface");
                CommandSurface = commandSurfaceObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(commandSurfaceObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
