#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.AI;
using Bloodlines.Components;
using Bloodlines.Siege;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for the supply-interdiction siege slice.
    /// Proves the convoy and camp states that feed AISiegeOrchestrationSystem.
    /// </summary>
    public static class BloodlinesSiegeSupplyInterdictionSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-siege-supply-interdiction-smoke.log";
        private const float SimStepSeconds = 0.05f;

        [MenuItem("Bloodlines/Siege/Run Siege Supply Interdiction Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchSiegeSupplyInterdictionSmokeValidation()
        {
            RunInternal(batchMode: true);
        }

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
                message = "Siege supply interdiction smoke errored: " + e;
            }

            string artifact = "BLOODLINES_SIEGE_SUPPLY_INTERDICTION_SMOKE " +
                              (success ? "PASS" : "FAIL") +
                              Environment.NewLine +
                              message;
            UnityDebug.Log(artifact);

            try
            {
                string logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, artifact);
            }
            catch
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
            ok &= RunBaselinePhase(sb);
            ok &= RunInterdictionPhase(sb);
            ok &= RunRecoveringUnscreenedPhase(sb);
            ok &= RunRecoveringScreenedPhase(sb);
            ok &= RunCampRecoveryPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunBaselinePhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSupplyInterdiction_Baseline");
            var em = world.EntityManager;
            var context = SeedContext(em);

            Tick(world, 1.35d);

            var camp = em.GetComponentData<SiegeSupplyCampComponent>(context.Camp);
            var train = em.GetComponentData<SiegeSupplyTrainComponent>(context.Wagon);
            var ai = em.GetComponentData<AISiegeOrchestrationComponent>(context.AIFaction);

            if (camp.NearbyRaiderCount != 0 ||
                camp.Stockpile < (SiegeSupplyInterdictionCanon.SupplyCampMaxStockpile - 0.01f) ||
                train.LogisticsInterdictedUntil > 0d ||
                train.ConvoyRecoveryUntil > 0d ||
                ai.InterdictedWagonCount != 0 ||
                ai.RecoveringWagonCount != 0 ||
                ai.Phase != SiegeOrchestrationPhase.Assaulting)
            {
                sb.AppendLine(
                    "Phase 1 FAIL: baseline logistics drifted. " +
                    $"campStockpile={camp.Stockpile:F2} raiders={camp.NearbyRaiderCount} " +
                    $"interdictedUntil={train.LogisticsInterdictedUntil:F2} recoveryUntil={train.ConvoyRecoveryUntil:F2} " +
                    $"phase={ai.Phase}");
                return false;
            }

            sb.AppendLine(
                $"Phase 1 PASS: campStockpile={camp.Stockpile:F2} phase={ai.Phase} supplyLineReady={ai.SupplyLineReady}.");
            return true;
        }

        private static bool RunInterdictionPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSupplyInterdiction_Interdicted");
            var em = world.EntityManager;
            var context = SeedContext(em);
            CreateUnit(
                em,
                "player",
                "scout_rider",
                UnitRole.LightCavalry,
                SiegeClass.None,
                new float3(9f, 0f, 0f),
                10f,
                10f,
                6f,
                7f);

            Tick(world, 0.15d);

            var camp = em.GetComponentData<SiegeSupplyCampComponent>(context.Camp);
            var train = em.GetComponentData<SiegeSupplyTrainComponent>(context.Wagon);
            var ai = em.GetComponentData<AISiegeOrchestrationComponent>(context.AIFaction);
            var stockpile = em.GetComponentData<ResourceStockpileComponent>(context.AIFaction);

            if (camp.NearbyRaiderCount <= 0 ||
                camp.Stockpile >= SiegeSupplyInterdictionCanon.SupplyCampMaxStockpile ||
                train.LogisticsInterdictedUntil <= world.Time.ElapsedTime ||
                ai.InterdictedWagonCount != 1 ||
                ai.Phase != SiegeOrchestrationPhase.SupplyInterdicted ||
                stockpile.Food >= 120f ||
                stockpile.Water >= 120f ||
                stockpile.Wood >= 120f)
            {
                sb.AppendLine(
                    "Phase 2 FAIL: active raid did not interdict convoy. " +
                    $"campStockpile={camp.Stockpile:F2} raiders={camp.NearbyRaiderCount} " +
                    $"interdictedUntil={train.LogisticsInterdictedUntil:F2} phase={ai.Phase} " +
                    $"food={stockpile.Food:F2} water={stockpile.Water:F2} wood={stockpile.Wood:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 2 PASS: campStockpile={camp.Stockpile:F2} interdictedUntil={train.LogisticsInterdictedUntil:F2} phase={ai.Phase}.");
            return true;
        }

        private static bool RunRecoveringUnscreenedPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSupplyInterdiction_RecoveringUnscreened");
            var em = world.EntityManager;
            var context = SeedContext(em);

            Tick(world, 0.05d);
            var train = em.GetComponentData<SiegeSupplyTrainComponent>(context.Wagon);
            train.LinkedCampEntity = context.Camp;
            train.LogisticsInterdictedUntil = 0d;
            train.ConvoyRecoveryUntil = world.Time.ElapsedTime + 8d;
            train.ConvoyReconsolidatedAt = -1d;
            em.SetComponentData(context.Wagon, train);

            Tick(world, 0.05d);

            train = em.GetComponentData<SiegeSupplyTrainComponent>(context.Wagon);
            var ai = em.GetComponentData<AISiegeOrchestrationComponent>(context.AIFaction);

            if (train.EscortCount != 0 ||
                train.RequiredEscortCount != SiegeSupplyInterdictionCanon.ConvoyEscortMinEscorts ||
                train.EscortScreened ||
                ai.RecoveringWagonCount != 1 ||
                ai.ConvoyRecoveringUnscreenedCount != 1 ||
                ai.Phase != SiegeOrchestrationPhase.SupplyRecoveringUnscreened)
            {
                sb.AppendLine(
                    "Phase 3 FAIL: unscreened recovery did not hold the breach. " +
                    $"escortCount={train.EscortCount} required={train.RequiredEscortCount} screened={train.EscortScreened} " +
                    $"recovering={ai.RecoveringWagonCount} unscreened={ai.ConvoyRecoveringUnscreenedCount} phase={ai.Phase}");
                return false;
            }

            sb.AppendLine(
                $"Phase 3 PASS: escortCount={train.EscortCount} required={train.RequiredEscortCount} phase={ai.Phase}.");
            return true;
        }

        private static bool RunRecoveringScreenedPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSupplyInterdiction_RecoveringScreened");
            var em = world.EntityManager;
            var context = SeedContext(em);

            CreateUnit(
                em,
                "enemy",
                "militia",
                UnitRole.Melee,
                SiegeClass.None,
                new float3(11f, 0f, 0f),
                10f,
                10f,
                6f,
                5f);
            CreateUnit(
                em,
                "enemy",
                "archer",
                UnitRole.Ranged,
                SiegeClass.None,
                new float3(12f, 0f, 0f),
                10f,
                10f,
                6f,
                5f);

            Tick(world, 0.05d);
            var train = em.GetComponentData<SiegeSupplyTrainComponent>(context.Wagon);
            train.LinkedCampEntity = context.Camp;
            train.LogisticsInterdictedUntil = 0d;
            train.ConvoyRecoveryUntil = world.Time.ElapsedTime + 8d;
            train.ConvoyReconsolidatedAt = 0d;
            em.SetComponentData(context.Wagon, train);

            Tick(world, 0.05d);

            train = em.GetComponentData<SiegeSupplyTrainComponent>(context.Wagon);
            var ai = em.GetComponentData<AISiegeOrchestrationComponent>(context.AIFaction);

            if (train.EscortCount < SiegeSupplyInterdictionCanon.ConvoyEscortMinEscorts ||
                !train.EscortScreened ||
                train.ConvoyReconsolidatedAt < 0d ||
                ai.RecoveringWagonCount != 1 ||
                ai.ConvoyRecoveringUnscreenedCount != 0 ||
                ai.Phase != SiegeOrchestrationPhase.SupplyRecoveringScreened)
            {
                sb.AppendLine(
                    "Phase 4 FAIL: screened recovery did not reconsolidate convoy. " +
                    $"escortCount={train.EscortCount} screened={train.EscortScreened} " +
                    $"reconsolidatedAt={train.ConvoyReconsolidatedAt:F2} phase={ai.Phase}");
                return false;
            }

            sb.AppendLine(
                $"Phase 4 PASS: escortCount={train.EscortCount} reconsolidatedAt={train.ConvoyReconsolidatedAt:F2} phase={ai.Phase}.");
            return true;
        }

        private static bool RunCampRecoveryPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSupplyInterdiction_CampRecovery");
            var em = world.EntityManager;
            var context = SeedContext(em);

            Tick(world, 0.05d);
            var camp = em.GetComponentData<SiegeSupplyCampComponent>(context.Camp);
            camp.Stockpile = 10f;
            camp.NearbyRaiderCount = 0;
            camp.InterdictedByFactionId = "player";
            em.SetComponentData(context.Camp, camp);

            Tick(world, 3.0d);

            camp = em.GetComponentData<SiegeSupplyCampComponent>(context.Camp);
            if (camp.Stockpile < SiegeSupplyInterdictionCanon.SupplyCampOperationalThreshold ||
                camp.InterdictedByFactionId.Length != 0 ||
                !SiegeSupplyInterdictionCanon.IsCampOperational(camp))
            {
                sb.AppendLine(
                    "Phase 5 FAIL: camp stockpile did not recover to operational state. " +
                    $"stockpile={camp.Stockpile:F2} attacker={camp.InterdictedByFactionId}");
                return false;
            }

            sb.AppendLine($"Phase 5 PASS: campStockpile={camp.Stockpile:F2} recoveredToOperational=True.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SiegeComponentInitializationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SiegeSupplyInterdictionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SiegeSupportRefreshSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AISiegeOrchestrationSystem>());
            simulationGroup.SortSystems();
            return world;
        }

        private static void Tick(World world, double seconds)
        {
            double elapsed = world.Time.ElapsedTime;
            double target = elapsed + seconds;
            while (elapsed < target)
            {
                world.SetTime(new TimeData(elapsed, SimStepSeconds));
                world.Update();
                elapsed += SimStepSeconds;
            }
        }

        private static SeededContext SeedContext(EntityManager em)
        {
            var playerFaction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(ResourceStockpileComponent));
            em.SetComponentData(playerFaction, new FactionComponent { FactionId = "player" });
            em.SetComponentData(playerFaction, new ResourceStockpileComponent
            {
                Food = 120f,
                Water = 120f,
                Wood = 120f,
            });

            var aiFaction = em.CreateEntity(
                typeof(FactionComponent),
                typeof(ResourceStockpileComponent),
                typeof(AIStrategyComponent),
                typeof(AISiegeOrchestrationComponent));
            em.SetComponentData(aiFaction, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(aiFaction, new ResourceStockpileComponent
            {
                Food = 120f,
                Water = 120f,
                Wood = 120f,
            });
            em.SetComponentData(aiFaction, new AIStrategyComponent
            {
                PlayerKeepFortified = true,
                WorkerGatherIntervalSeconds = 5f,
                AttackTimer = 0.001f,
            });
            em.SetComponentData(aiFaction, new AISiegeOrchestrationComponent
            {
                ArmyCount = 5,
                EnemyHasSiegeUnit = true,
                EngineeringReady = true,
                EscortArmyCount = 5,
                FormalSiegeLinesFormed = true,
            });

            var camp = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(BuildingTypeComponent));
            em.SetComponentData(camp, new FactionComponent { FactionId = "enemy" });
            em.SetComponentData(camp, new PositionComponent { Value = float3.zero });
            em.SetComponentData(camp, new HealthComponent { Current = 520f, Max = 520f });
            em.SetComponentData(camp, new BuildingTypeComponent
            {
                TypeId = "supply_camp",
                SupportsSiegeLogistics = true,
            });

            var wagon = CreateUnit(
                em,
                "enemy",
                "supply_wagon",
                UnitRole.Support,
                SiegeClass.None,
                new float3(10f, 0f, 0f),
                200f,
                200f,
                0f,
                4f);

            CreateUnit(
                em,
                "enemy",
                "trebuchet",
                UnitRole.SiegeEngine,
                SiegeClass.Trebuchet,
                new float3(12f, 0f, 0f),
                200f,
                200f,
                30f,
                3f);

            return new SeededContext
            {
                AIFaction = aiFaction,
                Camp = camp,
                Wagon = wagon,
            };
        }

        private static Entity CreateUnit(
            EntityManager em,
            string factionId,
            string unitId,
            UnitRole role,
            SiegeClass siegeClass,
            float3 position,
            float currentHealth,
            float maxHealth,
            float attackDamage,
            float maxSpeed)
        {
            var entity = em.CreateEntity(
                typeof(FactionComponent),
                typeof(PositionComponent),
                typeof(HealthComponent),
                typeof(UnitTypeComponent),
                typeof(CombatStatsComponent),
                typeof(MovementStatsComponent));
            em.SetComponentData(entity, new FactionComponent { FactionId = factionId });
            em.SetComponentData(entity, new PositionComponent { Value = position });
            em.SetComponentData(entity, new HealthComponent { Current = currentHealth, Max = maxHealth });
            em.SetComponentData(entity, new UnitTypeComponent
            {
                TypeId = unitId,
                Role = role,
                SiegeClass = siegeClass,
                PopulationCost = 1,
                Stage = 2,
            });
            em.SetComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = attackDamage,
                AttackRange = 18f,
                AttackCooldown = 1f,
                Sight = 120f,
            });
            em.SetComponentData(entity, new MovementStatsComponent
            {
                MaxSpeed = maxSpeed,
            });
            return entity;
        }

        private struct SeededContext
        {
            public Entity AIFaction;
            public Entity Camp;
            public Entity Wagon;
        }
    }
}
#endif
