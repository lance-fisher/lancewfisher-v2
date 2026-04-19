#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Fortification;
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
    /// Dedicated smoke validator for the breach-aware assault-pressure seam.
    /// Proves that intact walls do not grant attacker tempo, open breaches grant
    /// hostile units a bounded bonus inside the settlement threat envelope, the
    /// owning defender is excluded, and multiple breaches scale the bonus while
    /// distant attackers remain unaffected.
    /// </summary>
    public static class BloodlinesBreachAssaultPressureSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-breach-assault-pressure-smoke.log";
        private const float SimStepSeconds = 0.05f;

        [MenuItem("Bloodlines/Fortification/Run Breach Assault Pressure Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchBreachAssaultPressureSmokeValidation()
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
                message = "Breach assault pressure smoke errored: " + e;
            }

            string artifact = "BLOODLINES_BREACH_ASSAULT_PRESSURE_SMOKE " +
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
            ok &= RunIntactWallBaselinePhase(sb);
            ok &= RunSingleBreachAttackerBonusPhase(sb);
            ok &= RunDefenderExclusionPhase(sb);
            ok &= RunMultiBreachScalingAndRadiusPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunIntactWallBaselinePhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachAssaultPressure_Baseline");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            CreateSettlement(em, "keep_player", "player", ceiling: 4);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var attacker = CreateUnit(
                em,
                "enemy",
                "breach_shock",
                UnitRole.Melee,
                new float3(6f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, SimStepSeconds);
            PrimeFieldWater(em, attacker, suppliedUntil: 20d);
            Tick(world, SimStepSeconds);

            if (!scope.CommandSurface.TryDebugGetFieldWaterState(attacker, out var fieldWater))
            {
                sb.AppendLine("Phase 1 FAIL: baseline phase could not read attacker field-water state.");
                return false;
            }

            var combat = em.GetComponentData<CombatStatsComponent>(attacker);
            var movement = em.GetComponentData<MovementStatsComponent>(attacker);
            if (fieldWater.BreachAssaultAdvantageActive ||
                fieldWater.BreachOpenCount != 0 ||
                !Approximately(combat.AttackDamage, 10f, 0.05f) ||
                !Approximately(movement.MaxSpeed, 5f, 0.05f))
            {
                sb.AppendLine(
                    "Phase 1 FAIL: intact wall should not grant breach assault pressure. " +
                    $"active={fieldWater.BreachAssaultAdvantageActive} openBreaches={fieldWater.BreachOpenCount} " +
                    $"attack={combat.AttackDamage:F2} speed={movement.MaxSpeed:F2}");
                return false;
            }

            sb.AppendLine("Phase 1 PASS: intact wall kept breach assault pressure inactive.");
            return true;
        }

        private static bool RunSingleBreachAttackerBonusPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachAssaultPressure_SingleBreach");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            CreateSettlement(em, "keep_player", "player", ceiling: 4);
            var wall = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var attacker = CreateUnit(
                em,
                "enemy",
                "breach_shock",
                UnitRole.Melee,
                new float3(6f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, SimStepSeconds);
            PrimeFieldWater(em, attacker, suppliedUntil: 20d);
            DestroyStructure(em, wall, maxHealth: 900f);
            Tick(world, SimStepSeconds * 2d);

            if (!scope.CommandSurface.TryDebugGetFortificationBreachState(
                    "keep_player",
                    out int openBreaches,
                    out int destroyedWalls,
                    out _,
                    out _,
                    out _) ||
                !scope.CommandSurface.TryDebugGetFieldWaterState(attacker, out var fieldWater))
            {
                sb.AppendLine("Phase 2 FAIL: single-breach phase could not read breach or attacker state.");
                return false;
            }

            var combat = em.GetComponentData<CombatStatsComponent>(attacker);
            var movement = em.GetComponentData<MovementStatsComponent>(attacker);
            float expectedAttack = 10f * SiegeSupportCanon.ResolveBreachAssaultAttackMultiplier(1);
            float expectedSpeed = 5f * SiegeSupportCanon.ResolveBreachAssaultSpeedMultiplier(1);
            if (openBreaches != 1 ||
                destroyedWalls != 1 ||
                !fieldWater.BreachAssaultAdvantageActive ||
                fieldWater.BreachOpenCount != 1 ||
                !fieldWater.BreachTargetSettlementId.Equals("keep_player") ||
                !Approximately(combat.AttackDamage, expectedAttack, 0.05f) ||
                !Approximately(movement.MaxSpeed, expectedSpeed, 0.05f))
            {
                sb.AppendLine(
                    "Phase 2 FAIL: single breach did not grant the attacker the expected pressure bonus. " +
                    $"openBreaches={openBreaches} destroyedWalls={destroyedWalls} active={fieldWater.BreachAssaultAdvantageActive} " +
                    $"breachCount={fieldWater.BreachOpenCount} target={fieldWater.BreachTargetSettlementId} " +
                    $"attack={combat.AttackDamage:F2} expectedAttack={expectedAttack:F2} " +
                    $"speed={movement.MaxSpeed:F2} expectedSpeed={expectedSpeed:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 2 PASS: breachCount={fieldWater.BreachOpenCount} attack={combat.AttackDamage:F2} speed={movement.MaxSpeed:F2}.");
            return true;
        }

        private static bool RunDefenderExclusionPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachAssaultPressure_Defender");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            CreateSettlement(em, "keep_player", "player", ceiling: 4);
            var wall = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var defender = CreateUnit(
                em,
                "player",
                "garrison",
                UnitRole.Melee,
                new float3(4f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, SimStepSeconds);
            PrimeFieldWater(em, defender, suppliedUntil: 20d);
            DestroyStructure(em, wall, maxHealth: 900f);
            Tick(world, SimStepSeconds * 2d);

            if (!scope.CommandSurface.TryDebugGetFieldWaterState(defender, out var fieldWater))
            {
                sb.AppendLine("Phase 3 FAIL: defender-exclusion phase could not read defender field-water state.");
                return false;
            }

            var combat = em.GetComponentData<CombatStatsComponent>(defender);
            var movement = em.GetComponentData<MovementStatsComponent>(defender);
            if (fieldWater.BreachAssaultAdvantageActive ||
                fieldWater.BreachOpenCount != 0 ||
                !Approximately(combat.AttackDamage, 10f, 0.05f) ||
                !Approximately(movement.MaxSpeed, 5f, 0.05f))
            {
                sb.AppendLine(
                    "Phase 3 FAIL: settlement owner should not receive the breach assault bonus. " +
                    $"active={fieldWater.BreachAssaultAdvantageActive} openBreaches={fieldWater.BreachOpenCount} " +
                    $"attack={combat.AttackDamage:F2} speed={movement.MaxSpeed:F2}");
                return false;
            }

            sb.AppendLine("Phase 3 PASS: defending owner remained excluded from breach assault pressure.");
            return true;
        }

        private static bool RunMultiBreachScalingAndRadiusPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesBreachAssaultPressure_MultiBreach");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            CreateSettlement(em, "keep_player", "player", ceiling: 4);
            var wall = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            var gate = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Gate,
                "gatehouse",
                new float3(3f, 0f, 0f),
                currentHealth: 1200f,
                maxHealth: 1200f);
            var nearAttacker = CreateUnit(
                em,
                "enemy",
                "breach_shock",
                UnitRole.Melee,
                new float3(6f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);
            var farAttacker = CreateUnit(
                em,
                "enemy",
                "rear_guard",
                UnitRole.Melee,
                new float3(FortificationCanon.ThreatRadiusTiles + 40f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                attackDamage: 10f,
                maxSpeed: 5f);

            Tick(world, SimStepSeconds);
            PrimeFieldWater(em, nearAttacker, suppliedUntil: 20d);
            PrimeFieldWater(em, farAttacker, suppliedUntil: 20d);
            DestroyStructure(em, wall, maxHealth: 900f);
            DestroyStructure(em, gate, maxHealth: 1200f);
            Tick(world, SimStepSeconds * 2d);

            if (!scope.CommandSurface.TryDebugGetFortificationBreachState(
                    "keep_player",
                    out int openBreaches,
                    out int destroyedWalls,
                    out _,
                    out int destroyedGates,
                    out _) ||
                !scope.CommandSurface.TryDebugGetFieldWaterState(nearAttacker, out var nearFieldWater) ||
                !scope.CommandSurface.TryDebugGetFieldWaterState(farAttacker, out var farFieldWater))
            {
                sb.AppendLine("Phase 4 FAIL: multi-breach phase could not read breach or attacker states.");
                return false;
            }

            float expectedNearAttack = 10f * SiegeSupportCanon.ResolveBreachAssaultAttackMultiplier(2);
            float expectedNearSpeed = 5f * SiegeSupportCanon.ResolveBreachAssaultSpeedMultiplier(2);
            var nearCombat = em.GetComponentData<CombatStatsComponent>(nearAttacker);
            var nearMovement = em.GetComponentData<MovementStatsComponent>(nearAttacker);
            var farCombat = em.GetComponentData<CombatStatsComponent>(farAttacker);
            var farMovement = em.GetComponentData<MovementStatsComponent>(farAttacker);
            if (openBreaches != 2 ||
                destroyedWalls != 1 ||
                destroyedGates != 1 ||
                !nearFieldWater.BreachAssaultAdvantageActive ||
                nearFieldWater.BreachOpenCount != 2 ||
                !Approximately(nearCombat.AttackDamage, expectedNearAttack, 0.05f) ||
                !Approximately(nearMovement.MaxSpeed, expectedNearSpeed, 0.05f) ||
                farFieldWater.BreachAssaultAdvantageActive ||
                farFieldWater.BreachOpenCount != 0 ||
                !Approximately(farCombat.AttackDamage, 10f, 0.05f) ||
                !Approximately(farMovement.MaxSpeed, 5f, 0.05f))
            {
                sb.AppendLine(
                    "Phase 4 FAIL: multi-breach scaling or radius gating drifted. " +
                    $"openBreaches={openBreaches} destroyedWalls={destroyedWalls} destroyedGates={destroyedGates} " +
                    $"nearActive={nearFieldWater.BreachAssaultAdvantageActive} nearCount={nearFieldWater.BreachOpenCount} " +
                    $"nearAttack={nearCombat.AttackDamage:F2} expectedNearAttack={expectedNearAttack:F2} " +
                    $"nearSpeed={nearMovement.MaxSpeed:F2} expectedNearSpeed={expectedNearSpeed:F2} " +
                    $"farActive={farFieldWater.BreachAssaultAdvantageActive} farAttack={farCombat.AttackDamage:F2} farSpeed={farMovement.MaxSpeed:F2}");
                return false;
            }

            sb.AppendLine(
                $"Phase 4 PASS: breachCount={nearFieldWater.BreachOpenCount} nearAttack={nearCombat.AttackDamage:F2} farAttack={farCombat.AttackDamage:F2}.");
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
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationStructureLinkSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AdvanceFortificationTierSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationDestructionResolutionSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FieldWaterSupportScanSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BreachAssaultPressureSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FieldWaterStrainSystem>());
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

        private static Entity CreateSettlement(
            EntityManager entityManager,
            string settlementId,
            string factionId,
            int ceiling)
        {
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new FactionComponent
            {
                FactionId = factionId,
            });
            entityManager.AddComponentData(entity, new PositionComponent
            {
                Value = float3.zero,
            });
            entityManager.AddComponentData(entity, new SettlementComponent
            {
                SettlementId = settlementId,
                SettlementClassId = "primary_dynastic_keep",
                FortificationTier = 0,
                FortificationCeiling = ceiling,
            });
            entityManager.AddComponentData(entity, new FortificationComponent
            {
                SettlementId = settlementId,
                Tier = 0,
                Ceiling = ceiling,
                DestroyedWallSegmentCount = 0,
                DestroyedTowerCount = 0,
                DestroyedGateCount = 0,
                DestroyedKeepCount = 0,
                OpenBreachCount = 0,
                EcosystemRadiusTiles = FortificationCanon.EcosystemRadiusTiles,
                AuraRadiusTiles = FortificationCanon.AuraRadiusTiles,
                ThreatRadiusTiles = FortificationCanon.ThreatRadiusTiles,
                ReserveRadiusTiles = FortificationCanon.ReserveRadiusTiles,
                KeepPresenceRadiusTiles = FortificationCanon.KeepPresenceRadiusTiles,
                FaithWardId = "unwarded",
                FaithWardSightBonusTiles = 0f,
                FaithWardDefenderAttackMultiplier = 1f,
                FaithWardReserveHealMultiplier = 1f,
                FaithWardReserveMusterMultiplier = 1f,
                FaithWardLoyaltyProtectionMultiplier = 1f,
                FaithWardEnemySpeedMultiplier = 1f,
                FaithWardSurgeActive = false,
            });
            return entity;
        }

        private static Entity CreateFortificationBuilding(
            EntityManager entityManager,
            string factionId,
            FortificationRole role,
            string typeId,
            float3 position,
            float currentHealth,
            float maxHealth)
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
            entityManager.AddComponentData(entity, new BuildingTypeComponent
            {
                TypeId = typeId,
                FortificationRole = role,
                StructuralDamageMultiplier = role == FortificationRole.Gate ? 0.3f : role == FortificationRole.Wall ? 0.2f : 0.1f,
                PopulationCapBonus = 0,
                BlocksPassage = role == FortificationRole.Wall,
                SupportsSiegePreparation = false,
                SupportsSiegeLogistics = false,
            });
            return entity;
        }

        private static Entity CreateUnit(
            EntityManager entityManager,
            string factionId,
            string unitId,
            UnitRole role,
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
                SiegeClass = SiegeClass.None,
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

        private static void PrimeFieldWater(EntityManager entityManager, Entity unitEntity, double suppliedUntil)
        {
            var fieldWater = entityManager.GetComponentData<FieldWaterComponent>(unitEntity);
            fieldWater.SuppliedUntil = suppliedUntil;
            fieldWater.LastTransferAt = 0d;
            fieldWater.LastSupportRefreshAt = 0d;
            fieldWater.SupportRefreshCount = 0;
            fieldWater.Status = FieldWaterStatus.Steady;
            entityManager.SetComponentData(unitEntity, fieldWater);
        }

        private static void DestroyStructure(EntityManager entityManager, Entity structureEntity, float maxHealth)
        {
            entityManager.SetComponentData(structureEntity, new HealthComponent
            {
                Current = 0f,
                Max = maxHealth,
            });
        }

        private static bool Approximately(float actual, float expected, float tolerance)
        {
            return math.abs(actual - expected) <= tolerance;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesBreachAssaultPressure_CommandSurface")
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
