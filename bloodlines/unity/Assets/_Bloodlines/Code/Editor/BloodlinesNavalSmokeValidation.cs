#if UNITY_EDITOR
using System;
using Bloodlines.Components;
using Bloodlines.Naval;
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
    /// Naval-layer smoke validation. Runs in an isolated test world to prove
    /// the naval slice contracts hold without disturbing combat or bootstrap
    /// validation.
    ///
    /// S1 (embark) is the first phase. Future slices append phases:
    ///   S2 disembark, S3 fire-ship detonation, S4 vessel-vs-vessel combat,
    ///   S5 fishing gather, S6 AI naval dispatch.
    /// </summary>
    public static class BloodlinesNavalSmokeValidation
    {
        private const float StepSeconds = 0.1f;
        private const double TimeoutSeconds = 4d;

        [MenuItem("Bloodlines/Validation/Run Naval Smoke Validation")]
        public static void RunNavalSmokeValidation()
        {
            ExecuteValidation(exitOnComplete: false);
        }

        public static void RunBatchNavalSmokeValidation()
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
                message = "Naval smoke validation failed with an exception: " + exception;
                UnityDebug.LogError(message);
            }

            if (exitOnComplete)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunValidation(out string message)
        {
            if (!RunEmbarkPhase(out string embarkMessage))
            {
                message = embarkMessage;
                return false;
            }

            UnityDebug.Log(embarkMessage);

            message =
                "Naval smoke validation passed: embarkPhase=True.";
            return true;
        }

        private static bool RunEmbarkPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesNavalSmokeValidation_Embark");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player");
            CreateFaction(entityManager, "rival");

            Entity transport = CreateTransportVessel(
                entityManager,
                "player",
                "transport_ship",
                new float3(0f, 0f, 0f),
                transportCapacity: NavalCanon.DefaultTransportCapacity);

            Entity[] friendlyPassengers = new Entity[7];
            for (int i = 0; i < friendlyPassengers.Length; i++)
            {
                float angle = (math.PI2 / friendlyPassengers.Length) * i;
                float radius = 1.0f;
                friendlyPassengers[i] = CreateLandUnit(
                    entityManager,
                    "player",
                    "villager",
                    new float3(math.cos(angle) * radius, 0f, math.sin(angle) * radius));
            }

            Entity foreignCandidate = CreateLandUnit(
                entityManager,
                "rival",
                "villager",
                new float3(0.4f, 0f, 0.4f));

            Entity outOfRangeCandidate = CreateLandUnit(
                entityManager,
                "player",
                "villager",
                new float3(50f, 0f, 0f));

            for (int i = 0; i < friendlyPassengers.Length; i++)
            {
                IssueEmbarkOrder(entityManager, friendlyPassengers[i], transport);
            }
            IssueEmbarkOrder(entityManager, foreignCandidate, transport);
            IssueEmbarkOrder(entityManager, outOfRangeCandidate, transport);

            double elapsed = 0d;
            int embarkedCount = 0;
            while (elapsed < TimeoutSeconds)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;

                if (entityManager.HasBuffer<PassengerBufferElement>(transport))
                {
                    embarkedCount = entityManager.GetBuffer<PassengerBufferElement>(transport).Length;
                }

                bool ordersDrained = true;
                for (int i = 0; i < friendlyPassengers.Length; i++)
                {
                    if (entityManager.HasComponent<EmbarkOrderComponent>(friendlyPassengers[i]))
                    {
                        ordersDrained = false;
                        break;
                    }
                }

                if (ordersDrained &&
                    !entityManager.HasComponent<EmbarkOrderComponent>(foreignCandidate) &&
                    !entityManager.HasComponent<EmbarkOrderComponent>(outOfRangeCandidate))
                {
                    break;
                }
            }

            if (embarkedCount != NavalCanon.DefaultTransportCapacity)
            {
                message = "Naval smoke validation failed: embark phase expected capacity=" +
                    NavalCanon.DefaultTransportCapacity + " passengers, got " + embarkedCount + ".";
                return false;
            }

            int taggedCount = 0;
            int rejectedOverCapacity = 0;
            for (int i = 0; i < friendlyPassengers.Length; i++)
            {
                bool tagged = entityManager.HasComponent<EmbarkedPassengerTag>(friendlyPassengers[i]);
                if (tagged)
                {
                    taggedCount++;
                }
                else
                {
                    rejectedOverCapacity++;
                }
            }

            if (taggedCount != NavalCanon.DefaultTransportCapacity)
            {
                message = "Naval smoke validation failed: embark phase expected " +
                    NavalCanon.DefaultTransportCapacity + " tagged passengers, got " + taggedCount + ".";
                return false;
            }

            if (rejectedOverCapacity != 1)
            {
                message = "Naval smoke validation failed: embark phase expected 1 capacity-rejected passenger, got " +
                    rejectedOverCapacity + ".";
                return false;
            }

            if (entityManager.HasComponent<EmbarkedPassengerTag>(foreignCandidate))
            {
                message = "Naval smoke validation failed: embark phase admitted a cross-faction passenger.";
                return false;
            }

            if (entityManager.HasComponent<EmbarkedPassengerTag>(outOfRangeCandidate))
            {
                message = "Naval smoke validation failed: embark phase admitted an out-of-range passenger.";
                return false;
            }

            for (int i = 0; i < friendlyPassengers.Length; i++)
            {
                if (!entityManager.HasComponent<EmbarkedPassengerTag>(friendlyPassengers[i]))
                {
                    continue;
                }

                var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(friendlyPassengers[i]);
                if (moveCommand.IsActive)
                {
                    message = "Naval smoke validation failed: embark phase left an active move command on an embarked passenger.";
                    return false;
                }

                if (!entityManager.HasComponent<PassengerTransportLinkComponent>(friendlyPassengers[i]))
                {
                    message = "Naval smoke validation failed: embarked passenger missing PassengerTransportLinkComponent.";
                    return false;
                }

                var link = entityManager.GetComponentData<PassengerTransportLinkComponent>(friendlyPassengers[i]);
                if (link.Transport != transport)
                {
                    message = "Naval smoke validation failed: embarked passenger link points to wrong transport.";
                    return false;
                }
            }

            for (int i = 0; i < friendlyPassengers.Length; i++)
            {
                if (!entityManager.HasComponent<EmbarkedPassengerTag>(friendlyPassengers[i]))
                {
                    continue;
                }

                var moveCommand = entityManager.GetComponentData<MoveCommandComponent>(friendlyPassengers[i]);
                moveCommand.Destination = new float3(20f, 0f, 20f);
                moveCommand.IsActive = true;
                entityManager.SetComponentData(friendlyPassengers[i], moveCommand);
            }

            float3[] preTickPositions = new float3[friendlyPassengers.Length];
            for (int i = 0; i < friendlyPassengers.Length; i++)
            {
                preTickPositions[i] = entityManager.GetComponentData<PositionComponent>(friendlyPassengers[i]).Value;
            }

            for (int t = 0; t < 5; t++)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;
            }

            for (int i = 0; i < friendlyPassengers.Length; i++)
            {
                if (!entityManager.HasComponent<EmbarkedPassengerTag>(friendlyPassengers[i]))
                {
                    continue;
                }

                var post = entityManager.GetComponentData<PositionComponent>(friendlyPassengers[i]).Value;
                float drift = math.length(post - preTickPositions[i]);
                if (drift > 0.01f)
                {
                    message = "Naval smoke validation failed: embarked passenger drifted " +
                        drift.ToString("0.###") + " world units while tagged inactive.";
                    return false;
                }
            }

            message =
                "Naval smoke validation embark phase passed: passengerCount=" + embarkedCount +
                ", embarkRejectedOverCapacity=" + rejectedOverCapacity +
                ", embarkRejectedForeign=1" +
                ", embarkRejectedOutOfRange=1" +
                ", embarkedMovementSuppressed=True" +
                ", elapsedSeconds=" + elapsed.ToString("0.###") + ".";
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
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<EmbarkSystem>());
            simulationGroup.SortSystems();
            return world;
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

        private static Entity CreateTransportVessel(
            EntityManager entityManager,
            string factionId,
            string typeId,
            float3 position,
            int transportCapacity)
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
                Current = 280f,
                Max = 280f,
            });
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = typeId,
                Role = UnitRole.Vessel,
                SiegeClass = SiegeClass.None,
                PopulationCost = 2,
                Stage = 2,
            });
            entityManager.AddComponentData(entity, new MovementStatsComponent { MaxSpeed = 4.5f });
            entityManager.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.2f,
                IsActive = false,
            });
            entityManager.AddComponentData(entity, new NavalVesselComponent
            {
                Class = VesselClass.Transport,
                TransportCapacity = transportCapacity,
                OneUseSacrifice = false,
            });
            entityManager.AddBuffer<PassengerBufferElement>(entity);
            return entity;
        }

        private static Entity CreateLandUnit(
            EntityManager entityManager,
            string factionId,
            string typeId,
            float3 position)
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
                Current = 12f,
                Max = 12f,
            });
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = typeId,
                Role = UnitRole.Worker,
                SiegeClass = SiegeClass.None,
                PopulationCost = 1,
                Stage = 1,
            });
            entityManager.AddComponentData(entity, new MovementStatsComponent { MaxSpeed = 4.5f });
            entityManager.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.2f,
                IsActive = false,
            });
            return entity;
        }

        private static void IssueEmbarkOrder(EntityManager entityManager, Entity passenger, Entity transport)
        {
            if (entityManager.HasComponent<EmbarkOrderComponent>(passenger))
            {
                entityManager.SetComponentData(passenger, new EmbarkOrderComponent
                {
                    TargetTransport = transport,
                });
            }
            else
            {
                entityManager.AddComponentData(passenger, new EmbarkOrderComponent
                {
                    TargetTransport = transport,
                });
            }
        }
    }
}
#endif
