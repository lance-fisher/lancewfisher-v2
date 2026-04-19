#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
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
    /// Governed siege logistics smoke validator. Runs in isolated ECS worlds and proves:
    ///
    ///   1. A seeded steady unit does not accumulate strain or attrition.
    ///   2. An unsupported field unit crosses the canonical strain threshold and
    ///      receives the correct attack / speed penalties.
    ///   3. A strained field unit recovers inside support radius at the canonical rate
    ///      and lifts its operational penalties.
    ///   4. A siege engine beside an engineer, supply wagon, and live supply camp
    ///      receives refreshed engineer + supply-train support on the 1.25 second cadence.
    ///
    /// Artifact: artifacts/unity-siege-smoke.log.
    /// </summary>
    public static class BloodlinesSiegeSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-siege-smoke.log";
        private const float SimStepSeconds = 0.05f;

        [MenuItem("Bloodlines/Siege/Run Siege Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchSiegeSmokeValidation()
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
                message = "Siege smoke validation errored: " + e;
            }

            WriteResult(batchMode, success, message);
        }

        private static bool RunAllPhases(out string message)
        {
            if (!RunBaselinePhase(out string baselineMessage))
            {
                message = baselineMessage;
                return false;
            }

            if (!RunStrainPhase(out string strainMessage))
            {
                message = strainMessage;
                return false;
            }

            if (!RunRecoveryPhase(out string recoveryMessage))
            {
                message = recoveryMessage;
                return false;
            }

            if (!RunSiegeSupportPhase(out string supportMessage))
            {
                message = supportMessage;
                return false;
            }

            message =
                "Siege smoke validation passed: baselinePhase=True, strainPhase=True, recoveryPhase=True, supportPhase=True. " +
                baselineMessage + " " + strainMessage + " " + recoveryMessage + " " + supportMessage;
            return true;
        }

        private static bool RunBaselinePhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSmokeValidation_Baseline");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionRuntime(entityManager, "player", food: 50f, water: 50f, wood: 50f);
            var unit = CreateUnit(
                entityManager,
                "militia",
                UnitRole.Melee,
                SiegeClass.None,
                "player",
                position: float3.zero,
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, seconds: SimStepSeconds);
            var fieldWater = entityManager.GetComponentData<FieldWaterComponent>(unit);
            fieldWater.SuppliedUntil = 10d;
            fieldWater.LastTransferAt = 0d;
            fieldWater.LastSupportRefreshAt = -999d;
            fieldWater.SupportRefreshCount = 0;
            fieldWater.Status = FieldWaterStatus.Steady;
            entityManager.SetComponentData(unit, fieldWater);

            Tick(world, seconds: 0.5d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFieldWaterState(unit, out fieldWater))
            {
                message = "Siege smoke validation failed: baseline phase could not read field-water state.";
                return false;
            }

            if (fieldWater.Strain > 0.01f ||
                fieldWater.AttritionActive ||
                fieldWater.DesertionRisk ||
                fieldWater.SupportRefreshCount != 0 ||
                fieldWater.Status != FieldWaterStatus.Steady)
            {
                message =
                    "Siege smoke validation failed: baseline phase drifted. " +
                    "strain=" + fieldWater.Strain.ToString("0.00") +
                    ", attrition=" + fieldWater.AttritionActive +
                    ", desertionRisk=" + fieldWater.DesertionRisk +
                    ", supportRefreshCount=" + fieldWater.SupportRefreshCount +
                    ", status=" + fieldWater.Status + ".";
                return false;
            }

            message = "Baseline: strain=0, attrition=False, supportRefreshCount=0.";
            return true;
        }

        private static bool RunStrainPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSmokeValidation_Strain");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionRuntime(entityManager, "player", food: 50f, water: 50f, wood: 50f);
            var unit = CreateUnit(
                entityManager,
                "militia",
                UnitRole.Melee,
                SiegeClass.None,
                "player",
                position: new float3(320f, 0f, 320f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            double strainSeconds = (FieldWaterCanon.FieldWaterStrainThreshold / FieldWaterCanon.FieldWaterStrainPerSecond) + 0.25f;
            Tick(world, seconds: strainSeconds);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFieldWaterState(unit, out var fieldWater))
            {
                message = "Siege smoke validation failed: strain phase could not read field-water state.";
                return false;
            }

            var combat = entityManager.GetComponentData<CombatStatsComponent>(unit);
            var movement = entityManager.GetComponentData<MovementStatsComponent>(unit);
            float expectedAttack = 10f * FieldWaterCanon.FieldWaterStrainAttackMultiplier;
            float expectedSpeed = 5f * FieldWaterCanon.FieldWaterStrainSpeedMultiplier;
            if (fieldWater.Strain < FieldWaterCanon.FieldWaterStrainThreshold ||
                fieldWater.Status != FieldWaterStatus.Strained ||
                !Approximately(combat.AttackDamage, expectedAttack, 0.05f) ||
                !Approximately(movement.MaxSpeed, expectedSpeed, 0.05f))
            {
                message =
                    "Siege smoke validation failed: strain phase did not apply canonical penalties. " +
                    "strain=" + fieldWater.Strain.ToString("0.00") +
                    ", status=" + fieldWater.Status +
                    ", attack=" + combat.AttackDamage.ToString("0.00") +
                    ", speed=" + movement.MaxSpeed.ToString("0.00") + ".";
                return false;
            }

            message =
                "Strain: status=" + fieldWater.Status +
                ", strain=" + fieldWater.Strain.ToString("0.00") +
                ", attackMultiplier=" + (combat.AttackDamage / 10f).ToString("0.00") +
                ", speedMultiplier=" + (movement.MaxSpeed / 5f).ToString("0.00") + ".";
            return true;
        }

        private static bool RunRecoveryPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSmokeValidation_Recovery");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionRuntime(entityManager, "player", food: 50f, water: 50f, wood: 50f);
            CreateSettlement(entityManager, "keep_player", "player", float3.zero);
            var unit = CreateUnit(
                entityManager,
                "militia",
                UnitRole.Melee,
                SiegeClass.None,
                "player",
                position: new float3(4f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, seconds: SimStepSeconds);
            var fieldWater = entityManager.GetComponentData<FieldWaterComponent>(unit);
            fieldWater.Strain = 7f;
            fieldWater.Status = FieldWaterStatus.Strained;
            fieldWater.SuppliedUntil = 0d;
            entityManager.SetComponentData(unit, fieldWater);

            const double recoverySeconds = 2.5d;
            Tick(world, seconds: recoverySeconds);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFieldWaterState(unit, out fieldWater))
            {
                message = "Siege smoke validation failed: recovery phase could not read field-water state.";
                return false;
            }

            var combat = entityManager.GetComponentData<CombatStatsComponent>(unit);
            var movement = entityManager.GetComponentData<MovementStatsComponent>(unit);
            float expectedStrain = math.max(
                0f,
                7f - (float)recoverySeconds * FieldWaterCanon.FieldWaterRecoveryPerSecond);
            if (!fieldWater.IsSupported ||
                fieldWater.Strain >= FieldWaterCanon.FieldWaterStrainThreshold ||
                !Approximately(fieldWater.Strain, expectedStrain, 0.2f) ||
                combat.AttackDamage < 9.9f ||
                movement.MaxSpeed < 4.9f)
            {
                message =
                    "Siege smoke validation failed: recovery phase did not clear penalties. " +
                    "supported=" + fieldWater.IsSupported +
                    ", strain=" + fieldWater.Strain.ToString("0.00") +
                    ", attack=" + combat.AttackDamage.ToString("0.00") +
                    ", speed=" + movement.MaxSpeed.ToString("0.00") + ".";
                return false;
            }

            message =
                "Recovery: supported=True, strain=" + fieldWater.Strain.ToString("0.00") +
                ", supportRefreshCount=" + fieldWater.SupportRefreshCount + ".";
            return true;
        }

        private static bool RunSiegeSupportPhase(out string message)
        {
            using var world = CreateValidationWorld("BloodlinesSiegeSmokeValidation_Support");
            using var commandSurfaceScope = new DebugCommandSurfaceScope(world);
            var entityManager = world.EntityManager;

            CreateFactionRuntime(entityManager, "player", food: 100f, water: 100f, wood: 100f);
            CreateFactionRuntime(entityManager, "enemy", food: 50f, water: 50f, wood: 50f);
            CreateSettlement(entityManager, "enemy_keep", "enemy", new float3(24f, 0f, 0f));
            CreateBuilding(entityManager, "supply_camp", "player", float3.zero, supportsSiegeLogistics: true);
            var engine = CreateUnit(
                entityManager,
                "trebuchet",
                UnitRole.SiegeEngine,
                SiegeClass.Trebuchet,
                "player",
                position: new float3(14f, 0f, 0f),
                currentHealth: 280f,
                maxHealth: 320f,
                attackDamage: 38f,
                maxSpeed: 20f);
            CreateUnit(
                entityManager,
                "siege_engineer",
                UnitRole.EngineerSpecialist,
                SiegeClass.None,
                "player",
                position: new float3(12f, 0f, 0f),
                currentHealth: 72f,
                maxHealth: 72f,
                attackDamage: 4f,
                maxSpeed: 56f);
            var wagon = CreateUnit(
                entityManager,
                "supply_wagon",
                UnitRole.Support,
                SiegeClass.None,
                "player",
                position: new float3(10f, 0f, 0f),
                currentHealth: 240f,
                maxHealth: 240f,
                attackDamage: 0f,
                maxSpeed: 42f);

            Tick(world, seconds: 1.35d);

            if (!commandSurfaceScope.CommandSurface.TryDebugGetSiegeSupportState(engine, out var engineSupport))
            {
                message = "Siege smoke validation failed: support phase could not read siege-engine support state.";
                return false;
            }

            if (!commandSurfaceScope.CommandSurface.TryDebugGetSiegeSupportState(wagon, out var wagonSupport))
            {
                message = "Siege smoke validation failed: support phase could not read supply-wagon support state.";
                return false;
            }

            if (!commandSurfaceScope.CommandSurface.TryDebugGetFactionStockpile("player", out var stockpile))
            {
                message = "Siege smoke validation failed: support phase could not read faction stockpile.";
                return false;
            }

            double elapsed = world.Time.ElapsedTime;
            if (!engineSupport.HasEngineerSupport ||
                !engineSupport.HasSupplyTrainSupport ||
                engineSupport.RefreshCount <= 0 ||
                engineSupport.EngineerSupportUntil <= elapsed ||
                engineSupport.SuppliedUntil <= elapsed ||
                !wagonSupport.HasLinkedSupplyCamp ||
                stockpile.Food >= 100f ||
                stockpile.Water >= 100f ||
                stockpile.Wood >= 100f)
            {
                message =
                    "Siege smoke validation failed: support phase did not refresh canonical siege support. " +
                    "engineerSupport=" + engineSupport.HasEngineerSupport +
                    ", supplySupport=" + engineSupport.HasSupplyTrainSupport +
                    ", refreshCount=" + engineSupport.RefreshCount +
                    ", wagonLinked=" + wagonSupport.HasLinkedSupplyCamp +
                    ", stockpileFood=" + stockpile.Food.ToString("0.00") +
                    ", stockpileWater=" + stockpile.Water.ToString("0.00") +
                    ", stockpileWood=" + stockpile.Wood.ToString("0.00") + ".";
                return false;
            }

            message =
                "Support: engineerSupport=True, supplySupport=True, refreshCount=" + engineSupport.RefreshCount +
                ", suppliedUntil=" + engineSupport.SuppliedUntil.ToString("0.00") + ".";
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SiegeComponentInitializationSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SiegeSupportRefreshSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FieldWaterSupportScanSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BreachAssaultPressureSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FieldWaterStrainSystem>());
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

        private static Entity CreateFactionRuntime(
            EntityManager entityManager,
            string factionId,
            float food,
            float water,
            float wood)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new ResourceStockpileComponent
            {
                Food = food,
                Water = water,
                Wood = wood,
            });
            return entity;
        }

        private static Entity CreateSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            float3 position)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.AddComponentData(entity, new SettlementComponent
            {
                SettlementId = settlementId,
                SettlementClassId = "primary_dynastic_keep",
                FortificationTier = 0,
                FortificationCeiling = 3,
            });
            return entity;
        }

        private static Entity CreateBuilding(
            EntityManager entityManager,
            string buildingId,
            string factionId,
            float3 position,
            bool supportsSiegeLogistics)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = 520f,
                Max = 520f,
            });
            entityManager.AddComponentData(entity, new BuildingTypeComponent
            {
                TypeId = buildingId,
                SupportsSiegeLogistics = supportsSiegeLogistics,
            });
            return entity;
        }

        private static Entity CreateUnit(
            EntityManager entityManager,
            string unitId,
            UnitRole role,
            SiegeClass siegeClass,
            string factionId,
            float3 position,
            float currentHealth,
            float maxHealth,
            float attackDamage,
            float maxSpeed)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = position,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                Current = currentHealth,
                Max = maxHealth,
            });
            entityManager.AddComponentData(entity, new UnitTypeComponent
            {
                TypeId = unitId,
                Role = role,
                SiegeClass = siegeClass,
                PopulationCost = 1,
                Stage = 2,
            });
            entityManager.AddComponentData(entity, new CombatStatsComponent
            {
                AttackDamage = attackDamage,
                AttackRange = 18f,
                AttackCooldown = 1f,
                Sight = 120f,
            });
            entityManager.AddComponentData(entity, new MovementStatsComponent
            {
                MaxSpeed = maxSpeed,
            });
            return entity;
        }

        private static bool Approximately(float actual, float expected, float tolerance)
        {
            return math.abs(actual - expected) <= tolerance;
        }

        private static void WriteResult(bool batchMode, bool success, string message)
        {
            try
            {
                var logPath = Path.GetFullPath(Path.Combine(Application.dataPath, ArtifactPath));
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.AppendAllText(logPath, message + Environment.NewLine);
            }
            catch
            {
            }

            UnityDebug.Log(message);
            if (batchMode)
            {
                EditorApplication.Exit(success ? 0 : 1);
            }
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesSiegeSmokeValidation_CommandSurface")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                };
                CommandSurface = hostObject.AddComponent<BloodlinesDebugCommandSurface>();
            }

            public BloodlinesDebugCommandSurface CommandSurface { get; }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(hostObject);
                World.DefaultGameObjectInjectionWorld = previousDefaultWorld;
            }
        }
    }
}
#endif
