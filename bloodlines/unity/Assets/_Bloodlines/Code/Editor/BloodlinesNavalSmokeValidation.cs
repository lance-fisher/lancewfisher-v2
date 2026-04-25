#if UNITY_EDITOR
using System;
using Bloodlines.Components;
using Bloodlines.Naval;
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

            if (!RunDisembarkPhase(out string disembarkMessage))
            {
                message = disembarkMessage;
                return false;
            }
            UnityDebug.Log(disembarkMessage);

            if (!RunFireShipDetonationPhase(out string fireShipMessage))
            {
                message = fireShipMessage;
                return false;
            }
            UnityDebug.Log(fireShipMessage);

            if (!RunFishingGatherPhase(out string fishingMessage))
            {
                message = fishingMessage;
                return false;
            }
            UnityDebug.Log(fishingMessage);

            if (!RunVesselCombatPhase(out string vesselCombatMessage))
            {
                message = vesselCombatMessage;
                return false;
            }
            UnityDebug.Log(vesselCombatMessage);

            message =
                "Naval smoke validation passed: embarkPhase=True, disembarkPhase=True, fireShipPhase=True, fishingPhase=True, vesselCombatPhase=True.";
            return true;
        }

        private static bool RunVesselCombatPhase(out string message)
        {
            // S4: vessel-vs-vessel naval combat. Browser parity: simulation.js
            // updateVessel attack branch (~8814-8863). The browser allows any
            // unit with attack > 0 to engage hostile units in attackRange*1.2.
            // Unity inherits this via the existing AutoAcquireTargetSystem +
            // AttackResolutionSystem pipeline, which reads CombatStatsComponent
            // without filtering by domain. This phase proves a war-galley vs
            // war-galley engagement resolves through the standard pipeline,
            // does NOT spuriously trigger fire-ship detonation, and produces
            // damage on the target.
            using var world = new World("BloodlinesNavalSmokeValidation_VesselCombat");
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AutoAcquireTargetSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AttackResolutionSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ProjectileMovementSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ProjectileImpactSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DeathResolutionSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FireShipDetonationSystem>());
            simGroup.SortSystems();
            var em = world.EntityManager;

            CreateFaction(em, "player", "rival");
            CreateFaction(em, "rival", "player");

            // Player war galley (ranged combatant) at origin.
            Entity playerGalley = CreateWarGalley(em, "player", new float3(0f, 0f, 0f));

            // Enemy war galley within attack range (130) and sight (180).
            Entity enemyGalley = CreateWarGalley(em, "rival", new float3(0f, 0f, 50f));

            float startEnemyHealth = em.GetComponentData<HealthComponent>(enemyGalley).Current;

            // Tick long enough for AutoAcquireTargetSystem to lock the target
            // and for at least one attack-cooldown cycle (1.3s) plus projectile
            // travel (50/240 = ~0.21s) to land a strike.
            const float DtSeconds = 0.2f;
            const int MaxTicks = 30;
            int impactTicks = 0;
            for (int t = 0; t < MaxTicks; t++)
            {
                world.SetTime(new TimeData(t * DtSeconds, DtSeconds));
                world.Update();
                impactTicks = t + 1;
                if (em.HasComponent<DeadTag>(enemyGalley)) break;
                var hp = em.GetComponentData<HealthComponent>(enemyGalley);
                if (hp.Current < startEnemyHealth) break;
            }

            float endEnemyHealth = em.GetComponentData<HealthComponent>(enemyGalley).Current;
            if (endEnemyHealth >= startEnemyHealth)
            {
                message = "Naval smoke validation failed: vessel combat phase expected enemy galley to take damage; got " +
                    endEnemyHealth + "/" + startEnemyHealth + " after " + impactTicks + " ticks.";
                return false;
            }

            // Negative case: the player war galley should NOT have queued a
            // FireShipDetonationPendingTag because OneUseSacrifice=false.
            if (em.HasComponent<FireShipDetonationPendingTag>(playerGalley))
            {
                message = "Naval smoke validation failed: vessel combat phase queued a fire-ship detonation tag on a non-sacrifice vessel.";
                return false;
            }

            // The player galley itself should still be alive (enemy didn't have
            // a chance to retaliate through projectile travel time yet, and
            // even if it did, both should still be standing).
            if (em.HasComponent<DeadTag>(playerGalley))
            {
                var playerHp = em.GetComponentData<HealthComponent>(playerGalley);
                message = "Naval smoke validation failed: vessel combat phase killed the player galley unexpectedly. health=" + playerHp.Current;
                return false;
            }

            message =
                "Naval smoke validation vessel combat phase passed: enemyHealthDelta=" +
                (startEnemyHealth - endEnemyHealth).ToString("0.##") +
                " ticksToImpact=" + impactTicks +
                " fireShipFalsePositive=False" +
                " playerGalleyAlive=True.";
            return true;
        }

        private static Entity CreateWarGalley(EntityManager em, string factionId, float3 position)
        {
            var entity = em.CreateEntity();
            em.AddComponentData(entity, new FactionComponent { FactionId = factionId });
            em.AddComponentData(entity, new PositionComponent { Value = position });
            em.AddComponentData(entity, new LocalTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
            em.AddComponentData(entity, new HealthComponent
            {
                Current = 360f,
                Max = 360f,
            });
            em.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = "war_galley",
                Role = UnitRole.Vessel,
                SiegeClass = SiegeClass.None,
                PopulationCost = 2,
                Stage = 2,
            });
            em.AddComponentData(entity, new MovementStatsComponent { MaxSpeed = 60f });
            em.AddComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = 22f,
                AttackRange = 130f,
                AttackCooldown = 1.3f,
                Sight = 180f,
                CooldownRemaining = 0f,
                AcquireCooldownRemaining = 0f,
            });
            em.AddComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.2f,
                IsActive = false,
            });
            em.AddComponentData(entity, new RecentImpactComponent());
            em.AddComponentData(entity, new NavalVesselComponent
            {
                Class = VesselClass.WarGalley,
                TransportCapacity = 0,
                OneUseSacrifice = false,
            });
            em.AddComponentData(entity, new ProjectileFactoryComponent
            {
                ProjectileSpeed = 240f,
                ProjectileMaxLifetimeSeconds = 4f,
                ProjectileArrivalRadius = 0.2f,
            });
            return entity;
        }

        private static bool RunFishingGatherPhase(out string message)
        {
            using var world = new World("BloodlinesNavalSmokeValidation_Fishing");
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<Bloodlines.Naval.FishingGatherSystem>());
            simGroup.SortSystems();
            var em = world.EntityManager;

            // Player faction with stockpile.
            var faction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(ResourceStockpileComponent));
            em.SetComponentData(faction, new FactionComponent { FactionId = "player" });
            em.SetComponentData(faction, new ResourceStockpileComponent { Food = 100f });

            // Bootstrap singleton with one water patch covering tile (10,10..14,14).
            CreateMapBootstrapEntity(em, 1, (10, 10, 5, 5));

            // Idle fishing vessel ON water tile.
            var fishingBoat = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(NavalVesselComponent),
                typeof(FishingVesselComponent),
                typeof(MoveCommandComponent));
            em.SetComponentData(fishingBoat, new FactionComponent { FactionId = "player" });
            em.SetComponentData(fishingBoat, new PositionComponent { Value = new float3(10.5f, 0f, 10.5f) });
            em.SetComponentData(fishingBoat, new NavalVesselComponent
            {
                Class = VesselClass.Fishing,
                TransportCapacity = 0,
                OneUseSacrifice = false,
            });
            em.SetComponentData(fishingBoat, new FishingVesselComponent { FoodPerSecond = 1.2f });
            em.SetComponentData(fishingBoat, new MoveCommandComponent
            {
                Destination = new float3(10.5f, 0f, 10.5f),
                IsActive = false,
            });

            // Active-move fishing vessel on water (should NOT fish).
            var movingBoat = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(NavalVesselComponent),
                typeof(FishingVesselComponent),
                typeof(MoveCommandComponent));
            em.SetComponentData(movingBoat, new FactionComponent { FactionId = "player" });
            em.SetComponentData(movingBoat, new PositionComponent { Value = new float3(11.5f, 0f, 11.5f) });
            em.SetComponentData(movingBoat, new NavalVesselComponent { Class = VesselClass.Fishing });
            em.SetComponentData(movingBoat, new FishingVesselComponent { FoodPerSecond = 1.2f });
            em.SetComponentData(movingBoat, new MoveCommandComponent { IsActive = true });

            // Idle fishing vessel OFF water (should NOT fish).
            var landBoat = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(NavalVesselComponent),
                typeof(FishingVesselComponent),
                typeof(MoveCommandComponent));
            em.SetComponentData(landBoat, new FactionComponent { FactionId = "player" });
            em.SetComponentData(landBoat, new PositionComponent { Value = new float3(0.5f, 0f, 0.5f) });
            em.SetComponentData(landBoat, new NavalVesselComponent { Class = VesselClass.Fishing });
            em.SetComponentData(landBoat, new FishingVesselComponent { FoodPerSecond = 1.2f });
            em.SetComponentData(landBoat, new MoveCommandComponent { IsActive = false });

            // Tick 10 times at 1-second dt.
            const int Ticks = 10;
            const float DtSeconds = 1f;
            for (int t = 0; t < Ticks; t++)
            {
                world.SetTime(new TimeData(t * DtSeconds, DtSeconds));
                world.Update();
            }

            float foodAfter = em.GetComponentData<ResourceStockpileComponent>(faction).Food;
            // Expected: only the idle-on-water boat fishes. 1.2/sec * 10s = 12 food.
            // Tolerance accounts for any tick-boundary drift.
            if (foodAfter < 100f + 11.5f || foodAfter > 100f + 12.5f)
            {
                message = "Naval smoke validation failed: fishing phase expected ~12 food gain, got " +
                    (foodAfter - 100f).ToString("0.##") + ".";
                return false;
            }

            message =
                "Naval smoke validation fishing phase passed: idleOnWaterFood=" +
                (foodAfter - 100f).ToString("0.##") +
                " activeMoveFood=0 landFood=0 expectedRange=11.5..12.5.";
            return true;
        }

        private static bool RunFireShipDetonationPhase(out string message)
        {
            using var world = new World("BloodlinesNavalSmokeValidation_FireShip");
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>());
            simGroup.AddSystemToUpdateList(world.GetOrCreateSystem<Bloodlines.Naval.FireShipDetonationSystem>());
            simGroup.SortSystems();
            var em = world.EntityManager;

            // Fire-ship vessel with OneUseSacrifice=true and full health.
            var fireShip = em.CreateEntity(
                typeof(NavalVesselComponent),
                typeof(HealthComponent));
            em.SetComponentData(fireShip, new NavalVesselComponent
            {
                Class = VesselClass.FireShip,
                TransportCapacity = 0,
                OneUseSacrifice = true,
            });
            em.SetComponentData(fireShip, new HealthComponent { Current = 80f, Max = 80f });

            // Simulate the AttackResolutionSystem path: queue a pending detonation tag.
            em.AddComponent<FireShipDetonationPendingTag>(fireShip);

            // Tick until the tag is consumed (single tick should suffice since the
            // detonation system runs in SimulationSystemGroup).
            for (int t = 0; t < 4 && em.HasComponent<FireShipDetonationPendingTag>(fireShip); t++)
            {
                world.SetTime(new TimeData(t * StepSeconds, StepSeconds));
                world.Update();
            }

            if (em.HasComponent<FireShipDetonationPendingTag>(fireShip))
            {
                message = "Naval smoke validation failed: fire-ship detonation tag not consumed.";
                return false;
            }
            var health = em.GetComponentData<HealthComponent>(fireShip);
            if (health.Current > 0f)
            {
                message = "Naval smoke validation failed: fire-ship detonation expected health=0, got " +
                    health.Current + ".";
                return false;
            }
            if (!em.HasComponent<DeadTag>(fireShip))
            {
                message = "Naval smoke validation failed: fire-ship detonation expected DeadTag but missing.";
                return false;
            }

            // Negative case: a non-sacrifice vessel with the pending tag should NOT
            // be destroyed. The detonation system silently consumes the tag.
            var warGalley = em.CreateEntity(
                typeof(NavalVesselComponent),
                typeof(HealthComponent));
            em.SetComponentData(warGalley, new NavalVesselComponent
            {
                Class = VesselClass.WarGalley,
                TransportCapacity = 0,
                OneUseSacrifice = false,
            });
            em.SetComponentData(warGalley, new HealthComponent { Current = 200f, Max = 200f });
            em.AddComponent<FireShipDetonationPendingTag>(warGalley);

            for (int t = 0; t < 4 && em.HasComponent<FireShipDetonationPendingTag>(warGalley); t++)
            {
                world.SetTime(new TimeData(10d + t * StepSeconds, StepSeconds));
                world.Update();
            }
            if (em.HasComponent<FireShipDetonationPendingTag>(warGalley))
            {
                message = "Naval smoke validation failed: fire-ship detonation negative tag not consumed.";
                return false;
            }
            var galleyHealth = em.GetComponentData<HealthComponent>(warGalley);
            if (galleyHealth.Current <= 0f)
            {
                message = "Naval smoke validation failed: non-sacrifice vessel destroyed by fire-ship system.";
                return false;
            }
            if (em.HasComponent<DeadTag>(warGalley))
            {
                message = "Naval smoke validation failed: non-sacrifice vessel marked dead by fire-ship system.";
                return false;
            }

            message =
                "Naval smoke validation fire-ship phase passed: detonatedHealth=0 detonatedDeadTag=True nonSacrificeUntouched=True.";
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

        private static World CreateDisembarkValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();

            var endSimulation = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            simulationGroup.AddSystemToUpdateList(endSimulation);
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DisembarkSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static Entity CreateMapBootstrapEntity(
            EntityManager entityManager,
            int tileSize,
            params (int x, int y, int w, int h)[] waterPatches)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new MapBootstrapConfigComponent
            {
                TileSize = tileSize,
                Width = 100,
                Height = 100,
            });
            var buf = entityManager.AddBuffer<MapWaterTilePatchSeedElement>(entity);
            for (int i = 0; i < waterPatches.Length; i++)
            {
                var p = waterPatches[i];
                buf.Add(new MapWaterTilePatchSeedElement { X = p.x, Y = p.y, Width = p.w, Height = p.h });
            }
            return entity;
        }

        private static bool RunDisembarkPhase(out string message)
        {
            using var world = CreateDisembarkValidationWorld("BloodlinesNavalSmokeValidation_Disembark");
            var entityManager = world.EntityManager;

            CreateFaction(entityManager, "player");

            // Tile size = 1, water patch covers (10,10) to (11,11) inclusive.
            // Transport tile (10,10) -> NW neighbor (9,9) is land.
            CreateMapBootstrapEntity(entityManager, 1, (10, 10, 2, 2));

            // Transport at (10.5, 0, 10.5) -> floor(10.5/1) = 10 in both axes.
            Entity transport = CreateTransportVessel(
                entityManager, "player", "transport_ship",
                new float3(10.5f, 0f, 10.5f),
                transportCapacity: NavalCanon.DefaultTransportCapacity);

            // Pre-seed 6 embarked passengers. Create the entities and add their
            // tag/link components first; only then re-fetch the transport buffer
            // and append, since AddComponentData invalidates buffer handles.
            Entity[] passengers = new Entity[6];
            for (int i = 0; i < 6; i++)
            {
                passengers[i] = CreateLandUnit(entityManager, "player", "villager", new float3(10.5f, 0f, 10.5f));
                entityManager.AddComponentData(passengers[i], new EmbarkedPassengerTag());
                entityManager.AddComponentData(passengers[i], new PassengerTransportLinkComponent { Transport = transport });
            }
            {
                var passengerBuffer = entityManager.GetBuffer<PassengerBufferElement>(transport);
                for (int i = 0; i < 6; i++)
                {
                    passengerBuffer.Add(new PassengerBufferElement
                    {
                        Passenger = passengers[i],
                        PassengerTypeId = new FixedString64Bytes("villager"),
                    });
                }
            }

            // Issue disembark order on transport.
            entityManager.AddComponentData(transport, new DisembarkOrderComponent());

            double elapsed = 0d;
            for (int t = 0; t < 4 && entityManager.HasComponent<DisembarkOrderComponent>(transport); t++)
            {
                world.SetTime(new TimeData(elapsed, StepSeconds));
                world.Update();
                elapsed += StepSeconds;
            }

            if (entityManager.HasComponent<DisembarkOrderComponent>(transport))
            {
                message = "Naval smoke validation failed: disembark phase did not consume the order.";
                return false;
            }

            int remaining = entityManager.GetBuffer<PassengerBufferElement>(transport).Length;
            if (remaining != 0)
            {
                message = "Naval smoke validation failed: disembark phase left " + remaining + " passengers on transport.";
                return false;
            }

            // First non-water tile is (9,9). Land world center: (9.5, 0, 9.5).
            // Passengers placed in 3x3 offset grid (10-unit spacing).
            for (int i = 0; i < passengers.Length; i++)
            {
                if (entityManager.HasComponent<EmbarkedPassengerTag>(passengers[i]))
                {
                    message = "Naval smoke validation failed: disembarked passenger still has EmbarkedPassengerTag.";
                    return false;
                }
                if (entityManager.HasComponent<PassengerTransportLinkComponent>(passengers[i]))
                {
                    message = "Naval smoke validation failed: disembarked passenger still has PassengerTransportLinkComponent.";
                    return false;
                }

                float expectedX = 9.5f + ((i % 3) - 1) * 10f;
                float expectedZ = 9.5f + (math.floor(i / 3f) - 1f) * 10f;
                var pos = entityManager.GetComponentData<PositionComponent>(passengers[i]).Value;
                if (math.abs(pos.x - expectedX) > 0.01f || math.abs(pos.z - expectedZ) > 0.01f)
                {
                    message = "Naval smoke validation failed: passenger " + i + " expected (" +
                        expectedX + ", " + expectedZ + ") got (" + pos.x + ", " + pos.z + ").";
                    return false;
                }

                var move = entityManager.GetComponentData<MoveCommandComponent>(passengers[i]);
                if (move.IsActive)
                {
                    message = "Naval smoke validation failed: disembarked passenger has active MoveCommand.";
                    return false;
                }
            }

            // Failure case: order on a transport with all-water surroundings.
            using var failWorld = CreateDisembarkValidationWorld("BloodlinesNavalSmokeValidation_DisembarkFail");
            var failEm = failWorld.EntityManager;
            CreateFaction(failEm, "player");
            // Water patch covers (5,5) to (24,24); transport at tile (10,10) is fully surrounded.
            CreateMapBootstrapEntity(failEm, 1, (5, 5, 20, 20));
            Entity failTransport = CreateTransportVessel(
                failEm, "player", "transport_ship",
                new float3(10.5f, 0f, 10.5f), transportCapacity: NavalCanon.DefaultTransportCapacity);
            Entity failPassenger = CreateLandUnit(failEm, "player", "villager", new float3(10.5f, 0f, 10.5f));
            failEm.AddComponentData(failPassenger, new EmbarkedPassengerTag());
            failEm.AddComponentData(failPassenger, new PassengerTransportLinkComponent { Transport = failTransport });
            {
                var failPassengerBuffer = failEm.GetBuffer<PassengerBufferElement>(failTransport);
                failPassengerBuffer.Add(new PassengerBufferElement
                {
                    Passenger = failPassenger,
                    PassengerTypeId = new FixedString64Bytes("villager"),
                });
            }
            failEm.AddComponentData(failTransport, new DisembarkOrderComponent());

            double failElapsed = 0d;
            for (int t = 0; t < 4 && failEm.HasComponent<DisembarkOrderComponent>(failTransport); t++)
            {
                failWorld.SetTime(new TimeData(failElapsed, StepSeconds));
                failWorld.Update();
                failElapsed += StepSeconds;
            }

            if (failEm.HasComponent<DisembarkOrderComponent>(failTransport))
            {
                message = "Naval smoke validation failed: disembark fail-case did not drop the order.";
                return false;
            }
            if (failEm.GetBuffer<PassengerBufferElement>(failTransport).Length != 1)
            {
                message = "Naval smoke validation failed: disembark fail-case unexpectedly drained passenger buffer.";
                return false;
            }
            if (!failEm.HasComponent<EmbarkedPassengerTag>(failPassenger))
            {
                message = "Naval smoke validation failed: disembark fail-case removed EmbarkedPassengerTag without dropping passenger.";
                return false;
            }

            message =
                "Naval smoke validation disembark phase passed: dropTileX=9 dropTileY=9 droppedCount=6 failCaseRetained=True.";
            return true;
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
