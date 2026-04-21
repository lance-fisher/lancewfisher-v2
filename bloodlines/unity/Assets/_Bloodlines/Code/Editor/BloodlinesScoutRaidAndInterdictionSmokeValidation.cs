#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Pathing;
using Bloodlines.Raids;
using Bloodlines.Siege;
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
    /// Smoke validator for the scout raid and logistics interdiction slice.
    ///
    /// Phase 1: farm raid drains resources, suppresses trickle, shocks loyalty, and retreats.
    /// Phase 2: raided well stops field-water refresh until the raid timer expires.
    /// Phase 3: returning workers reroute around a raided drop-off building.
    /// Phase 4: supply wagon interdiction applies timers, stockpile loss, and retreat orders.
    /// </summary>
    public static class BloodlinesScoutRaidAndInterdictionSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-scout-raid-and-interdiction-smoke.log";
        private const float StepSeconds = 1f;

        [MenuItem("Bloodlines/Raids/Run Scout Raid And Interdiction Smoke Validation")]
        public static void RunInteractive() => RunInternal(batchMode: false);

        public static void RunBatchScoutRaidAndInterdictionSmokeValidation() => RunInternal(batchMode: true);

        private static void RunInternal(bool batchMode)
        {
            string message;
            bool success;
            try
            {
                success = RunAllPhases(out message);
            }
            catch (Exception e)
            {
                success = false;
                message = "Scout raid and interdiction smoke errored: " + e;
            }

            string artifact = "BLOODLINES_SCOUT_RAID_AND_INTERDICTION_SMOKE " + (success ? "PASS" : "FAIL") + "\n" + message;
            UnityDebug.Log(artifact);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ArtifactPath)!);
                File.WriteAllText(ArtifactPath, artifact);
            }
            catch (Exception)
            {
            }

            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private static bool RunAllPhases(out string report)
        {
            var sb = new System.Text.StringBuilder();
            bool ok = true;
            ok &= RunPhaseFarmRaid(sb);
            ok &= RunPhaseWellRaidBlocksFieldWater(sb);
            ok &= RunPhaseDropoffReroute(sb);
            ok &= RunPhaseSupplyWagonInterdiction(sb);
            report = sb.ToString();
            return ok;
        }

        private static SimulationSystemGroup SetupSimulation(World world)
        {
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulation = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            var endSimulation = world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulation.AddSystemToUpdateList(endSimulation);
            return simulation;
        }

        private static void AddRaidInteractionSystems(World world, SimulationSystemGroup simulation)
        {
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<UnitMovementSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<ScoutRaidResolutionSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<WorkerGatherSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<ResourceTrickleBuildingSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<SiegeSupportRefreshSystem>());
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<FieldWaterSupportScanSystem>());
            simulation.SortSystems();
        }

        private static void Advance(World world, double elapsed)
        {
            world.SetTime(new TimeData(elapsed, StepSeconds));
            world.Update();
        }

        private static bool RunPhaseFarmRaid(System.Text.StringBuilder sb)
        {
            using var world = new World("scout-raid-phase1");
            var em = world.EntityManager;
            var simulation = SetupSimulation(world);
            AddRaidInteractionSystems(world, simulation);

            SeedMapConfig(em);
            SeedFaction(em, "player", food: 50f, influence: 20f);
            SeedFaction(em, "enemy");
            SeedMutualHostility(em, "enemy", "player");

            var controlPoint = CreateControlPoint(em, "player", new float3(118f, 0f, 100f), 80f);
            var farm = CreateRaidableBuilding(em, "farm", "player", new float3(110f, 0f, 100f));
            em.AddComponentData(farm, new ResourceTrickleBuildingComponent
            {
                FoodPerSecond = 0.5f,
            });

            var scout = CreateScoutRaider(em, "enemy", new float3(96f, 0f, 100f));
            em.AddComponentData(scout, new ScoutRaidCommandComponent
            {
                TargetEntity = farm,
                TargetKind = ScoutRaidTargetKind.Building,
            });

            Advance(world, 1d);

            var playerResources = GetFactionStockpile(em, "player");
            var raidState = em.GetComponentData<BuildingRaidStateComponent>(farm);
            var loyalty = em.GetComponentData<ControlPointComponent>(controlPoint);
            var retreat = em.GetComponentData<MoveCommandComponent>(scout);

            if (!ScoutRaidCanon.IsBuildingRaided(raidState, 1d) ||
                math.abs(playerResources.Food - 38f) > 0.01f ||
                math.abs(playerResources.Influence - 18f) > 0.01f ||
                math.abs(loyalty.Loyalty - 72f) > 0.01f ||
                loyalty.ControlState != ControlState.Stabilized ||
                !retreat.IsActive ||
                retreat.Destination.x >= 96f)
            {
                sb.AppendLine(
                    "PhaseFarmRaid FAIL: farm raid did not apply canonical losses/suppression. " +
                    $"food={playerResources.Food:F2} influence={playerResources.Influence:F2} " +
                    $"loyalty={loyalty.Loyalty:F2} controlState={loyalty.ControlState} " +
                    $"raidedUntil={raidState.RaidedUntil:F2} retreatActive={retreat.IsActive} retreatX={retreat.Destination.x:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseFarmRaid PASS: food={playerResources.Food:F2} influence={playerResources.Influence:F2} " +
                $"loyalty={loyalty.Loyalty:F2} raidedUntil={raidState.RaidedUntil:F2} retreatX={retreat.Destination.x:F2}.");
            return true;
        }

        private static bool RunPhaseWellRaidBlocksFieldWater(System.Text.StringBuilder sb)
        {
            using var world = new World("scout-raid-phase2");
            var em = world.EntityManager;
            var simulation = SetupSimulation(world);
            AddRaidInteractionSystems(world, simulation);

            SeedMapConfig(em);
            SeedFaction(em, "player", water: 20f);
            SeedFaction(em, "enemy");
            SeedMutualHostility(em, "enemy", "player");

            var well = CreateRaidableBuilding(em, "well", "player", new float3(112f, 0f, 100f));
            var soldier = CreateFieldWaterUnit(em, "player", "militia", UnitRole.Melee, new float3(120f, 0f, 100f));
            var scout = CreateScoutRaider(em, "enemy", new float3(96f, 0f, 100f));
            em.AddComponentData(scout, new ScoutRaidCommandComponent
            {
                TargetEntity = well,
                TargetKind = ScoutRaidTargetKind.Building,
            });

            Advance(world, 1d);

            var fieldWater = em.GetComponentData<FieldWaterComponent>(soldier);
            if (fieldWater.SupportRefreshCount != 0 ||
                fieldWater.SuppliedUntil > 1d ||
                em.HasComponent<ScoutRaidCommandComponent>(scout))
            {
                sb.AppendLine(
                    "PhaseWellRaidBlocksFieldWater FAIL: raided well should not refresh field water immediately. " +
                    $"refreshCount={fieldWater.SupportRefreshCount} suppliedUntil={fieldWater.SuppliedUntil:F2} " +
                    $"raidOrderStillPresent={em.HasComponent<ScoutRaidCommandComponent>(scout)}");
                return false;
            }

            Advance(world, 26d);

            fieldWater = em.GetComponentData<FieldWaterComponent>(soldier);
            var playerResources = GetFactionStockpile(em, "player");
            var raidState = em.GetComponentData<BuildingRaidStateComponent>(well);
            if (fieldWater.SupportRefreshCount <= 0 ||
                fieldWater.SuppliedUntil <= 26d ||
                playerResources.Water >= 8f ||
                ScoutRaidCanon.IsBuildingRaided(raidState, 26d))
            {
                sb.AppendLine(
                    "PhaseWellRaidBlocksFieldWater FAIL: well support did not resume after raid expiry. " +
                    $"refreshCount={fieldWater.SupportRefreshCount} suppliedUntil={fieldWater.SuppliedUntil:F2} " +
                    $"water={playerResources.Water:F2} raidedUntil={raidState.RaidedUntil:F2}");
                return false;
            }

            sb.AppendLine(
                $"PhaseWellRaidBlocksFieldWater PASS: support resumed after expiry; refreshCount={fieldWater.SupportRefreshCount} " +
                $"suppliedUntil={fieldWater.SuppliedUntil:F2} water={playerResources.Water:F2}.");
            return true;
        }

        private static bool RunPhaseDropoffReroute(System.Text.StringBuilder sb)
        {
            using var world = new World("scout-raid-phase3");
            var em = world.EntityManager;
            var simulation = SetupSimulation(world);
            simulation.AddSystemToUpdateList(world.GetOrCreateSystem<WorkerGatherSystem>());
            simulation.SortSystems();

            SeedFaction(em, "player");

            var lumberCamp = CreateRaidableBuilding(em, "lumber_camp", "player", new float3(12f, 0f, 0f));
            var lumberRaidState = em.GetComponentData<BuildingRaidStateComponent>(lumberCamp);
            lumberRaidState.RaidedUntil = 12d;
            lumberRaidState.RaidedByFactionId = new FixedString32Bytes("enemy");
            em.SetComponentData(lumberCamp, lumberRaidState);

            var hall = CreateRaidableBuilding(em, "command_hall", "player", new float3(42f, 0f, 0f));
            var worker = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(LocalTransform),
                typeof(UnitTypeComponent),
                typeof(MovementStatsComponent),
                typeof(MoveCommandComponent),
                typeof(WorkerGatherComponent));
            em.SetComponentData(worker, new FactionComponent { FactionId = new FixedString32Bytes("player") });
            em.SetComponentData(worker, new PositionComponent { Value = new float3(0f, 0f, 0f) });
            em.SetComponentData(worker, LocalTransform.FromPosition(new float3(0f, 0f, 0f)));
            em.SetComponentData(worker, new UnitTypeComponent { TypeId = new FixedString64Bytes("villager"), Role = UnitRole.Worker });
            em.SetComponentData(worker, new MovementStatsComponent { MaxSpeed = 48f });
            em.SetComponentData(worker, new MoveCommandComponent { Destination = float3.zero, StoppingDistance = 0.2f, IsActive = false });
            em.SetComponentData(worker, new WorkerGatherComponent
            {
                CarryResourceId = new FixedString32Bytes("wood"),
                CarryAmount = 10f,
                CarryCapacity = 10f,
                GatherRate = 1f,
                Phase = WorkerGatherPhase.Returning,
                GatherRadius = 1.25f,
                DepositRadius = 1.25f,
            });

            Advance(world, 1d);

            var move = em.GetComponentData<MoveCommandComponent>(worker);
            if (!move.IsActive || math.distance(move.Destination.xz, new float2(42f, 0f)) > 0.1f)
            {
                sb.AppendLine(
                    "PhaseDropoffReroute FAIL: returning worker did not reroute around raided lumber camp. " +
                    $"moveActive={move.IsActive} destination=({move.Destination.x:F2},{move.Destination.z:F2})");
                return false;
            }

            sb.AppendLine(
                $"PhaseDropoffReroute PASS: worker routed to clear hall at ({move.Destination.x:F2},{move.Destination.z:F2}).");
            return true;
        }

        private static bool RunPhaseSupplyWagonInterdiction(System.Text.StringBuilder sb)
        {
            using var world = new World("scout-raid-phase4");
            var em = world.EntityManager;
            var simulation = SetupSimulation(world);
            AddRaidInteractionSystems(world, simulation);

            SeedMapConfig(em);
            SeedFaction(em, "player", food: 30f, water: 30f, wood: 20f);
            SeedFaction(em, "enemy");
            SeedMutualHostility(em, "enemy", "player");

            var camp = CreateSupplyCamp(em, "player", new float3(160f, 0f, 100f));
            var wagon = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(LocalTransform),
                typeof(HealthComponent),
                typeof(UnitTypeComponent),
                typeof(MoveCommandComponent),
                typeof(SiegeSupportComponent),
                typeof(SiegeSupplyTrainComponent));
            em.SetComponentData(wagon, new FactionComponent { FactionId = new FixedString32Bytes("player") });
            em.SetComponentData(wagon, new PositionComponent { Value = new float3(126f, 0f, 100f) });
            em.SetComponentData(wagon, LocalTransform.FromPosition(new float3(126f, 0f, 100f)));
            em.SetComponentData(wagon, new HealthComponent { Current = 240f, Max = 240f });
            em.SetComponentData(wagon, new UnitTypeComponent { TypeId = new FixedString64Bytes("supply_wagon"), Role = UnitRole.Support });
            em.SetComponentData(wagon, new MoveCommandComponent { Destination = new float3(126f, 0f, 100f), StoppingDistance = 0.2f, IsActive = false });
            em.SetComponentData(wagon, new SiegeSupportComponent
            {
                IsSupplyWagon = true,
                HasLinkedSupplyCamp = true,
                HasSupplyTrainSupport = true,
                Status = SiegeSupportStatus.Supporting,
            });
            em.SetComponentData(wagon, new SiegeSupplyTrainComponent
            {
                LinkedCampEntity = camp,
                LogisticsInterdictedUntil = 0d,
                ConvoyRecoveryUntil = 0d,
                RequiredEscortCount = 1,
                EscortScreened = true,
            });

            var scout = CreateScoutRaider(em, "enemy", new float3(104f, 0f, 100f));
            em.AddComponentData(scout, new ScoutRaidCommandComponent
            {
                TargetEntity = wagon,
                TargetKind = ScoutRaidTargetKind.LogisticsCarrier,
            });

            Advance(world, 1d);

            var stockpile = GetFactionStockpile(em, "player");
            var train = em.GetComponentData<SiegeSupplyTrainComponent>(wagon);
            var support = em.GetComponentData<SiegeSupportComponent>(wagon);
            var wagonMove = em.GetComponentData<MoveCommandComponent>(wagon);
            var scoutMove = em.GetComponentData<MoveCommandComponent>(scout);

            if (math.abs(stockpile.Food - 20f) > 0.01f ||
                math.abs(stockpile.Water - 20f) > 0.01f ||
                math.abs(stockpile.Wood - 12f) > 0.01f ||
                math.abs((float)train.LogisticsInterdictedUntil - 19f) > 0.01f ||
                math.abs((float)train.ConvoyRecoveryUntil - 31f) > 0.01f ||
                !wagonMove.IsActive ||
                math.distance(wagonMove.Destination.xz, new float2(160f, 100f)) > 0.1f ||
                support.HasLinkedSupplyCamp ||
                support.Status != SiegeSupportStatus.Interdicted ||
                !scoutMove.IsActive)
            {
                sb.AppendLine(
                    "PhaseSupplyWagonInterdiction FAIL: wagon interdiction did not land canonical effects. " +
                    $"food={stockpile.Food:F2} water={stockpile.Water:F2} wood={stockpile.Wood:F2} " +
                    $"interdictedUntil={train.LogisticsInterdictedUntil:F2} recoveryUntil={train.ConvoyRecoveryUntil:F2} " +
                    $"wagonMove=({wagonMove.Destination.x:F2},{wagonMove.Destination.z:F2}) supportStatus={support.Status} scoutRetreat={scoutMove.IsActive}");
                return false;
            }

            sb.AppendLine(
                $"PhaseSupplyWagonInterdiction PASS: interdictedUntil={train.LogisticsInterdictedUntil:F2} " +
                $"recoveryUntil={train.ConvoyRecoveryUntil:F2} wagonMove=({wagonMove.Destination.x:F2},{wagonMove.Destination.z:F2}).");
            return true;
        }

        private static void SeedMapConfig(EntityManager em)
        {
            var map = em.CreateEntity(typeof(MapBootstrapConfigComponent));
            em.SetComponentData(map, new MapBootstrapConfigComponent
            {
                TileSize = 32,
                Width = 160,
                Height = 160,
            });
        }

        private static Entity SeedFaction(
            EntityManager em,
            string factionId,
            float gold = 0f,
            float food = 0f,
            float water = 0f,
            float wood = 0f,
            float stone = 0f,
            float iron = 0f,
            float influence = 0f)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(ResourceStockpileComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(entity, new ResourceStockpileComponent
            {
                Gold = gold,
                Food = food,
                Water = water,
                Wood = wood,
                Stone = stone,
                Iron = iron,
                Influence = influence,
            });
            em.AddBuffer<HostilityComponent>(entity);
            return entity;
        }

        private static void SeedMutualHostility(EntityManager em, string sourceFactionId, string targetFactionId)
        {
            AddHostility(em, sourceFactionId, targetFactionId);
            AddHostility(em, targetFactionId, sourceFactionId);
        }

        private static void AddHostility(EntityManager em, string sourceFactionId, string targetFactionId)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>(), ComponentType.ReadOnly<HostilityComponent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (!factions[i].FactionId.Equals(new FixedString32Bytes(sourceFactionId)))
                {
                    continue;
                }

                var hostility = em.GetBuffer<HostilityComponent>(entities[i]);
                hostility.Add(new HostilityComponent { HostileFactionId = new FixedString32Bytes(targetFactionId) });
                return;
            }
        }

        private static Entity CreateRaidableBuilding(
            EntityManager em,
            string buildingId,
            string factionId,
            float3 position)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(LocalTransform),
                typeof(HealthComponent),
                typeof(BuildingTypeComponent),
                typeof(BuildingRaidStateComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(entity, new PositionComponent { Value = position });
            em.SetComponentData(entity, LocalTransform.FromPosition(position));
            em.SetComponentData(entity, new HealthComponent { Current = 400f, Max = 400f });
            em.SetComponentData(entity, new BuildingTypeComponent
            {
                TypeId = new FixedString64Bytes(buildingId),
                SupportsSiegeLogistics = string.Equals(buildingId, "supply_camp", StringComparison.OrdinalIgnoreCase),
            });
            em.SetComponentData(entity, new BuildingRaidStateComponent());
            return entity;
        }

        private static Entity CreateSupplyCamp(EntityManager em, string factionId, float3 position)
        {
            var entity = CreateRaidableBuilding(em, "supply_camp", factionId, position);
            em.AddComponentData(entity, new SiegeSupplyCampComponent
            {
                Stockpile = 100f,
                MaxStockpile = 100f,
                OperationalThreshold = 25f,
                NearbyRaiderCount = 0,
            });
            return entity;
        }

        private static Entity CreateControlPoint(EntityManager em, string factionId, float3 position, float loyalty)
        {
            var entity = em.CreateEntity(typeof(PositionComponent), typeof(ControlPointComponent));
            em.SetComponentData(entity, new PositionComponent { Value = position });
            em.SetComponentData(entity, new ControlPointComponent
            {
                ControlPointId = new FixedString32Bytes("cp-raid"),
                OwnerFactionId = new FixedString32Bytes(factionId),
                ControlState = ControlState.Stabilized,
                Loyalty = loyalty,
                RadiusTiles = 2.8f,
                CaptureTimeSeconds = 9f,
            });
            return entity;
        }

        private static Entity CreateScoutRaider(EntityManager em, string factionId, float3 position)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(LocalTransform),
                typeof(HealthComponent),
                typeof(UnitTypeComponent),
                typeof(MovementStatsComponent),
                typeof(MoveCommandComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(entity, new PositionComponent { Value = position });
            em.SetComponentData(entity, LocalTransform.FromPosition(position));
            em.SetComponentData(entity, new HealthComponent { Current = 90f, Max = 90f });
            em.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes("scout_rider"),
                Role = UnitRole.LightCavalry,
                Stage = 2,
            });
            em.SetComponentData(entity, new MovementStatsComponent { MaxSpeed = 108f });
            em.SetComponentData(entity, new MoveCommandComponent
            {
                Destination = position,
                StoppingDistance = 0.2f,
                IsActive = false,
            });
            return entity;
        }

        private static Entity CreateFieldWaterUnit(
            EntityManager em,
            string factionId,
            string unitId,
            UnitRole role,
            float3 position)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(LocalTransform),
                typeof(HealthComponent),
                typeof(UnitTypeComponent),
                typeof(FieldWaterComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = new FixedString32Bytes(factionId) });
            em.SetComponentData(entity, new PositionComponent { Value = position });
            em.SetComponentData(entity, LocalTransform.FromPosition(position));
            em.SetComponentData(entity, new HealthComponent { Current = 80f, Max = 80f });
            em.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = new FixedString64Bytes(unitId),
                Role = role,
            });
            em.SetComponentData(entity, new FieldWaterComponent
            {
                BaseAttackDamage = 9f,
                BaseMaxSpeed = 64f,
                LastTransferAt = -999d,
                LastSupportRefreshAt = -999d,
                OperationalAttackMultiplier = 1f,
                OperationalSpeedMultiplier = 1f,
                Status = FieldWaterStatus.Steady,
            });
            return entity;
        }

        private static ResourceStockpileComponent GetFactionStockpile(EntityManager em, string factionId)
        {
            var query = em.CreateEntityQuery(ComponentType.ReadOnly<FactionComponent>(), typeof(ResourceStockpileComponent));
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var factions = query.ToComponentDataArray<FactionComponent>(Allocator.Temp);
            using var stockpiles = query.ToComponentDataArray<ResourceStockpileComponent>(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                if (factions[i].FactionId.Equals(new FixedString32Bytes(factionId)))
                {
                    return stockpiles[i];
                }
            }

            throw new InvalidOperationException("Faction stockpile not found for " + factionId);
        }
    }
}
#endif
