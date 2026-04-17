#if UNITY_EDITOR
using System;
using Bloodlines.Components;
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
    /// It proves the first combat lane can auto-acquire, resolve damage, and finalize a death.
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
            using var world = new World("BloodlinesCombatSmokeValidation");
            var entityManager = world.EntityManager;
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            var endSimulation = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            simulationGroup.AddSystemToUpdateList(endSimulation);
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AutoAcquireTargetSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AttackResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DeathResolutionSystem>());
            simulationGroup.SortSystems();

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

            double elapsed = 0d;
            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                bool playerDead = entityManager.HasComponent<DeadTag>(playerUnit);
                bool enemyDead = entityManager.HasComponent<DeadTag>(enemyUnit);
                if (!playerDead && !enemyDead)
                {
                    continue;
                }

                if (playerDead == enemyDead)
                {
                    message = "Combat smoke validation failed: both combatants resolved to the same death state.";
                    return false;
                }

                var survivingEntity = playerDead ? enemyUnit : playerUnit;
                var deadEntity = playerDead ? playerUnit : enemyUnit;
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
                    "Combat smoke validation passed: dead=" + Quote(entityManager.GetComponentData<FactionComponent>(deadEntity).FactionId.ToString()) +
                    ", survivorHealth=" + survivingHealth.Current.ToString("0.###") +
                    "/" + survivingHealth.Max.ToString("0.###") +
                    ", elapsedSeconds=" + elapsed.ToString("0.###") + ".";
                return true;
            }

            message = "Combat smoke validation failed: timeout waiting for one combatant to die.";
            return false;
        }

        private static void CreateFaction(EntityManager entityManager, string factionId, string hostileFactionId)
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
            hostility.Add(new HostilityComponent { HostileFactionId = hostileFactionId });
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
            float sight)
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
                Role = UnitRole.Melee,
                SiegeClass = SiegeClass.None,
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
            });
            entityManager.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = attackRange,
                IsActive = false,
            });
            return entity;
        }

        private static string Quote(string value)
        {
            return "'" + (value ?? string.Empty) + "'";
        }
    }
}
#endif
