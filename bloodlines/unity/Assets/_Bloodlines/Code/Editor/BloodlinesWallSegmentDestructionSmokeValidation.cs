#if UNITY_EDITOR
using System;
using System.IO;
using Bloodlines.Components;
using Bloodlines.Debug;
using Bloodlines.Fortification;
using Unity.Core;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace Bloodlines.EditorTools
{
    /// <summary>
    /// Dedicated smoke validator for fortification wall-segment destruction resolution.
    /// It proves that live fortification-role buildings become linked tier contributors,
    /// that destroyed wall segments open explicit breaches and drop the settlement tier,
    /// and that reserve frontage shrinks once the breached wall no longer contributes.
    /// </summary>
    public static class BloodlinesWallSegmentDestructionSmokeValidation
    {
        private const string ArtifactPath = "../artifacts/unity-wall-segment-destruction-smoke.log";
        private const float SimStepSeconds = 0.05f;

        [MenuItem("Bloodlines/Fortification/Run Wall Segment Destruction Smoke Validation")]
        public static void RunInteractive()
        {
            RunInternal(batchMode: false);
        }

        public static void RunBatchWallSegmentDestructionSmokeValidation()
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
                message = "Wall segment destruction smoke errored: " + e;
            }

            string artifact = "BLOODLINES_WALL_SEGMENT_DESTRUCTION_SMOKE " +
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
            ok &= RunLinkedWallBaselinePhase(sb);
            ok &= RunDestroyedWallBreachPhase(sb);
            ok &= RunReserveFrontageWithWallPhase(sb);
            ok &= RunReserveFrontageAfterWallLossPhase(sb);
            report = sb.ToString();
            return ok;
        }

        private static bool RunLinkedWallBaselinePhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesWallSegmentDestruction_Baseline");
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

            Tick(world, 0.1d);

            if (!scope.CommandSurface.TryDebugGetFortificationTier("keep_player", out int tier, out int ceiling) ||
                !scope.CommandSurface.TryDebugGetFortificationBreachState(
                    "keep_player",
                    out int breaches,
                    out int destroyedWalls,
                    out int destroyedTowers,
                    out int destroyedGates,
                    out int destroyedKeeps))
            {
                sb.AppendLine("Phase 1 FAIL: baseline phase could not read fortification state.");
                return false;
            }

            if (tier != 1 ||
                ceiling != 4 ||
                breaches != 0 ||
                destroyedWalls != 0 ||
                destroyedTowers != 0 ||
                destroyedGates != 0 ||
                destroyedKeeps != 0)
            {
                sb.AppendLine(
                    "Phase 1 FAIL: linked wall did not contribute a clean tier-1 baseline. " +
                    $"tier={tier} ceiling={ceiling} breaches={breaches} walls={destroyedWalls} " +
                    $"towers={destroyedTowers} gates={destroyedGates} keeps={destroyedKeeps}");
                return false;
            }

            sb.AppendLine($"Phase 1 PASS: tier={tier} ceiling={ceiling} breaches={breaches}.");
            return true;
        }

        private static bool RunDestroyedWallBreachPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesWallSegmentDestruction_Breach");
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

            Tick(world, 0.1d);
            em.SetComponentData(wall, new HealthComponent
            {
                Current = 0f,
                Max = 900f,
            });

            Tick(world, 0.1d);

            if (!scope.CommandSurface.TryDebugGetFortificationTier("keep_player", out int tier, out _) ||
                !scope.CommandSurface.TryDebugGetFortificationBreachState(
                    "keep_player",
                    out int breaches,
                    out int destroyedWalls,
                    out int destroyedTowers,
                    out int destroyedGates,
                    out int destroyedKeeps))
            {
                sb.AppendLine("Phase 2 FAIL: destroyed-wall phase could not read fortification state.");
                return false;
            }

            if (tier != 0 ||
                breaches != 1 ||
                destroyedWalls != 1 ||
                destroyedTowers != 0 ||
                destroyedGates != 0 ||
                destroyedKeeps != 0)
            {
                sb.AppendLine(
                    "Phase 2 FAIL: destroyed wall did not resolve into a breach and tier drop. " +
                    $"tier={tier} breaches={breaches} walls={destroyedWalls} " +
                    $"towers={destroyedTowers} gates={destroyedGates} keeps={destroyedKeeps}");
                return false;
            }

            sb.AppendLine($"Phase 2 PASS: tier={tier} breaches={breaches} destroyedWalls={destroyedWalls}.");
            return true;
        }

        private static bool RunReserveFrontageWithWallPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesWallSegmentDestruction_ReserveBaseline");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            var settlement = CreateSettlement(em, "keep_player", "player", ceiling: 4);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Gate,
                "gatehouse",
                new float3(3f, 0f, 0f),
                currentHealth: 1200f,
                maxHealth: 1200f);

            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(1f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Engaged);
            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(-1f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Ready);
            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(-2f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Ready);
            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(-3f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Ready);
            CreateHostileCombatant(em, "enemy", new float3(6f, 0f, 0f), currentHealth: 10f, maxHealth: 10f);

            Tick(world, 0.1d);

            if (!scope.CommandSurface.TryDebugGetFortificationTier("keep_player", out int tier, out _) ||
                !scope.CommandSurface.TryDebugForceMuster("keep_player", out int committedCount))
            {
                sb.AppendLine("Phase 3 FAIL: reserve baseline phase could not read tier or force muster.");
                return false;
            }

            if (tier != 3 || committedCount != 3)
            {
                sb.AppendLine(
                    "Phase 3 FAIL: intact wall frontage did not preserve the higher reserve commit count. " +
                    $"tier={tier} committed={committedCount}");
                return false;
            }

            sb.AppendLine($"Phase 3 PASS: tier={tier} committed={committedCount}.");
            return true;
        }

        private static bool RunReserveFrontageAfterWallLossPhase(System.Text.StringBuilder sb)
        {
            using var world = CreateValidationWorld("BloodlinesWallSegmentDestruction_ReserveBreached");
            using var scope = new DebugCommandSurfaceScope(world);
            var em = world.EntityManager;

            var settlement = CreateSettlement(em, "keep_player", "player", ceiling: 4);
            var wall = CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Wall,
                "wall_segment",
                new float3(2f, 0f, 0f),
                currentHealth: 900f,
                maxHealth: 900f);
            CreateFortificationBuilding(
                em,
                "player",
                FortificationRole.Gate,
                "gatehouse",
                new float3(3f, 0f, 0f),
                currentHealth: 1200f,
                maxHealth: 1200f);

            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(1f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Engaged);
            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(-1f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Ready);
            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(-2f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Ready);
            CreateLinkedCombatant(
                em,
                settlement,
                "keep_player",
                "player",
                new float3(-3f, 0f, 0f),
                currentHealth: 10f,
                maxHealth: 10f,
                ReserveDutyState.Ready);
            CreateHostileCombatant(em, "enemy", new float3(6f, 0f, 0f), currentHealth: 10f, maxHealth: 10f);

            Tick(world, 0.1d);
            em.SetComponentData(wall, new HealthComponent
            {
                Current = 0f,
                Max = 900f,
            });

            Tick(world, 0.1d);

            if (!scope.CommandSurface.TryDebugGetFortificationTier("keep_player", out int tier, out _) ||
                !scope.CommandSurface.TryDebugGetFortificationBreachState(
                    "keep_player",
                    out int breaches,
                    out int destroyedWalls,
                    out _,
                    out _,
                    out _) ||
                !scope.CommandSurface.TryDebugForceMuster("keep_player", out int committedCount))
            {
                sb.AppendLine("Phase 4 FAIL: breached reserve phase could not read tier/breach state or force muster.");
                return false;
            }

            if (tier != 2 || breaches != 1 || destroyedWalls != 1 || committedCount != 2)
            {
                sb.AppendLine(
                    "Phase 4 FAIL: breached wall did not reduce reserve frontage. " +
                    $"tier={tier} breaches={breaches} destroyedWalls={destroyedWalls} committed={committedCount}");
                return false;
            }

            sb.AppendLine($"Phase 4 PASS: tier={tier} breaches={breaches} committed={committedCount}.");
            return true;
        }

        private static World CreateValidationWorld(string worldName)
        {
            var world = new World(worldName);
            world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            var simulationGroup = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
            world.GetOrCreateSystemManaged<PresentationSystemGroup>();
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationStructureLinkSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<AdvanceFortificationTierSystem>());
            simulationGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FortificationDestructionResolutionSystem>());
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
            entityManager.AddComponentData(entity, new FortificationReserveComponent
            {
                MusterIntervalSeconds = FortificationCanon.ReserveMusterIntervalSeconds,
                ReserveHealPerSecond = FortificationCanon.ReserveTriageHealPerSecond,
                RetreatHealthRatio = FortificationCanon.ReserveRetreatHealthRatio,
                RecoveryHealthRatio = FortificationCanon.ReserveRecoveryHealthRatio,
                TriageRadiusTiles = FortificationCanon.TriageRadiusTiles,
                LastCommitAt = -999d,
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

        private static Entity CreateLinkedCombatant(
            EntityManager entityManager,
            Entity settlement,
            string settlementId,
            string factionId,
            float3 position,
            float currentHealth,
            float maxHealth,
            ReserveDutyState duty)
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
            entityManager.AddComponentData(entity, new FortificationCombatantTag());
            entityManager.AddComponentData(entity, new FortificationSettlementLinkComponent
            {
                SettlementEntity = settlement,
                SettlementId = settlementId,
            });
            entityManager.AddComponentData(entity, new FortificationReserveAssignmentComponent
            {
                Duty = duty,
            });
            return entity;
        }

        private static Entity CreateHostileCombatant(
            EntityManager entityManager,
            string factionId,
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
            entityManager.AddComponentData(entity, new FortificationCombatantTag());
            return entity;
        }

        private sealed class DebugCommandSurfaceScope : IDisposable
        {
            private readonly World previousDefaultWorld;
            private readonly GameObject hostObject;

            public DebugCommandSurfaceScope(World world)
            {
                previousDefaultWorld = World.DefaultGameObjectInjectionWorld;
                World.DefaultGameObjectInjectionWorld = world;
                hostObject = new GameObject("BloodlinesWallSegmentDestruction_CommandSurface")
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
