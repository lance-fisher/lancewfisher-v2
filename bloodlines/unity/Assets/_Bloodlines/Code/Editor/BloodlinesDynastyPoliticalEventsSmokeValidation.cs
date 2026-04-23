using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Dynasties;
using Bloodlines.Faith;
using Bloodlines.GameTime;
using Bloodlines.PlayerDiplomacy;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    public static class BloodlinesDynastyPoliticalEventsSmokeValidation
    {
        [MenuItem("Bloodlines/Validation/Run Dynasty Political Events Smoke")]
        public static void RunMenu()
        {
            RunInternal(batchMode: false);
        }

        public static string RunBatchDynastyPoliticalEventsSmokeValidation()
        {
            return RunInternal(batchMode: true);
        }

        static string RunInternal(bool batchMode)
        {
            int exitCode = 0;
            try
            {
                string phase1 = RunAggregatePhase();
                string phase2 = RunDivineRightFailurePhase();
                string phase3 = RunExpiryPhase();
                string summary =
                    $"Dynasty political events smoke validation passed. {phase1}; {phase2}; {phase3}";
                UnityDebug.Log(summary);
                return summary;
            }
            catch
            {
                exitCode = 1;
                throw;
            }
            finally
            {
                if (batchMode)
                {
                    EditorApplication.Exit(exitCode);
                }
            }
        }

        static string RunAggregatePhase()
        {
            using var world = CreateValidationWorld("DynastyPoliticalAggregatePhase");
            var entityManager = world.EntityManager;

            var faction = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(DynastyPoliticalEventAggregateComponent));
            entityManager.SetComponentData(faction, new FactionComponent
            {
                FactionId = new FixedString32Bytes("player"),
            });
            var events = entityManager.AddBuffer<DynastyPoliticalEventComponent>(faction);
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = DynastyPoliticalEventTypes.SuccessionShock,
                ExpiresAtInWorldDays = 10f,
                ResourceTrickleFactor = 0.85f,
                AttackMultiplier = 0.9f,
                StabilizationMultiplier = 0.8f,
            });

            var clock = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clock, new DualClockComponent
            {
                InWorldDays = 5f,
                DaysPerRealSecond = 2f,
            });

            Tick(world, 5f);

            var aggregate = entityManager.GetComponentData<DynastyPoliticalEventAggregateComponent>(faction);
            if (aggregate.ActiveEventCount != 1 ||
                Mathf.Abs(aggregate.ResourceTrickleFactor - 0.85f) > 0.0001f ||
                Mathf.Abs(aggregate.AttackMultiplier - 0.9f) > 0.0001f ||
                Mathf.Abs(aggregate.StabilizationMultiplier - 0.8f) > 0.0001f)
            {
                throw new System.InvalidOperationException(
                    "Dynasty political event aggregate phase failed.");
            }

            return $"phase1ActiveEvents={aggregate.ActiveEventCount},phase1ResourceFactor={aggregate.ResourceTrickleFactor:0.00}";
        }

        static string RunDivineRightFailurePhase()
        {
            using var world = CreateValidationWorld("DynastyPoliticalDivineRightFailurePhase");
            var entityManager = world.EntityManager;

            var faction = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(FaithStateComponent),
                typeof(DynastyPoliticalEventAggregateComponent));
            entityManager.SetComponentData(faction, new FactionComponent
            {
                FactionId = new FixedString32Bytes("player"),
            });
            entityManager.SetComponentData(faction, new FaithStateComponent
            {
                SelectedFaith = CovenantId.TheOrder,
                Intensity = PlayerDivineRightDeclarationSystem.DivineRightIntensityThreshold - 5f,
                Level = PlayerDivineRightDeclarationSystem.DivineRightLevelThreshold,
            });

            var operation = entityManager.CreateEntity(
                typeof(DynastyOperationComponent),
                typeof(DynastyOperationDivineRightComponent));
            entityManager.SetComponentData(operation, new DynastyOperationComponent
            {
                OperationId = new FixedString64Bytes("divine-right-op"),
                SourceFactionId = new FixedString32Bytes("player"),
                OperationKind = DynastyOperationKind.DivineRight,
                StartedAtInWorldDays = 0f,
                Active = true,
            });
            entityManager.SetComponentData(operation, new DynastyOperationDivineRightComponent
            {
                ResolveAtInWorldDays = 180f,
                SourceFaithId = new FixedString64Bytes("the_order"),
            });

            var clock = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clock, new DualClockComponent
            {
                InWorldDays = 12f,
                DaysPerRealSecond = 2f,
            });

            Tick(world, 12f);

            var operationState = entityManager.GetComponentData<DynastyOperationComponent>(operation);
            if (operationState.Active)
            {
                throw new System.InvalidOperationException(
                    "Divine-right failure phase did not deactivate the operation.");
            }

            var events = entityManager.GetBuffer<DynastyPoliticalEventComponent>(faction);
            bool foundCooldown = false;
            float expiry = 0f;
            for (int i = 0; i < events.Length; i++)
            {
                if (!events[i].EventType.Equals(DynastyPoliticalEventTypes.DivineRightFailedCooldown))
                {
                    continue;
                }

                foundCooldown = true;
                expiry = events[i].ExpiresAtInWorldDays;
                break;
            }

            if (!foundCooldown || expiry <= 12f)
            {
                throw new System.InvalidOperationException(
                    "Divine-right failure phase did not write the cooldown event.");
            }

            return $"phase2CooldownExpiry={expiry:0.0}";
        }

        static string RunExpiryPhase()
        {
            using var world = CreateValidationWorld("DynastyPoliticalExpiryPhase");
            var entityManager = world.EntityManager;

            var faction = entityManager.CreateEntity(
                typeof(FactionComponent),
                typeof(DynastyPoliticalEventAggregateComponent));
            entityManager.SetComponentData(faction, new FactionComponent
            {
                FactionId = new FixedString32Bytes("player"),
            });
            var events = entityManager.AddBuffer<DynastyPoliticalEventComponent>(faction);
            events.Add(new DynastyPoliticalEventComponent
            {
                EventType = DynastyPoliticalEventTypes.CovenantTestCooldown,
                ExpiresAtInWorldDays = 20f,
                ResourceTrickleFactor = 0.92f,
                AttackMultiplier = 0.95f,
                StabilizationMultiplier = 0.9f,
            });

            var clock = entityManager.CreateEntity(typeof(DualClockComponent));
            entityManager.SetComponentData(clock, new DualClockComponent
            {
                InWorldDays = 21f,
                DaysPerRealSecond = 2f,
            });

            Tick(world, 21f);

            var aggregate = entityManager.GetComponentData<DynastyPoliticalEventAggregateComponent>(faction);
            if (events.Length != 0 ||
                aggregate.ActiveEventCount != 0 ||
                Mathf.Abs(aggregate.ResourceTrickleFactor - 1f) > 0.0001f ||
                Mathf.Abs(aggregate.AttackMultiplier - 1f) > 0.0001f ||
                Mathf.Abs(aggregate.StabilizationMultiplier - 1f) > 0.0001f)
            {
                throw new System.InvalidOperationException(
                    "Dynasty political event expiry phase failed.");
            }

            return "phase3ExpiredCount=0";
        }

        static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<DynastyPoliticalEventSystem>());
            simulation.SortSystems();
            return world;
        }

        static void Tick(World world, float inWorldDays)
        {
            world.SetTime(new Unity.Core.TimeData(inWorldDays, 0.05f));
            world.Update();
        }
    }
}
